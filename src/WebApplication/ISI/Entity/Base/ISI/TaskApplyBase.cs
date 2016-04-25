using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class TaskApplyBase : EntityBase
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
        public int? RepeatColumns { get; set; }
        public string Color { get; set; }
        public bool? MustMatch { get; set; }
        public string ValueField { get; set; }
        public string DescField { get; set; }
        public string ServicePath { get; set; }
        public string ServiceMethod { get; set; }
        public string Align { get; set; }
        public int? FontSize { get; set; }
        public bool? Checked { get; set; }
        public bool? IsRow { get; set; }
        public bool? IsVertical { get; set; }
        public bool? Required { get; set; }
        public bool? IsUser { get; set; }
        public string UOM { get; set; }
        public string UOMDesc1 { get; set; }
        public string UOMDesc2 { get; set; }
        public string Type { get; set; }
        public string Desc2 { get; set; }
        public string Currency { get; set; }

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
        public string Value { get; set; }
        public Decimal? Qty { get; set; }

        public DateTime? DateValue { get; set; }
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
            TaskApplyBase another = obj as TaskApplyBase;

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
