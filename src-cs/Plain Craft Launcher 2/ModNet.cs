using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using ThrottleDebounce;

namespace PCL
{
    [StandardModule]
    public sealed class ModNet
    {
        public static string NetRequestByClient(string Url, HttpMethod Method = null, object Content = null, string ContentType = null, string Accept = "*/*", int Timeout = 25000, string[,] Headers = null, bool RequireJson = false, Encoding Encoding = null, bool SimulateBrowserHeaders = false, bool MakeLog = true)
        {
            if (Method == null)
            {
                Method = HttpMethod.Get;
            }
            if (MakeLog)
            {
                Logger.Info(string.Format("发起网络请求（{0}，{1}），最大超时 {2}", Method.Method, Url, Timeout), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
            }
            string text2;
            try
            {
                Dictionary<string, string> dictionary = (Headers ?? new string[0, 0]).ToDictionary<string>();
                if (ContentType != null)
                {
                    dictionary["Content-Type"] = ContentType;
                }
                if (Accept != null)
                {
                    dictionary["Accept"] = Accept;
                }
                string text = ModNet.SendRequest(Url, Method, RuntimeHelpers.GetObjectValue(Content), dictionary, SimulateBrowserHeaders, Timeout, Encoding).GetString(Encoding);
                if (RequireJson)
                {
                    text = text.Trim("\r\n ".ToCharArray());
                    if ((!text.StartsWithF("{", false) || !text.EndsWithF("}", false)) && (!text.StartsWithF("[", false) || !text.EndsWithF("]", false)))
                    {
                        throw new FormatException("返回结果并非 JSON 格式：" + ((text.Length > 2000) ? (text.Substring(0, 1000) + "..." + text.Substring(checked(text.Length - 1000))) : text));
                    }
                }
                text2 = text;
            }
            catch (FormatException ex)
            {
                throw;
            }
            catch (Exception ex2)
            {
                if (MakeLog)
                {
                    Logger.Warn(ex2, string.Format("网络请求失败（{0}，{1}）", Method.Method, Url), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                }
                throw;
            }
            return text2;
        }

        public static string NetRequestByClientRetry(string Url, HttpMethod Method = null, string BackupUrl = null, object Content = null, string ContentType = null, string Accept = "*/*", string[,] Headers = null, bool RequireJson = false, Encoding Encoding = null, bool SimulateBrowserHeaders = false)
        {
            int num = 0;
            Exception ex = null;
            long timeMs = ModBase.GetTimeMs();
            checked
            {
                string text;
                try
                {
                    IL_000A:
                    if (num != 0)
                    {
                        if (num != 1)
                        {
                            if (ModBase.GetTimeMs() - timeMs <= 5500L)
                            {
                                throw ex;
                            }
                            Thread.Sleep(500);
                            text = ModNet.NetRequestByClient(Url, Method, RuntimeHelpers.GetObjectValue(Content), ContentType, Accept, 4000, Headers, RequireJson, Encoding, SimulateBrowserHeaders, true);
                        }
                        else
                        {
                            Thread.Sleep(500);
                            text = ModNet.NetRequestByClient(Url, Method, RuntimeHelpers.GetObjectValue(Content), ContentType, Accept, 30000, Headers, RequireJson, Encoding, SimulateBrowserHeaders, true);
                        }
                    }
                    else
                    {
                        text = ModNet.NetRequestByClient(Url, Method, RuntimeHelpers.GetObjectValue(Content), ContentType, Accept, 10000, Headers, RequireJson, Encoding, SimulateBrowserHeaders, true);
                    }
                }
                catch (Exception ex2)
                {
                    ex2.ThrowIfCanceled();
                    if (ex2 is HttpRequestCodeException)
                    {
                        if (((HttpRequestCodeException)ex2).StatusCode == HttpStatusCode.Forbidden)
                        {
                            throw;
                        }
                        if (((HttpRequestCodeException)ex2).StatusCode == HttpStatusCode.NotFound)
                        {
                            throw;
                        }
                        if (((HttpRequestCodeException)ex2).StatusCode == (HttpStatusCode)429)
                        {
                            Thread.Sleep(10000);
                        }
                    }
                    if (num == 0)
                    {
                        ex = ex2;
                        num++;
                        goto IL_000A;
                    }
                    if (num != 1)
                    {
                        throw;
                    }
                    num++;
                    goto IL_000A;
                }
                return text;
            }
        }

        public static string NetRequestByClientMultiple(string Url, HttpMethod Method = null, int ThreadCount = 3, object Content = null, string ContentType = null, string Accept = "*/*", string[,] Headers = null, bool RequireJson = false, int Timeout = 25000, Encoding Encoding = null, bool SimulateBrowserHeaders = false)
        {
            ModNet._Closure$__3-0 CS$<>8__locals1 = new ModNet._Closure$__3-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Url = Url;
            CS$<>8__locals1.$VB$Local_Method = Method;
            CS$<>8__locals1.$VB$Local_Content = Content;
            CS$<>8__locals1.$VB$Local_ContentType = ContentType;
            CS$<>8__locals1.$VB$Local_Accept = Accept;
            CS$<>8__locals1.$VB$Local_Headers = Headers;
            CS$<>8__locals1.$VB$Local_RequireJson = RequireJson;
            CS$<>8__locals1.$VB$Local_Timeout = Timeout;
            CS$<>8__locals1.$VB$Local_Encoding = Encoding;
            CS$<>8__locals1.$VB$Local_SimulateBrowserHeaders = SimulateBrowserHeaders;
            List<Thread> list = new List<Thread>();
            CS$<>8__locals1.$VB$Local_RequestResult = null;
            CS$<>8__locals1.$VB$Local_RequestEx = null;
            CS$<>8__locals1.$VB$Local_FailCount = 0;
            checked
            {
                for (int i = 1; i <= ThreadCount; i++)
                {
                    Thread thread = new Thread((CS$<>8__locals1.$I0 == null) ? (CS$<>8__locals1.$I0 = delegate
                    {
                        try
                        {
                            CS$<>8__locals1.$VB$Local_RequestResult = ModNet.NetRequestByClient(CS$<>8__locals1.$VB$Local_Url, CS$<>8__locals1.$VB$Local_Method, RuntimeHelpers.GetObjectValue(CS$<>8__locals1.$VB$Local_Content), CS$<>8__locals1.$VB$Local_ContentType, CS$<>8__locals1.$VB$Local_Accept, CS$<>8__locals1.$VB$Local_Timeout, CS$<>8__locals1.$VB$Local_Headers, CS$<>8__locals1.$VB$Local_RequireJson, CS$<>8__locals1.$VB$Local_Encoding, CS$<>8__locals1.$VB$Local_SimulateBrowserHeaders, true);
                        }
                        catch (Exception ex)
                        {
                            CS$<>8__locals1.$VB$Local_FailCount++;
                            CS$<>8__locals1.$VB$Local_RequestEx = ex;
                        }
                    }) : CS$<>8__locals1.$I0);
                    thread.Start();
                    list.Add(thread);
                    Thread.Sleep(i * 250);
                    if (CS$<>8__locals1.$VB$Local_RequestResult != null)
                    {
                        IL_00DB:
                        try
                        {
                            try
                            {
                                foreach (Thread thread2 in list)
                                {
                                    if (thread2.IsAlive)
                                    {
                                        thread2.Interrupt();
                                    }
                                }
                            }
                            finally
                            {
                                List<Thread>.Enumerator enumerator;
                                ((IDisposable)enumerator).Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        return Conversions.ToString(CS$<>8__locals1.$VB$Local_RequestResult);
                    }
                }
                while (CS$<>8__locals1.$VB$Local_RequestResult == null)
                {
                    if (CS$<>8__locals1.$VB$Local_FailCount == ThreadCount)
                    {
                        try
                        {
                            try
                            {
                                foreach (Thread thread3 in list)
                                {
                                    if (thread3.IsAlive)
                                    {
                                        thread3.Interrupt();
                                    }
                                }
                            }
                            finally
                            {
                                List<Thread>.Enumerator enumerator2;
                                ((IDisposable)enumerator2).Dispose();
                            }
                        }
                        catch (Exception ex2)
                        {
                        }
                        throw CS$<>8__locals1.$VB$Local_RequestEx;
                    }
                    Thread.Sleep(20);
                }
                goto IL_00DB;
            }
        }

        public static string NetRequestByLoader(string Url, int Timeout = 45000, bool IsJson = false, bool SimulateBrowserHeaders = false)
        {
            return ModNet.NetRequestByLoader(new string[] { Url }, Timeout, IsJson, SimulateBrowserHeaders);
        }

        public static string NetRequestByLoader(IEnumerable<string> Urls, int Timeout = 45000, bool IsJson = false, bool SimulateBrowserHeaders = false)
        {
            string text = ModMain.RequestTaskTempFolder(false) + "download.txt";
            ModNet.LoaderDownload loaderDownload = new ModNet.LoaderDownload("源码获取 " + Conversions.ToString(ModBase.GetUuid()) + "#", new List<ModNet.NetFile>
            {
                new ModNet.NetFile(Urls, text, new FileChecker
                {
                    IsJson = IsJson
                }, SimulateBrowserHeaders)
            });
            string text3;
            try
            {
                loaderDownload.WaitForExitTime(Timeout, null, "连接服务器超时（第一下载源：" + Urls.First<string>() + "）", null, false);
                string text2 = FileUtils.ReadAsString(text, null, null);
                FileUtils.Delete(text, false);
                text3 = text2;
            }
            finally
            {
                loaderDownload.Cancel();
            }
            return text3;
        }

        public static void NetDownloadByClient(string Url, string LocalPath, bool SimulateBrowserHeaders = false)
        {
            Logger.Info(string.Format("通过网络请求直接下载小文件：{0} → {1}", Url, LocalPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
            int num = 0;
            checked
            {
                for (;;)
                {
                    try
                    {
                        FileUtils.Write(LocalPath, ModNet.SendRequest(Url, HttpMethod.Get, null, null, SimulateBrowserHeaders, 25000, null));
                    }
                    catch (Exception ex)
                    {
                        FileUtils.Delete(LocalPath, false);
                        ex.ThrowIfCanceled();
                        if (num < 2)
                        {
                            num++;
                            continue;
                        }
                        throw new HttpRequestException(string.Format("通过网络请求直接下载小文件失败（{0}）", LocalPath), ex);
                    }
                    break;
                }
            }
        }

        public static void NetDownloadByLoader(string Url, string LocalPath, ModLoader.LoaderBase LoaderToSyncProgress = null, FileChecker Check = null, bool SimulateBrowserHeaders = false)
        {
            ModNet.NetDownloadByLoader(new string[] { Url }, LocalPath, LoaderToSyncProgress, Check, SimulateBrowserHeaders);
        }

        public static void NetDownloadByLoader(IEnumerable<string> Urls, string LocalPath, ModLoader.LoaderBase LoaderToSyncProgress = null, FileChecker Check = null, bool SimulateBrowserHeaders = false)
        {
            ModNet.LoaderDownload loaderDownload = new ModNet.LoaderDownload(string.Format("文件下载 {0}#", ModBase.GetUuid()), new List<ModNet.NetFile>
            {
                new ModNet.NetFile(Urls, LocalPath, Check, SimulateBrowserHeaders)
            });
            try
            {
                loaderDownload.WaitForExit(null, LoaderToSyncProgress, false, default(CancellationToken), null);
            }
            catch (Exception ex)
            {
                throw new WebException("多线程直接下载文件失败（第一下载源：" + Urls.First<string>() + "）", ex);
            }
            finally
            {
                loaderDownload.Cancel();
            }
        }

        private static byte[] SendRequest(string Url, HttpMethod Method, object Content = null, Dictionary<string, string> Headers = null, bool SimulateBrowserHeaders = false, int Timeout = 25000, Encoding Encoding = null)
        {
            ModNet._Closure$__9-0 CS$<>8__locals1 = new ModNet._Closure$__9-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Timeout = Timeout;
            if (ModBase.RunInUi() && !Url.Contains("//127."))
            {
                throw new Exception("在 UI 线程执行了网络请求");
            }
            CS$<>8__locals1.$VB$Local_Request = null;
            CS$<>8__locals1.$VB$Local_CancelToken = null;
            CS$<>8__locals1.$VB$Local_Response = null;
            byte[] array2;
            try
            {
                Directory.CreateDirectory(ModBase.PathTemp + "Cache\\Http\\");
                object requestClientLock = ModNet.RequestClientLock;
                ObjectFlowControl.CheckForSyncLockOnValueType(requestClientLock);
                lock (requestClientLock)
                {
                    if (ModNet.RequestClient == null)
                    {
                        ModNet.RequestClient = ClientExtensions.CreateClient(new FileStore(ModBase.PathTemp + "Cache/Http/"), new HttpClientHandler
                        {
                            AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate),
                            UseCookies = false
                        });
                    }
                }
                Url = ModSecret.SecretCdnSign(Url);
                CS$<>8__locals1.$VB$Local_Request = new HttpRequestMessage(Method, Url);
                if (Content != null && CS$<>8__locals1.$VB$Local_Request.Method != HttpMethod.Get && CS$<>8__locals1.$VB$Local_Request.Method != HttpMethod.Head)
                {
                    if (Content is HttpContent)
                    {
                        CS$<>8__locals1.$VB$Local_Request.Content = WebExtensions.Clone((HttpContent)Content);
                    }
                    else if (Content is byte[])
                    {
                        CS$<>8__locals1.$VB$Local_Request.Content = new ByteArrayContent((byte[])Content);
                    }
                    else if (Content != null)
                    {
                        CS$<>8__locals1.$VB$Local_Request.Content = new ByteArrayContent((Encoding ?? Encoding.UTF8).GetBytes(Content.ToString()));
                    }
                }
                try
                {
                    foreach (KeyValuePair<string, string> keyValuePair in (Headers ?? new Dictionary<string, string>()))
                    {
                        if (Operators.CompareString(keyValuePair.Key.Lower(), "content-type", false) == 0)
                        {
                            HttpContent content = CS$<>8__locals1.$VB$Local_Request.Content;
                            if (content != null)
                            {
                                content.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
                            }
                        }
                        else
                        {
                            CS$<>8__locals1.$VB$Local_Request.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                }
                finally
                {
                    Dictionary<string, string>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                ModSecret.SecretHeadersSign(Url, ref CS$<>8__locals1.$VB$Local_Request, SimulateBrowserHeaders);
                Retrier.Attempt(delegate(int Attempt)
                {
                    CancellationTokenSource $VB$Local_CancelToken2 = CS$<>8__locals1.$VB$Local_CancelToken;
                    if ($VB$Local_CancelToken2 != null)
                    {
                        $VB$Local_CancelToken2.Dispose();
                    }
                    CS$<>8__locals1.$VB$Local_CancelToken = new CancellationTokenSource(CS$<>8__locals1.$VB$Local_Timeout);
                    HttpRequestMessage httpRequestMessage = WebExtensions.Clone(CS$<>8__locals1.$VB$Local_Request);
                    try
                    {
                        CS$<>8__locals1.$VB$Local_Response = ModNet.RequestClient.SendAsync(httpRequestMessage, 1, CS$<>8__locals1.$VB$Local_CancelToken.Token).GetResultWithTimeout<HttpResponseMessage>(CS$<>8__locals1.$VB$Local_CancelToken, CS$<>8__locals1.$VB$Local_Timeout);
                    }
                    finally
                    {
                        httpRequestMessage.Dispose();
                    }
                }, new uint?(2U), (ModNet._Closure$__.$IR9-1 == null) ? (ModNet._Closure$__.$IR9-1 = (int a0) => ((ModNet._Closure$__.$I9-1 == null) ? (ModNet._Closure$__.$I9-1 = () => TimeSpan.FromMilliseconds(200.0)) : ModNet._Closure$__.$I9-1)()) : ModNet._Closure$__.$IR9-1, (ModNet._Closure$__.$I9-2 == null) ? (ModNet._Closure$__.$I9-2 = (Exception ex) => ex is FileNotFoundException || ex is IOException) : ModNet._Closure$__.$I9-2, null, default(CancellationToken));
                CS$<>8__locals1.$VB$Local_CancelToken.Dispose();
                CS$<>8__locals1.$VB$Local_CancelToken = new CancellationTokenSource(CS$<>8__locals1.$VB$Local_Timeout);
                byte[] array;
                using (Stream resultWithTimeout = CS$<>8__locals1.$VB$Local_Response.Content.ReadAsStreamAsync().GetResultWithTimeout<Stream>(CS$<>8__locals1.$VB$Local_CancelToken, CS$<>8__locals1.$VB$Local_Timeout))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        resultWithTimeout.CopyToAsync(memoryStream, 81920, CS$<>8__locals1.$VB$Local_CancelToken.Token).GetResultWithTimeout(CS$<>8__locals1.$VB$Local_CancelToken, CS$<>8__locals1.$VB$Local_Timeout);
                        array = memoryStream.ToArray();
                    }
                }
                if (!CS$<>8__locals1.$VB$Local_Response.IsSuccessStatusCode)
                {
                    string @string = array.GetString(Encoding);
                    throw new HttpRequestCodeException(string.Format("错误码 {0} ({1})，{2}，{3}", new object[]
                    {
                        CS$<>8__locals1.$VB$Local_Response.StatusCode,
                        (int)CS$<>8__locals1.$VB$Local_Response.StatusCode,
                        Method,
                        Url
                    }) + (string.IsNullOrEmpty(@string) ? "" : ("\r\n" + @string)), CS$<>8__locals1.$VB$Local_Response.StatusCode, @string);
                }
                array2 = array;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (TimeoutException ex2)
            {
                throw new WebException(string.Format("连接服务器超时，请稍后再试，或使用 VPN 改善网络环境（{0}, {1}）", Method, Url), WebExceptionStatus.Timeout);
            }
            catch (Exception ex3)
            {
                if (!(ex3 is TaskCanceledException))
                {
                    ex3.ThrowIfCanceled();
                }
                if (ex3.IsBadNetwork())
                {
                    throw new WebException(string.Format("网络请求失败，请稍后再试，或使用 VPN 改善网络环境（{0}, {1}）", Method, Url), WebExceptionStatus.Timeout);
                }
                throw new Exception(string.Format("网络请求出现意外异常（{0}, {1}）", Method, Url), ex3);
            }
            finally
            {
                HttpRequestMessage $VB$Local_Request = CS$<>8__locals1.$VB$Local_Request;
                if ($VB$Local_Request != null)
                {
                    $VB$Local_Request.Dispose();
                }
                CancellationTokenSource $VB$Local_CancelToken = CS$<>8__locals1.$VB$Local_CancelToken;
                if ($VB$Local_CancelToken != null)
                {
                    $VB$Local_CancelToken.Dispose();
                }
                HttpResponseMessage $VB$Local_Response = CS$<>8__locals1.$VB$Local_Response;
                if ($VB$Local_Response != null)
                {
                    $VB$Local_Response.Dispose();
                }
            }
            return array2;
        }

        public static int NetTaskThreadLimit
        {
            get
            {
                return checked(Settings.Get<int>("ToolDownloadThread", null) + 1);
            }
        }

        public static long NetTaskSpeedLimitHigh
        {
            get
            {
                checked
                {
                    if (ModNet._NetTaskSpeedLimitHigh == null)
                    {
                        int num = Settings.Get<int>("ToolDownloadSpeed", null);
                        if (num <= 14)
                        {
                            ModNet._NetTaskSpeedLimitHigh = new long?((long)Math.Round(unchecked((double)(checked(num + 1)) * 0.1 * 1024.0 * 1024.0)));
                        }
                        else if (num <= 31)
                        {
                            ModNet._NetTaskSpeedLimitHigh = new long?((long)Math.Round(unchecked((double)(checked(num - 11)) * 0.5 * 1024.0 * 1024.0)));
                        }
                        else if (num <= 41)
                        {
                            ModNet._NetTaskSpeedLimitHigh = new long?(unchecked((long)(checked((num - 21) * 1024))) * 1024L);
                        }
                        else
                        {
                            ModNet._NetTaskSpeedLimitHigh = new long?(-1L);
                        }
                    }
                    return ModNet._NetTaskSpeedLimitHigh.Value;
                }
            }
        }

        public static void UpdateNetTaskSpeedLimitHigh(int Value)
        {
            ModNet._NetTaskSpeedLimitHigh = null;
        }

        public static bool HasDownloadingTask(bool IgnoreCustomDownload = false)
        {
            return ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>((ModLoader.LoaderBase Task) => Task.Show && Task.State == ModBase.LoadState.Loading && (!IgnoreCustomDownload || !Task.Name.ToString().Contains("自定义下载")));
        }

        public static int FindFreePort()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }

        public static List<int> GetUsedPorts()
        {
            IPGlobalProperties ipglobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            List<int> list = new List<int>();
            list.AddRange(ipglobalProperties.GetActiveTcpListeners().Select<IPEndPoint, int>((ModNet._Closure$__.$I33-0 == null) ? (ModNet._Closure$__.$I33-0 = (IPEndPoint ep) => ep.Port) : ModNet._Closure$__.$I33-0));
            list.AddRange(ipglobalProperties.GetActiveUdpListeners().Select<IPEndPoint, int>((ModNet._Closure$__.$I33-1 == null) ? (ModNet._Closure$__.$I33-1 = (IPEndPoint ep) => ep.Port) : ModNet._Closure$__.$I33-1));
            list.AddRange(ipglobalProperties.GetActiveTcpConnections().Select<TcpConnectionInformation, int>((ModNet._Closure$__.$I33-2 == null) ? (ModNet._Closure$__.$I33-2 = (TcpConnectionInformation conn) => conn.LocalEndPoint.Port) : ModNet._Closure$__.$I33-2));
            return list.Distinct<int>().ToList<int>();
        }

        public static List<int> FindFreePorts(int ConsecutiveCount, params int[] ExtraBlackLists)
        {
            ModNet._Closure$__34-0 CS$<>8__locals1 = new ModNet._Closure$__34-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_UsedPorts = ModNet.GetUsedPorts().Concat<int>(ExtraBlackLists);
            checked
            {
                int num = 12000 + ModBase.RandomInteger(0, 1000);
                int num2 = 65000 - ConsecutiveCount;
                for (int i = num; i <= num2; i++)
                {
                    IEnumerable<int> enumerable = Enumerable.Range(i, ConsecutiveCount);
                    if (!enumerable.Any<int>((CS$<>8__locals1.$I0 == null) ? (CS$<>8__locals1.$I0 = (int p) => CS$<>8__locals1.$VB$Local_UsedPorts.Contains(p)) : CS$<>8__locals1.$I0))
                    {
                        return enumerable.ToList<int>();
                    }
                }
                throw new Exception("未能找到可用的端口！");
            }
        }

        public static int Ping(string Ip, int Timeout = 10000, bool MakeLog = true)
        {
            PingReply pingReply;
            try
            {
                pingReply = new Ping().Send(Ip);
            }
            catch (Exception ex)
            {
                if (MakeLog)
                {
                    Logger.Info(string.Format("Ping {0} 失败：{1}", Ip, ex.Message), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                }
                return -1;
            }
            int num;
            if (pingReply.Status == IPStatus.Success)
            {
                if (MakeLog)
                {
                    Logger.Info(string.Format("Ping {0} 结束：{1}ms", Ip, pingReply.RoundtripTime), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                }
                num = checked((int)pingReply.RoundtripTime);
            }
            else
            {
                if (MakeLog)
                {
                    Logger.Info(string.Format("Ping {0} 失败", Ip), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                }
                num = -1;
            }
            return num;
        }

        public static void ShouldValidateSslCertificateOnLogin(bool Enabled)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object Sender, X509Certificate Certificate, X509Chain Chain, SslPolicyErrors Failure)
            {
                HttpWebRequest httpWebRequest = Sender as HttpWebRequest;
                bool flag;
                if (Failure == SslPolicyErrors.None)
                {
                    flag = true;
                }
                else
                {
                    Logger.Warn(string.Format("未通过 SSL 证书验证（{0}），提供的证书为 {1}，URL：{2}", Failure, (Certificate != null) ? Certificate.Subject : null, (httpWebRequest != null) ? httpWebRequest.Address : null), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                    if (httpWebRequest == null)
                    {
                        flag = !Enabled;
                    }
                    else
                    {
                        flag = (httpWebRequest.Address.Host.Contains("xboxlive") || httpWebRequest.Address.Host.Contains("minecraftservices")) && !Enabled;
                    }
                }
                return flag;
            };
        }

        private static HttpClient RequestClient = null;

        private static object RequestClientLock = RuntimeHelpers.GetObjectValue(new object());

        private static HttpClient ThreadClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
        });

        public static long NetTaskSpeedLimitLow = 262144L;

        private static long? _NetTaskSpeedLimitHigh = null;

        public static long NetTaskSpeedLimitLeft = -1L;

        public static int NetTaskThreadCount = 0;

        public static ModNet.NetManagerClass NetManager = new ModNet.NetManagerClass();

        public class NetSource
        {
            public override string ToString()
            {
                return this.Url;
            }

            public int Id;

            public string Url;

            public int FailCount;

            public Exception Ex;

            public ModNet.NetThread SingleThread;

            public bool IsFailed;
        }

        public enum NetState
        {
            WaitingToCheck = -1,
            WaitingToDownload,
            Connecting,
            Reading,
            Downloading,
            Merging,
            Finished,
            Canceled
        }

        public enum NetPreDownloadBehaviour
        {
            HintWhileExists,
            ExitWhileExistsOrDownloading,
            IgnoreCheck
        }

        public class NetThread : IEnumerable<ModNet.NetThread>, IEquatable<ModNet.NetThread>
        {
            public NetThread()
            {
                this.DownloadDone = 0L;
                this.SpeedLastTime = ModBase.GetTimeMs();
                this.SpeedLastDone = 0L;
                this._Speed = 0L;
                this.InitTime = ModBase.GetTimeMs();
                this.LastReceiveTime = -1L;
                this.State = ModNet.NetState.WaitingToDownload;
            }

            private IEnumerable<ModNet.NetThread> Next
            {
                get
                {
                    ModNet.NetThread.VB$StateMachine_5_get_Next vb$StateMachine_5_get_Next = new ModNet.NetThread.VB$StateMachine_5_get_Next(-2);
                    vb$StateMachine_5_get_Next.$VB$Me = this;
                    return vb$StateMachine_5_get_Next;
                }
            }

            public IEnumerator<ModNet.NetThread> GetEnumerator()
            {
                return this.Next.GetEnumerator();
            }

            IEnumerator IEnumerable.IEnumerable_GetEnumerator()
            {
                return this.Next.GetEnumerator();
            }

            public bool IsFirstThread
            {
                get
                {
                    return this.DownloadStart == 0L && this.Task.FileSize == -2L;
                }
            }

            public long DownloadEnd
            {
                get
                {
                    object lockChain = this.Task.LockChain;
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockChain);
                    checked
                    {
                        long num;
                        lock (lockChain)
                        {
                            if (this.NextThread == null)
                            {
                                if (this.Task.IsUnknownSize)
                                {
                                    num = 1000000000000L;
                                }
                                else
                                {
                                    num = this.Task.FileSize - 1L;
                                }
                            }
                            else
                            {
                                num = this.NextThread.DownloadStart - 1L;
                            }
                        }
                        return num;
                    }
                }
            }

            public long DownloadUndone
            {
                get
                {
                    return checked(this.DownloadEnd - (this.DownloadStart + this.DownloadDone) + 1L);
                }
            }

            public long Speed
            {
                get
                {
                    checked
                    {
                        if (ModBase.GetTimeMs() - this.SpeedLastTime > 200L)
                        {
                            long num = ModBase.GetTimeMs() - this.SpeedLastTime;
                            this._Speed = (long)Math.Round((double)(this.DownloadDone - this.SpeedLastDone) / ((double)num / 1000.0));
                            this.SpeedLastDone = this.DownloadDone;
                            ref long ptr = ref this.SpeedLastTime;
                            this.SpeedLastTime = ptr + num;
                        }
                        return this._Speed;
                    }
                }
            }

            public bool IsEnded
            {
                get
                {
                    return this.State == ModNet.NetState.Finished || this.State == ModNet.NetState.Canceled;
                }
            }

            public bool Equals(ModNet.NetThread other)
            {
                return other != null && this.Uuid == other.Uuid;
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj as ModNet.NetThread);
            }

            public static bool operator ==(ModNet.NetThread left, ModNet.NetThread right)
            {
                return EqualityComparer<ModNet.NetThread>.Default.Equals(left, right);
            }

            public static bool operator !=(ModNet.NetThread left, ModNet.NetThread right)
            {
                return !(left == right);
            }

            public ModNet.NetFile Task;

            public Thread Thread;

            public ModNet.NetThread NextThread;

            public int Uuid;

            public string Temp;

            public long DownloadStart;

            public long DownloadDone;

            private long SpeedLastTime;

            private long SpeedLastDone;

            private long _Speed;

            public long InitTime;

            public long LastReceiveTime;

            public ModNet.NetState State;

            public ModNet.NetSource Source;
        }

        public class NetFile
        {
            private ModNet.NetSource GetSource(int Id = 0)
            {
                if (Id >= this.Sources.Count || Id < 0)
                {
                    Id = 0;
                }
                object lockSource = this.LockSource;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockSource);
                checked
                {
                    ModNet.NetSource netSource2;
                    lock (lockSource)
                    {
                        if (this.HasAvailableSource(false))
                        {
                            ModNet.NetSource netSource = this.Sources[Id];
                            while (netSource.IsFailed)
                            {
                                Id++;
                                if (Id >= this.Sources.Count)
                                {
                                    Id = 0;
                                }
                                netSource = this.Sources[Id];
                            }
                            netSource2 = netSource;
                        }
                        else if (this.SourcesOnce.Any<ModNet.NetSource>())
                        {
                            netSource2 = this.SourcesOnce.First<ModNet.NetSource>();
                        }
                        else
                        {
                            netSource2 = null;
                        }
                    }
                    return netSource2;
                }
            }

            public bool HasAvailableSource(bool AllowOnceSource = true)
            {
                object lockSource = this.LockSource;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockSource);
                lock (lockSource)
                {
                    if (this.Sources.Any<ModNet.NetSource>((ModNet.NetFile._Closure$__.$I6-0 == null) ? (ModNet.NetFile._Closure$__.$I6-0 = (ModNet.NetSource s) => !s.IsFailed) : ModNet.NetFile._Closure$__.$I6-0))
                    {
                        return true;
                    }
                    if (AllowOnceSource && this.SourcesOnce.Any<ModNet.NetSource>())
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool IsNoSplit
            {
                get
                {
                    return this.IsUnknownSize || this.FileSize < 1048576L;
                }
            }

            public long Speed
            {
                get
                {
                    checked
                    {
                        if (ModBase.GetTimeMs() - this.SpeedLastTime > 200L)
                        {
                            long num = ModBase.GetTimeMs() - this.SpeedLastTime;
                            this._Speed = (long)Math.Round((double)(this.DownloadDone - this.SpeedLastDone) / ((double)num / 1000.0));
                            this.SpeedLastDone = this.DownloadDone;
                            ref long ptr = ref this.SpeedLastTime;
                            this.SpeedLastTime = ptr + num;
                        }
                        return this._Speed;
                    }
                }
            }

            public double Progress
            {
                get
                {
                    double num;
                    switch (this.State)
                    {
                    case ModNet.NetState.WaitingToCheck:
                        num = 0.0;
                        break;
                    case ModNet.NetState.WaitingToDownload:
                        num = 0.01;
                        break;
                    case ModNet.NetState.Connecting:
                        num = 0.02;
                        break;
                    case ModNet.NetState.Reading:
                        num = 0.04;
                        break;
                    case ModNet.NetState.Downloading:
                    {
                        double num2 = (this.IsUnknownSize ? 0.5 : ((double)this.DownloadDone / (double)Math.Max(this.FileSize, 1L)));
                        num2 = 1.0 - Math.Pow(1.0 - num2, 0.9);
                        num = num2 * 0.93 + 0.05;
                        break;
                    }
                    case ModNet.NetState.Merging:
                        num = 0.99;
                        break;
                    case ModNet.NetState.Finished:
                    case ModNet.NetState.Canceled:
                        num = 1.0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("文件状态未知：" + Conversions.ToString((int)this.State));
                    }
                    return num;
                }
            }

            private int ConnectAverage
            {
                get
                {
                    int connectCount = this.ConnectCount;
                    return checked((int)Math.Round((connectCount == 0) ? (-1.0) : ((double)this.ConnectTime / (double)connectCount)));
                }
            }

            public override bool Equals(object obj)
            {
                ModNet.NetFile netFile = obj as ModNet.NetFile;
                return netFile != null && this.Uuid == netFile.Uuid;
            }

            public NetFile(IEnumerable<string> Urls, string LocalPath, FileChecker Checker = null, bool SimulateBrowserHeaders = false)
            {
                this.Tasks = new ConcurrentList<ModNet.LoaderDownload>();
                this.FirstThreadSource = 0;
                this.SourcesOnce = new ConcurrentList<ModNet.NetSource>();
                this.Retried = false;
                this.LocalPath = null;
                this.LocalName = null;
                this.State = ModNet.NetState.WaitingToCheck;
                this.Ex = new List<Exception>();
                this.FileSize = -2L;
                this.IsUnknownSize = false;
                this.DownloadDone = 0L;
                this.SpeedLastTime = ModBase.GetTimeMs();
                this.SpeedLastDone = 0L;
                this._Speed = 0L;
                this.IsCopy = false;
                this.ConnectCount = 0;
                this.ConnectTime = 0L;
                this.LockState = RuntimeHelpers.GetObjectValue(new object());
                this.LockChain = RuntimeHelpers.GetObjectValue(new object());
                this.LockSource = RuntimeHelpers.GetObjectValue(new object());
                this.Uuid = ModBase.GetUuid();
                List<ModNet.NetSource> list = new List<ModNet.NetSource>();
                int num = 0;
                Urls = Urls.Distinct<string>().ToArray<string>();
                checked
                {
                    try
                    {
                        foreach (string text in Urls)
                        {
                            list.Add(new ModNet.NetSource
                            {
                                FailCount = 0,
                                Url = ModSecret.SecretCdnSign(text.ReplaceLineEndings("", false).Trim()),
                                Id = num,
                                IsFailed = false,
                                Ex = null
                            });
                            num++;
                        }
                    }
                    finally
                    {
                        IEnumerator<string> enumerator;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    this.Sources = list;
                    this.LocalPath = LocalPath;
                    this.Check = Checker;
                    this.SimulateBrowserHeaders = SimulateBrowserHeaders;
                    this.LocalName = PathUtils.GetLastPart(LocalPath);
                }
            }

            public ModNet.NetThread TryBeginThread()
            {
                checked
                {
                    ModNet.NetThread netThread6;
                    try
                    {
                        ModNet.NetSource netSource = null;
                        object lockSource = this.LockSource;
                        ObjectFlowControl.CheckForSyncLockOnValueType(lockSource);
                        lock (lockSource)
                        {
                            if (ModNet.NetTaskThreadCount >= ModNet.NetTaskThreadLimit || !this.HasAvailableSource(true) || (this.IsNoSplit && this.Threads != null && this.Threads.State != ModNet.NetState.Canceled))
                            {
                                return null;
                            }
                            if (this.State >= ModNet.NetState.Merging || this.State == ModNet.NetState.WaitingToCheck)
                            {
                                return null;
                            }
                            object lockState = this.LockState;
                            ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                            lock (lockState)
                            {
                                if (this.State < ModNet.NetState.Connecting)
                                {
                                    this.State = ModNet.NetState.Connecting;
                                }
                            }
                            if (!this.IsNoSplit)
                            {
                                if (this.HasAvailableSource(false))
                                {
                                    goto IL_00F8;
                                }
                                if (this.SourcesOnce.First<ModNet.NetSource>().SingleThread != null && this.SourcesOnce.First<ModNet.NetSource>().SingleThread.State != ModNet.NetState.Canceled)
                                {
                                    return null;
                                }
                            }
                            this.Reset();
                            this.State = ModNet.NetState.Reading;
                            IL_00F8:;
                        }
                        object lockChain = this.LockChain;
                        ObjectFlowControl.CheckForSyncLockOnValueType(lockChain);
                        Thread thread;
                        ModNet.NetThread netThread4;
                        lock (lockChain)
                        {
                            long num;
                            if (this.Threads == null)
                            {
                                num = 0L;
                                netSource = this.GetSource(this.FirstThreadSource);
                                this.FirstThreadSource = netSource.Id + 1;
                            }
                            else
                            {
                                try
                                {
                                    foreach (ModNet.NetThread netThread in this.Threads)
                                    {
                                        if (netThread.State == ModNet.NetState.Canceled && netThread.DownloadUndone > 0L)
                                        {
                                            num = netThread.DownloadStart + netThread.DownloadDone;
                                            netSource = this.GetSource(netThread.Source.Id + 1);
                                            goto IL_02C2;
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<ModNet.NetThread> enumerator;
                                    if (enumerator != null)
                                    {
                                        enumerator.Dispose();
                                    }
                                }
                                ModNet.NetSource source = this.GetSource(0);
                                if (source == null)
                                {
                                    return null;
                                }
                                string url = source.Url;
                                if (url.Contains("pcl2-server") || url.Contains("meloong.com") || url.Contains("bmclapi") || url.Contains("github.com") || url.Contains("optifine.net") || url.Contains("momot.rs"))
                                {
                                    return null;
                                }
                                ModNet.NetThread netThread2 = this.Threads;
                                try
                                {
                                    foreach (ModNet.NetThread netThread3 in this.Threads)
                                    {
                                        if (netThread3.DownloadUndone > netThread2.DownloadUndone)
                                        {
                                            netThread2 = netThread3;
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<ModNet.NetThread> enumerator2;
                                    if (enumerator2 != null)
                                    {
                                        enumerator2.Dispose();
                                    }
                                }
                                if (netThread2 == null || netThread2.DownloadUndone < 262144L)
                                {
                                    return null;
                                }
                                num = (long)Math.Round(unchecked((double)netThread2.DownloadEnd - (double)netThread2.DownloadUndone * 0.4));
                                netSource = this.GetSource(0);
                            }
                            IL_02C2:
                            if ((num > this.FileSize && this.FileSize >= 0L && !this.IsUnknownSize) || num < 0L || Information.IsNothing(netSource))
                            {
                                return null;
                            }
                            int uuid = ModBase.GetUuid();
                            if (!this.Tasks.Any<ModNet.LoaderDownload>())
                            {
                                return null;
                            }
                            thread = new Thread(delegate(object a0)
                            {
                                this.Thread((ModNet.NetThread)a0);
                            })
                            {
                                Name = string.Format("下载 {0}/{1}/{2}#", this.Tasks[0].Uuid, this.Uuid, uuid),
                                Priority = ThreadPriority.BelowNormal
                            };
                            netThread4 = new ModNet.NetThread
                            {
                                Uuid = uuid,
                                DownloadStart = num,
                                Thread = thread,
                                Source = netSource,
                                Task = this,
                                State = ModNet.NetState.WaitingToDownload
                            };
                            if (netThread4.IsFirstThread || this.Threads == null)
                            {
                                this.Threads = netThread4;
                            }
                            else
                            {
                                ModNet.NetThread netThread5 = this.Threads;
                                while (netThread5.DownloadEnd <= num)
                                {
                                    netThread5 = netThread5.NextThread;
                                }
                                netThread4.NextThread = netThread5.NextThread;
                                netThread5.NextThread = netThread4;
                            }
                        }
                        Interlocked.Increment(ref ModNet.NetTaskThreadCount);
                        object lockSource2 = this.LockSource;
                        ObjectFlowControl.CheckForSyncLockOnValueType(lockSource2);
                        lock (lockSource2)
                        {
                            if (!this.HasAvailableSource(false))
                            {
                                this.SourcesOnce.First<ModNet.NetSource>().SingleThread = netThread4;
                            }
                        }
                        thread.Start(netThread4);
                        netThread6 = netThread4;
                    }
                    catch (Exception ex)
                    {
                        this.Fail(new Exception(string.Format("尝试开始下载线程失败（{0}）", this.LocalName), ex));
                        netThread6 = null;
                    }
                    return netThread6;
                }
            }

            private void Thread(ModNet.NetThread Th)
            {
                ModNet.NetFile._Closure$__40-0 CS$<>8__locals1 = new ModNet.NetFile._Closure$__40-0(CS$<>8__locals1);
                CS$<>8__locals1.$VB$Me = this;
                CS$<>8__locals1.$VB$Local_Th = Th;
                Logger.Log(string.Format("{0}：开始，起始点 {1}，{2}", this.LocalName, CS$<>8__locals1.$VB$Local_Th.DownloadStart, CS$<>8__locals1.$VB$Local_Th.Source.Url), (CS$<>8__locals1.$VB$Local_Th.DownloadStart == 0L) ? LogLevel.Info : LogLevel.Trace, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                Stream stream = null;
                HttpRequestMessage httpRequestMessage = null;
                CS$<>8__locals1.$VB$Local_Response = null;
                Stream stream2 = null;
                CancellationTokenSource cancellationTokenSource = null;
                checked
                {
                    int num = Math.Min(Math.Max(this.ConnectAverage, 15000) * (1 + CS$<>8__locals1.$VB$Local_Th.Source.FailCount), 30000);
                    CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Connecting;
                    try
                    {
                        int num2 = 0;
                        long num3;
                        if (!this.SourcesOnce.Contains(CS$<>8__locals1.$VB$Local_Th.Source) || !(CS$<>8__locals1.$VB$Local_Th != CS$<>8__locals1.$VB$Local_Th.Source.SingleThread))
                        {
                            httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, CS$<>8__locals1.$VB$Local_Th.Source.Url);
                            ModSecret.SecretHeadersSign(CS$<>8__locals1.$VB$Local_Th.Source.Url, ref httpRequestMessage, this.SimulateBrowserHeaders);
                            cancellationTokenSource = new CancellationTokenSource(num);
                            if (!CS$<>8__locals1.$VB$Local_Th.IsFirstThread)
                            {
                                httpRequestMessage.Headers.Range = new RangeHeaderValue(new long?(CS$<>8__locals1.$VB$Local_Th.DownloadStart), null);
                            }
                            num3 = 0L;
                            CS$<>8__locals1.$VB$Local_Response = ModNet.ThreadClient.SendAsync(httpRequestMessage, 1, cancellationTokenSource.Token).GetResultWithTimeout<HttpResponseMessage>(cancellationTokenSource, num);
                            if (!CS$<>8__locals1.$VB$Local_Response.IsSuccessStatusCode)
                            {
                                throw new HttpRequestCodeException(string.Format("错误码 {0} ({1})，{2}", CS$<>8__locals1.$VB$Local_Response.StatusCode, (int)CS$<>8__locals1.$VB$Local_Response.StatusCode, CS$<>8__locals1.$VB$Local_Th.Source.Url), CS$<>8__locals1.$VB$Local_Response.StatusCode, null);
                            }
                            if (this.State != ModNet.NetState.Canceled)
                            {
                                if (Operators.CompareString(CS$<>8__locals1.$VB$Local_Response.RequestMessage.RequestUri.ToString(), CS$<>8__locals1.$VB$Local_Th.Source.Url, false) != 0)
                                {
                                    Logger.Trace(() => string.Format("{0}：重定向至 {1}", CS$<>8__locals1.$VB$Me.LocalName, CS$<>8__locals1.$VB$Local_Response.RequestMessage.RequestUri), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                }
                                num3 = CS$<>8__locals1.$VB$Local_Response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
                                if (num3 < 0L)
                                {
                                    if (this.FileSize <= 1L)
                                    {
                                        this.FileSize = -1L;
                                        this.IsUnknownSize = true;
                                        Logger.Info(string.Format("{0}：文件大小未知", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                        goto IL_0625;
                                    }
                                    if (CS$<>8__locals1.$VB$Local_Th.DownloadStart == 0L)
                                    {
                                        Logger.Info(string.Format("{0}：文件大小未知，但已从其他下载源获取，不作处理", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                        goto IL_0625;
                                    }
                                    Logger.Info(string.Format("{0}：ContentLength 返回了 {1}，无法确定是否支持分段下载，视作不支持", this.LocalName, num3), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                }
                                else if (CS$<>8__locals1.$VB$Local_Th.IsFirstThread)
                                {
                                    if (this.Check != null)
                                    {
                                        if (num3 < this.Check.MinSize && this.Check.MinSize > 0L)
                                        {
                                            throw new Exception(string.Format("文件大小不足，获取结果为 {0}，要求至少为 {1}。", num3, this.Check.MinSize));
                                        }
                                        if (num3 != this.Check.ActualSize && this.Check.ActualSize > 0L)
                                        {
                                            throw new Exception(string.Format("文件大小不一致，获取结果为 {0}，要求必须为 {1}。", num3, this.Check.ActualSize));
                                        }
                                    }
                                    this.FileSize = num3;
                                    this.IsUnknownSize = false;
                                    Logger.Info(string.Format("{0}：文件大小 {1}（{2}）", this.LocalName, num3, StringUtils.FormatByteSize(num3)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                    if (num3 > 52428800L)
                                    {
                                        foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
                                        {
                                            string text = driveInfo.Name.First<char>().ToString();
                                            double num4;
                                            unchecked
                                            {
                                                num4 = (ModBase.PathTemp.StartsWithF(text, false) ? ((double)num3 * 1.1) : 0.0) + (double)(this.LocalPath.StartsWithF(text, false) ? checked(num3 + 5242880L) : 0L);
                                            }
                                            if (num4 > 0.0 && (double)driveInfo.TotalFreeSpace < num4)
                                            {
                                                throw new Exception(string.Concat(new string[]
                                                {
                                                    text,
                                                    " 盘空间不足，无法进行下载。\r\n需要至少 ",
                                                    StringUtils.FormatByteSize((long)Math.Round(num4)),
                                                    " 空间，但当前仅剩余 ",
                                                    StringUtils.FormatByteSize(driveInfo.TotalFreeSpace),
                                                    "。",
                                                    ModBase.PathTemp.StartsWithF(text, false) ? "\r\n\r\n下载时需要与文件同等大小的空间存放缓存，你可以在设置中调整缓存文件夹的位置。" : ""
                                                }));
                                            }
                                        }
                                        goto IL_0625;
                                    }
                                    goto IL_0625;
                                }
                                else
                                {
                                    if (this.FileSize < 0L)
                                    {
                                        throw new Exception("非首线程运行时，尚未获取文件大小");
                                    }
                                    if (CS$<>8__locals1.$VB$Local_Th.DownloadStart <= 0L || num3 < this.FileSize)
                                    {
                                        if (this.FileSize - CS$<>8__locals1.$VB$Local_Th.DownloadStart != num3)
                                        {
                                            throw new ModNet.RangeNotSupportedException(string.Format("获取到的分段大小不一致：Range 起始于 {0}，预期 ContentLength 为 {1}，返回 ContentLength 为 {2}，总文件大小 {3}", new object[]
                                            {
                                                CS$<>8__locals1.$VB$Local_Th.DownloadStart,
                                                this.FileSize - CS$<>8__locals1.$VB$Local_Th.DownloadStart,
                                                num3,
                                                this.FileSize
                                            }));
                                        }
                                        goto IL_0625;
                                    }
                                }
                                object lockSource = this.LockSource;
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockSource);
                                lock (lockSource)
                                {
                                    if (this.SourcesOnce.Contains(CS$<>8__locals1.$VB$Local_Th.Source))
                                    {
                                        goto IL_0A31;
                                    }
                                }
                                throw new ModNet.RangeNotSupportedException(string.Format("该下载源不支持分段下载：Range 起始于 {0}，预期 ContentLength 为 {1}，返回 ContentLength 为 {2}，总文件大小 {3}", new object[]
                                {
                                    CS$<>8__locals1.$VB$Local_Th.DownloadStart,
                                    this.FileSize - CS$<>8__locals1.$VB$Local_Th.DownloadStart,
                                    num3,
                                    this.FileSize
                                }));
                                IL_0625:
                                CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Reading;
                                object lockState = this.LockState;
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                                lock (lockState)
                                {
                                    if (this.State < ModNet.NetState.Reading)
                                    {
                                        this.State = ModNet.NetState.Reading;
                                    }
                                }
                                if (this.IsNoSplit)
                                {
                                    CS$<>8__locals1.$VB$Local_Th.Temp = null;
                                    this.Cache = new MemoryStream();
                                    stream = this.Cache;
                                }
                                else
                                {
                                    CS$<>8__locals1.$VB$Local_Th.Temp = string.Format("{0}Download\\{1}_{2}_{3}.tmp", new object[]
                                    {
                                        ModBase.PathTemp,
                                        this.Uuid,
                                        CS$<>8__locals1.$VB$Local_Th.Uuid,
                                        ModBase.RandomInteger(0, 999999)
                                    });
                                    stream = FileUtils.CreateAsStream(CS$<>8__locals1.$VB$Local_Th.Temp);
                                }
                                cancellationTokenSource.Dispose();
                                cancellationTokenSource = new CancellationTokenSource();
                                stream2 = CS$<>8__locals1.$VB$Local_Response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                                if (Settings.Get<bool>("SystemDebugDelay", null))
                                {
                                    global::System.Threading.Thread.Sleep(ModBase.RandomInteger(50, 3000));
                                }
                                byte[] array = new byte[16385];
                                num2 = stream2.ReadAsync(array, 0, 16384, cancellationTokenSource.Token).GetResultWithTimeout<int>(cancellationTokenSource, num);
                                while ((this.IsUnknownSize || CS$<>8__locals1.$VB$Local_Th.DownloadUndone > 0L) && num2 > 0 && !ModBase.IsProgramEnding && this.State < ModNet.NetState.Merging && (!CS$<>8__locals1.$VB$Local_Th.Source.IsFailed || CS$<>8__locals1.$VB$Local_Th.Source.SingleThread == CS$<>8__locals1.$VB$Local_Th))
                                {
                                    while (ModNet.NetTaskSpeedLimitHigh > 0L && ModNet.NetTaskSpeedLimitLeft <= 0L)
                                    {
                                        global::System.Threading.Thread.Sleep(16);
                                    }
                                    int num5 = (int)(unchecked(this.IsUnknownSize ? ((long)num2) : Math.Min((long)num2, CS$<>8__locals1.$VB$Local_Th.DownloadUndone)));
                                    unchecked
                                    {
                                        if (ModNet.NetTaskSpeedLimitHigh > 0L)
                                        {
                                            Interlocked.Add(ref ModNet.NetTaskSpeedLimitLeft, (long)(checked(0 - num5)));
                                        }
                                        long num6 = checked(ModBase.GetTimeMs() - CS$<>8__locals1.$VB$Local_Th.LastReceiveTime);
                                        if (num6 > 100000000L)
                                        {
                                            num6 = 1L;
                                        }
                                        if (num5 > 0)
                                        {
                                            if (CS$<>8__locals1.$VB$Local_Th.DownloadDone == 0L)
                                            {
                                                CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Downloading;
                                                object lockState2 = this.LockState;
                                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState2);
                                                lock (lockState2)
                                                {
                                                    if (this.State < ModNet.NetState.Downloading)
                                                    {
                                                        this.State = ModNet.NetState.Downloading;
                                                    }
                                                }
                                                Interlocked.Increment(ref this.ConnectCount);
                                                Interlocked.Add(ref this.ConnectTime, checked(ModBase.GetTimeMs() - CS$<>8__locals1.$VB$Local_Th.InitTime));
                                            }
                                            CS$<>8__locals1.$VB$Local_Th.Source.FailCount = 0;
                                            try
                                            {
                                                foreach (ModNet.LoaderDownload loaderDownload in this.Tasks)
                                                {
                                                    loaderDownload.FailCount = 0;
                                                }
                                            }
                                            finally
                                            {
                                                IEnumerator<ModNet.LoaderDownload> enumerator;
                                                if (enumerator != null)
                                                {
                                                    enumerator.Dispose();
                                                }
                                            }
                                            Interlocked.Add(ref ModNet.NetManager.DownloadDone, (long)num5);
                                            Interlocked.Add(ref this.DownloadDone, (long)num5);
                                            ModNet.NetThread $VB$Local_Th = CS$<>8__locals1.$VB$Local_Th;
                                            ref long ptr = ref $VB$Local_Th.DownloadDone;
                                            checked
                                            {
                                                $VB$Local_Th.DownloadDone = ptr + unchecked((long)num5);
                                                stream.Write(array, 0, num5);
                                                if (CS$<>8__locals1.$VB$Local_Th.DownloadUndone == 0L && !this.IsUnknownSize)
                                                {
                                                    break;
                                                }
                                            }
                                            if (CS$<>8__locals1.$VB$Local_Th.LastReceiveTime > 0L && num6 > 5000L && num6 > (long)num5 && CS$<>8__locals1.$VB$Local_Th.Source.SingleThread == null)
                                            {
                                                throw new TimeoutException(string.Concat(new string[]
                                                {
                                                    "由于速度过慢断开链接，下载 ",
                                                    Conversions.ToString(num5),
                                                    " B，消耗 ",
                                                    Conversions.ToString(num6),
                                                    " ms。"
                                                }));
                                            }
                                            CS$<>8__locals1.$VB$Local_Th.LastReceiveTime = ModBase.GetTimeMs();
                                        }
                                        else if (CS$<>8__locals1.$VB$Local_Th.LastReceiveTime > 0L && num6 > (long)num)
                                        {
                                            throw new TimeoutException("操作超时，无数据。");
                                        }
                                        num2 = stream2.ReadAsync(array, 0, 16384, cancellationTokenSource.Token).GetResultWithTimeout<int>(cancellationTokenSource, num);
                                    }
                                }
                            }
                        }
                        IL_0A31:
                        if (this.State == ModNet.NetState.Canceled || (CS$<>8__locals1.$VB$Local_Th.Source.IsFailed && CS$<>8__locals1.$VB$Local_Th.Source.SingleThread != CS$<>8__locals1.$VB$Local_Th) || (CS$<>8__locals1.$VB$Local_Th.DownloadUndone > 0L && !this.IsUnknownSize))
                        {
                            CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Canceled;
                            Logger.Info(string.Format("{0}：中断", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        }
                        else
                        {
                            if (num2 == 0 && CS$<>8__locals1.$VB$Local_Th.DownloadUndone > 0L && !this.IsUnknownSize)
                            {
                                throw new Exception(string.Format("返回的 ContentLength 过多：ContentLength 为 {0}，但获取到的总数据量仅为 {1}（全文件总数据量 {2}）", num3, CS$<>8__locals1.$VB$Local_Th.DownloadDone, this.DownloadDone));
                            }
                            CS$<>8__locals1.$VB$Local_Th.State = ModNet.NetState.Finished;
                            Logger.Trace(() => string.Format("{0}：完成，已下载 {1}（{2}~{3}）", new object[]
                            {
                                CS$<>8__locals1.$VB$Me.LocalName,
                                CS$<>8__locals1.$VB$Local_Th.DownloadDone,
                                CS$<>8__locals1.$VB$Local_Th.DownloadStart,
                                CS$<>8__locals1.$VB$Local_Th.DownloadEnd
                            }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsBadNetwork() || ex is TaskCanceledException)
                        {
                            Logger.Info(new TimeoutException(string.Format("{0}：超时（{1}ms），{2}", this.LocalName, num, ex.Message)), null, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        }
                        else
                        {
                            Logger.Info(ex, string.Format("{0}：出错", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        }
                        this.SourceFail(CS$<>8__locals1.$VB$Local_Th, ex, false);
                    }
                    finally
                    {
                        if (httpRequestMessage != null)
                        {
                            httpRequestMessage.Dispose();
                        }
                        HttpResponseMessage $VB$Local_Response = CS$<>8__locals1.$VB$Local_Response;
                        if ($VB$Local_Response != null)
                        {
                            $VB$Local_Response.Dispose();
                        }
                        if (stream2 != null)
                        {
                            stream2.Dispose();
                        }
                        if (cancellationTokenSource != null)
                        {
                            cancellationTokenSource.Dispose();
                        }
                        if (!this.IsNoSplit && stream != null)
                        {
                            stream.Dispose();
                        }
                        Interlocked.Decrement(ref ModNet.NetTaskThreadCount);
                        if (((this.FileSize >= 0L && this.DownloadDone >= this.FileSize) || (this.FileSize == -1L && this.DownloadDone > 0L)) && this.State < ModNet.NetState.Merging && CS$<>8__locals1.$VB$Local_Th.State != ModNet.NetState.Canceled)
                        {
                            this.Merge(CS$<>8__locals1.$VB$Local_Th);
                        }
                    }
                }
            }

            private void SourceFail(ModNet.NetThread Th, Exception ex, bool IsMergeFailure)
            {
                Interlocked.Increment(ref Th.Source.FailCount);
                try
                {
                    foreach (ModNet.LoaderDownload loaderDownload in this.Tasks)
                    {
                        int failCount = loaderDownload.FailCount;
                        Interlocked.Increment(ref failCount);
                        loaderDownload.FailCount = failCount;
                    }
                }
                finally
                {
                    IEnumerator<ModNet.LoaderDownload> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                Th.State = ModNet.NetState.Canceled;
                Th.Source.Ex = ex;
                bool flag = ex is ModNet.RangeNotSupportedException || ex.Message.Contains("(416)");
                if (IsMergeFailure || flag || ex.Message.Contains("(502)") || ex.Message.Contains("(404)") || ex.Message.Contains("未能解析") || ex.Message.Contains("无返回数据") || ex.Message.Contains("空间不足") || ((ex.Message.Contains("(403)") || ex.Message.Contains("(429)")) && !Th.Source.Url.Contains("bmclapi")) || (Th.Source.FailCount >= ModNet.NetTaskThreadLimit.Clamp(5, 30) && this.DownloadDone < 1L) || Th.Source.FailCount > checked(ModNet.NetTaskThreadLimit + 2))
                {
                    bool flag2 = false;
                    object lockSource = this.LockSource;
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockSource);
                    lock (lockSource)
                    {
                        if (!Th.Source.IsFailed || Th.Source.SingleThread == Th)
                        {
                            flag2 = true;
                            Th.Source.IsFailed = true;
                        }
                    }
                    if (flag2)
                    {
                        Logger.Info(string.Format("{0}：下载源被禁用（{1}，Range 问题：{2}）：{3}", new object[]
                        {
                            this.LocalName,
                            Th.Source.Id,
                            flag,
                            Th.Source.Url
                        }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        string text = "{0}下载源 {1} 已被禁用";
                        ModNet.NetSource netSource = this.SourcesOnce.FirstOrDefault<ModNet.NetSource>();
                        Logger.Warn(ex, string.Format(text, (((netSource != null) ? netSource.SingleThread : null) == null) ? "" : "单线程", Th.Source.Id), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        object lockSource2 = this.LockSource;
                        ObjectFlowControl.CheckForSyncLockOnValueType(lockSource2);
                        lock (lockSource2)
                        {
                            this.SourcesOnce.Remove(Th.Source);
                        }
                        if (ex.Message.Contains("空间不足"))
                        {
                            this.Fail(ex);
                        }
                        else if (!this.HasAvailableSource(true) || IsMergeFailure)
                        {
                            if (!this.Retried)
                            {
                                if (!flag)
                                {
                                    Logger.Warn(string.Format("{0}：文件下载失败，正在自动重试……", this.LocalName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                }
                                this.Retried = true;
                                object lockSource3 = this.LockSource;
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockSource3);
                                lock (lockSource3)
                                {
                                    this.SourcesOnce.Clear();
                                    try
                                    {
                                        foreach (ModNet.NetSource netSource2 in this.Sources)
                                        {
                                            this.SourcesOnce.Add(netSource2);
                                            netSource2.IsFailed = true;
                                        }
                                    }
                                    finally
                                    {
                                        List<ModNet.NetSource>.Enumerator enumerator2;
                                        ((IDisposable)enumerator2).Dispose();
                                    }
                                }
                                this.Reset();
                                object lockState = this.LockState;
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                                lock (lockState)
                                {
                                    this.State = ModNet.NetState.WaitingToDownload;
                                    goto IL_04AE;
                                }
                            }
                            if (this.HasAvailableSource(true) && IsMergeFailure)
                            {
                                this.Reset();
                                object lockState2 = this.LockState;
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState2);
                                lock (lockState2)
                                {
                                    this.State = ModNet.NetState.WaitingToDownload;
                                    goto IL_04AE;
                                }
                            }
                            Logger.Info(string.Format("{0}：已无可用下载源，下载失败", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            Exception ex2 = null;
                            object lockSource4 = this.LockSource;
                            ObjectFlowControl.CheckForSyncLockOnValueType(lockSource4);
                            lock (lockSource4)
                            {
                                try
                                {
                                    foreach (ModNet.NetSource netSource3 in this.Sources)
                                    {
                                        Logger.Info(string.Format("已禁用的下载源：{0}", netSource3.Url), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                        if (netSource3.Ex != null)
                                        {
                                            ex2 = netSource3.Ex;
                                            Logger.Warn(netSource3.Ex, "下载源禁用原因", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                        }
                                    }
                                }
                                finally
                                {
                                    List<ModNet.NetSource>.Enumerator enumerator3;
                                    ((IDisposable)enumerator3).Dispose();
                                }
                            }
                            this.Fail(ex2);
                        }
                    }
                }
                IL_04AE:
                if (this.FileSize == -2L)
                {
                    this.Reset();
                }
            }

            private void Reset()
            {
                this.FileSize = -2L;
                MemoryStream cache = this.Cache;
                if (cache != null)
                {
                    cache.Dispose();
                }
                this.Cache = null;
                object lockChain = this.LockChain;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockChain);
                lock (lockChain)
                {
                    this.Threads = null;
                }
                Interlocked.Add(ref ModNet.NetManager.DownloadDone, checked(0L - this.DownloadDone));
                this.DownloadDone = 0L;
                this.SpeedLastDone = 0L;
            }

            private void Merge(ModNet.NetThread Th)
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (this.State >= ModNet.NetState.Merging)
                    {
                        return;
                    }
                    this.State = ModNet.NetState.Merging;
                }
                int num = 0;
                checked
                {
                    try
                    {
                        IL_003A:
                        DirectoryUtils.Create(PathUtils.RemoveLastPart(this.LocalPath));
                        FileUtils.Delete(this.LocalPath, false);
                        object lockChain = this.LockChain;
                        ObjectFlowControl.CheckForSyncLockOnValueType(lockChain);
                        lock (lockChain)
                        {
                            if (this.IsNoSplit)
                            {
                                Logger.Trace(() => string.Format("{0}：下载结束，从内存输出文件", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                FileUtils.Write(this.LocalPath, this.Cache);
                            }
                            else if (this.Threads.DownloadDone == this.DownloadDone && this.Threads.Temp != null)
                            {
                                Logger.Trace(() => string.Format("{0}：下载结束，仅有一个文件，无需合并", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                FileUtils.Copy(this.Threads.Temp, this.LocalPath);
                            }
                            else
                            {
                                Logger.Trace(() => string.Format("{0}：下载结束，开始合并文件", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                using (FileStream fileStream = FileUtils.CreateAsStream(this.LocalPath))
                                {
                                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                                    {
                                        try
                                        {
                                            foreach (ModNet.NetThread netThread in this.Threads)
                                            {
                                                if (netThread.DownloadDone != 0L && netThread.Temp != null)
                                                {
                                                    using (Stream stream = FileUtils.ReadAsStream(netThread.Temp, null))
                                                    {
                                                        using (BinaryReader binaryReader = new BinaryReader(stream))
                                                        {
                                                            binaryWriter.Write(binaryReader.ReadBytes((int)netThread.DownloadDone));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            IEnumerator<ModNet.NetThread> enumerator;
                                            if (enumerator != null)
                                            {
                                                enumerator.Dispose();
                                            }
                                        }
                                    }
                                }
                            }
                            FileChecker check = this.Check;
                            string text = ((check != null) ? check.Check(this.LocalPath) : null);
                            if (text == null && !this.IsUnknownSize && this.Check != null)
                            {
                                if (this.Check.ActualSize == -1L)
                                {
                                    text = new FileChecker
                                    {
                                        ActualSize = this.FileSize
                                    }.Check(this.LocalPath);
                                }
                                else if (this.Check.ActualSize != this.FileSize)
                                {
                                    text = string.Format("文件大小不一致：任务校验要求为 {0}，请求结果为 {1}", this.Check.ActualSize, this.FileSize);
                                }
                            }
                            if (text != null)
                            {
                                Logger.Info(string.Format("{0} 文件校验失败，下载线程细节：", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                try
                                {
                                    foreach (ModNet.NetThread netThread2 in this.Threads)
                                    {
                                        Logger.Info(string.Format("- {0}#，状态 {1}，范围 {2}~{3}，已下载 {4}，未下载 {5}", new object[] { netThread2.Uuid, netThread2.State, netThread2.DownloadStart, netThread2.DownloadEnd, netThread2.DownloadDone, netThread2.DownloadUndone }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                                    }
                                }
                                finally
                                {
                                    IEnumerator<ModNet.NetThread> enumerator2;
                                    if (enumerator2 != null)
                                    {
                                        enumerator2.Dispose();
                                    }
                                }
                                throw new Exception(text);
                            }
                            if (!this.IsNoSplit)
                            {
                                try
                                {
                                    foreach (ModNet.NetThread netThread3 in this.Threads)
                                    {
                                        if (netThread3.Temp != null)
                                        {
                                            FileUtils.Delete(netThread3.Temp, false);
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<ModNet.NetThread> enumerator3;
                                    if (enumerator3 != null)
                                    {
                                        enumerator3.Dispose();
                                    }
                                }
                            }
                            this.Finish(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        num++;
                        if (this.State <= ModNet.NetState.Merging)
                        {
                            Logger.Warn(ex, string.Format("合并文件出错，第 {0} 次尝试（{1}）", num, this.LocalName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            if (num < 3)
                            {
                                global::System.Threading.Thread.Sleep(500 * num);
                                goto IL_003A;
                            }
                            try
                            {
                                FileUtils.Delete(this.LocalPath, false);
                            }
                            catch (Exception ex2)
                            {
                                Logger.Warn(ex, string.Format("删除合并失败的文件出错（{0}）", this.LocalPath), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            }
                            this.SourceFail(Th, ex, true);
                        }
                    }
                    finally
                    {
                        MemoryStream cache = this.Cache;
                        if (cache != null)
                        {
                            cache.Dispose();
                        }
                    }
                }
            }

            private void Fail(Exception RaiseEx = null)
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (this.State >= ModNet.NetState.Finished)
                    {
                        return;
                    }
                    if (RaiseEx != null)
                    {
                        this.Ex.Add(RaiseEx);
                    }
                    Logger.Info(string.Format("{0}：已失败，当前状态 {1}", this.LocalName, this.State), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                    this.State = ModNet.NetState.Canceled;
                }
                this.CancelInternal();
                try
                {
                    foreach (ModNet.LoaderDownload loaderDownload in this.Tasks)
                    {
                        loaderDownload.OnFileFail(this);
                    }
                }
                finally
                {
                    IEnumerator<ModNet.LoaderDownload> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }

            public void Cancel(ModNet.LoaderDownload CanceledTask)
            {
                this.Tasks.Remove(CanceledTask);
                if (!this.Tasks.Any<ModNet.LoaderDownload>())
                {
                    object lockState = this.LockState;
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                    lock (lockState)
                    {
                        if (this.State >= ModNet.NetState.Finished)
                        {
                            return;
                        }
                        Logger.Info(string.Format("{0}：已中断，当前状态 {1}", this.LocalName, this.State), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        this.State = ModNet.NetState.Canceled;
                    }
                    this.CancelInternal();
                }
            }

            private void CancelInternal()
            {
                int num;
                int num4;
                object obj;
                try
                {
                    IL_0000:
                    ProjectData.ClearProjectError();
                    num = 1;
                    IL_0007:
                    int num2 = 2;
                    this.Reset();
                    IL_000F:
                    num2 = 3;
                    Interlocked.Decrement(ref ModNet.NetManager.FileRemain);
                    IL_0021:
                    goto IL_0080;
                    IL_0023:
                    int num3 = num4 + 1;
                    num4 = 0;
                    @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3);
                    IL_0041:
                    goto IL_0075;
                    IL_0043:
                    num4 = num2;
                    @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
                    IL_0053:;
                }
                catch when (endfilter((obj is Exception) & (num != 0) & (num4 == 0)))
                {
                    Exception ex = (Exception)obj2;
                    goto IL_0043;
                }
                IL_0075:
                throw ProjectData.CreateProjectError(-2146828237);
                IL_0080:
                if (num4 != 0)
                {
                    ProjectData.ClearProjectError();
                }
            }

            public void Finish(bool PrintLog = true)
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (this.State >= ModNet.NetState.Finished)
                    {
                        return;
                    }
                    this.State = ModNet.NetState.Finished;
                }
                Interlocked.Decrement(ref ModNet.NetManager.FileRemain);
                if (PrintLog)
                {
                    Logger.Info(string.Format("{0}：已完成", this.LocalName), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                }
                try
                {
                    foreach (ModNet.LoaderDownload loaderDownload in this.Tasks)
                    {
                        loaderDownload.OnFileFinish(this);
                    }
                }
                finally
                {
                    IEnumerator<ModNet.LoaderDownload> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }

            public ConcurrentList<ModNet.LoaderDownload> Tasks;

            public List<ModNet.NetSource> Sources;

            private int FirstThreadSource;

            public ConcurrentList<ModNet.NetSource> SourcesOnce;

            private bool Retried;

            public string LocalPath;

            public string LocalName;

            public ModNet.NetState State;

            public List<Exception> Ex;

            public ModNet.NetThread Threads;

            public long FileSize;

            public bool IsUnknownSize;

            private MemoryStream Cache;

            public long DownloadDone;

            public FileChecker Check;

            public bool SimulateBrowserHeaders;

            private long SpeedLastTime;

            private long SpeedLastDone;

            private long _Speed;

            public bool IsCopy;

            private int ConnectCount;

            private long ConnectTime;

            private const long FilePieceLimit = 262144L;

            public readonly object LockState;

            public readonly object LockChain;

            public readonly object LockSource;

            public readonly int Uuid;
        }

        private class RangeNotSupportedException : HttpRequestException
        {
            public RangeNotSupportedException(string Message)
                : base(Message)
            {
            }
        }

        public class LoaderDownload : ModLoader.LoaderBase
        {
            public override double Progress
            {
                get
                {
                    double num;
                    if (base.State >= ModBase.LoadState.Finished)
                    {
                        num = 1.0;
                    }
                    else if (!this.Files.Any<ModNet.NetFile>())
                    {
                        num = 0.0;
                    }
                    else
                    {
                        num = this._Progress;
                    }
                    return num;
                }
                set
                {
                    throw new Exception("文件下载不允许指定进度");
                }
            }

            public int FailCount
            {
                get
                {
                    return this._FailCount;
                }
                set
                {
                    int num2;
                    int num4;
                    object obj;
                    try
                    {
                        IL_0000:
                        int num = 1;
                        this._FailCount = value;
                        IL_0009:
                        num = 2;
                        if (base.State != ModBase.LoadState.Loading || value < Math.Min(10000, checked(Math.Max(this.FileRemain * 8, ModNet.NetTaskThreadLimit * 8 + 3))))
                        {
                            goto IL_0106;
                        }
                        IL_003D:
                        num = 3;
                        Logger.Error(string.Format("由于同加载器中失败次数过多引发强制失败：连续失败了 {0} 次", value), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        IL_005A:
                        ProjectData.ClearProjectError();
                        num2 = 1;
                        IL_0061:
                        num = 5;
                        List<Exception> list = new List<Exception>();
                        IL_0069:
                        num = 6;
                        IEnumerator<ModNet.NetFile> enumerator = this.Files.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ModNet.NetFile netFile = enumerator.Current;
                            IL_0083:
                            num = 7;
                            List<ModNet.NetSource>.Enumerator enumerator2 = netFile.Sources.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                ModNet.NetSource netSource = enumerator2.Current;
                                IL_009E:
                                num = 8;
                                if (netSource.Ex != null)
                                {
                                    IL_00A9:
                                    num = 9;
                                    list.Add(netSource.Ex);
                                    IL_00B9:
                                    num = 10;
                                    if (list.Count > 10)
                                    {
                                        goto IL_00FC;
                                    }
                                }
                                IL_00C6:
                                num = 12;
                            }
                            IL_00D2:
                            num = 13;
                            ((IDisposable)enumerator2).Dispose();
                            IL_00E2:
                            num = 14;
                            continue;
                            IL_00FC:
                            num = 16;
                            this.OnFail(list);
                            goto IL_0106;
                        }
                        IL_00EE:
                        num = 15;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                            goto IL_00FC;
                        }
                        goto IL_00FC;
                        IL_0106:
                        goto IL_019C;
                        IL_010B:
                        int num3 = num4 + 1;
                        num4 = 0;
                        @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3);
                        IL_015D:
                        goto IL_0191;
                        IL_015F:
                        num4 = num;
                        @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num2);
                        IL_016F:;
                    }
                    catch when (endfilter((obj is Exception) & (num2 != 0) & (num4 == 0)))
                    {
                        Exception ex = (Exception)obj2;
                        goto IL_015F;
                    }
                    IL_0191:
                    throw ProjectData.CreateProjectError(-2146828237);
                    IL_019C:
                    if (num4 != 0)
                    {
                        ProjectData.ClearProjectError();
                    }
                }
            }

            public void RefreshStat()
            {
                double num = 0.0;
                double num2 = 0.0;
                try
                {
                    foreach (ModNet.NetFile netFile in this.Files)
                    {
                        if (netFile.IsCopy)
                        {
                            num += netFile.Progress * 0.2;
                            num2 += 0.2;
                        }
                        else
                        {
                            num += netFile.Progress;
                            num2 += 1.0;
                        }
                    }
                }
                finally
                {
                    IEnumerator<ModNet.NetFile> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                if (num2 > 0.0 && !double.IsNaN(num2))
                {
                    num /= num2;
                }
                this._Progress = num;
            }

            public LoaderDownload(string Name, IEnumerable<ModNet.NetFile> FileTasks)
            {
                this.FileRemainLock = RuntimeHelpers.GetObjectValue(new object());
                this._Progress = 0.0;
                this._FailCount = 0;
                this.Name = Name;
                this.Files = new ConcurrentQueue<ModNet.NetFile>(FileTasks);
            }

            public override void Start(object Input = null, bool IsForceRestart = false)
            {
                if (Input != null)
                {
                    this.Files = new ConcurrentQueue<ModNet.NetFile>((IEnumerable<ModNet.NetFile>)Input);
                }
                this.Files = new ConcurrentQueue<ModNet.NetFile>(this.Files.DistinctBy<ModNet.NetFile, string>((ModNet.LoaderDownload._Closure$__.$I13-0 == null) ? (ModNet.LoaderDownload._Closure$__.$I13-0 = (ModNet.NetFile a) => PathUtils.ForCompare(a.LocalPath).Lower()) : ModNet.LoaderDownload._Closure$__.$I13-0));
                object fileRemainLock = this.FileRemainLock;
                ObjectFlowControl.CheckForSyncLockOnValueType(fileRemainLock);
                checked
                {
                    lock (fileRemainLock)
                    {
                        ref int ptr = ref this.FileRemain;
                        this.FileRemain = ptr + this.Files.Where<ModNet.NetFile>((ModNet.LoaderDownload._Closure$__.$I13-1 == null) ? (ModNet.LoaderDownload._Closure$__.$I13-1 = (ModNet.NetFile f) => f.State != ModNet.NetState.Finished) : ModNet.LoaderDownload._Closure$__.$I13-1).Count<ModNet.NetFile>();
                    }
                    base.State = ModBase.LoadState.Loading;
                    ModBase.RunInNewThread(delegate
                    {
                        try
                        {
                            ModNet.LoaderDownload._Closure$__13-1 CS$<>8__locals1 = new ModNet.LoaderDownload._Closure$__13-1(CS$<>8__locals1);
                            CS$<>8__locals1.$VB$Me = this;
                            if (!this.Files.Any<ModNet.NetFile>())
                            {
                                this.OnFinish();
                            }
                            else
                            {
                                try
                                {
                                    foreach (ModNet.NetFile netFile in this.Files)
                                    {
                                        if (netFile == null)
                                        {
                                            throw new ArgumentException("存在空文件请求！");
                                        }
                                        try
                                        {
                                            foreach (ModNet.NetSource netSource in netFile.Sources)
                                            {
                                                if (!netSource.Url.StartsWithF("https://", true) && !netSource.Url.StartsWithF("http://", true))
                                                {
                                                    netSource.Ex = new ArgumentException("输入的下载链接不正确：" + netSource.Url);
                                                    netSource.IsFailed = true;
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            List<ModNet.NetSource>.Enumerator enumerator2;
                                            ((IDisposable)enumerator2).Dispose();
                                        }
                                        if (!netFile.HasAvailableSource(true))
                                        {
                                            throw new ArgumentException("输入的下载链接不正确！");
                                        }
                                        netFile.LocalPath = netFile.LocalPath.Replace("/", "\\");
                                        if (!netFile.LocalPath.Contains(":\\"))
                                        {
                                            throw new ArgumentException("输入的本地文件地址不正确：" + netFile.LocalPath);
                                        }
                                        if (netFile.LocalPath.EndsWithF("\\", false))
                                        {
                                            throw new ArgumentException("请输入含文件名的完整文件路径：" + netFile.LocalPath);
                                        }
                                        DirectoryUtils.Create(PathUtils.RemoveLastPart(netFile.LocalPath));
                                    }
                                }
                                finally
                                {
                                    IEnumerator<ModNet.NetFile> enumerator;
                                    if (enumerator != null)
                                    {
                                        enumerator.Dispose();
                                    }
                                }
                                List<ModNet.NetFile> list = ModNet.NetManager.Start(this);
                                List<ModNet.NetFile> list2 = new List<ModNet.NetFile>();
                                try
                                {
                                    foreach (ModNet.NetFile netFile2 in list)
                                    {
                                        if (netFile2.Check != null)
                                        {
                                            list2.Add(netFile2);
                                        }
                                        else
                                        {
                                            object lockState = this.LockState;
                                            ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                                            lock (lockState)
                                            {
                                                netFile2.State = ModNet.NetState.WaitingToDownload;
                                                netFile2.IsCopy = false;
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    List<ModNet.NetFile>.Enumerator enumerator3;
                                    ((IDisposable)enumerator3).Dispose();
                                }
                                if (list2.Any<ModNet.NetFile>())
                                {
                                    CS$<>8__locals1.$VB$Local_Folders = new List<string>();
                                    CS$<>8__locals1.$VB$Local_Folders.Add(Paths.AppData + ".minecraft\\");
                                    CS$<>8__locals1.$VB$Local_Folders.AddRange(ModMinecraft.McFolderList.Select<ModMinecraft.McFolder, string>((ModNet.LoaderDownload._Closure$__.$I13-3 == null) ? (ModNet.LoaderDownload._Closure$__.$I13-3 = (ModMinecraft.McFolder f) => f.Location) : ModNet.LoaderDownload._Closure$__.$I13-3));
                                    CS$<>8__locals1.$VB$Local_Folders = CS$<>8__locals1.$VB$Local_Folders.Distinct<string>().Where<string>((ModNet.LoaderDownload._Closure$__.$I13-4 == null) ? (ModNet.LoaderDownload._Closure$__.$I13-4 = (string f) => DirectoryUtils.Exists(f)) : ModNet.LoaderDownload._Closure$__.$I13-4).ToList<string>();
                                    int num = (list2.Count / 40).Clamp(1, 8);
                                    if (num == 1)
                                    {
                                        this.CheckExistingFiles(list2, CS$<>8__locals1.$VB$Local_Folders);
                                    }
                                    else
                                    {
                                        int num2 = list2.Count / num;
                                        int num3 = list2.Count % num;
                                        int num4 = 0;
                                        int num5 = num - 1;
                                        for (int i = 0; i <= num5; i++)
                                        {
                                            ModNet.LoaderDownload._Closure$__13-0 CS$<>8__locals2 = new ModNet.LoaderDownload._Closure$__13-0(CS$<>8__locals2);
                                            CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                                            int num6 = num2 + ((i < num3) ? 1 : 0);
                                            CS$<>8__locals2.$VB$Local_ThreadFiles = list2.GetRange(num4, num6);
                                            num4 += num6;
                                            ModBase.RunInNewThread(delegate
                                            {
                                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Me.CheckExistingFiles(CS$<>8__locals2.$VB$Local_ThreadFiles, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Folders);
                                            }, string.Format("下载 文件复制 {0}/{1}", this.Uuid, ModBase.GetUuid()), ThreadPriority.Normal);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this.OnFail(new List<Exception>
                            {
                                new Exception("下载初始化失败", ex)
                            });
                        }
                    }, "L/下载 " + Conversions.ToString(this.Uuid), ThreadPriority.Normal);
                }
            }

            private void CheckExistingFiles(List<ModNet.NetFile> Files, List<string> FolderList)
            {
                ModNet.LoaderDownload._Closure$__14-0 CS$<>8__locals1 = new ModNet.LoaderDownload._Closure$__14-0(CS$<>8__locals1);
                CS$<>8__locals1.$VB$Local_Files = Files;
                try
                {
                    Logger.Trace(delegate
                    {
                        string text3 = "文件检查开始，本线程负责 {0} 个文件，首个文件为 {1}";
                        object obj = CS$<>8__locals1.$VB$Local_Files.Count;
                        ModNet.NetFile netFile = CS$<>8__locals1.$VB$Local_Files.FirstOrDefault<ModNet.NetFile>();
                        return string.Format(text3, obj, (netFile != null) ? netFile.LocalName : null);
                    }, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                    List<string> list = new List<string>();
                    try
                    {
                        foreach (string text in FolderList)
                        {
                            try
                            {
                                foreach (string text2 in DirectoryUtils.EnumerateDirectories(text + "versions\\", false, "*"))
                                {
                                    list.Add(PathUtils.AddSlashSuffix(text2));
                                }
                            }
                            finally
                            {
                                IEnumerator<string> enumerator2;
                                if (enumerator2 != null)
                                {
                                    enumerator2.Dispose();
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                    try
                    {
                        List<ModNet.NetFile>.Enumerator enumerator3 = CS$<>8__locals1.$VB$Local_Files.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            ModNet.LoaderDownload._Closure$__14-1 CS$<>8__locals2 = new ModNet.LoaderDownload._Closure$__14-1(CS$<>8__locals2);
                            CS$<>8__locals2.$VB$Local_File = enumerator3.Current;
                            ModNet.LoaderDownload._Closure$__14-2 CS$<>8__locals3 = new ModNet.LoaderDownload._Closure$__14-2(CS$<>8__locals3);
                            CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals2;
                            CS$<>8__locals3.$VB$Local_Target = this.CheckExistingFile(FolderList, list, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File);
                            Logger.Trace(() => string.Format("本地文件匹配：{0} → {1}，当前状态 {2}", CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.LocalName, CS$<>8__locals3.$VB$Local_Target ?? "无匹配", CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.State), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            if (CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.State < ModNet.NetState.WaitingToDownload)
                            {
                                if (CS$<>8__locals3.$VB$Local_Target != null)
                                {
                                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.IsCopy = true;
                                    try
                                    {
                                        FileUtils.Copy(CS$<>8__locals3.$VB$Local_Target, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.LocalPath);
                                        CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.Finish(false);
                                        continue;
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                object lockState = this.LockState;
                                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                                lock (lockState)
                                {
                                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.State = ModNet.NetState.WaitingToDownload;
                                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_File.IsCopy = false;
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<ModNet.NetFile>.Enumerator enumerator3;
                        ((IDisposable)enumerator3).Dispose();
                    }
                }
                catch (Exception ex2)
                {
                    this.OnFail(new List<Exception>
                    {
                        new Exception("下载已存在文件查找失败", ex2)
                    });
                }
            }

            private string CheckExistingFile(List<string> FolderList, List<string> VersionFolders, ModNet.NetFile File)
            {
                ModNet.LoaderDownload._Closure$__15-0 CS$<>8__locals1 = new ModNet.LoaderDownload._Closure$__15-0(CS$<>8__locals1);
                CS$<>8__locals1.$VB$Local_File = File;
                string text;
                if (CS$<>8__locals1.$VB$Local_File.Check.Check(CS$<>8__locals1.$VB$Local_File.LocalPath) == null)
                {
                    text = CS$<>8__locals1.$VB$Local_File.LocalPath;
                }
                else if (Settings.Get<bool>("SystemDebugSkipCopy", null))
                {
                    text = null;
                }
                else if (CS$<>8__locals1.$VB$Local_File.Check.Hash == null && CS$<>8__locals1.$VB$Local_File.Check.ActualSize < 0L)
                {
                    text = null;
                }
                else
                {
                    List<ValueTuple<string, int>> list = new string[] { "\\assets\\", "\\libraries\\", "\\versions\\", "\\mods\\", "\\coremods\\", "\\lib\\", "\\resourcepacks\\", "\\texturepacks\\", "\\shaderpacks\\" }.Select<string, ValueTuple<string, int>>((string FolderName) => new ValueTuple<string, int>(FolderName, CS$<>8__locals1.$VB$Local_File.LocalPath.IndexOfF(FolderName, true))).Where<ValueTuple<string, int>>((ModNet.LoaderDownload._Closure$__.$I15-1 == null) ? (ModNet.LoaderDownload._Closure$__.$I15-1 = ([TupleElementNames(new string[] { "FolderName", null })] ValueTuple<string, int> kv) => kv.Item2 >= 0) : ModNet.LoaderDownload._Closure$__.$I15-1).ToList<ValueTuple<string, int>>();
                    if (!list.Any<ValueTuple<string, int>>())
                    {
                        if (Operators.CompareString(PathUtils.GetExtension(CS$<>8__locals1.$VB$Local_File.LocalName), "jar", false) != 0)
                        {
                            return null;
                        }
                        list.Add(new ValueTuple<string, int>("\\versions\\", 1));
                    }
                    string text2 = list.MaxBy<ValueTuple<string, int>, int>((ModNet.LoaderDownload._Closure$__.$I15-2 == null) ? (ModNet.LoaderDownload._Closure$__.$I15-2 = ([TupleElementNames(new string[] { "FolderName", null })] ValueTuple<string, int> kv) => kv.Item2) : ModNet.LoaderDownload._Closure$__.$I15-2).Item1.TrimStart(new char[] { '\\' });
                    if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init == null)
                    {
                        Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init, new StaticLocalInitFlag(), null);
                    }
                    bool flag = false;
                    try
                    {
                        Monitor.Enter(this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init, ref flag);
                        if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State == 0)
                        {
                            this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State = 2;
                            this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes = new ConcurrentDictionary<string, long>();
                        }
                        else if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State == 2)
                        {
                            throw new IncompleteInitialization();
                        }
                    }
                    finally
                    {
                        this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init.State = 1;
                        if (flag)
                        {
                            Monitor.Exit(this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init);
                        }
                    }
                    if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init == null)
                    {
                        Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init, new StaticLocalInitFlag(), null);
                    }
                    bool flag2 = false;
                    try
                    {
                        Monitor.Enter(this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init, ref flag2);
                        if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State == 0)
                        {
                            this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State = 2;
                            this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate = delegate(string Candidate, FileChecker Checker)
                            {
                                long orAdd = this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes.GetOrAdd(Candidate, (ModNet.LoaderDownload._Closure$__.$I15-4 == null) ? (ModNet.LoaderDownload._Closure$__.$I15-4 = delegate(string c)
                                {
                                    if (!FileUtils.Exists(c))
                                    {
                                        return -1L;
                                    }
                                    return FileUtils.GetInfo(c).Length;
                                }) : ModNet.LoaderDownload._Closure$__.$I15-4);
                                return Checker.ActualSize == orAdd && Checker.Check(Candidate) == null;
                            };
                        }
                        else if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State == 2)
                        {
                            throw new IncompleteInitialization();
                        }
                    }
                    finally
                    {
                        this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init.State = 1;
                        if (flag2)
                        {
                            Monitor.Exit(this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init);
                        }
                    }
                    if (Operators.CompareString(text2, "assets\\", false) != 0 && Operators.CompareString(text2, "libraries\\", false) != 0)
                    {
                        if (Operators.CompareString(text2, "versions\\", false) == 0)
                        {
                            try
                            {
                                foreach (string text3 in VersionFolders)
                                {
                                    try
                                    {
                                        foreach (string text4 in DirectoryUtils.EnumerateFiles(text3, false, "*." + PathUtils.GetExtension(CS$<>8__locals1.$VB$Local_File.LocalPath)))
                                        {
                                            if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate(text4, CS$<>8__locals1.$VB$Local_File.Check))
                                            {
                                                return text4;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        IEnumerator<string> enumerator2;
                                        if (enumerator2 != null)
                                        {
                                            enumerator2.Dispose();
                                        }
                                    }
                                }
                                goto IL_0489;
                            }
                            finally
                            {
                                List<string>.Enumerator enumerator;
                                ((IDisposable)enumerator).Dispose();
                            }
                        }
                        if (CS$<>8__locals1.$VB$Local_File.Check.ActualSize < 0L || CS$<>8__locals1.$VB$Local_File.Check.Hash == null)
                        {
                            return null;
                        }
                        try
                        {
                            foreach (string text5 in FolderList.Concat<string>(VersionFolders))
                            {
                                try
                                {
                                    foreach (string text6 in DirectoryUtils.EnumerateFiles(text5 + text2, false, "*"))
                                    {
                                        if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate(text6, CS$<>8__locals1.$VB$Local_File.Check))
                                        {
                                            return text6;
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<string> enumerator4;
                                    if (enumerator4 != null)
                                    {
                                        enumerator4.Dispose();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator<string> enumerator3;
                            if (enumerator3 != null)
                            {
                                enumerator3.Dispose();
                            }
                        }
                        IL_0489:
                        text = null;
                    }
                    else
                    {
                        try
                        {
                            foreach (string text7 in FolderList)
                            {
                                string text8 = text7 + text2 + CS$<>8__locals1.$VB$Local_File.LocalPath.AfterFirst(text2, false);
                                if (this.$STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate(text8, CS$<>8__locals1.$VB$Local_File.Check))
                                {
                                    return text8;
                                }
                            }
                        }
                        finally
                        {
                            List<string>.Enumerator enumerator5;
                            ((IDisposable)enumerator5).Dispose();
                        }
                        text = null;
                    }
                }
                return text;
            }

            public void OnFileFinish(ModNet.NetFile File)
            {
                object fileRemainLock = this.FileRemainLock;
                ObjectFlowControl.CheckForSyncLockOnValueType(fileRemainLock);
                lock (fileRemainLock)
                {
                    ref int ptr = ref this.FileRemain;
                    this.FileRemain = checked(ptr - 1);
                    if (this.FileRemain > 0)
                    {
                        return;
                    }
                }
                this.OnFinish();
            }

            public void OnFinish()
            {
                base.RaisePreviewFinish();
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State <= ModBase.LoadState.Loading)
                    {
                        base.State = ModBase.LoadState.Finished;
                    }
                }
            }

            public void OnFileFail(ModNet.NetFile File)
            {
                try
                {
                    foreach (ModNet.NetSource netSource in File.Sources)
                    {
                        if (!Information.IsNothing(netSource.Ex))
                        {
                            File.Ex.Add(netSource.Ex);
                        }
                    }
                }
                finally
                {
                    List<ModNet.NetSource>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                this.OnFail(File.Ex);
            }

            public override void Failed(Exception Ex)
            {
                this.OnFail(new List<Exception> { Ex });
            }

            public void OnFail(List<Exception> ExList)
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State >= ModBase.LoadState.Finished)
                    {
                        return;
                    }
                    if (ExList == null || !ExList.Any<Exception>())
                    {
                        ExList = new List<Exception>
                        {
                            new Exception("未知错误！")
                        };
                    }
                    List<Exception> list = ExList.Where<Exception>((ModNet.LoaderDownload._Closure$__.$I20-0 == null) ? (ModNet.LoaderDownload._Closure$__.$I20-0 = (Exception e) => !e.IsCanceled() && !(e is TimeoutException)) : ModNet.LoaderDownload._Closure$__.$I20-0).ToList<Exception>();
                    base.Error = list.FirstOrDefault<Exception>() ?? ExList.FirstOrDefault<Exception>();
                    try
                    {
                        foreach (ModNet.NetFile netFile in this.Files)
                        {
                            if (netFile.State == ModNet.NetState.Canceled && !netFile.Sources.All<ModNet.NetSource>((ModNet.LoaderDownload._Closure$__.$I20-1 == null) ? (ModNet.LoaderDownload._Closure$__.$I20-1 = (ModNet.NetSource s) => s.Ex.IsCanceled() || s.Ex is TimeoutException) : ModNet.LoaderDownload._Closure$__.$I20-1))
                            {
                                string text = netFile.Sources.Select<ModNet.NetSource, string>((ModNet.LoaderDownload._Closure$__.$I20-2 == null) ? (ModNet.LoaderDownload._Closure$__.$I20-2 = (ModNet.NetSource s) => string.Format("{0}（{1}）", (s.Ex == null) ? "无错误信息。" : s.Ex.GetDisplay(false), s.Url)) : ModNet.LoaderDownload._Closure$__.$I20-2).Join("\r\n");
                                base.Error = new Exception("文件下载失败：" + netFile.LocalPath + "\r\n各下载源的错误如下：\r\n" + text, base.Error);
                                ModMain.Telemetry("文件下载失败", new string[] { "FileName", netFile.LocalName, "Exception", text });
                                break;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<ModNet.NetFile> enumerator;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    base.State = ModBase.LoadState.Failed;
                }
                try
                {
                    foreach (ModNet.NetFile netFile2 in this.Files)
                    {
                        netFile2.Cancel(this);
                    }
                }
                finally
                {
                    IEnumerator<ModNet.NetFile> enumerator2;
                    if (enumerator2 != null)
                    {
                        enumerator2.Dispose();
                    }
                }
                Logger.Error(new AggregateException(ExList), "下载加载器失败", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
            }

            public override void Cancel()
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State >= ModBase.LoadState.Finished)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Canceled;
                }
                Logger.Info(string.Format("{0} 已取消！", this.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                try
                {
                    foreach (ModNet.NetFile netFile in this.Files)
                    {
                        netFile.Cancel(this);
                    }
                }
                finally
                {
                    IEnumerator<ModNet.NetFile> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }

            public ConcurrentQueue<ModNet.NetFile> Files;

            private int FileRemain;

            private readonly object FileRemainLock;

            private double _Progress;

            private int _FailCount;

            private ConcurrentDictionary<string, long> $STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes;

            private StaticLocalInitFlag $STATIC$CheckExistingFile$203E151281691E151281691E128724$Sizes$Init;

            private Func<string, FileChecker, bool> $STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate;

            private StaticLocalInitFlag $STATIC$CheckExistingFile$203E151281691E151281691E128724$CheckCandidate$Init;
        }

        public class NetManagerClass
        {
            public NetManagerClass()
            {
                this.AllFiles = new ConcurrentDictionary<string, ModNet.NetFile>();
                this.Tasks = new ConcurrentBag<ModNet.LoaderDownload>();
                this.DownloadDone = 0L;
                this.FileRemain = 0;
                this.Speed = 0L;
                this.Uuid = ModBase.GetUuid();
                this.DownloadCacheLock = RuntimeHelpers.GetObjectValue(new object());
                this.IsDownloadCacheCleared = false;
            }

            private void RefreshStat()
            {
                checked
                {
                    try
                    {
                        long num = ModBase.GetTimeMs() - this.RefreshStatLast;
                        if (num != 0L)
                        {
                            ref long ptr = ref this.RefreshStatLast;
                            this.RefreshStatLast = ptr + num;
                            if (this.$STATIC$RefreshStat$2001$SpeedLast$Init == null)
                            {
                                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$RefreshStat$2001$SpeedLast$Init, new StaticLocalInitFlag(), null);
                            }
                            bool flag = false;
                            try
                            {
                                Monitor.Enter(this.$STATIC$RefreshStat$2001$SpeedLast$Init, ref flag);
                                if (this.$STATIC$RefreshStat$2001$SpeedLast$Init.State == 0)
                                {
                                    this.$STATIC$RefreshStat$2001$SpeedLast$Init.State = 2;
                                    this.$STATIC$RefreshStat$2001$SpeedLast = new List<long>();
                                }
                                else if (this.$STATIC$RefreshStat$2001$SpeedLast$Init.State == 2)
                                {
                                    throw new IncompleteInitialization();
                                }
                            }
                            finally
                            {
                                this.$STATIC$RefreshStat$2001$SpeedLast$Init.State = 1;
                                if (flag)
                                {
                                    Monitor.Exit(this.$STATIC$RefreshStat$2001$SpeedLast$Init);
                                }
                            }
                            if (this.$STATIC$RefreshStat$2001$SpeedLastDone$Init == null)
                            {
                                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$RefreshStat$2001$SpeedLastDone$Init, new StaticLocalInitFlag(), null);
                            }
                            bool flag2 = false;
                            try
                            {
                                Monitor.Enter(this.$STATIC$RefreshStat$2001$SpeedLastDone$Init, ref flag2);
                                if (this.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State == 0)
                                {
                                    this.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State = 2;
                                    this.$STATIC$RefreshStat$2001$SpeedLastDone = 0L;
                                }
                                else if (this.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State == 2)
                                {
                                    throw new IncompleteInitialization();
                                }
                            }
                            finally
                            {
                                this.$STATIC$RefreshStat$2001$SpeedLastDone$Init.State = 1;
                                if (flag2)
                                {
                                    Monitor.Exit(this.$STATIC$RefreshStat$2001$SpeedLastDone$Init);
                                }
                            }
                            double num2 = Math.Max(0.0, (double)(this.DownloadDone - this.$STATIC$RefreshStat$2001$SpeedLastDone) / ((double)num / 1000.0));
                            this.$STATIC$RefreshStat$2001$SpeedLast.Insert(0, (long)Math.Round(num2));
                            if (this.$STATIC$RefreshStat$2001$SpeedLast.Count >= 31)
                            {
                                this.$STATIC$RefreshStat$2001$SpeedLast.RemoveAt(30);
                            }
                            this.$STATIC$RefreshStat$2001$SpeedLastDone = this.DownloadDone;
                            long num3 = 0L;
                            long num4 = 0L;
                            int num5 = this.$STATIC$RefreshStat$2001$SpeedLast.Count;
                            try
                            {
                                foreach (long num6 in this.$STATIC$RefreshStat$2001$SpeedLast)
                                {
                                    num3 += num6 * unchecked((long)num5);
                                    num4 += unchecked((long)num5);
                                    num5--;
                                }
                            }
                            finally
                            {
                                List<long>.Enumerator enumerator;
                                ((IDisposable)enumerator).Dispose();
                            }
                            this.Speed = (long)Math.Round((num4 > 0L) ? ((double)num3 / (double)num4) : 0.0);
                            long num7 = 0L;
                            if (this.$STATIC$RefreshStat$2001$SpeedLast.Count >= 10)
                            {
                                num7 = (long)Math.Round(unchecked(this.$STATIC$RefreshStat$2001$SpeedLast.Take<long>(10).Average() * 0.85));
                            }
                            if (num7 > ModNet.NetTaskSpeedLimitLow)
                            {
                                ModNet.NetTaskSpeedLimitLow = num7;
                                Logger.Info(string.Format("速度下限已提升到 {0}", StringUtils.FormatByteSize(num7)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            }
                            try
                            {
                                foreach (ModNet.LoaderDownload loaderDownload in this.Tasks)
                                {
                                    loaderDownload.RefreshStat();
                                }
                            }
                            finally
                            {
                                IEnumerator<ModNet.LoaderDownload> enumerator2;
                                if (enumerator2 != null)
                                {
                                    enumerator2.Dispose();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "刷新下载公开属性失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                    }
                }
            }

            private void StartManager()
            {
                ModNet.NetManagerClass._Closure$__9-0 CS$<>8__locals1 = new ModNet.NetManagerClass._Closure$__9-0(CS$<>8__locals1);
                if (this.$STATIC$StartManager$2001$IsStarted$Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$StartManager$2001$IsStarted$Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this.$STATIC$StartManager$2001$IsStarted$Init, ref flag);
                    if (this.$STATIC$StartManager$2001$IsStarted$Init.State == 0)
                    {
                        this.$STATIC$StartManager$2001$IsStarted$Init.State = 2;
                        this.$STATIC$StartManager$2001$IsStarted = false;
                    }
                    else if (this.$STATIC$StartManager$2001$IsStarted$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this.$STATIC$StartManager$2001$IsStarted$Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this.$STATIC$StartManager$2001$IsStarted$Init);
                    }
                }
                checked
                {
                    if (!this.$STATIC$StartManager$2001$IsStarted)
                    {
                        this.$STATIC$StartManager$2001$IsStarted = true;
                        CS$<>8__locals1.$VB$Local_ThreadStarter = delegate(int Id)
                        {
                            try
                            {
                                for (;;)
                                {
                                    IL_0000:
                                    Thread.Sleep(20);
                                    List<ModNet.NetFile> list = new List<ModNet.NetFile>();
                                    List<ModNet.NetFile> list2 = new List<ModNet.NetFile>();
                                    if (Id == 0 && this.FileRemain == 0)
                                    {
                                        this.AllFiles.Clear();
                                    }
                                    try
                                    {
                                        foreach (ModNet.NetFile netFile in this.AllFiles.Values)
                                        {
                                            if (netFile.Uuid % 2 != Id)
                                            {
                                                if (netFile.State == ModNet.NetState.WaitingToDownload)
                                                {
                                                    list.Add(netFile);
                                                }
                                                else if (netFile.State < ModNet.NetState.Merging && netFile.State >= ModNet.NetState.WaitingToDownload)
                                                {
                                                    list2.Add(netFile);
                                                }
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        IEnumerator<ModNet.NetFile> enumerator;
                                        if (enumerator != null)
                                        {
                                            enumerator.Dispose();
                                        }
                                    }
                                    try
                                    {
                                        foreach (ModNet.NetFile netFile2 in list)
                                        {
                                            if (ModNet.NetTaskThreadCount >= ModNet.NetTaskThreadLimit)
                                            {
                                                goto IL_0000;
                                            }
                                            ModNet.NetThread netThread = netFile2.TryBeginThread();
                                            if (netThread != null && netThread.Source.Url.Contains("bmclapi"))
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        List<ModNet.NetFile>.Enumerator enumerator2;
                                        ((IDisposable)enumerator2).Dispose();
                                    }
                                    if (this.Speed < ModNet.NetTaskSpeedLimitLow)
                                    {
                                        try
                                        {
                                            foreach (ModNet.NetFile netFile3 in list2)
                                            {
                                                if (ModNet.NetTaskThreadCount >= ModNet.NetTaskThreadLimit)
                                                {
                                                    break;
                                                }
                                                int num = 0;
                                                int num2 = 0;
                                                if (netFile3.Threads != null)
                                                {
                                                    try
                                                    {
                                                        foreach (ModNet.NetThread netThread2 in netFile3.Threads.ToList<ModNet.NetThread>())
                                                        {
                                                            if (netThread2.State < ModNet.NetState.Downloading)
                                                            {
                                                                num++;
                                                            }
                                                            else if (netThread2.State == ModNet.NetState.Downloading)
                                                            {
                                                                num2++;
                                                            }
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        List<ModNet.NetThread>.Enumerator enumerator4;
                                                        ((IDisposable)enumerator4).Dispose();
                                                    }
                                                }
                                                if (num <= num2)
                                                {
                                                    ModNet.NetThread netThread3 = netFile3.TryBeginThread();
                                                    if (netThread3 != null && netThread3.Source.Url.Contains("bmclapi"))
                                                    {
                                                        Thread.Sleep(100);
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            List<ModNet.NetFile>.Enumerator enumerator3;
                                            ((IDisposable)enumerator3).Dispose();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex, string.Format("下载管理启动线程 {0} 出错", Id), LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            }
                        };
                        ModBase.RunInNewThread(delegate
                        {
                            CS$<>8__locals1.$VB$Local_ThreadStarter(0);
                        }, "NetManager ThreadStarter 0", ThreadPriority.Normal);
                        ModBase.RunInNewThread(delegate
                        {
                            CS$<>8__locals1.$VB$Local_ThreadStarter(1);
                        }, "NetManager ThreadStarter 1", ThreadPriority.Normal);
                        ModBase.RunInNewThread(delegate
                        {
                            try
                            {
                                long num3 = ModBase.GetTimeMs();
                                for (;;)
                                {
                                    if (ModNet.NetTaskSpeedLimitHigh > 0L)
                                    {
                                        Interlocked.Add(ref ModNet.NetTaskSpeedLimitLeft, (long)Math.Round((double)ModNet.NetTaskSpeedLimitHigh / 10.0));
                                    }
                                    this.RefreshStat();
                                    num3 += 100L;
                                    long num4 = num3 - ModBase.GetTimeMs();
                                    if (num4 > 0L)
                                    {
                                        Thread.Sleep((int)num4);
                                    }
                                    else
                                    {
                                        num3 = ModBase.GetTimeMs();
                                    }
                                }
                            }
                            catch (Exception ex2)
                            {
                                Logger.Error(ex2, "下载管理刷新线程出错", LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            }
                        }, "NetManager StatRefresher", ThreadPriority.Normal);
                    }
                }
            }

            public List<ModNet.NetFile> Start(ModNet.LoaderDownload Task)
            {
                this.StartManager();
                object downloadCacheLock = this.DownloadCacheLock;
                ObjectFlowControl.CheckForSyncLockOnValueType(downloadCacheLock);
                lock (downloadCacheLock)
                {
                    if (!this.IsDownloadCacheCleared)
                    {
                        try
                        {
                            Logger.Info("开始清理下载缓存", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            DirectoryUtils.Delete(ModBase.PathTemp + "Download", false);
                            Logger.Info("下载缓存已清理", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, "清理下载缓存失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                        }
                        this.IsDownloadCacheCleared = true;
                    }
                }
                List<ModNet.NetFile> list = new List<ModNet.NetFile>();
                List<ModNet.NetFile> list2 = new List<ModNet.NetFile>();
                try
                {
                    IEnumerator<ModNet.NetFile> enumerator = Task.Files.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ModNet.NetManagerClass._Closure$__12-0 CS$<>8__locals1 = new ModNet.NetManagerClass._Closure$__12-0(CS$<>8__locals1);
                        CS$<>8__locals1.$VB$Local_File = enumerator.Current;
                        ModNet.NetFile netFile = null;
                        if (this.AllFiles.TryGetValue(CS$<>8__locals1.$VB$Local_File.LocalPath, out netFile) && netFile.State < ModNet.NetState.Finished)
                        {
                            Logger.Trace(() => string.Format("{0}：无需开始，该文件已在下载中，目标 {1}", CS$<>8__locals1.$VB$Local_File.LocalName, CS$<>8__locals1.$VB$Local_File.LocalPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            CS$<>8__locals1.$VB$Local_File = netFile;
                        }
                        else
                        {
                            Logger.Trace(() => string.Format("{0}：开始，目标 {1}", CS$<>8__locals1.$VB$Local_File.LocalName, CS$<>8__locals1.$VB$Local_File.LocalPath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModNet.vb");
                            this.AllFiles[CS$<>8__locals1.$VB$Local_File.LocalPath] = CS$<>8__locals1.$VB$Local_File;
                            list.Add(CS$<>8__locals1.$VB$Local_File);
                            Interlocked.Increment(ref this.FileRemain);
                        }
                        list2.Add(CS$<>8__locals1.$VB$Local_File);
                        CS$<>8__locals1.$VB$Local_File.Tasks.Add(Task);
                    }
                }
                finally
                {
                    IEnumerator<ModNet.NetFile> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                Task.Files = new ConcurrentQueue<ModNet.NetFile>(list2);
                this.Tasks.Add(Task);
                return list;
            }

            public ConcurrentDictionary<string, ModNet.NetFile> AllFiles;

            public ConcurrentBag<ModNet.LoaderDownload> Tasks;

            public long DownloadDone;

            public int FileRemain;

            public long Speed;

            public readonly int Uuid;

            private long RefreshStatLast;

            private object DownloadCacheLock;

            private bool IsDownloadCacheCleared;

            private List<long> $STATIC$RefreshStat$2001$SpeedLast;

            private StaticLocalInitFlag $STATIC$RefreshStat$2001$SpeedLast$Init;

            private long $STATIC$RefreshStat$2001$SpeedLastDone;

            private StaticLocalInitFlag $STATIC$RefreshStat$2001$SpeedLastDone$Init;

            private bool $STATIC$StartManager$2001$IsStarted;

            private StaticLocalInitFlag $STATIC$StartManager$2001$IsStarted$Init;
        }
    }
}
