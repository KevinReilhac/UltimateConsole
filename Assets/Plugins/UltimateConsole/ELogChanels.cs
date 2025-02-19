
using System;

namespace UltimateConsole
{
    [Flags]
    public enum ELogChanels : short
    {
        Default = 1,
        UI = 2,
        AI = 4,
        Network = 8,
        PlayerController = 16,
        SceneManagement = 32,
        SAUCISSES = 64,

    }
}