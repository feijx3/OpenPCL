Imports System
Imports MeloongCore.Extensions

Namespace PCLCS
    Public Module Resource
        Public Function FromCurseForgeModLoaderType(modLoaderType As Integer) As ModLoaders
            Select Case modLoaderType
                Case 1
                    Return ModLoaders.Forge
                Case 3
                    Return ModLoaders.LiteLoader
                Case 4
                    Return ModLoaders.Fabric
                Case 6
                    Return ModLoaders.NeoForge
            End Select
            Return ModLoaders.None
        End Function

        Public Function ToCurseForgeModLoaderType(loaders As ModLoaders) As Integer
            If loaders = ModLoaders.None Then
                Return 0
            End If
            If Not loaders.Flags().IsSingle() Then
                Throw New ArgumentException(String.Format("传入的枚举值 {0} 并非单个 Mod Loader", loaders), "loaders")
            End If
            Select Case loaders
                Case ModLoaders.Forge
                    Return 1
                Case ModLoaders.LiteLoader
                    Return 3
                Case ModLoaders.Forge Or ModLoaders.LiteLoader
                Case ModLoaders.Fabric
                    Return 4
                Case Else
                    If loaders = ModLoaders.NeoForge Then
                        Return 6
                    End If
            End Select
            Return 0
        End Function
    End Module
End Namespace
