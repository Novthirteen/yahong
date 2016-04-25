using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Batch;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity;
using System.Net.Mail;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Utility;
using com.Sconit.Entity.MasterData;
using NPOI.SS.UserModel;
using com.Sconit.ISI.Batch;
using System.Data.SqlClient;
using System.Data;

namespace com.Sconit.ISI.Batch.Job
{
    [Transactional]
    public class ProductionInOutJob : RepJob
    {
        public IResMatrixMgrE resMatrixMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public ICriteriaMgrE criteriaMgr { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            //resMatrixMgr.SendResChangeLog();

            SendInputOutputRatio();
        }


        public void SendInputOutputRatio()
        {

            DateTime dateTimeNow = DateTime.Now.Date;
            //DateTime dateTimeNow = new DateTime(2014, 8, 1);
            if (dateTimeNow.Day == 1)     //每月1号发上个月的
            {

                var startDate = dateTimeNow.Date.AddMonths(-1);
                var endDate = dateTimeNow.Date.AddDays(-1);


                #region 发邮件
                string key = "材料投入产出利用率报表（月）";
                string separator = ISIConstants.EMAIL_SEPRATOR;
                StringBuilder desc = new StringBuilder();
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;材料投入产出利用率");
                desc.Append(separator);
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;开始时间：");
                desc.Append(startDate.ToShortDateString());
                desc.Append(separator);
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;结束时间：");
                desc.Append(endDate.ToShortDateString());
                #endregion

                #region 全部日志

                SendEmail(key, ISIConstants.PERMISSION_PAGE_VALUE_PRODUCTIONINOUT, desc.ToString(), startDate, endDate);

                #endregion

            }
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {

            bool result = false;


            SqlParameter[] sqlParam = new SqlParameter[5];
            sqlParam[0] = new SqlParameter("@Region", string.Empty);
            sqlParam[1] = new SqlParameter("@Flow", string.Empty);
            sqlParam[2] = new SqlParameter("@Item", string.Empty);
            sqlParam[3] = new SqlParameter("@StartDate", startDate);
            sqlParam[4] = new SqlParameter("@EndDate", endDate);

            DataSet productionInOutDataSet = sqlHelperMgrE.GetDatasetByStoredProcedure("Usp_Report_ProductionInOutRep", sqlParam);

            //明细
            result = ProcessProductionInOut(workbook, (DataTable)(productionInOutDataSet.Tables[0]), "明细");
            if (result)
            {
                //缺成本
                result = ProcessProductionInOut1(workbook, (DataTable)(productionInOutDataSet.Tables[1]));
            }
            if (result)
            {
                //利用率高于110%
                result = ProcessProductionInOut(workbook, (DataTable)productionInOutDataSet.Tables[2], "利用率高于110%");
            }
            if (result)
            {
                //利用率低于90 %
                result = ProcessProductionInOut(workbook, (DataTable)(productionInOutDataSet.Tables[3]), "利用率低于90%");
            }
            if (result)
            {
                //未归集
                result = ProcessProductionInOut2(workbook, (DataTable)(productionInOutDataSet.Tables[4]));
            }
            return result;

        }


        public override void SendEmail(string key, string permissionCode, string desc, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string mailList = uesrMgrE.FindEmailByPermission(new string[] { permissionCode });

                IList<EntityPreference> entityPreferenceList = entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[]{BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME,
                                                    ISIConstants.ENTITY_PREFERENCE_WEBADDRESS});

                log.Info(key + ",发送列表：" + mailList);
                if (string.IsNullOrEmpty(mailList)) return;

                DateTime now = DateTime.Now;
                string companyName = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).SingleOrDefault().Value;
                string webAddress = entityPreferenceList.Where(e => e.Code == ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).SingleOrDefault().Value;

                string file = GenerateReport(key, permissionCode, desc, entityPreferenceList, startDate, endDate);

