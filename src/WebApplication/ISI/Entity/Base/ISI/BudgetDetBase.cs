using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class BudgetDetBase : EntityBase
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
        private Decimal _noTaxAmount;
        public Decimal NoTaxAmount
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
            BudgetDetBase another = obj as BudgetDetBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id && this.Account1 == another.Account1 && this.Account2 == another.Account2);
            }
        }
    }

}
