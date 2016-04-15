using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity.MRP;
using com.Sconit.Persistence.MRP;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class MrpPlanTransactionBaseMgr : SessionBase, IMrpPlanTransactionBaseMgr
    {
        public IMrpPlanTransactionDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateMrpPlanTransaction(MrpPlanTransaction entity)
        {
            entityDao.CreateMrpPlanTransaction(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual MrpPlanTransaction LoadMrpPlanTransaction(Int32 id)
        {
            return entityDao.LoadMrpPlanTransaction(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<MrpPlanTransaction> GetAllMrpPlanTransaction()
        {
            return entityDao.GetAllMrpPlanTransaction();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateMrpPlanTransaction(MrpPlanTransaction entity)
        {
            entityDao.UpdateMrpPlanTransaction(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpPlanTransaction(Int32 id)
        {
            entityDao.DeleteMrpPlanTransaction(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpPlanTransaction(MrpPlanTransaction entity)
        {
            entityDao.DeleteMrpPlanTransaction(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpPlanTransaction(IList<Int32> pkList)
        {
            entityDao.DeleteMrpPlanTransaction(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteMrpPlanTransaction(IList<MrpPlanTransaction> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteMrpPlanTransaction(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
