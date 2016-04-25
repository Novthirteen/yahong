using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;

//TODO:Add other using statements here.by liqiuyun
public partial class Modules_ISI_ResPatrol_New : NewModuleBase
{
    public event EventHandler Back;
    public event EventHandler Create;
    public object name
    {
        get { return ViewState["name"]; }
        set { ViewState["name"] = value; }
    }
    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        //Add code for Page_Load here.
        //Controls_TextBox tbLocFrom = (Controls_TextBox)this.FV_ResPatrol.FindControl("tbLocFrom");
        //tbLocFrom.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:";
        //tbLocFrom.DataBind();
        if (!IsPostBack)
        {

        }
    }

    protected void ODS_ResPatrol_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        ResPatrol dataItem = (ResPatrol)e.InputParameters[0];
        this.name = dataItem.Id;
        dataItem.Role = ((Controls_TextBox)(this.FV_ResPatrol.FindControl("tbRole"))).Text.Trim();
        dataItem.WorkShop = ((Controls_TextBox)(this.FV_ResPatrol.FindControl("tbWorkShop"))).Text.Trim();
        dataItem.CreateUser = this.CurrentUser.Code;
        dataItem.CreateDate = DateTime.Now;
        dataItem.LastModifyUser = this.CurrentUser.Code;
        dataItem.LastModifyDate = DateTime.Now;
    }

    protected void ODS_ResPatrol_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Insert.Failed");
            e.ExceptionHandled = true;
        }
        else
        {
            if (Create != null)
            {
                Create(this.name, e);
                ShowSuccessMessage("Common.Business.Result.Insert.Successfully");
            }
        }
    }

    //The event handler when user click button "Back"
    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            Back(this, e);
        }
    }

    public void PageCleanup()
    {
        //((TextBox)(this.FV_ResPatrol.FindControl("tbId"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_ResPatrol.FindControl("tbWorkShop"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_ResPatrol.FindControl("tbRole"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbWinTime1"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbWinTime2"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbWinTime3"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbWinTime4"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbWinTime5"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbWinTime6"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbWinTime7"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbNextOrderTime"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbNextWinTime"))).Text = string.Empty;
        ((TextBox)(this.FV_ResPatrol.FindControl("tbLeadTime"))).Text = string.Empty;
    }
}