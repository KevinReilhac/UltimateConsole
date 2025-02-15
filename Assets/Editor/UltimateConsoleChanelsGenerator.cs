using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using UnityEditor;
using UnityEngine;
using UltimateConsole.Editor.Settings;

namespace UltimateConsole.Editor
{
    internal static class UltimateConsoleChanelsGenerator
    {
        //Max chanel count is 63, because of the long type
        public const int MAX_CHANEL_COUNT = 63;

        public const string ENUM_NAME = "ELogChanels";

        private static readonly string PATH = $"Plugins/UltimateConsole/{ENUM_NAME}.cs";

        public static void Generate()
        {
            string fileString = ClassString.Replace("##CONTENT##", GetChanelStrings()).Replace("##ENUM_NAME##", ENUM_NAME);
            
            File.WriteAllText(Path.Join(Application.dataPath, PATH), fileString, Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        private static string GetChanelStrings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            LogChanelSettingChanel[] chanelsSettings = UltimateConsoleSettings.GetOrCreateSettings().chanelSettings;

            stringBuilder.AppendLine(GetChanelString("Default", 1));
            for (int i = 0; i < Mathf.Min(chanelsSettings.Length, MAX_CHANEL_COUNT); i++)
                stringBuilder.AppendLine(GetChanelString(chanelsSettings[i].Name, 1 << (i + 1)));


            return stringBuilder.ToString();
        }

        private static string GetChanelString(string name, int id)
        {
            return $"        {name} = {id},";
        }


        private const string ClassString =
@"
using System;

namespace UltimateConsole
{
    [Flags]
    public enum ##ENUM_NAME## : short
    {
##CONTENT##
    }
}";
    }
}