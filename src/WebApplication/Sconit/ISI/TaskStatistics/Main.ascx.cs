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
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Text;

public partial class ISI_TaskStatistics_Main : com.Sconit.Web.MainModuleBase
{
    public IList<AssignUser> AssignUserList
    {
        get
        {
            return (IList<AssignUser>)ViewState["AssignUserList"];
        }
        set
        {
            ViewState["AssignUserList"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:,bool:true,string:" + this.CurrentUser.Code + ",bool:false,bool:false,bool:false,bool:false";
        tbTaskSubType.DataBind();

        if (!IsPostBack)
        {
            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.AddDays(-7).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = now.ToString("yyyy-MM-dd");
            //btnSearch_Click(null, null);  
            this.ddlType.Items.RemoveAt(1);
            this.ddlType.Items.RemoveAt(this.ddlType.Items.Count - 1);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string startDate;
            string endDate;
            DataSet dataSet;
            GetDateSet(out startDate, out endDate, out dataSet, string.Empty);

            //获取非第一负责人
            StringBuilder sql1 = new StringBuilder();
            sql1.Append("select isnull(SchedulingStartUser,AssignStartUser) StartedUser  from ISI_TaskMstr where AssignDate >= @StartDate and AssignDate <= @EndDate ");
            sql1.Append(" and  isnull(SchedulingStartUser,AssignStartUser) is not null and isnull(SchedulingStartUser,AssignStartUser)!=''");

            IList<SqlParameter> sqlParam1 = new List<SqlParameter>();
            sqlParam1.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
            sqlParam1.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
            if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
            {
                sql1.Append(" and  TaskSubType =@TaskSubType ");
                sqlParam1.Add(new SqlParameter("@TaskSubType", this.tbTaskSubType.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
            {
                sql1.Append(" and  Type =@Type ");
                sqlParam1.Add(new SqlParameter("@Type", this.ddlType.SelectedValue));
            }
            sql1.Append("  and  Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");

            if (this.tbUser.Text.Trim() != string.Empty)
            {
                //sql1.Append("and (isnull(SchedulingStartUser,AssignStartUser) like '" + ISIConstants.ISI_LEVEL_SEPRATOR + tbUser.Text.Trim() + ISIConstants.ISI_USER_SEPRATOR + "%' ");
                sql1.Append(" and (  isnull(SchedulingStartUser,AssignStartUser) like '%" + ISIConstants.ISI_USER_SEPRATOR + tbUser.Text.Trim() + ISIConstants.ISI_USER_SEPRATOR + "%' ");
                sql1.Append("or  isnull(SchedulingStartUser,AssignStartUser) like '%" + ISIConstants.ISI_USER_SEPRATOR + tbUser.Text.Trim() + ISIConstants.ISI_LEVEL_SEPRATOR + "' )");
                //sql1.Append("or  isnull(SchedulingStartUser,AssignStartUser) like '%" + ISIConstants.ISI_LEVEL_SEPRATOR + tbUser.Text.Trim() + ISIConstants.ISI_LEVEL_SEPRATOR + "' )");
            }
            DataSet dataSetAssign = TheSqlHelperMgr.GetDatasetBySql(sql1.ToString(), sqlParam1.ToArray<SqlParameter>());
            AssignUserList = IListHelper.DataTableToList<AssignUser>(dataSetAssign.Tables[0]);

            this.GV_List.DataSource = dataSet;
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    private void GetDateSet(out string startDate, out string endDate, out DataSet dataSet, string sortdirection)
    {
        System.Text.StringBuilder sql = new System.Text.StringBuilder();
        sql.Append("select u.USR_Code Code,u.USR_FirstName + u.USR_LastName Name,u.Dept,u.Dept2,u.JobNo,u.Position,u.IsActive,");
        sql.Append("        isnull(TaskCreate.CreateCount,0) CreateCount,");
        sql.Append("        isnull(TaskSubmit.SubmitCount,0) SubmitCount,TaskSubmit.SubmitDate,");
        sql.Append("        isnull(TaskCancel.CancelCount,0) CancelCount,");
        sql.Append("        isnull(TaskAssign.AssignCount,0) AssignCount,");
        sql.Append("        isnull(TaskSubmitFirst.SubmitFirstCount,0) SubmitFirstCount,");
        sql.Append("        isnull(SubmitInProcessFirst.SubmitInProcessFirstCount,0) SubmitInProcessFirstCount,");
        sql.Append("        isnull(SubmitInProcessFirstStatus.SubmitInProcessFirstStatusCount,0) SubmitInProcessFirstStatusCount,");
        sql.Append("        NoStatus.NoStatusCount NoStatusCount,");
        sql.Append("        isnull(TaskFirst.FirstCount,0) FirstCount,");
        sql.Append("        isnull(TaskStatus.StatusCount,0) StatusCount,TaskStatus.StatusDate,");
        sql.Append("        isnull(TaskFile.FileCount,0) FileCount,");
        sql.Append("        isnull(TaskComment.CommentCount,0) CommentCount,");
        sql.Append("        isnull(TaskClose.CloseCount,0) CloseCount, ");
        sql.Append("        isnull(TaskOpen.OpenCount,0) OpenCount, ");
        sql.Append("        NoStatusAssign.NoStatusAssignCount NoStatusAssignCount,");
        sql.Append("        NoComment.NoCommentCount NoCommentCount ");//作为分派人、执行中、无评论数,包含下属
        sql.Append("from ACC_User u ");
        //创建
        sql.Append("left join ");
        sql.Append("(select Count(1) CreateCount,t.CreateUser from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where ");
        sql.Append("	   t.CreateDate >= @StartDate  and t.CreateDate <= @EndDate ");
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("group by t.CreateUser ) ");
        sql.Append("TaskCreate on TaskCreate.CreateUser=u.USR_Code  ");
        //提交
        sql.Append("left join ");
        sql.Append("(select Count(1) SubmitCount,t.SubmitUser,Max(SubmitDate) SubmitDate from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where  ");
        sql.Append("	   SubmitDate >= @StartDate  and SubmitDate <= @EndDate ");
        sql.Append("  and  SubmitUser is not null and SubmitUser !='' ");
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("group by t.SubmitUser ");
        sql.Append(") TaskSubmit on TaskSubmit.SubmitUser=u.USR_Code ");
        //取消
        sql.Append("left join  ");
        sql.Append("(select Count(1) CancelCount,t.CancelUser from ISI_TaskMstr t   join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where ");
        sql.Append("	   CancelDate >= @StartDate  and CancelDate <= @EndDate ");
        sql.Append("  and  CancelUser is not null and CancelUser !='' ");
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("group by t.CancelUser ");
        sql.Append(") TaskCancel on TaskCancel.CancelUser=u.USR_Code ");
        //分派
        sql.Append("left join ");
        sql.Append("(select Count(1) AssignCount,t.AssignUser from ISI_TaskMstr t   join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where  ");
        sql.Append("	  AssignDate >= @StartDate  and AssignDate <= @EndDate ");
        sql.Append(" and  t.AssignUser is not null and t.AssignUser !='' ");
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("group by t.AssignUser ");
        sql.Append(" ) TaskAssign on TaskAssign.AssignUser=u.USR_Code ");
        //第一责任人(自提)
        sql.Append("left join ");
        sql.Append("(select Count(1) SubmitFirstCount,SubmitUser,");
        sql.Append("(case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser)) ");
        sql.Append("when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("else ");
        sql.Append("substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("end) SubmitFirstUser ");
        sql.Append("from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where  ");
        sql.Append("	   SubmitDate >= @StartDate  and SubmitDate <= @EndDate ");
        sql.Append("  and  SubmitUser is not null and SubmitUser !='' and AssignDate is not null ");
        sql.Append("  and  isnull(SchedulingStartUser,AssignStartUser) is not null and isnull(SchedulingStartUser,AssignStartUser) !='' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by t.SubmitUser, ");
        sql.Append("    ( ");
        sql.Append("        case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser))  ");
        sql.Append("        when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("        else  ");
        sql.Append("        substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("        end) ");
        sql.Append(") TaskSubmitFirst on TaskSubmitFirst.SubmitUser=u.USR_Code and TaskSubmitFirst.SubmitFirstUser= u.USR_Code ");
        //第一责任人(自提/提交/分派/执行中)，不考虑时间范围
        sql.Append("left join ");
        sql.Append("(select Count(1) SubmitInProcessFirstCount,t.SubmitUser,");
        sql.Append("(case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser)) ");
        sql.Append("when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("else ");
        sql.Append("substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("end) SubmitFirstUser ");
        sql.Append("from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where  ");
        //sql.Append("       (t.Status ='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT + "' or t.Status ='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN + "' or t.Status ='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS + "' ) ");
        sql.Append("            ( ");//提交
        sql.Append("                ( t.SubmitDate is not null and t.SubmitDate <= @EndDate and ( t.AssignDate is null or t.AssignDate > @EndDate ) ) ");
        sql.Append("                or ");//分派
        sql.Append("                ( t.AssignDate is not null and t.AssignDate <= @EndDate and ( t.StartDate is null or t.StartDate > @EndDate or t.StartDate < t.AssignDate ) ) ");
        sql.Append("                or ");//执行中
        sql.Append("                ( t.StartDate is not null and t.StartDate <= @EndDate and ( t.CompleteDate is null or t.CompleteDate > @EndDate or t.CompleteDate < t.StartDate ) ) ");
        sql.Append("            ) ");
        //sql.Append("  and  SubmitDate >= @StartDate  and SubmitDate <= @EndDate ");
        //排除取消
        sql.Append("  and  (CancelDate is null or (CancelDate is not null and CancelDate > @EndDate ) ) ");

        sql.Append("  and  SubmitUser is not null and SubmitUser !='' and AssignDate is not null ");
        sql.Append("  and  isnull(SchedulingStartUser,AssignStartUser) is not null and isnull(SchedulingStartUser,AssignStartUser) !='' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by t.SubmitUser, ");
        sql.Append("    ( ");
        sql.Append("        case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser))  ");
        sql.Append("        when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("        else  ");
        sql.Append("        substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("        end) ");
        sql.Append(") SubmitInProcessFirst on SubmitInProcessFirst.SubmitUser=u.USR_Code and SubmitInProcessFirst.SubmitFirstUser= u.USR_Code ");

        //进展(自提/第一责任人/分派/执行中)
        sql.Append("left join ");
        sql.Append("(select Count(1) SubmitInProcessFirstStatusCount,t.SubmitUser,ts.LastModifyUser,");
        sql.Append("(case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser)) ");
        sql.Append("when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("else ");
        sql.Append("substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("end) SubmitFirstUser ");
        sql.Append("from ISI_TaskStatus ts join ISI_TaskMstr t on ts.TaskCode=t.Code  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where  ");
        sql.Append("       (t.Status ='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN + "' or t.Status ='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS + "' ) ");
        sql.Append("      and ");
        sql.Append("            ( ");//分派
        sql.Append("                ( t.AssignDate is not null and t.AssignDate <= @EndDate and ( t.StartDate is null or t.StartDate> @EndDate or t.StartDate<t.AssignDate ) ) ");
        sql.Append("                or ");//执行中
        sql.Append("                ( t.StartDate is not null and t.StartDate <= @EndDate and ( t.CompleteDate is null or t.CompleteDate> @EndDate or t.CompleteDate<t.StartDate ) ) ");
        sql.Append("            ) ");
        sql.Append("  and  ts.LastModifyDate >= @StartDate  and ts.LastModifyDate <= @EndDate ");
        sql.Append("  and  t.SubmitUser is not null and t.SubmitUser !='' and t.AssignDate is not null ");
        sql.Append("  and  isnull(SchedulingStartUser,AssignStartUser) is not null and isnull(SchedulingStartUser,AssignStartUser) !='' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by t.SubmitUser, ");
        sql.Append("    ( ");
        sql.Append("        case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser))  ");
        sql.Append("        when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("        else  ");
        sql.Append("        substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("        end) ,ts.LastModifyUser");
        sql.Append(") SubmitInProcessFirstStatus on SubmitInProcessFirstStatus.SubmitUser=u.USR_Code and SubmitInProcessFirstStatus.SubmitFirstUser= u.USR_Code and SubmitInProcessFirstStatus.LastModifyUser=u.USR_Code ");

        //无进展(第一责任人/执行中)
        sql.Append("left join ");
        sql.Append("(select Count(1) NoStatusCount,");
        sql.Append("(case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser)) ");
        sql.Append("when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("else ");
        sql.Append("substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("end) SubmitFirstUser ");
        sql.Append("from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where  ");
        sql.Append("      t.StartDate is not null and t.StartDate <= @EndDate and ( t.CompleteDate is null or t.CompleteDate> @EndDate or t.CompleteDate<t.StartDate ) ");
        //sql.Append("       t.Status ='" + ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS + "' ");        
        sql.Append("  and NOT EXISTS(select ts.Id from ISI_TaskStatus ts where ts.TaskCode = t.Code ");
        sql.Append("                    and  ts.LastModifyDate >= @StartDate and ts.LastModifyDate <= @EndDate ) ");
        sql.Append("  and  isnull(SchedulingStartUser,AssignStartUser) is not null and isnull(SchedulingStartUser,AssignStartUser) !='' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by ");
        sql.Append("    ( ");
        sql.Append("        case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser))  ");
        sql.Append("        when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("        else  ");
        sql.Append("        substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("        end) ");
        sql.Append(") NoStatus on NoStatus.SubmitFirstUser = u.USR_Code ");

        //第一执行人
        sql.Append("left join  ");
        sql.Append("(select Count(1) FirstCount,");
        sql.Append("(case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser)) ");
        sql.Append("when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("else ");
        sql.Append("substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("end) FirstUser ");
        sql.Append("from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code where isnull(SchedulingStartUser,AssignStartUser) is not null and isnull(SchedulingStartUser,AssignStartUser) !='' ");
        sql.Append(" and  AssignDate >= @StartDate  and AssignDate <= @EndDate ");
        sql.Append(" and  t.AssignUser is not null and t.AssignUser !='' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by  ");
        sql.Append("    ( ");
        sql.Append("        case  CHARINDEX (',' ,isnull(SchedulingStartUser,AssignStartUser))  ");
        sql.Append("        when 0 then substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX ('|',RIGHT( isnull(SchedulingStartUser,AssignStartUser),len(isnull(SchedulingStartUser,AssignStartUser))-1) )-1) ");
        sql.Append("        else  ");
        sql.Append("        substring(isnull(SchedulingStartUser,AssignStartUser),2,CHARINDEX (',',isnull(SchedulingStartUser,AssignStartUser) )-2) ");
        sql.Append("        end))  TaskFirst on TaskFirst.FirstUser=u.USR_Code ");
        //非第一负责人

        //进展
        sql.Append("left join  ");
        sql.Append("( ");
        sql.Append("select Count(1) StatusCount, ts.LastModifyUser,Max(ts.LastModifyDate) StatusDate ");
        sql.Append("from ISI_TaskStatus ts join ISI_TaskMstr t on ts.TaskCode=t.Code  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where ");
        sql.Append("	   ts.LastModifyDate >= @StartDate  and ts.LastModifyDate <= @EndDate ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by ts.LastModifyUser ");
        sql.Append(") TaskStatus  on TaskStatus.LastModifyUser=u.USR_Code ");
        //文件
        sql.Append("left join ");
        sql.Append("(select Count(1) FileCount,att.CreateUser from ISI_AttachmentDet att  join ISI_TaskMstr t on att.TaskCode=t.Code  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where ");
        sql.Append("	   att.CreateDate >= @StartDate  and att.CreateDate <= @EndDate ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by att.CreateUser ");
        sql.Append(") TaskFile on TaskFile.CreateUser=u.USR_Code ");
        //评论
        sql.Append("left join ");
        sql.Append("( ");
        sql.Append("select Count(1) CommentCount, c.LastModifyUser ");
        sql.Append("from ISI_CommentDet c join ISI_TaskMstr t on c.TaskCode=t.Code  join ISI_TaskSubType tst on t.TaskSubType = tst.Code ");
        sql.Append("where ");
        sql.Append("	   c.LastModifyDate >= @StartDate  and c.LastModifyDate <= @EndDate ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by c.LastModifyUser ");
        sql.Append(") TaskComment  on TaskComment.LastModifyUser=u.USR_Code ");
        //关闭
        sql.Append("left join ");
        sql.Append("(select Count(1) CloseCount,t.CloseUser from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code  ");
        sql.Append("where  ");
        sql.Append("	  CloseDate >= @StartDate  and CloseDate <= @EndDate ");
        sql.Append(" and  CloseUser is not null and CloseUser !='' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by t.CloseUser ");
        sql.Append(") TaskClose on TaskClose.CloseUser=u.USR_Code ");

        //开启

        sql.Append("left join ");
        sql.Append("(select Count(1) OpenCount,t.OpenUser from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code  ");
        sql.Append("where ");
        sql.Append("	  OpenDate >= @StartDate  and OpenDate <= @EndDate ");
        sql.Append(" and  OpenUser is not null and OpenUser !='' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        sql.Append("group by t.OpenUser ");
        sql.Append(") TaskOpen on TaskOpen.OpenUser=u.USR_Code ");

        //作为分派人、执行中、无进展数,包含下属
        sql.Append("left join ");
        sql.Append("(select Count(1) NoStatusAssignCount,t.AssignUser from ISI_TaskMstr t  join ISI_TaskSubType tst on t.TaskSubType = tst.Code  ");
        sql.Append("where  ");
        //执行中
        sql.Append("      t.StartDate is not null and t.StartDate <= @EndDate and ( t.CompleteDate is null or t.CompleteDate> @EndDate or t.CompleteDate<t.StartDate ) ");
        sql.Append("  and NOT EXISTS(select ts.Id from ISI_TaskStatus ts where ts.TaskCode = t.Code ");
        sql.Append("                    and  ts.LastModifyDate >= @StartDate and ts.LastModifyDate <= @EndDate ) ");
        sql.Append(" and  t.AssignUser is not null and t.AssignUser !='' ");
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("group by t.AssignUser ");
        sql.Append(" ) NoStatusAssign on NoStatusAssign.AssignUser=u.USR_Code ");


        //作为分派人、执行中、无评论数,包含下属
        sql.Append("left join ");
        sql.Append("(select Count(1) NoCommentCount,t.AssignUser from ISI_TaskMstr t join ISI_TaskSubType tst on t.TaskSubType = tst.Code  ");
        sql.Append("where  ");
        //执行中
        sql.Append("      t.StartDate is not null and t.StartDate <= @EndDate and ( t.CompleteDate is null or t.CompleteDate> @EndDate or t.CompleteDate<t.StartDate ) ");
        sql.Append("  and NOT EXISTS(select c.Id from ISI_CommentDet c where c.TaskCode = t.Code ");
        sql.Append("                    and  c.LastModifyDate >= @StartDate and c.LastModifyDate <= @EndDate ) ");
        sql.Append(" and  t.AssignUser is not null and t.AssignUser !='' ");
        if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
        {
            sql.Append("  and  t.Type = '" + this.ddlType.SelectedValue + "' ");
        }
        sql.Append("  and  t.Type != '" + ISIConstants.ISI_TASK_TYPE_PRIVACY + "' ");
        if (!string.IsNullOrEmpty(this.tbTaskSubType.Text.Trim()))
        {
            sql.Append("  and  t.TaskSubType = '" + this.tbTaskSubType.Text.Trim() + "' ");
        }
        if (!string.IsNullOrEmpty(this.ddlOrg.SelectedValue))
        {
            sql.Append("  and tst.Org = '" + this.ddlOrg.SelectedValue + "' ");
        }
        sql.Append("group by t.AssignUser ");
        sql.Append(" ) NoComment on NoComment.AssignUser=u.USR_Code ");

        System.Text.StringBuilder where = new System.Text.StringBuilder();
        where.Append("where u.IsActive = @IsActive ");

        string dept2 = this.tbDept2.Text.Trim();
        string dept = this.ddlDept.SelectedValue;
        string position = ddlPosition.SelectedValue;
        IList<SqlParameter> sqlParam = new List<SqlParameter>();
        sqlParam.Add(new SqlParameter("@IsActive", ckIsActive.Checked));

        if (this.tbUser.Text.Trim() != string.Empty)
        {
            where.Append("and u.USR_Code = @Code ");
            sqlParam.Add(new SqlParameter("@Code", tbUser.Text.Trim()));
        }

        if (!string.IsNullOrEmpty(position))
        {
            where.Append("and u.Position = @Position ");
            sqlParam.Add(new SqlParameter("@Position", position));
        }
        if (!string.IsNullOrEmpty(dept))
        {
            where.Append("and u.Dept = @Dept ");
            sqlParam.Add(new SqlParameter("@Dept", dept));
        }
        if (!string.IsNullOrEmpty(dept2))
        {
            where.Append("and u.Dept2 = @Dept2 ");
            sqlParam.Add(new SqlParameter("@Dept2", dept2));
        }

        startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        sqlParam.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
        sqlParam.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));

        sql.Append(where.ToString());

        if (!string.IsNullOrEmpty(sortdirection) && !string.IsNullOrEmpty(this.GridViewSortExpression))
        {
            sql.Append(" order by " + GridViewSortExpression + " " + sortdirection);
        }

        dataSet = TheSqlHelperMgr.GetDatasetBySql(sql.ToString(), sqlParam.ToArray<SqlParameter>());
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var statistics = ((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray;

            int index = 7;

            for (int rowCount = 5; rowCount <= 23; rowCount++)
            {
                if (rowCount == 7 || rowCount == 17)
                {
                    index++;
                    continue;
                }
                if (rowCount == 15)
                {
                    var startedCount = AssignUserList.Where(u =>
                        //    u.StartedUser.Contains(ISIConstants.ISI_LEVEL_SEPRATOR + statistics[0].ToString() + ISIConstants.ISI_USER_SEPRATOR)
                                                           u.StartedUser.Contains(ISIConstants.ISI_USER_SEPRATOR + statistics[0].ToString() + ISIConstants.ISI_USER_SEPRATOR)
                                                        || u.StartedUser.Contains(ISIConstants.ISI_USER_SEPRATOR + statistics[0].ToString() + ISIConstants.ISI_LEVEL_SEPRATOR)
                        //|| u.StartedUser.Contains(ISIConstants.ISI_LEVEL_SEPRATOR + statistics[0].ToString() + ISIConstants.ISI_LEVEL_SEPRATOR)
                                                        ).Count();

                    e.Row.Cells[rowCount].Text = startedCount == 0 ? string.Empty : startedCount.ToString();
                }
                else
                {
                    e.Row.Cells[rowCount].Text = statistics[index].ToString() == "0" ? string.Empty : statistics[index].ToString();
                    index++;
                }
            }
            bool visible = false;
            for (int i = 5; i < e.Row.Cells.Count; i++)
            {
                TableCell cell = e.Row.Cells[i];
                if (cell.Text != string.Empty && cell.Text != "&nbsp;" && cell.Text != "0")
                {
                    visible = true;
                    break;
                }
            }
            e.Row.Visible = visible;
        }
    }
    [Serializable]
    public class AssignUser
    {
        public string StartedUser { get; set; }
    }

    public string GridViewSortExpression
    {
        get
        {
            if (ViewState["sortExpression"] == null)
                ViewState["sortExpression"] = "itop";             //默认排序字段
            return ViewState["sortExpression"].ToString();
        }

        set { ViewState["sortExpression"] = value; }
    }

    public SortDirection GridViewSortDirection
    {
        get
        {
            if (ViewState["sortDirection"] == null)
                ViewState["sortDirection"] = SortDirection.Ascending;          //默认升序
            return (SortDirection)ViewState["sortDirection"];
        }

        set { ViewState["sortDirection"] = value; }
    }

    protected void GV_List_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression = e.SortExpression;
        GridViewSortExpression = sortExpression;       //存到ViewState中排序的字段:newsid     
        if (GridViewSortExpression == sortExpression)
        {
            if (GridViewSortDirection == SortDirection.Ascending) //设置排序方向
            {
                GridViewSortDirection = SortDirection.Descending;
            }
            else
            {
                GridViewSortDirection = SortDirection.Ascending;
            }
        }
        this.InitGridView();
    }

    protected void InitGridView()
    {
        string sortdirection;
        if (GridViewSortDirection == SortDirection.Ascending)
        {
            sortdirection = "asc";
        }
        else
        {
            sortdirection = "desc";
        }
        string startDate;
        string endDate;
        DataSet dataSet;
        GetDateSet(out startDate, out endDate, out dataSet, sortdirection);
        this.GV_List.DataSource = dataSet.Tables[0].DefaultView;
        this.GV_List.DataBind();
    }

}