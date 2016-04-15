using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.View;
using com.Sconit.Service.Ext;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using NHibernate.Expression;
using NHibernate.Transform;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace com.Sconit.Service.Batch.Job
{
    [Transactional]
    public class PercentPassJob : RepJob
    {

        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            string key = "日合格率";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;Excel附件包含: ");
            desc.Append(separator);
            desc.Append("1. 检验合格率：按照库位排序展示报验的合格率(注: 查看具体数量，请在表格的上方把组合展开)");
            desc.Append(separator);
            desc.Append("2. 合格率：每个生产线一个Sheet(注: 查看具体数量，请在表格的上方把组合展开)");
            desc.Append(separator);            
            desc.Append("3. 生产废品统计");
            desc.Append(separator);
            desc.Append("4. 工废: 原因为工废的计划外出库单统计");
            desc.Append(separator);
            desc.Append("5. 返工统计: 返工单的下线信息");
            desc.Append(separator);
            desc.Append("6. 客户退货(日)");
            desc.Append(separator);
            desc.Append("7. 客户退货(月)");
            desc.Append(separator);
            DateTime now = DateTime.Now;

            string percentPassStartTime = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_PERCENTPASS_STARTTIME).Value;

            string[] percentPassStartTimeArr = percentPassStartTime.Split(BusinessConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
            if (percentPassStartTimeArr == null || percentPassStartTimeArr.Length == 0) return;
            DateTime startDate;
            DateTime endDate;
            if (percentPassStartTimeArr.Length == 1)
            {
                startDate = DateTime.Parse(now.AddDays(-1).ToString("yyyy-MM-dd " + percentPassStartTimeArr[0]));
                endDate = startDate.AddDays(1);
            }
            else if (percentPassStartTimeArr.Length == 2)
            {
                startDate = DateTime.Parse(now.AddDays(-1).ToString("yyyy-MM-dd " + percentPassStartTimeArr[0]));
                endDate = DateTime.Parse(now.ToString("yyyy-MM-dd " + percentPassStartTimeArr[1]));
                //todo 2012-10-31有数据
                //endDate = new DateTime(2012, 10, 31);
                //startDate = endDate.AddDays(-30);
                //now = endDate;
            }
            else
            {
                return;
            }

            #region 日报、周报、月报、季度
            //日报
            {
                StringBuilder commentDay = new StringBuilder();
                commentDay.Append(desc.ToString());
                commentDay.Append("数据时间为：" + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentDay.ToString(), startDate, endDate);
            }
            //周报
            if (now.DayOfWeek == DayOfWeek.Monday)
            {
                key = "周合格率";
                startDate = endDate.AddDays(-7);
                StringBuilder commentWeek = new StringBuilder();
                commentWeek.Append(desc.ToString());
                commentWeek.Append("数据时间为：" + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentWeek.ToString(), startDate, endDate);
            }

            //月报
            if (now.Day == 26)
            {
                key = "月合格率";
                startDate = endDate.AddMonths(-1);
                StringBuilder commentMonth = new StringBuilder();
                commentMonth.Append(desc.ToString());
                commentMonth.Append("数据时间为：" + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentMonth.ToString(), startDate, endDate);
            }
            //季度
            if ((now.Day == 26) && (now.Month == 12 || now.Month == 3 || now.Month == 6 || now.Month == 9))
            {
                key = "季度合格率";
                startDate = endDate.AddMonths(-3);
                StringBuilder commentMonth = new StringBuilder();
                commentMonth.Append(desc.ToString());
                commentMonth.Append("数据时间为：" + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentMonth.ToString(), startDate, endDate);
            }

            //年中
            /*
            if (now.Day == 1 &&  now.Month == 7)
            {
                key = "年中合格率";
                startDate = endDate.AddMonths(-6);
                StringBuilder comment6Month = new StringBuilder();
                comment6Month.Append(desc.ToString());
                comment6Month.Append("数据时间为：" + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, comment6Month.ToString(), startDate, endDate);
            }
            
            //年报

            if (now.DayOfYear == 1)
            {
                key = "年合格率";
                startDate = endDate.AddYears(-1);
                StringBuilder commentYear = new StringBuilder();
                commentYear.Append(desc.ToString());
                commentYear.Append("数据时间为：" + startDate.ToString("yyyy-MM-dd HH:mm") + " 至 " + endDate.ToString("yyyy-MM-dd HH:mm"));

                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentYear.ToString(), startDate, endDate);
            }
            */
            #endregion
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {

            //检验
            bool result2 = ProcessInspect(workbook, startDate, endDate);

            //合格率
            bool result1 = ProcessPassWO(workbook, startDate, endDate);

            //逾期生产单和取消取消生产单
            //bool result3 = ProcessWO(workbook, startDate, endDate);

            //工废
            bool result5 = ProcessUnpIndustrialWaste(workbook, startDate, endDate);

            //返工
            bool result6 = ProcessRwo(workbook, startDate, endDate);

            //客户退货
            bool result4 = false;
            if (key == "日合格率")
            {
                result4 = ProcessOrder(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN, BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION, workbook, startDate, endDate);
            }

            return result1 || result2 || result4 || result5 || result6;
        }


        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessInspect(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                #region 获取报验确认结果数据
                StringBuilder inspectResultSql = new StringBuilder();
                inspectResultSql.Append(@"select l1.Name+'['+l1.Code+']' LocFrom, ");
                inspectResultSql.Append(@"Count(m.InspNo) InspNoCount,Count(r.InspComfirmResultNo) ComfirmCount, ");
                inspectResultSql.Append(@"l2.Name+'['+l2.Code+']' RejLoc, ");
                inspectResultSql.Append(@"l3.Name+'['+l3.Code+']' InspLoc, ");
                inspectResultSql.Append(@"i.Desc1+'['+i.Code+']' Item,i.Uom,i.UC, ");
                inspectResultSql.Append(@"sum(d.InspQty) InspQty,sum(d.QualifyQty) QualifyQty,sum(d.RejectQty) RejectQty, ");
                inspectResultSql.Append(@"sum(d.PendingQualifyQty) PendingQualifyQty,sum(d.PendingRejectQty) PendingRejectQty, ");
                inspectResultSql.Append(@"sum(r.QualifyQty) CurrQualifyQty,sum(r.RejectQty) CurrRejectQty ");
                inspectResultSql.Append(@"from InspectComfirmResult r  ");
                inspectResultSql.Append(@"join InspectDet d on r.InspDetId = d.Id  ");
                inspectResultSql.Append(@"join InspectMstr m on m.InspNo=d.InspNo  ");
                inspectResultSql.Append(@"join LocationLotDet lld on lld.Id=d.LocLotDetId  ");
                inspectResultSql.Append(@"join Item i on lld.Item=i.Code  ");
                inspectResultSql.Append(@"join Location l1 on d.LocFrom=l1.Code  ");
                inspectResultSql.Append(@"left join Location l2 on m.RejLoc=l2.Code  ");
                inspectResultSql.Append(@"left join Location l3 on m.InspLoc=l3.Code  ");
                inspectResultSql.Append(@"where r.CreateDate>= '" + startDate.Value + "' and r.CreateDate<'" + endDate.Value + "' ");
                inspectResultSql.Append(@"group by l1.Code,l1.Name,l2.Code,l2.Name,l3.Code,l3.Name,i.Code,i.Desc1,i.Uom,i.UC ");

                DataSet inspectResultDS = sqlHelperMgrE.GetDatasetBySql(inspectResultSql.ToString());
                List<InspectResult> inspectResultList = IListHelper.DataTableToList<InspectResult>(inspectResultDS.Tables[0]);

                #endregion

                if (inspectResultList != null && inspectResultList.Count > 0)
                {

                    #region 获取报验单头数据
                    /*
                    StringBuilder inspSql = new StringBuilder();

                    inspSql.Append(@"select m.InspNo,m.IpNo,m.RecNo , ");
                    inspSql.Append(@"l2.Name+'['+l2.Code+']' RejLoc, ");
                    inspSql.Append(@"l3.Name+'['+l3.Code+']' InspLoc, ");
                    inspSql.Append(@"c.Desc1,m.CreateDate,u.USR_FirstName+u.USR_LastName+'['+u.USR_Code+']' ");
                    inspSql.Append(@"from InspectMstr m  ");
                    inspSql.Append(@"join ACC_User u on m.CreateUser=u.USR_Code ");
                    inspSql.Append(@"join CodeMstr c on c.CodeValue=m.Status and c.Code='Status' ");
                    inspSql.Append(@"join Location l2 on m.RejLoc=l2.Code  ");
                    inspSql.Append(@"join Location l3 on m.InspLoc=l3.Code ");

                    DataSet inspDS = sqlHelperMgrE.GetDatasetBySql(inspSql.ToString());
                    List<> inspList = IListHelper.DataTableToList<>(inspDS.Tables[0]);
                 */
                    #endregion


                    #region 初始化Sheet

                    ISheet inspSheet = workbook.CreateSheet("检验合格率");

                    for (int i = 0; i <= 16; i++)
                    {
                        inspSheet.AutoSizeColumn(i);
                        inspSheet.SetDefaultColumnStyle(i, cellStyle);
                    }
                    inspSheet.SetColumnWidth(0, 26 * 256);
                    inspSheet.SetColumnWidth(1, 25 * 256);
                    inspSheet.SetColumnWidth(2, 7 * 256);
                    inspSheet.SetColumnWidth(4, 25 * 256);
                    inspSheet.SetColumnWidth(5, 26 * 256);
                    inspSheet.SetColumnWidth(7, 13 * 256);
                    inspSheet.SetColumnWidth(13, 15 * 256);
                    inspSheet.SetColumnWidth(14, 16 * 256);
                    inspSheet.SetColumnWidth(15, 13 * 256);
                    inspSheet.SetColumnWidth(16, 15 * 256);
                    int rownum = 0;
                    int colnum = 0;

                    #endregion
                    /*
                colnum = 0;
                
                XlsHelper.SetRowCell(sheet1, rownum, colnum++, "物料数：");
                XlsHelper.SetRowCell(sheet1, rownum, colnum++, inspectResultList.Count() + " 张", headStyle);

                XlsHelper.SetRowCell(sheet1, rownum, colnum++, "本次合格率：");
                XlsHelper.SetRowCell(sheet1, rownum, colnum++, inspectResultList.Sum(r => r.CurrQualifyQty / (r.CurrQualifyQty + r.CurrRejectQty)).Count() + " 笔", headStyle);
                rownum++;
                 */

                    #region 输出列头
                    colnum = 0;
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "来源库位", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "物料", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "单位", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "单包装", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "待验库位", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "不合格品库位", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "单数合计", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "确认次数合计", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "待验数", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "合格数", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "不合格数", headStyle);

                    IDrawing patr = inspSheet.CreateDrawingPatriarch();
                    IComment comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 3, rownum + 2));
                    comment.String = new HSSFRichTextString("完成率=合格数/待验数");
                    comment.Author = "LPP Team";

                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "完成率", comment, headStyle);

                    comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 3, rownum + 2));
                    comment.String = new HSSFRichTextString("合格率=合格数/(合格数+不合格率)");
                    comment.Author = "LPP Team";

                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "合格率", comment, headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "合格待确认数", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "不合格待确认数", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "本次合格数", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "本次不合格数", headStyle);

                    comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 6, rownum + 2));
                    comment.String = new HSSFRichTextString("本次合格率=本次合格数/(本次合格数+本次不合格率)");
                    comment.Author = "LPP Team";

                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "本次合格率", comment, headStyle);
                    #endregion

                    #region 输出数据
                    string locFrom = string.Empty;
                    inspectResultList = inspectResultList.OrderBy(r => r.CurrPassRate).ToList();
                    foreach (var inspectResult in inspectResultList)
                    {
                        rownum++;
                        colnum = 0;
                        if (string.IsNullOrEmpty(locFrom) || locFrom != inspectResult.LocFrom)
                        {
                            XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.LocFrom, cellStyleColor);
                        }
                        else
                        {
                            XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.LocFrom);
                        }
                        locFrom = inspectResult.LocFrom;

                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.Item);
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.Uom);
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.UC.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.InspLoc);
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.RejLoc);
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.InspNoCount.ToString());
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.ComfirmCount.ToString());
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.InspQty.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.QualifyQty.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.RejectQty.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.CompleteRate.ToString("P"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.PassRate.ToString("p"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.PendingQualifyQty.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.PendingRejectQty.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.CurrQualifyQty.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.CurrRejectQty.ToString("0.########"));
                        XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResult.CurrPassRate.ToString("p"));
                    }

                    #region 输出合计
                    rownum += 2;
                    XlsHelper.SetRowCell(inspSheet, rownum, 0, "合计", headStyle);
                    XlsHelper.SetMergedRegion(inspSheet, rownum, 0, rownum, 7);
                    colnum = 8;

                    InspectResult inspectResultTotal = new InspectResult();
                    inspectResultTotal.InspQty = inspectResultList.Sum(r => r.InspQty);
                    inspectResultTotal.QualifyQty = inspectResultList.Sum(r => r.QualifyQty);
                    inspectResultTotal.RejectQty = inspectResultList.Sum(r => r.RejectQty);
                    inspectResultTotal.PendingQualifyQty = inspectResultList.Sum(r => r.PendingQualifyQty);
                    inspectResultTotal.PendingRejectQty = inspectResultList.Sum(r => r.PendingRejectQty);
                    inspectResultTotal.CurrQualifyQty = inspectResultList.Sum(r => r.CurrQualifyQty);
                    inspectResultTotal.CurrRejectQty = inspectResultList.Sum(r => r.CurrRejectQty);

                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.InspQty.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.QualifyQty.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.RejectQty.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.CompleteRate.ToString("P"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.PassRate.ToString("P"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.PendingQualifyQty.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.PendingRejectQty.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.CurrQualifyQty.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.CurrRejectQty.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, inspectResultTotal.CurrPassRate.ToString("P"), headStyle);

                    #endregion

                    inspSheet.GroupColumn(2, 10);
                    inspSheet.SetColumnGroupCollapsed(10, true);

                    inspSheet.ForceFormulaRecalculation = true;
                    inspSheet.CreateFreezePane(0, 1, 0, 1);

                    #endregion

                    return true;
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessPassWO(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                IList<EntityPreference> entityPreferenceList = entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[]{BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME,
                                                    //BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYCODE,
                                                    BusinessConstants.ENTITY_PREFERENCE_WEBADDRESS,
                                                    BusinessConstants.ENTITY_PREFERENCE_CODE_PERCENTPASS_FLOWCODE,
                                                    BusinessConstants.ENTITY_PREFERENCE_CODE_RESISTANCE_FLOWCODE,
                                                    BusinessConstants.ENTITY_PREFERENCE_CODE_PERCENTPASS_TRENCH});
                string percentPassFlowCode = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_PERCENTPASS_FLOWCODE).SingleOrDefault().Value;
                string[] percentPassFlowCodeArr = percentPassFlowCode.Split(BusinessConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);

                string resistanceLine = string.Empty;
                EntityPreference resistanceLineOpt = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_RESISTANCE_FLOWCODE)
                                     .SingleOrDefault();
                if (resistanceLineOpt != null && !string.IsNullOrEmpty(resistanceLineOpt.Value))
                {
                    resistanceLine = resistanceLineOpt.Value;
                }

                var passRateTotalDic = new Dictionary<string, string[]>();
                if (percentPassFlowCodeArr != null && percentPassFlowCodeArr.Length > 0)
                {
                    StringBuilder flowHql = new StringBuilder();
                    IDictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("FlowCodeArray", percentPassFlowCodeArr);
                    flowHql.Append(@"select f.Code,f.Description from Flow f where f.Code in (:FlowCodeArray) and f.IsActive = 1 ");
                    IList<object[]> flowList = this.hqlMgrE.FindAll<object[]>(flowHql.ToString(), param);
                    if (flowList != null && flowList.Count > 0)
                    {
                        #region 获取合格率数据

                        StringBuilder receiptSql = new StringBuilder();

                        receiptSql.Append(@"select s.ShiftName +'['+s.Code+']' Shift,om.Flow,i.Code Item,od.Uom,i.Desc1 ItemDesc,  ");
                        receiptSql.Append(@"sum(od.OrderQty) OrderQty,sum(r.RecQty) RecQty,sum(r.RejQty) RejQty,sum(r.ScrapQty) ScrapQty, ");
                        receiptSql.Append(@"AVG(i.NumField3 * 0.8) Price,AVG(i.NumField3 * 0.8) * sum(r.ScrapQty) ScrapAmount ");
                        //,c1.Desc1 SubType
                        receiptSql.Append(@"from OrderDet od join OrderMstr om on od.OrderNo=om.OrderNo and om.SubType in ('" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML + "','" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO + "') ");
                        receiptSql.Append(@"    join ( ");
                        receiptSql.Append(@"            select olt.OrderDetId, sum(rd.RecQty) RecQty,sum(rd.RejQty) RejQty,sum(rd.ScrapQty) ScrapQty ");
                        receiptSql.Append(@"            from ReceiptDet rd join ReceiptMstr rm on rd.RecNo=rm.RecNo ");
                        receiptSql.Append(@"            join OrderLocTrans olt on rd.OrderLocTransId=olt.id ");
                        receiptSql.Append(@"            where rm.OrderType='" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION + "' ");
                        receiptSql.Append(@"            and rm.CreateDate >= @StartDate ");
                        receiptSql.Append(@"            and rm.CreateDate < @EndDate ");
                        receiptSql.Append(@"            group by olt.OrderDetId ");
                        receiptSql.Append(@"         ) r on r.OrderDetId = od.Id ");
                        receiptSql.Append(@"join ShiftMstr s on om.Shift = s.Code ");
                        receiptSql.Append(@"join Item i on od.Item = i.Code ");
                        receiptSql.Append(@"where ");
                        string flowStr = string.Join("','", flowList.Select(t => t[0].ToString()).ToArray<string>());
                        receiptSql.Append(@"         om.Flow in ('" + flowStr + "') ");
                        receiptSql.Append(@"group by om.Flow,i.Code,i.Desc1,od.Uom,s.Code,s.ShiftName ");
                        receiptSql.Append(@"order by om.Flow ASC,i.Code ASC,od.Uom ASC,s.Code ASC ");

                        SqlParameter[] sqlParam = new SqlParameter[2];
                        sqlParam[0] = new SqlParameter("@StartDate", startDate);
                        sqlParam[1] = new SqlParameter("@EndDate", endDate);
                        DataSet recDetDS = sqlHelperMgrE.GetDatasetBySql(receiptSql.ToString(), sqlParam);

                        List<ReceiptDet> recDetList =
                            IListHelper.DataTableToList<ReceiptDet>(recDetDS.Tables[0]);

                        if (recDetList == null || recDetList.Count == 0) return false;

                        #endregion

                        #region 合格率
                        IComment comment = null;
                        IDrawing patr = null;
                        foreach (object[] flow in flowList)
                        {
                            IList<ReceiptDet> flowReceiptDetList = recDetList.Where(o => o.Flow == flow[0].ToString()).ToList();
                            if (flowReceiptDetList == null || flowReceiptDetList.Count() == 0) continue;
                            IList<string> shiftList = flowReceiptDetList.Select(o => o.Shift).Distinct().OrderBy(s => s.Substring(s.IndexOf("["))).ToList();
                            if (shiftList == null || shiftList.Count() == 0) continue;

                            ISheet passSheet = workbook.CreateSheet(flow[0].ToString() + "-" + flow[1].ToString() + "合格率");
                            patr = passSheet.CreateDrawingPatriarch();
                            passSheet.SetDefaultColumnStyle(0, cellStyle);
                            //passSheet.SetDefaultColumnStyle(1, cellStyle);
                            passSheet.SetColumnWidth(0, 25 * 256);
                            //passSheet.SetColumnWidth(1, 25 * 256);

                            int rownum = 0;
                            int colnum = 0;
                            //sheet1.CreateRow(rownum);


                            //No.	生产线	描述	区域	下线时间	物料代码	物料描述	单位	计划数	实际数	次品数	废品数	参考数	完成率	合格率
                            #region 列头
                            //XlsHelper.SetRowCell(passSheet, rownum, colnum++, "No.", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "生产线", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "描述", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "区域", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "下线时间", headStyle);
                            XlsHelper.SetRowCell(passSheet, rownum, colnum++, "物料", headStyle);
                            //XlsHelper.SetRowCell(passSheet, rownum, colnum++, "物料描述", headStyle);
                            XlsHelper.SetRowCell(passSheet, rownum, colnum++, "单位", headStyle);
                            foreach (string shift in shiftList)
                            {
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "计划数", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "合格数", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "次品数", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "废品数", headStyle);
                                //sheet1.SetColumnWidth(colnum, 12 * 256);
                                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, shift + "参考数", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 4, rownum + 2));
                                comment.String = new HSSFRichTextString("完成率=合格数/计划数");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "完成率", comment, headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);

                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 4, rownum + 2));
                                comment.String = new HSSFRichTextString("合格率=合格数/(合格数+次品数+废品数)");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "合格率", comment, headStyle);
                            }
                            int passRateCol = 0;
                            if (shiftList.Count >= 2)
                            {
                                passRateCol = colnum;
                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 3, rownum + 2));
                                comment.String = new HSSFRichTextString("完成率=合格数/计划数");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, "完成率", comment, headStyle);
                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 5, rownum + 2));
                                comment.String = new HSSFRichTextString("合格率=合格数/(合格数+次品数+废品数)");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, "合格率", comment, headStyle);
                            }
                            //rownum++;

                            #endregion

                            #region 明细
                            decimal passRateCount = 0;
                            decimal passRateTotal = 0;
                            //decimal scrapTotal = 0;
                            decimal completeRateCount = 0;
                            decimal completeRateTotal = 0;
                            IList<string> passRateTotalList = new List<string>();
                            //flowReceiptDetList = flowReceiptDetList.OrderBy(r => r.PassRate).OrderBy(r => r.Item).ToList();
                            for (int i = 0; i < flowReceiptDetList.Count(); i++)
                            {
                                ReceiptDet r = flowReceiptDetList[i];
                                if (i == 0 || r.Item != flowReceiptDetList[i - 1].Item || r.Uom != flowReceiptDetList[i - 1].Uom)
                                {
                                    rownum++;
                                    colnum = 0;
                                    passRateCount = 0;
                                    passRateTotal = 0;
                                    completeRateCount = 0;
                                    completeRateTotal = 0;
                                    //scrapTotal = 0;
                                    //XlsHelper.SetRowCell(passSheet, rownum, colnum++, rownum.ToString());
                                    XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.ItemDesc + "[" + r.Item + "]");
                                    //XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.ItemDesc);
                                    XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.Uom);
                                }

								int drift = shiftList.IndexOf(r.Shift);
                                colnum = 6 * drift + 2;
								
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.OrderQty.ToString("0.########"));
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.RecQty.ToString("0.########"));
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.RejQty.ToString("0.########"));
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.ScrapQty.ToString("0.########"));
                                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, o.NumField1.ToString("0.########"));
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.CompleteRate.ToString("P"));
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, r.PassRate.ToString("P"));

                                passRateTotal += r.RecQty;
                                passRateCount += r.RecQty + r.RejQty + r.ScrapQty;
                                //scrapTotal += r.ScrapQty;

                                completeRateTotal += r.RecQty;
                                completeRateCount += r.OrderQty;

                                if ((i == flowReceiptDetList.Count() - 1)
                                    || r.Item != flowReceiptDetList[i + 1].Item || r.Uom != flowReceiptDetList[i + 1].Uom)
                                {
                                    if (shiftList.Count >= 2)
                                    {
                                        if (completeRateCount != 0)
                                        {
                                            XlsHelper.SetRowCell(passSheet, rownum, passRateCol,
                                                                 (completeRateTotal / completeRateCount).ToString("P"));
                                        }
                                        else
                                        {
                                            XlsHelper.SetRowCell(passSheet, rownum, passRateCol, "0.00 %");
                                        }

                                        if (passRateCount != 0)
                                        {
                                            XlsHelper.SetRowCell(passSheet, rownum, passRateCol + 1,
                                                                 (passRateTotal / passRateCount).ToString("P"));
                                        }
                                        else
                                        {
                                            XlsHelper.SetRowCell(passSheet, rownum, passRateCol + 1, "0.00 %");
                                        }
                                    }

                                    if (r.Flow == resistanceLine)
                                    {
                                        string[] num = new string[2];

                                        if (completeRateCount != 0)
                                        {
                                            num[0] = (completeRateTotal / completeRateCount).ToString("P");
                                        }
                                        else
                                        {
                                            num[0] = "0.00 %";
                                        }

                                        if (passRateCount != 0)
                                        {
                                            num[1] = (passRateTotal / passRateCount).ToString("P");
                                        }
                                        else
                                        {
                                            num[1] = "0.00 %";
                                        }

                                        passRateTotalDic.Add(r.Item, num);
                                    }
                                }
                            }

                            rownum += 2;
                            XlsHelper.SetRowCell(passSheet, rownum, 0, "合计", headStyle);
                            XlsHelper.SetMergedRegion(passSheet, rownum, 0, rownum, 1);

                            colnum = 2;
                            decimal receivedQtyT = 0;
                            decimal completeRateT = 0;
                            decimal passRateT = 0;
                            for (int i = 0; i < shiftList.Count; i++)
                            {
                                passSheet.GroupColumn(i * 6 + 2, 1 + i * 6 + 2 + 2);
                                passSheet.SetColumnGroupCollapsed(1 + i * 6 + 2 + 2, true);

                                decimal orderedQty = flowReceiptDetList.Where(d => d.Shift == shiftList[i]).Sum(d => d.OrderQty);
                                decimal receivedQty = flowReceiptDetList.Where(d => d.Shift == shiftList[i]).Sum(d => d.RecQty);
                                decimal rejectedQty = flowReceiptDetList.Where(d => d.Shift == shiftList[i]).Sum(d => d.RejQty);
                                decimal scrapQty = flowReceiptDetList.Where(d => d.Shift == shiftList[i]).Sum(d => d.ScrapQty);
                                decimal completeRate = (orderedQty == 0) ? 0 : (receivedQty / orderedQty);
                                decimal passRate = ((receivedQty + rejectedQty + scrapQty) == 0) ? 0 : (receivedQty / (receivedQty + rejectedQty + scrapQty));

                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, orderedQty.ToString("0.########"), headStyle);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, receivedQty.ToString("0.########"), headStyle);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, rejectedQty.ToString("0.########"), headStyle);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, scrapQty.ToString("0.########"), headStyle);
                                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, o.NumField1.ToString("0.########"));
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, completeRate.ToString("P"), headStyle);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, passRate.ToString("P"), headStyle);
                                completeRateT += orderedQty;
                                passRateT += receivedQty + rejectedQty + scrapQty;
                                receivedQtyT += receivedQty;
                                if (shiftList.Count >= 2 && i == shiftList.Count - 1)
                                {
                                    if (completeRateT != 0)
                                    {
                                        XlsHelper.SetRowCell(passSheet, rownum, colnum++, (receivedQtyT / completeRateT).ToString("P"), headStyle);
                                    }
                                    else
                                    {
                                        XlsHelper.SetRowCell(passSheet, rownum, colnum++, "0.00 %", headStyle);
                                    }
                                    if (passRateT != 0)
                                    {
                                        XlsHelper.SetRowCell(passSheet, rownum, colnum++, (receivedQtyT / passRateT).ToString("P"), headStyle);
                                    }
                                    else
                                    {
                                        XlsHelper.SetRowCell(passSheet, rownum, colnum++, "0.00 %", headStyle);
                                    }
                                }
                            }
                            #endregion

                            passSheet.ForceFormulaRecalculation = true;
                            passSheet.CreateFreezePane(0, 1, 0, 1);
                        }
                        #endregion

                        //废品
                        ProcessScrap(flowList, workbook, recDetList);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessRwo(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                #region 获取合格率数据

                StringBuilder receiptSql = new StringBuilder();

                receiptSql.Append(@"select om.OrderNo,s.ShiftName +'['+s.Code+']' Shift,f.Desc1+'['+f.Code+']' Flow,i.Desc1+'['+i.Code+']' Item,od.Uom, ");
                receiptSql.Append(@"od.OrderQty,r.RecQty,r.RejQty ,r.ScrapQty ");
                //,c1.Desc1 SubType
                receiptSql.Append(@"from OrderDet od join OrderMstr om on od.OrderNo=om.OrderNo and om.SubType = '" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO + "' ");
                receiptSql.Append(@"    join ( ");
                receiptSql.Append(@"            select olt.OrderDetId, sum(rd.RecQty) RecQty,sum(rd.RejQty) RejQty,sum(rd.ScrapQty) ScrapQty ");
                receiptSql.Append(@"            from ReceiptDet rd join ReceiptMstr rm on rd.RecNo=rm.RecNo ");
                receiptSql.Append(@"            join OrderLocTrans olt on rd.OrderLocTransId=olt.id ");
                receiptSql.Append(@"            where rm.OrderType='" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION + "' ");
                receiptSql.Append(@"            and rm.CreateDate >= @StartDate ");
                receiptSql.Append(@"            and rm.CreateDate < @EndDate ");
                receiptSql.Append(@"            group by olt.OrderDetId ");
                receiptSql.Append(@"         ) r on r.OrderDetId = od.Id ");
                receiptSql.Append(@"join ShiftMstr s on om.Shift = s.Code ");
                receiptSql.Append(@"join Item i on od.Item = i.Code ");
                receiptSql.Append(@"join FlowMstr f on om.Flow = f.Code ");
                //receiptSql.Append(@"where ");
                //string flowStr = string.Join("','", flowList.Select(t => t[0].ToString()).ToArray<string>());
                //receiptSql.Append(@"         om.Flow in ('" + flowStr + "') ");
                //receiptSql.Append(@"group by om.Flow,i.Code,i.Desc1,od.Uom,s.Code,s.ShiftName ");
                receiptSql.Append(@"order by om.Flow ASC,s.Code ASC,om.OrderNo ASC,i.Code ASC,od.Uom ASC ");

                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@StartDate", startDate);
                sqlParam[1] = new SqlParameter("@EndDate", endDate);
                DataSet recDetDS = sqlHelperMgrE.GetDatasetBySql(receiptSql.ToString(), sqlParam);

                List<ReceiptDet> recDetList =
                    IListHelper.DataTableToList<ReceiptDet>(recDetDS.Tables[0]);

                if (recDetList == null || recDetList.Count == 0) return false;

                //var flowDiv = flowList.ToDictionary(f => f[0].ToString(), f => f[1].ToString() + "[" + f[0].ToString() + "]");

                #endregion

                ISheet rwoSheet = workbook.CreateSheet("返工统计");
                rwoSheet.SetDefaultColumnStyle(0, cellStyle);
                rwoSheet.SetDefaultColumnStyle(3, cellStyle);
                rwoSheet.SetColumnWidth(0, 22 * 256);
                rwoSheet.SetColumnWidth(1, 9 * 256);
                rwoSheet.SetColumnWidth(2, 15 * 256);
                rwoSheet.SetColumnWidth(3, 43 * 256);
                int rownum = 0;
                int colnum = 0;

                #region 输出列头
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "生产线", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "班次", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "单号", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "物料", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "单位", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "计划数", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "合格数", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "次品数", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "废品数", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "完成率", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "合格率", headStyle);
                rownum++;
                #endregion

                #region 输出明细
                string flow = string.Empty;
                string shift = string.Empty;

                for (int i = 0; i < recDetList.Count; i++)
                {
                    colnum = 0;
                    var recDet = recDetList[i];

                    flow = recDet.Flow;
                    shift = recDet.Shift;

                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.Flow);
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.Shift);
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.OrderNo);
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.Item);
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.Uom);
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.OrderQty.ToString("0.########"));
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.RecQty.ToString("0.########"));
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.RejQty.ToString("0.########"));
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.ScrapQty.ToString("0.########"));
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.CompleteRate.ToString("P"));
                    XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, recDet.PassRate.ToString("P"));
                    rownum++;

                    if (i == recDetList.Count - 1 || shift != recDetList[i + 1].Shift)
                    {
                        XlsHelper.SetRowCell(rwoSheet, rownum, 0, flow, headStyle);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 1, shift, headStyle);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 2, "合计", headStyle);
                        XlsHelper.SetMergedRegion(rwoSheet, rownum, 2, rownum, 4);
                        ReceiptDet recDetFlowShiftTotal = new ReceiptDet();

                        var rwoFlowShiftList = recDetList.Where(s => s.Flow == flow && s.Shift == shift);

                        recDetFlowShiftTotal.OrderQty = rwoFlowShiftList.Sum(s => s.OrderQty);
                        recDetFlowShiftTotal.RecQty = rwoFlowShiftList.Sum(s => s.RecQty);
                        recDetFlowShiftTotal.RejQty = rwoFlowShiftList.Sum(s => s.RejQty);
                        recDetFlowShiftTotal.ScrapQty = rwoFlowShiftList.Sum(s => s.ScrapQty);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 5, recDetFlowShiftTotal.OrderQty.ToString("0.########"), headStyle);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 6, recDetFlowShiftTotal.RecQty.ToString("0.########"), headStyle);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 7, recDetFlowShiftTotal.RejQty.ToString("0.########"), headStyle);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 8, recDetFlowShiftTotal.ScrapQty.ToString("0.########"), headStyle);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 9, recDetFlowShiftTotal.CompleteRate.ToString("P"), headStyle);
                        XlsHelper.SetRowCell(rwoSheet, rownum, 10, recDetFlowShiftTotal.PassRate.ToString("P"), headStyle);
                        //scrapSheet.GroupRow(rownum - scrapFlowShiftList.Count(), rownum - 1);
                        //scrapSheet.SetRowGroupCollapsed(rownum - 1, true);

                        rownum++;
                    }
                }

                rownum += 2;
                XlsHelper.SetRowCell(rwoSheet, rownum, 0, "合计", headStyle);
                XlsHelper.SetMergedRegion(rwoSheet, rownum, 0, rownum, 4);

                ReceiptDet recDetTotal = new ReceiptDet();

                recDetTotal.OrderQty = recDetList.Sum(s => s.OrderQty);
                recDetTotal.RecQty = recDetList.Sum(s => s.RecQty);
                recDetTotal.RejQty = recDetList.Sum(s => s.RejQty);
                recDetTotal.ScrapQty = recDetList.Sum(s => s.ScrapQty);
                XlsHelper.SetRowCell(rwoSheet, rownum, 5, recDetTotal.OrderQty.ToString("0.########"), headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, 6, recDetTotal.RecQty.ToString("0.########"), headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, 7, recDetTotal.RejQty.ToString("0.########"), headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, 8, recDetTotal.ScrapQty.ToString("0.########"), headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, 9, recDetTotal.CompleteRate.ToString("P"), headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, 10, recDetTotal.PassRate.ToString("P"), headStyle);
                #endregion

                rwoSheet.ForceFormulaRecalculation = true;
                rwoSheet.CreateFreezePane(0, 1, 0, 1);

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        private bool ProcessScrap(IList<object[]> flowList, IWorkbook workbook, IList<ReceiptDet> recDetList)
        {
            try
            {
                if (recDetList == null || recDetList.Count == 0) return false;
                var scrapList = recDetList.Where(r => r.ScrapQty != 0).OrderByDescending(r => r.ScrapQty).OrderBy(r => r.Shift).OrderBy(r => r.Flow).ToList();
                if (scrapList == null || scrapList.Count == 0) return false;

                var flowDiv = flowList.ToDictionary(f => f[0].ToString(), f => f[1].ToString() + "[" + f[0].ToString() + "]");

                ISheet scrapSheet = workbook.CreateSheet("生产废品统计");
                for (int i = 0; i <= 3; i++)
                {
                    scrapSheet.SetDefaultColumnStyle(i, cellStyle);
                }
                scrapSheet.SetColumnWidth(0, 20 * 256);
                scrapSheet.SetColumnWidth(1, 10 * 256);
                scrapSheet.SetColumnWidth(2, 16 * 256);
                scrapSheet.SetColumnWidth(3, 45 * 256);
                int rownum = 0;
                int colnum = 0;

                #region 输出列头
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "生产线", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "班次", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "物料代码", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "物料描述", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "单位", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "废品数", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "单价", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "金额", headStyle);
                rownum++;
                #endregion

                #region 输出明细
                string flow = string.Empty;
                string shift = string.Empty;

                for (int i = 0; i < scrapList.Count; i++)
                {
                    colnum = 0;
                    var scrap = scrapList[i];

                    flow = scrap.Flow;
                    shift = scrap.Shift;

                    XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, flowDiv[scrap.Flow]);
                    XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, scrap.Shift);
                    XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, scrap.Item);
                    XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, scrap.ItemDesc);
                    XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, scrap.Uom);
                    XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, scrap.ScrapQty.ToString("0.########"));
                    if (scrap.Price.HasValue)
                    {
                        XlsHelper.SetRowCell(scrapSheet, rownum, colnum, scrap.Price.Value.ToString("0.########"));
                    }
                    colnum++;
                    if (scrap.ScrapAmount.HasValue)
                    {
                        XlsHelper.SetRowCell(scrapSheet, rownum, colnum, scrap.ScrapAmount.Value.ToString("0.########"));
                    }
                    colnum++;
                    rownum++;

                    if (i == scrapList.Count - 1 || shift != scrapList[i + 1].Shift)
                    {
                        XlsHelper.SetRowCell(scrapSheet, rownum, 0, flow, headStyle);
                        XlsHelper.SetRowCell(scrapSheet, rownum, 1, shift, headStyle);
                        XlsHelper.SetRowCell(scrapSheet, rownum, 2, "合计", headStyle);
                        XlsHelper.SetMergedRegion(scrapSheet, rownum, 2, rownum, 4);

                        var scrapFlowShiftList = scrapList.Where(s => s.Flow == flow && s.Shift == shift);
                        var scrapQtyFlowShift = scrapFlowShiftList.Sum(s => s.ScrapQty);
                        XlsHelper.SetRowCell(scrapSheet, rownum, 5, scrapQtyFlowShift.ToString("0.########"), headStyle);

                        var scrapAmountFlowShift = scrapFlowShiftList.Sum(s => s.ScrapAmount);
                        if (scrapAmountFlowShift.HasValue)
                        {
                            XlsHelper.SetRowCell(scrapSheet, rownum, 7, scrapAmountFlowShift.Value.ToString("0.########"), headStyle);
                        }
                        //scrapSheet.GroupRow(rownum - scrapFlowShiftList.Count(), rownum - 1);
                        //scrapSheet.SetRowGroupCollapsed(rownum - 1, true);

                        rownum++;
                    }
                }

                rownum += 2;
                XlsHelper.SetRowCell(scrapSheet, rownum, 0, "合计", headStyle);
                XlsHelper.SetMergedRegion(scrapSheet, rownum, 0, rownum, 4);

                var scrapTotal = scrapList.Sum(s => s.ScrapQty);
                XlsHelper.SetRowCell(scrapSheet, rownum, 5, scrapTotal.ToString("0.########"), headStyle);

                var scrapAmount = scrapList.Sum(s => s.ScrapAmount);
                if (scrapAmount.HasValue)
                {
                    XlsHelper.SetRowCell(scrapSheet, rownum, 7, scrapAmount.Value.ToString("0.########"), headStyle);
                }
                #endregion

                scrapSheet.ForceFormulaRecalculation = true;
                scrapSheet.CreateFreezePane(0, 1, 0, 1);

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

    }

}



#region Extend Class


namespace com.Sconit.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class PercentPassJob : com.Sconit.Service.Batch.Job.PercentPassJob
    {
        public PercentPassJob()
        {
        }
    }
}

#endregion
