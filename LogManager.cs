using Serilog;
using Serilog.Events;

namespace EasyWinFormLibrary
{
    public static class LogManager
    {
        private static ILogger _logger;

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
