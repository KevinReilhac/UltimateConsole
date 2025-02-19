using System;
using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace PrismLog
{
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

        public static void Log(string message, PrismLogChanel chanel = PrismLogChanel.Default, LogType logType = LogType.Log, object context = null)
        {
            PLog newLogLine = new PLog(message, chanel, logType, context);
            string chanelName = chanel != PrismLogChanel.Default ? chanel.ToString() : "Default";

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

        public static void LogFormat(string format, PrismLogChanel chanel = PrismLogChanel.Default, LogType logType = LogType.Log, object context = null, params object[] args)
        {
            string message = string.Format(format, args);

            Log(message, chanel, logType, context);
        }

        public static void LogWarning(string message, PrismLogChanel chanel = PrismLogChanel.Default, object context = null)
        {
            Log(message, chanel, LogType.Warning, context);
        }

        public static void LogWarningFormat(string format, PrismLogChanel chanel = PrismLogChanel.Default, object context = null, params object[] args)
        {
            LogFormat(format, chanel, LogType.Warning, context, args);
        }

        public static void LogError(string message, PrismLogChanel chanel = PrismLogChanel.Default, object context = null)
        {
            Log(message, chanel, LogType.Error, context);
        }

        public static void LogErrorFormat(string format, PrismLogChanel chanel = PrismLogChanel.Default, object context = null, params object[] args)
        {
            LogFormat(format, chanel, LogType.Error, context, args);
        }
    }
}
