using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;
using System.Text;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IUserSubscriptionMgr : IUserSubscriptionBaseMgr
    {
        #region Customized Methods

        string FindEmail(string[] userCodes);

        string FindEmailByPermission(string[] permissionCodes);

        IList<UserSub> GetAllUser();
        IList<UserSub> GetCacheAllUser();
        IList<UserSubView> GetUserAllTaskSubType(string userCode);

        void UpdateUserSubscription(UserSubView userSubView, User user);

        void UpdateUserSubscription(IList<UserSubView> userSubViewList, User user);

        void Check();
        string GetUserName(string userCodes);
        string GetDesc(string type, User user);

        void GenerateUserSub(IList<UserSub> userSubList, string users);
        IList<UserSub> GenerateUserSub(string taskSubType, User user);
        IList<UserSub> SubmitUserSub(TaskMstr task, User user);
        IList<UserSub> GenerateUserSub(TaskMstr task, string userCodes, bool isUserSub, User user);
        string GetSubject(User user, string code, string type, string priority, string value, string level, string status);
        void GenerateUserSub(string taskType, string taskSubTypeCode, string taskCode, User user, IList<UserSub> userSubList, string users);
        void Remind(string subject, StringBuilder body, string mailTo);
        void Remind(string subject, StringBuilder body, string mailTo, IList<string> files);
        void Remind(TaskMstr task, string level, IList<UserSub> userSubList, User operationUser);
        void Remind(TaskMstr task, IList<UserSub> userSubList, User operationUser);
        void Remind(TaskMstr task, string level, IList<UserSub> userSubList, bool isApprove, User operationUser);
        void Remind(TaskMstr task, IList<UserSub> userSubList, string helpContent, User operationUser);
        void Remind(TaskMstr task, string level, double minutes, IList<UserSub> userSubList, User operationUser);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IUserSubscriptionMgrE : com.Sconit.ISI.Service.IUserSubscriptionMgr
    {
    }
}

#endregion Extend Interface