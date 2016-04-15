using System;
using System.Web;
using System.Linq;
using System.Web.Security;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using System.Net;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;

public partial class Login : com.Sconit.Web.PageBase
{
    protected string PicDate
    {
        get { return (string)ViewState["PicDate"]; }
        set { ViewState["PicDate"] = value; }
    }

    protected string LoginImagePath
    {
        get { return (string)ViewState["LoginImagePath"]; }
        set { ViewState["LoginImagePath"] = value; }
    }

    private IList<EntityPreference> entityPerferences;
    private static log4net.ILog log = log4net.LogManager.GetLogger("Log.Login");

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Permission = BusinessConstants.PERMISSION_NOTNEED_CHECK_PERMISSION;
        //Response.Redirect("~/Index.aspx");

        entityPerferences = TheEntityPreferenceMgr.GetAllEntityPreference();

        if(!Page.IsPostBack)
        {
            string LoginImageDir = (from en in entityPerferences where en.Code == "LoginImageDir" select en.Value).FirstOrDefault();
            LoginImageDir = LoginImageDir.EndsWith("/") ? LoginImageDir : LoginImageDir + "/";
            string random;
            if(LoginImageDir.ToLower().StartsWith("http"))//远程图片
            {
                random = ThemeHelper.GetRandomDate();
            }
            else //本地图片,1-100,找图片存在的
            {
                //DateTime dateTimeMin = Convert.ToDateTime("2009-10-31");
                //DateTime dateTimeMax = Convert.ToDateTime("2009-11-30");
                //random = ThemeHelper.GetRandomDate(dateTimeMin, dateTimeMax);
                random = "1";
                LoginImageDir = LoginImageDir.ToLower().StartsWith("~/") ? LoginImageDir.Substring(2) : LoginImageDir;
            }

            if(Request.Cookies["PicDate"] == null || true)
            {
                this.PicDate = random;
            }
            else
            {
                this.PicDate = Request.Cookies["PicDate"].Value;
            }

            LoginImagePath = LoginImageDir + this.PicDate + ".jpg";
            HttpCookie randomPicDate = new HttpCookie("RandomPicDate");
            randomPicDate.Value = LoginImagePath;
            Response.Cookies.Add(randomPicDate);

            //LoginPage
            HttpCookie loginPageCookie = new HttpCookie("LoginPage");
            loginPageCookie.Value = "~/Login.aspx";
            Response.Cookies.Add(loginPageCookie);
        }

        this.login_bg_div.Attributes.Add("style", "background: url(" + LoginImagePath + ") no-repeat;");
        this.divAddTheme.Attributes.Add("onclick", "setCookie('" + PicDate + "')");
        this.Title = (from en in entityPerferences where en.Code == "CompanyName" select en.Value).FirstOrDefault();
        this.Documentation.NavigateUrl = (from en in entityPerferences where en.Code == "DocumentationURL" select en.Value).FirstOrDefault();
        this.Wiki.NavigateUrl = (from en in entityPerferences where en.Code == "WikiURL" select en.Value).FirstOrDefault();
        this.Forum.NavigateUrl = (from en in entityPerferences where en.Code == "ForumURL" select en.Value).FirstOrDefault();

        string companyCode = entityPerferences.Where(en => StringHelper.Eq(en.Code, "CompanyCode")).SingleOrDefault().Value;
        string imagePath = "Images/OEM/" + companyCode;
        this.imgLogo.ImageUrl = imagePath + "/Logo_lit.png";
        if(this.Title.ToLower().Contains("test"))
        {
            this.imgTest.Visible = true;
        }
    }

    protected void Login_Click(object sender, EventArgs e)
    {
        string userCode = this.txtUsername.Value.Trim().ToLower();
        string password = this.txtPassword.Value.Trim();

        string ipAdd = Request.UserHostAddress.ToString();
        string logInfo = " - IP:" + ipAdd + " User:" + userCode;

        //var q = entityPerferences.Where(en => StringHelper.Eq(en.Code, "isShutDown"));
        //if (StringHelper.Eq(this.GetEntityPreference("isShutDown"), "TRUE"))
        //{
        //    log.Error("System is shutdown" + logInfo);
        //    errorLabel.Text = "System is shutdown!";
        //    return;
        //}

        string ipFilter = (from en in entityPerferences where en.Code == "isEnableIPFilter" select en.Value).FirstOrDefault();
        bool isEnableIPFilter = ipFilter.ToUpper() == "TRUE" ? true : false;

        if(isEnableIPFilter && !IPHelper.IsInnerIP(ipAdd))
        {
            log.Error("IPFilter:Not in permit ip range" + logInfo);
            return;
        }

        password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");

        if(userCode.Equals(string.Empty))
        {
            errorLabel.Text = "Please enter your Account!";
            return;
        }

        User user = TheUserMgr.LoadUser(userCode, false, false);

        if(user == null)
        {
            errorLabel.Text = "Identification failure. Try again!";
            log.Error("User code not exsit" + logInfo);
        }
        else
        {
            if(password == user.Password && user.IsActive)
            {
                this.Session["Current_User"] = TheUserMgr.LoadUser(userCode, true, true);
                log.Info("Login successfully." + logInfo + " [" + user.Name + "]");
                Response.Redirect("~/Default.aspx");
            }
            else if(password != user.Password)
            {
                log.Error("Identification failure." + logInfo + " [" + user.Name + "]");
            }
            else if(!user.IsActive)
            {
                errorLabel.Text = "Identification failure. Try again!";
                log.Error("User is inactive." + logInfo + " [" + user.Name + "]");
            }
            else
            {
                log.Error("Identification failure." + logInfo + " [" + user.Name + "]");
            }
        }
    }

    private string GetRandomDate(int max)
    {
        Random r = new Random();
        int t1 = r.Next(1, max);
        return t1.ToString();
    }

    private string GetEntityPreference(string code)
    {
        var q = entityPerferences.Where(en => StringHelper.Eq(en.Code, code));
        if(q != null && q.Count() > 0)
        {
            return q.First().Code;
        }
        return string.Empty;
    }
}