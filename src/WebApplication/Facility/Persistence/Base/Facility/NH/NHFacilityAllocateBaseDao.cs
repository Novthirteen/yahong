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
    public class NHFacilityAllocateBaseDao : NHDaoBase, IFacilityAllocateBaseDao
    {
        public NHFacilityAllocateBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityAllocate(FacilityAllocate entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityAllocate> GetAllFacilityAllocate()
        {
            return GetAllFacilityAllocate(false);
        }

        public virtual IList<FacilityAllocate> GetAllFacilityAllocate(bool includeInactive)
        {
            string hql = @"from FacilityAllocate entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<FacilityAllocate> result = FindAllWithCustomQuery<FacilityAllocate>(hql);
            return result;
        }

        public virtual FacilityAllocate LoadFacilityAllocate(Int32 id)
        {
            return FindById<FacilityAllocate>(id);
        }

        public virtual void UpdateFacilityAllocate(FacilityAllocate entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityAllocate(Int32 id)
        {
            string hql = @"from FacilityAllocate entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityAllocate(FacilityAllocate entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityAllocate(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityAllocate entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityAllocate(IList<FacilityAllocate> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (FacilityAllocate entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFacilityAllocate(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
