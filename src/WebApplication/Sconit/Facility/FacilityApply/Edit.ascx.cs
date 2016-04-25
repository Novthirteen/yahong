using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;

public partial class Facility_FacilityApply_Edit : EditModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler FinishBackEvent;

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

    }

    protected void FV_FacilityMaster_DataBound(object sender, EventArgs e)
    {
        //FacilityMaster facilityMaster = FacilityMasterMgr.LoadFacilityMaster(FCID);

        FacilityMaster facilityMaster = (FacilityMaster)(((FormView)(sender)).DataItem);
        if (facilityMaster != null)
        {

        }
    }

    public void InitPageParameter(string code)
    {
        this.FCID = code;
        this.ODS_FacilityMaster.SelectParameters["fcId"].DefaultValue = this.FCID;
        this.ODS_FacilityMaster.DataBind();

        //if (!this.CurrentUser.HasPermission(BusinessConstants.PERMISSION_PAGE_MASTERDATA_VALUE_PAGE_EDITITEM))
        //{
        //    ((TextBox)(this.FV_FacilityMaster.FindControl("tbUom"))).ReadOnly = true;
        //    ((CodeMstrDropDownList)(this.FV_FacilityMaster.FindControl("ddlType"))).Enabled = false;
        //    ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbLocation"))).ReadOnly = true;
        //    ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbBom"))).ReadOnly = true;
        //    ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbRouting"))).ReadOnly = true;
        //    ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbFacilityMasterCategory"))).ReadOnly = true;
        //    ((System.Web.UI.WebControls.Image)(this.FV_FacilityMaster.FindControl("imgUpload"))).Visible = false;
        //}
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
            string applyPerson = ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbApplyPerson"))).Text;
            string applySite = ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbApplySite"))).Text;
            string applyOrg = ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbApplyOrg"))).Text;

            if (applyOrg == string.Empty || applySite == string.Empty || applyOrg == string.Empty)
            {
                return;
            }

            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = this.CurrentUser.Code;
            facilityTrans.EffDate = DateTime.Now.Date;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;

            facilityTrans.ToChargePerson = applyPerson;
            if (TheUserMgr.LoadUser(applyPerson) != null)
            {

                facilityTrans.ToChargePersonName = TheUserMgr.LoadUser(applyPerson).Name;
            }
            facilityTrans.ToChargeSite = applySite;
            facilityTrans.ToOrganization = applyOrg;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_APPLY;

            facilityMaster.IsInStore = false;
            facilityMaster.ChargeDate = DateTime.Now;
            facilityMaster.OldChargePerson = facilityMaster.CurrChargePerson;
            facilityMaster.CurrChargePerson = applyPerson;
            if (TheUserMgr.LoadUser(applyPerson) != null)
            {
                facilityMaster.CurrChargePersonName = TheUserMgr.LoadUser(applyPerson).Name;
            }
            else
            {
                facilityMaster.CurrChargePersonName = string.Empty;
            }
            facilityMaster.ChargeSite = applySite;
            facilityMaster.ChargeOrganization = applyOrg;

            TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, this.CurrentUser.Code);
            
            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterApply.Successfully", this.FCID);

            FinishBackEvent(facilityTrans.Id, e);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void ODS_FacilityMaster_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("MasterData.FacilityMaster.UpdateFacilityMaster.Successfully", FCID);
    }

    protected void ODS_FacilityMaster_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
        FacilityMaster facilityMaster = (FacilityMaster)e.InputParameters[0];

        //FacilityTrans facilityTrans = new FacilityTrans();
        //facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
        //facilityTrans.FromParty = facilityMaster.ChargeSite;
        //facilityTrans.ToChargePerson = facilityMaster.ApplyPerson;
        //facilityTrans.ToParty = facilityMaster.ApplySite;
        //facilityTrans.TransType = "Apply";
        //facilityTrans.FCID = facilityMaster.FCID;
        //facilityTrans.EffDate = DateTime.Now;
        //facilityTrans.CreateUser = CurrentUser.Code;
        //facilityTrans.CreateDate = DateTime.Now;
        //this.TheFacilityTransMgr.CreateFacilityTrans(facilityTrans);

        //facilityMaster.Status = BusinessConstants.CODE_MASTER_FACILITY_STATUS_LEND;
        //facilityMaster.ChargeDate = DateTime.Now;
        //facilityMaster.OldChargePerson = facilityMaster.Owner;
        //facilityMaster.CurrChargePerson = facilityMaster.ApplyPerson;
        //facilityMaster.ChargeSite = facilityMaster.ApplySite;
        //facilityMaster.ChargeOrganization = facilityMaster.ApplyOrg;
        //facilityMaster.LastModifyDate = DateTime.Now;
        //facilityMaster.LastModifyUser = this.CurrentUser.Code;
        InitPageParameter(this.FCID);
        ShowSuccessMessage("Facility.FacilityMaster.ScrapFacilityMaster.Successfully", this.FCID);
    }
    protected void ODS_FacilityMaster_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteFacilityMaster = (FacilityMaster)e.InputParameters[0];
    }

    protected void ODS_FacilityMaster_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("MasterData.FacilityMaster.DeleteFacilityMaster.Successfully", FCID);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("MasterData.FacilityMaster.DeleteFacilityMaster.Fail", FCID);
            e.ExceptionHandled = true;
        }
    }
}
