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
    public class NHCostBaseDao : NHDaoBase, ICostBaseDao
    {
        public NHCostBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateCost(Cost entity)
        {
            Create(entity);
        }

        public virtual IList<Cost> GetAllCost()
        {
            return FindAll<Cost>();
        }

        public virtual Cost LoadCost(Int32 id)
        {
            return FindById<Cost>(id);
        }

        public virtual void UpdateCost(Cost entity)
        {
            Update(entity);
        }

        public virtual void DeleteCost(Int32 id)
        {
            string hql = @"from Cost entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteCost(Cost entity)
        {
            Delete(entity);
        }

        public virtual void DeleteCost(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Cost entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteCost(IList<Cost> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (Cost entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteCost(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
