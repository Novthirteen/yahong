using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskStatusBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateTaskStatus(TaskStatus entity);

        TaskStatus LoadTaskStatus(Int32 id);

        IList<TaskStatus> GetAllTaskStatus();
    
        void UpdateTaskStatus(TaskStatus entity);

        void DeleteTaskStatus(Int32 id);
    
        void DeleteTaskStatus(TaskStatus entity);
    
        void DeleteTaskStatus(IList<Int32> pkList);
    
        void DeleteTaskStatus(IList<TaskStatus> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
