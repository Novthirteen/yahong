using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System.Data;
using System.IO;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Batch;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Batch;
using com.Sconit.ISI.Service.Util;
using System.Data.SqlClient;
namespace com.Sconit.ISI.Batch
{
    [Transactional]
    public abstract class RepJob : com.Sconit.Service.Ext.Batch.IJob
    {
        public virtual string appDataFolder { get; set; }
        public ISmtpMgrE smtpMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        public IShiftDetailMgrE shiftDetailMgrE { get; set; }
        public ISqlHelperMgrE sqlHelperMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public IAttachmentDetailMgrE attachmentDetailMgrE { get; set; }
        public IUserMgrE uesrMgrE { get; set; }
        protected static log4net.ILog log = log4net.LogManager.GetLogger("Log.Report");
        protected ICellStyle headStyle;
        protected ICellStyle cellStyle;
        protected ICellStyle cellStyleDet;
        protected ICellStyle cellStyleColor;
        protected ICellStyle cellStyleRed;
        protected ICellStyle headStyle2;
        protected ICellStyle cellStyleCenter;
        private ICellStyle cellStyleDay;
        private ICellStyle cellStyleTitle;
        private ICellStyle cellStyleDayCenter;
        private readonly string SHEET_NAME_NO_CLOSE = "逾期生产单";
        private readonly string SHEET_NAME_CANCEL = "取消生产单";
        protected readonly string SHEET_NAME_12 = "超过12小时生产单";

        public abstract void Execute(JobRunContext context);

        [Transaction(TransactionMode.Unspecified)]
        public virtual void SendEmail(string key, string permissionCode)
        {
            this.SendEmail(key, permissionCode, string.Empty, null, null);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual void SendEmail(string key, string permissionCode, DateTime? startDate, DateTime? endDate)
        {
            this.SendEmail(key, permissionCode, string.Empty, startDate, endDate);
        }
        [Transaction(TransactionMode.Unspecified)]
        public virtual void SendEmail(string key, string permissionCode, string desc, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string mailList = uesrMgrE.FindEmailByPermission(new string[] { permissionCode });

                IList<EntityPreference> entityPreferenceList = entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[]{BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANY_CODE,
                                                    BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME,
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

                    string companyCode = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANY_CODE).SingleOrDefault().Value;

                    if (File.Exists(appDataFolder + companyCode + "/" + (this.GetType().BaseType).Name + ".txt"))
                    {
                        string body = System.IO.File.ReadAllText(appDataFolder + companyCode + "/" + (this.GetType().BaseType).Name + ".txt");
                        content.Append(body.Replace("\r\n", ISIConstants.EMAIL_SEPRATOR));
                    }
                    content.Append(desc);
                    content.Append(separator);
                    //content.Append(separator);
                    //content.Append("&nbsp;&nbsp;请见附件");
                    content.Append(separator);
                    content.Append("信息技术：田甦 15901834083 tiansu@yfgm.com.cn");
                    content.Append(separator);
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

        protected abstract bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate);

        [Transaction(TransactionMode.Requires)]
        protected virtual string GenerateReport(string key, string permissionCode, string desc, IList<EntityPreference> entityPreferenceList, DateTime? startDate, DateTime? endDate)
        {
            string companyName = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).SingleOrDefault().Value;

            HSSFWorkbook workbook = new HSSFWorkbook();

            #region 初始化workbook

            cellStyle = workbook.CreateCellStyle();
            cellStyle.WrapText = true;

            headStyle = workbook.CreateCellStyle();
            IFont font = workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.BOLD;
            font.FontHeightInPoints = (short)10;
            font.IsItalic = true;
            font.Color = HSSFColor.DARK_BLUE.index;
            headStyle.SetFont(font);

            headStyle2 = workbook.CreateCellStyle();
            IFont font3 = workbook.CreateFont();
            font3.Boldweight = (short)FontBoldWeight.BOLD;
            font3.FontHeightInPoints = (short)11;
            font3.Color = HSSFColor.BLUE_GREY.index;
            headStyle2.SetFont(font3);

            cellStyleDet = workbook.CreateCellStyle();
            IFont font1 = workbook.CreateFont();
            font1.IsItalic = true;
            font1.Color = HSSFColor.BLUE_GREY.index;
            cellStyleDet.SetFont(font1);

            cellStyleColor = workbook.CreateCellStyle();
            IFont font2 = workbook.CreateFont();
            //font2.IsItalic = true;
            font2.Color = HSSFColor.LIGHT_ORANGE.index;
            cellStyleColor.WrapText = true;
            cellStyleColor.SetFont(font2);

            cellStyleRed = workbook.CreateCellStyle();
            cellStyleRed.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyleRed.FillForegroundColor = HSSFColor.RED.index;
            //IFont font4 = workbook.CreateFont();
            //font4.Color = HSSFColor.RED.index;
            //cellStyleRed.SetFont(font4);

            cellStyleDay = workbook.CreateCellStyle();
            IFont fontDay = workbook.CreateFont();
            fontDay.Boldweight = (short)FontBoldWeight.BOLD;
            fontDay.FontHeightInPoints = (short)11;
            fontDay.Color = HSSFColor.BLACK.index;
            cellStyleDay.WrapText = true;
            cellStyleDay.SetFont(fontDay);
            cellStyleDay.VerticalAlignment = VerticalAlignment.CENTER;
            //cellStyleDay.Alignment = HorizontalAlignment.CENTER;

            cellStyleTitle = workbook.CreateCellStyle();
            IFont fontBold = workbook.CreateFont();
            fontBold.FontName = "黑体";
            //fontBold.Boldweight = (short)FontBoldWeight.BOLD;
            cellStyleTitle.SetFont(fontBold);
            cellStyleTitle.WrapText = true;

            cellStyleCenter = workbook.CreateCellStyle();
            cellStyleCenter.Alignment = HorizontalAlignment.CENTER;
            cellStyleCenter.VerticalAlignment = VerticalAlignment.CENTER;

            cellStyleDayCenter = workbook.CreateCellStyle();
            cellStyleDayCenter.CloneStyleFrom(cellStyleDay);
            cellStyleDayCenter.Alignment = HorizontalAlignment.CENTER;

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = companyName;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = key;

            workbook.DocumentSummaryInformation = dsi;
            workbook.SummaryInformation = si;
            #endregion

            //填充数据
            if (FileData(key, workbook, startDate, endDate) == false)
            {
                return string.Empty;
            }
            string path = ISIUtil.GetPath(DateTime.Now, false);
            string appDataAllFolder = appDataFolder + path;
            if (!Directory.Exists(appDataAllFolder))//判断是否存在
            {
                Directory.CreateDirectory(appDataAllFolder);//创建新路径
            }

            string fileName = XlsHelper.WriteToLocalZip(companyName + "-" + key + ".xls", appDataAllFolder, workbook);

            FileInfo file = new FileInfo(fileName);
            AttachmentDetail attachment = new AttachmentDetail();
            attachment.TaskCode = path;
            attachment.Size = decimal.Parse((file.Length / 1024.0).ToString());
            attachment.CreateUser = this.uesrMgrE.GetMonitorUser().Code;
            attachment.CreateUserNm = this.uesrMgrE.GetMonitorUser().Name;
            attachment.CreateDate = DateTime.Now;
            attachment.FileName = file.Name;
            attachment.FileExtension = file.Extension;
            attachment.ContentType = "application/x-zip-compressed";
            attachment.ModuleType = permissionCode;
            attachment.Path = path + file.Name;
            this.attachmentDetailMgrE.CreateAttachmentDetail(attachment);
            return fileName;
        }

        protected void OutputItem90(ISheet sheet, int rownum)
        {
            int column = 0;

            XlsHelper.SetRowCell(sheet, rownum, column++, "                                     天数物料", cellStyleDay);

            //	物料号	单位	单包装	小于7	7至60	30至60	60至90	90以上

            //XlsHelper.SetRowCell(sheet, rownum, column++, "物料", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "单位", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "单包装", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "小于7", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "7至30", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "30至60", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "60至90", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "90以上", cellStyleDayCenter);
        }

        #region 订单

        protected bool ProcessSO(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessOrder(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION, workbook, startDate, endDate);
        }

        protected bool ProcessPO(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessOrder(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT, string.Empty, BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PROCUREMENT, workbook, startDate, endDate);
        }

        protected bool ProcessTO(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessOrder(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER, string.Empty, BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_TRANSFER, workbook, startDate, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessOrder(string orderType, string subType, string flowType, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                //当月一号
                DateTime day1 = DateTime.Parse(endDate.Value.ToString("yyyy-MM-01"));
                DateTime startDateT = startDate.HasValue && startDate < day1 ? startDate.Value : day1;

                #region 获取订单头数据
                StringBuilder orderMstrSql = new StringBuilder();
                orderMstrSql.Append(@" select o.OrderNo,o.RefOrderNo,o.ExtOrderNo,f.Desc1+'['+f.Code+']' Flow,o.StartTime,");
                orderMstrSql.Append(@"        o.WindowTime,c1.Desc1 Status,c3.Desc1 Priority,o.SubType,c2.Desc1 SubTypeDesc,p1.Name+'['+p1.Code+']' PartyFrom,");
                orderMstrSql.Append(@"        p2.Name+'['+p2.Code+']' PartyTo,pa1.Address+'['+pa1.Code+']' ShipFrom, pa2.Address+'['+pa2.Code+']' ShipTo, ");
                orderMstrSql.Append(@"        l1.Name+'['+l1.Code+']' LocFrom,l2.Name+'['+l2.Code+']' LocTo,o.DockDesc,c4.Desc1 BillSettleTerm, ");
                orderMstrSql.Append(@"        u.USR_FirstName+u.USR_LastName+'['+u.USR_Code+']' CreateUser, ");
                orderMstrSql.Append(@"        o.Type,c5.Desc1 TypeDesc,c.Name Currency,o.CreateDate ");//,SettleTime
                orderMstrSql.Append(@" from OrderMstr o ");
                orderMstrSql.Append(@" join ACC_User u (nolock) on u.USR_Code=o.CreateUser ");
                orderMstrSql.Append(@" join FlowMstr f (nolock) on o.Flow = f.Code and ");
                if (flowType == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_TRANSFER || flowType == BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION)
                {
                    orderMstrSql.Append(@" f.Type = '" + flowType + "' ");
                }
                else
                {
                    orderMstrSql.Append(@" f.Type in ('" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_SUBCONCTRACTING + "','" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PROCUREMENT + "','" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_CUSTOMERGOODS + "') ");
                }
                orderMstrSql.Append(@" join Party p1 (nolock) on o.PartyFrom=p1.Code ");
                orderMstrSql.Append(@" join Party p2 (nolock) on o.PartyTo=p2.Code ");
                orderMstrSql.Append(@" join PartyAddr pa1 (nolock) on o.ShipFrom=pa1.Code and pa1.AddrType= 'ShipAddr' ");
                orderMstrSql.Append(@" join PartyAddr pa2 (nolock) on o.ShipTo=pa2.Code and pa2.AddrType= 'ShipAddr' ");

                orderMstrSql.Append(@" join CodeMstr c1 (nolock) on o.Status=c1.CodeValue and c1.Code = '" + BusinessConstants.CODE_MASTER_STATUS + "'");
                orderMstrSql.Append(@" join CodeMstr c2 (nolock) on o.SubType=c2.CodeValue and c2.Code = '" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE + "'");
                orderMstrSql.Append(@" join CodeMstr c3 (nolock) on o.Priority=c3.CodeValue and c3.Code = '" + BusinessConstants.CODE_MASTER_ORDER_PRIORITY + "'");
                orderMstrSql.Append(@" join CodeMstr c5 (nolock) on o.Type=c5.CodeValue and c5.Code = '" + BusinessConstants.CODE_MASTER_ORDER_TYPE + "'");
                orderMstrSql.Append(@" left join Location l1 (nolock) on o.LocFrom=l1.Code ");
                orderMstrSql.Append(@" left join Location l2 (nolock) on o.LocTo=l2.Code ");
                orderMstrSql.Append(@" left join CodeMstr c4 (nolock) on o.BillSettleTerm=c4.CodeValue and c4.Code = '" + BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM + "'");
                orderMstrSql.Append(@" left join Currency c (nolock) on o.Currency=c.Code ");
                orderMstrSql.Append(@" where ");

                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    orderMstrSql.Append(@" o.Type = '" + orderType + "' ");
                }
                else
                {
                    orderMstrSql.Append(@" o.Type in ('" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING + "','" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT + "','" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_CUSTOMERGOODS + "') ");
                }

                orderMstrSql.Append(@" and (");
                if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION && subType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN))
                {
                    orderMstrSql.Append(@"          (");
                    orderMstrSql.Append(@"                  o.WindowTime < '" + endDate.Value + "' ");
                    orderMstrSql.Append(@"              and o.Status in  ('" + BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT + "','" + BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS + "') ");
                    orderMstrSql.Append(@"          )");
                }

