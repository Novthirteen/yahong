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
    public class ProcessDefinitionBaseMgr : SessionBase, IProcessDefinitionBaseMgr
    {
        public IProcessDefinitionDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateProcessDefinition(ProcessDefinition entity)
        {
            entityDao.CreateProcessDefinition(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ProcessDefinition LoadProcessDefinition(Int32 id)
        {
            return entityDao.LoadProcessDefinition(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ProcessDefinition> GetAllProcessDefinition()
        {
            return entityDao.GetAllProcessDefinition();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateProcessDefinition(ProcessDefinition entity)
        {
            entityDao.UpdateProcessDefinition(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessDefinition(Int32 id)
        {
            entityDao.DeleteProcessDefinition(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessDefinition(ProcessDefinition entity)
        {
            entityDao.DeleteProcessDefinition(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessDefinition(IList<Int32> pkList)
        {
            entityDao.DeleteProcessDefinition(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessDefinition(IList<ProcessDefinition> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteProcessDefinition(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
