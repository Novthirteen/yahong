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
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Web;
using com.Sconit.Control;


public partial class ISI_CheckupProject_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;

    private CheckupProject checkupProject;

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
        ((CheckBox)(this.FV_CheckupProject.FindControl("tbIsActive"))).Checked = true;
        ((TextBox)(this.FV_CheckupProject.FindControl("tbCode"))).Text = string.Empty;
        ((TextBox)(this.FV_CheckupProject.FindControl("tbDesc"))).Text = string.Empty;
    }

    protected void ODS_CheckupProject_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        CodeMstrDropDownList ddlType = (CodeMstrDropDownList)this.FV_CheckupProject.FindControl("ddlType");
        checkupProject = (CheckupProject)e.InputParameters[0];
        if (checkupProject != null)
        {
            checkupProject.Code = checkupProject.Code.Trim();
            checkupProject.Desc = checkupProject.Desc.Trim();
            checkupProject.CreateUser = this.CurrentUser.Code;
            checkupProject.CreateUserNm = this.CurrentUser.Name;
            checkupProject.CreateDate = DateTime.Now;
            checkupProject.LastModifyDate = DateTime.Now;
            checkupProject.LastModifyUser = this.CurrentUser.Code;
            checkupProject.LastModifyUserNm = this.CurrentUser.Name;
            checkupProject.Type = ddlType.SelectedValue;
        }
    }

    protected void ODS_CheckupProject_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(checkupProject.Code, e);
            ShowSuccessMessage("ISI.CheckupProject.AddCheckupProject.Successfully", checkupProject.Code);
        }
    }

    protected void checkCheckupProject(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvInsert":
                if (TheCheckupProjectMgr.LoadCheckupProject(args.Value) != null)
                {
                    ShowErrorMessage("ISI.CheckupProject.CodeExist", args.Value);
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }
}