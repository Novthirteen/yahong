using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.ISI.Entity;
using com.Sconit.Service;
using System.Linq;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Utility;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class MouldDetailMgr : IMouldDetailMgr
    {
        public IGenericMgr genericMgr { get; set; }

        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public IList<MouldDetail> GetMouldDetail(string code)
        {
            return this.genericMgr.FindAll<MouldDetail>("select d from MouldDetail d where d.Code='" + code + "' order by d.Phase Asc,d.CreateDate Asc ");
        }
        [Transaction(TransactionMode.Requires)]
        public void DeleteMouldDetail(int id)
        {
            string code = this.LoadMouldDetail(id).Code;
            genericMgr.DeleteById<MouldDetail>(id);
            genericMgr.FlushSession();
            UpdateMould(code);
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteMouldDetail(MouldDetail mouldDetail)
        {
            genericMgr.Delete("from MouldDetail d where d.Id = " + mouldDetail.Id);
            genericMgr.FlushSession();
            UpdateMould(mouldDetail.Code);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateMouldDetail(MouldDetail mouldDetail)
        {
            genericMgr.Create(mouldDetail);
            UpdateMould(mouldDetail.Code);
        }
        [Transaction(TransactionMode.Unspecified)]
        public MouldDetail LoadMouldDetail(int id)
        {
            return genericMgr.FindById<MouldDetail>(id);
        }
        [Transaction(TransactionMode.Requires)]
        public void UpdateMouldDetail(MouldDetail mouldDetail)
        {
            genericMgr.Update(mouldDetail);

            UpdateMould(mouldDetail.Code);
        }
        [Transaction(TransactionMode.Requires)]
        private void UpdateMould(string code)
        {
            var mouldDetailList = this.GetMouldDetail(code);
            var mould = genericMgr.FindById<Mould>(code);
            if (mouldDetailList != null && mouldDetailList.Count > 0)
            {
                //供应商开票
                var poMouldDetailList = mouldDetailList.Where(md => md.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_PO);
                mould.SupplierBilledAmount = poMouldDetailList.Sum(md => md.BillAmount);
                mould.SupplierBilledAmount1 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_1).Sum(md => md.BillAmount);
                mould.SupplierBilledAmount2 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_2).Sum(md => md.BillAmount);
                mould.SupplierBilledAmount3 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_3).Sum(md => md.BillAmount);
                mould.SupplierBilledAmount4 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_4).Sum(md => md.BillAmount);

                //供应商付款
                //poMouldDetailList = mouldDetailList.Where(md => md.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_PO);
                mould.SupplierPayAmount = poMouldDetailList.Sum(md => md.PayAmount);
                mould.SupplierPayAmount1 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_1).Sum(md => md.PayAmount);
                mould.SupplierPayAmount2 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_2).Sum(md => md.PayAmount);
                mould.SupplierPayAmount3 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_3).Sum(md => md.PayAmount);
                mould.SupplierPayAmount4 = poMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_4).Sum(md => md.PayAmount);
            }
            else
            {
                mould.SupplierBilledAmount = null;
                mould.SupplierBilledAmount1 = null;
                mould.SupplierBilledAmount2 = null;
                mould.SupplierBilledAmount3 = null;
                mould.SupplierBilledAmount4 = null;
                mould.SupplierBilledAmount = null;
                mould.SupplierBilledAmount1 = null;
                mould.SupplierBilledAmount2 = null;
                mould.SupplierBilledAmount3 = null;
                mould.SupplierBilledAmount4 = null;
            }


            //var mdList = genericMgr.FindAll<MouldDetail>("select md from MouldDetail md,Mould m where m.PrjCode='" + mould.PrjCode + "' and m.Type='" + mould.Type + "'");
            var mdList = this.genericMgr.FindAll<MouldDetail>("select d from MouldDetail d ,Mould m  where d.Code=m.Code and m.FCID='" + mould.FCID + "'  and m.Type='" + mould.Type + "' order by d.Phase Asc,d.CreateDate Asc ");

            if (mdList != null && mdList.Count > 0)
            {
                //销售开票
                var soMouldDetailList = mdList.Where(md => md.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_SO);
                mould.SOBilledAmount = soMouldDetailList.Sum(md => md.BillAmount);
                mould.SOBilledAmount1 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_1).Sum(md => md.BillAmount);
                mould.SOBilledAmount2 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_2).Sum(md => md.BillAmount);
                mould.SOBilledAmount3 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_3).Sum(md => md.BillAmount);
                mould.SOBilledAmount4 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_4).Sum(md => md.BillAmount);

                //销售付款
                mould.SOPayAmount = soMouldDetailList.Sum(md => md.PayAmount);
                mould.SOPayAmount1 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_1).Sum(md => md.PayAmount);
                mould.SOPayAmount2 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_2).Sum(md => md.PayAmount);
                mould.SOPayAmount3 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_3).Sum(md => md.PayAmount);
                mould.SOPayAmount4 = soMouldDetailList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_4).Sum(md => md.PayAmount);

                //采购开票
                var poList = mdList.Where(md => md.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_PO);
                mould.POBilledAmount = poList.Sum(md => md.BillAmount);
                mould.POBilledAmount1 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_1).Sum(md => md.BillAmount);
                mould.POBilledAmount2 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_2).Sum(md => md.BillAmount);
                mould.POBilledAmount3 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_3).Sum(md => md.BillAmount);
                mould.POBilledAmount4 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_4).Sum(md => md.BillAmount);

                //采购付款
                mould.POPayAmount = poList.Sum(md => md.PayAmount);
                mould.POPayAmount1 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_1).Sum(md => md.PayAmount);
                mould.POPayAmount2 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_2).Sum(md => md.PayAmount);
                mould.POPayAmount3 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_3).Sum(md => md.PayAmount);
                mould.POPayAmount4 = poList.Where(md => md.Phase == ISIConstants.CODE_MASTER_PSIBILLDETAIL_PHASE_4).Sum(md => md.PayAmount);
            }
            else
            {
                mould.SOBilledAmount = null;
                mould.SOBilledAmount1 = null;
                mould.SOBilledAmount2 = null;
                mould.SOBilledAmount3 = null;
                mould.SOBilledAmount4 = null;
                mould.SOPayAmount = null;
                mould.SOPayAmount1 = null;
                mould.SOPayAmount2 = null;
                mould.SOPayAmount3 = null;
                mould.SOPayAmount4 = null;
                mould.POBilledAmount = null;
                mould.POBilledAmount1 = null;
                mould.POBilledAmount2 = null;
                mould.POBilledAmount3 = null;
                mould.POBilledAmount4 = null;
                mould.POPayAmount = null;
                mould.POPayAmount1 = null;
                mould.POPayAmount2 = null;
                mould.POPayAmount3 = null;
                mould.POPayAmount4 = null;
            }

            if (mould.Status == ISIConstants.CODE_MASTER_PSI_BILL_STATUS_CREATE)
            {
                mould.Status = ISIConstants.CODE_MASTER_PSI_BILL_STATUS_INPROCESS;
            }
            genericMgr.Update(mould);

            //刷新采购和销售
            var mouldList = genericMgr.FindAll<Mould>("select m from Mould m where m.FCID='" + mould.FCID + "' and m.Code !='" + mould.Code + "'");
            if (mouldList != null && mouldList.Count > 0)
            {
                foreach (var m in mouldList)
                {
                    CloneHelper.CopyProperty(mould, m, new string[] { "PrjCode","PrjDesc", "QS",
                                                                            "Customer", "CustomerName","SOContractNo","SOAmount","SOBilledAmount","SOPayAmount","SOAmount1","SOBilledAmount1","SOPayAmount1","SOBillDate1","SOPayDate1","SOAmount2","SOBilledAmount2","SOPayAmount2","SOBillDate2","SOPayDate2","SOAmount3","SOBilledAmount3","SOPayAmount3","SOBillDate3","SOPayDate3","SOAmount4","SOBilledAmount4","SOPayAmount4","SOBillDate4","SOPayDate4",
                                                                            "POAmount","POBilledAmount","POPayAmount","POAmount1","POBilledAmount1","POPayAmount1","POAmount2","POBilledAmount2","POPayAmount2","POAmount3","POBilledAmount3","POPayAmount3","POAmount4","POBilledAmount4","POPayAmount4",
                                                                            });
                    genericMgr.Update(mould);
                }
            }
        }


        [Transaction(TransactionMode.Unspecified)]
        public IList<MouldDetail> GetAllMouldDetail()
        {
            return this.genericMgr.FindAll<MouldDetail>();
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class MouldDetailMgrE : com.Sconit.ISI.Service.Impl.MouldDetailMgr, IMouldDetailMgrE
    {
    }
}

#endregion Extend Class