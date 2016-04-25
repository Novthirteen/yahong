using com.Sconit.Entity;
using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class MouldDetail : EntityBase
    {
        #region O/R Mapping Properties
        public string Phase { get; set; }
        public string Remark { get; set; }
        
        /// <summary>
        /// Id
        /// </summary>
        public virtual int Id
        {
            get;
            set;
        }
        /// <summary>
        /// Code
        /// </summary>
        public virtual string Code
        {
            get;
            set;
        }
        /// <summary>
        /// Type
        /// </summary>
        public virtual string Type
        {
            get;
            set;
        }
        /// <summary>
        /// PayDate
        /// </summary>
        public virtual DateTime? PayDate
        {
            get;
            set;
        }
        /// <summary>
        /// PayAmount
        /// </summary>
        public virtual Decimal? PayAmount
        {
            get;
            set;
        }
        /// <summary>
        /// BillDate
        /// </summary>
        public virtual DateTime? BillDate
        {
            get;
            set;
        }
        /// <summary>
        /// BillAmount
        /// </summary>
        public virtual Decimal? BillAmount
        {
            get;
            set;
        }
        /// <summary>
        /// Invoice
        /// </summary>
        public virtual string Invoice
        {
            get;
            set;
        }
        
        /// <summary>
        /// CreateUser
        /// </summary>
        public virtual string CreateUser
        {
            get;
            set;
        }
        /// <summary>
        /// CreateUserNm
        /// </summary>
        public virtual string CreateUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// CreateDate
        /// </summary>
        public virtual DateTime CreateDate
        {
            get;
            set;
        }
        /// <summary>
        /// LastModifyUser
        /// </summary>
        public virtual string LastModifyUser
        {
            get;
            set;
        }
        /// <summary>
        /// LastModifyUserNm
        /// </summary>
        public virtual string LastModifyUserNm
        {
            get;
            set;
        }
        /// <summary>
        /// LastModifyDate
        /// </summary>
        public virtual DateTime LastModifyDate
        {
            get;
            set;
        }
        /// <summary>
        /// Version
        /// </summary>
        public virtual int Version
        {
            get;
            set;
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
            MouldDetail another = obj as MouldDetail;

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
