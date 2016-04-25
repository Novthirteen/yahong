using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ICommentDetailMgr : ICommentDetailBaseMgr
    {
        #region Customized Methods

        IList<Comment> GetComment(string taskCode, int firstRow, int maxRows);

        IList<CommentDetail> GetComment(string taskCode);
        IList<Comment>[] GetComment(string taskCode, int? currentCount, int? count, DateTime Monday, DateTime LastMonday, DateTime LastLastMonday);

        IDictionary<string, IList<object>> GetComment(IList<string> taskCodeList, DateTime monday, DateTime lastMonday, DateTime lastLastMonday);
        int GetCommentCount(string taskCode);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ICommentDetailMgrE : com.Sconit.ISI.Service.ICommentDetailMgr
    {
    }
}

#endregion Extend Interface