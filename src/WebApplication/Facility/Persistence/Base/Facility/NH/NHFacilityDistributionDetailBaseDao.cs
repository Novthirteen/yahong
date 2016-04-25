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
    public class NHFacilityDistributionDetailBaseDao : NHDaoBase, IFacilityDistributionDetailBaseDao
    {
        public NHFacilityDistributionDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityDistributionDetail> GetAllFacilityDistributionDetail()
        {
            return FindAll<FacilityDistributionDetail>();
        }

        public virtual FacilityDistributionDetail LoadFacilityDistributionDetail(Int32 id)
        {
            return FindById<FacilityDistributionDetail>(id);
        }

        public virtual void UpdateFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityDistributionDetail(Int32 id)
        {
            string hql = @"from FacilityDistributionDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityDistributionDetail(IList<Int32> pkList)
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


        #endregion Method Created By CodeSmith
    }
}
