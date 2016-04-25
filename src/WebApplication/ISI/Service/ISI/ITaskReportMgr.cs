using System;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskReportMgr : ITaskReportBaseMgr
    {
        #region Customized Methods

        IList<TaskReportView> GetUserAllTaskSubType(string userCode);

        void UpdateTaskReport(IList<TaskReportView> taskReportViewList, User user);

        void Check();

        void SendReport();

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskReportMgrE : com.Sconit.ISI.Service.ITaskReportMgr
    {
    }
}

#endregion Extend Interface