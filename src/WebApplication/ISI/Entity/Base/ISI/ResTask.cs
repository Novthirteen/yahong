using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public  class ResTask : EntityBase
    {
        #region O/R Mapping Properties

        public string TaskCode { get; set; }
        public string TimePeriodType { get; set; }
        public string ResRole { get; set; }
        public string UserCode { get; set; }
        public string TaskSubType { get; set; }
        public int ResId { get; set; }
		public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
            if (TaskCode != null)
            {
                return TaskCode.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ResTask another = obj as ResTask;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.TaskCode == another.TaskCode);
            }
        } 
    }
	
}
