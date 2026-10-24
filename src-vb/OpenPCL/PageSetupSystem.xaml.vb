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
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupSystem_Loaded
            Me.InitializeComponent()
        End Sub

        Private Sub PageSetupSystem_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
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
                Logger.Info("已初始化启动器页设置", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupSystem.xaml.vb")
                ModMain.Hint("已初始化启动器页设置！", ModMain.HintType.Green, False)
            Catch ex As Exception
                Logger.[Error](ex, "初始化启动器页设置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupSystem.xaml.vb")
            End Try
            Me.Reload()
        End Sub

        Private Sub SliderLoad()
            Dim sliderDownloadThread As OurSlider = Me.SliderDownloadThread
            Dim [delegate] As [Delegate]
            If PageSetupSystem._Closure$__.$I4-0 IsNot Nothing Then
                [delegate] = PageSetupSystem._Closure$__.$I4-0
            Else
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_25(Of Object, Object) = Function(v As Object) Operators.AddObject(v, 1)
                [delegate] = vb$AnonymousDelegate_
                PageSetupSystem._Closure$__.$I4-0 = vb$AnonymousDelegate_
            End If
            sliderDownloadThread.GetHintText = [delegate]
            Dim sliderDownloadSpeed As OurSlider = Me.SliderDownloadSpeed
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
            Dim sliderDebugAnim As OurSlider = Me.SliderDebugAnim
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
                ModMain.OurMsgBox("如果设置过多的下载线程，可能会导致下载时出现非常严重的卡顿。" & vbCrLf & "一般设置 64 线程即可满足大多数下载需求，除非你知道你在干什么，否则不建议设置更多的线程数！", "警告", "我知道了", "", "", True, True, False, Nothing, Nothing, Nothing)
            End If
        End Sub



        Private Sub CheckDebugMode_Change()
            If ModAnimation.AniControlEnabled = 0 Then
                ModMain.Hint("部分调试信息将在刷新或启动器重启后切换显示！", ModMain.HintType.Blue, False)
            End If
        End Sub




        Public Shared Function IsLauncherNewest() As Boolean?
            ' 此构建不再与上游启动器比较版本。
            Return Nothing
        End Function



        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property ComboDownloadSource As OurComboBox

        Friend Overridable Property ComboDownloadVersion As OurComboBox

        Friend Overridable Property SliderDownloadThread As OurSlider
            <CompilerGenerated()>
            Get
                Return Me._SliderDownloadThread
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurSlider)
                Dim previewChangeEventHandler As OurSlider.PreviewChangeEventHandler = AddressOf Me.SliderDownloadThread_PreviewChange
                Dim mySlider As OurSlider = Me._SliderDownloadThread
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

        Friend Overridable Property SliderDownloadSpeed As OurSlider

        Friend Overridable Property ComboDownloadMod As OurComboBox

        Friend Overridable Property ComboDownloadTranslateV2 As OurComboBox

        Friend Overridable Property ComboModLocalNameStyle As OurComboBox

        Friend Overridable Property CheckUpdateRelease As OurCheckBox

        Friend Overridable Property CheckUpdateSnapshot As OurCheckBox

        Friend Overridable Property CheckHelpChinese As OurCheckBox



















        Friend Overridable Property CardDebug As OurCard

        Friend Overridable Property PanDebugAnim As Grid

        Friend Overridable Property SliderDebugAnim As OurSlider

        Friend Overridable Property CheckDebugSkipCopy As OurCheckBox

        Friend Overridable Property CheckDebugMode As OurCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckDebugMode
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCheckBox)
                Dim changeEventHandler As OurCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.CheckDebugMode_Change()
                End Sub
                Dim myCheckBox As OurCheckBox = Me._CheckDebugMode
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

        Friend Overridable Property CheckDebugDelay As OurCheckBox

        Private $STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded As Boolean

        Private $STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init As StaticLocalInitFlag
    End Class
End Namespace
