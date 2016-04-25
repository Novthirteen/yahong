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
    public class ProcessInstanceBaseMgr : SessionBase, IProcessInstanceBaseMgr
    {
        public IProcessInstanceDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateProcessInstance(ProcessInstance entity)
        {
            entityDao.CreateProcessInstance(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ProcessInstance LoadProcessInstance(Int32 id)
        {
            return entityDao.LoadProcessInstance(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ProcessInstance> GetAllProcessInstance()
        {
            return entityDao.GetAllProcessInstance();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateProcessInstance(ProcessInstance entity)
        {
            entityDao.UpdateProcessInstance(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessInstance(Int32 id)
        {
            entityDao.DeleteProcessInstance(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessInstance(ProcessInstance entity)
        {
            entityDao.DeleteProcessInstance(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessInstance(IList<Int32> pkList)
        {
            entityDao.DeleteProcessInstance(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessInstance(IList<ProcessInstance> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteProcessInstance(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
