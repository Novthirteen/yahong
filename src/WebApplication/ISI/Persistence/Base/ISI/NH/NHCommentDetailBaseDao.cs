using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.ISI.Entity;
using com.Sconit.Persistence;

//TODO: Add other using statmens here.

namespace com.Sconit.ISI.Persistence.NH
{
    public class NHCommentDetailBaseDao : NHDaoBase, ICommentDetailBaseDao
    {
        public NHCommentDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateCommentDetail(CommentDetail entity)
        {
            Create(entity);
        }

        public virtual IList<CommentDetail> GetAllCommentDetail()
        {
            return FindAll<CommentDetail>();
        }

        public virtual CommentDetail LoadCommentDetail(Int32 id)
        {
            return FindById<CommentDetail>(id);
        }

        public virtual void UpdateCommentDetail(CommentDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteCommentDetail(Int32 id)
        {
            string hql = @"from CommentDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteCommentDetail(CommentDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteCommentDetail(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from CommentDetail entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteCommentDetail(IList<CommentDetail> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (CommentDetail entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteCommentDetail(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
