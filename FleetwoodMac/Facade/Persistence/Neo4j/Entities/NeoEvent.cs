using System;

namespace FleetwoodMac_Personel.Facade.Persistence.Neo4J.Entities
{
    
    public class NeoEvent
    {

        public const string TaxAdded = "TAX_ADDED";
        public const string ContractAssigned = "CONTRACT_ASSIGNED";
        public const string ContractCompleted = "CONTRACT_COMPLETED";
        public const string ContractUpdated = "CONTRACT_UPDATED";


        public Guid EventId { get; set; }
        
        public Guid MongoId { get; set; }
        
        public string EventType { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public Guid PersistenceIndex { get; set; }
    }
}