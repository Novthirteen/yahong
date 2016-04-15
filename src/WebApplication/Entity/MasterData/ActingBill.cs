using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class ActingBill : ActingBillBase
    {
        #region Non O/R Mapping Properties

        private decimal _currentBillQty;
        public decimal CurrentBillQty
        {
            get
            {
                return this._currentBillQty;
            }
            set
            {
                this._currentBillQty = value;
            }
        }

        private decimal _currentBillAmount;
        public decimal CurrentBillAmount
        {
            get
            {
                return this._currentBillAmount;
            }
            set
            {
                this._currentBillAmount = value;
            }
        }


        private decimal _currentDiscount;
        public decimal CurrentDiscount
        {
            get
            {
                return this._currentDiscount;
            }
            set
            {
                this._currentDiscount = value;
            }
        }

        private decimal _currentHeadDiscount;
        public decimal CurrentHeadDiscount
        {
            get
            {
                return this._currentHeadDiscount;
            }
            set
            {
                this._currentHeadDiscount = value;
            }
        }

        #endregion


        public string ErrorMessage { get; set; }

        public string WarningMessage { get; set; }

        public decimal Qty
        {
            get
            {
                return this.BillQty - this.BilledQty;
            }
        }

        public string ItemCode
        {
            get
            {
                if (this.Item == null)
                {
                    return null;
                }
                return this.Item.Code;
            }
        }
        public string ItemDescription
        {
            get
            {
                if (this.Item == null)
                {
                    return null;
                }
                return this.Item.Description;
            }
        }

        public string RowIndex { get; set; }
    }
}