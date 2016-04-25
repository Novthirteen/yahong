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
    public class SummaryBaseMgr : SessionBase, ISummaryBaseMgr
    {
        public ISummaryDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateSummary(Summary entity)
        {
            entityDao.CreateSummary(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Summary LoadSummary(String code)
        {
            return entityDao.LoadSummary(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Summary> GetAllSummary()
        {
            return entityDao.GetAllSummary();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateSummary(Summary entity)
        {
            entityDao.UpdateSummary(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummary(String code)
        {
            entityDao.DeleteSummary(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummary(Summary entity)
        {
            entityDao.DeleteSummary(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummary(IList<String> pkList)
        {
            entityDao.DeleteSummary(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummary(IList<Summary> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteSummary(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
