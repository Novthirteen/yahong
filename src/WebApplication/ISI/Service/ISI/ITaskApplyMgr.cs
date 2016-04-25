using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;
using System.Text;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskApplyMgr : ITaskApplyBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        IList<TaskApply> GetActiveTaskApply(string taskCode);
        IList<TaskApply> GetTaskApply(string taskCode);
        IList<TaskApply> GetTaskApply(IList<string> taskCodeList);
        void OutputApply(StringBuilder desc, IList<TaskApply> taskApplyList, string language);
        void OutputApply(StringBuilder desc, IList<TaskApply> taskApplyList);

        void OutputEmailApply(StringBuilder desc, IList<TaskApply> taskApplyList, bool IsRemoveApply);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskApplyMgrE : com.Sconit.ISI.Service.ITaskApplyMgr
    {
    }
}

#endregion Extend Interface