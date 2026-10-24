using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xaml;
using System.Xml.Linq;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PCL.Our;
using PCL.Our.Resources;
using ThrottleDebounce;

namespace PCL
{
    [StandardModule]
    public static class ModBase
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModBase()
        {
        }

        public static bool ModeDebug
        {
            get
            {
                return Settings.Get<bool>("SystemDebugMode", null);
            }
        }

        public static void IniClearCache(string FileName)
        {
            if (!FileName.Contains(":\\"))
            {
                FileName = string.Format("{0}PCL\\{1}.ini", Paths.Base, FileName);
            }
            ModBase.IniCache.Remove(FileName);
        }

        private static ConcurrentDictionary<string, string> IniGetContent(string FileName)
        {
            ConcurrentDictionary<string, string> concurrentDictionary2;
            try
            {
                if (!FileName.Contains(":\\"))
                {
                    FileName = string.Format("{0}PCL\\{1}.ini", Paths.Base, FileName);
                }
                ConcurrentDictionary<string, string> concurrentDictionary = null;
                if (ModBase.IniCache.TryGetValue(FileName, out concurrentDictionary))
                {
                    concurrentDictionary2 = concurrentDictionary;
                }
                else if (!FileUtils.Exists(FileName))
                {
                    concurrentDictionary2 = null;
                }
                else
                {
                    ConcurrentDictionary<string, string> concurrentDictionary3 = new ConcurrentDictionary<string, string>();
                    foreach (string text in FileUtils.ReadAsLines(FileName, true, null, null))
                    {
                        int num = text.IndexOfF(":", false);
                        if (num > 0)
                        {
                            concurrentDictionary3[text.Substring(0, num)] = text.Substring(checked(num + 1));
                        }
                    }
                    ModBase.IniCache[FileName] = concurrentDictionary3;
                    concurrentDictionary2 = concurrentDictionary3;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("生成 ini 文件缓存失败（{0}）", FileName), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                concurrentDictionary2 = null;
            }
            return concurrentDictionary2;
        }

        public static string ReadIni(string FileName, string Key, string DefaultValue = "")
        {
            ConcurrentDictionary<string, string> concurrentDictionary = ModBase.IniGetContent(FileName);
            string text;
            if (concurrentDictionary == null)
            {
                text = DefaultValue;
            }
            else
            {
                text = concurrentDictionary.GetOrDefault(Key, DefaultValue);
            }
            return text;
        }

        public static bool HasIniKey(string FileName, string Key)
        {
            ConcurrentDictionary<string, string> concurrentDictionary = ModBase.IniGetContent(FileName);
            return concurrentDictionary != null && concurrentDictionary.ContainsKey(Key);
        }

        public static void DeleteIniKey(string FileName, string Key)
        {
            ModBase.WriteIni(FileName, Key, null);
        }

        public static void WriteIni(string FileName, string Key, string Value)
        {
            try
            {
                if (Key.Contains(":"))
                {
                    throw new Exception(string.Format("尝试写入 ini 文件 {0} 的键名中包含了冒号：{1}", FileName, Key));
                }
                Key = Key.ReplaceLineEndings("", false);
                Value = ((Value != null) ? Value.ReplaceLineEndings("", false) : null);
                ConcurrentDictionary<string, string> concurrentDictionary = ModBase.IniGetContent(FileName);
                if (concurrentDictionary == null)
                {
                    concurrentDictionary = new ConcurrentDictionary<string, string>();
                }
                if (Value == null)
                {
                    if (!concurrentDictionary.Remove(Key))
                    {
                        return;
                    }
                }
                else
                {
                    string text = null;
                    if (concurrentDictionary.TryGetValue(Key, out text) && Operators.CompareString(text, Value, false) == 0)
                    {
                        return;
                    }
                    concurrentDictionary[Key] = Value;
                }
                StringBuilder stringBuilder = new StringBuilder();
                try
                {
                    foreach (KeyValuePair<string, string> keyValuePair in concurrentDictionary)
                    {
                        stringBuilder.Append(keyValuePair.Key);
                        stringBuilder.Append(":");
                        stringBuilder.Append(keyValuePair.Value);
                        stringBuilder.Append("\r\n");
                    }
                }
                finally
                {
                    IEnumerator<KeyValuePair<string, string>> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                if (!FileName.Contains(":\\"))
                {
                    FileName = string.Format("{0}PCL\\{1}.ini", Paths.Base, FileName);
                }
                FileName = (FileName.Contains(":\\") ? FileName : (Paths.Base + FileName));
                FileUtils.Write(FileName, stringBuilder.ToString(), null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("写入文件失败（{0} → {1}:{2}）", FileName, Key, Value), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
            }
        }

        public static bool CheckPermission(string Folder)
        {
            bool flag;
            try
            {
                if (string.IsNullOrEmpty(Folder))
                {
                    flag = false;
                }
                else
                {
                    Folder = PathUtils.AddSlashSuffix(Folder);
                    if (Folder.EndsWithF(":\\System Volume Information\\", true) || Folder.EndsWithF(":\\$RECYCLE.BIN\\", true))
                    {
                        flag = false;
                    }
                    else if (!DirectoryUtils.Exists(Folder))
                    {
                        flag = false;
                    }
                    else
                    {
                        string text = string.Format("{0}CheckPermission{1}.txt", Folder, ModBase.GetUuid());
                        FileUtils.Write(text, "临时文件，用于检测该文件夹的权限是否正常。", null);
                        FileUtils.Delete(text, false);
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("没有对文件夹 {0} 的权限，请尝试以管理员权限运行 PCL", Folder), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                flag = false;
            }
            return flag;
        }

        public static void CheckPermissionWithException(string Folder)
        {
            if (string.IsNullOrWhiteSpace(Folder))
            {
                throw new ArgumentNullException("文件夹名不能为空！");
            }
            Folder = PathUtils.AddSlashSuffix(Folder);
            if (Folder.EndsWithF(":\\System Volume Information\\", true) || Folder.EndsWithF(":\\$RECYCLE.BIN\\", true))
            {
                throw new UnauthorizedAccessException("没有对系统文件夹的权限！");
            }
            if (!DirectoryUtils.Exists(Folder))
            {
                throw new DirectoryNotFoundException("文件夹不存在！");
            }
            string text = string.Format("{0}CheckPermission{1}.txt", Folder, ModBase.GetUuid());
            FileUtils.Write(text, "临时文件，用于检测该文件夹的权限是否正常。", null);
            FileUtils.Delete(text, false);
        }

        public static double Val(object Str)
        {
            double num;
            if (Str == null)
            {
                num = 0.0;
            }
            else
            {
                try
                {
                    num = ((Str is string && Operators.ConditionalCompareObjectEqual(Str, "&", false)) ? 0.0 : Conversion.Val(RuntimeHelpers.GetObjectValue(Str)));
                }
                catch (Exception ex)
                {
                    num = 0.0;
                }
            }
            return num;
        }

        private static double SearchSimilarity(string Source, string Query)
        {
            double num;
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Query))
            {
                num = 0.0;
            }
            else
            {
                int i = 0;
                double num2 = 0.0;
                StringBuilder stringBuilder = new StringBuilder(Source.Length);
                stringBuilder.Append(Source.Lower().Replace(" ", ""));
                Query = Query.Lower().Replace(" ", "");
                int length = stringBuilder.Length;
                int length2 = Query.Length;
                if (length2 == 0)
                {
                    num = 0.0;
                }
                else
                {
                    checked
                    {
                        while (i < length2)
                        {
                            int j = 0;
                            int num3 = 0;
                            int num4 = 0;
                            int length3 = stringBuilder.Length;
                            while (j < length3)
                            {
                                int num5 = 0;
                                while (i + num5 < length2 && j + num5 < length3 && stringBuilder[j + num5] == Query[i + num5])
                                {
                                    num5++;
                                }
                                if (num5 > num3)
                                {
                                    num3 = num5;
                                    num4 = j;
                                }
                                j += ((num5 > 0) ? num5 : 1);
                            }
                            unchecked
                            {
                                if (num3 > 0)
                                {
                                    stringBuilder.Remove(num4, num3);
                                    double num6 = Math.Pow(1.4, (double)(checked(3 + num3))) - 3.6;
                                    num6 *= 1.0 + 0.3 * (double)Math.Max(0, checked(3 - Math.Abs(i - num4)));
                                    num2 += num6;
                                }
                            }
                            i += ((num3 > 0) ? num3 : 1);
                        }
                    }
                    num = num2 / (double)length2 * (3.0 / Math.Sqrt((double)(checked(length + 15)))) * (double)((length2 <= 2) ? checked(3 - length2) : 1);
                }
            }
            return num;
        }

        private static double SearchSimilarityWeighted(List<ModBase.SearchSource> Source, string Query)
        {
            ModBase._Closure___39_0 CS_locals1 = new ModBase._Closure___39_0(null);
            CS_locals1._VB_Local_Query = Query;
            double num = 0.0;
            double num2 = 0.0;
            try
            {
                foreach (ModBase.SearchSource searchSource in Source)
                {
                    if (searchSource.Aliases.Any<string>())
                    {
                        num2 += searchSource.Aliases.Max<string>((CS_locals1._I0 == null) ? (CS_locals1._I0 = new Func<string, double>(CS_locals1._Lambda___0)) : CS_locals1._I0) * searchSource.Weight;
                    }
                    num += searchSource.Weight;
                }
            }
            finally
            {
                List<ModBase.SearchSource>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            return num2 / num;
        }

        public static List<ModBase.SearchEntry<T>> Search<T>(List<ModBase.SearchEntry<T>> Entries, string Query, int MaxBlurCount = 5, double MinBlurSimilarity = 0.1)
        {
            List<ModBase.SearchEntry<T>> list = new List<ModBase.SearchEntry<T>>();
            checked
            {
                List<ModBase.SearchEntry<T>> list2;
                if (!Entries.Any<ModBase.SearchEntry<T>>())
                {
                    list2 = list;
                }
                else
                {
                    string[] array = Query.Split(" ", true);
                    List<ModBase.SearchEntry<T>> list3 = new List<ModBase.SearchEntry<T>>();
                    try
                    {
                        List<ModBase.SearchEntry<T>>.Enumerator enumerator = Entries.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ModBase._Closure___42_0<T> CS_locals1 = new ModBase._Closure___42_0<T>(null);
                            CS_locals1._VB_Local_Entry = enumerator.Current;
                            CS_locals1._VB_Local_Entry.Similarity = ModBase.SearchSimilarityWeighted(CS_locals1._VB_Local_Entry.SearchSource, Query);
                            CS_locals1._VB_Local_Entry.AbsoluteRight = array.All<string>(new Func<string, bool>(CS_locals1._Lambda___0));
                            if (CS_locals1._VB_Local_Entry.AbsoluteRight || CS_locals1._VB_Local_Entry.Similarity >= MinBlurSimilarity)
                            {
                                list3.Add(CS_locals1._VB_Local_Entry);
                            }
                        }
                    }
                    finally
                    {
                        List<ModBase.SearchEntry<T>>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    list3 = list3.SortByComparison<ModBase.SearchEntry<T>>((ModBase._Closure___42<T>._I42_3 == null) ? (ModBase._Closure___42<T>._I42_3 = new Func<ModBase.SearchEntry<T>, ModBase.SearchEntry<T>, bool>(ModBase._Closure___42<T>._I._Lambda___42_3)) : ModBase._Closure___42<T>._I42_3);
                    int num = 0;
                    try
                    {
                        foreach (ModBase.SearchEntry<T> searchEntry in list3)
                        {
                            if (searchEntry.AbsoluteRight)
                            {
                                list.Add(searchEntry);
                            }
                            else
                            {
                                if (num == MaxBlurCount)
                                {
                                    break;
                                }
                                list.Add(searchEntry);
                                num++;
                            }
                        }
                    }
                    finally
                    {
                        List<ModBase.SearchEntry<T>>.Enumerator enumerator2 = default;
                        ((IDisposable)enumerator2).Dispose();
                    }
                    list2 = list;
                }
                return list2;
            }
        }

        public static T GetResultWithTimeout<T>(this Task<T> TargetTask, CancellationTokenSource TokenSource, int TimeoutMs)
        {
            Task task = Task.Delay(TimeoutMs);
            if (Task.WhenAny(new Task[] { TargetTask, task }).ConfigureAwait(false).GetAwaiter()
                .GetResult() == task)
            {
                TokenSource.Cancel();
                throw new TimeoutException(string.Format("任务超时（{0} ms）", TimeoutMs));
            }
            return TargetTask.Run<T>();
        }

        public static void GetResultWithTimeout(this Task TargetTask, CancellationTokenSource TokenSource, int TimeoutMs)
        {
            Task task = Task.Delay(TimeoutMs);
            if (Task.WhenAny(new Task[] { TargetTask, task }).ConfigureAwait(false).GetAwaiter()
                .GetResult() == task)
            {
                TokenSource.Cancel();
                throw new TimeoutException(string.Format("任务超时（{0} ms）", TimeoutMs));
            }
            TargetTask.Run();
        }

        public static int RunAsAdmin(string Argument)
        {
            Process process = ModBase.StartProcess(new ProcessStartInfo(ModBase.PathExe)
            {
                Verb = "runas",
                Arguments = Argument
            });
            process.WaitForExit();
            return process.ExitCode;
        }

        public static int GetUuid()
        {
            if (ModBase.UuidLock == null)
            {
                ModBase.UuidLock = RuntimeHelpers.GetObjectValue(new object());
            }
            object uuidLock = ModBase.UuidLock;
            ObjectFlowControl.CheckForSyncLockOnValueType(uuidLock);
            checked
            {
                int uuid;
                lock (uuidLock)
                {
                    ModBase.Uuid++;
                    uuid = ModBase.Uuid;
                }
                return uuid;
            }
        }

        public static long GetTimeMs()
        {
            return Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000L);
        }

        public static long GetUnixTimestampUtc()
        {
            return checked((long)Math.Round((double)(DateAndTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000000.0));
        }

        public static long ToUnixTimestampUtc(DateTime Time)
        {
            return checked((long)Math.Round((double)(Time.ToUniversalTime().Ticks - 621355968000000000L) / 10000000.0));
        }

        public static DateTime GetDate(int timeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)timeStamp).LocalDateTime;
        }

        public static DateTime GetLocalTime(DateTime UtcDate)
        {
            return DateTime.SpecifyKind(UtcDate, DateTimeKind.Utc).ToLocalTime();
        }

        public static Process StartProcess(string FileName, string Arguments = "")
        {
            return ModBase.StartProcess(new ProcessStartInfo
            {
                FileName = FileName,
                Arguments = Arguments
            });
        }

        public static Process StartProcess(ProcessStartInfo StartInfo)
        {
            StartInfo.FileName = PathUtils.ToShortPath(StartInfo.FileName);
            if (!string.IsNullOrEmpty(StartInfo.WorkingDirectory))
            {
                StartInfo.WorkingDirectory = PathUtils.ToShortPath(StartInfo.WorkingDirectory);
            }
            Logger.Info(string.Format("启动进程：{0} {1}", StartInfo.FileName, StartInfo.Arguments), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
            return Process.Start(StartInfo);
        }

        public static Thread RunInNewThread(Action Action, string Name = null, ThreadPriority Priority = ThreadPriority.Normal)
        {
            ModBase._Closure___59_0 CS_locals1 = new ModBase._Closure___59_0();
            CS_locals1._VB_Local_Action = Action;
            CS_locals1._VB_Local_Name = Name;
            Thread thread = new Thread(new ThreadStart(CS_locals1._Lambda___0));
            thread.Name = CS_locals1._VB_Local_Name ?? ("Runtime New Invoke " + Conversions.ToString(ModBase.GetUuid()) + "#");
            thread.Priority = Priority;
            thread.Start();
            return thread;
        }

        public static Output RunInUiWait<Output>(Func<Output> Action)
        {
            Output output;
            if (ModBase.RunInUi())
            {
                output = Action();
            }
            else
            {
                output = Application.Current.Dispatcher.Invoke<Output>(Action);
            }
            return output;
        }

        public static void RunInUiWait(Action Action)
        {
            if (ModBase.RunInUi())
            {
                Action();
                return;
            }
            Application.Current.Dispatcher.Invoke(Action);
        }

        public static void RunInUi(Action Action, bool ForceWaitUntilLoaded = false)
        {
            if (ForceWaitUntilLoaded)
            {
                Application.Current.Dispatcher.InvokeAsync(Action, DispatcherPriority.Loaded);
                return;
            }
            if (ModBase.RunInUi())
            {
                Action();
                return;
            }
            Application.Current.Dispatcher.InvokeAsync(Action);
        }

        public static void RunInThread(Action Action)
        {
            if (ModBase.RunInUi())
            {
                ModBase.RunInNewThread(Action, "Invoke " + Conversions.ToString(ModBase.GetUuid()), ThreadPriority.Normal);
                return;
            }
            Action();
        }

        public static List<T> SortByComparison<T>(this IList<T> List, Func<T, T, bool> SortRule)
        {
            List<T> list = new List<T>();
            checked
            {
                while (List.Any<T>())
                {
                    T t = List[0];
                    int num = List.Count - 1;
                    for (int i = 1; i <= num; i++)
                    {
                        if (SortRule(List[i], t))
                        {
                            t = List[i];
                        }
                    }
                    List.Remove(t);
                    list.Add(t);
                }
                return list;
            }
        }

        public static object GetProgramArgument(string Name, [System.Runtime.InteropServices.Optional, System.Runtime.InteropServices.DefaultParameterValue("")] object DefaultValue)
        {
            string[] array = Interaction.Command().Split(" ", false);
            checked
            {
                int num = array.Length - 1;
                for (int i = 0; i <= num; i++)
                {
                    if (Operators.CompareString(array[i], "-" + Name, false) == 0)
                    {
                        object obj;
                        if (array.Length == i + 1 || array[i + 1].StartsWithF("-", false))
                        {
                            obj = true;
                        }
                        else
                        {
                            obj = array[i + 1];
                        }
                        return obj;
                    }
                }
                return DefaultValue;
            }
        }

        public static void OpenWebsite(string Url)
        {
            try
            {
                if (!Url.StartsWithF("http", true) && !Url.StartsWithF("minecraft://", true) && !Url.StartsWithF("minecraft-preview://", true))
                {
                    throw new Exception(Url + " 不是一个有效的网址，它必须以 http 开头！");
                }
                Logger.Info(string.Format("正在打开网页：{0}", Url), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                ModBase.StartProcess(Url, "");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Format("无法打开网页（{0}）", Url), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                ModBase.ClipboardSet(Url, false);
                ModMain.OurMsgBox("可能由于浏览器未正确配置，PCL 无法为你打开网页。\r\n网址已经复制到剪贴板，若有需要可以手动粘贴访问。\r\n" + string.Format("网址：{0}", Url), "无法打开网页", "确定", "", "", false, true, false, null, null, null);
            }
        }

        public static void OpenExplorer(string Location)
        {
            try
            {
                Location = Location.Replace("/", "\\").Trim(new char[] { ' ', '"' });
                Logger.Info(string.Format("正在打开资源管理器：{0}", Location), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                if (Location.EndsWithF("\\", false))
                {
                    ModBase.StartProcess(Location, "");
                }
                else
                {
                    ModBase.StartProcess("explorer", string.Format("/select,\"{0}\"", Location));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "打开资源管理器失败，请尝试关闭安全软件（如 360 安全卫士）", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
            }
        }

        public static void ClipboardSet(string Text, bool ShowSuccessHint = true)
        {
            ModBase._Closure___68_0 CS_locals1 = new ModBase._Closure___68_0();
            CS_locals1._VB_Local_Text = Text;
            CS_locals1._VB_Local_ShowSuccessHint = ShowSuccessHint;
            ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
        }

        public static string ClipboardGetText()
        {
            ModBase._Closure___69_0 CS_locals1 = new ModBase._Closure___69_0();
            CS_locals1._VB_Local_Result = null;
            ModBase.RunInUiWait(new Action(CS_locals1._Lambda___0));
            return CS_locals1._VB_Local_Result;
        }

        public static void ExtractResources(string FilePath, string ResourceName)
        {
            object objectValue = RuntimeHelpers.GetObjectValue(Resources.ResourceManager.GetObject(ResourceName));
            if (objectValue is byte[])
            {
                byte[] array = (byte[])objectValue;
                if (!FileUtils.Exists(FilePath) || Operators.CompareString(CryptographyUtils.ComputeFileHash(FilePath, CryptographyUtils.HashMethod.Md5), CryptographyUtils.ComputeHash(array, CryptographyUtils.HashMethod.Md5), false) != 0)
                {
                    Logger.Info(string.Format("将资源写入到文件：{0} → {1}", ResourceName, FilePath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                    FileUtils.Write(FilePath, array);
                    return;
                }
            }
            else
            {
                if (!(objectValue is string))
                {
                    throw new Exception(string.Format("资源 {0} 的类型不支持：{1}", ResourceName, objectValue.GetType().Name));
                }
                string text = (string)objectValue;
                if (!FileUtils.Exists(FilePath) || Operators.CompareString(FileUtils.ReadAsString(FilePath, null, null), text, false) != 0)
                {
                    Logger.Info(string.Format("将资源写入到文件：{0} → {1}", ResourceName, FilePath), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                    FileUtils.Write(FilePath, text, null);
                    return;
                }
            }
        }

        public static double GetPixelSize(double WPFSize)
        {
            return WPFSize / 96.0 * (double)ModBase.DPI;
        }

        public static double GetWPFSize(double PixelSize)
        {
            return PixelSize * 96.0 / (double)ModBase.DPI;
        }

        public static object GetObjectFromXML(XElement Str)
        {
            return ModBase.GetObjectFromXML(Str.ToString());
        }

        public static object GetObjectFromXML(string Str)
        {
            Str = Str.Replace("EventType=\"", "local:CustomEventService.EventType=\"").Replace("EventData=\"", "local:CustomEventService.EventData=\"").Replace("EventType='", "local:CustomEventService.EventType='")
                .Replace("EventData='", "local:CustomEventService.EventData='")
                .Replace("Property=\"EventType\"", "Property=\"local:CustomEventService.EventType\"")
                .Replace("Property=\"EventData\"", "Property=\"local:CustomEventService.EventData\"")
                .Replace("Property='EventType'", "Property='local:CustomEventService.EventType'")
                .Replace("Property='EventData'", "Property='local:CustomEventService.EventData'");
            object obj;
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Str)))
            {
                using (XamlXmlReader xamlXmlReader = new XamlXmlReader(memoryStream))
                {
                    while (xamlXmlReader.Read())
                    {
                        foreach (Type type in new Type[]
                        {
                            typeof(WebBrowser),
                            typeof(Frame),
                            typeof(MediaElement),
                            typeof(ObjectDataProvider),
                            typeof(global::System.Xaml.XamlReader),
                            typeof(Window),
                            typeof(XmlDataProvider),
                            typeof(SettingService)
                        })
                        {
                            if (xamlXmlReader.Type != null && type.IsAssignableFrom(xamlXmlReader.Type.UnderlyingType))
                            {
                                throw new UnauthorizedAccessException(string.Format("基于安全考虑，不允许使用 {0} 类型。", type.Name));
                            }
                            if (xamlXmlReader.Value != null && Operators.ConditionalCompareObjectEqual(xamlXmlReader.Value, type.Name, false))
                            {
                                throw new UnauthorizedAccessException(string.Format("基于安全考虑，不允许使用 {0} 值。", type.Name));
                            }
                        }
                        foreach (string text in new string[] { "Code", "FactoryMethod", "Static" })
                        {
                            if (xamlXmlReader.Member != null && Operators.CompareString(xamlXmlReader.Member.Name, text, false) == 0)
                            {
                                throw new UnauthorizedAccessException(string.Format("基于安全考虑，不允许使用 {0} 成员。", text));
                            }
                        }
                    }
                }
                memoryStream.Position = 0L;
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(Str);
                    streamWriter.Flush();
                    memoryStream.Position = 0L;
                    obj = global::System.Windows.Markup.XamlReader.Load(memoryStream);
                }
            }
            return obj;
        }

        public static bool RunInUi()
        {
            return Thread.CurrentThread.ManagedThreadId == ModBase.UiThreadId;
        }

        public static bool IsVisibleInForm(this FrameworkElement element)
        {
            bool flag;
            if (!element.IsVisible)
            {
                flag = false;
            }
            else
            {
                Rect rect = element.TransformToAncestor(ModMain.FrmMain).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
                Rect rect2 = new Rect(0.0, 0.0, ModMain.FrmMain.ActualWidth, ModMain.FrmMain.ActualHeight);
                flag = rect2.Contains(rect.TopLeft) || rect2.Contains(rect.BottomRight);
            }
            return flag;
        }

        public static bool IsTextTrimmed(this TextBlock TextBlock)
        {
            Typeface typeface = new Typeface(TextBlock.FontFamily, TextBlock.FontStyle, TextBlock.FontWeight, TextBlock.FontStretch);
            return new FormattedText(TextBlock.Text, Thread.CurrentThread.CurrentCulture, TextBlock.FlowDirection, typeface, TextBlock.FontSize, TextBlock.Foreground, (double)ModBase.DPI).Width > TextBlock.ActualWidth;
        }

        public static Visibility ToVisibility(this bool IsVisible)
        {
            if (!IsVisible)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public static T RandomOne<T>(ICollection<T> objects)
        {
            return objects.ElementAtOrDefault<T>(ModBase.RandomInteger(0, checked(objects.Count - 1)));
        }

        public static int RandomInteger(int min, int max)
        {
            checked
            {
                return (int)Math.Round(unchecked(Math.Floor((double)(checked(max - min + 1)) * ModBase.Random.NextDouble()) + (double)min));
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> Raw)
        {
            ModBase.VB_StateMachine_84_Shuffle<T> vb_StateMachine_84_Shuffle = new ModBase.VB_StateMachine_84_Shuffle<T>(-2);
            vb_StateMachine_84_Shuffle._P_Raw = Raw;
            return vb_StateMachine_84_Shuffle;
        }

        public const string VersionBaseName = "2.99.0.0";

        public const string CommitHash = "";

        public const int VersionCode = 414;

        public const string VersionDisplay = "快照版 2.99.0.0";

        public const string BuildTypeDisplay = "快照版";

        public const ModBase.BuildTypes BuildType = ModBase.BuildTypes.Snapshot;

        public static IntPtr Handle;

        public static string PathExe = Paths.Base + AppDomain.CurrentDomain.SetupInformation.ApplicationName;

        public static string PathImage = "pack://application:,,,/OpenPCL;component/Images/";

        public static string PathTemp = PathUtils.AddSlashSuffix(((Operators.CompareString(Settings.Get<string>("SystemSystemCache", null), "", false) == 0) ? (Path.GetTempPath() + "PCL\\") : Settings.Get<string>("SystemSystemCache", null)).Replace("/", "\\"));

        public static string Lang = "zh_CN";

        public static long ApplicationStartTick = ModBase.GetTimeMs();

        public static DateTime ApplicationOpenTime = DateTime.Now;

        public static string Identify = ModSecret.GetIdentify();

        public static bool IsProgramEnding = false;

        public static bool IsGBKEncoding = Encoding.Default.CodePage == 936;

        public static string OsDrive = Environment.GetLogicalDrives().First<string>(new Func<string, bool>(ModBase._Closure___._I._Lambda___0_0));

        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> IniCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        public static char vbLQ = Convert.ToChar(8220);

        public static char vbRQ = Convert.ToChar(8221);

        public static Lazy<string> PathPure = new Lazy<string>(new Func<string>(ModBase._Closure___._I._Lambda___0_1));

        private static int Uuid = 1;

        private static object UuidLock;

        public static readonly int DPI = checked((int)Math.Round((double)Graphics.FromHwnd(IntPtr.Zero).DpiX));

        private static readonly int UiThreadId = Thread.CurrentThread.ManagedThreadId;

        private static readonly Random Random = new Random();

        public enum BuildTypes
        {
            Debug = 100,
            Release = 50,
            Snapshot = 0
        }

        public class Logo
        {
            public Logo()
            {
            }

            public const string IconButtonLikeLine = "M512 896a42.666667 42.666667 0 0 1-30.293333-12.373333l-331.52-331.946667a224.426667 224.426667 0 0 1 0-315.733333 223.573333 223.573333 0 0 1 315.733333 0L512 282.026667l46.08-46.08a223.573333 223.573333 0 0 1 315.733333 0 224.426667 224.426667 0 0 1 0 315.733333l-331.52 331.946667A42.666667 42.666667 0 0 1 512 896zM308.053333 256a136.533333 136.533333 0 0 0-97.28 40.106667 138.24 138.24 0 0 0 0 194.986666L512 792.746667l301.226667-301.653334a138.24 138.24 0 0 0 0-194.986666 141.653333 141.653333 0 0 0-194.56 0l-76.373334 76.8a42.666667 42.666667 0 0 1-60.586666 0L405.333333 296.106667A136.533333 136.533333 0 0 0 308.053333 256z";

            public const string IconButtonLikeFill = "M700.856 155.543c-74.769 0-144.295 72.696-190.046 127.26-45.737-54.576-115.247-127.26-190.056-127.26-134.79 0-244.443 105.78-244.443 235.799 0 77.57 39.278 131.988 70.845 175.713C238.908 694.053 469.62 852.094 479.39 858.757c9.41 6.414 20.424 9.629 31.401 9.629 11.006 0 21.998-3.215 31.398-9.63 9.782-6.662 240.514-164.703 332.238-291.701 31.587-43.724 70.874-98.143 70.874-175.713-0.001-130.02-109.656-235.8-244.445-235.8z m0 0";

            public const string IconButtonDelete = "M520.192 0C408.43 0 317.44 82.87 313.563 186.734H52.736c-29.038 0-52.663 21.943-52.663 49.079s23.625 49.152 52.663 49.152h58.075v550.473c0 103.35 75.118 187.757 167.717 187.757h472.43c92.599 0 167.716-83.894 167.716-187.757V285.477h52.59c29.038 0 52.59-21.943 52.663-49.08-0.073-27.135-23.625-49.151-52.663-49.151H726.235C723.237 83.017 631.955 0 520.192 0zM404.846 177.957c3.803-50.03 50.176-89.015 107.447-89.015 57.197 0 103.57 38.985 106.788 89.015H404.92zM284.379 933.669c-33.353 0-69.997-39.351-69.997-95.525v-549.01H833.39v549.522c0 56.247-36.645 95.525-69.998 95.525H284.379v-0.512z M357.23 800.695a48.274 48.274 0 0 0 47.616-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.69 49.006V751.69c0 27.282 20.846 49.006 47.617 49.006z m166.62 0a48.274 48.274 0 0 0 47.688-49.006V471.7a48.274 48.274 0 0 0-47.689-49.08 48.274 48.274 0 0 0-47.543 49.006V751.69c0 27.282 21.431 49.006 47.543 49.006z m142.92 0a48.274 48.274 0 0 0 47.543-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.616 49.006V751.69c0 27.282 20.773 49.006 47.543 49.006z";

            public const string IconButtonStop = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z";

            public const string IconButtonCheck = "M512 0a512 512 0 1 0 512 512A512 512 0 0 0 512 0z m0 921.6a409.6 409.6 0 1 1 409.6-409.6 409.6 409.6 0 0 1-409.6 409.6z M716.8 339.968l-256 253.44L328.192 460.8A51.2 51.2 0 0 0 256 532.992l168.448 168.96a51.2 51.2 0 0 0 72.704 0l289.28-289.792A51.2 51.2 0 0 0 716.8 339.968z";

            public const string IconButtonEdit = "M732.64 64.32C688.576 21.216 613.696 21.216 569.6 64.32L120.128 499.52c-17.6 12.896-26.432 30.144-30.848 51.68L32 870.048c0 25.856 8.8 56 26.432 73.248 17.632 17.216 17.632 48.704 88.64 48.704h13.248l326.08-56c22.016-4.32 39.68-12.928 52.864-30.176l449.472-435.2c22.048-21.536 35.264-47.36 35.264-77.536 0-30.176-13.216-56-35.264-77.568l-256.096-251.2zM139.712 903.776l56-326.912 311.04-295.136 267.104 269.44-310.976 295.168-323.168 57.44zM844.576 467.84l-273.984-260.672 61.856-59.84c8.832-8.512 26.528-8.512 39.776 0l234.24 226.496c4.384 4.288 8.832 12.8 8.832 17.088s-4.416 8.544-8.864 12.8l-61.856 64.128z";

            public const string IconButtonSetup = "M651.946667 1001.813333c-22.186667 0-42.666667-10.24-61.44-27.306666-23.893333-23.893333-49.493333-35.84-75.093334-35.84-29.013333 0-56.32 11.946667-73.386666 30.72v3.413333c-17.066667 17.066667-42.666667 27.306667-66.56 27.306667h-6.826667c-6.826667 0-11.946667-1.706667-15.36-1.706667l-6.826667-1.706667c-64.853333-20.48-121.173333-54.613333-168.96-98.986666-29.013333-23.893333-37.546667-63.146667-25.6-95.573334 8.533333-23.893333 5.12-51.2-10.24-75.093333-15.36-27.306667-34.133333-40.96-59.733333-47.786667h-1.706667l-5.12-1.706666c-35.84-8.533333-61.44-34.133333-66.56-69.973334C1.706667 575.146667 0 537.6 0 512c0-32.426667 3.413333-63.146667 8.533333-93.866667v-6.826666l3.413334-8.533334c10.24-23.893333 23.893333-40.96 44.373333-51.2 5.12-3.413333 11.946667-6.826667 20.48-8.533333 27.306667-8.533333 51.2-25.6 63.146667-44.373333 13.653333-23.893333 17.066667-52.906667 10.24-81.92-11.946667-34.133333 0-71.68 30.72-93.866667 44.373333-37.546667 97.28-68.266667 158.72-93.866667l3.413333-1.706666c44.373333-13.653333 75.093333 3.413333 92.16 20.48 23.893333 23.893333 49.493333 35.84 75.093333 35.84 30.72 0 56.32-10.24 71.68-30.72l3.413334-3.413334c27.306667-27.306667 63.146667-35.84 93.866666-22.186666 63.146667 22.186667 117.76 54.613333 165.546667 97.28 29.013333 23.893333 37.546667 63.146667 25.6 95.573333-8.533333 23.893333-5.12 51.2 10.24 75.093333 15.36 27.306667 34.133333 40.96 59.733333 47.786667h1.706667l5.12 1.706667c35.84 8.533333 61.44 34.133333 66.56 71.68 6.826667 30.72 10.24 63.146667 11.946667 93.866666v3.413334c0 32.426667-3.413333 63.146667-8.533334 93.866666v6.826667l-3.413333 8.533333c-10.24 23.893333-23.893333 40.96-44.373333 51.2-5.12 3.413333-11.946667 6.826667-20.48 8.533334-27.306667 8.533333-51.2 25.6-63.146667 46.08-13.653333 23.893333-17.066667 52.906667-10.24 81.92 11.946667 35.84-1.706667 75.093333-30.72 95.573333-44.373333 35.84-95.573333 66.56-157.013333 92.16-15.36 3.413333-27.306667 3.413333-35.84 3.413333z m3.413333-83.626666z m1.706667 0zM517.12 853.333333c47.786667 0 93.866667 20.48 134.826667 59.733334 1.706667 1.706667 3.413333 1.706667 3.413333 3.413333 52.906667-22.186667 97.28-49.493333 136.533333-80.213333l1.706667-1.706667v-3.413333c-13.653333-52.906667-8.533333-104.106667 17.066667-148.48 23.893333-39.253333 64.853333-69.973333 114.346666-85.333334 1.706667 0 3.413333-1.706667 6.826667-6.826666 5.12-25.6 8.533333-51.2 8.533333-78.506667-1.706667-29.013333-3.413333-56.32-10.24-81.92v-5.12h-1.706666c-51.2-11.946667-90.453333-39.253333-119.466667-87.04-27.306667-44.373333-34.133333-100.693333-17.066667-148.48l-1.706666-1.706667h-3.413334c-39.253333-35.84-85.333333-63.146667-136.533333-80.213333H648.533333s-1.706667 1.706667-3.413333 1.706667c-32.426667 39.253333-80.213333 59.733333-136.533333 59.733333-47.786667 0-93.866667-20.48-134.826667-59.733333l-1.706667-1.706667h-1.706666c-54.613333 22.186667-98.986667 49.493333-136.533334 80.213333l-1.706666 1.706667v3.413333c13.653333 52.906667 8.533333 104.106667-17.066667 148.48-23.893333 39.253333-64.853333 69.973333-114.346667 85.333334-1.706667 0-3.413333 1.706667-6.826666 6.826666-6.826667 25.6-8.533333 51.2-8.533334 78.506667 0 30.72 3.413333 58.026667 6.826667 76.8l1.706667 5.12h1.706666c51.2 11.946667 90.453333 39.253333 119.466667 87.04 27.306667 44.373333 34.133333 100.693333 17.066667 148.48l1.706666 1.706667 1.706667 1.706666c37.546667 35.84 83.626667 63.146667 134.826667 80.213334 1.706667 0 3.413333 0 3.413333 1.706666h1.706667s1.706667 0 5.12-1.706666c34.133333-37.546667 81.92-59.733333 136.533333-59.733334z m-6.826667-146.773333c-110.933333 0-199.68-85.333333-199.68-196.266667 0-109.226667 87.04-196.266667 199.68-196.266666s199.68 85.333333 199.68 196.266666c-1.706667 109.226667-88.746667 196.266667-199.68 196.266667z m0-307.2c-63.146667 0-114.346667 49.493333-114.346666 110.933333 0 63.146667 49.493333 110.933333 114.346666 110.933334 30.72 0 59.733333-11.946667 80.213334-32.426667 20.48-20.48 32.426667-49.493333 32.426666-78.506667 0-63.146667-49.493333-110.933333-112.64-110.933333z";

            public const string IconButtonReset = "M667.6817627 313.65283203l-45.28564454 55.76660156L858.06933594 391.27124023 787.61950684 165.93066406l-56.01379395 69.01611328A354.47387695 354.47387695 0 0 0 520.89892578 165.93066406C324.87536621 165.93066406 165.93066406 324.43041992 165.93066406 519.91015625c0 195.52917481 158.94470215 353.97949219 354.96826172 353.97949219a355.06713867 355.06713867 0 0 0 331.73217774-227.66418458 50.52612305 50.52612305 0 0 0-29.21813966-65.25878905 50.77331543 50.77331543 0 0 0-65.50598144 29.16870117A253.61938477 253.61938477 0 0 1 520.94836426 772.78796387c-140.05920411 0-253.61938477-113.21411133-253.61938477-252.87780762 0-139.61425781 113.56018067-252.82836914 253.61938477-252.82836914 53.59130859 0 104.46350098 16.61132813 146.73339843 46.57104492";

            public const string IconButtonRefresh = "M512.0 838.3c-80.2 0-153.4-29.3-210.2-77.4l75.5-75.5c11.5-11.5 25.8-22.0 25.8-37.0a27.2 27.2 0 0 0-27.1-27.1H104.0c-27.1 0-27.1 23.9-27.1 27.1v271.9a27.1 27.1 0 0 0 27.1 27.1c15.0 0 27.8-16.6 42.5-31.2l77.9-77.9c76.6 67.7 177.1 108.9 287.4 108.9 221.7 0 404.5-166.0 431.2-380.6h-109.8c-25.9 154.2-159.7 271.9-321.3 271.9zM919.9 76.6c-15.0 0-27.8 16.6-42.5 31.3L799.5 185.8c-76.5-67.7-177.1-108.9-287.4-108.9-221.8 0-404.5 166.1-431.3 380.6H190.6c25.9-154.2 159.7-271.9 321.4-271.9 80.2 0 153.4 29.3 210.1 77.4l-75.5 75.5c-11.6 11.5-25.8 22.0-25.8 37.1a27.2 27.2 0 0 0 27.1 27.1h271.9c27.1 0 27.1-23.9 27.1-27.1V103.8a27.1 27.1 0 0 0-27.1-27.1z";

            public const string IconButtonSave = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z";

            public const string IconButtonInfo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z";

            public const string IconButtonList = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z";

            public const string IconButtonOpen = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z";

            public const string IconButtonArrowUp = "M554 333V853h-85V333l-228 228-60-60L512 170l331 331-60 60L554 333z";

            public const string IconButtonArrowDown = "M554 691V171h-85V691L241 463l-60 60L512 854l331-331-60-60L554 691z";

            public const string IconButtonCard = "M834.5 684.1c-31.2-70.4-98.9-120.9-179.1-127.3 63.5-8.5 112.6-63 112.6-128.8 0-71.8-58.2-130-130-130s-130 58.2-130 130c0 65.9 49 120.3 112.6 128.8-80.2 6.4-148 57-179.1 127.3-8.7 19.7 6 42 27.6 42 12.1 0 22.7-7.5 27.7-18.5 24.3-53.9 78.5-91.5 141.3-91.5s117 37.6 141.3 91.5c5 11.1 15.6 18.5 27.7 18.5 21.4 0 36.1-22.3 27.4-42zM567.9 427.9c0-38.6 31.4-70 70-70s70 31.4 70 70-31.4 70-70 70-70-31.4-70-70zM460.3 347.9H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h243.3c16.6 0 30-13.4 30-30 0.1-16.5-13.4-30-29.9-30zM367.4 459.6H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h150.4c16.6 0 30-13.4 30-30 0.1-16.6-13.4-30-29.9-30zM297.4 571.2H217c-16.6 0-30 13.4-30 30s13.4 30 30 30h80.4c16.6 0 30-13.4 30-30 0-16.5-13.5-30-30-30zM900 236v552H124V236h776m0-60H124c-33.1 0-60 26.9-60 60v552c0 33.1 26.9 60 60 60h776c33.1 0 60-26.9 60-60V236c0-33.1-26.9-60-60-60z";

            public const string IconButtonCross = "F1 M 26.9166,22.1667L 37.9999,33.25L 49.0832,22.1668L 53.8332,26.9168L 42.7499,38L 53.8332,49.0834L 49.0833,53.8334L 37.9999,42.75L 26.9166,53.8334L 22.1666,49.0833L 33.25,38L 22.1667,26.9167L 26.9166,22.1667 Z";

            public const string IconButtonMojang = "M9.183,18.967c-0.109-2.239,1.336-5.119,3.92-4.96c3.657-0.027,7.319-0.044,10.977,0.005\tc3.712,0.214,6.596,2.759,9.652,4.533c0.181-1.697-0.197-3.717,1.237-4.982c1.899,2.091,3.143,4.894,5.677,6.334\tc1.577,0.805,2.973-0.668,4.221-1.44c1.55,2.305,2.108,5.075,2.622,7.752c0.657,3.438,0.947,6.925,1.111,10.413\tc-1.734-0.733-3.355-1.708-4.971-2.671c-1.396-4.933-5.349-8.656-9.723-11.059c-3.46-1.817-7.752-2.185-11.338-0.52\tc-3.761,1.593-6.285,5.666-5.891,9.75c0.121,4.577,3.17,8.765,7.391,10.456c7.746,3.285,16.407,2.234,24.521,1.243\tc0.454,2.315-1.511,4.527-3.695,4.889c-10.577,0.038-21.148-0.011-31.725,0.022c-2.217,0.279-4.079-1.879-3.964-4.008\tC9.167,36.141,9.216,27.556,9.183,18.967z M40.114,8.09c0.294-0.801,0.872-1.417,1.542-1.924c1.379,2.589,2.742,5.482,2.311,8.491\tc-0.332,2.54-4.42,2.507-5.052,0.163C38.349,12.527,39.209,10.178,40.114,8.09z M0,53 l0.1,0 z";

            public const string IconButtonUser = "M660.338 528.065c63.61-46.825 105.131-121.964 105.131-206.83 0-141.7-115.29-256.987-256.997-256.987-141.706 0-256.998 115.288-256.998 256.987 0 85.901 42.52 161.887 107.456 208.562-152.1 59.92-260.185 207.961-260.185 381.077 0 21.276 17.253 38.53 38.53 38.53 21.278 0 38.53-17.254 38.53-38.53 0-183.426 149.232-332.671 332.667-332.671 1.589 0 3.113-0.207 4.694-0.244 0.8 0.056 1.553 0.244 2.362 0.244 183.434 0 332.664 149.245 332.664 332.671 0 21.276 17.255 38.53 38.533 38.53 21.277 0 38.53-17.254 38.53-38.53 0-174.885-110.354-324.13-264.917-382.809z m-331.803-206.83c0-99.22 80.72-179.927 179.935-179.927s179.937 80.708 179.937 179.927c0 99.203-80.721 179.91-179.937 179.91s-179.935-80.708-179.935-179.91z";

            public const string IconButtonShield = "M511.488256 95.184408c35.310345 22.516742 95.184408 55.78011 167.34033 84.437781 75.738131 29.681159 148.405797 40.93953 191.392304 45.033483v353.615193c0 73.691154-50.662669 164.781609-136.123938 244.101949C649.65917 901.181409 558.568716 942.12094 512 942.12094c-46.568716 0-137.65917-40.93953-222.096952-119.748126C204.441779 742.54073 153.77911 651.450275 153.77911 577.247376v-353.103448c42.474763-4.093953 116.165917-15.352324 191.904048-45.545227 75.226387-30.192904 133.565217-63.456272 165.805098-83.414293M512 0c-4.093953 0-8.187906 1.535232-11.258371 3.582209l-14.84058 10.234882c-1.023488 0.511744-67.550225 47.592204-170.410794 88.531735-100.813593 39.916042-198.556722 41.963018-199.58021 41.963018l-25.075462 0.511744c-10.746627 0.511744-18.934533 8.187906-18.934533 18.422789v414.000999c0 216.97951 286.064968 446.24088 440.09995 446.24088s440.09995-229.261369 440.09995-445.729136V163.758121c0-10.234883-8.69965-18.422789-18.934533-18.422789l-24.563718-0.511744c-1.023488 0-98.766617-2.046977-199.58021-41.963018-103.372314-40.93953-170.410795-88.01999-170.922538-88.531734L523.258371 3.582209c-3.070465-2.558721-7.164418-3.582209-11.258371-3.582209z M743.308346 410.930535l-260.477761 260.477761c-15.864068 15.864068-41.963018 15.864068-57.827087 0l-144.823588-144.823588c-15.864068-15.864068-15.864068-41.963018 0-57.827087 8.187906-8.187906 18.422789-11.770115 29.169415-11.770115 10.234883 0 20.981509 4.093953 29.169416 11.770115l115.654173 115.654173L685.993003 352.591704c15.864068-15.864068 41.963018-15.864068 57.827087 0 15.352324 16.375812 15.352324 42.474763-0.511744 58.338831z";

            public const string IconButtonOffline = "M533.293176 788.841412a60.235294 60.235294 0 1 1 85.202824 85.202823l-42.616471 42.586353c-129.355294 129.385412-339.124706 129.385412-468.510117 0-129.385412-129.385412-129.385412-339.124706 0-468.510117l42.586353-42.616471a60.235294 60.235294 0 1 1 85.202823 85.202824l-42.61647 42.586352a210.823529 210.823529 0 1 0 298.164706 298.164706l42.586352-42.61647z m255.548236-255.548236l42.61647-42.586352a210.823529 210.823529 0 1 0-298.164706-298.164706l-42.586352 42.61647a60.235294 60.235294 0 1 1-85.202824-85.202823l42.616471-42.586353c129.355294-129.385412 339.124706-129.385412 468.510117 0 129.385412 129.385412 129.385412 339.124706 0 468.510117l-42.586353 42.616471a60.235294 60.235294 0 1 1-85.202823-85.202824zM192.542118 192.542118a60.235294 60.235294 0 0 1 85.202823 0l553.712941 553.712941a60.235294 60.235294 0 0 1-85.202823 85.202823L192.542118 277.744941a60.235294 60.235294 0 0 1 0-85.202823z";

            public const string IconButtonServer = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z";

            public const string IconMusic = "M348.293565 716.53287V254.797913c0-41.672348 28.004174-78.358261 68.919652-90.37913L815.994435 40.826435c62.775652-18.610087 125.907478 26.579478 125.907478 89.933913v539.158261c8.013913 42.25113-8.94887 89.177043-47.014956 127.109565a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c21.014261 1.024 40.514783 5.164522 57.878261 12.065391V233.338435c0-12.109913-10.551652-20.034783-20.569044-20.034783a24.620522 24.620522 0 0 0-5.787826 0.934957L439.785739 338.18713a19.545043 19.545043 0 0 0-14.825739 19.144348v438.984348H423.846957c11.53113 43.987478-5.164522 94.208-45.412174 134.322087a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c20.791652 1.024 40.069565 5.075478 57.299478 11.842783z";

            public const string IconPlay = "M803.904 463.936a55.168 55.168 0 0 1 0 96.128l-463.616 264.448C302.848 845.888 256 819.136 256 776.448V247.616c0-42.752 46.848-69.44 84.288-48.064l463.616 264.384z";
        }

        public class OurColor
        {
            public static implicit operator ModBase.OurColor(string str)
            {
                return new ModBase.OurColor(str);
            }

            public static implicit operator ModBase.OurColor(global::System.Windows.Media.Color col)
            {
                return new ModBase.OurColor(col);
            }

            public static implicit operator global::System.Windows.Media.Color(ModBase.OurColor conv)
            {
                return global::System.Windows.Media.Color.FromArgb(ModBase.OurColor.ClampToByte(conv.A), ModBase.OurColor.ClampToByte(conv.R), ModBase.OurColor.ClampToByte(conv.G), ModBase.OurColor.ClampToByte(conv.B));
            }

            public static implicit operator global::System.Drawing.Color(ModBase.OurColor conv)
            {
                return global::System.Drawing.Color.FromArgb((int)ModBase.OurColor.ClampToByte(conv.A), (int)ModBase.OurColor.ClampToByte(conv.R), (int)ModBase.OurColor.ClampToByte(conv.G), (int)ModBase.OurColor.ClampToByte(conv.B));
            }

            public static implicit operator ModBase.OurColor(SolidColorBrush bru)
            {
                return new ModBase.OurColor(bru.Color);
            }

            public static implicit operator SolidColorBrush(ModBase.OurColor conv)
            {
                return new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(ModBase.OurColor.ClampToByte(conv.A), ModBase.OurColor.ClampToByte(conv.R), ModBase.OurColor.ClampToByte(conv.G), ModBase.OurColor.ClampToByte(conv.B)));
            }

            public static implicit operator ModBase.OurColor(global::System.Windows.Media.Brush bru)
            {
                return new ModBase.OurColor(bru);
            }

            public static implicit operator global::System.Windows.Media.Brush(ModBase.OurColor conv)
            {
                return new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(ModBase.OurColor.ClampToByte(conv.A), ModBase.OurColor.ClampToByte(conv.R), ModBase.OurColor.ClampToByte(conv.G), ModBase.OurColor.ClampToByte(conv.B)));
            }

            public static ModBase.OurColor operator +(ModBase.OurColor a, ModBase.OurColor b)
            {
                return new ModBase.OurColor
                {
                    A = a.A + b.A,
                    B = a.B + b.B,
                    G = a.G + b.G,
                    R = a.R + b.R
                };
            }

            public static ModBase.OurColor operator -(ModBase.OurColor a, ModBase.OurColor b)
            {
                return new ModBase.OurColor
                {
                    A = a.A - b.A,
                    B = a.B - b.B,
                    G = a.G - b.G,
                    R = a.R - b.R
                };
            }

            public static ModBase.OurColor operator *(ModBase.OurColor a, double b)
            {
                return new ModBase.OurColor
                {
                    A = a.A * b,
                    B = a.B * b,
                    G = a.G * b,
                    R = a.R * b
                };
            }

            public static ModBase.OurColor operator /(ModBase.OurColor a, double b)
            {
                return new ModBase.OurColor
                {
                    A = a.A / b,
                    B = a.B / b,
                    G = a.G / b,
                    R = a.R / b
                };
            }

            public static bool operator ==(ModBase.OurColor a, ModBase.OurColor b)
            {
                return (Information.IsNothing(a) && Information.IsNothing(b)) || (!Information.IsNothing(a) && !Information.IsNothing(b) && (a.A == b.A && a.R == b.R && a.G == b.G) && a.B == b.B);
            }

            public static bool operator !=(ModBase.OurColor a, ModBase.OurColor b)
            {
                return (!Information.IsNothing(a) || !Information.IsNothing(b)) && (Information.IsNothing(a) || Information.IsNothing(b) || a.A != b.A || a.R != b.R || a.G != b.G || a.B != b.B);
            }

            public static ModBase.OurColor Lerp(ModBase.OurColor ValueA, ModBase.OurColor ValueB, double Percent)
            {
                return ModBase.OurColor.Round(ValueA * (1.0 - Percent) + ValueB * Percent, 6);
            }

            public static ModBase.OurColor Round(ModBase.OurColor col, int w = 0)
            {
                return new ModBase.OurColor
                {
                    A = Math.Round(col.A, w),
                    R = Math.Round(col.R, w),
                    G = Math.Round(col.G, w),
                    B = Math.Round(col.B, w)
                };
            }

            public static byte ClampToByte(double d)
            {
                if (d < 0.0)
                {
                    d = 0.0;
                }
                if (d > 255.0)
                {
                    d = 255.0;
                }
                return checked((byte)Math.Round(d));
            }

            public OurColor()
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
            }

            public OurColor(global::System.Windows.Media.Color col)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                this.A = (double)col.A;
                this.R = (double)col.R;
                this.G = (double)col.G;
                this.B = (double)col.B;
            }

            public OurColor(string HexString)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                object obj = global::System.Windows.Media.ColorConverter.ConvertFromString(HexString);
                global::System.Windows.Media.Color color = ((obj != null) ? ((global::System.Windows.Media.Color)obj) : default(global::System.Windows.Media.Color));
                this.A = (double)color.A;
                this.R = (double)color.R;
                this.G = (double)color.G;
                this.B = (double)color.B;
            }

            public OurColor(double newA, ModBase.OurColor col)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                this.A = newA;
                this.R = col.R;
                this.G = col.G;
                this.B = col.B;
            }

            public OurColor(double newR, double newG, double newB)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                this.A = 255.0;
                this.R = newR;
                this.G = newG;
                this.B = newB;
            }

            public OurColor(double newA, double newR, double newG, double newB)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                this.A = newA;
                this.R = newR;
                this.G = newG;
                this.B = newB;
            }

