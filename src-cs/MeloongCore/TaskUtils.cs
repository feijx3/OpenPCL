using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MeloongCore.Extensions;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class TaskUtils
    {
        public static Task WhenAll([Nullable(new byte[] { 1, 1, 2, 1 })] IEnumerable<Func<ProgressProvider, Task>> tasks, [Nullable(2)] ProgressProvider progress = null)
        {
            TaskUtils.<>c__DisplayClass0_0 CS$<>8__locals1 = new TaskUtils.<>c__DisplayClass0_0();
            CS$<>8__locals1.progress = progress;
            List<Func<ProgressProvider, Task>> list = tasks.ToList<Func<ProgressProvider, Task>>();
            if (!list.Any<Func<ProgressProvider, Task>>())
            {
                ProgressProvider progress2 = CS$<>8__locals1.progress;
                if (progress2 != null)
                {
                    progress2.Skip(ProgressProvider.ChildrenAction.Skiped);
                }
                return Task.CompletedTask;
            }
            CS$<>8__locals1.progressEach = 1.0 / (double)list.Count;
            return Task.WhenAll(list.Select<Func<ProgressProvider, Task>, Task>(delegate(Func<ProgressProvider, Task> task, int _)
            {
                TaskUtils.<>c__DisplayClass0_0.<<WhenAll>b__0>d <<WhenAll>b__0>d;
                <<WhenAll>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
                <<WhenAll>b__0>d.<>4__this = CS$<>8__locals1;
                <<WhenAll>b__0>d.task = task;
                <<WhenAll>b__0>d.<>1__state = -1;
                <<WhenAll>b__0>d.<>t__builder.Start<TaskUtils.<>c__DisplayClass0_0.<<WhenAll>b__0>d>(ref <<WhenAll>b__0>d);
                return <<WhenAll>b__0>d.<>t__builder.Task;
            }));
        }

        [NullableContext(2)]
        [return: Nullable(1)]
        public static Task<TResult[]> WhenAll<TResult>([Nullable(new byte[] { 1, 1, 2, 1, 1 })] IEnumerable<Func<ProgressProvider, Task<TResult>>> tasks, ProgressProvider progress = null)
        {
            TaskUtils.<>c__DisplayClass1_0<TResult> CS$<>8__locals1 = new TaskUtils.<>c__DisplayClass1_0<TResult>();
            CS$<>8__locals1.progress = progress;
            List<Func<ProgressProvider, Task<TResult>>> list = tasks.ToList<Func<ProgressProvider, Task<TResult>>>();
            if (!list.Any<Func<ProgressProvider, Task<TResult>>>())
            {
                ProgressProvider progress2 = CS$<>8__locals1.progress;
                if (progress2 != null)
                {
                    progress2.Skip(ProgressProvider.ChildrenAction.Skiped);
                }
                return Task.FromResult<TResult[]>(Array.Empty<TResult>());
            }
            CS$<>8__locals1.progressEach = 1.0 / (double)list.Count;
            return Task.WhenAll<TResult>(list.Select<Func<ProgressProvider, Task<TResult>>, Task<TResult>>(delegate(Func<ProgressProvider, Task<TResult>> task, int _)
            {
                TaskUtils.<>c__DisplayClass1_0<TResult>.<<WhenAll>b__0>d <<WhenAll>b__0>d;
                <<WhenAll>b__0>d.<>t__builder = AsyncTaskMethodBuilder<TResult>.Create();
                <<WhenAll>b__0>d.<>4__this = CS$<>8__locals1;
                <<WhenAll>b__0>d.task = task;
                <<WhenAll>b__0>d.<>1__state = -1;
                <<WhenAll>b__0>d.<>t__builder.Start<TaskUtils.<>c__DisplayClass1_0<TResult>.<<WhenAll>b__0>d>(ref <<WhenAll>b__0>d);
                return <<WhenAll>b__0>d.<>t__builder.Task;
            }));
        }

        public static Task WhenAll(IEnumerable<Task> tasks, [Nullable(2)] ProgressProvider progress = null)
        {
            return TaskUtils.WhenAll(tasks.Select<Task, Func<ProgressProvider, Task>>((Task task) => ([Nullable(2)] ProgressProvider _) => task), progress);
        }

        public static Task<TResult[]> WhenAll<[Nullable(2)] TResult>(IEnumerable<Task<TResult>> tasks, [Nullable(2)] ProgressProvider progress = null)
        {
            return TaskUtils.WhenAll<TResult>(tasks.Select<Task<TResult>, Func<ProgressProvider, Task<TResult>>>((Task<TResult> task) => (ProgressProvider _) => task), progress);
        }

        public static void ForEach<[Nullable(2)] T>(IEnumerable<T> source, Action<T> body)
        {
            Parallel.ForEach<T>(source, body);
        }

        public static async Task ForEachAsync<[Nullable(2)] T>(IEnumerable<T> source, int maxDegreeOfParallelism, [Nullable(new byte[] { 1, 1, 2, 1 })] Func<T, ProgressProvider, Task> body, CancellationToken cancellationToken = default(CancellationToken), [Nullable(2)] ProgressProvider progress = null)
        {
            TaskUtils.<>c__DisplayClass5_0<T> CS$<>8__locals1 = new TaskUtils.<>c__DisplayClass5_0<T>();
            CS$<>8__locals1.body = body;
            CS$<>8__locals1.linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[] { cancellationToken });
            try
            {
                CS$<>8__locals1.token = CS$<>8__locals1.linkedCancellationTokenSource.Token;
                CS$<>8__locals1.enumeratorLock = new object();
                CS$<>8__locals1.enumerator = source.GetEnumerator();
                try
                {
                    await TaskUtils.WhenAll(Enumerable.Repeat<Func<ProgressProvider, Task>>(new Func<ProgressProvider, Task>(CS$<>8__locals1.<ForEachAsync>g__WorkerAsync|0), maxDegreeOfParallelism), progress).NoCapture();
                }
                finally
                {
                    if (CS$<>8__locals1.enumerator != null)
                    {
                        CS$<>8__locals1.enumerator.Dispose();
                    }
                }
            }
            finally
            {
                if (CS$<>8__locals1.linkedCancellationTokenSource != null)
                {
                    ((IDisposable)CS$<>8__locals1.linkedCancellationTokenSource).Dispose();
                }
            }
        }

        [return: TupleElementNames(new string[] { "Output", "ExitCode" })]
        [return: Nullable(new byte[] { 1, 0, 1 })]
        public static async Task<ValueTuple<string, int>> RunProgramAsync(string file, string arguments = "", int? timeoutMs = null, [Nullable(2)] Encoding encoding = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                Arguments = arguments,
                FileName = PathUtils.ToShortPath(file),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = encoding,
                RedirectStandardError = true,
                StandardErrorEncoding = encoding
            };
            ValueTuple<string, int> valueTuple;
            using (Process program = new Process())
            {
                program.StartInfo = processStartInfo;
                program.EnableRaisingEvents = true;
                if (!program.Start())
                {
                    throw new InvalidOperationException("运行程序时出现意外错误：" + file + " " + arguments);
                }
                bool flag = timeoutMs > 0;
                bool flag2;
                Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(13, 3, out flag2);
                if (flag2)
                {
                    logInterpolatedStringHandler.AppendLiteral("运行程序，并返回其输出：");
                    logInterpolatedStringHandler.AppendFormatted(file);
                    logInterpolatedStringHandler.AppendLiteral(" ");
                    logInterpolatedStringHandler.AppendFormatted(arguments);
                    logInterpolatedStringHandler.AppendFormatted(flag ? string.Format("，最长可等待 {0}ms", timeoutMs) : "");
                }
                Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\TaskUtils.cs");
                Task<string> outputTask = program.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = program.StandardError.ReadToEndAsync();
                TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                program.Exited += delegate(object _, EventArgs _)
                {
                    completionSource.TrySetResult(null);
                };
                if (program.HasExited)
                {
                    completionSource.TrySetResult(null);
                }
                Task<object> task = completionSource.Task;
                Task task2 = Task.Delay(flag ? timeoutMs.Value : (-1));
                Task cancelTask = Task.Delay(-1, cancellationToken);
                Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel>(8, 1, out flag2);
                if (flag2)
                {
                    logInterpolatedStringHandler2.AppendLiteral("等待程序 ");
                    logInterpolatedStringHandler2.AppendFormatted<int>(program.Id);
                    logInterpolatedStringHandler2.AppendLiteral(" 完成");
                }
                Logger.Trace(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\TaskUtils.cs");
                Task task3 = await Task.WhenAny(new Task[] { task, task2, cancelTask }).NoCapture<Task>();
                if (task3 != task)
                {
                    try
                    {
                        if (!program.HasExited)
                        {
                            program.Kill();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    if (task3 == cancelTask)
                    {
                        throw new OperationCanceledException(cancellationToken);
                    }
                    throw new TimeoutException("运行程序超时：" + file + " " + arguments);
                }
                else
                {
                    string text = await outputTask.NoCapture<string>();
                    valueTuple = new ValueTuple<string, int>(text + await errorTask.NoCapture<string>(), program.ExitCode);
                }
            }
            return valueTuple;
        }
    }
}
