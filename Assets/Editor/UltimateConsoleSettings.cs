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
        [SerializeField] internal Texture2D defaultIcon;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;

        private Dictionary<long, LogChanelSettingChanel> chanelSettingsDict = null;

        private void FillChanelSettingsDict()
        {
            chanelSettingsDict = new Dictionary<long, LogChanelSettingChanel>();

            for (int i = 0; i < chanelSettings.Length; i++)
                chanelSettingsDict.Add(Convert.ToInt64(i == 0 ? 0 : 1L << i), chanelSettings[i]);
        }

        #region GETTER
        public const string SETTINGS_PATH = "Assets/Plugins/UltimateConsole/Editor/UltimateConsoleSettings.asset";
        private static string AbsoluteSettingsPath => Path.Join(Application.dataPath.Replace("/Assets", ""), Path.GetDirectoryName(SETTINGS_PATH));

        internal Texture2D GetChanelIcon(long chanelId)
        {
            if (chanelId == 1)
                return defaultIcon;

            if (chanelSettingsDict == null) FillChanelSettingsDict();
            if (chanelSettingsDict.TryGetValue(chanelId, out LogChanelSettingChanel value))
                return value.Icon;
            return defaultIcon;
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

                settings.defaultIcon = EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D;

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
