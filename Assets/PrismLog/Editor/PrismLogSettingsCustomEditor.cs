using UnityEditor;
using UnityEngine;

namespace PrismLog.Editor.Settings
{
    [CustomEditor(typeof(PrismLogSettings))]
    public class PrismLogSettingsCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open in project Settings"))
            {
                SettingsService.OpenProjectSettings("Project/PrismLog");
            }
        }

    }
}
