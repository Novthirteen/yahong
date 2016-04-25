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
    public class NHFailureModeBaseDao : NHDaoBase, IFailureModeBaseDao
    {
        public NHFailureModeBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFailureMode(FailureMode entity)
        {
            Create(entity);
        }

        public virtual IList<FailureMode> GetAllFailureMode()
        {
            return GetAllFailureMode(false);
        }

        public virtual IList<FailureMode> GetAllFailureMode(bool includeInactive)
        {
            string hql = @"from FailureMode entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<FailureMode> result = FindAllWithCustomQuery<FailureMode>(hql);
            return result;
        }

        public virtual FailureMode LoadFailureMode(String code)
        {
            return FindById<FailureMode>(code);
        }

        public virtual void UpdateFailureMode(FailureMode entity)
        {
            Update(entity);
        }

        public virtual void DeleteFailureMode(String code)
        {
            string hql = @"from FailureMode entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteFailureMode(FailureMode entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFailureMode(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FailureMode entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFailureMode(IList<FailureMode> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (FailureMode entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteFailureMode(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
