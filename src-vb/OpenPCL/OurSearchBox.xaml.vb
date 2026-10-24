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
    Public Partial Class OurSearchBox
        Inherits OurCard

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.OurSearchBox_Loaded
            AddHandler MyBase.KeyUp, AddressOf Me.OurSearchBox_KeyUp
            Me.InitializeComponent()
        End Sub

        Public Event TextChanged As OurSearchBox.TextChangedEventHandler

        Private Sub OurSearchBox_Loaded(sender As Object, e As RoutedEventArgs)
            Me.TextBox.Focus()
        End Sub

        Public Property HintText As String
            Get
                Return Me.TextBox.HintText
            End Get
            Set(value As String)
                Me.TextBox.HintText = value
            End Set
        End Property

        Public Property Text As String
            Get
                Return Me.TextBox.Text
            End Get
            Set(value As String)
                Me.TextBox.Text = value
            End Set
        End Property

        Private Sub Text_TextChanged(sender As Object, e As TextChangedEventArgs)
            If String.IsNullOrEmpty(Me.TextBox.Text) Then
                ModAnimation.AniStart(ModAnimation.AaOpacity(Me.BtnClear, -Me.BtnClear.Opacity, 90, 0, Nothing, False), "OurSearchBox ClearBtn " + Conversions.ToString(Me.Uuid), False)
                Me.BtnClear.IsHitTestVisible = False
            Else
                ModAnimation.AniStart(ModAnimation.AaOpacity(Me.BtnClear, 1.0 - Me.BtnClear.Opacity, 90, 0, Nothing, False), "OurSearchBox ClearBtn " + Conversions.ToString(Me.Uuid), False)
                Me.BtnClear.IsHitTestVisible = True
            End If
            Dim textChangedEvent As OurSearchBox.TextChangedEventHandler = Me.TextChangedEvent
            If textChangedEvent IsNot Nothing Then
                textChangedEvent(RuntimeHelpers.GetObjectValue(sender), e)
            End If
        End Sub

        Private Sub BtnClear_Click(sender As Object, e As EventArgs)
            Me.TextBox.Text = ""
            Me.TextBox.Focus()
        End Sub

        Private Sub OurSearchBox_KeyUp(sender As Object, e As KeyEventArgs)
            If e.Key = Key.[Return] Then
                Me.RaiseCustomEvent()
            End If
        End Sub

        Friend Overridable Property TextBox As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextBox
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim textChangedEventHandler As Global.System.Windows.Controls.TextChangedEventHandler = AddressOf Me.Text_TextChanged
                Dim myTextBox As OurTextBox = Me._TextBox
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                End If
                Me._TextBox = value
                myTextBox = Me._TextBox
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnClear As OurIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnClear
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurIconButton)
                Dim clickEventHandler As OurIconButton.ClickEventHandler = AddressOf Me.BtnClear_Click
                Dim myIconButton As OurIconButton = Me._BtnClear
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnClear = value
                myIconButton = Me._BtnClear
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Public Delegate Sub TextChangedEventHandler(sender As Object, e As EventArgs)
    End Class
End Namespace
