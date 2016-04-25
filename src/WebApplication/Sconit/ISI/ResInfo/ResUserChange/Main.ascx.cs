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

public partial class ISI_ResInfo_ResUserChange_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string tbStartDate = this.tbStartDate.Text.Trim();
        string tbEndDate = this.tbEndDate.Text.Trim();

        string tableName = "ISI_ResWokShop";
        if (rblType.SelectedValue =="Role")
        {
            tableName = "ISI_ResRole";
        }

        string sql = string.Format(@"
        SELECT T.UserCode,u.USR_FirstName + isnull(u.USR_LastName,'') as UserName,T.{0} into #StartRes from  
            (select *,ROW_NUMBER()over(partition by ResMatrixId,UserCode order by Id desc) As NONO 
            from ISI_ResMatrixLog) T
            join ACC_User u on u.USR_Code = T.UserCode
            where NONO =1 and Action<>'Delete'
            and CreateDate<@StartDate and @StartDate>= StartDate and @StartDate < EndDate 
            group by T.UserCode,u.USR_FirstName,u.USR_LastName,T.{0}

        SELECT T.UserCode,u.USR_FirstName + isnull(u.USR_LastName,'') as UserName,T.{0} into #EndRes from  
            (select *,ROW_NUMBER()over(partition by ResMatrixId,UserCode order by Id desc) As NONO 
            from ISI_ResMatrixLog ) T
            join ACC_User u on u.USR_Code = T.UserCode
            where NONO =1 and Action<>'Delete'
            and CreateDate<@EndDate and @EndDate>= StartDate and @EndDate < EndDate
            group by T.UserCode,u.USR_FirstName,u.USR_LastName,T.{0}

        select {0},COUNT(1) as UserCount,
        stuff((SELECT ',' + UserCode+'['+UserName+']' from #EndRes as t where t.{0} = p.{0} group by UserCode,UserName order by UserName FOR xml path('')), 1, 1, '') as UserNames
        into #EndRes1
        from #EndRes p
        group by {0}

        select {0},COUNT(1) as UserCount,
        stuff((SELECT ',' + UserCode+'['+UserName+']' from #StartRes as t where t.{0} = p.{0} group by UserCode,UserName order by UserName FOR xml path('')), 1, 1, '') as UserNames
        into #StartRes1
        from #StartRes p
        group by {0}

        select isnull(a.{0},b.{0}) as {0},SPACE(50) as {0}Name,
        b.UserNames as UserNames,b.UserCount as NewCount,a.UserNames as OldUserNames,
        a.UserCount as OldCount,
        case when b.{0} is null then '删除' when a.{0} is null then '新增' 
        when a.UserNames=b.UserNames then '无变化' else '变更' end as Mark
        into #result
        from #StartRes1 a full join #EndRes1 b on a.{0}=b.{0}
        --where a.UserNames<>b.UserNames or b.{0} is null or a.{0} is null
        order by {0}
        update a set a.{0}Name =b.Name from #result a,{1} b where a.{0} = b.Code

        select {0} as Code,{0}Name as Name,UserNames,NewCount,OldUserNames,OldCount,Mark from #result
        ", this.rblType.SelectedValue, tableName);

        SqlParameter[] sqlParam = new SqlParameter[2];
        sqlParam[0] = new SqlParameter("@StartDate", tbStartDate);
        sqlParam[1] = new SqlParameter("@EndDate", tbEndDate);

        DataSet dataSetInv = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);
        List<Res> responsibilityList = IListHelper.DataTableToList<Res>(dataSetInv.Tables[0]);

        this.GV_List.DataSource = responsibilityList;
        this.GV_List.DataBind();
        this.fs.Visible = true;
        if ((Button)sender == this.btnExport)
        {
            this.Export(this.GV_List, "application/ms-excel", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xls");
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Res responsibility = (Res)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (responsibility.Mark != "无变化")
            {
                e.Row.Cells[3].Text = DiffMatchPatchHelper.DiffPrettyHtml(responsibility.OldUserNames, responsibility.UserNames);
            }
            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
        }
        else
        {
            if (rblType.SelectedValue == "Role")
            {
                e.Row.Cells[1].Text = "岗位";
                e.Row.Cells[2].Text = "岗位描述";
            }
        }
    }

    protected void GV_List_Attachment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        AttachmentDetail attachmentDetail = (AttachmentDetail)e.Row.DataItem;
        LinkButton lbtnDownLoad = (LinkButton)e.Row.FindControl("lbtnDownLoad");
        if (lbtnDownLoad != null)
        {
            lbtnDownLoad.Text = attachmentDetail.FileName;
        }
    }

    class Res
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string UserNames { get; set; }
        public int NewCount { get; set; }
        public string OldUserNames { get; set; }
        public int OldCount { get; set; }
        public string Mark { get; set; }
    }
}