using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.Cost
{
    [Serializable]
    public abstract class InventoryBalanceBase : EntityBase
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
        private string _item;
        public string Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
            }
        }
        public String Location { get; set; }
        private Decimal _qty;
        public Decimal Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                _qty = value;
            }
        }
        private Int32 _financeYear;
        public Int32 FinanceYear
        {
            get
            {
                return _financeYear;
            }
            set
            {
                _financeYear = value;
            }
        }
        private Int32 _financeMonth;
        public Int32 FinanceMonth
        {
            get
            {
                return _financeMonth;
            }
            set
            {
                _financeMonth = value;
            }
        }
        private DateTime _createDate;
        public DateTime CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
            }
        }
        private string _createUser;
        public string CreateUser
        {
            get
            {
                return _createUser;
            }
            set
            {
                _createUser = value;
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
            InventoryBalanceBase another = obj as InventoryBalanceBase;

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
