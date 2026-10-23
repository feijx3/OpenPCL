Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports MeloongCore

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    Public Module Configs
        Shared Sub New()
            AddHandler Configs.JavaForced.PreviewGet, Sub(args As PreviewGetEventArgs(Of Java), provider As IConfigProvider)
                If args.Value IsNot Nothing AndAlso Not Configs.JavaList.[Get]().Contains(args.Value) Then
                    args.Value = Nothing
                    Configs.JavaForced.Reset(provider)
                End If
            End Sub
        End Sub

        Public McInstallManifest As DynamicConfigEntry(Of McInstallManifest) = New DynamicConfigEntry(Of McInstallManifest)("MinecraftInstallManifest", Nothing, False)

        Public JavaConfigVersion As ConfigEntry(Of Integer) = New ConfigEntry(Of Integer)("JavaConfigVersion", 0, False, Nothing)

        Public JavaList As ConfigEntry(Of ConcurrentList(Of Java)) = New ConfigEntry(Of ConcurrentList(Of Java))("JavaList", New ConcurrentList(Of Java)(), False, Nothing)

        Public JavaRemovedList As ConfigEntry(Of List(Of String)) = New ConfigEntry(Of List(Of String))("JavaRemovedList", New List(Of String)(), False, Nothing)

        Public JavaMigrated As DynamicConfigEntry(Of Boolean) = New DynamicConfigEntry(Of Boolean)("InstanceMigratedJava", False, False)

        Public JavaForced As DynamicConfigEntry(Of Java) = New DynamicConfigEntry(Of Java)("InstanceForcedJava", Nothing, False)

        Public HintModifyInstanceWarning As ConfigEntry(Of Boolean) = New ConfigEntry(Of Boolean)("HintModifyInstanceWarning", False, False, Nothing)
    End Module
End Namespace
