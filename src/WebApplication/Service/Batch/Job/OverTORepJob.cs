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
    public class OverTORepJob : RepJob
    {
        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            string key = "�ƿ��쳣����";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;1. �����ƿⵥ������δ�ر��ƿⵥ");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;2. �˻�(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;3. �˻�(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;4. �����ƿ��ͻ���������δȷ���ƿ��ͻ���");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;5. �ջ�����(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;6. �ջ�����(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;7. ����");

            int toExpiredDays = int.Parse(entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[] { "TOExpiredDays" })[0].Value);

            DateTime endDate = DateTime.Now.AddDays(-1 * toExpiredDays);
            DateTime startDate = endDate.AddDays(-1);
            //todo 2012-10-31������
            //endDate = new DateTime(2012, 10, 31);
            //startDate = endDate.AddDays(-30);
            SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_OVERTOREP, desc.ToString(), startDate, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            bool result1 = false;
            //�������˻���
            result1 = ProcessTO(workbook, startDate, endDate);

            bool result2 = false;
            //�ͻ���
            result2 = ProcessTOASN(workbook, startDate, endDate);

            bool result3 = false;
            //����
            result3 = ProcessTOGap(workbook, startDate, endDate);

            //������ʱ��
            bool result4 = false;

            //�����ת��
            bool result6 = false;

            //�����һ��
            if (endDate.HasValue &&
                endDate.Value.ToString("yyyy-MM-dd") ==
                endDate.Value.AddDays(1 - endDate.Value.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"))
            {
                //�����ת��
                result6 = false;

                //������ʱ��
                result4 = false;
            }

            bool result5 = false;
            //����
            if (endDate.HasValue && endDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                //����
                result5 = ProcessLocAging(workbook);
            }
            
            return result1 || result2 || result3 || result4 || result5 || result6;
        }


        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessLocAging(IWorkbook workbook)
        {
            try
            {
                #region ��ȡ�������
                StringBuilder locLotDetSql = new StringBuilder();
                locLotDetSql.Append(@" SELECT i.Code ItemCode,i.Desc1 ItemDesc,i.UC,i.Uom, ");
                locLotDetSql.Append(@"        l.Name+'['+l.Code+']' Location,p.Name+'['+p.Code+']' Region, ");
                locLotDetSql.Append(@"        CONVERT(date,lld.CreateDate) CreateDate,");
                locLotDetSql.Append(@"        SUM(CASE WHEN IsCS = 1 THEN Qty ELSE 0 END) AS CsQty, ");
                locLotDetSql.Append(@"        SUM(CASE WHEN IsCS = 0 THEN Qty ELSE 0 END) AS NmlQty, ");
                locLotDetSql.Append(@"        SUM(lld.Qty) AS Qty ");
                locLotDetSql.Append(@" FROM LocationLotDet lld ");
                locLotDetSql.Append(@" join Item i on lld.Item=i.Code ");
                locLotDetSql.Append(@" join Location l on lld.Location=l.Code ");
                locLotDetSql.Append(@" join Party p on l.Region=p.Code ");
                locLotDetSql.Append(@" where lld.Qty!=0 ");
                locLotDetSql.Append(@" group by  i.Code,i.Desc1,i.UC,i.Uom,p.Code ,p.Name,l.Code,l.Name,CreateDate ");
                locLotDetSql.Append(@" order by i.Code ASC,CreateDate ASC,p.Code ASC,l.Code ASC,Qty ASC ");
                DataSet locLotDetDS = sqlHelperMgrE.GetDatasetBySql(locLotDetSql.ToString());
                var locLotDetList = IListHelper.DataTableToList<LocLotDet>(locLotDetDS.Tables[0]);

                #endregion

                if (locLotDetList != null && locLotDetList.Count > 0)
                {
                    return ProcessLocAging(workbook, locLotDetList);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }


        private bool ProcessLocAging(IWorkbook workbook, IList<LocLotDet> locLotDetList)
        {
            try
            {
                #region ��ʼ��Sheet

                ISheet sheet = workbook.CreateSheet("����");

                for (int i = 0; i <= 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }
                sheet.SetColumnWidth(0, 28 * 256);//����
                sheet.SetColumnWidth(1, 10 * 256);//��λ//���ʱ��
                sheet.SetColumnWidth(2, 20 * 256);//����װ//����
                sheet.SetColumnWidth(3, 25 * 256);//С��7//��λ
                sheet.SetColumnWidth(4, 25 * 256);//7��30//����
                sheet.SetColumnWidth(5, 25 * 256);//30��60//�Ǽ���
                sheet.SetColumnWidth(6, 25 * 256);//60��90//����
                sheet.SetColumnWidth(7, 25 * 256);//����90//

                int rownum = 0;
                int column = 0;

                #endregion

                #region �������

                OutputItem90(sheet, rownum);
                var itemLocLotDetList = new List<LocLotDet>();
                rownum++;
                DateTime now = DateTime.Now;
                for (int i = 0; i < locLotDetList.Count(); i++)
                {
                    var locLotDet = locLotDetList[i];

                    if (i == 0 || locLotDet.ItemCode != locLotDetList[i - 1].ItemCode)
                    {
                        column = 0;
                        //���ͷ
                        XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Item);
                        XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Uom);
                        XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.UC.ToString("0.########"));
                        rownum++;
                        //�����ͷ
                        OutputLocAgingColunmHead(sheet, rownum++);
                    }
                    //��ϸ
                    column = 1;

                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.CreateDate.ToString("yyyy-MM-dd"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Region);
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Location);

                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Qty.ToString("0.########"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.NmlQty.ToString("0.########"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.CsQty.ToString("0.########"));

                    #region ���ͷ�Ļ��ܲ���
                    itemLocLotDetList.Add(locLotDet);
                    if (i == locLotDetList.Count() - 1 || locLotDet.ItemCode != locLotDetList[i + 1].ItemCode)
                    {
                        column = 3;
                        //С��7
                        var nmlQty = itemLocLotDetList.Where(l => l.CreateDate > now.AddDays(-7)).Sum(l => l.NmlQty);
                        var csQty = itemLocLotDetList.Where(l => l.CreateDate > now.AddDays(-7)).Sum(l => l.CsQty);
                        var qty = nmlQty + csQty;
                        if (qty != 0)
                        {
                            XlsHelper.SetRowCell(sheet, rownum - itemLocLotDetList.Count() - 1, column, nmlQty.ToString("0.########") + (csQty != 0 ? "(" + csQty.ToString("0.########") + ")" : string.Empty));
                        }
                        column++;
                        //7-30
                        nmlQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-7) && l.CreateDate > now.AddDays(-30)).Sum(l => l.NmlQty);
                        csQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-7) && l.CreateDate > now.AddDays(-30)).Sum(l => l.CsQty);
                        qty = nmlQty + csQty;
                        if (qty != 0)
                        {
                            XlsHelper.SetRowCell(sheet, rownum - itemLocLotDetList.Count() - 1, column, nmlQty.ToString("0.########") + (csQty != 0 ? "(" + csQty.ToString("0.########") + ")" : string.Empty));
                        }
                        column++;
                        //30-60
                        nmlQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-30) && l.CreateDate > now.AddDays(-60)).Sum(l => l.NmlQty);
                        csQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-30) && l.CreateDate > now.AddDays(-60)).Sum(l => l.CsQty);
                        qty = nmlQty + csQty;
                        if (qty != 0)
                        {
                            XlsHelper.SetRowCell(sheet, rownum - itemLocLotDetList.Count() - 1, column, nmlQty.ToString("0.########") + (csQty != 0 ? "(" + csQty.ToString("0.########") + ")" : string.Empty));
                        }
                        column++;
                        //60-90
                        nmlQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-60) && l.CreateDate > now.AddDays(-90)).Sum(l => l.NmlQty);
                        csQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-60) && l.CreateDate > now.AddDays(-90)).Sum(l => l.CsQty);
                        qty = nmlQty + csQty;
                        if (qty != 0)
                        {
                            XlsHelper.SetRowCell(sheet, rownum - itemLocLotDetList.Count() - 1, column, nmlQty.ToString("0.########") + (csQty != 0 ? "(" + csQty.ToString("0.########") + ")" : string.Empty));
                        }
                        column++;
                        //>=90
                        nmlQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-90)).Sum(l => l.NmlQty);
                        csQty = itemLocLotDetList.Where(l => l.CreateDate <= now.AddDays(-90)).Sum(l => l.CsQty);
                        qty = nmlQty + csQty;
                        if (qty != 0)
                        {
                            XlsHelper.SetRowCell(sheet, rownum - itemLocLotDetList.Count() - 1, column, nmlQty.ToString("0.########") + (csQty != 0 ? "(" + csQty.ToString("0.########") + ")" : string.Empty));
                        }
                        rownum++;
                        column++;
                        sheet.GroupRow(rownum - itemLocLotDetList.Count() - 1, rownum);
                        sheet.SetRowGroupCollapsed(rownum, true);
                        itemLocLotDetList = new List<LocLotDet>();
                    }

                    #endregion

                    rownum++;
                }

                sheet.ForceFormulaRecalculation = true;
                sheet.CreateFreezePane(1, 1, 1, 1);

                #endregion

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }
        private void OutputLocAgingColunmHead(ISheet sheet, int rownum)
        {
            int colnum = 1;
            //���ʱ��	����	��λ  ����	�Ǽ���	����	
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "���ʱ��", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "����", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "��λ", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "����", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "�Ǽ���", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "����", headStyle);
        }
    }
}



#region Extend Class


namespace com.Sconit.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class OverTORepJob : com.Sconit.Service.Batch.Job.OverTORepJob
    {
        public OverTORepJob()
        {
        }
    }
}

#endregion
