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
    public class NHFacilityStockDetailBaseDao : NHDaoBase, IFacilityStockDetailBaseDao
    {
        public NHFacilityStockDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityStockDetail(FacilityStockDetail entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityStockDetail> GetAllFacilityStockDetail()
        {
            return FindAll<FacilityStockDetail>();
        }

        public virtual FacilityStockDetail LoadFacilityStockDetail(Int32 id)
        {
            return FindById<FacilityStockDetail>(id);
        }

        public virtual void UpdateFacilityStockDetail(FacilityStockDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityStockDetail(Int32 id)
        {
            string hql = @"from FacilityStockDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityStockDetail(FacilityStockDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityStockDetail(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityStockDetail entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityStockDetail(IList<FacilityStockDetail> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (FacilityStockDetail entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFacilityStockDetail(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
