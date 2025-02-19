using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PrismLog.Editor")]
namespace PrismLog
{
    internal interface IPLogHandler
    {
        void OnNewLog(PLog log);
        void OnRemoveLog(PLog log);
        void OnClearLogs();
    }
}