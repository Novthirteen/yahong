using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{

    public class WFPermission
    {
        /// <summary>
        /// 审批权限
        /// </summary>
        public bool IsApprove { get; set; }

        /// <summary>
        /// 流程控制
        /// </summary>
        public bool IsCtrl { get; set; }

        /// <summary>
        /// 中文描述
        /// </summary>
        public string Desc1 { get; set; }
        /// <summary>
        /// 英文描述
        /// </summary>
        public string Desc2 { get; set; }
        public bool HasPermissoin
        {
            get { return IsApprove || IsCtrl; }
        }
    }

    [Serializable]
    public class PlanDateWS
    {
        public string TaskCode { get; set; }
        public string PlanDate { get; set; }
        public string SrcPlanDate { get; set; }

        public string RefPlanDate { get; set; }


        public string TbControl { get; set; }
        public string LblControl { get; set; }
    }


    [Serializable]
    public class TaskWS
    {
        public int id { get; set; }
        public string name { get; set; }
        public serie[] series { get; set; }
    }

    [Serializable]
    public class serie
    {
        public string name { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string color { get; set; }
    }

    [Serializable]
    public class Comment
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string TaskCode { get; set; }
        public string Subject { get; set; }

        public string Type { get; set; }

        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int CurrentCount { get; set; }
        public int Count { get; set; }
        public int DisplayIndex { get; set; }
    }

    [Serializable]
    public class Status
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Value { get; set; }
        public string Flag { get; set; }
        public string Color { get; set; }
        public string TaskCode { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int CurrentCount { get; set; }
        public int Count { get; set; }
        public int DisplayIndex { get; set; }
        public bool IsCurrentStatus { get; set; }
        public bool IsComplete { get; set; }
        public string TaskStatus { get; set; }

        public bool IsApprove { get; set; }
        public string LevelDesc { get; set; }

        public string StartedUser { get; set; }

        public string Level { get; set; }
    }

    [Serializable]
    public class CheckupView
    {
        public string CheckupProject { get; set; }
        public double qty1 { get; set; }
        public decimal sum1 { get; set; }
        public double qty2 { get; set; }
        public decimal sum2 { get; set; }
        public string CheckupDate { get; set; }
    }

    [Serializable]
    public class CheckupProjectView
    {
        public string[] categories { get; set; }
        public double p1 { get; set; }
        public double p2 { get; set; }
        public double[] data1 { get; set; }
        public double[] data2 { get; set; }
        public double[] data3 { get; set; }
    }
}