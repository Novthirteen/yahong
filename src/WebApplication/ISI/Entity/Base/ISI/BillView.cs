using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class BillView : EntityBase
    {
        #region O/R Mapping Properties

        public string Type { get; set; }
        public DateTime? SOPayDate1 { get; set; }
        public DateTime? SOPayDate2 { get; set; }
        public DateTime? SOPayDate3 { get; set; }


        /// <summary>
        /// PrjCode
        /// </summary>
        public virtual string PrjCode
        {
            get;
            set;
        }
        /// <summary>
        /// PrjDesc
        /// </summary>
        public virtual string PrjDesc
        {
            get;
            set;
        }

        /// <summary>
        /// QS
        /// </summary>
        public virtual Decimal? QS
        {
            get;
            set;
        }


        /// <summary>
        /// Customer
        /// </summary>
        public virtual string Customer
        {
            get;
            set;
        }
        /// <summary>
        /// CustomerName
        /// </summary>
        public virtual string CustomerName
        {
            get;
            set;
        }

        /// <summary>
        /// SOContractNo
        /// </summary>
        public virtual string SOContractNo
        {
            get;
            set;
        }
        /// <summary>
        /// SOAmount
        /// </summary>
        public virtual Decimal? SOAmount
        {
            get;
            set;
        }
        /// <summary>
        /// SOBilledAmount
        /// </summary>
        public virtual Decimal? SOBilledAmount
        {
            get;
            set;
        }
        /// <summary>
        /// SOPayAmount
        /// </summary>
        public virtual Decimal? SOPayAmount
        {
            get;
            set;
        }
        /// <summary>
        /// SOAmount1
        /// </summary>
        public virtual Decimal? SOAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// SOBillDate1
        /// </summary>
        public virtual DateTime? SOBillDate1
        {
            get;
            set;
        }
        /// <summary>
        /// SOBilledAmount1
        /// </summary>
        public virtual Decimal? SOBilledAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// SOPayAmount1
        /// </summary>
        public virtual Decimal? SOPayAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// SOAmount2
        /// </summary>
        public virtual Decimal? SOAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// SOBillDate2
        /// </summary>
        public virtual DateTime? SOBillDate2
        {
            get;
            set;
        }
        /// <summary>
        /// SOBilledAmount2
        /// </summary>
        public virtual Decimal? SOBilledAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// SOPayAmount2
        /// </summary>
        public virtual Decimal? SOPayAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// SOAmount3
        /// </summary>
        public virtual Decimal? SOAmount3
        {
            get;
            set;
        }
        /// <summary>
        /// SOBillDate3
        /// </summary>
        public virtual DateTime? SOBillDate3
        {
            get;
            set;
        }
        /// <summary>
        /// SOBilledAmount3
        /// </summary>
        public virtual Decimal? SOBilledAmount3
        {
            get;
            set;
        }
        /// <summary>
        /// SOPayAmount3
        /// </summary>
        public virtual Decimal? SOPayAmount3
        {
            get;
            set;
        }
        public string SOUser { get; set; }
        public string SOUserNm { get; set; }

        /// <summary>
        /// POAmount
        /// </summary>
        public virtual Decimal? POAmount
        {
            get;
            set;
        }
        /// <summary>
        /// POBilledAmount
        /// </summary>
        public virtual Decimal? POBilledAmount
        {
            get;
            set;
        }
        /// <summary>
        /// POPayAmount
        /// </summary>
        public virtual Decimal? POPayAmount
        {
            get;
            set;
        }
        /// <summary>
        /// POAmount1
        /// </summary>
        public virtual Decimal? POAmount1
        {
            get;
            set;
        }

        /// <summary>
        /// POBilledAmount1
        /// </summary>
        public virtual Decimal? POBilledAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// POPayAmount1
        /// </summary>
        public virtual Decimal? POPayAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// POAmount2
        /// </summary>
        public virtual Decimal? POAmount2
        {
            get;
            set;
        }

        /// <summary>
        /// POBilledAmount2
        /// </summary>
        public virtual Decimal? POBilledAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// POPayAmount2
        /// </summary>
        public virtual Decimal? POPayAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// POAmount3
        /// </summary>
        public virtual Decimal? POAmount3
        {
            get;
            set;
        }

        /// <summary>
        /// POBilledAmount3
        /// </summary>
        public virtual Decimal? POBilledAmount3
        {
            get;
            set;
        }
        /// <summary>
        /// POPayAmount3
        /// </summary>
        public virtual Decimal? POPayAmount3
        {
            get;
            set;
        }

        #endregion
        public string Project
        {
            get
            {
                if (string.IsNullOrEmpty(this.PrjCode)) return string.Empty;
                return this.PrjDesc + "[" + this.PrjCode + "]";
            }
        }
        public string SoUserDesc
        {
            get
            {
                if (string.IsNullOrEmpty(this.SOUser)) return string.Empty;
                return this.SOUserNm + "[" + this.SOUser + "]";
            }
        }
        public string CustomerDesc
        {
            get
            {
                if (string.IsNullOrEmpty(this.Customer)) return string.Empty;
                return this.CustomerName + "[" + this.Customer + "]";
            }
        }
        public override int GetHashCode()
        {
            if (PrjCode != null)
            {
                return PrjCode.GetHashCode() + Type.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            BillView another = obj as BillView;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.PrjCode == another.PrjCode && this.Type == another.Type);
            }
        }

    }

}
