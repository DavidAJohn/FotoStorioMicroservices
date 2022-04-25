namespace EventBus.Messages.Events;

public class InventoryRestoredEvent : IntegrationBaseEvent
{
    public string Sku { get; set; }
}
