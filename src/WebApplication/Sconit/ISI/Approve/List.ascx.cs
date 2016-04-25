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
using NHibernate.Expression;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;

public partial class ISI_Approve_List : ListModuleBase
{
    private object[] Param
    {
        get { return (object[])ViewState["Param"]; }
        set { ViewState["Param"] = value; }
    }
    private decimal[] amountLimitEmployee
    {
        get { return (decimal[])ViewState["amountLimitEmployee"]; }
        set { ViewState["amountLimitEmployee"] = value; }
    }
    private decimal[] amountLimitCadre
    {
        get { return (decimal[])ViewState["amountLimitCadre"]; }
        set { ViewState["amountLimitCadre"] = value; }
    }
    public EventHandler EditEvent;
    public EventHandler SummaryEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string[] amountLimitCadreStr = this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_AMOUNTLIMIT_CADRE).Value.Split(new char[] { ',' });
            string[] amountLimitEmployeeStr = this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_AMOUNTLIMIT_EMPLOYEE).Value.Split(new char[] { ',' });

            amountLimitCadre = amountLimitCadreStr.Where(a => !string.IsNullOrEmpty(a)).Select(a => decimal.Parse(a)).ToArray();
            amountLimitEmployee = amountLimitEmployeeStr.Where(a => !string.IsNullOrEmpty(a)).Select(a => decimal.Parse(a)).ToArray();


        }
    }
    public override void UpdateView()
    {
    }

    public void InitPageParameter(object sender)
    {
        this.Param = (object[])sender;

        IList<string> departmentList = (IList<string>)Param[0];
        string checkupProject = (string)Param[1];
        string checkupUser = (string)Param[2];
        string createUser = (string)Param[3];
        string type = (string)Param[4];
        DateTime? startTime = (DateTime?)Param[5];
        DateTime? endTime = (DateTime?)Param[6];
        IList<string> statusList = (IList<string>)Param[7];
        object isSummary = Param[8];

        #region DetachedCriteria
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Checkup));

        if (statusList != null && statusList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
        }

        if (!string.IsNullOrEmpty(checkupProject))
        {
            selectCriteria.Add(Expression.Eq("CheckupProject.Code", checkupProject));
        }
        if (departmentList != null && departmentList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Department", departmentList.ToArray<string>()));
        }
        if (!string.IsNullOrEmpty(createUser))
        {
            selectCriteria.Add(Expression.Eq("CreateUser", createUser));
        }
        if (!string.IsNullOrEmpty(createUser))
        {
            selectCriteria.Add(Expression.Eq("CreateUser", createUser));
        }
        if (!string.IsNullOrEmpty(checkupUser))
        {
            selectCriteria.Add(Expression.Eq("CheckupUser", checkupUser));
        }
        if (!string.IsNullOrEmpty(type))
        {
            selectCriteria.Add(Expression.Eq("Type", type));
        }
        if (startTime.HasValue)
        {
            selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Ge("CreateDate", startTime)));
        }
        if (endTime.HasValue)
        {
            selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Le("CreateDate", endTime)));
        }

        if (!this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP)
                && !this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_CLOSECHECKUP)
                && !this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_CREATECHECKUP))
        {
            selectCriteria.Add(Expression.In("Status", new string[] { ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT, ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL, ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE }));

        }
        if (isSummary != null)
        {
            if (bool.Parse(isSummary.ToString()))
            {
                selectCriteria.Add(Expression.Eq("CheckupProject.Code", ISIConstants.CODE_MASTER_SUMMARY_CHECKUPPROJECT));
            }
            else
            {
                selectCriteria.Add(Expression.Not(Expression.Eq("CheckupProject.Code", ISIConstants.CODE_MASTER_SUMMARY_CHECKUPPROJECT)));
            }
        }

        #endregion

        selectCriteria.AddOrder(Order.Desc("CreateDate"));
        selectCriteria.AddOrder(Order.Asc("Department"));
        selectCriteria.AddOrder(Order.Asc("CheckupUser"));
        selectCriteria.AddOrder(Order.Asc("CreateUser"));
        selectCriteria.AddOrder(Order.Asc("CheckupProject"));

        IList<Checkup> checkupList = TheCriteriaMgr.FindAll<Checkup>(selectCriteria);
        IList<string> checkupUserList = checkupList.GroupBy(c => c.CheckupUser).Select(c => c.Key).ToList<string>();

        foreach (string checkupUserCode in checkupUserList)
        {
            Checkup checkupFirst = checkupList.First(c => c.CheckupUser == checkupUserCode);
            decimal amountTotal = checkupList.Where(checkup => checkup.CheckupUser == checkupUserCode).Sum(checkup => checkup.Amount.HasValue ? checkup.Amount.Value : 0);
            Checkup totalCheckup = new Checkup();
            totalCheckup.Amount = amountTotal;
            totalCheckup.Id = checkupFirst.Id;
            totalCheckup.Type = checkupFirst.Type;
            totalCheckup.CheckupUserNm = checkupFirst.CheckupUserNm;
            totalCheckup.Department = checkupFirst.Department;
            totalCheckup.Dept2 = checkupFirst.Dept2;
            totalCheckup.JobNo = checkupFirst.JobNo;
            checkupList.Insert(checkupList.IndexOf(checkupFirst), totalCheckup);

            var cuList = checkupList.Where(checkup => checkup.CheckupUser == checkupUserCode).ToList<Checkup>();
            for (int i = 1; i < cuList.Count; i++)
            {
                var t = cuList[i];
                checkupList.Remove(cuList[i]);
                checkupList.Insert(checkupList.IndexOf(checkupFirst) + 1 + i, t);
            }
        }

        this.GV_List.DataSource = checkupList;
        this.GV_List.DataBind();
    }
    protected void lbtnSummary_Click(object sender, EventArgs e)
    {
        if (SummaryEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            SummaryEvent(code, e);
        }
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }

    public void Export()
    {
        this.IsExport = true;
        GV_List.Columns[3].Visible = false;
        this.ExportXLS(GV_List, "考核" + DateTime.Now.ToString("yyyy年MM月dd日HH：mm") + ".xls");
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {

        GV_MergeTableCell(GV_List, new int[] { 0, 1 }, false);
        if (!this.IsExport)
        {
            GV_MergeTableCell(GV_List, 2, 7);
        }
    }

    public void Approve()
    {
        try
        {
            IList<Checkup> checkupList = PopulateData(string.Empty);
            if (checkupList != null && checkupList.Count > 0)
            {
                this.TheCheckupMgr.ApproveCheckup(checkupList, this.CurrentUser);

                this.ShowSuccessMessage("ISI.Checkup.Approve.Successfully");
            }

        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }

    }
    public void Close()
    {
        try
        {
            IList<Checkup> checkupList = PopulateData(ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL);
            if (checkupList != null && checkupList.Count > 0)
            {
                this.TheCheckupMgr.CloseCheckup(checkupList, this.CurrentUser);

                this.ShowSuccessMessage("ISI.Checkup.Close.Successfully");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }

    }

    public IList<Checkup> PopulateData(string status)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            IList<Checkup> checkupList = new List<Checkup>();
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                Checkup c = new Checkup();
                string hfId = (row.FindControl("lbtnEdit") as LinkButton).CommandArgument;
                c.Status = (row.FindControl("hfStatus") as HiddenField).Value;
                RangeValidator rvAmount = (row.FindControl("rvAmount") as RangeValidator);
                TextBox tbAuditInstructions = (TextBox)(row.Cells[5].FindControl("tbAuditInstructions"));
                if (rvAmount.IsValid && !string.IsNullOrEmpty(c.Status) && hfId != "0" && !string.IsNullOrEmpty(tbAuditInstructions.Text.Trim()) && ((string.IsNullOrEmpty(status) && ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE != c.Status) || c.Status == status))
                {
                    if (!string.IsNullOrEmpty(hfId) & hfId.Trim() != "null")
                    {
                        c.Id = int.Parse(hfId);
                    }
                    else
                    {
                        continue;
                    }
                    TextBox tbAmount = (TextBox)(row.Cells[4].FindControl("tbAmount"));

                    if (!string.IsNullOrEmpty(tbAmount.Text.Trim()))
                    {
                        c.Amount = decimal.Parse(tbAmount.Text.Trim());
                    }
                    else
                    {
                        c.Amount = null;
                    }
                    c.AuditInstructions = tbAuditInstructions.Text.Trim();
                    checkupList.Add(c);
                }
            }
            return checkupList;
        }

        return null;
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Checkup checkup = (Checkup)e.Row.DataItem;
            Label lblStatus = ((Label)(e.Row.FindControl("lblStatus")));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage("ISI.Status." + checkup.Status, this.CurrentUser);
            e.Row.Cells[5].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            if (string.IsNullOrEmpty(checkup.CheckupUser))
            {
                e.Row.Cells[6].FindControl("tbAmount").Visible = false;
                e.Row.Cells[7].FindControl("tbAuditInstructions").Visible = false;
                decimal? amount = checkup.Amount;
                if (amount.HasValue && amount != 0)
                {
                    e.Row.Cells[2].Text = amount.Value.ToString("0.##");
                    if (amount.Value > 0)
                    {
                        if (this.IsExport)
                        {
                            e.Row.Cells[4].Text = "奖";
                        }
                        else
                        {
                            e.Row.Cells[2].Text += "&nbsp;奖";
                        }
                    }
                    else
                    {
                        if (this.IsExport)
                        {
                            e.Row.Cells[4].Text = "罚";
                        }
                        else
                        {
                            e.Row.Cells[2].Text += "&nbsp;罚";
                        }
                    }

                    if ((checkup.Type == ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_EMPLOYEE && ((amountLimitEmployee.Length > 0 && amount.Value < amountLimitEmployee[0]) || (amountLimitEmployee.Length > 1 && amount.Value > amountLimitEmployee[1])))
                        || (checkup.Type == ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_CADRE && ((amountLimitCadre.Length > 0 && amount.Value < amountLimitCadre[0]) || (amountLimitCadre.Length > 1 && amount.Value > amountLimitCadre[1]))))
                    {
                        e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    e.Row.Cells[2].Text = amount.Value.ToString("0.##");
                }
            }
            else if (!this.IsExport)
            {
                if (checkup.Amount.HasValue)
                {
                    if (checkup.Amount.Value != 0)
                    {
                        ((TextBox)e.Row.Cells[6].FindControl("tbAmount")).Text = checkup.Amount.Value.ToString("0.##");
                    }
                    else
                    {
                        ((TextBox)e.Row.Cells[6].FindControl("tbAmount")).Text = string.Empty;
                    }
                }

                ((Label)(e.Row.Cells[5].FindControl("lblContent"))).Text = "<span style='color:#0000E5;'>" + checkup.SubmitUserNm + "(" + checkup.SubmitDate.Value.ToString("yyyy-MM-dd HH:mm") + ")</span>: " + (!string.IsNullOrEmpty(checkup.Content) ? checkup.Content.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") : string.Empty);
                if (checkup.ApprovalDate.HasValue)
                {
                    ((Label)(e.Row.Cells[6].FindControl("lblAuditInstructions"))).Text = "<span style='color:#0000E5;'>" + checkup.ApprovalUserNm + "(" + checkup.ApprovalDate.Value.ToString("yyyy-MM-dd HH:mm") + ")</span>: <br>";
                }
            }

            if (this.IsExport)
            {
                e.Row.Cells[2].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
                if (!string.IsNullOrEmpty(checkup.CheckupUser))
                {
                    LinkButton lbtnEdit = (LinkButton)e.Row.Cells[1].FindControl("lbtnEdit");
                    lbtnEdit.Visible = false;
                    e.Row.Cells[2].Text = checkup.CheckupDate.ToString("yyyy-MM-dd HH:ss");

                    e.Row.Cells[5].FindControl("lblContent").Visible = false;
                    if (checkup.SubmitDate.HasValue)
                    {
                        e.Row.Cells[5].Text = checkup.SubmitUserNm + "(" + checkup.SubmitDate.Value.ToString("yyyy-MM-dd HH:mm") + "): " + (!string.IsNullOrEmpty(checkup.Content) ? checkup.Content.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") : string.Empty);
                    }
                    e.Row.Cells[6].FindControl("tbAmount").Visible = false;
                    e.Row.Cells[6].Text = checkup.Amount.HasValue ? checkup.Amount.Value.ToString("0.##") : string.Empty;
                    e.Row.Cells[7].FindControl("tbAuditInstructions").Visible = false;
                    if (checkup.ApprovalDate.HasValue)
                    {
                        e.Row.Cells[7].Text = checkup.ApprovalUserNm + "(" + checkup.ApprovalDate.Value.ToString("yyyy-MM-dd HH:mm") + "): " + (!string.IsNullOrEmpty(checkup.AuditInstructions) ? checkup.AuditInstructions.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") : string.Empty);
                    }
                }
            }

            if (!this.IsExport && !this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVECHECKUP))
            {
                ((TextBox)e.Row.Cells[6].FindControl("tbAmount")).ReadOnly = true;
                ((TextBox)e.Row.Cells[7].FindControl("tbAuditInstructions")).ReadOnly = true;
            }
        }
    }



    public static void GV_MergeTableCell(GridView GV, int sCol, int eCol)
    {
        for (int j = 0; j < GV.Rows.Count; j++)
        {
            HiddenField hfStatus = (HiddenField)GV.Rows[j].FindControl("hfStatus");
            if (!string.IsNullOrEmpty(hfStatus.Value)) continue;

            TableCell oldTc = GV.Rows[j].Cells[sCol];
            for (int i = 1; i <= eCol - sCol; i++)
            {
                TableCell tc = GV.Rows[j].Cells[i + sCol];
                tc.Visible = false;
                if (oldTc.ColumnSpan == 0)
                {
                    oldTc.ColumnSpan = 1;
                }
                oldTc.ColumnSpan++;
                oldTc.VerticalAlign = VerticalAlign.Middle;
                //oldTc.HorizontalAlign = HorizontalAlign.Center;
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


