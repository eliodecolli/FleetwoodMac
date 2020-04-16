using MessagePack;
using System;
using System.Collections.Generic;

namespace FleetwoodMac_Personel.Facade.Models
{
    [MessagePackObject]
    public class PersonelData
    {
        [Key(0)]
        public Guid PersistenceIndex { get; set; }

        [Key(1)]
        public List<Tax> Taxes { get; private set; }

        [Key(2)]
        public List<PersonelContract> PastContracts { get; private set; }

        [Key(3)]
        public PersonelContract CurrentContract { get; private set; }


        public PersonelData()
        {
            Taxes = new List<Tax>();
            PastContracts = new List<PersonelContract>();
        }

        public void SetContract(PersonelContract contract)
        {
            CurrentContract = contract;
        }

        public void AddTax(Tax tax)
        {
            Taxes.Add(tax);
        }

        public void AddPastContract(PersonelContract contract)
        {
            PastContracts.Add(contract);
        }
    }
}
