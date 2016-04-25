using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

public partial class ISI_Reports_Amount_Main : com.Sconit.Web.MainModuleBase
{
    public string ModuleType
    {
        get { return (string)ViewState["ModuleType"]; }
        set { ViewState["ModuleType"] = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbPartyFrom.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code;
        this.tbPartyTo.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code;
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:true,bool:true,bool:false,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;

        if (!IsPostBack)
        {
            if (this.ModuleParameter.ContainsKey("ModuleType"))
            {
                this.ModuleType = this.ModuleParameter["ModuleType"];
            }

            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.ToString("yyyy-MM-01");
            this.tbEndDate.Text = now.ToString("yyyy-MM-dd");
            //btnSearch_Click(null, null);
        }
    }
}