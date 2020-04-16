using MessagePack;
using MongoDB.Bson.Serialization.Attributes;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    [MessagePackObject]
    public class ContractUpdated : BaseModel
    {
        [Key(0)]
        [BsonElement]
        public string Status { get; set; }
    }
}
