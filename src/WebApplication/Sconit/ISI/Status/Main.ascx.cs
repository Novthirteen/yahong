using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Status_Main : MainModuleBase
{
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
    public bool IsToDoList
    {
        get
        {
            return ViewState["IsToDoList"] != null ? (bool)ViewState["IsToDoList"] : false;
        }
        set
        {
            ViewState["IsToDoList"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucSearch.NewEvent += new System.EventHandler(this.New_Render);
        this.ucList.EditEvent += new System.EventHandler(this.ListEdit_Render);
        this.ucList.NewEvent += new System.EventHandler(this.ListNew_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucNew.BackEvent += new System.EventHandler(this.NewBack_Render);
        this.ucNew.CreateEvent += new System.EventHandler(this.CreateBack_Render);

        if (!IsPostBack)
        {
            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
            if (this.ModuleParameter.ContainsKey("ModuleType"))
            {
                this.ModuleType = this.ModuleParameter["ModuleType"];
            }
            if (this.ModuleParameter.ContainsKey("IsToDoList"))
            {
                this.IsToDoList = bool.Parse(this.ModuleParameter["IsToDoList"]);
            }
            if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
            {
                this.ucNew.ModuleType = ISIConstants.ISI_TASK_TYPE_PROJECT;
            }
            else if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
            {
                this.ucNew.ModuleType = ISIConstants.ISI_TASK_TYPE_WORKFLOW;
            }
            else
            {
                this.ucNew.ModuleType = ISIConstants.ISI_TASK_TYPE_PLAN;
            }
            this.ucList.IsToDoList = this.IsToDoList;
            this.ucSearch.IsToDoList = this.IsToDoList;
            this.ucList.ModuleType = this.ModuleType;
            this.ucSearch.ModuleType = this.ModuleType;
            this.ucSearch.TaskCode = this.TaskCode;
            if (this.IsToDoList)
            {
                this.ucSearch.Visible = false;
            }
        }
    }

    //The event handler when user click button "Save" button of ucNew
    void CreateBack_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter((string)sender);
    }

    //The event handler when user click button "Back" button of ucNew
    void NewBack_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = false;
        this.ucSearch.Visible = !IsToDoList; ;
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucNew.Visible = false;
        this.ucSearch.Visible = !IsToDoList;
        this.ucList.Visible = true;
        this.ucList.UpdateView(sender.ToString());
    }


    void New_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = true;
        this.ucNew.InitPageParameter();
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.Visible = false;
        this.ucNew.PageCleanup();
    }
    //The event handler when user click link "Edit" link of ucList
    void ListNew_Render(object sender, EventArgs e)
    {
        this.ucNew.BackYards = ((string[])sender)[0];
        this.ucNew.Subject = ((string[])sender)[1];
        this.ucNew.TaskAddress = ((string[])sender)[2];
        this.ucNew.Desc1 = ((string[])sender)[3];
        this.ucNew.Desc2 = ((string[])sender)[4];
        this.ucNew.ExpectedResults = ((string[])sender)[5];
        this.ucNew.PlanStartDate = ((string[])sender)[6];
        this.ucNew.PlanCompleteDate = ((string[])sender)[7];

        this.ucNew.Visible = true;
        this.ucNew.InitPageParameter();
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.Visible = false;
        this.ucNew.PageCleanup();
    }

    //The event handler when user click link "Edit" link of ucList
    void ListEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucNew.Visible = false;
        this.ucEdit.InitPageParameter((string)sender);
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1]);
        this.ucList.Visible = true;
        this.ucList.Type = (string)((object[])sender)[2];
        this.ucList.IsHighlight = (bool)((object[])sender)[3];
        this.ucList.Desc = (string)((object[])sender)[4];
        this.ucList.IsStatus = (bool)((object[])sender)[7];
        if (this.ucList.IsStatus)
        {
            if (!string.IsNullOrEmpty((string)((object[])sender)[8]))
            {
                this.ucList.StartDate = DateTime.Parse((string)((object[])sender)[8]);
            }
            else
            {
                this.ucList.StartDate = null;
            }
            if (!string.IsNullOrEmpty((string)((object[])sender)[9]))
            {
                this.ucList.EndDate = DateTime.Parse((string)((object[])sender)[9]);
            }
            else
            {
                this.ucList.EndDate = null;
            }
        }
        else
        {
            this.ucList.StartDate = null;
            this.ucList.EndDate = null;
        }

        this.ucList.UpdateView();
        this.ucList.SetAnchor();
    }
}
