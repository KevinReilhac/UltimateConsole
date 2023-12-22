using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateConsole
{
    public interface IULogHandler
    {
        void OnNewLog(ULog log);
        void OnRemoveLog(ULog log);
        void OnClearLogs();
    }
}