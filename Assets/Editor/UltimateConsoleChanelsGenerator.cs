using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UltimateConsole
{
    internal static class UltimateConsoleChanelsGenerator
    {
        private const string PATH = "Plugins/UltimateConsole/Chanels.cs";

        public static void Generate()
        {
            string fileString = ClassString.Replace("##CONTENT##", GetChanelStrings());
            
            File.WriteAllText(Path.Join(Application.dataPath, PATH), fileString, Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        private static string GetChanelStrings()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Chanel[] chanelsSettings = UltimateConsoleSettings.GetOrCreateSettings().chanelSettings;

            foreach (Chanel chanel in chanelsSettings)
            {
                stringBuilder.AppendLine(GetChanelString(chanel.Name, chanel.Color));
            }

            return stringBuilder.ToString();
        }

        private static string GetChanelString(string name, Color color)
        {
            string r = color.r.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string g = color.g.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string b = color.b.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return $"\t\tpublic static readonly Chanel {name} = new Chanel(\"{name}\", new Color({r}f, {g}f, {b}f));";
        }

        private const string ClassString =
@"
using UnityEngine;

namespace UltimateConsole
{
    public static class Chanels
    {
##CONTENT##
    }
}";
    }
}