Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Newtonsoft.Json

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    <Serializable()>
    Public Class Java
        Implements IEquatable(Of Java)

        Public Sub New(folder As String, <Nullable(2)> Optional version As Version = Nothing)
        End Sub

        <JsonIgnore()>
        Public ReadOnly Property JavaExePath As String
            Get
                Return Path.Combine(Me.Folder, "java.exe")
            End Get
        End Property

        Public Function CheckAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of Boolean)
            Dim <CheckAsync>d__ As Java.<CheckAsync>d__7
            <CheckAsync>d__.<>t__builder = AsyncTaskMethodBuilder(Of Boolean).Create()
            <CheckAsync>d__.<>4__this = Me
            <CheckAsync>d__.cancellationToken = cancellationToken
            <CheckAsync>d__.<>1__state = -1
            <CheckAsync>d__.<>t__builder.Start(Of Java.<CheckAsync>d__7)(<CheckAsync>d__)
            Return <CheckAsync>d__.<>t__builder.Task
        End Function

        Public Sub InvalidateChecked()
            Me.isChecked = 0
            Me.available = Nothing
        End Sub

        <NullableContext(2)>
        Public Overloads Function Equals(other As Java) As Boolean Implements System.IEquatable(Of PCLCS.Java).Equals
            Return other IsNot Nothing AndAlso PathUtils.IsSame(Me.Folder, other.Folder)
        End Function

        <NullableContext(2)>
        Public Overrides Overloads Function Equals(obj As Object) As Boolean
            Dim java As Java = TryCast(obj, Java)
            Return java IsNot Nothing AndAlso Me.Equals(java)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return StringComparer.CurrentCultureIgnoreCase.GetHashCode(PathUtils.ForCompare(Me.Folder))
        End Function

        Public Overrides Function ToString() As String
            Dim text As String = "Java {0} ({1})：{2}"
            Dim version As Version = Me.Version
            Return String.Format(text, If(If((version IsNot Nothing), version.Major.ToString(), Nothing), "尚未检查"), Me.Version, Me.Folder)
        End Function

        <CompilerGenerated()>
        Friend Shared Function <CheckAsync>g__GetProperty|7_0(name As String, ByRef A_1 As Java.<>c__DisplayClass7_0) As <Nullable(2)> String
            Dim prefix As String = name + " = "
            Dim enumerable As IEnumerable(Of String) = A_1.output.Split(New Char() { vbLf })
            Dim <>9__7_ As Func(Of String, String) = Java.<>c.<>9__7_1
            Dim func As Func(Of String, String) = <>9__7_
            If <>9__7_ Is Nothing Then
                Dim func2 As Func(Of String, String) = Function(l As String) l.Trim()
                func = func2
                Java.<>c.<>9__7_1 = func2
            End If
            Dim text As String = enumerable.[Select](func).FirstOrDefault(Function(l As String) l.StartsWithF(prefix, False))
            If text Is Nothing Then
                Return Nothing
            End If
            Return text.Substring(prefix.Length).Trim()
        End Function

        Public Folder As String = PathUtils.AddSlashSuffix(PathUtils.ForCompare(folder))

        <Nullable(2)>
        Public Version As Version = version

        <Nullable(2)>
        <JsonIgnore()>
        Public FileEncodingName As String

        <Nullable(2)>
        <JsonIgnore()>
        Public NativeEncodingName As String

        <JsonIgnore()>
        Private isChecked As Integer

        <JsonIgnore()>
        Friend available As Boolean?
    End Class
End Namespace
