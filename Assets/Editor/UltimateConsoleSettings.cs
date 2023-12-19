using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace UltimateConsole
{
    public class UltimateConsoleSettings : ScriptableObject
    {
        public const string SETTINGS_PATH = "Assets/Plugins/UltimateConsole/Editor/UltimateConsoleSettings.asset";
        private static string AbsoluteSettingsPath => Path.Join(Application.dataPath.Replace("/Assets", ""), Path.GetDirectoryName(SETTINGS_PATH));

        public List<Chanel> chanelSettings;

        internal static UltimateConsoleSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<UltimateConsoleSettings>(SETTINGS_PATH);
            if (settings == null)
            {
                settings = CreateInstance<UltimateConsoleSettings>();

                //Settings default value
                settings.chanelSettings = new List<Chanel>()
            {
                new Chanel("Default", Color.white),
                new Chanel("Warning", Color.yellow),
                new Chanel("Error", Color.red),
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
    }
}
