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
    public class NHResWokShopBaseDao : NHDaoBase, IResWokShopBaseDao
    {
        public NHResWokShopBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateResWokShop(ResWokShop entity)
        {
            Create(entity);
        }

        public virtual IList<ResWokShop> GetAllResWokShop()
        {
            return GetAllResWokShop(false);
        }

        public virtual IList<ResWokShop> GetAllResWokShop(bool includeInactive)
        {
            string hql = @"from ResWokShop entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<ResWokShop> result = FindAllWithCustomQuery<ResWokShop>(hql);
            return result;
        }

        public virtual ResWokShop LoadResWokShop(String code)
        {
            return FindById<ResWokShop>(code);
        }

        public virtual void UpdateResWokShop(ResWokShop entity)
        {
            Update(entity);
        }

        public virtual void DeleteResWokShop(String code)
        {
            string hql = @"from ResWokShop entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteResWokShop(ResWokShop entity)
        {
            Delete(entity);
        }

        public virtual void DeleteResWokShop(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ResWokShop entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteResWokShop(IList<ResWokShop> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (ResWokShop entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteResWokShop(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
