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
    public class NHResSopBaseDao : NHDaoBase, IResSopBaseDao
    {
        public NHResSopBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateResSop(ResSop entity)
        {
            Create(entity);
        }

        public virtual IList<ResSop> GetAllResSop()
        {
            return FindAll<ResSop>();
        }

        public virtual ResSop LoadResSop(int id)
        {
            return FindById<ResSop>(id);
        }

        public virtual void UpdateResSop(ResSop entity)
        {
            Update(entity);
        }

        public virtual void DeleteResSop(int id)
        {
            string hql = @"from ResSop entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteResSop(ResSop entity)
        {
            Delete(entity);
        }

        public virtual void DeleteResSop(IList<int> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ResSop entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteResSop(IList<ResSop> entityList)
        {
            IList<int> pkList = new List<int>();
            foreach (ResSop entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteResSop(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
