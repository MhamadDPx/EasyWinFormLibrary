namespace EasyWinFormLibrary.Data
{
    /// <summary>
    /// Builder class for creating SQL database connection configurations.
    /// Implements the fluent builder pattern for constructing SqlDatabaseConnectionConfig objects.
    /// </summary>
    public class SqlDatabaseConnectionConfigBuilder
    {
        /// <summary>SQL Server instance name or IP address</summary>
        private string _serverName;
        /// <summary>Name of the database</summary>
        private string _databaseName;
        /// <summary>Year suffix for the database name</summary>
        private string _databaseYear;
        /// <summary>Port number for SQL Server connection</summary>
        private int _port = 1433; // Default SQL Server port
        /// <summary>Whether to use Windows Authentication</summary>
        private bool _integratedSecurity = true;
        /// <summary>SQL Server authentication username</summary>
        private string _userId;
        /// <summary>SQL Server authentication password</summary>
        private string _password;
        /// <summary>Authentication user identifier</summary>
        private string _authUserId;
        /// <summary>Whether SQL actions are locked</summary>
        private bool _sqlActionsLocked = true;

        /// <summary>
        /// Gets or sets the currently selected database configuration.
        /// </summary>
        public static SqlDatabaseConnectionConfig SelectedDatabaseConfig;

        /// <summary>
        /// Sets the server name for the SQL Server connection.
        /// </summary>
        /// <param name="serverName">The SQL Server instance name or IP address.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetServerName(string serverName)
        {
            _serverName = serverName;
            return this;
        }

        /// <summary>
        /// Sets the database name.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetDatabaseName(string databaseName)
        {
            _databaseName = databaseName;
            return this;
        }

        /// <summary>
        /// Sets the database year suffix.
        /// </summary>
        /// <param name="databaseYear">The year suffix for the database name.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetDatabaseYear(string databaseYear)
        {
            _databaseYear = databaseYear;
            return this;
        }

        /// <summary>
        /// Sets the port number for the SQL Server connection.
        /// </summary>
        /// <param name="port">The port number (default is 1433).</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetPort(int port)
        {
            _port = port;
            return this;
        }

        /// <summary>
        /// Sets whether to use Windows Authentication.
        /// </summary>
        /// <param name="integratedSecurity">True to use Windows Authentication, false for SQL Server authentication.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder UseIntegratedSecurity(bool integratedSecurity)
        {
            _integratedSecurity = integratedSecurity;
            return this;
        }

        /// <summary>
        /// Sets the SQL Server authentication username.
        /// </summary>
        /// <param name="userId">The SQL Server authentication username.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        /// <summary>
        /// Sets the SQL Server authentication password.
        /// </summary>
        /// <param name="password">The SQL Server authentication password.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetPassword(string password)
        {
            _password = password;
            return this;
        }

        /// <summary>
        /// Sets the authentication user identifier.
        /// </summary>
        /// <param name="authUserId">The authentication user identifier.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetAuthUserId(string authUserId)
        {
            _authUserId = authUserId;
            return this;
        }

        /// <summary>
        /// Sets whether SQL actions are locked.
        /// </summary>
        /// <param name="sqlActionsLocked">True to lock SQL actions, false otherwise.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public SqlDatabaseConnectionConfigBuilder SetSqlActionsLocked(bool sqlActionsLocked)
        {
            _sqlActionsLocked = sqlActionsLocked;
            return this;
        }

        /// <summary>
        /// Builds and returns a new SqlDatabaseConnectionConfig instance with the configured settings.
        /// </summary>
        /// <returns>A new SqlDatabaseConnectionConfig instance.</returns>
        public SqlDatabaseConnectionConfig Build()
        {
            return new SqlDatabaseConnectionConfig(_serverName, _databaseName, _databaseYear, _port, _integratedSecurity, _userId, _password, _authUserId, _sqlActionsLocked);
        }

        /// <summary>
        /// Sets the currently selected database configuration.
        /// </summary>
        /// <param name="config">The database configuration to set as selected.</param>
        public static void SetDatabaseConfig(SqlDatabaseConnectionConfig config)
        {
            SelectedDatabaseConfig = config;
        }
    }
}
