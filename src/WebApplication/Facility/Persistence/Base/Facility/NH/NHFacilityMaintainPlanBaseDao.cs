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
    public class NHFacilityMaintainPlanBaseDao : NHDaoBase, IFacilityMaintainPlanBaseDao
    {
        public NHFacilityMaintainPlanBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityMaintainPlan(FacilityMaintainPlan entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityMaintainPlan> GetAllFacilityMaintainPlan()
        {
            return FindAll<FacilityMaintainPlan>();
        }

        public virtual FacilityMaintainPlan LoadFacilityMaintainPlan(Int32 id)
        {
            return FindById<FacilityMaintainPlan>(id);
        }

        public virtual void UpdateFacilityMaintainPlan(FacilityMaintainPlan entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityMaintainPlan(Int32 id)
        {
            string hql = @"from FacilityMaintainPlan entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityMaintainPlan(FacilityMaintainPlan entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityMaintainPlan(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityMaintainPlan entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityMaintainPlan(IList<FacilityMaintainPlan> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (FacilityMaintainPlan entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFacilityMaintainPlan(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
