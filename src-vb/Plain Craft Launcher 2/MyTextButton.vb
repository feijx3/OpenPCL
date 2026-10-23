Imports System
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class MyTextButton
        Inherits Label

        Public Event Click As MyTextButton.ClickEventHandler

        Public Sub New()
            AddHandler MyBase.PreviewMouseLeftButtonDown, AddressOf Me.MyTextButton_MouseLeftButtonDown
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.MyTextButton_MouseLeave()
            End Sub
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.MyTextButton_MouseLeftButtonUp
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeftButtonDown, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.RefreshColor()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me.IsMouseDown = False
            MyBase.SetResourceReference(Control.ForegroundProperty, "ColorBrush1")
            MyBase.Background = ModSecret.ColorSemiTransparent
        End Sub

        Public Property Text As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyTextButton.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyTextButton.TextProperty, value)
            End Set
        End Property

        Private Sub MyTextButton_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
            Me.IsMouseDown = True
            e.Handled = True
        End Sub

        Private Sub MyTextButton_MouseLeave()
            Me.IsMouseDown = False
        End Sub

        Private Sub MyTextButton_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsMouseDown Then
                Me.IsMouseDown = False
                Logger.Info(String.Format("按下文本按钮：{0}", Me.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyTextButton.vb")
                Dim clickEvent As MyTextButton.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(Me, Nothing)
                End If
                Me.RaiseCustomEvent()
                e.Handled = True
            End If
        End Sub

        Private Sub RefreshColor()
            Dim text As String
            Dim num As Integer
            If Me.IsMouseDown Then
                text = "ColorBrush4"
                num = 30
            ElseIf MyBase.IsMouseOver Then
                text = "ColorBrush3"
                num = 100
            Else
                text = "ColorBrush1"
                num = 200
            End If
            If Operators.CompareString(Me.ColorName, text, False) <> 0 Then
                Me.ColorName = text
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    ModAnimation.AniStart(ModAnimation.AaColor(Me, Control.ForegroundProperty, text, num, 0, Nothing, False), "MyTextButton Color " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStop("MyTextButton Color " + Conversions.ToString(Me.Uuid))
                MyBase.SetResourceReference(Control.ForegroundProperty, text)
            End If
        End Sub

        Public Uuid As Integer

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyTextButton), New PropertyMetadata("", Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_8(Of MyTextButton, DependencyPropertyChangedEventArgs)
            If MyTextButton._Closure$__.$I0-0 IsNot Nothing Then
                vb$AnonymousDelegate_ = MyTextButton._Closure$__.$I0-0
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_8(Of MyTextButton, DependencyPropertyChangedEventArgs) = Sub(sender As MyTextButton, e As DependencyPropertyChangedEventArgs)
                    If Not Operators.ConditionalCompareObjectEqual(e.OldValue, e.NewValue, False) Then
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(sender, -sender.Opacity, 50, 0, Nothing, False), ModAnimation.AaCode(Sub()
                            sender.Content = RuntimeHelpers.GetObjectValue(e.NewValue)
                        End Sub, 0, True), ModAnimation.AaOpacity(sender, 1.0, 170, 0, Nothing, False) }, "MyTextButton Text " + Conversions.ToString(sender.Uuid), False)
                    End If
                End Sub
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                MyTextButton._Closure$__.$I0-0 = vb$AnonymousDelegate_2
            End If
            vb$AnonymousDelegate_(CType(a0, MyTextButton), a1)
        End Sub))

        Public IsMouseDown As Boolean

        Private Const AnimationTimeIn As Integer = 100

        Private Const AnimationTimeOut As Integer = 200

        Private ColorName As String

        Public Delegate Sub ClickEventHandler(sender As Object, e As EventArgs)
    End Class
End Namespace
