using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ResMatrixLogBase : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public Int32 ResMatrixId { get; set; }
        public string WorkShop { get; set; }
        public string WorkShopName { get; set; }
        public Int32? Operate { get; set; }
        public string OperateDesc { get; set; }
        public string AttachmentIds { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }
        public string Responsibility { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Priority { get; set; }
        public string SkillLevel { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OldResponsibility { get; set; }


        public string Action { get; set; }
        public string Logs { get; set; }
        public Int32 Seq { get; set; }
        public string TaskSubType { get; set; }
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
            ResMatrixLogBase another = obj as ResMatrixLogBase;

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
