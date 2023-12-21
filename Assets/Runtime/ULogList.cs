using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateConsole
{
    public class ULogList
    {
        private const int MAX_SIZE = 300;

        public event Action<ULog> onRemoveLine;
        public event Action<ULog> onAddLine;
        public event Action onClear;

        private List<ULog> logs = new List<ULog>();

        public void Clear()
        {
            onClear?.Invoke();
            logs.Clear();
        }

        internal void AddLine(ULog log)
        {
            if (logs.Count >= MAX_SIZE)
            {
                onRemoveLine?.Invoke(logs[0]);
                logs.RemoveAt(0);
            }
            logs.Add(log);
            onAddLine?.Invoke(log);
        }

        public List<ULog> GetWithFilters(ushort chanel, LogType[] logTypes)
        {
            return logs.Where(l => l.CheckFilters(chanel, logTypes)).ToList();
        }
    }
}