using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IProjectTaskMgr : IProjectTaskBaseMgr
    {
        #region Customized Methods

        IList<ProjectTask> GetProjectTask(string taskSubTypeCode, string type);

        void DeleteProjectTask(int id, User user);

        void CreateProjectTask(ProjectTask task, User user);

        void UpdateProjectTask(ProjectTask task, User user);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IProjectTaskMgrE : com.Sconit.ISI.Service.IProjectTaskMgr
    {
    }
}

#endregion Extend Interface