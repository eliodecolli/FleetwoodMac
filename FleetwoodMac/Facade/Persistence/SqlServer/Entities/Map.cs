using System;
using System.Collections.Generic;
using System.Text;

namespace FleetwoodMac_Personel.Facade.Persistence.SqlServer.Entities
{
    public class Map
    {
        public Guid Id { get; set; }    // persistence id

        public virtual ICollection<MongoLink> MongoIds { get; set; }
    }
}
