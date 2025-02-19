
using System;

namespace PrismLog
{
    [Flags]
    public enum ELogChanels : long
    {
        Default = 1,
        UI = 2,
        AI = 4,
        Network = 8,

    }
}