using System;
using System.Collections.Generic;
using System.Text;

namespace FleetwoodMac_Personel.Facade.Persistence.SqlServer.Entities
{
    public class Event
    {
        public Guid EventId { get; set; }

        public Guid PersistenceIndex { get; set; }    // unique check key

        public Guid MongoKey { get; set; }

        public DateTime ThrownTime { get; set; }

        public virtual EventType EventType { get; set; }

        public virtual int EventTypeId { get; set; }
    }
}
