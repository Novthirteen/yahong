using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for CustomizeListModuleBase
/// </summary>
namespace com.Sconit.Web
{
    public abstract class ReportListModuleBase : ModuleBase
    {
        public ReportListModuleBase()
        {
        }

        public abstract IList GetDataSource(int pageSize, int pageIndex);
        public abstract int GetDataCount();
        public abstract void UpdateView();

        //todo
    }
}

