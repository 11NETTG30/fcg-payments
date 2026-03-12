namespace Application.Interfaces.Interfaces.Repositories;

public interface IProcessedEventRepository
{
    Task SaveAsync(Guid userId, Guid gameId);
}
