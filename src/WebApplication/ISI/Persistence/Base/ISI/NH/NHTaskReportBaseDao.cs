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
    public class NHTaskReportBaseDao : NHDaoBase, ITaskReportBaseDao
    {
        public NHTaskReportBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskReport(TaskReport entity)
        {
            Create(entity);
        }

        public virtual IList<TaskReport> GetAllTaskReport()
        {
            return GetAllTaskReport(false);
        }

        public virtual IList<TaskReport> GetAllTaskReport(bool includeInactive)
        {
            string hql = @"from TaskReport entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<TaskReport> result = FindAllWithCustomQuery<TaskReport>(hql);
            return result;
        }

        public virtual TaskReport LoadTaskReport(Int32 id)
        {
            return FindById<TaskReport>(id);
        }

        public virtual void UpdateTaskReport(TaskReport entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskReport(Int32 id)
        {
            string hql = @"from TaskReport entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteTaskReport(TaskReport entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskReport(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskReport entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskReport(IList<TaskReport> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (TaskReport entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteTaskReport(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
