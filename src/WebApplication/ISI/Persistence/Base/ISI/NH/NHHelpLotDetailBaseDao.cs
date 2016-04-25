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
    public class NHHelpLotDetailBaseDao : NHDaoBase, IHelpLotDetailBaseDao
    {
        public NHHelpLotDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateHelpLotDetail(HelpLotDetail entity)
        {
            Create(entity);
        }

        public virtual IList<HelpLotDetail> GetAllHelpLotDetail()
        {
            return FindAll<HelpLotDetail>();
        }

        public virtual HelpLotDetail LoadHelpLotDetail(Int32 id)
        {
            return FindById<HelpLotDetail>(id);
        }

        public virtual void UpdateHelpLotDetail(HelpLotDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteHelpLotDetail(Int32 id)
        {
            string hql = @"from HelpLotDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteHelpLotDetail(HelpLotDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteHelpLotDetail(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from HelpLotDetail entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteHelpLotDetail(IList<HelpLotDetail> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (HelpLotDetail entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteHelpLotDetail(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
