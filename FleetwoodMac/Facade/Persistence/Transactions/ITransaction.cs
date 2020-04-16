using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FleetwoodMac_Personel.Facade.Persistence.Transactions
{
    public interface ITransaction
    {
        Task Commit();
        Task Rollback();
    }
}
