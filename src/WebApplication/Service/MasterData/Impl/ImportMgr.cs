using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.View;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using NHibernate.Expression;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using com.Sconit.Entity.MRP;
using com.Sconit.Service.Ext.MRP;
using System.Text;
using log4net;
using System.Data;

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class ImportMgr : IImportMgr
    {
        #region 变量
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IShiftMgrE shiftMgrE { get; set; }
        public IFlowDetailMgrE flowDetailMgrE { get; set; }
        public IItemMgrE itemMgrE { get; set; }
        public IUomMgrE uomMgrE { get; set; }
        public IUomConversionMgrE uomConversionMgrE { get; set; }
        public IHuMgrE huMgrE { get; set; }
        public IStorageBinMgrE storageBinMgrE { get; set; }
        public IItemKitMgrE itemKitMgrE { get; set; }
        public IFlowMgrE flowMgrE { get; set; }
        public IBomMgrE bomMgrE { get; set; }
        //public IBomDetailMgrE bomDetailMgrE { get; set; }
        public ICustomerMgrE customerMgrE { get; set; }
        public IItemReferenceMgrE itemReferenceMgrE { get; set; }
        public ILocationMgrE locationMgrE { get; set; }
        public ICustomerScheduleMgrE customerScheduleMgrE { get; set; }
        public IActingBillMgrE actingBillMgrE { get; set; }
        #endregion

        private static log4net.ILog log = log4net.LogManager.GetLogger("Application");

        #region IImportMgr接口实现
        [Transaction(TransactionMode.Unspecified)]
        public IList<ShiftPlanSchedule> ReadPSModelFromXls(Stream inputStream, User user, string regionCode, string flowCode, DateTime date, string shiftCode)
        {
            IList<ShiftPlanSchedule> spsList = new List<ShiftPlanSchedule>();
            IList<Shift> shifts = shiftMgrE.GetAllShift();

            //IList<FlowDetail> flowDetails = flowDetailMgrE.GetFlowDetail(flowCode, true);

            var shift = shifts.Where(s => StringHelper.Eq(s.Code, shiftCode));

            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            //if (shift == null)
            //    throw new BusinessErrorException("Import.PSModel.ShiftNotExist");

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 4);

            //if (colIndex < 0)
            //    throw new BusinessErrorException("Import.PSModel.Shift.Not.Exist", shift.ShiftName);

            IRow shiftRow = (HSSFRow)rows.Current;
            ImportHelper.JumpRows(rows, 1);

            #region 列定义
            int colSeq = 0;//seq
            int colFlow = 1;//生产线
            int colItem = 2;//物料代码
            int colBom = 4;//Bom
            #endregion

            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                if (!this.CheckValidDataRow(row, 0, 4))
                {
                    break;//边界
                }

                string fCode = string.Empty;
                string itemCode = string.Empty;
                string bomstr = string.Empty;
                string remark = string.Empty;

                Bom bom = null;
                decimal uc = 0;
                decimal planQty = 0;
                ICell cell = null;

                #region 读取生产线
                fCode = row.GetCell(colFlow).StringCellValue;
                if (fCode.Trim() == string.Empty)
                    throw new BusinessErrorException("Import.PSModel.Empty.Error.Flow", (row.RowNum + 1).ToString());

                if (flowCode != null && flowCode.Trim() != string.Empty)
                {
                    if (fCode.Trim().ToUpper() != flowCode.Trim().ToUpper())
                        continue;//生产线过滤
                }
                #endregion

                #region 读取bom
                try
                {
                    bomstr = GetCellStringValue(row.GetCell(colBom));
                    if (bomstr != null && bomstr != "0")
                    {
                        bom = bomMgrE.CheckAndLoadBom(bomstr);
                    }
                }
                catch (Exception ex)
                {
                    throw new BusinessErrorException("Bom.Error.BomCodeNotExist", bomstr);
                }
                #endregion

                #region 读取序号
                try
                {
                    //string seqStr = row.GetCell(colSeq).StringCellValue;
                    //seq = row.GetCell(colSeq).StringCellValue.Trim() != string.Empty ? int.Parse(row.GetCell(colSeq).StringCellValue) : 0;
                    uc = (decimal)(row.GetCell(colSeq).NumericCellValue);
                }
                catch
                {
                    continue;
                    //throw new BusinessErrorException("Import.PSModel.Read.Error.Seq", (row.RowNum + 1).ToString());
                }
                #endregion

                #region 读取成品代码
                try
                {
                    itemCode = GetCellStringValue(row.GetCell(colItem));
                    if (itemCode == string.Empty)
                        throw new BusinessErrorException("Import.PSModel.Empty.Error.ItemCode", (row.RowNum + 1).ToString());
                }
                catch
                {
                    throw new BusinessErrorException("Import.PSModel.Read.Error.ItemCode", (row.RowNum + 1).ToString());
                }
                #endregion

                FlowDetail flowDetail = flowDetailMgrE.LoadFlowDetail(fCode, itemCode, uc);

                //var flowDetails_ = flowDetails.Where(f => StringHelper.Eq(f.Item.Code, itemCode));
                //if (flowDetails_.Count() > 1)
                //{
                //    var flowDetails_seq = flowDetails_.Where(f => f.Sequence == seq);
                //    if (flowDetails_seq.Count() > 0)
                //    {
                //        flowDetails_ = flowDetails_seq;
                //    }
                //}
                //FlowDetail flowDetail = flowDetails_.FirstOrDefault();

                if (flowDetail == null)
                    throw new BusinessErrorException("Import.PSModel.FlowDetail.Not.Exist", (row.RowNum + 1).ToString());

                //区域权限过滤
                if (regionCode != null && regionCode.Trim() != string.Empty)
                {
                    if (regionCode.Trim().ToUpper() != flowDetail.Flow.PartyFrom.Code.ToUpper())
                        continue;
                }
                if (!user.HasPermission(flowDetail.Flow.PartyTo.Code))
                    continue;

                if (shift.Count() == 0)
                {
                    int startColIndex = 5; //从第5列开始

                    int dayOfWeek = (int)date.DayOfWeek;
                    if (dayOfWeek == 0)
                        dayOfWeek = 7;

                    startColIndex = startColIndex + (dayOfWeek - 1) * 6;
                    int endColIndex = startColIndex + 6;
                    for (int i = startColIndex; i < endColIndex; i = i + 2)
                    {
                        string shiftName = this.GetCellStringValue(shiftRow.GetCell(i));

                        var shifts_ = shifts.Where(s => StringHelper.Eq(s.ShiftName, shiftName));

                        if (shifts_.Count() == 1)
                        {
                            string qty_ = GetCellStringValue(row.GetCell(i));
                            planQty = Convert.ToDecimal(qty_ == null ? "0" : qty_);

                            //if (planQty <= 0)
                            //{
                            //    continue;
                            //}
                            remark = GetCellStringValue(row.GetCell(i + 1));

                            ShiftPlanSchedule sps = new ShiftPlanSchedule();
                            sps.FlowDetail = flowDetail;
                            sps.ReqDate = date;
                            sps.Shift = shifts_.SingleOrDefault();
                            sps.PlanQty = planQty;
                            sps.LastModifyUser = user;
                            sps.LastModifyDate = DateTime.Now;
                            sps.Bom = bom;
                            sps.Remark = remark;
                            spsList.Add(sps);
                        }
                        else if (shifts_.Count() > 1)
                        {
                            throw new BusinessErrorException("找到重复的班次");
                        }
                    }

                }
                else
                {
                    #region 读取计划量
                    try
                    {
                        int colIndex = this.GetPlanColumnIndexToRead(shiftRow, shift.SingleOrDefault().ShiftName, date);
                        cell = row.GetCell(colIndex);
                        if (cell == null || cell.CellType == NPOI.SS.UserModel.CellType.BLANK)
                            continue;

                        planQty = Convert.ToDecimal(row.GetCell(colIndex).NumericCellValue);
                        //if (planQty <= 0)
                        //{
                        //    continue;
                        //}
                        remark = GetCellStringValue(row.GetCell(colIndex + 1));
                    }
                    catch
                    {
                        throw new BusinessErrorException("Import.PSModel.Read.Error.PlanQty", (row.RowNum + 1).ToString());
                    }
                    #endregion

                    ShiftPlanSchedule sps = new ShiftPlanSchedule();
                    sps.FlowDetail = flowDetail;
                    sps.ReqDate = date;
                    sps.Shift = shift.SingleOrDefault();
                    sps.PlanQty = planQty;
                    sps.LastModifyUser = user;
                    sps.LastModifyDate = DateTime.Now;
                    sps.Bom = bom;
                    sps.Remark = remark;
                    spsList.Add(sps);
                }
            }

            if (spsList.Count == 0)
                throw new BusinessErrorException("Import.Result.Error.ImportNothing");

            spsList = spsList.Where(s => s.PlanQty > 0).ToList();

            int sequece = 1;
            foreach (ShiftPlanSchedule sps in spsList)
            {
                sps.FlowDetail.Sequence = sequece;
                sequece++;
            }
            return spsList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FlowPlan> ReadShipScheduleYFKFromXls(Stream inputStream, User user, string planType, string flowCode, string timePeriodType, DateTime date)
        {
            IList<FlowPlan> flowPlanList = new List<FlowPlan>();
            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 8);
            int colIndex = this.GetColumnIndexToRead_ShipScheduleYFK((HSSFRow)rows.Current, date);

            if (colIndex < 0)
                throw new BusinessErrorException("Import.MRP.DateNotExist", date.ToShortDateString());

            #region 列定义
            int colFlow = 1;//Flow
            int colUC = 6;//单包装
            #endregion

            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                if (!this.CheckValidDataRow(row, 1, 6))
                {
                    break;//边界
                }

                //string regCode=row.GetCell(
                string itemCode = string.Empty;
                decimal UC = 1;
                decimal planQty = 0;
                string flowCodeCell = string.Empty;

                #region 读取Flow
                try
                {
                    if (row.GetCell(colFlow) == null)
                    {
                        continue;
                    }
                    flowCodeCell = row.GetCell(colFlow).StringCellValue;
                    if (flowCodeCell.Trim() == string.Empty)
                    {
                        continue;
                    }
                    else if ((flowCode == null || flowCode.Trim() == string.Empty) || flowCodeCell == flowCode)
                    {

                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    this.ThrowCommonError(row, colIndex);
                }
                #endregion

                #region 读取成品代码
                try
                {
                    itemCode = GetCellStringValue(row.GetCell(4));
                    if (itemCode == string.Empty)
                        throw new BusinessErrorException("Import.PSModel.Empty.Error.ItemCode", (row.RowNum + 1).ToString());
                }
                catch
                {
                    throw new BusinessErrorException("Import.PSModel.Read.Error.ItemCode", (row.RowNum + 1).ToString());
                }
                #endregion

                #region 读取单包装
                try
                {
                    UC = Convert.ToDecimal(row.GetCell(colUC).NumericCellValue);
                }
                catch
                {
                    this.ThrowCommonError(row.RowNum, colUC, row.GetCell(colUC));
                }
                #endregion

                #region 读取计划量
                try
                {
                    if (row.GetCell(colIndex) == null)
                    {
                        planQty = 0;
                    }
                    else
                    {
                        planQty = Convert.ToDecimal(row.GetCell(colIndex).NumericCellValue);
                    }
                }
                catch
                {
                    throw new BusinessErrorException("Import.PSModel.Read.Error.PlanQty", (row.RowNum + 1).ToString());
                }
                #endregion

                FlowDetail flowDetail = this.LoadFlowDetailByFlow(flowCodeCell, itemCode, UC);
                if (flowDetail == null)
                    throw new BusinessErrorException("Import.MRP.Distribution.FlowDetail.Not.Exist", (row.RowNum + 1).ToString());


                //if (partyCode != null && partyCode.Trim() != string.Empty)
                //{
                //    if (!StringHelper.Eq(partyCode, flowDetail.Flow.PartyTo.Code))
                //    {
                //        continue;//客户过滤
                //    }
                //}
                //区域权限过滤
                if (!user.HasPermission(flowDetail.Flow.PartyFrom.Code) && !user.HasPermission(flowDetail.Flow.PartyTo.Code))
                {
                    continue;
                }

                FlowPlan flowPlan = new FlowPlan();
                flowPlan.FlowDetail = flowDetail;
                flowPlan.TimePeriodType = timePeriodType;
                flowPlan.ReqDate = date;
                flowPlan.PlanQty = planQty;
                flowPlan.FlowCode = flowCodeCell;
                flowPlanList.Add(flowPlan);
            }

            if (flowPlanList.Count == 0)
                throw new BusinessErrorException("Import.Result.Error.ImportNothing");

            return flowPlanList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FlowPlan> ReadShipScheduleCSFromXls(Stream inputStream, User user, string planType, string flowCode, bool isRefItem, DateTime date)
        {
            IList<FlowPlan> flowPlanList = new List<FlowPlan>();
            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 8);
            int colIndex = this.GetColumnIndexToRead_ShipScheduleYFK((HSSFRow)rows.Current, date);
            int colrefOrderNoIndex = colIndex + 1;

            if (colIndex < 0)
                throw new BusinessErrorException("Import.MRP.DateNotExist", date.ToShortDateString());

            #region 列定义
            int colFlow = 1;//Flow
            int colUC = 6;//单包装
            #endregion
            rows.MoveNext();
            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                if (!this.CheckValidDataRow(row, 1, 6))
                {
                    break;//边界
                }

                //string regCode=row.GetCell(
                string itemCode = string.Empty;
                decimal UC = 1;
                decimal planQty = 0;
                string flowCodeCell = string.Empty;
                string refOrderNo = null;
                //Flow flow = null;
                #region 读取Flow
                try
                {
                    if (row.GetCell(colFlow) == null)
                    {
                        continue;
                    }
                    flowCodeCell = row.GetCell(colFlow).StringCellValue;
                    //flow = flowMgrE.CheckAndLoadFlow(flowCodeCell);
                    if (flowCodeCell.Trim() == string.Empty)
                    {
                        continue;
                    }
                    else if ((flowCode == null || flowCode.Trim() == string.Empty) || flowCodeCell == flowCode)
                    {

                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    this.ThrowCommonError(row, colIndex);
                }
                #endregion

                #region 读取成品代码
                try
                {
                    itemCode = GetCellStringValue(row.GetCell(4));
                    if (itemCode == string.Empty)
                    {
                        throw new BusinessErrorException("Import.PSModel.Empty.Error.ItemCode", (row.RowNum + 1).ToString());
                    }
                    if (isRefItem)
                    {
                        Item item = itemReferenceMgrE.GetItemReferenceByRefItem(itemCode, null, null);
                        if (item != null)
                        {
                            itemCode = item.Code;
                        }
                        else
                        {
                            throw new BusinessErrorException("Import.PSModel.Empty.Error.ItemCode", (row.RowNum + 1).ToString());
                        }
                    }
                }
                catch
                {
                    throw new BusinessErrorException("Import.PSModel.Read.Error.ItemCode", (row.RowNum + 1).ToString());
                }
                #endregion

                #region 读取单包装
                try
                {
                    UC = Convert.ToDecimal(row.GetCell(colUC).NumericCellValue);
                }
                catch
                {
                    this.ThrowCommonError(row.RowNum, colUC, row.GetCell(colUC));
                }
                #endregion

                #region 读取计划量
                try
                {
                    if (row.GetCell(colIndex) == null)
                    {
                        planQty = 0;
                    }
                    else
                    {
                        planQty = Convert.ToDecimal(row.GetCell(colIndex).NumericCellValue);
                    }
                }
                catch
                {
                    throw new BusinessErrorException("Import.PSModel.Read.Error.PlanQty", (row.RowNum + 1).ToString());
                }

                if (planQty == 0)
                {
                    continue;
                }
                #endregion

                #region 读取参考订单号
                try
                {
                    refOrderNo = GetCellStringValue(row.GetCell(colrefOrderNoIndex));
                }
                catch
                {
                    //Nothing to do
                }
                #endregion

                FlowDetail flowDetail = this.LoadFlowDetailByFlow(flowCodeCell, itemCode, UC);
                if (flowDetail == null)
                {
                    throw new BusinessErrorException("Import.MRP.Distribution.FlowDetail.Not.Exist", (row.RowNum + 1).ToString());
                }
                if (!user.HasPermission(flowDetail.Flow.PartyFrom.Code) && !user.HasPermission(flowDetail.Flow.PartyTo.Code))
                {
                    continue;
                }

                FlowPlan flowPlan = new FlowPlan();
                flowPlan.FlowDetail = flowDetail;
                //flowPlan.TimePeriodType = timePeriodType;
                flowPlan.ReqDate = date;
                flowPlan.PlanQty = planQty;
                flowPlan.Memo = refOrderNo;
                flowPlanList.Add(flowPlan);
            }

            if (flowPlanList.Count == 0)
            {
                throw new BusinessErrorException("Import.Result.Error.ImportNothing");
            }

            return flowPlanList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FlowPlan> ReadScheduleFromXls(Stream inputStream, User user, string moduleType, string flowCode, bool isRefItem, DateTime date)
        {
            IList<FlowPlan> flowPlanList = new List<FlowPlan>();
            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 9);

            #region 列定义
            int colFlow = 1;//Flow
            int colItemCode = 2;//物料代码
            int colQty = 3;//计划数量
            int colDate = 4;//发运时间
            int colUC = 5;//单包装
            int colMemo = 6;//备注
            #endregion

            while (rows.MoveNext())
            {
                Flow flow = null;
                FlowPlan flowPlan = new FlowPlan();
                //flowPlan.TimePeriodType = timePeriodType;

                IRow row = (HSSFRow)rows.Current;
                if (!this.CheckValidDataRow(row, 1, 6))
                {
                    break;//边界
                }

                #region 读取Flow
                try
                {
                    if (row.GetCell(colFlow) == null || row.GetCell(colFlow).StringCellValue.Trim() == string.Empty)
                    {
                        continue;
                    }

                    string rowFlowCode = row.GetCell(colFlow).StringCellValue.Trim();
                    if ((flowCode == null || flowCode.Trim() == string.Empty) || rowFlowCode == flowCode)
                    {
                        flowPlan.FlowCode = rowFlowCode;
                        flow = flowMgrE.CheckAndLoadFlow(rowFlowCode);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    this.ThrowCommonError(row, colFlow);
                }
                #endregion

                #region 读取发运时间
                try
                {
                    if (row.GetCell(colDate) == null)
                    {
                        continue;
                    }
                    DateTime cellValue = row.GetCell(colDate).DateCellValue;
                    if (DateTime.Compare(cellValue.Date, date) == 0)
                    {
                        flowPlan.ReqDate = cellValue;
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    this.ThrowCommonError(row.RowNum, colDate, row.GetCell(colDate));
                }
                #endregion

                #region 读取物料代码
                try
                {
                    if (row.GetCell(colItemCode) == null)
                    {
                        throw new BusinessErrorException("Import.PSModel.Empty.Error.ItemCode", (row.RowNum + 1).ToString());
                    }
                    string itemCode = GetCellStringValue(row.GetCell(colItemCode));
                    if (itemCode == string.Empty)
                    {
                        throw new BusinessErrorException("Import.PSModel.Empty.Error.ItemCode", (row.RowNum + 1).ToString());
                    }
                    else
                    {
                        if (isRefItem)
                        {
                            string partyCode1 = null;
                            string partyCode2 = null;
                            if (flow.Type != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION)
                            {
                                partyCode1 = flow.PartyFrom.Code;
                                partyCode2 = flow.PartyTo.Code;
                            }
                            else
                            {
                                partyCode1 = flow.PartyTo.Code;
                                partyCode2 = flow.PartyFrom.Code;
                            }

                            Item item = itemReferenceMgrE.GetItemReferenceByRefItem(itemCode, partyCode1, partyCode2);
                            if (item != null)
                            {
                                flowPlan.ItemCode = item.Code;
                            }
                            else
                            {
                                throw new BusinessErrorException("Import.PSModel.Empty.Error.ItemCode", (row.RowNum + 1).ToString());
                            }
                        }
                        else
                        {
                            flowPlan.ItemCode = itemCode;
                        }
                    }
                }
                catch
                {
                    throw new BusinessErrorException("Import.PSModel.Read.Error.ItemCode", (row.RowNum + 1).ToString());
                }
                #endregion

                #region 读取计划量
                try
                {
                    if (row.GetCell(colQty) == null)
                    {
                        flowPlan.PlanQty = 0;
                    }
                    else
                    {
                        flowPlan.PlanQty = Convert.ToDecimal(row.GetCell(colQty).NumericCellValue);
                    }
                }
                catch
                {
                    throw new BusinessErrorException("Import.PSModel.Read.Error.PlanQty", (row.RowNum + 1).ToString());
                }
                #endregion

                #region 读取单包装
                try
                {
                    if (row.GetCell(colUC) != null)
                    {
                        flowPlan.UC = Convert.ToDecimal(row.GetCell(colUC).NumericCellValue);
                    }
                }
                catch
                {
                    this.ThrowCommonError(row.RowNum, colUC, row.GetCell(colUC));
                }
                #endregion

                #region 读取备注
                try
                {
                    if (row.GetCell(colMemo) != null)
                    {
                        flowPlan.Memo = GetCellStringValue(row.GetCell(colMemo));
                    }
                }
                catch
                {
                    //Nothing to do
                }
                #endregion

                flowPlan.Seq = row.RowNum + 1;

                flowPlanList.Add(flowPlan);
            }

            List<string> flowCodes = flowPlanList.Select(f => f.FlowCode).Distinct().ToList();

            List<FlowDetail> flowDetails = new List<FlowDetail>();

            foreach (string f in flowCodes)
            {
                IList<FlowDetail> flowDetailList = flowDetailMgrE.GetFlowDetail(f, true);
                if (flowDetailList != null && flowDetailList.Count > 0 &&
                    user.HasPermission(flowDetailList[0].Flow.PartyTo.Code) &&
                    user.HasPermission(flowDetailList[0].Flow.PartyFrom.Code))
                {
                    if (flowDetailList[0].Flow.Type != moduleType)
                    {
                        throw new BusinessErrorException("Import.PSModel.FlowType.Error", moduleType);
                    }
                    flowDetails.AddRange(flowDetailList);
                }
            }

            IList<FlowPlan> newflowPlanList = new List<FlowPlan>();
            /*
            int seq = 1;
            foreach (FlowPlan flowPlan in flowPlanList)
            {
                bool mark = false;
                foreach (FlowPlan newfp in newflowPlanList)
                {
                    if (((flowPlan.UC == null || flowPlan.UC == 0) && (flowPlan.FlowCode == newfp.FlowCode && flowPlan.ItemCode == newfp.ItemCode)) ||
                        (flowPlan.FlowCode == newfp.FlowCode && flowPlan.ItemCode == newfp.ItemCode && flowPlan.UC == newfp.UC))
                    {
                        newfp.PlanQty += flowPlan.PlanQty;
                        if (flowPlan.Memo != null && flowPlan.Memo != string.Empty
                            && newfp.Memo != null && newfp.Memo != string.Empty)
                        {
                            newfp.Memo += "+" + flowPlan.Memo;
                            if (newfp.Memo.Length > 255)
                            {
                                newfp.Memo = newfp.Memo.Substring(0, 255);
                            }
                        }
                        mark = true;
                        break;
                    }
                }

                if (!mark)
                {
                    foreach (FlowDetail fd in flowDetails)
                    {
                        if ((flowPlan.UC == null || flowPlan.UC == 0) && (flowPlan.FlowCode == fd.Flow.Code && flowPlan.ItemCode == fd.Item.Code) ||
                            (flowPlan.FlowCode == fd.Flow.Code && flowPlan.ItemCode == fd.Item.Code && flowPlan.UC == fd.UnitCount))
                        {
                            flowPlan.FlowDetail = fd;
                            flowPlan.FlowDetail.Sequence = seq;
                            newflowPlanList.Add(flowPlan);
                            seq++;
                            mark = true;
                            break;
                        }
                    }
                }
                if (!mark)
                {
                    notMatchFlowPlans.Add(flowPlan);
                }
            }
            */

            string notMatchItems = string.Empty;
            foreach (FlowPlan flowPlan in flowPlanList)
            {
                var flowDetail = flowDetails.FirstOrDefault(p => flowPlan.FlowCode == p.Flow.Code && flowPlan.ItemCode == p.Item.Code);
                if (flowDetail!=null)
                {
                    flowPlan.FlowDetail = flowDetail;
                    newflowPlanList.Add(flowPlan);
                }
                else
                {
                    if (notMatchItems == string.Empty)
                    {
                        notMatchItems += "IRow" + flowPlan.Seq.ToString() + "Item" + flowPlan.ItemCode;
                    }
                    else
                    {
                        notMatchItems += "_Row" + flowPlan.Seq.ToString() + "Item" + flowPlan.ItemCode;
                    }
                }
            }
            if (notMatchItems != string.Empty)
            {
                throw new BusinessErrorException("Import.PSModel.FlowDetail.NotExist", notMatchItems);
            }
            if (newflowPlanList.Count == 0)
            {
                throw new BusinessErrorException("Import.Result.Error.ImportNothing");
            }
            return newflowPlanList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<CycleCountDetail> ReadCycleCountFromXls(Stream inputStream, User user, CycleCount cycleCount)
        {
            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            //区域权限过滤
            if (!user.HasPermission(cycleCount.Location.Region.Code))
            {
                throw new BusinessErrorException("Common.Business.Error.NoPartyPermission", cycleCount.Location.Region.Code);
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 11);

            #region 列定义
            int colItem = 1;//物料代码
            int colUom = 3;//单位
            int colQty = 4;//数量
            int colHu = 5;//条码
            int colBin = 6;//库格
            int colStartTime = 7;//开始时间
            int colEndTime = 8;//结束时间
            #endregion

            DateTime dateTimeNow = DateTime.Now;
            IList<CycleCountDetail> cycleCountDetailList = new List<CycleCountDetail>();
            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                if (!this.CheckValidDataRow(row, 1, 9))
                {
                    break;//边界
                }

                DateTime? startTime = cycleCount.StartDate;
                DateTime? endTime = dateTimeNow;

                if (row.GetCell(colHu) == null || row.GetCell(colHu).ToString() == string.Empty)
                {
                    string itemCode = string.Empty;
                    decimal qty = 0;
                    string uomCode = string.Empty;

                    #region 读取数据
                    #region 读取物料代码
                    itemCode = GetCellStringValue(row.GetCell(colItem));
                    if (itemCode == null || itemCode.Trim() == string.Empty)
                        this.ThrowCommonError(row.RowNum, colItem, row.GetCell(colItem));

                    var i = (
                        from c in cycleCountDetailList
                        where c.HuId == null && c.Item.Code.Trim().ToUpper() == itemCode.Trim().ToUpper()
                        select c).Count();

                    if (i > 0)
                        throw new BusinessErrorException("Import.Business.Error.Duplicate", itemCode, (row.RowNum + 1).ToString(), (colItem + 1).ToString());
                    #endregion

                    #region 读取数量
                    try
                    {
                        //按容积率导入，用hu盘点时不考虑此算法
                        if (!cycleCount.IsPlotRatio)
                            qty = Convert.ToDecimal(row.GetCell(colQty).NumericCellValue);
                        else
                            qty = Convert.ToDecimal(row.GetCell(colQty).NumericCellValue) * cycleCount.Location.Volume.Value;
                    }
                    catch
                    {
                        this.ThrowCommonError(row.RowNum, colQty, row.GetCell(colQty));
                    }
                    #endregion

                    #region 读取单位
                    uomCode = row.GetCell(colUom) != null ? row.GetCell(colUom).StringCellValue : string.Empty;
                    if (uomCode == null || uomCode.Trim() == string.Empty)
                        throw new BusinessErrorException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colUom.ToString());
                    #endregion

                    #region 读取开始时间
                    if (row.GetCell(colStartTime) != null)
                    {
                        try
                        {
                            startTime = row.GetCell(colStartTime).DateCellValue;
                        }
                        catch
                        {
                            this.ThrowCommonError(row.RowNum, colStartTime, row.GetCell(colStartTime));
                        }

                        //if (startTime < cycleCount.StartDate)
                        //{
                        //    throw new BusinessErrorException("MasterData.Inventory.Stocktaking.Error.StartTimeLtStartDate", itemCode);
                        //}
                    }
                    else
                    {
                        //startTime = null;
                    }
                    #endregion

                    #region 读取结束时间
                    if (row.GetCell(colEndTime) != null)
                    {
                        try
                        {
                            endTime = row.GetCell(colEndTime).DateCellValue;
                        }
                        catch
                        {
                            this.ThrowCommonError(row.RowNum, colEndTime, row.GetCell(colEndTime));
                        }

                        //if (endTime > dateTimeNow)
                        //{
                        //    throw new BusinessErrorException("MasterData.Inventory.Stocktaking.Error.EndDateGtNow", itemCode);
                        //}
                        //else if (endTime < startTime)
                        //{
                        //    throw new BusinessErrorException("MasterData.Inventory.Stocktaking.Error.EndDateGtStartTime", itemCode);
                        //}
                    }
                    else
                    {
                        endTime = null;
                    }
                    #endregion
                    #endregion

                    #region 填充数据
                    Item item = itemMgrE.CheckAndLoadItem(itemCode);
                    Uom uom = uomMgrE.CheckAndLoadUom(uomCode);
                    //单位换算
                    if (item.Uom.Code.Trim().ToUpper() != uom.Code.Trim().ToUpper())
                    {
                        qty = uomConversionMgrE.ConvertUomQty(item, uom, qty, item.Uom);
                    }

                    #region 套件处理
                    IDictionary<Item, decimal> newItemDic = new Dictionary<Item, decimal>();

                    decimal? convertRate = null;
                    IList<ItemKit> itemKitList = null;
                    if (item.Type == BusinessConstants.CODE_MASTER_ITEM_TYPE_VALUE_K)
                    {
                        itemKitList = itemKitMgrE.GetChildItemKit(item);
                        foreach (ItemKit itemKit in itemKitList)
                        {
                            if (!convertRate.HasValue)
                            {
                                if (itemKit.ParentItem.Uom.Code != uom.Code)
                                {
                                    convertRate = uomConversionMgrE.ConvertUomQty(item, uom, 1, itemKit.ParentItem.Uom);
                                }
                                else
                                {
                                    convertRate = 1;
                                }
                            }
                            newItemDic.Add(itemKit.ChildItem, convertRate.Value * qty * itemKit.Qty);

                        }
                    }
                    else
                    {
                        newItemDic.Add(item, qty);
                    }
                    #endregion

                    foreach (KeyValuePair<Item, decimal> entry in newItemDic)
                    {

                        var j = (
                            from c in cycleCountDetailList
                            where c.HuId == null && c.Item.Code.Trim().ToUpper() == entry.Key.Code.Trim().ToUpper()
                            select c).Count();

                        if (j > 0)
                        {
                            foreach (CycleCountDetail detail in cycleCountDetailList)
                            {
                                if (detail.Item.Code == entry.Key.Code)
                                {
                                    detail.Qty += entry.Value;
                                }
                            }
                        }
                        else
                        {
                            CycleCountDetail cycleCountDetail = new CycleCountDetail();
                            cycleCountDetail.CycleCount = cycleCount;
                            cycleCountDetail.Item = entry.Key;
                            cycleCountDetail.Qty = entry.Value;
                            cycleCountDetail.StartTime = startTime;
                            cycleCountDetail.EndTime = endTime;
                            cycleCountDetailList.Add(cycleCountDetail);
                        }
                    }
                    #endregion
                }
                else
                {
                    string huId = string.Empty;
                    string binCode = string.Empty;

                    #region 读取数据
                    #region 读取条码
                    huId = row.GetCell(colHu) != null ? row.GetCell(colHu).StringCellValue : string.Empty;
                    if (huId == null || huId.Trim() == string.Empty)
                        throw new BusinessErrorException("Import.Read.Error.Empty", (row.RowNum + 1).ToString(), colHu.ToString());

                    var i = (
                        from c in cycleCountDetailList
                        where c.HuId != null && c.HuId.Trim().ToUpper() == huId.Trim().ToUpper()
                        select c).Count();

                    if (i > 0)
                        throw new BusinessErrorException("Import.Business.Error.Duplicate", huId, (row.RowNum + 1).ToString(), colHu.ToString());
                    #endregion

                    #region 读取库格
                    binCode = row.GetCell(colBin) != null ? row.GetCell(colBin).StringCellValue : null;
                    #endregion
                    #endregion

                    #region 填充数据
                    Hu hu = huMgrE.CheckAndLoadHu(huId);
                    StorageBin bin = null;
                    if (binCode != null && binCode.Trim() != string.Empty)
                    {
                        bin = storageBinMgrE.CheckAndLoadStorageBin(binCode);
                    }

                    CycleCountDetail cycleCountDetail = new CycleCountDetail();
                    cycleCountDetail.CycleCount = cycleCount;
                    cycleCountDetail.Item = hu.Item;
                    cycleCountDetail.Qty = hu.Qty * hu.UnitQty;
                    cycleCountDetail.HuId = hu.HuId;
                    cycleCountDetail.LotNo = hu.LotNo;
                    cycleCountDetail.StorageBin = bin.Code;
                    cycleCountDetail.StartTime = startTime;
                    cycleCountDetail.EndTime = endTime;
                    cycleCountDetailList.Add(cycleCountDetail);
                    #endregion
                }
            }

            if (cycleCountDetailList.Count == 0)
                throw new BusinessErrorException("Import.Result.Error.ImportNothing");

            return cycleCountDetailList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FlowPlan> ReadShipSchedulePanaFromXls(Stream inputStream, User user, DateTime startDate, DateTime endDate, string flowCode, bool isItemRef)
        {
            IList<FlowPlan> flowPlanList = new List<FlowPlan>();
            if (inputStream.Length == 0)
            {
                throw new BusinessErrorException("Import.Stream.Empty");
            }
            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 5);
            IRow dateRow = (HSSFRow)rows.Current;
            int startColIndex = this.GetColumnIndexFromShipSchedule(dateRow, startDate, 3);
            int endColIndex = this.GetColumnIndexFromShipSchedule(dateRow, endDate, 3);

            Flow flow = flowMgrE.LoadFlow(flowCode, true, true);
            if (flow == null)
            {
                throw new BusinessErrorException("此路线不存在");
            }
            if (!user.HasPermission(flow.PartyFrom.Code) && !user.HasPermission(flow.PartyTo.Code))
            {
                throw new BusinessErrorException("没有此路线权限");
            }

            #region 列定义
            int colItem = 0;
            int colShift = 2;
            #endregion

            Item item = null;
            FlowDetail flowDetail = null;
            string itemReference = null;

            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;

                string rowIndex = (row.RowNum + 1).ToString();

                //if (!this.CheckValidDataRow(row, 1, 34))
                //{
                //    break;//边界
                //}
                string shift = null;

                #region 读取 colShift
                try
                {
                    shift = this.GetCellStringValue(row.GetCell(colShift));
                    if (shift == null)
                    {
                        continue;
                    }
                }
                catch
                {
                    this.ThrowCommonError(row, startColIndex);
                }
                #endregion

                #region 读取Item
                if (shift == "A")
                {
                    try
                    {
                        string itemCode = this.GetCellStringValue(row.GetCell(colItem));

                        if (isItemRef)
                        {
                            item = itemReferenceMgrE.GetItemReferenceByRefItem(itemCode, flow.PartyTo.Code, flow.PartyFrom.Code);
                            itemReference = itemCode;
                        }
                        else
                        {
                            item = itemMgrE.LoadItem(itemCode);
                            itemReference = itemReferenceMgrE.GetItemReferenceByItem(itemCode, flow.PartyTo.Code, flow.PartyFrom.Code);
                        }

                        if (item == null)
                        {
                            //continue;
                            throw new BusinessErrorException("物料号不存在");
                        }

                        var qf = flow.FlowDetails.Where(f => StringHelper.Eq(f.Item.Code, item.Code));

                        if (qf.Count() > 0)
                        {
                            flowDetail = qf.First();
                        }
                        else
                        {
                            throw new BusinessErrorException("Import.MRP.Distribution.FlowDetail.Not.Exist", rowIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ThrowCommonError(row, startColIndex);
                    }
                }
                #endregion

                if (flowDetail == null)
                {
                    throw new BusinessErrorException("Import.MRP.Distribution.FlowDetail.Not.Exist", rowIndex);
                }

                #region 读取数量
                try
                {
                    for (int colIndex = startColIndex; colIndex <= endColIndex; colIndex++)
                    {
                        ICell dateCell = dateRow.GetCell(colIndex);

                        DateTime colDate = DateTime.Now;
                        try
                        {
                            colDate = dateCell.DateCellValue;
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        FlowPlan flowPlan = new FlowPlan();
                        flowPlan.FlowDetail = flowDetail;
                        flowPlan.ReqDate = colDate;
                        flowPlan.Shift = shift;
                        flowPlan.TimePeriodType = BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY;

                        string qty = this.GetCellStringValue(row.GetCell(colIndex));

                        if (qty != null)
                        {
                            flowPlan.PlanQty = decimal.Parse(qty);
                        }
                        if (flowPlan.PlanQty >= 0)
                        {
                            flowPlanList.Add(flowPlan);
                        }
                        else if (flowPlan.PlanQty < 0)
                        {
                            throw new BusinessErrorException("Import.ShipSchedule.Qty.MustGreatThan0", rowIndex);
                        }
                    }
                }
                catch (Exception)
                {
                    throw new BusinessErrorException("Import.ShipSchedule.Qty.Error", rowIndex);
                }
                #endregion

            }

            if (flowPlanList.Count == 0)
            {
                throw new BusinessErrorException("Import.Result.Error.ImportNothing");
            }

            return flowPlanList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public CustomerSchedule ReadCustomerSchedulePanaFromXls(Stream inputStream, User user, DateTime startDate, DateTime endDate,
            string flowCode, string refScheduleNo, bool isItemRef)
        {
            IList<FlowPlan> flowPlans = ReadShipSchedulePanaFromXls(inputStream, user, startDate, endDate, flowCode, isItemRef);
            var q = from f in flowPlans
                    group f by new { f.FlowDetail, f.ItemReference, f.ReqDate, f.TimePeriodType } into result
                    select new CustomerScheduleDetail
                    {
                        DateFrom = Utility.DateTimeHelper.GetStartTime(result.Key.TimePeriodType, result.Key.ReqDate),
                        DateTo = Utility.DateTimeHelper.GetEndTime(result.Key.TimePeriodType, result.Key.ReqDate),
                        Item = result.Key.FlowDetail.Item.Code,
                        ItemDescription = result.Key.FlowDetail.Item.Description,
                        ItemReference = result.Key.ItemReference,
                        Location = result.Key.FlowDetail.DefaultLocationFrom.Code,
                        Qty = flowPlans.Where(fp => StringHelper.Eq(fp.TimePeriodType, result.Key.TimePeriodType)
                            && fp.FlowDetail.Id == result.Key.FlowDetail.Id && fp.ReqDate == result.Key.ReqDate).Sum(fp => fp.PlanQty),
                        StartTime = (DateTimeHelper.GetStartTime(result.Key.TimePeriodType, result.Key.ReqDate))
                            .AddHours(-(double)(result.Key.FlowDetail.Flow.LeadTime.HasValue ? result.Key.FlowDetail.Flow.LeadTime.Value : 0M)),
                        Type = result.Key.TimePeriodType,
                        UnitCount = result.Key.FlowDetail.UnitCount,
                        Uom = result.Key.FlowDetail.Uom.Code
                    };
            CustomerSchedule customerSchedule = new CustomerSchedule();
            customerSchedule.Flow = flowCode;
            customerSchedule.ReferenceScheduleNo = refScheduleNo;
            customerSchedule.CustomerScheduleDetails = q.ToList();
            return customerSchedule;
        }

        [Transaction(TransactionMode.Unspecified)]
        public CustomerSchedule ReadCustomerScheduleFromXls(Stream inputStream, User user, DateTime? startDate, DateTime? endDate,
            string flowCode, string refScheduleNo, bool isItemRef)
        {
            if (inputStream.Length == 0)
            {
                throw new BusinessErrorException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);

            IEnumerator rows = sheet.GetRowEnumerator();

            #region 读取路线,参考日程号等
            if (flowCode == null || flowCode.Trim() == string.Empty)
            {
                throw new BusinessErrorException("MRP.Schedule.Import.CustomerSchedule.Result.SelectFlow");
            }
            IRow flowRow = sheet.GetRow(1);
            string xlsFlowCode = GetCellStringValue(flowRow.GetCell(2));
            if (xlsFlowCode == null)
            {
                throw new BusinessErrorException("MRP.Schedule.Import.CustomerSchedule.Result.FillFLowInTemplate");
            }
            else if (!StringHelper.Eq(xlsFlowCode, flowCode))
            {
                throw new BusinessErrorException("MRP.Schedule.Import.CustomerSchedule.Result.SelectedFlowNotMatchTheFlowInTemplate");
            }
            Flow flow = flowMgrE.CheckAndLoadFlow(flowCode, true, true);
            //todo 权限判断

            decimal leadTime = flow.LeadTime.HasValue ? flow.LeadTime.Value : 0M;

            IRow refOrderNoRow = sheet.GetRow(2);
            string referenceScheduleNo = GetCellStringValue(refOrderNoRow.GetCell(2));
            if (referenceScheduleNo != null && !StringHelper.Eq(referenceScheduleNo, refScheduleNo))
            {
                throw new BusinessErrorException("MRP.Schedule.Import.CustomerSchedule.Result.RefCustomerScheduleNotMatchThatInTemplate");
            }

            IRow typeRow = sheet.GetRow(5);
            IRow dateRow = sheet.GetRow(6);

            //IList<CustomerSchedule> customerSchedules = customerScheduleMgrE.GetCustomerSchedules(flowCode, refScheduleNo, null, null, null);
            //if (customerSchedules.Count > 0)
            //{
            //    throw new BusinessErrorException("MRP.Schedule.Import.CustomerSchedule.Result.CannotImportSameRefCustomerSchedule");
            //}

            #endregion

            #region CustomerSchedule
            CustomerSchedule customerSchedule = new CustomerSchedule();
            customerSchedule.ReferenceScheduleNo = refScheduleNo;
            customerSchedule.Flow = flowCode;
            customerSchedule.CustomerScheduleDetails = new List<CustomerScheduleDetail>();
            #endregion

            ImportHelper.JumpRows(rows, 7);

            #region 列定义
            int colItemCode = 0;//物料代码或参考物料号
            int colItemDescription = 1;//物料描述
            int colUom = 2;//单位
            int colUc = 3;//单包装
            #endregion

            while (rows.MoveNext())
            {
                Item item = null;
                Uom uom = null;
                decimal? uc = null;
                string itemReference = null;
                string location = null;

                IRow row = (HSSFRow)rows.Current;
                if (!this.CheckValidDataRow(row, 0, 3))
                {
                    break;//边界
                }
                string rowIndex = (row.RowNum + 1).ToString();

                #region 读取物料代码
                try
                {
                    string itemCode = GetCellStringValue(row.GetCell(colItemCode));
                    if (itemCode == null)
                    {
                        throw new BusinessErrorException("Import.ShipSchedule.ItemCode.Empty", rowIndex);
                    }
                    if (isItemRef)
                    {
                        item = itemReferenceMgrE.GetItemReferenceByRefItem(itemCode, flow.PartyTo.Code, flow.PartyFrom.Code);
                        itemReference = itemCode;
                    }
                    else
                    {
                        item = itemMgrE.LoadItem(itemCode);
                        itemReference = itemReferenceMgrE.GetItemReferenceByItem(itemCode, flow.PartyTo.Code, flow.PartyFrom.Code);
                    }
                    if (item == null)
                    {
                        throw new BusinessErrorException("Import.ShipSchedule.Item.NotExist", itemCode, rowIndex);
                    }
                }
                catch
                {
                    throw new BusinessErrorException("Import.ShipSchedule.ItemCode.Error", rowIndex);
                }
                #endregion

                #region 读取单位
                try
                {
                    string uomCode = GetCellStringValue(row.GetCell(colUom));
                    if (uomCode != null)
                    {
                        uom = uomMgrE.CheckAndLoadUom(uomCode);
                    }
                }
                catch
                {
                    this.ThrowCommonError(row, colUom);
                }
                #endregion

                #region 读取单包装
                try
                {
                    string uc_ = GetCellStringValue(row.GetCell(colUc));
                    if (uc_ != null)
                    {
                        uc = Convert.ToDecimal(uc_);
                    }
                }
                catch
                {
                    this.ThrowCommonError(row, colUc);
                }
                #endregion

                #region 使用flowDet过滤
                bool isMatch = false;
                if (flow.FlowDetails != null)
                {

                    var q = flow.FlowDetails.Where(f => StringHelper.Eq(f.Item.Code, item.Code));
                    if (uom != null)
                    {
                        q = q.Where(f => StringHelper.Eq(f.Uom.Code, uom.Code));
                    }
                    if (uc.HasValue)
                    {
                        q = q.Where(f => uc.Value == f.UnitCount);
                    }
                    if (q.Count() > 0)
                    {
                        uom = q.FirstOrDefault().Uom;
                        uc = q.FirstOrDefault().UnitCount;
                        location = q.FirstOrDefault().DefaultLocationFrom.Code;
                        isMatch = true;
                    }
                }
                if (!isMatch)
                {
                    if (flow.AllowCreateDetail)
                    {
                        uc = uc == null ? item.UnitCount : uc;
                        uom = uom == null ? item.Uom : uom;
                        location = flow.LocationFrom.Code;
                    }
                    else
                    {
                        throw new BusinessErrorException("Import.MRP.Distribution.FlowDetail.Not.Exist", rowIndex);
                    }
                }
                #endregion

                #region 读取数量
                try
                {
                    for (int i = 4; ; i++)
                    {
                        string periodType = GetCellStringValue(typeRow.GetCell(i));

                        if (periodType != BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_DAY &&
                            periodType != BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_MONTH &&
                            periodType != BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_WEEK)
                        {
                            break;
                        }
                        ICell dateCell = dateRow.GetCell(i);
                        DateTime? dateCellValue = null;
                        if (dateCell != null && dateCell.CellType == CellType.NUMERIC)
                        {
                            dateCellValue = dateCell.DateCellValue;
                        }
                        else
                        {
                            break;
                        }
                        if (startDate.HasValue && dateCellValue.Value.Date < startDate.Value.Date)
                        {
                            continue;
                        }
                        if (endDate.HasValue && dateCellValue.Value.Date > endDate.Value.Date)
                        {
                            break;
                        }
                        string qtyValue = GetCellStringValue(row.GetCell(i));
                        decimal qty = 0M;
                        if (qtyValue != null)
                        {
                            qty = Convert.ToDecimal(qtyValue);
                        }
                        if (qty < 0M)
                        {
                            throw new BusinessErrorException("Import.ShipSchedule.Qty.MustGreatThan0", rowIndex);
                        }
                        else
                        {
                            CustomerScheduleDetail customerScheduleDetail = new CustomerScheduleDetail();
                            customerScheduleDetail.DateFrom = DateTimeHelper.GetStartTime(periodType, dateCellValue.Value);
                            customerScheduleDetail.DateTo = DateTimeHelper.GetEndTime(periodType, dateCellValue.Value);
                            customerScheduleDetail.Item = item.Code;
                            customerScheduleDetail.ItemDescription = item.Description;
                            customerScheduleDetail.ItemReference = itemReference;
                            customerScheduleDetail.Location = location;
                            customerScheduleDetail.Type = periodType;
                            customerScheduleDetail.UnitCount = uc.Value;
                            customerScheduleDetail.Uom = uom.Code;
                            customerScheduleDetail.StartTime = customerScheduleDetail.DateFrom.AddHours(-(double)leadTime);
                            customerScheduleDetail.Qty = qty;
                            customerSchedule.CustomerScheduleDetails.Add(customerScheduleDetail);
                        }
                    }
                }
                catch (Exception)
                {
                    throw new BusinessErrorException("Import.ShipSchedule.Qty.Error", rowIndex);
                }
                #endregion
            }
            customerSchedule.CustomerScheduleDetails = customerSchedule.CustomerScheduleDetails.OrderBy(c => c.StartTime).ToList();

            return customerSchedule;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<ActingBill> ReadActingBillFromXls(Stream inputStream, string partyCode, User user)
        {
            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 1);

            #region 列定义
            int colRecNo = 0;//收货单号
            int colRefRecNo = 1;//回单
            int colItem = 2;//物料代码
            int colRefItem = 4;//参考物料
            int colQty = 5;//数量
            int colPrice = 6;//单价
            int colSettleTime = 7;//结算日期
            #endregion

            DateTime dateTimeNow = DateTime.Now;
            IList<ActingBill> actinBills = new List<ActingBill>();
            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                string rowIndex = (row.RowNum + 1).ToString();
                ActingBill actingBill = new ActingBill();
                actingBill.ErrorMessage = string.Empty;

                if (!this.CheckValidDataRow(row, 0, 7))
                {
                    break;//边界
                }
                #region 读取数据

                #region 读取物料代码
                try
                {
                    string itemCode = GetCellStringValue(row.GetCell(colItem));
                    if (itemCode != null)
                    {
                        Item item = itemMgrE.LoadItem(itemCode);
                        if (item == null)
                        {
                            actingBill.ErrorMessage += " 系统中物料号不存在";
                        }
                        else
                        {
                            actingBill.Item = item;
                            actingBill.ReferenceItemCode = itemReferenceMgrE.GetItemReferenceByItem(itemCode, partyCode, null);
                        }
                    }
                }
                catch (Exception)
                {
                    actingBill.ErrorMessage += " 读取物料号出错";
                }
                #endregion

                #region 读取参考物料代码
                try
                {
                    string refItem = GetCellStringValue(row.GetCell(colRefItem));
                    if (refItem != null)
                    {
                        Item item = itemReferenceMgrE.GetItemReferenceByRefItem(refItem, partyCode, null);
                        if (item == null)
                        {
                            actingBill.ErrorMessage += " 系统中没有此参考物料号: " + refItem;
                        }
                        else
                        {
                            actingBill.Item = item;
                            actingBill.ReferenceItemCode = refItem;
                        }
                    }
                }
                catch (Exception)
                {
                    actingBill.ErrorMessage += " 读取参考物料号出错";
                }
                #endregion


                if (actingBill.Item == null)
                {
                    actingBill.ErrorMessage += " 系统中物料号不存在";
                }

                try
                {
                    ICell cell = row.GetCell(colPrice);

                    if (cell.CellType == CellType.NUMERIC)
                    {
                        actingBill.UnitPrice = Convert.ToDecimal(cell.NumericCellValue);
                    }
                    else
                    {
                        string price = GetCellStringValue(cell);
                        actingBill.UnitPrice = decimal.Parse(price);
                    }
                }
                catch (Exception)
                {
                    //actingBill.ErrorMessage += " 读取Excel单价出错:" + rowIndex;
                }

                try
                {
                    ICell cell = row.GetCell(colQty);
                    if (cell.CellType == CellType.NUMERIC)
                    {
                        actingBill.CurrentBillQty = Convert.ToDecimal(cell.NumericCellValue);
                    }
                    else
                    {
                        string qty = GetCellStringValue(cell);
                        actingBill.CurrentBillQty = decimal.Parse(qty);
                    }
                }
                catch (Exception)
                {
                    actingBill.ErrorMessage += " 读取Excel数量出错";
                }

                try
                {
                    actingBill.ReceiptNo = GetCellStringValue(row.GetCell(colRecNo));
                }
                catch (Exception)
                {
                    actingBill.ErrorMessage += " 读取Excel收货单出错";
                }

                try
                {
                    actingBill.ExternalReceiptNo = GetCellStringValue(row.GetCell(colRefRecNo));
                }
                catch (Exception)
                {
                    actingBill.ErrorMessage += " 读取Excel回单单出错";
                }

                try
                {
                    ICell cell = row.GetCell(colSettleTime);
                    if (cell.CellType == CellType.NUMERIC)
                    {
                        actingBill.EffectiveDate = cell.DateCellValue;
                    }
                    else
                    {
                        actingBill.EffectiveDate = DateTime.Parse(GetCellStringValue(cell));
                    }
                }
                catch (Exception)
                {
                    //actingBill.ErrorMessage += " 读取Excel日期出错:" + rowIndex;
                }
                actingBill.RowIndex = rowIndex;
                #endregion
                actinBills.Add(actingBill);
            }

            return actinBills;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<ActingBill> ReadActingBillFromXls1(Stream inputStream)
        {
            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 1);

            #region 列定义
            int colId = 0;//Id
            int colQty = 1;//数量
            #endregion

            DateTime dateTimeNow = DateTime.Now;
            IList<ActingBill> actinBills = new List<ActingBill>();
            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                string rowIndex = (row.RowNum + 1).ToString();
                ActingBill actingBill = new ActingBill();
                actingBill.ErrorMessage = string.Empty;

                if (!this.CheckValidDataRow(row, 0, 1))
                {
                    break;//边界
                }
                #region 读取数据

                try
                {
                    ICell cellId = row.GetCell(colId);
                    int id = 0;
                    if (cellId.CellType == CellType.NUMERIC)
                    {
                        id = Convert.ToInt32(cellId.NumericCellValue);
                    }
                    else
                    {
                        id = int.Parse(GetCellStringValue(cellId));
                    }
                    actingBill = actingBillMgrE.LoadActingBill(id);
                }
                catch (Exception)
                {
                    actingBill.ErrorMessage += " 读取Id出错";
                }

                try
                {
                    ICell cellQty = row.GetCell(colQty);
                    if (cellQty.CellType == CellType.NUMERIC)
                    {
                        actingBill.CurrentBillQty = Convert.ToDecimal(cellQty.NumericCellValue);
                    }
                    else
                    {
                        string qty = GetCellStringValue(cellQty);
                        actingBill.CurrentBillQty = decimal.Parse(qty);
                    }
                }
                catch (Exception)
                {
                    actingBill.ErrorMessage += " 读取数量出错";
                }

                actingBill.RowIndex = rowIndex;
                #endregion
                actinBills.Add(actingBill);
            }
            return actinBills;
        }

        [Transaction(TransactionMode.Unspecified)]
        public Dictionary<string, decimal> ReadBillFromXls(Stream inputStream, string partyCode, User user)
        {
            if (inputStream.Length == 0)
                throw new BusinessErrorException("Import.Stream.Empty");

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 1);

            #region 列定义
            int colItem = 0;//物料代码
            int colItemDescription = 1;//物料代码
            int colRefItem = 2;//参考物料
            int colQty = 3;//数量
            string itemCode = null;
            double qty = 0;
            #endregion

            DateTime dateTimeNow = DateTime.Now;
            Dictionary<string, decimal> bills = new Dictionary<string, decimal>();

            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                string rowIndex = (row.RowNum + 1).ToString();

                if (!this.CheckValidDataRow(row, 0, 3))
                {
                    break;//边界
                }
                #region 读取数据

                #region 读取物料代码
                try
                {
                    itemCode = GetCellStringValue(row.GetCell(colItem));
                    if (itemCode != null)
                    {
                        Item item = itemMgrE.CheckAndLoadItem(itemCode);
                    }
                }
                catch (Exception)
                {
                    throw new BusinessErrorException("物料号错误", rowIndex);
                }
                #endregion

                #region 读取参考物料代码
                try
                {
                    string refItem = GetCellStringValue(row.GetCell(colRefItem));
                    if (refItem != null)
                    {
                        Item item = itemReferenceMgrE.GetItemReferenceByRefItem(refItem, partyCode, null);
                        if (item == null)
                        {
                            throw new BusinessErrorException("读取参考物料号出错", rowIndex);
                        }
                        else
                        {
                            itemCode = item.Code;
                        }
                    }
                }
                catch (Exception)
                {
                    throw new BusinessErrorException("读取参考物料号出错", rowIndex);
                }
                #endregion

                if (itemCode == null)
                {
                    throw new BusinessErrorException("物料号错误", rowIndex);
                }

                try
                {
                    ICell cell = row.GetCell(colQty);
                    if (cell.CellType == CellType.NUMERIC)
                    {
                        qty = cell.NumericCellValue;
                    }
                    else
                    {
                        qty = double.Parse(GetCellStringValue(cell));
                    }
                }
                catch (Exception)
                {
                    throw new BusinessErrorException("读取数量错误", rowIndex);
                }
                #endregion
                bills.Add(itemCode, (decimal)qty);
            }
            return bills;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FlowPlan> ReadNewPanaOrderFromCSV(Stream inputStream, User user, string flowCode, bool isRefItem, DateTime startDate, DateTime endDate)
        {
            if (inputStream.Length == 0)
            {
                throw new BusinessErrorException("Import.Stream.Empty");
            }
            Flow flow = flowMgrE.LoadFlow(flowCode);
            if (flow != null && !user.HasPermission(flow.PartyTo.Code))
            {
                throw new BusinessErrorException("没有此路线的权限");
            }
            IList<FlowPlan> flowPlanList = new List<FlowPlan>();

            //DataSet dataSet = sqlHelperMgr.GetDatasetBySql("select Item,Party,RefCode from ItemRef");
            //IList<ItemRef> itemRefs = IListHelper.DataTableToList<ItemRef>(dataSet.Tables[0]);
            #region 列定义

            //	0	接收订单者码（全球）	21012072
            //	1	接收订单者名称	亚虹塑料
            //	2	订货者代码（全球）	29122
            //	3	订货者名称	上海松下微波炉
            //	4	建立日期	20120920171050
            //	5	保税／征税分类	1
            //	6	订单号	766438
            //	7	传票号码	153103
            int colRefOrderNo = 7;
            //	8	零件代码	F6903-1N40  2
            int colRefItemCode = 8;
            //	9	零件代码（全球）	F6903-1N40
            //	10	零件名称	变压器支架
            //	11	货币	CNY
            //	12	单价	1.95
            //	13	交货指示数量	11100
            int colQty = 13;
            //	14	订货金额	21645
            //	15	货期	20121011
            int colWindowTime = 15;
            //	16	数量单位	PC
            //int colUom = 16;
            //	17	包装单位（常规）	150
            //	18	包装单位（最小）	150
            //	19	交货场所	N3
            //	20	保管场所	SLD
            //	21	备注	AM
            int colShift = 21;

            #endregion
            try
            {
                FlatFileReader reader = new FlatFileReader(inputStream, Encoding.Default);
                int i = 0;
                for (string[] lineData = reader.ReadLine(); lineData != null; lineData = reader.ReadLine())
                {
                    if (i >= 1)
                    {
                        if (lineData.Length > 0)
                        {
                            FlowPlan flowPlan = new FlowPlan();

                            #region 读取发运时间
                            string cellWindowTime = GetLineDataValue(lineData, colWindowTime);

                            string windowTime = cellWindowTime.Substring(0, 4) + "-" + cellWindowTime.Substring(4, 2) + "-" + cellWindowTime.Substring(6, 2);
                            DateTime reqDate = DateTime.Parse(windowTime).Date;
                            if (reqDate >= startDate.Date && reqDate <= endDate.Date)
                            {
                                flowPlan.ReqDate = reqDate;
                            }
                            else
                            {
                                i++;
                                log.Debug(i);
                                continue;
                            }
                            #endregion

                            #region 读取物料代码
                            string itemCode = GetLineDataValue(lineData, colRefItemCode);
                            if (itemCode == string.Empty)
                            {
                                throw new BusinessErrorException("导入物料:" + itemCode + "出错,行:" + i.ToString());
                            }
                            else
                            {
                                if (isRefItem)
                                {
                                    string partyCode1 = null;
                                    string partyCode2 = null;
                                    if (flow.Type != BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION)
                                    {
                                        partyCode1 = flow.PartyFrom.Code;
                                        partyCode2 = flow.PartyTo.Code;
                                    }
                                    else
                                    {
                                        partyCode1 = flow.PartyTo.Code;
                                        partyCode2 = flow.PartyFrom.Code;
                                    }

                                    Item item = itemReferenceMgrE.GetItemReferenceByRefItem(itemCode, partyCode1, partyCode2);
                                    if (item != null)
                                    {
                                        flowPlan.ItemCode = item.Code;
                                        flowPlan.ItemReference = itemCode;
                                    }
                                    else
                                    {
                                        throw new BusinessErrorException("没有找到客户物料号:" + itemCode + ",行:" + i.ToString());
                                    }
                                }
                                else
                                {
                                    flowPlan.ItemCode = itemCode;
                                }
                            }

                            #endregion

                            #region 读取计划量
                            flowPlan.PlanQty = Convert.ToDecimal(GetLineDataValue(lineData, colQty));
                            #endregion

                            #region 读取备注
                            flowPlan.Memo = GetLineDataValue(lineData, colRefOrderNo);
                            #endregion

                            #region 读取班次
                            flowPlan.Shift = GetLineDataValue(lineData, colShift);
                            #endregion

                            flowPlan.Seq = i;
                            flowPlan.FlowCode = flowCode;

                            flowPlanList.Add(flowPlan);
                        }
                    }
                    i++;
                    log.Debug(i);
                }
            }
            catch (Exception ex)
            {
                inputStream.Dispose();
                throw new BusinessErrorException(ex.Message);
            }
            finally
            {
                inputStream.Dispose();
            }

            IList<FlowDetail> flowDetailList = flowDetailMgrE.GetFlowDetail(flowCode, true);

            IList<FlowPlan> newflowPlanList = new List<FlowPlan>();
            int seq = 1;
            IList<FlowPlan> notMatchFlowPlans = new List<FlowPlan>();
            foreach (FlowPlan flowPlan in flowPlanList)
            {
                bool mark = false;
                foreach (FlowDetail fd in flowDetailList)
                {
                    if (flowPlan.ItemCode == fd.Item.Code)
                    {
                        flowPlan.FlowDetail = fd;
                        flowPlan.FlowDetail.Sequence = seq;
                        flowPlan.UC = fd.UnitCount;
                        newflowPlanList.Add(flowPlan);
                        seq++;
                        mark = true;
                        break;
                    }
                }
                if (!mark)
                {
                    notMatchFlowPlans.Add(flowPlan);
                }
            }

            if (notMatchFlowPlans.Count > 0)
            {
                string notMatchItems = string.Empty;
                foreach (FlowPlan notMatchFlowPlan in notMatchFlowPlans)
                {
                    if (notMatchItems == string.Empty)
                    {
                        notMatchItems += "IRow" + notMatchFlowPlan.Seq.ToString() + "Item" + notMatchFlowPlan.ItemCode;
                    }
                    else
                    {
                        notMatchItems += "_Row" + notMatchFlowPlan.Seq.ToString() + "Item" + notMatchFlowPlan.ItemCode;
                    }
                }
                throw new BusinessErrorException("Import.PSModel.FlowDetail.NotExist", notMatchItems);
            }

            if (newflowPlanList.Count == 0)
            {
                throw new BusinessErrorException("Import.Result.Error.ImportNothing");
            }

            return newflowPlanList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public CustomerSchedule ReadNewPanaPlanFromCSV(Stream inputStream, User user, string flowCode, bool isRefItem, DateTime startDate, DateTime endDate, string refScheduleNo, string timePeriodType)
        {
            IList<FlowPlan> flowPlans = ReadNewPanaOrderFromCSV(inputStream, user, flowCode, isRefItem, startDate, endDate);
            var q = from f in flowPlans
                    group f by new
                    {
                        FlowDetail = f.FlowDetail,
                        ItemReference = f.ItemReference,
                        DateFrom = Utility.DateTimeHelper.GetStartTime(timePeriodType, f.ReqDate),
                        DateTo = Utility.DateTimeHelper.GetEndTime(timePeriodType, f.ReqDate),
                    } into result
                    select new CustomerScheduleDetail
                    {
                        DateFrom = result.Key.DateFrom,
                        DateTo = result.Key.DateTo,
                        Item = result.Key.FlowDetail.Item.Code,
                        ItemDescription = result.Key.FlowDetail.Item.Description,
                        ItemReference = result.Key.ItemReference,
                        Location = result.Key.FlowDetail.DefaultLocationFrom.Code,
                        Qty = result.Sum(p => p.PlanQty),
                        StartTime = result.Key.DateFrom
                            .AddHours(-(double)(result.Key.FlowDetail.Flow.LeadTime.HasValue ? result.Key.FlowDetail.Flow.LeadTime.Value : 0M)),
                        Type = timePeriodType,
                        UnitCount = result.Key.FlowDetail.UnitCount,
                        Uom = result.Key.FlowDetail.Uom.Code
                    };
            CustomerSchedule customerSchedule = new CustomerSchedule();
            customerSchedule.Flow = flowCode;
            customerSchedule.ReferenceScheduleNo = refScheduleNo;
            customerSchedule.CustomerScheduleDetails = q.ToList();
            return customerSchedule;
        }

        #endregion

        #region Private Method
        private int GetPlanColumnIndexToRead(IRow row, string shiftName, DateTime date)
        {
            int colIndex = -1;
            int startColIndex = 5; //从第5列开始

            int dayOfWeek = (int)date.DayOfWeek;
            if (dayOfWeek == 0)
                dayOfWeek = 7;

            startColIndex = startColIndex + (dayOfWeek - 1) * 6;
            for (int i = startColIndex; i < row.LastCellNum; i = i + 2)
            {
                ICell cell = row.GetCell(i);
                string cellValue = cell.StringCellValue;
                if (cellValue == shiftName)
                {
                    colIndex = i;
                    break;
                }
            }

            return colIndex;
        }

        private int GetColumnIndexToRead_ShipSchedulePana(IRow row, DateTime date)
        {
            int colIndex = -1;
            int startColIndex = 3; //从第3列开始
            int day = int.Parse(date.ToString("dd"));

            for (int i = startColIndex; i < row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell != null && cell.CellType == CellType.NUMERIC)
                {
                    int cellValue = 0;
                    try
                    {
                        cellValue = (int)cell.NumericCellValue;
                    }
                    catch (Exception e)
                    {
                        throw new BusinessErrorException("Import.Error");
                    }
                    if (day == cellValue)
                    {
                        colIndex = i;
                        break;
                    }
                }
            }
            return colIndex;
        }

        private int GetColumnIndexToRead_ShipScheduleYFK(IRow row, DateTime date)
        {
            int colIndex = -1;
            int startColIndex = 7; //从第7列开始

            for (int i = startColIndex; i < row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell != null && cell.CellType == CellType.NUMERIC)
                {
                    DateTime cellValue = DateTime.Now;
                    try
                    {
                        cellValue = cell.DateCellValue;
                    }
                    catch (Exception e)
                    {
                        throw new BusinessErrorException("Import.Error");
                    }
                    if (DateTime.Compare(cellValue, date) == 0)
                    {
                        colIndex = i;
                        break;
                    }
                }
            }
            return colIndex;
        }

        private int GetColumnIndexFromShipSchedule(IRow row, DateTime date, int startColIndex)
        {
            int colIndex = -1;
            //int startColIndex = 7; //从第7列开始

            for (int i = startColIndex; i < row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell != null && cell.CellType == CellType.NUMERIC)
                {
                    DateTime cellValue = DateTime.Now;
                    try
                    {
                        cellValue = cell.DateCellValue;
                    }
                    catch (Exception e)
                    {
                        throw new BusinessErrorException("Import.Error");
                    }
                    if (DateTime.Compare(cellValue, date) == 0)
                    {
                        colIndex = i;
                        break;
                    }
                }
            }
            return colIndex;
        }


        private bool CheckValidDataRow(IRow row, int startColIndex, int endColIndex)
        {
            for (int i = startColIndex; i < endColIndex; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell != null && cell.CellType != NPOI.SS.UserModel.CellType.BLANK)
                {
                    return true;
                }
            }

            return false;
        }

        private void ThrowCommonError(IRow row, int colIndex)
        {
            this.ThrowCommonError(row.RowNum, colIndex, row.GetCell(colIndex));
        }
        private void ThrowCommonError(int rowIndex, int colIndex, ICell cell)
        {
            string errorValue = string.Empty;
            if (cell != null)
            {
                if (cell.CellType == NPOI.SS.UserModel.CellType.STRING)
                {
                    errorValue = cell.StringCellValue;
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.NUMERIC)
                {
                    errorValue = cell.NumericCellValue.ToString("0.########");
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.BOOLEAN)
                {
                    errorValue = cell.NumericCellValue.ToString();
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.BLANK)
                {
                    errorValue = "Null";
                }
                else
                {
                    errorValue = "Unknow value";
                }
            }
            throw new BusinessErrorException("Import.Read.CommonError", (rowIndex + 1).ToString(), (colIndex + 1).ToString(), errorValue);
        }

        private string GetCellStringValue(ICell cell)
        {
            string strValue = null;
            if (cell != null)
            {
                if (cell.CellType == CellType.STRING)
                {
                    strValue = cell.StringCellValue;
                }
                else if (cell.CellType == CellType.NUMERIC)
                {
                    strValue = cell.NumericCellValue.ToString("0.########");
                }
                else if (cell.CellType == CellType.BOOLEAN)
                {
                    strValue = cell.NumericCellValue.ToString();
                }
                else if (cell.CellType == CellType.FORMULA)
                {
                    if (cell.CachedFormulaResultType == CellType.STRING)
                    {
                        strValue = cell.StringCellValue;
                    }
                    else if (cell.CachedFormulaResultType == CellType.NUMERIC)
                    {
                        strValue = cell.NumericCellValue.ToString("0.########");
                    }
                    else if (cell.CachedFormulaResultType == CellType.BOOLEAN)
                    {
                        strValue = cell.NumericCellValue.ToString();
                    }
                }
            }
            if (strValue != null)
            {
                strValue = strValue.Trim();
            }
            strValue = strValue == string.Empty ? null : strValue;
            return strValue;
        }

        [Transaction(TransactionMode.Unspecified)]
        private FlowDetail LoadFlowDetailByFlow(string flowCode, string itemCode, decimal UC)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FlowView));
            criteria.CreateAlias("FlowDetail", "fd");
            criteria.Add(Expression.Eq("Flow.Code", flowCode));
            criteria.Add(Expression.Eq("fd.Item.Code", itemCode));
            IList<FlowView> flowViewList = criteriaMgrE.FindAll<FlowView>(criteria);

            FlowDetail flowDetail = null;
            if (flowViewList != null && flowViewList.Count > 0)
            {
                var q1 = flowViewList.Where(f => f.FlowDetail.UnitCount == UC).Select(f => f.FlowDetail);
                if (q1.Count() > 0)
                {
                    flowDetail = q1.First();
                }
                else
                {
                    flowDetail = flowViewList[0].FlowDetail;
                }
            }

            return flowDetail;
        }

        private string GetLineDataValue(string[] lineData, int colIndex)
        {
            if (lineData.Length < colIndex || colIndex == 0)
            {
                return null;
            }
            else
            {
                string colData = lineData[colIndex];

                return colData == null ? null : colData.Trim();
            }
        }


        #endregion
    }


}

#region Extend Interface
namespace com.Sconit.Service.Ext.MasterData.Impl
{
    public partial class ImportMgrE : com.Sconit.Service.MasterData.Impl.ImportMgr, IImportMgrE
    {

    }
}

#endregion