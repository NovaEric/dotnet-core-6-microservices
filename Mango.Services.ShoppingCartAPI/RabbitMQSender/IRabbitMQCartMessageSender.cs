using Mango.MessageBus;

namespace Mango.Services.ShoppingCartAPI.RabbitMQSender
{
    public interface IRabbitMQCartMessageSender
    {
        public void SendMessage(BaseMessage baseMessage, string queueName);
    }
}
