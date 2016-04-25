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
    public class NHSummaryBaseDao : NHDaoBase, ISummaryBaseDao
    {
        public NHSummaryBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateSummary(Summary entity)
        {
            Create(entity);
        }

        public virtual IList<Summary> GetAllSummary()
        {
            return FindAll<Summary>();
        }

        public virtual Summary LoadSummary(String code)
        {
            return FindById<Summary>(code);
        }

        public virtual void UpdateSummary(Summary entity)
        {
            Update(entity);
        }

        public virtual void DeleteSummary(String code)
        {
            string hql = @"from Summary entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteSummary(Summary entity)
        {
            Delete(entity);
        }

        public virtual void DeleteSummary(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Summary entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteSummary(IList<Summary> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (Summary entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteSummary(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
