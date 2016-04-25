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

public partial class ISI_Reports_Acceptability_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlType.DataSource = this.GetType();
            ddlType.DataBind();

            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.Year + "-01";
            this.tbEndDate.Text = now.ToString("yyyy-MM");
            //btnSearch_Click(null, null);
        }
    }

    private IList<CodeMaster> GetType()
    {
        IList<CodeMaster> typeList = new List<CodeMaster>();
        typeList.Add(new CodeMaster());
        typeList.Add(GetType(ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECTTYPE_VALUE_CADRE));
        typeList.Add(GetType(ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECTTYPE_VALUE_EMPLOYEE));

        return typeList;
    }

    private CodeMaster GetType(string type)
    {
        return TheCodeMasterMgr.GetCachedCodeMaster(ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECTTYPE, type);
    }
}