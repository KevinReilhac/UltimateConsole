using System.Runtime.CompilerServices;

namespace PrismLog
{
    internal interface IPLogHandler
    {
        void OnNewLog(PLog log);
        void OnRemoveLog(PLog log);
        void OnClearLogs();
    }
}