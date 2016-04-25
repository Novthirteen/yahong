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

public partial class Facility_FacilityMaster_Maintain : EditModuleBase
{
    public event EventHandler BackEvent;

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

    private string[] EditFields = new string[]
    {
        "MaintainType",
        "MaintainStartDate",
        "MaintainPeriod",
        "MaintainLeadTime"
    };

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_FacilityMaintain_DataBound(object sender, EventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)(((FormView)(sender)).DataItem);
        ((CodeMstrDropDownList)(this.FV_FacilityMaintain.FindControl("ddlMaintainType"))).SelectedValue = facilityMaster.MaintainType;

    }

    public void InitPageParameter(string code)
    {
        this.FCID = code;
        this.ODS_FacilityMaintain.SelectParameters["fcId"].DefaultValue = this.FCID;
        this.ODS_FacilityMaintain.DataBind();

        #region 这个值一直更新不了
        FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(code);
        ((Label)(this.FV_FacilityMaintain.FindControl("tbNextMaintainTime"))).Text = facilityMaster.NextMaintainTime.HasValue ? facilityMaster.NextMaintainTime.Value.ToString() : string.Empty;
        #endregion
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FacilityMaintain_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {

        ShowSuccessMessage("Facility.FacilityMaster.UpdateFacilityMaintain.Successfully", FCID);
    }

    protected void ODS_FacilityMaintain_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.InputParameters[0];
        FacilityMaster oldFacilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);

        CloneHelper.CopyProperty(oldFacilityMaster, facilityMaster, EditFields, true);

        facilityMaster.MaintainType = ((CodeMstrDropDownList)(this.FV_FacilityMaintain.FindControl("ddlMaintainType"))).SelectedValue;

        facilityMaster.LastModifyDate = DateTime.Now;
        facilityMaster.LastModifyUser = this.CurrentUser.Code;
    }
}
