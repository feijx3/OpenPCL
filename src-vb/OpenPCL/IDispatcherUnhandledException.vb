Imports System
Imports System.Windows.Threading

Namespace PCL
    Public Interface IDispatcherUnhandledException
        Sub DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs)
    End Interface
End Namespace
