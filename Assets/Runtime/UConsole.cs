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

        public static void RegisterLogHandler(IULogHandler logHandler)
        {
            logList.onAddLine += logHandler.OnNewLog;
            logList.onClear += logHandler.OnClearLogs;
            logList.onRemoveLine += logHandler.OnRemoveLog;
        }

        public static void UnRegisterLogHandler(IULogHandler logHandler)
        {
            logList.onAddLine -= logHandler.OnNewLog;
            logList.onClear -= logHandler.OnClearLogs;
            logList.onRemoveLine -= logHandler.OnRemoveLog;
        }

        public static void Log(string message, IConvertible chanel = null, LogType logType = LogType.Log, object context = null)
        {
            ushort chanelValue = 0;
            if (chanel != null)
                chanelValue = Convert.ToUInt16(chanel);
            ULog newLogLine = new ULog(message, chanelValue, logType, context);

            logList.AddLine(newLogLine);
        }

        public static void LogFormat(string format, IConvertible chanel = null, LogType logType = LogType.Log, object context = null, params object[] args)
        {
            string message = string.Format(format, args);

            Log(message, chanel, logType, context);
        }

        public static void LogWarning(string message, IConvertible chanel = null, object context = null)
        {
            Log(message, chanel, LogType.Warning, context);
        }

        public static void LogWarningFormat(string format, IConvertible chanel = null, object context = null, params object[] args)
        {
            LogFormat(format, chanel, LogType.Warning, context, args);
        }

        public static void LogError(string message, IConvertible chanel = null, LogType logType = LogType.Log, object context = null)
        {
            Log(message, chanel, LogType.Error, context);
        }

        public static void LogErrorFormat(string format, IConvertible chanel = null, object context = null, params object[] args)
        {
            LogFormat(format, chanel, LogType.Error, context, args);
        }
    }
}
