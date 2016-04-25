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
using NHibernate.Expression;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

public partial class Facility_MaintainPlan_EditMain : MainModuleBase
{
    public event EventHandler BackEvent;

    protected string MaintainPlanCode
    {
        get
        {
            return (string)ViewState["MaintainPlanCode"];
        }
        set
        {
            ViewState["MaintainPlanCode"] = value;
        }
    }

    public void InitPageParameter(string mpCode)
    {
        this.MaintainPlanCode = mpCode;
        this.ucEdit.InitPageParameter(mpCode);
        this.ucTabNavigator.UpdateView();
        this.ucTabNavigator.UpdateAttachmentTitle(mpCode);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lbMaintainPlanClickEvent += new System.EventHandler(this.TabMaintainPlanClick_Render);
        this.ucTabNavigator.lbMaintainPlanAttachmentClickEvent += new System.EventHandler(this.TabMaintainPlanAttachmentClick_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
    }

    protected void TabMaintainPlanClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucAttachment.Visible = false;
    }

    protected void TabMaintainPlanAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucAttachment.InitPageParameter(this.MaintainPlanCode);
    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.MaintainPlanCode);
    }


    protected void EditBack_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

}
