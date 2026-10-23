Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Class ProgressObserver
        Inherits RateLimitedWorker

        Public Sub New(provider As ProgressProvider, updateAction As Action(Of Double), Optional minimalIntervalMs As Double = 70.0)
            MyBase.New(Sub()
                ProgressObserver.Observe(provider, updateAction)
            End Sub, minimalIntervalMs, RateLimitMode.ImmediateThenMerge)
            Me.provider = provider
            AddHandler provider.ProgressChanged, AddressOf Me.StartObserver
        End Sub

        Public Overrides Sub Dispose()
            RemoveHandler Me.provider.ProgressChanged, AddressOf Me.StartObserver
            MyBase.Dispose()
        End Sub

        <NullableContext(0)>
        Private Sub StartObserver(<TupleElementNames(New String() { "actual", "skiped" })> raw As ValueTuple(Of Double, Double))
            MyBase.Start()
        End Sub

        Private Shared Sub Observe(provider As ProgressProvider, updateAction As Action(Of Double))
            Dim increasing As Double = provider.GetIncreasing()
            updateAction(increasing)
        End Sub

        Private provider As ProgressProvider
    End Class
End Namespace
