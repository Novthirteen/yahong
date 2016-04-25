using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class TaskMstr : TaskMstrBase
    {
        #region Non O/R Mapping Properties


        public string TotalAmountDesc
        {
            get
            {
                if (TotalAmount.HasValue)
                {
                    return StringHelper.MoneyCn(TotalAmount.Value);
                }
                return string.Empty;
            }
        }
        public string TaxesDesc
        {
            get
            {
                if (this.Taxes.HasValue)
                {
                    return StringHelper.MoneyCn(Taxes.Value);
                }
                return string.Empty;
            }
        }

        public string AmountDesc
        {
            get
            {
                if (Amount.HasValue)
                {
                    return StringHelper.MoneyCn(Amount.Value);
                }
                return string.Empty;
            }
        }
        public string SupplierDesc
        {
            get
            {
                return this.SupplierCode + "[" + this.SupplierName + "]";
            }
        }
        public string CostCenter
        {
            get
            {
                if (!string.IsNullOrEmpty(CostCenterCode))
                {
                    return this.CostCenterCode + "[" + this.CostCenterDesc + "]";
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        public string Account1Name
        {
            get
            {
                if (String.IsNullOrEmpty(this.Account1)) return string.Empty;
                return this.Account1 + "[" + this.Account1Desc + "]";
            }
        }
        public string Account2Name
        {
            get
            {
                if (String.IsNullOrEmpty(this.Account2)) return string.Empty;
                return this.Account2 + "[" + this.Account2Desc + "]";
            }
        }

        public string Payee
        {
            get
            {
                if (String.IsNullOrEmpty(this.PayeeCode)) return string.Empty;
                return this.PayeeCode + "[" + this.PayeeName + "]";
            }
        }

        public bool IsUpdate { get; set; }
        public bool IsCompleteNoRemind { get; set; }
        public bool IsAutoRelease { get; set; }

        public bool IsNoSend { get; set; }

        public string HelpContent { get; set; }

        public CommentDetail CommentDetail { get; set; }

        public TaskStatus TaskStatus { get; set; }

        public String TaskSubTypeCode { get; set; }

        public String TaskSubTypeDesc { get; set; }

        public String TaskSubTypeAssignUser { get; set; }

        public String FailureModeCode { get; set; }

        public String FailureModeDesc { get; set; }

        public decimal? StartPercent { get; set; }

        public string StartedUser
        {
            get
            {
                if (string.IsNullOrEmpty(AssignStartUser))
                {
                    return SchedulingStartUser;
                }
                else
                {
                    return AssignStartUser;
                }
            }
        }

        public IList<TaskApply> TaskApplyList { get; set; }


        #endregion
    }
}