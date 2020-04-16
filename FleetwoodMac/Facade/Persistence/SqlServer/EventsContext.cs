using FleetwoodMac_Personel.Facade.Persistence.SqlServer.Entities;
using System.Data.Entity;

namespace FleetwoodMac_Personel.Facade.Persistence.SqlServer
{
    public class EventsContext : DbContext
    {

        public EventsContext() : base("Server=localhost;Database=Fleetwood_Mac_B;Trusted_Connection=False;") { }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventType> EventTypes { get; set; }

        public DbSet<Map> Maps { get; set; }

        public DbSet<MongoLink> MongoLinks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasKey(x => x.EventId)
                .HasRequired(x => x.EventType).WithMany().HasForeignKey(p => p.EventTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Map>()
                .HasKey(x => x.Id)
                .HasMany(x => x.MongoIds);

            base.OnModelCreating(modelBuilder);
        }

    }
}
