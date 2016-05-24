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
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;


public partial class ISI_Checkup_List : ListModuleBase
{
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

    public EventHandler EditEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

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

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            this.TheCheckupMgr.DeleteCheckup(int.Parse(code));
            ShowSuccessMessage("ISI.FailureMode.DeleteFailureMode.Successfully");
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.FailureMode.DeleteFailureMode.Fail");
        }

    }


    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        GV_MergeTableCell(GV_List, new int[] { 0 }, false);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[5].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[6].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            Checkup checkup = (Checkup)e.Row.DataItem;
            Label lblStatus = ((Label)(e.Row.FindControl("lblStatus")));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage("ISI.Status." + checkup.Status, this.CurrentUser);

            if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE
                    && (
                           this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP)
                        || this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_CLOSECHECKUP)
                        || checkup.CreateUser == this.CurrentUser.Code
                        )
                )
            {
                LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
                if (lbtnDelete != null)
                {
                    lbtnDelete.Visible = true;
                }
            }
            e.Row.Cells[5].VerticalAlign = VerticalAlign.Top;
            e.Row.Cells[6].VerticalAlign = VerticalAlign.Top;

            if (checkup.SubmitDate.HasValue)
            {
                ((Label)(e.Row.FindControl("lblContent"))).Text = "<span style='color:#0000E5;'>" + checkup.SubmitUserNm + "(" + checkup.SubmitDate.Value.ToString("yyyy-MM-dd HH:mm") + ")</span>: " + checkup.Content;
            }
            else
            {
                ((Label)(e.Row.FindControl("lblContent"))).Text = "<span style='color:#0000E5;'>" + checkup.CreateUserNm + "(" + checkup.CreateDate.ToString("yyyy-MM-dd HH:mm") + ")</span>: " + checkup.Content;
            }
            if (checkup.ApprovalDate.HasValue)
            {
                ((Label)(e.Row.FindControl("lblAuditInstructions"))).Text = "<span style='color:#0000E5;'>" + checkup.ApprovalUserNm + "(" + checkup.ApprovalDate.Value.ToString("yyyy-MM-dd HH:mm") + ")</span>: " + checkup.AuditInstructions;
            }

            if (checkup.Amount.HasValue)
            {
                if (checkup.Amount.Value > 0)
                {
                    e.Row.Cells[4].Text = "+" + checkup.Amount.Value.ToString("0.##") + "&nbsp;奖";
                }
                else if (checkup.Amount.Value == 0)
                {
                    e.Row.Cells[4].Text = string.Empty;
                }
                else
                {
                    e.Row.Cells[4].Text = checkup.Amount.Value.ToString("0.##") + "&nbsp;罚";
                }
            }

        }
    }


    public static void GV_MergeTableCell(GridView GV, int[] colIndex, bool switchStyle)
    {
        if (GV == null || GV.Rows.Count == 0)
            return;

        foreach (int k in colIndex)
        {
            bool GVAlternatingRow = false;
            TableCell oldTc = GV.Rows[0].Cells[k];
            for (int i = 1; i < GV.Rows.Count; i++)
            {
                TableCell tc = GV.Rows[i].Cells[k];
                if (oldTc.Text != "&nbsp;" && tc.Text != "&nbsp;"
                    && oldTc.Text.Trim() == tc.Text.Trim()
                    && (tc.FindControl("lbtnEdit") == null || oldTc.FindControl("lbtnEdit") == null || ((LinkButton)(tc.FindControl("lbtnEdit"))).Text == ((LinkButton)(oldTc.FindControl("lbtnEdit"))).Text))
                {
                    tc.Visible = false;
                    if (oldTc.RowSpan == 0)
                    {
                        oldTc.RowSpan = 1;
                    }
                    oldTc.VerticalAlign = VerticalAlign.Top;
                    oldTc.HorizontalAlign = HorizontalAlign.Center;
                    oldTc.RowSpan++;
                }
                else
                {
                    oldTc = tc;

                    #region 切换Style
                    GVAlternatingRow = !GVAlternatingRow;
                    if (switchStyle)
                    {
                        if (GVAlternatingRow)
                            GV.Rows[i].Cells[k].Attributes.Add("class", "GVAlternatingRow");
                        else
                            GV.Rows[i].Cells[k].Attributes.Add("class", "GVRow");
                    }
                    #endregion
                }
            }
        }
    }
}
