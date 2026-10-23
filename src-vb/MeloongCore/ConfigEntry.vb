Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(2)>
    <Nullable(0)>
    Public Class ConfigEntry(Of T)
        Public Sub New(<Nullable(1)> key As String, defaultValue As T, Optional encrypted As Boolean = False, Optional provider As IConfigProvider = Nothing)
        End Sub

        <Nullable(1)>
        Public ReadOnly Property Provider As IConfigProvider = If(provider, ConfigProviders.[Default])

        Public Event Changed As Action(Of T)

        <Nullable(New Byte() { 2, 1, 1, 1 })>
        <Nullable(New Byte() { 2, 1, 1, 1 })>
        Public Event PreviewGet As Action(Of PreviewGetEventArgs(Of T), IConfigProvider)

        Public Sub Edit(<Nullable(New Byte() { 1, 2, 2 })> editFunc As Func(Of T, T))
            Me.[Set](editFunc(Me.[Get]()))
        End Sub

        Public Sub Edit(<Nullable(New Byte() { 1, 0 })> editAction As ConfigEntry(Of T).RefAction)
            Dim t As T = Me.[Get]()
            editAction(t)
            Me.[Set](t)
        End Sub

        Public Sub [Set](value As T)
            Me.Provider.[Set](Of T)(Me.<key>P, value, Me.<encrypted>P)
            Dim changed As Action(Of T) = Me.Changed
            If changed Is Nothing Then
                Return
            End If
            changed(value)
        End Sub

        Public Sub Reset()
            If Not Me.Provider.HasValue(Me.<key>P) Then
                Return
            End If
            Me.Provider.Remove(Me.<key>P)
            Dim changed As Action(Of T) = Me.Changed
            If changed Is Nothing Then
                Return
            End If
            changed(Me.<defaultValue>P)
        End Sub

        Public Function [Get]() As T
            Dim previewGetEventArgs As PreviewGetEventArgs(Of T) = New PreviewGetEventArgs(Of T)(Me.Provider.Read(Of T)(Me.<key>P, Me.<defaultValue>P, Me.<encrypted>P))
            Dim previewGet As Action(Of PreviewGetEventArgs(Of T), IConfigProvider) = Me.PreviewGet
            If previewGet IsNot Nothing Then
                previewGet(previewGetEventArgs, Me.Provider)
            End If
            Return previewGetEventArgs.Value
        End Function

        Public Function HasValue() As Boolean
            Return Me.Provider.HasValue(Me.<key>P)
        End Function

        <Nullable(1)>
        <CompilerGenerated()>
        Private <key>P As String = key

        <CompilerGenerated()>
        Private <defaultValue>P As T = defaultValue

        <CompilerGenerated()>
        Private <encrypted>P As Boolean = encrypted

        <NullableContext(0)>
        Public Delegate Sub RefAction(ByRef value As T)
    End Class
End Namespace
