using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MeloongCore.Extensions;

namespace MeloongCore
{
    [NullableContext(2)]
    [Nullable(0)]
    public abstract class RedoableWorkerBase<TOut> : IWorker
    {
        [NullableContext(1)]
        protected RedoableWorkerBase([Nullable(new byte[] { 1, 2, 1 })] Func<CancellationToken?, ProgressProvider, TOut> workload, [Nullable(2)] ProgressProvider progress = null, [CallerMemberName] string creatorMemberName = "")
        {
        }

        public bool Running
        {
            get
            {
                bool flag2;
                lock (this)
                {
                    flag2 = this.running;
                }
                return flag2;
            }
        }

        public event Action Started;

        public event Action Stopped;

        public bool HasSucceeded
        {
            get
            {
                bool flag2;
                lock (this)
                {
                    flag2 = this.hasSucceeded;
                }
                return flag2;
            }
        }

        public event Action Succeeded;

        [Nullable(new byte[] { 2, 1 })]
        [field: Nullable(new byte[] { 2, 1 })]
        public event Action<Exception> Failed;

        public event Action Canceled;

        public bool LastSucceeded
        {
            get
            {
                bool flag2;
                lock (this)
                {
                    flag2 = this.lastSucceeded;
                }
                return flag2;
            }
        }

        [Nullable(1)]
        public TOut LastResult
        {
            [NullableContext(1)]
            get
            {
                TOut tout2;
                lock (this)
                {
                    if (typeof(TOut) == typeof(NoType))
                    {
                        throw new InvalidOperationException("该 Worker 没有返回值。");
                    }
                    if (!this.hasSucceeded)
                    {
                        throw new InvalidOperationException("从未成功完成过。");
                    }
                    TOut tout;
                    if (this.lastResult != null)
                    {
                        tout = (TOut)((object)this.lastResult);
                    }
                    else
                    {
                        tout2 = default(TOut);
                        tout = tout2;
                    }
                    tout2 = tout;
                }
                return tout2;
            }
        }

        [Nullable(1)]
        public ProgressProvider Progress
        {
            [NullableContext(1)]
            get;
            [NullableContext(1)]
            private set;
        } = progress ?? new ProgressProvider();

        public void Start(CancellationToken cancellationToken = default(CancellationToken), ProgressProvider progressOverride = null)
        {
            lock (this)
            {
                this.lastToken = cancellationToken;
                if (progressOverride != null)
                {
                    this.Progress = progressOverride;
                }
                if (this.running)
                {
                    bool flag2;
                    Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(7, 1, out flag2);
                    if (flag2)
                    {
                        logInterpolatedStringHandler.AppendFormatted(this.<creatorMemberName>P);
                        logInterpolatedStringHandler.AppendLiteral("：接到重启请求");
                    }
                    Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Worker\\RedoableWorker.cs");
                    this.pendingRedo = true;
                    CancellationTokenSource cancellationTokenSource = this.realCts;
                    if (cancellationTokenSource == null)
                    {
                        return;
                    }
                    cancellationTokenSource.Cancel();
                    return;
                }
                else
                {
                    this.running = true;
                    this.idleEvent.Reset();
                    this.realCts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[] { this.lastToken });
                }
            }
            new Thread(new ThreadStart(this._Start))
            {
                IsBackground = true,
                Name = "W/" + this.<creatorMemberName>P
            }.Start();
        }

