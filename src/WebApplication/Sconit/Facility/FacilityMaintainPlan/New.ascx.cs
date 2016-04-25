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

public partial class Facility_MaintainPlan_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public string FCID
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
        if (IsPostBack && !string.IsNullOrEmpty(this.FCID))
        {
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
            this.tbCode.ServiceParameter = "string:" + facilityMaster.Category;
        }
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
        this.tbCode.Text = string.Empty;
        this.tbStartDate.Text = string.Empty;
        this.tbStartQty.Text = string.Empty;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.tbCode.Text.Trim()))
        {
            this.ShowErrorMessage("Facility.FacilityMaintainCode.Empty");
            return;
        }

        FacilityMaintainPlan facilityMaintainPlan = new FacilityMaintainPlan();
        facilityMaintainPlan.FacilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
        facilityMaintainPlan.MaintainPlan = TheMaintainPlanMgr.LoadMaintainPlan(this.tbCode.Text.Trim());
        if (string.IsNullOrEmpty(this.tbStartDate.Text.Trim()))
        {
            facilityMaintainPlan.StartDate = DateTime.Now.Date;
        }
        else
        {
            facilityMaintainPlan.StartDate = Convert.ToDateTime(this.tbStartDate.Text.Trim());
        }

        if (string.IsNullOrEmpty(this.tbStartQty.Text.Trim()))
        {
            facilityMaintainPlan.StartQty = 0;
        }
        else
        {
            facilityMaintainPlan.StartQty = Convert.ToDecimal(this.tbStartQty.Text.Trim());
        }
        TheFacilityMaintainPlanMgr.CreateFacilityMaintainPlan(facilityMaintainPlan);
        this.ShowSuccessMessage("Facility.MaintainPlan.AddMaintainPlan.Successfully");
        CreateEvent(facilityMaintainPlan.Id, e);
    }

}
