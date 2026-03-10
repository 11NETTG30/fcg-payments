namespace Application.Interfaces.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T message, string queueName);
}