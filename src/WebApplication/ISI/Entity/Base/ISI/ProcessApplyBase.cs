using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ProcessApplyBase : EntityBase
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
        public string Color { get; set; }
        public bool? MustMatch { get; set; }
        public string ValueField { get; set; }
        public string DescField { get; set; }
        public string ServicePath { get; set; }
        public string ServiceMethod { get; set; }
        public string Align { get; set; }
        public int? FontSize { get; set; }
        public bool? IsRow { get; set; }

        public int? RepeatColumns { get; set; }
        public bool? IsVertical { get; set; }
        public bool? Required { get; set; }
        public string UOMDesc1 { get; set; }
        public string UOMDesc2 { get; set; }
        public string Type { get; set; }
        public string Desc2 { get; set; }
        public string Currency { get; set; }
        public bool? IsUser { get; set; }
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
        private string _uOM;
        public string UOM
        {
            get
            {
                return _uOM;
            }
            set
            {
                _uOM = value;
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
            ProcessApplyBase another = obj as ProcessApplyBase;

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
