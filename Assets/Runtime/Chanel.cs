using UnityEngine;

namespace UltimateConsole
{
[System.Serializable]
public class LogChanelSettingChanel
{
    [SerializeField] private string name;
    [SerializeField] private Texture2D icon;

    public LogChanelSettingChanel(string name, Texture2D icon = null)
    {
        this.name = name;
        this.icon = icon;
    }
    
    public string Name => name;
    public Texture2D Icon => icon;
}
}