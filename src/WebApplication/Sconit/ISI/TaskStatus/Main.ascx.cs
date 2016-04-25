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
using com.Sconit.Entity;
using NHibernate.Expression;
using System.Collections.Generic;

public partial class ISI_TaskStatus_Main : MainModuleBase
{
    public event EventHandler BackEvent;

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

    public string TaskCode
    {
        get
        {
            return (string)ViewState["TaskCode"];
        }
        set
        {
            ViewState["TaskCode"] = value;
        }
    }
    public void InitPageParameter(string taskCode, string moduleType)
    {
        this.TaskCode = taskCode;
        this.ModuleType = moduleType;

        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucNew.Visible = false;
        this.ucEdit.Visible = false;

        IDictionary<string, string> mpDic = new Dictionary<string, string>();
        mpDic.Add("TaskCode", this.TaskCode);
        mpDic.Add("ModuleType", this.ModuleType);
        this.ucSearch.QuickSearch(mpDic);

    }
    private void PageCleanup()
    {
        this.TaskCode = null;
        this.ucSearch.TaskCode = null;
        this.ucEdit.TaskCode = null;
        this.ucList.TaskCode = null;
        this.ucNew.TaskCode = null;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucSearch.NewEvent += new System.EventHandler(this.New_Render);
        this.ucSearch.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucList.EditEvent += new System.EventHandler(this.ListEdit_Render);
        this.ucNew.BackEvent += new System.EventHandler(this.NewBack_Render);
        this.ucNew.CreateEvent += new System.EventHandler(this.CreateBack_Render);
        this.ucNew.NewEvent += new System.EventHandler(this.New_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);

        if (!IsPostBack)
        {
            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
            if (this.Action == BusinessConstants.PAGE_NEW_ACTION)
            {
                New_Render(this, null);
            }
            if (this.Action == BusinessConstants.PAGE_EDIT_ACTION)
            {
                ListEdit_Render(this.ActionParameter["Code"], null);
            }
        }
        this.ucSearch.ModuleType = this.ModuleType;
        this.ucEdit.ModuleType = this.ModuleType;
        this.ucList.ModuleType = this.ModuleType;
        this.ucNew.ModuleType = this.ModuleType;

        this.ucSearch.TaskCode = this.TaskCode;
        this.ucEdit.TaskCode = this.TaskCode;
        this.ucList.TaskCode = this.TaskCode;
        this.ucNew.TaskCode = this.TaskCode;
    }


    protected void Back_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1]);
        this.ucList.Visible = true;
        this.ucList.UpdateView();
        this.CleanMessage();
    }

    //The event handler when user click button "New" button
    void New_Render(object sender, EventArgs e)
    {
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucNew.Visible = true;
        this.ucNew.PageCleanup();
    }

    //The event handler when user click button "Back" button of ucNew
    void NewBack_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = false;
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }

    //The event handler when user click button "Save" button of ucNew
    void CreateBack_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(int.Parse(sender.ToString()));
    }

    //The event handler when user click link "Edit" link of ucList
    void ListEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.InitPageParameter(int.Parse(sender.ToString()));
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }
}
