using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class TaskDetailBase : EntityBase
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
        private string _subject;
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }
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
        private string _desc2;
        public string Desc2
        {
            get
            {
                return _desc2;
            }
            set
            {
                _desc2 = value;
            }
        }
        private string _failureMode;
        public string FailureMode
        {
            get
            {
                return _failureMode;
            }
            set
            {
                _failureMode = value;
            }
        }
        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        private string _userEmail;
        public string UserEmail
        {
            get
            {
                return _userEmail;
            }
            set
            {
                _userEmail = value;
            }
        }
        private string _userMobilePhone;
        public string UserMobilePhone
        {
            get
            {
                return _userMobilePhone;
            }
            set
            {
                _userMobilePhone = value;
            }
        }
        private string _flag;
        public string Flag
        {
            get
            {
                return _flag;
            }
            set
            {
                _flag = value;
            }
        }
        private string _color;
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        private DateTime? _planStartDate;
        public DateTime? PlanStartDate
        {
            get
            {
                return _planStartDate;
            }
            set
            {
                _planStartDate = value;
            }
        }
        private DateTime? _planCompleteDate;
        public DateTime? PlanCompleteDate
        {
            get
            {
                return _planCompleteDate;
            }
            set
            {
                _planCompleteDate = value;
            }
        }
        private string _expectedResults;
        public string ExpectedResults
        {
            get
            {
                return _expectedResults;
            }
            set
            {
                _expectedResults = value;
            }
        }
        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        private string _level;
        public string Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }
        private string _priority;
        public string Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }
        private string _backYards;
        public string BackYards
        {
            get
            {
                return _backYards;
            }
            set
            {
                _backYards = value;
            }
        }
        private Boolean _isSMS;
        public Boolean IsSMS
        {
            get
            {
                return _isSMS;
            }
            set
            {
                _isSMS = value;
            }
        }
        private Boolean _isEmail;
        public Boolean IsEmail
        {
            get
            {
                return _isEmail;
            }
            set
            {
                _isEmail = value;
            }
        }
        private string _receiver;
        public string Receiver
        {
            get
            {
                return _receiver;
            }
            set
            {
                _receiver = value;
            }
        }
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }
        private Int32 _emailCount;
        public Int32 EmailCount
        {
            get
            {
                return _emailCount;
            }
            set
            {
                _emailCount = value;
            }
        }
        private string _emailStatus;
        public string EmailStatus
        {
            get
            {
                return _emailStatus;
            }
            set
            {
                _emailStatus = value;
            }
        }
        private string _mobilePhone;
        public string MobilePhone
        {
            get
            {
                return _mobilePhone;
            }
            set
            {
                _mobilePhone = value;
            }
        }
        private string _sMSStatus;
        public string SMSStatus
        {
            get
            {
                return _sMSStatus;
            }
            set
            {
                _sMSStatus = value;
            }
        }
        private Int32 _sMSCount;
        public Int32 SMSCount
        {
            get
            {
                return _sMSCount;
            }
            set
            {
                _sMSCount = value;
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
            TaskDetailBase another = obj as TaskDetailBase;

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
