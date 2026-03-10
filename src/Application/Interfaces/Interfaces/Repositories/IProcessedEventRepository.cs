namespace Application.Interfaces.Interfaces.Repositories;

public interface IProcessedEventRepository
{
    Task<bool> ExistsAsync(Guid orderId);

    Task SaveAsync(Guid orderId);
}
