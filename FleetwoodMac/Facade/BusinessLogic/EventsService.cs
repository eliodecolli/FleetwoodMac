using System;
using System.Collections.Generic;
using FleetwoodMac_Personel.Facade.Persistence.Mongo.Repository;
using FleetwoodMac_Personel.Facade.Persistence.Transactions;
using System.Threading.Tasks;
using FleetwoodMac_Personel.Facade.Persistence.Neo4J;
using FleetwoodMac_Personel.Facade.Persistence.Neo4J.Entities;

namespace FleetwoodMac_Personel.Facade.BusinessLogic
{
    public class EventsService : IDisposable
    {
        private readonly NeoConnection neo;
        private readonly MongoRepository mongoRepository;

        public EventsService()
        {
            neo = new NeoConnection();
            
            mongoRepository = new MongoRepository();
        }

        public async Task<Guid> InsertIntoPersistance<T>(NeoEvent e, T obj, Action<OperationCompletedEvent> callback) where T : class, new()
        {
            var asyncTransaction = new AsyncTransaction();

            // create a new Task for inserting the Guid in the Mongo database
            var mongoInsert = new Func<object, object>( state =>
            {
                var mongoId = mongoRepository.InsertModel(state);
                return mongoId;
            });

            var mongoRollback = new Action<object>(async x => {
                var id = (Guid)x;

                await mongoRepository.RemoveModel(id.ToString());  // maybe we should wrap this in a transaction too?
            });
            var mop = new Operation(mongoInsert, obj, mongoRollback);
            asyncTransaction.AddOperation(mop);
            asyncTransaction.OperationCompleted += callback;

            // create events transactions
            var eventInsert = new Func<object, Task<object>>(async (state) =>
           {
               e.EventId = Guid.NewGuid();
               e.MongoId = (Guid) state;

               var success = await neo.InsertEvent(e);
               
               if(!success)
                   throw new Exception("Something wrong happened while inserting data into Neo4j");     // will it ever be thrown ?
               
               return e.EventId;
           }); 

            var eventRollback = new Action<object>(x =>
            {
                // here we don't really want to remove the event, but rather invalidate it.
                // anyways this is going to be implemented later on if I'm not lazy
            });
            
            var eop = new Operation(eventInsert, null, eventRollback);
            eop.SetId(mop.OperationId);
            asyncTransaction.AddOperation(eop);


            var transaction = new PolygotTransaction();
            transaction.InsertTransaction(asyncTransaction);

            await transaction.Commit();
            
            Log.Info($" [x] Inserted event {e.EventType} into persistence, with index {e.PersistenceIndex}");

            return mop.OperationId;
        }

        public async Task<T[]> Query<T>(string eventType, Guid persistenceId) where T : class, new()
        {
            var retval = new List<T>();
            
             var events = await neo.QueryEventsForPersistenceIndex(persistenceId, eventType);
             
             events.ForEach(x =>
             {
                 var item = mongoRepository.GetModel<T>(x.MongoId.ToString());
                 
                 if(item != null)
                     retval.Add(item);
             });

             Log.Info($" [x] Received a total of {retval.Count} elements");
             
             return retval.ToArray();
        }

        public void Dispose()
        {
            neo.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}