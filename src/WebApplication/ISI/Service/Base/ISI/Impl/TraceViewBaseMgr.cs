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
    public class TraceViewBaseMgr : SessionBase, ITraceViewBaseMgr
    {
        public ITraceViewDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTraceView(TraceView entity)
        {
            entityDao.CreateTraceView(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TraceView LoadTraceView(Int32 id, String type)
        {
            return entityDao.LoadTraceView(id, type);
        }
    

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TraceView> GetAllTraceView()
        {
            return entityDao.GetAllTraceView();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTraceView(TraceView entity)
        {
            entityDao.UpdateTraceView(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTraceView(Int32 id, String type)
        {
            entityDao.DeleteTraceView(id, type);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTraceView(TraceView entity)
        {
            entityDao.DeleteTraceView(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTraceView(IList<TraceView> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTraceView(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
