using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TaskAddressMgr : TaskAddressBaseMgr, ITaskAddressMgr
    {

        private static IList<TaskAddress> cachedAllTaskAddress;
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public void UpdateTaskAddress(TaskAddress taskAddress, User user)
        {

            TaskAddress oldTaskAddress = this.CheckAndLoadTaskAddress(taskAddress.Code);
            oldTaskAddress.Desc = taskAddress.Desc;
            oldTaskAddress.Seq = taskAddress.Seq;
            oldTaskAddress.Parent = taskAddress.Parent;
            oldTaskAddress.LastModifyDate = DateTime.Now;
            oldTaskAddress.LastModifyUser = user.Code;

            this.UpdateTaskAddress(oldTaskAddress);


        }
        [Transaction(TransactionMode.Unspecified)]
        public TaskAddress CheckAndLoadTaskAddress(string code)
        {
            TaskAddress taskAddress = this.LoadTaskAddress(code);
            if (taskAddress != null)
            {
                return taskAddress;
            }
            else
            {
                throw new BusinessErrorException("ISI.TaskAddress.UserCodeNotExist", code);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskAddress> GetTaskAddressByParent(string code)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskAddress));
            criteria.Add(Expression.Eq("Parent.Code", code));
            criteria.AddOrder(Order.Asc("Seq"));
            return criteriaMgrE.FindAll<TaskAddress>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskAddress> GetCacheAllTaskAddress()
        {
            if (cachedAllTaskAddress == null)
            {
                cachedAllTaskAddress = GetAllTaskAddress();
            }
            else
            {
                //检查IssueType大小是否发生变化
                DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskAddress));
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Code")));
                IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);

                if (count[0] != cachedAllTaskAddress.Count)
                {
                    cachedAllTaskAddress = GetAllTaskAddress();
                }
            }

            if (cachedAllTaskAddress != null && cachedAllTaskAddress.Count > 0)
            {
                foreach (TaskAddress addr in cachedAllTaskAddress)
                {
                    addr.Name = GetName(addr);
                }
            }

            return cachedAllTaskAddress;
        }

        [Transaction(TransactionMode.Unspecified)]
        public override IList<TaskAddress> GetAllTaskAddress()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskAddress));
            criteria.AddOrder(Order.Asc("Seq"));
            return this.criteriaMgrE.FindAll<TaskAddress>(criteria);
        }

        public string GetName(TaskAddress addr)
        {
            string result = string.Empty;
            if (addr.Parent == null)
            {
                return addr.Desc;
            }
            else
            {
                result = GetName(addr.Parent) + "|" + addr.Desc;
            }

            return result;
        }


        [Transaction(TransactionMode.Unspecified)]
        public bool IsRef(string code)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskAddress)).SetProjection(Projections.CountDistinct("Code"));
            criteria.Add(Expression.Eq("Parent.Code", code));
            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            /* 脱离引用
            DetachedCriteria criteriaIssueHead = DetachedCriteria.For(typeof(IssueHead)).SetProjection(Projections.CountDistinct("Code"));
            criteriaIssueHead.Add(Expression.Eq("IssueAddress.Id", id));
            count = this.criteriaMgrE.FindAll<int>(criteriaIssueHead);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }
            */
            return false;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskAddress> GetTaskAddressNotCode(string code)
        {
            TaskAddress taskAddress = this.LoadTaskAddress(code);

            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskAddress));
            criteria.Add(Expression.Not(Expression.In("Code", (ICollection)GetChildrenCode(taskAddress))));
            return this.criteriaMgrE.FindAll<TaskAddress>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<string> GetChildrenCode(TaskAddress taskAddress)
        {
            IList<string> taskAddressList = new List<string>();

            IList<TaskAddress> children = GetChildren(taskAddress);
            if (children != null && children.Count > 0)
            {
                foreach (TaskAddress task in children)
                {
                    taskAddressList.Add(task.Code);
                }
            }

            return taskAddressList;
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<TaskAddress> GetChildren(TaskAddress taskAddress)
        {
            IList<TaskAddress> taskList = new List<TaskAddress>();
            taskList.Add(taskAddress);
            IList<TaskAddress> children = taskAddress.Children;
            if (children != null && children.Count > 0)
            {
                foreach (TaskAddress child in children)
                {
                    IList<TaskAddress> taskChildren = GetChildren(child);
                    foreach (TaskAddress task in taskChildren)
                    {
                        taskList.Add(task);
                    }
                }
            }
            return taskList;
        }


        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskAddressMgrE : com.Sconit.ISI.Service.Impl.TaskAddressMgr, ITaskAddressMgrE
    {
    }
}

#endregion Extend Class