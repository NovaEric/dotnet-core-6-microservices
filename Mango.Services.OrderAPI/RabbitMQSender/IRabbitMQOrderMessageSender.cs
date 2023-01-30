using Mango.MessageBus;

namespace Mango.Services.OrderAPI.RabbitMQSender
{
    public interface IRabbitMQOrderMessageSender
    {
        public void SendMessage(BaseMessage baseMessage, string queueName);
    }
}
