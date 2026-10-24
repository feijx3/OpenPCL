Imports System

Namespace PCL
    Public Class OurLoadingStateSimulator
        Implements ILoadingTrigger

        Public Sub New()
            Me._LoadingState = OurLoading.OurLoadingState.Unloaded
            Me.IsLoader = False
        End Sub

        Private Property _LoadingState As OurLoading.OurLoadingState

        Public Property LoadingState As OurLoading.OurLoadingState Implements PCL.ILoadingTrigger.LoadingState
            Get
                Return Me._LoadingState
            End Get
            Set(value As OurLoading.OurLoadingState)
                If Me._LoadingState <> value Then
                    Dim loadingState As OurLoading.OurLoadingState = Me._LoadingState
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
