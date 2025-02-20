using UnityEngine;


namespace PrismLog
{
    /// <summary>
    /// A static class that contains methods for logging messages to the ◭ console.
    /// </summary>
    public static class PConsole
    {
        public const string DEFAULT_CONSOLE_LOG_START = "◭";
        private const string DEFAULT_CONSOLE_LOG_FORMAT = DEFAULT_CONSOLE_LOG_START + "({0}) : {1}";

        internal static PLogList logList = new PLogList();

        internal static void RegisterLogHandler(IPLogHandler logHandler)
        {
            logList.onAddLine += logHandler.OnNewLog;
            logList.onClear += logHandler.OnClearLogs;
            logList.onRemoveLine += logHandler.OnRemoveLog;
        }

        internal static void UnRegisterLogHandler(IPLogHandler logHandler)
        {
            logList.onAddLine -= logHandler.OnNewLog;
            logList.onClear -= logHandler.OnClearLogs;
            logList.onRemoveLine -= logHandler.OnRemoveLog;
        }

        internal static PLogList GetLogList() => logList;

        /// <summary>
        /// Log a message with a channel and a log type.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="channel">The channel to log the message to.</param>
        /// <param name="logType">The type of log to log.</param>
        /// <param name="context">The context of the log.</param>
        public static void Log(string message, PrismLogChannel chanel = PrismLogChannel.Default, LogType logType = LogType.Log, object context = null)
        {
            PLog newLogLine = new PLog(message, chanel, logType, context);
            string chanelName = chanel != PrismLogChannel.Default ? chanel.ToString() : "Default";

            string defaultConsoleLogLine = string.Format(DEFAULT_CONSOLE_LOG_FORMAT, chanelName, newLogLine.message);

            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(defaultConsoleLogLine, context as UnityEngine.Object);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(defaultConsoleLogLine, context as UnityEngine.Object);
                    break;
                case LogType.Error:
                    Debug.LogError(defaultConsoleLogLine, context as UnityEngine.Object);
                    break;
            }

            logList.AddLine(newLogLine);
        }

        /// <summary>
        /// Log a formatted message with a channel and a log type.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="channel">The channel to log the message to.</param>
        /// <param name="logType">The type of log to log.</param>
        /// <param name="context">The context of the log.</param>
        public static void LogFormat(string format, PrismLogChannel channel = PrismLogChannel.Default, LogType logType = LogType.Log, object context = null, params object[] args)
        {
            string message = string.Format(format, args);

            Log(message, channel, logType, context);
        }

        /// <summary>
        /// Log a warning message with a channel.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="channel">The channel to log the message to.</param>
        /// <param name="context">The context of the log.</param>
        public static void LogWarning(string message, PrismLogChannel channel = PrismLogChannel.Default, object context = null)
        {
            Log(message, channel, LogType.Warning, context);
        }

        /// <summary>
        /// Log a formatted warning message with a channel.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="channel">The channel to log the message to.</param>
        /// <param name="context">The context of the log.</param>
        public static void LogWarningFormat(string format, PrismLogChannel channel = PrismLogChannel.Default, object context = null, params object[] args)
        {
            LogFormat(format, channel, LogType.Warning, context, args);
        }

        /// <summary>
        /// Log an error message with a channel.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="channel">The channel to log the message to.</param>
        /// <param name="context">The context of the log.</param>
        public static void LogError(string message, PrismLogChannel channel = PrismLogChannel.Default, object context = null)
        {
            Log(message, channel, LogType.Error, context);
        }

        /// <summary>
        /// Log a formatted error message with a channel.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="channel">The channel to log the message to.</param>
        /// <param name="context">The context of the log.</param>
        public static void LogErrorFormat(string format, PrismLogChannel channel = PrismLogChannel.Default, object context = null, params object[] args)
        {
            LogFormat(format, channel, LogType.Error, context, args);
        }
    }
}
