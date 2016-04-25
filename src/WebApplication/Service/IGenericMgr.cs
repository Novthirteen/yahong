using System;
using System.Collections;
using NHibernate.Type;
using System.Collections.Generic;
using System.Data.SqlClient;
using NHibernate.Expression;

namespace com.Sconit.Service
{
    public interface IGenericMgr
    {
        void Save(object instance);

        void BulkInsert<T>(IList<T> instanceList);

        void CreateWithTrim(object instance);

        void UpdateWithTrim(object instance);

        void Create(object instance);

        void Update(object instance);

        void Delete(object instance);

        void DeleteById<T>(object id);

        void Delete<T>(IList<T> instances);

        void Delete(string hqlString);

        void Delete(string hqlString, object value, IType type);

        void Delete(string hqlString, object[] values, IType[] types);

        void FlushSession();

        void CleanSession();

        T FindById<T>(object id);

        IList<T> FindAll<T>();

        IList<T> FindAll<T>(string hql);
        IList<T> FindAll<T>(string hql, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql, IEnumerable<object> values);
        IList<T> FindAll<T>(string hql, IEnumerable<object> values, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql, object value);
        IList<T> FindAll<T>(string hql, object value, int firstRow, int maxRows);

        IList<T> FindAll<T>(ICriterion[] criteria);
        IList<T> FindAll<T>(ICriterion[] criteria, int firstRow, int maxRows);

        IList<T> FindAllIn<T>(string hql, IEnumerable<object> inParam, IEnumerable<object> param = null);
        IList<T> FindAll<T>(string hqlString, IDictionary<string, object> param);

        //IList<T> FindAllWithNativeSql<T>(string sql);
        //IList<T> FindAllWithNativeSql<T>(string sql, object value, IType type = null);
        //IList<T> FindAllWithNativeSql<T>(string sql, IEnumerable<object> values, IEnumerable<IType> types = null);
    }
}
