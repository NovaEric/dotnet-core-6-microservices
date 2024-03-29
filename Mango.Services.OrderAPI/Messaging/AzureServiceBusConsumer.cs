﻿using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string suscriptionCheckOut;
        private readonly string checkoutMessageTopic;
        private readonly string orderPaymentProcessTopic;
        private readonly string orderUpdatePaymentResultTopic;

        private readonly OrderRepository _orderRepository;

        private ServiceBusProcessor checkOutProcessor;
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;

            serviceBusConnectionString      = _configuration.GetValue<string>("ServiceBusConnectionString");
            suscriptionCheckOut             = _configuration.GetValue<string>("SuscriptionCheckOut");
            checkoutMessageTopic            = _configuration.GetValue<string>("CheckoutMessageTopic");
            orderPaymentProcessTopic        = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            orderUpdatePaymentResultTopic   = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);

            //checkOutProcessor                   = client.CreateProcessor(checkoutMessageTopic, suscriptionCheckOut);
            checkOutProcessor = client.CreateProcessor(checkoutMessageTopic);
            orderUpdatePaymentStatusProcessor   = client.CreateProcessor(orderUpdatePaymentResultTopic, suscriptionCheckOut);
        }

        public async Task Start()
        {
            checkOutProcessor.ProcessMessageAsync   += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync     += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();

            orderUpdatePaymentStatusProcessor.ProcessMessageAsync   += OnOrderPaymentUpdateRecieved;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync     += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        }
         public async Task Stop()
        {
            await checkOutProcessor.StopProcessingAsync(); //Optional if using Dispose
            await checkOutProcessor.DisposeAsync();

            await orderUpdatePaymentStatusProcessor.StopProcessingAsync(); //Optional if using Dispose
            await orderUpdatePaymentStatusProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = new()
            {
                UserId          = checkoutHeaderDto.UserId,
                FirstName       = checkoutHeaderDto.FirstName,
                LastName        = checkoutHeaderDto.LastName,
                OrderDetails    = new List<OrderDetails>(),
                CardNumber      = checkoutHeaderDto.CardNumber,
                CouponCode      = checkoutHeaderDto.CouponCode,
                CVV             = checkoutHeaderDto.CVV,
                DiscountTotal   = checkoutHeaderDto.DiscountTotal,
                Email           = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime       = DateTime.Now,
                OrderTotal      = checkoutHeaderDto.OrderTotal,
                PaymentStatus   = false,
                Phone           = checkoutHeaderDto.Phone,
                PickDateTime    = checkoutHeaderDto.PickDateTime,
            };

            foreach (var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId       = detailList.ProductId,
                    ProductName     = detailList.Product.ProductName,
                    Price           = detailList.Product.ProductPrice,
                    ProductCount    = detailList.ProductCount
                };

                orderHeader.CartTotalItems += detailList.ProductCount;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await _orderRepository.AddOrder(orderHeader);

            var paymentRequestMessage = new PaymentRequestMessage()
            {
                Name            = $"{orderHeader.FirstName} {orderHeader.LastName}",
                CardNumber      = orderHeader.CardNumber,
                CVV             = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId         = orderHeader.OrderHeaderId,
                OrderTotal      = orderHeader.OrderTotal,
                Email           = orderHeader.Email,
            };

            try
            {
                await _messageBus.PublishMessage(paymentRequestMessage, orderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnOrderPaymentUpdateRecieved(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            
            await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
