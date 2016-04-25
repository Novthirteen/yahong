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
    public class NHResPatrolBaseDao : NHDaoBase, IResPatrolBaseDao
    {
        public NHResPatrolBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateResPatrol(ResPatrol entity)
        {
            Create(entity);
        }

        public virtual IList<ResPatrol> GetAllResPatrol()
        {
            return FindAll<ResPatrol>();
        }

        public virtual ResPatrol LoadResPatrol(Int32 id)
        {
            return FindById<ResPatrol>(id);
        }

        public virtual void UpdateResPatrol(ResPatrol entity)
        {
            Update(entity);
        }

        public virtual void DeleteResPatrol(Int32 id)
        {
            string hql = @"from ResPatrol entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteResPatrol(ResPatrol entity)
        {
            Delete(entity);
        }

        public virtual void DeleteResPatrol(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ResPatrol entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteResPatrol(IList<ResPatrol> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (ResPatrol entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteResPatrol(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
