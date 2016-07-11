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
using System.Collections.Generic;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Facility.Entity;
using Geekees.Common.Controls;
using com.Sconit.Entity;

public partial class Facility_MouldMaster_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    public event EventHandler ImportEvent;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GenerateTree();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        if (SearchEvent != null)
        {
            if (btn == this.btnExport)
            {

                object criteriaParam = this.CollectParam(true);
                SearchEvent(criteriaParam, null);

            }
            else
            {
                DoSearch();
            }
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("FCID"))
        {
            this.tbFCID.Text = actionParameter["FCID"];
        }
        if (actionParameter.ContainsKey("Name"))
        {
            this.tbName.Text = actionParameter["Desc"];
        }
        if (actionParameter.ContainsKey("StartDate"))
        {
            this.tbStartDate.Text = actionParameter["StartDate"];
        }
        if (actionParameter.ContainsKey("EndDate"))
        {
            this.tbEndDate.Text = actionParameter["EndDate"];
        }
    }

    protected override void DoSearch()
    {
        object criteriaParam = this.CollectParam(false);
        SearchEvent(criteriaParam, null);
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        //FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
        string template = "Facility2DBarCode.xls";
        sender = CollectParam(false);
        DetachedCriteria criteria = (DetachedCriteria)((object[])sender)[0];
        IList<FacilityMaster> facilityMasterList = TheCriteriaMgr.FindAll<FacilityMaster>(criteria);

        //IReportBaseMgr iReportBaseMgr = this.GetIReportBaseMgr(orderTemplate, orderHead);
        //string printUrl = XlsHelper.WriteToFile(iReportBaseMgr.GetWorkbook());
        //orderTemplate = "RequisitionOrderContract.xls";
        string printUrl = TheReportMgr.WriteToFile(template, new List<object> { facilityMasterList });
        Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + printUrl + "'); </script>");
    }

    protected void btnCreateMaintain_Click(object sender, EventArgs e)
    {
        TheFacilityMasterMgr.GenerateISITasks();
        TheFacilityMasterMgr.GenerateMouldISITasks();
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        if (ImportEvent != null)
        {
            ImportEvent(sender, e);
        }
    }

    private object CollectParam(bool isExport)
    {

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityMaster));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityMaster)).SetProjection(Projections.Count("FCID"));
        selectCriteria.Add(Expression.Eq("ParentCategory", "YH_MJ"));
        selectCountCriteria.Add(Expression.Eq("ParentCategory", "YH_MJ"));
        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbName.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Name", this.tbName.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Name", this.tbName.Text.Trim(), MatchMode.Anywhere));
        }


        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(this.tbStartDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(this.tbStartDate.Text.Trim())));
        }
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Le("CreateDate", DateTime.Parse(this.tbEndDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Le("CreateDate", DateTime.Parse(this.tbEndDate.Text.Trim())));
        }


        if (this.tbAssetNo.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
        }
        if (!this.cbIsAssetAll.Checked)
        {
           
                selectCriteria.Add(Expression.Eq("IsAsset", this.cbIsAsset.Checked));
                selectCountCriteria.Add(Expression.Eq("IsAsset", this.cbIsAsset.Checked));
           
        }
      

        if (this.tbChargePerson.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("CurrChargePerson", this.tbChargePerson.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("CurrChargePerson", this.tbChargePerson.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbSpecification.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Specification", this.tbSpecification.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Specification", this.tbSpecification.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbChargeSite.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("ChargeSite", this.tbChargeSite.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("ChargeSite", this.tbChargeSite.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbReferenceCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("ReferenceCode", this.tbReferenceCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("ReferenceCode", this.tbReferenceCode.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbChargeOrganization.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("ChargeOrganization", this.tbChargeOrganization.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("ChargeOrganization", this.tbChargeOrganization.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbMaintainGroup.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("MaintainGroup", this.tbMaintainGroup.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("MaintainGroup", this.tbMaintainGroup.Text.Trim(), MatchMode.Anywhere));
        } 
        if (this.tbOwnerDescription.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("OwnerDescription", this.tbOwnerDescription.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("OwnerDescription", this.tbOwnerDescription.Text.Trim(), MatchMode.Anywhere));
        }

        if (!this.cbIsOffBalanceAll.Checked)
        {

            selectCriteria.Add(Expression.Eq("IsOffBalance", this.cbIsOffBalance.Checked));
            selectCountCriteria.Add(Expression.Eq("IsOffBalance", this.cbIsOffBalance.Checked));
        }

        #region status
        IList<string> statusList = new List<string>();
        List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
        foreach (ASTreeViewNode node in nodes)
        {
            statusList.Add(node.NodeValue);
        }
        if (statusList != null && statusList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
        }

        #endregion

        #region cateogy
        IList<string> categoryList = new List<string>();
        List<ASTreeViewNode> categoryNodes = this.astvMyTree1.GetCheckedNodes();
        foreach (ASTreeViewNode node in categoryNodes)
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityCategory>();
            criteria.Add(Expression.Or(Expression.Eq("Code", node.NodeValue), Expression.Eq("ParentCategory", node.NodeValue)));
            IList<FacilityCategory> facilityCategoryList = TheCriteriaMgr.FindAll<FacilityCategory>(criteria);

            foreach (FacilityCategory category in facilityCategoryList)
            {
                if (!categoryList.Contains(category.Code))
                {
                    categoryList.Add(category.Code);
                }
            }
        }
        if (categoryList != null && categoryList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Category", categoryList.ToArray<string>()));
            selectCountCriteria.Add(Expression.In("Category", categoryList.ToArray<string>()));
        }

        #endregion

        return new object[] { selectCriteria, selectCountCriteria, isExport, true };
    }

    private void GenerateTree()
    {
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(FacilityConstants.CODE_MASTER_FACILITY_STATUS);
        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage(status.Value, CurrentUser), status.Value, string.Empty));
        }
        this.astvMyTree.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[2].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[4].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[5].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[6].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[7].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," +this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[2].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[6].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[2].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[6].NodeValue, CurrentUser);


        #region 类别多选
        IList<FacilityCategory> categoryList = TheFacilityCategoryMgr.GetAllMouldCategory();
        if (categoryList != null && categoryList.Count > 0)
        {
            foreach (FacilityCategory category in categoryList)
            {
                this.astvMyTree1.RootNode.AppendChild(new ASTreeViewLinkNode(category.Code + "[" + category.Description + "]", category.Code, string.Empty));
            }
        }
        #endregion
    }
}
