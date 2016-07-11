using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using com.Sconit.Facility.Service.Ext;
using com.Sconit.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using System.Linq;
using NHibernate;
using NPOI.SS.UserModel;
using com.Sconit.Entity.Exception;
using NPOI.HSSF.UserModel;
using com.Sconit.Utility;
using System.IO;
using com.Sconit.Service;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityFixOrderMgr : IFacilityFixOrderMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public IFacilityTransMgrE facilityTransMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public void CreateFacilityFixOrder(FacilityFixOrder facilityFixOrder)
        {

            genericMgr.Create(facilityFixOrder);

            FacilityMaster facilityMaster = genericMgr.FindById<FacilityMaster>(facilityFixOrder.FCID);

            #region 记报修事务
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = facilityFixOrder.CreateDate;
            facilityTrans.CreateUser = facilityFixOrder.CreateUser;
            facilityTrans.EffDate = facilityMaster.CreateDate;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_REPORT;

            facilityTransMgrE.CreateFacilityTrans(facilityTrans);
            #endregion

            #region 更新设备状态
            facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_BREAKDOWN;
            facilityMaster.LastModifyDate = facilityFixOrder.CreateDate;
            facilityMaster.LastModifyUser = facilityFixOrder.CreateUser;
            genericMgr.Update(facilityMaster);

            #endregion
        }


        [Transaction(TransactionMode.Requires)]
        public void ReleaseFacilityFixOrder(string fixNo, string userCode)
        {
            #region 更新保修单状态
            FacilityFixOrder facilityFixOrder = genericMgr.FindById<FacilityFixOrder>(fixNo);
            if (facilityFixOrder.Status != FacilityConstants.CODE_MASTER_FIX_ORDER_CREATE)
            {
                throw new BusinessErrorException("维修单不是创建状态,不能提交");
            }
            facilityFixOrder.Status = FacilityConstants.CODE_MASTER_FIX_ORDER_SUBMIT;
            facilityFixOrder.ReleaseUser = userCode;
            facilityFixOrder.ReleaseDate = DateTime.Now;
            genericMgr.Update(facilityFixOrder);
            #endregion


        }

        [Transaction(TransactionMode.Requires)]
        public void StartFacilityFixOrder(string fixNo, string userCode)
        {
            #region 更新报修单状态
            FacilityFixOrder facilityFixOrder = genericMgr.FindById<FacilityFixOrder>(fixNo);
            if (facilityFixOrder.Status != FacilityConstants.CODE_MASTER_FIX_ORDER_SUBMIT)
            {
                throw new BusinessErrorException("维修单不是提交状态,不能开始");
            }
            facilityFixOrder.Status = FacilityConstants.CODE_MASTER_FIX_ORDER_INPROCESS;
            facilityFixOrder.StartUser = userCode;
            facilityFixOrder.StartDate = DateTime.Now;
            genericMgr.Update(facilityFixOrder);
            #endregion

            #region 记维修修事务
            FacilityMaster facilityMaster = genericMgr.FindById<FacilityMaster>(facilityFixOrder.FCID);
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = userCode;
            facilityTrans.EffDate = DateTime.Now;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_FIX_START;

            facilityTransMgrE.CreateFacilityTrans(facilityTrans);
            #endregion

            #region 更新设备状态
            facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_FIX;
            facilityMaster.LastModifyDate = DateTime.Now;
            facilityMaster.LastModifyUser = userCode;
            genericMgr.Update(facilityMaster);
            #endregion
        }


        [Transaction(TransactionMode.Requires)]
        public void CompleteFacilityFixOrder(string fixNo,string userCode)
        {
            #region 更新报修单状态
            FacilityFixOrder facilityFixOrder = genericMgr.FindById<FacilityFixOrder>(fixNo);
            if (facilityFixOrder.Status != FacilityConstants.CODE_MASTER_FIX_ORDER_INPROCESS)
            {
                throw new BusinessErrorException("维修单不是执行状态,不能完成");
            }

            facilityFixOrder.Status = FacilityConstants.CODE_MASTER_FIX_ORDER_SUBMIT;
            facilityFixOrder.ReleaseUser = userCode;
            facilityFixOrder.ReleaseDate = DateTime.Now;
            genericMgr.Update(facilityFixOrder);
            #endregion


            #region 记维修修事务
            FacilityMaster facilityMaster = genericMgr.FindById<FacilityMaster>(facilityFixOrder.FCID);
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = userCode;
            facilityTrans.EffDate = DateTime.Now;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_FIX_FINISH;

            facilityTransMgrE.CreateFacilityTrans(facilityTrans);
            #endregion

            #region 更新设备状态
            facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_REPAIRED;
            facilityMaster.LastModifyDate = DateTime.Now;
            facilityMaster.LastModifyUser = userCode;
            genericMgr.Update(facilityMaster);
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseFacilityFixOrder(string fixNo,string userCode)
        {

            #region 更新报修单状态
            FacilityFixOrder facilityFixOrder = genericMgr.FindById<FacilityFixOrder>(fixNo);
            if (facilityFixOrder.Status != FacilityConstants.CODE_MASTER_FIX_ORDER_COMPLETE)
            {
                throw new BusinessErrorException("维修单不是完成状态,不能关闭");
            }  
            facilityFixOrder.Status = FacilityConstants.CODE_MASTER_FIX_ORDER_INPROCESS;
            facilityFixOrder.StartUser = userCode;
            facilityFixOrder.StartDate = DateTime.Now;
            genericMgr.Update(facilityFixOrder);
            #endregion
          

            #region 更新设备状态
            FacilityMaster facilityMaster = genericMgr.FindById<FacilityMaster>(facilityFixOrder.FCID);
         
            facilityMaster.Status = FacilityConstants.CODE_MASTER_FROCK_STARUS_AVALIABLE;
            facilityMaster.LastModifyDate = DateTime.Now;
            facilityMaster.LastModifyUser = userCode;
            genericMgr.Update(facilityMaster);
            #endregion
        }



        [Transaction(TransactionMode.Requires)]
        public FacilityFixOrder GetFacilityFixOrder(string fcId)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityFixOrder));
            criteria.Add(Expression.Eq("FCID", fcId));
            criteria.Add(Expression.Eq("Status", FacilityConstants.CODE_MASTER_FIX_ORDER_INPROCESS));
            IList<FacilityFixOrder> facilityFixOrderList = criteriaMgrE.FindAll<FacilityFixOrder>(criteria);

            return facilityFixOrderList.FirstOrDefault();
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityFixOrderMgrE : com.Sconit.Facility.Service.Impl.FacilityFixOrderMgr, IFacilityFixOrderMgrE
    {
    }
}

#endregion Extend Class