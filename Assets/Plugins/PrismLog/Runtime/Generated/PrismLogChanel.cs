
#if PRISM_LOG_CHANEL_GENERATED
using System;

namespace PrismLog
{
    [Flags]
    public enum PrismLogChannel : long
    {
        Default = 1,
        UI = 2,
        AI = 4,
        Network = 8,
        SaveSystem = 16,
        PlayerController = 32,

    }
}
#endif
