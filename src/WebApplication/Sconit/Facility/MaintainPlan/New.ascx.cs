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
using com.Sconit.ISI.Service.Util;

public partial class Facility_MaintainPlan_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private MaintainPlan maintainPlan;

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

        ((TextBox)(this.FV_MaintainPlan.FindControl("tbCode"))).Text = string.Empty;
        ((TextBox)(this.FV_MaintainPlan.FindControl("tbDescription"))).Text = string.Empty;
    }

    protected void ODS_MaintainPlan_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        maintainPlan = (MaintainPlan)e.InputParameters[0];
        maintainPlan.Type = ((CodeMstrDropDownList)(this.FV_MaintainPlan.FindControl("ddlType"))).SelectedValue;
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

    protected void ODS_MaintainPlan_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(maintainPlan.Code, e);
            ShowSuccessMessage("Facility.MaintainPlan.AddMaintainPlan.Successfully", maintainPlan.Code);
        }
    }

    protected void checkMaintainPlanExists(object source, ServerValidateEventArgs args)
    {
        string code = ((TextBox)(this.FV_MaintainPlan.FindControl("tbCode"))).Text;

        if (TheMaintainPlanMgr.LoadMaintainPlan(code) != null)
        {
            args.IsValid = false;
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
