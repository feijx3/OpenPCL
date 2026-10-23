Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(New Byte() { 0, 2 })>
    Public Class RedoableWorker
        Inherits RedoableWorkerBase(Of NoType)

        Public Sub New(<Nullable(New Byte() { 1, 2 })> workload As Action(Of CancellationToken?, ProgressProvider), <CallerMemberName()> Optional creatorMemberName As String = "")
            MyBase.New(Function(c As CancellationToken?, p As ProgressProvider)
                workload(c, p)
                Return Nothing
            End Function, Nothing, creatorMemberName)
        End Sub

        Public Sub New(workload As Action(Of CancellationToken?), <CallerMemberName()> Optional creatorMemberName As String = "")
            MyBase.New(Function(c As CancellationToken?, _ As ProgressProvider)
                workload(c)
                Return Nothing
            End Function, Nothing, creatorMemberName)
        End Sub

        Public Sub New(workload As Action, <CallerMemberName()> Optional creatorMemberName As String = "")
            MyBase.New(Function(_ As CancellationToken?, _ As ProgressProvider)
                workload()
                Return Nothing
            End Function, Nothing, creatorMemberName)
        End Sub
    End Class
End Namespace
