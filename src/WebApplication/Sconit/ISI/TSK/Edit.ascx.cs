﻿using System;
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

public partial class ISI_TSK_Edit : EditModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler UpdateTitleEvent;
    public event EventHandler UpdateAttachmentTitleEvent;

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
    /// <summary>
    /// 附件扩展名
    /// </summary>
    public string FileExtensions
    {
        get
        {
            return (string)ViewState["FileExtensions"];
        }
        set
        {
            ViewState["FileExtensions"] = value;
        }
    }
    /// <summary>
    ///  附件文件大小
    /// </summary>
    public int ContentLength
    {
        get
        {
            return (int)ViewState["ContentLength"];
        }
        set
        {
            ViewState["ContentLength"] = value;
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
    public bool IsApply
    {
        get
        {
            return ViewState["IsApply"] == null ? false : (bool)ViewState["IsApply"];
        }
        set
        {
            ViewState["IsApply"] = value;
        }
    }
    public bool IsWF
    {
        get
        {
            return ViewState["IsWF"] == null ? false : (bool)ViewState["IsWF"];
        }
        set
        {
            ViewState["IsWF"] = value;
        }
    }
    public bool IsAccountCtrl
    {
        get
        {
            return ViewState["IsAccountCtrl"] == null ? false : (bool)ViewState["IsAccountCtrl"];
        }
        set
        {
            ViewState["IsAccountCtrl"] = value;
        }
    }
    public int? CurrentLevel
    {
        get
        {
            if (ViewState["CurrentLevel"] == null)
            {
                return null;
            }
            else
            {
                return int.Parse(ViewState["CurrentLevel"].ToString());
            }
        }
        set
        {
            ViewState["CurrentLevel"] = value;
        }
    }
    public string CurrentTaskSubType
    {
        get
        {
            return (string)ViewState["CurrentTaskSubType"];
        }
        set
        {
            ViewState["CurrentTaskSubType"] = value;
        }
    }
    public IList<TaskApply> TaskApplyList
    {
        get
        {
            return (IList<TaskApply>)ViewState["TaskApplyList"];
        }
        set
        {
            ViewState["TaskApplyList"] = value;
        }
    }
    private void PageCleanup()
    {
        this.TaskCode = null;
        this.CurrentTaskSubType = null;
        this.IsApply = false;
        this.IsWF = false;
        this.FileExtensions = null;
        this.ContentLength = 0;
        this.TaskApplyList = null;
        var ucCostList = (ISI_TSK_CostList)this.FV_ISI.FindControl("ucCostList");
        ucCostList.PageCleanup();
        ucCostList.Visible = false;
    }
    protected void tbCostCenter_TextChanged(Object sender, EventArgs e)
    {
        try
        {
            var ucCostList = (ISI_TSK_CostList)this.FV_ISI.FindControl("ucCostList");
            Controls_TextBox tbCostCenter = (Controls_TextBox)this.FV_ISI.FindControl("tbCostCenter");
            ucCostList.CostCenter = tbCostCenter.Text;
            ucCostList.InitPageParameter(ucCostList.TheCostList);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void tbTaskSubType_TextChanged(Object sender, EventArgs e)
    {
        try
        {
            Controls_TextBox tbTaskSubType = ((Controls_TextBox)this.FV_ISI.FindControl("tbTaskSubType"));
            if (!string.IsNullOrEmpty(tbTaskSubType.Text.Trim()) && (string.IsNullOrEmpty(this.CurrentTaskSubType) || this.CurrentTaskSubType != tbTaskSubType.Text))
            {
                this.CurrentTaskSubType = tbTaskSubType.Text.Trim();
                var taskSubType = TheTaskSubTypeMgr.LoadTaskSubType(CurrentTaskSubType);
                if (taskSubType != null)
                {
                    if (!string.IsNullOrEmpty(taskSubType.CostCenter))
                    {
                        ((Controls_TextBox)this.FV_ISI.FindControl("tbCostCenter")).Text = taskSubType.CostCenter;
                    }
                    if (!string.IsNullOrEmpty(taskSubType.Account1))
                    {
                        ((Controls_TextBox)this.FV_ISI.FindControl("tbAccount1")).Text = taskSubType.Account1;
                    }
                    if (!string.IsNullOrEmpty(taskSubType.Account2))
                    {
                        ((Controls_TextBox)this.FV_ISI.FindControl("tbAccount2")).Text = taskSubType.Account2;
                    }
                    if (this.IsApply && !string.IsNullOrEmpty(CurrentTaskSubType))
                    {
                        UpdateTaskApply(this.IsApply, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE, null, CurrentTaskSubType, true);
                    }
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private void UpdateTaskApply(bool isApply, string status, int? level, string taskSubTypeCode, bool enable)
    {
        if (!isApply) return;

        this.TaskApplyList = this.TheTaskApplyMgr.GetTaskApply(this.TaskCode);

        GenerateGynamic(enable && !(level.HasValue && level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE || status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE && status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN));
    }

    protected void GenerateGynamic(bool enabled)
    {
        if (IsApply && TaskApplyList != null && TaskApplyList.Count > 0)
        {
            HtmlTable taskTable = (HtmlTable)this.FV_ISI.FindControl("taskTable");
            HtmlTableRow tr = null;
            int j = 0;
            this.FV_ISI.FindControl("workHoursTR2").Visible = TaskApplyList.Where(p => p.IsUser.HasValue && p.IsUser.Value).Count() > 0;
            for (int i = 0; i < TaskApplyList.Count; i++)
            {
                var taskApply = TaskApplyList[i];
                HtmlTableCell tc1 = new HtmlTableCell();
                HtmlTableCell tc2 = new HtmlTableCell();
                tc1.ColSpan = -1;
                tc2.ColSpan = -1;
                tc1.Attributes.Remove("class");
                tc2.Attributes.Remove("class");
                tc1.Attributes.Remove("style");
                tc2.Attributes.Remove("style");

                tc1.Attributes.Add("class", "td01");
                tc2.Attributes.Add("class", "td02");

                string id = taskApply.Apply + taskApply.UOM + taskApply.Seq + taskApply.Id;
                /*
                HiddenField hfid = new HiddenField();
                hfid.ID = "hf" + id;
                tc1.Controls.Remove(hfid);
                */
                if (j % 2 == 0
                        || taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK || TaskApplyList[i - 1].Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK
                        || taskApply.IsRow.HasValue && taskApply.IsRow.Value || TaskApplyList[i - 1].IsRow.HasValue && TaskApplyList[i - 1].IsRow.Value)
                {
                    tr = new HtmlTableRow();
                    j = 0;
                    int count = taskTable.Rows.Count;
                    taskTable.Rows.Insert(count, tr);
                }
                tc1.Attributes.Remove("style");
                tc2.Attributes.Remove("style");
                tr.Controls.Add(tc1);
                tr.Controls.Add(tc2);

                if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK)
                {
                    tc1.ColSpan = 4;
                    tr.Controls.Remove(tc2);
                    j = 0;
                    continue;
                }

                StringBuilder style = new StringBuilder();
                if (taskApply.FontSize.HasValue && taskApply.FontSize.Value > 0)
                {
                    style.Append("font-size: " + taskApply.FontSize.Value + "px;");
                }
                if (!string.IsNullOrEmpty(taskApply.Align))
                {
                    style.Append("text-align: " + taskApply.Align + ";");
                }
                if (style.Length > 0)
                {
                    tc1.Attributes.Add("style", style.ToString());
                }

                if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_LABEL)
                {
                    Label lbl = new Label();
                    lbl.ID = "lbl" + id;
                    lbl.Text = taskApply.GetDesc(this.CurrentUser.UserLanguage);
                    if (!string.IsNullOrEmpty(taskApply.Color))
                    {
                        lbl.Attributes.Add("style", "color: " + taskApply.Color + ";");
                    }
                    tc1.Controls.Add(lbl);
                    tr.Controls.Remove(tc2);
                    if (taskApply.IsRow.HasValue && taskApply.IsRow.Value)
                    {
                        tc1.ColSpan = 4;
                        j = 0;
                    }
                    else
                    {
                        tc1.ColSpan = 2;
                    }
                }
                else
                {
                    if (taskApply.IsRow.HasValue && taskApply.IsRow.Value)
                    {
                        tc2.ColSpan = 3;
                        j = 0;
                    }
                    else
                    {
                        tc2.ColSpan = -1;
                        j++;
                    }

                    if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                    {
                        var applyList = TaskApplyList.Where(p => p.Type == taskApply.Type && p.Seq == taskApply.Seq).OrderBy(p => p.Seq).ToList();

                        var desc = taskApply.UOMDesc(this.CurrentUser.UserLanguage);
                        if (!string.IsNullOrEmpty(desc))
                        {
                            Label lbl = new Label();
                            if (!string.IsNullOrEmpty(taskApply.Color))
                            {
                                lbl.Attributes.Add("style", "color: " + taskApply.Color + ";");
                            }
                            lbl.Text = desc + ":";
                            tc1.Controls.Add(lbl);
                        }

                        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX)
                        {
                            CheckBoxList cbListProcessApply = new CheckBoxList();
                            cbListProcessApply.ID = "cb" + id;
                            if (this.CurrentUser.UserLanguage == BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN)
                            {
                                cbListProcessApply.DataTextField = "Desc1";
                            }
                            else
                            {
                                cbListProcessApply.DataTextField = "Desc2";
                            }
                            cbListProcessApply.DataValueField = "Apply";
                            if (taskApply.IsVertical.HasValue && taskApply.IsVertical.Value)
                            {
                                cbListProcessApply.RepeatDirection = RepeatDirection.Vertical;
                            }
                            else
                            {
                                cbListProcessApply.RepeatDirection = RepeatDirection.Horizontal;
                            }
                            if (taskApply.RepeatColumns.HasValue && taskApply.RepeatColumns.Value > 0)
                            {
                                cbListProcessApply.RepeatColumns = taskApply.RepeatColumns.Value;
                            }
                            else
                            {
                                cbListProcessApply.RepeatLayout = RepeatLayout.Flow;
                            }
                            cbListProcessApply.DataSource = applyList;
                            cbListProcessApply.DataBind();
                            for (int ii = 0; ii < cbListProcessApply.Items.Count; ii++)
                            {
                                if (cbListProcessApply.Items[ii].Value == applyList[ii].Apply)
                                {
                                    cbListProcessApply.Items[ii].Selected = applyList[ii].Checked.HasValue ? applyList[ii].Checked.Value : false;
                                }
                            }
                            cbListProcessApply.Enabled = enabled;
                            tc2.Controls.Add(cbListProcessApply);
                        }
                        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                        {
                            RadioButtonList rbListProcessApply = new RadioButtonList();
                            rbListProcessApply.ID = "rb" + id;
                            if (this.CurrentUser.UserLanguage == BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN)
                            {
                                rbListProcessApply.DataTextField = "Desc1";
                            }
                            else
                            {
                                rbListProcessApply.DataTextField = "Desc2";
                            }
                            rbListProcessApply.DataValueField = "Apply";

                            if (taskApply.IsVertical.HasValue && taskApply.IsVertical.Value)
                            {
                                rbListProcessApply.RepeatDirection = RepeatDirection.Vertical;
                            }
                            else
                            {
                                rbListProcessApply.RepeatDirection = RepeatDirection.Horizontal;
                            }

                            if (taskApply.RepeatColumns.HasValue && taskApply.RepeatColumns.Value > 0)
                            {
                                rbListProcessApply.RepeatColumns = taskApply.RepeatColumns.Value;
                            }
                            else
                            {
                                rbListProcessApply.RepeatLayout = RepeatLayout.Flow;
                            }
                            rbListProcessApply.DataSource = applyList;
                            rbListProcessApply.DataBind();
                            for (int ii = 0; ii < rbListProcessApply.Items.Count; ii++)
                            {
                                if (rbListProcessApply.Items[ii].Value == applyList[ii].Apply)
                                {
                                    rbListProcessApply.Items[ii].Selected = applyList[ii].Checked.HasValue ? applyList[ii].Checked.Value : false;
                                }
                            }
                            rbListProcessApply.Enabled = enabled;
                            tc2.Controls.Add(rbListProcessApply);
                        }
                        i += applyList.Count - 1;
                    }
                    else
                    {
                        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_TEXTBOX && !string.IsNullOrEmpty(taskApply.ServicePath)
                                && !string.IsNullOrEmpty(taskApply.ServiceMethod) && !string.IsNullOrEmpty(taskApply.DescField) && !string.IsNullOrEmpty(taskApply.ValueField))
                        {
                            Controls_TextBox tb = new Controls_TextBox();
                            NewTextBox(enabled, tc2, taskApply, id, tb);
                        }
                        else
                        {
                            TextBox tb = new TextBox();
                            NewTextBox(enabled, tc2, taskApply, id, tb);
                        }

                        Validator(tc1, tc2, taskApply, id);
                    }
                }
            }
        }
    }

    private void NewTextBox(bool enabled, HtmlTableCell tc2, TaskApply taskApply, string id, TextBox tb)
    {
        tb.ID = "tb" + id;
        tb.ReadOnly = !enabled;
        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_TEXTAREA)
        {
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Height = 50;
            tb.Width = new Unit(77, UnitType.Percentage);
        }

        if (taskApply.Required.HasValue && taskApply.Required.Value)
        {
            tb.CssClass = "inputRequired";
        }

        tb.Text = taskApply.GetValue(enabled, this.CurrentUser.UserLanguage);

        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATE)
        {
            if (enabled)
            {
                tb.Attributes.Add("onclick", "WdatePicker({dateFmt:'yyyy-MM-dd'})");
            }
            else
            {
                tb.Attributes.Remove("onclick");
            }
        }
        else if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATETIME)
        {
            if (enabled)
            {
                tb.Attributes.Add("onclick", "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})");
            }
            else
            {
                tb.Attributes.Remove("onclick");
            }
        }

        tc2.Controls.Add(tb);
    }

    private void NewTextBox(bool enabled, HtmlTableCell tc2, TaskApply taskApply, string id, Controls_TextBox tb)
    {
        tb.ReadOnly = !enabled;
        tb.Text = taskApply.Value;
        tb.ID = "tb" + id;
        tb.ServicePath = taskApply.ServicePath;
        tb.ServiceMethod = taskApply.ServiceMethod;
        tb.MustMatch = taskApply.MustMatch.HasValue ? taskApply.MustMatch.Value : false;
        tb.DescField = taskApply.DescField;
        tb.ValueField = taskApply.ValueField;
        tb.DataBind();

        if (taskApply.Required.HasValue && taskApply.Required.Value)
        {
            tb.CssClass = "inputRequired";
        }
        tc2.Controls.Add(tb);
    }


    private void Validator(HtmlTableCell tc1, HtmlTableCell tc2, TaskApply taskApply, string id)
    {
        Label lbl = new Label();
        if (!string.IsNullOrEmpty(taskApply.Color))
        {
            lbl.Attributes.Add("style", "color: " + taskApply.Color + ";");
        }
        lbl.ID = "lbl" + id;
        lbl.Text = taskApply.GetDesc(this.CurrentUser.UserLanguage) + ":";
        tc1.Controls.Add(lbl);

        if (taskApply.Required.HasValue && taskApply.Required.Value)
        {
            RequiredFieldValidator rfv = new RequiredFieldValidator();
            rfv.ID = "rfv" + id;
            rfv.Display = ValidatorDisplay.Dynamic;
            rfv.ErrorMessage = "${Common.Business.Error.Required}";
            rfv.ControlToValidate = "tb" + id;
            rfv.ValidationGroup = "vgSave";

            tc2.Controls.Add(rfv);
        }

        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_QTY)
        {
            RegularExpressionValidator rev = new RegularExpressionValidator();
            rev.ID = "rev" + id;

            rev.Display = ValidatorDisplay.Dynamic;
            rev.ErrorMessage = "${Common.Validator.Valid.Number}";
            rev.ValidationExpression = "^[0-9]+(.[0-9]{1,8})?$";
            rev.ControlToValidate = "tb" + id;
            rev.ValidationGroup = "vgSave";
            tc2.Controls.Add(rev);
        }

        if (!string.IsNullOrEmpty(taskApply.UOM))
        {
            Literal lblDesc = new Literal();
            lblDesc.Text = taskApply.UOMDesc(this.CurrentUser.UserLanguage);
            tc2.Controls.Add(lblDesc);
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        var ucCostList = ((ISI_TSK_CostList)this.FV_ISI.FindControl("ucCostList"));
        if (ucCostList != null)
        {
            ucCostList.EditEvent += new System.EventHandler(this.Edit_Render);
        }

        if (!IsPostBack)
        {
            FileExtensions = this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_FILEEXTENSION).Value;
            ContentLength = int.Parse(this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_CONTENTLENGTH).Value);
        }
        else
        {
            GenerateGynamic(false);
        }
    }

    public void InitPageParameter(string taskCode)
    {
        this.TaskCode = taskCode;
        TaskMstr task = TheTaskMstrMgr.LoadTaskMstr(taskCode);
        this.IsApply = task.IsApply;
        this.CurrentLevel = task.Level;
        this.IsWF = task.IsWF;
        this.ModuleType = task.Type;
        this.ODS_ISI.SelectParameters["code"].DefaultValue = this.TaskCode;
        this.FV_ISI.DataBind();
    }

    protected void FV_ISI_DataBound(object sender, EventArgs e)
    {
        if (TaskCode != null && TaskCode != string.Empty)
        {
            TaskMstr task = (TaskMstr)((FormView)sender).DataItem;
            UpdateView(task);
        }
    }
    private void UpdateAccount(bool isCostCenter)
    {
        UpdateAccount(null, isCostCenter, false);
    }
    private void UpdateAccount(TaskMstr task, bool isCostCenter, bool enable)
    {
        this.FV_ISI.FindControl("trAccount").Visible = isCostCenter;
        if (isCostCenter)
        {
            var tbAccount1 = (Controls_TextBox)this.FV_ISI.FindControl("tbAccount1");
            var tbAccount2 = (Controls_TextBox)this.FV_ISI.FindControl("tbAccount2");
            var rfvAccount1 = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvAccount1");
            var rfvAccount2 = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvAccount2");
            var rtbAccount1 = this.FV_ISI.FindControl("rtbAccount1");
            var rtbAccount2 = this.FV_ISI.FindControl("rtbAccount2");
            if (enable)
            {
                tbAccount1.Visible = true;
                tbAccount2.Visible = true;
                tbAccount1.CssClass = "inputRequired";
                tbAccount2.CssClass = "inputRequired";
                rfvAccount1.Enabled = true;
                rfvAccount2.Enabled = true;
                rtbAccount1.Visible = false;
                rtbAccount2.Visible = false;

                if (!string.IsNullOrEmpty(task.Account1))
                {
                    tbAccount1.Text = task.Account1;
                }

                if (!string.IsNullOrEmpty(task.Account2))
                {
                    tbAccount2.Text = task.Account2;
                }
            }
            else
            {
                tbAccount1.Visible = false;
                tbAccount2.Visible = false;
                tbAccount1.CssClass = string.Empty;
                tbAccount2.CssClass = string.Empty;
                rfvAccount1.Enabled = false;
                rfvAccount2.Enabled = false;
                rtbAccount1.Visible = true;
                rtbAccount2.Visible = true;
            }
        }
    }

    private void UpdateView(TaskMstr task)
    {
        TaskSubType taskSubType = task.TaskSubType;
        this.IsAccountCtrl = false;
        string isiStatus = task.Status;
        bool isISIAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN);
        bool isWFAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN);
        bool isTaskFlowAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN);
        bool isCloser = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE);
        bool isAssigner = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN);

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT
                    || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            ((Literal)this.FV_ISI.FindControl("ltlTaskSubType")).Text = "${ISI.TSK.TaskSubType." + this.ModuleType + "}:";
        }

        UpdateTaskApply(task.IsApply, task.Status, task.Level, task.TaskSubType.Code, isWFAdmin || isISIAdmin || task.CreateUser == this.CurrentUser.Code || task.SubmitUser == this.CurrentUser.Code);
        var ucCostList = (ISI_TSK_CostList)this.FV_ISI.FindControl("ucCostList");
        ucCostList.PageAction = BusinessConstants.PAGE_LIST_ACTION;
        //成本中心
        this.FV_ISI.FindControl("tbCostCenter").Visible = taskSubType.IsCostCenter && isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE;
        this.FV_ISI.FindControl("rfvCostCenter").Visible = taskSubType.IsCostCenter && isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE;
        this.FV_ISI.FindControl("rtbCostCenter").Visible = !(taskSubType.IsCostCenter && isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE);

        #region 创建、退回 状态
        if (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
        {
            this.FV_ISI.FindControl("btnReject").Visible = false;
            this.FV_ISI.FindControl("btnAssign").Visible = false;
            this.FV_ISI.FindControl("btnStart").Visible = false;
            this.FV_ISI.FindControl("btnComplete").Visible = false;
            this.FV_ISI.FindControl("btnCancel").Visible = false;
            this.FV_ISI.FindControl("btnClose").Visible = false;
            this.FV_ISI.FindControl("btnSave").Visible = false;
            this.FV_ISI.FindControl("btnSubmit").Visible = false;
            this.FV_ISI.FindControl("btnDelete").Visible = false;
            this.FV_ISI.FindControl("btnApprove").Visible = false;
            this.FV_ISI.FindControl("btnDispute").Visible = false;
            this.FV_ISI.FindControl("btnRefuse").Visible = false;
            this.FV_ISI.FindControl("btnReturn").Visible = false;
            this.FV_ISI.FindControl("btnPrint").Visible = false;

            //禁用所有组件
            SetEnableBase(false);
            SetEnableFlow(false);
            ((TextBox)this.FV_ISI.FindControl("tbPlanStartDate")).CssClass = string.Empty;
            ((TextBox)this.FV_ISI.FindControl("tbPlanCompleteDate")).CssClass = string.Empty;

            Page.ClientScript.RegisterStartupScript(GetType(), @"method", " <script language='javascript' type='text/javascript'>HideComment();</script>");

            //超级管理员与创建人
            if (this.TheTaskMgr.HasPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, isCloser, this.CurrentUser.Code))
            {
                ucCostList.PageAction = BusinessConstants.PAGE_NEW_ACTION;
                this.FV_ISI.FindControl("btnSave").Visible = true;
                this.FV_ISI.FindControl("btnSubmit").Visible = true;
                this.FV_ISI.FindControl("btnDelete").Visible = isiStatus != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN;
                this.FV_ISI.FindControl("btnCancel").Visible = isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN;
                SetEnableBase(true);
                SetEnableFlow(true);
                //UpdateAccount(task, taskSubType.IsCostCenter && ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION);
                UpdateAccount(task, taskSubType.IsCostCenter, true);
            }
            else
            {
                UpdateAccount(taskSubType.IsCostCenter);
            }
        }
        #endregion

        bool isAssign = false;
        #region  提交、批准 状态
        if (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
        {
            this.FV_ISI.FindControl("btnReturn").Visible = false;
            this.FV_ISI.FindControl("btnRefuse").Visible = false;
            this.FV_ISI.FindControl("btnSubmit").Visible = false;
            this.FV_ISI.FindControl("btnReject").Visible = false;
            this.FV_ISI.FindControl("btnDelete").Visible = false;
            this.FV_ISI.FindControl("btnClose").Visible = false;
            this.FV_ISI.FindControl("btnComplete").Visible = false;
            this.FV_ISI.FindControl("btnStart").Visible = false;
            this.FV_ISI.FindControl("btnSave").Visible = false;
            this.FV_ISI.FindControl("btnAssign").Visible = false;
            this.FV_ISI.FindControl("btnCancel").Visible = false;
            this.FV_ISI.FindControl("btnDispute").Visible = false;
            this.FV_ISI.FindControl("fsApprove").Visible = false;
            this.FV_ISI.FindControl("btnPrint").Visible = isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE && taskSubType.IsPrint;

            //禁用所有组件
            SetEnableBase(false);
            SetEnableFlow(false);
            if (task.IsWF && task.Level != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
            {
                //审批人
                string costCenter = task.TaskSubType.Code;
                if (!string.IsNullOrEmpty(task.CostCenterCode))
                {
                    costCenter = task.CostCenterCode;
                }
                string assignUser = this.TheHqlMgr.FindAll<string>("select tst.AssignUser from TaskSubType tst where tst.Code='" + costCenter + "'").FirstOrDefault();
                isAssign = taskSubType.IsAssignUser && (isWFAdmin || ISIUtil.Contains(assignUser, this.CurrentUser.Code));
            }
            else //if (!task.IsWF || task.IsWF && task.IsTrace)
            {
                isAssign = this.TheTaskMgr.HasAssignPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code);
            }
            WFPermission wfPermission = this.TheTaskMgr.ProcessPermission(isiStatus, task.Code, task.Level, isWFAdmin, this.CurrentUser.Code);

            if (task.IsWF && task.Level != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && wfPermission.IsApprove)
            {
                this.FV_ISI.FindControl("ctrlTR").Visible = wfPermission.IsCtrl;
                ((CheckBox)this.FV_ISI.FindControl("cbIsCountersignSerial")).Enabled = wfPermission.IsCtrl;
                this.FV_ISI.FindControl("tbCountersignUser").Visible = wfPermission.IsCtrl;
                this.FV_ISI.FindControl("rtbCountersignUser").Visible = !wfPermission.IsCtrl;
                this.FV_ISI.FindControl("btnApprove").Visible = true;
                this.FV_ISI.FindControl("btnRefuse").Visible = task.Level == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE;
                this.FV_ISI.FindControl("btnReturn").Visible = true;
                this.FV_ISI.FindControl("fsApprove").Visible = true;
                this.FV_ISI.FindControl("btnSave").Visible = true;
                //ucCostList.PageAction = BusinessConstants.PAGE_EDIT_ACTION;
                SetEnableFlow(true);
            }
            else if (isAssign && task.IsTrace)
            {
                this.FV_ISI.FindControl("btnAssign").Visible = true;
                this.FV_ISI.FindControl("btnSave").Visible = true;
                SetEnableFlow(true);
            }
            else
            {
                ((TextBox)this.FV_ISI.FindControl("tbPlanStartDate")).CssClass = string.Empty;
                ((TextBox)this.FV_ISI.FindControl("tbPlanCompleteDate")).CssClass = string.Empty;
            }

            if ((isISIAdmin
                     || isTaskFlowAdmin
                     || isWFAdmin
                     || task.CreateUser == this.CurrentUser.Code
                                    || task.SubmitUser == this.CurrentUser.Code) && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE)
            {
                ucCostList.PageAction = BusinessConstants.PAGE_EDIT_ACTION;
                SetEnableBase(true);
                SetEnableFlow(true);
                this.FV_ISI.FindControl("btnSave").Visible = true;
                this.FV_ISI.FindControl("tbWorkHoursUser").Visible = isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT;
                this.FV_ISI.FindControl("rtbWorkHoursUser").Visible = isiStatus != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT;
                UpdateAccount(task, taskSubType.IsCostCenter, ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION);
            }
            else
            {
                if (wfPermission.IsAccountCtrl)
                {
                    ucCostList.PageAction = BusinessConstants.PAGE_EDIT_ACTION;
                }
                UpdateAccount(task, taskSubType.IsCostCenter, wfPermission.IsAccountCtrl && ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION);
                this.FV_ISI.FindControl("tbWorkHoursUser").Visible = false;
                this.FV_ISI.FindControl("rtbWorkHoursUser").Visible = true;
                /*
                 * ((TextBox)this.FV_ISI.FindControl("tbPlanAmount")).ReadOnly = true;
                ((TextBox)this.FV_ISI.FindControl("tbAmount")).ReadOnly = true;
                ((TextBox)this.FV_ISI.FindControl("tbPlanWorkHours")).ReadOnly = true;
                ((TextBox)this.FV_ISI.FindControl("tbWorkHours")).ReadOnly = true;
                 * */
            }

            if (task.Level == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
            {
                ((TextBox)this.FV_ISI.FindControl("tbPlanStartDate")).Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }

            if (!isISIAdmin && !isTaskFlowAdmin)
            {
                this.FV_ISI.FindControl("tbTaskSubType").Visible = false;
                this.FV_ISI.FindControl("rtbTaskSubType").Visible = true;
            }
        }
        #endregion

        #region  审批中状态
        if (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)
        // || (!isAssign && isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE))
        {
            //禁用所有组件
            SetEnableBase(false);
            SetEnableFlow(false);
            this.FV_ISI.FindControl("btnSave").Visible = false;
            //审批人
            WFPermission wfPermission = this.TheTaskMgr.ProcessPermission(isiStatus, task.Code, task.Level, isWFAdmin, this.CurrentUser.Code);
            //上一步审批人
            WFPermission wfPrePermission = this.TheTaskMgr.PreProcessPermission(isiStatus, task.Code, task.PreLevel, task.Level, isWFAdmin, this.CurrentUser.Code);

            //控制科目
            this.IsAccountCtrl = wfPermission.IsAccountCtrl;
            if (wfPermission.IsAccountCtrl)
            {
                ucCostList.PageAction = BusinessConstants.PAGE_EDIT_ACTION;
            }
            UpdateAccount(task, taskSubType.IsCostCenter, wfPermission.IsAccountCtrl && ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION);

            if (wfPermission.IsApprove || wfPrePermission.IsApprove)
            {
                this.FV_ISI.FindControl("btnApprove").Visible = wfPermission.IsApprove || (wfPrePermission.IsApprove && (wfPrePermission.IsCtrl || isiStatus != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE));
                this.FV_ISI.FindControl("btnDispute").Visible = wfPermission.IsApprove && task.Level != ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE || (!wfPermission.IsApprove && wfPrePermission.IsApprove && (wfPrePermission.IsCtrl || isiStatus != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE));
                this.FV_ISI.FindControl("btnRefuse").Visible = wfPermission.IsApprove && task.Level == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE;
                this.FV_ISI.FindControl("btnReturn").Visible = true;
                this.FV_ISI.FindControl("fsApprove").Visible = true;
                this.FV_ISI.FindControl("btnSave").Visible = taskSubType.IsCostCenter;
                if (wfPermission.IsCtrl || wfPrePermission.IsCtrl)
                {
                    this.FV_ISI.FindControl("ctrlTR").Visible = wfPermission.IsCtrl || wfPrePermission.IsCtrl;
                    ((CheckBox)this.FV_ISI.FindControl("cbIsCountersignSerial")).Enabled = wfPermission.IsCtrl || wfPrePermission.IsCtrl;
                    this.FV_ISI.FindControl("tbCountersignUser").Visible = wfPermission.IsCtrl || wfPrePermission.IsCtrl;
                    this.FV_ISI.FindControl("rtbCountersignUser").Visible = !(wfPermission.IsCtrl || wfPrePermission.IsCtrl);
                }
            }
            else
            {
                this.FV_ISI.FindControl("fsApprove").Visible = false;
                this.FV_ISI.FindControl("btnApprove").Visible = false;
                this.FV_ISI.FindControl("btnDispute").Visible = false;
                this.FV_ISI.FindControl("btnRefuse").Visible = false;
                this.FV_ISI.FindControl("btnReturn").Visible = false;
                this.FV_ISI.FindControl("tbCountersignUser").Visible = false;
                //this.FV_ISI.FindControl("rtbCountersignUser").Visible = true;
            }
            this.FV_ISI.FindControl("btnCancel").Visible = false;
            this.FV_ISI.FindControl("btnReject").Visible = false;
            this.FV_ISI.FindControl("btnAssign").Visible = false;
            this.FV_ISI.FindControl("btnSubmit").Visible = false;
            this.FV_ISI.FindControl("btnDelete").Visible = false;
            this.FV_ISI.FindControl("btnClose").Visible = false;
            this.FV_ISI.FindControl("btnComplete").Visible = false;
            this.FV_ISI.FindControl("btnStart").Visible = false;
            //this.FV_ISI.FindControl("btnPrint").Visible = taskSubType.IsPrint;
            this.FV_ISI.FindControl("btnPrint").Visible = false;
            this.FV_ISI.FindControl("tbWorkHoursUser").Visible = false;
            this.FV_ISI.FindControl("rtbWorkHoursUser").Visible = true;
        }
        #endregion

        #region 分派状态
        if (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
        {
            this.FV_ISI.FindControl("btnReturn").Visible = false;
            this.FV_ISI.FindControl("btnApprove").Visible = false;
            this.FV_ISI.FindControl("btnDispute").Visible = false;
            this.FV_ISI.FindControl("btnRefuse").Visible = false;
            this.FV_ISI.FindControl("btnReject").Visible = false;
            this.FV_ISI.FindControl("btnSubmit").Visible = false;
            this.FV_ISI.FindControl("btnDelete").Visible = false;
            this.FV_ISI.FindControl("btnClose").Visible = false;
            this.FV_ISI.FindControl("btnComplete").Visible = false;
            this.FV_ISI.FindControl("btnSave").Visible = false;
            this.FV_ISI.FindControl("btnAssign").Visible = false;
            this.FV_ISI.FindControl("btnPrint").Visible = taskSubType.IsPrint;
            SetEnableBase(false);
            SetEnableFlow(false);
            UpdateAccount(taskSubType.IsCostCenter);
            if (this.TheTaskMgr.HasPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, isCloser, this.CurrentUser.Code))
            {
                this.FV_ISI.FindControl("btnStart").Visible = true;
            }
            else
            {
                this.FV_ISI.FindControl("btnStart").Visible = false;
            }

            if (this.TheTaskMgr.HasAssignPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code))
            {
                this.FV_ISI.FindControl("btnAssign").Visible = true;
                this.FV_ISI.FindControl("btnSave").Visible = true;
                SetEnableFlow(true);
            }

            if (isISIAdmin || isTaskFlowAdmin
                    || task.CreateUser == this.CurrentUser.Code
                    || task.SubmitUser == this.CurrentUser.Code
                    || (task.Type == ISIConstants.ISI_TASK_TYPE_IMPROVE && task.CreateUser != this.CurrentUser.Code && (ISIUtil.Contains(taskSubType.AssignUser, this.CurrentUser.Code) || ISIUtil.Contains(taskSubType.AssignUpUser, this.CurrentUser.Code))))
            {
                this.FV_ISI.FindControl("btnSave").Visible = true;
                this.FV_ISI.FindControl("btnCancel").Visible = true;
                SetEnableBase(true);
            }
            else
            {
                this.FV_ISI.FindControl("btnCancel").Visible = false;
            }

            if (!isISIAdmin && !isTaskFlowAdmin)
            {
                ((Controls_TextBox)this.FV_ISI.FindControl("tbTaskSubType")).Visible = false;
                ((ReadonlyTextBox)this.FV_ISI.FindControl("rtbTaskSubType")).Visible = true;
            }
            this.FV_ISI.FindControl("tbWorkHoursUser").Visible = false;
            this.FV_ISI.FindControl("rtbWorkHoursUser").Visible = true;
        }
        #endregion

        #region 执行中状态
        if (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
        {
            this.FV_ISI.FindControl("btnReject").Visible = false;
            this.FV_ISI.FindControl("btnReturn").Visible = false;
            this.FV_ISI.FindControl("btnApprove").Visible = false;
            this.FV_ISI.FindControl("btnDispute").Visible = false;
            this.FV_ISI.FindControl("btnRefuse").Visible = false;
            this.FV_ISI.FindControl("btnSubmit").Visible = false;
            this.FV_ISI.FindControl("btnDelete").Visible = false;
            this.FV_ISI.FindControl("btnStart").Visible = false;
            this.FV_ISI.FindControl("btnCancel").Visible = false;
            this.FV_ISI.FindControl("btnClose").Visible = false;
            this.FV_ISI.FindControl("btnSave").Visible = false;
            this.FV_ISI.FindControl("btnAssign").Visible = false;
            this.FV_ISI.FindControl("btnPrint").Visible = taskSubType.IsPrint;
            SetEnableBase(false);
            SetEnableFlow(false);
            UpdateAccount(taskSubType.IsCostCenter);
            if (this.TheTaskMgr.HasPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, isCloser, this.CurrentUser.Code))
            {
                this.FV_ISI.FindControl("btnComplete").Visible = true;
            }
            else
            {
                this.FV_ISI.FindControl("btnComplete").Visible = false;
            }

            if (this.TheTaskMgr.HasAssignPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code))
            {
                this.FV_ISI.FindControl("btnAssign").Visible = true;
                this.FV_ISI.FindControl("btnSave").Visible = true;
                SetEnableFlow(true);
            }

            if (isISIAdmin || isTaskFlowAdmin
                    || task.CreateUser == this.CurrentUser.Code
                    || task.SubmitUser == this.CurrentUser.Code)
            {
                if (isISIAdmin || isTaskFlowAdmin)
                {
                    this.FV_ISI.FindControl("btnSave").Visible = true;
                }
                //this.FV_ISI.FindControl("btnCancel").Visible = true;
                if (!task.IsWF)
                {
                    SetEnableBase(true);
                }
            }

            if (!isISIAdmin
                     && !isTaskFlowAdmin)
            {
                ((Controls_TextBox)this.FV_ISI.FindControl("tbTaskSubType")).Visible = false;
                ((ReadonlyTextBox)this.FV_ISI.FindControl("rtbTaskSubType")).Visible = true;
            }

            this.FV_ISI.FindControl("tbWorkHoursUser").Visible = false;
            this.FV_ISI.FindControl("rtbWorkHoursUser").Visible = true;
        }
        #endregion

        #region 取消、完成、关闭、不批准
        if (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL
                    || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                    || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE
                    || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE)
        //|| (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE && !task.IsTrace))、批准（不跟踪）状态
        {
            this.FV_ISI.FindControl("btnSave").Visible = false;
            this.FV_ISI.FindControl("btnSubmit").Visible = false;
            this.FV_ISI.FindControl("btnDelete").Visible = false;
            this.FV_ISI.FindControl("btnStart").Visible = false;
            this.FV_ISI.FindControl("btnComplete").Visible = false;
            this.FV_ISI.FindControl("btnCancel").Visible = false;
            this.FV_ISI.FindControl("btnAssign").Visible = false;
            this.FV_ISI.FindControl("btnReturn").Visible = false;
            this.FV_ISI.FindControl("btnApprove").Visible = false;
            this.FV_ISI.FindControl("btnDispute").Visible = false;
            this.FV_ISI.FindControl("btnRefuse").Visible = false;
            this.FV_ISI.FindControl("btnPrint").Visible = (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE) && taskSubType.IsPrint;

            SetEnableBase(false);
            SetEnableFlow(false);
            UpdateAccount(taskSubType.IsCostCenter);
            if (isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                    && this.TheTaskMgr.HasPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, isCloser, this.CurrentUser.Code))
            {
                this.FV_ISI.FindControl("btnReject").Visible = true;
                this.FV_ISI.FindControl("btnClose").Visible = true;
            }
            else
            {
                this.FV_ISI.FindControl("btnReject").Visible = false;
                this.FV_ISI.FindControl("btnClose").Visible = false;
            }

            if ((isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL
                    || isiStatus == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE)
                   && this.TheTaskMgr.HasPermission(task, isISIAdmin, isTaskFlowAdmin, isAssigner, isCloser, this.CurrentUser.Code))
            {
                this.FV_ISI.FindControl("btnOpen").Visible = true;
            }

            this.FV_ISI.FindControl("tbWorkHoursUser").Visible = false;
            this.FV_ISI.FindControl("rtbWorkHoursUser").Visible = true;
        }
        #endregion

        UpdatePage(task, isISIAdmin, isTaskFlowAdmin, isWFAdmin);
    }

    private void SetEnableBase(bool isEnable)
    {
        if (isEnable)
        {
            //追索码
            ((TextBox)this.FV_ISI.FindControl("tbBackYards")).ReadOnly = false;
        }

        //优先级
        ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlPriority")).Enabled = isEnable;
        //地点
        this.FV_ISI.FindControl("tbTaskAddress").Visible = isEnable;
        this.FV_ISI.FindControl("rtbTaskAddress").Visible = !isEnable;

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlPhase")).Enabled = isEnable;
            ((TextBox)this.FV_ISI.FindControl("tbSeq")).Enabled = isEnable;
        }

        //失效模式
        this.FV_ISI.FindControl("tbFailureMode").Visible = isEnable;
        this.FV_ISI.FindControl("rtbFailureMode").Visible = !isEnable;

        //供应商
        this.FV_ISI.FindControl("tbSupplier").Visible = isEnable;
        this.FV_ISI.FindControl("rtbSupplier").Visible = !isEnable;
        //金额
        //((TextBox)this.FV_ISI.FindControl("tbAmount")).ReadOnly = !isEnable;

        ((TextBox)this.FV_ISI.FindControl("tbExtNo")).ReadOnly = !isEnable;

        //((TextBox)this.FV_ISI.FindControl("tbPlanAmount")).ReadOnly = !isEnable;
        //((TextBox)this.FV_ISI.FindControl("tbAmount")).ReadOnly = !isEnable;
        //((TextBox)this.FV_ISI.FindControl("tbPlanWorkHours")).ReadOnly = !isEnable;
        //((TextBox)this.FV_ISI.FindControl("tbWorkHours")).ReadOnly = !isEnable;

        //描述
        ((TextBox)this.FV_ISI.FindControl("tbDesc1")).ReadOnly = !isEnable;
        //主题
        ((TextBox)this.FV_ISI.FindControl("tbSubject")).ReadOnly = !isEnable;

        //分类
        ((Controls_TextBox)this.FV_ISI.FindControl("tbTaskSubType")).Visible = isEnable;
        ((ReadonlyTextBox)this.FV_ISI.FindControl("rtbTaskSubType")).Visible = !isEnable;

        ((TextBox)this.FV_ISI.FindControl("tbEmail")).ReadOnly = !isEnable;
        ((TextBox)this.FV_ISI.FindControl("tbMobilePhone")).ReadOnly = !isEnable;
        ((TextBox)this.FV_ISI.FindControl("tbUserName")).ReadOnly = !isEnable;
    }

    private void SetEnableFlow(bool isEnable)
    {
        //追索码
        ((TextBox)this.FV_ISI.FindControl("tbBackYards")).ReadOnly = !isEnable;
        //预期结果
        ((TextBox)this.FV_ISI.FindControl("tbExpectedResults")).ReadOnly = !isEnable;
        //预计开始、结束时间            
        TextBox tbPlanStartDate = (TextBox)this.FV_ISI.FindControl("tbPlanStartDate");
        TextBox tbPlanCompleteDate = (TextBox)this.FV_ISI.FindControl("tbPlanCompleteDate");
        tbPlanStartDate.ReadOnly = !isEnable;
        tbPlanCompleteDate.ReadOnly = !isEnable;
        if (!isEnable)
        {
            //tbPlanStartDate.Attributes.Add("onfocus", "this.blur();");
            tbPlanStartDate.Attributes.Remove("onclick");
            //tbPlanStartDate.Attributes.Remove("class");
            //tbPlanCompleteDate.Attributes.Add("onfocus", "this.blur();");
            tbPlanCompleteDate.Attributes.Remove("onclick");
            //tbPlanCompleteDate.Attributes.Remove("class");
        }
        else
        {
            //tbPlanStartDate.Attributes.Remove("onfocus");
            tbPlanStartDate.Attributes.Add("onclick", "var " + tbPlanCompleteDate.ClientID + "=$dp.$('" + tbPlanCompleteDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',qsEnabled:true,quickSel:['%y-01-01 08:00:00','%y-02-01 08:00:00','%y-%M-01 08:00:00','%y-%M-15 08:00:00'],dateFmt:'yyyy-MM-dd HH:mm',onpicked:function(){" + tbPlanCompleteDate.ClientID + ".click();},maxDate:'#F{$dp.$D(\\'" + tbPlanCompleteDate.ClientID + "\\')}' })");
            //tbPlanCompleteDate.Attributes.Remove("onfocus");
            tbPlanCompleteDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',qsEnabled:true,quickSel:['%y-%M-15 16:30:00','%y-%M-%ld 16:30:00','%y-{%M+1}-%ld 16:30:00','%y-12-%ld 16:30:00','{%y+1}-01-%ld 16:30:00'],dateFmt:'yyyy-MM-dd HH:mm',minDate:'#F{$dp.$D(\\'" + tbPlanStartDate.ClientID + "\\')}'})");
        }
        //执行人
        ((TextBox)this.FV_ISI.FindControl("tbAssignStartUser")).Visible = isEnable;
        this.FV_ISI.FindControl("rtbAssignStartUser").Visible = !isEnable;
        ((TextBox)this.FV_ISI.FindControl("tbWorkHoursUser")).Visible = isEnable;
        this.FV_ISI.FindControl("rtbWorkHoursUser").Visible = !isEnable;
        //补充描述
        ((TextBox)this.FV_ISI.FindControl("tbDesc2")).ReadOnly = !isEnable;
    }
    void Edit_Render(object sender, EventArgs e)
    {
        ISI_TSK_CostDet ucCostDet = (ISI_TSK_CostDet)this.FV_ISI.FindControl("ucCostDet");
        if (ucCostDet.Visible)
        {
            ucCostDet.ModuleType = this.ModuleType;
            ucCostDet.InitPageParameter(this.TaskCode);
        }
        this.UpdateAmount(((object[])sender)[0].ToString());
        this.UpdateTaxes(((object[])sender)[1].ToString());
        this.UpdateTotalAmount(((object[])sender)[2].ToString());
        this.UpdateQty(((object[])sender)[3].ToString());
    }
    public void UpdateAmount(string amount)
    {
        ((TextBox)(this.FV_ISI.FindControl("tbAmount"))).Text = amount;
    }
    public void UpdateTotalAmount(string totalAmount)
    {
        ((TextBox)(this.FV_ISI.FindControl("tbTotalAmount"))).Text = totalAmount;
    }
    public void UpdateQty(string qty)
    {
        ((TextBox)(this.FV_ISI.FindControl("tbQty"))).Text = qty;
    }
    public void UpdateTaxes(string taxes)
    {
        ((TextBox)(this.FV_ISI.FindControl("tbTaxes"))).Text = taxes;
    }

    private void UpdatePage(TaskMstr task, bool isISIAdmin, bool isTaskFlowAdmin, bool isWFAdmin)//bool isDisable
    {
        ISI_TSK_CostDet ucCostDet = (ISI_TSK_CostDet)this.FV_ISI.FindControl("ucCostDet");

        if (task.TaskSubType.IsCostCenter && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL)
        {
            if (isWFAdmin)
            {
                ucCostDet.Visible = true;
                //ucCostDet.TaskCode = task.Code;
                ucCostDet.ModuleType = task.Type;
                ucCostDet.InitPageParameter(task.Code);
            }
            else
            {
                var processInstanceList = this.TheHqlMgr.FindAll<ProcessInstance>("from ProcessInstance where UserCode !='' and UserCode is not null and TaskCode='" + task.Code + "' and UserCode='" + this.CurrentUser.Code + "' and Level >=" + ISIConstants.CODE_MASTER_WFS_LEVEL3 + " order by Level asc ");
                if (processInstanceList != null && processInstanceList.Count > 0)
                {
                    ucCostDet.Visible = true;
                    //ucCostDet.TaskCode = task.Code;
                    ucCostDet.ModuleType = task.Type;
                    ucCostDet.InitPageParameter(task.Code);
                }
                else
                {
                    ucCostDet.Visible = false;
                }
            }
        }
        else
        {
            ucCostDet.Visible = false;
        }
        this.cbIsRemindCreateUser.Checked = false;
        this.cbIsRemindAssignUser.Checked = false;
        this.cbIsRemindStartUser.Checked = false;
        this.cbIsRemindCommentUser.Checked = false;
        this.cbIsRemindAdmin.Checked = false;
        this.tbHelpUser.Text = string.Empty;
        var taskSubType = task.TaskSubType;
        //this.FV_ISI.FindControl("lblCostCenter").Visible = true;

        this.FV_ISI.FindControl("trWF").Visible = task.IsWF;
        //this.FV_ISI.FindControl("trAccount").Visible = task.IsWF;

        //((Literal)this.FV_ISI.FindControl("lblFailureMode")).Text = task.IsWF ? "${ISI.TSK.CostCenter}:" : (task.Type == ISIConstants.ISI_TASK_TYPE_IMPROVE ? "${ISI.TSK.Mode}:" : "${ISI.TSK.FailureMode}:");
        var ucCostList = (ISI_TSK_CostList)this.FV_ISI.FindControl("ucCostList");

        Controls_TextBox tbCostCenter = (Controls_TextBox)this.FV_ISI.FindControl("tbCostCenter");
        /*
        tbCostCenter.Visible = taskSubType.IsCostCenter && ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION;
        this.FV_ISI.FindControl("rtbCostCenter").Visible = !taskSubType.IsCostCenter || ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        */
        /*
        Controls_TextBox tbAccount1 = (Controls_TextBox)this.FV_ISI.FindControl("tbAccount1");
        tbAccount1.Visible = taskSubType.IsCostCenter && ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION;
        this.FV_ISI.FindControl("rfvAccount1").Visible = tbAccount1.Visible;
        this.FV_ISI.FindControl("rtbAccount1").Visible = !taskSubType.IsCostCenter || ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        Controls_TextBox tbAccount2 = (Controls_TextBox)this.FV_ISI.FindControl("tbAccount2");
        tbAccount2.Visible = taskSubType.IsCostCenter && ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION;
        this.FV_ISI.FindControl("rfvAccount2").Visible = tbAccount1.Visible;
        this.FV_ISI.FindControl("rtbAccount2").Visible = !taskSubType.IsCostCenter || ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        */
        ((TextBox)this.FV_ISI.FindControl("tbAmount")).ReadOnly = ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        ((TextBox)this.FV_ISI.FindControl("tbVoucher")).ReadOnly = ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        ((TextBox)this.FV_ISI.FindControl("tbTotalAmount")).ReadOnly = true;// ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        ((TextBox)this.FV_ISI.FindControl("tbTaxes")).ReadOnly = ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        ((TextBox)this.FV_ISI.FindControl("tbQty")).ReadOnly = ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;
        ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlTravelType")).Enabled = ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION;
        Controls_TextBox tbPayeeCode = (Controls_TextBox)this.FV_ISI.FindControl("tbPayeeCode");
        tbPayeeCode.Visible = taskSubType.IsCostCenter && ucCostList.PageAction != BusinessConstants.PAGE_LIST_ACTION;
        this.FV_ISI.FindControl("rtbPayeeCode").Visible = !taskSubType.IsCostCenter || ucCostList.PageAction == BusinessConstants.PAGE_LIST_ACTION;

        ucCostList.Visible = !string.IsNullOrEmpty(taskSubType.FormType);

        //税金和不含税金额
        this.FV_ISI.FindControl("trAmount").Visible = taskSubType.IsCostCenter || taskSubType.IsAmount || taskSubType.IsAmountDetail;// && !string.IsNullOrEmpty(taskSubType.FormType);

        //成本明细2和成本明细3 显示数量
        this.FV_ISI.FindControl("trQty").Visible = taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2 || taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3;

        if (taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2 || taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3)
        {
            ((Literal)this.FV_ISI.FindControl("lblQty")).Text = "${ISI.TSK.Hours}:";
        }
        else
        {
            ((Literal)this.FV_ISI.FindControl("lblQty")).Text = "${ISI.TSK.Qty}:";
        }

        //持续改进和启用成本控制，显示金额和附件张数
        this.FV_ISI.FindControl("isImp").Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_IMPROVE || taskSubType.IsCostCenter;

        if (this.FV_ISI.FindControl("isImp").Visible && taskSubType.IsAmount)
        {
            TextBox tbAmount = (TextBox)this.FV_ISI.FindControl("tbAmount");
            tbAmount.CssClass = "inputRequired";
            this.FV_ISI.FindControl("rfvAmount").Visible = true;
        }
        else
        {
            TextBox tbAmount = (TextBox)this.FV_ISI.FindControl("tbAmount");
            tbAmount.CssClass = string.Empty;
            this.FV_ISI.FindControl("rfvAmount").Visible = false;
        }

        if (this.FV_ISI.FindControl("isImp").Visible && taskSubType.IsAmountDetail)
        {
            TextBox tbAmount = (TextBox)this.FV_ISI.FindControl("tbAmount");
            TextBox tbTaxes = (TextBox)this.FV_ISI.FindControl("tbTaxes");
            tbAmount.ReadOnly = true;
            tbTaxes.ReadOnly = true;
        }

        //出差类型和领款人
        this.FV_ISI.FindControl("trFormType2").Visible = taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2;

        //标准成本明细    显示供应商
        this.FV_ISI.FindControl("isIss").Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE
                                                        || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ISSUE
                                                        || taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_1;

        if (!string.IsNullOrEmpty(taskSubType.FormType))
        {
            ucCostList.CostCenter = tbCostCenter.Text;
            ucCostList.FormType = taskSubType.FormType;
            ucCostList.IsAmountDetail = taskSubType.IsAmountDetail;
            ucCostList.TaskCode = task.Code;
            ucCostList.IsCostCenter = taskSubType.IsCostCenter;
            var costDetList = this.TheCostMgr.GetCost(task.Code);
            if ((costDetList == null || costDetList.Count == 0)
                            && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE))
            {
                ucCostList.Visible = false;
            }
            else
            {
                //VerifyAccount(costDetList);
                ucCostList.InitPageParameter(costDetList);
            }
        }

        this.FV_ISI.FindControl("fsFlow").Visible = !task.IsWF || task.IsWF && task.IsTrace && task.Level == ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE;

        ((Literal)(this.FV_ISI.FindControl("lblStatus"))).Text = "${ISI.Status." + task.Status + "}";
        ((Literal)(this.FV_ISI.FindControl("lblLevel"))).Visible = task.Level.HasValue;
        if (task.Level.HasValue)
        {
            ((Literal)(this.FV_ISI.FindControl("lblLevel"))).Text = "<font color='blue'>${" + task.Level.Value / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL + "}</font>";
        }
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            this.FV_ISI.FindControl("isPrj").Visible = true;

            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
            {
                ((Literal)(this.FV_ISI.FindControl("lblBackYards"))).Text = "${ISI.TSK.RefTask}:";
                ((Literal)(this.FV_ISI.FindControl("lblSubject"))).Text = "${ISI.TSK.PrjIss.Subject}:";
                ((Literal)(this.FV_ISI.FindControl("lblDesc1"))).Text = "${ISI.TSK.PrjIss.Desc1}:";
                ((Literal)(this.FV_ISI.FindControl("lblExpectedResults"))).Text = "${ISI.TSK.PrjIss.ExpectedResults}:";
                this.FV_ISI.FindControl("isIss").Visible = true;
            }
            ((Literal)(this.FV_ISI.FindControl("ltlTaskSubType"))).Text = "${ISI.TSK.Project}:";
        }
        else
        {
            this.FV_ISI.FindControl("isPrj").Visible = false;
            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_IMPROVE)
            {
                this.FV_ISI.FindControl("isImp").Visible = true;
            }
            else if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_ISSUE)
            {
                this.FV_ISI.FindControl("isIss").Visible = true;
            }
        }

        if (task.Color == ISIConstants.CODE_MASTER_ISI_FLAG_RED)
        {
            ((CodeMstrLabel)(this.FV_ISI.FindControl("cmlFlag"))).BackColor = System.Drawing.Color.Red;
        }
        else if (task.Color == ISIConstants.CODE_MASTER_ISI_FLAG_YELLOW)
        {
            ((CodeMstrLabel)(this.FV_ISI.FindControl("cmlFlag"))).BackColor = System.Drawing.Color.FromArgb(255, 255, 0);
        }
        else if (task.Color == ISIConstants.CODE_MASTER_ISI_FLAG_GREEN)
        {
            ((CodeMstrLabel)(this.FV_ISI.FindControl("cmlFlag"))).BackColor = System.Drawing.Color.Green;
        }

        //((HtmlGenericControl)(this.FV_ISI.FindControl("lgd"))).InnerText = "${ISI.TSK.Update" + this.ModuleType + "}";
        GenerateTree();

        Controls_TextBox tbTaskSubType = (Controls_TextBox)(this.FV_ISI.FindControl("tbTaskSubType"));
        tbTaskSubType.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code;
        tbTaskSubType.DataBind();
        /*
        if (!string.IsNullOrEmpty(task.Template))
        {
            CodeMstrDropDownList ddlTemplate = (CodeMstrDropDownList)(this.FV_ISI.FindControl("ddlTemplate"));
            ddlTemplate.SelectedValue = task.Template;
        }
        */
        //Literal lblAttachmentCount = (Literal)(this.FV_ISI.FindControl("lblAttachmentCount"));
        //lblAttachmentCount.Text = "(<font color='blue'>" + this.TheAttachmentDetailMgr.GetTaskAttachmentCount(this.TaskCode) + "</font>)";

        ((TextBox)(this.FV_ISI.FindControl("tbSchedulingStartUser"))).Text = this.TheUserSubscriptionMgr.GetUserName(task.SchedulingStartUser);

        if (!string.IsNullOrEmpty(task.AssignStartUser))
        {
            TextBox tbAssignStartUser = (TextBox)this.FV_ISI.FindControl("tbAssignStartUser");
            TextBox rtbAssignStartUser = (TextBox)this.FV_ISI.FindControl("rtbAssignStartUser");
            rtbAssignStartUser.Text = string.Empty;
            tbAssignStartUser.Text = string.Empty;
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE ||
                    task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE)
            {
                if (!string.IsNullOrEmpty(task.AssignStartUserNm))
                {
                    rtbAssignStartUser.Text = ISIUtil.GetUserMerge(task.AssignStartUser, task.AssignStartUserNm);
                }
                else
                {
                    string userNames = this.TheUserSubscriptionMgr.GetUserName(task.AssignStartUser);
                    rtbAssignStartUser.Text = ISIUtil.GetUserMerge(task.AssignStartUser, userNames);
                }
            }
            else
            {
                string userNames = this.TheUserSubscriptionMgr.GetUserName(task.AssignStartUser);
                rtbAssignStartUser.Text = ISIUtil.GetUserMerge(task.AssignStartUser, userNames);
                tbAssignStartUser.Text = ISIUtil.GetUserMerge(task.AssignStartUser, userNames);
            }
        }

        if (!string.IsNullOrEmpty(task.CountersignUser))
        {
            TextBox tbCountersignUser = (TextBox)this.FV_ISI.FindControl("tbCountersignUser");
            TextBox rtbCountersignUser = (TextBox)this.FV_ISI.FindControl("rtbCountersignUser");
            CheckBox cbIsCountersignSerial = (CheckBox)this.FV_ISI.FindControl("cbIsCountersignSerial");
            cbIsCountersignSerial.Checked = task.IsCountersignSerial.HasValue && task.IsCountersignSerial.Value;
            rtbCountersignUser.Text = ISIUtil.GetUserMerge(task.CountersignUser, task.CountersignUserNm);
            tbCountersignUser.Text = ISIUtil.GetUserMerge(task.CountersignUser, task.CountersignUserNm);
            this.FV_ISI.FindControl("ctrlTR").Visible = true;
        }

        ((CheckBox)this.FV_ISI.FindControl("cbIsCountersignSerial")).Checked = task.IsCountersignSerial.HasValue && task.IsCountersignSerial.Value;

        if (!string.IsNullOrEmpty(task.WorkHoursUser))
        {
            TextBox tbWorkHoursUser = (TextBox)this.FV_ISI.FindControl("tbWorkHoursUser");
            TextBox rtbWorkHoursUser = (TextBox)this.FV_ISI.FindControl("rtbWorkHoursUser");

            rtbWorkHoursUser.Text = ISIUtil.GetUserMerge(task.WorkHoursUser, task.WorkHoursUserNm);
            tbWorkHoursUser.Text = ISIUtil.GetUserMerge(task.WorkHoursUser, task.WorkHoursUserNm);
            this.FV_ISI.FindControl("workHoursTR2").Visible = true;
        }

        com.Sconit.Control.CodeMstrDropDownList ddlPriority = (com.Sconit.Control.CodeMstrDropDownList)this.FV_ISI.FindControl("ddlPriority");
        if (task.Priority != string.Empty)
        {
            ddlPriority.SelectedValue = task.Priority;
        }
        com.Sconit.Control.CodeMstrDropDownList ddlColor = (com.Sconit.Control.CodeMstrDropDownList)this.FV_ISI.FindControl("ddlColor");
        if (!string.IsNullOrEmpty(task.Color))
        {
            ddlColor.SelectedValue = task.Color;
        }
        if (task.Type == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            com.Sconit.Control.CodeMstrDropDownList ddlPhase =
                (com.Sconit.Control.CodeMstrDropDownList)this.FV_ISI.FindControl("ddlPhase");
            if (!string.IsNullOrEmpty(task.Phase))
            {
                ddlPhase.SelectedValue = task.Phase;
            }
            else
            {
                ddlPhase.SelectedIndex = 0;
            }
            //((TextBox)this.FV_ISI.FindControl("tbSeq")).Text = task.Seq;
        }

        if (task.FailureMode != null)
        {
            Controls_TextBox tbFailureMode = (Controls_TextBox)this.FV_ISI.FindControl("tbFailureMode");
            tbFailureMode.Text = task.FailureMode.Code;
        }

        if (task.SupplierCode != null)
        {
            Controls_TextBox tbSupplier = (Controls_TextBox)this.FV_ISI.FindControl("tbSupplier");
            tbSupplier.Text = task.SupplierCode;
        }

        //tbCostCenter = (Controls_TextBox)this.FV_ISI.FindControl("tbCostCenter");
        if (!string.IsNullOrEmpty(task.CostCenterCode))
        {
            tbCostCenter.Text = task.CostCenterCode;
        }



        Controls_TextBox tbTaskAddress = (Controls_TextBox)this.FV_ISI.FindControl("tbTaskAddress");
        if (task.TaskSubType != null && task.TaskSubType.Code.Trim() != string.Empty)
        {
            tbTaskSubType.Text = task.TaskSubType.Code;
        }
        if (task.TaskAddress != null)
        {
            tbTaskAddress.Text = task.TaskAddress;
        }

        if (task.IsWF)
        {
            this.FV_ISI.FindControl("tbTaskSubType").Visible = false;
            this.FV_ISI.FindControl("rtbTaskSubType").Visible = true;
        }

        if (this.UpdateAttachmentTitleEvent != null)
        {
            this.UpdateAttachmentTitleEvent(new string[] { task.Code, task.ExtNo, task.ProjectTask.ToString(), task.TaskSubType.Code }, null);
        }
    }
    /// <summary>
    /// 验证科目
    /// </summary>
    /// <returns></returns>
    private bool VerifyAccount()
    {
        var ucCostList = (ISI_TSK_CostList)this.FV_ISI.FindControl("ucCostList");
        return this.VerifyAccount(ucCostList.TheCostList);
    }
    /// <summary>
    /// 验证科目
    /// </summary>
    /// <returns></returns>
    private bool VerifyAccount(IList<Cost> costDetList)
    {
        if (this.IsAccountCtrl)
        {
            Controls_TextBox tbAccount1 = ((Controls_TextBox)this.FV_ISI.FindControl("tbAccount1"));
            Controls_TextBox tbAccount2 = ((Controls_TextBox)this.FV_ISI.FindControl("tbAccount2"));
            bool isAccount1 = !string.IsNullOrEmpty(tbAccount1.Text.Trim());
            bool isAccount2 = !string.IsNullOrEmpty(tbAccount2.Text.Trim());

            if (!isAccount1 || !isAccount2)
            {
                if (costDetList == null || costDetList.Count == 0)
                {
                    return false;
                }
                else
                {
                    bool isError = false;
                    foreach (var costDet in costDetList)
                    {
                        costDet.IsAccount1 = !isAccount1 && String.IsNullOrEmpty(costDet.Account1);
                        costDet.IsAccount2 = !isAccount1 && String.IsNullOrEmpty(costDet.Account2);

                        if (costDet.IsAccount1 || costDet.IsAccount2)
                        {
                            isError = true;
                        }
                    }

                    if (isError)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        try
        {
            if (IsWF)
            {
                //验证科目
                /*
                if (!VerifyAccount())
                {
                    this.ShowErrorMessage("WFS.Cost.Account.Error");
                    this.FV_ISI.DataBind();
                    return;
                }
                */
                IList<object> conuntersignList = new List<object>();
                if (this.FV_ISI.FindControl("tbCountersignUser").Visible)
                {
                    string countersignUser = ((TextBox)this.FV_ISI.FindControl("tbCountersignUser")).Text.Trim();
                    bool isCountersignSerial = ((CheckBox)this.FV_ISI.FindControl("cbIsCountersignSerial")).Checked;
                    //验证有效性
                    if (string.IsNullOrEmpty(countersignUser))
                    {
                        //conuntersignList.Add(ISIUtil.GetUser(this.CurrentUser.Code));
                        //conuntersignList.Add(this.CurrentUser.Name);
                        //conuntersignList.Add(isCountersignSerial);
                    }
                    else
                    {
                        string[] userCodeName = this.TheTaskMgr.GetUserCodeName(countersignUser);
                        string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                        if (!string.IsNullOrEmpty(invalidUser))
                        {
                            ShowWarningMessage("ISI.Error.CountersignUserNotExist", new string[] { invalidUser });
                            this.FV_ISI.DataBind();
                            return;
                        }
                        if (userCodeName != null && userCodeName.Length == 2)
                        {
                            string countersignUserCode = ISIUtil.GetUser(userCodeName[0]);
                            string countersignUserName = userCodeName[1];
                            conuntersignList.Add(countersignUserCode);
                            conuntersignList.Add(countersignUserName);
                            conuntersignList.Add(isCountersignSerial);
                        }
                    }
                }
                RequiredFieldValidator rfvApprove = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvApprove");
                if (rfvApprove.IsValid)
                {
                    /*
                    string level = ((HiddenField)this.FV_ISI.FindControl("hfLevel")).Value;
                    //验证
                    if (level == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE)
                    {

                    }*/
                    string approveDesc = ((TextBox)this.FV_ISI.FindControl("tbApprove")).Text.Trim();
                    string color = ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlColor")).SelectedValue;

                    CurrentLevel = this.TheTaskMgr.ProcessNew(this.TaskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_APPROVE, approveDesc, color, conuntersignList, this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN), this.CurrentUser).Level;
                    this.FV_ISI.DataBind();
                    ShowSuccessMessage("ISI.TSK.Approve.Successfully", this.TaskCode);
                }
                else
                {
                    this.FV_ISI.DataBind();
                    this.ShowWarningMessage("ISI.TSK.Approve.Empty");
                }

                if (this.UpdateTitleEvent != null)
                {
                    this.UpdateTitleEvent(sender, e);
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void btnAssign_Click(object sender, EventArgs e)
    {
        try
        {
            //如果是超级管理员和流程管理员 指派用户可以为空，走默认分派
            //
            if (((RequiredFieldValidator)this.FV_ISI.FindControl("rfvPlanCompleteDate")).IsValid
                    && ((RequiredFieldValidator)this.FV_ISI.FindControl("rfvPlanCompleteDate")).IsValid
                    && ((this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                                || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)
                                    && (((RequiredFieldValidator)this.FV_ISI.FindControl("rfvTaskSubType")).IsValid))
                                || ((RequiredFieldValidator)this.FV_ISI.FindControl("rfvAssignStartUser")).IsValid))
            {
                string startTime = ((TextBox)(this.FV_ISI.FindControl("tbPlanStartDate"))).Text.Trim();
                string endTime = ((TextBox)(this.FV_ISI.FindControl("tbPlanCompleteDate"))).Text.Trim();
                DateTime planStartDate = DateTime.Parse(startTime);
                DateTime planCompleteDate = DateTime.Parse(endTime);
                if (DateTime.Compare(planStartDate, planCompleteDate) > 0)
                {
                    this.FV_ISI.DataBind();
                    ShowWarningMessage("ISI.TSK.WarningMessage.TimeCompare");
                }
                else
                {
                    string assignStartUser = ((TextBox)this.FV_ISI.FindControl("tbAssignStartUser")).Text.Trim();
                    string[] userCodeName = ISIUtil.GetUserSplit(assignStartUser);
                    //验证有效性
                    if (!string.IsNullOrEmpty(assignStartUser))
                    {
                        string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                        if (!string.IsNullOrEmpty(invalidUser))
                        {
                            ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
                            this.FV_ISI.DataBind();
                            return;
                        }
                    }
                    string backYards = ((TextBox)this.FV_ISI.FindControl("tbBackYards")).Text.Trim();
                    string taskSubType = ((Controls_TextBox)this.FV_ISI.FindControl("tbTaskSubType")).Text.Trim();
                    string expectedResults = ((TextBox)this.FV_ISI.FindControl("tbExpectedResults")).Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                    string desc2 = ((TextBox)this.FV_ISI.FindControl("tbDesc2")).Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                    this.TheTaskMgr.AssignTask(this.TaskCode, backYards, taskSubType, userCodeName, planStartDate, planCompleteDate, desc2, expectedResults, this.CurrentUser);

                    this.FV_ISI.DataBind();
                    ShowSuccessMessage("ISI.TSK.Assign" + this.ModuleType + ".Successfully", this.TaskCode);

                    if (this.UpdateTitleEvent != null)
                    {
                        this.UpdateTitleEvent(sender, e);
                    }
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            var template = this.TheHqlMgr.FindAll<object>("select t.Template from TaskMstr t where t.Code ='" + TaskCode + "' ");
            if (template != null && template.Count > 0 && template[0] != null && template[0].ToString().Length > 0)
            {
                //string printUrl = TheReportMgr.WriteToFile("WFS.xls", this.TaskCode);
                string printUrl = TheReportMgr.WriteToFile(template[0].ToString(), this.TaskCode);
                Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + printUrl + "'); </script>");
            }
            this.FV_ISI.DataBind();
            ShowSuccessMessage("ISI.TSK.Print.Successfully", this.TaskCode);

            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void btnOpen_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheTaskMgr.OpenTask(this.TaskCode, this.CurrentUser);
            this.FV_ISI.DataBind();
            ShowSuccessMessage("ISI.TSK.Open" + this.ModuleType + ".Successfully", this.TaskCode);

            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    /*
    protected void btnReassign_Click(object sender, EventArgs e)
    {
        try
        {
            if (((RequiredFieldValidator)this.FV_ISI.FindControl("rfvAssignStartUser")).IsValid)
            {
                string assignStartUser = ((Controls_TextBox)this.FV_ISI.FindControl("tbAssignStartUser")).Text.Trim();
                TheTaskMgr.ReassignTask(this.TaskCode, assignStartUser, this.CurrentUser);
                this.FV_ISI.DataBind();
                //UpdateView();
                ShowSuccessMessage("ISI.TSK.Reassign" + this.ModuleType + ".Successfully", this.TaskCode);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    */
    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }
    protected void btnDispute_Click(object sender, EventArgs e)
    {
        try
        {
            if (IsWF && CurrentLevel.HasValue && CurrentLevel != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
            {
                //验证科目
                /*
                if (!VerifyAccount())
                {
                    ShowWarningMessage("WFS.Cost.Account.Error");
                    this.FV_ISI.DataBind();
                    return;
                }
                */
                IList<object> conuntersignList = new List<object>();
                if (this.FV_ISI.FindControl("tbCountersignUser").Visible)
                {
                    string countersignUser = ((TextBox)this.FV_ISI.FindControl("tbCountersignUser")).Text.Trim();
                    bool isCountersignSerial = ((CheckBox)this.FV_ISI.FindControl("cbIsCountersignSerial")).Checked;
                    //验证有效性
                    if (string.IsNullOrEmpty(countersignUser))
                    {
                        //conuntersignList.Add(ISIUtil.GetUser(this.CurrentUser.Code));
                        //conuntersignList.Add(this.CurrentUser.Name);
                        //conuntersignList.Add(isCountersignSerial);
                    }
                    else
                    {
                        string[] userCodeName = this.TheTaskMgr.GetUserCodeName(countersignUser);
                        string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                        if (!string.IsNullOrEmpty(invalidUser))
                        {
                            ShowWarningMessage("ISI.Error.CountersignUserNotExist", new string[] { invalidUser });
                            this.FV_ISI.DataBind();
                            return;
                        }
                        if (userCodeName != null && userCodeName.Length == 2)
                        {
                            string countersignUserCode = ISIUtil.GetUser(userCodeName[0]);
                            string countersignUserName = userCodeName[1];
                            conuntersignList.Add(countersignUserCode);
                            conuntersignList.Add(countersignUserName);
                            conuntersignList.Add(isCountersignSerial);
                        }
                    }
                }
                RequiredFieldValidator rfvApprove = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvApprove");
                if (rfvApprove.IsValid)
                {
                    string approveDesc = ((TextBox)this.FV_ISI.FindControl("tbApprove")).Text.Trim();
                    string color = ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlColor")).SelectedValue;
                    CurrentLevel = this.TheTaskMgr.ProcessNew(this.TaskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE, approveDesc, color, conuntersignList, this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN), this.CurrentUser).Level;
                    this.FV_ISI.DataBind();
                    ShowSuccessMessage("ISI.TSK.Dispute.Successfully", this.TaskCode);
                    if (this.UpdateTitleEvent != null)
                    {
                        this.UpdateTitleEvent(sender, e);
                    }
                }
                else
                {
                    this.ShowWarningMessage("ISI.TSK.ApproveDes.Empty");
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnReturn_Click(object sender, EventArgs e)
    {
        try
        {
            if (IsWF)
            {
                RequiredFieldValidator rfvApprove = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvApprove");
                if (rfvApprove.IsValid)
                {
                    string approveDesc = ((TextBox)this.FV_ISI.FindControl("tbApprove")).Text.Trim();
                    string color = ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlColor")).SelectedValue;
                    CurrentLevel = this.TheTaskMgr.ProcessNew(this.TaskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN, approveDesc, color, this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN), this.CurrentUser).Level;
                    this.FV_ISI.DataBind();
                    ShowSuccessMessage("ISI.TSK.Return.Successfully", this.TaskCode);
                    if (this.UpdateTitleEvent != null)
                    {
                        this.UpdateTitleEvent(sender, e);
                    }
                }
                else
                {
                    this.ShowWarningMessage("ISI.TSK.ApproveDes.Empty");
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheTaskMgr.RejectTask(this.TaskCode, this.CurrentUser);
            this.FV_ISI.DataBind();
            ShowSuccessMessage("ISI.TSK.Reject" + this.ModuleType + ".Successfully", this.TaskCode);
            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheTaskMgr.CloseTask(this.TaskCode, this.CurrentUser);
            this.FV_ISI.DataBind();
            ShowSuccessMessage("ISI.TSK.Close" + this.ModuleType + ".Successfully", this.TaskCode);
            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            TheTaskMgr.DeleteTask(this.TaskCode, this.CurrentUser);

            if (this.BackEvent != null)
            {
                this.BackEvent(string.Empty, e);
            }

            ShowSuccessMessage("ISI.TSK.Delete" + this.ModuleType + ".Successfully", this.TaskCode);

            this.PageCleanup();
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        try
        {
            if (((RequiredFieldValidator)this.FV_ISI.FindControl("rfvPlanCompleteDate")).IsValid
                    && ((RequiredFieldValidator)this.FV_ISI.FindControl("rfvPlanCompleteDate")).IsValid)
            {
                string startTime = ((TextBox)(this.FV_ISI.FindControl("tbPlanStartDate"))).Text.Trim();
                string endTime = ((TextBox)(this.FV_ISI.FindControl("tbPlanCompleteDate"))).Text.Trim();
                DateTime planStartDate = DateTime.Parse(startTime);
                DateTime planCompleteDate = DateTime.Parse(endTime);
                if (DateTime.Compare(planStartDate, planCompleteDate) > 0)
                {
                    this.FV_ISI.DataBind();
                    ShowWarningMessage("ISI.TSK.WarningMessage.TimeCompare");
                }
                else
                {
                    string expectedResults = ((TextBox)this.FV_ISI.FindControl("tbExpectedResults")).Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                    string desc2 = ((TextBox)this.FV_ISI.FindControl("tbDesc2")).Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                    this.TheTaskMgr.ConfirmTask(this.TaskCode, planStartDate, planCompleteDate, desc2, expectedResults, this.CurrentUser);

                    this.FV_ISI.DataBind();
                    ShowSuccessMessage("ISI.TSK.Confirm" + this.ModuleType + ".Successfully", this.TaskCode);
                }
            }
            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnQRCode_Click(object sender, EventArgs e)
    {
        TaskMstr task = this.TheTaskMstrMgr.CheckAndLoadTaskMstr(TaskCode, false);
        StringBuilder content = new StringBuilder();
        content.Append(task.Code);
        content.Append(task.Subject);
        content.Append(task.Desc1);
        content.Append(task.Desc2);
        content.Append(task.ExpectedResults);
        //BarcodeHelper.DownQRCode(content.ToString(), 0, 4);
    }
    protected void btnRefuse_Click(object sender, EventArgs e)
    {
        try
        {
            RequiredFieldValidator rfvApprove = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvApprove");
            if (rfvApprove.IsValid)
            {
                string approveDesc = ((TextBox)this.FV_ISI.FindControl("tbApprove")).Text.Trim();
                string color = ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlColor")).SelectedValue;
                this.CurrentLevel = this.TheTaskMgr.ProcessNew(this.TaskCode, ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE, approveDesc, color, this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN), this.CurrentUser).Level;
                this.FV_ISI.DataBind();
                ShowSuccessMessage("ISI.TSK.Refuse.Successfully", this.TaskCode);
                if (this.UpdateTitleEvent != null)
                {
                    this.UpdateTitleEvent(sender, e);
                }
            }
            else
            {
                this.ShowWarningMessage("ISI.TSK.ApproveDes.Empty");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            var status = this.TheTaskMgr.CancelTask(this.TaskCode, this.CurrentUser);
            this.FV_ISI.DataBind();
            if (status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_REFUSE)
            {
                ShowSuccessMessage("ISI.TSK.Refuse" + this.ModuleType + ".Successfully", this.TaskCode);
            }
            else
            {
                ShowSuccessMessage("ISI.TSK.Cancel" + this.ModuleType + ".Successfully", this.TaskCode);
            }
            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
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
            RequiredFieldValidator rfvTaskSubType = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvTaskSubType");
            RequiredFieldValidator rfvTaskAddress = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvTaskAddress");

            if (rfvTaskSubType.IsValid && rfvTaskAddress.IsValid)
            {
                CodeMstrDropDownList ddlPriority = (CodeMstrDropDownList)this.FV_ISI.FindControl("ddlPriority");
                CodeMstrDropDownList ddlSendType = (CodeMstrDropDownList)this.FV_ISI.FindControl("ddlSendType");
                //TextBox tbCode = (TextBox)this.FV_ISI.FindControl("tbCode");
                TextBox tbDesc1 = (TextBox)this.FV_ISI.FindControl("tbDesc1");
                //TextBox tbDesc2 = (TextBox)this.FV_ISI.FindControl("tbDesc2");
                TextBox tbSubject = (TextBox)this.FV_ISI.FindControl("tbSubject");
                TextBox tbBackYards = (TextBox)this.FV_ISI.FindControl("tbBackYards");
                CodeMstrDropDownList ddlBackYards = (CodeMstrDropDownList)this.FV_ISI.FindControl("ddlBackYards");
                Controls_TextBox tbTaskSubType = (Controls_TextBox)this.FV_ISI.FindControl("tbTaskSubType");
                Controls_TextBox tbTaskAddress = (Controls_TextBox)this.FV_ISI.FindControl("tbTaskAddress");

                TextBox tbUserName = (TextBox)this.FV_ISI.FindControl("tbUserName");
                TextBox tbEmail = (TextBox)this.FV_ISI.FindControl("tbEmail");
                TextBox tbMobilePhone = (TextBox)this.FV_ISI.FindControl("tbMobilePhone");

                TextBox tbAmount = (TextBox)this.FV_ISI.FindControl("tbAmount");
                TextBox tbTotalAmount = (TextBox)this.FV_ISI.FindControl("tbTotalAmount");
                TextBox tbTaxes = (TextBox)this.FV_ISI.FindControl("tbTaxes");
                TextBox tbQty = (TextBox)this.FV_ISI.FindControl("tbQty");
                TextBox tbVoucher = (TextBox)this.FV_ISI.FindControl("tbVoucher");
                Controls_TextBox tbPayeeCode = (Controls_TextBox)this.FV_ISI.FindControl("tbPayeeCode");
                Controls_TextBox tbSupplier = (Controls_TextBox)this.FV_ISI.FindControl("tbSupplier");

                TaskMstr task = new TaskMstr();//TheTaskMstrMgr.CheckAndLoadTaskMstr(this.TaskCode);
                task.TaskSubType = TheTaskSubTypeMgr.LoadTaskSubType(tbTaskSubType.Text.Trim());
                if (task.TaskSubType == null)
                {
                    ShowWarningMessage("ISI.Error.TaskCodeNotExist");
                    return;
                }

                if (task.TaskSubType.IsCostCenter)
                {
                    RequiredFieldValidator rfvAccount1 = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvAccount1");
                    RequiredFieldValidator rfvAccount2 = (RequiredFieldValidator)this.FV_ISI.FindControl("rfvAccount2");
                    if (!rfvAccount1.IsValid || !rfvAccount2.IsValid)
                    {
                        return;
                    }
                }

                if (ddlPriority.SelectedIndex != -1)
                {
                    task.Priority = ddlPriority.SelectedValue;
                }
                task.Type = this.ModuleType;
                task.Code = this.TaskCode;
                task.BackYards = tbBackYards.Text.Trim();
                task.TaskAddress = tbTaskAddress.Text.Trim();
                task.Subject = tbSubject.Text.Trim();
                task.Desc1 = tbDesc1.Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                //task.Desc2 = tbDesc2.Text.Trim();
                //TaskSubType taskSubType = new TaskSubType();
                //taskSubType.Code = tbTaskSubType.Text.Trim();

                task.FormType = task.TaskSubType.FormType;
                if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
                {
                    TextBox tbSeq = (TextBox)this.FV_ISI.FindControl("tbSeq");
                    task.Seq = tbSeq.Text.Trim();
                    task.Phase = ((CodeMstrDropDownList)this.FV_ISI.FindControl("ddlPhase")).SelectedValue;
                }

                if (!string.IsNullOrEmpty(tbSupplier.Text.Trim()))
                {
                    var supplier = this.TheSupplierMgr.LoadSupplier(tbSupplier.Text.Trim());
                    if (supplier != null)
                    {
                        task.SupplierCode = supplier.Code;
                        task.SupplierName = supplier.Name;
                    }
                }
                if (!string.IsNullOrEmpty(tbPayeeCode.Text.Trim()))
                {
                    var user = this.TheUserMgr.LoadUser(tbPayeeCode.Text.Trim());
                    task.PayeeCode = user.Code;
                    task.PayeeName = user.Name;
                }
                else
                {
                    task.PayeeCode = string.Empty;
                    task.PayeeName = string.Empty;
                }
                if (!string.IsNullOrEmpty(tbVoucher.Text.Trim()))
                {
                    task.Voucher = int.Parse(tbVoucher.Text.Trim());
                }
                else
                {
                    task.Voucher = null;
                }
                if (!string.IsNullOrEmpty(tbQty.Text.Trim()))
                {
                    task.Qty = decimal.Parse(tbQty.Text.Trim());
                }
                else
                {
                    task.Qty = null;
                }
                if (!string.IsNullOrEmpty(tbAmount.Text.Trim()))
                {
                    task.Amount = decimal.Parse(tbAmount.Text.Trim());
                }
                else
                {
                    task.Amount = null;
                }
                if (!string.IsNullOrEmpty(tbTaxes.Text.Trim()))
                {
                    task.Taxes = decimal.Parse(tbTaxes.Text.Trim());
                }
                if (task.Amount.HasValue || task.Taxes.HasValue)
                {
                    task.TotalAmount = (task.Amount.HasValue ? task.Amount.Value : 0) + (task.Taxes.HasValue ? task.Taxes.Value : 0);
                }
                else if (!string.IsNullOrEmpty(tbTotalAmount.Text.Trim()))
                {
                    task.TotalAmount = decimal.Parse(tbTotalAmount.Text.Trim());
                }
                else
                {
                    task.TotalAmount = null;
                }

                Controls_TextBox tbFailureMode = (Controls_TextBox)this.FV_ISI.FindControl("tbFailureMode");
                if (!string.IsNullOrEmpty(tbFailureMode.Text.Trim()))
                {
                    //FailureMode failureMode = new FailureMode();
                    //failureMode.Code = tbFailureMode.Text.Trim();
                    task.FailureMode = TheFailureModeMgr.LoadFailureMode(tbFailureMode.Text.Trim());
                }

                TaskSubType costCenter = null;
                Controls_TextBox tbCostCenter = (Controls_TextBox)this.FV_ISI.FindControl("tbCostCenter");
                if (!string.IsNullOrEmpty(tbCostCenter.Text.Trim()))
                {
                    costCenter = TheTaskSubTypeMgr.LoadTaskSubType(tbCostCenter.Text.Trim());
                    task.CostCenterCode = costCenter.Code;
                    task.CostCenterDesc = costCenter.Desc;
                }
                else if (!string.IsNullOrEmpty(this.CurrentUser.CostCenter))
                {
                    costCenter = TheTaskSubTypeMgr.LoadTaskSubType(this.CurrentUser.CostCenter);
                    task.CostCenterCode = costCenter.Code;
                    task.CostCenterDesc = costCenter.Desc;
                }
                else
                {
                    task.CostCenterCode = string.Empty;
                    task.CostCenterDesc = string.Empty;
                }

                string account1Code = ((Controls_TextBox)FV_ISI.FindControl("tbAccount1")).Text.Trim();
                if (!string.IsNullOrEmpty(account1Code))
                {
                    var account = this.TheAccountMgr.LoadAccount(account1Code);
                    task.Account1 = account.Code;
                    task.Account1Desc = account.Desc1;
                }
                else
                {
                    task.Account1 = null;
                    task.Account1Desc = null;
                }

                string account2Code = ((Controls_TextBox)FV_ISI.FindControl("tbAccount2")).Text.Trim();
                if (!string.IsNullOrEmpty(account2Code))
                {
                    var account = this.TheAccountMgr.LoadAccount(account2Code);
                    task.Account2 = account.Code;
                    task.Account2Desc = account.Desc1;
                }
                else
                {
                    task.Account2 = null;
                    task.Account2Desc = null;
                }

                task.Dept = this.CurrentUser.CostCenter;
                task.UserName = tbUserName.Text.Trim();
                task.Email = tbEmail.Text.Trim();
                task.MobilePhone = tbMobilePhone.Text.Trim();

                //跟踪信息
                task.ExpectedResults = ((TextBox)this.FV_ISI.FindControl("tbExpectedResults")).Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                task.Desc2 = ((TextBox)this.FV_ISI.FindControl("tbDesc2")).Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());

                string startTime = ((TextBox)this.FV_ISI.FindControl("tbPlanStartDate")).Text.Trim();
                string endTime = ((TextBox)this.FV_ISI.FindControl("tbPlanCompleteDate")).Text.Trim();

                if (!string.IsNullOrEmpty(startTime))
                {
                    task.PlanStartDate = DateTime.Parse(startTime);
                }
                if (!string.IsNullOrEmpty(endTime))
                {
                    task.PlanCompleteDate = DateTime.Parse(endTime);
                }
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime)
                            && task.PlanStartDate.HasValue && task.PlanCompleteDate.HasValue)
                {
                    if (DateTime.Compare(task.PlanStartDate.Value, task.PlanCompleteDate.Value) > 0)
                    {
                        ShowWarningMessage("ISI.TSK.WarningMessage.TimeCompare");
                        this.FV_ISI.DataBind();
                        return;
                    }
                }

                string assignStartUser = ((TextBox)this.FV_ISI.FindControl("tbAssignStartUser")).Text.Trim();

                //验证有效性
                if (!string.IsNullOrEmpty(assignStartUser))
                {
                    string[] userCodeName = this.TheTaskMgr.GetUserCodeName(assignStartUser);

                    string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                    if (!string.IsNullOrEmpty(invalidUser))
                    {
                        ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
                        this.FV_ISI.DataBind();
                        return;
                    }

                    if (userCodeName != null && userCodeName.Length == 2)
                    {
                        var assignStartUserCode = ISIUtil.GetUser(userCodeName[0]);
                        if (task.AssignStartUser != assignStartUserCode)
                        {
                            task.AssignStartUser = assignStartUserCode;
                            task.AssignStartUserNm = userCodeName[1];
                        }
                    }
                }

                string workHoursUser = ((TextBox)this.FV_ISI.FindControl("tbWorkHoursUser")).Text.Trim();

                //验证有效性
                if (!string.IsNullOrEmpty(workHoursUser))
                {
                    string[] userCodeName = this.TheTaskMgr.GetUserCodeName(workHoursUser);

                    string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                    if (!string.IsNullOrEmpty(invalidUser))
                    {
                        ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
                        this.FV_ISI.DataBind();
                        return;
                    }

                    if (userCodeName != null && userCodeName.Length == 2)
                    {
                        var workHoursUserCode = ISIUtil.GetUser(userCodeName[0]);
                        if (task.WorkHoursUser != workHoursUserCode)
                        {
                            task.WorkHoursUser = workHoursUserCode;
                            task.WorkHoursUserNm = userCodeName[1];
                        }
                    }
                    else
                    {
                        task.WorkHoursUser = null;
                        task.WorkHoursUserNm = null;
                    }
                }
                else
                {
                    task.WorkHoursUser = null;
                    task.WorkHoursUserNm = null;
                }

                if (this.TaskApplyList != null && this.TaskApplyList != null)
                {
                    IList<TaskApply> taskApplyList = new List<TaskApply>();

                    for (int i = 0; i < TaskApplyList.Count; i++)
                    {
                        var taskApply = TaskApplyList[i];
                        string id = taskApply.Apply + taskApply.UOM + taskApply.Seq + taskApply.Id;

                        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                        {
                            string idType = string.Empty;
                            if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX) idType = "cb";
                            if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO) idType = "rb";
                            var applyList = TaskApplyList.Where(p => p.Type == taskApply.Type && p.Seq == taskApply.Seq).OrderBy(p => p.Seq).ToList();
                            for (int j = 0; j < applyList.Count; j++)
                            {
                                string value = string.Empty;
                                if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX)
                                {
                                    value = Request.Form[FV_ISI.UniqueID + "$" + idType + id + "$" + j];
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        applyList[j].Checked = true;
                                    }
                                    else
                                    {
                                        applyList[j].Checked = false;
                                    }
                                }
                                if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                                {
                                    value = Request.Form[FV_ISI.UniqueID + "$" + idType + id];
                                    if (applyList[j].Apply == value)
                                    {
                                        applyList[j].Checked = true;
                                    }
                                    else
                                    {
                                        applyList[j].Checked = false;
                                    }
                                }
                                taskApplyList.Add(applyList[j]);
                            }
                            i += applyList.Count - 1;
                        }
                        else
                        {
                            if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK || taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_LABEL)
                            {
                                taskApplyList.Add(taskApply);
                            }
                            else
                            {
                                string value = Request.Form[FV_ISI.UniqueID + "$tb" + id];

                                if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_TEXTBOX && !string.IsNullOrEmpty(taskApply.ServicePath)
                                        && !string.IsNullOrEmpty(taskApply.ServiceMethod) && !string.IsNullOrEmpty(taskApply.DescField) && !string.IsNullOrEmpty(taskApply.ValueField))
                                {
                                    //Controls_TextBox tb = (Controls_TextBox)this.FV_ISI.FindControl(tbId);
                                    //if (tb.Text.Trim() != string.Empty)
                                    {
                                        taskApply.Value = value.Trim();
                                    }
                                }
                                else
                                {
                                    //TextBox tb = (TextBox)this.FV_ISI.FindControl(tbId);
                                    //if (tb.Text.Trim() != string.Empty)
                                    {
                                        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_QTY)
                                        {
                                            if (value != String.Empty)
                                            {
                                                taskApply.Qty = decimal.Parse(value.Trim());
                                            }
                                            else
                                            {
                                                taskApply.Qty = null;
                                            }
                                        }
                                        else if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATE || taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATETIME)
                                        {
                                            if (value != String.Empty)
                                            {
                                                taskApply.DateValue = DateTime.Parse(value.Trim());
                                            }
                                            else
                                            {
                                                taskApply.DateValue = null;
                                            }
                                        }
                                        else
                                        {
                                            taskApply.Value = value.Trim();
                                        }
                                    }
                                }
                                taskApplyList.Add(taskApply);
                            }
                        }
                    }

                    if (taskApplyList.Count > 0)
                    {
                        task.TaskApplyList = taskApplyList;
                    }
                }

                TheTaskMstrMgr.UpdateTaskMstr(task, this.CurrentUser);

                this.FV_ISI.DataBind();

                ShowSuccessMessage("ISI.TSK.Update" + this.ModuleType + ".Successfully", task.Code);

                if (this.UpdateTitleEvent != null)
                {
                    this.UpdateTitleEvent(sender, e);
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnComplete_Click(object sender, EventArgs e)
    {
        try
        {
            string desc2 = ((TextBox)this.FV_ISI.FindControl("tbDesc2")).Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());

            TheTaskMgr.CompleteTask(this.TaskCode, desc2, this.CurrentUser);
            this.FV_ISI.DataBind();
            //UpdateView();
            ShowSuccessMessage("ISI.TSK.Complete" + this.ModuleType + ".Successfully", this.TaskCode);

            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private void GenerateTree()
    {

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            var ucCostList = (ISI_TSK_CostList)this.FV_ISI.FindControl("ucCostList");
            if (ucCostList.Visible)
            {
                if (ucCostList.TheCostList == null || ucCostList.TheCostList.Count == 0)
                {
                    this.ShowErrorMessage("WFS.Cost.Warn.DetailEmpty");
                    return;
                }
            }

            this.TheTaskMgr.SubmitTask(this.TaskCode, this.CurrentUser);
            this.FV_ISI.DataBind();
            ShowSuccessMessage("ISI.TSK.Submit" + this.ModuleType + ".Successfully", this.TaskCode);

            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnHelp_Click(object sender, EventArgs e)
    {
        tbHelpUser.Text = string.Empty;
        tbHelpContent.Text = string.Empty;
        this.fsHelp.Visible = true;
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.cbIsRemindCreateUser.Checked ||
                this.cbIsRemindAssignUser.Checked ||
                this.cbIsRemindStartUser.Checked ||
                this.cbIsRemindCommentUser.Checked ||
                this.cbIsRemindAdmin.Checked ||
                !string.IsNullOrEmpty(this.tbHelpUser.Text.Trim()))
            {
                TheTaskMgr.HelpTask(this.TaskCode, this.tbHelpUser.Text.Trim(),
                                    this.cbIsRemindCreateUser.Checked,
                                    this.cbIsRemindAssignUser.Checked,
                                    this.cbIsRemindStartUser.Checked,
                                    this.cbIsRemindCommentUser.Checked,
                                    this.cbIsRemindAdmin.Checked,
                                    this.tbHelpContent.Text.Trim(), this.CurrentUser);
                this.ShowSuccessMessage("ISI.TSK.Help.Successfully");
                this.fsHelp.Visible = false;
                if (this.UpdateTitleEvent != null)
                {
                    this.UpdateTitleEvent(sender, e);
                }
            }
            else
            {
                this.ShowWarningMessage("ISI.TSK.Help.PleaseSelectHelp");
            }
        }
        catch (Exception ee)
        {
            this.ShowErrorMessage("ISI.TSK.Help.Fail");
        }
    }

    protected void btnSendClose_Click(object sender, EventArgs e)
    {
        this.fsHelp.Visible = false;

        if (this.UpdateTitleEvent != null)
        {
            this.UpdateTitleEvent(sender, e);
        }

    }
}