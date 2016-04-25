using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityCategory_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected string FacilityCategoryCode
    {
        get
        {
            return (string)ViewState["FacilityCategoryCode"];
        }
        set
        {
            ViewState["FacilityCategoryCode"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_FacilityCategory_DataBound(object sender, EventArgs e)
    {
        FacilityCategory facilityCategory = (FacilityCategory)(((FormView)(sender)).DataItem);
        ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbParentCategory"))).Text = facilityCategory.ParentCategory;
        ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbChargePerson"))).Text = facilityCategory.ChargePerson;
   
    }

    public void InitPageParameter(string code)
    {
        this.FacilityCategoryCode = code;
        this.ODS_FacilityCategory.SelectParameters["code"].DefaultValue = this.FacilityCategoryCode;
        this.ODS_FacilityCategory.DataBind();
        this.FV_FacilityCategory.DataBind();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FacilityCategory_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("Facility.Facility.UpdateFacilityCategory.Successfully", this.FacilityCategoryCode);
    }

    protected void ODS_FacilityCategory_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FacilityCategory facilityCategory = (FacilityCategory)e.InputParameters[0];
        facilityCategory.ParentCategory = ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbParentCategory"))).Text;
        facilityCategory.ChargePerson = ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbChargePerson"))).Text.Trim();
        if (TheUserMgr.LoadUser(facilityCategory.ChargePerson) != null)
        {
            facilityCategory.ChargePersonName = TheUserMgr.LoadUser(facilityCategory.ChargePerson).Name;
        }
    }
    protected void ODS_FacilityCategory_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_FacilityCategory_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.Facility.DeleteFacilityCategory.Successfully", this.FacilityCategoryCode);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.Facility.DeleteFacilityCategory.Failed", this.FacilityCategoryCode);
            e.ExceptionHandled = true;
        }
    }

}
