Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSetupSystem
        Inherits MyPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupSystem_Loaded
            Me.InitializeComponent()
        End Sub

        Private Sub PageSetupSystem_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            Me.PanDonate.Visibility = Visibility.Visible
            Me.ItemSystemUpdateDownload.Content = "在有新版本时自动下载（更新快照版前需要先输入土豆码）"
            If Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init, flag)
                If Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State = 0S Then
                    Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State = 2S
                    Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded = False
                ElseIf Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init)
                End If
            End Try
            If Not Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded Then
                Me.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded = True
                ModAnimation.AniControlEnabled += 1
                Me.Reload()
                Me.SliderLoad()
                ModAnimation.AniControlEnabled -= 1
            End If
        End Sub

        Public Sub Reload()
            SettingService.RefreshSettings(Me)
        End Sub

        Public Sub Reset()
            Try
                SettingService.ResetSettings(Me)
                Logger.Info("已初始化启动器页设置", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupSystem.xaml.vb")
                ModMain.Hint("已初始化启动器页设置！", ModMain.HintType.Green, False)
            Catch ex As Exception
                Logger.[Error](ex, "初始化启动器页设置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupSystem.xaml.vb")
            End Try
            Me.Reload()
        End Sub

        Private Sub SliderLoad()
            Dim sliderDownloadThread As MySlider = Me.SliderDownloadThread
            Dim [delegate] As [Delegate]
            If PageSetupSystem._Closure$__.$I4-0 IsNot Nothing Then
                [delegate] = PageSetupSystem._Closure$__.$I4-0
            Else
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.AddObject(v, 1)
                [delegate] = vb$AnonymousDelegate_
                PageSetupSystem._Closure$__.$I4-0 = vb$AnonymousDelegate_
            End If
            sliderDownloadThread.GetHintText = [delegate]
            Dim sliderDownloadSpeed As MySlider = Me.SliderDownloadSpeed
            Dim delegate2 As [Delegate]
            If PageSetupSystem._Closure$__.$I4-1 IsNot Nothing Then
                delegate2 = PageSetupSystem._Closure$__.$I4-1
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object)
                    Dim obj As Object
                    If Operators.ConditionalCompareObjectLessEqual(v, 14, False) Then
                        obj = Operators.ConcatenateObject(Operators.MultiplyObject(Operators.AddObject(v, 1), 0.1), " M/s")
                    ElseIf Operators.ConditionalCompareObjectLessEqual(v, 31, False) Then
                        obj = Operators.ConcatenateObject(Operators.MultiplyObject(Operators.SubtractObject(v, 11), 0.5), " M/s")
                    ElseIf Operators.ConditionalCompareObjectLessEqual(v, 41, False) Then
                        obj = Operators.ConcatenateObject(Operators.SubtractObject(v, 21), " M/s")
                    Else
                        obj = "无限制"
                    End If
                    Return obj
                End Function
                delegate2 = vb$AnonymousDelegate_2
                PageSetupSystem._Closure$__.$I4-1 = vb$AnonymousDelegate_2
            End If
            sliderDownloadSpeed.GetHintText = delegate2
            Dim sliderDebugAnim As MySlider = Me.SliderDebugAnim
            Dim delegate3 As [Delegate]
            If PageSetupSystem._Closure$__.$I4-2 IsNot Nothing Then
                delegate3 = PageSetupSystem._Closure$__.$I4-2
            Else
                Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object)
                    If Not Operators.ConditionalCompareObjectGreater(v, 29, False) Then
                        Return Operators.ConcatenateObject(Operators.AddObject(Operators.DivideObject(v, 10), 0.1), "x")
                    End If
                    Return "关闭"
                End Function
                delegate3 = vb$AnonymousDelegate_3
                PageSetupSystem._Closure$__.$I4-2 = vb$AnonymousDelegate_3
            End If
            sliderDebugAnim.GetHintText = delegate3
        End Sub

        Private Sub SliderDownloadThread_PreviewChange(sender As Object, e As ModBase.RouteEventArgs)
            If Me.SliderDownloadThread.Value >= 100 AndAlso Not Settings.[Get](Of Boolean)("HintDownloadThread", Nothing) Then
                Settings.[Set]("HintDownloadThread", True, Nothing)
                ModMain.MyMsgBox("如果设置过多的下载线程，可能会导致下载时出现非常严重的卡顿。" & vbCrLf & "一般设置 64 线程即可满足大多数下载需求，除非你知道你在干什么，否则不建议设置更多的线程数！", "警告", "我知道了", "", "", True, True, False, Nothing, Nothing, Nothing)
            End If
        End Sub

        Private Sub BtnSystemIdentify_Click(sender As Object, e As MouseButtonEventArgs)
            PageOtherAbout.CopyIdentify()
        End Sub

        Private Sub BtnSystemUnlock_Click(sender As Object, e As MouseButtonEventArgs)
            ModSecret.InputPotatoCode(False)
        End Sub

        Private Sub CheckDebugMode_Change()
            If ModAnimation.AniControlEnabled = 0 Then
                ModMain.Hint("部分调试信息将在刷新或启动器重启后切换显示！", ModMain.HintType.Blue, False)
            End If
        End Sub

        Private Sub ComboSystemActivity_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.ComboSystemActivity.SelectedIndex = 2 AndAlso ModMain.MyMsgBox("若选择此项，即使在将来出现严重问题时，你也无法获取相关通知。" & vbCrLf & "例如，如果发现某个版本游戏存在严重 Bug，你可能就会因为无法得到通知而导致无法预知的后果。" & vbCrLf & vbCrLf & "一般选择 仅在有重要通知时显示公告 就可以让你尽量不受打扰了。" & vbCrLf & "除非你在制作服务器整合包，或时常手动更新启动器，否则极度不推荐选择此项！", "警告", "我知道我在做什么", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 2 AndAlso e.RemovedItems.Count > 0 Then
                Me.ComboSystemActivity.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems(0))
            End If
        End Sub

        Private Sub ComboSystemUpdate_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.ComboSystemUpdate.SelectedIndex = 3 AndAlso ModMain.MyMsgBox("若选择此项，即使在启动器将来出现严重问题时，你也无法获取更新并获得修复。" & vbCrLf & "例如，如果官方修改了登录方式，从而导致现有启动器无法登录，你可能就会因为无法更新而无法开始游戏。" & vbCrLf & vbCrLf & "一般选择 仅在有重大漏洞更新时显示提示 就可以让你尽量不受打扰了。" & vbCrLf & "除非你在制作服务器整合包，或时常手动更新启动器，否则极度不推荐选择此项！", "警告", "我知道我在做什么", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 2 AndAlso e.RemovedItems.Count > 0 Then
                Me.ComboSystemUpdate.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems(0))
            End If
        End Sub

        Private Sub BtnSystemUpdate_Click(sender As Object, e As EventArgs)
            ModSecret.UpdateCheckByButton()
        End Sub

        Public Shared Function IsLauncherNewest() As Boolean?
            Dim flag As Boolean?
            Try
                Dim text As String = If(FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\Notice.cfg", Nothing, Nothing), "")
                If text.Split("|", False).Count() < 3 Then
                    flag = Nothing
                Else
                    flag = New Boolean?(Conversions.ToDouble(text.Split("|", False)(1)) <= 414.0)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "确认启动器更新失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSetup\PageSetupSystem.xaml.vb")
                flag = Nothing
            End Try
            Return flag
        End Function

        Private Sub BtnSystemSettingExp_Click(sender As Object, e As MouseButtonEventArgs)
            ModMain.Hint("该功能尚在开发中！", ModMain.HintType.Blue, True)
        End Sub

        Private Sub BtnSystemSettingImp_Click(sender As Object, e As MouseButtonEventArgs)
            ModMain.Hint("该功能尚在开发中！", ModMain.HintType.Blue, True)
        End Sub

        Friend Overridable Property PanBack As MyScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property ComboDownloadSource As MyComboBox

        Friend Overridable Property ComboDownloadVersion As MyComboBox

        Friend Overridable Property SliderDownloadThread As MySlider
            <CompilerGenerated()>
            Get
                Return Me._SliderDownloadThread
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MySlider)
                Dim previewChangeEventHandler As MySlider.PreviewChangeEventHandler = AddressOf Me.SliderDownloadThread_PreviewChange
                Dim mySlider As MySlider = Me._SliderDownloadThread
                If mySlider IsNot Nothing Then
                    RemoveHandler mySlider.PreviewChange, previewChangeEventHandler
                End If
                Me._SliderDownloadThread = value
                mySlider = Me._SliderDownloadThread
                If mySlider IsNot Nothing Then
                    AddHandler mySlider.PreviewChange, previewChangeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property SliderDownloadSpeed As MySlider

        Friend Overridable Property ComboDownloadMod As MyComboBox

        Friend Overridable Property ComboDownloadTranslateV2 As MyComboBox

        Friend Overridable Property ComboModLocalNameStyle As MyComboBox

        Friend Overridable Property CheckUpdateRelease As MyCheckBox

        Friend Overridable Property CheckUpdateSnapshot As MyCheckBox

        Friend Overridable Property CheckHelpChinese As MyCheckBox

        Friend Overridable Property ComboSystemUpdate As MyComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboSystemUpdate
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = AddressOf Me.ComboSystemUpdate_SelectionChanged
                Dim myComboBox As MyComboBox = Me._ComboSystemUpdate
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboSystemUpdate = value
                myComboBox = Me._ComboSystemUpdate
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ItemSystemUpdateDownload As MyComboBoxItem

        Friend Overridable Property ComboSystemActivity As MyComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboSystemActivity
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = AddressOf Me.ComboSystemActivity_SelectionChanged
                Dim myComboBox As MyComboBox = Me._ComboSystemActivity
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboSystemActivity = value
                myComboBox = Me._ComboSystemActivity
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property TextSystemCache As MyTextBox

        Friend Overridable Property CheckSystemTelemetry As MyCheckBox

        Friend Overridable Property BtnSystemUpdate As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSystemUpdate
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnSystemUpdate_Click
                Dim myButton As MyButton = Me._BtnSystemUpdate
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSystemUpdate = value
                myButton = Me._BtnSystemUpdate
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSystemSettingExp As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSystemSettingExp
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnSystemSettingExp_Click
                Dim myButton As MyButton = Me._BtnSystemSettingExp
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSystemSettingExp = value
                myButton = Me._BtnSystemSettingExp
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSystemSettingImp As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSystemSettingImp
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnSystemSettingImp_Click
                Dim myButton As MyButton = Me._BtnSystemSettingImp
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSystemSettingImp = value
                myButton = Me._BtnSystemSettingImp
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property PanDonate As Grid

        Friend Overridable Property BtnSystemIdentify As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSystemIdentify
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnSystemIdentify_Click
                Dim myButton As MyButton = Me._BtnSystemIdentify
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSystemIdentify = value
                myButton = Me._BtnSystemIdentify
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSystemUnlock As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSystemUnlock
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnSystemUnlock_Click
                Dim myButton As MyButton = Me._BtnSystemUnlock
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnSystemUnlock = value
                myButton = Me._BtnSystemUnlock
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CardDebug As MyCard

        Friend Overridable Property PanDebugAnim As Grid

        Friend Overridable Property SliderDebugAnim As MySlider

        Friend Overridable Property CheckDebugSkipCopy As MyCheckBox

        Friend Overridable Property CheckDebugMode As MyCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckDebugMode
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCheckBox)
                Dim changeEventHandler As MyCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.CheckDebugMode_Change()
                End Sub
                Dim myCheckBox As MyCheckBox = Me._CheckDebugMode
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckDebugMode = value
                myCheckBox = Me._CheckDebugMode
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckDebugDelay As MyCheckBox

        Private $STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded As Boolean

        Private $STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init As StaticLocalInitFlag
    End Class
End Namespace
