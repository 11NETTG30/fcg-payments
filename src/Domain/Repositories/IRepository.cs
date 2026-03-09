using Domain.Entities;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IRepository<T> : IDisposable where T : Entity, IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}