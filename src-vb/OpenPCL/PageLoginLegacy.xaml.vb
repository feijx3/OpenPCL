Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageLoginLegacy
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageLoginLegacy_Loaded
            Me.IsReloaded = False
            Me.InitializeComponent()
            Me.Skin.Loader = PageLaunchLeft.SkinLegacy
        End Sub

        Private Sub PageLoginLegacy_Loaded(sender As Object, e As RoutedEventArgs)
            Me.Skin.Loader.Start(Nothing, False)
        End Sub

        Public Sub Reload(KeepInput As Boolean)
            If KeepInput AndAlso Me.IsReloaded Then
                Dim text As String = Me.ComboName.Text.Trim()
                Me.ComboName.ItemsSource = If((Operators.CompareString(Settings.[Get](Of String)("LoginLegacyName", Nothing), "", False) = 0), Nothing, Settings.[Get](Of String)("LoginLegacyName", Nothing).Split("¨", False))
                Me.ComboName.Text = text
            ElseIf Operators.CompareString(Settings.[Get](Of String)("LoginLegacyName", Nothing), "", False) = 0 Then
                Me.ComboName.ItemsSource = Nothing
            Else
                Me.ComboName.ItemsSource = Settings.[Get](Of String)("LoginLegacyName", Nothing).ToString().Split("¨", False)
                Me.ComboName.Text = Settings.[Get](Of String)("LoginLegacyName", Nothing).ToString().BeforeFirst("¨", False).Trim()
            End If
            Me.IsReloaded = True
        End Sub

        Public Shared Function GetLoginData() As ModLaunch.McLoginData
            Dim text As String = If((ModMain.FrmLoginLegacy Is Nothing), "", ModMain.FrmLoginLegacy.ComboName.Text.Replace("¨", "").Trim())
            Return New ModLaunch.McLoginLegacy() With { .UserName = text, .SkinType = Settings.[Get](Of Integer)("LaunchSkinType", Nothing), .SkinName = Settings.[Get](Of String)("LaunchSkinID", Nothing) }
        End Function

        Public Shared Function IsVaild(LoginData As ModLaunch.McLoginLegacy) As String
            Dim text As String
            If Operators.CompareString(LoginData.UserName.Trim(), "", False) = 0 Then
                text = "玩家名不能为空！"
            ElseIf LoginData.UserName.Contains("""") Then
                text = "玩家名不能包含英文引号！"
            ElseIf ModMinecraft.McInstanceSelected IsNot Nothing AndAlso ModMinecraft.McInstanceSelected.Version.Valid AndAlso ModMinecraft.McInstanceSelected.Version.Vanilla >= New Version(20, 0, 3) AndAlso LoginData.UserName.Trim().Length > 16 Then
                text = "自 1.20.3 起，玩家名至多只能包含 16 个字符！"
            Else
                text = ""
            End If
            Return text
        End Function

        Public Function IsVaild() As String
            Return PageLoginLegacy.IsVaild(CType(PageLoginLegacy.GetLoginData(), ModLaunch.McLoginLegacy))
        End Function

        Private Sub ComboName_PreviewKeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.Space Then
                Dim caretIndex As Integer = CType(Me.ComboName.Template.FindName("PART_EditableTextBox", Me.ComboName), OurTextBox).CaretIndex
                If caretIndex = Me.ComboName.Text.Length OrElse caretIndex = 0 Then
                    e.Handled = True
                End If
            End If
        End Sub

        Private Sub ComboLegacy_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Settings.[Get](Of Integer)("LaunchSkinType", Nothing) = 0 Then
                PageLaunchLeft.SkinLegacy.Start(Nothing, True)
            End If
            Me.HintChinese.Visibility = If(Me.ComboName.Text.RegexCheck("^[0-9A-Za-z_]*$", RegexOptions.None), Visibility.Collapsed, Visibility.Visible)
            ModBase.RunInUi(Sub()
                Me.PanMain.InvalidateMeasure()
            End Sub, True)
        End Sub

        Private Sub Skin_Click()
            If(Settings.[Get](Of Boolean)("UiHiddenPageSetup", Nothing) OrElse Settings.[Get](Of Boolean)("UiHiddenSetupLaunch", Nothing)) AndAlso Not PageSetupUI.HiddenForceShow Then
                ModMain.Hint("启动设置已被禁用！", ModMain.HintType.Red, True)
                Return
            End If
            ModMain.FrmMain.PageChange(FormMain.PageType.Setup, FormMain.PageSubType.[Default])
        End Sub

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property HintChinese As OurHint

        Friend Overridable Property Skin As OurSkin
            <CompilerGenerated()>
            Get
                Return Me._Skin
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurSkin)
                Dim clickEventHandler As OurSkin.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Skin_Click()
                End Sub
                Dim mySkin As OurSkin = Me._Skin
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

        Friend Overridable Property ComboName As OurComboBox
            <CompilerGenerated()>
            Get
                Return Me._ComboName
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurComboBox)
                Dim keyEventHandler As KeyEventHandler = AddressOf Me.ComboName_PreviewKeyDown
                Dim textChangedEventHandler As OurComboBox.TextChangedEventHandler = AddressOf Me.ComboLegacy_TextChanged
                Dim myComboBox As OurComboBox = Me._ComboName
                If myComboBox IsNot Nothing Then
                    RemoveHandler myComboBox.PreviewKeyDown, keyEventHandler
                    RemoveHandler myComboBox.TextChanged, textChangedEventHandler
                End If
                Me._ComboName = value
                myComboBox = Me._ComboName
                If myComboBox IsNot Nothing Then
                    AddHandler myComboBox.PreviewKeyDown, keyEventHandler
                    AddHandler myComboBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Public IsReloaded As Boolean
    End Class
End Namespace
