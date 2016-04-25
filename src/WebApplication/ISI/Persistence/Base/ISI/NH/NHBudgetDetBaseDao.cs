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
    public class NHBudgetDetBaseDao : NHDaoBase, IBudgetDetBaseDao
    {
        public NHBudgetDetBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateBudgetDet(BudgetDet entity)
        {
            Create(entity);
        }

        public virtual IList<BudgetDet> GetAllBudgetDet()
        {
            return FindAll<BudgetDet>();
        }

        public virtual BudgetDet LoadBudgetDet(Int32 id)
        {
            return FindById<BudgetDet>(id);
        }

        public virtual void UpdateBudgetDet(BudgetDet entity)
        {
            Update(entity);
        }

        public virtual void DeleteBudgetDet(Int32 id)
        {
            string hql = @"from BudgetDet entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteBudgetDet(BudgetDet entity)
        {
            Delete(entity);
        }

        public virtual void DeleteBudgetDet(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from BudgetDet entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteBudgetDet(IList<BudgetDet> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (BudgetDet entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteBudgetDet(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
