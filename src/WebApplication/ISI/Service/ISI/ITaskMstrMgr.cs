using System;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskMstrMgr : ITaskMstrBaseMgr
    {
        #region Customized Methods

        TaskMstr CheckAndLoadTaskMstr(string taskCode);

        TaskMstr CheckAndLoadTaskMstr(string taskCode, bool includeTaskDetail);

        TaskMstr LoadTaskMstr(string code, bool includeTaskDetail);

        void UpdateApplay(TaskMstr task, DateTime effDate, User user);

        void UpdateTaskMstr(TaskMstr task, User user);

        void UpdateTaskMstr(string code, string wiki, User user);

        void UpdateTaskStatus(TaskStatus taskStatus, TaskMstr take);

        IDictionary<string, string> GetUser(string userCodes);

        IDictionary<string, UserSub> GetUser2(string userCode);

        IList<object[]> GetUser3(string userCode);

        IList<Task> GetRefTask(string taskCode, int firstRow, int maxRows);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskMstrMgrE : com.Sconit.ISI.Service.ITaskMstrMgr
    {
    }
}

#endregion Extend Interface