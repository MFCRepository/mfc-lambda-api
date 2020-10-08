using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace MyHttpGatewayApi
{
    public class Database
    {
        private const string _connStringTemplate = "Data Source={0}; Database={1};User ID={2}; Password={3}; Pooling=true; Min Pool Size=5; Max Pool Size=200; Persist Security Info=false;";
        private const string _defaultDataSource = "sql01.soundbilling.com";

        private static string ConnString { get; set; }
        public static Database_Names DatabaseNames;

        private string GetDatabaseNameFromEnvironment(string _environment,
            string _databaseName)
        {
            switch (_environment)
            {
                case "":
                case "api":
                case "staging":
                case "production":
                    /* use production database */
                    return _databaseName;

                default:
                    /* use sandbox database */
                    return string.Format("{0}_Sandbox", _databaseName);
            }
        }

        public struct Database_Names
        {
            public string SoundBilling { get => "SoundBilling"; }
            public string DocumentArchive { get => "DocumentArchive"; }
            public string CheckImages { get => "SoundBilling_CheckImages"; }
        }

        #region "conn string constructors"
        /// <summary>
        /// Initialize the connection string using the data source, database name, access type, username, and password
        /// </summary>
        /// <param name="_dataSource">Database URL</param>
        /// <param name="_databaseName">Name of database to use (user defined or use pre-defined DatabaseNames.[DatabaseName] value)</param>
        /// <param name="_username">Database username</param>
        /// <param name="_password">Database password</param>
        public Database(string _dataSource,
            string _databaseName,
            string _username,
            string _password)
        {
            ConnString = string.Format(_connStringTemplate, _dataSource, _databaseName, _username, _password);
        }

        /// <summary>
        /// Initialize the connection string using read-only (least privileged) account and default database based on _environment value
        /// This is a read-only connection and will be realistically used the majority of the time
        /// Uses the defaults for data source, database name, username and password for _access_type parameter
        /// Uses the _environment parameter to select the appropriate database (ie. production/sandbox)
        /// </summary>
        /// <param name="_environment">Deployment environment (ie. api, production, staging, development. Anything other than a blank string, api, production, or staging will default to the sandbox database)</param>
        public Database(string _environment)
        {
            var _databaseCredentials = Utilities.AmazonWebServices.SecretsManager.database_credentials.credentials.Find(x => x.permissions.Contains("read-only"));

            ConnString = string.Format(_connStringTemplate,
                    _defaultDataSource,
                    GetDatabaseNameFromEnvironment(_environment, DatabaseNames.SoundBilling),
                    _databaseCredentials.username,
                    _databaseCredentials.password
                );

        }

        /// <summary>
        /// Initialize the connection string using the access type and _environment values
        /// Use this to specify read-only or read-write permissions
        /// Uses the defaults for data source, database name, username and password for _access_type parameter
        /// Uses the _environment parameter to select the appropriate database (ie. production/sandbox)
        /// </summary>
        /// <param name="_environment">Deployment environment (ie. api, production, staging, development. Anything other than a blank string, api, production, or staging will default to the sandbox database)</param>
        /// <param name="_access_type">Type of DB access, acceptable values: r, r-w, read-only, read-write, read, write</param>
        public Database(string _environment,
            string _access_type)
        {
            var _databaseCredentials = Utilities.AmazonWebServices.SecretsManager.database_credentials.credentials.Find(x => x.permissions.Contains(_access_type));

            ConnString = string.Format(_connStringTemplate,
                    _defaultDataSource, 
                    GetDatabaseNameFromEnvironment(_environment, DatabaseNames.SoundBilling),
                    _databaseCredentials.username,
                    _databaseCredentials.password
                );

        }

        /// <summary>
        /// Initialize the connection string using the access type, _databaseName, and _environment parameters
        /// Uses the defaults for data source, username and password for _access_type parameter
        /// Uses the _environment parameter to select the appropriate database (ie. production/sandbox)
        /// </summary>
        /// <param name="_environment">Deployment environment (ie. api, production, staging, development. Anything other than a blank string, api, production, or staging will default to the sandbox database)</param>
        /// <param name="_access_type">Type of DB access, acceptable values: r, r-w, read-only, read-write, read, write</param>
        /// <param name="_databaseName">Name of database to use (user defined or use pre-defined DatabaseNames.[DatabaseName] value)</param>
        public Database(string _environment,
            string _access_type,
            string _databaseName)
        {
            var _databaseCredentials = Utilities.AmazonWebServices.SecretsManager.database_credentials.credentials.Find(x => x.permissions.Contains(_access_type));

            ConnString = string.Format(_connStringTemplate,
                    _defaultDataSource, 
                    GetDatabaseNameFromEnvironment(_environment, _databaseName), 
                    _databaseCredentials.username, 
                    _databaseCredentials.password
                );

        }
        #endregion

        public DataTable ExecuteSqlQuery(string _query,
            List<SqlParameter> _sqlParameters) 
        {
            DataTable _dt = new DataTable();

            try
            {
                using (SqlConnection cn = new SqlConnection(ConnString))
                {
                    using (SqlCommand cmd = new SqlCommand(_query, cn))
                    {
                        cmd.CommandTimeout = 6000;
                        cmd.CommandType = CommandType.Text;

                        foreach (SqlParameter sqlParameter in _sqlParameters)
                        {
                            cmd.Parameters.Add(sqlParameter);
                        }

                        cn.Open();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            _dt.Load(ds.CreateDataReader());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return _dt;
        }

        public DataTable ExecuteStoredProcedure(string _storedProcedureName,
            List<SqlParameter> _sqlParameters)
        {
            DataTable _dt = new DataTable();

            try
            {
                using (SqlConnection cn = new SqlConnection(ConnString))
                {
                    using (SqlCommand cmd = new SqlCommand(_storedProcedureName, cn))
                    {
                        cmd.CommandTimeout = 6000;
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (SqlParameter sqlParameter in _sqlParameters)
                        {
                            cmd.Parameters.Add(sqlParameter);
                        }

                        cn.Open();

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            _dt.Load( ds.CreateDataReader());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return _dt;
        }

        //public DataTableReader ExecuteSQLQuery(string _query,
        //        List<SqlParameter> _sqlParameters
        //    )
        //{
        //    /*
        //     * Usage:
        //     * string _SQL = "SELECT * FROM TABLE WHERE TABLE.COLUMN = @VALUE";
        //     * 
        //     * using (DataTable _dt = new DataTable())
        //        {
        //            using (DataTableReader _dr = new Classes.Utilities.Utilities().ExecuteSQLQuery(_SQL, new List<SqlParameter> {
        //                { new SqlParameter("@PARAMETER-NAME", PARAMETER-VALUE) },
        //                ETC...
        //            }))
        //            {
        //                //ALWAYS LOAD DATA TO DATA TABLE TO RELEASE CLOSE CONNECTION TO DATABASE
        //                _dt.Load(_dr);
        //            }
                    
        //            //DO WHATEVER WITH DATATABLE _dt
        //        }
        //     * 
        //     * 
        //     */

        //    try
        //    {
        //        using (SqlConnection cn = new SqlConnection(_connString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(_query, cn))
        //            {
        //                cmd.CommandTimeout = 6000;
        //                cmd.CommandType = CommandType.Text;

        //                foreach (SqlParameter sqlParameter in _sqlParameters)
        //                {
        //                    cmd.Parameters.Add(sqlParameter);
        //                }

        //                cn.Open();

        //                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //                {
        //                    DataSet ds = new DataSet();
        //                    da.Fill(ds);

        //                    return ds.CreateDataReader();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}

        //public DataTableReader ExecuteStoredProcedure(string _storedProcedureName,
        //    List<SqlParameter> _sqlParameters)
        //{
        //    /*
        //     * Usage:
        //     * 
        //     * using (DataTable _dt = new DataTable())
        //        {
        //            using (DataTableReader _dr = new Classes.Utilities.Utilities().ExecuteStoredProcedure("STORED-PROCEDURE-NAME", new List<SqlParameter> {
        //                { new SqlParameter("@PARAMETER-NAME", PARAMETER-VALUE) },
        //                ETC...
        //            }))
        //            {
        //                //ALWAYS LOAD DATA TO DATA TABLE TO RELEASE CLOSE CONNECTION TO DATABASE
        //                _dt.Load(_dr);
        //            }
                    
        //            //DO WHATEVER WITH DATATABLE _dt
        //        }
        //     * 
        //     * 
        //     */
        //    try
        //    {
        //        using (SqlConnection cn = new SqlConnection(_connString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(_storedProcedureName, cn))
        //            {
        //                cmd.CommandTimeout = 6000;
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                foreach (SqlParameter sqlParameter in _sqlParameters)
        //                {
        //                    cmd.Parameters.Add(sqlParameter);
        //                }

        //                cn.Open();

        //                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //                {
        //                    DataSet ds = new DataSet();
        //                    da.Fill(ds);

        //                    return ds.CreateDataReader();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //}
    }
}
