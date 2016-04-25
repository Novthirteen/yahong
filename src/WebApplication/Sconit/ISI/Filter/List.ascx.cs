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
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;


public partial class ISI_Filter_List : ListModuleBase
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
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        try
        {
            TheFilterMgr.DeleteFilter(int.Parse(id));
            ShowSuccessMessage("ISI.Filter.DeleteFilter.Successfully");
            UpdateView();

        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.Filter.DeleteFilter.Fail");
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Filter filter = (Filter)e.Row.DataItem;

            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            if (!string.IsNullOrEmpty(filter.TaskType))
            {
                e.Row.Cells[5].Text = "${ISI.TSK." + filter.TaskType + "}";
            }

            if (this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_PERMISSION_FILTER_VALUE_FILTERADMIN)
                    || filter.CreateUser == this.CurrentUser.Code || filter.LastModifyUser == this.CurrentUser.Code)
            {
                var lbtnDelete = e.Row.FindControl("lbtnDelete");
                lbtnDelete.Visible = true;
            }
        }
    }
}
