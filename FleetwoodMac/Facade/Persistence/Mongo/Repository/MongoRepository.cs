/*
 * Reasons to combine Mongo and Neo4j:
 * 1. If for some reason the structure of the events, or new types of relationships between events are introduced Neo4J allows us to easily implement them.
 * 2. Fast query over multiple joins with Neo4J.
 * 3. In case of (1) where the related blob of the event changes, MongoDB allows us to change the structure of the aggregate, or perhaps add new types of aggregates into the models.
 */

// Is it reasonable to use a single collection of Models though? Perhaps in a real life scenario collections should be divided, ie Tasks, Contracts and so on. However, MessagePack being pretty fast might compensate a little bit for this kind of choice.

using System;
using MongoDB.Driver;
using FleetwoodMac_Personel.Facade.Persistence.Mongo.Entities;
using MessagePack;
using System.Threading.Tasks;

namespace FleetwoodMac_Personel.Facade.Persistence.Mongo.Repository
{
    public class MongoRepository
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<BaseModel> collection;

        public MongoRepository()
        {
            client = new MongoClient();
            database = client.GetDatabase("FleedwoodMac");
            collection = database.GetCollection<BaseModel>("Models");
        }

        public  async Task RemoveModel(string id)
        {
            if (!Guid.TryParse(id, out Guid _id))
            {
                Log.Error(" [x] Invalid object id specified.");
                throw new Exception("Invalid object id");
            }

            await collection.FindOneAndDeleteAsync(Builders<BaseModel>.Filter.Eq("Id", _id));
        }
        
        public  async Task<T> SumOverCluster<T>(Guid persistenceId, string joinProperty)
        {
            Log.Data($"Summing over cluster on prop {joinProperty}");
            
            var map = @"
                function() {
                    var model = this;
                    emit(model.Id, { count: 1, eVal: {0} });
                }";
            map = string.Format(map, joinProperty);

            var reduce = @"
                function(key, value) {
                    var retval = { count: 0, total: 0 };
                    value.forEach( function( val ) {
                        retval.count += 1;
                        retval.total += val.eVal;                    
                    });
                    return retval;
                }";
            
            var retval = await collection.MapReduceAsync(map, reduce, new MapReduceOptions<BaseModel, T>()
            {
                Filter = Builders<BaseModel>.Filter.Eq("PersistenceId", persistenceId.ToString())
            });

            return await retval.FirstOrDefaultAsync();
        }

        public Guid InsertModel<T>(T model)
        {
            var bm = new BaseModel() { Id = Guid.NewGuid() };
            bm.Blob = MessagePackSerializer.Serialize<T>(model);
            collection.InsertOne(bm);

            return Guid.Parse(bm.Id.ToString());
        }

        public  T GetModel<T>(string id) where T : class, new()
        {
            if (!Guid.TryParse(id, out Guid _id))
            {
                Log.Error(" [x] Invalid object id specified.");
                throw new Exception("Invalid object id");
            }

            var model = collection.Find(Builders<BaseModel>.Filter.Eq("Id", _id)).FirstOrDefault();
            if(model == null)
            {
                //Log.Error($" [x] Cannot find object with Id {id}");
                return null;
            }

            return MessagePackSerializer.Deserialize<T>(model.Blob);
        }

        public  void UpdateModel<T>(T model, string objectId)
        {
            if (!Guid.TryParse(objectId, out Guid _id))
            {
                Log.Error(" [x] Invalid object id specified.");
                return;
            }
            var found = collection.Find(Builders<BaseModel>.Filter.Eq("Id", _id)).FirstOrDefault();
            found.Blob = MessagePackSerializer.Serialize<T>(model);

            collection.FindOneAndReplace(Builders<BaseModel>.Filter.Eq("Id", _id), found);

            Log.Info($" [x] Updated model with _id {objectId}");
        }
    }
}
