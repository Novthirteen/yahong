using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MRP;
using com.Sconit.Entity.MRP;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Entity;
using System.Linq;
using System.Reflection;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class MrpShipPlanViewMgr : MrpShipPlanViewBaseMgr, IMrpShipPlanViewMgr
    {
        public ICriteriaMgrE criteriaMgr { get; set; }
        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public IList<MrpShipPlanView> GetMrpShipPlanViews(string flowCode, string locCode, string itemCode, DateTime effectiveDate, DateTime? winDate, DateTime? startDate)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(MrpShipPlanView));
            if (itemCode != null && itemCode.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("Item", itemCode));
            }
            if (flowCode != null && flowCode.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("Flow", flowCode));
            }
            if (locCode != null && locCode.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("Location", locCode));
            }
            criteria.Add(Expression.Eq("EffectiveDate", effectiveDate));
            if (winDate.HasValue)
            {
                criteria.Add(Expression.Eq("WindowTime", winDate.Value.Date));
            }
            if (startDate.HasValue)
            {
                criteria.Add(Expression.Eq("StartTime", startDate.Value.Date));
            }
            return criteriaMgr.FindAll<MrpShipPlanView>(criteria);
        }

        [Transaction(TransactionMode.Requires)]
        public ScheduleView TransferMrpShipPlanViews2ScheduleView(IList<MrpShipPlanView> mrpShipPlanViews,
            IList<ExpectTransitInventoryView> expectTransitInventoryViews,
            IList<ItemDiscontinue> itemDiscontinueList,
            string locOrFlow, string winOrStartTime)
        {
            if (mrpShipPlanViews == null || mrpShipPlanViews.Count == 0)
            {
                return null;
            }
            #region 头
            List<ScheduleHead> scheduleHeads = new List<ScheduleHead>();

            if (locOrFlow == "Flow")
            {
                if (winOrStartTime == "WindowTime")
                {
                    scheduleHeads = (from det in mrpShipPlanViews
                                     group det by new { det.Flow, det.FlowType, det.WindowTime } into result
                                     select new ScheduleHead
                                     {
                                         Flow = result.Key.Flow,
                                         Type = result.Key.FlowType,
                                         DateTo = result.Key.WindowTime
                                     }).ToList();
                }
                else
                {
                    scheduleHeads = (from det in mrpShipPlanViews
                                     group det by new { det.Flow, det.FlowType, det.StartTime } into result
                                     select new ScheduleHead
                                     {
                                         Flow = result.Key.Flow,
                                         Type = result.Key.FlowType,
                                         DateFrom = result.Key.StartTime
                                     }).ToList();
                }
            }
            else if (locOrFlow == "Location")
            {
                if (winOrStartTime == "WindowTime")
                {
                    scheduleHeads = (from det in mrpShipPlanViews
                                     group det by new { det.Location, det.WindowTime } into result
                                     select new ScheduleHead
                                     {
                                         Location = result.Key.Location,
                                         Type = "Location",
                                         DateTo = result.Key.WindowTime,
                                     }).ToList();
                }
                else
                {
                    scheduleHeads = (from det in mrpShipPlanViews
                                     group det by new { det.Location, det.StartTime } into result
                                     select new ScheduleHead
                                     {
                                         Location = result.Key.Location,
                                         Type = "Location",
                                         DateFrom = result.Key.StartTime,
                                     }).ToList();
                }
            }
            else
            {
                throw new TechnicalException(locOrFlow);
            }

            if (winOrStartTime == "WindowTime")
            {
                scheduleHeads = scheduleHeads.OrderBy(c => c.DateTo).Take(41).ToList();
            }
            else
            {
                scheduleHeads = scheduleHeads.OrderBy(c => c.DateFrom).Take(41).ToList();
            }
            #endregion

            #region 明细
            List<ScheduleBody> scheduleBodys =
                (from det in mrpShipPlanViews
                 group det by new { det.Item, det.ItemDescription, det.ItemReference, det.Uom, det.UnitCount } into result
                 select new ScheduleBody
                 {
                     Item = result.Key.Item,
                     ItemDescription = result.Key.ItemDescription,
                     ItemReference = result.Key.ItemReference,
                     Uom = result.Key.Uom,
                     UnitCount = result.Key.UnitCount,
                 }).ToList();
            #endregion

            #region 赋值
            foreach (ScheduleBody scheduleBody in scheduleBodys)
            {
                int i = 0;
                DateTime? lastDate = null;
                ScheduleHead lastScheduleHead = null;


                var itemDisconList = itemDiscontinueList == null ? null :
                        from discon in itemDiscontinueList
                        where discon.Item.Code == scheduleBody.Item
                        select discon;


                foreach (ScheduleHead scheduleHead in scheduleHeads)
                {

                    string qty = "Qty" + i.ToString();
                    string actQty = "ActQty" + i.ToString();
                    string disconActQty = "DisconActQty" + i.ToString();

                    if (locOrFlow == "Location")
                    {
                        if (winOrStartTime == "WindowTime")
                        {
                            var p = from plan in mrpShipPlanViews
                                    where plan.Location == scheduleHead.Location
                                    && plan.Item == scheduleBody.Item
                                    && plan.WindowTime == scheduleHead.DateTo
                                    select plan;

                            if (p != null && p.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                    {
                                        pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                        break;
                                    }
                                }
                            }

                            var q = from inv in expectTransitInventoryViews
                                    where inv.Location == scheduleHead.Location
                                    && inv.Item == scheduleBody.Item
                                    && inv.WindowTime <= scheduleHead.DateTo
                                    && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                    select inv;

                            if (q != null && q.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                    {
                                        pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                        break;
                                    }
                                }
                            }

                            if (itemDisconList != null && itemDisconList.Count() > 0)
                            {
                                var r = from discon in itemDisconList
                                        join inv in expectTransitInventoryViews
                                        on discon.DiscontinueItem.Code equals inv.Item
                                        where inv.Location == scheduleHead.Location
                                        && inv.WindowTime <= scheduleHead.DateTo
                                        && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                        && discon.StartDate <= inv.StartTime
                                        && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                        select inv;

                                if (r != null && r.Count() >= 0)
                                {
                                    PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                    foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                    {
                                        if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                        {
                                            pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (winOrStartTime == "StartTime")
                        {
                            var p = from plan in mrpShipPlanViews
                                    where plan.Location == scheduleHead.Location
                                    && plan.Item == scheduleBody.Item
                                    && plan.StartTime == scheduleHead.DateFrom
                                    select plan;

                            if (p != null && p.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                    {
                                        pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                        break;
                                    }
                                }
                            }

                            var q = from inv in expectTransitInventoryViews
                                    where inv.Location == scheduleHead.Location
                                    && inv.Item == scheduleBody.Item
                                    && inv.StartTime <= scheduleHead.DateFrom
                                    && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                    select inv;

                            if (q != null && q.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                    {
                                        pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                        break;
                                    }
                                }
                            }

                            if (itemDisconList != null && itemDisconList.Count() > 0)
                            {
                                var r = from discon in itemDisconList
                                        join inv in expectTransitInventoryViews
                                        on discon.DiscontinueItem.Code equals inv.Item
                                        where inv.Location == scheduleHead.Location
                                        && inv.StartTime <= scheduleHead.DateFrom
                                        && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                        && discon.StartDate <= inv.StartTime
                                        && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                        select inv;

                                if (r != null && r.Count() >= 0)
                                {
                                    PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                    foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                    {
                                        if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                        {
                                            pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (locOrFlow == "Flow")
                    {
                        if (winOrStartTime == "WindowTime")
                        {
                            var p = from plan in mrpShipPlanViews
                                    where plan.Flow == scheduleHead.Flow
                                    && plan.Item == scheduleBody.Item
                                    && plan.WindowTime == scheduleHead.DateTo
                                    select plan;

                            if (p != null && p.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                    {
                                        pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                        break;
                                    }
                                }
                            }

                            var q = from inv in expectTransitInventoryViews
                                    where inv.Flow == scheduleHead.Flow
                                    && inv.Item == scheduleBody.Item
                                    && inv.WindowTime <= scheduleHead.DateTo
                                    && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                    select inv;

                            if (q != null && q.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                    {
                                        pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                        break;
                                    }
                                }
                            }

                            if (itemDisconList != null && itemDisconList.Count() > 0)
                            {
                                var r = from discon in itemDisconList
                                        join inv in expectTransitInventoryViews
                                        on discon.DiscontinueItem.Code equals inv.Item
                                        where inv.Flow == scheduleHead.Flow
                                        && inv.WindowTime <= scheduleHead.DateTo
                                        && (!lastDate.HasValue || inv.WindowTime > lastDate.Value)
                                        && discon.StartDate <= inv.StartTime
                                        && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                        select inv;

                                if (r != null && r.Count() >= 0)
                                {
                                    PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                    foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                    {
                                        if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                        {
                                            pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (winOrStartTime == "StartTime")
                        {
                            var p = from plan in mrpShipPlanViews
                                    where plan.Flow == scheduleHead.Flow
                                    && plan.Item == scheduleBody.Item
                                    && plan.StartTime == scheduleHead.DateFrom
                                    select plan;

                            if (p != null && p.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                                    {
                                        pi.SetValue(scheduleBody, p.Sum(pp => pp.Qty), null);
                                        break;
                                    }
                                }
                            }

                            var q = from inv in expectTransitInventoryViews
                                    where inv.Flow == scheduleHead.Flow
                                    && inv.Item == scheduleBody.Item
                                    && inv.StartTime <= scheduleHead.DateFrom
                                    && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                    select inv;

                            if (q != null && q.Count() >= 0)
                            {
                                PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                {
                                    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), actQty))
                                    {
                                        pi.SetValue(scheduleBody, q.Sum(qq => qq.TransitQty), null);
                                        break;
                                    }
                                }
                            }

                            if (itemDisconList != null && itemDisconList.Count() > 0)
                            {
                                var r = from discon in itemDisconList
                                        join inv in expectTransitInventoryViews
                                        on discon.DiscontinueItem.Code equals inv.Item
                                        where inv.Flow == scheduleHead.Flow
                                        && inv.StartTime <= scheduleHead.DateFrom
                                        && (!lastDate.HasValue || inv.StartTime > lastDate.Value)
                                        && discon.StartDate <= inv.StartTime
                                        && (!discon.EndDate.HasValue || discon.EndDate.Value >= inv.WindowTime)
                                        select inv;

                                if (r != null && r.Count() >= 0)
                                {
                                    PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                                    foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                                    {
                                        if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), disconActQty))
                                        {
                                            pi.SetValue(scheduleBody, r.Sum(rr => rr.TransitQty), null);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    i++;
                    if (winOrStartTime == "WindowTime")
                    {
                        lastDate = scheduleHead.DateTo;
                    }
                    else if (winOrStartTime == "StartTime")
                    {
                        lastDate = scheduleHead.DateFrom;
                    }
                    else
                    {
                        throw new TechnicalException(winOrStartTime);
                    }

                    if (lastScheduleHead != null)
                    {
                        scheduleHead.LastDateTo = lastScheduleHead.DateTo;
                        scheduleHead.LastDateFrom = lastScheduleHead.DateFrom;
                    }
                    lastScheduleHead = scheduleHead;
                }
            }
            #endregion

            #region 过滤全部是0的
            scheduleBodys = scheduleBodys.Where(s => s.TotalQty > 0).ToList();
            #endregion

            ScheduleView scheduleView = new ScheduleView();
            scheduleView.ScheduleHeads = scheduleHeads;
            scheduleView.ScheduleBodys = scheduleBodys;
            return scheduleView;
        }

        private static bool FindScheduleHead(ScheduleHead scheduleHead, MrpShipPlanView mrpShipPlanView)
        {
            if (mrpShipPlanView.StartTime == scheduleHead.DateFrom &&
                mrpShipPlanView.WindowTime == scheduleHead.DateTo &&
                mrpShipPlanView.FlowType == scheduleHead.Type)
            {
                return true;
            }
            return false;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Service.Ext.MRP.Impl
{
    [Transactional]
    public partial class MrpShipPlanViewMgrE : com.Sconit.Service.MRP.Impl.MrpShipPlanViewMgr, IMrpShipPlanViewMgrE
    {
    }
}

#endregion Extend Class