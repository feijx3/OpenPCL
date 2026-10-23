Imports System
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module MsDelta
        Public Sub Create(oldFilePath As String, newFilePath As String, deltaFilePath As String)
            If Not FileUtils.Exists(oldFilePath) Then
                Throw New FileNotFoundException("旧文件不存在：" + oldFilePath, oldFilePath)
            End If
            If Not FileUtils.Exists(newFilePath) Then
                Throw New FileNotFoundException("新文件不存在：" + newFilePath, newFilePath)
            End If
            DirectoryUtils.Create(PathUtils.RemoveLastPart(deltaFilePath))
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(20, 3, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("正在生成补丁：")
                logInterpolatedStringHandler.AppendFormatted(oldFilePath)
                logInterpolatedStringHandler.AppendLiteral(" → ")
                logInterpolatedStringHandler.AppendFormatted(newFilePath)
                logInterpolatedStringHandler.AppendLiteral("，补丁文件将输出到：")
                logInterpolatedStringHandler.AppendFormatted(deltaFilePath)
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\MsDelta.cs")
            If Not MsDelta.CreateDelta(15L, 131072L, 0L, PathUtils.ForApi(oldFilePath), PathUtils.ForApi(newFilePath), Nothing, Nothing, Nothing, IntPtr.Zero, 32UI, PathUtils.ForApi(deltaFilePath)) Then
                Throw New Win32Exception()
            End If
        End Sub

        Public Sub Apply(oldFilePath As String, deltaFilePath As String, newFilePath As String)
            If Not FileUtils.Exists(oldFilePath) Then
                Throw New FileNotFoundException("旧文件不存在：" + oldFilePath, oldFilePath)
            End If
            If Not FileUtils.Exists(deltaFilePath) Then
                Throw New FileNotFoundException("补丁文件不存在：" + deltaFilePath, deltaFilePath)
            End If
            DirectoryUtils.Create(PathUtils.RemoveLastPart(newFilePath))
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(27, 3, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("正在应用补丁：原始文件 ")
                logInterpolatedStringHandler.AppendFormatted(oldFilePath)
                logInterpolatedStringHandler.AppendLiteral(" + 补丁文件 ")
                logInterpolatedStringHandler.AppendFormatted(deltaFilePath)
                logInterpolatedStringHandler.AppendLiteral(" → 输出至 ")
                logInterpolatedStringHandler.AppendFormatted(newFilePath)
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\General\MsDelta.cs")
            If Not MsDelta.ApplyDelta(1L, PathUtils.ForApi(oldFilePath), PathUtils.ForApi(deltaFilePath), PathUtils.ForApi(newFilePath)) Then
                Throw New Win32Exception()
            End If
        End Sub

        Private Declare Unicode Function CreateDelta Lib "msdelta.dll" (fileTypeSet As Long, setFlags As Long, resetFlags As Long, sourceName As String, targetName As String, <Nullable(2)> sourceOptionsName As String, <Nullable(2)> targetOptionsName As String, globalOptions As MsDelta.DeltaInput, targetFileTime As IntPtr, hashAlgId As UInteger, deltaName As String) As <MarshalAs(UnmanagedType.Bool)> Boolean

        Private Declare Unicode Function ApplyDelta Lib "msdelta.dll" (applyFlags As Long, sourceName As String, deltaName As String, targetName As String) As <MarshalAs(UnmanagedType.Bool)> Boolean

        <NullableContext(0)>
        Private Structure DeltaInput
            Public Start As IntPtr

            Public Size As IntPtr

            <MarshalAs(UnmanagedType.Bool)>
            Public Editable As Boolean
        End Structure
    End Module
End Namespace
