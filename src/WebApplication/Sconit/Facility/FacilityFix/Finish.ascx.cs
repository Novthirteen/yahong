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

public partial class Facility_FacilityFix_Finish : EditModuleBase
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

    private string[] EditFields = new string[]
    {
       
    };


    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_FacilityMaster_DataBound(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string code)
    {
        this.FCID = code;
        this.ODS_FacilityMaster.SelectParameters["fcId"].DefaultValue = this.FCID;
        this.ODS_FacilityMaster.DataBind();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void btnFinish_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
            facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE;

            string id = ((HiddenField)(this.FV_FacilityTrans.FindControl("hfId"))).Value;
            string remark = ((TextBox)(this.FV_FacilityTrans.FindControl("tbRemark"))).Text;
            string startDate = ((TextBox)(this.FV_FacilityTrans.FindControl("tbStartDate"))).Text;
            string endDate = ((TextBox)(this.FV_FacilityTrans.FindControl("tbEndDate"))).Text;

            FacilityTrans facilityTrans = new FacilityTrans();
            FacilityTrans oldFacilityTrans = TheFacilityTransMgr.LoadFacilityTrans(Convert.ToInt32(id));

            CloneHelper.CopyProperty(oldFacilityTrans, facilityTrans, EditFields, true);


            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = this.CurrentUser.Code;

            facilityTrans.EffDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(startDate))
            {
                facilityTrans.StartDate = Convert.ToDateTime(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                facilityTrans.EndDate = Convert.ToDateTime(endDate);
            }

            facilityTrans.Remark = remark;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_FIX_FINISH;

            TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, this.CurrentUser.Code);
            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterFixFinish.Successfully", this.FCID);

            FinishBackEvent(facilityTrans.Id, e);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

}
