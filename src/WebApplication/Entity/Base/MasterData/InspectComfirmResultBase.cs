using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public abstract class InspectComfirmResultBase : EntityBase
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
        private string _inspComfirmResultNo;
        public string InspComfirmResultNo
        {
            get
            {
                return _inspComfirmResultNo;
            }
            set
            {
                _inspComfirmResultNo = value;
            }
        }
        private Int32? _inspResultId;
        public Int32? InspResultId
        {
            get
            {
                return _inspResultId;
            }
            set
            {
                _inspResultId = value;
            }
        }
        private Int32 _inspDetId;
        public Int32 InspDetId
        {
            get
            {
                return _inspDetId;
            }
            set
            {
                _inspDetId = value;
            }
        }
        private string _disposition;
        public string Disposition
        {
            get
            {
                return _disposition;
            }
            set
            {
                _disposition = value;
            }
        }
        public Decimal QualifiedQty { get; set; }

        public Decimal RejectedQty { get; set; }

        private DateTime? _createDate;
        public DateTime? CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
            }
        }
        private string _createUser;
        public string CreateUser
        {
            get
            {
                return _createUser;
            }
            set
            {
                _createUser = value;
            }
        }

        public string CreateUserNm { get; set; }

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
            InspectComfirmResultBase another = obj as InspectComfirmResultBase;

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
