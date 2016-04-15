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

        public virtual int Delete(string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(sqlString, commandParameters);
        }

        public virtual int Create(string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(sqlString, commandParameters);
        }

        public virtual int Update(string sqlString, SqlParameter[] commandParameters)
        {
            return ExecuteSql(sqlString, commandParameters);
        }

        public virtual int SearchCount(string sqlString, SqlParameter[] commandParameters)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sqlString, commandParameters));
        }

        public virtual int ExecuteSql(string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, commandParameters);
        }

        public virtual int ExecuteStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, commandText, commandParameters);
        }

        public virtual DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(CommandType.Text, commandText, commandParameters);
        }

        public virtual DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(CommandType.StoredProcedure, commandText, commandParameters);
        }

        private int ExecuteNonQuery(CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            int executeRecord = 0;
            try
            {
                connection = new SqlConnection(ConnectionString);
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

        private DataSet GetDataset(CommandType commandType, string commandText, SqlParameter[] commandParameters)
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
                    connection = new SqlConnection(ConnectionString);
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
    }
}
