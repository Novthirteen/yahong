using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public abstract class FacilityMaintainPlansBase : EntityBase
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
        public string MaintainPlanCode { get; set; }
        public string FCID { get; set; }
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }
        private DateTime? _nextMaintainDate;
        public DateTime? NextMaintainDate
        {
            get
            {
                return _nextMaintainDate;
            }
            set
            {
                _nextMaintainDate = value;
            }
        }
        private DateTime? _nextWarnDate;
        public DateTime? NextWarnDate
        {
            get
            {
                return _nextWarnDate;
            }
            set
            {
                _nextWarnDate = value;
            }
        }

        private Decimal _startQty;
        public Decimal StartQty
        {
            get
            {
                return _startQty;
            }
            set
            {
                _startQty = value;
            }
        }
        private Decimal _nextMaintainQty;
        public Decimal NextMaintainQty
        {
            get
            {
                return _nextMaintainQty;
            }
            set
            {
                _nextMaintainQty = value;
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
            FacilityMaintainPlansBase another = obj as FacilityMaintainPlansBase;

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
