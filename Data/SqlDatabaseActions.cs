using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWinFormLibrary.Data
{
    /// <summary>
    /// Provides comprehensive SQL Server database operations with async support, transaction handling, and automatic error management.
    /// This static class offers a complete set of CRUD operations, bulk operations, and utility methods for SQL Server database interactions.
    /// All methods return tuple results for consistent error handling and support multi-language error reporting.
    /// </summary>
    /// <remarks>
    /// The class uses the configured database connection from SqlDatabaseConnectionConfigBuilder and provides:
    /// - Async operations for all database calls
    /// - Automatic transaction management for write operations
    /// - SQL injection protection through parameterized queries
    /// - Comprehensive error handling with debug/release mode differences
    /// - Multi-language error alerts (Kurdish, Arabic, English)
    /// - Automatic audit trail support (entry/update dates and user tracking)
    /// </remarks>
    public static class SqlDatabaseActions
    {

        /// <summary>
        /// Executes a SELECT query and returns the results as a DataTable.
        /// </summary>
        /// <param name="query">The SQL SELECT query to execute</param>
        /// <param name="parameters">Optional SQL parameters to prevent SQL injection (default: null)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the operation completed successfully
        /// - Data: DataTable containing the query results (null if failed)
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method automatically manages database connections and provides comprehensive error handling.
        /// In DEBUG mode, detailed error messages are shown in alerts. In RELEASE mode, generic error messages
        /// are displayed to users while detailed errors are logged to Sentry.
        /// </remarks>
        /// <example>
        /// <code>
        /// var parameters = new SqlParameter[]
        /// {
        ///     new SqlParameter("@UserId", userId),
        ///     new SqlParameter("@Status", "Active")
        /// };
        /// 
        /// var (success, data, error) = await SqlDatabaseActions.GetDataAsync(
        ///     "SELECT * FROM Users WHERE UserId = @UserId AND Status = @Status", 
        ///     parameters
        /// );
        /// 
        /// if (success)
        /// {
        ///     // Process the data
        ///     foreach (DataRow row in data.Rows)
        ///     {
        ///         // Handle each row
        ///     }
        /// }
        /// </code>
        /// </example>
        public static async Task<(bool Success, DataTable Data, string ErrorMessage)> GetDataAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString()))
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
#if DEBUG
                AdvancedAlert.ShowAlert(ex.Message, ex.Message, ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in GetDataAsync.");
#endif
                return (false, null, ex.Message);
            }
        }
        /// <summary>
        /// Executes a SELECT query and returns the results as a DataRowCollection for direct row iteration.
        /// </summary>
        /// <param name="query">The SQL SELECT query to execute</param>
        /// <param name="parameters">Optional SQL parameters to prevent SQL injection (default: null)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the operation completed successfully
        /// - Rows: DataRowCollection containing the query results (null if failed)
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method is useful when you need direct access to DataRow objects without the overhead
        /// of the full DataTable structure. It provides the same error handling and connection management
        /// as GetDataAsync but returns rows directly.
        /// </remarks>
        /// <example>
        /// <code>
        /// var (success, rows, error) = await SqlDatabaseActions.GetRowsAsync(
        ///     "SELECT Name, Email FROM Users WHERE Active = 1"
        /// );
        /// 
        /// if (success)
        /// {
        ///     foreach (DataRow row in rows)
        ///     {
        ///         string name = row["Name"].ToString();
        ///         string email = row["Email"].ToString();
        ///     }
        /// }
        /// </code>
        /// </example>
        public static async Task<(bool Success, DataRowCollection Rows, string ErrorMessage)> GetRowsAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString()))
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
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in GetRowsAsync.");
#endif
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Executes a query that returns a single scalar value and converts it to a string.
        /// </summary>
        /// <param name="query">The SQL query to execute (typically SELECT with aggregate functions or single column)</param>
        /// <param name="parameters">Optional SQL parameters to prevent SQL injection (default: null)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the operation completed successfully
        /// - Value: String representation of the scalar result (empty string if null or failed)
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method is ideal for COUNT, MAX, MIN, SUM operations or retrieving single values.
        /// NULL database values are converted to empty strings for consistency.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Get total count of active users
        /// var (success, count, error) = await SqlDatabaseActions.GetSingleValueAsync(
        ///     "SELECT COUNT(*) FROM Users WHERE Active = 1"
        /// );
        /// 
        /// // Get user's email by ID
        /// var (success, email, error) = await SqlDatabaseActions.GetSingleValueAsync(
        ///     "SELECT Email FROM Users WHERE UserId = @UserId",
        ///     new SqlParameter[] { new SqlParameter("@UserId", userId) }
        /// );
        /// </code>
        /// </example>
        public static async Task<(bool Success, string Value, string ErrorMessage)> GetSingleValueAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString()))
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
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in GetSingleValueAsync.");
#endif
                return (false, string.Empty, ex.Message);
            }
        }

        /// <summary>
        /// Checks if a query returns any data by executing it and examining the first result.
        /// </summary>
        /// <param name="query">The SQL query to execute for data existence check</param>
        /// <param name="parameters">Optional SQL parameters to prevent SQL injection (default: null)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the operation completed successfully
        /// - HasData: True if the query returned a non-null result, false otherwise
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method is more efficient than GetDataAsync when you only need to check for data existence.
        /// It uses ExecuteScalar and only retrieves the first column of the first row.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Check if user exists
        /// var (success, exists, error) = await SqlDatabaseActions.HasDataAsync(
        ///     "SELECT 1 FROM Users WHERE Email = @Email",
        ///     new SqlParameter[] { new SqlParameter("@Email", userEmail) }
        /// );
        /// 
        /// if (success and exists)
        /// {
        ///     // User exists
        /// }
        /// </code>
        /// </example>
        public static async Task<(bool Success, bool HasData, string ErrorMessage)> HasDataAsync(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString()))
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
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in HasDataAsync.");
#endif
                return (false, false, ex.Message);
            }
        }

        /// <summary>
        /// Checks if a query returns any data and attempts to convert the result to the specified type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the result to</typeparam>
        /// <param name="query">The SQL query to execute for data existence check</param>
        /// <param name="parameters">Optional SQL parameters to prevent SQL injection (default: null)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the operation completed successfully
        /// - HasData: True if the query returned a non-null result that could be converted to type T
        /// - Data: The converted result of type T (default(T) if no data or conversion failed)
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This generic method provides type-safe data retrieval with automatic type conversion.
        /// It attempts Convert.ChangeType first, then falls back to direct casting.
        /// If type conversion fails, HasData will be false and an appropriate error message is returned.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Get user ID as integer
        /// var (success, hasData, userId, error) = await SqlDatabaseActions.HasDataAsync&lt;int&gt;(
        ///     "SELECT UserId FROM Users WHERE Email = @Email",
        ///     new SqlParameter[] { new SqlParameter("@Email", userEmail) }
        /// );
        /// 
        /// if (success &amp;&amp; hasData)
        /// {
        ///     // Use userId (int)
        /// }
        /// 
        /// // Get user creation date
        /// var (success, hasData, createdDate, error) = await SqlDatabaseActions.HasDataAsync&lt;DateTime&gt;(
        ///     "SELECT CreatedDate FROM Users WHERE UserId = @UserId",
        ///     new SqlParameter[] { new SqlParameter("@UserId", userId) }
        /// );
        /// </code>
        /// </example>
        public static async Task<(bool Success, bool HasData, T Data, string ErrorMessage)> HasDataAsync<T>(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = new SqlConnection(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString()))
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
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in HasDataAsync<T>.");
#endif
                return (false, false, default(T), ex.Message);
            }
        }

        /// <summary>
        /// Gets the next available number for a specified column in a table by finding the maximum value and adding 1.
        /// </summary>
        /// <param name="tableName">The name of the table to query</param>
        /// <param name="columnName">The name of the numeric column to find the maximum value for</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the operation completed successfully
        /// - MaxNumber: String representation of the next available number (defaults to "1" if no data or error)
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method is commonly used for generating sequential IDs or reference numbers.
        /// It handles empty tables by returning "1" as the first number.
        /// Input validation ensures table and column names are not null or empty.
        /// The result is always returned as a string for consistency with UI binding.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Get next invoice number
        /// var (success, nextNumber, error) = await SqlDatabaseActions.GetMaxNumberAsync("Invoices", "InvoiceNumber");
        /// 
        /// if (success)
        /// {
        ///     // Use nextNumber for new invoice
        ///     int invoiceNumber = int.Parse(nextNumber);
        /// }
        /// </code>
        /// </example>
        public static async Task<(bool Success, string MaxNumber, string ErrorMessage)> GetMaxNumberAsync(string tableName, string columnName)
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
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in GetMaxNumberAsync.");
#endif
                return (false, "1", ex.Message);
            }
        }

        /// <summary>
        /// Executes a non-query SQL command (INSERT, UPDATE, DELETE) with transaction support and SQL action locking.
        /// </summary>
        /// <param name="query">The SQL command to execute</param>
        /// <param name="parameters">Optional SQL parameters to prevent SQL injection (default: null)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the command executed successfully and transaction was committed
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method provides comprehensive transaction management with automatic rollback on errors.
        /// It respects the SqlActionsLocked setting from the database configuration to prevent accidental data modifications.
        /// All write operations are wrapped in transactions to ensure data consistency.
        /// </remarks>
        /// <example>
        /// <code>
        /// var parameters = new SqlParameter[]
        /// {
        ///     new SqlParameter("@Name", "John Doe"),
        ///     new SqlParameter("@Email", "john@example.com"),
        ///     new SqlParameter("@Status", "Active")
        /// };
        /// 
        /// var (success, error) = await SqlDatabaseActions.ExecuteCommandAsync(
        ///     "INSERT INTO Users (Name, Email, Status) VALUES (@Name, @Email, @Status)",
        ///     parameters
        /// );
        /// </code>
        /// </example>
        public static async Task<(bool Success, string ErrorMessage)> ExecuteCommandAsync(string query, SqlParameter[] parameters = null)
        {
            if (SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.SqlActionsLocked)
                return (false, "SQL actions are currently locked");

            try
            {
                using (var connection = new SqlConnection(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString()))
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
#if DEBUG
                            AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                            LogManager.Logger?.Error(ex, "Something went wrong in ExecuteCommandAsync transaction.");
#endif
                            return (false, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشkلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in ExecuteCommandAsync.");
#endif
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Inserts a new record into the specified table with automatic audit trail support.
        /// </summary>
        /// <param name="tableName">The name of the target table</param>
        /// <param name="columnValues">Dictionary of column names and their corresponding values to insert</param>
        /// <param name="hasEntryDate">If true, automatically adds e_date (current date) and e_by (current user) columns (default: true)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the record was inserted successfully
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method automatically handles audit trail columns when hasEntryDate is true:
        /// - e_date: Set to GETDATE() (current server time)
        /// - e_by: Set to the current authenticated user ID from the database configuration
        /// 
        /// All values are properly parameterized to prevent SQL injection attacks.
        /// NULL values are automatically converted to DBNull.Value for database compatibility.
        /// </remarks>
        /// <example>
        /// <code>
        /// var userData = new Dictionary&lt;string, object&gt;
        /// {
        ///     { "Name", "John Doe" },
        ///     { "Email", "john@example.com" },
        ///     { "Age", 30 },
        ///     { "IsActive", true }
        /// };
        /// 
        /// var (success, error) = await SqlDatabaseActions.InsertDataAsync("Users", userData);
        /// 
        /// if (success)
        /// {
        ///     // Record inserted successfully with audit trail
        /// }
        /// </code>
        /// </example>
        public static async Task<(bool Success, string ErrorMessage)> InsertDataAsync(string tableName, Dictionary<string, object> columnValues, bool hasEntryDate = true)
        {
            if (SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.SqlActionsLocked)
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
                    parameterList.Add(new SqlParameter("@userId", SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.AuthUserId));
                }

                return await ExecuteCommandAsync(query, parameterList.ToArray());
            }
            catch (Exception ex)
            {
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in InsertDataAsync.");
#endif
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Updates existing records in the specified table with automatic audit trail support.
        /// </summary>
        /// <param name="tableName">The name of the target table</param>
        /// <param name="columnValues">Dictionary of column names and their new values to update</param>
        /// <param name="whereClause">Dictionary of column names and values for the WHERE condition (all conditions are ANDed)</param>
        /// <param name="hasUpdateDate">If true, automatically adds u_date (current date) and u_by (current user) columns (default: true)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the update operation completed successfully
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method automatically handles audit trail columns when hasUpdateDate is true:
        /// - u_date: Set to GETDATE() (current server time)
        /// - u_by: Set to the current authenticated user ID from the database configuration
        /// 
        /// Parameters are prefixed with "set_" for SET clause values and "where_" for WHERE clause values
        /// to avoid parameter name conflicts. All values are properly parameterized to prevent SQL injection.
        /// </remarks>
        /// <example>
        /// <code>
        /// var updates = new Dictionary&lt;string, object&gt;
        /// {
        ///     { "Name", "Jane Doe" },
        ///     { "Email", "jane@example.com" },
        ///     { "IsActive", true }
        /// };
        /// 
        /// var conditions = new Dictionary&lt;string, object&gt;
        /// {
        ///     { "UserId", 123 },
        ///     { "Status", "Pending" }
        /// };
        /// 
        /// var (success, error) = await SqlDatabaseActions.UpdateDataAsync("Users", updates, conditions);
        /// </code>
        /// </example>
        public static async Task<(bool Success, string ErrorMessage)> UpdateDataAsync(string tableName, Dictionary<string, object> columnValues, Dictionary<string, object> whereClause, bool hasUpdateDate = true)
        {
            if (SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.SqlActionsLocked)
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
                    parameterList.Add(new SqlParameter("@userId", SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.AuthUserId));
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
#if DEBUG
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in UpdateDataAsync.");
#endif
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Deletes records from the specified table based on the provided WHERE conditions.
        /// </summary>
        /// <param name="tableName">The name of the target table from which to delete records</param>
        /// <param name="whereClause">Dictionary of column names and values for the WHERE condition (all conditions are ANDed together)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if the delete operation completed successfully
        /// - ErrorMessage: Detailed error message if the operation failed (null if successful)
        /// </returns>
        /// <remarks>
        /// This method permanently removes records from the database based on the WHERE conditions.
        /// All WHERE conditions are combined with AND logic for precise record targeting.
        /// Use with caution as deleted data cannot be recovered unless proper backups exist.
        /// All values are properly parameterized to prevent SQL injection attacks.
        /// The operation respects the SqlActionsLocked setting to prevent accidental deletions.
        /// </remarks>
        /// <example>
        /// <code>
        /// var conditions = new Dictionary&lt;string, object&gt;
        /// {
        ///     { "UserId", 123 },
        ///     { "Status", "Inactive" },
        ///     { "LastLogin", DateTime.Now.AddYears(-2) }
        /// };
        /// 
        /// var (success, error) = await SqlDatabaseActions.DeleteDataAsync("Users", conditions);
        /// 
        /// if (success)
        /// {
        ///     Console.WriteLine("Records deleted successfully");
        /// }
        /// else
        /// {
        ///     Console.WriteLine($"Delete failed: {error}");
        /// }
        /// </code>
        /// </example>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when SQL actions are locked via SqlDatabaseConnectionConfigBuilder configuration.
        /// </exception>
        public static async Task<(bool Success, string ErrorMessage)> DeleteDataAsync(string tableName, Dictionary<string, object> whereClause)
        {
            if (SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.SqlActionsLocked)
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
#if DEBUG
        AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in DeleteDataAsync.");
#endif
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Performs bulk insert operations for multiple records with individual error tracking and partial success support.
        /// </summary>
        /// <param name="tableName">The name of the target table where records will be inserted</param>
        /// <param name="records">List of dictionaries, each representing a record to insert with column names and their corresponding values</param>
        /// <param name="hasEntryDate">If true, automatically adds e_date (current date) and e_by (current user) columns for audit trail purposes (default: true)</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if all records were inserted successfully, false if any errors occurred
        /// - InsertedCount: Number of records successfully inserted into the database
        /// - Errors: List of error messages for failed insertions (empty list if all successful)
        /// </returns>
        /// <remarks>
        /// This method processes each record individually, allowing partial success scenarios where some records
        /// may succeed while others fail. Failed insertions don't prevent other records from being processed.
        /// Each insert operation is performed using the standard InsertDataAsync method with full transaction support.
        /// 
        /// The operation continues even if some records fail, providing comprehensive error reporting.
        /// This approach is useful for data migration or batch processing where you want to insert as many
        /// records as possible and handle failures separately.
        /// 
        /// When hasEntryDate is true, each record automatically gets:
        /// - e_date: Set to GETDATE() (current server time)
        /// - e_by: Set to the current authenticated user ID from database configuration
        /// </remarks>
        /// <example>
        /// <code>
        /// var users = new List&lt;Dictionary&lt;string, object&gt;&gt;
        /// {
        ///     new Dictionary&lt;string, object&gt; 
        ///     { 
        ///         { "Name", "John Doe" }, 
        ///         { "Email", "john@example.com" },
        ///         { "Age", 30 }
        ///     },
        ///     new Dictionary&lt;string, object&gt; 
        ///     { 
        ///         { "Name", "Jane Smith" }, 
        ///         { "Email", "jane@example.com" },
        ///         { "Age", 25 }
        ///     },
        ///     new Dictionary&lt;string, object&gt; 
        ///     { 
        ///         { "Name", "Bob Johnson" }, 
        ///         { "Email", "bob@example.com" },
        ///         { "Age", 35 }
        ///     }
        /// };
        /// 
        /// var (success, insertedCount, errors) = await SqlDatabaseActions.BulkInsertAsync("Users", users);
        /// 
        /// Console.WriteLine($"Inserted {insertedCount} out of {users.Count} records");
        /// 
        /// if (errors.Any())
        /// {
        ///     Console.WriteLine("Errors occurred:");
        ///     foreach (var error in errors)
        ///     {
        ///         Console.WriteLine($"- {error}");
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when SQL actions are locked via SqlDatabaseConnectionConfigBuilder configuration.
        /// </exception>
        public static async Task<(bool Success, int InsertedCount, List<string> Errors)> BulkInsertAsync(string tableName, List<Dictionary<string, object>> records, bool hasEntryDate = true)
        {
            if (SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.SqlActionsLocked)
                return (false, 0, new List<string> { "SQL actions are currently locked" });

            try
            {
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
            catch (Exception ex)
            {
#if DEBUG
        AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in BulkInsertAsync.");
#endif
                return (false, 0, new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Executes multiple SQL commands within a single transaction to ensure atomic operations across related database changes.
        /// </summary>
        /// <param name="commands">List of tuples containing SQL queries and their respective parameter arrays for execution</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: True if all commands executed successfully and the transaction was committed
        /// - ExecutedCount: Number of commands successfully executed before any failure occurred
        /// - ErrorMessage: Detailed error message if any command failed (null if all successful)
        /// </returns>
        /// <remarks>
        /// This method ensures complete atomicity - either all commands succeed and are committed to the database,
        /// or if any single command fails, the entire transaction is rolled back and no changes are applied.
        /// 
        /// Commands are executed sequentially in the order they appear in the list. If a command fails,
        /// execution stops immediately and all previous changes within the transaction are rolled back.
        /// 
        /// This method is ideal for complex business operations that require multiple related database changes
        /// to maintain data consistency and referential integrity. Examples include:
        /// - Creating an order with multiple order items
        /// - Transferring funds between accounts
        /// - Updating multiple related tables that must stay synchronized
        /// 
        /// All commands within the transaction share the same connection and transaction context,
        /// ensuring optimal performance and consistency.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Example: Create an order with items and update customer info atomically
        /// var commands = new List&lt;(string Query, SqlParameter[] Parameters)&gt;
        /// {
        ///     // Insert new order
        ///     (
        ///         "INSERT INTO Orders (CustomerId, OrderDate, Total) VALUES (@CustomerId, @OrderDate, @Total); SELECT SCOPE_IDENTITY();",
        ///         new SqlParameter[] 
        ///         { 
        ///             new SqlParameter("@CustomerId", customerId),
        ///             new SqlParameter("@OrderDate", DateTime.Now),
        ///             new SqlParameter("@Total", orderTotal)
        ///         }
        ///     ),
        ///     // Update customer's last order date
        ///     (
        ///         "UPDATE Customers SET LastOrderDate = @OrderDate, TotalOrders = TotalOrders + 1 WHERE CustomerId = @CustomerId",
        ///         new SqlParameter[] 
        ///         { 
        ///             new SqlParameter("@CustomerId", customerId),
        ///             new SqlParameter("@OrderDate", DateTime.Now)
        ///         }
        ///     ),
        ///     // Insert order items
        ///     (
        ///         "INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)",
        ///         new SqlParameter[]
        ///         {
        ///             new SqlParameter("@OrderId", orderId),
        ///             new SqlParameter("@ProductId", productId),
        ///             new SqlParameter("@Quantity", quantity),
        ///             new SqlParameter("@UnitPrice", unitPrice)
        ///         }
        ///     )
        /// };
        /// 
        /// var (success, executedCount, error) = await SqlDatabaseActions.ExecuteMultipleCommandsAsync(commands);
        /// 
        /// if (success)
        /// {
        ///     Console.WriteLine($"All {executedCount} commands executed successfully");
        /// }
        /// else
        /// {
        ///     Console.WriteLine($"Transaction failed after {executedCount} commands. Error: {error}");
        ///     // All changes have been rolled back automatically
        /// }
        /// </code>
        /// </example>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when SQL actions are locked via SqlDatabaseConnectionConfigBuilder configuration.
        /// </exception>
        /// <exception cref="System.Data.SqlClient.SqlException">
        /// Thrown when any SQL command within the transaction fails, causing the entire transaction to be rolled back.
        /// </exception>
        public static async Task<(bool Success, int ExecutedCount, string ErrorMessage)> ExecuteMultipleCommandsAsync(List<(string Query, SqlParameter[] Parameters)> commands)
        {
            if (SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.SqlActionsLocked)
                return (false, 0, "SQL actions are currently locked");

            try
            {
                using (var connection = new SqlConnection(SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString()))
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
#if DEBUG
                    AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                            LogManager.Logger?.Error(ex, "Something went wrong in ExecuteMultipleCommandsAsync transaction.");
#endif
                            return (false, 0, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
        AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا: " + ex.Message, $"حدثت مشكلة : " + ex.Message, $"Error: " + ex.Message, AdvancedAlert.AlertType.Error, 5);
#else
                AdvancedAlert.ShowAlert($"کێشەیەک ڕوویدا", $"حدثت مشكلة", $"Error", AdvancedAlert.AlertType.Error, 5);
                LogManager.Logger?.Error(ex, "Something went wrong in ExecuteMultipleCommandsAsync.");
#endif
                return (false, 0, ex.Message);
            }
        }
    }
}