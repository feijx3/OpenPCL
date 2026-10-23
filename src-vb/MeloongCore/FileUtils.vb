Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.ExceptionServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports MeloongCore.Extensions
Imports Newtonsoft.Json.Linq
Imports ThrottleDebounce

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module FileUtils
        Public Function ReadAsStream(filePath As String, <Nullable(2)> Optional type As Type = Nothing) As Stream
            If type Is Nothing Then
                Dim func As Func(Of Integer, FileStream) = Function(_ As Integer) New FileStream(PathUtils.ForApi(filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim num As UInteger? = New UInteger?(2UI)
                Dim <>9__0_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__0_1
                Dim func2 As Func(Of Integer, TimeSpan) = <>9__0_
                If <>9__0_ Is Nothing Then
                    Dim func3 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                    func2 = func3
                    FileUtils.<>c.<>9__0_1 = func3
                End If
                Dim <>9__0_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__0_2
                Dim func4 As Func(Of Exception, Boolean) = <>9__0_2
                If <>9__0_2 Is Nothing Then
                    Dim func5 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                    func4 = func5
                    FileUtils.<>c.<>9__0_2 = func5
                End If
                Return Retrier.Attempt(Of FileStream)(func, num, func2, func4, Nothing, Nothing)
            End If
            Dim manifestResourceStream As Stream = type.Assembly.GetManifestResourceStream(filePath)
            If manifestResourceStream Is Nothing Then
                Throw New FileNotFoundException("Embedded resource """ + filePath + """ was not found.")
            End If
            Return manifestResourceStream
        End Function

        Public Function ReadAsBytes(filePath As String, <Nullable(2)> Optional type As Type = Nothing) As Byte()
            Dim array As Byte()
            Using stream As Stream = FileUtils.ReadAsStream(filePath, type)
                Using memoryStream As MemoryStream = New MemoryStream()
                    stream.CopyTo(memoryStream)
                    array = memoryStream.ToArray()
                End Using
            End Using
            Return array
        End Function

        Public Function ReadAsString(filePath As String, <Nullable(2)> Optional encoding As Encoding = Nothing, <Nullable(2)> Optional type As Type = Nothing) As String
            Return FileUtils.ReadAsBytes(filePath, type).GetString(encoding)
        End Function

        <NullableContext(2)>
        Public Function TryReadAsString(<Nullable(1)> filePath As String, Optional encoding As Encoding = Nothing, Optional type As Type = Nothing) As String
            Dim text As String
            Try
                If type Is Nothing AndAlso Not FileUtils.Exists(filePath) Then
                    text = Nothing
                Else
                    text = FileUtils.ReadAsBytes(filePath, type).GetString(encoding)
                End If
            Catch
                text = Nothing
            End Try
            Return text
        End Function

        Public Function ReadAsLines(filePath As String, Optional skipEmptyLines As Boolean = False, <Nullable(2)> Optional encoding As Encoding = Nothing, <Nullable(2)> Optional type As Type = Nothing) As String()
            Return FileUtils.ReadAsString(filePath, encoding, type).SplitLines(skipEmptyLines)
        End Function

        Public Function TryReadAsLines(filePath As String, Optional skipEmptyLines As Boolean = False, <Nullable(2)> Optional encoding As Encoding = Nothing, <Nullable(2)> Optional type As Type = Nothing) As String()
            Dim text As String = FileUtils.TryReadAsString(filePath, encoding, type)
            Return If(If((text IsNot Nothing), text.SplitLines(skipEmptyLines), Nothing), Array.Empty(Of String)())
        End Function

        <NullableContext(2)>
        Public Function ReadAsJson(<Nullable(1)> filePath As String, Optional encoding As Encoding = Nothing, Optional type As Type = Nothing) As JToken
            Return FileUtils.ReadAsString(filePath, encoding, type).DeserializeJson()
        End Function

        <NullableContext(2)>
        Public Function ReadAsJson(Of T As JToken)(<Nullable(1)> filePath As String, Optional encoding As Encoding = Nothing, Optional type As Type = Nothing) As T
            Return FileUtils.ReadAsString(filePath, encoding, type).DeserializeJson()
        End Function

        Public Sub Write(filePath As String, text As String, <Nullable(2)> Optional encoding As Encoding = Nothing)
            FileUtils.Write(filePath, (If(encoding, New UTF8Encoding())).GetBytes(text))
        End Sub

        Public Sub Write(filePath As String, content As IEnumerable(Of Byte))
            FileUtils.Write(filePath, content.ToArray())
        End Sub

        Public Sub Write(filePath As String, content As Byte())
            DirectoryUtils.Create(PathUtils.RemoveLastPart(filePath))
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(10, 2, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("写入文件：")
                logInterpolatedStringHandler.AppendFormatted(filePath)
                logInterpolatedStringHandler.AppendLiteral("（")
                logInterpolatedStringHandler.AppendFormatted(Of Integer)(content.Length)
                logInterpolatedStringHandler.AppendLiteral(" 字节）")
            End If
            Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            Dim action As Action(Of Integer) = Sub(_ As Integer)
                File.WriteAllBytes(PathUtils.ForApi(filePath), content)
            End Sub
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__10_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__10_1
            Dim func As Func(Of Integer, TimeSpan) = <>9__10_
            If <>9__10_ Is Nothing Then
                Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func = func2
                FileUtils.<>c.<>9__10_1 = func2
            End If
            Dim <>9__10_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__10_2
            Dim func3 As Func(Of Exception, Boolean) = <>9__10_2
            If <>9__10_2 Is Nothing Then
                Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func3 = func4
                FileUtils.<>c.<>9__10_2 = func4
            End If
            Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
        End Sub

        Public Sub WriteAtomic(filePath As String, text As String, <Nullable(2)> Optional encoding As Encoding = Nothing)
            FileUtils.WriteAtomic(filePath, (If(encoding, New UTF8Encoding())).GetBytes(text))
        End Sub

        Public Sub WriteAtomic(filePath As String, content As Byte())
            Dim text As String = String.Format("{0}.{1:N}.tmp", filePath, Guid.NewGuid())
            Try
                FileUtils.Write(text, content)
                If FileUtils.Exists(filePath) Then
                    File.Replace(PathUtils.ForApi(text), PathUtils.ForApi(filePath), Nothing, True)
                Else
                    File.Move(PathUtils.ForApi(text), PathUtils.ForApi(filePath))
                End If
            Finally
                FileUtils.Delete(text, False)
            End Try
        End Sub

        Public Sub Write(filePath As String, stream As Stream)
            Dim action As Action(Of Integer) = Sub(_ As Integer)
                FileUtils.Delete(filePath, False)
                Using fileStream As FileStream = FileUtils.CreateAsStream(filePath)
                    If stream.CanSeek AndAlso stream.Position <> 0L Then
                        stream.Seek(0L, SeekOrigin.Begin)
                    End If
                    Dim flag As Boolean
                    Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(7, 3, flag)
                    If flag Then
                        logInterpolatedStringHandler.AppendLiteral("写入文件：")
                        logInterpolatedStringHandler.AppendFormatted(filePath)
                        logInterpolatedStringHandler.AppendLiteral("（")
                        logInterpolatedStringHandler.AppendFormatted(stream.[GetType]().Name)
                        logInterpolatedStringHandler.AppendFormatted(If(stream.CanSeek, String.Format(" {0} 字节", stream.Length), ""))
                        logInterpolatedStringHandler.AppendLiteral("）")
                    End If
                    Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                    stream.CopyTo(fileStream)
                End Using
            End Sub
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__13_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__13_1
            Dim func As Func(Of Integer, TimeSpan) = <>9__13_
            If <>9__13_ Is Nothing Then
                Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func = func2
                FileUtils.<>c.<>9__13_1 = func2
            End If
            Dim <>9__13_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__13_2
            Dim func3 As Func(Of Exception, Boolean) = <>9__13_2
            If <>9__13_2 Is Nothing Then
                Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func3 = func4
                FileUtils.<>c.<>9__13_2 = func4
            End If
            Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
        End Sub

        Public Function CreateAsStream(filePath As String) As FileStream
            DirectoryUtils.Create(PathUtils.RemoveLastPart(filePath))
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(6, 1, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("创建文件流：")
                logInterpolatedStringHandler.AppendFormatted(filePath)
            End If
            Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            Dim func As Func(Of Integer, FileStream) = Function(_ As Integer)
                FileUtils.Delete(filePath, False)
                Return New FileStream(PathUtils.ForApi(filePath), FileMode.Create)
            End Function
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__14_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__14_1
            Dim func2 As Func(Of Integer, TimeSpan) = <>9__14_
            If <>9__14_ Is Nothing Then
                Dim func3 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func2 = func3
                FileUtils.<>c.<>9__14_1 = func3
            End If
            Dim <>9__14_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__14_2
            Dim func4 As Func(Of Exception, Boolean) = <>9__14_2
            If <>9__14_2 Is Nothing Then
                Dim func5 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func4 = func5
                FileUtils.<>c.<>9__14_2 = func5
            End If
            Return Retrier.Attempt(Of FileStream)(func, num, func2, func4, Nothing, Nothing)
        End Function

        Public Function CreateRandom() As String
            Dim <>9__15_ As Func(Of Integer, String) = FileUtils.<>c.<>9__15_0
            Dim func As Func(Of Integer, String) = <>9__15_
            If <>9__15_ Is Nothing Then
                Dim func2 As Func(Of Integer, String) = Function(_ As Integer) Path.GetTempFileName()
                func = func2
                FileUtils.<>c.<>9__15_0 = func2
            End If
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__15_2 As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__15_1
            Dim func3 As Func(Of Integer, TimeSpan) = <>9__15_2
            If <>9__15_2 Is Nothing Then
                Dim func4 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func3 = func4
                FileUtils.<>c.<>9__15_1 = func4
            End If
            Dim <>9__15_3 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__15_2
            Dim func5 As Func(Of Exception, Boolean) = <>9__15_3
            If <>9__15_3 Is Nothing Then
                Dim func6 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func5 = func6
                FileUtils.<>c.<>9__15_2 = func6
            End If
            Return Retrier.Attempt(Of String)(func, num, func3, func5, Nothing, Nothing)
        End Function

        Public Sub ExtractResources(logicalName As String, localFilePath As String, type As Type)
            Dim array As Byte() = FileUtils.ReadAsBytes(logicalName, type)
            If FileUtils.Exists(localFilePath) AndAlso CryptographyUtils.ComputeFileHash(localFilePath, CryptographyUtils.HashMethod.Md5) = CryptographyUtils.ComputeHash(array, CryptographyUtils.HashMethod.Md5) Then
                Return
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(12, 2, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("将资源写入到文件：")
                logInterpolatedStringHandler.AppendFormatted(logicalName)
                logInterpolatedStringHandler.AppendLiteral(" → ")
                logInterpolatedStringHandler.AppendFormatted(localFilePath)
            End If
            Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            FileUtils.Write(localFilePath, array)
        End Sub

        Public Sub Copy(sourceFilePath As String, destFilePath As String)
            sourceFilePath = PathUtils.ForCompare(sourceFilePath)
            destFilePath = PathUtils.ForCompare(destFilePath)
            Dim flag As Boolean
            If String.CompareOrdinal(sourceFilePath, destFilePath) = 0 Then
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(17, 2, flag)
                If flag Then
                    logInterpolatedStringHandler.AppendLiteral("复制文件到自身，不执行操作：")
                    logInterpolatedStringHandler.AppendFormatted(sourceFilePath)
                    logInterpolatedStringHandler.AppendLiteral(" → ")
                    logInterpolatedStringHandler.AppendFormatted(destFilePath)
                End If
                Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                Return
            End If
            If PathUtils.IsSame(sourceFilePath, destFilePath) Then
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(27, 2, flag)
                If flag Then
                    logInterpolatedStringHandler2.AppendLiteral("复制文件到自身，但大小写不同，等效于重命名文件：")
                    logInterpolatedStringHandler2.AppendFormatted(sourceFilePath)
                    logInterpolatedStringHandler2.AppendLiteral(" → ")
                    logInterpolatedStringHandler2.AppendFormatted(destFilePath)
                End If
                Logger.Trace(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                FileUtils.Move(sourceFilePath, destFilePath, False)
                Return
            End If
            DirectoryUtils.Create(PathUtils.RemoveLastPart(destFilePath))
            Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(8, 2, flag)
            If flag Then
                logInterpolatedStringHandler3.AppendLiteral("复制文件：")
                logInterpolatedStringHandler3.AppendFormatted(sourceFilePath)
                logInterpolatedStringHandler3.AppendLiteral(" → ")
                logInterpolatedStringHandler3.AppendFormatted(destFilePath)
            End If
            Logger.Trace(logInterpolatedStringHandler3, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            If FileUtils.Exists(destFilePath) Then
                FileUtils.SetReadOnly(destFilePath, False)
            End If
            Dim action As Action(Of Integer) = Sub(_ As Integer)
                File.Copy(PathUtils.ForApi(sourceFilePath), PathUtils.ForApi(destFilePath), True)
            End Sub
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__17_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__17_1
            Dim func As Func(Of Integer, TimeSpan) = <>9__17_
            If <>9__17_ Is Nothing Then
                Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func = func2
                FileUtils.<>c.<>9__17_1 = func2
            End If
            Dim <>9__17_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__17_2
            Dim func3 As Func(Of Exception, Boolean) = <>9__17_2
            If <>9__17_2 Is Nothing Then
                Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func3 = func4
                FileUtils.<>c.<>9__17_2 = func4
            End If
            Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
        End Sub

        Public Sub Move(sourceFilePath As String, destFilePath As String, Optional toRecycleBin As Boolean = False)
            sourceFilePath = PathUtils.ForCompare(sourceFilePath)
            destFilePath = PathUtils.ForCompare(destFilePath)
            Dim flag As Boolean
            If String.CompareOrdinal(sourceFilePath, destFilePath) = 0 Then
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(17, 2, flag)
                If flag Then
                    logInterpolatedStringHandler.AppendLiteral("剪切文件到自身，不执行操作：")
                    logInterpolatedStringHandler.AppendFormatted(sourceFilePath)
                    logInterpolatedStringHandler.AppendLiteral(" → ")
                    logInterpolatedStringHandler.AppendFormatted(destFilePath)
                End If
                Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                Return
            End If
            If PathUtils.IsSame(sourceFilePath, destFilePath) Then
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(18, 2, flag)
                If flag Then
                    logInterpolatedStringHandler2.AppendLiteral("剪切文件到自身，但大小写不同：")
                    logInterpolatedStringHandler2.AppendFormatted(sourceFilePath)
                    logInterpolatedStringHandler2.AppendLiteral(" → ")
                    logInterpolatedStringHandler2.AppendFormatted(destFilePath)
                End If
                Logger.Trace(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                Dim action As Action(Of Integer) = Sub(_ As Integer)
                    Dim text As String = Path.Combine(PathUtils.RemoveLastPart(sourceFilePath), Path.GetRandomFileName())
                    FileUtils.SetReadOnly(sourceFilePath, False)
                    If FileUtils.Exists(destFilePath) Then
                        FileUtils.SetReadOnly(destFilePath, False)
                    End If
                    File.Move(PathUtils.ForApi(sourceFilePath), PathUtils.ForApi(text))
                    File.Move(PathUtils.ForApi(text), PathUtils.ForApi(destFilePath))
                End Sub
                Dim num As UInteger? = New UInteger?(2UI)
                Dim <>9__18_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__18_1
                Dim func As Func(Of Integer, TimeSpan) = <>9__18_
                If <>9__18_ Is Nothing Then
                    Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                    func = func2
                    FileUtils.<>c.<>9__18_1 = func2
                End If
                Dim <>9__18_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__18_2
                Dim func3 As Func(Of Exception, Boolean) = <>9__18_2
                If <>9__18_2 Is Nothing Then
                    Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                    func3 = func4
                    FileUtils.<>c.<>9__18_2 = func4
                End If
                Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
                Return
            End If
            DirectoryUtils.Create(PathUtils.RemoveLastPart(destFilePath))
            FileUtils.Delete(destFilePath, toRecycleBin)
            Dim logInterpolatedStringHandler3 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(8, 2, flag)
            If flag Then
                logInterpolatedStringHandler3.AppendLiteral("剪切文件：")
                logInterpolatedStringHandler3.AppendFormatted(sourceFilePath)
                logInterpolatedStringHandler3.AppendLiteral(" → ")
                logInterpolatedStringHandler3.AppendFormatted(destFilePath)
            End If
            Logger.Trace(logInterpolatedStringHandler3, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            Dim action2 As Action(Of Integer) = Sub(_ As Integer)
                FileUtils.SetReadOnly(sourceFilePath, False)
                If FileUtils.Exists(destFilePath) Then
                    FileUtils.SetReadOnly(destFilePath, False)
                End If
                File.Move(PathUtils.ForApi(sourceFilePath), PathUtils.ForApi(destFilePath))
            End Sub
            Dim num2 As UInteger? = New UInteger?(2UI)
            Dim <>9__18_3 As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__18_4
            Dim func5 As Func(Of Integer, TimeSpan) = <>9__18_3
            If <>9__18_3 Is Nothing Then
                Dim func6 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func5 = func6
                FileUtils.<>c.<>9__18_4 = func6
            End If
            Dim <>9__18_4 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__18_5
            Dim func7 As Func(Of Exception, Boolean) = <>9__18_4
            If <>9__18_4 Is Nothing Then
                Dim func8 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func7 = func8
                FileUtils.<>c.<>9__18_5 = func8
            End If
            Retrier.Attempt(action2, num2, func5, func7, Nothing, Nothing)
        End Sub

        Public Function Delete(filePath As String, Optional toRecycleBin As Boolean = False) As Boolean?
            If Not FileUtils.Exists(filePath) Then
                Return Nothing
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(1, 2, flag)
            If flag Then
                logInterpolatedStringHandler.AppendFormatted(If(toRecycleBin, "将文件删除到回收站", "删除文件"))
                logInterpolatedStringHandler.AppendLiteral("：")
                logInterpolatedStringHandler.AppendFormatted(filePath)
            End If
            Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            If toRecycleBin Then
                Try
                    FileUtils.DeleteToRecycleBin(filePath)
                    Return New Boolean?(True)
                Catch ex As Exception
                    Dim ex2 As Exception
                    Logger.Warn(ex2, "无法将文件删除到回收站，回退到永久删除：" + filePath, LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                End Try
            End If
            Dim action As Action(Of Integer) = Sub(_ As Integer)
                FileUtils.SetReadOnly(filePath, False)
                File.Delete(PathUtils.ForApi(filePath))
            End Sub
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__19_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__19_1
            Dim func As Func(Of Integer, TimeSpan) = <>9__19_
            If <>9__19_ Is Nothing Then
                Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func = func2
                FileUtils.<>c.<>9__19_1 = func2
            End If
            Dim <>9__19_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__19_2
            Dim func3 As Func(Of Exception, Boolean) = <>9__19_2
            If <>9__19_2 Is Nothing Then
                Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func3 = func4
                FileUtils.<>c.<>9__19_2 = func4
            End If
            Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
            Return New Boolean?(False)
        End Function

        Friend Sub DeleteToRecycleBin(target As String)
            Dim CS$<>8__locals1 As FileUtils.<>c__DisplayClass20_0 = New FileUtils.<>c__DisplayClass20_0()
            CS$<>8__locals1.target = target
            If Thread.CurrentThread.GetApartmentState() = ApartmentState.STA Then
                Dim action As Action(Of Integer) = Sub(_ As Integer)
                    MyBase.<DeleteToRecycleBin>g__Run|0()
                End Sub
                Dim num As UInteger? = New UInteger?(2UI)
                Dim <>9__20_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__20_2
                Dim func As Func(Of Integer, TimeSpan) = <>9__20_
                If <>9__20_ Is Nothing Then
                    Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                    func = func2
                    FileUtils.<>c.<>9__20_2 = func2
                End If
                Dim <>9__20_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__20_3
                Dim func3 As Func(Of Exception, Boolean) = <>9__20_2
                If <>9__20_2 Is Nothing Then
                    Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                    func3 = func4
                    FileUtils.<>c.<>9__20_3 = func4
                End If
                Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
                Return
            End If
            Dim internalEx As ExceptionDispatchInfo = Nothing
            Dim thread As Thread = New Thread(Sub()
                Try
                    Dim <>9__ As Action(Of Integer) = CS$<>8__locals1.<>9__5
                    Dim action2 As Action(Of Integer) = <>9__
                    If <>9__ Is Nothing Then
                        Dim CS$<>8__locals3 As FileUtils.<>c__DisplayClass20_0 = CS$<>8__locals1
                        Dim action3 As Action(Of Integer) = Sub(_ As Integer)
                            MyBase.<DeleteToRecycleBin>g__Run|0()
                        End Sub
                        Dim action4 As Action(Of Integer) = action3
                        CS$<>8__locals3.<>9__5 = action3
                        action2 = action4
                    End If
                    Dim num2 As UInteger? = New UInteger?(2UI)
                    Dim <>9__20_3 As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__20_6
                    Dim func5 As Func(Of Integer, TimeSpan) = <>9__20_3
                    If <>9__20_3 Is Nothing Then
                        Dim func6 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                        func5 = func6
                        FileUtils.<>c.<>9__20_6 = func6
                    End If
                    Dim <>9__20_4 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__20_7
                    Dim func7 As Func(Of Exception, Boolean) = <>9__20_4
                    If <>9__20_4 Is Nothing Then
                        Dim func8 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException OrElse TypeOf ex Is COMException
                        func7 = func8
                        FileUtils.<>c.<>9__20_7 = func8
                    End If
                    Retrier.Attempt(action2, num2, func5, func7, Nothing, Nothing)
                Catch ex As Exception
                    Dim ex2 As Exception
                    internalEx = ExceptionDispatchInfo.Capture(ex2)
                End Try
            End Sub)
            thread.IsBackground = True
            thread.Name = "DeleteToRecycleBin"
            thread.SetApartmentState(ApartmentState.STA)
            thread.Start()
            thread.Join()
            Dim internalEx2 As ExceptionDispatchInfo = internalEx
            If internalEx2 Is Nothing Then
                Return
            End If
            internalEx2.[Throw]()
        End Sub

        Private Declare Unicode Function SHCreateItemFromParsingName Lib "shell32.dll" (<MarshalAs(UnmanagedType.LPWStr)> pszPath As String, pbc As IntPtr, ByRef riid As Guid, <MarshalAs(UnmanagedType.[Interface])> <System.Runtime.InteropServices.OutAttribute()> ByRef ppv As FileUtils.IShellItem) As Integer

        Public Function OpenZip(zipFilePath As String, Optional c As CancellationToken = Nothing) As ZipArchive
            Dim func As Func(Of Integer, ZipArchive) = Function(_ As Integer)
                Dim zipArchive As ZipArchive
                Try
                    Try
                        zipArchive = MyBase.<OpenZip>g__TryOpen|0(New UTF8Encoding(False, True))
                    Catch ex As DecoderFallbackException
                        zipArchive = MyBase.<OpenZip>g__TryOpen|0(Encoding.GetEncoding("GB18030"))
                    End Try
                Catch ex2 As InvalidDataException
                    Throw New InvalidDataException("文件不是压缩包，或者文件已损坏（" + zipFilePath + "）", ex2)
                End Try
                Return zipArchive
            End Function
            Dim num As UInteger? = New UInteger?(10UI)
            Dim <>9__25_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__25_2
            Dim func2 As Func(Of Integer, TimeSpan) = <>9__25_
            If <>9__25_ Is Nothing Then
                Dim func3 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func2 = func3
                FileUtils.<>c.<>9__25_2 = func3
            End If
            Dim <>9__25_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__25_3
            Dim func4 As Func(Of Exception, Boolean) = <>9__25_2
            If <>9__25_2 Is Nothing Then
                Dim func5 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func4 = func5
                FileUtils.<>c.<>9__25_3 = func5
            End If
            Return Retrier.Attempt(Of ZipArchive)(func, num, func2, func4, Nothing, c)
        End Function

        Public Sub ExtractToDirectory(compressionFile As String, outputDirectory As String, Optional c As CancellationToken = Nothing, <Nullable(2)> Optional p As ProgressProvider = Nothing)
            compressionFile = PathUtils.ForApi(compressionFile)
            DirectoryUtils.Create(outputDirectory)
            If PathUtils.GetExtension(compressionFile) = "gz" Then
                Dim text As String = Path.Combine(outputDirectory, PathUtils.GetFileNameWithoutExtension(compressionFile))
                Dim flag As Boolean
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(12, 2, flag)
                If flag Then
                    logInterpolatedStringHandler.AppendLiteral("解压 gz 文件：")
                    logInterpolatedStringHandler.AppendFormatted(compressionFile)
                    logInterpolatedStringHandler.AppendLiteral(" → ")
                    logInterpolatedStringHandler.AppendFormatted(text)
                End If
                Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                Using stream As Stream = FileUtils.ReadAsStream(compressionFile, Nothing)
                    Using gzipStream As GZipStream = New GZipStream(stream, CompressionMode.Decompress)
                        FileUtils.Write(text, gzipStream)
                        If p Is Nothing Then
                            Return
                        End If
                        p.Finish(ProgressProvider.ChildrenAction.Finished)
                        Return
                    End Using
                End Using
            End If
            Using zipArchive As ZipArchive = FileUtils.OpenZip(compressionFile, c)
                Dim entries As IEnumerable(Of ZipArchiveEntry) = zipArchive.Entries
                Dim <>9__26_ As Func(Of ZipArchiveEntry, Boolean) = FileUtils.<>c.<>9__26_0
                Dim func As Func(Of ZipArchiveEntry, Boolean) = <>9__26_
                If <>9__26_ Is Nothing Then
                    Dim func2 As Func(Of ZipArchiveEntry, Boolean) = Function(e As ZipArchiveEntry) Not String.IsNullOrEmpty(e.Name)
                    func = func2
                    FileUtils.<>c.<>9__26_0 = func2
                End If
                Dim list As List(Of ZipArchiveEntry) = entries.Where(func).ToList()
                Dim count As Integer = list.Count
                Dim num As Integer = 0
                Dim flag As Boolean
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(19, 3, flag)
                If flag Then
                    logInterpolatedStringHandler2.AppendLiteral("解压 zip 文件：")
                    logInterpolatedStringHandler2.AppendFormatted(compressionFile)
                    logInterpolatedStringHandler2.AppendLiteral(" → ")
                    logInterpolatedStringHandler2.AppendFormatted(outputDirectory)
                    logInterpolatedStringHandler2.AppendLiteral("（共 ")
                    logInterpolatedStringHandler2.AppendFormatted(Of Integer)(count)
                    logInterpolatedStringHandler2.AppendLiteral(" 项）")
                End If
                Logger.Trace(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                For Each zipArchiveEntry As ZipArchiveEntry In list
                    c.ThrowIfCancellationRequested()
                    num += 1
                    If count > 0 AndAlso p IsNot Nothing Then
                        p.[Set](CDbl(num) / CDbl(count), False, ProgressProvider.ChildrenAction.Finished)
                    End If
                    Dim text2 As String = Path.Combine(outputDirectory, zipArchiveEntry.FullName)
                    If Not PathUtils.IsParentOf(outputDirectory, text2) Then
                        Throw New InvalidDataException("Zip 文件项 " + zipArchiveEntry.FullName + " 的路径在压缩包之外，这可能导致安全问题")
                    End If
                    Using stream2 As Stream = zipArchiveEntry.Open()
                        FileUtils.Write(text2, stream2)
                    End Using
                Next
            End Using
        End Sub

        Public Sub CreateZipFromDirectory(outputFullPath As String, sourceDirectory As String)
            DirectoryUtils.Create(PathUtils.RemoveLastPart(outputFullPath))
            FileUtils.Delete(outputFullPath, False)
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(22, 2, flag)
            If flag Then
                logInterpolatedStringHandler.AppendLiteral("将文件夹中的内容压缩为 zip 文件：")
                logInterpolatedStringHandler.AppendFormatted(sourceDirectory)
                logInterpolatedStringHandler.AppendLiteral(" → ")
                logInterpolatedStringHandler.AppendFormatted(outputFullPath)
            End If
            Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            Dim action As Action(Of Integer) = Sub(_ As Integer)
                ZipFile.CreateFromDirectory(PathUtils.ForApi(sourceDirectory), PathUtils.ForApi(outputFullPath))
            End Sub
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__27_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__27_1
            Dim func As Func(Of Integer, TimeSpan) = <>9__27_
            If <>9__27_ Is Nothing Then
                Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func = func2
                FileUtils.<>c.<>9__27_1 = func2
            End If
            Dim <>9__27_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__27_2
            Dim func3 As Func(Of Exception, Boolean) = <>9__27_2
            If <>9__27_2 Is Nothing Then
                Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func3 = func4
                FileUtils.<>c.<>9__27_2 = func4
            End If
            Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
        End Sub

        Public Sub CreateZipFromFiles(outputFullPath As String, ParamArray sourceFiles As String())
            Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            For Each text As String In sourceFiles
                Dim lastPart As String = PathUtils.GetLastPart(text)
                If dictionary.ContainsKey(lastPart) Then
                    Throw New ArgumentException("尝试将多个同文件名的文件放进压缩包中（" + lastPart + "）", "sourceFiles")
                End If
                dictionary.Add(lastPart, text)
            Next
            FileUtils.CreateZipFromFiles(outputFullPath, dictionary)
        End Sub

        Public Sub CreateZipFromFiles(outputFullPath As String, sources As IDictionary(Of String, String))
            DirectoryUtils.Create(PathUtils.RemoveLastPart(outputFullPath))
            FileUtils.Delete(outputFullPath, False)
            Dim func As Func(Of Integer, ZipArchive) = Function(_ As Integer) ZipFile.Open(PathUtils.ForApi(outputFullPath), ZipArchiveMode.Create)
            Dim num As UInteger? = New UInteger?(2UI)
            Dim <>9__29_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__29_1
            Dim func2 As Func(Of Integer, TimeSpan) = <>9__29_
            If <>9__29_ Is Nothing Then
                Dim func3 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                func2 = func3
                FileUtils.<>c.<>9__29_1 = func3
            End If
            Dim <>9__29_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__29_2
            Dim func4 As Func(Of Exception, Boolean) = <>9__29_2
            If <>9__29_2 Is Nothing Then
                Dim func5 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                func4 = func5
                FileUtils.<>c.<>9__29_2 = func5
            End If
            Using zipArchive As ZipArchive = Retrier.Attempt(Of ZipArchive)(func, num, func2, func4, Nothing, Nothing)
                Dim flag As Boolean
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(18, 3, flag)
                If flag Then
                    logInterpolatedStringHandler.AppendLiteral("创建 zip 文件：")
                    logInterpolatedStringHandler.AppendFormatted(Of Integer)(sources.Count)
                    logInterpolatedStringHandler.AppendLiteral(" 个文件 → ")
                    logInterpolatedStringHandler.AppendFormatted(outputFullPath)
                    logInterpolatedStringHandler.AppendLiteral(vbLf)
                    Dim <>9__29_3 As Func(Of KeyValuePair(Of String, String), String) = FileUtils.<>c.<>9__29_3
                    Dim func6 As Func(Of KeyValuePair(Of String, String), String) = <>9__29_3
                    If <>9__29_3 Is Nothing Then
                        Dim func7 As Func(Of KeyValuePair(Of String, String), String) = Function(p As KeyValuePair(Of String, String)) "- " + p.Value + " → " + p.Key
                        func6 = func7
                        FileUtils.<>c.<>9__29_3 = func7
                    End If
                    logInterpolatedStringHandler.AppendFormatted(sources.[Select](func6).Join(vbLf))
                End If
                Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
                For Each keyValuePair As KeyValuePair(Of String, String) In sources
                    zipArchive.CreateEntryFromFile(PathUtils.ForApi(keyValuePair.Value), keyValuePair.Key.Replace("\"c, "/"c), CompressionLevel.Optimal)
                Next
            End Using
        End Sub

        Public Function Exists(filePath As String) As Boolean
            Return File.Exists(PathUtils.ForApi(filePath))
        End Function

        Public Function IsAvailable(filePath As String) As Boolean
            If Not FileUtils.Exists(filePath) Then
                Return False
            End If
            Dim flag As Boolean
            Dim ex2 As Exception
            Try
                Dim action As Action(Of Integer) = Sub(_ As Integer)
                    Using New FileStream(PathUtils.ForApi(filePath), FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                    End Using
                End Sub
                Dim num As UInteger? = New UInteger?(3UI)
                Dim <>9__31_ As Func(Of Integer, TimeSpan) = FileUtils.<>c.<>9__31_1
                Dim func As Func(Of Integer, TimeSpan) = <>9__31_
                If <>9__31_ Is Nothing Then
                    Dim func2 As Func(Of Integer, TimeSpan) = Function(_ As Integer) TimeSpan.FromMilliseconds(200.0)
                    func = func2
                    FileUtils.<>c.<>9__31_1 = func2
                End If
                Dim <>9__31_2 As Func(Of Exception, Boolean) = FileUtils.<>c.<>9__31_2
                Dim func3 As Func(Of Exception, Boolean) = <>9__31_2
                If <>9__31_2 Is Nothing Then
                    Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is IOException
                    func3 = func4
                    FileUtils.<>c.<>9__31_2 = func4
                End If
                Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
                flag = True
            Catch ex As Exception When TypeOf ex2 Is IOException OrElse TypeOf ex2 Is UnauthorizedAccessException
                flag = False
            End Try
            Return flag
        End Function

        Public Function GetInfo(path As String) As FileInfo
            Return New FileInfo(PathUtils.ForApi(path))
        End Function

        Public Sub SetReadOnly(path As String, [readOnly] As Boolean)
            path = PathUtils.ForApi(path)
            Dim attributes As FileAttributes = File.GetAttributes(path)
            Dim fileAttributes As FileAttributes = If([readOnly], (attributes Or FileAttributes.[ReadOnly]), (attributes And Not FileAttributes.[ReadOnly]))
            If attributes = fileAttributes Then
                Return
            End If
            Dim flag As Boolean
            Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(16, 4, flag)
            If flag Then
                logInterpolatedStringHandler.AppendFormatted(If([readOnly], "添加", "移除"))
                logInterpolatedStringHandler.AppendLiteral("只读属性：")
                logInterpolatedStringHandler.AppendFormatted(path)
                logInterpolatedStringHandler.AppendLiteral("（原属性：")
                logInterpolatedStringHandler.AppendFormatted(Of FileAttributes)(attributes)
                logInterpolatedStringHandler.AppendLiteral("，新属性：")
                logInterpolatedStringHandler.AppendFormatted(Of FileAttributes)(fileAttributes)
                logInterpolatedStringHandler.AppendLiteral("）")
            End If
            Logger.Trace(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\FileUtils.cs")
            File.SetAttributes(path, fileAttributes)
        End Sub

        <Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")>
        <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
        <ComImport()>
        Private Interface IShellItem
        End Interface

        <NullableContext(0)>
        <Guid("3AD05575-8857-4850-9277-11B85BDB8E09")>
        <ComImport()>
        Private Class FileOperation
            <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType := MethodCodeType.Runtime)>
            Public Sub New()

            End Sub
        End Class

        <Guid("947AAB5F-0A5C-4C13-B4D6-4BF7836FC9F8")>
        <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
        <ComImport()>
        Private Interface IFileOperation
            Sub Advise(pfops As IntPtr, <System.Runtime.InteropServices.OutAttribute()> ByRef pdwCookie As UInteger)

            Sub Unadvise(dwCookie As UInteger)

            Sub SetOperationFlags(dwOperationFlags As UInteger)

            Sub SetProgressMessage(<MarshalAs(UnmanagedType.LPWStr)> pszMessage As String)

            Sub SetProgressDialog(popd As IntPtr)

            Sub SetProperties(pproparray As IntPtr)

            Sub SetOwnerWindow(hwndOwner As IntPtr)

            Sub ApplyPropertiesToItem(psiItem As FileUtils.IShellItem)

            Sub ApplyPropertiesToItems(punkItems As IntPtr)

            Sub RenameItem(psiItem As FileUtils.IShellItem, <MarshalAs(UnmanagedType.LPWStr)> pszNewName As String, pfopsItem As IntPtr)

            Sub RenameItems(pUnkItems As IntPtr, <MarshalAs(UnmanagedType.LPWStr)> pszNewName As String)

            Sub MoveItem(psiItem As FileUtils.IShellItem, psiDestinationFolder As FileUtils.IShellItem, <MarshalAs(UnmanagedType.LPWStr)> pszNewName As String, pfopsItem As IntPtr)

            Sub MoveItems(punkItems As IntPtr, psiDestinationFolder As FileUtils.IShellItem)

            Sub CopyItem(psiItem As FileUtils.IShellItem, psiDestinationFolder As FileUtils.IShellItem, <MarshalAs(UnmanagedType.LPWStr)> pszCopyName As String, pfopsItem As IntPtr)

            Sub CopyItems(punkItems As IntPtr, psiDestinationFolder As FileUtils.IShellItem)

            Sub DeleteItem(psiItem As FileUtils.IShellItem, pfopsItem As IntPtr)

            Sub DeleteItems(punkItems As IntPtr)

            Sub NewItem(psiDestinationFolder As FileUtils.IShellItem, dwFileAttributes As UInteger, <MarshalAs(UnmanagedType.LPWStr)> pszName As String, <MarshalAs(UnmanagedType.LPWStr)> pszTemplateName As String, pfopsItem As IntPtr)

            Sub PerformOperations()

            Sub GetAnyOperationsAborted(<MarshalAs(UnmanagedType.Bool)> <System.Runtime.InteropServices.OutAttribute()> ByRef pfAnyOperationsAborted As Boolean)
        End Interface
    End Module
End Namespace
