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

public partial class ISI_MyTask_Search : SearchModuleBase
{
    public string tabAction { get; set; }
    public event EventHandler SearchEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
        tbTaskSubType.DataBind();

        if (!IsPostBack)
        {
            this.ddlType.Items.RemoveAt(1);
            if (string.IsNullOrEmpty(this.tabAction))
            {
                lblStatus.Visible = true;
                astvMyTree.Visible = true;
                GenerateTree();
            }
            else
            {
                lblStatus.Visible = false;
                astvMyTree.Visible = false;
            }

        }
    }

    public void Refresh(object sender, EventArgs e)
    {
        btnSearch_Click(sender, e);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (SearchEvent != null)
        {
            DoSearch();
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

    }

    protected override void DoSearch()
    {
        string code = this.tbCode.Text.Trim();
        string taskSubType = this.tbTaskSubType.Text.Trim();
        string type = this.ddlType.SelectedValue;


        #region DetachedCriteria

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskMstr))
                        .SetProjection(Projections.Distinct(Projections.ProjectionList()
                            .Add(Projections.Property("Code").As("Code"))
                            .Add(Projections.Property("TaskAddress").As("TaskAddress"))
                            .Add(Projections.Property("TaskSubType").As("TaskSubType"))
                            .Add(Projections.Property("Subject").As("Subject"))
                            .Add(Projections.Property("Status").As("Status"))
                            .Add(Projections.Property("Priority").As("Priority"))
                            .Add(Projections.Property("Flag").As("Flag"))
                            .Add(Projections.Property("Color").As("Color"))
                            .Add(Projections.Property("CreateUserNm").As("CreateUserNm"))
                            .Add(Projections.Property("PlanStartDate").As("PlanStartDate"))
                            .Add(Projections.Property("PlanCompleteDate").As("PlanCompleteDate"))
                            .Add(Projections.Property("AssignStartUser").As("AssignStartUser"))
                            .Add(Projections.Property("SchedulingStartUser").As("SchedulingStartUser"))
                            ));

        selectCriteria.CreateAlias("TaskSubType", "tst");
        IDictionary<string, string> alias = new Dictionary<string, string>();
        alias.Add("TaskSubType", "tst");

        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskMstr))
            .SetProjection(Projections.CountDistinct("Code"));

        selectCountCriteria.CreateAlias("TaskSubType", "tst");

        if (code != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Code", code, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Code", code, MatchMode.Anywhere));
        }

        if (!string.IsNullOrEmpty(type))
        {
            selectCriteria.Add(Expression.Eq("Type", type));
            selectCountCriteria.Add(Expression.Eq("Type", type));
        }

        if (taskSubType != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("tst.Code", taskSubType));
            selectCountCriteria.Add(Expression.Eq("tst.Code", taskSubType));
        }


        #region 权限的过滤

        if (string.IsNullOrEmpty(this.tabAction))
        {
            #region status
            IList<string> statusList = new List<string>();
            List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
            foreach (ASTreeViewNode node in nodes)
            {
                statusList.Add(node.NodeValue);
            }

            #endregion

            if (statusList != null && statusList.Count > 0)
            {
                selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
                selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            }

            if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
            {

                if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                {
                    #region 非观察者
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
                    selectCriteria.Add(Expression.Or(Expression.Eq("CreateUser", this.CurrentUser.Code),
                                                 Expression.Or(Expression.Eq("SubmitUser", this.CurrentUser.Code),
                                                               Expression.Or(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)),
                                                                                            Expression.And(Expression.IsNotNull("AssignStartUser"),
                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Eq("AssignStartUser", this.CurrentUser.Code))))))),
                                                                            Expression.Or(Expression.And(Expression.Or(Expression.IsNull("AssignStartUser"), Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq("SchedulingStartUser", this.CurrentUser.Code)))))),
                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq("tst.AssignUpUser", this.CurrentUser.Code))))),
                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq("tst.StartUpUser", this.CurrentUser.Code))))),
                                                                                                                        Expression.Or(Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Eq("tst.CloseUpUser", this.CurrentUser.Code))))),
                                                                                                                                        Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                            Expression.Eq("tst.AssignUser", this.CurrentUser.Code))))),
                                                                                                                                                     Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                            Expression.Eq("tst.ViewUser", this.CurrentUser.Code))))))
                                                                                                                                                                                                ))))))));
                    selectCountCriteria.Add(Expression.Or(Expression.Eq("CreateUser", this.CurrentUser.Code),
                                                 Expression.Or(Expression.Eq("SubmitUser", this.CurrentUser.Code),
                                                               Expression.Or(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)),
                                                                                            Expression.And(Expression.IsNotNull("AssignStartUser"),
                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Eq("AssignStartUser", this.CurrentUser.Code))))))),
                                                                            Expression.Or(Expression.And(Expression.Or(Expression.IsNull("AssignStartUser"), Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq("SchedulingStartUser", this.CurrentUser.Code)))))),
                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq("tst.AssignUpUser", this.CurrentUser.Code))))),
                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Eq("tst.StartUpUser", this.CurrentUser.Code))))),
                                                                                                                        Expression.Or(Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Eq("tst.CloseUpUser", this.CurrentUser.Code))))),
                                                                                                                                        Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                            Expression.Eq("tst.AssignUser", this.CurrentUser.Code))))),
                                                                                                                                                     Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                            Expression.Eq("tst.ViewUser", this.CurrentUser.Code))))))
                                                                                                                                                                                                ))))))));
                    #endregion
                }
                else
                {
                    #region 观察者
                    selectCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                        Expression.Or(Expression.Eq("CreateUser", this.CurrentUser.Code),
                                                                     Expression.Or(Expression.Eq("SubmitUser", this.CurrentUser.Code),
                                                                                   Expression.Or(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                Expression.And(Expression.IsNotNull("AssignStartUser"),
                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Eq("AssignStartUser", this.CurrentUser.Code))))))),
                                                                                                Expression.Or(Expression.And(Expression.Or(Expression.IsNull("AssignStartUser"), Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Eq("SchedulingStartUser", this.CurrentUser.Code)))))),
                                                                                                                Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Eq("tst.AssignUpUser", this.CurrentUser.Code))))),
                                                                                                                                Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Eq("tst.StartUpUser", this.CurrentUser.Code))))),
                                                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                            Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Eq("tst.CloseUpUser", this.CurrentUser.Code))))),
                                                                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                                Expression.Eq("tst.AssignUser", this.CurrentUser.Code))))),
                                                                                                                                                                         Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                            Expression.Eq("tst.ViewUser", this.CurrentUser.Code))))))
                                                                                                                                                                                                                    ))))))))));
                    selectCountCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                        Expression.Or(Expression.Eq("CreateUser", this.CurrentUser.Code),
                                                                     Expression.Or(Expression.Eq("SubmitUser", this.CurrentUser.Code),
                                                                                   Expression.Or(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                Expression.And(Expression.IsNotNull("AssignStartUser"),
                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                    Expression.Eq("AssignStartUser", this.CurrentUser.Code))))))),
                                                                                                Expression.Or(Expression.And(Expression.Or(Expression.IsNull("AssignStartUser"), Expression.Eq("AssignStartUser", string.Empty)),
                                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Eq("SchedulingStartUser", this.CurrentUser.Code)))))),
                                                                                                                Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Eq("tst.AssignUpUser", this.CurrentUser.Code))))),
                                                                                                                                Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Eq("tst.StartUpUser", this.CurrentUser.Code))))),
                                                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                            Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Eq("tst.CloseUpUser", this.CurrentUser.Code))))),
                                                                                                                                                            Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                                Expression.Eq("tst.AssignUser", this.CurrentUser.Code))))),
                                                                                                                                                                         Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + this.CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                                                                                Expression.Eq("tst.ViewUser", this.CurrentUser.Code))))))
                                                                                                                                                                                                                    ))))))))));

                    #endregion
                }
            }
        }
        else
        {
            selectCriteria.Add(Expression.Eq("Status", tabAction));
            selectCountCriteria.Add(Expression.Eq("Status", tabAction));

            if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                        && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
            {
                if (tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
                {
                    selectCriteria.Add(Expression.Eq("CreateUser", this.CurrentUser.Code));
                    selectCountCriteria.Add(Expression.Eq("CreateUser", this.CurrentUser.Code));
                }
                if (tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
                {
                    if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                    {
                        #region 非观察者
                        selectCriteria.Add(Expression.Or(Expression.Or(Expression.Eq("tst.AssignUpUser", CurrentUser.Code),
                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                             Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                        ));
                        selectCountCriteria.Add(Expression.Or(Expression.Or(Expression.Eq("tst.AssignUpUser", CurrentUser.Code),
                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                             Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                        ));
                        #endregion
                    }
                    else
                    {
                        #region 观察者
                        selectCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                        Expression.Or(Expression.Or(Expression.Eq("tst.AssignUpUser", CurrentUser.Code),
                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                 Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                            ))));
                        selectCountCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                                Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                                Expression.Or(Expression.Or(Expression.Eq("tst.AssignUpUser", CurrentUser.Code),
                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                         Expression.Or(Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Eq("tst.AssignUser", CurrentUser.Code))))),
                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                                    ))));
                        #endregion
                    }
                }
                if (tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                        || tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
                {
                    if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                    {
                        #region 非观察者
                        //找执行人
                        selectCriteria.Add(Expression.Or(Expression.And(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)), Expression.IsNotNull("AssignStartUser")),
                                                                                 Expression.Or(Expression.Eq("AssignStartUser", CurrentUser.Code),
                                                                                               Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                             Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                          Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                             ))))),
                                                                Expression.Or(Expression.And(Expression.Or(Expression.Eq("AssignStartUser", string.Empty), Expression.IsNull("AssignStartUser")),
                                                                                             Expression.Or(Expression.Eq("SchedulingStartUser", CurrentUser.Code),
                                                                                               Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                             Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                          Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                                          ))))),
                                                                              Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                    )));

                        selectCountCriteria.Add(Expression.Or(Expression.And(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)), Expression.IsNotNull("AssignStartUser")),
                                                                                 Expression.Or(Expression.Eq("AssignStartUser", CurrentUser.Code),
                                                                                               Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                             Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                          Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                             ))))),
                                                                Expression.Or(Expression.And(Expression.Or(Expression.Eq("AssignStartUser", string.Empty), Expression.IsNull("AssignStartUser")),
                                                                                             Expression.Or(Expression.Eq("SchedulingStartUser", CurrentUser.Code),
                                                                                               Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                             Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                          Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                                          ))))),
                                                                              Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                    )));
                        #endregion
                    }
                    else
                    {
                        #region 观察者
                        selectCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                Expression.Or(Expression.And(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)), Expression.IsNotNull("AssignStartUser")),
                                                                                     Expression.Or(Expression.Eq("AssignStartUser", CurrentUser.Code),
                                                                                                   Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                 Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                              Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                 ))))),
                                                                    Expression.Or(Expression.And(Expression.Or(Expression.Eq("AssignStartUser", string.Empty), Expression.IsNull("AssignStartUser")),
                                                                                                 Expression.Or(Expression.Eq("SchedulingStartUser", CurrentUser.Code),
                                                                                                   Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                 Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                              Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                                              ))))),
                                                                                  Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                )))));
                        selectCountCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                                    Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                            Expression.Or(Expression.And(Expression.And(Expression.Not(Expression.Eq("AssignStartUser", string.Empty)), Expression.IsNotNull("AssignStartUser")),
                                                                                                 Expression.Or(Expression.Eq("AssignStartUser", CurrentUser.Code),
                                                                                                               Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                             Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                          Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                             ))))),
                                                                                Expression.Or(Expression.And(Expression.Or(Expression.Eq("AssignStartUser", string.Empty), Expression.IsNull("AssignStartUser")),
                                                                                                             Expression.Or(Expression.Eq("SchedulingStartUser", CurrentUser.Code),
                                                                                                               Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                             Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                          Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere)
                                                                                                                                                          ))))),
                                                                                              Expression.Or(Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Or(Expression.Like("tst.StartUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                            Expression.Eq("tst.StartUpUser", CurrentUser.Code))))),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                            )))));
                        #endregion
                    }
                }
                if (tabAction == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
                {
                    if (!CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)
                                && !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE))
                    {
                        #region 非观察者
                        selectCriteria.Add(Expression.Or(Expression.And(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE }), Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Eq("SubmitUser", CurrentUser.Code))),
                                            Expression.And(Expression.Not(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE })),
                                                            Expression.Or(Expression.Or(Expression.Eq("tst.CloseUpUser", CurrentUser.Code),
                                                                                                 Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),

                                                                            Expression.And(Expression.Or(Expression.Not(Expression.Eq("CreateUser", CurrentUser.Code)), Expression.Or(Expression.IsNull("tst.CloseUpUser"), Expression.Eq("tst.CloseUpUser", string.Empty))),
                                                                                     Expression.Or(Expression.Or(Expression.Eq("tst.AssignUser", CurrentUser.Code),
                                                                                                                 Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                                                   Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                  Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                       ))))));
                        selectCountCriteria.Add(Expression.Or(Expression.And(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE }), Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Eq("SubmitUser", CurrentUser.Code))),
                                            Expression.And(Expression.Not(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE })),
                                                            Expression.Or(Expression.Or(Expression.Eq("tst.CloseUpUser", CurrentUser.Code),
                                                                                                 Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                    Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),

                                                                            Expression.And(Expression.Or(Expression.Not(Expression.Eq("CreateUser", CurrentUser.Code)), Expression.Or(Expression.IsNull("tst.CloseUpUser"), Expression.Eq("tst.CloseUpUser", string.Empty))),
                                                                                     Expression.Or(Expression.Or(Expression.Eq("tst.AssignUser", CurrentUser.Code),
                                                                                                                 Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                                                   Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                        Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                    Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                  Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                                                       ))))));
                        #endregion
                    }
                    else
                    {
                        #region 观察者

                        selectCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                Expression.Or(Expression.And(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE }), Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Eq("SubmitUser", CurrentUser.Code))),
                                                    Expression.And(Expression.Not(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE })),
                                                                    Expression.Or(Expression.Or(Expression.Eq("tst.CloseUpUser", CurrentUser.Code),
                                                                                                         Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),

                                                                                    Expression.And(Expression.Or(Expression.Not(Expression.Eq("CreateUser", CurrentUser.Code)), Expression.Or(Expression.IsNull("tst.CloseUpUser"), Expression.Eq("tst.CloseUpUser", string.Empty))),
                                                                                             Expression.Or(Expression.Or(Expression.Eq("tst.AssignUser", CurrentUser.Code),
                                                                                                                         Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                                                           Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                          Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                         ))))))));
                        selectCountCriteria.Add(Expression.Or(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)),
                                        Expression.And(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY),
                                                Expression.Or(Expression.And(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE }), Expression.Or(Expression.Eq("CreateUser", CurrentUser.Code), Expression.Eq("SubmitUser", CurrentUser.Code))),
                                                    Expression.And(Expression.Not(Expression.In("Type", new string[] { ISIConstants.ISI_TASK_TYPE_ISSUE, ISIConstants.ISI_TASK_TYPE_RESPONSE })),
                                                                    Expression.Or(Expression.Or(Expression.Eq("tst.CloseUpUser", CurrentUser.Code),
                                                                                                         Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                            Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                        Expression.Or(Expression.Like("tst.CloseUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Like("tst.CloseUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),

                                                                                    Expression.And(Expression.Or(Expression.Not(Expression.Eq("CreateUser", CurrentUser.Code)), Expression.Or(Expression.IsNull("tst.CloseUpUser"), Expression.Eq("tst.CloseUpUser", string.Empty))),
                                                                                             Expression.Or(Expression.Or(Expression.Eq("tst.AssignUser", CurrentUser.Code),
                                                                                                                         Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                        Expression.Or(Expression.Like("tst.AssignUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                        Expression.Like("tst.AssignUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere))))),
                                                                                                           Expression.Or(Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                            Expression.Or(Expression.Like("tst.AssignUpUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                          Expression.Eq("tst.AssignUpUser", CurrentUser.Code))))),
                                                                                                                        Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                    Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_USER_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                Expression.Or(Expression.Like("tst.ViewUser", ISIConstants.ISI_LEVEL_SEPRATOR + CurrentUser.Code + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                                                                    Expression.Eq("tst.ViewUser", CurrentUser.Code))))))
                                                                                         ))))))));
                        #endregion
                    }
                }
            }
        }


        selectCriteria.SetResultTransformer(Transformers.AliasToBean(typeof(TaskMstr)));

        //selectCriteria.SetResultTransformer(new NHibernate.Transform.DistinctRootEntityResultTransformer());
        //selectCountCriteria.SetResultTransformer(NHibernate.CriteriaUtil.DistinctRootEntity);
                        #endregion

        SearchEvent((new object[] { selectCriteria, selectCountCriteria, alias }), null);
                        #endregion

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

        this.astvMyTree.RootNode.ChildNodes[7].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[8].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[9].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[10].CheckedState = ASTreeViewCheckboxState.Checked;

        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[7].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[8].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[9].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[10].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[5].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[7].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[8].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[9].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[10].NodeValue, CurrentUser);

    }
}

