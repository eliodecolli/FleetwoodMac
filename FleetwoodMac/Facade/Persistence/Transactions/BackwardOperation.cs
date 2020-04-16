using System;
using System.Collections.Generic;
using System.Text;

namespace FleetwoodMac_Personel.Facade.Persistence.Transactions
{
    public class BackwardOperation
    {
        public Guid OperationId { get; set; }

        public Action<object> Delegate { get; set; }
    }
}
