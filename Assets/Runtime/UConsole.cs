using System;
using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace UltimateConsole
{
    public static class UConsole
    {
        public static ULogList logList = new ULogList();

        public static void Log(string message, ushort chanel = 0, LogType logType = LogType.Log, object context = null)
        {
            ULog newLogLine = new ULog(message, chanel, logType, context);
        }

        public static void LogFormat(string format, ushort chanel = 0, LogType logType = LogType.Log, object context = null, params object[] args)
        {
            string message = string.Format(format, args);

            Log(message, chanel, logType, context);
        }

        public static void LogWarning(string message, ushort chanel = 0, object context = null)
        {
            Log(message, chanel, LogType.Warning, context);
        }

        public static void LogWarningFormat(string format, ushort chanel = 0, object context = null, params object[] args)
        {
            LogFormat(format, chanel, LogType.Warning, context, args);
        }

        public static void LogError(string message, ushort chanel = 0, LogType logType = LogType.Log, object context = null)
        {
            Log(message, chanel, LogType.Error, context);
        }

        public static void LogErrorFormat(string format, ushort chanel = 0, object context = null, params object[] args)
        {
            LogFormat(format, chanel, LogType.Error, context, args);
        }
    }
}
