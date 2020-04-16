using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FleetwoodMac_Personel.Facade.Persistence.Transactions
{
    public class Operation
    {
        public ForwardOperation Forward { get; private set; }

        public BackwardOperation Backward { get; private set; }

        public Operation(Func<object, object> taskForward, object forwardState, Action<object> backward)
        {
            var guid = Guid.NewGuid();

            Forward = new ForwardOperation() { OperationId = guid, Delegate = taskForward, State = forwardState };
            Backward = new BackwardOperation() { OperationId = guid, Delegate = backward };
        }

        public void SetId(Guid id)
        {
            Forward.OperationId = id;
            Backward.OperationId = id;
        }

        public Guid OperationId => Forward.OperationId;
    }
}
