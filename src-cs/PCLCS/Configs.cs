using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MeloongCore;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class Configs
    {
        static Configs()
        {
            Configs.JavaForced.PreviewGet += delegate(PreviewGetEventArgs<Java> args, IConfigProvider provider)
            {
                if (args.Value != null && !Configs.JavaList.Get().Contains(args.Value))
                {
                    args.Value = null;
                    Configs.JavaForced.Reset(provider);
                }
            };
        }

        public static readonly DynamicConfigEntry<McInstallManifest> McInstallManifest = new DynamicConfigEntry<McInstallManifest>("MinecraftInstallManifest", null, false);

        public static readonly ConfigEntry<int> JavaConfigVersion = new ConfigEntry<int>("JavaConfigVersion", 0, false, null);

        public static readonly ConfigEntry<ConcurrentList<Java>> JavaList = new ConfigEntry<ConcurrentList<Java>>("JavaList", new ConcurrentList<Java>(), false, null);

        public static readonly ConfigEntry<List<string>> JavaRemovedList = new ConfigEntry<List<string>>("JavaRemovedList", new List<string>(), false, null);

        public static readonly DynamicConfigEntry<bool> JavaMigrated = new DynamicConfigEntry<bool>("InstanceMigratedJava", false, false);

        public static readonly DynamicConfigEntry<Java> JavaForced = new DynamicConfigEntry<Java>("InstanceForcedJava", null, false);

        public static readonly ConfigEntry<bool> HintModifyInstanceWarning = new ConfigEntry<bool>("HintModifyInstanceWarning", false, false, null);
    }
}
