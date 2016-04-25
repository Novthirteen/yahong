using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Task_List : ListModuleBase
{
    public EventHandler EditEvent;

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string taskCode = ((LinkButton)sender).CommandArgument;
            EditEvent(taskCode, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        TaskMstr task = this.TheTaskMstrMgr.LoadTaskMstr(code);
        try
        {
            TheTaskMgr.DeleteTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Delete" + task.Type + ".Successfully", code);
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            //TheTaskMgr.DeleteTaskMstr(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Delete" + task.Type + ".Fail", code);
        }
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            string startUsers = string.Empty;
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                if (!string.IsNullOrEmpty(row.Cells[10].Text) && row.Cells[10].Text != "&nbsp;")
                {
                    startUsers += row.Cells[10].Text + ";";
                }
            }

            IDictionary<string, string> startUserDic = null;
            if (!string.IsNullOrEmpty(startUsers))
            {
                startUserDic = this.TheTaskMstrMgr.GetUser(startUsers);

                foreach (GridViewRow row in this.GV_List.Rows)
                {
                    string startUser = row.Cells[10].Text;
                    if (!string.IsNullOrEmpty(startUser))
                    {
                        string[] userCodes = startUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                        startUser = string.Empty;
                        if (userCodes != null && userCodes.Length > 0)
                        {
                            foreach (string userCode in userCodes)
                            {
                                if (startUserDic.Keys.Contains(userCode))
                                {
                                    if (!string.IsNullOrEmpty(startUser))
                                    {
                                        startUser += ", ";
                                    }
                                    startUser += startUserDic[userCode].Trim();
                                }
                            }

                            row.Cells[10].Text = startUser;
                        }
                    }
                }
            }
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        TaskView task = (TaskView)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(task.Status, this.CurrentUser);

            if (!string.IsNullOrEmpty(task.Color))
            {
                e.Row.Cells[12].Attributes["style"] = "background-color:" + task.Color;
            }
            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[1].ToolTip = task.Priority;
            }
            if (!string.IsNullOrEmpty(task.SubmitUser))
            {
                e.Row.Cells[8].ToolTip = task.SubmitUser;
            }
            if (!string.IsNullOrEmpty(task.AssignUser))
            {
                e.Row.Cells[9].ToolTip = task.AssignUser;
            }
            if (!string.IsNullOrEmpty(task.StartUser))
            {
                e.Row.Cells[10].ToolTip = task.StartUser;
            }
            e.Row.Cells[2].ToolTip = task.TaskSubTypeDesc;
            e.Row.Cells[3].Text = ISIUtil.GetStrLength(task.Subject, 30);
            e.Row.Cells[3].ToolTip = task.Subject;
            e.Row.Cells[5].Text = ISIUtil.GetStrLength(task.TaskAddress, 18);
            e.Row.Cells[5].ToolTip = task.TaskAddress;
            e.Row.Cells[6].Text = ISIUtil.GetStrLength(task.Desc1, 50);
            e.Row.Cells[6].ToolTip = task.Desc1;

            e.Row.Cells[10].ToolTip = ISIUtil.ShowUser(task.StartedUser);
        }
    }

    public void Export()
    {
        this.ExportXLS(GV_List);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }
}
