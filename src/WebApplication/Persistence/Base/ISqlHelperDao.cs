using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;

namespace com.Sconit.Persistence
{
    public interface ISqlHelperDao
    {
        int Create(string sql, SqlParameter[] commandParameters);

        int Create(int dataBaseNo, string sqlString, SqlParameter[] commandParameters);

        int Update(string sql, SqlParameter[] commandParameters);

        int Update(int dataBaseNo, string sqlString, SqlParameter[] commandParameters);

        int Delete(string sql, SqlParameter[] commandParameters);

        int Delete(int dataBaseNo, string sqlString, SqlParameter[] commandParameters);

        int SearchCount(string sql, SqlParameter[] commandParameters);

        int ExecuteSql(string commandText, SqlParameter[] commandParameters);

        int ExecuteSql(int dataBaseNo, string commandText, SqlParameter[] commandParameters);

        int ExecuteStoredProcedure(string commandText, SqlParameter[] commandParameters);

        DataSet GetDatasetBySql(int dataBaseNo, string commandText, SqlParameter[] commandParameters);

        DataSet GetDatasetByStoredProcedure(int dataBaseNo, string commandText, SqlParameter[] commandParameters);

        DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters);

        DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters);

        void BulkInsert(DataTable dataTable);
    }
}
