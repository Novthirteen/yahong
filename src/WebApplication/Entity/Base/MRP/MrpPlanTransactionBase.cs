using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP
{
    [Serializable]
    public abstract class MrpPlanTransactionBase : EntityBase
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
		private string _flow;
		public string Flow
		{
			get
			{
				return _flow;
			}
			set
			{
				_flow = value;
			}
		}
		private string _flowType;
		public string FlowType
		{
			get
			{
				return _flowType;
			}
			set
			{
				_flowType = value;
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
		private string _uom;
		public string Uom
		{
			get
			{
				return _uom;
			}
			set
			{
				_uom = value;
			}
		}
		private Decimal _unitCount;
		public Decimal UnitCount
		{
			get
			{
				return _unitCount;
			}
			set
			{
				_unitCount = value;
			}
		}
		private DateTime _startTime;
		public DateTime StartTime
		{
			get
			{
				return _startTime;
			}
			set
			{
				_startTime = value;
			}
		}
		private DateTime _windowTime;
		public DateTime WindowTime
		{
			get
			{
				return _windowTime;
			}
			set
			{
				_windowTime = value;
			}
		}
		private string _location;
		public string Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}
		private string _sourceType;
		public string SourceType
		{
			get
			{
				return _sourceType;
			}
			set
			{
				_sourceType = value;
			}
		}
		private string _periodType;
		public string PeriodType
		{
			get
			{
				return _periodType;
			}
			set
			{
				_periodType = value;
			}
		}
		private DateTime _effDate;
        public DateTime EffectiveDate
		{
			get
			{
				return _effDate;
			}
			set
			{
				_effDate = value;
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
		private string _ref;
		public string Reference
		{
			get
			{
				return _ref;
			}
			set
			{
				_ref = value;
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
            MrpPlanTransactionBase another = obj as MrpPlanTransactionBase;

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
