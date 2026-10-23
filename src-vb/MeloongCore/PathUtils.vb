Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports MeloongCore.Extensions
Imports Microsoft.Win32.SafeHandles

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module PathUtils
        Public Function AddSlashSuffix(folder As String) As String
            Dim text As String = If((folder.EndsWithF(Path.DirectorySeparatorChar, False) OrElse folder.EndsWithF(Path.AltDirectorySeparatorChar, False)), "", Path.DirectorySeparatorChar)
            Return folder + If((text IsNot Nothing), text.ToString(), Nothing)
        End Function

        Public Function RemoveSlashSuffix(folder As String) As String
            Return folder.TrimEnd(New Char() { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar })
        End Function

        Public Function ToShortPath(pathName As String) As String
            If String.IsNullOrEmpty(pathName) Then
                Return pathName
            End If
            If Not pathName.Contains(":") OrElse pathName.Contains("://") Then
                Return pathName
            End If
            If pathName.Length <= 220 Then
                Return pathName
            End If
            pathName = PathUtils.ForCompare(pathName)
            Dim text As String = ""
            Dim text2 As String = pathName
            If FileUtils.Exists(pathName) Then
                Dim lastPart As String = PathUtils.GetLastPart(pathName)
                If lastPart.Length >= 180 Then
                    Dim flag As Boolean
                    Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel)(17, 1, flag)
                    If flag Then
                        logInterpolatedStringHandler.AppendLiteral("文件名过长，将为文件名生成短路径：")
                        logInterpolatedStringHandler.AppendFormatted(pathName)
                    End If
                    Logger.Warn(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\PathUtils.cs")
                Else
                    text = lastPart
                    text2 = PathUtils.RemoveLastPart(pathName)
                End If
            End If
            While Not DirectoryUtils.Exists(text2) AndAlso Not FileUtils.Exists(text2)
                Dim directoryName As String = Path.GetDirectoryName(text2)
                If String.IsNullOrEmpty(directoryName) OrElse PathUtils.IsSame(directoryName, text2) Then
                    Return pathName
                End If
                text = Path.Combine(PathUtils.GetLastPart(text2), text)
                text2 = directoryName
            End While
            If text.Length >= 200 Then
                Dim flag As Boolean
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel)(32, 2, flag)
                If flag Then
                    logInterpolatedStringHandler2.AppendLiteral("路径中的大部分内容都尚未建立，无法在内容不存在时缩短路径：")
                    logInterpolatedStringHandler2.AppendFormatted(pathName)
                    logInterpolatedStringHandler2.AppendLiteral(" → ")
                    logInterpolatedStringHandler2.AppendFormatted(text2)
                End If
                Logger.Warn(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\PathUtils.cs")
                Return pathName
            End If
            Dim <>9__2_ As Func(Of String, String) = PathUtils.<>c.<>9__2_0
            Dim func As Func(Of String, String) = <>9__2_
            If <>9__2_ Is Nothing Then
                Dim func2 As Func(Of String, String) = Function(pathName As String)
                    Dim array As Char() = New Char(219) {}
                    Dim shortPathNameW As Integer = PathUtils.GetShortPathNameW(PathUtils.ForApi(pathName), array, array.Length)
                    If shortPathNameW <= 220 AndAlso shortPathNameW <> 0 Then
                        Return New String(array, 0, shortPathNameW)
                    End If
                    Return pathName
                End Function
                func = func2
                PathUtils.<>c.<>9__2_0 = func2
            End If
            Dim func3 As Func(Of String, String) = func
            Dim text3 As String = func3(text2)
            If Path.Combine(text3, text).Length > 220 Then
                Dim flag As Boolean
                Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.WarnLogLevel)(24, 3, flag)
                If flag Then
                    logInterpolatedStringHandler3.AppendFormatted("GetShortPathNameW")
                    logInterpolatedStringHandler3.AppendLiteral(" 未能有效缩短路径，将尝试生成新的短名称：")
                    logInterpolatedStringHandler3.AppendFormatted(text2)
                    logInterpolatedStringHandler3.AppendLiteral(" → ")
                    logInterpolatedStringHandler3.AppendFormatted(text3)
                End If
                Logger.Warn(logInterpolatedStringHandler3, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\PathUtils.cs")
                Dim list As List(Of ValueTuple(Of String, String)) = New List(Of ValueTuple(Of String, String))()
                Dim text4 As String = text2
                While Not String.IsNullOrEmpty(text4)
                    Dim lastPart2 As String = PathUtils.GetLastPart(text4)
                    If lastPart2.Length > 12 AndAlso String.Equals(PathUtils.GetLastPart(func3(text4)), lastPart2, StringComparison.OrdinalIgnoreCase) Then
                        list.Add(New ValueTuple(Of String, String)(text4, lastPart2))
                    End If
                    text4 = Path.GetDirectoryName(text4)
                End While
                Dim enumerable As IEnumerable(Of ValueTuple(Of String, String)) = list
                Dim <>9__2_2 As Func(Of ValueTuple(Of String, String), Integer) = PathUtils.<>c.<>9__2_1
                Dim func4 As Func(Of ValueTuple(Of String, String), Integer) = <>9__2_2
                If <>9__2_2 Is Nothing Then
                    Dim func5 As Func(Of ValueTuple(Of String, String), Integer) = Function(<TupleElementNames(New String() { "Path", "Name" })> candidate As ValueTuple(Of String, String)) candidate.Item2.Length
                    func4 = func5
                    PathUtils.<>c.<>9__2_1 = func5
                End If
                For Each valueTuple As ValueTuple(Of String, String) In enumerable.OrderByDescending(func4)
                    Using safeFileHandle As SafeFileHandle = PathUtils.CreateFileW(PathUtils.ForApi(valueTuple.Item1), 1073807360UI, 7UI, IntPtr.Zero, 3UI, 33554432UI, IntPtr.Zero)
                        If Not safeFileHandle.IsInvalid Then
                            Dim flag2 As Boolean = False
                            Try
                                Try
                                    ' Emulating fixed-Statement, might not be entirely correct!
                                    Dim text5 As GCHandle = GCHandle.Alloc(valueTuple.Item2, GCHandleType.Pinned)
                                    Try
                                        Dim ptr As __Pointer(Of Char) = text5.AddrOfPinnedObject()
                                        If ptr.AddrOfPinnedObject() IsNot Nothing Then
                                            ptr.AddrOfPinnedObject() += RuntimeHelpers.OffsetToStringData / 2
                                        End If
                                        Dim ptr2 As __Pointer(Of Char) = __StackAlloc.AddrOfPinnedObject()(Char, CType(24, UIntPtr))
                                        Dim unicodeString As PathUtils.UnicodeString = New PathUtils.UnicodeString() With { .Length = CUShort((valueTuple.AddrOfPinnedObject().Item2.Length * 2)), .MaximumLength = CUShort(((valueTuple.AddrOfPinnedObject().Item2.Length + 1) * 2)), .Buffer = ptr.AddrOfPinnedObject() }
                                        Dim unicodeString2 As PathUtils.UnicodeString = New PathUtils.UnicodeString() With { .MaximumLength = 24US, .Buffer = ptr2.AddrOfPinnedObject() }
                                        Dim generateNameContext As PathUtils.GenerateNameContext = Nothing
                                        For i As Integer = 0 To 20 - 1
                                            unicodeString2.AddrOfPinnedObject().Length = 0US
                                            If PathUtils.RtlGenerate8dot3Name(unicodeString.AddrOfPinnedObject(), 0, generateNameContext.AddrOfPinnedObject(), unicodeString2.AddrOfPinnedObject()) <> 0 Then
                                                Exit For
                                            End If
                                            Dim text6 As String = New String(unicodeString2.AddrOfPinnedObject().Buffer, 0, CInt((unicodeString2.AddrOfPinnedObject().Length / 2US)))
                                            If PathUtils.SetFileShortNameW(safeFileHandle.AddrOfPinnedObject(), text6.AddrOfPinnedObject()) Then
                                                flag2.AddrOfPinnedObject() = True
                                                Exit For
                                            End If
                                            If Marshal.GetLastWin32Error() <> 183 Then
                                                Exit For
                                            End If
                                        Next
                                    Finally
                                        text5.Free()
                                    End Try
                                Finally
                                    Dim text5 As String = Nothing
                                End Try
                            Catch ex As EntryPointNotFoundException
                            End Try
                            If flag2 Then
                                text3 = func3(text2)
                                If Path.Combine(text3, text).Length <= 220 Then
                                    Exit For
                                End If
                            End If
                        End If
                    End Using
                Next
            End If
            Return PathUtils.RemoveSlashSuffix(PathUtils.RemoveExtendedPrefix(Path.Combine(text3, text)))
        End Function

        Private Declare Unicode Function GetShortPathNameW Lib "kernel32.dll" (lpszLongPath As String, <Nullable(2)> <Out()> buffer As Char(), cchBuffer As Integer) As Integer

        Private Declare Unicode Function CreateFileW Lib "kernel32.dll" (fileName As String, desiredAccess As UInteger, shareMode As UInteger, securityAttributes As IntPtr, creationDisposition As UInteger, flagsAndAttributes As UInteger, templateFile As IntPtr) As SafeFileHandle

        Private Declare Unicode Function SetFileShortNameW Lib "kernel32.dll" (file As SafeFileHandle, shortName As String) As Boolean

        Private Declare Function RtlGenerate8dot3Name Lib "ntdll.dll" (ByRef name As PathUtils.UnicodeString, allowExtendedCharacters As Byte, ByRef context As PathUtils.GenerateNameContext, ByRef name8dot3 As PathUtils.UnicodeString) As Integer

        Public Function ToExtendedFormat(pathName As String) As String
            If String.IsNullOrWhiteSpace(pathName) Then
                Return pathName
            End If
            If Not pathName.Contains(":") Then
                Return pathName
            End If
            If pathName.StartsWithF("\\?\", False) Then
                Return pathName
            End If
            pathName = PathUtils.RemoveSlashSuffix(pathName).Replace("/"c, "\"c)
            If pathName.StartsWithF("\\", False) Then
                Return "\\?\UNC\" + pathName.Substring(2)
            End If
            Return "\\?\" + pathName
        End Function

        Public Function RemoveExtendedPrefix(pathName As String) As String
            If pathName.StartsWithF("\\?\UNC\", False) Then
                Return "\\" + pathName.AfterLast("\\?\UNC\", False)
            End If
            If pathName.StartsWithF("\\?\", False) Then
                Return pathName.AfterLast("\\?\", False)
            End If
            Return pathName
        End Function

        Public Function RemoveLastPart(pathName As String) As String
            If pathName.Contains("://") Then
                pathName = PathUtils.RemoveSlashSuffix(pathName.BeforeFirst("#", False).BeforeFirst("?", False))
            End If
            Return PathUtils.RemoveSlashSuffix(pathName).BeforeLastOfAny(New <>z__ReadOnlyArray(Of String)(New String() { "\", "/" }), False)
        End Function

        Public Function GetLastPart(pathName As String) As String
            If pathName.Contains("://") Then
                pathName = PathUtils.RemoveSlashSuffix(pathName.BeforeFirst("#", False).BeforeFirst("?", False))
            Else
                pathName = PathUtils.RemoveSlashSuffix(pathName)
            End If
            Return pathName.AfterLastOfAny(New <>z__ReadOnlyArray(Of String)(New String() { "\", "/" }), False)
        End Function

        Public Function GetFileNameWithoutExtension(pathName As String) As String
            Return PathUtils.GetLastPart(pathName).BeforeLast(".", False)
        End Function

        Public Function GetExtension(pathName As String) As String
            Dim lastPart As String = PathUtils.GetLastPart(pathName)
            If Not lastPart.Contains(".") Then
                Return ""
            End If
            Return lastPart.AfterLast(".", False).Lower()
        End Function

        <NullableContext(2)>
        Public Function GetDiskName(pathName As String) As Char?
            If String.IsNullOrEmpty(pathName) Then
                Return Nothing
            End If
            pathName = PathUtils.RemoveExtendedPrefix(pathName)
            If pathName.StartsWithF("\\", False) OrElse pathName.StartsWithF("/", False) Then
                Return Nothing
            End If
            Return New Char?(Char.ToUpper(pathName(0)))
        End Function

        Public Function ForCompare(pathName As String) As String
            Dim text As String
            Try
                text = PathUtils.RemoveSlashSuffix(PathUtils.RemoveExtendedPrefix(Path.GetFullPath(pathName)).Replace("/", "\"))
            Catch ex As ArgumentException
                Throw New ArgumentException("路径不正确：" + pathName, ex)
            End Try
            Return text
        End Function

        <NullableContext(2)>
        Public Function IsSame(path1 As String, path2 As String) As Boolean
            Return PathUtils.Compare(path1, path2) = 0
        End Function

        <NullableContext(2)>
        Public Function Compare(path1 As String, path2 As String) As Integer
            If String.IsNullOrEmpty(path1) Then
                If Not String.IsNullOrEmpty(path2) Then
                    Return -1
                End If
                Return 0
            Else
                If String.IsNullOrEmpty(path2) Then
                    Return 1
                End If
                Return String.Compare(PathUtils.ForCompare(path1), PathUtils.ForCompare(path2), StringComparison.OrdinalIgnoreCase)
            End If
        End Function

        Public Function ForApi(pathName As String) As String
            pathName = PathUtils.ToExtendedFormat(pathName)
            If pathName.EndsWithF(":", False) Then
                pathName += "\"
            End If
            If pathName.EndsWithF(":\", False) Then
                Return pathName
            End If
            Return PathUtils.RemoveSlashSuffix(pathName)
        End Function

        Public Function IsParentOf(parentPath As String, childPath As String) As Boolean
            parentPath = PathUtils.ForCompare(parentPath)
            childPath = PathUtils.ForCompare(childPath)
            Return childPath.StartsWithF(parentPath + "\", True) OrElse PathUtils.IsSame(childPath, parentPath)
        End Function

        <NullableContext(0)>
        Private Structure UnicodeString
            Public Length As UShort

            Public MaximumLength As UShort

            Public Buffer As __Pointer(Of Char)
        End Structure

        <NullableContext(0)>
        Private Structure GenerateNameContext
            Public Checksum As UShort

            Public ChecksumInserted As Byte

            Public NameLength As Byte

            <FixedBuffer(GetType(Char), 8)>
            Public NameBuffer As PathUtils.GenerateNameContext.<NameBuffer>e__FixedBuffer

            Public ExtensionLength As UInteger

            <FixedBuffer(GetType(Char), 4)>
            Public ExtensionBuffer As PathUtils.GenerateNameContext.<ExtensionBuffer>e__FixedBuffer

            Public LastIndexValue As UInteger

            <CompilerGenerated()>
            <UnsafeValueType()>
            <StructLayout(LayoutKind.Sequential, Size := 8)>
            Public Structure <ExtensionBuffer>e__FixedBuffer
                Public FixedElementField As Char
            End Structure

            <CompilerGenerated()>
            <UnsafeValueType()>
            <StructLayout(LayoutKind.Sequential, Size := 16)>
            Public Structure <NameBuffer>e__FixedBuffer
                Public FixedElementField As Char
            End Structure
        End Structure
    End Module
End Namespace
