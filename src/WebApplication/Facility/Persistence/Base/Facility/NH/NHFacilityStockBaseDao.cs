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
    public class NHFacilityStockBaseDao : NHDaoBase, IFacilityStockBaseDao
    {
        public NHFacilityStockBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityStock(FacilityStock entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityStock> GetAllFacilityStock()
        {
            return FindAll<FacilityStock>();
        }

        public virtual FacilityStock LoadFacilityStock(Int32 id)
        {
            return FindById<FacilityStock>(id);
        }

        public virtual void UpdateFacilityStock(FacilityStock entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityStock(Int32 id)
        {
            string hql = @"from FacilityStock entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityStock(FacilityStock entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityStock(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityStock entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityStock(IList<FacilityStock> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (FacilityStock entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFacilityStock(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
