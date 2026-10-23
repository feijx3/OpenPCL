Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module Paths
        Public ReadOnly Property Base As String
            Get
                Return PathUtils.AddSlashSuffix(AppContext.BaseDirectory)
            End Get
        End Property

        Public ReadOnly Property BaseThenName As String
            Get
                Return PathUtils.AddSlashSuffix(Paths.Base + Main.AppName)
            End Get
        End Property

        Public ReadOnly Property AppData As String
            Get
                Return PathUtils.AddSlashSuffix(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
            End Get
        End Property

        Public ReadOnly Property AppDataThenName As String
            Get
                Return PathUtils.AddSlashSuffix(Paths.AppData + Main.AppName)
            End Get
        End Property
    End Module
End Namespace
