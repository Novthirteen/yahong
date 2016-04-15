using com.Sconit.Service.Ext.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.MRP;

namespace com.Sconit.Service.MasterData
{
    public interface IImportMgr
    {
        IList<ShiftPlanSchedule> ReadPSModelFromXls(Stream inputStream, User user, string regionCode, string flowCode, DateTime date, string shiftCode);

        IList<CycleCountDetail> ReadCycleCountFromXls(Stream inputStream, User user, CycleCount cycleCount);

        IList<FlowPlan> ReadShipScheduleYFKFromXls(Stream inputStream, User user, string planType, string flowCode, string timePeriodType, DateTime date);

        IList<FlowPlan> ReadShipScheduleCSFromXls(Stream inputStream, User user, string planType, string flowCode, bool isRefItem, DateTime date);

        IList<FlowPlan> ReadScheduleFromXls(Stream inputStream, User user, string moduleType, string flowCode, bool isRefItem, DateTime date);

        IList<FlowPlan> ReadShipSchedulePanaFromXls(Stream inputStream, User user, DateTime startDate, DateTime endDate, string flowCode, bool isItemRef);

        CustomerSchedule ReadCustomerScheduleFromXls(Stream inputStream, User user, DateTime? startDate, DateTime? endDate, string flowCode, string refScheduleNo, bool isItemRef);

        CustomerSchedule ReadCustomerSchedulePanaFromXls(Stream inputStream, User user, DateTime startDate, DateTime endDate, string flowCode, string refScheduleNo, bool isItemRef);

        IList<ActingBill> ReadActingBillFromXls(Stream inputStream, string partyCode, User user);

        IList<ActingBill> ReadActingBillFromXls1(Stream inputStream);

        Dictionary<string, decimal> ReadBillFromXls(Stream inputStream, string partyCode, User user);

        IList<FlowPlan> ReadNewPanaOrderFromCSV(Stream inputStream, User user, string flowCode, bool isRefItem, DateTime startDate, DateTime endDate);

        CustomerSchedule ReadNewPanaPlanFromCSV(Stream inputStream, User user, string flowCode, bool isRefItem, DateTime startDate, DateTime endDate, string refScheduleNo, string timePeriodType);
    }
}





#region Extend Interface


namespace com.Sconit.Service.Ext.MasterData
{
    public partial interface IImportMgrE : com.Sconit.Service.MasterData.IImportMgr
    {

    }
}

#endregion
