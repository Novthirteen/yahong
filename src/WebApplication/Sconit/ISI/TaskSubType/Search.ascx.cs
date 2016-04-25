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
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using Geekees.Common.Controls;

public partial class ISI_TaskSubType_Search : SearchModuleBase
{
    public string Action
    {
        get
        {
            return (string)ViewState["Action"];
        }
        set
        {
            ViewState["Action"] = value;
        }
    }
    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        tbCode.ServiceParameter = "string:" + this.ModuleType;
        tbCode.DataBind();
        if (!IsPostBack)
        {
            GenerateTree();
            this.btnNew.Visible = this.ModuleType != ISIConstants.ISI_TASK_TYPE_WORKFLOW || this.Action != "View" && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("Code"))
        {
            this.tbCode.Text = actionParameter["Code"];
        }
        if (actionParameter.ContainsKey("Description"))
        {
            this.tbDesc.Text = actionParameter["Description"];
        }
        if (actionParameter.ContainsKey("ProcessUser"))
        {
            this.tbProcessUser.Text = actionParameter["ProcessUser"];
        }
        if (actionParameter.ContainsKey("User"))
        {
            this.tbUser.Text = actionParameter["User"];
        }
    }

    protected override void DoSearch()
    {
        string code = this.tbCode.Text.Trim();
        string desc = this.tbDesc.Text.Trim();
        string user = this.tbUser.Text.Trim();
        string processUser = this.tbProcessUser.Text.Trim();
        string apply = this.tbApply.Text.Trim();

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskSubType));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskSubType)).SetProjection(Projections.Count("Code"));
            if (code != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Code", code));
                selectCountCriteria.Add(Expression.Eq("Code", code));
            }

            if (desc != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Desc", desc, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Desc", desc, MatchMode.Anywhere));
            }

            #region org
            IList<string> orgList = new List<string>();
            List<ASTreeViewNode> orgNodes = this.astvMyTreeOrg.GetCheckedNodes();
            foreach (ASTreeViewNode node in orgNodes)
            {
                orgList.Add(node.NodeValue);
            }
            #endregion
            if (orgList != null && orgList.Count > 0)
            {
                selectCriteria.Add(Expression.In("Org", orgList.ToArray<string>()));
                selectCountCriteria.Add(Expression.In("Org", orgList.ToArray<string>()));
            }

            selectCriteria.Add(Expression.Eq("Type", this.ModuleType));
            selectCountCriteria.Add(Expression.Eq("Type", this.ModuleType));

            if (!string.IsNullOrEmpty(processUser))
            {
                selectCriteria.Add(Subqueries.PropertyIn("Code", ISIUtil.GetProcessDefinitionCriteria(processUser)));
                selectCountCriteria.Add(Subqueries.PropertyIn("Code", ISIUtil.GetProcessDefinitionCriteria(processUser)));
            }
            if (!string.IsNullOrEmpty(apply))
            {
                selectCriteria.Add(Subqueries.PropertyIn("Code", ISIUtil.GetProcessApplyCriteria(apply)));
                selectCountCriteria.Add(Subqueries.PropertyIn("Code", ISIUtil.GetProcessApplyCriteria(apply)));
            }
            if (cbIsActive.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsActive", cbIsActive.Checked));
                selectCountCriteria.Add(Expression.Eq("IsActive", cbIsActive.Checked));
            }
            if (cbIsWF.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsWF", cbIsWF.Checked));
                selectCountCriteria.Add(Expression.Eq("IsWF", cbIsWF.Checked));
            }
            if (cbIsTrace.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsTrace", cbIsTrace.Checked));
                selectCountCriteria.Add(Expression.Eq("IsTrace", cbIsTrace.Checked));
            }
            if (cbIsCost.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsCost", cbIsCost.Checked));
                selectCountCriteria.Add(Expression.Eq("IsCost", cbIsCost.Checked));
            }
            if (cbIsCostCenter.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsCostCenter", cbIsCostCenter.Checked));
                selectCountCriteria.Add(Expression.Eq("IsCostCenter", cbIsCostCenter.Checked));
            }
            if (cbIsBudget.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsBudget", cbIsBudget.Checked));
                selectCountCriteria.Add(Expression.Eq("IsBudget", cbIsBudget.Checked));
            }
            if (cbIsAttachment.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsAttachment", cbIsAttachment.Checked));
                selectCountCriteria.Add(Expression.Eq("IsAttachment", cbIsAttachment.Checked));
            }
            if (this.cbIsApply.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsApply", cbIsApply.Checked));
                selectCountCriteria.Add(Expression.Eq("IsApply", cbIsApply.Checked));
            }
            if (this.cbIsAmount.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsAmount", cbIsAmount.Checked));
                selectCountCriteria.Add(Expression.Eq("IsAmount", cbIsAmount.Checked));
            }
            if (this.cbIsAmountDetail.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsAmountDetail", cbIsAmountDetail.Checked));
                selectCountCriteria.Add(Expression.Eq("IsAmountDetail", cbIsAmountDetail.Checked));
            }
            if (this.cbIsPrint.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsPrint", cbIsPrint.Checked));
                selectCountCriteria.Add(Expression.Eq("IsPrint", cbIsPrint.Checked));
            }
            if (this.cbIsAssignUser.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsAssignUser", cbIsAssignUser.Checked));
                selectCountCriteria.Add(Expression.Eq("IsAssignUser", cbIsAssignUser.Checked));
            }
            if (this.cbIsCtrl.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsCtrl", cbIsCtrl.Checked));
                selectCountCriteria.Add(Expression.Eq("IsCtrl", cbIsCtrl.Checked));
            }
            if (this.cbIsRemoveForm.Checked)
            {
                selectCriteria.Add(Expression.Eq("IsRemoveForm", cbIsRemoveForm.Checked));
                selectCountCriteria.Add(Expression.Eq("IsRemoveForm", cbIsRemoveForm.Checked));
            }
            if (!string.IsNullOrEmpty(processUser))
            {
                selectCriteria.Add(Subqueries.PropertyIn("Code", ISIUtil.GetProcessDefinitionCriteria(processUser)));
                selectCountCriteria.Add(Subqueries.PropertyIn("Code", ISIUtil.GetProcessDefinitionCriteria(processUser)));
            }

            if (user != string.Empty)
            {
                selectCriteria.Add(
                    Expression.Or(Subqueries.PropertyIn("Code", ISIUtil.GetProcessInstanceCriteria(user)),
                         Expression.Or(
                                Expression.Or(
                                         Expression.Or(
                                                    Expression.Or(
                                                            Expression.Or(
                                                                    Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                    ,
                                                                                    Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                                    ,
                                                                    Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                                    ,
                                                                                    Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                                                ,

                                                                    Expression.Or(
                                                                            Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                            ,
                                                                                            Expression.Like("AssignUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                                            ,
                                                                            Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                                            ,
                                                                                            Expression.Like("AssignUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                                                 )
                                                      ,

                                                    Expression.Or(
                                                            Expression.Or(Expression.Like("AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                            ,
                                                            Expression.Or(Expression.Like("AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                                    )
                                        ,

                                        Expression.Or(
                                                Expression.Or(Expression.Like("StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("StartUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                ,
                                                Expression.Or(Expression.Like("StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("StartUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                       )
                         ,

                        Expression.Or(
                                    Expression.Or(
                                                Expression.Or(Expression.Like("CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                ,
                                                Expression.Or(Expression.Like("CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))
                                     ,
                                    Expression.Or(
                                                Expression.Or(
                                                            Expression.Or(Expression.Like("ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ViewUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                            ,
                                                            Expression.Or(Expression.Like("ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ViewUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))
                                                ,
                                                Expression.Or(
                                                            Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ECUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                            ,
                                                            Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ECUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))
                                    )

                       )
                    )
                );
                selectCountCriteria.Add(
                    Expression.Or(Subqueries.PropertyIn("Code", ISIUtil.GetProcessInstanceCriteria(user)),
                         Expression.Or(
                                Expression.Or(
                                         Expression.Or(
                                                    Expression.Or(
                                                            Expression.Or(
                                                                    Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                    ,
                                                                                    Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                                    ,
                                                                    Expression.Or(Expression.Like("StartUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                                    ,
                                                                                    Expression.Like("StartUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                                                ,

                                                                    Expression.Or(
                                                                            Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                            ,
                                                                                            Expression.Like("AssignUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                                            ,
                                                                            Expression.Or(Expression.Like("AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                                            ,
                                                                                            Expression.Like("AssignUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                                                 )
                                                      ,

                                                    Expression.Or(
                                                            Expression.Or(Expression.Like("AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                            ,
                                                            Expression.Or(Expression.Like("AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                                    )
                                        ,

                                        Expression.Or(
                                                Expression.Or(Expression.Like("StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("StartUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                ,
                                                Expression.Or(Expression.Like("StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("StartUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))

                                       )
                         ,

                        Expression.Or(
                                    Expression.Or(
                                                Expression.Or(Expression.Like("CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                ,
                                                Expression.Or(Expression.Like("CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                ,
                                                                Expression.Like("CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))
                                     ,
                                    Expression.Or(
                                                Expression.Or(
                                                            Expression.Or(Expression.Like("ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ViewUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                            ,
                                                            Expression.Or(Expression.Like("ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ViewUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)))
                                                ,
                                                Expression.Or(
                                                            Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ECUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))
                                                            ,
                                                            Expression.Or(Expression.Like("ECUser", ISIConstants.ISI_LEVEL_SEPRATOR + user + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                            ,
                                                                            Expression.Like("ECUser", ISIConstants.ISI_USER_SEPRATOR + user + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))
                                    )
                       )
                    )
                );
            }


            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }

    private void GenerateTree()
    {
        IList<CodeMaster> orgList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_ORG);
        foreach (CodeMaster org in orgList)
        {
            this.astvMyTreeOrg.RootNode.AppendChild(new ASTreeViewLinkNode(org.Value, org.Value, string.Empty));
        }
    }
}
