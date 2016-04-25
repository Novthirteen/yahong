using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class Task
    {
        public string Code { get; set; }
        public string Status { get; set; }
        public string Level { get; set; }
        public string Subject { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public DateTime CreateDate { get; set; }
        public string StartedUser { get; set; }
        public int DisplayIndex { get; set; }
    }
}