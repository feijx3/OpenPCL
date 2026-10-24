Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class CrashAnalyzer
        Public Sub New(Optional TargetInstance As ModMinecraft.McInstance = Nothing)
            Me.AnalyzeRawFiles = New List(Of KeyValuePair(Of String, String()))()
            Me.DirectFile = Nothing
            Me.LogMc = Nothing
            Me.LogMcDebug = Nothing
            Me.LogHs = Nothing
            Me.LogCrash = Nothing
            Me.CrashReasons = New Dictionary(Of CrashAnalyzer.CrashReason, List(Of String))()
            Me.OutputFiles = New List(Of String)()
            Me.TargetInstance = TargetInstance
            Me.TempFolder = ModMain.RequestTaskTempFolder(False)
            DirectoryUtils.Create(Me.TempFolder + "Temp\")
            DirectoryUtils.Create(Me.TempFolder + "Report\")
            Logger.Info(String.Format("崩溃分析暂存文件夹：{0}", Me.TempFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
        End Sub

        Public Sub Collect(VersionPathIndie As String, Optional LatestLog As IList(Of String) = Nothing)
            Logger.Info("步骤 1：收集日志文件", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            Dim list As List(Of String) = New List(Of String)()
            Try
                list.AddRange(DirectoryUtils.EnumerateFiles(VersionPathIndie + "crash-reports\", True, "*"))
            Catch ex As Exception
                Logger.Warn(ex, "收集 Minecraft 崩溃日志文件夹下的日志失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            End Try
            Try
                Try
                    For Each fileInfo As FileInfo In DirectoryUtils.GetInfo(VersionPathIndie).Parent.Parent.EnumerateFiles()
                        If Operators.CompareString(If(fileInfo.Extension, ""), ".log", False) = 0 Then
                            list.Add(PathUtils.RemoveExtendedPrefix(fileInfo.FullName))
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of FileInfo)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            Catch ex2 As Exception
                Logger.Warn(ex2, "收集 Minecraft 主文件夹下的日志失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            End Try
            Try
                Try
                    For Each fileInfo2 As FileInfo In DirectoryUtils.GetInfo(VersionPathIndie).EnumerateFiles()
                        If Operators.CompareString(If(fileInfo2.Extension, ""), ".log", False) = 0 Then
                            list.Add(PathUtils.RemoveExtendedPrefix(fileInfo2.FullName))
                        End If
                    Next
                Finally
                    Dim enumerator2 As IEnumerator(Of FileInfo)
                    If enumerator2 IsNot Nothing Then
                        enumerator2.Dispose()
                    End If
                End Try
            Catch ex3 As Exception
                Logger.Warn(ex3, "收集 Minecraft 隔离文件夹下的日志失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            End Try
            list.Add(VersionPathIndie + "logs\latest.log")
            list.Add(VersionPathIndie + "logs\debug.log")
            list = list.Distinct().ToList()
            Dim list2 As List(Of String) = New List(Of String)()
            Try
                For Each text As String In list
                    Try
                        Dim info As FileInfo = FileUtils.GetInfo(text)
                        If info.Exists Then
                            Dim num As Double = Math.Abs((info.LastWriteTime - DateTime.Now).TotalMinutes)
                            If num < 3.0 AndAlso info.Length > 0L Then
                                list2.Add(text)
                                Logger.Info(String.Format("可能可用的日志文件：{0}（{1} 分钟）", text, Math.Round(num, 1)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                            End If
                        End If
                    Catch ex4 As Exception
                        Logger.Warn(ex4, String.Format("确认崩溃日志时间失败（{0}）", text), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    End Try
                Next
            Finally
                Dim enumerator3 As List(Of String).Enumerator
                CType(enumerator3, IDisposable).Dispose()
            End Try
            If Not list2.Any() Then
                Logger.Info("未发现可能可用的日志文件", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            End If
            Try
                For Each text2 As String In list2
                    Try
                        Me.AnalyzeRawFiles.Add(New KeyValuePair(Of String, String())(text2, FileUtils.ReadAsLines(text2, False, Nothing, Nothing)))
                    Catch ex5 As Exception
                        Logger.Warn(ex5, String.Format("读取可能的崩溃日志文件失败（{0}）", text2), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    End Try
                Next
            Finally
                Dim enumerator4 As List(Of String).Enumerator
                CType(enumerator4, IDisposable).Dispose()
            End Try
            If LatestLog IsNot Nothing AndAlso LatestLog.Any() Then
                Dim text3 As String = LatestLog.Join(vbCrLf)
                Logger.Info(String.Format("以下为游戏输出的最后一段内容：{0}{1}", vbCrLf, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                FileUtils.Write(Me.TempFolder + "RawOutput.log", text3, Nothing)
                Me.AnalyzeRawFiles.Add(New KeyValuePair(Of String, String())(Me.TempFolder + "RawOutput.log", LatestLog.ToArray()))
                LatestLog.Clear()
            End If
            Logger.Info(String.Format("步骤 1：收集日志文件完成，收集到 {0} 个文件", Me.AnalyzeRawFiles.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
        End Sub

        Public Sub Import(FilePath As String)
            Logger.Info("步骤 1：自主导入日志文件", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            Try
                Dim info As FileInfo = FileUtils.GetInfo(FilePath)
                If info.Exists AndAlso info.Length > 0L AndAlso Operators.CompareString(PathUtils.GetExtension(FilePath), "jar", False) <> 0 Then
                    FileUtils.ExtractToDirectory(FilePath, Me.TempFolder + "Temp\", Nothing, Nothing)
                    Logger.Info(String.Format("已解压导入的日志文件：{0}", FilePath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    GoTo IL_00B4
                End If
            Catch ex As Exception
            End Try
            FileUtils.Copy(FilePath, Me.TempFolder + "Temp\" + PathUtils.GetLastPart(FilePath))
            Logger.Info(String.Format("已复制导入的日志文件：{0}", FilePath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            IL_00B4:
            Try
                For Each fileInfo As FileInfo In DirectoryUtils.GetInfo(Me.TempFolder + "Temp\").EnumerateFiles()
                    Try
                        If fileInfo.Exists AndAlso fileInfo.Length <> 0L Then
                            Dim text As String = fileInfo.Extension.Lower()
                            If Operators.CompareString(text, ".log", False) = 0 OrElse Operators.CompareString(text, ".txt", False) = 0 Then
                                Me.AnalyzeRawFiles.Add(New KeyValuePair(Of String, String())(PathUtils.RemoveExtendedPrefix(fileInfo.FullName), FileUtils.ReadAsLines(fileInfo.FullName, False, Nothing, Nothing)))
                            Else
                                FileUtils.Delete(fileInfo.FullName, False)
                            End If
                        End If
                    Catch ex2 As Exception
                        Logger.Warn(ex2, "导入单个日志文件失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    End Try
                Next
            Finally
                Dim enumerator As IEnumerator(Of FileInfo)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Logger.Info(String.Format("步骤 1：自主导入日志文件，收集到 {0} 个文件", Me.AnalyzeRawFiles.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
        End Sub

        Public Function Prepare() As Boolean
            Logger.Info("步骤 2：准备日志文本", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            Me.DirectFile = Nothing
            Dim list As List(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))) = New List(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String())))()
            Try
                For Each keyValuePair As KeyValuePair(Of String, String()) In Me.AnalyzeRawFiles
                    Dim text As String = PathUtils.GetLastPart(keyValuePair.Key).Lower()
                    Dim analyzeFileType As CrashAnalyzer.AnalyzeFileType
                    If text.StartsWithF("hs_err", False) Then
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.HsErr
                        If Me.DirectFile Is Nothing Then
                            Me.DirectFile = New KeyValuePair(Of String, String())?(keyValuePair)
                        End If
                    ElseIf text.StartsWithF("crash-", False) Then
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.CrashReport
                        If Me.DirectFile Is Nothing Then
                            Me.DirectFile = New KeyValuePair(Of String, String())?(keyValuePair)
                        End If
                    ElseIf Operators.CompareString(text, "latest.log", False) = 0 OrElse Operators.CompareString(text, "latest log.txt", False) = 0 OrElse Operators.CompareString(text, "debug.log", False) = 0 OrElse Operators.CompareString(text, "debug log.txt", False) = 0 OrElse Operators.CompareString(text, "游戏崩溃前的输出.txt", False) = 0 OrElse Operators.CompareString(text, "rawoutput.log", False) = 0 Then
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.MinecraftLog
                        Me.DirectFile = New KeyValuePair(Of String, String())?(keyValuePair)
                    ElseIf Operators.CompareString(text, "启动器日志.txt", False) = 0 OrElse Operators.CompareString(text, "PCL2 启动器日志.txt", False) = 0 OrElse Operators.CompareString(text, "PCL 启动器日志.txt", False) = 0 OrElse Operators.CompareString(text, "log1.txt", False) = 0 Then
                        Dim value As IEnumerable(Of String) = keyValuePair.Value
                        Dim func As Func(Of String, Boolean)
                        If CrashAnalyzer._Closure$__.$I8-0 IsNot Nothing Then
                            func = CrashAnalyzer._Closure$__.$I8-0
                        Else
                            Dim func2 As Func(Of String, Boolean) = Function(s As String) s.Contains("以下为游戏输出的最后一段内容")
                            func = func2
                            CrashAnalyzer._Closure$__.$I8-0 = func2
                        End If
                        If value.Any(func) Then
                            analyzeFileType = CrashAnalyzer.AnalyzeFileType.MinecraftLog
                            If Me.DirectFile Is Nothing Then
                                Me.DirectFile = New KeyValuePair(Of String, String())?(keyValuePair)
                            End If
                        Else
                            analyzeFileType = CrashAnalyzer.AnalyzeFileType.ExtraLogFile
                        End If
                    ElseIf Operators.CompareString(PathUtils.GetExtension(text), "log", False) = 0 Then
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.ExtraLogFile
                    Else
                        If Operators.CompareString(PathUtils.GetExtension(text), "txt", False) <> 0 Then
                            Logger.Info(String.Format("{0} 分类为 Ignore", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                            Continue For
                        End If
                        analyzeFileType = CrashAnalyzer.AnalyzeFileType.ExtraReportFile
                    End If
                    If keyValuePair.Value.Any() Then
                        list.Add(New KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))(analyzeFileType, keyValuePair))
                        Logger.Info(String.Format("{0} 分类为 {1}", text, analyzeFileType), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    Else
                        Logger.Info(String.Format("{0} 由于内容为空跳过", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    End If
                Next
            Finally
                Dim enumerator As List(Of KeyValuePair(Of String, String())).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            If list.Any() Then
                Dim enumerable As IEnumerable(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))) = list
                Dim func3 As Func(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String())), Boolean)
                If CrashAnalyzer._Closure$__.$I8-1 IsNot Nothing Then
                    func3 = CrashAnalyzer._Closure$__.$I8-1
                Else
                    Dim func4 As Func(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String())), Boolean) = Function(p As KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))) p.Key = CrashAnalyzer.AnalyzeFileType.ExtraLogFile
                    func3 = func4
                    CrashAnalyzer._Closure$__.$I8-1 = func4
                End If
                If enumerable.All(func3) Then
                    Logger.Info("由于仅发现了额外日志，将它们视作 Minecraft 日志进行分析", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    Dim enumerable2 As IEnumerable(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))) = list
                    Dim func5 As Func(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String())), KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String())))
                    If CrashAnalyzer._Closure$__.$I8-2 IsNot Nothing Then
                        func5 = CrashAnalyzer._Closure$__.$I8-2
                    Else
                        Dim func6 As Func(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String())), KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))) = Function(p As KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))) New KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))(CrashAnalyzer.AnalyzeFileType.MinecraftLog, p.Value)
                        func5 = func6
                        CrashAnalyzer._Closure$__.$I8-2 = func6
                    End If
                    list = enumerable2.[Select](func5).ToList()
                End If
            End If
            For Each analyzeFileType2 As CrashAnalyzer.AnalyzeFileType In New CrashAnalyzer.AnalyzeFileType() { CrashAnalyzer.AnalyzeFileType.MinecraftLog, CrashAnalyzer.AnalyzeFileType.HsErr, CrashAnalyzer.AnalyzeFileType.ExtraLogFile, CrashAnalyzer.AnalyzeFileType.CrashReport }
                Dim list2 As List(Of KeyValuePair(Of String, String())) = New List(Of KeyValuePair(Of String, String()))()
                Try
                    For Each keyValuePair2 As KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String())) In list
                        If analyzeFileType2 = keyValuePair2.Key Then
                            list2.Add(keyValuePair2.Value)
                        End If
                    Next
                Finally
                    Dim enumerator2 As List(Of KeyValuePair(Of CrashAnalyzer.AnalyzeFileType, KeyValuePair(Of String, String()))).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                If list2.Any() Then
                    Try
                        Select Case analyzeFileType2
                            Case CrashAnalyzer.AnalyzeFileType.HsErr, CrashAnalyzer.AnalyzeFileType.CrashReport
                                Dim sortedList As SortedList(Of DateTime, KeyValuePair(Of String, String())) = New SortedList(Of DateTime, KeyValuePair(Of String, String()))()
                                Try
                                    For Each keyValuePair3 As KeyValuePair(Of String, String()) In list2
                                        Try
                                            sortedList.Add(FileUtils.GetInfo(keyValuePair3.Key).LastWriteTime, keyValuePair3)
                                        Catch ex As Exception
                                            Logger.Warn(ex, "获取日志文件修改时间失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                            sortedList.Add(New DateTime(1900, 1, 1), keyValuePair3)
                                        End Try
                                    Next
                                Finally
                                    Dim enumerator3 As List(Of KeyValuePair(Of String, String())).Enumerator
                                    CType(enumerator3, IDisposable).Dispose()
                                End Try
                                Dim value2 As KeyValuePair(Of String, String()) = sortedList.Last().Value
                                Me.OutputFiles.Add(value2.Key)
                                If analyzeFileType2 = CrashAnalyzer.AnalyzeFileType.HsErr Then
                                    Me.LogHs = Me.GetHeadTailLines(value2.Value, 200, 100)
                                    Logger.Info(String.Format("输出报告：{0}，作为虚拟机错误信息", value2.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                    Logger.Info(String.Format("导入分析：{0}，作为虚拟机错误信息", value2.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                Else
                                    Me.LogCrash = Me.GetHeadTailLines(value2.Value, 300, 700)
                                    Logger.Info(String.Format("输出报告：{0}，作为 Minecraft 崩溃报告", value2.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                    Logger.Info(String.Format("导入分析：{0}，作为 Minecraft 崩溃报告", value2.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                End If
                            Case CrashAnalyzer.AnalyzeFileType.MinecraftLog
                                Me.LogMc = ""
                                Me.LogMcDebug = ""
                                Dim dictionary As Dictionary(Of String, KeyValuePair(Of String, String())) = New Dictionary(Of String, KeyValuePair(Of String, String()))()
                                Try
                                    For Each keyValuePair4 As KeyValuePair(Of String, String()) In list2
                                        dictionary(PathUtils.GetLastPart(keyValuePair4.Key).Lower()) = keyValuePair4
                                        Me.OutputFiles.Add(keyValuePair4.Key)
                                        Logger.Info(String.Format("输出报告：{0}，作为 Minecraft 或启动器日志", keyValuePair4.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                    Next
                                Finally
                                    Dim enumerator4 As List(Of KeyValuePair(Of String, String())).Enumerator
                                    CType(enumerator4, IDisposable).Dispose()
                                End Try
                                For Each text2 As String In New String() { "rawoutput.log", "启动器日志.txt", "log1.txt", "游戏崩溃前的输出.txt", "PCL2 启动器日志.txt", "PCL 启动器日志.txt" }
                                    If dictionary.ContainsKey(text2) Then
                                        Dim keyValuePair5 As KeyValuePair(Of String, String()) = dictionary(text2)
                                        Dim flag As Boolean = False
                                        For Each text3 As String In keyValuePair5.Value
                                            If flag Then
                                                Dim ptr As __ByRef(Of String) = Me.LogMc
                                                Me.LogMc = ptr + text3 + vbLf
                                            ElseIf text3.Contains("以下为游戏输出的最后一段内容") Then
                                                flag = True
                                                Logger.Info("找到 PCL 输出的游戏实时日志头", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                            End If
                                        Next
                                        If Not flag Then
                                            Dim ptr As __ByRef(Of String) = Me.LogMc
                                            Me.LogMc = ptr + Me.GetHeadTailLines(keyValuePair5.Value, 0, 500)
                                        End If
                                        Me.LogMc = Me.LogMc.TrimEnd(vbCrLf.ToCharArray())
                                        Logger.Info(String.Format("导入分析：{0}，作为启动器日志", keyValuePair5.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                        Exit For
                                    End If
                                Next
                                For Each text4 As String In New String() { "latest.log", "latest log.txt", "debug.log", "debug log.txt" }
                                    If dictionary.ContainsKey(text4) Then
                                        Dim keyValuePair6 As KeyValuePair(Of String, String()) = dictionary(text4)
                                        Dim ptr As __ByRef(Of String) = Me.LogMc
                                        Me.LogMc = ptr + Me.GetHeadTailLines(keyValuePair6.Value, 1500, 500)
                                        Logger.Info(String.Format("导入分析：{0}，作为 Minecraft 日志", keyValuePair6.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                        Exit For
                                    End If
                                Next
                                For Each text5 As String In New String() { "debug.log", "debug log.txt" }
                                    If dictionary.ContainsKey(text5) Then
                                        Dim keyValuePair7 As KeyValuePair(Of String, String()) = dictionary(text5)
                                        Dim ptr As __ByRef(Of String) = Me.LogMcDebug
                                        Me.LogMcDebug = ptr + Me.GetHeadTailLines(keyValuePair7.Value, 1000, 0)
                                        Logger.Info(String.Format("导入分析：{0}，作为 Minecraft Debug 日志", keyValuePair7.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                        Exit For
                                    End If
                                Next
                                If Operators.CompareString(Me.LogMc, "", False) = 0 Then
                                    If Operators.CompareString(Me.LogMcDebug, "", False) <> 0 Then
                                        Me.LogMc = Me.LogMcDebug
                                    Else
                                        If Not dictionary.Any() Then
                                            Me.LogMc = Nothing
                                            Throw New Exception("无法找到匹配的 Minecraft Log")
                                        End If
                                        Dim value4 As KeyValuePair(Of String, String()) = dictionary.First().Value
                                        Dim ptr As __ByRef(Of String) = Me.LogMc
                                        Me.LogMc = ptr + Me.GetHeadTailLines(value4.Value, 1500, 500)
                                        Logger.Info(String.Format("导入分析：{0}，作为兜底日志", value4.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                    End If
                                End If
                                If Operators.CompareString(Me.LogMcDebug, "", False) = 0 Then
                                    Me.LogMcDebug = Nothing
                                End If
                            Case CrashAnalyzer.AnalyzeFileType.ExtraLogFile, CrashAnalyzer.AnalyzeFileType.ExtraReportFile
                                Try
                                    For Each keyValuePair8 As KeyValuePair(Of String, String()) In list2
                                        Me.OutputFiles.Add(keyValuePair8.Key)
                                        Logger.Info(String.Format("输出报告：{0}，不用作分析", keyValuePair8.Key), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                                    Next
                                Finally
                                    Dim enumerator5 As List(Of KeyValuePair(Of String, String())).Enumerator
                                    CType(enumerator5, IDisposable).Dispose()
                                End Try
                        End Select
                    Catch ex2 As Exception
                        Logger.Warn(ex2, "分类处理日志文件时出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    End Try
                End If
            Next
            Dim flag2 As Boolean = Me.LogMc IsNot Nothing OrElse Me.LogHs IsNot Nothing OrElse Me.LogCrash <> Nothing
            If flag2 Then
                Logger.Info(String.Format("{0}用作分析", String.Concat(New String() { "[Crash] 步骤 2：准备日志文本完成，找到", If((Me.LogMc Is Nothing), "", "游戏日志、"), If((Me.LogMcDebug Is Nothing), "", "游戏 Debug 日志、"), If((Me.LogHs Is Nothing), "", "虚拟机日志、"), If((Me.LogCrash Is Nothing), "", "崩溃日志、") }).TrimEnd(New Char() { "、"c })), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            Else
                Logger.Info("步骤 2：准备日志文本完成，没有任何可供分析的日志", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            End If
            Return flag2
        End Function

        Private Function GetHeadTailLines(Raw As String(), HeadLines As Integer, TailLines As Integer) As String
            ' The following expression was wrapped in a checked-statement
            Dim text As String
            If Raw.Length <= HeadLines + TailLines Then
                text = Raw.Distinct().Join(vbLf)
            Else
                Dim list As List(Of String) = New List(Of String)()
                Dim num As Integer = 0
                Dim num2 As Integer = Raw.Length - 1
                Dim i As Integer
                i = 0
                While i <= num2
                    If Not list.Contains(Raw(i)) Then
                        num += 1
                        list.Add(Raw(i))
                        If num >= HeadLines Then
                            Exit While
                        End If
                    End If
                    i += 1
                End While
                Dim num3 As Integer = 0
                Dim num4 As Integer = Raw.Length - 1
                Dim num5 As Integer = i
                For j As Integer = num4 To num5 Step -1
                    If Not list.Contains(Raw(j)) Then
                        num3 += 1
                        list.Insert(num, Raw(j))
                        If num3 >= TailLines Then
                            Exit For
                        End If
                    End If
                Next
                Dim stringBuilder As StringBuilder = New StringBuilder()
                Try
                    For Each text2 As String In list
                        If Operators.CompareString(text2, "", False) <> 0 Then
                            stringBuilder.Append(text2)
                            stringBuilder.Append(vbLf)
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                text = stringBuilder.ToString()
            End If
            Return text
        End Function

        Public Sub Analyze()
            Logger.Info("步骤 3：分析崩溃原因", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            Me.LogAll = (If(Me.LogMc, If(Me.LogMcDebug, ""))) + Me.LogHs + Me.LogCrash
            Me.AnalyzeCrit1()
            If Not Me.CrashReasons.Any() Then
                Me.AnalyzeCrit2()
                If Not Me.CrashReasons.Any() Then
                    If Me.LogAll.Contains("orge") OrElse Me.LogAll.Contains("abric") OrElse Me.LogAll.Contains("uilt") OrElse Me.LogAll.Contains("iteloader") Then
                        Dim list As List(Of String) = New List(Of String)()
                        If Me.LogCrash IsNot Nothing Then
                            Logger.Info("开始进行崩溃日志堆栈分析", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                            list.AddRange(Me.AnalyzeStackKeyword(Me.LogCrash.BeforeFirst("System Details", False)))
                        End If
                        If Me.LogMc IsNot Nothing Then
                            Dim list2 As List(Of String) = Me.LogMc.RegexSearch("/FATAL] .+?(?=[\n]+\[)", RegexOptions.None).ToList()
                            If Me.LogMc.Contains("Unreported exception thrown!") Then
                                list2.Add(Me.LogMc.Between("Unreported exception thrown!", "at oolloo.jlw.Wrapper", False))
                            End If
                            Logger.Info(String.Format("开始进行 Minecraft 日志堆栈分析，发现 {0} 个报错项", list2.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                            Try
                                For Each text As String In list2
                                    list.AddRange(Me.AnalyzeStackKeyword(text))
                                Next
                            Finally
                                Dim enumerator As List(Of String).Enumerator
                                CType(enumerator, IDisposable).Dispose()
                            End Try
                        End If
                        If Me.LogHs IsNot Nothing Then
                            Logger.Info("开始进行虚拟机堆栈分析", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                            Dim text2 As String = Me.LogHs.Between("T H R E A D", "Registers:", False)
                            list.AddRange(Me.AnalyzeStackKeyword(text2))
                        End If
                        If list.Any() Then
                            Dim list3 As List(Of String) = Me.AnalyzeModName(list)
                            If list3 Is Nothing Then
                                Me.AppendReason(CrashAnalyzer.CrashReason.堆栈分析发现关键字, list)
                                GoTo IL_0219
                            End If
                            Me.AppendReason(CrashAnalyzer.CrashReason.堆栈分析发现Mod名称, list3)
                            GoTo IL_0219
                        End If
                    Else
                        Logger.Info("可能并未安装 Mod，不进行堆栈分析", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    End If
                    Me.AnalyzeCrit3()
                End If
            End If
            IL_0219:
            If Not Me.CrashReasons.Any() Then
                Logger.Info("步骤 3：分析崩溃原因完成，未找到可能的原因", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                Return
            End If
            Logger.Info(String.Format("步骤 3：分析崩溃原因完成，找到 {0} 条可能的原因", Me.CrashReasons.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            Try
                For Each keyValuePair As KeyValuePair(Of CrashAnalyzer.CrashReason, List(Of String)) In Me.CrashReasons
                    Logger.Info(String.Format("- {0}{1}", keyValuePair.Key, If(keyValuePair.Value.Any(), ("（" + keyValuePair.Value.Join("；"c) + "）"), "")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                Next
            Finally
                Dim enumerator2 As Dictionary(Of CrashAnalyzer.CrashReason, List(Of String)).Enumerator
                CType(enumerator2, IDisposable).Dispose()
            End Try
        End Sub

        Private Sub AppendReason(Reason As CrashAnalyzer.CrashReason, Optional Additional As IEnumerable(Of String) = Nothing)
            If Me.CrashReasons.ContainsKey(Reason) Then
                If Additional IsNot Nothing Then
                    Me.CrashReasons(Reason).AddRange(Additional)
                    Me.CrashReasons(Reason) = Me.CrashReasons(Reason).Distinct().ToList()
                End If
            Else
                Me.CrashReasons.Add(Reason, New List(Of String)((If(Additional, New String(-1) {})).ToList()))
            End If
            Logger.Info(String.Format("可能的崩溃原因：{0}{1}", Reason, If((Additional IsNot Nothing AndAlso Additional.Any()), ("（" + Additional.Join("；"c) + "）"), "")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
        End Sub

        Private Sub AppendReason(Reason As CrashAnalyzer.CrashReason, Additional As String)
            Dim enumerable As IEnumerable(Of String)
            If Not String.IsNullOrEmpty(Additional) Then
                Dim list As List(Of String) = New List(Of String)()
                enumerable = list
                list.Add(Additional)
            Else
                enumerable = Nothing
            End If
            Me.AppendReason(Reason, enumerable)
        End Sub

        Private Sub AnalyzeCrit1()
            If Me.LogMc Is Nothing AndAlso Me.LogHs Is Nothing AndAlso Me.LogCrash Is Nothing Then
                Me.AppendReason(CrashAnalyzer.CrashReason.没有可用的分析文件, Nothing)
                Return
            End If
            If Me.LogCrash IsNot Nothing Then
                If Me.LogCrash.Contains("Unable to make protected final java.lang.Class java.lang.ClassLoader.defineClass") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, Nothing)
                End If
                If Me.LogCrash.Contains("Failed loading config file ") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃, New String() { Me.TryAnalyzeModName((If(Me.LogCrash.RegexSeek("(?<=Failed loading config file .+ for modid )[^\n]+", RegexOptions.None), "")).TrimEnd(New Char() { vbCr })).First(), (If(Me.LogCrash.RegexSeek("(?<=Failed loading config file ).+(?= of type)", RegexOptions.None), "")).TrimEnd(New Char() { vbCr }) })
                End If
            End If
            If Me.LogMc IsNot Nothing Then
                If Me.LogMc.Contains("Unrecognized option:") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java虚拟机参数有误, Nothing)
                End If
                If Me.LogMc.Contains("Found multiple arguments for option fml.forgeVersion, but you asked for only one") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.版本Json中存在多个Forge, Nothing)
                End If
                If Me.LogMc.Contains("The driver does not appear to support OpenGL") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.显卡不支持OpenGL, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.ClassCastException: java.base/jdk") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.使用JDK, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.ClassCastException: class jdk.") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.使用JDK, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraft.client.renderer.texture.SpriteContents.<init>") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: 'java.lang.String com.mojang.blaze3d.systems.RenderSystem.getBackendDescription") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraft.client.renderer.block.model.BakedQuad.<init>") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraftforge.client.gui.overlay.ForgeGui.renderSelectedItemName") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: 'void net.minecraft.server.level.DistanceManager") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: 'net.minecraft.network.chat.FormattedText net.minecraft.client.gui.Font.ellipsize") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, Nothing)
                End If
                If Me.LogMc.Contains("Open J9 is not supported") OrElse Me.LogMc.Contains("OpenJ9 is incompatible") OrElse Me.LogMc.Contains(".J9VMInternals.") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.使用OpenJ9, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchFieldException: ucp") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, Nothing)
                End If
                If Me.LogMc.Contains("because module java.base does not export") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.ClassNotFoundException: jdk.nashorn.api.scripting.NashornScriptEngineFactory") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.ClassNotFoundException: java.lang.invoke.LambdaMetafactory") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本过高, Nothing)
                End If
                If Me.LogMc.Contains("The directories below appear to be extracted jar files. Fix this before you continue.") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod文件被解压, Nothing)
                End If
                If Me.LogMc.Contains("Extracted mod jars found, loading will NOT continue") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod文件被解压, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.ClassNotFoundException: org.spongepowered.asm.launch.MixinTweaker") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.MixinBootstrap缺失, Nothing)
                End If
                If Me.LogMc.Contains("Couldn't set pixel format") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.显卡驱动不支持导致无法设置像素格式, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.OutOfMemoryError") OrElse Me.LogMc.Contains("an out of memory error") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.内存不足, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.RuntimeException: Shaders Mod detected. Please remove it, OptiFine has built-in support for shaders.") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.ShadersMod与OptiFine同时安装, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: sun.security.util.ManifestEntryVerifier") OrElse Me.LogMc.Contains("java.lang.NoSuchMethodError: 'void sun.security.util.ManifestEntryVerifier") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.低版本Forge与高版本Java不兼容, Nothing)
                End If
                If Me.LogMc.Contains("1282: Invalid operation") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.光影或资源包导致OpenGL1282错误, Nothing)
                End If
                If Me.LogMc.Contains("signer information does not match signer information of other classes in the same package") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.文件或内容校验失败, (If(Me.LogMc.RegexSeek("(?<=class "")[^']+(?=""'s signer information)", RegexOptions.None), "")).TrimEnd(New Char() { vbCr }))
                End If
                If Me.LogMc.Contains("Maybe try a lower resolution resourcepack?") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.材质过大或显卡配置不足, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.NoSuchMethodError: net.minecraft.world.server.ChunkManager$ProxyTicketManager.shouldForceTicks(J)Z") AndAlso Me.LogMc.Contains("OptiFine") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine导致无法加载世界, Nothing)
                End If
                If Me.LogMc.Contains("com.electronwill.nightconfig.core.io.ParsingException: Not enough data available") AndAlso Not Me.CrashReasons.ContainsKey(CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃) Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.NightConfig的Bug, Nothing)
                End If
                If Me.LogMc.Contains("Cannot find launch target fmlclient, unable to launch") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Forge安装不完整, Nothing)
                End If
                If Me.LogMc.Contains("Invalid paths argument, contained no existing paths") AndAlso Me.LogMc.Contains("libraries\net\minecraftforge\fmlcore") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Forge安装不完整, Nothing)
                End If
                If Me.LogMc.Contains("Invalid module name: '' is not a Java identifier") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod名称包含特殊字符, Nothing)
                End If
                If Me.LogMc.Contains("has been compiled by a more recent version of the Java Runtime (class file version 55.0), this version of the Java Runtime only recognizes class file versions up to") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod需要Java11, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.RuntimeException: java.lang.NoSuchMethodException: no such method: sun.misc.Unsafe.defineAnonymousClass(Class,byte[],Object[])Class/invokeVirtual") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod需要Java11, Nothing)
                End If
                If Me.LogMc.Contains("java.lang.IllegalArgumentException: The requested compatibility level JAVA_11 could not be set. Level is not supported by the active JRE or ASM version") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod需要Java11, Nothing)
                End If
                If Me.LogMc.Contains("Unsupported class file major version") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本不兼容, Nothing)
                End If
                If Me.LogMc.Contains("Unsupported major.minor version") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本不兼容, Nothing)
                End If
                If Me.LogMc.Contains("Level is not supported by the active JRE or ASM version") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Java版本不兼容, Nothing)
                End If
                If Me.LogMc.Contains("Invalid maximum heap size") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.使用32位Java导致JVM无法分配足够多的内存, Nothing)
                End If
                If Me.LogMc.Contains("Could not reserve enough space") Then
                    If Me.LogMc.Contains("for 1048576KB object heap") Then
                        Me.AppendReason(CrashAnalyzer.CrashReason.使用32位Java导致JVM无法分配足够多的内存, Nothing)
                    Else
                        Me.AppendReason(CrashAnalyzer.CrashReason.内存不足, Nothing)
                    End If
                End If
                If Me.LogMc.Contains("Caught exception from ") Then
                    Dim crashReason As CrashAnalyzer.CrashReason = CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃
                    Dim text As String = Me.LogMc.RegexSeek("(?<=Caught exception from )[^\n]+?", RegexOptions.None)
                    Me.AppendReason(crashReason, Me.TryAnalyzeModName(If((text IsNot Nothing), text.TrimEnd(vbCrLf & " ".ToCharArray()), Nothing)))
                End If
                If Me.LogMc.Contains("DuplicateModsFoundException") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, Me.LogMc.RegexSearch("(?<=\n\t[\w]+ : [A-Z]:[^\n]+(/|\\))[^/\\\n]+?.jar", RegexOptions.IgnoreCase))
                End If
                If Me.LogMc.Contains("Found a duplicate mod") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, (If(Me.LogMc.RegexSeek("Found a duplicate mod[^\n]+", RegexOptions.None), "")).RegexSearch("[^\\/]+.jar", RegexOptions.IgnoreCase))
                End If
                If Me.LogMc.Contains("Found duplicate mods") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, Me.LogMc.RegexSearch("(?<=Mod ID: ')\w+?(?=' from mod files:)", RegexOptions.None).Distinct().ToList())
                End If
                If Me.LogMc.Contains("ModResolutionException: Duplicate") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod重复安装, (If(Me.LogMc.RegexSeek("ModResolutionException: Duplicate[^\n]+", RegexOptions.None), "")).RegexSearch("[^\\/]+.jar", RegexOptions.IgnoreCase))
                End If
                If Me.LogMc.Contains("Incompatible mods found!") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod互不兼容, (If(Me.LogMc.RegexSeek("(?<=Incompatible mods found![\s\S]+: )[\s\S]+?(?=\tat )", RegexOptions.None), "")).BeforeFirst("更多信息：", False).Replace("Some of your mods are incompatible with the game or each other!", "").Trim(vbCrLf & " ".ToCharArray()))
                End If
                If Me.LogMc.Contains("Missing or unsupported mandatory dependencies:") Then
                    Dim crashReason2 As CrashAnalyzer.CrashReason = CrashAnalyzer.CrashReason.Mod缺少前置或MC版本错误
                    Dim enumerable As IEnumerable(Of String) = Me.LogMc.RegexSearch("(?<=Missing or unsupported mandatory dependencies:)([\n\r]+\t(.*))+", RegexOptions.IgnoreCase)
                    Dim func As Func(Of String, String)
                    If CrashAnalyzer._Closure$__.$I20-0 IsNot Nothing Then
                        func = CrashAnalyzer._Closure$__.$I20-0
                    Else
                        Dim func2 As Func(Of String, String) = Function(s As String) s.Trim(vbCrLf & vbTab & " ".ToCharArray())
                        func = func2
                        CrashAnalyzer._Closure$__.$I20-0 = func2
                    End If
                    Me.AppendReason(crashReason2, enumerable.[Select](func).Distinct().ToList())
                End If
            End If
            If Me.LogHs IsNot Nothing Then
                If Me.LogHs.Contains("The system is out of physical RAM or swap space") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.内存不足, Nothing)
                End If
                If Me.LogHs.Contains("Out of Memory Error") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.内存不足, Nothing)
                End If
                If Me.LogHs.Contains("EXCEPTION_ACCESS_VIOLATION") Then
                    If Me.LogHs.Contains("# C  [ig") Then
                        Me.AppendReason(CrashAnalyzer.CrashReason.Intel驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, Nothing)
                    End If
                    If Me.LogHs.Contains("# C  [atio") Then
                        Me.AppendReason(CrashAnalyzer.CrashReason.AMD驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, Nothing)
                    End If
                    If Me.LogHs.Contains("# C  [nvoglv") Then
                        Me.AppendReason(CrashAnalyzer.CrashReason.Nvidia驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, Nothing)
                    End If
                End If
            End If
            If Me.LogCrash IsNot Nothing Then
                If Me.LogCrash.Contains("maximum id range exceeded") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod过多导致超出ID限制, Nothing)
                End If
                If Me.LogCrash.Contains("java.lang.OutOfMemoryError") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.内存不足, Nothing)
                End If
                If Me.LogCrash.Contains("Pixel format not accelerated") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.显卡驱动不支持导致无法设置像素格式, Nothing)
                End If
                If Me.LogCrash.Contains("Manually triggered debug crash") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.玩家手动触发调试崩溃, Nothing)
                End If
                If Me.LogCrash.Contains("has mods that were not found") AndAlso Me.LogCrash.RegexCheck("The Mod File [^\n]+optifine\\OptiFine[^\n]+ has mods that were not found", RegexOptions.None) Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.OptiFine与Forge不兼容, Nothing)
                End If
                If Me.LogCrash.Contains("-- MOD ") Then
                    Dim text2 As String = Me.LogCrash.Between("-- MOD ", "Failure message:", False)
                    If text2.ContainsIgnoreCase(".jar") Then
                        Me.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, (If(text2.RegexSeek("(?<=Mod File: ).+", RegexOptions.None), "")).TrimEnd(vbCrLf & " ".ToCharArray()))
                    Else
                        Me.AppendReason(CrashAnalyzer.CrashReason.Mod加载器报错, (If(Me.LogCrash.RegexSeek("(?<=Failure message: )[\w\W]+?(?=\tMod)", RegexOptions.None), "")).Replace(vbTab, " ").TrimEnd(vbCrLf & " ".ToCharArray()))
                    End If
                End If
                If Me.LogCrash.Contains("Multiple entries with same key: ") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, Me.TryAnalyzeModName((If(Me.LogCrash.RegexSeek("(?<=Multiple entries with same key: )[^=]+", RegexOptions.None), "")).TrimEnd(vbCrLf & " ".ToCharArray())))
                End If
                If Me.LogCrash.Contains("LoaderExceptionModCrash: Caught exception from ") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, Me.TryAnalyzeModName((If(Me.LogCrash.RegexSeek("(?<=LoaderExceptionModCrash: Caught exception from )[^\n]+", RegexOptions.None), "")).TrimEnd(vbCrLf & " ".ToCharArray())))
                End If
                If Me.LogCrash.Contains("Failed loading config file ") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃, New String() { Me.TryAnalyzeModName((If(Me.LogCrash.RegexSeek("(?<=Failed loading config file .+ for modid )[^\n]+", RegexOptions.None), "")).TrimEnd(New Char() { vbCr })).First(), (If(Me.LogCrash.RegexSeek("(?<=Failed loading config file ).+(?= of type)", RegexOptions.None), "")).TrimEnd(New Char() { vbCr }) })
                End If
            End If
        End Sub

        Private Sub AnalyzeCrit2()
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_9(Of String, Boolean) = Function(LogText As String)
                Dim flag2 As Boolean
                If Not LogText.Contains("Mixin prepare failed ") AndAlso Not LogText.Contains("Mixin apply failed ") AndAlso Not LogText.Contains("MixinApplyError") AndAlso Not LogText.Contains("MixinTransformerError") AndAlso Not LogText.Contains("mixin.injection.throwables.") AndAlso Not LogText.Contains(".json] FAILED during )") Then
                    flag2 = False
                Else
                    Dim text3 As String = LogText.RegexSeek("(?<=from mod )[^.\/ ]+(?=\] from)", RegexOptions.None)
                    If text3 Is Nothing Then
                        text3 = LogText.RegexSeek("(?<=for mod )[^.\/ ]+(?= failed)", RegexOptions.None)
                    End If
                    If text3 IsNot Nothing Then
                        Me.AppendReason(CrashAnalyzer.CrashReason.ModMixin失败, Me.TryAnalyzeModName(text3.TrimEnd(vbCrLf & " ".ToCharArray())))
                        flag2 = True
                    Else
                        Try
                            Dim enumerator As IEnumerator(Of String) = LogText.RegexSearch("(?<=^[^\t]+[ \[{(])[^ \[{(]+\.[^ ]+(?=\.json)", RegexOptions.Multiline).GetEnumerator()
                            If enumerator.MoveNext() Then
                                Dim text4 As String = enumerator.Current
                                Me.AppendReason(CrashAnalyzer.CrashReason.ModMixin失败, Me.TryAnalyzeModName(text4.Replace("mixins", "mixin").Replace(".mixin", "").Replace("mixin.", "")))
                                Return True
                            End If
                        Finally
                            Dim enumerator As IEnumerator(Of String)
                            If enumerator IsNot Nothing Then
                                enumerator.Dispose()
                            End If
                        End Try
                        Me.AppendReason(CrashAnalyzer.CrashReason.ModMixin失败, Nothing)
                        flag2 = True
                    End If
                End If
                Return flag2
            End Function
            If Me.LogMc IsNot Nothing Then
                Dim flag As Boolean = vb$AnonymousDelegate_(Me.LogMc)
                If Me.LogMc.Contains("An exception was thrown, the game will display an error screen and halt.") Then
                    Dim crashReason As CrashAnalyzer.CrashReason = CrashAnalyzer.CrashReason.Forge报错
                    Dim text As String = Me.LogMc.RegexSeek("(?<=the game will display an error screen and halt.[\n\r]+[^\n]+?Exception: )[\s\S]+?(?=\n\tat)", RegexOptions.None)
                    Me.AppendReason(crashReason, If((text IsNot Nothing), text.Trim(New Char() { vbCr }), Nothing))
                End If
                If Me.LogMc.Contains("A potential solution has been determined:") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Fabric报错并给出解决方案, (If(Me.LogMc.RegexSeek("(?<=A potential solution has been determined:\n)(\s+ - [^\n]+\n)+", RegexOptions.None), "")).RegexSearch("(?<=\s+)[^\n]+", RegexOptions.None).Join(vbLf))
                End If
                If Me.LogMc.Contains("A potential solution has been determined, this may resolve your problem:") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Fabric报错并给出解决方案, (If(Me.LogMc.RegexSeek("(?<=A potential solution has been determined, this may resolve your problem:\n)(\s+ - [^\n]+\n)+", RegexOptions.None), "")).RegexSearch("(?<=\s+)[^\n]+", RegexOptions.None).Join(vbLf))
                End If
                If Me.LogMc.Contains("确定了一种可能的解决方法，这样做可能会解决你的问题：") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Fabric报错并给出解决方案, (If(Me.LogMc.RegexSeek("(?<=确定了一种可能的解决方法，这样做可能会解决你的问题：\n)(\s+ - [^\n]+\n)+", RegexOptions.None), "")).RegexSearch("(?<=\s+)[^\n]+", RegexOptions.None).Join(vbLf))
                End If
                If Not flag AndAlso Me.LogMc.Contains("due to errors, provided by ") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃, Me.TryAnalyzeModName((If(Me.LogMc.RegexSeek("(?<=due to errors, provided by ')[^']+", RegexOptions.None), "")).TrimEnd(vbCrLf & " ".ToCharArray())))
                End If
            End If
            If Me.LogCrash IsNot Nothing Then
                vb$AnonymousDelegate_(Me.LogCrash)
                If Me.LogCrash.Contains("Suspected Mod") Then
                    Dim text2 As String = Me.LogCrash.Between("Suspected Mod", "Stacktrace", False)
                    If Not text2.StartsWithF("s: None", False) Then
                        Dim enumerable As IEnumerable(Of String) = text2.RegexSearch("(?<=\n\t[^(\t]+\()[^)\n]+", RegexOptions.None)
                        If enumerable.Any() Then
                            Me.AppendReason(CrashAnalyzer.CrashReason.怀疑Mod导致游戏崩溃, Me.TryAnalyzeModName(enumerable))
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub AnalyzeCrit3()
            If Me.LogMc IsNot Nothing Then
                If Not Me.LogMc.Contains("at net.") AndAlso Not Me.LogMc.Contains("INFO]") AndAlso Me.LogHs Is Nothing AndAlso Me.LogCrash Is Nothing AndAlso Me.LogMc.Length < 100 Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.极短的程序输出, Me.LogMc)
                End If
                If Me.LogMc.Contains("Mod resolution failed") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod加载器报错, Nothing)
                End If
                If Me.LogMc.Contains("Failed to create mod instance.") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.Mod初始化失败, Me.TryAnalyzeModName((If(Me.LogMc.RegexSeek("(?<=Failed to create mod instance. ModID: )[^,]+", RegexOptions.None), If(Me.LogMc.RegexSeek("(?<=Failed to create mod instance. ModId )[^\n]+(?= for )", RegexOptions.None), ""))).TrimEnd(New Char() { vbCr })))
                End If
            End If
            If Me.LogCrash IsNot Nothing Then
                If Me.LogCrash.Contains(vbTab & "Block location: World: ") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.特定方块导致崩溃, Me.LogCrash.RegexSeek("(?<=\tBlock: Block\{)[^\}]+", RegexOptions.None) + " " + Me.LogCrash.RegexSeek("(?<=\tBlock location: World: )\([^\)]+\)", RegexOptions.None))
                End If
                If Me.LogCrash.Contains(vbTab & "Entity's Exact location: ") Then
                    Me.AppendReason(CrashAnalyzer.CrashReason.特定实体导致崩溃, Me.LogCrash.RegexSeek("(?<=\tEntity Type: )[^\n]+(?= \()", RegexOptions.None) + " (" + (If(Me.LogCrash.RegexSeek("(?<=\tEntity's Exact location: )[^\n]+", RegexOptions.None), "")).TrimEnd(vbCrLf.ToCharArray()) + ")")
                End If
            End If
        End Sub

        Private Function AnalyzeStackKeyword(ErrorStack As String) As List(Of String)
            ErrorStack = vbLf + ErrorStack + vbLf
            Dim list As List(Of String) = New List(Of String)()
            list.AddRange(ErrorStack.RegexSearch("(?<=\n[^{]+)[a-zA-Z_]+\w+\.[a-zA-Z_]+[\w\.]+(?=\.[\w\.$]+\.)", RegexOptions.None))
            Dim list2 As List(Of String) = list
            Dim enumerable As IEnumerable(Of String) = ErrorStack.RegexSearch("(?<=at [^(]+?\.\w+\$\w+\$)[\w\$]+?(?=\$\w+\()", RegexOptions.None)
            Dim func As Func(Of String, String)
            If CrashAnalyzer._Closure$__.$I23-0 IsNot Nothing Then
                func = CrashAnalyzer._Closure$__.$I23-0
            Else
                Dim func2 As Func(Of String, String) = Function(s As String) s.Replace("$", ".")
                func = func2
                CrashAnalyzer._Closure$__.$I23-0 = func2
            End If
            list2.AddRange(enumerable.[Select](func))
            list = list.Distinct().ToList()
            Dim list3 As List(Of String) = New List(Of String)()
            Try
                Dim enumerator As List(Of String).Enumerator = list.GetEnumerator()
                IL_0174:
                While enumerator.MoveNext()
                    Dim text As String = enumerator.Current
                    For Each text2 As String In New String() { "java", "sun", "javax", "jdk", "oolloo", "org.lwjgl", "com.sun", "net.minecraftforge", "paulscode.sound", "com.mojang", "net.minecraft", "cpw.mods", "com.google", "org.apache", "org.spongepowered", "net.fabricmc", "com.mumfrey", "com.electronwill.nightconfig", "it.unimi.dsi", "MojangTricksIntelDriversForPerformance_javaw" }
                        If text.StartsWithF(text2, False) Then
                            GoTo IL_0174
                        End If
                    Next
                    list3.Add(text.Trim())
                End While
            Finally
                Dim enumerator As List(Of String).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            list3 = list3.Distinct().ToList()
            Logger.Info(String.Format("找到 {0} 条可能的堆栈信息", list3.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
            Dim list4 As List(Of String)
            If Not list3.Any() Then
                list4 = New List(Of String)()
            Else
                Try
                    For Each text3 As String In list3
                        Logger.Info(String.Format("- {0}", text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    Next
                Finally
                    Dim enumerator2 As List(Of String).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                Dim list5 As List(Of String) = New List(Of String)()
                Try
                    For Each text4 As String In list3
                        Dim array2 As String() = text4.Split(".", False)
                        Dim num As Integer = Math.Min(3, array2.Count() - 1)
                        For j As Integer = 0 To num
                            Dim text5 As String = array2(j)
                            If text5.Length > 2 AndAlso Not text5.StartsWithF("func_", False) AndAlso Not New String() { "com", "org", "net", "asm", "fml", "mod", "jar", "sun", "lib", "map", "gui", "dev", "nio", "api", "dsi", "top", "mcp", "core", "init", "mods", "main", "file", "game", "load", "read", "done", "util", "tile", "item", "base", "fake", "oshi", "impl", "data", "pool", "task", "forge", "setup", "block", "model", "mixin", "event", "unimi", "netty", "world", "lwjgl", "fakes", "fabric", "gitlab", "common", "server", "config", "mixins", "compat", "loader", "launch", "script", "entity", "assist", "client", "plugin", "modapi", "mojang", "shader", "events", "github", "recipe", "render", "packet", "events", "preinit", "preload", "machine", "reflect", "channel", "general", "handler", "content", "systems", "modules", "service", "scripts", "network", "fastutil", "optifine", "internal", "platform", "override", "fabricmc", "neoforge", "external", "injection", "listeners", "scheduler", "minecraft", "universal", "multipart", "neoforged", "microsoft", "transformer", "transformers", "minecraftforge", "blockentity", "spongepowered", "electronwill", "concurrent" }.Contains(text5.Lower()) Then
                                list5.Add(text5.Trim())
                            End If
                        Next
                    Next
                Finally
                    Dim enumerator3 As List(Of String).Enumerator
                    CType(enumerator3, IDisposable).Dispose()
                End Try
                list5 = list5.Distinct().ToList()
                Logger.Info(String.Format("从堆栈信息中找到 {0} 个可能的 Mod ID 关键词", list5.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                If list5.Any() Then
                    Logger.Info(String.Format("- {0}", list5.Join(", ")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                End If
                If list5.Count > 10 Then
                    Logger.Info("关键词过多，考虑匹配出错，不纳入考虑", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    list4 = New List(Of String)()
                Else
                    list4 = list5
                End If
            End If
            Return list4
        End Function

        Private Function AnalyzeModName(Keywords As IEnumerable(Of String)) As List(Of String)
            Dim list As List(Of String) = New List(Of String)()
            Dim list2 As List(Of String) = New List(Of String)()
            Try
                For Each text As String In Keywords
                    For Each text2 As String In text.Split("(", False)
                        list2.Add(text2.Trim(" )".ToCharArray()))
                    Next
                Next
            Finally
                Dim enumerator As IEnumerator(Of String)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Keywords = list2
            If Me.LogCrash IsNot Nothing AndAlso Me.LogCrash.Contains("A detailed walkthrough of the error") Then
                Dim text3 As String = Me.LogCrash.Replace("A detailed walkthrough of the error", "¨")
                Dim flag As Boolean = text3.Contains("Fabric Mods")
                If flag Then
                    text3 = text3.Replace("Fabric Mods", "¨")
                    Logger.Info("崩溃报告中检测到 Fabric Mod 信息格式", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                End If
                text3 = text3.AfterLast("¨", False)
                Dim list3 As List(Of String) = New List(Of String)()
                For Each text4 As String In text3.Split(vbLf, False)
                    ' The following expression was wrapped in a checked-expression
                    If(text4.ContainsIgnoreCase(".jar") AndAlso text4.Length - text4.Replace(".jar", "").Length = 4) OrElse (flag AndAlso text4.StartsWithF(vbTab & vbTab, False) AndAlso Not text4.RegexCheck("\t\tfabric[\w-]*: Fabric", RegexOptions.None)) Then
                        list3.Add(text4)
                    End If
                Next
                Logger.Info(String.Format("崩溃报告中找到 {0} 个可能的 Mod 项目行", list3.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                Dim list4 As List(Of String) = New List(Of String)()
                Try
                    For Each text5 As String In Keywords
                        Try
                            For Each text6 As String In list3
                                Dim text7 As String = text6.Lower().Replace("_", "")
                                If text7.Contains(text5.Lower().Replace("_", "")) AndAlso Not text7.Contains("minecraft.jar") AndAlso Not text7.Contains(" forge-") AndAlso Not text7.Contains(" mixin-") Then
                                    list4.Add(text6.Trim(vbCrLf.ToCharArray()))
                                    Exit For
                                End If
                            Next
                        Finally
                            Dim enumerator3 As List(Of String).Enumerator
                            CType(enumerator3, IDisposable).Dispose()
                        End Try
                    Next
                Finally
                    Dim enumerator2 As IEnumerator(Of String)
                    If enumerator2 IsNot Nothing Then
                        enumerator2.Dispose()
                    End If
                End Try
                list4 = list4.Distinct().ToList()
                Logger.Info(String.Format("崩溃报告中找到 {0} 个可能的崩溃 Mod 匹配行", list4.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                Try
                    For Each text8 As String In list4
                        Logger.Info(String.Format("- {0}", text8), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    Next
                Finally
                    Dim enumerator4 As List(Of String).Enumerator
                    CType(enumerator4, IDisposable).Dispose()
                End Try
                Try
                    For Each text9 As String In list4
                        Dim text10 As String
                        If flag Then
                            text10 = text9.RegexSeek("(?<=: )[^\n]+(?= [^\n]+)", RegexOptions.None)
                        Else
                            text10 = text9.RegexSeek("(?<=\()[^\t]+.jar(?=\))|(?<=(\t\t)|(\| ))[^\t\|]+.jar", RegexOptions.IgnoreCase)
                        End If
                        If text10 IsNot Nothing Then
                            list.Add(text10)
                        End If
                    Next
                Finally
                    Dim enumerator5 As List(Of String).Enumerator
                    CType(enumerator5, IDisposable).Dispose()
                End Try
            End If
            If Me.LogMcDebug IsNot Nothing Then
                Dim enumerable As IEnumerable(Of String) = Me.LogMcDebug.RegexSearch("(?<=valid mod file ).*", RegexOptions.Multiline)
                Logger.Info(String.Format("Debug 信息中找到 {0} 个可能的 Mod 项目行", enumerable.Count()), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                Dim list5 As List(Of String) = New List(Of String)()
                Try
                    For Each text11 As String In Keywords
                        Try
                            For Each text12 As String In enumerable
                                If text12.Contains(String.Format("{{{0}}}", text11)) Then
                                    list5.Add(text12)
                                End If
                            Next
                        Finally
                            Dim enumerator7 As IEnumerator(Of String)
                            If enumerator7 IsNot Nothing Then
                                enumerator7.Dispose()
                            End If
                        End Try
                    Next
                Finally
                    Dim enumerator6 As IEnumerator(Of String)
                    If enumerator6 IsNot Nothing Then
                        enumerator6.Dispose()
                    End If
                End Try
                list5 = list5.Distinct().ToList()
                Logger.Info(String.Format("Debug 信息中找到 {0} 个可能的崩溃 Mod 匹配行", list5.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                Try
                    For Each text13 As String In list5
                        Logger.Info(String.Format("- {0}", text13), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    Next
                Finally
                    Dim enumerator8 As List(Of String).Enumerator
                    CType(enumerator8, IDisposable).Dispose()
                End Try
                Try
                    For Each text14 As String In list5
                        Dim text15 As String = text14.RegexSeek(".*(?= with)", RegexOptions.None)
                        If text15 IsNot Nothing Then
                            list.Add(text15)
                        End If
                    Next
                Finally
                    Dim enumerator9 As List(Of String).Enumerator
                    CType(enumerator9, IDisposable).Dispose()
                End Try
            End If
            list = list.Distinct().ToList()
            Dim list6 As List(Of String)
            If Not list.Any() Then
                list6 = Nothing
            Else
                Logger.Info(String.Format("找到 {0} 个可能的崩溃 Mod 文件名", list.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                Try
                    For Each text16 As String In list
                        Logger.Info(String.Format("- {0}", text16), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    Next
                Finally
                    Dim enumerator10 As List(Of String).Enumerator
                    CType(enumerator10, IDisposable).Dispose()
                End Try
                list6 = list
            End If
            Return list6
        End Function

        Private Function TryAnalyzeModName(Keyword As String) As List(Of String)
            Dim list As List(Of String) = New List(Of String)() From { If(Keyword, "") }
            Dim list2 As List(Of String)
            If String.IsNullOrEmpty(Keyword) Then
                list2 = list
            Else
                list2 = If(Me.AnalyzeModName(list), list)
            End If
            Return list2
        End Function

        Private Function TryAnalyzeModName(Keywords As IEnumerable(Of String)) As List(Of String)
            Dim list As List(Of String)
            If Not Keywords.Any() Then
                list = Keywords.ToList()
            Else
                list = If(Me.AnalyzeModName(Keywords), Keywords.ToList())
            End If
            Return list
        End Function

        Public Sub Output(IsManualAnalyze As Boolean, Optional ExtraFiles As List(Of String) = Nothing)
            ModMain.FrmMain.ShowWindowToTop()
            Dim analyzeResult As String = Me.GetAnalyzeResult(IsManualAnalyze)
            Dim text As String = analyzeResult
            Dim text2 As String = If(IsManualAnalyze, "错误报告分析结果", "Minecraft 出现错误")
            Dim text3 As String = "确定"
            Dim text4 As String = If((IsManualAnalyze OrElse Me.DirectFile Is Nothing), "", "查看输出")
            Dim text5 As String = If(IsManualAnalyze, "", "导出错误报告")
            Dim flag As Boolean = False
            Dim flag2 As Boolean = True
            Dim flag3 As Boolean = False
            Dim action As Action = Nothing
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_10 = If((IsManualAnalyze OrElse Me.DirectFile Is Nothing), Nothing, New VB$AnonymousDelegate_10(Sub()
                If FileUtils.Exists(Me.DirectFile.Value.Key) Then
                    ModBase.StartProcess(Me.DirectFile.Value.Key, "")
                    Return
                End If
                Dim text9 As String = ModBase.PathTemp + "Crash.txt"
                FileUtils.Write(text9, Me.DirectFile.Value.Value.Join(vbCrLf), Nothing)
                ModBase.StartProcess(text9, "")
            End Sub))
            Dim num As Integer = ModMain.OurMsgBox(text, text2, text3, text4, text5, flag, flag2, flag3, action, If((vb$AnonymousDelegate_ Is Nothing), Nothing, AddressOf vb$AnonymousDelegate_.Invoke), Nothing)
            If num = 3 Then
                Dim CS$<>8__locals1 As CrashAnalyzer._Closure$__28-0 = New CrashAnalyzer._Closure$__28-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Local_FileAddress = Nothing
                Try
                    ModBase.RunInUiWait(Sub()
                        CS$<>8__locals1.$VB$Local_FileAddress = Dialogs.SaveFile("选择保存位置", "错误报告-" + DateTime.Now.ToString("G").Replace("/", "-").Replace(":", ".").Replace(" ", "_") + ".zip", Nothing, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)("zip", "Minecraft 错误报告") })
                    End Sub)
                    If CS$<>8__locals1.$VB$Local_FileAddress Is Nothing Then
                        Return
                    End If
                    FileUtils.Delete(CS$<>8__locals1.$VB$Local_FileAddress, False)
                    ModMain.FeedbackInfo()
                    Logger.Instance.Flush()
                    If ExtraFiles IsNot Nothing Then
                        Me.OutputFiles.AddRange(ExtraFiles)
                    End If
                    Try
                        For Each text6 As String In Me.OutputFiles
                            Dim text7 As String = PathUtils.GetLastPart(text6)
                            Dim encoding As Encoding = Nothing
                            If Operators.CompareString(text7, "LatestLaunch.bat", False) <> 0 Then
                                If Operators.CompareString(text7, "Log1.txt", False) <> 0 Then
                                    If Operators.CompareString(text7, "RawOutput.log", False) = 0 Then
                                        text7 = "游戏崩溃前的输出.txt"
                                        encoding = Encoding.UTF8
                                    End If
                                Else
                                    text7 = "PCL 启动器日志.txt"
                                    encoding = Encoding.UTF8
                                End If
                            Else
                                text7 = "启动脚本.bat"
                            End If
                            If FileUtils.Exists(text6) Then
                                Dim text8 As String = FileUtils.ReadAsString(text6, encoding, Nothing)
                                text8 = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(text8, Conversions.ToChar(If((Operators.CompareString(text7, "启动脚本.bat", False) = 0), "F", "*"))), "*"c)
                                FileUtils.Write(Me.TempFolder + "Report\" + text7, text8, encoding)
                                Logger.Info(String.Format("导出错误报告文件：{0}", text7), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                            End If
                        Next
                    Finally
                        Dim enumerator As List(Of String).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    FileUtils.CreateZipFromDirectory(CS$<>8__locals1.$VB$Local_FileAddress, Me.TempFolder + "Report\")
                    DirectoryUtils.Delete(Me.TempFolder + "Report\", False)
                    ModMain.Hint("错误报告已导出！", ModMain.HintType.Green, True)
                    ModBase.OpenExplorer(CS$<>8__locals1.$VB$Local_FileAddress)
                Catch ex As Exception
                    Logger.[Error](ex, "导出错误报告失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModCrash.vb")
                    Return
                End Try
            End If
            If Not IsManualAnalyze Then
                ModMain.Telemetry("Minecraft 崩溃", New String() { "Version", If((Me.TargetInstance Is Nothing), "null", Me.TargetInstance.VersionDisplayName()), "Detail", ModMinecraft.FilterUserName(analyzeResult, "*"c) })
            End If
        End Sub

        Private Function GetAnalyzeResult(IsHandAnalyze As Boolean) As String
            Dim text As String
            If Not Me.CrashReasons.Any() Then
                If IsHandAnalyze Then
                    text = "很抱歉，PCL 无法确定错误原因。"
                Else
                    text = String.Format("很抱歉，你的游戏出现了一些问题……{0}如果要寻求帮助，请把错误报告文件发给对方，而不是发送这个窗口的照片或者截图。", vbCrLf)
                End If
            Else
                Dim list As List(Of String) = New List(Of String)()
                Try
                    For Each keyValuePair As KeyValuePair(Of CrashAnalyzer.CrashReason, List(Of String)) In Me.CrashReasons
                        Dim value As List(Of String) = keyValuePair.Value
                        Select Case keyValuePair.Key
                            Case CrashAnalyzer.CrashReason.Java虚拟机参数有误
                                list.Add("由于 Java 参数有误，导致游戏无法继续运行。\n请检查高级选项中的 Java 虚拟机参数和游戏参数设置是否有误。")
                            Case CrashAnalyzer.CrashReason.Mod文件被解压
                                list.Add("由于 Mod 文件被解压了，导致游戏无法继续运行。\n直接把整个 Mod 文件放进 Mod 文件夹中即可，若解压就会导致游戏出错。\n\n请删除 Mod 文件夹中已被解压的 Mod，然后再启动游戏。")
                            Case CrashAnalyzer.CrashReason.MixinBootstrap缺失
                                list.Add("由于缺失 MixinBootstrap，导致游戏崩溃。\n请尝试安装 MixinBootstrap。若安装后依然崩溃，可以尝试在文件名前添加英文感叹号。")
                            Case CrashAnalyzer.CrashReason.内存不足
                                list.Add("Minecraft 内存不足，导致其无法继续运行。\n这很可能是因为电脑内存不足、游戏分配的内存不足，或是配置要求过高。\n\n你可以尝试在 更多 → 百宝箱 中选择 内存优化，然后再启动游戏。\n如果还是不行，请在启动设置中增加为游戏分配的内存，并删除配置要求较高的材质、Mod、光影。\n如果依然不奏效，请在开始游戏前尽量关闭其他软件，或者……换台电脑？\h")
                            Case CrashAnalyzer.CrashReason.使用JDK
                                list.Add("游戏似乎因为使用 JDK，或 Java 版本过高而崩溃了。\n请在版本设置的 Java 选项中改用 Java 8，然后再启动游戏。\n如果你没有安装 Java 8，可以从网络中下载、安装一个。")
                            Case CrashAnalyzer.CrashReason.显卡不支持OpenGL, CrashAnalyzer.CrashReason.显卡驱动不支持导致无法设置像素格式, CrashAnalyzer.CrashReason.Intel驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, CrashAnalyzer.CrashReason.AMD驱动不兼容导致EXCEPTION_ACCESS_VIOLATION, CrashAnalyzer.CrashReason.Nvidia驱动不兼容导致EXCEPTION_ACCESS_VIOLATION
                                If Me.LogAll.Contains("hd graphics ") Then
                                    list.Add("你的显卡驱动存在问题，或未使用独立显卡，导致游戏无法正常运行。\n\n如果你的电脑存在独立显卡，请使用独立显卡而非 Intel 核显启动 PCL 与 Minecraft。\n如果问题依然存在，请尝试升级你的显卡驱动到最新版本，或回退到出厂版本。\n如果还是不行，还可以尝试使用 8.0.51 或更低版本的 Java。\h")
                                Else
                                    list.Add("你的显卡驱动存在问题，导致游戏无法正常运行。\n\n请尝试升级你的显卡驱动到最新版本，或回退到出厂版本，然后再启动游戏。\n如果还是不行，可以尝试使用 8.0.51 或更低版本的 Java。\n如果问题依然存在，那么你可能需要换个更好的显卡……\h")
                                End If
                            Case CrashAnalyzer.CrashReason.使用OpenJ9
                                list.Add("游戏因为使用 OpenJ9 而崩溃了。\n请在版本设置的 Java 选项中改用非 OpenJ9 的 Java，然后再启动游戏。")
                            Case CrashAnalyzer.CrashReason.Java版本过高
                                list.Add("游戏似乎因为你所使用的 Java 版本过高而崩溃了。\n请在版本设置的 Java 选项中改用较低版本的 Java，然后再启动游戏。\n如果没有，可以从网络中下载、安装一个。")
                            Case CrashAnalyzer.CrashReason.Java版本不兼容
                                list.Add("游戏不兼容你当前使用的 Java。\n请在版本设置的 Java 选项中改用合适版本的 Java，然后再启动游戏。\n如果你没有安装任何 Mod 并且因此崩溃，请向 PCL 提交反馈。")
                            Case CrashAnalyzer.CrashReason.Mod名称包含特殊字符
                                list.Add("由于有 Mod 的名称包含特殊字符，导致游戏崩溃。\n请尝试修改 Mod 文件名，让它只包含英文字母、数字、减号（-）、下划线（_）和小数点，然后再启动游戏。")
                            Case CrashAnalyzer.CrashReason.极短的程序输出
                                list.Add(String.Format("程序返回了以下信息：\n{0}\n\h", value.First()))
                            Case CrashAnalyzer.CrashReason.玩家手动触发调试崩溃
                                list.Add("* 事实上，你的游戏没有任何问题，这是你自己触发的崩溃。\n* 你难道没有更重要的事要做吗？")
                            Case CrashAnalyzer.CrashReason.光影或资源包导致OpenGL1282错误
                                list.Add("你所使用的光影或材质导致游戏出现了一些问题……\n\n请尝试删除你所添加的这些额外资源。\h")
                            Case CrashAnalyzer.CrashReason.文件或内容校验失败
                                list.Add("部分文件或内容校验失败，导致游戏出现了问题。\n\n请尝试删除游戏（包括 Mod）并重新下载，或尝试在重新下载时使用 VPN。\h")
                            Case CrashAnalyzer.CrashReason.确定Mod导致游戏崩溃
                                If value.IsSingle() Then
                                    list.Add("名为 " + value.First() + " 的 Mod 导致了游戏出错。\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                Else
                                    list.Add("以下 Mod 导致了游戏出错：\n - " + value.Join("\n - ") + "\n\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                End If
                            Case CrashAnalyzer.CrashReason.怀疑Mod导致游戏崩溃, CrashAnalyzer.CrashReason.堆栈分析发现Mod名称
                                If value.IsSingle() Then
                                    list.Add("PCL 怀疑名为 " + value.First() + " 的 Mod 导致了游戏出错，但不能完全确定。\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                Else
                                    list.Add("PCL 怀疑以下 Mod 导致了游戏出错，但不能完全确定：\n - " + value.Join("\n - ") + "\n\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                End If
                            Case CrashAnalyzer.CrashReason.Mod配置文件导致游戏崩溃
                                If value(1) Is Nothing Then
                                    list.Add("名为 " + value.First() + " 的 Mod 导致了游戏出错。\n\e\h")
                                Else
                                    list.Add(String.Concat(New String() { "名为 ", value.First(), " 的 Mod 导致了游戏出错：\n其配置文件 ", value(1), " 存在异常，无法读取。" }))
                                End If
                            Case CrashAnalyzer.CrashReason.ModMixin失败
                                If value.Count = 0 Then
                                    list.Add("部分 Mod 注入失败，导致游戏出错。\n这一般代表着部分 Mod 与其他 Mod 或当前环境不兼容，或是它存在 Bug。\n你可以尝试逐步禁用 Mod，然后观察游戏是否还会崩溃，以此定位导致崩溃的 Mod。\n\e\h")
                                ElseIf value.IsSingle() Then
                                    list.Add("名为 " + value.First() + " 的 Mod 注入失败，导致游戏出错。\n这一般代表着它与其他 Mod 或当前环境不兼容，或是它存在 Bug。\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                Else
                                    list.Add("以下 Mod 导致了游戏出错：\n - " + value.Join("\n - ") + "\n这一般代表着它们与其他 Mod 或当前环境不兼容，或是它存在 Bug。\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                End If
                            Case CrashAnalyzer.CrashReason.Mod加载器报错
                                If value.IsSingle() Then
                                    list.Add("Mod 加载器提供了以下错误信息：\n" + value.First() + "\n\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。")
                                Else
                                    list.Add("Mod 加载器可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\h")
                                End If
                            Case CrashAnalyzer.CrashReason.Mod初始化失败
                                If value.IsSingle() Then
                                    list.Add("名为 " + value.First() + " 的 Mod 初始化失败，导致游戏无法继续加载。\n你可以尝试禁用此 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                Else
                                    list.Add("以下 Mod 初始化失败，导致游戏出错：\n - " + value.Join("\n - ") + "\n\n你可以尝试依次禁用上述 Mod，然后观察游戏是否还会崩溃。\n\e\h")
                                End If
                            Case CrashAnalyzer.CrashReason.堆栈分析发现关键字
                                If value.IsSingle() Then
                                    list.Add("你的游戏遇到了一些问题，PCL 为此找到了一个可疑的关键词：" + value.First() + "。\n\n如果你知道某个关键词对应的 Mod，那么有可能就是它引起的错误，你也可以查看错误报告获取详情。\h")
                                Else
                                    list.Add("你的游戏遇到了一些问题，PCL 为此找到了以下可疑的关键词：\n - " + value.Join(", ") + "\n\n如果你知道某个关键词对应的 Mod，那么有可能就是它引起的错误，你也可以查看错误报告获取详情。\h")
                                End If
                            Case CrashAnalyzer.CrashReason.OptiFine导致无法加载世界
                                list.Add("你所使用的 OptiFine 可能导致了你的游戏出现问题。\n\n该问题只在特定 OptiFine 版本中出现，你可以尝试更换 OptiFine 的版本。\h")
                            Case CrashAnalyzer.CrashReason.特定方块导致崩溃
                                If value.IsSingle() Then
                                    list.Add("游戏似乎因为方块 " + value.First() + " 出现了问题。\n\n你可以创建一个新世界，并观察游戏的运行情况：\n - 若正常运行，则是该方块导致出错，你或许需要使用一些方式删除此方块。\n - 若仍然出错，问题就可能来自其他原因……\h")
                                Else
                                    list.Add("游戏似乎因为世界中的某些方块出现了问题。\n\n你可以创建一个新世界，并观察游戏的运行情况：\n - 若正常运行，则是某些方块导致出错，你或许需要删除该世界。\n - 若仍然出错，问题就可能来自其他原因……\h")
                                End If
                            Case CrashAnalyzer.CrashReason.特定实体导致崩溃
                                If value.IsSingle() Then
                                    If value.First().Contains("minecraft:player") Then
                                        If value.First().Contains(" ") Then
                                            list.Add(String.Format("游戏因为位于 {0} 的玩家实体导致了崩溃。\h", value.First().AfterFirst(" ", False)))
                                        Else
                                            list.Add("游戏因为玩家实体导致了崩溃。\h")
                                        End If
                                    Else
                                        list.Add(String.Format("游戏因为实体 {0} 导致了崩溃。\h", value.First()))
                                    End If
                                Else
                                    list.Add(String.Format("游戏因为下列实体导致了崩溃：{0}\h", value.Join("、"c)))
                                End If
                            Case CrashAnalyzer.CrashReason.材质过大或显卡配置不足
                                list.Add("你所使用的材质分辨率过高，或显卡配置不足，导致游戏无法继续运行。\n\n如果你正在使用高清材质，请将它移除。\n如果你没有使用材质，那么你可能需要更新显卡驱动，或者换个更好的显卡……\h")
                            Case CrashAnalyzer.CrashReason.没有可用的分析文件
                                list.Add("你的游戏出现了一些问题，但 PCL 未能找到相关记录文件，因此无法进行分析。\h")
                            Case CrashAnalyzer.CrashReason.使用32位Java导致JVM无法分配足够多的内存
                                If Environment.Is64BitOperatingSystem Then
                                    list.Add("你似乎正在使用 32 位 Java，这会导致 Minecraft 无法使用所需的内存，进而造成崩溃。\n\n请在启动设置的 Java 选择一项中改用 64 位的 Java 再启动游戏，然后再启动游戏。\n如果你没有安装 64 位的 Java，你可以从网络中下载、安装一个。")
                                Else
                                    list.Add("你正在使用 32 位的操作系统，这会导致 Minecraft 无法使用所需的内存，进而造成崩溃。\n\n你或许只能重装 64 位的操作系统来解决此问题。\n如果你的电脑内存在 2GB 以内，那或许只能换台电脑了……\h")
                                End If
                            Case CrashAnalyzer.CrashReason.Mod重复安装
                                If value.Count >= 2 Then
                                    list.Add("你重复安装了多个相同的 Mod：\n - " + value.Join("\n - ") + "\n\n每个 Mod 只能出现一次，请删除重复的 Mod，然后再启动游戏。")
                                Else
                                    list.Add("你可能重复安装了多个相同的 Mod，导致游戏出错。\n\n每个 Mod 只能出现一次，请删除重复的 Mod，然后再启动游戏。\e\h")
                                End If
                            Case CrashAnalyzer.CrashReason.Mod互不兼容
                                If value.IsSingle() Then
                                    list.Add("你所安装的 Mod 不兼容：\n" + value.First() + "\n\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。")
                                Else
                                    list.Add("你所安装的 Mod 不兼容，Mod 加载器可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\h")
                                End If
                            Case CrashAnalyzer.CrashReason.OptiFine与Forge不兼容
                                list.Add("由于 OptiFine 与当前版本的 Forge 不兼容，导致了游戏崩溃。\n\n请前往 OptiFine 官网（https://optifine.net/downloads）查看 OptiFine 所兼容的 Forge 版本，并严格按照对应版本重新安装游戏。")
                            Case CrashAnalyzer.CrashReason.Fabric报错
                                If value.IsSingle() Then
                                    list.Add("Fabric 提供了以下错误信息：\n" + value.First() + "\n\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。")
                                Else
                                    list.Add("Fabric 可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\h")
                                End If
                            Case CrashAnalyzer.CrashReason.Fabric报错并给出解决方案
                                If value.IsSingle() Then
                                    list.Add("Fabric 提供了以下解决方案：\n" + value.First() + "\n\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。")
                                Else
                                    list.Add("Fabric 可能已经提供了解决方案，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\h")
                                End If
                            Case CrashAnalyzer.CrashReason.Forge报错
                                If value.IsSingle() Then
                                    list.Add("Forge 提供了以下错误信息：\n" + value.First() + "\n\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。")
                                Else
                                    list.Add("Forge 可能已经提供了错误信息，请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\h")
                                End If
                            Case CrashAnalyzer.CrashReason.低版本Forge与高版本Java不兼容
                                list.Add("由于低版本 Forge 与当前 Java 不兼容，导致了游戏崩溃。\n\n请尝试以下解决方案：\n - 更新 Forge 到 36.2.26 或更高版本\n - 换用版本低于 1.8.0.320 的 Java")
                            Case CrashAnalyzer.CrashReason.版本Json中存在多个Forge
                                list.Add("可能由于其他启动器修改了 Forge 版本，当前版本的文件存在异常，导致了游戏崩溃。\n请尝试重新全新安装 Forge，而非使用其他启动器修改 Forge 版本。")
                            Case CrashAnalyzer.CrashReason.Mod过多导致超出ID限制
                                list.Add("你所安装的 Mod 过多，超出了游戏的 ID 限制，导致了游戏崩溃。\n请尝试安装 JEID 等修复 Mod，或删除部分大型 Mod。")
                            Case CrashAnalyzer.CrashReason.NightConfig的Bug
                                list.Add("由于 Night Config 存在问题，导致了游戏崩溃。\n你可以尝试安装 Night Config Fixes 模组，这或许能解决此问题。\h")
                            Case CrashAnalyzer.CrashReason.ShadersMod与OptiFine同时安装
                                list.Add("无需同时安装 OptiFine 和 Shaders Mod，OptiFine 已经集成了 Shaders Mod 的功能。\n在删除 Shaders Mod 后，游戏即可正常运行。")
                            Case CrashAnalyzer.CrashReason.Forge安装不完整
                                list.Add("由于安装的 Forge 文件丢失，导致游戏无法正常运行。\n请重新安装一次相同版本的 Forge，然后再启动游戏。\n在打包游戏时删除 libraries 文件夹可能导致此错误。\h")
                            Case CrashAnalyzer.CrashReason.Mod需要Java11
                                list.Add("你所安装的部分 Mod 似乎需要使用 Java 11 启动。\n请在启动设置的 Java 选择一项中改用 Java 11，然后再启动游戏。\n如果你没有安装 Java 11，你可以从网络中下载、安装一个。")
                            Case CrashAnalyzer.CrashReason.Mod缺少前置或MC版本错误
                                If value.Any() Then
                                    list.Add("由于未安装正确的前置 Mod，导致游戏退出。\n缺失的依赖项：\n - " + value.Join("\n - ") + "\n\n请根据上述信息进行对应处理，如果看不懂英文可以使用翻译软件。")
                                Else
                                    list.Add("由于未安装正确的前置 Mod，导致游戏退出。\n请根据错误报告中的日志信息进行对应处理，如果看不懂英文可以使用翻译软件。\h")
                                End If
                            Case Else
                                list.Add("PCL 获取到了没有详细信息的错误原因（" + Conversions.ToString(CInt(Me.CrashReasons.First().Key)) + "），请向 PCL 作者提交反馈以获取详情。\h")
                        End Select
                    Next
                Finally
                    Dim enumerator As Dictionary(Of CrashAnalyzer.CrashReason, List(Of String)).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Dim text2 As String = list.Join("\n\n此外，").Replace("\n", vbCrLf).Replace("\h", "").Replace("\e", If(IsHandAnalyze, "", vbCrLf & "你可以查看错误报告了解错误具体是如何发生的。")).ReplaceLineEndings(vbCrLf, False).Trim(vbCrLf.ToCharArray())
                Dim enumerable As IEnumerable(Of String) = list
                Dim func As Func(Of String, Boolean)
                If CrashAnalyzer._Closure$__.$I29-0 IsNot Nothing Then
                    func = CrashAnalyzer._Closure$__.$I29-0
                Else
                    Dim func2 As Func(Of String, Boolean) = Function(r As String) r.EndsWithF("\h", False)
                    func = func2
                    CrashAnalyzer._Closure$__.$I29-0 = func2
                End If
                text = text2 + If((Not enumerable.Any(func) OrElse IsHandAnalyze), "", (vbCrLf & "如果要寻求帮助，请把错误报告文件发给对方，而不是发送这个窗口的照片或者截图。" + If(PageSetupSystem.IsLauncherNewest().GetValueOrDefault(True), "", vbCrLf & vbCrLf & "此外，你正在使用老版本 PCL，更新 PCL 或许也能解决这个问题。" & vbCrLf & "你可以点击 [设置 → 其他 → 启动器 → 检查更新] 更新 PCL。")))
            End If
            Return text
        End Function

        Private TempFolder As String

        Private TargetInstance As ModMinecraft.McInstance

        Private AnalyzeRawFiles As List(Of KeyValuePair(Of String, String()))

        Private DirectFile As KeyValuePair(Of String, String())?

        Private LogMc As String

        Private LogMcDebug As String

        Private LogHs As String

        Private LogCrash As String

        Private LogAll As String

        Private CrashReasons As Dictionary(Of CrashAnalyzer.CrashReason, List(Of String))

        Private OutputFiles As List(Of String)

        Private Enum AnalyzeFileType
            HsErr
            MinecraftLog
            ExtraLogFile
            ExtraReportFile
            CrashReport
        End Enum

        Private Enum CrashReason
            Java虚拟机参数有误
            Mod文件被解压
            MixinBootstrap缺失
            内存不足
            使用JDK
            显卡不支持OpenGL
            使用OpenJ9
            Java版本过高
            Java版本不兼容
            Mod名称包含特殊字符
            显卡驱动不支持导致无法设置像素格式
            极短的程序输出
            Intel驱动不兼容导致EXCEPTION_ACCESS_VIOLATION
            AMD驱动不兼容导致EXCEPTION_ACCESS_VIOLATION
            Nvidia驱动不兼容导致EXCEPTION_ACCESS_VIOLATION
            玩家手动触发调试崩溃
            光影或资源包导致OpenGL1282错误
            文件或内容校验失败
            确定Mod导致游戏崩溃
            怀疑Mod导致游戏崩溃
            Mod配置文件导致游戏崩溃
            ModMixin失败
            Mod加载器报错
            Mod初始化失败
            堆栈分析发现关键字
            堆栈分析发现Mod名称
            OptiFine导致无法加载世界
            特定方块导致崩溃
            特定实体导致崩溃
            材质过大或显卡配置不足
            没有可用的分析文件
            使用32位Java导致JVM无法分配足够多的内存
            Mod重复安装
            Mod互不兼容
            OptiFine与Forge不兼容
            Fabric报错
            Fabric报错并给出解决方案
            Forge报错
            低版本Forge与高版本Java不兼容
            版本Json中存在多个Forge
            Mod过多导致超出ID限制
            NightConfig的Bug
            ShadersMod与OptiFine同时安装
            Forge安装不完整
            Mod需要Java11
            Mod缺少前置或MC版本错误
        End Enum
    End Class
End Namespace
