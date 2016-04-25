using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class CommentDetailBaseMgr : SessionBase, ICommentDetailBaseMgr
    {
        public ICommentDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateCommentDetail(CommentDetail entity)
        {
            entityDao.CreateCommentDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual CommentDetail LoadCommentDetail(Int32 id)
        {
            return entityDao.LoadCommentDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<CommentDetail> GetAllCommentDetail()
        {
            return entityDao.GetAllCommentDetail();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateCommentDetail(CommentDetail entity)
        {
            entityDao.UpdateCommentDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCommentDetail(Int32 id)
        {
            entityDao.DeleteCommentDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCommentDetail(CommentDetail entity)
        {
            entityDao.DeleteCommentDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCommentDetail(IList<Int32> pkList)
        {
            entityDao.DeleteCommentDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCommentDetail(IList<CommentDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteCommentDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