            public OurColor(global::System.Windows.Media.Brush brush)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                global::System.Windows.Media.Color color = ((SolidColorBrush)brush).Color;
                this.A = (double)color.A;
                this.R = (double)color.R;
                this.G = (double)color.G;
                this.B = (double)color.B;
            }

            public OurColor(SolidColorBrush brush)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                global::System.Windows.Media.Color color = brush.Color;
                this.A = (double)color.A;
                this.R = (double)color.R;
                this.G = (double)color.G;
                this.B = (double)color.B;
            }

            public OurColor(object obj)
            {
                this.A = 255.0;
                this.R = 0.0;
                this.G = 0.0;
                this.B = 0.0;
                if (obj == null)
                {
                    this.A = 255.0;
                    this.R = 255.0;
                    this.G = 255.0;
                    this.B = 255.0;
                    return;
                }
                if (obj is SolidColorBrush)
                {
                    global::System.Windows.Media.Color color = ((SolidColorBrush)obj).Color;
                    this.A = (double)color.A;
                    this.R = (double)color.R;
                    this.G = (double)color.G;
                    this.B = (double)color.B;
                    return;
                }
                this.A = Conversions.ToDouble(NewLateBinding.LateGet(obj, null, "A", new object[0], null, null, null));
                this.R = Conversions.ToDouble(NewLateBinding.LateGet(obj, null, "R", new object[0], null, null, null));
                this.G = Conversions.ToDouble(NewLateBinding.LateGet(obj, null, "G", new object[0], null, null, null));
                this.B = Conversions.ToDouble(NewLateBinding.LateGet(obj, null, "B", new object[0], null, null, null));
            }

            public double Hue(double v1, double v2, double vH)
            {
                if (vH < 0.0)
                {
                    vH += 1.0;
                }
                if (vH > 1.0)
                {
                    vH -= 1.0;
                }
                double num;
                if (vH < 0.16667)
                {
                    num = v1 + (v2 - v1) * 6.0 * vH;
                }
                else if (vH < 0.5)
                {
                    num = v2;
                }
                else if (vH < 0.66667)
                {
                    num = v1 + (v2 - v1) * (4.0 - vH * 6.0);
                }
                else
                {
                    num = v1;
                }
                return num;
            }

            public ModBase.OurColor FromHSL(double sH, double sS, double sL)
            {
                this.A = 255.0;
                ModBase.OurColor ourColor;
                if (sS == 0.0)
                {
                    this.R = sL * 2.55;
                    this.G = this.R;
                    this.B = this.R;
                    ourColor = this;
                }
                else
                {
                    double num = sH / 360.0;
                    double num2 = sS / 100.0;
                    double num3 = sL / 100.0;
                    num2 = ((num3 < 0.5) ? (num2 * num3 + num3) : (num2 * (1.0 - num3) + num3));
                    num3 = 2.0 * num3 - num2;
                    this.R = 255.0 * this.Hue(num3, num2, num + 0.3333333333333333);
                    this.G = 255.0 * this.Hue(num3, num2, num);
                    this.B = 255.0 * this.Hue(num3, num2, num - 0.3333333333333333);
                    ourColor = this;
                }
                return ourColor;
            }

            public ModBase.OurColor FromHSL2(double sH, double sS, double sL)
            {
                this.A = 255.0;
                ModBase.OurColor ourColor;
                if (sS == 0.0)
                {
                    this.R = sL * 2.55;
                    this.G = this.R;
                    this.B = this.R;
                    ourColor = this;
                }
                else
                {
                    sH = (sH + 3600000.0) % 360.0;
                    double[] array = new double[]
                    {
                        0.1, -0.06, -0.3, -0.19, -0.15, -0.24, -0.32, -0.09, 0.18, 0.05,
                        -0.12, -0.02, 0.1, -0.06
                    };
                    double num = sH / 30.0;
                    int num2 = checked((int)Math.Floor(num));
                    num = 50.0 - ((1.0 - num + (double)num2) * array[num2] + (num - (double)num2) * array[checked(num2 + 1)]) * sS;
                    sL = ((sL < num) ? (sL / num) : (1.0 + (sL - num) / (100.0 - num))) * 50.0;
                    this.FromHSL(sH, sS, sL);
                    ourColor = this;
                }
                return ourColor;
            }

            public override string ToString()
            {
                return string.Concat(new string[]
                {
                    "(",
                    Conversions.ToString(this.A),
                    ",",
                    Conversions.ToString(this.R),
                    ",",
                    Conversions.ToString(this.G),
                    ",",
                    Conversions.ToString(this.B),
                    ")"
                });
            }

            public override bool Equals(object obj)
            {
                return this == (ModBase.OurColor)obj;
            }

            public double A;

            public double R;

            public double G;

            public double B;
        }

        public class OurRect
        {
            public double Width
            {
                [CompilerGenerated]
                get
                {
                    return this._Width;
                }
                [CompilerGenerated]
                set
                {
                    this._Width = value;
                }
            }

            public double Height
            {
                [CompilerGenerated]
                get
                {
                    return this._Height;
                }
                [CompilerGenerated]
                set
                {
                    this._Height = value;
                }
            }

            public double Left
            {
                [CompilerGenerated]
                get
                {
                    return this._Left;
                }
                [CompilerGenerated]
                set
                {
                    this._Left = value;
                }
            }

            public double Top
            {
                [CompilerGenerated]
                get
                {
                    return this._Top;
                }
                [CompilerGenerated]
                set
                {
                    this._Top = value;
                }
            }

            public OurRect()
            {
                this.Width = 0.0;
                this.Height = 0.0;
                this.Left = 0.0;
                this.Top = 0.0;
            }

            public OurRect(double left, double top, double width, double height)
            {
                this.Width = 0.0;
                this.Height = 0.0;
                this.Left = 0.0;
                this.Top = 0.0;
                this.Left = left;
                this.Top = top;
                this.Width = width;
                this.Height = height;
            }

            [CompilerGenerated]
            private double _Width;

            [CompilerGenerated]
            private double _Height;

            [CompilerGenerated]
            private double _Left;

            [CompilerGenerated]
            private double _Top;
        }

        public enum LoadState
        {
            Waiting,
            Loading,
            Finished,
            Failed,
            Canceled
        }

        public enum ProcessReturnValues
        {
            Success,
            Fail,
            Exception,
            Timeout,
            Cancel,
            TaskDone
        }

        public class SearchEntry<T>
        {
            public SearchEntry()
            {
            }

            public override string ToString()
            {
                return Conversions.ToString(Math.Round(this.Similarity, 3)) + " - " + this.Item.ToString();
            }

            public T Item;

            public List<ModBase.SearchSource> SearchSource;

            public double Similarity;

            public bool AbsoluteRight;
        }

        public class SearchSource
        {
            public SearchSource(string[] Aliases, double Weight = 1.0)
            {
                this.Aliases = Aliases;
                this.Weight = Weight;
            }

            public SearchSource(string Text, double Weight = 1.0)
            {
                this.Aliases = new string[] { Text };
                this.Weight = Weight;
            }

            public string[] Aliases;

            public double Weight;
        }

        public class RestartException : Exception
        {
            public RestartException()
            {
            }
        }

        public sealed class RouteEventArgs : EventArgs
        {
            public RouteEventArgs(bool RaiseByMouse = false)
            {
                this.Handled = false;
                this.RaiseByMouse = RaiseByMouse;
            }

            public bool RaiseByMouse;

            public bool Handled;
        }

        [CompilerGenerated]
        private sealed class VB_StateMachine_84_Shuffle<SM_T> : IEnumerable<SM_T>, IEnumerable, IEnumerator<SM_T>, IDisposable, IEnumerator
        {
            public VB_StateMachine_84_Shuffle(int _State)
            {
                this._State = _State;
                this._InitialThreadId = Environment.CurrentManagedThreadId;
            }

            void IDisposable.Dispose()
            {
                this._State = -2;
            }

            [CompilerGenerated]
            public bool MoveNext()
            {
                int num = this._State;
                if (num != 0)
                {
                    if (num != 1)
                    {
                        return false;
                    }
                    this._State = -1;
                    this._VB_ResumableLocal_RawCopy_0.RemoveAt(this._VB_ResumableLocal_i_1);
                }
                else
                {
                    this._State = -1;
                    this._VB_ResumableLocal_RawCopy_0 = new List<SM_T>(this._VB_Local_Raw);
                }
                if (!this._VB_ResumableLocal_RawCopy_0.Any<SM_T>())
                {
                    return false;
                }
                this._VB_ResumableLocal_i_1 = ModBase.RandomInteger(0, checked(this._VB_ResumableLocal_RawCopy_0.Count - 1));
                this._Current = this._VB_ResumableLocal_RawCopy_0[this._VB_ResumableLocal_i_1];
                this._State = 1;
                return true;
            }

            public IEnumerator<SM_T> GetEnumerator()
            {
                ModBase.VB_StateMachine_84_Shuffle<SM_T> vb_StateMachine_84_Shuffle;
                if (this._State == -2 && this._InitialThreadId == Environment.CurrentManagedThreadId)
                {
                    this._State = 0;
                    vb_StateMachine_84_Shuffle = this;
                }
                else
                {
                    vb_StateMachine_84_Shuffle = new ModBase.VB_StateMachine_84_Shuffle<SM_T>(0);
                }
                vb_StateMachine_84_Shuffle._VB_Local_Raw = this._P_Raw;
                return vb_StateMachine_84_Shuffle;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            SM_T IEnumerator<SM_T>.Current
            {
                get
                {
                    return this._Current;
                }
            }

            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator.Current
            {
                get
                {
                    return this._Current;
                }
            }

            public int _State;

            public SM_T _Current;

            public int _InitialThreadId;

            internal IEnumerable<SM_T> _VB_Local_Raw;

            internal IEnumerable<SM_T> _P_Raw;

            internal List<SM_T> _VB_ResumableLocal_RawCopy_0;

            internal int _VB_ResumableLocal_i_1;
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal bool _Lambda___0_0(string p)
            {
                return Directory.Exists(p);
            }

            internal string _Lambda___0_1()
            {
                string text;
                if (Paths.Base.IsAsciiOnly())
                {
                    text = Paths.Base + "PCL\\";
                }
                else if (Paths.AppDataThenName.IsAsciiOnly())
                {
                    text = Paths.AppDataThenName;
                }
                else if (ModBase.PathTemp.IsAsciiOnly())
                {
                    text = ModBase.PathTemp;
                }
                else
                {
                    text = ModBase.OsDrive + "ProgramData\\PCL\\";
                }
                return text;
            }

            internal TimeSpan _Lambda___68_3(int Attempt)
            {
                return TimeSpan.FromMilliseconds(200.0);
            }

            internal bool _Lambda___68_4(Exception ex)
            {
                return true;
            }

            public static readonly ModBase._Closure___ _I = new ModBase._Closure___();

            public static Func<int, TimeSpan> _I68_3;

            public static Func<Exception, bool> _I68_4;
        }

        [CompilerGenerated]
        private sealed class _Closure___39_0
        {
            public _Closure___39_0(ModBase._Closure___39_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Query = arg0._VB_Local_Query;
                }
            }

            internal double _Lambda___0(string a)
            {
                return ModBase.SearchSimilarity(a, this._VB_Local_Query);
            }

            public string _VB_Local_Query;

            public Func<string, double> _I0;
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___42<_CLS0>
        {
            public _Closure___42()
            {
            }

            static _Closure___42()
            {
            }

            internal bool _Lambda___42_3(ModBase.SearchEntry<_CLS0> Left, ModBase.SearchEntry<_CLS0> Right)
            {
                if (Left.AbsoluteRight == Right.AbsoluteRight)
                {
                    return Left.Similarity > Right.Similarity;
                }
                return Left.AbsoluteRight;
            }

            public static readonly ModBase._Closure___42<_CLS0> _I = new ModBase._Closure___42<_CLS0>();

            public static Func<ModBase.SearchEntry<_CLS0>, ModBase.SearchEntry<_CLS0>, bool> _I42_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___42_0<_CLS0>
        {
            public _Closure___42_0(ModBase._Closure___42_0<_CLS0> arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Entry = arg0._VB_Local_Entry;
                }
            }

            internal bool _Lambda___0(string QueryPart)
            {
                ModBase._Closure___42_1<_CLS0> CS_locals1 = new ModBase._Closure___42_1<_CLS0>(null);
                CS_locals1._VB_Local_QueryPart = QueryPart;
                return this._VB_Local_Entry.SearchSource.Any<ModBase.SearchSource>(new Func<ModBase.SearchSource, bool>(CS_locals1._Lambda___1));
            }

            public ModBase.SearchEntry<_CLS0> _VB_Local_Entry;
        }

        [CompilerGenerated]
        private sealed class _Closure___42_1<_CLS0>
        {
            public _Closure___42_1(ModBase._Closure___42_1<_CLS0> arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_QueryPart = arg0._VB_Local_QueryPart;
                }
            }

            internal bool _Lambda___1(ModBase.SearchSource Source)
            {
                return Source.Aliases.Any<string>((this._I2 == null) ? (this._I2 = new Func<string, bool>(this._Lambda___2)) : this._I2);
            }

            internal bool _Lambda___2(string Alias)
            {
                return Alias.Replace(" ", "").ContainsIgnoreCase(this._VB_Local_QueryPart);
            }

            public string _VB_Local_QueryPart;

            public Func<string, bool> _I2;
        }

        [CompilerGenerated]
        private sealed class _Closure___59_0
        {
            public _Closure___59_0()
            {
            }

            internal void _Lambda___0()
            {
                try
                {
                    this._VB_Local_Action();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("{0}：线程执行失败", this._VB_Local_Name), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                }
            }

            public Action _VB_Local_Action;

            public string _VB_Local_Name;
        }

        [CompilerGenerated]
        private sealed class _Closure___68_0
        {
            public _Closure___68_0()
            {
            }

            internal void _Lambda___0()
            {
                try
                {
                    Retrier.Attempt((this._IR1 == null) ? (this._IR1 = new Action<int>(this._Lambda___R1)) : this._IR1, new uint?(4U), (ModBase._Closure___._I68_3 == null) ? (ModBase._Closure___._I68_3 = new Func<int, TimeSpan>(ModBase._Closure___._I._Lambda___68_3)) : ModBase._Closure___._I68_3, (ModBase._Closure___._I68_4 == null) ? (ModBase._Closure___._I68_4 = new Func<Exception, bool>(ModBase._Closure___._I._Lambda___68_4)) : ModBase._Closure___._I68_4, null, default(CancellationToken));
                    if (this._VB_Local_ShowSuccessHint)
                    {
                        ModMain.Hint("已成功复制！", ModMain.HintType.Green, true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "可能由于剪贴板被其他程序占用，文本复制失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                }
            }

            internal void _Lambda___R1(int a0)
            {
                ((this._I1 == null) ? (this._I1 = new VB_AnonymousDelegate_10(this._Lambda___1)) : this._I1)();
            }

            internal void _Lambda___1()
            {
                ModBase.RunInUiWait((this._I2 == null) ? (this._I2 = new Action(this._Lambda___2)) : this._I2);
            }

            internal void _Lambda___2()
            {
                OurWpfExtension.Computer.Clipboard.Clear();
                if (!string.IsNullOrEmpty(this._VB_Local_Text))
                {
                    OurWpfExtension.Computer.Clipboard.SetText(this._VB_Local_Text);
                }
            }

            public string _VB_Local_Text;

            public bool _VB_Local_ShowSuccessHint;

            public Action _I2;

            public VB_AnonymousDelegate_10 _I1;

            public Action<int> _IR1;
        }

        [CompilerGenerated]
        private sealed class _Closure___69_0
        {
            public _Closure___69_0()
            {
            }

            internal void _Lambda___0()
            {
                try
                {
                    if (OurWpfExtension.Computer.Clipboard.ContainsText())
                    {
                        this._VB_Local_Result = OurWpfExtension.Computer.Clipboard.GetText();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "获取剪贴板文本失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModBase.vb");
                }
            }

            public string _VB_Local_Result;
        }
    }
}
