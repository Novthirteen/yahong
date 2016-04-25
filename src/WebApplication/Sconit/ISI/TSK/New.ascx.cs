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
using com.Sconit.Entity.Exception;
using System.Text;

public partial class ISI_TSK_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
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

    public string Subject
    {
        get
        {
            return (string)ViewState["Subject"];
        }
        set
        {
            ViewState["Subject"] = value;
        }
    }
    public string BackYards
    {
        get
        {
            return (string)ViewState["BackYards"];
        }
        set
        {
            ViewState["BackYards"] = value;
        }
    }
    public string ExpectedResults
    {
        get
        {
            return (string)ViewState["ExpectedResults"];
        }
        set
        {
            ViewState["ExpectedResults"] = value;
        }
    }
    public string TaskAddress
    {
        get
        {
            return (string)ViewState["TaskAddress"];
        }
        set
        {
            ViewState["TaskAddress"] = value;
        }
    }
    public string Desc1
    {
        get
        {
            return (string)ViewState["Desc1"];
        }
        set
        {
            ViewState["Desc1"] = value;
        }
    }
    public string Desc2
    {
        get
        {
            return (string)ViewState["Desc2"];
        }
        set
        {
            ViewState["Desc2"] = value;
        }
    }
    public string PlanStartDate
    {
        get
        {
            return (string)ViewState["PlanStartDate"];
        }
        set
        {
            ViewState["PlanStartDate"] = value;
        }
    }

    public string PlanCompleteDate
    {
        get
        {
            return (string)ViewState["PlanCompleteDate"];
        }
        set
        {
            ViewState["PlanCompleteDate"] = value;
        }
    }
    public IList<ProcessApply> ProcessApplyList
    {
        get
        {
            return (IList<ProcessApply>)ViewState["ProcessApplyList"];
        }
        set
        {
            ViewState["ProcessApplyList"] = value;
        }
    }

    public bool CurrentIsApply
    {
        get
        {
            return ViewState["CurrentIsApply"] == null ? false : (bool)ViewState["CurrentIsApply"];
        }
        set
        {
            ViewState["CurrentIsApply"] = value;
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


    public void InitPageParameter()
    {
        btnBack.Visible = true;
    }
    protected void tbCostCenter_TextChanged(Object sender, EventArgs e)
    {
        try
        {
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
            if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()) && (string.IsNullOrEmpty(this.CurrentTaskSubType) || this.CurrentTaskSubType != this.tbTaskSubType.Text))
            {
                this.CurrentTaskSubType = this.tbTaskSubType.Text.Trim();
                if (!string.IsNullOrEmpty(CurrentTaskSubType))
                {
                    var taskSubType = TheTaskSubTypeMgr.LoadTaskSubType(CurrentTaskSubType);
                    if (taskSubType != null)
                    {
                        tbCostCenter.Visible = taskSubType.IsCostCenter;
                        rtbCostCenter.Visible = !taskSubType.IsCostCenter;
                        this.rfvCostCenter.Visible = taskSubType.IsCostCenter;
                        if (taskSubType != null)
                        {
                            this.ucCostList.Visible = !string.IsNullOrEmpty(taskSubType.FormType);

                            //税金和不含税金额
                            this.trAmount.Visible = taskSubType.IsCostCenter || taskSubType.IsAmount || taskSubType.IsAmountDetail;// && !string.IsNullOrEmpty(taskSubType.FormType);

                            //成本明细2和成本明细3 显示数量
                            this.trQty.Visible = taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2 || taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3;

                            //持续改进和启用成本控制，显示金额和附件张数
                            this.isImp.Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_IMPROVE || taskSubType.IsCostCenter;
                            if (this.isImp.Visible && taskSubType.IsAmount)
                            {
                                tbAmount.CssClass = "inputRequired";
                                rfvAmount.Visible = true;
                            }
                            else
                            {
                                tbAmount.CssClass = string.Empty;
                                rfvAmount.Visible = false;
                            }
                            if (this.isImp.Visible && taskSubType.IsAmountDetail)
                            {
                                tbAmount.ReadOnly = true;
                                tbTaxes.ReadOnly = true;
                            }
                            //出差类型和领款人
                            this.trFormType2.Visible = taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2;

                            if (taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2 || taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3)
                            {
                                this.lblQty.Text = "${ISI.TSK.Hours}:";
                            }
                            else
                            {
                                this.lblQty.Text = "${ISI.TSK.Qty}:";
                            }

                            //标准成本明细    显示供应商
                            this.isIss.Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE
                                                        || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ISSUE
                                                        || taskSubType.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_1;

                            if (!string.IsNullOrEmpty(taskSubType.FormType))
                            {
                                this.ucCostList.CostCenter = tbCostCenter.Text;
                                this.ucCostList.TaskCode = string.Empty;
                                this.ucCostList.IsCostCenter = taskSubType.IsCostCenter;
                                this.ucCostList.FormType = taskSubType.FormType;
                                this.ucCostList.IsAmountDetail = taskSubType.IsAmountDetail;
                                this.ucCostList.PageAction = BusinessConstants.PAGE_NEW_ACTION;
                                this.ucCostList.InitPageParameter();
                            }


                            fsFlow.Visible = !taskSubType.IsWF;
                            //this.tbPlanStartDate.Text = string.Empty;
                            this.trWF.Visible = taskSubType.IsWF;
                            this.trAccount.Visible = taskSubType.IsCostCenter;
                            this.tbCostCenter.Text = this.CurrentUser.CostCenter;
                            rtbCostCenter.Text = this.CurrentUser.CostCenter;
                            //ltlCostCenter.Text = this.CurrentUser.CostCenter;
                            CurrentIsApply = taskSubType.IsApply;
                            //GenerateGynamic();
                            /*
                            for (int i = 7; i < taskTable.Rows.Count; i++)
                            {
                                taskTable.Rows.RemoveAt(i);
                            }*/
                            this.cbIsAutoRelease.Visible = !taskSubType.IsAttachment;

                            if (!string.IsNullOrEmpty(taskSubType.CostCenter))
                            {
                                this.tbCostCenter.Text = taskSubType.CostCenter;
                            }
                            if (!string.IsNullOrEmpty(taskSubType.Account1))
                            {
                                this.tbAccount1.Text = taskSubType.Account1;
                            }
                            if (!string.IsNullOrEmpty(taskSubType.Account2))
                            {
                                this.tbAccount2.Text = taskSubType.Account2;
                            }
                        }
                    }
                    else
                    {
                        this.cbIsAutoRelease.Visible = false;
                        CurrentIsApply = false;
                        fsFlow.Visible = true;
                        ProcessApplyList = null;
                    }
                }
                else
                {
                    this.cbIsAutoRelease.Visible = false;
                    CurrentIsApply = false;
                    fsFlow.Visible = true;
                    ProcessApplyList = null;
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void lbtnDownLoad_Click(object sender, EventArgs e)
    {
        string id = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;
        string fileName = string.Empty;
        try
        {
            TheAttachmentDetailMgr.DownLoadFile(int.Parse(id), System.Web.HttpContext.Current.Request, Response, Server);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {

        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        AttachmentDetail attachment = (AttachmentDetail)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            System.Web.UI.WebControls.LinkButton lbtnDownLoad = (System.Web.UI.WebControls.LinkButton)e.Row.FindControl("lbtnDownLoad");
            if (lbtnDownLoad != null)
            {
                lbtnDownLoad.Text = attachment.FileName;
            }
        }
    }

    protected void GenerateGynamic()
    {
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            var attachmentList = this.TheAttachmentDetailMgr.GetTaskSubTypeAttachment(this.tbTaskSubType.Text);
            if (attachmentList != null && attachmentList.Count > 0)
            {
                HtmlTableRow tr = null;
                HtmlTableCell tc1 = null;
                HtmlTableCell tc2 = null;
                tr = new HtmlTableRow();
                tc1 = new HtmlTableCell();
                tc2 = new HtmlTableCell();
                tc1.Attributes.Remove("class");
                tc2.Attributes.Remove("class");
                tc1.Attributes.Remove("style");
                tc2.Attributes.Remove("style");
                tc1.Attributes.Add("class", "td01");
                tc2.Attributes.Add("class", "td02");

                tr.Controls.Add(tc1);
                tr.Controls.Add(tc2);
                Label lblAttachment = new Label();
                //lblAttachment.ID = "lblAttachment";
                lblAttachment.Text = "${ISI.TSK.TaskSubType.Attachment}:";
                tc1.Controls.Add(lblAttachment);

                tc2.ColSpan = 3;
                int count = taskTable.Rows.Count;
                taskTable.Rows.Insert(count, tr);

                for (int i = 0; i < attachmentList.Count; i++)
                {
                    var attachment = attachmentList[i];
                    System.Web.UI.WebControls.LinkButton lbtnDownLoad = new System.Web.UI.WebControls.LinkButton();
                    lbtnDownLoad.ID = "lbtnDownLoad" + attachment.Id.ToString();
                    lbtnDownLoad.Click += new System.EventHandler(this.lbtnDownLoad_Click);
                    lbtnDownLoad.CommandArgument = attachment.Id.ToString();
                    lbtnDownLoad.ToolTip = attachment.CreateUserNm + "(" + attachment.CreateDate.ToString("yyyy-MM-dd HH:mm") + ")";
                    lbtnDownLoad.Text = attachment.FileName;
                    if (i != 0)
                    {
                        Label lbl = new Label();
                        lbl.ID = "lblAttachment" + attachment.Id.ToString();
                        lbl.Text = "&nbsp;&nbsp;";
                        tc2.Controls.Add(lbl);
                    }
                    tc2.Controls.Add(lbtnDownLoad);
                }
            }
        }

        string tst = this.tbTaskSubType.Text.Trim();
        if (string.IsNullOrEmpty(tst))
        {
            ProcessApplyList = null;
            return;
        }
        else
        {
            var tstList = this.TheHqlMgr.FindAll<bool>("select tst.IsApply from TaskSubType tst where tst.Code='" + tst + "'");
            if (tstList != null && tstList.Count > 0)
            {
                this.CurrentIsApply = tstList[0];
            }
            if (tstList == null || tstList.Count == 0 || !CurrentIsApply)
            {
                ProcessApplyList = null;
                return;
            }
        }

        ProcessApplyList = this.TheProcessApplyMgr.GetProcessApply(this.tbTaskSubType.Text.Trim());
        if (ProcessApplyList != null && ProcessApplyList.Count > 0)
        {
            HtmlTableRow tr = null;
            int j = 0;
            this.workHoursTR2.Visible = ProcessApplyList.Where(p => p.IsUser.HasValue && p.IsUser.Value).Count() > 0;
            for (int i = 0; i < ProcessApplyList.Count; i++)
            {
                var processApply = ProcessApplyList[i];
                string id = processApply.Apply + processApply.UOM + processApply.Seq + processApply.Id;

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

                if (j % 2 == 0
                        || processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK || ProcessApplyList[i - 1].Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK
                        || processApply.IsRow.HasValue && processApply.IsRow.Value || ProcessApplyList[i - 1].IsRow.HasValue && ProcessApplyList[i - 1].IsRow.Value)
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

                if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK)
                {
                    tc1.ColSpan = 4;
                    tr.Controls.Remove(tc2);
                    j = 0;
                    continue;
                }

                StringBuilder style = new StringBuilder();
                if (processApply.FontSize.HasValue && processApply.FontSize.Value > 0)
                {
                    style.Append("font-size: " + processApply.FontSize.Value + "px;");
                }
                if (!string.IsNullOrEmpty(processApply.Align))
                {
                    style.Append("text-align: " + processApply.Align + ";");
                }
                if (style.Length > 0)
                {
                    tc1.Attributes.Add("style", style.ToString());
                }

                if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_LABEL)
                {
                    Label lbl = new Label();
                    if (!string.IsNullOrEmpty(processApply.Color))
                    {
                        lbl.Attributes.Add("style", "color: " + processApply.Color + ";");
                    }
                    lbl.ID = "lbl" + id;
                    lbl.Text = processApply.GetDesc(this.CurrentUser.UserLanguage);
                    tc1.Controls.Add(lbl);
                    tr.Controls.Remove(tc2);
                    if (processApply.IsRow.HasValue && processApply.IsRow.Value)
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
                    if (processApply.IsRow.HasValue && processApply.IsRow.Value)
                    {
                        tc2.ColSpan = 3;
                        j = 0;
                    }
                    else
                    {
                        tc2.ColSpan = -1;
                        j++;
                    }

                    if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX || processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                    {
                        var applyList = ProcessApplyList.Where(p => p.Type == processApply.Type && p.Seq == processApply.Seq).OrderBy(p => p.Seq).ToList();
                        var desc = processApply.UOMDesc(this.CurrentUser.UserLanguage);
                        if (!string.IsNullOrEmpty(desc))
                        {
                            Label lbl = new Label();
                            if (!string.IsNullOrEmpty(processApply.Color))
                            {
                                lbl.Attributes.Add("style", "color: " + processApply.Color + ";");
                            }
                            lbl.Text = desc + ":";
                            tc1.Controls.Add(lbl);
                        }

                        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX)
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
                            if (processApply.IsVertical.HasValue && processApply.IsVertical.Value)
                            {
                                cbListProcessApply.RepeatDirection = RepeatDirection.Vertical;
                            }
                            else
                            {
                                cbListProcessApply.RepeatDirection = RepeatDirection.Horizontal;
                            }
                            if (processApply.RepeatColumns.HasValue && processApply.RepeatColumns.Value > 0)
                            {
                                cbListProcessApply.RepeatColumns = processApply.RepeatColumns.Value;
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
                                    cbListProcessApply.Items[ii].Selected = applyList[ii].Required.HasValue ? applyList[ii].Required.Value : false;
                                }
                            }
                            tc2.Controls.Add(cbListProcessApply);
                        }
                        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
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
                            if (processApply.IsVertical.HasValue && processApply.IsVertical.Value)
                            {
                                rbListProcessApply.RepeatDirection = RepeatDirection.Vertical;
                            }
                            else
                            {
                                rbListProcessApply.RepeatDirection = RepeatDirection.Horizontal;
                            }
                            if (processApply.RepeatColumns.HasValue && processApply.RepeatColumns.Value > 0)
                            {
                                rbListProcessApply.RepeatColumns = processApply.RepeatColumns.Value;
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
                                    rbListProcessApply.Items[ii].Selected = applyList[ii].Required.HasValue ? applyList[ii].Required.Value : false;
                                }
                            }
                            tc2.Controls.Add(rbListProcessApply);
                        }
                        i += applyList.Count - 1;
                    }
                    else
                    {
                        string tbId = "tb" + id;
                        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_TEXTBOX && !string.IsNullOrEmpty(processApply.ServicePath)
                                && !string.IsNullOrEmpty(processApply.ServiceMethod) && !string.IsNullOrEmpty(processApply.DescField) && !string.IsNullOrEmpty(processApply.ValueField))
                        {
                            Controls_TextBox tb = new Controls_TextBox();
                            NewTextBox(tc2, processApply, tb, id);
                        }
                        else
                        {
                            TextBox tb = new TextBox();
                            NewTextBox(tc2, processApply, tb, id);
                        }

                        Validator(tc1, tc2, processApply, id);
                    }
                }
            }
        }
    }

    private void NewTextBox(HtmlTableCell tc2, ProcessApply processApply, Controls_TextBox tb, string id)
    {
        tb.ID = "tb" + id;

        tb.ServicePath = processApply.ServicePath;
        tb.ServiceMethod = processApply.ServiceMethod;
        tb.MustMatch = processApply.MustMatch.HasValue ? processApply.MustMatch.Value : false;
        tb.DescField = processApply.DescField;
        tb.ValueField = processApply.ValueField;
        tb.SuggestPhLocaldata = new PlaceHolder();
        tb.Visible = true;
        tb.SuggestTextBox = new TextBox();
        tb.SuggestTextBox.ID = "suggest" + id;
        tb.SuggestTextBox.EnableViewState = true;
        tb.SuggestTextBox.CssClass = "suggestTextBox";
        tb.SuggestTextBox.Visible = true;
        tb.Width = 260;
        tb.SuggestPhLocaldata.Controls.Add(tb.SuggestTextBox);
        tb.DataBind();

        if (processApply.Required.HasValue && processApply.Required.Value)
        {
            tb.CssClass = "inputRequired";
        }

        tc2.Controls.Add(tb);
    }

    private void NewTextBox(HtmlTableCell tc2, ProcessApply processApply, TextBox tb, string id)
    {
        tb.ID = "tb" + id;

        if (processApply.Required.HasValue && processApply.Required.Value)
        {
            tb.CssClass = "inputRequired";
        }

        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_TEXTAREA)
        {
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Height = 50;
            tb.Width = new Unit(77, UnitType.Percentage);
        }

        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATE)
        {
            tb.Attributes.Add("onclick", "WdatePicker({dateFmt:'yyyy-MM-dd'})");
        }
        else if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATETIME)
        {
            tb.Attributes.Add("onclick", "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})");
        }
        tc2.Controls.Add(tb);
    }

    private void Validator(HtmlTableCell tc1, HtmlTableCell tc2, ProcessApply processApply, string id)
    {
        Label lbl = new Label();
        if (!string.IsNullOrEmpty(processApply.Color))
        {
            lbl.Attributes.Add("style", "color: " + processApply.Color + ";");
        }
        lbl.ID = "lbl" + id;
        lbl.Text = processApply.GetDesc(this.CurrentUser.UserLanguage) + ":";
        tc1.Controls.Add(lbl);

        if (processApply.Required.HasValue && processApply.Required.Value)
        {
            RequiredFieldValidator rfv = new RequiredFieldValidator();
            rfv.ID = "rfv" + id;
            rfv.Display = ValidatorDisplay.Dynamic;
            rfv.ErrorMessage = "${Common.Business.Error.Required}";
            rfv.ControlToValidate = "tb" + id;
            rfv.ValidationGroup = "vgSave";

            tc2.Controls.Add(rfv);
        }

        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_QTY)
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

        if (!string.IsNullOrEmpty(processApply.UOM))
        {
            Literal lblDesc = new Literal();
            lblDesc.Text = processApply.UOMDesc(this.CurrentUser.UserLanguage);
            tc2.Controls.Add(lblDesc);
        }

        /*
        RangeValidator rv = new RangeValidator();
        rv.ID = "rv" + id;
        rv.Display = ValidatorDisplay.Dynamic;
        rv.ErrorMessage = "${Common.Validator.Valid.Number}";
        rv.MaximumValue = "999999999";
        rv.MinimumValue = "0.00000001";
        rv.Type = ValidationDataType.Double;
        rv.ControlToValidate = "tb" + id;
        rv.ValidationGroup = "vgSave";
        tc2.Controls.Add(rv);
        */
    }

    void Edit_Render(object sender, EventArgs e)
    {
        this.UpdateAmount(((object[])sender)[0].ToString());
        this.UpdateTaxes(((object[])sender)[1].ToString());
        this.UpdateTotalAmount(((object[])sender)[2].ToString());
        this.UpdateQty(((object[])sender)[3].ToString());
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbTaskSubType.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code;
        this.tbTaskSubType.DataBind();
        this.ucCostList.EditEvent += new System.EventHandler(this.Edit_Render);
        if (!IsPostBack)
        {
            this.lgd.InnerText = "${ISI.TSK.Add" + this.ModuleType + "}";
            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT
                    || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            {
                this.ltlTaskSubType.Text = "${ISI.TSK.TaskSubType." + this.ModuleType + "}:";
            }
            this.fsFlow.Visible = this.ModuleType != ISIConstants.CODE_PREFIX_ENGINEERING_WORKFLOW;
            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
            {
                this.isPrj.Visible = true;
                if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
                {
                    this.lblBackYards.Text = "${ISI.TSK.RefTask}:";
                    this.lblSubject.Text = "${ISI.TSK.PrjIss.Subject}:";
                    this.lblDesc1.Text = "${ISI.TSK.PrjIss.Desc1}:";
                    this.lblExpectedResults.Text = "${ISI.TSK.PrjIss.ExpectedResults}";
                    this.isIss.Visible = true;
                }
                this.ltlTaskSubType.Text = "${ISI.TSK.Project}:";
            }
            else
            {
                this.isPrj.Visible = false;
                if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_IMPROVE)
                {
                    this.isImp.Visible = true;
                }
                else if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_ISSUE)
                {
                    this.isIss.Visible = true;
                }
            }

            //if (this.ModuleType != ISIConstants.ISI_TASK_TYPE_WORKFLOW)
            {
                PageCleanup();
            }
        }
        else
        {
            //动态加载
            GenerateGynamic();
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
        tbTaskSubType.Text = string.Empty;
        CurrentTaskSubType = string.Empty;
        this.CurrentIsApply = false;
        PageCleanup1();
    }

    public void UpdateAmount(string amount)
    {
        this.tbAmount.Text = amount;
    }
    public void UpdateTotalAmount(string totalAmount)
    {
        this.tbTotalAmount.Text = totalAmount;
    }
    public void UpdateQty(string qty)
    {
        this.tbQty.Text = qty;
    }
    public void UpdateTaxes(string taxes)
    {
        this.tbTaxes.Text = taxes;
    }

    public void PageCleanup1()
    {
        this.trAccount.Visible = false;
        this.trFormType2.Visible = false;
        this.trQty.Visible = false;
        this.trWF.Visible = false;
        this.workHoursTR2.Visible = false;
        isImp.Visible = false;
        trAmount.Visible = false;

        this.ucCostList.PageCleanup();
        this.ucCostList.Visible = false;

        ProcessApplyList = null;
        tbSubject.Text = !string.IsNullOrEmpty(this.Subject) ? this.Subject : string.Empty;
        tbDesc1.Text = !string.IsNullOrEmpty(this.Desc1) ? this.Desc1 : string.Empty;

        tbTaskAddress.Text = !string.IsNullOrEmpty(this.TaskAddress) ? this.TaskAddress : string.Empty;
        tbFailureMode.Text = string.Empty;
        tbExtNo.Text = string.Empty;
        tbCostCenter.Text = this.CurrentUser.CostCenter;
        rtbCostCenter.Text = this.CurrentUser.CostCenter;
        //ltlCostCenter.Text = this.CurrentUser.CostCenter;
        tbSeq.Text = string.Empty;
        tbAmount.Text = string.Empty;
        tbAmount.CssClass = string.Empty;
        tbSupplier.Text = string.Empty;
        tbPayeeCode.Text = string.Empty;
        tbVoucher.Text = string.Empty;
        ddlPriority.SelectedIndex = 0;
        ddlTravelType.SelectedIndex = 0;
        ddlPhase.SelectedIndex = 0;
        tbAccount1.Text = string.Empty;
        this.tbAccount2.Text = string.Empty;
        this.tbQty.Text = string.Empty;
        this.tbTotalAmount.Text = string.Empty;
        this.tbVoucher.Text = string.Empty;
        //this.planAmountTR.Visible = false;
        //this.tbPlanAmount.Text = string.Empty;
        //this.workHoursTR.Visible = false;
        this.workHoursTR2.Visible = false;
        //this.tbPlanWorkHours.Text = string.Empty;
        this.tbExpectedResults.Text = !string.IsNullOrEmpty(this.ExpectedResults) ? this.ExpectedResults : string.Empty;
        this.tbBackYards.Text = !string.IsNullOrEmpty(this.BackYards) ? this.BackYards : string.Empty;
        this.tbAssignStartUser.Text = string.Empty;
        this.tbDesc2.Text = !string.IsNullOrEmpty(this.Desc2) ? this.Desc2 : string.Empty;
        this.tbPlanStartDate.Text = !string.IsNullOrEmpty(this.PlanStartDate) ? this.PlanStartDate : DateTime.Now.ToString("yyyy-MM-dd 08:00");
        this.tbPlanCompleteDate.Text = !string.IsNullOrEmpty(this.PlanCompleteDate) ? this.PlanCompleteDate : string.Empty;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (CreateEvent != null)
        {
            if (this.rfvTaskSubType.IsValid && this.rfvTaskAddress.IsValid)
            {
                TaskMstr task = new TaskMstr();

                task.TaskSubType = TheTaskSubTypeMgr.LoadTaskSubType(tbTaskSubType.Text.Trim());
                if (task.TaskSubType == null)
                {
                    ShowWarningMessage("ISI.Error.TaskCodeNotExist");
                    return;
                }

                if (task.TaskSubType.IsCostCenter && (!rfvAccount1.IsValid || !rfvAccount2.IsValid))
                {
                    return;
                }

                task.FormType = task.TaskSubType.FormType;
                if (ddlPriority.SelectedIndex != -1)
                {
                    task.Priority = ddlPriority.SelectedValue;
                }
                if (this.ddlTravelType.SelectedIndex != -1)
                {
                    task.TravelType = ddlTravelType.SelectedValue;
                }
                task.BackYards = this.tbBackYards.Text.Trim();
                task.Desc1 = tbDesc1.Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                task.Subject = tbSubject.Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());

                if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
                {
                    task.Phase = this.ddlPhase.SelectedValue;
                    task.Seq = this.tbSeq.Text;
                }

                //task.Phase = string.Empty;
                //task.Seq = string.Empty;
                if (!string.IsNullOrEmpty(this.tbSupplier.Text.Trim()))
                {
                    var supplier = this.TheSupplierMgr.LoadSupplier(tbSupplier.Text.Trim());
                    if (supplier != null)
                    {
                        task.SupplierCode = supplier.Code;
                        task.SupplierName = supplier.Name;
                    }
                }
                if (!string.IsNullOrEmpty(this.tbPayeeCode.Text.Trim()))
                {
                    var user = this.TheUserMgr.LoadUser(tbPayeeCode.Text.Trim());
                    task.PayeeCode = user.Code;
                    task.PayeeName = user.Name;
                }
                if (!string.IsNullOrEmpty(this.tbVoucher.Text.Trim()))
                {
                    task.Voucher = int.Parse(this.tbVoucher.Text.Trim());
                }
                if (!string.IsNullOrEmpty(this.tbQty.Text.Trim()))
                {
                    task.Qty = decimal.Parse(this.tbQty.Text.Trim());
                }
                if (!string.IsNullOrEmpty(this.tbAmount.Text.Trim()))
                {
                    task.Amount = decimal.Parse(this.tbAmount.Text.Trim());
                }
                if (!string.IsNullOrEmpty(this.tbTaxes.Text.Trim()))
                {
                    task.Taxes = decimal.Parse(this.tbTaxes.Text.Trim());
                }
                if (task.Amount.HasValue || task.Taxes.HasValue)
                {
                    task.TotalAmount = (task.Amount.HasValue ? task.Amount.Value : 0) + (task.Taxes.HasValue ? task.Taxes.Value : 0);
                }
                else if (!string.IsNullOrEmpty(this.tbTotalAmount.Text.Trim()))
                {
                    task.TotalAmount = decimal.Parse(this.tbTotalAmount.Text.Trim());
                }
                else
                {
                    task.TotalAmount = null;
                }

                if (!string.IsNullOrEmpty(tbFailureMode.Text.Trim()))
                {
                    //FailureMode failureMode = new FailureMode();
                    //failureMode.Code = tbFailureMode.Text.Trim();
                    task.FailureMode = this.TheFailureModeMgr.LoadFailureMode(tbFailureMode.Text.Trim());
                }

                TaskSubType costCenter = null;
                if (!string.IsNullOrEmpty(this.tbCostCenter.Text.Trim()))
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
                task.Dept = this.CurrentUser.CostCenter;

                if (!string.IsNullOrEmpty(this.tbAccount1.Text.Trim()))
                {
                    var account1 = this.TheAccountMgr.LoadAccount(tbAccount1.Text.Trim());
                    task.Account1 = account1.Code;
                    task.Account1Desc = account1.Desc1;
                }
                else
                {
                    task.Account1 = null;
                    task.Account1Desc = null;
                }
                if (!string.IsNullOrEmpty(this.tbAccount2.Text.Trim()))
                {
                    var account2 = this.TheAccountMgr.LoadAccount(tbAccount2.Text.Trim());
                    task.Account2 = account2.Code;
                    task.Account2Desc = account2.Desc1;
                }
                else
                {
                    task.Account2 = null;
                    task.Account2Desc = null;
                }
                if (task.TaskSubType.IsApply && this.ProcessApplyList != null && this.ProcessApplyList != null)
                {
                    IList<TaskApply> taskApplyList = new List<TaskApply>();

                    for (int i = 0; i < ProcessApplyList.Count; i++)
                    {
                        var processApply = ProcessApplyList[i];
                        string id = processApply.Apply + processApply.UOM + processApply.Seq + processApply.Id;
                        if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_CHECKBOX)
                        {
                            CheckBoxList cbListProcessApply = (CheckBoxList)this.FindControl("cb" + id);
                            for (int j = 0; j < cbListProcessApply.Items.Count; j++)
                            {
                                processApply = ProcessApplyList[i + j];
                                ListItem li = cbListProcessApply.Items[j];
                                TaskApply taskApply = new TaskApply();
                                CloneHelper.CopyProperty(processApply, taskApply);
                                taskApply.TaskSubType = task.TaskSubType.Code;
                                if (li.Selected)
                                {
                                    taskApply.Checked = true;
                                }
                                taskApplyList.Add(taskApply);
                            }

                            i += cbListProcessApply.Items.Count - 1;
                        }
                        else if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_RADIO)
                        {
                            RadioButtonList rbListProcessApply = (RadioButtonList)this.FindControl("rb" + id);
                            for (int j = 0; j < rbListProcessApply.Items.Count; j++)
                            {
                                processApply = ProcessApplyList[i + j];
                                ListItem li = rbListProcessApply.Items[j];
                                TaskApply taskApply = new TaskApply();
                                CloneHelper.CopyProperty(processApply, taskApply);
                                taskApply.TaskSubType = task.TaskSubType.Code;
                                if (li.Selected)
                                {
                                    taskApply.Checked = true;
                                }
                                taskApplyList.Add(taskApply);
                            }
                            i += rbListProcessApply.Items.Count - 1;
                        }
                        else
                        {
                            TaskApply taskApply = new TaskApply();
                            CloneHelper.CopyProperty(processApply, taskApply);
                            taskApply.TaskSubType = task.TaskSubType.Code;

                            if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_BLANK || processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_LABEL)
                            {
                                taskApplyList.Add(taskApply);
                            }
                            else
                            {
                                string tbId = "tb" + id;
                                if (processApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_TEXTBOX && !string.IsNullOrEmpty(processApply.ServicePath)
                                        && !string.IsNullOrEmpty(processApply.ServiceMethod) && !string.IsNullOrEmpty(processApply.DescField) && !string.IsNullOrEmpty(processApply.ValueField))
                                {
                                    Controls_TextBox tb = (Controls_TextBox)this.FindControl(tbId);
                                    if (tb.Text.Trim() != string.Empty)
                                    {
                                        taskApply.Value = tb.Text.Trim();
                                    }
                                }
                                else
                                {
                                    TextBox tb = (TextBox)this.FindControl(tbId);
                                    if (tb.Text.Trim() != string.Empty)
                                    {
                                        if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_QTY)
                                        {
                                            taskApply.Qty = decimal.Parse(tb.Text.Trim());
                                        }
                                        else if (taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATE || taskApply.Type == ISIConstants.CODE_MASTER_WFS_TYPE_DATETIME)
                                        {
                                            taskApply.DateValue = DateTime.Parse(tb.Text.Trim());
                                        }
                                        else
                                        {
                                            taskApply.Value = tb.Text.Trim();
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

                task.UserName = this.CurrentUser.Name;
                task.Email = ISIUtil.IsValidEmail(this.CurrentUser.Email) ? this.CurrentUser.Email : string.Empty;
                task.MobilePhone = ISIUtil.IsValidMobilePhone(this.CurrentUser.MobliePhone) ? this.CurrentUser.MobliePhone : string.Empty;

                task.TaskAddress = tbTaskAddress.Text.Trim();

                task.Type = this.ModuleType;
                task.IsAutoRelease = cbIsAutoRelease.Visible && this.cbIsAutoRelease.Checked;

                //跟踪信息
                task.ExpectedResults = tbExpectedResults.Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
                task.Desc2 = tbDesc2.Text.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());

                string startTime = tbPlanStartDate.Text.Trim();
                string endTime = tbPlanCompleteDate.Text.Trim();

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
                        return;
                    }
                }

                string assignStartUser = tbAssignStartUser.Text.Trim();

                //验证有效性
                if (!string.IsNullOrEmpty(assignStartUser))
                {
                    string[] userCodeName = this.TheTaskMgr.GetUserCodeName(assignStartUser);

                    string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                    if (!string.IsNullOrEmpty(invalidUser))
                    {
                        ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
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

                string workHoursUser = tbWorkHoursUser.Text.Trim();

                //验证有效性
                if (!string.IsNullOrEmpty(workHoursUser))
                {
                    string[] userCodeName = this.TheTaskMgr.GetUserCodeName(workHoursUser);

                    string invalidUser = TheTaskMgr.GetInvalidUser(userCodeName[0], this.CurrentUser.Code);
                    if (!string.IsNullOrEmpty(invalidUser))
                    {
                        ShowWarningMessage("ISI.Error.UserNotExist", new string[] { invalidUser });
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
                }

                if (this.ucCostList.Visible)
                {
                    if (this.ucCostList.TheCostList == null || this.ucCostList.TheCostList.Count == 0)
                    {
                        this.ShowErrorMessage("WFS.Cost.Warn.DetailEmpty");
                        return;
                    }
                }
                TheTaskMgr.CreateTask(task, this.ucCostList.TheCostList, this.CurrentUser);

                CreateEvent(task.Code, e);

                ShowSuccessMessage("ISI.TSK.Add" + this.ModuleType + ".Successfully", task.Code);
            }
        }
    }
}
