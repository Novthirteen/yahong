using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Facility.Entity
{
    [Serializable]
    public class CheckListMaster
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string Description { get; set; }

        public string TaskSubType { get; set; }
        public string SubUser { get; set; }
        public bool NeekCreateTask { get; set; }

        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public List<CheckListDetail> CheckListDetailList { get; set; }

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
            CheckListMaster another = obj as CheckListMaster;

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
