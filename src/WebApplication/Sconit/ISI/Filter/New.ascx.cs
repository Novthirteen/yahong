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
using com.Sconit.Control;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Filter_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;

    private Filter filter;
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

    protected void FV_Filter_OnDataBinding(object sender, EventArgs e)
    {

    }

    public void PageCleanup()
    {
        ((TextBox)(this.FV_Filter.FindControl("tbTaskCode"))).Text = string.Empty;
        ((TextBox)(this.FV_Filter.FindControl("tbDesc"))).Text = string.Empty;
        ((TextBox)(this.FV_Filter.FindControl("tbEmail"))).Text = string.Empty;
        ((CodeMstrDropDownList)(this.FV_Filter.FindControl("ddlTaskType"))).SelectedIndex = 0;
        ((Controls_TextBox)(this.FV_Filter.FindControl("tbTaskSubType"))).Text = string.Empty;
    }

    protected void ODS_Filter_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        filter = (Filter)e.InputParameters[0];

        if (filter != null)
        {
            filter.TaskSubType = ((Controls_TextBox)(this.FV_Filter.FindControl("tbTaskSubType"))).Text.Trim();
            filter.TaskType = ((CodeMstrDropDownList)(this.FV_Filter.FindControl("ddlTaskType"))).SelectedValue;

            DateTime now = DateTime.Now;
            if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_PERMISSION_FILTER_VALUE_FILTERADMIN))
            {
                filter.UserName = this.CurrentUser.Name;
            }
            else if (!string.IsNullOrEmpty(filter.UserCode))
            {
                filter.UserName = this.TheUserMgr.LoadUser(filter.UserCode).Name;
            }
            filter.CreateDate = now;
            filter.CreateUser = this.CurrentUser.Code;
            filter.CreateUserNm = this.CurrentUser.Name;
            filter.LastModifyDate = now;
            filter.LastModifyUser = this.CurrentUser.Code;
            filter.LastModifyUserNm = this.CurrentUser.Name;
        }
    }

    protected void ODS_Filter_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(filter.Id.ToString(), e);
            ShowSuccessMessage("ISI.Filter.AddFilter.Successfully", filter.UserCode);
        }
    }

    protected void FV_Filter_DataBound(object sender, EventArgs e)
    {
        if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_PERMISSION_FILTER_VALUE_FILTERADMIN))
        {
            Controls_TextBox tbUserCode = (Controls_TextBox)this.FV_Filter.FindControl("tbUserCode");
            tbUserCode.Visible = false;
            tbUserCode.Text = this.CurrentUser.Code;

            TextBox rtbUserCode = (TextBox)this.FV_Filter.FindControl("rtbUserCode");
            rtbUserCode.Text = this.CurrentUser.Code;
            rtbUserCode.Visible = true;

            TextBox tbEmail = (TextBox)this.FV_Filter.FindControl("tbEmail");
            tbEmail.ReadOnly = true;
        }

        CodeMstrDropDownList ddlTaskType = (CodeMstrDropDownList)this.FV_Filter.FindControl("ddlTaskType");
        ddlTaskType.Items.RemoveAt(1);
    }
}
