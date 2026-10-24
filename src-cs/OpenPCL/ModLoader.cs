using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Shell;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [StandardModule]
    public static class ModLoader
    {
        // Note: this type is marked as 'beforefieldinit'.
        static ModLoader()
        {
        }

        public static void LoaderTaskbarAdd<T>(ModLoader.LoaderCombo<T> Loader)
        {
            if (ModMain.FrmSpeedLeft != null)
            {
                ModMain.FrmSpeedLeft.TaskRemove(Loader);
            }
            ModLoader.LoaderTaskbar.Add(Loader);
            Logger.Info(string.Format("{0} 已加入任务列表", Loader.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
        }

        public static void LoaderTaskbarProgressRefresh()
        {
            try
            {
                double num = ModLoader.LoaderTaskbarProgressGet();
                try
                {
                    foreach (ModLoader.LoaderBase loaderBase in ModLoader.LoaderTaskbar)
                    {
                        if (ModLoader.LoaderTaskbar.All<ModLoader.LoaderBase>((ModLoader._Closure___._I9_0 == null) ? (ModLoader._Closure___._I9_0 = new Func<ModLoader.LoaderBase, bool>(ModLoader._Closure___._I._Lambda___9_0)) : ModLoader._Closure___._I9_0) || loaderBase.State == ModBase.LoadState.Waiting || loaderBase.State == ModBase.LoadState.Canceled)
                        {
                            PageSpeedLeft frmSpeedLeft = ModMain.FrmSpeedLeft;
                            if (frmSpeedLeft != null)
                            {
                                frmSpeedLeft.TaskRefresh(loaderBase);
                            }
                            ModLoader.LoaderTaskbar.Remove(loaderBase);
                            Logger.Info(string.Format("{0} 已移出任务列表", loaderBase.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                        }
                    }
                }
                finally
                {
                    IEnumerator<ModLoader.LoaderBase> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                if (num <= 0.0 || num >= 1.0 || ModLoader.LoaderTaskbarProgress > num)
                {
                    ModLoader.LoaderTaskbarProgress = num;
                }
                else
                {
                    ModLoader.LoaderTaskbarProgress = ModLoader.LoaderTaskbarProgress * 0.9 + num * 0.1;
                }
                ModBase.RunInUi((ModLoader._Closure___._I9_1 == null) ? (ModLoader._Closure___._I9_1 = new Action(ModLoader._Closure___._I._Lambda___9_1)) : ModLoader._Closure___._I9_1, false);
                TaskbarItemProgressState taskbarItemProgressState;
                if (!ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>() || ModLoader.LoaderTaskbarProgress == 1.0)
                {
                    taskbarItemProgressState = TaskbarItemProgressState.None;
                }
                else if (ModLoader.LoaderTaskbarProgress < 0.015)
                {
                    taskbarItemProgressState = TaskbarItemProgressState.Indeterminate;
                }
                else
                {
                    taskbarItemProgressState = TaskbarItemProgressState.Normal;
                    ModMain.FrmMain.TaskbarItemInfo.ProgressValue = ModLoader.LoaderTaskbarProgress;
                }
                if (ModLoader.LoaderTaskbarProgressLast != taskbarItemProgressState)
                {
                    ModLoader.LoaderTaskbarProgressLast = taskbarItemProgressState;
                    ModMain.FrmMain.TaskbarItemInfo.ProgressState = taskbarItemProgressState;
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "刷新任务栏进度显示失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
            }
        }

        public static double LoaderTaskbarProgressGet()
        {
            double num;
            try
            {
                if (!ModLoader.LoaderTaskbar.Any<ModLoader.LoaderBase>())
                {
                    num = 1.0;
                }
                else
                {
                    num = ModLoader.LoaderTaskbar.Select<ModLoader.LoaderBase, double>((ModLoader._Closure___._I10_0 == null) ? (ModLoader._Closure___._I10_0 = new Func<ModLoader.LoaderBase, double>(ModLoader._Closure___._I._Lambda___10_0)) : ModLoader._Closure___._I10_0).Average().Clamp(0.0, 1.0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "获取任务栏进度出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                num = 0.5;
            }
            return num;
        }

        public static bool LoaderFolderRun(ModLoader.LoaderBase Loader, string FolderPath, ModLoader.LoaderFolderRunType Type, int MaxDepth = 0, string ExtraPath = "", bool WaitForExit = false)
        {
            ModLoader.LoaderFolderDictionaryEntry loaderFolderDictionaryEntry = default(ModLoader.LoaderFolderDictionaryEntry);
            loaderFolderDictionaryEntry.FolderPath = FolderPath + ExtraPath;
            loaderFolderDictionaryEntry.LastCheckTime = null;
            try
            {
                DirectoryInfo info = DirectoryUtils.GetInfo(FolderPath + ExtraPath);
                loaderFolderDictionaryEntry.LastCheckTime = (info.Exists ? new DateTime?(ModLoader.GetActualLastWriteTimeUtc(info, MaxDepth)) : ((DateTime?)null));
                if (Type == ModLoader.LoaderFolderRunType.RunOnUpdated && ModLoader.LoaderFolderDictionary.ContainsKey(Loader))
                {
                    if (info.Exists)
                    {
                        ModLoader.LoaderFolderDictionaryEntry loaderFolderDictionaryEntry2 = ModLoader.LoaderFolderDictionary[Loader];
                        if (loaderFolderDictionaryEntry2.LastCheckTime != null && loaderFolderDictionaryEntry.Equals(ModLoader.LoaderFolderDictionary[Loader]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        ModLoader.LoaderFolderDictionaryEntry loaderFolderDictionaryEntry2 = ModLoader.LoaderFolderDictionary[Loader];
                        if (loaderFolderDictionaryEntry2.LastCheckTime == null)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "文件夹加载器启动检测出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
            }
            ModLoader.LoaderFolderDictionary[Loader] = loaderFolderDictionaryEntry;
            bool flag;
            if (Type == ModLoader.LoaderFolderRunType.UpdateOnly)
            {
                flag = false;
            }
            else
            {
                if (WaitForExit)
                {
                    Loader.WaitForExit(FolderPath, null, true, default(CancellationToken), null);
                }
                else
                {
                    Loader.Start(FolderPath, true);
                }
                flag = true;
            }
            return flag;
        }

        private static DateTime GetActualLastWriteTimeUtc(DirectoryInfo FolderInfo, int MaxDepth)
        {
            DateTime dateTime = FolderInfo.LastWriteTimeUtc;
            if (MaxDepth > 0)
            {
                try
                {
                    foreach (DirectoryInfo directoryInfo in FolderInfo.EnumerateDirectories())
                    {
                        DateTime actualLastWriteTimeUtc = ModLoader.GetActualLastWriteTimeUtc(directoryInfo, checked(MaxDepth - 1));
                        if (DateTime.Compare(actualLastWriteTimeUtc, dateTime) > 0)
                        {
                            dateTime = actualLastWriteTimeUtc;
                        }
                    }
                }
                finally
                {
                    IEnumerator<DirectoryInfo> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }
            return dateTime;
        }

        public static ConcurrentList<ModLoader.LoaderBase> LoaderTaskbar = new ConcurrentList<ModLoader.LoaderBase>();

        public static double LoaderTaskbarProgress = 0.0;

        private static TaskbarItemProgressState LoaderTaskbarProgressLast = TaskbarItemProgressState.None;

        private static Dictionary<ModLoader.LoaderBase, ModLoader.LoaderFolderDictionaryEntry> LoaderFolderDictionary = new Dictionary<ModLoader.LoaderBase, ModLoader.LoaderFolderDictionaryEntry>();

        public abstract class LoaderBase : ILoadingTrigger
        {
            protected LoaderBase()
            {
                this._IsLoader = true;
                this.Uuid = ModBase.GetUuid();
                this.Name = "未命名任务 " + Conversions.ToString(this.Uuid) + "#";
                this.LockState = RuntimeHelpers.GetObjectValue(new object());
                this.Parent = null;
                this.HasOnStateChangedThread = false;
                this._State = ModBase.LoadState.Waiting;
                this._LoadingState = OurLoading.OurLoadingState.Stop;
                this.Error = null;
                this.Block = true;
                this.Show = true;
                this.IsForceRestarting = false;
                this._Progress = -1.0;
                this.ProgressWeight = 1.0;
            }

            public bool IsLoader
            {
                [CompilerGenerated]
                get
                {
                    return this._IsLoader;
                }
            }

            public ModLoader.LoaderBase RealParent
            {
                get
                {
                    ModLoader.LoaderBase loaderBase;
                    try
                    {
                        loaderBase = this.Parent;
                        while (loaderBase != null && loaderBase.Parent != null)
                        {
                            loaderBase = loaderBase.Parent;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("获取父加载器失败（{0}）", this.Name), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                        loaderBase = null;
                    }
                    return loaderBase;
                }
            }

            public virtual void InitParent(ModLoader.LoaderBase Parent)
            {
                this.Parent = Parent;
            }

            public event ModLoader.LoaderBase.OnStateChangedThreadEventHandler OnStateChangedThread
            {
                [CompilerGenerated]
                add
                {
                    ModLoader.LoaderBase.OnStateChangedThreadEventHandler onStateChangedThreadEventHandler = this.OnStateChangedThreadEvent;
                    ModLoader.LoaderBase.OnStateChangedThreadEventHandler onStateChangedThreadEventHandler2;
                    do
                    {
                        onStateChangedThreadEventHandler2 = onStateChangedThreadEventHandler;
                        ModLoader.LoaderBase.OnStateChangedThreadEventHandler onStateChangedThreadEventHandler3 = (ModLoader.LoaderBase.OnStateChangedThreadEventHandler)Delegate.Combine(onStateChangedThreadEventHandler2, value);
                        onStateChangedThreadEventHandler = Interlocked.CompareExchange<ModLoader.LoaderBase.OnStateChangedThreadEventHandler>(ref this.OnStateChangedThreadEvent, onStateChangedThreadEventHandler3, onStateChangedThreadEventHandler2);
                    }
                    while (onStateChangedThreadEventHandler != onStateChangedThreadEventHandler2);
                }
                [CompilerGenerated]
                remove
                {
                    ModLoader.LoaderBase.OnStateChangedThreadEventHandler onStateChangedThreadEventHandler = this.OnStateChangedThreadEvent;
                    ModLoader.LoaderBase.OnStateChangedThreadEventHandler onStateChangedThreadEventHandler2;
                    do
                    {
                        onStateChangedThreadEventHandler2 = onStateChangedThreadEventHandler;
                        ModLoader.LoaderBase.OnStateChangedThreadEventHandler onStateChangedThreadEventHandler3 = (ModLoader.LoaderBase.OnStateChangedThreadEventHandler)Delegate.Remove(onStateChangedThreadEventHandler2, value);
                        onStateChangedThreadEventHandler = Interlocked.CompareExchange<ModLoader.LoaderBase.OnStateChangedThreadEventHandler>(ref this.OnStateChangedThreadEvent, onStateChangedThreadEventHandler3, onStateChangedThreadEventHandler2);
                    }
                    while (onStateChangedThreadEventHandler != onStateChangedThreadEventHandler2);
                }
            }

            public event ModLoader.LoaderBase.OnStateChangedUiEventHandler OnStateChangedUi
            {
                [CompilerGenerated]
                add
                {
                    ModLoader.LoaderBase.OnStateChangedUiEventHandler onStateChangedUiEventHandler = this.OnStateChangedUiEvent;
                    ModLoader.LoaderBase.OnStateChangedUiEventHandler onStateChangedUiEventHandler2;
                    do
                    {
                        onStateChangedUiEventHandler2 = onStateChangedUiEventHandler;
                        ModLoader.LoaderBase.OnStateChangedUiEventHandler onStateChangedUiEventHandler3 = (ModLoader.LoaderBase.OnStateChangedUiEventHandler)Delegate.Combine(onStateChangedUiEventHandler2, value);
                        onStateChangedUiEventHandler = Interlocked.CompareExchange<ModLoader.LoaderBase.OnStateChangedUiEventHandler>(ref this.OnStateChangedUiEvent, onStateChangedUiEventHandler3, onStateChangedUiEventHandler2);
                    }
                    while (onStateChangedUiEventHandler != onStateChangedUiEventHandler2);
                }
                [CompilerGenerated]
                remove
                {
                    ModLoader.LoaderBase.OnStateChangedUiEventHandler onStateChangedUiEventHandler = this.OnStateChangedUiEvent;
                    ModLoader.LoaderBase.OnStateChangedUiEventHandler onStateChangedUiEventHandler2;
                    do
                    {
                        onStateChangedUiEventHandler2 = onStateChangedUiEventHandler;
                        ModLoader.LoaderBase.OnStateChangedUiEventHandler onStateChangedUiEventHandler3 = (ModLoader.LoaderBase.OnStateChangedUiEventHandler)Delegate.Remove(onStateChangedUiEventHandler2, value);
                        onStateChangedUiEventHandler = Interlocked.CompareExchange<ModLoader.LoaderBase.OnStateChangedUiEventHandler>(ref this.OnStateChangedUiEvent, onStateChangedUiEventHandler3, onStateChangedUiEventHandler2);
                    }
                    while (onStateChangedUiEventHandler != onStateChangedUiEventHandler2);
                }
            }

            public Action<ModLoader.LoaderBase> OnStateChanged
            {
                set
                {
                    ModLoader.LoaderBase._Closure___23_0 CS_locals1 = new ModLoader.LoaderBase._Closure___23_0();
                    CS_locals1._VB_Local_value = value;
                    this.OnStateChangedUi += CS_locals1._Lambda___0;
                }
            }

            public event ModLoader.LoaderBase.PreviewFinishEventHandler PreviewFinish
            {
                [CompilerGenerated]
                add
                {
                    ModLoader.LoaderBase.PreviewFinishEventHandler previewFinishEventHandler = this.PreviewFinishEvent;
                    ModLoader.LoaderBase.PreviewFinishEventHandler previewFinishEventHandler2;
                    do
                    {
                        previewFinishEventHandler2 = previewFinishEventHandler;
                        ModLoader.LoaderBase.PreviewFinishEventHandler previewFinishEventHandler3 = (ModLoader.LoaderBase.PreviewFinishEventHandler)Delegate.Combine(previewFinishEventHandler2, value);
                        previewFinishEventHandler = Interlocked.CompareExchange<ModLoader.LoaderBase.PreviewFinishEventHandler>(ref this.PreviewFinishEvent, previewFinishEventHandler3, previewFinishEventHandler2);
                    }
                    while (previewFinishEventHandler != previewFinishEventHandler2);
                }
                [CompilerGenerated]
                remove
                {
                    ModLoader.LoaderBase.PreviewFinishEventHandler previewFinishEventHandler = this.PreviewFinishEvent;
                    ModLoader.LoaderBase.PreviewFinishEventHandler previewFinishEventHandler2;
                    do
                    {
                        previewFinishEventHandler2 = previewFinishEventHandler;
                        ModLoader.LoaderBase.PreviewFinishEventHandler previewFinishEventHandler3 = (ModLoader.LoaderBase.PreviewFinishEventHandler)Delegate.Remove(previewFinishEventHandler2, value);
                        previewFinishEventHandler = Interlocked.CompareExchange<ModLoader.LoaderBase.PreviewFinishEventHandler>(ref this.PreviewFinishEvent, previewFinishEventHandler3, previewFinishEventHandler2);
                    }
                    while (previewFinishEventHandler != previewFinishEventHandler2);
                }
            }

            protected void RaisePreviewFinish()
            {
                ModLoader.LoaderBase.PreviewFinishEventHandler previewFinishEvent = this.PreviewFinishEvent;
                if (previewFinishEvent != null)
                {
                    previewFinishEvent(this);
                }
            }

            public ModBase.LoadState State
            {
                get
                {
                    return this._State;
                }
                set
                {
                    ModLoader.LoaderBase._Closure___32_0 CS_locals1 = new ModLoader.LoaderBase._Closure___32_0();
                    CS_locals1._VB_Me = this;
                    CS_locals1._VB_Local_value = value;
                    if (this._State != CS_locals1._VB_Local_value)
                    {
                        CS_locals1._VB_Local_OldState = this._State;
                        if (CS_locals1._VB_Local_value == ModBase.LoadState.Finished && Settings.Get<bool>("SystemDebugDelay", null))
                        {
                            Thread.Sleep(ModBase.RandomInteger(100, 2000));
                        }
                        this._State = CS_locals1._VB_Local_value;
                        if (CS_locals1._VB_Local_value == ModBase.LoadState.Canceled)
                        {
                            CancellationTokenSource cancellationTokenSource = this.CancellationTokenSource;
                            if (cancellationTokenSource != null)
                            {
                                cancellationTokenSource.Cancel();
                            }
                        }
                        Logger.Log(string.Format("加载器 {0} {1} 状态改变：{2}", base.GetType().Name, this.Name, CS_locals1._VB_Local_value), (CS_locals1._VB_Local_value == ModBase.LoadState.Failed) ? LogLevel.Error : LogLevel.Info, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                        ModBase.RunInUi(new Action(CS_locals1._Lambda___0), false);
                        if (this.HasOnStateChangedThread)
                        {
                            ModBase.RunInThread(new Action(CS_locals1._Lambda___1));
                        }
                    }
                }
            }

            public OurLoading.OurLoadingState LoadingState
            {
                get
                {
                    return this._LoadingState;
                }
                set
                {
                    if (this._LoadingState != value)
                    {
                        OurLoading.OurLoadingState loadingState = this._LoadingState;
                        this._LoadingState = value;
                        ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEvent = this.LoadingStateChangedEvent;
                        if (loadingStateChangedEvent != null)
                        {
                            loadingStateChangedEvent(value, loadingState);
                        }
                    }
                }
            }

            public event ILoadingTrigger.LoadingStateChangedEventHandler LoadingStateChanged
            {
                [CompilerGenerated]
                add
                {
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler = this.LoadingStateChangedEvent;
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler2;
                    do
                    {
                        loadingStateChangedEventHandler2 = loadingStateChangedEventHandler;
                        ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler3 = (ILoadingTrigger.LoadingStateChangedEventHandler)Delegate.Combine(loadingStateChangedEventHandler2, value);
                        loadingStateChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.LoadingStateChangedEventHandler>(ref this.LoadingStateChangedEvent, loadingStateChangedEventHandler3, loadingStateChangedEventHandler2);
                    }
                    while (loadingStateChangedEventHandler != loadingStateChangedEventHandler2);
                }
                [CompilerGenerated]
                remove
                {
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler = this.LoadingStateChangedEvent;
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler2;
                    do
                    {
                        loadingStateChangedEventHandler2 = loadingStateChangedEventHandler;
                        ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler3 = (ILoadingTrigger.LoadingStateChangedEventHandler)Delegate.Remove(loadingStateChangedEventHandler2, value);
                        loadingStateChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.LoadingStateChangedEventHandler>(ref this.LoadingStateChangedEvent, loadingStateChangedEventHandler3, loadingStateChangedEventHandler2);
                    }
                    while (loadingStateChangedEventHandler != loadingStateChangedEventHandler2);
                }
            }

            public Exception Error
            {
                [CompilerGenerated]
                get
                {
                    return this._Error;
                }
                [CompilerGenerated]
                set
                {
                    this._Error = value;
                }
            }

            public virtual double Progress
            {
                get
                {
                    ModBase.LoadState state = this.State;
                    double num;
                    if (state != ModBase.LoadState.Waiting)
                    {
                        if (state != ModBase.LoadState.Loading)
                        {
                            num = 1.0;
                        }
                        else
                        {
                            num = ((this._Progress == -1.0) ? 0.0 : this._Progress);
                        }
                    }
                    else
                    {
                        num = 0.0;
                    }
                    return num;
                }
                set
                {
                    if (this._Progress != value)
                    {
                        double progress = this._Progress;
                        this._Progress = value;
                        ILoadingTrigger.ProgressChangedEventHandler progressChangedEvent = this.ProgressChangedEvent;
                        if (progressChangedEvent != null)
                        {
                            progressChangedEvent(value, progress);
                        }
                    }
                }
            }

            public event ILoadingTrigger.ProgressChangedEventHandler ProgressChanged
            {
                [CompilerGenerated]
                add
                {
                    ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler = this.ProgressChangedEvent;
                    ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler2;
                    do
                    {
                        progressChangedEventHandler2 = progressChangedEventHandler;
                        ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler3 = (ILoadingTrigger.ProgressChangedEventHandler)Delegate.Combine(progressChangedEventHandler2, value);
                        progressChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.ProgressChangedEventHandler>(ref this.ProgressChangedEvent, progressChangedEventHandler3, progressChangedEventHandler2);
                    }
                    while (progressChangedEventHandler != progressChangedEventHandler2);
                }
                [CompilerGenerated]
                remove
                {
                    ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler = this.ProgressChangedEvent;
                    ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler2;
                    do
                    {
                        progressChangedEventHandler2 = progressChangedEventHandler;
                        ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler3 = (ILoadingTrigger.ProgressChangedEventHandler)Delegate.Remove(progressChangedEventHandler2, value);
                        progressChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.ProgressChangedEventHandler>(ref this.ProgressChangedEvent, progressChangedEventHandler3, progressChangedEventHandler2);
                    }
                    while (progressChangedEventHandler != progressChangedEventHandler2);
                }
            }

            public ProgressProvider CreateSyncProgressProvider(double FromPercentage = 0.0, double ToPercentage = 1.0)
            {
                ModLoader.LoaderBase._Closure___57_0 CS_locals1 = new ModLoader.LoaderBase._Closure___57_0();
                CS_locals1._VB_Me = this;
                CS_locals1._VB_Local_FromPercentage = FromPercentage;
                CS_locals1._VB_Local_ToPercentage = ToPercentage;
                CS_locals1._VB_Local_Provider = new ProgressProvider();
                CS_locals1._VB_Local_Provider.ProgressChanged += CS_locals1._Lambda___0;
                return CS_locals1._VB_Local_Provider;
            }

            public double ProgressWeight
            {
                [CompilerGenerated]
                get
                {
                    return this._ProgressWeight;
                }
                [CompilerGenerated]
                set
                {
                    this._ProgressWeight = value;
                }
            }

            public abstract void Start(object Input = null, bool IsForceRestart = false);

            public abstract void Cancel();

            public abstract void Failed(Exception Ex);

            public CancellationToken CreateCancellationToken()
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                CancellationToken token;
                lock (lockState)
                {
                    if (this.CancellationTokenSource == null || this.CancellationTokenSource.IsCancellationRequested)
                    {
                        this.CancellationTokenSource = new CancellationTokenSource();
                        this.CancellationTokenSource.Token.Register(new Action(this.Cancel));
                    }
                    if (this.State == ModBase.LoadState.Canceled)
                    {
                        this.CancellationTokenSource.Cancel();
                    }
                    token = this.CancellationTokenSource.Token;
                }
                return token;
            }

            public void WaitForExit(object Input = null, ModLoader.LoaderBase LoaderToSyncProgress = null, bool IsForceRestart = false, CancellationToken c = default(CancellationToken), ProgressProvider p = null)
            {
                this.Start(RuntimeHelpers.GetObjectValue(Input), IsForceRestart);
                while (this.State == ModBase.LoadState.Loading)
                {
                    if (LoaderToSyncProgress != null)
                    {
                        LoaderToSyncProgress.Progress = this.Progress;
                    }
                    if (p != null)
                    {
                        p.Set(this.Progress, false, ProgressProvider.ChildrenAction.Finished);
                    }
                    c.ThrowIfCancellationRequested();
                    Thread.Sleep(10);
                }
                if (this.State == ModBase.LoadState.Finished)
                {
                    return;
                }
                if (this.State == ModBase.LoadState.Canceled)
                {
                    throw new OperationCanceledException("加载器执行已中断");
                }
                if (Information.IsNothing(this.Error))
                {
                    throw new Exception("未知错误！");
                }
                throw new Exception(this.Error.Message, this.Error);
            }

            public void WaitForExitTime(int Timeout, object Input = null, string TimeoutMessage = "等待加载器执行超时。", object LoaderToSyncProgress = null, bool IsForceRestart = false)
            {
                this.Start(RuntimeHelpers.GetObjectValue(Input), IsForceRestart);
                checked
                {
                    while (this.State == ModBase.LoadState.Loading)
                    {
                        if (LoaderToSyncProgress != null)
                        {
                            NewLateBinding.LateSet(LoaderToSyncProgress, null, "Progress", new object[] { this.Progress }, null, null);
                        }
                        Thread.Sleep(10);
                        Timeout -= 10;
                        if (Timeout < 0)
                        {
                            throw new TimeoutException(TimeoutMessage);
                        }
                    }
                    if (this.State == ModBase.LoadState.Finished)
                    {
                        return;
                    }
                    if (this.State == ModBase.LoadState.Canceled)
                    {
                        throw new OperationCanceledException("加载器执行已中断");
                    }
                    if (Information.IsNothing(this.Error))
                    {
                        throw new Exception("未知错误！");
                    }
                    throw this.Error;
                }
            }

            public override bool Equals(object obj)
            {
                ModLoader.LoaderBase loaderBase = obj as ModLoader.LoaderBase;
                return loaderBase != null && this.Uuid == loaderBase.Uuid;
            }

            [CompilerGenerated]
            private bool _IsLoader;

            public int Uuid;

            public string Name;

            public readonly object LockState;

            public ModLoader.LoaderBase Parent;

            [CompilerGenerated]
            private ModLoader.LoaderBase.OnStateChangedThreadEventHandler OnStateChangedThreadEvent;

            public bool HasOnStateChangedThread;

            [CompilerGenerated]
            private ModLoader.LoaderBase.OnStateChangedUiEventHandler OnStateChangedUiEvent;

            [CompilerGenerated]
            private ModLoader.LoaderBase.PreviewFinishEventHandler PreviewFinishEvent;

            private ModBase.LoadState _State;

            private OurLoading.OurLoadingState _LoadingState;

            [CompilerGenerated]
            private ILoadingTrigger.LoadingStateChangedEventHandler LoadingStateChangedEvent;

            [CompilerGenerated]
            private Exception _Error;

            public bool Block;

            public bool Show;

            public bool IsForceRestarting;

            private double _Progress;

            [CompilerGenerated]
            private ILoadingTrigger.ProgressChangedEventHandler ProgressChangedEvent;

            [CompilerGenerated]
            private double _ProgressWeight;

            private CancellationTokenSource CancellationTokenSource;

            public const string WaitForExitTimeoutMessage = "等待加载器执行超时。";

            public delegate void OnStateChangedThreadEventHandler(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState);

            public delegate void OnStateChangedUiEventHandler(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState);

            public delegate void PreviewFinishEventHandler(ModLoader.LoaderBase Loader);

            [CompilerGenerated]
            private sealed class _Closure___23_0
            {
                public _Closure___23_0()
                {
                }

                internal void _Lambda___0(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState)
                {
                    this._VB_Local_value(Loader);
                }

                public Action<ModLoader.LoaderBase> _VB_Local_value;
            }

            [CompilerGenerated]
            private sealed class _Closure___32_0
            {
                public _Closure___32_0()
                {
                }

                internal void _Lambda___0()
                {
                    ModBase.LoadState loadState = this._VB_Local_value;
                    if (loadState != ModBase.LoadState.Loading)
                    {
                        if (loadState != ModBase.LoadState.Failed)
                        {
                            this._VB_Me.LoadingState = OurLoading.OurLoadingState.Stop;
                        }
                        else
                        {
                            this._VB_Me.LoadingState = OurLoading.OurLoadingState.Error;
                        }
                    }
                    else
                    {
                        this._VB_Me.LoadingState = OurLoading.OurLoadingState.Run;
                    }
                    ModLoader.LoaderBase.OnStateChangedUiEventHandler onStateChangedUiEvent = this._VB_Me.OnStateChangedUiEvent;
                    if (onStateChangedUiEvent != null)
                    {
                        onStateChangedUiEvent(this._VB_Me, this._VB_Local_value, this._VB_Local_OldState);
                    }
                }

                internal void _Lambda___1()
                {
                    ModLoader.LoaderBase.OnStateChangedThreadEventHandler onStateChangedThreadEvent = this._VB_Me.OnStateChangedThreadEvent;
                    if (onStateChangedThreadEvent != null)
                    {
                        onStateChangedThreadEvent(this._VB_Me, this._VB_Local_value, this._VB_Local_OldState);
                    }
                }

                public ModBase.LoadState _VB_Local_value;

                public ModBase.LoadState _VB_Local_OldState;

                public ModLoader.LoaderBase _VB_Me;
            }

            [CompilerGenerated]
            private sealed class _Closure___57_0
            {
                public _Closure___57_0()
                {
                }

                internal void _Lambda___0(ValueTuple<double, double> ChangedProgress)
                {
                    ValueTuple<double, double> raw = this._VB_Local_Provider.GetRaw();
                    this._VB_Me.Progress = this._VB_Local_FromPercentage + (raw.Item1 + raw.Item2) * (this._VB_Local_ToPercentage - this._VB_Local_FromPercentage);
                }

                public ProgressProvider _VB_Local_Provider;

                public double _VB_Local_FromPercentage;

                public double _VB_Local_ToPercentage;

                public ModLoader.LoaderBase _VB_Me;
            }
        }

        public class LoaderTask<TIn, TOut> : ModLoader.LoaderBase
        {
            public bool IsCanceled
            {
                get
                {
                    return this.IsCanceledWithThread(Thread.CurrentThread);
                }
            }

            public bool IsCanceledWithThread(Thread Thread)
            {
                return this.LastRunningThread == null || !object.ReferenceEquals(Thread, this.LastRunningThread) || base.State == ModBase.LoadState.Canceled;
            }

            public TIn StartGetInput(TIn Input = default(TIn), Func<TIn> InputDelegate = null)
            {
                ModLoader.LoaderTask<TIn, TOut>._Closure___11_0 CS_locals1 = new ModLoader.LoaderTask<TIn, TOut>._Closure___11_0();
                CS_locals1._VB_Local_Input = Input;
                CS_locals1._VB_Local_InputDelegate = InputDelegate;
                if (CS_locals1._VB_Local_InputDelegate == null)
                {
                    CS_locals1._VB_Local_InputDelegate = ((this.InputDelegate == null) ? null : new Func<TIn>(() => (TIn)this.InputDelegate()));
                }
                TIn tin = default(TIn);
                if (CS_locals1._VB_Local_Input != null)
                {
                    if (tin == null)
                    {
                        goto IL_0094;
                    }
                    ModLoader.LoaderTask<TIn, TOut>._Closure___11_0 CS_locals2 = CS_locals1;
                    if (!object.Equals(CS_locals2._VB_Local_Input, tin))
                    {
                        goto IL_0094;
                    }
                }
                if (CS_locals1._VB_Local_InputDelegate != null)
                {
                    ModBase.RunInUiWait(new Action(CS_locals1._Lambda___0));
                }
                IL_0094:
                return CS_locals1._VB_Local_Input;
            }

            public bool ShouldStart(ref TIn Input, bool IsForceRestart = false, bool IgnoreReloadTimeout = false)
            {
                try
                {
                    Input = this.StartGetInput(Input, null);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("加载输入获取失败（{0}）", this.Name), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                    base.Error = ex;
                    object lockState = this.LockState;
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                    lock (lockState)
                    {
                        base.State = ModBase.LoadState.Failed;
                    }
                }
                bool flag2;
                if (IsForceRestart)
                {
                    flag2 = true;
                }
                else
                {
                    if (Input == null == (this.Input == null))
                    {
                        if (Input != null)
                        {
                            if (!object.Equals(Input, this.Input))
                            {
                                goto IL_00D8;
                            }
                        }
                        return (base.State != ModBase.LoadState.Loading && base.State != ModBase.LoadState.Finished) || (!IgnoreReloadTimeout && this.ReloadTimeout != -1 && this.LastFinishedTime != 0L && checked(ModBase.GetTimeMs() - this.LastFinishedTime) >= (long)this.ReloadTimeout);
                    }
                    IL_00D8:
                    flag2 = true;
                }
                return flag2;
            }

            public override void Start(object Input = null, bool IsForceRestart = false)
            {
                ModLoader.LoaderTask<TIn, TOut>._Closure___13_0 CS_locals1 = new ModLoader.LoaderTask<TIn, TOut>._Closure___13_0();
                CS_locals1._VB_Me = this;
                CS_locals1._VB_Local_IsForceRestart = IsForceRestart;
                TIn tin = Conversions.ToGenericParameter<TIn>(Input);
                bool flag = this.ShouldStart(ref tin, CS_locals1._VB_Local_IsForceRestart, false);
                Input = tin;
                if (flag)
                {
                    if (base.State == ModBase.LoadState.Loading)
                    {
                        this.TriggerThreadInterrupt();
                    }
                    this.Input = Conversions.ToGenericParameter<TIn>(Input);
                    object lockState = this.LockState;
                    ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                    lock (lockState)
                    {
                        base.State = ModBase.LoadState.Loading;
                        this.Progress = -1.0;
                    }
                    this.LastRunningThread = new Thread(new ThreadStart(CS_locals1._Lambda___0))
                    {
                        Name = "L/" + this.Name,
                        Priority = this.ThreadPriority
                    };
                    try
                    {
                        this.LastRunningThread.Start();
                    }
                    catch (ThreadStateException ex)
                    {
                        Thread.Sleep(500);
                        this.LastRunningThread.Start();
                    }
                }
            }

            public override void Failed(Exception ex)
            {
                base.Error = ex;
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (this.IsCanceled || base.State >= ModBase.LoadState.Finished)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Failed;
                }
                Logger.Warn(ex, string.Format("加载线程 {0} 出错，已完成 {1}%", this.Name, Math.Round(this.Progress * 100.0)), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                this.TriggerThreadInterrupt();
            }

            public override void Cancel()
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State != ModBase.LoadState.Loading)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Canceled;
                }
                this.TriggerThreadInterrupt();
            }

            private void TriggerThreadInterrupt()
            {
                if (this.LastRunningThread != null)
                {
                    if (this.LastRunningThread.IsAlive)
                    {
                        this.LastRunningThread.Interrupt();
                        Logger.Trace(new Func<string>(this._Lambda___16_0), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                    }
                    this.LastRunningThread = null;
                }
            }

            public LoaderTask()
            {
                this.ReloadTimeout = -1;
                this.LastFinishedTime = 0L;
                this.LastRunningThread = null;
                this.Input = default(TIn);
                this.Output = default(TOut);
            }

            public LoaderTask(string Name, Action<ModLoader.LoaderTask<TIn, TOut>> LoadDelegate, Func<TIn> InputDelegate = null, ThreadPriority Priority = ThreadPriority.Normal)
            {
                this.ReloadTimeout = -1;
                this.LastFinishedTime = 0L;
                this.LastRunningThread = null;
                this.Input = default(TIn);
                this.Output = default(TOut);
                this.Name = Name;
                this.LoadDelegate = LoadDelegate;
                this.InputDelegate = (InputDelegate == null) ? null : new Func<object>(() => InputDelegate());
                this.ThreadPriority = Priority;
            }

            [CompilerGenerated]
            private string _Lambda___13_1()
            {
                return string.Format("加载线程 {0} 已{1}启动", this.Name, this.IsForceRestarting ? "强制" : "");
            }

            [CompilerGenerated]
            private string _Lambda___13_2()
            {
                return string.Format("加载线程 {0} 已完成", this.Name);
            }

            [CompilerGenerated]
            private string _Lambda___16_0()
            {
                return string.Format("加载线程 {0} 已中断", this.Name);
            }

            protected internal ThreadPriority ThreadPriority;

            protected internal Action<ModLoader.LoaderTask<TIn, TOut>> LoadDelegate;

            protected internal Func<object> InputDelegate;

            public int ReloadTimeout;

            public long LastFinishedTime;

            public Thread LastRunningThread;

            public TIn Input;

            public TOut Output;

            [CompilerGenerated]
            private sealed class _Closure___11_0
            {
                public _Closure___11_0()
                {
                }

                internal void _Lambda___0()
                {
                    this._VB_Local_Input = this._VB_Local_InputDelegate();
                }

                public TIn _VB_Local_Input;

                public Func<TIn> _VB_Local_InputDelegate;
            }

            [CompilerGenerated]
            private sealed class _Closure___13_0
            {
                public _Closure___13_0()
                {
                }

                internal void _Lambda___0()
                {
                    try
                    {
                        this._VB_Me.IsForceRestarting = this._VB_Local_IsForceRestart;
                        Logger.Trace(new Func<string>(this._VB_Me._Lambda___13_1), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                        this._VB_Me.LoadDelegate(this._VB_Me);
                        if (this._VB_Me.IsCanceled)
                        {
                            Logger.Warn(string.Format("加载线程 {0} 已中断但线程正常运行至结束，输出被弃用（最新线程：{1}）", this._VB_Me.Name, (this._VB_Me.LastRunningThread == null) ? (-1) : this._VB_Me.LastRunningThread.ManagedThreadId), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                        }
                        else
                        {
                            Logger.Trace(new Func<string>(this._VB_Me._Lambda___13_2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                            this._VB_Me.RaisePreviewFinish();
                            this._VB_Me.State = ModBase.LoadState.Finished;
                            this._VB_Me.LastFinishedTime = ModBase.GetTimeMs();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsCanceled())
                        {
                            Logger.Trace(ex, string.Format("加载线程 {0} 已取消，当前已完成 {1}%", this._VB_Me.Name, Math.Round(this._VB_Me.Progress * 100.0)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                            if (!this._VB_Me.IsCanceled)
                            {
                                this._VB_Me.State = ModBase.LoadState.Canceled;
                            }
                        }
                        else
                        {
                            this._VB_Me.Failed(ex);
                        }
                    }
                }

                public bool _VB_Local_IsForceRestart;

                public ModLoader.LoaderTask<TIn, TOut> _VB_Me;
            }
        }

        public class LoaderWorker : ModLoader.LoaderBase
        {
            public virtual IWorker Worker
            {
                [CompilerGenerated]
                get
                {
                    return this._Worker;
                }
                [CompilerGenerated]
                [MethodImpl(MethodImplOptions.Synchronized)]
                set
                {
                    Action action = new Action(this.Worker_Succeeded);
                    Action action2 = new Action(this.Cancel);
                    Action<Exception> action3 = new Action<Exception>(this.Failed);
                    IWorker worker = this._Worker;
                    if (worker != null)
                    {
                        worker.Succeeded -= action;
                        worker.Canceled -= action2;
                        worker.Failed -= action3;
                    }
                    this._Worker = value;
                    worker = this._Worker;
                    if (worker != null)
                    {
                        worker.Succeeded += action;
                        worker.Canceled += action2;
                        worker.Failed += action3;
                    }
                }
            }

            public LoaderWorker(string Name, IWorker Worker)
            {
                this.Name = Name;
                this.Worker = Worker;
                Worker.Progress.ProgressChanged += this._Lambda___4_0;
            }

            public override void Start(object Input = null, bool IsForceRestart = false)
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    base.State = ModBase.LoadState.Loading;
                }
                Logger.Trace(new Func<string>(this._Lambda___5_0), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                this.IsForceRestarting = IsForceRestart;
                this.Worker.Start(default(CancellationToken), null);
            }

            private void Worker_Succeeded()
            {
                Logger.Trace(new Func<string>(this._Lambda___6_0), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                base.RaisePreviewFinish();
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    base.State = ModBase.LoadState.Finished;
                }
            }

            public override void Cancel()
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State != ModBase.LoadState.Loading)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Canceled;
                }
                Logger.Trace(new Func<string>(this._Lambda___7_0), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                this.Worker.Cancel();
            }

            public override void Failed(Exception Ex)
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State >= ModBase.LoadState.Finished)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Failed;
                }
                Logger.Warn(Ex, string.Format("Worker {0} 出错，已完成 {1}%", this.Name, Math.Round(this.Progress * 100.0)), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                base.Error = Ex;
                this.Worker.Cancel();
            }

            [CompilerGenerated]
            private void _Lambda___4_0(ValueTuple<double, double> Raw)
            {
                this.Progress = Raw.Item1 + Raw.Item2;
            }

            [CompilerGenerated]
            private string _Lambda___5_0()
            {
                return string.Format("Worker {0} 已启动", this.Name);
            }

            [CompilerGenerated]
            private string _Lambda___6_0()
            {
                return string.Format("Worker {0} 已完成", this.Name);
            }

            [CompilerGenerated]
            private string _Lambda___7_0()
            {
                return string.Format("Worker {0} 已取消，当前已完成 {1}%", this.Name, Math.Round(this.Progress * 100.0));
            }

            [CompilerGenerated]
            [AccessedThroughProperty("Worker")]
            private IWorker _Worker;
        }

        public class LoaderCombo<InputType> : ModLoader.LoaderBase
        {
            public override double Progress
            {
                get
                {
                    ModBase.LoadState state = base.State;
                    double num;
                    if (state != ModBase.LoadState.Waiting)
                    {
                        if (state != ModBase.LoadState.Loading)
                        {
                            num = 1.0;
                        }
                        else
                        {
                            double num2 = 0.0;
                            double num3 = 0.0;
                            try
                            {
                                foreach (ModLoader.LoaderBase loaderBase in this.Loaders)
                                {
                                    num2 += loaderBase.ProgressWeight;
                                    num3 += loaderBase.ProgressWeight * loaderBase.Progress;
                                }
                            }
                            finally
                            {
                                List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                                ((IDisposable)enumerator).Dispose();
                            }
                            if (num2 == 0.0)
                            {
                                num = 0.0;
                            }
                            else
                            {
                                num = num3 / num2;
                            }
                        }
                    }
                    else
                    {
                        num = 0.0;
                    }
                    return num;
                }
                set
                {
                    throw new Exception("多重加载器不支持设置进度");
                }
            }

            public LoaderCombo(string Name, IEnumerable<ModLoader.LoaderBase> Loaders)
            {
                this.Loaders = new List<ModLoader.LoaderBase>();
                this.Loaders.Clear();
                try
                {
                    foreach (ModLoader.LoaderBase loaderBase in Loaders)
                    {
                        if (loaderBase != null)
                        {
                            this.Loaders.Add(loaderBase);
                            loaderBase.OnStateChangedThread += this.SubTaskStateChanged;
                            loaderBase.HasOnStateChangedThread = true;
                        }
                    }
                }
                finally
                {
                    IEnumerator<ModLoader.LoaderBase> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                this.InitParent(null);
                this.Name = Name;
            }

            public override void InitParent(ModLoader.LoaderBase Parent)
            {
                this.Parent = Parent;
                try
                {
                    foreach (ModLoader.LoaderBase loaderBase in this.Loaders)
                    {
                        loaderBase.InitParent(this);
                    }
                }
                finally
                {
                    List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
            }

            public override void Start(object Input = null, bool IsForceRestart = false)
            {
                this.IsForceRestarting = IsForceRestart;
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State == ModBase.LoadState.Loading)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Loading;
                }
                this.Input = Conversions.ToGenericParameter<InputType>(Input);
                if (IsForceRestart)
                {
                    try
                    {
                        foreach (ModLoader.LoaderBase loaderBase in this.Loaders)
                        {
                            loaderBase.State = ModBase.LoadState.Waiting;
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                }
                ModBase.RunInThread(new Action(this.Update));
            }

            public override void Cancel()
            {
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State != ModBase.LoadState.Loading && base.State != ModBase.LoadState.Waiting)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Canceled;
                }
                ModBase.RunInThread(new Action(this._Lambda___8_0));
            }

            public override void Failed(Exception Ex)
            {
                base.Error = Ex;
                object lockState = this.LockState;
                ObjectFlowControl.CheckForSyncLockOnValueType(lockState);
                lock (lockState)
                {
                    if (base.State >= ModBase.LoadState.Finished)
                    {
                        return;
                    }
                    base.State = ModBase.LoadState.Failed;
                }
                try
                {
                    foreach (ModLoader.LoaderBase loaderBase in this.Loaders)
                    {
                        loaderBase.Cancel();
                    }
                }
                finally
                {
                    List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
            }

            private void SubTaskStateChanged(ModLoader.LoaderBase Loader, ModBase.LoadState NewState, ModBase.LoadState OldState)
            {
                switch (NewState)
                {
                case ModBase.LoadState.Waiting:
                case ModBase.LoadState.Loading:
                    break;
                case ModBase.LoadState.Finished:
                    this.Update();
                    return;
                case ModBase.LoadState.Failed:
                    this.Failed(new Exception(Loader.Name + "失败", Loader.Error));
                    break;
                case ModBase.LoadState.Canceled:
                    this.Cancel();
                    return;
                default:
                    return;
                }
            }

            private void Update()
            {
                if (base.State != ModBase.LoadState.Finished && base.State != ModBase.LoadState.Failed && base.State != ModBase.LoadState.Canceled)
                {
                    bool flag = true;
                    bool flag2 = false;
                    object obj = this.Input;
                    try
                    {
                        foreach (ModLoader.LoaderBase loaderBase in this.Loaders)
                        {
                            ModBase.LoadState state = loaderBase.State;
                            if (state == ModBase.LoadState.Loading)
                            {
                                if (GeneralUtils.IsGenericInstanceOf(loaderBase, typeof(ModLoader.LoaderTask<, >)))
                                {
                                    object obj2 = loaderBase;
                                    Type type = null;
                                    string text = "ShouldStart";
                                    object[] array = new object[]
                                    {
                                        null,
                                        (obj != null && loaderBase.GetType().GenericTypeArguments.First<Type>() == obj.GetType()) ? obj : null
                                    };
                                    array[0] = true;
                                    if (Conversions.ToBoolean(NewLateBinding.LateGet(obj2, type, text, array, new string[] { "IgnoreReloadTimeout" }, null, null)))
                                    {
                                        Logger.Warn(string.Format("由于输入条件变更，重启进行中的加载器 {0}", loaderBase.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                                        goto IL_01B9;
                                    }
                                }
                                flag = false;
                                flag2 = true;
                                continue;
                            }
                            if (state == ModBase.LoadState.Finished)
                            {
                                if (GeneralUtils.IsGenericInstanceOf(loaderBase, typeof(ModLoader.LoaderTask<, >)))
                                {
                                    object obj3 = loaderBase;
                                    Type type2 = null;
                                    string text2 = "ShouldStart";
                                    object[] array2 = new object[]
                                    {
                                        null,
                                        (obj != null && loaderBase.GetType().GenericTypeArguments.First<Type>() == obj.GetType()) ? obj : null
                                    };
                                    array2[0] = true;
                                    if (Conversions.ToBoolean(NewLateBinding.LateGet(obj3, type2, text2, array2, new string[] { "IgnoreReloadTimeout" }, null, null)))
                                    {
                                        Logger.Info(string.Format("由于输入条件变更，重启已完成的加载器 {0}", loaderBase.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModLoader.vb");
                                        goto IL_01B9;
                                    }
                                    obj = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(loaderBase, null, "Output", new object[0], null, null, null));
                                }
                                if (loaderBase.Block && !flag)
                                {
                                    flag2 = true;
                                    continue;
                                }
                                continue;
                            }
                            IL_01B9:
                            flag = false;
                            if (!flag2)
                            {
                                if (obj != null)
                                {
                                    if (GeneralUtils.IsGenericInstanceOf(loaderBase, typeof(ModLoader.LoaderTask<, >)) || GeneralUtils.IsGenericInstanceOf(loaderBase, typeof(ModLoader.LoaderCombo<>)))
                                    {
                                        loaderBase.Start(RuntimeHelpers.GetObjectValue((loaderBase.GetType().GenericTypeArguments.First<Type>() == obj.GetType()) ? obj : null), this.IsForceRestarting);
                                    }
                                    else if (loaderBase is ModLoader.LoaderWorker)
                                    {
                                        loaderBase.Start(null, this.IsForceRestarting);
                                    }
                                    else
                                    {
                                        if (!(loaderBase is ModNet.LoaderDownload))
                                        {
                                            throw new Exception("未知的加载器类型（" + loaderBase.GetType().Name + "）");
                                        }
                                        loaderBase.Start(RuntimeHelpers.GetObjectValue((obj is List<ModNet.NetFile>) ? obj : null), this.IsForceRestarting);
                                    }
                                }
                                else
                                {
                                    loaderBase.Start(null, this.IsForceRestarting);
                                }
                                if (loaderBase.Block)
                                {
                                    flag2 = true;
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    if (flag)
                    {
                        base.RaisePreviewFinish();
                        base.State = ModBase.LoadState.Finished;
                        ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    }
                }
            }

            public static void GetLoaderList(object Loader, ref List<ModLoader.LoaderBase> List, bool RequireShow = true)
            {
                try
                {
                    foreach (object obj in ((IEnumerable)NewLateBinding.LateGet(Loader, null, "Loaders", new object[0], null, null, null)))
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(obj);
                        if (Conversions.ToBoolean(Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, null, "Show", new object[0], null, null, null)) || !RequireShow))
                        {
                            List.Add((ModLoader.LoaderBase)objectValue);
                        }
                        if (GeneralUtils.IsGenericInstanceOf(RuntimeHelpers.GetObjectValue(objectValue), typeof(ModLoader.LoaderCombo<>)))
                        {
                            ModLoader.LoaderCombo<InputType>.GetLoaderList(RuntimeHelpers.GetObjectValue(objectValue), ref List, true);
                        }
                    }
                }
                finally
                {
                    IEnumerator enumerator = default;
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            }

            public void GetLoaderList(ref List<ModLoader.LoaderBase> List, bool RequireShow = true)
            {
                ModLoader.LoaderCombo<InputType>.GetLoaderList(this, ref List, RequireShow);
            }

            public List<ModLoader.LoaderBase> GetLoaderList(bool RequireShow = true)
            {
                List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                this.GetLoaderList(ref list, RequireShow);
                return list;
            }

            [CompilerGenerated]
            private void _Lambda___8_0()
            {
                try
                {
                    foreach (ModLoader.LoaderBase loaderBase in this.Loaders)
                    {
                        loaderBase.Cancel();
                    }
                }
                finally
                {
                    List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
            }

            public List<ModLoader.LoaderBase> Loaders;

            public InputType Input;
        }

        private struct LoaderFolderDictionaryEntry
        {
            public override bool Equals(object obj)
            {
                bool flag;
                if (!(obj is ModLoader.LoaderFolderDictionaryEntry))
                {
                    flag = false;
                }
                else
                {
                    ModLoader.LoaderFolderDictionaryEntry loaderFolderDictionaryEntry = (ModLoader.LoaderFolderDictionaryEntry)obj;
                    flag = EqualityComparer<DateTime?>.Default.Equals(this.LastCheckTime, loaderFolderDictionaryEntry.LastCheckTime) && Operators.CompareString(this.FolderPath, loaderFolderDictionaryEntry.FolderPath, false) == 0;
                }
                return flag;
            }

            public DateTime? LastCheckTime;

            public string FolderPath;
        }

        public enum LoaderFolderRunType
        {
            RunOnUpdated,
            ForceRun,
            UpdateOnly
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

            internal bool _Lambda___9_0(ModLoader.LoaderBase l)
            {
                return l.State != ModBase.LoadState.Loading;
            }

            internal void _Lambda___9_1()
            {
                ModMain.FrmMain.BtnExtraDownload.Progress = ModLoader.LoaderTaskbarProgress;
            }

            internal double _Lambda___10_0(ModLoader.LoaderBase l)
            {
                return l.Progress;
            }

            public static readonly ModLoader._Closure___ _I = new ModLoader._Closure___();

            public static Func<ModLoader.LoaderBase, bool> _I9_0;

            public static Action _I9_1;

            public static Func<ModLoader.LoaderBase, double> _I10_0;
        }
    }
}
