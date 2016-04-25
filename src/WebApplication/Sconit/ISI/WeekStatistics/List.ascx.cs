using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;

public partial class ISI_WeekStatistics_List : ListModuleBase
{

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    public void Export()
    {
        this.ExportXLS(GV_List);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TaskWeekStatisticsView task = (TaskWeekStatisticsView)e.Row.DataItem;
            int rowCoun = 5;
            e.Row.Cells[rowCoun++].Text = task.CreateCount == 0 ? string.Empty : task.CreateCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.SubmitCount == 0 ? string.Empty : task.SubmitCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.CancelCount == 0 ? string.Empty : task.CancelCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.AssignCount == 0 ? string.Empty : task.AssignCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.SubmitFirstCount == 0 ? string.Empty : task.SubmitFirstCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.FirstCount == 0 ? string.Empty : task.FirstCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.StatusCount == 0 ? string.Empty : task.StatusCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.FileCount == 0 ? string.Empty : task.FileCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.CommentCount == 0 ? string.Empty : task.CommentCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.CloseCount == 0 ? string.Empty : task.CloseCount.ToString();
            e.Row.Cells[rowCoun++].Text = task.OpenCount == 0 ? string.Empty : task.OpenCount.ToString();
        }
    }
}
