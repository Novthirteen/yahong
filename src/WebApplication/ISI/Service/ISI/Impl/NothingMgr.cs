using com.Sconit.Entity.MasterData;
using System;
using System.Collections;
using System.Collections.Generic;


//TODO: Add other using statements here.

#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    public partial class NothingMgrE : IEmppMgrE
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        public NothingMgrE() { }

        public void AsyncSend(string mobilePhones, string msg, User user)
        {
            log.Info("NothingMgrE.AsyncSend was called");
        }
    }
}

#endregion Extend Class