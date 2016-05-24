using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Util;
using System;
using System.Collections.Generic;
using System.Linq;
//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class TaskMstr : TaskMstrBase
    {
        #region Non O/R Mapping Properties

        public bool IsUpdate { get; set; }
        public bool IsCompleteNoRemind { get; set; }
        public bool IsAutoRelease { get; set; }

        public bool IsNoSend { get; set; }

        public string HelpContent { get; set; }

        public CommentDetail CommentDetail { get; set; }

        public TaskStatus TaskStatus { get; set; }

        public String TaskSubTypeCode { get; set; }

        public String TaskSubTypeDesc { get; set; }

        public String TaskSubTypeAssignUser { get; set; }

        public String FailureModeCode { get; set; }

        public String FailureModeDesc { get; set; }

        public decimal? StartPercent { get; set; }

        public string StartedUser
        {
            get
            {
                if (string.IsNullOrEmpty(AssignStartUser))
                {
                    return SchedulingStartUser;
                }
                else
                {
                    return AssignStartUser;
                }
            }
        }

        public IList<TaskApply> TaskApplyList { get; set; }


        #endregion
    }
}