using UnityEngine;

namespace UltimateConsole
{
[System.Serializable]
public class Chanel
{
    [SerializeField] private string name;
    [SerializeField] private Color color;
    [SerializeField] private Texture2D icon;

    public Chanel(string name, Color color, Texture2D icon = null)
    {
        this.name = name;
        this.color = color;
        this.icon = icon;
    }
    
    public string Name => name;
    public Color Color => color;
}
}