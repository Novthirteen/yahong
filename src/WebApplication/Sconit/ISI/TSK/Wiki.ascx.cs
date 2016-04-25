using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;

public partial class ISI_TSK_Wiki : com.Sconit.Web.MainModuleBase
{
    public event EventHandler BackEvent;
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

    private void UpdateView(TaskMstr task)
    {
        this.lgd.InnerText = "${ISI.TSK." + this.ModuleType + "}" + this.TaskCode;
        this.tbWiki.Text = task.Wiki;
    }

    private void PageCleanup()
    {
        this.TaskCode = null;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    public void InitPageParameter(string taskCode)
    {
        this.TaskCode = taskCode;
        TaskMstr task = TheTaskMstrMgr.LoadTaskMstr(taskCode);
        this.UpdateView(task);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        this.TheTaskMstrMgr.UpdateTaskMstr(this.TaskCode, this.tbWiki.Text, this.CurrentUser);
        InitPageParameter(this.TaskCode);
        ShowSuccessMessage("Common.Business.Result.Update.Successfully", this.TaskCode);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }
}