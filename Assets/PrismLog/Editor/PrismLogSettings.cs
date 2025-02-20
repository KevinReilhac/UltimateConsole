using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PrismLog.Editor.Settings
{
    public class PrismLogSettings : ScriptableObject
    {
        private static readonly string[] DEFAULT_CHANNELS = new string[] {
            "UI",
            "AI",
            "Network",
            "SaveSystem",
            "PlayerController"
        };

        [Flags]
        public enum EExceptionDisplayMode
        {
            CheckChannel = 1,
            CheckLogType = 2,
        }

        [SerializeField] internal LogChannelSettingChanel[] channelSettings;
        [SerializeField] internal Texture2D defaultIcon;
        [SerializeField] private EExceptionDisplayMode exceptionDisplayMode = EExceptionDisplayMode.CheckChannel | EExceptionDisplayMode.CheckLogType;

        private Dictionary<PrismLogChannel, LogChannelSettingChanel> channelSettingsDict = null;

        private void FillChanelSettingsDict()
        {
            channelSettingsDict = new Dictionary<PrismLogChannel, LogChannelSettingChanel>();

            for (int i = 0; i < channelSettings.Length; i++)
                channelSettingsDict.Add(Enum.Parse<PrismLogChannel>(DEFAULT_CHANNELS[i]), channelSettings[i]);
        }

        #region GETTER
        public const string SETTINGS_PATH = "Assets/Plugins/PrismLog/Editor/PrismLogSettings.asset";
        private static string AbsoluteSettingsPath => Path.Join(Application.dataPath.Replace("/Assets", ""), Path.GetDirectoryName(SETTINGS_PATH));

        internal Texture2D GetChannelIcon(PrismLogChannel channel)
        {
            if (channel == PrismLogChannel.Default)
                return defaultIcon;

            if (channelSettingsDict == null) FillChanelSettingsDict();
            if (channelSettingsDict.TryGetValue(channel, out LogChannelSettingChanel value))
                return value.Icon;
            return defaultIcon;
        }

        internal static PrismLogSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PrismLogSettings>(SETTINGS_PATH);
            if (settings == null)
            {
                settings = CreateInstance<PrismLogSettings>();
                settings.channelSettings = new LogChannelSettingChanel[DEFAULT_CHANNELS.Length];

                //Settings default value
                for (int i = 0; i < DEFAULT_CHANNELS.Length; i++)
                {
                    settings.channelSettings[i] = new LogChannelSettingChanel(DEFAULT_CHANNELS[i], GetChannelIcon(DEFAULT_CHANNELS[i]));
                }

                settings.defaultIcon = EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D;

                if (!Directory.Exists(AbsoluteSettingsPath))
                    Directory.CreateDirectory(AbsoluteSettingsPath);
                AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        private static Texture2D GetChannelIcon(string channelName)
        {
            string iconName = string.Format("{0}-channel-icon", channelName.ToLower());

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