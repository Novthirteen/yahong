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

public partial class ISI_ProjectTask_List : ListModuleBase
{

    public EventHandler EditEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string id = ((LinkButton)sender).CommandArgument;
            EditEvent(id, e);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ProjectTask task = (ProjectTask)e.Row.DataItem;
        }
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {

        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        try
        {
            TheProjectTaskMgr.DeleteProjectTask(int.Parse(id), this.CurrentUser);
            ShowSuccessMessage("ISI.ProjectTask.Delete.Successfully");
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.ProjectTask.Delete.Fail");
        }
    }
}
