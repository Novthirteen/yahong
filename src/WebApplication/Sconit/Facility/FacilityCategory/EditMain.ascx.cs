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

public partial class Facility_FacilityCategory_EditMain : MainModuleBase
{
    public event EventHandler BackEvent;

    protected string FacilityCategoryCode
    {
        get
        {
            return (string)ViewState["FacilityCategoryCode"];
        }
        set
        {
            ViewState["FacilityCategoryCode"] = value;
        }
    }

    public void InitPageParameter(string code)
    {
        this.FacilityCategoryCode = code;
        this.ucEdit.InitPageParameter(code);
        this.ucTabNavigator.UpdateView();
        this.ucTabNavigator.UpdateAttachmentTitle(code);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lbFacilityCategoryClickEvent += new System.EventHandler(this.TabFacilityCategoryClick_Render);
        this.ucTabNavigator.lbFacilityCategoryAttachmentClickEvent += new System.EventHandler(this.TabFacilityCategoryAttachmentClick_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
    }

    protected void TabFacilityCategoryClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucAttachment.Visible = false;
    }

    protected void TabFacilityCategoryAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucAttachment.InitPageParameter(this.FacilityCategoryCode);
    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.FacilityCategoryCode);
    }


    protected void EditBack_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

}
