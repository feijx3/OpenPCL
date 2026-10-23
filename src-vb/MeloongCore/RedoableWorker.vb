Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(New Byte() { 0, 1 })>
    Public Class RedoableWorker(Of TOut)
        Inherits RedoableWorkerBase(Of TOut)

        Public Sub New(<Nullable(New Byte() { 1, 2, 1 })> workload As Func(Of CancellationToken?, ProgressProvider, TOut), <CallerMemberName()> Optional creatorMemberName As String = "")
            MyBase.New(workload, Nothing, creatorMemberName)
        End Sub

        Public Sub New(workload As Func(Of CancellationToken?, TOut), <CallerMemberName()> Optional creatorMemberName As String = "")
            MyBase.New(Function(c As CancellationToken?, <Nullable(2)> _ As ProgressProvider) workload(c), Nothing, creatorMemberName)
        End Sub

        Public Sub New(workload As Func(Of TOut), <CallerMemberName()> Optional creatorMemberName As String = "")
            MyBase.New(Function(_ As CancellationToken?, <Nullable(2)> _ As ProgressProvider) workload(), Nothing, creatorMemberName)
        End Sub
    End Class
End Namespace
