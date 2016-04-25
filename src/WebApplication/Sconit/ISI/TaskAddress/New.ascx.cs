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

public partial class ISI_TaskAddress_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;

    private TaskAddress taskAddress;
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

    protected void FV_TaskAddress_OnDataBinding(object sender, EventArgs e)
    {

    }

    public void PageCleanup()
    {
        ((TextBox)(this.FV_TaskAddress.FindControl("tbCode"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskAddress.FindControl("tbDesc"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskAddress.FindControl("tbSeq"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_TaskAddress.FindControl("tbParent"))).Text = string.Empty;

    }

    protected void ODS_TaskAddress_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        taskAddress = (TaskAddress)e.InputParameters[0];

        if (taskAddress != null)
        {
            string parent = ((Controls_TextBox)(this.FV_TaskAddress.FindControl("tbParent"))).Text.Trim();

            if (parent != null && parent != string.Empty)
            {
                TaskAddress parentTaskAddress = new TaskAddress();
                parentTaskAddress.Code = parent;
                taskAddress.Parent = parentTaskAddress;
            }
            DateTime now = DateTime.Now;
            taskAddress.CreateDate = now;
            taskAddress.CreateUser = this.CurrentUser.Code;
            taskAddress.LastModifyDate = now;
            taskAddress.LastModifyUser = this.CurrentUser.Code;
        }
    }

    protected void ODS_TaskAddress_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(taskAddress.Code, e);
            ShowSuccessMessage("ISI.TaskAddress.AddTaskAddress.Successfully", taskAddress.Code);
        }
    }

    protected void checkTaskAddress(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvInsert":
                if (TheTaskAddressMgr.LoadTaskAddress(args.Value) != null)
                {
                    ShowErrorMessage("ISI.TaskAddress.CodeExist", args.Value);
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }
}
