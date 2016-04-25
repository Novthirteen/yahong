using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Utility;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityDistributionMgr : FacilityDistributionBaseMgr, IFacilityDistributionMgr
    {
        #region Customized Methods

        public ICriteriaMgrE criteriaMgrE { get; set; }


        public IList<FacilityDistribution> GetFacilityDistributionList(string purchaseContractCode)
        {
            return GetFacilityDistributionList(null, purchaseContractCode);
        }

        public IList<FacilityDistribution> GetFacilityDistributionList(int? id, string purchaseContractCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistribution));
            if (id != null)
            {
                criteria.Add(Expression.Not(Expression.Eq("Id", id.Value)));
            }
            criteria.Add(Expression.Eq("PurchaseContractCode", purchaseContractCode));

            IList<FacilityDistribution> facilityDistributionList = criteriaMgrE.FindAll<FacilityDistribution>(criteria);
            return facilityDistributionList;
        }

        public IList<FacilityDistribution> GetFacilityDistributionListByDistribution(string distributionContractCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistribution));

            criteria.Add(Expression.Eq("DistributionContractCode", distributionContractCode));

            IList<FacilityDistribution> facilityDistributionList = criteriaMgrE.FindAll<FacilityDistribution>(criteria);
            return facilityDistributionList;
        }

        public IList<FacilityDistribution> GetFacilityDistributionListByCode(string code)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistribution));

            criteria.Add(Expression.Eq("Code", code));

            IList<FacilityDistribution> facilityDistributionList = criteriaMgrE.FindAll<FacilityDistribution>(criteria);
            return facilityDistributionList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityDistribution> GetFacilityDistributionSupplier()
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityDistribution>();

            criteria.Add(Expression.Not(Expression.Eq("SupplierName", string.Empty)));

            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList()
                .Add(Projections.Alias(Projections.Property("SupplierName"), "SupplierName"))));

            criteria.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(FacilityDistribution)));

            return criteriaMgrE.FindAll<FacilityDistribution>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityDistribution> GetFacilityDistributionCustomer()
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityDistribution>();

            criteria.Add(Expression.Not(Expression.Eq("CustomerName", string.Empty)));
            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList()
                .Add(Projections.Alias(Projections.Property("CustomerName"), "CustomerName"))));

            criteria.SetResultTransformer(
                new NHibernate.Transform.AliasToBeanResultTransformer(typeof(FacilityDistribution)));

            return criteriaMgrE.FindAll<FacilityDistribution>(criteria);
        }


        [Transaction(TransactionMode.Requires)]
        public override void DeleteFacilityDistribution(Int32 id)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistributionDetail));
            criteria.Add(Expression.Eq("FacilityDistribution.Id", id));
            IList<FacilityDistributionDetail> facilityDistributionDetailList = criteriaMgrE.FindAll<FacilityDistributionDetail>(criteria);
            foreach (FacilityDistributionDetail d in facilityDistributionDetailList)
            {
                Delete(d);
            }

            base.DeleteFacilityDistribution(id);
        }

        [Transaction(TransactionMode.Requires)]
        public override void CreateFacilityDistribution(FacilityDistribution entity)
        {
            #region 先检查下有没有已经销售合同号有没有已经存在的，有的话要把那些明细加过来，12.3改为按照系统合同号
            IList<FacilityDistributionDetail> addFacilityDIstributionDetailList = new List<FacilityDistributionDetail>();
            //if (!string.IsNullOrEmpty(entity.DistributionContractCode))
            //{
                DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityDistribution));
                criteria.Add(Expression.Eq("Code", entity.Code));
                IList<FacilityDistribution> facilityDistributionList = criteriaMgrE.FindAll<FacilityDistribution>(criteria);

                if (facilityDistributionList != null && facilityDistributionList.Count > 0)
                {
                    FacilityDistribution f = facilityDistributionList[0];
                    DetachedCriteria dCriteria = DetachedCriteria.For(typeof(FacilityDistributionDetail));
                    dCriteria.Add(Expression.Eq("FacilityDistribution.Id", f.Id));
                    dCriteria.Add(Expression.Eq("Type", FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_DISTRIBUTION));
                    IList<FacilityDistributionDetail> facilityDistributionDetailList = criteriaMgrE.FindAll<FacilityDistributionDetail>(dCriteria);
                    if (facilityDistributionDetailList != null && facilityDistributionDetailList.Count > 0)
                    {
                        foreach (FacilityDistributionDetail d in facilityDistributionDetailList)
                        {
                            #region 把批次号加上去
                            if (string.IsNullOrEmpty(d.BatchNo))
                            {
                                d.BatchNo = DateTime.Now.ToString("yyyyMMddhhmmss");
                                this.Update(d);
                            }
                            #endregion

                            #region 插入明细
                            FacilityDistributionDetail fdd = new FacilityDistributionDetail();
                            CloneHelper.CopyProperty(d, fdd, new string[] { }, true);
                            fdd.FacilityDistribution = entity;
                            addFacilityDIstributionDetailList.Add(fdd);

                            #endregion
                        }
                        entity.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_INPROCESS;
                    }
                }
            //}
            base.CreateFacilityDistribution(entity);
            if (addFacilityDIstributionDetailList.Count > 0)
            {
                foreach (FacilityDistributionDetail ad in addFacilityDIstributionDetailList)
                {
                    Create(ad);
                }
            }

            #endregion


        }

        [Transaction(TransactionMode.Requires)]
        public void CopyFacilityDistribution(FacilityDistribution facilityDistribution,User user)
        {
            #region 复制头
            FacilityDistribution newFacilityDistribution = new FacilityDistribution();
            CloneHelper.CopyProperty(facilityDistribution, newFacilityDistribution, new string[] { "FCID","CustomerName", "DistributionContractCode", "DistributionContractAmount", "DistributionBilledAmount", "DistributionPayAmount", "DistributionContact","Code" });
            newFacilityDistribution.CreateDate = DateTime.Now;
            newFacilityDistribution.CreateUser = user.Code;
            newFacilityDistribution.LastModifyDate = DateTime.Now;
            newFacilityDistribution.LastModifyUser = user.Code;
            newFacilityDistribution.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CREATE;
            this.CreateFacilityDistribution(newFacilityDistribution);
            #endregion

        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityDistributionMgrE : com.Sconit.Facility.Service.Impl.FacilityDistributionMgr, IFacilityDistributionMgrE
    {
    }
}

#endregion Extend Class