using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCL.Our;
using PCL.Our.Resources;

namespace PCL
{
    public partial class Application : System.Windows.Application
    {
        private void Application_Startup(object sender, global::System.Windows.StartupEventArgs e)
        {
            checked
            {
                try
                {
                    global::MeloongCore.Main.Init("PCL");
                    global::MeloongCore.Wpf.Main.Init();
                    if (!e.Args.Contains("--update") && !e.Args.Contains("--gpu") && !e.Args.Contains("--memory"))
                    {
                        Application.WaitingMutex(e);
                        Logger.Instance = new PclLogger
                        {
                            logFolder = Paths.BaseThenName,
                            MinLevel = (ModBase.ModeDebug ? LogLevel.Trace : LogLevel.Info)
                        };
                    }
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                    try
                    {
                        if (Conversions.ToInteger(RegistryUtils.TryRead("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full", "Release", 999999)) < 528040)
                        {
                            Application.OldEnvironmentAssert(null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "检查 .NET Framework 版本失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    }
                    try
                    {
                        new FormattedText("", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Fonts.SystemTypefaces.First<Typeface>(), 96.0, new ModBase.OurColor(), (double)ModBase.DPI);
                    }
                    catch (UriFormatException ex2)
                    {
                        Environment.SetEnvironmentVariable("windir", Environment.GetEnvironmentVariable("SystemRoot"), EnvironmentVariableTarget.User);
                        new FormattedText("", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Fonts.SystemTypefaces.First<Typeface>(), 96.0, new ModBase.OurColor(), (double)ModBase.DPI);
                    }
                    if (e.Args.Length > 0)
                    {
                        if (Operators.CompareString(e.Args[0], "--update", false) == 0)
                        {
                            ModSecret.UpdateReplace(Conversions.ToInteger(e.Args[1]), e.Args[2].Trim(new char[] { '"' }), e.Args[3].Trim(new char[] { '"' }), Conversions.ToBoolean(e.Args[4]));
                            Environment.Exit(5);
                        }
                        else
                        {
                            if (Operators.CompareString(e.Args[0], "--gpu", false) == 0)
                            {
                                try
                                {
                                    ModMain.SetGPUPreference(e.Args[1].Trim(new char[] { '"' }));
                                    Environment.Exit(5);
                                    goto IL_02C6;
                                }
                                catch (Exception ex3)
                                {
                                    Environment.Exit(1);
                                    goto IL_02C6;
                                }
                            }
                            if (Operators.CompareString(e.Args[0], "--memory", false) == 0)
                            {
                                ulong availablePhysicalMemory = OurWpfExtension.Computer.Info.AvailablePhysicalMemory;
                                try
                                {
                                    PageOtherTest.MemoryOptimizeInternal(false);
                                }
                                catch (Exception ex4)
                                {
                                    Interaction.MsgBox(ex4.Message, MsgBoxStyle.Critical, "内存优化失败");
                                    Environment.Exit(-1);
                                }
                                if (OurWpfExtension.Computer.Info.AvailablePhysicalMemory < availablePhysicalMemory)
                                {
                                    Environment.Exit(0);
                                }
                                else
                                {
                                    Environment.Exit((int)Math.Round((OurWpfExtension.Computer.Info.AvailablePhysicalMemory - availablePhysicalMemory) / 1024.0));
                                }
                            }
                        }
                    }
                    IL_02C6:
                    try
                    {
                        DirectoryUtils.Create(Paths.Base + "PCL\\Pictures\\");
                        DirectoryUtils.Create(Paths.Base + "PCL\\Musics\\");
                        ModBase.CheckPermissionWithException(Paths.Base + "PCL\\");
                    }
                    catch (Exception ex5)
                    {
                        string text = string.Format("PCL 没有对当前文件夹的权限（{0}PCL\\），请尝试：", Paths.Base);
                        string text2 = "\r\n1. 将 PCL 移动到其他文件夹";
                        char? diskName = PathUtils.GetDiskName(Paths.Base);
                        Interaction.MsgBox(text + text2 + (((diskName != null) ? new bool?(diskName.GetValueOrDefault() == 'C') : null).GetValueOrDefault() ? "，例如 C 盘和桌面以外的其他位置。" : "。") + "\r\n2. 删除当前目录中的 PCL 文件夹，然后再试。\r\n3. 右键 PCL 选择属性，打开 兼容性 中的 以管理员身份运行此程序。", MsgBoxStyle.Critical, "运行环境错误");
                        Environment.Exit(4);
                    }
                    for (;;)
                    {
                        try
                        {
                            DirectoryUtils.Create(ModBase.PathTemp);
                            ModBase.CheckPermissionWithException(ModBase.PathTemp);
                        }
                        catch (Exception ex6)
                        {
                            if (!PathUtils.IsSame(ModBase.PathTemp, Path.GetTempPath() + "PCL\\"))
                            {
                                ModMain.OurMsgBox("手动设置的缓存文件夹不可用，PCL 将使用默认缓存文件夹。\r\n\r\n错误原因：" + ex6.GetDisplay(true), "缓存文件夹不可用", "确定", "", "", false, true, false, null, null, null);
                                Settings.Set("SystemSystemCache", "", null);
                                ModBase.PathTemp = Path.GetTempPath() + "PCL\\";
                                continue;
                            }
                            ModMain.OurMsgBox("PCL 无法访问缓存文件夹，可能导致程序出错或无法正常使用！\r\n\r\n错误原因：" + ex6.GetDisplay(true), "缓存文件夹不可用", "确定", "", "", false, true, false, null, null, null);
                        }
                        break;
                    }
                    DirectoryUtils.Create(ModBase.PathTemp + "Cache\\");
                    DirectoryUtils.Create(Paths.AppDataThenName);
                    ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(100));
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.DefaultConnectionLimit = 10000;
                    ServicePointManager.UseNagleAlgorithm = false;
                    ServicePointManager.EnableDnsRoundRobin = true;
                    ServicePointManager.ReusePort = true;
                    if (Environment.OSVersion.Version < new Version(6, 2))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    }
                    if (Settings.Get<bool>("UiLauncherLogo", null))
                    {
                        ModMain.FrmStart = new SplashScreen("Images\\icon.ico");
                        ModMain.FrmStart.Show(false, true);
                    }
                    Logger.Info(string.Format("程序版本：{0} ({1}{2})", "快照版 2.99.0.0", 414, ""), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    Logger.Info(string.Format("识别码：{0}{1}", ModBase.Identify, ModSecret.ThemeCheckOne(9) ? "，已解锁反馈主题" : "，未解锁反馈主题"), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    Logger.Info(string.Format("程序路径：{0}", ModBase.PathExe), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    Logger.Info(string.Format("系统编码：{0} ({1}, GBK={2})", Encoding.Default.HeaderName, Encoding.Default.CodePage, ModBase.IsGBKEncoding), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    Logger.Info(string.Format("管理员权限：{0}", WindowsUtils.HasAdminRole()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    if (Paths.Base.Contains(Path.GetTempPath()) || Paths.Base.Contains("AppData\\Local\\Temp\\"))
                    {
                        ModMain.OurMsgBox("请将 PCL 从压缩包中解压后再使用！\r\n如果不会解压，可以在网上寻找教程。", "需要解压！", "我知道了", "", "", true, true, true, null, null, null);
                        FormMain.EndProgramForce(ModBase.ProcessReturnValues.Cancel);
                    }
                    if (!Environment.Is64BitOperatingSystem)
                    {
                        ModMain.OurMsgBox("PCL 和新版 Minecraft 均不再支持 32 位系统，请重装为 64 位系统后再进行游戏！", "环境警告", "我知道了", "", "", true, true, true, null, null, null);
                        FormMain.EndProgramForce(ModBase.ProcessReturnValues.Cancel);
                    }
                    Logger.Info(string.Format("第一阶段加载用时：{0} ms", ModBase.GetTimeMs() - ModBase.ApplicationStartTick), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    ModBase.ApplicationStartTick = ModBase.GetTimeMs();
                    ModAnimation.AniControlEnabled++;
                }
                catch (Exception ex7)
                {
                    string text3 = null;
                    try
                    {
                        text3 = ModBase.PathExe;
                    }
                    catch (Exception ex8)
                    {
                    }
                    Interaction.MsgBox(ex7.GetDisplay(true) + "\r\nPCL 所在路径：" + (string.IsNullOrEmpty(text3) ? "获取失败" : text3), MsgBoxStyle.Critical, "PCL 初始化错误");
                    FormMain.EndProgramForce(ModBase.ProcessReturnValues.Exception);
                }
            }
        }

        private static void WaitingMutex(global::System.Windows.StartupEventArgs e)
        {
            try
            {
                bool flag;
                Application.PclMutex = new Mutex(true, "PCL_SingletonMutex", out flag);
                if (!flag)
                {
                    Logger.Warn("已有一个 PCL 实例正在运行", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                    if (e.Args.Length > 0 && Operators.CompareString(e.Args[0], "--wait", false) == 0)
                    {
                        try
                        {
                            if (Application.PclMutex.WaitOne(10000))
                            {
                                return;
                            }
                        }
                        catch (AbandonedMutexException ex)
                        {
                            return;
                        }
                    }
                    string text = null;
                    string text2 = "OpenPCL\u3000";
                    IntPtr intPtr = ModMain.FindWindow(ref text, ref text2);
                    if (intPtr == IntPtr.Zero)
                    {
                        text2 = null;
                        text = "OpenPCL\u3000";
                        intPtr = ModMain.FindWindow(ref text2, ref text);
                    }
                    if (intPtr != IntPtr.Zero)
                    {
                        ModMain.ShowWindowToTop(intPtr);
                    }
                    Interaction.Beep();
                    Environment.Exit(4);
                }
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, "单例检查失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
            }
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            FormMain frmMain = ModMain.FrmMain;
            if (frmMain == null)
            {
                return;
            }
            frmMain.EndProgram(false);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            int num = 0;
            int num4;
            object obj;
            int num2 = 0;
            FormMain frmMain = default;
            FormMain frmMain2 = default;
            string display = default;
            int __onErrorFrom_IL_01D1 = -1;
            ONERR_IL_01D1:
            try
            {
                if (__onErrorFrom_IL_01D1 >= 0)
                {
                    if (__onErrorFrom_IL_01D1 < 2) goto IL_0007;
                    if (__onErrorFrom_IL_01D1 < 3) goto IL_002D;
                    if (__onErrorFrom_IL_01D1 < 4) goto IL_004A;
                    if (__onErrorFrom_IL_01D1 < 6) goto IL_0057;
                    if (__onErrorFrom_IL_01D1 < 7) goto IL_007D;
                    if (__onErrorFrom_IL_01D1 < 8) goto IL_009A;
                    if (__onErrorFrom_IL_01D1 < 10) goto IL_00A7;
                    if (__onErrorFrom_IL_01D1 < 11) goto IL_00B1;
                    if (__onErrorFrom_IL_01D1 < 13) goto IL_00BB;
                    if (__onErrorFrom_IL_01D1 < 14) goto IL_00C3;
                    if (__onErrorFrom_IL_01D1 < 15) goto IL_00D3;
                    if (__onErrorFrom_IL_01D1 < 16) goto IL_010A;
                    if (__onErrorFrom_IL_01D1 < 18) goto IL_011A;
                    goto IL_01D1;
                }
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                num2 = 2;
                frmMain = ModMain.FrmMain;
                if (((frmMain != null) ? frmMain.PageLeft : null) == null || !(ModMain.FrmMain.PageLeft is IDispatcherUnhandledException))
                {
                    goto IL_0057;
                }
                IL_002D:
                num2 = 3;
                ((IDispatcherUnhandledException)ModMain.FrmMain.PageLeft).DispatcherUnhandledException(RuntimeHelpers.GetObjectValue(sender), e);
                IL_004A:
                num2 = 4;
                if (e.Handled)
                {
                    goto IL_0133;
                }
                IL_0057:
                num2 = 6;
                frmMain2 = ModMain.FrmMain;
                if (((frmMain2 != null) ? frmMain2.PageRight : null) == null || !(ModMain.FrmMain.PageRight is IDispatcherUnhandledException))
                {
                    goto IL_00A7;
                }
                IL_007D:
                num2 = 7;
                ((IDispatcherUnhandledException)ModMain.FrmMain.PageRight).DispatcherUnhandledException(RuntimeHelpers.GetObjectValue(sender), e);
                IL_009A:
                num2 = 8;
                if (e.Handled)
                {
                    goto IL_0133;
                }
                IL_00A7:
                num2 = 10;
                e.Handled = true;
                IL_00B1:
                num2 = 11;
                if (ModBase.IsProgramEnding)
                {
                    goto IL_0133;
                }
                IL_00BB:
                num2 = 13;
                ModMain.FeedbackInfo();
                IL_00C3:
                num2 = 14;
                display = e.Exception.GetDisplay(true);
                IL_00D3:
                num2 = 15;
                if (!display.Contains("System.Windows.Threading.Dispatcher.Invoke") && !display.Contains("MS.Internal.AppModel.ITaskbarList.HrInit") && !display.Contains("未能加载文件或程序集") && !display.Contains(".NET Framework"))
                {
                    goto IL_011A;
                }
                IL_010A:
                num2 = 16;
                Application.OldEnvironmentAssert(e.Exception);
                goto IL_0133;
                IL_011A:
                num2 = 18;
                Logger.Error(e.Exception, "程序出现未知错误", LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                IL_0133:
                goto IL_01D1;
            }
            catch (Exception) when (num != 0)
            {
                __onErrorFrom_IL_01D1 = num2;
                goto ONERR_IL_01D1;
            }
            IL_01D1:;
        }

        private static void OldEnvironmentAssert(Exception ex = null)
        {
            ModBase.OpenWebsite("https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/thank-you/net48-offline-installer");
            if (ex == null)
            {
                Logger.Error(string.Format("你的 .NET Framework 版本过低或损坏，请下载并重新安装 .NET Framework 4.8！{0}若安装失败，可以先卸载高版本的 .NET Framework 然后再试。", "\r\n"), LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
                return;
            }
            Logger.Error(ex, string.Format("你的 .NET Framework 版本过低或损坏，请下载并重新安装 .NET Framework 4.8！{0}若安装失败，可以先卸载高版本的 .NET Framework 然后再试。", "\r\n"), LogBehavior.AlertThenCrash, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
        }

        private Application()
        {
            base.Startup += this.Application_Startup;
            base.SessionEnding += this.Application_SessionEnding;
            base.DispatcherUnhandledException += this.Application_DispatcherUnhandledException;
            if (this._STATIC_ctor_2001_Prefixes_Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_ctor_2001_Prefixes_Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this._STATIC_ctor_2001_Prefixes_Init, ref flag);
                if (this._STATIC_ctor_2001_Prefixes_Init.State == 0)
                {
                    this._STATIC_ctor_2001_Prefixes_Init.State = 2;
                    this._STATIC_ctor_2001_Prefixes = new string[]
                    {
                        "NAudio", "Newtonsoft.Json", "Ookii.Dialogs.Wpf", "Imazen.WebP", "CacheCow.Common", "CacheCow.Client.FileStore", "CacheCow.Client", "ThrottleDebounce", "System.Net.Http.Formatting", "Microsoft.Win32.Registry",
                        "PCLCS", "MeloongCore.Wpf", "MeloongCore"
                    };
                }
                else if (this._STATIC_ctor_2001_Prefixes_Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this._STATIC_ctor_2001_Prefixes_Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this._STATIC_ctor_2001_Prefixes_Init);
                }
            }
            if (this._STATIC_ctor_2001_LoadedAssemblies_Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_ctor_2001_LoadedAssemblies_Init, new StaticLocalInitFlag(), null);
            }
            bool flag2 = false;
            try
            {
                Monitor.Enter(this._STATIC_ctor_2001_LoadedAssemblies_Init, ref flag2);
                if (this._STATIC_ctor_2001_LoadedAssemblies_Init.State == 0)
                {
                    this._STATIC_ctor_2001_LoadedAssemblies_Init.State = 2;
                    this._STATIC_ctor_2001_LoadedAssemblies = new ConcurrentDictionary<string, Lazy<Assembly>>(StringComparer.Ordinal);
                }
                else if (this._STATIC_ctor_2001_LoadedAssemblies_Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this._STATIC_ctor_2001_LoadedAssemblies_Init.State = 1;
                if (flag2)
                {
                    Monitor.Exit(this._STATIC_ctor_2001_LoadedAssemblies_Init);
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve += this._Lambda___6_0;
        }

        private static void ExtractLibwebp()
        {
            string text = PathUtils.RemoveSlashSuffix(ModBase.PathPure.Value);
            Application.SetDllDirectory(ref text);
            try
            {
                ModBase.ExtractResources(ModBase.PathPure.Value + "libwebp.dll", "libwebp64");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "写入 libwebp.dll 失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Application.xaml.vb");
            }
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "SetDllDirectoryA", ExactSpelling = true, SetLastError = true)]
        private static extern bool SetDllDirectory([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpPathName);

        private void OurIconButton_Click(object sender, EventArgs e)
        {
            ModLaunch.McLoginType mcLoginType = Settings.Get<ModLaunch.McLoginType>("LoginType", null);
            if (mcLoginType == ModLaunch.McLoginType.Legacy)
            {
                List<string> list = new List<string>();
                list.AddRange(Settings.Get<string>("LoginLegacyName", null).ToString().Split("\u00a8", false));
                list.Remove(Conversions.ToString(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null)));
                Settings.Set("LoginLegacyName", list.Join('\u00a8'), null);
                ModMain.FrmLoginLegacy.ComboName.ItemsSource = list;
                ModMain.FrmLoginLegacy.ComboName.Text = (list.Any<string>() ? list[0] : "");
                return;
            }
            if (mcLoginType == ModLaunch.McLoginType.Ms)
            {
                JObject jobject = (JObject)Settings.Get<string>("LoginMsJson", null).DeserializeJson();
                jobject.Remove(Conversions.ToString(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null)));
                Settings.Set("LoginMsJson", jobject.ToString(Formatting.None, new JsonConverter[0]), null);
                if (ModMain.FrmLoginMs.ComboAccounts.SelectedItem == NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null))
                {
                    ModMain.FrmLoginMs.ComboAccounts.SelectedIndex = 0;
                }
                ModMain.FrmLoginMs.ComboAccounts.Items.Remove(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null)));
                return;
            }
            string text = Settings.Get<ModLaunch.McLoginType>("LoginType", null).ToString();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<string> list2 = new List<string>();
            List<string> list3 = new List<string>();
            if (Operators.CompareString(Settings.Get<string>("Login" + text + "Email", null), "", false) != 0)
            {
                list2.AddRange(Settings.Get<string>("Login" + text + "Email", null).Split("\u00a8", false));
            }
            if (Operators.CompareString(Settings.Get<string>("Login" + text + "Pass", null), "", false) != 0)
            {
                list3.AddRange(Settings.Get<string>("Login" + text + "Pass", null).Split("\u00a8", false));
            }
            checked
            {
                int num = list2.Count - 1;
                for (int i = 0; i <= num; i++)
                {
                    dictionary.Add(list2[i], list3[i]);
                }
                dictionary.Remove(Conversions.ToString(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null)));
                Settings.Set("Login" + text + "Email", dictionary.Keys.Join('\u00a8'), null);
                Settings.Set("Login" + text + "Pass", dictionary.Values.Join('\u00a8'), null);
                if (Operators.CompareString(text, "Nide", false) == 0)
                {
                    ModMain.FrmLoginNide.ComboName.ItemsSource = dictionary.Keys;
                    ModMain.FrmLoginNide.ComboName.Text = (dictionary.Keys.Any<string>() ? dictionary.Keys.ElementAtOrDefault<string>(0) : "");
                    ModMain.FrmLoginNide.TextPass.Password = (dictionary.Values.Any<string>() ? dictionary.Values.ElementAtOrDefault<string>(0) : "");
                    return;
                }
                if (Operators.CompareString(text, "Auth", false) != 0)
                {
                    return;
                }
                ModMain.FrmLoginAuth.ComboName.ItemsSource = dictionary.Keys;
                ModMain.FrmLoginAuth.ComboName.Text = (dictionary.Keys.Any<string>() ? dictionary.Keys.ElementAtOrDefault<string>(0) : "");
                ModMain.FrmLoginAuth.TextPass.Password = (dictionary.Values.Any<string>() ? dictionary.Values.ElementAtOrDefault<string>(0) : "");
            }
        }

        internal AssemblyInfo Info
        {
            [DebuggerHidden]
            get
            {
                return new AssemblyInfo(Assembly.GetExecutingAssembly());
            }
        }

        [CompilerGenerated]
        private Assembly _Lambda___6_0(object sender, ResolveEventArgs Args)
        {
            Application._Closure___6_0 CS_locals1 = new Application._Closure___6_0();
            CS_locals1._VB_Local_Args = Args;
            string text = this._STATIC_ctor_2001_Prefixes.FirstOrDefault<string>(new Func<string, bool>(CS_locals1._Lambda___1));
            Assembly assembly;
            if (text == null)
            {
                assembly = null;
            }
            else
            {
                assembly = this._STATIC_ctor_2001_LoadedAssemblies.GetOrAdd(text, (Application._Closure___._I6_2 == null) ? (Application._Closure___._I6_2 = new Func<string, Lazy<Assembly>>(Application._Closure___._I._Lambda___6_2)) : Application._Closure___._I6_2).Value;
            }
            return assembly;
        }

        private static Mutex PclMutex;

        private string[] _STATIC_ctor_2001_Prefixes;

        private StaticLocalInitFlag _STATIC_ctor_2001_Prefixes_Init;

        private ConcurrentDictionary<string, Lazy<Assembly>> _STATIC_ctor_2001_LoadedAssemblies;

        private StaticLocalInitFlag _STATIC_ctor_2001_LoadedAssemblies_Init;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal Lazy<Assembly> _Lambda___6_2(string p)
            {
                return new Lazy<Assembly>(new Func<Assembly>(new Application._Closure___6_1
                {
                    _VB_Local_p = p
                }._Lambda___3), LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public static readonly Application._Closure___ _I = new Application._Closure___();

            public static Func<string, Lazy<Assembly>> _I6_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_0
        {
            internal bool _Lambda___1(string p)
            {
                return this._VB_Local_Args.Name.StartsWith(p, StringComparison.Ordinal);
            }

            public ResolveEventArgs _VB_Local_Args;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_1
        {
            internal Assembly _Lambda___3()
            {
                Assembly assembly = Assembly.Load((byte[])PCL.Our.Resources.Resources.ResourceManager.GetObject(this._VB_Local_p));
                if (Operators.CompareString(this._VB_Local_p, "Imazen.WebP", false) == 0)
                {
                    Application.ExtractLibwebp();
                }
                return assembly;
            }

            public string _VB_Local_p;
        }
    }
}
