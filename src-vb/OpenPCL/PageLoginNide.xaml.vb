Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLoginNide
        Inherits Grid

        Public Sub New()
            Me.IsFirstLoad = True
            Me.InitializeComponent()
        End Sub

        Public Sub Reload(KeepInput As Boolean)
            Me.CheckRemember.Checked = Settings.[Get](Of Boolean)("LoginRemember", Nothing)
            If KeepInput AndAlso Not Me.IsFirstLoad Then
                Dim text As String = Me.ComboName.Text
                Me.ComboName.ItemsSource = If((Operators.CompareString(Settings.[Get](Of String)("LoginNideEmail", Nothing), "", False) = 0), Nothing, Settings.[Get](Of String)("LoginNideEmail", Nothing).ToString().Split("¨", False))
                Me.ComboName.Text = text
            ElseIf Operators.CompareString(Settings.[Get](Of String)("LoginNideEmail", Nothing), "", False) = 0 Then
                Me.ComboName.ItemsSource = Nothing
            Else
                Me.ComboName.ItemsSource = Settings.[Get](Of String)("LoginNideEmail", Nothing).Split("¨", False)
                Me.ComboName.Text = Settings.[Get](Of String)("LoginNideEmail", Nothing).BeforeFirst("¨", False)
                If Settings.[Get](Of Boolean)("LoginRemember", Nothing) Then
                    Me.TextPass.Password = Settings.[Get](Of String)("LoginNidePass", Nothing).BeforeFirst("¨", False).Trim()
                End If
            End If
            Me.IsFirstLoad = False
        End Sub

        Public Shared Function GetLoginData() As ModLaunch.McLoginServer
            Dim text As String = If(Information.IsNothing(ModMinecraft.McInstanceSelected), Settings.[Get](Of String)("CacheNideServer", Nothing), Settings.[Get](Of String)("VersionServerNide", ModMinecraft.McInstanceSelected))
            Dim mcLoginServer As ModLaunch.McLoginServer
            If ModMain.FrmLoginNide Is Nothing Then
                mcLoginServer = New ModLaunch.McLoginServer(ModLaunch.McLoginType.Nide) With { .Token = "Nide", .UserName = "", .Password = "", .Description = "统一通行证", .Type = ModLaunch.McLoginType.Nide, .BaseUrl = "https://auth.mc-user.com:233/" + text + "/authserver" }
            Else
                mcLoginServer = New ModLaunch.McLoginServer(ModLaunch.McLoginType.Nide) With { .Token = "Nide", .UserName = ModMain.FrmLoginNide.ComboName.Text.Replace("¨", "").Trim(), .Password = ModMain.FrmLoginNide.TextPass.Password.Replace("¨", "").Trim(), .Description = "统一通行证", .Type = ModLaunch.McLoginType.Nide, .BaseUrl = "https://auth.mc-user.com:233/" + text + "/authserver" }
            End If
            Return mcLoginServer
        End Function

        Public Shared Function IsVaild(LoginData As ModLaunch.McLoginServer) As String
            Dim text As String
            If Operators.CompareString(LoginData.UserName, "", False) = 0 Then
                text = "账号不能为空！"
            ElseIf Operators.CompareString(LoginData.Password, "", False) = 0 Then
                text = "密码不能为空！"
            Else
                text = ""
            End If
            Return text
        End Function

        Public Function IsVaild() As String
            Return PageLoginNide.IsVaild(PageLoginNide.GetLoginData())
        End Function

        Private Sub ComboName_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(sender, Nothing, "Text", New Object(-1) {}, Nothing, Nothing, Nothing), "", False) Then
                Me.TextPass.Password = ""
            End If
            If ModAnimation.AniControlEnabled = 0 Then
                Settings.[Set]("CacheNideAccess", "", Nothing)
            End If
        End Sub

        Private Sub TextPass_PasswordChanged(sender As Object, e As RoutedEventArgs)
            If ModAnimation.AniControlEnabled = 0 Then
                Settings.[Set]("CacheNideAccess", "", Nothing)
            End If
        End Sub

        Private Sub ComboName_SelectionChanged(sender As OurComboBox, e As SelectionChangedEventArgs)
            If sender.SelectedIndex = -1 OrElse Not Settings.[Get](Of Boolean)("LoginRemember", Nothing) Then
                Me.TextPass.Password = ""
                Return
            End If
            Me.TextPass.Password = Settings.[Get](Of String)("LoginNidePass", Nothing).Split("¨", False)(sender.SelectedIndex).Trim()
        End Sub

        Private Sub CheckBoxChange(sender As OurCheckBox, e As Object)
            If ModAnimation.AniControlEnabled = 0 Then
                Settings.[Set](Conversions.ToString(sender.Tag), sender.Checked, Nothing)
            End If
        End Sub

        Private Sub ComboName_TextChanged()
            Me.BtnLink.Content = If((Operators.CompareString(Me.ComboName.Text, "", False) = 0), "注册账号", "找回密码")
        End Sub

        Private Sub Btn_Click(sender As Object, e As EventArgs)
            If Operators.ConditionalCompareObjectEqual(Me.BtnLink.Content, "注册账号", False) Then
                ModBase.OpenWebsite("https://login.mc-user.com:233/" + Settings.[Get](Of String)("VersionServerNide", ModMinecraft.McInstanceSelected) + "/register")
                Return
            End If
            ModBase.OpenWebsite("https://login.mc-user.com:233/account/login")
        End Sub

        Friend Overridable Property ComboName As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboName
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim textChangedEventHandler As OurComboBox.TextChangedEventHandler = AddressOf Me.ComboName_TextChanged
                Dim selectionChangedEventHandler As SelectionChangedEventHandler = Sub(a0 As Object, a1 As SelectionChangedEventArgs)
                    Me.ComboName_SelectionChanged(CType(a0, OurComboBox), a1)
                End Sub
                Dim textChangedEventHandler2 As OurComboBox.TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.ComboName_TextChanged()
                End Sub
                Dim myComboBox As OurComboBox = Me._ComboName
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.TextChanged, textChangedEventHandler
                    RemoveHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                    RemoveHandler myComboBox.TextChanged, textChangedEventHandler2
                End If
                Me._ComboName = value
                myComboBox = Me._ComboName
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.TextChanged, textChangedEventHandler
                    AddHandler myComboBox.SelectionChanged, selectionChangedEventHandler
                    AddHandler myComboBox.TextChanged, textChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overridable Property TextPass As PasswordBox
            <CompilerGenerated()>
            Get
                Return Me._TextPass
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As PasswordBox)
                Dim routedEventHandler As RoutedEventHandler = AddressOf Me.TextPass_PasswordChanged
                Dim passwordBox As PasswordBox = Me._TextPass
                If passwordBox IsNot Nothing Then
                    RemoveHandler passwordBox.PasswordChanged, routedEventHandler
                End If
                Me._TextPass = value
                passwordBox = Me._TextPass
                If passwordBox IsNot Nothing Then
                    AddHandler passwordBox.PasswordChanged, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckRemember As OurCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckRemember
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCheckBox)
                Dim changeEventHandler As OurCheckBox.ChangeEventHandler = Sub(a0 As Object, a1 As Boolean)
                    Me.CheckBoxChange(CType(a0, OurCheckBox), a1)
                End Sub
                Dim myCheckBox As OurCheckBox = Me._CheckRemember
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckRemember = value
                myCheckBox = Me._CheckRemember
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnLink As OurTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnLink
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextButton)
                Dim clickEventHandler As OurTextButton.ClickEventHandler = AddressOf Me.Btn_Click
                Dim myTextButton As OurTextButton = Me._BtnLink
                If myTextButton IsNot Nothing Then
                    RemoveHandler myTextButton.Click, clickEventHandler
                End If
                Me._BtnLink = value
                myTextButton = Me._BtnLink
                If myTextButton IsNot Nothing Then
                    AddHandler myTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private IsFirstLoad As Boolean
    End Class
End Namespace
