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

public partial class Facility_FacilityLend_List : ListModuleBase
{
    public EventHandler EditEvent;
    public EventHandler TransferEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnLend_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }

    protected void lbtnReturn_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(code);
            FacilityCategory facilityCategory = TheFacilityCategoryMgr.LoadFacilityCategory(facilityMaster.Category);
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = this.CurrentUser.Code;
            facilityTrans.EffDate = DateTime.Now.Date;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePerson = facilityCategory.ChargePerson;
            facilityTrans.ToChargePersonName = facilityCategory.ChargePersonName;
            facilityTrans.ToOrganization = facilityCategory.ChargeOrganization;
            facilityTrans.ToChargeSite = facilityCategory.ChargeSite;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_RETURN;

            facilityMaster.IsInStore = true;
            facilityMaster.ChargeDate = DateTime.Now;
            facilityMaster.OldChargePerson = facilityMaster.CurrChargePerson;
            facilityMaster.OldChargePersonName = facilityMaster.CurrChargePersonName;
            facilityMaster.CurrChargePerson = facilityCategory.ChargePerson;
            facilityMaster.CurrChargePersonName = facilityCategory.ChargePersonName;
            facilityMaster.ChargeOrganization = facilityCategory.ChargeOrganization;
            facilityMaster.ChargeSite = facilityCategory.ChargeSite;
            //facilityMaster.ChargeSite = applySite;
            //facilityMaster.ChargeOrganization = applyOrg;
            //TheFacilityMasterMgr.DeleteFacilityMaster(code);
            //TheItemMgr.DeleteItem(code);
            TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, this.CurrentUser.Code);
            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterReturn.Successfully", code);
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("设施{0}归还失败,失败原因{1}", code, ex.Message);
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
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

            if (facilityMaster.Status == FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE)
            {
                LinkButton lbtnLend = (LinkButton)e.Row.FindControl("lbtnLend");
                if (lbtnLend != null)
                {
                    lbtnLend.Visible = true;
                }
            }
            else if (facilityMaster.Status == FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND)
            {
                LinkButton lbtnReturn = (LinkButton)e.Row.FindControl("lbtnReturn");
                if (lbtnReturn != null)
                {
                    lbtnReturn.Visible = true;
                }
            }
        }
    }
}
