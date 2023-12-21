using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateConsole.Editor
{
    public interface IULogHandler
    {
        void OnNewLog(ULog log);
        void OnRemoveLog(ULog log);
        void OnClearLogs();

        public void Register()
        {
            UConsole.logList.onAddLine += OnNewLog;
            UConsole.logList.onClear += OnClearLogs;
            UConsole.logList.onRemoveLine += OnRemoveLog;
        }

        public void Unregister()
        {
            UConsole.logList.onAddLine -= OnNewLog;
            UConsole.logList.onClear -= OnClearLogs;
            UConsole.logList.onRemoveLine -= OnRemoveLog;
        }
    }
}