using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace FleetwoodMac_Personel.Facade.Models.Events
{
    public abstract class BaseModel
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement]
        public ObjectId Id { get; set; }
        
        [BsonElement]
        public Guid PersistenceIndex { get; set; }
    }
}
