Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks

Namespace MeloongCore.Extensions
    <NullableContext(1)>
    <Nullable(0)>
    Public Module ExceptionExtensions
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function RootException(ex As Exception) As Exception
            While True
                Dim innerException As Exception = ex.InnerException
                If innerException Is Nothing Then
                    Exit For
                End If
                ex = innerException
            End While
            Return ex
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IsCanceled(ex As Exception) As Boolean
            For Each ex2 As Exception In ex.Flatten()
                Dim flag As Boolean = TypeOf ex2 Is OperationCanceledException OrElse TypeOf ex2 Is ThreadInterruptedException
                If flag Then
                    Return True
                End If
            Next
            Return False
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub ThrowIfCanceled(ex As Exception)
            For Each ex2 As Exception In ex.Flatten()
                Dim flag As Boolean = TypeOf ex2 Is OperationCanceledException OrElse TypeOf ex2 Is ThreadInterruptedException
                If flag Then
                    Throw ex2
                End If
            Next
        End Sub

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Flatten(ex As Exception) As IEnumerable(Of Exception)
            Dim <Flatten>d__ As ExceptionExtensions.<Flatten>d__3 = New ExceptionExtensions.<Flatten>d__3(-2)
            <Flatten>d__.<>3__ex = ex
            Return <Flatten>d__
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function GetDisplay(<Nullable(2)> ex As Exception, multiline As Boolean) As String
            If ex Is Nothing Then
                Return "无可用错误信息！"
            End If
            Dim list As List(Of String) = New List(Of String)()
            Dim flag As Boolean = False
            For Each ex2 As Exception In ex.Flatten()
                If multiline Then
                    list.Add(If(flag, "→ ", "") + ExceptionExtensions.<GetDisplay>g__getExceptionMessage|4_0(ex2).ReplaceLineEndings(vbCrLf, True))
                    Dim stackTrace As String = ex2.StackTrace
                    Dim enumerable As IEnumerable(Of String) = If(If((stackTrace IsNot Nothing), stackTrace.SplitLines(True), Nothing), Array.Empty(Of String)())
                    Dim <>9__4_ As Func(Of String, String) = ExceptionExtensions.<>c.<>9__4_2
                    Dim func As Func(Of String, String) = <>9__4_
                    If <>9__4_ Is Nothing Then
                        Dim func2 As Func(Of String, String) = Function(l As String) l.BeforeLast("(", False) + l.AfterLast(")", False)
                        func = func2
                        ExceptionExtensions.<>c.<>9__4_2 = func2
                    End If
                    Dim enumerable2 As IEnumerable(Of String) = enumerable.[Select](func).Distinct()
                    list.AddRange(enumerable2)
                    If ex2.[GetType]() IsNot GetType(Exception) Then
                        list.Add("   错误类型：" + ex2.[GetType]().FullName)
                    End If
                Else
                    list.Add(ExceptionExtensions.<GetDisplay>g__getExceptionMessage|4_0(ex2).ReplaceLineEndings(" ", True))
                End If
                flag = True
            Next
            Dim text As String = Nothing
            Dim ex3 As Exception = ex.RootException()
            Dim flag2 As Boolean = TypeOf ex3 Is TypeLoadException OrElse TypeOf ex3 Is BadImageFormatException OrElse TypeOf ex3 Is MissingMethodException OrElse TypeOf ex3 Is NotImplementedException OrElse TypeOf ex3 Is TypeInitializationException
            If flag2 Then
                text = "运行环境存在问题。请尝试重新安装 .NET Framework 4.8 然后再试。若无法安装，请先卸载较新版本的 .NET Framework，然后再尝试安装。"
            ElseIf TypeOf ex3 Is UnauthorizedAccessException Then
                text = "程序权限不足。请尝试右键程序，选择以管理员身份运行，或将文件移动到其他文件夹。"
            ElseIf TypeOf ex3 Is OutOfMemoryException Then
                text = "系统的运行内存不足。请在关闭一部分不需要的程序后再试。"
            Else
                If TypeOf ex3 Is SocketException Then
                    Dim enumerable3 As IEnumerable(Of String) = list
                    Dim <>9__4_2 As Func(Of String, Boolean) = ExceptionExtensions.<>c.<>9__4_1
                    Dim func3 As Func(Of String, Boolean) = <>9__4_2
                    If <>9__4_2 Is Nothing Then
                        Dim func4 As Func(Of String, Boolean) = Function(l As String) l.Contains("WSAStartup")
                        func3 = func4
                        ExceptionExtensions.<>c.<>9__4_1 = func4
                    End If
                    If enumerable3.Any(func3) Then
                        text = "请尝试卸载中国移动云盘，然后再试。"
                        GoTo IL_01C2
                    End If
                End If
                If ex.IsBadNetwork() Then
                    text = "你的网络环境不佳，请稍后再试，或使用 VPN 改善网络环境。"
                End If
            End If
            IL_01C2:
            If multiline Then
                If text Is Nothing Then
                    Return list.Join(vbCrLf)
                End If
                Return text + vbCrLf & vbCrLf & "——————— 详细错误信息 ———————" & vbCrLf + list.Join(vbCrLf)
            Else
                list = list.Distinct().ToList()
                If text Is Nothing Then
                    list.Reverse()
                    Return list.Join(" ← ")
                End If
                Return text + "（" + list.First() + "）"
            End If
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IsBadNetwork(ex As Exception) As Boolean
            For Each ex2 As Exception In ex.Flatten()
                If TypeOf ex2 Is TimeoutException Then
                    Return True
                End If
                If TypeOf ex2 Is TaskCanceledException Then
                    Return True
                End If
                Dim ex3 As HttpRequestCodeException = TryCast(ex2, HttpRequestCodeException)
                If ex3 IsNot Nothing Then
                    If ex3.StatusCode = HttpStatusCode.Forbidden Then
                        Return False
                    End If
                    If ex3.StatusCode = HttpStatusCode.RequestTimeout Then
                        Return True
                    End If
                Else
                    Dim ex4 As WebException = TryCast(ex2, WebException)
                    If ex4 IsNot Nothing Then
                        Dim httpWebResponse As HttpWebResponse = TryCast(ex4.Response, HttpWebResponse)
                        If httpWebResponse IsNot Nothing Then
                            If httpWebResponse.StatusCode = HttpStatusCode.Forbidden Then
                                Return False
                            End If
                            If httpWebResponse.StatusCode = HttpStatusCode.RequestTimeout Then
                                Return True
                            End If
                        End If
                        Dim flag As Boolean
                        Select Case ex4.Status
                            Case WebExceptionStatus.NameResolutionFailure, WebExceptionStatus.ConnectFailure, WebExceptionStatus.ReceiveFailure, WebExceptionStatus.SendFailure, WebExceptionStatus.ConnectionClosed, WebExceptionStatus.SecureChannelFailure, WebExceptionStatus.KeepAliveFailure, WebExceptionStatus.Timeout, WebExceptionStatus.ProxyNameResolutionFailure
                                flag = True
                            Case WebExceptionStatus.PipelineFailure, WebExceptionStatus.RequestCanceled, WebExceptionStatus.ProtocolError, WebExceptionStatus.TrustFailure, WebExceptionStatus.ServerProtocolViolation, WebExceptionStatus.Pending
                                GoTo IL_010B
                            Case Else
                                GoTo IL_010B
                        End Select
                        IL_010E:
                        If flag Then
                            Return True
                        End If
                        Continue For
                        IL_010B:
                        flag = False
                        GoTo IL_010E
                    End If
                    Dim ex5 As SocketException = TryCast(ex2, SocketException)
                    If ex5 IsNot Nothing Then
                        Dim socketErrorCode As SocketError = ex5.SocketErrorCode
                        If socketErrorCode <= SocketError.ConnectionRefused Then
                            If socketErrorCode - SocketError.NetworkDown > 4 AndAlso socketErrorCode - SocketError.TimedOut > 1 Then
                                GoTo IL_016E
                            End If
                            GoTo IL_0169
                        Else
                            If socketErrorCode = SocketError.HostUnreachable OrElse socketErrorCode - SocketError.HostNotFound <= 1 OrElse socketErrorCode = SocketError.NoData Then
                                GoTo IL_0169
                            End If
                            GoTo IL_016E
                        End If
                        IL_0171:
                        Dim flag As Boolean
                        If flag Then
                            Return True
                        End If
                        Continue For
                        IL_016E:
                        flag = False
                        GoTo IL_0171
                        IL_0169:
                        flag = True
                        GoTo IL_0171
                    End If
                End If
            Next
            Return False
        End Function

        <CompilerGenerated()>
        Friend Function <GetDisplay>g__getExceptionMessage|4_0(e As Exception) As String
            Dim message As String = e.Message
            Dim ex As WebException = TryCast(e, WebException)
            Dim text As String
            If ex Is Nothing Then
                Dim ex2 As SocketException = TryCast(e, SocketException)
                If ex2 Is Nothing Then
                    text = ""
                Else
                    text = String.Format(" (SocketErrorCode={0})", ex2.SocketErrorCode)
                End If
            Else
                text = String.Format(" (Status={0})", ex.Status)
            End If
            Return message + text
        End Function
    End Module
End Namespace
