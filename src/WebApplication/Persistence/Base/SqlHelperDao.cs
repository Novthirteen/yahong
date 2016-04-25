using System;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using NHibernate;

namespace com.Sconit.Persistence
{
    //this is the delegate for sql helper
    public class SqlHelperDao : ISqlHelperDao
    {
        public INHDao daoBase { get; set; }

        private String connectionString;
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        private String pLCConnStr;
        public string PLCConnStr
        {
            get { return pLCConnStr; }
            set { pLCConnStr = value; }
        }

        private String pLCConnStr2;
        public string PLCConnStr2
        {
            get { return pLCConnStr2; }
            set { pLCConnStr2 = value; }
        }

        public virtual int Delete(string sqlString, SqlParameter[] commandParameters)
        {
            return this.Delete(0, sqlString, commandParameters);
        }

        public virtual int Delete(int dataBaseNo, string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(dataBaseNo, sqlString, commandParameters);
        }

        public virtual int Create(string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(sqlString, commandParameters);
        }

        public virtual int Create(int dataBaseNo, string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(dataBaseNo, sqlString, commandParameters);
        }

        public virtual int Update(string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(sqlString, commandParameters);
        }

        public virtual int Update(int dataBaseNo, string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(dataBaseNo, sqlString, commandParameters);
        }

        public virtual int SearchCount(string sqlString, SqlParameter[] commandParameters)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlString, commandParameters));
        }

        public virtual int ExecuteSql(string commandText, SqlParameter[] commandParameters)
        {
            return this.ExecuteSql(0, commandText, commandParameters);
        }

        public virtual int ExecuteSql(int dataBaseNo, string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(dataBaseNo, CommandType.Text, commandText, commandParameters);
        }

        public virtual int ExecuteStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, commandText, commandParameters);
        }

        public virtual DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(0, CommandType.Text, commandText, commandParameters);
        }

        public virtual DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(0, CommandType.StoredProcedure, commandText, commandParameters);
        }

        public virtual DataSet GetDatasetBySql(int dataBaseNo, string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(dataBaseNo, CommandType.Text, commandText, commandParameters);
        }

        public virtual DataSet GetDatasetByStoredProcedure(int dataBaseNo, string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(dataBaseNo, CommandType.StoredProcedure, commandText, commandParameters);
        }

        private int ExecuteNonQuery(CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            return this.ExecuteNonQuery(0, commandType, commandText, commandParameters);
        }

        private int ExecuteNonQuery(int dataBaseNo, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            int executeRecord = 0;
            try
            {
                if (dataBaseNo == 0)
                    connection = new SqlConnection(ConnectionString);
                else if (dataBaseNo == 1)
                    connection = new SqlConnection(PLCConnStr);
                else if (dataBaseNo == 2)
                    connection = new SqlConnection(PLCConnStr2);
                connection.Open();

                //start a transaction
                transaction = connection.BeginTransaction();
                executeRecord += SqlHelper.ExecuteNonQuery(transaction, commandType, commandText, commandParameters);
                transaction.Commit();
                return executeRecord;
            }
            catch (SqlException ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        private DataSet GetDataset(int dataBaseNo, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            //commandText = commandText.Replace("\r\n", "");
            //using (ISession session = daoBase.GetSession())
            //{
            //    SqlConnection connection = (SqlConnection)session.Connection;

            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DataSet executeDataSet = new DataSet();
            try
            {
                if (dataBaseNo == 0)
                {
                    connection = new SqlConnection(ConnectionString);
                }
                else if (dataBaseNo == 1)
                    //connection = new SqlConnection(@"Data Source=10.20.60.80;Initial Catalog=yksjk;Persist Security Info=True;User ID=sa;Password=sa");
                    //connection = new SqlConnection(@"Data Source=10.20.70.250;Initial Catalog=YKSJK;Persist Security Info=True;User ID=sconit;Password=sconit");
                    connection = new SqlConnection(PLCConnStr);
                else if (dataBaseNo == 2)
                    connection = new SqlConnection(PLCConnStr2);
                connection.Open();

                //start a transaction
                transaction = connection.BeginTransaction();
                executeDataSet = SqlHelper.ExecuteDataset(transaction, commandType, commandText, commandParameters);
                transaction.Commit();
                return executeDataSet;
            }
            catch (SqlException ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
        //}
        public void BulkInsert(DataTable dataTable)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            SqlBulkCopy bulkCopy = null;

            try
            {
                connection = new SqlConnection(ConnectionString);
                bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.BatchSize = dataTable.Rows.Count;
                if (dataTable != null && dataTable.Columns.Count > 0)
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                }
                connection.Open();
                if (dataTable != null && dataTable.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dataTable);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                if (bulkCopy != null)
                {
                    bulkCopy.Close();
                }
            }
        }
    }
}
