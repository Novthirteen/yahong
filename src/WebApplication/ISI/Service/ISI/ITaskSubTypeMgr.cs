using System;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskSubTypeMgr : ITaskSubTypeBaseMgr
    {
        #region Customized Methods

        IList<TaskSubType> GetCacheAllTaskSubType();

        IList<TaskSubType> GetTaskSubType(string userCode);

        IList<TaskSubType> GetTaskSubType(string userCode, bool includePrivacy);

        IList<TaskSubType> GetTaskSubType(string type, string userCode);

        IList<TaskSubType> GetTaskSubTypeList(string type);

        IList<TaskSubType> GetTaskSubType(string type, string userCode, bool includeInactive);

        IList<TaskSubType> GetPublicTaskSubType(string type, string userCode);

        IList<TaskSubType> GetProjectTaskSubType(string type, string userCode, bool isProjectImportCheck);

        IList<TaskSubType> GetWFSTaskSubType();
        IList<TaskSubType> GetWFSTaskSubType(string type, string userCode);
        IList<TaskSubType> GetWFSTaskSubType(string type, string userCode, string taskSubTypeCode);
        IList<TaskSubType> GetTaskSubType(string type, bool onlyType, string userCode, bool includeInactive, bool includePrivacy, bool onlyPublic, bool? isProjectImportCheck);

        bool ExistsCode(string code);

        bool IsRef(string code);

        IList<User> GetUser(string taskSubType);

        IList<TaskSubType> GetTaskSubTypeNotCode(string code);

        IList<TaskSubType> GetCostCenter();

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskSubTypeMgrE : com.Sconit.ISI.Service.ITaskSubTypeMgr
    {
    }
}

#endregion Extend Interface