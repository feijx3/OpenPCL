Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks

Namespace MeloongCore
    <NullableContext(2)>
    Public Interface IWorker
        Sub Start(Optional cancellationToken As CancellationToken = Nothing, Optional progressOverride As ProgressProvider = Nothing)

        Sub Cancel()

        Function WaitIfRunning(Optional millisecondsTimeout As Integer = -1, Optional cancellationToken As CancellationToken = Nothing) As Boolean

        <NullableContext(1)>
        Function WaitIfRunningAsync(Optional millisecondsTimeout As Integer = -1, Optional cancellationToken As CancellationToken = Nothing) As Task(Of Boolean)

        Event Started As Action

        Event Stopped As Action

        Event Succeeded As Action

        <Nullable(New Byte() { 2, 1 })>
        Event Failed As Action(Of Exception)

        Event Canceled As Action

        ReadOnly Property Running As Boolean

        ReadOnly Property HasSucceeded As Boolean

        ReadOnly Property LastSucceeded As Boolean

        <Nullable(1)>
        ReadOnly Property Progress As ProgressProvider
    End Interface
End Namespace
