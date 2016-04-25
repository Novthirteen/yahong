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
    public class NHCheckupProjectBaseDao : NHDaoBase, ICheckupProjectBaseDao
    {
        public NHCheckupProjectBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateCheckupProject(CheckupProject entity)
        {
            Create(entity);
        }

        public virtual IList<CheckupProject> GetAllCheckupProject()
        {
            return GetAllCheckupProject(false);
        }

        public virtual IList<CheckupProject> GetAllCheckupProject(bool includeInactive)
        {
            string hql = @"from CheckupProject entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<CheckupProject> result = FindAllWithCustomQuery<CheckupProject>(hql);
            return result;
        }

        public virtual CheckupProject LoadCheckupProject(String code)
        {
            return FindById<CheckupProject>(code);
        }

        public virtual void UpdateCheckupProject(CheckupProject entity)
        {
            Update(entity);
        }

        public virtual void DeleteCheckupProject(String code)
        {
            string hql = @"from CheckupProject entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteCheckupProject(CheckupProject entity)
        {
            Delete(entity);
        }

        public virtual void DeleteCheckupProject(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from CheckupProject entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteCheckupProject(IList<CheckupProject> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (CheckupProject entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteCheckupProject(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
