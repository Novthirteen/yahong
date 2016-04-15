using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class BomDetail : BomDetailBase
    {
        #region Non O/R Mapping Properties

        //选装件分组标记，默认取父件的BomCode
        private string _optionalItemGroup;
        public string OptionalItemGroup
        {
            set
            {
                this._optionalItemGroup = value;
            }
            get
            {
                return this._optionalItemGroup;
            }
        }

        private decimal _calculatedQty = 1;
        public decimal CalculatedQty
        {
            set
            {
                this._calculatedQty = value;
            }
            get
            {
                return this._calculatedQty;
            }
        }

        //默认flow上设置的库位
        private Location _defaultLocation;
        public Location DefaultLocation
        {
            set
            {
                this._defaultLocation = value;
            }
            get
            {
                return this._defaultLocation;
            }
        }
        #endregion

        public decimal DefaultScrapPercentage
        {
            get
            {
                if (this.ScrapPercentage.HasValue)
                {
                    return this.ScrapPercentage.Value;
                }
                else if (this.Item.ScrapPercentage.HasValue)
                {
                    return this.Item.ScrapPercentage.Value;
                }

                return 0;
            }
        }

        private string _scrapPctString;
        public string ScrapPctString
        {
            get
            {
                return ScrapPercentage.HasValue ? ScrapPercentage.Value.ToString() : string.Empty;
            }

            set
            {
                _scrapPctString = value;
            }

        }

        public string BomCode
        {
            get { return this.Bom.Code; }
        }

        public string BomDesc
        {
            get { return this.Bom.Description; }
        }

        public string ItemCode
        {
            get { return this.Item.Code; }
        }

        public string ItemDesc
        {
            get { return this.Item.Description; }
        }

        public string UomCode
        {
            get { return this.Uom.Code; }
        }

        public int BomLevel { get; set; }

        public decimal AccumQty { get; set; }
    }
}