using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Entity.Production;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Service.Criteria;
using NHibernate.Expression;
using com.Sconit.Service.Distribution;
using com.Sconit.Entity.Distribution;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.Distribution;
using com.Sconit.Service.Ext.Cost;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class ProductLineInProcessLocationDetailMgr : ProductLineInProcessLocationDetailBaseMgr, IProductLineInProcessLocationDetailMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IFlowMgrE flowMgrE { get; set; }
        public ILocationMgrE locationMgrE { get; set; }
        public ILocationTransactionMgrE locationTransactionMgrE { get; set; }
        public IOrderLocationTransactionMgrE orderLocationTransactionMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public IOrderPlannedBackflushMgrE orderPlannedBackflushMgrE { get; set; }
        public IInProcessLocationDetailMgrE inProcessLocationDetailMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public ICostMgrE costMgr { get; set; }
        public IPlannedBillMgrE plannedBillMgr { get; set; }
        public IHuMgrE huMgr { get; set; }

        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public IList<ProductLineInProcessLocationDetail> GetProductLineInProcessLocationDetail(string prodLineCode, string status)
        {
            DetachedCriteria criteria = DetachedCriteria.For<ProductLineInProcessLocationDetail>();

            criteria.Add(Expression.Eq("ProductLine.Code", prodLineCode));
            criteria.Add(Expression.Eq("Status", status));

            criteria.AddOrder(Order.Asc("Id"));

            return this.criteriaMgrE.FindAll<ProductLineInProcessLocationDetail>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<ProductLineInProcessLocationDetail> GetProductLineInProcessLocationDetail(string prodLineCode, string status, string[] items)
        {
            DetachedCriteria criteria = DetachedCriteria.For<ProductLineInProcessLocationDetail>();

            criteria.Add(Expression.Eq("ProductLine.Code", prodLineCode));
            criteria.Add(Expression.Eq("Status", status));

            if (items != null && items.Length > 0)
            {
                criteria.CreateAlias("Item", "item");
                criteria.Add(Expression.In("item.Code", items));
            }

            criteria.AddOrder(Order.Asc("Id"));

            return this.criteriaMgrE.FindAll<ProductLineInProcessLocationDetail>(criteria);
        }

        public IList<ProductLineInProcessLocationDetail> GetProductLineInProcessLocationDetailGroupByItem(string prodLineCode, string status)
        {
            IList<ProductLineInProcessLocationDetail> plIpGroupList = new List<ProductLineInProcessLocationDetail>();
            IList<ProductLineInProcessLocationDetail> plIpList = GetProductLineInProcessLocationDetail(prodLineCode, status);
            foreach (ProductLineInProcessLocationDetail plIpDetail in plIpList)
            {
                bool isExist = false;
                foreach (ProductLineInProcessLocationDetail plIpGroupDetail in plIpGroupList)
                {
                    if (plIpGroupDetail.Item.Code == plIpDetail.Item.Code)
                    {
                        isExist = true;
                        plIpGroupDetail.Qty += plIpDetail.Qty;
                        plIpGroupDetail.BackflushQty += plIpGroupDetail.BackflushQty;
                        break;
                    }
                }
                if (!isExist)
                {
                    ProductLineInProcessLocationDetail newPlIpDetail = new ProductLineInProcessLocationDetail();
                    newPlIpDetail.Item = plIpDetail.Item;
                    newPlIpDetail.Qty = plIpDetail.Qty;
                    newPlIpDetail.BackflushQty = plIpDetail.BackflushQty;
                    plIpGroupList.Add(newPlIpDetail);
                }

            }

            return plIpGroupList;
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialIn(string prodLineCode, IList<MaterialIn> materialInList, User user)
        {
            Flow flow = this.flowMgrE.CheckAndLoadFlow(prodLineCode);
            IList<BomDetail> bomDetailList = this.flowMgrE.GetBatchFeedBomDetail(flow);

            IList<MaterialIn> noneZeroMaterialInList = new List<MaterialIn>();
            DateTime dateTimeNow = DateTime.Now;

            if (materialInList != null && materialInList.Count > 0)
            {
                foreach (MaterialIn materialIn in materialInList)
                {
                    if (materialIn.Qty != 0)
                    {
                        noneZeroMaterialInList.Add(materialIn);
                    }

                    #region 查找物料是否是生产线上投料的
                    if (bomDetailList != null && bomDetailList.Count > 0)
                    {
                        bool findMatch = (from det in bomDetailList
                                          where det.Item.Code == materialIn.RawMaterial.Code
                                          select det).ToList().Count > 0;

                        #region 判断是否后续物料
                        if (!findMatch)
                        {
                            DetachedCriteria criteria = DetachedCriteria.For<ItemDiscontinue>();

                            criteria.Add(Expression.Eq("DiscontinueItem", materialIn.RawMaterial));
                            criteria.Add(Expression.Le("StartDate", dateTimeNow));
                            criteria.Add(Expression.Or(Expression.IsNull("EndDate"), Expression.Ge("EndDate", dateTimeNow)));

                            IList<ItemDiscontinue> disConItems = this.criteriaMgrE.FindAll<ItemDiscontinue>(criteria);
                            if (disConItems != null && disConItems.Count > 0)
                            {
                                findMatch = (from det in bomDetailList
                                             join disConItem in disConItems
                                             on det.Item.Code equals disConItem.Item.Code
                                             where disConItem.Bom == null || disConItem.Bom.Code == det.Bom.Code
                                             select det).ToList().Count > 0;
                            }
                        }
                        #endregion

                        if (!findMatch)
                        {
                            throw new BusinessErrorException("MasterData.Production.Feed.Error.NotContainMaterial", materialIn.RawMaterial.Code, prodLineCode);
                        }
                    }
                    else
                    {
                        throw new BusinessErrorException("MasterData.Production.Feed.Error.NoFeedMaterial", prodLineCode);
                    }
                    #endregion
                }
            }

            if (noneZeroMaterialInList.Count == 0)
            {
                throw new BusinessErrorException("Order.Error.ProductLineInProcessLocationDetailEmpty");
            }

            foreach (MaterialIn materialIn in noneZeroMaterialInList)
            {
                #region 出库
                IList<InventoryTransaction> inventoryTransactionList = this.locationMgrE.InventoryOut(materialIn, user, flow);
                #endregion

                #region 入生产线物料
                foreach (InventoryTransaction inventoryTransaction in inventoryTransactionList)
                {
                    ProductLineInProcessLocationDetail productLineInProcessLocationDetail = new ProductLineInProcessLocationDetail();
                    productLineInProcessLocationDetail.ProductLine = flow;
                    productLineInProcessLocationDetail.Operation = materialIn.Operation;
                    productLineInProcessLocationDetail.Item = inventoryTransaction.Item;
                    productLineInProcessLocationDetail.HuId = inventoryTransaction.Hu != null ? inventoryTransaction.Hu.HuId : null;
                    productLineInProcessLocationDetail.LotNo = inventoryTransaction.Hu != null ? inventoryTransaction.Hu.LotNo : null;
                    productLineInProcessLocationDetail.Qty = 0 - inventoryTransaction.Qty;
                    productLineInProcessLocationDetail.CurrentQty = productLineInProcessLocationDetail.Qty;
                    productLineInProcessLocationDetail.IsConsignment = inventoryTransaction.IsConsignment;
                    productLineInProcessLocationDetail.PlannedBill = inventoryTransaction.PlannedBill;
                    productLineInProcessLocationDetail.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
                    productLineInProcessLocationDetail.LocationFrom = inventoryTransaction.Location;
                    productLineInProcessLocationDetail.CreateDate = dateTimeNow;
                    productLineInProcessLocationDetail.CreateUser = user;
                    //productLineInProcessLocationDetail.LastModifyDate = dateTimeNow;
                    productLineInProcessLocationDetail.LastModifyUser = user;

                    this.CreateProductLineInProcessLocationDetail(productLineInProcessLocationDetail);

                    //记录库存事务
                    this.locationTransactionMgrE.RecordLocationTransaction(productLineInProcessLocationDetail, BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_MATERIAL_IN, user, BusinessConstants.IO_TYPE_IN);
                }
                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialIn(Flow prodLine, IList<MaterialIn> materialInList, User user)
        {
            this.RawMaterialIn(prodLine.Code, materialInList, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialBackflush(string prodLineCode, User user)
        {
            this.RawMaterialBackflush(prodLineCode, null, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialBackflush(string prodLineCode, IDictionary<string, decimal> itemQtydic, User user)
        {

            if (itemQtydic == null || itemQtydic.Count == 0)
            {
                throw new BusinessErrorException("MasterData.Production.Feed.Error.NoSelectFeed");
            }

            Flow flow = this.flowMgrE.CheckAndLoadFlow(prodLineCode);
            DateTime dateTimeNow = DateTime.Now;

            IList<ProductLineInProcessLocationDetail> productLineInProcessLocationDetailList =
                this.GetProductLineInProcessLocationDetail(prodLineCode, BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE, itemQtydic.Keys.ToArray<string>());

            IList<ProductLineInProcessLocationDetail> targetProductLineInProcessLocationDetailList = new List<ProductLineInProcessLocationDetail>();

            #region 根据剩余数量计算回冲零件数量，添加到待处理列表
            if (itemQtydic != null && itemQtydic.Count > 0)
            {
                foreach (string itemCode in itemQtydic.Keys)
                {
                    decimal remainQty = itemQtydic[itemCode];   //剩余投料量
                    decimal inQty = 0;                     //总投料量
                    IList<ProductLineInProcessLocationDetail> currentProductLineInProcessLocationDetailList = new List<ProductLineInProcessLocationDetail>();
                    foreach (ProductLineInProcessLocationDetail productLineInProcessLocationDetail in productLineInProcessLocationDetailList)
                    {
                        if (productLineInProcessLocationDetail.Item.Code == itemCode)
                        {
                            inQty += (productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty);
                            currentProductLineInProcessLocationDetailList.Add(productLineInProcessLocationDetail);
                        }
                    }

                    if (remainQty > inQty)
                    {
                        //throw new BusinessErrorException("MasterData.Production.Feed.Error.RemainQtyGtFeedQty", itemCode);
                    }

                    decimal backflushQty = inQty - remainQty;  //本次回冲量

                    #region 设定本次回冲数量
                    if (backflushQty > 0)
                    {
                        foreach (ProductLineInProcessLocationDetail productLineInProcessLocationDetail in currentProductLineInProcessLocationDetailList)
                        {
                            if (backflushQty - (productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty) > 0)
                            {
                                productLineInProcessLocationDetail.CurrentBackflushQty = productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty;
                                backflushQty -= productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty;
                                productLineInProcessLocationDetail.BackflushQty = productLineInProcessLocationDetail.Qty;
                                targetProductLineInProcessLocationDetailList.Add(productLineInProcessLocationDetail);
                            }
                            else
                            {
                                productLineInProcessLocationDetail.CurrentBackflushQty = backflushQty;
                                productLineInProcessLocationDetail.BackflushQty += backflushQty;
                                backflushQty = 0;
                                targetProductLineInProcessLocationDetailList.Add(productLineInProcessLocationDetail);
                                break;
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion

            //为了多次回冲，注掉此处代码
            //#region 处理未填写剩余数量的投料，全部添加到待处理列表
            //foreach (ProductLineInProcessLocationDetail productLineInProcessLocationDetail in productLineInProcessLocationDetailList)
            //{
            //    bool isUsed = false;
            //    foreach (string itemCode in itemQtydic.Keys)
            //    {
            //        if (productLineInProcessLocationDetail.Item.Code == itemCode)
            //        {
            //            isUsed = true;
            //            break;
            //        }
            //    }

            //    //未填写剩余数量的全部回冲
            //    if (!isUsed)
            //    {
            //        productLineInProcessLocationDetail.CurrentBackflushQty = productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty;
            //        productLineInProcessLocationDetail.BackflushQty = productLineInProcessLocationDetail.Qty;
            //        targetProductLineInProcessLocationDetailList.Add(productLineInProcessLocationDetail);
            //    }
            //}
            //#endregion

            if (targetProductLineInProcessLocationDetailList != null && targetProductLineInProcessLocationDetailList.Count > 0)
            {
                #region 更新生产线上的物料
                foreach (ProductLineInProcessLocationDetail productLineInProcessLocationDetail in targetProductLineInProcessLocationDetailList)
                {
                    if (productLineInProcessLocationDetail.Qty == productLineInProcessLocationDetail.BackflushQty)
                    {
                        productLineInProcessLocationDetail.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
                    }
                    //productLineInProcessLocationDetail.LastModifyDate = dateTimeNow;
                    productLineInProcessLocationDetail.LastModifyUser = user;

                    this.UpdateProductLineInProcessLocationDetail(productLineInProcessLocationDetail);

                    //记录库存事务
                    //this.locationTransactionMgrE.RecordLocationTransaction(productLineInProcessLocationDetail, BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_TR, user, BusinessConstants.IO_TYPE_OUT);
                    //this.locationTransactionMgrE.RecordLocationTransaction(productLineInProcessLocationDetail, BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_TR, user, BusinessConstants.IO_TYPE_OUT);
                }
                #endregion

                IList<OrderPlannedBackflush> orderPlannedBackflushList = this.orderPlannedBackflushMgrE.GetActiveOrderPlannedBackflush(prodLineCode, itemQtydic.Keys.ToArray<string>());
                if (orderPlannedBackflushList == null || orderPlannedBackflushList.Count == 0)
                {
                    throw new BusinessErrorException("MasterData.Production.Feed.Error.NoWO", prodLineCode);
                }

                var mainOrderPlannedBackflushList = from opb in orderPlannedBackflushList
                                                    select new
                                                    {
                                                        Item = opb.OrderLocationTransaction.Item,
                                                        PlannedQty = opb.PlannedQty,
                                                        OrderPlannedBackflush = opb
                                                    };
                #region 查找替换物料
                DateTime minCreateDate = (from opb in orderPlannedBackflushList
                                          orderby opb.CreateDate ascending
                                          select opb.CreateDate).First();

                DateTime maxCreateDate = (from opb in orderPlannedBackflushList
                                          orderby opb.CreateDate descending
                                          select opb.CreateDate).First();


                DetachedCriteria criteria = DetachedCriteria.For<ItemDiscontinue>();

                criteria.Add(Expression.Ge("StartDate", maxCreateDate));
                criteria.Add(Expression.Or(Expression.Le("EndDate", minCreateDate), Expression.IsNull("EndDate")));
                criteria.Add(Expression.In("Item.Code", itemQtydic.Keys.ToArray<string>()));

                IList<ItemDiscontinue> itemDiscontinueList = this.criteriaMgrE.FindAll<ItemDiscontinue>(criteria);

                if (itemDiscontinueList != null && itemDiscontinueList.Count > 0)
                {
                    var disOrderPlannedBackflushList = from i in itemDiscontinueList
                                                       join m in mainOrderPlannedBackflushList on i.Item.Code equals m.Item.Code
                                                       select new
                                                       {
                                                           Item = i.DiscontinueItem,
                                                           PlannedQty = m.PlannedQty * i.UnitQty,
                                                           OrderPlannedBackflush = m.OrderPlannedBackflush
                                                       };

                    if (disOrderPlannedBackflushList != null && disOrderPlannedBackflushList.Count() > 0)
                    {
                        mainOrderPlannedBackflushList = mainOrderPlannedBackflushList.Concat(disOrderPlannedBackflushList);
                    }
                }
                #endregion

                var productLineInProcessLocationDetailDic = from plIp in targetProductLineInProcessLocationDetailList
                                                            group plIp by new
                                                            {
                                                                Item = plIp.Item.Code,
                                                                Operation = plIp.Operation,
                                                                HuId = plIp.HuId,
                                                                LotNo = plIp.LotNo,
                                                                LocationFrom = plIp.LocationFrom,
                                                                IsConsignment = plIp.IsConsignment,
                                                                PlannedBill = plIp.PlannedBill
                                                            } into result
                                                            select new
                                                            {
                                                                Item = result.Key.Item,
                                                                Operation = result.Key.Operation,
                                                                HuId = result.Key.HuId,
                                                                LotNo = result.Key.LotNo,
                                                                LocationFrom = result.Key.LocationFrom,
                                                                IsConsignment = result.Key.IsConsignment,
                                                                PlannedBill = result.Key.PlannedBill,
                                                                BackflushQty = result.Sum(plIp => plIp.CurrentBackflushQty)
                                                            };

                foreach (var productLineInProcessLocationDetail in productLineInProcessLocationDetailDic)
                {
                    var planList = mainOrderPlannedBackflushList.Where(p => p.Item.Code == productLineInProcessLocationDetail.Item
                        && (!productLineInProcessLocationDetail.Operation.HasValue || productLineInProcessLocationDetail.Operation == p.OrderPlannedBackflush.OrderLocationTransaction.Operation)).ToList();

                    var totalBaseQty = planList.Sum(p => p.PlannedQty); //回冲分配基数

                    if (planList.Count > 0)
                    {
                        EntityPreference entityPreference = this.entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_AMOUNT_DECIMAL_LENGTH);
                        int amountDecimalLength = int.Parse(entityPreference.Value);

                        decimal remainTobeBackflushQty = productLineInProcessLocationDetail.BackflushQty;  //剩余待回冲数量
                        decimal unitQty = remainTobeBackflushQty / totalBaseQty;  //单位基数的回冲数量

                        for (int i = 0; i < planList.Count; i++)
                        {
                            #region 物料回冲
                            #region 更新匹配的OrderLocationTransaction
                            Item matchedItem = planList[i].Item;
                            OrderPlannedBackflush matchedOrderPlannedBackflush = planList[i].OrderPlannedBackflush;
                            OrderLocationTransaction matchedOrderLocationTransaction = matchedOrderPlannedBackflush.OrderLocationTransaction;

                            bool isLastestRecord = (i == (planList.Count - 1));
                            decimal currentTotalBackflushQty = 0;

                            if (!matchedOrderLocationTransaction.AccumulateQty.HasValue)
                            {
                                matchedOrderLocationTransaction.AccumulateQty = 0;
                            }

                            if (!isLastestRecord)
                            {
                                decimal currentBackflushQty = Math.Round(planList[i].PlannedQty * unitQty, amountDecimalLength, MidpointRounding.AwayFromZero);
                                currentTotalBackflushQty += currentBackflushQty;
                                matchedOrderLocationTransaction.AccumulateQty += currentBackflushQty;
                                remainTobeBackflushQty -= currentBackflushQty;
                            }
                            else
                            {
                                currentTotalBackflushQty += remainTobeBackflushQty;
                                matchedOrderLocationTransaction.AccumulateQty += remainTobeBackflushQty;
                                remainTobeBackflushQty = 0;
                            }

                            this.orderLocationTransactionMgrE.UpdateOrderLocationTransaction(matchedOrderLocationTransaction);
                            #endregion

                            #region 新增/更新AsnDetail
                            //InProcessLocationDetail inProcessLocationDetail = null;
                            //if (productLineInProcessLocationDetail.HuId == null || productLineInProcessLocationDetail.HuId.Trim() == string.Empty)
                            //{
                            //    inProcessLocationDetail = this.inProcessLocationDetailMgrE.GetNoneHuAndIsConsignmentInProcessLocationDetail(matchedOrderPlannedBackflush.InProcessLocation, matchedOrderPlannedBackflush.OrderLocationTransaction);
                            //    if (inProcessLocationDetail != null)
                            //    {
                            //        inProcessLocationDetail.Qty += currentTotalBackflushQty;

                            //        this.inProcessLocationDetailMgrE.UpdateInProcessLocationDetail(inProcessLocationDetail);
                            //    }
                            //}

                            //if (inProcessLocationDetail == null)
                            //{
                            //    inProcessLocationDetail = new InProcessLocationDetail();
                            //    inProcessLocationDetail.InProcessLocation = matchedOrderPlannedBackflush.InProcessLocation;
                            //    inProcessLocationDetail.OrderLocationTransaction = matchedOrderPlannedBackflush.OrderLocationTransaction;
                            //    inProcessLocationDetail.HuId = productLineInProcessLocationDetail.HuId;
                            //    inProcessLocationDetail.LotNo = productLineInProcessLocationDetail.LotNo;
                            //    inProcessLocationDetail.IsConsignment = productLineInProcessLocationDetail.IsConsignment;
                            //    inProcessLocationDetail.PlannedBill = productLineInProcessLocationDetail.PlannedBill;
                            //    inProcessLocationDetail.Qty = currentTotalBackflushQty;
                            //    inProcessLocationDetail.Item = matchedItem;

                            //    this.inProcessLocationDetailMgrE.CreateInProcessLocationDetail(inProcessLocationDetail);

                            //    matchedOrderPlannedBackflush.InProcessLocation.AddInProcessLocationDetail(inProcessLocationDetail);
                            //}

                            #endregion

                            #region 新增库存事务
                            this.locationTransactionMgrE.RecordWOBackflushLocationTransaction(
                                matchedOrderPlannedBackflush.OrderLocationTransaction, matchedItem,
                                productLineInProcessLocationDetail.HuId, productLineInProcessLocationDetail.LotNo, currentTotalBackflushQty,
                                null, user, productLineInProcessLocationDetail.LocationFrom);
                            #endregion

                            #region 记录回冲成本事务
                            this.costMgr.RecordProductionBackFlushCostTransaction(matchedOrderPlannedBackflush.OrderLocationTransaction, matchedItem, -1 * currentTotalBackflushQty, user);
                            #endregion
                            #endregion

                            #region 关闭OrderPlannedBackflush
                            if (matchedOrderPlannedBackflush.IsActive)
                            {
                                matchedOrderPlannedBackflush.IsActive = false;
                                this.orderPlannedBackflushMgrE.UpdateOrderPlannedBackflush(matchedOrderPlannedBackflush);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        #region 没有匹配的OrderLocationTransaction
                        //退回原库位
                        //throw new BusinessErrorException("MasterData.BackFlush.NotFoundResources", productLineInProcessLocationDetail.Item);
                        //this.locationMgrE.InventoryIn(productLineInProcessLocationDetail, user);
                        #endregion
                    }
                }
            }
            else
            {
                throw new BusinessErrorException("MasterData.Production.Feed.Error.NoFeed", prodLineCode);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialBackflush(Flow prodLine, User user)
        {
            this.RawMaterialBackflush(prodLine.Code, null, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialBackflush(Flow prodLine, IDictionary<string, decimal> itemQtydic, User user)
        {
            this.RawMaterialBackflush(prodLine.Code, itemQtydic, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void BackflushRawMaterial(string prodLineCode, Item item, ref decimal qty, OrderLocationTransaction orderLocationTransaction, string ipNo, User user)
        {
            DetachedCriteria criteria = DetachedCriteria.For<ProductLineInProcessLocationDetail>();

            criteria.Add(Expression.Eq("ProductLine.Code", prodLineCode));
            criteria.Add(Expression.Eq("Item", item));
            criteria.Add(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE));

            criteria.AddOrder(Order.Asc("Id"));

            IList<ProductLineInProcessLocationDetail> productLineInProcessLocationDetailList = this.criteriaMgrE.FindAll<ProductLineInProcessLocationDetail>(criteria);

            if (productLineInProcessLocationDetailList != null && productLineInProcessLocationDetailList.Count > 0)
            {
                foreach (ProductLineInProcessLocationDetail productLineInProcessLocationDetail in productLineInProcessLocationDetailList)
                {
                    if (productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty > 0)
                    {
                        decimal currentTotalBackflushQty = 0;
                        if ((productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty) > qty)
                        {
                            currentTotalBackflushQty = qty;
                            productLineInProcessLocationDetail.BackflushQty += currentTotalBackflushQty;
                            qty = 0;
                        }
                        else
                        {
                            currentTotalBackflushQty = productLineInProcessLocationDetail.Qty - productLineInProcessLocationDetail.BackflushQty;
                            qty -= currentTotalBackflushQty;
                            productLineInProcessLocationDetail.BackflushQty = productLineInProcessLocationDetail.Qty;
                            productLineInProcessLocationDetail.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
                        }

                        #region 新增库存事务
                        this.locationTransactionMgrE.RecordWOBackflushLocationTransaction(
                            orderLocationTransaction, item, productLineInProcessLocationDetail.HuId,
                            productLineInProcessLocationDetail.LotNo, currentTotalBackflushQty,
                            ipNo, user, productLineInProcessLocationDetail.LocationFrom);
                        #endregion

                        if (qty == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialReturn(Flow prodLine, User user)
        {
            RawMaterialReturn(prodLine.Code, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialReturn(string prodLineCode, User user)
        {
            Flow flow = this.flowMgrE.CheckAndLoadFlow(prodLineCode);
            DateTime dateTimeNow = DateTime.Now;

            IList<ProductLineInProcessLocationDetail> productLineInProcessLocationDetailList = this.GetProductLineInProcessLocationDetail(flow.Code, BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE);
            foreach (ProductLineInProcessLocationDetail productLineInProcessLocationDetail in productLineInProcessLocationDetailList)
            {
                doReturnRawMaterial(productLineInProcessLocationDetail, productLineInProcessLocationDetail.RemainQty, dateTimeNow, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialReturn(Flow prodLine, IDictionary<string, decimal> returnHuQty, User user)
        {
            RawMaterialReturn(prodLine.Code, returnHuQty, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void RawMaterialReturn(string prodLineCode, IDictionary<string, decimal> returnHuQty, User user)
        {

            if (returnHuQty == null || returnHuQty.Count == 0)
            {
                throw new BusinessErrorException("MasterData.Return.Error.NotEmpty");
            }

            DateTime dateTimeNow = DateTime.Now;
            foreach (string huId in returnHuQty.Keys)
            {
                decimal returnQty = returnHuQty[huId];
                DetachedCriteria criteria = DetachedCriteria.For<ProductLineInProcessLocationDetail>();

                criteria.Add(Expression.Eq("ProductLine.Code", prodLineCode));
                criteria.Add(Expression.Eq("HuId", huId));
                criteria.Add(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE));

                IList<ProductLineInProcessLocationDetail> productLineInProcessLocationDetailList = this.criteriaMgrE.FindAll<ProductLineInProcessLocationDetail>(criteria);

                if (productLineInProcessLocationDetailList == null && productLineInProcessLocationDetailList.Count > 0)
                {
                    throw new BusinessErrorException("MasterData.Return.Error.HuIdNotExist", prodLineCode, huId);
                }

                ProductLineInProcessLocationDetail productLineInProcessLocationDetail = productLineInProcessLocationDetailList[0];
                if (productLineInProcessLocationDetail.RemainQty < returnQty)
                {
                    throw new BusinessErrorException("MasterData.Return.Error.ReturnHuQtyLeRemianQty", prodLineCode, huId);
                }

                doReturnRawMaterial(productLineInProcessLocationDetail, returnQty, dateTimeNow, user);
            }
        }
        #endregion Customized Methods

        #region Private Methods
        private void doReturnRawMaterial(ProductLineInProcessLocationDetail productLineInProcessLocationDetail, decimal returnQty, DateTime dateTimeNow, User user)
        {
            #region 生产线退料
            productLineInProcessLocationDetail.CurrentQty = 0 - returnQty;

            productLineInProcessLocationDetail.Qty -= returnQty;
            if (productLineInProcessLocationDetail.RemainQty == 0)
            {
                productLineInProcessLocationDetail.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
            }
            productLineInProcessLocationDetail.LastModifyDate = dateTimeNow;
            productLineInProcessLocationDetail.LastModifyUser = user;

            this.UpdateProductLineInProcessLocationDetail(productLineInProcessLocationDetail);

            this.locationTransactionMgrE.RecordLocationTransaction(productLineInProcessLocationDetail, BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT_MATERIAL_IN, user, BusinessConstants.IO_TYPE_IN);
            #endregion

            #region 线边库位收货
            PlannedBill plannedBill = null;
            if (productLineInProcessLocationDetail.PlannedBill.HasValue)
            {
                this.plannedBillMgr.LoadPlannedBill(productLineInProcessLocationDetail.PlannedBill.Value);
            }
            this.locationMgrE.InventoryIn(productLineInProcessLocationDetail.LocationFrom, null, productLineInProcessLocationDetail.Item, productLineInProcessLocationDetail.HuId, productLineInProcessLocationDetail.LotNo, returnQty, productLineInProcessLocationDetail.IsConsignment, plannedBill, BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_TR, user);
            #endregion

            #region 更新条码数量
            if (productLineInProcessLocationDetail.HuId != null && productLineInProcessLocationDetail.HuId != string.Empty)
            {
                Hu hu = this.huMgr.CheckAndLoadHu(productLineInProcessLocationDetail.HuId);
                hu.Qty -= returnQty;

                this.huMgr.UpdateHu(hu);
            }
            #endregion
        }
        #endregion
    }
}


#region 扩展


namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class ProductLineInProcessLocationDetailMgrE : com.Sconit.Service.MasterData.Impl.ProductLineInProcessLocationDetailMgr, IProductLineInProcessLocationDetailMgrE
    {

    }
}


#endregion