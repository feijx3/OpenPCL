Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Threading
Imports System.Xaml
Imports System.Xml.Linq
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My
Imports PCL.My.Resources
Imports ThrottleDebounce

Namespace PCL
    Public NotInheritable Module ModBase
        Public ReadOnly Property ModeDebug As Boolean
            Get
                Return Settings.[Get](Of Boolean)("SystemDebugMode", Nothing)
            End Get
        End Property

        Public Sub IniClearCache(FileName As String)
            If Not FileName.Contains(":\") Then
                FileName = String.Format("{0}PCL\{1}.ini", Paths.Base, FileName)
            End If
            ModBase.IniCache.Remove(FileName)
        End Sub

        Private Function IniGetContent(FileName As String) As ConcurrentDictionary(Of String, String)
            Dim concurrentDictionary2 As ConcurrentDictionary(Of String, String)
            Try
                If Not FileName.Contains(":\") Then
                    FileName = String.Format("{0}PCL\{1}.ini", Paths.Base, FileName)
                End If
                Dim concurrentDictionary As ConcurrentDictionary(Of String, String) = Nothing
                If ModBase.IniCache.TryGetValue(FileName, concurrentDictionary) Then
                    concurrentDictionary2 = concurrentDictionary
                ElseIf Not FileUtils.Exists(FileName) Then
                    concurrentDictionary2 = Nothing
                Else
                    Dim concurrentDictionary3 As ConcurrentDictionary(Of String, String) = New ConcurrentDictionary(Of String, String)()
                    For Each text As String In FileUtils.ReadAsLines(FileName, True, Nothing, Nothing)
                        Dim num As Integer = text.IndexOfF(":", False)
                        If num > 0 Then
                            ' The following expression was wrapped in a checked-expression
                            concurrentDictionary3(text.Substring(0, num)) = text.Substring(num + 1)
                        End If
                    Next
                    ModBase.IniCache(FileName) = concurrentDictionary3
                    concurrentDictionary2 = concurrentDictionary3
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("生成 ini 文件缓存失败（{0}）", FileName), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                concurrentDictionary2 = Nothing
            End Try
            Return concurrentDictionary2
        End Function

        Public Function ReadIni(FileName As String, Key As String, Optional DefaultValue As String = "") As String
            Dim concurrentDictionary As ConcurrentDictionary(Of String, String) = ModBase.IniGetContent(FileName)
            Dim text As String
            If concurrentDictionary Is Nothing Then
                text = DefaultValue
            Else
                text = concurrentDictionary.GetOrDefault(Key, DefaultValue)
            End If
            Return text
        End Function

        Public Function HasIniKey(FileName As String, Key As String) As Boolean
            Dim concurrentDictionary As ConcurrentDictionary(Of String, String) = ModBase.IniGetContent(FileName)
            Return concurrentDictionary IsNot Nothing AndAlso concurrentDictionary.ContainsKey(Key)
        End Function

        Public Sub DeleteIniKey(FileName As String, Key As String)
            ModBase.WriteIni(FileName, Key, Nothing)
        End Sub

        Public Sub WriteIni(FileName As String, Key As String, Value As String)
            Try
                If Key.Contains(":") Then
                    Throw New Exception(String.Format("尝试写入 ini 文件 {0} 的键名中包含了冒号：{1}", FileName, Key))
                End If
                Key = Key.ReplaceLineEndings("", False)
                Value = If((Value IsNot Nothing), Value.ReplaceLineEndings("", False), Nothing)
                Dim concurrentDictionary As ConcurrentDictionary(Of String, String) = ModBase.IniGetContent(FileName)
                If concurrentDictionary Is Nothing Then
                    concurrentDictionary = New ConcurrentDictionary(Of String, String)()
                End If
                If Value Is Nothing Then
                    If Not concurrentDictionary.Remove(Key) Then
                        Return
                    End If
                Else
                    Dim text As String = Nothing
                    If concurrentDictionary.TryGetValue(Key, text) AndAlso Operators.CompareString(text, Value, False) = 0 Then
                        Return
                    End If
                    concurrentDictionary(Key) = Value
                End If
                Dim stringBuilder As StringBuilder = New StringBuilder()
                Try
                    For Each keyValuePair As KeyValuePair(Of String, String) In concurrentDictionary
                        stringBuilder.Append(keyValuePair.Key)
                        stringBuilder.Append(":")
                        stringBuilder.Append(keyValuePair.Value)
                        stringBuilder.Append(vbCrLf)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of KeyValuePair(Of String, String))
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If Not FileName.Contains(":\") Then
                    FileName = String.Format("{0}PCL\{1}.ini", Paths.Base, FileName)
                End If
                FileName = If(FileName.Contains(":\"), FileName, (Paths.Base + FileName))
                FileUtils.Write(FileName, stringBuilder.ToString(), Nothing)
            Catch ex As Exception
                Logger.[Error](ex, String.Format("写入文件失败（{0} → {1}:{2}）", FileName, Key, Value), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
            End Try
        End Sub

        Public Function CheckPermission(Folder As String) As Boolean
            Dim flag As Boolean
            Try
                If String.IsNullOrEmpty(Folder) Then
                    flag = False
                Else
                    Folder = PathUtils.AddSlashSuffix(Folder)
                    If Folder.EndsWithF(":\System Volume Information\", True) OrElse Folder.EndsWithF(":\$RECYCLE.BIN\", True) Then
                        flag = False
                    ElseIf Not DirectoryUtils.Exists(Folder) Then
                        flag = False
                    Else
                        Dim text As String = String.Format("{0}CheckPermission{1}.txt", Folder, ModBase.GetUuid())
                        FileUtils.Write(text, "临时文件，用于检测该文件夹的权限是否正常。", Nothing)
                        FileUtils.Delete(text, False)
                        flag = True
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, String.Format("没有对文件夹 {0} 的权限，请尝试以管理员权限运行 PCL", Folder), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                flag = False
            End Try
            Return flag
        End Function

        Public Sub CheckPermissionWithException(Folder As String)
            If String.IsNullOrWhiteSpace(Folder) Then
                Throw New ArgumentNullException("文件夹名不能为空！")
            End If
            Folder = PathUtils.AddSlashSuffix(Folder)
            If Folder.EndsWithF(":\System Volume Information\", True) OrElse Folder.EndsWithF(":\$RECYCLE.BIN\", True) Then
                Throw New UnauthorizedAccessException("没有对系统文件夹的权限！")
            End If
            If Not DirectoryUtils.Exists(Folder) Then
                Throw New DirectoryNotFoundException("文件夹不存在！")
            End If
            Dim text As String = String.Format("{0}CheckPermission{1}.txt", Folder, ModBase.GetUuid())
            FileUtils.Write(text, "临时文件，用于检测该文件夹的权限是否正常。", Nothing)
            FileUtils.Delete(text, False)
        End Sub

        Public Function Val(Str As Object) As Double
            Dim num As Double
            If Str Is Nothing Then
                num = 0.0
            Else
                Try
                    num = If((TypeOf Str Is String AndAlso Operators.ConditionalCompareObjectEqual(Str, "&", False)), 0.0, Conversion.Val(RuntimeHelpers.GetObjectValue(Str)))
                Catch ex As Exception
                    num = 0.0
                End Try
            End If
            Return num
        End Function

        Private Function SearchSimilarity(Source As String, Query As String) As Double
            Dim num As Double
            If String.IsNullOrEmpty(Source) OrElse String.IsNullOrEmpty(Query) Then
                num = 0.0
            Else
                Dim i As Integer = 0
                Dim num2 As Double = 0.0
                Dim stringBuilder As StringBuilder = New StringBuilder(Source.Length)
                stringBuilder.Append(Source.Lower().Replace(" ", ""))
                Query = Query.Lower().Replace(" ", "")
                Dim length As Integer = stringBuilder.Length
                Dim length2 As Integer = Query.Length
                If length2 = 0 Then
                    num = 0.0
                Else
                    ' The following expression was wrapped in a checked-statement
                    While i < length2
                        Dim j As Integer = 0
                        Dim num3 As Integer = 0
                        Dim num4 As Integer = 0
                        Dim length3 As Integer = stringBuilder.Length
                        While j < length3
                            Dim num5 As Integer = 0
                            While i + num5 < length2 AndAlso j + num5 < length3 AndAlso stringBuilder(j + num5) = Query(i + num5)
                                num5 += 1
                            End While
                            If num5 > num3 Then
                                num3 = num5
                                num4 = j
                            End If
                            j += If((num5 > 0), num5, 1)
                        End While

                            If num3 > 0 Then
                                stringBuilder.Remove(num4, num3)
                                Dim num6 As Double = Math.Pow(1.4, CDbl((3 + num3))) - 3.6
                                num6 *= 1.0 + 0.3 * CDbl(Math.Max(0, 3 - Math.Abs(i - num4)))
                                num2 += num6
                            End If

                        i += If((num3 > 0), num3, 1)
                    End While
                    num = num2 / CDbl(length2) * (3.0 / Math.Sqrt(CDbl((length + 15)))) * CDbl(If((length2 <= 2), 3 - length2, 1))
                End If
            End If
            Return num
        End Function

        Private Function SearchSimilarityWeighted(Source As List(Of ModBase.SearchSource), Query As String) As Double
            Dim CS$<>8__locals1 As ModBase._Closure$__39-0 = New ModBase._Closure$__39-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Query = Query
            Dim num As Double = 0.0
            Dim num2 As Double = 0.0
            Try
                For Each searchSource As ModBase.SearchSource In Source
                    If searchSource.Aliases.Any() Then
                        Dim num3 As Double = num2
                        Dim aliases As IEnumerable(Of String) = searchSource.Aliases
                        Dim func As Func(Of String, Double)
                        If CS$<>8__locals1.$I0 IsNot Nothing Then
                            func = CS$<>8__locals1.$I0
                        Else
                            Dim CS$<>8__locals2 As ModBase._Closure$__39-0 = CS$<>8__locals1
                            Dim func2 As Func(Of String, Double) = Function(a As String) ModBase.SearchSimilarity(a, CS$<>8__locals1.$VB$Local_Query)
                            Dim func3 As Func(Of String, Double) = func2
                            CS$<>8__locals2.$I0 = func2
                            func = func3
                        End If
                        num2 = num3 + aliases.Max(func) * searchSource.Weight
                    End If
                    num += searchSource.Weight
                Next
            Finally
                Dim enumerator As List(Of ModBase.SearchSource).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Return num2 / num
        End Function

        Public Function Search(Of T)(Entries As List(Of ModBase.SearchEntry(Of T)), Query As String, Optional MaxBlurCount As Integer = 5, Optional MinBlurSimilarity As Double = 0.1) As List(Of ModBase.SearchEntry(Of T))
            Dim list As List(Of ModBase.SearchEntry(Of T)) = New List(Of ModBase.SearchEntry(Of T))()
            Dim list2 As List(Of ModBase.SearchEntry(Of T))
            If Not Entries.Any() Then
                list2 = list
            Else
                Dim array As String() = Query.Split(" ", True)
                Dim list3 As List(Of ModBase.SearchEntry(Of T)) = New List(Of ModBase.SearchEntry(Of T))()
                Try
                    Dim enumerator As List(Of ModBase.SearchEntry(Of T)).Enumerator = Entries.GetEnumerator()
                    While enumerator.MoveNext()
                        Dim CS$<>8__locals1 As ModBase._Closure$__42-0(Of T) = New ModBase._Closure$__42-0(Of T)(CS$<>8__locals1)
                        CS$<>8__locals1.$VB$Local_Entry = enumerator.Current
                        CS$<>8__locals1.$VB$Local_Entry.Similarity = ModBase.SearchSimilarityWeighted(CS$<>8__locals1.$VB$Local_Entry.SearchSource, Query)
                        CS$<>8__locals1.$VB$Local_Entry.AbsoluteRight = array.All(Function(QueryPart As String)
                            Dim CS$<>8__locals2 As ModBase._Closure$__42-1(Of $CLS0) = New ModBase._Closure$__42-1(Of $CLS0)(CS$<>8__locals2)
                            CS$<>8__locals2.$VB$Local_QueryPart = QueryPart
                            Return CS$<>8__locals1.$VB$Local_Entry.SearchSource.Any(Function(Source As ModBase.SearchSource)
                                Dim aliases As IEnumerable(Of String) = Source.Aliases
                                Dim func3 As Func(Of String, Boolean)
                                If CS$<>8__locals2.$I2 IsNot Nothing Then
                                    func3 = CS$<>8__locals2.$I2
                                Else
                                    Dim func4 As Func(Of String, Boolean) = Function([Alias] As String) [Alias].Replace(" ", "").ContainsIgnoreCase(CS$<>8__locals2.$VB$Local_QueryPart)
                                    Dim func5 As Func(Of String, Boolean) = func4
                                    CS$<>8__locals2.$I2 = func4
                                    func3 = func5
                                End If
                                Return aliases.Any(func3)
                            End Function)
                        End Function)
                        If CS$<>8__locals1.$VB$Local_Entry.AbsoluteRight OrElse CS$<>8__locals1.$VB$Local_Entry.Similarity >= MinBlurSimilarity Then
                            list3.Add(CS$<>8__locals1.$VB$Local_Entry)
                        End If
                    End While
                Finally
                    Dim enumerator As List(Of ModBase.SearchEntry(Of T)).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Dim list4 As IList(Of ModBase.SearchEntry(Of T)) = list3
                Dim func As Func(Of ModBase.SearchEntry(Of T), ModBase.SearchEntry(Of T), Boolean)
                If ModBase._Closure$__42(Of T).$I42-3 IsNot Nothing Then
                    func = ModBase._Closure$__42(Of T).$I42-3
                Else
                    Dim func2 As Func(Of ModBase.SearchEntry(Of T), ModBase.SearchEntry(Of T), Boolean) = Function(Left As ModBase.SearchEntry(Of $CLS0), Right As ModBase.SearchEntry(Of $CLS0))
                        If Left.AbsoluteRight = Right.AbsoluteRight Then
                            Return Left.Similarity > Right.Similarity
                        End If
                        Return Left.AbsoluteRight
                    End Function
                    func = func2
                    ModBase._Closure$__42(Of T).$I42-3 = func2
                End If
                list3 = list4.SortByComparison(func)
                Dim num As Integer = 0
                Try
                    For Each searchEntry As ModBase.SearchEntry(Of T) In list3
                        If searchEntry.AbsoluteRight Then
                            list.Add(searchEntry)
                        Else
                            If num = MaxBlurCount Then
                                Exit For
                            End If
                            list.Add(searchEntry)
                            num += 1
                        End If
                    Next
                Finally
                    Dim enumerator2 As List(Of ModBase.SearchEntry(Of T)).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                list2 = list
            End If
            Return list2
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function GetResultWithTimeout(Of T)(TargetTask As Task(Of T), TokenSource As CancellationTokenSource, TimeoutMs As Integer) As T
            Dim task As Task = Task.Delay(TimeoutMs)
            If Task.WhenAny(New Task() { TargetTask, task }).ConfigureAwait(False).GetAwaiter().GetResult() Is task Then
                TokenSource.Cancel()
                Throw New TimeoutException(String.Format("任务超时（{0} ms）", TimeoutMs))
            End If
            Return TargetTask.Run()
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub GetResultWithTimeout(TargetTask As Task, TokenSource As CancellationTokenSource, TimeoutMs As Integer)
            Dim task As Task = Task.Delay(TimeoutMs)
            If Task.WhenAny(New Task() { TargetTask, task }).ConfigureAwait(False).GetAwaiter().GetResult() Is task Then
                TokenSource.Cancel()
                Throw New TimeoutException(String.Format("任务超时（{0} ms）", TimeoutMs))
            End If
            TargetTask.Run()
        End Sub

        Public Function RunAsAdmin(Argument As String) As Integer
            Dim process As Process = ModBase.StartProcess(New ProcessStartInfo(ModBase.PathExe) With { .Verb = "runas", .Arguments = Argument })
            process.WaitForExit()
            Return process.ExitCode
        End Function

        Public Function GetUuid() As Integer
            If ModBase.UuidLock Is Nothing Then
                ModBase.UuidLock = RuntimeHelpers.GetObjectValue(New Object())
            End If
            Dim uuidLock As Object = ModBase.UuidLock
            ObjectFlowControl.CheckForSyncLockOnValueType(uuidLock)
            Dim uuid As Integer
            SyncLock uuidLock
                ModBase.Uuid += 1
                uuid = ModBase.Uuid
            End SyncLock
            Return uuid
        End Function

        Public Function GetTimeMs() As Long
            Return Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000L)
        End Function

        Public Function GetUnixTimestampUtc() As Long
            ' The following expression was wrapped in a checked-expression
            Return CLng(Math.Round(CDbl((DateAndTime.Now.ToUniversalTime().Ticks - 621355968000000000L)) / 10000000.0))
        End Function

        Public Function ToUnixTimestampUtc(Time As DateTime) As Long
            ' The following expression was wrapped in a checked-expression
            Return CLng(Math.Round(CDbl((Time.ToUniversalTime().Ticks - 621355968000000000L)) / 10000000.0))
        End Function

        Public Function GetDate(timeStamp As Integer) As DateTime
            Return DateTimeOffset.FromUnixTimeSeconds(CLng(timeStamp)).LocalDateTime
        End Function

        Public Function GetLocalTime(UtcDate As DateTime) As DateTime
            Return DateTime.SpecifyKind(UtcDate, DateTimeKind.Utc).ToLocalTime()
        End Function

        Public Function StartProcess(FileName As String, Optional Arguments As String = "") As Process
            Return ModBase.StartProcess(New ProcessStartInfo() With { .FileName = FileName, .Arguments = Arguments })
        End Function

        Public Function StartProcess(StartInfo As ProcessStartInfo) As Process
            StartInfo.FileName = PathUtils.ToShortPath(StartInfo.FileName)
            If Not String.IsNullOrEmpty(StartInfo.WorkingDirectory) Then
                StartInfo.WorkingDirectory = PathUtils.ToShortPath(StartInfo.WorkingDirectory)
            End If
            Logger.Info(String.Format("启动进程：{0} {1}", StartInfo.FileName, StartInfo.Arguments), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
            Return Process.Start(StartInfo)
        End Function

        Public Function RunInNewThread(Action As Action, Optional Name As String = Nothing, Optional Priority As ThreadPriority = ThreadPriority.Normal) As Thread
            Dim thread As Thread = New Thread(Sub()
                Try
                    Action()
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("{0}：线程执行失败", Name), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                End Try
            End Sub)
            thread.Name = If(Name, ("Runtime New Invoke " + Conversions.ToString(ModBase.GetUuid()) + "#"))
            thread.Priority = Priority
            thread.Start()
            Return thread
        End Function

        Public Function RunInUiWait(Of Output)(Action As Func(Of Output)) As Output
            Dim output As Output
            If ModBase.RunInUi() Then
                output = Action()
            Else
                output = Application.Current.Dispatcher.Invoke(Of Output)(Action)
            End If
            Return output
        End Function

        Public Sub RunInUiWait(Action As Action)
            If ModBase.RunInUi() Then
                Action()
                Return
            End If
            Application.Current.Dispatcher.Invoke(Action)
        End Sub

        Public Sub RunInUi(Action As Action, Optional ForceWaitUntilLoaded As Boolean = False)
            If ForceWaitUntilLoaded Then
                Application.Current.Dispatcher.InvokeAsync(Action, DispatcherPriority.Loaded)
                Return
            End If
            If ModBase.RunInUi() Then
                Action()
                Return
            End If
            Application.Current.Dispatcher.InvokeAsync(Action)
        End Sub

        Public Sub RunInThread(Action As Action)
            If ModBase.RunInUi() Then
                ModBase.RunInNewThread(Action, "Invoke " + Conversions.ToString(ModBase.GetUuid()), ThreadPriority.Normal)
                Return
            End If
            Action()
        End Sub

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function SortByComparison(Of T)(List As IList(Of T), SortRule As Func(Of T, T, Boolean)) As List(Of T)
            Dim list As List(Of T) = New List(Of T)()
            While List.Any()
                Dim t As T = List(0)
                Dim num As Integer = List.Count - 1
                For i As Integer = 1 To num
                    If SortRule(List(i), t) Then
                        t = List(i)
                    End If
                Next
                List.Remove(t)
                list.Add(t)
            End While
            Return list
        End Function

        Public Function GetProgramArgument(Name As String, Optional DefaultValue As Object = "") As Object
            Dim array As String() = Interaction.Command().Split(" ", False)
            Dim num As Integer = array.Length - 1
            For i As Integer = 0 To num
                If Operators.CompareString(array(i), "-" + Name, False) = 0 Then
                    Dim obj As Object
                    If array.Length = i + 1 OrElse array(i + 1).StartsWithF("-", False) Then
                        obj = True
                    Else
                        obj = array(i + 1)
                    End If
                    Return obj
                End If
            Next
            Return DefaultValue
        End Function

        Public Sub OpenWebsite(Url As String)
            Try
                If Not Url.StartsWithF("http", True) AndAlso Not Url.StartsWithF("minecraft://", True) AndAlso Not Url.StartsWithF("minecraft-preview://", True) Then
                    Throw New Exception(Url + " 不是一个有效的网址，它必须以 http 开头！")
                End If
                Logger.Info(String.Format("正在打开网页：{0}", Url), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                ModBase.StartProcess(Url, "")
            Catch ex As Exception
                Logger.Warn(ex, String.Format("无法打开网页（{0}）", Url), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                ModBase.ClipboardSet(Url, False)
                ModMain.OurMsgBox("可能由于浏览器未正确配置，PCL 无法为你打开网页。" & vbCrLf & "网址已经复制到剪贴板，若有需要可以手动粘贴访问。" & vbCrLf + String.Format("网址：{0}", Url), "无法打开网页", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
            End Try
        End Sub

        Public Sub OpenExplorer(Location As String)
            Try
                Location = Location.Replace("/", "\").Trim(New Char() { " "c, """"c })
                Logger.Info(String.Format("正在打开资源管理器：{0}", Location), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                If Location.EndsWithF("\", False) Then
                    ModBase.StartProcess(Location, "")
                Else
                    ModBase.StartProcess("explorer", String.Format("/select,""{0}""", Location))
                End If
            Catch ex As Exception
                Logger.[Error](ex, "打开资源管理器失败，请尝试关闭安全软件（如 360 安全卫士）", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
            End Try
        End Sub

        Public Sub ClipboardSet(Text As String, Optional ShowSuccessHint As Boolean = True)
            Dim $I2 As Action
            Dim $I1 As VB$AnonymousDelegate_10
            Dim $IR1 As Action(Of Integer)
            ModBase.RunInThread(Sub()
                Try
                    Dim action As Action(Of Integer)
                    If $IR1 IsNot Nothing Then
                        action = $IR1
                    Else
                        Dim action2 As Action(Of Integer) = Sub(a0 As Integer)
                            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_10
                            If $I1 IsNot Nothing Then
                                vb$AnonymousDelegate_ = $I1
                            Else
                                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_10 = Sub()
                                    Dim action4 As Action
                                    If $I2 IsNot Nothing Then
                                        action4 = $I2
                                    Else
                                        Dim action5 As Action = Sub()
                                            MyWpfExtension.Computer.Clipboard.Clear()
                                            If Not String.IsNullOrEmpty(Text) Then
                                                MyWpfExtension.Computer.Clipboard.SetText(Text)
                                            End If
                                        End Sub
                                        Dim action6 As Action = action5
                                        $I2 = action5
                                        action4 = action6
                                    End If
                                    ModBase.RunInUiWait(action4)
                                End Sub
                                Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_10 = vb$AnonymousDelegate_2
                                $I1 = vb$AnonymousDelegate_2
                                vb$AnonymousDelegate_ = vb$AnonymousDelegate_3
                            End If
                            vb$AnonymousDelegate_()
                        End Sub
                        Dim action3 As Action(Of Integer) = action2
                        $IR1 = action2
                        action = action3
                    End If
                    Dim num As UInteger? = New UInteger?(4UI)
                    Dim func As Func(Of Integer, TimeSpan)
                    If ModBase._Closure$__.$I68-3 IsNot Nothing Then
                        func = ModBase._Closure$__.$I68-3
                    Else
                        Dim func2 As Func(Of Integer, TimeSpan) = Function(Attempt As Integer) TimeSpan.FromMilliseconds(200.0)
                        func = func2
                        ModBase._Closure$__.$I68-3 = func2
                    End If
                    Dim func3 As Func(Of Exception, Boolean)
                    If ModBase._Closure$__.$I68-4 IsNot Nothing Then
                        func3 = ModBase._Closure$__.$I68-4
                    Else
                        Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) True
                        func3 = func4
                        ModBase._Closure$__.$I68-4 = func4
                    End If
                    Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
                    If ShowSuccessHint Then
                        ModMain.Hint("已成功复制！", ModMain.HintType.Green, True)
                    End If
                Catch ex As Exception
                    Dim ex2 As Exception
                    Logger.[Error](ex2, "可能由于剪贴板被其他程序占用，文本复制失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                End Try
            End Sub)
        End Sub

        Public Function ClipboardGetText() As String
            Dim Result As String = Nothing
            ModBase.RunInUiWait(Sub()
                Try
                    If MyWpfExtension.Computer.Clipboard.ContainsText() Then
                        Result = MyWpfExtension.Computer.Clipboard.GetText()
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, "获取剪贴板文本失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                End Try
            End Sub)
            Return Result
        End Function

        Public Sub ExtractResources(FilePath As String, ResourceName As String)
            Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject(ResourceName))
            If TypeOf objectValue Is Byte() Then
                Dim array As Byte() = CType(objectValue, Byte())
                If Not FileUtils.Exists(FilePath) OrElse Operators.CompareString(CryptographyUtils.ComputeFileHash(FilePath, CryptographyUtils.HashMethod.Md5), CryptographyUtils.ComputeHash(array, CryptographyUtils.HashMethod.Md5), False) <> 0 Then
                    Logger.Info(String.Format("将资源写入到文件：{0} → {1}", ResourceName, FilePath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                    FileUtils.Write(FilePath, array)
                    Return
                End If
            Else
                If Not(TypeOf objectValue Is String) Then
                    Throw New Exception(String.Format("资源 {0} 的类型不支持：{1}", ResourceName, objectValue.[GetType]().Name))
                End If
                Dim text As String = CStr(objectValue)
                If Not FileUtils.Exists(FilePath) OrElse Operators.CompareString(FileUtils.ReadAsString(FilePath, Nothing, Nothing), text, False) <> 0 Then
                    Logger.Info(String.Format("将资源写入到文件：{0} → {1}", ResourceName, FilePath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModBase.vb")
                    FileUtils.Write(FilePath, text, Nothing)
                    Return
                End If
            End If
        End Sub

        Public Function GetPixelSize(WPFSize As Double) As Double
            Return WPFSize / 96.0 * CDbl(ModBase.DPI)
        End Function

        Public Function GetWPFSize(PixelSize As Double) As Double
            Return PixelSize * 96.0 / CDbl(ModBase.DPI)
        End Function

        Public Function GetObjectFromXML(Str As XElement) As Object
            Return ModBase.GetObjectFromXML(Str.ToString())
        End Function

        Public Function GetObjectFromXML(Str As String) As Object
            Str = Str.Replace("EventType=""", "local:CustomEventService.EventType=""").Replace("EventData=""", "local:CustomEventService.EventData=""").Replace("EventType='", "local:CustomEventService.EventType='").Replace("EventData='", "local:CustomEventService.EventData='").Replace("Property=""EventType""", "Property=""local:CustomEventService.EventType""").Replace("Property=""EventData""", "Property=""local:CustomEventService.EventData""").Replace("Property='EventType'", "Property='local:CustomEventService.EventType'").Replace("Property='EventData'", "Property='local:CustomEventService.EventData'")
            Dim obj As Object
            Using memoryStream As MemoryStream = New MemoryStream(Encoding.UTF8.GetBytes(Str))
                Using xamlXmlReader As XamlXmlReader = New XamlXmlReader(memoryStream)
                    While xamlXmlReader.Read()
                        For Each type As Type In New Type() { GetType(WebBrowser), GetType(Frame), GetType(MediaElement), GetType(ObjectDataProvider), GetType(Global.System.Xaml.XamlReader), GetType(Window), GetType(XmlDataProvider), GetType(SettingService) }
                            If xamlXmlReader.Type IsNot Nothing AndAlso type.IsAssignableFrom(xamlXmlReader.Type.UnderlyingType) Then
                                Throw New UnauthorizedAccessException(String.Format("基于安全考虑，不允许使用 {0} 类型。", type.Name))
                            End If
                            If xamlXmlReader.Value IsNot Nothing AndAlso Operators.ConditionalCompareObjectEqual(xamlXmlReader.Value, type.Name, False) Then
                                Throw New UnauthorizedAccessException(String.Format("基于安全考虑，不允许使用 {0} 值。", type.Name))
                            End If
                        Next
                        For Each text As String In New String() { "Code", "FactoryMethod", "Static" }
                            If xamlXmlReader.Member IsNot Nothing AndAlso Operators.CompareString(xamlXmlReader.Member.Name, text, False) = 0 Then
                                Throw New UnauthorizedAccessException(String.Format("基于安全考虑，不允许使用 {0} 成员。", text))
                            End If
                        Next
                    End While
                End Using
                memoryStream.Position = 0L
                Using streamWriter As StreamWriter = New StreamWriter(memoryStream)
                    streamWriter.Write(Str)
                    streamWriter.Flush()
                    memoryStream.Position = 0L
                    obj = Global.System.Windows.Markup.XamlReader.Load(memoryStream)
                End Using
            End Using
            Return obj
        End Function

        Public Function RunInUi() As Boolean
            Return Thread.CurrentThread.ManagedThreadId = ModBase.UiThreadId
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IsVisibleInForm(element As FrameworkElement) As Boolean
            Dim flag As Boolean
            If Not element.IsVisible Then
                flag = False
            Else
                Dim rect As Rect = element.TransformToAncestor(ModMain.FrmMain).TransformBounds(New Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight))
                Dim rect2 As Rect = New Rect(0.0, 0.0, ModMain.FrmMain.ActualWidth, ModMain.FrmMain.ActualHeight)
                flag = rect2.Contains(rect.TopLeft) OrElse rect2.Contains(rect.BottomRight)
            End If
            Return flag
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IsTextTrimmed(TextBlock As TextBlock) As Boolean
            Dim typeface As Typeface = New Typeface(TextBlock.FontFamily, TextBlock.FontStyle, TextBlock.FontWeight, TextBlock.FontStretch)
            Return New FormattedText(TextBlock.Text, Thread.CurrentThread.CurrentCulture, TextBlock.FlowDirection, typeface, TextBlock.FontSize, TextBlock.Foreground, CDbl(ModBase.DPI)).Width > TextBlock.ActualWidth
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ToVisibility(IsVisible As Boolean) As Visibility
            If Not IsVisible Then
                Return Visibility.Collapsed
            End If
            Return Visibility.Visible
        End Function

        Public Function RandomOne(Of T)(objects As ICollection(Of T)) As T
            ' The following expression was wrapped in a checked-expression
            Return objects.ElementAtOrDefault(ModBase.RandomInteger(0, objects.Count - 1))
        End Function

        Public Function RandomInteger(min As Integer, max As Integer) As Integer
            ' The following expression was wrapped in a checked-statement
            Return CInt(Math.Round(Math.Floor(CDbl((max - min + 1)) * ModBase.Random.NextDouble()) + CDbl(min)))
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Shuffle(Of T)(Raw As IEnumerable(Of T)) As IEnumerable(Of T)
            Dim vb$StateMachine_84_Shuffle As ModBase.VB$StateMachine_84_Shuffle(Of T) = New ModBase.VB$StateMachine_84_Shuffle(Of T)(-2)
            vb$StateMachine_84_Shuffle.$P_Raw = Raw
            Return vb$StateMachine_84_Shuffle
        End Function

        Public Const VersionBaseName As String = "2.99.0.0"

        Public Const CommitHash As String = ""

        Public Const VersionCode As Integer = 414

        Public Const VersionDisplay As String = "快照版 2.99.0.0"

        Public Const BuildTypeDisplay As String = "快照版"

        Public Const BuildType As ModBase.BuildTypes = ModBase.BuildTypes.Snapshot

        Public Handle As IntPtr

        Public PathExe As String = Paths.Base + AppDomain.CurrentDomain.SetupInformation.ApplicationName

        Public PathImage As String = "pack://application:,,,/OpenPCL;component/Images/"

        Public PathTemp As String = PathUtils.AddSlashSuffix(If((Operators.CompareString(Settings.[Get](Of String)("SystemSystemCache", Nothing), "", False) = 0), (Path.GetTempPath() + "PCL\"), Settings.[Get](Of String)("SystemSystemCache", Nothing)).Replace("/", "\"))

        Public Lang As String = "zh_CN"

        Public ApplicationStartTick As Long = ModBase.GetTimeMs()

        Public ApplicationOpenTime As DateTime = DateTime.Now

        Public Identify As String = ModSecret.GetIdentify()

        Public IsProgramEnding As Boolean = False

        Public IsGBKEncoding As Boolean = Encoding.[Default].CodePage = 936

        Public OsDrive As String = Environment.GetLogicalDrives().First(Function(p As String) Directory.Exists(p))

        Private IniCache As ConcurrentDictionary(Of String, ConcurrentDictionary(Of String, String)) = New ConcurrentDictionary(Of String, ConcurrentDictionary(Of String, String))()

        Public vbLQ As Char = Convert.ToChar(8220)

        Public vbRQ As Char = Convert.ToChar(8221)

        Public PathPure As Lazy(Of String) = New Lazy(Of String)(Function()
            Dim text As String
            If Paths.Base.IsAsciiOnly() Then
                text = Paths.Base + "PCL\"
            ElseIf Paths.AppDataThenName.IsAsciiOnly() Then
                text = Paths.AppDataThenName
            ElseIf ModBase.PathTemp.IsAsciiOnly() Then
                text = ModBase.PathTemp
            Else
                text = ModBase.OsDrive + "ProgramData\PCL\"
            End If
            Return text
        End Function)

        Private Uuid As Integer = 1

        Private UuidLock As Object

        Public DPI As Integer = CInt(Math.Round(CDbl(Graphics.FromHwnd(IntPtr.Zero).DpiX)))

        Private UiThreadId As Integer = Thread.CurrentThread.ManagedThreadId

        Private Random As Random = New Random()

        Public Enum BuildTypes
            Debug = 100
            Release = 50
            Snapshot = 0
        End Enum

        Public Class Logo
            Public Const IconButtonLikeLine As String = "M512 896a42.666667 42.666667 0 0 1-30.293333-12.373333l-331.52-331.946667a224.426667 224.426667 0 0 1 0-315.733333 223.573333 223.573333 0 0 1 315.733333 0L512 282.026667l46.08-46.08a223.573333 223.573333 0 0 1 315.733333 0 224.426667 224.426667 0 0 1 0 315.733333l-331.52 331.946667A42.666667 42.666667 0 0 1 512 896zM308.053333 256a136.533333 136.533333 0 0 0-97.28 40.106667 138.24 138.24 0 0 0 0 194.986666L512 792.746667l301.226667-301.653334a138.24 138.24 0 0 0 0-194.986666 141.653333 141.653333 0 0 0-194.56 0l-76.373334 76.8a42.666667 42.666667 0 0 1-60.586666 0L405.333333 296.106667A136.533333 136.533333 0 0 0 308.053333 256z"

            Public Const IconButtonLikeFill As String = "M700.856 155.543c-74.769 0-144.295 72.696-190.046 127.26-45.737-54.576-115.247-127.26-190.056-127.26-134.79 0-244.443 105.78-244.443 235.799 0 77.57 39.278 131.988 70.845 175.713C238.908 694.053 469.62 852.094 479.39 858.757c9.41 6.414 20.424 9.629 31.401 9.629 11.006 0 21.998-3.215 31.398-9.63 9.782-6.662 240.514-164.703 332.238-291.701 31.587-43.724 70.874-98.143 70.874-175.713-0.001-130.02-109.656-235.8-244.445-235.8z m0 0"

            Public Const IconButtonDelete As String = "M520.192 0C408.43 0 317.44 82.87 313.563 186.734H52.736c-29.038 0-52.663 21.943-52.663 49.079s23.625 49.152 52.663 49.152h58.075v550.473c0 103.35 75.118 187.757 167.717 187.757h472.43c92.599 0 167.716-83.894 167.716-187.757V285.477h52.59c29.038 0 52.59-21.943 52.663-49.08-0.073-27.135-23.625-49.151-52.663-49.151H726.235C723.237 83.017 631.955 0 520.192 0zM404.846 177.957c3.803-50.03 50.176-89.015 107.447-89.015 57.197 0 103.57 38.985 106.788 89.015H404.92zM284.379 933.669c-33.353 0-69.997-39.351-69.997-95.525v-549.01H833.39v549.522c0 56.247-36.645 95.525-69.998 95.525H284.379v-0.512z M357.23 800.695a48.274 48.274 0 0 0 47.616-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.69 49.006V751.69c0 27.282 20.846 49.006 47.617 49.006z m166.62 0a48.274 48.274 0 0 0 47.688-49.006V471.7a48.274 48.274 0 0 0-47.689-49.08 48.274 48.274 0 0 0-47.543 49.006V751.69c0 27.282 21.431 49.006 47.543 49.006z m142.92 0a48.274 48.274 0 0 0 47.543-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.616 49.006V751.69c0 27.282 20.773 49.006 47.543 49.006z"

            Public Const IconButtonStop As String = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z"

            Public Const IconButtonCheck As String = "M512 0a512 512 0 1 0 512 512A512 512 0 0 0 512 0z m0 921.6a409.6 409.6 0 1 1 409.6-409.6 409.6 409.6 0 0 1-409.6 409.6z M716.8 339.968l-256 253.44L328.192 460.8A51.2 51.2 0 0 0 256 532.992l168.448 168.96a51.2 51.2 0 0 0 72.704 0l289.28-289.792A51.2 51.2 0 0 0 716.8 339.968z"

            Public Const IconButtonEdit As String = "M732.64 64.32C688.576 21.216 613.696 21.216 569.6 64.32L120.128 499.52c-17.6 12.896-26.432 30.144-30.848 51.68L32 870.048c0 25.856 8.8 56 26.432 73.248 17.632 17.216 17.632 48.704 88.64 48.704h13.248l326.08-56c22.016-4.32 39.68-12.928 52.864-30.176l449.472-435.2c22.048-21.536 35.264-47.36 35.264-77.536 0-30.176-13.216-56-35.264-77.568l-256.096-251.2zM139.712 903.776l56-326.912 311.04-295.136 267.104 269.44-310.976 295.168-323.168 57.44zM844.576 467.84l-273.984-260.672 61.856-59.84c8.832-8.512 26.528-8.512 39.776 0l234.24 226.496c4.384 4.288 8.832 12.8 8.832 17.088s-4.416 8.544-8.864 12.8l-61.856 64.128z"

            Public Const IconButtonSetup As String = "M651.946667 1001.813333c-22.186667 0-42.666667-10.24-61.44-27.306666-23.893333-23.893333-49.493333-35.84-75.093334-35.84-29.013333 0-56.32 11.946667-73.386666 30.72v3.413333c-17.066667 17.066667-42.666667 27.306667-66.56 27.306667h-6.826667c-6.826667 0-11.946667-1.706667-15.36-1.706667l-6.826667-1.706667c-64.853333-20.48-121.173333-54.613333-168.96-98.986666-29.013333-23.893333-37.546667-63.146667-25.6-95.573334 8.533333-23.893333 5.12-51.2-10.24-75.093333-15.36-27.306667-34.133333-40.96-59.733333-47.786667h-1.706667l-5.12-1.706666c-35.84-8.533333-61.44-34.133333-66.56-69.973334C1.706667 575.146667 0 537.6 0 512c0-32.426667 3.413333-63.146667 8.533333-93.866667v-6.826666l3.413334-8.533334c10.24-23.893333 23.893333-40.96 44.373333-51.2 5.12-3.413333 11.946667-6.826667 20.48-8.533333 27.306667-8.533333 51.2-25.6 63.146667-44.373333 13.653333-23.893333 17.066667-52.906667 10.24-81.92-11.946667-34.133333 0-71.68 30.72-93.866667 44.373333-37.546667 97.28-68.266667 158.72-93.866667l3.413333-1.706666c44.373333-13.653333 75.093333 3.413333 92.16 20.48 23.893333 23.893333 49.493333 35.84 75.093333 35.84 30.72 0 56.32-10.24 71.68-30.72l3.413334-3.413334c27.306667-27.306667 63.146667-35.84 93.866666-22.186666 63.146667 22.186667 117.76 54.613333 165.546667 97.28 29.013333 23.893333 37.546667 63.146667 25.6 95.573333-8.533333 23.893333-5.12 51.2 10.24 75.093333 15.36 27.306667 34.133333 40.96 59.733333 47.786667h1.706667l5.12 1.706667c35.84 8.533333 61.44 34.133333 66.56 71.68 6.826667 30.72 10.24 63.146667 11.946667 93.866666v3.413334c0 32.426667-3.413333 63.146667-8.533334 93.866666v6.826667l-3.413333 8.533333c-10.24 23.893333-23.893333 40.96-44.373333 51.2-5.12 3.413333-11.946667 6.826667-20.48 8.533334-27.306667 8.533333-51.2 25.6-63.146667 46.08-13.653333 23.893333-17.066667 52.906667-10.24 81.92 11.946667 35.84-1.706667 75.093333-30.72 95.573333-44.373333 35.84-95.573333 66.56-157.013333 92.16-15.36 3.413333-27.306667 3.413333-35.84 3.413333z m3.413333-83.626666z m1.706667 0zM517.12 853.333333c47.786667 0 93.866667 20.48 134.826667 59.733334 1.706667 1.706667 3.413333 1.706667 3.413333 3.413333 52.906667-22.186667 97.28-49.493333 136.533333-80.213333l1.706667-1.706667v-3.413333c-13.653333-52.906667-8.533333-104.106667 17.066667-148.48 23.893333-39.253333 64.853333-69.973333 114.346666-85.333334 1.706667 0 3.413333-1.706667 6.826667-6.826666 5.12-25.6 8.533333-51.2 8.533333-78.506667-1.706667-29.013333-3.413333-56.32-10.24-81.92v-5.12h-1.706666c-51.2-11.946667-90.453333-39.253333-119.466667-87.04-27.306667-44.373333-34.133333-100.693333-17.066667-148.48l-1.706666-1.706667h-3.413334c-39.253333-35.84-85.333333-63.146667-136.533333-80.213333H648.533333s-1.706667 1.706667-3.413333 1.706667c-32.426667 39.253333-80.213333 59.733333-136.533333 59.733333-47.786667 0-93.866667-20.48-134.826667-59.733333l-1.706667-1.706667h-1.706666c-54.613333 22.186667-98.986667 49.493333-136.533334 80.213333l-1.706666 1.706667v3.413333c13.653333 52.906667 8.533333 104.106667-17.066667 148.48-23.893333 39.253333-64.853333 69.973333-114.346667 85.333334-1.706667 0-3.413333 1.706667-6.826666 6.826666-6.826667 25.6-8.533333 51.2-8.533334 78.506667 0 30.72 3.413333 58.026667 6.826667 76.8l1.706667 5.12h1.706666c51.2 11.946667 90.453333 39.253333 119.466667 87.04 27.306667 44.373333 34.133333 100.693333 17.066667 148.48l1.706666 1.706667 1.706667 1.706666c37.546667 35.84 83.626667 63.146667 134.826667 80.213334 1.706667 0 3.413333 0 3.413333 1.706666h1.706667s1.706667 0 5.12-1.706666c34.133333-37.546667 81.92-59.733333 136.533333-59.733334z m-6.826667-146.773333c-110.933333 0-199.68-85.333333-199.68-196.266667 0-109.226667 87.04-196.266667 199.68-196.266666s199.68 85.333333 199.68 196.266666c-1.706667 109.226667-88.746667 196.266667-199.68 196.266667z m0-307.2c-63.146667 0-114.346667 49.493333-114.346666 110.933333 0 63.146667 49.493333 110.933333 114.346666 110.933334 30.72 0 59.733333-11.946667 80.213334-32.426667 20.48-20.48 32.426667-49.493333 32.426666-78.506667 0-63.146667-49.493333-110.933333-112.64-110.933333z"

            Public Const IconButtonReset As String = "M667.6817627 313.65283203l-45.28564454 55.76660156L858.06933594 391.27124023 787.61950684 165.93066406l-56.01379395 69.01611328A354.47387695 354.47387695 0 0 0 520.89892578 165.93066406C324.87536621 165.93066406 165.93066406 324.43041992 165.93066406 519.91015625c0 195.52917481 158.94470215 353.97949219 354.96826172 353.97949219a355.06713867 355.06713867 0 0 0 331.73217774-227.66418458 50.52612305 50.52612305 0 0 0-29.21813966-65.25878905 50.77331543 50.77331543 0 0 0-65.50598144 29.16870117A253.61938477 253.61938477 0 0 1 520.94836426 772.78796387c-140.05920411 0-253.61938477-113.21411133-253.61938477-252.87780762 0-139.61425781 113.56018067-252.82836914 253.61938477-252.82836914 53.59130859 0 104.46350098 16.61132813 146.73339843 46.57104492"

            Public Const IconButtonRefresh As String = "M512.0 838.3c-80.2 0-153.4-29.3-210.2-77.4l75.5-75.5c11.5-11.5 25.8-22.0 25.8-37.0a27.2 27.2 0 0 0-27.1-27.1H104.0c-27.1 0-27.1 23.9-27.1 27.1v271.9a27.1 27.1 0 0 0 27.1 27.1c15.0 0 27.8-16.6 42.5-31.2l77.9-77.9c76.6 67.7 177.1 108.9 287.4 108.9 221.7 0 404.5-166.0 431.2-380.6h-109.8c-25.9 154.2-159.7 271.9-321.3 271.9zM919.9 76.6c-15.0 0-27.8 16.6-42.5 31.3L799.5 185.8c-76.5-67.7-177.1-108.9-287.4-108.9-221.8 0-404.5 166.1-431.3 380.6H190.6c25.9-154.2 159.7-271.9 321.4-271.9 80.2 0 153.4 29.3 210.1 77.4l-75.5 75.5c-11.6 11.5-25.8 22.0-25.8 37.1a27.2 27.2 0 0 0 27.1 27.1h271.9c27.1 0 27.1-23.9 27.1-27.1V103.8a27.1 27.1 0 0 0-27.1-27.1z"

            Public Const IconButtonSave As String = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z"

            Public Const IconButtonInfo As String = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z"

            Public Const IconButtonList As String = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z"

            Public Const IconButtonOpen As String = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z"

            Public Const IconButtonArrowUp As String = "M554 333V853h-85V333l-228 228-60-60L512 170l331 331-60 60L554 333z"

            Public Const IconButtonArrowDown As String = "M554 691V171h-85V691L241 463l-60 60L512 854l331-331-60-60L554 691z"

            Public Const IconButtonCard As String = "M834.5 684.1c-31.2-70.4-98.9-120.9-179.1-127.3 63.5-8.5 112.6-63 112.6-128.8 0-71.8-58.2-130-130-130s-130 58.2-130 130c0 65.9 49 120.3 112.6 128.8-80.2 6.4-148 57-179.1 127.3-8.7 19.7 6 42 27.6 42 12.1 0 22.7-7.5 27.7-18.5 24.3-53.9 78.5-91.5 141.3-91.5s117 37.6 141.3 91.5c5 11.1 15.6 18.5 27.7 18.5 21.4 0 36.1-22.3 27.4-42zM567.9 427.9c0-38.6 31.4-70 70-70s70 31.4 70 70-31.4 70-70 70-70-31.4-70-70zM460.3 347.9H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h243.3c16.6 0 30-13.4 30-30 0.1-16.5-13.4-30-29.9-30zM367.4 459.6H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h150.4c16.6 0 30-13.4 30-30 0.1-16.6-13.4-30-29.9-30zM297.4 571.2H217c-16.6 0-30 13.4-30 30s13.4 30 30 30h80.4c16.6 0 30-13.4 30-30 0-16.5-13.5-30-30-30zM900 236v552H124V236h776m0-60H124c-33.1 0-60 26.9-60 60v552c0 33.1 26.9 60 60 60h776c33.1 0 60-26.9 60-60V236c0-33.1-26.9-60-60-60z"

            Public Const IconButtonCross As String = "F1 M 26.9166,22.1667L 37.9999,33.25L 49.0832,22.1668L 53.8332,26.9168L 42.7499,38L 53.8332,49.0834L 49.0833,53.8334L 37.9999,42.75L 26.9166,53.8334L 22.1666,49.0833L 33.25,38L 22.1667,26.9167L 26.9166,22.1667 Z"

            Public Const IconButtonMojang As String = "M9.183,18.967c-0.109-2.239,1.336-5.119,3.92-4.96c3.657-0.027,7.319-0.044,10.977,0.005" & vbTab & "c3.712,0.214,6.596,2.759,9.652,4.533c0.181-1.697-0.197-3.717,1.237-4.982c1.899,2.091,3.143,4.894,5.677,6.334" & vbTab & "c1.577,0.805,2.973-0.668,4.221-1.44c1.55,2.305,2.108,5.075,2.622,7.752c0.657,3.438,0.947,6.925,1.111,10.413" & vbTab & "c-1.734-0.733-3.355-1.708-4.971-2.671c-1.396-4.933-5.349-8.656-9.723-11.059c-3.46-1.817-7.752-2.185-11.338-0.52" & vbTab & "c-3.761,1.593-6.285,5.666-5.891,9.75c0.121,4.577,3.17,8.765,7.391,10.456c7.746,3.285,16.407,2.234,24.521,1.243" & vbTab & "c0.454,2.315-1.511,4.527-3.695,4.889c-10.577,0.038-21.148-0.011-31.725,0.022c-2.217,0.279-4.079-1.879-3.964-4.008" & vbTab & "C9.167,36.141,9.216,27.556,9.183,18.967z M40.114,8.09c0.294-0.801,0.872-1.417,1.542-1.924c1.379,2.589,2.742,5.482,2.311,8.491" & vbTab & "c-0.332,2.54-4.42,2.507-5.052,0.163C38.349,12.527,39.209,10.178,40.114,8.09z M0,53 l0.1,0 z"

            Public Const IconButtonUser As String = "M660.338 528.065c63.61-46.825 105.131-121.964 105.131-206.83 0-141.7-115.29-256.987-256.997-256.987-141.706 0-256.998 115.288-256.998 256.987 0 85.901 42.52 161.887 107.456 208.562-152.1 59.92-260.185 207.961-260.185 381.077 0 21.276 17.253 38.53 38.53 38.53 21.278 0 38.53-17.254 38.53-38.53 0-183.426 149.232-332.671 332.667-332.671 1.589 0 3.113-0.207 4.694-0.244 0.8 0.056 1.553 0.244 2.362 0.244 183.434 0 332.664 149.245 332.664 332.671 0 21.276 17.255 38.53 38.533 38.53 21.277 0 38.53-17.254 38.53-38.53 0-174.885-110.354-324.13-264.917-382.809z m-331.803-206.83c0-99.22 80.72-179.927 179.935-179.927s179.937 80.708 179.937 179.927c0 99.203-80.721 179.91-179.937 179.91s-179.935-80.708-179.935-179.91z"

            Public Const IconButtonShield As String = "M511.488256 95.184408c35.310345 22.516742 95.184408 55.78011 167.34033 84.437781 75.738131 29.681159 148.405797 40.93953 191.392304 45.033483v353.615193c0 73.691154-50.662669 164.781609-136.123938 244.101949C649.65917 901.181409 558.568716 942.12094 512 942.12094c-46.568716 0-137.65917-40.93953-222.096952-119.748126C204.441779 742.54073 153.77911 651.450275 153.77911 577.247376v-353.103448c42.474763-4.093953 116.165917-15.352324 191.904048-45.545227 75.226387-30.192904 133.565217-63.456272 165.805098-83.414293M512 0c-4.093953 0-8.187906 1.535232-11.258371 3.582209l-14.84058 10.234882c-1.023488 0.511744-67.550225 47.592204-170.410794 88.531735-100.813593 39.916042-198.556722 41.963018-199.58021 41.963018l-25.075462 0.511744c-10.746627 0.511744-18.934533 8.187906-18.934533 18.422789v414.000999c0 216.97951 286.064968 446.24088 440.09995 446.24088s440.09995-229.261369 440.09995-445.729136V163.758121c0-10.234883-8.69965-18.422789-18.934533-18.422789l-24.563718-0.511744c-1.023488 0-98.766617-2.046977-199.58021-41.963018-103.372314-40.93953-170.410795-88.01999-170.922538-88.531734L523.258371 3.582209c-3.070465-2.558721-7.164418-3.582209-11.258371-3.582209z M743.308346 410.930535l-260.477761 260.477761c-15.864068 15.864068-41.963018 15.864068-57.827087 0l-144.823588-144.823588c-15.864068-15.864068-15.864068-41.963018 0-57.827087 8.187906-8.187906 18.422789-11.770115 29.169415-11.770115 10.234883 0 20.981509 4.093953 29.169416 11.770115l115.654173 115.654173L685.993003 352.591704c15.864068-15.864068 41.963018-15.864068 57.827087 0 15.352324 16.375812 15.352324 42.474763-0.511744 58.338831z"

            Public Const IconButtonOffline As String = "M533.293176 788.841412a60.235294 60.235294 0 1 1 85.202824 85.202823l-42.616471 42.586353c-129.355294 129.385412-339.124706 129.385412-468.510117 0-129.385412-129.385412-129.385412-339.124706 0-468.510117l42.586353-42.616471a60.235294 60.235294 0 1 1 85.202823 85.202824l-42.61647 42.586352a210.823529 210.823529 0 1 0 298.164706 298.164706l42.586352-42.61647z m255.548236-255.548236l42.61647-42.586352a210.823529 210.823529 0 1 0-298.164706-298.164706l-42.586352 42.61647a60.235294 60.235294 0 1 1-85.202824-85.202823l42.616471-42.586353c129.355294-129.385412 339.124706-129.385412 468.510117 0 129.385412 129.385412 129.385412 339.124706 0 468.510117l-42.586353 42.616471a60.235294 60.235294 0 1 1-85.202823-85.202824zM192.542118 192.542118a60.235294 60.235294 0 0 1 85.202823 0l553.712941 553.712941a60.235294 60.235294 0 0 1-85.202823 85.202823L192.542118 277.744941a60.235294 60.235294 0 0 1 0-85.202823z"

            Public Const IconButtonServer As String = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z"

            Public Const IconMusic As String = "M348.293565 716.53287V254.797913c0-41.672348 28.004174-78.358261 68.919652-90.37913L815.994435 40.826435c62.775652-18.610087 125.907478 26.579478 125.907478 89.933913v539.158261c8.013913 42.25113-8.94887 89.177043-47.014956 127.109565a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c21.014261 1.024 40.514783 5.164522 57.878261 12.065391V233.338435c0-12.109913-10.551652-20.034783-20.569044-20.034783a24.620522 24.620522 0 0 0-5.787826 0.934957L439.785739 338.18713a19.545043 19.545043 0 0 0-14.825739 19.144348v438.984348H423.846957c11.53113 43.987478-5.164522 94.208-45.412174 134.322087a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c20.791652 1.024 40.069565 5.075478 57.299478 11.842783z"

            Public Const IconPlay As String = "M803.904 463.936a55.168 55.168 0 0 1 0 96.128l-463.616 264.448C302.848 845.888 256 819.136 256 776.448V247.616c0-42.752 46.848-69.44 84.288-48.064l463.616 264.384z"
        End Class

        Public Class OurColor
            Public Shared Widening Operator CType(str As String) As ModBase.OurColor
                Return New ModBase.OurColor(str)
            End Operator

            Public Shared Widening Operator CType(col As Global.System.Windows.Media.Color) As ModBase.OurColor
                Return New ModBase.OurColor(col)
            End Operator

            Public Shared Widening Operator CType(conv As ModBase.OurColor) As Global.System.Windows.Media.Color
                Return Global.System.Windows.Media.Color.FromArgb(ModBase.OurColor.ClampToByte(conv.A), ModBase.OurColor.ClampToByte(conv.R), ModBase.OurColor.ClampToByte(conv.G), ModBase.OurColor.ClampToByte(conv.B))
            End Operator

            Public Shared Widening Operator CType(conv As ModBase.OurColor) As Global.System.Drawing.Color
                Return Global.System.Drawing.Color.FromArgb(CInt(ModBase.OurColor.ClampToByte(conv.A)), CInt(ModBase.OurColor.ClampToByte(conv.R)), CInt(ModBase.OurColor.ClampToByte(conv.G)), CInt(ModBase.OurColor.ClampToByte(conv.B)))
            End Operator

            Public Shared Widening Operator CType(bru As SolidColorBrush) As ModBase.OurColor
                Return New ModBase.OurColor(bru.Color)
            End Operator

            Public Shared Widening Operator CType(conv As ModBase.OurColor) As SolidColorBrush
                Return New SolidColorBrush(Global.System.Windows.Media.Color.FromArgb(ModBase.OurColor.ClampToByte(conv.A), ModBase.OurColor.ClampToByte(conv.R), ModBase.OurColor.ClampToByte(conv.G), ModBase.OurColor.ClampToByte(conv.B)))
            End Operator

            Public Shared Widening Operator CType(bru As Global.System.Windows.Media.Brush) As ModBase.OurColor
                Return New ModBase.OurColor(bru)
            End Operator

            Public Shared Widening Operator CType(conv As ModBase.OurColor) As Global.System.Windows.Media.Brush
                Return New SolidColorBrush(Global.System.Windows.Media.Color.FromArgb(ModBase.OurColor.ClampToByte(conv.A), ModBase.OurColor.ClampToByte(conv.R), ModBase.OurColor.ClampToByte(conv.G), ModBase.OurColor.ClampToByte(conv.B)))
            End Operator

            Public Shared Operator +(a As ModBase.OurColor, b As ModBase.OurColor) As ModBase.OurColor
                Return New ModBase.OurColor() With { .A = a.A + b.A, .B = a.B + b.B, .G = a.G + b.G, .R = a.R + b.R }
            End Operator

            Public Shared Operator -(a As ModBase.OurColor, b As ModBase.OurColor) As ModBase.OurColor
                Return New ModBase.OurColor() With { .A = a.A - b.A, .B = a.B - b.B, .G = a.G - b.G, .R = a.R - b.R }
            End Operator

            Public Shared Operator *(a As ModBase.OurColor, b As Double) As ModBase.OurColor
                Return New ModBase.OurColor() With { .A = a.A * b, .B = a.B * b, .G = a.G * b, .R = a.R * b }
            End Operator

            Public Shared Operator /(a As ModBase.OurColor, b As Double) As ModBase.OurColor
                Return New ModBase.OurColor() With { .A = a.A / b, .B = a.B / b, .G = a.G / b, .R = a.R / b }
            End Operator

            Public Shared Operator =(a As ModBase.OurColor, b As ModBase.OurColor) As Boolean
                Return(Information.IsNothing(a) AndAlso Information.IsNothing(b)) OrElse (Not Information.IsNothing(a) AndAlso Not Information.IsNothing(b) AndAlso (a.A = b.A AndAlso a.R = b.R AndAlso a.G = b.G) AndAlso a.B = b.B)
            End Operator

            Public Shared Operator <>(a As ModBase.OurColor, b As ModBase.OurColor) As Boolean
                Return(Not Information.IsNothing(a) OrElse Not Information.IsNothing(b)) AndAlso (Information.IsNothing(a) OrElse Information.IsNothing(b) OrElse a.A <> b.A OrElse a.R <> b.R OrElse a.G <> b.G OrElse a.B <> b.B)
            End Operator

            Public Shared Function Lerp(ValueA As ModBase.OurColor, ValueB As ModBase.OurColor, Percent As Double) As ModBase.OurColor
                Return ModBase.OurColor.Round(ValueA * (1.0 - Percent) + ValueB * Percent, 6)
            End Function

            Public Shared Function Round(col As ModBase.OurColor, Optional w As Integer = 0) As ModBase.OurColor
                Return New ModBase.OurColor() With { .A = Math.Round(col.A, w), .R = Math.Round(col.R, w), .G = Math.Round(col.G, w), .B = Math.Round(col.B, w) }
            End Function

            Public Shared Function ClampToByte(d As Double) As Byte
                If d < 0.0 Then
                    d = 0.0
                End If
                If d > 255.0 Then
                    d = 255.0
                End If
                Return CByte(Math.Round(d))
            End Function

            Public Sub New()
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
            End Sub

            Public Sub New(col As Global.System.Windows.Media.Color)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                Me.A = CDbl(col.A)
                Me.R = CDbl(col.R)
                Me.G = CDbl(col.G)
                Me.B = CDbl(col.B)
            End Sub

            Public Sub New(HexString As String)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                Dim obj As Object = Global.System.Windows.Media.ColorConverter.ConvertFromString(HexString)
                Dim color As Global.System.Windows.Media.Color = If((obj IsNot Nothing), CType(obj, Global.System.Windows.Media.Color), Nothing)
                Me.A = CDbl(color.A)
                Me.R = CDbl(color.R)
                Me.G = CDbl(color.G)
                Me.B = CDbl(color.B)
            End Sub

            Public Sub New(newA As Double, col As ModBase.OurColor)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                Me.A = newA
                Me.R = col.R
                Me.G = col.G
                Me.B = col.B
            End Sub

            Public Sub New(newR As Double, newG As Double, newB As Double)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                Me.A = 255.0
                Me.R = newR
                Me.G = newG
                Me.B = newB
            End Sub

            Public Sub New(newA As Double, newR As Double, newG As Double, newB As Double)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                Me.A = newA
                Me.R = newR
                Me.G = newG
                Me.B = newB
            End Sub

            Public Sub New(brush As Global.System.Windows.Media.Brush)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                Dim color As Global.System.Windows.Media.Color = CType(brush, SolidColorBrush).Color
                Me.A = CDbl(color.A)
                Me.R = CDbl(color.R)
                Me.G = CDbl(color.G)
                Me.B = CDbl(color.B)
            End Sub

            Public Sub New(brush As SolidColorBrush)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                Dim color As Global.System.Windows.Media.Color = brush.Color
                Me.A = CDbl(color.A)
                Me.R = CDbl(color.R)
                Me.G = CDbl(color.G)
                Me.B = CDbl(color.B)
            End Sub

            Public Sub New(obj As Object)
                Me.A = 255.0
                Me.R = 0.0
                Me.G = 0.0
                Me.B = 0.0
                If obj Is Nothing Then
                    Me.A = 255.0
                    Me.R = 255.0
                    Me.G = 255.0
                    Me.B = 255.0
                    Return
                End If
                If TypeOf obj Is SolidColorBrush Then
                    Dim color As Global.System.Windows.Media.Color = CType(obj, SolidColorBrush).Color
                    Me.A = CDbl(color.A)
                    Me.R = CDbl(color.R)
                    Me.G = CDbl(color.G)
                    Me.B = CDbl(color.B)
                    Return
                End If
                Me.A = Conversions.ToDouble(NewLateBinding.LateGet(obj, Nothing, "A", New Object(-1) {}, Nothing, Nothing, Nothing))
                Me.R = Conversions.ToDouble(NewLateBinding.LateGet(obj, Nothing, "R", New Object(-1) {}, Nothing, Nothing, Nothing))
                Me.G = Conversions.ToDouble(NewLateBinding.LateGet(obj, Nothing, "G", New Object(-1) {}, Nothing, Nothing, Nothing))
                Me.B = Conversions.ToDouble(NewLateBinding.LateGet(obj, Nothing, "B", New Object(-1) {}, Nothing, Nothing, Nothing))
            End Sub

            Public Function Hue(v1 As Double, v2 As Double, vH As Double) As Double
                If vH < 0.0 Then
                    vH += 1.0
                End If
                If vH > 1.0 Then
                    vH -= 1.0
                End If
                Dim num As Double
                If vH < 0.16667 Then
                    num = v1 + (v2 - v1) * 6.0 * vH
                ElseIf vH < 0.5 Then
                    num = v2
                ElseIf vH < 0.66667 Then
                    num = v1 + (v2 - v1) * (4.0 - vH * 6.0)
                Else
                    num = v1
                End If
                Return num
            End Function

            Public Function FromHSL(sH As Double, sS As Double, sL As Double) As ModBase.OurColor
                Me.A = 255.0
                Dim myColor As ModBase.OurColor
                If sS = 0.0 Then
                    Me.R = sL * 2.55
                    Me.G = Me.R
                    Me.B = Me.R
                    myColor = Me
                Else
                    Dim num As Double = sH / 360.0
                    Dim num2 As Double = sS / 100.0
                    Dim num3 As Double = sL / 100.0
                    num2 = If((num3 < 0.5), (num2 * num3 + num3), (num2 * (1.0 - num3) + num3))
                    num3 = 2.0 * num3 - num2
                    Me.R = 255.0 * Me.Hue(num3, num2, num + 0.3333333333333333)
                    Me.G = 255.0 * Me.Hue(num3, num2, num)
                    Me.B = 255.0 * Me.Hue(num3, num2, num - 0.3333333333333333)
                    myColor = Me
                End If
                Return myColor
            End Function

            Public Function FromHSL2(sH As Double, sS As Double, sL As Double) As ModBase.OurColor
                Me.A = 255.0
                Dim myColor As ModBase.OurColor
                If sS = 0.0 Then
                    Me.R = sL * 2.55
                    Me.G = Me.R
                    Me.B = Me.R
                    myColor = Me
                Else
                    sH = (sH + 3600000.0) Mod 360.0
                    Dim array As Double() = New Double() { 0.1, -0.06, -0.3, -0.19, -0.15, -0.24, -0.32, -0.09, 0.18, 0.05, -0.12, -0.02, 0.1, -0.06 }
                    Dim num As Double = sH / 30.0
                    Dim num2 As Integer = CInt(Math.Floor(num))
                    num = 50.0 - ((1.0 - num + CDbl(num2)) * array(num2) + (num - CDbl(num2)) * array(num2 + 1)) * sS
                    sL = If((sL < num), (sL / num), (1.0 + (sL - num) / (100.0 - num))) * 50.0
                    Me.FromHSL(sH, sS, sL)
                    myColor = Me
                End If
                Return myColor
            End Function

            Public Overrides Function ToString() As String
                Return String.Concat(New String() { "(", Conversions.ToString(Me.A), ",", Conversions.ToString(Me.R), ",", Conversions.ToString(Me.G), ",", Conversions.ToString(Me.B), ")" })
            End Function

            Public Overrides Function Equals(obj As Object) As Boolean
                Return Me Is CType(obj, ModBase.OurColor)
            End Function

            Public A As Double

            Public R As Double

            Public G As Double

            Public B As Double
        End Class

        Public Class OurRect
            Public Property Width As Double

            Public Property Height As Double

            Public Property Left As Double

            Public Property Top As Double

            Public Sub New()
                Me.Width = 0.0
                Me.Height = 0.0
                Me.Left = 0.0
                Me.Top = 0.0
            End Sub

            Public Sub New(left As Double, top As Double, width As Double, height As Double)
                Me.Width = 0.0
                Me.Height = 0.0
                Me.Left = 0.0
                Me.Top = 0.0
                Me.Left = left
                Me.Top = top
                Me.Width = width
                Me.Height = height
            End Sub
        End Class

        Public Enum LoadState
            Waiting
            Loading
            Finished
            Failed
            Canceled
        End Enum

        Public Enum ProcessReturnValues
            Success
            Fail
            Exception
            Timeout
            Cancel
            TaskDone
        End Enum

        Public Class SearchEntry(Of T)
            Public Overrides Function ToString() As String
                Return Conversions.ToString(Math.Round(Me.Similarity, 3)) + " - " + Me.Item.ToString()
            End Function

            Public Item As T

            Public SearchSource As List(Of ModBase.SearchSource)

            Public Similarity As Double

            Public AbsoluteRight As Boolean
        End Class

        Public Class SearchSource
            Public Sub New(Aliases As String(), Optional Weight As Double = 1.0)
                Me.Aliases = Aliases
                Me.Weight = Weight
            End Sub

            Public Sub New(Text As String, Optional Weight As Double = 1.0)
                Me.Aliases = New String() { Text }
                Me.Weight = Weight
            End Sub

            Public Aliases As String()

            Public Weight As Double
        End Class

        Public Class RestartException
            Inherits Exception

        End Class

        Public NotInheritable Class RouteEventArgs
            Inherits EventArgs

            Public Sub New(Optional RaiseByMouse As Boolean = False)
                Me.Handled = False
                Me.RaiseByMouse = RaiseByMouse
            End Sub

            Public RaiseByMouse As Boolean

            Public Handled As Boolean
        End Class
    End Module
End Namespace
