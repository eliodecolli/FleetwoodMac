using System;

namespace FleetwoodMac_Personel.Facade.Persistence.Transactions
{
    public class OperationCompletedEvent
    {
        public OperationCompletedEvent()
        {
        }

        public Guid OperationId { get; set; }

        public object State { get; set; }
    }
}