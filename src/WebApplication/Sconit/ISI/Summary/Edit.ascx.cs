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
using com.Sconit.Utility;
using System.Text;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;

public partial class ISI_Summary_Edit : EditModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler UpdateTitleEvent;
    public string SummaryCode
    {
        get
        {

            return (string)ViewState["SummaryCode"];

        }
        set
        {
            ViewState["SummaryCode"] = value;
        }
    }

    public Summary Summary
    {
        get
        {

            return (Summary)ViewState["Summary"];

        }
        set
        {
            ViewState["Summary"] = value;
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

    public IList<SummaryDet> SummaryDetList
    {
        get
        {
            return ViewState["SummaryDetList"] == null ? null : (IList<SummaryDet>)ViewState["SummaryDetList"];
        }
        set
        {
            ViewState["SummaryDetList"] = value;
        }
    }

    public IList<CodeMaster> TypeList
    {
        get
        {
            return ViewState["TypeList"] == null ? null : (IList<CodeMaster>)ViewState["TypeList"];
        }
        set
        {
            ViewState["TypeList"] = value;
        }
    }
    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            SummaryDet summaryDet = (SummaryDet)e.Row.DataItem;

            if (this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE && !summaryDet.IsBlankDetail && Summary.CreateUser == this.CurrentUser.Code)
            {
                e.Row.FindControl("lbtnDelete").Visible = true;

            }

            if ((this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE || this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT)
                    && summaryDet.IsLast && summaryDet.IsBlankDetail && Summary.CreateUser == this.CurrentUser.Code)
            {
                e.Row.FindControl("btnSave").Visible = summaryDet.IsLast;
                ((TextBox)e.Row.FindControl("tbConment")).Width = new Unit(100, UnitType.Percentage);
                ((TextBox)e.Row.FindControl("tbApproveDesc")).Width = new Unit(100, UnitType.Percentage);
            }

            e.Row.FindControl("approveDescTr").Visible = (this.Summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE && this.Summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN);
            if (!e.Row.FindControl("approveDescTr").Visible)
            {
                e.Row.FindControl("descImg").Visible = e.Row.DataItemIndex % 2 != 0;
            }
            else
            {
                e.Row.FindControl("descImg").Visible = false;
                e.Row.FindControl("approveImg").Visible = e.Row.DataItemIndex % 2 != 0;
            }
            ((TextBox)e.Row.FindControl("tbConment")).ReadOnly = !((Summary.CreateUser == this.CurrentUser.Code || Summary.SubmitUser == this.CurrentUser.Code) && (this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE || this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT));
            ((TextBox)e.Row.FindControl("tbTaskCode")).ReadOnly = !((Summary.CreateUser == this.CurrentUser.Code || Summary.SubmitUser == this.CurrentUser.Code) && (this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE || this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT));
            ((TextBox)e.Row.FindControl("tbSubject")).ReadOnly = !((Summary.CreateUser == this.CurrentUser.Code || Summary.SubmitUser == this.CurrentUser.Code) && (this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE || this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT));
            ((CheckBox)e.Row.FindControl("cbChecked")).Visible = ((Summary.CreateUser == this.CurrentUser.Code || Summary.SubmitUser == this.CurrentUser.Code) && (this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE || this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT));
            e.Row.FindControl("lblSeq").Visible = !((CheckBox)e.Row.FindControl("cbChecked")).Visible;

            if (this.CurrentUser.UserLanguage == BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN)
            {
                ((CheckBox)e.Row.FindControl("cbChecked")).Text = StringHelper.NumberCn(e.Row.DataItemIndex + 1);
                ((Label)e.Row.FindControl("lblSeq")).Text = StringHelper.NumberCn(e.Row.DataItemIndex + 1);
            }

            if (this.Summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE)
            {
                TextBox tbApproveDesc = ((TextBox)e.Row.FindControl("tbApproveDesc"));

                tbApproveDesc.ReadOnly = this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL || this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CLOSE || this.Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_APPROVAL || !(this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN));
                if (!string.IsNullOrEmpty(summaryDet.ApproveDesc))
                {
                    tbApproveDesc.Text = summaryDet.ApproveDesc;
                }

                RadioButtonList rblType = (RadioButtonList)e.Row.FindControl("rblType");
                rblType.Enabled = this.Summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL && this.Summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CLOSE && this.Summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_APPROVAL;
                rblType.Attributes.Add("onchange", "GetCount()");
                rblType.DataSource = TypeList;
                rblType.DataBind();

                if (!string.IsNullOrEmpty(summaryDet.Type))
                {
                    for (int ii = 0; ii < rblType.Items.Count; ii++)
                    {
                        if (!string.IsNullOrEmpty(summaryDet.Type) && rblType.Items[ii].Value == summaryDet.Type)
                        {
                            rblType.Items[ii].Selected = true;
                        }
                    }
                }
            }
        }
    }
    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (this.Summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE)
        {
            this.GV_List.Columns[this.GV_List.Columns.Count - 1].Visible = false;
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        int id = int.Parse(((System.Web.UI.WebControls.LinkButton)sender).CommandArgument);
        try
        {
            this.TheSummaryDetMgr.DeleteSummaryDet(id);
            this.ShowSuccessMessage("ISI.Summary.DeleteSummaryDet.Successfully");
            InitPageParameter(this.SummaryCode);
            //this.GV_List_ProcessDefinition.DataBind();
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private void UpdateView(Summary summary)
    {
        ((Literal)(this.FV_Summary.FindControl("lblStatus"))).Text = "${ISI.Status." + summary.Status + "}";
        //((Literal)(this.FV_Summary.FindControl("lblStandardQty"))).Text = "${ISI.Summary.StandardQty}:" + summary.StandardQty.ToString();

        Summary = summary;

        this.TypeList = TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_SUMMARY_TYPE);
        SummaryDetList = this.TheSummaryDetMgr.GetSummaryDet(summary.Code);

        CheckBox cbIsCheckup = (CheckBox)this.FV_Summary.FindControl("cbIsCheckup");
        cbIsCheckup.Enabled = false;
        this.FV_Summary.FindControl("btnSave").Visible = false;
        this.FV_Summary.FindControl("btnCancel").Visible = false;
        this.FV_Summary.FindControl("btnSubmit").Visible = false;
        this.FV_Summary.FindControl("btnApprove").Visible = false;
        this.FV_Summary.FindControl("btnApprove2").Visible = false;
        this.FV_Summary.FindControl("btnClose").Visible = false;
        this.FV_Summary.FindControl("approveDescTr").Visible = false;

        if (Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE)
        {
            if (this.CurrentUser.Code == Summary.CreateUser || this.CurrentUser.Code == Summary.SubmitUser || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN))
            {
                SummaryDet summaryDet = new SummaryDet();
                summaryDet.IsBlankDetail = true;
                for (int i = 0; i < 19; i++)
                {
                    SummaryDetList.Add(summaryDet);
                }
                summaryDet = new SummaryDet();
                summaryDet.IsBlankDetail = true;
                summaryDet.IsLast = true;
                SummaryDetList.Add(summaryDet);

                this.FV_Summary.FindControl("btnSave").Visible = true;
                this.FV_Summary.FindControl("btnCancel").Visible = true;
                this.FV_Summary.FindControl("btnSubmit").Visible = true;
                ((TextBox)this.FV_Summary.FindControl("tbDesc")).ReadOnly = false;
            }
        }

        if (Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT)
        {
            if (this.CurrentUser.Code == Summary.CreateUser || this.CurrentUser.Code == Summary.SubmitUser || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN))
            {
                SummaryDet summaryDet = new SummaryDet();
                summaryDet.IsBlankDetail = true;
                for (int i = 0; i < 19; i++)
                {
                    SummaryDetList.Add(summaryDet);
                }
                summaryDet = new SummaryDet();
                summaryDet.IsBlankDetail = true;
                summaryDet.IsLast = true;
                SummaryDetList.Add(summaryDet);

                this.FV_Summary.FindControl("btnSave").Visible = true;
                this.FV_Summary.FindControl("btnCancel").Visible = true;

                ((TextBox)this.FV_Summary.FindControl("tbDesc")).ReadOnly = false;
            }

            if (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
            {
                this.FV_Summary.FindControl("btnSave").Visible = true;
                this.FV_Summary.FindControl("approveDescTr").Visible = true;
                this.FV_Summary.FindControl("btnCancel").Visible = true;
                this.FV_Summary.FindControl("btnApprove").Visible = true;
                this.FV_Summary.FindControl("btnApprove2").Visible = true;

                ((TextBox)this.FV_Summary.FindControl("tbQty")).ReadOnly = false;
                ((TextBox)this.FV_Summary.FindControl("tbApproveDesc")).ReadOnly = false;
                cbIsCheckup.Enabled = true;
            }
        }

        this.GV_List.DataSource = SummaryDetList;
        this.GV_List.DataBind();

        if (Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_INAPPROVE)
        {
            this.FV_Summary.FindControl("approveDescTr").Visible = true;

            if (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
            {
                this.FV_Summary.FindControl("btnSave").Visible = true;
                this.FV_Summary.FindControl("btnCancel").Visible = true;
                this.FV_Summary.FindControl("btnApprove").Visible = true;
                this.FV_Summary.FindControl("btnApprove2").Visible = true;
                this.FV_Summary.FindControl("approveDescTr").Visible = true;
                ((TextBox)this.FV_Summary.FindControl("tbQty")).ReadOnly = false;
                ((TextBox)this.FV_Summary.FindControl("tbApproveDesc")).ReadOnly = false;
                cbIsCheckup.Enabled = true;
            }
        }

        if (Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_APPROVAL && (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE)))
        {
            this.FV_Summary.FindControl("approveDescTr").Visible = true;
            this.FV_Summary.FindControl("btnClose").Visible = true;
            ((TextBox)this.FV_Summary.FindControl("tbApproveDesc")).ReadOnly = true;
        }

        if (Summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CLOSE)
        {
            this.FV_Summary.FindControl("approveDescTr").Visible = true;
            this.FV_Summary.FindControl("ultimatelyDescTr1").Visible = true;
            this.FV_Summary.FindControl("ultimatelyDescTr2").Visible = true;
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        try
        {
            this.TheSummaryMgr.CloseSummary(this.SummaryCode, this.CurrentUser);
            this.InitPageParameter(this.SummaryCode);
            ShowSuccessMessage("ISI.Summary.Close.Successfully", this.SummaryCode);
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

    private void PageCleanup()
    {
        this.SummaryCode = null;
        FileExtensions = null;
        ContentLength = 0;
        this.SummaryDetList = null;

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            FileExtensions = this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_FILEEXTENSION).Value;
            ContentLength = int.Parse(this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_CONTENTLENGTH).Value);
        }
    }

    public void InitPageParameter(string code)
    {
        this.SummaryCode = code;
        this.ODS_Summary.SelectParameters["Code"].DefaultValue = this.SummaryCode;
        this.FV_Summary.DataBind();
    }


    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        this.TheSummaryMgr.CancelSummary(this.SummaryCode, this.CurrentUser);
        this.InitPageParameter(Summary.Code);
        if (this.UpdateTitleEvent != null)
        {
            this.UpdateTitleEvent(sender, e);
        }
        ShowSuccessMessage("ISI.Summary.CancelSummary.Successfully", Summary.Code);
    }
    protected void btnApprove2_Click(object sender, EventArgs e)
    {
        Approve(true, sender, e);
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        Approve(false, sender, e);
    }

    private void Approve(bool isNext, object sender, EventArgs e)
    {
        try
        {
            SummaryDetList = this.PopulateSelectedData(true);
            if (SummaryDetList != null && SummaryDetList.Count > 0)
            {
                Summary.ApproveDesc = ((TextBox)this.FV_Summary.FindControl("tbApproveDesc")).Text.Trim();
                string qty = ((TextBox)this.FV_Summary.FindControl("tbQty")).Text.Trim();
                Summary.Qty = !string.IsNullOrEmpty(qty) ? int.Parse(qty) : 0;
                Summary.IsCheckup = ((CheckBox)this.FV_Summary.FindControl("cbIsCheckup")).Checked;
                var code = this.SummaryCode;
                if (isNext)
                {
                    this.SummaryCode = this.TheSummaryMgr.ApproveSummary2(Summary, SummaryDetList, this.CurrentUser);
                }
                else
                {
                    this.TheSummaryMgr.ApproveSummary(Summary, SummaryDetList, this.CurrentUser);
                }
                if (!string.IsNullOrEmpty(this.SummaryCode))
                {
                    this.InitPageParameter(SummaryCode);

                    if (this.UpdateTitleEvent != null)
                    {
                        this.UpdateTitleEvent(sender, e);
                    }
                }
                else
                {
                    //this.InitPageParameter(Summary.Code);
                    if (BackEvent != null)
                    {
                        BackEvent(this, e);
                    }
                }

                ShowSuccessMessage("ISI.Summary.ApproveSummary.Successfully", code);
            }
            else
            {
                this.ShowWarningMessage("Common.Business.Warn.DetailEmpty");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        SummaryDetList = this.PopulateSelectedData();
        if (SummaryDetList != null && SummaryDetList.Count > 0)
        {
            Summary.Desc = ((TextBox)this.FV_Summary.FindControl("tbDesc")).Text;
            this.TheSummaryMgr.SubmitSummary(Summary, SummaryDetList, this.CurrentUser);
            this.InitPageParameter(Summary.Code);
            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
            ShowSuccessMessage("ISI.Summary.SubmitSummary.Successfully", Summary.Code);
        }
        else
        {
            this.ShowWarningMessage("Common.Business.Warn.DetailEmpty");
        }

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        SummaryDetList = this.PopulateSelectedData(this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE));
        if (SummaryDetList != null && SummaryDetList.Count > 0)
        {
            Summary.Desc = ((TextBox)this.FV_Summary.FindControl("tbDesc")).Text;
            if (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
            {
                Summary.ApproveDesc = ((TextBox)this.FV_Summary.FindControl("tbApproveDesc")).Text.Trim();
                string qty = ((TextBox)this.FV_Summary.FindControl("tbQty")).Text.Trim();
                Summary.Qty = !string.IsNullOrEmpty(qty) ? int.Parse(qty) : 0;
                Summary.IsCheckup = ((CheckBox)this.FV_Summary.FindControl("cbIsCheckup")).Checked;
            }
            this.TheSummaryMgr.UpdateSummary(Summary, SummaryDetList, this.CurrentUser);
            this.InitPageParameter(Summary.Code);
            if (this.UpdateTitleEvent != null)
            {
                this.UpdateTitleEvent(sender, e);
            }
            ShowSuccessMessage("ISI.Summary.UpdateSummary.Successfully", Summary.Code);
        }
        else
        {
            this.ShowWarningMessage("Common.Business.Warn.DetailEmpty");
        }

    }
    public IList<SummaryDet> PopulateSelectedData()
    {
        return PopulateSelectedData(false);

    }
    public IList<SummaryDet> PopulateSelectedData(bool isApprove)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            IList<SummaryDet> summaryDetList = new List<SummaryDet>();
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                CheckBox cbChecked = (CheckBox)row.FindControl("cbChecked");

                if (isApprove || cbChecked.Checked)
                {
                    HiddenField hfId = row.FindControl("hfId") as HiddenField;

                    TextBox tbConment = (TextBox)row.FindControl("tbConment");
                    TextBox tbSubject = (TextBox)row.FindControl("tbSubject");
                    SummaryDet summaryDet = new SummaryDet();
                    if (!string.IsNullOrEmpty(hfId.Value))
                    {
                        summaryDet.Id = int.Parse(hfId.Value);
                    }
                    summaryDet.SummaryCode = this.SummaryCode;
                    if (tbSubject.Text.Trim() != string.Empty || tbConment.Text.Trim() != string.Empty)
                    {
                        TextBox tbTaskCode = (TextBox)row.FindControl("tbTaskCode");

                        summaryDet.Subject = tbSubject.Text.Trim();
                        summaryDet.Conment = tbConment.Text.Trim();
                        summaryDet.TaskCode = tbTaskCode.Text.Trim();

                        //if (isApprove || this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP))
                        {
                            TextBox tbApproveDesc = (TextBox)row.FindControl("tbApproveDesc");
                            RadioButtonList rblType = (RadioButtonList)row.FindControl("rblType");
                            summaryDet.ApproveDesc = tbApproveDesc.Text.Trim();
                            //summaryDet.Type = !string.IsNullOrEmpty(rblType.SelectedValue) ? rblType.SelectedValue : ISIConstants.CODE_MASTER_SUMMARY_TYPE_MODERATE;
                            summaryDet.Type = !string.IsNullOrEmpty(rblType.SelectedValue) ? rblType.SelectedValue : null;
                        }

                        summaryDetList.Add(summaryDet);
                    }
                }

            }
            return summaryDetList;
        }

        return null;
    }

    protected void FV_Summary_DataBound(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.SummaryCode))
        {
            Summary summary = (Summary)((FormView)sender).DataItem;

            UpdateView(summary);
        }
    }

}
