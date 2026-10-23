Imports System
Imports System.Net
Imports System.Net.Http
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(2)>
    <Nullable(0)>
    Public Class HttpRequestCodeException
        Inherits HttpRequestException

        <NullableContext(1)>
        Public Sub New(message As String, statusCode As HttpStatusCode, <Nullable(2)> response As String)
            MyBase.New(message)
        End Sub

        Public ReadOnly Property StatusCode As HttpStatusCode = statusCode

        Public ReadOnly Property Response As String = response
    End Class
End Namespace
