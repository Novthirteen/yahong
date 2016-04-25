using System;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskStatusMgr : ITaskStatusBaseMgr
    {
        #region Customized Methods
        IList<TaskStatus> GetTaskStatus(string taskCode);
        IList<TaskStatus> GetTaskStatus(string taskCode, int firstRow, int maxRows);
        IDictionary<string, IList<object>> GetTaskStatus(IList<string> taskCodeList, DateTime monday, DateTime lastMonday, DateTime lastLastMonday);
        IList<TaskStatus>[] GetTaskStatus(string taskCode, int? currentCount, int? count, DateTime Monday, DateTime LastMonday, DateTime LastLastMonday);
        object[] GetLastTaskStatus(string taskCode);

        IList<object[]> GetLastTaskStatus(string[] taskCodes);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskStatusMgrE : com.Sconit.ISI.Service.ITaskStatusMgr
    {
    }
}

#endregion Extend Interface