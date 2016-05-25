using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Distribution;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Procurement;
using com.Sconit.Entity.Production;
using com.Sconit.Service.Distribution;
using com.Sconit.Service.Procurement;
using com.Sconit.Service.Production;
using com.Sconit.Utility;
using com.Sconit.Service.Criteria;
using NHibernate.Expression;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Ext.Distribution;
using com.Sconit.Service.Ext.Procurement;
using com.Sconit.Service.Ext.Criteria;
using LeanEngine.Utility;
using System.Linq;
using com.Sconit.Service.Ext.Cost;
using System.Net.Mail;
using System.Text;
using com.Sconit.Entity.View;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.Entity.MRP;
using com.Sconit.Entity.FMS;

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class OrderMgr : IOrderMgr
    {
        #region 变量
        //public ISmtpMgrE smtpMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public IOrderHeadMgrE orderHeadMgrE { get; set; }
        public IOrderDetailMgrE orderDetailMgrE { get; set; }
        public IOrderLocationTransactionMgrE orderLocationTransactionMgrE { get; set; }
        public IOrderOperationMgrE orderOperationMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public IFlowMgrE flowMgrE { get; set; }
        public IFlowDetailMgrE flowDetailMgrE { get; set; }
        public INumberControlMgrE numberControlMgrE { get; set; }
        public IInProcessLocationMgrE inProcessLocationMgrE { get; set; }
        public IInProcessLocationDetailMgrE inProcessLocationDetailMgrE { get; set; }
        public IAutoOrderTrackMgrE autoOrderTrackMgrE { get; set; }
        public IReceiptMgrE receiptMgrE { get; set; }
        public IFlowBindingMgrE flowBindingMgrE { get; set; }
        public IUomConversionMgrE uomConversionMgrE { get; set; }
        public IItemKitMgrE itemKitMgrE { get; set; }
        public IWorkingHoursMgrE workingHoursMgrE { get; set; }
        public IOrderBindingMgrE orderBindingMgrE { get; set; }
        public ILocationMgrE locationMgrE { get; set; }
        public IPickListMgrE pickListMgrE { get; set; }
        public IPickListResultMgrE pickListResultMgrE { get; set; }
        public IShiftMgrE shiftMgrE { get; set; }
        public IOrderPlannedBackflushMgrE orderPlannedBackflushMgrE { get; set; }
        public IActingBillMgrE actingBillMgrE { get; set; }
        public IPlannedBillMgrE plannedBillMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IHuMgrE huMgrE { get; set; }
        public IPriceListDetailMgrE priceListDetailMgrE { get; set; }
        public ILocationLotDetailMgrE locationLotDetailMgrE { get; set; }
        public IOrderTracerMgrE orderTracerMgrE { get; set; }
        public ICostMgrE costMgr { set; get; }
        public ICostCenterMgrE costCenterMgr { set; get; }
        public IBomMgrE bomMgr { set; get; }
        public IBomDetailMgrE bomDetailMgr { set; get; }
        public IRoutingDetailMgrE routingDetailMgr { set; get; }
        public IHqlMgrE hqlMgr { set; get; }
        public IGenericMgr genericMgr { get; set; }

        private string[] FlowHead2OrderHeadCloneFields = new string[] 
            { 
                "Type",
                "PartyFrom",
                "PartyTo",
                "ShipFrom",
                "ShipTo",
                "LocationFrom",
                "LocationTo",
                "BillAddress",
                "PriceList",
                "DockDescription",
                "Carrier",
                "CarrierBillAddress",
                "Routing",
                "IsAutoRelease",
                "IsAutoStart",
                "IsAutoShip",
                "IsAutoReceive",
                "IsAutoBill",
                "StartLatency",
                "CompleteLatency",
                "NeedPrintOrder",
                "NeedPrintAsn",
                "NeedPrintReceipt",
                "GoodsReceiptGapTo",
                "AllowExceed",
                "AllowCreateDetail",
                "OrderTemplate",
                "AsnTemplate",
                "ReceiptTemplate",
                "HuTemplate",
                "CheckDetailOption",
                "Currency",
                "IsShowPrice",
                "BillSettleTerm",
                "FulfillUnitCount",
                "IsShipScanHu",
                "IsReceiptScanHu",
                "AutoPrintHu",
                "IsOddCreateHu",
                "CreateHuOption",
                "IsAutoCreatePickList",
                "NeedInspection",
                "IsGoodsReceiveFIFO",
                "MaxOnlineQty",
                "AllowRepeatlyExceed",
                "IsPickFromBin",
                "IsShipByOrder",
                "IsAsnUniqueReceipt",
                "InspectLocationFrom",
                "InspectLocationTo",
                "RejectLocationFrom",
                "RejectLocationTo",
                "NeedRejectInspection"
            };

        private string[] OrderHead2OrderHeadCloneFields = new string[] 
            { 
                "ReferenceOrderNo",
                "ExternalOrderNo",
                "Sequence",
                "StartTime",
                "WindowTime",
                "Priority",
                "Type",
                "PartyFrom",
                "PartyTo",
                "ShipFrom",
                "ShipTo",
                "LocationFrom",
                "LocationTo",
                "BillAddress",
                "PriceList",
                "DockDescription",
                "Carrier",
                "CarrierBillAddress",
                "Routing",
                "IsAutoRelease",
                "IsAutoStart",
                "IsAutoShip",
                "IsAutoReceive",
                "IsAutoBill",
                "StartLatency",
                "CompleteLatency",
                "NeedPrintOrder",
                "NeedPrintAsn",
                "NeedPrintReceipt",
                "GoodsReceiptGapTo",
                "AllowExceed",
                "AllowCreateDetail",
                "OrderTemplate",
                "AsnTemplate",
                "ReceiptTemplate",
                "HuTemplate",
                "CancelReason",
                "Memo",
                "CheckDetailOption",
                "Currency",
                "IsShowPrice",
                "DiscountFrom",
                "DiscountTo",
                "BillSettleTerm",
                "FulfillUnitCount",
                "IsShipScanHu",
                "IsReceiptScanHu",
                "AutoPrintHu",
                "IsOddCreateHu",
                "CreateHuOption",
                "IsAutoCreatePickList",
                "NeedInspection",
                "IsGoodsReceiveFIFO",
                "MaxOnlineQty",
                "AllowRepeatlyExceed",
                "IsPickFromBin",
                "IsShipByOrder",
                "IsAsnUniqueReceipt",
                "InspectLocationFrom",
                "InspectLocationTo",
                "RejectLocationFrom",
                "RejectLocationTo",
                "NeedRejectInspection",
                "SettleTime"
            };
        #endregion

        #region IOrderMgr接口实现
        [Transaction(TransactionMode.Unspecified)]
        public OrderHead LoadOrder(string orderNo, string userCode)
        {
            return this.LoadOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead LoadOrder(string orderNo, User user)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            //OrderHelper.CheckOrderOperationAuthrize(orderHead, user, new List<string>());
            return orderHead;
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrder(OrderHead orderHead, string userCode)
        {
            UpdateOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrder(OrderHead orderHead, string userCode, bool updateDetail)
        {
            UpdateOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode), updateDetail);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrder(OrderHead orderHead, User user)
        {
            UpdateOrder(orderHead, user, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrder(OrderHead orderHead, User user, bool updateDetail)
        {
            OrderHead oldOrderHead = orderHeadMgrE.CheckAndLoadOrderHead(orderHead.OrderNo);
            //if (!OrderHelper.CheckOrderOperationAuthrize(oldOrderHead, user, BusinessConstants.ORDER_OPERATION_EDIT_ORDER))
            //{
            //    throw new BusinessErrorException("Order.Error.NoEditPermission", orderHead.OrderNo);
            //}

            if (oldOrderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                bool isReGenerateOrderLocTrans = false;
                bool isReGenerateOrderOperation = false;


                #region 检查模具号
                if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML && (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                    || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING))
                {
                    CheckFacilityAllocate(orderHead.OrderDetails);
                }
                #endregion

                if (updateDetail
                    && (!EntityHelper.EntityPropertyEquals(oldOrderHead, orderHead, "LocationFrom.Code")
                    || !EntityHelper.EntityPropertyEquals(oldOrderHead, orderHead, "LocationTo.Code")))
                {
                    //订单头的LocationFrom或LocationTo发生变化，重新生成OrderLocTrans
                    isReGenerateOrderLocTrans = true;
                }

                if (!EntityHelper.EntityPropertyEquals(oldOrderHead, orderHead, "Routing.Code"))
                {
                    ////订单头的Routing发生变化，重新生成OrderOperation
                    isReGenerateOrderOperation = true;
                }

                //同步OrderHead，以后都用oldOrderHead操作，不然会造成Session中包含同两个相同对象的错误
                CloneHelper.CopyProperty(orderHead, oldOrderHead, OrderHead2OrderHeadCloneFields); //复制订单的字段
                oldOrderHead.OrderDetails = orderHead.OrderDetails;

                if (isReGenerateOrderLocTrans)
                {
                    //todo
                    //如果订单计划数更改，则orderloctrans的计划用量也随之更改
                    foreach (OrderDetail orderDetail in oldOrderHead.OrderDetails)
                    {


                        List<OrderLocationTransaction> orderLocTrans = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id).ToList();
                        foreach (OrderLocationTransaction targetOrderLocTrans in orderLocTrans)
                        {
                            targetOrderLocTrans.OrderedQty = orderDetail.OrderedQty * targetOrderLocTrans.UnitQty;
                            this.orderLocationTransactionMgrE.UpdateOrderLocationTransaction(targetOrderLocTrans);
                        }
                    }
                }

                if (isReGenerateOrderOperation)
                {
                    //todo
                }

                if (updateDetail)
                {
                    //更新订单明细数量,折扣
                    UpdateOrderQty(oldOrderHead.OrderDetails, user);
                }
                else
                {
                    orderHead.LastModifyDate = DateTime.Now;
                    orderHead.LastModifyUser = user;
                }

                this.orderHeadMgrE.UpdateOrderHead(oldOrderHead);
            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenModify", oldOrderHead.Status, oldOrderHead.OrderNo);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(string flowCode)
        {
            return TransferFlow2Order(flowCode, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(string flowCode, string orderSubType)
        {
            return TransferFlow2Order(flowCode, orderSubType, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(Flow flow)
        {
            return TransferFlow2Order(flow, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(Flow flow, string orderSubType)
        {
            return TransferFlow2Order(flow, orderSubType, false, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(string flowCode, bool isGenerateOrderSubsidiary)
        {
            return TransferFlow2Order(flowCode, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, isGenerateOrderSubsidiary, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(string flowCode, string orderSubType, bool isGenerateOrderSubsidiary)
        {
            return TransferFlow2Order(flowCode, orderSubType, isGenerateOrderSubsidiary, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(Flow flow, bool isGenerateOrderSubsidiary)
        {
            return TransferFlow2Order(flow, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, isGenerateOrderSubsidiary, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(Flow flow, string orderSubType, bool isGenerateOrderSubsidiary)
        {
            return TransferFlow2Order(flow, orderSubType, isGenerateOrderSubsidiary, DateTime.Now);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(string flowCode, bool isGenerateOrderSubsidiary, DateTime startTime)
        {
            return TransferFlow2Order(flowCode, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, isGenerateOrderSubsidiary, startTime);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(string flowCode, string orderSubType, bool isGenerateOrderSubsidiary, DateTime startTime)
        {
            Flow flow = this.flowMgrE.LoadFlow(flowCode);
            return TransferFlow2Order(flow, orderSubType, isGenerateOrderSubsidiary, startTime);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(Flow flow, bool isGenerateOrderSubsidiary, DateTime startTime)
        {
            return TransferFlow2Order(flow, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, isGenerateOrderSubsidiary, startTime);
        }

        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(Flow flow, string orderSubType, bool isGenerateOrderSubsidiary, DateTime startTime)
        {
            return TransferFlow2Order(flow, orderSubType, isGenerateOrderSubsidiary, startTime, false);
        }
        [Transaction(TransactionMode.Unspecified)]
        public OrderHead TransferFlow2Order(Flow flow, string orderSubType, bool isGenerateOrderSubsidiary, DateTime startTime, bool isStartKit)
        {
            #region 创建OrderHead
            OrderHead orderHead = this.TransferFlow2OrderHead(flow, orderSubType, startTime);
            //OrderHead orderHead = new OrderHead();
            //CloneHelper.CopyProperty(flow, orderHead, FlowHead2OrderHeadCloneFields);
            //orderHead.SubType = orderSubType;
            //if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO
            //    && flow.ReturnRouting != null)     //返工，使用ReturnRouting
            //{
            //    orderHead.Routing = flow.ReturnRouting;
            //}
            ////if (orderSubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN
            ////    || orderSubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO)
            ////{
            ////    //退货和次品库位设置
            ////    if (flow.LocationFrom != null && flow.LocationFrom.ActingLocation != null)
            ////    {
            ////        orderHead.LocationFrom = flow.LocationFrom.ActingLocation;
            ////    }

            ////    if (flow.LocationTo != null && flow.LocationTo.ActingLocation != null)
            ////    {
            ////        orderHead.LocationTo = flow.LocationTo.ActingLocation;
            ////    }
            ////}
            ////if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO)
            ////{
            ////    orderHead.LocationTo = this.locationMgrE.GetRejectLocation();
            ////}
            //orderHead.StartTime = startTime;
            //orderHead.Flow = flow.Code;
            //orderHead.SubType = orderSubType;
            //orderHead.Priority = orderHead.Priority == null ? BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL : orderHead.Priority;
            #endregion

            #region 创建OrderDetail
            IList<FlowDetail> flowDetaiList = new List<FlowDetail>();
            if (flow.FlowDetails != null && flow.FlowDetails.Count > 0)
            {
                IListHelper.AddRange<FlowDetail>(flowDetaiList, flow.FlowDetails);
            }

            //根据引用路线创建订单明细
            if (flow.ReferenceFlow != null && flow.ReferenceFlow.Trim() != string.Empty)
            {
                Flow referenceFlow = this.flowMgrE.LoadFlow(flow.ReferenceFlow);
                //if (flow.Routing != null && referenceFlow.Routing != null
                //    && !flow.Routing.Equals(referenceFlow.Routing))
                //{
                //    throw new BusinessErrorException("Flow.Error.ReferenceFlowRoutingNotEqual", flow.Code, referenceFlow.Code);
                //}

                if (referenceFlow.FlowDetails != null && referenceFlow.FlowDetails.Count > 0)
                {
                    IListHelper.AddRange<FlowDetail>(flowDetaiList, referenceFlow.FlowDetails);
                }
            }

            //先根据序号排序，序号在生成订单的时候会重新生成
            flowDetaiList = IListHelper.Sort<FlowDetail>(flowDetaiList, "Sequence");
            foreach (FlowDetail flowDetail in flowDetaiList)
            {
                if (orderHead.Flow == flowDetail.Flow.Code)
                {
                    this.orderDetailMgrE.GenerateOrderDetail(orderHead, flowDetail, false, isStartKit);
                }
                else
                {
                    this.orderDetailMgrE.GenerateOrderDetail(orderHead, flowDetail, true, isStartKit);
                }
            }
            #endregion

            if (isGenerateOrderSubsidiary)
            {
                orderHead.StartTime = startTime;
                this.orderHeadMgrE.GenerateOrderHeadSubsidiary(orderHead);
            }

            return orderHead;
        }

        [Transaction(TransactionMode.Requires)]
        public OrderHead TransferFlow2OrderHead(Flow flow, string orderSubType, DateTime startTime)
        {
            #region 创建OrderHead
            OrderHead orderHead = new OrderHead();
            CloneHelper.CopyProperty(flow, orderHead, FlowHead2OrderHeadCloneFields);
            orderHead.SubType = orderSubType;
            if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO
                && flow.ReturnRouting != null)     //返工，使用ReturnRouting
            {
                orderHead.Routing = flow.ReturnRouting;
            }
            orderHead.StartTime = startTime;
            double leadTime = flow.LeadTime.HasValue ? Convert.ToDouble(flow.LeadTime.Value) : 0;
            orderHead.WindowTime = startTime.AddHours(leadTime);

            orderHead.Flow = flow.Code;
            //orderHead.SubType = orderSubType;
            orderHead.Priority = orderHead.Priority == null ? BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL : orderHead.Priority;
            return orderHead;
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(IList<OrderHead> orderHeadList, string userCode)
        {
            if (orderHeadList != null && orderHeadList.Count > 0)
            {
                foreach (OrderHead orderHead in orderHeadList)
                {
                    this.CreateOrder(orderHead, userCode);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(OrderHead orderHead, string userCode)
        {
            CreateOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(OrderHead orderHead, User user)
        {
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_EDIT_ORDER))
            //{
            //    throw new BusinessErrorException("Order.Error.NoCreatePermission");
            //}

            DateTime dateTimeNow = DateTime.Now;
            //过滤OrderQty数量为0的明细
            OrderHelper.FilterZeroOrderQty(orderHead);
            #region 整包校验,快速的不考虑
            if (!(orderHead.IsAutoRelease && orderHead.IsAutoStart))
            {
                if (orderHead.OrderDetails != null && orderHead.OrderDetails.Count > 0)
                {
                    foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                    {
                        if (orderDetail.OrderHead.FulfillUnitCount && orderDetail.OrderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
                        {
                            if (orderDetail.OrderedQty % orderDetail.UnitCount != 0)
                            {
                                throw new BusinessErrorException("Order.Error.NotFulfillUnitCount", orderDetail.Item.Code);
                            }
                        }
                    }
                }
            }
            #endregion

            //生成OrderLocationTransaction和OrderOperation的记录
            this.orderHeadMgrE.GenerateOrderHeadSubsidiary(orderHead);

            #region 创建OrderHead
            orderHead.OrderNo = numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_ORDER);
            orderHead.CreateUser = user;
            orderHead.CreateDate = dateTimeNow;
            orderHead.LastModifyUser = user;
            orderHead.LastModifyDate = dateTimeNow;
            orderHead.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;

            orderHeadMgrE.CreateOrderHead(orderHead);
            #endregion

            #region 创建OrderDetail
            if (orderHead.OrderDetails != null && orderHead.OrderDetails.Count > 0)
            {
                #region 查询所有的FacilityAllocate
                var facilityAllocatesList = hqlMgr.FindAll<FacilityAllocates>("from FacilityAllocates where IsActive = 1");
                #endregion

                foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                {

                    #region 模具编号记到TextField3字段
                    if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML && (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING))
                    {
                        FacilityAllocates facilityAllocates = facilityAllocatesList.Where(p => p.ItemCode == orderDetail.Item.Code).FirstOrDefault();
                        if (facilityAllocates != null)
                        {
                            orderDetail.TextField3 = facilityAllocates.FCID;
                        }
                    }
                    #endregion
                    CreateOrderDetailSubsidiary(orderDetail);
                }
            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailEmpty");
            }
            #endregion

            #region 创建OrderOP
            if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                this.orderOperationMgrE.GenerateOrderOperation(orderHead);

                if (orderHead.OrderOperations != null && orderHead.OrderOperations.Count > 0)
                {
                    foreach (OrderOperation orderOperation in orderHead.OrderOperations)
                    {
                        this.orderOperationMgrE.CreateOrderOperation(orderOperation);
                    }
                }
            }
            #endregion

            #region 创建OrderBinding
            IList<FlowBinding> flowBindingList = this.flowBindingMgrE.GetFlowBinding(orderHead.Flow);
            if (flowBindingList != null && flowBindingList.Count > 0 && orderHead.IsEnableBinding)
            {
                foreach (FlowBinding flowBinding in flowBindingList)
                {
                    this.orderBindingMgrE.CreateOrderBinding(orderHead, flowBinding.SlaveFlow, flowBinding.BindingType, flowBinding.InTrans);
                }
            }
            #endregion

            #region 判断订单绑定
            this.CreateBindingOrder(orderHead, user, BusinessConstants.CODE_MASTER_BINDING_TYPE_VALUE_CREATE);
            #endregion

            #region 判断自动Release
            if (orderHead.IsAutoRelease)
            {
                //ReleaseOrder(orderHead, user, true);
                ReleaseOrder(orderHead.OrderNo, user, true, orderHead.IsForceRelease);
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(string flowCode, User user, IList<Hu> huList)
        {
            Flow flow = flowMgrE.CheckAndLoadFlow(flowCode);
            CreateOrder(flow, user, huList);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(string flowCode, string userCode, IList<Hu> huList)
        {
            Flow flow = flowMgrE.CheckAndLoadFlow(flowCode);
            CreateOrder(flow, userMgrE.CheckAndLoadUser(userCode), huList);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(Flow flow, string userCode, IList<Hu> huList)
        {
            CreateOrder(flow, userMgrE.CheckAndLoadUser(userCode), huList);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(Flow flow, User user, IList<Hu> huList)
        {

            #region 初始化订单头
            OrderHead orderHead = this.TransferFlow2Order(flow);


            IList<OrderDetail> targetOrderDetailList = orderHead.OrderDetails;

            orderHead.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
            orderHead.SubType = BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO;
            orderHead.WindowTime = DateTime.Now;
            orderHead.StartTime = DateTime.Now;

            orderHead.IsAutoRelease = false;
            orderHead.IsAutoStart = true;
            orderHead.IsAutoShip = false;
            orderHead.IsAutoReceive = false;
            #endregion

            #region 合并OrderDetailList
            if (huList != null && huList.Count > 0)
            {
                IList<OrderDetail> newOrderDetailList = new List<OrderDetail>();
                foreach (Hu hu in huList)
                {
                    bool findMatch = false;

                    #region 在FlowDetail转换的OrderDetail里面查找匹配项
                    foreach (OrderDetail targetOrderDetail in targetOrderDetailList)
                    {
                        if (hu.Item.Code == targetOrderDetail.Item.Code
                            && hu.Uom.Code == targetOrderDetail.Uom.Code)
                        {
                            targetOrderDetail.RequiredQty += hu.Qty;
                            targetOrderDetail.OrderedQty += hu.Qty;
                            findMatch = true;
                            break;
                        }
                    }
                    #endregion

                    if (!findMatch)
                    {
                        #region 没有找到匹配项，从新增匹配项中找
                        foreach (OrderDetail newOrderDetail in newOrderDetailList)
                        {
                            if (hu.Item.Code == newOrderDetail.Item.Code
                            && hu.Uom.Code == newOrderDetail.Uom.Code)
                            {
                                newOrderDetail.RequiredQty += hu.Qty;
                                newOrderDetail.OrderedQty += hu.Qty;

                                findMatch = true;

                                break;
                            }
                        }
                        #endregion

                        if (!findMatch)
                        {
                            #region 还没有找到匹配项,报错
                            throw new BusinessErrorException("OrderDetail.Item.NotInFlow");
                            #endregion
                        }
                    }
                }

                if (newOrderDetailList.Count > 0)
                {
                    #region 合并新增的OrderDetail
                    int seqInterval = int.Parse(this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_SEQ_INTERVAL).Value);
                    int maxSeq = 0;
                    foreach (OrderDetail targetOrderDetail in targetOrderDetailList)
                    {
                        if (targetOrderDetail.Sequence > maxSeq)
                        {
                            maxSeq = targetOrderDetail.Sequence;
                        }
                    }

                    foreach (OrderDetail newOrderDetail in newOrderDetailList)
                    {
                        maxSeq += seqInterval;
                        newOrderDetail.Sequence = maxSeq;

                        orderHead.AddOrderDetail(newOrderDetail);
                    }
                    #endregion
                }
            }

            #endregion

            #region 创建订单
            CreateOrder(orderHead, user);
            #endregion

            #region 更新订单bom数量为负
            foreach (OrderDetail orderDetail in orderHead.OrderDetails)
            {

                IList<OrderLocationTransaction> orderLocTransList = orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT);
                foreach (OrderLocationTransaction ordeLocTrans in orderLocTransList)
                {
                    if (ordeLocTrans.Item.Code == ordeLocTrans.OrderDetail.Item.Code)
                    {
                        continue;
                    }
                    else
                    {
                        ordeLocTrans.OrderedQty = 0 - ordeLocTrans.OrderedQty;
                        orderLocationTransactionMgrE.UpdateOrderLocationTransaction(ordeLocTrans);
                    }
                }
            }

            #endregion

            ReleaseReuseOrder(orderHead, user, huList);
        }

        /// <summary>
        /// 兼容转客户计划转发运单和供应商计划转要货单
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="user"></param>
        /// <param name="planDetails"></param>
        /// <param name="startTime"></param>
        /// <param name="windowTime"></param>
        /// <param name="refOrderNo"></param>
        /*
        [Transaction(TransactionMode.Requires)]
        public void CreateOrder(string flowCode, User user, IList<PlanDetail> planDetails, DateTime startTime, DateTime windowTime, string refOrderNo)
        {
            Flow flow = flowMgrE.LoadFlow(flowCode);
            OrderHead orderHead = this.TransferFlow2OrderHead(flow, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, startTime);
            orderHead.WindowTime = windowTime;

            if (orderHead != null)
            {
                orderHead.OrderDetails = new List<OrderDetail>();
                foreach (PlanDetail planDetail in planDetails)
                {
                    if (planDetail.CurrentQty > 0)
                    {
                        FlowDetail flowDetail = flowDetailMgrE.LoadFlowDetail(planDetail.FlowDetail);
                        OrderDetail orderDetail = orderDetailMgrE.TransferFlowDetail2OrderDetail(flowDetail);
                        orderDetail.OrderedQty = planDetail.CurrentQty;
                        orderDetail.RequiredQty = planDetail.Qty;

                        #region orderTracer
                        OrderTracer orderTracer = new OrderTracer();
                        orderTracer.TracerType = BusinessConstants.ORDERTRACER_TRACERTYPE_LAN;
                        orderTracer.OrderedQty = planDetail.Qty;
                        orderTracer.Qty = planDetail.CurrentQty;
                        orderTracer.Code = planDetail.PeriodType;
                        orderTracer.ReqTime = planDetail.WinTime;
                        orderTracer.Item = orderDetail.Item.Code;
                        orderTracer.OrderDetail = orderDetail;
                        orderTracer.RefId = planDetail.Id;
                        orderDetail.OrderTracers = new List<OrderTracer>() { orderTracer };
                        #endregion

                        orderHead.OrderDetails.Add(orderDetail);
                    }
                    PlanDetail reloadedPlanDetail = planDetailMgr.LoadPlanDetail(planDetail.Id);
                    reloadedPlanDetail.AccumQty = planDetail.CurrentQty + planDetail.AccumQty;
                    planDetailMgr.UpdatePlanDetail(reloadedPlanDetail);
                }
            }
            orderHead.ReferenceOrderNo = refOrderNo;
            this.CreateOrder(orderHead, user);
        }
*/

        [Transaction(TransactionMode.Requires)]
        public void AddOrderDetail(OrderDetail orderDetail, string userCode)
        {
            AddOrderDetail(orderDetail, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void AddOrderDetail(OrderDetail orderDetail, User user)
        {
            OrderHead orderHead = orderHeadMgrE.LoadOrderHead(orderDetail.OrderHead.OrderNo);
            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_EDIT_ORDER_DETAIL))
            //{
            //    throw new BusinessErrorException("OrderDetail.Error.NoEditPermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                if (orderHead.OrderDetails != null && orderHead.OrderDetails.Count > 0)
                {
                    //检验orderDetail序号是否重复
                    orderHead.AddOrderDetail(orderDetail);
                }

                this.orderDetailMgrE.GenerateOrderDetailSubsidiary(orderDetail);

                //新增OrderDetail
                orderDetailMgrE.CreateOrderDetail(orderDetail);

                if (orderDetail.OrderLocationTransactions != null && orderDetail.OrderLocationTransactions.Count > 0)
                {
                    string costGroupFrom = null;
                    if (orderHead.PartyFrom.GetType() == typeof(Region))
                    {
                        costGroupFrom = this.costCenterMgr.CheckAndLoadCostCenter(((Region)orderHead.PartyFrom).CostCenter).CostGroup.Code;
                    }

                    string costGroupTo = null;
                    if (orderHead.PartyTo.GetType() == typeof(Region))
                    {
                        costGroupTo = this.costCenterMgr.CheckAndLoadCostCenter(((Region)orderHead.PartyTo).CostCenter).CostGroup.Code;
                    }


                    //新增OrderLocationTransaction
                    foreach (OrderLocationTransaction orderLocationTransaction in orderDetail.OrderLocationTransactions)
                    {
                        if (orderLocationTransaction.IOType == BusinessConstants.IO_TYPE_IN
                            && costGroupTo != null)
                        {
                            IsLocationInCostGroup(orderLocationTransaction, costGroupTo);

                        }
                        else if (orderLocationTransaction.IOType == BusinessConstants.IO_TYPE_OUT
                            && costGroupFrom != null)
                        {
                            IsLocationInCostGroup(orderLocationTransaction, costGroupFrom);
                        }

                        orderLocationTransactionMgrE.CreateOrderLocationTransaction(orderLocationTransaction);
                    }
                }

                if (orderHead.OrderOperations != null && orderHead.OrderOperations.Count > 0)
                {
                    foreach (OrderOperation orderOperation in orderHead.OrderOperations)
                    {
                        //判断需要新增那些Op
                        if (orderOperation.Id == 0)
                        {
                            this.orderOperationMgrE.CreateOrderOperation(orderOperation);
                        }
                    }
                }

                //更新LastModifyDate、LastModifyUser
                orderHead.LastModifyDate = DateTime.Now;
                orderHead.LastModifyUser = user;
                orderHeadMgrE.UpdateOrderHead(orderHead);
            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenModify", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE, orderDetail.OrderHead.OrderNo);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrderDetail(OrderDetail orderDetail, string userCode)
        {
            UpdateOrderDetail(orderDetail, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrderDetail(OrderDetail orderDetail, User user)
        {
            OrderDetail oldOrderDetail = orderDetailMgrE.LoadOrderDetail(orderDetail.Id);
            //检验权限
            //if (!OrderHelper.CheckOrderOperationAuthrize(oldOrderDetail.OrderHead, user, BusinessConstants.ORDER_OPERATION_EDIT_ORDER_DETAIL))
            //{
            //    throw new BusinessErrorException("OrderDetail.Error.NoEditPermission", oldOrderDetail.OrderHead.OrderNo);
            //}
            orderDetail.Id = 0;
            orderDetail.OrderedQty = oldOrderDetail.OrderedQty;
            orderDetail.RequiredQty = oldOrderDetail.RequiredQty;
            this.DeleteOrderDetail(oldOrderDetail, user);
            this.AddOrderDetail(orderDetail, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateOrderQty(IList<OrderDetail> orderDetailList, string userCode)
        {
            UpdateOrderQty(orderDetailList, this.userMgrE.CheckAndLoadUser(userCode));
        }

        /**
         * 保存订单数量 
         */
        [Transaction(TransactionMode.Requires)]
        public void UpdateOrderQty(IList<OrderDetail> orderDetailList, User user)
        {
            if (orderDetailList != null && orderDetailList.Count > 0)
            {
                IDictionary<string, OrderHead> cachedOrderHead = new Dictionary<string, OrderHead>(); //缓存出现过的OrderHead，一般来说只会有一个

                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    if (!cachedOrderHead.ContainsKey(orderDetail.OrderHead.OrderNo))
                    {
                        OrderHead orderHead = orderHeadMgrE.LoadOrderHead(orderDetail.OrderHead.OrderNo);
                        //检验权限
                        //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_EDIT_ORDER_DETAIL))
                        //{
                        //    throw new BusinessErrorException("OrderDetail.Error.NoEditPermission", orderHead.OrderNo);
                        //}

                        //检验订单是否Create状态
                        if (orderHead.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
                        {
                            throw new BusinessErrorException("Order.Error.StatusErrorWhenModify", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE, orderDetail.OrderHead.OrderNo);
                        }

                        //整包下单检查
                        if (orderHead.FulfillUnitCount)
                        {
                            if (orderDetail.OrderedQty % orderDetail.UnitCount != 0)
                            {
                                throw new BusinessErrorException("Order.Error.NotFulfillUnitCount", orderDetail.Item.Code);
                            }
                        }

                        //缓存
                        cachedOrderHead.Add(orderHead.OrderNo, orderHead);
                    }

                    //更新OrderDetail数量,折扣
                    OrderDetail targetOrderDetail = orderDetailMgrE.LoadOrderDetail(orderDetail.Id);
                    targetOrderDetail.RequiredQty = orderDetail.RequiredQty;
                    targetOrderDetail.OrderedQty = orderDetail.OrderedQty;
                    targetOrderDetail.Discount = orderDetail.Discount;
                    targetOrderDetail.Remark = orderDetail.Remark;
                    orderDetailMgrE.UpdateOrderDetail(targetOrderDetail);

                    if (!(orderDetail.OrderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION))
                    {
                        if (targetOrderDetail.OrderLocationTransactions != null && targetOrderDetail.OrderLocationTransactions.Count > 0)
                        {
                            //更新OrderLocationTransaction数量
                            foreach (OrderLocationTransaction orderLocationTransaction in targetOrderDetail.OrderLocationTransactions)
                            {
                                orderLocationTransaction.OrderedQty = orderDetail.OrderedQty * orderLocationTransaction.UnitQty;
                                orderLocationTransactionMgrE.UpdateOrderLocationTransaction(orderLocationTransaction);
                            }
                        }
                    }
                }

                //更新订单头LastModifyDate、LastModifyUser
                foreach (OrderHead orderHead in cachedOrderHead.Values)
                {
                    orderHead.LastModifyDate = DateTime.Now;
                    orderHead.LastModifyUser = user;
                    orderHeadMgrE.UpdateOrderHead(orderHead);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderDetail(OrderDetail orderDetail, string userCode)
        {
            DeleteOrderDetail(orderDetail.Id, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderDetail(OrderDetail orderDetail, User user)
        {
            DeleteOrderDetail(orderDetail.Id, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderDetail(int orderDetailId, string userCode)
        {
            DeleteOrderDetail(orderDetailId, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderDetail(int orderDetailId, User user)
        {
            OrderDetail orderDetail = orderDetailMgrE.LoadOrderDetail(orderDetailId);
            OrderHead orderHead = orderDetail.OrderHead;

            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_DELETE_ORDER_DETAIL))
            //{
            //    throw new BusinessErrorException("OrderDetail.Error.NoDeletePermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {

                //删除OrderTracer
                if (orderDetail.OrderTracers != null && orderDetail.OrderTracers.Count > 0)
                {
                    foreach (OrderTracer orderTracer in orderDetail.OrderTracers)
                    {
                        this.orderTracerMgrE.DeleteOrderTracer(orderTracer);
                    }
                }

                //删除OrderLocationTransaction
                IList<int> unusedList = new List<int>();
                if (orderDetail.OrderLocationTransactions != null && orderDetail.OrderLocationTransactions.Count > 0)
                {
                    orderLocationTransactionMgrE.DeleteOrderLocationTransaction(orderDetail.OrderLocationTransactions);

                    foreach (OrderLocationTransaction orderLocationTransactions in orderDetail.OrderLocationTransactions)
                    {
                        if (orderLocationTransactions.Operation != 0)
                        {
                            unusedList.Add(orderLocationTransactions.Operation);
                        }
                    }
                }

                //删除OrderDetail
                orderDetailMgrE.DeleteOrderDetail(orderDetailId);

                //更新OrderOp
                if (unusedList.Count > 0)
                {
                    orderHead.RemoveOrderDetailBySequence(orderDetail.Sequence);
                    this.orderOperationMgrE.TryDeleteOrderOperation(orderHead, unusedList);
                }

                //更新LastModifyDate、LastModifyUser
                orderHead.LastModifyDate = DateTime.Now;
                orderHead.LastModifyUser = user;
                orderHeadMgrE.UpdateOrderHead(orderHead);
            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenModify", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE, orderDetail.OrderHead.OrderNo);
            }
        }



        [Transaction(TransactionMode.Requires)]
        public void AddOrderLocationTransaction(OrderLocationTransaction orderLocationTransaction, string userCode)
        {
            AddOrderLocationTransaction(orderLocationTransaction, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void AddOrderLocationTransaction(OrderLocationTransaction orderLocationTransaction, User user)
        {

            OrderDetail orderDetail = orderDetailMgrE.LoadOrderDetail(orderLocationTransaction.OrderDetail.Id);
            OrderHead orderHead = orderDetail.OrderHead;

            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_EDIT_ORDER_DETAIL))
            //{
            //    throw new BusinessErrorException("OrderDetail.Error.NoEditPermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                if (orderLocationTransaction.IOType == BusinessConstants.IO_TYPE_IN
                            && orderHead.PartyTo.GetType() == typeof(Region))
                {
                    string costGroupTo = this.costCenterMgr.CheckAndLoadCostCenter(((Region)orderHead.PartyTo).CostCenter).CostGroup.Code;
                    IsLocationInCostGroup(orderLocationTransaction, costGroupTo);

                }
                else if (orderLocationTransaction.IOType == BusinessConstants.IO_TYPE_OUT
                    && orderHead.PartyFrom.GetType() == typeof(Region))
                {
                    string costGroupFrom = this.costCenterMgr.CheckAndLoadCostCenter(((Region)orderHead.PartyFrom).CostCenter).CostGroup.Code;
                    IsLocationInCostGroup(orderLocationTransaction, costGroupFrom);
                }

                //添加OrderLocationTransaction
                IList<int> unusedList = new List<int>();
                orderLocationTransactionMgrE.CreateOrderLocationTransaction(orderLocationTransaction);
                orderDetail.AddOrderLocationTransaction(orderLocationTransaction);

                //更新OrderOp
                if (orderHead.Type != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    this.orderOperationMgrE.TryAddOrderOperation(orderHead, orderLocationTransaction.Operation, null);
                }

                //更新LastModifyDate、LastModifyUser
                orderHead.LastModifyDate = DateTime.Now;
                orderHead.LastModifyUser = user;
                orderHeadMgrE.UpdateOrderHead(orderHead);

            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenModify", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE, orderDetail.OrderHead.OrderNo);
            }
        }




        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderLocationTransaction(OrderLocationTransaction orderLocationTransaction, string userCode)
        {
            DeleteOrderLocationTransaction(orderLocationTransaction.Id, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderLocationTransaction(OrderLocationTransaction orderLocationTransaction, User user)
        {
            DeleteOrderLocationTransaction(orderLocationTransaction.Id, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderLocationTransaction(int orderLocationTransactionId, string userCode)
        {
            DeleteOrderLocationTransaction(orderLocationTransactionId, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrderLocationTransaction(int orderLocationTransactionId, User user)
        {
            OrderLocationTransaction orderLocationTransaction = orderLocationTransactionMgrE.LoadOrderLocationTransaction(orderLocationTransactionId);
            OrderDetail orderDetail = orderDetailMgrE.LoadOrderDetail(orderLocationTransaction.OrderDetail.Id);
            OrderHead orderHead = orderDetail.OrderHead;

            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_DELETE_ORDER_DETAIL))
            //{
            //    throw new BusinessErrorException("OrderDetail.Error.NoDeletePermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {


                //删除OrderLocationTransaction
                IList<int> unusedList = new List<int>();
                orderLocationTransactionMgrE.DeleteOrderLocationTransaction(orderLocationTransactionId);
                orderDetail.RemoveOrderLocationTransaction(orderLocationTransaction);
                if (orderLocationTransaction.Operation != 0)
                {
                    unusedList.Add(orderLocationTransaction.Operation);
                }


                //更新OrderOp
                if (unusedList.Count > 0)
                {
                    this.orderOperationMgrE.TryDeleteOrderOperation(orderHead, unusedList);
                }

                //更新LastModifyDate、LastModifyUser
                orderHead.LastModifyDate = DateTime.Now;
                orderHead.LastModifyUser = user;
                orderHeadMgrE.UpdateOrderHead(orderHead);

            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenModify", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE, orderDetail.OrderHead.OrderNo);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrder(OrderHead orderHead, string userCode)
        {
            DeleteOrder(orderHead.OrderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrder(OrderHead orderHead, User user)
        {
            DeleteOrder(orderHead.OrderNo, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrder(string orderNo, string userCode)
        {
            DeleteOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteOrder(string orderNo, User user)
        {
            OrderHead orderHead = orderHeadMgrE.CheckAndLoadOrderHead(orderNo);

            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_DELETE_ORDER))
            //{
            //    throw new BusinessErrorException("Order.Error.NoDeletePermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                if (orderHead.OrderDetails != null && orderHead.OrderDetails.Count > 0)
                {
                    foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                    {
                        //删除OrderTracer
                        if (orderDetail.OrderTracers != null && orderDetail.OrderTracers.Count > 0)
                        {
                            foreach (OrderTracer orderTracer in orderDetail.OrderTracers)
                            {
                                this.orderTracerMgrE.DeleteOrderTracer(orderTracer);
                            }
                        }

                        //删除OrderLocationTransaction
                        if (orderDetail.OrderLocationTransactions != null && orderDetail.OrderLocationTransactions.Count > 0)
                        {
                            orderLocationTransactionMgrE.DeleteOrderLocationTransaction(orderDetail.OrderLocationTransactions);
                        }
                    }

                    //删除OrderDetail
                    orderDetailMgrE.DeleteOrderDetail(orderHead.OrderDetails);
                }

                if (orderHead.OrderOperations != null && orderHead.OrderOperations.Count > 0)
                {
                    //删除OrderOperation
                    orderOperationMgrE.DeleteOrderOperation(orderHead.OrderOperations);
                }
                if (orderHead.OrderBindings != null && orderHead.OrderBindings.Count > 0)
                {
                    //删除OrderBinding
                    orderBindingMgrE.DeleteOrderBinding(orderHead.OrderBindings);
                }

                //删除OrderHead
                orderHeadMgrE.DeleteOrderHead(orderNo);
            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenDelete", orderHead.Status, orderNo);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(OrderHead orderHead, string userCode)
        {
            ReleaseOrder(orderHead.OrderNo, this.userMgrE.CheckAndLoadUser(userCode), false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(OrderHead orderHead, User user)
        {
            ReleaseOrder(orderHead.OrderNo, user, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(string orderNo, string userCode)
        {
            ReleaseOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode), false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(string orderNo, User user)
        {
            ReleaseOrder(orderNo, user, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(OrderHead orderHead, string userCode, bool autoHandleAbstractItem)
        {
            ReleaseOrder(orderHead.OrderNo, this.userMgrE.CheckAndLoadUser(userCode), autoHandleAbstractItem);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(OrderHead orderHead, User user, bool autoHandleAbstractItem)
        {
            ReleaseOrder(orderHead.OrderNo, user, autoHandleAbstractItem);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(string orderNo, string userCode, bool autoHandleAbstractItem)
        {
            ReleaseOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode), autoHandleAbstractItem);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(string orderNo, User user, bool autoHandleAbstractItem)
        {
            ReleaseOrder(orderNo, user, autoHandleAbstractItem, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(string orderNo, User user, bool autoHandleAbstractItem, bool isForce)
        {
            ReleaseOrder(orderNo, user, autoHandleAbstractItem, false, false);
        }

        [Transaction(TransactionMode.Requires)]
        public void ReleaseOrder(string orderNo, User user, bool autoHandleAbstractItem, bool isForce, bool isJumpFacilityCheck)
        {
            OrderHead orderHead = orderHeadMgrE.LoadOrderHead(orderNo);
            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_SUBMIT_ORDER))
            //{
            //    throw new BusinessErrorException("Order.Error.NoReleasePermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                #region 检查设备是否需要保养

                if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML && (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING))
                {
                    IList<FacilityMasters> facilityMastersList = hqlMgr.FindAll<FacilityMasters>("  from FacilityMasters where ParentCategory = ? and Status = ?", new object[] { "YH_MJ", BusinessConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE });
                    IList<FacilityAllocates> facilityAllocatesList = hqlMgr.FindAll<FacilityAllocates>("  from FacilityAllocates where IsActive = ?", new object[] { true });
                    IList<FacilityMasters> useFacilityMastersList = new List<FacilityMasters>();
                    foreach (OrderDetail od in orderHead.OrderDetails)
                    {
                        if (!string.IsNullOrEmpty(od.TextField3))
                        {
                            FacilityMasters facility = facilityMastersList.Where(p => p.FCID == od.TextField3).FirstOrDefault();
                            FacilityAllocates facilityAllocates = facilityAllocatesList.Where(p => p.FCID == od.TextField3 && p.ItemCode == od.Item.Code).FirstOrDefault();
                            if (facility == null)
                            {
                                throw new BusinessErrorException("Order.Error.FacilityStatusNotAvaliable", od.TextField3);
                            }
                            if (!isJumpFacilityCheck && (facility.UseQty + od.OrderedQty / facilityAllocates.MouldCount) > facility.NextMaintainQty)
                            {
                                throw new BusinessErrorException("Order.Error.FacilityUseQtyError", od.TextField3, facility.UseQty.ToString("0.##"), od.OrderedQty.ToString("0.##"), facility.NextMaintainQty.ToString("0.##"));
                            }
                            useFacilityMastersList.Add(facility);

                        }
                    }
                    foreach (FacilityMasters f in useFacilityMastersList)
                    {
                        f.Status = BusinessConstants.CODE_MASTER_FACILITY_STATUS_INUSE;
                        f.LastModifyDate = DateTime.Now;
                        f.LastModifyUser = user.Code;
                        hqlMgr.Update(f);
                    }
                }
                #endregion

                //this.AllocateOrderHeadDicount(orderHead);
                if (!isForce && orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML
                        && (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                        || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT
                        || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING))
                {
                    //判断订单量是否超过了需求量
                    //订单量 <= (需求-订单待收) * (1+余量)

                    //需求
                    var planTrans = hqlMgr.FindAll<MrpPlanTransaction>(" from MrpPlanTransaction order by Id desc", 0, 1);
                    var effectiveDate = planTrans[0].EffectiveDate;

                    DetachedCriteria criteria = DetachedCriteria.For(typeof(MrpShipPlanView));
                    if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING)
                    {
                        //criteria.Add(Expression.Eq("Flow", orderHead.));
                        var flow = this.flowMgrE.LoadFlow(orderHead.Flow, false, false);
                        criteria.Add(Expression.Eq("Flow", flow.ReferenceFlow));
                    }
                    else
                    {
                        criteria.Add(Expression.Eq("Flow", orderHead.Flow));
                    }
                    criteria.Add(Expression.Eq("EffectiveDate", effectiveDate));
                    IList<MrpShipPlanView> mrpShipPlanViewList = criteriaMgrE.FindAll<MrpShipPlanView>(criteria);

                    //待收
                    #region 订单待收
                    //老的待收
                    criteria = DetachedCriteria.For(typeof(ExpectTransitInventory));
                    criteria.Add(Expression.Eq("Flow", orderHead.Flow));
                    criteria.Add(Expression.Eq("EffectiveDate", effectiveDate));
                    var oldOrderList = criteriaMgrE.FindAll<ExpectTransitInventory>(criteria);

                    //新的待收
                    string hql = @"select oh.OrderNo, oh.Type, oh.Flow, olt.Location.Code, olt.Item.Code, olt.Uom.Code, od.UnitCount, 
                                       oh.StartTime, oh.WindowTime, od.OrderedQty, od.ShippedQty, od.ReceivedQty, olt.UnitQty,oh.Status
                                        from OrderLocationTransaction as olt 
                                        join olt.OrderDetail as od
                                        join od.OrderHead as oh
                                   where oh.Status in (?,?,?,?) and oh.SubType = ? and oh.Flow = ? and olt.IOType = ? and oh.ReleaseDate>? ";

                    IList<object[]> expectTransitInvList = hqlMgr.FindAll<object[]>(hql,
                        new Object[] {
                            BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT, 
                            BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS, 
                            BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE,
                            BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE,
                            BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, 
                            orderHead.Flow,
                            BusinessConstants.IO_TYPE_IN,
                            effectiveDate
                        });

                    var newOrderList = from inv in expectTransitInvList ?? new List<object[]>()
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
                                           TransitQty = ((string)inv[13] == BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE || (string)inv[13] == BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE) ? (((decimal?)inv[11]).HasValue ? ((decimal?)inv[11]).Value : 0M) : (decimal)inv[9],
                                           EffectiveDate = effectiveDate
                                       };
                    #endregion
                    var entityPreference = this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_OVERORDERRATE);
                    decimal overOrderRate = 0;
                    if (entityPreference != null)
                    {
                        decimal.TryParse(entityPreference.Value, out overOrderRate);
                    }
                    string errorMessage = string.Empty;
                    foreach (var orderDetail in orderHead.OrderDetails)
                    {
                        var demandQty = mrpShipPlanViewList.Where(p => p.Item == orderDetail.Item.Code).Sum(p => p.Qty);
                        var oldOrderQty = oldOrderList.Where(p => p.Item == orderDetail.Item.Code).Sum(p => p.TransitQty);
                        var newOrderQty = newOrderList.Where(p => p.Item == orderDetail.Item.Code).Sum(p => p.TransitQty);
                        if ((demandQty - oldOrderQty - newOrderQty) * (1 + overOrderRate) < orderDetail.OrderedQty)
                        {
                            errorMessage += string.Format("物料{0}超出需求量", orderDetail.Item.Description);
                        }
                    }
                    if (errorMessage != string.Empty)
                    {
                        throw new BusinessErrorException(errorMessage);
                    }
                }

                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    #region 处理抽象件
                    string unHandledAbstractItemIds = string.Empty;
                    foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                    {
                        IList<OrderLocationTransaction> abstractOrderLocationTransactionList = new List<OrderLocationTransaction>();
                        foreach (OrderLocationTransaction orderLocationTransaction in orderDetail.OrderLocationTransactions)
                        {
                            if (orderLocationTransaction.Item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_A)
                            {
                                abstractOrderLocationTransactionList.Add(orderLocationTransaction);
                            }
                        }

                        foreach (OrderLocationTransaction orderLocationTransaction in abstractOrderLocationTransactionList)
                        {
                            if (autoHandleAbstractItem)
                            {
                                //替换抽象零件
                                this.orderLocationTransactionMgrE.AutoReplaceAbstractItem(orderLocationTransaction);
                            }
                            else
                            {
                                if (unHandledAbstractItemIds != string.Empty)
                                {
                                    unHandledAbstractItemIds = orderLocationTransaction.Item.Code;
                                }
                                else
                                {
                                    unHandledAbstractItemIds = " ," + orderLocationTransaction.Item.Code;
                                }
                            }
                        }
                    }

                    if (unHandledAbstractItemIds != string.Empty)
                    {
                        throw new BusinessErrorException("Order.Warning.UnhandleAbstractItem", unHandledAbstractItemIds);
                    }
                    #endregion

                    #region 处理选装件
                    IList<int> unusedOpList = new List<int>();
                    IList<OrderLocationTransaction> deleteOrderLocationTransactionList = new List<OrderLocationTransaction>();
                    foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                    {
                        IList<OrderLocationTransaction> optionalOrderLocationTransactionList = new List<OrderLocationTransaction>();
                        foreach (OrderLocationTransaction orderLocationTransaction in orderDetail.OrderLocationTransactions)
                        {
                            if (orderLocationTransaction.BomDetail != null
                                && orderLocationTransaction.BomDetail.StructureType == BusinessConstants.CODE_MASTER_BOM_DETAIL_TYPE_VALUE_O
                                && !orderLocationTransaction.IsAssemble)
                            {
                                optionalOrderLocationTransactionList.Add(orderLocationTransaction);
                            }
                        }

                        foreach (OrderLocationTransaction orderLocationTransaction in optionalOrderLocationTransactionList)
                        {
                            //选装件没有安装，删除orderLocationTransaction;
                            deleteOrderLocationTransactionList.Add(orderLocationTransaction);
                            orderDetail.RemoveOrderLocationTransaction(orderLocationTransaction);
                            if (orderLocationTransaction.Operation != 0)
                            {
                                unusedOpList.Add(orderLocationTransaction.Operation);
                            }
                        }
                    }

                    this.orderLocationTransactionMgrE.DeleteOrderLocationTransaction(deleteOrderLocationTransactionList);
                    this.orderOperationMgrE.TryDeleteOrderOperation(orderHead, unusedOpList);  //删除orderLocationTransaction对应的Op
                    #endregion
                }

                DateTime nowDate = DateTime.Now;
                orderHead.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT;
                orderHead.ReleaseDate = nowDate;
                orderHead.ReleaseUser = user;
                orderHead.LastModifyDate = nowDate;
                orderHead.LastModifyUser = user;

                this.orderHeadMgrE.UpdateOrderHead(orderHead);
                if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
                {
                    SendEmail(orderHead, user);
                }

                #region 判断自动Start
                if (orderHead.IsAutoStart)
                {
                    StartOrder(orderHead, user);
                }
                #endregion

                #region 处理路线绑定
                this.CreateBindingOrder(orderHead, user, BusinessConstants.CODE_MASTER_BINDING_TYPE_VALUE_SUBMIT);
                #endregion

            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenSubmit", orderHead.Status, orderNo);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void StartOrder(OrderHead orderHead, string userCode)
        {
            StartOrder(orderHead.OrderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void StartOrder(string orderNo, string userCode)
        {
            StartOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void StartOrder(OrderHead orderHead, User user)
        {
            StartOrder(orderHead.OrderNo, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void StartOrder(string orderNo, User user)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            Flow flow = this.flowMgrE.LoadFlow(orderHead.Flow);

            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_START_ORDER))
            //{
            //    throw new BusinessErrorException("Order.Error.NoStartPermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
            {
                #region 检查生产单最大上线数量
                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                    && orderHead.Flow != null && flow.MaxOnlineQty > 0
                    && this.GetInPorcessWOCount(orderHead.Flow, user) >= flow.MaxOnlineQty)
                {
                    throw new BusinessErrorException("Order.Error.ExcceedMaxOnlineQty");
                }
                #endregion

                DateTime nowDate = DateTime.Now;
                orderHead.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS;
                orderHead.StartDate = nowDate;
                orderHead.StartUser = user;
                orderHead.LastModifyDate = nowDate;
                orderHead.LastModifyUser = user;

                this.orderHeadMgrE.UpdateOrderHead(orderHead);

                #region 判断自动PickList、Ship
                if (orderHead.IsAutoShip && orderHead.IsAutoReceive
                    && orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    if (orderHead.CompleteLatency.HasValue && orderHead.CompleteLatency.Value > 0)
                    {
                        //todo 收货延迟，记录到Quratz表中
                        throw new NotImplementedException("Complete Latency Not Implemented");
                    }
                    else
                    {
                        //立即收货
                        Receipt receipt = new Receipt();
                        foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                        {
                            ReceiptDetail receiptDetail = new ReceiptDetail();
                            receiptDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_IN)[0];
                            receiptDetail.HuId = orderDetail.HuId;
                            receiptDetail.ReceivedQty = orderDetail.OrderedQty;
                            receiptDetail.Receipt = receipt;

                            #region 生产自动收货，找Out的OrderLocTrans，填充MaterialFulshBack
                            if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                            {
                                IList<OrderLocationTransaction> orderLocTransList = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT);
                                foreach (OrderLocationTransaction orderLocTrans in orderLocTransList)
                                {
                                    MaterialFlushBack material = new MaterialFlushBack();
                                    material.OrderLocationTransaction = orderLocTrans;
                                    if (orderLocTrans.UnitQty != 0)
                                    {
                                        material.Qty = orderLocTrans.OrderedQty / orderLocTrans.UnitQty;
                                    }
                                    receiptDetail.AddMaterialFlushBack(material);
                                }
                            }
                            #endregion
                            receipt.AddReceiptDetail(receiptDetail);
                        }

                        ReceiveOrder(receipt, user);
                    }
                }
                else if (orderHead.IsAutoShip)
                {
                    if (orderHead.StartLatency.HasValue && orderHead.StartLatency.Value > 0)
                    {
                        //todo 上线延迟，记录到Quratz表中
                        throw new NotImplementedException("Start Latency Not Implemented");
                    }
                    else
                    {
                        //立即上线
                        IList<InProcessLocationDetail> inProcessLocationDetailList = new List<InProcessLocationDetail>();
                        foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                        {
                            InProcessLocationDetail inProcessLocationDetail = new InProcessLocationDetail();
                            inProcessLocationDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT)[0];
                            inProcessLocationDetail.Qty = orderDetail.OrderedQty;

                            inProcessLocationDetailList.Add(inProcessLocationDetail);
                        }

                        ShipOrder(inProcessLocationDetailList, user);
                    }
                }
                else if (orderHead.IsAutoCreatePickList
                    && orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)  //过滤掉退货和调整
                {
                    IList<OrderLocationTransaction> orderLocationTransactionList = new List<OrderLocationTransaction>();
                    foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                    {
                        IList<OrderLocationTransaction> outOrderLocationTransactionList = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT);
                        foreach (OrderLocationTransaction orderLocationTransaction in outOrderLocationTransactionList)
                        {
                            orderLocationTransaction.CurrentShipQty = orderLocationTransaction.OrderedQty;
                        }
                        IListHelper.AddRange<OrderLocationTransaction>(orderLocationTransactionList, outOrderLocationTransactionList);
                    }

                    this.pickListMgrE.CreatePickList(orderLocationTransactionList, user);
                }
                #endregion
            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenStart", orderHead.Status, orderNo);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseScrapOrder(string orderNo, string userCode)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            return ReleaseScrapOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseScrapOrder(OrderHead orderHead, string userCode)
        {
            return ReleaseScrapOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseScrapOrder(string orderNo, User currentUser)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            return ReleaseScrapOrder(orderHead, currentUser);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseScrapOrder(OrderHead orderHead, User currentUser)
        {
            ReleaseOrder(orderHead, currentUser);
            Receipt receipt = ReceiveScrapOrder(orderHead, currentUser);
            ManualCompleteOrder(orderHead, currentUser);
            return receipt;
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseReuseOrder(string orderNo, string userCode, IList<Hu> huList)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            return ReleaseReuseOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode), huList);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseReuseOrder(OrderHead orderHead, string userCode, IList<Hu> huList)
        {
            return ReleaseReuseOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode), huList);
        }


        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseReuseOrder(string orderNo, User currentUser, IList<Hu> huList)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            return ReleaseReuseOrder(orderHead, currentUser, huList);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReleaseReuseOrder(OrderHead orderHead, User currentUser, IList<Hu> huList)
        {
            if (huList.Count == 0)
            {
                throw new BusinessErrorException("Hu.Error.DetailEmpty");
            }

            #region 更新成品的库位为条码的库位
            Location location = null;
            foreach (Hu hu in huList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailMgrE.GetHuLocationLotDetail(hu.HuId)[0];
                if (location == null)
                {
                    location = locationLotDetail.Location;
                }
                if (location != locationLotDetail.Location)
                {
                    throw new BusinessErrorException("Hu.Error.Location.NotEqual");
                }
            }


            foreach (OrderDetail orderDetail in orderHead.OrderDetails)
            {
                IList<OrderLocationTransaction> orderLocTransList = orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT);
                foreach (OrderLocationTransaction orderLoctrans in orderLocTransList)
                {
                    if (orderLoctrans.Item.Code == orderDetail.Item.Code && orderLoctrans.TransactionType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_WO)
                    {
                        orderLoctrans.Location = location;
                        orderLocationTransactionMgrE.UpdateOrderLocationTransaction(orderLoctrans);
                    }
                }
            }
            #endregion

            ReleaseOrder(orderHead, currentUser);
            Receipt receipt = ReceiveReuseOrder(orderHead, currentUser, huList);
            ManualCompleteOrder(orderHead, currentUser);
            return receipt;
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveScrapOrder(string orderNo, string userCode)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);

            return ReceiveScrapOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveScrapOrder(string orderNo, User user)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            return ReceiveScrapOrder(orderHead, user);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveScrapOrder(OrderHead orderHead, string userCode)
        {
            return ReceiveScrapOrder(orderHead, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveScrapOrder(OrderHead orderHead, User user)
        {
            IList<ReceiptDetail> receiptDetailList = new List<ReceiptDetail>();
            foreach (OrderDetail orderDetail in orderHead.OrderDetails)
            {
                ReceiptDetail receiptDetail = new ReceiptDetail();
                receiptDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_IN)[0];
                receiptDetail.HuId = orderDetail.HuId;
                receiptDetail.ScrapQty = orderDetail.OrderedQty;

                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    IList<OrderLocationTransaction> orderLocTransList = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT);
                    foreach (OrderLocationTransaction orderLocTrans in orderLocTransList)
                    {
                        MaterialFlushBack material = new MaterialFlushBack();
                        material.OrderLocationTransaction = orderLocTrans;

                        if (orderLocTrans.UnitQty != 0)
                        {
                            material.Qty = orderLocTrans.OrderedQty / orderLocTrans.UnitQty;
                        }
                        receiptDetail.AddMaterialFlushBack(material);
                    }
                }
                receiptDetailList.Add(receiptDetail);
            }


            return ReceiveOrder(receiptDetailList, user, null, null, null, true, false);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveReuseOrder(string orderNo, User user, IList<Hu> huList)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);
            return ReceiveReuseOrder(orderHead, user, huList);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveReuseOrder(OrderHead orderHead, User user, IList<Hu> huList)
        {
            foreach (OrderDetail orderDetail in orderHead.OrderDetails)
            {
                decimal qty = orderDetail.OrderedQty;
                foreach (Hu hu in huList)
                {
                    if (orderDetail.Item.Code == hu.Item.Code && orderDetail.Uom.Code == hu.Uom.Code)
                    {
                        qty = qty - hu.Qty;
                    }
                }
                if (qty != 0)
                {
                    throw new BusinessErrorException("OrderDetail.Item.Qty.NotMatch", orderDetail.Item.Code);
                }

            }

            IList<ReceiptDetail> receiptDetailList = new List<ReceiptDetail>();
            foreach (OrderDetail orderDetail in orderHead.OrderDetails)
            {
                ReceiptDetail receiptDetail = new ReceiptDetail();
                receiptDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_IN)[0];
                receiptDetail.HuId = orderDetail.HuId;
                receiptDetail.ScrapQty = orderDetail.OrderedQty;

                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    IList<OrderLocationTransaction> orderLocTransList = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT);
                    foreach (OrderLocationTransaction orderLocTrans in orderLocTransList)
                    {

                        MaterialFlushBack material = new MaterialFlushBack();
                        material.OrderLocationTransaction = orderLocTrans;
                        if (orderLocTrans.OrderDetail.Item.Code == orderLocTrans.Item.Code
                            && orderLocTrans.TransactionType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_WO)
                        {
                            foreach (Hu hu in huList)
                            {
                                if (hu.Item.Code == orderLocTrans.Item.Code && hu.Uom.Code == orderLocTrans.OrderDetail.Uom.Code)
                                {
                                    material = new MaterialFlushBack();
                                    material.OrderLocationTransaction = orderLocTrans;
                                    material.HuId = hu.HuId;
                                    if (orderLocTrans.UnitQty != 0)
                                    {
                                        material.Qty = hu.Qty;
                                    }
                                    receiptDetail.AddMaterialFlushBack(material);
                                }
                            }
                        }
                        else
                        {
                            if (orderLocTrans.UnitQty != 0)
                            {
                                material.Qty = orderLocTrans.OrderedQty / orderLocTrans.UnitQty;
                            }
                            receiptDetail.AddMaterialFlushBack(material);
                        }
                    }
                }
                receiptDetailList.Add(receiptDetail);
            }


            return ReceiveOrder(receiptDetailList, user, null, null, null, true, false);
        }


        [Transaction(TransactionMode.Requires)]
        public void CancelOrder(OrderHead orderHead, string userCode)
        {
            CancelOrder(orderHead.OrderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelOrder(OrderHead orderHead, User user)
        {
            CancelOrder(orderHead.OrderNo, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelOrder(string orderNo, string userCode)
        {
            CancelOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelOrder(string orderNo, User user)
        {
            OrderHead orderHead = orderHeadMgrE.LoadOrderHead(orderNo);
            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_CANCEL_ORDER))
            //{
            //    throw new BusinessErrorException("Order.Error.NoCancelPermission", orderHead.OrderNo);
            //}
            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS
              && orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                var i = (
                     from d in orderHead.OrderDetails
                     where d.ReceivedQty != null
                     select d).Count();
                if (i > 0)
                {
                    throw new BusinessErrorException("Order.Error.ReceivedQtyNotZero", orderHead.Status, orderNo);
                }
            }

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT || orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS)
            {
                DateTime nowDate = DateTime.Now;
                orderHead.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CANCEL;
                orderHead.CancelDate = nowDate;
                orderHead.CancelUser = user;
                orderHead.LastModifyDate = nowDate;
                orderHead.LastModifyUser = user;

                this.orderHeadMgrE.UpdateOrderHead(orderHead);
            }
            else
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenCancel", orderHead.Status, orderNo);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCompleteOrder(string[] flowCodeArray)
        {
            DateTime nowDate = DateTime.Now;
            foreach (string flowCode in flowCodeArray)
            {
                DetachedCriteria criteria = DetachedCriteria.For<OrderHead>();

                criteria.Add(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS));
                criteria.Add(Expression.Eq("Flow", flowCode));
                criteria.Add(Expression.Lt("WindowTime", nowDate));
                IList<OrderHead> orderHeadList = this.criteriaMgrE.FindAll<OrderHead>(criteria);

                foreach (OrderHead orderHead in orderHeadList)
                {
                    if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                        || (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER && orderHead.WindowTime.AddMinutes(30) < nowDate))
                    {
                        ManualCompleteOrder(orderHead, userMgrE.LoadUser(BusinessConstants.SYSTEM_USER_MONITOR));
                    }
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCompleteWoOrder(string[] flowCodeArray)
        {
            DateTime nowDate = DateTime.Now;
            DetachedCriteria criteria = DetachedCriteria.For<OrderHead>();

            criteria.Add(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS));
            criteria.Add(Expression.Eq("Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION));
            if (flowCodeArray != null && flowCodeArray.Length > 0)
            {
                criteria.Add(Expression.In("Flow", flowCodeArray));
            }
            criteria.Add(Expression.Lt("WindowTime", nowDate.AddDays(2)));


            IList<OrderHead> orderHeadList = this.criteriaMgrE.FindAll<OrderHead>(criteria);

            foreach (OrderHead orderHead in orderHeadList)
            {
                var i = (
                        from d in orderHead.OrderDetails
                        where
                            d.ReceivedQty == null || d.ReceivedQty < d.OrderedQty
                        select d).Count();
                if (i == 0)
                {
                    ManualCompleteOrder(orderHead, userMgrE.LoadUser(BusinessConstants.SYSTEM_USER_MONITOR));
                }
            }

        }


        [Transaction(TransactionMode.Requires)]
        public void TryCloseOrder()
        {
            IList<OrderHead> orderHeadList = orderHeadMgrE.GetOrderHead(BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE);
            foreach (OrderHead orderHead in orderHeadList)
            {
                TryCloseOrder(orderHead, userMgrE.LoadUser(BusinessConstants.SYSTEM_USER_MONITOR));
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCloseOrder(OrderHead orderHead, string userCode)
        {
            TryCloseOrder(orderHead.OrderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCloseOrder(OrderHead orderHead, User user)
        {
            TryCloseOrder(orderHead.OrderNo, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCloseOrder(string orderNo, string userCode)
        {
            TryCloseOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCloseOrder(string orderNo, User user)
        {
            OrderHead orderHead = orderHeadMgrE.LoadOrderHead(orderNo);

            //权限校验
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_CLOSE_ORDER))
            //{
            //    return;
            //    //throw new BusinessErrorException("Order.Error.NoClosePermission", orderHead.OrderNo);
            //}

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE)
            {
                #region 存在未关闭ASN，不可以关闭
                IList<InProcessLocationDetail> ipLocDetailList = inProcessLocationDetailMgrE.GetInProcessLocationDetail(orderHead, BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE);
                if (ipLocDetailList != null && ipLocDetailList.Count > 0)
                {
                    return;
                }
                #endregion

                #region 存在PlanBill未结算，不可以关闭
                IList<PlannedBill> plannedBillList = this.plannedBillMgrE.GetUnSettledPlannedBill(orderNo);
                if (plannedBillList != null && plannedBillList.Count > 0)
                {
                    return;
                }
                #endregion

                #region 存在ActingBill未开票，不可以关闭
                IList<ActingBill> actingBillList = this.actingBillMgrE.GetUnBilledActingBill(orderNo);
                if (actingBillList != null && actingBillList.Count > 0)
                {
                    return;
                }
                #endregion

                #region 存在BillDetail未关闭，不可以关闭
                DetachedCriteria criteria = DetachedCriteria.For<BillDetail>();
                criteria.CreateAlias("Bill", "b");
                if (actingBillList.Count == 1)
                {
                    criteria.Add(Expression.Eq("ActingBill", actingBillList[0]));
                }
                else
                {
                    criteria.Add(Expression.InG("ActingBill", actingBillList));
                }
                criteria.Add(Expression.Or(
                    Expression.Not(Expression.Eq("b.Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE)),
                    Expression.Not(Expression.Eq("b.Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_VOID))
                    ));
                IList<BillDetail> billDetailList = this.criteriaMgrE.FindAll<BillDetail>(criteria);

                if (billDetailList != null && billDetailList.Count > 0)
                {
                    return;
                }
                #endregion

                #region 生产有投料未回冲，不能关闭
                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    DetachedCriteria criteria2 = DetachedCriteria.For<OrderPlannedBackflush>()
                        .SetProjection(Projections.Count("Id"));

                    criteria2.CreateAlias("OrderLocationTransaction", "olt");
                    criteria2.CreateAlias("olt.OrderDetail", "od");
                    criteria2.CreateAlias("od.OrderHead", "oh");

                    criteria2.Add(Expression.Eq("IsActive", true));
                    criteria2.Add(Expression.Eq("oh.OrderNo", orderNo));

                    IList list = criteriaMgrE.FindAll(criteria2);
                    if (int.Parse(list[0].ToString()) > 0)
                    {
                        return;
                    }
                }
                #endregion

                DateTime nowDate = DateTime.Now;
                orderHead.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
                orderHead.CloseDate = nowDate;
                orderHead.CloseUser = user;
                orderHead.LastModifyDate = nowDate;
                orderHead.LastModifyUser = user;

                this.orderHeadMgrE.UpdateOrderHead(orderHead);
            }
            else
            {
                return;
                //throw new BusinessErrorException("Order.Error.StatusErrorWhenClose", orderHead.Status, orderNo);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(IList<OrderDetail> orderDetailList, string userCode)
        {
            return ShipOrder(orderDetailList, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(IList<OrderDetail> orderDetailList, User user)
        {
            InProcessLocation inProcessLocation = new InProcessLocation();
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                InProcessLocationDetail inProcessLocationDetail = new InProcessLocationDetail();
                inProcessLocationDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT)[0];
                inProcessLocationDetail.Qty = orderDetail.CurrentShipQty;
                inProcessLocationDetail.InProcessLocation = inProcessLocation;

                inProcessLocation.AddInProcessLocationDetail(inProcessLocationDetail);
            }

            return ShipOrder(inProcessLocation, user);
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(IList<InProcessLocationDetail> inProcessLocationDetailList, string userCode)
        {
            return ShipOrder(inProcessLocationDetailList, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(IList<InProcessLocationDetail> inProcessLocationDetailList, User user)
        {
            if (inProcessLocationDetailList != null && inProcessLocationDetailList.Count > 0)
            {

                InProcessLocation inProcessLocation = new InProcessLocation();
                inProcessLocation.InProcessLocationDetails = inProcessLocationDetailList;

                foreach (InProcessLocationDetail inProcessLocationDetail in inProcessLocationDetailList)
                {
                    inProcessLocationDetail.InProcessLocation = inProcessLocation;
                }

                return ShipOrder(inProcessLocation, user);
            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailShipEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(string pickListNo, string userCode)
        {
            return ShipOrder(pickListNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(string pickListNo, User user)
        {
            PickList pickList = this.pickListMgrE.CheckAndLoadPickList(pickListNo);

            //订单关闭后，拣货单也应该能够发货
            if (pickList.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS
                && pickList.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE
                && pickList.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE)
            {
                throw new BusinessErrorException("Order.Error.PickUp.StatusErrorWhenShip", pickList.Status, pickList.PickListNo);
            }

            PickListHelper.CheckAuthrize(pickList, user);

            InProcessLocation inProcessLocation = new InProcessLocation();

            foreach (PickListDetail pickListDetail in pickList.PickListDetails)
            {
                OrderLocationTransaction orderLocationTransaction = pickListDetail.OrderLocationTransaction;

                foreach (PickListResult pickListResult in pickListDetail.PickListResults)
                {
                    InProcessLocationDetail inProcessLocationDetail = new InProcessLocationDetail();
                    inProcessLocationDetail.HuId = pickListResult.LocationLotDetail.Hu.HuId;
                    inProcessLocationDetail.LotNo = pickListResult.LocationLotDetail.LotNo;
                    inProcessLocationDetail.OrderLocationTransaction = orderLocationTransaction;
                    inProcessLocationDetail.Qty = pickListResult.Qty / orderLocationTransaction.UnitQty; //订单单位
                    inProcessLocationDetail.InProcessLocation = inProcessLocation;

                    inProcessLocation.AddInProcessLocationDetail(inProcessLocationDetail);
                }
            }

            inProcessLocation = ShipOrder(inProcessLocation, user, false);

            #region 关闭捡货单
            pickList.LastModifyDate = DateTime.Now;
            pickList.LastModifyUser = user;
            pickList.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;

            this.pickListMgrE.UpdatePickList(pickList);
            #endregion

            return inProcessLocation;
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(PickList pickList, string userCode)
        {
            return ShipOrder(pickList.PickListNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(PickList pickList, User user)
        {
            return ShipOrder(pickList.PickListNo, user);
        }


        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(InProcessLocation inProcessLocation, string userCode)
        {
            return ShipOrder(inProcessLocation, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public InProcessLocation ShipOrder(InProcessLocation inProcessLocation, User user)
        {
            return ShipOrder(inProcessLocation, user, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<OrderDetail> orderDetailList, string userCode)
        {
            return ReceiveOrder(orderDetailList, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<OrderDetail> orderDetailList, User user)
        {
            return ReceiveOrder(orderDetailList, user, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<OrderDetail> orderDetailList, string userCode, bool isOddCreateHu)
        {
            return ReceiveOrder(orderDetailList, this.userMgrE.CheckAndLoadUser(userCode), isOddCreateHu);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<OrderDetail> orderDetailList, User user, bool isOddCreateHu)
        {
            if (orderDetailList != null && orderDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    ReceiptDetail receiptDetail = new ReceiptDetail();
                    receiptDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_IN)[0];
                    receiptDetail.HuId = orderDetail.HuId;
                    receiptDetail.ReceivedQty = orderDetail.CurrentReceiveQty;
                    receiptDetail.RejectedQty = orderDetail.CurrentRejectQty;
                    receiptDetail.ScrapQty = orderDetail.CurrentScrapQty;
                    receiptDetail.PutAwayBinCode = orderDetail.PutAwayBinCode;

                    //把模具字段带过去
                    receiptDetail.TextField3 = orderDetail.TextField3;
                    receiptDetail.Receipt = receipt;


                    receipt.AddReceiptDetail(receiptDetail);
                }

                return ReceiveOrder(receipt, user, null, true, isOddCreateHu);
            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, string userCode)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), null, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, string userCode, InProcessLocation inProcessLocation)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), null, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, string userCode, InProcessLocation inProcessLocation, string externalReceiptNo)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), null, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, string userCode, InProcessLocation inProcessLocation, string externalReceiptNo, IList<WorkingHours> workingHoursList)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), workingHoursList, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, string userCode, InProcessLocation inProcessLocation, string externalReceiptNo, IList<WorkingHours> workingHoursList, bool createIp)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), workingHoursList, createIp, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, string userCode, InProcessLocation inProcessLocation, string externalReceiptNo, IList<WorkingHours> workingHoursList, bool createIp, bool isOddCreateHu)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), workingHoursList, createIp, isOddCreateHu);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, string userCode)
        {
            return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), null, true, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, string userCode, IList<WorkingHours> workingHoursList)
        {
            return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), workingHoursList, true, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, string userCode, IList<WorkingHours> workingHoursList, bool createIp)
        {
            return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), workingHoursList, createIp, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, string userCode, IList<WorkingHours> workingHoursList, bool createIp, bool isOddCreateHu)
        {
            return ReceiveOrder(receipt, this.userMgrE.CheckAndLoadUser(userCode), workingHoursList, createIp, isOddCreateHu);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, User user)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, user, null, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, User user, InProcessLocation inProcessLocation)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, user, null, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, User user, InProcessLocation inProcessLocation, string externalReceiptNo)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, user, null, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, User user, InProcessLocation inProcessLocation, string externalReceiptNo, IList<WorkingHours> workingHoursList)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, user, workingHoursList, true, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, User user, InProcessLocation inProcessLocation, string externalReceiptNo, IList<WorkingHours> workingHoursList, bool createIp)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, user, workingHoursList, createIp, true);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(IList<ReceiptDetail> receiptDetailList, User user, InProcessLocation inProcessLocation, string externalReceiptNo, IList<WorkingHours> workingHoursList, bool createIp, bool isOddCreateHu)
        {
            if (receiptDetailList != null && receiptDetailList.Count > 0)
            {
                Receipt receipt = new Receipt();
                if (inProcessLocation != null)
                {
                    receipt.AddInProcessLocation(inProcessLocation);
                }
                receipt.ExternalReceiptNo = externalReceiptNo;
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    receiptDetail.Receipt = receipt;

                    receipt.AddReceiptDetail(receiptDetail);
                }
                return ReceiveOrder(receipt, user, workingHoursList, createIp, isOddCreateHu);

            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, User user)
        {
            return ReceiveOrder(receipt, user, null, true, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, User user, IList<WorkingHours> workingHoursList)
        {
            return ReceiveOrder(receipt, user, workingHoursList, true, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, User user, IList<WorkingHours> workingHoursList, bool createIp)
        {
            return ReceiveOrder(receipt, user, workingHoursList, createIp, true);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt ReceiveOrder(Receipt receipt, User user, IList<WorkingHours> workingHoursList, bool createIp, bool isOddCreateHu)
        {
            #region 变量定义
            IDictionary<string, OrderHead> cachedOrderHead = new Dictionary<string, OrderHead>();  //缓存用到的OrderHead
            DateTime dateTimeNow = DateTime.Now;
            #endregion

            #region 判断全0收货
            if (receipt != null && receipt.ReceiptDetails != null && receipt.ReceiptDetails.Count > 0)
            {
                //判断全0收货
                IList<ReceiptDetail> nonZeroReceiptDetailList = new List<ReceiptDetail>();
                foreach (ReceiptDetail receiptDetail in receipt.ReceiptDetails)
                {
                    if (receiptDetail.ReceivedQty != 0
                        || receiptDetail.RejectedQty != 0
                        || receiptDetail.ScrapQty != 0)
                    {
                        nonZeroReceiptDetailList.Add(receiptDetail);
                    }
                }

                if (nonZeroReceiptDetailList.Count == 0)
                {
                    throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
                }

                receipt.ReceiptDetails = nonZeroReceiptDetailList;
            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailReceiveEmpty");
            }
            #endregion

            #region 为未发货就收货创建ASN
            if ((receipt.InProcessLocations == null || receipt.InProcessLocations.Count == 0)
                && createIp)
            {
                InProcessLocation inProcessLocation = new InProcessLocation();

                #region 循环收货列表，并添加到发货列表中
                foreach (ReceiptDetail receiptDetail in receipt.ReceiptDetails)
                {
                    OrderLocationTransaction orderLocationTransaction = receiptDetail.OrderLocationTransaction;
                    OrderDetail orderDetail = orderLocationTransaction.OrderDetail;
                    OrderHead orderHead = orderDetail.OrderHead;

                    if (receiptDetail.MaterialFlushBack != null && receiptDetail.MaterialFlushBack.Count > 0)
                    {
                        #region 根据物料回冲创建ASN，只适应生产，其它情况没有测试
                        foreach (MaterialFlushBack materialFlushBack in receiptDetail.MaterialFlushBack)
                        {
                            InProcessLocationDetail inProcessLocationDetail = new InProcessLocationDetail();
                            inProcessLocationDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.LoadOrderLocationTransaction(materialFlushBack.OrderLocationTransaction.Id);
                            inProcessLocationDetail.HuId = materialFlushBack.HuId;
                            inProcessLocationDetail.LotNo = materialFlushBack.LotNo;
                            inProcessLocationDetail.Qty = materialFlushBack.Qty;
                            inProcessLocationDetail.InProcessLocation = inProcessLocation;

                            inProcessLocation.AddInProcessLocationDetail(inProcessLocationDetail);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 根据out的OrderLocationTransaction自动创建ASN
                        IList<OrderLocationTransaction> orderLocationTransactionList =
                            this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_OUT);

                        foreach (OrderLocationTransaction orderLocTrans in orderLocationTransactionList)
                        {
                            #region 直接Copy收货项至发货项
                            InProcessLocationDetail inProcessLocationDetail = new InProcessLocationDetail();
                            if (orderHead.IsShipScanHu)
                            {
                                //只有发货扫描条码才复制条码信息
                                inProcessLocationDetail.HuId = receiptDetail.HuId;
                                inProcessLocationDetail.LotNo = receiptDetail.LotNo;
                            }
                            inProcessLocationDetail.OrderLocationTransaction = orderLocTrans;
                            inProcessLocationDetail.Qty =
                                receiptDetail.ReceivedQty + receiptDetail.RejectedQty + receiptDetail.ScrapQty;
                            inProcessLocationDetail.InProcessLocation = inProcessLocation;

                            inProcessLocation.AddInProcessLocationDetail(inProcessLocationDetail);
                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion

                #region 发货
                DoShipOrder(inProcessLocation, user, true);

                receipt.AddInProcessLocation(inProcessLocation);
                #endregion
            }
            #endregion

            #region 更新订单信息
            EntityPreference entityPreference = this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_NO_PRICE_LIST_RECEIPT);
            foreach (ReceiptDetail receiptDetail in receipt.ReceiptDetails)
            {
                OrderLocationTransaction orderLocationTransaction = this.orderLocationTransactionMgrE.LoadOrderLocationTransaction(receiptDetail.OrderLocationTransaction.Id);
                receiptDetail.OrderLocationTransaction = orderLocationTransaction;
                OrderDetail orderDetail = orderLocationTransaction.OrderDetail;
                OrderHead orderHead = orderDetail.OrderHead;

                #region 判断OrderHead状态并缓存
                if (!cachedOrderHead.ContainsKey(orderHead.OrderNo))
                {
                    //检查权限
                    //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_RECEIVE_ORDER))
                    //{
                    //    throw new BusinessErrorException("Order.Error.NoReceivePermission", orderHead.OrderNo);
                    //}

                    //判断OrderHead状态，只要有ASN就都可以收货
                    if (!(orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS
                        || orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE
                        || orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE))
                    {
                        throw new BusinessErrorException("Order.Error.StatusErrorWhenReceive", orderHead.Status, orderHead.OrderNo);
                    }

                    //缓存OrderHead
                    cachedOrderHead.Add(orderHead.OrderNo, orderHead);
                }
                #endregion

                #region 整包装收货判断,快速的不考虑
                if (orderHead.FulfillUnitCount && orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML
                   && !(orderHead.IsAutoRelease && orderHead.IsAutoStart))
                {
                    if (receiptDetail.ReceivedQty % orderDetail.UnitCount != 0)
                    {
                        //不是整包装
                        throw new BusinessErrorException("Order.Error.NotFulfillUnitCountGrGi", orderDetail.Item.Code);
                    }
                }
                #endregion

                #region 是否过量收货判断
                if (orderDetail.OrderHead.SubType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_ADJ)
                {
                    //EntityPreference allowExceedentityPreference = this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_ALLOW_EXCEED_GI_GR);
                    //bool allowExceedGiGR = bool.Parse(allowExceedentityPreference.Value); //企业属性，允许过量发货和收货


                    //检查Received(已收数)不能大于等于OrderedQty(订单数)
                    //if (!(orderHead.AllowExceed && allowExceedGiGR) && orderDetail.ReceivedQty.HasValue)
                    //{
                    //    if ((orderDetail.OrderedQty > 0 && orderDetail.ReceivedQty.Value >= orderDetail.OrderedQty)
                    //        || (orderDetail.OrderedQty < 0 && orderDetail.ReceivedQty.Value <= orderDetail.OrderedQty))
                    //    {
                    //        throw new BusinessErrorException("Order.Error.ReceiveExcceed", orderHead.OrderNo, orderDetail.Item.Code);
                    //    }
                    //}
                    EntityPreference allowExceedentityPreference = this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_ALLOW_EXCEED_GI_GR);
                    bool allowExceedGiGR = false;//bool.Parse(entityPreference.Value); //企业属性，允许过量发货和收货,-1不做控制.正数控制超收超发范围
                    decimal allowPercent = 0;
                    try
                    {
                        allowPercent = decimal.Parse(allowExceedentityPreference.Value);
                        if (allowPercent == -1)
                        {
                            allowExceedGiGR = true;
                        }
                    }
                    catch (Exception)
                    {

                    }

                    if (!orderHead.AllowExceed && !allowExceedGiGR)   //不允许过量收货
                    {
                        decimal diffQty = 0;
                        if (!allowExceedGiGR)
                        {
                            diffQty = orderLocationTransaction.OrderedQty * allowPercent;
                        }

                        //检查AccumulateQty(已收数) + CurrentReceiveQty(本次收货数)不能大于OrderedQty(订单数)
                        orderDetail.ReceivedQty = orderDetail.ReceivedQty.HasValue ? orderDetail.ReceivedQty.Value : 0;
                        if ((orderDetail.OrderedQty > 0 && orderDetail.ReceivedQty + receiptDetail.ReceivedQty > orderDetail.OrderedQty + diffQty)
                            || (orderDetail.OrderedQty < 0 && orderDetail.ReceivedQty + receiptDetail.ReceivedQty < orderDetail.OrderedQty + diffQty))
                        {
                            throw new BusinessErrorException("Order.Error.ReceiveExcceed", orderHead.OrderNo, orderDetail.Item.Code);
                        }
                    }
                }
                #endregion

                #region 采购收货是否有价格单判断
                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT
                    && !bool.Parse(entityPreference.Value))
                {
                    if (orderDetail.UnitPrice == Decimal.Zero)
                    {
                        //重新查找一次价格
                        PriceListDetail priceListDetail = priceListDetailMgrE.GetLastestPriceListDetail(
                            orderDetail.DefaultPriceList,
                            orderDetail.Item,
                            orderHead.StartTime,
                            orderHead.Currency,
                            orderDetail.Uom);

                        if (priceListDetail != null)
                        {
                            orderDetail.UnitPrice = priceListDetail.UnitPrice;
                            orderDetail.IsProvisionalEstimate = priceListDetail.IsProvisionalEstimate;
                            orderDetail.IsIncludeTax = priceListDetail.IsIncludeTax;
                            orderDetail.TaxCode = priceListDetail.TaxCode;
                        }
                        else
                        {
                            throw new BusinessErrorException("Order.Error.NoPriceListReceipt", orderDetail.Item.Code);
                        }
                    }
                }
                #endregion

                //#region 计算PlannedAmount，用ReceiptDetail缓存本次收货产生的PlannedAmount金额，在创建PlannedBill时使用
                //if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT
                //    || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                //{
                //    CalculatePlannedAmount(orderDetail, receiptDetail, orderDetail.OrderHeadAmountAfterDiscount);
                //}
                //#endregion

                #region 记录OrderDetail的累计收货量
                if (!orderDetail.ReceivedQty.HasValue)
                {
                    orderDetail.ReceivedQty = 0;
                }
                if (!orderDetail.RejectedQty.HasValue)
                {
                    orderDetail.RejectedQty = 0;
                }
                if (!orderDetail.ScrapQty.HasValue)
                {
                    orderDetail.ScrapQty = 0;
                }
                orderDetail.ReceivedQty += receiptDetail.ReceivedQty;
                orderDetail.RejectedQty += receiptDetail.RejectedQty;
                orderDetail.ScrapQty += receiptDetail.ScrapQty;

                this.orderDetailMgrE.UpdateOrderDetail(orderDetail);
                #endregion

                #region 记录OrderLocationTransaction的累计收货量

                #region 成品
                if (!orderLocationTransaction.AccumulateQty.HasValue)
                {
                    orderLocationTransaction.AccumulateQty = 0;
                }
                orderLocationTransaction.AccumulateQty += receiptDetail.ReceivedQty * orderLocationTransaction.UnitQty;
                #endregion

                #region 次品
                if (!orderLocationTransaction.AccumulateRejectQty.HasValue)
                {
                    orderLocationTransaction.AccumulateRejectQty = 0;
                }
                orderLocationTransaction.AccumulateRejectQty += receiptDetail.RejectedQty * orderLocationTransaction.UnitQty;
                #endregion

                #region 废品
                if (!orderLocationTransaction.AccumulateScrapQty.HasValue)
                {
                    orderLocationTransaction.AccumulateScrapQty = 0;
                }
                orderLocationTransaction.AccumulateScrapQty += receiptDetail.ScrapQty * orderLocationTransaction.UnitQty;
                #endregion

                this.orderLocationTransactionMgrE.UpdateOrderLocationTransaction(orderLocationTransaction);
                #endregion
            }
            #endregion

            #region 创建收货单
            this.receiptMgrE.CreateReceipt(receipt, user, isOddCreateHu);
            #endregion

            #region 记录工时
            if (workingHoursList != null)
            {
                foreach (WorkingHours workingHours in workingHoursList)
                {
                    workingHours.Receipt = receipt;
                    workingHours.LastModifyDate = DateTime.Now;
                    workingHours.LastModifyUser = user;
                    this.workingHoursMgrE.CreateWorkingHours(workingHours);
                }
            }
            #endregion

            #region 更新订单头信息
            foreach (OrderHead orderHead in cachedOrderHead.Values)
            {
                orderHead.LastModifyUser = user;
                orderHead.LastModifyDate = dateTimeNow;
                this.orderHeadMgrE.UpdateOrderHead(orderHead);
                if (!orderHead.AllowRepeatlyExceed)
                {
                    TryCompleteOrder(orderHead, user);
                }
            }
            #endregion

            #region 处理委外加工
            foreach (ReceiptDetail receiptDetail in receipt.ReceiptDetails)
            {
                OrderLocationTransaction orderLocationTransaction = receiptDetail.OrderLocationTransaction;
                OrderDetail orderDetail = orderLocationTransaction.OrderDetail;
                OrderHead orderHead = orderDetail.OrderHead;
                Flow flow = this.flowMgrE.LoadFlow(orderHead.Flow);
                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING)
                {
                    Flow productionFlow = this.flowMgrE.LoadFlow(flow.ReferenceFlow, true);

                    OrderHead productionOrderHead = this.TransferFlow2Order(productionFlow, false);

                    foreach (FlowDetail productionFlowDetail in productionFlow.FlowDetails)
                    {
                        if (((productionFlowDetail.DefaultLocationTo != null && receiptDetail.OrderLocationTransaction.Location.Code == productionFlowDetail.DefaultLocationTo.Code)//目的库位相同
                            || receiptDetail.OrderLocationTransaction.Location.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_REJECT)//不合格品库位
                            && receiptDetail.OrderLocationTransaction.Item.Code == productionFlowDetail.Item.Code)
                        {
                            OrderDetail productionOrderDetail = this.orderDetailMgrE.TransferFlowDetail2OrderDetail(productionFlowDetail);
                            productionOrderDetail.LocationTo = receiptDetail.OrderLocationTransaction.Location;

                            #region 合并相同的productionOrderDetail
                            bool findMatch = false;
                            if (productionOrderHead.OrderDetails != null && productionOrderHead.OrderDetails.Count > 0)
                            {
                                foreach (OrderDetail addProductionOrderDetail in productionOrderHead.OrderDetails)
                                {
                                    if (productionOrderDetail.Item.Code == addProductionOrderDetail.Item.Code
                                       && productionOrderDetail.Uom.Code == addProductionOrderDetail.Uom.Code
                                       && productionOrderDetail.UnitCount == addProductionOrderDetail.UnitCount
                                       && LocationHelper.IsLocationEqual(productionOrderDetail.DefaultLocationFrom, addProductionOrderDetail.DefaultLocationFrom)
                                       && LocationHelper.IsLocationEqual(productionOrderDetail.DefaultLocationTo, addProductionOrderDetail.DefaultLocationTo))
                                    {
                                        decimal addQty = receiptDetail.ReceivedQty;
                                        if (addProductionOrderDetail.Uom.Code != orderDetail.Uom.Code)
                                        {
                                            addProductionOrderDetail.OrderedQty += this.uomConversionMgrE.ConvertUomQty(addProductionOrderDetail.Item, orderDetail.Uom, addQty, productionOrderDetail.Uom);
                                        }
                                        else
                                        {
                                            addProductionOrderDetail.OrderedQty += addQty;
                                        }
                                        findMatch = true;
                                    }
                                }
                            }
                            #endregion

                            if (!findMatch)
                            {
                                productionOrderDetail.OrderHead = productionOrderHead;
                                productionOrderDetail.OrderedQty = receiptDetail.ReceivedQty;
                                if (productionOrderDetail.Uom.Code != orderDetail.Uom.Code)
                                {
                                    productionOrderDetail.OrderedQty = this.uomConversionMgrE.ConvertUomQty(productionOrderDetail.Item, orderDetail.Uom, receiptDetail.ReceivedQty, productionOrderDetail.Uom);
                                }

                                productionOrderHead.AddOrderDetail(productionOrderDetail);
                            }
                        }
                    }

                    if (productionOrderHead.OrderDetails != null && productionOrderHead.OrderDetails.Count > 0)
                    {
                        productionOrderHead.IsAutoRelease = true;
                        productionOrderHead.IsAutoStart = true;
                        productionOrderHead.IsAutoShip = true;
                        productionOrderHead.IsAutoReceive = true;
                        productionOrderHead.IsForceRelease = true;
                        productionOrderHead.StartLatency = 0;
                        productionOrderHead.CompleteLatency = 0;
                        productionOrderHead.StartTime = orderHead.StartTime;
                        productionOrderHead.WindowTime = orderHead.WindowTime;
                        productionOrderHead.ReferenceOrderNo = orderHead.OrderNo;
                        productionOrderHead.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                        productionOrderHead.IsSubcontract = true;

                        #region 生产要把班次加上
                        productionOrderHead.Shift = shiftMgrE.GetDefaultShift();

                        #endregion
                        this.CreateOrder(productionOrderHead, user);
                    }
                }
            }
            #endregion

            #region 处理路线绑定
            foreach (OrderHead orderHead in cachedOrderHead.Values)
            {
                this.CreateBindingOrder(orderHead, user,
                    BusinessConstants.CODE_MASTER_BINDING_TYPE_VALUE_RECEIVE_ASYN,
                    BusinessConstants.CODE_MASTER_BINDING_TYPE_VALUE_RECEIVE_SYN);
            }
            #endregion

            #region 处理模具使用次数

            if (receipt.OrderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
             || receipt.OrderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING)
            {
                var facilityAllocatesList = hqlMgr.FindAll<FacilityAllocates>("from FacilityAllocates where IsActive = 1");
                foreach (ReceiptDetail d in receipt.ReceiptDetails)
                {
                    FacilityAllocates facilityAllocates = facilityAllocatesList.Where(p => p.ItemCode == d.OrderLocationTransaction.Item.Code && p.FCID == d.TextField3).FirstOrDefault();
                    if (facilityAllocates != null)
                    {
                        FacilityMasters facilityMasters = hqlMgr.FindById<FacilityMasters>(d.TextField3);
                        facilityMasters.UseQty += (d.ReceivedQty + d.RejectedQty +d.ScrapQty) / facilityAllocates.MouldCount;
                        hqlMgr.Update(facilityMasters);
                    }
                }
            }

            #endregion

            return receipt;
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt QuickReceiveOrder(string flowCode, IList<OrderDetail> orderDetailList, string userCode)
        {
            DateTime dateTimeNow = DateTime.Now;
            return this.QuickReceiveOrder(flowCode, orderDetailList, userCode, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML,
                dateTimeNow, dateTimeNow, false, null, null);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt QuickReceiveOrder(string flowCode, IList<OrderDetail> orderDetailList, string userCode, string orderSubType, DateTime winTime, DateTime startTime, bool isUrgent, string referenceOrderNo, string externalOrderNo)
        {
            Flow flow = this.flowMgrE.CheckAndLoadFlow(flowCode, true);
            User user = this.userMgrE.CheckAndLoadUser(userCode);
            return this.QuickReceiveOrder(flow, orderDetailList, user, orderSubType, winTime, startTime, isUrgent, referenceOrderNo, externalOrderNo);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt QuickReceiveOrder(Flow flow, IList<OrderDetail> orderDetailList, User user)
        {
            DateTime dateTimeNow = DateTime.Now;
            return this.QuickReceiveOrder(flow, orderDetailList, user, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML,
               dateTimeNow, dateTimeNow, false, null, null);
        }

        [Transaction(TransactionMode.Requires)]
        public Receipt QuickReceiveOrder(Flow flow, IList<OrderDetail> orderDetailList, User user, string orderSubType, DateTime winTime, DateTime startTime, bool isUrgent, string referenceOrderNo, string externalOrderNo)
        {
            if (flow.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                throw new TechnicalException("QuickReceiveOrder not support Production");
            }

            #region 缓存上架信息
            IDictionary<string, string> huIdStorageBinDic = new Dictionary<string, string>();
            foreach (OrderDetail sourceOrderDetail in orderDetailList)
            {
                if (sourceOrderDetail.HuId != null && sourceOrderDetail.HuId.Trim() != string.Empty &&
                    sourceOrderDetail.PutAwayBinCode != null && sourceOrderDetail.PutAwayBinCode.Trim() != string.Empty)
                {
                    if (!huIdStorageBinDic.ContainsKey(sourceOrderDetail.HuId.Trim()))
                    {
                        huIdStorageBinDic.Add(sourceOrderDetail.HuId.Trim(), sourceOrderDetail.PutAwayBinCode.Trim());
                    }
                    else
                    {
                        if (huIdStorageBinDic[sourceOrderDetail.HuId.Trim()] != sourceOrderDetail.PutAwayBinCode.Trim())
                        {
                            throw new BusinessErrorException("Common.Business.Error.OneHuCannotInTwoBin");
                        }
                    }
                }
            }
            #endregion

            #region 初始化订单头
            OrderHead orderHead = this.TransferFlow2Order(flow, orderSubType);

            #region 从不合格品库位退货，收货扫描一定要设为False
            if (orderDetailList != null && orderDetailList.Count > 0)
            {
                //如果不是所有明细的目的库位都是Reject，可能有问题。
                //if (orderSubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN &&
                //    orderDetailList[0].DefaultLocationTo != null &&
                //    orderDetailList[0].DefaultLocationTo.Code == BusinessConstants.SYSTEM_LOCATION_REJECT)
                //{
                //    orderHead.IsReceiptScanHu = false;
                //    orderHead.LocationTo = this.locationMgrE.GetRejectLocation();
                //}
                if (orderSubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_REJ)
                {
                    orderHead.IsReceiptScanHu = false;
                }
            }
            #endregion

            IList<OrderDetail> targetOrderDetailList = orderHead.OrderDetails;

            if (targetOrderDetailList == null)
            {
                targetOrderDetailList = new List<OrderDetail>();
            }

            orderHead.SubType = orderSubType;
            orderHead.WindowTime = winTime;
            orderHead.StartTime = startTime;
            orderHead.ReferenceOrderNo = referenceOrderNo;
            orderHead.ExternalOrderNo = externalOrderNo;
            if (isUrgent)
            {
                orderHead.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_URGENT;
            }
            else
            {
                orderHead.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
            }

            orderHead.IsAutoRelease = true;
            orderHead.IsAutoStart = true;
            orderHead.IsAutoCreatePickList = false;
            orderHead.IsAutoShip = false;
            orderHead.IsAutoReceive = false;
            #endregion

            #region 合并OrderDetailList
            if (orderDetailList != null && orderDetailList.Count > 0)
            {
                IList<OrderDetail> newOrderDetailList = new List<OrderDetail>();
                foreach (OrderDetail sourceOrderDetail in orderDetailList)
                {
                    bool findMatch = false;

                    #region 在FlowDetail转换的OrderDetail里面查找匹配项
                    foreach (OrderDetail targetOrderDetail in targetOrderDetailList)
                    {
                        if (sourceOrderDetail.Item.Code == targetOrderDetail.Item.Code
                            && sourceOrderDetail.Uom.Code == targetOrderDetail.Uom.Code
                            && sourceOrderDetail.UnitCount == targetOrderDetail.UnitCount
                            && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationFrom, targetOrderDetail.DefaultLocationFrom)
                            && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationTo, targetOrderDetail.DefaultLocationTo))
                        {
                            targetOrderDetail.RequiredQty += sourceOrderDetail.OrderedQty;
                            targetOrderDetail.OrderedQty += sourceOrderDetail.OrderedQty;
                            targetOrderDetail.Remark = sourceOrderDetail.Remark;

                            findMatch = true;

                            break;
                        }
                    }
                    #endregion

                    if (!findMatch)
                    {
                        #region 没有找到匹配项，从新增匹配项中找
                        foreach (OrderDetail newOrderDetail in newOrderDetailList)
                        {
                            if (sourceOrderDetail.Item.Code == newOrderDetail.Item.Code
                            && sourceOrderDetail.Uom.Code == newOrderDetail.Uom.Code
                            && sourceOrderDetail.UnitCount == newOrderDetail.UnitCount
                            && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationFrom, newOrderDetail.DefaultLocationFrom)
                            && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationTo, newOrderDetail.DefaultLocationTo))
                            {
                                newOrderDetail.RequiredQty += sourceOrderDetail.OrderedQty;
                                newOrderDetail.OrderedQty += sourceOrderDetail.OrderedQty;
                                newOrderDetail.Remark = sourceOrderDetail.Remark;
                                findMatch = true;

                                break;
                            }
                        }
                        #endregion

                        if (!findMatch)
                        {
                            #region 还没有找到匹配项，新增到newOrderDetailList中
                            OrderDetail clonedSourceOrderDetail = new OrderDetail();
                            CloneHelper.CopyProperty(sourceOrderDetail, clonedSourceOrderDetail);
                            //CloneHelper.DeepClone<OrderDetail>(sourceOrderDetail);
                            clonedSourceOrderDetail.OrderHead = orderHead;
                            newOrderDetailList.Add(clonedSourceOrderDetail);
                            #endregion
                        }
                    }
                }

                if (newOrderDetailList.Count > 0)
                {
                    #region 合并新增的OrderDetail
                    int seqInterval = int.Parse(this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_SEQ_INTERVAL).Value);
                    int maxSeq = 0;
                    foreach (OrderDetail targetOrderDetail in targetOrderDetailList)
                    {
                        if (targetOrderDetail.Sequence > maxSeq)
                        {
                            maxSeq = targetOrderDetail.Sequence;
                        }
                    }

                    foreach (OrderDetail newOrderDetail in newOrderDetailList)
                    {
                        maxSeq += seqInterval;
                        newOrderDetail.Sequence = maxSeq;

                        orderHead.AddOrderDetail(newOrderDetail);
                    }
                    #endregion
                }
            }

            #endregion

            #region 创建订单
            this.CreateOrder(orderHead, user);
            #endregion

            #region 发货
            IList<InProcessLocationDetail> inProcessLocationDetailList = new List<InProcessLocationDetail>();
            foreach (OrderDetail sourceOrderDetail in orderDetailList)
            {
                foreach (OrderDetail targetOrderDetail in orderHead.OrderDetails)
                {
                    if (sourceOrderDetail.Item.Code == targetOrderDetail.Item.Code
                        && sourceOrderDetail.Uom.Code == targetOrderDetail.Uom.Code
                        && sourceOrderDetail.UnitCount == targetOrderDetail.UnitCount
                        && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationFrom, targetOrderDetail.DefaultLocationFrom)
                        && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationTo, targetOrderDetail.DefaultLocationTo))
                    {

                        if (orderSubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_ADJ
                            && sourceOrderDetail.HuId != null && sourceOrderDetail.HuId.Trim() != string.Empty)
                        {
                            #region 处理按条码的调整，去掉条码，只调整原库位数量
                            InProcessLocationDetail rtnInProcessLocationDetail = new InProcessLocationDetail();

                            rtnInProcessLocationDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(targetOrderDetail.Id, BusinessConstants.IO_TYPE_OUT)[0];
                            Hu hu = this.huMgrE.CheckAndLoadHu(sourceOrderDetail.HuId);
                            rtnInProcessLocationDetail.LotNo = hu.LotNo;
                            rtnInProcessLocationDetail.Qty = 0 - hu.Qty;
                            inProcessLocationDetailList.Add(rtnInProcessLocationDetail);

                            InProcessLocationDetail adjInProcessLocationDetail = new InProcessLocationDetail();
                            adjInProcessLocationDetail.OrderLocationTransaction = rtnInProcessLocationDetail.OrderLocationTransaction;
                            adjInProcessLocationDetail.LotNo = hu.LotNo;
                            adjInProcessLocationDetail.Qty = sourceOrderDetail.OrderedQty + hu.Qty;
                            inProcessLocationDetailList.Add(adjInProcessLocationDetail);
                            #endregion
                        }
                        else
                        {
                            InProcessLocationDetail inProcessLocationDetail = new InProcessLocationDetail();

                            inProcessLocationDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(targetOrderDetail.Id, BusinessConstants.IO_TYPE_OUT)[0];
                            inProcessLocationDetail.HuId = sourceOrderDetail.HuId;
                            if (inProcessLocationDetail.HuId != null && inProcessLocationDetail.HuId.Trim() != string.Empty)
                            {
                                Hu hu = this.huMgrE.CheckAndLoadHu(inProcessLocationDetail.HuId);
                                inProcessLocationDetail.LotNo = hu.LotNo;

                                //设置退货上架库格
                                if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN)
                                {
                                    if (huIdStorageBinDic.ContainsKey(hu.HuId))
                                    {
                                        inProcessLocationDetail.ReturnPutAwaySorageBinCode = huIdStorageBinDic[hu.HuId];
                                    }
                                }
                            }
                            inProcessLocationDetail.Qty = sourceOrderDetail.OrderedQty;

                            inProcessLocationDetailList.Add(inProcessLocationDetail);

                            break;
                        }
                    }

                }
            }

            InProcessLocation inProcessLocation = this.ShipOrder(inProcessLocationDetailList, user);
            #endregion

            #region 为收货调整重新赋条码，增加目的库位的条码数量
            if (orderSubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_ADJ)
            {
                foreach (InProcessLocationDetail inProcessLocationDetail in inProcessLocation.InProcessLocationDetails)
                {
                    foreach (OrderDetail sourceOrderDetail in orderDetailList)
                    {
                        if (sourceOrderDetail.HuId != null && sourceOrderDetail.HuId.Trim() != string.Empty)
                        {
                            if (sourceOrderDetail.Item.Code == inProcessLocationDetail.OrderLocationTransaction.OrderDetail.Item.Code
                            && sourceOrderDetail.Uom.Code == inProcessLocationDetail.OrderLocationTransaction.OrderDetail.Uom.Code
                            && sourceOrderDetail.UnitCount == inProcessLocationDetail.OrderLocationTransaction.OrderDetail.UnitCount
                            && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationFrom, inProcessLocationDetail.OrderLocationTransaction.OrderDetail.DefaultLocationFrom)
                            && LocationHelper.IsLocationEqual(sourceOrderDetail.DefaultLocationTo, inProcessLocationDetail.OrderLocationTransaction.OrderDetail.DefaultLocationTo))
                            {

                                Hu hu = this.huMgrE.CheckAndLoadHu(sourceOrderDetail.HuId);
                                inProcessLocationDetail.HuId = hu.HuId;
                                inProcessLocationDetail.LotNo = hu.LotNo;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region 收货
            if (orderSubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
            {
                return ReceiveFromInProcessLocation(inProcessLocation, user, huIdStorageBinDic, externalOrderNo);
            }
            else
            {
                return ReceiveFromInProcessLocation(inProcessLocation, user, null, externalOrderNo);
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCompleteOrder(string orderNo, string userCode)
        {
            this.ManualCompleteOrder(orderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCompleteOrder(string orderNo, User user)
        {
            OrderHead orderHead = this.orderHeadMgrE.LoadOrderHead(orderNo);

            //检查权限
            //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_COMPLETE_ORDER))
            //{
            //    throw new BusinessErrorException("Order.Error.NoCompletePermission", orderHead.OrderNo);
            //}

            //检查状态
            if (orderHead.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS)
            {
                throw new BusinessErrorException("Order.Error.StatusErrorWhenComplete", orderHead.Status, orderHead.OrderNo);
            }

            //#region 检查是否有未关闭的ASN
            //IList<InProcessLocationDetail> inProcessLocationDetailList =
            //    this.inProcessLocationDetailMgrE.GetInProcessLocationDetail(orderHead);

            //if (inProcessLocationDetailList != null && inProcessLocationDetailList.Count > 0)
            //{
            //    foreach (InProcessLocationDetail inProcessLocationDetail in inProcessLocationDetailList)
            //    {
            //        if (inProcessLocationDetail.InProcessLocation.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            //        {
            //            throw new BusinessErrorException("Order.Error.OrderComplete.UnclosedASN", orderNo);
            //        }
            //    }
            //}
            //#endregion

            #region 处理模具,发保养任务
            if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML && (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING))
            {
                foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                {
                    if (!string.IsNullOrEmpty(orderDetail.TextField3))
                    {
                        FacilityMasters facilityMasters = hqlMgr.FindById<FacilityMasters>(orderDetail.TextField3);
                        facilityMasters.Status = BusinessConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE;
                        facilityMasters.LastModifyUser = user.Code;
                        facilityMasters.LastModifyDate = DateTime.Now;
                        hqlMgr.Update(facilityMasters);

                        #region 记事务
                        #endregion

                        #region 记保养任务
                        FacilityMaintainPlans facilityMaintainPlans = hqlMgr.FindAll<FacilityMaintainPlans>(" from FacilityMaintainPlans where FCID = ? and MPCode = ?", new object[] { orderDetail.TextField3, "YH_MP_1" }).FirstOrDefault();
                        if (facilityMaintainPlans == null)
                        {
                            facilityMaintainPlans = new FacilityMaintainPlans();
                            facilityMaintainPlans.NextMaintainDate = DateTime.Now;
                            facilityMaintainPlans.NextWarnDate = DateTime.Now;
                            facilityMaintainPlans.StartDate = DateTime.Now;
                            facilityMaintainPlans.MaintainPlanCode = "YH_MP_1";
                            facilityMaintainPlans.FCID = orderDetail.TextField3;
                            hqlMgr.Create(facilityMaintainPlans);
                        }
                        else
                        {
                            facilityMaintainPlans.NextMaintainDate = DateTime.Now;
                            facilityMaintainPlans.NextWarnDate = DateTime.Now;
                            facilityMaintainPlans.StartDate = DateTime.Now;
                            hqlMgr.Update(facilityMaintainPlans);
                        }

                        #endregion
                    }
                }
            }
            #endregion


            DateTime nowDate = DateTime.Now;
            orderHead.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE;
            orderHead.CompleteDate = nowDate;
            orderHead.CompleteUser = user;
            orderHead.LastModifyDate = nowDate;
            orderHead.LastModifyUser = user;

            this.orderHeadMgrE.UpdateOrderHead(orderHead);

            #region 记录工单设置成本
            if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                this.costMgr.RecordProductionSettingCostTransaction(orderHead, user);
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCompleteOrder(OrderHead orderHead, string userCode)
        {
            this.ManualCompleteOrder(orderHead.OrderNo, this.userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void ManualCompleteOrder(OrderHead orderHead, User user)
        {
            this.ManualCompleteOrder(orderHead.OrderNo, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCompleteOrder(OrderHead orderHead, User user)
        {
            OrderHead oldOrderHead = this.orderHeadMgrE.LoadOrderHead(orderHead.OrderNo, true);
            bool isComplete = true;
            if (oldOrderHead.Type != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                #region 物流完工，发货数大于等于订单数
                foreach (OrderDetail orderDetail in oldOrderHead.OrderDetails)
                {
                    if (orderDetail.ShippedQty.HasValue)
                    {
                        if (orderDetail.OrderedQty > 0 && (orderDetail.OrderedQty > orderDetail.ShippedQty.Value)
                            || orderDetail.OrderedQty < 0 && (orderDetail.OrderedQty < orderDetail.ShippedQty.Value))
                        {
                            isComplete = false;
                            break;
                        }
                    }
                    else
                    {
                        isComplete = false;
                        break;
                    }
                }


                #endregion
            }
            else
            {
                #region 生产完工，收货数大于等于订单数
                //电镀：合格数+废品数+不合格数>=计划数，订单完成
                bool isIncludeRejectAndScrap = bool.Parse(entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_ORDER_COMPLETE_PRINCIPLE).Value);
                foreach (OrderDetail orderDetail in oldOrderHead.OrderDetails)
                {
                    if (orderDetail.ReceivedQty.HasValue)
                    {
                        if (isIncludeRejectAndScrap)
                        {
                            decimal totalHandleQty = orderDetail.ReceivedQty.Value + orderDetail.RejectedQty.Value + orderDetail.ScrapQty.Value;
                            if (orderDetail.OrderedQty > 0 && (orderDetail.OrderedQty > totalHandleQty)
                                || orderDetail.OrderedQty < 0 && (orderDetail.OrderedQty < totalHandleQty)
                                || (orderDetail.OrderedQty > 0 && totalHandleQty == orderDetail.ScrapQty.Value))
                            {
                                isComplete = false;
                                break;
                            }
                        }
                        else
                        {
                            if (orderDetail.OrderedQty > 0 && (orderDetail.OrderedQty > orderDetail.ReceivedQty.Value)
                                || orderDetail.OrderedQty < 0 && (orderDetail.OrderedQty < orderDetail.ReceivedQty.Value))
                            {
                                isComplete = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        isComplete = false;
                        break;
                    }
                }
                #endregion
            }

     


            if (isComplete)
            {
                DateTime nowDate = DateTime.Now;
                orderHead.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE;
                orderHead.CompleteDate = nowDate;
                orderHead.CompleteUser = user;
                orderHead.LastModifyDate = nowDate;
                orderHead.LastModifyUser = user;

                this.orderHeadMgrE.UpdateOrderHead(orderHead);

                #region 记录工单设置成本
                if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    this.costMgr.RecordProductionSettingCostTransaction(orderHead, user);
                }
                #endregion

                #region 处理模具,发保养任务
                if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML && (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                  || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING))
                {
                    foreach (OrderDetail orderDetail in oldOrderHead.OrderDetails)
                    {
                        if (!string.IsNullOrEmpty(orderDetail.TextField3))
                        {
                            FacilityMasters facilityMasters = hqlMgr.FindById<FacilityMasters>(orderDetail.TextField3);
                            facilityMasters.Status = BusinessConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE;
                            facilityMasters.LastModifyUser = user.Code;
                            facilityMasters.LastModifyDate = DateTime.Now;
                            hqlMgr.Update(facilityMasters);

                            #region 记事务
                            #endregion

                            #region 记保养任务
                            FacilityMaintainPlans facilityMaintainPlans = hqlMgr.FindAll<FacilityMaintainPlans>(" from FacilityMaintainPlans where FCID = ? and MPCode = ?", new object[] { orderDetail.TextField3, "YH_MP_1" }).FirstOrDefault();
                            if (facilityMaintainPlans == null)
                            {
                                facilityMaintainPlans = new FacilityMaintainPlans();
                                facilityMaintainPlans.NextMaintainDate = DateTime.Now;
                                facilityMaintainPlans.NextWarnDate = DateTime.Now;
                                facilityMaintainPlans.StartDate = DateTime.Now;
                                facilityMaintainPlans.MaintainPlanCode = "YH_MP_1";
                                facilityMaintainPlans.FCID = orderDetail.TextField3;
                                hqlMgr.Create(facilityMaintainPlans);
                            }
                            else
                            {
                                facilityMaintainPlans.NextMaintainDate = DateTime.Now;
                                facilityMaintainPlans.NextWarnDate = DateTime.Now;
                                facilityMaintainPlans.StartDate = DateTime.Now;
                                hqlMgr.Update(facilityMaintainPlans);
                            }
                            #endregion
                        }
                    }
                }
                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void TryCompleteOrder(IList<OrderHead> orderHeadList, User user)
        {
            foreach (OrderHead orderHead in orderHeadList)
            {
                TryCompleteOrder(orderHead, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void TryUpdateWoLoctrans(string orderNo, bool isReuse)
        {
            OrderHead orderHead = this.orderHeadMgrE.LoadOrderHead(orderNo, true, false, true);
            foreach (OrderDetail od in orderHead.OrderDetails)
            {
                foreach (OrderLocationTransaction ol in od.OrderLocationTransactions)
                {
                    if (ol.IOType == BusinessConstants.IO_TYPE_OUT)
                    {
                        if (!isReuse || isReuse && ol.Item.Code.ToUpper() != od.Item.Code)
                        {
                            ol.OrderedQty = 0;
                            this.orderLocationTransactionMgrE.UpdateOrderLocationTransaction(ol);
                        }
                    }
                }
            }
        }

        #region Convert Object Methods
        [Transaction(TransactionMode.Unspecified)]
        public InProcessLocation ConvertOrderLocTransToInProcessLocation(IList<OrderLocationTransaction> orderLocTransList)
        {
            InProcessLocation inProcessLocation = new InProcessLocation();
            if (orderLocTransList != null && orderLocTransList.Count > 0)
            {
                inProcessLocation = inProcessLocationMgrE.GenerateInProcessLocation(orderLocTransList[0].OrderDetail.OrderHead);
                foreach (OrderLocationTransaction orderLocTrans in orderLocTransList)
                {
                    if (orderLocTrans.OrderDetail.RemainShippedQty != 0)
                    {
                        InProcessLocationDetail inProcessLocationDetail = new InProcessLocationDetail();
                        inProcessLocationDetail.OrderLocationTransaction = orderLocTrans;
                        inProcessLocationDetail.QtyToShip = orderLocTrans.OrderDetail.RemainShippedQty;
                        inProcessLocationDetail.Qty = inProcessLocationDetail.QtyToShip;
                        inProcessLocationDetail.InProcessLocation = inProcessLocation;

                        inProcessLocation.AddInProcessLocationDetail(inProcessLocationDetail);
                    }
                }
            }

            return inProcessLocation;
        }

        [Transaction(TransactionMode.Unspecified)]
        public InProcessLocation ConvertOrderToInProcessLocation(string orderNo)
        {
            InProcessLocation inProcessLocation = new InProcessLocation();
            IList<OrderLocationTransaction> orderLocTransList = orderLocationTransactionMgrE.GetOrderLocationTransaction(orderNo, BusinessConstants.IO_TYPE_OUT);

            return this.ConvertOrderLocTransToInProcessLocation(orderLocTransList);
        }

        [Transaction(TransactionMode.Unspecified)]
        public InProcessLocation ConvertPickListToInProcessLocation(string pickListNo)
        {
            IList<InProcessLocationDetail> ipDetList = new List<InProcessLocationDetail>();
            IList<PickListResult> pickListResultList = pickListResultMgrE.GetPickListResult(pickListNo);
            if (pickListResultList != null && pickListResultList.Count > 0)
            {
                foreach (PickListResult pickListResult in pickListResultList)
                {
                    InProcessLocationDetail ipDet = new InProcessLocationDetail();
                    ipDet.OrderLocationTransaction = pickListResult.PickListDetail.OrderLocationTransaction;
                    ipDet.HuId = pickListResult.LocationLotDetail.Hu.HuId;
                    ipDet.LotNo = pickListResult.LocationLotDetail.LotNo;
                    ipDet.QtyToShip = pickListResult.Qty;//单位换算
                    ipDet.Qty = pickListResult.Qty;

                    ipDetList.Add(ipDet);
                }
            }

            InProcessLocation ip = inProcessLocationMgrE.GenerateInProcessLocation(ipDetList[0].OrderLocationTransaction.OrderDetail.OrderHead);
            ip.InProcessLocationDetails = ipDetList;

            return ip;
        }

        [Transaction(TransactionMode.Unspecified)]
        public Receipt ConvertOrderDetailToReceipt(IList<OrderDetail> orderDetailList)
        {
            Receipt receipt = new Receipt();
            if (orderDetailList != null && orderDetailList.Count > 0)
            {
                foreach (OrderDetail orderDetail in orderDetailList)
                {
                    ReceiptDetail receiptDetail = new ReceiptDetail();
                    receiptDetail.OrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_IN)[0];
                    if (orderDetail.OrderHead.IsReceiptScanHu)   //只有收货扫描条码赋值
                    {
                        receiptDetail.HuId = orderDetail.HuId;
                    }
                    //receiptDetail.ShippedQty = orderDetail.RemainShippedQty;  ShippedQty都是后台赋值的，从InProcessLocationDetail中取值
                    //receiptDetail.ReceivedQty = receiptDetail.ShippedQty;
                    receiptDetail.ReceivedQty = orderDetail.RemainShippedQty;
                    receiptDetail.Receipt = receipt;

                    //if (receiptDetail.ShippedQty != 0)//过滤已收满数量
                    if (receiptDetail.ReceivedQty != 0)//过滤已收满数量
                        receipt.AddReceiptDetail(receiptDetail);
                }
            }

            return receipt;
        }

        [Transaction(TransactionMode.Unspecified)]
        public Receipt ConvertInProcessLocationToReceipt(InProcessLocation inProcessLocation)
        {
            return ConvertInProcessLocationToReceipt(inProcessLocation, null, null);
        }

        public Receipt ConvertInProcessLocationToReceipt(InProcessLocation inProcessLocation, IDictionary<string, string> huIdStorageBinDic)
        {
            return ConvertInProcessLocationToReceipt(inProcessLocation, huIdStorageBinDic, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public Receipt ConvertInProcessLocationToReceipt(InProcessLocation inProcessLocation, IDictionary<string, string> huIdStorageBinDic, string externalOrderNo)
        {
            Receipt receipt = new Receipt();
            receipt.ExternalReceiptNo = externalOrderNo;
            receipt.AddInProcessLocation(inProcessLocation);
            if (inProcessLocation.InProcessLocationDetails != null && inProcessLocation.InProcessLocationDetails.Count > 0)
            {
                foreach (InProcessLocationDetail inProcessLocationDetail in inProcessLocation.InProcessLocationDetails)
                {
                    OrderLocationTransaction orderLocationTransaction = inProcessLocationDetail.OrderLocationTransaction;
                    OrderDetail orderDetail = orderLocationTransaction.OrderDetail;
                    OrderHead orderHead = orderDetail.OrderHead;

                    OrderLocationTransaction inOrderLocationTransaction = this.orderLocationTransactionMgrE.GetOrderLocationTransaction(orderDetail.Id, BusinessConstants.IO_TYPE_IN)[0];

                    bool isMerge = false;
                    if (receipt.ReceiptDetails != null && receipt.ReceiptDetails.Count > 0 && !inProcessLocation.IsReceiptScanHu)
                    {
                        //如果收货不扫描条码，收货项需要根据发货项进行合并
                        foreach (ReceiptDetail receiptDetail in receipt.ReceiptDetails)
                        {
                            if (inOrderLocationTransaction.Id == receiptDetail.OrderLocationTransaction.Id)
                            {
                                //if (inProcessLocationDetail.IsConsignment == receiptDetail.IsConsignment
                                //    && inProcessLocationDetail.PlannedBill == receiptDetail.PlannedBill) {
                                //    throw new BusinessErrorException("寄售库存，不能按按数量进行收货。");
                                //}

                                isMerge = true;
                                receiptDetail.ShippedQty += inProcessLocationDetail.Qty;
                                receiptDetail.ReceivedQty += inProcessLocationDetail.Qty;
                                break;
                            }
                        }
                    }

                    if (!isMerge)
                    {
                        ReceiptDetail receiptDetail = new ReceiptDetail();

                        receiptDetail.OrderLocationTransaction = inOrderLocationTransaction;
                        receiptDetail.Id = inProcessLocationDetail.Id;
                        receiptDetail.ShippedQty = inProcessLocationDetail.Qty;
                        receiptDetail.ReceivedQty = inProcessLocationDetail.Qty;
                        if (inProcessLocation.IsReceiptScanHu)   //只有按条码收货才Copy条码信息
                        {
                            receiptDetail.HuId = inProcessLocationDetail.HuId;
                            receiptDetail.LotNo = inProcessLocationDetail.LotNo;

                            //上架库位赋值
                            if (inOrderLocationTransaction.OrderDetail.OrderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML
                                && huIdStorageBinDic != null
                                && receiptDetail.HuId != null
                                && receiptDetail.HuId.Trim() != string.Empty
                                && huIdStorageBinDic.ContainsKey(receiptDetail.HuId.Trim()))
                            {
                                receiptDetail.PutAwayBinCode = huIdStorageBinDic[receiptDetail.HuId.Trim()];
                            }
                        }
                        receiptDetail.IsConsignment = inProcessLocationDetail.IsConsignment;
                        receiptDetail.PlannedBill = inProcessLocationDetail.PlannedBill;
                        receiptDetail.Receipt = receipt;

                        if (receiptDetail.ShippedQty != 0)//过滤已收满数量
                        {
                            receipt.AddReceiptDetail(receiptDetail);
                        }
                    }
                }
            }

            return receipt;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<InProcessLocationDetail> ConvertTransformerToInProcessLocationDetail(List<Transformer> transformerList)
        {
            return this.ConvertTransformerToInProcessLocationDetail(transformerList, false);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<InProcessLocationDetail> ConvertTransformerToInProcessLocationDetail(List<Transformer> transformerList, bool includeZero)
        {
            IList<InProcessLocationDetail> ipDetList = new List<InProcessLocationDetail>();
            InProcessLocationDetail ipDet = new InProcessLocationDetail();
            if (transformerList != null && transformerList.Count > 0)
            {
                foreach (Transformer transformer in transformerList)
                {
                    OrderLocationTransaction orderLocTrans = orderLocationTransactionMgrE.LoadOrderLocationTransaction(transformer.OrderLocTransId);
                    if (transformer.TransformerDetails != null && transformer.TransformerDetails.Count > 0)
                    {
                        foreach (TransformerDetail transformerDetail in transformer.TransformerDetails)
                        {
                            ipDet = new InProcessLocationDetail();
                            ipDet.OrderLocationTransaction = orderLocTrans;
                            ipDet.HuId = transformerDetail.HuId;
                            ipDet.LotNo = transformerDetail.LotNo;
                            ipDet.QtyToShip = transformerDetail.Qty;
                            ipDet.Qty = transformerDetail.CurrentQty;

                            if (ipDet.Qty != 0 || includeZero)
                                ipDetList.Add(ipDet);
                        }
                    }
                    else
                    {
                        ipDet = new InProcessLocationDetail();
                        ipDet.OrderLocationTransaction = orderLocTrans;
                        ipDet.QtyToShip = transformer.Qty;
                        ipDet.Qty = transformer.CurrentQty;

                        if (ipDet.Qty != 0 || includeZero)
                            ipDetList.Add(ipDet);
                    }
                }
            }

            return ipDetList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<ReceiptDetail> ConvertTransformerToReceiptDetail(List<Transformer> transformerList)
        {
            return this.ConvertTransformerToReceiptDetail(transformerList, false);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<ReceiptDetail> ConvertTransformerToReceiptDetail(List<Transformer> transformerList, bool includeZero)
        {
            IList<ReceiptDetail> recDetList = new List<ReceiptDetail>();
            ReceiptDetail recDet = new ReceiptDetail();
            if (transformerList != null && transformerList.Count > 0)
            {
                foreach (Transformer transformer in transformerList)
                {
                    OrderLocationTransaction orderLocTrans = orderLocationTransactionMgrE.LoadOrderLocationTransaction(transformer.OrderLocTransId);
                    if (transformer.TransformerDetails != null && transformer.TransformerDetails.Count > 0
                        && orderLocTrans.OrderDetail.OrderHead.IsReceiptScanHu)
                    {
                        foreach (TransformerDetail transformerDetail in transformer.TransformerDetails)
                        {
                            recDet = new ReceiptDetail();
                            recDet.OrderLocationTransaction = orderLocTrans;
                            recDet.HuId = transformerDetail.HuId;
                            recDet.LotNo = transformerDetail.LotNo;
                            recDet.ShippedQty = transformerDetail.Qty;
                            recDet.ReceivedQty = transformerDetail.CurrentQty;
                            recDet.PutAwayBinCode = transformerDetail.StorageBinCode;

                            if (recDet.ReceivedQty != 0 || includeZero)
                                recDetList.Add(recDet);
                        }
                    }
                    else
                    {
                        recDet = new ReceiptDetail();
                        recDet.OrderLocationTransaction = orderLocTrans;
                        recDet.ShippedQty = transformer.Qty;
                        recDet.ReceivedQty = transformer.CurrentQty;
                        recDet.RejectedQty = transformer.RejectedQty;
                        recDet.ScrapQty = transformer.ScrapQty;
                        if (recDet.ReceivedQty != 0 || recDet.RejectedQty != 0 || recDet.ScrapQty != 0 || includeZero)
                            recDetList.Add(recDet);
                    }
                }
            }

            return recDetList;
        }


        [Transaction(TransactionMode.Unspecified)]
        public IList<OrderHead> ConvertShiftPlanScheduleToOrders(IList<ShiftPlanSchedule> shiftPlanScheduleList)
        {
            return this.ConvertShiftPlanScheduleToOrders(shiftPlanScheduleList, true);
        }

        public IList<OrderHead> ConvertShiftPlanScheduleToOrders(IList<ShiftPlanSchedule> shiftPlanScheduleList, bool isIncludeMultiDetail)
        {
            IList<OrderHead> orderHeadList = new List<OrderHead>();

            if (shiftPlanScheduleList != null && shiftPlanScheduleList.Count > 0)
            {
                foreach (ShiftPlanSchedule sps in shiftPlanScheduleList)
                {
                    decimal orderLotSize = sps.FlowDetail.OrderLotSize.HasValue ? sps.FlowDetail.OrderLotSize.Value : 0;
                    IList<decimal> reqQtyList = OrderHelper.SplitByOrderLotSize(sps.PlanQty, orderLotSize);
                    foreach (decimal reqQty in reqQtyList)
                    {
                        double leadTime = sps.FlowDetail.Flow.LeadTime.HasValue ? Convert.ToDouble(sps.FlowDetail.Flow.LeadTime.Value) : 0;
                        OrderHead oh = new OrderHead();
                        //oh = this.TransferFlow2Order(sps.FlowDetail.Flow.Code);
                        oh = this.TransferFlowDetail2Order(sps.FlowDetail);
                        oh.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                        oh.StartTime = shiftMgrE.GetShiftStartTime(sps.ReqDate, sps.Shift.Code);
                        oh.WindowTime = shiftMgrE.GetShiftEndTime(sps.ReqDate, sps.Shift.Code);
                        oh.Shift = sps.Shift;

                        if (sps.FlowDetail.Item.Type != BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
                        {
                            oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).RequiredQty = reqQty;
                            oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).OrderedQty = reqQty;
                            if (sps.Remark != null && sps.Remark.Trim() != string.Empty)
                            {
                                //亚虹客户化
                                string[] memo = sps.Remark.Split('|');
                                if (memo.Length > 2)
                                {
                                    oh.OrderDetails[0].Remark = memo[0];
                                    oh.OrderDetails[0].TextField1 = memo[1];
                                    oh.OrderDetails[0].TextField2 = memo[2];
                                }
                                else if (memo.Length == 2)
                                {
                                    oh.OrderDetails[0].Remark = memo[0];
                                    oh.OrderDetails[0].TextField1 = memo[1];
                                }
                                else
                                {
                                    oh.OrderDetails[0].Remark = sps.Remark;
                                }
                                //oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).Remark = sps.Remark;
                            }
                            //oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).TextField2 = sps.TextField2;
                            //oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).TextField1 = sps.TextField1;
                            if (sps.Bom != null)
                            {
                                oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).Bom = sps.Bom;
                            }
                        }
                        else
                        {
                            IList<ItemKit> kitList = this.itemKitMgrE.GetChildItemKit(sps.FlowDetail.Item.Code);
                            if (kitList != null && kitList.Count > 0)
                            {
                                foreach (ItemKit kit in kitList)
                                {
                                    OrderDetail kitOrderDetail = oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, kit.ChildItem.Code, true);
                                    if (kitOrderDetail == null)
                                    {
                                        throw new BusinessErrorException("MasterData.KitItem.NotInFlowDetail", kit.ChildItem.Code);
                                    }
                                    kitOrderDetail.RequiredQty = reqQty * kit.Qty;
                                    kitOrderDetail.OrderedQty = reqQty * kit.Qty;
                                    if (sps.Remark != null && sps.Remark.Trim() != string.Empty)
                                    {
                                        kitOrderDetail.Remark = sps.Remark;
                                    }
                                    //kitOrderDetail.TextField2 = sps.TextField2;
                                    //kitOrderDetail.TextField1 = sps.TextField1;
                                }
                            }
                        }
                        orderHeadList.Add(oh);
                    }
                }
            }

            OrderHelper.FilterZeroOrderQty(orderHeadList);
            //合并OrderHead相同的OrderDetail
            if (isIncludeMultiDetail)
                orderHeadList = this.MergeOrder(orderHeadList);
            return orderHeadList;
        }

        //[Transaction(TransactionMode.Unspecified)]
        //public IList<OrderHead> ConvertShiftPlanScheduleToOrders(IList<ShiftPlanSchedule> shiftPlanScheduleList, bool isIncludeMultiDetail)
        //{
        //    return ConvertShiftPlanScheduleToOrders(shiftPlanScheduleList, decimal.Zero, isIncludeMultiDetail);
        //}

        [Transaction(TransactionMode.Unspecified)]
        public IList<OrderHead> ConvertShiftPlanScheduleToOrders(IList<ShiftPlanSchedule> shiftPlanScheduleList, decimal leadTime, bool isIncludeMultiDetail)
        {
            IList<OrderHead> orderHeadList = new List<OrderHead>();
            if (shiftPlanScheduleList != null && shiftPlanScheduleList.Count > 0)
            {

                foreach (ShiftPlanSchedule sps in shiftPlanScheduleList)
                {
                    decimal orderLotSize = sps.FlowDetail.OrderLotSize.HasValue ? sps.FlowDetail.OrderLotSize.Value : 0;
                    IList<decimal> reqQtyList = OrderHelper.SplitByOrderLotSize(sps.PlanQty, orderLotSize);
                    DateTime startTime = shiftMgrE.GetShiftStartTime(sps.ReqDate, sps.Shift.Code);
                    DateTime windowTime = startTime.AddHours(Convert.ToDouble(leadTime));
                    if (startTime.AddHours((double)(reqQtyList.Count * leadTime)) > shiftMgrE.GetShiftEndTime(sps.ReqDate, sps.Shift.Code))
                    {
                        throw new BusinessErrorException("MasterData.WindowTime.LaterThanShiftEndTime");
                    }
                    foreach (decimal reqQty in reqQtyList)
                    {
                        OrderHead oh = new OrderHead();
                        //oh = this.TransferFlow2Order(sps.FlowDetail.Flow.Code);
                        oh = this.TransferFlowDetail2Order(sps.FlowDetail);
                        oh.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;

                        oh.StartTime = startTime;
                        oh.WindowTime = windowTime;
                        oh.Shift = sps.Shift;

                        oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).RequiredQty = reqQty;
                        oh.GetOrderDetailByFlowDetailIdAndItemCode(sps.FlowDetail.Id, sps.FlowDetail.Item.Code).OrderedQty = reqQty;
                        orderHeadList.Add(oh);

                        startTime = windowTime;
                        windowTime = startTime.AddHours(Convert.ToDouble(leadTime));
                    }
                }
            }

            OrderHelper.FilterZeroOrderQty(orderHeadList);
            //根据选项控制是否需要将相同路线相同班次的合并
            if (isIncludeMultiDetail)
                orderHeadList = this.MergeOrder(orderHeadList);
            return orderHeadList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<OrderHead> ConvertFlowPlanToOrders(IList<FlowPlan> flowPlanList)
        {
            return ConvertFlowPlanToOrders(flowPlanList, true);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<OrderHead> ConvertFlowPlanToOrdersPana(IList<FlowPlan> flowPlanList)
        {
            IList<OrderHead> orderHeadList = new List<OrderHead>();

            if (flowPlanList != null && flowPlanList.Count > 0)
            {
                foreach (FlowPlan flowPlan in flowPlanList)
                {
                    decimal orderLotSize = flowPlan.FlowDetail.OrderLotSize.HasValue ? flowPlan.FlowDetail.OrderLotSize.Value : 0;
                    IList<decimal> reqQtyList = OrderHelper.SplitByOrderLotSize(flowPlan.PlanQty, orderLotSize);
                    foreach (decimal reqQty in reqQtyList)
                    {
                        double leadTime = flowPlan.FlowDetail.Flow.LeadTime.HasValue ? Convert.ToDouble(flowPlan.FlowDetail.Flow.LeadTime.Value) : 0;
                        OrderHead oh = new OrderHead();
                        //oh = this.TransferFlow2Order(flowPlan.FlowDetail.Flow);
                        oh = this.TransferFlowDetail2Order(flowPlan.FlowDetail);
                        oh.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                        oh.StartTime = shiftMgrE.GetShiftStartTime(flowPlan.ReqDate, flowPlan.Shift);
                        oh.WindowTime = shiftMgrE.GetShiftEndTime(flowPlan.ReqDate, flowPlan.Shift);
                        oh.Shift = shiftMgrE.LoadShift(flowPlan.Shift);

                        if (flowPlan.FlowDetail.Item.Type != BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
                        {
                            oh.OrderDetails[0].RequiredQty = reqQty;
                            oh.OrderDetails[0].OrderedQty = reqQty;
                            if (flowPlan.Memo != null && flowPlan.Memo.Trim() != string.Empty)
                            {
                                oh.OrderDetails[0].Remark = flowPlan.Memo;
                            }
                        }
                        else
                        {
                            IList<ItemKit> kitList = this.itemKitMgrE.GetChildItemKit(flowPlan.FlowDetail.Item.Code);
                            if (kitList != null && kitList.Count > 0)
                            {
                                foreach (ItemKit kit in kitList)
                                {
                                    OrderDetail kitOrderDetail = oh.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, kit.ChildItem.Code, true);
                                    if (kitOrderDetail == null)
                                    {
                                        throw new BusinessErrorException("MasterData.KitItem.NotInFlowDetail", kit.ChildItem.Code);
                                    }
                                    kitOrderDetail.RequiredQty = reqQty * kit.Qty;
                                    kitOrderDetail.OrderedQty = reqQty * kit.Qty;
                                    if (flowPlan.Memo != null && flowPlan.Memo.Trim() != string.Empty)
                                    {
                                        kitOrderDetail.Remark = flowPlan.Memo;
                                    }
                                }
                            }
                        }
                        orderHeadList.Add(oh);
                    }
                }
            }

            OrderHelper.FilterZeroOrderQty(orderHeadList);
            //合并OrderHead相同的OrderDetail
            orderHeadList = this.MergeOrder(orderHeadList);
            return orderHeadList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<OrderHead> ConvertFlowPlanToOrders(IList<FlowPlan> flowPlanList, bool isWinTime)
        {
            IList<OrderHead> orderHeadList = new List<OrderHead>();
            var groupPlans = flowPlanList.Where(p => p.PlanQty > 0).GroupBy(p => p.FlowCode);
            foreach (var groupPlan in groupPlans)
            {
                var firstPlan = groupPlan.First();
                var flow = firstPlan.FlowDetail.Flow;
                double leadTime = flow.LeadTime.HasValue ? Convert.ToDouble(flow.LeadTime.Value) : 0;
                var startTime = firstPlan.ReqDate;
                if (isWinTime)
                {
                    startTime = firstPlan.ReqDate.AddHours(-leadTime);
                }
                OrderHead oh = this.TransferFlow2OrderHead(flow, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, startTime);
                foreach (var plan in groupPlan)
                {
                    var orderDetails = orderDetailMgrE.GenerateOrderDetail(oh, plan.FlowDetail);
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.RequiredQty = plan.PlanQty;
                        orderDetail.OrderedQty = plan.PlanQty;
                        orderDetail.Remark = plan.Memo;
                    }
                }
                orderHeadList.Add(oh);
            }
            return orderHeadList;

            if (flowPlanList != null && flowPlanList.Count > 0)
            {
                foreach (FlowPlan flowPlan in flowPlanList)
                {
                    bool isExist = false;
                    foreach (OrderHead orderHead in orderHeadList)
                    {
                        if (orderHead.Flow.Trim().ToUpper() == flowPlan.FlowCode.Trim().ToUpper())
                        {
                            if (flowPlan.FlowDetail.Item.Type != BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
                            {
                                orderHead.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, flowPlan.FlowDetail.Item.Code).RequiredQty = flowPlan.PlanQty;
                                orderHead.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, flowPlan.FlowDetail.Item.Code).OrderedQty = flowPlan.PlanQty;
                                if (flowPlan.Memo != null && flowPlan.Memo.Trim() != string.Empty)
                                {
                                    orderHead.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, flowPlan.FlowDetail.Item.Code).Remark = flowPlan.Memo;
                                }

                                isExist = true;
                                break;
                            }
                            else
                            {
                                IList<ItemKit> kitList = this.itemKitMgrE.GetChildItemKit(flowPlan.FlowDetail.Item.Code);
                                if (kitList != null && kitList.Count > 0)
                                {
                                    foreach (ItemKit kit in kitList)
                                    {
                                        orderHead.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, kit.ChildItem.Code).RequiredQty = flowPlan.PlanQty;
                                        orderHead.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, kit.ChildItem.Code).OrderedQty = flowPlan.PlanQty;
                                        if (flowPlan.Memo != null && flowPlan.Memo.Trim() != string.Empty)
                                        {
                                            orderHead.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, kit.ChildItem.Code).Remark = flowPlan.Memo;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!isExist)
                    {
                        double leadTime = flowPlan.FlowDetail.Flow.LeadTime.HasValue ? Convert.ToDouble(flowPlan.FlowDetail.Flow.LeadTime.Value) : 0;

                        OrderHead oh = new OrderHead();
                        oh = this.TransferFlow2Order(flowPlan.FlowCode);
                        if (isWinTime)
                        {
                            oh.StartTime = flowPlan.ReqDate.AddHours(-leadTime);
                            oh.WindowTime = flowPlan.ReqDate;
                        }
                        else
                        {
                            oh.StartTime = flowPlan.ReqDate;
                            oh.WindowTime = flowPlan.ReqDate.AddHours(leadTime);
                        }
                        //oh = this.TransferFlowDetail2Order(flowPlan.FlowDetail, oh.StartTime, oh.WindowTime);
                        oh.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;

                        if (flowPlan.FlowDetail.Item.Type != BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
                        {
                            oh.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, flowPlan.FlowDetail.Item.Code).RequiredQty = flowPlan.PlanQty;
                            oh.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, flowPlan.FlowDetail.Item.Code).OrderedQty = flowPlan.PlanQty;
                            if (flowPlan.Memo != null && flowPlan.Memo.Trim() != string.Empty)
                            {
                                oh.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, flowPlan.FlowDetail.Item.Code).Remark = flowPlan.Memo;
                            }
                        }
                        else
                        {
                            IList<ItemKit> kitList = this.itemKitMgrE.GetChildItemKit(flowPlan.FlowDetail.Item.Code);
                            if (kitList != null && kitList.Count > 0)
                            {
                                foreach (ItemKit kit in kitList)
                                {
                                    oh.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, kit.ChildItem.Code).RequiredQty = flowPlan.PlanQty;
                                    oh.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, kit.ChildItem.Code).OrderedQty = flowPlan.PlanQty;
                                    if (flowPlan.Memo != null && flowPlan.Memo.Trim() != string.Empty)
                                    {
                                        oh.GetOrderDetailByFlowDetailIdAndItemCode(flowPlan.FlowDetail.Id, kit.ChildItem.Code).Remark = flowPlan.Memo;
                                    }
                                }
                            }
                        }
                        orderHeadList.Add(oh);
                    }
                }
            }

            OrderHelper.FilterZeroOrderQty(orderHeadList);
            return orderHeadList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<OrderHead> ConvertOrderDetailToOrders(IList<OrderDetail> orderDetailList)
        {
            if (orderDetailList == null || orderDetailList.Count == 0)
                throw new BusinessErrorException("Common.Business.Warn.Empty");

            IList<OrderHead> orderHeadList = new List<OrderHead>();
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                OrderHead oh = this.TransferFlow2Order(orderDetail.FlowDetail.Flow.Code);
                oh.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                oh.StartTime = orderDetail.OrderHead.StartTime;
                oh.WindowTime = orderDetail.OrderHead.WindowTime;
                oh.GetOrderDetailBySequence(orderDetail.FlowDetail.Sequence).RequiredQty = orderDetail.RequiredQty;
                oh.GetOrderDetailBySequence(orderDetail.FlowDetail.Sequence).OrderedQty = orderDetail.RequiredQty;

                orderHeadList.Add(oh);
            }

            return orderHeadList;
        }

        //用于精益引擎拆分订单
        [Transaction(TransactionMode.Unspecified)]
        public void CreateOrder(OrderHead orderHead, User user, List<OrderHead> orderHeads, DateTime jobTime)
        {
            this.CreateOrder(orderHead, user);
            foreach (OrderHead od in orderHeads)
            {
                od.DateField1 = jobTime;
                orderHeadMgrE.UpdateOrderHead(od);
            }
        }


        [Transaction(TransactionMode.Unspecified)]
        public IDictionary<string, decimal> FindRMShortageForWO(string orderNo)
        {
            OrderHead orderHead = this.orderHeadMgrE.CheckAndLoadOrderHead(orderNo);

            if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_CUSTOMERGOODS
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING)
            {
                throw new BusinessErrorException("Order.OrderHead.Error.TypeNotCustomergoodsOrProcurement", orderNo);
            }

            if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CANCEL
                || orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE
                || orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE)
            {
                throw new BusinessErrorException("Order.OrderHead.Error.NotNeedCheck", orderNo);
            }

            IDictionary<string, decimal> rmShortageDic = new Dictionary<string, decimal>();
            foreach (OrderDetail orderDetail in orderHead.OrderDetails)
            {
                foreach (OrderLocationTransaction orderLocationTransaction in orderDetail.OrderLocationTransactions)
                {
                    if (orderLocationTransaction.IOType == BusinessConstants.IO_TYPE_OUT)
                    {
                        decimal qty = 0M;
                        if (orderLocationTransaction.OrderDetail.OrderedQty != 0M)
                        {
                            qty = (orderLocationTransaction.OrderedQty / orderLocationTransaction.OrderDetail.OrderedQty) * orderLocationTransaction.OrderDetail.RemainShippedQty;
                        }
                        IDictionary<string, decimal> dics = NestFindRmShortage(orderHead.Flow, orderHead.Type, orderHead.PartyFrom, orderLocationTransaction.Location, orderLocationTransaction.Item, qty, orderHead.StartTime);
                        if (dics != null && dics.Count > 0)
                        {
                            foreach (string key in dics.Keys)
                            {
                                if (!rmShortageDic.ContainsKey(key))
                                {
                                    rmShortageDic.Add(key, dics[key]);
                                }
                                else
                                {
                                    rmShortageDic[key] += dics[key];
                                }
                            }
                        }
                    }
                }
            }

            if (rmShortageDic == null || rmShortageDic.Count == 0)
            {
                //orderHead.CheckRM = true;
                //this.orderHeadMgrE.UpdateOrderHead(orderHead);
            }
            return rmShortageDic;
        }

        #endregion

        #endregion

        #region private方法
        private InProcessLocation ShipOrder(InProcessLocation inProcessLocation, User user, bool checkOrderStatus)
        {
            #region 变量定义
            IList<InProcessLocationDetail> nonZeroInProcessLocationDetailList = new List<InProcessLocationDetail>(); //存储非0发货项
            IDictionary<string, OrderHead> cachedOrderHead = new Dictionary<string, OrderHead>();  //缓存用到的OrderHead
            #endregion

            #region 判断是否全0发货
            if (inProcessLocation.InProcessLocationDetails != null && inProcessLocation.InProcessLocationDetails.Count > 0)
            {
                //判断全0发货
                foreach (InProcessLocationDetail inProcessLocationDetail in inProcessLocation.InProcessLocationDetails)
                {
                    if (inProcessLocationDetail.Qty != 0)
                    {
                        nonZeroInProcessLocationDetailList.Add(inProcessLocationDetail);
                    }
                }

                if (nonZeroInProcessLocationDetailList.Count == 0)
                {
                    throw new BusinessErrorException("OrderDetail.Error.OrderDetailShipEmpty");
                }
                else
                {
                    inProcessLocation.InProcessLocationDetails = nonZeroInProcessLocationDetailList;
                }
            }
            else
            {
                throw new BusinessErrorException("OrderDetail.Error.OrderDetailShipEmpty");
            }
            #endregion

            #region 发货
            DoShipOrder(inProcessLocation, user, checkOrderStatus);
            #endregion

            #region 判断自动收货
            if (inProcessLocation.IsAutoReceive)
            {
                if (inProcessLocation.CompleteLatency.HasValue && inProcessLocation.CompleteLatency.Value > 0)
                {
                    //todo 收货延迟，记录到Quratz表中
                    throw new NotImplementedException("Complete Latency Not Implemented");
                }
                else
                {
                    //立即收货
                    ReceiveFromInProcessLocation(inProcessLocation, user, null, null);
                }
            }
            #endregion

            return inProcessLocation;
        }

        private void DoShipOrder(InProcessLocation inProcessLocation, User user, bool checkOrderStatus)
        {
            #region 变量定义
            IDictionary<string, OrderHead> cachedOrderHead = new Dictionary<string, OrderHead>();  //缓存用到的OrderHead
            IList<InProcessLocationDetail> batchFeedInProcessLocationDetailList = new List<InProcessLocationDetail>();
            #endregion

            #region 更新订单信息
            foreach (InProcessLocationDetail inProcessLocationDetail in inProcessLocation.InProcessLocationDetails)
            {
                OrderLocationTransaction orderLocationTransaction = inProcessLocationDetail.OrderLocationTransaction;
                OrderDetail orderDetail = orderLocationTransaction.OrderDetail;
                OrderHead orderHead = orderDetail.OrderHead;

                if (orderLocationTransaction.BackFlushMethod != BusinessConstants.CODE_MASTER_BACKFLUSH_METHOD_VALUE_BATCH_FEED)
                {
                    #region 判断OrderHead状态并缓存
                    if (!cachedOrderHead.ContainsKey(orderHead.OrderNo))
                    {
                        //检查权限
                        //if (!OrderHelper.CheckOrderOperationAuthrize(orderHead, user, BusinessConstants.ORDER_OPERATION_SHIP_ORDER))
                        //{
                        //    throw new BusinessErrorException("Order.Error.NoShipPermission", orderHead.OrderNo);
                        //}

                        //判断OrderHead状态
                        if (checkOrderStatus && orderHead.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS)
                        {
                            throw new BusinessErrorException("Order.Error.StatusErrorWhenShip", orderHead.Status, orderHead.OrderNo);
                        }

                        #region 整包装收货判断,快速的不要判断
                        if (orderHead.FulfillUnitCount && orderHead.Type != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION
                            && orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML && !(orderHead.IsAutoRelease && orderHead.IsAutoStart))
                        {
                            if (inProcessLocationDetail.Qty % orderDetail.UnitCount != 0)
                            {
                                //不是整包装
                                throw new BusinessErrorException("Order.Error.NotFulfillUnitCountGrGi", orderDetail.Item.Code);
                            }
                        }
                        #endregion

                        //缓存OrderHead
                        cachedOrderHead.Add(orderHead.OrderNo, orderHead);
                    }
                    #endregion

                    #region 更新OrderLocationTransaction、OrderDetail数量
                    this.orderDetailMgrE.RecordOrderShipQty(orderLocationTransaction, inProcessLocationDetail, true);
                    #endregion
                }
                else
                {
                    //只有普通的工单才记录待回冲表，返工不记录。
                    if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
                    {

                        batchFeedInProcessLocationDetailList.Add(inProcessLocationDetail);
                    }
                }
            }
            #endregion

            #region 创建ASN
            this.inProcessLocationMgrE.CreateInProcessLocation(inProcessLocation, user);
            #endregion

            #region 新增投料回冲计划数
            if (batchFeedInProcessLocationDetailList.Count > 0)
            {
                foreach (InProcessLocationDetail inProcessLocationDetail in batchFeedInProcessLocationDetailList)
                {
                    OrderLocationTransaction orderLocationTransaction = inProcessLocationDetail.OrderLocationTransaction;

                    OrderPlannedBackflush orderPlannedBackflush = new OrderPlannedBackflush();
                    orderPlannedBackflush.OrderLocationTransaction = orderLocationTransaction;
                    orderPlannedBackflush.InProcessLocation = inProcessLocation;
                    orderPlannedBackflush.IsActive = true;
                    orderPlannedBackflush.PlannedQty = orderLocationTransaction.UnitQty * inProcessLocationDetail.Qty;
                    orderPlannedBackflush.CreateDate = DateTime.Now;
                    orderPlannedBackflush.CreateUser = user.Code;

                    this.orderPlannedBackflushMgrE.CreateOrderPlannedBackflush(orderPlannedBackflush);
                }
            }
            #endregion

            #region 更新订单头信息
            DateTime dateTimeNow = DateTime.Now;
            foreach (OrderHead orderHead in cachedOrderHead.Values)
            {
                orderHead.LastModifyUser = user;
                orderHead.LastModifyDate = dateTimeNow;
                this.orderHeadMgrE.UpdateOrderHead(orderHead);
            }
            #endregion
        }

        private IList<OrderHead> CreateBindingOrder(OrderHead orderHead, User user, params string[] bindingTypes)
        {
            if (orderHead.SubType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
            {
                return null;
            }

            DateTime dateTimeNow = DateTime.Now;
            IList<OrderHead> orderHeadList = new List<OrderHead>();
            IList<OrderBinding> orderBindingList = this.orderBindingMgrE.GetOrderBinding(orderHead, bindingTypes);

            if (orderBindingList != null && orderBindingList.Count > 0)
            {
                foreach (OrderBinding orderBinding in orderBindingList)
                {
                    OrderHead bindedOrderHead = this.TransferFlow2Order(orderBinding.BindedFlow);
                    bindedOrderHead.OrderDetails = new List<OrderDetail>();

                    foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                    {
                        IList<FlowDetail> bindedFlowDetailList = this.flowBindingMgrE.GetBindedFlowDetail(
                            orderDetail, orderBinding.BindedFlow.Code);

                        if (bindedFlowDetailList != null && bindedFlowDetailList.Count > 0)
                        {
                            foreach (FlowDetail bindedFlowDetail in bindedFlowDetailList)
                            {
                                //对于生产，相同的零件可以出现在不同的工序里，绑定的时候需要合并
                                OrderDetail binedOrderDetail = (from od in bindedOrderHead.OrderDetails where od.Item.Code == bindedFlowDetail.Item.Code select od).SingleOrDefault();

                                if (binedOrderDetail == null)
                                {
                                    //合并生产线上不同成品用到的相同的零件
                                    binedOrderDetail = bindedOrderHead.GetOrderDetailByFlowDetailIdAndItemCode(bindedFlowDetail.Id, bindedFlowDetail.Item.Code);
                                }

                                if (binedOrderDetail != null)
                                {
                                    #region 合并相同的零件
                                    if (binedOrderDetail.Uom.Code != bindedFlowDetail.Uom.Code)
                                    {
                                        decimal qty = this.uomConversionMgrE.ConvertUomQty(binedOrderDetail.Item, bindedFlowDetail.Uom, bindedFlowDetail.OrderedQty, binedOrderDetail.Uom);

                                        binedOrderDetail.OrderedQty += qty;
                                        binedOrderDetail.RequiredQty += qty;
                                    }
                                    else
                                    {
                                        binedOrderDetail.OrderedQty += bindedFlowDetail.OrderedQty;
                                        binedOrderDetail.RequiredQty += bindedFlowDetail.OrderedQty;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    bool isReferenceFlow = (bindedFlowDetail.Flow.Code != orderBinding.BindedFlow.Code);
                                    IList<OrderDetail> newOrderDetailList = this.orderDetailMgrE.GenerateOrderDetail(bindedOrderHead, bindedFlowDetail, isReferenceFlow);
                                    if (newOrderDetailList.Count == 1)
                                    {
                                        //if (bindedFlowDetail.RoundUpOption == "1")
                                        //{
                                        //    newOrderDetailList[0].OrderedQty = Math.Ceiling(bindedFlowDetail.OrderedQty / bindedFlowDetail.UnitCount) * bindedFlowDetail.UnitCount;
                                        //}
                                        //else if (bindedFlowDetail.RoundUpOption == "-1")
                                        //{
                                        //    newOrderDetailList[0].OrderedQty = Math.Floor(bindedFlowDetail.OrderedQty / bindedFlowDetail.UnitCount) * bindedFlowDetail.UnitCount;
                                        //}
                                        //else
                                        //{
                                        newOrderDetailList[0].OrderedQty = bindedFlowDetail.OrderedQty;
                                        //}
                                        newOrderDetailList[0].RequiredQty = bindedFlowDetail.OrderedQty;
                                    }
                                    else if (newOrderDetailList.Count > 1)
                                    {
                                        #region 处理套件的RequiredQty、OrderedQty
                                        IList<ItemKit> itemKitList = this.itemKitMgrE.GetChildItemKit(bindedFlowDetail.Item.Code);

                                        decimal? convertRate = null;
                                        foreach (ItemKit itemKit in itemKitList)
                                        {
                                            if (!convertRate.HasValue)
                                            {
                                                if (itemKit.ParentItem.Uom.Code != bindedFlowDetail.Uom.Code)
                                                {
                                                    convertRate = this.uomConversionMgrE.ConvertUomQty(orderDetail.Item, bindedFlowDetail.Uom, bindedFlowDetail.OrderedQty, itemKit.ParentItem.Uom);
                                                }
                                                else
                                                {
                                                    convertRate = 1;
                                                }
                                            }

                                            foreach (OrderDetail newOrderDetail in newOrderDetailList)
                                            {
                                                if (newOrderDetail.Item.Code == itemKit.ChildItem.Code)
                                                {
                                                    newOrderDetail.RequiredQty = bindedFlowDetail.OrderedQty * itemKit.Qty * convertRate.Value;
                                                    newOrderDetail.OrderedQty = bindedFlowDetail.OrderedQty * itemKit.Qty * convertRate.Value;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                    }

                    if (bindedOrderHead.OrderDetails.Count > 0)
                    {
                        #region 考虑待收待发 已有库存
                        IList<OrderLocTransView> orderLocTransViews = new List<OrderLocTransView>();
                        IList<LocationDetail> locationDetails = new List<LocationDetail>();
                        if (orderBinding.InTrans)
                        {
                            List<string> locList = bindedOrderHead.OrderDetails.Select(o => o.DefaultLocationTo.Code).Distinct().ToList();
                            List<string> itemList = bindedOrderHead.OrderDetails.Select(o => o.Item.Code).ToList();

                            DetachedCriteria criteria = DetachedCriteria.For(typeof(OrderLocTransView));
                            OrderHelper.SetOpenOrderStatusCriteria(criteria, "Status");
                            this.SetInCriteria<string>(criteria, "Location", locList);
                            this.SetInCriteria<string>(criteria, "Item.Code", itemList);
                            orderLocTransViews = criteriaMgrE.FindAll<OrderLocTransView>(criteria);

                            criteria = DetachedCriteria.For(typeof(LocationDetail));
                            criteria.Add(Expression.Not(Expression.Eq("Qty", 0M)));
                            this.SetInCriteria<string>(criteria, "Location.Code", locList);
                            this.SetInCriteria<string>(criteria, "Item.Code", itemList);
                            locationDetails = criteriaMgrE.FindAll<LocationDetail>(criteria);
                        }
                        #endregion

                        //圆整明细
                        foreach (OrderDetail bindedOrderDetail in bindedOrderHead.OrderDetails)
                        {
                            bindedOrderDetail.OrderTracers = new List<OrderTracer>();
                            var q = orderLocTransViews.Where(o => o.Item.Code == bindedOrderDetail.Item.Code
                                    && o.Location == bindedOrderDetail.DefaultLocationTo.Code);
                            if (q != null && q.Count() > 0)
                            {
                                //orderTracer
                                foreach (OrderLocTransView orderLocTransView in q)
                                {
                                    OrderTracer orderTracer = new OrderTracer();
                                    orderTracer.OrderedQty = orderLocTransView.PlanQty;
                                    orderTracer.Qty = orderLocTransView.PlanQty - orderLocTransView.AccumQty;
                                    orderTracer.Code = orderLocTransView.OrderNo;
                                    orderTracer.OrderDetail = bindedOrderDetail;
                                    orderTracer.Item = orderLocTransView.Item.Code;
                                    orderTracer.ReqTime = orderLocTransView.StartTime;
                                    orderTracer.RefId = orderLocTransView.Id;
                                    if (orderLocTransView.IOType == BusinessConstants.IO_TYPE_IN)
                                    {
                                        orderTracer.TracerType = BusinessConstants.ORDERTRACER_TRACERTYPE_ORDERRCT;
                                        bindedOrderDetail.OrderedQty -= orderTracer.Qty;
                                    }
                                    else
                                    {
                                        orderTracer.TracerType = BusinessConstants.ORDERTRACER_TRACERTYPE_ORDERISS;
                                        bindedOrderDetail.OrderedQty += orderTracer.Qty;
                                    }

                                    bindedOrderDetail.OrderTracers.Add(orderTracer);
                                }
                            }

                            var q3 = locationDetails.Where(l => l.Item.Code == bindedOrderDetail.Item.Code
                                && l.Location.Code == bindedOrderDetail.DefaultLocationTo.Code);

                            if (q3 != null && q3.Count() > 0)
                            {
                                bindedOrderDetail.OrderedQty -= q3.Sum(l => l.Qty);
                                //OrderTracer
                                OrderTracer orderTracer = new OrderTracer();
                                orderTracer.TracerType = BusinessConstants.ORDERTRACER_TRACERTYPE_ONHANDINV;

                                //orderTracer.OrderedQty = bindedOrderDetail.OrderedQty;
                                orderTracer.Qty = q3.First().Qty;
                                orderTracer.Code = q3.First().Location.Code;
                                orderTracer.OrderDetail = bindedOrderDetail;
                                orderTracer.Item = q3.First().Item.Code;
                                orderTracer.ReqTime = dateTimeNow;
                                orderTracer.RefId = q3.First().Id;

                                bindedOrderDetail.OrderTracers.Add(orderTracer);
                            }

                            bindedOrderDetail.OrderedQty = bindedOrderDetail.OrderedQty > 0 ? bindedOrderDetail.OrderedQty : 0;

                            if (bindedOrderDetail.FlowDetail.RoundUpOption == "1")
                            {
                                bindedOrderDetail.OrderedQty = Math.Ceiling(bindedOrderDetail.OrderedQty / bindedOrderDetail.UnitCount) * bindedOrderDetail.UnitCount;
                            }
                            else if (bindedOrderDetail.FlowDetail.RoundUpOption == "-1")
                            {
                                bindedOrderDetail.OrderedQty = Math.Floor(bindedOrderDetail.OrderedQty / bindedOrderDetail.UnitCount) * bindedOrderDetail.UnitCount;
                            }
                        }

                        Decimal leadTime = orderBinding.BindedFlow.LeadTime.HasValue ? orderBinding.BindedFlow.LeadTime.Value : 0;
                        bindedOrderHead.ReferenceOrderNo = orderHead.OrderNo;
                        bindedOrderHead.StartTime = orderHead.StartTime.AddHours(-double.Parse(leadTime.ToString()));
                        bindedOrderHead.WindowTime = orderHead.StartTime;
                        bindedOrderHead.Priority = BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL;
                        //新南港需要把备注copy到绑定订单
                        bindedOrderHead.Memo = orderHead.Memo;

                        #region 生产要把班次加上
                        if (bindedOrderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                        {
                            bindedOrderHead.Shift = shiftMgrE.GetDefaultShift();
                        }
                        #endregion

                        if (orderBinding.BindingType == BusinessConstants.CODE_MASTER_BINDING_TYPE_VALUE_RECEIVE_SYN)
                        {
                            bindedOrderHead.IsAutoRelease = true;
                            bindedOrderHead.IsAutoStart = true;
                            bindedOrderHead.IsAutoShip = true;
                            bindedOrderHead.IsAutoReceive = true;
                            bindedOrderHead.StartLatency = 0;
                            bindedOrderHead.CompleteLatency = 0;
                        }
                        else if (orderBinding.BindingType == BusinessConstants.CODE_MASTER_BINDING_TYPE_VALUE_RECEIVE_ASYN)
                        {
                            bindedOrderHead.IsAutoRelease = true;
                            bindedOrderHead.StartLatency = 0;
                        }

                        //绑定的订单用管理员账号建订单
                        OrderHelper.FilterZeroOrderQty(bindedOrderHead);
                        if (bindedOrderHead.OrderDetails != null && bindedOrderHead.OrderDetails.Count > 0)
                        {
                            this.CreateOrder(bindedOrderHead, userMgrE.GetMonitorUser());

                            orderBinding.BindedOrderHead = bindedOrderHead;
                            this.orderBindingMgrE.UpdateOrderBinding(orderBinding);

                            orderHeadList.Add(bindedOrderHead);
                        }
                    }
                }
            }

            return orderHeadList;
        }

        private void a(List<string> locList, List<string> itemList)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(OrderLocTransView));
            OrderHelper.SetOpenOrderStatusCriteria(criteria, "Status");
            this.SetInCriteria<string>(criteria, "Location", locList);
            this.SetInCriteria<string>(criteria, "Item.Code", itemList);
            IList<OrderLocTransView> orderLocTransViews = criteriaMgrE.FindAll<OrderLocTransView>(criteria);

            criteria.Add(Expression.Not(Expression.Eq("Qty", 0M)));
            this.SetInCriteria<string>(criteria, "Location.Code", locList);
            this.SetInCriteria<string>(criteria, "Item.Code", itemList);
            IList<LocationDetail> locationDetails = criteriaMgrE.FindAll<LocationDetail>(criteria);

        }

        private void SetInCriteria<T>(DetachedCriteria criteria, string propertyName, List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                if (list.Count == 1)
                {
                    criteria.Add(Expression.Eq(propertyName, list[0]));
                }
                else
                {
                    criteria.Add(Expression.InG<T>(propertyName, list));
                }
            }
        }

        private void CreateOrderDetailSubsidiary(OrderDetail orderDetail)
        {
            //CheckDetOpt选项
            if (orderDetailMgrE.CheckOrderDet(orderDetail))
            {
                orderDetailMgrE.CreateOrderDetail(orderDetail);

                if (orderDetail.OrderLocationTransactions != null && orderDetail.OrderLocationTransactions.Count > 0)
                {
                    string costGroupFrom = null;
                    if (orderDetail.OrderHead.PartyFrom.GetType() == typeof(Region))
                    {
                        costGroupFrom = this.costCenterMgr.CheckAndLoadCostCenter(((Region)orderDetail.OrderHead.PartyFrom).CostCenter).CostGroup.Code;
                    }

                    string costGroupTo = null;
                    if (orderDetail.OrderHead.PartyTo.GetType() == typeof(Region))
                    {
                        costGroupTo = this.costCenterMgr.CheckAndLoadCostCenter(((Region)orderDetail.OrderHead.PartyTo).CostCenter).CostGroup.Code;
                    }

                    foreach (OrderLocationTransaction orderLocationTransaction in orderDetail.OrderLocationTransactions)
                    {
                        if (orderLocationTransaction.IOType == BusinessConstants.IO_TYPE_IN
                            && costGroupTo != null)
                        {
                            IsLocationInCostGroup(orderLocationTransaction, costGroupTo);

                        }
                        else if (orderLocationTransaction.IOType == BusinessConstants.IO_TYPE_OUT
                            && costGroupFrom != null)
                        {
                            IsLocationInCostGroup(orderLocationTransaction, costGroupFrom);
                        }

                        orderLocationTransactionMgrE.CreateOrderLocationTransaction(orderLocationTransaction);
                    }
                }

                #region Order Tracer
                //旧代码
                if (orderDetail.AutoOrderTracks != null && orderDetail.AutoOrderTracks.Count > 0)
                {
                    foreach (AutoOrderTrack autoOrderTrack in orderDetail.AutoOrderTracks)
                    {
                        autoOrderTrackMgrE.CreateAutoOrderTrack(autoOrderTrack);
                    }
                }

                //新代码
                if (orderDetail.OrderTracers != null && orderDetail.OrderTracers.Count > 0)
                {
                    foreach (var orderTracer in orderDetail.OrderTracers)
                    {
                        orderTracerMgrE.CreateOrderTracer(orderTracer);
                    }
                }
                #endregion
            }
            else
            {
                throw new BusinessErrorException("Master.CheckDetOpt.Error");
            }
        }

        /**
        * 分摊OrderHead头折扣 
        */
        private void AllocateOrderHeadDicount(OrderHead orderHead)
        {

            if (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING
                || orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            {
                #region 折扣分摊至明细
                EntityPreference entityPreference = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_AMOUNT_DECIMAL_LENGTH);

                int decimalLength = int.Parse(entityPreference.Value);

                IList<OrderDetail> noneZeroUnitPriceFromOrderDetailList = new List<OrderDetail>();
                foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                {
                    if (orderDetail.UnitPrice != Decimal.Zero)
                    {
                        noneZeroUnitPriceFromOrderDetailList.Add(orderDetail);
                    }
                }

                decimal orderDiscount = orderHead.Discount.HasValue ? orderHead.Discount.Value : 0;
                decimal remainDiscount = orderDiscount;
                for (int i = 0; i < noneZeroUnitPriceFromOrderDetailList.Count; i++)
                {
                    OrderDetail orderDetail = orderDetailMgrE.LoadOrderDetail(noneZeroUnitPriceFromOrderDetailList[i].Id);

                    if (i < noneZeroUnitPriceFromOrderDetailList.Count - 1)
                    {
                        if (orderHead.OrderDetailAmountAfterDiscount != Decimal.Zero)
                        {
                            orderDetail.HeadDiscount = Math.Round(orderDiscount * orderDetail.OrderDetailAmountAfterDiscount / orderHead.OrderDetailAmountAfterDiscount, decimalLength, MidpointRounding.AwayFromZero);
                        }

                        if (orderDetail.HeadDiscount.HasValue)
                        {
                            remainDiscount -= orderDetail.HeadDiscount.Value;
                        }
                    }
                    else
                    {
                        orderDetail.HeadDiscount = remainDiscount;
                    }

                    if (orderDetail.UnitPrice.HasValue)
                    {
                        orderDetail.UnitPriceAfterDiscount = (orderDetail.UnitPrice * orderDetail.OrderedQty
                           - (orderDetail.Discount.HasValue ? orderDetail.Discount.Value : 0)
                           - (orderDetail.HeadDiscount.HasValue ? orderDetail.HeadDiscount.Value : 0))
                           / orderDetail.OrderedQty;

                        if (orderDetail.UnitPriceAfterDiscount != null && orderDetail.UnitPriceAfterDiscount.HasValue)
                        {
                            orderDetail.UnitPriceAfterDiscount = Math.Round(orderDetail.UnitPriceAfterDiscount.Value, decimalLength, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            orderDetail.UnitPriceAfterDiscount = 0;
                        }
                    }

                    this.orderDetailMgrE.UpdateOrderDetail(orderDetail);
                }
                #endregion
            }
        }

        //private void CalculatePlannedAmount(OrderDetail orderDetail, ReceiptDetail receiptDetail, decimal? totalAmount)
        //{
        //    if (totalAmount.HasValue && totalAmount.Value != 0)
        //    {
        //        EntityPreference decimalLengthEntityPreference = this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_AMOUNT_DECIMAL_LENGTH);
        //        int decimalLength = int.Parse(decimalLengthEntityPreference.Value);
        //        decimal actualUnitPrice = Math.Round((totalAmount.Value / orderDetail.OrderedQty), decimalLength, MidpointRounding.AwayFromZero);

        //        if (orderDetail.ReceivedQty.Value + receiptDetail.ReceivedQty.Value > orderDetail.OrderedQty)
        //        {
        //            //已收数 + 本次收货数大于订单数量
        //            if (orderDetail.ReceivedQty.Value >= orderDetail.OrderedQty)
        //            {
        //                //已收数大于订单数量，PlannedAmount = 剩余订单Amount + 实际单价 * 超出订单的数量
        //                receiptDetail.PlannedAmount = totalAmount.Value - actualUnitPrice * orderDetail.ReceivedQty.Value;
        //                receiptDetail.PlannedAmount += actualUnitPrice * (orderDetail.ReceivedQty.Value + receiptDetail.ReceivedQty.Value - orderDetail.OrderedQty);
        //            }
        //            else
        //            {
        //                //已收数大于订单数量，PlannedAmount = 实际单价 * 本次收货数量
        //                receiptDetail.PlannedAmount = actualUnitPrice * receiptDetail.ReceivedQty.Value;
        //            }
        //        }
        //        else if (orderDetail.ReceivedQty + receiptDetail.ReceivedQty.Value == orderDetail.OrderedQty)
        //        {
        //            //已收数 + 本次收货数正好等于订单数量
        //            receiptDetail.PlannedAmount = totalAmount.Value - actualUnitPrice * orderDetail.ReceivedQty.Value;
        //        }
        //        else
        //        {
        //            //已收数 + 本次收货数小于订单数量
        //            receiptDetail.PlannedAmount = actualUnitPrice * receiptDetail.ReceivedQty.Value;
        //        }
        //    }
        //}

        private Receipt ReceiveFromInProcessLocation(InProcessLocation inProcessLocation, User user, IDictionary<string, string> huIdStorageBinDic, string externalOrderNo)
        {
            if (inProcessLocation.OrderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                throw new TechnicalException("production can't auto receive after shipping raw marterial");
            }

            Receipt receipt = this.ConvertInProcessLocationToReceipt(inProcessLocation, huIdStorageBinDic, externalOrderNo);
            return this.ReceiveOrder(receipt, user);
        }

        private int GetInPorcessWOCount(string flowCode, User user)
        {
            DetachedCriteria criteria = DetachedCriteria.For<OrderHead>();
            criteria.SetProjection(Projections.Count("OrderNo"));

            criteria.Add(Expression.Eq("Flow", flowCode));
            criteria.Add(Expression.Eq("StartUser", user));
            criteria.Add(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS));

            return this.criteriaMgrE.FindAll<int>(criteria)[0];
        }

        //OrderHead有且仅有一条明细
        private IList<OrderHead> MergeOrder(IList<OrderHead> orderHeadList)
        {
            if (orderHeadList == null)
            {
                return null;
            }
            IList<OrderHead> newOrderHeadList = new List<OrderHead>();
            int i = orderHeadList.Select(o => o.IsMark).Where(m => !m).ToList().Count();
            while (i > 0)
            {
                OrderHead orderHead = null;
                foreach (OrderHead oh in orderHeadList)
                {
                    if (!oh.IsMark)
                    {
                        if (orderHead == null)
                        {
                            orderHead = oh;
                            oh.IsMark = true;
                            continue;
                        }
                        if (orderHead.Flow == oh.Flow && orderHead.Shift == oh.Shift)
                        {
                            OrderDetail od = oh.OrderDetails[0];
                            //List<string> items = orderHead.OrderDetails.Select(o => o.Item.Code).ToList();
                            var q = orderHead.OrderDetails.Where(o => o.Item.Code == od.Item.Code && o.UnitCount == od.UnitCount);
                            if (oh.OrderDetails.Count > 0 && (q == null || q.Count() == 0))
                            {
                                orderHead.OrderDetails.Add(od);
                                oh.IsMark = true;
                            }
                        }
                    }
                }
                i = orderHeadList.Select(o => o.IsMark).Where(m => !m).ToList().Count();
                newOrderHeadList.Add(orderHead);
            }
            return newOrderHeadList;
        }

        private bool IsLocationInCostGroup(OrderLocationTransaction orderLocationTransaction, string costGroupCode)
        {
            if (orderLocationTransaction.Location != null
                    && this.costCenterMgr.CheckAndLoadCostCenter(orderLocationTransaction.Location.Region.CostCenter).CostGroup.Code != costGroupCode)
            {
                throw new BusinessErrorException("Cost.Error.LocationNotInCostGroup", orderLocationTransaction.Location.Code, costGroupCode);
            }

            if (orderLocationTransaction.RejectLocation != null && orderLocationTransaction.RejectLocation.Trim() != string.Empty
            && this.costCenterMgr.CheckAndLoadCostCenter(this.locationMgrE.CheckAndLoadLocation(orderLocationTransaction.RejectLocation).Region.CostCenter).CostGroup.Code != costGroupCode)
            {
                throw new BusinessErrorException("Cost.Error.LocationNotInCostGroup", orderLocationTransaction.RejectLocation, costGroupCode);
            }

            if (orderLocationTransaction.InspectLocation != null && orderLocationTransaction.InspectLocation.Trim() != string.Empty
            && this.costCenterMgr.CheckAndLoadCostCenter(this.locationMgrE.CheckAndLoadLocation(orderLocationTransaction.InspectLocation).Region.CostCenter).CostGroup.Code != costGroupCode)
            {
                throw new BusinessErrorException("Cost.Error.LocationNotInCostGroup", orderLocationTransaction.InspectLocation, costGroupCode);
            }

            return true;
        }

        public OrderHead TransferFlowDetail2Order(FlowDetail flowDetail)
        {
            return TransferFlowDetail2Order(flowDetail, DateTime.Now, DateTime.Now);
        }

        private OrderHead TransferFlowDetail2Order(FlowDetail flowDetail, DateTime startTime, DateTime winTime)
        {
            #region 创建OrderHead
            OrderHead orderHead = new OrderHead();
            CloneHelper.CopyProperty(flowDetail.Flow, orderHead, FlowHead2OrderHeadCloneFields);

            orderHead.Flow = flowDetail.Flow.Code;
            orderHead.StartTime = startTime;
            orderHead.WindowTime = winTime;
            orderHead.SubType = BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML;
            #endregion

            #region 创建OrderDetail
            this.orderDetailMgrE.GenerateOrderDetail(orderHead, flowDetail, false);
            #endregion

            return orderHead;
        }

        private IDictionary<string, decimal> NestFindRmShortage(string flowCode, string orderType, Party region, Location location, Item item, decimal orderedQty, DateTime startTime)
        {
            IDictionary<string, decimal> rmShortageDic = new Dictionary<string, decimal>();

            IList<string> rmLocList = new List<string>();
            rmLocList.Add(location.Code);

            #region 查找上层库位
            DetachedCriteria criteria = DetachedCriteria.For<FlowDetail>();
            criteria.CreateAlias("Flow", "f");
            criteria.CreateAlias("LocationTo", "lt", NHibernate.SqlCommand.JoinType.LeftOuterJoin);
            criteria.CreateAlias("LocationFrom", "lf", NHibernate.SqlCommand.JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.LocationTo", "flt", NHibernate.SqlCommand.JoinType.LeftOuterJoin);
            criteria.CreateAlias("f.LocationFrom", "flf", NHibernate.SqlCommand.JoinType.LeftOuterJoin);

            criteria.Add(Expression.Eq("f.Type", BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_TRANSFER));
            criteria.Add(Expression.Eq("f.IsActive", true));
            criteria.Add(Expression.Eq("Item", item));
            criteria.Add(Expression.Or(Expression.Eq("lt.Code", location.Code),
            Expression.And(Expression.IsNull("LocationTo"), Expression.Eq("flt.Code", location.Code))));

            criteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("lf.Code")).Add(Projections.GroupProperty("flf.Code")));

            IList<object[]> locFromList = this.criteriaMgrE.FindAll<object[]>(criteria);
            if (locFromList != null && locFromList.Count > 0)
            {
                foreach (object[] locFrom in locFromList)
                {
                    if (locFrom != null && locFrom[0] != null && ((string)locFrom[0]).Trim() != string.Empty)
                    {
                        rmLocList.Add((string)locFrom[0]);
                    }
                    else if (locFrom != null && locFrom[1] != null && ((string)locFrom[1]).Trim() != string.Empty)
                    {
                        rmLocList.Add((string)locFrom[1]);
                    }
                }
            }
            #endregion

            #region 查找可用库存
            criteria = DetachedCriteria.For<LocationLotDetail>();

            criteria.Add(Expression.Eq("Item", item));
            criteria.Add(Expression.In("Location.Code", rmLocList.ToArray()));
            criteria.Add(Expression.Not(Expression.Eq("Qty", decimal.Zero)));

            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Sum("Qty")));

            IList qtyList = this.criteriaMgrE.FindAll(criteria);
            decimal locQty = 0;
            if (qtyList != null && qtyList.Count > 0 && qtyList[0] != null)
            {
                locQty = (decimal)qtyList[0];
            }
            #endregion

            #region 查找相关工单消耗
            decimal woQtyIss = 0;
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                criteria = DetachedCriteria.For<OrderLocationTransaction>();

                criteria.CreateAlias("OrderDetail", "od");
                criteria.CreateAlias("od.OrderHead", "oh");

                criteria.Add(Expression.Eq("IOType", BusinessConstants.IO_TYPE_OUT));
                criteria.Add(Expression.Eq("oh.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION));
                criteria.Add(Expression.Eq("oh.SubType", BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML));
                criteria.Add(Expression.In("oh.Status", new string[] { BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT, BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS }));
                criteria.Add(Expression.Eq("Item", item));
                criteria.Add(Expression.Eq("Location", location));

                criteria.SetProjection(Projections.ProjectionList().Add(Projections.Sum("OrderedQty")).Add(Projections.Sum("AccumulateQty")).Add(Projections.Sum("AccumulateRejectQty")));

                IList<object[]> woQtyIssList = this.criteriaMgrE.FindAll<object[]>(criteria);

                if (woQtyIssList != null && woQtyIssList.Count > 0 && woQtyIssList[0] != null)
                {
                    woQtyIss = (woQtyIssList[0][0] != null ? (decimal)woQtyIssList[0][0] : 0) - (woQtyIssList[0][1] != null ? (decimal)woQtyIssList[0][1] : 0) - (woQtyIssList[0][2] != null ? (decimal)woQtyIssList[0][2] : 0);
                }
            }
            #endregion

            #region 查找相关工单待收
            decimal woQtyRct = 0;
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
            {
                criteria = DetachedCriteria.For<OrderLocationTransaction>();

                criteria.CreateAlias("OrderDetail", "od");
                criteria.CreateAlias("od.OrderHead", "oh");

                criteria.Add(Expression.Eq("IOType", BusinessConstants.IO_TYPE_IN));
                criteria.Add(Expression.Eq("oh.Type", BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION));
                criteria.Add(Expression.Eq("oh.SubType", BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML));
                criteria.Add(Expression.In("oh.Status", new string[] { BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT, BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS }));
                criteria.Add(Expression.Eq("Item", item));
                criteria.Add(Expression.Eq("Location", location));

                criteria.SetProjection(Projections.ProjectionList().Add(Projections.Sum("OrderedQty")).Add(Projections.Sum("AccumulateQty")).Add(Projections.Sum("AccumulateRejectQty")));

                IList<object[]> woQtyRctList = this.criteriaMgrE.FindAll<object[]>(criteria);


                if (woQtyRctList != null && woQtyRctList.Count > 0 && woQtyRctList[0] != null)
                {
                    woQtyRct = (woQtyRctList[0][0] != null ? (decimal)woQtyRctList[0][0] : 0) - (woQtyRctList[0][1] != null ? (decimal)woQtyRctList[0][1] : 0) - (woQtyRctList[0][2] != null ? (decimal)woQtyRctList[0][2] : 0);
                }
            }
            #endregion

            #region 判断是否缺料
            decimal diffQty = locQty - orderedQty - woQtyIss + woQtyRct;
            if (diffQty >= 0)
            {
                //满足所有工单需求
            }
            else
            {
                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                {
                    //把零件作为半成品往下分解
                    IDictionary<string, decimal> dics = FindSemiProductRmShortage(flowCode, region, item, startTime, -diffQty);
                    if (dics != null && dics.Count > 0)
                    {
                        foreach (string key in dics.Keys)
                        {
                            if (!rmShortageDic.ContainsKey(key))
                            {
                                rmShortageDic.Add(key, dics[key]);
                            }
                            else
                            {
                                rmShortageDic[key] += dics[key];
                            }
                        }
                    }
                }
                else
                {
                    rmShortageDic.Add(item.Code, -diffQty);
                }
            }
            #endregion

            return rmShortageDic;
        }

        private IDictionary<string, decimal> FindSemiProductRmShortage(string flowCode, Party region, Item item, DateTime startTime, Decimal semiQty)
        {
            decimal itemQty = semiQty;
            IDictionary<string, decimal> rmShortageDic = new Dictionary<string, decimal>();

            DetachedCriteria criteria = DetachedCriteria.For<FlowDetail>();

            criteria.CreateAlias("Flow", "f");

            criteria.Add(Expression.Eq("f.Type", BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION));
            criteria.Add(Expression.Eq("f.IsActive", true));
            criteria.Add(Expression.Eq("Item", item));
            criteria.Add(Expression.Eq("f.PartyFrom", region));

            IList<FlowDetail> fdList = this.criteriaMgrE.FindAll<FlowDetail>(criteria);
            if (fdList != null && fdList.Count > 0)
            {
                //优先查找相同生产线
                FlowDetail flowDetail = (from fd in fdList
                                         where fd.Flow.Code == flowCode
                                         select fd).SingleOrDefault();

                if (flowDetail == null)
                {
                    flowDetail = fdList[0];
                }

                #region 查找Bom
                Bom bom = flowDetail.Bom;

                if (bom == null)
                {
                    bom = flowDetail.Item.Bom;
                }

                if (bom == null)
                {
                    bom = this.bomMgr.CheckAndLoadBom(item.Code);
                }
                #endregion

                #region Bom单位转换
                if (bom.Uom.Code != item.Uom.Code)
                {
                    itemQty = this.uomConversionMgrE.ConvertUomQty(item.Code, item.Uom.Code, itemQty, bom.Uom.Code);  //先把半成品转为Bom上的成品单位
                }
                #endregion

                #region 分解Bom，循环校验物料
                IList<BomDetail> bomDetailList = this.bomDetailMgr.GetFlatBomDetail(bom.Code, startTime);

                if (bomDetailList != null && bomDetailList.Count > 0)
                {
                    Routing routing = flowDetail.Routing != null ? flowDetail.Routing : flowDetail.Flow.Routing;
                    IList<RoutingDetail> routingDetailList = null;
                    if (routing != null)
                    {
                        routingDetailList = this.routingDetailMgr.GetRoutingDetail(routing.Code, startTime);
                    }

                    foreach (BomDetail bomDetail in bomDetailList)
                    {
                        #region 取库位
                        Location rmLocation = flowDetail.DefaultLocationFrom;  //默认库位
                        if (bomDetail.Location != null)
                        {
                            rmLocation = bomDetail.Location;
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
                                    rmLocation = location;
                                }
                            }
                        }
                        #endregion

                        #region 计算用量
                        decimal rmQty = itemQty //半成品用量                                  
                               * bomDetail.RateQty //乘以单位用量
                               * (1 + bomDetail.DefaultScrapPercentage);  //乘以损耗

                        if (bomDetail.Item.Uom.Code != bomDetail.Uom.Code)
                        {
                            //转换为库存单位
                            rmQty = this.uomConversionMgrE.ConvertUomQty(bomDetail.Item.Code, bomDetail.Uom.Code, rmQty, bomDetail.Item.Uom.Code);
                        }
                        #endregion

                        IDictionary<string, decimal> dics = NestFindRmShortage(flowCode, BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION, region, rmLocation, bomDetail.Item, rmQty, startTime);
                        if (dics != null && dics.Count > 0)
                        {
                            foreach (string key in dics.Keys)
                            {
                                if (!rmShortageDic.ContainsKey(key))
                                {
                                    rmShortageDic.Add(key, dics[key]);
                                }
                                else
                                {
                                    rmShortageDic[key] += dics[key];
                                }
                            }
                        }
                        //IDictionaryHelper.AddRange<string, decimal>(rmShortageDic, NestFindRmShortage(flowCode, region, rmLocation, bomDetail.Item, rmQty, startTime));
                    }
                }
                #endregion
            }
            else
            {
                rmShortageDic.Add(item.Code, itemQty);
            }

            return rmShortageDic;
        }

        private string GetEntityPreference(IList<EntityPreference> entityPreferences, string code)
        {
            var q = entityPreferences.Where(en => StringHelper.Eq(en.Code, code));
            if (q != null && q.Count() > 0)
            {
                return q.First().Value;
            }
            return string.Empty;
        }

        private void SendEmail(OrderHead orderHead, User user)
        {
            #region 发送邮件通知

            string supplierEmail = string.Empty;
            if (orderHead.ShipFrom != null && orderHead.ShipFrom.Email != null)
            {
                supplierEmail = orderHead.ShipFrom.Email.Trim();
            }
            if (supplierEmail != string.Empty && (orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ||
                orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING))
            {
                string shipTo = orderHead.ShipTo.Address == null ? string.Empty : orderHead.ShipTo.Address;
                IList<EntityPreference> entityPreferences = entityPreferenceMgrE.GetAllEntityPreference();
                string EnableMailToSupplier = this.GetEntityPreference(entityPreferences, BusinessConstants.ENTITY_PREFERENCE_CODE_ENABLEMAILTOSUPPLIER);
                if (EnableMailToSupplier.ToLower() == "true")
                {
                    try
                    {
                        string subject = string.Empty;
                        string emailFrom = this.GetEntityPreference(entityPreferences, BusinessConstants.ENTITY_PREFERENCE_CODE_SMTPEMAILADDR);
                        string userMail = emailFrom;
                        if (user.Email != null && user.Email.Trim() != string.Empty)
                        {
                            userMail = user.Email.Trim();
                        }
                        string companyName = this.GetEntityPreference(entityPreferences, BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME);

                        subject = companyName + "给您下了采购订单:" + orderHead.OrderNo;

                        string mailBody = "尊敬的" + orderHead.PartyFrom.Name + ":<br />";
                        mailBody += "我们给您下了采购订单:" + orderHead.OrderNo + "<br />";
                        mailBody += "请于:" + orderHead.WindowTime.ToString("yyyy-MM-dd HH:mm") + "送达本公司" + shipTo + "<br /><br />";
                        mailBody += "<table cellspacing='0' cellpadding='4' rules='all' border='1' style='border-collapse:collapse;font-size:12px;'>";
                        mailBody += "<tr style='color:#FFFFFF;background-color:#000000;font-weight:bold;line-height:150%;'>";
                        mailBody += "<th scope='col'>物料号</th><th scope='col'>物料描述</th><th scope='col'>单位</th><th scope='col'>订单数</th></tr>";
                        foreach (OrderDetail orderDetail in orderHead.OrderDetails)
                        {
                            mailBody += "<tr><td>" + orderDetail.Item.Code + "</td><td>" + orderDetail.Item.Description + "</td><td>"
                                + orderDetail.Uom.Name + "</td><td>" + orderDetail.OrderedQty.ToString("0.####") + "</td></tr>";
                        }
                        mailBody += "</table><br />";
                        mailBody += "重要:本邮件只是采购提示,具体订单需求以本公司的网站订单为准.<br />";
                        mailBody += "谢谢合作!<br /><br />";
                        mailBody += user.Name == null ? string.Empty : user.Name;
                        mailBody += "<br />电话:" + (user.MobliePhone == null ? string.Empty : user.MobliePhone);
                        mailBody += " " + (user.Phone == null ? string.Empty : user.Phone) + "<br />";
                        mailBody += companyName + "<br />";
                        mailBody += "网站:<a href='http://sconit.yahong-mould.com'>http://sconit.yahong-mould.com</a>";

                        string SMTPEmailHost = this.GetEntityPreference(entityPreferences, BusinessConstants.ENTITY_PREFERENCE_CODE_SMTPEMAILHOST);
                        string SMTPEmailPasswd = this.GetEntityPreference(entityPreferences, BusinessConstants.ENTITY_PREFERENCE_CODE_SMTPEMAILPASSWD);

                        SMTPHelper.SendSMTPEMail(subject, mailBody, emailFrom, supplierEmail, SMTPEmailHost, SMTPEmailPasswd, userMail);
                        //smtpMgrE.AsyncSend2(subject, mailBody, supplierEmail, userMail, MailPriority.Normal, null);
                    }
                    catch
                    {

                    }
                }
            }
            #endregion

        }


        private void CheckFacilityAllocate(IList<OrderDetail> orderDetailList)
        {
            #region 检查模具数是否符合条件
            var facilityAllocatesList = hqlMgr.FindAll<FacilityAllocates>("from FacilityAllocates where IsActive = 1");
            foreach (OrderDetail orderDetail in orderDetailList)
            {
                if (!string.IsNullOrEmpty(orderDetail.TextField3))
                {
                    var facilityAllocates = facilityAllocatesList.Where(p => p.ItemCode == orderDetail.Item.Code && p.FCID == orderDetail.TextField3).FirstOrDefault();
                    if (facilityAllocates == null)
                    {
                        throw new BusinessErrorException("Order.Error.FacilityAllocatesNotExist", orderDetail.Item.Code, orderDetail.TextField3);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}



#region Extend Class

namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class OrderMgrE : com.Sconit.Service.MasterData.Impl.OrderMgr, IOrderMgrE
    {

    }
}

#endregion