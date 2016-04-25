using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface ITaskMstrBaseDao
    {
        #region Method Created By CodeSmith

        void CreateTaskMstr(TaskMstr entity);

        TaskMstr LoadTaskMstr(String code);
  
        IList<TaskMstr> GetAllTaskMstr();
  
        void UpdateTaskMstr(TaskMstr entity);
        
        void DeleteTaskMstr(String code);
    
        void DeleteTaskMstr(TaskMstr entity);
    
        void DeleteTaskMstr(IList<String> pkList);
    
        void DeleteTaskMstr(IList<TaskMstr> entityList);    
        #endregion Method Created By CodeSmith
    }
}
