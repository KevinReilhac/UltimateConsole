
using System;

namespace UltimateConsole
{
    [Flags]
    public enum LogChanels : ushort
    {
        Default = 0,
        UI = 1,
        AI = 2,
        Network = 4,

    }
}