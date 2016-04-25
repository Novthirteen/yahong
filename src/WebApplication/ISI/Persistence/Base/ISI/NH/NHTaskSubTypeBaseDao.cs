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
    public class NHTaskSubTypeBaseDao : NHDaoBase, ITaskSubTypeBaseDao
    {
        public NHTaskSubTypeBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskSubType(TaskSubType entity)
        {
            Create(entity);
        }

        public virtual IList<TaskSubType> GetAllTaskSubType()
        {
            return GetAllTaskSubType(false);
        }

        public virtual IList<TaskSubType> GetAllTaskSubType(bool includeInactive)
        {
            string hql = @"from TaskSubType entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<TaskSubType> result = FindAllWithCustomQuery<TaskSubType>(hql);
            return result;
        }

        public virtual TaskSubType LoadTaskSubType(String code)
        {
            return FindById<TaskSubType>(code);
        }

        public virtual void UpdateTaskSubType(TaskSubType entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskSubType(String code)
        {
            string hql = @"from TaskSubType entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteTaskSubType(TaskSubType entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskSubType(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskSubType entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskSubType(IList<TaskSubType> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (TaskSubType entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteTaskSubType(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
