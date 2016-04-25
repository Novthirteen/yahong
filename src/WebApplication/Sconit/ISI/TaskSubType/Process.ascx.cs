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
public partial class ISI_TaskSubType_Process : com.Sconit.Web.MainModuleBase
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
    public void InitPageParameter(string taskSubTypeCode, string taskSubTypeDesc)
    {
        this.TaskSubTypeCode = taskSubTypeCode;
        this.TaskSubTypeDesc = taskSubTypeDesc;
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
        //TaskSubType taskSubType = (TaskSubType)e.ReturnValue;

        ShowSuccessMessage("ISI.TaskSubType.UpdateTaskSubType.Successfully", TaskSubTypeCode);
    }
    protected void ODS_TaskSubType_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        TaskSubType taskSubType = (TaskSubType)e.InputParameters[0];
        if (taskSubType != null)
        {
            var oldTaskSubType = this.TheTaskSubTypeMgr.LoadTaskSubType(taskSubType.Code);
            CloneHelper.CopyProperty(oldTaskSubType, taskSubType);
            taskSubType.IsWF = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsWF")).Checked;
            taskSubType.IsAssignUser = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsAssignUser")).Checked;
            taskSubType.IsCtrl = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsCtrl")).Checked;

            taskSubType.IsRemind = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsRemind")).Checked;
            taskSubType.IsPrint = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsPrint")).Checked;
            taskSubType.Template = ((com.Sconit.Control.CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlTemplate"))).SelectedValue;

            taskSubType.IsAttachment = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsAttachment")).Checked;
            taskSubType.IsTrace = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsTrace")).Checked;
            taskSubType.IsCostCenter = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsCostCenter")).Checked;
            taskSubType.ProcessNo = ((Controls_TextBox)this.FV_TaskSubType.FindControl("tbProcessNo")).Text;
            taskSubType.CostCenter = ((Controls_TextBox)this.FV_TaskSubType.FindControl("tbCostCenter")).Text;
            taskSubType.Account1 = ((Controls_TextBox)this.FV_TaskSubType.FindControl("tbAccount1")).Text;
            taskSubType.Account2 = ((Controls_TextBox)this.FV_TaskSubType.FindControl("tbAccount2")).Text;
            taskSubType.FormType = ((CodeMstrDropDownList)this.FV_TaskSubType.FindControl("ddlFormType")).SelectedValue;
            taskSubType.IsBudget = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsBudget")).Checked;
            taskSubType.IsAmount = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsAmount")).Checked;
            taskSubType.IsAmountDetail = ((CheckBox)this.FV_TaskSubType.FindControl("cbIsAmountDetail")).Checked;
            taskSubType.LastModifyDate = DateTime.Now;
            taskSubType.LastModifyUser = this.CurrentUser.Code;
        }
    }

    private void UpdateView(TaskSubType taskSubType)
    {
        CodeMstrDropDownList ddlFormType = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlFormType")));
        ddlFormType.SelectedValue = taskSubType.FormType;

        Controls_TextBox tbProcessNo = (Controls_TextBox)this.FV_TaskSubType.FindControl("tbProcessNo");
        tbProcessNo.ServiceParameter = "string:,string:" + this.CurrentUser.Code + ",string:" + taskSubType.Code;
        tbProcessNo.Text = taskSubType.ProcessNo;
        tbProcessNo.DataBind();

        Controls_TextBox tbCostCenter = (Controls_TextBox)this.FV_TaskSubType.FindControl("tbCostCenter");
        tbCostCenter.Text = taskSubType.CostCenter;
        tbCostCenter.DataBind();

        Controls_TextBox tbAccount1 = (Controls_TextBox)this.FV_TaskSubType.FindControl("tbAccount1");
        tbAccount1.ServiceParameter = "string:" + this.TaskSubTypeCode + ",string:#tbCostCenter";
        tbAccount1.Text = taskSubType.Account1;
        tbAccount1.DataBind();

        Controls_TextBox tbAccount2 = (Controls_TextBox)this.FV_TaskSubType.FindControl("tbAccount2");
        tbAccount2.ServiceParameter = "string:" + this.TaskSubTypeCode + ",string:#tbCostCenter,string:#tbAccount1";
        tbAccount2.Text = taskSubType.Account2;
        tbAccount2.DataBind();

        if (!string.IsNullOrEmpty(taskSubType.Template))
        {
            CodeMstrDropDownList ddlTemplate = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlTemplate")));
            ddlTemplate.SelectedValue = taskSubType.Template;
        }

        var processDefinitionList = this.TheProcessDefinitionMgr.GetProcessDefinition(taskSubType.Code, taskSubType.ProcessNo);
        if (string.IsNullOrEmpty(taskSubType.ProcessNo))
        {
            if (processDefinitionList.Count > 0)
            {
                ProcessDefinition processDefinition = new ProcessDefinition();
                processDefinition.IsBlankDetail = true;
                processDefinition.Seq = processDefinitionList != null && processDefinitionList.Count > 0 ? processDefinitionList.Max(p => p.Seq) + ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL : ISIConstants.CODE_MASTER_WFS_LEVEL3;

                processDefinitionList.Add(processDefinition);
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    ProcessDefinition processDefinition = new ProcessDefinition();
                    processDefinition.IsBlankDetail = true;
                    processDefinition.Seq = ISIConstants.CODE_MASTER_WFS_LEVEL3 + i * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                    processDefinitionList.Add(processDefinition);
                }
            }
            btnSave.Visible = true;
            this.GV_List_ProcessDefinition.Columns[this.GV_List_ProcessDefinition.Columns.Count - 1].Visible = true;
        }
        else
        {
            btnSave.Visible = false;
            this.GV_List_ProcessDefinition.Columns[this.GV_List_ProcessDefinition.Columns.Count - 1].Visible = false;
        }
        this.GV_List_ProcessDefinition.DataSource = processDefinitionList;
        this.GV_List_ProcessDefinition.DataBind();
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ProcessDefinition processDefinition = (ProcessDefinition)e.Row.DataItem;
            ((CodeMstrDropDownList)e.Row.FindControl("ddlLevel")).SelectedValue = processDefinition.Seq.ToString();
            //e.Row.FindControl("lbtnAdd").Visible = processDefinition.IsBlankDetail;
            e.Row.FindControl("lbtnDelete").Visible = !processDefinition.IsBlankDetail;
            if (processDefinition.IsBlankDetail)
            {
                //e.Row.Cells[4].Text = string.Empty;
                e.Row.Cells[10].Text = string.Empty;
                e.Row.Cells[12].Text = string.Empty;
            }
            else
            {
                ((TextBox)e.Row.FindControl("tbDesc1")).Text = processDefinition.Desc1;
                ((Controls_TextBox)e.Row.FindControl("tbUserCode")).Text = processDefinition.UserCode;
                ((Controls_TextBox)e.Row.FindControl("tbUOM")).Text = processDefinition.UOM;
                ((TextBox)e.Row.FindControl("tbQty")).Text = processDefinition.Qty.HasValue ? processDefinition.Qty.Value.ToString("0.########") : string.Empty;

                ((Controls_TextBox)e.Row.FindControl("tbApply")).Text = processDefinition.Apply;
                ((TextBox)e.Row.FindControl("tbApplyQty")).Text = processDefinition.ApplyQty.HasValue ? processDefinition.ApplyQty.Value.ToString("0.########") : string.Empty;
            }
        }
    }


    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        int id = int.Parse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);
        try
        {
            this.TheProcessDefinitionMgr.DeleteProcessDefinition(id);
            this.ShowSuccessMessage("ISI.TaskSubType.ProcessDefinition.DeleteProcessDefinition.Successfully");
            InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
            //this.GV_List_ProcessDefinition.DataBind();
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime now = DateTime.Now;
            IList<ProcessDefinition> processDefinitionList = new List<ProcessDefinition>();
            for (int i = 0; i < this.GV_List_ProcessDefinition.Rows.Count; i++)
            {
                GridViewRow row = this.GV_List_ProcessDefinition.Rows[i];
                var rfvDesc1 = ((RequiredFieldValidator)row.FindControl("rfvDesc1"));
                string desc1 = ((TextBox)row.FindControl("tbDesc1")).Text.Trim();
                if (rfvDesc1.IsValid && !string.IsNullOrEmpty(desc1))
                {
                    ProcessDefinition processDefinition = GetProcessDefinition(row, now, i == 0);
                    if (processDefinition != null)
                    {
                        processDefinitionList.Add(processDefinition);
                    }
                }
            }

            if (processDefinitionList.Count == 1 && string.IsNullOrEmpty(processDefinitionList[0].UserCode))
            {
                processDefinitionList.RemoveAt(0);
            }

            //更新控制选项
            processDefinitionList = processDefinitionList.OrderBy(p => p.Seq).ToList();
            IList<ProcessDefinition> newProcessDefinitionList = new List<ProcessDefinition>();
            int seq = 0;
            for (int i = 0; i < processDefinitionList.Count; i++)
            {
                var processDefinition = processDefinitionList[i];
                if (string.IsNullOrEmpty(processDefinition.UserCode))
                {
                    if (i != 0)
                    {
                        //找到小于此级别的是否配置了流程控制
                        var preLevelList = processDefinitionList.Where(p => p.Seq == processDefinitionList.Where(pd => pd.Seq < processDefinition.Seq).Max(pd => pd.Seq)).ToList();
                        if (preLevelList.Where(p => p.IsCtrl).Count() == 0)
                        {
                            foreach (var preLevel in preLevelList)
                            {
                                preLevel.IsCtrl = true;
                            }
                        }
                    }
                    //第一步为会签步，更新流程上的部门审批
                    else if (!string.IsNullOrEmpty(this.TaskSubTypeCode))
                    {
                        var taskSubType = this.TheTaskSubTypeMgr.LoadTaskSubType(this.TaskSubTypeCode);
                        if (taskSubType != null && taskSubType.IsAssignUser && !taskSubType.IsCtrl)
                        {
                            taskSubType.IsCtrl = true;
                            this.TheTaskSubTypeMgr.UpdateTaskSubType(taskSubType);
                        }
                    }
                }
                else if (i == 0)
                {
                    var taskSubType = this.TheTaskSubTypeMgr.LoadTaskSubType(this.TaskSubTypeCode);
                    if (taskSubType != null && taskSubType.IsCtrl)
                    {
                        ProcessDefinition p = new ProcessDefinition();
                        p.Desc1 = "会签";
                        p.IsApprove = true;
                        p.Seq = ISIConstants.CODE_MASTER_WFS_LEVEL3;
                        seq = ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                        p.TaskSubType = processDefinition.TaskSubType;
                        p.CreateDate = now;
                        p.CreateUser = this.CurrentUser.Code;
                        p.CreateUserNm = this.CurrentUser.Name;
                        p.LastModifyDate = now;
                        p.LastModifyUser = this.CurrentUser.Code;
                        p.LastModifyUserNm = this.CurrentUser.Name;

                        newProcessDefinitionList.Add(p);
                    }
                }
                processDefinition.Seq += seq;
                newProcessDefinitionList.Add(processDefinition);

                if (processDefinition.IsCtrl && (i == processDefinitionList.Count - 1 || !string.IsNullOrEmpty(processDefinitionList[i + 1].UserCode)))
                {
                    ProcessDefinition p = new ProcessDefinition();
                    p.Desc1 = "会签";
                    p.IsApprove = true;
                    seq += ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                    p.Seq += seq;
                    p.TaskSubType = processDefinition.TaskSubType;
                    p.CreateDate = now;
                    p.CreateUser = this.CurrentUser.Code;
                    p.CreateUserNm = this.CurrentUser.Name;
                    p.LastModifyDate = now;
                    p.LastModifyUser = this.CurrentUser.Code;
                    p.LastModifyUserNm = this.CurrentUser.Name;
                    newProcessDefinitionList.Add(p);
                }
            }

            foreach (var processDefinition in newProcessDefinitionList)
            {
                if (processDefinition.Id != 0)
                {
                    this.TheProcessDefinitionMgr.UpdateProcessDefinition(processDefinition);
                }
                else
                {
                    this.TheProcessDefinitionMgr.CreateProcessDefinition(processDefinition);
                }
            }

            this.ShowSuccessMessage("ISI.TaskSubType.ProcessDefinition.UpdateProcessDefinition.Successfully");
            InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void lbtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            int currentRow = ((GridViewRow)(((DataControlFieldCell)(((System.Web.UI.WebControls.LinkButton)(sender)).Parent)).Parent)).RowIndex;
            GridViewRow row = this.GV_List_ProcessDefinition.Rows[currentRow];

            //新增
            ProcessDefinition processDefinition = new ProcessDefinition();
            DateTime now = DateTime.Now;

            processDefinition.TaskSubType = this.TaskSubTypeCode;

            processDefinition = GetProcessDefinition(row, now, false);
            if (processDefinition != null)
            {
                this.TheProcessDefinitionMgr.CreateProcessDefinition(processDefinition);
            }
            //this.GV_List_ProcessDefinition.DataBind();               

            InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private ProcessDefinition GetProcessDefinition(GridViewRow row, DateTime now, bool includeBlank)
    {
        string desc1 = ((TextBox)row.FindControl("tbDesc1")).Text.Trim();
        if (!includeBlank && string.IsNullOrEmpty(desc1)) return null;
        int id = int.Parse(((HiddenField)row.FindControl("hfId")).Value);

        ProcessDefinition processDefinition = null;
        if (id == 0)
        {
            processDefinition = new ProcessDefinition();
            processDefinition.IsApprove = true;
            processDefinition.TaskSubType = this.TaskSubTypeCode;
            processDefinition.CreateDate = now;
            processDefinition.CreateUser = this.CurrentUser.Code;
            processDefinition.CreateUserNm = this.CurrentUser.Name;
        }
        else
        {
            processDefinition = this.TheProcessDefinitionMgr.LoadProcessDefinition(id);
        }
        processDefinition.Seq = int.Parse(((CodeMstrDropDownList)row.FindControl("ddlLevel")).SelectedValue);
        processDefinition.ATicket = ((CheckBox)row.FindControl("cbATicket")).Checked;
        processDefinition.IsCtrl = ((CheckBox)row.FindControl("cbIsCtrl")).Checked;
        processDefinition.IsAccountCtrl = ((CheckBox)row.FindControl("cbIsAccountCtrl")).Checked;
        processDefinition.IsRemind = ((CheckBox)row.FindControl("cbIsRemind")).Checked;
        processDefinition.Desc1 = desc1;
        string userCode = ((Controls_TextBox)row.FindControl("tbUserCode")).Text;
        if (!string.IsNullOrEmpty(userCode))
        {
            var user = this.TheUserMgr.CheckAndLoadUser(userCode);
            processDefinition.UserCode = user.Code;
            processDefinition.UserNm = user.Name;
        }
        else
        {
            processDefinition.UserCode = null;
            processDefinition.UserNm = null;
        }
        string uomCode = ((Controls_TextBox)row.FindControl("tbUOM")).Text;
        if (!string.IsNullOrEmpty(uomCode))
        {
            var uom = this.TheUomMgr.CheckAndLoadUom(uomCode);
            processDefinition.UOM = uom.Code;
            processDefinition.UOMDesc = uom.Description;
        }
        else
        {
            processDefinition.UOM = null;
            processDefinition.UOMDesc = null;
        }

        string applyCode = ((Controls_TextBox)row.FindControl("tbApply")).Text;
        if (!string.IsNullOrEmpty(applyCode))
        {
            var apply = this.TheApplyMgr.LoadApply(applyCode);
            if (apply == null)
            {
                var processApplyList = this.TheProcessApplyMgr.GetProcessApply(this.TaskSubTypeCode, applyCode);
                if (processApplyList != null && processApplyList.Count > 0)
                {
                    processDefinition.Apply = processApplyList[0].Apply;
                    processDefinition.ApplyDesc = processApplyList[0].GetDesc();
                }
                else
                {
                    processDefinition.Apply = null;
                    processDefinition.ApplyDesc = null;
                }
            }
            else
            {
                processDefinition.Apply = apply.Code;
                processDefinition.ApplyDesc = apply.Desc1;
            }
        }
        else
        {
            processDefinition.Apply = null;
            processDefinition.ApplyDesc = null;
        }
        string applyQty = ((TextBox)row.FindControl("tbApplyQty")).Text;
        if (!string.IsNullOrEmpty(applyQty))
        {
            processDefinition.ApplyQty = decimal.Parse(applyQty);
        }
        else
        {
            processDefinition.ApplyQty = null;
        }
        string qty = ((TextBox)row.FindControl("tbQty")).Text;
        if (!string.IsNullOrEmpty(qty))
        {
            processDefinition.Qty = decimal.Parse(qty);
        }
        else
        {
            processDefinition.Qty = null;
        }
        processDefinition.LastModifyDate = now;
        processDefinition.LastModifyUser = this.CurrentUser.Code;
        processDefinition.LastModifyUserNm = this.CurrentUser.Name;
        return processDefinition;
    }
}