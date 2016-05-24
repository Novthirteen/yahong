using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.Control;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
public partial class ISI_TaskSubType_Form : com.Sconit.Web.MainModuleBase
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
    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        int id = int.Parse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);
        try
        {
            this.TheProcessApplyMgr.DeleteProcessApply(id);
            this.ShowSuccessMessage("ISI.TaskSubType.ProcessApply.DeleteProcessApply.Successfully");
            InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
            //this.GV_List_ProcessDefinition.DataBind();
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    public void InitPageParameter(string taskSubTypeCode, string taskSubTypeDesc)
    {
        this.TaskSubTypeCode = taskSubTypeCode;
        TaskSubTypeDesc = taskSubTypeDesc;
        this.ODS_TaskSubType.SelectParameters["Code"].DefaultValue = this.TaskSubTypeCode;
        this.FV_TaskSubType.DataBind();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }

    private void PageCleanup()
    {
        this.TaskSubTypeCode = null;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    protected void FV_TaskSubType_DataBound(object sender, EventArgs e)
    {
        if (TaskSubTypeCode != null)
        {
            TaskSubType taskSubType = (TaskSubType)((FormView)sender).DataItem;
            UpdateView(taskSubType);
        }
    }

    protected void ODS_TaskSubType_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        btnUpdate_Click(null, null);
        ShowSuccessMessage("ISI.TaskSubType.UpdateTaskSubType.Successfully", TaskSubTypeCode);
    }
    protected void ODS_TaskSubType_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        TaskSubType taskSubType = (TaskSubType)e.InputParameters[0];
        if (taskSubType != null)
        {
            var oldTaskSubType = this.TheTaskSubTypeMgr.LoadTaskSubType(taskSubType.Code);
            CloneHelper.CopyProperty(oldTaskSubType, taskSubType);
            taskSubType.IsApply = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsApply")).Checked;
            taskSubType.IsRemoveForm = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsRemoveForm")).Checked;
            taskSubType.LastModifyDate = DateTime.Now;
            taskSubType.LastModifyUser = this.CurrentUser.Code;
        }
    }

    private void UpdateView(TaskSubType taskSubType)
    {
        var processApplyList = this.TheProcessApplyMgr.GetProcessApply(taskSubType.Code);
        ProcessApply processApply = new ProcessApply();
        processApply.IsBlankDetail = true;
        if (processApplyList != null && processApplyList.Count > 0)
        {
            var maxSeqProcessApply = processApplyList[processApplyList.Count - 1];
            if (maxSeqProcessApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || maxSeqProcessApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
            {
                processApply.Seq = maxSeqProcessApply.Seq;
                processApply.Type = maxSeqProcessApply.Type;
            }
            else
            {
                processApply.Seq = maxSeqProcessApply.Seq + 10;
            }
        }
        else
        {
            processApply.Seq = 10;
        }

        if (processApplyList.Count != 0)
        {
            ProcessApply blank = new ProcessApply();
            blank.IsSeparator = true;
            processApplyList.Add(blank);
        }

        processApplyList.Add(processApply);
        processApplyList.Add(processApply);
        processApplyList.Add(processApply);
        processApplyList.Add(processApply);
        processApplyList.Add(processApply);
        this.GV_List.DataSource = processApplyList;
        this.GV_List.DataBind();
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ProcessApply processApply = (ProcessApply)e.Row.DataItem;

            if (processApply.IsSeparator)
            {
                for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                {
                    TableCell cell = e.Row.Cells[i];
                    cell.Text = string.Empty;
                }
                ((HiddenField)e.Row.FindControl("hfId")).Value = "-1";
                e.Row.FindControl("lbtnDelete").Visible = false;
                e.Row.Height = 39;
            }
            else
            {
                e.Row.FindControl("lbtnDelete").Visible = !processApply.IsBlankDetail;
                ((TextBox)e.Row.FindControl("tbSeq")).Text = processApply.Seq.ToString();
                Controls_TextBox tbApply = (Controls_TextBox)e.Row.FindControl("tbApply");
                tbApply.SuggestTextBox.Attributes.Add("onchange", "GenerateApply(this);");
                Controls_TextBox tbUOM = (Controls_TextBox)e.Row.FindControl("tbUOM");
                tbUOM.SuggestTextBox.Attributes.Add("onchange", "GenerateUOM(this);");
                if (processApply.IsBlankDetail)
                {
                    //e.Row.Cells[9].Text = string.Empty;
                    //e.Row.Cells[11].Text = string.Empty;
                }
                else
                {
                    ((Controls_TextBox)e.Row.FindControl("tbApply")).Text = processApply.Apply;
                    ((TextBox)e.Row.FindControl("tbDesc1")).Text = processApply.Desc1;
                    ((TextBox)e.Row.FindControl("tbDesc2")).Text = processApply.Desc2;
                    ((Controls_TextBox)e.Row.FindControl("tbUOM")).Text = processApply.UOM;
                    if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                    {
                        ((TextBox)e.Row.FindControl("tbGroupDesc1")).Text = processApply.UOMDesc1;
                        ((TextBox)e.Row.FindControl("tbGroupDesc2")).Text = processApply.UOMDesc2;
                    }
                    else
                    {
                        ((TextBox)e.Row.FindControl("tbUOMDesc1")).Text = processApply.UOMDesc1;
                        ((TextBox)e.Row.FindControl("tbUOMDesc2")).Text = processApply.UOMDesc2;

                        ((CheckBox)e.Row.FindControl("cbMustMatch")).Checked = processApply.MustMatch.HasValue ? processApply.MustMatch.Value : false;
                        ((TextBox)e.Row.FindControl("tbServiceMethod")).Text = processApply.ServiceMethod;
                        ((TextBox)e.Row.FindControl("tbServicePath")).Text = processApply.ServicePath;
                        ((TextBox)e.Row.FindControl("tbDescField")).Text = processApply.DescField;
                        ((TextBox)e.Row.FindControl("tbValueField")).Text = processApply.ValueField;
                    }
                    ((TextBox)e.Row.FindControl("tbFontSize")).Text = processApply.FontSize.HasValue ? processApply.FontSize.Value.ToString() : string.Empty;
                    ((TextBox)e.Row.FindControl("tbRepeatColumns")).Text = processApply.RepeatColumns.HasValue ? processApply.RepeatColumns.Value.ToString() : string.Empty;
                    ((TextBox)e.Row.FindControl("tbColor")).Text = processApply.Color;
                    if (!string.IsNullOrEmpty(processApply.Align))
                    {
                        ((CodeMstrDropDownList)e.Row.FindControl("ddlAlign")).SelectedValue = processApply.Align;
                    }
                    ((Controls_TextBox)e.Row.FindControl("tbCurrency")).Text = processApply.Currency;
                    ((CheckBox)e.Row.FindControl("cbIsUser")).Checked = processApply.IsUser.HasValue ? processApply.IsUser.Value : false;
                    ((CheckBox)e.Row.FindControl("cbIsVertical")).Checked = processApply.IsVertical.HasValue ? processApply.IsVertical.Value : false;
                    ((CheckBox)e.Row.FindControl("cbIsRow")).Checked = processApply.IsRow.HasValue ? processApply.IsRow.Value : false;
                    ((CheckBox)e.Row.FindControl("cbRequired")).Checked = processApply.Required.HasValue ? processApply.Required.Value : false;
                    ((CodeMstrDropDownList)e.Row.FindControl("ddlType")).SelectedValue = processApply.Type;
                }
            }
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime now = DateTime.Now;

            //IDictionary<string, ProcessApply> processApplyDic = new Dictionary<string, ProcessApply>();
            IList<ProcessApply> processApplyList = new List<ProcessApply>();
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                ProcessApply processApply = GetProcessApply(row, now);
                if (processApply != null)//&& !processApplyDic.Keys.Contains(processApply.Apply))
                {
                    processApplyList.Add(processApply);
                    //processApplyDic.Add(processApply.Apply, processApply);
                }
            }
            this.TheProcessApplyMgr.UpdateProcessApply(processApplyList);

            this.ShowSuccessMessage("ISI.TaskSubType.ProcessApply.UpdateProcessApply.Successfully");
            InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private ProcessApply GetProcessApply(GridViewRow row, DateTime now)
    {
        string apply = ((Controls_TextBox)row.FindControl("tbApply")).Text.Trim();
        string type = ((CodeMstrDropDownList)row.FindControl("ddlType")).SelectedValue;
        if (string.IsNullOrEmpty(apply) && type != ISIConstants.CODE_MASTER_WFS_TYPE_BLANK) return null;
        int id = int.Parse(((HiddenField)row.FindControl("hfId")).Value);
        if (id < 0) return null;
        ProcessApply processApply = null;

        if (id == 0)
        {
            processApply = new ProcessApply();
            processApply.TaskSubType = this.TaskSubTypeCode;
            processApply.CreateDate = now;
            processApply.CreateUser = this.CurrentUser.Code;
            processApply.CreateUserNm = this.CurrentUser.Name;
        }
        else
        {
            processApply = this.TheProcessApplyMgr.LoadProcessApply(id);
        }

        processApply.Type = type;
        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
        {
            processApply.UOMDesc1 = ((TextBox)row.FindControl("tbGroupDesc1")).Text;
            processApply.UOMDesc2 = ((TextBox)row.FindControl("tbGroupDesc2")).Text;
        }
        else
        {
            processApply.UOM = ((Controls_TextBox)row.FindControl("tbUOM")).Text;
            processApply.UOMDesc1 = ((TextBox)row.FindControl("tbUOMDesc1")).Text;
            processApply.UOMDesc2 = ((TextBox)row.FindControl("tbUOMDesc2")).Text;

            processApply.MustMatch = ((CheckBox)row.FindControl("cbMustMatch")).Checked;
            processApply.ServiceMethod = ((TextBox)row.FindControl("tbServiceMethod")).Text;
            processApply.ServicePath = ((TextBox)row.FindControl("tbServicePath")).Text;
            processApply.DescField = ((TextBox)row.FindControl("tbDescField")).Text;
            processApply.ValueField = ((TextBox)row.FindControl("tbValueField")).Text;
        }
        processApply.Apply = apply;
        processApply.Seq = int.Parse(((TextBox)row.FindControl("tbSeq")).Text);
        var fontSize = ((TextBox)row.FindControl("tbFontSize")).Text.Trim();
        if (fontSize != string.Empty)
        {
            processApply.FontSize = int.Parse(fontSize);
        }
        else
        {
            processApply.FontSize = null;
        }
        var repeatColumns = ((TextBox)row.FindControl("tbRepeatColumns")).Text.Trim();
        if (repeatColumns != string.Empty)
        {
            processApply.RepeatColumns = int.Parse(repeatColumns);
        }
        else
        {
            processApply.RepeatColumns = null;
        }
        processApply.Color = ((TextBox)row.FindControl("tbColor")).Text;
        processApply.Align = ((CodeMstrDropDownList)row.FindControl("ddlAlign")).SelectedValue;
        processApply.IsUser = ((CheckBox)row.FindControl("cbIsUser")).Checked;
        processApply.IsVertical = ((CheckBox)row.FindControl("cbIsVertical")).Checked;
        processApply.IsRow = ((CheckBox)row.FindControl("cbIsRow")).Checked;
        processApply.Required = ((CheckBox)row.FindControl("cbRequired")).Checked;
        processApply.Desc1 = ((TextBox)row.FindControl("tbDesc1")).Text;
        processApply.Desc2 = ((TextBox)row.FindControl("tbDesc2")).Text;
        processApply.Currency = ((Controls_TextBox)row.FindControl("tbCurrency")).Text;
        processApply.LastModifyDate = now;
        processApply.LastModifyUser = this.CurrentUser.Code;
        processApply.LastModifyUserNm = this.CurrentUser.Name;
        return processApply;
    }

}