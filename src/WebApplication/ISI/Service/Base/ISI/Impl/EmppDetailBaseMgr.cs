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
    public class EmppDetailBaseMgr : SessionBase, IEmppDetailBaseMgr
    {
        public IEmppDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateEmppDetail(EmppDetail entity)
        {
            entityDao.CreateEmppDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual EmppDetail LoadEmppDetail(Int32 id)
        {
            return entityDao.LoadEmppDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<EmppDetail> GetAllEmppDetail()
        {
            return entityDao.GetAllEmppDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateEmppDetail(EmppDetail entity)
        {
            entityDao.UpdateEmppDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEmppDetail(Int32 id)
        {
            entityDao.DeleteEmppDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEmppDetail(EmppDetail entity)
        {
            entityDao.DeleteEmppDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEmppDetail(IList<Int32> pkList)
        {
            entityDao.DeleteEmppDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteEmppDetail(IList<EmppDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteEmppDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
