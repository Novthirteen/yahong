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
    public class NHTaskWeekStatisticsViewBaseDao : NHDaoBase, ITaskWeekStatisticsViewBaseDao
    {
        public NHTaskWeekStatisticsViewBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateTaskWeekStatisticsView(TaskWeekStatisticsView entity)
        {
            Create(entity);
        }

        public virtual IList<TaskWeekStatisticsView> GetAllTaskWeekStatisticsView()
        {
            return FindAll<TaskWeekStatisticsView>();
        }

        public virtual TaskWeekStatisticsView LoadTaskWeekStatisticsView(String code)
        {
            return FindById<TaskWeekStatisticsView>(code);
        }

        public virtual void UpdateTaskWeekStatisticsView(TaskWeekStatisticsView entity)
        {
            Update(entity);
        }

        public virtual void DeleteTaskWeekStatisticsView(String code)
        {
            string hql = @"from TaskWeekStatisticsView entity where entity.Code = ?";
            Delete(hql, new object[] { code }, new IType[] { NHibernateUtil.String });
        }

        public virtual void DeleteTaskWeekStatisticsView(TaskWeekStatisticsView entity)
        {
            Delete(entity);
        }

        public virtual void DeleteTaskWeekStatisticsView(IList<String> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from TaskWeekStatisticsView entity where entity.Code in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteTaskWeekStatisticsView(IList<TaskWeekStatisticsView> entityList)
        {
            IList<String> pkList = new List<String>();
            foreach (TaskWeekStatisticsView entity in entityList)
            {
                pkList.Add(entity.Code);
            }

            DeleteTaskWeekStatisticsView(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
