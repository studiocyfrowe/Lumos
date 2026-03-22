using Lumos.Agent.Application.Contexts;
using System;
using System.IO;

namespace Lumos.Agent.Application.Helpers
{
    public class SqlLoader
    {
        public SqlLoader() { }

        public string Load(string resourceName)
        {
            var assembly = typeof(LumosContext).Assembly;

            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception($"SQL resource not found: {resourceName}");

            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
