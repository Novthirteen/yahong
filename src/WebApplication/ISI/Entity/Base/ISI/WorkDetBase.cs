using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class WorkDetBase : EntityBase
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
        public decimal Qty { get; set; }
        public string UOM { get; set; }
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
        public string Type { get; set; }
        private Int32 _taskApplyId;
        public Int32 TaskApplyId
        {
            get
            {
                return _taskApplyId;
            }
            set
            {
                _taskApplyId = value;
            }
        }
        private string _apply;
        public string Apply
        {
            get
            {
                return _apply;
            }
            set
            {
                _apply = value;
            }
        }
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
        private string _userNm;
        public string UserNm
        {
            get
            {
                return _userNm;
            }
            set
            {
                _userNm = value;
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
        private string _approveUserNm;
        public string ApproveUserNm
        {
            get
            {
                return _approveUserNm;
            }
            set
            {
                _approveUserNm = value;
            }
        }
        private string _approveUser;
        public string ApproveUser
        {
            get
            {
                return _approveUser;
            }
            set
            {
                _approveUser = value;
            }
        }
        private DateTime? _approveDate;
        public DateTime? ApproveDate
        {
            get
            {
                return _approveDate;
            }
            set
            {
                _approveDate = value;
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
            WorkDetBase another = obj as WorkDetBase;

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
