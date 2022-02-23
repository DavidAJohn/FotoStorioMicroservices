using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Common
{
    public class EventBusConstants
    {
        public const string PaymentReceivedQueue = "payment-received-queue";
        public const string InventoryZeroQueue = "inventory-zero-queue";
        public const string InventoryRestoredQueue = "inventory-restored-queue";
    }
}
