
using UnityEngine;

namespace UltimateConsole
{
    public static class Chanels
    {
		public static readonly Chanel Default = new Chanel("Default", new Color(1f, 1f, 1f));
		public static readonly Chanel Warning = new Chanel("Warning", new Color(1f, 0.9215686f, 0.01568628f));
		public static readonly Chanel Error = new Chanel("Error", new Color(1f, 0f, 0f));

    }
}