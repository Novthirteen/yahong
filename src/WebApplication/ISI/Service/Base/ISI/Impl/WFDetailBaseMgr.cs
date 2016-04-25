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
    public class WFDetailBaseMgr : SessionBase, IWFDetailBaseMgr
    {
        public IWFDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateWFDetail(WFDetail entity)
        {
            entityDao.CreateWFDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual WFDetail LoadWFDetail(Int32 id)
        {
            return entityDao.LoadWFDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<WFDetail> GetAllWFDetail()
        {
            return entityDao.GetAllWFDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateWFDetail(WFDetail entity)
        {
            entityDao.UpdateWFDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWFDetail(Int32 id)
        {
            entityDao.DeleteWFDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWFDetail(WFDetail entity)
        {
            entityDao.DeleteWFDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWFDetail(IList<Int32> pkList)
        {
            entityDao.DeleteWFDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWFDetail(IList<WFDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteWFDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
