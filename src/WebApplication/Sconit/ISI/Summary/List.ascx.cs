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
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;


public partial class ISI_Summary_List : ListModuleBase
{
    public EventHandler EditEvent;

    public bool IsNoSubmit
    {
        get
        {
            return ViewState["IsNoSubmit"] == null ? false : (bool)ViewState["IsNoSubmit"];
        }
        set
        {
            ViewState["IsNoSubmit"] = value;
        }
    }

    public string SummaryDate
    {
        get
        {
            return (string)ViewState["SummaryDate"];
        }
        set
        {
            ViewState["SummaryDate"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (this.IsNoSubmit && !string.IsNullOrEmpty(SummaryDate))
        {
            IList<Evaluation> evaluationList = this.TheSummaryMgr.GetNoSummary(SummaryDate, this.CurrentUser);
            GV_Evaluation.DataSource = evaluationList;
            GV_Evaluation.DataBind();
            GV_Evaluation.Visible = true;
            fds.Visible = true;
            if (evaluationList == null || evaluationList.Count == 0)
            {
                this.lblMessage.Text = "${ISI.Summary.GridView.NoRecordFound}";
            }
            else
            {
                this.lblMessage.Text = string.Empty;
            }
        }
        else
        {
            GV_Evaluation.Visible = false;
            fds.Visible = false;
            this.lblMessage.Text = string.Empty;
        }
    }
    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Summary summary = (Summary)e.Row.DataItem;
            ((Label)e.Row.FindControl("lblStatus")).Text = "${ISI.Status." + summary.Status + "}";
            e.Row.Cells[2].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[9].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            if ((summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE || summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT
                            || summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_INAPPROVE)
                        && (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) ||
                            this.CurrentUser.Code == summary.CreateUser || this.CurrentUser.Code == summary.SubmitUser))
            {
                var lbtnCancel = e.Row.FindControl("lbtnCancel");
                lbtnCancel.Visible = true;
            }
        }
    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }

    protected void lbtnCancel_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            TheSummaryMgr.CancelSummary(code, this.CurrentUser);
            ShowSuccessMessage("ISI.Summary.DeleteSummary.Successfully", code);
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.Summary.DeleteSummary.Fail", code);
        }
    }
}
