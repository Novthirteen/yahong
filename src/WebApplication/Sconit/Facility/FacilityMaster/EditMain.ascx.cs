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

public partial class Facility_FacilityMaster_EditMain : MainModuleBase
{
    public event EventHandler BackEvent;

    protected string FCID
    {
        get
        {
            return (string)ViewState["FCID"];
        }
        set
        {
            ViewState["FCID"] = value;
        }
    }

    public void InitPageParameter(string fcId)
    {
        this.FCID = fcId;
        this.ucEdit.InitPageParameter(fcId);
        this.ucMaintain.InitPageParameter(fcId);
        this.ucTabNavigator.UpdateView(fcId);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lbFacilityMasterClickEvent += new System.EventHandler(this.TabFacilityMasterClick_Render);
        this.ucTabNavigator.lbFacilityMaintainClickEvent += new System.EventHandler(this.TabFacilityMaintainClick_Render);
        this.ucTabNavigator.lbFacilityTransClickEvent += new System.EventHandler(this.TabFacilityTransClick_Render);
        this.ucTabNavigator.lbFacilityAttachmentClickEvent += new System.EventHandler(this.TabFacilityAttachmentClick_Render);
        this.ucTabNavigator.lbFacilityTemplateClickEvent += new System.EventHandler(this.TabFacilityTemplateClick_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);
        this.ucTemplate.UpdateTemplateTitleEvent += new System.EventHandler(this.UpdateTemplateTitle_Render);
        this.ucTemplate.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucMaintain.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucTrans.BackEvent += new System.EventHandler(this.EditBack_Render);
    }

    protected void TabFacilityMasterClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucMaintain.Visible = false;
        this.ucTrans.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucTemplate.Visible = false;

    }

    protected void TabFacilityMaintainClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucMaintain.Visible = true;
        this.ucTrans.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucTemplate.Visible = false;
        this.ucMaintain.InitPageParameter(this.FCID);

    }

    protected void TabFacilityTransClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucMaintain.Visible = false;
        this.ucTrans.Visible = true;
        this.ucAttachment.Visible = false;
        this.ucTemplate.Visible = false;
        this.ucTrans.InitPageParameter(this.FCID);
    }

    protected void TabFacilityAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucMaintain.Visible = false;
        this.ucTrans.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucTemplate.Visible = false;
        this.ucAttachment.InitPageParameter(this.FCID);

    }

    protected void TabFacilityTemplateClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucMaintain.Visible = false;
        this.ucTrans.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucTemplate.Visible = true;
        this.ucTemplate.InitPageParameter(this.FCID);

    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.FCID);
    }

    protected void UpdateTemplateTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateTemplateTitle(this.FCID);
    }

    protected void EditBack_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

}
