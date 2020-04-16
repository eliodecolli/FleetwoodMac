using System;
using System.Linq;
using System.Threading.Tasks;
using FleetwoodMac_Personel.Facade.BusinessLogic;
using FleetwoodMac_Personel.Facade.Models.Events;
using FleetwoodMac_Personel.Facade.Persistence.Neo4J.Entities;
using MongoDB.Bson;

namespace FleetwoodMac
{

    class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Initialize("log.txt", LogLevel.All, false);
            
            var g = Guid.Empty;
            
            using var service = new EventsService();
            /*for (int i = 0; i < 100; i++)
            {
                await service.InsertIntoPersistance<UserTaxAddedEvent>(new NeoEvent()
                {
                    EventType = NeoEvent.TaxAdded,
                    PersistenceIndex = g
                }, new UserTaxAddedEvent()
                {
                    Id = ObjectId.GenerateNewId(),
                    PersistenceIndex = g,
                    Amount = 10
                }, null, "Taxes");
            }

            Console.Read();*/

            var tt = await service.GetTotalTaxes(g);

            Console.WriteLine($"Total: {tt.Total} ({tt.Count})");

            /*var ret = await service.QueryByDate<UserTaxAddedEvent>(NeoEvent.TaxAdded, Guid.Empty,
                DateTime.Parse("01/01/1995"), DateTime.Now, "Taxes");
                //await service.Query<UserTaxAddedEvent>(NeoEvent.TaxAdded, Guid.Empty);

            foreach(var r in ret)
                Console.WriteLine(r.Amount);*/
        }
    }
}
