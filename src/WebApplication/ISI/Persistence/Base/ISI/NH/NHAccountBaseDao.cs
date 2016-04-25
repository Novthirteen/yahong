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
    public class NHAccountBaseDao : NHDaoBase, IAccountBaseDao
    {
        public NHAccountBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateAccount(Account entity)
        {
            Create(entity);
        }

        public virtual IList<Account> GetAllAccount()
        {
            return FindAll<Account>();
        }

        public virtual Account LoadAccount(String code)
        {
            return FindById<Account>(code);
        }

        public virtual void UpdateAccount(Account entity)
        {
            Update(entity);
        }

        public virtual void DeleteAccount(String code)
        {
            string hql = @"from Account entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteAccount(Account entity)
        {
            Delete(entity);
        }

        public virtual void DeleteAccount(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Account entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteAccount(IList<Account> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (Account entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteAccount(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
