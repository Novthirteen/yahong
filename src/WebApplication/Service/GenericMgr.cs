namespace com.Sconit.Service.Impl
{
    #region retrive
    using System;
    using System.Collections;
    using Castle.Services.Transaction;
    using com.Sconit.Entity;
    using com.Sconit.Persistence;
    using NHibernate.Type;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using NHibernate.Expression;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [Transactional]
    public class GenericMgr : IGenericMgr
    {
        public GenericMgr(INHDao dao)
        {
            this.dao = dao;
        }
        /// <summary>
        /// NHibernate数据获取对象
        /// </summary>
        private INHDao dao { get; set; }
        public ISqlDao sqlDao { get; set; }

        [Transaction(TransactionMode.Requires)]
        public void Save(object instance)
        {
            dao.Save(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void BulkInsert<T>(IList<T> instanceList)
        {
            if (instanceList != null && instanceList.Count > 0)
            {
                DataTable dataTable = IListToDataTable<T>(instanceList);
                sqlDao.BulkInsert(dataTable);
            }
        }

        private DataTable IListToDataTable<T>(IList<T> instanceList)
        {
            if (instanceList == null || instanceList.Count == 0)
            {
                return null;
            }
            string tableName = string.Empty;
            Dictionary<string, string> propertyAndColumnNames = new Dictionary<string, string>();
            dao.GetTableProperty(instanceList.First(), out tableName, out propertyAndColumnNames);

            DataTable dt = new DataTable(tableName);

            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (T t in instanceList)
            {
                if (t == null)
                {
                    continue;
                }

                DataRow row = dt.NewRow();

                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];
                    if (pi.CanWrite)
                    {
                        if (propertyAndColumnNames.ContainsKey(pi.Name))
                        {
                            string columnName = propertyAndColumnNames[pi.Name];
                            if (dt.Columns[columnName] == null)
                            {
                                DataColumn column = new DataColumn(columnName);
                                if (!pi.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    column = new DataColumn(columnName, pi.PropertyType);
                                }
                                dt.Columns.Add(column);
                            }
                            row[columnName] = pi.GetValue(t, null);
                        }
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateWithTrim(object instance)
        {
            TrimInstance(instance);
            this.Create(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateWithTrim(object instance)
        {
            TrimInstance(instance);
            this.Update(instance);
        }

        private static void TrimInstance(object instance)
        {
            PropertyInfo[] propertyInfo = instance.GetType().GetProperties();
            foreach (PropertyInfo pi in propertyInfo)
            {
                object oldValue = pi.GetValue(instance, null);
                if (pi.CanWrite && pi.PropertyType.ToString() == "System.String" && oldValue != null)
                {
                    string newValue = oldValue.ToString().Trim();
                    if (newValue == string.Empty)
                    {
                        newValue = null;
                    }
                    pi.SetValue(instance, newValue, null);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Create(object instance)
        {
            dao.Create(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void Update(object instance)
        {
            dao.Update(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete(object instance)
        {
            dao.Delete(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteById<T>(object id)
        {
            object instance = this.FindById<T>(id);
            dao.Delete(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete<T>(IList<T> instances)
        {
            if (instances != null && instances.Count > 0)
            {
                foreach (object inst in instances)
                {
                    dao.Delete(inst);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete(string hqlString)
        {
            dao.Delete(hqlString);
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete(string hqlString, object value, IType type)
        {
            dao.Delete(hqlString, value, type);
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete(string hqlString, object[] values, IType[] types)
        {
            dao.Delete(hqlString, values, types);
        }

        public void FlushSession()
        {
            this.dao.FlushSession();
        }

        public void CleanSession()
        {
            dao.CleanSession();
        }

        [Transaction(TransactionMode.Requires)]
        public T FindById<T>(object id)
        {
            return dao.FindById<T>(id);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>()
        {
            return dao.FindAll<T>();
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(string hql)
        {
            return dao.FindAllWithCustomQuery<T>(hql);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(string hql, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, firstRow, maxRows);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(string hql, IEnumerable<object> values)
        {
            if (values != null)
            {
                return dao.FindAllWithCustomQuery<T>(hql, values.ToArray());
            }
            else
            {
                return dao.FindAllWithCustomQuery<T>(hql);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(string hql, IEnumerable<object> values, int firstRow, int maxRows)
        {
            if (values != null)
            {
                return dao.FindAllWithCustomQuery<T>(hql, values.ToArray(), firstRow, maxRows);
            }
            else
            {
                return dao.FindAllWithCustomQuery<T>(hql, firstRow, maxRows);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(string hql, object value)
        {
            return dao.FindAllWithCustomQuery<T>(hql, value);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(string hql, object value, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, value, firstRow, maxRows);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(ICriterion[] criteria)
        {
            return dao.FindAll<T>(criteria);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(ICriterion[] criteria, int firstRow, int maxRows)
        {
            return dao.FindAll<T>(criteria, firstRow, maxRows);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAllIn<T>(string hql, IEnumerable<object> inParam, IEnumerable<object> param = null)
        {
            if (inParam == null || inParam.Count() == 0)
            {
                return null;
            }
            List<T> tList = new List<T>();

            int inParamCount = 1000;
            if (param != null)
            {
                inParamCount -= param.Count();
            }
            //把in转成or
            //from Item where Code in ( ? 
            hql = hql.TrimEnd(); //from Item Id in ( ?
            hql = hql.Remove(hql.Length - 1).TrimEnd();//删除"?"//from Item where Code in (
            hql = hql.Remove(hql.Length - 1).TrimEnd();//删除"("//from Item where Code in
            hql = hql.Remove(hql.Length - 2).TrimEnd();//删除"in"//from Item where Code
            string inField = hql.Split(' ').Last();//Code
            hql = hql.Remove(hql.Length - inField.Length);//from Item where 

            hql = string.Format("{0} ({1}=? ", hql, inField);
            //hqlStr.Append("(");//from Item where (
            //hqlStr.Append(inField);//from Item where (Code
            //hqlStr.Append("=? ");//from Item where (Code=? 

            int skipCount = 0;
            while (true)
            {
                var hqlStr = new StringBuilder(hql);
                var paramList = new List<object>();
                var batchinParam = inParam.Skip(skipCount).Take(inParamCount).ToList();
                if (batchinParam.Count() == 0)
                {
                    break;
                }
                skipCount += inParamCount;

                for (int i = 0; i < batchinParam.Count(); i++)
                {
                    if (i > 0)
                    {
                        hqlStr.Append("or ");//from Item where (Code=? or 
                        hqlStr.Append(inField);//from Item where (Code=? or Code
                        hqlStr.Append("=? ");//from Item where (Code=? or Code=
                    }
                }
                hqlStr.Append(")");

                if (param != null)
                {
                    paramList.AddRange(param);
                }
                paramList.AddRange(batchinParam);
                var newList = dao.FindAllWithCustomQuery<T>(hqlStr.ToString(), paramList.ToArray());
                if (newList != null)
                {
                    tList.AddRange(newList);
                }
            }
            return tList;
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAllWithNativeSql<T>(string sql)
        {
            return dao.FindAllWithNativeSql<T>(sql);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAllWithNativeSql<T>(string sql, object value, IType type = null)
        {
            return dao.FindAllWithNativeSql<T>(sql, new object[] { value }, new IType[] { type });
        }

        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAllWithNativeSql<T>(string sql, IEnumerable<object> values, IEnumerable<IType> types = null)
        {
            if (values == null)
            {
                return dao.FindAllWithNativeSql<T>(sql);
            }
            else
            {
                if (types == null)
                {
                    return dao.FindAllWithNativeSql<T>(sql, values.ToArray());
                }
                else
                {
                    return dao.FindAllWithNativeSql<T>(sql, values.ToArray(), types.ToArray());
                }
            }
        }
        [Transaction(TransactionMode.Requires)]
        public IList<T> FindAll<T>(string hqlString, IDictionary<string, object> param)
        {
            return this.dao.FindAllWithCustomQuery<T>(hqlString, param);
        }

    }
}
