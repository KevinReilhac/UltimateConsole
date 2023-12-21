using System;
using System.Linq;
using UnityEngine;

namespace UltimateConsole
{
    public struct ULog
    {
        public Guid guid;
        public string message;
        public ushort chanel;
        public LogType logType;
        public object context;
        public string stacktrace;

        public ULog(string message, ushort chanel, LogType logType, object context)
        {
            this.guid = Guid.NewGuid();
            this.message = message;
            this.chanel = chanel;
            this.logType = logType;
            this.context = context;
            this.stacktrace = StackTraceUtility.ExtractStackTrace();
        }

        public bool CheckFilters(ushort chanel, LogType[] logTypes)
        {
            return (this.chanel & chanel) == chanel &&
                    logTypes.Contains(this.logType);
        }
    }
}
