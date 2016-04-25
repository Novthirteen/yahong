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
using com.Sconit.Entity;
using NHibernate.Expression;

public partial class ISI_MyTask_Main : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lblMstrClickEvent += new System.EventHandler(this.TabMstrClick_Render);
        this.ucTabNavigator.lblCreateClickEvent += new System.EventHandler(this.TabCreateClick_Render);
        this.ucTabNavigator.lblStartClickEvent += new System.EventHandler(this.TabStartClick_Render);
        this.ucTabNavigator.lblSubmitClickEvent += new System.EventHandler(this.TabSubmitClick_Render);
        this.ucTabNavigator.lblCompleteClickEvent += new System.EventHandler(this.TabCompleteClick_Render);
        this.ucTabNavigator.lblAssignClickEvent += new System.EventHandler(this.TabAssignClick_Render);

        this.ucMstr.UpdateTitleEvent += new System.EventHandler(this.UpdateTitle_Render);
        this.ucCreate.UpdateTitleEvent += new System.EventHandler(this.UpdateTitle_Render);
        this.ucStart.UpdateTitleEvent += new System.EventHandler(this.UpdateTitle_Render);
        this.ucSubmit.UpdateTitleEvent += new System.EventHandler(this.UpdateTitle_Render);
        this.ucComplete.UpdateTitleEvent += new System.EventHandler(this.UpdateTitle_Render);
        this.ucAssign.UpdateTitleEvent += new System.EventHandler(this.UpdateTitle_Render);
        if (!IsPostBack)
        {
            
        }
    }


    protected void UpdateTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateTitle();
    }

    protected void TabCreateClick_Render(object sender, EventArgs e)
    {
        this.ucMstr.Visible = false;
        this.ucCreate.Visible = true;
        this.ucCreate.InitPageParameter();
        this.ucSubmit.Visible = false;
        this.ucAssign.Visible = false;
        this.ucComplete.Visible = false;
        this.ucStart.Visible = false;
    }


    protected void TabAssignClick_Render(object sender, EventArgs e)
    {
        this.ucMstr.Visible = false;
        this.ucCreate.Visible = false;
        this.ucSubmit.Visible = false;
        this.ucAssign.Visible = true;
        this.ucAssign.InitPageParameter();
        this.ucComplete.Visible = false;
        this.ucStart.Visible = false;
    }

    protected void TabCompleteClick_Render(object sender, EventArgs e)
    {
        this.ucMstr.Visible = false;
        this.ucCreate.Visible = false;
        this.ucSubmit.Visible = false;
        this.ucAssign.Visible = false;
        this.ucComplete.Visible = true;
        this.ucComplete.InitPageParameter();
        this.ucStart.Visible = false;
    }


    protected void TabSubmitClick_Render(object sender, EventArgs e)
    {
        this.ucMstr.Visible = false;
        this.ucCreate.Visible = false;
        this.ucSubmit.Visible = true;
        this.ucSubmit.InitPageParameter();
        this.ucAssign.Visible = false;
        this.ucComplete.Visible = false;
        this.ucStart.Visible = false;
    }

    protected void TabStartClick_Render(object sender, EventArgs e)
    {
        this.ucMstr.Visible = false;
        this.ucCreate.Visible = false;
        this.ucSubmit.Visible = false;
        this.ucAssign.Visible = false;
        this.ucComplete.Visible = false;
        this.ucStart.Visible = true;
        this.ucStart.InitPageParameter();
    }

    protected void TabMstrClick_Render(object sender, EventArgs e)
    {
        this.ucMstr.Visible = true;
        this.ucMstr.InitPageParameter();
        this.ucCreate.Visible = false;
        this.ucSubmit.Visible = false;
        this.ucAssign.Visible = false;
        this.ucComplete.Visible = false;
        this.ucStart.Visible = false;
    }
}
