using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MRP;
using com.Sconit.Entity.MRP;
using NHibernate.Expression;
using com.Sconit.Entity;
using com.Sconit.Service.Ext.Criteria;
using System.Linq;
using System.Reflection;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MRP;
using com.Sconit.Entity.Exception;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class CustomerScheduleDetailMgr : CustomerScheduleDetailBaseMgr, ICustomerScheduleDetailMgr
    {
        public ICriteriaMgrE criteriaMgr { get; set; }
        #region Customized Methods

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.MRP");

        public DateTime? GetCustomerScheduleDetailDateTime(int id, bool isStart)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CustomerScheduleDetail));
            criteria.CreateAlias("CustomerSchedule", "cs");
            criteria.Add(Expression.Eq("cs.Id", id));
            if (isStart)
            {
                criteria.AddOrder(Order.Asc("StartTime"));
            }
            else
            {
                criteria.AddOrder(Order.Desc("StartTime"));
            }
            IList<CustomerScheduleDetail> list = criteriaMgr.FindAll<CustomerScheduleDetail>(criteria, 0, 1);

            if (list != null && list.Count > 0)
            {
                return list[0].StartTime;
            }
            return null;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<CustomerScheduleDetail> GetCustomerScheduleDetails(string flowCode, DateTime currentDate)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CustomerScheduleDetail));
            criteria.CreateAlias("CustomerSchedule", "cs");
            criteria.Add(Expression.Eq("cs.Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT));
            if (flowCode != null && flowCode.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("cs.Flow", flowCode));
            }
            //criteria.Add(Expression.Le("cs.ReleaseDate", currentDate.Date.AddDays(1)));
            criteria.Add(Expression.Ge("DateFrom", currentDate));
            criteria.AddOrder(Order.Asc("StartTime"));
            return criteriaMgr.FindAll<CustomerScheduleDetail>(criteria, 0, 15000);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<CustomerScheduleDetail> GetCustomerScheduleDetails(int customerScheduleId)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CustomerScheduleDetail));
            criteria.CreateAlias("CustomerSchedule", "cs");
            criteria.Add(Expression.Le("cs.Id", customerScheduleId));
            return criteriaMgr.FindAll<CustomerScheduleDetail>(criteria);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<CustomerScheduleDetail> GetEffectiveCustomerScheduleDetail(IList<CustomerScheduleDetail> customerScheduleDetailList)
        {
            return GetEffectiveCustomerScheduleDetail(customerScheduleDetailList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<CustomerScheduleDetail> GetEffectiveCustomerScheduleDetail(IList<CustomerScheduleDetail> customerScheduleDetailList, DateTime effectiveDate)
        {
            //customerScheduleDetailList = customerScheduleDetailList.Where(c => c.Item == "215307710").ToList();//test
            IList<CustomerScheduleDetail> effectiveCustomerScheduleDetailList = new List<CustomerScheduleDetail>();

            if (customerScheduleDetailList != null && customerScheduleDetailList.Count > 0)
            {
                #region 根据Flow分组
                var groupedCustomerScheduleDetailList = from det in customerScheduleDetailList
                                                        where det.DateTo >= effectiveDate
                                                        group det by det.CustomerSchedule.Flow into result
                                                        select new
                                                        {
                                                            Flow = result.Key,
                                                            List = result.ToList()
                                                        };
                #endregion

                if (groupedCustomerScheduleDetailList != null && groupedCustomerScheduleDetailList.Count() > 0)
                {
                    foreach (var groupedCustomerScheduleDetail in groupedCustomerScheduleDetailList)
                    {
#if true // 方法1 取最大Id
                        foreach (CustomerScheduleDetail det in groupedCustomerScheduleDetail.List)
                        {
                            var q = effectiveCustomerScheduleDetailList.Where(c => //c.CustomerSchedule.Id < det.CustomerSchedule.Id &&
                                  StringHelper.Eq(c.Item, det.Item) && StringHelper.Eq(c.Type, det.Type) &&
                                  StringHelper.Eq(c.Uom, det.Uom) && c.UnitCount == det.UnitCount &&
                                  c.StartTime.Date == det.StartTime.Date);

                            if (q.Count() == 0)
                            {
                                effectiveCustomerScheduleDetailList.Add(det);
                            }
                            else if (q.Count() == 1)
                            {
                                if (q.Single().Id < det.Id)
                                {
                                    //q.Single().Qty = det.Qty;
                                    effectiveCustomerScheduleDetailList.Remove(q.Single());
                                    effectiveCustomerScheduleDetailList.Add(det);
                                }
                            }
                            else
                            {
                                log.Error("Have Same CustomerScheduleDetail");
                                throw new TechnicalException("Have Same CustomerScheduleDetail");
                            }
                        }
#endif
#if false //方法2:优先取最大的Id,然后取小于 最大Id最小日期的
                        IList<CustomerScheduleDetail> detList = groupedCustomerScheduleDetail.List;
                        #region 根据Id分组(按客户日程分组)
                        var groupedDetailList = from det in detList
                                                group det by det.CustomerSchedule.Id into result
                                                select new
                                                {
                                                    Id = result.Key,
                                                    List = result.ToList()
                                                };
                        #endregion

                        #region 再根据Id排序
                        var orderedAndGroupedDetailList = from det in groupedDetailList
                                                          orderby det.Id descending
                                                          select det;
                        #endregion

                        #region 循环获取有效日程

                        DateTime? minDateFrom = null;  //最新日程的最小开始日期，旧的日程取比开始日期小的列
                        foreach (var orderedAndGroupedDetail in orderedAndGroupedDetailList)
                        {
                            if (!minDateFrom.HasValue)
                            {
                                //最新日程，全部是有效的
                                IListHelper.AddRange<CustomerScheduleDetail>(effectiveCustomerScheduleDetailList, orderedAndGroupedDetail.List);
                            }
                            else
                            {
                        #region 旧日程，只有小于最小开始日期是有效的
                                var effDetail = (from det in orderedAndGroupedDetail.List
                                                 where det.DateFrom < minDateFrom
                                                 select det);

                                if (effDetail != null && effDetail.Count() > 0)
                                {
                                    IListHelper.AddRange<CustomerScheduleDetail>(effectiveCustomerScheduleDetailList, effDetail.ToList());
                                }
                                else
                                {
                                    continue;
                                }
                        #endregion
                            }

                            //最小开始日期赋值
                            minDateFrom = (from det in orderedAndGroupedDetail.List
                                           orderby det.DateFrom ascending
                                           select det.DateFrom).FirstOrDefault();
                        }
                        #endregion
#endif
                    }
                }
            }

            return effectiveCustomerScheduleDetailList;
        }

        [Transaction(TransactionMode.Requires)]
        public ScheduleView TransferCustomerScheduleDetails2ScheduleView(IList<CustomerScheduleDetail> customerScheduleDetails, DateTime effDate)
        {
            customerScheduleDetails = this.GetEffectiveCustomerScheduleDetail(customerScheduleDetails, effDate).ToList();

            #region 头
            List<ScheduleHead> scheduleHeads =
               (from det in customerScheduleDetails
                group det by new { det.DateFrom, det.DateTo, det.Type, det.StartTime } into result
                select new ScheduleHead
            {
                DateFrom = result.Key.DateFrom,
                DateTo = result.Key.DateTo,
                Type = result.Key.Type,
                StartDate = result.Key.StartTime
            }).ToList();
            scheduleHeads = scheduleHeads.OrderBy(c => c.DateFrom).Take(41).ToList();
            #endregion

            #region 明细
            List<ScheduleBody> scheduleBodys =
                (from det in customerScheduleDetails
                 group det by new { det.Item, det.Uom, det.UnitCount, det.Location, det.ItemDescription, det.ItemReference } into result
                 select new ScheduleBody
                 {
                     Item = result.Key.Item,
                     Uom = result.Key.Uom,
                     UnitCount = result.Key.UnitCount,
                     Location = result.Key.Location,
                     ItemDescription = result.Key.ItemDescription,
                     ItemReference = result.Key.ItemReference
                 }).ToList();
            #endregion

            #region 赋值
#if false //方法1
            if (false)
            {
                foreach (CustomerScheduleDetail customerScheduleDetail in customerScheduleDetails)
                {
                    var q_scheduleHeads = scheduleHeads.Where(c => c.DateFrom == customerScheduleDetail.DateFrom
                        && c.DateTo == customerScheduleDetail.DateTo && StringHelper.Eq(c.Type, customerScheduleDetail.Type));
                    if (q_scheduleHeads.Count() == 1)
                    {
                        int index = scheduleHeads.IndexOf(q_scheduleHeads.Single());
                        string qtyIndex = "Qty" + index.ToString();
                        var q_scheduleBodys = scheduleBodys.Where(c => StringHelper.Eq(c.Item, customerScheduleDetail.Item) && c.UnitCount == customerScheduleDetail.UnitCount
                             && StringHelper.Eq(c.Uom, customerScheduleDetail.Uom) && StringHelper.Eq(c.Location, customerScheduleDetail.Location));
                        if (q_scheduleBodys.Count() == 1)
                        {
                            ScheduleBody scheduleBody = q_scheduleBodys.Single();
                            PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                            foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                            {
                                if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qtyIndex))
                                {
                                    pi.SetValue(scheduleBody, customerScheduleDetail.Qty, null);
                                    break;
                                }
                            }
                        }

                    }
                }
            }
#endif
#if false // 方法2
            foreach (ScheduleBody scheduleBody in scheduleBodys)
            {
                int i = 0;
                foreach (ScheduleHead scheduleHead in scheduleHeads)
                {
                    //string qty = "Qty" + i.ToString();
                    var q = customerScheduleDetails
                        .Where(c => StringHelper.Eq(c.Item, scheduleBody.Item) && StringHelper.Eq(c.Uom, scheduleBody.Uom) && c.UnitCount == scheduleBody.UnitCount &&
                            c.DateFrom == scheduleHead.DateFrom && c.DateTo == scheduleHead.DateTo && c.Type == scheduleHead.Type);
                    if (q.Count() == 1)
                    {
                        SetScheduleBodyQty(scheduleBody, i, q.Single().Qty);
                        //PropertyInfo[] scheduleBodyPropertyInfo = typeof(ScheduleBody).GetProperties();
                        //foreach (PropertyInfo pi in scheduleBodyPropertyInfo)
                        //{
                        //    if (pi.Name != null && StringHelper.Eq(pi.Name.ToLower(), qty))
                        //    {
                        //        pi.SetValue(scheduleBody, q.Single().Qty, null);
                        //        break;
                        //    }
                        //}
                    }
                    i++;
                }
            }
#endif
#if true // 方法3
            foreach (CustomerScheduleDetail customerScheduleDetail in customerScheduleDetails)
            {
                int i = scheduleHeads.FindIndex(s => FindScheduleHead(s, customerScheduleDetail));
                var q = scheduleBodys.Where(s =>
                    StringHelper.Eq(customerScheduleDetail.Item, s.Item) &&
                    StringHelper.Eq(customerScheduleDetail.Uom, s.Uom) &&
                    StringHelper.Eq(customerScheduleDetail.Location, s.Location) &&
                    StringHelper.Eq(customerScheduleDetail.ItemDescription, s.ItemDescription) &&
                    StringHelper.Eq(customerScheduleDetail.ItemReference, s.ItemReference) &&
                    customerScheduleDetail.UnitCount == s.UnitCount);
                if (q.Count() == 1)
                {
                    SetScheduleBodyQty(q.Single(), i, customerScheduleDetail.Qty);
                }
                else if (q.Count() > 1)
                {
                    throw new BusinessErrorException("more than 1");
                }
                else
                {
                    continue;
                }
            }
#endif
            #endregion

            #region 过滤全部是0的
            scheduleBodys = scheduleBodys.Where(s => s.TotalQty > 0).ToList();
            #endregion

            ScheduleView scheduleView = new ScheduleView();
            scheduleView.ScheduleHeads = scheduleHeads;
            scheduleView.ScheduleBodys = scheduleBodys;
            return scheduleView;
        }

        private static bool FindScheduleHead(ScheduleHead scheduleHead, CustomerScheduleDetail customerScheduleDetail)
        {
            if (customerScheduleDetail.DateFrom == scheduleHead.DateFrom &&
                customerScheduleDetail.DateTo == scheduleHead.DateTo &&
                customerScheduleDetail.Type == scheduleHead.Type)
            {
                return true;
            }
            return false;
        }

        private void SetScheduleBodyQty(ScheduleBody scheduleBody, int qtyIndex, decimal qty)
        {
            switch (qtyIndex)
            {
                case 0:
                    scheduleBody.Qty0 = qty;
                    break;
                case 1:
                    scheduleBody.Qty1 = qty;
                    break;
                case 2:
                    scheduleBody.Qty2 = qty;
                    break;
                case 3:
                    scheduleBody.Qty3 = qty;
                    break;
                case 4:
                    scheduleBody.Qty4 = qty;
                    break;
                case 5:
                    scheduleBody.Qty5 = qty;
                    break;
                case 6:
                    scheduleBody.Qty6 = qty;
                    break;
                case 7:
                    scheduleBody.Qty7 = qty;
                    break;
                case 8:
                    scheduleBody.Qty8 = qty;
                    break;
                case 9:
                    scheduleBody.Qty9 = qty;
                    break;
                case 10:
                    scheduleBody.Qty10 = qty;
                    break;
                case 11:
                    scheduleBody.Qty11 = qty;
                    break;
                case 12:
                    scheduleBody.Qty12 = qty;
                    break;
                case 13:
                    scheduleBody.Qty13 = qty;
                    break;
                case 14:
                    scheduleBody.Qty14 = qty;
                    break;
                case 15:
                    scheduleBody.Qty15 = qty;
                    break;
                case 16:
                    scheduleBody.Qty16 = qty;
                    break;
                case 17:
                    scheduleBody.Qty17 = qty;
                    break;
                case 18:
                    scheduleBody.Qty18 = qty;
                    break;
                case 19:
                    scheduleBody.Qty19 = qty;
                    break;
                case 20:
                    scheduleBody.Qty20 = qty;
                    break;
                case 21:
                    scheduleBody.Qty21 = qty;
                    break;
                case 22:
                    scheduleBody.Qty22 = qty;
                    break;
                case 23:
                    scheduleBody.Qty23 = qty;
                    break;
                case 24:
                    scheduleBody.Qty24 = qty;
                    break;
                case 25:
                    scheduleBody.Qty25 = qty;
                    break;
                case 26:
                    scheduleBody.Qty26 = qty;
                    break;
                case 27:
                    scheduleBody.Qty27 = qty;
                    break;
                case 28:
                    scheduleBody.Qty28 = qty;
                    break;
                case 29:
                    scheduleBody.Qty29 = qty;
                    break;
                case 30:
                    scheduleBody.Qty30 = qty;
                    break;
                case 31:
                    scheduleBody.Qty31 = qty;
                    break;
                case 32:
                    scheduleBody.Qty32 = qty;
                    break;
                case 33:
                    scheduleBody.Qty33 = qty;
                    break;
                case 34:
                    scheduleBody.Qty34 = qty;
                    break;
                case 35:
                    scheduleBody.Qty35 = qty;
                    break;
                case 36:
                    scheduleBody.Qty36 = qty;
                    break;
                case 37:
                    scheduleBody.Qty37 = qty;
                    break;
                case 38:
                    scheduleBody.Qty38 = qty;
                    break;
                case 39:
                    scheduleBody.Qty39 = qty;
                    break;
                case 40:
                    scheduleBody.Qty40 = qty;
                    break;
                default:
                    break;
            }
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Service.Ext.MRP.Impl
{
    [Transactional]
    public partial class CustomerScheduleDetailMgrE : com.Sconit.Service.MRP.Impl.CustomerScheduleDetailMgr, ICustomerScheduleDetailMgrE
    {
    }
}

#endregion Extend Class