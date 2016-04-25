using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ISchedulingMgr : ISchedulingBaseMgr
    {
        #region Customized Methods

        //List<SchedulingView> GetScheduling(DateTime startdate, DateTime enddate, string taskSubType);

        //bool Exists(string taskSubType, string shift, bool isSpecial);

        void Check();

        void SetStartUser(IList<SchedulingView> schedulingViews);

        IList<SchedulingView> GetScheduling2(DateTime date, string taskSubTypeCode);

        IList<SchedulingView> GetScheduling2(DateTime startDate, DateTime endDate, string taskSubTypeCode, string userCode);

        bool HasSpecialScheduling2(string taskSubType, DateTime startDate, DateTime endDate);

        bool HasSpecialScheduling2(string taskSubType, DateTime startDate, DateTime endDate, Int32? schedulingId);

        IList<Shift> GetShift(string dayOfWeek);

        bool Exists(string dayOfWeek, string shift, string taskSubType, bool isSpecial);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ISchedulingMgrE : com.Sconit.ISI.Service.ISchedulingMgr
    {
    }
}

#endregion Extend Interface