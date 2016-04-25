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
    public class NHFacilityTransBaseDao : NHDaoBase, IFacilityTransBaseDao
    {
        public NHFacilityTransBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateFacilityTrans(FacilityTrans entity)
        {
            Create(entity);
        }

        public virtual IList<FacilityTrans> GetAllFacilityTrans()
        {
            return FindAll<FacilityTrans>();
        }

        public virtual FacilityTrans LoadFacilityTrans(Int32 id)
        {
            return FindById<FacilityTrans>(id);
        }

        public virtual void UpdateFacilityTrans(FacilityTrans entity)
        {
            Update(entity);
        }

        public virtual void DeleteFacilityTrans(Int32 id)
        {
            string hql = @"from FacilityTrans entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteFacilityTrans(FacilityTrans entity)
        {
            Delete(entity);
        }

        public virtual void DeleteFacilityTrans(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from FacilityTrans entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteFacilityTrans(IList<FacilityTrans> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (FacilityTrans entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteFacilityTrans(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
