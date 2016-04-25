using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityDistributionBase : EntityBase
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
   
        private string _fcID;
        public string FCID
        {
            get
            {
                return _fcID;
            }
            set
            {
                _fcID = value;
            }
        }
        private string _supplierName;
        public string SupplierName
        {
            get
            {
                return _supplierName;
            }
            set
            {
                _supplierName = value;
            }
        }
        private string _customerName;
        public string CustomerName
        {
            get
            {
                return _customerName;
            }
            set
            {
                _customerName = value;
            }
        }
        private string _purchaseContractCode;
        public string PurchaseContractCode
        {
            get
            {
                return _purchaseContractCode;
            }
            set
            {
                _purchaseContractCode = value;
            }
        }
        private Decimal _purchaseContractAmount;
        public Decimal PurchaseContractAmount
        {
            get
            {
                return _purchaseContractAmount;
            }
            set
            {
                _purchaseContractAmount = value;
            }
        }
        private Decimal _purchaseBilledAmount;
        public Decimal PurchaseBilledAmount
        {
            get
            {
                return _purchaseBilledAmount;
            }
            set
            {
                _purchaseBilledAmount = value;
            }
        }


        private Decimal _purchasePayAmount;
        public Decimal PurchasePayAmount
        {
            get
            {
                return _purchasePayAmount;
            }
            set
            {
                _purchasePayAmount = value;
            }
        }

        private string _distributionContractCode;
        public string DistributionContractCode
        {
            get
            {
                return _distributionContractCode;
            }
            set
            {
                _distributionContractCode = value;
            }
        }
        private Decimal _distributionContractAmount;
        public Decimal DistributionContractAmount
        {
            get
            {
                return _distributionContractAmount;
            }
            set
            {
                _distributionContractAmount = value;
            }
        }
        private Decimal _distributionBilledAmount;
        public Decimal DistributionBilledAmount
        {
            get
            {
                return _distributionBilledAmount;
            }
            set
            {
                _distributionBilledAmount = value;
            }
        }


        private Decimal _distributionPayAmount;
        public Decimal DistributionPayAmount
        {
            get
            {
                return _distributionPayAmount;
            }
            set
            {
                _distributionPayAmount = value;
            }
        }

     
        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
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

        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }
        private string _purchaseContact;
        public string PurchaseContact
        {
            get
            {
                return _purchaseContact;
            }
            set
            {
                _purchaseContact = value;
            }
        }
        private string _distributionContact;
        public string DistributionContact
        {
            get
            {
                return _distributionContact;
            }
            set
            {
                _distributionContact = value;
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
            FacilityDistributionBase another = obj as FacilityDistributionBase;

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
