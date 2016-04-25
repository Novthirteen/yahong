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
    public class NHResUserBaseDao : NHDaoBase, IResUserBaseDao
    {
        public NHResUserBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateResUser(ResUser entity)
        {
            Create(entity);
        }

        public virtual IList<ResUser> GetAllResUser()
        {
            return FindAll<ResUser>();
        }

        public virtual ResUser LoadResUser(Int32 id)
        {
            return FindById<ResUser>(id);
        }

        public virtual void UpdateResUser(ResUser entity)
        {
            Update(entity);
        }

        public virtual void DeleteResUser(Int32 id)
        {
            string hql = @"from ResUser entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteResUser(ResUser entity)
        {
            Delete(entity);
        }

        public virtual void DeleteResUser(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ResUser entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteResUser(IList<ResUser> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (ResUser entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteResUser(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
