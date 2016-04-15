using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Procurement;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using NHibernate.Expression;
using System.Linq;
using com.Sconit.Entity.View;
using com.Sconit.Utility;

namespace com.Sconit.Service.Procurement.Impl
{
    [Transactional]
    public class SupplyChainMgr : ISupplyChainMgr
    {
        public ICriteriaMgrE CriteriaMgrE { get; set; }
        public IFlowMgrE FlowMgrE { get; set; }
        public IBomDetailMgrE BomDetailMgrE { get; set; }


        #region Public Method

        [Transaction(TransactionMode.Unspecified)]
        public IList<SupplyChain> GenerateSupplyChain(string flowCode, string itemCode)
        {
            IList<SupplyChain> supplyChainList = new List<SupplyChain>();
            Flow flow = FlowMgrE.LoadFlow(flowCode, true);
            if (flow.FlowDetails != null && flow.FlowDetails.Count > 0)
            {
                foreach (FlowDetail flowDetail in flow.FlowDetails)
                {
                    if (flowDetail.Item.Code == itemCode)
                    {
                        supplyChainList.Add(this.GenerateSupplyChain(flowDetail));
                    }
                }
            }
            else
            {
                if (flow.ReferenceFlow != null && flow.ReferenceFlow.Trim() != string.Empty)
                {
                    Flow refFlow = this.FlowMgrE.LoadFlow(flow.ReferenceFlow, true);
                    if (refFlow.FlowDetails != null && refFlow.FlowDetails.Count > 0)
                    {
                        foreach (FlowDetail flowDetail in refFlow.FlowDetails)
                        {
                            if (flowDetail.Item.Code == itemCode)
                            {
                                supplyChainList.Add(this.GenerateSupplyChain(flow, flowDetail));
                            }
                        }
                    }
                }
            }

            return supplyChainList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public SupplyChain GenerateSupplyChain(FlowDetail flowDetail)
        {
            return this.GenerateSupplyChain(flowDetail.Flow, flowDetail);
        }

        [Transaction(TransactionMode.Unspecified)]
        public SupplyChain GenerateSupplyChain(Flow flow, FlowDetail flowDetail)
        {
            SupplyChain supplyChain = new SupplyChain();
            supplyChain.Flow = flow;
            supplyChain.FlowDetail = flowDetail;

            SupplyChainDetail supplyChainDetail = new SupplyChainDetail();
            supplyChainDetail.SupplyChain = supplyChain;
            supplyChainDetail.Id = 1;
            supplyChainDetail.ParentId = 0;
            supplyChainDetail.Flow = flow;
            supplyChainDetail.FlowDetail = flowDetail;
            supplyChainDetail.LocationTo = flowDetail.DefaultLocationTo == null ? null : flowDetail.DefaultLocationTo;
            supplyChainDetail.QuantityPer = 1;

            IList<SupplyChainDetail> supplyChainDetailList = new List<SupplyChainDetail>();
            supplyChainDetailList.Add(supplyChainDetail);
            this.GetSupplyChainDetail(supplyChainDetailList, supplyChainDetail);

            supplyChain.AddRangeSupplyChainDetail(supplyChainDetailList);
            return supplyChain;
        }

        #endregion

        #region Private Method

        [Transaction(TransactionMode.Unspecified)]
        private void GetSupplyChainDetail(IList<SupplyChainDetail> supplyChainDetailList, SupplyChainDetail parentSupplyChainDetail)
        {
            if (parentSupplyChainDetail.Flow.Type == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION)
            {
                string bomCode = this.GetBomCode(parentSupplyChainDetail.FlowDetail);
                IList<BomDetail> bomDetailList = BomDetailMgrE.GetFlatBomDetail(bomCode, DateTime.Now);
                if (bomDetailList != null && bomDetailList.Count > 0)
                {
                    foreach (BomDetail bomDetail in bomDetailList)
                    {
                        string itemCode = bomDetail.Item.Code;
                        string locFrom = string.Empty;
                        if (bomDetail.Location != null)
                        {
                            locFrom = bomDetail.Location.Code;
                        }
                        else
                        {
                            if (parentSupplyChainDetail.FlowDetail.DefaultLocationFrom == null)
                            {
                                //end
                                continue;
                            }
                            else
                            {
                                locFrom = parentSupplyChainDetail.FlowDetail.DefaultLocationFrom.Code;
                            }
                        }
                        decimal QtyPer = bomDetail.RateQty * (1 + bomDetail.DefaultScrapPercentage);

                        IList<FlowDetail> flowDetailList = this.GetFlowDetailList(itemCode, locFrom);
                        this.FillSupplyChainDetail(supplyChainDetailList, parentSupplyChainDetail, flowDetailList, QtyPer, locFrom);
                    }
                }
            }
            else
            {
                string locFrom = this.GetLocFrom(parentSupplyChainDetail);
                if (locFrom != null)
                {
                    decimal QtyPer = 1;
                    IList<FlowDetail> flowDetailList = this.GetFlowDetailList(parentSupplyChainDetail.FlowDetail.Item.Code, locFrom);
                    this.FillSupplyChainDetail(supplyChainDetailList, parentSupplyChainDetail, flowDetailList, QtyPer, locFrom);
                }
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        private void FillSupplyChainDetail(IList<SupplyChainDetail> supplyChainDetailList, SupplyChainDetail parentSupplyChainDetail, IList<FlowDetail> flowDetailList, decimal QtyPer, string locFrom)
        {
            SupplyChainDetail supplyChainDetail = new SupplyChainDetail();

            if (flowDetailList != null && flowDetailList.Count > 0)
            {
                foreach (FlowDetail flowDetail in flowDetailList)
                {
                    supplyChainDetail = new SupplyChainDetail();
                    supplyChainDetail.SupplyChain = parentSupplyChainDetail.SupplyChain;
                    supplyChainDetail.Id = supplyChainDetailList.Count + 1;
                    supplyChainDetail.ParentId = parentSupplyChainDetail.Id;
                    supplyChainDetail.Flow = flowDetail.Flow;
                    supplyChainDetail.FlowDetail = flowDetail;
                    supplyChainDetail.LocationTo = flowDetail.DefaultLocationTo == null ? null : flowDetail.DefaultLocationTo;
                    supplyChainDetail.QuantityPer = QtyPer;

                    if (supplyChainDetailList.Select(s => s.FlowDetail.Id).Contains(flowDetail.Id))
                    {
                        //throw new BusinessErrorException("Visualization.SupplyChainRouting.Error.Loop", supplyChainDetailList[0].FlowDetail.Flow.Code, flowDetail.Flow.Code, flowDetail.Item.Code);
                    }

                    if (supplyChainDetailList.Contains(supplyChainDetail))
                    {
                        continue;
                    }
                    else
                    {
                        supplyChainDetailList.Add(supplyChainDetail);
                        this.GetSupplyChainDetail(supplyChainDetailList, supplyChainDetail);
                    }

                    IList<Flow> flowList = this.GetReferenceFlow(flowDetail, locFrom);
                    if (flowList != null && flowList.Count > 0)
                    {
                        //ReferenceFlow
                        foreach (Flow flow in flowList)
                        {
                            if (flow.ReferenceFlow != null && flow.ReferenceFlow == flowDetail.Flow.Code)
                            {
                                supplyChainDetail = new SupplyChainDetail();
                                supplyChainDetail.SupplyChain = parentSupplyChainDetail.SupplyChain;
                                supplyChainDetail.Id = supplyChainDetailList.Count + 1;
                                supplyChainDetail.ParentId = parentSupplyChainDetail.Id;
                                supplyChainDetail.Flow = flow;
                                supplyChainDetail.FlowDetail = flowDetail;
                                supplyChainDetail.LocationTo = flowDetail.DefaultLocationTo == null ? null : flowDetail.DefaultLocationTo;
                                supplyChainDetail.QuantityPer = QtyPer;

                                if (supplyChainDetailList.Contains(supplyChainDetail))
                                {
                                    continue;
                                }
                                else
                                {
                                    supplyChainDetailList.Add(supplyChainDetail);
                                    this.GetSupplyChainDetail(supplyChainDetailList, supplyChainDetail);
                                }
                            }
                        }
                    }
                }
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        private string GetBomCode(FlowDetail flowDetail)
        {
            string bomCode = string.Empty;
            if (flowDetail.Bom != null)
            {
                bomCode = flowDetail.Bom.Code;
            }
            else
            {
                bomCode = flowDetail.Item.Bom == null ? flowDetail.Item.Code : flowDetail.Item.Bom.Code;
            }

            return bomCode;
        }

        [Transaction(TransactionMode.Unspecified)]
        private string GetLocFrom(SupplyChainDetail supplyChainDetail)
        {
            string locFrom = null;
            if (supplyChainDetail.Flow.Code == supplyChainDetail.FlowDetail.Flow.Code)
            {
                locFrom = supplyChainDetail.FlowDetail.DefaultLocationFrom == null ? null : supplyChainDetail.FlowDetail.DefaultLocationFrom.Code;
            }
            else
            {
                //ReferenceFlow
                locFrom = supplyChainDetail.Flow.LocationFrom == null ? null : supplyChainDetail.Flow.LocationFrom.Code;
            }

            return locFrom;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FlowDetail> GetFlowDetailList(string itemCode, string locTo)
        {
            IList<FlowDetail> returnFlowDetailList = new List<FlowDetail>();
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FlowDetail));
            criteria.Add(Expression.Eq("Item.Code", itemCode));
            IList<FlowDetail> flowDetailList = CriteriaMgrE.FindAll<FlowDetail>(criteria);

            if (flowDetailList != null && flowDetailList.Count > 0)
            {
                foreach (FlowDetail flowDetail in flowDetailList)
                {
                    if (flowDetail.DefaultLocationTo != null)
                    {
                        if (flowDetail.Item.Code == itemCode && flowDetail.DefaultLocationTo.Code == locTo)
                        {
                            returnFlowDetailList.Add(flowDetail);
                        }
                    }
                }
            }

            return returnFlowDetailList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Flow> GetReferenceFlow(FlowDetail flowDetail, string locTo)
        {
            IList<Flow> refFlowList = new List<Flow>();
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Flow));
            criteria.Add(Expression.IsNotNull("ReferenceFlow"));
            IList<Flow> flowList = CriteriaMgrE.FindAll<Flow>(criteria);
            if (flowList != null && flowList.Count > 0)
            {
                foreach (Flow flow in flowList)
                {
                    if (flow.LocationTo != null)
                    {
                        if (flow.LocationTo.Code == locTo && flowDetail.Flow.Code == flow.ReferenceFlow)
                        {
                            refFlowList.Add(flow);
                            break;
                        }
                    }
                }
            }
            return refFlowList;
        }

        /// ///////////

        [Transaction(TransactionMode.Unspecified)]
        public IList<SupplyChain> GenerateSupplyChainUp(string flowCode, string itemCode)
        {
            IList<SupplyChain> supplyChainList = new List<SupplyChain>();

            DetachedCriteria criteria = DetachedCriteria.For(typeof(FlowView));
            criteria.CreateAlias("FlowDetail", "fd");
            criteria.CreateAlias("fd.Item", "item");
            criteria.CreateAlias("Flow", "flow");
            criteria.Add(Expression.Eq("item.Code", itemCode));
            criteria.Add(Expression.Eq("flow.Code", flowCode));
            IList<FlowView> flowViewList = CriteriaMgrE.FindAll<FlowView>(criteria);

            if (flowViewList != null && flowViewList.Count == 1)
            {
                FlowView flowView = flowViewList.Single();
                Flow flow = flowView.Flow;
                FlowDetail flowDetail = flowView.FlowDetail;

                SupplyChain supplyChain = new SupplyChain();
                supplyChain.Flow = flow;
                supplyChain.FlowDetail = flowDetail;

                SupplyChainDetail supplyChainDetail = new SupplyChainDetail();
                supplyChainDetail.SupplyChain = supplyChain;
                supplyChainDetail.Id = 1;
                supplyChainDetail.ParentId = 0;
                supplyChainDetail.Flow = flow;
                supplyChainDetail.FlowDetail = flowDetail;
                supplyChainDetail.LocationFrom = flowDetail.DefaultLocationFrom;
                supplyChainDetail.LocationTo = flowDetail.DefaultLocationTo;
                supplyChainDetail.QuantityPer = 1;

                IList<SupplyChainDetail> supplyChainDetailList = new List<SupplyChainDetail>();
                supplyChainDetailList.Add(supplyChainDetail);

                #region bomdetail
                List<BomDetail> bomDetails = this.GetBomDetails(flowDetail.DefaultBomCode, DateTime.Now);
                #endregion

                this.GetSupplyChainDetailUp(supplyChainDetailList, bomDetails, supplyChainDetail);

                supplyChain.AddRangeSupplyChainDetail(supplyChainDetailList);

                supplyChainList.Add(supplyChain);
            }

            return supplyChainList;
        }

        [Transaction(TransactionMode.Unspecified)]
        private void FillSupplyChainDetailUp(IList<SupplyChainDetail> supplyChainDetailList, List<BomDetail> bomDetails,
            SupplyChainDetail parentSupplyChainDetail, Dictionary<Flow, FlowDetail> fdDic, decimal QtyPer, string LocFrom)
        {
            double leadTime1 = parentSupplyChainDetail.Flow.LeadTime.HasValue ? (double)parentSupplyChainDetail.Flow.LeadTime.Value : 0;
            SupplyChainDetail supplyChainDetail = new SupplyChainDetail();

            if (fdDic != null && fdDic.Count > 0)
            {
                foreach (KeyValuePair<Flow, FlowDetail> dic in fdDic)
                {
                    double leadTime2 = dic.Key.LeadTime.HasValue ? (double)dic.Key.LeadTime.Value : 0;

                    supplyChainDetail = new SupplyChainDetail();
                    supplyChainDetail.SupplyChain = parentSupplyChainDetail.SupplyChain;
                    supplyChainDetail.Id = supplyChainDetailList.Count + 1;
                    supplyChainDetail.ParentId = parentSupplyChainDetail.Id;
                    supplyChainDetail.Flow = dic.Key;
                    supplyChainDetail.FlowDetail = dic.Value;
                    supplyChainDetail.LocationTo = dic.Value.DefaultLocationTo;
                    supplyChainDetail.LocationFrom = dic.Value.DefaultLocationFrom;
                    supplyChainDetail.QuantityPer = QtyPer;
                    supplyChainDetail.LeadTime = leadTime2 + leadTime1;

                    if (supplyChainDetailList.Contains(supplyChainDetail))
                    {
                        continue;
                    }
                    else
                    {
                        supplyChainDetailList.Add(supplyChainDetail);
                        this.GetSupplyChainDetailUp(supplyChainDetailList, bomDetails, supplyChainDetail);
                    }
                }
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        private void GetSupplyChainDetailUp(IList<SupplyChainDetail> supplyChainDetailList, List<BomDetail> bomDetails, SupplyChainDetail parentSupplyChainDetail)
        {
            if (parentSupplyChainDetail.LocationTo != null)
            {
                decimal qtyPer = 1;
                string locFrom = parentSupplyChainDetail.LocationTo.Code;
                string itemCode = parentSupplyChainDetail.FlowDetail.Item.Code;
                //locTo作为locFrom查找路线
                Dictionary<Flow, FlowDetail> fdDic = GetFlows(itemCode, locFrom, false);
                if (fdDic != null && fdDic.Count > 0)
                {
                    this.FillSupplyChainDetailUp(supplyChainDetailList, bomDetails, parentSupplyChainDetail, fdDic, qtyPer, locFrom);
                }
                else if (bomDetails != null)
                {
                    var q = bomDetails.Where(b => StringHelper.Eq(b.Item.Code, itemCode));
                    if (q != null && q.Count() > 0)
                    {
                        foreach (BomDetail bomDetail in q)
                        {
                            fdDic = GetFlows(bomDetail.Bom.Code, locFrom, true);
                            this.FillSupplyChainDetailUp(supplyChainDetailList, bomDetails, parentSupplyChainDetail, fdDic, qtyPer, locFrom);
                        }
                    }
                }
            }
        }

        private List<BomDetail> GetBomDetails(string itemCode, DateTime effTime)
        {
            DetachedCriteria detachedCriteria = DetachedCriteria.For<BomDetail>();
            detachedCriteria.Add(Expression.Eq("Item.Code", itemCode));
            detachedCriteria.Add(Expression.Le("StartDate", effTime));
            detachedCriteria.Add(Expression.Or(Expression.Ge("EndDate", effTime), Expression.IsNull("EndDate")));

            List<BomDetail> bomDetails = CriteriaMgrE.FindAll<BomDetail>(detachedCriteria).ToList();

            if (bomDetails != null)
            {
                List<BomDetail> newBomDetails = new List<BomDetail>();
                foreach (BomDetail bomDetail in bomDetails)
                {
                    List<BomDetail> childBomDetails = GetBomDetails(bomDetail.Bom.Code, effTime);
                    if (childBomDetails != null)
                    {
                        newBomDetails.AddRange(childBomDetails);
                    }
                }
                bomDetails.AddRange(newBomDetails);
            }
            return bomDetails;
        }

        [Transaction(TransactionMode.Unspecified)]
        private Dictionary<Flow, FlowDetail> GetFlows(string itemCode, string locFrom, bool isProduction)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FlowView));
            criteria.CreateAlias("FlowDetail", "fd");
            criteria.CreateAlias("fd.Item", "item");
            criteria.CreateAlias("Flow", "flow");
            criteria.CreateAlias("LocationFrom", "locfrom");
            criteria.Add(Expression.Eq("item.Code", itemCode));
            criteria.Add(Expression.Eq("locfrom.Code", locFrom));
            if (isProduction)
            {
                criteria.Add(Expression.Eq("flow.Type", BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION));
            }
            else
            {
                criteria.Add(Expression.Not(Expression.Eq("flow.Type", BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION)));
            }

            IList<FlowView> flowViewList = CriteriaMgrE.FindAll<FlowView>(criteria);
            if (flowViewList != null)
            {
                Dictionary<Flow, FlowDetail> fdDic = new Dictionary<Flow, FlowDetail>();
                foreach (FlowView flowView in flowViewList)
                {
                    fdDic.Add(flowView.Flow, flowView.FlowDetail);
                }
                return fdDic;
            }
            return null;
        }

        #endregion
    }
}



#region Extend Class

namespace com.Sconit.Service.Ext.Procurement.Impl
{
    [Transactional]
    public partial class SupplyChainMgrE : com.Sconit.Service.Procurement.Impl.SupplyChainMgr, ISupplyChainMgrE
    {

    }
}
#endregion
