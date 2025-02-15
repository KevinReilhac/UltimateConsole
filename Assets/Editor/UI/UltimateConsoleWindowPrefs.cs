using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UltimateConsole.Editor.Window
{
    internal static class UltimateConsoleWindowPrefs
    {
        private const string KEY_LOG_ENABLED = "UltimateConsole.Editor.Window.UltimateConsoleWindowPrefs.logEnabled";
        private const string KEY_LOG_WARNING_ENABLED = "UltimateConsole.Editor.Window.UltimateConsoleWindowPrefs.logWarningEnabled";
        private const string KEY_LOG_ERROR_ENABLED = "UltimateConsole.Editor.Window.UltimateConsoleWindowPrefs.logErrorEnabled";
        private const string KEY_ERROR_PAUSE_ENABLED = "UltimateConsole.Editor.Window.UltimateConsoleWindowPrefs.errorPauseEnabled";
        private const string KEY_COLLAPSE_ENABLED = "UltimateConsole.Editor.Window.UltimateConsoleWindowPrefs.collapseEnabled";
        private const string CHANELS = "UltimateConsole.Editor.Window.UltimateConsoleWindowPrefs.chanelEnabled";

        public static bool LogEnabled
        {
            get => EditorPrefs.GetBool(KEY_LOG_ENABLED, true);
            set => EditorPrefs.SetBool(KEY_LOG_ENABLED, value);
        }

        public static bool LogWarningEnabled
        {
            get => EditorPrefs.GetBool(KEY_LOG_WARNING_ENABLED, true);
            set => EditorPrefs.SetBool(KEY_LOG_WARNING_ENABLED, value);
        }
        
        public static bool LogErrorEnabled
        {
            get => EditorPrefs.GetBool(KEY_LOG_ERROR_ENABLED, true);
            set => EditorPrefs.SetBool(KEY_LOG_ERROR_ENABLED, value);
        }

        public static bool ErrorPauseEnabled
        {
            get => EditorPrefs.GetBool(KEY_ERROR_PAUSE_ENABLED, true);
            set => EditorPrefs.SetBool(KEY_ERROR_PAUSE_ENABLED, value);
        }
        
        public static bool CollapseEnabled
        {
            get => EditorPrefs.GetBool(KEY_COLLAPSE_ENABLED, true);
            set => EditorPrefs.SetBool(KEY_COLLAPSE_ENABLED, value);
        }
        
        public static int Chanels
        {
            get => EditorPrefs.GetInt(CHANELS, -1);
            set => EditorPrefs.SetInt(CHANELS, value);
        }

        public static List<LogType> GetEnabledLogTypes()
        {
            List<LogType> logTypes = new List<LogType>();

            if (LogEnabled) logTypes.Add(LogType.Log);
            if (LogWarningEnabled) logTypes.Add(LogType.Warning);
            if (LogErrorEnabled) logTypes.Add(LogType.Error);

            return logTypes;
        }

        public static void SetEnabledLogTypes(List<LogType> logTypes)
        {
            LogEnabled = logTypes.Contains(LogType.Log);
            LogWarningEnabled = logTypes.Contains(LogType.Warning);
            LogErrorEnabled = logTypes.Contains(LogType.Error);
        }
    }
}
