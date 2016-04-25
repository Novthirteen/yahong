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

public partial class ISI_TSK_ResMatrixDet : com.Sconit.Web.MainModuleBase
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


    public void InitPageParameter(string taskCode)
    {
        this.TaskCode = taskCode;
        if (!string.IsNullOrEmpty(TaskCode))
        {
            this.lgd.InnerText = this.TaskCode;

            var resTaskDets = TheGenericMgr.FindAll<ResTaskDet>(" from ResTaskDet where TaskCode=? order by Id desc",
                new object[] { taskCode }, 0, 500);

            this.GV_List_Detail.DataSource = resTaskDets;
            this.GV_List_Detail.DataBind();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    private void PageCleanup()
    {
        this.TaskCode = null;
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
            e.Row.Cells[0].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[1].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");

            ResTaskDet wfDetail = (ResTaskDet)e.Row.DataItem;
            Label lblDesc = (Label)e.Row.FindControl("lblDesc1");
            lblDesc.Text = ISIUtil.GetHtmlBody(wfDetail.Desc1);
        }
    }
}