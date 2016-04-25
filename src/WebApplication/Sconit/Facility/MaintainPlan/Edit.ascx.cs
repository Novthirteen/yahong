using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;
using com.Sconit.ISI.Service.Util;

public partial class Facility_MaintainPlan_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected string MaintainPlanCode
    {
        get
        {
            return (string)ViewState["MaintainPlanCode"];
        }
        set
        {
            ViewState["MaintainPlanCode"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_MaintainPlan_DataBound(object sender, EventArgs e)
    {
        MaintainPlan maintainPlan = (MaintainPlan)(((FormView)(sender)).DataItem);

        if (maintainPlan != null)
        {
            ((CodeMstrDropDownList)(this.FV_MaintainPlan.FindControl("ddlType"))).SelectedValue = maintainPlan.Type;
            ((Controls_TextBox)(this.FV_MaintainPlan.FindControl("tbCategory"))).Text = (maintainPlan.FacilityCategory == null) ? string.Empty : maintainPlan.FacilityCategory;

            if (!string.IsNullOrEmpty(maintainPlan.StartUpUser))
            {
                TextBox tbAssignStartUser = (TextBox)this.FV_MaintainPlan.FindControl("tbAssignStartUser");

                string userNames = this.TheUserSubscriptionMgr.GetUserName(maintainPlan.StartUpUser);

                tbAssignStartUser.Text = ISIUtil.GetUserMerge(maintainPlan.StartUpUser, userNames);

            }
        }
    }

    public void InitPageParameter(string code)
    {
        this.MaintainPlanCode = code;
        this.ODS_MaintainPlan.SelectParameters["code"].DefaultValue = this.MaintainPlanCode;
        this.ODS_MaintainPlan.DataBind();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_MaintainPlan_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("Facility.Facility.UpdateMaintainPlan.Successfully", this.MaintainPlanCode);
    }

    protected void ODS_MaintainPlan_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        MaintainPlan maintainPlan = (MaintainPlan)e.InputParameters[0];
        maintainPlan.Type = ((CodeMstrDropDownList)(this.FV_MaintainPlan.FindControl("ddlType"))).SelectedValue;
        maintainPlan.StartUpUser = ISIUtil.GetUser(maintainPlan.StartUpUser);

        string assignStartUser = ((TextBox)this.FV_MaintainPlan.FindControl("tbAssignStartUser")).Text.Trim();

        if (!string.IsNullOrEmpty(maintainPlan.StartUpUser))
        {
            string[] userCodeName = ISIUtil.GetUserSplit(assignStartUser);

            string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
            if (!string.IsNullOrEmpty(invalidUser))
            {
                ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
                return;
            }

            if (userCodeName != null && userCodeName.Length == 2)
            {
                var assignStartUserCode = ISIUtil.GetUser(userCodeName[0]);
                if (maintainPlan.StartUpUser != assignStartUserCode)
                {
                    maintainPlan.StartUpUser = assignStartUserCode;
                  //  maintainPlan.AssignStartUserNm = userCodeName[1];
                }
            }
        }

    }
    protected void ODS_MaintainPlan_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_MaintainPlan_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.Facility.DeleteMaintainPlan.Successfully", this.MaintainPlanCode);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.Facility.DeleteMaintainPlan.Failed", this.MaintainPlanCode);
            e.ExceptionHandled = true;
        }
    }
    protected void checkUser(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvStartUpUser":
                string invalidUser = TheTaskMgr.GetInvalidUser(args.Value, this.CurrentUser.Code);
                if (!string.IsNullOrEmpty(invalidUser))
                {
                    cv.ErrorMessage = this.TheLanguageMgr.TranslateMessage("ISI.Error.UserNotExist", this.CurrentUser, new string[] { invalidUser });
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }
}
