namespace Infrastructure.Persistence.Entities;

public class ProcessedEventEntity
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public DateTime ProcessedAt {  get; set; }
}
