using System;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskDetailMgr : ITaskDetailBaseMgr
    {
        #region Customized Methods

        void CreateTaskDetail(TaskMstr task, string level, IList<UserSub> userSubList, bool isEmailException, bool isSMSException, User user);

        bool IsSended(string taskCode, string taskSubType, string status, string level);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskDetailMgrE : com.Sconit.ISI.Service.ITaskDetailMgr
    {
    }
}

#endregion Extend Interface