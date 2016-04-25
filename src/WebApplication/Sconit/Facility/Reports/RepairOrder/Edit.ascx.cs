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

public partial class Facility_RepairOrder_Edit : EditModuleBase
{

    private String Code
    {
        get { return (string)ViewState["Code"]; }
        set { ViewState["Code"] = value; }
    }

    private String Status
    {
        get { return (string)ViewState["Status"]; }
        set { ViewState["Status"] = value; }
    }

    public event EventHandler Back;


    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public void PageCleanup()
    {
        this.lblStatus.Text = string.Empty;
        this.Status = string.Empty;
        this.lblCode.Text = string.Empty;
        this.tbSubmitDept.Text = string.Empty;
        this.tbSubmitTime.Text = string.Empty;
        this.lblFCID.Text = string.Empty;
        this.lblFCName.Text = string.Empty;
        this.lblAssetNo.Text = string.Empty;
        this.lblSubmitUserName.Text = string.Empty;
        this.tbFaultDescription.Text = string.Empty;
        this.tbHaltStartTime.Text = string.Empty;
        this.tbHaltEndTime.Text = string.Empty;
        this.lblOperateUserName.Text = string.Empty;
        this.tbRepairDescription.Text = string.Empty;
        this.tbHaltReason.Text = string.Empty;
        this.tbItems.Text = string.Empty;
        this.tbSuggestion.Text = string.Empty;
        this.lblSuggestionUserName.Text = string.Empty;
        this.tbRepairUser.Text = string.Empty;
        this.tbRefOrderNo.Text = string.Empty;
        this.tbRepairStartTime.Text = string.Empty;
        this.tbRepairEndTime.Text = string.Empty;

    }
    public void InitPageParameter(string Code)
    {
        PageCleanup();
        #region 赋值
        RepairOrder repairOrder = this.TheGenericMgr.FindById<RepairOrder>(Code);
        this.lblStatus.Text = repairOrder.Status;
        this.Status = repairOrder.Status;
        this.Code = Code;
        this.lblCode.Text = repairOrder.OrderNo;
        this.tbSubmitDept.Text = repairOrder.SubmitDept;
        this.tbSubmitTime.Text = repairOrder.SubmitTime.ToString("yyyy-MM-dd HH:mm");
        this.lblFCID.Text = repairOrder.FCID;
        this.lblFCName.Text = repairOrder.FCName;
        this.lblAssetNo.Text = repairOrder.AssetNo;
        this.lblSubmitUserName.Text = repairOrder.SubmitUserName;

        this.tbFaultDescription.Text = repairOrder.FaultDescription;
        this.tbHaltStartTime.Text = repairOrder.HaltStartTime.ToString("yyyy-MM-dd HH:mm");
        if (repairOrder.HaltEndTime.HasValue)
        { 
            this.tbHaltEndTime.Text = ((DateTime)repairOrder.HaltEndTime).ToString("yyyy-MM-dd HH:mm");
        }
        this.lblOperateUserName.Text = repairOrder.OperateUserName;
        this.tbRepairDescription.Text = repairOrder.RepairDescription;
        this.tbHaltReason.Text = repairOrder.HaltReason;
        this.tbItems.Text = repairOrder.Items;
        this.tbSuggestion.Text = repairOrder.Suggestion;
        this.lblSuggestionUserName.Text = repairOrder.SuggestionUserName;
        if (!string.IsNullOrEmpty(repairOrder.RepairUser))
        {
            string userNames = this.TheUserSubscriptionMgr.GetUserName(repairOrder.RepairUser);
            this.tbRepairUser.Text = ISIUtil.GetUserMerge(repairOrder.RepairUser, userNames);
        }
        else
        {
            this.tbRepairUser.Text = string.Empty;
        }

        this.tbRefOrderNo.Text = repairOrder.RefOrderNo;
        if (repairOrder.RepairStartTime.HasValue)
        {
            this.tbRepairStartTime.Text = ((DateTime)repairOrder.RepairStartTime).ToString("yyyy-MM-dd HH:mm");
        }
        if (repairOrder.RepairEndTime.HasValue)
        {
            this.tbRepairEndTime.Text = ((DateTime)repairOrder.RepairEndTime).ToString("yyyy-MM-dd HH:mm");
        }
        #endregion
        #region 状态判断
        switch (repairOrder.Status)
        {
            case "Create":
                this.btnUpdate.Visible = false;
                this.btnPost.Visible = true;
                this.btnDelete.Visible = true;
                this.btnRepair.Visible = false;
                this.btnAcceptance.Visible = false;
                this.btnClose.Visible = false;
                this.btnBack2.Visible = true;
                this.fldRepair.Visible = false;
                break;
            case "等待维修":
                this.btnUpdate.Visible = true;
                this.btnPost.Visible = false;
                this.btnDelete.Visible = false;
                this.btnRepair.Visible = true;
                this.btnAcceptance.Visible = false;
                this.btnClose.Visible = false;
                this.btnBack2.Visible = false;
                this.fldRepair.Visible = true;
                break;
            case "等待验收":
                this.btnUpdate.Visible = true;
                this.btnPost.Visible = false;
                this.btnDelete.Visible = false;
                this.btnRepair.Visible = false;
                this.btnAcceptance.Visible = true;
                this.btnClose.Visible = false;
                this.btnBack2.Visible = false;
                this.fldRepair.Visible = true;
                break;
            case "等待关闭":
                this.btnUpdate.Visible = true;
                this.btnPost.Visible = false;
                this.btnDelete.Visible = false;
                this.btnRepair.Visible = false;
                this.btnAcceptance.Visible = false;
                this.btnClose.Visible = true;
                this.btnBack2.Visible = false;
                this.fldRepair.Visible = true;
                break;
            default:
                this.btnUpdate.Visible = false;
                this.btnPost.Visible = false;
                this.btnDelete.Visible = false;
                this.btnRepair.Visible = false;
                this.btnAcceptance.Visible = false;
                this.btnClose.Visible = false;
                this.btnBack2.Visible = false;
                this.fldRepair.Visible = true;
                break;
        }
        #endregion

    }


    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            this.Visible = false;
            Back(sender, e);
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        if (tbRepairDescription.Text.Trim() == "" || tbHaltReason.Text.Trim() == "" || tbItems.Text.Trim()==""
            || tbRepairUser.Text.Trim() == "" || tbRepairStartTime.Text.Trim()=="" || tbRepairEndTime.Text.Trim()=="")
        {
            ShowWarningMessage("维修时间、人员、步骤、原因、备件为必填项");
        }
        else if (Convert.ToDateTime(tbRepairStartTime.Text.Trim()) > Convert.ToDateTime(tbRepairEndTime.Text.Trim()))
        {
            ShowWarningMessage("维修开始时间不能大于结束时间");
        }
        else
        {
            RepairOrder repairOrder = TheGenericMgr.FindById<RepairOrder>(this.Code);
            repairOrder.SubmitDept = this.tbSubmitDept.Text.Trim();
            repairOrder.SubmitTime = Convert.ToDateTime(this.tbSubmitTime.Text.Trim());
            repairOrder.FaultDescription = this.tbFaultDescription.Text.Trim();
            repairOrder.HaltStartTime = Convert.ToDateTime(this.tbHaltStartTime.Text.Trim());
            if (!string.IsNullOrEmpty(tbHaltEndTime.Text.Trim()))
            { 
            repairOrder.HaltEndTime = Convert.ToDateTime(this.tbHaltEndTime.Text.Trim());
            }
            repairOrder.RepairDescription = this.tbRepairDescription.Text.Trim();
            repairOrder.HaltReason = this.tbHaltReason.Text.Trim();
            repairOrder.Items = this.tbItems.Text.Trim();
            repairOrder.RefOrderNo = this.tbRefOrderNo.Text.Trim();
            repairOrder.Suggestion = this.tbSuggestion.Text.Trim();
            repairOrder.LastModifyDate = DateTime.Now;
            repairOrder.LastModifyUser = this.CurrentUser.Code;
            repairOrder.RepairStartTime = Convert.ToDateTime(tbRepairStartTime.Text.Trim());
            repairOrder.RepairEndTime = Convert.ToDateTime(tbRepairEndTime.Text.Trim()); 
           

            string RepairUser = tbRepairUser.Text.Trim();
            if (!string.IsNullOrEmpty(RepairUser))
            {
                string[] userCodeName = ISIUtil.GetUserSplit(RepairUser);

                string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                if (!string.IsNullOrEmpty(invalidUser))
                {
                    ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
                    return;
                }

                if (userCodeName != null && userCodeName.Length == 2)
                {
                    var assignStartUserCode = ISIUtil.GetUser(userCodeName[0]);
                    if (repairOrder.RepairUser != assignStartUserCode)
                    {
                        repairOrder.RepairUser = assignStartUserCode;
                        //  maintainPlan.AssignStartUserNm = userCodeName[1];
                    }
                }
            }
            else
            {
                repairOrder.RepairUser = string.Empty;
            }
            repairOrder.RepairUserName = this.TheUserSubscriptionMgr.GetUserName(repairOrder.RepairUser);

            this.TheGenericMgr.Update(repairOrder);
            ShowSuccessMessage("更新成功");
            InitPageParameter(Code);
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            RepairOrder repairOrder = this.TheGenericMgr.FindById<RepairOrder>(Code);
            if (repairOrder.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                ShowErrorMessage("非创建状态的设备报修单不能删除");
            }
            else
            { 
                //this.TheGenericMgr.Delete(repairOrder);
                #region 更新状态
                FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(lblFCID.Text.Trim());
                facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE;
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
                //facilityTrans.StartDate = Convert.ToDateTime(tbRepairStartTime.Text.Trim());
                //facilityTrans.EndDate = Convert.ToDateTime(tbRepairEndTime.Text.Trim());
                facilityTrans.Remark = "报修单号："+repairOrder.OrderNo+" 被删除";
                facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_FIX_FINISH;
                #endregion
                TheFacilityMasterMgr.UpdateFacilityAndCreateFacilityTransAndRepairOrder(facilityMaster, facilityTrans, repairOrder, "Delete");
                ShowSuccessMessage("删除成功");
            }
            Back(sender, e);
        }
        catch (BusinessException ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        if(tbSuggestion.Text.Trim()!="")
        {
            #region RepairOrder
            RepairOrder repairOrder = this.TheGenericMgr.FindById<RepairOrder>(Code);
            repairOrder.LastModifyDate = DateTime.Now;
            repairOrder.LastModifyUser = this.CurrentUser.Code;
            repairOrder.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;

            repairOrder.Suggestion = this.tbSuggestion.Text.Trim();
            repairOrder.SuggestionUser = this.CurrentUser.Code;
            repairOrder.SuggestionUserName = this.CurrentUser.CodeName;
            #endregion
            #region 更新状态
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(lblFCID.Text.Trim());
            facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE;
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
            facilityTrans.StartDate = Convert.ToDateTime(tbRepairStartTime.Text.Trim());
            facilityTrans.EndDate = Convert.ToDateTime(tbRepairEndTime.Text.Trim());
            facilityTrans.Remark = tbRepairDescription.Text.Trim();
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_FIX_FINISH;
            #endregion
            TheFacilityMasterMgr.UpdateFacilityAndCreateFacilityTransAndRepairOrder(facilityMaster, facilityTrans, repairOrder,"Update");

            //TheGenericMgr.Update(repairOrder);
            ShowSuccessMessage("关闭成功");
        
            InitPageParameter(this.Code);
        }
        else
        {
            ShowWarningMessage("工程师主管意见不能为空");
        }

    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        RepairOrder repairOrder = this.TheGenericMgr.FindById<RepairOrder>(Code);
        IList<object> list = new List<object>();
        list.Add(repairOrder);
        string template = "RepairOrderSheet.xls";
        string printUrl = TheReportMgr.WriteToFile(template, list);
        Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + printUrl + "'); </script>");
        ShowSuccessMessage("打印成功");
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {

        RepairOrder repairOrder = this.TheGenericMgr.FindById<RepairOrder>(Code);
        IList<object> list = new List<object>();
        list.Add(repairOrder);
        TheReportMgr.WriteToClient("RepairOrderSheet.xls", list, "RepairOrderSheet.xls");
    }

