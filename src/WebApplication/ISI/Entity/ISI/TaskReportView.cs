using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class TaskReportView
    {
        public Int32? Id { get; set; }
        public string TaskSubTypeCode { get; set; }
        public string TaskType { get; set; }
        public string TaskSubTypeDesc { get; set; }
        public bool IsActive { get; set; }
    }
}