using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityCategoryBase : EntityBase
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
        private string _chargeOrganization;
        public string ChargeOrganization
        {
            get
            {
                return _chargeOrganization;
            }
            set
            {
                _chargeOrganization = value;
            }
        }
        private string _chargeSite;
        public string ChargeSite
        {
            get
            {
                return _chargeSite;
            }
            set
            {
                _chargeSite = value;
            }
        }
        private string _chargePerson;
        public string ChargePerson
        {
            get
            {
                return _chargePerson;
            }
            set
            {
                _chargePerson = value;
            }
        }
        private string _parentCategory;
        public string ParentCategory
        {
            get
            {
                return _parentCategory;
            }
            set
            {
                _parentCategory = value;
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

        private string _chargePersonName;
        public string ChargePersonName
        {
            get
            {
                return _chargePersonName;
            }
            set
            {
                _chargePersonName = value;
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
            FacilityCategoryBase another = obj as FacilityCategoryBase;

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
