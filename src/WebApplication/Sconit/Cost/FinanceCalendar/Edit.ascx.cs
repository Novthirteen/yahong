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
using com.Sconit.Entity.MasterData;
using com.Sconit.Control;
using com.Sconit.Utility;
using com.Sconit.Entity.Cost;

public partial class Cost_FinanceCalendar_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected Int32 Id
    {
        get
        {
            return (Int32)ViewState["Id"];
        }
        set
        {
            ViewState["Id"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }


    public void InitPageParameter(Int32 id)
    {
        this.Id = id;
        this.ODS_FinanceCalendar.SelectParameters["id"].DefaultValue = this.Id.ToString();
        this.ODS_FinanceCalendar.DataBind();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }
    protected void FV_FinanceCalendar_DataBound(object sender, EventArgs e)
    {
        FinanceCalendar financeCalendar = (FinanceCalendar)((FormView)sender).DataItem;
        ((TextBox)(this.FV_FinanceCalendar.FindControl("tbStartDate"))).Text = financeCalendar.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
        ((TextBox)(this.FV_FinanceCalendar.FindControl("tbEndDate"))).Text = financeCalendar.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
    }
    protected void ODS_FinanceCalendar_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
       
    }
    protected void ODS_FinanceCalendar_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("Cost.FinanceCalendar.Update.Successfully");
    }

    protected void ODS_FinanceCalendar_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        try
        {
           
            ShowSuccessMessage("Cost.FinanceCalendar.Delete.Successfully");
            if (BackEvent != null)
            {
                BackEvent(this, e);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Cost.FinanceCalendar.Delete.Failed");
        }
    }

}
