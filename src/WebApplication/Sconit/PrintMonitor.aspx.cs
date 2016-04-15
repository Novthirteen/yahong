using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.MasterData;
using com.Sconit.Utility;
using System.Drawing;
using System.Text;
using NHibernate.Expression;
using com.Sconit.Entity;
using com.Sconit.Service.Criteria;
using System.Collections;
using com.Sconit.Entity.Distribution;
using com.Sconit.Service.Distribution;
using com.Sconit.Service.Report;
using System.IO.Compression;
using System.IO;

public partial class PrintMonitor : com.Sconit.Web.PageBase
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Permission = BusinessConstants.PERMISSION_NOTNEED_CHECK_PERMISSION;
    }

}
