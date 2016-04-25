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

public partial class ISI_Reports_Task_Main : com.Sconit.Web.MainModuleBase
{
    public IList<Task> NoStatusTaskList
    {
        get
        {
            return (IList<Task>)ViewState["NoStatusTaskList"];
        }
        set
        {
            ViewState["NoStatusTaskList"] = value;
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

            DataSet dataSet;
            GetDateSet(out dataSet, string.Empty);

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


    private void GetDateSet(out DataSet dataSet, string sortdirection)
    {
        string dept2 = this.tbDept2.Text.Trim();
        string dept = this.ddlDept.SelectedValue;
        string position = ddlPosition.SelectedValue;
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;

        IList<SqlParameter> sqlParam = new List<SqlParameter>();
        sqlParam.Add(new SqlParameter("@IsActive", ckIsActive.Checked));
        sqlParam.Add(new SqlParameter("@Type", this.ddlType.SelectedValue));
        sqlParam.Add(new SqlParameter("@Org", this.ddlOrg.SelectedValue));
        sqlParam.Add(new SqlParameter("@TaskSubType", this.tbTaskSubType.Text.Trim()));
        sqlParam.Add(new SqlParameter("@User", tbUser.Text.Trim()));
        sqlParam.Add(new SqlParameter("@Position", position));
        sqlParam.Add(new SqlParameter("@Dept", dept));
        sqlParam.Add(new SqlParameter("@Dept2", dept2));
        sqlParam.Add(new SqlParameter("@IsFilter", this.ckIsFilter.Checked));
        sqlParam.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
        sqlParam.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
        sqlParam.Add(new SqlParameter("@Sort", GridViewSortExpression));
        sqlParam.Add(new SqlParameter("@Direction", sortdirection));
        dataSet = TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Rep_Task", sqlParam.ToArray<SqlParameter>());

    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var statistics = ((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray;
            e.Row.Cells[4].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            if ((!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN) || this.CurrentUser.Code == statistics[0].ToString()) && statistics[2].ToString() == "执委会")
            {
                e.Row.Cells[4].Text = string.Empty;
            }
            else if (NoStatusTaskList != null && NoStatusTaskList.Count > 0)
            {
                StringBuilder text = new StringBuilder();

                var taskList = NoStatusTaskList.Where(t => t.FirstUser == statistics[0].ToString()).Select(t => t.Code).ToList();
                if (taskList != null && taskList.Count > 0)
                {
                    foreach (var task in taskList)
                    {
                        if (text.Length != 0)
                        {
                            text.Append("、");
                        }
                        text.Append(task);
                    }
                }
                e.Row.Cells[4].Text = "<font color='red'><b>" + e.Row.Cells[4].Text + "</b></font><br><br>" + text.ToString();
            }
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            for (int i = 2; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].ToolTip = (((System.Web.UI.WebControls.DataControlFieldCell)(e.Row.Cells[i])).ContainingField).HeaderText.Insert((((System.Web.UI.WebControls.DataControlFieldCell)(e.Row.Cells[i])).ContainingField).HeaderText.IndexOf('}'), ".Desc");
            }
        }
    }
    [Serializable]
    public class AssignUser
    {
        public string StartedUser { get; set; }
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
    protected void GV_List_DataBinding(object sender, EventArgs e)
    {
        string dept2 = this.tbDept2.Text.Trim();
        string dept = this.ddlDept.SelectedValue;
        string position = ddlPosition.SelectedValue;
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;

        IList<SqlParameter> sqlParam = new List<SqlParameter>();
        sqlParam.Add(new SqlParameter("@IsActive", ckIsActive.Checked));
        sqlParam.Add(new SqlParameter("@Type", this.ddlType.SelectedValue));
        sqlParam.Add(new SqlParameter("@Org", this.ddlOrg.SelectedValue));
        sqlParam.Add(new SqlParameter("@TaskSubType", this.tbTaskSubType.Text.Trim()));
        sqlParam.Add(new SqlParameter("@User", tbUser.Text.Trim()));
        sqlParam.Add(new SqlParameter("@Position", position));
        sqlParam.Add(new SqlParameter("@Dept", dept));
        sqlParam.Add(new SqlParameter("@Dept2", dept2));
        sqlParam.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
        sqlParam.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1)));

        DataSet noAssignTaskDS = TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Rep_NoStatusTask", sqlParam.ToArray<SqlParameter>());
        NoStatusTaskList = IListHelper.DataTableToList<Task>(noAssignTaskDS.Tables[0]);

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

        DataSet dataSet;
        GetDateSet(out dataSet, sortdirection);

        this.GV_List.DataSource = dataSet.Tables[0].DefaultView;
        this.GV_List.DataBind();
    }

    [Serializable]
    public class Task
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string FirstUser { get; set; }
    }

}