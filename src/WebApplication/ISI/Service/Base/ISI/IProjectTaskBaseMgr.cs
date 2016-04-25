using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IProjectTaskBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateProjectTask(ProjectTask entity);

        ProjectTask LoadProjectTask(Int32 id);

        IList<ProjectTask> GetAllProjectTask();
    
        IList<ProjectTask> GetAllProjectTask(bool includeInactive);
      
        void UpdateProjectTask(ProjectTask entity);

        void DeleteProjectTask(Int32 id);
    
        void DeleteProjectTask(ProjectTask entity);
    
        void DeleteProjectTask(IList<Int32> pkList);
    
        void DeleteProjectTask(IList<ProjectTask> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
