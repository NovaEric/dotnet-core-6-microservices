using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string suscriptionEmail;
        private readonly string orderUpdatePaymentResultTopic;

        private readonly EmailRepository _emailRepository;

        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

        private readonly IConfiguration _configuration;        

        public AzureServiceBusConsumer(EmailRepository emailRepository, IConfiguration configuration)
        {
            _emailRepository    = emailRepository;
            _configuration      = configuration;            

            serviceBusConnectionString      = _configuration.GetValue<string>("ServiceBusConnectionString");
            suscriptionEmail                = _configuration.GetValue<string>("SubscriptionName");     
            orderUpdatePaymentResultTopic   = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            orderUpdatePaymentStatusProcessor   = client.CreateProcessor(orderUpdatePaymentResultTopic, suscriptionEmail);
        }

        public async Task Start()
        {
            orderUpdatePaymentStatusProcessor.ProcessMessageAsync   += OnOrderPaymentUpdateRecieved;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync     += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }
         public async Task Stop()
        {            
            await orderUpdatePaymentStatusProcessor.StopProcessingAsync(); //Optional if using Dispose
            await orderUpdatePaymentStatusProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnOrderPaymentUpdateRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var objMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            
            try
            {
                await _emailRepository.SendAndLogEmail(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }      
    }
}
