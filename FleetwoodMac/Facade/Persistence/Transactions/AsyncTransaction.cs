using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Threading;

namespace FleetwoodMac_Personel.Facade.Persistence.Transactions
{
    public class AsyncTransaction
    {
        public event Action<OperationCompletedEvent> OperationCompleted;

        public readonly List<Operation> operations;

        public AsyncTransaction()
        {
            states = new Dictionary<Guid, object>();
            operations = new List<Operation>();
        }

        private Dictionary<Guid, object> states;

        public void AddOperation(Operation operation)
        {
            operations.Add(operation);
        }

        public async Task Commit()
        {
            for (int i = 0; i < operations.Count; i++)
            {
                var main = operations[i];
                var op = main.Forward;

                var state = states.ContainsKey(main.OperationId) ? states[main.OperationId] : op.State;

                // we retrieve the first object state here
                var obj = await Task.Run(() => op.Delegate.Invoke(state));

                if (OperationCompleted != null)
                {
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        OperationCompleted(new OperationCompletedEvent() { OperationId = op.OperationId, State = obj });
                    });
                }

                if(!states.ContainsKey(main.OperationId))
                    states.Add(main.OperationId, obj);
                else
                    states[main.OperationId] = obj;
                
                Log.Info($" [x] Commited operation {main.OperationId}");
            }
            Log.Info(" [x] Commited async transaction");
        }

        public async Task Rollback()
        {
            for (int i = 0; i < operations.Count; i++)
            {
                var main = operations[i];
                var func = main.Backward;

                var state = states[main.OperationId];

                await Task.Run(() =>
                {
                    func.Delegate.Invoke(state);
                });

                Log.Info($" [x] Rolling back operation {main.OperationId}");
            }
        }
    }
}
