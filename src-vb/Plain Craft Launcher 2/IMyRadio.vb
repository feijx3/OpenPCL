Imports System

Namespace PCL
    Public Interface IMyRadio
        Event Check As IMyRadio.CheckEventHandler

        Event Changed As IMyRadio.ChangedEventHandler

        Public Delegate Sub CheckEventHandler(sender As Object, e As ModBase.RouteEventArgs)

        Public Delegate Sub ChangedEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Interface
End Namespace
