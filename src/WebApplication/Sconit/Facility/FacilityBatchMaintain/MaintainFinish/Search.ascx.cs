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
using System.Collections.Generic;
using NHibernate.Expression;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityMaintain_FinishSearch : SearchModuleBase
{
    public event EventHandler SearchEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }


    protected void tbMaintainGroup_TextChanged(Object sender, EventArgs e)
    {

        DoSearch();
    }

    protected override void DoSearch()
    {
        if (this.tbMaintainGroup != null && this.tbMaintainGroup.Text.Trim() != string.Empty)
        {
            SearchEvent((new object[] { this.tbMaintainGroup.Text.Trim() }), null);
        }
    }


    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        //todo
    }
}
