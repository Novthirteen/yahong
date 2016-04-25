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
    public class NHFacilityDistributionBaseDao : NHDaoBase, IFacilityDistributionBaseDao
    {
        public NHFacilityDistributionBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityDistribution(FacilityDistribution entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityDistribution> GetAllFacilityDistribution()
        {
            return FindAll<FacilityDistribution>();
        }

        public virtual FacilityDistribution LoadFacilityDistribution(Int32 id)
        {
            return FindById<FacilityDistribution>(id);
        }

        public virtual void UpdateFacilityDistribution(FacilityDistribution entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityDistribution(Int32 id)
        {
            string hql = @"from FacilityDistribution entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityDistribution(FacilityDistribution entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityDistribution(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityDistribution entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityDistribution(IList<FacilityDistribution> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (FacilityDistribution entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFacilityDistribution(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
