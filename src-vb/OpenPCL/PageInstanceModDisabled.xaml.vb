Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Markup
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageInstanceModDisabled
        Inherits OurPageRight

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Private Sub BtnDownload_Click(sender As Object, e As EventArgs)
            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.[Default])
        End Sub

        Private Sub BtnVersion_Click(sender As Object, e As EventArgs)
            ModMain.FrmMain.PageChange(FormMain.PageType.Launch, FormMain.PageSubType.[Default])
            ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSelect, FormMain.PageSubType.[Default])
        End Sub

        Public Sub BtnDownload_Loaded()
            Dim visibility As Visibility = If(((Settings.[Get](Of Boolean)("UiHiddenPageDownload", Nothing) AndAlso Not PageSetupUI.HiddenForceShow) OrElse (ModMain.FrmSelectRight IsNot Nothing AndAlso ModMain.FrmSelectRight.ShowHidden)), Visibility.Collapsed, Visibility.Visible)
            If Me.BtnDownload.Visibility <> visibility Then
                Me.BtnDownload.Visibility = visibility
                Me.PanMain.TriggerForceResize()
            End If
        End Sub

        Friend Overridable Property PanMain As OurCard

        Friend Overridable Property BtnDownload As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnDownload
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnDownload_Click
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.BtnDownload_Loaded()
                End Sub
                Dim myButton As OurButton = Me._BtnDownload
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                    RemoveHandler myButton.Loaded, routedEventHandler
                End If
                Me._BtnDownload = value
                myButton = Me._BtnDownload
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                    AddHandler myButton.Loaded, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnVersion As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnVersion
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = AddressOf Me.BtnVersion_Click
                Dim myButton As OurButton = Me._BtnVersion
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnVersion = value
                myButton = Me._BtnVersion
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property
    End Class
End Namespace
