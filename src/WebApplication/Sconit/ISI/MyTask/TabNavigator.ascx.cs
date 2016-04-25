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
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_MyTask_TabNavigator : com.Sconit.Web.ModuleBase
{
    public event EventHandler lblMstrClickEvent;
    public event EventHandler lblAssignClickEvent;
    public event EventHandler lblStartClickEvent;
    public event EventHandler lblSubmitClickEvent;
    public event EventHandler lblCreateClickEvent;
    public event EventHandler lblCompleteClickEvent;

    public void UpdateView()
    {
        lbMstr_Click(this, null);
        UpdateTitle();
    }

    public void UpdateTitle()
    {
        int createCount = this.TheTaskMgr.CountTask(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE, this.CurrentUser);
        lbCreate.Text = "${ISI.MyTask.Create}" + (createCount == 0 ? string.Empty : "(<font color='blue'>" + createCount.ToString() + "</font>)");
        int submitCount = this.TheTaskMgr.CountTask(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT, this.CurrentUser);
        lbSubmit.Text = "${ISI.MyTask.Submit}" + (submitCount == 0 ? string.Empty : "(<font color='blue'>" + submitCount.ToString() + "</font>)");
        int assignCount = this.TheTaskMgr.CountTask(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN, this.CurrentUser);
        lbAssign.Text = "${ISI.MyTask.Assign}" + (assignCount == 0 ? string.Empty : "(<font color='blue'>" + assignCount.ToString() + "</font>)");
        int inprocessCount = this.TheTaskMgr.CountTask(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS, this.CurrentUser);
        lbStart.Text = "${ISI.MyTask.Start}" + (inprocessCount == 0 ? string.Empty : "(<font color='blue'>" + inprocessCount.ToString() + "</font>)");
        int completeCount = this.TheTaskMgr.CountTask(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE, this.CurrentUser);
        lbComplete.Text = "${ISI.MyTask.Complete}" + (completeCount == 0 ? string.Empty : "(<font color='blue'>" + completeCount.ToString() + "</font>)");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        /*
        this.lbMstr.Text = FlowHelper.GetFlowLabel(this.ModuleType);
        this.lbStrategy.Text = FlowHelper.GetFlowStrategyLabel(this.ModuleType);
        this.lbDetail.Text = FlowHelper.GetFlowDetailLabel(this.ModuleType);
        this.lbRouting.Text = FlowHelper.GetFlowRoutingLabel(this.ModuleType);
        */
        if (!IsPostBack)
        {
            //UpdateView();
            //lbMstr_Click(sender, e);
        }
    }

    protected void lbCreate_Click(object sender, EventArgs e)
    {
        if (lblCreateClickEvent != null)
        {
            lblCreateClickEvent(this, e);
        }

        this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
        this.tab_create.Attributes["class"] = "ajax__tab_active";
        this.tab_submit.Attributes["class"] = "ajax__tab_inactive";
        this.tab_assign.Attributes["class"] = "ajax__tab_inactive";
        this.tab_start.Attributes["class"] = "ajax__tab_inactive";
        this.tab_complete.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbMstr_Click(object sender, EventArgs e)
    {
        if (lblMstrClickEvent != null)
        {
            lblMstrClickEvent(this, e);
        }

        this.tab_mstr.Attributes["class"] = "ajax__tab_active";
        this.tab_create.Attributes["class"] = "ajax__tab_inactive";
        this.tab_submit.Attributes["class"] = "ajax__tab_inactive";
        this.tab_assign.Attributes["class"] = "ajax__tab_inactive";
        this.tab_start.Attributes["class"] = "ajax__tab_inactive";

        this.tab_complete.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbSubmit_Click(object sender, EventArgs e)
    {
        if (lblSubmitClickEvent != null)
        {
            lblSubmitClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_create.Attributes["class"] = "ajax__tab_inactive";
            this.tab_submit.Attributes["class"] = "ajax__tab_active";
            this.tab_assign.Attributes["class"] = "ajax__tab_inactive";
            this.tab_start.Attributes["class"] = "ajax__tab_inactive";

            this.tab_complete.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbAssign_Click(object sender, EventArgs e)
    {
        if (lblAssignClickEvent != null)
        {
            lblAssignClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_create.Attributes["class"] = "ajax__tab_inactive";
            this.tab_submit.Attributes["class"] = "ajax__tab_inactive";
            this.tab_assign.Attributes["class"] = "ajax__tab_active";
            this.tab_start.Attributes["class"] = "ajax__tab_inactive";

            this.tab_complete.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbStart_Click(object sender, EventArgs e)
    {
        if (lblStartClickEvent != null)
        {
            lblStartClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_create.Attributes["class"] = "ajax__tab_inactive";
            this.tab_submit.Attributes["class"] = "ajax__tab_inactive";
            this.tab_assign.Attributes["class"] = "ajax__tab_inactive";
            this.tab_start.Attributes["class"] = "ajax__tab_active";

            this.tab_complete.Attributes["class"] = "ajax__tab_inactive";
        }
    }


    protected void lbComplete_Click(object sender, EventArgs e)
    {
        if (lblCompleteClickEvent != null)
        {
            lblCompleteClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_create.Attributes["class"] = "ajax__tab_inactive";
            this.tab_submit.Attributes["class"] = "ajax__tab_inactive";
            this.tab_assign.Attributes["class"] = "ajax__tab_inactive";
            this.tab_start.Attributes["class"] = "ajax__tab_inactive";

            this.tab_complete.Attributes["class"] = "ajax__tab_active";
        }
    }
}
