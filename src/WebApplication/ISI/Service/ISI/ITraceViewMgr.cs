using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITraceViewMgr : ITraceViewBaseMgr
    {
        #region Customized Methods

        IList<Comment> GetComment(string taskCode, int firstRow, int maxRows);
        int GetCommentCount(string taskCode);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITraceViewMgrE : com.Sconit.ISI.Service.ITraceViewMgr
    {
    }
}

#endregion Extend Interface