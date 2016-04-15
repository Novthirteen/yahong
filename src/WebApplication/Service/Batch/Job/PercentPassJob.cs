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
            string key = "�պϸ���";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;Excel��������: ");
            desc.Append(separator);
            desc.Append("1. ����ϸ��ʣ����տ�λ����չʾ����ĺϸ���(ע: �鿴�������������ڱ����Ϸ������չ��)");
            desc.Append(separator);
            desc.Append("2. �ϸ��ʣ�ÿ��������һ��Sheet(ע: �鿴�������������ڱ����Ϸ������չ��)");
            desc.Append(separator);            
            desc.Append("3. ������Ʒͳ��");
            desc.Append(separator);
            desc.Append("4. ����: ԭ��Ϊ���ϵļƻ�����ⵥͳ��");
            desc.Append(separator);
            desc.Append("5. ����ͳ��: ��������������Ϣ");
            desc.Append(separator);
            desc.Append("6. �ͻ��˻�(��)");
            desc.Append(separator);
            desc.Append("7. �ͻ��˻�(��)");
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
                //todo 2012-10-31������
                //endDate = new DateTime(2012, 10, 31);
                //startDate = endDate.AddDays(-30);
                //now = endDate;
            }
            else
            {
                return;
            }

            #region �ձ����ܱ����±�������
            //�ձ�
            {
                StringBuilder commentDay = new StringBuilder();
                commentDay.Append(desc.ToString());
                commentDay.Append("����ʱ��Ϊ��" + startDate.ToString("yyyy-MM-dd HH:mm") + " �� " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentDay.ToString(), startDate, endDate);
            }
            //�ܱ�
            if (now.DayOfWeek == DayOfWeek.Monday)
            {
                key = "�ܺϸ���";
                startDate = endDate.AddDays(-7);
                StringBuilder commentWeek = new StringBuilder();
                commentWeek.Append(desc.ToString());
                commentWeek.Append("����ʱ��Ϊ��" + startDate.ToString("yyyy-MM-dd HH:mm") + " �� " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentWeek.ToString(), startDate, endDate);
            }

            //�±�
            if (now.Day == 26)
            {
                key = "�ºϸ���";
                startDate = endDate.AddMonths(-1);
                StringBuilder commentMonth = new StringBuilder();
                commentMonth.Append(desc.ToString());
                commentMonth.Append("����ʱ��Ϊ��" + startDate.ToString("yyyy-MM-dd HH:mm") + " �� " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentMonth.ToString(), startDate, endDate);
            }
            //����
            if ((now.Day == 26) && (now.Month == 12 || now.Month == 3 || now.Month == 6 || now.Month == 9))
            {
                key = "���Ⱥϸ���";
                startDate = endDate.AddMonths(-3);
                StringBuilder commentMonth = new StringBuilder();
                commentMonth.Append(desc.ToString());
                commentMonth.Append("����ʱ��Ϊ��" + startDate.ToString("yyyy-MM-dd HH:mm") + " �� " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentMonth.ToString(), startDate, endDate);
            }

            //����
            /*
            if (now.Day == 1 &&  now.Month == 7)
            {
                key = "���кϸ���";
                startDate = endDate.AddMonths(-6);
                StringBuilder comment6Month = new StringBuilder();
                comment6Month.Append(desc.ToString());
                comment6Month.Append("����ʱ��Ϊ��" + startDate.ToString("yyyy-MM-dd HH:mm") + " �� " + endDate.ToString("yyyy-MM-dd HH:mm"));
                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, comment6Month.ToString(), startDate, endDate);
            }
            
            //�걨

            if (now.DayOfYear == 1)
            {
                key = "��ϸ���";
                startDate = endDate.AddYears(-1);
                StringBuilder commentYear = new StringBuilder();
                commentYear.Append(desc.ToString());
                commentYear.Append("����ʱ��Ϊ��" + startDate.ToString("yyyy-MM-dd HH:mm") + " �� " + endDate.ToString("yyyy-MM-dd HH:mm"));

                SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_PERCENTPASSREP, commentYear.ToString(), startDate, endDate);
            }
            */
            #endregion
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {

            //����
            bool result2 = ProcessInspect(workbook, startDate, endDate);

            //�ϸ���
            bool result1 = ProcessPassWO(workbook, startDate, endDate);

            //������������ȡ��ȡ��������
            //bool result3 = ProcessWO(workbook, startDate, endDate);

            //����
            bool result5 = ProcessUnpIndustrialWaste(workbook, startDate, endDate);

            //����
            bool result6 = ProcessRwo(workbook, startDate, endDate);

            //�ͻ��˻�
            bool result4 = false;
            if (key == "�պϸ���")
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
                #region ��ȡ����ȷ�Ͻ������
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

                    #region ��ȡ���鵥ͷ����
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


                    #region ��ʼ��Sheet

                    ISheet inspSheet = workbook.CreateSheet("����ϸ���");

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
                
                XlsHelper.SetRowCell(sheet1, rownum, colnum++, "��������");
                XlsHelper.SetRowCell(sheet1, rownum, colnum++, inspectResultList.Count() + " ��", headStyle);

                XlsHelper.SetRowCell(sheet1, rownum, colnum++, "���κϸ��ʣ�");
                XlsHelper.SetRowCell(sheet1, rownum, colnum++, inspectResultList.Sum(r => r.CurrQualifyQty / (r.CurrQualifyQty + r.CurrRejectQty)).Count() + " ��", headStyle);
                rownum++;
                 */

                    #region �����ͷ
                    colnum = 0;
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "��Դ��λ", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "����", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "��λ", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "����װ", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "�����λ", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "���ϸ�Ʒ��λ", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "�����ϼ�", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "ȷ�ϴ����ϼ�", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "������", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "�ϸ���", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "���ϸ���", headStyle);

                    IDrawing patr = inspSheet.CreateDrawingPatriarch();
                    IComment comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 3, rownum + 2));
                    comment.String = new HSSFRichTextString("�����=�ϸ���/������");
                    comment.Author = "LPP Team";

                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "�����", comment, headStyle);

                    comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 3, rownum + 2));
                    comment.String = new HSSFRichTextString("�ϸ���=�ϸ���/(�ϸ���+���ϸ���)");
                    comment.Author = "LPP Team";

                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "�ϸ���", comment, headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "�ϸ��ȷ����", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "���ϸ��ȷ����", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "���κϸ���", headStyle);
                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "���β��ϸ���", headStyle);

                    comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 6, rownum + 2));
                    comment.String = new HSSFRichTextString("���κϸ���=���κϸ���/(���κϸ���+���β��ϸ���)");
                    comment.Author = "LPP Team";

                    XlsHelper.SetRowCell(inspSheet, rownum, colnum++, "���κϸ���", comment, headStyle);
                    #endregion

                    #region �������
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

                    #region ����ϼ�
                    rownum += 2;
                    XlsHelper.SetRowCell(inspSheet, rownum, 0, "�ϼ�", headStyle);
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
                        #region ��ȡ�ϸ�������

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

                        #region �ϸ���
                        IComment comment = null;
                        IDrawing patr = null;
                        foreach (object[] flow in flowList)
                        {
                            IList<ReceiptDet> flowReceiptDetList = recDetList.Where(o => o.Flow == flow[0].ToString()).ToList();
                            if (flowReceiptDetList == null || flowReceiptDetList.Count() == 0) continue;
                            IList<string> shiftList = flowReceiptDetList.Select(o => o.Shift).Distinct().OrderBy(s => s.Substring(s.IndexOf("["))).ToList();
                            if (shiftList == null || shiftList.Count() == 0) continue;

                            ISheet passSheet = workbook.CreateSheet(flow[0].ToString() + "-" + flow[1].ToString() + "�ϸ���");
                            patr = passSheet.CreateDrawingPatriarch();
                            passSheet.SetDefaultColumnStyle(0, cellStyle);
                            //passSheet.SetDefaultColumnStyle(1, cellStyle);
                            passSheet.SetColumnWidth(0, 25 * 256);
                            //passSheet.SetColumnWidth(1, 25 * 256);

                            int rownum = 0;
                            int colnum = 0;
                            //sheet1.CreateRow(rownum);


                            //No.	������	����	����	����ʱ��	���ϴ���	��������	��λ	�ƻ���	ʵ����	��Ʒ��	��Ʒ��	�ο���	�����	�ϸ���
                            #region ��ͷ
                            //XlsHelper.SetRowCell(passSheet, rownum, colnum++, "No.", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "������", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����", headStyle);
                            //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����ʱ��", headStyle);
                            XlsHelper.SetRowCell(passSheet, rownum, colnum++, "����", headStyle);
                            //XlsHelper.SetRowCell(passSheet, rownum, colnum++, "��������", headStyle);
                            XlsHelper.SetRowCell(passSheet, rownum, colnum++, "��λ", headStyle);
                            foreach (string shift in shiftList)
                            {
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "�ƻ���", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "�ϸ���", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "��Ʒ��", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "��Ʒ��", headStyle);
                                //sheet1.SetColumnWidth(colnum, 12 * 256);
                                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, shift + "�ο���", headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);
                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 4, rownum + 2));
                                comment.String = new HSSFRichTextString("�����=�ϸ���/�ƻ���");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "�����", comment, headStyle);
                                passSheet.SetColumnWidth(colnum, 15 * 256);

                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 4, rownum + 2));
                                comment.String = new HSSFRichTextString("�ϸ���=�ϸ���/(�ϸ���+��Ʒ��+��Ʒ��)");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, shift + "�ϸ���", comment, headStyle);
                            }
                            int passRateCol = 0;
                            if (shiftList.Count >= 2)
                            {
                                passRateCol = colnum;
                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 3, rownum + 2));
                                comment.String = new HSSFRichTextString("�����=�ϸ���/�ƻ���");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, "�����", comment, headStyle);
                                comment = patr.CreateCellComment(new HSSFClientAnchor(0, 0, 0, 0, colnum, rownum + 1, colnum + 5, rownum + 2));
                                comment.String = new HSSFRichTextString("�ϸ���=�ϸ���/(�ϸ���+��Ʒ��+��Ʒ��)");
                                comment.Author = "LPP Team";
                                XlsHelper.SetRowCell(passSheet, rownum, colnum++, "�ϸ���", comment, headStyle);
                            }
                            //rownum++;

                            #endregion

                            #region ��ϸ
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
                            XlsHelper.SetRowCell(passSheet, rownum, 0, "�ϼ�", headStyle);
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

                        //��Ʒ
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
                #region ��ȡ�ϸ�������

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

                ISheet rwoSheet = workbook.CreateSheet("����ͳ��");
                rwoSheet.SetDefaultColumnStyle(0, cellStyle);
                rwoSheet.SetDefaultColumnStyle(3, cellStyle);
                rwoSheet.SetColumnWidth(0, 22 * 256);
                rwoSheet.SetColumnWidth(1, 9 * 256);
                rwoSheet.SetColumnWidth(2, 15 * 256);
                rwoSheet.SetColumnWidth(3, 43 * 256);
                int rownum = 0;
                int colnum = 0;

                #region �����ͷ
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "������", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "���", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "����", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "����", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "��λ", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "�ƻ���", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "�ϸ���", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "��Ʒ��", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "��Ʒ��", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "�����", headStyle);
                XlsHelper.SetRowCell(rwoSheet, rownum, colnum++, "�ϸ���", headStyle);
                rownum++;
                #endregion

                #region �����ϸ
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
                        XlsHelper.SetRowCell(rwoSheet, rownum, 2, "�ϼ�", headStyle);
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
                XlsHelper.SetRowCell(rwoSheet, rownum, 0, "�ϼ�", headStyle);
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

                ISheet scrapSheet = workbook.CreateSheet("������Ʒͳ��");
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

                #region �����ͷ
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "������", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "���", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "���ϴ���", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "��������", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "��λ", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "��Ʒ��", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "����", headStyle);
                XlsHelper.SetRowCell(scrapSheet, rownum, colnum++, "���", headStyle);
                rownum++;
                #endregion

                #region �����ϸ
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
                        XlsHelper.SetRowCell(scrapSheet, rownum, 2, "�ϼ�", headStyle);
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
                XlsHelper.SetRowCell(scrapSheet, rownum, 0, "�ϼ�", headStyle);
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
