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
            if (dateTimeNow.Day == 1)     //ÿ��1�ŷ��ϸ��µ�
            {

                var startDate = dateTimeNow.Date.AddMonths(-1);
                var endDate = dateTimeNow.Date.AddDays(-1);


                #region ���ʼ�
                string key = "����Ͷ����������ʱ����£�";
                string separator = ISIConstants.EMAIL_SEPRATOR;
                StringBuilder desc = new StringBuilder();
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;����Ͷ�����������");
                desc.Append(separator);
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;��ʼʱ�䣺");
                desc.Append(startDate.ToShortDateString());
                desc.Append(separator);
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;����ʱ�䣺");
                desc.Append(endDate.ToShortDateString());
                #endregion

                #region ȫ����־

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

            //��ϸ
            result = ProcessProductionInOut(workbook, (DataTable)(productionInOutDataSet.Tables[0]), "��ϸ");
            if (result)
            {
                //ȱ�ɱ�
                result = ProcessProductionInOut1(workbook, (DataTable)(productionInOutDataSet.Tables[1]));
            }
            if (result)
            {
                //�����ʸ���110%
                result = ProcessProductionInOut(workbook, (DataTable)productionInOutDataSet.Tables[2], "�����ʸ���110%");
            }
            if (result)
            {
                //�����ʵ���90 %
                result = ProcessProductionInOut(workbook, (DataTable)(productionInOutDataSet.Tables[3]), "�����ʵ���90%");
            }
            if (result)
            {
                //δ�鼯
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

                log.Info(key + ",�����б�" + mailList);
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
                    content.Append("����");
                    content.Append(separator);
                    content.Append(desc);
                    content.Append(separator);
                    //content.Append(separator);
                    //content.Append("&nbsp;&nbsp;�������");

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
                #region ��ʼ��Sheet

                ISheet sheet = workbook.CreateSheet(sheetName);

                for (int i = 0; i <= 15; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 10 * 256);//��Ʒ
                sheet.SetColumnWidth(1, 40 * 256);//��Ʒ����
                sheet.SetColumnWidth(2, 10 * 256);//��λ
                sheet.SetColumnWidth(3, 10 * 256);//������
                sheet.SetColumnWidth(4, 10 * 256);//�̲�
                sheet.SetColumnWidth(5, 10 * 256);//��׼���
                sheet.SetColumnWidth(6, 10 * 256);//ʵ�ʽ��
                sheet.SetColumnWidth(7, 10 * 256);//������
                sheet.SetColumnWidth(8, 10 * 256);//ԭ����
                sheet.SetColumnWidth(9, 40 * 256);//ԭ��������
                sheet.SetColumnWidth(10, 10 * 256);//��λ1
                sheet.SetColumnWidth(11, 10 * 256);//��������
                sheet.SetColumnWidth(12, 10 * 256);//ʵ������
                sheet.SetColumnWidth(13, 10 * 256);//��׼���1
                sheet.SetColumnWidth(14, 10 * 256);//ʵ�ʽ��1
                sheet.SetColumnWidth(15, 20 * 256);//�ɱ�

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "��Ʒ", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��Ʒ����", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��λ", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "������", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "�̲�", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��׼���", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ʵ�ʽ��", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "������", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ԭ����", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ԭ��������", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��λ1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��������", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ʵ������", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��׼���1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ʵ�ʽ��1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "�ɱ�", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, sheetName, headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, dataTable.Rows.Count + " ��", headStyle2);

                #region �������
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

        //ȱ�ɱ�
        private bool ProcessProductionInOut1(IWorkbook workbook, DataTable dataTable)
        {
            try
            {
                #region ��ʼ��Sheet

                ISheet sheet = workbook.CreateSheet("ȱ�ɱ�");

                for (int i = 0; i <= 1; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 20 * 256);//ԭ����
                sheet.SetColumnWidth(1, 80 * 256);//ԭ��������

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "ԭ����", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ԭ��������", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, "ȱ�ɱ���", headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, dataTable.Rows.Count + " ��", headStyle2);

                #region �������
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

        //δ�鼯 
        private bool ProcessProductionInOut2(IWorkbook workbook, DataTable dataTable)
        {
            try
            {
                #region ��ʼ��Sheet

                ISheet sheet = workbook.CreateSheet("δ�鼯");

                for (int i = 0; i <= 1; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 20 * 256);//ԭ����
                sheet.SetColumnWidth(1, 80 * 256);//ԭ��������
                sheet.SetColumnWidth(0, 10 * 256);//��λ1
                sheet.SetColumnWidth(1, 20 * 256);//ʵ������
                sheet.SetColumnWidth(0, 20 * 256);//ʵ�ʽ��1
                sheet.SetColumnWidth(1, 20 * 256);//�ɱ�

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "ԭ����", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ԭ��������", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��λ1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ʵ������", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ʵ�ʽ��1", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "�ɱ�", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, "δ�鼯��", headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, dataTable.Rows.Count + " ��", headStyle2);

                #region �������
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
