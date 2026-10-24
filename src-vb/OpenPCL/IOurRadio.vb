Imports System

Namespace PCL
    Public Interface IOurRadio
        Event Check As IOurRadio.CheckEventHandler

        Event Changed As IOurRadio.ChangedEventHandler

        Public Delegate Sub CheckEventHandler(sender As Object, e As ModBase.RouteEventArgs)

        Public Delegate Sub ChangedEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Interface
End Namespace
