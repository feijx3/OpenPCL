using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class Main
    {
        public static void Init(string appName)
        {
            Main.AppName = appName;
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(5.0));
        }

        public static string AppName = "Meloong";
    }
}
