using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public class CheckListDetail 
    {
        public Int32 Id { get; set; }
        public String Code { get; set; }
        public String CheckListCode { get; set; }
        public String Description { get; set; }
        public int Seq { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public bool IsRequired { get; set; }

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
            CheckListDetail another = obj as CheckListDetail;

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
