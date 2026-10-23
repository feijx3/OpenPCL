using System;

namespace PCLCS
{
    [Flags]
    public enum ResourceTypes
    {
        Mod = 1,
        ModPack = 2,
        ResourcePack = 4,
        Shader = 8,
        DataPack = 16,
        Plugin = 32,
        Map = 64,
        ModOrDataPack = 17,
        Any = 127
    }
}
