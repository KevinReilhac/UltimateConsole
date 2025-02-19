using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PrismLog.Editor.Window
{
    public class LogTypesToggles : VisualElement
    {
        private const string CLASS_NAME = "log-types-toggles";
        private Dictionary<LogType, LogToggle> logToggles;

        public event Action<List<LogType>> OnLogTypesChanged;

        public new class UxmlFactory : UxmlFactory<LogTypesToggles, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public UxmlIntAttributeDescription logCountAttr = new UxmlIntAttributeDescription { name = "log-count" };
            public UxmlIntAttributeDescription logWarningCountAttr = new UxmlIntAttributeDescription { name = "log-warning-count" };
            public UxmlIntAttributeDescription logErrorCountAttr = new UxmlIntAttributeDescription { name = "log-error-count" };

            public UxmlBoolAttributeDescription logToggleAttr = new UxmlBoolAttributeDescription { name = "log-toggle" };
            public UxmlBoolAttributeDescription logWarningToggleAttr = new UxmlBoolAttributeDescription { name = "log-warning-toggle" };
            public UxmlBoolAttributeDescription logErrorToggleAttr = new UxmlBoolAttributeDescription { name = "log-error-toggle" };


            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                LogTypesToggles logTypesToggles = ve as LogTypesToggles;


                base.Init(ve, bag, cc);
                
                logTypesToggles.LogCount = logCountAttr.GetValueFromBag(bag, cc);
                logTypesToggles.LogWarningCount = logWarningCountAttr.GetValueFromBag(bag, cc);
                logTypesToggles.LogErrorCount = logErrorCountAttr.GetValueFromBag(bag, cc);

                logTypesToggles.LogToggle = logToggleAttr.GetValueFromBag(bag, cc);
                logTypesToggles.LogWarningToggle = logWarningToggleAttr.GetValueFromBag(bag, cc);
                logTypesToggles.LogErrorToggle = logErrorToggleAttr.GetValueFromBag(bag, cc);
            }
        }

        public int LogCount
        {
            get => logToggles[LogType.Log].Count;
            set => logToggles[LogType.Log].Count = value;
        }

        public int LogWarningCount
        {
            get => logToggles[LogType.Warning].Count;
            set => logToggles[LogType.Warning].Count = value;
        }

        public int LogErrorCount
        {
            get => logToggles[LogType.Error].Count;
            set => logToggles[LogType.Error].Count = value;
        }

        public bool LogToggle
        {
            get => logToggles[LogType.Log].value;
            set => logToggles[LogType.Log].value = value;
        }

        public bool LogWarningToggle
        {
            get => logToggles[LogType.Warning].value;
            set => logToggles[LogType.Warning].value = value;
        }
        
        public bool LogErrorToggle
        {
            get => logToggles[LogType.Error].value;
            set => logToggles[LogType.Error].value = value;
        }

        public LogTypesToggles()
        {
            logToggles = new Dictionary<LogType, LogToggle>();

            foreach (LogType logType in Enum.GetValues(typeof(LogType)))
            {
                // Ignore Exception and Assert logs
                if (logType == LogType.Exception || logType == LogType.Assert)
                    continue;

                LogToggle logToggle = new LogToggle(logType);
                logToggles.Add(logType, logToggle);
                logToggle.RegisterValueChangedCallback(OnLogToggleValueChanged);
                hierarchy.Add(logToggle);
            }

            AddToClassList(CLASS_NAME);
        }

        private void OnLogToggleValueChanged(ChangeEvent<bool> evt)
        {
            OnLogTypesChanged?.Invoke(GetActiveLogTypes());
        }

        public void IncrementLogCount(LogType logType)
        {
            logToggles[GetSimpleLogType(logType)].Count++;
        }

        public bool IsLogTypeActive(LogType logType)
        {
            return logToggles[GetSimpleLogType(logType)].value;
        }

        private LogType GetSimpleLogType(LogType logType)
        {
            if (logType == LogType.Exception || logType == LogType.Assert)
                return LogType.Error;

            return logType;
        }

        public List<LogType> GetActiveLogTypes()
        {
            List<LogType> activeLogTypes = new List<LogType>();
            foreach (LogType logType in logToggles.Keys)
            {
                if (logToggles[logType].value)
                    activeLogTypes.Add(logType);
            }

            return activeLogTypes;
        }

        public void SetLogTypes(List<LogType> logTypes, bool notify = true)
        {
            foreach (LogType logType in logTypes)
            {
                if (logToggles.ContainsKey(logType))
                {
                    if (notify)
                        logToggles[logType].value = true;
                    else
                        logToggles[logType].SetValueWithoutNotify(true);
                }
                else
                {
                    if (notify)
                        logToggles[logType].value = false;
                    else
                        logToggles[logType].SetValueWithoutNotify(false);
                }
            }
        }

        public void ResetCounts()
        {
            LogCount = 0;
            LogWarningCount = 0;
            LogErrorCount = 0;
        }
    }
}
