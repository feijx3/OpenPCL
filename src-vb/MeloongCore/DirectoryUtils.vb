Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports ThrottleDebounce

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module DirectoryUtils
        Public Sub Create(folder As String)
            If DirectoryUtils.Exists(folder) Then
                Return
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(6, 1, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("新建文件夹：")
                logInterpolatedStringHandler.AppendFormatted(folder)
            End If
            Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
            Directory.CreateDirectory(PathUtils.ForApi(folder))
        End Sub

        Public Function CreateRandom() As String
            Dim text As String = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
            DirectoryUtils.Create(text)
            Return text
        End Function

        Public Function Exists(folder As String) As Boolean
            Return Directory.Exists(PathUtils.ForApi(folder))
        End Function

        Public Function GetInfo(folder As String) As DirectoryInfo
            Return New DirectoryInfo(PathUtils.ForApi(folder))
        End Function

        Public Function EnumerateFiles(folder As String, Optional includeSubDirectories As Boolean = False, Optional searchPattern As String = "*") As IEnumerable(Of String)
            If Not DirectoryUtils.Exists(folder) Then
                Return Array.Empty(Of String)()
            End If
            Dim enumerable As IEnumerable(Of String) = Directory.EnumerateFiles(PathUtils.ForApi(folder), searchPattern, If(includeSubDirectories, SearchOption.AllDirectories, SearchOption.TopDirectoryOnly))
            Dim <0>__RemoveExtendedPrefix As Func(Of String, String) = DirectoryUtils.<>O.<0>__RemoveExtendedPrefix
            Dim func As Func(Of String, String) = <0>__RemoveExtendedPrefix
            If <0>__RemoveExtendedPrefix Is Nothing Then
                Dim func2 As Func(Of String, String) = AddressOf PathUtils.RemoveExtendedPrefix
                func = func2
                DirectoryUtils.<>O.<0>__RemoveExtendedPrefix = func2
            End If
            Return enumerable.[Select](func)
        End Function

        Public Function EnumerateDirectories(folder As String, Optional includeSubDirectories As Boolean = False, Optional searchPattern As String = "*") As IEnumerable(Of String)
            If Not DirectoryUtils.Exists(folder) Then
                Return Array.Empty(Of String)()
            End If
            Dim enumerable As IEnumerable(Of String) = Directory.EnumerateDirectories(PathUtils.ForApi(folder), searchPattern, If(includeSubDirectories, SearchOption.AllDirectories, SearchOption.TopDirectoryOnly))
            Dim <0>__RemoveExtendedPrefix As Func(Of String, String) = DirectoryUtils.<>O.<0>__RemoveExtendedPrefix
            Dim func As Func(Of String, String) = <0>__RemoveExtendedPrefix
            If <0>__RemoveExtendedPrefix Is Nothing Then
                Dim func2 As Func(Of String, String) = AddressOf PathUtils.RemoveExtendedPrefix
                func = func2
                DirectoryUtils.<>O.<0>__RemoveExtendedPrefix = func2
            End If
            Return enumerable.[Select](func)
        End Function

        Public Function IsEmpty(folder As String) As Boolean
            Return Not DirectoryUtils.Exists(folder) OrElse Not Directory.EnumerateFileSystemEntries(PathUtils.ForApi(folder)).Any()
        End Function

        Public Sub Copy(sourceFolder As String, destFolder As String)
            sourceFolder = PathUtils.ForCompare(sourceFolder)
            destFolder = PathUtils.ForCompare(destFolder)
            Dim flag As Boolean
            If String.CompareOrdinal(sourceFolder, destFolder) = 0 Then
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(18, 2, flag)
                If flag Then
                    logInterpolatedStringHandler.AppendLiteral("复制文件夹到自身，不执行操作：")
                    logInterpolatedStringHandler.AppendFormatted(sourceFolder)
                    logInterpolatedStringHandler.AppendLiteral(" → ")
                    logInterpolatedStringHandler.AppendFormatted(destFolder)
                End If
                Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
                Return
            End If
            If PathUtils.IsSame(sourceFolder, destFolder) Then
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(29, 2, flag)
                If flag Then
                    logInterpolatedStringHandler2.AppendLiteral("复制文件夹到自身，但大小写不同，等效于重命名文件夹：")
                    logInterpolatedStringHandler2.AppendFormatted(sourceFolder)
                    logInterpolatedStringHandler2.AppendLiteral(" → ")
                    logInterpolatedStringHandler2.AppendFormatted(destFolder)
                End If
                Logger.Trace(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
                DirectoryUtils.Move(sourceFolder, destFolder)
                Return
            End If
            Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(9, 2, flag)
            If flag Then
                logInterpolatedStringHandler3.AppendLiteral("复制文件夹：")
                logInterpolatedStringHandler3.AppendFormatted(sourceFolder)
                logInterpolatedStringHandler3.AppendLiteral(" → ")
                logInterpolatedStringHandler3.AppendFormatted(destFolder)
            End If
            Logger.Trace(logInterpolatedStringHandler3, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
            For Each text As String In DirectoryUtils.EnumerateFiles(sourceFolder, True, "*").ToList()
                FileUtils.Copy(text, text.Replace(sourceFolder, destFolder))
            Next
        End Sub

        Public Sub Move(sourceFolder As String, destFolder As String)
            DirectoryUtils.SafetyCheck(sourceFolder)
            sourceFolder = PathUtils.ForCompare(sourceFolder)
            destFolder = PathUtils.ForCompare(destFolder)
            Dim flag As Boolean
            If String.CompareOrdinal(sourceFolder, destFolder) = 0 Then
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(18, 2, flag)
                If flag Then
                    logInterpolatedStringHandler.AppendLiteral("剪切文件夹到自身，不执行操作：")
                    logInterpolatedStringHandler.AppendFormatted(sourceFolder)
                    logInterpolatedStringHandler.AppendLiteral(" → ")
                    logInterpolatedStringHandler.AppendFormatted(destFolder)
                End If
                Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
                Return
            End If
            If PathUtils.IsSame(sourceFolder, destFolder) Then
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(19, 2, flag)
                If flag Then
                    logInterpolatedStringHandler2.AppendLiteral("剪切文件夹到自身，但大小写不同：")
                    logInterpolatedStringHandler2.AppendFormatted(sourceFolder)
                    logInterpolatedStringHandler2.AppendLiteral(" → ")
                    logInterpolatedStringHandler2.AppendFormatted(destFolder)
                End If
                Logger.Trace(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
                Dim text As String = Path.Combine(PathUtils.RemoveLastPart(sourceFolder), Path.GetRandomFileName())
                DirectoryUtils.Move(sourceFolder, text)
                DirectoryUtils.Move(text, destFolder)
                Return
            End If
            Dim c As Char? = PathUtils.GetDiskName(sourceFolder)
            Dim num As Integer? = If((c IsNot Nothing), New Integer?(CInt(c.GetValueOrDefault())), Nothing)
            c = PathUtils.GetDiskName(destFolder)
            Dim num2 As Integer? = If((c IsNot Nothing), New Integer?(CInt(c.GetValueOrDefault())), Nothing)
            If(num.GetValueOrDefault() = num2.GetValueOrDefault()) And (num IsNot Nothing = (num2 IsNot Nothing)) Then
                Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(14, 2, flag)
                If flag Then
                    logInterpolatedStringHandler3.AppendLiteral("剪切文件夹到同一磁盘：")
                    logInterpolatedStringHandler3.AppendFormatted(sourceFolder)
                    logInterpolatedStringHandler3.AppendLiteral(" → ")
                    logInterpolatedStringHandler3.AppendFormatted(destFolder)
                End If
                Logger.Trace(logInterpolatedStringHandler3, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
                DirectoryUtils.Delete(destFolder, False)
                Dim action As Action(Of Integer) = Sub(_ As Integer)
                    FileUtils.SetReadOnly(sourceFolder, False)
                    Directory.Move(PathUtils.ForApi(sourceFolder), PathUtils.ForApi(destFolder))
                End Sub
                Dim num3 As UInteger? = New UInteger?(2UI)
                Dim <>9__8_ As Func(Of Integer, TimeSpan) = DirectoryUtils.<>c.<>9__8_1
                Dim func As Func(Of Integer, TimeSpan) = <>9__8_
                If <>9__8_ Is Nothing Then
                    Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                    func = func2
                    DirectoryUtils.<>c.<>9__8_1 = func2
                End If
                Dim <>9__8_2 As Func(Of Exception, Boolean) = DirectoryUtils.<>c.<>9__8_2
                Dim func3 As Func(Of Exception, Boolean) = <>9__8_2
                If <>9__8_2 Is Nothing Then
                    Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                    func3 = func4
                    DirectoryUtils.<>c.<>9__8_2 = func4
                End If
                Retrier.Attempt(action, num3, func, func3, Nothing, Nothing)
                Return
            End If
            Dim logInterpolatedStringHandler4 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(14, 2, flag)
            If flag Then
                logInterpolatedStringHandler4.AppendLiteral("剪切文件夹到不同磁盘：")
                logInterpolatedStringHandler4.AppendFormatted(sourceFolder)
                logInterpolatedStringHandler4.AppendLiteral(" → ")
                logInterpolatedStringHandler4.AppendFormatted(destFolder)
            End If
            Logger.Trace(logInterpolatedStringHandler4, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
            DirectoryUtils.Copy(sourceFolder, destFolder)
            DirectoryUtils.Delete(sourceFolder, False)
        End Sub

        Public Function Delete(folder As String, Optional toRecycleBin As Boolean = False) As Boolean?
            If Not DirectoryUtils.Exists(folder) Then
                Return Nothing
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(1, 2, flag)
            If flag Then
                logInterpolatedStringHandler.AppendFormatted(If(toRecycleBin, "将文件夹删除到回收站", "删除文件夹"))
                logInterpolatedStringHandler.AppendLiteral("：")
                logInterpolatedStringHandler.AppendFormatted(folder)
            End If
            Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
            DirectoryUtils.SafetyCheck(folder)
            If toRecycleBin Then
                Try
                    FileUtils.DeleteToRecycleBin(folder)
                    Return New Boolean?(True)
                Catch ex As Exception
                    Logger.Warn(ex, "无法将文件夹删除到回收站，回退到永久删除：" + folder, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
                End Try
            End If
            DirectoryUtils.<Delete>g__DeleteInternal|9_0(folder)
            Return New Boolean?(False)
        End Function

        Private Sub SafetyCheck(folder As String)
            folder = PathUtils.ForCompare(folder)
            If PathUtils.IsSame(folder, Path.GetPathRoot(folder)) Then
                Throw New UnauthorizedAccessException("不应操作磁盘根目录：" + folder)
            End If
            If DirectoryUtils.criticalFolders.Value.Any(Function(f As String) PathUtils.IsParentOf(folder, f)) Then
                Throw New UnauthorizedAccessException("不应操作文件夹：" + folder)
            End If
        End Sub

        <CompilerGenerated()>
        Friend Sub <Delete>g__DeleteInternal|9_0(folder As String)
            Try
                folder = PathUtils.ForApi(folder)
                For Each text As String In DirectoryUtils.EnumerateFiles(folder, False, "*")
                    FileUtils.Delete(text, False)
                Next
                For Each text2 As String In DirectoryUtils.EnumerateDirectories(folder, False, "*")
                    DirectoryUtils.<Delete>g__DeleteInternal|9_0(text2)
                Next
                Dim action As Action(Of Integer) = Sub(_ As Integer)
                    FileUtils.SetReadOnly(folder, False)
                    Directory.Delete(folder, True)
                End Sub
                Dim num As UInteger? = New UInteger?(2UI)
                Dim <>9__9_ As Func(Of Integer, TimeSpan) = DirectoryUtils.<>c.<>9__9_2
                Dim func As Func(Of Integer, TimeSpan) = <>9__9_
                If <>9__9_ Is Nothing Then
                    Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                    func = func2
                    DirectoryUtils.<>c.<>9__9_2 = func2
                End If
                Dim <>9__9_2 As Func(Of Exception, Boolean) = DirectoryUtils.<>c.<>9__9_3
                Dim func3 As Func(Of Exception, Boolean) = <>9__9_2
                If <>9__9_2 Is Nothing Then
                    Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                    func3 = func4
                    DirectoryUtils.<>c.<>9__9_3 = func4
                End If
                Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
            Catch ex As DirectoryNotFoundException
                If Not DirectoryUtils.Exists(folder) Then
                    Throw
                End If
                Dim ex2 As DirectoryNotFoundException
                Logger.Warn(ex2, "该文件夹可能为孤立的符号链接，尝试直接删除（" + folder + "）", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\DirectoryUtils.cs")
                FileUtils.SetReadOnly(folder, False)
                Directory.Delete(folder)
            End Try
        End Sub

        Private criticalFolders As Lazy(Of HashSet(Of String)) = New Lazy(Of HashSet(Of String))(Function()
            Dim enumerable As IEnumerable(Of String) = New String() { Paths.Base, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Environment.GetFolderPath(Environment.SpecialFolder.Personal), Environment.GetFolderPath(Environment.SpecialFolder.Windows), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads") }
            Dim <>9__12_ As Func(Of String, Boolean) = DirectoryUtils.<>c.<>9__12_1
            Dim func As Func(Of String, Boolean) = <>9__12_
            If <>9__12_ Is Nothing Then
                Dim func2 As Func(Of String, Boolean) = Function(f As String) f.Contains(Path.VolumeSeparatorChar)
                func = func2
                DirectoryUtils.<>c.<>9__12_1 = func2
            End If
            Return New HashSet(Of String)(enumerable.Where(func))
        End Function)

        <CompilerGenerated()>
        Private NotInheritable Class <>O
            <Nullable(0)>
            Public Shared <0>__RemoveExtendedPrefix As Func(Of String, String)
        End Class
    End Module
End Namespace
