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
using com.Sconit.Entity.Distribution;
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
    public class OverASNRepJob : RepJob
    {

        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            string key = "����δȷ�ϻص�";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;����δȷ�ϻص�");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;1. ����δȷ�ϻص�����");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;2. ��ϸ�г�����δ�رյķ������嵥");

            int asnExpiredDays = int.Parse(entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[] { "ASNExpiredDays" })[0].Value);

            DateTime endDate = DateTime.Now.AddDays(-1 * asnExpiredDays);

            SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_OVERASNREP, desc.ToString(), null, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            bool result1 = ProcessASN(key, workbook, startDate, endDate);

            //bool result2 =ProcessSOGap(key, workbook,startDate,endDate);

            return result1;
        }

        [Transaction(TransactionMode.Unspecified)]
        protected virtual bool ProcessASN(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                #region ��ȡ����ͷ����
                StringBuilder ipSql = new StringBuilder();
                ipSql.Append(@"select i.IpNo,i.DockDesc, ");//,c1.Desc1 Status
                ipSql.Append(@"i.CreateDate,i.DepartTime,i.ArriveTime, ");
                ipSql.Append(@"u.USR_FirstName+u.USR_LastName+'['+u.USR_Code+']' CreateUser, ");
                ipSql.Append(@"p1.Name+'['+p1.Code+']' PartyFrom,p2.Name+'['+p2.Code+']' PartyTo, ");
                ipSql.Append(@"pa1.Address+'['+pa1.Code+']' ShipFrom, pa2.Address+'['+pa2.Code+']' ShipTo ");
                ipSql.Append(@"from ipmstr i  ");
                //ipSql.Append(@"join ipdet d on i.ipno=d.ipno   ");
                ipSql.Append(@"join Party p1 on p1.Code=i.PartyFrom  ");
                ipSql.Append(@"join Party p2 on p2.Code=i.PartyTo ");
                ipSql.Append(@"join PartyAddr pa1 on i.ShipFrom=pa1.Code and pa1.AddrType= 'ShipAddr' ");
                ipSql.Append(@"join PartyAddr pa2 on i.ShipTo=pa2.Code and pa2.AddrType= 'ShipAddr' ");
                //ipSql.Append(@"join CodeMstr c1 on i.Status=c1.CodeValue and c1.Code = '" + BusinessConstants.CODE_MASTER_STATUS + "' ");
                ipSql.Append(@"join ACC_User u on u.USR_Code=i.CreateUser ");
                ipSql.Append(@"where i.OrderType='" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION + "'  ");
                ipSql.Append(@"and i.Status ='" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE + "' ");
                ipSql.Append(@"and i.Type='" + BusinessConstants.CODE_MASTER_INPROCESS_LOCATION_TYPE_VALUE_NORMAL + "'  ");
                ipSql.Append(@"and i.ArriveTime <'" + endDate.Value + "'  ");
                ipSql.Append(@"order by i.PartyTo ASC,i.ArriveTime ASC,i.IpNo ASC ");

                DataSet ipDS = sqlHelperMgrE.GetDatasetBySql(ipSql.ToString());
                List<IpMstr> ipList = IListHelper.DataTableToList<IpMstr>(ipDS.Tables[0]);

                #endregion

                if (ipList != null && ipList.Count > 0)
                {
                    #region ��ȡ��ϸ����
                    StringBuilder ipDetSql = new StringBuilder();
                    ipDetSql.Append(@"select d.IpNo,i.Code+'['+ i.Desc1+']' Item,d.RefItemCode, ");
                    ipDetSql.Append(@"       d.HuId,d.LotNo,d.Qty, d.Uom,d.UC ");//d.CustomerItemCode,
                    ipDetSql.Append(@"from ipmstr ip ");
                    ipDetSql.Append(@"join ipdet d on ip.ipno=d.ipno ");
                    ipDetSql.Append(@"join Item i on d.Item=i.Code ");
                    ipDetSql.Append(@"where ip.OrderType='" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION + "'  ");
                    ipDetSql.Append(@"and ip.Status ='" + BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE + "' ");
                    ipDetSql.Append(@"and ip.Type='" + BusinessConstants.CODE_MASTER_INPROCESS_LOCATION_TYPE_VALUE_NORMAL + "'  ");
                    ipDetSql.Append(@"and ip.ArriveTime <'" + endDate.Value + "'  ");
                    ipDetSql.Append(@"order by ip.PartyTo ASC,ip.ArriveTime ASC,ip.IpNo ASC,d.Id ASC");
                    DataSet ipDetDS = sqlHelperMgrE.GetDatasetBySql(ipDetSql.ToString());
                    List<IpDet> ipDetList = IListHelper.DataTableToList<IpDet>(ipDetDS.Tables[0]);

                    #endregion

                    #region ��ʼ��Sheet

                    ISheet sheet1 = workbook.CreateSheet(key);

                    for (int i = 0; i <= 7; i++)
                    {
                        sheet1.AutoSizeColumn(i);
                        sheet1.SetDefaultColumnStyle(i, cellStyle);
                    }
                    sheet1.SetColumnWidth(0, 15 * 256);
                    sheet1.SetColumnWidth(1, 28 * 256);
                    sheet1.SetColumnWidth(2, 15 * 256);
                    sheet1.SetColumnWidth(3, 20 * 256);
                    sheet1.SetColumnWidth(4, 28 * 256);
                    sheet1.SetColumnWidth(5, 20 * 256);
                    sheet1.SetColumnWidth(6, 30 * 256);
                    sheet1.SetColumnWidth(7, 10 * 256);
                    //sheet1.SetColumnWidth(8, 12 * 256);

                    int rownum = 0;
                    int colnum = 0;

                    #endregion

                    colnum = 0;
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "δȷ�ϻص���");
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipList.Count() + " ��", headStyle);

                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "δȷ����ϸ��");
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDetList.Count() + " ��", headStyle);
                    rownum++;

                    #region �����ͷ
                    colnum = 0;

                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "�ͻ�����", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����ʱ��", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "Ԥ���ʹ�", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "�ͻ�", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "������ַ", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "�ջ���ַ", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "�ͻ�����", headStyle);
                    XlsHelper.SetRowCell(sheet1, rownum, colnum++, "������", headStyle);

                    #endregion

                    #region �������
                    DateTime? ArriveTime = null;
                    string partyTo = string.Empty;
                    foreach (var ip in ipList)
                    {
                        rownum++;
                        colnum = 0;
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.IpNo);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.DepartTime.ToString("yyyy-MM-dd HH:mm"));
                        if (!ArriveTime.HasValue || ArriveTime.Value.ToString("yyyy-MM-dd").CompareTo(ip.ArriveTime.ToString("yyyy-MM-dd")) != 0
                               || string.IsNullOrEmpty(partyTo) || partyTo != ip.PartyTo)
                        {
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.ArriveTime.ToString("yyyy-MM-dd HH:mm"), cellStyleColor);
                        }
                        else
                        {
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.ArriveTime.ToString("yyyy-MM-dd HH:mm"));
                        }
                        ArriveTime = ip.ArriveTime;
                        partyTo = ip.PartyTo;

                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.PartyFrom);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.PartyTo);

                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.ShipFrom);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.ShipTo);

                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.DockDesc);
                        XlsHelper.SetRowCell(sheet1, rownum, colnum++, ip.CreateUser);


                        #region ������ϸ
                        var ipDetTList = ipDetList.Where(d => d.IpNo == ip.IpNo).ToList<IpDet>();
                        if (ipDetTList != null && ipDetTList.Count > 0)
                        {
                            colnum = 1;
                            rownum++;

                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "�ο����Ϻ�", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "��λ", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����װ", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����", cellStyleDet);
                            XlsHelper.SetRowCell(sheet1, rownum, colnum++, "����", cellStyleDet);

                            foreach (var ipDet in ipDetTList)
                            {
                                colnum = 1;
                                rownum++;
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDet.Item);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDet.RefItemCode);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDet.Uom);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDet.UC.ToString("0.########"));
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDet.LotNo);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDet.HuId);
                                XlsHelper.SetRowCell(sheet1, rownum, colnum++, ipDet.Qty.ToString("0.########"));
                                colnum++;
                            }

                            sheet1.GroupRow(rownum - ipDetTList.Count, rownum);
                            sheet1.SetRowGroupCollapsed(rownum, true);
                        }
                        #endregion
                    }

                    sheet1.GroupRow(rownum - ipList.Count * 2 - ipDetList.Count, rownum + 1);
                    sheet1.SetRowGroupCollapsed(rownum + 1, true);

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
    }
}



#region Extend Class


namespace com.Sconit.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class OverASNRepJob : com.Sconit.Service.Batch.Job.OverASNRepJob
    {
        public OverASNRepJob()
        {
        }
    }
}

#endregion
