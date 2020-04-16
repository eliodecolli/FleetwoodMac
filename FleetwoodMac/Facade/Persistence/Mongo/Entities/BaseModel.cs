using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FleetwoodMac_Personel.Facade.Persistence.Mongo.Entities
{
    public class BaseModel
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [BsonElement]
        public Guid Id { get; set; }
        
        [BsonElement]
        public Guid PersistenceId { get; set; }

        [BsonElement]
        public byte[] Blob { get; set; }
    }
}
