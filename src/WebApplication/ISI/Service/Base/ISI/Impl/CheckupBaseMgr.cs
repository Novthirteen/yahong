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
    public class CheckupBaseMgr : SessionBase, ICheckupBaseMgr
    {
        public ICheckupDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateCheckup(Checkup entity)
        {
            entityDao.CreateCheckup(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Checkup LoadCheckup(Int32 id)
        {
            return entityDao.LoadCheckup(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Checkup> GetAllCheckup()
        {
            return entityDao.GetAllCheckup();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateCheckup(Checkup entity)
        {
            entityDao.UpdateCheckup(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckup(Int32 id)
        {
            entityDao.DeleteCheckup(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckup(Checkup entity)
        {
            entityDao.DeleteCheckup(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckup(IList<Int32> pkList)
        {
            entityDao.DeleteCheckup(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckup(IList<Checkup> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteCheckup(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
