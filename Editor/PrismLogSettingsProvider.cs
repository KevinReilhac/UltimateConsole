using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace PrismLog.Editor.Settings
{
    public partial class PrismLogSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/Prism Log", SettingsScope.Project)
            {
                guiHandler = Drawer,
                keywords = new HashSet<string>(new[] { "Prism", "Log", "Channel" })
            };

            return provider;
        }

        private static void Drawer(string searchContext)
        {
            SerializedObject settings = PrismLogSettings.GetSerializedSettings();
            settings.Update();

            //Chanels
            DrawTitle("Channels");
            GUI.enabled = ChanelListDrawer(settings.FindProperty(nameof(PrismLogSettings.channelSettings)), settings.FindProperty(nameof(PrismLogSettings.defaultIcon)));
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate Channels Enum", GUILayout.Width(200f), GUILayout.Height(50f)))
                PrismLogChannelsGenerator.Generate();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            //Other Settings
            HorizontalLine();
            EditorGUILayout.Space();
            DrawTitle("Other Settings");
            EditorGUILayout.PropertyField(settings.FindProperty("exceptionDisplayMode"), PrismLogSettingsProviderGUIContents.ExceptionDisplayModeContent);

            settings.ApplyModifiedProperties();
        }

        private static bool ChanelListDrawer(SerializedProperty chanels, SerializedProperty defaultChanelIcon)
        {
            //Variable Initialisation
            string[] chanelNames = new string[chanels.arraySize];
            SerializedProperty item = null;
            SerializedProperty nameProperty = null;
            SerializedProperty iconProperty = null;
            bool valid = true;


            //Default Chanel
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextField("Default Chanel");
            GUI.enabled = true;
            EditorGUILayout.PropertyField(defaultChanelIcon, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            //Lines
            for (int i = 0; i < chanels.arraySize; i++)
            {
                item = chanels.GetArrayElementAtIndex(i);
                nameProperty = item.FindPropertyRelative("name");
                iconProperty = item.FindPropertyRelative("icon");

                Color oldColor = GUI.color;
                nameProperty.stringValue = nameProperty.stringValue.Trim();
                if (chanelNames.Contains(nameProperty.stringValue) ||
                    nameProperty.stringValue == string.Empty ||
                    Regex.IsMatch(nameProperty.stringValue, @"^\d"))
                {
                    GUI.color = Color.red;
                    valid = false;
                }
                EditorGUILayout.BeginHorizontal();
                string newNameValue = EditorGUILayout.TextField(GUIContent.none, nameProperty.stringValue);
                if (newNameValue != nameProperty.stringValue)
                    nameProperty.stringValue = ClearEnumValueName(newNameValue);
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
            GUI.enabled = chanels.arraySize < PrismLogChannelsGenerator.MAX_CHANEL_COUNT;
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus"), EditorStyles.miniButtonRight, GUILayout.Width(50f)))
                chanels.arraySize++;
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            return valid;
        }

        private static string ClearEnumValueName(string enumValueName)
        {
            List<char> chars = enumValueName.ToList();

            chars[0] = char.ToUpper(chars[0]);
            for (int i = 0; i < chars.Count; i++)
            {
                if (i == 0)
                    chars[i] = char.ToUpper(chars[i]);
                else if (char.IsUpper(chars[i]))
                    chars[i] = char.ToLower(chars[i]);
                if (!char.IsLetter(chars[i]))
                    chars[i] = ' ';
            }

            string result = string.Empty;
            foreach (char c in chars)
            {
                if (c != ' ')
                    result += c;
            }

            return NormalizeString(result);
        }

        public static string NormalizeString(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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