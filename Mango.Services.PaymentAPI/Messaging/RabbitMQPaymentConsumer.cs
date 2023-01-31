using Mango.Services.PaymentAPI.Messages;
using Mango.Services.PaymentAPI.RabbitMQSender;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly IRabbitMQPaymentMessageSender _rabbitMQPaymentMessageSender;
        private readonly IProcessPayment _processPayment;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQPaymentConsumer(IRabbitMQPaymentMessageSender rabbitMQPaymentMessageSender, IProcessPayment processPayment)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "orderpaymentprocesstopic", false, false, false, arguments: null);
            _rabbitMQPaymentMessageSender = rabbitMQPaymentMessageSender;
            _processPayment = processPayment;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, eventConsumer) =>
            {
                var content = Encoding.UTF8.GetString(eventConsumer.Body.ToArray());
                var paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);
                HandleMessage(paymentRequestMessage).GetAwaiter().GetResult();

                _channel.BasicAck(eventConsumer.DeliveryTag, false);
            };

            _channel.BasicConsume("orderpaymentprocesstopic", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentRequestMessage paymentRequestMessage)
        {
            var result = _processPayment.PaymentProcessor();

            var updatePaymentResultMessage = new UpdatePaymentResultMessage()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email,
            };

            try
            {
                _rabbitMQPaymentMessageSender.SendMessage(updatePaymentResultMessage);
                //await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
                //await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
