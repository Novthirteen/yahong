using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using System.Linq;
using com.Sconit.Service.Ext.Hql;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Util;
using NHibernate;
using NHibernate.SqlCommand;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class SchedulingMgr : SchedulingBaseMgr, ISchedulingMgr
    {
        public IHqlMgrE hqlMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IWorkCalendarMgrE workCalendarMgrE { get; set; }
        public ITaskSubTypeMgrE taskSubTypeMgrE { get; set; }
        public ITaskMstrMgrE taskMstrMgrE { get; set; }

        /*
        [Transaction(TransactionMode.Unspecified)]
        public List<SchedulingView> GetScheduling(DateTime date, string taskSubType)
        {
            List<SchedulingView> schedulingViews = new List<SchedulingView>();

            date = date.Date;
            string dayofweek = date.DayOfWeek.ToString();
            IList<Scheduling> generalSchedulingList = GetGeneralScheduling(taskSubType);
            IList<Scheduling> specialSchedulingList = GetSpecialScheduling(taskSubType, date);

            if ((generalSchedulingList == null || generalSchedulingList.Count == 0) && (specialSchedulingList == null || specialSchedulingList.Count == 0))
            {
                return null;
            }

            schedulingViews.AddRange(
                    from generalScheduling in generalSchedulingList
                    select new SchedulingView
                    {
                        Date = date,
                        TaskSubTypeCode = generalScheduling.TaskSubType.Code,
                        TaskSubTypeDesc = generalScheduling.TaskSubType.Desc,
                        DayOfWeek = dayofweek,
                        ShiftCode = generalScheduling.Shift,
                        StartTime = AssembleActualTime(date, "00:00:00"),
                        EndTime = AssembleActualTime(date.AddDays(1), "00:00:00"),
                        StartUser = generalScheduling.StartUser
                    }
                );

            foreach (Scheduling specialScheduling in specialSchedulingList)
            {
                AddSchedulingViews(date, schedulingViews, dayofweek, specialScheduling);
            }

            schedulingViews.Sort(SchedulingTimeCompare);

            return schedulingViews;
        }

        private DateTime AssembleActualTime(DateTime date, string time)
        {
            DateTime actualTime = date;
            try
            {
                actualTime = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " " + time);
            }
            catch (Exception)
            { }

            return actualTime;
        }

        private static void AddSchedulingViews(DateTime date, List<SchedulingView> schedulingViews, string dayofweek, Scheduling special)
        {
            IList<SchedulingView> viewList = schedulingViews.Where<SchedulingView>(s => s.TaskSubTypeCode == special.TaskSubType.Code && s.ShiftCode == special.Shift).ToList<SchedulingView>();
            if (viewList == null || viewList.Count == 0)
            {
                SchedulingView sv = new SchedulingView();
                sv.Date = date;
                sv.DayOfWeek = dayofweek;
                sv.StartTime = special.StartDate.Value;
                sv.EndTime = special.EndDate.Value;
                sv.ShiftCode = special.Shift;
                sv.TaskSubTypeCode = special.TaskSubType.Code;
                sv.TaskSubTypeDesc = special.TaskSubType.Desc;
                sv.StartUser = special.StartUser;
                schedulingViews.Add(sv);
            }
            else//有重叠
            {
                foreach (SchedulingView view in viewList)
                {
                    //SpecialTime            ------
                    //ViewTime                       ------
                    if (DateTime.Compare(special.EndDate.Value, view.StartTime) <= 0)
                    {
                        continue;
                    }
                    //SpecialTime                            ------
                    //ViewTime                   ------
                    else if (DateTime.Compare(special.StartDate.Value, view.EndTime) >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (DateTime.Compare(special.StartDate.Value, view.StartTime) <= 0)
                        {
                            //SpecialTime              ----------
                            //ViewTime                   ------
                            if (DateTime.Compare(special.EndDate.Value, view.EndTime) >= 0)
                            {
                                view.StartUser = special.StartUser;
                            }
                            //SpecialTime            ------
                            //ViewTime                   ------
                            else
                            {
                                SchedulingView sv = new SchedulingView();
                                sv.StartUser = special.StartUser;
                                sv.ShiftCode = special.Shift;
                                sv.TaskSubTypeCode = special.TaskSubType.Code;
                                sv.TaskSubTypeDesc = special.TaskSubType.Desc;
                                sv.Date = date;
                                sv.DayOfWeek = dayofweek;
                                sv.StartTime = view.StartTime;
                                sv.EndTime = special.EndDate.Value;
                                schedulingViews.Add(sv);

                                view.StartTime = sv.EndTime;
                            }
                        }
                        else
                        {
                            //SpecialTime                    ------
                            //ViewTime                   ------
                            if (DateTime.Compare(special.EndDate.Value, view.EndTime) >= 0)
                            {
                                SchedulingView sv = new SchedulingView();
                                sv.StartUser = special.StartUser;
                                sv.ShiftCode = special.Shift;
                                sv.TaskSubTypeCode = special.TaskSubType.Code;
                                sv.TaskSubTypeDesc = special.TaskSubType.Desc;
                                sv.Date = date;
                                sv.DayOfWeek = dayofweek;
                                sv.StartTime = special.StartDate.Value;
                                sv.EndTime = view.EndTime;
                                schedulingViews.Add(sv);

                                view.EndTime = sv.StartTime;
                            }
                            //SpecialTime                  ------
                            //ViewTime                   ----------
                            else
                            {
                                //中段
                                SchedulingView sv = new SchedulingView();
                                sv.StartUser = special.StartUser;
                                sv.ShiftCode = special.Shift;
                                sv.TaskSubTypeCode = special.TaskSubType.Code;
                                sv.TaskSubTypeDesc = special.TaskSubType.Desc;
                                sv.Date = date;
                                sv.DayOfWeek = dayofweek;
                                sv.StartTime = special.StartDate.Value;
                                sv.EndTime = special.EndDate.Value;
                                schedulingViews.Add(sv);
                                //后段
                                sv = new SchedulingView();
                                sv.StartUser = special.StartUser;
                                sv.ShiftCode = special.Shift;
                                sv.TaskSubTypeCode = special.TaskSubType.Code;
                                sv.TaskSubTypeDesc = special.TaskSubType.Desc;
                                sv.Date = date;
                                sv.DayOfWeek = dayofweek;
                                sv.StartTime = special.EndDate.Value;
                                sv.EndTime = view.EndTime;
                                schedulingViews.Add(sv);
                                //前段
                                view.EndTime = special.StartDate.Value;
                            }
                        }
                    }
                }
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<Scheduling> GetGeneralScheduling(string taskSubType)
        {
            return GetScheduling(false, taskSubType, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<Scheduling> GetSpecialScheduling(string taskSubType, DateTime date)
        {
            return GetScheduling(true, taskSubType, date);
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<Scheduling> GetScheduling(bool isSpecial, string taskSubType, DateTime? date)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Scheduling));
            criteria.CreateAlias("TaskSubType", "tst", JoinType.InnerJoin);
            if (!string.IsNullOrEmpty(taskSubType))
            {
                criteria.Add(Expression.Eq("tst.Code", taskSubType));
            }
            if (date.HasValue)
            {
                criteria.Add(Expression.Sql(" CONVERT(varchar(10), StartDate, 23) <= ? ", date.Value.ToString("yyyy-MM-dd"), NHibernateUtil.String));
                criteria.Add(Expression.Sql(" CONVERT(varchar(10), EndDate, 23) >= ? ", date.Value.ToString("yyyy-MM-dd"), NHibernateUtil.String));
            }
            criteria.Add(Expression.Eq("IsSpecial", isSpecial));
            criteria.AddOrder(Order.Asc("tst.Seq"));
            criteria.AddOrder(Order.Asc("tst.Code"));
            criteria.AddOrder(Order.Desc("StartDate"));
            criteria.AddOrder(Order.Desc("CreateDate"));
            return this.criteriaMgrE.FindAll<Scheduling>(criteria, 0, 500);
        }

        [Transaction(TransactionMode.Unspecified)]
        public List<SchedulingView> GetScheduling(DateTime startdate, DateTime enddate, string taskSubType)
        {
            List<SchedulingView> schedulings = new List<SchedulingView>();
            startdate = startdate.Date;
            enddate = enddate.Date;
            while (DateTime.Compare(startdate, enddate) <= 0)
            {
                List<SchedulingView> newSchedulings = this.GetScheduling(startdate, taskSubType);
                if (newSchedulings != null && newSchedulings.Count > 0)
                    schedulings.AddRange(newSchedulings);
                startdate = startdate.AddDays(1);
            }

            //this.WorkCalendarContinuousTime(newSchedulings);
            return schedulings;
        }
        */

        public void SetStartUser(IList<SchedulingView> schedulingViews)
        {
            if (schedulingViews == null || schedulingViews.Count() == 0) return;

            string startUsers = string.Join(";", schedulingViews.Select(s => s.StartUser).ToArray<string>());

            IDictionary<string, string> startUserDic = taskMstrMgrE.GetUser(startUsers);

            foreach (SchedulingView view in schedulingViews)
            {
                string startUser = view.StartUser;
                view.StartUser = string.Empty;
                if (startUserDic == null || startUserDic.Count == 0) continue;
                if (!string.IsNullOrEmpty(startUser))
                {
                    string[] userCodes = startUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                    if (userCodes != null && userCodes.Length > 0)
                    {
                        foreach (string userCode in userCodes)
                        {
                            if (startUserDic.Keys.Contains(userCode))
                            {
                                if (!string.IsNullOrEmpty(view.StartUser))
                                {
                                    view.StartUser += ", ";
                                }
                                view.StartUser += startUserDic[userCode].Trim();
                            }
                        }
                    }
                }
            }
        }

        public List<SchedulingView> SpecialTimeWizard(List<SchedulingView> schedulingViews)
        {

            List<SchedulingView> resultList = new List<SchedulingView>();
            resultList.AddRange(schedulingViews);

            DateTime startDate = schedulingViews[0].StartTime;
            DateTime endDate = schedulingViews[schedulingViews.Count - 1].EndTime;
            IList<Scheduling> specialAllList = this.GetSpecialScheduling2(startDate, endDate);
            if (specialAllList != null && specialAllList.Count > 0)
            {
                foreach (Scheduling special in specialAllList)
                {
                    //处理特殊安排
                    //Scheduling special = specialAllList.Where(s => s.StartDate.Value.ToString("yyyy-MM-dd").CompareTo(view.Date.ToString("yyyy-MM-dd")) <= 0 && view.Date.ToString("yyyy-MM-dd").CompareTo(s.EndDate.Value.ToString("yyyy-MM-dd")) <= 0).FirstOrDefault();   //this.GetSpecialScheduling2(view.TaskSubTypeCode, view.Date);
                    //if (special != null)
                    //{
                    AddSchedulingViews2(resultList, special);
                    //}
                }
            }

            return resultList;
        }

        //用于任务查找排班表
        [Transaction(TransactionMode.Unspecified)]
        public IList<SchedulingView> GetScheduling2(DateTime date, string taskSubTypeCode)
        {
            return GetScheduling2(date, date, taskSubTypeCode, string.Empty);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<SchedulingView> GetScheduling2(DateTime startDate, DateTime endDate, string taskSubTypeCode, string userCode)
        {
            IList<WorkCalendar> workCalendars = workCalendarMgrE.GetWorkCalendar(startDate, endDate, string.Empty, string.Empty);
            List<SchedulingView> schedulingViews = new List<SchedulingView>();
            if (workCalendars == null || workCalendars.Count == 0) return schedulingViews;

            workCalendars = workCalendars.Distinct().ToList();
            //获得所有常规排班表集合
            IList<Scheduling> generalSchedulings = this.GetGeneralScheduling2(taskSubTypeCode, userCode);

            IList<TaskSubType> taskSubTypes = null;
            if (!string.IsNullOrEmpty(taskSubTypeCode))
            {
                TaskSubType taskSubType = taskSubTypeMgrE.LoadTaskSubType(taskSubTypeCode);

                if (taskSubType == null) return null;
                taskSubTypes = new List<TaskSubType>();
                taskSubTypes.Add(taskSubType);
            }
            else
            {
                taskSubTypes = taskSubTypeMgrE.GetTaskSubType(userCode);
                if (taskSubTypes == null || taskSubTypes.Count == 0) return null;
            }

            foreach (WorkCalendar workCalendar in workCalendars)
            {
                foreach (TaskSubType taskSubType in taskSubTypes)
                {
                    IList<SchedulingView> schedulingViewList = GetScheduling2(workCalendar, taskSubType, generalSchedulings);
                    if (schedulingViewList != null && schedulingViewList.Count > 0)
                    {
                        schedulingViews.AddRange(schedulingViewList);
                    }
                }
            }

            if (schedulingViews == null || schedulingViews.Count == 0) return schedulingViews;

            //schedulingViews.Sort(SchedulingTimeCompare);
            schedulingViews = schedulingViews.Distinct().ToList();

            schedulingViews = SpecialTimeWizard(schedulingViews);

            schedulingViews = schedulingViews.Distinct().ToList();
            schedulingViews.Sort(SchedulingTimeCompare);

            schedulingViews = SchedulingViewDataClean(schedulingViews);

            return schedulingViews;
        }
        /*
        private List<WorkCalendar> WorkCalendarDataClean(List<WorkCalendar> workCalendars)
        {
            List<WorkCalendar> removeList = new List<WorkCalendar>();
            if (workCalendars.Count > 1)
            {
                foreach (WorkCalendar workCalendar2 in workCalendars)
                {

                    int index = workCalendars.IndexOf(workCalendar);
                    if (index == 0)
                    {
                        continue;
                    }
                    WorkCalendar workCalendar1 = workCalendars[index - 1];

                    if (workCalendar1.Date == workCalendar2.Date
                          && workCalendar1.DayOfWeek == workCalendar2.DayOfWeek)
                    {

                        //workCalendar1            ------
                        //workCalendar2                       ------
                        if (DateTime.Compare(workCalendar1.EndTime, workCalendar2.StartTime) <= 0)
                        {
                            continue;
                        }
                        //workCalendar1                            ------
                        //workCalendar2                   ------
                        else if (DateTime.Compare(workCalendar1.StartTime, workCalendar2.EndTime) >= 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (DateTime.Compare(workCalendar1.StartTime, workCalendar2.StartTime) <= 0)
                            {
                                //workCalendar1              ----------
                                //workCalendar2                ------
                                if (DateTime.Compare(workCalendar1.EndDate.Value, workCalendar2.EndTime) >= 0)
                                {
                                    view.StartUser = special.StartUser;
                                    view.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                    view.Id = special.Id;
                                }
                                //workCalendar1            ------
                                //workCalendar2                ------
                                else
                                {
                                    SchedulingView sv = new SchedulingView();
                                    sv.Date = view.Date;
                                    sv.DayOfWeek = view.DayOfWeek;
                                    sv.ShiftCode = view.ShiftCode;
                                    sv.ShiftName = view.ShiftName;
                                    sv.WorkdayType = view.WorkdayType;
                                    sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                    sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                    sv.StartTime = view.StartTime;
                                    sv.EndTime = special.EndDate.Value;
                                    sv.StartUser = special.StartUser;
                                    sv.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                    sv.Id = special.Id;
                                    schedulingViews.Add(sv);

                                    view.StartTime = sv.EndTime;
                                }
                            }
                            else
                            {
                                //SpecialTime                    ------
                                //ViewTime                   ------
                                if (DateTime.Compare(special.EndDate.Value, view.EndTime) >= 0)
                                {
                                    SchedulingView sv = new SchedulingView();
                                    sv.Date = view.Date;
                                    sv.DayOfWeek = view.DayOfWeek;
                                    sv.ShiftCode = view.ShiftCode;
                                    sv.ShiftName = view.ShiftName;
                                    sv.WorkdayType = view.WorkdayType;
                                    sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                    sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                    sv.StartTime = special.StartDate.Value;
                                    sv.EndTime = view.EndTime;
                                    sv.StartUser = special.StartUser;
                                    sv.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                    sv.Id = special.Id;
                                    schedulingViews.Add(sv);

                                    view.EndTime = sv.StartTime;
                                }
                                //SpecialTime                  ------
                                //ViewTime                   ----------
                                else
                                {
                                    //中段
                                    SchedulingView sv = new SchedulingView();

                                    sv.Date = view.Date;
                                    sv.DayOfWeek = view.DayOfWeek;
                                    sv.ShiftCode = view.ShiftCode;
                                    sv.ShiftName = view.ShiftName;
                                    sv.WorkdayType = view.WorkdayType;
                                    sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                    sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                    sv.StartTime = special.StartDate.Value;
                                    sv.EndTime = special.EndDate.Value;
                                    sv.StartUser = special.StartUser;
                                    sv.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                    sv.Id = special.Id;
                                    schedulingViews.Add(sv);
                                    //后段
                                    sv = new SchedulingView();
                                    sv.Date = view.Date;
                                    sv.DayOfWeek = view.DayOfWeek;
                                    sv.ShiftCode = view.ShiftCode;
                                    sv.ShiftName = view.ShiftName;
                                    sv.WorkdayType = view.WorkdayType;
                                    sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                    sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                    sv.StartTime = special.EndDate.Value;
                                    sv.EndTime = view.EndTime;
                                    sv.StartUser = view.StartUser;
                                    sv.Id = view.Id;
                                    schedulingViews.Add(sv);
                                    //前段
                                    view.EndTime = special.StartDate.Value;
                                }
                            }
                        }

                    }
                }
            }

            List<WorkCalendar> returnList = new List<WorkCalendar>();
            if (removeList.Count > 0)
            {
                foreach (WorkCalendar returnWorkCalendar in workCalendars)
                {
                    if (removeList.Contains(returnWorkCalendar))
                    {
                        continue;
                    }
                    else
                    {
                        returnList.Add(returnWorkCalendar);
                    }
                }
            }
            else
            {
                returnList = workCalendars;
            }

            return returnList;
        }
        */

        private List<SchedulingView> SchedulingViewDataClean(List<SchedulingView> schedulingViews)
        {
            List<SchedulingView> removeList = new List<SchedulingView>();
            if (schedulingViews.Count > 1)
            {
                foreach (SchedulingView schedulingView in schedulingViews)
                {
                    int index = schedulingViews.IndexOf(schedulingView);
                    if (index == 0)
                    {
                        continue;
                    }

                    if (schedulingViews[index - 1].WorkdayType == null || schedulingView.WorkdayType == null)
                    {
                        continue;
                    }

                    if (this.Compare(schedulingViews[index - 1], schedulingView))
                    {
                        schedulingView.StartTime = schedulingViews[index - 1].StartTime;
                        if (schedulingView.ShiftCode != schedulingViews[index - 1].ShiftCode)
                        {
                            schedulingView.ShiftCode = string.Empty;
                            schedulingView.ShiftName = string.Empty;
                        }

                        if (schedulingView.WorkdayType != schedulingViews[index - 1].WorkdayType)
                        {
                            if (schedulingView.WorkdayType != BusinessConstants.CODE_MASTER_WORKCALENDAR_TYPE_VALUE_WORK)
                            {
                                schedulingView.WorkdayType = schedulingViews[index - 1].WorkdayType;
                            }
                        }

                        if (schedulingView.SchedulingType != schedulingViews[index - 1].SchedulingType)
                        {
                            schedulingView.SchedulingType = string.Empty;
                        }
                        removeList.Add(schedulingViews[index - 1]);
                    }
                }
            }

            List<SchedulingView> returnList = new List<SchedulingView>();
            if (removeList.Count > 0)
            {
                returnList = schedulingViews.Except(removeList).ToList();
            }
            else
            {
                returnList = schedulingViews;
            }

            return returnList;
        }

        private bool Compare(SchedulingView schedulingView1, SchedulingView schedulingView2)
        {
            if (schedulingView1.Date == schedulingView2.Date
                    && schedulingView1.DayOfWeek == schedulingView2.DayOfWeek
                    && schedulingView1.TaskSubTypeCode == schedulingView2.TaskSubTypeCode
                    && DateTime.Compare(schedulingView1.EndTime, schedulingView2.StartTime) >= 0
                    && ((schedulingView1.Id.HasValue && schedulingView2.Id.HasValue)
                            || (!schedulingView1.Id.HasValue && !schedulingView2.Id.HasValue)))
            {
                if (schedulingView1.StartUser.Trim().ToLower() == schedulingView2.StartUser.Trim().ToLower())
                {
                    return true;
                }
                else
                {
                    var startUser1 = schedulingView1.StartUser.ToLower().Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().OrderBy(s => s);
                    var startUser2 = schedulingView2.StartUser.ToLower().Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct().OrderBy(s => s);
                    if (startUser1.Count() == startUser2.Count())
                    {
                        foreach (string su in startUser1)
                        {
                            if (!startUser2.Contains(su))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        //特殊安排
        private static void AddSchedulingViews2(List<SchedulingView> schedulingViews, Scheduling special)
        {
            IList<SchedulingView> viewList = schedulingViews.Where(s => s.TaskSubTypeCode == special.TaskSubType.Code && special.StartDate.Value.ToString("yyyy-MM-dd").CompareTo(s.Date.ToString("yyyy-MM-dd")) <= 0 && s.Date.ToString("yyyy-MM-dd").CompareTo(special.EndDate.Value.ToString("yyyy-MM-dd")) <= 0).ToList<SchedulingView>();
            if (viewList == null || viewList.Count == 0)
            {
                return;
                //没有找到特殊安排与常规的重叠
                /*
                SchedulingView sv = new SchedulingView();
                sv.Date = current.Date;
                sv.DayOfWeek = current.DayOfWeek;
                sv.ShiftCode = current.ShiftCode;
                sv.ShiftName = current.ShiftName;
                sv.WorkdayType = current.WorkdayType;
                sv.TaskSubTypeCode = current.TaskSubTypeCode;
                sv.TaskSubTypeDesc = current.TaskSubTypeDesc;
                sv.StartTime = special.StartDate.Value;
                sv.EndTime = special.EndDate.Value;
                sv.Id = special.Id;
                sv.StartUser = special.StartUser;
                schedulingViews.Add(sv);
              * */
            }
            else//有重叠
            {
                foreach (SchedulingView view in viewList)
                {
                    //SpecialTime            ------
                    //ViewTime                       ------
                    if (DateTime.Compare(special.EndDate.Value, view.StartTime) <= 0)
                    {
                        continue;
                    }
                    //SpecialTime                            ------
                    //ViewTime                   ------
                    else if (DateTime.Compare(special.StartDate.Value, view.EndTime) >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (DateTime.Compare(special.StartDate.Value, view.StartTime) <= 0)
                        {
                            //SpecialTime              ----------
                            //ViewTime                   ------
                            if (DateTime.Compare(special.EndDate.Value, view.EndTime) >= 0)
                            {
                                view.StartUser = special.StartUser;
                                view.IsAutoAssign = false;
                                view.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                view.Id = special.Id;
                            }
                            //SpecialTime            ------
                            //ViewTime                   ------
                            else
                            {
                                SchedulingView sv = new SchedulingView();
                                sv.Date = view.Date;
                                sv.DayOfWeek = view.DayOfWeek;
                                sv.ShiftCode = view.ShiftCode;
                                sv.ShiftName = view.ShiftName;
                                sv.WorkdayType = view.WorkdayType;
                                sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                sv.StartTime = view.StartTime;
                                sv.EndTime = special.EndDate.Value;
                                sv.StartUser = special.StartUser;
                                sv.IsAutoAssign = false;
                                sv.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                sv.Id = special.Id;
                                schedulingViews.Add(sv);

                                view.StartTime = sv.EndTime;
                            }
                        }
                        else
                        {
                            //SpecialTime                    ------
                            //ViewTime                   ------
                            if (DateTime.Compare(special.EndDate.Value, view.EndTime) >= 0)
                            {
                                SchedulingView sv = new SchedulingView();
                                sv.Date = view.Date;
                                sv.DayOfWeek = view.DayOfWeek;
                                sv.ShiftCode = view.ShiftCode;
                                sv.ShiftName = view.ShiftName;
                                sv.WorkdayType = view.WorkdayType;
                                sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                sv.StartTime = special.StartDate.Value;
                                sv.EndTime = view.EndTime;
                                sv.StartUser = special.StartUser;
                                sv.IsAutoAssign = false;
                                sv.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                sv.Id = special.Id;
                                schedulingViews.Add(sv);

                                view.EndTime = sv.StartTime;
                            }
                            //SpecialTime                  ------
                            //ViewTime                   ----------
                            else
                            {
                                //中段
                                SchedulingView sv = new SchedulingView();

                                sv.Date = view.Date;
                                sv.DayOfWeek = view.DayOfWeek;
                                sv.ShiftCode = view.ShiftCode;
                                sv.ShiftName = view.ShiftName;
                                sv.WorkdayType = view.WorkdayType;
                                sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                sv.StartTime = special.StartDate.Value;
                                sv.EndTime = special.EndDate.Value;
                                sv.StartUser = special.StartUser;
                                sv.IsAutoAssign = false;
                                sv.SchedulingType = ISIConstants.SCHEDULINGTYPE_SPECIAL;
                                sv.Id = special.Id;
                                schedulingViews.Add(sv);
                                //后段
                                sv = new SchedulingView();
                                sv.Date = view.Date;
                                sv.DayOfWeek = view.DayOfWeek;
                                sv.ShiftCode = view.ShiftCode;
                                sv.ShiftName = view.ShiftName;
                                sv.WorkdayType = view.WorkdayType;
                                sv.TaskSubTypeCode = view.TaskSubTypeCode;
                                sv.TaskSubTypeDesc = view.TaskSubTypeDesc;
                                sv.StartTime = special.EndDate.Value;
                                sv.EndTime = view.EndTime;
                                sv.StartUser = view.StartUser;
                                sv.IsAutoAssign = view.IsAutoAssign;
                                sv.Id = view.Id;
                                schedulingViews.Add(sv);
                                //前段
                                view.EndTime = special.StartDate.Value;
                            }
                        }
                    }
                }
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<SchedulingView> GetScheduling2(WorkCalendar workCalendar, TaskSubType taskSubType, IList<Scheduling> generalSchedulings)
        {
            //工作日、班次和任务分类   常规排班表
            IList<Scheduling> schedulingList = null;
            if (!string.IsNullOrEmpty(workCalendar.DayOfWeek) && !string.IsNullOrEmpty(workCalendar.ShiftCode))
            {
                schedulingList = this.GetGeneralScheduling2(workCalendar.DayOfWeek, workCalendar.ShiftCode, taskSubType.Code, generalSchedulings);
            }
            //按照班次和任务分类去找常规排班表
            if ((schedulingList == null || schedulingList.Count == 0) && !string.IsNullOrEmpty(workCalendar.ShiftCode))
            {
                schedulingList = this.GetGeneralScheduling2(string.Empty, workCalendar.ShiftCode, taskSubType.Code, generalSchedulings);
            }
            //按照工作日和任务分类去找常规排班表
            if ((schedulingList == null || schedulingList.Count == 0) && !string.IsNullOrEmpty(workCalendar.DayOfWeek))
            {
                schedulingList = this.GetGeneralScheduling2(workCalendar.DayOfWeek, string.Empty, taskSubType.Code, generalSchedulings);
            }
            //按照任务分类次去找常规排班表
            if (schedulingList == null || schedulingList.Count == 0)
            {
                schedulingList = this.GetGeneralScheduling2(string.Empty, string.Empty, taskSubType.Code, generalSchedulings);
            }
            if ((schedulingList != null && schedulingList.Count > 0))
            {
                return (from scheduling in schedulingList
                        select new SchedulingView
                        {
                            Date = workCalendar.Date,
                            DayOfWeek = workCalendar.DayOfWeek,
                            ShiftCode = workCalendar.ShiftCode,
                            ShiftName = workCalendar.ShiftName,
                            StartTime = workCalendar.StartTime,
                            EndTime = workCalendar.EndTime,
                            SchedulingType = ISIConstants.SCHEDULINGTYPE_GENERAL,
                            WorkdayType = workCalendar.Type,
                            TaskSubTypeCode = scheduling.TaskSubType.Code,
                            TaskSubTypeDesc = scheduling.TaskSubType.Desc,
                            StartUser = scheduling.StartUser,
                            IsAutoAssign = false,
                            Id = scheduling.Id
                        }
                       ).ToList();
            }
            else if (taskSubType != null)//!string.IsNullOrEmpty(taskSubType.StartUser))
            {
                //取任务分类表上的
                return new SchedulingView[]{new SchedulingView(){Date = workCalendar.Date,
                                    DayOfWeek = workCalendar.DayOfWeek,
                                    ShiftCode = workCalendar.ShiftCode,
                                    ShiftName = workCalendar.ShiftName,
                                    StartTime = workCalendar.StartTime,
                                    EndTime = workCalendar.EndTime,
                                    WorkdayType = workCalendar.Type,
                                    TaskSubTypeCode = taskSubType.Code,
                                    TaskSubTypeDesc = taskSubType.Desc,
                                    StartUser = taskSubType.StartUser,
                                    IsAutoAssign = taskSubType.IsAutoAssign,
                                    SchedulingType = ISIConstants.SCHEDULINGTYPE_TASKSUBTYPE,
                                    Id = null}};
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<Scheduling> GetGeneralScheduling2(string taskSubTypeCode, string userCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Scheduling));
            criteria.CreateAlias("TaskSubType", "tst", JoinType.InnerJoin);

            if (!string.IsNullOrEmpty(taskSubTypeCode))
            {
                criteria.Add(Expression.Eq("tst.Code", taskSubTypeCode));
            }
            else
            {
                DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(userCode,
                            ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);
                criteria.Add(
                        Expression.Or(
                            Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                                Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));
            }
            criteria.Add(Expression.Eq("IsSpecial", false));

            criteria.AddOrder(Order.Desc("StartDate"));
            criteria.AddOrder(Order.Desc("CreateDate"));
            criteria.AddOrder(Order.Asc("tst.Seq"));
            criteria.AddOrder(Order.Asc("tst.Code"));
            return this.criteriaMgrE.FindAll<Scheduling>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<Scheduling> GetGeneralScheduling2(string dayOfWeek, string shift, string taskSubType, IList<Scheduling> generalSchedulings)
        {
            if (!string.IsNullOrEmpty(dayOfWeek) && !string.IsNullOrEmpty(shift))
            {
                return generalSchedulings.Where(g => g.TaskSubType.Code == taskSubType && g.DayOfWeek == dayOfWeek && g.Shift == shift).ToList();
            }
            else if (string.IsNullOrEmpty(dayOfWeek) && !string.IsNullOrEmpty(shift))
            {
                return generalSchedulings.Where(g => g.TaskSubType.Code == taskSubType && string.IsNullOrEmpty(g.DayOfWeek) && g.Shift == shift).ToList();
            }
            else if (!string.IsNullOrEmpty(dayOfWeek) && string.IsNullOrEmpty(shift))
            {
                return generalSchedulings.Where(g => g.TaskSubType.Code == taskSubType && g.DayOfWeek == dayOfWeek && string.IsNullOrEmpty(g.Shift)).ToList();
            }
            else if (string.IsNullOrEmpty(dayOfWeek) && string.IsNullOrEmpty(shift))
            {
                return generalSchedulings.Where(g => g.TaskSubType.Code == taskSubType && string.IsNullOrEmpty(g.DayOfWeek) && string.IsNullOrEmpty(g.Shift)).ToList();
            }
            else
            {
                return null;
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool HasSpecialScheduling2(string taskSubType, DateTime startDate, DateTime endDate)
        {
            return HasSpecialScheduling2(taskSubType, startDate, endDate, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool HasSpecialScheduling2(string taskSubType, DateTime startDate, DateTime endDate, Int32? schedulingId)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Scheduling));
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Id")));
            criteria.Add(Expression.Eq("IsSpecial", true));
            criteria.CreateAlias("TaskSubType", "tst", JoinType.InnerJoin);
            if (!string.IsNullOrEmpty(taskSubType))
            {
                criteria.Add(Expression.Eq("tst.Code", taskSubType));
            }
            criteria.Add(Expression.Or(
                Expression.And(Expression.Le("StartDate", startDate), Expression.Gt("EndDate", startDate)),
                Expression.Or(Expression.And(Expression.Lt("StartDate", endDate), Expression.Ge("EndDate", endDate)),
                              Expression.And(Expression.Gt("StartDate", startDate), Expression.Lt("EndDate", endDate)))));

            if (schedulingId.HasValue)
            {
                criteria.Add(Expression.Not(Expression.Eq("Id", schedulingId)));
            }

            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [Transaction(TransactionMode.Unspecified)]
        private IList<Scheduling> GetSpecialScheduling2(DateTime startDate, DateTime endDate)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Scheduling));

            criteria.Add(Expression.Eq("IsSpecial", true));
            criteria.CreateAlias("TaskSubType", "tst", JoinType.InnerJoin);

            criteria.Add(Expression.Or(
                                Expression.Or(
                                                            Expression.Between("StartDate", startDate, endDate),
                                                            Expression.Between("EndDate", startDate, endDate))//))
                                ,
                                Expression.And(Expression.Le("StartDate", startDate), Expression.Eq("EndDate", endDate))));
            criteria.AddOrder(Order.Desc("StartDate"));
            criteria.AddOrder(Order.Desc("CreateDate"));
            criteria.AddOrder(Order.Asc("tst.Seq"));
            criteria.AddOrder(Order.Asc("tst.Code"));
            return this.criteriaMgrE.FindAll<Scheduling>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Shift> GetShift(string dayOfWeek)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@" select distinct s from WorkdayShift sf join sf.Workday w join sf.Shift s ");
            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                sql.Append(@" where w.DayOfWeek='" + dayOfWeek + "'");
            }
            return hqlMgrE.FindAll<Shift>(sql.ToString());
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool Exists(string dayOfWeek, string shift, string taskSubType, bool isSpecial)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Scheduling));
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Id")));
            criteria.CreateAlias("TaskSubType", "tst", JoinType.InnerJoin);

            criteria.Add(Expression.Eq("tst.Code", taskSubType));
            if (!string.IsNullOrEmpty(shift))
            {
                criteria.Add(Expression.Eq("Shift", shift));
            }
            else
            {
                criteria.Add(Expression.Or(Expression.IsNull("Shift"), Expression.Eq("Shift", string.Empty)));
            }
            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                criteria.Add(Expression.Eq("DayOfWeek", dayOfWeek));
            }
            else
            {
                criteria.Add(Expression.Or(Expression.IsNull("DayOfWeek"), Expression.Eq("DayOfWeek", string.Empty)));
            }
            criteria.Add(Expression.Eq("IsSpecial", isSpecial));

            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }
            return false;
        }

        private static int SchedulingTimeCompare(SchedulingView x, SchedulingView y)
        {
            int compareDate = DateTime.Compare(x.Date, y.Date);
            if (compareDate != 0)
            {
                return compareDate;
            }
            else
            {
                int compareTaskSubTypeCode = string.Compare(x.TaskSubTypeCode, y.TaskSubTypeCode);
                if (compareTaskSubTypeCode != 0)
                {
                    return compareTaskSubTypeCode;
                }
                else
                {
                    int compareStartTime = DateTime.Compare(x.StartTime, y.StartTime);
                    if (compareStartTime != 0)
                    {
                        return compareStartTime;
                    }
                    else
                    {
                        int compareShift = string.Compare(x.ShiftCode, y.ShiftCode);
                        if (compareShift != 0)
                        {
                            return compareShift;
                        }
                        else
                        {
                            int compareWorkdayType = string.Compare(x.WorkdayType, y.WorkdayType);
                            if (compareWorkdayType != 0)
                            {
                                return compareWorkdayType;
                            }
                            else
                            {
                                int compareSchedulingType = string.Compare(x.SchedulingType, y.SchedulingType);
                                if (compareSchedulingType != 0)
                                {
                                    return compareSchedulingType;
                                }
                                else
                                {
                                    if ((x.Id.HasValue && y.Id.HasValue) ||
                                          (!x.Id.HasValue && !y.Id.HasValue))
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Check()
        {
            StringBuilder hql = new StringBuilder();
            hql.Append(@" select s.Id from Scheduling s ");
            hql.Append(@" where s.Shift is not null and s.Shift != '' and ");
            hql.Append(@" not exists ( ");
            hql.Append(@"   select sf.Code from Shift sf where sf.Code = s.Shift ");
            hql.Append(@"       ) ");

            IList<int> idList = hqlMgrE.FindAll<int>(hql.ToString());

            if (idList == null || idList.Count == 0) return;

            this.DeleteScheduling(idList);
        }
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class SchedulingMgrE : com.Sconit.ISI.Service.Impl.SchedulingMgr, ISchedulingMgrE
    {
    }
}

#endregion Extend Class