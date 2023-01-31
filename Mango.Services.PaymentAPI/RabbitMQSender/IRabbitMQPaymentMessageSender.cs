using Mango.MessageBus;

namespace Mango.Services.PaymentAPI.RabbitMQSender
{
    public interface IRabbitMQPaymentMessageSender
    {
        public void SendMessage(BaseMessage baseMessage);
    }
}
