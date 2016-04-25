using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;
using System.Drawing;
using com.Sconit.ISI.Service.Util;
using LeanEngine.Entity;
using com.Sconit.Facility.Service;
using com.Sconit.Facility.Entity;

public partial class Facility_RepairOrder_New : NewModuleBase
{
    public event EventHandler Back;
    public event EventHandler Create;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    public void PageCleanup()
    {
        this.tbFCID.Text = string.Empty;
        this.tbSubmitDept.Text = string.Empty;
        this.tbSubmitTime.Text = string.Empty;
        this.tbFaultDescription.Text = string.Empty;
        this.tbHaltStartTime.Text = string.Empty;

    }

    public bool CheckFCID(string FCID)
    {
        var list = this.TheGenericMgr.FindAll<RepairOrder>(" from RepairOrder where FCID=? and Status <>'Close'", FCID);
        if (list != null && list.Any())
        { return false; }
        else
        { return true; }
    }

    protected void btnInsert_Click(object sender, EventArgs e)
    {
        if (!CheckFCID(tbFCID.Text.Trim()))
        {
            ShowWarningMessage("该设备编号有未关闭的维修单");
        }
        else
        {
            var repairOrder = new RepairOrder();
            repairOrder.FCID = this.tbFCID.Text.Trim();
            FacilityMaster facilityMaster = this.TheGenericMgr.FindById<FacilityMaster>(repairOrder.FCID);
            repairOrder.FCName = facilityMaster.Name;
            repairOrder.AssetNo = facilityMaster.AssetNo;
            repairOrder.SubmitDept = this.tbSubmitDept.Text.Trim();
            repairOrder.SubmitUser = this.CurrentUser.Code;
            repairOrder.SubmitTime = Convert.ToDateTime(this.tbSubmitTime.Text.Trim());
            repairOrder.FaultDescription = this.tbFaultDescription.Text.Trim();
            repairOrder.HaltStartTime = Convert.ToDateTime(this.tbHaltStartTime.Text.Trim());
            //repairOrder.HaltEndTime = Convert.ToDateTime("1900-01-01");
            repairOrder.OperateUser = "";
            repairOrder.RepairDescription = "";
            repairOrder.HaltReason = "";
            repairOrder.Items = "";
            repairOrder.Suggestion = "";
            repairOrder.SuggestionUser = "";

            repairOrder.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
            repairOrder.SubmitUserName = this.CurrentUser.CodeName;
            repairOrder.RefOrderNo = "";
            repairOrder.OperateUserName = "";
            repairOrder.SuggestionUserName = "";

            repairOrder.CreateUser = this.CurrentUser.Code;
            repairOrder.CreateDate = DateTime.Now;
            repairOrder.LastModifyDate = DateTime.Now;
            repairOrder.LastModifyUser = this.CurrentUser.Code;
            repairOrder.OrderNo = TheNumberControlMgr.GenerateNumber("RO");

            repairOrder.RepairUser = "";
            repairOrder.RepairUserName = "";
            //repairOrder.RepairStartTime = Convert.ToDateTime("1900-01-01");
            //repairOrder.RepairEndTime = Convert.ToDateTime("1900-01-01");
            //repairOrder.AcceptanceTime = Convert.ToDateTime("1900-01-01");

            #region 更新状态
            facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_FIX;
            facilityMaster.LastModifyDate = DateTime.Now;
            facilityMaster.LastModifyUser = this.CurrentUser.Code;
            #endregion
            #region 记事务
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = this.CurrentUser.Code;
            facilityTrans.EffDate = DateTime.Now.Date;
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
            facilityTrans.Remark = "报修单号：" + repairOrder.OrderNo;
            #endregion
            TheFacilityMasterMgr.UpdateFacilityAndCreateFacilityTransAndRepairOrder(facilityMaster, facilityTrans, repairOrder, "Create");
            //this.TheGenericMgr.Create(repairOrder);
            ShowSuccessMessage("创建成功");
            Create(repairOrder.OrderNo, e);
        }

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            Back(this, e);
        }
    }
}