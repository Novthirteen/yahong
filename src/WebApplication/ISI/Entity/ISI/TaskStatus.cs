using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class TaskStatus : TaskStatusBase
    {
        #region Non O/R Mapping Properties

        public bool IsRemindCreateUser { get; set; }
        public bool IsRemindAssignUser { get; set; }
        public bool IsRemindStartUser { get; set; }
        public bool IsRemindCommentUser { get; set; }

        private Boolean _isCurrentStatus;
        public Boolean IsCurrentStatus
        {
            get
            {
                return _isCurrentStatus;
            }
            set
            {
                _isCurrentStatus = value;
            }
        }

        #endregion
    }
}