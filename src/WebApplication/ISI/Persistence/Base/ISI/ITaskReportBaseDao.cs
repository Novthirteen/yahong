using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface ITaskReportBaseDao
    {
        #region Method Created By CodeSmith

        void CreateTaskReport(TaskReport entity);

        TaskReport LoadTaskReport(Int32 id);
  
        IList<TaskReport> GetAllTaskReport();
  
        IList<TaskReport> GetAllTaskReport(bool includeInactive);
  
        void UpdateTaskReport(TaskReport entity);
        
        void DeleteTaskReport(Int32 id);
    
        void DeleteTaskReport(TaskReport entity);
    
        void DeleteTaskReport(IList<Int32> pkList);
    
        void DeleteTaskReport(IList<TaskReport> entityList);    
        #endregion Method Created By CodeSmith
    }
}
