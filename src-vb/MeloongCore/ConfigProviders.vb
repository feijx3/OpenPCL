Imports System
Imports System.IO
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    Public Module ConfigProviders
        <Nullable(1)>
        Public [Default] As JsonConfigProvider = New JsonConfigProvider(Path.Combine(Paths.AppDataThenName, "config.json"))
    End Module
End Namespace
