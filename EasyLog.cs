using System;

namespace EasyWinFormLibrary
{
    /// <summary>
    /// Minimal logging seam used internally by EasyWinFormLibrary so the core library
    /// has no hard dependency on any specific logging framework.
    /// </summary>
    /// <remarks>
    /// By default <see cref="Logger"/> is null and internal error logging is a no-op.
    /// Install the optional EasyWinFormLibrary.Logging package and call
    /// <c>LogManager.InitLogger(...)</c> at startup to route these calls to
    /// Serilog/Sentry, or assign your own <see cref="IEasyLibraryLogger"/> implementation
    /// to <see cref="Logger"/> to use any logging framework you like.
    /// </remarks>
    public static class EasyLog
    {
        /// <summary>
        /// The active logger. Null by default (logging is disabled until one is assigned).
        /// </summary>
        public static IEasyLibraryLogger Logger { get; set; }
    }

    /// <summary>
    /// Minimal logger contract used by EasyWinFormLibrary's core error handling.
    /// Implement this to plug in any logging framework (Serilog, NLog, log4net, etc.).
    /// </summary>
    public interface IEasyLibraryLogger
    {
        void Error(Exception ex, string message);
    }
}
