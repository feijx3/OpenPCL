Imports System

Namespace PCL
    Public Class MyLoadingStateSimulator
        Implements ILoadingTrigger

        Public Sub New()
            Me._LoadingState = MyLoading.MyLoadingState.Unloaded
            Me.IsLoader = False
        End Sub

        Private Property _LoadingState As MyLoading.MyLoadingState

        Public Property LoadingState As MyLoading.MyLoadingState Implements PCL.ILoadingTrigger.LoadingState
            Get
                Return Me._LoadingState
            End Get
            Set(value As MyLoading.MyLoadingState)
                If Me._LoadingState <> value Then
                    Dim loadingState As MyLoading.MyLoadingState = Me._LoadingState
                    Me._LoadingState = value
                    Dim loadingStateChangedEvent As ILoadingTrigger.LoadingStateChangedEventHandler = Me.LoadingStateChangedEvent
                    If loadingStateChangedEvent IsNot Nothing Then
                        loadingStateChangedEvent(value, loadingState)
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property IsLoader As Boolean Implements PCL.ILoadingTrigger.IsLoader

        Public Event LoadingStateChanged As ILoadingTrigger.LoadingStateChangedEventHandler Implements PCL.ILoadingTrigger.LoadingStateChanged

        Public Event ProgressChanged As ILoadingTrigger.ProgressChangedEventHandler Implements PCL.ILoadingTrigger.ProgressChanged
    End Class
End Namespace
