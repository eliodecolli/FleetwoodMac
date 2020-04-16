using System;
using MessagePack;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    [MessagePackObject]
    public class ContractAssigned
    {
        [Key(0)]
        public Guid PersistenceIndex { get; set; }

        [Key(1)]
        public string Client { get; set; }
    }
}
