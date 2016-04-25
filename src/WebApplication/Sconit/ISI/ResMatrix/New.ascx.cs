using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;

//TODO:Add other using statements here.by liqiuyun
public partial class Modules_ISI_ResMatrix_New : NewModuleBase
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
        if (!IsPostBack)
        {
     
        }
    }

    private CodeMaster GetTimePeriodType(string statusValue)
    {
        return TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE, statusValue);
    }

    protected void ODS_ResMatrix_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        ResMatrix dataItem = (ResMatrix)e.InputParameters[0];
        Controls_TextBox tbWorkShop = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbWorkShop");
        dataItem.WorkShop = tbWorkShop.Text.Trim();
        dataItem.TaskSubType = ((Controls_TextBox)this.FV_ResMatrix.FindControl("tbTaskSubType")).Text.Trim();

        Controls_TextBox tbRole = (Controls_TextBox)this.FV_ResMatrix.FindControl("tbRole");
        dataItem.Role = tbRole.Text.Trim();

        //DropDownList ddlTimePeriodType = (DropDownList)this.FV_ResMatrix.FindControl("ddlTimePeriodType");
        //dataItem.TimePeriodType = ddlTimePeriodType.SelectedValue;

        this.name = dataItem.Id;
        dataItem.CreateUser = this.CurrentUser.Code;
        dataItem.CreateDate = DateTime.Now;
        dataItem.LastModifyUser = this.CurrentUser.Code;
        dataItem.LastModifyDate = DateTime.Now;
    }

    protected void ODS_ResMatrix_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception != null)
        {
            ShowErrorMessage("Common.Business.Result.Update.Failed.Reason", e.Exception.InnerException.Message);
            e.ExceptionHandled = true;
        }
        else
        {
            if (Create != null)
            {
                if (e.ReturnValue != null)
                {
                    this.name = ((ResMatrix)e.ReturnValue).Id;
                }
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
        //((TextBox)(this.FV_ResMatrix.FindControl("tbId"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_ResMatrix.FindControl("tbWorkShop"))).Text = string.Empty;
        ((TextBox)(this.FV_ResMatrix.FindControl("tbOperate"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_ResMatrix.FindControl("tbRole"))).Text = string.Empty;
        ((TextBox)(this.FV_ResMatrix.FindControl("tbResponsibility"))).Text = string.Empty;
        ((TextBox)(this.FV_ResMatrix.FindControl("tbSequence"))).Text = "10";
        ((CheckBox)(this.FV_ResMatrix.FindControl("cbNeedPatrol"))).Checked = true;
        ((Controls_TextBox)(this.FV_ResMatrix.FindControl("tbTaskSubType"))).Text = string.Empty;

        //DropDownList ddlTimePeriodType = (DropDownList)this.FV_ResMatrix.FindControl("ddlTimePeriodType");
        //var codeMaster = GetTimePeriodType(BusinessConstants.CODE_MASTER_TIME_PERIOD_TYPE_VALUE_WEEK);
        //ddlTimePeriodType.SelectedValue = codeMaster.Value;
        //ddlTimePeriodType.Text = codeMaster.Description;

        //((TextBox)(this.FV_ResMatrix.FindControl("tbDirector"))).Text = string.Empty;
        //((TextBox)(this.FV_ResMatrix.FindControl("tbPriority"))).Text = string.Empty;
        //((TextBox)(this.FV_ResMatrix.FindControl("tbSkillLevel"))).Text = string.Empty;
        //((CheckBox)(this.FV_ResMatrix.FindControl("cbNeedPatrol"))).Checked = true;
    }
}