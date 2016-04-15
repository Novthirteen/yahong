using System;
using com.Sconit.Entity.Distribution;
using com.Sconit.Entity.Procurement;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class FlowDetail : FlowDetailBase
    {
        #region Non O/R Mapping Properties
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Location DefaultLocationFrom
        {
            get
            {
                if (this.LocationFrom == null)
                {
                    return this.Flow.LocationFrom;
                }
                else
                {
                    return this.LocationFrom;
                }
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Location DefaultLocationTo
        {
            get
            {
                if (this.LocationTo == null)
                {
                    return this.Flow.LocationTo;
                }
                else
                {
                    return this.LocationTo;
                }
            }
        }

        public BillAddress DefaultBillAddress
        {
            get
            {
                if (this.BillAddress == null)
                {
                    return this.Flow.BillAddress;
                }
                else
                {
                    return this.BillAddress;
                }
            }
        }

        public PriceList DefaultPriceList
        {
            get
            {
                if (this.PriceList == null)
                {
                    return this.Flow.PriceList;
                }
                else
                {
                    return this.PriceList;
                }
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string DefaultBillSettleTerm
        {
            get
            {
                if (this.BillSettleTerm == null)
                {
                    return this.Flow.BillSettleTerm;
                }
                else
                {
                    return this.BillSettleTerm;
                }
            }
        }

        private decimal _orderedQty;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public decimal OrderedQty
        {
            get
            {
                return this._orderedQty;
            }
            set
            {
                this._orderedQty = value;
            }
        }

        public bool IsBlankDetail { get; set; }

        public string HuLotNo { get; set; }

        public string HuShiftCode { get; set; }

        public string ItemVersion { get; set; }

        public string Memo { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string DefaultBomCode
        {
            get
            {
                if (this.Bom != null)
                {
                   return this.Bom.Code;
                }
                else
                {
                    if (this.Item.Bom!=null)
                    {
                        return this.Item.Bom.Code;
                    }
                    return Item.Code;
                }
            }
        }
        #endregion
    }
}