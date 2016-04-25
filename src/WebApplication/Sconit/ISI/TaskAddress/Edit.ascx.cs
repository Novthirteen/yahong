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

public partial class ISI_TaskAddress_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected string TaskAddressCode
    {
        get
        {
            return (string)ViewState["TaskAddressCode"];
        }
        set
        {
            ViewState["TaskAddressCode"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string code)
    {
        this.TaskAddressCode = code;
        this.ODS_TaskAddress.SelectParameters["Code"].DefaultValue = this.TaskAddressCode;
        this.ODS_TaskAddress.DeleteParameters["Code"].DefaultValue = this.TaskAddressCode;
    }

    protected void FV_TaskAddress_DataBound(object sender, EventArgs e)
    {
        if (TaskAddressCode != null)
        {
            TaskAddress taskAddress = (TaskAddress)((FormView)sender).DataItem;
            UpdateView(taskAddress);
        }
    }

    private void UpdateView(TaskAddress taskAddress)
    {

        Controls_TextBox tbParent = (Controls_TextBox)this.FV_TaskAddress.FindControl("tbParent");

        if (taskAddress.Parent != null)
        {
            tbParent.Text = taskAddress.Parent.Code;
        }

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_TaskAddress_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.TaskAddress.UpdateTaskAddress.Successfully", TaskAddressCode);
        btnBack_Click(this, e);
    }

    protected void ODS_TaskAddress_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        TaskAddress taskAddress = (TaskAddress)e.InputParameters[0];
        if (taskAddress != null)
        {
            TaskAddress oldTaskAddress = TheTaskAddressMgr.LoadTaskAddress(taskAddress.Code);
            taskAddress.CreateDate = oldTaskAddress.CreateDate;
            taskAddress.CreateUser = oldTaskAddress.CreateUser;

            string parent = ((Controls_TextBox)(this.FV_TaskAddress.FindControl("tbParent"))).Text.Trim();
            if (parent != null && parent != string.Empty)
            {
                TaskAddress parentTaskAddress = new TaskAddress();
                parentTaskAddress.Code = parent;
                taskAddress.Parent = parentTaskAddress;
            }
            taskAddress.LastModifyDate = DateTime.Now;
            taskAddress.LastModifyUser = this.CurrentUser.Code;
        }

    }

    protected void ODS_TaskAddress_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        TaskAddress taskAddress = (TaskAddress)e.InputParameters[0];
        //IList<TaskAddress> taskAddressList = TheTaskAddressMgr.GetTaskAddressByParent(taskAddress.Code);
        //if (taskAddressList != null && taskAddressList.Count > 0)
        if (TheTaskAddressMgr.IsRef(taskAddress.Code))
        {
            ShowErrorMessage("ISI.TaskAddress.DeleteTaskAddress.Ref.Fail", TaskAddressCode.ToString());
            e.Cancel = true;
        }
    }
    protected void ODS_TaskAddress_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.TaskAddress.DeleteTaskAddress.Successfully", TaskAddressCode);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("ISI.TaskAddress.DeleteTaskAddress.Fail", TaskAddressCode);
            e.ExceptionHandled = true;
        }
    }
}