    protected void btnAcceptance_Click(object sender, EventArgs e)
    {
        if (tbHaltEndTime.Text.Trim() != "")
        {
            RepairOrder repairOrder = TheGenericMgr.FindById<RepairOrder>(this.Code);
            repairOrder.HaltEndTime = Convert.ToDateTime(this.tbHaltEndTime.Text.Trim());
            repairOrder.OperateUser = this.CurrentUser.Code;
            repairOrder.OperateUserName = this.CurrentUser.CodeName;

            repairOrder.LastModifyDate = DateTime.Now;
            repairOrder.LastModifyUser = this.CurrentUser.Code;

            repairOrder.Status = "等待关闭";
            repairOrder.AcceptanceTime = DateTime.Now;

            this.TheGenericMgr.Update(repairOrder);
            ShowSuccessMessage("更新成功");
            InitPageParameter(Code);
        }
        else
        {
            ShowWarningMessage("停机结束时间不能为空");
        }
    }
    protected void btnRepair_Click(object sender, EventArgs e)
    {
        if (tbRepairDescription.Text.Trim() == "" || tbHaltReason.Text.Trim() == "" || tbItems.Text.Trim()==""
            || tbRepairUser.Text.Trim() == "" || tbRepairStartTime.Text.Trim()=="" || tbRepairEndTime.Text.Trim()=="")
        {
            ShowWarningMessage("维修时间、人员、步骤、原因、备件为必填项");
        }
        else if (Convert.ToDateTime(tbRepairStartTime.Text.Trim()) > Convert.ToDateTime(tbRepairEndTime.Text.Trim()))
        {
            ShowWarningMessage("维修开始时间不能大于结束时间");
        }
        else
        {
            RepairOrder repairOrder = TheGenericMgr.FindById<RepairOrder>(this.Code);
            repairOrder.RepairDescription = this.tbRepairDescription.Text.Trim();
            repairOrder.HaltReason = this.tbHaltReason.Text.Trim();
            repairOrder.Items = this.tbItems.Text.Trim();
            repairOrder.RefOrderNo = this.tbRefOrderNo.Text.Trim();
            repairOrder.LastModifyDate = DateTime.Now;
            repairOrder.LastModifyUser = this.CurrentUser.Code;

            repairOrder.Status = "等待验收";
            repairOrder.RepairStartTime = Convert.ToDateTime(tbRepairStartTime.Text.Trim());
            repairOrder.RepairEndTime = Convert.ToDateTime(tbRepairEndTime.Text.Trim());
            if(!string.IsNullOrEmpty(tbHaltEndTime.Text.Trim()))
            {
                repairOrder.HaltEndTime = Convert.ToDateTime(tbHaltEndTime.Text.Trim());
            }

            string RepairUser = tbRepairUser.Text.Trim();
            if (!string.IsNullOrEmpty(RepairUser))
            {
                string[] userCodeName = ISIUtil.GetUserSplit(RepairUser);

                string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                if (!string.IsNullOrEmpty(invalidUser))
                {
                    ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
                    return;
                }

                if (userCodeName != null && userCodeName.Length == 2)
                {
                    var assignStartUserCode = ISIUtil.GetUser(userCodeName[0]);
                    if (repairOrder.RepairUser != assignStartUserCode)
                    {
                        repairOrder.RepairUser = assignStartUserCode;
                        //  maintainPlan.AssignStartUserNm = userCodeName[1];
                    }
                }
            }
            else
            {
                repairOrder.RepairUser = string.Empty;
            }
            repairOrder.RepairUserName = this.TheUserSubscriptionMgr.GetUserName(repairOrder.RepairUser);

            this.TheGenericMgr.Update(repairOrder);
            ShowSuccessMessage("更新成功");
            InitPageParameter(Code);
         }
    }
    protected void btnPost_Click(object sender, EventArgs e)
    {
        RepairOrder repairOrder = TheGenericMgr.FindById<RepairOrder>(this.Code);
        repairOrder.SubmitDept = tbSubmitDept.Text.Trim();
        repairOrder.SubmitTime = Convert.ToDateTime(tbSubmitTime.Text.Trim());
        repairOrder.HaltStartTime = Convert.ToDateTime(tbHaltStartTime.Text.Trim());
        repairOrder.FaultDescription = tbFaultDescription.Text;
        repairOrder.LastModifyDate = DateTime.Now;
        repairOrder.LastModifyUser = this.CurrentUser.Code;

        repairOrder.Status = "等待维修";
        this.TheGenericMgr.Update(repairOrder);
        InitPageParameter(Code);

    }
}