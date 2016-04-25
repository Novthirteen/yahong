using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface ITaskAddressBaseDao
    {
        #region Method Created By CodeSmith

        void CreateTaskAddress(TaskAddress entity);

        TaskAddress LoadTaskAddress(String code);
  
        IList<TaskAddress> GetAllTaskAddress();
  
        void UpdateTaskAddress(TaskAddress entity);
        
        void DeleteTaskAddress(String code);
    
        void DeleteTaskAddress(TaskAddress entity);
    
        void DeleteTaskAddress(IList<String> pkList);
    
        void DeleteTaskAddress(IList<TaskAddress> entityList);    
        #endregion Method Created By CodeSmith
    }
}
