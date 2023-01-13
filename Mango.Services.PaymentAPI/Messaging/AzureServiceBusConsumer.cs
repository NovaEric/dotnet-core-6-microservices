using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string suscriptionPayment;
        private readonly string orderPaymentProcessTopic;
        private readonly string orderUpdatePaymentResultTopic;
        
        private ServiceBusProcessor orderPaymentProcessor;

        private readonly IProcessPayment _processPayment;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(IProcessPayment processPayment, IConfiguration configuration, IMessageBus messageBus)
        {
            _processPayment = processPayment;
            _configuration  = configuration;
            _messageBus     = messageBus;

            serviceBusConnectionString      = _configuration.GetValue<string>("ServiceBusConnectionString");
            suscriptionPayment              = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
            orderUpdatePaymentResultTopic   = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");            
            orderPaymentProcessTopic        = _configuration.GetValue<string>("OrderPaymentProcessTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            orderPaymentProcessor = client.CreateProcessor(orderPaymentProcessTopic, suscriptionPayment);
        }

        public async Task Start()
        {
            orderPaymentProcessor.ProcessMessageAsync   += ProcessPayments;
            orderPaymentProcessor.ProcessErrorAsync     += ErrorHandler;
            await orderPaymentProcessor.StartProcessingAsync();
        }
         public async Task Stop()
        {
            await orderPaymentProcessor.StopProcessingAsync(); //Optional if using Dispose
            await orderPaymentProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task ProcessPayments(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

            var result = _processPayment.PaymentProcessor();

            var updatePaymentResultMessage = new UpdatePaymentResultMessage()
            {
                Status  = result,
                OrderId = paymentRequestMessage.OrderId,
                Email   = paymentRequestMessage.Email,
            };

            try
            {
                await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
