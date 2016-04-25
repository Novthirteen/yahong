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

public partial class Facility_FacilityDistribution_EditMain : MainModuleBase
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

    public void InitPageParameter(Int32 id)
    {
        this.Id = id;
        this.ucEdit.InitPageParameter(id);
        this.ucDetail.InitPageParameter(id);
        this.ucAttachment.InitPageParameter(id.ToString());
        this.ucTabNavigator.UpdateView(id.ToString());

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lbFacilityDistributionClickEvent += new System.EventHandler(this.TabFacilityDistributionClick_Render);
        this.ucTabNavigator.lbFacilityDistributionDetailClickEvent += new System.EventHandler(this.TabFacilityDistributionDetailClick_Render);
        this.ucTabNavigator.lbFacilityDistributionAttachmentClickEvent += new System.EventHandler(this.TabFacilityDistributionAttachmentClick_Render);

        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);

        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucDetail.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.EditBack_Render);
    }

    protected void TabFacilityDistributionClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucDetail.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucEdit.UpdateView();
    }

    protected void TabFacilityDistributionDetailClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = true;
        this.ucAttachment.Visible = false;
        this.ucDetail.InitPageParameter(this.Id);
    }


    protected void TabFacilityDistributionAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucAttachment.InitPageParameter(this.Id.ToString());
    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.Id.ToString());
    }

    protected void EditBack_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

}
