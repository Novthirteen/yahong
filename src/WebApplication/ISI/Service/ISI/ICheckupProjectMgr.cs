using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ICheckupProjectMgr : ICheckupProjectBaseMgr
    {
        #region Customized Methods
        IList<CheckupProject> GetAllCheckupProject(string type);
        IList<CheckupProject> GetAllCheckupProject(string checkupUser, DateTime? checkupDate, string type);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ICheckupProjectMgrE : com.Sconit.ISI.Service.ICheckupProjectMgr
    {
    }
}

#endregion Extend Interface