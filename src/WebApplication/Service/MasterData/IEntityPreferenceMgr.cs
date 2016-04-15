using com.Sconit.Service.Ext.MasterData;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData
{
    public interface IEntityPreferenceMgr : IEntityPreferenceBaseMgr
    {
        #region Customized Methods

        IList<EntityPreference> GetAllEntityPreferenceOrderBySeq();

        IList<EntityPreference> GetEntityPreferenceOrderBySeq(string codePart, bool include);

        IList<EntityPreference> GetEntityPreferenceOrderBySeq(string[] codeArrays);

        #endregion Customized Methods
    }
}





#region Extend Interface





namespace com.Sconit.Service.Ext.MasterData
{
    public partial interface IEntityPreferenceMgrE : com.Sconit.Service.MasterData.IEntityPreferenceMgr
    {
        
    }
}

#endregion
