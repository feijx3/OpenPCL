using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class Paths
    {
        public static string Base
        {
            get
            {
                return PathUtils.AddSlashSuffix(AppContext.BaseDirectory);
            }
        }

        public static string BaseThenName
        {
            get
            {
                return PathUtils.AddSlashSuffix(Paths.Base + Main.AppName);
            }
        }

        public static string AppData
        {
            get
            {
                return PathUtils.AddSlashSuffix(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
        }

        public static string AppDataThenName
        {
            get
            {
                return PathUtils.AddSlashSuffix(Paths.AppData + Main.AppName);
            }
        }
    }
}
