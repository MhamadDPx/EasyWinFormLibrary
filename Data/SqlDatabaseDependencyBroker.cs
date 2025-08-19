using EasyWinFormLibrary.WinAppNeeds;
using System;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace EasyWinFormLibrary.Data
{
    public class SqlDatabaseDependencyBroker<T> : IDisposable where T : class, new()
    {
        #region Fields and Properties
        private SqlTableDependency<T> _tableDependency;
        private readonly Form _handlerForm;
        private readonly string _tableName;
        private bool _disposed = false;

        public bool IsStarted { get; private set; }
        public string TableName => _tableName;

        // Events
        public delegate void RecordChangedHandler(RecordChangedEventArgs<T> e);
        public event RecordChangedHandler OnRecordChanged;
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler OnStarted;
        public event EventHandler OnStopped;
        #endregion

        #region Constructor
        public SqlDatabaseDependencyBroker(string tableName, Form handlerForm)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));

            _handlerForm = handlerForm ?? throw new ArgumentNullException(nameof(handlerForm));
            _tableName = tableName;

            InitializeTableDependency();
        }
        #endregion

        #region Private Methods
        private void InitializeTableDependency()
        {
            try
            {
                var connectionString = SqlDatabaseConnectionConfigBuilder.SelectedDatabaseConfig.GetConnectionString(true);
                _tableDependency = new SqlTableDependency<T>(connectionString, _tableName);

                // Wire up events
                _tableDependency.OnChanged += TableDependency_OnChanged;
                _tableDependency.OnError += TableDependency_OnError;
                _tableDependency.OnStatusChanged += TableDependency_OnStatusChanged;
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize table dependency: {ex.Message}", ex);
                throw;
            }
        }

        private void TableDependency_OnChanged(object sender, RecordChangedEventArgs<T> e)
        {
            try
            {
                // Ensure we're on the UI thread when invoking the event
                ThreadSafe(() => OnRecordChanged?.Invoke(e));
            }
            catch (Exception ex)
            {
                LogError($"Error handling record changed event: {ex.Message}", ex);
            }
        }

        private void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            LogError($"TableDependency error: {e.Error?.Message}", e.Error);
            ThreadSafe(() => OnError?.Invoke(this, e));
        }

        private void TableDependency_OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            IsStarted = e.Status == TableDependencyStatus.Started || e.Status == TableDependencyStatus.WaitingForNotification;

            ThreadSafe(() =>
            {
                if (IsStarted)
                    OnStarted?.Invoke(this, EventArgs.Empty);
                else
                    OnStopped?.Invoke(this, EventArgs.Empty);
            });
        }
        #endregion

        #region Public Methods
        public bool StartTableDependency()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SqlDatabaseDependencyBroker<T>));

            if (IsStarted)
            {
                LogInfo("Table dependency is already started.");
                return true;
            }

            try
            {
                _tableDependency?.Start();
                LogInfo($"Table dependency started for table: {_tableName}");
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Failed to start table dependency: {ex.Message}", ex);
                return false;
            }
        }

        public bool StopTableDependency()
        {
            if (_disposed)
                return true; // Already disposed

            if (!IsStarted)
            {
                LogInfo("Table dependency is already stopped.");
                return true;
            }

            try
            {
                _tableDependency?.Stop();
                LogInfo($"Table dependency stopped for table: {_tableName}");
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Failed to stop table dependency: {ex.Message}", ex);
                return false;
            }
        }

        public void RestartTableDependency()
        {
            StopTableDependency();
            System.Threading.Thread.Sleep(1000); // Brief delay
            StartTableDependency();
        }
        #endregion

        #region Logging Methods
        private void LogError(string message, Exception exception = null)
        {
            var fullMessage = exception != null
                ? $"{message} | Exception: {exception}"
                : message;

            ThreadSafe(() => AdvancedAlert.ShowAlert(
                fullMessage,
                "Database Dependency Error",
                $"Table: {_tableName}",
                AdvancedAlert.AlertType.Error));
        }

        private void LogInfo(string message)
        {
            // You might want to add info logging capability
            System.Diagnostics.Debug.WriteLine($"[SqlDependencyBroker] {message}");
        }
        #endregion

        #region Thread Safety
        private void ThreadSafe(MethodInvoker method)
        {
            try
            {
                if (_handlerForm?.InvokeRequired == true)
                {
                    if (!_handlerForm.IsDisposed)
                        _handlerForm.Invoke(method);
                }
                else
                {
                    method?.Invoke();
                }
            }
            catch (ObjectDisposedException ex)
            {
                // Form was disposed, log but don't show alert
                System.Diagnostics.Debug.WriteLine($"Form disposed during invoke: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                // Handle case where form handle is not created
                System.Diagnostics.Debug.WriteLine($"Form handle not created: {ex.Message}");
            }
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        StopTableDependency();
                        _tableDependency?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error during disposal: {ex.Message}");
                    }
                }
                _disposed = true;
            }
        }

        ~SqlDatabaseDependencyBroker()
        {
            Dispose(false);
        }
        #endregion
    }

    #region Usage Example
    // Example usage in your form:
    /*
    public partial class MyForm : Form
    {
        private SqlDatabaseDependencyBroker<MyEntity> _dependencyBroker;

        public MyForm()
        {
            InitializeComponent();
            InitializeDependencyBroker();
        }

        private void InitializeDependencyBroker()
        {
            _dependencyBroker = new SqlDatabaseDependencyBroker<MyEntity>("MyTable", this);
            
            // Subscribe to events
            _dependencyBroker.OnRecordChanged += OnRecordChanged;
            _dependencyBroker.OnError += OnDependencyError;
            _dependencyBroker.OnStarted += OnDependencyStarted;
            _dependencyBroker.OnStopped += OnDependencyStopped;
            
            // Start monitoring
            _dependencyBroker.StartTableDependency();
        }

        private void OnRecordChanged(RecordChangedEventArgs<MyEntity> e)
        {
            // Handle the change based on ChangeType
            switch (e.ChangeType)
            {
                case ChangeType.Insert:
                    HandleInsert(e.Entity);
                    break;
                case ChangeType.Update:
                    HandleUpdate(e.Entity, e.EntityOldValues);
                    break;
                case ChangeType.Delete:
                    HandleDelete(e.EntityOldValues);
                    break;
            }
        }

        private void OnDependencyError(object sender, ErrorEventArgs e)
        {
            // Handle dependency errors
            MessageBox.Show($"Dependency error: {e.Error?.Message}");
        }

        private void OnDependencyStarted(object sender, EventArgs e)
        {
            // Update UI to show monitoring is active
            statusLabel.Text = "Monitoring active";
        }

        private void OnDependencyStopped(object sender, EventArgs e)
        {
            // Update UI to show monitoring is stopped
            statusLabel.Text = "Monitoring stopped";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dependencyBroker?.Dispose();
            base.OnFormClosing(e);
        }
    }
    */
    #endregion
}