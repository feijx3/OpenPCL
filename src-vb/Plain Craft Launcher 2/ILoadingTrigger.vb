Imports System

Namespace PCL
    Public Interface ILoadingTrigger
        ReadOnly Property IsLoader As Boolean

        Property LoadingState As MyLoading.MyLoadingState

        Event LoadingStateChanged As ILoadingTrigger.LoadingStateChangedEventHandler

        Event ProgressChanged As ILoadingTrigger.ProgressChangedEventHandler

        Public Delegate Sub LoadingStateChangedEventHandler(NewState As MyLoading.MyLoadingState, OldState As MyLoading.MyLoadingState)

        Public Delegate Sub ProgressChangedEventHandler(NewProgress As Double, OldProgress As Double)
    End Interface
End Namespace
