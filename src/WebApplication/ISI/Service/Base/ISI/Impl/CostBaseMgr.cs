using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class CostBaseMgr : SessionBase, ICostBaseMgr
    {
        public ICostDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateCost(Cost entity)
        {
            entityDao.CreateCost(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Cost LoadCost(Int32 id)
        {
            return entityDao.LoadCost(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Cost> GetAllCost()
        {
            return entityDao.GetAllCost();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateCost(Cost entity)
        {
            entityDao.UpdateCost(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCost(Int32 id)
        {
            entityDao.DeleteCost(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCost(Cost entity)
        {
            entityDao.DeleteCost(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCost(IList<Int32> pkList)
        {
            entityDao.DeleteCost(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCost(IList<Cost> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteCost(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
