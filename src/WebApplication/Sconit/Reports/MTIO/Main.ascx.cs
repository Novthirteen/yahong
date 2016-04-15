using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using com.Sconit.Utility;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using NHibernate.Transform;
using System.Data.SqlClient;
using System.IO;
public partial class Reports_MTIO_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DateTime? startTime = null;
        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        else
        {
            ShowErrorMessage("请选择开始时间。");
            return;
        }
        DateTime? endTime = null;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endTime = DateTime.Parse(this.tbEndDate.Text.Trim());
        }
        else
        { 
            ShowErrorMessage("请选择结束时间。");
            return;
        }
        TimeSpan duration = endTime.Value - startTime.Value;
        if (duration.Days > 7)
        {
            ShowErrorMessage("请将时间查询范围限制在7天以内.");
            return;
        }
        try
        {
            string sql = @"USP_Report_Material_InOut";
            string type;
            if (this.groupType.SelectedIndex == 0)
            {
                type = "ByCategory";
            }
            else if (this.groupType.SelectedIndex == 1)
            {
                type = "ByItem";
            }
            else
            {
                type = "ByBondedMT";
            }

            SqlParameter[] sqlParam = new SqlParameter[3];
            sqlParam[0] = new SqlParameter("@StartTime", startTime);
            sqlParam[1] = new SqlParameter("@EndTime", endTime);
            sqlParam[2] = new SqlParameter("@Type", type);

            this.GV_List.DataSource = TheSqlHelperMgr.GetDatasetByStoredProcedure(sql, sqlParam);
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

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }
    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (this.groupType.SelectedIndex == 0)
        {
            GridViewHelper.GV_MergeTableCell(this.GV_List, new int[] { 1, 2 });
        }
        else if (this.groupType.SelectedIndex == 1)
        {
            GridViewHelper.GV_MergeTableCell(this.GV_List, new int[] { 1, 2, 3, 4 });
        }
    }
}
