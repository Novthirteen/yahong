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
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using Whidsoft.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity;
using System.Reflection;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

public partial class ISI_OrgChart_View : MainModuleBase
{
    protected void Page_Load(object sender, System.EventArgs e)
    {
        DataTable userTable = TheSqlHelperMgr.GetDatasetBySql(
        @"select b.WorkShop as WorkShop,a.USR_Code  as UserCode,a.USR_FirstName+isnull(a.USR_LastName,'') as UserName , 
        d.Name as RoleName from ACC_User a,ISI_ResMatrix b,isi_resuser c,ISI_ResRole d
        where a.USR_Code = c.UserCode and b.Id= c.MatrixId and d.Code=b.Role and a.IsActive= 1
        and c.EndDate>GETDATE()
        group by b.WorkShop,a.USR_FirstName,a.USR_LastName,a.USR_Code, d.Name
        order by b.WorkShop,d.Name desc ").Tables[0];

        var workShopUserDic = IListHelper.DataTableToList<WorkShopUser>(userTable)
            .GroupBy(p => p.WorkShop).ToDictionary(p => p.Key, p => p.GroupBy(q => new { q.UserCode }).Select(q =>
                {
                    var r = q.First();
                    r.RoleName = string.Join("<br/>", q.Select(s => s.RoleName).ToArray());
                    return r;
                }
                ).ToList());

        var workShopList = this.TheGenericMgr.FindAll<ResWokShop>("from ResWokShop r where r.IsActive= 1 ").ToList();
        var jsonObjectList = workShopList.Select(p => new JsonObject
                            {
                                Code = p.Code,
                                Name = p.Name,
                                ParentCode = p.ParentCode,
                                ToolTips = GetDetail(p, workShopUserDic.ValueOrDefault(p.Code))
                            });
        JavaScriptSerializer json = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
        this.hfa.Value = json.Serialize(jsonObjectList);
    }

    private string GetDetail(ResWokShop resWokShop, List<WorkShopUser> workShopUserList)
    {
        var detail = new StringBuilder();
        if (workShopUserList != null)
        {
            detail.AppendFormat("cssbody=[obbd] cssheader=[obhd] header=[{0}  共{1}人] body=[<table width=100%>", resWokShop.Code, workShopUserList.Select(p => p.UserCode).Distinct().Count());

            for (int i = 0; i < workShopUserList.Count; i += 3)
            {
                var od1 = workShopUserList[i];
                var od2 = new WorkShopUser();
                var od3 = new WorkShopUser();
                if (i < workShopUserList.Count - 1)
                {
                    od2 = workShopUserList[i + 1];
                }
                if (i < workShopUserList.Count - 2)
                {
                    od3 = workShopUserList[i + 2];
                }
                detail.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>",
                    od1.UserName, od1.RoleName, od2.UserName, od2.RoleName, od3.UserName, od3.RoleName);
            }
            detail.Append("</table>]");
        }
        return detail.ToString();
    }

    public class JsonObject
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
        public string ToolTips { get; set; }
    }

    public class WorkShopUser
    {
        public string WorkShop { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}


