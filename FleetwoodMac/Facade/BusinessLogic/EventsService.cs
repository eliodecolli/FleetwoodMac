using System;
using System.Collections.Generic;
using System.Linq;
using FleetwoodMac_Personel.Facade.Persistence.Mongo.Repository;
using FleetwoodMac_Personel.Facade.Persistence.Transactions;
using System.Threading.Tasks;
using FleetwoodMac_Personel.Facade.Models.Aggregates;
using FleetwoodMac_Personel.Facade.Models.Events;
using FleetwoodMac_Personel.Facade.Persistence.Neo4J;
using FleetwoodMac_Personel.Facade.Persistence.Neo4J.Entities;
using MongoDB.Bson;

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
        
        

        public async Task<Guid> InsertIntoPersistance<T>(NeoEvent e, T obj, Action<OperationCompletedEvent> callback, string collection) where T : class, new()
        {
            var asyncTransaction = new AsyncTransaction();

            // create a new Task for inserting the Guid in the Mongo database
            var mongoInsert = new Func<object, object>( state =>
            {
                var mongoId = mongoRepository.InsertModel<T>((T)state, collection);
                return mongoId;
            });

            var mongoRollback = new Action<object>(async x => {
                var id = (ObjectId)x;

                await mongoRepository.RemoveModel<T>(id.ToString(), collection);  // maybe we should wrap this in a transaction too?
            });
            var mop = new Operation(mongoInsert, obj, mongoRollback);
            asyncTransaction.AddOperation(mop);
            asyncTransaction.OperationCompleted += callback;

            // create events transactions
            var eventInsert = new Func<object, Task<object>>(async (state) =>
           {
               e.EventId = Guid.NewGuid();
               e.MongoId = (ObjectId) state;

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

        public async Task<T[]> QueryByDate<T>(string eventType, Guid persistenceId, DateTime from, DateTime to, string mongoCollection) where T : class, new()
        {
            var retval = new List<T>();
            
            var events = await neo.QueryEventsByDate(persistenceId, eventType, from, to);
             
            events.ForEach(async x =>
            {
                var item = await mongoRepository.GetModel<T>(x.MongoId.ToString(), mongoCollection);
                 
                if(item != null)
                    retval.Add(item);
            });

            Log.Info($" [x] Received a total of {retval.Count} elements");
             
            return retval.ToArray();
        }

        public async Task<T[]> Query<T>(string eventType, Guid persistenceId, string mongoCollection) where T : class, new()
        {
            var retval = new List<T>();
            
             var events = await neo.QueryEventsForPersistenceIndex(persistenceId, eventType);
             
             events.ForEach(async x =>
             {
                 var item = await mongoRepository.GetModel<T>(x.MongoId.ToString(), mongoCollection);
                 
                 if(item != null)
                     retval.Add(item);
             });

             Log.Info($" [x] Received a total of {retval.Count} elements");
             
             return retval.ToArray();
        }

        public async Task<TaxPeriod> GetTotalTaxes(Guid persistenceId)
        {
            var taxes = await mongoRepository.SumOverCluster<UserTaxAddedEvent>(persistenceId, 
                "Amount", "Taxes");

            return new TaxPeriod()
            {
                Count = (double)taxes["value"]["Count"],
                Total = (double)taxes["value"]["Total"]
            };
        }

        public void Dispose()
        {
            neo.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}