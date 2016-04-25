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
using System.Text;

public partial class ISI_TaskReport_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InitPageParameter();
        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter()
    {
        this.ODS_GV_TaskReport.SelectParameters["userCode"].DefaultValue = this.CurrentUser.Code;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            IList<TaskReportView> taskReportViewList = this.PopulateData();
            this.TheTaskReportMgr.UpdateTaskReport(taskReportViewList, this.CurrentUser);
            this.GV_TaskReport.DataBind();
            this.ShowSuccessMessage("ISI.TaskReport.Successfully");
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    private IList<TaskReportView> PopulateData()
    {
        if (this.GV_TaskReport.Rows != null && this.GV_TaskReport.Rows.Count > 0)
        {
            IList<TaskReportView> taskReportViewList = new List<TaskReportView>();
            foreach (GridViewRow row in this.GV_TaskReport.Rows)
            {
                CheckBox cbIsActive = row.FindControl("cbIsActive") as CheckBox;

                TaskReportView taskReportView = new TaskReportView();
                HiddenField hfId = row.FindControl("hfId") as HiddenField;
                if (!string.IsNullOrEmpty(hfId.Value.Trim()) & hfId.Value.Trim() != "null")
                {
                    taskReportView.Id = int.Parse(hfId.Value.Trim());
                }
                else
                {
                    taskReportView.Id = null;
                }

                taskReportView.TaskSubTypeCode = row.Cells[1].Text.Trim();
                taskReportView.IsActive = cbIsActive.Checked;

                taskReportViewList.Add(taskReportView);
            }
            return taskReportViewList;
        }

        return null;
    }

    protected void FV_List_DataBound(object sender, EventArgs e)
    {
        if ((((System.Web.UI.WebControls.GridView)(sender))).PageCount == 0)
        {
            btnSave.Visible = false;
        }
        else
        {
            btnSave.Visible = true;
        }
    }
    protected void FV_List_OnDataBinding(object sender, EventArgs e)
    {

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //TaskReportView taskReportView = (TaskReportView)e.Row.DataItem;
        }
    }

    protected void ODS_GV_TaskReport_OnUpdating(object source, ObjectDataSourceMethodEventArgs e)
    {

    }

    protected void GV_TaskReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string Code = GV_TaskReport.Rows[e.RowIndex].Cells[1].Text;
        ShowSuccessMessage("ISI.TaskReport.Update.Successfully", Code);
    }
}