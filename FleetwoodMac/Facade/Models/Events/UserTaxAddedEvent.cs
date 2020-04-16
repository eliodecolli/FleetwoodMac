using MessagePack;
using System;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    [MessagePackObject]
    public class UserTaxAddedEvent
    {
        [Key(0)]
        public Guid PersistenceIndex { get; set; }

        [Key(1)]
        public double Amount { get; set; }
    }
}
