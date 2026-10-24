Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class OurSkin
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.MouseEnter, AddressOf Me.PanSkin_MouseEnter
            AddHandler MyBase.MouseLeave, AddressOf Me.PanSkin_MouseLeave
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.PanSkin_MouseLeftButtonDown
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.PanSkin_MouseLeftButtonUp
            Me.IsSkinMouseDown = False
            Me.IsChanging = False
            Me.InitializeComponent()
        End Sub

        Public Event Click As OurSkin.ClickEventHandler

        Public Property Address As String
            Get
                Return Me._Address
            End Get
            Set(value As String)
                Me._Address = value
                MyBase.ToolTip = If((Operators.CompareString(Me._Address, "", False) = 0), "加载中", "点击更换皮肤（右键查看更多选项）")
            End Set
        End Property

        Private Sub PanSkin_MouseEnter(sender As Object, e As MouseEventArgs)
            ModAnimation.AniStart(ModAnimation.AaOpacity(Me.ShadowSkin, 0.8 - Me.ShadowSkin.Opacity, 200, 100, Nothing, False), "Skin Shadow", False)
        End Sub

        Private Sub PanSkin_MouseLeave(sender As Object, e As MouseEventArgs)
            ModAnimation.AniStart(ModAnimation.AaOpacity(Me.ShadowSkin, 0.2 - Me.ShadowSkin.Opacity, 200, 0, Nothing, False), "Skin Shadow", False)
            Me.IsSkinMouseDown = False
            ModAnimation.AniStart(ModAnimation.AaScaleTransform(Me, 1.0 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, 60, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), "Skin Scale", False)
        End Sub

        Private Sub PanSkin_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
            Me.IsSkinMouseDown = True
            ModAnimation.AniStart(ModAnimation.AaScaleTransform(Me, 0.9 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, 60, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), "Skin Scale", False)
        End Sub

        Private Sub PanSkin_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
            ModAnimation.AniStart(ModAnimation.AaScaleTransform(Me, 1.0 - CType(MyBase.RenderTransform, ScaleTransform).ScaleX, 60, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), "Skin Scale", False)
            If Me.IsSkinMouseDown Then
                Me.IsSkinMouseDown = False
                Dim clickEvent As OurSkin.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
            End If
        End Sub

        Public Sub BtnSkinSave_Click()
            OurSkin.Save(Me.Loader)
        End Sub

        Public Shared Sub Save(Loader As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String))
            Dim output As String = Loader.Output
            If Loader.State <> ModBase.LoadState.Finished Then
                ModMain.Hint("皮肤正在获取中，请稍候！", ModMain.HintType.Red, True)
                If Loader.State <> ModBase.LoadState.Loading Then
                    Loader.Start(Nothing, False)
                    Return
                End If
            Else
                Try
                    Dim text As String = Dialogs.SaveFile("选取保存皮肤的位置", PathUtils.GetLastPart(output), Nothing, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)("png", "皮肤图片文件") })
                    If text IsNot Nothing Then
                        If text.Contains("\") Then
                            FileUtils.Delete(text, False)
                            If output.StartsWithF(ModBase.PathImage, False) Then
                                New OurBitmap(output).Save(text)
                            Else
                                FileUtils.Copy(output, text)
                            End If
                            ModMain.Hint("皮肤保存成功！", ModMain.HintType.Green, True)
                        End If
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "保存皮肤失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                End Try
            End If
        End Sub

        Private Sub BtnSkinSave_Checked(sender As OurMenuItem, e As RoutedEventArgs)
            sender.IsEnabled = Operators.CompareString(Me.Address, "", False) = 0
        End Sub

        Public Sub Load()
            ' The following expression was wrapped in a checked-statement
            Try
                Me.Address = Me.Loader.Output
                If String.IsNullOrEmpty(Me.Address) Then
                    Throw New Exception("皮肤加载器 " + Me.Loader.Name + " 没有输出")
                End If
                If Not Me.Address.StartsWithF(ModBase.PathImage, False) AndAlso Not FileUtils.Exists(Me.Address) Then
                    Throw New FileNotFoundException("皮肤文件未找到", Me.Address)
                End If
                Dim myBitmap As OurBitmap
                Try
                    myBitmap = New OurBitmap(Me.Address)
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("皮肤文件已损坏：{0}", Me.Address), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                    FileUtils.Delete(Me.Address, False)
                    Return
                End Try
                Me.ImgBack.Tag = Me.Address
                Dim num As Integer = CInt(Math.Round(CDbl(myBitmap.Pic.Width) / 64.0))
                If myBitmap.Pic.Width < 32 OrElse myBitmap.Pic.Height < 32 Then
                    Me.ImgFore.Source = Nothing
                    Me.ImgBack.Source = Nothing
                    Throw New Exception("图片大小不足，长为 " + Conversions.ToString(myBitmap.Pic.Height) + "，宽为 " + Conversions.ToString(myBitmap.Pic.Width))
                End If
                If myBitmap.Pic.Width >= 64 AndAlso myBitmap.Pic.Height >= 32 Then
                    ' The following expression was wrapped in a unchecked-expression
                    ' The following expression was wrapped in a unchecked-expression
                    If myBitmap.Pic.GetPixel(1, 1).A = 0 OrElse myBitmap.Pic.GetPixel(myBitmap.Pic.Width - 1, myBitmap.Pic.Height - 1).A = 0 OrElse myBitmap.Pic.GetPixel(myBitmap.Pic.Width - 2, CInt(Math.Round(CDbl(myBitmap.Pic.Height) / 2.0 - 2.0))).A = 0 OrElse (myBitmap.Pic.GetPixel(1, 1) <> myBitmap.Pic.GetPixel(num * 41, num * 9) AndAlso myBitmap.Pic.GetPixel(myBitmap.Pic.Width - 1, myBitmap.Pic.Height - 1) <> myBitmap.Pic.GetPixel(num * 41, num * 9) AndAlso myBitmap.Pic.GetPixel(myBitmap.Pic.Width - 2, CInt(Math.Round(CDbl(myBitmap.Pic.Height) / 2.0 - 2.0))) <> myBitmap.Pic.GetPixel(num * 41, num * 9)) Then
                        Me.ImgFore.Source = myBitmap.Clip(num * 40, num * 8, num * 8, num * 8)
                    Else
                        Me.ImgFore.Source = Nothing
                    End If
                Else
                    Me.ImgFore.Source = Nothing
                End If
                Me.ImgBack.Source = myBitmap.Clip(num * 8, num * 8, num * 8, num * 8)
                Logger.Info(String.Format("载入头像成功：{0}", Me.Loader.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
            Catch ex2 As Exception
                Logger.[Error](ex2, String.Format("载入头像失败（{0},{1}）", If(Me.Address, "null"), Me.Loader.Name), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
            End Try
        End Sub

        Public Sub Clear()
            Me.Address = ""
            Me.ImgFore.Source = Nothing
            Me.ImgBack.Source = Nothing
        End Sub

        Public Sub RefreshClick()
            OurSkin.RefreshCache(Me.Loader)
        End Sub

        Public Shared Sub RefreshCache(Optional sender As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String) = Nothing)
            Dim CS$<>8__locals1 As OurSkin._Closure$__22-0 = New OurSkin._Closure$__22-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_sender = sender
            Dim flag As Boolean = False
            Try
                Dim enumerator As List(Of ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)).Enumerator = PageLaunchLeft.SkinLoaders.GetEnumerator()
                While enumerator.MoveNext()
                    If enumerator.Current.State = ModBase.LoadState.Loading Then
                        flag = True
                        Exit While
                    End If
                End While
            Finally
                Dim enumerator As List(Of ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            If ModMain.FrmLaunchLeft IsNot Nothing AndAlso flag Then
                ModMain.Hint("有正在获取中的皮肤，请稍后再试！", ModMain.HintType.Blue, True)
                Return
            End If
            ModBase.RunInThread(Sub()
                Try
                    ModMain.Hint("正在刷新皮肤……", ModMain.HintType.Blue, True)
                    Logger.Info("正在清空皮肤缓存", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                    If DirectoryUtils.Exists(ModBase.PathTemp + "Cache\Skin") Then
                        DirectoryUtils.Delete(ModBase.PathTemp + "Cache\Skin", False)
                    End If
                    If DirectoryUtils.Exists(ModBase.PathTemp + "Cache\Uuid") Then
                        DirectoryUtils.Delete(ModBase.PathTemp + "Cache\Uuid", False)
                    End If
                    ModBase.IniClearCache(ModBase.PathTemp + "Cache\Skin\IndexMs.ini")
                    ModBase.IniClearCache(ModBase.PathTemp + "Cache\Skin\IndexNide.ini")
                    ModBase.IniClearCache(ModBase.PathTemp + "Cache\Skin\IndexAuth.ini")
                    ModBase.IniClearCache(ModBase.PathTemp + "Cache\Uuid\Mojang.ini")
                    Dim array2 As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)()
                    If CS$<>8__locals1.$VB$Local_sender Is Nothing Then
                        Dim array As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)() = New ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)(1) {}
                        array(0) = PageLaunchLeft.SkinLegacy
                        array2 = array
                        array(1) = PageLaunchLeft.SkinMs
                    Else
                        Dim array3 As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)() = New ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)(0) {}
                        array2 = array3
                        array3(0) = CS$<>8__locals1.$VB$Local_sender
                    End If
                    Dim array4 As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)() = array2
                    For i As Integer = 0 To array4.Length - 1
                        array4(i).WaitForExit(Nothing, Nothing, True, Nothing, Nothing)
                    Next
                    ModMain.Hint("已刷新皮肤！", ModMain.HintType.Green, True)
                Catch ex As Exception
                    Logger.[Error](ex, "刷新皮肤缓存失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                End Try
            End Sub)
        End Sub

        Public Shared Sub ReloadCache(SkinAddress As String)
            Dim CS$<>8__locals1 As OurSkin._Closure$__23-0 = New OurSkin._Closure$__23-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_SkinAddress = SkinAddress
            ModBase.RunInThread(Sub()
                Try
                    ModBase.WriteIni(ModBase.PathTemp + "Cache\Skin\IndexMs.ini", Settings.[Get](Of String)("CacheMsV2Uuid", Nothing), CS$<>8__locals1.$VB$Local_SkinAddress)
                    Logger.Info(String.Format("已写入皮肤地址缓存 {0} -> {1}", Settings.[Get](Of String)("CacheMsV2Uuid", Nothing), CS$<>8__locals1.$VB$Local_SkinAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                    Dim array As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)() = New ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)() { PageLaunchLeft.SkinMs, PageLaunchLeft.SkinLegacy }
                    For i As Integer = 0 To array.Length - 1
                        array(i).WaitForExit(Nothing, Nothing, True, Nothing, Nothing)
                    Next
                    ModMain.Hint("更改皮肤成功！", ModMain.HintType.Green, True)
                Catch ex As Exception
                    Logger.[Error](ex, "更改正版皮肤后刷新皮肤失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                End Try
            End Sub)
        End Sub

        Public Property HasCape As Boolean
            Get
                Return Me.BtnSkinCape.Visibility = Visibility.Collapsed
            End Get
            Set(value As Boolean)
                If value Then
                    Me.BtnSkinCape.Visibility = Visibility.Visible
                    Return
                End If
                Me.BtnSkinCape.Visibility = Visibility.Collapsed
            End Set
        End Property

        Public Sub BtnSkinCape_Click()
            If Me.IsChanging Then
                ModMain.Hint("正在更改披风中，请稍候！", ModMain.HintType.Blue, True)
                Return
            End If
            If ModLaunch.McLoginMsLoader.State = ModBase.LoadState.Failed Then
                ModMain.Hint("登录失败，无法更改披风！", ModMain.HintType.Red, True)
                Return
            End If
            If ModLaunch.McLoginMsLoader.State <> ModBase.LoadState.Finished Then
                ModMain.Hint("正在获取披风列表，请稍候……", ModMain.HintType.Blue, True)
            End If
            Me.IsChanging = True
            ModBase.RunInNewThread(Function()
                While True
                    Try
                        Dim CS$<>8__locals1 As OurSkin._Closure$__28-0 = New OurSkin._Closure$__28-0(CS$<>8__locals1)
                        If ModLaunch.McLoginMsLoader.State <> ModBase.LoadState.Finished Then
                            ModLaunch.McLoginMsLoader.WaitForExit(PageLoginMsSkin.GetLoginData(), Nothing, False, Nothing, Nothing)
                        End If
                        If ModLaunch.McLoginMsLoader.State <> ModBase.LoadState.Finished Then
                            ModMain.Hint("登录失败，无法更改披风！", ModMain.HintType.Red, True)
                        Else
                            Dim accessToken As String = ModLaunch.McLoginMsLoader.Output.AccessToken
                            Dim output As ModLaunch.McLoginResult = ModLaunch.McLoginMsLoader.Output
                            CS$<>8__locals1.$VB$Local_SkinData = CType(ModLaunch.McLoginMsLoader.Output.ProfileJson.DeserializeJson(), JObject)
                            CS$<>8__locals1.$VB$Local_SelectedIndex = Nothing
                            ModBase.RunInUiWait(Function()
                                Try
                                    Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)() From { { "Migrator", "迁移者披风" }, { "MapMaker", "Realms 地图制作者披风" }, { "Moderator", "Mojira 管理员披风" }, { "Translator-Chinese", "Crowdin 中文翻译者披风" }, { "Translator", "Crowdin 翻译者披风" }, { "Cobalt", "Cobalt 披风" }, { "Vanilla", "原版披风" }, { "Minecon2011", "Minecon 2011 参与者披风" }, { "Minecon2012", "Minecon 2012 参与者披风" }, { "Minecon2013", "Minecon 2013 参与者披风" }, { "Minecon2015", "Minecon 2015 参与者披风" }, { "Minecon2016", "Minecon 2016 参与者披风" }, { "Cherry Blossom", "樱花披风" }, { "15th Anniversary", "15 周年纪念披风" }, { "Purple Heart", "紫色心形披风" }, { "Follower's", "追随者披风" }, { "MCC 15th Year", "MCC 15 周年披风" }, { "Minecraft Experience", "村民救援披风" }, { "Mojang Office", "Mojang 办公室披风" }, { "Home", "家园披风" }, { "Menace", "入侵披风" }, { "Yearn", "渴望披风" }, { "Common", "普通披风" }, { "Pan", "薄煎饼披风" }, { "Founder's", "创始人披风" }, { "Copper", "铜披风" }, { "Zombie Horse", "僵尸马披风" }, { "Builder", "建造者披风" }, { "Crafter", "工匠披风" }, { "Aurora", "极光披风" } }
                                    Dim list As List(Of IOurRadio) = New List(Of IOurRadio)()
                                    Dim myRadioBox As OurRadioBox = New OurRadioBox()
                                    myRadioBox.Text = "无披风"
                                    Dim enumerable As IEnumerable(Of JToken) = CS$<>8__locals1.$VB$Local_SkinData("capes")
                                    Dim func As Func(Of JToken, Boolean)
                                    If OurSkin._Closure$__.$I28-2 IsNot Nothing Then
                                        func = OurSkin._Closure$__.$I28-2
                                    Else
                                        Dim func2 As Func(Of JToken, Boolean) = Function(c As JToken)
                                            Dim jtoken4 As JToken = c("state")
                                            Return Operators.CompareString(If((jtoken4 IsNot Nothing), jtoken4.ToString(), Nothing), "ACTIVE", False) = 0
                                        End Function
                                        func = func2
                                        OurSkin._Closure$__.$I28-2 = func2
                                    End If
                                    myRadioBox.Checked = Not enumerable.Any(func)
                                    list.Add(myRadioBox)
                                    Dim list2 As List(Of IOurRadio) = list
                                    Try
                                        For Each jtoken2 As JToken In CS$<>8__locals1.$VB$Local_SkinData("capes")
                                            Dim text6 As String = jtoken2("alias").ToString()
                                            If dictionary.ContainsKey(text6) Then
                                                text6 = dictionary(text6)
                                            End If
                                            Dim list3 As List(Of IOurRadio) = list2
                                            Dim myRadioBox2 As OurRadioBox = New OurRadioBox()
                                            myRadioBox2.Text = text6
                                            Dim myRadioBox3 As OurRadioBox = myRadioBox2
                                            Dim jtoken3 As JToken = jtoken2("state")
                                            myRadioBox3.Checked = Operators.CompareString(If((jtoken3 IsNot Nothing), jtoken3.ToString(), Nothing), "ACTIVE", False) = 0
                                            list3.Add(myRadioBox2)
                                        Next
                                    Finally
                                        Dim enumerator2 As IEnumerator(Of JToken)
                                        If enumerator2 IsNot Nothing Then
                                            enumerator2.Dispose()
                                        End If
                                    End Try
                                    CS$<>8__locals1.$VB$Local_SelectedIndex = ModMain.OurMsgBoxSelect(list2, "选择披风", "确定", "取消", False)
                                Catch ex3 As Exception
                                    Logger.[Error](ex3, "获取玩家皮肤列表失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                                End Try
                            End Function)
                            If CS$<>8__locals1.$VB$Local_SelectedIndex IsNot Nothing Then
                                Dim text As String = "https://api.minecraftservices.com/minecraft/profile/capes/active"
                                Dim num As Integer? = CS$<>8__locals1.$VB$Local_SelectedIndex
                                Dim httpMethod As HttpMethod = If(If((num IsNot Nothing), New Boolean?(num.GetValueOrDefault() = 0), Nothing).GetValueOrDefault(), HttpMethod.Delete, HttpMethod.Put)
                                Dim text2 As String = Nothing
                                num = CS$<>8__locals1.$VB$Local_SelectedIndex
                                Dim obj As Object = If(If((num IsNot Nothing), New Boolean?(num.GetValueOrDefault() = 0), Nothing).GetValueOrDefault(), "", New JObject(New JProperty("capeId", CS$<>8__locals1.$VB$Local_SkinData("capes")(CS$<>8__locals1.$VB$Local_SelectedIndex - 1)("id"))).ToString(0, New JsonConverter(-1) {}))
                                Dim text3 As String = "application/json"
                                Dim text4 As String = "*/*"
                                Dim array As String(,) = New String(0, 1) {}
                                array(0, 0) = "Authorization"
                                array(0, 1) = "Bearer " + accessToken
                                Dim text5 As String = ModNet.NetRequestByClientRetry(text, httpMethod, text2, obj, text3, text4, array, False, Nothing, False)
                                If text5.Contains("""errorMessage""") Then
                                    ModMain.Hint("更改披风失败：" + text5.DeserializeJson()("errorMessage").ToString(), ModMain.HintType.Red, True)
                                Else
                                    ModMain.Hint("更改披风成功！", ModMain.HintType.Green, True)
                                    Try
                                        For Each jtoken As JToken In CS$<>8__locals1.$VB$Local_SkinData("capes")
                                            jtoken("state") = "INACTIVE"
                                        Next
                                    Finally
                                        Dim enumerator As IEnumerator(Of JToken)
                                        If enumerator IsNot Nothing Then
                                            enumerator.Dispose()
                                        End If
                                    End Try
                                    num = CS$<>8__locals1.$VB$Local_SelectedIndex
                                    If If((num IsNot Nothing), New Boolean?(num.GetValueOrDefault() > 0), Nothing).GetValueOrDefault() Then
                                        CS$<>8__locals1.$VB$Local_SkinData("capes")(CS$<>8__locals1.$VB$Local_SelectedIndex - 1)("state") = "ACTIVE"
                                    End If
                                    ModLaunch.McLoginMsLoader.Output.ProfileJson = CS$<>8__locals1.$VB$Local_SkinData.ToString()
                                    Settings.[Set]("CacheMsV2ProfileJson", ModLaunch.McLoginMsLoader.Output.ProfileJson, Nothing)
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        If TypeOf ex Is HttpRequestCodeException Then
                            Dim ex2 As HttpRequestCodeException = CType(ex, HttpRequestCodeException)
                            Dim statusCode As HttpStatusCode = ex2.StatusCode
                            If statusCode <> HttpStatusCode.BadRequest Then
                                If statusCode = HttpStatusCode.Unauthorized Then
                                    Logger.Warn(ex, "更改披风时遭遇 401 错误", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                                    ModMain.Hint("正在重新登录，将在登录后自动更改披风……", ModMain.HintType.Blue, True)
                                    ModLaunch.McLoginMsLoader.Start(PageLoginMsSkin.GetLoginData(), True)
                                    Continue For
                                End If
                            Else
                                Logger.Warn(ex, "更改披风时遭遇 400 错误", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                                Dim response As String = ex2.Response
                                If response IsNot Nothing AndAlso response.Contains("""error""") Then
                                    ModMain.Hint("更改披风失败：" + ex2.Response.DeserializeJson()("error").ToString(), ModMain.HintType.Red, True)
                                Else
                                    Dim response2 As String = ex2.Response
                                    If response2 IsNot Nothing AndAlso response2.Contains("""errorMessage""") Then
                                        ModMain.Hint("更改披风失败：" + ex2.Response.DeserializeJson()("errorMessage").ToString(), ModMain.HintType.Red, True)
                                    End If
                                End If
                            End If
                        ElseIf ex.IsBadNetwork() Then
                            ModMain.Hint("更改披风失败：连接 Mojang 服务器超时，请稍后再试，或使用 VPN 改善网络环境", ModMain.HintType.Red, True)
                        Else
                            Logger.[Error](ex, "更改披风失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageLaunch\OurSkin.xaml.vb")
                        End If
                    Finally
                        Me.IsChanging = False
                    End Try
                    Exit For
                End While
            End Function, "Cape Change", ThreadPriority.Normal)
        End Sub

        Friend Overridable Property ShadowSkin As DropShadowEffect

        Friend Overridable Property BtnSkinSave As OurMenuItem
            <CompilerGenerated()>
            Get
                Return Me._BtnSkinSave
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurMenuItem)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.BtnSkinSave_Click()
                End Sub
                Dim routedEventHandler2 As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.BtnSkinSave_Checked(CType(a0, OurMenuItem), a1)
                End Sub
                Dim myMenuItem As OurMenuItem = Me._BtnSkinSave
                If myMenuItem IsNot Nothing Then
                    RemoveHandler myMenuItem.Click, routedEventHandler
                    RemoveHandler myMenuItem.Checked, routedEventHandler2
                End If
                Me._BtnSkinSave = value
                myMenuItem = Me._BtnSkinSave
                If myMenuItem IsNot Nothing Then
                    AddHandler myMenuItem.Click, routedEventHandler
                    AddHandler myMenuItem.Checked, routedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property BtnSkinRefresh As OurMenuItem
            <CompilerGenerated()>
            Get
                Return Me._BtnSkinRefresh
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurMenuItem)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.RefreshClick()
                End Sub
                Dim myMenuItem As OurMenuItem = Me._BtnSkinRefresh
                If myMenuItem IsNot Nothing Then
                    RemoveHandler myMenuItem.Click, routedEventHandler
                End If
                Me._BtnSkinRefresh = value
                myMenuItem = Me._BtnSkinRefresh
                If myMenuItem IsNot Nothing Then
                    AddHandler myMenuItem.Click, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnSkinCape As OurMenuItem
            <CompilerGenerated()>
            Get
                Return Me._BtnSkinCape
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurMenuItem)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.BtnSkinCape_Click()
                End Sub
                Dim myMenuItem As OurMenuItem = Me._BtnSkinCape
                If myMenuItem IsNot Nothing Then
                    RemoveHandler myMenuItem.Click, routedEventHandler
                End If
                Me._BtnSkinCape = value
                myMenuItem = Me._BtnSkinCape
                If myMenuItem IsNot Nothing Then
                    AddHandler myMenuItem.Click, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ImgBack As Image

        Friend Overridable Property ImgFore As Image

        Private _Address As String

        Public Loader As ModLoader.LoaderTask(Of ValueTuple(Of String, String), String)

        Private IsSkinMouseDown As Boolean

        Private IsChanging As Boolean

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)
    End Class
End Namespace
