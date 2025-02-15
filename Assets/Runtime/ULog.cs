using System;
using System.Linq;
using UnityEngine;

namespace UltimateConsole
{
    public struct ULog
    {
        private const int HASHCODE_STACKTRACE_SUBSTRING_LENGHT = 100;

        public string message;
        public long chanel;
        public LogType logType;
        public object context;
        public string stacktrace;


        public ULog(string message, long chanel, LogType logType, object context)
        {
            this.message = message;
            this.chanel = chanel;
            this.logType = logType;
            this.context = context;
            this.stacktrace = StackTraceUtility.ExtractStackTrace();
        }

        public override int GetHashCode()
        {
            string stackTraceSubString = stacktrace.Substring(0, Math.Min(stacktrace.Length, HASHCODE_STACKTRACE_SUBSTRING_LENGHT));
            return HashCode.Combine(message, chanel, logType, stackTraceSubString);
        }
    }
}
