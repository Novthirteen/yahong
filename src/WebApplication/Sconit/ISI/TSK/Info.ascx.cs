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
using com.Sconit.Control;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TSK_Info : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;
    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }
    public string TaskCode
    {
        get
        {
            return (string)ViewState["TaskCode"];
        }
        set
        {
            ViewState["TaskCode"] = value;
        }
    }
    public void InitPageParameter(string moduleType, string taskCode)
    {
        this.TaskCode = taskCode;
        this.ModuleType = moduleType;
        this.ODS_TaskMstr.SelectParameters["code"].DefaultValue = TaskCode;

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            ((Literal)(this.FV_TSKView.FindControl("lblFailureMode"))).Text = "${ISI.TSK.Phase}|${ISI.TSK.Seq}:";
            this.FV_TSKView.FindControl("ddlPhase").Visible = true;
            this.FV_TSKView.FindControl("tbSeq").Visible = true;
            this.FV_TSKView.FindControl("rtbFailureMode").Visible = false;
            ((Literal)(this.FV_TSKView.FindControl("ltlTaskSubType"))).Text = "${ISI.TSK.Project}:";

            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
            {
                ((Literal)(this.FV_TSKView.FindControl("lblBackYards"))).Text = "${ISI.TSK.RefTask}:";
            }
        }
    }

    protected void FV_ISI_DataBound(object sender, EventArgs e)
    {
        if (TaskCode != null && TaskCode != string.Empty)
        {
            TaskMstr task = (TaskMstr)((FormView)sender).DataItem;
            ((Literal)(this.FV_TSKView.FindControl("lblStatus"))).Text = "${" + task.Status + "}";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    public void PageCleanup()
    {

    }
}
