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
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TSK_List : ListModuleBase
{

    public EventHandler EditEvent;

    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateView()
    {
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
        {
            this.GV_List.Columns[2].HeaderText = "${ISI.TSK.PrjIss.Subject}";
            this.GV_List.Columns[4].HeaderText = "${ISI.TSK.Project}";
        }
        else
        {
            this.GV_List.Columns[2].HeaderText = "${ISI.TSK.Subject}";
            this.GV_List.Columns[4].HeaderText = "${ISI.TSK.TaskSubType}";
        }
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

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TaskMstr task = (TaskMstr)e.Row.DataItem;

            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(task.Status, this.CurrentUser);

            if (!this.IsExport && !string.IsNullOrEmpty(task.Color))
            {
                e.Row.Cells[10].Attributes["style"] = "background-color:" + task.Color;
            }

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE
                    && this.TheTaskMgr.HasPermission(task, this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN), false, false, false, this.CurrentUser.Code))
            {
                LinkButton lbtnDelete = (LinkButton)e.Row.FindControl("lbtnDelete");
                if (lbtnDelete != null)
                {
                    lbtnDelete.Visible = true;
                }
            }

            //e.Row.Cells[2].Text = ISIUtil.GetStrLength(task.Subject, 40);
            //e.Row.Cells[6].Text = ISIUtil.GetStrLength(task.TaskAddress, 20);
            //e.Row.Cells[9].ToolTip = ISIUtil.ShowUser(task.StartedUser);

            e.Row.Cells[2].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[9].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
        }
    }


    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            string startUsers = string.Empty;

            this.GV_List.Columns[5].Visible = (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT);
            this.GV_List.Columns[6].Visible = (this.ModuleType != ISIConstants.ISI_TASK_TYPE_PROJECT);
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                Label lblStartedUser = (Label)(row.FindControl("lblStartedUser"));
                if (!string.IsNullOrEmpty(lblStartedUser.Text) && lblStartedUser.Text != "&nbsp;")
                {
                    startUsers += lblStartedUser.Text + ";";
                }
            }

            IDictionary<string, string> startUserDic = null;
            if (!string.IsNullOrEmpty(startUsers))
            {
                startUserDic = this.TheTaskMstrMgr.GetUser(startUsers);

                foreach (GridViewRow row in this.GV_List.Rows)
                {
                    Label lblStartedUser = ((Label)(row.FindControl("lblStartedUser")));
                    string startUser = lblStartedUser.Text;
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
                            lblStartedUser.Text = startUser;
                        }
                    }
                }
            }
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            TheTaskMgr.DeleteTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Delete" + this.ModuleType + ".Successfully", code);
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            //TheTaskMgr.DeleteTaskMstr(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Delete" + this.ModuleType + ".Fail", code);
        }
    }
}
