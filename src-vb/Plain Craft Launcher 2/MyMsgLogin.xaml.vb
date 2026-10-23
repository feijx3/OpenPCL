Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports System.Windows.Shapes
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class MyMsgLogin
        Inherits Grid

        Public Sub New(Converter As ModMain.MyMsgBoxConverter)
            AddHandler MyBase.Loaded, AddressOf Me.Load
            Me.Uuid = ModBase.GetUuid()
            Try
                Me.InitializeComponent()
                Me.Btn1.Name = Me.Btn1.Name + Conversions.ToString(ModBase.GetUuid())
                Me.Btn2.Name = Me.Btn2.Name + Conversions.ToString(ModBase.GetUuid())
                Me.Btn3.Name = Me.Btn3.Name + Conversions.ToString(ModBase.GetUuid())
                Me.MyConverter = Converter
                Me.ShapeLine.StrokeThickness = ModBase.GetWPFSize(1.0)
                Me.Data = CType(Converter.Content, JObject)
                Me.Init()
            Catch ex As Exception
                Logger.[Error](ex, "登录弹窗初始化失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\MyMsgLogin.xaml.vb")
            End Try
        End Sub

        Private Sub Load(sender As Object, e As EventArgs)
            Try
                MyBase.Opacity = 0.0
                ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, If(Me.MyConverter.IsWarn, New ModBase.MyColor(140.0, 80.0, 0.0, 0.0), New ModBase.MyColor(90.0, 0.0, 0.0, 0.0)) - ModMain.FrmMain.PanMsg.Background, 200, 0, Nothing, False), "PanMsg Background", False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me, 1.0, 120, 60, Nothing, False), ModAnimation.AaDouble(Sub(i As Object)
                    Dim transformPos As TranslateTransform = Me.TransformPos
                    Dim translateTransform As TranslateTransform = transformPos
                    transformPos.Y = Conversions.ToDouble(Operators.AddObject(translateTransform.Y, i))
                End Sub, -Me.TransformPos.Y, 300, 60, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaDouble(Sub(i As Object)
                    Dim transformRotate As RotateTransform = Me.TransformRotate
                    Dim rotateTransform As RotateTransform = transformRotate
                    transformRotate.Angle = Conversions.ToDouble(Operators.AddObject(rotateTransform.Angle, i))
                End Sub, -Me.TransformRotate.Angle, 300, 60, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False) }, "MyMsgBox " + Conversions.ToString(Me.Uuid), False)
                Logger.Info(String.Format("登录弹窗：{0}{1}{2}", Me.LabTitle.Text, vbCrLf, Me.LabCaption.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\MyMsgLogin.xaml.vb")
            Catch ex As Exception
                Logger.[Error](ex, "登录弹窗加载失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\MyMsgLogin.xaml.vb")
            End Try
        End Sub

        Private Sub Close()
            Dim array As ModAnimation.AniData() = New ModAnimation.AniData(4) {}
            Dim num As Integer = 0
            Dim threadStart As ThreadStart
            If MyMsgLogin._Closure$__.$I8-0 IsNot Nothing Then
                threadStart = MyMsgLogin._Closure$__.$I8-0
            Else
                Dim threadStart2 As ThreadStart = Sub()
                    If Not ModMain.WaitingMyMsgBox.Any() Then
                        ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, New ModBase.MyColor(0.0, 0.0, 0.0, 0.0) - ModMain.FrmMain.PanMsg.Background, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), "", False)
                    End If
                End Sub
                threadStart = threadStart2
                MyMsgLogin._Closure$__.$I8-0 = threadStart2
            End If
            array(num) = ModAnimation.AaCode(threadStart, 30, False)
            array(1) = ModAnimation.AaOpacity(Me, -MyBase.Opacity, 80, 20, Nothing, False)
            array(2) = ModAnimation.AaDouble(Sub(i As Object)
                Dim transformPos As TranslateTransform = Me.TransformPos
                Dim translateTransform As TranslateTransform = transformPos
                transformPos.Y = Conversions.ToDouble(Operators.AddObject(translateTransform.Y, i))
            End Sub, 20.0 - Me.TransformPos.Y, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False)
            array(3) = ModAnimation.AaDouble(Sub(i As Object)
                Dim transformRotate As RotateTransform = Me.TransformRotate
                Dim rotateTransform As RotateTransform = transformRotate
                transformRotate.Angle = Conversions.ToDouble(Operators.AddObject(rotateTransform.Angle, i))
            End Sub, 6.0 - Me.TransformRotate.Angle, 150, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False)
            array(4) = ModAnimation.AaCode(Sub()
                CType(MyBase.Parent, Grid).Children.Remove(Me)
            End Sub, 0, True)
            ModAnimation.AniStart(array, "MyMsgBox " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Public Sub Btn1_Click()
        End Sub

        Public Sub Btn3_Click()
            Me.Finished(New OperationCanceledException())
        End Sub

        Private Sub Drag(sender As Object, e As MouseButtonEventArgs)
            Dim num As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_0000:
                ProjectData.ClearProjectError()
                num = 1
                IL_0007:
                Dim num2 As Integer = 2
                If e.GetPosition(Me.ShapeLine).Y > 2.0 Then
                    GoTo IL_0034
                End If
                IL_0028:
                num2 = 3
                ModMain.FrmMain.DragMove()
                IL_0034:
                GoTo IL_0093
                IL_0036:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_0054:
                GoTo IL_0088
                IL_0056:
                num4 = num2
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                IL_0066:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_0056
            End Try
            IL_0088:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_0093:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Private Sub Finished(Result As Object)
            If Not Me.MyConverter.IsExited Then
                Me.MyConverter.IsExited = True
                Me.MyConverter.Result = RuntimeHelpers.GetObjectValue(Result)
                ModBase.RunInUi(AddressOf Me.Close, False)
                Thread.Sleep(200)
                ModMain.FrmMain.ShowWindowToTop()
            End If
        End Sub

        Private Sub Init()
            Me.UserCode = CStr(Me.Data("user_code"))
            Me.DeviceCode = CStr(Me.Data("device_code"))
            Me.Website = CStr(Me.Data("verification_uri"))
            Me.LabTitle.Text = "登录 Minecraft"
            Me.LabCaption.Text = String.Format("登录网页将自动开启，请在网页中输入 {0}（已自动复制）。", Me.UserCode) + vbCrLf & vbCrLf & "如果网络环境不佳，网页可能一直加载不出来，届时请使用加速器或 VPN 改善网络环境。" & vbCrLf + String.Format("你也可以用其他设备打开 {0} 并输入上述代码。", Me.Website)
            CustomEventService.SetEventData(Me.Btn1, Me.Website)
            CustomEventService.SetEventData(Me.Btn2, Me.UserCode)
            ModBase.RunInNewThread(AddressOf Me.WorkThread, "MyMsgLogin", ThreadPriority.Normal)
        End Sub

        Private Sub WorkThread()
            Thread.Sleep(2000)
            If Not Me.MyConverter.IsExited Then
                ModBase.OpenWebsite(Me.Website)
                ModBase.ClipboardSet(Me.UserCode, True)
                Thread.Sleep((Me.Data("interval").ToObject(Of Integer)() - 1) * 1000)
                Dim num As Integer = 0
                While Not Me.MyConverter.IsExited
                    Try
                        Dim jobject As JObject = CType(ModNet.NetRequestByClient("https://login.microsoftonline.com/consumers/oauth2/v2.0/token", HttpMethod.Post, "grant_type=urn:ietf:params:oauth:grant-type:device_code&client_id=fe72edc2-3a6f-4280-90e8-e2beb64ce7e1&device_code=" + Me.DeviceCode + "&scope=XboxLive.signin%20offline_access", "application/x-www-form-urlencoded", "*/*", 5000 + num * 5000, Nothing, True, Nothing, False, False).DeserializeJson(), JObject)
                        ModLaunch.McLaunchLog(String.Format("令牌过期时间：{0} 秒", jobject("expires_in")), LogLevel.Info)
                        ModMain.Hint("网页登录成功！", ModMain.HintType.Green, True)
                        Me.Finished(New String() { jobject("access_token").ToString(), jobject("refresh_token").ToString() })
                        Exit Try
                    Catch ex As Exception
                        If TypeOf ex Is HttpRequestCodeException Then
                            Dim response As String = CType(ex, HttpRequestCodeException).Response
                            If response.Contains("authorization_declined") Then
                                Me.Finished(New Exception("$你拒绝了 PCL 申请的权限……"))
                                Exit Try
                            End If
                            If response.Contains("expired_token") Then
                                Me.Finished(New Exception("$登录用时太长啦，重新试试吧！"))
                                Exit Try
                            End If
                            If response.Contains("Account security interrupt") Then
                                Me.Finished(New Exception("$该账号由于安全问题无法登陆，请前往微软账户页获取更多信息。"))
                                Exit Try
                            End If
                            If response.Contains("service abuse") Then
                                Me.Finished(New Exception("$非常抱歉，该账号已被微软封禁，无法登录。"))
                                Exit Try
                            End If
                            If response.Contains("AADSTS70000") Then
                                Me.Finished(New ModBase.RestartException())
                                Exit Try
                            End If
                            If response.Contains("authorization_pending") Then
                                Thread.Sleep(2000)
                                Continue While
                            End If
                        End If
                        If num > 2 Then
                            Me.Finished(New Exception("登录轮询失败", ex))
                            Exit Try
                        End If
                        num += 1
                        Logger.Warn(ex, String.Format("登录轮询第 {0} 次失败", num), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\MyMsgLogin.xaml.vb")
                        Thread.Sleep(2000)
                    End Try
                End While
            End If
        End Sub

        Friend Overridable Property TransformRotate As RotateTransform

        Friend Overridable Property TransformPos As TranslateTransform

        Friend Overridable Property PanBorder As Border
            <CompilerGenerated()>
            Get
                Return Me._PanBorder
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Border)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.Drag
                Dim border As Border = Me._PanBorder
                If border IsNot Nothing Then
                    RemoveHandler border.MouseLeftButtonDown, mouseButtonEventHandler
                End If
                Me._PanBorder = value
                border = Me._PanBorder
                If border IsNot Nothing Then
                    AddHandler border.MouseLeftButtonDown, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property EffectShadow As DropShadowEffect

        Friend Overridable Property PanMain As Grid

        Friend Overridable Property LabTitle As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabTitle
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.Drag
                Dim textBlock As TextBlock = Me._LabTitle
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.MouseLeftButtonDown, mouseButtonEventHandler
                End If
                Me._LabTitle = value
                textBlock = Me._LabTitle
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.MouseLeftButtonDown, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ShapeLine As Rectangle

        Friend Overridable Property PanCaption As MyScrollViewer

        Friend Overridable Property LabCaption As TextBlock

        Friend Overridable Property PanBtn As StackPanel

        Public Overridable Property Btn1 As MyButton
            <CompilerGenerated()>
            Get
                Return Me._Btn1
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Btn1_Click()
                End Sub
                Dim myButton As MyButton = Me._Btn1
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._Btn1 = value
                myButton = Me._Btn1
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Public Overridable Property Btn2 As MyButton

        Public Overridable Property Btn3 As MyButton
            <CompilerGenerated()>
            Get
                Return Me._Btn3
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Btn3_Click()
                End Sub
                Dim myButton As MyButton = Me._Btn3
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._Btn3 = value
                myButton = Me._Btn3
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private Data As JObject

        Private UserCode As String

        Private DeviceCode As String

        Private Website As String

        Private MyConverter As ModMain.MyMsgBoxConverter

        Private Uuid As Integer
    End Class
End Namespace
