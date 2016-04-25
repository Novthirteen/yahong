using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IProcessApplyMgr : IProcessApplyBaseMgr
    {
        #region Customized Methods

        void UpdateProcessApply(IList<ProcessApply> processApplyList);
        void UpdateProcessApply(IDictionary<string, ProcessApply> processApplyDic);
        IList<ProcessApply> GetProcessApply(string taskSubType);
        IList<ProcessApply> GetProcessApply(string taskSubType, string apply);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IProcessApplyMgrE : com.Sconit.ISI.Service.IProcessApplyMgr
    {
    }
}

#endregion Extend Interface