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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Utility;


public partial class Facility_FacilityFixOrder_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    private IDictionary<string, string> dicParam;

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            this.StatusDataBind();
            this.tbStartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        if (NewEvent != null)
        {
            NewEvent(sender, e);
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.dicParam = new Dictionary<string, string>();

        this.dicParam.Add("inspectNo", this.tbInspectNo.Text.Trim());
        this.dicParam.Add("startDate", this.tbStartDate.Text.Trim());
        this.dicParam.Add("endDate", this.tbEndDate.Text.Trim());
        this.dicParam.Add("createUser", this.tbCreateUser.Text.Trim());
        this.dicParam.Add("status", this.ddlStatus.SelectedValue);


        Button btn = (Button)sender;
        if (SearchEvent != null)
        {
            if (btn == this.btnExport)
            {

                object criteriaParam = this.CollectMasterParam(true);
                SearchEvent(criteriaParam, null);

            }
            else
            {
                DoSearch();
            }
        }
    }
    protected override void DoSearch()
    {

        object criteriaParam = this.CollectMasterParam(false);
        SearchEvent(criteriaParam, null);

    }

    private object CollectMasterParam(bool isExport)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(InspectOrder));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(InspectOrder))
            .SetProjection(Projections.ProjectionList()
           .Add(Projections.Count("InspectNo")));

        if (this.dicParam["inspectNo"] != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("InspectNo", this.dicParam["inspectNo"]));
            selectCountCriteria.Add(Expression.Eq("InspectNo", this.dicParam["inspectNo"]));
        }

        if (this.dicParam["startDate"] != string.Empty)
        {
            selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(this.dicParam["startDate"])));
            selectCountCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(this.dicParam["startDate"])));
        }
        if (this.dicParam["endDate"] != string.Empty)
        {
            selectCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(this.dicParam["endDate"]).AddDays(1)));
            selectCountCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(this.dicParam["endDate"]).AddDays(1)));
        }

        if (this.dicParam["status"] != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Status", this.dicParam["status"]));
            selectCountCriteria.Add(Expression.Eq("Status", this.dicParam["status"]));
        }
        if (this.dicParam["createUser"] != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("CreateUser.Code", this.dicParam["createUser"]));
            selectCountCriteria.Add(Expression.Eq("CreateUser.Code", this.dicParam["createUser"]));
        }
        
        return new object[] { selectCriteria, selectCountCriteria, isExport, true };
    }

  
    private void StatusDataBind()
    {
        this.ddlStatus.DataSource = this.GetStatusGroup();
        this.ddlStatus.DataBind();
    }

    private IList<CodeMaster> GetStatusGroup()
    {
        IList<CodeMaster> statusGroup = new List<CodeMaster>();

        statusGroup.Add(new CodeMaster()); //添加空选项
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE));

        return statusGroup;
    }
    private CodeMaster GetStatus(string statusValue)
    {
        return TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_STATUS, statusValue);
    }
}
