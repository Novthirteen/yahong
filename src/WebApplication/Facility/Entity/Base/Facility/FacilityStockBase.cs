using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public abstract class FacilityStockBase : EntityBase
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
        private string _stockNo;
        public string StockNo
        {
            get
            {
                return _stockNo;
            }
            set
            {
                _stockNo = value;
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

        #endregion

        public override int GetHashCode()
        {
            if (Id != 0)
            {
                return FCID.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FacilityStockBase another = obj as FacilityStockBase;

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
