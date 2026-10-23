Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public NotInheritable Module ModAnimation
        Public Sub AniDispose(Control As MyCard, RemoveFromChildren As Boolean, Optional CallBack As ParameterizedThreadStart = Nothing)
            If Control.IsHitTestVisible Then
                Control.IsHitTestVisible = False
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Control, -0.08, 200, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Control, -1.0, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Control, -Control.ActualHeight, 150, 100, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                    If RemoveFromChildren Then
                        If Control.Parent Is Nothing Then
                            Return
                        End If
                        Dim obj2 As Object = NewLateBinding.LateGet(Control.Parent, Nothing, "Children", New Object(-1) {}, Nothing, Nothing, Nothing)
                        Dim type2 As Type = Nothing
                        Dim text2 As String = "Remove"
                        Dim array7 As Object() = New Object() { Control }
                        Dim array8 As Object() = array7
                        Dim array9 As String() = Nothing
                        Dim array10 As Type() = Nothing
                        Dim array11 As Boolean() = New Boolean() { True }
                        Dim array12 As Boolean() = array11
                        NewLateBinding.LateCall(obj2, type2, text2, array7, array9, array10, array11, True)
                        If array12(0) Then
                            Control = CType(Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array8(0)), GetType(MyCard)), MyCard)
                        End If
                    Else
                        Control.Visibility = Visibility.Collapsed
                    End If
                    If CallBack IsNot Nothing Then
                        CallBack(Control)
                    End If
                End Sub, 0, True) }, "MyCard Dispose " + Conversions.ToString(Control.Uuid), False)
                Return
            End If
            If RemoveFromChildren Then
                If Control.Parent Is Nothing Then
                    Return
                End If
                Dim obj As Object = NewLateBinding.LateGet(Control.Parent, Nothing, "Children", New Object(-1) {}, Nothing, Nothing, Nothing)
                Dim type As Type = Nothing
                Dim text As String = "Remove"
                Dim array As Object() = New Object() { Control }
                Dim array2 As Object() = array
                Dim array3 As String() = Nothing
                Dim array4 As Type() = Nothing
                Dim array5 As Boolean() = New Boolean() { True }
                Dim array6 As Boolean() = array5
                NewLateBinding.LateCall(obj, type, text, array, array3, array4, array5, True)
                If array6(0) Then
                    Control = CType(Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2(0)), GetType(MyCard)), MyCard)
                End If
            Else
                Control.Visibility = Visibility.Collapsed
            End If
            If CallBack IsNot Nothing Then
                CallBack(Control)
            End If
        End Sub

        Public Sub AniDispose(Control As MyHint, RemoveFromChildren As Boolean, Optional CallBack As ParameterizedThreadStart = Nothing)
            If Control.IsHitTestVisible Then
                Control.IsHitTestVisible = False
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Control, -0.08, 200, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Control, -1.0, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Control, -Control.ActualHeight, 150, 100, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                    If RemoveFromChildren Then
                        Dim obj As Object = NewLateBinding.LateGet(Control.Parent, Nothing, "Children", New Object(-1) {}, Nothing, Nothing, Nothing)
                        Dim type As Type = Nothing
                        Dim text As String = "Remove"
                        Dim array As Object() = New Object() { Control }
                        Dim array2 As Object() = array
                        Dim array3 As String() = Nothing
                        Dim array4 As Type() = Nothing
                        Dim array5 As Boolean() = New Boolean() { True }
                        Dim array6 As Boolean() = array5
                        NewLateBinding.LateCall(obj, type, text, array, array3, array4, array5, True)
                        If array6(0) Then
                            Control = CType(Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2(0)), GetType(MyHint)), MyHint)
                        End If
                    Else
                        Control.Visibility = Visibility.Collapsed
                    End If
                    If CallBack IsNot Nothing Then
                        CallBack(Control)
                    End If
                End Sub, 0, True) }, "MyCard Dispose " + Conversions.ToString(Control.Uuid), False)
            End If
        End Sub

        Public Sub AniDispose(Control As MyIconButton, RemoveFromChildren As Boolean, Optional CallBack As ParameterizedThreadStart = Nothing)
            If Control.IsHitTestVisible Then
                Control.IsHitTestVisible = False
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScaleTransform(Control, -1.5, 200, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                    If RemoveFromChildren Then
                        Dim obj As Object = NewLateBinding.LateGet(Control.Parent, Nothing, "Children", New Object(-1) {}, Nothing, Nothing, Nothing)
                        Dim type As Type = Nothing
                        Dim text As String = "Remove"
                        Dim array As Object() = New Object() { Control }
                        Dim array2 As Object() = array
                        Dim array3 As String() = Nothing
                        Dim array4 As Type() = Nothing
                        Dim array5 As Boolean() = New Boolean() { True }
                        Dim array6 As Boolean() = array5
                        NewLateBinding.LateCall(obj, type, text, array, array3, array4, array5, True)
                        If array6(0) Then
                            Control = CType(Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array2(0)), GetType(MyIconButton)), MyIconButton)
                        End If
                    Else
                        Control.Visibility = Visibility.Collapsed
                    End If
                    If CallBack IsNot Nothing Then
                        CallBack(Control)
                    End If
                End Sub, 0, True) }, "MyIconButton Dispose " + Conversions.ToString(Control.Uuid), False)
            End If
        End Sub

        Public ReadOnly Property AniSpeed As Double
            Get
                Dim num As Integer = Settings.[Get](Of Integer)("SystemDebugAnim", Nothing)
                If num < 30 Then
                    Return(CDbl(num) * 0.1 + 0.1).Clamp(0.1, 3.0)
                End If
                Return 200.0
            End Get
        End Property

        Public Property AniControlEnabled As Integer
            Get
                Return ModAnimation._AniControlEnabled
            End Get
            Set(value As Integer)
                Dim aniControlEnabledLock As Object = ModAnimation.AniControlEnabledLock
                ObjectFlowControl.CheckForSyncLockOnValueType(aniControlEnabledLock)
                SyncLock aniControlEnabledLock
                    ModAnimation._AniControlEnabled = value
                End SyncLock
            End Set
        End Property

        Public Function AaX(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.X, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaY(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.Y, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaWidth(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.Width, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaHeight(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.Height, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaOpacity(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.Opacity, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaValue(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.Value, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaRadius(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.Radius, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaBorderThickness(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.BorderThickness, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaStrokeThickness(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.StrokeThickness, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaGridLengthWidth(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.GridLengthWidth, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaDouble(Obj As Object, Prop As DependencyProperty, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.[Double], .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = New Object() { Obj, Prop, "" }, .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaDouble(Lambda As ParameterizedThreadStart, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.DoubleParam, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = Lambda, .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaColor(Obj As FrameworkElement, Prop As DependencyProperty, Value As ModBase.MyColor, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Color, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = New Object() { Obj, Prop, "" }, .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)), .ValueLast = New ModBase.MyColor(0.0, 0.0, 0.0, 0.0) }
        End Function

        Public Function AaColor(Obj As FrameworkElement, Prop As DependencyProperty, Res As String, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Color, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = New Object() { Obj, Prop, Res }, .Value = New ModBase.MyColor(RuntimeHelpers.GetObjectValue(Application.Current.FindResource(Res))) - New ModBase.MyColor(RuntimeHelpers.GetObjectValue(Obj.GetValue(Prop))), .IsAfter = After, .TimeFinished = CDbl((0 - Delay)), .ValueLast = New ModBase.MyColor(0.0, 0.0, 0.0, 0.0) }
        End Function

        Public Function AaScale(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False, Optional Absolute As Boolean = False) As ModAnimation.AniData
            Dim myRect As ModBase.MyRect
            If Absolute Then
                myRect = New ModBase.MyRect(-0.5 * Value, -0.5 * Value, Value, Value)
            Else
                myRect = New ModBase.MyRect(Conversions.ToDouble(Operators.MultiplyObject(Operators.MultiplyObject(-0.5, NewLateBinding.LateGet(Obj, Nothing, "ActualWidth", New Object(-1) {}, Nothing, Nothing, Nothing)), Value)), Conversions.ToDouble(Operators.MultiplyObject(Operators.MultiplyObject(-0.5, NewLateBinding.LateGet(Obj, Nothing, "ActualHeight", New Object(-1) {}, Nothing, Nothing, Nothing)), Value)), Conversions.ToDouble(Operators.MultiplyObject(NewLateBinding.LateGet(Obj, Nothing, "ActualWidth", New Object(-1) {}, Nothing, Nothing, Nothing), Value)), Conversions.ToDouble(Operators.MultiplyObject(NewLateBinding.LateGet(Obj, Nothing, "ActualHeight", New Object(-1) {}, Nothing, Nothing, Nothing), Value)))
            End If
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Scale, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = myRect, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaTextAppear(Obj As Object, Optional Hide As Boolean = False, Optional TimePerText As Boolean = True, Optional Time As Integer = 70, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.TextAppear, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .TimeTotal = If(TimePerText, (Time * If((TypeOf Obj Is TextBlock), NewLateBinding.LateGet(Obj, Nothing, "Text", New Object(-1) {}, Nothing, Nothing, Nothing), NewLateBinding.LateGet(Obj, Nothing, "Context", New Object(-1) {}, Nothing, Nothing, Nothing).ToString()).ToString().Length), Time), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = New Object() { If((TypeOf Obj Is TextBlock), NewLateBinding.LateGet(Obj, Nothing, "Text", New Object(-1) {}, Nothing, Nothing, Nothing), NewLateBinding.LateGet(Obj, Nothing, "Context", New Object(-1) {}, Nothing, Nothing, Nothing).ToString()), Hide }, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaCode(Code As ThreadStart, Optional Delay As Integer = 0, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Code, .TimeTotal = 1, .Value = Code, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaScaleTransform(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.ScaleTransform, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaRotateTransform(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.RotateTransform, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaTranslateX(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.TranslateX, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaTranslateY(Obj As Object, Value As Double, Optional Time As Integer = 400, Optional Delay As Integer = 0, Optional Ease As ModAnimation.AniEase = Nothing, Optional After As Boolean = False) As ModAnimation.AniData
            ' The following expression was wrapped in a checked-expression
            Return New ModAnimation.AniData() With { .TypeMain = ModAnimation.AniType.Number, .TypeSub = ModAnimation.AniTypeSub.TranslateY, .TimeTotal = Time, .Ease = (If(Ease, New ModAnimation.AniEaseLinear())), .Obj = RuntimeHelpers.GetObjectValue(Obj), .Value = Value, .IsAfter = After, .TimeFinished = CDbl((0 - Delay)) }
        End Function

        Public Function AaStack(Stack As StackPanel, Optional Time As Integer = 100, Optional Delay As Integer = 25) As List(Of ModAnimation.AniData)
            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
            Dim num As Integer = 0
            Try
                For Each obj As Object In Stack.Children
                    Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                    NewLateBinding.LateSet(objectValue, Nothing, "Opacity", New Object() { 0 }, Nothing, Nothing)
                    list.Add(ModAnimation.AaOpacity(RuntimeHelpers.GetObjectValue(objectValue), 1.0, Time, num, Nothing, False))
                    num += Delay
                Next
            Finally
                Dim enumerator As IEnumerator
                If TypeOf enumerator Is IDisposable Then
                    TryCast(enumerator, IDisposable).Dispose()
                End If
            End Try
            Return list
        End Function

        Public Sub AniStart(AniGroup As IList, Optional Name As String = "", Optional RefreshTime As Boolean = False)
            If RefreshTime Then
                ModAnimation.AniLastTick = ModBase.GetTimeMs()
            End If
            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
            Try
                For Each obj As Object In AniGroup
                    Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                    If TypeOf objectValue Is ICollection Then
                        list.AddRange(CType(objectValue, IEnumerable(Of ModAnimation.AniData)))
                    Else
                        Dim list2 As List(Of ModAnimation.AniData) = list
                        Dim obj2 As Object = objectValue
                        list2.Add(If((obj2 IsNot Nothing), CType(obj2, ModAnimation.AniData), Nothing))
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator
                If TypeOf enumerator Is IDisposable Then
                    TryCast(enumerator, IDisposable).Dispose()
                End If
            End Try
            Dim aniGroupEntry As ModAnimation.AniGroupEntry = New ModAnimation.AniGroupEntry() With { .Data = list, .StartTick = ModBase.GetTimeMs() }
            If Operators.CompareString(Name, "", False) = 0 Then
                Name = Conversions.ToString(aniGroupEntry.Uuid)
            Else
                ModAnimation.AniStop(Name)
            End If
            ModAnimation.AniGroups.Add(Name, aniGroupEntry)
        End Sub

        Public Sub AniStart(AniGroup As ModAnimation.AniData, Optional Name As String = "", Optional RefreshTime As Boolean = False)
            ModAnimation.AniStart(New List(Of ModAnimation.AniData)() From { AniGroup }, Name, RefreshTime)
        End Sub

        Public Sub AniStop(Name As String)
            ModAnimation.AniGroups.Remove(Name)
        End Sub

        Public Function AniIsRun(Name As String) As Boolean
            Return ModAnimation.AniGroups.ContainsKey(Name)
        End Function

        Public Sub AniStart()
            ModAnimation.AniLastTick = ModBase.GetTimeMs()
            ModAnimation.AniFPSTimer = ModAnimation.AniLastTick
            ModAnimation.AniRunning = True
            Dim action As Action
            If ModAnimation._Closure$__.$I62-0 IsNot Nothing Then
                action = ModAnimation._Closure$__.$I62-0
            Else
                Dim action2 As Action = Sub()
                    Try
                        Logger.Info("动画线程开始", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModAnimation.vb")
                        While True
                            Dim CS$<>8__locals1 As ModAnimation._Closure$__62-0 = New ModAnimation._Closure$__62-0(CS$<>8__locals1)
                            CS$<>8__locals1.$VB$Local_DeltaTime = (ModBase.GetTimeMs() - ModAnimation.AniLastTick).Clamp(0L, 100000L)
                            If CS$<>8__locals1.$VB$Local_DeltaTime >= 3L Then
                                ModAnimation.AniLastTick = ModBase.GetTimeMs()
                                If ModBase.ModeDebug Then
                                    If(ModAnimation.AniLastTick - ModAnimation.AniFPSTimer).Clamp(0L, 100000L) >= 500L Then
                                        ModAnimation.AniFPS = ModAnimation.AniFPSCounter
                                        ModAnimation.AniFPSCounter = 0
                                        ModAnimation.AniFPSTimer = ModAnimation.AniLastTick
                                    End If
                                    ModAnimation.AniFPSCounter += 2
                                End If
                                ModBase.RunInUiWait(Sub()
                                    ModAnimation.AniCount = 0
                                    ModAnimation.AniTimer(CDbl(CS$<>8__locals1.$VB$Local_DeltaTime) * ModAnimation.AniSpeed)
                                    If ModBase.RandomInteger(0, 64 * If(ModBase.ModeDebug, 5, 30)) = 0 AndAlso ((ModAnimation.AniFPS < 62 AndAlso ModAnimation.AniFPS > 0) OrElse ModAnimation.AniCount > 4 OrElse ModNet.NetManager.FileRemain <> 0) Then
                                        Logger.Info(String.Format("FPS {0}, 动画 {1}, 下载中 {2}（{3}/s）", New Object() { ModAnimation.AniFPS, ModAnimation.AniCount, ModNet.NetManager.FileRemain, StringUtils.FormatByteSize(ModNet.NetManager.Speed) }), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModAnimation.vb")
                                        If ModNet.NetManager.FileRemain < 5 Then
                                            Try
                                                For Each netFile As ModNet.NetFile In ModNet.NetManager.AllFiles.Values
                                                    If netFile.State < ModNet.NetState.Finished Then
                                                        Logger.Trace(String.Format("- {0} {1}/s {2}", netFile.State, StringUtils.FormatByteSize(netFile.Speed), netFile.LocalPath), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModAnimation.vb")
                                                    End If
                                                Next
                                            Finally
                                                Dim enumerator As IEnumerator(Of ModNet.NetFile)
                                                If enumerator IsNot Nothing Then
                                                    enumerator.Dispose()
                                                End If
                                            End Try
                                        End If
                                    End If
                                End Sub)
                            End If
                            Thread.Sleep(1)
                        End While
                    Catch ex As Exception
                        Logger.[Error](ex, "动画帧执行失败", LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModAnimation.vb")
                    End Try
                End Sub
                action = action2
                ModAnimation._Closure$__.$I62-0 = action2
            End If
            ModBase.RunInNewThread(action, "Animation", ThreadPriority.AboveNormal)
        End Sub

        Public Sub AniTimer(DeltaTick As Double)
            ' The following expression was wrapped in a checked-statement
            Try
                If DeltaTick / ModAnimation.AniSpeed > 100.0 Then
                    Logger.Info(String.Format("两个动画帧间隔 {0} ms", DeltaTick), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModAnimation.vb")
                End If
                Dim num As Integer = -1
                While num + 1 < ModAnimation.AniGroups.Count
                    num += 1
                    Dim aniGroupEntry As ModAnimation.AniGroupEntry = ModAnimation.AniGroups.Values.ElementAtOrDefault(num)
                    If aniGroupEntry.StartTick <= ModAnimation.AniLastTick Then
                        Dim flag As Boolean = True
                        Dim i As Integer = 0
                        While i < aniGroupEntry.Data.Count
                            Dim aniData As ModAnimation.AniData = aniGroupEntry.Data(i)
                            If Not aniData.IsAfter Then
                                flag = False
                                Dim ptr As __ByRef(Of Double) = aniData.TimeFinished
                                aniData.TimeFinished = ptr + DeltaTick
                                If aniData.TimeFinished > 0.0 Then
                                    aniData = ModAnimation.AniRun(aniData)
                                    ModAnimation.AniCount += 1
                                End If
                                If aniData.TimeFinished >= CDbl(aniData.TimeTotal) Then
                                    If Conversions.ToBoolean(aniData.TypeMain = ModAnimation.AniType.Color AndAlso Conversions.ToBoolean(Operators.NotObject(Operators.CompareObjectEqual(NewLateBinding.LateIndexGet(aniData.Obj, New Object() { 2 }, Nothing), "", False)))) Then
                                        Dim obj As Object = NewLateBinding.LateIndexGet(aniData.Obj, New Object() { 0 }, Nothing)
                                        Dim type As Type = Nothing
                                        Dim text As String = "SetResourceReference"
                                        Dim array As Object() = New Object(1) {}
                                        Dim num2 As Integer = 0
                                        Dim obj2 As Object = aniData.Obj
                                        Dim obj3 As Object = obj2
                                        Dim array2 As Object() = New Object(0) {}
                                        Dim num3 As Integer = 0
                                        Dim obj4 As Object = 1
                                        Dim obj5 As Object = obj4
                                        array2(num3) = obj4
                                        array(num2) = NewLateBinding.LateIndexGet(obj3, array2, Nothing)
                                        Dim num4 As Integer = 1
                                        Dim obj6 As Object = aniData.Obj
                                        Dim obj7 As Object = obj6
                                        Dim array3 As Object() = New Object(0) {}
                                        Dim num5 As Integer = 0
                                        Dim obj8 As Object = 2
                                        Dim obj9 As Object = obj8
                                        array3(num5) = obj8
                                        array(num4) = NewLateBinding.LateIndexGet(obj7, array3, Nothing)
                                        Dim array4 As Object() = array
                                        Dim array5 As String() = Nothing
                                        Dim array6 As Type() = Nothing
                                        Dim array7 As Boolean() = New Boolean() { True, True }
                                        Dim array8 As Boolean() = array7
                                        NewLateBinding.LateCall(obj, type, text, array, array5, array6, array7, True)
                                        If array8(0) Then
                                            NewLateBinding.LateIndexSetComplex(obj2, New Object() { obj5, array4(0) }, Nothing, True, False)
                                        End If
                                        If array8(1) Then
                                            NewLateBinding.LateIndexSetComplex(obj6, New Object() { obj9, array4(1) }, Nothing, True, False)
                                        End If
                                    End If
                                    aniGroupEntry.Data.RemoveAt(i)
                                Else
                                    aniGroupEntry.Data(i) = aniData
                                    i += 1
                                End If
                            Else
                                If Not flag Then
                                    Exit While
                                End If
                                flag = False
                                aniData.IsAfter = False
                                aniGroupEntry.Data(i) = aniData
                            End If
                        End While
                        If Not aniGroupEntry.Data.Any() Then
                            Dim num6 As Integer = ModAnimation.AniGroups.Count - 1
                            For j As Integer = 0 To num6
                                If ModAnimation.AniGroups.ElementAt(j).Value.Uuid = aniGroupEntry.Uuid Then
                                    ModAnimation.AniGroups.Remove(ModAnimation.AniGroups.ElementAt(j).Key)
                                    Exit For
                                End If
                            Next
                            num -= 1
                        End If
                    End If
                End While
            Catch ex As Exception
                Logger.[Error](ex, "动画刻执行失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModAnimation.vb")
            End Try
        End Sub

        Private Function AniRun(Ani As ModAnimation.AniData) As ModAnimation.AniData
            Try
                Select Case Ani.TypeMain
                    Case ModAnimation.AniType.Number
                        Dim num As Double = MathUtils.Lerp(0.0, Conversions.ToDouble(Ani.Value), Ani.Ease.GetDelta(Ani.TimeFinished / CDbl(Ani.TimeTotal), Ani.TimePercent))
                        If num <> 0.0 Then
                            Select Case Ani.TypeSub
                                Case ModAnimation.AniTypeSub.X
                                    If TypeOf Ani.Obj Is Window Then
                                        Dim window As Window = CType(Ani.Obj, Window)
                                        Dim window2 As Window = window
                                        window.Left = window2.Left + num
                                    Else
                                        Dim obj As Object = NewLateBinding.LateGet(Ani.Obj, Nothing, "HorizontalAlignment", New Object(-1) {}, Nothing, Nothing, Nothing)
                                        If Conversions.ToBoolean(Conversions.ToBoolean(Operators.CompareObjectEqual(obj, HorizontalAlignment.Left, False)) OrElse Conversions.ToBoolean(Operators.CompareObjectEqual(obj, HorizontalAlignment.Stretch, False))) Then
                                            NewLateBinding.LateSet(Ani.Obj, Nothing, "Margin", New Object() { New Thickness(Conversions.ToDouble(Operators.AddObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Left", New Object(-1) {}, Nothing, Nothing, Nothing), num)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Top", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Right", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Bottom", New Object(-1) {}, Nothing, Nothing, Nothing))) }, Nothing, Nothing)
                                        ElseIf Operators.ConditionalCompareObjectEqual(obj, HorizontalAlignment.Right, False) Then
                                            NewLateBinding.LateSet(Ani.Obj, Nothing, "Margin", New Object() { New Thickness(Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Left", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Top", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Right", New Object(-1) {}, Nothing, Nothing, Nothing), num)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Bottom", New Object(-1) {}, Nothing, Nothing, Nothing))) }, Nothing, Nothing)
                                        End If
                                    End If
                                Case ModAnimation.AniTypeSub.Y
                                    If TypeOf Ani.Obj Is Window Then
                                        Dim window3 As Window = CType(Ani.Obj, Window)
                                        Dim window2 As Window = window3
                                        window3.Top = window2.Top + num
                                    Else
                                        Dim obj2 As Object = NewLateBinding.LateGet(Ani.Obj, Nothing, "VerticalAlignment", New Object(-1) {}, Nothing, Nothing, Nothing)
                                        If Operators.ConditionalCompareObjectEqual(obj2, VerticalAlignment.Top, False) Then
                                            NewLateBinding.LateSet(Ani.Obj, Nothing, "Margin", New Object() { New Thickness(Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Left", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(Operators.AddObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Top", New Object(-1) {}, Nothing, Nothing, Nothing), num)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Right", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Bottom", New Object(-1) {}, Nothing, Nothing, Nothing))) }, Nothing, Nothing)
                                        ElseIf Operators.ConditionalCompareObjectEqual(obj2, VerticalAlignment.Bottom, False) Then
                                            NewLateBinding.LateSet(Ani.Obj, Nothing, "Margin", New Object() { New Thickness(Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Left", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Top", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Right", New Object(-1) {}, Nothing, Nothing, Nothing)), Conversions.ToDouble(Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Margin", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Bottom", New Object(-1) {}, Nothing, Nothing, Nothing), num))) }, Nothing, Nothing)
                                        End If
                                    End If
                                Case ModAnimation.AniTypeSub.Width
                                    Dim frameworkElement As FrameworkElement = CType(Ani.Obj, FrameworkElement)
                                    frameworkElement.Width = Math.Max(If(Double.IsNaN(frameworkElement.Width), frameworkElement.ActualWidth, frameworkElement.Width) + num, 0.0)
                                Case ModAnimation.AniTypeSub.Height
                                    Dim frameworkElement2 As FrameworkElement = CType(Ani.Obj, FrameworkElement)
                                    frameworkElement2.Height = Math.Max(If(Double.IsNaN(frameworkElement2.Height), frameworkElement2.ActualHeight, frameworkElement2.Height) + num, 0.0)
                                Case ModAnimation.AniTypeSub.Opacity
                                    If TypeOf Ani.Obj Is UIElement Then
                                        Dim uielement As UIElement = CType(Ani.Obj, UIElement)
                                        uielement.Opacity = (uielement.Opacity + num).Clamp(0.0, 1.0)
                                    ElseIf TypeOf Ani.Obj Is DropShadowEffect Then
                                        Dim dropShadowEffect As DropShadowEffect = CType(Ani.Obj, DropShadowEffect)
                                        dropShadowEffect.Opacity = (dropShadowEffect.Opacity + num).Clamp(0.0, 1.0)
                                    End If
                                Case ModAnimation.AniTypeSub.Value
                                    Dim obj3 As Object = Ani.Obj
                                    NewLateBinding.LateSet(obj3, Nothing, "Value", New Object() { Operators.AddObject(NewLateBinding.LateGet(obj3, Nothing, "Value", New Object(-1) {}, Nothing, Nothing, Nothing), num) }, Nothing, Nothing)
                                Case ModAnimation.AniTypeSub.Radius
                                    Dim obj3 As Object = Ani.Obj
                                    NewLateBinding.LateSet(obj3, Nothing, "Radius", New Object() { Operators.AddObject(NewLateBinding.LateGet(obj3, Nothing, "Radius", New Object(-1) {}, Nothing, Nothing, Nothing), num) }, Nothing, Nothing)
                                Case ModAnimation.AniTypeSub.BorderThickness
                                    Dim obj4 As Object = Ani.Obj
                                    Dim type As Type = Nothing
                                    Dim text As String = "BorderThickness"
                                    Dim array As Object() = New Object(0) {}
                                    Dim num2 As Integer = 0
                                    Dim obj5 As Object = NewLateBinding.LateGet(Ani.Obj, Nothing, "BorderThickness", New Object(-1) {}, Nothing, Nothing, Nothing)
                                    array(num2) = New Thickness(If((obj5 IsNot Nothing), CType(obj5, Thickness), Nothing).Bottom + num)
                                    NewLateBinding.LateSet(obj4, type, text, array, Nothing, Nothing)
                                Case ModAnimation.AniTypeSub.StrokeThickness
                                    NewLateBinding.LateSet(Ani.Obj, Nothing, "StrokeThickness", New Object() { NewLateBinding.LateGet(Nothing, GetType(Math), "Max", New Object() { Operators.AddObject(NewLateBinding.LateGet(Ani.Obj, Nothing, "StrokeThickness", New Object(-1) {}, Nothing, Nothing, Nothing), num), 0 }, Nothing, Nothing, Nothing) }, Nothing, Nothing)
                                Case ModAnimation.AniTypeSub.TranslateX
                                    If Information.IsNothing(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Ani.Obj, Nothing, "RenderTransform", New Object(-1) {}, Nothing, Nothing, Nothing))) OrElse Not(TypeOf NewLateBinding.LateGet(Ani.Obj, Nothing, "RenderTransform", New Object(-1) {}, Nothing, Nothing, Nothing)Is TranslateTransform) Then
                                        NewLateBinding.LateSet(Ani.Obj, Nothing, "RenderTransform", New Object() { New TranslateTransform(0.0, 0.0) }, Nothing, Nothing)
                                    End If
                                    Dim translateTransform As TranslateTransform = CType(NewLateBinding.LateGet(Ani.Obj, Nothing, "RenderTransform", New Object(-1) {}, Nothing, Nothing, Nothing), TranslateTransform)
                                    Dim translateTransform2 As TranslateTransform = translateTransform
                                    translateTransform.X = translateTransform2.X + num
                                Case ModAnimation.AniTypeSub.TranslateY
                                    If Information.IsNothing(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Ani.Obj, Nothing, "RenderTransform", New Object(-1) {}, Nothing, Nothing, Nothing))) OrElse Not(TypeOf NewLateBinding.LateGet(Ani.Obj, Nothing, "RenderTransform", New Object(-1) {}, Nothing, Nothing, Nothing)Is TranslateTransform) Then
                                        NewLateBinding.LateSet(Ani.Obj, Nothing, "RenderTransform", New Object() { New TranslateTransform(0.0, 0.0) }, Nothing, Nothing)
                                    End If
                                    Dim translateTransform3 As TranslateTransform = CType(NewLateBinding.LateGet(Ani.Obj, Nothing, "RenderTransform", New Object(-1) {}, Nothing, Nothing, Nothing), TranslateTransform)
                                    Dim translateTransform2 As TranslateTransform = translateTransform3
                                    translateTransform3.Y = translateTransform2.Y + num
                                Case ModAnimation.AniTypeSub.[Double]
                                    Dim obj6 As Object = NewLateBinding.LateIndexGet(Ani.Obj, New Object() { 0 }, Nothing)
                                    Dim type2 As Type = Nothing
                                    Dim text2 As String = "SetValue"
                                    Dim array2 As Object() = New Object(1) {}
                                    Dim num3 As Integer = 0
                                    Dim obj3 As Object = Ani.Obj
                                    Dim obj7 As Object = obj3
                                    Dim array3 As Object() = New Object(0) {}
                                    Dim num4 As Integer = 0
                                    Dim obj8 As Object = 1
                                    Dim obj9 As Object = obj8
                                    array3(num4) = obj8
                                    array2(num3) = NewLateBinding.LateIndexGet(obj7, array3, Nothing)
                                    Dim num5 As Integer = 1
                                    Dim obj10 As Object = NewLateBinding.LateIndexGet(Ani.Obj, New Object() { 0 }, Nothing)
                                    Dim type3 As Type = Nothing
                                    Dim text3 As String = "GetValue"
                                    Dim array4 As Object() = New Object(0) {}
                                    Dim num6 As Integer = 0
                                    Dim obj11 As Object = Ani.Obj
                                    Dim obj12 As Object = obj11
                                    Dim array5 As Object() = New Object(0) {}
                                    Dim num7 As Integer = 0
                                    Dim obj13 As Object = 1
                                    Dim obj14 As Object = obj13
                                    array5(num7) = obj13
                                    array4(num6) = NewLateBinding.LateIndexGet(obj12, array5, Nothing)
                                    Dim array6 As Object() = array4
                                    Dim array7 As String() = Nothing
                                    Dim array8 As Type() = Nothing
                                    Dim array9 As Boolean() = New Boolean() { True }
                                    Dim array10 As Boolean() = array9
                                    Dim obj15 As Object = NewLateBinding.LateGet(obj10, type3, text3, array4, array7, array8, array9)
                                    If array10(0) Then
                                        NewLateBinding.LateIndexSetComplex(obj11, New Object() { obj14, array6(0) }, Nothing, True, False)
                                    End If
                                    array2(num5) = Operators.AddObject(obj15, num)
                                    Dim array11 As Object() = array2
                                    Dim array12 As String() = Nothing
                                    Dim array13 As Type() = Nothing
                                    Dim array14 As Boolean() = New Boolean(1) {}
                                    array14(0) = True
                                    Dim array15 As Boolean() = array14
                                    NewLateBinding.LateCall(obj6, type2, text2, array2, array12, array13, array14, True)
                                    If array15(0) Then
                                        NewLateBinding.LateIndexSetComplex(obj3, New Object() { obj9, array11(0) }, Nothing, True, False)
                                    End If
                                Case ModAnimation.AniTypeSub.DoubleParam
                                    CType(Ani.Obj, ParameterizedThreadStart)(num)
                                Case ModAnimation.AniTypeSub.GridLengthWidth
                                    NewLateBinding.LateSet(Ani.Obj, Nothing, "Width", New Object() { New GridLength(Conversions.ToDouble(NewLateBinding.LateGet(Nothing, GetType(Math), "Max", New Object() { Operators.AddObject(NewLateBinding.LateGet(NewLateBinding.LateGet(Ani.Obj, Nothing, "Width", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Value", New Object(-1) {}, Nothing, Nothing, Nothing), num), 0 }, Nothing, Nothing, Nothing)), GridUnitType.Star) }, Nothing, Nothing)
                            End Select
                        End If
                    Case ModAnimation.AniType.Color
                        Dim myColor As ModBase.MyColor = ModBase.MyColor.Lerp(New ModBase.MyColor(0.0, 0.0, 0.0, 0.0), CType(Ani.Value, ModBase.MyColor), Ani.Ease.GetDelta(Ani.TimeFinished / CDbl(Ani.TimeTotal), Ani.TimePercent)) + CType(Ani.ValueLast, ModBase.MyColor)
                        Dim frameworkElement3 As FrameworkElement = CType(NewLateBinding.LateIndexGet(Ani.Obj, New Object() { 0 }, Nothing), FrameworkElement)
                        Dim dependencyProperty As DependencyProperty = CType(NewLateBinding.LateIndexGet(Ani.Obj, New Object() { 1 }, Nothing), DependencyProperty)
                        Dim myColor2 As ModBase.MyColor = New ModBase.MyColor(RuntimeHelpers.GetObjectValue(frameworkElement3.GetValue(dependencyProperty))) + myColor
                        frameworkElement3.SetValue(dependencyProperty, RuntimeHelpers.GetObjectValue(If((Operators.CompareString(dependencyProperty.PropertyType.Name, "Color", False) = 0), myColor2, myColor2)))
                        Ani.ValueLast = myColor2 - New ModBase.MyColor(RuntimeHelpers.GetObjectValue(frameworkElement3.GetValue(dependencyProperty)))
                    Case ModAnimation.AniType.Scale
                        Dim frameworkElement4 As FrameworkElement = CType(Ani.Obj, FrameworkElement)
                        Dim delta As Double = Ani.Ease.GetDelta(Ani.TimeFinished / CDbl(Ani.TimeTotal), Ani.TimePercent)
                        frameworkElement4.Margin = New Thickness(frameworkElement4.Margin.Left + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, Nothing, "Left", New Object(-1) {}, Nothing, Nothing, Nothing)), delta), frameworkElement4.Margin.Top + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, Nothing, "Top", New Object(-1) {}, Nothing, Nothing, Nothing)), delta), frameworkElement4.Margin.Right + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, Nothing, "Left", New Object(-1) {}, Nothing, Nothing, Nothing)), delta), frameworkElement4.Margin.Bottom + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, Nothing, "Top", New Object(-1) {}, Nothing, Nothing, Nothing)), delta))
                        frameworkElement4.Width = Math.Max(frameworkElement4.Width + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, Nothing, "Width", New Object(-1) {}, Nothing, Nothing, Nothing)), delta), 0.0)
                        frameworkElement4.Height = Math.Max(frameworkElement4.Height + MathUtils.Lerp(0.0, Conversions.ToDouble(NewLateBinding.LateGet(Ani.Value, Nothing, "Height", New Object(-1) {}, Nothing, Nothing, Nothing)), delta), 0.0)
                    Case ModAnimation.AniType.TextAppear
                        Dim text4 As String = Conversions.ToString(NewLateBinding.LateIndexGet(Ani.Value, New Object() { 0 }, Nothing))
                        Dim num8 As Integer = CInt(Math.Round(CDbl(If(Conversions.ToBoolean(NewLateBinding.LateIndexGet(Ani.Value, New Object() { 1 }, Nothing)), text4.Length, 0)) + Math.Round(CDbl((text4.Length * If(Conversions.ToBoolean(NewLateBinding.LateIndexGet(Ani.Value, New Object() { 1 }, Nothing)), (-1), 1))) * Ani.Ease.GetDelta(Ani.TimeFinished / CDbl(Ani.TimeTotal), 0.0))))
                        Dim text5 As String = text4.Substring(0, num8)
                        If num8 < text4.Length Then
                            If Convert.ToInt32(Convert.ToChar(text4.Substring(num8, 1))) >= Convert.ToInt32(Convert.ToChar(128)) Then
                                text5 += Encoding.GetEncoding("GB18030").GetString(New Byte() { CByte(ModBase.RandomInteger(176, 247)), CByte(ModBase.RandomInteger(161, 249)) })
                            Else
                                text5 += ModBase.RandomInteger(0, 85).ConvertRadix(86)
                            End If
                        End If
                        If TypeOf Ani.Obj Is TextBlock Then
                            NewLateBinding.LateSet(Ani.Obj, Nothing, "Text", New Object() { text5 }, Nothing, Nothing)
                        Else
                            NewLateBinding.LateSet(Ani.Obj, Nothing, "Context", New Object() { text5 }, Nothing, Nothing)
                        End If
                    Case ModAnimation.AniType.Code
                        Ani.TimeFinished = CDbl(Ani.TimeTotal)
                        CType(Ani.Value, ThreadStart)()
                    Case ModAnimation.AniType.ScaleTransform
                        Dim frameworkElement5 As FrameworkElement = CType(Ani.Obj, FrameworkElement)
                        If Not(TypeOf frameworkElement5.RenderTransform Is ScaleTransform) Then
                            If frameworkElement5.RenderTransformOrigin = New Point(0.0, 0.0) Then
                                frameworkElement5.RenderTransformOrigin = New Point(0.5, 0.5)
                            End If
                            frameworkElement5.RenderTransform = New ScaleTransform(1.0, 1.0)
                        End If
                        Dim num9 As Double = MathUtils.Lerp(0.0, Conversions.ToDouble(Ani.Value), Ani.Ease.GetDelta(Ani.TimeFinished / CDbl(Ani.TimeTotal), Ani.TimePercent))
                        CType(frameworkElement5.RenderTransform, ScaleTransform).ScaleX = Math.Max(CType(frameworkElement5.RenderTransform, ScaleTransform).ScaleX + num9, 0.0)
                        CType(frameworkElement5.RenderTransform, ScaleTransform).ScaleY = Math.Max(CType(frameworkElement5.RenderTransform, ScaleTransform).ScaleY + num9, 0.0)
                    Case ModAnimation.AniType.RotateTransform
                        Dim frameworkElement6 As FrameworkElement = CType(Ani.Obj, FrameworkElement)
                        If Not(TypeOf frameworkElement6.RenderTransform Is RotateTransform) Then
                            If frameworkElement6.RenderTransformOrigin = New Point(0.0, 0.0) Then
                                frameworkElement6.RenderTransformOrigin = New Point(0.5, 0.5)
                            End If
                            frameworkElement6.RenderTransform = New RotateTransform(0.0)
                        End If
                        Dim num10 As Double = MathUtils.Lerp(0.0, Conversions.ToDouble(Ani.Value), Ani.Ease.GetDelta(Ani.TimeFinished / CDbl(Ani.TimeTotal), Ani.TimePercent))
                        CType(frameworkElement6.RenderTransform, RotateTransform).Angle = CType(frameworkElement6.RenderTransform, RotateTransform).Angle + num10
                End Select
                Ani.TimePercent = Ani.TimeFinished / CDbl(Ani.TimeTotal)
            Catch ex As Exception
                Logger.[Error](ex, String.Format("执行动画失败：{0}", Ani), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Base\ModAnimation.vb")
            End Try
            Return Ani
        End Function

        Public AniGroups As Dictionary(Of String, ModAnimation.AniGroupEntry) = New Dictionary(Of String, ModAnimation.AniGroupEntry)()

        Private AniLastTick As Long

        Public AniRunning As Boolean = False

        Private _AniControlEnabled As Integer = 0

        Private AniControlEnabledLock As Object = RuntimeHelpers.GetObjectValue(New Object())

        Private AniCount As Integer = 0

        Private AniFPSCounter As Integer = 0

        Private AniFPSTimer As Long = 0L

        Public AniFPS As Integer = 0

        Public Class AniGroupEntry
            Public Sub New()
                Me.Uuid = ModBase.GetUuid()
            End Sub

            Public Data As List(Of ModAnimation.AniData)

            Public StartTick As Long

            Public Uuid As Integer
        End Class

        Public Structure AniData
            Public Overrides Function ToString() As String
                Return String.Concat(New String() { Me.TypeMain.ToString(), " | ", Conversions.ToString(Me.TimeFinished), "/", Conversions.ToString(Me.TimeTotal), "(", Conversions.ToString(Math.Round(Me.TimePercent * 100.0)), "%)", If((Me.Obj Is Nothing), "", String.Concat(New String() { " | ", Me.Obj.ToString(), "(", Me.Obj.[GetType]().Name, ")" })) })
            End Function

            Public TypeMain As ModAnimation.AniType

            Public TypeSub As ModAnimation.AniTypeSub

            Public TimeTotal As Integer

            Public TimeFinished As Double

            Public TimePercent As Double

            Public IsAfter As Boolean

            Public Ease As ModAnimation.AniEase

            Public Obj As Object

            Public Value As Object

            Public ValueLast As Object
        End Structure

        Public Enum AniType
            Number
            Color
            Scale
            TextAppear
            Code
            ScaleTransform
            RotateTransform
        End Enum

        Public Enum AniTypeSub
            X
            Y
            Width
            Height
            Opacity
            Value
            Radius
            BorderThickness
            StrokeThickness
            TranslateX
            TranslateY
            [Double]
            DoubleParam
            GridLengthWidth
        End Enum

        Public Enum AniEasePower
            Weak = 2
            Middle
            Strong
            ExtraStrong
        End Enum

        Public MustInherit Class AniEase
            Public MustOverride Function GetValue(t As Double) As Double

            Public Overridable Function GetDelta(t1 As Double, t0 As Double) As Double
                Return Me.GetValue(t1) - Me.GetValue(t0)
            End Function
        End Class

        Public Class AniEaseInout
            Inherits ModAnimation.AniEase

            Public Sub New(EaseIn As ModAnimation.AniEase, EaseOut As ModAnimation.AniEase, Optional EaseInPercent As Double = 0.5)
                Me.EaseIn = EaseIn
                Me.EaseOut = EaseOut
                Me.EaseInPercent = EaseInPercent
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                Dim num As Double
                If t < Me.EaseInPercent Then
                    num = Me.EaseInPercent * Me.EaseIn.GetValue(t / Me.EaseInPercent)
                Else
                    num = (1.0 - Me.EaseInPercent) * Me.EaseOut.GetValue((t - Me.EaseInPercent) / (1.0 - Me.EaseInPercent)) + Me.EaseInPercent
                End If
                Return num
            End Function

            Private EaseIn As ModAnimation.AniEase

            Private EaseOut As ModAnimation.AniEase

            Private EaseInPercent As Double
        End Class

        Public Class AniEaseLinear
            Inherits ModAnimation.AniEase

            Public Overrides Function GetValue(t As Double) As Double
                Return t.Clamp(0.0, 1.0)
            End Function

            Public Overrides Function GetDelta(t1 As Double, t0 As Double) As Double
                Return t1.Clamp(0.0, 1.0) - t0.Clamp(0.0, 1.0)
            End Function
        End Class

        Public Class AniEaseInFluent
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                Me.p = Power
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                Return Math.Pow(t.Clamp(0.0, 1.0), CDbl(Me.p))
            End Function

            Private p As ModAnimation.AniEasePower
        End Class

        Public Class AniEaseOutFluent
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                Me.p = Power
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                Return 1.0 - Math.Pow((1.0 - t).Clamp(0.0, 1.0), CDbl(Me.p))
            End Function

            Private p As ModAnimation.AniEasePower
        End Class

        Public Class AniEaseInoutFluent
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle, Optional Middle As Double = 0.5)
                Me.Ease = New ModAnimation.AniEaseInout(New ModAnimation.AniEaseInFluent(Power), New ModAnimation.AniEaseOutFluent(Power), Middle)
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                Return Me.Ease.GetValue(t)
            End Function

            Private Ease As ModAnimation.AniEaseInout
        End Class

        Public Class AniEaseOutFluentWithInitial
            Inherits ModAnimation.AniEase

            Public Sub New(InitialPixelPerSecond As Double, TotalSecond As Double, TotalDistance As Double)
                Dim num As Double = InitialPixelPerSecond * TotalSecond / TotalDistance
                Me.alpha = num - 1.0
                If Me.alpha < 0.0 Then
                    Me.alpha = 0.0
                End If
            End Sub

            Public Overrides Function GetValue(percent As Double) As Double
                Dim num As Double = percent.Clamp(0.0, 1.0)
                Dim num2 As Double
                If Me.alpha = 0.0 Then
                    num2 = num
                Else
                    num2 = (Me.alpha + 1.0) * num / (1.0 + Me.alpha * num)
                End If
                Return num2
            End Function

            Private alpha As Double
        End Class

        Public Class AniEaseInBack
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                Me.p = 3.0 - CDbl(Power) * 0.5
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                t = t.Clamp(0.0, 1.0)
                Return Math.Pow(t, Me.p) * Math.Cos(4.71238898038469 * (1.0 - t))
            End Function

            Private p As Double
        End Class

        Public Class AniEaseOutBack
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                Me.p = 3.0 - CDbl(Power) * 0.5
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                t = t.Clamp(0.0, 1.0)
                Return 1.0 - Math.Pow(1.0 - t, Me.p) * Math.Cos(4.71238898038469 * t)
            End Function

            Private p As Double
        End Class

        Public Class AniEaseInCar
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Middle As Double = 0.7, Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                Me.Ease = New ModAnimation.AniEaseInout(New ModAnimation.AniEaseInBack(Power), New ModAnimation.AniEaseOutFluent(Power), Middle)
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                Return Me.Ease.GetValue(t)
            End Function

            Private Ease As ModAnimation.AniEaseInout
        End Class

        Public Class AniEaseOutCar
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Middle As Double = 0.3, Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                Me.Ease = New ModAnimation.AniEaseInout(New ModAnimation.AniEaseInFluent(Power), New ModAnimation.AniEaseOutBack(Power), Middle)
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                Return Me.Ease.GetValue(t)
            End Function

            Private Ease As ModAnimation.AniEaseInout
        End Class

        Public Class AniEaseInElastic
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                ' The following expression was wrapped in a checked-expression
                Me.p = CInt((Power + 4))
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                t = t.Clamp(0.0, 1.0)
                Return Math.Pow(t, CDbl((Me.p - 1)) * 0.25) * Math.Cos((CDbl(Me.p) - 3.5) * 3.141592653589793 * Math.Pow(1.0 - t, 1.5))
            End Function

            Private p As Integer
        End Class

        Public Class AniEaseOutElastic
            Inherits ModAnimation.AniEase

            Public Sub New(Optional Power As ModAnimation.AniEasePower = ModAnimation.AniEasePower.Middle)
                ' The following expression was wrapped in a checked-expression
                Me.p = CInt((Power + 4))
            End Sub

            Public Overrides Function GetValue(t As Double) As Double
                t = 1.0 - t.Clamp(0.0, 1.0)
                Return 1.0 - Math.Pow(t, CDbl((Me.p - 1)) * 0.25) * Math.Cos((CDbl(Me.p) - 3.5) * 3.141592653589793 * Math.Pow(1.0 - t, 1.5))
            End Function

            Private p As Integer
        End Class
    End Module
End Namespace
