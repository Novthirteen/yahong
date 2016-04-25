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
    public class NHSchedulingBaseDao : NHDaoBase, ISchedulingBaseDao
    {
        public NHSchedulingBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateScheduling(Scheduling entity)
        {
            Create(entity);
        }

        public virtual IList<Scheduling> GetAllScheduling()
        {
            return FindAll<Scheduling>();
        }

        public virtual Scheduling LoadScheduling(Int32 id)
        {
            return FindById<Scheduling>(id);
        }

        public virtual void UpdateScheduling(Scheduling entity)
        {
            Update(entity);
        }

        public virtual void DeleteScheduling(Int32 id)
        {
            string hql = @"from Scheduling entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteScheduling(Scheduling entity)
        {
            Delete(entity);
        }

        public virtual void DeleteScheduling(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Scheduling entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteScheduling(IList<Scheduling> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (Scheduling entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteScheduling(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
