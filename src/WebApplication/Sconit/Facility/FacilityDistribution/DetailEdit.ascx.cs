using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity;

public partial class Facility_FacilityDistributionDetail_Edit : EditModuleBase
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

    public Int32 FacilityDistributionId
    {
        get
        {
            return (Int32)ViewState["FacilityDistributionId"];
        }
        set
        {
            ViewState["FacilityDistributionId"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_FacilityDistributionDetail_DataBound(object sender, EventArgs e)
    {
        FacilityDistributionDetail facilityDistributionDetail = (FacilityDistributionDetail)(((FormView)(sender)).DataItem);
        ((CodeMstrDropDownList)(this.FV_FacilityDistributionDetail.FindControl("ddlType"))).Text = facilityDistributionDetail.Type;
        FacilityDistribution fd = TheFacilityDistributionMgr.LoadFacilityDistribution(this.FacilityDistributionId);
        if (fd.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE)
        {
            ((com.Sconit.Control.Button)(this.FV_FacilityDistributionDetail.FindControl("btnSave"))).Visible = false;
        }
    }

    public void InitPageParameter(Int32 id)
    {
        this.Id = id;
        this.ODS_FacilityDistributionDetail.SelectParameters["id"].DefaultValue = this.Id.ToString();
        this.ODS_FacilityDistributionDetail.DataBind();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FacilityDistributionDetail_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("Facility.FacilityDistributionDetail.UpdateFacilityDistribution.Successfully");
    }

    protected void ODS_FacilityDistributionDetail_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FacilityDistributionDetail facilityDistributionDetail = (FacilityDistributionDetail)e.InputParameters[0];
        facilityDistributionDetail.FacilityDistribution = TheFacilityDistributionMgr.LoadFacilityDistribution(this.FacilityDistributionId);
        facilityDistributionDetail.Type = ((CodeMstrDropDownList)(this.FV_FacilityDistributionDetail.FindControl("ddlType"))).SelectedValue;


        if (facilityDistributionDetail.FacilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_DISTRIBUTIONCOMPLETE
           && facilityDistributionDetail.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_DISTRIBUTION)
        {
            ShowErrorMessage("Facility.FacilityDistributionDetail.AddFacilityDistributionDetail.Fail", this.TheLanguageMgr.TranslateMessage(facilityDistributionDetail.FacilityDistribution.Status, this.CurrentUser), this.TheLanguageMgr.TranslateMessage(facilityDistributionDetail.Type, this.CurrentUser));
            e.Cancel = true;
        }
        if (facilityDistributionDetail.FacilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_PURCHASECOMPLETE
           && facilityDistributionDetail.Type == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_PROCUREMENT)
        {
            ShowErrorMessage("Facility.FacilityDistributionDetail.AddFacilityDistributionDetail.Fail", this.TheLanguageMgr.TranslateMessage(facilityDistributionDetail.FacilityDistribution.Status, this.CurrentUser), this.TheLanguageMgr.TranslateMessage(facilityDistributionDetail.Type, this.CurrentUser));
            e.Cancel = true;
        }

     
        facilityDistributionDetail.LastModifyDate = DateTime.Now;
        facilityDistributionDetail.LastModifyUser = this.CurrentUser.Code;
    }
    protected void ODS_FacilityDistributionDetail_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_FacilityDistributionDetail_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.FacilityDistributionDetail.DeleteFacilityDistribution.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.FacilityDistributionDetail.DeleteFacilityDistribution.Failed");
            e.ExceptionHandled = true;
        }
    }
}
