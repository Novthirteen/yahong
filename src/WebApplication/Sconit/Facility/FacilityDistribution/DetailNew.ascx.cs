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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using com.Sconit.Utility;
using com.Sconit.Facility.Entity;
using com.Sconit.Control;
using System.Collections.Generic;
using com.Sconit.Entity;

public partial class Facility_FacilityDistributionDetail_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    protected Int32 FacilityDistributionId
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

    private FacilityDistributionDetail facilityDistributionDetail;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    public void PageCleanup()
    {
        ((TextBox)(this.FV_FacilityDistributionDetail.FindControl("tbInvoice"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistributionDetail.FindControl("tbBillDate"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistributionDetail.FindControl("tbPayDate"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistributionDetail.FindControl("tbBillAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistributionDetail.FindControl("tbPayAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistributionDetail.FindControl("tbContact"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistributionDetail.FindControl("tbRemark"))).Text = string.Empty;
    }

    protected void ODS_FacilityDistributionDetail_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        facilityDistributionDetail = (FacilityDistributionDetail)e.InputParameters[0];
        facilityDistributionDetail.FacilityDistribution = TheFacilityDistributionMgr.LoadFacilityDistribution(Convert.ToInt32(this.FacilityDistributionId));
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


        facilityDistributionDetail.CreateDate = DateTime.Now;
        facilityDistributionDetail.LastModifyDate = DateTime.Now;
        facilityDistributionDetail.CreateUser = this.CurrentUser.Code;
        facilityDistributionDetail.LastModifyUser = this.CurrentUser.Code;
      
    }

    protected void ODS_FacilityDistributionDetail_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(facilityDistributionDetail.Id, e);
            ShowSuccessMessage("Facility.FacilityDistributionDetail.AddFacilityDistributionDetail.Successfully");
        }
    }
    public void InitPageParameter(Int32 facilityDistributionId)
    {
        this.FacilityDistributionId = facilityDistributionId;
        PageCleanup();
    }

}