        private void _Start()
        {
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(5, 1, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendFormatted(this.<creatorMemberName>P);
                logInterpolatedStringHandler.AppendLiteral("：运行开始");
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Worker\\RedoableWorker.cs");
            Action started = this.Started;
            if (started != null)
            {
                started();
            }
            for (;;)
            {
                try
                {
                    this.realCts.Token.ThrowIfCancellationRequested();
                    ProgressProvider progress = this.Progress;
                    if (progress != null)
                    {
                        progress.Reset();
                    }
                    TOut tout = this.<workload>P(new CancellationToken?(this.realCts.Token), this.Progress);
                    this.realCts.Token.ThrowIfCancellationRequested();
                    RedoableWorkerBase<TOut> redoableWorkerBase = this;
                    lock (redoableWorkerBase)
                    {
                        CancellationTokenSource cancellationTokenSource = this.realCts;
                        if (cancellationTokenSource != null)
                        {
                            cancellationTokenSource.Dispose();
                        }
                        if (this.pendingRedo)
                        {
                            this.pendingRedo = false;
                            this.realCts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[] { this.lastToken });
                            continue;
                        }
                        this.realCts = null;
                        this.lastSucceeded = true;
                        this.lastResult = tout;
                        this.hasSucceeded = true;
                        this.running = false;
                        this.idleEvent.Set();
                        ProgressProvider progress2 = this.Progress;
                        if (progress2 != null)
                        {
                            progress2.Finish(ProgressProvider.ChildrenAction.Finished);
                        }
                    }
                    Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(5, 1, out flag);
                    if (flag)
                    {
                        logInterpolatedStringHandler2.AppendFormatted(this.<creatorMemberName>P);
                        logInterpolatedStringHandler2.AppendLiteral("：运行成功");
                    }
                    Logger.Info(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Worker\\RedoableWorker.cs");
                    Action succeeded = this.Succeeded;
                    if (succeeded != null)
                    {
                        succeeded();
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.IsCanceled())
                    {
                        Logger.Log(ex, this.<creatorMemberName>P + "：运行失败" + (this.pendingRedo ? "，但即将重启，或可忽略" : ""), this.pendingRedo ? LogLevel.Warn : LogLevel.Error, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Worker\\RedoableWorker.cs");
                    }
                    RedoableWorkerBase<TOut> redoableWorkerBase = this;
                    lock (redoableWorkerBase)
                    {
                        CancellationTokenSource cancellationTokenSource2 = this.realCts;
                        if (cancellationTokenSource2 != null)
                        {
                            cancellationTokenSource2.Dispose();
                        }
                        if (this.pendingRedo)
                        {
                            bool flag2;
                            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(5, 1, out flag2);
                            if (flag2)
                            {
                                logInterpolatedStringHandler3.AppendFormatted(this.<creatorMemberName>P);
                                logInterpolatedStringHandler3.AppendLiteral("：重启开始");
                            }
                            Logger.Info(ref logInterpolatedStringHandler3, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Worker\\RedoableWorker.cs");
                            this.pendingRedo = false;
                            this.realCts = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[] { this.lastToken });
                            continue;
                        }
                        this.realCts = null;
                        this.lastSucceeded = false;
                        this.running = false;
                        this.idleEvent.Set();
                        ProgressProvider progress3 = this.Progress;
                        if (progress3 != null)
                        {
                            progress3.Skip(ProgressProvider.ChildrenAction.Skiped);
                        }
                    }
                    if (ex.IsCanceled())
                    {
                        Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler4 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(6, 1, out flag);
                        if (flag)
                        {
                            logInterpolatedStringHandler4.AppendFormatted(this.<creatorMemberName>P);
                            logInterpolatedStringHandler4.AppendLiteral("：运行已取消");
                        }
                        Logger.Info(ref logInterpolatedStringHandler4, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Worker\\RedoableWorker.cs");
                        Action canceled = this.Canceled;
                        if (canceled != null)
                        {
                            canceled();
                        }
                    }
                    else
                    {
                        Action<Exception> failed = this.Failed;
                        if (failed != null)
                        {
                            failed(ex);
                        }
                    }
                }
                break;
            }
            Action stopped = this.Stopped;
            if (stopped == null)
            {
                return;
            }
            stopped();
        }

        public void Cancel()
        {
            lock (this)
            {
                this.pendingRedo = false;
                bool flag2;
                Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(7, 1, out flag2);
                if (flag2)
                {
                    logInterpolatedStringHandler.AppendFormatted(this.<creatorMemberName>P);
                    logInterpolatedStringHandler.AppendLiteral("：接到取消请求");
                }
                Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Worker\\RedoableWorker.cs");
                CancellationTokenSource cancellationTokenSource = this.realCts;
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                }
            }
        }

        public bool WaitIfRunning(int millisecondsTimeout = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.idleEvent.Wait(millisecondsTimeout, cancellationToken);
        }

        [NullableContext(1)]
        public async Task<bool> WaitIfRunningAsync(int millisecondsTimeout = -1, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            RegisteredWaitHandle waitHandle = ThreadPool.RegisterWaitForSingleObject(this.idleEvent.WaitHandle, delegate(object _, bool timedOut)
            {
                completionSource.TrySetResult(!timedOut);
            }, null, millisecondsTimeout, true);
            bool flag;
            using (cancellationToken.Register(delegate
            {
                completionSource.TrySetCanceled(cancellationToken);
            }))
            {
                try
                {
                    flag = await completionSource.Task.NoCapture<bool>();
                }
                finally
                {
                    waitHandle.Unregister(null);
                }
            }
            return flag;
        }

        [Nullable(new byte[] { 1, 2, 1 })]
        [CompilerGenerated]
        private Func<CancellationToken?, ProgressProvider, TOut> <workload>P = workload;

        [Nullable(1)]
        [CompilerGenerated]
        private string <creatorMemberName>P = creatorMemberName;

        private bool running;

        private bool hasSucceeded;

        private bool lastSucceeded;

        private object lastResult;

        private bool pendingRedo;

        private CancellationTokenSource realCts;

        private CancellationToken lastToken;

        [Nullable(1)]
        private readonly ManualResetEventSlim idleEvent = new ManualResetEventSlim(true);
    }
}
