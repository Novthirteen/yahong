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

public partial class Facility_FacilityDistributionDetail_Main : MainModuleBase
{
    public event EventHandler BackEvent;

    protected Int32 FacilityDistributionId
    {
        get
        {
            return (Int32)ViewState["FacilityDistributionId"];
        }
        set
        {
            ViewState["FacilityDistributionId"] = value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucList.EditEvent += new System.EventHandler(this.ListEdit_Render);
        this.ucList.NewEvent += new System.EventHandler(this.New_Render);
        this.ucNew.BackEvent += new System.EventHandler(this.NewBack_Render);
        this.ucNew.CreateEvent += new System.EventHandler(this.CreateBack_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucList.BackEvent += new System.EventHandler(this.ListBack_Render);

        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter(Int32 id)
    {
        this.FacilityDistributionId = id;
        this.ucList.InitPageParameter(id);
        this.ucNew.InitPageParameter(id);
        this.ucEdit.FacilityDistributionId = id;

        this.ucList.Visible = true;
        this.ucNew.Visible = false;
        this.ucEdit.Visible = false;
    }


    //The event handler when user click button "New" button
    void New_Render(object sender, EventArgs e)
    {
        this.ucList.Visible = false;
        this.ucNew.Visible = true;
        this.ucNew.PageCleanup();
    }

    //The event handler when user click button "Back" button of ucNew
    void NewBack_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = false;
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }

    //The event handler when user click button "Save" button of ucNew
    void CreateBack_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter((int)sender);
    }

    //The event handler when user click link "Edit" link of ucList
    void ListEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucList.Visible = false;
        this.ucEdit.InitPageParameter(Convert.ToInt32(sender));
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }

    //The event handler when user click button "Back" button of ucEdit
    void ListBack_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }
}
