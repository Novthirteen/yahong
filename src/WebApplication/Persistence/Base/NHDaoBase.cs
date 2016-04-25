using System;
using System.Text;
using System.Collections;

using Castle.Facilities.NHibernateIntegration;
using Castle.Facilities.NHibernateIntegration.Util;

using NHibernate;
using NHibernate.Collection;
using NHibernate.Expression;
using NHibernate.Proxy;
using NHibernate.Transform;
using NHibernate.Type;
using System.Collections.Generic;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;

namespace com.Sconit.Persistence
{
    /// <summary>
    /// Summary description for GenericDao.
    /// </summary>
    /// <remarks>
    ///  Contributed by Jackey <Jackey.ding@atosorigin.com>
    /// </remarks>
    /// 
    public class NHDaoBase : INHDao
    {
        private readonly ISessionManager sessionManager;
        private string sessionFactoryAlias = null;

        public NHDaoBase(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        protected ISessionManager SessionManager
        {
            get { return sessionManager; }
        }

        public string SessionFactoryAlias
        {
            get { return sessionFactoryAlias; }
            set { sessionFactoryAlias = value; }
        }


        #region IDAOBase Members

        public virtual IList<T> FindAll<T>()
        {
            return FindAll<T>(int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAll<T>(int firstRow, int maxRows)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    ICriteria criteria = session.CreateCriteria(typeof(T));

                    if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
                    IList<T> result = criteria.List<T>();
                    if (result == null || result.Count == 0) return null;

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindAll for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual T FindById<T>(object id)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return session.Load<T>(id);
                }
                catch (ObjectNotFoundException)
                {
                    //throw;
                    return default(T);
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindByPrimaryKey for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual object Create(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return session.Save(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Create for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void Delete(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(instance);
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Delete for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void Update(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Update(instance);
                    //SaveOrUpdateCopy可以解决在hibernate中同一个session里面有了两个相同标识的错误
                    //a different object with the same identifier value was already associated with the session
                    //不知道有没有什么未知影响
                    //session.SaveOrUpdateCopy(instance);
                    session.Flush();
                }
                catch (NHibernate.StaleObjectStateException se)
                {
                    throw new BusinessErrorException("Common.StaleObjectStateException", se);
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Update for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void DeleteAll(Type type)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(String.Format("from {0}", type.Name));
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform DeleteAll for " + type.Name, ex);
                }
            }
        }

        public virtual void Save(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.SaveOrUpdate(instance);
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Save for " + instance.GetType().Name, ex);
                }
            }
        }

        #endregion

        #region INHDaoBase Members

        public virtual void Delete(string hqlString)
        {
            Delete(hqlString, (object[])null, (IType[])null);
        }

        public virtual void Delete(string hqlString, object value, IType type)
        {
            Delete(hqlString, new object[] { value }, new IType[] { type });
        }

        public virtual void Delete(string hqlString, object[] values, IType[] types)
        {
            if (hqlString == null || hqlString.Length == 0) throw new ArgumentNullException("hqlString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    if (values == null)
                    {
                        session.Delete(hqlString);
                    }
                    else
                    {
                        session.Delete(hqlString, values, types);
                    }
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Delete for " + hqlString, ex);
                }
            }
        }

        public virtual IList FindAllWithCustomQuery(string queryString)
        {
            return FindAllWithCustomQuery(queryString, (object[])null, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object value)
        {
            return FindAllWithCustomQuery(queryString, new object[] { value }, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object value, IType type)
        {
            return FindAllWithCustomQuery(queryString, new object[] { value }, new IType[] { type }, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object[] values)
        {
            return FindAllWithCustomQuery(queryString, values, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object[] values, IType[] types)
        {
            return FindAllWithCustomQuery(queryString, values, types, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery(queryString, (object[])null, (IType[])null, firstRow, maxRows);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object value, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery(queryString, new object[] { value }, (IType[])null, firstRow, maxRows);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object value, IType type, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery(queryString, new object[] { value }, new IType[] { type }, firstRow, maxRows);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object[] values, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery(queryString, values, (IType[])null, firstRow, maxRows);
        }

        public virtual IList FindAllWithCustomQuery(string queryString, object[] values, IType[] types, int firstRow, int maxRows)
        {
            if (queryString == null || queryString.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.CreateQuery(queryString);
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
                    IList result = query.List();
                    if (result == null || result.Count == 0) return null;

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + queryString, ex);
                }
            }
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery)
        {
            return FindAllWithNamedQuery(namedQuery, (object[])null, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object value)
        {
            return FindAllWithNamedQuery(namedQuery, new object[] { value }, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object value, IType type)
        {
            return FindAllWithNamedQuery(namedQuery, new object[] { value }, new IType[] { type }, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object[] values)
        {
            return FindAllWithNamedQuery(namedQuery, values, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types)
        {
            return FindAllWithNamedQuery(namedQuery, values, types, int.MinValue, int.MinValue);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery(namedQuery, (object[])null, (IType[])null, firstRow, maxRows);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object value, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery(namedQuery, new object[] { value }, (IType[])null, firstRow, maxRows);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object value, IType type, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery(namedQuery, new object[] { value }, new IType[] { type }, firstRow, maxRows);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object[] values, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery(namedQuery, values, (IType[])null, firstRow, maxRows);
        }

        public virtual IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows)
        {
            if (namedQuery == null || namedQuery.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.GetNamedQuery(namedQuery);
                    if (query == null) throw new ArgumentException("Cannot find named query", "namedQuery");
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
                    IList result = query.List();
                    if (result == null || result.Count == 0) return null;

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for named query : " + namedQuery, ex);
                }
            }
        }

        public virtual IList<T> FindAll<T>(ICriterion[] criterias)
        {
            return FindAll<T>(criterias, null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAll<T>(ICriterion[] criterias, int firstRow, int maxRows)
        {
            return FindAll<T>(criterias, null, firstRow, maxRows);
        }

        public virtual IList<T> FindAll<T>(ICriterion[] criterias, Order[] sortItems)
        {
            return FindAll<T>(criterias, sortItems, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAll<T>(ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    ICriteria criteria = session.CreateCriteria(typeof(T));

                    if (criterias != null)
                    {
                        foreach (ICriterion cond in criterias)
                            criteria.Add(cond);
                    }

                    if (sortItems != null)
                    {
                        foreach (Order order in sortItems)
                            criteria.AddOrder(order);
                    }

                    if (firstRow != int.MinValue) criteria.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) criteria.SetMaxResults(maxRows);
                    IList<T> result = criteria.List<T>();
                    if (result == null || result.Count == 0) return null;

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform FindAll for " + typeof(T).Name, ex);
                }
            }
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString)
        {
            return FindAllWithCustomQuery<T>(queryString, (object[])null, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object value)
        {
            return FindAllWithCustomQuery<T>(queryString, new object[] { value }, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object value, IType type)
        {
            return FindAllWithCustomQuery<T>(queryString, new object[] { value }, new IType[] { type }, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values)
        {
            return FindAllWithCustomQuery<T>(queryString, values, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values, IType[] types)
        {
            return FindAllWithCustomQuery<T>(queryString, values, types, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery<T>(queryString, (object[])null, (IType[])null, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object value, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery<T>(queryString, new object[] { value }, (IType[])null, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object value, IType type, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery<T>(queryString, new object[] { value }, new IType[] { type }, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values, int firstRow, int maxRows)
        {
            return FindAllWithCustomQuery<T>(queryString, values, (IType[])null, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values, IType[] types, int firstRow, int maxRows)
        {
            if (queryString == null || queryString.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.CreateQuery(queryString);
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
                    IList<T> result = query.List<T>();
                    if (result == null || result.Count == 0) return null;

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + queryString, ex);
                }
            }
        }


        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery)
        {
            return FindAllWithNamedQuery<T>(namedQuery, (object[])null, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value)
        {
            return FindAllWithNamedQuery<T>(namedQuery, new object[] { value }, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type)
        {
            return FindAllWithNamedQuery<T>(namedQuery, new object[] { value }, new IType[] { type }, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values)
        {
            return FindAllWithNamedQuery<T>(namedQuery, values, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types)
        {
            return FindAllWithNamedQuery<T>(namedQuery, values, types, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery<T>(namedQuery, (object[])null, (IType[])null, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery<T>(namedQuery, new object[] { value }, (IType[])null, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery<T>(namedQuery, new object[] { value }, new IType[] { type }, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, int firstRow, int maxRows)
        {
            return FindAllWithNamedQuery<T>(namedQuery, values, (IType[])null, firstRow, maxRows);
        }

        public virtual IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows)
        {
            if (namedQuery == null || namedQuery.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.GetNamedQuery(namedQuery);
                    if (query == null) throw new ArgumentException("Cannot find named query", "namedQuery");
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
                    IList<T> result = query.List<T>();
                    if (result == null || result.Count == 0) return null;

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for named query : " + namedQuery, ex);
                }
            }
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string namedQuery, IDictionary<string, object> param)
        {
            return FindAllWithCustomQuery<T>(namedQuery, param, (IType[])null, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string namedQuery, IDictionary<string, object> param, IType[] types)
        {
            return FindAllWithCustomQuery<T>(namedQuery, param, types, int.MinValue, int.MinValue);
        }

        public virtual IList<T> FindAllWithCustomQuery<T>(string queryString, IDictionary<string, object> param, IType[] types, int firstRow, int maxRows)
        {

            if (queryString == null || queryString.Length == 0) throw new ArgumentNullException("queryString");
            if (param != null && types != null && types.Length != param.Count) throw new ArgumentException("Length of param array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.CreateQuery(queryString);
                    if (param != null)
                    {
                        int i = 0;
                        foreach (string key in param.Keys)
                        {
                            if (param[key] is IList)
                            {
                                if (types != null && types[i] != null)
                                {
                                    query.SetParameterList(key, (IEnumerable)param[key], types[i]);
                                }
                                else
                                {
                                    query.SetParameterList(key, (IEnumerable)param[key]);
                                }
                            }
                            else
                            {
                                if (types != null && types[i] != null)
                                {
                                    query.SetParameter(key, param[key], types[i]);
                                }
                                else
                                {
                                    query.SetParameter(key, param[key]);
                                }
                            }
                            i++;
                        }

                    }

                    if (firstRow != int.MinValue) query.SetFirstResult(firstRow);
                    if (maxRows != int.MinValue) query.SetMaxResults(maxRows);
                    IList<T> result = query.List<T>();
                    if (result == null || result.Count == 0) return null;

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + queryString, ex);
                }
            }
        }

        public void InitializeLazyProperties(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            using (ISession session = GetSession())
            {
                foreach (object val in ReflectionUtil.GetPropertiesDictionary(instance).Values)
                {
                    if (val is INHibernateProxy || val is IPersistentCollection)
                    {
                        if (!NHibernateUtil.IsInitialized(val))
                        {
                            session.Lock(instance, LockMode.None);
                            NHibernateUtil.Initialize(val);
                        }
                    }
                }
            }
        }

        public void InitializeLazyProperty(object instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null || propertyName.Length == 0) throw new ArgumentNullException("collectionPropertyName");

            IDictionary properties = ReflectionUtil.GetPropertiesDictionary(instance);
            if (!properties.Contains(propertyName))
                throw new ArgumentOutOfRangeException("collectionPropertyName", "Property "
                    + propertyName + " doest not exist for type "
                    + instance.GetType().ToString() + ".");

            using (ISession session = GetSession())
            {
                object val = properties[propertyName];

                if (val is INHibernateProxy || val is IPersistentCollection)
                {
                    if (!NHibernateUtil.IsInitialized(val))
                    {
                        session.Lock(instance, LockMode.None);
                        NHibernateUtil.Initialize(val);
                    }
                }
            }
        }

        public void FlushSession()
        {
            using (ISession session = GetSession())
            {
                session.Flush();
            }
        }

        public void CleanSession()
        {
            using (ISession session = GetSession())
            {
                session.Clear();
            }
        }

        #endregion

        #region FindWithSql
        public IList<T> FindEntityWithNativeSql<T>(string sql)
        {
            return FindEntityWithNativeSql<T>(sql, (object[])null, (IType[])null);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object value)
        {
            return FindEntityWithNativeSql<T>(sql, new object[] { value }, (IType[])null);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object value, IType type)
        {
            return FindEntityWithNativeSql<T>(sql, new object[] { value }, new IType[] { type });

        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object[] values)
        {
            return FindEntityWithNativeSql<T>(sql, values, (IType[])null);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object[] values, IType[] types)
        {
            if (sql == null || sql.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    ISQLQuery query = session.CreateSQLQuery(sql).AddEntity(typeof(T));
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    IList<T> result = query.List<T>();

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + sql, ex);
                }
            }
        }

        public IList FindAllWithNativeSql(string sql, object[] values, IType[] types)
        {
            if (sql == null || sql.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    ISQLQuery query = session.CreateSQLQuery(sql);
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    IList result = query.List();

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + sql, ex);
                }
            }
        }

        public IList<T> FindAllWithNativeSql<T>(string sql)
        {
            return FindAllWithNativeSql<T>(sql, (object[])null, (IType[])null);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object value)
        {
            return FindAllWithNativeSql<T>(sql, new object[] { value }, (IType[])null);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object value, IType type)
        {
            return FindAllWithNativeSql<T>(sql, new object[] { value }, new IType[] { type });

        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object[] values)
        {
            return FindAllWithNativeSql<T>(sql, values, (IType[])null);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object[] values, IType[] types)
        {
            if (sql == null || sql.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    ISQLQuery query = session.CreateSQLQuery(sql);
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    IList<T> result = query.List<T>();

                    return result;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + sql, ex);
                }
            }
        }

        public IList FindAllWithNativeSql(string sql)
        {
            return FindAllWithNativeSql(sql, (object[])null, (IType[])null);
        }

        public IList FindAllWithNativeSql(string sql, object value)
        {
            return FindAllWithNativeSql(sql, new object[] { value }, (IType[])null);
        }

        public IList FindAllWithNativeSql(string sql, object value, IType type)
        {
            return FindAllWithNativeSql(sql, new object[] { value }, new IType[] { type });

        }

        public IList FindAllWithNativeSql(string sql, object[] values)
        {
            return FindAllWithNativeSql(sql, values, (IType[])null);
        }

        #endregion
        public void GetTableProperty(object entity, out string tableName, out Dictionary<string, string> propertyAndColumnNames)
        {
            using (ISession session = GetSession())
            {
                NHibernateHelper.GetTableProperty(session, entity, out tableName, out propertyAndColumnNames);
            }
        }
      
        #region protected methods
        public ISession GetSession()
        {
            if (sessionFactoryAlias == null || sessionFactoryAlias.Length == 0)
                return sessionManager.OpenSession();
            else
                return sessionManager.OpenSession(sessionFactoryAlias);
        }
        #endregion
    }
}
