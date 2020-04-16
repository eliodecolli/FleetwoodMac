using System;
using MessagePack;
using MongoDB.Bson.Serialization.Attributes;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    [MessagePackObject]
    public class ContractAssigned : BaseModel
    {
        [Key(0)]
        [BsonElement]
        public string Client { get; set; }
    }
}
