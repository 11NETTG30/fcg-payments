namespace Application.Events;

public class PaymentProcessedEvent
{
    public Guid PaymentId { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}
