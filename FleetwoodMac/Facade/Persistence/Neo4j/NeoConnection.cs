using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FleetwoodMac_Personel.Facade.Persistence.Neo4J.Entities;
using Neo4j.Driver;


namespace FleetwoodMac_Personel.Facade.Persistence.Neo4J
{
    public class NeoConnection : IDisposable
    {
        private readonly IDriver driver;
        
        public NeoConnection()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test"));
        }


        public async Task<bool> InsertEvent(NeoEvent e)
        {
            var session = driver.AsyncSession();

            var res = await session.WriteTransactionAsync(async w =>
            {
                try
                {
                    e.Timestamp = DateTime.Now;
                    Log.Info($" [x] Inserting event {e.EventType}");
                    var k = await w.RunAsync(string.Format(@"
                        MERGE (i:PersistenceIndex {{ Id: $persistence_id }})
                        CREATE (i)-[:{0} {{ timestamp: $ts }}]->(e:Event {{ mongoLink: $mongo }})", e.EventType),
                        new Dictionary<string, object>()
                        {
                            { "persistence_id", e.PersistenceIndex.ToString() },
                            { "ts", e.Timestamp.ToBinary().ToString() },
                            { "mongo", e.MongoId.ToString() }
                        });
                    
                    await w.CommitAsync();
                    return k;
                }
                catch (Exception exception)
                {
                    Log.Error("Error while adding event");
                    await w.RollbackAsync();
                    return null;
                }
            });

            return res != null;
        }

        public async Task<List<NeoEvent>> QueryEventsForPersistenceIndex(Guid persistenceIndex, string eventType)
        {
            var session = driver.AsyncSession();

            var retval = new List<NeoEvent>();

            await session.ReadTransactionAsync(async r =>
            {
                try
                {
                    var eventAppend = "";
                    if (!string.IsNullOrEmpty(eventType))
                    {
                        eventAppend = $"event_type='{eventType}'";
                    }
                    
                    var result = await r.RunAsync($"MATCH (i:PersistenceIndex {{ Id: $pId }})-[r:{eventType}]->(e:Event) RETURN i, r, e",
                        new Dictionary<string, object>()
                        {
                            { "pId", persistenceIndex.ToString() }
                        });
                    
                    //await r.CommitAsync();
                    
                    await result.ForEachAsync(x =>
                    {
                        if (!(x["e"] is INode e) || !(x["r"] is IRelationship r) || !(x["i"] is INode i)) return;
                        var @event = new NeoEvent()
                        {
                            EventType = r.Type,
                            Timestamp = DateTime.FromBinary(long.Parse((string)r["timestamp"])),
                            MongoId = Guid.Parse((string)e["mongoLink"]),
                            PersistenceIndex = Guid.Parse((string)i["Id"])
                        };
                        retval.Add(@event);
                    });
                }
                catch (Exception e)
                {
                    Log.Error(" [x] Invalid read happened");
                    //await r.RollbackAsync();
                }
            });
            
            Log.Info($"Received {retval.Count.ToString()} from query");

            return retval;
        }

        public void Dispose()
        {
            driver?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}