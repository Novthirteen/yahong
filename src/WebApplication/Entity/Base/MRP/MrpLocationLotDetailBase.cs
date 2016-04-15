using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP
{
    [Serializable]
    public abstract class MrpLocationLotDetailBase : EntityBase
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
		private Decimal _safeQty;
		public Decimal SafeQty
		{
			get
			{
				return _safeQty;
			}
			set
			{
				_safeQty = value;
			}
		}
		private Decimal _qty;
        public Decimal StaticQty
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
            MrpLocationLotDetailBase another = obj as MrpLocationLotDetailBase;

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
