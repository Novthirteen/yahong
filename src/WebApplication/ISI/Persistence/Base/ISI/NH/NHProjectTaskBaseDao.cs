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
    public class NHProjectTaskBaseDao : NHDaoBase, IProjectTaskBaseDao
    {
        public NHProjectTaskBaseDao(ISessionManager sessionManager)
            : base(sessionManager)
        {
        }

        #region Method Created By CodeSmith

        public virtual void CreateProjectTask(ProjectTask entity)
        {
            Create(entity);
        }

        public virtual IList<ProjectTask> GetAllProjectTask()
        {
            return GetAllProjectTask(false);
        }

        public virtual IList<ProjectTask> GetAllProjectTask(bool includeInactive)
        {
            string hql = @"from ProjectTask entity";
            if (!includeInactive)
            {
                hql += " where entity.IsActive = 1";
            }
            IList<ProjectTask> result = FindAllWithCustomQuery<ProjectTask>(hql);
            return result;
        }

        public virtual ProjectTask LoadProjectTask(Int32 id)
        {
            return FindById<ProjectTask>(id);
        }

        public virtual void UpdateProjectTask(ProjectTask entity)
        {
            Update(entity);
        }

        public virtual void DeleteProjectTask(Int32 id)
        {
            string hql = @"from ProjectTask entity where entity.Id = ?";
            Delete(hql, new object[] { id }, new IType[] { NHibernateUtil.Int32 });
        }

        public virtual void DeleteProjectTask(ProjectTask entity)
        {
            Delete(entity);
        }

        public virtual void DeleteProjectTask(IList<Int32> pkList)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("from ProjectTask entity where entity.Id in (");
            hql.Append(pkList[0]);
            for (int i = 1; i < pkList.Count; i++)
            {
                hql.Append(",");
                hql.Append(pkList[i]);
            }
            hql.Append(")");

            Delete(hql.ToString());
        }

        public virtual void DeleteProjectTask(IList<ProjectTask> entityList)
        {
            IList<Int32> pkList = new List<Int32>();
            foreach (ProjectTask entity in entityList)
            {
                pkList.Add(entity.Id);
            }

            DeleteProjectTask(pkList);
        }


        #endregion Method Created By CodeSmith
    }
}
