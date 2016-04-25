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
    public class ApplyBaseMgr : SessionBase, IApplyBaseMgr
    {
        public IApplyDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateApply(Apply entity)
        {
            entityDao.CreateApply(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Apply LoadApply(String code)
        {
            return entityDao.LoadApply(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Apply> GetAllApply()
        {
            return entityDao.GetAllApply();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateApply(Apply entity)
        {
            entityDao.UpdateApply(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteApply(String code)
        {
            entityDao.DeleteApply(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteApply(Apply entity)
        {
            entityDao.DeleteApply(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteApply(IList<String> pkList)
        {
            entityDao.DeleteApply(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteApply(IList<Apply> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteApply(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
