using System;

//TODO: Add other using statements here

namespace com.Sconit.ISI.Entity
{
    [Serializable]
    public class TaskAddress : TaskAddressBase
    {
        #region Non O/R Mapping Properties

        private string _name = string.Empty;
        public string Name
        {
            get
            {
                if (_name == string.Empty)
                {
                    _name = this.Desc;
                }
                return _name;
            }
            set
            {
                _name = value;
            }

        }

        #endregion
    }
}