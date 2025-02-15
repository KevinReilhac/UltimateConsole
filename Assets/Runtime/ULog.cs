using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace UltimateConsole
{
    public struct ULog
    {
        private static readonly string[] STACKTRACE_EXLUDE_CLASSNAME = new string[] { nameof(ULog), nameof(UConsole) };
        public string message;
        public long chanel;
        public LogType logType;
        public object context;
        public List<StackFrame> stacktrace;


        public ULog(string message, long chanel, LogType logType, object context)
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
            foreach (StackFrame frame in stackTrace.GetFrames())
            {
                //Exclude UltimateConsole classes from stacktrace
                if (STACKTRACE_EXLUDE_CLASSNAME.Contains(frame.GetMethod().DeclaringType.Name))
                    continue;
                stackFrames.Add(frame);
            }
            return stackFrames;
        }

        public override int GetHashCode()
        {
            string stackTraceSubString = string.Empty;
            if (stacktrace != null && stacktrace.Count > 0)
                stackTraceSubString = stacktrace[0].ToString();

            return HashCode.Combine(message, chanel, logType, stackTraceSubString);
        }
    }
}
