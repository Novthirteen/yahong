using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;

public partial class ISI_TaskSubType_Budget : com.Sconit.Web.MainModuleBase
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

    public string TaskSubTypeCode
    {
        get
        {
            return (string)ViewState["TaskSubTypeCode"];
        }
        set
        {
            ViewState["TaskSubTypeCode"] = value;
        }
    }
    public string TaskSubTypeDesc
    {
        get
        {
            return (string)ViewState["TaskSubTypeDesc"];
        }
        set
        {
            ViewState["TaskSubTypeDesc"] = value;
        }
    }
    private void UpdateView(TaskMstr task)
    {

    }

    private void PageCleanup()
    {
        this.TaskSubTypeCode = null;
        TaskSubTypeDesc = null;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    public void InitPageParameter(string taskSubTypeCode, string taskSubTypeDesc)
    {
        this.TaskSubTypeCode = taskSubTypeCode;
        TaskSubTypeDesc = taskSubTypeDesc;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }
}