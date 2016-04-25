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
    public class SummaryDetBaseMgr : SessionBase, ISummaryDetBaseMgr
    {
        public ISummaryDetDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateSummaryDet(SummaryDet entity)
        {
            entityDao.CreateSummaryDet(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual SummaryDet LoadSummaryDet(Int32 id)
        {
            return entityDao.LoadSummaryDet(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<SummaryDet> GetAllSummaryDet()
        {
            return entityDao.GetAllSummaryDet();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateSummaryDet(SummaryDet entity)
        {
            entityDao.UpdateSummaryDet(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummaryDet(Int32 id)
        {
            entityDao.DeleteSummaryDet(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummaryDet(SummaryDet entity)
        {
            entityDao.DeleteSummaryDet(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummaryDet(IList<Int32> pkList)
        {
            entityDao.DeleteSummaryDet(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteSummaryDet(IList<SummaryDet> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteSummaryDet(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
