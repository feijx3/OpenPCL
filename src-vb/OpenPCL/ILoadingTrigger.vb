Imports System

Namespace PCL
    Public Interface ILoadingTrigger
        ReadOnly Property IsLoader As Boolean

        Property LoadingState As OurLoading.OurLoadingState

        Event LoadingStateChanged As ILoadingTrigger.LoadingStateChangedEventHandler

        Event ProgressChanged As ILoadingTrigger.ProgressChangedEventHandler

        Public Delegate Sub LoadingStateChangedEventHandler(NewState As OurLoading.OurLoadingState, OldState As OurLoading.OurLoadingState)

        Public Delegate Sub ProgressChangedEventHandler(NewProgress As Double, OldProgress As Double)
    End Interface
End Namespace
