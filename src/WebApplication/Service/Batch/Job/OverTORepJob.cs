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
            string key = "移库异常报表";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;1. 逾期移库单：超期未关闭移库单");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;2. 退货(日)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;3. 退货(月)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;4. 逾期移库送货单：超期未确认移库送货单");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;5. 收货差异(日)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;6. 收货差异(月)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;7. 库龄");

            int toExpiredDays = int.Parse(entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[] { "TOExpiredDays" })[0].Value);

            DateTime endDate = DateTime.Now.AddDays(-1 * toExpiredDays);
            DateTime startDate = endDate.AddDays(-1);
            //todo 2012-10-31有数据
            //endDate = new DateTime(2012, 10, 31);
            //startDate = endDate.AddDays(-30);
            SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_OVERTOREP, desc.ToString(), startDate, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            bool result1 = false;
            //订单、退货单
            result1 = ProcessTO(workbook, startDate, endDate);

            bool result2 = false;
            //送货单
            result2 = ProcessTOASN(workbook, startDate, endDate);

            bool result3 = false;
            //差异
            result3 = ProcessTOGap(workbook, startDate, endDate);

            //供货及时率
            bool result4 = false;

            //库存周转率
            bool result6 = false;

            //月最后一天
            if (endDate.HasValue &&
                endDate.Value.ToString("yyyy-MM-dd") ==
                endDate.Value.AddDays(1 - endDate.Value.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"))
            {
                //库存周转率
                result6 = false;

                //供货及时率
                result4 = false;
            }

            bool result5 = false;
            //周日
            if (endDate.HasValue && endDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                //库龄
                result5 = ProcessLocAging(workbook);
            }
            
            return result1 || result2 || result3 || result4 || result5 || result6;
        }


        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessLocAging(IWorkbook workbook)
        {
            try
            {
                #region 获取库存数据
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
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet("库龄");

                for (int i = 0; i <= 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }
                sheet.SetColumnWidth(0, 28 * 256);//物料
                sheet.SetColumnWidth(1, 10 * 256);//单位//入库时间
                sheet.SetColumnWidth(2, 20 * 256);//单包装//区域
                sheet.SetColumnWidth(3, 25 * 256);//小于7//库位
                sheet.SetColumnWidth(4, 25 * 256);//7至30//数量
                sheet.SetColumnWidth(5, 25 * 256);//30至60//非寄售
                sheet.SetColumnWidth(6, 25 * 256);//60至90//寄售
                sheet.SetColumnWidth(7, 25 * 256);//大于90//

                int rownum = 0;
                int column = 0;

                #endregion

                #region 输出数据

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
                        //输出头
                        XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Item);
                        XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Uom);
                        XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.UC.ToString("0.########"));
                        rownum++;
                        //输出列头
                        OutputLocAgingColunmHead(sheet, rownum++);
                    }
                    //明细
                    column = 1;

                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.CreateDate.ToString("yyyy-MM-dd"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Region);
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Location);

                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.Qty.ToString("0.########"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.NmlQty.ToString("0.########"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, locLotDet.CsQty.ToString("0.########"));

                    #region 输出头的汇总部分
                    itemLocLotDetList.Add(locLotDet);
                    if (i == locLotDetList.Count() - 1 || locLotDet.ItemCode != locLotDetList[i + 1].ItemCode)
                    {
                        column = 3;
                        //小于7
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
            //入库时间	区域	库位  数量	非寄售	寄售	
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "入库时间", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "区域", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "库位", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "数量", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "非寄售", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "寄售", headStyle);
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
