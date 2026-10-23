Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Class DynamicConfigEntry(Of T)
        Public Sub New(key As String, <Nullable(2)> defaultValue As T, Optional encrypted As Boolean = False)
        End Sub

        <Nullable(2)>
        <NullableContext(2)>
        <Nullable(2)>
        Public Event Changed As Action(Of T)

        <Nullable(New Byte() { 2, 1, 1, 1 })>
        <Nullable(New Byte() { 2, 1, 1, 1 })>
        Public Event PreviewGet As Action(Of PreviewGetEventArgs(Of T), IConfigProvider)

        Public Sub Edit(<Nullable(New Byte() { 1, 2, 2 })> editFunc As Func(Of T, T), provider As IConfigProvider)
            Me.[Set](editFunc(Me.[Get](provider)), provider)
        End Sub

        Public Sub Edit(<Nullable(New Byte() { 1, 0 })> editAction As DynamicConfigEntry(Of T).RefAction, provider As IConfigProvider)
            Dim t As T = Me.[Get](provider)
            editAction(t)
            Me.[Set](t, provider)
        End Sub

        Public Sub [Set](<Nullable(2)> value As T, provider As IConfigProvider)
            provider.[Set](Of T)(Me.<key>P, value, Me.<encrypted>P)
            Dim changed As Action(Of T) = Me.Changed
            If changed Is Nothing Then
                Return
            End If
            changed(value)
        End Sub

        Public Sub Reset(provider As IConfigProvider)
            If Not provider.HasValue(Me.<key>P) Then
                Return
            End If
            provider.Remove(Me.<key>P)
            Dim changed As Action(Of T) = Me.Changed
            If changed Is Nothing Then
                Return
            End If
            changed(Me.<defaultValue>P)
        End Sub

        Public Function [Get](provider As IConfigProvider) As <Nullable(2)> T
            Dim previewGetEventArgs As PreviewGetEventArgs(Of T) = New PreviewGetEventArgs(Of T)(provider.Read(Of T)(Me.<key>P, Me.<defaultValue>P, Me.<encrypted>P))
            Dim previewGet As Action(Of PreviewGetEventArgs(Of T), IConfigProvider) = Me.PreviewGet
            If previewGet IsNot Nothing Then
                previewGet(previewGetEventArgs, provider)
            End If
            Return previewGetEventArgs.Value
        End Function

        Public Function HasValue(provider As IConfigProvider) As Boolean
            Return provider.HasValue(Me.<key>P)
        End Function

        <CompilerGenerated()>
        Private <key>P As String = key

        <Nullable(2)>
        <CompilerGenerated()>
        Private <defaultValue>P As T = defaultValue

        <CompilerGenerated()>
        Private <encrypted>P As Boolean = encrypted

        <NullableContext(0)>
        Public Delegate Sub RefAction(ByRef value As T)
    End Class
End Namespace
