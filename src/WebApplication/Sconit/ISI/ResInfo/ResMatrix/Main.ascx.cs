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

public partial class ISI_ResInfo_ResMatrix_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {

        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        var sql = @"select u.UserName as 用户,m.WorkShop as 作业区,s.Name as 作业区描述,sop.Operate as 工位,sop.OperateDesc as 工位描述, 
                    m.[Role] as 角色,r.Name as 角色描述,r.RoleType as 类型,
                    u.[Priority] as 优先级,u.SkillLevel as 技能,m.Responsibility as 职责
                    into #temp
                    from dbo.ISI_ResMatrix as m
                    left join ISI_ResRole r on m.[Role] = r.Code
                    left join ISI_ResWokShop s on m.WorkShop = s.Code
                    left join ISI_ResSop sop on m.WorkShop = sop.WorkShop and m.Operate = sop.Operate
                    left join ISI_ResUser u on m.Id = u.MatrixId 
                    where  u.StartDate<=GETDATE() and u.EndDate>GETDATE()
                    and m.Operate is not null  

                    insert into #temp
                    select u.UserName as 用户,m.WorkShop as 作业区,s.Name as 作业区描述,sop.Operate as 工位,sop.OperateDesc as 工位描述, 
                    m.[Role] as 角色,r.Name as 角色描述, r.RoleType as 类型,
                    u.[Priority] as 优先级,u.SkillLevel as 技能,m.Responsibility as 职责
                    from dbo.ISI_ResMatrix as m
                    left join ISI_ResRole r on m.[Role] = r.Code
                    left join ISI_ResWokShop s on m.WorkShop = s.Code
                    left join ISI_ResSop sop on m.WorkShop = sop.WorkShop
                    left join ISI_ResUser u on m.Id = u.MatrixId
                    where u.StartDate<=GETDATE() and u.EndDate>GETDATE()
                    and m.Operate is null

                    update a set a.类型 = b.Desc1 from  #temp a, CodeMstr b where b.Code ='ISIResRoleType' and b.CodeValue = a.类型
                    update a set a.技能 = b.Desc1 from  #temp a, CodeMstr b where b.Code ='ISISkillLevel' and b.CodeValue = a.技能

                    select * from #temp order by 用户,角色,作业区

                    drop table #temp
                   ";

        DataSet dataSetInv = TheSqlHelperMgr.GetDatasetBySql(sql);
        this.Gv_Export.DataSource = dataSetInv;
        this.Gv_Export.DataBind();
        this.ExportXLS(this.Gv_Export);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ResMatrix));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ResMatrix))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("Id")));
        string workshop = this.tbWorkShop.Text.Trim();
        if(workshop != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("WorkShop", workshop));
            selectCountCriteria.Add(Expression.Eq("WorkShop", workshop));
        }
        string operate = this.tbOperate.Text.Trim();
        if (operate != string.Empty)
        {
            int op = 0;
            bool r = int.TryParse(operate, out op);
            if (r)
            {
                selectCriteria.Add(Expression.Eq("Operate", op));
                selectCountCriteria.Add(Expression.Eq("Operate", op));
            }
            else
            {
                selectCriteria.Add(Expression.IsNotNull("Operate"));
                selectCountCriteria.Add(Expression.IsNotNull("Operate"));
            }
        }
        string role = this.tbRole.Text.Trim();
        if(role != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Role", role));
            selectCountCriteria.Add(Expression.Eq("Role", role));
        }
        selectCriteria.AddOrder(Order.Asc("WorkShop"));
        selectCriteria.AddOrder(Order.Asc("Operate"));
        selectCriteria.AddOrder(Order.Asc("Seq"));
        this.SetSearchCriteria(selectCriteria, selectCountCriteria);
        this.fldList.Visible = true;
        this.UpdateView();
    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnDownLoad_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        string fileName = string.Empty;
        try
        {
            AttachmentDetail attachment = this.TheAttachmentDetailMgr.LoadAttachmentDetail(int.Parse(id));

            this.DownLoadFile(attachment);
        }
        catch(Castle.Facilities.NHibernateIntegration.DataException ex)
        {

        }
    }

    //  下载文件类
    public void DownLoadFile(AttachmentDetail attachment)
    {
        string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");
        // 保存文件的虚拟路径
        //string Url = "File\\" + FullFileName;
        // 保存文件的物理路径
        string FullPath = absolutePath + attachment.Path;// HttpContext.Current.Server.MapPath(Url);
        // 初始化FileInfo类的实例，作为文件路径的包装
        FileInfo FI = new FileInfo(FullPath);
        // 判断文件是否存在
        if(FI.Exists)
        {
            // 将文件保存到本机
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(attachment.FileName));
            Response.AddHeader("Content-Length", FI.Length.ToString());
            Response.ContentType = attachment.ContentType;
            Response.Filter.Close();
            Response.WriteFile(FI.FullName);
            Response.End();
        }
    }
  
    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if(e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[4].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
            e.Row.Cells[5].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");

            ResMatrix resMatrix = (ResMatrix)(e.Row.DataItem);
            if(!string.IsNullOrEmpty(resMatrix.WorkShop))
            {
                e.Row.Cells[1].Text = this.TheResWokShopMgr.LoadResWokShop(resMatrix.WorkShop).CodeName;
            }

            if (resMatrix.Operate != null)
            {
                var sop = this.TheResSopMgr.LoadResSop(resMatrix.WorkShop, resMatrix.Operate.Value);
                e.Row.Cells[2].Text = sop.Operate + "[" + sop.OperateDesc + "]";
            }

            if(!string.IsNullOrEmpty(resMatrix.Role))
            {
                e.Row.Cells[3].Text = this.TheResRoleMgr.LoadResRole(resMatrix.Role).CodeName;
            }
            //e.Row.Cells[4].Style.Add("min-width", "45px");
            //e.Row.Cells[5].Style.Add("min-width", "55px");

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ResUser));
            selectCriteria.Add(Expression.Eq("MatrixId", resMatrix.Id));
            selectCriteria.Add(Expression.Le("StartDate", DateTime.Now));
            selectCriteria.Add(Expression.Gt("EndDate", DateTime.Now));
            var userList = TheCriteriaMgr.FindAll<ResUser>(selectCriteria) ?? new List<ResUser>();

            e.Row.Cells[5].Text = string.Empty;
            foreach(var user in userList)
            {
                var userName = string.IsNullOrEmpty(user.SkillLevel) ? user.UserName : user.UserName + "[" + user.SkillLevel + "]";
                if(e.Row.Cells[5].Text == string.Empty)
                {
                    e.Row.Cells[5].Text = userName;
                }
                else
                {
                    e.Row.Cells[5].Text += "," + userName;
                }
            }
        }
    }

    protected void GV_List_Attachment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        AttachmentDetail attachmentDetail = (AttachmentDetail)e.Row.DataItem;
        LinkButton lbtnDownLoad = (LinkButton)e.Row.FindControl("lbtnDownLoad");
        if(lbtnDownLoad != null)
        {
            lbtnDownLoad.Text = attachmentDetail.FileName;
        }
    }

    protected void GV_Export_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if(e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[11].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
        }
    }

}