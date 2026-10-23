Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageOtherAbout
        Inherits MyPageRight

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageOtherAbout_Loaded
            Me.IsLoaded = False
            Me.InitializeComponent()
        End Sub

        Private Sub PageOtherAbout_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToHome()
            If Not Me.IsLoaded Then
                Me.IsLoaded = True
                Me.ItemAboutPcl.Info = Me.ItemAboutPcl.Info.Replace("%VERSION%", "快照版 2.99.0.0").Replace("%VERSIONCODE%", Conversions.ToString(414)).Replace("%BRANCH%", Conversions.ToString(0))
            End If
        End Sub

        Public Shared Sub CopyIdentify()
            ModBase.ClipboardSet(ModBase.Identify, True)
        End Sub

        Private Sub BtnDonateCodeInput_Click()
            ModSecret.InputPotatoCode(False)
        End Sub

        Private Sub BtnDonateOutput_Click(sender As Object, e As EventArgs)
            ModSecret.GeneratePotatoCode()
        End Sub

        Private Sub BtnAboutUpdate_Click(sender As Object, e As MouseButtonEventArgs)
            ModSecret.UpdateCheckByButton()
        End Sub

        Friend Overridable Property PanBack As MyScrollViewer

        Friend Overridable Property PanMain As StackPanel

        Friend Overridable Property ItemAboutPcl As MyListItem

        Friend Overridable Property BtnAboutUpdate As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnAboutUpdate
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnAboutUpdate_Click
                Dim myButton As MyButton = Me._BtnAboutUpdate
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnAboutUpdate = value
                myButton = Me._BtnAboutUpdate
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnDonateOutput As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnDonateOutput
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = AddressOf Me.BtnDonateOutput_Click
                Dim myButton As MyButton = Me._BtnDonateOutput
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnDonateOutput = value
                myButton = Me._BtnDonateOutput
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnDonateDonate As MyButton

        Friend Overridable Property BtnDonateCopy As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnDonateCopy
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler
                If PageOtherAbout._Closure$__.$IR33-1 IsNot Nothing Then
                    clickEventHandler = PageOtherAbout._Closure$__.$IR33-1
                Else
                    Dim clickEventHandler2 As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                        PageOtherAbout.CopyIdentify()
                    End Sub
                    clickEventHandler = clickEventHandler2
                    PageOtherAbout._Closure$__.$IR33-1 = clickEventHandler2
                End If
                Dim clickEventHandler3 As MyButton.ClickEventHandler = clickEventHandler
                Dim myButton As MyButton = Me._BtnDonateCopy
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler3
                End If
                Me._BtnDonateCopy = value
                myButton = Me._BtnDonateCopy
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler3
                End If
            End Set
        End Property

        Friend Overridable Property BtnDonateInput As MyButton
            <CompilerGenerated()>
            Get
                Return Me._BtnDonateInput
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.BtnDonateCodeInput_Click()
                End Sub
                Dim myButton As MyButton = Me._BtnDonateInput
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnDonateInput = value
                myButton = Me._BtnDonateInput
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private IsLoaded As Boolean
    End Class
End Namespace
