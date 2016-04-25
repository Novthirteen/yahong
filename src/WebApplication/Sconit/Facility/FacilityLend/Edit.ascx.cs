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

public partial class Facility_FacilityLend_Edit : EditModuleBase
{
    public event EventHandler BackEvent;
    //public event EventHandler EditEvent;
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
            string lendPerson = ((TextBox)(this.FV_FacilityMaster.FindControl("tbLendPerson"))).Text;
            string lendSite = ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbLendSite"))).Text;
            string lendOrg = ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbLendOrg"))).Text;
            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = this.CurrentUser.Code;
            facilityTrans.EffDate = DateTime.Now.Date;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePerson = lendPerson;
            if (TheUserMgr.LoadUser(lendPerson) != null)
            {
                facilityTrans.ToChargePersonName = TheUserMgr.LoadUser(lendPerson).Name;
            }
            facilityTrans.ToChargeSite = lendSite;
            facilityTrans.ToOrganization = lendOrg;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_LEND;

            facilityMaster.IsInStore = false;
            facilityMaster.ChargeDate = DateTime.Now;
            facilityMaster.OldChargePerson = facilityMaster.CurrChargePerson;
            facilityMaster.OldChargePersonName = facilityMaster.CurrChargePersonName;
            facilityMaster.CurrChargePerson = lendPerson;
            if (TheUserMgr.LoadUser(lendPerson) != null)
            {
                facilityMaster.CurrChargePersonName = TheUserMgr.LoadUser(lendPerson).Name;
            }
            else
            {
                facilityMaster.CurrChargePersonName = string.Empty;
            }
            facilityMaster.ChargeSite = lendSite;
            facilityMaster.ChargeOrganization = lendOrg;

            TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND, this.CurrentUser.Code);
            InitPageParameter(this.FCID);
            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterLend.Successfully", this.FCID);
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
