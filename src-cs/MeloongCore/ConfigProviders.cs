using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    public static class ConfigProviders
    {
        [Nullable(1)]
        public static JsonConfigProvider Default = new JsonConfigProvider(Path.Combine(Paths.AppDataThenName, "config.json"));
    }
}
