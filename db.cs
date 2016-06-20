//
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
//
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
//
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
//
using Npgsql;
//
namespace db
{
    public class db
    {
        //
        public enum dataBaseType
        {
            undef,
            access,
            sqlServer,
            mySql,
            postgre,
        }
        //
        private String _connectionString = "";
        private db.dataBaseType _curDbType;
        private Boolean configInit;
        public String _dbType;
        //
        public string connectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                
                this._connectionString = value;
            }
        }
        //
        public db.dataBaseType curDbType
        {
            get
            {
                return this._curDbType;
            }
            set
            {
                this._curDbType = value;
            }
        }
        IDbConnection myConnection = null;
        //
        public void forceConfig(String dbType, String connStr)
        {
            if (myConnection != null) { myConnection.Dispose(); }
            this._dbType = dbType;
            this._connectionString = connStr;
            configInit = true;
        }
        public IDbConnection getConnection()
        {
            
                if (!this.configInit)
                {
                    NpgsqlEventLog.LogName = @"c:\_ekoal\LogFile.txt";
                    this.configInit = true;
                    this._dbType = ConfigurationManager.AppSettings.Get("db.type");
                    this._connectionString = ConfigurationManager.AppSettings.Get("db.connectionString");
                }
                switch (this._dbType)
                {
                    case "postgre":
                        myConnection = new NpgsqlConnection();
                        this._curDbType = db.dataBaseType.postgre;
                        break;
                    case "access":
                        this._curDbType = db.dataBaseType.access;
                        break;
                    case "sqlServer":
                        this._curDbType = db.dataBaseType.sqlServer;
                        break;
                    case "mySql":
                        myConnection = new MySqlConnection();
                        this._curDbType = db.dataBaseType.mySql;
                        break;
                    default:
                        this._curDbType = db.dataBaseType.undef;
                        break;
                }
                myConnection.ConnectionString = this._connectionString;
                myConnection.Open();
            
            return myConnection;
        }
        //
        public IDataReader getReader(String query, IDbConnection iConn)
        {
            IDbCommand command = iConn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            return command.ExecuteReader();
        }
        //
        public Object getScalar(String query, IDbConnection iConn)
        {
            IDbCommand command = iConn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            return command.ExecuteScalar();
        }
        //
        public Int32 getIdentity(IDbConnection iConn)
        {
            return (getIdentity(iConn, ""));
        }
        public Int32 getIdentity(IDbConnection iConn, String tableName)
        {
            IDbCommand command = iConn.CreateCommand();
            command.CommandType = CommandType.Text;
            switch (this._dbType)
            {
                case "postgre":
                    command.CommandText = "select currval('"+tableName+"');";
                    break;
                /*case "access":
                    this._curDbType = db.dataBaseType.access;
                    break;*/
                /*case "sqlServer":
                    this._curDbType = db.dataBaseType.sqlServer;
                    break;*/
                case "mySql":
                    command.CommandText = "SELECT @@IDENTITY AS 'Identity';";
                    break;
            }
            
            return Convert.ToInt32(command.ExecuteScalar());
        }
        //
        public IDbCommand getCommand(String query)
        {
            return getCommand(query, this.getConnection());
        }
        public IDbCommand getCommand(String query, IDbConnection iConn)
        {
            switch (this._dbType)
            {
                case "postgre":
                    return new NpgsqlCommand(query, (NpgsqlConnection)iConn);
                /*case "access":
                    this._curDbType = db.dataBaseType.access;
                    break;*/
                /*case "sqlServer":
                    this._curDbType = db.dataBaseType.sqlServer;
                    break;*/
                case "mySql":
                    return new MySqlCommand(query, (MySqlConnection)iConn);
                default:
                    return null;
            }
        }
        public Boolean hasRow(IDataReader idr)
        {
            switch (this._dbType)
            {
                case "postgre":
                    return ((NpgsqlDataReader)idr).HasRows;
                /*case "access":
                    this._curDbType = db.dataBaseType.access;
                    break;*/
                /*case "sqlServer":
                    this._curDbType = db.dataBaseType.sqlServer;
                    break;*/
                case "mySql":
                    return ((MySqlDataReader)idr).HasRows;
                default:
                    return false;
            }
        }
        //
        public String getParameterName(String paramName)
        {
            switch (this._dbType)
            {
                case "postgre":
                    return ":" + paramName;
                /*case "access":
                    this._curDbType = db.dataBaseType.access;
                    break;*/
                /*case "sqlServer":
                    this._curDbType = db.dataBaseType.sqlServer;
                    break;*/
                case "mySql":
                    return "?" + paramName;
                default:
                    return "?" + paramName;
            }
        }
        //
        public IDbDataParameter getParameter(DbType dbT, String paramName, Object value)
        {
            switch (this._dbType)
            {
                case "postgre":
                    NpgsqlParameter p = new NpgsqlParameter(paramName,dbT);
                    p.Value = value;
                    return p;
                /*case "access":
                    this._curDbType = db.dataBaseType.access;
                    break;*/
                /*case "sqlServer":
                    this._curDbType = db.dataBaseType.sqlServer;
                    break;*/
                case "mySql":
                    MySqlDbType mysDbType = new MySqlDbType();
                    switch (dbT) 
                    {
                        case DbType.String:
                            mysDbType = MySqlDbType.String;
                            break;
                        case DbType.Int32:
                            mysDbType = MySqlDbType.Int32;
                            break;
                        case DbType.Double:
                            mysDbType = MySqlDbType.Double;
                            break;
                        case DbType.DateTime:
                            mysDbType = MySqlDbType.DateTime;
                            break;
                    }
                    MySqlParameter p2 = new MySqlParameter(paramName, mysDbType);
                    p2.Value = value;
                    return p2;
                default:
                    return null;
            }
        }
        //
    }
}
