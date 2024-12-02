using System;

using DiscordRPC.Logging;

namespace IVPresence.Classes
{
    internal class DiscordLogger : ILogger
    {

        #region Properties
        /// <summary>
        /// The level of logging to apply to this logger.
        /// </summary>
        public LogLevel Level { get; set; }
        #endregion

        #region Constructor
        public DiscordLogger(LogLevel logLevel)
        {
            Level = logLevel;
        }
        public DiscordLogger()
        {
            Level = LogLevel.Info;
        }
        #endregion

        public void Trace(string message, params object[] args)
        {
            if (Level <= LogLevel.Trace)
            {
                string text = "TRACE: " + message;
                Logging.LogDebug(text, args);
            }
        }
        public void Info(string message, params object[] args)
        {
            if (Level <= LogLevel.Info)
            {
                string text = "INFO: " + message;
                Logging.Log(text, args);
            }
        }
        public void Warning(string message, params object[] args)
        {
            if (Level <= LogLevel.Warning)
            {
                string text = "WARN: " + message;
                Logging.LogWarning(text, args);
            }
        }
        public void Error(string message, params object[] args)
        {
            if (Level <= LogLevel.Error)
            {
                string text = "ERR : " + message;
                Logging.LogError(text, args);
            }
        }

    }
}
