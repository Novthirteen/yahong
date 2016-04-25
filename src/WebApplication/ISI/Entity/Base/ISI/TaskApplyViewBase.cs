using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class TaskApplyViewBase : EntityBase
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

        public string Subject { get; set; }
        public string TaskDesc { get; set; }

        public string CostCenterCode { get; set; }
        public string CostCenterDesc { get; set; }
        public string TaskSubTypeDesc { get; set; }
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
        private string _uOMDesc1;
        public string UOMDesc1
        {
            get
            {
                return _uOMDesc1;
            }
            set
            {
                _uOMDesc1 = value;
            }
        }
        private string _uOMDesc2;
        public string UOMDesc2
        {
            get
            {
                return _uOMDesc2;
            }
            set
            {
                _uOMDesc2 = value;
            }
        }
        private Boolean? _multiLine;
        public Boolean? MultiLine
        {
            get
            {
                return _multiLine;
            }
            set
            {
                _multiLine = value;
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
        private string _currency;
        public string Currency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
            }
        }
        private Boolean? _isUser;
        public Boolean? IsUser
        {
            get
            {
                return _isUser;
            }
            set
            {
                _isUser = value;
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
        private Boolean? _isVertical;
        public Boolean? IsVertical
        {
            get
            {
                return _isVertical;
            }
            set
            {
                _isVertical = value;
            }
        }
        private Boolean? _checked;
        public Boolean? Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
            }
        }
        private Boolean? _isRow;
        public Boolean? IsRow
        {
            get
            {
                return _isRow;
            }
            set
            {
                _isRow = value;
            }
        }
        private Decimal? _qty;
        public Decimal? Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                _qty = value;
            }
        }
        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        private DateTime? _dateValue;
        public DateTime? DateValue
        {
            get
            {
                return _dateValue;
            }
            set
            {
                _dateValue = value;
            }
        }
        private Boolean? _required;
        public Boolean? Required
        {
            get
            {
                return _required;
            }
            set
            {
                _required = value;
            }
        }
        private string _valueField;
        public string ValueField
        {
            get
            {
                return _valueField;
            }
            set
            {
                _valueField = value;
            }
        }
        private string _descField;
        public string DescField
        {
            get
            {
                return _descField;
            }
            set
            {
                _descField = value;
            }
        }
        private Boolean? _mustMatch;
        public Boolean? MustMatch
        {
            get
            {
                return _mustMatch;
            }
            set
            {
                _mustMatch = value;
            }
        }
        private string _servicePath;
        public string ServicePath
        {
            get
            {
                return _servicePath;
            }
            set
            {
                _servicePath = value;
            }
        }
        private string _serviceMethod;
        public string ServiceMethod
        {
            get
            {
                return _serviceMethod;
            }
            set
            {
                _serviceMethod = value;
            }
        }
        private string _align;
        public string Align
        {
            get
            {
                return _align;
            }
            set
            {
                _align = value;
            }
        }
        private Int32? _fontSize;
        public Int32? FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
            }
        }
        private Int32? _repeatColumns;
        public Int32? RepeatColumns
        {
            get
            {
                return _repeatColumns;
            }
            set
            {
                _repeatColumns = value;
            }
        }
        private string _taskType;
        public string TaskType
        {
            get
            {
                return _taskType;
            }
            set
            {
                _taskType = value;
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
        private DateTime? _createDate;
        public DateTime? CreateDate
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
        private DateTime? _submitDate;
        public DateTime? SubmitDate
        {
            get
            {
                return _submitDate;
            }
            set
            {
                _submitDate = value;
            }
        }
        private string _submitUser;
        public string SubmitUser
        {
            get
            {
                return _submitUser;
            }
            set
            {
                _submitUser = value;
            }
        }
        private string _submitUserNm;
        public string SubmitUserNm
        {
            get
            {
                return _submitUserNm;
            }
            set
            {
                _submitUserNm = value;
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
        private DateTime? _refuseDate;
        public DateTime? RefuseDate
        {
            get
            {
                return _refuseDate;
            }
            set
            {
                _refuseDate = value;
            }
        }
        private string _refuseUser;
        public string RefuseUser
        {
            get
            {
                return _refuseUser;
            }
            set
            {
                _refuseUser = value;
            }
        }
        private string _refuseUserNm;
        public string RefuseUserNm
        {
            get
            {
                return _refuseUserNm;
            }
            set
            {
                _refuseUserNm = value;
            }
        }
        private DateTime? _cancelDate;
        public DateTime? CancelDate
        {
            get
            {
                return _cancelDate;
            }
            set
            {
                _cancelDate = value;
            }
        }
        private string _cancelUser;
        public string CancelUser
        {
            get
            {
                return _cancelUser;
            }
            set
            {
                _cancelUser = value;
            }
        }
        private string _cancelUserNm;
        public string CancelUserNm
        {
            get
            {
                return _cancelUserNm;
            }
            set
            {
                _cancelUserNm = value;
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
            TaskApplyViewBase another = obj as TaskApplyViewBase;

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
