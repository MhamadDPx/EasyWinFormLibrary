
namespace EasyWinFormLibrary.Data
{
    public class SqlDatabaseConnectionConfigBuilder
    {
        private string _serverName;
        private string _databaseName;
        private string _databaseYear;
        private int _port = 1433; // Default SQL Server port
        private bool _integratedSecurity = true;
        private string _userId;
        private string _password;
        private string _authUserId;
        private bool _sqlActionsLocked = true;
        public SqlDatabaseConnectionConfigBuilder SetServerName(string serverName)
        {
            _serverName = serverName;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder SetDatabaseName(string databaseName)
        {
            _databaseName = databaseName;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder SetDatabaseYear(string databaseYear)
        {
            _databaseYear = databaseYear;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder SetPort(int port)
        {
            _port = port;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder UseIntegratedSecurity(bool integratedSecurity)
        {
            _integratedSecurity = integratedSecurity;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder SetUserId(string userId)
        {
            _userId = userId;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder SetPassword(string password)
        {
            _password = password;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder SetAuthUserId(string authUserId)
        {
            _authUserId = authUserId;
            return this;
        }
        public SqlDatabaseConnectionConfigBuilder SetSqlActionsLocked(bool sqlActionsLocked)
        {
            _sqlActionsLocked = sqlActionsLocked;
            return this;
        }
        public SqlDatabaseConnectionConfig Build()
        {
            return new SqlDatabaseConnectionConfig(_serverName, _databaseName, _databaseYear, _port, _integratedSecurity, _userId, _password, _authUserId, _sqlActionsLocked);
        }
    }
}
