using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Entity;
using com.Sconit.Service;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Utility;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity;
using com.Sconit.Service.Ext;
using System.Data;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class MouldMgr : IMouldMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public ISqlHelperMgrE sqlHelperMgrE { get; set; }
        public INumberControlMgrE numberControlMgrE { get; set; }

        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public ITaskSubTypeMgrE taskSubTypeMgrE { get; set; }

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        #region Customized Methods

        public IList<Mould> GetMouldList(string purchaseContractCode)
        {
            return GetMouldList(null, purchaseContractCode);
        }

        public IList<Mould> GetMouldList(string code, string supplierContractNo)
        {
            if (string.IsNullOrEmpty(supplierContractNo)) return null;
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Mould));
            if (!string.IsNullOrEmpty(code))
            {
                criteria.Add(Expression.Not(Expression.Eq("Code", code)));
            }
            criteria.Add(Expression.Eq("SupplierContractNo", supplierContractNo));

            IList<Mould> MouldList = criteriaMgrE.FindAll<Mould>(criteria);
            return MouldList;
        }


        [Transaction(TransactionMode.Requires)]
        public void CreateMould(Mould mould)
        {
            genericMgr.Create(mould);
        }
        [Transaction(TransactionMode.Unspecified)]
        public Mould LoadMould(string code)
        {
            return genericMgr.FindById<Mould>(code);
        }
        [Transaction(TransactionMode.Requires)]
        public void UpdateMould(Mould mould)
        {
            genericMgr.CleanSession();
            genericMgr.Update(mould);
        }
        [Transaction(TransactionMode.Requires)]
        public void DeleteMould(Mould mould)
        {
            this.DeleteMould(mould.Code);
        }
        [Transaction(TransactionMode.Requires)]
        public void DeleteMould(string code)
        {
            //删除明细
            this.genericMgr.Delete("from MouldDetail d where d.Code ='" + code + "'");

            genericMgr.DeleteById<Mould>(code);

        }
        [Transaction(TransactionMode.Requires)]
        public string CopyMould(string code, User user)
        {
            #region 复制头
            Mould mould = this.LoadMould(code);

            Mould newMould = new Mould();
            /*
            CloneHelper.CopyProperty(mould, newMould, new string[] { "Version","Code", "FCID", "Supplier", "SupplierName", "SOContractNo","POContractNo",
                                                                            "POPayDays", "SOAmount", "SOBilledAmount", "SOPayAmount", "SOAmount1", "SOBillDate1", "SOBilledAmount1", "SOPayAmount1", "SOAmount2", "SOBillDate2", "SOBilledAmount2", "SOPayAmount2", "SOAmount3", "SOBillDate3", "SOBilledAmount3", "SOPayAmount3", "POAmount", "POBilledAmount", "POPayAmount", "POAmount1", "SupplierBillDate1", "POBilledAmount1", "POPayAmount1", "POAmount2", "SupplierBillDate2", "POBilledAmount2", "POPayAmount2", "POAmount3", "SupplierBillDate3", "POBilledAmount3", "POPayAmount3" ,
                                                                             "SOCompleteDate","SOCompleteUser","SOCompleteUserNm","POCompleteDate","POCompleteUser","POCompleteUserNm","CloseDate","CloseUser","CloseUserNm"}, true);

            */
            //复制销售和采购数据，不复制供应商的信息
            CloneHelper.CopyProperty(mould, newMould, new string[] { "Type" , "Desc1", "PrjCode", "PrjDesc", "FCID",
                                                                            "QS",
                                                                            "SOUser", "SOUserNm", "Customer", "CustomerName","SOContractNo","SOAmount","SOBilledAmount","SOPayAmount","SOAmount1","SOBilledAmount1","SOPayAmount1","SOBillDate1","SOPayDate1","SOAmount2","SOBilledAmount2","SOPayAmount2","SOBillDate2","SOPayDate2","SOAmount3","SOBilledAmount3","SOPayAmount3","SOBillDate3","SOPayDate3","SOAmount4","SOBilledAmount4","SOPayAmount4","SOBillDate4","SOPayDate4",
                                                                            "POAmount","POBilledAmount","POPayAmount","POAmount1","POBilledAmount1","POPayAmount1","POAmount2","POBilledAmount2","POPayAmount2","POAmount3","POBilledAmount3","POPayAmount3","POAmount4","POBilledAmount4","POPayAmount4",
                                                                            });


            newMould.Code = numberControlMgrE.GenerateNumber(BusinessConstants.CODE_PREFIX_BILL);
            newMould.CreateDate = DateTime.Now;
            newMould.CreateUserNm = user.Name;
            newMould.CreateUser = user.Code;
            newMould.LastModifyDate = DateTime.Now;
            newMould.LastModifyUser = user.Code;
            newMould.LastModifyUserNm = user.Name;
            newMould.Status = ISIConstants.CODE_MASTER_PSI_BILL_STATUS_CREATE;
            this.CreateMould(newMould);
            return newMould.Code;
            #endregion

        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<Mould> GetAllMould()
        {
            return this.genericMgr.FindAll<Mould>();
        }

        [Transaction(TransactionMode.Unspecified)]
        public void RemindBill()
        {
            DataSet dataSet = sqlHelperMgrE.GetDatasetByStoredProcedure("USP_Rep_Mould", null);
            var mouldList = IListHelper.DataTableToList<Mould>(dataSet.Tables[0]);

            if (mouldList != null && mouldList.Count > 0)
            {
                DateTime now = DateTime.Now;
                DateTime day2 = now.AddDays(1);
                string separator = ISIConstants.EMAIL_SEPRATOR;
                var remindEmail = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_VALUE_REMINDPSIBILL });
                var remindEmail2 = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_VALUE_REMINDPSIBILL2 });
                IList<string> codes = new List<string>();
                for (int i = 0; i < mouldList.Count; i++)
                {
                    try
                    {
                        var mould = mouldList[i];
                        StringBuilder subject = new StringBuilder();
                        StringBuilder body = new StringBuilder("您好！");
                        int typeRemind = 0;
                        if (mould.Status != ISIConstants.CODE_MASTER_PSI_BILL_STATUS_POCOMPLETE && (mould.SupplierBillDate1.HasValue || mould.SupplierBillDate2.HasValue || mould.SupplierBillDate4.HasValue || mould.SupplierBillDate3.HasValue || mould.SupplierPayDate1.HasValue || mould.SupplierPayDate2.HasValue || mould.SupplierPayDate4.HasValue || mould.SupplierPayDate3.HasValue))
                        {
                            //采购开票，采购人员
                            if (mould.SupplierBillDate1.HasValue && mould.SupplierBillDate1.Value >= now && mould.SupplierBillDate1.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购首付款到开票时间了！");
                                body.Append(separator + "&nbsp;&nbsp;采购首付款到开票时间了，" + mould.SupplierBillDate1.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 1;
                            }
                            if (mould.SupplierBillDate2.HasValue && mould.SupplierBillDate2.Value >= now && mould.SupplierBillDate2.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购中期款1到开票时间了");
                                body.Append(separator + "&nbsp;&nbsp;采购中期款1到开票时间了，" + mould.SupplierBillDate2.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 2;
                            }
                            if (mould.SupplierBillDate4.HasValue && mould.SupplierBillDate4.Value >= now && mould.SupplierBillDate4.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购中期款2到开票时间了");
                                body.Append(separator + "&nbsp;&nbsp;采购中期款2到开票时间了，" + mould.SupplierBillDate4.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 3;
                            }
                            if (mould.SupplierBillDate3.HasValue && mould.SupplierBillDate3.Value >= now && mould.SupplierBillDate3.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购尾款到开票时间了");
                                body.Append(separator + "&nbsp;&nbsp;采购尾款到开票时间了，" + mould.SupplierBillDate3.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 4;
                            }

                            //采购付款，通知采购、出纳
                            if (mould.SupplierPayDate1.HasValue && mould.SupplierPayDate1.Value >= now && mould.SupplierPayDate1.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购首付款到付款时间了");
                                body.Append(separator + "&nbsp;&nbsp;采购首付款到付款时间了，" + mould.SupplierPayDate1.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 5;
                            }
                            if (mould.SupplierPayDate2.HasValue && mould.SupplierPayDate2.Value >= now && mould.SupplierPayDate2.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购中期款1到付款时间了");
                                body.Append(separator + "&nbsp;&nbsp;采购中期款1到付款时间了，" + mould.SupplierPayDate2.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 6;
                            }
                            if (mould.SupplierPayDate4.HasValue && mould.SupplierPayDate4.Value >= now && mould.SupplierPayDate4.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购中期款2到付款时间了");
                                body.Append(separator + "&nbsp;&nbsp;采购中期款2到付款时间了，" + mould.SupplierPayDate4.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 7;
                            }
                            if (mould.SupplierPayDate3.HasValue && mould.SupplierPayDate3.Value >= now && mould.SupplierPayDate3.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 采购尾款到付款时间了");
                                body.Append(separator + "&nbsp;&nbsp;采购尾款到付款时间了，" + mould.SupplierPayDate3.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 8;
                            }
                        }
                        if ((i == 0 || !(mould.Type == mouldList[i - 1].Type && mould.PrjCode == mouldList[i - 1].PrjCode)) && //销售因为是一对多所以只发一次
                            mould.Status != ISIConstants.CODE_MASTER_PSI_BILL_STATUS_SOCOMPLETE && (mould.SOBillDate1.HasValue || mould.SOBillDate2.HasValue || mould.SOBillDate3.HasValue || mould.SOPayDate1.HasValue || mould.SOPayDate2.HasValue || mould.SOPayDate3.HasValue))
                        {
                            if (mould.SOBillDate1.HasValue && mould.SOBillDate1.Value >= now && mould.SOBillDate1.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售预收款到开票时间了！");
                                body.Append(separator + "&nbsp;&nbsp;销售预收款到开票时间了，" + mould.SOBillDate1.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 9;
                            }
                            if (mould.SOBillDate2.HasValue && mould.SOBillDate2.Value >= now && mould.SOBillDate2.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售中期款1到开票时间了");
                                body.Append(separator + "&nbsp;&nbsp;销售中期款1到开票时间了，" + mould.SOBillDate2.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 10;
                            }
                            if (mould.SOBillDate4.HasValue && mould.SOBillDate4.Value >= now && mould.SOBillDate4.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售中期款2到开票时间了");
                                body.Append(separator + "&nbsp;&nbsp;销售中期款2到开票时间了，" + mould.SOBillDate4.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 11;
                            }
                            if (mould.SOBillDate3.HasValue && mould.SOBillDate3.Value >= now && mould.SOBillDate3.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售尾款到开票时间了");
                                body.Append(separator + "&nbsp;&nbsp;销售尾款到开票时间了，" + mould.SOBillDate3.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 12;
                            }
                            if (mould.SOPayDate1.HasValue && mould.SOPayDate1.Value >= now && mould.SOPayDate1.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售预收款到收款时间了");
                                body.Append(separator + "&nbsp;&nbsp;销售预收款到收款时间了，" + mould.SOBillDate1.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 13;
                            }
                            if (mould.SOPayDate2.HasValue && mould.SOPayDate2.Value >= now && mould.SOPayDate2.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售中期款1到收款时间了");
                                body.Append(separator + "&nbsp;&nbsp;销售中期款1到收款时间了，" + mould.SOPayDate2.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 14;
                            }
                            if (mould.SOPayDate4.HasValue && mould.SOPayDate4.Value >= now && mould.SOPayDate4.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售中期款2到收款时间了");
                                body.Append(separator + "&nbsp;&nbsp;销售中期款2到收款时间了，" + mould.SOPayDate4.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 15;
                            }
                            if (mould.SOPayDate3.HasValue && mould.SOPayDate3.Value >= now && mould.SOPayDate3.Value <= day2)
                            {
                                subject = new StringBuilder("模具 " + mould.Code + " 销售尾款到收款时间了");
                                body.Append(separator + "&nbsp;&nbsp;销售尾款到收款时间了，" + mould.SOPayDate3.Value.ToString("yyyy-MM-dd"));
                                typeRemind = 16;
                            }
                        }
                        if (typeRemind != 0)
                        {
                            List<string> userCodes = new List<string>();
                            IList<string> userCodes2 = new List<string>();

                            //项目经理
                            string projectUser = taskSubTypeMgrE.LoadTaskSubType(mould.PrjCode).AssignUser;
                            if (!string.IsNullOrEmpty(projectUser))
                            {
                                userCodes.AddRange(projectUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).Distinct());
                            }
                            //供应商开票：通知采购工程师、模具工程师、项目经理、出纳和设备经理
                            string emails = string.Empty;
                            string emails2 = string.Empty;
                            if (typeRemind >= 1 && typeRemind <= 4)
                            {
                                userCodes.Add(mould.POUser);                                
                                emails = this.userSubscriptionMgrE.FindEmail(userCodes.ToArray());
                                if (!string.IsNullOrEmpty(remindEmail))
                                {
                                    emails += ";" + remindEmail;
                                }

                                userCodes2.Add(mould.MouldUser);
                                emails2 = this.userSubscriptionMgrE.FindEmail(userCodes2.ToArray());
                                if (!string.IsNullOrEmpty(remindEmail2))
                                {
                                    emails2 += ";" + remindEmail2;
                                }
                            }
                            //供应商付款：通知采购工程师、模具工程师、项目经理、出纳和设备经理
                            if (typeRemind >= 5 && typeRemind <= 8)
                            {
                                userCodes.Add(mould.POUser);
                                emails = this.userSubscriptionMgrE.FindEmail(userCodes.ToArray());
                                if (!string.IsNullOrEmpty(remindEmail))
                                {
                                    emails += ";" + remindEmail;
                                }

                                userCodes2.Add(mould.MouldUser);
                                emails2 = this.userSubscriptionMgrE.FindEmail(userCodes2.ToArray());
                                if (!string.IsNullOrEmpty(remindEmail2))
                                {
                                    emails2 += ";" + remindEmail2;
                                }
                            }
                            //销售开票：通知销售工程师、项目经理、出纳和设备经理
                            if (typeRemind >= 9 && typeRemind <= 13)
                            {
                                userCodes.Add(mould.CreateUser);
                                userCodes.Add(mould.SOUser);
                                emails = this.userSubscriptionMgrE.FindEmail(userCodes.ToArray());

                                if (!string.IsNullOrEmpty(remindEmail))
                                {
                                    emails += ";" + remindEmail;
                                }

                                if (!string.IsNullOrEmpty(remindEmail2))
                                {
                                    emails2 += ";" + remindEmail2;
                                }
                            }
                            //销售收款：通知销售工程师
                            if (typeRemind >= 14 && typeRemind <= 16)
                            {
                                userCodes.Add(mould.CreateUser);
                                userCodes.Add(mould.SOUser);
                                emails = this.userSubscriptionMgrE.FindEmail(userCodes.ToArray());
                            }

                            //模具基本信息
                            MouldInfo(mould, body);

                            this.userSubscriptionMgrE.Remind(subject.ToString(), body, emails);

                            //模具工程师、设备经理
                            this.userSubscriptionMgrE.Remind(subject.ToString(), subject, emails2);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e.Message, e);
                    }
                }
            }
        }

        private void MouldInfo(Mould mould, StringBuilder body)
        {
            string separator = ISIConstants.EMAIL_SEPRATOR;
            body.Append(separator);
            body.Append(separator);
            body.Append(separator);
            body.Append("系统代码：" + mould.FCID + separator);
            body.Append("编号：" + mould.Code + separator);
            body.Append("状态：" + languageMgrE.ProcessLanguage("${ISI.Status." + mould.Status + "}", BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN) + separator);
            body.Append("描述：" + mould.Desc1 + separator);
            if (mould.QS.HasValue)
            {
                body.Append("QS：" + mould.QS.Value.ToString("0.########") + separator);
            }
            body.Append("数量：" + mould.Qty + separator);
            body.Append("项目：" + mould.Project + separator);            
            body.Append("<hr/>");
            body.Append("客户：" + mould.CustomerDesc + separator);
            body.Append("客户合同号：" + mould.FCID + separator);
            body.Append("客户工程师：" + mould.SOUserDesc + separator);

            body.Append("供应商：" + mould.SupplierDesc + separator);
            body.Append("采购合同号：" + mould.FCID + separator);
            body.Append("采购工程师：" + mould.POUserDesc + separator);
            body.Append("模具工程师：" + mould.POUserDesc + separator);

            body.Append("<hr/>");
            if (mould.SOAmount.HasValue)
            {
                body.Append("客户合同金额（含税）：" + mould.SOAmount.Value.ToString("0.########") + separator);
            }
            if (mould.POAmount.HasValue)
            {
                body.Append("采购合同金额（含税）：" + mould.POAmount.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierAmount.HasValue)
            {
                body.Append("供应商合同金额（含税）：" + mould.SupplierAmount.Value.ToString("0.########") + separator);
            }
            if (mould.SOBilledAmount.HasValue)
            {
                body.Append("客户已开票金额（含税）：" + mould.SOBilledAmount.Value.ToString("0.########") + separator);
            }
            if (mould.POBilledAmount.HasValue)
            {
                body.Append("采购已开票金额（含税）：" + mould.POBilledAmount.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierBilledAmount.HasValue)
            {
                body.Append("供应商已开票金额（含税）：" + mould.SupplierBilledAmount.Value.ToString("0.########") + separator);
            }
            if (mould.SOPayAmount.HasValue)
            {
                body.Append("客户已付款金额（含税）：" + mould.SOPayAmount.Value.ToString("0.########") + separator);
            }
            if (mould.POPayAmount.HasValue)
            {
                body.Append("采购已付款金额（含税）：" + mould.POPayAmount.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierPayAmount.HasValue)
            {
                body.Append("供应商已付款金额（含税）：" + mould.SupplierPayAmount.Value.ToString("0.########") + separator);
            }
            body.Append("<hr/>");
            if (mould.SOAmount1.HasValue)
            {
                body.Append("客户首付款（含税）：" + mould.SOAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.POAmount1.HasValue)
            {
                body.Append("采购首付款（含税）：" + mould.POAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierAmount1.HasValue)
            {
                body.Append("供应商首付款（含税）：" + mould.SupplierAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.SOBillDate1.HasValue)
            {
                body.Append("计划客户首付款开票日期：" + mould.SOBillDate1.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SupplierBillDate1.HasValue)
            {
                body.Append("计划供应商首付款开票日期：" + mould.SupplierBillDate1.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOPayDate1.HasValue)
            {
                body.Append("计划客户首付款付款日期：" + mould.SOPayDate1.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SupplierPayDate1.HasValue)
            {
                body.Append("计划供应商首付款付款日期：" + mould.SupplierPayDate1.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOBilledAmount1.HasValue)
            {
                body.Append("客户首付款已开票金额（含税）：" + mould.SOBilledAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.POBilledAmount1.HasValue)
            {
                body.Append("采购首付款已开票金额（含税）：" + mould.POBilledAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierBilledAmount1.HasValue)
            {
                body.Append("供应商首付款已开票金额（含税）：" + mould.SupplierBilledAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.SOPayAmount1.HasValue)
            {
                body.Append("客户首付款已付款金额（含税）：" + mould.SOPayAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.POPayAmount1.HasValue)
            {
                body.Append("采购首付款已付款金额（含税）：" + mould.POPayAmount1.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierPayAmount1.HasValue)
            {
                body.Append("供应商首付款已付款金额（含税）：" + mould.SupplierPayAmount1.Value.ToString("0.########") + separator);
            }
            body.Append("<hr/>");
            if (mould.SOAmount2.HasValue)
            {
                body.Append("客户中期款1（含税）：" + mould.SOAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.POAmount2.HasValue)
            {
                body.Append("采购中期款1（含税）：" + mould.POAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierAmount2.HasValue)
            {
                body.Append("供应商中期款1（含税）：" + mould.SupplierAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.SOBillDate2.HasValue)
            {
                body.Append("计划客户中期款1开票日期：" + mould.SOBillDate2.Value.ToString("yyyy-MM-dd") + separator);
            }

            if (mould.SupplierBillDate2.HasValue)
            {
                body.Append("计划供应商中期款1开票日期：" + mould.SupplierBillDate2.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOPayDate2.HasValue)
            {
                body.Append("计划客户中期款1付款日期：" + mould.SOPayDate2.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SupplierPayDate2.HasValue)
            {
                body.Append("计划供应商中期款1付款日期：" + mould.SupplierPayDate2.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOBilledAmount2.HasValue)
            {
                body.Append("客户中期款1已开票金额（含税）：" + mould.SOBilledAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.POBilledAmount2.HasValue)
            {
                body.Append("采购中期款1已开票金额（含税）：" + mould.POBilledAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.POBilledAmount2.HasValue)
            {
                body.Append("供应商中期款1已开票金额（含税）：" + mould.SupplierBilledAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.SOPayAmount2.HasValue)
            {
                body.Append("客户中期款1已付款金额（含税）：" + mould.SOPayAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.POPayAmount2.HasValue)
            {
                body.Append("采购中期款1已付款金额（含税）：" + mould.POPayAmount2.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierPayAmount2.HasValue)
            {
                body.Append("供应商中期款1已付款金额（含税）：" + mould.SupplierPayAmount2.Value.ToString("0.########") + separator);
            }


            body.Append("<hr/>");
            if (mould.SOAmount4.HasValue)
            {
                body.Append("客户中期款2（含税）：" + mould.SOAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.POAmount4.HasValue)
            {
                body.Append("采购中期款2（含税）：" + mould.POAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierAmount4.HasValue)
            {
                body.Append("供应商中期款2（含税）：" + mould.SupplierAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.SOBillDate4.HasValue)
            {
                body.Append("计划客户中期款2开票日期：" + mould.SOBillDate4.Value.ToString("yyyy-MM-dd") + separator);
            }

            if (mould.SupplierBillDate4.HasValue)
            {
                body.Append("计划供应商中期款2开票日期：" + mould.SupplierBillDate4.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOPayDate4.HasValue)
            {
                body.Append("计划客户中期款2付款日期：" + mould.SOPayDate4.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SupplierPayDate4.HasValue)
            {
                body.Append("计划供应商中期款2付款日期：" + mould.SupplierPayDate4.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOBilledAmount4.HasValue)
            {
                body.Append("客户中期款2已开票金额（含税）：" + mould.SOBilledAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.POBilledAmount4.HasValue)
            {
                body.Append("采购中期款2已开票金额（含税）：" + mould.POBilledAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.POBilledAmount4.HasValue)
            {
                body.Append("供应商中期款2已开票金额（含税）：" + mould.SupplierBilledAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.SOPayAmount4.HasValue)
            {
                body.Append("客户中期款2已付款金额（含税）：" + mould.SOPayAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.POPayAmount4.HasValue)
            {
                body.Append("采购中期款2已付款金额（含税）：" + mould.POPayAmount4.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierPayAmount4.HasValue)
            {
                body.Append("供应商中期款2已付款金额（含税）：" + mould.SupplierPayAmount4.Value.ToString("0.########") + separator);
            }



            body.Append("<hr/>");
            if (mould.SOAmount3.HasValue)
            {
                body.Append("客户尾款（含税）：" + mould.SOAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.POAmount3.HasValue)
            {
                body.Append("采购尾款（含税）：" + mould.POAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierAmount3.HasValue)
            {
                body.Append("供应商尾款（含税）：" + mould.SupplierAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.SOBillDate3.HasValue)
            {
                body.Append("计划客户尾款开票日期：" + mould.SOBillDate3.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SupplierBillDate3.HasValue)
            {
                body.Append("计划供应商尾款开票日期：" + mould.SupplierBillDate3.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOPayDate3.HasValue)
            {
                body.Append("计划客户尾款付款日期：" + mould.SOPayDate3.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SupplierBillDate3.HasValue)
            {
                body.Append("计划供应商尾款付款日期：" + mould.SupplierPayDate3.Value.ToString("yyyy-MM-dd") + separator);
            }
            if (mould.SOBilledAmount3.HasValue)
            {
                body.Append("客户尾款已开票金额（含税）：" + mould.SOBilledAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.POBilledAmount3.HasValue)
            {
                body.Append("采购尾款已开票金额（含税）：" + mould.POBilledAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierBilledAmount3.HasValue)
            {
                body.Append("供应商尾款已开票金额（含税）：" + mould.SupplierBilledAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.SOPayAmount3.HasValue)
            {
                body.Append("客户尾款已付款金额（含税）：" + mould.SOPayAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.POPayAmount3.HasValue)
            {
                body.Append("采购尾款已付款金额（含税）：" + mould.POPayAmount3.Value.ToString("0.########") + separator);
            }
            if (mould.SupplierPayAmount3.HasValue)
            {
                body.Append("供应商尾款已付款金额（含税）：" + mould.SupplierPayAmount3.Value.ToString("0.########") + separator);
            }
            body.Append("<hr/>");
            body.Append("备注：" + mould.Remark + separator);
            body.Append(separator + separator + separator);

            body.Append("创建人：" + mould.CreateUserNm + separator);
            body.Append("创建日期：" + mould.CreateDate.ToString("yyyy-MM-dd"));
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Mould> GetMould(string prjCode, string type)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Mould));

            criteria.Add(Expression.Eq("PrjCode", prjCode));
            criteria.Add(Expression.Eq("Type", type));

            IList<Mould> mouldList = criteriaMgrE.FindAll<Mould>(criteria);
            return mouldList;
        }


        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class MouldMgrE : com.Sconit.ISI.Service.Impl.MouldMgr, IMouldMgrE
    {
    }
}

#endregion Extend Class