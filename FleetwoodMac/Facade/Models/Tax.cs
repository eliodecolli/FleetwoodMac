using System;
using MessagePack;

namespace FleetwoodMac_Personel.Facade.Models
{
    [MessagePackObject(true)]
    public class Tax
    {
        public Guid PersistanceIndex { get; set; }

        public double Amount { get; set; }
    }
}
