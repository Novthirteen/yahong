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
    public class NHPositionBaseDao : NHDaoBase, IPositionBaseDao
    {
        public NHPositionBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreatePosition(Position entity)
        {
            Create(entity);
        }

        public virtual IList<Position> GetAllPosition()
        {
            return FindAll<Position>();
        }

        public virtual Position LoadPosition(String position)
        {
            return FindById<Position>(position);
        }

        public virtual void UpdatePosition(Position entity)
        {
            Update(entity);
        }

        public virtual void DeletePosition(String position)
        {
            string hql = @"from Position entity where entity.Position = ?";
            Delete(hql, new object[] { position }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeletePosition(Position entity)
        {
            Delete(entity);
        }

        public virtual void DeletePosition(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from Position entity where entity.Position in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeletePosition(IList<Position> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (Position entity in entityList)
            {
                pkList.Add(entity.Position);
            }

            DeletePosition(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
