using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;

//TODO: Add other using statements here.by liqiuyun
public partial class Modules_ISI_ResPatrol_Edit : EditModuleBase
{
    public event EventHandler Back;

    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        //TODO: Add code for Page_Load here.
        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter(object Code)
    {
        this.ODS_ResPatrol.SelectParameters["Id"].DefaultValue = Code.ToString();
        this.ODS_ResPatrol.DeleteParameters["Id"].DefaultValue = Code.ToString();
        this.FV_ResPatrol.DataBind();
    }

    protected void FV_ResPatrol_DataBound(object sender, EventArgs e)
    {
        ResPatrol dataItem = (ResPatrol)this.FV_ResPatrol.DataItem;
        if (dataItem != null)
        {
            Controls_TextBox tbWorkShop = (Controls_TextBox)this.FV_ResPatrol.FindControl("tbWorkShop");
            tbWorkShop.Text = dataItem.WorkShop;
            Controls_TextBox tbRole = (Controls_TextBox)this.FV_ResPatrol.FindControl("tbRole");
            tbRole.Text = dataItem.Role;
        }
    }

    protected void ODS_ResPatrol_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        ResPatrol dataItem = (ResPatrol)e.InputParameters[0];
        dataItem.LastModifyDate = DateTime.Now;
        dataItem.LastModifyUser = this.CurrentUser.Code;
        dataItem.WorkShop = ((Controls_TextBox)this.FV_ResPatrol.FindControl("tbWorkShop")).Text.Trim();
        var role = ((Controls_TextBox)this.FV_ResPatrol.FindControl("tbRole")).Text.Trim();
        role = string.IsNullOrEmpty(role) ? null : role;
        dataItem.Role = role;
    }

    protected void ODS_ResPatrol_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Update.Failed");
            e.ExceptionHandled = true;
        }
        else
        {
            Back(sender, e);
            ShowSuccessMessage("Common.Business.Result.Update.Successfully");
        }
    }

    protected void ODS_ResPatrol_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Delete.Failed");
            e.ExceptionHandled = true;
        }
        else
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Common.Business.Result.Delete.Successfully");
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            this.Visible = false;
            Back(sender, e);
        }
    }
}