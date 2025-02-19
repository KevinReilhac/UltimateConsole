using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using UnityEditor;
using UnityEngine;
using PrismLog.Editor.Settings;

namespace PrismLog.Editor
{
    internal static class PrismLogChanelsGenerator
    {
        //Max chanel count is 63, because of the long type
        public const int MAX_CHANEL_COUNT = 63;

        public const string ENUM_NAME = "ELogChanels";

        private static readonly string PATH = $"Plugins/PrismLog/Runtime/{ENUM_NAME}.cs";

        public static void Generate()
        {
            string fullPath = Path.Join(Application.dataPath, PATH);
            string fileString = ClassString.Replace("##CONTENT##", GetChanelStrings()).Replace("##ENUM_NAME##", ENUM_NAME);
            string directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllText(fullPath, fileString, Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        private static string GetChanelStrings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            LogChanelSettingChanel[] chanelsSettings = PrismLogSettings.GetOrCreateSettings().chanelSettings;

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

namespace PrismLog
{
    [Flags]
    public enum ##ENUM_NAME## : long
    {
##CONTENT##
    }
}";
    }
}