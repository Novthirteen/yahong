using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class Mould : EntityBase
    {
        #region O/R Mapping Properties
        public int Qty { get; set; }
        public string Type { get; set; }
        public DateTime? SOPayDate1 { get; set; }
        public DateTime? SOPayDate2 { get; set; }
        public DateTime? SOPayDate3 { get; set; }

        public DateTime? SOPayDate4 { get; set; }
        public DateTime? SupplierPayDate1 { get; set; }
        public DateTime? SupplierPayDate2 { get; set; }
        public DateTime? SupplierPayDate3 { get; set; }
        public DateTime? SupplierPayDate4 { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public virtual string Code
        {
            get;
            set;
        }
        /// <summary>
        /// Desc1
        /// </summary>
        public virtual string Desc1
        {
            get;
            set;
        }
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
        /// FCId
        /// </summary>
        public virtual string FCID
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
        /// Status
        /// </summary>
        public virtual string Status
        {
            get;
            set;
        }
        /// <summary>
        /// MouldUser
        /// </summary>
        public virtual string MouldUser
        {
            get;
            set;
        }
        /// <summary>
        /// MouldUserNm
        /// </summary>
        public virtual string MouldUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// SOUser
        /// </summary>
        public virtual string SOUser
        {
            get;
            set;
        }
        /// <summary>
        /// SOUserNm
        /// </summary>
        public virtual string SOUserNm
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
        /// SOAmount4
        /// </summary>
        public virtual Decimal? SOAmount4
        {
            get;
            set;
        }
        /// <summary>
        /// SOBillDate4
        /// </summary>
        public virtual DateTime? SOBillDate4
        {
            get;
            set;
        }
        /// <summary>
        /// SOBilledAmount4
        /// </summary>
        public virtual Decimal? SOBilledAmount4
        {
            get;
            set;
        }
        /// <summary>
        /// SOPayAmount4
        /// </summary>
        public virtual Decimal? SOPayAmount4
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







        /// <summary>
        /// POAmount4
        /// </summary>
        public virtual Decimal? POAmount4
        {
            get;
            set;
        }

        /// <summary>
        /// POBilledAmount4
        /// </summary>
        public virtual Decimal? POBilledAmount4
        {
            get;
            set;
        }
        /// <summary>
        /// POPayAmount4
        /// </summary>
        public virtual Decimal? POPayAmount4
        {
            get;
            set;
        }







        /// <summary>
        /// POUser
        /// </summary>
        public virtual string POUser
        {
            get;
            set;
        }
        /// <summary>
        /// POUserNm
        /// </summary>
        public virtual string POUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// Supplier
        /// </summary>
        public virtual string Supplier
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierName
        /// </summary>
        public virtual string SupplierName
        {
            get;
            set;
        }
        
        /// <summary>
        /// SupplierContractNo
        /// </summary>
        public virtual string SupplierContractNo
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierAmount
        /// </summary>
        public virtual Decimal? SupplierAmount
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBilledAmount
        /// </summary>
        public virtual Decimal? SupplierBilledAmount
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierPayAmount
        /// </summary>
        public virtual Decimal? SupplierPayAmount
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierAmount1
        /// </summary>
        public virtual Decimal? SupplierAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBillDate1
        /// </summary>
        public virtual DateTime? SupplierBillDate1
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBilledAmount1
        /// </summary>
        public virtual Decimal? SupplierBilledAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierPayAmount1
        /// </summary>
        public virtual Decimal? SupplierPayAmount1
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierAmount2
        /// </summary>
        public virtual Decimal? SupplierAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBillDate2
        /// </summary>
        public virtual DateTime? SupplierBillDate2
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBilledAmount2
        /// </summary>
        public virtual Decimal? SupplierBilledAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierPayAmount2
        /// </summary>
        public virtual Decimal? SupplierPayAmount2
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierAmount3
        /// </summary>
        public virtual Decimal? SupplierAmount3
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBillDate3
        /// </summary>
        public virtual DateTime? SupplierBillDate3
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBilledAmount3
        /// </summary>
        public virtual Decimal? SupplierBilledAmount3
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierPayAmount3
        /// </summary>
        public virtual Decimal? SupplierPayAmount3
        {
            get;
            set;
        }


        /// <summary>
        /// SupplierAmount4
        /// </summary>
        public virtual Decimal? SupplierAmount4
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBillDate4
        /// </summary>
        public virtual DateTime? SupplierBillDate4
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierBilledAmount4
        /// </summary>
        public virtual Decimal? SupplierBilledAmount4
        {
            get;
            set;
        }
        /// <summary>
        /// SupplierPayAmount4
        /// </summary>
        public virtual Decimal? SupplierPayAmount4
        {
            get;
            set;
        }



        /// <summary>
        /// Remark
        /// </summary>
        public virtual string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// CreateUser
        /// </summary>
        public virtual string CreateUser
        {
            get;
            set;
        }
        /// <summary>
        /// CreateUserNm
        /// </summary>
        public virtual string CreateUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// CreateDate
        /// </summary>
        public virtual DateTime CreateDate
        {
            get;
            set;
        }
        /// <summary>
        /// SubmitUser
        /// </summary>
        public virtual string SubmitUser
        {
            get;
            set;
        }
        /// <summary>
        /// SubmitUserNm
        /// </summary>
        public virtual string SubmitUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// SubmitDate
        /// </summary>
        public virtual DateTime? SubmitDate
        {
            get;
            set;
        }
        /// <summary>
        /// CloseUser
        /// </summary>
        public virtual string CloseUser
        {
            get;
            set;
        }
        /// <summary>
        /// CloseUserNm
        /// </summary>
        public virtual string CloseUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// CloseDate
        /// </summary>
        public virtual DateTime? CloseDate
        {
            get;
            set;
        }
        /// <summary>
        /// LastModifyUser
        /// </summary>
        public virtual string LastModifyUser
        {
            get;
            set;
        }
        /// <summary>
        /// LastModifyUserNm
        /// </summary>
        public virtual string LastModifyUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// LastModifyDate
        /// </summary>
        public virtual DateTime LastModifyDate
        {
            get;
            set;
        }
        /// <summary>
        /// Version
        /// </summary>
        public virtual int Version
        {
            get;
            set;
        }

        public DateTime? SOCompleteDate { get; set; }
        public string SOCompleteUser { get; set; }
        public string SOCompleteUserNm { get; set; }
        public DateTime? POCompleteDate { get; set; }
        public string POCompleteUser { get; set; }
        public string POCompleteUserNm { get; set; }
        #endregion
        public string Project
        {
            get
            {
                if (string.IsNullOrEmpty(this.PrjCode)) return string.Empty;
                return this.PrjDesc + "[" + this.PrjCode + "]";
            }
        }
        public string SOUserDesc
        {
            get
            {
                if (string.IsNullOrEmpty(this.SOUser)) return string.Empty;
                return this.SOUserNm + "[" + this.SOUser + "]";
            }
        }
        public string MouldUserDesc
        {
            get
            {
                if (string.IsNullOrEmpty(this.MouldUser)) return string.Empty;
                return this.MouldUserNm + "[" + this.MouldUser + "]";
            }
        }
        public string POUserDesc
        {
            get
            {
                if (string.IsNullOrEmpty(this.POUser)) return string.Empty;
                return this.POUserNm + "[" + this.POUser + "]";
            }
        }

        public string SupplierDesc
        {
            get
            {
                if (string.IsNullOrEmpty(this.Supplier)) return string.Empty;
                return this.SupplierName + "[" + this.Supplier + "]";
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
            if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            Mould another = obj as Mould;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code);
            }
        }

    }

}
