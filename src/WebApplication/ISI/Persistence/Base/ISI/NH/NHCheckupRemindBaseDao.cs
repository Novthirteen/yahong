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
    public class NHCheckupRemindBaseDao : NHDaoBase, ICheckupRemindBaseDao
    {
        public NHCheckupRemindBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateCheckupRemind(CheckupRemind entity)
        {
            Create(entity);
        }

        public virtual IList<CheckupRemind> GetAllCheckupRemind()
        {
            return FindAll<CheckupRemind>();
        }

        public virtual CheckupRemind LoadCheckupRemind(Int32 id)
        {
            return FindById<CheckupRemind>(id);
        }

        public virtual void UpdateCheckupRemind(CheckupRemind entity)
        {
            Update(entity);
        }

        public virtual void DeleteCheckupRemind(Int32 id)
        {
            string hql = @"from CheckupRemind entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteCheckupRemind(CheckupRemind entity)
        {
            Delete(entity);
        }

        public virtual void DeleteCheckupRemind(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from CheckupRemind entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteCheckupRemind(IList<CheckupRemind> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (CheckupRemind entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteCheckupRemind(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
