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
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Control;


public partial class ISI_CheckupProject_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected string CheckupCode
    {
        get
        {
            return (string)ViewState["CheckupCode"];
        }
        set
        {
            ViewState["CheckupCode"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {


    }

    protected void FV_CheckupProject_DataBound(object sender, EventArgs e)
    {
        if (CheckupCode != null && CheckupCode != string.Empty)
        {
            CheckupProject checkupProject = (CheckupProject)((FormView)sender).DataItem;
            CodeMstrDropDownList ddlType = (CodeMstrDropDownList)this.FV_CheckupProject.FindControl("ddlType");
            if (checkupProject.Type != null)
            {
                ddlType.Text = checkupProject.Type;
            }
        }
    }

    public void InitPageParameter(string code)
    {
        this.CheckupCode = code;
        this.ODS_CheckupProject.SelectParameters["code"].DefaultValue = this.CheckupCode;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_CheckupProject_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.CheckupProject.UpdateCheckupProject.Successfully", CheckupCode);
        btnBack_Click(this, e);
    }

    protected void ODS_CheckupProject_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        CodeMstrDropDownList ddlType = (CodeMstrDropDownList)this.FV_CheckupProject.FindControl("ddlType");
        CheckupProject checkUpProject = (CheckupProject)e.InputParameters[0];
        if (checkUpProject != null)
        {
            CheckupProject oldCheckUpProject = this.TheCheckupProjectMgr.LoadCheckupProject(checkUpProject.Code);
            checkUpProject.CreateDate = oldCheckUpProject.CreateDate;
            checkUpProject.CreateUser = oldCheckUpProject.CreateUser;
            checkUpProject.CreateUserNm = oldCheckUpProject.CreateUserNm;
            checkUpProject.Desc = checkUpProject.Desc.Trim();
            checkUpProject.LastModifyDate = DateTime.Now;
            checkUpProject.LastModifyUser = this.CurrentUser.Code;
            checkUpProject.LastModifyUserNm = this.CurrentUser.Name;
            checkUpProject.Type = ddlType.SelectedValue;
        }
    }

    protected void ODS_CheckupProject_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.CheckupProject.DeleteCheckupProject.Successfully", CheckupCode);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("ISI.CheckupProject.DeleteCheckupProject.Fail", CheckupCode);
            e.ExceptionHandled = true;
        }
    }
}