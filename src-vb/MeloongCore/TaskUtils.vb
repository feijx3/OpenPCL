Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports MeloongCore.Extensions

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module TaskUtils
        Public Function WhenAll(<Nullable(New Byte() { 1, 1, 2, 1 })> tasks As IEnumerable(Of Func(Of ProgressProvider, Task)), <Nullable(2)> Optional progress As ProgressProvider = Nothing) As Task
            Dim CS$<>8__locals1 As TaskUtils.<>c__DisplayClass0_0 = New TaskUtils.<>c__DisplayClass0_0()
            CS$<>8__locals1.progress = progress
            Dim list As List(Of Func(Of ProgressProvider, Task)) = tasks.ToList()
            If Not list.Any() Then
                Dim progress2 As ProgressProvider = CS$<>8__locals1.progress
                If progress2 IsNot Nothing Then
                    progress2.Skip(ProgressProvider.ChildrenAction.Skiped)
                End If
                Return Task.CompletedTask
            End If
            CS$<>8__locals1.progressEach = 1.0 / CDbl(list.Count)
            Return Task.WhenAll(list.[Select](Function(task As Func(Of ProgressProvider, Task), _ As Integer)
                Dim <<WhenAll>b__0>d As TaskUtils.<>c__DisplayClass0_0.<<WhenAll>b__0>d
                <<WhenAll>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create()
                <<WhenAll>b__0>d.<>4__this = CS$<>8__locals1
                <<WhenAll>b__0>d.task = task
                <<WhenAll>b__0>d.<>1__state = -1
                <<WhenAll>b__0>d.<>t__builder.Start(Of TaskUtils.<>c__DisplayClass0_0.<<WhenAll>b__0>d)(<<WhenAll>b__0>d)
                Return <<WhenAll>b__0>d.<>t__builder.Task
            End Function))
        End Function

        <NullableContext(2)>
        Public Function WhenAll(Of TResult)(<Nullable(New Byte() { 1, 1, 2, 1, 1 })> tasks As IEnumerable(Of Func(Of ProgressProvider, Task(Of TResult))), Optional progress As ProgressProvider = Nothing) As <Nullable(1)> Task(Of TResult())
            Dim CS$<>8__locals1 As TaskUtils.<>c__DisplayClass1_0(Of TResult) = New TaskUtils.<>c__DisplayClass1_0(Of TResult)()
            CS$<>8__locals1.progress = progress
            Dim list As List(Of Func(Of ProgressProvider, Task(Of TResult))) = tasks.ToList()
            If Not list.Any() Then
                Dim progress2 As ProgressProvider = CS$<>8__locals1.progress
                If progress2 IsNot Nothing Then
                    progress2.Skip(ProgressProvider.ChildrenAction.Skiped)
                End If
                Return Task.FromResult(Of TResult())(Array.Empty(Of TResult)())
            End If
            CS$<>8__locals1.progressEach = 1.0 / CDbl(list.Count)
            Return Task.WhenAll(Of TResult)(list.[Select](Function(task As Func(Of ProgressProvider, Task(Of TResult)), _ As Integer)
                Dim <<WhenAll>b__0>d As TaskUtils.<>c__DisplayClass1_0(Of TResult).<<WhenAll>b__0>d
                <<WhenAll>b__0>d.<>t__builder = AsyncTaskMethodBuilder(Of TResult).Create()
                <<WhenAll>b__0>d.<>4__this = CS$<>8__locals1
                <<WhenAll>b__0>d.task = task
                <<WhenAll>b__0>d.<>1__state = -1
                <<WhenAll>b__0>d.<>t__builder.Start(Of TaskUtils.<>c__DisplayClass1_0(Of TResult).<<WhenAll>b__0>d)(<<WhenAll>b__0>d)
                Return <<WhenAll>b__0>d.<>t__builder.Task
            End Function))
        End Function

        Public Function WhenAll(tasks As IEnumerable(Of Task), <Nullable(2)> Optional progress As ProgressProvider = Nothing) As Task
            Dim <>9__2_ As Func(Of Task, Func(Of ProgressProvider, Task)) = TaskUtils.<>c.<>9__2_0
            Dim func As Func(Of Task, Func(Of ProgressProvider, Task)) = <>9__2_
            If <>9__2_ Is Nothing Then
                Dim func2 As Func(Of Task, Func(Of ProgressProvider, Task)) = Function(task As Task) Function(<Nullable(2)> _ As ProgressProvider) task
                func = func2
                TaskUtils.<>c.<>9__2_0 = func2
            End If
            Return TaskUtils.WhenAll(tasks.[Select](func), progress)
        End Function

        Public Function WhenAll(Of TResult)(tasks As IEnumerable(Of Task(Of TResult)), <Nullable(2)> Optional progress As ProgressProvider = Nothing) As Task(Of TResult())
            Dim <>9__3_ As Func(Of Task(Of TResult), Func(Of ProgressProvider, Task(Of TResult))) = TaskUtils.<>c__3(Of TResult).<>9__3_0
            Dim func As Func(Of Task(Of TResult), Func(Of ProgressProvider, Task(Of TResult))) = <>9__3_
            If <>9__3_ Is Nothing Then
                Dim func2 As Func(Of Task(Of TResult), Func(Of ProgressProvider, Task(Of TResult))) = Function(task As Task(Of TResult)) Function(_ As ProgressProvider) task
                func = func2
                TaskUtils.<>c__3(Of TResult).<>9__3_0 = func2
            End If
            Return TaskUtils.WhenAll(Of TResult)(tasks.[Select](func), progress)
        End Function

        Public Sub ForEach(Of T)(source As IEnumerable(Of T), body As Action(Of T))
            Parallel.ForEach(Of T)(source, body)
        End Sub

        Public Async Function ForEachAsync(Of T)(source As IEnumerable(Of T), maxDegreeOfParallelism As Integer, <Nullable(New Byte() { 1, 1, 2, 1 })> body As Func(Of T, ProgressProvider, Task), Optional cancellationToken As CancellationToken = Nothing, <Nullable(2)> Optional progress As ProgressProvider = Nothing) As Task
            Dim CS$<>8__locals1 As TaskUtils.<>c__DisplayClass5_0(Of T) = New TaskUtils.<>c__DisplayClass5_0(Of T)()
            CS$<>8__locals1.body = body
            CS$<>8__locals1.linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(New CancellationToken() { cancellationToken })
            Try
                CS$<>8__locals1.token = CS$<>8__locals1.linkedCancellationTokenSource.Token
                CS$<>8__locals1.enumeratorLock = New Object()
                CS$<>8__locals1.enumerator = source.GetEnumerator()
                Try
                    Await TaskUtils.WhenAll(Enumerable.Repeat(Of Func(Of ProgressProvider, Task))(New Func(Of ProgressProvider, Task)(CS$<>8__locals1.<ForEachAsync>g__WorkerAsync|0), maxDegreeOfParallelism), progress).NoCapture()
                Finally
                    If CS$<>8__locals1.enumerator IsNot Nothing Then
                        CS$<>8__locals1.enumerator.Dispose()
                    End If
                End Try
            Finally
                If CS$<>8__locals1.linkedCancellationTokenSource IsNot Nothing Then
                    CType(CS$<>8__locals1.linkedCancellationTokenSource, IDisposable).Dispose()
                End If
            End Try
        End Function

        Public Async Function RunProgramAsync(file As String, Optional arguments As String = "", Optional timeoutMs As Integer? = Nothing, <Nullable(2)> Optional encoding As Encoding = Nothing, Optional cancellationToken As CancellationToken = Nothing) As <TupleElementNames(New String() { "Output", "ExitCode" })> <Nullable(New Byte() { 1, 0, 1 })> Task(Of ValueTuple(Of String, Integer))
            cancellationToken.ThrowIfCancellationRequested()
            Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With { .Arguments = arguments, .FileName = PathUtils.ToShortPath(file), .UseShellExecute = False, .CreateNoWindow = True, .RedirectStandardOutput = True, .StandardOutputEncoding = encoding, .RedirectStandardError = True, .StandardErrorEncoding = encoding }
            Dim valueTuple As ValueTuple(Of String, Integer)
            Using program As Process = New Process()
                program.StartInfo = processStartInfo
                program.EnableRaisingEvents = True
                If Not program.Start() Then
                    Throw New InvalidOperationException("运行程序时出现意外错误：" + file + " " + arguments)
                End If
                Dim flag As Boolean = timeoutMs > 0
                Dim flag2 As Boolean
                Dim logInterpolatedStringHandler As Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.InfoLogLevel)(13, 3, flag2)
                If flag2 Then
                    logInterpolatedStringHandler.AppendLiteral("运行程序，并返回其输出：")
                    logInterpolatedStringHandler.AppendFormatted(file)
                    logInterpolatedStringHandler.AppendLiteral(" ")
                    logInterpolatedStringHandler.AppendFormatted(arguments)
                    logInterpolatedStringHandler.AppendFormatted(If(flag, String.Format("，最长可等待 {0}ms", timeoutMs), ""))
                End If
                Logger.Info(logInterpolatedStringHandler, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\TaskUtils.cs")
                Dim outputTask As Task(Of String) = program.StandardOutput.ReadToEndAsync()
                Dim errorTask As Task(Of String) = program.StandardError.ReadToEndAsync()
                Dim completionSource As TaskCompletionSource(Of Object) = New TaskCompletionSource(Of Object)(TaskCreationOptions.RunContinuationsAsynchronously)
                AddHandler program.Exited, Sub(_ As Object, _ As EventArgs)
                    completionSource.TrySetResult(Nothing)
                End Sub
                If program.HasExited Then
                    completionSource.TrySetResult(Nothing)
                End If
                Dim task As Task(Of Object) = completionSource.Task
                Dim task2 As Task = Task.Delay(If(flag, timeoutMs.Value, (-1)))
                Dim cancelTask As Task = Task.Delay(-1, cancellationToken)
                Dim logInterpolatedStringHandler2 As Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel) = New Logger.LogInterpolatedStringHandler(Of Logger.TraceLogLevel)(8, 1, flag2)
                If flag2 Then
                    logInterpolatedStringHandler2.AppendLiteral("等待程序 ")
                    logInterpolatedStringHandler2.AppendFormatted(Of Integer)(program.Id)
                    logInterpolatedStringHandler2.AppendLiteral(" 完成")
                End If
                Logger.Trace(logInterpolatedStringHandler2, LogBehavior.None, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Utils\TaskUtils.cs")
                Dim task3 As Task = Await Task.WhenAny(New Task() { task, task2, cancelTask }).NoCapture()
                If task3 IsNot task Then
                    Try
                        If Not program.HasExited Then
                            program.Kill()
                        End If
                    Catch ex As InvalidOperationException
                    End Try
                    If task3 Is cancelTask Then
                        Throw New OperationCanceledException(cancellationToken)
                    End If
                    Throw New TimeoutException("运行程序超时：" + file + " " + arguments)
                Else
                    Dim text As String = Await outputTask.NoCapture()
                    valueTuple = New ValueTuple(Of String, Integer)(text + Await errorTask.NoCapture(), program.ExitCode)
                End If
            End Using
            Return valueTuple
        End Function
    End Module
End Namespace
