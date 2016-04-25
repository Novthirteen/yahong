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
    public class NHFacilityMasterBaseDao : NHDaoBase, IFacilityMasterBaseDao
    {
        public NHFacilityMasterBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityMaster(FacilityMaster entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityMaster> GetAllFacilityMaster()
        {
            return FindAll<FacilityMaster>();
        }

        public virtual FacilityMaster LoadFacilityMaster(String fCID)
        {
            return FindById<FacilityMaster>(fCID);
        }

        public virtual void UpdateFacilityMaster(FacilityMaster entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityMaster(String fCID)
        {
            string hql = @"from FacilityMaster entity where entity.FCID = ?";
            Delete(hql, new object[] { fCID }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteFacilityMaster(FacilityMaster entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityMaster(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityMaster entity where entity.FCID in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityMaster(IList<FacilityMaster> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (FacilityMaster entity in entityList)
            {
                pkList.Add(entity.FCID);
            }

            DeleteFacilityMaster(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
