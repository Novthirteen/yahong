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

public partial class ISI_TaskSubTypeStatistics_Main : com.Sconit.Web.MainModuleBase
{

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
            this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
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
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.Append("select  tst.Code,tst.Desc_ Description,");
        sql.Append("        Count(distinct(t.Code)) CreateCount, ");
        sql.Append("        Count(distinct(t1.Code)) TotalCount, ");
        sql.Append("        Max(t.CreateDate) CreateDate,");
        sql.Append("        Max(t.SubmitDate) SubmitDate, ");
        sql.Append("        Max(t.AssignDate) AssignDate,");
        sql.Append("        Max(t.StartDate) StartDate,");
        sql.Append("        Max(t.CompleteDate) CompleteDate,");
        sql.Append("        Max(t.CloseDate) CloseDate,");
        sql.Append("        Max(t.OpenDate) OpenDate,");
        sql.Append("        Max(t.CancelDate) CancelDate,");
        sql.Append("        Count(distinct(ts.Id)) StatusCount, ");
        sql.Append("        Max(ts.LastModifyDate) StatusDate, ");
        sql.Append("        Count(distinct(c.Id)) CommentCount, ");
        sql.Append("        Max(c.LastModifyDate) CommentDate, ");
        sql.Append("        pt.Project, ");
        sql.Append("        et.Enc, ");
        sql.Append("        pt.Project2 ");
        sql.Append("from ISI_TaskSubType tst ");
        sql.Append("left join ISI_TaskMstr t1 on t1.Status!='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL + "' and  tst.Code = t1.TaskSubType and t1.CreateDate <= @EndDate ");

        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t1.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t1.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");

        sql.Append("left join ISI_TaskMstr t on t.Status!='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL + "' and  tst.Code = t.TaskSubType and t.CreateDate >= @StartDate  and t.CreateDate <= @EndDate ");

        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");


        string dept = this.ddlDept.SelectedValue;
        IList<SqlParameter> sqlParam = new List<SqlParameter>();
        sqlParam.Add(new SqlParameter("@IsActive", ckIsActive.Checked));

        if (!string.IsNullOrEmpty(dept))
        {
            sql.Append("and tst.Org = @Org ");
            sqlParam.Add(new SqlParameter("@Org", dept));
        }

        sql.Append("     left join ISI_TaskStatus ts on t.Code = ts.TaskCode and ts.LastModifyDate >= @StartDate  and ts.LastModifyDate <= @EndDate ");
        sql.Append("     left join ISI_CommentDet c on t.Code = c.TaskCode  and c.LastModifyDate >= @StartDate  and c.LastModifyDate <= @EndDate  ");


        sql.Append("    left join (select pt.TaskSubType,max(Phase) Project,min(Phase) Project2 from ISI_TaskMstr pt where pt.type='Project' and pt.Phase is not null and pt.Status in ('Submit','In-Proect','Complete','Assign') group by pt.TaskSubType) pt on pt.TaskSubType=tst.Code ");
        sql.Append("    left join (select et.TaskSubType,max(et.Code) Enc from ISI_TaskMstr et where et.type='Enc' and et.Status!='Cancel' group by et.TaskSubType ) et on et.TaskSubType=tst.Code ");

        System.Text.StringBuilder where = new System.Text.StringBuilder();
        where.Append("where tst.IsActive = @IsActive ");
        where.Append("  and  tst.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            where.Append("  and  tst.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            where.Append("  and  tst.Code = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        sqlParam.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
        sqlParam.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));

        sql.Append(where.ToString());

        sql.Append("group by tst.Code,tst.Desc_,pt.Project,pt.Project2,et.Enc ");

        if (!string.IsNullOrEmpty(sortdirection) && !string.IsNullOrEmpty(this.GridViewSortExpression))
        {
            sql.Append(" order by " + GridViewSortExpression + " " + sortdirection);
        }

        dataSet = TheSqlHelperMgr.GetDatasetBySql(sql.ToString(), sqlParam.ToArray<SqlParameter>());
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
            var lastWeekCount = int.Parse(statistics[3].ToString()) - int.Parse(statistics[2].ToString());
            e.Row.Cells[5].Text = lastWeekCount == 0 ? string.Empty : lastWeekCount.ToString();
            if (statistics[16] != null && !string.IsNullOrEmpty(statistics[16].ToString()))
            {
                if (statistics[18] != null && !string.IsNullOrEmpty(statistics[18].ToString()) && statistics[16].ToString() != statistics[18].ToString())
                {
                    e.Row.Cells[3].Text = statistics[18].ToString() + "-" + statistics[16].ToString();
                }
            }
            if (statistics[17] != null && !string.IsNullOrEmpty(statistics[17].ToString()))
            {
                e.Row.Cells[4].Text = "${ISI.TaskSubTypeStatistics.Executed}";
            }
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

}