using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using com.Sconit.Control;

//TODO: Add other using statements here.by liqiuyun
public partial class Modules_ISI_ResRole_Edit : EditModuleBase
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
        this.ODS_ResRole.SelectParameters["Code"].DefaultValue = Code.ToString();
        this.ODS_ResRole.DeleteParameters["Code"].DefaultValue = Code.ToString();
        this.FV_ResRole.DataBind();
    }

	protected void FV_ResRole_DataBound(object sender, EventArgs e)
    {
        ResRole dataItem = (ResRole)this.FV_ResRole.DataItem;
        if (dataItem != null)
        {
            CodeMstrDropDownList ddlRoleType = (CodeMstrDropDownList)this.FV_ResRole.FindControl("ddlRoleType");
            ddlRoleType.SelectedValue = dataItem.RoleType;
        }
    }

    protected void ODS_ResRole_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        ResRole dataItem = (ResRole)e.InputParameters[0];
        dataItem.LastModifyDate = DateTime.Now;
        dataItem.LastModifyUser = this.CurrentUser.Code;
        CodeMstrDropDownList ddlRoleType = (CodeMstrDropDownList)this.FV_ResRole.FindControl("ddlRoleType");
        dataItem.RoleType = ddlRoleType.SelectedValue;
    }

    protected void ODS_ResRole_Updated(object sender, ObjectDataSourceStatusEventArgs e)
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

    protected void ODS_ResRole_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
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