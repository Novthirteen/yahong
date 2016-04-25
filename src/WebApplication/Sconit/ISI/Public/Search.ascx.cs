using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Entity;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Utility;
using com.Sconit.Entity.MasterData;
using Geekees.Common.Controls;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using NHibernate.Transform;
using System.Collections;

public partial class ISI_Public_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
        tbTaskSubType.DataBind();
        if (!IsPostBack)
        {
            this.ddlType.Items.RemoveAt(1);

            GenerateTree();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.DoSearch();
    }


    protected override void DoSearch()
    {

        if (SearchEvent != null)
        {
            object[] param = CollectParam();
            if (param != null)
                SearchEvent(param, null);
        }
    }


    protected void btnExport_Click(object sender, EventArgs e)
    {
        IList<object> objList = new List<object>();
        IList<TaskStatusView> taskViews = this.TheCriteriaMgr.FindAll<TaskStatusView>((DetachedCriteria)(CollectParam()[0]));
        if (taskViews != null && taskViews.Count > 0)
        {
            objList.Add(taskViews);
            objList.Add("从 " + this.tbStartDate.Text.Trim() + " 到 " + this.tbEndDate.Text.Trim());
            objList.Add(this.tbTaskSubType.Text.Trim());
            TheReportMgr.WriteToClient("Task.xls", objList, "Task.xls");
        }
    }

    private object[] CollectParam()
    {
        string type = this.ddlType.SelectedValue;
        string taskSubType = this.tbTaskSubType.Text.Trim();
        string taskAddress = this.tbTaskAddress.Text.Trim();
        string backYards = this.tbBackYards.Text.Trim();
        string assignUser = this.tbAssignUser.Text.Trim();
        string startUser = this.tbStartUser.Text.Trim();
        string submitUser = this.tbSubmitUser.Text.Trim();

        DateTime? startTime = null;
        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        DateTime? endTime = null;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endTime = (DateTime.Parse(this.tbEndDate.Text.Trim())).AddDays(1).AddMilliseconds(-1);
        }

        #region status
        IList<string> statusList = new List<string>();
        List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
        foreach (ASTreeViewNode node in nodes)
        {
            statusList.Add(node.NodeValue);
        }

        #endregion

        #region DetachedCriteria

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskStatusView));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskStatusView))
            .SetProjection(Projections.CountDistinct("TaskCode"));


        if (!string.IsNullOrEmpty(backYards))
        {
            selectCriteria.Add(Expression.Like("BackYards", backYards, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("BackYards", backYards, MatchMode.Anywhere));
        }
        if (taskSubType != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("TaskSubTypeCode", taskSubType));
            selectCountCriteria.Add(Expression.Eq("TaskSubTypeCode", taskSubType));
        }
        if (taskAddress != string.Empty)
        {
            selectCriteria.Add(Expression.Like("TaskAddress", taskAddress, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("TaskAddress", taskAddress, MatchMode.Anywhere));
        }
        if (statusList != null && statusList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
        }
        if (!string.IsNullOrEmpty(assignUser))
        {
            selectCriteria.Add(Expression.Eq("AssignUser", assignUser));
            selectCountCriteria.Add(Expression.Eq("AssignUser", assignUser));
        }
        if (!string.IsNullOrEmpty(startUser))
        {
            selectCriteria.Add(
                Expression.Or(
                        Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                       Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                     Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                    Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                                                  Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                              Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                        Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                       Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                     Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                    Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                                                  Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                              Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)))))))));
            selectCountCriteria.Add(
                        Expression.Or(
                        Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                       Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                     Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                    Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                                                  Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                              Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                        Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                       Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                     Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                    Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                                                  Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                              Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + assignUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)))))))));
        }
         
        if (!string.IsNullOrEmpty(submitUser))
        {
            selectCriteria.Add(Expression.Eq("SubmitUser", submitUser));
            selectCountCriteria.Add(Expression.Eq("SubmitUser", submitUser));
        }
        if (startTime.HasValue)
        {
            selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Ge("CreateDate", startTime)));
            selectCountCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Ge("CreateDate", startTime)));
        }
        if (endTime.HasValue)
        {
            selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Le("CreateDate", endTime)));
            selectCountCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Le("CreateDate", endTime)));
        }

        if (!string.IsNullOrEmpty(type))
        {
            selectCriteria.Add(Expression.Eq("Type", type));
            selectCountCriteria.Add(Expression.Eq("Type", type));
        }


        #region 权限的过滤
        if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                && !this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
        {
            if (!(CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                    || CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE)
                    || CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)))
            {
                #region 非观察人
                /*
                DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(this.CurrentUser.Code,
                                ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);

                //创建人
                selectCriteria.Add(
                        Expression.Or(
                            Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                                Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));

                selectCountCriteria.Add(
                        Expression.Or(
                            Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                                Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));
                */

                ISIUtil.SetNoVierUserCriteria(selectCriteria, this.CurrentUser);
                ISIUtil.SetNoVierUserCriteria(selectCountCriteria, this.CurrentUser);

                #endregion
            }
            else
            {
                #region 观察人

                string[] propertyNames = new string[] { "Type", "CreateUser", "SubmitUser", "AssignStartUser", "SchedulingStartUser", "AssignUpUser", "StartUpUser", "CloseUpUser", "TaskSubTypeAssignUser", "ViewUser", "ECUser", "TaskSubTypeCode" };
                ISIUtil.SetVierUserCriteria(selectCriteria, this.CurrentUser.Code, propertyNames);
                ISIUtil.SetVierUserCriteria(selectCountCriteria, this.CurrentUser.Code, propertyNames);

                #endregion
            }
        }


        selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT)));
        selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)));
        selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)));
        selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT)));
        selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)));
        selectCountCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)));

        #endregion


        #endregion

        return new object[] { selectCriteria, selectCountCriteria};

    }


    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        //if (actionParameter.ContainsKey("Location"))
        //{
        //    this.tbLocation.Text = actionParameter["Location"];
        //}
        //if (actionParameter.ContainsKey("Item"))
        //{
        //    this.tbItem.Text = actionParameter["Item"];
        //}
        //if (actionParameter.ContainsKey("EffDate"))
        //{
        //    this.tbEffDate.Text = actionParameter["EffDate"];
        //}
    }

    private void GenerateTree()
    {
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_ISI_STATUS);
        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage("ISI.Status." + status.Value, CurrentUser), status.Value, string.Empty));
        }
        this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[4].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[5].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser);
    }
}

