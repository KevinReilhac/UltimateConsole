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
            SettingsProvider provider = new SettingsProvider("Project/Ultimate Console", SettingsScope.Project)
            {
                guiHandler = Drawer,

                keywords = new HashSet<string>(new[] { "Ultimate", "Console", "Chanel" })
            };

            return provider;
        }

        private static void Drawer(string searchContext)
        {
            SerializedObject settings = UltimateConsoleSettings.GetSerializedSettings();
            SerializedProperty chanelSettings = settings.FindProperty("chanelSettings");

            settings.Update();
            ChanelListDrawer(chanelSettings);
            settings.ApplyModifiedProperties();
            if (GUILayout.Button("Generate"))
                UltimateConsoleChanelsGenerator.Generate();
        }

        private static void ChanelListDrawer(SerializedProperty chanels)
        {
            for (int i = 0; i < chanels.arraySize; i++)
            {
                SerializedProperty item = chanels.GetArrayElementAtIndex(i);
    
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(item.FindPropertyRelative("name"));
                EditorGUILayout.PropertyField(item.FindPropertyRelative("color"));
                EditorGUILayout.PropertyField(item.FindPropertyRelative("icon"));
                //Delete button on line
                EditorGUILayout.EndHorizontal();
            }


            //Check chanel names doublon
            //Plus minus button
        }
    }
}