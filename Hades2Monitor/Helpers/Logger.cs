using System;
using System.Diagnostics;

namespace Hades2Monitor.Helpers
{
    public static class Logger
    {
        private const string Source = "Hades2Monitor";

        static Logger()
        {
            try
            {
                if (!EventLog.SourceExists(Source))
                {
                    EventLog.CreateEventSource(Source, "Application");
                }
            }
            catch (Exception ex)
            {
                // Nie mamy uprawnień – fallback na Console w trybie debug
#if DEBUG
                Console.WriteLine("Logger init failed: " + ex.Message);
#endif
            }
        }

        public static void Info(string message)
        {
            try
            {
                EventLog.WriteEntry(Source, message, EventLogEntryType.Information);
            }
            catch
            {
#if DEBUG
                Console.WriteLine("[INFO] " + message);
#endif
            }
        }

        public static void Error(Exception ex)
        {
            try
            {
                EventLog.WriteEntry(Source, ex.ToString(), EventLogEntryType.Error);
            }
            catch
            {
#if DEBUG
                Console.WriteLine("[ERROR] " + ex);
#endif
            }
        }

        public static void Error(string message)
        {
            try
            {
                EventLog.WriteEntry(Source, message, EventLogEntryType.Error);
            }
            catch
            {
#if DEBUG
                Console.WriteLine("[ERROR] " + message);
#endif
            }
        }
    }
}
