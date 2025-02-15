using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateConsole.Editor.Settings;
using UnityEngine;

namespace UltimateConsole
{
    public class LogFilters
    {
        private UltimateConsoleSettings Settings => UltimateConsoleSettings.GetOrCreateSettings();

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

            public LogFiltersResult(bool isDisplayable)
            {
                this.isDisplayable = isDisplayable;
            }
        }

        public event Action OnLogFilterChanged;

        private long _chanels = -1;
        public long Chanels
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
            // Check special log types, Assert is not filtered and Exception depends on the settings
            if (log.logType == LogType.Assert)
                return new LogFiltersResult(true);
            else if (log.logType == LogType.Exception)
                return CheckLogForException(log);

            LogFiltersResult result = new LogFiltersResult(true);

            // Check if the search text is not empty and if the log message contains the search text
            if (!string.IsNullOrEmpty(SearchText))
            {
                result.isSearchStringSuccess = CheckSearchText(log.message, out result.searchStringStartIndex, out result.searchStringEndIndex);
                if (result.isSearchStringSuccess == false)
                    result.isDisplayable = false;
            }

            // Check if the log type is in the list of log types filter
            if (CheckLogType(log))
                result.isDisplayable = false;

            // Check if the log chanel is in the list of chanels filter
            if (!CheckChanel(log))
                result.isDisplayable = false;

            return result;
        }

        private bool CheckLogType(ULog log)
        {
            return _logTypes.Count > 0 && !_logTypes.Contains(log.logType);
        }

        private LogFiltersResult CheckLogForException(ULog log)
        {
            LogFiltersResult result = new LogFiltersResult(true);

            if (Settings.ExceptionDisplayMode.HasFlag(UltimateConsoleSettings.EExceptionDisplayMode.CheckChanel))
            {
                if (!CheckChanel(log))
                {
                    result.isDisplayable = false;
                    return result;
                }
            }

            if (Settings.ExceptionDisplayMode.HasFlag(UltimateConsoleSettings.EExceptionDisplayMode.CheckLogType))
            {
                if (!_logTypes.Contains(LogType.Error)) 
                {
                    result.isDisplayable = false;
                    return result;
                }
            }

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

        public void UpdateLogTypes(List<LogType> logTypes)
        {
            bool isEqual = logTypes.Count == _logTypes.Count && 
                logTypes.All(item => _logTypes.Contains(item));

            if (isEqual) return;

            _logTypes = logTypes;
            OnLogFilterChanged?.Invoke();
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