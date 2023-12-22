using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UltimateConsole
{
    internal static class UltimateConsoleChanelsGenerator
    {
        public const string ENUM_NAME = "LogChanels";

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

            stringBuilder.AppendLine(GetChanelString("Default", 0));
            for (int i = 0; i < chanelsSettings.Length; i++)
                stringBuilder.AppendLine(GetChanelString(chanelsSettings[i].Name, 1 << i));

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
    public enum ##ENUM_NAME## : ushort
    {
##CONTENT##
    }
}";
    }
}