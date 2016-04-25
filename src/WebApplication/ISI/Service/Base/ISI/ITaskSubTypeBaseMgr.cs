using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskSubTypeBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateTaskSubType(TaskSubType entity);

        TaskSubType LoadTaskSubType(String code);

        IList<TaskSubType> GetAllTaskSubType();
    
        IList<TaskSubType> GetAllTaskSubType(bool includeInactive);
      
        void UpdateTaskSubType(TaskSubType entity);

        void DeleteTaskSubType(String code);
    
        void DeleteTaskSubType(TaskSubType entity);
    
        void DeleteTaskSubType(IList<String> pkList);
    
        void DeleteTaskSubType(IList<TaskSubType> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
