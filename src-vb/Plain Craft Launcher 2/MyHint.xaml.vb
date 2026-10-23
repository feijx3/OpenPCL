Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <ContentProperty("Inlines")>
    <DesignerGenerated()>
    Public Partial Class MyHint
        Inherits Border

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.UpdateUI()
            End Sub
            AddHandler MyBase.Loaded, AddressOf Me.MyHint_Loaded
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.MyHint_MouseUp
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.MyHint_MouseDown
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.MyHint_MouseLeave()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me._ColorType = MyHint.Themes.Red
            Me.RelativeSetup = ""
            Me.IsMouseDown = False
            Me.InitializeComponent()
        End Sub

        Public Property HasBorder As Boolean
            Get
                Return MyBase.BorderThickness.Top > 0.0
            End Get
            Set(value As Boolean)
                If value Then
                    MyBase.BorderThickness = New Thickness(3.0, ModBase.GetWPFSize(1.0), ModBase.GetWPFSize(1.0), ModBase.GetWPFSize(1.0))
                    Return
                End If
                MyBase.BorderThickness = New Thickness(3.0, 0.0, 0.0, 0.0)
            End Set
        End Property

        Public Property Theme As MyHint.Themes
            Get
                Return Me._ColorType
            End Get
            Set(value As MyHint.Themes)
                Me._ColorType = value
                Me.UpdateUI()
            End Set
        End Property

        Private Sub UpdateUI()
            Select Case Me.Theme
                Case MyHint.Themes.Blue
                    MyBase.Background = New ModBase.MyColor("#d9ecff")
                    MyBase.BorderBrush = New ModBase.MyColor("#1172D4")
                    Me.LabText.Foreground = New ModBase.MyColor("#0F64B8")
                    Me.BtnClose.Foreground = New ModBase.MyColor("#0F64B8")
                    Return
                Case MyHint.Themes.Red
                    MyBase.Background = New ModBase.MyColor("#ffdddf")
                    MyBase.BorderBrush = New ModBase.MyColor("#d82929")
                    Me.LabText.Foreground = New ModBase.MyColor("#bf0b0b")
                    Me.BtnClose.Foreground = New ModBase.MyColor("#bf0b0b")
                    Return
                Case MyHint.Themes.Yellow
                    MyBase.Background = New ModBase.MyColor("#ffebd7")
                    MyBase.BorderBrush = New ModBase.MyColor("#f57a00")
                    Me.LabText.Foreground = New ModBase.MyColor("#d86c00")
                    Me.BtnClose.Foreground = New ModBase.MyColor("#d86c00")
                    Return
                Case Else
                    Return
            End Select
        End Sub

        <Obsolete("IsWarn 已过时。请换用 Theme 属性。")>
        Public Property IsWarn As Boolean
            Get
                Return Me.Theme = MyHint.Themes.Red
            End Get
            Set(value As Boolean)
                Me.Theme = If(value, MyHint.Themes.Red, MyHint.Themes.Blue)
            End Set
        End Property

        Public ReadOnly Property Inlines As InlineCollection
            Get
                Return Me.LabText.Inlines
            End Get
        End Property

        Public Property Text As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyHint.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyHint.TextProperty, value)
            End Set
        End Property

        Public Property CanClose As Boolean
            Get
                Return Me.BtnClose.Visibility = Visibility.Visible
            End Get
            Set(value As Boolean)
                Me.BtnClose.Visibility = If(value, Visibility.Visible, Visibility.Collapsed)
            End Set
        End Property

        Public Property RelativeSetup As String

        Private Sub MyHint_Loaded(sender As Object, e As RoutedEventArgs)
            If Me.CanClose AndAlso Settings.[Get](Of Boolean)(Me.RelativeSetup, Nothing) Then
                MyBase.Visibility = Visibility.Collapsed
            End If
        End Sub

        Private Sub BtnClose_Click(sender As Object, e As EventArgs)
            Settings.SetSafe(Me.RelativeSetup, True, Nothing)
            ModAnimation.AniDispose(Me, False, Nothing)
        End Sub

        Public Event Click As MyHint.ClickEventHandler

        Private Sub MyHint_MouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsMouseDown Then
                Me.IsMouseDown = False
                Logger.Info(String.Format("按下提示条{0}", If(String.IsNullOrEmpty(MyBase.Name), "", ("：" + MyBase.Name))), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyHint.xaml.vb")
                Dim clickEvent As MyHint.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                If Not e.Handled Then
                    e.Handled = True
                    Me.RaiseCustomEvent()
                End If
            End If
        End Sub

        Private Sub MyHint_MouseDown(sender As Object, e As MouseButtonEventArgs)
            Me.IsMouseDown = True
        End Sub

        Private Sub MyHint_MouseLeave()
            Me.IsMouseDown = False
        End Sub

        Friend Overridable Property PanBack As MyHint

        Friend Overridable Property LabText As TextBlock

        Friend Overridable Property BtnClose As MyIconButton
            <CompilerGenerated()>
            Get
                Return Me._BtnClose
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.BtnClose_Click
                Dim myIconButton As MyIconButton = Me._BtnClose
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                Me._BtnClose = value
                myIconButton = Me._BtnClose
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Public Uuid As Integer

        Private _ColorType As MyHint.Themes

        Public Shared IsWarnProperty As DependencyProperty = DependencyProperty.Register("IsWarn", GetType(Boolean), GetType(MyHint), New PropertyMetadata(True, Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_6(Of MyHint, DependencyPropertyChangedEventArgs)
            If MyHint._Closure$__.$I0-0 IsNot Nothing Then
                vb$AnonymousDelegate_ = MyHint._Closure$__.$I0-0
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_6(Of MyHint, DependencyPropertyChangedEventArgs) = Sub(d As MyHint, e As DependencyPropertyChangedEventArgs)
                    d.Theme = If(Conversions.ToBoolean(e.NewValue), MyHint.Themes.Red, MyHint.Themes.Blue)
                End Sub
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                MyHint._Closure$__.$I0-0 = vb$AnonymousDelegate_2
            End If
            vb$AnonymousDelegate_(CType(a0, MyHint), a1)
        End Sub))

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyHint), New PropertyMetadata("", Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_6(Of MyHint, DependencyPropertyChangedEventArgs)
            If MyHint._Closure$__.$I0-1 IsNot Nothing Then
                vb$AnonymousDelegate_3 = MyHint._Closure$__.$I0-1
            Else
                Dim vb$AnonymousDelegate_4 As VB$AnonymousDelegate_6(Of MyHint, DependencyPropertyChangedEventArgs) = Sub(d As MyHint, e As DependencyPropertyChangedEventArgs)
                    d.LabText.Text = Conversions.ToString(e.NewValue)
                End Sub
                vb$AnonymousDelegate_3 = vb$AnonymousDelegate_4
                MyHint._Closure$__.$I0-1 = vb$AnonymousDelegate_4
            End If
            vb$AnonymousDelegate_3(CType(a0, MyHint), a1)
        End Sub))

        Private IsMouseDown As Boolean

        Public Enum Themes
            Blue
            Red
            Yellow
        End Enum

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)
    End Class
End Namespace
