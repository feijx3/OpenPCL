Imports System
Imports System.Runtime.CompilerServices
Imports ThrottleDebounce

Namespace MeloongCore
    Public Class RateLimitedWorker
        Implements IDisposable

        <NullableContext(1)>
        Public Sub New(workload As Action, minimalIntervalMs As Double, mode As RateLimitMode)
            Dim rateLimitedAction As RateLimitedAction
            If mode <> RateLimitMode.ImmediateThenMerge Then
                If mode <> RateLimitMode.WaitUnillNoNewInvoke Then
                    rateLimitedAction = Debouncer.Debounce(workload, TimeSpan.FromMilliseconds(minimalIntervalMs), True, False)
                Else
                    rateLimitedAction = Debouncer.Debounce(workload, TimeSpan.FromMilliseconds(minimalIntervalMs), False, True)
                End If
            Else
                rateLimitedAction = Throttler.Throttle(workload, TimeSpan.FromMilliseconds(minimalIntervalMs), True, True)
            End If
            Me.action = rateLimitedAction
            MyBase..ctor()
        End Sub

        Public Sub Start()
            If Not Me.disposed Then
                Me.action.Invoke()
            End If
        End Sub

        Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
            If Me.disposed Then
                Return
            End If
            Me.disposed = True
            Me.action.Dispose()
        End Sub

        <Nullable(1)>
        Private action As RateLimitedAction

        Private disposed As Boolean
    End Class
End Namespace
