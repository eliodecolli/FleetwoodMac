using System;
using MessagePack;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    [MessagePackObject]
    public class ContractUpdated
    {
        [Key(0)]
        public Guid PersistenceIndex { get; set; }

        [Key(1)]
        public string Status { get; set; }
    }
}
