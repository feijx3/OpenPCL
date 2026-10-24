Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Net
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLoginMsSkin
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageLoginLegacy_Loaded
            Me.IsChanging = False
            Me.InitializeComponent()
            Me.Skin.Loader = PageLaunchLeft.SkinMs
        End Sub

        Private Sub PageLoginLegacy_Loaded(sender As Object, e As RoutedEventArgs)
            Me.Skin.Loader.Start(Nothing, False)
        End Sub

        Public Sub Reload(KeepInput As Boolean)
            Me.TextName.Text = Settings.[Get](Of String)("CacheMsV2Name", Nothing)
        End Sub

        Public Shared Function GetLoginData() As ModLaunch.McLoginMs
            Dim mcLoginMs As ModLaunch.McLoginMs
            If ModLaunch.McLoginMsLoader.State = ModBase.LoadState.Finished Then
                mcLoginMs = New ModLaunch.McLoginMs() With { .OAuthRefreshToken = Settings.[Get](Of String)("CacheMsV2OAuthRefresh", Nothing), .UserName = Settings.[Get](Of String)("CacheMsV2Name", Nothing), .AccessToken = Settings.[Get](Of String)("CacheMsV2Access", Nothing), .Uuid = Settings.[Get](Of String)("CacheMsV2Uuid", Nothing), .ProfileJson = Settings.[Get](Of String)("CacheMsV2ProfileJson", Nothing) }
            Else
                mcLoginMs = New ModLaunch.McLoginMs() With { .OAuthRefreshToken = Settings.[Get](Of String)("CacheMsV2OAuthRefresh", Nothing), .UserName = Settings.[Get](Of String)("CacheMsV2Name", Nothing) }
            End If
            Return mcLoginMs
        End Function

        Private Sub ShowPanel(sender As Object, e As MouseEventArgs)
            ModAnimation.AniStart(ModAnimation.AaOpacity(Me.PanButtons, 1.0 - Me.PanButtons.Opacity, 120, 0, Nothing, False), "PageLoginMsSkin Button", False)
        End Sub

        Public Sub HidePanel(sender As Object, e As EventArgs)
            If Not Me.BtnEdit.ContextMenu.IsOpen AndAlso Not Me.BtnSkin.ContextMenu.IsOpen AndAlso Not Me.PanData.IsMouseOver Then
                ModAnimation.AniStart(ModAnimation.AaOpacity(Me.PanButtons, -Me.PanButtons.Opacity, 120, 0, Nothing, False), "PageLoginMsSkin Button", False)
            End If
        End Sub

        Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
            Me.BtnEdit.ContextMenu.IsOpen = True
        End Sub

        Private Sub BtnExit_Click()
            Settings.[Set]("CacheMsV2OAuthRefresh", "", Nothing)
            Settings.[Set]("CacheMsV2Access", "", Nothing)
            Settings.[Set]("CacheMsV2ProfileJson", "", Nothing)
            Settings.[Set]("CacheMsV2Uuid", "", Nothing)
            Settings.[Set]("CacheMsV2Name", "", Nothing)
            Settings.[Set]("CacheMsV2Expires", 0L, Nothing)
            ModLaunch.McLoginMsLoader.Cancel()
            ModMain.FrmLaunchLeft.RefreshPage(False, True)
        End Sub

        Private Sub BtnSkin_Click(sender As Object, e As RoutedEventArgs)
            Me.BtnSkin.ContextMenu.IsOpen = True
        End Sub

        Public Sub BtnSkinEdit_Click(sender As Object, e As RoutedEventArgs)
            If Me.IsChanging Then
                ModMain.Hint("正在更改皮肤中，请稍候！", ModMain.HintType.Blue, True)
                Return
            End If
            If ModLaunch.McLoginLoader.State = ModBase.LoadState.Failed Then
                ModMain.Hint("登录失败，无法更改皮肤！", ModMain.HintType.Red, True)
                Return
            End If
            Dim SkinInfo As ModMinecraft.McSkinInfo = ModMinecraft.McSkinSelect()
            If SkinInfo.IsVaild Then
                ModMain.Hint("正在更改皮肤……", ModMain.HintType.Blue, True)
                Me.IsChanging = True
                ModBase.RunInNewThread(Sub()
                    Me.EditSkin(SkinInfo)
                End Sub, "Ms Skin Upload", ThreadPriority.Normal)
            End If
        End Sub

        Private Sub EditSkin(SkinInfo As ModMinecraft.McSkinInfo)
            While True
                Try
                    While ModLaunch.McLoginMsLoader.State = ModBase.LoadState.Loading
                        Thread.Sleep(10)
                    End While
                    If ModLaunch.McLoginMsLoader.State = ModBase.LoadState.Failed Then
                        Throw New Exception("登录失败", ModLaunch.McLoginMsLoader.[Error])
                    End If
                    Dim text As String = Settings.[Get](Of String)("CacheMsV2Access", Nothing)
                    Settings.[Get](Of String)("CacheMsV2Uuid", Nothing)
                    Dim text2 As String = "https://api.minecraftservices.com/minecraft/profile/skins"
                    Dim post As HttpMethod = HttpMethod.Post
                    Dim text3 As String = Nothing
                    Dim multipartFormDataContent As MultipartFormDataContent = New MultipartFormDataContent()
                    multipartFormDataContent.Add(New StringContent(If(SkinInfo.IsSlim, "slim", "classic")), "variant")
                    multipartFormDataContent.Add(New ByteArrayContent(FileUtils.ReadAsBytes(SkinInfo.LocalFile, Nothing)), "file", PathUtils.GetLastPart(SkinInfo.LocalFile))
                    Dim text4 As String = Nothing
                    Dim text5 As String = "*/*"
                    Dim array As String(,) = New String(2, 1) {}
                    array(0, 0) = "Authorization"
                    array(0, 1) = "Bearer " + text
                    array(1, 0) = "Accept"
                    array(1, 1) = "*/*"
                    array(2, 0) = "User-Agent"
                    array(2, 1) = "MojangSharp/0.1"
                    Dim text6 As String = ModNet.NetRequestByClientRetry(text2, post, text3, multipartFormDataContent, text4, text5, array, False, Nothing, False)
                    If text6.Contains("request requires user authentication") Then
                        ModMain.Hint("正在重新登录，将在登录后自动更改皮肤……", ModMain.HintType.Blue, True)
                        ModLaunch.McLoginMsLoader.Start(PageLoginMsSkin.GetLoginData(), True)
                        Continue For
                    End If
                    Logger.Info(String.Format("皮肤修改返回值：{0}{1}", vbCrLf, text6), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLoginMsSkin.xaml.vb")
                    Dim jobject As JObject = CType(text6.DeserializeJson(), JObject)
                    If jobject.ContainsKey("errorMessage") Then
                        Throw New Exception(jobject("errorMessage").ToString())
                    End If
                    Try
                        For Each jtoken As JToken In jobject("skins")
                            Dim jobject2 As JObject = CType(jtoken, JObject)
                            If Operators.CompareString(jobject2("state").ToString(), "ACTIVE", False) = 0 Then
                                OurSkin.ReloadCache(CStr(jobject2("url")))
                                Return
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of JToken)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    Throw New Exception("未知错误（" + text6 + "）")
                Catch ex As Exception
                    If TypeOf ex Is HttpRequestCodeException Then
                        Dim ex2 As HttpRequestCodeException = CType(ex, HttpRequestCodeException)
                        Dim statusCode As HttpStatusCode = ex2.StatusCode
                        If statusCode <> HttpStatusCode.BadRequest Then
                            If statusCode = HttpStatusCode.Unauthorized Then
                                Logger.Warn(ex, "更改皮肤时遭遇 401 错误", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLoginMsSkin.xaml.vb")
                                ModMain.Hint("正在重新登录，将在登录后自动更改皮肤……", ModMain.HintType.Blue, True)
                                ModLaunch.McLoginMsLoader.Start(PageLoginMsSkin.GetLoginData(), True)
                                Continue For
                            End If
                        Else
                            Logger.Warn(ex, "更改皮肤时遭遇 400 错误", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLoginMsSkin.xaml.vb")
                            Dim response As String = ex2.Response
                            If response IsNot Nothing AndAlso response.Contains("""error""") Then
                                ModMain.Hint("更改皮肤失败：" + ex2.Response.DeserializeJson()("error").ToString(), ModMain.HintType.Red, True)
                            Else
                                Dim response2 As String = ex2.Response
                                If response2 IsNot Nothing AndAlso response2.Contains("""errorMessage""") Then
                                    ModMain.Hint("更改皮肤失败：" + ex2.Response.DeserializeJson()("errorMessage").ToString(), ModMain.HintType.Red, True)
                                End If
                            End If
                        End If
                    ElseIf ex.IsBadNetwork() Then
                        ModMain.Hint("更改皮肤失败：连接 Mojang 服务器超时，请稍后再试，或使用 VPN 改善网络环境", ModMain.HintType.Red, True)
                    Else
                        Logger.[Error](ex, "更改皮肤失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLoginMsSkin.xaml.vb")
                    End If
                Finally
                    Me.IsChanging = False
                End Try
                Exit For
            End While
        End Sub

        Public Sub BtnSkinSave_Click(sender As Object, e As RoutedEventArgs)
            Me.Skin.BtnSkinSave_Click()
        End Sub

        Public Sub BtnSkinRefresh_Click(sender As Object, e As RoutedEventArgs)
            Me.Skin.RefreshClick()
        End Sub

        Public Sub BtnSkinCape_Click(sender As Object, e As RoutedEventArgs)
            Me.Skin.BtnSkinCape_Click()
        End Sub

        Public Sub BtnSkinCapeRefresh_Click(sender As Object, e As RoutedEventArgs)
            Dim action As Action
            If PageLoginMsSkin._Closure$__.$I15-0 IsNot Nothing Then
                action = PageLoginMsSkin._Closure$__.$I15-0
            Else
                Dim action2 As Action = Sub()
                    Try
                        ModMain.Hint("正在刷新披风列表……", ModMain.HintType.Blue, True)
                        If ModLaunch.McLaunchLoader.State = ModBase.LoadState.Loading Then
                            ModLaunch.McLoginMsLoader.WaitForExit(Nothing, Nothing, False, Nothing, Nothing)
                        Else
                            ModLaunch.McLoginMsLoader.WaitForExit(PageLoginMsSkin.GetLoginData(), Nothing, True, Nothing, Nothing)
                        End If
                        ModMain.Hint("已刷新披风列表！", ModMain.HintType.Green, True)
                    Catch ex As Exception
                        Logger.[Error](ex, "刷新披风列表失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\PageLoginMsSkin.xaml.vb")
                    End Try
                End Sub
                action = action2
                PageLoginMsSkin._Closure$__.$I15-0 = action2
            End If
            ModBase.RunInThread(action)
        End Sub

        Friend Overridable Property PanData As Grid
            <CompilerGenerated()>
            Get
                Return Me._PanData
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Grid)
                Dim mouseEventHandler As MouseEventHandler = AddressOf Me.ShowPanel
                Dim mouseEventHandler2 As MouseEventHandler = AddressOf Me.HidePanel
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

        Friend Overridable Property PanButtons As Border

        Friend Overridable Property BtnSkin As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnSkin
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnSkin_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                Dim myIconButton As OurIconButton = Me._BtnSkin
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnSkin = value
                myIconButton = Me._BtnSkin
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnEdit As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnEdit
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.BtnEdit_Click
                Dim myIconButton As OurIconButton = Me._BtnEdit
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

        Friend Overridable Property BtnExit As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExit
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.BtnExit_Click()
                End Sub
                Dim myIconButton As OurIconButton = Me._BtnExit
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnExit = value
                myIconButton = Me._BtnExit
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property Skin As OurSkin

        Private IsChanging As Boolean
    End Class
End Namespace
