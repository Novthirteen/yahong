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
using com.Sconit.Service.MasterData;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;
using Geekees.Common.Controls;
using NHibernate.Expression;
using System.Text;

public partial class Facility_FacilityStock_New : NewModuleBase
{
    public event EventHandler SaveEvent;
    public event EventHandler BackEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            UpdateView();

        }
        else
        {
            if (this.astvChargeOrganization.RootNode.ChildNodes.Count == 0 &&
           this.astvChargePerson.RootNode.ChildNodes.Count == 0 &&
           this.astvChargeSite.RootNode.ChildNodes.Count == 0 &&
           this.astvFacilityCategory.RootNode.ChildNodes.Count == 0)
            {
                GenerateTree();
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string effectiveDate = this.tbEffDate.Text.Trim();

        DateTime effDate = DateTime.Now;
       
        try
        {
            effDate = Convert.ToDateTime(effectiveDate);
        }
        catch (Exception)
        {
            ShowErrorMessage("Common.Business.Error.DateInvalid");
            return;
        }

        FacilityStockMaster stockMaster = new FacilityStockMaster();
        stockMaster.EffDate = effDate;
        #region cateogy
        StringBuilder categoryStr = new StringBuilder();
        List<ASTreeViewNode> categoryNodes = this.astvFacilityCategory.GetCheckedNodes();
        foreach (ASTreeViewNode node in categoryNodes)
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityCategory>();
            criteria.Add(Expression.Or(Expression.Eq("Code", node.NodeValue), Expression.Eq("ParentCategory", node.NodeValue)));
            IList<FacilityCategory> facilityCategoryList = TheCriteriaMgr.FindAll<FacilityCategory>(criteria);

            foreach (FacilityCategory category in facilityCategoryList)
            {
                if (string.IsNullOrEmpty(categoryStr.ToString()))
                {
                    categoryStr.Append(category.Code);
                }
                else
                {
                    categoryStr.Append(",").Append(category.Code);
                }
            }
        }
        stockMaster.FacilityCategory = categoryStr.ToString();
        #endregion

        #region chargeOrg
        StringBuilder chargeOrgStr = new StringBuilder();
        List<ASTreeViewNode> chargeOrgNodes = this.astvChargeOrganization.GetCheckedNodes();
        foreach (ASTreeViewNode node in chargeOrgNodes)
        {
            if (string.IsNullOrEmpty(chargeOrgStr.ToString()))
            {
                chargeOrgStr.Append(node.NodeValue);
            }
            else
            {
                chargeOrgStr.Append(",").Append(node.NodeValue);
            }
        }
        stockMaster.ChargeOrg = chargeOrgStr.ToString();
        #endregion

        #region chargeSite
        StringBuilder chargeSiteStr = new StringBuilder();
        List<ASTreeViewNode> chargeSiteNodes = this.astvChargeSite.GetCheckedNodes();
        foreach (ASTreeViewNode node in chargeSiteNodes)
        {
            if (string.IsNullOrEmpty(chargeSiteStr.ToString()))
            {
                chargeSiteStr.Append(node.NodeValue);
            }
            else
            {
                chargeSiteStr.Append(",").Append(node.NodeValue);
            }
        }
        stockMaster.ChargeSite = chargeSiteStr.ToString();
        #endregion

        #region chargePerson
        StringBuilder chargePersonStr = new StringBuilder();
        StringBuilder chargePersonNameStr = new StringBuilder();
        List<ASTreeViewNode> chargePersonNodes = this.astvChargePerson.GetCheckedNodes();
        foreach (ASTreeViewNode node in chargePersonNodes)
        {
            User chargePerson = TheUserMgr.CheckAndLoadUser(node.NodeValue);
            if (chargePerson != null)
            {
                if (string.IsNullOrEmpty(chargePersonStr.ToString()))
                {
                    chargePersonStr.Append(chargePerson.Code);
                    chargePersonNameStr.Append(chargePerson.Name);
                }
                else
                {
                    chargePersonStr.Append(",").Append(node.NodeValue);
                    chargePersonNameStr.Append(",").Append(chargePerson.Name);
                }
            }
        }
        stockMaster.ChargePerson = chargePersonStr.ToString();
        stockMaster.ChargePersonName = chargePersonNameStr.ToString();
        stockMaster.CreateUser = this.CurrentUser.Code;
        stockMaster.LastModifyUser = this.CurrentUser.Code;
        stockMaster.CreateDate = DateTime.Now;
        stockMaster.LastModifyDate = DateTime.Now;
        stockMaster.StNo = TheNumberControlMgr.GenerateNumber(FacilityConstants.CODE_PREFIX_FACILITYSTOCKTAKE);
        stockMaster.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;
        stockMaster.AssetNo = this.tbAssetNo.Text.Trim();

        TheFacilityStockMasterMgr.CreateFacilityStockMaster(stockMaster);
        #endregion

        ShowSuccessMessage("Facility.FacilityStock.AddFacilityStockMaster.Successfully",stockMaster.StNo);

        if (SaveEvent != null)
        {
            SaveEvent(stockMaster.StNo, e);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    public void UpdateView()
    {
        this.tbEffDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        this.tbAssetNo.Text = string.Empty;
       
    }

    private void GenerateTree()
    {

        #region 负责地点多选
        IList<FacilityMaster> chargeSiteList = TheFacilityMasterMgr.GetFacilityChargeSite();
        if (chargeSiteList != null && chargeSiteList.Count > 0)
        {
            foreach (FacilityMaster chargeSite in chargeSiteList)
            {
                this.astvChargeSite.RootNode.AppendChild(new ASTreeViewLinkNode(chargeSite.ChargeSite, chargeSite.ChargeSite, string.Empty));
            }
        }
        #endregion

        #region 负责组织多选
        IList<FacilityMaster> chargeOrganizationList = TheFacilityMasterMgr.GetFacilityChargeOrganization();
        if (chargeOrganizationList != null && chargeOrganizationList.Count > 0)
        {
            foreach (FacilityMaster chargeOrganization in chargeOrganizationList)
            {
                this.astvChargeOrganization.RootNode.AppendChild(new ASTreeViewLinkNode(chargeOrganization.ChargeOrganization, chargeOrganization.ChargeOrganization, string.Empty));
            }
        }
        #endregion

        #region 负责人多选
        IList<FacilityMaster> chargePersonList = TheFacilityMasterMgr.GetFacilityChargePerson();
        if (chargePersonList != null && chargePersonList.Count > 0)
        {
            foreach (FacilityMaster chargePerson in chargePersonList)
            {
                this.astvChargePerson.RootNode.AppendChild(new ASTreeViewLinkNode(chargePerson.CurrChargePerson + "[" + chargePerson.CurrChargePersonName + "]", chargePerson.CurrChargePerson, string.Empty));
            }
        }
        #endregion

        #region 类别多选
        IList<FacilityCategory> categoryList = TheFacilityCategoryMgr.GetAllFacilityCategory();
        if (categoryList != null && categoryList.Count > 0)
        {
            foreach (FacilityCategory category in categoryList)
            {
                this.astvFacilityCategory.RootNode.AppendChild(new ASTreeViewLinkNode(category.Code + "[" + category.Description + "]", category.Code, string.Empty));
            }
        }
        #endregion
    }
}
