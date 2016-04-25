using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class MaintainPlanBase : EntityBase
    {
        #region O/R Mapping Properties
		
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
		private string _description;
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
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
        private Decimal? _period;
        public Decimal? Period
		{
			get
			{
				return _period;
			}
			set
			{
				_period = value;
			}
		}
		private Int32? _leadTime;
		public Int32? LeadTime
		{
			get
			{
				return _leadTime;
			}
			set
			{
				_leadTime = value;
			}
		}
		private Int32? _typePeriod;
		public Int32? TypePeriod
		{
			get
			{
				return _typePeriod;
			}
			set
			{
				_typePeriod = value;
			}
		}
        private string _facilityCategory;
        public string FacilityCategory
        {
            get
            {
                return _facilityCategory;
            }
            set
            {
                _facilityCategory = value;
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
			if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MaintainPlanBase another = obj as MaintainPlanBase;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Code == another.Code);
            }
        } 
    }
	
}
