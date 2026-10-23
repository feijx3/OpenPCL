Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Security.Authentication
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLoginMs
        Inherits StackPanel

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Public Sub Reload(KeepInput As Boolean)
            Dim selectedIndex As Integer = Me.ComboAccounts.SelectedIndex
            Me.ComboAccounts.Items.Clear()
            Me.ComboAccounts.Items.Add(New MyComboBoxItem() With { .Content = "添加新账号" })
            Try
                Dim jobject As JObject = CType(Settings.[Get](Of String)("LoginMsJson", Nothing).DeserializeJson(), JObject)
                Try
                    For Each keyValuePair As KeyValuePair(Of String, JToken) In jobject
                        Dim myListItem As MyListItem = CType(CType(MyBase.FindResource("ComboBoxItemTemplateWithDelete"), DataTemplate).LoadContent(), MyListItem)
                        myListItem.Tag = keyValuePair.Value.ToString()
                        myListItem.Title = keyValuePair.Key
                        myListItem.Buttons.ElementAtOrDefault(0).Tag = keyValuePair.Key
                        Me.ComboAccounts.Items.Add(myListItem)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of KeyValuePair(Of String, JToken))
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            Catch ex As Exception
                Logger.[Error](ex, "微软登录信息出错，登录信息已被重置", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\PageLoginMs.xaml.vb")
                Settings.[Set]("LoginMsJson", "{}", Nothing)
            End Try
            Me.ComboAccounts.SelectedIndex = If(KeepInput, Math.Max(0, selectedIndex), 0)
        End Sub

        Private Sub ComboAccounts_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If ModAnimation.AniControlEnabled = 0 AndAlso Me.ComboAccounts.SelectedItem IsNot Nothing AndAlso Me.ComboAccounts.ContentPresenter IsNot Nothing Then
                If TypeOf Me.ComboAccounts.SelectedItem Is MyListItem Then
                    Me.ComboAccounts.ContentPresenter.Content = CType(Me.ComboAccounts.SelectedItem, MyListItem).Title
                    Return
                End If
                If TypeOf Me.ComboAccounts.SelectedItem Is MyComboBoxItem Then
                    Me.ComboAccounts.ContentPresenter.Content = RuntimeHelpers.GetObjectValue(CType(Me.ComboAccounts.SelectedItem, MyComboBoxItem).Content)
                End If
            End If
        End Sub

        Public Shared Function GetLoginData() As ModLaunch.McLoginMs
            Dim mcLoginMs As ModLaunch.McLoginMs
            If ModMain.FrmLoginMs Is Nothing Then
                mcLoginMs = New ModLaunch.McLoginMs() With { .OAuthRefreshToken = Settings.[Get](Of String)("CacheMsV2OAuthRefresh", Nothing), .UserName = Settings.[Get](Of String)("CacheMsV2Name", Nothing) }
            Else
                Dim Result As ModLaunch.McLoginMs = Nothing
                ModBase.RunInUiWait(Sub()
                    If ModMain.FrmLoginMs.ComboAccounts.SelectedIndex = 0 Then
                        Result = New ModLaunch.McLoginMs()
                        Return
                    End If
                    Dim myListItem As MyListItem = CType(ModMain.FrmLoginMs.ComboAccounts.SelectedItem, MyListItem)
                    Result = New ModLaunch.McLoginMs() With { .OAuthRefreshToken = Conversions.ToString(myListItem.Tag), .UserName = myListItem.Title }
                End Sub)
                mcLoginMs = Result
            End If
            Return mcLoginMs
        End Function

        Public Shared Function IsVaild(LoginData As ModLaunch.McLoginMs) As String
            Dim text As String
            If Operators.CompareString(LoginData.OAuthRefreshToken, "", False) = 0 Then
                text = "请在登录账号后再启动游戏！"
            Else
                text = ""
            End If
            Return text
        End Function

        Public Function IsVaild() As String
            Return PageLoginMs.IsVaild(PageLoginMs.GetLoginData())
        End Function

        Private Sub BtnLogin_Click(sender As Object, e As EventArgs)
            Me.ComboAccounts.IsEnabled = False
            Me.BtnLogin.IsEnabled = False
            Me.BtnLogin.Text = "0%"
            ModBase.RunInNewThread(Sub()
                Try
                    ModLaunch.McLoginMsLoader.Start(PageLoginMs.GetLoginData(), True)
                    While ModLaunch.McLoginMsLoader.State = ModBase.LoadState.Loading
                        ModBase.RunInUi(Sub()
                            Me.BtnLogin.Text = Conversions.ToString(Math.Round(ModLaunch.McLoginMsLoader.Progress * 100.0)) + "%"
                        End Sub, False)
                        Thread.Sleep(50)
                    End While
                    If ModLaunch.McLoginMsLoader.State = ModBase.LoadState.Finished Then
                        Dim action As Action
                        If PageLoginMs._Closure$__.$I6-2 IsNot Nothing Then
                            action = PageLoginMs._Closure$__.$I6-2
                        Else
                            Dim action2 As Action = Sub()
                                ModMain.FrmLaunchLeft.RefreshPage(False, True)
                            End Sub
                            action = action2
                            PageLoginMs._Closure$__.$I6-2 = action2
                        End If
                        ModBase.RunInUi(action, False)
                    Else
                        If ModLaunch.McLoginMsLoader.State = ModBase.LoadState.Canceled Then
                            Throw New OperationCanceledException()
                        End If
                        If ModLaunch.McLoginMsLoader.[Error] Is Nothing Then
                            Throw New Exception("未知错误！")
                        End If
                        Throw New Exception(ModLaunch.McLoginMsLoader.[Error].Message, ModLaunch.McLoginMsLoader.[Error])
                    End If
                Catch ex As Exception
                    If Not ex.IsCanceled() Then
                        If ex.Message.StartsWithF("$", False) Then
                            ModMain.Hint(ex.Message.TrimStart(New Char() { "$"c }), ModMain.HintType.Red, True)
                        ElseIf TypeOf ex Is AuthenticationException AndAlso ex.Message.Contains("SSL/TLS") Then
                            Logger.[Error](ex, String.Format("正版登录验证失败，请考虑在 [设置 → 其他] 中关闭 [在正版登录时验证 SSL 证书]，然后再试。{0}{1}原始错误信息：", vbCrLf, vbCrLf), LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\PageLoginMs.xaml.vb")
                        Else
                            Logger.[Error](ex, "正版登录尝试失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageLaunch\PageLoginMs.xaml.vb")
                        End If
                    End If
                Finally
                    ModBase.RunInUi(Sub()
                        Me.ComboAccounts.IsEnabled = True
                        Me.BtnLogin.IsEnabled = True
                        Me.BtnLogin.Text = "登录"
                    End Sub, False)
                End Try
            End Sub, "Ms Login", ThreadPriority.Normal)
        End Sub

        Friend Overridable Property PanEmpty As Grid

        Friend Overridable Property ComboAccounts As MyComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboAccounts
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyComboBox)
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = AddressOf Me.ComboAccounts_SelectionChanged
                Dim myComboBox As MyComboBox = Me._ComboAccounts
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
                Me._ComboAccounts = value
                myComboBox = Me._ComboAccounts
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnLogin As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnLogin
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnLogin_Click
                Dim myButton As MyButton = Me._BtnLogin
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnLogin = value
                myButton = Me._BtnLogin
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property
    End Class
End Namespace
