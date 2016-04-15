using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Type;
using com.Sconit.Entity.MRP;

//TODO: Add other using statmens here.

namespace com.Sconit.Persistence.MRP.NH
{
    public class NHMrpLocationLotDetailBaseDao : NHDaoBase, IMrpLocationLotDetailBaseDao
    {
        public NHMrpLocationLotDetailBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateMrpLocationLotDetail(MrpLocationLotDetail entity)
        {
            Create(entity);
        }

        public virtual IList<MrpLocationLotDetail> GetAllMrpLocationLotDetail()
        {
            return FindAll<MrpLocationLotDetail>();
        }

        public virtual MrpLocationLotDetail LoadMrpLocationLotDetail(Int32 id)
        {
            return FindById<MrpLocationLotDetail>(id);
        }

        public virtual void UpdateMrpLocationLotDetail(MrpLocationLotDetail entity)
        {
            Update(entity);
        }

        public virtual void DeleteMrpLocationLotDetail(Int32 id)
        {
            string hql = @"from MrpLocationLotDetail entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteMrpLocationLotDetail(MrpLocationLotDetail entity)
        {
            Delete(entity);
        }

        public virtual void DeleteMrpLocationLotDetail(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from MrpLocationLotDetail entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteMrpLocationLotDetail(IList<MrpLocationLotDetail> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (MrpLocationLotDetail entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteMrpLocationLotDetail(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
