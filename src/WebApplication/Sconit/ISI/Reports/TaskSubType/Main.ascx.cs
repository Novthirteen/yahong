using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Text;

public partial class ISI_Reports_TaskSubType_Main : com.Sconit.Web.MainModuleBase
{
    public IList<Task> NoAssignTaskList
    {
        get
        {
            return (IList<Task>)ViewState["NoAssignTaskList"];
        }
        set
        {
            ViewState["NoAssignTaskList"] = value;
        }
    }

    public IList<Task> OverStartTaskCodeList
    {
        get
        {
            return (IList<Task>)ViewState["OverStartTaskCodeList"];
        }
        set
        {
            ViewState["OverStartTaskCodeList"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:,bool:true,string:" + this.CurrentUser.Code + ",bool:false,bool:false,bool:false,bool:false";
        tbTaskSubType.DataBind();

        if (!IsPostBack)
        {
            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.AddDays(-7).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = now.ToString("yyyy-MM-dd");
            //btnSearch_Click(null, null);  
            this.ddlType.Items.RemoveAt(1);
            //this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
            //this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string startDate;
            string endDate;
            DataSet dataSet;
            GetDateSet(out startDate, out endDate, out dataSet, string.Empty);

            this.GV_List.DataSource = dataSet;
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    private void GetDateSet(out string startDate, out string endDate, out DataSet dataSet, string sortdirection)
    {
        string dept = this.ddlDept.SelectedValue;
        IList<SqlParameter> sqlParam = new List<SqlParameter>();
        sqlParam.Add(new SqlParameter("@IsActive", ckIsActive.Checked));

        startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        sqlParam.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
        sqlParam.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1)));

        sqlParam.Add(new SqlParameter("@Org", dept));
        sqlParam.Add(new SqlParameter("@Type", this.ddlType.SelectedValue));
        sqlParam.Add(new SqlParameter("@TaskSubType", this.tbTaskSubType.Text.Trim()));
        sqlParam.Add(new SqlParameter("@Sort", GridViewSortExpression));
        sqlParam.Add(new SqlParameter("@Direction", sortdirection));

        dataSet = TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Rep_TaskSubType", sqlParam.ToArray<SqlParameter>());

    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
    }
    protected void GV_List_DataBinding(object sender, EventArgs e)
    {
        IList<SqlParameter> sqlParam = new List<SqlParameter>();
        sqlParam.Add(new SqlParameter("@IsActive", ckIsActive.Checked));

        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        sqlParam.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
        sqlParam.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
        string dept = this.ddlDept.SelectedValue;
        sqlParam.Add(new SqlParameter("@Org", dept));
        sqlParam.Add(new SqlParameter("@Type", this.ddlType.SelectedValue));
        sqlParam.Add(new SqlParameter("@TaskSubType", this.tbTaskSubType.Text.Trim()));

        DataSet noAssignTaskDS = TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Rep_NoAssignTask", sqlParam.ToArray<SqlParameter>());
        NoAssignTaskList = IListHelper.DataTableToList<Task>(noAssignTaskDS.Tables[0]);

        DataSet overStartTaskDS = TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Rep_OverStartTask", sqlParam.ToArray<SqlParameter>());
        OverStartTaskCodeList = IListHelper.DataTableToList<Task>(overStartTaskDS.Tables[0]);
    }
    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[8].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[10].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");

            var statistics = ((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray;
            if (statistics[2] != null && statistics[2].ToString() != string.Empty)
            {
                e.Row.Cells[0].Attributes["style"] = "background-color:" + statistics[2].ToString();
            }
            if (NoAssignTaskList != null && NoAssignTaskList.Count > 0)
            {
                StringBuilder text = new StringBuilder();

                var taskList = NoAssignTaskList.Where(t => t.TaskSubType == statistics[0].ToString()).ToList();
                if (taskList != null && taskList.Count > 0)
                {
                    foreach (var task in taskList)
                    {
                        if (text.Length != 0)
                        {
                            text.Append("、");
                        }
                        text.Append(task.Code + " " + task.SubmitUserNm);
                    }
                }
                e.Row.Cells[8].Text = "<font color='red'><b>" + e.Row.Cells[8].Text + "</b></font><br><br>" + text.ToString();

            }

            if (OverStartTaskCodeList != null && OverStartTaskCodeList.Count > 0)
            {
                StringBuilder text = new StringBuilder();
                var taskList = OverStartTaskCodeList.Where(t => t.TaskSubType == statistics[0].ToString()).ToList();
                if (taskList != null && taskList.Count > 0)
                {
                    foreach (var task in taskList)
                    {
                        if (text.Length != 0)
                        {
                            text.Append("、");
                        }
                        text.Append(task.Code);

                        if (!string.IsNullOrEmpty(task.AssignStartUserNm))
                        {
                            var pos = task.AssignStartUserNm.IndexOf(",");
                            if (pos != -1)
                            {
                                text.Append(" " + task.AssignStartUserNm.Substring(0, task.AssignStartUserNm.IndexOf(",")));
                            }
                            else
                            {
                                text.Append(" " + task.AssignStartUserNm);
                            }
                        }
                    }
                }
                e.Row.Cells[10].Text = "<font color='red'><b>" + e.Row.Cells[10].Text + "</b></font><br><br>" + text.ToString();

            }


        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[3].ToolTip = this.TheLanguageMgr.TranslateMessage("ISI.Reports.TaskSubType.ProcessCount.Desc", this.CurrentUser);
            e.Row.Cells[7].ToolTip = this.TheLanguageMgr.TranslateMessage("ISI.Reports.TaskSubType.AssignCount.Desc", this.CurrentUser);
            e.Row.Cells[8].ToolTip = this.TheLanguageMgr.TranslateMessage("ISI.Reports.TaskSubType.NoAssignCount.Desc", this.CurrentUser);
            e.Row.Cells[9].ToolTip = this.TheLanguageMgr.TranslateMessage("ISI.Reports.TaskSubType.InProcessCount.Desc", this.CurrentUser);
            e.Row.Cells[10].ToolTip = this.TheLanguageMgr.TranslateMessage("ISI.Reports.TaskSubType.OverStartCount.Desc", this.CurrentUser);
        }
    }

    public string GridViewSortExpression
    {
        get
        {
            if (ViewState["sortExpression"] == null)
                ViewState["sortExpression"] = "itop";             //默认排序字段
            return ViewState["sortExpression"].ToString();
        }

        set { ViewState["sortExpression"] = value; }
    }

    public SortDirection GridViewSortDirection
    {
        get
        {
            if (ViewState["sortDirection"] == null)
                ViewState["sortDirection"] = SortDirection.Ascending;          //默认升序
            return (SortDirection)ViewState["sortDirection"];
        }

        set { ViewState["sortDirection"] = value; }
    }

    protected void GV_List_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression = e.SortExpression;
        GridViewSortExpression = sortExpression;       //存到ViewState中排序的字段:newsid     
        if (GridViewSortExpression == sortExpression)
        {
            if (GridViewSortDirection == SortDirection.Ascending) //设置排序方向
            {
                GridViewSortDirection = SortDirection.Descending;
            }
            else
            {
                GridViewSortDirection = SortDirection.Ascending;
            }
        }
        this.InitGridView();
    }

    protected void InitGridView()
    {
        string sortdirection;
        if (GridViewSortDirection == SortDirection.Ascending)
        {
            sortdirection = "asc";
        }
        else
        {
            sortdirection = "desc";
        }
        string startDate;
        string endDate;
        DataSet dataSet;
        GetDateSet(out startDate, out endDate, out dataSet, sortdirection);
        this.GV_List.DataSource = dataSet.Tables[0].DefaultView;
        this.GV_List.DataBind();
    }
    [Serializable]
    public class Task
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string SubmitUserNm { get; set; }
        public string TaskSubType { get; set; }
        public string AssignStartUserNm { get; set; }
    }
}
