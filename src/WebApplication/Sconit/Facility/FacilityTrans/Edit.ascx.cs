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
using System.Collections.Generic;

public partial class Facility_FacilityTrans_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected Int32 Id
    {
        get
        {
            return (Int32)ViewState["Id"];
        }
        set
        {
            ViewState["Id"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_FacilityTrans_DataBound(object sender, EventArgs e)
    {

        FacilityTrans facilityTrans = (FacilityTrans)(((FormView)(sender)).DataItem);
        if (facilityTrans != null)
        {
            ((Label)(this.FV_FacilityTrans.FindControl("tbCategory"))).Text = TheFacilityCategoryMgr.LoadFacilityCategory(facilityTrans.FacilityCategory).Description;
            ((Controls_TextBox)(this.FV_FacilityTrans.FindControl("tbToChargePerson"))).Text = facilityTrans.ToChargePerson;
            ((Label)(this.FV_FacilityTrans.FindControl("tbToChargePersonName"))).Text = facilityTrans.ToChargePersonName;
        }
    }

    public void InitPageParameter(string id)
    {
        this.Id = Convert.ToInt32(id);
        this.ODS_FacilityTrans.SelectParameters["id"].DefaultValue = id;

        this.ODS_FacilityTrans.DataBind();
        UpdateView();
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
        FacilityTrans facilityTrans = TheFacilityTransMgr.LoadFacilityTrans(this.Id);
        string toChargePerson = ((Controls_TextBox)(this.FV_FacilityTrans.FindControl("tbToChargePerson"))).Text;
        string toChargeSite = ((TextBox)(this.FV_FacilityTrans.FindControl("tbToChargeSite"))).Text;
        string toOrganization = ((TextBox)(this.FV_FacilityTrans.FindControl("tbToOrganization"))).Text;
        string startDate = ((TextBox)(this.FV_FacilityTrans.FindControl("tbStartDate"))).Text;
        string endDate = ((TextBox)(this.FV_FacilityTrans.FindControl("tbEndDate"))).Text;
        facilityTrans.ToChargePerson = toChargePerson;
        facilityTrans.ToChargeSite = toChargeSite;
        facilityTrans.ToOrganization = toOrganization;
        if (!string.IsNullOrEmpty(startDate))
        {
            facilityTrans.StartDate = Convert.ToDateTime(startDate);
        }
        if (!string.IsNullOrEmpty(endDate))
        {
            facilityTrans.EndDate = Convert.ToDateTime(endDate);
        }
        if (TheUserMgr.LoadUser(toChargePerson) != null)
        {
            facilityTrans.ToChargePersonName = TheUserMgr.LoadUser(toChargePerson).Name;
        }
        TheFacilityTransMgr.UpdateFacilityTrans(facilityTrans);
        ShowSuccessMessage("Facility.FacilityMaster.FacilityTrans.UpdateSuccessfully");
        this.InitPageParameter(this.Id.ToString());
    }

    public void UpdateView()
    {

    }
}
