using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWinFormLibrary.Data
{
    public class SqlDatabaseActions
    {
        private readonly SqlDatabaseConnectionConfig _config;
        public SqlDatabaseActions(SqlDatabaseConnectionConfig config)
        {
            _config = config;
        }

        public async Task<(bool Success, DataTable Data, string ErrorMessage)> GetDataAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString()))
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);

                        return (true, dataTable, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
        public async Task<(bool Success, DataRowCollection Rows, string ErrorMessage)> GetRowsAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                            command.Parameters.AddRange(parameters);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            return (true, dataTable.Rows, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
        public async Task<(bool Success, string Value, string ErrorMessage)> GetSingleValueAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString()))
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    object result = await command.ExecuteScalarAsync();
                    return (true, result?.ToString() ?? string.Empty, null);
                }
            }
            catch (Exception ex)
            {
                return (false, string.Empty, ex.Message);
            }
        }
        public async Task<(bool Success, bool HasData, string ErrorMessage)> HasDataAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString()))
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    var result = await command.ExecuteScalarAsync();
                    return (true, result != null, null);
                }
            }
            catch (Exception ex)
            {
                return (false, false, ex.Message);
            }
        }
        public async Task<(bool Success, bool HasData, T Data, string ErrorMessage)> HasDataAsync<T>(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString() ))
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    var result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        // Try to convert the result to type T
                        try
                        {
                            T convertedData = (T)Convert.ChangeType(result, typeof(T));
                            return (true, true, convertedData, null);
                        }
                        catch (InvalidCastException)
                        {
                            // If conversion fails, try direct cast
                            if (result is T directCast)
                            {
                                return (true, true, directCast, null);
                            }
                            else
                            {
                                return (true, false, default(T), "Data type conversion failed");
                            }
                        }
                    }
                    else
                    {
                        return (true, false, default(T), null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, false, default(T), ex.Message);
            }
        }
        public async Task<(bool Success, string MaxNumber, string ErrorMessage)> GetMaxNumberAsync(string tableName, string columnName)
        {
            try
            {
                // Validate input parameters
                if (string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(columnName))
                {
                    return (false, "1", "Table name and column name cannot be empty");
                }

                string query = $"SELECT ISNULL(MAX({columnName}), 0) + 1 FROM {tableName}";

                var (success, value, error) = await GetSingleValueAsync(query);

                if (success)
                {
                    if (string.IsNullOrEmpty(value) || value == "0")
                    {
                        return (true, "1", null);
                    }
                    else
                    {
                        return (true, value, null);
                    }
                }
                else
                {
                    return (false, "1", error);
                }
            }
            catch (Exception ex)
            {
                return (false, "1", ex.Message);
            }
        }
        public async Task<(bool Success, string ErrorMessage)> ExecuteCommandAsync(string query, SqlParameter[] parameters = null)
        {
            if (_config.SqlActionsLocked)
                return (false, "SQL actions are currently locked");

            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    using (var command = connection.CreateCommand())
                    {
                        try
                        {
                            command.Transaction = transaction;
                            command.CommandText = query;

                            if (parameters != null)
                                command.Parameters.AddRange(parameters);

                            await command.ExecuteNonQueryAsync();
                            transaction.Commit();

                            return (true, null);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return (false, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string ErrorMessage)> InsertDataAsync(string tableName, Dictionary<string, object> columnValues, bool hasEntryDate = true)
        {
            if (_config.SqlActionsLocked)
                return (false, "SQL actions are currently locked");

            try
            {
                // Build columns list
                var columns = new List<string>(columnValues.Keys);
                var paramNames = new List<string>(columnValues.Keys.Select(k => "@" + k));

                if (hasEntryDate)
                {
                    columns.AddRange(new[] { "e_date", "e_by" });
                    paramNames.AddRange(new[] { "GETDATE()", "@userId" });
                }

                string query = $"INSERT INTO {tableName} ({string.Join(",", columns)}) VALUES ({string.Join(",", paramNames)})";

                var parameterList = new List<SqlParameter>();
                foreach (var kvp in columnValues)
                {
                    parameterList.Add(new SqlParameter("@" + kvp.Key, kvp.Value ?? DBNull.Value));
                }

                if (hasEntryDate)
                {
                    parameterList.Add(new SqlParameter("@userId", _config.AuthUserId));
                }

                return await ExecuteCommandAsync(query, parameterList.ToArray());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool Success, string ErrorMessage)> UpdateDataAsync(string tableName, Dictionary<string, object> columnValues, Dictionary<string, object> whereClause, bool hasUpdateDate = true)
        {
            if (_config.SqlActionsLocked)
                return (false, "SQL actions are currently locked");

            try
            {
                // Build SET clause
                var setClauses = new List<string>();
                var parameterList = new List<SqlParameter>();

                foreach (var kvp in columnValues)
                {
                    setClauses.Add($"{kvp.Key} = @set_{kvp.Key}");
                    parameterList.Add(new SqlParameter("@set_" + kvp.Key, kvp.Value ?? DBNull.Value));
                }

                if (hasUpdateDate)
                {
                    setClauses.Add("u_date = GETDATE()");
                    setClauses.Add("u_by = @userId");
                    parameterList.Add(new SqlParameter("@userId", _config.AuthUserId));
                }

                // Build WHERE clause
                var whereClauses = new List<string>();
                foreach (var kvp in whereClause)
                {
                    whereClauses.Add($"{kvp.Key} = @where_{kvp.Key}");
                    parameterList.Add(new SqlParameter("@where_" + kvp.Key, kvp.Value ?? DBNull.Value));
                }

                string query = $"UPDATE {tableName} SET {string.Join(",", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";

                return await ExecuteCommandAsync(query, parameterList.ToArray());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool Success, string ErrorMessage)> DeleteDataAsync(string tableName, Dictionary<string, object> whereClause)
        {
            if (_config.SqlActionsLocked)
                return (false, "SQL actions are currently locked");

            try
            {
                // Build WHERE clause
                var whereClauses = new List<string>();
                var parameterList = new List<SqlParameter>();

                foreach (var kvp in whereClause)
                {
                    whereClauses.Add($"{kvp.Key} = @{kvp.Key}");
                    parameterList.Add(new SqlParameter("@" + kvp.Key, kvp.Value ?? DBNull.Value));
                }

                string query = $"DELETE FROM {tableName} WHERE {string.Join(" AND ", whereClauses)}";

                return await ExecuteCommandAsync(query, parameterList.ToArray());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool Success, int InsertedCount, List<string> Errors)> BulkInsertAsync(string tableName, List<Dictionary<string, object>> records, bool hasEntryDate = true)
        {
            if (_config.SqlActionsLocked)
                return (false, 0, new List<string> { "SQL actions are currently locked" });

            int insertedCount = 0;
            var errors = new List<string>();

            foreach (var record in records)
            {
                var (success, error) = await InsertDataAsync(tableName, record, hasEntryDate);
                if (success)
                {
                    insertedCount++;
                }
                else
                {
                    errors.Add(error);
                }
            }

            return (errors.Count == 0, insertedCount, errors);
        }
        public async Task<(bool Success, int ExecutedCount, string ErrorMessage)> ExecuteMultipleCommandsAsync(List<(string Query, SqlParameter[] Parameters)> commands)
        {
            if (_config.SqlActionsLocked)
                return (false, 0, "SQL actions are currently locked");

            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString()))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int executedCount = 0;

                            foreach (var (query, parameters) in commands)
                            {
                                using (var command = connection.CreateCommand())
                                {
                                    command.Transaction = transaction;
                                    command.CommandText = query;

                                    if (parameters != null)
                                        command.Parameters.AddRange(parameters);

                                    await command.ExecuteNonQueryAsync();
                                    executedCount++;
                                }
                            }

                            transaction.Commit();
                            return (true, executedCount, null);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return (false, 0, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, 0, ex.Message);
            }
        }
    }
}
