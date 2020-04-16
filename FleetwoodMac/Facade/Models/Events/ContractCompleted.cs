using System;
using MessagePack;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    [MessagePackObject]
    public class ContractCompleted
    {
        [Key(0)]
        public Guid PersistenceIndex { get; set; }

        [Key(1)]
        public string Result { get; set; }
    }
}
