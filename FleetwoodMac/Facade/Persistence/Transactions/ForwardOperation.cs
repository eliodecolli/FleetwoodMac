using System;
using System.Threading.Tasks;
namespace FleetwoodMac_Personel.Facade.Persistence.Transactions
{
    public class ForwardOperation
    {
        public Guid OperationId { get; set; }

        public Func<object, object> Delegate { get; set; }

        public object State { get; set; }
    }
}
