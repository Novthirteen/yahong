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
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;

public partial class Facility_FacilityMaintain_Finish : EditModuleBase
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

        System.Web.UI.WebControls.DropDownList ddlISITaskCode = (System.Web.UI.WebControls.DropDownList)(this.FV_FacilityTrans.FindControl("ddlISITaskCode"));
        ddlISITaskCode.DataSource = this.GetISITask(this.FCID);
        ddlISITaskCode.DataBind();

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

            string taskCode = ((System.Web.UI.WebControls.DropDownList)(this.FV_FacilityTrans.FindControl("ddlISITaskCode"))).SelectedValue;

            FacilityTrans facilityTrans = new FacilityTrans();
            FacilityTrans oldFacilityTrans = TheFacilityTransMgr.LoadFacilityTrans(Convert.ToInt32(id));

            CloneHelper.CopyProperty(oldFacilityTrans, facilityTrans, EditFields, true);

            if (!string.IsNullOrEmpty(startDate))
            {
                facilityTrans.StartDate = Convert.ToDateTime(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                facilityTrans.EndDate = Convert.ToDateTime(endDate);
            }

            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = this.CurrentUser.Code;
            facilityTrans.EffDate = DateTime.Now.Date;
            facilityTrans.Remark = remark;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_FINISH;

            TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, this.CurrentUser.Code, taskCode);
            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterMaintainFinish.Successfully", this.FCID);

            FinishBackEvent(facilityTrans.Id, e);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private IList<TaskMstr> GetISITask(string fcId)
    {
        DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskMstr));
        criteria.CreateAlias("TaskSubType", "t");
        criteria.Add(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS }));
        criteria.Add(Expression.Eq("t.Code", "SSGL"));
        criteria.Add(Expression.Eq("Desc2", fcId));
        IList<TaskMstr> taskMstrList = TheCriteriaMgr.FindAll<TaskMstr>(criteria);
        return taskMstrList;
    }

}
