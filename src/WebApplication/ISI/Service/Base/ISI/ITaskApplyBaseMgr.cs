using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskApplyBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateTaskApply(TaskApply entity);

        TaskApply LoadTaskApply(Int32 id);

        IList<TaskApply> GetAllTaskApply();
    
        void UpdateTaskApply(TaskApply entity);

        void DeleteTaskApply(Int32 id);
    
        void DeleteTaskApply(TaskApply entity);
    
        void DeleteTaskApply(IList<Int32> pkList);
    
        void DeleteTaskApply(IList<TaskApply> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
