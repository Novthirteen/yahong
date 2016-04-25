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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Web;
using com.Sconit.Control;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using com.Sconit.Entity;

public partial class ISI_Summary_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;
    public string CurrentRefCode
    {
        get
        {
            return (string)ViewState["CurrentRefCode"];
        }
        set
        {
            ViewState["CurrentRefCode"] = value;
        }
    }
    public Summary Summary
    {
        get
        {
            return ViewState["Summary"] == null ? null : (Summary)ViewState["Summary"];
        }
        set
        {
            ViewState["Summary"] = value;
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

    public void InitPageParameter()
    {
        if (Summary != null)
        {
            this.tbStandardQty.Text = Summary.StandardQty.ToString();
            this.cbIsCheckup.Checked = Summary.IsCheckup;
            this.tbUserCode.Text = this.CurrentUser.LongCodeName;

            if (SummaryDetList == null)
            {
                SummaryDetList = new List<SummaryDet>();
            }

            SummaryDet summaryDet = new SummaryDet();
            summaryDet.IsBlankDetail = true;
            for (int i = 0; i < 29; i++)
            {
                SummaryDetList.Add(summaryDet);
            }
            summaryDet = new SummaryDet();
            summaryDet.IsBlankDetail = true;
            summaryDet.IsLast = true;
            SummaryDetList.Add(summaryDet);
            this.GV_List.DataSource = this.SummaryDetList;
            this.GV_List.DataBind();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (CreateEvent != null && this.cvInsert.IsValid)
        {
            SummaryDetList = this.PopulateSelectedData();
            if (SummaryDetList != null && SummaryDetList.Count > 0)
            {
                Summary.Date = DateTime.Parse(this.tbDate.Text.Trim());
                Summary.IsAutoRelease = cbIsAutoRelease.Checked;
                Summary.Desc = this.tbDesc.Text.Trim();
                Summary.RefCode = this.tbRefCode.Text.Trim();
                this.TheSummaryMgr.CreateSummary(Summary, SummaryDetList, this.CurrentUser);
                CreateEvent(Summary.Code, e);
                ShowSuccessMessage("ISI.Summary.AddSummary.Successfully", Summary.Code);
            }
            else
            {
                this.ShowWarningMessage("Common.Business.Warn.DetailEmpty");
            }
        }
    }

    public IList<SummaryDet> PopulateSelectedData()
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            IList<SummaryDet> summaryDetList = new List<SummaryDet>();
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                CheckBox cbChecked = (CheckBox)row.FindControl("cbChecked");
                if (cbChecked.Checked)
                {
                    TextBox tbConment = (TextBox)row.FindControl("tbConment");
                    TextBox tbSubject = (TextBox)row.FindControl("tbSubject");
                    TextBox tbSeq = (TextBox)row.FindControl("tbSeq");

                    if (tbSubject.Text.Trim() != string.Empty || tbConment.Text.Trim() != string.Empty)
                    {
                        TextBox tbTaskCode = (TextBox)row.FindControl("tbTaskCode");

                        SummaryDet summaryDet = new SummaryDet();

                        summaryDet.Subject = tbSubject.Text.Trim();
                        summaryDet.Conment = tbConment.Text.Trim();
                        summaryDet.TaskCode = tbTaskCode.Text.Trim();
                        summaryDet.Seq = int.Parse(tbSeq.Text.Trim());
                        summaryDetList.Add(summaryDet);
                    }
                }
            }
            return summaryDetList;
        }

        return null;
    }

    protected void tbRefCode_TextChanged(Object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(this.tbRefCode.Text.Trim()) && (string.IsNullOrEmpty(this.CurrentRefCode) || this.CurrentRefCode != this.tbRefCode.Text))
            {
                this.CurrentRefCode = this.tbRefCode.Text.Trim();
                if (!string.IsNullOrEmpty(CurrentRefCode))
                {
                    SummaryDetList = this.TheSummaryDetMgr.GetSummaryDet(CurrentRefCode, this.CurrentUser.Code);

                    //SummaryDetList = this.TheSummaryMgr.GetSummaryDet(this.CurrentUser.Code, DateTime.Now.AddMonths(-2).ToString("yyyy-MM-01"));
                    if (SummaryDetList == null)
                    {
                        SummaryDetList = new List<SummaryDet>();
                    }
                    SummaryDet summaryDet = new SummaryDet();
                    summaryDet.IsBlankDetail = true;

                    if (SummaryDetList.Count != 0)
                    {
                        lgd.InnerText = "${ISI.Summary.PreDetail}";
                        for (int i = 0; i < 20; i++)
                        {
                            SummaryDetList.Add(summaryDet);
                        }
                    }
                    else
                    {
                        this.tbRefCode.Text = string.Empty;
                        int rows = this.CurrentUser.Code == "liudan" ? 80 : 30;
                        for (int i = 0; i < rows; i++)
                        {
                            SummaryDetList.Add(summaryDet);
                        }
                    }

                    this.GV_List.DataSource = this.SummaryDetList;
                    this.GV_List.DataBind();
                }
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        tbRefCode.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:#tbDate";

        if (!IsPostBack)
        {
            this.tbDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
            Summary = this.TheSummaryMgr.TransferEvaluation2Summary(this.CurrentUser);

            this.tbStandardQty.Text = Summary.StandardQty.ToString();
            this.cbIsCheckup.Checked = Summary.IsCheckup;
            this.tbUserCode.Text = this.CurrentUser.LongCodeName;

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
            SummaryDet summaryDet = (SummaryDet)e.Row.DataItem;
            if (this.CurrentUser.UserLanguage == BusinessConstants.CODE_MASTER_LANGUAGE_VALUE_ZH_CN)
            {
                ((CheckBox)e.Row.FindControl("cbChecked")).Text = StringHelper.NumberCn(e.Row.DataItemIndex + 1);
            }
            e.Row.FindControl("descImg").Visible = e.Row.DataItemIndex % 2 != 0;
            if (summaryDet.IsLast)
            {
                e.Row.FindControl("btnSave").Visible = summaryDet.IsLast;
                ((TextBox)e.Row.FindControl("tbConment")).Width = new Unit(100, UnitType.Percentage);
            }
        }
    }

    public void PageCleanup()
    {
        tbUserCode.Text = string.Empty;
        tbDate.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        tbStandardQty.Text = string.Empty;
        cbIsCheckup.Checked = true;
        SummaryDetList = new List<SummaryDet>();
        cbIsAutoRelease.Checked = false;
    }

    protected void checkSummaryDate(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvInsert":
                var summary = TheSummaryMgr.LoadSummary(this.CurrentUser.Code, args.Value);
                if (summary != null)
                {
                    //ShowErrorMessage("ISI.Summary.DateExist", args.Value, summary.Code, summary.Status);
                    cv.ErrorMessage = "${ISI.Summary.DateExist" + "," + summary.Code + "," + summary.Status + "}";
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }
}
