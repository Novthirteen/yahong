using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityItemBase : EntityBase
    {
        #region O/R Mapping Properties
        public bool IsWarn { get; set; }
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
        private Item _item;
        public Item Item
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
        private String _fcid;
        public String FCID
        {
            get
            {
                return _fcid;
            }
            set
            {
                _fcid = value;
            }
        }
        private Boolean _isActive;
        public Boolean IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }
        private Boolean _isAllocate;
        public Boolean IsAllocate
        {
            get
            {
                return _isAllocate;
            }
            set
            {
                _isAllocate = value;
            }
        }
        public decimal InitQty { get; set; }
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
        private Decimal _amount;
        public Decimal Amount
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



        private Decimal _allocatedAmount;
        public Decimal AllocatedAmount
        {
            get
            {
                return _allocatedAmount;
            }
            set
            {
                _allocatedAmount = value;
            }
        }

        private Decimal _allocatedQty;
        public Decimal AllocatedQty
        {
            get
            {
                return _allocatedQty;
            }
            set
            {
                _allocatedQty = value;
            }
        }

        private String _allocateType;
        public String AllocateType
        {
            get
            {
                return _allocateType;
            }
            set
            {
                _allocateType = value;
            }
        }

        private Decimal _warnRate;
        public Decimal WarnRate
        {
            get
            {
                return _warnRate;
            }
            set
            {
                _warnRate = value;
            }
        }

        private Decimal _passRate;
        public Decimal PassRate
        {
            get
            {
                return _passRate;
            }
            set
            {
                _passRate = value;
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

        private Decimal _singleQty;
        public Decimal SingleQty
        {
            get
            {
                return _singleQty;
            }
            set
            {
                _singleQty = value;
            }
        }

        private string _remark;
        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                _remark = value;
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
            FacilityItemBase another = obj as FacilityItemBase;

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