                if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION && subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN))
                {
                    if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                    {
                        orderMstrSql.Append(@"          or ");
                    }
                    orderMstrSql.Append(@"          (");
                    orderMstrSql.Append(@"                  o.SubType in ('" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN + "','" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_REJ + "')");
                    orderMstrSql.Append(@"              and o.CreateDate >= '" + startDateT + "' and o.CreateDate< '" + endDate.Value + "'");
                    orderMstrSql.Append(@"          )");
                }
                orderMstrSql.Append(@"       )");
                orderMstrSql.Append(@" order by ");

                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    orderMstrSql.Append(@" o.PartyTo ");
                }
                else
                {
                    orderMstrSql.Append(@" o.PartyFrom ");
                }

                orderMstrSql.Append(@" ASC,o.Type ASC,o.SubType ASC,o.WindowTime ASC,o.OrderNo ASC ");

                DataSet orderMstrDS = sqlHelperMgrE.GetDatasetBySql(orderMstrSql.ToString());
                List<OrderMstr> orderMstrList = IListHelper.DataTableToList<OrderMstr>(orderMstrDS.Tables[0]);

                #endregion

                if (orderMstrList != null && orderMstrList.Count > 0)
                {
                    #region 获取明细数据
                    StringBuilder orderDetSql = new StringBuilder();
                    orderDetSql.Append(@" select d.Id,d.OrderNo,i.Desc1+'['+i.Code+']' Item,d.RefItemCode,");
                    orderDetSql.Append(@"        d.Uom,d.UC,d.ReqQty,d.OrderQty,d.ShipQty,d.RecQty,d.RejQty,");
                    orderDetSql.Append(@"        l1.Name+'['+l1.Code+']' LocFrom,l2.Name+'['+l2.Code+']' LocTo,i.NumField3  Price,i.NumField3  * d.OrderQty Amount ");//,d.UnitPriceAfterDiscount
                    orderDetSql.Append(@" from OrderDet d ");
                    orderDetSql.Append(@" join OrderMstr o (nolock) on o.OrderNo = d.OrderNo ");
                    orderDetSql.Append(@" join Item i (nolock) on i.Code = d.Item ");
                    orderDetSql.Append(@" left join Location l1 (nolock) on o.LocFrom=l1.Code ");
                    orderDetSql.Append(@" left join Location l2 (nolock) on o.LocTo=l2.Code ");
                    orderDetSql.Append(@" where ");
                    if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                    {
                        orderDetSql.Append(@" o.Type = '" + orderType + "' ");
                    }
                    else
                    {
                        orderDetSql.Append(@" o.Type in ('" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_SUBCONCTRACTING + "','" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT + "','" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_CUSTOMERGOODS + "') ");
                    }
                    orderDetSql.Append(@" and (");

                    if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION && subType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN))
                    {
                        orderDetSql.Append(@"          (");
                        orderDetSql.Append(@"                  o.WindowTime < '" + endDate.Value + "' ");
                        orderDetSql.Append(@"              and o.Status in  ('" + BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT + "','" + BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS + "') ");
                        orderDetSql.Append(@"          )");
                    }

                    if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION && subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN))
                    {
                        if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                        {
                            orderDetSql.Append(@"          or ");
                        }
                        orderDetSql.Append(@"          (");
                        orderDetSql.Append(@"                  o.SubType in ('" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN + "','" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_REJ + "')");
                        orderDetSql.Append(@"              and o.CreateDate >= '" + startDateT + "' and o.CreateDate< '" + endDate.Value + "'");
                        orderDetSql.Append(@"          )");
                    }
                    orderDetSql.Append(@"       )");
                    orderDetSql.Append(@" order by ");
                    if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                    {
                        orderDetSql.Append(@"       o.PartyTo ");
                    }
                    else
                    {
                        orderDetSql.Append(@"       o.PartyFrom ");
                    }
                    orderDetSql.Append(@" ASC,o.Type ASC,o.SubType ASC,o.WindowTime ASC,o.OrderNo ASC,d.RecQty ASC, d.Seq ASC ");
                    DataSet orderDetDS = sqlHelperMgrE.GetDatasetBySql(orderDetSql.ToString());
                    List<OrderDet> orderDetList = IListHelper.DataTableToList<OrderDet>(orderDetDS.Tables[0]);

                    #endregion

                    return ProcessOrder(orderType, subType, workbook, orderMstrList, orderDetList, startDate.Value, endDate.Value);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        private bool ProcessOrder(string orderType, string subType, IWorkbook workbook, IList<OrderMstr> orderMstrList, IList<OrderDet> orderDetList, DateTime startDate, DateTime endDate)
        {
            if (orderMstrList == null || orderMstrList.Count() == 0) return false;
            string type = string.Empty;
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            {
                type = "销售";
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
            {
                type = "采购";
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
            {
                type = "移库";
            }

            //正常
            var nmlOrderMstrList = orderMstrList.Where(o => o.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML).ToList<OrderMstr>();
            if (nmlOrderMstrList != null && nmlOrderMstrList.Count() > 0)
            {
                ProcessOrder(orderType, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML, string.Empty, "逾期" + type, workbook, nmlOrderMstrList, orderDetList, endDate);
            }
            if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN))
            {
                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    type = "客户";
                }

                //日退货
                var rtn1OrderMstrList = orderMstrList.Where(o => o.CreateDate >= startDate && o.SubType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML).ToList<OrderMstr>();
                if (rtn1OrderMstrList != null && rtn1OrderMstrList.Count() > 0)
                {
                    ProcessOrder(orderType, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN, BusinessConstants.DATETIME_TYPE_DAY, type + "退货(日)", workbook, rtn1OrderMstrList, orderDetList, endDate);
                }

                //月退货
                DateTime day1 = DateTime.Parse(endDate.ToString("yyyy-MM-01"));
                IList<OrderMstr> rtn30OrderMstrList;
                if (startDate < day1)
                {
                    rtn30OrderMstrList = orderMstrList.Where(o => o.CreateDate >= day1 && o.SubType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML).ToList<OrderMstr>();
                }
                else
                {
                    rtn30OrderMstrList = orderMstrList.Where(o => o.SubType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML).ToList<OrderMstr>();
                }
                if (rtn30OrderMstrList != null && rtn30OrderMstrList.Count() > 0)
                {
                    ProcessOrder(orderType, BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN, BusinessConstants.DATETIME_TYPE_MONTH, type + "退货(月)", workbook, rtn30OrderMstrList, orderDetList, endDate);
                }
            }
            return true;
        }

        private bool ProcessOrder(string orderType, string subType, string dateTimeType, string type, IWorkbook workbook, IList<OrderMstr> orderMstrList, IList<OrderDet> orderDetList, DateTime endDate)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet(type);

                for (int i = 0; i <= 20; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }
                sheet.SetColumnWidth(0, 28 * 256);
                sheet.SetColumnWidth(1, 16 * 256);//单号
                sheet.SetColumnWidth(2, 20 * 256);//窗口时间//类型
                sheet.SetColumnWidth(3, 25 * 256);//类别//物料
                sheet.SetColumnWidth(4, 15 * 256);//类型//参考物料号
                sheet.SetColumnWidth(5, 20 * 256);//外部单号//来源库位
                sheet.SetColumnWidth(6, 20 * 256);//参考单号//目的库位
                sheet.SetColumnWidth(7, 10 * 256);//开始时间//单位
                sheet.SetColumnWidth(8, 15 * 256);//区域//单包装
                sheet.SetColumnWidth(9, 9 * 256);//优先级//订单数
                sheet.SetColumnWidth(10, 20 * 256);//来源库位//已发数
                sheet.SetColumnWidth(11, 20 * 256);//目的库位//已收数
                sheet.SetColumnWidth(12, 20 * 256);//发货地址//完成率
                sheet.SetColumnWidth(13, 30 * 256);//收货地址
                sheet.SetColumnWidth(14, 15 * 256);//送货道口
                sheet.SetColumnWidth(15, 7 * 256);//状态
                sheet.SetColumnWidth(16, 10 * 256);//创建时间
                sheet.SetColumnWidth(17, 15 * 256);//创建人
                sheet.SetColumnWidth(18, 18 * 256);//路线
                sheet.SetColumnWidth(19, 10 * 256);//结算方式
                sheet.SetColumnWidth(20, 8 * 256);//币种
                int rownum = 0;
                int column = 0;

                #endregion

                column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, type + "单据：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, orderMstrList.Count() + " 张", headStyle2);

                XlsHelper.SetRowCell(sheet, rownum, column++, type + "明细：", headStyle2);
                var orderDetT1List = orderDetList.Where(d => orderMstrList.Select(m => m.OrderNo).Contains(d.OrderNo));
                int detCount = orderDetT1List.Count();
                XlsHelper.SetRowCell(sheet, rownum, column++, detCount + " 笔", headStyle2);

                if (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN && orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
                {
                    XlsHelper.SetRowCell(sheet, rownum, column++, "金额：", headStyle2);
                    var amount = orderDetT1List.Sum(d => d.Amount);
                    if (amount.HasValue)
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, amount.Value.ToString("0.########"), headStyle2);
                    }
                }

                rownum++;
                rownum++;

                #region 输出数据
                DateTime? WindowTime = null;
                string party = string.Empty;
                IEnumerable<OrderMstr> orderMstrSubList = null;
                IEnumerable<OrderDet> orderDetSubList = null;
                int partyOrderNum = 0;
                //int partyNum = 0;
                for (int i = 0; i < orderMstrList.Count(); i++)
                {
                    var orderMstr = orderMstrList[i];

                    if (i == 0 || orderMstr.Party != orderMstrList[i - 1].Party)
                    {
                        if (i == 0)
                        {
                            if (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML ||
                                    (subType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML &&
                                     dateTimeType == BusinessConstants.DATETIME_TYPE_MONTH))
                            {
                                //输出天数表头
                                OutputParty15(sheet, orderType, rownum);
                                rownum++;
                            }
                            else if (subType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML &&
                                dateTimeType == BusinessConstants.DATETIME_TYPE_DAY)
                            {
                                column = 0;
                                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column++, "客户", cellStyleDay);
                                }
                                else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column++, "区域", cellStyleDay);
                                }
                                else
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column++, "供应商", cellStyleDay);
                                }
                                XlsHelper.SetRowCell(sheet, rownum, column++, "退货单数", cellStyleDayCenter);
                                XlsHelper.SetRowCell(sheet, rownum, column++, "退货数", cellStyleDayCenter);
                                if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column++, "金额", cellStyleDayCenter);
                                }
                                rownum++;
                            }
                        }
                        //供应商、来源区域、客户的合计
                        orderMstrSubList = orderMstrList.Where(d => d.Party == orderMstr.Party);
                        orderDetSubList = orderDetList.Where(d => orderMstrSubList.Select(m => m.OrderNo).Contains(d.OrderNo));
                        column = 0;
                        XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.Party, cellStyleTitle);

                        if (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
                        {
                            #region 天统计

                            //1-15天
                            var day1Orders = orderMstrSubList.Where(o => o.WindowTime <= endDate && endDate < o.WindowTime.AddDays(2)).Select(m => m.OrderNo);
                            if (day1Orders.Count() > 0)
                            {
                                decimal day1 = orderDetSubList.Where(d => day1Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day1.ToString("0.########") + "/" + day1Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day2Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(2) <= endDate && endDate < o.WindowTime.AddDays(3)).Select(m => m.OrderNo);
                            if (day2Orders.Count() > 0)
                            {
                                decimal day2 = orderDetSubList.Where(d => day2Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day2.ToString("0.########") + "/" + day2Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day3Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(3) <= endDate && endDate < o.WindowTime.AddDays(4)).Select(m => m.OrderNo);
                            if (day3Orders.Count() > 0)
                            {
                                decimal day3 = orderDetSubList.Where(d => day3Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day3.ToString("0.########") + "/" + day3Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day4Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(4) <= endDate && endDate < o.WindowTime.AddDays(5)).Select(m => m.OrderNo);
                            if (day4Orders.Count() > 0)
                            {
                                decimal day4 = orderDetSubList.Where(d => day4Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day4.ToString("0.########") + "/" + day4Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day5Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(5) <= endDate && endDate < o.WindowTime.AddDays(6)).Select(m => m.OrderNo);
                            if (day5Orders.Count() > 0)
                            {
                                decimal day5 = orderDetSubList.Where(d => day5Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day5.ToString("0.########") + "/" + day5Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day6Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(6) <= endDate && endDate < o.WindowTime.AddDays(7)).Select(m => m.OrderNo);
                            if (day6Orders.Count() > 0)
                            {
                                decimal day6 = orderDetSubList.Where(d => day6Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day6.ToString("0.########") + "/" + day6Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day7Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(7) <= endDate && endDate < o.WindowTime.AddDays(8)).Select(m => m.OrderNo);
                            if (day7Orders.Count() > 0)
                            {
                                decimal day7 = orderDetSubList.Where(d => day7Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day7.ToString("0.########") + "/" + day7Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day8Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(8) <= endDate && endDate < o.WindowTime.AddDays(9)).Select(m => m.OrderNo);
                            if (day8Orders.Count() > 0)
                            {
                                decimal day8 = orderDetSubList.Where(d => day8Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day8.ToString("0.########") + "/" + day8Orders.Count(), cellStyleCenter);
                            }
                            column++;


                            var day9Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(9) <= endDate && endDate < o.WindowTime.AddDays(10)).Select(m => m.OrderNo);
                            if (day9Orders.Count() > 0)
                            {
                                decimal day9 = orderDetSubList.Where(d => day9Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day9.ToString("0.########") + "/" + day9Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day10Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(10) <= endDate && endDate < o.WindowTime.AddDays(11)).Select(m => m.OrderNo);
                            if (day10Orders.Count() > 0)
                            {
                                decimal day10 = orderDetSubList.Where(d => day10Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day10.ToString("0.########") + "/" + day10Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day11Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(11) <= endDate && endDate < o.WindowTime.AddDays(12)).Select(m => m.OrderNo);
                            if (day11Orders.Count() > 0)
                            {
                                decimal day11 = orderDetSubList.Where(d => day11Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day11.ToString("0.########") + "/" + day11Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day12Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(12) <= endDate && endDate < o.WindowTime.AddDays(13)).Select(m => m.OrderNo);
                            if (day12Orders.Count() > 0)
                            {
                                decimal day12 = orderDetSubList.Where(d => day12Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day12.ToString("0.########") + "/" + day12Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day13Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(13) <= endDate && endDate < o.WindowTime.AddDays(14)).Select(m => m.OrderNo);
                            if (day13Orders.Count() > 0)
                            {
                                decimal day13 = orderDetSubList.Where(d => day13Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day13.ToString("0.########") + "/" + day13Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day14Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(14) <= endDate && endDate < o.WindowTime.AddDays(15)).Select(m => m.OrderNo);
                            if (day14Orders.Count() > 0)
                            {
                                decimal day14 = orderDetSubList.Where(d => day14Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day14.ToString("0.########") + "/" + day14Orders.Count(), cellStyleCenter);
                            }
                            column++;


                            var day15Orders = orderMstrSubList.Where(o => o.WindowTime.AddDays(15) <= endDate && endDate < o.WindowTime.AddDays(16)).Select(m => m.OrderNo);
                            if (day15Orders.Count() > 0)
                            {
                                decimal day15 = orderDetSubList.Where(d => day15Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day15.ToString("0.########") + "/" + day15Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            //var day16Orders = orderMstrSubList.Where(o => endDate.AddDays(-16) >= o.WindowTime).Select(m => m.OrderNo);
                            var day16Orders = orderMstrSubList.Select(m => m.OrderNo);
                            if (day16Orders.Count() > 0)
                            {
                                decimal day16 = orderDetSubList.Where(d => day16Orders.Contains(d.OrderNo)).Sum(d => d.OrderQty);
                                XlsHelper.SetRowCell(sheet, rownum, column, day16.ToString("0.########") + "/" + day16Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            #endregion
                        }
                        else if (subType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML && dateTimeType == BusinessConstants.DATETIME_TYPE_MONTH)
                        {
                            #region 天统计
                            //1-15天
                            var day1Orders = orderMstrSubList.Where(o => o.CreateDate <= endDate && endDate < o.CreateDate.AddDays(2)).Select(m => m.OrderNo);
                            if (day1Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day1Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day1Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day2Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(2) <= endDate && endDate < o.CreateDate.AddDays(3)).Select(m => m.OrderNo);
                            if (day2Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day2Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day2Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;


                            var day3Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(3) <= endDate && endDate < o.CreateDate.AddDays(4)).Select(m => m.OrderNo);
                            if (day3Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day3Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day3Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day4Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(4) <= endDate && endDate < o.CreateDate.AddDays(5)).Select(m => m.OrderNo);
                            if (day4Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day4Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day4Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day5Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(5) <= endDate && endDate < o.CreateDate.AddDays(6)).Select(m => m.OrderNo);
                            if (day5Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day5Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day5Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day6Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(6) <= endDate && endDate < o.CreateDate.AddDays(7)).Select(m => m.OrderNo);
                            if (day6Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day6Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day6Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day7Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(7) <= endDate && endDate < o.CreateDate.AddDays(8)).Select(m => m.OrderNo);
                            if (day7Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day7Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day7Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day8Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(8) <= endDate && endDate < o.CreateDate.AddDays(9)).Select(m => m.OrderNo);
                            if (day8Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day8Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day8Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;


                            var day9Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(9) <= endDate && endDate < o.CreateDate.AddDays(10)).Select(m => m.OrderNo);
                            if (day9Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day9Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day9Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day10Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(10) <= endDate && endDate < o.CreateDate.AddDays(11)).Select(m => m.OrderNo);
                            if (day10Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day10Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day10Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day11Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(11) <= endDate && endDate < o.CreateDate.AddDays(12)).Select(m => m.OrderNo);
                            if (day11Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day11Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day11Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day12Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(12) <= endDate && endDate < o.CreateDate.AddDays(13)).Select(m => m.OrderNo);
                            if (day12Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day12Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day12Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day13Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(13) <= endDate && endDate < o.CreateDate.AddDays(14)).Select(m => m.OrderNo);
                            if (day13Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day13Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day13Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            var day14Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(14) <= endDate && endDate < o.CreateDate.AddDays(15)).Select(m => m.OrderNo);
                            if (day14Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day14Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day14Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;


                            var day15Orders = orderMstrSubList.Where(o => o.CreateDate.AddDays(15) <= endDate && endDate < o.CreateDate.AddDays(16)).Select(m => m.OrderNo);
                            if (day15Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day15Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day15Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            //var day16Orders = orderMstrSubList.Where(o => endDate.AddDays(-16) >= o.CreateDate).Select(m => m.OrderNo);
                            var day16Orders = orderMstrSubList.Select(m => m.OrderNo);
                            if (day16Orders.Count() > 0)
                            {
                                var orderDetSub2List = orderDetSubList.Where(d => day16Orders.Contains(d.OrderNo));
                                decimal day = orderDetSub2List.Sum(d => d.OrderQty);
                                var amountT = orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT ? orderDetSub2List.Sum(d => d.Amount) : decimal.Zero;
                                XlsHelper.SetRowCell(sheet, rownum, column, day.ToString("0.########") + "/" + day16Orders.Count() + (amountT.HasValue && amountT.Value != decimal.Zero ? "/" + amountT.Value.ToString("0.########元") : string.Empty), cellStyleCenter);
                            }
                            column++;

                            #endregion
                        }
                        else
                        {
                            decimal subDetCount = orderDetSubList.Sum(d => d.OrderQty);
                            var orderDetSubAmount = orderDetSubList.Sum(d => d.Amount);
                            XlsHelper.SetRowCell(sheet, rownum, column++, orderMstrSubList.Count().ToString());
                            XlsHelper.SetRowCell(sheet, rownum, column++, subDetCount.ToString("0.########"));
                            if (orderType != BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column++,
                                                     orderDetSubAmount.HasValue
                                                         ? orderDetSubAmount.Value.ToString("0.########")
                                                         : string.Empty);
                            }
                        }
                        rownum++;
                        //输出列头
                        OutputOrderColunmHead(orderType, subType, sheet, rownum++);
                        //partyNum++;
                        partyOrderNum = 0;
                    }
                    column = 1;
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.OrderNo);
                    if (!WindowTime.HasValue || WindowTime.Value.ToString("yyyy-MM-dd").CompareTo(orderMstr.WindowTime.ToString("yyyy-MM-dd")) != 0
                           || partyOrderNum == 0)
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.WindowTime.ToString("yyyy-MM-dd HH:mm"), cellStyleColor);
                    }
                    else
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.WindowTime.ToString("yyyy-MM-dd HH:mm"));
                    }
                    WindowTime = orderMstr.WindowTime;
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.TypeDesc);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.SubTypeDesc);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.ExtOrderNo);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.RefOrderNo);

                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.StartTime.ToString("yyyy-MM-dd HH:mm"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.RefParty);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.Priority);

                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.LocFrom);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.LocTo);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.ShipFrom);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.ShipTo);

                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.DockDesc);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.Status);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.CreateDate.ToString("yyyy-MM-dd HH:mm"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.CreateUser);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.Flow);

                    var orderDetTList = orderDetSubList.Where(d => d.OrderNo == orderMstr.OrderNo).ToList<OrderDet>();

                    if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.BillSettleTerm);
                        XlsHelper.SetRowCell(sheet, rownum, column++, orderMstr.Currency);
                        if (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN)
                        {
                            var amount2 = orderDetTList.Sum(d => d.Amount);
                            if (amount2.HasValue)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column++, amount2.Value.ToString("0.########"));
                            }
                        }
                    }
                    #region 发货明细

                    if (orderDetTList != null && orderDetTList.Count > 0)
                    {
                        rownum++;

                        OutputOrderDetColumnHead(orderType, subType, sheet, rownum);

                        foreach (var orderDet in orderDetTList)
                        {
                            column = 2;
                            rownum++;
                            if (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column++, "退货");
                            }
                            else
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.Type);
                            }
                            XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.Item);
                            XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.RefItemCode);
                            XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.LocFrom);
                            XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.LocTo);
                            XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.Uom);
                            if (orderDet.UC != 0)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column, orderDet.UC.ToString("0.########"));
                            }
                            column++;
                            XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.OrderQty.ToString("0.########"));
                            if (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
                            {
                                if (orderDet.ShipQty.HasValue)
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column, orderDet.ShipQty.Value.ToString("0.########"));
                                }
                                column++;
                                if (orderDet.RecQty.HasValue)
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column, orderDet.RecQty.Value.ToString("0.########"));
                                }
                                column++;
                                XlsHelper.SetRowCell(sheet, rownum, column++, orderDet.CompleteRate.ToString("P"));
                            }
                            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
                            {
                                if (orderDet.Price.HasValue)
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column, orderDet.Price.Value.ToString("0.########"));
                                }
                                column++;
                                if (orderDet.Amount.HasValue)
                                {
                                    XlsHelper.SetRowCell(sheet, rownum, column, orderDet.Amount.Value.ToString("0.########"));
                                }
                            }
                        }

                        sheet.GroupRow(rownum - orderDetTList.Count, rownum);
                        sheet.SetRowGroupCollapsed(rownum, true);
                    }
                    #endregion

                    if (partyOrderNum == orderMstrSubList.Count() - 1)
                    {
                        rownum++;
                        sheet.GroupRow(rownum - orderMstrSubList.Count() * 2 - orderDetSubList.Count() - 1, rownum);
                        sheet.SetRowGroupCollapsed(rownum, true);
                    }
                    partyOrderNum++;
                    rownum++;
                }
                //  sheet.GroupRow(2, rownum);
                //  sheet.SetRowGroupCollapsed(rownum, true);
                /*
                column = 0;
                rownum += 2;
                
                if (dateTimeType != BusinessConstants.DATETIME_TYPE_MONTH)
                {
                    XlsHelper.SetRowCell(sheet, rownum, column++, "合计", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderMstrList.Count().ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, column++, orderDetList.Sum(d => d.OrderQty).ToString("0.########"), headStyle);
                    var amount = orderDetList.Sum(d => d.Amount);
                    if (amount.HasValue)
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++,
                                             amount.Value.ToString("0.########"), headStyle);
                    }
                    sheet.GroupColumn(12, 18);
                    sheet.SetColumnGroupCollapsed(18, true);
                }
                */
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

        private void OutputOrderDetColumnHead(string orderType, string subType, ISheet sheet, int rownum)
        {
            int column = 2;
            XlsHelper.SetRowCell(sheet, rownum, column++, "类型", cellStyleDet);
            XlsHelper.SetRowCell(sheet, rownum, column++, "物料", cellStyleDet);
            XlsHelper.SetRowCell(sheet, rownum, column++, "参考物料号", cellStyleDet);
            XlsHelper.SetRowCell(sheet, rownum, column++, "来源库位", cellStyleDet);
            XlsHelper.SetRowCell(sheet, rownum, column++, "目的库位", cellStyleDet);
            XlsHelper.SetRowCell(sheet, rownum, column++, "单位", cellStyleDet);
            XlsHelper.SetRowCell(sheet, rownum, column++, "单包装", cellStyleDet);

            if (subType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
            {
                XlsHelper.SetRowCell(sheet, rownum, column++, "订单数", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "已发数", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "已收数", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "完成率", cellStyleDet);
            }
            else
            {
                XlsHelper.SetRowCell(sheet, rownum, column++, "退货数", cellStyleDet);
                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
                {
                    XlsHelper.SetRowCell(sheet, rownum, column++, "单价", cellStyleDet);
                    XlsHelper.SetRowCell(sheet, rownum, column++, "金额", cellStyleDet);
                }
            }
        }

        private void OutputOrderColunmHead(string orderType, string subType, ISheet sheet, int rownum)
        {
            int colnum = 1;

            XlsHelper.SetRowCell(sheet, rownum, colnum++, "单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "窗口时间", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "类别", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "类型", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "外部单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "参考单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "开始时间", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "区域", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "优先级", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "来源库位", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "目的库位", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "发货地址", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "收货地址", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "送货道口", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "状态", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "创建时间", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "创建人", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "路线", headStyle);
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION || orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
            {
                XlsHelper.SetRowCell(sheet, rownum, colnum++, "结算方式", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, colnum++, "币种", headStyle);
                if (subType != BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML)
                {
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "金额", headStyle);
                }
            }
        }

        #endregion

        #region 差异

        protected bool ProcessSOGap(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessGap(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION, workbook, startDate, endDate);
        }

        protected bool ProcessPOGap(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessGap(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT, workbook, startDate, endDate);
        }

        protected bool ProcessTOGap(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessGap(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER, workbook, startDate, endDate);
        }

        //收货差异
        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessGap(string orderType, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                DateTime day1 = DateTime.Parse(endDate.Value.ToString("yyyy-MM-01"));
                DateTime startDateT = startDate.HasValue && startDate < day1 ? startDate.Value : day1;

                #region 获取明细数据
                StringBuilder ipDetSql = new StringBuilder();
                ipDetSql.Append(@"select d.IpNo,i.Desc1+'['+ i.Code+']' Item,d.RefItemCode, ");
                ipDetSql.Append(@"       d.HuId,d.LotNo,d.Qty, isnull(d.Uom,od.Uom) Uom,od.UC UC,d.RecQty,d.Qty - d.RecQty DiffQty, ");//d.CustomerItemCode,
                ipDetSql.Append(@"       ip.DockDesc,ip.OrderType, ");//,c1.Desc1 Status
                ipDetSql.Append(@"       ip.CreateDate,ip.DepartTime,ip.ArriveTime,ip.LastModifyDate, ");
                ipDetSql.Append(@"       u.USR_FirstName+u.USR_LastName+'['+u.USR_Code+']' CreateUser, ");
                ipDetSql.Append(@"       p1.Name+'['+p1.Code+']' PartyFrom,p2.Name+'['+p2.Code+']' PartyTo, ");
                ipDetSql.Append(@"       pa1.Address+'['+pa1.Code+']' ShipFrom, pa2.Address+'['+pa2.Code+']' ShipTo ");
                ipDetSql.Append(@"from IpMstr ip ");
                ipDetSql.Append(@"join IpDet d (nolock) on ip.ipno=d.ipno and ip.OrderType='" + orderType + "' ");
                ipDetSql.Append(@"join OrderLocTrans olt (nolock) on d.OrderLocTransId =olt.Id ");
                ipDetSql.Append(@"join OrderDet od (nolock) on olt.OrderDetId=od.Id  ");
                ipDetSql.Append(@"join Item i (nolock) on olt.Item=i.Code ");
                ipDetSql.Append(@"join Party p1 (nolock) on p1.Code=ip.PartyFrom  ");
                ipDetSql.Append(@"join Party p2 (nolock) on p2.Code=ip.PartyTo ");
                ipDetSql.Append(@"join PartyAddr pa1 (nolock) on ip.ShipFrom=pa1.Code and pa1.AddrType= 'ShipAddr' ");
                ipDetSql.Append(@"join PartyAddr pa2 (nolock) on ip.ShipTo=pa2.Code and pa2.AddrType= 'ShipAddr' ");
                ipDetSql.Append(@"join ACC_User u (nolock) on u.USR_Code=ip.CreateUser ");
                ipDetSql.Append(@"where ");
                ipDetSql.Append(@"      d.RecQty != d.Qty ");
                ipDetSql.Append(@"and   ip.Status ='" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE + "' ");
                ipDetSql.Append(@"and   ip.Type='" + BusinessConstants.CODE_MASTER_INPROCESS_LOCATION_TYPE_VALUE_NORMAL + "'  ");
                ipDetSql.Append(@"and   ip.LastModifyDate >= '" + startDateT + "' and ip.LastModifyDate< '" + endDate.Value + "' ");
                ipDetSql.Append(@"order by ");

                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    ipDetSql.Append(@"ip.PartyTo ");
                }
                else
                {
                    ipDetSql.Append(@"ip.PartyFrom ");
                }

                ipDetSql.Append(@"ASC,ip.ArriveTime ASC,ip.IpNo ASC,d.Id ASC");
                DataSet ipDetDS = sqlHelperMgrE.GetDatasetBySql(ipDetSql.ToString());
                List<IpDet> ipDetList = IListHelper.DataTableToList<IpDet>(ipDetDS.Tables[0]);

                #endregion

                if (ipDetList != null && ipDetList.Count > 0)
                {
                    return ProcessGap(orderType, workbook, ipDetList, startDate.Value, endDate.Value);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        private bool ProcessGap(string orderType, IWorkbook workbook, IList<IpDet> ipDetList, DateTime startDate, DateTime endDate)
        {
            if (ipDetList == null || ipDetList.Count() == 0) return false;
            string type = string.Empty;
            /*
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            {
                type = "客户";
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
            {
                type = "采购";
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
            {
                type = "移库";
            }
            */
            //差异（日）
            var rtn1IpDetList = ipDetList.Where(o => o.LastModifyDate >= startDate).ToList<IpDet>();
            if (rtn1IpDetList != null && rtn1IpDetList.Count() > 0)
            {
                ProcessGap(orderType, BusinessConstants.DATETIME_TYPE_DAY, type + "收货差异(日)", workbook, rtn1IpDetList, endDate);
            }

            //差异（月）
            DateTime day1 = DateTime.Parse(endDate.ToString("yyyy-MM-01"));
            if (startDate < day1)
            {
                ipDetList = ipDetList.Where(o => o.LastModifyDate >= day1).ToList<IpDet>();
            }
            ProcessGap(orderType, BusinessConstants.DATETIME_TYPE_MONTH, type + "收货差异(月)", workbook, ipDetList, endDate);

            return true;
        }

        private bool ProcessGap(string orderType, string dateTimeType, string type, IWorkbook workbook, IList<IpDet> ipDetList, DateTime endDate)
        {
            try
            {
                if (ipDetList == null || ipDetList.Count() == 0) return false;

                #region 初始化Sheet

                ISheet gapSheet = workbook.CreateSheet(type);
                for (int i = 0; i <= 17; i++)
                {
                    //if (dateTimetype == BusinessConstants.DATETIME_TYPE_DAY && (i == 1 || i == 3)) continue;
                    gapSheet.AutoSizeColumn(i);
                    gapSheet.SetDefaultColumnStyle(i, cellStyle);
                }
                gapSheet.SetColumnWidth(0, 28 * 256); //客户、供应商、来源区域
                gapSheet.SetColumnWidth(1, 15 * 256); //送货单号
                gapSheet.SetColumnWidth(2, 15 * 256); //类型
                gapSheet.SetColumnWidth(3, 25 * 256); //物料
                gapSheet.SetColumnWidth(5, 10 * 256); //发运时间
                gapSheet.SetColumnWidth(6, 10 * 256); //预计送达
                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
                {
                    gapSheet.SetColumnWidth(4, 10 * 256); //参考物料号
                    gapSheet.SetColumnWidth(7, 22 * 256); //创建人
                }
                else
                {
                    gapSheet.SetColumnWidth(4, 15 * 256); //参考物料号
                    gapSheet.SetColumnWidth(7, 15 * 256); //创建人
                }
                gapSheet.SetColumnWidth(8, 7 * 256);//单位
                gapSheet.SetColumnWidth(9, 7 * 256);//单包装
                gapSheet.SetColumnWidth(10, 25 * 256);//区域
                gapSheet.SetColumnWidth(11, 25 * 256);//发货地址
                gapSheet.SetColumnWidth(12, 25 * 256);//收货地址
                gapSheet.SetColumnWidth(13, 15 * 256);//送货道口                               
                gapSheet.SetColumnWidth(14, 10 * 256);//批号
                gapSheet.SetColumnWidth(15, 20 * 256);//条码
                gapSheet.SetColumnWidth(16, 10 * 256);//发货数量
                gapSheet.SetColumnWidth(17, 10 * 256);//收货数量
                //gapSheet.SetColumnWidth(18, 10 * 256);//差异
                int rownum = 0;
                int column = 0;

                #endregion

                #region 输出数据
                DateTime? ArriveTime = null;
                decimal qtyTotal = 0;
                decimal recQtyTotal = 0;
                decimal diffQtyTotal = 0;
                IEnumerable<IpDet> ipDetSubList = null;
                for (int i = 0, num = 0; i < ipDetList.Count(); i++)
                {
                    var ipDet = ipDetList[i];
                    column = 1;
                    if (i == 0 || ipDet.Party != ipDetList[i - 1].Party)
                    {
                        if (i == 0)
                        {
                            if (dateTimeType == BusinessConstants.DATETIME_TYPE_MONTH)
                            {
                                //输出天数表头
                                OutputParty15(gapSheet, orderType, rownum);
                                rownum++;
                            }
                            else if (dateTimeType == BusinessConstants.DATETIME_TYPE_DAY)
                            {
                                column = 0;
                                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                                {
                                    XlsHelper.SetRowCell(gapSheet, rownum, column++, "客户", cellStyleDay);
                                }
                                else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
                                {
                                    XlsHelper.SetRowCell(gapSheet, rownum, column++, "区域", cellStyleDay);
                                }
                                else
                                {
                                    XlsHelper.SetRowCell(gapSheet, rownum, column++, "供应商", cellStyleDay);
                                }
                                XlsHelper.SetRowCell(gapSheet, rownum, column++, "发货数", cellStyleDayCenter);
                                XlsHelper.SetRowCell(gapSheet, rownum, column++, "收货数", cellStyleDayCenter);
                                XlsHelper.SetRowCell(gapSheet, rownum, column++, "差异", cellStyleDayCenter);
                                rownum++;
                            }
                        }
                        //供应商、来源区域、客户的合计
                        ipDetSubList = ipDetList.Where(d => d.Party == ipDet.Party);
                        column = 0;
                        XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.Party, cellStyleTitle);
                        if (dateTimeType == BusinessConstants.DATETIME_TYPE_MONTH)
                        {
                            #region 天统计

                            //1-15天
                            var day1Orders = ipDetSubList.Where(o => o.LastModifyDate <= endDate && endDate < o.LastModifyDate.AddDays(2)).Select(m => m.IpNo);
                            if (day1Orders.Count() > 0)
                            {
                                decimal day1 = ipDetSubList.Where(d => day1Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day1.ToString("0.########") + "/" + day1Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day2Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(2) <= endDate && endDate < o.LastModifyDate.AddDays(3)).Select(m => m.IpNo);
                            if (day2Orders.Count() > 0)
                            {
                                decimal day2 = ipDetSubList.Where(d => day2Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day2.ToString("0.########") + "/" + day2Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day3Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(3) <= endDate && endDate < o.LastModifyDate.AddDays(4)).Select(m => m.IpNo);
                            if (day3Orders.Count() > 0)
                            {
                                decimal day3 = ipDetSubList.Where(d => day3Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day3.ToString("0.########") + "/" + day3Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day4Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(4) <= endDate && endDate < o.LastModifyDate.AddDays(5)).Select(m => m.IpNo);
                            if (day4Orders.Count() > 0)
                            {
                                decimal day4 = ipDetSubList.Where(d => day4Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day4.ToString("0.########") + "/" + day4Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day5Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(5) <= endDate && endDate < o.LastModifyDate.AddDays(6)).Select(m => m.IpNo);
                            if (day5Orders.Count() > 0)
                            {
                                decimal day5 = ipDetSubList.Where(d => day5Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day5.ToString("0.########") + "/" + day5Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day6Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(6) <= endDate && endDate < o.LastModifyDate.AddDays(7)).Select(m => m.IpNo);
                            if (day6Orders.Count() > 0)
                            {
                                decimal day6 = ipDetSubList.Where(d => day6Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day6.ToString("0.########") + "/" + day6Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day7Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(7) <= endDate && endDate < o.LastModifyDate.AddDays(8)).Select(m => m.IpNo);
                            if (day7Orders.Count() > 0)
                            {
                                decimal day7 = ipDetSubList.Where(d => day7Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day7.ToString("0.########") + "/" + day7Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day8Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(8) <= endDate && endDate < o.LastModifyDate.AddDays(9)).Select(m => m.IpNo);
                            if (day8Orders.Count() > 0)
                            {
                                decimal day8 = ipDetSubList.Where(d => day8Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day8.ToString("0.########") + "/" + day8Orders.Count(), cellStyleCenter);
                            }
                            column++;


                            var day9Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(9) <= endDate && endDate < o.LastModifyDate.AddDays(10)).Select(m => m.IpNo);
                            if (day9Orders.Count() > 0)
                            {
                                decimal day9 = ipDetSubList.Where(d => day9Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day9.ToString("0.########") + "/" + day9Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day10Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(10) <= endDate && endDate < o.LastModifyDate.AddDays(11)).Select(m => m.IpNo);
                            if (day10Orders.Count() > 0)
                            {
                                decimal day10 = ipDetSubList.Where(d => day10Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day10.ToString("0.########") + "/" + day10Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day11Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(11) <= endDate && endDate < o.LastModifyDate.AddDays(12)).Select(m => m.IpNo);
                            if (day11Orders.Count() > 0)
                            {
                                decimal day11 = ipDetSubList.Where(d => day11Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day11.ToString("0.########") + "/" + day11Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day12Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(12) <= endDate && endDate < o.LastModifyDate.AddDays(13)).Select(m => m.IpNo);
                            if (day12Orders.Count() > 0)
                            {
                                decimal day12 = ipDetSubList.Where(d => day12Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day12.ToString("0.########") + "/" + day12Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day13Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(13) <= endDate && endDate < o.LastModifyDate.AddDays(14)).Select(m => m.IpNo);
                            if (day13Orders.Count() > 0)
                            {
                                decimal day13 = ipDetSubList.Where(d => day13Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day13.ToString("0.########") + "/" + day13Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day14Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(14) <= endDate && endDate < o.LastModifyDate.AddDays(15)).Select(m => m.IpNo);
                            if (day14Orders.Count() > 0)
                            {
                                decimal day14 = ipDetSubList.Where(d => day14Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day14.ToString("0.########") + "/" + day14Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            var day15Orders = ipDetSubList.Where(o => o.LastModifyDate.AddDays(15) <= endDate && endDate < o.LastModifyDate.AddDays(16)).Select(m => m.IpNo);
                            if (day15Orders.Count() > 0)
                            {
                                decimal day15 = ipDetSubList.Where(d => day15Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day15.ToString("0.########") + "/" + day15Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            //var day16Orders = ipDetSubList.Where(o => endDate.AddDays(-16) >= o.LastModifyDate).Select(m => m.IpNo);
                            var day16Orders = ipDetSubList.Select(m => m.IpNo);
                            if (day16Orders.Count() > 0)
                            {
                                decimal day16 = ipDetSubList.Where(d => day16Orders.Contains(d.IpNo)).Sum(d => d.Qty - d.RecQty);
                                XlsHelper.SetRowCell(gapSheet, rownum, column, day16.ToString("0.########") + "/" + day16Orders.Count(), cellStyleCenter);
                            }
                            column++;

                            #endregion
                        }
                        else
                        {
                            //供应商、来源区域、客户的合计
                            XlsHelper.SetRowCell(gapSheet, rownum, 0, ipDet.Party, cellStyleTitle);
                            ipDetSubList = ipDetList.Where(d => d.Party == ipDet.Party);
                            //XlsHelper.SetRowCell(gapSheet, rownum, 1, "合计: ");
                            XlsHelper.SetRowCell(gapSheet, rownum, 1, ipDetSubList.Sum(d => d.Qty).ToString("0.########"));
                            XlsHelper.SetRowCell(gapSheet, rownum, 2, ipDetSubList.Sum(d => d.RecQty).ToString("0.########"));
                            XlsHelper.SetRowCell(gapSheet, rownum, 3, ipDetSubList.Sum(d => d.DiffQty).ToString("0.########"));
                        }
                        rownum++;
                        //输出列头
                        OutputGapColunmHead(orderType, gapSheet, rownum++);
                        column = 1;
                        num = 0;
                    }
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.IpNo);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.Status);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.Item);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.RefItemCode);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.DepartTime.ToString("yyyy-MM-dd HH:mm"));
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.ArriveTime.ToString("yyyy-MM-dd HH:mm"));
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.CreateUser);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.Uom);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.UC.ToString("0.########"));
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.RefParty);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.ShipFrom);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.ShipTo);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.DockDesc);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.LotNo);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.HuId);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.Qty.ToString("0.########"));
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.RecQty.ToString("0.########"));
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, ipDet.DiffQty.ToString("0.########"));


                    qtyTotal += ipDet.Qty;
                    recQtyTotal += ipDet.RecQty;
                    diffQtyTotal += ipDet.DiffQty;

                    if (num == ipDetSubList.Count() - 1)
                    {
                        rownum++;
                        gapSheet.GroupRow(rownum - ipDetSubList.Count() - 1, rownum);
                        gapSheet.SetRowGroupCollapsed(rownum, true);
                    }
                    rownum++;
                    num++;
                }
                column = 0;
                rownum += 2;

                if (dateTimeType != BusinessConstants.DATETIME_TYPE_MONTH)
                {
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, "合计", headStyle);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, qtyTotal.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, recQtyTotal.ToString("0.########"), headStyle);
                    XlsHelper.SetRowCell(gapSheet, rownum, column++, diffQtyTotal.ToString("0.########"), headStyle);

                    gapSheet.GroupColumn(11, 15);
                    gapSheet.SetColumnGroupCollapsed(15, true);
                }
                gapSheet.ForceFormulaRecalculation = true;
                gapSheet.CreateFreezePane(1, 0, 1, 0);

                #endregion

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        private void OutputParty360(ISheet sheet, string transType, int rownum)
        {
            int column = 0;
            if (transType == BusinessConstants.BILL_TRANS_TYPE_SO)
            {
                XlsHelper.SetRowCell(sheet, rownum, column++, "                                     天数客户", cellStyleDay);
            }
            else if (transType == BusinessConstants.BILL_TRANS_TYPE_PO)
            {
                XlsHelper.SetRowCell(sheet, rownum, column++, "                                     天数供应商", cellStyleDay);
            }

            //	物料	单位	单包装	小于5   5至30	30至60	60至90	90至120	120至150	150至180	180至210	210至360	大于360

            XlsHelper.SetRowCell(sheet, rownum, column++, "物料", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "单位", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "单包装", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "小于5", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "5至30", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "30至60", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "60至90", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "90至120", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "120至150", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "150至180", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "180至210", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "210至360", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "大于360", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "合计", cellStyleDayCenter);
        }

        private void OutputParty15(ISheet sheet, string orderType, int rownum)
        {
            int column = 0;
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            {
                XlsHelper.SetRowCell(sheet, rownum, column++, "                                     天数客户", cellStyleDay);
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
            {
                XlsHelper.SetRowCell(sheet, rownum, column++, "                                     天数区域", cellStyleDay);
            }
            else
            {
                XlsHelper.SetRowCell(sheet, rownum, column++, "                                     天数供应商", cellStyleDay);
            }

            XlsHelper.SetRowCell(sheet, rownum, column++, "1天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "2天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "3天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "4天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "5天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "6天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "7天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "8天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "9天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "10天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "11天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "12天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "13天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "14天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "15天", cellStyleDayCenter);
            XlsHelper.SetRowCell(sheet, rownum, column++, "累计", cellStyleDayCenter);

        }

        private void OutputGapColunmHead(string orderType, ISheet asnSheet, int rownum)
        {
            int colnum = 1;

            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "送货单号", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "状态", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "物料", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "参考物料号", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "发运时间", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "预计送达", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "创建人", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "单位", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "单包装", cellStyleDet);
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            {
                XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "区域", cellStyleDet);
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
            {
                XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "区域", cellStyleDet);
            }
            else
            {
                XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "目的区域", cellStyleDet);
            }
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "发货地址", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "收货地址", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "送货道口", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "批号", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "条码", cellStyleDet);

            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "发货数量", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "收货数量", cellStyleDet);
            XlsHelper.SetRowCell(asnSheet, rownum, colnum++, "差异", cellStyleDet);

        }

        #endregion

        #region 未确认ASN

        protected bool ProcessSOASN(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessASN(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION, workbook, startDate, endDate);
        }

        protected bool ProcessPOASN(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessASN(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT, workbook, startDate, endDate);
        }

        protected bool ProcessTOASN(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            return ProcessASN(BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER, workbook, startDate, endDate);
        }

        private bool ProcessASN(string orderType, IWorkbook workbook, IList<IpMstr> ipMstrList, IList<IpDet> ipDetList, DateTime endDate)
        {
            if (ipMstrList == null || ipMstrList.Count() == 0) return false;
            string type = string.Empty;
            if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
            {
                type = "超期未确认回单";
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PROCUREMENT)
            {
                type = "逾期采购送货单";
            }
            else if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_TRANSFER)
            {
                type = "逾期移库送货单";
            }

            ProcessASN(orderType, type, workbook, ipMstrList, ipDetList, endDate);

            return true;
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessASN(string orderType, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                #region 获取订单头数据
                StringBuilder ipSql = new StringBuilder();
                ipSql.Append(@"select i.IpNo,i.DockDesc,i.OrderType, ");//,c1.Desc1 Status
                ipSql.Append(@"i.CreateDate,i.DepartTime,i.ArriveTime, ");
                ipSql.Append(@"u.USR_FirstName+u.USR_LastName+'['+u.USR_Code+']' CreateUser, ");
                ipSql.Append(@"p1.Name+'['+p1.Code+']' PartyFrom,p2.Name+'['+p2.Code+']' PartyTo, ");
                ipSql.Append(@"pa1.Address+'['+pa1.Code+']' ShipFrom, pa2.Address+'['+pa2.Code+']' ShipTo, ");
                ipSql.Append(@"t.WaybillNo,t.Carrier,t.CarrierDesc ");
                ipSql.Append(@"from IpMstr i  ");
                ipSql.Append(@"join Party p1 (nolock) on p1.Code=i.PartyFrom  ");
                ipSql.Append(@"join Party p2 (nolock) on p2.Code=i.PartyTo ");
                ipSql.Append(@"join PartyAddr pa1 (nolock) on i.ShipFrom=pa1.Code and pa1.AddrType= 'ShipAddr' ");
                ipSql.Append(@"join PartyAddr pa2 (nolock) on i.ShipTo=pa2.Code and pa2.AddrType= 'ShipAddr' ");
                //ipSql.Append(@"join CodeMstr c1 (nolock) on i.Status=c1.CodeValue and c1.Code = '" + BusinessConstants.CODE_MASTER_STATUS + "' ");
                ipSql.Append(@"join ACC_User u (nolock) on u.USR_Code=i.CreateUser ");
                ipSql.Append(@"left join TMS_WaybillMstr t (nolock) on t.WaybillNo = i.WaybillNo ");
                ipSql.Append(@"where i.OrderType='" + orderType + "'  ");
                ipSql.Append(@"and i.Status ='" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE + "' ");
                ipSql.Append(@"and i.Type='" + BusinessConstants.CODE_MASTER_INPROCESS_LOCATION_TYPE_VALUE_NORMAL + "'  ");
                ipSql.Append(@"and i.ArriveTime <'" + endDate.Value + "'  ");
                ipSql.Append(@"order by ");
                if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    ipSql.Append(@"i.PartyTo ");
                }
                else
                {
                    ipSql.Append(@"i.PartyFrom ");
                }
                ipSql.Append(@" ASC,i.ArriveTime ASC,i.IpNo ASC ");

                DataSet ipDS = sqlHelperMgrE.GetDatasetBySql(ipSql.ToString());
                List<IpMstr> ipList = IListHelper.DataTableToList<IpMstr>(ipDS.Tables[0]);

                #endregion

                if (ipList != null && ipList.Count > 0)
                {
                    #region 获取明细数据

                    StringBuilder ipDetSql = new StringBuilder();
                    ipDetSql.Append(@"select d.IpNo,i.Desc1+'['+ i.Code+']' Item,d.RefItemCode, ");
                    ipDetSql.Append(@"       d.HuId,d.LotNo,d.Qty, isnull(d.Uom,od.Uom) Uom,od.UC UC  ");//d.CustomerItemCode,
                    ipDetSql.Append(@"from IpMstr ip ");
                    ipDetSql.Append(@"join IpDet d (nolock) on ip.IpNo = d.IpNo and ip.OrderType='" + orderType + "' ");
                    ipDetSql.Append(@"join OrderLocTrans olt (nolock) on d.OrderLocTransId =olt.Id ");
                    ipDetSql.Append(@"join OrderDet od (nolock) on olt.OrderDetId=od.Id  ");
                    ipDetSql.Append(@"join Item i (nolock) on olt.Item=i.Code ");

                    ipDetSql.Append(@"where ");
                    ipDetSql.Append(@"    ip.Status ='" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE + "' ");
                    ipDetSql.Append(@"and ip.Type='" + BusinessConstants.CODE_MASTER_INPROCESS_LOCATION_TYPE_VALUE_NORMAL + "'  ");
                    ipDetSql.Append(@"and ip.ArriveTime <'" + endDate.Value + "'  ");
                    ipDetSql.Append(@"order by ");
                    if (orderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                    {
                        ipDetSql.Append(@"ip.PartyTo ");
                    }
                    else
                    {
                        ipDetSql.Append(@"ip.PartyFrom ");
                    }
                    ipDetSql.Append(@"ASC,ip.ArriveTime ASC,ip.IpNo ASC,d.Id ASC");
                    DataSet ipDetDS = sqlHelperMgrE.GetDatasetBySql(ipDetSql.ToString());
                    List<IpDet> ipDetList = IListHelper.DataTableToList<IpDet>(ipDetDS.Tables[0]);

                    #endregion

                    return ProcessASN(orderType, workbook, ipList, ipDetList, endDate.Value);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        private bool ProcessASN(string orderType, string type, IWorkbook workbook, IList<IpMstr> ipList, IList<IpDet> ipDetList, DateTime endDate)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet(type);

                for (int i = 0; i <= 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }
                //客户	送货单号	预计送达        发运时间	区域	发货地址	收货地址	送货道口	创建人   运单号     经销商代码   经销商名称
                //                      物料	参考物料号	单位	单包装	        批号	    条码	  数量

                sheet.SetColumnWidth(0, 28 * 256);
                sheet.SetColumnWidth(1, 15 * 256);//送货单号
                sheet.SetColumnWidth(2, 25 * 256);//发运时间//物料
                sheet.SetColumnWidth(3, 15 * 256);//预计送达//参考物料号
                sheet.SetColumnWidth(4, 20 * 256);//区域//单位
                sheet.SetColumnWidth(5, 28 * 256);//发货地址//单包装
                sheet.SetColumnWidth(6, 28 * 256);//收货地址//数量
                sheet.SetColumnWidth(7, 15 * 256);//送货道口//批号
                sheet.SetColumnWidth(8, 10 * 256);//创建人//条码
                sheet.SetColumnWidth(9, 15 * 256);//运单号
                sheet.SetColumnWidth(10, 10 * 256);//经销商代码
                //sheet.SetColumnWidth(11, 18 * 256);//经销商名称
                sheet.SetColumnWidth(16, 12 * 256);//15天以上

                int rownum = 0;
                int column = 0;

                #endregion

                column = 0;

                ICellStyle cellStyle3 = workbook.CreateCellStyle();
                IFont font3 = workbook.CreateFont();
                font3.Boldweight = (short)FontBoldWeight.BOLD;
                font3.FontHeightInPoints = (short)11;
                font3.Color = HSSFColor.BLUE_GREY.index;
                cellStyle3.SetFont(font3);

                XlsHelper.SetRowCell(sheet, rownum, column++, type + "单：", cellStyle3);
                XlsHelper.SetRowCell(sheet, rownum, column++, ipList.Count() + " 张", cellStyle3);

                XlsHelper.SetRowCell(sheet, rownum, column++, type + "明细：", cellStyle3);
                int detCount = ipDetList.Where(d => ipList.Select(m => m.IpNo).Contains(d.IpNo)).Count();
                XlsHelper.SetRowCell(sheet, rownum, column++, detCount + " 笔", cellStyle3);

                rownum++;
                rownum++;

                #region 输出数据
                DateTime? arriveTime = null;
                string party = string.Empty;
                IEnumerable<IpMstr> ipSubList = null;
                IEnumerable<IpDet> ipDetSubList = null;
                int partyASNNum = 0;
                //int partyNum = 0;
                for (int i = 0; i < ipList.Count(); i++)
                {
                    var ipMstr = ipList[i];

                    if (i == 0 || ipMstr.Party != ipList[i - 1].Party)
                    {
                        if (i == 0)
                        {
                            // 输出天数表头
                            OutputParty15(sheet, orderType, rownum);
                            rownum++;
                        }
                        //供应商、来源区域、客户的合计
                        ipSubList = ipList.Where(d => d.Party == ipMstr.Party);
                        ipDetSubList = ipDetList.Where(d => ipSubList.Select(m => m.IpNo).Contains(d.IpNo));
                        column = 0;
                        XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.Party, cellStyleTitle);

                        #region 天统计
                        //1-15天

                        var day1Orders = ipSubList.Where(o => o.ArriveTime <= endDate && endDate < o.ArriveTime.AddDays(2)).Select(m => m.IpNo);
                        if (day1Orders.Count() > 0)
                        {
                            decimal day1 = ipDetSubList.Where(d => day1Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day1.ToString("0.########") + "/" + day1Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day2Orders = ipSubList.Where(o => o.ArriveTime.AddDays(2) <= endDate && endDate < o.ArriveTime.AddDays(3)).Select(m => m.IpNo);
                        if (day2Orders.Count() > 0)
                        {
                            decimal day2 = ipDetSubList.Where(d => day2Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day2.ToString("0.########") + "/" + day2Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day3Orders = ipSubList.Where(o => o.ArriveTime.AddDays(3) <= endDate && endDate < o.ArriveTime.AddDays(4)).Select(m => m.IpNo);
                        if (day3Orders.Count() > 0)
                        {
                            decimal day3 = ipDetSubList.Where(d => day3Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day3.ToString("0.########") + "/" + day3Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day4Orders = ipSubList.Where(o => o.ArriveTime.AddDays(4) <= endDate && endDate < o.ArriveTime.AddDays(5)).Select(m => m.IpNo);
                        if (day4Orders.Count() > 0)
                        {
                            decimal day4 = ipDetSubList.Where(d => day4Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day4.ToString("0.########") + "/" + day4Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day5Orders = ipSubList.Where(o => o.ArriveTime.AddDays(5) <= endDate && endDate < o.ArriveTime.AddDays(6)).Select(m => m.IpNo);
                        if (day5Orders.Count() > 0)
                        {
                            decimal day5 = ipDetSubList.Where(d => day5Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day5.ToString("0.########") + "/" + day5Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day6Orders = ipSubList.Where(o => o.ArriveTime.AddDays(6) <= endDate && endDate < o.ArriveTime.AddDays(7)).Select(m => m.IpNo);
                        if (day6Orders.Count() > 0)
                        {
                            decimal day6 = ipDetSubList.Where(d => day6Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day6.ToString("0.########") + "/" + day6Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day7Orders = ipSubList.Where(o => o.ArriveTime.AddDays(7) <= endDate && endDate < o.ArriveTime.AddDays(8)).Select(m => m.IpNo);
                        if (day7Orders.Count() > 0)
                        {
                            decimal day7 = ipDetSubList.Where(d => day7Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day7.ToString("0.########") + "/" + day7Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day8Orders = ipSubList.Where(o => o.ArriveTime.AddDays(8) <= endDate && endDate < o.ArriveTime.AddDays(9)).Select(m => m.IpNo);
                        if (day8Orders.Count() > 0)
                        {
                            decimal day8 = ipDetSubList.Where(d => day8Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day8.ToString("0.########") + "/" + day8Orders.Count(), cellStyleCenter);
                        }
                        column++;


                        var day9Orders = ipSubList.Where(o => o.ArriveTime.AddDays(9) <= endDate && endDate < o.ArriveTime.AddDays(10)).Select(m => m.IpNo);
                        if (day9Orders.Count() > 0)
                        {
                            decimal day9 = ipDetSubList.Where(d => day9Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day9.ToString("0.########") + "/" + day9Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day10Orders = ipSubList.Where(o => o.ArriveTime.AddDays(10) <= endDate && endDate < o.ArriveTime.AddDays(11)).Select(m => m.IpNo);
                        if (day10Orders.Count() > 0)
                        {
                            decimal day10 = ipDetSubList.Where(d => day10Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day10.ToString("0.########") + "/" + day10Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day11Orders = ipSubList.Where(o => o.ArriveTime.AddDays(11) <= endDate && endDate < o.ArriveTime.AddDays(12)).Select(m => m.IpNo);
                        if (day11Orders.Count() > 0)
                        {
                            decimal day11 = ipDetSubList.Where(d => day11Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day11.ToString("0.########") + "/" + day11Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day12Orders = ipSubList.Where(o => o.ArriveTime.AddDays(12) <= endDate && endDate < o.ArriveTime.AddDays(13)).Select(m => m.IpNo);
                        if (day12Orders.Count() > 0)
                        {
                            decimal day12 = ipDetSubList.Where(d => day12Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day12.ToString("0.########") + "/" + day12Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day13Orders = ipSubList.Where(o => o.ArriveTime.AddDays(13) <= endDate && endDate < o.ArriveTime.AddDays(14)).Select(m => m.IpNo);
                        if (day13Orders.Count() > 0)
                        {
                            decimal day13 = ipDetSubList.Where(d => day13Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day13.ToString("0.########") + "/" + day13Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day14Orders = ipSubList.Where(o => o.ArriveTime.AddDays(14) <= endDate && endDate < o.ArriveTime.AddDays(15)).Select(m => m.IpNo);
                        if (day14Orders.Count() > 0)
                        {
                            decimal day14 = ipDetSubList.Where(d => day14Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day14.ToString("0.########") + "/" + day14Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        var day15Orders = ipSubList.Where(o => o.ArriveTime.AddDays(15) <= endDate && endDate < o.ArriveTime.AddDays(16)).Select(m => m.IpNo);
                        if (day15Orders.Count() > 0)
                        {
                            decimal day15 = ipDetSubList.Where(d => day15Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day15.ToString("0.########") + "/" + day15Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        //var day16Orders = ipSubList.Where(o => endDate.AddDays(-16) >= o.ArriveTime).Select(m => m.IpNo);
                        var day16Orders = ipSubList.Select(m => m.IpNo);
                        if (day16Orders.Count() > 0)
                        {
                            decimal day16 = ipDetSubList.Where(d => day16Orders.Contains(d.IpNo)).Sum(d => d.Qty);
                            XlsHelper.SetRowCell(sheet, rownum, column, day16.ToString("0.########") + "/" + day16Orders.Count(), cellStyleCenter);
                        }
                        column++;

                        #endregion

                        rownum++;
                        //输出列头
                        OutputASNColunmHead(sheet, rownum++);
                        //partyNum++;
                        partyASNNum = 0;
                    }
                    column = 1;

                    //客户	送货单号	预计送达     发运时间	区域	发货地址	收货地址	送货道口	创建人     运单号   运单号  经销商代码   经销商名称
                    //                      物料	参考物料号	单位	单包装	        数量        批号	  条码	
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.IpNo);

                    if (!arriveTime.HasValue || arriveTime.Value.ToString("yyyy-MM-dd").CompareTo(ipMstr.ArriveTime.ToString("yyyy-MM-dd")) != 0
                           || partyASNNum == 0)
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.ArriveTime.ToString("yyyy-MM-dd HH:mm"), cellStyleColor);
                    }
                    else
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.ArriveTime.ToString("yyyy-MM-dd HH:mm"));
                    }
                    arriveTime = ipMstr.ArriveTime;
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.DepartTime.ToString("yyyy-MM-dd HH:mm"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.RefParty);
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.ShipFrom);
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.ShipTo);

                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.DockDesc);
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.CreateUser);
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.WaybillNo);
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.Carrier);
                    XlsHelper.SetRowCell(sheet, rownum, column++, ipMstr.CarrierDesc);

                    #region 发货明细
                    var ipDetTList = ipDetSubList.Where(d => d.IpNo == ipMstr.IpNo).ToList<IpDet>();
                    if (ipDetTList != null && ipDetTList.Count > 0)
                    {
                        rownum++;
                        OutputASNDetColunmHead(sheet, rownum);

                        foreach (var ipDet in ipDetTList)
                        {
                            column = 2;
                            rownum++;
                            //客户	送货单号	预计送达     发运时间	区域	发货地址	收货地址	送货道口	创建人   运单号  经销商代码   经销商名称
                            //                      物料	参考物料号	单位	单包装	        数量        批号	  条码	  

                            XlsHelper.SetRowCell(sheet, rownum, column++, ipDet.Item);
                            XlsHelper.SetRowCell(sheet, rownum, column++, ipDet.RefItemCode);
                            XlsHelper.SetRowCell(sheet, rownum, column++, ipDet.Uom);
                            if (ipDet.UC != 0)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column, ipDet.UC.ToString("0.########"));
                            }
                            column++;
                            XlsHelper.SetRowCell(sheet, rownum, column++, ipDet.Qty.ToString("0.########"));
                            XlsHelper.SetRowCell(sheet, rownum, column++, ipDet.LotNo);
                            XlsHelper.SetRowCell(sheet, rownum, column++, ipDet.HuId);
                        }

                        sheet.GroupRow(rownum - ipDetTList.Count, rownum);
                        sheet.SetRowGroupCollapsed(rownum, true);
                    }
                    #endregion

                    if (partyASNNum == ipSubList.Count() - 1)
                    {
                        rownum++;
                        sheet.GroupRow(rownum - ipSubList.Count() * 2 - ipDetSubList.Count() - 1, rownum);
                        sheet.SetRowGroupCollapsed(rownum, true);
                    }
                    partyASNNum++;
                    rownum++;
                }
                //  sheet.GroupRow(2, rownum);
                //  sheet.SetRowGroupCollapsed(rownum, true);
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

        private void OutputASNColunmHead(ISheet sheet, int rownum)
        {
            int colnum = 1;
            //客户	送货单号	预计送达      发运时间	区域	发货地址	收货地址	送货道口	创建人    运单号 承运商代码       承运商名字
            //                      物料	参考物料号	单位	单包装	        数量        批号	  条码	

            XlsHelper.SetRowCell(sheet, rownum, colnum++, "送货单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "预计送达", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "发运时间", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "区域", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "发货地址", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "收货地址", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "送货道口", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "创建人", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "运单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "承运商代码", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "承运商名字", headStyle);

        }

        private void OutputASNDetColunmHead(ISheet sheet, int rownum)
        {
            int colnum = 2;
            //客户	送货单号	预计送达      发运时间	区域	发货地址	收货地址	送货道口	创建人
            //                      物料	参考物料号	单位	单包装	        数量        批号	  条码	  

            XlsHelper.SetRowCell(sheet, rownum, colnum++, "物料", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "参考物料号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "单位", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "单包装", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "数量", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "批号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "条码", headStyle);

        }

        #endregion

        protected virtual bool ProcessBill(string type, IWorkbook workbook, IList<Bill> billList, IList<BillDet> billDetList)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet(type);

                for (int i = 0; i <= 6; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }
                sheet.SetColumnWidth(0, 15 * 256);//账单号
                sheet.SetColumnWidth(1, 25 * 256);//客户//物料
                sheet.SetColumnWidth(2, 25 * 256);//开票地址//物料描述
                sheet.SetColumnWidth(3, 25 * 256);//金额//单位
                sheet.SetColumnWidth(4, 12 * 256);//开票人//单包装
                sheet.SetColumnWidth(5, 12 * 256);//开票时间//开票数量
                sheet.SetColumnWidth(6, 16 * 256);//外部单号//单价
                //sheet.SetColumnWidth(7, 12 * 256);//参考单号//折扣

                int rownum = 0;
                int column = 0;

                #endregion

                column = 0;

                ICellStyle cellStyle3 = workbook.CreateCellStyle();
                IFont font3 = workbook.CreateFont();
                font3.Boldweight = (short)FontBoldWeight.BOLD;
                font3.FontHeightInPoints = (short)11;
                font3.Color = HSSFColor.BLUE_GREY.index;
                cellStyle3.SetFont(font3);

                XlsHelper.SetRowCell(sheet, rownum, column++, "账单：", cellStyle3);
                XlsHelper.SetRowCell(sheet, rownum, column++, billList.Count + " 张", cellStyle3);
                XlsHelper.SetRowCell(sheet, rownum, column++, "账单明细：", cellStyle3);
                XlsHelper.SetRowCell(sheet, rownum, column++, billDetList.Count + " 笔", cellStyle3);
                var amount = billDetList.Sum(od => od.Amount) - billList.Sum(b => b.Discount);

                XlsHelper.SetRowCell(sheet, rownum, column++, "金额：", cellStyle3);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount.ToString("N2") + " 元", cellStyle3);

                rownum++;
                rownum++;

                #region 列头
                column = 0;
                XlsHelper.SetRowCell(sheet, rownum, column++, "账单号", cellStyleDet);
                if (billList[0].BillType == BusinessConstants.BILL_TRANS_TYPE_PO)
                {
                    XlsHelper.SetRowCell(sheet, rownum, column++, "供应商", cellStyleDet);
                }
                else
                {
                    XlsHelper.SetRowCell(sheet, rownum, column++, "客户", cellStyleDet);
                }

                XlsHelper.SetRowCell(sheet, rownum, column++, "开票地址", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "金额", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "开票人", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "开票日期", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "外部单号", cellStyleDet);
                XlsHelper.SetRowCell(sheet, rownum, column++, "参考单号", cellStyleDet);
                rownum++;
                #endregion

                #region 输出数据
                string party = string.Empty;
                for (int i = 0; i < billList.Count(); i++)
                {
                    var bill = billList[i];
                    column = 0;
                    XlsHelper.SetRowCell(sheet, rownum, column++, bill.BillNo);

                    if (string.IsNullOrEmpty(party) || party != bill.Party)
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, bill.Party, cellStyleColor);
                    }
                    else
                    {
                        XlsHelper.SetRowCell(sheet, rownum, column++, bill.Party);
                    }
                    party = bill.Party;
                    XlsHelper.SetRowCell(sheet, rownum, column++, bill.BillAddr);

                    #region 发货明细
                    var billDetTList = billDetList.Where(d => d.BillNo == bill.BillNo).ToList<BillDet>();

                    if (billDetTList != null && billDetTList.Count > 0)
                    {
                        var billAmount = billDetTList.Sum(od => od.Amount);
                        if (billAmount != 0)
                        {
                            XlsHelper.SetRowCell(sheet, rownum, column, billAmount.ToString("N2"));
                        }
                        column++;
                        XlsHelper.SetRowCell(sheet, rownum, column++, bill.CreateUser);
                        XlsHelper.SetRowCell(sheet, rownum, column++, bill.CreateDate.ToString("yyyy-MM-dd HH:mm"));
                        XlsHelper.SetRowCell(sheet, rownum, column++, bill.ExtBillNo);
                        XlsHelper.SetRowCell(sheet, rownum, column++, bill.RefBillNo);

                        rownum++;
                        int colnum = 1;

                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "物料", headStyle);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "物料描述", headStyle);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "单位", headStyle);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "单包装", headStyle);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "开票数量", headStyle);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "单价", headStyle);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "折扣", headStyle);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, "金额", headStyle);

                        foreach (var billDet in billDetTList)
                        {
                            column = 1;
                            rownum++;

                            XlsHelper.SetRowCell(sheet, rownum, column++, billDet.Item);
                            XlsHelper.SetRowCell(sheet, rownum, column++, billDet.ItemDesc);
                            XlsHelper.SetRowCell(sheet, rownum, column++, billDet.Uom);
                            if (billDet.UC != 0)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column, billDet.UC.ToString("0.########"));
                            }
                            column++;
                            XlsHelper.SetRowCell(sheet, rownum, column++, billDet.BilledQty.ToString("0.########"));
                            XlsHelper.SetRowCell(sheet, rownum, column++, billDet.UnitPrice.ToString("0.########"));
                            XlsHelper.SetRowCell(sheet, rownum, column++, billDet.Discount.ToString("N2"));
                            XlsHelper.SetRowCell(sheet, rownum, column++, billDet.Amount.ToString("N2"));
                        }
                        sheet.GroupRow(rownum - billDetTList.Count, rownum);
                        sheet.SetRowGroupCollapsed(rownum, true);
                    }
                    #endregion

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

        protected virtual bool ProcessTimelyRate(string key, IWorkbook workbook, IList<TimelyRate> timelyRateList)
        {
            try
            {
                if (timelyRateList != null && timelyRateList.Count > 0)
                {

                    #region 初始化Sheet

                    ISheet sheet = workbook.CreateSheet(key);

                    for (int i = 0; i <= 6; i++)
                    {
                        sheet.AutoSizeColumn(i);
                        sheet.SetDefaultColumnStyle(i, cellStyle);
                    }
                    sheet.SetColumnWidth(0, 15 * 256);//年月
                    sheet.SetColumnWidth(1, 17 * 256);//代码
                    sheet.SetColumnWidth(2, 25 * 256);//名称
                    sheet.SetColumnWidth(3, 15 * 256);//单数                    
                    sheet.SetColumnWidth(4, 15 * 256);//不及时单数
                    sheet.SetColumnWidth(5, 15 * 256);//及时单数
                    sheet.SetColumnWidth(6, 15 * 256);//及时率
                    //sheet.SetColumnWidth(7, 15 * 256);//完成率
                    int rownum = 0;
                    int colnum = 0;

                    #endregion

                    #region 输出列头
                    colnum = 0;
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "年月", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "代码", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "名称", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "单数", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "不及时单数", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "及时单数", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "及时率", headStyle);
                    XlsHelper.SetRowCell(sheet, rownum, colnum++, "完成率", headStyle);
                    #endregion

                    string ym = string.Empty;
                    foreach (var timelyRate in timelyRateList)
                    {
                        rownum++;
                        colnum = 0;
                        if (string.IsNullOrEmpty(ym) || ym != timelyRate.年月)
                        {
                            XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.年月, cellStyleColor);
                        }
                        else
                        {
                            XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.年月);
                        }
                        ym = timelyRate.年月;

                        XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.代码);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.名称);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.单数);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.不及时单数);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.及时单数);
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.及时率, "P");
                        XlsHelper.SetRowCell(sheet, rownum, colnum++, timelyRate.完成率, "P");
                    }

                    sheet.ForceFormulaRecalculation = true;
                    sheet.CreateFreezePane(0, 1, 0, 1);
                }

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        #region 账龄
        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessSOBillAging(IWorkbook workbook)
        {
            return ProcessBillAging("账龄", BusinessConstants.BILL_TRANS_TYPE_SO, workbook);
        }
        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessPOBillAging(IWorkbook workbook)
        {
            return ProcessBillAging("账龄", BusinessConstants.BILL_TRANS_TYPE_PO, workbook);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessBillAging(string key, string transType, IWorkbook workbook)
        {
            return ProcessBillAging(key, true, transType, workbook);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessBillAging(string key, bool isNewSystem, string transType, IWorkbook workbook)
        {
            try
            {
                #region 获取头数据
                StringBuilder billSql = new StringBuilder();
                billSql.Append(@" select P.Name +'['+p.Code+']' Party,TransType,b.BillAddr, ");//, a.Address BillAddrDesc
                billSql.Append(@"        i.Code Item,i.Desc1 ItemDesc, b.Uom, b.UC,");
                billSql.Append(@"        Qty10, i.NumField3  * Qty10 Amount10,Qty11, i.NumField3  * Qty11 Amount11,");
                //billSql.Append(@"        Qty1, i.NumField3  * Qty1 Amount1, ");
                billSql.Append(@"        Qty2, i.NumField3  * Qty2 Amount2, ");
                billSql.Append(@"        Qty3, i.NumField3  * Qty3 Amount3, Qty4, i.NumField3  * Qty4 Amount4, ");
                billSql.Append(@"        Qty5, i.NumField3  * Qty5 Amount5, Qty6, i.NumField3  * Qty6 Amount6, ");
                billSql.Append(@"        Qty7, i.NumField3  * Qty7 Amount7, Qty8, i.NumField3  * Qty8 Amount8, ");
                billSql.Append(@"        Qty9, i.NumField3  * Qty9 Amount9, i.NumField3  Price ");
                billSql.Append(@" from BillAgingView b (nolock) ");
                billSql.Append(@" join Item i  (nolock) on b.Item =i.Code ");
                billSql.Append(@" join PartyAddr a  (nolock) on b.BillAddr = a.Code and a.AddrType='BillAddr' ");
                billSql.Append(@" join Party p  (nolock) on a.PartyCode = p.Code ");
                billSql.Append(@" where TransType='" + transType + "' ");
                billSql.Append(@" order by p.Code ASC,i.Code ASC,b.Uom ASC,b.UC ASC ");

                DataSet orderMstrDS = sqlHelperMgrE.GetDatasetBySql(0, billSql.ToString());
                var billList = IListHelper.DataTableToList<BillAging>(orderMstrDS.Tables[0]);

                #endregion

                if (billList != null && billList.Count > 0)
                {
                    #region 获取明细数据
                    StringBuilder detSql = new StringBuilder();
                    detSql.Append(@"select OrderNo, RecNo, ExtRecNo, TransType, BillAddr,");
                    detSql.Append(@"       i.Code Item,i.Desc1 ItemDesc, a.Uom, a.UC, EffDate, Qty, ");
                    detSql.Append(@"       i.NumField3  Price,i.NumField3  * Qty Amount  ");
                    detSql.Append(@"from ActBillView a  (nolock) ");
                    detSql.Append(@"join Item i  (nolock) on a.Item=i.Code ");
                    detSql.Append(@"where TransType='" + transType + "' ");
                    detSql.Append(@"order by a.BillAddr ASC,i.Code ASC,a.Uom ASC,a.UC ASC,a.EffDate ASC ");
                    DataSet orderDetDS = sqlHelperMgrE.GetDatasetBySql(0, detSql.ToString());
                    var actBillList = IListHelper.DataTableToList<ActBill>(orderDetDS.Tables[0]);
                    #endregion

                    return ProcessBillAging(key, transType, workbook, billList, actBillList);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        private bool ProcessBillAging(string key, string transType, IWorkbook workbook, IList<BillAging> billList, IList<ActBill> actBillList)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet(key);

                for (int i = 0; i <= 14; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }
                sheet.SetColumnWidth(0, 28 * 256);//客户、供应商
                sheet.SetColumnWidth(1, 25 * 256);//物料
                sheet.SetColumnWidth(2, 15 * 256);//单位
                sheet.SetColumnWidth(3, 20 * 256);//单包装
                sheet.SetColumnWidth(4, 12 * 256);//小于5
                sheet.SetColumnWidth(5, 12 * 256);//5至30
                sheet.SetColumnWidth(6, 12 * 256);//30至60
                sheet.SetColumnWidth(7, 12 * 256);//60至90
                sheet.SetColumnWidth(8, 12 * 256);//90至120
                sheet.SetColumnWidth(9, 12 * 256);//120至150
                sheet.SetColumnWidth(10, 12 * 256);//150至180
                sheet.SetColumnWidth(11, 12 * 256);//180至210
                sheet.SetColumnWidth(12, 12 * 256);//210至360
                sheet.SetColumnWidth(13, 12 * 256);//大于360
                sheet.SetColumnWidth(14, 12 * 256);//合计
                int rownum = 0;
                int column = 0;

                #endregion

                #region 输出数据

                var amount = billList.Sum(b => b.Amount);
                var amount5T = billList.Sum(b => b.Amount5T);
                var amount30 = billList.Sum(b => b.Amount30);
                var amount60 = billList.Sum(b => b.Amount60);
                var amount90 = billList.Sum(b => b.Amount90);
                var qty = billList.Sum(b => b.Qty);
                var itemCount = actBillList.Select(a => a.Item).Distinct().Count();
                XlsHelper.SetRowCell(sheet, rownum, column++, "总金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount.HasValue ? amount.Value.ToString("0.##元") : string.Empty, headStyle2);
                column++;
                column++;
                XlsHelper.SetRowCell(sheet, rownum, column++, "总数量：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, qty.ToString("0.##"), headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "物料种类：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, itemCount.ToString(), headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于5天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount5T.HasValue ? amount5T.Value.ToString("0.##元") : string.Empty, headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于30天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount30.HasValue ? amount30.Value.ToString("0.##元") : string.Empty, headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于60天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount60.HasValue ? amount60.Value.ToString("0.##元") : string.Empty, headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于90天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount90.HasValue ? amount90.Value.ToString("0.##元") : string.Empty, headStyle2);
                rownum++;

                OutputParty360(sheet, transType, rownum);
                rownum++;
                for (int i = 0; i < billList.Count(); i++)
                {
                    var bill = billList[i];

                    column = 0;
                    XlsHelper.SetRowCell(sheet, rownum, column++, bill.Party);
                    //XlsHelper.SetRowCell(sheet, rownum, column++, bill.BillAddrDesc);
                    XlsHelper.SetRowCell(sheet, rownum, column++, bill.ItemDesc + "[" + bill.Item + "]");
                    XlsHelper.SetRowCell(sheet, rownum, column++, bill.Uom);
                    XlsHelper.SetRowCell(sheet, rownum, column++, bill.UC != 0 ? bill.UC.ToString("0.########") : string.Empty);
                    //XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount1.HasValue && bill.Amount1.Value != 0 ? bill.Amount1.Value.ToString("0.########") : string.Empty) + (bill.Qty1 != 0 ? "/" + bill.Qty1.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount10.HasValue && bill.Amount10.Value != 0 ? bill.Amount10.Value.ToString("0.########元") : string.Empty) + (bill.Qty10 != 0 ? "/" + bill.Qty10.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount11.HasValue && bill.Amount11.Value != 0 ? bill.Amount11.Value.ToString("0.########元") : string.Empty) + (bill.Qty11 != 0 ? "/" + bill.Qty11.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount2.HasValue && bill.Amount2.Value != 0 ? bill.Amount2.Value.ToString("0.########元") : string.Empty) + (bill.Qty2 != 0 ? "/" + bill.Qty2.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount3.HasValue && bill.Amount3.Value != 0 ? bill.Amount3.Value.ToString("0.########元") : string.Empty) + (bill.Qty3 != 0 ? "/" + bill.Qty3.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount4.HasValue && bill.Amount4.Value != 0 ? bill.Amount4.Value.ToString("0.########元") : string.Empty) + (bill.Qty4 != 0 ? "/" + bill.Qty4.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount5.HasValue && bill.Amount5.Value != 0 ? bill.Amount5.Value.ToString("0.########元") : string.Empty) + (bill.Qty5 != 0 ? "/" + bill.Qty5.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount6.HasValue && bill.Amount6.Value != 0 ? bill.Amount6.Value.ToString("0.########元") : string.Empty) + (bill.Qty6 != 0 ? "/" + bill.Qty6.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount7.HasValue && bill.Amount7.Value != 0 ? bill.Amount7.Value.ToString("0.########元") : string.Empty) + (bill.Qty7 != 0 ? "/" + bill.Qty7.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount8.HasValue && bill.Amount8.Value != 0 ? bill.Amount8.Value.ToString("0.########元") : string.Empty) + (bill.Qty8 != 0 ? "/" + bill.Qty8.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount9.HasValue && bill.Amount9.Value != 0 ? bill.Amount9.Value.ToString("0.########元") : string.Empty) + (bill.Qty9 != 0 ? "/" + bill.Qty9.ToString("0.########") : string.Empty));
                    XlsHelper.SetRowCell(sheet, rownum, column++, (bill.Amount.HasValue && bill.Amount.Value != 0 ? bill.Amount.Value.ToString("0.########元") : string.Empty) + (bill.Qty != 0 ? "/" + bill.Qty.ToString("0.########") : string.Empty));

                    #region 发货明细
                    var actBillTList = actBillList.Where(a => a.BillAddr == bill.BillAddr && a.Item == bill.Item && a.Uom == bill.Uom && a.UC == bill.UC);
                    if (actBillTList != null && actBillTList.Count() > 0)
                    {
                        rownum++;
                        //输出列头
                        OutputBillAgingColunmHead(transType, sheet, rownum++);

                        foreach (var actBill in actBillTList)
                        {
                            column = 1;
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.OrderNo);
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.RecNo);
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.ExtRecNo);
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.EffDate.ToString("yyyy-MM-dd"));
                            /*
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.ItemDesc + "[" + actBill.Item + "]");
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.Uom);
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.UC.ToString("0.########"));
                            if (actBill.UC != 0)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column, actBill.UC.ToString("0.########"));
                            }
                            column++;
                             */
                            XlsHelper.SetRowCell(sheet, rownum, column++, actBill.Qty.ToString("0.########"));
                            if (actBill.Price.HasValue)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column, actBill.Price.Value.ToString("0.########"));
                            }
                            column++;
                            if (actBill.Amount.HasValue)
                            {
                                XlsHelper.SetRowCell(sheet, rownum, column, actBill.Amount.Value.ToString("0.########"));
                            }
                            column++;
                            rownum++;
                        }
                        sheet.GroupRow(rownum - actBillTList.Count() - 1, rownum);
                        sheet.SetRowGroupCollapsed(rownum, true);
                    }

                    #endregion
                    rownum++;
                }
                sheet.GroupColumn(2, 3);
                sheet.SetColumnGroupCollapsed(3, true);

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

        private void OutputBillAgingColunmHead(string transType, ISheet sheet, int rownum)
        {
            int colnum = 1;
            //	订单号	收货单号	物料	物料描述	单位	单包装	数量	生效日期
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "订单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "收货单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "外部单号", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "生效日期", headStyle);
            //XlsHelper.SetRowCell(sheet, rownum, colnum++, "物料", headStyle);
            //XlsHelper.SetRowCell(sheet, rownum, colnum++, "单位", headStyle);
            //XlsHelper.SetRowCell(sheet, rownum, colnum++, "单包装", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "数量", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "单价", headStyle);
            XlsHelper.SetRowCell(sheet, rownum, colnum++, "金额", headStyle);
        }

        #endregion

        #region  生产异常
        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessWO(string dateTimeType, string type, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                #region 获取订单头数据
                StringBuilder orderMstrSql = new StringBuilder();
                orderMstrSql.Append(@" select o.OrderNo,o.RefOrderNo,o.ExtOrderNo,f.Desc1+'['+f.Code+']' Flow,o.StartTime,");
                orderMstrSql.Append(@"        o.WindowTime,c1.Desc1 Status,c3.Desc1 Priority,o.SubType,c2.Desc1 SubTypeDesc,p1.Name+'['+p1.Code+']' PartyFrom,");
                //orderMstrSql.Append(@"        p2.Name+'['+p2.Code+']' PartyTo,pa1.Address+'['+pa1.Code+']' ShipFrom, pa2.Address+'['+pa2.Code+']' ShipTo, ");
                orderMstrSql.Append(@"        l1.Name+'['+l1.Code+']' LocFrom,l2.Name+'['+l2.Code+']' LocTo, ");//o.DockDesc,c4.Desc1 BillSettleTerm,
                orderMstrSql.Append(@"        o.Shift,u1.USR_FirstName + u1.USR_LastName+'['+u1.USR_Code+']' CreateUser, ");//,c5.Desc1 Type,c.Name Currency,SettleTime
                orderMstrSql.Append(@"        u2.USR_FirstName + u2.USR_LastName+'['+u2.USR_Code+']' CancelUser, ");
                orderMstrSql.Append(@"        o.CancelDate,o.CreateDate ");
                orderMstrSql.Append(@" from OrderMstr o ");
                orderMstrSql.Append(@" join FlowMstr f (nolock) on o.Flow = f.Code and f.Type = '" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION + "' ");
                orderMstrSql.Append(@" join Party p1 (nolock) on o.PartyFrom=p1.Code ");
                //orderMstrSql.Append(@" join Party p2 (nolock) on o.PartyTo=p2.Code ");
                //orderMstrSql.Append(@" join PartyAddr pa1 (nolock) on o.ShipFrom=pa1.Code and pa1.AddrType= 'ShipAddr' ");
                //orderMstrSql.Append(@" join PartyAddr pa2 (nolock) on o.ShipTo=pa2.Code and pa2.AddrType= 'ShipAddr' ");

                orderMstrSql.Append(@" join CodeMstr c1 (nolock) on o.Status=c1.CodeValue and c1.Code = '" + BusinessConstants.CODE_MASTER_STATUS + "'");
                orderMstrSql.Append(@" join CodeMstr c2 (nolock) on o.SubType=c2.CodeValue and c2.Code = '" + BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE + "'");
                orderMstrSql.Append(@" join CodeMstr c3 (nolock) on o.Priority=c3.CodeValue and c3.Code = '" + BusinessConstants.CODE_MASTER_ORDER_PRIORITY + "'");
                //orderMstrSql.Append(@" join CodeMstr c5 (nolock) on o.Type=c5.CodeValue and c5.Code = '" + BusinessConstants.CODE_MASTER_ORDER_TYPE + "'");
                orderMstrSql.Append(@" join Location l1 (nolock) on o.LocFrom=l1.Code ");
                orderMstrSql.Append(@" join Location l2 (nolock) on o.LocTo=l2.Code ");
                orderMstrSql.Append(@" join ACC_User u1 (nolock) on u1.USR_Code = o.CreateUser ");
                orderMstrSql.Append(@" left join ACC_User u2 (nolock) on u2.USR_Code = o.CancelUser ");
                //orderMstrSql.Append(@" left join CodeMstr c4 (nolock) on o.BillSettleTerm=c4.CodeValue and c4.Code = '" + BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM + "'");
                //orderMstrSql.Append(@" left join Currency c (nolock) on o.Currency=c.Code ");
                orderMstrSql.Append(@" where o.Type = '" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION + "'  and o.Flow not in (select RefFlow from FlowMstr where type='" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_SUBCONCTRACTING + "') ");
                if (dateTimeType == BusinessConstants.DATETIME_TYPE_MONTH)
                {
                    orderMstrSql.Append(@" and ( ( (o.CompleteDate is null or o.CompleteDate > o.WindowTime ) ");
                    orderMstrSql.Append(@" and o.WindowTime >='" + startDate.Value + "' and o.WindowTime < '" + endDate.Value + "' ");
                    orderMstrSql.Append(@" and o.Status != '" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE + "' and o.Status != '" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CANCEL + "' ) ");
                }
                else
                {
                    orderMstrSql.Append(@" and ( (o.WindowTime < '" + endDate.Value + "' ");
                    orderMstrSql.Append(@" and o.Status in  ('" + BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT + "','" + BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS + "') ) ");
                }
                orderMstrSql.Append(@" or ( o.Status ='" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CANCEL + "' ");
                orderMstrSql.Append(@" and  DATEDIFF (minute, o.CreateDate,o.CancelDate)  > 60  ");
                orderMstrSql.Append(@" and o.CancelDate >='" + startDate.Value + "' and o.CancelDate < '" + endDate.Value + "' )) ");

                orderMstrSql.Append(@" order by c1.Desc1 ASC,c2.Desc1 ASC,o.PartyTo ASC,o.WindowTime ASC,o.OrderNo ASC ");
                DataSet orderMstrDS = sqlHelperMgrE.GetDatasetBySql(orderMstrSql.ToString());
                IList<OrderMstr> orderMstrList = IListHelper.DataTableToList<OrderMstr>(orderMstrDS.Tables[0]);

                #endregion

                if (orderMstrList != null && orderMstrList.Count > 0)
                {

                    #region 获取明细数据
                    StringBuilder orderDetSql = new StringBuilder();
                    orderDetSql.Append(@" select d.Id,d.OrderNo,i.Desc1+'['+i.Code+']' Item,d.RefItemCode,");
                    orderDetSql.Append(@"        d.Uom,d.UC,d.ReqQty,d.OrderQty,d.RejQty,d.RecQty,d.ScrapQty, ");
                    orderDetSql.Append(@"        l1.Name+'['+l1.Code+']' LocFrom,l2.Name+'['+l2.Code+']' LocTo ");
                    orderDetSql.Append(@" from OrderDet d ");
                    orderDetSql.Append(@" join OrderMstr o (nolock) on o.OrderNo = d.OrderNo ");
                    orderDetSql.Append(@" join Item i (nolock) on i.Code = d.Item ");
                    orderDetSql.Append(@" left join Location l1 (nolock) on o.LocFrom=l1.Code ");
                    orderDetSql.Append(@" left join Location l2 (nolock) on o.LocTo=l2.Code ");
                    orderDetSql.Append(@" where o.Type = '" + BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_PRODUCTION + "'  and o.Flow not in (select RefFlow from FlowMstr where type='" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_SUBCONCTRACTING + "') ");
                    if (dateTimeType == BusinessConstants.DATETIME_TYPE_MONTH)
                    {
                        orderDetSql.Append(@" and ( ( (o.CompleteDate is null or o.CompleteDate > o.WindowTime ) ");
                        orderDetSql.Append(@" and o.WindowTime >='" + startDate.Value + "' and o.WindowTime < '" + endDate.Value + "' ");
                        orderDetSql.Append(@" and o.Status != '" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE + "' and o.Status != '" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CANCEL + "' ) ");
                    }
                    else
                    {
                        orderDetSql.Append(@" and ( (o.WindowTime < '" + endDate.Value + "' ");
                        orderDetSql.Append(@" and o.Status in  ('" + BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT + "','" + BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS + "') ) ");
                    }
                    orderDetSql.Append(@" or ( o.Status ='" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CANCEL + "' ");
                    orderDetSql.Append(@" and  DATEDIFF (minute, o.CreateDate,o.CancelDate)  > 60  ");
                    orderDetSql.Append(@" and o.CancelDate >='" + startDate.Value + "' and o.CancelDate < '" + endDate.Value + "' )) ");
                    orderDetSql.Append(@" order by o.Status ASC,o.SubType ASC,o.PartyTo ASC,o.WindowTime ASC,o.OrderNo ASC,d.RecQty ASC, d.Seq ASC ");
                    DataSet orderDetDS = sqlHelperMgrE.GetDatasetBySql(orderDetSql.ToString());
                    IList<OrderDet> orderDetList = IListHelper.DataTableToList<OrderDet>(orderDetDS.Tables[0]);

                    #endregion

                    var noCloseOrderList = orderMstrList.Where(o => o.Status != "取消").ToList();
                    if (noCloseOrderList != null && noCloseOrderList.Count > 0)
                    {
                        return ProcessWO(SHEET_NAME_NO_CLOSE, type, workbook, noCloseOrderList, orderDetList);
                    }
                    var cancelOrderList = orderMstrList.Where(o => o.Status == "取消").ToList();
                    if (cancelOrderList != null && cancelOrderList.Count > 0)
                    {
                        return ProcessWO(SHEET_NAME_CANCEL, type, workbook, cancelOrderList, orderDetList);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        protected virtual bool ProcessWO(string sheetName, string type, IWorkbook workbook, IList<OrderMstr> orderMstrList, IList<OrderDet> orderDetList)
        {
            try
            {
                #region 初始化Sheet

                ISheet woSheet = workbook.CreateSheet(sheetName + type);

                for (int i = 0; i <= 16; i++)
                {
                    woSheet.AutoSizeColumn(i);
                    woSheet.SetDefaultColumnStyle(i, cellStyle);
                }

                woSheet.SetColumnWidth(0, 15 * 256);
                woSheet.SetColumnWidth(1, 15 * 256);
                woSheet.SetColumnWidth(2, 25 * 256);
                woSheet.SetColumnWidth(3, 10 * 256);
                woSheet.SetColumnWidth(4, 15 * 256);
                woSheet.SetColumnWidth(5, 10 * 256);
                woSheet.SetColumnWidth(6, 10 * 256);
                woSheet.SetColumnWidth(7, 7 * 256);
                woSheet.SetColumnWidth(8, 7 * 256);
                woSheet.SetColumnWidth(9, 15 * 256);
                woSheet.SetColumnWidth(10, 15 * 256);
                woSheet.SetColumnWidth(11, 15 * 256);
                woSheet.SetColumnWidth(12, 7 * 256);
                woSheet.SetColumnWidth(13, 15 * 256);
                woSheet.SetColumnWidth(14, 10 * 256);
                woSheet.SetColumnWidth(15, 10 * 256);
                woSheet.SetColumnWidth(16, 10 * 256);
                //woSheet.SetColumnWidth(16, 10 * 256);
                int rownum = 0;
                int colnum = 0;

                #endregion

                colnum = 0;
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "生产单：", headStyle2);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstrList.Count()
                    + " 张", headStyle2);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "生产明细：", headStyle2);
                int detCount = orderDetList.Where(d => orderMstrList.Select(m => m.OrderNo).Contains(d.OrderNo)).Count();
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, detCount + " 笔", headStyle2);
                rownum++;

                #region 输出列头
                colnum = 0;
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "生产线", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "生产单号 ↑", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "窗口时间 ↑", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "类型 ↑", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "外部单号", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "参考单号", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "开始时间", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "班次", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "优先级", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "区域 ↑", headStyle);
                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "目的", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "来源库位", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "目的库位", headStyle);
                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "发货地址", headStyle);
                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "收货地址", headStyle);
                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "送货道口", headStyle);
                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "结算方式", headStyle);
                XlsHelper.SetRowCell(woSheet, rownum, colnum++, "状态 ↑", headStyle);
                if (sheetName == SHEET_NAME_CANCEL)
                {
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, "取消人", headStyle);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, "取消日期", headStyle);
                }
                else
                {
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, "创建人", headStyle);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, "创建日期", headStyle);
                }

                if (sheetName == SHEET_NAME_12)
                {
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, "上线日期", headStyle);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, "完成日期", headStyle);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, "小时差", headStyle);
                }

                //XlsHelper.SetRowCell(sheet1, rownum, colnum++, "币种", headStyle);
                #endregion

                #region 输出数据
                DateTime? WindowTime = null;
                string partyFrom = string.Empty;
                foreach (var orderMstr in orderMstrList)
                {
                    rownum++;
                    colnum = 0;
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.Flow);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.OrderNo);
                    if (!WindowTime.HasValue || WindowTime.Value.ToString("yyyy-MM-dd").CompareTo(orderMstr.WindowTime.ToString("yyyy-MM-dd")) != 0
                           || string.IsNullOrEmpty(partyFrom) || partyFrom != orderMstr.PartyFrom)
                    {
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.WindowTime.ToString("yyyy-MM-dd HH:mm"), cellStyleColor);
                    }
                    else
                    {
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.WindowTime.ToString("yyyy-MM-dd HH:mm"));
                    }
                    WindowTime = orderMstr.WindowTime;
                    partyFrom = orderMstr.PartyFrom;
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.SubTypeDesc);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.ExtOrderNo);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.RefOrderNo);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.StartTime.ToString("yyyy-MM-dd HH:mm"));
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.Shift);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.Priority);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.PartyFrom);
                    //XlsHelper.SetRowCell(sheet1, rownum, colnum++, orderMstr.PartyTo);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.LocFrom);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.LocTo);
                    //XlsHelper.SetRowCell(sheet1, rownum, colnum++, orderMstr.ShipFrom);
                    //XlsHelper.SetRowCell(sheet1, rownum, colnum++, orderMstr.ShipTo);
                    //XlsHelper.SetRowCell(sheet1, rownum, colnum++, orderMstr.DockDesc);
                    //XlsHelper.SetRowCell(sheet1, rownum, colnum++, orderMstr.BillSettleTerm);
                    XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.Status);
                    if (sheetName == SHEET_NAME_CANCEL)
                    {
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.CancelUser);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.CancelDate.Value.ToString("yyyy-MM-dd HH:mm"));
                    }
                    else
                    {
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.CreateUser);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.CreateDate.ToString("yyyy-MM-dd HH:mm"));
                    }

                    if (sheetName == SHEET_NAME_12)
                    {
                        if (orderMstr.StartDate.HasValue)
                        {
                            XlsHelper.SetRowCell(woSheet, rownum, colnum, orderMstr.StartDate.Value.ToString("yyyy-MM-dd HH:mm"));
                        }
                        colnum++;
                        if (orderMstr.CompleteDate.HasValue)
                        {
                            XlsHelper.SetRowCell(woSheet, rownum, colnum, orderMstr.CompleteDate.Value.ToString("yyyy-MM-dd HH:mm"));
                        }
                        colnum++;
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderMstr.DiffHour);
                    }

                    //XlsHelper.SetRowCell(sheet1, rownum, colnum++, orderMstr.Currency);

                    #region 发货明细
                    var orderDetTList = orderDetList.Where(d => d.OrderNo == orderMstr.OrderNo).ToList<OrderDet>();
                    if (orderDetTList != null && orderDetTList.Count > 0)
                    {
                        colnum = 1;
                        rownum++;
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "类型", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "物料", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "参考物料号", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "来源库位", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "目的库位", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "单位", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "单包装", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "计划数", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "合格数", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "次品数", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "废品数", cellStyleDet);
                        XlsHelper.SetRowCell(woSheet, rownum, colnum++, "完成率", cellStyleDet);
                        foreach (var orderDet in orderDetTList)
                        {
                            colnum = 1;
                            rownum++;
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.Type);
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.Item);
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.RefItemCode);
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.LocFrom);
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.LocTo);
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.Uom);
                            if (orderDet.UC != 0)
                            {
                                XlsHelper.SetRowCell(woSheet, rownum, colnum, orderDet.UC.ToString("0.########"));
                            }
                            colnum++;
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.OrderQty.ToString("0.########"));
                            if (orderDet.RecQty.HasValue)
                            {
                                XlsHelper.SetRowCell(woSheet, rownum, colnum, orderDet.RecQty.Value.ToString("0.########"));
                            }
                            colnum++;
                            if (orderDet.RejQty.HasValue)
                            {
                                XlsHelper.SetRowCell(woSheet, rownum, colnum, orderDet.RejQty.Value.ToString("0.########"));
                            }
                            colnum++;
                            if (orderDet.ScrapQty.HasValue)
                            {
                                XlsHelper.SetRowCell(woSheet, rownum, colnum, orderDet.ScrapQty.Value.ToString("0.########"));
                            }
                            colnum++;
                            XlsHelper.SetRowCell(woSheet, rownum, colnum++, orderDet.CompleteRate.ToString("P"));
                        }

                        woSheet.GroupRow(rownum - orderDetTList.Count, rownum);
                        woSheet.SetRowGroupCollapsed(rownum, true);
                    }
                    #endregion
                }

                woSheet.GroupRow(rownum - orderMstrList.Count * 2 - detCount, rownum + 1);
                woSheet.SetRowGroupCollapsed(rownum + 1, true);

                #endregion
                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }
        #endregion

        #region 库龄

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessLocAging(IWorkbook workbook)
        {
            try
            {
                #region 获取库存数据
                StringBuilder locLotDetSql = new StringBuilder();
                locLotDetSql.Append(@" SELECT i.Code ItemCode,i.Desc1 ItemDesc,i.UC,i.Uom, ");
                locLotDetSql.Append(@"        l.Name+'['+l.Code+']' Location,p.Name+'['+p.Code+']' Region, ");
                locLotDetSql.Append(@"        CONVERT(datetime,convert(varchar(10),lld.CreateDate,120)) CreateDate,");
                locLotDetSql.Append(@"        SUM(CASE WHEN IsCS = 1 THEN Qty ELSE 0 END) AS CsQty, ");
                locLotDetSql.Append(@"        SUM(CASE WHEN IsCS = 0 THEN Qty ELSE 0 END) AS NmlQty, ");
                locLotDetSql.Append(@"        SUM(lld.Qty) AS Qty ");
                locLotDetSql.Append(@" FROM LocationLotDet lld ");
                locLotDetSql.Append(@" join Item i (nolock) on lld.Item=i.Code ");
                locLotDetSql.Append(@" join Location l (nolock) on lld.Location=l.Code ");
                locLotDetSql.Append(@" join Party p (nolock) on l.Region=p.Code ");
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

        protected virtual bool ProcessLocAging(IWorkbook workbook, IList<LocLotDet> locLotDetList)
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

                //输出汇总
                var amount = locLotDetList.Where(b => b.Amount.HasValue).Sum(b => b.Amount.Value);
                var amount7 = locLotDetList.Where(b => b.Amount7.HasValue).Sum(b => b.Amount7.Value);
                var amount30 = locLotDetList.Where(b => b.Amount30.HasValue).Sum(b => b.Amount30.Value);
                var amount60 = locLotDetList.Where(b => b.Amount60.HasValue).Sum(b => b.Amount60.Value);
                var amount90 = locLotDetList.Where(b => b.Amount90.HasValue).Sum(b => b.Amount90.Value);
                var totalQty = locLotDetList.Sum(b => b.Qty);
                var itemCount = locLotDetList.Select(a => a.Item).Distinct().Count();
                XlsHelper.SetRowCell(sheet, rownum, column++, "总金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount.ToString("0.##元"), headStyle2);
                column++;
                XlsHelper.SetRowCell(sheet, rownum, column++, "总数量：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, totalQty.ToString("0.##"), headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "物料种类：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, itemCount.ToString(), headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于7天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount7.ToString("0.##元"), headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于30天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount30.ToString("0.##元"), headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于60天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount60.ToString("0.##元"), headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, "大于90天金额：", headStyle2);
                XlsHelper.SetRowCell(sheet, rownum, column++, amount90.ToString("0.##元"), headStyle2);

                column = 0;
                rownum++;
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

        #endregion

        #region 计划外出库

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessUnpCyc(IWorkbook workbook, string title, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                /*
                    计划外库存调整项：
                    1. 库存计划外调整条目数（含计划外入库、计划外出库、盘点差异调整）
                    2. 明细按类型列出计划外调整明细
                */

                #region 初始化Sheet
                bool rtn = false;
                ISheet sheet1 = workbook.CreateSheet(title);

                for (int i = 0; i <= 12; i++)
                {
                    sheet1.AutoSizeColumn(i);
                    if (i != 8)
                    {
                        sheet1.SetDefaultColumnStyle(i, cellStyle);
                    }
                }
                sheet1.SetColumnWidth(0, 15 * 256);
                sheet1.SetColumnWidth(1, 23 * 256);
                sheet1.SetColumnWidth(2, 27 * 256);
                sheet1.SetColumnWidth(3, 21 * 256);
                sheet1.SetColumnWidth(4, 15 * 256);
                sheet1.SetColumnWidth(5, 15 * 256);
                sheet1.SetColumnWidth(6, 16 * 256);
                sheet1.SetColumnWidth(7, 16 * 256);
                sheet1.SetColumnWidth(8, 16 * 256);
                sheet1.SetColumnWidth(9, 16 * 256);

                int rownum = 0;
                int colnum = 0;

                #endregion

                #region 计划外出入库

                //计划外出入库数据
                var miscOrderList = GetMisOrder(startDate.Value, endDate.Value, string.Empty);

                if (miscOrderList != null && miscOrderList.Count > 0)
                {
                    var miscOrderDetList = GetMisOrderDet(startDate.Value, endDate.Value, string.Empty);
                    if (miscOrderDetList != null && miscOrderDetList.Count > 0)
                    {
                        rtn = true;

                        #region 计划外出库

                        var giList =
                            miscOrderList.Where(m => m.Type == BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GI)
                                         .ToList<MiscOrder>();
                        var giDetList =
                            miscOrderDetList.Where(d => d.Type == BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GI)
                                            .ToList<MiscOrderDetail>();

                        #region 数据输出

                        if (giList.Count() > 0 && giDetList.Count() > 0)
                        {
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "计划外出库单：", headStyle2);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, giList.Count() + " 张", headStyle2);

                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "计划外出库明细：", headStyle2);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, giDetList.Count() + " 笔", headStyle2);

                            var amountTotal = giDetList.Sum(c => c.Amount);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "金额：", headStyle2);
                            if (amountTotal.HasValue)
                            {
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, amountTotal.Value.ToString("0.########"),
                                                     headStyle2);
                            }

                            rownum++;

                            OutUnpData(giList, giDetList, ref rownum, ref colnum, sheet1, cellStyleDet);
                        }

                        #endregion

                        rownum++;

                        #endregion

                        #region 计划外入库

                        var grList =
                            miscOrderList.Where(m => m.Type == BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GR)
                                         .ToList<MiscOrder>();
                        var grDetList =
                            miscOrderDetList.Where(d => d.Type == BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GR)
                                            .ToList<MiscOrderDetail>();

                        #region 数据输出

                        if (grList.Count() > 0 && grDetList.Count() > 0)
                        {

                            colnum = 0;
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "计划外入库单：", headStyle2);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, grList.Count() + " 张", headStyle2);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "计划外入库明细：", headStyle2);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, grDetList.Count() + " 笔", headStyle2);

                            var amountTotal = grDetList.Sum(c => c.Amount);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "金额：", headStyle2);
                            if (amountTotal.HasValue)
                            {
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, amountTotal.Value.ToString("0.########"),
                                                     headStyle2);
                            }

                            rownum++;

                            OutUnpData(grList, grDetList, ref rownum, ref colnum, sheet1, cellStyleDet);
                        }

                        #endregion

                        #endregion
                    }
                }
                #endregion

                #region 盘点
                #region 获取盘点明细数据
                StringBuilder cycResultSql = new StringBuilder();
                cycResultSql.Append(@" select r.Id,r.OrderNo, r.Item+'['+ i.Desc1+']' Item, r.HuId, ");
                cycResultSql.Append(@" r.LotNo, r.Qty, r.InvQty, r.DiffQty, r.Bin, l.Code+'['+l.Name+']' Location, ");
                cycResultSql.Append(@" r.ProcessDate,r.ProcessUserNm+'['+r.ProcessUser+']' ProcessUser,i.NumField3  Price,i.NumField3  * r.DiffQty Amount ");
                cycResultSql.Append(@" from CycleCountResult r ");
                cycResultSql.Append(@" join CycleCountMstr m (nolock) on m.Code=r.OrderNo ");
                cycResultSql.Append(@" join Item i (nolock) on i.Code=r.Item ");
                cycResultSql.Append(@" left join Location l (nolock) on l.Code=r.Location ");
                cycResultSql.Append(@" where r.IsProcess =1 and r.ProcessDate >='" + startDate.Value + "' and r.ProcessDate<'" + endDate.Value + "' ");
                cycResultSql.Append(@" order by r.ProcessDate asc,m.EffDate asc,r.Id asc ");
                DataSet cycResultDS = sqlHelperMgrE.GetDatasetBySql(cycResultSql.ToString());
                List<CycleCountResult> cycResultList = IListHelper.DataTableToList<CycleCountResult>(cycResultDS.Tables[0]);
                #endregion
                if (cycResultList != null && cycResultList.Count() > 0)
                {
                    rtn = true;
                    #region 获取盘点单数据
                    var cycOrderNo = cycResultList.Select(r => r.OrderNo).Distinct().ToList<string>();
                    StringBuilder cycMstrSql = new StringBuilder();
                    cycMstrSql.Append(@" select m.Code OrderNo, c1.Desc1 Type, l.Code+'['+l.Name+']' Location, m.EffDate, c2.Desc1 Status,u.USR_FirstName+u.USR_LastName+'['+u.USR_Code+']' CompleteUser, CompleteDate, c3.Desc1  PhyCntGroupBy ");
                    cycMstrSql.Append(@" from CycleCountMstr m ");
                    cycMstrSql.Append(@" join ACC_User u (nolock) on u.USR_Code = m.CompleteUser ");
                    cycMstrSql.Append(@" join CodeMstr c1 (nolock) on c1.Code = '" + BusinessConstants.CODE_MASTER_PHYCNT_TYPE + "' and c1.CodeValue=m.Type ");
                    cycMstrSql.Append(@" join CodeMstr c2 (nolock) on c2.Code = '" + BusinessConstants.CODE_MASTER_STATUS + "' and c2.CodeValue=m.Status ");
                    cycMstrSql.Append(@" left join CodeMstr c3 (nolock) on c3.Code = '" + BusinessConstants.CODE_MASTER_PHYCNT_GROUPBY + "' and c3.CodeValue=m.PhyCntGroupBy ");
                    cycMstrSql.Append(@" left join Location l (nolock) on l.Code=m.Location ");
                    for (int i = 0; i < cycOrderNo.Count(); i++)
                    {
                        if (i == 0)
                        {
                            cycMstrSql.Append(@" where  ");
                        }
                        else
                        {
                            cycMstrSql.Append(@" or ");
                        }
                        cycMstrSql.Append(" m.Code = '" + cycOrderNo[i] + "' ");

                    }

                    DataSet cycMstrDS = sqlHelperMgrE.GetDatasetBySql(cycMstrSql.ToString());
                    List<CycleCountMstr> cycMstrList = IListHelper.DataTableToList<CycleCountMstr>(cycMstrDS.Tables[0]);
                    #endregion

                    #region 输出数据
                    rownum++;
                    colnum = 0;
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘点单：", headStyle2);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstrList.Count() + " 张", headStyle2);

                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘点结果：", headStyle2);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResultList.Count() + " 笔", headStyle2);

                    var lossesAmountTotal = cycResultList.Where(c => c.DiffQty < 0).Sum(c => c.Amount);
                    var profitAmountTotal = cycResultList.Where(c => c.DiffQty > 0).Sum(c => c.Amount);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘亏金额：", headStyle2);
                    if (lossesAmountTotal.HasValue)
                    {
                        XlsHelper.SetRowCell(sheet1, rownum, colnum, lossesAmountTotal.Value.ToString("0.########"), headStyle2);
                    }
                    colnum++;
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘盈金额：", headStyle2);
                    if (profitAmountTotal.HasValue)
                    {
                        XlsHelper.SetRowCell(sheet1, rownum, colnum, profitAmountTotal.Value.ToString("0.########"), headStyle2);
                    }
                    colnum++;
                    //调整物料种类数
                    int itemCount = cycResultList.Select(c => c.Item).Distinct().Count();
                    //库存物料种类数
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "调整率：", headStyle2);
                    var itemInvCountList = hqlMgrE.FindAll<long>("select Count(Distinct i.Code) from LocationLotDetail lld join lld.Item i  where lld.Qty!=0 ");
                    //var itemInvCountDS = sqlHelperMgrE.GetDatasetBySql("select count(distinct(item)) C from LocationLotDet  where qty!=0");
                    //var itemInvCountList = IListHelper.DataTableToList<int>(itemInvCountDS.Tables[0]);
                    long itemInvCount = 0;
                    if (itemInvCountList != null && itemInvCountList.Count > 0)
                    {
                        itemInvCount = itemInvCountList[0];
                    }
                    float p = 1.0f * itemCount / itemInvCount;
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, p.ToString("P"), headStyle2);

                    rownum++;

                    #region 输出列头
                    colnum = 0;
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "类型", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "单号", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "库位", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "完成人", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "完成日期", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "生效日期", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "状态", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘点方式", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘亏金额", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘盈金额", headStyle);
                    #endregion

                    foreach (var cycMstr in cycMstrList)
                    {
                        rownum++;
                        colnum = 0;
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.Type);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.OrderNo);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.Location);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.CompleteUser);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.CompleteDate.ToString("yyyy-MM-dd HH:mm"));
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.EffDate.ToString("yyyy-MM-dd HH:mm"));
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.Status);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycMstr.PhyCntGroupBy);

                        #region 盘点结果
                        var cycResultTList = cycResultList.Where(r => r.OrderNo == cycMstr.OrderNo).ToList<CycleCountResult>();
                        if (cycResultTList != null && cycResultTList.Count > 0)
                        {
                            var lossesAmount = cycResultTList.Where(c => c.DiffQty < 0).Sum(c => c.Amount);
                            var profitAmount = cycResultTList.Where(c => c.DiffQty > 0).Sum(c => c.Amount);
                            if (lossesAmount.HasValue)
                            {
                                XlsHelper.SetRowCell(sheet1, rownum, colnum, lossesAmount.Value.ToString("0.########"));
                            }
                            colnum++;
                            if (profitAmount.HasValue)
                            {
                                XlsHelper.SetRowCell(sheet1, rownum, colnum, profitAmount.Value.ToString("0.########"));
                            }


                            rownum++;
                            colnum = 1;
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "物料", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "库位", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "处理人", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "处理时间", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "类型", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "库存数", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "盘点数", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "差异数", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "单价", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "金额", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "条码", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "批号", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "库格", cellStyleDet);

                            foreach (var cycResult in cycResultTList)
                            {
                                colnum = 1;
                                rownum++;
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.Item);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.Location);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.ProcessUser);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.ProcessDate.ToString("yyyy-MM-dd HH:mm"));
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.Type);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.InvQty.ToString("0.########"));
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.Qty.ToString("0.########"));
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.DiffQty.ToString("0.########"));
                                if (cycResult.Price.HasValue)
                                {
                                    XlsHelper.SetRowCell(sheet1, rownum, colnum, cycResult.Price.Value.ToString("0.########"));
                                }
                                colnum++;
                                if (cycResult.Amount.HasValue)
                                {
                                    XlsHelper.SetRowCell(sheet1, rownum, colnum, cycResult.Amount.Value.ToString("0.########"));
                                }
                                colnum++;
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.HuId);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.LotNo);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, cycResult.Bin);
                            }
                            sheet1.GroupRow(rownum - cycResultTList.Count, rownum);
                            sheet1.SetRowGroupCollapsed(rownum, true);
                        }
                        #endregion
                    }

                    sheet1.GroupRow(rownum - cycMstrList.Count * 2 - cycResultList.Count, rownum + 1);
                    sheet1.SetRowGroupCollapsed(rownum + 1, true);

                    #endregion
                }
                #endregion

                return rtn;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        //工废//报废
        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessUnpIndustrialWaste(IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    //计划外出入库数据
                    var miscOrderList = GetMisOrder(startDate.Value, endDate.Value, @" and m.Reason in ('" + BusinessConstants.CODE_MASTER_STOCK_OUT_REASON_VALUE_REASON1 + "','" + BusinessConstants.CODE_MASTER_STOCK_OUT_REASON_VALUE_REASON13 + "') ");

                    if (miscOrderList != null && miscOrderList.Count > 0)
                    {
                        var miscOrderDetList = GetMisOrderDet(startDate.Value, endDate.Value, @" and m.Reason in ('" + BusinessConstants.CODE_MASTER_STOCK_OUT_REASON_VALUE_REASON1 + "','" + BusinessConstants.CODE_MASTER_STOCK_OUT_REASON_VALUE_REASON13 + "') ");
                        if (miscOrderDetList != null && miscOrderDetList.Count > 0)
                        {
                            #region 初始化Sheet
                            bool rtn = false;
                            ISheet sheet1 = workbook.CreateSheet("工废&报废");

                            for (int i = 0; i <= 7; i++)
                            {
                                sheet1.AutoSizeColumn(i);
                                if (i != 8)
                                {
                                    sheet1.SetDefaultColumnStyle(i, cellStyle);
                                }
                            }
                            sheet1.SetColumnWidth(0, 10 * 256);
                            sheet1.SetColumnWidth(1, 28 * 256);
                            sheet1.SetColumnWidth(2, 27 * 256);
                            sheet1.SetColumnWidth(3, 21 * 256);
                            sheet1.SetColumnWidth(4, 15 * 256);
                            sheet1.SetColumnWidth(5, 15 * 256);
                            sheet1.SetColumnWidth(6, 16 * 256);
                            sheet1.SetColumnWidth(7, 15 * 256);


                            int rownum = 0;
                            int colnum = 0;

                            #endregion

                            OutUnpData(miscOrderList, miscOrderDetList, ref rownum, ref colnum, sheet1, cellStyleDet);

                            sheet1.ForceFormulaRecalculation = true;
                            sheet1.CreateFreezePane(0, 1, 0, 1);

                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual IList<MiscOrder> GetMisOrder(DateTime startDate, DateTime endDate, string where)
        {
            try
            {
                StringBuilder miscOrderSql = new StringBuilder();
                miscOrderSql.Append(@" select m.OrderNo,m.Type,c2.Desc1 TypeDesc,l.Code+'['+l.Name+']' Location, ");
                miscOrderSql.Append(@" m.EffDate,m.Remark,m.CreateDate,u.USR_FirstName+u.USR_LastName+'['+u.USR_Code+']' CreateUser,c1.Desc1 Reason ");
                miscOrderSql.Append(@" from MiscOrderMstr m ");
                miscOrderSql.Append(@" join Location l (nolock) on m.Location = l.Code  ");
                miscOrderSql.Append(@" join ACC_User u (nolock) on m.CreateUser = u.USR_Code  ");
                miscOrderSql.Append(@" left join CodeMstr c1 (nolock) on c1.Code in ('" + BusinessConstants.CODE_MASTER_STOCK_OUT_REASON + "','" + BusinessConstants.CODE_MASTER_STOCK_IN_REASON + "') and c1.CodeValue=m.Reason ");
                miscOrderSql.Append(@" left join CodeMstr c2 (nolock) on c2.Code = '" + BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE + "' and c2.CodeValue=m.Type ");
                miscOrderSql.Append(@" where m.CreateDate >='" + startDate + "' and m.CreateDate<'" + endDate + "' ");

                if (!string.IsNullOrEmpty(where))
                {
                    miscOrderSql.Append(where);
                }

                miscOrderSql.Append(@" order by m.Type ASC,m.CreateDate ASC,m.EffDate ASC ");
                DataSet miscOrderDS = sqlHelperMgrE.GetDatasetBySql(miscOrderSql.ToString());
                var miscOrderList = IListHelper.DataTableToList<MiscOrder>(miscOrderDS.Tables[0]);
                return miscOrderList;
            }
            catch (Exception)
            {

            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual IList<MiscOrderDetail> GetMisOrderDet(DateTime startDate, DateTime endDate, string where)
        {
            try
            {
                StringBuilder miscOrderDetSql = new StringBuilder();
                miscOrderDetSql.Append(@" select m.Type,d.Id,d.OrderNo,d.Item+'['+i.Desc1+']' Item,d.HuId,d.LotNo,d.Qty,i.NumField3  Price,i.NumField3  * d.Qty Amount ");
                miscOrderDetSql.Append(@" from MiscOrderDet d ");
                miscOrderDetSql.Append(@" join MiscOrderMstr m (nolock) on d.OrderNo = m.OrderNo ");
                miscOrderDetSql.Append(@" join Item i (nolock) on i.Code = d.Item ");
                miscOrderDetSql.Append(@" where m.CreateDate >='" + startDate + "' and m.CreateDate<'" + endDate + "' ");

                if (!string.IsNullOrEmpty(where))
                {
                    miscOrderDetSql.Append(where);
                }

                miscOrderDetSql.Append(@" order by m.Type ASC,m.CreateDate ASC,m.EffDate ASC,d.Id ASC ");
                DataSet miscOrderDetDS = sqlHelperMgrE.GetDatasetBySql(miscOrderDetSql.ToString());
                var miscOrderDetList = IListHelper.DataTableToList<MiscOrderDetail>(miscOrderDetDS.Tables[0]);

                return miscOrderDetList;
            }
            catch (Exception)
            {

            }
            return null;
        }

        private void OutUnpData(IList<MiscOrder> miscOrderList, IList<MiscOrderDetail> miscOrderDetList, ref int rownum, ref int colnum, ISheet sheet2, ICellStyle cellStyleDet)
        {
            #region 列头
            colnum = 0;
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "类型", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "单号", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "库位", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "创建人", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "创建日期", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "生效日期", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "原因", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "金额", headStyle);
            XlsHelper.SetRowCell(sheet2, rownum, colnum++, "备注", headStyle);

            #endregion

            foreach (var miscOrder in miscOrderList)
            {
                rownum++;
                colnum = 0;
                XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrder.TypeDesc);
                XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrder.OrderNo);
                XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrder.Location);
                XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrder.CreateUser);
                XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrder.CreateDate.ToString("yyyy-MM-dd HH:mm"));
                XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrder.EffDate.ToString("yyyy-MM-dd HH:mm"));
                XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrder.Reason);

                colnum++;

                XlsHelper.SetRowCell(sheet2, rownum, colnum, miscOrder.Remark);
                rownum++;

                #region 明细
                var miscOrderDetailTList = miscOrderDetList.Where(d => d.OrderNo == miscOrder.OrderNo).ToList<MiscOrderDetail>();

                if (miscOrderDetailTList != null && miscOrderDetailTList.Count > 0)
                {
                    var amount = miscOrderDetailTList.Sum(c => c.Amount);
                    if (amount.HasValue)
                    {
                        XlsHelper.SetRowCell(sheet2, rownum - 1, colnum - 1, amount.Value.ToString("0.########"));
                    }
                    colnum = 1;
                    XlsHelper.SetRowCell(sheet2, rownum, colnum++, "物料", cellStyleDet);
                    XlsHelper.SetRowCell(sheet2, rownum, colnum++, "数量", cellStyleDet);
                    XlsHelper.SetRowCell(sheet2, rownum, colnum++, "单价", cellStyleDet);
                    XlsHelper.SetRowCell(sheet2, rownum, colnum++, "金额", cellStyleDet);
                    XlsHelper.SetRowCell(sheet2, rownum, colnum++, "条码", cellStyleDet);
                    XlsHelper.SetRowCell(sheet2, rownum, colnum++, "批号", cellStyleDet);
                    foreach (var miscOrderDet in miscOrderDetailTList)
                    {
                        colnum = 1;
                        rownum++;
                        XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrderDet.Item);
                        XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrderDet.Qty.ToString("0.########"));
                        if (miscOrderDet.Price.HasValue)
                        {
                            XlsHelper.SetRowCell(sheet2, rownum, colnum, miscOrderDet.Price.Value.ToString("0.########"));
                        }
                        colnum++;
                        if (miscOrderDet.Amount.HasValue)
                        {
                            XlsHelper.SetRowCell(sheet2, rownum, colnum, miscOrderDet.Amount.Value.ToString("0.########"));
                        }
                        colnum++;
                        XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrderDet.HuId);
                        XlsHelper.SetRowCell(sheet2, rownum, colnum++, miscOrderDet.LotNo);
                    }

                    sheet2.GroupRow(rownum - miscOrderDetailTList.Count, rownum);
                    sheet2.SetRowGroupCollapsed(rownum, true);

                    //sheet2.CreateFreezePane(0, 4, 0, 4);
                }
                #endregion
                //rownum++;
            }
            //sheet2.GroupRow(rownum - miscOrderList.Count * 2 - miscOrderDetList.Count, rownum + 1);
            //sheet2.SetRowGroupCollapsed(rownum + 1, true);
        }

        #endregion


        #region 加料统计
        protected virtual bool ProcessLoadingStatistics(string key, IWorkbook workbook)
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                var shiftDetailList = hqlMgrE.FindAll<ShiftDetail>("select sf from ShiftDetail sf join sf.Shift s where s.Code in ('A','B') order by s.Code asc ");
                if (shiftDetailList == null || shiftDetailList.Count < 2) return false;
                Shift shiftA = new Shift();
                shiftA.ShiftTime = shiftDetailList[0].ShiftTime.Split('-');
                shiftA.Name = shiftDetailList[0].Shift.ShiftName;
                Shift shiftB = new Shift();
                shiftB.ShiftTime = shiftDetailList[1].ShiftTime.Split('-');
                shiftB.Name = shiftDetailList[1].Shift.ShiftName;

                DateTime now = DateTime.Now;
                DateTime startDate = now;
                //上个月
                DateTime endDate = DateTime.Parse(now.ToString("yyyy-MM-dd " + shiftA.ShiftTime[0]));
                if (now.Day == 1 && now.Hour >= 8)
                {
                    startDate = DateTime.Parse(now.AddMonths(-1).ToString("yyyy-MM-01 " + shiftA.ShiftTime[0]));
                }
                //本月
                else
                {
                    startDate = DateTime.Parse(now.ToString("yyyy-MM-01 " + shiftA.ShiftTime[0]));
                }
                // endDate = DateTime.Parse(now.ToString("yyyy-MM-dd " + shiftA.ShiftTime[1]));

                sqlParam[0] = new SqlParameter("@StartDate", startDate);
                sqlParam[1] = new SqlParameter("@EndDate", endDate);
                DataSet lsDS = sqlHelperMgrE.GetDatasetByStoredProcedure("USP_Rep_LoadingStatistics", sqlParam);

                //一次合格率
                IList<LoadingStatistics> lsList =
                    IListHelper.DataTableToList<LoadingStatistics>(lsDS.Tables[0]);
                if (lsList == null || lsList.Count == 0) return false;

                int days = System.DateTime.DaysInMonth(startDate.Year, startDate.Month);

                var loadingStatisticsList = (from l in lsList
                                             group l by new { l.Loc, l.Item, l.ItemDesc, l.Uom, Day = (l.EffDate < DateTime.Parse(l.EffDate.ToString("yyyy-MM-dd " + shiftA.ShiftTime[0])) ? l.Day - 1 : l.Day), ShiftPos = l.ShiftPos(shiftA.ShiftTime) } into g
                                             select new
                                             {
                                                 Loc = g.Key.Loc,
                                                 Item = g.Key.Item,
                                                 ItemDesc = g.Key.ItemDesc,
                                                 Uom = g.Key.Uom,
                                                 Pos = (g.Key.Day - 1) * 2 + g.Key.ShiftPos,
                                                 Day = g.Key.Day,
                                                 Qty = g.Sum(a => a.Qty),
                                                 Area = g.Sum(a => a.Area),
                                                 MinEffDate = g.Min(a => a.EffDate),
                                                 MaxEffDate = g.Max(a => a.EffDate)
                                             }
                                             ).OrderBy(l => l.Item).OrderBy(l => l.Loc).ToList();
                #region 初始化Sheet

                ISheet lsSheet = workbook.CreateSheet(key);

                for (int i = 0; i <= 16; i++)
                {
                    lsSheet.AutoSizeColumn(i);
                    lsSheet.SetDefaultColumnStyle(i, cellStyle);
                }
                //库位		物料代码	物料名称		单位					
                lsSheet.SetColumnWidth(0, 26 * 256);
                lsSheet.SetColumnWidth(1, 13 * 256);
                lsSheet.SetColumnWidth(2, 31 * 256);
                lsSheet.SetColumnWidth(4, 6 * 256);

                int rownum = 0;
                int colnum = 0;

                #endregion

                #region 输出列头
                colnum = 0;

                ICellStyle headStyle1 = workbook.CreateCellStyle();
                headStyle1.CloneStyleFrom(headStyle);
                headStyle1.Alignment = HorizontalAlignment.CENTER;
                headStyle1.VerticalAlignment = VerticalAlignment.CENTER;
                XlsHelper.SetRowCell(lsSheet, rownum, colnum++, "库位", headStyle1);
                XlsHelper.SetMergedRegion(lsSheet, rownum, colnum - 1, rownum + 1, colnum - 1);
                XlsHelper.SetRowCell(lsSheet, rownum, colnum++, "物料代码", headStyle1);
                XlsHelper.SetMergedRegion(lsSheet, rownum, colnum - 1, rownum + 1, colnum - 1);
                XlsHelper.SetRowCell(lsSheet, rownum, colnum++, "物料名称", headStyle1);
                XlsHelper.SetMergedRegion(lsSheet, rownum, colnum - 1, rownum + 1, colnum - 1);
                XlsHelper.SetRowCell(lsSheet, rownum, colnum++, "单位", headStyle1);
                XlsHelper.SetMergedRegion(lsSheet, rownum, colnum - 1, rownum + 1, colnum - 1);
                string desc = startDate.ToString("MM月");

                for (int i = 1; i <= days; i++)
                {
                    XlsHelper.SetRowCell(lsSheet, rownum, colnum + (i - 1) * 2, desc + i + "日", headStyle1);
                    XlsHelper.SetMergedRegion(lsSheet, rownum, colnum + (i - 1) * 2, rownum, colnum + (i - 1) * 2 + 1);
                    XlsHelper.SetRowCell(lsSheet, rownum + 1, colnum + (i - 1) * 2, shiftA.Name, headStyle1);
                    XlsHelper.SetRowCell(lsSheet, rownum + 1, colnum + (i - 1) * 2 + 1, shiftB.Name, headStyle1);
                    lsSheet.SetColumnWidth(3 + i, 10 * 256);
                    lsSheet.SetColumnWidth(3 + i + 1, 10 * 256);
                    //colnum++;
                }
                XlsHelper.SetRowCell(lsSheet, rownum, 4 + days * 2, "加料汇总", headStyle1);
                XlsHelper.SetMergedRegion(lsSheet, rownum, 4 + days * 2, rownum + 1, 4 + days * 2);

                #endregion
                rownum++;
                rownum++;
                decimal rowQty = 0;
                decimal totalArea = 0;
                decimal[] areaArray = new decimal[days * 2];
                int locationItemCount = 0;
                for (int i = 0; i < loadingStatisticsList.Count; i++)
                {
                    var ls = loadingStatisticsList[i];
                    if (i == 0 || loadingStatisticsList[i - 1].Loc != loadingStatisticsList[i].Loc || loadingStatisticsList[i - 1].Item != loadingStatisticsList[i].Item)
                    {
                        if (i == 0 || loadingStatisticsList[i - 1].Loc != loadingStatisticsList[i].Loc)
                        {
                            XlsHelper.SetRowCell(lsSheet, rownum, 0, ls.Loc, cellStyleCenter);
                        }
                        XlsHelper.SetRowCell(lsSheet, rownum, 1, ls.Item);
                        XlsHelper.SetRowCell(lsSheet, rownum, 2, ls.ItemDesc);
                        XlsHelper.SetRowCell(lsSheet, rownum, 3, ls.Uom);
                    }
                    XlsHelper.SetRowCell(lsSheet, rownum, 4 + ls.Pos, ls.Qty);
                    rowQty += ls.Qty;
                    areaArray[ls.Pos] += ls.Area;
                    totalArea += ls.Area;
                    if (locationItemCount != 0 && (i == loadingStatisticsList.Count - 1 || loadingStatisticsList[i].Loc != loadingStatisticsList[i + 1].Loc))
                    {
                        XlsHelper.SetMergedRegion(lsSheet, rownum - locationItemCount, 0, rownum, 0);
                        locationItemCount = 0;
                    }
                    else if (i != loadingStatisticsList.Count - 1 && loadingStatisticsList[i].Loc == loadingStatisticsList[i + 1].Loc && loadingStatisticsList[i].Item != loadingStatisticsList[i + 1].Item)
                    {
                        locationItemCount++;
                    }
                    if (i == loadingStatisticsList.Count - 1 || loadingStatisticsList[i].Loc != loadingStatisticsList[i + 1].Loc || loadingStatisticsList[i].Item != loadingStatisticsList[i + 1].Item)
                    {
                        XlsHelper.SetRowCell(lsSheet, rownum, 4 + days * 2, rowQty);
                        rowQty = 0;
                        rownum++;
                    }
                }
                //面积数据                
                XlsHelper.SetMergedRegion(lsSheet, rownum, 0, rownum, 3);
                XlsHelper.SetRowCell(lsSheet, rownum, 0, "电镀面积(平方分米)", headStyle1);
                for (int i = 0; i < areaArray.Length; i++)
                {
                    var area = areaArray[i];
                    if (area != 0)
                    {
                        XlsHelper.SetRowCell(lsSheet, rownum, 4 + i, area);
                    }
                }
                XlsHelper.SetRowCell(lsSheet, rownum, 4 + days * 2, totalArea);
                lsSheet.ForceFormulaRecalculation = true;
                lsSheet.CreateFreezePane(3, 2, 3, 2);
                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }
        #endregion
    }

    #region  报表实体

    public class OrderDet
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string Item { get; set; }
        public string RefItemCode { get; set; }
        public string Uom { get; set; }
        public Decimal UC { get; set; }
        public Decimal? ReqQty { get; set; }
        public Decimal OrderQty { get; set; }
        public Decimal? ShipQty { get; set; }
        public Decimal? RecQty { get; set; }
        public Decimal? RejQty { get; set; }
        public Decimal? ScrapQty { get; set; }
        public string LocFrom { get; set; }
        public string LocTo { get; set; }
        public Decimal? UnitPriceAfterDiscount { get; set; }
        public string Type
        {
            get
            {
                if (RecQty.HasValue && RecQty.Value > 0)
                {
                    if (OrderQty > RecQty.Value)
                    {
                        return "部分收货";
                    }
                    else
                    {
                        return "完全收货";
                    }
                }
                else
                {
                    return "未收货";
                }
            }
        }
        public Decimal? Price { get; set; }
        public Decimal? Amount { get; set; }
        //计划完成率
        public Decimal CompleteRate
        {
            get
            {
                if (this.OrderQty != 0 && this.RecQty.HasValue)
                {
                    return this.RecQty.Value / this.OrderQty;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    class ReceiptDet
    {

        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Uom { get; set; }
        //public Decimal UC { get; set; }
        public string ShiftName { get; set; }

        public string ShiftCode { get; set; }

        public string Shift
        {
            get
            {
                return ShiftName + '[' + ShiftCode + ']';
            }
        }
        public DateTime? EffDate { get; set; }
        public Decimal OrderQty { get; set; }
        public Decimal RecQty { get; set; }
        public Decimal RejQty { get; set; }
        public Decimal ScrapQty { get; set; }
        public Decimal OnlineRwoQty { get; set; }
        public string Flow { get; set; }
        public string OrderNo { get; set; }
        public Decimal? Price { get; set; }
        public Decimal? ScrapAmount { get; set; }
        public Decimal? Amount { get; set; }
        public string Type { get; set; }

        public decimal Weight { get; set; }
        public decimal Weight2 { get; set; }
        public decimal Area { get; set; }
        /// <summary>
        /// 退货
        /// </summary>
        public decimal RtnQty { get; set; }
        /// <summary>
        /// 盘差
        /// </summary>
        public decimal DiffQty { get; set; }
        /// <summary>
        /// 工费报废
        /// </summary>
        public decimal Qty { get; set; }

        //在线返修率
        public Decimal OnlineRwoRate
        {
            get
            {
                if (this.OrderQty != 0 && this.OnlineRwoQty != 0)
                {
                    return this.OnlineRwoQty / this.OrderQty;
                }
                else
                {
                    return 0;
                }
            }
        }
        //计划完成率
        public Decimal CompleteRate
        {
            get
            {
                if (this.OrderQty != 0)
                {
                    return this.RecQty / this.OrderQty;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 上线数
        /// </summary>
        public Decimal OnlineQty
        {
            get
            {
                return this.RecQty + this.RejQty + this.ScrapQty;
            }
        }
        public Decimal PassRate2
        {
            get
            {
                if (OnlineQty != 0)
                {
                    return this.RecQty / OnlineQty;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 费损率
        /// </summary>
        public decimal LossReductionRate(bool isDayReport)
        {

            decimal totalQty = (OnlineQty + (isDayReport ? 0 : this.Qty)) * (Price.HasValue ? Price.Value : 0);
            if (totalQty != 0)
            {
                return ((this.ScrapQty + (isDayReport ? 0 : this.Qty)) * (Price.HasValue ? Price.Value : 0)) / totalQty;
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// 合格率
        /// </summary>
        /// <param name="isDayReport"></param>
        /// <returns></returns>
        public decimal PassRate(bool isDayReport)
        {
            decimal totalQty = OnlineQty + (isDayReport ? 0 : Qty);
            if (totalQty != 0)
            {
                return this.RecQty / totalQty;
            }
            else
            {
                return 0;
            }

        }
    }

    public class OrderMstr
    {
        public int DiffHour1 { get; set; }
        public int DiffHour2 { get; set; }



        public string Type { get; set; }
        public string TypeDesc { get; set; }
        public string OrderNo { get; set; }
        public string RefOrderNo { get; set; }
        public string ExtOrderNo { get; set; }
        public string Flow { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime WindowTime { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string SubType { get; set; }
        public string SubTypeDesc { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public string LocFrom { get; set; }
        public string LocTo { get; set; }
        public string DockDesc { get; set; }
        public string BillSettleTerm { get; set; }
        public string Currency { get; set; }
        public string Shift { get; set; }
        public string CancelUser { get; set; }
        public string CreateUser { get; set; }
        public int DiffHour
        {
            get
            {
                if (DiffHour1 > DiffHour2)
                {
                    return DiffHour1;
                }
                else
                {
                    return DiffHour2;
                }
            }
        }
        public string Party
        {
            get
            {
                if (Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    return this.PartyTo;
                }
                else
                {
                    return this.PartyFrom;
                }
            }
        }
        public string RefParty
        {
            get
            {
                if (Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    return this.PartyFrom;
                }
                else
                {
                    return this.PartyTo;
                }
            }
        }
    }

    class IpMstr
    {
        public string IpNo { get; set; }
        public string DockDesc { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DepartTime { get; set; }
        public DateTime ArriveTime { get; set; }
        //public string Status { get; set; }
        public string CreateUser { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public string OrderType { get; set; }
        public string WaybillNo { get; set; }
        public string Carrier { get; set; }
        public string CarrierDesc { get; set; }

        public string Party
        {
            get
            {
                if (OrderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    return this.PartyTo;
                }
                else
                {
                    return this.PartyFrom;
                }
            }
        }
        public string RefParty
        {
            get
            {
                if (OrderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    return this.PartyFrom;
                }
                else
                {
                    return this.PartyTo;
                }
            }
        }
    }

    class IpDet
    {
        public string IpNo { get; set; }
        public string Item { get; set; }
        public string RefItemCode { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public decimal Qty { get; set; }
        public string Uom { get; set; }
        public Decimal UC { get; set; }
        public decimal RecQty { get; set; }
        public decimal DiffQty { get; set; }
        public string DockDesc { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DepartTime { get; set; }
        public DateTime ArriveTime { get; set; }
        public DateTime LastModifyDate { get; set; }
        //public string Status { get; set; }
        public string CreateUser { get; set; }
        public string PartyFrom { get; set; }
        public string PartyTo { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public string OrderType { get; set; }

        public string Party
        {
            get
            {
                if (OrderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    return this.PartyTo;
                }
                else
                {
                    return this.PartyFrom;
                }
            }
        }
        public string RefParty
        {
            get
            {
                if (OrderType == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION)
                {
                    return this.PartyFrom;
                }
                else
                {
                    return this.PartyTo;
                }
            }
        }

        public string Status
        {
            get
            {
                if (DiffQty == Qty)
                {
                    return "全部差异";
                }
                else if (Qty != RecQty)
                {
                    return "部分差异";
                }
                else if (DiffQty == 0)
                {
                    return "无差异";
                }
                else if (Qty > 0 && Qty < RecQty)
                {
                    return "超收";
                }
                return string.Empty;
            }
        }
    }

    class Trench
    {
        public string Code { get; set; }
        public decimal Value { get; set; }
        public int Level { get; set; }
        public string Num { get; set; }
        //public int Seq { get; set; }
    }

    public class ElectricPlatingInfo
    {
        public int ID { get; set; }
        public DateTime 入槽时间 { get; set; }
        public DateTime? 出槽时间 { get; set; }
        public int 类型 { get; set; }

        public int 设定电流 { get; set; }
        public int 实际电流 { get; set; }
        public Decimal? 实际电压 { get; set; }
        /*
       public int 备注1 { get; set; }
       public int 备注2 { get; set; }
       */
        public int 工艺 { get; set; }
        public string 序号 { get; set; }
        public string 描述 { get; set; }
        public Decimal? 电阻 { get; set; }
        public int Seq { get; set; }
        public int No { get; set; }
    }

    class OrderItem
    {
        public string OrderNo { get; set; }
        public string Craft { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public Decimal? RateQty { get; set; }
        public Decimal? Angle { get; set; }
        public Decimal? Area { get; set; }
        public string SX
        {
            get
            {
                if (RateQty.HasValue && Angle.HasValue && Angle.Value != 0)
                {
                    return (RateQty.Value / Angle.Value).ToString("0.########");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }

    class ElectricPlatingInfoAll
    {
        public string 生产单号 { get; set; }
        public int 工艺 { get; set; }
        public string 槽位 { get; set; }
        public int 总数 { get; set; }
    }

    public class TimelyRate
    {
        public string 年月 { get; set; }
        public string 代码 { get; set; }
        public string 名称 { get; set; }
        public int 单数 { get; set; }
        public int 不及时单数 { get; set; }
        public int 及时单数 { get; set; }
        public decimal 及时率 { get; set; }
        public decimal 完成率 { get; set; }
    }

    class InspectResult
    {
        public int ComfirmCount { get; set; }
        public int InspNoCount { get; set; }
        public string LocFrom { get; set; }
        public string RejLoc { get; set; }
        public string InspLoc { get; set; }
        public string Item { get; set; }
        public string Uom { get; set; }
        public Decimal UC { get; set; }
        public Decimal InspQty { get; set; }
        public Decimal QualifyQty { get; set; }
        public Decimal RejectQty { get; set; }
        public Decimal PendingQualifyQty { get; set; }
        public Decimal PendingRejectQty { get; set; }
        public Decimal CurrQualifyQty { get; set; }
        public Decimal CurrRejectQty { get; set; }

        //计划完成率
        public Decimal CompleteRate
        {
            get
            {
                if (this.InspQty != 0)
                {
                    return (this.QualifyQty + RejectQty) / this.InspQty;
                }
                else
                {
                    return 0;
                }
            }
        }

        //合格率
        public Decimal PassRate
        {
            get
            {
                if (InspQty != 0)
                {
                    return this.QualifyQty / InspQty;
                }
                else
                {
                    return 0;
                }
            }
        }

        //本次合格率
        public Decimal CurrPassRate
        {
            get
            {
                decimal total = this.CurrQualifyQty + this.CurrRejectQty;
                if (total != 0)
                {
                    return this.CurrQualifyQty / total;
                }
                else
                {
                    return 0;
                }
            }
        }

    }

    class ActBill
    {
        public string TransType { get; set; }
        public string OrderNo { get; set; }
        public string RecNo { get; set; }
        public string ExtRecNo { get; set; }
        public string BillAddr { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Uom { get; set; }
        public Decimal UC { get; set; }
        public DateTime EffDate { get; set; }
        public Decimal Qty { get; set; }
        public Decimal? Price { get; set; }
        public Decimal? Amount { get; set; }
    }

    public class Plans
    {
        public string Item { get; set; }
        public Decimal Qty { get; set; }
    }
    public class TrWoGap
    {
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string WoUom { get; set; }
        public Decimal? WoQty { get; set; }
        public string TrUom { get; set; }
        public Decimal? TrQty { get; set; }

        public Decimal GapQty { get; set; }

    }
    public class LocLotDet
    {
        public string Region { get; set; }
        public string Location { get; set; }

        public string LocationName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDesc { get; set; }
        public string Item
        {
            get { return this.ItemDesc + "[" + this.ItemCode + "]"; }
        }
        public string ItemCategoryCode { get; set; }
        public string ItemCategoryDesc { get; set; }
        public string Type { get; set; }
        public string Uom { get; set; }
        public Decimal UC { get; set; }
        public DateTime CreateDate { get; set; }
        //public string HuId { get; set; }
        public Decimal Qty { get; set; }
        public Decimal CsQty { get; set; }
        public Decimal NmlQty { get; set; }
        public Decimal? Price { get; set; }
        public Decimal? Amount { get; set; }
        public Decimal? Amount7 { get; set; }
        public Decimal? Amount30 { get; set; }
        public Decimal? Amount60 { get; set; }

        public Decimal? Amount90 { get; set; }

        public Decimal SafeStock { get; set; }
        public Decimal MaxStock { get; set; }
        //public string Bin { get; set; }
        //public string LotNo { get; set; }
        //public Boolean IsCS { get; set; }
    }

    class BillAging
    {
        public string Party { get; set; }
        public string TransType { get; set; }
        public string BillAddr { get; set; }
        public string BillAddrDesc { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Uom { get; set; }
        public Decimal UC { get; set; }
        //public Decimal Qty1 { get; set; }
        public Decimal Qty2 { get; set; }
        public Decimal Qty3 { get; set; }
        public Decimal Qty4 { get; set; }
        public Decimal Qty5 { get; set; }
        public Decimal Qty6 { get; set; }
        public Decimal Qty7 { get; set; }
        public Decimal Qty8 { get; set; }
        public Decimal Qty9 { get; set; }
        public Decimal Qty10 { get; set; }
        public Decimal Qty11 { get; set; }

        public Decimal Qty
        {
            get
            {
                return Qty10 + Qty11 + Qty2 + Qty3 + Qty4 + Qty5 + Qty6 + Qty7 + Qty8 + Qty9;
            }
        }
        //public Decimal? Amount1 { get; set; }
        public Decimal? Amount2 { get; set; }
        public Decimal? Amount3 { get; set; }
        public Decimal? Amount4 { get; set; }
        public Decimal? Amount5 { get; set; }
        public Decimal? Amount6 { get; set; }
        public Decimal? Amount7 { get; set; }
        public Decimal? Amount8 { get; set; }
        public Decimal? Amount9 { get; set; }
        public Decimal? Amount10 { get; set; }
        public Decimal? Amount11 { get; set; }
        public Decimal? Amount5T
        {
            get
            {
                return Amount11 + Amount2 + Amount3 + Amount4 + Amount5 + Amount6 + Amount7 + Amount8 + Amount9;
            }
        }
        public Decimal? Amount30
        {
            get
            {
                return Amount2 + Amount3 + Amount4 + Amount5 + Amount6 + Amount7 + Amount8 + Amount9;
            }
        }

        public Decimal? Amount60
        {
            get
            {
                return Amount3 + Amount4 + Amount5 + Amount6 + Amount7 + Amount8 + Amount9;
            }
        }

        public Decimal? Amount90
        {
            get
            {
                return Amount4 + Amount5 + Amount6 + Amount7 + Amount8 + Amount9;
            }
        }
        public Decimal? Amount
        {
            get
            {
                return Amount10 + Amount11 + Amount2 + Amount3 + Amount4 + Amount5 + Amount6 + Amount7 + Amount8 + Amount9;
            }
        }
    }

    public class MiscOrder
    {
        public string TypeDesc { get; set; }
        public string Type { get; set; }
        public string OrderNo { get; set; }
        public string Location { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EffDate { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }

    }

    public class MiscOrderDetail
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string Item { get; set; }
        public Decimal Qty { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public Decimal? Price { get; set; }
        public Decimal? Amount { get; set; }
    }

    class CycleCountMstr
    {
        public string Type { get; set; }
        public string OrderNo { get; set; }
        public string Location { get; set; }
        public string CompleteUser { get; set; }
        public DateTime CompleteDate { get; set; }
        public DateTime EffDate { get; set; }
        public string Status { get; set; }
        public string PhyCntGroupBy { get; set; }
    }

    class CycleCountResult
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string Item { get; set; }

        public Decimal Qty { get; set; }
        public string HuId { get; set; }
        public string LotNo { get; set; }
        public Decimal InvQty { get; set; }
        public Decimal DiffQty { get; set; }
        public string Bin { get; set; }
        public string Location { get; set; }
        public DateTime ProcessDate { get; set; }
        public string ProcessUser { get; set; }
        public Decimal? Price { get; set; }
        public Decimal? Amount { get; set; }
        public string Type
        {
            get
            {
                if (DiffQty > 0)
                {
                    return "盘盈";
                }
                else if (DiffQty == 0)
                {
                    return "盘正";
                }
                else
                {
                    return "盘亏";
                }
            }
        }
    }

    class UnpCyc
    {
        public string Flow { get; set; }
        public string Type { get; set; }
        public string Item { get; set; }

        public string Uom { get; set; }
        public string Region { get; set; }
        public string ItemDesc { get; set; }
        public decimal Weight { get; set; }
        public decimal Weight2 { get; set; }
        public decimal Area { get; set; }

        public Decimal? Price { get; set; }

        public Decimal Qty { get; set; }
        //public Decimal? DiffQty { get; set; }

        public DateTime EffDate { get; set; }

        public Decimal? Amount { get; set; }
    }

    public class Bill
    {
        public string BillNo { get; set; }
        public string Party { get; set; }
        public string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public string ExtBillNo { get; set; }
        public string RefBillNo { get; set; }
        public string BillAddr { get; set; }
        public string Currency { get; set; }
        public decimal Discount { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyUser { get; set; }
        public string BillType { get; set; }
    }
    public class BillDet
    {
        public string BillNo { get; set; }
        public string Status { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Uom { get; set; }
        public decimal UC { get; set; }
        public decimal BilledQty { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount
        {
            get
            {
                return this.BilledQty * this.UnitPrice - this.Discount;
            }
        }
    }


    /// <summary>
    /// 加料统计
    /// </summary>
    class LoadingStatistics
    {
        public string Loc { get; set; }
        public string Item { get; set; }
        public string ItemDesc { get; set; }
        public string Uom { get; set; }


        public int ShiftPos(string[] ShiftATime)
        {
            DateTime startDate = DateTime.Parse(this.EffDate.ToString("yyyy-MM-dd " + ShiftATime[0]));
            DateTime endDate = DateTime.Parse(this.EffDate.ToString("yyyy-MM-dd " + ShiftATime[1]));
            if (this.EffDate >= startDate && this.EffDate < endDate)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public string[] ShiftATime { get; set; }
        public string[] ShiftBTime { get; set; }
        public decimal Area { get; set; }
        public decimal Qty { get; set; }
        public DateTime EffDate { get; set; }
        public int Day
        {
            get
            {
                return this.EffDate.Day;
            }
        }
    }
    class Shift
    {
        public string Name;
        public string[] ShiftTime;
    }

    #endregion

}
