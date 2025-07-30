using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EasyWinFormLibrary.Data
{
    /// <summary>
    /// Represents configuration settings for SQL Server database connections.
    /// Handles connection string generation and connection testing.
    /// </summary>
    public class SqlDatabaseConnectionConfig
    {
        /// <summary>Gets or sets the name of the database.</summary>
        public string DatabaseName { get; set; }

        /// <summary>Gets or sets the year suffix for the database name.</summary>
        public string DatabaseYear { get; set; }

        /// <summary>Gets or sets the complete database name including year suffix if specified.</summary>
        private string ActualDatabaseName { get; set; }

        /// <summary>Gets or sets the SQL Server instance name or IP address.</summary>
        public string ServerName { get; set; }

        /// <summary>Gets or sets the port number for the SQL Server connection.</summary>
        public int Port { get; set; }

        /// <summary>Gets or sets whether to use Windows Authentication.</summary>
        public bool IntegratedSecurity { get; set; }

        /// <summary>Gets or sets the SQL Server authentication username.</summary>
        public string DatabaseUserId { get; set; }

        /// <summary>Gets or sets the SQL Server authentication password.</summary>
        public string Password { get; set; }

        /// <summary>Gets or sets the authentication user identifier.</summary>
        public string AuthUserId { get; set; }

        /// <summary>Gets or sets whether SQL actions are locked.</summary>
        public bool SqlActionsLocked { get; set; }

        /// <summary>
        /// Initializes a new instance of the SqlDatabaseConnectionConfig class.
        /// </summary>
        /// <param name="serverName">The SQL Server instance name or IP address.</param>
        /// <param name="databaseName">The name of the database.</param>
        /// <param name="databaseYear">Optional year suffix for the database name.</param>
        /// <param name="port">The port number for the SQL Server connection.</param>
        /// <param name="integratedSecurity">Whether to use Windows Authentication.</param>
        /// <param name="databaseUserId">Optional SQL Server authentication username.</param>
        /// <param name="password">Optional SQL Server authentication password.</param>
        /// <param name="authUserId">Optional authentication user identifier.</param>
        /// <param name="sqlActionsLocked">Whether SQL actions are locked.</param>
        public SqlDatabaseConnectionConfig(string serverName, string databaseName, string databaseYear, int port, bool integratedSecurity, string databaseUserId = null, string password = null, string authUserId = null, bool sqlActionsLocked = false)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            DatabaseYear = databaseYear;
            Port = port;
            IntegratedSecurity = integratedSecurity;
            DatabaseUserId = databaseUserId;
            Password = password;
            AuthUserId = authUserId;
            SqlActionsLocked = sqlActionsLocked;

            ActualDatabaseName = DatabaseName;
            if (!string.IsNullOrEmpty(DatabaseYear))
            {
                ActualDatabaseName = $"{DatabaseName}_{DatabaseYear}";
            }
        }

        /// <summary>
        /// Generates a SQL Server connection string based on the configuration settings.
        /// </summary>
        /// <param name="HasDatabaseName">Whether to include the database name in the connection string.</param>
        /// <returns>A valid SQL Server connection string.</returns>
        public string GetConnectionString(bool HasDatabaseName = true)
        {
            if (IntegratedSecurity)
            {
                return $"Server={ServerName},{Port};{(HasDatabaseName ? $"Database={ActualDatabaseName}" : string.Empty)};Integrated Security=True;";
            }
            else
            {
                return $"Server={ServerName},{Port};{(HasDatabaseName ? $"Database={ActualDatabaseName}" : string.Empty)};User Id={DatabaseUserId};Password={Password};";
            }
        }

        /// <summary>
        /// Tests the database connection and verifies the server's datetime year matches the local system.
        /// </summary>
        /// <returns>
        /// A tuple containing:
        /// - IsConnected: Whether the connection was successful
        /// - ErrorMessage: Any error message if the connection failed
        /// </returns>
        public async Task<(bool IsConnected, string ErrorMessage)> CheckDatabaseConnectionAsync()
        {
            try
            {
                using (var connection = new SqlConnection(GetConnectionString(false)))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("SELECT YEAR(GETDATE()) AS current_db_year", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        string serverYear = result?.ToString();
                        string currentYear = DateTime.Now.Year.ToString();

                        if (currentYear != serverYear)
                        {
                            return (false, $"Computer year ({currentYear}) does not match server year ({serverYear})");
                        }

                        return (true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Connection to server failed: {ex.Message}");
            }
        }
    }
}