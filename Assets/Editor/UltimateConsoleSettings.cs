using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UltimateConsole
{
    public class UltimateConsoleSettings : ScriptableObject
    {
        [SerializeField] internal LogChanelSettingChanel[] chanelSettings;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;

        private Dictionary<ushort, LogChanelSettingChanel> chanelSettingsDict = null;

        private void FillChanelSettingsDict()
        {
            chanelSettingsDict = new Dictionary<ushort, LogChanelSettingChanel>();

            for (ushort i = 0; i < chanelSettings.Length; i++)
                chanelSettingsDict.Add(Convert.ToUInt16(i == 0 ? 0 : 1 << i), chanelSettings[i]);
        }

        #region GETTER
        public const string SETTINGS_PATH = "Assets/Plugins/UltimateConsole/Editor/UltimateConsoleSettings.asset";
        private static string AbsoluteSettingsPath => Path.Join(Application.dataPath.Replace("/Assets", ""), Path.GetDirectoryName(SETTINGS_PATH));

        internal Texture2D GetChanelIcon(ushort chanelId)
        {
            if (chanelSettingsDict == null) FillChanelSettingsDict();
            if (chanelSettingsDict.TryGetValue(chanelId, out LogChanelSettingChanel value))
                return value.Icon;
            return null;
        }

        internal static UltimateConsoleSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<UltimateConsoleSettings>(SETTINGS_PATH);
            if (settings == null)
            {
                settings = CreateInstance<UltimateConsoleSettings>();

                //Settings default value
                settings.chanelSettings = new LogChanelSettingChanel[] {
                    new LogChanelSettingChanel("UI"),
                    new LogChanelSettingChanel("AI"),
                    new LogChanelSettingChanel("Network"),
                };

                if (!Directory.Exists(AbsoluteSettingsPath))
                    Directory.CreateDirectory(AbsoluteSettingsPath);
                AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public Color GetColorFromLogType(LogType logType)
        {
            switch (logType)
            {
                case LogType.Exception:
                case LogType.Error:
                    return errorColor;
                case LogType.Warning:
                    return warningColor;
                default:
                    return defaultColor;
            }
        }
        #endregion
    }
}
