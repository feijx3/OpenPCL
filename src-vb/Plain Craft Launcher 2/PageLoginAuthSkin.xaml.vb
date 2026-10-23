Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLoginAuthSkin
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.PageLoginLegacy_Loaded()
            End Sub
            Me.InitializeComponent()
            Me.Skin.Loader = PageLaunchLeft.SkinAuth
        End Sub

        Private Sub PageLoginLegacy_Loaded()
            Me.Skin.Loader.Start(Nothing, False)
        End Sub

        Public Sub Reload(KeepInput As Boolean)
            Me.TextName.Text = Settings.[Get](Of String)("CacheAuthName", Nothing)
            Me.TextEmail.Text = Settings.[Get](Of String)("CacheAuthUsername", Nothing)
            Me.TextEmail.Visibility = If(Settings.[Get](Of Boolean)("UiLauncherEmail", Nothing), Visibility.Collapsed, Visibility.Visible)
            Me.PageLoginLegacy_Loaded()
        End Sub

        Public Shared Function GetLoginData() As ModLaunch.McLoginServer
            Dim text As String = If(Information.IsNothing(ModMinecraft.McInstanceSelected), Settings.[Get](Of String)("CacheAuthServerServer", Nothing), Settings.[Get](Of String)("VersionServerAuthServer", ModMinecraft.McInstanceSelected)) + "/authserver"
            Return New ModLaunch.McLoginServer(ModLaunch.McLoginType.Auth) With { .Token = "Auth", .BaseUrl = text, .UserName = Settings.[Get](Of String)("CacheAuthUsername", Nothing), .Password = Settings.[Get](Of String)("CacheAuthPass", Nothing), .Description = "Authlib-Injector", .Type = ModLaunch.McLoginType.Auth }
        End Function

        Private Sub PageLoginAuthSkin_MouseEnter(sender As Object, e As MouseEventArgs)
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.BtnEdit, 1.0 - Me.BtnEdit.Opacity, 80, 0, Nothing, False), ModAnimation.AaHeight(Me.BtnEdit, 25.5 - Me.BtnEdit.Height, 140, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnEdit, -1.5, 50, 140, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.BtnExit, 1.0 - Me.BtnExit.Opacity, 80, 0, Nothing, False), ModAnimation.AaHeight(Me.BtnExit, 25.5 - Me.BtnExit.Height, 140, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnExit, -1.5, 50, 140, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False) }, "PageLoginAuthSkin Button", False)
        End Sub

        Private Sub PageLoginAuthSkin_MouseLeave(sender As Object, e As MouseEventArgs)
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.BtnEdit, -Me.BtnEdit.Opacity, 120, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnEdit, 14.0 - Me.BtnEdit.Height, 120, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaOpacity(Me.BtnExit, -Me.BtnExit.Opacity, 120, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaHeight(Me.BtnExit, 14.0 - Me.BtnExit.Height, 120, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False) }, "PageLoginAuthSkin Button", False)
        End Sub

        Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
            If ModLaunch.McLoginLoader.State <> ModBase.LoadState.Loading Then
                ModMain.Hint("正在尝试更换，请稍候！", ModMain.HintType.Blue, True)
                Settings.[Set]("CacheAuthUuid", "", Nothing)
                Settings.[Set]("CacheAuthName", "", Nothing)
                ModBase.RunInThread(Sub()
                    Try
                        Dim loginData As ModLaunch.McLoginServer = PageLoginAuthSkin.GetLoginData()
                        loginData.ForceReselectProfile = True
                        ModLaunch.McLoginLoader.WaitForExit(loginData, Nothing, True, Nothing, Nothing)
                        ModBase.RunInUi(Sub()
                            Me.Reload(True)
                        End Sub, False)
                    Catch ex As Exception
                        Logger.[Error](ex, "更换角色失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\PageLoginAuthSkin.xaml.vb")
                    End Try
                End Sub)
                Return
            End If
            Logger.Warn("要求更换角色，但登录加载器繁忙", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\PageLoginAuthSkin.xaml.vb")
            If TypeOf ModLaunch.McLoginLoader.Input Is ModLaunch.McLoginServer AndAlso CType(ModLaunch.McLoginLoader.Input, ModLaunch.McLoginServer).ForceReselectProfile Then
                ModMain.Hint("正在尝试更换，请稍候！", ModMain.HintType.Blue, True)
                Return
            End If
            ModMain.Hint("正在登录中，请稍后再更换角色！", ModMain.HintType.Red, True)
        End Sub

        Public Shared Sub ExitLogin()
            Settings.[Set]("CacheAuthAccess", "", Nothing)
            Settings.[Set]("CacheAuthUuid", "", Nothing)
            Settings.[Set]("CacheAuthName", "", Nothing)
            ModLaunch.McLoginAuthLoader.Input = Nothing
            ModMain.FrmLaunchLeft.RefreshPage(False, True)
        End Sub

        Private Sub Skin_Click(sender As Object, e As MouseButtonEventArgs)
            Dim text As String = If((ModMinecraft.McInstanceSelected IsNot Nothing), Settings.[Get](Of String)("VersionServerAuthRegister", ModMinecraft.McInstanceSelected), Settings.[Get](Of String)("CacheAuthServerRegister", Nothing))
            If String.IsNullOrEmpty(New ValidateHttp().Validate(text)) Then
                ModBase.OpenWebsite(text.Replace("/auth/register", "/user/closet"))
            End If
        End Sub

        Friend Overridable Property PanData As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanData
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim mouseEventHandler As MouseEventHandler = AddressOf Me.PageLoginAuthSkin_MouseEnter
                Dim mouseEventHandler2 As MouseEventHandler = AddressOf Me.PageLoginAuthSkin_MouseLeave
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
            <CompilerGenerated()>
            Get
                Return Me._BtnEdit
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.BtnEdit_Click
                Dim myIconButton As MyIconButton = Me._BtnEdit
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnEdit = value
                myIconButton = Me._BtnEdit
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnExit As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExit
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler
                If PageLoginAuthSkin._Closure$__.$IR27-2 IsNot Nothing Then
                    clickEventHandler = PageLoginAuthSkin._Closure$__.$IR27-2
                Else
                    Dim clickEventHandler2 As MyIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                        PageLoginAuthSkin.ExitLogin()
                    End Sub
                    clickEventHandler = clickEventHandler2
                    PageLoginAuthSkin._Closure$__.$IR27-2 = clickEventHandler2
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
