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
    public class NHFacilityStockMasterBaseDao : NHDaoBase, IFacilityStockMasterBaseDao
    {
        public NHFacilityStockMasterBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityStockMaster(FacilityStockMaster entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityStockMaster> GetAllFacilityStockMaster()
        {
            return FindAll<FacilityStockMaster>();
        }

        public virtual FacilityStockMaster LoadFacilityStockMaster(String stNo)
        {
            return FindById<FacilityStockMaster>(stNo);
        }

        public virtual void UpdateFacilityStockMaster(FacilityStockMaster entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityStockMaster(String stNo)
        {
            string hql = @"from FacilityStockMaster entity where entity.StNo = ?";
            Delete(hql, new object[] { stNo }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteFacilityStockMaster(FacilityStockMaster entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityStockMaster(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityStockMaster entity where entity.StNo in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityStockMaster(IList<FacilityStockMaster> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (FacilityStockMaster entity in entityList)
            {
                pkList.Add(entity.StNo);
            }

            DeleteFacilityStockMaster(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
