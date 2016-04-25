using System;
using System.Collections;
using System.Collections.Generic;

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public class RepairOrder
    {
        public string OrderNo { get; set; }
        public string FCID { get; set; }
        public string AssetNo { get; set; }
        public string FCName { get; set; }
        public string FaultDescription { get; set; }
        public string SubmitDept { get; set; }
        public DateTime SubmitTime { get; set; }
        public string SubmitUser { get; set; }
        public string SubmitUserName { get; set; }
        public DateTime HaltStartTime { get; set; }
        public DateTime? HaltEndTime { get; set; }
        public string RepairUser { get; set; }
        public string RepairUserName { get; set; }
        public string OperateUser { get; set; }
        public string OperateUserName { get; set; }
        public string RepairDescription { get; set; }
        public string HaltReason { get; set; }
        public string Items { get; set; }
        public string RefOrderNo { get; set; }
        public string Suggestion { get; set; }
        public string SuggestionUser { get; set; }
        public string SuggestionUserName { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public DateTime? RepairStartTime { get; set; }
        public DateTime? RepairEndTime { get; set; }
        public DateTime? AcceptanceTime { get; set; }

        public override int GetHashCode()
        {
            if (OrderNo != null)
            {
                return OrderNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            RepairOrder another = obj as RepairOrder;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.OrderNo == another.OrderNo);
            }
        }
    }
}
