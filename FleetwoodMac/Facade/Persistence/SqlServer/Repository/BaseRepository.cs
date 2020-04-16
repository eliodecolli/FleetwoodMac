using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace FleetwoodMac_Personel.Facade.Persistence.SqlServer.Repository
{
    public class BaseRepository<T> : IDisposable
        where T : class, new()
    {
        private EventsContext context;

        public BaseRepository(EventsContext context)
            => this.context = context;

        public DbContextTransaction BeginTransaction()
        {
            //context.Database.Connection.Open();
            return context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        }

        public void Update(T value)
        {
            //var set = context.Set<T>();
            context.Entry(value).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void Remove(object id)
        {
            var f = context.Set<T>().Find(id);

            if (f != null)
                context.Set<T>().Remove(f);
        }

        public T InsertNew(T value)
        {
            var retval = context.Set<T>().Add(value);
            context.SaveChanges();
            return retval;
        }

        public T[] Get(Func<T, bool> query = null)
        {
            if (query == null)
                return context.Set<T>().AsQueryable().ToArray();

            var qb = context.Set<T>().AsQueryable();
            return qb.Where(query).ToArray();
        }

        public T GetById(object id)
        {
            return context.Set<T>().Find(id);
        }

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
