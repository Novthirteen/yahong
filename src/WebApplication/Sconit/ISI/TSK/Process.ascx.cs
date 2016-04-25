using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.Control;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TSK_Process : com.Sconit.Web.MainModuleBase
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
        this.lgd.InnerText = "${ISI.TSK." + this.ModuleType + "}" + this.TaskCode;
        this.btnSearch_Click(null, null);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private void PageCleanup()
    {
        this.TaskCode = null;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {

        DateTime? startTime = null;
        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        DateTime? endTime = null;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endTime = DateTime.Parse(this.tbEndDate.Text.Trim());
        }

        DetachedCriteria criteria = DetachedCriteria.For(typeof(WFDetail));
        criteria.Add(Expression.Eq("TaskCode", this.TaskCode));
        if (startTime.HasValue)
        {
            criteria.Add(Expression.Ge("CreateDate", startTime));
        }
        if (endTime.HasValue)
        {
            criteria.Add(Expression.Le("CreateDate", endTime));
        }
        criteria.AddOrder(Order.Desc("CreateDate"));
        criteria.AddOrder(Order.Desc("Id"));
        IList<WFDetail> wfDetailList = TheCriteriaMgr.FindAll<WFDetail>(criteria, 0, 500);
        if ((System.Web.UI.WebControls.Button)sender != this.btnExport && rblViewType.SelectedValue == "Tree")
        {
            dsNormal.Visible = false;
            try
            {
                if (wfDetailList != null && wfDetailList.Count > 0)
                {
                    IList<WFDetail> wfDetailListTree = wfDetailList.Reverse().ToList<WFDetail>();
                    WFDetail wf = wfDetailListTree[0];
                    com.Sconit.Control.MyOrgNode rootNode = GetOrgNode(wf);
                    if (wfDetailList.Count > 1)
                    {
                        createChildNode(wfDetailListTree.Skip(1).ToList<WFDetail>(), rootNode, wf);
                    }
                    if (rootNode != null)
                    {
                        OrgChartTreeView.Node = rootNode;
                    }
                }
                this.dsTree.Visible = true;
            }
            catch
            {
                this.dsTree.Visible = false;
            }
        }
        else
        {
            this.dsTree.Visible = false;
            this.dsNormal.Visible = true;
            this.GV_List_Process.DataSource = wfDetailList;
            this.GV_List_Process.DataBind();
        }
        if ((System.Web.UI.WebControls.Button)sender == this.btnExport)
        {
            this.ExportXLS(this.GV_List_Process);
        }
    }
    private MyOrgNode GetOrgNode(WFDetail wf)
    {
        return GetOrgNode(wf, null);
    }
    private MyOrgNode GetOrgNode(WFDetail wf, DateTime? lastDate)
    {
        com.Sconit.Control.MyOrgNode subOrgNode = new com.Sconit.Control.MyOrgNode();
        if (lastDate.HasValue)
        {
            string diff = ISIUtil.GetDiff(wf.CreateDate, lastDate.Value);
            subOrgNode.Code = !string.IsNullOrEmpty(diff) ? diff : "0秒";
        }
        else
        {
            subOrgNode.Code = wf.TaskCode;
        }
        subOrgNode.Name = wf.CreateUserNm;
        subOrgNode.Memo1 = wf.Status;
        subOrgNode.Memo2 = wf.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
        return subOrgNode;
    }

    private void createChildNode(IList<WFDetail> wfDetailList, MyOrgNode parentnode, WFDetail src)
    {
        for (int i = 0; i < wfDetailList.Count; i++)
        {
            WFDetail wf = wfDetailList[i];
            if (src.Id != wf.Id && wf.CreateDate >= src.CreateDate)
            {
                MyOrgNode subOrgNode = GetOrgNode(wf, src.CreateDate);
                parentnode.Nodes.Add(subOrgNode);
                createChildNode(wfDetailList, subOrgNode, wf);
                break;
            }
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
            WFDetail wfDetail = (WFDetail)e.Row.DataItem;

            e.Row.Cells[5].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");

            if (!string.IsNullOrEmpty(wfDetail.Status))
            {
                e.Row.Cells[0].Text = "${ISI.Status." + wfDetail.Status + "}";
            }
            if (wfDetail.Level.HasValue && wfDetail.Level.Value > 0)
            {
                e.Row.Cells[1].Text = "${" + wfDetail.Level.Value.ToString() + "}";
            }
        }
    }
}