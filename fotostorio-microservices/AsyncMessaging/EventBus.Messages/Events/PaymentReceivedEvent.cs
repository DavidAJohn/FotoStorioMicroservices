namespace EventBus.Messages.Events;

public class PaymentReceivedEvent : IntegrationBaseEvent
{
    public string Sku { get; set; }
    public int QuantityOrdered { get; set; }
    public int OrderId { get; set; }
}
