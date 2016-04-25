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
    public class FilterBaseMgr : SessionBase, IFilterBaseMgr
    {
        public IFilterDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFilter(Filter entity)
        {
            entityDao.CreateFilter(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Filter LoadFilter(Int32 id)
        {
            return entityDao.LoadFilter(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Filter> GetAllFilter()
        {
            return entityDao.GetAllFilter();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFilter(Filter entity)
        {
            entityDao.UpdateFilter(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFilter(Int32 id)
        {
            entityDao.DeleteFilter(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFilter(Filter entity)
        {
            entityDao.DeleteFilter(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFilter(IList<Int32> pkList)
        {
            entityDao.DeleteFilter(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFilter(IList<Filter> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFilter(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
