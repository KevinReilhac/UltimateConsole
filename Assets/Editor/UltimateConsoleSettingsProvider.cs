using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

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
            settings.Update();

            //Chanels
            DrawTitle("Chanels");
            GUI.enabled = ChanelListDrawer(settings.FindProperty(nameof(UltimateConsoleSettings.chanelSettings)));
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_RotateTool On", "Generate"), GUILayout.Width(200f), GUILayout.Height(50f)))
                UltimateConsoleChanelsGenerator.Generate();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            //LogTypes Color
            HorizontalLine();
            EditorGUILayout.Space();
            DrawTitle("LogTypes Color");
            EditorGUILayout.PropertyField(settings.FindProperty(nameof(UltimateConsoleSettings.defaultColor)));
            EditorGUILayout.PropertyField(settings.FindProperty(nameof(UltimateConsoleSettings.warningColor)));
            EditorGUILayout.PropertyField(settings.FindProperty(nameof(UltimateConsoleSettings.errorColor)));
            settings.ApplyModifiedProperties();
        }

        private static bool ChanelListDrawer(SerializedProperty chanels)
        {
            //Variable Initialisation
            string[] chanelNames = new string[chanels.arraySize];
            SerializedProperty item = null;
            SerializedProperty nameProperty = null;
            SerializedProperty iconProperty = null;
            bool valid = true;

            //Lines
            for (int i = 0; i < chanels.arraySize; i++)
            {
                item = chanels.GetArrayElementAtIndex(i);
                nameProperty = item.FindPropertyRelative("name");
                iconProperty = item.FindPropertyRelative("icon");
    
                Color oldColor = GUI.color;
                if (chanelNames.Contains(nameProperty.stringValue))
                {
                    GUI.color = Color.red;
                    valid = false;
                }
    
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(nameProperty, new GUIContent(""));
                EditorGUILayout.PropertyField(iconProperty, new GUIContent(""));
                GUI.color = oldColor;
                chanelNames[i] = nameProperty.stringValue;
                EditorGUILayout.EndHorizontal();
            }

            //Button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = chanels.arraySize > 1;
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus"), EditorStyles.miniButtonLeft, GUILayout.Width(50f)))
                chanels.arraySize--;
            GUI.enabled = true;
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), EditorStyles.miniButtonRight, GUILayout.Width(50f)))
                chanels.arraySize++;
            EditorGUILayout.EndHorizontal();
            return valid;
        }

        private static void DrawTitle(string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        private static void HorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}