using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public abstract class FacilityAllocatesBase : EntityBase
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
        private string _itemCode;
        public string ItemCode
        {
            get
            {
                return _itemCode;
            }
            set
            {
                _itemCode = value;
            }
        }
        private string _fCID;
        public string FCID
        {
            get
            {
                return _fCID;
            }
            set
            {
                _fCID = value;
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
	

        public string GroupName { get; set; }
        public int MouldCount { get; set; }
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
            FacilityAllocatesBase another = obj as FacilityAllocatesBase;

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
