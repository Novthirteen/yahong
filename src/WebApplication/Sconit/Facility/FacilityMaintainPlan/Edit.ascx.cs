using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;

public partial class Facility_MaintainPlan_Edit : EditModuleBase
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

    protected void FV_MaintainPlan_DataBound(object sender, EventArgs e)
    {
        FacilityMaintainPlan facilityMaintainPlan = (FacilityMaintainPlan)(((FormView)(sender)).DataItem);

        if (facilityMaintainPlan != null)
        {
            ((Label)(this.FV_MaintainPlan.FindControl("tbType"))).Text = this.TheLanguageMgr.TranslateMessage(facilityMaintainPlan.MaintainPlan.Type, this.CurrentUser);

        }
    }

    public void InitPageParameter(string id)
    {
        this.Id = Convert.ToInt32(id);
        this.ODS_MaintainPlan.SelectParameters["id"].DefaultValue = id;
        this.ODS_MaintainPlan.DataBind();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FacilityMaintainPlan_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_FacilityMaintainPlan_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.Facility.DeleteMaintainPlan.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.Facility.DeleteMaintainPlan.Failed");
            e.ExceptionHandled = true;
        }
    }

    protected void ODS_FacilityMaintainPlan_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_FacilityMaintainPlan_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        this.ShowSuccessMessage("Facility.MaintainPlan.UpdateMaintainPlan.Successfully");
    }

}
