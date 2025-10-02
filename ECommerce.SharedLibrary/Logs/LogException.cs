using Serilog;

namespace ECommerce.SharedLibrary.Logs
{
    public static class LogException
    {
        public static void LogExceptions(Exception ex)
        {
            LogTofile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }
        public static void LogTofile(string message) => Log.Information(message);
        public static void LogToConsole(string message) => Log.Warning(message);
        public static void LogToDebugger(string message) => Log.Debug(message);
        
    }



   }
  

