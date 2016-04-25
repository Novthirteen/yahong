using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityStockDetailBase : EntityBase
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
		private string _stNo;
        public string StNo
		{
			get
			{
                return _stNo;
			}
			set
			{
                _stNo = value;
			}
		}
		private FacilityMaster _facilityMaster;
        public FacilityMaster FacilityMaster
		{
			get
			{
                return _facilityMaster;
			}
			set
			{
                _facilityMaster = value;
			}
		}
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
		private Decimal _invQty;
		public Decimal InvQty
		{
			get
			{
				return _invQty;
			}
			set
			{
				_invQty = value;
			}
		}
		private Decimal _diffQty;
		public Decimal DiffQty
		{
			get
			{
				return _diffQty;
			}
			set
			{
				_diffQty = value;
			}
		}
		private string _diffReason;
		public string DiffReason
		{
			get
			{
				return _diffReason;
			}
			set
			{
				_diffReason = value;
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
            FacilityStockDetailBase another = obj as FacilityStockDetailBase;

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
