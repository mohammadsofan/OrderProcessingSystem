namespace OrderProcessingSystem.Api.Interfaces.IService
{
    public interface IMessageBrokerService
    {
        Task SendMessage<T>(T message);
    }
}
