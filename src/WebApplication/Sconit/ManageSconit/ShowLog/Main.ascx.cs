using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Web;
using com.Sconit.Entity;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using com.Sconit.Utility;

public partial class ManageSconit_ShowLog_Main : MainModuleBase
{
    private string logDir
    {
        get
        {
            string companyName = TheEntityPreferenceMgr.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
            string dir = companyName.ToLower().Contains("test") ? "test" : string.Empty;
            if (this.rblType.SelectedIndex == 0)
            {
                return @"D:\logs\Web" + dir + @"\";
            }
            else
            {
                return @"D:\logs\windowsservice\";
            }
        }
    }

    private List<Logview> logviews
    {
        get
        {
            FileStream fs = new FileStream(logDir + ddlLogFile.SelectedValue, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            string log = sr.ReadToEnd();
            string[] lines = Regex.Split(log, "\r\n", RegexOptions.IgnoreCase);

            List<Logview> logviews_ = new List<Logview>();
            foreach (string line in lines)
            {
                string[] strs = line.Split('|');
                if (strs.Length > 4)
                {
                    Logview logview_ = new Logview();
                    DateTime date = DateTime.Now.Date;
                    DateTime.TryParse(strs[0], out date);
                    logview_.Date = date;
                    logview_.Thread = strs[1].Trim();
                    logview_.Level = strs[2].Trim();
                    logview_.Logger = strs[3].Trim();
                    logview_.Message = strs[4].Trim();
                    logviews_.Add(logview_);
                }
                else
                {
                    logviews_.Last().Exception += line.Trim();
                }
            }
            fs.Dispose();
            return logviews_;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.rblType_Change(sender, e);
            this.ddlLogFile_Change(sender, e);
            this.tbDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }

    protected void rblType_Change(object sender, EventArgs e)
    {
        DirectoryInfo di = new System.IO.DirectoryInfo(logDir);
        FileInfo[] fimore = di.GetFiles();
        this.ddlLogFile.DataSource = fimore.Select(f => f.Name);
        this.ddlLogFile.DataBind();

    }
    protected void ddlLogFile_Change(object sender, EventArgs e)
    {
        List<string> loggers = logviews.GroupBy(l => l.Logger).Select(l => l.Key).ToList();
        loggers.Add("ALL");
        loggers.Sort();
        this.ddlLogger.DataSource = loggers;
        this.ddlLogger.DataBind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        var newLogView = logviews.Where(l => (ddlLevel.SelectedValue == "ALL" || StringHelper.Eq(l.Level.Trim(), ddlLevel.SelectedValue.Trim()))
            && (ddlLogger.SelectedValue == "ALL" || StringHelper.Eq(l.Logger.Trim(), ddlLogger.SelectedValue.Trim()))
            && (this.tbDateTime.Text == string.Empty || l.Date > DateTime.Parse(this.tbDateTime.Text)));

        this.GV_List.DataSource = newLogView.OrderByDescending(l => l.Date).Take(1000);
        this.GV_List.DataBind();
        this.fldList.Visible = newLogView.Count() > 0;
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[4].Text = this.breakString(e.Row.Cells[4].Text, 100);
            e.Row.Cells[5].Text = this.breakString(e.Row.Cells[5].Text, 150);
        }
    }

    class Logview
    {
        public DateTime Date { get; set; }
        public string Thread { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }

    private string breakString(string content, int length)
    {
        if (length == 0)
        {
            length = 50;
        }
        string strTemp = string.Empty;
        while (content.Length > length)
        {
            strTemp += content.Substring(0, length) + "<br/>";
            content = content.Substring(length, content.Length - length);
        }
        strTemp += content;
        return strTemp;
    }
}
