using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity.ISI
{
    [Serializable]
    public class CostDet : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
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
        public string TaskSubType { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        private DateTime? _submitDate;
        public DateTime? SubmitDate
        {
            get
            {
                return _submitDate;
            }
            set
            {
                _submitDate = value;
            }
        }
        private string _costCenter;
        public string CostCenter
        {
            get
            {
                return _costCenter;
            }
            set
            {
                _costCenter = value;
            }
        }
        public string Account1Desc { get; set; }
        public string Account2Desc { get; set; }
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
        private Decimal? _amount;
        public Decimal? Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
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
        private Int32? _countY;
        public Int32? CountY
        {
            get
            {
                return _countY;
            }
            set
            {
                _countY = value;
            }
        }
        private string _budgetCode;
        public string BudgetCode
        {
            get
            {
                return _budgetCode;
            }
            set
            {
                _budgetCode = value;
            }
        }
        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        private Decimal? _budgetAmount2;
        public Decimal? BudgetAmount2
        {
            get
            {
                return _budgetAmount2;
            }
            set
            {
                _budgetAmount2 = value;
            }
        }
        private Decimal? _budgetAmount;
        public Decimal? BudgetAmount
        {
            get
            {
                return _budgetAmount;
            }
            set
            {
                _budgetAmount = value;
            }
        }
        private Decimal? _amountY1;
        public Decimal? AmountY1
        {
            get
            {
                return _amountY1;
            }
            set
            {
                _amountY1 = value;
            }
        }
        private Decimal? _amountY3;
        public Decimal? AmountY3
        {
            get
            {
                return _amountY3;
            }
            set
            {
                _amountY3 = value;
            }
        }
        private Decimal? _amountY4;
        public Decimal? AmountY4
        {
            get
            {
                return _amountY4;
            }
            set
            {
                _amountY4 = value;
            }
        }
        private Decimal? _amountY2;
        public Decimal? AmountY2
        {
            get
            {
                return _amountY2;
            }
            set
            {
                _amountY2 = value;
            }
        }
        private Int32? _countM;
        public Int32? CountM
        {
            get
            {
                return _countM;
            }
            set
            {
                _countM = value;
            }
        }
        private Decimal? _budgetAmountMonth;
        public Decimal? BudgetAmountMonth
        {
            get
            {
                return _budgetAmountMonth;
            }
            set
            {
                _budgetAmountMonth = value;
            }
        }
        private Decimal? _amountM1;
        public Decimal? AmountM1
        {
            get
            {
                return _amountM1;
            }
            set
            {
                _amountM1 = value;
            }
        }
        private Decimal? _amountM3;
        public Decimal? AmountM3
        {
            get
            {
                return _amountM3;
            }
            set
            {
                _amountM3 = value;
            }
        }
        private Decimal? _amountM4;
        public Decimal? AmountM4
        {
            get
            {
                return _amountM4;
            }
            set
            {
                _amountM4 = value;
            }
        }

        private Decimal? _amountM2;
        public Decimal? AmountM2
        {
            get
            {
                return _amountM2;
            }
            set
            {
                _amountM2 = value;
            }
        }

        #endregion

    }

}
