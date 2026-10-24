Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCL.My
Imports PCLCS

Namespace PCL
    Public NotInheritable Module ModMain
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub RaiseCustomEvent(Control As DependencyObject)
            Dim CS$<>8__locals1 As ModMain._Closure$__1-0 = New ModMain._Closure$__1-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Events = CustomEventService.GetEvents(Control).ToList()
            Dim eventType As CustomEvent.EventType = CustomEventService.GetEventType(Control)
            If eventType <> CustomEvent.EventType.None Then
                CS$<>8__locals1.$VB$Local_Events.Add(New CustomEvent(eventType, CustomEventService.GetEventData(Control)))
            End If
            If CS$<>8__locals1.$VB$Local_Events.Any() Then
                ModBase.RunInNewThread(Sub()
                    Try
                        For Each customEvent As CustomEvent In CS$<>8__locals1.$VB$Local_Events
                            customEvent.Raise()
                        Next
                    Finally
                        Dim enumerator As List(Of CustomEvent).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                End Sub, "执行自定义事件 " + Conversions.ToString(ModBase.GetUuid()), ThreadPriority.Normal)
            End If
        End Sub

        Public Sub Hint(Text As String, Optional Type As ModMain.HintType = ModMain.HintType.Blue, Optional Log As Boolean = True)
            ModMain.HintWaiting.Enqueue(New ModMain.HintMessage() With { .Text = (If(Text, "")), .Type = Type, .Log = Log })
        End Sub

        Private Sub HintTick()
            Try
                Dim hintMessage As ModMain.HintMessage = Nothing
                While ModMain.HintWaiting.TryDequeue(hintMessage)
                    Dim CS$<>8__locals1 As ModMain._Closure$__6-0 = New ModMain._Closure$__6-0(CS$<>8__locals1)
                    hintMessage.Text = hintMessage.Text.ReplaceLineEndings(" ", True)
                    If ModMain.FrmMain.PanHint.Children.Count < 20 Then
                        CS$<>8__locals1.$VB$Local_DoubleStack = Nothing
                        Try
                            For Each obj As Object In ModMain.FrmMain.PanHint.Children
                                Dim border As Border = CType(obj, Border)
                                If Conversions.ToBoolean(Conversions.ToBoolean(NewLateBinding.LateIndexGet(border.Tag, New Object() { 0 }, Nothing)) AndAlso Operators.CompareString(CType(border.Child, TextBlock).Text, hintMessage.Text, False) = 0) Then
                                    CS$<>8__locals1.$VB$Local_DoubleStack = border
                                End If
                            Next
                        Finally
                            Dim enumerator As IEnumerator
                            If TypeOf enumerator Is IDisposable Then
                                TryCast(enumerator, IDisposable).Dispose()
                            End If
                        End Try
                        CS$<>8__locals1.$VB$Local_Percent = 0.3
                        Dim type As ModMain.HintType = hintMessage.Type
                        If type <> ModMain.HintType.Blue Then
                            If type <> ModMain.HintType.Green Then
                                CS$<>8__locals1.$VB$Local_TargetColor0 = New ModBase.OurColor(215.0, 255.0, 53.0, 11.0)
                                CS$<>8__locals1.$VB$Local_TargetColor1 = New ModBase.OurColor(215.0, 255.0, 43.0, 0.0)
                            Else
                                CS$<>8__locals1.$VB$Local_TargetColor0 = New ModBase.OurColor(215.0, 33.0, 177.0, 33.0)
                                CS$<>8__locals1.$VB$Local_TargetColor1 = New ModBase.OurColor(215.0, 29.0, 160.0, 29.0)
                            End If
                        Else
                            CS$<>8__locals1.$VB$Local_TargetColor0 = New ModBase.OurColor(215.0, 37.0, 155.0, 252.0)
                            CS$<>8__locals1.$VB$Local_TargetColor1 = New ModBase.OurColor(215.0, 10.0, 142.0, 252.0)
                        End If
                        If Not Information.IsNothing(CS$<>8__locals1.$VB$Local_DoubleStack) Then
                            If Not ModAnimation.AniIsRun(Conversions.ToString(Operators.ConcatenateObject("Hint Show ", NewLateBinding.LateIndexGet(CS$<>8__locals1.$VB$Local_DoubleStack.Tag, New Object() { 1 }, Nothing)))) Then
                                ModAnimation.AniStop(Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS$<>8__locals1.$VB$Local_DoubleStack.Tag, New Object() { 1 }, Nothing))))
                                Dim num As Double = CDbl((800 + hintMessage.Text.Length.Clamp(5, 23) * 180)) * ModAnimation.AniSpeed
                                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaX(CS$<>8__locals1.$VB$Local_DoubleStack, -12.0 - CS$<>8__locals1.$VB$Local_DoubleStack.Margin.Left, 50, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaX(CS$<>8__locals1.$VB$Local_DoubleStack, -8.0, 50, 50, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaX(CS$<>8__locals1.$VB$Local_DoubleStack, 8.0, 50, 100, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaX(CS$<>8__locals1.$VB$Local_DoubleStack, -8.0, 50, 150, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaDouble(Sub(i As Object)
                                    CS$<>8__locals1.$VB$Local_Percent = Conversions.ToDouble(Operators.AddObject(CS$<>8__locals1.$VB$Local_Percent, i))
                                    Dim linearGradientBrush As LinearGradientBrush = CType(CS$<>8__locals1.$VB$Local_DoubleStack.Background, LinearGradientBrush)
                                    linearGradientBrush.GradientStops(0).Color = CS$<>8__locals1.$VB$Local_TargetColor0 * CS$<>8__locals1.$VB$Local_Percent + New ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS$<>8__locals1.$VB$Local_Percent)
                                    linearGradientBrush.GradientStops(1).Color = CS$<>8__locals1.$VB$Local_TargetColor1 * CS$<>8__locals1.$VB$Local_Percent + New ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS$<>8__locals1.$VB$Local_Percent)
                                End Sub, 0.7, 250, 0, Nothing, False), ModAnimation.AaX(CS$<>8__locals1.$VB$Local_DoubleStack, -50.0, 200, CInt(Math.Round(num)), New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(CS$<>8__locals1.$VB$Local_DoubleStack, -1.0, 150, CInt(Math.Round(num)), Nothing, False), ModAnimation.AaCode(Sub()
                                    NewLateBinding.LateIndexSetComplex(CS$<>8__locals1.$VB$Local_DoubleStack.Tag, New Object() { 0, False }, Nothing, False, True)
                                End Sub, CInt(Math.Round(num)), False), ModAnimation.AaHeight(CS$<>8__locals1.$VB$Local_DoubleStack, -26.0, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), True), ModAnimation.AaCode(Sub()
                                    ModMain.FrmMain.PanHint.Children.Remove(CS$<>8__locals1.$VB$Local_DoubleStack)
                                End Sub, 0, True) }, Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS$<>8__locals1.$VB$Local_DoubleStack.Tag, New Object() { 1 }, Nothing))), False)
                            End If
                        Else
                            Dim CS$<>8__locals2 As ModMain._Closure$__6-1 = New ModMain._Closure$__6-1(CS$<>8__locals2)
                            CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                            CS$<>8__locals2.$VB$Local_NewHintControl = New Border() With { .Tag = New Object() { True, ModBase.GetUuid() }, .Margin = New Thickness(-70.0, 0.0, 20.0, 0.0), .Opacity = 0.0, .Height = 0.0, .HorizontalAlignment = HorizontalAlignment.Left, .CornerRadius = New CornerRadius(0.0, 6.0, 6.0, 0.0) }
                            CS$<>8__locals2.$VB$Local_NewHintControl.Background = New LinearGradientBrush(New GradientStopCollection(New List(Of GradientStop)() From { New GradientStop(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_TargetColor0 * CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent + New ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent), 0.0), New GradientStop(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_TargetColor1 * CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent + New ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent), 1.0) }), 90.0)
                            CS$<>8__locals2.$VB$Local_NewHintControl.Child = New TextBlock() With { .TextTrimming = TextTrimming.CharacterEllipsis, .FontSize = 13.0, .Text = hintMessage.Text, .Foreground = New ModBase.OurColor(255.0, 255.0, 255.0), .Margin = New Thickness(33.0, 5.0, 8.0, 5.0) }
                            ModMain.FrmMain.PanHint.Children.Add(CS$<>8__locals2.$VB$Local_NewHintControl)
                            Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                            If ModMain.FrmMain.PanHint.Children.Count > 1 Then
                                list.Add(ModAnimation.AaHeight(CS$<>8__locals2.$VB$Local_NewHintControl, 26.0, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                            Else
                                CS$<>8__locals2.$VB$Local_NewHintControl.Height = 26.0
                            End If
                            list.AddRange(New ModAnimation.AniData() { ModAnimation.AaX(CS$<>8__locals2.$VB$Local_NewHintControl, 30.0, 400, 0, New ModAnimation.AniEaseOutElastic(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaX(CS$<>8__locals2.$VB$Local_NewHintControl, 20.0, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(CS$<>8__locals2.$VB$Local_NewHintControl, 1.0, 100, 0, Nothing, False), ModAnimation.AaDouble(Sub(i As Object)
                                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent = Conversions.ToDouble(Operators.AddObject(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent, i))
                                Dim linearGradientBrush2 As LinearGradientBrush = CType(CS$<>8__locals2.$VB$Local_NewHintControl.Background, LinearGradientBrush)
                                linearGradientBrush2.GradientStops(0).Color = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_TargetColor0 * CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent + New ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent)
                                linearGradientBrush2.GradientStops(1).Color = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_TargetColor1 * CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent + New ModBase.OurColor(255.0, 255.0, 255.0) * (1.0 - CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Percent)
                            End Sub, 0.7, 250, 100, Nothing, False) })
                            ModAnimation.AniStart(list, Conversions.ToString(Operators.ConcatenateObject("Hint Show ", NewLateBinding.LateIndexGet(CS$<>8__locals2.$VB$Local_NewHintControl.Tag, New Object() { 1 }, Nothing))), False)
                            Dim num2 As Double = CDbl((800 + hintMessage.Text.Length.Clamp(5, 23) * 180)) * ModAnimation.AniSpeed
                            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaX(CS$<>8__locals2.$VB$Local_NewHintControl, -50.0, 200, CInt(Math.Round(num2)), New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(CS$<>8__locals2.$VB$Local_NewHintControl, -1.0, 150, CInt(Math.Round(num2)), Nothing, False), ModAnimation.AaCode(Sub()
                                NewLateBinding.LateIndexSetComplex(CS$<>8__locals2.$VB$Local_NewHintControl.Tag, New Object() { 0, False }, Nothing, False, True)
                            End Sub, CInt(Math.Round(num2)), False), ModAnimation.AaHeight(CS$<>8__locals2.$VB$Local_NewHintControl, -26.0, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), True), ModAnimation.AaCode(Sub()
                                ModMain.FrmMain.PanHint.Children.Remove(CS$<>8__locals2.$VB$Local_NewHintControl)
                            End Sub, 0, True) }, Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS$<>8__locals2.$VB$Local_NewHintControl.Tag, New Object() { 1 }, Nothing))), False)
                        End If
                    End If
                    If hintMessage.Log Then
                        Logger.Info(String.Format("弹出提示：{0}", hintMessage.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    End If
                End While
            Catch ex As Exception
                Logger.Info(ex, "显示弹出提示失败", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            End Try
        End Sub

        Private Sub HideAllHint()
            Try
                Dim enumerator As IEnumerator = ModMain.FrmMain.PanHint.Children.GetEnumerator()
                While enumerator.MoveNext()
                    Dim CS$<>8__locals1 As ModMain._Closure$__7-0 = New ModMain._Closure$__7-0(CS$<>8__locals1)
                    CS$<>8__locals1.$VB$Local_Control = CType(enumerator.Current, Border)
                    CS$<>8__locals1.$VB$Local_Control.IsHitTestVisible = False
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaX(CS$<>8__locals1.$VB$Local_Control, -50.0, 200, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(CS$<>8__locals1.$VB$Local_Control, -1.0, 150, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                        NewLateBinding.LateIndexSetComplex(CS$<>8__locals1.$VB$Local_Control.Tag, New Object() { 0, False }, Nothing, False, True)
                    End Sub, 0, False), ModAnimation.AaHeight(CS$<>8__locals1.$VB$Local_Control, -26.0, 100, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), True), ModAnimation.AaCode(Sub()
                        ModMain.FrmMain.PanHint.Children.Remove(CS$<>8__locals1.$VB$Local_Control)
                    End Sub, 0, True) }, Conversions.ToString(Operators.ConcatenateObject("Hint Hide ", NewLateBinding.LateIndexGet(CS$<>8__locals1.$VB$Local_Control.Tag, New Object() { 1 }, Nothing))), False)
                End While
            Finally
                Dim enumerator As IEnumerator
                If TypeOf enumerator Is IDisposable Then
                    TryCast(enumerator, IDisposable).Dispose()
                End If
            End Try
        End Sub

        Public Function OurMsgBox(Caption As String, Optional Title As String = "提示", Optional Button1 As String = "确定", Optional Button2 As String = "", Optional Button3 As String = "", Optional IsWarn As Boolean = False, Optional HighLight As Boolean = True, Optional ForceWait As Boolean = False, Optional Button1Action As Action = Nothing, Optional Button2Action As Action = Nothing, Optional Button3Action As Action = Nothing) As Integer
            Dim myMsgBoxConverter As ModMain.OurMsgBoxConverter = New ModMain.OurMsgBoxConverter() With { .Type = ModMain.OurMsgBoxType.Text, .Button1 = Button1, .Button2 = Button2, .Button3 = Button3, .Text = Caption, .IsWarn = IsWarn, .Title = Title, .HighLight = HighLight, .ForceWait = True, .Button1Action = Button1Action, .Button2Action = Button2Action, .Button3Action = Button3Action }
            Dim num As Integer
            If Button2.Length > 0 OrElse ForceWait Then
                If ModMain.FrmMain Is Nothing OrElse (ModMain.FrmMain.PanMsg Is Nothing AndAlso ModBase.RunInUi()) Then
                    If Button2.Length > 0 Then
                        Dim msgBoxResult As MsgBoxResult = Interaction.MsgBox(Caption, If((Button3.Length > 0), MsgBoxStyle.YesNoCancel, MsgBoxStyle.YesNo) + If(IsWarn, 16, 32), Title)
                        If msgBoxResult <> MsgBoxResult.Cancel Then
                            If msgBoxResult <> MsgBoxResult.Yes Then
                                If msgBoxResult = MsgBoxResult.No Then
                                    myMsgBoxConverter.Result = 2
                                End If
                            Else
                                myMsgBoxConverter.Result = 1
                            End If
                        Else
                            myMsgBoxConverter.Result = 3
                        End If
                    Else
                        Interaction.MsgBox(Caption, MsgBoxStyle.OkOnly + If(IsWarn, 16, 32), Title)
                        myMsgBoxConverter.Result = 1
                    End If
                    Logger.Warn(String.Format("主窗体加载完成前出现意料外的等待弹窗：{0},{1},{2}", Button1, Button2, Button3), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                Else
                    ModMain.WaitingOurMsgBox.Enqueue(myMsgBoxConverter)
                    Try
                        ModMain.FrmMain.DragStop()
                        If ModBase.RunInUi() Then
                            ModMain.OurMsgBoxTick()
                        End If
                        ComponentDispatcher.PushModal()
                        Dispatcher.PushFrame(myMsgBoxConverter.WaitFrame)
                    Finally
                        ComponentDispatcher.PopModal()
                    End Try
                End If
                Logger.Info(String.Format("普通弹框返回：{0}", RuntimeHelpers.GetObjectValue(If(myMsgBoxConverter.Result, "null"))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                num = Conversions.ToInteger(myMsgBoxConverter.Result)
            Else
                ModMain.WaitingOurMsgBox.Enqueue(myMsgBoxConverter)
                num = 1
            End If
            Return num
        End Function

        Public Function OurMsgBoxInput(Title As String, Optional Text As String = Nothing, Optional DefaultInput As String = Nothing, Optional ValidateRules As Collection(Of Validate) = Nothing, Optional HintText As String = "", Optional Button1 As String = "确定", Optional Button2 As String = "取消", Optional IsWarn As Boolean = False) As String
            Dim myMsgBoxConverter As ModMain.OurMsgBoxConverter = New ModMain.OurMsgBoxConverter() With { .Text = (If(Text, "")), .HintText = (If(HintText, "")), .Type = ModMain.OurMsgBoxType.Input, .ValidateRules = (If(ValidateRules, New Collection(Of Validate)())), .Button1 = Button1, .Button2 = Button2, .Content = DefaultInput, .IsWarn = IsWarn, .Title = Title }
            ModMain.WaitingOurMsgBox.Enqueue(myMsgBoxConverter)
            Try
                If ModMain.FrmMain IsNot Nothing Then
                    ModMain.FrmMain.DragStop()
                End If
                If ModBase.RunInUi() Then
                    ModMain.OurMsgBoxTick()
                End If
                ComponentDispatcher.PushModal()
                Dispatcher.PushFrame(myMsgBoxConverter.WaitFrame)
            Finally
                ComponentDispatcher.PopModal()
            End Try
            Logger.Info(String.Format("输入弹框返回：{0}", RuntimeHelpers.GetObjectValue(If(myMsgBoxConverter.Result, "null"))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            Return Conversions.ToString(myMsgBoxConverter.Result)
        End Function

        Public Function OurMsgBoxSelect(Selections As IEnumerable(Of IOurRadio), Optional Title As String = "提示", Optional Button1 As String = "确定", Optional Button2 As String = "", Optional IsWarn As Boolean = False) As Integer?
            Dim myMsgBoxConverter As ModMain.OurMsgBoxConverter = New ModMain.OurMsgBoxConverter() With { .Type = ModMain.OurMsgBoxType.[Select], .Button1 = Button1, .Button2 = Button2, .Content = Selections, .IsWarn = IsWarn, .Title = Title }
            ModMain.WaitingOurMsgBox.Enqueue(myMsgBoxConverter)
            Try
                If ModMain.FrmMain IsNot Nothing Then
                    ModMain.FrmMain.DragStop()
                End If
                If ModBase.RunInUi() Then
                    ModMain.OurMsgBoxTick()
                End If
                ComponentDispatcher.PushModal()
                Dispatcher.PushFrame(myMsgBoxConverter.WaitFrame)
            Finally
                ComponentDispatcher.PopModal()
            End Try
            Logger.Info(String.Format("选择弹框返回：{0}", RuntimeHelpers.GetObjectValue(If(myMsgBoxConverter.Result, "null"))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            Return CType(myMsgBoxConverter.Result, Integer?)
        End Function

        Public Sub OurMsgBoxTick()
            Try
                Dim frmMain As FormMain = ModMain.FrmMain
                If If((frmMain IsNot Nothing), frmMain.PanMsg, Nothing) IsNot Nothing AndAlso ModMain.FrmMain.WindowState <> WindowState.Minimized Then
                    Dim myMsgBoxConverter As ModMain.OurMsgBoxConverter = Nothing
                    If ModMain.FrmMain.PanMsg.Children.Count > 0 Then
                        ModMain.FrmMain.PanMsg.Visibility = Visibility.Visible
                    ElseIf ModMain.WaitingOurMsgBox.TryDequeue(myMsgBoxConverter) Then
                        ModMain.FrmMain.PanMsg.Visibility = Visibility.Visible
                        Select Case myMsgBoxConverter.Type
                            Case ModMain.OurMsgBoxType.Text
                                ModMain.FrmMain.PanMsg.Children.Add(New OurMsgText(myMsgBoxConverter))
                            Case ModMain.OurMsgBoxType.[Select]
                                ModMain.FrmMain.PanMsg.Children.Add(New OurMsgSelect(myMsgBoxConverter))
                            Case ModMain.OurMsgBoxType.Input
                                ModMain.FrmMain.PanMsg.Children.Add(New OurMsgInput(myMsgBoxConverter))
                            Case ModMain.OurMsgBoxType.Login
                                ModMain.FrmMain.PanMsg.Children.Add(New OurMsgLogin(myMsgBoxConverter))
                        End Select
                    ElseIf ModMain.FrmMain.PanMsg.Visibility <> Visibility.Collapsed Then
                        ModMain.FrmMain.PanMsg.Visibility = Visibility.Collapsed
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "处理等待中的弹窗失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            End Try
        End Sub

        Private Sub HelpLoad(Loader As ModLoader.LoaderTask(Of Integer, List(Of ModMain.HelpEntry)))
            Dim helpLoadLock As Object = ModMain.HelpLoadLock
            ObjectFlowControl.CheckForSyncLockOnValueType(helpLoadLock)
            SyncLock helpLoadLock
                Try
                    ModMain.HelpTryExtract()
                    Dim list As List(Of String) = New List(Of String)()
                    Try
                        Dim list2 As List(Of String) = New List(Of String)()
                        If DirectoryUtils.Exists(Paths.Base + "PCL\Help\") Then
                            Try
                                For Each text As String In DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\Help\", True, "*")
                                    Dim extension As String = PathUtils.GetExtension(text)
                                    If Operators.CompareString(extension, "helpignore", False) <> 0 Then
                                        If Operators.CompareString(extension, "json", False) = 0 Then
                                            list.Add(text)
                                        End If
                                    Else
                                        Logger.Info(String.Format("发现 .helpignore 文件：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                                        Dim array As String() = FileUtils.ReadAsLines(text, True, Nothing, Nothing)
                                        For i As Integer = 0 To array.Length - 1
                                            Dim text2 As String = array(i).BeforeFirst("#", False).Trim()
                                            If Not String.IsNullOrWhiteSpace(text2) Then
                                                list2.Add(text2)
                                                Logger.Trace(String.Format("> {0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                                            End If
                                        Next
                                    End If
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of String)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                        End If
                        Logger.Info(String.Format("已扫描 PCL 文件夹下的帮助文件，目前总计 {0} 条", list.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                        Try
                            Dim enumerator2 As IEnumerator(Of String) = DirectoryUtils.EnumerateFiles(ModBase.PathTemp + "Help", True, "*").GetEnumerator()
                            IL_0241:
                            While enumerator2.MoveNext()
                                Dim text3 As String = enumerator2.Current
                                If Not PathUtils.RemoveLastPart(text3).Substring((ModBase.PathTemp + "Help").Length).Contains("\.") AndAlso Operators.CompareString(PathUtils.GetExtension(text3), "json", False) = 0 Then
                                    Dim text4 As String = text3.Substring((ModBase.PathTemp + "Help\").Length)
                                    Try
                                        For Each text5 As String In list2
                                            If text4.RegexCheck(text5, RegexOptions.None) Then
                                                Logger.Trace(String.Format("已忽略 {0}：{1}", text4, text5), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                                                GoTo IL_0241
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator3 As List(Of String).Enumerator
                                        CType(enumerator3, IDisposable).Dispose()
                                    End Try
                                    list.Add(text3)
                                End If
                            End While
                        Finally
                            Dim enumerator2 As IEnumerator(Of String)
                            If enumerator2 IsNot Nothing Then
                                enumerator2.Dispose()
                            End If
                        End Try
                        Logger.Info(String.Format("已扫描缓存文件夹下的帮助文件，目前总计 {0} 条", list.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    Catch ex As Exception
                        Logger.[Error](ex, "检查帮助文件夹失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    End Try
                    If Not Loader.IsCanceled Then
                        Dim list3 As List(Of ModMain.HelpEntry) = New List(Of ModMain.HelpEntry)()
                        Try
                            For Each text6 As String In list
                                Try
                                    Dim helpEntry As ModMain.HelpEntry = New ModMain.HelpEntry(text6)
                                    list3.Add(helpEntry)
                                    Logger.Trace(String.Format("已加载的帮助条目：{0} ← {1}", helpEntry.Title, text6), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                                Catch ex2 As Exception
                                    Logger.[Error](ex2, String.Format("初始化帮助条目失败（{0}）", text6), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                                End Try
                            Next
                        Finally
                            Dim enumerator4 As List(Of String).Enumerator
                            CType(enumerator4, IDisposable).Dispose()
                        End Try
                        If Not list3.Any() Then
                            Throw New Exception("未找到可用的帮助；若不需要帮助页面，可以在 设置 → 个性化 → 功能隐藏 中将其隐藏")
                        End If
                        If Not Loader.IsCanceled Then
                            Loader.Output = list3
                        End If
                    End If
                Catch ex3 As Exception
                    Logger.Warn(ex3, "帮助列表初始化失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    Throw
                End Try
            End SyncLock
        End Sub

        Public Sub HelpTryExtract()
            If Settings.[Get](Of Integer)("SystemHelpVersion", Nothing) <> 414 OrElse Not FileUtils.Exists(ModBase.PathTemp + "Help\启动器\备份设置.xaml") Then
                DirectoryUtils.Delete(ModBase.PathTemp + "Help", False)
                ModBase.ExtractResources(ModBase.PathTemp + "Cache\Help.zip", "Help")
                FileUtils.ExtractToDirectory(ModBase.PathTemp + "Cache\Help.zip", ModBase.PathTemp + "Help", Nothing, Nothing)
                Settings.[Set]("SystemHelpVersion", 414, Nothing)
                Logger.Info(String.Format("已解压内置帮助文件，目前状态：{0}", FileUtils.Exists(ModBase.PathTemp + "Help\启动器\备份设置.xaml")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            End If
        End Sub

        Public ReadOnly Property IsAprilEnabled As Boolean
            Get
                If ModMain._IsAprilEnabled Is Nothing Then
                    ModMain._IsAprilEnabled = New Boolean?(DateTime.Now.Month = 4 AndAlso DateTime.Now.Day = 1 AndAlso Settings.[Get](Of Integer)("AprilYear", Nothing) <> DateTime.Now.Year)
                End If
                Return ModMain._IsAprilEnabled.Value
            End Get
        End Property

        Private Sub TimerFool()
            Try
                If ModMain.FrmLaunchLeft IsNot Nothing AndAlso ModMain.FrmLaunchLeft.AprilPosTrans IsNot Nothing AndAlso ModMain.FrmMain.lastMouseArg IsNot Nothing Then
                    If Not ModMain.IsAprilGiveup AndAlso Not(ModMain.FrmMain.PageCurrent IsNot FormMain.PageType.Launch) AndAlso ModAnimation.AniControlEnabled = 0 AndAlso ModMain.FrmLaunchLeft.BtnLaunch.IsLoaded Then
                        Dim position As Point = ModMain.FrmMain.lastMouseArg.GetPosition(ModMain.FrmMain)
                        If ModMain.$STATIC$TimerFool$001$IdieCount$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModMain.$STATIC$TimerFool$001$IdieCount$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag As Boolean = False
                        Try
                            Monitor.Enter(ModMain.$STATIC$TimerFool$001$IdieCount$Init, flag)
                            If ModMain.$STATIC$TimerFool$001$IdieCount$Init.State = 0S Then
                                ModMain.$STATIC$TimerFool$001$IdieCount$Init.State = 2S
                                ModMain.$STATIC$TimerFool$001$IdieCount = 0
                            ElseIf ModMain.$STATIC$TimerFool$001$IdieCount$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            ModMain.$STATIC$TimerFool$001$IdieCount$Init.State = 1S
                            If flag Then
                                Monitor.[Exit](ModMain.$STATIC$TimerFool$001$IdieCount$Init)
                            End If
                        End Try
                        If ModMain.$STATIC$TimerFool$001$MousePosLast$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModMain.$STATIC$TimerFool$001$MousePosLast$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag2 As Boolean = False
                        Try
                            Monitor.Enter(ModMain.$STATIC$TimerFool$001$MousePosLast$Init, flag2)
                            If ModMain.$STATIC$TimerFool$001$MousePosLast$Init.State = 0S Then
                                ModMain.$STATIC$TimerFool$001$MousePosLast$Init.State = 2S
                                ModMain.$STATIC$TimerFool$001$MousePosLast = New Point(0.0, 0.0)
                            ElseIf ModMain.$STATIC$TimerFool$001$MousePosLast$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            ModMain.$STATIC$TimerFool$001$MousePosLast$Init.State = 1S
                            If flag2 Then
                                Monitor.[Exit](ModMain.$STATIC$TimerFool$001$MousePosLast$Init)
                            End If
                        End Try
                        Dim num As Double
                        Dim num2 As Double
                        Dim vector As Vector
                        Dim vector3 As Vector
                        If position = ModMain.$STATIC$TimerFool$001$MousePosLast Then
                            ModMain.$STATIC$TimerFool$001$IdieCount += 1
                        Else
                            ModMain.$STATIC$TimerFool$001$MousePosLast = position
                            ModMain.$STATIC$TimerFool$001$IdieCount = 0
                        End If
                        num = ModMain.FrmLaunchLeft.BtnLaunch.ActualWidth / 2.0
                        num2 = ModMain.FrmLaunchLeft.BtnLaunch.ActualHeight / 2.0
                        vector = CType((ModMain.FrmMain.lastMouseArg.GetPosition(ModMain.FrmLaunchLeft.BtnLaunch) - New Vector(num, num2)), Vector)
                        Dim vector2 As Vector = New Vector(vector.X, vector.Y)
                        vector2.Normalize()
                        vector3 = -vector2
                        Dim length As Double = New Vector(Math.Max(0.0, Math.Abs(vector.X) - num), Math.Max(0.0, Math.Abs(vector.Y) - num2)).Length
                        Math.Sin(CDbl(ModMain.Timer150Count) / 37.5 * 3.141592653589793)
                        Dim num3 As Double = 1.0 / (1.0 + Math.Pow(CDbl(ModMain.AprilDifficultiedDistance) / 6000.0, 2.0)) + 0.25
                        Dim vector4 As Vector = Math.Max(0.0, -0.65 - Math.Log((length / num3 + 0.4) / 200.0)) * vector3 * num3
                        If ModMain.$STATIC$TimerFool$001$IdieCount >= 320 Then
                            Dim vector5 As Vector = CType((ModMain.FrmMain.lastMouseArg.GetPosition(ModMain.FrmMain.PanMain) - New Vector(num, ModMain.FrmMain.PanMain.ActualHeight - num2 * 3.0)), Vector)
                            Dim vector6 As Vector = New Vector(ModMain.FrmLaunchLeft.AprilPosTrans.X, ModMain.FrmLaunchLeft.AprilPosTrans.Y)
                            If vector5.Length > 250.0 AndAlso vector6.Length > 0.4 Then
                                vector4 -= vector6 * 0.0005
                                vector6.Normalize()
                                vector4 -= vector6 * 0.15
                            End If
                        End If
                        If ModMain.$STATIC$TimerFool$001$Speed$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModMain.$STATIC$TimerFool$001$Speed$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag3 As Boolean = False
                        Try
                            Monitor.Enter(ModMain.$STATIC$TimerFool$001$Speed$Init, flag3)
                            If ModMain.$STATIC$TimerFool$001$Speed$Init.State = 0S Then
                                ModMain.$STATIC$TimerFool$001$Speed$Init.State = 2S
                                ModMain.$STATIC$TimerFool$001$Speed = New Vector(0.0, 0.0)
                            ElseIf ModMain.$STATIC$TimerFool$001$Speed$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            ModMain.$STATIC$TimerFool$001$Speed$Init.State = 1S
                            If flag3 Then
                                Monitor.[Exit](ModMain.$STATIC$TimerFool$001$Speed$Init)
                            End If
                        End Try
                        Dim point As Point = ModMain.FrmLaunchLeft.BtnLaunch.TranslatePoint(New Point(0.0, 0.0), ModMain.FrmMain.PanForm)
                        If point.X < -num * 2.0 Then
                            Dim aprilPosTrans As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                            Dim translateTransform As TranslateTransform = aprilPosTrans
                            aprilPosTrans.X = translateTransform.X + (ModMain.FrmMain.PanForm.ActualWidth + num * 2.0)
                            ModMain.$STATIC$TimerFool$001$Speed.X = ModMain.$STATIC$TimerFool$001$Speed.X - 80.0
                            If point.Y < 0.0 Then
                                Dim aprilPosTrans2 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans2
                                aprilPosTrans2.Y = translateTransform.Y + num2 * 2.5
                            ElseIf point.Y > ModMain.FrmMain.PanForm.ActualHeight - num2 * 2.0 Then
                                Dim aprilPosTrans3 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans3
                                aprilPosTrans3.Y = translateTransform.Y - num2 * 2.5
                            End If
                        ElseIf point.X > ModMain.FrmMain.PanForm.ActualWidth Then
                            Dim aprilPosTrans4 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                            Dim translateTransform As TranslateTransform = aprilPosTrans4
                            aprilPosTrans4.X = translateTransform.X - (ModMain.FrmMain.PanForm.ActualWidth + num * 2.0)
                            ModMain.$STATIC$TimerFool$001$Speed.X = ModMain.$STATIC$TimerFool$001$Speed.X + 80.0
                            If point.Y < 0.0 Then
                                Dim aprilPosTrans5 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans5
                                aprilPosTrans5.Y = translateTransform.Y + num2 * 2.5
                            ElseIf point.Y > ModMain.FrmMain.PanForm.ActualHeight - num2 * 2.0 Then
                                Dim aprilPosTrans6 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans6
                                aprilPosTrans6.Y = translateTransform.Y - num2 * 2.5
                            End If
                        ElseIf point.Y < -num2 * 2.0 Then
                            Dim aprilPosTrans7 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                            Dim translateTransform As TranslateTransform = aprilPosTrans7
                            aprilPosTrans7.Y = translateTransform.Y + (ModMain.FrmMain.PanForm.ActualHeight + num2 * 2.0)
                            ModMain.$STATIC$TimerFool$001$Speed.Y = ModMain.$STATIC$TimerFool$001$Speed.Y - 25.0
                            If point.X < 0.0 Then
                                Dim aprilPosTrans8 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans8
                                aprilPosTrans8.X = translateTransform.X + num * 2.0
                            ElseIf point.X > ModMain.FrmMain.PanForm.ActualWidth - num * 2.0 Then
                                Dim aprilPosTrans9 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans9
                                aprilPosTrans9.X = translateTransform.X - num * 2.0
                            End If
                        ElseIf point.Y > ModMain.FrmMain.PanForm.ActualHeight Then
                            Dim aprilPosTrans10 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                            Dim translateTransform As TranslateTransform = aprilPosTrans10
                            aprilPosTrans10.Y = translateTransform.Y - (ModMain.FrmMain.PanForm.ActualHeight + num2 * 2.0)
                            ModMain.$STATIC$TimerFool$001$Speed.Y = ModMain.$STATIC$TimerFool$001$Speed.Y + 25.0
                            If point.X < 0.0 Then
                                Dim aprilPosTrans11 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans11
                                aprilPosTrans11.X = translateTransform.X + num * 2.0
                            ElseIf point.X > ModMain.FrmMain.PanForm.ActualWidth - num * 2.0 Then
                                Dim aprilPosTrans12 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                                translateTransform = aprilPosTrans12
                                aprilPosTrans12.X = translateTransform.X - num * 2.0
                            End If
                        End If
                        ModMain.$STATIC$TimerFool$001$Speed = ModMain.$STATIC$TimerFool$001$Speed * 0.8 + vector4
                        Dim num4 As Double = Math.Min(60.0, ModMain.$STATIC$TimerFool$001$Speed.Length)
                        If num4 >= 0.01 Then
                            ModMain.$STATIC$TimerFool$001$Speed.Normalize()
                            ModMain.$STATIC$TimerFool$001$Speed *= num4
                            Dim translateTransform As TranslateTransform
                            Dim aprilPosTrans13 As TranslateTransform
                            ModMain.AprilDifficultiedDistance = CInt(Math.Round(CDbl(ModMain.AprilDifficultiedDistance) + num4))
                            aprilPosTrans13 = ModMain.FrmLaunchLeft.AprilPosTrans
                            translateTransform = aprilPosTrans13
                            aprilPosTrans13.X = translateTransform.X + ModMain.$STATIC$TimerFool$001$Speed.X
                            Dim aprilPosTrans14 As TranslateTransform = ModMain.FrmLaunchLeft.AprilPosTrans
                            translateTransform = aprilPosTrans14
                            aprilPosTrans14.Y = translateTransform.Y + ModMain.$STATIC$TimerFool$001$Speed.Y
                            ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleX = (1.0 - (Math.Abs(vector3.X) - Math.Abs(vector3.Y)) * (num4 / 160.0)).Clamp(0.2, 1.8)
                            ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleY = (1.0 - (Math.Abs(vector3.Y) - Math.Abs(vector3.X)) * (num4 / 100.0)).Clamp(0.2, 1.8)
                            If ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init Is Nothing Then
                                Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init, New StaticLocalInitFlag(), Nothing)
                            End If
                            Dim flag4 As Boolean = False
                            Try
                                Monitor.Enter(ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init, flag4)
                                If ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init.State = 0S Then
                                    ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init.State = 2S
                                    ModMain.$STATIC$TimerFool$001$GiveUpDistance = -1500.0
                                ElseIf ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init.State = 2S Then
                                    Throw New IncompleteInitialization()
                                End If
                            Finally
                                ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init.State = 1S
                                If flag4 Then
                                    Monitor.[Exit](ModMain.$STATIC$TimerFool$001$GiveUpDistance$Init)
                                End If
                            End Try
                            ModMain.$STATIC$TimerFool$001$GiveUpDistance += num4
                            If ModMain.$STATIC$TimerFool$001$GiveUpDistance > 2500.0 Then
                                ModMain.$STATIC$TimerFool$001$GiveUpDistance = 0.0
                                Select Case ModBase.RandomInteger(0, 3)
                                    Case 0
                                        ModMain.Hint("放弃吧！只需要点一下右下角的小白旗……", ModMain.HintType.Red, True)
                                    Case 1
                                        ModMain.Hint("看到右下角的那面小白旗了吗？", ModMain.HintType.Red, True)
                                    Case 2
                                        ModMain.Hint("这里建议点一下右下角的小白旗投降呢.jpg", ModMain.HintType.Red, True)
                                    Case 3
                                        ModMain.Hint("右下角的小白旗永远等着你……", ModMain.HintType.Red, True)
                                End Select
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "愚人节移动出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            End Try
        End Sub

        Public Sub ShowWindowToTop(Handle As IntPtr)
            Try
                ModMain.PostMessage(Handle, 6402UI, 0L, 0L)
                ModMain.SetForegroundWindow(Handle)
            Catch ex As Exception
                Logger.[Error](ex, "设置窗口置顶失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            End Try
        End Sub

        Public Declare Ansi Function FindWindow Lib "user32" Alias "FindWindowA" (<MarshalAs(UnmanagedType.VBByRefStr)> ByRef ClassName As String, <MarshalAs(UnmanagedType.VBByRefStr)> ByRef WindowName As String) As IntPtr

        Public Declare Ansi Function SetForegroundWindow Lib "user32" (hWnd As IntPtr) As Integer

        Private Declare Ansi Function PostMessage Lib "user32" Alias "PostMessageA" (hWnd As IntPtr, msg As UInteger, wParam As Long, lParam As Long) As Boolean

        Public Sub SetGPUPreference(Executeable As String)
            Dim item As Object = RegistryUtils.Read("HKEY_CURRENT_USER\Software\Microsoft\DirectX\UserGpuPreferences", Executeable).Item1
            If Operators.CompareString(If((item IsNot Nothing), item.ToString(), Nothing), "GpuPreference=2;", False) = 0 Then
                Logger.Info(String.Format("无需调整显卡设置：{0}", Executeable), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                Return
            End If
            RegistryUtils.Write("HKEY_CURRENT_USER\Software\Microsoft\DirectX\UserGpuPreferences", Executeable, "GpuPreference=2;")
            Logger.Info(String.Format("已调整显卡设置：{0}", Executeable), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
        End Sub

        Public Function ArgumentReplace(Text As String, Optional EscapeHandler As Func(Of String, String) = Nothing, Optional ReplaceTime As Boolean = True) As String
            Dim text As String
            If Text Is Nothing OrElse Not Text.Contains("{") Then
                text = Text
            Else
                If ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init Is Nothing Then
                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init, New StaticLocalInitFlag(), Nothing)
                End If
                Dim flag As Boolean = False
                Try
                    Monitor.Enter(ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init, flag)
                    If ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init.State = 0S Then
                        ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init.State = 2S
                        ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer = Function(s As String)
                            Dim text20 As String
                            If s Is Nothing Then
                                text20 = ""
                            ElseIf EscapeHandler Is Nothing Then
                                text20 = s
                            Else
                                If s.Contains(":\") Then
                                    Dim flag2 As Boolean = s.EndsWithF("\", False) OrElse s.EndsWithF("/", False)
                                    s = PathUtils.ToShortPath(s)
                                    If flag2 Then
                                        s = PathUtils.AddSlashSuffix(s)
                                    End If
                                End If
                                text20 = EscapeHandler(s)
                            End If
                            Return text20
                        End Function
                    ElseIf ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init.State = 2S Then
                        Throw New IncompleteInitialization()
                    End If
                Finally
                    ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init.State = 1S
                    If flag Then
                        Monitor.[Exit](ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init)
                    End If
                End Try
                Text = Text.Replace("{pcl_version}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer("2.99.0.0"))
                Text = Text.Replace("{pcl_version_code}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Conversions.ToString(414)))
                Text = Text.Replace("{pcl_version_branch}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer("快照版"))
                Text = Text.Replace("{pcl_build_type}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModBase.BuildTypes.Snapshot.ToString()))
                Text = Text.Replace("{pcl_branch}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer("Official"))
                Text = Text.Replace("{identify}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModBase.Identify))
                Text = Text.Replace("{path}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Paths.Base))
                Text = Text.Replace("{path_with_name}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModBase.PathExe))
                Text = Text.Replace("{path_temp}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModBase.PathTemp))
                Dim text2 As String = Text
                Dim text3 As String = "{pcl_md5}"
                Dim func As Func(Of String)
                If ModMain._Closure$__.$I71-1 IsNot Nothing Then
                    func = ModMain._Closure$__.$I71-1
                Else
                    Dim func2 As Func(Of String) = Function() ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(CryptographyUtils.ComputeFileHash(ModBase.PathExe, CryptographyUtils.HashMethod.Md5))
                    func = func2
                    ModMain._Closure$__.$I71-1 = func2
                End If
                Text = text2.Replace(text3, func)
                Dim text4 As String = Text
                Dim text5 As String = "{pcl_sha1}"
                Dim func3 As Func(Of String)
                If ModMain._Closure$__.$I71-2 IsNot Nothing Then
                    func3 = ModMain._Closure$__.$I71-2
                Else
                    Dim func4 As Func(Of String) = Function() ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(CryptographyUtils.ComputeFileHash(ModBase.PathExe, CryptographyUtils.HashMethod.Sha1))
                    func3 = func4
                    ModMain._Closure$__.$I71-2 = func4
                End If
                Text = text4.Replace(text5, func3)
                If ReplaceTime Then
                    Text = Text.Replace("{date}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(DateTime.Now.ToString("yyyy'/'M'/'d")))
                    Text = Text.Replace("{time}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(DateTime.Now.ToString("HH':'mm':'ss")))
                End If
                Dim text6 As String = Text
                Dim text7 As String = "{java}"
                Dim func5 As Func(Of String, String) = ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer
                Dim mcLaunchJavaSelected As Java = ModLaunch.McLaunchJavaSelected
                Text = text6.Replace(text7, func5(If((mcLaunchJavaSelected IsNot Nothing), mcLaunchJavaSelected.Folder, Nothing)))
                Text = Text.Replace("{minecraft}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McFolderSelected))
                Dim mcInstanceSelected As ModMinecraft.McInstance = ModMinecraft.McInstanceSelected
                If mcInstanceSelected IsNot Nothing AndAlso mcInstanceSelected.IsLoaded Then
                    Text = Text.Replace("{version_path}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McInstanceSelected.PathVersion))
                    Text = Text.Replace("{verpath}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McInstanceSelected.PathVersion))
                    Text = Text.Replace("{version_indie}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McInstanceSelected.PathIndie))
                    Text = Text.Replace("{verindie}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McInstanceSelected.PathIndie))
                    Text = Text.Replace("{name}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McInstanceSelected.Name))
                    If New String() { "unknown", "old", "pending" }.Contains(ModMinecraft.McInstanceSelected.Version.VanillaName.Lower()) Then
                        Text = Text.Replace("{version}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McInstanceSelected.Name))
                    Else
                        Text = Text.Replace("{version}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModMinecraft.McInstanceSelected.Version.VanillaName))
                    End If
                    Text = Text.Replace("{forge}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(If(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.Forge), ModMinecraft.McInstanceSelected.Version.ModLoaderVersions(ModLoaders.Forge), "")))
                    Text = Text.Replace("{fabric}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(If(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.Fabric), ModMinecraft.McInstanceSelected.Version.ModLoaderVersions(ModLoaders.Fabric), "")))
                    Text = Text.Replace("{neoforge}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(If(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge), ModMinecraft.McInstanceSelected.Version.ModLoaderVersions(ModLoaders.NeoForge), "")))
                    Text = Text.Replace("{liteloader}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(If(ModMinecraft.McInstanceSelected.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader), "true", "")))
                Else
                    Text = Text.Replace("{version_path}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{verpath}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{version_indie}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{verindie}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{name}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{version}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{forge}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{fabric}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{neoforge}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{liteloader}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                End If
                If ModLaunch.McLoginLoader.State = ModBase.LoadState.Finished Then
                    Text = Text.Replace("{user}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(ModLaunch.McLoginLoader.Output.Name))
                    Dim text8 As String = Text
                    Dim text9 As String = "{uuid}"
                    Dim func6 As Func(Of String, String) = ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer
                    Dim uuid As String = ModLaunch.McLoginLoader.Output.Uuid
                    Text = text8.Replace(text9, func6(If((uuid IsNot Nothing), uuid.Lower(), Nothing)))
                    Select Case ModLaunch.McLoginLoader.Input.Type
                        Case ModLaunch.McLoginType.Legacy
                            Text = Text.Replace("{login}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer("离线"))
                        Case ModLaunch.McLoginType.Nide
                            Text = Text.Replace("{login}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer("统一通行证"))
                        Case ModLaunch.McLoginType.Auth
                            Text = Text.Replace("{login}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer("Authlib-Injector"))
                        Case ModLaunch.McLoginType.Ms
                            Text = Text.Replace("{login}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer("正版"))
                    End Select
                Else
                    Text = Text.Replace("{user}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{uuid}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                    Text = Text.Replace("{login}", ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Nothing))
                End If
                Dim text10 As String = Text
                Dim text11 As String = "\{hint\}"
                Dim matchEvaluator As MatchEvaluator
                If ModMain._Closure$__.$IR71-1 IsNot Nothing Then
                    matchEvaluator = ModMain._Closure$__.$IR71-1
                Else
                    Dim matchEvaluator2 As MatchEvaluator = Function(a0 As Match)
                        Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_0(Of String)
                        If ModMain._Closure$__.$I71-3 IsNot Nothing Then
                            vb$AnonymousDelegate_ = ModMain._Closure$__.$I71-3
                        Else
                            Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_0(Of String) = Function() ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(PageOtherTest.GetRandomHint())
                            vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                            ModMain._Closure$__.$I71-3 = vb$AnonymousDelegate_2
                        End If
                        Return vb$AnonymousDelegate_()
                    End Function
                    matchEvaluator = matchEvaluator2
                    ModMain._Closure$__.$IR71-1 = matchEvaluator2
                End If
                Text = text10.RegexReplace(text11, matchEvaluator, RegexOptions.None)
                Dim text12 As String = Text
                Dim text13 As String = "\{cave\}"
                Dim matchEvaluator3 As MatchEvaluator
                If ModMain._Closure$__.$IR71-2 IsNot Nothing Then
                    matchEvaluator3 = ModMain._Closure$__.$IR71-2
                Else
                    Dim matchEvaluator4 As MatchEvaluator = Function(a0 As Match)
                        Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_0(Of String)
                        If ModMain._Closure$__.$I71-4 IsNot Nothing Then
                            vb$AnonymousDelegate_3 = ModMain._Closure$__.$I71-4
                        Else
                            Dim vb$AnonymousDelegate_4 As VB$AnonymousDelegate_0(Of String) = Function() ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(PageOtherTest.GetRandomCave())
                            vb$AnonymousDelegate_3 = vb$AnonymousDelegate_4
                            ModMain._Closure$__.$I71-4 = vb$AnonymousDelegate_4
                        End If
                        Return vb$AnonymousDelegate_3()
                    End Function
                    matchEvaluator3 = matchEvaluator4
                    ModMain._Closure$__.$IR71-2 = matchEvaluator4
                End If
                Text = text12.RegexReplace(text13, matchEvaluator3, RegexOptions.None)
                Dim text14 As String = Text
                Dim text15 As String = "\{setup:([a-zA-Z0-9]+)\}"
                Dim matchEvaluator5 As MatchEvaluator
                If ModMain._Closure$__.$I71-5 IsNot Nothing Then
                    matchEvaluator5 = ModMain._Closure$__.$I71-5
                Else
                    Dim matchEvaluator6 As MatchEvaluator = Function(m As Match) ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Conversions.ToString(Settings.GetSafe(m.Groups(1).Value, ModMinecraft.McInstanceSelected)))
                    matchEvaluator5 = matchEvaluator6
                    ModMain._Closure$__.$I71-5 = matchEvaluator6
                End If
                Text = text14.RegexReplace(text15, matchEvaluator5, RegexOptions.None)
                Dim text16 As String = Text
                Dim text17 As String = "\{varible:([^:\}]+)(?::([^\}]+))?\}"
                Dim matchEvaluator7 As MatchEvaluator
                If ModMain._Closure$__.$I71-6 IsNot Nothing Then
                    matchEvaluator7 = ModMain._Closure$__.$I71-6
                Else
                    Dim matchEvaluator8 As MatchEvaluator = Function(m As Match) ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Conversions.ToString(RegistryUtils.TryRead(String.Format("HKEY_CURRENT_USER\Software\{0}", "PCL"), String.Format("CustomEvent{0}", m.Groups(1).Value), m.Groups(2).Value)))
                    matchEvaluator7 = matchEvaluator8
                    ModMain._Closure$__.$I71-6 = matchEvaluator8
                End If
                Text = text16.RegexReplace(text17, matchEvaluator7, RegexOptions.None)
                Dim text18 As String = Text
                Dim text19 As String = "\{variable:([^:\}]+)(?::([^\}]+))?\}"
                Dim matchEvaluator9 As MatchEvaluator
                If ModMain._Closure$__.$I71-7 IsNot Nothing Then
                    matchEvaluator9 = ModMain._Closure$__.$I71-7
                Else
                    Dim matchEvaluator10 As MatchEvaluator = Function(m As Match) ModMain.$STATIC$ArgumentReplace$03EE151280F92EE2$Replacer(Conversions.ToString(RegistryUtils.TryRead(String.Format("HKEY_CURRENT_USER\Software\{0}", "PCL"), String.Format("CustomEvent{0}", m.Groups(1).Value), m.Groups(2).Value)))
                    matchEvaluator9 = matchEvaluator10
                    ModMain._Closure$__.$I71-7 = matchEvaluator10
                End If
                Text = text18.RegexReplace(text19, matchEvaluator9, RegexOptions.None)
                text = Text
            End If
            Return text
        End Function

        Public Sub TryClearTaskTemp()
            If Not ModMain.IsTaskTempCleared Then
                ModMain.IsTaskTempCleared = True
                ModMain.IsTaskTempClearing = True
                Try
                    Logger.Info("开始清理任务缓存文件夹", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    DirectoryUtils.Delete(String.Format("{0}ProgramData\PCL\TaskTemp\", ModBase.OsDrive), False)
                    DirectoryUtils.Delete(String.Format("{0}TaskTemp\", ModBase.PathTemp), False)
                    Logger.Info("已清理任务缓存文件夹", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    Return
                Catch ex As Exception
                    Logger.Warn(ex, "清理任务缓存文件夹失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    Return
                Finally
                    ModMain.IsTaskTempClearing = False
                End Try
            End If
            If ModMain.IsTaskTempClearing Then
                While ModMain.IsTaskTempClearing
                    Thread.Sleep(1)
                End While
            End If
        End Sub

        Public Function RequestTaskTempFolder(Optional RequireNonSpace As Boolean = False) As String
            ModMain.TryClearTaskTemp()
            Dim text As String
            Try
                text = String.Format("{0}TaskTemp\{1}-{2}\", ModBase.PathTemp, ModBase.GetUuid(), ModBase.RandomInteger(0, 1000000))
                If Not RequireNonSpace OrElse Not text.Contains(" ") Then
                    DirectoryUtils.Create(text)
                    ModBase.CheckPermissionWithException(text)
                    Return text
                End If
            Catch ex As Exception
            End Try
            text = String.Format("{0}ProgramData\PCL\TaskTemp\{1}-{2}\", ModBase.OsDrive, ModBase.GetUuid(), ModBase.RandomInteger(0, 1000000))
            DirectoryUtils.Create(text)
            ModBase.CheckPermission(text)
            Return text
        End Function

        Public Sub Feedback(Optional ShowMsgbox As Boolean = True, Optional ForceOpenLog As Boolean = False)
            Dim num As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_0000:
                ProjectData.ClearProjectError()
                num = 1
                IL_0007:
                Dim num2 As Integer = 2
                ModMain.FeedbackInfo()
                IL_000E:
                num2 = 3
                If Not ForceOpenLog AndAlso (Not ShowMsgbox OrElse ModMain.OurMsgBox("若你在汇报一个 Bug，请点击 打开文件夹 按钮，并上传 Log(1~5).txt 中包含错误信息的文件。" & vbCrLf & "游戏崩溃一般与启动器无关，请不要因为游戏崩溃而提交反馈。", "反馈提交提醒", "打开文件夹", "不需要", "", False, True, False, Nothing, Nothing, Nothing) <> 1) Then
                    GoTo IL_0053
                End If
                IL_003D:
                num2 = 4
                ModBase.OpenExplorer(Paths.Base + "PCL\Log1.txt")
                IL_0053:
                num2 = 5
                ModBase.OpenWebsite("https://github.com/Meloong-Git/PCL/issues/")
                IL_005F:
                GoTo IL_00C6
                IL_0061:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_0087:
                GoTo IL_00BB
                IL_0089:
                num4 = num2
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                IL_0099:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_0089
            End Try
            IL_00BB:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_00C6:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Public Sub FeedbackInfo()
            Dim num As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_0000:
                ProjectData.ClearProjectError()
                num = 1
                IL_0007:
                Dim num2 As Integer = 2
                Logger.Warn(String.Concat(New String() { String.Format("诊断信息：{0}", vbCrLf), "操作系统：", MyWpfExtension.Computer.Info.OSFullName, vbCrLf & "剩余内存：", Conversions.ToString(Conversion.Int(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0)), " M / ", Conversions.ToString(Conversion.Int(MyWpfExtension.Computer.Info.TotalPhysicalMemory / 1024.0 / 1024.0)), " M" & vbCrLf & "DPI：", Conversions.ToString(ModBase.DPI), "（", Conversions.ToString(Math.Round(CDbl(ModBase.DPI) / 96.0, 2) * 100.0), "%）" & vbCrLf & "MC 文件夹：", If(ModMinecraft.McFolderSelected, "Nothing"), vbCrLf & "文件位置：", Paths.Base }), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                IL_0134:
                GoTo IL_018F
                IL_0136:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_0150:
                GoTo IL_0184
                IL_0152:
                num4 = num2
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                IL_0162:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_0152
            End Try
            IL_0184:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_018F:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Public Sub Telemetry([Event] As String, ParamArray Datas As String())
            ' 保留调用接口以兼容现有事件，禁用数据上报。
        End Sub

        Private Sub TimerMain()
            Try
                ModMain.HintTick()
                ModMain.OurMsgBoxTick()
                ModMain.FrmMain.DragTick()
                ModLoader.LoaderTaskbarProgressRefresh()
                If ModSecret.ThemeDontClick = 2 Then
                    ModSecret.ThemeRefresh(-1)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "短程主时钟执行异常", LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
            End Try
            ModMain.Timer4Count += 1
            If ModMain.Timer4Count = 4 Then
                ModMain.Timer4Count = 0
                Try
                    If ModSecret.ThemeNow = 12 Then
                        ModSecret.ThemeRefresh(-1)
                    End If
                Catch ex2 As Exception
                    Logger.Warn(ex2, "中程主时钟执行异常", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                End Try
            End If
            ModMain.Timer150Count += 1
            If ModMain.Timer150Count = 150 Then
                ModMain.Timer150Count = 0
                Try
                    If ModMain.FrmMain.BtnExtraApril_ShowCheck() AndAlso ModMain.AprilDifficultiedDistance <> 0 Then
                        ModMain.FrmMain.BtnExtraApril.Ribble()
                    End If
                    Dim action As Action
                    If ModMain._Closure$__.$I82-0 IsNot Nothing Then
                        action = ModMain._Closure$__.$I82-0
                    Else
                        Dim action2 As Action = Sub()
                            If Not ModMain.FrmMain.Hidden Then
                                If ModMain.FrmMain.Top < -9000.0 Then
                                    ModMain.FrmMain.Top = 100.0
                                End If
                                If ModMain.FrmMain.Left < -9000.0 Then
                                    ModMain.FrmMain.Left = 100.0
                                End If
                            End If
                        End Sub
                        action = action2
                        ModMain._Closure$__.$I82-0 = action2
                    End If
                    ModBase.RunInUi(action, False)
                Catch ex3 As Exception
                    Logger.[Error](ex3, "长程主时钟执行异常", LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                End Try
            End If
        End Sub

        Public Sub TimerMainStart()
            Dim action As Action
            If ModMain._Closure$__.$I83-0 IsNot Nothing Then
                action = ModMain._Closure$__.$I83-0
            Else
                Dim action2 As Action = Sub()
                    Try
                        While True
                            ModBase.RunInUiWait(AddressOf ModMain.TimerMain)
                            Thread.Sleep(49)
                        End While
                    Catch ex As Exception
                        Logger.[Error](ex, "程序主时钟出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                    End Try
                End Sub
                action = action2
                ModMain._Closure$__.$I83-0 = action2
            End If
            ModBase.RunInNewThread(action, "Timer Main", ThreadPriority.Normal)
            If ModMain.IsAprilEnabled Then
                Dim action3 As Action
                If ModMain._Closure$__.$I83-1 IsNot Nothing Then
                    action3 = ModMain._Closure$__.$I83-1
                Else
                    Dim action4 As Action = Sub()
                        Try
                            Dim num As Integer = MyWpfExtension.Computer.Clock.TickCount
                            While True
                                If num <> MyWpfExtension.Computer.Clock.TickCount Then
                                    num = MyWpfExtension.Computer.Clock.TickCount
                                    ModBase.RunInUiWait(AddressOf ModMain.TimerFool)
                                End If
                                Thread.Sleep(1)
                            End While
                        Catch ex2 As Exception
                            Logger.[Error](ex2, "愚人节主时钟出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModMain.vb")
                        End Try
                    End Sub
                    action3 = action4
                    ModMain._Closure$__.$I83-1 = action4
                End If
                ModBase.RunInNewThread(action3, "Timer Main Fool", ThreadPriority.Normal)
            End If
        End Sub

        Private HintWaiting As ConcurrentQueue(Of ModMain.HintMessage) = New ConcurrentQueue(Of ModMain.HintMessage)()

        Public WaitingOurMsgBox As ConcurrentQueue(Of ModMain.OurMsgBoxConverter) = New ConcurrentQueue(Of ModMain.OurMsgBoxConverter)()

        Public FrmMain As FormMain

        Public FrmStart As SplashScreen

        Public FrmLaunchLeft As PageLaunchLeft

        Public FrmLaunchRight As PageLaunchRight

        Public FrmSelectLeft As PageSelectLeft

        Public FrmSelectRight As PageSelectRight

        Public FrmSpeedLeft As PageSpeedLeft

        Public FrmSpeedRight As PageSpeedRight

        Public FrmLinkMain As PageLinkMain

        Public FrmDownloadLeft As PageDownloadLeft

        Public FrmDownloadInstall As PageDownloadInstall

        Public FrmDownloadMod As PageDownloadMod

        Public FrmDownloadPack As PageDownloadPack

        Public FrmDownloadDataPack As PageDownloadDataPack

        Public FrmDownloadShader As PageDownloadShader

        Public FrmDownloadResourcePack As PageDownloadResourcePack

        Public FrmSetupLeft As PageSetupLeft

        Public FrmSetupLaunch As PageSetupLaunch

        Public FrmSetupUI As PageSetupUI

        Public FrmSetupSystem As PageSetupSystem

        Public FrmSetupLink As PageSetupLink

        Public FrmOtherLeft As PageOtherLeft

        Public FrmOtherHelp As PageOtherHelp

        Public FrmOtherAbout As PageOtherAbout

        Public FrmOtherTest As PageOtherTest

        Public FrmLoginLegacy As PageLoginLegacy

        Public FrmLoginNide As PageLoginNide

        Public FrmLoginNideSkin As PageLoginNideSkin

        Public FrmLoginAuth As PageLoginAuth

        Public FrmLoginAuthSkin As PageLoginAuthSkin

        Public FrmLoginMs As PageLoginMs

        Public FrmLoginMsSkin As PageLoginMsSkin

        Public FrmInstanceLeft As PageInstanceLeft

        Public FrmInstanceOverall As PageInstanceOverall

        Public FrmInstanceMod As PageInstanceMod

        Public FrmInstanceModDisabled As PageInstanceModDisabled

        Public FrmInstanceSetup As PageInstanceSetup

        Public FrmInstanceExport As PageInstanceExport

        Public FrmDownloadModify As PageDownloadModify

        Public FrmDownloadResourceDetail As PageDownloadResourceDetail

        Public HelpLoader As ModLoader.LoaderTask(Of Integer, List(Of ModMain.HelpEntry)) = New ModLoader.LoaderTask(Of Integer, List(Of ModMain.HelpEntry))("Help Page", AddressOf ModMain.HelpLoad, Nothing, ThreadPriority.BelowNormal)

        Private HelpLoadLock As Object = RuntimeHelpers.GetObjectValue(New Object())

        Private _IsAprilEnabled As Boolean? = Nothing

        Public IsAprilGiveup As Boolean = False

        Private AprilDifficultiedDistance As Integer = 0

        Private IsTaskTempCleared As Boolean = False

        Private IsTaskTempClearing As Boolean = False

        Public DragControl As Object = Nothing

        Private Timer4Count As Integer = 0

        Private Timer150Count As Integer = 0

        Private $STATIC$TimerFool$001$IdieCount As Integer

        Private $STATIC$TimerFool$001$IdieCount$Init As StaticLocalInitFlag

        Private $STATIC$TimerFool$001$MousePosLast As Point

        Private $STATIC$TimerFool$001$MousePosLast$Init As StaticLocalInitFlag

        Private $STATIC$TimerFool$001$Speed As Vector

        Private $STATIC$TimerFool$001$Speed$Init As StaticLocalInitFlag

        Private $STATIC$TimerFool$001$GiveUpDistance As Double

        Private $STATIC$TimerFool$001$GiveUpDistance$Init As StaticLocalInitFlag

        Private $STATIC$ArgumentReplace$03EE151280F92EE2$Replacer As Func(Of String, String)

        Private $STATIC$ArgumentReplace$03EE151280F92EE2$Replacer$Init As StaticLocalInitFlag

        Public Enum HintType
            Blue
            Green
            Red
        End Enum

        Private Structure HintMessage
            Public Text As String

            Public Type As ModMain.HintType

            Public Log As Boolean
        End Structure

        Public Class OurMsgBoxConverter
            Public Sub New()
                Me.HintText = ""
                Me.Button1 = "确定"
                Me.Button2 = ""
                Me.Button3 = ""
                Me.Button1Action = Nothing
                Me.Button2Action = Nothing
                Me.Button3Action = Nothing
                Me.IsWarn = False
                Me.ForceWait = False
                Me.WaitFrame = New DispatcherFrame(True)
                Me.IsExited = False
            End Sub

            Public Type As ModMain.OurMsgBoxType

            Public Title As String

            Public Text As String

            Public Content As Object

            Public ValidateRules As Collection(Of Validate)

            Public HintText As String

            Public HighLight As Boolean

            Public Button1 As String

            Public Button2 As String

            Public Button3 As String

            Public Button1Action As Action

            Public Button2Action As Action

            Public Button3Action As Action

            Public IsWarn As Boolean

            Public ForceWait As Boolean

            Public WaitFrame As DispatcherFrame

            Public IsExited As Boolean

            Public Result As Object
        End Class

        Public Enum OurMsgBoxType
            Text
            [Select]
            Input
            Login
        End Enum

        Public Class HelpEntry
            Public Sub New(FilePath As String)
                Me.Logo = Nothing
                Me.ShowInSearch = True
                Me.ShowInPublic = True
                Me.ShowInSnapshot = True
                Me.RawPath = FilePath
                If Not FileUtils.Exists(FilePath) Then
                    Throw New FileNotFoundException("未找到帮助文件：" + FilePath, FilePath)
                End If
                Dim jobject As JObject = CType(ModMain.ArgumentReplace(FileUtils.ReadAsString(FilePath, Nothing, Nothing), New Func(Of String, String)(StringUtils.XmlEscape), True).DeserializeJson(), JObject)
                If jobject("Title") Is Nothing Then
                    Throw New ArgumentException("未找到 Title 项")
                End If
                Me.Title = CStr(jobject("Title"))
                Me.Desc = CStr((If(jobject("Description"), "")))
                Me.Search = CStr((If(jobject("Keywords"), "")))
                Me.Logo = CStr(jobject("Logo"))
                Me.ShowInSearch = CBool((If(jobject("ShowInSearch"), Me.ShowInSearch)))
                Me.ShowInPublic = CBool((If(jobject("ShowInPublic"), Me.ShowInPublic)))
                Me.ShowInSnapshot = CBool((If(jobject("ShowInSnapshot"), Me.ShowInSnapshot)))
                Me.Types = New List(Of String)()
                Try
                    For Each jtoken As JToken In (If(jobject("Types"), New JArray()))
                        Me.Types.Add(CStr(jtoken))
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If CType((If(jobject("IsEvent"), False)), Boolean?).GetValueOrDefault() Then
                    Me.EventType = jobject("EventType").ToObject(Of CustomEvent.EventType)()
                    Me.EventData = CStr((If(jobject("EventData"), "")))
                    Me.IsEvent = True
                    Return
                End If
                Dim text As String = Path.ChangeExtension(FilePath, ".xaml")
                If FileUtils.Exists(text) Then
                    Me.XamlContent = FileUtils.ReadAsString(text, Nothing, Nothing)
                    Me.IsEvent = False
                    Return
                End If
                Throw New FileNotFoundException("未找到帮助条目 .json 对应的 .xaml 文件（" + text + "）")
            End Sub

            Public Function ToListItem() As OurListItem
                Return Me.SetToListItem(New OurListItem())
            End Function

            Public Function SetToListItem(Item As OurListItem) As OurListItem
                Dim text As String
                If Me.IsEvent Then
                    If Me.EventType = CustomEvent.EventType.弹出窗口 Then
                        text = ModBase.PathImage + "Blocks/GrassPath.png"
                    Else
                        text = ModBase.PathImage + "Blocks/CommandBlock.png"
                    End If
                Else
                    text = ModBase.PathImage + "Blocks/Grass.png"
                End If
                Item.SnapsToDevicePixels = True
                Item.Title = Me.Title
                Item.Info = Me.Desc
                Item.Logo = If(Me.Logo, text)
                Item.Height = 42.0
                Item.Type = OurListItem.CheckType.Clickable
                Item.Tag = Me
                CustomEventService.SetEventType(Item, CustomEvent.EventType.None)
                CustomEventService.SetEventData(Item, Nothing)
                Dim clickEventHandler As OurListItem.ClickEventHandler
                If ModMain.HelpEntry._Closure$__.$I15-0 IsNot Nothing Then
                    clickEventHandler = ModMain.HelpEntry._Closure$__.$I15-0
                Else
                    Dim clickEventHandler2 As OurListItem.ClickEventHandler = Sub(sender As Object, e As MouseButtonEventArgs)
                        PageOtherHelp.OnItemClick(CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModMain.HelpEntry))
                    End Sub
                    clickEventHandler = clickEventHandler2
                    ModMain.HelpEntry._Closure$__.$I15-0 = clickEventHandler2
                End If
                AddHandler Item.Click, clickEventHandler
                Return Item
            End Function

            Public RawPath As String

            Public Title As String

            Public Desc As String

            Public Search As String

            Public Types As List(Of String)

            Public Logo As String

            Public ShowInSearch As Boolean

            Public ShowInPublic As Boolean

            Public ShowInSnapshot As Boolean

            Public IsEvent As Boolean

            Public EventType As CustomEvent.EventType

            Public EventData As String

            Public XamlContent As String
        End Class
    End Module
End Namespace
