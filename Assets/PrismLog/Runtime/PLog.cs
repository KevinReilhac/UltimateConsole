using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace PrismLog
{
    public struct PLog
    {
        private static readonly string[] STACKTRACE_EXLUDE_CLASSNAME = new string[]
        {
            nameof(PLog), nameof(PConsole), nameof(Application),
            "DebugLogHandler", nameof(Logger), nameof(UnityEngine.Debug)
        };

        public string message;
        public long chanel;
        public LogType logType;
        public object context;
        public List<StackFrame> stacktrace;


        public PLog(string message, long chanel, LogType logType, object context)
        {
            this.message = message;
            this.chanel = chanel;
            this.logType = logType;
            this.context = context;
            this.stacktrace = null;

            this.stacktrace = GetStacktrace();
        }

        private List<StackFrame> GetStacktrace()
        {
            StackTrace stackTrace = new StackTrace(true);
            List<StackFrame> stackFrames = new List<StackFrame>();

            Type declaringType = null;
            foreach (StackFrame frame in stackTrace.GetFrames())
            {
                declaringType = frame.GetMethod().DeclaringType;
                //Exclude UltimateConsole classes from stacktrace
                if (STACKTRACE_EXLUDE_CLASSNAME.Contains(declaringType.Name))
                    continue;
                if (!string.IsNullOrEmpty(declaringType.Namespace) && declaringType.Namespace.Contains("UltimateConsole"))
                    continue;
                stackFrames.Add(frame);
            }
            return stackFrames;
        }

        public override int GetHashCode()
        {
            int hashCode = HashCode.Combine(message, chanel, logType);

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
