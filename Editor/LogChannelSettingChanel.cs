using UnityEngine;

namespace PrismLog
{
    [System.Serializable]
    public class LogChannelSettingChanel
    {
        [SerializeField] private string name;
        [SerializeField] private Texture2D icon;

        public LogChannelSettingChanel(string name, Texture2D icon = null)
        {
            this.name = name;
            this.icon = icon;
        }

        public string Name => name;
        public Texture2D Icon => icon;
    }
}