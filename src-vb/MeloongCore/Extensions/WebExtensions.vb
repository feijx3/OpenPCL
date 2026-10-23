Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks

Namespace MeloongCore.Extensions
    <NullableContext(1)>
    <Nullable(0)>
    Public Module WebExtensions
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Clone(request As HttpRequestMessage) As HttpRequestMessage
            Dim httpRequestMessage As HttpRequestMessage = New HttpRequestMessage(request.Method, request.RequestUri) With { .Content = request.Content.Clone(), .Version = request.Version }
            For Each keyValuePair As KeyValuePair(Of String, Object) In request.Properties
                httpRequestMessage.Properties.Add(keyValuePair)
            Next
            For Each keyValuePair2 As KeyValuePair(Of String, IEnumerable(Of String)) In request.Headers
                httpRequestMessage.Headers.TryAddWithoutValidation(keyValuePair2.Key, keyValuePair2.Value)
            Next
            Return httpRequestMessage
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Async Function CloneAsync(request As HttpRequestMessage) As Task(Of HttpRequestMessage)
            Dim httpRequestMessage As HttpRequestMessage = New HttpRequestMessage(request.Method, request.RequestUri)
            Dim httpRequestMessage2 As HttpRequestMessage = httpRequestMessage
            Dim httpContent As HttpContent = Await request.Content.CloneAsync().NoCapture()
            httpRequestMessage2.Content = httpContent
            httpRequestMessage.Version = request.Version
            Dim httpRequestMessage3 As HttpRequestMessage = httpRequestMessage
            httpRequestMessage2 = Nothing
            httpRequestMessage = Nothing
            For Each keyValuePair As KeyValuePair(Of String, Object) In request.Properties
                httpRequestMessage3.Properties.Add(keyValuePair)
            Next
            For Each keyValuePair2 As KeyValuePair(Of String, IEnumerable(Of String)) In request.Headers
                httpRequestMessage3.Headers.TryAddWithoutValidation(keyValuePair2.Key, keyValuePair2.Value)
            Next
            Return httpRequestMessage3
        End Function

        <NullableContext(2)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Clone(content As HttpContent) As HttpContent
            If content Is Nothing Then
                Return Nothing
            End If
            Dim memoryStream As MemoryStream = New MemoryStream()
            content.CopyToAsync(memoryStream).Wait()
            memoryStream.Position = 0L
            Dim streamContent As StreamContent = New StreamContent(memoryStream)
            For Each keyValuePair As KeyValuePair(Of String, IEnumerable(Of String)) In content.Headers
                streamContent.Headers.Add(keyValuePair.Key, keyValuePair.Value)
            Next
            Return streamContent
        End Function

        <NullableContext(2)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Async Function CloneAsync(content As HttpContent) As <Nullable(New Byte() { 1, 2 })> Task(Of HttpContent)
            Dim httpContent As HttpContent
            If content Is Nothing Then
                httpContent = Nothing
            Else
                Dim ms As MemoryStream = New MemoryStream()
                Await content.CopyToAsync(ms).NoCapture()
                ms.Position = 0L
                Dim streamContent As StreamContent = New StreamContent(ms)
                For Each keyValuePair As KeyValuePair(Of String, IEnumerable(Of String)) In content.Headers
                    streamContent.Headers.Add(keyValuePair.Key, keyValuePair.Value)
                Next
                httpContent = streamContent
            End If
            Return httpContent
        End Function
    End Module
End Namespace
