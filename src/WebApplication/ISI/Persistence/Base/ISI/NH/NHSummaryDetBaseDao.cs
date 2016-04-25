using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.ISI.Entity;
using com.Sconit.Persistence;

//TODO: Add other using statmens here.

namespace com.Sconit.ISI.Persistence.NH
{
    public class NHSummaryDetBaseDao : NHDaoBase, ISummaryDetBaseDao
    {
        public NHSummaryDetBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateSummaryDet(SummaryDet entity)
        {
            Create(entity);
        }

        public virtual IList<SummaryDet> GetAllSummaryDet()
        {
            return FindAll<SummaryDet>();
        }

        public virtual SummaryDet LoadSummaryDet(Int32 id)
        {
            return FindById<SummaryDet>(id);
        }

        public virtual void UpdateSummaryDet(SummaryDet entity)
        {
            Update(entity);
        }

        public virtual void DeleteSummaryDet(Int32 id)
        {
            string hql = @"from SummaryDet entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteSummaryDet(SummaryDet entity)
        {
            Delete(entity);
        }

        public virtual void DeleteSummaryDet(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from SummaryDet entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteSummaryDet(IList<SummaryDet> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (SummaryDet entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteSummaryDet(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
