using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskWeekStatisticsViewMgr : ITaskWeekStatisticsViewBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskWeekStatisticsViewMgrE : com.Sconit.ISI.Service.ITaskWeekStatisticsViewMgr
    {
    }
}

#endregion Extend Interface