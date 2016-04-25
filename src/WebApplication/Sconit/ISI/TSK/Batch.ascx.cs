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
using com.Sconit.Entity.Exception;
using com.Sconit.Web;
using com.Sconit.Control;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TSK_Batch : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private string CurrentTaskSubTypeCode
    {
        get
        {
            return (string)ViewState["CurrentTaskSubTypeCode"];
        }
        set
        {
            ViewState["CurrentTaskSubTypeCode"] = value;
        }
    }

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

    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbTaskSubType.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code + ",bool:true";
        this.tbTaskSubType.DataBind();

        if (!IsPostBack)
        {
            this.lgd.InnerText = "${ISI.TSK.BatchAdd" + this.ModuleType + "}";
            PageCleanup();
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ProjectTask projectTask = (ProjectTask)e.Row.DataItem;
            com.Sconit.Control.DropDownList ddlPhase = (com.Sconit.Control.DropDownList)e.Row.FindControl("ddlPhase");
            ddlPhase.SelectedValue = projectTask.Phase;

            TextBox tbDesc = e.Row.FindControl("tbDesc") as TextBox;
            tbDesc.Width = new Unit(100, UnitType.Percentage);

            TextBox tbExpectedResults = e.Row.FindControl("tbExpectedResults") as TextBox;
            tbExpectedResults.Width = new Unit(100, UnitType.Percentage);
        }
    }

    public void PageCleanup()
    {
        tbTaskSubType.Text = string.Empty;
        tbTaskAddress.Text = string.Empty;

        tbUserName.Text = this.CurrentUser.Name;
        tbEmail.Text = ISIUtil.IsValidEmail(this.CurrentUser.Email) ? this.CurrentUser.Email : string.Empty;
        tbMobilePhone.Text = ISIUtil.IsValidMobilePhone(this.CurrentUser.MobliePhone) ? this.CurrentUser.MobliePhone : string.Empty;

        lblEncSubject.Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE;
        tbEncSubject.Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE;

        this.GV_List.DataSource = null;
        this.CurrentTaskSubTypeCode = string.Empty;
        this.GV_List.DataBind();
        this.fs.Visible = false;
    }

    protected void tbTaskSubType_TextChanged(Object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()) && (string.IsNullOrEmpty(this.CurrentTaskSubTypeCode) || this.CurrentTaskSubTypeCode != this.tbTaskSubType.Text))
            {
                this.CurrentTaskSubTypeCode = this.tbTaskSubType.Text.Trim();
                if (!string.IsNullOrEmpty(CurrentTaskSubTypeCode))
                {
                    var projectTaskList = this.TheProjectTaskMgr.GetProjectTask(CurrentTaskSubTypeCode, this.ModuleType);
                    if (projectTaskList != null && projectTaskList.Count > 0)
                    {
                        this.hfProjectSubType.Value = projectTaskList[0].ProjectSubType;
                        this.GV_List.DataSource = projectTaskList;
                        this.GV_List.DataBind();
                        this.fs.Visible = true;
                    }
                    else
                    {
                        this.hfProjectSubType.Value = string.Empty;
                        this.GV_List.DataSource = null;
                        this.GV_List.DataBind();
                        this.fs.Visible = false;
                        this.ShowWarningMessage("ISI.TSK.Batch.NoProjectTask");
                    }
                }
                else
                {
                    this.hfProjectSubType.Value = string.Empty;
                    this.GV_List.DataSource = null;
                    this.GV_List.DataBind();
                    this.fs.Visible = false;
                    this.ShowWarningMessage("ISI.TSK.Batch.NoProjectTask");
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (CreateEvent != null)
            {
                if (this.rfvTaskSubType.IsValid && this.rfvTaskAddress.IsValid && this.fs.Visible == true
                            && !string.IsNullOrEmpty(this.hfProjectSubType.Value))
                {
                    IList<TaskMstr> taskList = this.PopulateSelectedData(true);
                    if (taskList != null && taskList.Count > 0)
                    {
                        string taskSutType = tbTaskSubType.Text.Trim();
                        int count = TheTaskMgr.BatchTask(this.ModuleType, taskList, taskSutType, this.hfProjectSubType.Value, this.CurrentUser);
                        //CreateEvent(taskList, e);
                        this.PageCleanup();
                        ShowSuccessMessage("ISI.TSK.BatchAdd" + this.ModuleType + ".Successfully", tbTaskSubType.Text.Trim(),
                                           count.ToString());
                        return;
                    }
                }
            }
            this.ShowWarningMessage("ISI.TSK.Batch.NoImportProjectTask");
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private IList<TaskMstr> PopulateSelectedData(bool checkedAll)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            IList<TaskMstr> taskList = new List<TaskMstr>();

            string taskAddress = tbTaskAddress.Text.Trim();
            string email = tbEmail.Text.Trim();
            string userName = tbUserName.Text.Trim();
            string mobilePhone = tbMobilePhone.Text.Trim();
            bool isAutoRelease = IsAutoRelease.Checked;
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                CheckBox checkBoxGroup = row.FindControl("CheckBoxGroup") as CheckBox;
                if (checkBoxGroup.Checked || checkedAll)
                {
                    TextBox tbDesc = row.FindControl("tbDesc") as TextBox;
                    TextBox tbSeq = row.FindControl("tbSeq") as TextBox;
                    TextBox tbSubject = row.FindControl("tbSubject") as TextBox;
                    TextBox tbBackYards = row.FindControl("tbBackYards") as TextBox;
                    TextBox tbExpectedResults = row.FindControl("tbExpectedResults") as TextBox;
                    CodeMstrDropDownList ddlPhase = row.FindControl("ddlPhase") as CodeMstrDropDownList;
                    HiddenField hfId = row.FindControl("hfId") as HiddenField;

                    TextBox tbStartDate = row.FindControl("tbStartDate") as TextBox;
                    TextBox tbEndDate = row.FindControl("tbEndDate") as TextBox;

                    TaskMstr task = new TaskMstr();
                    task.ProjectTask = int.Parse(hfId.Value);
                    task.TaskAddress = taskAddress;
                    task.Desc1 = tbDesc.Text.Trim();
                    task.Desc2 = tbEncSubject.Text.Trim();
                    task.Phase = ddlPhase.SelectedValue;
                    task.Seq = tbSeq.Text.Trim();
                    task.Subject = tbSubject.Text.Trim();
                    task.ExpectedResults = tbExpectedResults.Text.Trim();
                    task.Type = this.ModuleType;
                    task.BackYards = tbBackYards.Text.Trim();
                    task.UserName = userName;
                    task.MobilePhone = mobilePhone;
                    task.Email = email;
                    task.IsAutoRelease = isAutoRelease;
                    if (tbStartDate.Text.Trim() != String.Empty)
                    {
                        task.PlanStartDate = DateTime.Parse(tbStartDate.Text.Trim() + " 08:00");
                    }
                    if (tbEndDate.Text.Trim() != String.Empty)
                    {
                        task.PlanCompleteDate = DateTime.Parse(tbEndDate.Text.Trim() + " 16:30");
                    }
                    if (task.PlanStartDate.HasValue && task.PlanCompleteDate.HasValue && task.PlanStartDate.Value.CompareTo(task.PlanCompleteDate.Value) >= 0)
                    {
                        throw new BusinessErrorException("ISI.TSK.ErrorMessage.TimeCompare", (row.RowIndex + 1).ToString());
                    }
                    taskList.Add(task);
                }
            }
            return taskList;
        }
        return null;
    }
}
