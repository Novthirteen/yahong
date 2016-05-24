using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class TaskSubTypeBase : EntityBase
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
        public bool IsCtrl { get; set; }
        public string Template { get; set; }
        public bool IsAttachment { get; set; }
        public bool IsPrint { get; set; }
        public bool IsCostCenter { get; set; }
        public string Color { get; set; }
        public string ProcessNo { get; set; }
        public string RegisterNo { get; set; }
        public string ExtNo { get; set; }

        public string ECType { get; set; }
        public bool IsEC { get; set; }

        public bool IsApply { get; set; }

        public bool IsWF { get; set; }

        public bool IsRemoveForm { get; set; }
        public bool IsTrace { get; set; }

        public string ECUser { get; set; }
        public Int32 Version { get; set; }
        public string Desc2 { get; set; }

        private string _desc;
        public string Desc
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
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
        private TaskSubType _parent;
        public TaskSubType Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        private Int32 _seq;
        public Int32 Seq
        {
            get
            {
                return _seq;
            }
            set
            {
                _seq = value;
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
        public Boolean IsPublic { get; set; }
        public Boolean IsReport { get; set; }
        public string ViewUser { get; set; }

        public Boolean IsOpen { get; set; }
        public Decimal? OpenTime { get; set; }

        public Boolean IsAutoStart { get; set; }
        public Boolean IsAutoComplete { get; set; }
        public Boolean IsAutoStatus { get; set; }
        public Boolean IsAutoClose { get; set; }
        private Boolean _isAutoAssign;
        public Boolean IsAutoAssign
        {
            get
            {
                return _isAutoAssign;
            }
            set
            {
                _isAutoAssign = value;
            }
        }
        /// <summary>
        /// ≤ø√≈…Û≈˙
        /// </summary>
        public bool IsAssignUser { get; set; }
        private string _assignUser;
        public string AssignUser
        {
            get
            {
                return _assignUser;
            }
            set
            {
                _assignUser = value;
            }
        }
        private string _startUser;
        public string StartUser
        {
            get
            {
                return _startUser;
            }
            set
            {
                _startUser = value;
            }
        }
        private Boolean _isassignUp;
        public Boolean IsAssignUp
        {
            get
            {
                return _isassignUp;
            }
            set
            {
                _isassignUp = value;
            }
        }
        private Decimal? _assignUpTime;
        public Decimal? AssignUpTime
        {
            get
            {
                return _assignUpTime;
            }
            set
            {
                _assignUpTime = value;
            }
        }
        private string _assignUpUser;
        public string AssignUpUser
        {
            get
            {
                return _assignUpUser;
            }
            set
            {
                _assignUpUser = value;
            }
        }
        private Boolean _isStartUp;
        public Boolean IsStartUp
        {
            get
            {
                return _isStartUp;
            }
            set
            {
                _isStartUp = value;
            }
        }
        private Decimal? _startUpTime;
        public Decimal? StartUpTime
        {
            get
            {
                return _startUpTime;
            }
            set
            {
                _startUpTime = value;
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
        private Boolean _isCloseUp;
        public Boolean IsCloseUp
        {
            get
            {
                return _isCloseUp;
            }
            set
            {
                _isCloseUp = value;
            }
        }
        private Decimal? _closeUpTime;
        public Decimal? CloseUpTime
        {
            get
            {
                return _closeUpTime;
            }
            set
            {
                _closeUpTime = value;
            }
        }
        private string _closeUpUser;
        public string CloseUpUser
        {
            get
            {
                return _closeUpUser;
            }
            set
            {
                _closeUpUser = value;
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

        public bool IsCompleteUp { get; set; }
        public decimal? CompleteUpTime { get; set; }

        public bool IsStart { get; set; }
        public decimal? StartPercent { get; set; }
        public string ProjectType { get; set; }
        public bool IsQuote { get; set; }
        public bool IsInitiation { get; set; }
        public string Org { get; set; }
        #endregion

        private IList<TaskSubType> _children;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public IList<TaskSubType> Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
            }
        }

        private IList<Scheduling> _schedulings;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public IList<Scheduling> Schedulings
        {
            get
            {
                return _schedulings;
            }
            set
            {
                _schedulings = value;
            }
        }


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
            TaskSubTypeBase another = obj as TaskSubTypeBase;

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
