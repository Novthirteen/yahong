using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IEmppMgr
    {
        //void Send(string mobilePhones, string msg, User user);

        //void Send(IssueHead issue, string level, string mobilePhones, string msg, User user);

        void AsyncSend(string mobilePhones, string msg, User user);

        //void Send(string mobilePhones, string msg, User user);

        //void AsyncSend(string level, string mobilePhones, string msg, User user);

        //void Send(string level, string mobilePhones, string msg, User user);

    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IEmppMgrE : com.Sconit.ISI.Service.IEmppMgr
    {
    }
}

#endregion Extend Interface