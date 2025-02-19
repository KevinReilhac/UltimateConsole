using System;
using System.Collections.Generic;

namespace PrismLog
{
    internal class PLogList
    {
        private const int MAX_SIZE = 100;

        public event Action<PLog> onRemoveLine;
        public event Action<PLog> onAddLine;
        public event Action onClear;

        private List<PLog> logs = new List<PLog>();

        public void Clear()
        {
            onClear?.Invoke();
            logs.Clear();
        }

        internal void AddLine(PLog log)
        {
            if (logs.Count >= MAX_SIZE)
            {
                onRemoveLine?.Invoke(logs[0]);
                logs.RemoveAt(0);
            }
            logs.Add(log);
            onAddLine?.Invoke(log);
        }
    }
}