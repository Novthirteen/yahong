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

public partial class MasterData_Bom_TabNavigator : ModuleBase
{
    public event EventHandler lbBomViewClickEvent;
    public event EventHandler lbBomClickEvent;
    public event EventHandler lbBomDetailClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.CurrentUser.PagePermission.Select(p => p.Code).Contains(BusinessConstants.PERMISSION_PAGE_MASTERDATA_VALUE_PAGE_EDITBOM))
        {
            this.tab_bom.Visible = true;
            this.tab_bomdetail.Visible = true;
        }
    }

    protected void lbBomView_Click(object sender, EventArgs e)
    {
        if (lbBomViewClickEvent != null)
        {
            lbBomViewClickEvent(this, e);
        }

        this.tab_bomview.Attributes["class"] = "ajax__tab_active";
        this.tab_bom.Attributes["class"] = "ajax__tab_inactive";
        this.tab_bomdetail.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbBom_Click(object sender, EventArgs e)
    {
        if (lbBomClickEvent != null)
        {
            lbBomClickEvent(this, e);
        }

        this.tab_bomview.Attributes["class"] = "ajax__tab_inactive";
        this.tab_bom.Attributes["class"] = "ajax__tab_active";
        this.tab_bomdetail.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbBomDetail_Click(object sender, EventArgs e)
    {
        if (lbBomDetailClickEvent != null)
        {
            lbBomDetailClickEvent(this, e);
        }

        this.tab_bomview.Attributes["class"] = "ajax__tab_inactive";
        this.tab_bom.Attributes["class"] = "ajax__tab_inactive";
        this.tab_bomdetail.Attributes["class"] = "ajax__tab_active";
    }
}
