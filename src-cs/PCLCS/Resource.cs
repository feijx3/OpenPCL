using System;
using MeloongCore.Extensions;

namespace PCLCS
{
    public static class Resource
    {
        public static ModLoaders FromCurseForgeModLoaderType(int modLoaderType)
        {
            switch (modLoaderType)
            {
            case 1:
                return ModLoaders.Forge;
            case 3:
                return ModLoaders.LiteLoader;
            case 4:
                return ModLoaders.Fabric;
            case 6:
                return ModLoaders.NeoForge;
            }
            return ModLoaders.None;
        }

        public static int ToCurseForgeModLoaderType(ModLoaders loaders)
        {
            if (loaders == ModLoaders.None)
            {
                return 0;
            }
            if (!loaders.Flags<ModLoaders>().IsSingle<ModLoaders>())
            {
                throw new ArgumentException(string.Format("传入的枚举值 {0} 并非单个 Mod Loader", loaders), "loaders");
            }
            switch (loaders)
            {
            case ModLoaders.Forge:
                return 1;
            case ModLoaders.LiteLoader:
                return 3;
            case ModLoaders.Forge | ModLoaders.LiteLoader:
                break;
            case ModLoaders.Fabric:
                return 4;
            default:
                if (loaders == ModLoaders.NeoForge)
                {
                    return 6;
                }
                break;
            }
            return 0;
        }
    }
}
