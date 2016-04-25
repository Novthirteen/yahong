using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class CostBase : EntityBase
    {
        #region O/R Mapping Properties

        private Int32 _id;
        public Int32 Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        private string _taskCode;
        public string TaskCode
        {
            get
            {
                return _taskCode;
            }
            set
            {
                _taskCode = value;
            }
        }
        public string Vehicle { get; set; }
        public Int32 Version { get; set; }
        public decimal? Qty { get; set; }
        public string Item { get; set; }
        public string Uom { get; set; }
        public string TaskSubTypeDesc { get; set; }
        private string _taskSubType;
        public string TaskSubType
        {
            get
            {
                return _taskSubType;
            }
            set
            {
                _taskSubType = value;
            }
        }
        private string _extNo;
        public string ExtNo
        {
            get
            {
                return _extNo;
            }
            set
            {
                _extNo = value;
            }
        }
        private Int32? _budgetDetId;
        public Int32? BudgetDetId
        {
            get
            {
                return _budgetDetId;
            }
            set
            {
                _budgetDetId = value;
            }
        }
        private string _account1;
        public string Account1
        {
            get
            {
                return _account1;
            }
            set
            {
                _account1 = value;
            }
        }
        private string _account1Desc;
        public string Account1Desc
        {
            get
            {
                return _account1Desc;
            }
            set
            {
                _account1Desc = value;
            }
        }
        private string _account2;
        public string Account2
        {
            get
            {
                return _account2;
            }
            set
            {
                _account2 = value;
            }
        }
        private string _account2Desc;
        public string Account2Desc
        {
            get
            {
                return _account2Desc;
            }
            set
            {
                _account2Desc = value;
            }
        }
        private string _desc1;
        public string Desc1
        {
            get
            {
                return _desc1;
            }
            set
            {
                _desc1 = value;
            }
        }
        private Decimal? _noTaxAmount;
        public Decimal? NoTaxAmount
        {
            get
            {
                return _noTaxAmount;
            }
            set
            {
                _noTaxAmount = value;
            }
        }
        private Decimal? _taxes;
        public Decimal? Taxes
        {
            get
            {
                return _taxes;
            }
            set
            {
                _taxes = value;
            }
        }
        private Decimal? _totalAmount;
        public Decimal? TotalAmount
        {
            get
            {
                return _totalAmount;
            }
            set
            {
                _totalAmount = value;
            }
        }
        private string _userCode;
        public string UserCode
        {
            get
            {
                return _userCode;
            }
            set
            {
                _userCode = value;
            }
        }
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        private string _startAddr;
        public string StartAddr
        {
            get
            {
                return _startAddr;
            }
            set
            {
                _startAddr = value;
            }
        }
        private string _endAddr;
        public string EndAddr
        {
            get
            {
                return _endAddr;
            }
            set
            {
                _endAddr = value;
            }
        }
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }
        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }
        private Decimal? _allowance;
        public Decimal? Allowance
        {
            get
            {
                return _allowance;
            }
            set
            {
                _allowance = value;
            }
        }
        private Decimal? _fare;
        public Decimal? Fare
        {
            get
            {
                return _fare;
            }
            set
            {
                _fare = value;
            }
        }
        private Decimal? _quarterage;
        public Decimal? Quarterage
        {
            get
            {
                return _quarterage;
            }
            set
            {
                _quarterage = value;
            }
        }
        private Decimal? _haulage;
        public Decimal? Haulage
        {
            get
            {
                return _haulage;
            }
            set
            {
                _haulage = value;
            }
        }
        private string _lastModifyUser;
        public string LastModifyUser
        {
            get
            {
                return _lastModifyUser;
            }
            set
            {
                _lastModifyUser = value;
            }
        }
        private string _lastModifyUserNm;
        public string LastModifyUserNm
        {
            get
            {
                return _lastModifyUserNm;
            }
            set
            {
                _lastModifyUserNm = value;
            }
        }
        private DateTime _lastModifyDate;
        public DateTime LastModifyDate
        {
            get
            {
                return _lastModifyDate;
            }
            set
            {
                _lastModifyDate = value;
            }
        }

        #endregion

        public override int GetHashCode()
        {
            if (Id != null)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            CostBase another = obj as CostBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }
    }

}
