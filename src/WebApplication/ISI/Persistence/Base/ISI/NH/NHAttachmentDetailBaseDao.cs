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
    public class NHAttachmentDetailBaseDao : NHDaoBase, IAttachmentDetailBaseDao
    {
        public NHAttachmentDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateAttachmentDetail(AttachmentDetail entity)
        {
            Create(entity);
        }

        public virtual IList<AttachmentDetail> GetAllAttachmentDetail()
        {
            return FindAll<AttachmentDetail>();
        }

        public virtual AttachmentDetail LoadAttachmentDetail(Int32 id)
        {
            return FindById<AttachmentDetail>(id);
        }

        public virtual void UpdateAttachmentDetail(AttachmentDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteAttachmentDetail(Int32 id)
        {
            string hql = @"from AttachmentDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteAttachmentDetail(AttachmentDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteAttachmentDetail(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from AttachmentDetail entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteAttachmentDetail(IList<AttachmentDetail> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (AttachmentDetail entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteAttachmentDetail(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
