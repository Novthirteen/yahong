using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskApplyViewBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateTaskApplyView(TaskApplyView entity);

        TaskApplyView LoadTaskApplyView(Int32 id);

        IList<TaskApplyView> GetAllTaskApplyView();
    
        void UpdateTaskApplyView(TaskApplyView entity);

        void DeleteTaskApplyView(Int32 id);
    
        void DeleteTaskApplyView(TaskApplyView entity);
    
        void DeleteTaskApplyView(IList<Int32> pkList);
    
        void DeleteTaskApplyView(IList<TaskApplyView> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
