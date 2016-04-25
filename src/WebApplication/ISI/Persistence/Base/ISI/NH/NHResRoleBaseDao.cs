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
    public class NHResRoleBaseDao : NHDaoBase, IResRoleBaseDao
    {
        public NHResRoleBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateResRole(ResRole entity)
        {
            Create(entity);
        }

        public virtual IList<ResRole> GetAllResRole()
        {
            return GetAllResRole(false);
        }

        public virtual IList<ResRole> GetAllResRole(bool includeInactive)
        {
            string hql = @"from ResRole entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<ResRole> result = FindAllWithCustomQuery<ResRole>(hql);
            return result;
        }

        public virtual ResRole LoadResRole(String code)
        {
            return FindById<ResRole>(code);
        }

        public virtual void UpdateResRole(ResRole entity)
        {
            Update(entity);
        }

        public virtual void DeleteResRole(String code)
        {
            string hql = @"from ResRole entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteResRole(ResRole entity)
        {
            Delete(entity);
        }

        public virtual void DeleteResRole(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ResRole entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteResRole(IList<ResRole> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (ResRole entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteResRole(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
