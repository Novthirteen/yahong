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

public partial class ISI_MyTask_List : ListModuleBase
{
    public string tabAction { get; set; }
    public EventHandler EditEvent;

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string[] task = ((LinkButton)sender).CommandArgument.Split(new char[] { '|' });
            EditEvent(task, e);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TaskMstr task = (TaskMstr)e.Row.DataItem;

            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage("ISI.Status." + task.Status, this.CurrentUser);
            if (!this.IsExport && !string.IsNullOrEmpty(task.Color))
            {
                e.Row.Cells[10].Attributes["style"] = "background-color:" + task.Color;
            }
            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                LinkButton lbtnEdit = (LinkButton)(e.Row.FindControl("lbtnEdit"));
                lbtnEdit.ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[1].ToolTip = task.Priority;
            }
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                    && this.TheTaskMgr.HasPermission(task, this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), false, false, this.CurrentUser.Code))
            {
                LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
                if (lbtnDelete != null)
                {
                    lbtnDelete.Visible = true;
                }
            }

            //e.Row.Cells[2].Text = ISIUtil.GetStrLength(task.Subject, 40);
            //e.Row.Cells[4].Text = ISIUtil.GetStrLength(task.TaskAddress, 20);
            //e.Row.Cells[7].ToolTip = ISIUtil.ShowUser(task.StartedUser);

            e.Row.Cells[2].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[4].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[7].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
        }
    }


    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (this.tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
        {
            GV_List.Columns[11].Visible = true;
        }
        else
        {
            GV_List.Columns[11].Visible = false;
        }

        if (this.tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                || this.tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
        {
            GV_List.Columns[6].Visible = true;
            GV_List.Columns[7].Visible = false;
            GV_List.Columns[8].Visible = false;
            GV_List.Columns[9].Visible = false;
        }
        else
        {
            if (this.tabAction != string.Empty)
            {
                GV_List.Columns[6].Visible = false;
            }
            GV_List.Columns[7].Visible = true;
            GV_List.Columns[8].Visible = true;
            GV_List.Columns[9].Visible = true;
        }

        if (this.tabAction != string.Empty)
        {
            GV_List.Columns[5].Visible = false;
        }


        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            string startUsers = string.Empty;
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                if (!string.IsNullOrEmpty(row.Cells[7].Text) && row.Cells[7].Text != "&nbsp;")
                {
                    startUsers += row.Cells[7].Text + ";";
                }
            }

            IDictionary<string, string> startUserDic = null;
            if (!string.IsNullOrEmpty(startUsers))
            {
                startUserDic = this.TheTaskMstrMgr.GetUser(startUsers);

                foreach (GridViewRow row in this.GV_List.Rows)
                {
                    string startUser = row.Cells[7].Text;
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

                            row.Cells[7].Text = startUser;
                        }
                    }
                }
            }
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

}
