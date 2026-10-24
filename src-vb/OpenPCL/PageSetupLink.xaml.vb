Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSetupLink
        Inherits OurPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageSetupLink_Loaded
            Me.InitializeComponent()
        End Sub

        Private Sub PageSetupLink_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            If Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init, flag)
                If Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State = 0S Then
                    Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State = 2S
                    Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded = False
                ElseIf Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init)
                End If
            End Try
            If Not Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded Then
                Me.$STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded = True
                ModAnimation.AniControlEnabled += 1
                Me.Reload()
                ModAnimation.AniControlEnabled -= 1
            End If
        End Sub

        Public Sub Reload()
            SettingService.RefreshSettings(Me)
        End Sub

        Public Sub Reset()
            Try
                SettingService.ResetSettings(Me)
                Logger.Info("已初始化联机页设置", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLink.xaml.vb")
                ModMain.Hint("已初始化联机页设置！", ModMain.HintType.Green, False)
            Catch ex As Exception
                Logger.[Error](ex, "初始化联机页设置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSetup\PageSetupLink.xaml.vb")
            End Try
            Me.Reload()
        End Sub

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property CardArgument As OurCard

        Friend Overridable Property ComboLatencyMode As OurComboBox

        Friend Overridable Property TextCustomPeer As OurTextBox

        Private $STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded As Boolean

        Private $STATIC$PageSetupLink_Loaded$20211C128141$IsLoaded$Init As StaticLocalInitFlag
    End Class
End Namespace
