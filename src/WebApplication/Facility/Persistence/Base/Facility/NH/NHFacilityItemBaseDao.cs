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
    public class NHFacilityItemBaseDao : NHDaoBase, IFacilityItemBaseDao
    {
        public NHFacilityItemBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityItem(FacilityItem entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityItem> GetAllFacilityItem()
        {
            return GetAllFacilityItem(false);
        }

        public virtual IList<FacilityItem> GetAllFacilityItem(bool includeInactive)
        {
            string hql = @"from FacilityItem entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<FacilityItem> result = FindAllWithCustomQuery<FacilityItem>(hql);
            return result;
        }

        public virtual FacilityItem LoadFacilityItem(Int32 id)
        {
            return FindById<FacilityItem>(id);
        }

        public virtual void UpdateFacilityItem(FacilityItem entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityItem(Int32 id)
        {
            string hql = @"from FacilityItem entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityItem(FacilityItem entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityItem(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityItem entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityItem(IList<FacilityItem> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (FacilityItem entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFacilityItem(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
