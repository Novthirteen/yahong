using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public abstract class ResSopBase : EntityBase
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        public string WorkShop { get; set; }
        public int Operate { get; set; }
        public string OperateDesc { get; set; }
        public Int32? Instruction { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string LastModifyUser { get; set; }

        #endregion

    }

}
