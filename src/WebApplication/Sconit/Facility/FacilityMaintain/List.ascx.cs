using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using System.IO;
using com.Sconit.Entity;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityMaintain_List : ListModuleBase
{
    public EventHandler MaintainFinishEvent;
    public EventHandler MaintainStartEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnMaintainStart_Click(object sender, EventArgs e)
    {

        //if (MaintainStartEvent != null)
        //{
        //    string code = ((LinkButton)sender).CommandArgument;
        //    MaintainStartEvent(code, e);
        //}

        string fcId = ((LinkButton)sender).CommandArgument;
        try
        {
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(fcId);

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
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_START;


            TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_MAINTAIN, this.CurrentUser.Code);
            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterMaintainStart.Successfully", fcId);
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("设施{0}开始保养失败,失败原因{1}", fcId, ex.Message);
        }
    }


    protected void lbtnMaintainFinish_Click(object sender, EventArgs e)
    {
        if (MaintainFinishEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            MaintainFinishEvent(code, e);
        }
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtnMaintainStart = (LinkButton)e.Row.FindControl("lbtnMaintainStart");
            LinkButton lbtnMaintainFinish = (LinkButton)e.Row.FindControl("lbtnMaintainFinish");

            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Status, this.CurrentUser);

            Label lblOwner = (Label)(e.Row.FindControl("lblOwner"));
            lblOwner.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Owner, this.CurrentUser);

            Label lblCategory = (Label)(e.Row.FindControl("lblCategory"));
            FacilityCategory facilityCategory = this.TheFacilityCategoryMgr.LoadFacilityCategory(facilityMaster.Category);
            if (facilityCategory != null)
            {
                lblCategory.Text = facilityCategory.Description;
            }
            if (facilityMaster.Status == FacilityConstants.CODE_MASTER_FACILITY_STATUS_MAINTAIN)
            {
                lbtnMaintainStart.Visible = false;
                lbtnMaintainFinish.Visible = true;
            }
            else
            {
                lbtnMaintainStart.Visible = true;
                lbtnMaintainFinish.Visible = false;
            }
        }
    }
}
