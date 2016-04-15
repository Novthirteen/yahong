using com.Sconit.Service.Ext.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using Castle.Services.Transaction;
using com.Sconit.Utility;

namespace com.Sconit.Service.Business.Impl
{
    public class OnlineMgr : AbstractBusinessMgr
    {
        public ISetBaseMgrE setBaseMgrE { get; set; }
        public ISetDetailMgrE setDetailMgrE { get; set; }
        public IExecuteMgrE executeMgrE { get; set; }
        public IOrderMgrE orderMgrE { get; set; }
        public IFlowMgrE flowMgrE { get; set; }
        public IOrderHeadMgrE orderHeadMgrE { get; set; }
        public IOrderDetailMgrE orderDetailMgrE { get; set; }


        protected override void SetBaseInfo(Resolver resolver)
        {
            this.StartOrder(resolver);
        }

        protected override void GetDetail(Resolver resolver)
        {
        }

        protected override void SetDetail(Resolver resolver)
        {
            this.StartOrder(resolver);
        }

        protected override void ExecuteSubmit(Resolver resolver)
        {
        }

        protected override void ExecuteCancel(Resolver resolver)
        {
        }

        private void StartOrder(Resolver resolver)
        {
            if (resolver.CodePrefix == BusinessConstants.CODE_PREFIX_ORDER)
            {
                setBaseMgrE.FillResolverByOrder(resolver);

                #region 校验
                if (resolver.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
                    throw new BusinessErrorException("Common.Business.Error.StatusError", resolver.Code, resolver.Status);

                if (resolver.OrderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION)
                    throw new BusinessErrorException("Order.Error.OrderOfflineIsNotProduction", resolver.Code, resolver.OrderType);
                #endregion

                orderMgrE.StartOrder(resolver.Input, resolver.UserCode);
                resolver.Result = DateTime.Now.ToString("HH:mm:ss");
                resolver.Code = resolver.Input;
            }
            else if(resolver.CodePrefix == BusinessConstants.BARCODE_HEAD_FLOW)
            {
                try
                {
                    Flow flow = flowMgrE.CheckAndLoadFlow(resolver.Input);
                    if (flow.Type != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION)
                        throw new BusinessErrorException("Flow.Error.FlowTypeIsNotProduction", resolver.CodePrefix);
                    else
                    {
                        IList<OrderHead> orderHeadList = orderHeadMgrE.GetSubmitOrderHead(resolver.Input);
                        IList<OrderDetail> orderDetailList = orderDetailMgrE.GetOrderDetail(orderHeadList);
                        resolver.Transformers = TransformerHelper.ConvertOrderDetailListToTransformers(orderDetailList, false);
                    }
                }
                catch (Exception ex)
                {
                    resolver.Result = ex.Message;
                }
            }
            else
            {
                throw new BusinessErrorException("Common.Business.Error.BarCodeInvalid");
            }

        }

        [Transaction(TransactionMode.Unspecified)]
        protected override void ExecutePrint(Resolver resolver)
        {
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override void GetReceiptNotes(Resolver resolver)
        {


        }
    }
}





#region Extend Class
namespace com.Sconit.Service.Ext.Business.Impl
{
    public partial class OnlineMgrE : com.Sconit.Service.Business.Impl.OnlineMgr, IBusinessMgrE
    {

    }
}

#endregion
