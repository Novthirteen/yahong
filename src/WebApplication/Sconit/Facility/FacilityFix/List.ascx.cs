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

public partial class Facility_FacilityFix_List : ListModuleBase
{
    public EventHandler FixStartEvent;
    public EventHandler FixFinishEvent;

    protected string FCID
    {
        get
        {
            return (string)ViewState["FCID"];
        }
        set
        {
            ViewState["FCID"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      //  this.ucConfirmInfo.ConfirmEvent += new System.EventHandler(this.FixRender);
    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }


    protected void lbtnFixFinish_Click(object sender, EventArgs e)
    {
        if (FixFinishEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            FixFinishEvent(code, e);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtnFixStart = (LinkButton)e.Row.FindControl("lbtnFixStart");
            LinkButton lbtnFixFinish = (LinkButton)e.Row.FindControl("lbtnFixFinish");

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
            if (facilityMaster.Status == FacilityConstants.CODE_MASTER_FACILITY_STATUS_FIX)
            {
                lbtnFixStart.Visible = false;
                lbtnFixFinish.Visible = true;
            }
            else
            {
                lbtnFixStart.Visible = true;
                lbtnFixFinish.Visible = false;
            }
        }
    }


    protected void lbtnFixStart_Click(object sender, EventArgs e)
    {
        this.FCID = ((LinkButton)sender).CommandArgument;
        //this.ucConfirmInfo.Visible = true;

        FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
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


        TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_FIX, this.CurrentUser.Code);
        UpdateView();
        ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterFixStart.Successfully", this.FCID);
    }
}
