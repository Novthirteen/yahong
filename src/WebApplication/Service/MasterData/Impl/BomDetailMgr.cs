using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using NHibernate.Expression;
using System.Linq;
using com.Sconit.Entity.View;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class BomDetailMgr : BomDetailBaseMgr, IBomDetailMgr
    {
        #region 变量
        public ICriteriaMgrE criterialMgrE { get; set; }
        public IUomConversionMgrE uomConversionMgrE { get; set; }
        public IBomMgrE bomMgrE { get; set; }
        public IItemMgrE itemMgrE { get; set; }
        public IRoutingMgrE routingMgrE { get; set; }
        public IRoutingDetailMgrE routingDetailMgrE { get; set; }
        //public IFlowMgrE flowMgrE { get; set; }

        private string[] BomCompDetail = new string[] { 
            "Item", 
            "Operation", 
            "Reference", 
            "StructureType", 
            "StartDate", 
            "EndDate", 
            "Uom", 
            "RateQty", 
            "ScrapPercentage", 
            "NeedPrint", 
            "Priority", 
            "Location", 
            "IsShipScanHu", 
            "HuLotSize", 
            "OptionalItemGroup",
            "CalculatedQty",
            "MrpMark",
            "LeadTime",
            "BomLevel"
        };
        #endregion

        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetNextLevelBomDetail(string bomCode, DateTime efftiveDate)
        {
            //NullableDateTime nullableEffDate = new NullableDateTime(efftiveDate);

            DetachedCriteria detachedCriteria = DetachedCriteria.For<BomDetail>();
            detachedCriteria.Add(Expression.Eq("Bom.Code", bomCode));
            detachedCriteria.Add(Expression.Le("StartDate", efftiveDate));
            detachedCriteria.Add(Expression.Or(Expression.Ge("EndDate", efftiveDate), Expression.IsNull("EndDate")));

            IList<BomDetail> bomDetailList = criterialMgrE.FindAll<BomDetail>(detachedCriteria);

            return this.GetNoOverloadBomDetail(bomDetailList);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetFlatBomDetail(string bomCode, DateTime efftiveDate)
        {
            IList<BomDetail> flatBomDetailList = new List<BomDetail>();
            IList<BomDetail> nextBomDetailList = this.GetNextLevelBomDetail(bomCode, efftiveDate);

            if (nextBomDetailList != null && nextBomDetailList.Count > 0)
            {
                foreach (BomDetail nextBomDetail in nextBomDetailList)
                {
                    nextBomDetail.CalculatedQty = nextBomDetail.RateQty * (1 + nextBomDetail.DefaultScrapPercentage);
                    ProcessCurrentBomDetail(flatBomDetailList, nextBomDetail, efftiveDate);
                }
            }
            return flatBomDetailList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool CheckUniqueExist(string parCode, string compCode, int Operation, string Reference, DateTime startTime)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(BomDetail));
            criteria.Add(Expression.Eq("Bom.Code", parCode));
            criteria.Add(Expression.Eq("Item.Code", compCode));
            criteria.Add(Expression.Eq("Operation", Operation));
            criteria.Add(Expression.Eq("Reference", Reference));
            criteria.Add(Expression.Ge("StartDate", startTime));

            IList bomDetails = criterialMgrE.FindAll(criteria);
            if (bomDetails.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetTreeBomDetail(string bomCode, DateTime effDate)
        {
            IList<BomDetail> treeBomDetailList = new List<BomDetail>();
            IList<BomDetail> nextBomDetailList = this.GetNextLevelBomDetail(bomCode, effDate);
            if (nextBomDetailList != null && nextBomDetailList.Count > 0)
            {
                foreach (BomDetail nextBomDetail in nextBomDetailList)
                {
                    nextBomDetail.CalculatedQty = nextBomDetail.RateQty * (1 + nextBomDetail.DefaultScrapPercentage);
                    nextBomDetail.AccumQty = nextBomDetail.CalculatedQty;
                    nextBomDetail.BomLevel = 1;
                }
                treeBomDetailList = this.GetAllBomDetailTree(nextBomDetailList, effDate);
            }

            return this.GetNoOverloadBomDetail(treeBomDetailList);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetTopLevelBomDetail(string itemCode, DateTime effDate)
        {
            IList<BomDetail> returnBomDetailList = new List<BomDetail>();
            //IList<BomDetail> bomDetailList = new List<BomDetail>();
            //IList<BomDetail> lastLevelBomDetailList = new List<BomDetail>();
            //lastLevelBomDetailList = this.GetLastLevelBomDetail(itemCode, effDate);
            //if (lastLevelBomDetailList != null && lastLevelBomDetailList.Count > 0)
            //{
            //    foreach (BomDetail bomDetail in lastLevelBomDetailList)
            //    {
            //        bomDetailList = this.GetTopLevelBomDetail(bomDetail.Bom.Code, effDate);
            //        if (lastLevelBomDetailList != null && lastLevelBomDetailList.Count > 0)
            //        {
            //            foreach (BomDetail bd in bomDetailList)
            //            {
            //                returnBomDetailList.Add(bd);
            //            }
            //        }
            //        else
            //        {
            //            returnBomDetailList.Add(bomDetail);
            //        }
            //    }
            //}

            return returnBomDetailList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetLastLevelBomDetail(string itemCode, DateTime effDate)
        {
            //NullableDateTime nullableEffDate = new NullableDateTime(effDate);

            DetachedCriteria criteria = DetachedCriteria.For<BomDetail>();
            criteria.Add(Expression.Eq("Item.Code", itemCode));
            criteria.Add(Expression.Le("StartDate", effDate));
            criteria.Add(Expression.Or(Expression.Ge("EndDate", effDate), Expression.IsNull("EndDate")));

            IList<BomDetail> bomDetailList = criterialMgrE.FindAll<BomDetail>(criteria);

            return this.GetNoOverloadBomDetail(bomDetailList);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetBomView_Nml(Item item, DateTime effDate)
        {
            IList<BomDetail> bomViewList = new List<BomDetail>();
            IList<BomDetail> bomDetailList = new List<BomDetail>();
            IList<BomDetail> lastLevelBomDetailList = this.GetLastLevelBomDetail(item.Code, effDate);
            if (lastLevelBomDetailList != null && lastLevelBomDetailList.Count > 0)
            {
                foreach (BomDetail bomDetail in lastLevelBomDetailList)
                {
                    bomDetailList = this.GetNextLevelBomDetail(bomDetail.Bom.Code, effDate);
                    foreach (BomDetail bd in bomDetailList)
                    {
                        bd.CalculatedQty = bd.RateQty * (1 + bd.DefaultScrapPercentage);
                        bomViewList.Add(bd);
                    }
                }
            }

            string nextLevelBomCode = (item.Bom != null ? item.Bom.Code : item.Code);
            bomDetailList = this.GetNextLevelBomDetail(nextLevelBomCode, effDate);
            if (bomDetailList != null && bomDetailList.Count > 0)
            {
                foreach (BomDetail bd in bomDetailList)
                {
                    bd.CalculatedQty = bd.RateQty * (1 + bd.DefaultScrapPercentage);
                    bomViewList.Add(bd);
                }
            }

            return this.GetNoOverloadBomDetail(bomViewList);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetBomView_Cost(string itemCode, DateTime effDate)
        {
            Item item = itemMgrE.LoadItem(itemCode);
            string bomCode = item.Bom != null ? item.Bom.Code : item.Code;
            IList<BomDetail> bomDetailList = this.GetFlatBomDetail(bomCode, effDate);
            IList<BomDetail> bomViewList = new List<BomDetail>();
            IList<BomDetail> costBomViewList = new List<BomDetail>();
            bomViewList = this.GetAllBomDetailTree(bomDetailList, effDate);
            bomViewList = this.GetNoOverloadBomDetail(bomViewList);
            if (bomViewList != null && bomViewList.Count > 0)
            {
                bomViewList = this.GetCostBomDetail(bomViewList);
                Bom bom = bomMgrE.LoadBom(bomCode);
                foreach (BomDetail bomDetail in bomViewList)
                {
                    BomDetail costBomDetail = new BomDetail();
                    CloneHelper.CopyProperty(bomDetail, costBomDetail, BomCompDetail);
                    costBomDetail.Bom = bom;

                    costBomViewList.Add(costBomDetail);
                }
            }

            return this.GetNoOverloadBomDetail(costBomViewList);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(Item abstractItem, Routing routing, DateTime effDate)
        {
            return GetDefaultBomDetailForAbstractItem(abstractItem, routing, effDate, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(string abstractItemCode, Routing routing, DateTime effDate)
        {
            return GetDefaultBomDetailForAbstractItem(abstractItemCode, routing, effDate, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(Item abstractItem, Routing routing, DateTime effDate, Location defaultLocationFrom)
        {
            string bomCode = this.bomMgrE.FindBomCode(abstractItem);
            IList<BomDetail> bomDetailList = this.GetNextLevelBomDetail(bomCode, effDate);
            if (bomDetailList != null && bomDetailList.Count > 0)
            {
                bomDetailList = IListHelper.Sort<BomDetail>(bomDetailList, "Priority"); //根据Priority进行排序

                foreach (BomDetail bomDetail in bomDetailList)
                {
                    #region 来源库位查找逻辑BomDetail-->RoutingDetail-->defaultLocationFrom
                    //defaultLocationFrom = FlowDetail-->Flow
                    Location bomLocation = bomDetail.Location;

                    if (bomLocation == null)
                    {
                        RoutingDetail routingDetail = routingDetailMgrE.LoadRoutingDetail(routing, bomDetail.Operation, bomDetail.Reference);
                        if (routingDetail != null)
                        {
                            if (bomLocation == null)
                            {
                                bomLocation = routingDetail.Location;
                            }
                        }
                    }

                    if (bomLocation == null)
                    {
                        bomLocation = defaultLocationFrom;
                    }
                    #endregion

                    //如果没有找到库位，直接跳到下一个bomDetail
                    if (bomLocation != null)
                    {
                        if (!bomLocation.AllowNegativeInventory)
                        {
                            //不允许负库存
                            //todo 检查库存
                            throw new NotImplementedException();
                        }
                        else
                        {
                            //允许负库存，直接返回
                            return bomDetail;
                        }
                    }
                }
            }

            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetBomDetailListForAbstractItem(Item abstractItem, Routing routing, DateTime effDate, Location defaultLocationFrom)
        {
            string bomCode = this.bomMgrE.FindBomCode(abstractItem);

            return GetBomDetailListForAbstractItem(routing, bomCode, effDate, defaultLocationFrom);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetBomDetailListForAbstractItem(Routing routing, string bomCode, DateTime effDate, Location defaultLocationFrom)
        {
            IList<BomDetail> bomDetailList = this.GetNextLevelBomDetail(bomCode, effDate);
            if (bomDetailList != null && bomDetailList.Count > 0)
            {
                bomDetailList = IListHelper.Sort<BomDetail>(bomDetailList, "Priority"); //根据Priority进行排序

                foreach (BomDetail bomDetail in bomDetailList)
                {
                    #region 来源库位查找逻辑BomDetail-->RoutingDetail-->defaultLocationFrom
                    //defaultLocationFrom = FlowDetail-->Flow
                    Location bomLocation = bomDetail.Location;

                    if (bomLocation == null)
                    {
                        RoutingDetail routingDetail = routingDetailMgrE.LoadRoutingDetail(routing, bomDetail.Operation, bomDetail.Reference);
                        if (routingDetail != null)
                        {
                            if (bomLocation == null)
                            {
                                bomLocation = routingDetail.Location;
                            }
                        }
                    }

                    if (bomLocation == null)
                    {
                        bomLocation = defaultLocationFrom;
                    }
                    bomDetail.Location = bomLocation;
                    #endregion
                }
            }

            return bomDetailList;
        }


        [Transaction(TransactionMode.Unspecified)]
        public IList<BomDetail> GetBomDetailListForAbstractItem(string abstractItemCode, Routing routing, DateTime effDate, Location defaultLocationFrom)
        {
            Item abstractItem = this.itemMgrE.LoadItem(abstractItemCode);
            return GetBomDetailListForAbstractItem(abstractItem, routing, effDate, defaultLocationFrom);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(string abstractItemCode, Routing routing, DateTime effDate, Location defaultLocationFrom)
        {
            Item abstractItem = this.itemMgrE.LoadItem(abstractItemCode);
            return GetDefaultBomDetailForAbstractItem(abstractItem, routing, effDate, defaultLocationFrom);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(Item abstractItem, string routingCode, DateTime effDate)
        {
            return GetDefaultBomDetailForAbstractItem(abstractItem, routingCode, effDate, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(string abstractItemCode, string routingCode, DateTime effDate)
        {
            return GetDefaultBomDetailForAbstractItem(abstractItemCode, routingCode, effDate, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(Item abstractItem, string routingCode, DateTime effDate, Location defaultLocationFrom)
        {
            Routing routing = this.routingMgrE.LoadRouting(routingCode);
            return GetDefaultBomDetailForAbstractItem(abstractItem, routing, effDate, defaultLocationFrom);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail GetDefaultBomDetailForAbstractItem(string abstractItemCode, string routingCode, DateTime effDate, Location defaultLocationFrom)
        {
            Item abstractItem = this.itemMgrE.LoadItem(abstractItemCode);
            Routing routing = this.routingMgrE.LoadRouting(routingCode);
            return GetDefaultBomDetailForAbstractItem(abstractItem, routing, effDate, defaultLocationFrom);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BomDetail LoadBomDetail(string bomCode, string itemCode, string reference, DateTime date)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(BomDetail));
            criteria.Add(Expression.Eq("Bom.Code", bomCode));
            criteria.Add(Expression.Eq("Item.Code", itemCode));
            criteria.Add(Expression.Eq("Reference", reference));
            criteria.Add(Expression.Le("StartDate", date));
            criteria.Add(Expression.Or(Expression.IsNull("EndDate"), Expression.Ge("EndDate", date)));
            criteria.AddOrder(Order.Desc("StartDate"));

            IList<BomDetail> bomDetailList = criterialMgrE.FindAll<BomDetail>(criteria);
            if (bomDetailList != null && bomDetailList.Count > 0)
            {
                return bomDetailList[0];
            }
            else
            {
                return null;
            }
        }
        #endregion Customized Methods


        #region Private Methods
        private void ProcessCurrentBomDetail(IList<BomDetail> flatBomDetailList, BomDetail currentBomDetail, DateTime efftiveDate)
        {
            if (currentBomDetail.StructureType == BusinessConstants.CODE_MASTER_BOM_DETAIL_TYPE_VALUE_N) //普通结构(N)
            {
                ProcessCurrentBomDetailByItemType(flatBomDetailList, currentBomDetail, efftiveDate);
            }
            else if (currentBomDetail.StructureType == BusinessConstants.CODE_MASTER_BOM_DETAIL_TYPE_VALUE_O) //选配件(O)
            {
                currentBomDetail.OptionalItemGroup = currentBomDetail.Bom.Code;   //默认用BomCode作为选配件的组号
                ProcessCurrentBomDetailByItemType(flatBomDetailList, currentBomDetail, efftiveDate);
            }
            else if (currentBomDetail.StructureType == BusinessConstants.CODE_MASTER_BOM_DETAIL_TYPE_VALUE_X) //虚结构(X)
            {
                //如果是虚结构(X)，不把自己加到返回表里，继续向下分解
                NestingGetNextLevelBomDetail(flatBomDetailList, currentBomDetail, efftiveDate);
            }
            else
            {
                throw new TechnicalException("no such kind fo bomdetail structure type " + currentBomDetail.StructureType);
            }
        }

        private void ProcessCurrentBomDetailByItemType(IList<BomDetail> flatBomDetailList, BomDetail currentBomDetail, DateTime efftiveDate)
        {
            if (currentBomDetail.Item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_X)
            {
                //如果是虚零件(X)，继续向下分解
                NestingGetNextLevelBomDetail(flatBomDetailList, currentBomDetail, efftiveDate);
            }
            else if (currentBomDetail.Item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_A)
            {
                //todo 抽象件需不需要分解？是否可以在物料回冲时在指定？
                flatBomDetailList.Add(currentBomDetail);
            }
            else if (currentBomDetail.Item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
            {
                //K类型的Item不能出现在Bom结构中
                throw new BusinessErrorException("Bom.Error.ItemTypeKInBom", currentBomDetail.Bom.Code);

                //组件，继续向下分解
                //NestingGetNextLevelBomDetail(flatBomDetailList, currentBomDetail, efftiveDate);
            }
            else
            {
                //直接加入到flatBomDetailList
                flatBomDetailList.Add(currentBomDetail);
            }
        }

        private void NestingGetNextLevelBomDetail(IList<BomDetail> flatBomDetailList, BomDetail currentBomDetail, DateTime efftiveDate)
        {
            string nextLevelBomCode = this.bomMgrE.FindBomCode(currentBomDetail.Item);
            IList<BomDetail> nextBomDetailList = this.GetNextLevelBomDetail(nextLevelBomCode, efftiveDate);

            if (nextBomDetailList != null && nextBomDetailList.Count > 0)
            {
                foreach (BomDetail nextBomDetail in nextBomDetailList)
                {
                    if (nextBomDetail.Item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
                    {
                        //K类型的Item不能出现在Bom结构中
                        throw new BusinessErrorException("Bom.Error.ItemTypeKInBom", nextBomDetail.Bom.Code);
                    }

                    //当前子件的Uom和下层Bom的Uom不匹配，需要做单位转换
                    if (currentBomDetail.Uom.Code != nextBomDetail.Bom.Uom.Code)
                    {
                        //单位换算
                        nextBomDetail.CalculatedQty = uomConversionMgrE.ConvertUomQty(currentBomDetail.Item, currentBomDetail.Uom, 1, nextBomDetail.Bom.Uom)
                            * currentBomDetail.CalculatedQty * nextBomDetail.RateQty * (1 + nextBomDetail.DefaultScrapPercentage);
                    }
                    else
                    {
                        nextBomDetail.CalculatedQty = nextBomDetail.RateQty * currentBomDetail.CalculatedQty * (1 + nextBomDetail.DefaultScrapPercentage);
                    }

                    nextBomDetail.OptionalItemGroup = currentBomDetail.OptionalItemGroup;

                    ProcessCurrentBomDetail(flatBomDetailList, nextBomDetail, efftiveDate);
                }
            }
            else
            {
                throw new BusinessErrorException("Bom.Error.NotFoundForItem", currentBomDetail.Item.Code);
            }
        }

        public IList<BomDetail> GetNoOverloadBomDetail(IList<BomDetail> bomDetailList)
        {
            //过滤BomCode，ItemCode，Operation，Reference相同的BomDetail，只取StartDate最大的。
            if (bomDetailList != null && bomDetailList.Count > 0)
            {
                IList<BomDetail> noOverloadBomDetailList = new List<BomDetail>();
                foreach (BomDetail bomDetail in bomDetailList)
                {
                    int overloadIndex = -1;
                    for (int i = 0; i < noOverloadBomDetailList.Count; i++)
                    {
                        //判断BomCode，ItemCode，Operation，Reference是否相同
                        if (noOverloadBomDetailList[i].Bom.Code == bomDetail.Bom.Code
                            && noOverloadBomDetailList[i].Item.Code == bomDetail.Item.Code
                            && noOverloadBomDetailList[i].Operation == bomDetail.Operation
                            && noOverloadBomDetailList[i].Reference == bomDetail.Reference)
                        {
                            //存在相同的，记录位置。
                            overloadIndex = i;
                            break;
                        }
                    }

                    if (overloadIndex == -1)
                    {
                        //没有相同的记录，直接把BomDetail加入返回列表
                        noOverloadBomDetailList.Add(bomDetail);
                    }
                    else
                    {
                        //有相同的记录，判断bomDetail.StartDate和结果集中的大。
                        if (noOverloadBomDetailList[overloadIndex].StartDate < bomDetail.StartDate)
                        {
                            //bomDetail.StartDate大于结果集中的，替换结果集
                            noOverloadBomDetailList[overloadIndex] = bomDetail;
                        }
                    }
                }
                return noOverloadBomDetailList;
            }
            else
            {
                return null;
            }
        }

        public IList<BomDetail> GetAllBomDetailTree(IList<BomDetail> treeBomDetailList, DateTime effDate)
        {
            IList<BomDetail> bomDetailList = new List<BomDetail>();
            foreach (BomDetail bomDetail in treeBomDetailList)
            {
                bomDetail.CalculatedQty = bomDetail.RateQty * (1 + bomDetail.DefaultScrapPercentage);
                bomDetailList.Add(bomDetail);

                IList<BomDetail> tempBomDetailList = new List<BomDetail>();
                string nextLevelBomCode = (bomDetail.Item.Bom != null ? bomDetail.Item.Bom.Code : bomDetail.Item.Code);
                tempBomDetailList = this.GetNextLevelBomDetail(nextLevelBomCode, effDate);
                if (tempBomDetailList != null && tempBomDetailList.Count > 0)
                {
                    foreach (var tempBomDetail in tempBomDetailList)
                    {
                        tempBomDetail.BomLevel = bomDetail.BomLevel + 1;
                        tempBomDetail.CalculatedQty = tempBomDetail.RateQty * (1 + tempBomDetail.DefaultScrapPercentage);
                        tempBomDetail.AccumQty = bomDetail.AccumQty * tempBomDetail.CalculatedQty;
                    }
                    IList<BomDetail> nextBomDetailList = new List<BomDetail>();
                    nextBomDetailList = this.GetAllBomDetailTree(tempBomDetailList, effDate);
                    foreach (BomDetail bd in nextBomDetailList)
                    {
                        bd.CalculatedQty = bd.RateQty * (1 + bd.DefaultScrapPercentage);
                        bomDetailList.Add(bd);
                    }
                }
            }

            return bomDetailList;
        }

        public IList<BomDetail> GetCostBomDetail(IList<BomDetail> bomDetailList)
        {
            IList<BomDetail> costBomDetailList = new List<BomDetail>();
            foreach (BomDetail bomDetail in bomDetailList)
            {
                bool isExist = false;
                int index = bomDetailList.IndexOf(bomDetail);
                foreach (BomDetail bd in bomDetailList)
                {
                    int bdIndex = bomDetailList.IndexOf(bd);
                    if (bdIndex <= index)
                    {
                        continue;
                    }

                    string nextLevelBomCode = (bomDetail.Item.Bom != null ? bomDetail.Item.Bom.Code : bomDetail.Item.Code);
                    if (nextLevelBomCode == bd.Bom.Code)
                    {
                        isExist = true;
                        bomDetailList[bdIndex].CalculatedQty = bomDetail.CalculatedQty * bd.CalculatedQty;
                        //break;
                    }
                }

                if (!isExist)
                {
                    costBomDetailList.Add(bomDetail);
                }
            }

            return this.GetNoOverloadCostBomDetail(costBomDetailList);
        }

        public IList<BomDetail> GetNoOverloadCostBomDetail(IList<BomDetail> bomDetailList)
        {
            IList<BomDetail> noOverloadBomDetailList = new List<BomDetail>();
            //过滤ItemCode,UOM相同的BomDetail,并累加
            if (bomDetailList != null && bomDetailList.Count > 0)
            {
                foreach (BomDetail bomDetail in bomDetailList)
                {
                    int overloadIndex = -1;
                    for (int i = 0; i < noOverloadBomDetailList.Count; i++)
                    {
                        //判断BomCode，ItemCode，Operation，Reference是否相同
                        if (noOverloadBomDetailList[i].Item.Code == bomDetail.Item.Code
                            && noOverloadBomDetailList[i].Uom.Code == bomDetail.Uom.Code)
                        {
                            //存在相同的，记录位置。
                            overloadIndex = i;
                            break;
                        }
                    }

                    if (overloadIndex == -1)
                    {
                        //没有相同的记录，直接把BomDetail加入返回列表
                        noOverloadBomDetailList.Add(bomDetail);
                    }
                    else
                    {
                        //有相同的记录累加
                        noOverloadBomDetailList[overloadIndex].CalculatedQty += bomDetail.CalculatedQty;
                    }
                }
            }

            return noOverloadBomDetailList;
        }

        //LeadTime取值规则 BomDetail=>Item
        private decimal GetLeadTime(Bom bom)
        {
            //if (bom.LeadTime.HasValue)
            //{
            //    return bom.LeadTime.Value;
            //}
            //else
            //{
            //    string itemCode = (bom.Item == null || bom.Item.Trim() == string.Empty) ? bom.Code : bom.Item;
            //    Item item = itemMgrE.LoadItem(itemCode);

            //    if (item != null)
            //    {
            //        if (item.LeadTime.HasValue)
            //        {
            //            return item.LeadTime.Value;
            //        }
            //        //else
            //        //{
            //        //    FlowView flowView = flowMgrE.LoadFlowView(null, null, null, null, item, new List<string> { BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION });
            //        //    if (flowView != null && flowView.Flow.LeadTime.HasValue)
            //        //    {
            //        //        return flowView.Flow.LeadTime.Value;
            //        //    }
            //        //}
            //    }
            //}
            return 0;
        }
        #endregion
    }
}


#region Extend Class




namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class BomDetailMgrE : com.Sconit.Service.MasterData.Impl.BomDetailMgr, IBomDetailMgrE
    {

    }
}
#endregion
