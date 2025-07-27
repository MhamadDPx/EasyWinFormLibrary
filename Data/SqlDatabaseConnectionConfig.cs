
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EasyWinFormLibrary.Data
{
    public class SqlDatabaseConnectionConfig
    {
        public string DatabaseName { get; set; }
        public string DatabaseYear { get; set; }
        private string ActualDatabaseName { get; set; }
        public string ServerName { get; set; }
        public int Port { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string DatabaseUserId { get; set; }
        public string Password { get; set; }
        public string AuthUserId { get; set; }
        public bool SqlActionsLocked { get; set; }

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
