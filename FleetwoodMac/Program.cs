using System;
using System.Linq;
using System.Threading.Tasks;
using FleetwoodMac_Personel.Facade.BusinessLogic;
using FleetwoodMac_Personel.Facade.Models.Events;
using FleetwoodMac_Personel.Facade.Persistence.Neo4J.Entities;

namespace FleetwoodMac
{

    class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Initialize("log.txt", LogLevel.All, false);
            
            using var service = new EventsService();
            await service.InsertIntoPersistance<UserTaxAddedEvent>(new NeoEvent()
            {
                EventType = NeoEvent.TaxAdded,
                PersistenceIndex = Guid.Empty
            }, new UserTaxAddedEvent()
            {
                PersistenceIndex = new Guid(),
                Amount = 10
            }, null);

            Console.Read();

            var ret = await service.Query<UserTaxAddedEvent>(NeoEvent.TaxAdded, Guid.Empty);

            ret.Select(x => {
                Console.WriteLine($"Tax: {x.Amount}");
                return x;
            });
        }
    }
}
