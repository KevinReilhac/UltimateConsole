using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrismLog
{
    public interface IPLogHandler
    {
        void OnNewLog(PLog log);
        void OnRemoveLog(PLog log);
        void OnClearLogs();
    }
}