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
using com.Sconit.Control;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_ProjectTask_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    public void PageCleanup()
    {
        tbSubject.Text = string.Empty;
        tbDesc.Text = string.Empty;
        tbExpectedResults.Text = string.Empty;
        tbSeq.Text = string.Empty;
        ddlProjectType.SelectedIndex = 0;
        ddlProjectSubType.SelectedIndex = 0;
        ddlPhase.SelectedIndex = 0;

    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (CreateEvent != null)
        {
            ProjectTask task = new ProjectTask();
            if (ddlPhase.SelectedIndex != -1)
            {
                task.Phase = ddlPhase.SelectedValue;
            }
            if (ddlProjectType.SelectedIndex != -1)
            {
                task.ProjectType = ddlProjectType.SelectedValue;
            }
            if (ddlProjectSubType.SelectedIndex != -1)
            {
                task.ProjectSubType = ddlProjectSubType.SelectedValue;
            }
            task.Seq = tbSeq.Text.Trim();
            task.Desc = tbDesc.Text.Trim();
            task.Subject = tbSubject.Text.Trim();
            task.ExpectedResults = tbExpectedResults.Text.Trim();
            task.IsActive = ckIsActive.Checked;
            this.TheProjectTaskMgr.CreateProjectTask(task, this.CurrentUser);
            CreateEvent(task.Id, e);

            ShowSuccessMessage("ISI.ProjectTask.Add.Successfully", task.Subject);
        }
    }
}
