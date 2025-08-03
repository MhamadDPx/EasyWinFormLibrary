using Serilog;
using Serilog.Events;

namespace EasyWinFormLibrary
{
    /// <summary>
    /// Provides centralized logging functionality using Serilog with optional Sentry integration.
    /// This static class manages logger initialization and provides a global logger instance for the entire library.
    /// </summary>
    /// <remarks>
    /// The LogManager uses lazy initialization for the default logger and supports Sentry error tracking
    /// when initialized with a DSN. The default logger configuration creates a basic logger without any sinks,
    /// while the initialized logger includes Sentry integration for error-level events.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Initialize with Sentry integration
    /// LogManager.InitLogger("https://your-sentry-dsn@sentry.io/project-id");
    /// 
    /// // Use the logger throughout your application
    /// LogManager.Logger.Information("Application started");
    /// LogManager.Logger.Error("An error occurred: {Error}", ex.Message);
    /// </code>
    /// </example>
    public static class LogManager
    {
        /// <summary>
        /// The internal Serilog logger instance.
        /// </summary>
        private static ILogger _logger;

        /// <summary>
        /// Gets the global logger instance, creating a default logger if none has been initialized.
        /// </summary>
        /// <value>
        /// An ILogger instance that can be used for logging throughout the application.
        /// If no custom initialization has been performed, returns a basic logger with default configuration.
        /// </value>
        /// <remarks>
        /// The property uses lazy initialization - the logger is created only when first accessed.
        /// If InitLogger has been called, this property returns the configured logger with Sentry integration.
        /// Otherwise, it returns a basic logger created with default settings.
        /// </remarks>
        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new LoggerConfiguration()
                        .CreateLogger();
                }
                return _logger;
            }
        }

        /// <summary>
        /// Initializes the logger with Sentry integration for error tracking and reporting.
        /// </summary>
        /// <param name="sentryDsn">
        /// The Sentry Data Source Name (DSN) URL for connecting to your Sentry project.
        /// This should be in the format: https://public-key@sentry.io/project-id
        /// </param>
        /// <remarks>
        /// This method configures the logger with the following settings:
        /// - Minimum log level set to Debug for comprehensive logging
        /// - Sentry sink configured to capture Error level events and above
        /// - Replaces any existing logger configuration
        /// 
        /// Call this method early in your application startup, preferably in the Main method
        /// or application initialization code, to ensure all subsequent logging includes Sentry integration.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Initialize logging with Sentry in your application startup
        /// LogManager.InitLogger("https://abcd1234@o123456.ingest.sentry.io/987654");
        /// 
        /// // Now all Error and Fatal level logs will be sent to Sentry
        /// LogManager.Logger.Error("Database connection failed");
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the sentryDsn parameter is null, empty, or not a valid Sentry DSN format.
        /// </exception>
        public static void InitLogger(string sentryDsn)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Sentry(o =>
                {
                    o.Dsn = sentryDsn;
                    o.MinimumEventLevel = LogEventLevel.Error;
                })
                .CreateLogger();
        }
    }
}