using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ProcessInstanceBase : EntityBase
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
        public bool IsAccountCtrl { get; set; }
        public bool IsRemind { get; set; }
        public string Apply { get; set; }
        public string ApplyDesc { get; set; }
        public decimal? ApplyQty { get; set; }
        public string UOM { get; set; }
        public string UOMDesc { get; set; }
        public decimal? Qty { get; set; }

       
        public string Status { get; set; }
        
        private string _taskCode;
        public string TaskCode
        {
            get
            {
                return _taskCode;
            }
            set
            {
                _taskCode = value;
            }
        }
        private string _taskSubType;
        public string TaskSubType
        {
            get
            {
                return _taskSubType;
            }
            set
            {
                _taskSubType = value;
            }
        }
        public Int32 Level { get; set; }

        private string _desc1;
        public string Desc1
        {
            get
            {
                return _desc1;
            }
            set
            {
                _desc1 = value;
            }
        }
        public string UserCode { get; set; }
        public string UserNm { get; set; }
        public Boolean ATicket { get; set; }
        private Boolean _isParallel;
        public Boolean IsParallel
        {
            get
            {
                return _isParallel;
            }
            set
            {
                _isParallel = value;
            }
        }
        private Boolean _isOpt;
        public Boolean IsOpt
        {
            get
            {
                return _isOpt;
            }
            set
            {
                _isOpt = value;
            }
        }
        private Boolean _isApprove;
        public Boolean IsApprove
        {
            get
            {
                return _isApprove;
            }
            set
            {
                _isApprove = value;
            }
        }
        private Boolean _isCtrl;
        public Boolean IsCtrl
        {
            get
            {
                return _isCtrl;
            }
            set
            {
                _isCtrl = value;
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
        public DateTime? ProcessDate { get; set; }
        public string ProcessUser { get; set; }
        public string ProcessUserNm { get; set; }

        private Int32 _version;
        public Int32 Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
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
            ProcessInstanceBase another = obj as ProcessInstanceBase;

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
