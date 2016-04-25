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
    public class NHBudgetBaseDao : NHDaoBase, IBudgetBaseDao
    {
        public NHBudgetBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateBudget(Budget entity)
        {
            Create(entity);
        }

        public virtual IList<Budget> GetAllBudget()
        {
            return GetAllBudget(false);
        }

        public virtual IList<Budget> GetAllBudget(bool includeInactive)
        {
            string hql = @"from Budget entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<Budget> result = FindAllWithCustomQuery<Budget>(hql);
            return result;
        }

        public virtual Budget LoadBudget(String code)
        {
            return FindById<Budget>(code);
        }

        public virtual void UpdateBudget(Budget entity)
        {
            Update(entity);
        }

        public virtual void DeleteBudget(String code)
        {
            string hql = @"from Budget entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteBudget(Budget entity)
        {
            Delete(entity);
        }

        public virtual void DeleteBudget(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Budget entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteBudget(IList<Budget> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (Budget entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteBudget(pkList);
        }


        public virtual Budget LoadBudget(String taskSubType, Boolean isActive, Int32 year)
        {
            string hql = @"from Budget entity where entity.TaskSubType = ? and entity.IsActive = ? and entity.Year = ?";
            IList<Budget> result = FindAllWithCustomQuery<Budget>(hql, new object[] { taskSubType, isActive, year }, new IType[] { NHibernateUtil.String, NHibernateUtil.Boolean, NHibernateUtil.Int32 });
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            else
            {
                return null;
            }
        }

        public virtual void DeleteBudget(String taskSubType, Boolean isActive, Int32 year)
        {
            string hql = @"from Budget entity where entity.TaskSubType = ? and entity.IsActive = ? and entity.Year = ?";
            Delete(hql, new object[] { taskSubType, isActive, year }, new IType[] { NHibernateUtil.String, NHibernateUtil.Boolean, NHibernateUtil.Int32 });
        }

        #endregion Method Created By CodeSmith
    }
}
