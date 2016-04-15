using com.Sconit.Service.Ext.MasterData;


using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using NHibernate.Expression;
using System;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Persistence;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class SupplierMgr : SupplierBaseMgr, ISupplierMgr
    {

        public IAddressMgrE addressMgrE { get; set; }
        public IWorkCenterMgrE workCenterMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IPermissionMgrE permissionMgrE { get; set; }
        public IPermissionCategoryMgrE permissionCategoryMgrE { get; set; }
        public IUserPermissionMgrE userPermissionMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public IPartyDao partyDao { get; set; }
        public ISqlHelperDao sqlHelperDao { get; set; }


        #region Customized Methods
        [Transaction(TransactionMode.Requires)]
        public override void CreateSupplier(Supplier entity)
        {
            CreateSupplier(entity, userMgrE.GetMonitorUser());
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateSupplier(Supplier entity, User currentUser)
        {
            if (partyDao.LoadParty(entity.Code) == null)
            {
                base.CreateSupplier(entity);
            }
            else
            {
                CreateSupplierOnly(entity);
            }
            Permission permission = new Permission();
            permission.Category = permissionCategoryMgrE.LoadPermissionCategory(BusinessConstants.CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_SUPPLIER);
            permission.Code = entity.Code;
            permission.Description = entity.Name;
            permissionMgrE.CreatePermission(permission);
            UserPermission userPermission = new UserPermission();
            userPermission.Permission = permission;
            userPermission.User = currentUser;
            userPermissionMgrE.CreateUserPermission(userPermission);
        }

        [Transaction(TransactionMode.Unspecified)]
        public int CreateSupplierOnly(Supplier entity)
        {
            string sql = "insert into Supplier(code) values(@code) ";

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@code", SqlDbType.NVarChar, 50);
            param[0].Value = entity.Code;
            return sqlHelperDao.Create(sql, param);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteSupplier(string code)
        {
            IList<UserPermission> userPermissionList = userPermissionMgrE.GetUserPermission(code);
            userPermissionMgrE.DeleteUserPermission(userPermissionList);
            permissionMgrE.DeletePermission(code);
            if (partyDao.LoadParty(code) == null)
            {
                workCenterMgrE.DeleteWorkCenterByParent(code);
                addressMgrE.DeleteAddressByParent(code);
                base.DeleteSupplier(code);
            }
            else
            {
                DeleteSupplierOnly(code);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteSupplier(Supplier supplier)
        {
            DeleteSupplier(supplier.Code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public int DeleteSupplierOnly(string code)
        {
            string sql = "delete from Supplier where code=@code ";

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@code", SqlDbType.NVarChar, 50);
            param[0].Value = code;
            return sqlHelperDao.Delete(sql, param);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Supplier> GetSupplier(string userCode)
        {
            return GetSupplier(userCode, false);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Supplier> GetSupplier(string userCode, bool includeInactive)
        {
            DetachedCriteria criteria = DetachedCriteria.For<Supplier>();
            if (!includeInactive)
            {
                criteria.Add(Expression.Eq("IsActive", true));
            }

            DetachedCriteria[] pCrieteria = SecurityHelper.GetSupplierPermissionCriteria(userCode);

            criteria.Add(
                Expression.Or(
                    Subqueries.PropertyIn("Code", pCrieteria[0]),
                    Subqueries.PropertyIn("Code", pCrieteria[1])));

            return criteriaMgrE.FindAll<Supplier>(criteria);
        }

        #endregion Customized Methods
    }
}


#region Extend Class


namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class SupplierMgrE : com.Sconit.Service.MasterData.Impl.SupplierMgr, ISupplierMgrE
    {


    }
}
#endregion
