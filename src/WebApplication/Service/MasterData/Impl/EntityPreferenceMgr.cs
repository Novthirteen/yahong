using com.Sconit.Service.Ext.MasterData;


using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity.MasterData;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using NHibernate.Expression;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class EntityPreferenceMgr : EntityPreferenceBaseMgr, IEntityPreferenceMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }


        #region Customized Methods

        public IList<EntityPreference> GetAllEntityPreferenceOrderBySeq()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(EntityPreference)).AddOrder(Order.Asc("Seq"));
            return criteriaMgrE.FindAll<EntityPreference>(criteria);
        }

        public IList<EntityPreference> GetEntityPreferenceOrderBySeq(string codePart, bool include)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(EntityPreference)).AddOrder(Order.Asc("Seq"));
            if (!string.IsNullOrEmpty(codePart))
            {
                if (include)
                {
                    criteria.Add(Expression.Like("Code", codePart, MatchMode.Start));
                }
                else
                {
                    criteria.Add(Expression.Not(Expression.Like("Code", codePart, MatchMode.Start)));
                }
            }
            return criteriaMgrE.FindAll<EntityPreference>(criteria);
        }

        public IList<EntityPreference> GetEntityPreferenceOrderBySeq(string[] codeArrays)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(EntityPreference)).AddOrder(Order.Asc("Seq"));
            if (codeArrays != null && codeArrays.Length > 0)
                criteria.Add(Expression.In("Code", codeArrays));
            return criteriaMgrE.FindAll<EntityPreference>(criteria);
        }

        #endregion Customized Methods
    }
}


#region Extend Class
namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class EntityPreferenceMgrE : com.Sconit.Service.MasterData.Impl.EntityPreferenceMgr, IEntityPreferenceMgrE
    {

    }
}
#endregion
