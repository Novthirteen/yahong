using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Persistence;

//TODO: Add other using statmens here.

namespace com.Sconit.ISI.Persistence.NH
{
    public class NHApplyBaseDao : NHDaoBase, IApplyBaseDao
    {
        public NHApplyBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateApply(Apply entity)
        {
            Create(entity);
        }

        public virtual IList<Apply> GetAllApply()
        {
            return GetAllApply(false);
        }

        public virtual IList<Apply> GetAllApply(bool includeInactive)
        {
            string hql = @"from Apply entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<Apply> result = FindAllWithCustomQuery<Apply>(hql);
            return result;
        }

        public virtual Apply LoadApply(String code)
        {
            return FindById<Apply>(code);
        }

        public virtual void UpdateApply(Apply entity)
        {
            Update(entity);
        }

        public virtual void DeleteApply(String code)
        {
            string hql = @"from Apply entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteApply(Apply entity)
        {
            Delete(entity);
        }

        public virtual void DeleteApply(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Apply entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteApply(IList<Apply> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (Apply entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteApply(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
