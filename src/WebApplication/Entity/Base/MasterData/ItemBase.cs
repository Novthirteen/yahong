using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public abstract class ItemBase : EntityBase
    {
        #region O/R Mapping Properties

        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }
        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
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
        private string _desc2;
        public string Desc2
        {
            get
            {
                return _desc2;
            }
            set
            {
                _desc2 = value;
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
        private com.Sconit.Entity.MasterData.Location _location;
        public com.Sconit.Entity.MasterData.Location Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }
        private string _imageUrl;
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
            }
        }
        private com.Sconit.Entity.MasterData.Bom _bom;
        public com.Sconit.Entity.MasterData.Bom Bom
        {
            get
            {
                return _bom;
            }
            set
            {
                _bom = value;
            }
        }
        private com.Sconit.Entity.MasterData.Routing _routing;
        public com.Sconit.Entity.MasterData.Routing Routing
        {
            get
            {
                return _routing;
            }
            set
            {
                _routing = value;
            }
        }
        private Boolean _isActive;
        public Boolean IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }
        private string _memo;
        public string Memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = value;
            }
        }
        private DateTime _lastModifyDate;
        public DateTime LastModifyDate
        {
            get
            {
                return _lastModifyDate;
            }
            set
            {
                _lastModifyDate = value;
            }
        }

        public Decimal? LeadTime
        {
            get;
            set;
        }
        //private com.Sconit.Entity.MasterData.User _lastModifyUser;
        //[System.Xml.Serialization.XmlIgnoreAttribute()]
        //public com.Sconit.Entity.MasterData.User LastModifyUser
        //{
        //    get
        //    {
        //        return _lastModifyUser;
        //    }
        //    set
        //    {
        //        _lastModifyUser = value;
        //    }
        //}
        public string LastModifyUser { get; set; }

        public ItemCategory ItemCategory { get; set; }
        private Decimal? _scrapPercentage;
        public Decimal? ScrapPercentage
        {
            get
            {
                return _scrapPercentage;
            }
            set
            {
                _scrapPercentage = value;
            }
        }

        public ItemBrand ItemBrand { get; set; }
        public string ScrapBillAddress { get; set; }
        public decimal? ScrapPrice { get; set; }
        public bool IsFreeze { get; set; }
        public string Container { get; set; }
        #endregion

        private com.Sconit.Entity.MasterData.ItemType _category1;
        public com.Sconit.Entity.MasterData.ItemType Category1
        {
            get
            {
                return _category1;
            }
            set
            {
                _category1 = value;
            }
        }
        private com.Sconit.Entity.MasterData.ItemType _category2;
        public com.Sconit.Entity.MasterData.ItemType Category2
        {
            get
            {
                return _category2;
            }
            set
            {
                _category2 = value;
            }
        }

        #region O/R Mapping Retention Properties

        private string _textField1;
        public string TextField1
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
        public string TextField2
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
        private string _textField5;
        public string TextField5
        {
            get
            {
                return _textField5;
            }
            set
            {
                _textField5 = value;
            }
        }
        private string _textField6;
        public string TextField6
        {
            get
            {
                return _textField6;
            }
            set
            {
                _textField6 = value;
            }
        }
        private string _textField7;
        public string TextField7
        {
            get
            {
                return _textField7;
            }
            set
            {
                _textField7 = value;
            }
        }
        private string _textField8;
        public string TextField8
        {
            get
            {
                return _textField8;
            }
            set
            {
                _textField8 = value;
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
            ItemBase another = obj as ItemBase;

            if (another == null)
            {
                return false;
            }
            else
            {
                return string.Equals(this.Code, another.Code, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

}
