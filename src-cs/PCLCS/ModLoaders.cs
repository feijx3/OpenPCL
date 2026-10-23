using System;

namespace PCLCS
{
    [Flags]
    public enum ModLoaders
    {
        None = 0,
        Forge = 1,
        LiteLoader = 2,
        Fabric = 4,
        NeoForge = 16,
        All = 23
    }
}
