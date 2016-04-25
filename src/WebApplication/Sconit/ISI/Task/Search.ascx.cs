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

public partial class ISI_Task_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler ExportEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
        tbTaskSubType.DataBind();
        if (!IsPostBack)
        {
            this.ddlType.Items.RemoveAt(1);
            //this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
            GenerateTree();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.DoSearch();
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (ExportEvent != null)
        {
            object[] param = this.CollectParam();
            if (param != null)
                ExportEvent(param, null);
        }
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
            endTime = DateTime.Parse(this.tbEndDate.Text.Trim());
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

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskMstr))
                        .SetProjection(Projections.Distinct(Projections.ProjectionList()
                            .Add(Projections.Property("Code").As("Code"))
                            .Add(Projections.Property("BackYards").As("BackYards"))
                            .Add(Projections.Property("TaskAddress").As("TaskAddress"))
                            .Add(Projections.Property("tst.Code").As("TaskSubTypeCode"))
                            .Add(Projections.Property("tst.Desc").As("TaskSubTypeDesc"))
                            .Add(Projections.Property("Subject").As("Subject"))
                            .Add(Projections.Property("Status").As("Status"))
                            .Add(Projections.Property("Priority").As("Priority"))
                            .Add(Projections.Property("Flag").As("Flag"))
                            .Add(Projections.Property("Desc1").As("Desc1"))
                            .Add(Projections.Property("Color").As("Color"))
                            .Add(Projections.Property("SubmitUser").As("SubmitUser"))
                            .Add(Projections.Property("SubmitUserNm").As("SubmitUserNm"))
                            .Add(Projections.Property("AssignUserNm").As("AssignUserNm"))
                            .Add(Projections.Property("AssignUser").As("AssignUser"))
                            .Add(Projections.Property("CreateDate").As("CreateDate"))
                            .Add(Projections.Property("AssignStartUser").As("StartedUser"))
                            .Add(Projections.Property("AssignStartUserNm").As("AssignStartUserNm"))
                            ));


        IDictionary<string, string> alias = new Dictionary<string, string>();
        alias.Add("TaskSubType", "tst");

        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskMstr))
            .SetProjection(Projections.CountDistinct("Code"));

        selectCriteria.CreateAlias("TaskSubType", "tst");
        selectCountCriteria.CreateAlias("TaskSubType", "tst");


        if (!string.IsNullOrEmpty(backYards))
        {
            selectCriteria.Add(Expression.Like("BackYards", backYards, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("BackYards", backYards, MatchMode.Anywhere));
        }
        if (taskSubType != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("tst.Code", taskSubType));
            selectCountCriteria.Add(Expression.Eq("tst.Code", taskSubType));
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
            selectCriteria.Add(Expression.Eq("assignUser", startUser));
            selectCountCriteria.Add(Expression.Eq("assignUser", startUser));
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
            selectCriteria.Add(Expression.Ge("CreateDate", startTime));
            selectCountCriteria.Add(Expression.Ge("CreateDate", startTime));
        }
        if (endTime.HasValue)
        {
            selectCriteria.Add(Expression.Le("CreateDate", endTime));
            selectCountCriteria.Add(Expression.Le("CreateDate", endTime));
        }

        if (!string.IsNullOrEmpty(type))
        {
            selectCriteria.Add(Expression.Eq("Type", type));
            selectCountCriteria.Add(Expression.Eq("Type", type));
        }

        #region 权限的过滤

        if (!(this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                || this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)))
        {
            if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW))
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

        selectCriteria.AddOrder(Order.Desc("CreateDate"));
        selectCriteria.SetResultTransformer(Transformers.AliasToBean(typeof(TaskView)));

        #endregion

        #endregion

        return new object[] { selectCriteria, selectCountCriteria, alias };

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

