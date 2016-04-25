using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using Geekees.Common.Controls;
using NHibernate.Transform;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using NHibernate;
using NHibernate.Type;

public partial class ISI_Client_Main : com.Sconit.Web.MainModuleBase
{
    public event EventHandler BackEvent;

    private int defaultInterval { get; set; }

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

    public void InitPageParameter(string taskCode)
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:#ddlType,string:" + this.CurrentUser.Code;
        tbTaskSubType.DataBind();
        if (!IsPostBack)
        {
            this.ddlType.Items.RemoveAt(1);
            this.ddlType.Items.RemoveAt(4);
            this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);

            if (this.ModuleParameter.ContainsKey("ModuleType"))
            {
                this.ModuleType = this.ModuleParameter["ModuleType"];
            }
            GenerateTree();

            Control rvISIRefresh = this.NamingContainer.FindControl("rvISIRefresh");

            try
            {
                defaultInterval = int.Parse(this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CLIENT_ROWS).Value);
            }
            catch
            {
                defaultInterval = 15;
            }
            ((RangeValidator)rvISIRefresh).ErrorMessage = "${ISI.Validator.Valid.Number," + defaultInterval + ",600}";
            ((RangeValidator)rvISIRefresh).MinimumValue = defaultInterval.ToString();
            /*
            DateTime now = DateTime.Now;
            tbStartDate.Text = now.ToString();
            tbEndDate.Text = now.AddDays(7).ToString();
            */
            //btnSearch_Click(sender, e);

        }
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["LastMinCreateDate"] = null;
        }
    }

    private void PageCleanup()
    {

    }

    protected void hfIsView_OnValueChanged(object sender, EventArgs e)
    {
        this.tmr.Enabled = !this.tmr.Enabled;
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        DetachedCriteria criteria = this.GetCriteria();
        IList<TaskView> taskItemList = (List<TaskView>)TheCriteriaMgr.FindAll<TaskView>(criteria);
        SetStartUser(taskItemList);
        this.GV_List.DataSource = taskItemList;
        this.GV_List.DataBind();
        this.ExportXLS(this.GV_List);
    }

    protected void btnHide_Click(object sender, EventArgs e)
    {
        doSearch();

        //Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>ISIClient();</script>");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.tmr.Enabled = false;
        Session["LastMinCreateDate"] = null;

        DetachedCriteria selectCriteria = GetCriteria();
        IList<TaskView> taskItemList = (List<TaskView>)TheCriteriaMgr.FindAll<TaskView>(selectCriteria);
        SetStartUser(taskItemList);
        this.GV_List.DataSource = taskItemList;
        this.GV_List.DataBind();
    }

    protected void Timer1_Tick(object sender, EventArgs e)
    {
        //ScriptManager.RegisterStartupScript(UP_GV_List, UP_GV_List.GetType(), "", "GVFadeOut();", true);
        doSearch();
        //lblJs.Text = "<script type='text/javascript'>GVFadeOut();</script>";
    }

    private void doSearch()
    {
        try
        {
            lblLoading.Visible = true;
            DateTime? lastMinCreateDate = null;
            if (Session["LastMinCreateDate"] != null)
            {
                lastMinCreateDate = DateTime.Parse(Session["LastMinCreateDate"].ToString());
            }

            int rows = 0;
            DetachedCriteria selectCriteria = GetCriteria(out rows);

            if (lastMinCreateDate.HasValue)
            {
                selectCriteria.Add(Expression.Lt("CreateDate", lastMinCreateDate));
            }

            IList<TaskView> taskItemList = TheCriteriaMgr.FindAll<TaskView>(selectCriteria, 0, rows);
            if (taskItemList == null || taskItemList.Count == 0)
            {
                lastMinCreateDate = null;
                selectCriteria = GetCriteria(out rows);
                taskItemList = (List<TaskView>)TheCriteriaMgr.FindAll<TaskView>(selectCriteria, 0, rows);
            }
            if (taskItemList.Count < rows)
            {

                lastMinCreateDate = null;
            }
            else
            {
                lastMinCreateDate = taskItemList[taskItemList.Count - 1].CreateDate;
            }

            SetStartUser(taskItemList);


            this.GV_List.DataSource = taskItemList;
            //this.UP_GV_List.DataBind();
            this.GV_List.DataBind();

            Session["LastMinCreateDate"] = lastMinCreateDate;

            SetTimer();

        }
        catch (Exception e)
        {
            this.ShowWarningMessage(e.Message);

            Session["LastMinCreateDate"] = null;
            this.tmr.Enabled = false;
        }
        finally
        {
            lblLoading.Visible = false;
        }
    }

    private void SetStartUser(IList<TaskView> taskItemList)
    {
        string startUsers = string.Join(";", taskItemList.Select(t => (!string.IsNullOrEmpty(t.StartedUser) ? t.StartedUser : string.Empty)).ToArray<string>());

        IDictionary<string, string> startUserDic = null;
        if (!string.IsNullOrEmpty(startUsers))
        {
            startUserDic = this.TheTaskMstrMgr.GetUser(startUsers);
        }
        foreach (TaskView taskItem in taskItemList)
        {
            taskItem.StartUser = string.Empty;
            if (!string.IsNullOrEmpty(startUsers))
            {
                string startUser = !string.IsNullOrEmpty(taskItem.StartedUser) ? taskItem.StartedUser : string.Empty;
                if (!string.IsNullOrEmpty(startUser))
                {
                    string[] userCodes = startUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                    if (userCodes != null && userCodes.Length > 0)
                    {
                        foreach (string userCode in userCodes)
                        {
                            if (startUserDic.Keys.Contains(userCode))
                            {
                                if (!string.IsNullOrEmpty(taskItem.StartUser))
                                {
                                    taskItem.StartUser += ", ";
                                }
                                taskItem.StartUser += startUserDic[userCode].Trim();
                            }
                        }
                    }
                }
            }
        }
    }

    private DetachedCriteria GetCriteria(out int rows)
    {
        rows = GetRows();
        return GetCriteria();
    }

    private DetachedCriteria GetCriteria()
    {
        string type = this.ddlType.SelectedValue;
        string taskSubType = this.tbTaskSubType.Text.Trim();
        string taskAddress = this.tbTaskAddress.Text.Trim();
        //string priority = this.ddlPriority.SelectedValue;
        #region status
        IList<string> statusList = new List<string>();
        List<ASTreeViewNode> nodes = this.astvMyTree.GetCheckedNodes();
        foreach (ASTreeViewNode node in nodes)
        {
            statusList.Add(node.NodeValue);
        }

        #endregion

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

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskMstr))
                    .SetProjection(Projections.Distinct(Projections.ProjectionList()
                     .Add(Projections.Property("SubmitDate").As("SubmitDate"))
                    .Add(Projections.Property("Code").As("Code"))
            //.Add(Projections.Property("BackYards").As("BackYards"))
                       .Add(Projections.Property("TaskAddress").As("TaskAddress"))
            //.Add(Projections.Property("TaskSubType").As("TaskSubType"))
                       .Add(Projections.Property("Subject").As("Subject"))
                       .Add(Projections.Property("Desc1").As("Desc1"))
            //.Add(Projections.Property("Status").As("Status"))
                       .Add(Projections.Property("Priority").As("Priority"))
                       .Add(Projections.Property("Flag").As("Flag"))
                       .Add(Projections.Property("Color").As("Color"))
                       .Add(Projections.Property("AssignStartUser").As("StartedUser"))
            //           .Add(Projections.Property("SchedulingStartUser").As("SchedulingStartUser"))
            // .Add(Projections.SqlProjection(@"isnull(AssignStartUser,SchedulingStartUser) as StartedUser", new String[] { "StartedUser" }, new IType[] { NHibernateUtil.String }))
            //           .Add(Projections.Property("AssignStartUser").As("AssignStartUser"))
                        .Add(Projections.Property("PlanCompleteDate").As("PlanCompleteDate"))
                       .Add(Projections.Property("CreateDate").As("CreateDate"))
                       .Add(Projections.Property("Status").As("Status"))
                       ));
        selectCriteria.CreateAlias("TaskSubType", "tst");
        //selectCriteria.Add(Expression.Eq("tst.IsActive", true));

        if (taskSubType != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("tst.Code", taskSubType));
        }
        if (taskAddress != string.Empty)
        {
            selectCriteria.Add(Expression.Like("TaskAddress", taskAddress, MatchMode.Anywhere));
        }
        if (statusList != null && statusList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
        }
        /*
        if (priority != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Priority", priority));
        }*/
        if (startTime.HasValue)
        {
            selectCriteria.Add(Expression.Ge("CreateDate", startTime));
        }
        if (endTime.HasValue)
        {
            selectCriteria.Add(Expression.Le("CreateDate", endTime));
        }

        if (type != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Type", type));
        }

        if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
               && !this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN)
              )
        {
            if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW))
            {
                #region 非观察者
                DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(this.CurrentUser.Code,
                                ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);

                selectCriteria.Add(
                         Expression.Or(
                             Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                                 Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));

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

                #endregion
            }
        }

        //selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PRIVACY)));
        //selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PLAN)));
        //selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT)));
        //selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)));
        selectCriteria.AddOrder(Order.Desc("CreateDate"));

        selectCriteria.SetResultTransformer(Transformers.AliasToBean(typeof(TaskView)));

        return selectCriteria;
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        TaskView task = (TaskView)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (!string.IsNullOrEmpty(task.Color))
            {
                e.Row.Cells[6].Attributes["style"] = "background-color:" + task.Color;
            }
            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                e.Row.Cells[1].ToolTip = task.Priority;
            }
            if ((task.Status == ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN
                    || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT
                    || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS) &&
                   task.PlanCompleteDate < DateTime.Now)
            {
                e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
            }
            //string startUser = this.TheUserSubscriptionMgr.GetUserName(task.StartUser);
            if (!this.IsExport)
            {
                e.Row.Cells[2].Text = ISIUtil.GetStrLength(task.TaskAddress, 22);
                e.Row.Cells[2].ToolTip = task.TaskAddress;
                e.Row.Cells[3].Text = ISIUtil.GetStrLength(task.Subject, 40);
                e.Row.Cells[3].ToolTip = task.Subject;
                e.Row.Cells[4].Text = ISIUtil.GetStrLength(task.Desc1, 80);
                e.Row.Cells[4].ToolTip = task.Desc1;
                e.Row.Cells[5].Text = ISIUtil.GetStrLength(task.StartUser, 40);
                e.Row.Cells[5].ToolTip = task.StartUser;
            }
            else
            {
                e.Row.Cells[5].Text = task.StartUser;
            }
        }
    }

    private int GetRows()
    {
        int defaultRows = 18;
        int rows = 0;
        Control tbISIRows = this.NamingContainer.FindControl("tbISIRows");
        try
        {
            if (tbISIRows != null && tbISIRows is TextBox && !string.IsNullOrEmpty(((TextBox)tbISIRows).Text.Trim()) && ((TextBox)tbISIRows).Text.Trim() != "0" && Int32.Parse(((TextBox)tbISIRows).Text.Trim()) > 0)
            {
                rows = Int32.Parse(((TextBox)tbISIRows).Text.Trim());
            }
            else
            {
                rows = defaultRows;
            }
        }
        catch
        {
            rows = defaultRows;
        }
        return rows;
    }

    private void SetTimer()
    {
        int interval = 1000;
        Control tbISIRefresh = this.NamingContainer.FindControl("tbISIRefresh");
        Control rvISIRefresh = this.NamingContainer.FindControl("rvISIRefresh");

        try
        {
            defaultInterval = int.Parse(this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CLIENT_ROWS).Value);
        }
        catch
        {
            defaultInterval = 15;
        }
        ((RangeValidator)rvISIRefresh).ErrorMessage = "${ISI.Validator.Valid.Number," + defaultInterval + ",600}";
        ((RangeValidator)rvISIRefresh).MinimumValue = defaultInterval.ToString();

        try
        {
            if (tbISIRefresh != null && tbISIRefresh is TextBox && !string.IsNullOrEmpty(((TextBox)tbISIRefresh).Text.Trim()) && ((TextBox)tbISIRefresh).Text.Trim() != "0" && Int32.Parse(((TextBox)tbISIRefresh).Text.Trim()) >= defaultInterval)
            {
                interval *= Int32.Parse(((TextBox)tbISIRefresh).Text.Trim());
            }
            else
            {
                interval *= defaultInterval;
            }
        }
        catch
        {
            interval *= defaultInterval;
        }
        this.tmr.Interval = interval;
        /*
        if (!this.tmr.Enabled)
        {
            this.tmr.Enabled = true;
        }
        */
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
