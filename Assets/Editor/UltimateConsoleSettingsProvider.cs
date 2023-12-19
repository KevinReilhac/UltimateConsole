using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UltimateConsole
{
    static class UltimateConsoleSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/UltimateConsole", SettingsScope.Project)
            {
                label = "Ultimate Console",
                guiHandler = (searchContext) =>
                {
                    SerializedObject settings = UltimateConsoleSettings.GetSerializedSettings();

                    EditorGUILayout.PropertyField(settings.FindProperty("chanelSettings"), new GUIContent("Chanels"));
                    if (GUILayout.Button("Generate"))
                        UltimateConsoleChanelsGenerator.Generate();
                },

                keywords = new HashSet<string>(new[] { "Ultimate", "Console", "Chanel" })
            };

            return provider;
        }
    }
}