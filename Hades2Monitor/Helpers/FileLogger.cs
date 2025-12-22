using System;
using System.IO;

namespace Hades2Monitor.Helpers
{
    public static class FileLogger
    {
        private static readonly string LogPath =
            @"C:\Tests\Hades2Monitor\logs";

        static FileLogger()
        {
            Directory.CreateDirectory(LogPath);
        }

        public static void Log(string message)
        {
            var file = Path.Combine(
                LogPath,
                $"log_{DateTime.UtcNow:yyyyMMdd}.txt");

            File.AppendAllText(file,
                $"{DateTime.UtcNow:o} {message}{Environment.NewLine}");
        }
    }

}
