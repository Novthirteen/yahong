using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using NHibernate;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Ext;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class CommentDetailMgr : CommentDetailBaseMgr, ICommentDetailMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<Comment> GetComment(string taskCode, int firstRow, int maxRows)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CommentDetail));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            ICriteria c = criteria.GetExecutableCriteria(this.daoBase.GetSession());
            ProjectionList list = Projections.ProjectionList().Create();
            list.Add(Projections.Distinct(Projections.Property("Id")));
            list.Add(Projections.GroupProperty("Id"), "Id");
            list.Add(Projections.GroupProperty("Comment"), "Value");
            list.Add(Projections.GroupProperty("TaskCode"), "TaskCode");
            list.Add(Projections.GroupProperty("CreateDate"), "CreateDate");
            list.Add(Projections.GroupProperty("CreateUserNm"), "CreateUser");
            criteria.SetProjection(list);
            criteria.AddOrder(Order.Desc("CreateDate"));
            criteria.AddOrder(Order.Desc("Id"));
            c.SetFirstResult(firstRow);
            c.SetMaxResults(maxRows);
            c.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Comment)));

            return c.List<Comment>();
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<CommentDetail> GetComment(string taskCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CommentDetail));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            criteria.AddOrder(Order.Desc("LastModifyDate"));
            var commentList = this.criteriaMgrE.FindAll<CommentDetail>(criteria);
            return commentList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IDictionary<string, IList<object>> GetComment(IList<string> taskCodeList, DateTime monday, DateTime lastMonday, DateTime lastLastMonday)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CommentDetail));
            criteria.Add(Expression.In("TaskCode", taskCodeList.ToArray()));
            ICriteria c = criteria.GetExecutableCriteria(this.daoBase.GetSession());
            ProjectionList list = Projections.ProjectionList().Create();
            list.Add(Projections.Distinct(Projections.Property("Id")));
            list.Add(Projections.GroupProperty("Id"), "Id");
            list.Add(Projections.GroupProperty("Comment"), "Value");
            list.Add(Projections.GroupProperty("TaskCode"), "TaskCode");
            list.Add(Projections.GroupProperty("CreateDate"), "CreateDate");
            list.Add(Projections.GroupProperty("CreateUserNm"), "CreateUser");
            criteria.SetProjection(list);
            criteria.AddOrder(Order.Desc("CreateDate"));
            criteria.AddOrder(Order.Desc("Id"));
            c.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Comment)));

            IList<Comment> commentList = c.List<Comment>();
            IDictionary<string, IList<object>> commentDic = new Dictionary<string, IList<object>>();
            if (commentList != null && commentList.Count > 0)
            {
                foreach (var code in taskCodeList)
                {
                    var thisCommentList = commentList.Where(t => t.TaskCode == code).ToList();
                    if (thisCommentList != null && thisCommentList.Count > 0)
                    {
                        int count = thisCommentList.Count;
                        int surplus = count;
                        var thisMondayCommentList = thisCommentList.Where(t => t.CreateDate >= monday).ToList();
                        var thisLastMondayCommentList = thisCommentList.Where(t => t.CreateDate < monday && t.CreateDate >= lastMonday).ToList();
                        IList<IList<Comment>> commentListList = new List<IList<Comment>>();
                        if (thisMondayCommentList != null && thisMondayCommentList.Count > 0)
                        {
                            commentListList.Add(thisMondayCommentList);
                            surplus -= thisMondayCommentList.Count;
                        }
                        if (thisLastMondayCommentList != null && thisLastMondayCommentList.Count > 0)
                        {
                            commentListList.Add(thisLastMondayCommentList);
                            surplus -= thisLastMondayCommentList.Count;
                        }
                        /*
                        if (commentListList.Count != 2)
                        {
                            var thisLastLastMondayCommentList = thisCommentList.Where(t => t.LastModifyDate < lastMonday && t.LastModifyDate >= lastLastMonday).ToList();
                            if (thisLastLastMondayCommentList != null && thisLastLastMondayCommentList.Count > 0)
                            {
                                commentListList.Add(thisLastLastMondayCommentList);
                                surplus -= thisLastLastMondayCommentList.Count;
                            }
                        }
                        */
                        if (commentListList.Count < 2 && surplus > 0)
                        {
                            int count1 = 0;
                            int count2 = 0;
                            if (surplus >= 5)
                            {
                                count1 = 5;
                                if (surplus >= 10)
                                {
                                    count2 = 5;
                                }
                                else
                                {
                                    count2 = surplus - 5;
                                }
                            }
                            else
                            {
                                count1 = surplus;
                            }

                            var surplusCommentList = thisCommentList.Skip(count - surplus);
                            if (count1 > 0)
                            {
                                if (commentListList.Count == 0)
                                {
                                    commentListList.Add(surplusCommentList.Take(count1).ToList());
                                    if (count2 > 0)
                                    {
                                        commentListList.Add(surplusCommentList.Skip(count1).Take(count2).ToList());
                                    }
                                }
                                else if (commentListList.Count == 1)
                                {
                                    commentListList.Add(surplusCommentList.Take(count1).ToList());
                                }
                            }
                        }

                        IList<object> objectList = new List<object>();
                        objectList.Add(commentListList);

                        //当周数
                        if (thisMondayCommentList != null)
                        {
                            objectList.Add(thisMondayCommentList.Count);
                        }
                        else
                        {
                            objectList.Add(0);
                        }

                        //总数
                        objectList.Add(count);

                        //所有评论
                        objectList.Add(thisCommentList);

                        commentDic.Add(code, objectList);
                    }
                }
            }
            return commentDic;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Comment>[] GetComment(string taskCode, int? currentCount, int? count, DateTime Monday, DateTime LastMonday, DateTime LastLastMonday)
        {
            if (count.HasValue && count.Value > 1)
            {
                IList<Comment> commentList = GetComment(taskCode, 0, currentCount.HasValue && currentCount.Value > 0 ? currentCount.Value + 5 : 10);

                IList<Comment>[] commentListArray = new List<Comment>[2];

                if (currentCount.HasValue && currentCount.Value > 0)
                {
                    commentListArray[0] = commentList.Where(s => s.CreateDate >= Monday).ToList();
                    commentListArray[1] = commentList.Where(s => s.CreateDate < Monday).ToList();
                }
                else
                {
                    commentListArray[0] = commentList.Where(s => s.CreateDate >= LastMonday).ToList();
                    if (commentListArray[0] == null || commentListArray[0].Count == 0)
                    {
                        commentListArray[0] = commentList.Where(s => s.CreateDate >= LastLastMonday).ToList();
                    }
                    else
                    {
                        commentListArray[1] = commentList.Where(s => s.CreateDate < LastMonday).Take(5).ToList();
                    }

                    if (commentListArray[0] == null || commentListArray[0].Count == 0)
                    {
                        commentListArray[0] = commentList.Take(3).ToList();
                        if (commentList.Count > 3)
                        {
                            commentList.RemoveAt(0);
                            commentList.RemoveAt(0);
                            commentList.RemoveAt(0);
                            commentListArray[1] = commentList.Take(5).ToList();
                        }
                    }
                    else if (commentListArray[1] == null || commentListArray[1].Count == 0)
                    {
                        commentListArray[1] = commentList.Where(s => s.CreateDate < LastLastMonday).Take(5).ToList();
                    }
                }

                return commentListArray;
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetCommentCount(string taskCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CommentDetail));
            criteria.Add(Expression.Eq("TaskCode", taskCode));

            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Id")));

            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return count[0];
            }
            else
            {
                return 0;
            }
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class CommentDetailMgrE : com.Sconit.ISI.Service.Impl.CommentDetailMgr, ICommentDetailMgrE
    {
    }
}

#endregion Extend Class