using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PrismLog.Editor.Settings
{
    public class PrismLogSettings : ScriptableObject
    {
        private static readonly string[] DEFAULT_CHANELS = new string[] {
            "UI",
            "AI",
            "Network",
            "SaveSystem",
            "PlayerController"
        };

        [Flags]
        public enum EExceptionDisplayMode
        {
            CheckChanel = 1,
            CheckLogType = 2,
        }

        [SerializeField] internal LogChanelSettingChanel[] chanelSettings;
        [SerializeField] internal Texture2D defaultIcon;
        [SerializeField] private EExceptionDisplayMode exceptionDisplayMode = EExceptionDisplayMode.CheckChanel | EExceptionDisplayMode.CheckLogType;

        private Dictionary<PrismLogChanel, LogChanelSettingChanel> chanelSettingsDict = null;

        private void FillChanelSettingsDict()
        {
            chanelSettingsDict = new Dictionary<PrismLogChanel, LogChanelSettingChanel>();

            for (int i = 0; i < chanelSettings.Length; i++)
                chanelSettingsDict.Add(Enum.Parse<PrismLogChanel>(DEFAULT_CHANELS[i]), chanelSettings[i]);
        }

        #region GETTER
        public const string SETTINGS_PATH = "Assets/Plugins/PrismLog/Editor/PrismLogSettings.asset";
        private static string AbsoluteSettingsPath => Path.Join(Application.dataPath.Replace("/Assets", ""), Path.GetDirectoryName(SETTINGS_PATH));

        internal Texture2D GetChanelIcon(PrismLogChanel chanel)
        {
            if (chanel == PrismLogChanel.Default)
                return defaultIcon;

            if (chanelSettingsDict == null) FillChanelSettingsDict();
            if (chanelSettingsDict.TryGetValue(chanel, out LogChanelSettingChanel value))
                return value.Icon;
            return defaultIcon;
        }

        internal static PrismLogSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PrismLogSettings>(SETTINGS_PATH);
            if (settings == null)
            {
                settings = CreateInstance<PrismLogSettings>();
                settings.chanelSettings = new LogChanelSettingChanel[DEFAULT_CHANELS.Length];

                //Settings default value
                for (int i = 0; i < DEFAULT_CHANELS.Length; i++)
                {
                    settings.chanelSettings[i] = new LogChanelSettingChanel(DEFAULT_CHANELS[i], GetChanelIcon(DEFAULT_CHANELS[i]));
                }

                settings.defaultIcon = EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D;

                if (!Directory.Exists(AbsoluteSettingsPath))
                    Directory.CreateDirectory(AbsoluteSettingsPath);
                AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        private static Texture2D GetChanelIcon(string chanelName)
        {
            string iconName = string.Format("{0}-chanel-icon", chanelName.ToLower());

            string[] assets = AssetDatabase.FindAssets($"{iconName} t:texture2D");
            if (assets.Length > 0)
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(assets[0]));
            return null;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public EExceptionDisplayMode ExceptionDisplayMode
        {
            get => exceptionDisplayMode;
        }

        #endregion
    }
}