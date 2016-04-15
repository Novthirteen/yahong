using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MasterData
{
    [Serializable]
    public class LocationTransaction : LocationTransactionBase
    {
        #region Non O/R Mapping Properties

        public bool LocationIsFreeze { get; set; }

        public bool ItemIsFreeze { get; set; }

        #endregion
    }
}