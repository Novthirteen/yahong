using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class SummaryDetBase : EntityBase
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
		private string _summaryCode;
		public string SummaryCode
		{
			get
			{
				return _summaryCode;
			}
			set
			{
				_summaryCode = value;
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
		private string _conment;
		public string Conment
		{
			get
			{
				return _conment;
			}
			set
			{
				_conment = value;
			}
		}
        private string _approveDesc;
        public string ApproveDesc
		{
			get
			{
                return _approveDesc;
			}
			set
			{
                _approveDesc = value;
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
            SummaryDetBase another = obj as SummaryDetBase;

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
