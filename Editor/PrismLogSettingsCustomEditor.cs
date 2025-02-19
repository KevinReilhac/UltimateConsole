using UnityEditor;
using UnityEngine;

namespace PrismLog.Editor.Settings
{
    [CustomEditor(typeof(PrismLogSettings))]
    public class PrismLogSettingsCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open PrismLog in project Settings"))
            {
                SettingsService.OpenProjectSettings("Project/Prism Log");
            }
        }

    }
}
