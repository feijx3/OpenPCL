Imports System

Namespace PCLCS
    <Flags()>
    Public Enum ModLoaders
        None = 0
        Forge = 1
        LiteLoader = 2
        Fabric = 4
        NeoForge = 16
        All = 23
    End Enum
End Namespace
