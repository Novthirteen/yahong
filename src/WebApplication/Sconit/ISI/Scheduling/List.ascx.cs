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
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;


public partial class ISI_Scheduling_List : ListModuleBase
{
    public EventHandler EditEvent;
    public bool IsSpecial
    {
        get
        {
            return (bool)ViewState["IsSpecial"];
        }
        set
        {
            ViewState["IsSpecial"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateView()
    {
        if (!this.IsSpecial)
        {
            this.GV_List.Columns[1].Visible = true;
            this.GV_List.Columns[2].Visible = true;
            this.GV_List.Columns[8].Visible = false;
            this.GV_List.Columns[9].Visible = false;
        }
        else
        {
            this.GV_List.Columns[1].Visible = false;
            this.GV_List.Columns[2].Visible = false;
            this.GV_List.Columns[8].Visible = true;
            this.GV_List.Columns[9].Visible = true;
        }
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
            Scheduling scheduling = (Scheduling)e.Row.DataItem;
            e.Row.Cells[6].Text = this.TheUserSubscriptionMgr.GetUserName(scheduling.StartUser);
            if (!this.IsExport)
            {
                e.Row.Cells[6].ToolTip = ISIUtil.ShowUser(scheduling.StartUser);
            }

            string week = e.Row.Cells[1].Text;
            switch (week)
            {
                case "Monday":
                    e.Row.Cells[1].Text = "${Common.Week.Monday}";
                    break;
                case "Tuesday":
                    e.Row.Cells[1].Text = "${Common.Week.Tuesday}";
                    break;
                case "Wednesday":
                    e.Row.Cells[1].Text = "${Common.Week.Wednesday}";
                    break;
                case "Thursday":
                    e.Row.Cells[1].Text = "${Common.Week.Thursday}";
                    break;
                case "Friday":
                    e.Row.Cells[1].Text = "${Common.Week.Friday}";
                    break;
                case "Saturday":
                    e.Row.Cells[1].Text = "${Common.Week.Saturday}";
                    break;
                case "Sunday":
                    e.Row.Cells[1].Text = "${Common.Week.Sunday}";
                    break;
                default:
                    break;
            }
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        try
        {
            //IList<Scheduling> taskAddressList = TheSchedulingMgr.GetSchedulingByParent(code);
            //if (taskAddressList != null && taskAddressList.Count > 0)
            /*
             if (TheSchedulingMgr.IsRef(code))
             {
                 ShowErrorMessage("ISI.Scheduling.DeleteScheduling.Ref.Fail", code);
             }
             else
             */
            {
                TheSchedulingMgr.DeleteScheduling(int.Parse(id));
                ShowSuccessMessage("ISI.Scheduling.DeleteScheduling.Successfully");
                UpdateView();
            }
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.Scheduling.DeleteScheduling.Fail");
        }

    }
}
