using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Service.Ext.MasterData;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class InspectOrderMgr : InspectOrderBaseMgr, IInspectOrderMgr
    {
        public INumberControlMgrE numberControlMgrE { get; set; }
        public ILocationMgrE locationMgrE { get; set; }
        public ILocationLotDetailMgrE locationLotDetailMgrE { get; set; }
        public IInspectOrderDetailMgrE inspectOrderDetailMgrE { get; set; }
        public IInspectResultMgrE inspectResultMgrE { get; set; }
        public IItemMgrE itemMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public IBomDetailMgrE bomDetailMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IRegionMgrE regionMgr { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public IInspectComfirmResultMgrE inspectComfirmResultMgrE { get; set; }

        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public override void CreateInspectOrder(InspectOrder entity)
        {
            entity.EstimateInspectDate = DateTime.Now;
            base.CreateInspectOrder(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public InspectOrder CheckAndLoadInspectOrder(string inspectOrderNo)
        {
            return this.CheckAndLoadInspectOrder(inspectOrderNo, false);
        }

        [Transaction(TransactionMode.Unspecified)]
        public InspectOrder CheckAndLoadInspectOrder(string inspectOrderNo, bool includeDetail)
        {
            InspectOrder inspectOrder = this.LoadInspectOrder(inspectOrderNo);
            if (inspectOrder == null)
            {
                throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.InspectOrderNoNotExist", inspectOrderNo);
            }

            if (includeDetail && inspectOrder.InspectOrderDetails != null && inspectOrder.InspectOrderDetails.Count > 0)
            {
            }

            return inspectOrder;
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(IList<LocationLotDetail> locationLotDetailList, string inspectLocation, string rejectLocation, User user)
        {
            return CreateInspectOrder(locationLotDetailList, inspectLocation, rejectLocation, user, null, null, true);
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(IList<LocationLotDetail> locationLotDetailList, string inspectLocationCode, string rejectLocationCode, User user, string ipNo, string receiptNo, bool isSeperated)
        {
            IList<LocationLotDetail> noneZeroLocationLotDetailList = new List<LocationLotDetail>();

            bool? isDetailHasHu = null;
            if (locationLotDetailList != null && locationLotDetailList.Count > 0)
            {
                foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
                {
                    if (locationLotDetail.Location.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                    {
                        throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.LocationFrom", locationLotDetail.Location.Code);
                    }

                    if (locationLotDetail.CurrentInspectQty > 0)
                    {
                        if (isDetailHasHu == null)
                        {
                            isDetailHasHu = (locationLotDetail.Hu != null);
                        }
                        else if (isDetailHasHu != (locationLotDetail.Hu != null))
                        {
                            throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.NotAllDetailHasHu");
                        }
                        noneZeroLocationLotDetailList.Add(locationLotDetail);
                    }
                }
            }

            if (noneZeroLocationLotDetailList.Count == 0)
            {
                throw new BusinessErrorException("Order.Error.Inspection.DetailEmpty");
            }

            #region �������鵥ͷ
            DateTime dateTimeNow = DateTime.Now;
            InspectOrder inspectOrder = new InspectOrder();
            inspectOrder.InspectNo = this.numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_INSPECTION);
            inspectOrder.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
            inspectOrder.CreateUser = user;
            inspectOrder.CreateDate = dateTimeNow;
            inspectOrder.LastModifyUser = user;
            inspectOrder.LastModifyDate = dateTimeNow;
            inspectOrder.IsDetailHasHu = isDetailHasHu.Value;
            inspectOrder.IpNo = ipNo;
            inspectOrder.ReceiptNo = receiptNo;
            inspectOrder.IsSeperated = isSeperated;

            Location inspectLocation = this.locationMgrE.CheckAndLoadLocation(inspectLocationCode);
            if (inspectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_INSPECT)
            {
                inspectOrder.InspectLocation = inspectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.InspectLocationTypeError", inspectLocationCode);
            }

            Location rejectLocation = locationMgrE.CheckAndLoadLocation(rejectLocationCode);
            if (rejectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_REJECT)
            {
                inspectOrder.RejectLocation = rejectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.RejectLocationTypeError", rejectLocationCode);
            }

            this.CreateInspectOrder(inspectOrder);
            #endregion

            #region ����������ϸ
            foreach (LocationLotDetail locationLotDetail in noneZeroLocationLotDetailList)
            {
                //�������
                this.locationMgrE.InspectOut(locationLotDetail, user, false, inspectOrder.InspectNo, inspectLocation);

                //������λ
                IList<InventoryTransaction> inventoryTransactionList = this.locationMgrE.InspectIn(locationLotDetail, inspectLocation, user, false, inspectOrder.InspectNo, null);

                if (inventoryTransactionList != null && inventoryTransactionList.Count > 0)
                {
                    foreach (InventoryTransaction inventoryTransaction in inventoryTransactionList)
                    {
                        InspectOrderDetail inspectOrderDetail = new InspectOrderDetail();
                        inspectOrderDetail.InspectOrder = inspectOrder;
                        inspectOrderDetail.InspectQty = inventoryTransaction.Qty;
                        inspectOrderDetail.LocationLotDetail = this.locationLotDetailMgrE.LoadLocationLotDetail(inventoryTransaction.LocationLotDetailId);
                        inspectOrderDetail.LocationFrom = locationLotDetail.Location;
                        inspectOrderDetail.LocationTo = locationLotDetail.InspectQualifyLocation != null ? locationLotDetail.InspectQualifyLocation : locationLotDetail.Location;

                        if (inspectOrderDetail.LocationFrom.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                        {
                            throw new BusinessErrorException("InspectOrder.Error.LocationFromIsNotNormal", inspectOrderDetail.LocationFrom.Code);
                        }

                        if (inspectOrderDetail.LocationTo.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                        {
                            throw new BusinessErrorException("InspectOrder.Error.LocationToIsNotNormal", inspectOrderDetail.LocationTo.Code);
                        }

                        this.inspectOrderDetailMgrE.CreateInspectOrderDetail(inspectOrderDetail);

                        inspectOrder.AddInspectOrderDetail(inspectOrderDetail);
                    }
                }
            }
            #endregion

            return inspectOrder;
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(string locationCode, IDictionary<string, decimal> itemQtyDic, User user)
        {
            Location location = this.locationMgrE.LoadLocation(locationCode);
            string inspectLocation = this.regionMgr.GetDefaultInspectLocation(location.Region.Code);
            string rejectLocation = this.regionMgr.GetDefaultRejectLocation(location.Region.Code);
            
            return CreateInspectOrder(locationCode, itemQtyDic, inspectLocation, rejectLocation, user);
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(Location location, IDictionary<string, decimal> itemQtyDic, User user)
        {
            return CreateInspectOrder(location.Code, itemQtyDic, user);
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(Location location, IDictionary<string, decimal> itemQtyDic, string inspectLocation, string rejectLocation, User user)
        {
            return CreateInspectOrder(location.Code, itemQtyDic, inspectLocation, rejectLocation, user);
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(string locationCode, IDictionary<string, decimal> itemQtyDic, string inspectLocationCode, string rejectLocationCode, User user)
        {
            Location location = this.locationMgrE.CheckAndLoadLocation(locationCode);

            if (location.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
            {
                throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.LocationFrom", locationCode);
            }

            #region �������鵥ͷ
            DateTime dateTimeNow = DateTime.Now;
            InspectOrder inspectOrder = new InspectOrder();
            inspectOrder.InspectNo = this.numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_INSPECTION);
            inspectOrder.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
            inspectOrder.CreateUser = user;
            inspectOrder.CreateDate = dateTimeNow;
            inspectOrder.LastModifyUser = user;
            inspectOrder.LastModifyDate = dateTimeNow;
            inspectOrder.IsDetailHasHu = false;
            inspectOrder.IsSeperated = true;

            Location inspectLocation = this.locationMgrE.CheckAndLoadLocation(inspectLocationCode);
            if (inspectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_INSPECT)
            {
                inspectOrder.InspectLocation = inspectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.InspectLocationTypeError", inspectLocationCode);
            }

            Location rejectLocation = locationMgrE.CheckAndLoadLocation(rejectLocationCode);
            if (rejectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_REJECT)
            {
                inspectOrder.RejectLocation = rejectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.RejectLocationTypeError", rejectLocationCode);
            }

            this.CreateInspectOrder(inspectOrder);
            #endregion

            #region ����������ϸ
            if (itemQtyDic != null && itemQtyDic.Count > 0)
            {
                foreach (string itemCode in itemQtyDic.Keys)
                {
                    if (itemQtyDic[itemCode] == 0)
                    {
                        continue;
                    }

                    Item item = this.itemMgrE.CheckAndLoadItem(itemCode);

                    //�������
                    IList<InventoryTransaction> inventoryTransactionList = this.locationMgrE.InspectOut(location, item, itemQtyDic[itemCode], user, inspectOrder.InspectNo, inspectLocation);

                    //������λ
                    foreach (InventoryTransaction outInventoryTransaction in inventoryTransactionList)
                    {
                        IList<InventoryTransaction> inInventoryTransactionList = this.locationMgrE.InspectIn(
                            item, 0 - outInventoryTransaction.Qty, user, inspectOrder.InspectNo, outInventoryTransaction.PlannedBill,
                            inspectLocation, location);

                        if (inInventoryTransactionList != null && inInventoryTransactionList.Count > 0)
                        {
                            foreach (InventoryTransaction inInventoryTransaction in inInventoryTransactionList)
                            {
                                InspectOrderDetail inspectOrderDetail = new InspectOrderDetail();
                                inspectOrderDetail.InspectOrder = inspectOrder;
                                inspectOrderDetail.InspectQty = inInventoryTransaction.Qty;
                                inspectOrderDetail.LocationLotDetail = this.locationLotDetailMgrE.LoadLocationLotDetail(inInventoryTransaction.LocationLotDetailId);
                                inspectOrderDetail.LocationFrom = location;
                                inspectOrderDetail.LocationTo = location;
                                if (inspectOrderDetail.LocationFrom.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                                {
                                    throw new BusinessErrorException("InspectOrder.Error.LocationFromIsNotNormal", inspectOrderDetail.LocationFrom.Code);
                                }

                                if (inspectOrderDetail.LocationTo.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                                {
                                    throw new BusinessErrorException("InspectOrder.Error.LocationToIsNotNormal", inspectOrderDetail.LocationTo.Code);
                                }

                                this.inspectOrderDetailMgrE.CreateInspectOrderDetail(inspectOrderDetail);

                                inspectOrder.AddInspectOrderDetail(inspectOrderDetail);
                            }
                        }
                    }
                }
            }
            #endregion

            return inspectOrder;
        }


        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(string locationCode, IList<InspectItem> inspectItemList, User user)
        {
            Location location = this.locationMgrE.LoadLocation(locationCode);
            string inspectLocation = this.regionMgr.GetDefaultInspectLocation(location.Region.Code);
            string rejectLocation = this.regionMgr.GetDefaultRejectLocation(location.Region.Code);

            return CreateInspectOrder(locationCode, inspectItemList, inspectLocation, rejectLocation, user);
        }


        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateInspectOrder(string locationCode, IList<InspectItem> inspectItemList, string inspectLocationCode, string rejectLocationCode, User user)
        {
            Location location = this.locationMgrE.CheckAndLoadLocation(locationCode);

            if (location.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
            {
                throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.LocationFrom", locationCode);
            }

            #region �������鵥ͷ
            DateTime dateTimeNow = DateTime.Now;
            InspectOrder inspectOrder = new InspectOrder();
            inspectOrder.InspectNo = this.numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_INSPECTION);
            inspectOrder.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
            inspectOrder.CreateUser = user;
            inspectOrder.CreateDate = dateTimeNow;
            inspectOrder.LastModifyUser = user;
            inspectOrder.LastModifyDate = dateTimeNow;
            inspectOrder.IsDetailHasHu = false;
            inspectOrder.IsSeperated = true;

            Location inspectLocation = this.locationMgrE.CheckAndLoadLocation(inspectLocationCode);
            if (inspectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_INSPECT)
            {
                inspectOrder.InspectLocation = inspectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.InspectLocationTypeError", inspectLocationCode);
            }

            Location rejectLocation = locationMgrE.CheckAndLoadLocation(rejectLocationCode);
            if (rejectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_REJECT)
            {
                inspectOrder.RejectLocation = rejectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.RejectLocationTypeError", rejectLocationCode);
            }

            this.CreateInspectOrder(inspectOrder);
            #endregion

            #region ����������ϸ
            if (inspectItemList != null && inspectItemList.Count > 0)
            {
                foreach (InspectItem inspectItem in inspectItemList)
                {
                    if (inspectItem.InspectQty == 0)
                    {
                        continue;
                    }

                    Item item = inspectItem.Item;

                    //�������
                    IList<InventoryTransaction> inventoryTransactionList = this.locationMgrE.InspectOut(location, item, inspectItem.InspectQty, user, inspectOrder.InspectNo, inspectLocation);

                    //������λ
                    foreach (InventoryTransaction outInventoryTransaction in inventoryTransactionList)
                    {
                        IList<InventoryTransaction> inInventoryTransactionList = this.locationMgrE.InspectIn(
                            item, 0 - outInventoryTransaction.Qty, user, inspectOrder.InspectNo, outInventoryTransaction.PlannedBill, 
                            inspectLocation, location);

                        if (inInventoryTransactionList != null && inInventoryTransactionList.Count > 0)
                        {
                            foreach (InventoryTransaction inInventoryTransaction in inInventoryTransactionList)
                            {
                                InspectOrderDetail inspectOrderDetail = new InspectOrderDetail();
                                inspectOrderDetail.InspectOrder = inspectOrder;
                                inspectOrderDetail.InspectQty = inInventoryTransaction.Qty;
                                inspectOrderDetail.LocationLotDetail = this.locationLotDetailMgrE.LoadLocationLotDetail(inInventoryTransaction.LocationLotDetailId);
                                inspectOrderDetail.LocationFrom = location;
                                inspectOrderDetail.LocationTo = location;
                                inspectOrderDetail.LotNo = inspectItem.LotNo;
                                if (inspectOrderDetail.LocationFrom.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                                {
                                    throw new BusinessErrorException("InspectOrder.Error.LocationFromIsNotNormal", inspectOrderDetail.LocationFrom.Code);
                                }

                                if (inspectOrderDetail.LocationTo.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                                {
                                    throw new BusinessErrorException("InspectOrder.Error.LocationToIsNotNormal", inspectOrderDetail.LocationTo.Code);
                                }

                                this.inspectOrderDetailMgrE.CreateInspectOrderDetail(inspectOrderDetail);

                                inspectOrder.AddInspectOrderDetail(inspectOrderDetail);
                            }
                        }
                    }
                }
            }
            #endregion

            return inspectOrder;
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateFgInspectOrder(string locationCode, IDictionary<string, decimal> itemFgQtyDic, User user)
        {
            Location location = this.locationMgrE.LoadLocation(locationCode);
            string inspectLocation = this.regionMgr.GetDefaultInspectLocation(location.Region.Code);
            string rejectLocation = this.regionMgr.GetDefaultRejectLocation(location.Region.Code);

            return CreateFgInspectOrder(locationCode, itemFgQtyDic, inspectLocation, rejectLocation, user);
        }

        [Transaction(TransactionMode.Requires)]
        public InspectOrder CreateFgInspectOrder(string locationCode, IDictionary<string, decimal> itemFgQtyDic, string inspectLocationCode, string rejectLocationCode, User user)
        {
            #region �������鵥ͷ
            DateTime dateTimeNow = DateTime.Now;
            InspectOrder inspectOrder = new InspectOrder();
            inspectOrder.InspectNo = this.numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_INSPECTION);
            inspectOrder.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
            inspectOrder.CreateUser = user;
            inspectOrder.CreateDate = dateTimeNow;
            inspectOrder.LastModifyUser = user;
            inspectOrder.LastModifyDate = dateTimeNow;
            inspectOrder.IsDetailHasHu = false;
            inspectOrder.IsSeperated = true;

            Location inspectLocation = this.locationMgrE.CheckAndLoadLocation(inspectLocationCode);
            if (inspectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_INSPECT)
            {
                inspectOrder.InspectLocation = inspectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.InspectLocationTypeError", inspectLocationCode);
            }

            Location rejectLocation = locationMgrE.CheckAndLoadLocation(rejectLocationCode);
            if (rejectLocation.Type == BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_REJECT)
            {
                inspectOrder.RejectLocation = rejectLocationCode;
            }
            else
            {
                throw new BusinessErrorException("InspectOrder.Error.RejectLocationTypeError", rejectLocationCode);
            }

            this.CreateInspectOrder(inspectOrder);
            #endregion

            #region ����������ϸ
            if (itemFgQtyDic != null && itemFgQtyDic.Count > 0)
            {
                Location location = this.locationMgrE.CheckAndLoadLocation(locationCode);
                string itemCode = string.Empty;
                string fgCode = string.Empty;

                foreach (string itemFgCode in itemFgQtyDic.Keys)
                {
                    if (itemFgQtyDic[itemFgCode] == 0)
                    {
                        continue;
                    }
                    string[] itemFg = itemFgCode.Split('-');
                    itemCode = itemFg[0];
                    fgCode = itemFg[1];

                    Item item = this.itemMgrE.CheckAndLoadItem(itemCode);

                    //�������
                    this.locationMgrE.InspectOut(location, item, itemFgQtyDic[itemFgCode], user, inspectOrder.InspectNo, inspectLocation);

                    //������λ
                    IList<InventoryTransaction> inventoryTransactionList = this.locationMgrE.InspectIn(item, itemFgQtyDic[itemFgCode], user, inspectOrder.InspectNo, null, inspectLocation, location);

                    if (inventoryTransactionList != null && inventoryTransactionList.Count > 0)
                    {
                        foreach (InventoryTransaction inventoryTransaction in inventoryTransactionList)
                        {
                            InspectOrderDetail inspectOrderDetail = new InspectOrderDetail();
                            inspectOrderDetail.InspectOrder = inspectOrder;
                            inspectOrderDetail.InspectQty = inventoryTransaction.Qty;
                            inspectOrderDetail.LocationLotDetail = this.locationLotDetailMgrE.LoadLocationLotDetail(inventoryTransaction.LocationLotDetailId);
                            inspectOrderDetail.LocationFrom = location;
                            inspectOrderDetail.LocationTo = location;
                            inspectOrderDetail.FinishGoods = this.itemMgrE.LoadItem(fgCode);
                            if (inspectOrderDetail.LocationFrom.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                            {
                                throw new BusinessErrorException("InspectOrder.Error.LocationFromIsNotNormal", inspectOrderDetail.LocationFrom.Code);
                            }

                            if (inspectOrderDetail.LocationTo.Type != BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_NORMAL)
                            {
                                throw new BusinessErrorException("InspectOrder.Error.LocationToIsNotNormal", inspectOrderDetail.LocationTo.Code);
                            }

                            this.inspectOrderDetailMgrE.CreateInspectOrderDetail(inspectOrderDetail);

                            inspectOrder.AddInspectOrderDetail(inspectOrderDetail);
                        }
                    }
                }
            }
            #endregion

            return inspectOrder;
        }

        [Transaction(TransactionMode.Requires)]
        public void ProcessInspectOrder(IList<InspectOrderDetail> inspectOrderDetailList, string userCode)
        {
            ProcessInspectOrder(inspectOrderDetailList, userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void ProcessInspectOrder(IList<InspectOrderDetail> inspectOrderDetailList, User user)
        {
            #region ���˼�������Ϊ0�ļ�����ϸ
            IList<InspectOrderDetail> noneZeroInspectOrderDetailList = new List<InspectOrderDetail>();

            if (inspectOrderDetailList != null && inspectOrderDetailList.Count > 0)
            {
                foreach (InspectOrderDetail inspectOrderDetail in inspectOrderDetailList)
                {
                    if (inspectOrderDetail.CurrentQualifiedQty > 0 || inspectOrderDetail.CurrentRejectedQty > 0)
                    {
                        noneZeroInspectOrderDetailList.Add(inspectOrderDetail);
                    }
                }
            }

            if (noneZeroInspectOrderDetailList.Count == 0)
            {
                throw new BusinessErrorException("Order.Error.Inspection.DetailEmpty");
            }
            #endregion

            #region ѭ�����鵥��ϸ
            IDictionary<string, InspectOrder> cachedInspectOrderDic = new Dictionary<string, InspectOrder>();
            string incNo = this.numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_INSPECTION_COMFIRM_RESULT);  //������ȷ�ϵ���

            //string inrNo = this.numberControlMgr.GenerateNumber(BusinessConstants.CODE_PREFIX_INSPECTION_RESULT);  //����������
            foreach (InspectOrderDetail inspectOrderDetail in noneZeroInspectOrderDetailList)
            {
                #region ������鵥ͷ
                InspectOrder inspectOrder = inspectOrderDetail.InspectOrder;
                if (!cachedInspectOrderDic.ContainsKey(inspectOrder.InspectNo))
                {
                    cachedInspectOrderDic.Add(inspectOrder.InspectNo, inspectOrder);
                }
                #endregion

                #region ��������
                InspectOrderDetail oldInspectOrderDetail = this.inspectOrderDetailMgrE.LoadInspectOrderDetail(inspectOrderDetail.Id);
                //oldInspectOrderDetail.Disposition = inspectOrderDetail.Disposition;
                oldInspectOrderDetail.CurrentQualifiedQty = inspectOrderDetail.CurrentQualifiedQty;
                oldInspectOrderDetail.CurrentRejectedQty = inspectOrderDetail.CurrentRejectedQty;

                if (oldInspectOrderDetail.LocationLotDetail.Hu != null)
                {
                    if (oldInspectOrderDetail.PendingQualifiedQty != inspectOrderDetail.CurrentQualifiedQty
                    && oldInspectOrderDetail.PendingRejectedQty != inspectOrderDetail.CurrentRejectedQty)
                    {
                        //�������ֻ�ܼ���һ��
                        throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.HuInspectQtyNotMatch", oldInspectOrderDetail.LocationLotDetail.Hu.HuId);
                    }
                }
                else
                {
                    if (oldInspectOrderDetail.PendingQualifiedQty < inspectOrderDetail.CurrentQualifiedQty)
                    {
                        throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.QualifyQtyExcceed", oldInspectOrderDetail.LocationLotDetail.Item.Code);
                    }
                    if (oldInspectOrderDetail.PendingRejectedQty < inspectOrderDetail.CurrentRejectedQty)
                    {
                        throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.RejectQtyExcceed", oldInspectOrderDetail.LocationLotDetail.Item.Code);
                    }
                }
                #endregion

                #region �ϸ�Ʒ
                if (oldInspectOrderDetail.CurrentQualifiedQty > 0)
                {
                    oldInspectOrderDetail.LocationLotDetail.CurrentInspectQty = oldInspectOrderDetail.CurrentQualifiedQty;

                    //�������λ
                    this.locationMgrE.InspectOut(oldInspectOrderDetail.LocationLotDetail, user, true, inspectOrder.InspectNo, oldInspectOrderDetail.LocationTo);

                    //��ϸ�Ʒ��λ
                    this.locationMgrE.InspectIn(oldInspectOrderDetail.LocationLotDetail, oldInspectOrderDetail.LocationTo, oldInspectOrderDetail.CurrentStorageBin, user, true, inspectOrder.InspectNo, null);

                    //���ºϸ�Ʒ����
                    if (!oldInspectOrderDetail.QualifiedQty.HasValue)
                    {
                        oldInspectOrderDetail.QualifiedQty = 0;
                    }
                    oldInspectOrderDetail.QualifiedQty += oldInspectOrderDetail.CurrentQualifiedQty;
                    oldInspectOrderDetail.PendingQualifiedQty -= oldInspectOrderDetail.CurrentQualifiedQty;

                }
                #endregion

                #region ���ϸ�Ʒ
                if (oldInspectOrderDetail.CurrentRejectedQty > 0)
                {
                    Location rejectLocation = this.locationMgrE.CheckAndLoadLocation(oldInspectOrderDetail.InspectOrder.RejectLocation);

                    oldInspectOrderDetail.LocationLotDetail.CurrentInspectQty = oldInspectOrderDetail.CurrentRejectedQty;

                    //�������λ
                    this.locationMgrE.InspectOut(oldInspectOrderDetail.LocationLotDetail, user, false, inspectOrder.InspectNo, rejectLocation);

                    //�벻�ϸ�Ʒ��λ
                    this.locationMgrE.InspectIn(oldInspectOrderDetail.LocationLotDetail, rejectLocation, user, false, inspectOrder.InspectNo, null);

                    //���²��ϸ�Ʒ����
                    if (!oldInspectOrderDetail.RejectedQty.HasValue)
                    {
                        oldInspectOrderDetail.RejectedQty = 0;
                    }
                    oldInspectOrderDetail.RejectedQty += oldInspectOrderDetail.CurrentRejectedQty;
                    oldInspectOrderDetail.PendingRejectedQty -= oldInspectOrderDetail.CurrentRejectedQty;
                }
                #endregion

                #region �������ȷ�Ͻ��
                InspectComfirmResult inspectComfirmResult = new InspectComfirmResult();
                inspectComfirmResult.InspComfirmResultNo = incNo;
                inspectComfirmResult.InspDetId = oldInspectOrderDetail.Id;
                inspectComfirmResult.InspResultId = oldInspectOrderDetail.InspectResultId;
                inspectComfirmResult.QualifiedQty = oldInspectOrderDetail.CurrentQualifiedQty;
                inspectComfirmResult.RejectedQty = oldInspectOrderDetail.CurrentRejectedQty;
                inspectComfirmResult.CreateDate = DateTime.Now;
                inspectComfirmResult.CreateUser = user.Code;
                inspectComfirmResult.CreateUserNm = user.Name;
                inspectComfirmResult.Disposition = oldInspectOrderDetail.Disposition;
                this.inspectComfirmResultMgrE.CreateInspectComfirmResult(inspectComfirmResult);
                #endregion

                #region ���¼��鵥��ϸ
                this.inspectOrderDetailMgrE.UpdateInspectOrderDetail(oldInspectOrderDetail);
                #endregion
            }
            #endregion

            #region ���¼��鵥
            DateTime dataTimeNow = DateTime.Now;
            foreach (InspectOrder oldInspectOrder in cachedInspectOrderDic.Values)
            {
                InspectOrder inspectOrder = this.LoadInspectOrder(oldInspectOrder.InspectNo);
                inspectOrder.LastModifyUser = user;
                inspectOrder.LastModifyDate = dataTimeNow;

                bool allClose = true;
                IList<InspectOrderDetail> detailList = inspectOrderDetailMgrE.GetInspectOrderDetail(inspectOrder);
                foreach (InspectOrderDetail inspectOrderDetail in detailList)
                {
                    if (inspectOrderDetail.InspectQty !=
                        (inspectOrderDetail.QualifiedQty.HasValue ? inspectOrderDetail.QualifiedQty.Value : 0)
                        + (inspectOrderDetail.RejectedQty.HasValue ? inspectOrderDetail.RejectedQty.Value : 0))
                    {
                        allClose = false;
                        break;
                    }
                }

                if (allClose)
                {
                    inspectOrder.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
                }

                this.UpdateInspectOrder(inspectOrder);
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void PendInspectOrder(IList<InspectOrderDetail> inspectOrderDetailList, string userCode)
        {
            PendInspectOrder(inspectOrderDetailList, userMgrE.CheckAndLoadUser(userCode));
        }

        [Transaction(TransactionMode.Requires)]
        public void PendInspectOrder(IList<InspectOrderDetail> inspectOrderDetailList, User user)
        {
            #region ���˼�������Ϊ0�ļ�����ϸ
            IList<InspectOrderDetail> noneZeroInspectOrderDetailList = new List<InspectOrderDetail>();

            if (inspectOrderDetailList != null && inspectOrderDetailList.Count > 0)
            {
                foreach (InspectOrderDetail inspectOrderDetail in inspectOrderDetailList)
                {
                    if (inspectOrderDetail.CurrentQualifiedQty > 0 || inspectOrderDetail.CurrentRejectedQty > 0)
                    {
                        noneZeroInspectOrderDetailList.Add(inspectOrderDetail);
                    }
                }
            }

            if (noneZeroInspectOrderDetailList.Count == 0)
            {
                throw new BusinessErrorException("Order.Error.Inspection.DetailEmpty");
            }
            #endregion

            #region ѭ�����鵥��ϸ
            IDictionary<string, InspectOrder> cachedInspectOrderDic = new Dictionary<string, InspectOrder>();
            string irNo = this.numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_INSPECTION_RESULT);  //����������


            foreach (InspectOrderDetail inspectOrderDetail in noneZeroInspectOrderDetailList)
            {
                #region ������鵥ͷ
                InspectOrder inspectOrder = inspectOrderDetail.InspectOrder;
                if (!cachedInspectOrderDic.ContainsKey(inspectOrder.InspectNo))
                {
                    cachedInspectOrderDic.Add(inspectOrder.InspectNo, inspectOrder);
                }
                #endregion

                #region ��������
                InspectOrderDetail oldInspectOrderDetail = this.inspectOrderDetailMgrE.LoadInspectOrderDetail(inspectOrderDetail.Id);
                oldInspectOrderDetail.Disposition = inspectOrderDetail.Disposition;
                oldInspectOrderDetail.CurrentQualifiedQty = inspectOrderDetail.CurrentQualifiedQty;
                oldInspectOrderDetail.CurrentRejectedQty = inspectOrderDetail.CurrentRejectedQty;
                decimal totalInspectedQty = (oldInspectOrderDetail.QualifiedQty.HasValue ? oldInspectOrderDetail.QualifiedQty.Value : 0)
                                          + (oldInspectOrderDetail.RejectedQty.HasValue ? oldInspectOrderDetail.RejectedQty.Value : 0)
                                          + oldInspectOrderDetail.PendingQualifiedQty
                                          + oldInspectOrderDetail.PendingRejectedQty
                                          + oldInspectOrderDetail.CurrentQualifiedQty
                                          + oldInspectOrderDetail.CurrentRejectedQty;

                if (oldInspectOrderDetail.LocationLotDetail.Hu != null
                    && oldInspectOrderDetail.InspectQty != totalInspectedQty)
                {
                    //�������ֻ�ܼ���һ��
                    throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.HuInspectQtyNotMatch", oldInspectOrderDetail.LocationLotDetail.Hu.HuId);
                }

                if (oldInspectOrderDetail.InspectQty < totalInspectedQty)
                {
                    throw new BusinessErrorException("MasterData.Inventory.Inspect.Error.InspectQtyExcceed", oldInspectOrderDetail.LocationLotDetail.Item.Code);
                }
                #endregion

                #region �ϸ�Ʒ
                if (oldInspectOrderDetail.CurrentQualifiedQty > 0)
                {
                    //���´�����ϸ�Ʒ����
                    oldInspectOrderDetail.PendingQualifiedQty += oldInspectOrderDetail.CurrentQualifiedQty;
                }
                #endregion

                #region ���ϸ�Ʒ
                if (oldInspectOrderDetail.CurrentRejectedQty > 0)
                {
                    //���²��ϸ�Ʒ����
                    oldInspectOrderDetail.PendingRejectedQty += oldInspectOrderDetail.CurrentRejectedQty;
                }
                #endregion

                #region ���������
                InspectResult inspectResult = new InspectResult();
                inspectResult.InspectResultNo = irNo;
                inspectResult.InspectOrderDetail = oldInspectOrderDetail;
                inspectResult.QualifiedQty = oldInspectOrderDetail.CurrentQualifiedQty;
                inspectResult.RejectedQty = oldInspectOrderDetail.CurrentRejectedQty;
                inspectResult.CreateDate = DateTime.Now;
                inspectResult.CreateUser = user;
                inspectResult.LastModifyDate = DateTime.Now;
                inspectResult.LastModifyUser = user;
                inspectResult.PrintCount = 0;
                inspectResult.IsPrinted = false;
                inspectResult.Disposition = oldInspectOrderDetail.Disposition;
                this.inspectResultMgrE.CreateInspectResult(inspectResult);
                #endregion

                #region ���¼��鵥��ϸ
                this.inspectOrderDetailMgrE.UpdateInspectOrderDetail(oldInspectOrderDetail);
                #endregion

                #region ���������Ҫȷ�ϣ���������Ĳ���Ҫȷ��
                bool needInspectConfirm = bool.Parse(entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_NEED_INSPECT_CONFIRM).Value);
                if (!needInspectConfirm)
                {
                    IList<InspectOrderDetail> pendingList = new List<InspectOrderDetail>();
                    oldInspectOrderDetail.InspectResultId = inspectResult.Id;
                    pendingList.Add(oldInspectOrderDetail);
                    ProcessInspectOrder(pendingList, user);
                }
                #endregion


            }
            #endregion

            #region ���¼��鵥
            DateTime dataTimeNow = DateTime.Now;
            foreach (InspectOrder oldInspectOrder in cachedInspectOrderDic.Values)
            {
                InspectOrder inspectOrder = this.LoadInspectOrder(oldInspectOrder.InspectNo);
                inspectOrder.LastModifyUser = user;
                inspectOrder.LastModifyDate = dataTimeNow;
                this.UpdateInspectOrder(inspectOrder);
            }
            #endregion
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<InspectOrder> GetInspectOrder(string ipNo, string receiptNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(InspectOrder));

            if (receiptNo != null && receiptNo.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("ReceiptNo", receiptNo));
            }
            if (ipNo != null && ipNo.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("IpNo", ipNo));
            }
            IList<InspectOrder> inspectOrders = criteriaMgrE.FindAll<InspectOrder>(criteria);
            return inspectOrders;
        }

        [Transaction(TransactionMode.Unspecified)]
        public InspectOrder LoadInspectOrder(String inspectNo, bool includeDetail)
        {
            return this.LoadInspectOrder(inspectNo, includeDetail, false);
        }


        [Transaction(TransactionMode.Unspecified)]
        public InspectOrder LoadInspectOrder(String inspectNo, bool includeDetail, bool isSort)
        {
            InspectOrder inspectOrder = this.LoadInspectOrder(inspectNo);

            if (includeDetail && inspectOrder.InspectOrderDetails != null && inspectOrder.InspectOrderDetails.Count > 0)
            {
                if (isSort)
                {
                    InspectOrderDetailComparer inspectOrderDetailComparer = new InspectOrderDetailComparer();

                    IList<InspectOrderDetail> inspectOrderDetails = inspectOrder.InspectOrderDetails;
                    List<InspectOrderDetail> targetInspectOrderDetails = new List<InspectOrderDetail>();
                    foreach (InspectOrderDetail inspectOrderDetail in inspectOrderDetails)
                    {
                        targetInspectOrderDetails.Add(inspectOrderDetail);
                    }
                    targetInspectOrderDetails.Sort(inspectOrderDetailComparer);
                    inspectOrderDetails.Clear();
                    foreach (InspectOrderDetail targetInspectOrderDetail in targetInspectOrderDetails)
                    {
                        inspectOrderDetails.Add(targetInspectOrderDetail);
                    }
                }
            }
            return inspectOrder;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<InspectOrder> GetInspectOrder(string userCode, int firstRow, int maxRows)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(InspectOrder));
            criteria.Add(Expression.Eq("CreateUser.Code", userCode));
            criteria.Add(Expression.Ge("CreateDate", DateTime.Today));
            criteria.AddOrder(Order.Desc("InspectNo"));
            IList<InspectOrder> inspectOrders = criteriaMgrE.FindAll<InspectOrder>(criteria, firstRow, maxRows);
            if (inspectOrders.Count > 0)
            {
                return inspectOrders;
            }
            return null;
        }

        #endregion Customized Methods
    }

    class InspectOrderDetailComparer : IComparer<InspectOrderDetail>
    {
        public int Compare(InspectOrderDetail x, InspectOrderDetail y)
        {

            if (x.LocationLotDetail.Item.Code == y.LocationLotDetail.Item.Code
                 && (x.LocationLotDetail.Hu == null
                        || (x.LocationLotDetail.Hu.Uom.Code == y.LocationLotDetail.Hu.Uom.Code
                 && x.LocationLotDetail.Hu.UnitCount == y.LocationLotDetail.Hu.UnitCount))
                 && x.LocationFrom.Code == y.LocationFrom.Code
                 && x.LocationTo.Code == y.LocationTo.Code
                 )
            {
                return 0;
            }
            if (x.LocationLotDetail.Item.Code != y.LocationLotDetail.Item.Code)
            {
                return string.Compare(x.LocationLotDetail.Item.Code, y.LocationLotDetail.Item.Code);
            }
            if (x.LocationLotDetail.Hu != null && x.LocationLotDetail.Hu.Uom.Code != y.LocationLotDetail.Hu.Uom.Code)
            {
                return string.Compare(x.LocationLotDetail.Hu.Uom.Code, y.LocationLotDetail.Hu.Uom.Code);
            }
            if (x.LocationLotDetail.Hu != null && x.LocationLotDetail.Hu.UnitCount != y.LocationLotDetail.Hu.UnitCount)
            {
                return x.LocationLotDetail.Hu.UnitCount.CompareTo(y.LocationLotDetail.Hu.UnitCount);
            }
            if (x.LocationFrom.Code != y.LocationFrom.Code)
            {
                return string.Compare(x.LocationFrom.Code, y.LocationFrom.Code);
            }

            return string.Compare(x.LocationTo.Code, y.LocationTo.Code);


        }
    }
}


#region Extend Interface


namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class InspectOrderMgrE : com.Sconit.Service.MasterData.Impl.InspectOrderMgr, IInspectOrderMgrE
    {

    }


}
#endregion