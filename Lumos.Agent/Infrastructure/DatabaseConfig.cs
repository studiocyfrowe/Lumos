using System;
using System.IO;

namespace Lumos.Agent.Infrastructure
{
    public static class DatabaseConfig
    {
        public static string GetConnectionString()
        {
            string basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "studiocyfrowe_apps",
                "Lumos");

            Directory.CreateDirectory(basePath);

            string dbPath = Path.Combine(basePath, "LumosDatabase.db");

            return $"Data Source={dbPath};Cache=Shared;";
        }
    }
}
