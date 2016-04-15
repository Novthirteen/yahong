using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MRP;
using NHibernate.Expression;
using com.Sconit.Entity.MRP;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.Hql;
using NHibernate;
using NHibernate.Type;
using com.Sconit.Entity.Cost;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;
using com.Sconit.Entity.Distribution;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;
using System.Collections;
using NHibernate.SqlCommand;

namespace com.Sconit.Service.MRP.Impl
{
    public class MrpMgr : IMrpMgr
    {
        public IMrpRunLogMgrE mrpRunLogMgr { get; set; }
        public ICriteriaMgrE criteriaMgr { get; set; }
        public IHqlMgrE hqlMgr { get; set; }
        public IFinanceCalendarMgrE financeCalendarMgr { get; set; }
        public IMrpShipPlanMgrE mrpShipPlanMgr { get; set; }
        //public IUomConversionMgrE uomConversionMgr { get; set; }
        public ICustomerScheduleDetailMgrE customerScheduleDetailMgr { get; set; }
        public IItemMgrE itemMgr { get; set; }
        public IBomMgrE bomMgr { get; set; }
        public IBomDetailMgrE bomDetailMgr { get; set; }
        public IRoutingDetailMgrE routingDetailMgr { get; set; }
        public IMrpReceivePlanMgrE mrpReceivePlanMgr { get; set; }
        public IExpectTransitInventoryMgrE expectTransitInventoryMgr { get; set; }
        public IMrpLocationLotDetailMgrE mrpLocationLotDetailMgr { get; set; }
        public IMrpPlanTransactionMgrE mrpPlanTransactionMgr { get; set; }

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.MRP");

