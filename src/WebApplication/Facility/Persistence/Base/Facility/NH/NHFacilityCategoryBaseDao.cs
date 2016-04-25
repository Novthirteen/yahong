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
    public class NHFacilityCategoryBaseDao : NHDaoBase, IFacilityCategoryBaseDao
    {
        public NHFacilityCategoryBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityCategory(FacilityCategory entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityCategory> GetAllFacilityCategory()
        {
            return FindAll<FacilityCategory>();
        }

        public virtual FacilityCategory LoadFacilityCategory(String code)
        {
            return FindById<FacilityCategory>(code);
        }

        public virtual void UpdateFacilityCategory(FacilityCategory entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityCategory(String code)
        {
            string hql = @"from FacilityCategory entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteFacilityCategory(FacilityCategory entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityCategory(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityCategory entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityCategory(IList<FacilityCategory> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (FacilityCategory entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteFacilityCategory(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
