using System;
using MessagePack;

namespace FleetwoodMac_Personel.Facade.Models
{
    [MessagePackObject(true)]
    public class PersonelContract
    {
        public Guid PersistenceIndex { get; set; }

        public string ContractStatus { get; set; }

        public bool IsActive { get; set; }

        public string Result { get; set; }
        
        public string Client { get; set; }
    }
}
