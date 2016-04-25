using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskDetailBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateTaskDetail(TaskDetail entity);

        TaskDetail LoadTaskDetail(Int32 id);

        IList<TaskDetail> GetAllTaskDetail();
    
        IList<TaskDetail> GetAllTaskDetail(bool includeInactive);
      
        void UpdateTaskDetail(TaskDetail entity);

        void DeleteTaskDetail(Int32 id);
    
        void DeleteTaskDetail(TaskDetail entity);
    
        void DeleteTaskDetail(IList<Int32> pkList);
    
        void DeleteTaskDetail(IList<TaskDetail> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
