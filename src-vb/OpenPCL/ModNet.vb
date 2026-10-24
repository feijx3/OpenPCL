Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.NetworkInformation
Imports System.Net.Security
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports CacheCow.Client
Imports CacheCow.Client.FileCacheStore
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports ThrottleDebounce

Namespace PCL
    Public NotInheritable Module ModNet
        Public Function NetRequestByClient(Url As String, Optional Method As HttpMethod = Nothing, Optional Content As Object = Nothing, Optional ContentType As String = Nothing, Optional Accept As String = "*/*", Optional Timeout As Integer = 25000, Optional Headers As String(,) = Nothing, Optional RequireJson As Boolean = False, Optional Encoding As Encoding = Nothing, Optional SimulateBrowserHeaders As Boolean = False, Optional MakeLog As Boolean = True) As String
            If Method Is Nothing Then
                Method = HttpMethod.[Get]
            End If
            If MakeLog Then
                Logger.Info(String.Format("发起网络请求（{0}，{1}），最大超时 {2}", Method.Method, Url, Timeout), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
            End If
            Dim text2 As String
            Try
                Dim dictionary As Dictionary(Of String, String) = (If(Headers, New String(-1, -1) {})).ToDictionary()
                If ContentType IsNot Nothing Then
                    dictionary("Content-Type") = ContentType
                End If
                If Accept IsNot Nothing Then
                    dictionary("Accept") = Accept
                End If
                Dim text As String = ModNet.SendRequest(Url, Method, RuntimeHelpers.GetObjectValue(Content), dictionary, SimulateBrowserHeaders, Timeout, Encoding).GetString(Encoding)
                If RequireJson Then
                    text = text.Trim(vbCrLf & " ".ToCharArray())
                    If(Not text.StartsWithF("{", False) OrElse Not text.EndsWithF("}", False)) AndAlso (Not text.StartsWithF("[", False) OrElse Not text.EndsWithF("]", False)) Then
                        ' The following expression was wrapped in a checked-expression
                        Throw New FormatException("返回结果并非 JSON 格式：" + If((text.Length > 2000), (text.Substring(0, 1000) + "..." + text.Substring(text.Length - 1000)), text))
                    End If
                End If
                text2 = text
            Catch ex As FormatException
                Throw
            Catch ex2 As Exception
                If MakeLog Then
                    Logger.Warn(ex2, String.Format("网络请求失败（{0}，{1}）", Method.Method, Url), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                End If
                Throw
            End Try
            Return text2
        End Function

        Public Function NetRequestByClientRetry(Url As String, Optional Method As HttpMethod = Nothing, Optional BackupUrl As String = Nothing, Optional Content As Object = Nothing, Optional ContentType As String = Nothing, Optional Accept As String = "*/*", Optional Headers As String(,) = Nothing, Optional RequireJson As Boolean = False, Optional Encoding As Encoding = Nothing, Optional SimulateBrowserHeaders As Boolean = False) As String
            Dim num As Integer = 0
            Dim ex As Exception = Nothing
            Dim timeMs As Long = ModBase.GetTimeMs()
            Dim text As String
            Try
                IL_000A:
                If num <> 0 Then
                    If num <> 1 Then
                        If ModBase.GetTimeMs() - timeMs <= 5500L Then
                            Throw ex
                        End If
                        Thread.Sleep(500)
                        text = ModNet.NetRequestByClient(Url, Method, RuntimeHelpers.GetObjectValue(Content), ContentType, Accept, 4000, Headers, RequireJson, Encoding, SimulateBrowserHeaders, True)
                    Else
                        Thread.Sleep(500)
                        text = ModNet.NetRequestByClient(Url, Method, RuntimeHelpers.GetObjectValue(Content), ContentType, Accept, 30000, Headers, RequireJson, Encoding, SimulateBrowserHeaders, True)
                    End If
                Else
                    text = ModNet.NetRequestByClient(Url, Method, RuntimeHelpers.GetObjectValue(Content), ContentType, Accept, 10000, Headers, RequireJson, Encoding, SimulateBrowserHeaders, True)
                End If
            Catch ex2 As Exception
                ex2.ThrowIfCanceled()
                If TypeOf ex2 Is HttpRequestCodeException Then
                    If CType(ex2, HttpRequestCodeException).StatusCode = HttpStatusCode.Forbidden Then
                        Throw
                    End If
                    If CType(ex2, HttpRequestCodeException).StatusCode = HttpStatusCode.NotFound Then
                        Throw
                    End If
                    If CType(ex2, HttpRequestCodeException).StatusCode = CType(429, HttpStatusCode) Then
                        Thread.Sleep(10000)
                    End If
                End If
                If num = 0 Then
                    ex = ex2
                    num += 1
                    GoTo IL_000A
                End If
                If num <> 1 Then
                    Throw
                End If
                num += 1
                GoTo IL_000A
            End Try
            Return text
        End Function

        Public Function NetRequestByClientMultiple(Url As String, Optional Method As HttpMethod = Nothing, Optional ThreadCount As Integer = 3, Optional Content As Object = Nothing, Optional ContentType As String = Nothing, Optional Accept As String = "*/*", Optional Headers As String(,) = Nothing, Optional RequireJson As Boolean = False, Optional Timeout As Integer = 25000, Optional Encoding As Encoding = Nothing, Optional SimulateBrowserHeaders As Boolean = False) As String
            Dim CS$<>8__locals1 As ModNet._Closure$__3-0 = New ModNet._Closure$__3-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Url = Url
            CS$<>8__locals1.$VB$Local_Method = Method
            CS$<>8__locals1.$VB$Local_Content = Content
            CS$<>8__locals1.$VB$Local_ContentType = ContentType
            CS$<>8__locals1.$VB$Local_Accept = Accept
            CS$<>8__locals1.$VB$Local_Headers = Headers
            CS$<>8__locals1.$VB$Local_RequireJson = RequireJson
            CS$<>8__locals1.$VB$Local_Timeout = Timeout
            CS$<>8__locals1.$VB$Local_Encoding = Encoding
            CS$<>8__locals1.$VB$Local_SimulateBrowserHeaders = SimulateBrowserHeaders
            Dim list As List(Of Thread) = New List(Of Thread)()
            CS$<>8__locals1.$VB$Local_RequestResult = Nothing
            CS$<>8__locals1.$VB$Local_RequestEx = Nothing
            CS$<>8__locals1.$VB$Local_FailCount = 0
            For i As Integer = 1 To ThreadCount
                Dim threadStart As ThreadStart
                If CS$<>8__locals1.$I0 IsNot Nothing Then
                    threadStart = CS$<>8__locals1.$I0
                Else
                    Dim CS$<>8__locals2 As ModNet._Closure$__3-0 = CS$<>8__locals1
                    Dim threadStart2 As ThreadStart = Sub()
                        Try
                            CS$<>8__locals1.$VB$Local_RequestResult = ModNet.NetRequestByClient(CS$<>8__locals1.$VB$Local_Url, CS$<>8__locals1.$VB$Local_Method, RuntimeHelpers.GetObjectValue(CS$<>8__locals1.$VB$Local_Content), CS$<>8__locals1.$VB$Local_ContentType, CS$<>8__locals1.$VB$Local_Accept, CS$<>8__locals1.$VB$Local_Timeout, CS$<>8__locals1.$VB$Local_Headers, CS$<>8__locals1.$VB$Local_RequireJson, CS$<>8__locals1.$VB$Local_Encoding, CS$<>8__locals1.$VB$Local_SimulateBrowserHeaders, True)
                        Catch ex As Exception
                            CS$<>8__locals1.$VB$Local_FailCount += 1
                            CS$<>8__locals1.$VB$Local_RequestEx = ex
                        End Try
                    End Sub
                    Dim threadStart3 As ThreadStart = threadStart2
                    CS$<>8__locals2.$I0 = threadStart2
                    threadStart = threadStart3
                End If
                Dim thread As Thread = New Thread(threadStart)
                thread.Start()
                list.Add(thread)
                Thread.Sleep(i * 250)
                If CS$<>8__locals1.$VB$Local_RequestResult IsNot Nothing Then
                    IL_00DB:
                    Try
                        Try
                            For Each thread2 As Thread In list
                                If thread2.IsAlive Then
                                    thread2.Interrupt()
                                End If
                            Next
                        Finally
                            Dim enumerator As List(Of Thread).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                    Catch ex As Exception
                    End Try
                    Return Conversions.ToString(CS$<>8__locals1.$VB$Local_RequestResult)
                End If
            Next
            While CS$<>8__locals1.$VB$Local_RequestResult Is Nothing
                If CS$<>8__locals1.$VB$Local_FailCount = ThreadCount Then
                    Try
                        Try
                            For Each thread3 As Thread In list
                                If thread3.IsAlive Then
                                    thread3.Interrupt()
                                End If
                            Next
                        Finally
                            Dim enumerator2 As List(Of Thread).Enumerator
                            CType(enumerator2, IDisposable).Dispose()
                        End Try
                    Catch ex2 As Exception
                    End Try
                    Throw CS$<>8__locals1.$VB$Local_RequestEx
                End If
                Thread.Sleep(20)
            End While
            GoTo IL_00DB
        End Function

        Public Function NetRequestByLoader(Url As String, Optional Timeout As Integer = 45000, Optional IsJson As Boolean = False, Optional SimulateBrowserHeaders As Boolean = False) As String
            Return ModNet.NetRequestByLoader(New String() { Url }, Timeout, IsJson, SimulateBrowserHeaders)
        End Function

        Public Function NetRequestByLoader(Urls As IEnumerable(Of String), Optional Timeout As Integer = 45000, Optional IsJson As Boolean = False, Optional SimulateBrowserHeaders As Boolean = False) As String
            Dim text As String = ModMain.RequestTaskTempFolder(False) + "download.txt"
            Dim loaderDownload As ModNet.LoaderDownload = New ModNet.LoaderDownload("源码获取 " + Conversions.ToString(ModBase.GetUuid()) + "#", New List(Of ModNet.NetFile)() From { New ModNet.NetFile(Urls, text, New FileChecker() With { .IsJson = IsJson }, SimulateBrowserHeaders) })
            Dim text3 As String
            Try
                loaderDownload.WaitForExitTime(Timeout, Nothing, "连接服务器超时（第一下载源：" + Urls.First() + "）", Nothing, False)
                Dim text2 As String = FileUtils.ReadAsString(text, Nothing, Nothing)
                FileUtils.Delete(text, False)
                text3 = text2
            Finally
                loaderDownload.Cancel()
            End Try
            Return text3
        End Function

        Public Sub NetDownloadByClient(Url As String, LocalPath As String, Optional SimulateBrowserHeaders As Boolean = False)
            Logger.Info(String.Format("通过网络请求直接下载小文件：{0} → {1}", Url, LocalPath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
            Dim num As Integer = 0
            While True
                Try
                    FileUtils.Write(LocalPath, ModNet.SendRequest(Url, HttpMethod.[Get], Nothing, Nothing, SimulateBrowserHeaders, 25000, Nothing))
                Catch ex As Exception
                    FileUtils.Delete(LocalPath, False)
                    ex.ThrowIfCanceled()
                    If num < 2 Then
                        num += 1
                        Continue For
                    End If
                    Throw New HttpRequestException(String.Format("通过网络请求直接下载小文件失败（{0}）", LocalPath), ex)
                End Try
                Exit For
            End While
        End Sub

        Public Sub NetDownloadByLoader(Url As String, LocalPath As String, Optional LoaderToSyncProgress As ModLoader.LoaderBase = Nothing, Optional Check As FileChecker = Nothing, Optional SimulateBrowserHeaders As Boolean = False)
            ModNet.NetDownloadByLoader(New String() { Url }, LocalPath, LoaderToSyncProgress, Check, SimulateBrowserHeaders)
        End Sub

        Public Sub NetDownloadByLoader(Urls As IEnumerable(Of String), LocalPath As String, Optional LoaderToSyncProgress As ModLoader.LoaderBase = Nothing, Optional Check As FileChecker = Nothing, Optional SimulateBrowserHeaders As Boolean = False)
            Dim loaderDownload As ModNet.LoaderDownload = New ModNet.LoaderDownload(String.Format("文件下载 {0}#", ModBase.GetUuid()), New List(Of ModNet.NetFile)() From { New ModNet.NetFile(Urls, LocalPath, Check, SimulateBrowserHeaders) })
            Try
                loaderDownload.WaitForExit(Nothing, LoaderToSyncProgress, False, Nothing, Nothing)
            Catch ex As Exception
                Throw New WebException("多线程直接下载文件失败（第一下载源：" + Urls.First() + "）", ex)
            Finally
                loaderDownload.Cancel()
            End Try
        End Sub

        Private Function SendRequest(Url As String, Method As HttpMethod, Optional Content As Object = Nothing, Optional Headers As Dictionary(Of String, String) = Nothing, Optional SimulateBrowserHeaders As Boolean = False, Optional Timeout As Integer = 25000, Optional Encoding As Encoding = Nothing) As Byte()
            Dim CS$<>8__locals1 As ModNet._Closure$__9-0 = New ModNet._Closure$__9-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Timeout = Timeout
            If ModBase.RunInUi() AndAlso Not Url.Contains("//127.") Then
                Throw New Exception("在 UI 线程执行了网络请求")
            End If
            CS$<>8__locals1.$VB$Local_Request = Nothing
            CS$<>8__locals1.$VB$Local_CancelToken = Nothing
            CS$<>8__locals1.$VB$Local_Response = Nothing
            Dim array2 As Byte()
            Try
                Directory.CreateDirectory(ModBase.PathTemp + "Cache\Http\")
                Dim requestClientLock As Object = ModNet.RequestClientLock
                ObjectFlowControl.CheckForSyncLockOnValueType(requestClientLock)
                SyncLock requestClientLock
                    If ModNet.RequestClient Is Nothing Then
                        ModNet.RequestClient = ClientExtensions.CreateClient(New FileStore(ModBase.PathTemp + "Cache/Http/"), New HttpClientHandler() With { .AutomaticDecompression = (DecompressionMethods.GZip Or DecompressionMethods.Deflate), .UseCookies = False })
                    End If
                End SyncLock
                Url = ModSecret.SecretCdnSign(Url)
                CS$<>8__locals1.$VB$Local_Request = New HttpRequestMessage(Method, Url)
                If Content IsNot Nothing AndAlso CS$<>8__locals1.$VB$Local_Request.Method <> HttpMethod.[Get] AndAlso CS$<>8__locals1.$VB$Local_Request.Method <> HttpMethod.Head Then
                    If TypeOf Content Is HttpContent Then
                        CS$<>8__locals1.$VB$Local_Request.Content = WebExtensions.Clone(CType(Content, HttpContent))
                    ElseIf TypeOf Content Is Byte() Then
                        CS$<>8__locals1.$VB$Local_Request.Content = New ByteArrayContent(CType(Content, Byte()))
                    ElseIf Content IsNot Nothing Then
                        CS$<>8__locals1.$VB$Local_Request.Content = New ByteArrayContent((If(Encoding, Encoding.UTF8)).GetBytes(Content.ToString()))
                    End If
                End If
                Try
                    For Each keyValuePair As KeyValuePair(Of String, String) In (If(Headers, New Dictionary(Of String, String)()))
                        If Operators.CompareString(keyValuePair.Key.Lower(), "content-type", False) = 0 Then
                            Dim content As HttpContent = CS$<>8__locals1.$VB$Local_Request.Content
                            If content IsNot Nothing Then
                                content.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value)
                            End If
                        Else
                            CS$<>8__locals1.$VB$Local_Request.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value)
                        End If
                    Next
                Finally
                    Dim enumerator As Dictionary(Of String, String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                ModSecret.SecretHeadersSign(Url, CS$<>8__locals1.$VB$Local_Request, SimulateBrowserHeaders)
                Dim action As Action(Of Integer) = Sub(Attempt As Integer)
                    Dim $VB$Local_CancelToken2 As CancellationTokenSource = CS$<>8__locals1.$VB$Local_CancelToken
                    If $VB$Local_CancelToken2 IsNot Nothing Then
                        $VB$Local_CancelToken2.Dispose()
                    End If
                    CS$<>8__locals1.$VB$Local_CancelToken = New CancellationTokenSource(CS$<>8__locals1.$VB$Local_Timeout)
                    Dim httpRequestMessage As HttpRequestMessage = WebExtensions.Clone(CS$<>8__locals1.$VB$Local_Request)
                    Try
                        CS$<>8__locals1.$VB$Local_Response = ModNet.RequestClient.SendAsync(httpRequestMessage, 1, CS$<>8__locals1.$VB$Local_CancelToken.Token).GetResultWithTimeout(CS$<>8__locals1.$VB$Local_CancelToken, CS$<>8__locals1.$VB$Local_Timeout)
                    Finally
                        httpRequestMessage.Dispose()
                    End Try
                End Sub
                Dim num As UInteger? = New UInteger?(2UI)
                Dim func As Func(Of Integer, TimeSpan)
                If ModNet._Closure$__.$IR9-1 IsNot Nothing Then
                    func = ModNet._Closure$__.$IR9-1
                Else
                    Dim func2 As Func(Of Integer, TimeSpan) = Function(a0 As Integer)
                        Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_0(Of TimeSpan)
                        If ModNet._Closure$__.$I9-1 IsNot Nothing Then
                            vb$AnonymousDelegate_ = ModNet._Closure$__.$I9-1
                        Else
                            Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_0(Of TimeSpan) = Function() TimeSpan.FromMilliseconds(200.0)
                            vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                            ModNet._Closure$__.$I9-1 = vb$AnonymousDelegate_2
                        End If
                        Return vb$AnonymousDelegate_()
                    End Function
                    func = func2
                    ModNet._Closure$__.$IR9-1 = func2
                End If
                Dim func3 As Func(Of Exception, Boolean)
                If ModNet._Closure$__.$I9-2 IsNot Nothing Then
                    func3 = ModNet._Closure$__.$I9-2
                Else
                    Dim func4 As Func(Of Exception, Boolean) = Function(ex As Exception) TypeOf ex Is FileNotFoundException OrElse TypeOf ex Is IOException
                    func3 = func4
                    ModNet._Closure$__.$I9-2 = func4
                End If
                Retrier.Attempt(action, num, func, func3, Nothing, Nothing)
                CS$<>8__locals1.$VB$Local_CancelToken.Dispose()
                CS$<>8__locals1.$VB$Local_CancelToken = New CancellationTokenSource(CS$<>8__locals1.$VB$Local_Timeout)
                Dim array As Byte()
                Using resultWithTimeout As Stream = CS$<>8__locals1.$VB$Local_Response.Content.ReadAsStreamAsync().GetResultWithTimeout(CS$<>8__locals1.$VB$Local_CancelToken, CS$<>8__locals1.$VB$Local_Timeout)
                    Using memoryStream As MemoryStream = New MemoryStream()
                        resultWithTimeout.CopyToAsync(memoryStream, 81920, CS$<>8__locals1.$VB$Local_CancelToken.Token).GetResultWithTimeout(CS$<>8__locals1.$VB$Local_CancelToken, CS$<>8__locals1.$VB$Local_Timeout)
                        array = memoryStream.ToArray()
                    End Using
                End Using
                If Not CS$<>8__locals1.$VB$Local_Response.IsSuccessStatusCode Then
                    Dim [string] As String = array.GetString(Encoding)
                    Throw New HttpRequestCodeException(String.Format("错误码 {0} ({1})，{2}，{3}", New Object() { CS$<>8__locals1.$VB$Local_Response.StatusCode, CInt(CS$<>8__locals1.$VB$Local_Response.StatusCode), Method, Url }) + If(String.IsNullOrEmpty([string]), "", (vbCrLf + [string])), CS$<>8__locals1.$VB$Local_Response.StatusCode, [string])
                End If
                array2 = array
            Catch ex As HttpRequestException
                Throw
            Catch ex2 As TimeoutException
                Throw New WebException(String.Format("连接服务器超时，请稍后再试，或使用 VPN 改善网络环境（{0}, {1}）", Method, Url), WebExceptionStatus.Timeout)
            Catch ex3 As Exception
                If Not(TypeOf ex3 Is TaskCanceledException) Then
                    ex3.ThrowIfCanceled()
                End If
                If ex3.IsBadNetwork() Then
                    Throw New WebException(String.Format("网络请求失败，请稍后再试，或使用 VPN 改善网络环境（{0}, {1}）", Method, Url), WebExceptionStatus.Timeout)
                End If
                Throw New Exception(String.Format("网络请求出现意外异常（{0}, {1}）", Method, Url), ex3)
            Finally
                Dim $VB$Local_Request As HttpRequestMessage = CS$<>8__locals1.$VB$Local_Request
                If $VB$Local_Request IsNot Nothing Then
                    $VB$Local_Request.Dispose()
                End If
                Dim $VB$Local_CancelToken As CancellationTokenSource = CS$<>8__locals1.$VB$Local_CancelToken
                If $VB$Local_CancelToken IsNot Nothing Then
                    $VB$Local_CancelToken.Dispose()
                End If
                Dim $VB$Local_Response As HttpResponseMessage = CS$<>8__locals1.$VB$Local_Response
                If $VB$Local_Response IsNot Nothing Then
                    $VB$Local_Response.Dispose()
                End If
            End Try
            Return array2
        End Function

        Public ReadOnly Property NetTaskThreadLimit As Integer
            Get
                ' The following expression was wrapped in a checked-expression
                Return Settings.[Get](Of Integer)("ToolDownloadThread", Nothing) + 1
            End Get
        End Property

        Public ReadOnly Property NetTaskSpeedLimitHigh As Long
            Get
                ' The following expression was wrapped in a checked-statement
                If ModNet._NetTaskSpeedLimitHigh Is Nothing Then
                    Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadSpeed", Nothing)
                    If num <= 14 Then
                        ' The following expression was wrapped in a unchecked-expression
                        ' The following expression was wrapped in a checked-expression
                        ModNet._NetTaskSpeedLimitHigh = New Long?(CLng(Math.Round(CDbl((num + 1)) * 0.1 * 1024.0 * 1024.0)))
                    ElseIf num <= 31 Then
                        ' The following expression was wrapped in a unchecked-expression
                        ' The following expression was wrapped in a checked-expression
                        ModNet._NetTaskSpeedLimitHigh = New Long?(CLng(Math.Round(CDbl((num - 11)) * 0.5 * 1024.0 * 1024.0)))
                    ElseIf num <= 41 Then
                        ' The following expression was wrapped in a unchecked-expression
                        ' The following expression was wrapped in a checked-expression
                        ModNet._NetTaskSpeedLimitHigh = New Long?(CLng(((num - 21) * 1024)) * 1024L)
                    Else
                        ModNet._NetTaskSpeedLimitHigh = New Long?(-1L)
                    End If
                End If
                Return ModNet._NetTaskSpeedLimitHigh.Value
            End Get
        End Property

        Public Sub UpdateNetTaskSpeedLimitHigh(Value As Integer)
            ModNet._NetTaskSpeedLimitHigh = Nothing
        End Sub

        Public Function HasDownloadingTask(Optional IgnoreCustomDownload As Boolean = False) As Boolean
            Return ModLoader.LoaderTaskbar.Any(Function(Task As ModLoader.LoaderBase) Task.Show AndAlso Task.State = ModBase.LoadState.Loading AndAlso (Not IgnoreCustomDownload OrElse Not Task.Name.ToString().Contains("自定义下载")))
        End Function

        Public Function FindFreePort() As Integer
            Dim tcpListener As TcpListener = New TcpListener(IPAddress.Loopback, 0)
            tcpListener.Start()
            Dim port As Integer = CType(tcpListener.LocalEndpoint, IPEndPoint).Port
            tcpListener.[Stop]()
            Return port
        End Function

        Public Function GetUsedPorts() As List(Of Integer)
            Dim ipglobalProperties As IPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties()
            Dim list As List(Of Integer) = New List(Of Integer)()
            Dim activeTcpListeners As IEnumerable(Of IPEndPoint) = ipglobalProperties.GetActiveTcpListeners()
            Dim func As Func(Of IPEndPoint, Integer)
            If ModNet._Closure$__.$I33-0 IsNot Nothing Then
                func = ModNet._Closure$__.$I33-0
            Else
                Dim func2 As Func(Of IPEndPoint, Integer) = Function(ep As IPEndPoint) ep.Port
                func = func2
                ModNet._Closure$__.$I33-0 = func2
            End If
            list.AddRange(activeTcpListeners.[Select](func))
            Dim activeUdpListeners As IEnumerable(Of IPEndPoint) = ipglobalProperties.GetActiveUdpListeners()
            Dim func3 As Func(Of IPEndPoint, Integer)
            If ModNet._Closure$__.$I33-1 IsNot Nothing Then
                func3 = ModNet._Closure$__.$I33-1
            Else
                Dim func4 As Func(Of IPEndPoint, Integer) = Function(ep As IPEndPoint) ep.Port
                func3 = func4
                ModNet._Closure$__.$I33-1 = func4
            End If
            list.AddRange(activeUdpListeners.[Select](func3))
            Dim activeTcpConnections As IEnumerable(Of TcpConnectionInformation) = ipglobalProperties.GetActiveTcpConnections()
            Dim func5 As Func(Of TcpConnectionInformation, Integer)
            If ModNet._Closure$__.$I33-2 IsNot Nothing Then
                func5 = ModNet._Closure$__.$I33-2
            Else
                Dim func6 As Func(Of TcpConnectionInformation, Integer) = Function(conn As TcpConnectionInformation) conn.LocalEndPoint.Port
                func5 = func6
                ModNet._Closure$__.$I33-2 = func6
            End If
            list.AddRange(activeTcpConnections.[Select](func5))
            Return list.Distinct().ToList()
        End Function

        Public Function FindFreePorts(ConsecutiveCount As Integer, ParamArray ExtraBlackLists As Integer()) As List(Of Integer)
            Dim CS$<>8__locals1 As ModNet._Closure$__34-0 = New ModNet._Closure$__34-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_UsedPorts = ModNet.GetUsedPorts().Concat(ExtraBlackLists)
            Dim num As Integer = 12000 + ModBase.RandomInteger(0, 1000)
            Dim num2 As Integer = 65000 - ConsecutiveCount
            For i As Integer = num To num2
                Dim enumerable As IEnumerable(Of Integer) = Enumerable.Range(i, ConsecutiveCount)
                Dim enumerable2 As IEnumerable(Of Integer) = enumerable
                Dim func As Func(Of Integer, Boolean)
                If CS$<>8__locals1.$I0 IsNot Nothing Then
                    func = CS$<>8__locals1.$I0
                Else
                    Dim CS$<>8__locals2 As ModNet._Closure$__34-0 = CS$<>8__locals1
                    Dim func2 As Func(Of Integer, Boolean) = Function(p As Integer) CS$<>8__locals1.$VB$Local_UsedPorts.Contains(p)
                    Dim func3 As Func(Of Integer, Boolean) = func2
                    CS$<>8__locals2.$I0 = func2
                    func = func3
                End If
                If Not enumerable2.Any(func) Then
                    Return enumerable.ToList()
                End If
            Next
            Throw New Exception("未能找到可用的端口！")
        End Function

        Public Function Ping(Ip As String, Optional Timeout As Integer = 10000, Optional MakeLog As Boolean = True) As Integer
            Dim pingReply As PingReply
            Try
                pingReply = New Ping().Send(Ip)
            Catch ex As Exception
                If MakeLog Then
                    Logger.Info(String.Format("Ping {0} 失败：{1}", Ip, ex.Message), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                End If
                Return -1
            End Try
            Dim num As Integer
            If pingReply.Status = IPStatus.Success Then
                If MakeLog Then
                    Logger.Info(String.Format("Ping {0} 结束：{1}ms", Ip, pingReply.RoundtripTime), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                End If
                num = CInt(pingReply.RoundtripTime)
            Else
                If MakeLog Then
                    Logger.Info(String.Format("Ping {0} 失败", Ip), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                End If
                num = -1
            End If
            Return num
        End Function

        Public Sub ShouldValidateSslCertificateOnLogin(Enabled As Boolean)
            ServicePointManager.ServerCertificateValidationCallback = Function(Sender As Object, Certificate As X509Certificate, Chain As X509Chain, Failure As SslPolicyErrors)
                Dim httpWebRequest As HttpWebRequest = TryCast(Sender, HttpWebRequest)
                Dim flag As Boolean
                If Failure = SslPolicyErrors.None Then
                    flag = True
                Else
                    Logger.Warn(String.Format("未通过 SSL 证书验证（{0}），提供的证书为 {1}，URL：{2}", Failure, If((Certificate IsNot Nothing), Certificate.Subject, Nothing), If((httpWebRequest IsNot Nothing), httpWebRequest.Address, Nothing)), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                    If httpWebRequest Is Nothing Then
                        flag = Not Enabled
                    Else
                        flag = (httpWebRequest.Address.Host.Contains("xboxlive") OrElse httpWebRequest.Address.Host.Contains("minecraftservices")) AndAlso Not Enabled
                    End If
                End If
                Return flag
            End Function
        End Sub

        Private RequestClient As HttpClient = Nothing

        Private RequestClientLock As Object = RuntimeHelpers.GetObjectValue(New Object())

        Private ThreadClient As HttpClient = New HttpClient(New HttpClientHandler() With { .AutomaticDecompression = (DecompressionMethods.GZip Or DecompressionMethods.Deflate) })

        Public NetTaskSpeedLimitLow As Long = 262144L

        Private _NetTaskSpeedLimitHigh As Long? = Nothing

        Public NetTaskSpeedLimitLeft As Long = -1L

        Public NetTaskThreadCount As Integer = 0

        Public NetManager As ModNet.NetManagerClass = New ModNet.NetManagerClass()

        Public Class NetSource
            Public Overrides Function ToString() As String
                Return Me.Url
            End Function

            Public Id As Integer

            Public Url As String

            Public FailCount As Integer

            Public Ex As Exception

            Public SingleThread As ModNet.NetThread

            Public IsFailed As Boolean
        End Class

        Public Enum NetState
            WaitingToCheck = -1
            WaitingToDownload
            Connecting
            Reading
            Downloading
            Merging
            Finished
            Canceled
        End Enum

        Public Enum NetPreDownloadBehaviour
            HintWhileExists
            ExitWhileExistsOrDownloading
            IgnoreCheck
        End Enum

        Public Class NetThread
            Implements IEnumerable(Of ModNet.NetThread), IEquatable(Of ModNet.NetThread)

            Public Sub New()
                Me.DownloadDone = 0L
                Me.SpeedLastTime = ModBase.GetTimeMs()
                Me.SpeedLastDone = 0L
                Me._Speed = 0L
                Me.InitTime = ModBase.GetTimeMs()
                Me.LastReceiveTime = -1L
                Me.State = ModNet.NetState.WaitingToDownload
            End Sub

            Private ReadOnly Property [Next] As IEnumerable(Of ModNet.NetThread)
                Get
                    Dim vb$StateMachine_5_get_Next As ModNet.NetThread.VB$StateMachine_5_get_Next = New ModNet.NetThread.VB$StateMachine_5_get_Next(-2)
                    vb$StateMachine_5_get_Next.$VB$Me = Me
                    Return vb$StateMachine_5_get_Next
                End Get
            End Property

            Public Function GetEnumerator() As IEnumerator(Of ModNet.NetThread) Implements System.Collections.Generic.IEnumerable(Of PCL.ModNet.NetThread).GetEnumerator
                Return Me.[Next].GetEnumerator()
            End Function

            Function IEnumerable_GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
                Return Me.[Next].GetEnumerator()
            End Function

            Public ReadOnly Property IsFirstThread As Boolean
                Get
                    Return Me.DownloadStart = 0L AndAlso Me.Task.FileSize = -2L
                End Get
            End Property

            Public ReadOnly Property DownloadEnd As Long
                Get
                    Dim lockChain As Object = Me.Task.LockChain
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockChain)
                    Dim num As Long
                    SyncLock lockChain
                        If Me.NextThread Is Nothing Then
                            If Me.Task.IsUnknownSize Then
                                num = 1000000000000L
                            Else
                                num = Me.Task.FileSize - 1L
                            End If
                        Else
                            num = Me.NextThread.DownloadStart - 1L
                        End If
                    End SyncLock
                    Return num
                End Get
            End Property

            Public ReadOnly Property DownloadUndone As Long
                Get
                    ' The following expression was wrapped in a checked-expression
                    Return Me.DownloadEnd - (Me.DownloadStart + Me.DownloadDone) + 1L
                End Get
            End Property

            Public ReadOnly Property Speed As Long
                Get
                    ' The following expression was wrapped in a checked-statement
                    If ModBase.GetTimeMs() - Me.SpeedLastTime > 200L Then
                        Dim num As Long = ModBase.GetTimeMs() - Me.SpeedLastTime
                        Me._Speed = CLng(Math.Round(CDbl((Me.DownloadDone - Me.SpeedLastDone)) / (CDbl(num) / 1000.0)))
                        Me.SpeedLastDone = Me.DownloadDone
                        Dim ptr As __ByRef(Of Long) = Me.SpeedLastTime
                        Me.SpeedLastTime = ptr + num
                    End If
                    Return Me._Speed
                End Get
            End Property

            Public ReadOnly Property IsEnded As Boolean
                Get
                    Return Me.State = ModNet.NetState.Finished OrElse Me.State = ModNet.NetState.Canceled
                End Get
            End Property

            Public Overloads Function Equals(other As ModNet.NetThread) As Boolean Implements System.IEquatable(Of PCL.ModNet.NetThread).Equals
                Return other IsNot Nothing AndAlso Me.Uuid = other.Uuid
            End Function

            Public Overrides Overloads Function Equals(obj As Object) As Boolean
                Return Me.Equals(TryCast(obj, ModNet.NetThread))
            End Function

            Public Shared Operator =(left As ModNet.NetThread, right As ModNet.NetThread) As Boolean
                Return EqualityComparer(Of ModNet.NetThread).[Default].Equals(left, right)
            End Operator

            Public Shared Operator <>(left As ModNet.NetThread, right As ModNet.NetThread) As Boolean
                Return Not(left Is right)
            End Operator

            Public Task As ModNet.NetFile

            Public Thread As Thread

            Public NextThread As ModNet.NetThread

            Public Uuid As Integer

            Public Temp As String

            Public DownloadStart As Long

            Public DownloadDone As Long

            Private SpeedLastTime As Long

            Private SpeedLastDone As Long

            Private _Speed As Long

            Public InitTime As Long

            Public LastReceiveTime As Long

            Public State As ModNet.NetState

            Public Source As ModNet.NetSource
        End Class

        Public Class NetFile
            Private Function GetSource(Optional Id As Integer = 0) As ModNet.NetSource
                If Id >= Me.Sources.Count OrElse Id < 0 Then
                    Id = 0
                End If
                Dim lockSource As Object = Me.LockSource
                ObjectFlowControl.CheckForSyncLockOnValueType(lockSource)
                Dim netSource2 As ModNet.NetSource
                SyncLock lockSource
                    If Me.HasAvailableSource(False) Then
                        Dim netSource As ModNet.NetSource = Me.Sources(Id)
                        While netSource.IsFailed
                            Id += 1
                            If Id >= Me.Sources.Count Then
                                Id = 0
                            End If
                            netSource = Me.Sources(Id)
                        End While
                        netSource2 = netSource
                    ElseIf Me.SourcesOnce.Any() Then
                        netSource2 = Me.SourcesOnce.First()
                    Else
                        netSource2 = Nothing
                    End If
                End SyncLock
                Return netSource2
            End Function

            Public Function HasAvailableSource(Optional AllowOnceSource As Boolean = True) As Boolean
                Dim lockSource As Object = Me.LockSource
                ObjectFlowControl.CheckForSyncLockOnValueType(lockSource)
                SyncLock lockSource
                    Dim sources As IEnumerable(Of ModNet.NetSource) = Me.Sources
                    Dim func As Func(Of ModNet.NetSource, Boolean)
                    If ModNet.NetFile._Closure$__.$I6-0 IsNot Nothing Then
                        func = ModNet.NetFile._Closure$__.$I6-0
                    Else
                        Dim func2 As Func(Of ModNet.NetSource, Boolean) = Function(s As ModNet.NetSource) Not s.IsFailed
                        func = func2
                        ModNet.NetFile._Closure$__.$I6-0 = func2
                    End If
                    If sources.Any(func) Then
                        Return True
                    End If
                    If AllowOnceSource AndAlso Me.SourcesOnce.Any() Then
                        Return True
                    End If
                End SyncLock
                Return False
            End Function

            Public ReadOnly Property IsNoSplit As Boolean
                Get
                    Return Me.IsUnknownSize OrElse Me.FileSize < 1048576L
                End Get
            End Property

            Public ReadOnly Property Speed As Long
                Get
                    ' The following expression was wrapped in a checked-statement
                    If ModBase.GetTimeMs() - Me.SpeedLastTime > 200L Then
                        Dim num As Long = ModBase.GetTimeMs() - Me.SpeedLastTime
                        Me._Speed = CLng(Math.Round(CDbl((Me.DownloadDone - Me.SpeedLastDone)) / (CDbl(num) / 1000.0)))
                        Me.SpeedLastDone = Me.DownloadDone
                        Dim ptr As __ByRef(Of Long) = Me.SpeedLastTime
                        Me.SpeedLastTime = ptr + num
                    End If
                    Return Me._Speed
                End Get
            End Property

            Public ReadOnly Property Progress As Double
                Get
                    Dim num As Double
                    Select Case Me.State
                        Case ModNet.NetState.WaitingToCheck
                            num = 0.0
                        Case ModNet.NetState.WaitingToDownload
                            num = 0.01
                        Case ModNet.NetState.Connecting
                            num = 0.02
                        Case ModNet.NetState.Reading
                            num = 0.04
                        Case ModNet.NetState.Downloading
                            Dim num2 As Double = If(Me.IsUnknownSize, 0.5, (CDbl(Me.DownloadDone) / CDbl(Math.Max(Me.FileSize, 1L))))
                            num2 = 1.0 - Math.Pow(1.0 - num2, 0.9)
                            num = num2 * 0.93 + 0.05
                        Case ModNet.NetState.Merging
                            num = 0.99
                        Case ModNet.NetState.Finished, ModNet.NetState.Canceled
                            num = 1.0
                        Case Else
                            Throw New ArgumentOutOfRangeException("文件状态未知：" + Conversions.ToString(CInt(Me.State)))
                    End Select
                    Return num
                End Get
            End Property

            Private ReadOnly Property ConnectAverage As Integer
                Get
                    Dim connectCount As Integer = Me.ConnectCount
                    Return CInt(Math.Round(If((connectCount = 0), (-1.0), (CDbl(Me.ConnectTime) / CDbl(connectCount)))))
                End Get
            End Property

            Public Overrides Function Equals(obj As Object) As Boolean
                Dim netFile As ModNet.NetFile = TryCast(obj, ModNet.NetFile)
                Return netFile IsNot Nothing AndAlso Me.Uuid = netFile.Uuid
            End Function

            Public Sub New(Urls As IEnumerable(Of String), LocalPath As String, Optional Checker As FileChecker = Nothing, Optional SimulateBrowserHeaders As Boolean = False)
                Me.Tasks = New ConcurrentList(Of ModNet.LoaderDownload)()
                Me.FirstThreadSource = 0
                Me.SourcesOnce = New ConcurrentList(Of ModNet.NetSource)()
                Me.Retried = False
                Me.LocalPath = Nothing
                Me.LocalName = Nothing
                Me.State = ModNet.NetState.WaitingToCheck
                Me.Ex = New List(Of Exception)()
                Me.FileSize = -2L
                Me.IsUnknownSize = False
                Me.DownloadDone = 0L
                Me.SpeedLastTime = ModBase.GetTimeMs()
                Me.SpeedLastDone = 0L
                Me._Speed = 0L
                Me.IsCopy = False
                Me.ConnectCount = 0
                Me.ConnectTime = 0L
                Me.LockState = RuntimeHelpers.GetObjectValue(New Object())
                Me.LockChain = RuntimeHelpers.GetObjectValue(New Object())
                Me.LockSource = RuntimeHelpers.GetObjectValue(New Object())
                Me.Uuid = ModBase.GetUuid()
                Dim list As List(Of ModNet.NetSource) = New List(Of ModNet.NetSource)()
                Dim num As Integer = 0
                Urls = Urls.Distinct().ToArray()
                Try
                    For Each text As String In Urls
                        list.Add(New ModNet.NetSource() With { .FailCount = 0, .Url = ModSecret.SecretCdnSign(text.ReplaceLineEndings("", False).Trim()), .Id = num, .IsFailed = False, .Ex = Nothing })
                        num += 1
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of String)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Me.Sources = list
                Me.LocalPath = LocalPath
                Me.Check = Checker
                Me.SimulateBrowserHeaders = SimulateBrowserHeaders
                Me.LocalName = PathUtils.GetLastPart(LocalPath)
            End Sub

            Public Function TryBeginThread() As ModNet.NetThread
                ' The following expression was wrapped in a checked-statement
                Dim netThread6 As ModNet.NetThread
                Try
                    Dim netSource As ModNet.NetSource = Nothing
                    Dim lockSource As Object = Me.LockSource
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockSource)
                    SyncLock lockSource
                        If ModNet.NetTaskThreadCount >= ModNet.NetTaskThreadLimit OrElse Not Me.HasAvailableSource(True) OrElse (Me.IsNoSplit AndAlso Me.Threads IsNot Nothing AndAlso Me.Threads.State <> ModNet.NetState.Canceled) Then
                            Return Nothing
                        End If
                        If Me.State >= ModNet.NetState.Merging OrElse Me.State = ModNet.NetState.WaitingToCheck Then
                            Return Nothing
                        End If
                        Dim lockState As Object = Me.LockState
                        ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                        SyncLock lockState
                            If Me.State < ModNet.NetState.Connecting Then
                                Me.State = ModNet.NetState.Connecting
                            End If
                        End SyncLock
                        If Not Me.IsNoSplit Then
                            If Me.HasAvailableSource(False) Then
                                GoTo IL_00F8
                            End If
                            If Me.SourcesOnce.First().SingleThread IsNot Nothing AndAlso Me.SourcesOnce.First().SingleThread.State <> ModNet.NetState.Canceled Then
                                Return Nothing
                            End If
                        End If
                        Me.Reset()
                        Me.State = ModNet.NetState.Reading
                        IL_00F8:
                    End SyncLock
                    Dim lockChain As Object = Me.LockChain
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockChain)
                    Dim thread As Thread
                    Dim netThread4 As ModNet.NetThread
                    SyncLock lockChain
                        Dim num As Long
                        If Me.Threads Is Nothing Then
                            num = 0L
                            netSource = Me.GetSource(Me.FirstThreadSource)
                            Me.FirstThreadSource = netSource.Id + 1
                        Else
                            Try
                                For Each netThread As ModNet.NetThread In Me.Threads
                                    If netThread.State = ModNet.NetState.Canceled AndAlso netThread.DownloadUndone > 0L Then
                                        num = netThread.DownloadStart + netThread.DownloadDone
                                        netSource = Me.GetSource(netThread.Source.Id + 1)
                                        GoTo IL_02C2
                                    End If
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of ModNet.NetThread)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                            Dim source As ModNet.NetSource = Me.GetSource(0)
                            If source Is Nothing Then
                                Return Nothing
                            End If
                            Dim url As String = source.Url
                            If url.Contains("pcl2-server") OrElse url.Contains("meloong.com") OrElse url.Contains("bmclapi") OrElse url.Contains("github.com") OrElse url.Contains("optifine.net") OrElse url.Contains("momot.rs") Then
                                Return Nothing
                            End If
                            Dim netThread2 As ModNet.NetThread = Me.Threads
                            Try
                                For Each netThread3 As ModNet.NetThread In Me.Threads
                                    If netThread3.DownloadUndone > netThread2.DownloadUndone Then
                                        netThread2 = netThread3
                                    End If
                                Next
                            Finally
                                Dim enumerator2 As IEnumerator(Of ModNet.NetThread)
                                If enumerator2 IsNot Nothing Then
                                    enumerator2.Dispose()
                                End If
                            End Try
                            If netThread2 Is Nothing OrElse netThread2.DownloadUndone < 262144L Then
                                Return Nothing
                            End If
                            num = CLng(Math.Round(CDbl(netThread2.DownloadEnd) - CDbl(netThread2.DownloadUndone) * 0.4))
                            netSource = Me.GetSource(0)
                        End If
                        IL_02C2:
                        If(num > Me.FileSize AndAlso Me.FileSize >= 0L AndAlso Not Me.IsUnknownSize) OrElse num < 0L OrElse Information.IsNothing(netSource) Then
                            Return Nothing
                        End If
                        Dim uuid As Integer = ModBase.GetUuid()
                        If Not Me.Tasks.Any() Then
                            Return Nothing
                        End If
                        thread = New Thread(Sub(a0 As Object)
                            Me.Thread(CType(a0, ModNet.NetThread))
                        End Sub) With { .Name = String.Format("下载 {0}/{1}/{2}#", Me.Tasks(0).Uuid, Me.Uuid, uuid), .Priority = ThreadPriority.BelowNormal }
                        netThread4 = New ModNet.NetThread() With { .Uuid = uuid, .DownloadStart = num, .Thread = thread, .Source = netSource, .Task = Me, .State = ModNet.NetState.WaitingToDownload }
                        If netThread4.IsFirstThread OrElse Me.Threads Is Nothing Then
                            Me.Threads = netThread4
                        Else
                            Dim netThread5 As ModNet.NetThread = Me.Threads
                            While netThread5.DownloadEnd <= num
                                netThread5 = netThread5.NextThread
                            End While
                            netThread4.NextThread = netThread5.NextThread
                            netThread5.NextThread = netThread4
                        End If
                    End SyncLock
                    Interlocked.Increment(ModNet.NetTaskThreadCount)
                    Dim lockSource2 As Object = Me.LockSource
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockSource2)
                    SyncLock lockSource2
                        If Not Me.HasAvailableSource(False) Then
                            Me.SourcesOnce.First().SingleThread = netThread4
                        End If
                    End SyncLock
                    thread.Start(netThread4)
                    netThread6 = netThread4
                Catch ex As Exception
                    Me.Fail(New Exception(String.Format("尝试开始下载线程失败（{0}）", Me.LocalName), ex))
                    netThread6 = Nothing
                End Try
                Return netThread6
            End Function

            Private Sub Thread(Th As ModNet.NetThread)
                Dim CS$<>8__locals1 As ModNet.NetFile._Closure$__40-0 = New ModNet.NetFile._Closure$__40-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Me = Me
                CS$<>8__locals1.$VB$Local_Th = Th
                Logger.Log(String.Format("{0}：开始，起始点 {1}，{2}", Me.LocalName, CS$<>8__locals1.$VB$Local_Th.DownloadStart, CS$<>8__locals1.$VB$Local_Th.Source.Url), If((CS$<>8__locals1.$VB$Local_Th.DownloadStart = 0L), LogLevel.Info, LogLevel.Trace), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                Dim stream As Stream = Nothing
                Dim httpRequestMessage As HttpRequestMessage = Nothing
                CS$<>8__locals1.$VB$Local_Response = Nothing
                Dim stream2 As Stream = Nothing
                Dim cancellationTokenSource As CancellationTokenSource = Nothing
                Dim num As Integer = Math.Min(Math.Max(Me.ConnectAverage, 15000) * (1 + CS$<>8__locals1.$VB$Local_Th.Source.FailCount), 30000)
                CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Connecting
                Try
                    Dim num2 As Integer = 0
                    Dim num3 As Long
                    If Not Me.SourcesOnce.Contains(CS$<>8__locals1.$VB$Local_Th.Source) OrElse Not(CS$<>8__locals1.$VB$Local_Th IsNot CS$<>8__locals1.$VB$Local_Th.Source.SingleThread) Then
                        httpRequestMessage = New HttpRequestMessage(HttpMethod.[Get], CS$<>8__locals1.$VB$Local_Th.Source.Url)
                        ModSecret.SecretHeadersSign(CS$<>8__locals1.$VB$Local_Th.Source.Url, httpRequestMessage, Me.SimulateBrowserHeaders)
                        cancellationTokenSource = New CancellationTokenSource(num)
                        If Not CS$<>8__locals1.$VB$Local_Th.IsFirstThread Then
                            httpRequestMessage.Headers.Range = New RangeHeaderValue(New Long?(CS$<>8__locals1.$VB$Local_Th.DownloadStart), Nothing)
                        End If
                        num3 = 0L
                        CS$<>8__locals1.$VB$Local_Response = ModNet.ThreadClient.SendAsync(httpRequestMessage, 1, cancellationTokenSource.Token).GetResultWithTimeout(cancellationTokenSource, num)
                        If Not CS$<>8__locals1.$VB$Local_Response.IsSuccessStatusCode Then
                            Throw New HttpRequestCodeException(String.Format("错误码 {0} ({1})，{2}", CS$<>8__locals1.$VB$Local_Response.StatusCode, CInt(CS$<>8__locals1.$VB$Local_Response.StatusCode), CS$<>8__locals1.$VB$Local_Th.Source.Url), CS$<>8__locals1.$VB$Local_Response.StatusCode, Nothing)
                        End If
                        If Me.State <> ModNet.NetState.Canceled Then
                            If Operators.CompareString(CS$<>8__locals1.$VB$Local_Response.RequestMessage.RequestUri.ToString(), CS$<>8__locals1.$VB$Local_Th.Source.Url, False) <> 0 Then
                                Logger.Trace(Function() String.Format("{0}：重定向至 {1}", CS$<>8__locals1.$VB$Me.LocalName, CS$<>8__locals1.$VB$Local_Response.RequestMessage.RequestUri), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            End If
                            num3 = CS$<>8__locals1.$VB$Local_Response.Content.Headers.ContentLength.GetValueOrDefault(-1L)
                            If num3 < 0L Then
                                If Me.FileSize <= 1L Then
                                    Me.FileSize = -1L
                                    Me.IsUnknownSize = True
                                    Logger.Info(String.Format("{0}：文件大小未知", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                                    GoTo IL_0625
                                End If
                                If CS$<>8__locals1.$VB$Local_Th.DownloadStart = 0L Then
                                    Logger.Info(String.Format("{0}：文件大小未知，但已从其他下载源获取，不作处理", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                                    GoTo IL_0625
                                End If
                                Logger.Info(String.Format("{0}：ContentLength 返回了 {1}，无法确定是否支持分段下载，视作不支持", Me.LocalName, num3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            ElseIf CS$<>8__locals1.$VB$Local_Th.IsFirstThread Then
                                If Me.Check IsNot Nothing Then
                                    If num3 < Me.Check.MinSize AndAlso Me.Check.MinSize > 0L Then
                                        Throw New Exception(String.Format("文件大小不足，获取结果为 {0}，要求至少为 {1}。", num3, Me.Check.MinSize))
                                    End If
                                    If num3 <> Me.Check.ActualSize AndAlso Me.Check.ActualSize > 0L Then
                                        Throw New Exception(String.Format("文件大小不一致，获取结果为 {0}，要求必须为 {1}。", num3, Me.Check.ActualSize))
                                    End If
                                End If
                                Me.FileSize = num3
                                Me.IsUnknownSize = False
                                Logger.Info(String.Format("{0}：文件大小 {1}（{2}）", Me.LocalName, num3, StringUtils.FormatByteSize(num3)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                                If num3 > 52428800L Then
                                    For Each driveInfo As DriveInfo In DriveInfo.GetDrives()
                                        Dim text As String = driveInfo.Name.First().ToString()
                                        Dim num4 As Double

                                            ' The following expression was wrapped in a checked-expression
                                            num4 = If(ModBase.PathTemp.StartsWithF(text, False), (CDbl(num3) * 1.1), 0.0) + CDbl(If(Me.LocalPath.StartsWithF(text, False), num3 + 5242880L, 0L))

                                        If num4 > 0.0 AndAlso CDbl(driveInfo.TotalFreeSpace) < num4 Then
                                            Throw New Exception(String.Concat(New String() { text, " 盘空间不足，无法进行下载。" & vbCrLf & "需要至少 ", StringUtils.FormatByteSize(CLng(Math.Round(num4))), " 空间，但当前仅剩余 ", StringUtils.FormatByteSize(driveInfo.TotalFreeSpace), "。", If(ModBase.PathTemp.StartsWithF(text, False), vbCrLf & vbCrLf & "下载时需要与文件同等大小的空间存放缓存，你可以在设置中调整缓存文件夹的位置。", "") }))
                                        End If
                                    Next
                                    GoTo IL_0625
                                End If
                                GoTo IL_0625
                            Else
                                If Me.FileSize < 0L Then
                                    Throw New Exception("非首线程运行时，尚未获取文件大小")
                                End If
                                If CS$<>8__locals1.$VB$Local_Th.DownloadStart <= 0L OrElse num3 < Me.FileSize Then
                                    If Me.FileSize - CS$<>8__locals1.$VB$Local_Th.DownloadStart <> num3 Then
                                        Throw New ModNet.RangeNotSupportedException(String.Format("获取到的分段大小不一致：Range 起始于 {0}，预期 ContentLength 为 {1}，返回 ContentLength 为 {2}，总文件大小 {3}", New Object() { CS$<>8__locals1.$VB$Local_Th.DownloadStart, Me.FileSize - CS$<>8__locals1.$VB$Local_Th.DownloadStart, num3, Me.FileSize }))
                                    End If
                                    GoTo IL_0625
                                End If
                            End If
                            Dim lockSource As Object = Me.LockSource
                            ObjectFlowControl.CheckForSyncLockOnValueType(lockSource)
                            SyncLock lockSource
                                If Me.SourcesOnce.Contains(CS$<>8__locals1.$VB$Local_Th.Source) Then
                                    GoTo IL_0A31
                                End If
                            End SyncLock
                            Throw New ModNet.RangeNotSupportedException(String.Format("该下载源不支持分段下载：Range 起始于 {0}，预期 ContentLength 为 {1}，返回 ContentLength 为 {2}，总文件大小 {3}", New Object() { CS$<>8__locals1.$VB$Local_Th.DownloadStart, Me.FileSize - CS$<>8__locals1.$VB$Local_Th.DownloadStart, num3, Me.FileSize }))
                            IL_0625:
                            CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Reading
                            Dim lockState As Object = Me.LockState
                            ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                            SyncLock lockState
                                If Me.State < ModNet.NetState.Reading Then
                                    Me.State = ModNet.NetState.Reading
                                End If
                            End SyncLock
                            If Me.IsNoSplit Then
                                CS$<>8__locals1.$VB$Local_Th.Temp = Nothing
                                Me.Cache = New MemoryStream()
                                stream = Me.Cache
                            Else
                                CS$<>8__locals1.$VB$Local_Th.Temp = String.Format("{0}Download\{1}_{2}_{3}.tmp", New Object() { ModBase.PathTemp, Me.Uuid, CS$<>8__locals1.$VB$Local_Th.Uuid, ModBase.RandomInteger(0, 999999) })
                                stream = FileUtils.CreateAsStream(CS$<>8__locals1.$VB$Local_Th.Temp)
                            End If
                            cancellationTokenSource.Dispose()
                            cancellationTokenSource = New CancellationTokenSource()
                            stream2 = CS$<>8__locals1.$VB$Local_Response.Content.ReadAsStreamAsync().GetAwaiter().GetResult()
                            If Settings.[Get](Of Boolean)("SystemDebugDelay", Nothing) Then
                                Global.System.Threading.Thread.Sleep(ModBase.RandomInteger(50, 3000))
                            End If
                            Dim array As Byte() = New Byte(16384) {}
                            num2 = stream2.ReadAsync(array, 0, 16384, cancellationTokenSource.Token).GetResultWithTimeout(cancellationTokenSource, num)
                            While (Me.IsUnknownSize OrElse CS$<>8__locals1.$VB$Local_Th.DownloadUndone > 0L) AndAlso num2 > 0 AndAlso Not ModBase.IsProgramEnding AndAlso Me.State < ModNet.NetState.Merging AndAlso (Not CS$<>8__locals1.$VB$Local_Th.Source.IsFailed OrElse CS$<>8__locals1.$VB$Local_Th.Source.SingleThread Is CS$<>8__locals1.$VB$Local_Th)
                                While ModNet.NetTaskSpeedLimitHigh > 0L AndAlso ModNet.NetTaskSpeedLimitLeft <= 0L
                                    Global.System.Threading.Thread.Sleep(16)
                                End While
                                Dim num5 As Integer = CInt((If(Me.IsUnknownSize, CLng(num2), Math.Min(CLng(num2), CS$<>8__locals1.$VB$Local_Th.DownloadUndone))))

                                    If ModNet.NetTaskSpeedLimitHigh > 0L Then
                                        ' The following expression was wrapped in a checked-expression
                                        Interlocked.Add(ModNet.NetTaskSpeedLimitLeft, CLng((0 - num5)))
                                    End If
                                    Dim num6 As Long = ModBase.GetTimeMs() - CS$<>8__locals1.$VB$Local_Th.LastReceiveTime
                                    If num6 > 100000000L Then
                                        num6 = 1L
                                    End If
                                    If num5 > 0 Then
                                        If CS$<>8__locals1.$VB$Local_Th.DownloadDone = 0L Then
                                            CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Downloading
                                            Dim lockState2 As Object = Me.LockState
                                            ObjectFlowControl.CheckForSyncLockOnValueType(lockState2)
                                            SyncLock lockState2
                                                If Me.State < ModNet.NetState.Downloading Then
                                                    Me.State = ModNet.NetState.Downloading
                                                End If
                                            End SyncLock
                                            Interlocked.Increment(Me.ConnectCount)
                                            Interlocked.Add(Me.ConnectTime, ModBase.GetTimeMs() - CS$<>8__locals1.$VB$Local_Th.InitTime)
                                        End If
                                        CS$<>8__locals1.$VB$Local_Th.Source.FailCount = 0
                                        Try
                                            For Each loaderDownload As ModNet.LoaderDownload In Me.Tasks
                                                loaderDownload.FailCount = 0
                                            Next
                                        Finally
                                            Dim enumerator As IEnumerator(Of ModNet.LoaderDownload)
                                            If enumerator IsNot Nothing Then
                                                enumerator.Dispose()
                                            End If
                                        End Try
                                        Interlocked.Add(ModNet.NetManager.DownloadDone, CLng(num5))
                                        Interlocked.Add(Me.DownloadDone, CLng(num5))
                                        Dim $VB$Local_Th As ModNet.NetThread = CS$<>8__locals1.$VB$Local_Th
                                        Dim ptr As __ByRef(Of Long) = $VB$Local_Th.DownloadDone
                                        $VB$Local_Th.DownloadDone = ptr + CLng(num5)
                                        stream.Write(array, 0, num5)
                                        If CS$<>8__locals1.$VB$Local_Th.DownloadUndone = 0L AndAlso Not Me.IsUnknownSize Then
                                            Exit While
                                        End If
                                        If CS$<>8__locals1.$VB$Local_Th.LastReceiveTime > 0L AndAlso num6 > 5000L AndAlso num6 > CLng(num5) AndAlso CS$<>8__locals1.$VB$Local_Th.Source.SingleThread Is Nothing Then
                                            Throw New TimeoutException(String.Concat(New String() { "由于速度过慢断开链接，下载 ", Conversions.ToString(num5), " B，消耗 ", Conversions.ToString(num6), " ms。" }))
                                        End If
                                        CS$<>8__locals1.$VB$Local_Th.LastReceiveTime = ModBase.GetTimeMs()
                                    ElseIf CS$<>8__locals1.$VB$Local_Th.LastReceiveTime > 0L AndAlso num6 > CLng(num) Then
                                        Throw New TimeoutException("操作超时，无数据。")
                                    End If
                                    num2 = stream2.ReadAsync(array, 0, 16384, cancellationTokenSource.Token).GetResultWithTimeout(cancellationTokenSource, num)

                            End While
                        End If
                    End If
                    IL_0A31:
                    If Me.State = ModNet.NetState.Canceled OrElse (CS$<>8__locals1.$VB$Local_Th.Source.IsFailed AndAlso CS$<>8__locals1.$VB$Local_Th.Source.SingleThread IsNot CS$<>8__locals1.$VB$Local_Th) OrElse (CS$<>8__locals1.$VB$Local_Th.DownloadUndone > 0L AndAlso Not Me.IsUnknownSize) Then
                        CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Canceled
                        Logger.Info(String.Format("{0}：中断", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                    Else
                        If num2 = 0 AndAlso CS$<>8__locals1.$VB$Local_Th.DownloadUndone > 0L AndAlso Not Me.IsUnknownSize Then
                            Throw New Exception(String.Format("返回的 ContentLength 过多：ContentLength 为 {0}，但获取到的总数据量仅为 {1}（全文件总数据量 {2}）", num3, CS$<>8__locals1.$VB$Local_Th.DownloadDone, Me.DownloadDone))
                        End If
                        CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Finished
                        Logger.Trace(Function() String.Format("{0}：完成，已下载 {1}（{2}~{3}）", New Object() { CS$<>8__locals1.$VB$Me.LocalName, CS$<>8__locals1.$VB$Local_Th.DownloadDone, CS$<>8__locals1.$VB$Local_Th.DownloadStart, CS$<>8__locals1.$VB$Local_Th.DownloadEnd }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                    End If
                Catch ex As Exception
                    If ex.IsBadNetwork() OrElse TypeOf ex Is TaskCanceledException Then
                        Logger.Info(New TimeoutException(String.Format("{0}：超时（{1}ms），{2}", Me.LocalName, num, ex.Message)), Nothing, LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                    Else
                        Logger.Info(ex, String.Format("{0}：出错", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                    End If
                    Me.SourceFail(CS$<>8__locals1.$VB$Local_Th, ex, False)
                Finally
                    If httpRequestMessage IsNot Nothing Then
                        httpRequestMessage.Dispose()
                    End If
                    Dim $VB$Local_Response As HttpResponseMessage = CS$<>8__locals1.$VB$Local_Response
                    If $VB$Local_Response IsNot Nothing Then
                        $VB$Local_Response.Dispose()
                    End If
                    If stream2 IsNot Nothing Then
                        stream2.Dispose()
                    End If
                    If cancellationTokenSource IsNot Nothing Then
                        cancellationTokenSource.Dispose()
                    End If
                    If Not Me.IsNoSplit AndAlso stream IsNot Nothing Then
                        stream.Dispose()
                    End If
                    Interlocked.Decrement(ModNet.NetTaskThreadCount)
                    If((Me.FileSize >= 0L AndAlso Me.DownloadDone >= Me.FileSize) OrElse (Me.FileSize = -1L AndAlso Me.DownloadDone > 0L)) AndAlso Me.State < ModNet.NetState.Merging AndAlso CS$<>8__locals1.$VB$Local_Th.State <> ModNet.NetState.Canceled Then
                        Me.Merge(CS$<>8__locals1.$VB$Local_Th)
                    End If
                End Try
            End Sub

            Private Sub SourceFail(Th As ModNet.NetThread, ex As Exception, IsMergeFailure As Boolean)
                Interlocked.Increment(Th.Source.FailCount)
                Try
                    For Each loaderDownload As ModNet.LoaderDownload In Me.Tasks
                        Dim failCount As Integer = loaderDownload.FailCount
                        Interlocked.Increment(failCount)
                        loaderDownload.FailCount = failCount
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModNet.LoaderDownload)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Th.State = ModNet.NetState.Canceled
                Th.Source.Ex = ex
                Dim flag As Boolean = TypeOf ex Is ModNet.RangeNotSupportedException OrElse ex.Message.Contains("(416)")
                If IsMergeFailure OrElse flag OrElse ex.Message.Contains("(502)") OrElse ex.Message.Contains("(404)") OrElse ex.Message.Contains("未能解析") OrElse ex.Message.Contains("无返回数据") OrElse ex.Message.Contains("空间不足") OrElse ((ex.Message.Contains("(403)") OrElse ex.Message.Contains("(429)")) AndAlso Not Th.Source.Url.Contains("bmclapi")) OrElse (Th.Source.FailCount >= ModNet.NetTaskThreadLimit.Clamp(5, 30) AndAlso Me.DownloadDone < 1L) OrElse Th.Source.FailCount > ModNet.NetTaskThreadLimit + 2 Then
                    Dim flag2 As Boolean = False
                    Dim lockSource As Object = Me.LockSource
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockSource)
                    SyncLock lockSource
                        If Not Th.Source.IsFailed OrElse Th.Source.SingleThread Is Th Then
                            flag2 = True
                            Th.Source.IsFailed = True
                        End If
                    End SyncLock
                    If flag2 Then
                        Logger.Info(String.Format("{0}：下载源被禁用（{1}，Range 问题：{2}）：{3}", New Object() { Me.LocalName, Th.Source.Id, flag, Th.Source.Url }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        Dim text As String = "{0}下载源 {1} 已被禁用"
                        Dim netSource As ModNet.NetSource = Me.SourcesOnce.FirstOrDefault()
                        Logger.Warn(ex, String.Format(text, If((If((netSource IsNot Nothing), netSource.SingleThread, Nothing) Is Nothing), "", "单线程"), Th.Source.Id), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        Dim lockSource2 As Object = Me.LockSource
                        ObjectFlowControl.CheckForSyncLockOnValueType(lockSource2)
                        SyncLock lockSource2
                            Me.SourcesOnce.Remove(Th.Source)
                        End SyncLock
                        If ex.Message.Contains("空间不足") Then
                            Me.Fail(ex)
                        ElseIf Not Me.HasAvailableSource(True) OrElse IsMergeFailure Then
                            If Not Me.Retried Then
                                If Not flag Then
                                    Logger.Warn(String.Format("{0}：文件下载失败，正在自动重试……", Me.LocalName), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                                End If
                                Me.Retried = True
                                Dim lockSource3 As Object = Me.LockSource
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockSource3)
                                SyncLock lockSource3
                                    Me.SourcesOnce.Clear()
                                    Try
                                        For Each netSource2 As ModNet.NetSource In Me.Sources
                                            Me.SourcesOnce.Add(netSource2)
                                            netSource2.IsFailed = True
                                        Next
                                    Finally
                                        Dim enumerator2 As List(Of ModNet.NetSource).Enumerator
                                        CType(enumerator2, IDisposable).Dispose()
                                    End Try
                                End SyncLock
                                Me.Reset()
                                Dim lockState As Object = Me.LockState
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                                SyncLock lockState
                                    Me.State = ModNet.NetState.WaitingToDownload
                                    GoTo IL_04AE
                                End SyncLock
                            End If
                            If Me.HasAvailableSource(True) AndAlso IsMergeFailure Then
                                Me.Reset()
                                Dim lockState2 As Object = Me.LockState
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState2)
                                SyncLock lockState2
                                    Me.State = ModNet.NetState.WaitingToDownload
                                    GoTo IL_04AE
                                End SyncLock
                            End If
                            Logger.Info(String.Format("{0}：已无可用下载源，下载失败", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            Dim ex2 As Exception = Nothing
                            Dim lockSource4 As Object = Me.LockSource
                            ObjectFlowControl.CheckForSyncLockOnValueType(lockSource4)
                            SyncLock lockSource4
                                Try
                                    For Each netSource3 As ModNet.NetSource In Me.Sources
                                        Logger.Info(String.Format("已禁用的下载源：{0}", netSource3.Url), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                                        If netSource3.Ex IsNot Nothing Then
                                            ex2 = netSource3.Ex
                                            Logger.Warn(netSource3.Ex, "下载源禁用原因", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                                        End If
                                    Next
                                Finally
                                    Dim enumerator3 As List(Of ModNet.NetSource).Enumerator
                                    CType(enumerator3, IDisposable).Dispose()
                                End Try
                            End SyncLock
                            Me.Fail(ex2)
                        End If
                    End If
                End If
                IL_04AE:
                If Me.FileSize = -2L Then
                    Me.Reset()
                End If
            End Sub

            Private Sub Reset()
                Me.FileSize = -2L
                Dim cache As MemoryStream = Me.Cache
                If cache IsNot Nothing Then
                    cache.Dispose()
                End If
                Me.Cache = Nothing
                Dim lockChain As Object = Me.LockChain
                ObjectFlowControl.CheckForSyncLockOnValueType(lockChain)
                SyncLock lockChain
                    Me.Threads = Nothing
                End SyncLock
                Interlocked.Add(ModNet.NetManager.DownloadDone, 0L - Me.DownloadDone)
                Me.DownloadDone = 0L
                Me.SpeedLastDone = 0L
            End Sub

            Private Sub Merge(Th As ModNet.NetThread)
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If Me.State >= ModNet.NetState.Merging Then
                        Return
                    End If
                    Me.State = ModNet.NetState.Merging
                End SyncLock
                Dim num As Integer = 0
                Try
                    IL_003A:
                    DirectoryUtils.Create(PathUtils.RemoveLastPart(Me.LocalPath))
                    FileUtils.Delete(Me.LocalPath, False)
                    Dim lockChain As Object = Me.LockChain
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockChain)
                    SyncLock lockChain
                        If Me.IsNoSplit Then
                            Logger.Trace(Function() String.Format("{0}：下载结束，从内存输出文件", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            FileUtils.Write(Me.LocalPath, Me.Cache)
                        ElseIf Me.Threads.DownloadDone = Me.DownloadDone AndAlso Me.Threads.Temp IsNot Nothing Then
                            Logger.Trace(Function() String.Format("{0}：下载结束，仅有一个文件，无需合并", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            FileUtils.Copy(Me.Threads.Temp, Me.LocalPath)
                        Else
                            Logger.Trace(Function() String.Format("{0}：下载结束，开始合并文件", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            Using fileStream As FileStream = FileUtils.CreateAsStream(Me.LocalPath)
                                Using binaryWriter As BinaryWriter = New BinaryWriter(fileStream)
                                    Try
                                        For Each netThread As ModNet.NetThread In Me.Threads
                                            If netThread.DownloadDone <> 0L AndAlso netThread.Temp IsNot Nothing Then
                                                Using stream As Stream = FileUtils.ReadAsStream(netThread.Temp, Nothing)
                                                    Using binaryReader As BinaryReader = New BinaryReader(stream)
                                                        binaryWriter.Write(binaryReader.ReadBytes(CInt(netThread.DownloadDone)))
                                                    End Using
                                                End Using
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator As IEnumerator(Of ModNet.NetThread)
                                        If enumerator IsNot Nothing Then
                                            enumerator.Dispose()
                                        End If
                                    End Try
                                End Using
                            End Using
                        End If
                        Dim check As FileChecker = Me.Check
                        Dim text As String = If((check IsNot Nothing), check.Check(Me.LocalPath), Nothing)
                        If text Is Nothing AndAlso Not Me.IsUnknownSize AndAlso Me.Check IsNot Nothing Then
                            If Me.Check.ActualSize = -1L Then
                                text = New FileChecker() With { .ActualSize = Me.FileSize }.Check(Me.LocalPath)
                            ElseIf Me.Check.ActualSize <> Me.FileSize Then
                                text = String.Format("文件大小不一致：任务校验要求为 {0}，请求结果为 {1}", Me.Check.ActualSize, Me.FileSize)
                            End If
                        End If
                        If text IsNot Nothing Then
                            Logger.Info(String.Format("{0} 文件校验失败，下载线程细节：", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            Try
                                For Each netThread2 As ModNet.NetThread In Me.Threads
                                    Logger.Info(String.Format("- {0}#，状态 {1}，范围 {2}~{3}，已下载 {4}，未下载 {5}", New Object() { netThread2.Uuid, netThread2.State, netThread2.DownloadStart, netThread2.DownloadEnd, netThread2.DownloadDone, netThread2.DownloadUndone }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                                Next
                            Finally
                                Dim enumerator2 As IEnumerator(Of ModNet.NetThread)
                                If enumerator2 IsNot Nothing Then
                                    enumerator2.Dispose()
                                End If
                            End Try
                            Throw New Exception(text)
                        End If
                        If Not Me.IsNoSplit Then
                            Try
                                For Each netThread3 As ModNet.NetThread In Me.Threads
                                    If netThread3.Temp IsNot Nothing Then
                                        FileUtils.Delete(netThread3.Temp, False)
                                    End If
                                Next
                            Finally
                                Dim enumerator3 As IEnumerator(Of ModNet.NetThread)
                                If enumerator3 IsNot Nothing Then
                                    enumerator3.Dispose()
                                End If
                            End Try
                        End If
                        Me.Finish(True)
                    End SyncLock
                Catch ex As Exception
                    num += 1
                    If Me.State <= ModNet.NetState.Merging Then
                        Logger.Warn(ex, String.Format("合并文件出错，第 {0} 次尝试（{1}）", num, Me.LocalName), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        If num < 3 Then
                            Global.System.Threading.Thread.Sleep(500 * num)
                            GoTo IL_003A
                        End If
                        Try
                            FileUtils.Delete(Me.LocalPath, False)
                        Catch ex2 As Exception
                            Logger.Warn(ex, String.Format("删除合并失败的文件出错（{0}）", Me.LocalPath), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        End Try
                        Me.SourceFail(Th, ex, True)
                    End If
                Finally
                    Dim cache As MemoryStream = Me.Cache
                    If cache IsNot Nothing Then
                        cache.Dispose()
                    End If
                End Try
            End Sub

            Private Sub Fail(Optional RaiseEx As Exception = Nothing)
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If Me.State >= ModNet.NetState.Finished Then
                        Return
                    End If
                    If RaiseEx IsNot Nothing Then
                        Me.Ex.Add(RaiseEx)
                    End If
                    Logger.Info(String.Format("{0}：已失败，当前状态 {1}", Me.LocalName, Me.State), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                    Me.State = ModNet.NetState.Canceled
                End SyncLock
                Me.CancelInternal()
                Try
                    For Each loaderDownload As ModNet.LoaderDownload In Me.Tasks
                        loaderDownload.OnFileFail(Me)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModNet.LoaderDownload)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            End Sub

            Public Sub Cancel(CanceledTask As ModNet.LoaderDownload)
                Me.Tasks.Remove(CanceledTask)
                If Not Me.Tasks.Any() Then
                    Dim lockState As Object = Me.LockState
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                    SyncLock lockState
                        If Me.State >= ModNet.NetState.Finished Then
                            Return
                        End If
                        Logger.Info(String.Format("{0}：已中断，当前状态 {1}", Me.LocalName, Me.State), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        Me.State = ModNet.NetState.Canceled
                    End SyncLock
                    Me.CancelInternal()
                End If
            End Sub

            Private Sub CancelInternal()
                Dim num As Integer
                Dim num4 As Integer
                Dim obj As Object
                Try
                    IL_0000:
                    ProjectData.ClearProjectError()
                    num = 1
                    IL_0007:
                    Dim num2 As Integer = 2
                    Me.Reset()
                    IL_000F:
                    num2 = 3
                    Interlocked.Decrement(ModNet.NetManager.FileRemain)
                    IL_0021:
                    GoTo IL_0080
                    IL_0023:
                    Dim num3 As Integer = num4 + 1
                    num4 = 0
                    switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                    IL_0041:
                    GoTo IL_0075
                    IL_0043:
                    num4 = num2
                    switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                    IL_0053:
                Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                    Dim ex As Exception = CType(obj2, Exception)
                    GoTo IL_0043
                End Try
                IL_0075:
                Throw ProjectData.CreateProjectError(-2146828237)
                IL_0080:
                If num4 <> 0 Then
                    ProjectData.ClearProjectError()
                End If
            End Sub

            Public Sub Finish(Optional PrintLog As Boolean = True)
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If Me.State >= ModNet.NetState.Finished Then
                        Return
                    End If
                    Me.State = ModNet.NetState.Finished
                End SyncLock
                Interlocked.Decrement(ModNet.NetManager.FileRemain)
                If PrintLog Then
                    Logger.Info(String.Format("{0}：已完成", Me.LocalName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                End If
                Try
                    For Each loaderDownload As ModNet.LoaderDownload In Me.Tasks
                        loaderDownload.OnFileFinish(Me)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModNet.LoaderDownload)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            End Sub

            Public Tasks As ConcurrentList(Of ModNet.LoaderDownload)

            Public Sources As List(Of ModNet.NetSource)

            Private FirstThreadSource As Integer

            Public SourcesOnce As ConcurrentList(Of ModNet.NetSource)

            Private Retried As Boolean

            Public LocalPath As String

            Public LocalName As String

            Public State As ModNet.NetState

            Public Ex As List(Of Exception)

            Public Threads As ModNet.NetThread

            Public FileSize As Long

            Public IsUnknownSize As Boolean

            Private Cache As MemoryStream

            Public DownloadDone As Long

            Public Check As FileChecker

            Public SimulateBrowserHeaders As Boolean

            Private SpeedLastTime As Long

            Private SpeedLastDone As Long

            Private _Speed As Long

            Public IsCopy As Boolean

            Private ConnectCount As Integer

            Private ConnectTime As Long

            Private Const FilePieceLimit As Long = 262144L

            Public LockState As Object

            Public LockChain As Object

            Public LockSource As Object

            Public Uuid As Integer
        End Class

        Private Class RangeNotSupportedException
            Inherits HttpRequestException

            Public Sub New(Message As String)
                MyBase.New(Message)
            End Sub
        End Class

        Public Class LoaderDownload
            Inherits ModLoader.LoaderBase

            Public Overrides Property Progress As Double
                Get
                    Dim num As Double
                    If MyBase.State >= ModBase.LoadState.Finished Then
                        num = 1.0
                    ElseIf Not Me.Files.Any() Then
                        num = 0.0
                    Else
                        num = Me._Progress
                    End If
                    Return num
                End Get
                Set(value As Double)
                    Throw New Exception("文件下载不允许指定进度")
                End Set
            End Property

            Public Property FailCount As Integer
                Get
                    Return Me._FailCount
                End Get
                Set(value As Integer)
                    Dim num2 As Integer
                    Dim num4 As Integer
                    Dim obj As Object
                    Try
                        IL_0000:
                        Dim num As Integer = 1
                        Me._FailCount = value
                        IL_0009:
                        num = 2
                        If MyBase.State <> ModBase.LoadState.Loading OrElse value < Math.Min(10000, Math.Max(Me.FileRemain * 8, ModNet.NetTaskThreadLimit * 8 + 3)) Then
                            GoTo IL_0106
                        End If
                        IL_003D:
                        num = 3
                        Logger.[Error](String.Format("由于同加载器中失败次数过多引发强制失败：连续失败了 {0} 次", value), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        IL_005A:
                        ProjectData.ClearProjectError()
                        num2 = 1
                        IL_0061:
                        num = 5
                        Dim list As List(Of Exception) = New List(Of Exception)()
                        IL_0069:
                        num = 6
                        Dim enumerator As IEnumerator(Of ModNet.NetFile) = Me.Files.GetEnumerator()
                        While enumerator.MoveNext()
                            Dim netFile As ModNet.NetFile = enumerator.Current
                            IL_0083:
                            num = 7
                            Dim enumerator2 As List(Of ModNet.NetSource).Enumerator = netFile.Sources.GetEnumerator()
                            While enumerator2.MoveNext()
                                Dim netSource As ModNet.NetSource = enumerator2.Current
                                IL_009E:
                                num = 8
                                If netSource.Ex IsNot Nothing Then
                                    IL_00A9:
                                    num = 9
                                    list.Add(netSource.Ex)
                                    IL_00B9:
                                    num = 10
                                    If list.Count > 10 Then
                                        GoTo IL_00FC
                                    End If
                                End If
                                IL_00C6:
                                num = 12
                            End While
                            IL_00D2:
                            num = 13
                            CType(enumerator2, IDisposable).Dispose()
                            IL_00E2:
                            num = 14
                            Continue While
                            IL_00FC:
                            num = 16
                            Me.OnFail(list)
                            GoTo IL_0106
                        End While
                        IL_00EE:
                        num = 15
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                            GoTo IL_00FC
                        End If
                        GoTo IL_00FC
                        IL_0106:
                        GoTo IL_019C
                        IL_010B:
                        Dim num3 As Integer = num4 + 1
                        num4 = 0
                        switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                        IL_015D:
                        GoTo IL_0191
                        IL_015F:
                        num4 = num
                        switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num2)
                        IL_016F:
                    Catch obj2 When endfilter((TypeOf obj Is Exception) And (num2 <> 0) And (num4 = 0))
                        Dim ex As Exception = CType(obj2, Exception)
                        GoTo IL_015F
                    End Try
                    IL_0191:
                    Throw ProjectData.CreateProjectError(-2146828237)
                    IL_019C:
                    If num4 <> 0 Then
                        ProjectData.ClearProjectError()
                    End If
                End Set
            End Property

            Public Sub RefreshStat()
                Dim num As Double = 0.0
                Dim num2 As Double = 0.0
                Try
                    For Each netFile As ModNet.NetFile In Me.Files
                        If netFile.IsCopy Then
                            num += netFile.Progress * 0.2
                            num2 += 0.2
                        Else
                            num += netFile.Progress
                            num2 += 1.0
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModNet.NetFile)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If num2 > 0.0 AndAlso Not Double.IsNaN(num2) Then
                    num /= num2
                End If
                Me._Progress = num
            End Sub

            Public Sub New(Name As String, FileTasks As IEnumerable(Of ModNet.NetFile))
                Me.FileRemainLock = RuntimeHelpers.GetObjectValue(New Object())
                Me._Progress = 0.0
                Me._FailCount = 0
                Me.Name = Name
                Me.Files = New ConcurrentQueue(Of ModNet.NetFile)(FileTasks)
            End Sub

            Public Overrides Sub Start(Optional Input As Object = Nothing, Optional IsForceRestart As Boolean = False)
                If Input IsNot Nothing Then
                    Me.Files = New ConcurrentQueue(Of ModNet.NetFile)(CType(Input, IEnumerable(Of ModNet.NetFile)))
                End If
                Dim files As IEnumerable(Of ModNet.NetFile) = Me.Files
                Dim func As Func(Of ModNet.NetFile, String)
                If ModNet.LoaderDownload._Closure$__.$I13-0 IsNot Nothing Then
                    func = ModNet.LoaderDownload._Closure$__.$I13-0
                Else
                    Dim func2 As Func(Of ModNet.NetFile, String) = Function(a As ModNet.NetFile) PathUtils.ForCompare(a.LocalPath).Lower()
                    func = func2
                    ModNet.LoaderDownload._Closure$__.$I13-0 = func2
                End If
                Me.Files = New ConcurrentQueue(Of ModNet.NetFile)(files.DistinctBy(func))
                Dim fileRemainLock As Object = Me.FileRemainLock
                ObjectFlowControl.CheckForSyncLockOnValueType(fileRemainLock)
                SyncLock fileRemainLock
                    Dim ptr As __ByRef(Of Integer) = Me.FileRemain
                    Dim num As Integer = ptr
                    Dim files2 As IEnumerable(Of ModNet.NetFile) = Me.Files
                    Dim func3 As Func(Of ModNet.NetFile, Boolean)
                    If ModNet.LoaderDownload._Closure$__.$I13-1 IsNot Nothing Then
                        func3 = ModNet.LoaderDownload._Closure$__.$I13-1
                    Else
                        Dim func4 As Func(Of ModNet.NetFile, Boolean) = Function(f As ModNet.NetFile) f.State <> ModNet.NetState.Finished
                        func3 = func4
                        ModNet.LoaderDownload._Closure$__.$I13-1 = func4
                    End If
                    Me.FileRemain = num + files2.Where(func3).Count()
                End SyncLock
                MyBase.State = ModBase.LoadState.Loading
                ModBase.RunInNewThread(Sub()
                    Try
                        Dim CS$<>8__locals1 As ModNet.LoaderDownload._Closure$__13-1 = New ModNet.LoaderDownload._Closure$__13-1(CS$<>8__locals1)
                        CS$<>8__locals1.$VB$Me = Me
                        If Not Me.Files.Any() Then
                            Me.OnFinish()
                        Else
                            Try
                                For Each netFile As ModNet.NetFile In Me.Files
                                    If netFile Is Nothing Then
                                        Throw New ArgumentException("存在空文件请求！")
                                    End If
                                    Try
                                        For Each netSource As ModNet.NetSource In netFile.Sources
                                            If Not netSource.Url.StartsWithF("https://", True) AndAlso Not netSource.Url.StartsWithF("http://", True) Then
                                                netSource.Ex = New ArgumentException("输入的下载链接不正确：" + netSource.Url)
                                                netSource.IsFailed = True
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator2 As List(Of ModNet.NetSource).Enumerator
                                        CType(enumerator2, IDisposable).Dispose()
                                    End Try
                                    If Not netFile.HasAvailableSource(True) Then
                                        Throw New ArgumentException("输入的下载链接不正确！")
                                    End If
                                    netFile.LocalPath = netFile.LocalPath.Replace("/", "\")
                                    If Not netFile.LocalPath.Contains(":\") Then
                                        Throw New ArgumentException("输入的本地文件地址不正确：" + netFile.LocalPath)
                                    End If
                                    If netFile.LocalPath.EndsWithF("\", False) Then
                                        Throw New ArgumentException("请输入含文件名的完整文件路径：" + netFile.LocalPath)
                                    End If
                                    DirectoryUtils.Create(PathUtils.RemoveLastPart(netFile.LocalPath))
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of ModNet.NetFile)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                            Dim list As List(Of ModNet.NetFile) = ModNet.NetManager.Start(Me)
                            Dim list2 As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                            Try
                                For Each netFile2 As ModNet.NetFile In list
                                    If netFile2.Check IsNot Nothing Then
                                        list2.Add(netFile2)
                                    Else
                                        Dim lockState As Object = Me.LockState
                                        ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                                        SyncLock lockState
                                            netFile2.State = ModNet.NetState.WaitingToDownload
                                            netFile2.IsCopy = False
                                        End SyncLock
                                    End If
                                Next
                            Finally
                                Dim enumerator3 As List(Of ModNet.NetFile).Enumerator
                                CType(enumerator3, IDisposable).Dispose()
                            End Try
                            If list2.Any() Then
                                CS$<>8__locals1.$VB$Local_Folders = New List(Of String)()
                                CS$<>8__locals1.$VB$Local_Folders.Add(Paths.AppData + ".minecraft\")
                                Dim $VB$Local_Folders As List(Of String) = CS$<>8__locals1.$VB$Local_Folders
                                Dim mcFolderList As IEnumerable(Of ModMinecraft.McFolder) = ModMinecraft.McFolderList
                                Dim func5 As Func(Of ModMinecraft.McFolder, String)
                                If ModNet.LoaderDownload._Closure$__.$I13-3 IsNot Nothing Then
                                    func5 = ModNet.LoaderDownload._Closure$__.$I13-3
                                Else
                                    Dim func6 As Func(Of ModMinecraft.McFolder, String) = Function(f As ModMinecraft.McFolder) f.Location
                                    func5 = func6
                                    ModNet.LoaderDownload._Closure$__.$I13-3 = func6
                                End If
                                $VB$Local_Folders.AddRange(mcFolderList.[Select](func5))
                                Dim CS$<>8__locals2 As ModNet.LoaderDownload._Closure$__13-1 = CS$<>8__locals1
                                Dim enumerable As IEnumerable(Of String) = CS$<>8__locals1.$VB$Local_Folders.Distinct()
                                Dim func7 As Func(Of String, Boolean)
                                If ModNet.LoaderDownload._Closure$__.$I13-4 IsNot Nothing Then
                                    func7 = ModNet.LoaderDownload._Closure$__.$I13-4
                                Else
                                    Dim func8 As Func(Of String, Boolean) = Function(f As String) DirectoryUtils.Exists(f)
                                    func7 = func8
                                    ModNet.LoaderDownload._Closure$__.$I13-4 = func8
                                End If
                                CS$<>8__locals2.$VB$Local_Folders = enumerable.Where(func7).ToList()
                                Dim num2 As Integer = (list2.Count / 40).Clamp(1, 8)
                                If num2 = 1 Then
                                    Me.CheckExistingFiles(list2, CS$<>8__locals1.$VB$Local_Folders)
                                Else
                                    Dim num3 As Integer = list2.Count / num2
                                    Dim num4 As Integer = list2.Count Mod num2
                                    Dim num5 As Integer = 0
                                    Dim num6 As Integer = num2 - 1
                                    For i As Integer = 0 To num6
                                        Dim CS$<>8__locals3 As ModNet.LoaderDownload._Closure$__13-0 = New ModNet.LoaderDownload._Closure$__13-0(CS$<>8__locals3)
                                        CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                                        Dim num7 As Integer = num3 + If((i < num4), 1, 0)
                                        CS$<>8__locals3.$VB$Local_ThreadFiles = list2.GetRange(num5, num7)
                                        num5 += num7
                                        ModBase.RunInNewThread(Sub()
                                            CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Me.CheckExistingFiles(CS$<>8__locals3.$VB$Local_ThreadFiles, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_Folders)
                                        End Sub, String.Format("下载 文件复制 {0}/{1}", Me.Uuid, ModBase.GetUuid()), ThreadPriority.Normal)
                                    Next
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        Me.OnFail(New List(Of Exception)() From { New Exception("下载初始化失败", ex) })
                    End Try
                End Sub, "L/下载 " + Conversions.ToString(Me.Uuid), ThreadPriority.Normal)
            End Sub

            Private Sub CheckExistingFiles(Files As List(Of ModNet.NetFile), FolderList As List(Of String))
                Dim CS$<>8__locals1 As ModNet.LoaderDownload._Closure$__14-0 = New ModNet.LoaderDownload._Closure$__14-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Local_Files = Files
                Try
                    Logger.Trace(Function()
                        Dim text3 As String = "文件检查开始，本线程负责 {0} 个文件，首个文件为 {1}"
                        Dim obj As Object = CS$<>8__locals1.$VB$Local_Files.Count
                        Dim netFile As ModNet.NetFile = CS$<>8__locals1.$VB$Local_Files.FirstOrDefault()
                        Return String.Format(text3, obj, If((netFile IsNot Nothing), netFile.LocalName, Nothing))
                    End Function, LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                    Dim list As List(Of String) = New List(Of String)()
                    Try
                        For Each text As String In FolderList
                            Try
                                For Each text2 As String In DirectoryUtils.EnumerateDirectories(text + "versions\", False, "*")
                                    list.Add(PathUtils.AddSlashSuffix(text2))
                                Next
                            Finally
                                Dim enumerator2 As IEnumerator(Of String)
                                If enumerator2 IsNot Nothing Then
                                    enumerator2.Dispose()
                                End If
                            End Try
                        Next
                    Finally
                        Dim enumerator As List(Of String).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Try
                        Dim enumerator3 As List(Of ModNet.NetFile).Enumerator = CS$<>8__locals1.$VB$Local_Files.GetEnumerator()
                        While enumerator3.MoveNext()
                            Dim CS$<>8__locals2 As ModNet.LoaderDownload._Closure$__14-1 = New ModNet.LoaderDownload._Closure$__14-1(CS$<>8__locals2)
                            CS$<>8__locals2.$VB$Local_File = enumerator3.Current
                            Dim CS$<>8__locals3 As ModNet.LoaderDownload._Closure$__14-2 = New ModNet.LoaderDownload._Closure$__14-2(CS$<>8__locals3)
                            CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals2
                            CS$<>8__locals3.$VB$Local_Target = Me.CheckExistingFile(FolderList, list, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File)
                            Logger.Trace(Function() String.Format("本地文件匹配：{0} → {1}，当前状态 {2}", CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.LocalName, If(CS$<>8__locals3.$VB$Local_Target, "无匹配"), CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.State), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            If CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.State < ModNet.NetState.WaitingToDownload Then
                                If CS$<>8__locals3.$VB$Local_Target IsNot Nothing Then
                                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.IsCopy = True
                                    Try
                                        FileUtils.Copy(CS$<>8__locals3.$VB$Local_Target, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.LocalPath)
                                        CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.Finish(False)
                                        Continue While
                                    Catch ex As Exception
                                    End Try
                                End If
                                Dim lockState As Object = Me.LockState
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                                SyncLock lockState
                                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.State = ModNet.NetState.WaitingToDownload
                                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.IsCopy = False
                                End SyncLock
                            End If
                        End While
                    Finally
                        Dim enumerator3 As List(Of ModNet.NetFile).Enumerator
                        CType(enumerator3, IDisposable).Dispose()
                    End Try
                Catch ex2 As Exception
                    Me.OnFail(New List(Of Exception)() From { New Exception("下载已存在文件查找失败", ex2) })
                End Try
            End Sub

            Private Function CheckExistingFile(FolderList As List(Of String), VersionFolders As List(Of String), File As ModNet.NetFile) As String
                Dim CS$<>8__locals1 As ModNet.LoaderDownload._Closure$__15-0 = New ModNet.LoaderDownload._Closure$__15-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Local_File = File
                Dim text As String
                If CS$<>8__locals1.$VB$Local_File.Check.Check(CS$<>8__locals1.$VB$Local_File.LocalPath) Is Nothing Then
                    text = CS$<>8__locals1.$VB$Local_File.LocalPath
                ElseIf Settings.[Get](Of Boolean)("SystemDebugSkipCopy", Nothing) Then
                    text = Nothing
                ElseIf CS$<>8__locals1.$VB$Local_File.Check.Hash Is Nothing AndAlso CS$<>8__locals1.$VB$Local_File.Check.ActualSize < 0L Then
                    text = Nothing
                Else
                    Dim enumerable As IEnumerable(Of ValueTuple(Of String, Integer)) = New String() { "\assets\", "\libraries\", "\versions\", "\mods\", "\coremods\", "\lib\", "\resourcepacks\", "\texturepacks\", "\shaderpacks\" }.[Select](Function(FolderName As String) New ValueTuple(Of String, Integer)(FolderName, CS$<>8__locals1.$VB$Local_File.LocalPath.IndexOfF(FolderName, True)))
                    Dim func As Func(Of ValueTuple(Of String, Integer), Boolean)
                    If ModNet.LoaderDownload._Closure$__.$I15-1 IsNot Nothing Then
                        func = ModNet.LoaderDownload._Closure$__.$I15-1
                    Else
                        Dim func2 As Func(Of ValueTuple(Of String, Integer), Boolean) = Function(<TupleElementNames(New String() { "FolderName", Nothing })> kv As ValueTuple(Of String, Integer)) kv.Item2 >= 0
                        func = func2
                        ModNet.LoaderDownload._Closure$__.$I15-1 = func2
                    End If
                    Dim list As List(Of ValueTuple(Of String, Integer)) = enumerable.Where(func).ToList()
                    If Not list.Any() Then
                        If Operators.CompareString(PathUtils.GetExtension(CS$<>8__locals1.$VB$Local_File.LocalName), "jar", False) <> 0 Then
                            Return Nothing
                        End If
                        list.Add(New ValueTuple(Of String, Integer)("\versions\", 1))
                    End If
                    Dim enumerable2 As IEnumerable(Of ValueTuple(Of String, Integer)) = list
                    Dim func3 As Func(Of ValueTuple(Of String, Integer), Integer)
                    If ModNet.LoaderDownload._Closure$__.$I15-2 IsNot Nothing Then
                        func3 = ModNet.LoaderDownload._Closure$__.$I15-2
                    Else
                        Dim func4 As Func(Of ValueTuple(Of String, Integer), Integer) = Function(<TupleElementNames(New String() { "FolderName", Nothing })> kv As ValueTuple(Of String, Integer)) kv.Item2
                        func3 = func4
                        ModNet.LoaderDownload._Closure$__.$I15-2 = func4
                    End If
                    Dim text2 As String = enumerable2.MaxBy(func3).Item1.TrimStart(New Char() { "\"c })
                    If Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init Is Nothing Then
                        Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init, New StaticLocalInitFlag(), Nothing)
                    End If
                    Dim flag As Boolean = False
                    Try
                        Monitor.Enter(Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init, flag)
                        If Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State = 0S Then
                            Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State = 2S
                            Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes = New ConcurrentDictionary(Of String, Long)()
                        ElseIf Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State = 2S Then
                            Throw New IncompleteInitialization()
                        End If
                    Finally
                        Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State = 1S
                        If flag Then
                            Monitor.[Exit](Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init)
                        End If
                    End Try
                    If Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init Is Nothing Then
                        Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init, New StaticLocalInitFlag(), Nothing)
                    End If
                    Dim flag2 As Boolean = False
                    Try
                        Monitor.Enter(Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init, flag2)
                        If Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State = 0S Then
                            Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State = 2S
                            Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate = Function(Candidate As String, Checker As FileChecker)
                                Dim concurrentDictionary As ConcurrentDictionary(Of String, Long) = Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes
                                Dim func5 As Func(Of String, Long)
                                If ModNet.LoaderDownload._Closure$__.$I15-4 IsNot Nothing Then
                                    func5 = ModNet.LoaderDownload._Closure$__.$I15-4
                                Else
                                    Dim func6 As Func(Of String, Long) = Function(c As String)
                                        If Not FileUtils.Exists(c) Then
                                            Return -1L
                                        End If
                                        Return FileUtils.GetInfo(c).Length
                                    End Function
                                    func5 = func6
                                    ModNet.LoaderDownload._Closure$__.$I15-4 = func6
                                End If
                                Dim orAdd As Long = concurrentDictionary.GetOrAdd(Candidate, func5)
                                Return Checker.ActualSize = orAdd AndAlso Checker.Check(Candidate) = Nothing
                            End Function
                        ElseIf Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State = 2S Then
                            Throw New IncompleteInitialization()
                        End If
                    Finally
                        Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State = 1S
                        If flag2 Then
                            Monitor.[Exit](Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init)
                        End If
                    End Try
                    If Operators.CompareString(text2, "assets\", False) <> 0 AndAlso Operators.CompareString(text2, "libraries\", False) <> 0 Then
                        If Operators.CompareString(text2, "versions\", False) = 0 Then
                            Try
                                For Each text3 As String In VersionFolders
                                    Try
                                        For Each text4 As String In DirectoryUtils.EnumerateFiles(text3, False, "*." + PathUtils.GetExtension(CS$<>8__locals1.$VB$Local_File.LocalPath))
                                            If Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate(text4, CS$<>8__locals1.$VB$Local_File.Check) Then
                                                Return text4
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator2 As IEnumerator(Of String)
                                        If enumerator2 IsNot Nothing Then
                                            enumerator2.Dispose()
                                        End If
                                    End Try
                                Next
                                GoTo IL_0489
                            Finally
                                Dim enumerator As List(Of String).Enumerator
                                CType(enumerator, IDisposable).Dispose()
                            End Try
                        End If
                        If CS$<>8__locals1.$VB$Local_File.Check.ActualSize < 0L OrElse CS$<>8__locals1.$VB$Local_File.Check.Hash Is Nothing Then
                            Return Nothing
                        End If
                        Try
                            For Each text5 As String In FolderList.Concat(VersionFolders)
                                Try
                                    For Each text6 As String In DirectoryUtils.EnumerateFiles(text5 + text2, False, "*")
                                        If Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate(text6, CS$<>8__locals1.$VB$Local_File.Check) Then
                                            Return text6
                                        End If
                                    Next
                                Finally
                                    Dim enumerator4 As IEnumerator(Of String)
                                    If enumerator4 IsNot Nothing Then
                                        enumerator4.Dispose()
                                    End If
                                End Try
                            Next
                        Finally
                            Dim enumerator3 As IEnumerator(Of String)
                            If enumerator3 IsNot Nothing Then
                                enumerator3.Dispose()
                            End If
                        End Try
                        IL_0489:
                        text = Nothing
                    Else
                        Try
                            For Each text7 As String In FolderList
                                Dim text8 As String = text7 + text2 + CS$<>8__locals1.$VB$Local_File.LocalPath.AfterFirst(text2, False)
                                If Me.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate(text8, CS$<>8__locals1.$VB$Local_File.Check) Then
                                    Return text8
                                End If
                            Next
                        Finally
                            Dim enumerator5 As List(Of String).Enumerator
                            CType(enumerator5, IDisposable).Dispose()
                        End Try
                        text = Nothing
                    End If
                End If
                Return text
            End Function

            Public Sub OnFileFinish(File As ModNet.NetFile)
                Dim fileRemainLock As Object = Me.FileRemainLock
                ObjectFlowControl.CheckForSyncLockOnValueType(fileRemainLock)
                SyncLock fileRemainLock
                    Dim ptr As __ByRef(Of Integer) = Me.FileRemain
                    Me.FileRemain = ptr - 1
                    If Me.FileRemain > 0 Then
                        Return
                    End If
                End SyncLock
                Me.OnFinish()
            End Sub

            Public Sub OnFinish()
                MyBase.RaisePreviewFinish()
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State <= ModBase.LoadState.Loading Then
                        MyBase.State = ModBase.LoadState.Finished
                    End If
                End SyncLock
            End Sub

            Public Sub OnFileFail(File As ModNet.NetFile)
                Try
                    For Each netSource As ModNet.NetSource In File.Sources
                        If Not Information.IsNothing(netSource.Ex) Then
                            File.Ex.Add(netSource.Ex)
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of ModNet.NetSource).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                Me.OnFail(File.Ex)
            End Sub

            Public Overrides Sub Failed(Ex As Exception)
                Me.OnFail(New List(Of Exception)() From { Ex })
            End Sub

            Public Sub OnFail(ExList As List(Of Exception))
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State >= ModBase.LoadState.Finished Then
                        Return
                    End If
                    If ExList Is Nothing OrElse Not ExList.Any() Then
                        ExList = New List(Of Exception)() From { New Exception("未知错误！") }
                    End If
                    Dim enumerable As IEnumerable(Of Exception) = ExList
                    Dim func As Func(Of Exception, Boolean)
                    If ModNet.LoaderDownload._Closure$__.$I20-0 IsNot Nothing Then
                        func = ModNet.LoaderDownload._Closure$__.$I20-0
                    Else
                        Dim func2 As Func(Of Exception, Boolean) = Function(e As Exception) Not e.IsCanceled() AndAlso Not(TypeOf e Is TimeoutException)
                        func = func2
                        ModNet.LoaderDownload._Closure$__.$I20-0 = func2
                    End If
                    Dim list As List(Of Exception) = enumerable.Where(func).ToList()
                    MyBase.[Error] = If(list.FirstOrDefault(), ExList.FirstOrDefault())
                    Try
                        For Each netFile As ModNet.NetFile In Me.Files
                            If netFile.State = ModNet.NetState.Canceled Then
                                Dim sources As IEnumerable(Of ModNet.NetSource) = netFile.Sources
                                Dim func3 As Func(Of ModNet.NetSource, Boolean)
                                If ModNet.LoaderDownload._Closure$__.$I20-1 IsNot Nothing Then
                                    func3 = ModNet.LoaderDownload._Closure$__.$I20-1
                                Else
                                    Dim func4 As Func(Of ModNet.NetSource, Boolean) = Function(s As ModNet.NetSource) s.Ex.IsCanceled() OrElse TypeOf s.Ex Is TimeoutException
                                    func3 = func4
                                    ModNet.LoaderDownload._Closure$__.$I20-1 = func4
                                End If
                                If Not sources.All(func3) Then
                                    Dim sources2 As IEnumerable(Of ModNet.NetSource) = netFile.Sources
                                    Dim func5 As Func(Of ModNet.NetSource, String)
                                    If ModNet.LoaderDownload._Closure$__.$I20-2 IsNot Nothing Then
                                        func5 = ModNet.LoaderDownload._Closure$__.$I20-2
                                    Else
                                        Dim func6 As Func(Of ModNet.NetSource, String) = Function(s As ModNet.NetSource) String.Format("{0}（{1}）", If((s.Ex Is Nothing), "无错误信息。", s.Ex.GetDisplay(False)), s.Url)
                                        func5 = func6
                                        ModNet.LoaderDownload._Closure$__.$I20-2 = func6
                                    End If
                                    Dim text As String = sources2.[Select](func5).Join(vbCrLf)
                                    MyBase.[Error] = New Exception("文件下载失败：" + netFile.LocalPath + vbCrLf & "各下载源的错误如下：" & vbCrLf + text, MyBase.[Error])
                                    ModMain.Telemetry("文件下载失败", New String() { "FileName", netFile.LocalName, "Exception", text })
                                    Exit For
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of ModNet.NetFile)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    MyBase.State = ModBase.LoadState.Failed
                End SyncLock
                Try
                    For Each netFile2 As ModNet.NetFile In Me.Files
                        netFile2.Cancel(Me)
                    Next
                Finally
                    Dim enumerator2 As IEnumerator(Of ModNet.NetFile)
                    If enumerator2 IsNot Nothing Then
                        enumerator2.Dispose()
                    End If
                End Try
                Logger.[Error](New AggregateException(ExList), "下载加载器失败", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
            End Sub

            Public Overrides Sub Cancel()
                Dim lockState As Object = Me.LockState
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState)
                SyncLock lockState
                    If MyBase.State >= ModBase.LoadState.Finished Then
                        Return
                    End If
                    MyBase.State = ModBase.LoadState.Canceled
                End SyncLock
                Logger.Info(String.Format("{0} 已取消！", Me.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                Try
                    For Each netFile As ModNet.NetFile In Me.Files
                        netFile.Cancel(Me)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModNet.NetFile)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            End Sub

            Public Files As ConcurrentQueue(Of ModNet.NetFile)

            Private FileRemain As Integer

            Private FileRemainLock As Object

            Private _Progress As Double

            Private _FailCount As Integer

            Private $STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes As ConcurrentDictionary(Of String, Long)

            Private $STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init As StaticLocalInitFlag

            Private $STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate As Func(Of String, FileChecker, Boolean)

            Private $STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init As StaticLocalInitFlag
        End Class

        Public Class NetManagerClass
            Public Sub New()
                Me.AllFiles = New ConcurrentDictionary(Of String, ModNet.NetFile)()
                Me.Tasks = New ConcurrentBag(Of ModNet.LoaderDownload)()
                Me.DownloadDone = 0L
                Me.FileRemain = 0
                Me.Speed = 0L
                Me.Uuid = ModBase.GetUuid()
                Me.DownloadCacheLock = RuntimeHelpers.GetObjectValue(New Object())
                Me.IsDownloadCacheCleared = False
            End Sub

            Private Sub RefreshStat()
                ' The following expression was wrapped in a checked-statement
                Try
                    Dim num As Long = ModBase.GetTimeMs() - Me.RefreshStatLast
                    If num <> 0L Then
                        Dim ptr As __ByRef(Of Long) = Me.RefreshStatLast
                        Me.RefreshStatLast = ptr + num
                        If Me.$STATIC$RefreshStat$2001$SpeedLast$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$RefreshStat$2001$SpeedLast$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag As Boolean = False
                        Try
                            Monitor.Enter(Me.$STATIC$RefreshStat$2001$SpeedLast$Init, flag)
                            If Me.$STATIC$RefreshStat$2001$SpeedLast$Init.State = 0S Then
                                Me.$STATIC$RefreshStat$2001$SpeedLast$Init.State = 2S
                                Me.$STATIC$RefreshStat$2001$SpeedLast = New List(Of Long)()
                            ElseIf Me.$STATIC$RefreshStat$2001$SpeedLast$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            Me.$STATIC$RefreshStat$2001$SpeedLast$Init.State = 1S
                            If flag Then
                                Monitor.[Exit](Me.$STATIC$RefreshStat$2001$SpeedLast$Init)
                            End If
                        End Try
                        If Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag2 As Boolean = False
                        Try
                            Monitor.Enter(Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init, flag2)
                            If Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State = 0S Then
                                Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State = 2S
                                Me.$STATIC$RefreshStat$2001$SpeedLastDone = 0L
                            ElseIf Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State = 1S
                            If flag2 Then
                                Monitor.[Exit](Me.$STATIC$RefreshStat$2001$SpeedLastDone$Init)
                            End If
                        End Try
                        Dim num2 As Double = Math.Max(0.0, CDbl((Me.DownloadDone - Me.$STATIC$RefreshStat$2001$SpeedLastDone)) / (CDbl(num) / 1000.0))
                        Me.$STATIC$RefreshStat$2001$SpeedLast.Insert(0, CLng(Math.Round(num2)))
                        If Me.$STATIC$RefreshStat$2001$SpeedLast.Count >= 31 Then
                            Me.$STATIC$RefreshStat$2001$SpeedLast.RemoveAt(30)
                        End If
                        Me.$STATIC$RefreshStat$2001$SpeedLastDone = Me.DownloadDone
                        Dim num3 As Long = 0L
                        Dim num4 As Long = 0L
                        Dim num5 As Integer = Me.$STATIC$RefreshStat$2001$SpeedLast.Count
                        Try
                            For Each num6 As Long In Me.$STATIC$RefreshStat$2001$SpeedLast
                                ' The following expression was wrapped in a unchecked-expression
                                num3 += num6 * CLng(num5)
                                num4 += CLng(num5)
                                num5 -= 1
                            Next
                        Finally
                            Dim enumerator As List(Of Long).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                        Me.Speed = CLng(Math.Round(If((num4 > 0L), (CDbl(num3) / CDbl(num4)), 0.0)))
                        Dim num7 As Long = 0L
                        If Me.$STATIC$RefreshStat$2001$SpeedLast.Count >= 10 Then
                            ' The following expression was wrapped in a unchecked-expression
                            num7 = CLng(Math.Round(Me.$STATIC$RefreshStat$2001$SpeedLast.Take(10).Average() * 0.85))
                        End If
                        If num7 > ModNet.NetTaskSpeedLimitLow Then
                            ModNet.NetTaskSpeedLimitLow = num7
                            Logger.Info(String.Format("速度下限已提升到 {0}", StringUtils.FormatByteSize(num7)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        End If
                        Try
                            For Each loaderDownload As ModNet.LoaderDownload In Me.Tasks
                                loaderDownload.RefreshStat()
                            Next
                        Finally
                            Dim enumerator2 As IEnumerator(Of ModNet.LoaderDownload)
                            If enumerator2 IsNot Nothing Then
                                enumerator2.Dispose()
                            End If
                        End Try
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, "刷新下载公开属性失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                End Try
            End Sub

            Private Sub StartManager()
                Dim CS$<>8__locals1 As ModNet.NetManagerClass._Closure$__9-0 = New ModNet.NetManagerClass._Closure$__9-0(CS$<>8__locals1)
                If Me.$STATIC$StartManager$2001$IsStarted$Init Is Nothing Then
                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$StartManager$2001$IsStarted$Init, New StaticLocalInitFlag(), Nothing)
                End If
                Dim flag As Boolean = False
                Try
                    Monitor.Enter(Me.$STATIC$StartManager$2001$IsStarted$Init, flag)
                    If Me.$STATIC$StartManager$2001$IsStarted$Init.State = 0S Then
                        Me.$STATIC$StartManager$2001$IsStarted$Init.State = 2S
                        Me.$STATIC$StartManager$2001$IsStarted = False
                    ElseIf Me.$STATIC$StartManager$2001$IsStarted$Init.State = 2S Then
                        Throw New IncompleteInitialization()
                    End If
                Finally
                    Me.$STATIC$StartManager$2001$IsStarted$Init.State = 1S
                    If flag Then
                        Monitor.[Exit](Me.$STATIC$StartManager$2001$IsStarted$Init)
                    End If
                End Try
                If Not Me.$STATIC$StartManager$2001$IsStarted Then
                    Me.$STATIC$StartManager$2001$IsStarted = True
                    CS$<>8__locals1.$VB$Local_ThreadStarter = Sub(Id As Integer)
                        Try
                            While True
                                IL_0000:
                                Thread.Sleep(20)
                                Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                                Dim list2 As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                                If Id = 0 AndAlso Me.FileRemain = 0 Then
                                    Me.AllFiles.Clear()
                                End If
                                Try
                                    For Each netFile As ModNet.NetFile In Me.AllFiles.Values
                                        If netFile.Uuid Mod 2 <> Id Then
                                            If netFile.State = ModNet.NetState.WaitingToDownload Then
                                                list.Add(netFile)
                                            ElseIf netFile.State < ModNet.NetState.Merging AndAlso netFile.State >= ModNet.NetState.WaitingToDownload Then
                                                list2.Add(netFile)
                                            End If
                                        End If
                                    Next
                                Finally
                                    Dim enumerator As IEnumerator(Of ModNet.NetFile)
                                    If enumerator IsNot Nothing Then
                                        enumerator.Dispose()
                                    End If
                                End Try
                                Try
                                    For Each netFile2 As ModNet.NetFile In list
                                        If ModNet.NetTaskThreadCount >= ModNet.NetTaskThreadLimit Then
                                            GoTo IL_0000
                                        End If
                                        Dim netThread As ModNet.NetThread = netFile2.TryBeginThread()
                                        If netThread IsNot Nothing AndAlso netThread.Source.Url.Contains("bmclapi") Then
                                            Thread.Sleep(100)
                                        End If
                                    Next
                                Finally
                                    Dim enumerator2 As List(Of ModNet.NetFile).Enumerator
                                    CType(enumerator2, IDisposable).Dispose()
                                End Try
                                If Me.Speed < ModNet.NetTaskSpeedLimitLow Then
                                    Try
                                        For Each netFile3 As ModNet.NetFile In list2
                                            If ModNet.NetTaskThreadCount >= ModNet.NetTaskThreadLimit Then
                                                Exit For
                                            End If
                                            Dim num As Integer = 0
                                            Dim num2 As Integer = 0
                                            If netFile3.Threads IsNot Nothing Then
                                                Try
                                                    For Each netThread2 As ModNet.NetThread In netFile3.Threads.ToList()
                                                        If netThread2.State < ModNet.NetState.Downloading Then
                                                            num += 1
                                                        ElseIf netThread2.State = ModNet.NetState.Downloading Then
                                                            num2 += 1
                                                        End If
                                                    Next
                                                Finally
                                                    Dim enumerator4 As List(Of ModNet.NetThread).Enumerator
                                                    CType(enumerator4, IDisposable).Dispose()
                                                End Try
                                            End If
                                            If num <= num2 Then
                                                Dim netThread3 As ModNet.NetThread = netFile3.TryBeginThread()
                                                If netThread3 IsNot Nothing AndAlso netThread3.Source.Url.Contains("bmclapi") Then
                                                    Thread.Sleep(100)
                                                End If
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator3 As List(Of ModNet.NetFile).Enumerator
                                        CType(enumerator3, IDisposable).Dispose()
                                    End Try
                                End If
                            End While
                        Catch ex As Exception
                            Logger.[Error](ex, String.Format("下载管理启动线程 {0} 出错", Id), LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        End Try
                    End Sub
                    ModBase.RunInNewThread(Sub()
                        CS$<>8__locals1.$VB$Local_ThreadStarter(0)
                    End Sub, "NetManager ThreadStarter 0", ThreadPriority.Normal)
                    ModBase.RunInNewThread(Sub()
                        CS$<>8__locals1.$VB$Local_ThreadStarter(1)
                    End Sub, "NetManager ThreadStarter 1", ThreadPriority.Normal)
                    ModBase.RunInNewThread(Sub()
                        Try
                            Dim num3 As Long = ModBase.GetTimeMs()
                            While True
                                If ModNet.NetTaskSpeedLimitHigh > 0L Then
                                    Interlocked.Add(ModNet.NetTaskSpeedLimitLeft, CLng(Math.Round(CDbl(ModNet.NetTaskSpeedLimitHigh) / 10.0)))
                                End If
                                Me.RefreshStat()
                                num3 += 100L
                                Dim num4 As Long = num3 - ModBase.GetTimeMs()
                                If num4 > 0L Then
                                    Thread.Sleep(CInt(num4))
                                Else
                                    num3 = ModBase.GetTimeMs()
                                End If
                            End While
                        Catch ex2 As Exception
                            Logger.[Error](ex2, "下载管理刷新线程出错", LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        End Try
                    End Sub, "NetManager StatRefresher", ThreadPriority.Normal)
                End If
            End Sub

            Public Function Start(Task As ModNet.LoaderDownload) As List(Of ModNet.NetFile)
                Me.StartManager()
                Dim downloadCacheLock As Object = Me.DownloadCacheLock
                ObjectFlowControl.CheckForSyncLockOnValueType(downloadCacheLock)
                SyncLock downloadCacheLock
                    If Not Me.IsDownloadCacheCleared Then
                        Try
                            Logger.Info("开始清理下载缓存", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            DirectoryUtils.Delete(ModBase.PathTemp + "Download", False)
                            Logger.Info("下载缓存已清理", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        Catch ex As Exception
                            Logger.Warn(ex, "清理下载缓存失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                        End Try
                        Me.IsDownloadCacheCleared = True
                    End If
                End SyncLock
                Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                Dim list2 As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                Try
                    Dim enumerator As IEnumerator(Of ModNet.NetFile) = Task.Files.GetEnumerator()
                    While enumerator.MoveNext()
                        Dim CS$<>8__locals1 As ModNet.NetManagerClass._Closure$__12-0 = New ModNet.NetManagerClass._Closure$__12-0(CS$<>8__locals1)
                        CS$<>8__locals1.$VB$Local_File = enumerator.Current
                        Dim netFile As ModNet.NetFile = Nothing
                        If Me.AllFiles.TryGetValue(CS$<>8__locals1.$VB$Local_File.LocalPath, netFile) AndAlso netFile.State < ModNet.NetState.Finished Then
                            Logger.Trace(Function() String.Format("{0}：无需开始，该文件已在下载中，目标 {1}", CS$<>8__locals1.$VB$Local_File.LocalName, CS$<>8__locals1.$VB$Local_File.LocalPath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            CS$<>8__locals1.$VB$Local_File = netFile
                        Else
                            Logger.Trace(Function() String.Format("{0}：开始，目标 {1}", CS$<>8__locals1.$VB$Local_File.LocalName, CS$<>8__locals1.$VB$Local_File.LocalPath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\ModNet.vb")
                            Me.AllFiles(CS$<>8__locals1.$VB$Local_File.LocalPath) = CS$<>8__locals1.$VB$Local_File
                            list.Add(CS$<>8__locals1.$VB$Local_File)
                            Interlocked.Increment(Me.FileRemain)
                        End If
                        list2.Add(CS$<>8__locals1.$VB$Local_File)
                        CS$<>8__locals1.$VB$Local_File.Tasks.Add(Task)
                    End While
                Finally
                    Dim enumerator As IEnumerator(Of ModNet.NetFile)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Task.Files = New ConcurrentQueue(Of ModNet.NetFile)(list2)
                Me.Tasks.Add(Task)
                Return list
            End Function

            Public AllFiles As ConcurrentDictionary(Of String, ModNet.NetFile)

            Public Tasks As ConcurrentBag(Of ModNet.LoaderDownload)

            Public DownloadDone As Long

            Public FileRemain As Integer

            Public Speed As Long

            Public Uuid As Integer

            Private RefreshStatLast As Long

            Private DownloadCacheLock As Object

            Private IsDownloadCacheCleared As Boolean

            Private $STATIC$RefreshStat$2001$SpeedLast As List(Of Long)

            Private $STATIC$RefreshStat$2001$SpeedLast$Init As StaticLocalInitFlag

            Private $STATIC$RefreshStat$2001$SpeedLastDone As Long

            Private $STATIC$RefreshStat$2001$SpeedLastDone$Init As StaticLocalInitFlag

            Private $STATIC$StartManager$2001$IsStarted As Boolean

            Private $STATIC$StartManager$2001$IsStarted$Init As StaticLocalInitFlag
        End Class
    End Module
End Namespace
