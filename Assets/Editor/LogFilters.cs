using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateConsole
{
    public class LogFilters
    {
        public class LogFiltersResult
        {
            public bool isDisplayable;
            public bool isSearchStringSuccess;
            public int searchStringStartIndex = -1;
            public int searchStringEndIndex = -1;


            public static implicit operator bool(LogFiltersResult result)
            {
                return result.isDisplayable;
            }
        }

        public event Action OnLogFilterChanged;

        private short _chanels = -1;
        public short Chanels
        {
            get => _chanels;
            set
            {
                if (_chanels == value)
                    return;
                _chanels = value;
                OnLogFilterChanged?.Invoke();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value)
                    return;
                _searchText = value;
                OnLogFilterChanged?.Invoke();
            }
        }
        private List<LogType> _logTypes = new List<LogType>();

        public LogFiltersResult CheckLog(ULog log)
        {
            LogFiltersResult result = new LogFiltersResult();

            result.isDisplayable = true;

            // Check if the search text is not empty and if the log message contains the search text
            if (!string.IsNullOrEmpty(SearchText))
            {
                result.isSearchStringSuccess = CheckSearchText(log.message, out int startIndex, out int endIndex);
                if (result.isSearchStringSuccess == false)
                    result.isDisplayable = false;
            }

            // Check if the log type is in the list of log types filter
            if (_logTypes.Count > 0 && !_logTypes.Contains(log.logType))
                result.isDisplayable = false;

            // Check if the log chanel is in the list of chanels filter
            if (!CheckChanel(log))
                result.isDisplayable = false;

            return result;
        }

        private bool CheckChanel(ULog log)
        {
            short logicAnd = (short)((int)log.chanel & (int)Chanels);
            return logicAnd == log.chanel;
        }

        private bool CheckSearchText(string message, out int startIndex, out int endIndex)
        {
            startIndex = -1;
            endIndex = -1;

            if (!message.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase))
                return false;


            startIndex = message.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase);
            endIndex = startIndex + SearchText.Length;

            return true;
        }

        public void UpdateLogTypes(LogType logType, bool isEnabled)
        {
            bool isChanged = false;
            if (isEnabled)
            {
                if (!_logTypes.Contains(logType))
                {
                    _logTypes.Add(logType);
                    isChanged = true;
                }
                if (logType == LogType.Error)
                {
                    if (!_logTypes.Contains(LogType.Exception))
                    {
                        _logTypes.Add(LogType.Exception);
                        isChanged = true;
                    }
                }
            }
            else
            {
                if (_logTypes.Contains(logType))
                {
                    _logTypes.Remove(logType);
                    isChanged = true;
                }
            }

            if (isChanged)
                OnLogFilterChanged?.Invoke();
        }
    }
}