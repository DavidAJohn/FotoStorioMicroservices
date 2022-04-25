namespace EventBus.Messages.Events;

public class InventoryZeroEvent : IntegrationBaseEvent
{
    public string Sku { get; set; }
}
