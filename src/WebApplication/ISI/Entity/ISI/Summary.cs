using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class Summary : SummaryBase
    {
        #region Non O/R Mapping Properties

        //TODO: Add Non O/R Mapping Properties here. 
        public bool IsAutoRelease { get; set; }

        public int Diff
        {
            get
            {
                return this.Qty - this.StandardQty;
            }
        }

        public decimal CheckupAmount
        {
            get
            {
                return this.Diff * this.Amount;
            }
        }
        public string LongCodeName
        {
            get
            {
                return ((Code != null ? Code : string.Empty) + (LongName != null ? "[" + LongName + "]" : string.Empty));
            }

        }

        public string LongName
        {
            get
            {
                return ((!string.IsNullOrEmpty(UserName) ? this.UserName : string.Empty)
                        + (!string.IsNullOrEmpty(Dept2) ? " " + this.Dept2 : string.Empty)
                        + (!string.IsNullOrEmpty(Company) ? " " + this.Company : string.Empty));
            }
        }

        public string User
        {
            get
            {
                return this.UserCode + "[" + this.UserName + "]";
            }
        }
        #endregion
    }
}