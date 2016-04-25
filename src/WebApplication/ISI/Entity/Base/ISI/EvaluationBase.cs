using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class EvaluationBase : EntityBase
    {
        #region O/R Mapping Properties

        public string UserName { get; set; }
        private string _userCode;
        public string UserCode
        {
            get
            {
                return _userCode;
            }
            set
            {
                _userCode = value;
            }
        }
        public Int32 Version { get; set; }
        private Int32 _standardQty;
        public Int32 StandardQty
        {
            get
            {
                return _standardQty;
            }
            set
            {
                _standardQty = value;
            }
        }
        private Decimal _amount;
        public Decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
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
        private Boolean _isCheckup;
        public Boolean IsCheckup
        {
            get
            {
                return _isCheckup;
            }
            set
            {
                _isCheckup = value;
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
        private string _createUserNm;
        public string CreateUserNm
        {
            get
            {
                return _createUserNm;
            }
            set
            {
                _createUserNm = value;
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
        private string _lastModifyUserNm;
        public string LastModifyUserNm
        {
            get
            {
                return _lastModifyUserNm;
            }
            set
            {
                _lastModifyUserNm = value;
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
            if (UserCode != null)
            {
                return UserCode.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            EvaluationBase another = obj as EvaluationBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.UserCode == another.UserCode);
            }
        }
    }

}
