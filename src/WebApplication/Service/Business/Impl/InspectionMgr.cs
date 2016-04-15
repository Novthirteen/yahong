using com.Sconit.Service.Ext.Business;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MasterData;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.Report;

namespace com.Sconit.Service.Business.Impl
{
    public class InspectionMgr : AbstractBusinessMgr
    {
        public ISetBaseMgrE setBaseMgrE { get; set; }
        public ISetDetailMgrE setDetailMgrE { get; set; }
        public IExecuteMgrE executeMgrE { get; set; }
        public ILocationLotDetailMgrE locationLotDetailMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public IInspectOrderMgrE inspectOrderMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public IReportMgrE reportMgrE { get; set; }
        public IRegionMgrE regionMgr { get; set; }

        protected override void SetBaseInfo(Resolver resolver)
        {
        }

        protected override void GetDetail(Resolver resolver)
        {
        }

        protected override void SetDetail(Resolver resolver)
        {
            LocationLotDetail locationLotDetail = locationLotDetailMgrE.CheckLoadHuLocationLotDetail(resolver.Input, resolver.UserCode);
            TransformerDetail transformerDetail = TransformerHelper.ConvertLocationLotDetailToTransformerDetail(locationLotDetail, false);
            resolver.AddTransformerDetail(transformerDetail);
            resolver.Command = BusinessConstants.CS_BIND_VALUE_TRANSFORMERDETAIL;
        }

        protected override void ExecuteSubmit(Resolver resolver)
        {
            this.CreateInspectOrder(resolver);
        }

        protected override void ExecuteCancel(Resolver resolver)
        {
            executeMgrE.CancelOperation(resolver);
        }

        /// <summary>
        /// 报验单
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Unspecified)]
        public void CreateInspectOrder(Resolver resolver)
        {
            IList<LocationLotDetail> locationLotDetailList = executeMgrE.ConvertTransformersToLocationLotDetails(resolver.Transformers, false);
            if (locationLotDetailList.Count == 0)
            {
                throw new BusinessErrorException("MasterData.Inventory.Repack.Error.RepackDetailEmpty");
            }

            var list = locationLotDetailList.GroupBy(p => p.Location).ToList();
            if (list.Count > 1)  //检验报验的条码不能属于多个库位
            {
                throw new BusinessErrorException("InspectOrder.Error.MultiLocationFrom");
            }

            User user = userMgrE.LoadUser(resolver.UserCode, false, true);
            InspectOrder inspectOrder = inspectOrderMgrE.CreateInspectOrder(locationLotDetailList,
                this.regionMgr.GetDefaultInspectLocation(list[0].Key.Region.Code),
                this.regionMgr.GetDefaultRejectLocation(list[0].Key.Region.Code), user);
            resolver.Result = languageMgrE.TranslateMessage("MasterData.InspectOrder.Create.Successfully", resolver.UserCode, inspectOrder.InspectNo);
            resolver.Transformers = null;
            resolver.Code = inspectOrder.InspectNo;
            resolver.Command = BusinessConstants.CS_BIND_VALUE_TRANSFORMER;
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override void ExecutePrint(Resolver resolver)
        {
            resolver.PrintUrl = reportMgrE.WriteToFile("InspectReceipt.xls", resolver.Code);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override void GetReceiptNotes(Resolver resolver)
        {
            string[] row = resolver.Code.Split('|');
            int firstRow = int.Parse(row[0]);
            int maxRows = int.Parse(row[1]);
            IList<InspectOrder> inspectOrders = inspectOrderMgrE.GetInspectOrder(resolver.UserCode, firstRow, maxRows);

            if (inspectOrders != null)
            {
                List<ReceiptNote> receiptNotes = new List<ReceiptNote>();
                int seq = 1;
                foreach (InspectOrder inspectOrder in inspectOrders)
                {
                    ReceiptNote receiptNote = new ReceiptNote();
                    receiptNote.CreateDate = inspectOrder.CreateDate;
                    receiptNote.CreateUser = inspectOrder.CreateUser == null ? string.Empty : inspectOrder.CreateUser.Name;
                    receiptNote.IpNo = inspectOrder.IpNo;
                    receiptNote.ReceiptNo = inspectOrder.ReceiptNo;
                    receiptNote.OrderNo = inspectOrder.InspectNo;
                    receiptNote.Status = inspectOrder.Status;
                    receiptNote.Sequence = seq;
                    receiptNotes.Add(receiptNote);
                    seq++;
                }
                resolver.ReceiptNotes = receiptNotes;
            }
        }
    }
}


#region Extend Interface

namespace com.Sconit.Service.Ext.Business.Impl
{
    public partial class InspectionMgrE : com.Sconit.Service.Business.Impl.InspectionMgr, IBusinessMgrE
    {

    }
}

#endregion
