using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class TaskMstrBase : EntityBase
    {
        #region O/R Mapping Properties
        public string FirstUser { get; set; }
        public string FirstUserNm { get; set; }
        public string CurrentApprovalUser { get; set; }
        public string CurrentApprovalUserNm { get; set; }
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
        public string Dept { get; set; }
        public decimal? Amount { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Template { get; set; }
        public decimal? Qty { get; set; }
        public string ApprovalUser { get; set; }
        public string ApprovalUserNm { get; set; }
        public string ApprovalLevel { get; set; }
        public bool? IsCountersignSerial { get; set; }
        public string CountersignUser { get; set; }
        public string CountersignUserNm { get; set; }
        //public TaskSubType CostCenter { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterDesc { get; set; }

        public bool IsApply { get; set; }

        public bool IsWF { get; set; }
        public bool IsTrace { get; set; }
        public string WorkHoursUser { get; set; }
        public string WorkHoursUserNm { get; set; }

        /// <summary>
        /// ¹Ø×¢ÈË
        /// </summary>
        public string FocusUser { get; set; }
        private string _taskAddress;
        public string TaskAddress
        {
            get
            {
                return _taskAddress;
            }
            set
            {
                _taskAddress = value;
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
        public int? Level { get; set; }
        public int? PreLevel { get; set; }
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
        private TaskSubType _taskSubType;
        public TaskSubType TaskSubType
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
        private FailureMode _failureMode;
        public FailureMode FailureMode
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
        public int? Voucher { get; set; }
        public string PayeeCode { get; set; }
        public string PayeeName { get; set; }
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
        private string _wiki;
        public string Wiki
        {
            get
            {
                return _wiki;
            }
            set
            {
                _wiki = value;
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
        private Int32? _scheduling;
        public Int32? Scheduling
        {
            get
            {
                return _scheduling;
            }
            set
            {
                _scheduling = value;
            }
        }
        private string _schedulingStartUser;
        public string SchedulingStartUser
        {
            get
            {
                return _schedulingStartUser;
            }
            set
            {
                _schedulingStartUser = value;
            }
        }
        private string _schedulingShift;
        public string SchedulingShift
        {
            get
            {
                return _schedulingShift;
            }
            set
            {
                _schedulingShift = value;
            }
        }
        private string _schedulingShiftTime;
        public string SchedulingShiftTime
        {
            get
            {
                return _schedulingShiftTime;
            }
            set
            {
                _schedulingShiftTime = value;
            }
        }
        private string _assignStartUser;
        public string AssignStartUser
        {
            get
            {
                return _assignStartUser;
            }
            set
            {
                _assignStartUser = value;
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
        private string _rejectUser;
        public string RejectUser
        {
            get
            {
                return _rejectUser;
            }
            set
            {
                _rejectUser = value;
            }
        }
        private string _rejectUserNm;
        public string RejectUserNm
        {
            get
            {
                return _rejectUserNm;
            }
            set
            {
                _rejectUserNm = value;
            }
        }
        private DateTime? _rejectDate;
        public DateTime? RejectDate
        {
            get
            {
                return _rejectDate;
            }
            set
            {
                _rejectDate = value;
            }
        }
        public decimal? PlanAmount { get; set; }

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
        public DateTime? InApproveDate { get; set; }
        public string InApproveUser { get; set; }
        public string InApproveUserNm { get; set; }

        public DateTime? ApproveDate { get; set; }
        public string ApproveUser { get; set; }
        public string ApproveUserNm { get; set; }

        public DateTime? ReturnDate { get; set; }
        public string ReturnUser { get; set; }
        public string ReturnUserNm { get; set; }

        public DateTime? RefuseDate { get; set; }
        public string RefuseUser { get; set; }
        public string RefuseUserNm { get; set; }

        public DateTime? InDisputeDate { get; set; }
        public string InDisputeUser { get; set; }
        public string InDisputeUserNm { get; set; }

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
        private string _assignUserNm;
        public string AssignUserNm
        {
            get
            {
                return _assignUserNm;
            }
            set
            {
                _assignUserNm = value;
            }
        }
        private DateTime? _assignDate;
        public DateTime? AssignDate
        {
            get
            {
                return _assignDate;
            }
            set
            {
                _assignDate = value;
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
        private string _startUserNm;
        public string StartUserNm
        {
            get
            {
                return _startUserNm;
            }
            set
            {
                _startUserNm = value;
            }
        }
        private string _completeUser;
        public string CompleteUser
        {
            get
            {
                return _completeUser;
            }
            set
            {
                _completeUser = value;
            }
        }
        private string _completeUserNm;
        public string CompleteUserNm
        {
            get
            {
                return _completeUserNm;
            }
            set
            {
                _completeUserNm = value;
            }
        }
        private DateTime? _completeDate;
        public DateTime? CompleteDate
        {
            get
            {
                return _completeDate;
            }
            set
            {
                _completeDate = value;
            }
        }
        private string _closeUser;
        public string CloseUser
        {
            get
            {
                return _closeUser;
            }
            set
            {
                _closeUser = value;
            }
        }
        private string _closeUserNm;
        public string CloseUserNm
        {
            get
            {
                return _closeUserNm;
            }
            set
            {
                _closeUserNm = value;
            }
        }
        private DateTime? _closeDate;
        public DateTime? CloseDate
        {
            get
            {
                return _closeDate;
            }
            set
            {
                _closeDate = value;
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

        //public string PatrolTime { get; set; }
        //public DateTime? LastSendEmailTime { get; set; }
        public string Seq { get; set; }
        public string Phase { get; set; }
        public Int32 ProjectTask { get; set; }

        public string RefNo { get; set; }
        public string ExtNo { get; set; }
        public string TravelType { get; set; }

        public Int32 Version { get; set; }
        public string AssignStartUserNm { get; set; }
        public string OpenUser { get; set; }
        public string OpenUserNm { get; set; }
        public DateTime? OpenDate { get; set; }

        public string SuspendUser { get; set; }
        public string SuspendUserNm { get; set; }
        public DateTime? SuspendDate { get; set; }

        private string _account1;
        public string Account1
        {
            get
            {
                return _account1;
            }
            set
            {
                _account1 = value;
            }
        }
        private string _account1Desc;
        public string Account1Desc
        {
            get
            {
                return _account1Desc;
            }
            set
            {
                _account1Desc = value;
            }
        }
        private string _account2;
        public string Account2
        {
            get
            {
                return _account2;
            }
            set
            {
                _account2 = value;
            }
        }
        private string _account2Desc;
        public string Account2Desc
        {
            get
            {
                return _account2Desc;
            }
            set
            {
                _account2Desc = value;
            }
        }
        private Decimal? _noTaxAmount;
        public Decimal? NoTaxAmount
        {
            get
            {
                return _noTaxAmount;
            }
            set
            {
                _noTaxAmount = value;
            }
        }
        private Decimal? _taxes;
        public Decimal? Taxes
        {
            get
            {
                return _taxes;
            }
            set
            {
                _taxes = value;
            }
        }
        private Decimal? _totalAmount;
        public Decimal? TotalAmount
        {
            get
            {
                return _totalAmount;
            }
            set
            {
                _totalAmount = value;
            }
        }
        public string FormType { get; set; }
        #endregion

        private IList<TaskDetail> _taskDetails;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public IList<TaskDetail> TaskDetails
        {
            get
            {
                return _taskDetails;
            }
            set
            {
                _taskDetails = value;
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
            TaskMstrBase another = obj as TaskMstrBase;

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
