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

public partial class ISI_BillDetail_Main : MainModuleBase
{
    public event EventHandler BackEvent;

    
    protected void Page_Load(object sender, EventArgs e)
    {

        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
    }

    public void InitPageParameter()
    {
       
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1]);
        this.ucList.Visible = true;
        this.ucList.isExport = (bool)((object[])sender)[2];
        this.ucList.isGroup = (bool)((object[])sender)[3];
        this.ucList.UpdateView();
    }

}
