using UnityEngine;

[System.Serializable]
public class ChanelSettings
{
    [SerializeField] private string name;
    [SerializeField] private Color color;
    [SerializeField] private Texture2D icon;

    public ChanelSettings(string name, Color color, Texture2D icon = null)
    {
        this.name = name;
        this.color = color;
        this.icon = icon;
    }
}
