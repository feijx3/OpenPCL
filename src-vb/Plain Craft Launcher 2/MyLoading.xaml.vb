Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class MyLoading
        Inherits Grid

        Public Event IsErrorChanged As MyLoading.IsErrorChangedEventHandler

        Public Event StateChanged As MyLoading.StateChangedEventHandler

        Public Event Click As MyLoading.ClickEventHandler

        Public Property AutoRun As Boolean

        Public Property Foreground As SolidColorBrush
            Get
                Return CType(MyBase.GetValue(MyLoading.ForegroundProperty), SolidColorBrush)
            End Get
            Set(value As SolidColorBrush)
                MyBase.SetValue(MyLoading.ForegroundProperty, value)
            End Set
        End Property

        Public Sub New()
            AddHandler Me.IsErrorChanged, Sub(a0 As Object, a1 As Boolean)
                Me.RefreshText()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshText()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.InitState()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshState()
            End Sub
            AddHandler MyBase.Unloaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshState()
            End Sub
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.Button_MouseUp
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.Button_MouseDown
            AddHandler MyBase.MouseLeave, AddressOf Me.Button_MouseLeave
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.Button_MouseLeave
            Me.AutoRun = True
            Me.Uuid = ModBase.GetUuid()
            Me._ShowProgress = False
            Me._Text = "加载中"
            Me._TextError = "加载失败"
            Me.TextErrorInherit = True
            Me._OuterState = MyLoading.MyLoadingState.Unloaded
            Me._InnerState = MyLoading.MyLoadingState.Unloaded
            Me.HasAnimation = True
            Me.IsLooping = False
            Me.ErrorAnimationWaiting = False
            Me.IsMouseDown = False
            Me.InitializeComponent()
            MyBase.SetResourceReference(MyLoading.ForegroundProperty, "ColorBrush3")
        End Sub

        Private Property _ShowProgress As Boolean

        Public Property ShowProgress As Boolean
            Get
                Return Me._ShowProgress
            End Get
            Set(value As Boolean)
                If Me._ShowProgress <> value Then
                    Me._ShowProgress = value
                    Me.RefreshText()
                End If
            End Set
        End Property

        Public Property Text As String
            Get
                Return Me._Text
            End Get
            Set(value As String)
                Me._Text = value
                Me.RefreshText()
            End Set
        End Property

        Public Property TextError As String
            Get
                Return Me._TextError
            End Get
            Set(value As String)
                Me._TextError = value
                Me.RefreshText()
            End Set
        End Property

        Public Property TextErrorInherit As Boolean

        Private Sub RefreshText()
            ModBase.RunInUi(Sub()
                If Me.InnerState = MyLoading.MyLoadingState.[Error] Then
                    If Not Me.TextErrorInherit OrElse Not Me.State.IsLoader Then
                        Me.LabText.Text = Me.TextError
                        Return
                    End If
                    Dim ex As Exception = CType(NewLateBinding.LateGet(Me.State, Nothing, "Error", New Object(-1) {}, Nothing, Nothing, Nothing), Exception)
                    If ex Is Nothing Then
                        Me.LabText.Text = "未知错误"
                        Return
                    End If
                    Me.LabText.Text = ex.GetDisplay(False)
                    Return
                Else
                    If Me.ShowProgress AndAlso Me.State.IsLoader Then
                        Me.LabText.Text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Me.Text + " - ", NewLateBinding.LateGet(Nothing, GetType(Math), "Floor", New Object() { Operators.MultiplyObject(NewLateBinding.LateGet(Me.State, Nothing, "Progress", New Object(-1) {}, Nothing, Nothing, Nothing), 100) }, Nothing, Nothing, Nothing)), "%"))
                        Return
                    End If
                    Me.LabText.Text = Me.Text
                    Return
                End If
            End Sub, False)
        End Sub

        Private Overridable Property _State As ILoadingTrigger
            <CompilerGenerated()>
            Get
                Return Me.__State
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As ILoadingTrigger)
                Dim progressChangedEventHandler As ILoadingTrigger.ProgressChangedEventHandler = Sub(a0 As Double, a1 As Double)
                    Me.RefreshText()
                End Sub
                Dim loadingStateChangedEventHandler As ILoadingTrigger.LoadingStateChangedEventHandler = Sub(a0 As MyLoading.MyLoadingState, a1 As MyLoading.MyLoadingState)
                    Me.RefreshState()
                End Sub
                Dim loadingTrigger As ILoadingTrigger = Me.__State
                If loadingTrigger IsNot Nothing Then
                    RemoveHandler loadingTrigger.ProgressChanged, progressChangedEventHandler
                    RemoveHandler loadingTrigger.LoadingStateChanged, loadingStateChangedEventHandler
                End If
                Me.__State = value
                loadingTrigger = Me.__State
                If loadingTrigger IsNot Nothing Then
                    AddHandler loadingTrigger.ProgressChanged, progressChangedEventHandler
                    AddHandler loadingTrigger.LoadingStateChanged, loadingStateChangedEventHandler
                End If
            End Set
        End Property

        Public Property State As ILoadingTrigger
            Get
                Me.InitState()
                Return Me._State
            End Get
            Set(value As ILoadingTrigger)
                Me._State = value
                Me.RefreshState()
            End Set
        End Property

        Private Sub InitState()
            If Me._State Is Nothing Then
                Me._State = New MyLoadingStateSimulator()
                If Me.AutoRun Then
                    Me._State.LoadingState = MyLoading.MyLoadingState.Run
                End If
            End If
        End Sub

        Private Sub RefreshState()
            If Me._State.LoadingState = MyLoading.MyLoadingState.Run AndAlso Not MyBase.IsLoaded Then
                Me.InnerState = MyLoading.MyLoadingState.[Stop]
            End If
            Me.InnerState = Me._State.LoadingState
            Me.OuterState = Me._State.LoadingState
            Me.AniLoop()
        End Sub

        Private Property _OuterState As MyLoading.MyLoadingState

        Private Property OuterState As MyLoading.MyLoadingState
            Get
                Return Me._OuterState
            End Get
            Set(value As MyLoading.MyLoadingState)
                If Me._OuterState <> value Then
                    Dim outerState As MyLoading.MyLoadingState = Me._OuterState
                    Me._OuterState = value
                    Dim stateChangedEvent As MyLoading.StateChangedEventHandler = Me.StateChangedEvent
                    If stateChangedEvent IsNot Nothing Then
                        stateChangedEvent(Me, value, outerState)
                    End If
                    If outerState = MyLoading.MyLoadingState.[Error] <> (value = MyLoading.MyLoadingState.[Error]) Then
                        Dim isErrorChangedEvent As MyLoading.IsErrorChangedEventHandler = Me.IsErrorChangedEvent
                        If isErrorChangedEvent IsNot Nothing Then
                            isErrorChangedEvent(Me, value = MyLoading.MyLoadingState.[Error])
                        End If
                    End If
                End If
            End Set
        End Property

        Private Property _InnerState As MyLoading.MyLoadingState

        Private Property InnerState As MyLoading.MyLoadingState
            Get
                Return Me._InnerState
            End Get
            Set(value As MyLoading.MyLoadingState)
                If Me._InnerState <> value Then
                    Dim innerState As Integer = CInt(Me._InnerState)
                    Me._InnerState = value
                    Me.AniLoop()
                    If innerState = 2 <> (value = MyLoading.MyLoadingState.[Error]) Then
                        Me.ErrorAnimation(Me, value = MyLoading.MyLoadingState.[Error])
                    End If
                End If
            End Set
        End Property

        Public Property HasAnimation As Boolean

        Private Sub AniLoop()
            If Me.HasAnimation AndAlso Not Me.IsLooping AndAlso Me.InnerState = MyLoading.MyLoadingState.Run AndAlso ModAnimation.AniSpeed <= 10.0 AndAlso MyBase.IsLoaded Then
                Me.IsLooping = True
                Me.ErrorAnimationWaiting = True
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaRotateTransform(Me.PathPickaxe, -20.0 - CType(Me.PathPickaxe.RenderTransform, RotateTransform).Angle, 350, 250, New ModAnimation.AniEaseInBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaRotateTransform(Me.PathPickaxe, 50.0, 900, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), True), ModAnimation.AaRotateTransform(Me.PathPickaxe, 25.0, 900, 0, New ModAnimation.AniEaseOutElastic(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaCode(Sub()
                    Me.PathLeft.Opacity = 1.0
                    Me.PathLeft.Margin = New Thickness(7.0, 41.0, 0.0, 0.0)
                    Me.PathRight.Opacity = 1.0
                    Me.PathRight.Margin = New Thickness(14.0, 41.0, 0.0, 0.0)
                    Me.ErrorAnimationWaiting = False
                End Sub, 0, False), ModAnimation.AaOpacity(Me.PathLeft, -1.0, 100, 50, Nothing, False), ModAnimation.AaX(Me.PathLeft, -5.0, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaY(Me.PathLeft, -6.0, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.PathRight, -1.0, 100, 50, Nothing, False), ModAnimation.AaX(Me.PathRight, 5.0, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaY(Me.PathRight, -6.0, 180, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                    Me.IsLooping = False
                    Me.AniLoop()
                End Sub, 0, True) }, "MyLoader Loop " + Conversions.ToString(Me.Uuid) + "/" + Conversions.ToString(ModBase.GetUuid()), False)
            End If
        End Sub

        Private Sub ErrorAnimation(sender As Object, isError As Boolean)
            If isError Then
                Dim num As Integer = If(Me.ErrorAnimationWaiting, 400, 0)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.PanBack, MyLoading.ForegroundProperty, "ColorBrushRedLight", 300, 0, Nothing, False), ModAnimation.AaOpacity(Me.PathError, 1.0 - Me.PathError.Opacity, 100, 300 + num, Nothing, False), ModAnimation.AaScaleTransform(Me.PathError, 1.0 - CType(Me.PathError.RenderTransform, ScaleTransform).ScaleX, 400, 300 + num, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False) }, "MyLoader Error " + Conversions.ToString(Me.Uuid), False)
                Return
            End If
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.PathError, -Me.PathError.Opacity, 100, 0, Nothing, False), ModAnimation.AaScaleTransform(Me.PathError, 0.5 - CType(Me.PathError.RenderTransform, ScaleTransform).ScaleX, 200, 0, Nothing, False), ModAnimation.AaColor(Me.PanBack, MyLoading.ForegroundProperty, "ColorBrush3", 300, 0, Nothing, False) }, "MyLoader Error " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub Button_MouseUp(sender As Object, e As MouseButtonEventArgs)
            Dim clickEvent As MyLoading.ClickEventHandler = Me.ClickEvent
            If clickEvent IsNot Nothing Then
                clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
            End If
        End Sub

        Private Sub Button_MouseDown(sender As Object, e As MouseButtonEventArgs)
            Me.IsMouseDown = True
        End Sub

        Private Sub Button_MouseLeave(sender As Object, e As Object)
            Me.IsMouseDown = False
        End Sub

        Friend Overridable Property PanBack As MyLoading

        Friend Overridable Property PathPickaxe As Path

        Friend Overridable Property PathLeft As Path

        Friend Overridable Property PathRight As Path

        Friend Overridable Property PathError As Path

        Friend Overridable Property LabText As TextBlock

        Private Uuid As Integer

        Public Shared ForegroundProperty As DependencyProperty = DependencyProperty.Register("Foreground", GetType(SolidColorBrush), GetType(MyLoading))

        Private _Text As String

        Private _TextError As String

        Private IsLooping As Boolean

        Private ErrorAnimationWaiting As Boolean

        Private IsMouseDown As Boolean

        Public Delegate Sub IsErrorChangedEventHandler(sender As Object, isError As Boolean)

        Public Delegate Sub StateChangedEventHandler(sender As Object, newState As MyLoading.MyLoadingState, oldState As MyLoading.MyLoadingState)

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)

        Public Enum MyLoadingState
            Unloaded = -1
            Run
            [Stop]
            [Error]
        End Enum
    End Class
End Namespace
