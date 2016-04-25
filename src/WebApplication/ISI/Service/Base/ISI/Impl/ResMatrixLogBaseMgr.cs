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
    public class ResMatrixLogBaseMgr : SessionBase, IResMatrixLogBaseMgr
    {
        public IResMatrixLogDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateResMatrixLog(ResMatrixLog entity)
        {
            entityDao.CreateResMatrixLog(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ResMatrixLog LoadResMatrixLog(Int32 id)
        {
            return entityDao.LoadResMatrixLog(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResMatrixLog> GetAllResMatrixLog()
        {
            return entityDao.GetAllResMatrixLog();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateResMatrixLog(ResMatrixLog entity)
        {
            entityDao.UpdateResMatrixLog(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrixLog(Int32 id)
        {
            entityDao.DeleteResMatrixLog(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrixLog(ResMatrixLog entity)
        {
            entityDao.DeleteResMatrixLog(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrixLog(IList<Int32> pkList)
        {
            entityDao.DeleteResMatrixLog(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResMatrixLog(IList<ResMatrixLog> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteResMatrixLog(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
