using MessagePack;
using System;
using MongoDB.Bson.Serialization.Attributes;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    [MessagePackObject]
    public class UserTaxAddedEvent : BaseModel
    {
        [Key(0)]
        [BsonElement]
        public double Amount { get; set; }
    }
}
