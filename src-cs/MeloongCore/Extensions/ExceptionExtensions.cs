using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MeloongCore.Extensions
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class ExceptionExtensions
    {
        public static Exception RootException(this Exception ex)
        {
            for (;;)
            {
                Exception innerException = ex.InnerException;
                if (innerException == null)
                {
                    break;
                }
                ex = innerException;
            }
            return ex;
        }

        public static bool IsCanceled(this Exception ex)
        {
            foreach (Exception ex2 in ex.Flatten())
            {
                bool flag = ex2 is OperationCanceledException || ex2 is ThreadInterruptedException;
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ThrowIfCanceled(this Exception ex)
        {
            foreach (Exception ex2 in ex.Flatten())
            {
                bool flag = ex2 is OperationCanceledException || ex2 is ThreadInterruptedException;
                if (flag)
                {
                    throw ex2;
                }
            }
        }

        public static IEnumerable<Exception> Flatten(this Exception ex)
        {
            ExceptionExtensions.<Flatten>d__3 <Flatten>d__ = new ExceptionExtensions.<Flatten>d__3(-2);
            <Flatten>d__.<>3__ex = ex;
            return <Flatten>d__;
        }

        public static string GetDisplay([Nullable(2)] this Exception ex, bool multiline)
        {
            if (ex == null)
            {
                return "无可用错误信息！";
            }
            List<string> list = new List<string>();
            bool flag = false;
            foreach (Exception ex2 in ex.Flatten())
            {
                if (multiline)
                {
                    list.Add((flag ? "→ " : "") + ExceptionExtensions.<GetDisplay>g__getExceptionMessage|4_0(ex2).ReplaceLineEndings("\r\n", true));
                    string stackTrace = ex2.StackTrace;
                    IEnumerable<string> enumerable = (((stackTrace != null) ? stackTrace.SplitLines(true) : null) ?? Array.Empty<string>()).Select<string, string>((string l) => l.BeforeLast("(", false) + l.AfterLast(")", false)).Distinct<string>();
                    list.AddRange(enumerable);
                    if (ex2.GetType() != typeof(Exception))
                    {
                        list.Add("   错误类型：" + ex2.GetType().FullName);
                    }
                }
                else
                {
                    list.Add(ExceptionExtensions.<GetDisplay>g__getExceptionMessage|4_0(ex2).ReplaceLineEndings(" ", true));
                }
                flag = true;
            }
            string text = null;
            Exception ex3 = ex.RootException();
            bool flag2 = ex3 is TypeLoadException || ex3 is BadImageFormatException || ex3 is MissingMethodException || ex3 is NotImplementedException || ex3 is TypeInitializationException;
            if (flag2)
            {
                text = "运行环境存在问题。请尝试重新安装 .NET Framework 4.8 然后再试。若无法安装，请先卸载较新版本的 .NET Framework，然后再尝试安装。";
            }
            else if (ex3 is UnauthorizedAccessException)
            {
                text = "程序权限不足。请尝试右键程序，选择以管理员身份运行，或将文件移动到其他文件夹。";
            }
            else if (ex3 is OutOfMemoryException)
            {
                text = "系统的运行内存不足。请在关闭一部分不需要的程序后再试。";
            }
            else
            {
                if (ex3 is SocketException)
                {
                    if (list.Any<string>((string l) => l.Contains("WSAStartup")))
                    {
                        text = "请尝试卸载中国移动云盘，然后再试。";
                        goto IL_01C2;
                    }
                }
                if (ex.IsBadNetwork())
                {
                    text = "你的网络环境不佳，请稍后再试，或使用 VPN 改善网络环境。";
                }
            }
            IL_01C2:
            if (multiline)
            {
                if (text == null)
                {
                    return list.Join("\r\n");
                }
                return text + "\r\n\r\n——————— 详细错误信息 ———————\r\n" + list.Join("\r\n");
            }
            else
            {
                list = list.Distinct<string>().ToList<string>();
                if (text == null)
                {
                    list.Reverse();
                    return list.Join(" ← ");
                }
                return text + "（" + list.First<string>() + "）";
            }
        }

        public static bool IsBadNetwork(this Exception ex)
        {
            foreach (Exception ex2 in ex.Flatten())
            {
                if (ex2 is TimeoutException)
                {
                    return true;
                }
                if (ex2 is TaskCanceledException)
                {
                    return true;
                }
                HttpRequestCodeException ex3 = ex2 as HttpRequestCodeException;
                if (ex3 != null)
                {
                    if (ex3.StatusCode == HttpStatusCode.Forbidden)
                    {
                        return false;
                    }
                    if (ex3.StatusCode == HttpStatusCode.RequestTimeout)
                    {
                        return true;
                    }
                }
                else
                {
                    WebException ex4 = ex2 as WebException;
                    if (ex4 != null)
                    {
                        HttpWebResponse httpWebResponse = ex4.Response as HttpWebResponse;
                        if (httpWebResponse != null)
                        {
                            if (httpWebResponse.StatusCode == HttpStatusCode.Forbidden)
                            {
                                return false;
                            }
                            if (httpWebResponse.StatusCode == HttpStatusCode.RequestTimeout)
                            {
                                return true;
                            }
                        }
                        bool flag;
                        switch (ex4.Status)
                        {
                        case WebExceptionStatus.NameResolutionFailure:
                        case WebExceptionStatus.ConnectFailure:
                        case WebExceptionStatus.ReceiveFailure:
                        case WebExceptionStatus.SendFailure:
                        case WebExceptionStatus.ConnectionClosed:
                        case WebExceptionStatus.SecureChannelFailure:
                        case WebExceptionStatus.KeepAliveFailure:
                        case WebExceptionStatus.Timeout:
                        case WebExceptionStatus.ProxyNameResolutionFailure:
                            flag = true;
                            break;
                        case WebExceptionStatus.PipelineFailure:
                        case WebExceptionStatus.RequestCanceled:
                        case WebExceptionStatus.ProtocolError:
                        case WebExceptionStatus.TrustFailure:
                        case WebExceptionStatus.ServerProtocolViolation:
                        case WebExceptionStatus.Pending:
                            goto IL_010B;
                        default:
                            goto IL_010B;
                        }
                        IL_010E:
                        if (flag)
                        {
                            return true;
                        }
                        continue;
                        IL_010B:
                        flag = false;
                        goto IL_010E;
                    }
                    SocketException ex5 = ex2 as SocketException;
                    if (ex5 != null)
                    {
                        SocketError socketErrorCode = ex5.SocketErrorCode;
                        if (socketErrorCode <= SocketError.ConnectionRefused)
                        {
                            if (socketErrorCode - SocketError.NetworkDown > 4 && socketErrorCode - SocketError.TimedOut > 1)
                            {
                                goto IL_016E;
                            }
                            goto IL_0169;
                        }
                        else
                        {
                            if (socketErrorCode == SocketError.HostUnreachable || socketErrorCode - SocketError.HostNotFound <= 1 || socketErrorCode == SocketError.NoData)
                            {
                                goto IL_0169;
                            }
                            goto IL_016E;
                        }
                        IL_0171:
                        bool flag;
                        if (flag)
                        {
                            return true;
                        }
                        continue;
                        IL_016E:
                        flag = false;
                        goto IL_0171;
                        IL_0169:
                        flag = true;
                        goto IL_0171;
                    }
                }
            }
            return false;
        }

        [CompilerGenerated]
        internal static string <GetDisplay>g__getExceptionMessage|4_0(Exception e)
        {
            string message = e.Message;
            WebException ex = e as WebException;
            string text;
            if (ex == null)
            {
                SocketException ex2 = e as SocketException;
                if (ex2 == null)
                {
                    text = "";
                }
                else
                {
                    text = string.Format(" (SocketErrorCode={0})", ex2.SocketErrorCode);
                }
            }
            else
            {
                text = string.Format(" (Status={0})", ex.Status);
            }
            return message + text;
        }
    }
}
