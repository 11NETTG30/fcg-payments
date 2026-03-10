namespace Infrastructure.Persistence.Entities;

public class ProcessedEventEntity
{
    public Guid OrderId { get; set; }
    public DateTime ProcessedAt {  get; set; }
}
