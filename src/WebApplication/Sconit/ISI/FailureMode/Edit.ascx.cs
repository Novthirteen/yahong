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

public partial class ISI_FailureMode_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected string FailureModeCode
    {
        get
        {
            return (string)ViewState["FailureModeCode"];
        }
        set
        {
            ViewState["FailureModeCode"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string code)
    {
        this.FailureModeCode = code;
        this.ODS_FailureMode.SelectParameters["Code"].DefaultValue = this.FailureModeCode;
        this.ODS_FailureMode.DeleteParameters["Code"].DefaultValue = this.FailureModeCode;
    }

    protected void FV_FailureMode_DataBound(object sender, EventArgs e)
    {
        if (FailureModeCode != null)
        {
            FailureMode failureMode = (FailureMode)((FormView)sender).DataItem;
            UpdateView(failureMode);
        }
    }

    private void UpdateView(FailureMode failureMode)
    {

        Controls_TextBox tbTaskSubType = (Controls_TextBox)this.FV_FailureMode.FindControl("tbTaskSubType");

        if (failureMode.TaskSubType != null)
        {
            tbTaskSubType.Text = failureMode.TaskSubType.Code;
        }

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FailureMode_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.FailureMode.UpdateFailureMode.Successfully", FailureModeCode);
        btnBack_Click(this, e);
    }

    protected void ODS_FailureMode_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FailureMode failureMode = (FailureMode)e.InputParameters[0];
        if (failureMode != null)
        {
            FailureMode oldFailureMode = TheFailureModeMgr.LoadFailureMode(failureMode.Code);
            failureMode.CreateDate = oldFailureMode.CreateDate;
            failureMode.CreateUser = oldFailureMode.CreateUser;

            string taskSubTypeCode = ((Controls_TextBox)(this.FV_FailureMode.FindControl("tbTaskSubType"))).Text.Trim();
            if (!string.IsNullOrEmpty(taskSubTypeCode))
            {
                //TaskSubType taskSubType = new TaskSubType();
                //taskSubType.Code = taskSubTypeCode;
                failureMode.TaskSubType = TheTaskSubTypeMgr.LoadTaskSubType(taskSubTypeCode);
            }
            failureMode.LastModifyDate = DateTime.Now;
            failureMode.LastModifyUser = this.CurrentUser.Code;
        }

    }

    protected void ODS_FailureMode_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FailureMode failureMode = (FailureMode)e.InputParameters[0];
        //IList<FailureMode> failureModeList = TheFailureModeMgr.GetFailureModeByParent(failureMode.Code);
        //if (failureModeList != null && failureModeList.Count > 0)
        if (TheFailureModeMgr.IsRef(failureMode.Code))
        {
            ShowErrorMessage("ISI.FailureMode.DeleteFailureMode.Ref.Fail", FailureModeCode.ToString());
            e.Cancel = true;
        }
    }
    protected void ODS_FailureMode_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.FailureMode.DeleteFailureMode.Successfully", FailureModeCode);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("ISI.FailureMode.DeleteFailureMode.Fail", FailureModeCode);
            e.ExceptionHandled = true;
        }
    }
}
