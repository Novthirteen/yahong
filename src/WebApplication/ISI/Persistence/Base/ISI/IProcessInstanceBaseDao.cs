using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IProcessInstanceBaseDao
    {
        #region Method Created By CodeSmith

        void CreateProcessInstance(ProcessInstance entity);

        ProcessInstance LoadProcessInstance(Int32 id);
  
        IList<ProcessInstance> GetAllProcessInstance();
  
        void UpdateProcessInstance(ProcessInstance entity);
        
        void DeleteProcessInstance(Int32 id);
    
        void DeleteProcessInstance(ProcessInstance entity);
    
        void DeleteProcessInstance(IList<Int32> pkList);
    
        void DeleteProcessInstance(IList<ProcessInstance> entityList);    
        #endregion Method Created By CodeSmith
    }
}
