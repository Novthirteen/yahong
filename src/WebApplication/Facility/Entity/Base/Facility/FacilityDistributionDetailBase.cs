using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityDistributionDetailBase : EntityBase
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

        private FacilityDistribution _facilityDistribution;
        public FacilityDistribution FacilityDistribution
        {
            get
            {
                return _facilityDistribution;
            }
            set
            {
                _facilityDistribution = value;
            }
        }
        private DateTime? _payDate;
        public DateTime? PayDate
        {
            get
            {
                return _payDate;
            }
            set
            {
                _payDate = value;
            }
        }

        private Decimal _payAmount;
        public Decimal PayAmount
        {
            get
            {
                return _payAmount;
            }
            set
            {
                _payAmount = value;
            }
        }
        private DateTime? _billDate;
        public DateTime? BillDate
        {
            get
            {
                return _billDate;
            }
            set
            {
                _billDate = value;
            }
        }

        private Decimal _billAmount;
        public Decimal BillAmount
        {
            get
            {
                return _billAmount;
            }
            set
            {
                _billAmount = value;
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

        private string _contact;
        public string Contact
        {
            get
            {
                return _contact;
            }
            set
            {
                _contact = value;
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

        private string _invoice;
        public string Invoice
        {
            get
            {
                return _invoice;
            }
            set
            {
                _invoice = value;
            }
        }

        public string _batchNo;
        public string BatchNo
        {
            get
            {
                return _batchNo;
            }
            set
            {
                _batchNo = value;
            }
        }
        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
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
            FacilityDistributionDetailBase another = obj as FacilityDistributionDetailBase;

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
