using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public abstract class PickListDetailBase : EntityBase
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
		private com.Sconit.Entity.MasterData.PickList _pickList;
		public com.Sconit.Entity.MasterData.PickList PickList
		{
			get
			{
				return _pickList;
			}
			set
			{
				_pickList = value;
			}
		}
        private com.Sconit.Entity.MasterData.OrderLocationTransaction _orderLocationTransaction;
        public com.Sconit.Entity.MasterData.OrderLocationTransaction OrderLocationTransaction
		{
			get
			{
                return _orderLocationTransaction;
			}
			set
			{
                _orderLocationTransaction = value;
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
		private com.Sconit.Entity.MasterData.StorageArea _storageArea;
		public com.Sconit.Entity.MasterData.StorageArea StorageArea
		{
			get
			{
				return _storageArea;
			}
			set
			{
				_storageArea = value;
			}
		}
		private com.Sconit.Entity.MasterData.StorageBin _storageBin;
		public com.Sconit.Entity.MasterData.StorageBin StorageBin
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

        private IList<PickListResult> _pickListResults;
        public IList<PickListResult> PickListResults
        {
            get
            {
                return _pickListResults;
            }
            set
            {
                _pickListResults = value;
            }
        }
        public String Memo { get; set; }
        #endregion

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
            PickListDetailBase another = obj as PickListDetailBase;

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
