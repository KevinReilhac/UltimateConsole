using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

static class UltimateConsoleSettingsProvider
{
    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        SettingsProvider provider = new SettingsProvider("Project/UltimateConsole", SettingsScope.Project)
        {
            label = "UltimateConsole",
            guiHandler = (searchContext) => 
            {
                SerializedObject settings = UltimateConsoleSettings.GetSerializedSettings();

                EditorGUILayout.PropertyField(settings.FindProperty("chanelSettings"), new GUIContent("Settings"));
            },

            keywords = new HashSet<string>(new [] {"Ultimate", "Console", "Chanel"})
        };

        return provider;
    }
}