                if (!string.IsNullOrEmpty(file))
                {
                    IList<string> files = new List<string>();
                    files.Add(file);

                    StringBuilder content = new StringBuilder();
                    content.Append("<p style='font-size:15px;'>");
                    string separator = ISIConstants.EMAIL_SEPRATOR;

                    ISIUtil.AppendTestText(this.smtpMgrE.IsTestSystem(), content, separator);

                    content.Append(separator);
                    content.Append("您好");
                    content.Append(separator);
                    content.Append(desc);
                    content.Append(separator);
                    //content.Append(separator);
                    //content.Append("&nbsp;&nbsp;请见附件");

                    content.Append(separator);
                    content.Append(companyName);
                    content.Append(separator);
                    content.Append("<a href='http://" + webAddress + "'>http://" + webAddress + "</a>");
                    content.Append(separator);
                    content.Append("</p>");

                    smtpMgrE.AsyncSend2(companyName + "-" + key, content.ToString(), mailList, string.Empty, MailPriority.Normal, files);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        private bool ProcessProductionInOut(IWorkbook workbook, DataTable dataTable, string sheetName)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet(sheetName);

                for (int i = 0; i <= 15; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 10 * 256);//成品
                sheet.SetColumnWidth(1, 40 * 256);//成品描述
                sheet.SetColumnWidth(2, 10 * 256);//单位
                sheet.SetColumnWidth(3, 10 * 256);//产出数
                sheet.SetColumnWidth(4, 10 * 256);//盘差
                sheet.SetColumnWidth(5, 10 * 256);//标准金额
                sheet.SetColumnWidth(6, 10 * 256);//实际金额
                sheet.SetColumnWidth(7, 10 * 256);//利用率
                sheet.SetColumnWidth(8, 10 * 256);//原材料
                sheet.SetColumnWidth(9, 40 * 256);//原材料描述
                sheet.SetColumnWidth(10, 10 * 256);//单位1
                sheet.SetColumnWidth(11, 10 * 256);//理论消耗
                sheet.SetColumnWidth(12, 10 * 256);//实际消耗
                sheet.SetColumnWidth(13, 10 * 256);//标准金额1
                sheet.SetColumnWidth(14, 10 * 256);//实际金额1
                sheet.SetColumnWidth(15, 20 * 256);//成本

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "成品", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "成品描述", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "单位", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "产出数", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "盘差", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "标准金额", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "实际金额", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "利用率", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "原材料", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "原材料描述", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "单位1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "理论消耗", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "实际消耗", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "标准金额1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "实际金额1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "成本", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, sheetName, headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, dataTable.Rows.Count + " 条", headStyle2);

                #region 输出数据
                rownum = 2;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    column = 0;
                    DataRow inputOutput = dataTable.Rows[i];

                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[0]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[1]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[2]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[3]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[4]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[5]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[6]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[7]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[8]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[9]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[10]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[11]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[12]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[13]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[14]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[15]);
                    rownum++;
                }

                sheet.ForceFormulaRecalculation = true;
                sheet.CreateFreezePane(1, 0, 1, 0);

                #endregion

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        //缺成本
        private bool ProcessProductionInOut1(IWorkbook workbook, DataTable dataTable)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet("缺成本");

                for (int i = 0; i <= 1; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 20 * 256);//原材料
                sheet.SetColumnWidth(1, 80 * 256);//原材料描述

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "原材料", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "原材料描述", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, "缺成本：", headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, dataTable.Rows.Count + " 条", headStyle2);

                #region 输出数据
                rownum = 2;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    column = 0;
                    DataRow inputOutput = dataTable.Rows[i];

                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[0]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[1]);
                    rownum++;
                }

                sheet.ForceFormulaRecalculation = true;
                sheet.CreateFreezePane(1, 0, 1, 0);

                #endregion

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        //未归集 
        private bool ProcessProductionInOut2(IWorkbook workbook, DataTable dataTable)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet("未归集");

                for (int i = 0; i <= 1; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 20 * 256);//原材料
                sheet.SetColumnWidth(1, 80 * 256);//原材料描述
                sheet.SetColumnWidth(0, 10 * 256);//单位1
                sheet.SetColumnWidth(1, 20 * 256);//实际消耗
                sheet.SetColumnWidth(0, 20 * 256);//实际金额1
                sheet.SetColumnWidth(1, 20 * 256);//成本

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "原材料", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "原材料描述", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "单位1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "实际消耗", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "实际金额1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "成本", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, "未归集：", headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, dataTable.Rows.Count + " 条", headStyle2);

                #region 输出数据
                rownum = 2;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    column = 0;
                    DataRow inputOutput = dataTable.Rows[i];

                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[0]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[1]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, (string)inputOutput[2]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[3]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[4]);
                    XlsHelper.SetRowCell(sheet, rownum, column++, inputOutput[5]);
                    rownum++;
                }

                sheet.ForceFormulaRecalculation = true;
                sheet.CreateFreezePane(1, 0, 1, 0);

                #endregion

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

namespace com.Sconit.ISI.Batch.Ext.Job
{
    [Transactional]
    public partial class ProductionInOutJob : com.Sconit.ISI.Batch.Job.ProductionInOutJob
    {
        public ProductionInOutJob()
        {
        }
    }
}

#endregion
