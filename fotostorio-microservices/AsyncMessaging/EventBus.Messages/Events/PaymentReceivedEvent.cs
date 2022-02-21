using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class PaymentReceivedEvent : IntegrationBaseEvent
    {
        public string Sku { get; set; }
        public int AmountOrdered { get; set; }
        public int OrderId { get; set; }
    }
}
