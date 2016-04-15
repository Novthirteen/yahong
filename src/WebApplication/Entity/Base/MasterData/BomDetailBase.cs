using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public abstract class BomDetailBase : EntityBase
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
		private Int32 _operation;
		public Int32 Operation
		{
			get
			{
				return _operation;
			}
			set
			{
				_operation = value;
			}
		}
		private string _reference;
		public string Reference
		{
			get
			{
				return _reference;
			}
			set
			{
				_reference = value;
			}
		}
		private string _structureType;
		public string StructureType
		{
			get
			{
				return _structureType;
			}
			set
			{
				_structureType = value;
			}
		}
		private DateTime _startDate;
		public DateTime StartDate
		{
			get
			{
				return _startDate;
			}
			set
			{
				_startDate = value;
			}
		}
		private DateTime? _endDate;
		public DateTime? EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				_endDate = value;
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
		private Decimal _rateQty;
		public Decimal RateQty
		{
			get
			{
				return _rateQty;
			}
			set
			{
				_rateQty = value;
			}
		}
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
		private Boolean _needPrint;
		public Boolean NeedPrint
		{
			get
			{
				return _needPrint;
			}
			set
			{
				_needPrint = value;
			}
		}
        private Int32 _priority;
        public Int32 Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
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
        private Boolean _IsShipScanHu;
        public Boolean IsShipScanHu
		{
			get
			{
                return _IsShipScanHu;
			}
			set
			{
                _IsShipScanHu = value;
			}
		}
		private Int32? _huLotSize;
		public Int32? HuLotSize
		{
			get
			{
				return _huLotSize;
			}
			set
			{
				_huLotSize = value;
			}
		}
        private string _backFlushMethod;
        public string BackFlushMethod
        {
            get
            {
                return _backFlushMethod;
            }
            set
            {
                _backFlushMethod = value;
            }
        }
        #endregion

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

		public override int GetHashCode()
        {
			if (Id != 0)
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
            BomDetailBase another = obj as BomDetailBase;

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
