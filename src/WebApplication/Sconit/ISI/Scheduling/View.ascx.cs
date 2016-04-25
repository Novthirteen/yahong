using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using System.Text;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using NHibernate;
using com.Sconit.Utility;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using System.Drawing;

public partial class ISI_Scheduling_View : com.Sconit.Web.MainModuleBase
{
    public event EventHandler BackEvent;

    public void InitPageParameter(string taskCode)
    {


    }

    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:" + this.CurrentUser.Code;
        tbTaskSubType.DataBind();

        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.tbEndDate.Text = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.fs.Visible = true;
        string para_starttime = this.tbStartDate.Text.Trim();
        string para_endtime = this.tbEndDate.Text.Trim();
        DateTime starttime = DateTime.Now;
        DateTime endtime = DateTime.Now;
        try
        {
            starttime = Convert.ToDateTime(para_starttime);
        }
        catch (Exception)
        {
            ShowWarningMessage("ISI.Scheduling.WarningMessage.StartTimeInvalid");
            return;
        }
        try
        {
            endtime = Convert.ToDateTime(para_endtime);
        }
        catch (Exception)
        {
            ShowWarningMessage("ISI.Scheduling.WarningMessage.EndTimeInvalid");
            return;
        }
        if (DateTime.Compare(starttime, endtime) > 0)
        {
            ShowWarningMessage("ISI.Scheduling.WarningMessage.TimeCompare");
            return;
        }

        string taskSubType = this.tbTaskSubType.Text.Trim();
        IList<SchedulingView> views = this.TheSchedulingMgr.GetScheduling2(starttime, endtime, taskSubType, this.CurrentUser.Code);

        this.TheSchedulingMgr.SetStartUser(views);

        this.GV_List_View.DataSource = views;

        this.GV_List_View.DataBind();
        GridViewHelper.GV_MergeTableCell(GV_List_View, new int[] { 0, 1 });

        if ((Button)sender == this.btnExport)
        {
            this.ExportXLS(this.GV_List_View);
        }
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
            SchedulingView schedulingView = (SchedulingView)e.Row.DataItem;
            #region text format
            string week = e.Row.Cells[1].Text;
            switch (week)
            {
                case "Monday":
                    e.Row.Cells[1].Text = "${Common.Week.Monday}";
                    break;
                case "Tuesday":
                    e.Row.Cells[1].Text = "${Common.Week.Tuesday}";
                    break;
                case "Wednesday":
                    e.Row.Cells[1].Text = "${Common.Week.Wednesday}";
                    break;
                case "Thursday":
                    e.Row.Cells[1].Text = "${Common.Week.Thursday}";
                    break;
                case "Friday":
                    e.Row.Cells[1].Text = "${Common.Week.Friday}";
                    break;
                case "Saturday":
                    e.Row.Cells[1].Text = "${Common.Week.Saturday}";
                    break;
                case "Sunday":
                    e.Row.Cells[1].Text = "${Common.Week.Sunday}";
                    break;
                default:
                    break;
            }
            CodeMaster type = TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_WORKCALENDAR_TYPE, e.Row.Cells[8].Text.Trim());
            if (type != null)
            {
                e.Row.Cells[8].Text = type.Description;
            }

            #endregion
            #region add class 休息日和工作日以不同的背景色区分

            if (schedulingView.WorkdayType == BusinessConstants.CODE_MASTER_WORKCALENDAR_TYPE_VALUE_WORK)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("class", "GVRow");
                }
            }
            else if (schedulingView.WorkdayType == BusinessConstants.CODE_MASTER_WORKCALENDAR_TYPE_VALUE_REST)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Attributes.Add("class", "GVAlternatingRow");
                }
            }

            #endregion
            if (string.IsNullOrEmpty(schedulingView.StartUser))
            {
                e.Row.Cells[5].BackColor = Color.Pink;
            }
            else
            {
                if (schedulingView.SchedulingType == ISIConstants.SCHEDULINGTYPE_TASKSUBTYPE)
                {
                    e.Row.Cells[5].ForeColor = Color.BlueViolet;
                }
                else if (schedulingView.SchedulingType == ISIConstants.SCHEDULINGTYPE_SPECIAL)
                {
                    e.Row.Cells[5].ForeColor = Color.Brown;
                }
                else if (schedulingView.SchedulingType == string.Empty)//合并的
                {
                    e.Row.Cells[5].ForeColor = Color.Purple;
                }
            }
            if (!this.IsExport)
            {
                if (!string.IsNullOrEmpty(schedulingView.StartUser))
                {
                    e.Row.Cells[5].ToolTip = schedulingView.SchedulingType;
                }
            }
        }
    }
}