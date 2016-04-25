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

public partial class Facility_FacilityStock_EditMain : MainModuleBase
{
    public event EventHandler BackEvent;

    protected string StNo
    {
        get
        {
            return (string)ViewState["StNo"];
        }
        set
        {
            ViewState["StNo"] = value;
        }
    }

    public void InitPageParameter(string stNo)
    {
        this.StNo = stNo;
        this.ucEdit.InitPageParameter(stNo);
        this.ucDetail.InitPageParameter(stNo);
        this.ucTabNavigator.UpdateView(stNo);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lbFacilityStockMasterClickEvent += new System.EventHandler(this.TabFacilityStockMasterClick_Render);
        this.ucTabNavigator.lbFacilityStockDetailClickEvent += new System.EventHandler(this.TabFacilityStockDetailClick_Render);
        this.ucTabNavigator.lbFacilityAttachmentClickEvent += new System.EventHandler(this.TabFacilityAttachmentClick_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.EditBack_Render);
     
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucDetail.BackEvent += new System.EventHandler(this.EditBack_Render);
    }

    protected void TabFacilityStockMasterClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucDetail.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucEdit.InitPageParameter(StNo);
    }

    protected void TabFacilityStockDetailClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = true;
        this.ucAttachment.Visible = false;
        this.ucDetail.InitPageParameter(StNo);
    }

    protected void TabFacilityAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucDetail.Visible = false;
        this.ucAttachment.InitPageParameter(this.StNo);

    }



    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.StNo);
    }


    protected void EditBack_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

}
