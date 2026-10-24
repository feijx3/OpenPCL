Imports System
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class OurComboBox
        Inherits ComboBox
        Implements ISettingControl

        Public Sub New()
            AddHandler MyBase.PreviewMouseLeftButtonDown, AddressOf Me.OurComboBox_PreviewMouseLeftButtonDown
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.OurComboBox_PreviewMouseLeftButtonUp
            AddHandler MyBase.MouseLeave, AddressOf Me.OurComboBox_PreviewMouseLeftButtonUp
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.PreviewMouseLeftButtonDown, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.PreviewMouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.GotKeyboardFocus, Sub(a0 As Object, a1 As KeyboardFocusChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.DropDownOpened, AddressOf Me.OurComboBox_DropDownOpened
            AddHandler MyBase.DropDownClosed, AddressOf Me.OurComboBox_DropDownClosed
            AddHandler Me.TextChanged, AddressOf Me.OurComboBox_TextChanged
            AddHandler MyBase.SelectionChanged, AddressOf Me.OurComboBox_SelectionChanged
            AddHandler MyBase.SelectionChanged, Sub(a0 As Object, a1 As SelectionChangedEventArgs)
                Me.SaveSetting()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me.HintText = ""
            Me._Text = Conversions.ToString(MyBase.SelectedItem)
            Me.IsMouseDown = False
            Me.IsTextChanging = False
        End Sub

        Public Event TextChanged As OurComboBox.TextChangedEventHandler

        Public Property HintText As String

        Public Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            If MyBase.IsEditable Then
                Try
                    Me.TextBox = CType(MyBase.Template.FindName("PART_EditableTextBox", Me), OurTextBox)
                    Me.TextBox.[AddHandler](UIElement.LostFocusEvent, New RoutedEventHandler(Sub(a0 As Object, a1 As RoutedEventArgs)
                        Me.RefreshColor()
                    End Sub))
                    Me.TextBox.ChangedEventList.Add(Sub(sender As Object, e As RoutedEventArgs)
                        Dim textChangedEvent2 As OurComboBox.TextChangedEventHandler = Me.TextChangedEvent
                        If textChangedEvent2 IsNot Nothing Then
                            textChangedEvent2(RuntimeHelpers.GetObjectValue(sender), CType(e, TextChangedEventArgs))
                        End If
                    End Sub)
                    Me.TextBox.Tag = RuntimeHelpers.GetObjectValue(MyBase.Tag)
                    If Operators.CompareString(Me.Text, "", False) = 0 Then
                        Me.TextBox.Text = Me._Text
                    Else
                        Dim textChangedEvent As OurComboBox.TextChangedEventHandler = Me.TextChangedEvent
                        If textChangedEvent IsNot Nothing Then
                            textChangedEvent(Me, Nothing)
                        End If
                    End If
                    If Me.HintText.Length > 0 Then
                        Me.TextBox.HintText = Me.HintText
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, String.Format("初始化可编辑文本框失败（{0}）", If(MyBase.Name, "")), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurComboBox.vb")
                End Try
            End If
        End Sub

        Public Property Text As String
            Get
                Dim text As String
                If MyBase.IsEditable Then
                    If Me.TextBox Is Nothing Then
                        text = If(Me._Text, "")
                    Else
                        text = If(Me.TextBox.Text, "")
                    End If
                Else
                    text = (If(MyBase.SelectedItem, "")).ToString()
                End If
                Return text
            End Get
            Set(value As String)
                If Not MyBase.IsEditable Then
                    Throw New NotSupportedException("该 ComboBox 不支持修改文本。")
                End If
                If Information.IsNothing(Me.TextBox) Then
                    Me._Text = value
                    Return
                End If
                Me.TextBox.Text = value
            End Set
        End Property

        Private Sub OurComboBox_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
            Me.IsMouseDown = True
        End Sub

        Private Sub OurComboBox_PreviewMouseLeftButtonUp(sender As Object, e As EventArgs)
            Me.IsMouseDown = False
        End Sub

        Public Sub RefreshColor()
            Dim text As String
            Dim text2 As String
            Dim num As Integer
            If MyBase.IsEnabled Then
                If Conversions.ToBoolean(Me.IsMouseDown OrElse MyBase.IsDropDownOpen OrElse (MyBase.IsEditable AndAlso Conversions.ToBoolean(NewLateBinding.LateGet(MyBase.Template.FindName("PART_EditableTextBox", Me), Nothing, "IsFocused", New Object(-1) {}, Nothing, Nothing, Nothing)))) Then
                    text = "ColorBrush3"
                    text2 = "ColorBrush7"
                    num = 10
                ElseIf MyBase.IsMouseOver Then
                    text = "ColorBrush4"
                    text2 = "ColorBrush7"
                    num = 100
                Else
                    text = "ColorBrushBg0"
                    text2 = "ColorBrushHalfWhite"
                    num = 100
                End If
            Else
                text = "ColorBrushGray5"
                text2 = "ColorBrushGray6"
                num = 200
            End If
            If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me, Control.ForegroundProperty, text, num, 0, Nothing, False), ModAnimation.AaColor(Me, Control.BackgroundProperty, text2, num, 0, Nothing, False) }, "OurComboBox Color " + Conversions.ToString(Me.Uuid), False)
                Return
            End If
            ModAnimation.AniStop("OurComboBox Color " + Conversions.ToString(Me.Uuid))
            MyBase.SetResourceReference(Control.ForegroundProperty, text)
            MyBase.SetResourceReference(Control.BackgroundProperty, text2)
        End Sub

        Private Sub OurComboBox_DropDownOpened(sender As Object, e As EventArgs)
            Me.RealWidth = MyBase.Width
            MyBase.Width = MyBase.ActualWidth
            Try
                CType(MyBase.Template.FindName("PanPopup", Me), Grid).Opacity = ModMain.FrmMain.Opacity
            Catch ex As Exception
                Logger.[Error](ex, "设置下拉框透明度失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurComboBox.vb")
            End Try
        End Sub

        Private Sub OurComboBox_DropDownClosed(sender As Object, e As EventArgs)
            MyBase.Width = Me.RealWidth
        End Sub

        Private Sub OurComboBox_TextChanged(sender As Object, e As TextChangedEventArgs)
            If Not Me.IsTextChanging AndAlso MyBase.IsEditable AndAlso MyBase.SelectedItem IsNot Nothing AndAlso Operators.CompareString(Me.Text, MyBase.SelectedItem.ToString(), False) <> 0 Then
                Dim text As String = Me.Text
                Dim selectionStart As Integer = Me.TextBox.SelectionStart
                Me.IsTextChanging = True
                MyBase.SelectedItem = Nothing
                Me.Text = text
                Me.TextBox.SelectionStart = selectionStart
                Me.IsTextChanging = False
            End If
        End Sub

        Public ReadOnly Property ContentPresenter As ContentPresenter
            Get
                Return CType(MyBase.Template.FindName("PART_Content", Me), ContentPresenter)
            End Get
        End Property

        Private Sub OurComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                Me.RaiseCustomEvent()
            End If
        End Sub

        Sub RefreshSetting(NewValue As String) Implements PCL.ISettingControl.RefreshSetting
            Dim dependencyObject As DependencyObject = MyBase.Items.OfType().FirstOrDefault(Function(i As DependencyObject) ModBase.Val(RuntimeHelpers.GetObjectValue(If(SettingService.GetValue(i), (-1)))) = Conversions.ToDouble(NewValue))
            If dependencyObject IsNot Nothing Then
                MyBase.SelectedItem = dependencyObject
                Return
            End If
            If CDbl(MyBase.Items.Count) > Conversions.ToDouble(NewValue) Then
                MyBase.SelectedIndex = Conversions.ToInteger(NewValue)
                Return
            End If
            Logger.Warn(String.Format("尝试显示设置 {0}，但没有找到设置值 {1} 的对应项", SettingService.GetKey(Me), NewValue), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurComboBox.vb")
            MyBase.SelectedIndex = -1
            MyBase.SelectedItem = Nothing
        End Sub

        Function GetCurrentSetting() As String Implements PCL.ISettingControl.GetCurrentSetting
            Dim text As String
            If MyBase.SelectedItem Is Nothing Then
                text = Nothing
            ElseIf TypeOf MyBase.SelectedItem Is DependencyObject Then
                text = Conversions.ToString(If(SettingService.GetValue(CType(MyBase.SelectedItem, DependencyObject)), MyBase.SelectedIndex))
            Else
                text = Conversions.ToString(MyBase.SelectedIndex)
            End If
            Return text
        End Function

        Private Sub SaveSetting()
            If MyBase.SelectedItem IsNot Nothing Then
                SettingService.SaveSetting(Me)
            End If
        End Sub

        Public Uuid As Integer

        Private TextBox As OurTextBox

        Private _Text As String

        Private IsMouseDown As Boolean

        Private RealWidth As Double

        Private IsTextChanging As Boolean

        Public Delegate Sub TextChangedEventHandler(sender As Object, e As TextChangedEventArgs)
    End Class
End Namespace
