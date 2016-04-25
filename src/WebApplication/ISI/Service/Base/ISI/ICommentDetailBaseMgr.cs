using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ICommentDetailBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateCommentDetail(CommentDetail entity);

        CommentDetail LoadCommentDetail(Int32 id);

        IList<CommentDetail> GetAllCommentDetail();
    
        void UpdateCommentDetail(CommentDetail entity);

        void DeleteCommentDetail(Int32 id);
    
        void DeleteCommentDetail(CommentDetail entity);
    
        void DeleteCommentDetail(IList<Int32> pkList);
    
        void DeleteCommentDetail(IList<CommentDetail> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
