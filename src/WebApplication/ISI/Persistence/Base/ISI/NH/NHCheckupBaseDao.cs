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
    public class NHCheckupBaseDao : NHDaoBase, ICheckupBaseDao
    {
        public NHCheckupBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateCheckup(Checkup entity)
        {
            Create(entity);
        }

        public virtual IList<Checkup> GetAllCheckup()
        {
            return FindAll<Checkup>();
        }

        public virtual Checkup LoadCheckup(Int32 id)
        {
            return FindById<Checkup>(id);
        }

        public virtual void UpdateCheckup(Checkup entity)
        {
            Update(entity);
        }

        public virtual void DeleteCheckup(Int32 id)
        {
            string hql = @"from Checkup entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteCheckup(Checkup entity)
        {
            Delete(entity);
        }

        public virtual void DeleteCheckup(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Checkup entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteCheckup(IList<Checkup> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (Checkup entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteCheckup(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
