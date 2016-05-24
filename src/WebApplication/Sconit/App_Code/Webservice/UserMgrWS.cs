using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using System.Collections.Generic;
using com.Sconit.Web;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;
using System.Text;

/// <summary>
/// Summary description for FlowMgrWS
/// </summary>
[WebService(Namespace = "http://com.Sconit.Webservice/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class UserMgrWS : BaseWS
{
    public UserMgrWS()
    {

    }

    [WebMethod]
    public string[] GetAllUser()
    {
        try
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select u.Code,u.FirstName,u.LastName from User u where u.IsActive=1 ");

            IList<object[]> userList = this.TheHqlMgr.FindAll<object[]>(sql.ToString());

            if ((userList != null && userList.Count > 0))
            {
                //sql = new StringBuilder();
                //sql.Append("select r.Code,r.Description,count(u.Code) from UserRole ur join ur.Role r join ur.User u where r.IsISIRole=1 and u.IsActive=1 group by r.Code,r.Description");
                //var roleList = this.TheHqlMgr.FindAll<object[]>(sql.ToString());

                //if (roleList != null && roleList.Count > 0)
                //{
                //    var users = userList.Select(u => u[0].ToString() + "[" + u[1].ToString() + " " + u[2].ToString() + "]")
                //                    .Union(roleList.Select(r => "Role_" + r[0].ToString() + "[" + r[1].ToString() + r[2].ToString() + "]")).ToArray<string>();
                //    return users;
                //}
                //else
                //{
                    var users = userList.Select(u => u[0].ToString() + "[" + u[1].ToString() + " " + u[2].ToString() + "]").ToArray<string>();
                    return users;
                //}
            }
            else
            {
                return null;
            }

        }
        catch (Exception ex)
        {
            throw new SoapException(ex.Message, SoapException.ServerFaultCode, Context.Request.Url.AbsoluteUri);
        }
    }
}