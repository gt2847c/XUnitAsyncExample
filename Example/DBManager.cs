using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace DB
{
    public interface IDBManager
    {
        DataTable ExecuteStoredProcedure(Dictionary<string, object> parameters, string sqlStatement);
        Task<DataTable> ExecuteStoredProcedureAsync(Dictionary<string, object> parameters, string sqlStatement);
        Task<object> ExecuteStoredProcedureScalarAsync(Dictionary<string, object> parameters, string sqlStatement);
        object ExecuteStoredProcedureScalar(Dictionary<string, object> parameters, string sqlStatement);
        DataTable ExecuteSelectStatement(string sqlStatement);
        object ExecuteSelectStatementScalar(string sqlStatement);
        Task<object> ExecuteSelectStatementScalarAsync(string sqlStatement);
        int UpdateTable(string sqlStatement, DataTable dataTable);
        Task<int> UpdateTableAsync(string sqlStatement, DataTable dataTable);
        Task<DataTable> ExecuteSelectStatementAsync(string sqlStatement);
        int ExecuteNonQuery(string sqlStatement);
        Task<int> ExecuteNonQueryAsync(string sqlStatement);

        int CommandTimeout { get; set; }
    }

    public class DBManager : IDBManager
    {
        private ILogger _log;
        private string _providerName;
        private string _connectionString;


        public int CommandTimeout { get; set; }


        public DBManager(ILogger log, string providerName, string connectionString)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log), "ILogger instance may not be null");
            try
            {
                if (string.IsNullOrEmpty(providerName))
                    throw new ArgumentNullException(nameof(providerName), "Provider may not be null or empty");
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException(nameof(connectionString),
                        "Connection string may not be null or empty");

                _providerName = providerName;
                _connectionString = connectionString;
                CommandTimeout = 300;
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw;
            }
        }

        public DataTable ExecuteStoredProcedure(Dictionary<string, object> parameters, string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);
            var result = new DataTable();

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = CommandTimeout;

                        foreach (var parameterName in parameters.Keys)
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = FixParameterName(parameterName);
                            parameters.TryGetValue(parameterName, out var value);
                            param.Value = value;
                            cmd.Parameters.Add(param);
                        }

                        var adapter = factory.CreateDataAdapter();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(result);
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        public async Task<DataTable> ExecuteStoredProcedureAsync(Dictionary<string, object> parameters,
            string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);
            var result = new DataTable();

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = CommandTimeout;

                        foreach (var parameterName in parameters.Keys)
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = FixParameterName(parameterName);
                            parameters.TryGetValue(parameterName, out var value);
                            param.Value = value;
                            cmd.Parameters.Add(param);
                        }

                        var adapter = factory.CreateDataAdapter();
                        adapter.SelectCommand = cmd;
                        await Task.Run(() => adapter.Fill(result));
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        public object ExecuteStoredProcedureScalar(Dictionary<string, object> parameters, string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = CommandTimeout;

                        foreach (var parameterName in parameters.Keys)
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = FixParameterName(parameterName);
                            parameters.TryGetValue(parameterName, out var value);
                            param.Value = value;
                            cmd.Parameters.Add(param);
                        }

                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        public async Task<object> ExecuteStoredProcedureScalarAsync(Dictionary<string, object> parameters,
            string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = CommandTimeout;

                        foreach (var parameterName in parameters.Keys)
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = FixParameterName(parameterName);
                            parameters.TryGetValue(parameterName, out var value);
                            param.Value = value;
                            cmd.Parameters.Add(param);
                        }

                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }


        public DataTable ExecuteSelectStatement(string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);
            var result = new DataTable();

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        var adapter = factory.CreateDataAdapter();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(result);
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        public async Task<DataTable> ExecuteSelectStatementAsync(string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);
            var result = new DataTable();

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        var adapter = factory.CreateDataAdapter();
                        adapter.SelectCommand = cmd;
                        await Task.Run(()=>adapter.Fill(result));
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }


        public object ExecuteSelectStatementScalar(string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        public async Task<object> ExecuteSelectStatementScalarAsync(string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }

            throw new NotImplementedException();
        }

        public int UpdateTable(string sqlStatement, DataTable dataTable)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        var adapter = factory.CreateDataAdapter();
                        adapter.SelectCommand = cmd;
                        var builder = factory.CreateCommandBuilder();
                        builder.DataAdapter = adapter;
                        adapter.UpdateCommand = builder.GetUpdateCommand();
                        adapter.InsertCommand = builder.GetInsertCommand();
                        return adapter.Update(dataTable);
                    }
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        public async Task<int> UpdateTableAsync(string sqlStatement, DataTable dataTable)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        var adapter = factory.CreateDataAdapter();
                        adapter.SelectCommand = cmd;
                        var builder = factory.CreateCommandBuilder();
                        builder.DataAdapter = adapter;
                        adapter.UpdateCommand = builder.GetUpdateCommand();
                        adapter.InsertCommand = builder.GetInsertCommand();
                        var result = await Task.Run(() => adapter.Update(dataTable));
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        public int ExecuteNonQuery(string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);
            var result = new DataTable();

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        return cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }


        public async Task<int> ExecuteNonQueryAsync(string sqlStatement)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);
            var result = new DataTable();

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlStatement;
                        cmd.CommandTimeout = CommandTimeout;
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                var methodInfo = System.Reflection.MethodBase.GetCurrentMethod();
                var fullName = methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "()";
                var message = $"{fullName} - Exception thrown";
                _log.Error(message);
                _log.Error(e.Message);
                throw new ApplicationException(message,e);
            }
        }

        private string FixParameterName(string parameterName)
        {
            var factory = DbProviderFactories.GetFactory(_providerName);
            switch (factory.GetType().Name)
            {
                case "SqlClientFactory":
                    return "@" + parameterName;
                case "OracleClientFactory":
                    return ":" + parameterName;
                case "OleDbFactory":
                case "OdbcFactory":
                    return "?" + parameterName;
                default:
                    return parameterName;
            }
        }
    }
}