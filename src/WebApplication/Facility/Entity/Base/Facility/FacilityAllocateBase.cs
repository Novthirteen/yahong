using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityAllocateBase : EntityBase
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
		private string _allocateType;
		public string AllocateType
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
		private Decimal _warnQty;
		public Decimal WarnQty
		{
			get
			{
				return _warnQty;
			}
			set
			{
				_warnQty = value;
			}
		}
		private Decimal _nextWarnQty;
		public Decimal NextWarnQty
		{
			get
			{
				return _nextWarnQty;
			}
			set
			{
				_nextWarnQty = value;
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
		private string _startUpUser;
		public string StartUpUser
		{
			get
			{
				return _startUpUser;
			}
			set
			{
				_startUpUser = value;
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
            FacilityAllocateBase another = obj as FacilityAllocateBase;

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
