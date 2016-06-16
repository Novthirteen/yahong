using System;

/// <summary>
/// Summary description for BusinessConstants
/// </summary>
namespace com.Sconit.Entity
{
    public static class FacilityConstants
    {

        #region Facility
        public static readonly string CODE_PREFIX_FACILITY = "FC";
        public static readonly string CODE_PREFIX_FACILITYSTOCKTAKE = "FS";
        public static readonly string CODE_PREFIX_FACILITYFIXORDER = "FF";

        public static readonly string CODE_MASTER_FACILITY_STATUS = "FacilityStatus";
        public static readonly string CODE_MASTER_FACILITY_STATUS_TEST = "Test";
        public static readonly string CODE_MASTER_FACILITY_STATUS_AVAILABLE = "Available";
        public static readonly string CODE_MASTER_FACILITY_STATUS_INSPECT = "Inspect";
        public static readonly string CODE_MASTER_FACILITY_STATUS_MAINTAIN = "Maintain";
        public static readonly string CODE_MASTER_FACILITY_STATUS_FIX = "Fixing";
        public static readonly string CODE_MASTER_FACILITY_STATUS_LEND = "Lend";
        public static readonly string CODE_MASTER_FACILITY_STATUS_ENVELOP = "Envelop";
        public static readonly string CODE_MASTER_FACILITY_STATUS_SCRAP = "Scrap";
        public static readonly string CODE_MASTER_FACILITY_STATUS_SELL = "Sell";
        public static readonly string CODE_MASTER_FACILITY_STATUS_LOSE = "Lose";
        public static readonly string CODE_MASTER_FACILITY_STATUS_INUSE = "InUse";
        public static readonly string CODE_MASTER_FACILITY_STATUS_BREAKDOWN = "BreakDown";

        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_START = "MaintainStart";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_FINISH = "MaintainFinish";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_APPLY = "Apply";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_RETURN = "Return";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_TRANSFER = "Transfer";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_ENVELOP = "Envelop";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_REOPEN = "Reopen";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_FIX_START = "FixStart";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_FIX_FINISH = "FixFinish";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_INSPECT_START = "InspectStart";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_INSPECT_FINISH = "InspectFinish";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_LEND = "Lend";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_SELL = "Sell";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_LOSE = "Lose";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_SCRAP = "Scrap";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_CREATE = "Create";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_ENABLE = "Enable";
        public static readonly string CODE_MASTER_FACILITY_TRANSTYPE_REPORT = "Report";

        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_ONCE = "Once";
        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_MINUTE = "Minute";
        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_HOUR = "Hour";
        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_DAY = "Day";
        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_WEEK = "Week";
        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_MONTH = "Month";
        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_YEAR = "Year";
        public static readonly string CODE_MASTER_FACILITY_MAINTAIN_TYPE_FREQUENCY = "Frequency";

        public static readonly string CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_PROCUREMENT = "PO";
        public static readonly string CODE_MASTER_FACILITY_DISTRIBUTION_TYPE_DISTRIBUTION = "SO";

        public static readonly string CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CREATE = "Create";
        public static readonly string CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_INPROCESS = "In-Process";
        public static readonly string CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_PURCHASECOMPLETE = "PurchaseComplete";
        public static readonly string CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_DISTRIBUTIONCOMPLETE = "DistributionComplete";
        public static readonly string CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE = "Close";

        public static readonly string CODE_MASTER_FROCK_STARUS = "FrockStatus";
        public static readonly string CODE_MASTER_FROCK_STARUS_AVALIABLE = "Available";
        public static readonly string CODE_MASTER_FROCK_STARUS_SCRAP= "Scrap";

        public static readonly string PERMISSION_PAGE_VALUE_FACILITYTRANSWARN = "FacilityTransWarnRep";
        public static readonly string PERMISSION_PAGE_VALUE_FACILITYMAINTAINWARN = "FacilityMaintainWarnRep";
        public static readonly string PERMISSION_PAGE_VALUE_FACILITYDOWNTIMEWARN = "FacilityDownTimeWarnRep";
        public static readonly string PERMISSION_PAGE_VALUE_FACILITYITEMWARNREP = "FacilityItemWarnRep";

        public static readonly string ENTITYPREFERENCE_DOWNTIME_WARNTIME = "DowntimeWarnTime";


        public static readonly string CODE_MASTER_FIX_ORDER_STATUS = "FixOrderStatus";
        public static readonly string CODE_MASTER_FIX_ORDER_CREATE = "Create";
        public static readonly string CODE_MASTER_FIX_ORDER_SUBMIT = "Submit";
        public static readonly string CODE_MASTER_FIX_ORDER_INPROCESS = "InProcess";
        public static readonly string CODE_MASTER_FIX_ORDER_COMPLETE = "Complete";
        public static readonly string CODE_MASTER_FIX_ORDER_CLOSE = "Close";

        #endregion
    }
}

