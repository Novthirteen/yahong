using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.Facility.Entity;
using com.Sconit.Persistence;

//TODO: Add other using statmens here.

namespace com.Sconit.Facility.Persistence.NH
{
    public class NHMaintainPlanBaseDao : NHDaoBase, IMaintainPlanBaseDao
    {
        public NHMaintainPlanBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateMaintainPlan(MaintainPlan entity)
        {
            Create(entity);
        }

        public virtual IList<MaintainPlan> GetAllMaintainPlan()
        {
            return FindAll<MaintainPlan>();
        }

        public virtual MaintainPlan LoadMaintainPlan(String code)
        {
            return FindById<MaintainPlan>(code);
        }

        public virtual void UpdateMaintainPlan(MaintainPlan entity)
        {
            Update(entity);
        }

        public virtual void DeleteMaintainPlan(String code)
        {
            string hql = @"from MaintainPlan entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteMaintainPlan(MaintainPlan entity)
        {
            Delete(entity);
        }

        public virtual void DeleteMaintainPlan(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from MaintainPlan entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteMaintainPlan(IList<MaintainPlan> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (MaintainPlan entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteMaintainPlan(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
