using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class SummaryBase : EntityBase
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
        public DateTime? InApproveDate { get; set; }
        public string InApproveUser { get; set; }
        public string InApproveUserNm { get; set; }
        public string RefCode { get; set; }
        public DateTime? ApproveDate { get; set; }
        public Int32 Version { get; set; }
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
        protected string _department;
        public string Department
        {
            get
            {
                return _department;
            }
            set
            {
                _department = value;
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
        public string Position { get; set; }
        public string JobNo { get; set; }
        public string Company { get; set; }
        public string Desc { get; set; }
        private string _dept2;
        public string Dept2
        {
            get
            {
                return _dept2;
            }
            set
            {
                _dept2 = value;
            }
        }
        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
        private Int32 _count;
        public Int32 Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }
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
        /// <summary>
        /// 优数
        /// </summary>
        public Int32 Excellent { get; set; }
        /// <summary>
        /// 中数
        /// </summary>
        public Int32 Moderate { get; set; }
        /// <summary>
        /// 差数
        /// </summary>
        public Int32 Poor { get; set; }

        private Int32 _qty;
        public Int32 Qty
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
        public string ApproveDesc { get; set; }

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
        public decimal? UltimatelyAmount { get; set; }

        public DateTime? UltimatelyDate { get; set; }
        public string UltimatelyDesc { get; set; }

        private string _ultimatelyApproveUser;
        public string UltimatelyApproveUser
        {
            get
            {
                return _ultimatelyApproveUser;
            }
            set
            {
                _ultimatelyApproveUser = value;
            }
        }
        private string _ultimatelyApproveUserNm;
        public string UltimatelyApproveUserNm
        {
            get
            {
                return _ultimatelyApproveUserNm;
            }
            set
            {
                _ultimatelyApproveUserNm = value;
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
            SummaryBase another = obj as SummaryBase;

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
