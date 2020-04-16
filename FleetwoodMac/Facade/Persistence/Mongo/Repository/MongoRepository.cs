/*
 * Reasons to combine Mongo and Neo4j:
 * 1. If for some reason the structure of the events, or new types of relationships between events are introduced Neo4J allows us to easily implement them.
 * 2. Fast query over multiple joins with Neo4J.
 * 3. In case of (1) where the related blob of the event changes, MongoDB allows us to change the structure of the aggregate, or perhaps add new types of aggregates into the models.
 */

// Is it reasonable to use a single collection of Models though? Perhaps in a real life scenario collections should be divided, ie Tasks, Contracts and so on. However, MessagePack being pretty fast might compensate a little bit for this kind of choice.

using System;
using System.Linq;
using System.Reflection;
using MongoDB.Driver;
using MessagePack;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace FleetwoodMac_Personel.Facade.Persistence.Mongo.Repository
{
    public class MongoRepository
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;

        public MongoRepository()
        {
            client = new MongoClient();
            database = client.GetDatabase("FleedwoodMac");
        }

        public  async Task RemoveModel<T>(string id, string collection)
        {
            var col = database.GetCollection<T>(collection);
            
            if (!ObjectId.TryParse(id, out ObjectId _id))
            {
                Log.Error(" [x] Invalid object id specified.");
                throw new Exception("Invalid object id");
            }

            await col.FindOneAndDeleteAsync(Builders<T>.Filter.Eq("Id", _id));
        }
        
        // NOTE: If you want to specify more conditions here, you have to granulate your data further.
        // IE: To map accordingly to persistenceIndex and a specific window of time, the date should be stored as split elements (year, month, day).
        public  async Task<BsonDocument> SumOverCluster<T>(Guid persistenceIndex, string joinProperty, string collection)
        {
            Log.Data($"Summing over cluster on prop {joinProperty}");

            var col = database.GetCollection<T>(collection);
            
            var map = @"
                function() {{
                    var model = this;
                    emit(model.PersistenceIndex, {{ count: 1, eVal: model.{0} }});
                }}";
            map = string.Format(map, joinProperty);

            var reduce = @"
                function(key, value) {
                    var retval = { Count: 0, Total: 0 };
                    value.forEach( function( val ) {
                        retval.Count += 1;
                        retval.Total += val.eVal;                    
                    });
                    return retval;
                }";

            var retval = await col.MapReduceAsync(map, reduce, new MapReduceOptions<T, BsonDocument>()
            {
                Filter = Builders<T>.Filter.Eq("PersistenceIndex", persistenceIndex)
            });

            return await retval.FirstOrDefaultAsync();
        }

        public ObjectId InsertModel<T>(T model, string collection)
        {
            var col = database.GetCollection<T>(collection);
            col.InsertOne(model);

            return (ObjectId)typeof(T).GetProperty("Id", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).GetValue(model);   // let it throw whatever
        }

        public async Task<T> GetModel<T>(string id, string collection) where T : class, new()
        {
            var col = database.GetCollection<T>(collection);
            
            if (!ObjectId.TryParse(id, out ObjectId _id))
            {
                Log.Error(" [x] Invalid object id specified.");
                throw new Exception("Invalid object id");
            }

            return await (await col.FindAsync(Builders<T>.Filter.Eq("Id", _id))).FirstOrDefaultAsync();   // awkward
        }

        public async Task UpdateModel<T>(T model, string objectId, string collection)
        {
            var col = database.GetCollection<T>(collection);
            
            if (!ObjectId.TryParse(objectId, out ObjectId id))
            {
                Log.Error(" [x] Invalid object id specified.");
                return;
            }

            await col.FindOneAndReplaceAsync(Builders<T>.Filter.Eq("Id", id), model);

            Log.Info($" [x] Updated model with _id {objectId}");
        }
    }
}
