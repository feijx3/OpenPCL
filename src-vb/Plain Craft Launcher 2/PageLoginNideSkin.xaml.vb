Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLoginNideSkin
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageLoginLegacy_Loaded
            Me.InitializeComponent()
            Me.Skin.Loader = PageLaunchLeft.SkinNide
        End Sub

        Private Sub PageLoginLegacy_Loaded(sender As Object, e As RoutedEventArgs)
            Me.Skin.Loader.Start(Nothing, False)
        End Sub

        Public Sub Reload(KeepInput As Boolean)
            Me.TextName.Text = Settings.[Get](Of String)("CacheNideName", Nothing)
            Me.TextEmail.Text = Settings.[Get](Of String)("CacheNideUsername", Nothing)
            Me.TextEmail.Visibility = (Not Settings.[Get](Of Boolean)("UiLauncherEmail", Nothing)).ToVisibility()
        End Sub

        Public Shared Function GetLoginData() As ModLaunch.McLoginServer
            Dim text As String = If(Information.IsNothing(ModMinecraft.McInstanceSelected), Settings.[Get](Of String)("CacheNideServer", Nothing), Settings.[Get](Of String)("VersionServerNide", ModMinecraft.McInstanceSelected))
            Return New ModLaunch.McLoginServer(ModLaunch.McLoginType.Nide) With { .Token = "Nide", .UserName = Settings.[Get](Of String)("CacheNideUsername", Nothing), .Password = Settings.[Get](Of String)("CacheNidePass", Nothing), .Description = "统一通行证", .Type = ModLaunch.McLoginType.Nide, .BaseUrl = "https://auth.mc-user.com:233/" + text + "/authserver" }
        End Function

        Private Sub PageLoginNideSkin_MouseEnter(sender As Object, e As MouseEventArgs)
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.BtnEdit, 1.0 - Me.BtnEdit.Opacity, 80, 0, Nothing, False), ModAnimation.AaHeight(Me.BtnEdit, 25.5 - Me.BtnEdit.Height, 140, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnEdit, -1.5, 50, 140, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.BtnExit, 1.0 - Me.BtnExit.Opacity, 80, 0, Nothing, False), ModAnimation.AaHeight(Me.BtnExit, 25.5 - Me.BtnExit.Height, 140, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnExit, -1.5, 50, 140, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False) }, "PageLoginNideSkin Button", False)
        End Sub

        Private Sub PageLoginNideSkin_MouseLeave(sender As Object, e As MouseEventArgs)
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.BtnEdit, -Me.BtnEdit.Opacity, 120, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnEdit, 14.0 - Me.BtnEdit.Height, 120, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.BtnExit, -Me.BtnExit.Opacity, 120, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnExit, 14.0 - Me.BtnExit.Height, 120, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False) }, "PageLoginNideSkin Button", False)
        End Sub

        Public Shared Sub ExitLogin()
            Settings.[Set]("CacheNideAccess", "", Nothing)
            ModLaunch.McLoginNideLoader.Input = Nothing
            ModMain.FrmLaunchLeft.RefreshPage(False, True)
        End Sub

        Private Sub Skin_Click(sender As Object, e As MouseButtonEventArgs)
            ModBase.OpenWebsite("https://login.mc-user.com:233/" + If(Information.IsNothing(ModMinecraft.McInstanceSelected), Settings.[Get](Of String)("CacheNideServer", Nothing), Settings.[Get](Of String)("VersionServerNide", ModMinecraft.McInstanceSelected)) + "/skin")
        End Sub

        Friend Overridable Property PanData As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanData
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim mouseEventHandler As MouseEventHandler = AddressOf Me.PageLoginNideSkin_MouseEnter
                Dim mouseEventHandler2 As MouseEventHandler = AddressOf Me.PageLoginNideSkin_MouseLeave
                Dim grid As Grid = Me._PanData
                If grid IsNot Nothing Then
                    RemoveHandler grid.MouseEnter, mouseEventHandler
                    RemoveHandler grid.MouseLeave, mouseEventHandler2
                End If
                Me._PanData = value
                grid = Me._PanData
                If grid IsNot Nothing Then
                    AddHandler grid.MouseEnter, mouseEventHandler
                    AddHandler grid.MouseLeave, mouseEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property TextName As TextBlock

        Friend Overridable Property TextEmail As TextBlock

        Friend Overridable Property BtnEdit As MyIconButton

        Friend Overridable Property BtnExit As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExit
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler
                If PageLoginNideSkin._Closure$__.$IR26-1 IsNot Nothing Then
                    clickEventHandler = PageLoginNideSkin._Closure$__.$IR26-1
                Else
                    Dim clickEventHandler2 As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                        PageLoginNideSkin.ExitLogin()
                    End Sub
                    clickEventHandler = clickEventHandler2
                    PageLoginNideSkin._Closure$__.$IR26-1 = clickEventHandler2
                End If
                Dim clickEventHandler3 As MyIconButton.ClickEventHandler = clickEventHandler
                Dim myIconButton As MyIconButton = Me._BtnExit
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler3
                End If
                Me._BtnExit = value
                myIconButton = Me._BtnExit
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler3
                End If
            End Set
        End Property

        Friend Overridable Property Skin As MySkin
            <CompilerGenerated()>
            Get
                Return Me._Skin
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MySkin)
                Dim clickEventHandler As MySkin.ClickEventHandler = AddressOf Me.Skin_Click
                Dim mySkin As MySkin = Me._Skin
                If mySkin IsNot Nothing Then
                    RemoveHandler mySkin.Click, clickEventHandler
                End If
                Me._Skin = value
                mySkin = Me._Skin
                If mySkin IsNot Nothing Then
                    AddHandler mySkin.Click, clickEventHandler
                End If
            End Set
        End Property
    End Class
End Namespace
