Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    Public Interface IConfigProvider
        <NullableContext(2)>
        Sub [Set](Of T)(<Nullable(1)> key As String, value As T, encrypted As Boolean)

        Sub Remove(key As String)

        <NullableContext(2)>
        Function Read(Of T)(<Nullable(1)> key As String, defaultValue As T, encrypted As Boolean) As T

        Function HasValue(key As String) As Boolean

        Sub DiscardCache()
    End Interface
End Namespace
