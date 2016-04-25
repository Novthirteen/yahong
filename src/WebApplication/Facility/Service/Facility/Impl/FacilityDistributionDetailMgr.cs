using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Facility.Service.Ext;
using System.Linq;
using com.Sconit.Entity;
using NHibernate.Mapping;
using com.Sconit.Utility;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityDistributionDetailMgr : FacilityDistributionDetailBaseMgr, IFacilityDistributionDetailMgr
    {

        #region Customized Methods

        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IFacilityDistributionMgrE facilityDistributionMgrE { get; set; }

        [Transaction(TransactionMode.Requires)]
        public override void CreateFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            CreateFacilityDistributionDetail(entity, true);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateFacilityDistributionDetail(FacilityDistributionDetail entity, bool isCheckDistribution)
        {
            string batchNo = string.Empty;

            #region 更新头金额

            FacilityDistribution facilityDistribution = facilityDistributionMgrE.LoadFacilityDistribution(entity.FacilityDistribution.Id);

            if (entity.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_PROCUREMENT)
            {
                facilityDistribution.PurchaseBilledAmount += entity.BillAmount;
                facilityDistribution.PurchasePayAmount += entity.PayAmount;
            }
            else
            {
                facilityDistribution.DistributionBilledAmount += +entity.BillAmount;
                facilityDistribution.DistributionPayAmount += entity.PayAmount;
            }

            if (facilityDistribution.PurchaseBilledAmount > 0 || facilityDistribution.PurchasePayAmount > 0 || facilityDistribution.DistributionBilledAmount > 0 || facilityDistribution.DistributionPayAmount > 0)
            {
                if (facilityDistribution.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
                {
                    facilityDistribution.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS;
                }
            }
            facilityDistributionMgrE.UpdateFacilityDistribution(facilityDistribution);
            #endregion


            if (isCheckDistribution && entity.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_DISTRIBUTION)
            {
                #region 销售因为是一对多，需要销售的时候同时把其他更新掉，12.3按Code更新

                DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistribution));
                criteria.Add(Expression.Eq("Code", facilityDistribution.Code));
                criteria.Add(Expression.Not(Expression.Eq("Id", facilityDistribution.Id)));

                IList<FacilityDistribution> facilityDistributionList = criteriaMgrE.FindAll<FacilityDistribution>(criteria);
                if (facilityDistributionList != null && facilityDistributionList.Count > 0)
                {
                    batchNo = DateTime.Now.ToString("yyyyMMddhhmmss");
                    foreach (FacilityDistribution f in facilityDistributionList)
                    {
                        #region 插入明细
                        FacilityDistributionDetail fdd = new FacilityDistributionDetail();
                        CloneHelper.CopyProperty(entity, fdd, new string[] { }, true);
                        fdd.FacilityDistribution = f;
                        fdd.BatchNo = batchNo;
                        this.CreateFacilityDistributionDetail(fdd, false);
                        #endregion
                    }
                }

                #endregion
            }

            #region 最后在更新，如果有的话更新batchno
            if (entity.BatchNo == null)
            {
                entity.BatchNo = batchNo;
            }
            base.CreateFacilityDistributionDetail(entity);
            #endregion
        }


        [Transaction(TransactionMode.Requires)]
        public override void DeleteFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            DeleteFacilityDistributionDetail(entity, true);
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteFacilityDistributionDetail(FacilityDistributionDetail entity, bool isCheckDistribution)
        {
            #region 更新头金额

            FacilityDistribution facilityDistribution = facilityDistributionMgrE.LoadFacilityDistribution(entity.FacilityDistribution.Id);

            if (entity.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_PROCUREMENT)
            {
                facilityDistribution.PurchaseBilledAmount -= entity.BillAmount;
                facilityDistribution.PurchasePayAmount -= entity.PayAmount;
            }
            else
            {
                facilityDistribution.DistributionBilledAmount -= +entity.BillAmount;
                facilityDistribution.DistributionPayAmount -= entity.PayAmount;
            }

            if (facilityDistribution.PurchaseBilledAmount > 0 || facilityDistribution.PurchasePayAmount > 0 || facilityDistribution.DistributionBilledAmount > 0 || facilityDistribution.DistributionPayAmount > 0)
            {
                if (facilityDistribution.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
                {
                    facilityDistribution.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS;
                }
            }
            facilityDistributionMgrE.UpdateFacilityDistribution(facilityDistribution);
            #endregion

            base.DeleteFacilityDistributionDetail(entity);


            if (isCheckDistribution && !string.IsNullOrEmpty(entity.BatchNo) && entity.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_DISTRIBUTION)
            {
                #region 销售因为是一对多，需要销售的时候同时把其他删除掉

                DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistributionDetail));
                criteria.Add(Expression.Eq("BatchNo", entity.BatchNo));
                criteria.Add(Expression.Not(Expression.Eq("Id", entity.Id)));

                IList<FacilityDistributionDetail> facilityDistributionDetailList = criteriaMgrE.FindAll<FacilityDistributionDetail>(criteria);
                if (facilityDistributionDetailList != null && facilityDistributionDetailList.Count > 0)
                {
                    foreach (FacilityDistributionDetail fd in facilityDistributionDetailList)
                    {
                        #region 删除明细
                        this.DeleteFacilityDistributionDetail(fd, false);
                        #endregion
                    }
                }

                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public override void UpdateFacilityDistributionDetail(FacilityDistributionDetail entity)
        {
            UpdateFacilityDistributionDetail(entity, true);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateFacilityDistributionDetail(FacilityDistributionDetail entity, bool isCheckDistribution)
        {
            #region 更新头金额
            decimal billedAmount = 0;
            decimal payAmount = 0;
            FacilityDistribution facilityDistribution = facilityDistributionMgrE.LoadFacilityDistribution(entity.FacilityDistribution.Id);
            IList<FacilityDistributionDetail> facilityDistributionDetailList = GetOtherFacilityDistributionDetailList(entity.FacilityDistribution.Id, entity.Id, entity.Type);
            if (facilityDistributionDetailList != null && facilityDistributionDetailList.Count > 0)
            {
                billedAmount = facilityDistributionDetailList.Sum(p => p.BillAmount);
                payAmount = facilityDistributionDetailList.Sum(p => p.PayAmount);
            }
            if (entity.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_PROCUREMENT)
            {
                facilityDistribution.PurchaseBilledAmount = billedAmount + entity.BillAmount;
                facilityDistribution.PurchasePayAmount = payAmount + entity.PayAmount;
            }
            else
            {
                facilityDistribution.DistributionBilledAmount = billedAmount + entity.BillAmount;
                facilityDistribution.DistributionPayAmount = payAmount + entity.PayAmount;
            }
            if (facilityDistribution.PurchaseBilledAmount > 0 || facilityDistribution.PurchasePayAmount > 0 || facilityDistribution.DistributionBilledAmount > 0 || facilityDistribution.DistributionPayAmount > 0)
            {
                if (facilityDistribution.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
                {
                    facilityDistribution.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS;
                }
            }

            facilityDistributionMgrE.UpdateFacilityDistribution(facilityDistribution);
            #endregion

            base.UpdateFacilityDistributionDetail(entity);

            if (isCheckDistribution && !string.IsNullOrEmpty(entity.BatchNo) && entity.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_DISTRIBUTION)
            {
                #region 销售因为是一对多，需要销售的时候同时把其他更新掉

                DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistributionDetail));
                criteria.Add(Expression.Eq("BatchNo", entity.BatchNo));
                criteria.Add(Expression.Not(Expression.Eq("Id", entity.Id)));
                criteria.Add(Expression.Eq("Type", FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_DISTRIBUTION));

                IList<FacilityDistributionDetail> fddList = criteriaMgrE.FindAll<FacilityDistributionDetail>(criteria);
                if (fddList != null && fddList.Count > 0)
                {
                    foreach (FacilityDistributionDetail fdd in fddList)
                    {
                        #region 更新明细
                        CloneHelper.CopyProperty(entity, fdd, new string[] { "PayDate", "PayAmount", "BillDate", "BillAmount", "Contact", "Remark" });
                        this.UpdateFacilityDistributionDetail(fdd, false);
                        #endregion
                    }
                }

                #endregion
            }
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteFacilityDistributionDetail(int id)
        {
            FacilityDistributionDetail d = this.LoadFacilityDistributionDetail(id);
            DeleteFacilityDistributionDetail(d);
        }

        private IList<FacilityDistributionDetail> GetOtherFacilityDistributionDetailList(Int32 facilityDistributionId, Int32 id, string type)
        {

            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistributionDetail));
            criteria.CreateAlias("FacilityDistribution", "d");
            criteria.Add(Expression.Eq("d.Id", facilityDistributionId));
            criteria.Add(Expression.Not(Expression.Eq("Id", id)));
            criteria.Add(Expression.Eq("Type", type));
            IList<FacilityDistributionDetail> facilityDistributionDetailList = criteriaMgrE.FindAll<FacilityDistributionDetail>(criteria);

            return facilityDistributionDetailList;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityDistributionDetailMgrE : com.Sconit.Facility.Service.Impl.FacilityDistributionDetailMgr, IFacilityDistributionDetailMgrE
    {
    }
}

#endregion Extend Class