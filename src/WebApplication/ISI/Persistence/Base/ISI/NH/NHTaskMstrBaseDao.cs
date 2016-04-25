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
    public class NHTaskMstrBaseDao : NHDaoBase, ITaskMstrBaseDao
    {
        public NHTaskMstrBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskMstr(TaskMstr entity)
        {
            Create(entity);
        }

        public virtual IList<TaskMstr> GetAllTaskMstr()
        {
            return FindAll<TaskMstr>();
        }

        public virtual TaskMstr LoadTaskMstr(String code)
        {
            return FindById<TaskMstr>(code);
        }

        public virtual void UpdateTaskMstr(TaskMstr entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskMstr(String code)
        {
            string hql = @"from TaskMstr entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteTaskMstr(TaskMstr entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskMstr(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskMstr entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskMstr(IList<TaskMstr> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (TaskMstr entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteTaskMstr(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
