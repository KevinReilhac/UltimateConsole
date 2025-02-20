
#if !PRISM_LOG_CHANEL_GENERATED
using System;

namespace PrismLog
{
    [Flags]
    public enum PrismLogChannel : long
    {
        Default = 1,
        UI = 1 << 1,
        AI = 1 << 2,
        Network = 1 << 3,
        SaveSystem = 1 << 4,
        PlayerController = 1 << 5,
        GenerateYourOwnPrismLogChannel = 1 << 6,
    }
}
#endif
