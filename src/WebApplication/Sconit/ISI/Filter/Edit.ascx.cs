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
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Control;

using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Filter_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected string FilterId
    {
        get
        {
            return (string)ViewState["FilterId"];
        }
        set
        {
            ViewState["FilterId"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string id)
    {
        this.FilterId = id;
        this.ODS_Filter.SelectParameters["Id"].DefaultValue = this.FilterId;
        this.ODS_Filter.DeleteParameters["Id"].DefaultValue = this.FilterId;
    }

    protected void FV_Filter_DataBound(object sender, EventArgs e)
    {
        if (FilterId != null)
        {
            Filter filter = (Filter)((FormView)sender).DataItem;
            UpdateView(filter);
        }
    }

    private void UpdateView(Filter filter)
    {

        Controls_TextBox tbTaskSubType = (Controls_TextBox)this.FV_Filter.FindControl("tbTaskSubType");

        if (filter.TaskSubType != null)
        {
            tbTaskSubType.Text = filter.TaskSubType;
        }

        CodeMstrDropDownList ddlTaskType = ((CodeMstrDropDownList)(this.FV_Filter.FindControl("ddlTaskType")));
        
        ddlTaskType.Items.RemoveAt(1);
        if (filter.TaskType != null)
        {
            ddlTaskType.SelectedValue = filter.TaskType;
        }

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
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_Filter_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.Filter.UpdateFilter.Successfully", FilterId);
        btnBack_Click(this, e);
    }

    protected void ODS_Filter_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        Filter filter = (Filter)e.InputParameters[0];
        if (filter != null)
        {
            Filter oldFilter = this.TheFilterMgr.LoadFilter(filter.Id);

            if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_PERMISSION_FILTER_VALUE_FILTERADMIN))
            {
                filter.UserName = oldFilter.UserName;
            }
            else if (!string.IsNullOrEmpty(filter.UserCode))
            {
                filter.UserName = this.TheUserMgr.LoadUser(filter.UserCode).Name;
            }

            filter.CreateDate = oldFilter.CreateDate;
            filter.CreateUserNm = oldFilter.CreateUserNm;
            filter.CreateUser = oldFilter.CreateUser;

            string taskSubType = ((Controls_TextBox)(this.FV_Filter.FindControl("tbTaskSubType"))).Text.Trim();
            filter.TaskSubType = taskSubType;

            string taskType = ((CodeMstrDropDownList)(this.FV_Filter.FindControl("ddlTaskType"))).SelectedValue;
            filter.TaskType = taskType;

            filter.LastModifyDate = DateTime.Now;
            filter.LastModifyUser = this.CurrentUser.Code;
            filter.LastModifyUserNm = this.CurrentUser.Name;
        }
    }
    protected void ODS_Filter_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.Filter.DeleteFilter.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("ISI.Filter.DeleteFilter.Fail");
            e.ExceptionHandled = true;
        }
    }
}
