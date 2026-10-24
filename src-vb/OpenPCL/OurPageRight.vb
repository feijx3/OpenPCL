Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class OurPageRight
        Inherits AdornerDecorator

        Public Sub New()
            Me.PageUuid = ModBase.GetUuid()
            Me._PageState = OurPageRight.PageStates.Empty
            Me.DisabledPageAnimControls = New List(Of FrameworkElement)()
        End Sub

        Public Property PanScroll As String
            Get
                Return Conversions.ToString(MyBase.GetValue(CType(OurPageRight.PanScrollProperty, DependencyProperty)))
            End Get
            Set(value As String)
                Dim type As Type = Nothing
                Dim text As String = "SetValue"
                Dim array As Object() = New Object() { OurPageRight.PanScrollProperty, value }
                Dim array2 As Object() = array
                Dim array3 As String() = Nothing
                Dim array4 As Type() = Nothing
                Dim array5 As Boolean() = New Boolean() { Nothing, True }
                Dim array6 As Boolean() = array5
                NewLateBinding.LateCall(Me, type, text, array, array3, array4, array5, True)
                If array6(1) Then
                    value = CStr(Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2(1)), GetType(String)))
                End If
            End Set
        End Property

        Public Property PageState As OurPageRight.PageStates
            Get
                Return Me._PageState
            End Get
            Set(value As OurPageRight.PageStates)
                If Me._PageState <> value Then
                    Me._PageState = value
                    Logger.Trace(String.Format("页面状态切换为 {0}", value), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
                End If
            End Set
        End Property

        Public Sub PageLoaderInit(Of TIn, TOut)(LoaderUi As OurLoading, PanLoader As FrameworkElement, PanContent As FrameworkElement, PanAlways As FrameworkElement, RealLoader As ModLoader.LoaderTask(Of TIn, TOut), Optional FinishedInvoke As Action(Of ModLoader.LoaderTask(Of TIn, TOut)) = Nothing, Optional InputInvoke As Func(Of TIn) = Nothing, Optional AutoRun As Boolean = True)
            Dim CS$<>8__locals1 As OurPageRight._Closure$__19-0(Of TIn, TOut) = New OurPageRight._Closure$__19-0(Of TIn, TOut)(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_RealLoader = RealLoader
            CS$<>8__locals1.$VB$Local_FinishedInvoke = FinishedInvoke
            Me.PanLoader = PanLoader
            Me.PanContent = PanContent
            Me.PanAlways = PanAlways
            Me.PageLoader = CS$<>8__locals1.$VB$Local_RealLoader
            Me.PageLoaderUi = LoaderUi
            Me.PageLoaderInputInvoke = InputInvoke
            Me.PageLoaderAutoRun = AutoRun
            If CS$<>8__locals1.$VB$Local_FinishedInvoke IsNot Nothing Then
                AddHandler CS$<>8__locals1.$VB$Local_RealLoader.PreviewFinish, Sub(a0 As ModLoader.LoaderBase)
                    MyBase._Lambda$__0()
                End Sub
            End If
            AddHandler CS$<>8__locals1.$VB$Local_RealLoader.OnStateChangedUi, Sub(Loader As ModLoader.LoaderBase, NewState As ModBase.LoadState, OldState As ModBase.LoadState)
                Dim CS$<>8__locals2 As OurPageRight._Closure$__19-1(Of $CLS0, $CLS1) = New OurPageRight._Closure$__19-1(Of $CLS0, $CLS1)(CS$<>8__locals2)
                CS$<>8__locals2.$VB$Me = Me
                CS$<>8__locals2.$VB$Local_Loader = Loader
                CS$<>8__locals2.$VB$Local_NewState = NewState
                CS$<>8__locals2.$VB$Local_OldState = OldState
                ModBase.RunInUi(Sub()
                    CS$<>8__locals2.$VB$Me.PageLoaderState(CS$<>8__locals2.$VB$Local_Loader, CS$<>8__locals2.$VB$Local_NewState, CS$<>8__locals2.$VB$Local_OldState)
                End Sub, False)
            End Sub
            PanLoader.Visibility = Visibility.Collapsed
            PanContent.Visibility = Visibility.Collapsed
            If PanAlways IsNot Nothing Then
                PanAlways.Visibility = Visibility.Collapsed
            End If
            If Me.PageLoaderAutoRun Then
                Me.PageLoader.Start(CS$<>8__locals1.$VB$Local_RealLoader.StartGetInput(Nothing, CType(Me.PageLoaderInputInvoke, Func(Of TIn))), False)
            End If
            If Me.PageLoader.State = ModBase.LoadState.Finished AndAlso CS$<>8__locals1.$VB$Local_FinishedInvoke IsNot Nothing Then
                ModBase.RunInUiWait(Sub()
                    CS$<>8__locals1.$VB$Local_FinishedInvoke(CS$<>8__locals1.$VB$Local_RealLoader)
                End Sub)
            End If
            Me.PageLoaderUi.State = CS$<>8__locals1.$VB$Local_RealLoader
            AddHandler Me.PageLoaderUi.Click, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                MyBase._Lambda$__5()
            End Sub
        End Sub

        Public Sub PageLoaderInit(Of TIn)(LoaderUi As OurLoading, PanLoader As FrameworkElement, PanContent As FrameworkElement, PanAlways As FrameworkElement, RealLoader As ModLoader.LoaderCombo(Of TIn), Optional FinishedInvoke As Action(Of ModLoader.LoaderCombo(Of TIn)) = Nothing, Optional InputInvoke As Func(Of TIn) = Nothing, Optional AutoRun As Boolean = True)
            Dim CS$<>8__locals1 As OurPageRight._Closure$__20-0(Of TIn) = New OurPageRight._Closure$__20-0(Of TIn)(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_RealLoader = RealLoader
            CS$<>8__locals1.$VB$Local_FinishedInvoke = FinishedInvoke
            Me.PanLoader = PanLoader
            Me.PanContent = PanContent
            Me.PanAlways = PanAlways
            Me.PageLoader = CS$<>8__locals1.$VB$Local_RealLoader
            Me.PageLoaderUi = LoaderUi
            Me.PageLoaderInputInvoke = InputInvoke
            Me.PageLoaderAutoRun = AutoRun
            If CS$<>8__locals1.$VB$Local_FinishedInvoke IsNot Nothing Then
                AddHandler CS$<>8__locals1.$VB$Local_RealLoader.PreviewFinish, Sub(a0 As ModLoader.LoaderBase)
                    MyBase._Lambda$__0()
                End Sub
            End If
            AddHandler CS$<>8__locals1.$VB$Local_RealLoader.OnStateChangedUi, Sub(Loader As ModLoader.LoaderBase, NewState As ModBase.LoadState, OldState As ModBase.LoadState)
                Dim CS$<>8__locals2 As OurPageRight._Closure$__20-1(Of $CLS0) = New OurPageRight._Closure$__20-1(Of $CLS0)(CS$<>8__locals2)
                CS$<>8__locals2.$VB$Me = Me
                CS$<>8__locals2.$VB$Local_Loader = Loader
                CS$<>8__locals2.$VB$Local_NewState = NewState
                CS$<>8__locals2.$VB$Local_OldState = OldState
                ModBase.RunInUi(Sub()
                    CS$<>8__locals2.$VB$Me.PageLoaderState(CS$<>8__locals2.$VB$Local_Loader, CS$<>8__locals2.$VB$Local_NewState, CS$<>8__locals2.$VB$Local_OldState)
                End Sub, False)
            End Sub
            PanLoader.Visibility = Visibility.Collapsed
            PanContent.Visibility = Visibility.Collapsed
            If PanAlways IsNot Nothing Then
                PanAlways.Visibility = Visibility.Collapsed
            End If
            If Me.PageLoaderAutoRun Then
                Dim obj As Object = Nothing
                If Me.PageLoaderInputInvoke IsNot Nothing Then
                    obj = RuntimeHelpers.GetObjectValue(NewLateBinding.LateIndexGet(Me.PageLoaderInputInvoke, New Object(-1) {}, Nothing))
                End If
                Me.PageLoader.Start(RuntimeHelpers.GetObjectValue(obj), False)
            End If
            If Me.PageLoader.State = ModBase.LoadState.Finished AndAlso CS$<>8__locals1.$VB$Local_FinishedInvoke IsNot Nothing Then
                ModBase.RunInUiWait(Sub()
                    CS$<>8__locals1.$VB$Local_FinishedInvoke(CS$<>8__locals1.$VB$Local_RealLoader)
                End Sub)
            End If
            Me.PageLoaderUi.State = CS$<>8__locals1.$VB$Local_RealLoader
            AddHandler Me.PageLoaderUi.Click, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                MyBase._Lambda$__5()
            End Sub
        End Sub

        Public Sub PageLoaderRestart(Optional Input As Object = Nothing, Optional IsForceRestart As Boolean = True)
            If Me.PageLoaderAutoRun Then
                If GeneralUtils.IsGenericInstanceOf(Me.PageLoader, GetType(ModLoader.LoaderTask(Of, ))) OrElse TypeOf Me.PageLoader Is ModLoader.LoaderWorker Then
                    Dim pageLoader As ModLoader.LoaderBase = Me.PageLoader
                    Dim pageLoader2 As Object = Me.PageLoader
                    Dim type As Type = Nothing
                    Dim text As String = "StartGetInput"
                    Dim array As Object() = New Object(1) {}
                    array(0) = Input
                    Dim num As Integer = 1
                    Dim ptr As __ByRef(Of Object) = Me.PageLoaderInputInvoke
                    array(num) = Me.PageLoaderInputInvoke
                    Dim array2 As Object() = array
                    Dim array3 As String() = Nothing
                    Dim array4 As Type() = Nothing
                    Dim array5 As Boolean() = New Boolean() { True, True }
                    Dim array6 As Boolean() = array5
                    Dim obj As Object = NewLateBinding.LateGet(pageLoader2, type, text, array, array3, array4, array5)
                    If array6(0) Then
                        Input = RuntimeHelpers.GetObjectValue(array2(0))
                    End If
                    If array6(1) Then
                        ptr = RuntimeHelpers.GetObjectValue(array2(1))
                    End If
                    pageLoader.Start(RuntimeHelpers.GetObjectValue(obj), IsForceRestart)
                    Return
                End If
                If Input Is Nothing AndAlso Me.PageLoaderInputInvoke IsNot Nothing Then
                    Input = RuntimeHelpers.GetObjectValue(NewLateBinding.LateIndexGet(Me.PageLoaderInputInvoke, New Object(-1) {}, Nothing))
                End If
                Me.PageLoader.Start(RuntimeHelpers.GetObjectValue(Input), IsForceRestart)
            End If
        End Sub

        Public Sub PageOnEnter()
            Logger.Trace("已触发 PageOnEnter", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
            Dim pageEnterEvent As OurPageRight.PageEnterEventHandler = Me.PageEnterEvent
            If pageEnterEvent IsNot Nothing Then
                pageEnterEvent()
            End If
            Dim pageState As OurPageRight.PageStates = Me.PageState
            If pageState <> OurPageRight.PageStates.Empty Then
                If pageState = OurPageRight.PageStates.ContentEnter Then
                    Return
                End If
                If pageState <> OurPageRight.PageStates.ContentExit Then
                    Throw New Exception("在状态为 " + Me.PageState.ToString() + " 时触发了 PageOnEnter 事件。")
                End If
                If Me.PageLoader Is Nothing OrElse Me.PageLoader.State = ModBase.LoadState.Finished OrElse Me.PageLoader.State = ModBase.LoadState.Waiting OrElse Me.PageLoader.State = ModBase.LoadState.Canceled Then
                    Me.PageState = OurPageRight.PageStates.ContentEnter
                    Me.TriggerEnterAnimation(New FrameworkElement() { CType((If(Me.PanContent, Me.Child)), FrameworkElement) })
                    Return
                End If
                If Me.PageLoader.State = ModBase.LoadState.Loading Then
                    Me.PageState = OurPageRight.PageStates.LoaderWait
                    ModAnimation.AniStart(ModAnimation.AaCode(AddressOf Me.PageOnLoaderWaitFinished, 400, False), "PageRight PageChange " + Conversions.ToString(Me.PageUuid), False)
                    Return
                End If
                Me.PageState = OurPageRight.PageStates.LoaderEnter
                Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanLoader })
                Return
            Else
                If Me.PageLoader Is Nothing OrElse Me.PageLoader.State = ModBase.LoadState.Finished OrElse Me.PageLoader.State = ModBase.LoadState.Waiting OrElse Me.PageLoader.State = ModBase.LoadState.Canceled Then
                    Me.PageState = OurPageRight.PageStates.ContentEnter
                    Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanAlways, CType((If(Me.PanContent, Me.Child)), FrameworkElement) })
                    Return
                End If
                If Me.PageLoader.State = ModBase.LoadState.Loading Then
                    Me.PageState = OurPageRight.PageStates.LoaderWait
                    ModAnimation.AniStart(ModAnimation.AaCode(AddressOf Me.PageOnLoaderWaitFinished, 400, False), "PageRight PageChange " + Conversions.ToString(Me.PageUuid), False)
                    Return
                End If
                Me.PageState = OurPageRight.PageStates.LoaderEnter
                Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanAlways, Me.PanLoader })
                Return
            End If
        End Sub

        Public Event PageEnter As OurPageRight.PageEnterEventHandler

        Public Sub PageOnExit()
            Logger.Trace("已触发 PageOnExit", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
            Dim pageExitEvent As OurPageRight.PageExitEventHandler = Me.PageExitEvent
            If pageExitEvent IsNot Nothing Then
                pageExitEvent()
            End If
            Select Case Me.PageState
                Case OurPageRight.PageStates.Empty, OurPageRight.PageStates.PageExit
                Case OurPageRight.PageStates.LoaderWait
                    Me.PageState = OurPageRight.PageStates.PageExit
                    Me.TriggerExitAnimation(New FrameworkElement() { Me.PanAlways })
                    Return
                Case OurPageRight.PageStates.LoaderEnter, OurPageRight.PageStates.LoaderStayForce, OurPageRight.PageStates.LoaderStay
                    Me.PageState = OurPageRight.PageStates.PageExit
                    Me.TriggerExitAnimation(New FrameworkElement() { Me.PanAlways, Me.PanLoader })
                    Return
                Case OurPageRight.PageStates.LoaderExit, OurPageRight.PageStates.ContentExit
                    Me.PageState = OurPageRight.PageStates.PageExit
                    If Me.PanAlways IsNot Nothing Then
                        Me.TriggerExitAnimation(New FrameworkElement() { Me.PanAlways, CType((If(Me.PanContent, Me.Child)), FrameworkElement) })
                    End If
                Case OurPageRight.PageStates.ContentEnter, OurPageRight.PageStates.ContentStay
                    Me.PageState = OurPageRight.PageStates.PageExit
                    Me.TriggerExitAnimation(New FrameworkElement() { Me.PanAlways, CType((If(Me.PanContent, Me.Child)), FrameworkElement) })
                    Return
                Case Else
                    Return
            End Select
        End Sub

        Public Event PageExit As OurPageRight.PageExitEventHandler

        Public Sub PageOnForceExit()
            If Me.PageState <> OurPageRight.PageStates.Empty Then
                Logger.Trace("已触发 PageOnForceExit", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
                Me.PageState = OurPageRight.PageStates.Empty
                ModAnimation.AniStop("PageRight PageChange " + Conversions.ToString(Me.PageUuid))
                If Me.PageLoader Is Nothing Then
                    Me.Child.Visibility = Visibility.Collapsed
                    Return
                End If
                Me.PanContent.Visibility = Visibility.Collapsed
                Me.PanLoader.Visibility = Visibility.Collapsed
                If Me.PanAlways IsNot Nothing Then
                    Me.PanAlways.Visibility = Visibility.Collapsed
                End If
            End If
        End Sub

        Public Sub PageOnContentExit()
            Logger.Trace("已触发 PageOnContentExit", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
            If Me.PageLoader IsNot Nothing AndAlso Me.PageLoader.State = ModBase.LoadState.Loading Then
                Throw New Exception("在调用 PageOnContentExit 时，加载器不能为 Loading 状态")
            End If
            Select Case Me.PageState
                Case OurPageRight.PageStates.Empty
                    If MyBase.IsLoaded Then
                        Me.PageOnEnter()
                    End If
                    Return
                Case OurPageRight.PageStates.LoaderWait
                    Me.PageOnEnter()
                    Return
                Case OurPageRight.PageStates.LoaderEnter, OurPageRight.PageStates.LoaderStayForce, OurPageRight.PageStates.LoaderStay
                    Me.PageState = OurPageRight.PageStates.ContentExit
                    Me.TriggerExitAnimation(New FrameworkElement() { Me.PanLoader })
                    Return
                Case OurPageRight.PageStates.LoaderExit
                    Me.PageState = OurPageRight.PageStates.ContentExit
                    Return
                Case OurPageRight.PageStates.ContentEnter, OurPageRight.PageStates.ContentStay
                    Me.PageState = OurPageRight.PageStates.ContentExit
                    Me.TriggerExitAnimation(New FrameworkElement() { Me.PanContent })
                    Return
                Case Else
                    Return
            End Select
        End Sub

        Private Sub PageOnEnterAnimationFinished()
            Logger.Trace("已触发 PageOnEnterAnimationFinished", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
            Dim pageState As OurPageRight.PageStates = Me.PageState
            If pageState = OurPageRight.PageStates.LoaderEnter Then
                Me.PageState = OurPageRight.PageStates.LoaderStayForce
                ModAnimation.AniStart(ModAnimation.AaCode(AddressOf Me.PageOnLoaderStayFinished, 400, False), "PageRight PageChange " + Conversions.ToString(Me.PageUuid), False)
                Return
            End If
            If pageState = OurPageRight.PageStates.ContentEnter Then
                Me.PageState = OurPageRight.PageStates.ContentStay
                Return
            End If
            Throw New Exception("在状态为 " + Me.PageState.ToString() + " 时触发了 PageOnEnterAnimationFinished 事件。")
        End Sub

        Private Sub PageOnExitAnimationFinished()
            Logger.Trace("已触发 PageOnExitAnimationFinished", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
            Select Case Me.PageState
                Case OurPageRight.PageStates.LoaderExit
                    Me.PageState = OurPageRight.PageStates.ContentEnter
                    Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanContent })
                    Return
                Case OurPageRight.PageStates.ContentExit
                    Me.PageOnEnter()
                    Return
                Case OurPageRight.PageStates.PageExit
                    Me.PageState = OurPageRight.PageStates.Empty
                    Return
            End Select
            Throw New Exception("在状态为 " + Me.PageState.ToString() + " 时触发了 PageOnExitAnimationFinished 事件。")
        End Sub

        Private Sub PageOnLoaderWaitFinished()
            Logger.Trace("已触发 PageOnLoaderWaitFinished", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
            Dim pageState As OurPageRight.PageStates = Me.PageState
            If pageState <> OurPageRight.PageStates.LoaderWait Then
                Throw New Exception("在状态为 " + Me.PageState.ToString() + " 时触发了 PageOnLoaderWaitFinished 事件。")
            End If
            Me.PageState = OurPageRight.PageStates.LoaderEnter
            If Me.PanAlways IsNot Nothing AndAlso Me.PanAlways.Visibility = Visibility.Collapsed Then
                Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanAlways, Me.PanLoader })
                Return
            End If
            Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanLoader })
        End Sub

        Private Sub PageOnLoaderStayFinished()
            Logger.Trace("已触发 PageOnLoaderStayFinished", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
            Dim pageState As OurPageRight.PageStates = Me.PageState
            If pageState <> OurPageRight.PageStates.LoaderStayForce Then
                Throw New Exception("在状态为 " + Me.PageState.ToString() + " 时触发了 PageOnLoaderWaitFinished 事件。")
            End If
            If Me.PageLoader.State = ModBase.LoadState.Finished Then
                Me.PageState = OurPageRight.PageStates.LoaderExit
                Me.TriggerExitAnimation(New FrameworkElement() { Me.PanLoader })
                Return
            End If
            Me.PageState = OurPageRight.PageStates.LoaderStay
        End Sub

        Private Sub PageLoaderState(sender As Object, NewState As ModBase.LoadState, OldState As ModBase.LoadState)
            Select Case NewState
                Case ModBase.LoadState.Waiting, ModBase.LoadState.Finished, ModBase.LoadState.Canceled
                    If OldState = ModBase.LoadState.Failed OrElse OldState = ModBase.LoadState.Loading Then
                        Logger.Trace("已触发 PageLoaderState (Stop/Canceled)", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
                        Dim pageState As OurPageRight.PageStates = Me.PageState
                        If pageState <> OurPageRight.PageStates.LoaderWait Then
                            If pageState <> OurPageRight.PageStates.LoaderStay Then
                                Return
                            End If
                            Me.PageState = OurPageRight.PageStates.LoaderExit
                            Me.TriggerExitAnimation(New FrameworkElement() { Me.PanLoader })
                        Else
                            Me.PageState = OurPageRight.PageStates.ContentEnter
                            If Me.PanAlways IsNot Nothing AndAlso Me.PanAlways.Visibility = Visibility.Collapsed Then
                                Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanAlways, Me.PanContent })
                                Return
                            End If
                            Me.TriggerEnterAnimation(New FrameworkElement() { Me.PanContent })
                            Return
                        End If
                    End If
                Case ModBase.LoadState.Loading, ModBase.LoadState.Failed
                    If OldState <> ModBase.LoadState.Failed AndAlso OldState <> ModBase.LoadState.Loading Then
                        Logger.Trace("已触发 PageLoaderState (Start/Refresh)", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurPageRight.vb")
                        Dim pageState2 As OurPageRight.PageStates = Me.PageState
                        If pageState2 = OurPageRight.PageStates.LoaderExit Then
                            Me.PageState = OurPageRight.PageStates.ContentExit
                            Return
                        End If
                        If pageState2 = OurPageRight.PageStates.ContentEnter OrElse pageState2 = OurPageRight.PageStates.ContentStay Then
                            Me.PageState = OurPageRight.PageStates.ContentExit
                            Me.TriggerExitAnimation(New FrameworkElement() { Me.PanContent })
                            Return
                        End If
                    End If
                Case Else
                    Return
            End Select
        End Sub

        Public Sub TriggerEnterAnimation(ParamArray Elements As FrameworkElement())
            Dim func As Func(Of FrameworkElement, Boolean)
            If OurPageRight._Closure$__.$I41-0 IsNot Nothing Then
                func = OurPageRight._Closure$__.$I41-0
            Else
                Dim func2 As Func(Of FrameworkElement, Boolean) = Function(e As FrameworkElement) e IsNot Nothing
                func = func2
                OurPageRight._Closure$__.$I41-0 = func2
            End If
            Dim enumerable As IEnumerable(Of FrameworkElement) = Elements.Where(func)
            Try
                For Each frameworkElement As FrameworkElement In enumerable
                    frameworkElement.Visibility = Visibility.Visible
                Next
            Finally
                Dim enumerator As IEnumerator(Of FrameworkElement)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
            Dim num As Integer = 0
            Dim firstScrollViewer As OurScrollBar
            Try
                For Each frameworkElement2 As FrameworkElement In enumerable
                    Try
                        For Each frameworkElement3 As FrameworkElement In Me.GetAllAnimControls(frameworkElement2, True)
                            frameworkElement3.IsHitTestVisible = True
                            If frameworkElement3.RenderTransform IsNot Nothing AndAlso TypeOf frameworkElement3.RenderTransform Is TranslateTransform Then
                                frameworkElement3.RenderTransform = Nothing
                            End If
                        Next
                    Finally
                        Dim enumerator3 As IEnumerator(Of FrameworkElement)
                        If enumerator3 IsNot Nothing Then
                            enumerator3.Dispose()
                        End If
                    End Try
                    Try
                        For Each frameworkElement4 As FrameworkElement In Me.GetAllAnimControls(frameworkElement2, False)
                            If TypeOf frameworkElement4 Is OurExtraTextButton Then
                                CType(frameworkElement4, OurExtraTextButton).Show = True
                            Else
                                frameworkElement4.Opacity = 0.0
                                frameworkElement4.RenderTransform = New TranslateTransform(0.0, -16.0)
                                list.Add(ModAnimation.AaOpacity(frameworkElement4, 1.0, 100, num, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False))
                                list.Add(ModAnimation.AaTranslateY(frameworkElement4, 5.0, 250, num, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                                list.Add(ModAnimation.AaTranslateY(frameworkElement4, 11.0, 350, num, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False))
                                num += 25
                            End If
                        Next
                    Finally
                        Dim enumerator4 As IEnumerator(Of FrameworkElement)
                        If enumerator4 IsNot Nothing Then
                            enumerator4.Dispose()
                        End If
                    End Try
                Next
            Finally
                Dim enumerator2 As IEnumerator(Of FrameworkElement)
                If enumerator2 IsNot Nothing Then
                    enumerator2.Dispose()
                End If
            End Try
            firstScrollViewer = Me.GetFirstScrollViewer(enumerable)
            If firstScrollViewer IsNot Nothing Then
                If Not(TypeOf firstScrollViewer.RenderTransform Is TranslateTransform) Then
                    firstScrollViewer.RenderTransform = New TranslateTransform(10.0, 0.0)
                End If
                list.Add(ModAnimation.AaTranslateX(firstScrollViewer, -CType(firstScrollViewer.RenderTransform, TranslateTransform).X, 350, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
            End If
            list.Add(ModAnimation.AaCode(Sub()
                Me.PageOnEnterAnimationFinished()
            End Sub, 0, True))
            ModAnimation.AniStart(list, "PageRight PageChange " + Conversions.ToString(Me.PageUuid), True)
        End Sub

        Public Sub TriggerExitAnimation(ParamArray Elements As FrameworkElement())
            Dim CS$<>8__locals1 As OurPageRight._Closure$__42-0 = New OurPageRight._Closure$__42-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            Dim CS$<>8__locals2 As OurPageRight._Closure$__42-0 = CS$<>8__locals1
            Dim func As Func(Of FrameworkElement, Boolean)
            If OurPageRight._Closure$__.$I42-0 IsNot Nothing Then
                func = OurPageRight._Closure$__.$I42-0
            Else
                Dim func2 As Func(Of FrameworkElement, Boolean) = Function(e As FrameworkElement) e IsNot Nothing
                func = func2
                OurPageRight._Closure$__.$I42-0 = func2
            End If
            CS$<>8__locals2.$VB$Local_RealElements = Elements.Where(func)
            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
            Dim num As Integer = 0
            Dim firstScrollViewer As OurScrollBar
            Try
                For Each frameworkElement As FrameworkElement In CS$<>8__locals1.$VB$Local_RealElements
                    Try
                        For Each frameworkElement2 As FrameworkElement In Me.GetAllAnimControls(frameworkElement, False)
                            If TypeOf frameworkElement2 Is OurExtraTextButton Then
                                CType(frameworkElement2, OurExtraTextButton).Show = False
                            Else
                                frameworkElement2.IsHitTestVisible = False
                                list.Add(ModAnimation.AaOpacity(frameworkElement2, -1.0, 70, num, Nothing, False))
                                list.Add(ModAnimation.AaTranslateY(frameworkElement2, -6.0, 70, num, Nothing, False))
                                num += 15
                            End If
                        Next
                    Finally
                        Dim enumerator2 As IEnumerator(Of FrameworkElement)
                        If enumerator2 IsNot Nothing Then
                            enumerator2.Dispose()
                        End If
                    End Try
                Next
            Finally
                Dim enumerator As IEnumerator(Of FrameworkElement)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            firstScrollViewer = Me.GetFirstScrollViewer(CS$<>8__locals1.$VB$Local_RealElements)
            If firstScrollViewer IsNot Nothing Then
                If Not(TypeOf firstScrollViewer.RenderTransform Is TranslateTransform) Then
                    firstScrollViewer.RenderTransform = New TranslateTransform()
                End If
                list.Add(ModAnimation.AaTranslateX(firstScrollViewer, 10.0 - CType(firstScrollViewer.RenderTransform, TranslateTransform).X, 90, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False))
            End If
            list.Add(ModAnimation.AaCode(Sub()
                Try
                    For Each frameworkElement3 As FrameworkElement In CS$<>8__locals1.$VB$Local_RealElements
                        frameworkElement3.Visibility = Visibility.Collapsed
                    Next
                Finally
                    Dim enumerator3 As IEnumerator(Of FrameworkElement)
                    If enumerator3 IsNot Nothing Then
                        enumerator3.Dispose()
                    End If
                End Try
                CS$<>8__locals1.$VB$Me.PageOnExitAnimationFinished()
            End Sub, 0, True))
            ModAnimation.AniStart(list, "PageRight PageChange " + Conversions.ToString(Me.PageUuid), False)
        End Sub

        Friend Function GetAllAnimControls(Element As FrameworkElement, Optional IgnoreInvisibility As Boolean = False) As IEnumerable(Of FrameworkElement)
            Dim list As List(Of FrameworkElement) = New List(Of FrameworkElement)()
            Me._GetAllAnimControls(Element, list, IgnoreInvisibility)
            Return list.Except(Me.DisabledPageAnimControls)
        End Function

        Private Sub _GetAllAnimControls(Element As FrameworkElement, ByRef AllControls As List(Of FrameworkElement), IgnoreInvisibility As Boolean)
            If IgnoreInvisibility OrElse Element.Visibility <> Visibility.Collapsed Then
                If TypeOf Element Is OurCard OrElse TypeOf Element Is OurHint OrElse TypeOf Element Is OurExtraTextButton OrElse TypeOf Element Is TextBlock OrElse TypeOf Element Is OurTextButton Then
                    AllControls.Add(Element)
                    Return
                End If
                If TypeOf Element Is ContentControl Then
                    Dim objectValue As Object = RuntimeHelpers.GetObjectValue(CType(Element, ContentControl).Content)
                    If objectValue IsNot Nothing AndAlso TypeOf objectValue Is FrameworkElement Then
                        Me._GetAllAnimControls(CType(objectValue, FrameworkElement), AllControls, IgnoreInvisibility)
                        Return
                    End If
                ElseIf TypeOf Element Is Panel Then
                    Try
                        For Each obj As Object In CType(Element, Panel).Children
                            Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(obj)
                            If TypeOf objectValue2 Is FrameworkElement Then
                                Me._GetAllAnimControls(CType(objectValue2, FrameworkElement), AllControls, IgnoreInvisibility)
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator
                        If TypeOf enumerator Is IDisposable Then
                            TryCast(enumerator, IDisposable).Dispose()
                        End If
                    End Try
                End If
            End If
        End Sub

        Private Function GetFirstScrollViewer(Elements As IEnumerable(Of FrameworkElement)) As OurScrollBar
            Dim myScrollViewer As OurScrollViewer = Nothing
            Try
                For Each frameworkElement As FrameworkElement In Elements
                    If TypeOf frameworkElement Is OurScrollViewer Then
                        myScrollViewer = CType(frameworkElement, OurScrollViewer)
                        GoTo IL_008D
                    End If
                    Try
                        For Each obj As Object In LogicalTreeHelper.GetChildren(frameworkElement)
                            Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                            If TypeOf objectValue Is OurScrollViewer Then
                                myScrollViewer = CType(objectValue, OurScrollViewer)
                                GoTo IL_008D
                            End If
                        Next
                    Finally
                        Dim enumerator2 As IEnumerator
                        If TypeOf enumerator2 Is IDisposable Then
                            TryCast(enumerator2, IDisposable).Dispose()
                        End If
                    End Try
                Next
            Finally
                Dim enumerator As IEnumerator(Of FrameworkElement)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Return Nothing
            IL_008D:
            Dim myScrollBar As OurScrollBar
            If myScrollViewer.ComputedVerticalScrollBarVisibility <> Visibility.Visible Then
                myScrollBar = Nothing
            Else
                myScrollBar = myScrollViewer.ScrollBar
            End If
            Return myScrollBar
        End Function

        Public PageUuid As Integer

        Private Shared PanScrollProperty As Object = DependencyProperty.Register("PanScroll", GetType(String), GetType(OurPageRight), New PropertyMetadata(Nothing))

        Private _PageState As OurPageRight.PageStates

        Private PageLoader As ModLoader.LoaderBase

        Private PageLoaderInputInvoke As Object

        Private PageLoaderUi As OurLoading

        Private PanLoader As FrameworkElement

        Private PanContent As FrameworkElement

        Private PanAlways As FrameworkElement

        Private PageLoaderAutoRun As Boolean

        Public DisabledPageAnimControls As List(Of FrameworkElement)

        Public Enum PageStates
            Empty
            LoaderWait
            LoaderEnter
            LoaderStayForce
            LoaderStay
            LoaderExit
            ContentEnter
            ContentStay
            ContentExit
            PageExit
        End Enum

        Public Delegate Sub PageEnterEventHandler()

        Public Delegate Sub PageExitEventHandler()
    End Class
End Namespace
