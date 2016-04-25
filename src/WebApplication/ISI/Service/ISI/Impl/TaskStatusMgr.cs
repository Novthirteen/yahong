using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.Hql;
using System.Linq;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using NHibernate;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TaskStatusMgr : TaskStatusBaseMgr, ITaskStatusMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public ICommentDetailMgrE commentDetailMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskStatus> GetTaskStatus(string taskCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskStatus));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            criteria.AddOrder(Order.Desc("LastModifyDate"));
            IList<TaskStatus> taskStatusList = this.criteriaMgrE.FindAll<TaskStatus>(criteria);
            return taskStatusList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskStatus> GetTaskStatus(string taskCode, int firstRow, int maxRows)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskStatus));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            criteria.AddOrder(Order.Desc("LastModifyDate"));
            IList<TaskStatus> taskStatusList = this.criteriaMgrE.FindAll<TaskStatus>(criteria, firstRow, maxRows);
            return taskStatusList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IDictionary<string, IList<object>> GetTaskStatus(IList<string> taskCodeList, DateTime monday, DateTime lastMonday, DateTime lastLastMonday)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskStatus));
            criteria.Add(Expression.In("TaskCode", taskCodeList.ToArray()));
            //criteria.Add(Expression.Or(Expression.Like("TaskCode", ISIConstants.CODE_PREFIX_RESMATRIX, MatchMode.Start), Expression.Ge("LastModifyDate", lastMonday)));
            criteria.AddOrder(Order.Desc("LastModifyDate"));
            IList<TaskStatus> taskStatusList = this.criteriaMgrE.FindAll<TaskStatus>(criteria);
            IDictionary<string, IList<object>> taskStatusDic = new Dictionary<string, IList<object>>();
            if (taskStatusList != null && taskStatusList.Count > 0)
            {
                foreach (var code in taskCodeList)
                {
                    var thisTaskStatusList = taskStatusList.Where(t => t.TaskCode == code).ToList();
                    if (thisTaskStatusList != null && thisTaskStatusList.Count > 0)
                    {
                        int count = thisTaskStatusList.Count;
                        int surplus = count;
                        var thisMondayTaskStatusList = thisTaskStatusList.Where(t => t.LastModifyDate >= monday).ToList();
                        IList<IList<TaskStatus>> taskStatusListList = new List<IList<TaskStatus>>();
                        if ((thisMondayTaskStatusList == null || thisMondayTaskStatusList.Count == 0) && code.StartsWith(ISIConstants.CODE_PREFIX_RESMATRIX))
                        {
                            //如果是责任方针是显示上周的一条
                            taskStatusListList.Add(thisTaskStatusList.Take(1).ToList());
                            if (thisTaskStatusList.Count > 1)
                            {
                                var t = new List<TaskStatus>();
                                t.Add(thisTaskStatusList[1]);
                                taskStatusListList.Add(t);
                            }
                        }
                        else
                        {
                            var thisLastMondayTaskStatusList = thisTaskStatusList.Where(t => t.LastModifyDate < monday && t.LastModifyDate >= lastMonday).ToList();

                            if (thisMondayTaskStatusList != null && thisMondayTaskStatusList.Count > 0)
                            {
                                taskStatusListList.Add(thisMondayTaskStatusList);
                                surplus -= thisMondayTaskStatusList.Count;
                            }
                            if (thisLastMondayTaskStatusList != null && thisLastMondayTaskStatusList.Count > 0)
                            {
                                //如果是责任方针是显示上周的一条
                                if (code.StartsWith(ISIConstants.CODE_PREFIX_RESMATRIX))
                                {
                                    taskStatusListList.Add(thisLastMondayTaskStatusList.Take(1).ToList());
                                    surplus--;
                                }
                                else
                                {
                                    taskStatusListList.Add(thisLastMondayTaskStatusList);
                                    surplus -= thisLastMondayTaskStatusList.Count;
                                }
                            }
                            /*
                            if (taskStatusListList.Count != 2)
                            {
                                var thisLastLastMondayTaskStatusList = thisTaskStatusList.Where(t => t.LastModifyDate < lastMonday && t.LastModifyDate >= lastLastMonday).ToList();
                                if (thisLastLastMondayTaskStatusList != null && thisLastLastMondayTaskStatusList.Count > 0)
                                {
                                    taskStatusListList.Add(thisLastLastMondayTaskStatusList);
                                    surplus -= thisLastLastMondayTaskStatusList.Count;
                                }
                            }
                            */

                            if (taskStatusListList.Count < 2 && surplus > 0)
                            {
                                int num = 3;
                                if (code.StartsWith(ISIConstants.CODE_PREFIX_RESMATRIX))
                                {
                                    num = 1;
                                }

                                int count1 = 0;
                                int count2 = 0;
                                if (surplus >= num)
                                {
                                    count1 = 3;
                                    if (surplus >= num * 2)
                                    {
                                        count2 = num;
                                    }
                                    else
                                    {
                                        count2 = surplus - num;
                                    }
                                }
                                else
                                {
                                    count1 = surplus;
                                }

                                var surplusTaskStatusList = thisTaskStatusList.Skip(count - surplus);
                                if (count1 > 0)
                                {
                                    if (taskStatusListList.Count == 0)
                                    {
                                        taskStatusListList.Add(surplusTaskStatusList.Take(count1).ToList());
                                        if (count2 > 0)
                                        {
                                            taskStatusListList.Add(surplusTaskStatusList.Skip(count1).Take(count2).ToList());
                                        }
                                    }
                                    else if (taskStatusListList.Count == 1)
                                    {
                                        taskStatusListList.Add(surplusTaskStatusList.Take(count1).ToList());
                                    }
                                }
                            }
                        }

                        IList<object> objectList = new List<object>();
                        objectList.Add(taskStatusListList);

                        //当周数
                        if (thisMondayTaskStatusList != null)
                        {
                            objectList.Add(thisMondayTaskStatusList.Count);
                        }
                        else
                        {
                            objectList.Add(0);
                        }

                        //总数
                        objectList.Add(count);

                        //所有进展
                        objectList.Add(thisTaskStatusList);

                        taskStatusDic.Add(code, objectList);
                    }
                }
            }
            return taskStatusDic;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<TaskStatus>[] GetTaskStatus(string taskCode, int? currentCount, int? count, DateTime Monday, DateTime LastMonday, DateTime LastLastMonday)
        {
            if (count.HasValue && count.Value > 1)
            {
                IList<TaskStatus> taskStatusList = GetTaskStatus(taskCode, 0, currentCount.HasValue && currentCount.Value > 0 ? currentCount.Value + 3 : 10);

                IList<TaskStatus>[] taskStatusListArray = new List<TaskStatus>[2];

                if (currentCount.HasValue && currentCount.Value > 0)
                {
                    taskStatusListArray[0] = taskStatusList.Where(s => s.LastModifyDate >= Monday).ToList();
                    taskStatusListArray[1] = taskStatusList.Where(s => s.LastModifyDate < Monday).ToList();
                }
                else
                {
                    taskStatusListArray[0] = taskStatusList.Where(s => s.LastModifyDate >= LastMonday).ToList();
                    if (taskStatusListArray[0] == null || taskStatusListArray[0].Count == 0)
                    {
                        taskStatusListArray[0] = taskStatusList.Where(s => s.LastModifyDate >= LastLastMonday).ToList();
                    }
                    else
                    {
                        taskStatusListArray[1] = taskStatusList.Where(s => s.LastModifyDate < LastMonday).Take(3).ToList();
                    }

                    if (taskStatusListArray[0] == null || taskStatusListArray[0].Count == 0)
                    {
                        taskStatusListArray[0] = taskStatusList.Take(3).ToList();
                        if (taskStatusList.Count > 3)
                        {
                            taskStatusList.RemoveAt(0);
                            taskStatusList.RemoveAt(0);
                            taskStatusList.RemoveAt(0);
                            taskStatusListArray[1] = taskStatusList.Take(3).ToList();
                        }
                    }
                    else if (taskStatusListArray[1] == null || taskStatusListArray[1].Count == 0)
                    {
                        taskStatusListArray[1] = taskStatusList.Where(s => s.LastModifyDate < LastLastMonday).Take(3).ToList();
                    }
                }


                return taskStatusListArray;
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public object[] GetLastTaskStatus(string taskCode)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append("select ts.Flag,ts.Color from TaskStatus ts where ts.Id = (select max(ts1.Id) from TaskStatus ts1 where ts1.TaskCode=:TaskCode ) ");
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("TaskCode", taskCode);
            IList<object[]> objList = this.hqlMgrE.FindAll<object[]>(hql.ToString(), param);
            if (objList == null || objList.Count == 0) return null;
            return objList.FirstOrDefault();
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<object[]> GetLastTaskStatus(string[] taskCodes)
        {
            if (taskCodes == null || taskCodes.Length == 0) return null;
            StringBuilder hql = new StringBuilder();
            hql.Append("select ts.TaskCode,ts.CreateDate,ts.Desc from TaskStatus ts where ts.Id in (select max(ts1.Id) from TaskStatus ts1 where ts1.TaskCode in (:TaskCode) group by ts1.TaskCode ) ");
            IDictionary<string, object> param = new Dictionary<string, object>();
            param.Add("TaskCode", taskCodes);
            IList<object[]> objList = this.hqlMgrE.FindAll<object[]>(hql.ToString(), param);
            if (objList == null || objList.Count == 0) return null;
            return objList;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TaskStatusMgrE : com.Sconit.ISI.Service.Impl.TaskStatusMgr, ITaskStatusMgrE
    {
    }
}

#endregion Extend Class