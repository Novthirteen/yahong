using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TSK_ProcessInstance : com.Sconit.Web.MainModuleBase
{
    public event EventHandler BackEvent;
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

    public string TaskCode
    {
        get
        {
            return (string)ViewState["TaskCode"];
        }
        set
        {
            ViewState["TaskCode"] = value;
        }
    }

    public int? CurrentLevel
    {
        get
        {
            if (ViewState["CurrentLevel"] == null)
            {
                return null;
            }
            else
            {
                return int.Parse(ViewState["CurrentLevel"].ToString());
            }
        }
        set
        {
            ViewState["CurrentLevel"] = value;
        }
    }

    public void InitPageParameter(string taskCode)
    {
        this.TaskCode = taskCode;
        if (!string.IsNullOrEmpty(TaskCode))
        {
            this.lgd.InnerText = "${ISI.TSK." + this.ModuleType + "}" + this.TaskCode;
            var level = TheHqlMgr.FindAll<object>("select t.Level from TaskMstr t where t.Code ='" + taskCode + "'")[0];
            if (level == null)
            {
                this.CurrentLevel = null;
            }
            else
            {
                this.CurrentLevel = int.Parse(level.ToString());
            }
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ProcessInstance));
            criteria.Add(Expression.Eq("TaskCode", this.TaskCode));
            criteria.AddOrder(Order.Desc("Level"));
            this.GV_List_Detail.DataSource = TheCriteriaMgr.FindAll<ProcessInstance>(criteria, 0, 500);
            this.GV_List_Detail.DataBind();

            this.GV_List_Process.DataSource = TheHqlMgr.FindAll<WFDetail>("from WFDetail where TaskCode ='" + taskCode + "' order by Id Desc ");
            this.GV_List_Process.DataBind();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {


    }
    private void PageCleanup()
    {
        this.TaskCode = null;
        this.CurrentLevel = null;
        this.ModuleType = string.Empty;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }



    protected void GV_List_DataBound(object sender, EventArgs e)
    {

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ProcessInstance processInstance = (ProcessInstance)e.Row.DataItem;
            e.Row.Cells[3].Text = "${ISI.Status." + processInstance.Status + "}";
            var level = processInstance.Level / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
            e.Row.Cells[4].Text = "${" + level + "}" + (level != processInstance.Level ? "&nbsp;" + (processInstance.Level / ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL).ToString().Substring(1) : string.Empty);

            if (CurrentLevel.HasValue && processInstance.Level == CurrentLevel.Value && !processInstance.ProcessDate.HasValue)
            {
                e.Row.ForeColor = System.Drawing.Color.Blue;
            }
        }
    }

    protected void GV_List_Process_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            WFDetail wfDetail = (WFDetail)e.Row.DataItem;

            e.Row.Cells[5].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");

            if (!string.IsNullOrEmpty(wfDetail.Status))
            {
                e.Row.Cells[0].Text = "${ISI.Status." + wfDetail.Status + "}";
            }
            if (wfDetail.PreLevel.HasValue && wfDetail.PreLevel.Value > 0)
            {
                var level = wfDetail.PreLevel / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                e.Row.Cells[1].Text = "${" + level + "}" + (level != wfDetail.PreLevel ? "&nbsp;" + (wfDetail.PreLevel / ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL).ToString().Substring(1) : string.Empty);
            }
            if (wfDetail.Level.HasValue && wfDetail.Level.Value > 0)
            {
                var level = wfDetail.Level / ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL * ISIConstants.CODE_MASTER_WFS_LEVEL_INTERVAL;
                e.Row.Cells[2].Text = "${" + level + "}" + (level != wfDetail.Level ? "&nbsp;" + (wfDetail.Level / ISIConstants.CODE_MASTER_WF_COUNTERSIGN_LEVEL_INTERVAL).ToString().Substring(1) : string.Empty);
            }
        }
    }
}