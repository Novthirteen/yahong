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
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Utility;
using System.Text;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using NHibernate.Transform;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate;

public partial class ISI_ProjectTask_Edit : EditModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler UpdateTitleEvent;


    public Int32 ProjectTaskId
    {
        get
        {
            if (ViewState["ProjectTaskId"] == null)
            {
                return 0;
            }
            else
            {
                return (Int32)ViewState["ProjectTaskId"];
            }
        }
        set
        {
            ViewState["ProjectTaskId"] = value;
        }
    }

    private void PageCleanup()
    {
        this.ProjectTaskId = 0;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter(int id)
    {
        this.ProjectTaskId = id;
        ProjectTask task = this.TheProjectTaskMgr.LoadProjectTask(ProjectTaskId);
        this.ODS_ISI.SelectParameters["id"].DefaultValue = this.ProjectTaskId.ToString();
        this.FV_ISI.DataBind();

        UpdateView(task);
    }

    protected void FV_ISI_DataBound(object sender, EventArgs e)
    {
        if (this.ProjectTaskId != 0)
        {
            ProjectTask task = (ProjectTask)((FormView)sender).DataItem;
            UpdateView(task);
        }
    }

    private void UpdateView(ProjectTask task)
    {
        ((CodeMstrDropDownList)(this.FV_ISI.FindControl("ddlPhase"))).SelectedValue = task.Phase;
        ((CodeMstrDropDownList)(this.FV_ISI.FindControl("ddlProjectSubType"))).SelectedValue = task.ProjectSubType;
        ((CodeMstrDropDownList)(this.FV_ISI.FindControl("ddlProjectType"))).SelectedValue = task.ProjectType;
    }



    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheProjectTaskMgr.DeleteProjectTask(this.ProjectTaskId, this.CurrentUser);

            if (this.BackEvent != null)
            {
                this.BackEvent(this, e);
            }

            ShowSuccessMessage("ISI.ProjectTask.Delete.Successfully");

            this.PageCleanup();
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }



    protected void btnSave_Click(object sender, EventArgs e)
    {
        ProjectTask task = new ProjectTask();
        task.Id = this.ProjectTaskId;
        task.Phase = ((CodeMstrDropDownList)(this.FV_ISI.FindControl("ddlPhase"))).SelectedValue;
        task.ProjectSubType = ((CodeMstrDropDownList)(this.FV_ISI.FindControl("ddlProjectSubType"))).SelectedValue;
        task.ProjectType = ((CodeMstrDropDownList)(this.FV_ISI.FindControl("ddlProjectType"))).SelectedValue;
        task.Subject = ((TextBox)(this.FV_ISI.FindControl("tbSubject"))).Text.Trim();
        task.Desc = ((TextBox)(this.FV_ISI.FindControl("tbDesc"))).Text.Trim();
        task.ExpectedResults = ((TextBox)(this.FV_ISI.FindControl("tbExpectedResults"))).Text.Trim();
        task.Seq = ((TextBox)(this.FV_ISI.FindControl("tbSeq"))).Text.Trim();
        task.IsActive = ((CheckBox)(this.FV_ISI.FindControl("ckIsActive"))).Checked;
        this.TheProjectTaskMgr.UpdateProjectTask(task, this.CurrentUser);
        this.FV_ISI.DataBind();
        ShowSuccessMessage("ISI.ProjectTask.Update.Successfully", task.Subject);

        if (this.UpdateTitleEvent != null)
        {
            this.UpdateTitleEvent(sender, e);
        }
    }
}