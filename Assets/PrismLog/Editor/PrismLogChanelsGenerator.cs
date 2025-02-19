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

        public const string ENUM_NAME = "PrismLogChanel";
        public const string DEFINE = "PRISM_LOG_CHANEL_GENERATED";

        public static readonly string PATH = $"Plugins/PrismLog/Runtime/Generated/{ENUM_NAME}.cs";
        private static readonly string ASMDEF_PATH = $"Plugins/PrismLog/Runtime/Generated/PrismLog.Generated.asmdef";

#if PRISM_LOG_CHANEL_GENERATED

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CheckIfFileNoLongerExist();
        }

        private static void CheckIfFileNoLongerExist()
        {
            if (!File.Exists(Path.Join(Application.dataPath, PATH)))
                RemoveAsGenerated();
        }
#endif

        public static void Generate()
        {
            string fullFilePath = Path.Join(Application.dataPath, PATH);
            string fullAsmdefPath = Path.Join(Application.dataPath, ASMDEF_PATH);
            string fileString = ClassString.Replace("##CONTENT##", GetChanelStrings()).Replace("##ENUM_NAME##", ENUM_NAME);
            string directory = Path.GetDirectoryName(fullFilePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllText(fullFilePath, fileString, Encoding.UTF8);
            File.WriteAllText(fullAsmdefPath, ASMDEF_STRING, Encoding.UTF8);
            AssetDatabase.Refresh();

            DefineAsGenerated();
        }

        private static void DefineAsGenerated()
        {
            // Get all possible build target groups
            foreach (BuildTargetGroup buildTargetGroup in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                // Skip Invalid or Unknown targets
                if (buildTargetGroup == BuildTargetGroup.Unknown ||
                    !System.Enum.IsDefined(typeof(BuildTargetGroup), buildTargetGroup))
                    continue;

                // Get current defines for this target
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

                // Add our define if not present
                if (!defines.Contains(DEFINE))
                {
                    defines = string.IsNullOrEmpty(defines) ? DEFINE : defines + ";" + DEFINE;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                }
            }
        }

        public static void RemoveAsGenerated()
        {
            foreach (BuildTargetGroup buildTargetGroup in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                if (defines.Contains(DEFINE))
                {
                    defines = defines.Replace(DEFINE, "");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
                }
            }
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

        private static void CreateAsmdef()
        {
            string fullPath = Path.Join(Application.dataPath, PATH);
            string fileString = ASMDEF_STRING.Replace("##ENUM_NAME##", ENUM_NAME);
            string directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllText(fullPath, fileString, Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        private static string GetChanelString(string name, int id)
        {
            return $"        {name} = {id},";
        }


        private const string ClassString =
@"
#if PRISM_LOG_CHANEL_GENERATED
using System;

namespace PrismLog
{
    [Flags]
    public enum ##ENUM_NAME## : long
    {
##CONTENT##
    }
}
#endif
";
        private const string ASMDEF_STRING =
    @"
{
    ""name"": ""PrismLog.Generated"",
    ""rootNamespace"": """",
    ""references"": [],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}
";
    }
}