        [Transaction(TransactionMode.Requires)]
        public void RunMrp(User user)
        {
            RunMrp(DateTime.Now, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void RunMrp(DateTime effectiveDate, User user)
        {
            DateTime dateTimeNow = DateTime.Now;
            IList<MrpShipPlan> mrpShipPlanList = new List<MrpShipPlan>();
            #region EffectiveDate格式化
            effectiveDate = effectiveDate.Date;
            #endregion

            log.Info("----------------------------------Invincible's dividing line---------------------------------------");
            log.Info("Start run mrp effectivedate:" + effectiveDate.ToLongDateString());

            #region 删除有效期相同的ShipPlan、ReceivePlan、TransitInventory、MrpLocationLotDetail
            string hql = @"from MrpPlanTransaction entity where entity.EffectiveDate = ?";
            hqlMgr.Delete(hql, new object[] { effectiveDate }, new IType[] { NHibernateUtil.DateTime });

            //string hql = @"from MrpShipPlan entity where entity.EffectiveDate = ?";
            //hqlMgr.Delete(hql, new object[] { effectiveDate }, new IType[] { NHibernateUtil.DateTime });

            //hql = @"from MrpReceivePlan entity where entity.EffectiveDate = ?";
            //hqlMgr.Delete(hql, new object[] { effectiveDate }, new IType[] { NHibernateUtil.DateTime });

            hql = @"from ExpectTransitInventory entity where entity.EffectiveDate = ?";
            hqlMgr.Delete(hql, new object[] { effectiveDate }, new IType[] { NHibernateUtil.DateTime });

            hql = @"from MrpLocationLotDetail entity where entity.EffectiveDate = ?";
            hqlMgr.Delete(hql, new object[] { effectiveDate }, new IType[] { NHibernateUtil.DateTime });

            this.hqlMgr.FlushSession();
            this.hqlMgr.CleanSession();
            #endregion

            #region 获取实时库存和在途
            #region 查询
            #region 订单待收
            hql = @"select oh.OrderNo, oh.Type, oh.Flow, olt.Location.Code, olt.Item.Code, olt.Uom.Code, od.UnitCount, oh.StartTime, oh.WindowTime, od.OrderedQty, od.ShippedQty, od.ReceivedQty, olt.UnitQty
                    from OrderLocationTransaction as olt 
                    join olt.OrderDetail as od
                    join od.OrderHead as oh
                    where oh.Status in (?, ?) and oh.SubType = ? and not oh.Type = ? and olt.IOType = ?";

            IList<object[]> expectTransitInvList = hqlMgr.FindAll<object[]>(hql,
                new Object[] {
                    BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT, 
                    BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS, 
                    BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, 
                    BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION,
                    BusinessConstants.IO_TYPE_IN
                });
            #endregion

            #region 安全库存
            hql = @"select fl.Code, fdl.Code, i.Code, fd.SafeStock, fd.MaxStock from FlowDetail as fd 
                                        join fd.Flow as f 
                                        left join fd.LocationTo as fdl 
                                        left join f.LocationTo as fl
                                        join fd.Item as i
                                        where fd.LocationTo is not null 
                                        or f.LocationTo is not null";
            IList<object[]> safeQtyList = hqlMgr.FindAll<object[]>(hql);
            #endregion

            #region 实时库存
            hql = @"select l.Code, i.Code, sum(lld.Qty) from LocationLotDetail as lld
                    join lld.Location as l
                    join lld.Item as i
                    where not lld.Qty = 0 and l.Type = ? and l.IsMrp = ?
                    group by l.Code, i.Code";
            IList<object[]> invList = hqlMgr.FindAll<object[]>(hql, new object[] { BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL, true });
            #endregion

            #region 发运在途
            DetachedCriteria criteria = DetachedCriteria.For<InProcessLocationDetail>();

            //criteria.CreateAlias("LocationTo", "lt");
            criteria.CreateAlias("InProcessLocation", "ip");
            criteria.CreateAlias("OrderLocationTransaction", "olt");
            criteria.CreateAlias("olt.OrderDetail", "od");
            criteria.CreateAlias("od.OrderHead", "oh");
            criteria.CreateAlias("olt.Item", "i");
            criteria.CreateAlias("olt.Uom", "uom");
            criteria.CreateAlias("od.LocationTo", "lt", JoinType.LeftOuterJoin);
            criteria.CreateAlias("oh.LocationTo", "ohlt", JoinType.LeftOuterJoin);

            criteria.Add(Expression.Eq("ip.Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE));
            criteria.Add(Expression.Eq("oh.SubType", BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML));
            criteria.Add(Expression.In("ip.OrderType", new string[] { 
                            BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_CUSTOMERGOODS, 
                            BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT, 
                            BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING, 
                            BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER }));

            criteria.SetProjection(Projections.ProjectionList()
              .Add(Projections.GroupProperty("od.LocationTo"))
              .Add(Projections.GroupProperty("i.Code"))
              .Add(Projections.Sum("Qty"))
              .Add(Projections.Sum("ReceivedQty"))
              .Add(Projections.GroupProperty("ip.ArriveTime"))
              .Add(Projections.GroupProperty("oh.LocationTo"))
              .Add(Projections.GroupProperty("oh.OrderNo")) //6
              .Add(Projections.GroupProperty("oh.Flow")) //7
              .Add(Projections.GroupProperty("uom.Code")) //8
              .Add(Projections.GroupProperty("od.UnitCount")) //9
              .Add(Projections.GroupProperty("oh.StartTime")) //10
              .Add(Projections.GroupProperty("oh.Status"))  //11
              );
            IList<object[]> ipDetList = this.criteriaMgr.FindAll<object[]>(criteria);
            #endregion

            #region 检验在途
            criteria = DetachedCriteria.For<InspectOrderDetail>();

            criteria.CreateAlias("InspectOrder", "io");
            criteria.CreateAlias("LocationTo", "lt");
            criteria.CreateAlias("LocationLotDetail", "lld");
            criteria.CreateAlias("lld.Item", "i");

            criteria.Add(Expression.Eq("io.IsSeperated", false));
            criteria.Add(Expression.Eq("io.Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE));

            criteria.SetProjection(Projections.ProjectionList()
               .Add(Projections.GroupProperty("lt.Code").As("Location"))
               .Add(Projections.GroupProperty("i.Code").As("Item"))
               .Add(Projections.Sum("lld.Qty"))
               .Add(Projections.GroupProperty("io.EstimateInspectDate"))
               );

            IList<object[]> inspLocList = this.criteriaMgr.FindAll<object[]>(criteria);
            #endregion
            #endregion

            #region 处理数据
            #region 获取所有库位的安全库存
            IList<SafeInventory> locationSafeQtyList = new List<SafeInventory>();
            if (safeQtyList != null && safeQtyList.Count > 0)
            {
                var unGroupSafeQtyList = from safeQty in safeQtyList
                                         select new
                                         {
                                             Location = (safeQty[1] != null ? (string)safeQty[1] : (string)safeQty[0]),
                                             Item = (string)safeQty[2],
                                             SafeQty = ((safeQty[4] != null ? (decimal)safeQty[4] : 0) > (safeQty[3] != null ? (decimal)safeQty[3] : 0))
                                                        ? (safeQty[4] != null ? (decimal)safeQty[4] : 0) : (safeQty[3] != null ? (decimal)safeQty[3] : 0)
                                         };

                var groupSafeQtyList = from g in unGroupSafeQtyList
                                       group g by new { g.Location, g.Item } into result
                                       select new SafeInventory
                                       {
                                           Location = result.Key.Location,
                                           Item = result.Key.Item,
                                           SafeQty = result.Max(g => g.SafeQty)
                                       };

                locationSafeQtyList = groupSafeQtyList != null ? groupSafeQtyList.ToList() : new List<SafeInventory>();
            }
            #endregion

            #region 获取实时库存
            IList<MrpLocationLotDetail> inventoryBalanceList = new List<MrpLocationLotDetail>();
            if (invList != null && invList.Count > 0)
            {
                IListHelper.AddRange<MrpLocationLotDetail>(inventoryBalanceList, (from inv in invList
                                                                                  select new MrpLocationLotDetail
                                                                                  {
                                                                                      Location = (string)inv[0],
                                                                                      Item = (string)inv[1],
                                                                                      Qty = (decimal)inv[2],
                                                                                      SafeQty = (from g in locationSafeQtyList
                                                                                                 where g.Location == (string)inv[0]
                                                                                                    && g.Item == (string)inv[1]
                                                                                                 select g.SafeQty).FirstOrDefault()
                                                                                  }).ToList());
            }
            #endregion

            #region 没有库存的安全库存全部转换为InventoryBalance
            if (locationSafeQtyList != null && locationSafeQtyList.Count > 0)
            {
                var eqSafeQtyList = from sq in locationSafeQtyList
                                    join inv in inventoryBalanceList on new { Location = sq.Location, Item = sq.Item } equals new { Location = inv.Location, Item = inv.Item }
                                    select sq;

                IList<SafeInventory> lackSafeQtyList = null;
                if (eqSafeQtyList != null && eqSafeQtyList.Count() > 0)
                {
                    lackSafeQtyList = locationSafeQtyList.Except(eqSafeQtyList.ToList(), new SafeInventoryComparer()).ToList();
                }
                else
                {
                    lackSafeQtyList = locationSafeQtyList;
                }

                if (lackSafeQtyList != null && lackSafeQtyList.Count > 0)
                {
                    var mlldList = from sq in lackSafeQtyList
                                   where sq.SafeQty > 0
                                   select new MrpLocationLotDetail
                                   {
                                       Location = sq.Location,
                                       Item = sq.Item,
                                       Qty = 0,
                                       SafeQty = sq.SafeQty
                                   };

                    if (mlldList != null && mlldList.Count() > 0)
                    {
                        if (inventoryBalanceList == null)
                        {
                            inventoryBalanceList = mlldList.ToList();
                        }
                        else
                        {
                            IListHelper.AddRange<MrpLocationLotDetail>(inventoryBalanceList, mlldList.ToList());
                        }
                    }
                }
            }
            #endregion

            #region 记录库存
            foreach (MrpLocationLotDetail mrpLocationLotDetail in inventoryBalanceList)
            {
                mrpLocationLotDetail.EffectiveDate = effectiveDate;
                mrpLocationLotDetail.StaticQty = mrpLocationLotDetail.Qty;
                mrpLocationLotDetailMgr.CreateMrpLocationLotDetail(mrpLocationLotDetail);
            }
            #endregion

            #region 发运在途 ASN
            IList<TransitInventory> transitInventoryList = new List<TransitInventory>();

            if (ipDetList != null && ipDetList.Count > 0)
            {
                foreach (object[] ipDet in ipDetList)
                {
                    //记录在途库存
                    TransitInventory transitInventory = new TransitInventory();
                    transitInventory.Location = ipDet[0] != null ? ((Location)ipDet[0]).Code : (ipDet[5] != null ? ((Location)ipDet[5]).Code : null);
                    transitInventory.Item = (string)ipDet[1];
                    transitInventory.Qty = (decimal)ipDet[2] - (decimal)ipDet[3];
                    transitInventory.EffectiveDate = (DateTime)ipDet[4];

                    transitInventoryList.Add(transitInventory);

                    //记录完工的订单待收
                    //if ((string)ipDet[11] == BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE)
                    //{
                    //    ExpectTransitInventory expectTransitInventory = new ExpectTransitInventory();

                    //    expectTransitInventory.OrderNo = (string)ipDet[6];
                    //    expectTransitInventory.Flow = (string)ipDet[7];
                    //    expectTransitInventory.Location = transitInventory.Location;
                    //    expectTransitInventory.Item = transitInventory.Item;
                    //    expectTransitInventory.Uom = (string)ipDet[8];
                    //    expectTransitInventory.UnitCount = (decimal)ipDet[9];
                    //    expectTransitInventory.StartTime = (DateTime)ipDet[10];
                    //    expectTransitInventory.WindowTime = (DateTime)ipDet[4];
                    //    expectTransitInventory.TransitQty = transitInventory.Qty;
                    //    expectTransitInventory.EffectiveDate = effectiveDate;

                    //    this.expectTransitInventoryMgr.CreateExpectTransitInventory(expectTransitInventory);
                    //}
                }
            }
            #endregion

            #region 检验在途
            if (inspLocList != null && inspLocList.Count > 0)
            {
                foreach (object[] inspLoc in inspLocList)
                {
                    //记录在途库存
                    TransitInventory transitInventory = new TransitInventory();
                    transitInventory.Location = (string)inspLoc[0];
                    transitInventory.Item = (string)inspLoc[1];
                    transitInventory.Qty = (decimal)inspLoc[2];
                    transitInventory.EffectiveDate = (DateTime)inspLoc[3];

                    transitInventoryList.Add(transitInventory);
                    log.Debug("In-Process inspect order detail records as transit inventory. location[" + transitInventory.Location + "], item[" + transitInventory.Item + "], qty[" + transitInventory.Qty.ToString("0.####") + "], effectiveDate[" + transitInventory.EffectiveDate + "]");
                }
            }
            #endregion

            #region Snapshot 订单待收
            if (expectTransitInvList != null)
            {
                var expTransInvListSnapShot = from inv in expectTransitInvList
                                              select new ExpectTransitInventory
                                              {
                                                  OrderNo = (string)inv[0],
                                                  Flow = (string)inv[2],
                                                  Location = (string)inv[3],
                                                  Item = (string)inv[4],
                                                  Uom = (string)inv[5],
                                                  UnitCount = (decimal)inv[6],
                                                  StartTime = (DateTime)inv[7],
                                                  WindowTime = (DateTime)inv[8],
                                                  TransitQty = (string)inv[1] != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION ? ((decimal)inv[9] - (inv[10] != null ? (decimal)inv[10] : 0) * (decimal)inv[12]) : ((decimal)inv[9] - (inv[11] != null ? (decimal)inv[11] : 0) * (decimal)inv[12]),
                                                  EffectiveDate = effectiveDate
                                              };

                foreach (ExpectTransitInventory snapShot in expTransInvListSnapShot)
                {
                    this.expectTransitInventoryMgr.CreateExpectTransitInventory(snapShot);
                }
            }
            #endregion
            #endregion
            #endregion


            #region 获取所有替代物料
            criteria = DetachedCriteria.For<ItemDiscontinue>();

            criteria.Add(Expression.Le("StartDate", effectiveDate));
            criteria.Add(Expression.Or(Expression.IsNull("EndDate"), Expression.Ge("EndDate", effectiveDate)));

            IList<ItemDiscontinue> itemDiscontinueList = this.criteriaMgr.FindAll<ItemDiscontinue>(criteria);
            #endregion


            #region 根据客户需求/销售订单生成发货计划

            #region 获取所有销售路线明细
            criteria = DetachedCriteria.For<Flow>();

            criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.GroupProperty("Code"))
                .Add(Projections.GroupProperty("MRPOption")));

            criteria.Add(Expression.Eq("IsActive", true));
            criteria.Add(Expression.Eq("Type", BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION));

            IList<object[]> flowList = this.criteriaMgr.FindAll<object[]>(criteria);
            #endregion

            #region 获取客户需求
            criteria = DetachedCriteria.For<CustomerScheduleDetail>();
            criteria.CreateAlias("CustomerSchedule", "cs");

            criteria.Add(Expression.Eq("cs.Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT));
            //criteria.Add(Expression.Ge("StartTime", effectiveDate));

            IList<CustomerScheduleDetail> customerScheduleDetailList = this.criteriaMgr.FindAll<CustomerScheduleDetail>(criteria);

            #region 取得有效的CustomerScheduleDetail
            IList<CustomerScheduleDetail> effectiveCustomerScheduleDetailList = customerScheduleDetailMgr.GetEffectiveCustomerScheduleDetail(customerScheduleDetailList, effectiveDate);
            #endregion
            #endregion

            #region 获取所有销售定单明细
            //活动的
            criteria = DetachedCriteria.For<OrderDetail>();
            criteria.CreateAlias("OrderHead", "od");
            criteria.Add(Expression.Eq("od.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION));
            criteria.Add(Expression.Eq("od.SubType", BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML));
            criteria.Add(Expression.In("od.Status", new string[] { BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT,
                BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS }));
            criteria.Add(Expression.Or(Expression.GtProperty("OrderedQty", "ShippedQty"), Expression.IsNull("ShippedQty")));


            criteria.AddOrder(Order.Asc("od.StartTime"));
            IList<OrderDetail> salesOrderDetailList = this.criteriaMgr.FindAll<OrderDetail>(criteria);
            #endregion

            #region 循环销售路线生成发货计划
            if (flowList != null && flowList.Count > 0)
            {
                foreach (object[] flow in flowList)
                {
                    string flowCode = (string)flow[0];
                    string mrpOption = (string)flow[1];
                    if (flowCode == "YSSY")
                    {
                        //
                    }

                    var targetSalesOrderDetailList = (from det in salesOrderDetailList
                                                      where det.OrderHead.Flow == flowCode
                                                      select det).ToList();

                    var targetCustomerScheduleDetailList = (from det in effectiveCustomerScheduleDetailList
                                                            where det.CustomerSchedule.Flow == flowCode
                                                            select det).ToList();
                    //订单优先
                    if (mrpOption == BusinessConstants.CODE_MASTER_MRP_OPTION_VALUE_ORDER_BEFORE_PLAN)
                    {
                        IListHelper.AddRange(mrpShipPlanList, TransferSalesOrderAndCustomerPlan2ShipPlan(
                            targetSalesOrderDetailList, targetCustomerScheduleDetailList,
                            effectiveDate, dateTimeNow, user));
                    }
                    //仅计划
                    else if (mrpOption == BusinessConstants.CODE_MASTER_MRP_OPTION_VALUE_PLAN_ONLY)
                    {
                        IListHelper.AddRange(mrpShipPlanList, TransferCustomerPlan2ShipPlan(
                            targetCustomerScheduleDetailList, effectiveDate, dateTimeNow, user));
                    }
                    //仅订单
                    else if (mrpOption == BusinessConstants.CODE_MASTER_MRP_OPTION_VALUE_ORDER_ONLY)
                    {
                        IListHelper.AddRange(mrpShipPlanList,
                            TransferSalesOrder2ShipPlan(targetSalesOrderDetailList, effectiveDate, dateTimeNow, user));
                    }
                    //计划加订单
                    else if (mrpOption == BusinessConstants.CODE_MASTER_MRP_OPTION_VALUE_PLAN_ADD_ORDER)
                    {
                        IListHelper.AddRange(mrpShipPlanList, TransferCustomerPlan2ShipPlan(
                            targetCustomerScheduleDetailList, effectiveDate, dateTimeNow, user));

                        IListHelper.AddRange(mrpShipPlanList, TransferSalesOrder2ShipPlan(
                            targetSalesOrderDetailList, effectiveDate, dateTimeNow, user));
                    }
                    //计划减订单
                    else if (mrpOption == BusinessConstants.CODE_MASTER_MRP_OPTION_VALUE_PLAN_MINUS_ORDER)
                    {
                        IList<MrpShipPlan> customerPlanShipPlan = TransferCustomerPlan2ShipPlan(
                            targetCustomerScheduleDetailList, effectiveDate, dateTimeNow, user);

                        IListHelper.AddRange(mrpShipPlanList, PlanMinusOrder(customerPlanShipPlan));
                    }
                    else if (string.IsNullOrEmpty(mrpOption) || mrpOption == BusinessConstants.CODE_MASTER_MRP_OPTION_VALUE_SAFESTOCK_ONLY)
                    {

                    }
                    else
                    {
                        throw new TechnicalException("MRP option " + mrpOption + " is not valid.");
                    }
                }
            }
            #endregion
            #endregion

            #region 查询并缓存所有FlowDetail
            criteria = DetachedCriteria.For<FlowDetail>();
            criteria.CreateAlias("Flow", "f");
            criteria.CreateAlias("Item", "i");
            criteria.CreateAlias("i.Uom", "iu");
            criteria.CreateAlias("Uom", "u");
            criteria.CreateAlias("i.Location", "il", JoinType.LeftOuterJoin);
            criteria.CreateAlias("i.Bom", "ib", JoinType.LeftOuterJoin);
            criteria.CreateAlias("i.Routing", "ir", JoinType.LeftOuterJoin);
            criteria.CreateAlias("LocationFrom", "lf", JoinType.LeftOuterJoin);
            criteria.CreateAlias("LocationTo", "lt", JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.LocationFrom", "flf", JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.LocationTo", "flt", JoinType.LeftOuterJoin);
            criteria.CreateAlias("Bom", "b", JoinType.LeftOuterJoin);
            criteria.CreateAlias("Routing", "r", JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.Routing", "fr", JoinType.LeftOuterJoin);

            criteria.SetProjection(Projections.ProjectionList()
                .Add(Projections.GroupProperty("f.Code").As("Flow"))
                .Add(Projections.GroupProperty("f.Type").As("FlowType"))
                .Add(Projections.GroupProperty("i.Code").As("Item"))
                .Add(Projections.GroupProperty("lf.Code").As("LocationFrom"))
                .Add(Projections.GroupProperty("lt.Code").As("LocationTo"))
                .Add(Projections.GroupProperty("flf.Code").As("FlowLocationFrom"))
                .Add(Projections.GroupProperty("flt.Code").As("FlowLocationTo"))
                .Add(Projections.GroupProperty("MRPWeight").As("MRPWeight"))
                .Add(Projections.GroupProperty("b.Code").As("Bom"))
                .Add(Projections.GroupProperty("r.Code").As("Routing"))
                .Add(Projections.GroupProperty("fr.Code").As("FlowRouting"))
                .Add(Projections.GroupProperty("iu.Code").As("ItemUom"))
                .Add(Projections.GroupProperty("u.Code").As("Uom"))
                .Add(Projections.GroupProperty("f.LeadTime").As("LeadTime"))
                .Add(Projections.GroupProperty("ib.Code").As("ItemBom"))
                .Add(Projections.GroupProperty("ir.Code").As("ItemRouting"))
                .Add(Projections.GroupProperty("il.Code").As("ItemLocation"))
                .Add(Projections.GroupProperty("UnitCount").As("UnitCount"))
                .Add(Projections.GroupProperty("i.Desc1").As("ItemDesc1"))
                .Add(Projections.GroupProperty("i.Desc2").As("ItemDesc2"))
                .Add(Projections.GroupProperty("Id").As("Id"))
                );

            criteria.Add(Expression.Eq("f.IsActive", true));
            //criteria.Add(Expression.Not(Expression.Eq("f.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_CUSTOMERGOODS)));
            criteria.Add(Expression.Gt("MRPWeight", 0));
            criteria.Add(Expression.Eq("f.IsMRP", true));

            IList<object[]> flowDetailList = this.criteriaMgr.FindAll<object[]>(criteria);

            var targetFlowDetailList = from fd in flowDetailList
                                       select new FlowDetailSnapShot
                                       {
                                           Flow = (string)fd[0],
                                           FlowType = (string)fd[1],
                                           Item = (string)fd[2],
                                           LocationFrom = fd[3] != null ? (string)fd[3] : fd[5] != null ? (string)fd[5] : (string)fd[16],
                                           LocationTo = fd[4] != null ? (string)fd[4] : (string)fd[6],
                                           MRPWeight = (int)fd[7],
                                           Bom = (string)fd[1] != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION ? null : fd[8] != null ? (string)fd[8] : fd[14] != null ? (string)fd[14] : (string)fd[2],  //FlowDetail --> Item.Bom --> Item.Code
                                           Routing = (string)fd[1] != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION ? null : fd[9] != null ? (string)fd[9] : fd[10] != null ? (string)fd[10] : fd[15] != null ? (string)fd[15] : null, //FlowDetail --> Flow --> Item.Routing
                                           BaseUom = (string)fd[11],
                                           Uom = (string)fd[12],
                                           LeadTime = fd[13] != null ? (decimal)fd[13] : 0,
                                           UnitCount = (decimal)fd[17],
                                           ItemDescription = ((fd[18] != null ? fd[18] : string.Empty) + ((fd[19] != null) ? "[" + fd[19] + "]" : string.Empty)),
                                           Id = (int)fd[20]
                                       };

            IList<FlowDetailSnapShot> flowDetailSnapShotList = new List<FlowDetailSnapShot>();
            if (targetFlowDetailList != null && targetFlowDetailList.Count() > 0)
            {
                flowDetailSnapShotList = targetFlowDetailList.ToList();
            }

            #region 处理引用
            if (flowDetailSnapShotList != null && flowDetailSnapShotList.Count > 0)
            {
                criteria = DetachedCriteria.For<Flow>();

                criteria.CreateAlias("LocationFrom", "flf", JoinType.LeftOuterJoin);
                criteria.CreateAlias("LocationTo", "flt", JoinType.LeftOuterJoin);
                criteria.CreateAlias("Routing", "fr", JoinType.LeftOuterJoin);

                criteria.SetProjection(Projections.ProjectionList()
                    .Add(Projections.GroupProperty("Code").As("Flow"))
                    .Add(Projections.GroupProperty("Type").As("FlowType"))
                    .Add(Projections.GroupProperty("ReferenceFlow").As("ReferenceFlow"))
                    .Add(Projections.GroupProperty("flf.Code").As("FlowLocationFrom"))
                    .Add(Projections.GroupProperty("flt.Code").As("FlowLocationTo"))
                    .Add(Projections.GroupProperty("fr.Code").As("FlowRouting"))
                    );

                criteria.Add(Expression.Eq("IsActive", true));
                criteria.Add(Expression.IsNotNull("ReferenceFlow"));
                criteria.Add(Expression.Eq("IsMRP", true));
                criteria.Add(Expression.Not(Expression.Eq("Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)));
                criteria.Add(Expression.Not(Expression.Eq("Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING)));

                IList<object[]> refFlowList = this.criteriaMgr.FindAll<object[]>(criteria);

                if (refFlowList != null && refFlowList.Count > 0)
                {
                    foreach (object[] refFlow in refFlowList)
                    {
                        var refFlowDetailList = from fd in flowDetailSnapShotList
                                                where string.Compare(fd.Flow, (string)refFlow[2]) == 0
                                                select fd;

                        if (refFlowDetailList != null && refFlowDetailList.Count() > 0)
                        {
                            IListHelper.AddRange(flowDetailSnapShotList, (from fd in refFlowDetailList
                                                                          select new FlowDetailSnapShot
                                                                          {
                                                                              Flow = (string)refFlow[0],
                                                                              FlowType = (string)refFlow[1],
                                                                              Item = fd.Item,
                                                                              LocationFrom = (string)refFlow[3],
                                                                              LocationTo = (string)refFlow[4],
                                                                              MRPWeight = fd.MRPWeight,
                                                                              Bom = fd.Bom,
                                                                              Routing = (string)refFlow[5],
                                                                              BaseUom = fd.BaseUom,
                                                                              Uom = fd.Uom,
                                                                              LeadTime = fd.LeadTime,
                                                                              UnitCount = fd.UnitCount,
                                                                              ItemDescription = fd.ItemDescription
                                                                          }).ToList());
                        }
                    }
                }
            }
            #endregion

            #endregion

            #region 补充安全库存
            if (inventoryBalanceList != null && inventoryBalanceList.Count > 0)
            {
                var lackInventoryList = from inv in inventoryBalanceList
                                        where inv.ActiveQty < 0  //可用库存小于0，要补充安全库存
                                        select inv;

                if (lackInventoryList != null && lackInventoryList.Count() > 0)
                {
                    foreach (MrpLocationLotDetail lackInventory in lackInventoryList)
                    {
                        #region 扣减在途，不考虑在途的到货时间
                        var transitConsumed = from trans in transitInventoryList
                                              where trans.Location == lackInventory.Location
                                                  && trans.Item == lackInventory.Item && trans.Qty > 0
                                              select trans;

                        if (transitConsumed != null && transitConsumed.Count() > 0)
                        {
                            foreach (TransitInventory inventory in transitConsumed)
                            {
                                if ((-lackInventory.ActiveQty) > inventory.Qty)
                                {
                                    lackInventory.Qty += inventory.Qty;
                                    inventory.Qty = 0;
                                }
                                else
                                {
                                    inventory.Qty += lackInventory.ActiveQty;
                                    lackInventory.Qty = lackInventory.SafeQty;

                                    break;
                                }
                            }
                        }

                        if (lackInventory.ActiveQty == 0)
                        {
                            //在途满足库存短缺
                            continue;
                        }
                        else
                        {
                            //在途不满足库存短缺
                            Item item = this.itemMgr.CheckAndLoadItem(lackInventory.Item);

                            MrpReceivePlan mrpReceivePlan = new MrpReceivePlan();
                            mrpReceivePlan.Item = lackInventory.Item;
                            mrpReceivePlan.Uom = item.Uom.Code;
                            mrpReceivePlan.Location = lackInventory.Location;
                            mrpReceivePlan.Qty = -lackInventory.ActiveQty;
                            mrpReceivePlan.UnitCount = item.UnitCount;
                            mrpReceivePlan.ReceiveTime = effectiveDate;
                            mrpReceivePlan.SourceType = BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_SAFE_STOCK;
                            mrpReceivePlan.SourceDateType = BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY;
                            mrpReceivePlan.SourceId = lackInventory.Location;
                            mrpReceivePlan.SourceUnitQty = 1;
                            mrpReceivePlan.EffectiveDate = effectiveDate;
                            mrpReceivePlan.CreateDate = dateTimeNow;
                            mrpReceivePlan.CreateUser = user.Code;
                            mrpReceivePlan.ItemDescription = item.Description;

                            //this.mrpReceivePlanMgr.CreateMrpReceivePlan(mrpReceivePlan);
                            mrpReceivePlan.StartTime = DateTime.Now;
                            mrpReceivePlan.WindowTime = DateTime.Now;
                            //this.RecordTrans(mrpReceivePlan, mrpReceivePlan.Qty);

                            log.Debug("Create receive plan for safe stock, location[" + mrpReceivePlan.Location + "], item[" + mrpReceivePlan.Item + "], qty[" + mrpReceivePlan.Qty + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "]");

                            CalculateNextShipPlan(mrpReceivePlan, inventoryBalanceList, transitInventoryList, flowDetailSnapShotList, itemDiscontinueList, effectiveDate, dateTimeNow, user);
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region 循环生成入库计划/发货计划
            if (mrpShipPlanList != null && mrpShipPlanList.Count > 0)
            {
                var sortedMrpShipPlanList = from plan in mrpShipPlanList
                                            orderby plan.StartTime ascending
                                            select plan;

                //var sortedMrpShipPlanList = mrpShipPlanList.OrderBy(m => m.StartTime).ToList();
                //sortedMrpShipPlanList.Sort(new SourceDateTypeComparer());
                foreach (MrpShipPlan mrpShipPlan in sortedMrpShipPlanList)
                {
                    NestCalculateMrpShipPlanAndReceivePlan(mrpShipPlan, inventoryBalanceList, transitInventoryList, flowDetailSnapShotList, itemDiscontinueList, effectiveDate, dateTimeNow, user);
                }
            }
            #endregion

            #region 记录MRP Run日志
            MrpRunLog currLog = new MrpRunLog();
            currLog.RunDate = effectiveDate;
            currLog.StartTime = dateTimeNow;
            currLog.EndTime = DateTime.Now;
            currLog.CreateDate = dateTimeNow;
            currLog.CreateUser = user.Code;

            this.mrpRunLogMgr.CreateMrpRunLog(currLog);
            #endregion

            log.Info("End run mrp effectivedate:" + effectiveDate.ToLongDateString());
        }

        #region Private Methods

        /// <summary>
        /// 计划减订单
        /// </summary>
        /// <param name="planShipPlan"></param>
        private IList<MrpShipPlan> PlanMinusOrder(IList<MrpShipPlan> planShipPlan)
        {
            if (planShipPlan != null && planShipPlan.Count > 0)
            {
                DateTime planStartTime = planShipPlan.Min(t => t.DateFrom);

                #region 发运在途 已销售出库
                DetachedCriteria criteria = DetachedCriteria.For<InProcessLocationDetail>();

                criteria.CreateAlias("InProcessLocation", "ip");
                criteria.CreateAlias("OrderLocationTransaction", "olt");
                criteria.CreateAlias("olt.OrderDetail", "od");
                criteria.CreateAlias("od.OrderHead", "oh");
                criteria.CreateAlias("olt.Item", "i");
                criteria.CreateAlias("olt.Uom", "uom");
                criteria.CreateAlias("i.Uom", "u");
                criteria.CreateAlias("od.LocationFrom", "lt", JoinType.LeftOuterJoin);
                criteria.CreateAlias("oh.LocationFrom", "ohlt", JoinType.LeftOuterJoin);

                criteria.Add(Expression.Eq("oh.SubType", BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML));
                criteria.Add(Expression.Eq("ip.OrderType", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION));
                criteria.Add(Expression.Ge("oh.StartTime", planStartTime));
                criteria.Add(Expression.Eq("oh.Flow", planShipPlan.First().Flow));

                criteria.SetProjection(Projections.ProjectionList()
                  .Add(Projections.GroupProperty("od.LocationFrom"))//0
                  .Add(Projections.GroupProperty("i.Code"))//1
                  .Add(Projections.Sum("Qty"))//2
                  .Add(Projections.Sum("ReceivedQty"))//3
                  .Add(Projections.GroupProperty("ip.ArriveTime"))//4
                  .Add(Projections.GroupProperty("oh.LocationFrom"))//5
                  .Add(Projections.GroupProperty("oh.OrderNo")) //6
                  .Add(Projections.GroupProperty("oh.Flow")) //7
                  .Add(Projections.GroupProperty("uom.Code")) //8
                  .Add(Projections.GroupProperty("od.UnitCount")) //9
                  .Add(Projections.GroupProperty("oh.StartTime")) //10
                  .Add(Projections.GroupProperty("oh.Status"))  //11
                  .Add(Projections.GroupProperty("u.Code"))  //12
                  );
                IList<object[]> ipDets = this.criteriaMgr.FindAll<object[]>(criteria);
                #endregion

                IList<MrpShipPlan> mrpShipPlans = new List<MrpShipPlan>();

                if (ipDets != null && ipDets.Count() > 0)
                {
                    foreach (object[] ipDet in ipDets)
                    {
                        MrpShipPlan mrpShipPlan = new MrpShipPlan();

                        mrpShipPlan.Flow = (string)ipDet[7];
                        mrpShipPlan.FlowType = BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION;
                        mrpShipPlan.Item = (string)ipDet[1];
                        //mrpShipPlan.ItemDescription = ipDet.ItemDescription;
                        //mrpShipPlan.ItemReference = ipDet.ItemReference;
                        mrpShipPlan.StartTime = (DateTime)ipDet[10];
                        mrpShipPlan.WindowTime = (DateTime)ipDet[4];
                        mrpShipPlan.LocationFrom = ipDet[0] != null ? ((Location)ipDet[0]).Code : (ipDet[5] != null ? ((Location)ipDet[5]).Code : null);
                        mrpShipPlan.SourceType = BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_ORDER;
                        mrpShipPlan.SourceDateType = BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY;
                        mrpShipPlan.SourceId = (string)ipDet[6];
                        mrpShipPlan.SourceUnitQty = 1;
                        mrpShipPlan.EffectiveDate = planShipPlan.First().EffectiveDate;
                        mrpShipPlan.Qty = -(decimal)ipDet[2];
                        mrpShipPlan.Uom = (string)ipDet[8];
                        mrpShipPlan.UnitCount = (decimal)ipDet[9];
                        mrpShipPlan.BaseUom = (string)ipDet[12];
                        if (mrpShipPlan.Uom != mrpShipPlan.BaseUom)
                        {
                            mrpShipPlan.UnitQty = this.ConvertUomQty(mrpShipPlan.Item, mrpShipPlan.Uom, 1, mrpShipPlan.BaseUom);
                        }
                        else
                        {
                            mrpShipPlan.UnitQty = 1;
                        }
                        mrpShipPlan.CreateDate = planShipPlan.First().CreateDate;
                        mrpShipPlan.CreateUser = planShipPlan.First().CreateUser;
                        mrpShipPlan.LocationTo = mrpShipPlan.LocationFrom;

                        this.RecordTrans(mrpShipPlan, mrpShipPlan.Qty);
                        //this.mrpShipPlanMgr.CreateMrpShipPlan(mrpShipPlan);
                        mrpShipPlans.Add(mrpShipPlan);

                        log.Debug("Create ship plan for customer schedule, flow[" + mrpShipPlan.Flow + "], item[" + mrpShipPlan.Item + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "], sourceType[" + mrpShipPlan.SourceType + "], sourceId[" + (mrpShipPlan.SourceId != null ? mrpShipPlan.SourceId : string.Empty) + "]");
                    }
                }

                var groupPlan = from plan in planShipPlan
                                group plan by plan.Item into result
                                select new
                                {
                                    Item = result.Key,
                                    MrpPlans = result
                                };

                foreach (var plan in groupPlan)
                {
                    if (plan.MrpPlans != null && plan.MrpPlans.Count() > 0)
                    {
                        MrpShipPlan mrpPlan = plan.MrpPlans.OrderBy(m => m.DateFrom).First();
                        foreach (MrpShipPlan mrpShipPlan in mrpShipPlans)
                        {
                            if (mrpShipPlan.Item == mrpPlan.Item)
                            {
                                mrpPlan.Qty += mrpShipPlan.Qty;
                                if (mrpPlan.Qty < 0)
                                {
                                    mrpPlan.Qty = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
                return planShipPlan;
            }
            return null;
        }

        private void ProcessEffectiveInventoryBalance(ref IList<MrpLocationLotDetail> inventoryBalanceList, object[] invLoc, IList<SafeInventory> safeQtyList, DateTime effectiveDate, DateTime dateTimeNow, User user)
        {
            MrpLocationLotDetail matchedInv = (from g in inventoryBalanceList
                                               where g.Location == ((string)invLoc[0])
                                                  && g.Item == ((string)invLoc[1])
                                               select g).FirstOrDefault();

            if (matchedInv != null)
            {
                matchedInv.Qty += (decimal)invLoc[2];
            }
            else
            {
                MrpLocationLotDetail locationLotDetail = new MrpLocationLotDetail();
                locationLotDetail.Location = (string)invLoc[0];
                locationLotDetail.Item = (string)invLoc[1];
                locationLotDetail.Qty = (decimal)invLoc[2];
                locationLotDetail.SafeQty = (from g in safeQtyList
                                             where g.Location == locationLotDetail.Location
                                                && g.Item == locationLotDetail.Item
                                             select g.SafeQty).FirstOrDefault();

                inventoryBalanceList.Add(locationLotDetail);
            }
        }

        private IList<MrpShipPlan> TransferSalesOrder2ShipPlan(IList<OrderDetail> salesOrderDetailList, DateTime effectiveDate, DateTime dateTimeNow, User user)
        {
            IList<MrpShipPlan> mrpShipPlanList = new List<MrpShipPlan>();

            if (salesOrderDetailList != null && salesOrderDetailList.Count > 0)
            {
                foreach (OrderDetail salesOrderDetail in salesOrderDetailList)
                {
                    OrderHead orderHead = salesOrderDetail.OrderHead;
                    MrpShipPlan mrpShipPlan = new MrpShipPlan();

                    if (salesOrderDetail.OrderHead.StartTime < effectiveDate)
                    {
                        mrpShipPlan.IsExpire = true;
                        mrpShipPlan.ExpireStartTime = salesOrderDetail.OrderHead.StartTime;
                    }
                    else
                    {
                        mrpShipPlan.IsExpire = false;
                    }
                    mrpShipPlan.Flow = salesOrderDetail.OrderHead.Flow;
                    mrpShipPlan.FlowType = BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION;
                    mrpShipPlan.Item = salesOrderDetail.Item.Code;
                    mrpShipPlan.ItemDescription = salesOrderDetail.Item.Description;
                    if (mrpShipPlan.IsExpire)
                    {
                        mrpShipPlan.StartTime = DateTime.Now;
                    }
                    else
                    {
                        mrpShipPlan.StartTime = salesOrderDetail.OrderHead.StartTime;
                    }
                    if (salesOrderDetail.OrderHead.WindowTime < effectiveDate)
                    {
                        mrpShipPlan.WindowTime = DateTime.Now;
                    }
                    else
                    {
                        mrpShipPlan.WindowTime = salesOrderDetail.OrderHead.WindowTime;
                    }
                    mrpShipPlan.LocationFrom = salesOrderDetail.DefaultLocationFrom.Code;
                    mrpShipPlan.SourceType = BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_ORDER;
                    mrpShipPlan.SourceDateType = BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY;
                    mrpShipPlan.SourceId = salesOrderDetail.OrderHead.OrderNo;
                    mrpShipPlan.SourceUnitQty = 1;
                    mrpShipPlan.EffectiveDate = effectiveDate;
                    mrpShipPlan.Qty = (salesOrderDetail.OrderedQty - (salesOrderDetail.ShippedQty.HasValue ? salesOrderDetail.ShippedQty.Value : 0));
                    mrpShipPlan.Uom = salesOrderDetail.Uom.Code;
                    mrpShipPlan.BaseUom = salesOrderDetail.Item.Uom.Code;
                    mrpShipPlan.UnitCount = salesOrderDetail.UnitCount;
                    if (mrpShipPlan.Uom != mrpShipPlan.BaseUom)
                    {
                        mrpShipPlan.UnitQty = this.ConvertUomQty(mrpShipPlan.Item, mrpShipPlan.Uom, 1, mrpShipPlan.BaseUom);
                    }
                    else
                    {
                        mrpShipPlan.UnitQty = 1;
                    }
                    mrpShipPlan.CreateDate = dateTimeNow;
                    mrpShipPlan.CreateUser = user.Code;
                    this.RecordTrans(mrpShipPlan, mrpShipPlan.Qty);
                    //this.mrpShipPlanMgr.CreateMrpShipPlan(mrpShipPlan);
                    mrpShipPlanList.Add(mrpShipPlan);

                    log.Debug("Create ship plan for sales order, flow[" + mrpShipPlan.Flow + "], item[" + mrpShipPlan.Item + "], qty[" + mrpShipPlan.Qty + "], sourceType[" + mrpShipPlan.SourceType + "], sourceId[" + (mrpShipPlan.SourceId != null ? mrpShipPlan.SourceId : string.Empty) + "]");
                }
            }

            return mrpShipPlanList;
        }

        private IList<MrpShipPlan> TransferCustomerPlan2ShipPlan(IList<CustomerScheduleDetail> customerScheduleDetaillList, DateTime effectiveDate, DateTime dateTimeNow, User user)
        {
            IList<MrpShipPlan> mrpShipPlanList = new List<MrpShipPlan>();

            if (customerScheduleDetaillList != null && customerScheduleDetaillList.Count() > 0)
            {
                foreach (CustomerScheduleDetail customerScheduleDetail in customerScheduleDetaillList)
                {
                    Item item = this.itemMgr.LoadItem(customerScheduleDetail.Item);
                    MrpShipPlan mrpShipPlan = new MrpShipPlan();

                    mrpShipPlan.Flow = customerScheduleDetail.CustomerSchedule.Flow;
                    mrpShipPlan.FlowType = BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION;
                    mrpShipPlan.Item = customerScheduleDetail.Item;
                    mrpShipPlan.ItemDescription = customerScheduleDetail.ItemDescription;
                    mrpShipPlan.ItemReference = customerScheduleDetail.ItemReference;
                    mrpShipPlan.StartTime = customerScheduleDetail.DateFrom;//customerScheduleDetail.StartTime;
                    mrpShipPlan.WindowTime = customerScheduleDetail.DateTo;//customerScheduleDetail.DateFrom;
                    mrpShipPlan.LocationFrom = customerScheduleDetail.Location;
                    mrpShipPlan.SourceType = BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_CUSTOMER_PLAN;
                    mrpShipPlan.SourceDateType = customerScheduleDetail.Type;
                    mrpShipPlan.SourceId = customerScheduleDetail.Id.ToString();
                    mrpShipPlan.SourceUnitQty = 1;
                    mrpShipPlan.EffectiveDate = effectiveDate;
                    mrpShipPlan.Qty = customerScheduleDetail.Qty;
                    mrpShipPlan.Uom = customerScheduleDetail.Uom;
                    mrpShipPlan.UnitCount = customerScheduleDetail.UnitCount;
                    mrpShipPlan.BaseUom = item.Uom.Code;
                    mrpShipPlan.DateFrom = customerScheduleDetail.DateFrom;
                    mrpShipPlan.DateTo = customerScheduleDetail.DateTo;
                    if (mrpShipPlan.Uom != mrpShipPlan.BaseUom)
                    {
                        mrpShipPlan.UnitQty = this.ConvertUomQty(mrpShipPlan.Item, mrpShipPlan.Uom, 1, mrpShipPlan.BaseUom);
                    }
                    else
                    {
                        mrpShipPlan.UnitQty = 1;
                    }
                    mrpShipPlan.CreateDate = dateTimeNow;
                    mrpShipPlan.CreateUser = user.Code;
                    this.RecordTrans(mrpShipPlan, mrpShipPlan.Qty);
                    //this.mrpShipPlanMgr.CreateMrpShipPlan(mrpShipPlan);
                    mrpShipPlanList.Add(mrpShipPlan);

                    log.Debug("Create ship plan for customer schedule, flow[" + mrpShipPlan.Flow + "], item[" + mrpShipPlan.Item + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "], sourceType[" + mrpShipPlan.SourceType + "], sourceId[" + (mrpShipPlan.SourceId != null ? mrpShipPlan.SourceId : string.Empty) + "]");
                }
            }

            return mrpShipPlanList;
        }

        private IList<MrpShipPlan> TransferSalesOrderAndCustomerPlan2ShipPlan(IList<OrderDetail> salesOrderDetailList, IList<CustomerScheduleDetail> customerScheduleDetaillList, DateTime effectiveDate, DateTime dateTimeNow, User user)
        {
            IList<MrpShipPlan> mrpShipPlanList = TransferSalesOrder2ShipPlan(salesOrderDetailList, effectiveDate, dateTimeNow, user);

            IList<CustomerScheduleDetail> newDetails = new List<CustomerScheduleDetail>();

            if (mrpShipPlanList != null && mrpShipPlanList.Count > 0)
            {
                if (customerScheduleDetaillList != null && customerScheduleDetaillList.Count > 0)
                {
                    #region new
                    var gDetails = from p in customerScheduleDetaillList
                                   group p by new
                                   {
                                       Flow = p.CustomerSchedule.Flow,
                                       Item = p.Item
                                   } into g
                                   select new
                                   {
                                       Flow = g.Key.Flow,
                                       Item = g.Key.Item,
                                       List = g
                                   };

                    foreach (var gDetail in gDetails)
                    {
                        string hql = @"select m.StartTime from OrderDetail as d
                                join d.OrderHead as m
                                where m.Flow = ? and d.Item = ? order by  m.StartTime desc";

                        IList<object> lastStartTime = hqlMgr.FindAll<object>(hql, new object[] { gDetail.Flow, gDetail.Item }, 0, 1);

                        DateTime maxStartTime = effectiveDate;
                        if (lastStartTime != null && lastStartTime.Count > 0)
                        {
                            maxStartTime = (DateTime)(lastStartTime.First());
                        }

                        foreach (var detail in gDetail.List)
                        {
                            if (detail.DateFrom > maxStartTime)
                            {
                                newDetails.Add(detail);
                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {
                newDetails = customerScheduleDetaillList;
            }

            IListHelper.AddRange(mrpShipPlanList, TransferCustomerPlan2ShipPlan(newDetails, effectiveDate, dateTimeNow, user));

            return mrpShipPlanList;
        }

        private void NestCalculateMrpShipPlanAndReceivePlan(MrpShipPlan mrpShipPlan, IList<MrpLocationLotDetail> inventoryBalanceList, IList<TransitInventory> transitInventoryList, IList<FlowDetailSnapShot> flowDetailSnapShotList, IList<ItemDiscontinue> itemDiscontinueList, DateTime effectiveDate, DateTime dateTimeNow, User user)
        {
            //if (mrpShipPlan.IsExpire)
            //{
            //    return; //过期需求不往下传递
            //}

            if (mrpShipPlan.LocationFrom != null && mrpShipPlan.LocationFrom.Trim() != string.Empty)
            {
                #region 消耗本机物料
                if (mrpShipPlan.Qty == 0)
                {
                    return;
                }
                else if (mrpShipPlan.Qty < 0)
                {
                    throw new TechnicalException("Mrp Ship Plan Qty Can't < 0");
                }

                //回冲库存
                BackFlushInventory(mrpShipPlan, mrpShipPlan.Item, mrpShipPlan.UnitQty, inventoryBalanceList);

                //回冲在途
                BackFlushTransitInventory(mrpShipPlan, mrpShipPlan.Item, mrpShipPlan.UnitQty, transitInventoryList);
                //if (mrpShipPlan.StartTime >= effectiveDate      //只有StartTime>= EffectiveDate才能回冲
                //|| mrpShipPlan.SourceType == BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_SAFE_STOCK)  //或者回冲安全库存
                //{

                //}
                #endregion

                #region 消耗替代物料
                if (itemDiscontinueList != null && itemDiscontinueList.Count > 0 && mrpShipPlan.Qty > 0)
                {
                    var discontinuedItemList = from itemDis in itemDiscontinueList
                                               where itemDis.Item.Code == mrpShipPlan.Item
                                               orderby itemDis.Priority ascending
                                               select itemDis;

                    if (discontinuedItemList != null && discontinuedItemList.Count() > 0)
                    {
                        foreach (ItemDiscontinue itemDis in discontinuedItemList)
                        {
                            //回冲库存
                            BackFlushInventory(mrpShipPlan, itemDis.DiscontinueItem.Code, mrpShipPlan.UnitQty * itemDis.UnitQty, inventoryBalanceList);

                            //回冲在途
                            if (itemDis.EndDate >= mrpShipPlan.StartTime)
                            {
                                BackFlushTransitInventory(mrpShipPlan, itemDis.DiscontinueItem.Code, mrpShipPlan.UnitQty * itemDis.UnitQty, transitInventoryList);
                            }
                        }
                    }
                }
                #endregion

                #region 生成入库计划
                if (mrpShipPlan.Qty == 0)
                {
                    return;
                }

                IList<MrpReceivePlan> currMrpReceivePlanList = new List<MrpReceivePlan>();
                if (mrpShipPlan.FlowType != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION)
                {
                    #region 非生产直接从发运计划变为入库计划
                    MrpReceivePlan mrpReceivePlan = new MrpReceivePlan();
                    mrpReceivePlan.IsExpire = mrpShipPlan.IsExpire;
                    mrpReceivePlan.ExpireStartTime = mrpShipPlan.ExpireStartTime;
                    mrpReceivePlan.Item = mrpShipPlan.Item;
                    mrpReceivePlan.ItemDescription = mrpReceivePlan.ItemDescription;
                    mrpReceivePlan.ItemReference = mrpReceivePlan.ItemReference;
                    mrpReceivePlan.Location = mrpShipPlan.LocationFrom;
                    mrpReceivePlan.Qty = mrpShipPlan.Qty * mrpShipPlan.UnitQty;  //转换为库存单位
                    mrpReceivePlan.UnitCount = mrpShipPlan.UnitCount;
                    mrpReceivePlan.Uom = mrpShipPlan.BaseUom;
                    mrpReceivePlan.ReceiveTime = mrpShipPlan.StartTime;
                    mrpReceivePlan.SourceId = mrpShipPlan.SourceId;
                    mrpReceivePlan.SourceDateType = mrpShipPlan.SourceDateType;
                    mrpReceivePlan.SourceType = mrpShipPlan.SourceType;
                    mrpReceivePlan.SourceUnitQty = mrpShipPlan.SourceUnitQty * mrpShipPlan.UnitQty;
                    mrpReceivePlan.EffectiveDate = effectiveDate;
                    mrpReceivePlan.CreateDate = dateTimeNow;
                    mrpReceivePlan.CreateUser = user.Code;
                    mrpReceivePlan.FlowDetailIdList = mrpShipPlan.FlowDetailIdList;

                    //this.mrpReceivePlanMgr.CreateMrpReceivePlan(mrpReceivePlan);

                    currMrpReceivePlanList.Add(mrpReceivePlan);

                    log.Debug("Transfer ship plan flow[" + mrpShipPlan.Flow + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "] to receive plan location[" + mrpReceivePlan.Location + "], item[" + mrpReceivePlan.Item + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "]");
                    #endregion
                }
                else
                {
                    #region 生产，需要分解Bom
                    log.Debug("Production flow start resolve bom");
                    Bom bom = this.bomMgr.LoadBom(mrpShipPlan.Bom);
                    if (bom == null)
                    {
                        log.Error("Can Find Bom:" + mrpShipPlan.Bom);
                        return;
                    }
                    IList<BomDetail> bomDetailList = this.bomDetailMgr.GetFlatBomDetail(mrpShipPlan.Bom, mrpShipPlan.StartTime);

                    if (bomDetailList != null && bomDetailList.Count > 0)
                    {
                        IList<RoutingDetail> routingDetailList = null;
                        if (mrpShipPlan.Routing != null && mrpShipPlan.Routing.Trim() != null)
                        {
                            routingDetailList = this.routingDetailMgr.GetRoutingDetail(mrpShipPlan.Routing, mrpShipPlan.StartTime);
                        }

                        foreach (BomDetail bomDetail in bomDetailList)
                        {
                            log.Debug("Find bomDetail FG[" + mrpShipPlan.Item + "], RM[" + bomDetail.Item.Code + "]");

                            #region 创建MrpReceivePlan
                            MrpReceivePlan mrpReceivePlan = new MrpReceivePlan();
                            mrpReceivePlan.IsExpire = mrpShipPlan.IsExpire;
                            mrpReceivePlan.ExpireStartTime = mrpShipPlan.ExpireStartTime;
                            mrpReceivePlan.Item = bomDetail.Item.Code;
                            mrpReceivePlan.UnitCount = bomDetail.Item.UnitCount;
                            mrpReceivePlan.ItemDescription = bomDetail.Item.Description;

                            mrpReceivePlan.ReceiveTime = mrpShipPlan.StartTime;
                            mrpReceivePlan.SourceId = mrpShipPlan.SourceId;
                            mrpReceivePlan.SourceDateType = mrpShipPlan.SourceDateType;
                            mrpReceivePlan.SourceType = mrpShipPlan.SourceType;
                            mrpReceivePlan.EffectiveDate = effectiveDate;
                            mrpReceivePlan.CreateDate = dateTimeNow;
                            mrpReceivePlan.CreateUser = user.Code;
                            mrpReceivePlan.FlowDetailIdList = mrpShipPlan.FlowDetailIdList;

                            #region 取库位
                            mrpReceivePlan.Location = mrpShipPlan.LocationFrom;  //默认库位
                            if (bomDetail.Location != null)
                            {
                                mrpReceivePlan.Location = bomDetail.Location.Code;
                            }
                            else
                            {
                                if (routingDetailList != null)
                                {
                                    Location location = (from det in routingDetailList
                                                         where det.Operation == bomDetail.Operation
                                                         && det.Reference == bomDetail.Reference
                                                         select det.Location).FirstOrDefault();

                                    if (location != null)
                                    {
                                        mrpReceivePlan.Location = location.Code;
                                    }
                                }
                            }
                            #endregion
                            decimal fgQty = mrpShipPlan.Qty;
                            decimal fgSourceUnitQty = mrpShipPlan.SourceUnitQty;
                            if (mrpShipPlan.Uom != bom.Uom.Code)
                            {
                                //成品数量转换为Bom单位
                                fgQty = this.ConvertUomQty(mrpShipPlan.Item, mrpShipPlan.Uom, fgQty, bom.Uom.Code);
                                fgSourceUnitQty = this.ConvertUomQty(mrpShipPlan.Item, mrpShipPlan.Uom, fgSourceUnitQty, bom.Uom.Code);
                            }
                            mrpReceivePlan.Uom = bomDetail.Item.Uom.Code;
                            #region 计算用量
                            //BomDetail上的单位
                            mrpReceivePlan.Qty = fgQty //成品用量                                  
                                * bomDetail.RateQty //乘以单位用量
                                * (1 + bomDetail.DefaultScrapPercentage);  //乘以损耗
                            mrpReceivePlan.SourceUnitQty = fgSourceUnitQty
                                * bomDetail.RateQty //乘以单位用量
                                * (1 + bomDetail.DefaultScrapPercentage);  //乘以损耗
                            if (mrpReceivePlan.Uom != bomDetail.Uom.Code)
                            {
                                //转换为库存单位
                                mrpReceivePlan.Qty = this.ConvertUomQty(mrpReceivePlan.Item, bomDetail.Uom.Code, mrpReceivePlan.Qty, mrpReceivePlan.Uom);
                                mrpReceivePlan.SourceUnitQty = this.ConvertUomQty(mrpReceivePlan.Item, bomDetail.Uom.Code, mrpReceivePlan.SourceUnitQty, mrpReceivePlan.Uom);
                            }

                            #region 消耗本级物料
                            mrpReceivePlan.StartTime = mrpShipPlan.StartTime;
                            mrpReceivePlan.WindowTime = mrpShipPlan.StartTime;
                            mrpReceivePlan.Flow = mrpShipPlan.Flow;
                            mrpReceivePlan.FlowType = mrpShipPlan.FlowType;
                            #region 扣减线边库位库存

                            BackFlushInventory(mrpReceivePlan, mrpReceivePlan.Item, 1, inventoryBalanceList);
                            #endregion

                            #region 扣减线边在途库存
                            BackFlushTransitInventory(mrpReceivePlan, mrpReceivePlan.Item, 1, transitInventoryList);
                            #endregion
                            #endregion

                            #region 消耗替代物料
                            if (itemDiscontinueList != null && itemDiscontinueList.Count > 0 && mrpReceivePlan.Qty > 0)
                            {
                                var discontinuedItemList = from itemDis in itemDiscontinueList
                                                           where itemDis.Item.Code == mrpReceivePlan.Item
                                                           orderby itemDis.Priority ascending
                                                           select itemDis;

                                if (discontinuedItemList != null && discontinuedItemList.Count() > 0)
                                {
                                    foreach (ItemDiscontinue itemDis in discontinuedItemList)
                                    {
                                        //回冲库存
                                        BackFlushInventory(mrpReceivePlan, itemDis.DiscontinueItem.Code, itemDis.UnitQty, inventoryBalanceList);

                                        //回冲在途
                                        if (itemDis.EndDate >= mrpReceivePlan.ReceiveTime)
                                        {
                                            BackFlushTransitInventory(mrpReceivePlan, itemDis.DiscontinueItem.Code, itemDis.UnitQty, transitInventoryList);
                                        }
                                    }
                                }
                            }
                            #endregion
                            #endregion


                            //this.mrpReceivePlanMgr.CreateMrpReceivePlan(mrpReceivePlan);
                            currMrpReceivePlanList.Add(mrpReceivePlan);
                            #endregion
                        }
                    }
                    else
                    {
                        log.Error("Can't find bom detial for code " + mrpShipPlan.Bom);
                    }
                    log.Debug("Production flow end resolve bom");
                    #endregion
                }
                #endregion

                #region 计算下游发运计划
                foreach (MrpReceivePlan mrpReceivePlan in currMrpReceivePlanList)
                {
                    log.Debug("Transfer ship plan flow[" + mrpShipPlan.Flow + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "] to receive plan location[" + mrpReceivePlan.Location + "], item[" + mrpReceivePlan.Item + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "]");
                    CalculateNextShipPlan(mrpReceivePlan, inventoryBalanceList, transitInventoryList, flowDetailSnapShotList, itemDiscontinueList, effectiveDate, dateTimeNow, user);
                }
                #endregion
            }
        }

        private void CalculateNextShipPlan(MrpReceivePlan mrpReceivePlan, IList<MrpLocationLotDetail> inventoryBalanceList, IList<TransitInventory> transitInventoryList, IList<FlowDetailSnapShot> flowDetailSnapShotList, IList<ItemDiscontinue> itemDiscontinueList, DateTime effectiveDate, DateTime dateTimeNow, User user)
        {
            if (mrpReceivePlan.ReceiveTime < effectiveDate)
            {
                //如果窗口时间小于effectivedate，不往下计算
                //return;
            }
            var nextFlowDetailList = from det in flowDetailSnapShotList
                                     where det.LocationTo == mrpReceivePlan.Location
                                    && det.Item == mrpReceivePlan.Item
                                     select det;

            if (nextFlowDetailList != null && nextFlowDetailList.Count() > 0)
            {
                int mrpWeight = nextFlowDetailList.Sum(p => p.MRPWeight);
                decimal rate = mrpReceivePlan.Qty / mrpWeight;
                decimal remainQty = mrpReceivePlan.Qty;

                for (int i = 0; i < nextFlowDetailList.Count(); i++)
                {
                    FlowDetailSnapShot flowDetail = nextFlowDetailList.ElementAt(i);

                    MrpShipPlan mrpShipPlan = new MrpShipPlan();

                    if (mrpReceivePlan.ContainFlowDetailId(flowDetail.Id))
                    {
                        log.Info("Cycle Flow Detail Find when transfer receive plan location[" + mrpReceivePlan.Location + "], item[" + mrpReceivePlan.Item + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "] to ship plan flow[" + flowDetail.Flow + "]");
                        //continue;
                    }
                    else
                    {
                        mrpShipPlan.FlowDetailIdList = mrpReceivePlan.FlowDetailIdList;
                        mrpShipPlan.AddFlowDetailId(flowDetail.Id);
                    }

                    mrpShipPlan.Flow = flowDetail.Flow;
                    mrpShipPlan.FlowType = flowDetail.FlowType;
                    mrpShipPlan.Item = flowDetail.Item;
                    mrpShipPlan.ItemDescription = flowDetail.ItemDescription;//CODE_MASTER_MRP_SOURCE_TYPE_VALUE_SAFE_STOCK
                    if (mrpReceivePlan.SourceDateType != BusinessConstants.CODE_MASTER_MRP_SOURCE_TYPE_VALUE_SAFE_STOCK)
                    {
                        mrpShipPlan.StartTime = mrpReceivePlan.ReceiveTime.AddHours(-Convert.ToDouble(flowDetail.LeadTime));
                    }
                    else
                    {
                        mrpShipPlan.StartTime = mrpReceivePlan.ReceiveTime;
                    }
                    if (mrpShipPlan.StartTime < effectiveDate)
                    {
                        mrpShipPlan.IsExpire = true;
                        mrpShipPlan.ExpireStartTime = mrpShipPlan.StartTime;
                        mrpShipPlan.StartTime = dateTimeNow;
                    }
                    else
                    {
                        mrpShipPlan.IsExpire = false;
                    }
                    mrpShipPlan.WindowTime = mrpReceivePlan.ReceiveTime;
                    mrpShipPlan.LocationFrom = flowDetail.LocationFrom;
                    mrpShipPlan.LocationTo = flowDetail.LocationTo;
                    mrpShipPlan.SourceType = mrpReceivePlan.SourceType;
                    mrpShipPlan.SourceDateType = mrpReceivePlan.SourceDateType;
                    mrpShipPlan.SourceId = mrpReceivePlan.SourceId;
                    mrpShipPlan.EffectiveDate = effectiveDate;
                    mrpShipPlan.Uom = flowDetail.Uom;
                    mrpShipPlan.BaseUom = flowDetail.BaseUom;
                    if (mrpShipPlan.Uom != mrpShipPlan.BaseUom)
                    {
                        mrpShipPlan.UnitQty = this.ConvertUomQty(mrpShipPlan.Item, mrpShipPlan.Uom, 1, mrpShipPlan.BaseUom);
                    }
                    else
                    {
                        mrpShipPlan.UnitQty = 1;
                    }
                    if (i != nextFlowDetailList.Count() - 1)
                    {
                        remainQty -= rate * flowDetail.MRPWeight;
                        mrpShipPlan.Qty = rate * flowDetail.MRPWeight / mrpShipPlan.UnitQty;   //转换为定单单位                        
                    }
                    else
                    {
                        mrpShipPlan.Qty = remainQty / mrpShipPlan.UnitQty;   //转换为定单单位
                    }
                    mrpShipPlan.SourceUnitQty = mrpReceivePlan.SourceUnitQty / mrpWeight * flowDetail.MRPWeight / mrpShipPlan.UnitQty;
                    mrpShipPlan.UnitCount = flowDetail.UnitCount;
                    mrpShipPlan.Bom = flowDetail.Bom;
                    mrpShipPlan.Routing = flowDetail.Routing;
                    //mrpShipPlan.IsExpire = mrpReceivePlan.IsExpire;
                    //mrpShipPlan.ExpireStartTime = mrpReceivePlan.ExpireStartTime;
                    mrpShipPlan.CreateDate = dateTimeNow;
                    mrpShipPlan.CreateUser = user.Code;

                    this.RecordTrans(mrpShipPlan, mrpShipPlan.Qty);
                    //this.mrpShipPlanMgr.CreateMrpShipPlan(mrpShipPlan);
                    log.Debug("Transfer receive plan location[" + mrpReceivePlan.Location + "], item[" + mrpReceivePlan.Item + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "] to ship plan flow[" + mrpShipPlan.Flow + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "]");

                    NestCalculateMrpShipPlanAndReceivePlan(mrpShipPlan, inventoryBalanceList, transitInventoryList, flowDetailSnapShotList, itemDiscontinueList, effectiveDate, dateTimeNow, user);
                }
            }
            else
            {
                log.Warn("Can't find next flow for location[" + mrpReceivePlan.Location + "], item[" + mrpReceivePlan.Item + "]");
            }
        }

        private void BackFlushInventory(MrpShipPlan mrpShipPlan, string itemCode, decimal unitQty, IList<MrpLocationLotDetail> inventoryBalanceList)
        {
            #region 先消耗库存
            if (mrpShipPlan.Qty == 0)
            {
                return;
            }

            var inventoryConsumed = from inv in inventoryBalanceList
                                    where inv.Location == mrpShipPlan.LocationFrom
                                    && inv.Item == itemCode && inv.Qty > inv.SafeQty
                                    select inv;

            if (inventoryConsumed != null && inventoryConsumed.Count() > 0)
            {
                foreach (MrpLocationLotDetail inventory in inventoryConsumed)
                {
                    if (mrpShipPlan.Qty * unitQty > inventory.ActiveQty)
                    {
                        log.Debug("Backflush inventory for mrpShipPlan flow[" + mrpShipPlan.Flow + "], item[" + itemCode + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "], sourceType[" + mrpShipPlan.SourceType + "], sourceId[" + (mrpShipPlan.SourceId != null ? mrpShipPlan.SourceId : string.Empty) + "], backflushQty[" + (inventory.ActiveQty / unitQty).ToString("0.####") + "]");

                        mrpShipPlan.Qty -= inventory.ActiveQty / unitQty;
                        inventory.Qty = inventory.SafeQty;
                    }
                    else
                    {
                        log.Debug("Backflush inventory for mrpShipPlan flow[" + mrpShipPlan.Flow + "], item[" + itemCode + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "], sourceType[" + mrpShipPlan.SourceType + "], sourceId[" + (mrpShipPlan.SourceId != null ? mrpShipPlan.SourceId : string.Empty) + "], backflushQty[" + (mrpShipPlan.Qty * unitQty).ToString("0.####") + "]");

                        inventory.Qty -= mrpShipPlan.Qty * unitQty;
                        mrpShipPlan.Qty = 0;
                        break;
                    }
                    this.RecordTrans(mrpShipPlan, inventory.ActiveQty / unitQty);
                }
            }
            #endregion
        }

        private void BackFlushTransitInventory(MrpShipPlan mrpShipPlan, string itemCode, decimal unitQty, IList<TransitInventory> transitInventoryList)
        {
            #region 再根据ShipPlan的StartTime < 在途库存的EffectiveDate消耗在途库存
            if (mrpShipPlan.Qty == 0)
            {
                return;
            }

            var transitConsumed = from trans in transitInventoryList
                                  where trans.Location == mrpShipPlan.LocationFrom
                                      && trans.Item == itemCode && trans.Qty > 0
                                      && trans.EffectiveDate <= mrpShipPlan.StartTime
                                  select trans;

            if (transitConsumed != null && transitConsumed.Count() > 0)
            {
                foreach (TransitInventory inventory in transitConsumed)
                {
                    if (mrpShipPlan.Qty * unitQty > inventory.Qty)
                    {
                        log.Debug("Backflush transit inventory for mrpShipPlan flow[" + mrpShipPlan.Flow + "], item[" + itemCode + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "], sourceType[" + mrpShipPlan.SourceType + "], sourceId[" + (mrpShipPlan.SourceId != null ? mrpShipPlan.SourceId : string.Empty) + "], effectiveDate[" + inventory.EffectiveDate + "], backflushQty[" + (inventory.Qty / unitQty).ToString("0.####") + "]");

                        mrpShipPlan.Qty -= inventory.Qty / unitQty;
                        inventory.Qty = 0;
                    }
                    else
                    {
                        log.Debug("Backflush transit inventory for mrpShipPlan flow[" + mrpShipPlan.Flow + "], item[" + itemCode + "], qty[" + mrpShipPlan.Qty.ToString("0.####") + "], sourceType[" + mrpShipPlan.SourceType + "], sourceId[" + (mrpShipPlan.SourceId != null ? mrpShipPlan.SourceId : string.Empty) + "], effectiveDate[" + inventory.EffectiveDate + "], backflushQty[" + (mrpShipPlan.Qty * unitQty).ToString("0.####") + "]");

                        inventory.Qty -= mrpShipPlan.Qty * unitQty;
                        mrpShipPlan.Qty = 0;
                        break;
                    }
                    this.RecordTrans(mrpShipPlan, inventory.Qty / unitQty);
                }
            }
            #endregion
        }

        private void BackFlushInventory(MrpReceivePlan mrpReceivePlan, string itemCode, decimal unitQty, IList<MrpLocationLotDetail> inventoryBalanceList)
        {
            #region 先消耗库存
            if (mrpReceivePlan.Qty == 0)
            {
                return;
            }

            var wipInventoryConsumed = from inv in inventoryBalanceList
                                       where inv.Location == mrpReceivePlan.Location
                                       && inv.Item == mrpReceivePlan.Item && inv.Qty > inv.SafeQty
                                       select inv;

            if (wipInventoryConsumed != null && wipInventoryConsumed.Count() > 0)
            {
                foreach (MrpLocationLotDetail inventory in wipInventoryConsumed)
                {
                    if (mrpReceivePlan.Qty * unitQty > inventory.ActiveQty)
                    {
                        log.Debug("Backflush inventory for mrpReceivePlan location[" + mrpReceivePlan.Location + "], item[" + itemCode + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "], backflushQty[" + (inventory.ActiveQty / unitQty).ToString("0.####") + "]");

                        mrpReceivePlan.Qty -= inventory.ActiveQty / unitQty;
                        inventory.Qty = inventory.SafeQty;
                    }
                    else
                    {
                        log.Debug("Backflush inventory for mrpReceivePlan location[" + mrpReceivePlan.Location + "], item[" + itemCode + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "], backflushQty[" + (mrpReceivePlan.Qty * unitQty).ToString("0.####") + "]");

                        inventory.Qty -= mrpReceivePlan.Qty * unitQty;
                        mrpReceivePlan.Qty = 0;
                        break;
                    }
                    this.RecordTrans(mrpReceivePlan, inventory.ActiveQty / unitQty);
                }
            }
            #endregion
        }

        private void BackFlushTransitInventory(MrpReceivePlan mrpReceivePlan, string itemCode, decimal unitQty, IList<TransitInventory> transitInventoryList)
        {
            #region 再根据ShipPlan的StartTime < 在途库存的EffectiveDate消耗在途库存
            if (mrpReceivePlan.Qty == 0)
            {
                return;
            }

            var transitConsumed = from trans in transitInventoryList
                                  where trans.Location == mrpReceivePlan.Location
                                      && trans.Item == itemCode && trans.Qty > 0
                                      && trans.EffectiveDate <= mrpReceivePlan.ReceiveTime
                                  select trans;

            if (transitConsumed != null && transitConsumed.Count() > 0)
            {
                foreach (TransitInventory inventory in transitConsumed)
                {
                    if (mrpReceivePlan.Qty * unitQty > inventory.Qty)
                    {
                        log.Debug("Backflush transit inventory for mrpReceivePlan location[" + mrpReceivePlan.Location + "], item[" + itemCode + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "], effectiveDate[" + inventory.EffectiveDate + "], backflushQty[" + (inventory.Qty / unitQty).ToString("0.####") + "]");

                        mrpReceivePlan.Qty -= inventory.Qty / unitQty;
                        inventory.Qty = 0;
                    }
                    else
                    {
                        log.Debug("Backflush transit inventory for mrpReceivePlan location[" + mrpReceivePlan.Location + "], item[" + itemCode + "], qty[" + mrpReceivePlan.Qty.ToString("0.####") + "], sourceType[" + mrpReceivePlan.SourceType + "], sourceId[" + (mrpReceivePlan.SourceId != null ? mrpReceivePlan.SourceId : string.Empty) + "], effectiveDate[" + inventory.EffectiveDate + "], backflushQty[" + (mrpReceivePlan.Qty * unitQty).ToString("0.####") + "]");

                        inventory.Qty -= mrpReceivePlan.Qty * unitQty;
                        mrpReceivePlan.Qty = 0;

                        break;
                    }
                    this.RecordTrans(mrpReceivePlan, inventory.Qty / unitQty);
                }
            }
            #endregion
        }

        /// <summary>
        /// 仅支持无限级单位转换
        /// </summary>
        private decimal ConvertUomQty(string itemCode, string sourceUomCode, decimal sourceQty, string targetUomCode)
        {
            if (itemCode == null || sourceUomCode == null || targetUomCode == null)
            {
                //throw new BusinessErrorException("UomConversion Error:itemCode Or sourceUomCode Or targetUomCode is null");
                log.Error("UomConversion Error:itemCode Or sourceUomCode Or targetUomCode is null");
                return sourceQty;
            }

            if (sourceUomCode == targetUomCode || sourceQty == 0)
            {
                return sourceQty;
            }

            DetachedCriteria criteria = DetachedCriteria.For(typeof(UomConversion));
            criteria.Add(Expression.Or(Expression.IsNull("Item"), Expression.Eq("Item.Code", itemCode)));

            IList<UomConversion> unGroupUomConversionList = criteriaMgr.FindAll<UomConversion>(criteria);
            if (unGroupUomConversionList != null)
            {
                List<UomConversion> uomConversionList = unGroupUomConversionList.Where(u => u.Item != null).ToList();
                foreach (UomConversion y in unGroupUomConversionList)
                {
                    if (uomConversionList.Where(x => (StringHelper.Eq(x.AlterUom.Code, y.AlterUom.Code) && StringHelper.Eq(x.BaseUom.Code, y.BaseUom.Code))
                        || (StringHelper.Eq(x.AlterUom.Code, y.BaseUom.Code) && StringHelper.Eq(x.BaseUom.Code, y.AlterUom.Code))).Count() == 0)
                    {
                        uomConversionList.Add(y);
                    }
                }
                foreach (UomConversion u in uomConversionList)
                {
                    //顺
                    if (StringHelper.Eq(u.BaseUom.Code, sourceUomCode))
                    {
                        u.Qty = sourceQty * u.AlterQty / u.BaseQty;
                        u.IsAsc = true;
                        if (StringHelper.Eq(u.AlterUom.Code, targetUomCode))
                        {
                            return u.Qty.Value;
                        }
                    }
                    //反
                    else if (StringHelper.Eq(u.AlterUom.Code, sourceUomCode))
                    {
                        u.Qty = sourceQty * u.BaseQty / u.AlterQty;
                        u.IsAsc = false;
                        if (StringHelper.Eq(u.BaseUom.Code, targetUomCode))
                        {
                            return u.Qty.Value;
                        }
                    }
                }

                for (int i = 1; i < uomConversionList.Count; i++)
                {
                    foreach (UomConversion uomConversion1 in uomConversionList)
                    {
                        if (uomConversion1.Qty.HasValue)
                        {
                            foreach (UomConversion uomConversion2 in uomConversionList)
                            {
                                //顺
                                if (uomConversion1.IsAsc)
                                {
                                    //顺
                                    if (StringHelper.Eq(uomConversion2.BaseUom.Code, uomConversion1.AlterUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.AlterQty / uomConversion2.BaseQty;
                                        uomConversion2.IsAsc = true;
                                        if (StringHelper.Eq(uomConversion2.AlterUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                    //反
                                    else if (StringHelper.Eq(uomConversion2.AlterUom.Code, uomConversion1.AlterUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.BaseQty / uomConversion2.AlterQty;
                                        uomConversion2.IsAsc = false;
                                        if (StringHelper.Eq(uomConversion2.BaseUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                }
                                //反
                                else
                                {
                                    //顺
                                    if (StringHelper.Eq(uomConversion2.BaseUom.Code, uomConversion1.BaseUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.AlterQty / uomConversion2.BaseQty;
                                        uomConversion2.IsAsc = true;
                                        if (StringHelper.Eq(uomConversion2.AlterUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                    //反
                                    else if (StringHelper.Eq(uomConversion2.AlterUom.Code, uomConversion1.BaseUom.Code) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.BaseQty / uomConversion2.AlterQty;
                                        uomConversion2.IsAsc = false;
                                        if (StringHelper.Eq(uomConversion2.BaseUom.Code, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //throw new BusinessErrorException("UomConversion.Error.NotFound", itemCode, sourceUomCode, targetUomCode);
            log.Error("UomConversion.Error.NotFound,itemCode:" + itemCode + ",sourceUomCode:" + sourceUomCode + ",targetUomCode:" + targetUomCode);
            return sourceQty;
        }

        private void RecordTrans(MrpShipPlan mrpShipPlan, decimal recordQty)
        {
            MrpPlanTransaction trans = new MrpPlanTransaction();
            trans.CreateDate = mrpShipPlan.CreateDate;
            trans.CreateUser = mrpShipPlan.CreateUser;
            trans.EffectiveDate = mrpShipPlan.EffectiveDate;
            trans.Flow = mrpShipPlan.Flow;
            trans.FlowType = mrpShipPlan.FlowType;
            trans.Item = mrpShipPlan.Item;
            trans.Location = mrpShipPlan.LocationTo;
            trans.PeriodType = mrpShipPlan.SourceDateType;
            trans.Reference = mrpShipPlan.SourceId;
            trans.Qty = recordQty;
            trans.SourceType = mrpShipPlan.SourceType;
            trans.StartTime = mrpShipPlan.StartTime;
            trans.UnitCount = mrpShipPlan.UnitCount;
            trans.Uom = mrpShipPlan.Uom;
            trans.WindowTime = mrpShipPlan.WindowTime;
            mrpPlanTransactionMgr.CreateMrpPlanTransaction(trans);
        }

        private void RecordTrans(MrpReceivePlan mrpReceivePlan, decimal recordQty)
        {
            MrpPlanTransaction trans = new MrpPlanTransaction();
            trans.CreateDate = mrpReceivePlan.CreateDate;
            trans.CreateUser = mrpReceivePlan.CreateUser;
            trans.EffectiveDate = mrpReceivePlan.EffectiveDate;
            trans.Flow = mrpReceivePlan.Flow;
            trans.FlowType = mrpReceivePlan.FlowType;
            trans.Item = mrpReceivePlan.Item;
            trans.Location = mrpReceivePlan.Location;
            trans.PeriodType = mrpReceivePlan.SourceDateType;
            trans.Reference = mrpReceivePlan.SourceId;
            trans.Qty = recordQty;
            trans.SourceType = mrpReceivePlan.SourceType;
            trans.StartTime = mrpReceivePlan.StartTime;
            trans.UnitCount = mrpReceivePlan.UnitCount;
            trans.Uom = mrpReceivePlan.Uom;
            trans.WindowTime = mrpReceivePlan.WindowTime;
            mrpPlanTransactionMgr.CreateMrpPlanTransaction(trans);
        }

        #endregion
    }

    class SafeInventory
    {
        public string Location { get; set; }
        public string Item { get; set; }
        public decimal SafeQty { get; set; }
    }

    class TransitInventory
    {
        public string Location { get; set; }
        public string Item { get; set; }
        public decimal Qty { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    class FlowDetailSnapShot
    {
        public string Flow { get; set; }
        public string FlowType { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string Item { get; set; }
        public int MRPWeight { get; set; }
        public string Bom { get; set; }
        public string Routing { get; set; }
        public string BaseUom { get; set; }
        public string Uom { get; set; }
        public decimal LeadTime { get; set; }
        public decimal UnitCount { get; set; }
        public string ItemDescription { get; set; }
        public int Id { get; set; }
    }

    class SafeInventoryComparer : IEqualityComparer<SafeInventory>
    {
        public bool Equals(SafeInventory x, SafeInventory y)
        {
            return x.Location == y.Location && x.Item == y.Item;
        }

        public int GetHashCode(SafeInventory obj)
        {
            string hCode = obj.Location + "|" + obj.Item;
            return hCode.GetHashCode();
        }
    }

    class SourceDateTypeComparer : IComparer<MrpShipPlan>
    {
        public int Compare(MrpShipPlan x, MrpShipPlan y)
        {
            if (x.SourceDateType == y.SourceDateType)
            {
                return 0;
            }
            else if (x.SourceDateType == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY)
            {
                return 1;
            }
            else if (x.SourceDateType == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_WEEK
                && x.SourceDateType == BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_MONTH)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }


    class LastActionQty
    {
        public string Flow { get; set; }
        public string Item { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitQty { get; set; }
    }

}

#region Extend Class

namespace com.Sconit.Service.Ext.MRP.Impl
{
    [Transactional]
    public partial class MrpMgrE : com.Sconit.Service.MRP.Impl.MrpMgr, IMrpMgrE
    {
    }
}

#endregion Extend Class