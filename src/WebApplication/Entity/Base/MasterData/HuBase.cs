using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public abstract class HuBase : EntityBase
    {
        #region O/R Mapping Properties

        private string _huId;
        public string HuId
        {
            get
            {
                return _huId;
            }
            set
            {
                _huId = value;
            }
        }
        private string _lotNo;
        public string LotNo
        {
            get
            {
                return _lotNo;
            }
            set
            {
                _lotNo = value;
            }
        }
        private com.Sconit.Entity.MasterData.Item _item;
        public com.Sconit.Entity.MasterData.Item Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
            }
        }
        private string _qualityLevel;
        public string QualityLevel
        {
            get
            {
                return _qualityLevel;
            }
            set
            {
                _qualityLevel = value;
            }
        }
        private com.Sconit.Entity.MasterData.Uom _uom;
        public com.Sconit.Entity.MasterData.Uom Uom
        {
            get
            {
                return _uom;
            }
            set
            {
                _uom = value;
            }
        }
        private Decimal _unitQty;
        public Decimal UnitQty
        {
            get
            {
                return _unitQty;
            }
            set
            {
                _unitQty = value;
            }
        }
        private Decimal _qty;
        public Decimal Qty
        {
            get
            {
                return _qty;
            }
            set
            {
                _qty = value;
            }
        }
        private string _orderNo;
        public string OrderNo
        {
            get
            {
                return _orderNo;
            }
            set
            {
                _orderNo = value;
            }
        }
        private string _receiptNo;
        public string ReceiptNo
        {
            get
            {
                return _receiptNo;
            }
            set
            {
                _receiptNo = value;
            }
        }
        private DateTime _manufactureDate;
        public DateTime ManufactureDate
        {
            get
            {
                return _manufactureDate;
            }
            set
            {
                _manufactureDate = value;
            }
        }
        private com.Sconit.Entity.MasterData.Party _manufactureParty;
        public com.Sconit.Entity.MasterData.Party ManufactureParty
        {
            get
            {
                return _manufactureParty;
            }
            set
            {
                _manufactureParty = value;
            }
        }
        private string _remark;
        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                _remark = value;
            }
        }
        private string _parentHuId;
        public string ParentHuId
        {
            get
            {
                return _parentHuId;
            }
            set
            {
                _parentHuId = value;
            }
        }
        private Int32 _printCount;
        public Int32 PrintCount
        {
            get
            {
                return _printCount;
            }
            set
            {
                _printCount = value;
            }
        }
        private DateTime _createDate;
        public DateTime CreateDate
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
        private com.Sconit.Entity.MasterData.User _createUser;
        public com.Sconit.Entity.MasterData.User CreateUser
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
        private Decimal _unitCount;
        public Decimal UnitCount
        {
            get
            {
                return _unitCount;
            }
            set
            {
                _unitCount = value;
            }
        }
        private DateTime? _expiredDate;
        public DateTime? ExpiredDate
        {
            get
            {
                return _expiredDate;
            }
            set
            {
                _expiredDate = value;
            }
        }
        public String Version { get; set; }
        public Decimal LotSize { get; set; }
        public String AntiResolveHu { get; set; }
        public String Location { get; set; }
        public String Status { get; set; }
        public String CustomerItemCode { get; set; }
        private String _storageBin;
        public String StorageBin
        {
            get
            {
                return _storageBin;
            }
            set
            {
                _storageBin = value;
            }
        }
        public string HuTemplate { get; set; }
        #endregion

        #region O/R Mapping Retention Properties

        private string _textField1;
        public string TextField1   //´ÃµãÊý
        {
            get
            {
                return _textField1;
            }
            set
            {
                _textField1 = value;
            }
        }
        private string _textField2;
        public string TextField2  //ÈÃÂëÊý
        {
            get
            {
                return _textField2;
            }
            set
            {
                _textField2 = value;
            }
        }
        private string _textField3;
        public string TextField3
        {
            get
            {
                return _textField3;
            }
            set
            {
                _textField3 = value;
            }
        }
        private string _textField4;
        public string TextField4
        {
            get
            {
                return _textField4;
            }
            set
            {
                _textField4 = value;
            }
        }

        private Decimal? _numField1;
        public Decimal? NumField1
        {
            get
            {
                return _numField1;
            }
            set
            {
                _numField1 = value;
            }
        }
        private Decimal? _numField2;
        public Decimal? NumField2
        {
            get
            {
                return _numField2;
            }
            set
            {
                _numField2 = value;
            }
        }
        private Decimal? _numField3;
        public Decimal? NumField3
        {
            get
            {
                return _numField3;
            }
            set
            {
                _numField3 = value;
            }
        }
        private Decimal? _numField4;
        public Decimal? NumField4
        {
            get
            {
                return _numField4;
            }
            set
            {
                _numField4 = value;
            }
        }

        private DateTime? _dateField1;
        public DateTime? DateField1
        {
            get
            {
                return _dateField1;
            }
            set
            {
                _dateField1 = value;
            }
        }
        private DateTime? _dateField2;
        public DateTime? DateField2
        {
            get
            {
                return _dateField2;
            }
            set
            {
                _dateField2 = value;
            }
        }

        #endregion

        public override int GetHashCode()
        {
            if (HuId != null)
            {
                return HuId.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            HuBase another = obj as HuBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.HuId == another.HuId);
            }
        }
    }

}
