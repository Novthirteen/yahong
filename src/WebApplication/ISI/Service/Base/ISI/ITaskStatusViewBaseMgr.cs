using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskStatusViewBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateTaskStatusView(TaskStatusView entity);

        TaskStatusView LoadTaskStatusView(String taskCode);

        IList<TaskStatusView> GetAllTaskStatusView();
    
        void UpdateTaskStatusView(TaskStatusView entity);

        void DeleteTaskStatusView(String taskCode);
    
        void DeleteTaskStatusView(TaskStatusView entity);
    
        void DeleteTaskStatusView(IList<String> pkList);
    
        void DeleteTaskStatusView(IList<TaskStatusView> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
