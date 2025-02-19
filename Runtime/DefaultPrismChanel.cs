
#if !PRISM_LOG_CHANEL_GENERATED
using System;

namespace PrismLog
{
    [Flags]
    public enum PrismLogChanel : long
    {
        Default = 0,
        UI = 1 << 0,
        AI = 1 << 1,
        Network = 1 << 2,
        SaveSystem = 1 << 3,
        PlayerController = 1 << 4,
        ProofThisIsDefault = 1 << 5,
    }
}
#endif
