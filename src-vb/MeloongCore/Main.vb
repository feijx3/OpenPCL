Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module Main
        Public Sub Init(appName As String)
            Main.AppName = appName
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(5.0))
        End Sub

        Public AppName As String = "Meloong"
    End Module
End Namespace
