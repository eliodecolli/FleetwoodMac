using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace FleetwoodMac_Personel.Facade.Persistence.Transactions
{
    public class PolygotTransaction : ITransaction
    {
        private List<AsyncTransaction> babies;

        public PolygotTransaction()
            => babies = new List<AsyncTransaction>();

        public void InsertTransaction(AsyncTransaction t)
            => babies.Add(t);

        public async Task Commit()
        {
            try
            {
                for(int i = 0; i < babies.Count; i++)
                {
                    var t = babies[i];
                    await t.Commit();
                }
            }
            catch (Exception ex)
            {
                Log.Error($" [x] Rolling back transaction {ex.Message}");
                await Rollback();
            }
            finally
            {
                Log.Info("Polygot transaction has been committed");
            }
        }

        public async Task Rollback()
        {
            for (int i = 0; i < babies.Count; i++)
                await babies[i].Rollback();
        }
    }
}
