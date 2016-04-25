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
    public class NHFilterBaseDao : NHDaoBase, IFilterBaseDao
    {
        public NHFilterBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFilter(Filter entity)
        {
            Create(entity);
        }

        public virtual IList<Filter> GetAllFilter()
        {
            return FindAll<Filter>();
        }

        public virtual Filter LoadFilter(Int32 id)
        {
            return FindById<Filter>(id);
        }

        public virtual void UpdateFilter(Filter entity)
        {
            Update(entity);
        }

        public virtual void DeleteFilter(Int32 id)
        {
            string hql = @"from Filter entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFilter(Filter entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFilter(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Filter entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFilter(IList<Filter> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (Filter entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFilter(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
