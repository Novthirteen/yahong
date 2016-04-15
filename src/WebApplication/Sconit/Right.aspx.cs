using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.View;

public partial class Right : com.Sconit.Web.PageBase
{
    protected string url = "Main.aspx?mid=Security.UserPreference";
    protected string name = "用户偏好";

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Permission = BusinessConstants.PERMISSION_NOTNEED_CHECK_PERMISSION;
        Favorites favorites = TheFavoritesMgr.LoadLastFavorites(this.CurrentUser.Code, "History");

        if (favorites != null)
        {
            MenuView menuView = TheMenuViewMgr.GetMenuView(favorites.PageName);
            if (menuView != null)
            {
                url = menuView.PageUrl.Substring(2);
                name = TheLanguageMgr.TranslateContent(menuView.Code, this.CurrentUser.UserLanguage);
            }
        }
    }
}
