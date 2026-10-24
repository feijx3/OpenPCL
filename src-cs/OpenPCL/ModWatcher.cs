using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [StandardModule]
    public static class ModWatcher
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModWatcher()
        {
        }

        private static void WatcherStateChanged()
        {
            bool flag = false;
            bool flag2 = true;
            try
            {
                foreach (ModWatcher.Watcher watcher in ModWatcher.McWatcherList)
                {
                    if (watcher.State == ModWatcher.Watcher.MinecraftState.Loading || watcher.State == ModWatcher.Watcher.MinecraftState.Running || (watcher.State == ModWatcher.Watcher.MinecraftState.Crashed && !watcher.GameProcess.HasExited))
                    {
                        flag = true;
                        break;
                    }
                    if (watcher.State == ModWatcher.Watcher.MinecraftState.Crashed || watcher.State == ModWatcher.Watcher.MinecraftState.Canceled)
                    {
                        flag2 = false;
                    }
                }
            }
            finally
            {
                List<ModWatcher.Watcher>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            if (ModWatcher.IsWatcherRunning != flag)
            {
                ModWatcher.IsWatcherRunning = flag;
                if (ModWatcher.IsWatcherRunning)
                {
                    ModWatcher.MinecraftStart();
                    return;
                }
                ModWatcher.MinecraftStop(flag2);
            }
        }

        private static void MinecraftStart()
        {
            ModLaunch.McLaunchLog("[全局] 出现运行中的 Minecraft", LogLevel.Info);
            ModWatcher.HasRunningMinecraft = true;
            ModMain.FrmMain.BtnExtraShutdown.ShowRefresh();
        }

        private static void MinecraftStop(bool TriggerLauncherShutdown)
        {
            ModLaunch.McLaunchLog("[全局] 已无运行中的 Minecraft", LogLevel.Info);
            ModWatcher.HasRunningMinecraft = false;
            ModMain.FrmMain.BtnExtraShutdown.ShowRefresh();
            if (Settings.Get<bool>("UiMusicStop", null))
            {
                ModBase.RunInUi((ModWatcher._Closure___._I6_0 == null) ? (ModWatcher._Closure___._I6_0 = new Action(ModWatcher._Closure___._I._Lambda___6_0)) : ModWatcher._Closure___._I6_0, false);
            }
            else if (Settings.Get<bool>("UiMusicStart", null))
            {
                ModBase.RunInUi((ModWatcher._Closure___._I6_1 == null) ? (ModWatcher._Closure___._I6_1 = new Action(ModWatcher._Closure___._I._Lambda___6_1)) : ModWatcher._Closure___._I6_1, false);
            }
            int num = Settings.Get<int>("LaunchArgumentVisible", null);
            if (num != 2)
            {
                if (num != 3)
                {
                    return;
                }
                ModBase.RunInUi((ModWatcher._Closure___._I6_4 == null) ? (ModWatcher._Closure___._I6_4 = new Action(ModWatcher._Closure___._I._Lambda___6_4)) : ModWatcher._Closure___._I6_4, false);
                return;
            }
            else
            {
                if (TriggerLauncherShutdown)
                {
                    ModBase.RunInUi((ModWatcher._Closure___._I6_2 == null) ? (ModWatcher._Closure___._I6_2 = new Action(ModWatcher._Closure___._I._Lambda___6_2)) : ModWatcher._Closure___._I6_2, false);
                    return;
                }
                ModBase.RunInUi((ModWatcher._Closure___._I6_3 == null) ? (ModWatcher._Closure___._I6_3 = new Action(ModWatcher._Closure___._I._Lambda___6_3)) : ModWatcher._Closure___._I6_3, false);
                return;
            }
        }

        public static List<ModWatcher.Watcher> McWatcherList = new List<ModWatcher.Watcher>();

        private static bool IsWatcherRunning = false;

        public static bool HasRunningMinecraft = false;

        public class Watcher
        {
            public Watcher(ModLoader.LoaderTask<Process, int> Loader, ModMinecraft.McInstance Instance, string WindowTitle)
            {
                ModWatcher.Watcher._Closure___5_0 CS_locals1 = new ModWatcher.Watcher._Closure___5_0(null);
                CS_locals1._VB_Local_Loader = Loader;
                CS_locals1._VB_Local_WindowTitle = WindowTitle;
                CS_locals1._VB_Me = this;
                this.WindowTitle = "";
                this._State = ModWatcher.Watcher.MinecraftState.Loading;
                this.WaitingLog = new List<string>(1000);
                this.WaitingLogLock = RuntimeHelpers.GetObjectValue(new object());
                this.LatestLog = new ConcurrentQueue<string>();
                this.LogProgress = 0;
                this.IsWindowAppeared = false;
                this.IsWindowFinished = false;
                this.Loader = CS_locals1._VB_Local_Loader;
                this.Instance = Instance;
                this.WindowTitle = CS_locals1._VB_Local_WindowTitle;
                this.PID = CS_locals1._VB_Local_Loader.Input.Id;
                this.WatcherLog("开始 Minecraft 日志监控", LogLevel.Info);
                if (Operators.CompareString(this.WindowTitle, "", false) != 0)
                {
                    this.WatcherLog("要求窗口标题：" + CS_locals1._VB_Local_WindowTitle, LogLevel.Info);
                }
                List<ModWatcher.Watcher> list = new List<ModWatcher.Watcher>();
                try
                {
                    foreach (ModWatcher.Watcher watcher in ModWatcher.McWatcherList)
                    {
                        if (watcher.State != ModWatcher.Watcher.MinecraftState.Crashed && watcher.State != ModWatcher.Watcher.MinecraftState.Ended && watcher.State != ModWatcher.Watcher.MinecraftState.Canceled)
                        {
                            list.Add(watcher);
                        }
                    }
                }
                finally
                {
                    List<ModWatcher.Watcher>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                list.Add(this);
                ModWatcher.McWatcherList = list;
                ModWatcher.WatcherStateChanged();
                this.GameProcess = CS_locals1._VB_Local_Loader.Input;
                this.GameProcess.BeginOutputReadLine();
                this.GameProcess.BeginErrorReadLine();
                this.GameProcess.OutputDataReceived += this.LogReceived;
                this.GameProcess.ErrorDataReceived += this.LogReceived;
                ModBase.RunInNewThread(new Action(CS_locals1._Lambda___0), "Minecraft Watcher PID " + Conversions.ToString(this.PID), ThreadPriority.Normal);
            }

            public ModWatcher.Watcher.MinecraftState State
            {
                get
                {
                    return this._State;
                }
                set
                {
                    if (this._State != value)
                    {
                        this._State = value;
                        ModWatcher.WatcherStateChanged();
                    }
                }
            }

            private void LogReceived(object sender, DataReceivedEventArgs e)
            {
                object waitingLogLock = this.WaitingLogLock;
                ObjectFlowControl.CheckForSyncLockOnValueType(waitingLogLock);
                lock (waitingLogLock)
                {
                    this.WaitingLog.Add(e.Data);
                }
            }

            private void TimerLog()
            {
                try
                {
                    List<string> list = new List<string>();
                    object waitingLogLock = this.WaitingLogLock;
                    ObjectFlowControl.CheckForSyncLockOnValueType(waitingLogLock);
                    lock (waitingLogLock)
                    {
                        if (!this.WaitingLog.Any<string>())
                        {
                            return;
                        }
                        list = this.WaitingLog;
                        this.WaitingLog = new List<string>(1000);
                    }
                    try
                    {
                        foreach (string text in list)
                        {
                            this.GameLog(text);
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    if (this.State == ModWatcher.Watcher.MinecraftState.Loading)
                    {
                        this.ProgressUpdate();
                    }
                    if (this.GameProcess.HasExited)
                    {
                        this.WatcherLog("Minecraft 已退出，返回值：" + Conversions.ToString(this.GameProcess.ExitCode), LogLevel.Info);
                        if (this.State == ModWatcher.Watcher.MinecraftState.Loading)
                        {
                            this.WatcherLog("Minecraft 尚未加载完成，可能已崩溃", LogLevel.Error);
                            this.Crashed();
                        }
                        else if (this.GameProcess.ExitCode != 0 && this.State == ModWatcher.Watcher.MinecraftState.Running && this.Instance.ReleaseTime.Year >= 2012)
                        {
                            this.WatcherLog("Minecraft 返回值异常，可能已崩溃", LogLevel.Error);
                            this.Crashed();
                        }
                        else if (this.State != ModWatcher.Watcher.MinecraftState.Crashed)
                        {
                            this.State = ModWatcher.Watcher.MinecraftState.Ended;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "输出 Minecraft 日志失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                }
            }

            private void GameLog(string Text)
            {
                if (Text != null)
                {
                    Text = Text.ReplaceLineEndings("\r\n", false);
                    this.LatestLog.Enqueue(Text);
                    if (this.LatestLog.Count >= 501)
                    {
                        ConcurrentQueue<string> latestLog = this.LatestLog;
                        string text = null;
                        latestLog.TryDequeue(out text);
                    }
                    Logger.Trace(Text, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                    if (this.LogProgress < 1)
                    {
                        this.WatcherLog("日志 1/5：已出现日志输出", LogLevel.Info);
                        this.LogProgress = 1;
                    }
                    if (this.LogProgress < 2 && Text.Contains("Setting user:"))
                    {
                        this.WatcherLog("日志 2/5：游戏用户已设置", LogLevel.Info);
                        this.LogProgress = 2;
                    }
                    else if (this.LogProgress < 3 && Text.ContainsIgnoreCase("lwjgl version"))
                    {
                        this.WatcherLog("日志 3/5：LWJGL 版本已确认", LogLevel.Info);
                        this.LogProgress = 3;
                    }
                    else if (this.LogProgress < 4 && (Text.Contains("OpenAL initialized") || Text.Contains("Starting up SoundSystem")))
                    {
                        this.WatcherLog("日志 4/5：OpenAL 已加载", LogLevel.Info);
                        this.LogProgress = 4;
                    }
                    else if (this.LogProgress < 5 && ((Text.Contains("Created") && Text.Contains("textures") && Text.Contains("-atlas")) || Text.Contains("Found animation info")))
                    {
                        this.WatcherLog("日志 5/5：材质已加载", LogLevel.Info);
                        this.LogProgress = 5;
                    }
                    if (!Text.Contains("[CHAT]"))
                    {
                        if (Text.Contains("Someone is closing me!") || Text.Contains("Restarting Minecraft with command"))
                        {
                            this.WatcherLog("识别为关闭的 Log：" + Text, LogLevel.Info);
                            this.State = ModWatcher.Watcher.MinecraftState.Ended;
                            return;
                        }
                        if (Text.Contains("Crash report saved to") || Text.Contains("This crash report has been saved to:"))
                        {
                            this.WatcherLog("识别为崩溃的 Log：" + Text, LogLevel.Error);
                            this.Crashed();
                            return;
                        }
                        if (Text.Contains("Could not save crash report to"))
                        {
                            this.WatcherLog("识别为崩溃的 Log：" + Text, LogLevel.Error);
                            this.Crashed();
                            return;
                        }
                        if (Text.Contains("/ERROR]: Unable to launch") || Text.Contains("An exception was thrown, the game will display an error screen and halt."))
                        {
                            this.WatcherLog("识别为崩溃的 Log：" + Text, LogLevel.Error);
                            this.Crashed();
                        }
                    }
                }
            }

            private void WatcherLog(string Text, LogLevel Level = LogLevel.Info)
            {
                ModLaunch.McLaunchLog("[" + Conversions.ToString(this.PID) + "] " + Text, Level);
            }

            private void ProgressUpdate()
            {
                double num;
                if (this.IsWindowAppeared || this.LogProgress == 5)
                {
                    num = 0.95;
                    this.WatcherLog("Minecraft 加载已完成", LogLevel.Info);
                    this.State = ModWatcher.Watcher.MinecraftState.Running;
                }
                else
                {
                    num = (double)Math.Min(this.LogProgress, 3) / 3.0 * 0.9;
                }
                this.Loader.Progress = num;
            }

            private void TimerWindow()
            {
                try
                {
                    ModWatcher.Watcher._Closure___23_0 CS_locals1 = new ModWatcher.Watcher._Closure___23_0();
                    CS_locals1._VB_Me = this;
                    if (!this.GameProcess.HasExited)
                    {
                        if (!this.IsWindowFinished)
                        {
                            KeyValuePair<IntPtr, string>? keyValuePair = null;
                            try
                            {
                                keyValuePair = this.TryGetMinecraftWindow();
                            }
                            catch (Win32Exception ex)
                            {
                                Logger.Error(ex, "由于反作弊或安全软件拦截，PCL 无法操作游戏窗口", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                                this.IsWindowFinished = true;
                            }
                            if (keyValuePair != null)
                            {
                                string value = keyValuePair.Value.Value;
                                CS_locals1._VB_Local_MinecraftWindowHandle = keyValuePair.Value.Key;
                                if (!value.StartsWithF("FML", false) && !value.StartsWithF("Quilt Loader", false))
                                {
                                    this.WindowHandle = CS_locals1._VB_Local_MinecraftWindowHandle;
                                    this.WatcherLog(string.Format("Minecraft 窗口已加载：{0}（{1}）", value, CS_locals1._VB_Local_MinecraftWindowHandle.ToInt64()), LogLevel.Info);
                                    this.IsWindowFinished = true;
                                    if (Settings.Get<int>("LaunchArgumentWindowType", null) == 4)
                                    {
                                        ModBase.RunInNewThread(new Action(CS_locals1._Lambda___0), "MinecraftWindowMaximize", ThreadPriority.Normal);
                                    }
                                }
                                else if (!this.IsWindowAppeared)
                                {
                                    this.WatcherLog(string.Concat(new string[]
                                    {
                                        "Mod Loader 窗口已加载：",
                                        value,
                                        "（",
                                        Conversions.ToString(CS_locals1._VB_Local_MinecraftWindowHandle.ToInt64()),
                                        "）"
                                    }), LogLevel.Info);
                                }
                                this.IsWindowAppeared = true;
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, "检查 Minecraft 窗口失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                }
            }

            private KeyValuePair<IntPtr, string>? TryGetMinecraftWindow()
            {
                ModWatcher.Watcher._Closure___24_0 CS_locals1 = new ModWatcher.Watcher._Closure___24_0();
                CS_locals1._VB_Me = this;
                CS_locals1._VB_Local_Result = null;
                ModWatcher.Watcher.EnumWindows(new ModWatcher.Watcher.EnumWindowsSub(CS_locals1._Lambda___0), 0);
                return (KeyValuePair<IntPtr, string>?)CS_locals1._VB_Local_Result;
            }

            [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            private static extern bool EnumWindows(ModWatcher.Watcher.EnumWindowsSub hWnd, int lParam);

            [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "GetClassNameA", ExactSpelling = true, SetLastError = true)]
            private static extern int GetClassName(int hWnd, StringBuilder str, int maxCount);

            [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "GetWindowTextA", ExactSpelling = true, SetLastError = true)]
            private static extern int GetWindowText(int hWnd, StringBuilder str, int maxCount);

            [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "SetWindowTextA", ExactSpelling = true, SetLastError = true)]
            private static extern bool SetWindowText(int hWnd, [MarshalAs(UnmanagedType.VBByRefStr)] ref string str);

            [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            private static extern bool ShowWindow(IntPtr hWnd, uint cmdWindow);

            [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            private static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            private static extern int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

            private void Crashed()
            {
                if (this.State != ModWatcher.Watcher.MinecraftState.Crashed && this.State != ModWatcher.Watcher.MinecraftState.Ended)
                {
                    this.State = ModWatcher.Watcher.MinecraftState.Crashed;
                    this.WatcherLog("Minecraft 已崩溃，将在 2 秒后开始崩溃分析", LogLevel.Error);
                    ModMain.Hint("检测到 Minecraft 出现错误，错误分析已开始……", ModMain.HintType.Red, true);
                    ModMain.FeedbackInfo();
                    ModBase.RunInNewThread(new Action(this._Lambda___33_0), "Crash Analyzer", ThreadPriority.Normal);
                }
            }

            public void Kill()
            {
                this.State = ModWatcher.Watcher.MinecraftState.Canceled;
                this.WatcherLog("尝试强制结束 Minecraft 进程", LogLevel.Info);
                try
                {
                    if (!this.GameProcess.HasExited)
                    {
                        this.GameProcess.Kill();
                    }
                    this.WatcherLog("已强制结束 Minecraft 进程", LogLevel.Info);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "强制结束 Minecraft 进程失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                }
            }

            [CompilerGenerated]
            private void _Lambda___33_0()
            {
                try
                {
                    Thread.Sleep(2000);
                    this.WatcherLog("崩溃分析开始", LogLevel.Info);
                    CrashAnalyzer crashAnalyzer = new CrashAnalyzer(this.Instance);
                    crashAnalyzer.Collect(this.Instance.PathIndie, this.LatestLog.ToList<string>());
                    crashAnalyzer.Prepare();
                    crashAnalyzer.Analyze();
                    crashAnalyzer.Output(false, new List<string>
                    {
                        this.Instance.PathVersion + this.Instance.Name + ".json",
                        Paths.Base + "PCL\\Log1.txt",
                        Paths.Base + "PCL\\LatestLaunch.bat"
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "崩溃分析失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                }
            }

            public Process GameProcess;

            public ModMinecraft.McInstance Instance;

            private string WindowTitle;

            private int PID;

            public ModLoader.LoaderTask<Process, int> Loader;

            private ModWatcher.Watcher.MinecraftState _State;

            public List<string> WaitingLog;

            private readonly object WaitingLogLock;

            public ConcurrentQueue<string> LatestLog;

            private int LogProgress;

            private bool IsWindowAppeared;

            private bool IsWindowFinished;

            private IntPtr WindowHandle;

            public enum MinecraftState
            {
                Loading,
                Running,
                Crashed,
                Ended,
                Canceled
            }

            private delegate void EnumWindowsSub(IntPtr hwnd, int lParam);

            [CompilerGenerated]
            private sealed class _Closure___23_0
            {
                public _Closure___23_0()
                {
                }

                internal void _Lambda___0()
                {
                    try
                    {
                        Thread.Sleep(2000);
                        ModWatcher.Watcher.ShowWindow(this._VB_Me.WindowHandle, 3U);
                        this._VB_Me.WatcherLog(string.Format("已最大化 Minecraft 窗口：{0}", this._VB_Local_MinecraftWindowHandle.ToInt64()), LogLevel.Info);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "最大化 Minecraft 窗口时出现错误", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                    }
                }

                public IntPtr _VB_Local_MinecraftWindowHandle;

                public ModWatcher.Watcher _VB_Me;
            }

            [CompilerGenerated]
            private sealed class _Closure___24_0
            {
                public _Closure___24_0()
                {
                }

                internal void _Lambda___0(IntPtr hwnd, int lParam)
                {
                    if (this._VB_Local_Result == null)
                    {
                        StringBuilder stringBuilder = new StringBuilder(512);
                        ModWatcher.Watcher.GetClassName((int)hwnd, stringBuilder, stringBuilder.Capacity);
                        string text = stringBuilder.ToString();
                        if ((Operators.CompareString(text, "GLFW30", false) == 0 || Operators.CompareString(text, "SDL_app", false) == 0 || Operators.CompareString(text, "LWJGL", false) == 0 || Operators.CompareString(text, "SunAwtFrame", false) == 0) && (Operators.CompareString(text, "SDL_app", false) != 0 || ModWatcher.Watcher.IsWindowVisible(hwnd)))
                        {
                            stringBuilder = new StringBuilder(512);
                            ModWatcher.Watcher.GetWindowText((int)hwnd, stringBuilder, stringBuilder.Capacity);
                            string text2 = stringBuilder.ToString();
                            if (!string.IsNullOrWhiteSpace(text2) && Operators.CompareString(text2, "SDL_app", false) != 0 && Operators.CompareString(text2, "PopupMessageWindow", false) != 0 && !text2.StartsWithF("GLFW", false) && !text2.EndsWithF("Hidden Utility Window", false))
                            {
                                int num = default;
                                ModWatcher.Watcher.GetWindowThreadProcessId(hwnd, ref num);
                                try
                                {
                                    if (DateTime.Compare(Process.GetProcessById(num).StartTime, this._VB_Me.GameProcess.StartTime) < 0)
                                    {
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn(ex, "枚举 Minecraft 窗口进程失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                                    return;
                                }
                                this._VB_Local_Result = new KeyValuePair<IntPtr, string>(hwnd, text2);
                            }
                        }
                    }
                }

                public object _VB_Local_Result;

                public ModWatcher.Watcher _VB_Me;
            }

            [CompilerGenerated]
            private sealed class _Closure___5_0
            {
                public _Closure___5_0(ModWatcher.Watcher._Closure___5_0 arg0)
                {
                    if (arg0 != null)
                    {
                        this._VB_Local_WindowTitle = arg0._VB_Local_WindowTitle;
                        this._VB_Local_Loader = arg0._VB_Local_Loader;
                    }
                }

                internal void _Lambda___0()
                {
                    checked
                    {
                        try
                        {
                            while (this._VB_Me.State != ModWatcher.Watcher.MinecraftState.Ended && this._VB_Me.State != ModWatcher.Watcher.MinecraftState.Canceled && (this._VB_Local_Loader.State != ModBase.LoadState.Canceled || this._VB_Me.State == ModWatcher.Watcher.MinecraftState.Crashed) && (this._VB_Me.State != ModWatcher.Watcher.MinecraftState.Crashed || !this._VB_Me.GameProcess.HasExited))
                            {
                                this._VB_Me.TimerWindow();
                                this._VB_Me.TimerLog();
                                int num = 1;
                                do
                                {
                                    if (this._VB_Me.IsWindowFinished && this._VB_Me.IsWindowAppeared && Operators.CompareString(this._VB_Local_WindowTitle, "", false) != 0 && this._VB_Me.State == ModWatcher.Watcher.MinecraftState.Running && !this._VB_Me.GameProcess.HasExited)
                                    {
                                        string text = this._VB_Local_WindowTitle.Replace("{date}", DateTime.Now.ToString("yyyy'/'M'/'d")).Replace("{time}", DateTime.Now.ToString("HH':'mm':'ss"));
                                        int num2 = (int)this._VB_Me.WindowHandle;
                                        string text2 = new string(text.ToCharArray());
                                        ModWatcher.Watcher.SetWindowText(num2, ref text2);
                                    }
                                    Thread.Sleep(64);
                                    num++;
                                }
                                while (num <= 3);
                            }
                            if (this._VB_Me.State == ModWatcher.Watcher.MinecraftState.Crashed)
                            {
                                ModWatcher.WatcherStateChanged();
                            }
                            this._VB_Me.WatcherLog("Minecraft 日志监控已退出", LogLevel.Info);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Minecraft 日志监控主循环出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                            this._VB_Me.State = ModWatcher.Watcher.MinecraftState.Ended;
                        }
                    }
                }

                public string _VB_Local_WindowTitle;

                public ModLoader.LoaderTask<Process, int> _VB_Local_Loader;

                public ModWatcher.Watcher _VB_Me;
            }
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

            internal void _Lambda___6_0()
            {
                if (ModMusic.MusicResume())
                {
                    Logger.Info("已根据设置，在结束后开始音乐播放", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                }
            }

            internal void _Lambda___6_1()
            {
                if (ModMusic.MusicPause())
                {
                    Logger.Info("已根据设置，在结束后暂停音乐播放", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Minecraft\\ModWatcher.vb");
                }
            }

            internal void _Lambda___6_2()
            {
                ModMain.FrmMain.EndProgram(false);
            }

            internal void _Lambda___6_3()
            {
                ModMain.FrmMain.Hidden = false;
            }

            internal void _Lambda___6_4()
            {
                ModMain.FrmMain.Hidden = false;
            }

            public static readonly ModWatcher._Closure___ _I = new ModWatcher._Closure___();

            public static Action _I6_0;

            public static Action _I6_1;

            public static Action _I6_2;

            public static Action _I6_3;

            public static Action _I6_4;
        }
    }
}
