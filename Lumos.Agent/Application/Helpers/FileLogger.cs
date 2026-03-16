using System;
using System.IO;

namespace Lumos.Agent.Application.Helpers
{
    public static class FileLogger
    {
        private static readonly string LogPath =
            @"C:\Tests\Lumos\logs";

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
                $"[INFO] {DateTime.UtcNow:o} {message}{Environment.NewLine}");
        }
    }

}
