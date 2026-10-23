Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class MyTextBox
        Inherits TextBox
        Implements ISettingControl

        Public Sub New()
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.Validate()
            End Sub
            AddHandler MyBase.TextChanged, Sub(a0 As Object, a1 As TextChangedEventArgs)
                Me.MyTextBox_TextChanged(CType(a0, MyTextBox), a1)
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.GotFocus, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.LostFocus, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshColor()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.RefreshTextColor()
            End Sub
            AddHandler MyBase.KeyUp, AddressOf Me.MyTextBox_KeyUp
            AddHandler Me.ValidatedTextChanged, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.SaveSetting()
            End Sub
            Me.HasBackground = True
            Me.ShowValidateResult = True
            Me._labWrong = Nothing
            Me._labHint = Nothing
            Me.Uuid = ModBase.GetUuid()
            Me._ValidateRules = New Collection(Of Validate)()
            Me.ShownValidateResult = MyTextBox.ValidateState.NotInited
            Me.IsTextChanged = False
        End Sub

        Public Property HasBackground As Boolean

        Public Property ShowValidateResult As Boolean

        Public Property CornerRadius As CornerRadius
            Get
                Dim value As Object = MyBase.GetValue(MyTextBox.CornerRadiusProperty)
                If value Is Nothing Then
                    Return Nothing
                End If
                Return CType(value, CornerRadius)
            End Get
            Set(value As CornerRadius)
                MyBase.SetValue(MyTextBox.CornerRadiusProperty, value)
            End Set
        End Property

        Private ReadOnly Property labWrong As TextBlock
            Get
                Dim textBlock As TextBlock
                If MyBase.Template Is Nothing Then
                    textBlock = Nothing
                Else
                    If Me._labWrong Is Nothing Then
                        Me._labWrong = CType(MyBase.Template.FindName("labWrong", Me), TextBlock)
                    End If
                    textBlock = Me._labWrong
                End If
                Return textBlock
            End Get
        End Property

        Private ReadOnly Property labHint As TextBlock
            Get
                Dim textBlock As TextBlock
                If MyBase.Template Is Nothing Then
                    textBlock = Nothing
                Else
                    If Me._labHint Is Nothing Then
                        Me._labHint = CType(MyBase.Template.FindName("labHint", Me), TextBlock)
                    End If
                    textBlock = Me._labHint
                End If
                Return textBlock
            End Get
        End Property

        Public Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            If Operators.CompareString(Me.HintText, "", False) <> 0 AndAlso Operators.CompareString(Me.labHint.Text, "", False) = 0 Then
                Me.labHint.Text = If((Operators.CompareString(MyBase.Text, "", False) = 0), Me.HintText, "")
            End If
        End Sub

        Public Shared Event ValidateChanged As MyTextBox.ValidateChangedEventHandler

        Public ReadOnly Property ChangedEventList As List(Of RoutedEventHandler)
            Get
                If Me._ChangedEventList Is Nothing Then
                    Me._ChangedEventList = New List(Of RoutedEventHandler)()
                End If
                Return Me._ChangedEventList
            End Get
        End Property

        Public Custom Event ValidatedTextChanged As RoutedEventHandler
            AddHandler
                Me.ChangedEventList.Add(value)
            End AddHandler
            RemoveHandler
                Me.ChangedEventList.Remove(value)
            End RemoveHandler
        End Event

        ' Note: this method is marked as 'fire'.
        Private Sub raise_ValidatedTextChanged(sender As Object, e As TextChangedEventArgs)
            Try
                For Each routedEventHandler As RoutedEventHandler In Me.ChangedEventList
                    If Not Information.IsNothing(routedEventHandler) Then
                        routedEventHandler(RuntimeHelpers.GetObjectValue(sender), e)
                    End If
                Next
            Finally
                Dim enumerator As List(Of RoutedEventHandler).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
        End Sub

        Public Property ValidateResult As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyTextBox.ValidateResultProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyTextBox.ValidateResultProperty, value)
            End Set
        End Property

        Public ReadOnly Property IsValidated As Boolean
            Get
                Return Conversions.ToBoolean(MyBase.GetValue(MyTextBox.IsValidatedProperty))
            End Get
        End Property

        Public Property ValidateRules As Collection(Of Validate)
            Get
                Return Me._ValidateRules
            End Get
            Set(value As Collection(Of Validate))
                Me._ValidateRules = value
                Me.Validate()
            End Set
        End Property

        Public Sub Validate()
            Me.ValidateResult = ModValidate.Validate(MyBase.Text, Me.ValidateRules)
            If Me.ShownValidateResult <> If(Me.IsValidated, MyTextBox.ValidateState.Success, MyTextBox.ValidateState.FailedAndShowDetail) Then
                If MyBase.IsLoaded AndAlso Me.labWrong IsNot Nothing Then
                    Me.ChangeValidateResult(Me.IsValidated, True)
                Else
                    ModBase.RunInNewThread(Sub()
                        Thread.Sleep(30)
                        ModBase.RunInUi(Sub()
                            Me.ChangeValidateResult(Me.IsValidated, False)
                        End Sub, False)
                    End Sub, "DelayedValidate Change", ThreadPriority.Normal)
                End If
            End If
            If Me.ShowValidateResult AndAlso Not Me.IsValidated Then
                If MyBase.IsLoaded AndAlso Me.labWrong IsNot Nothing Then
                    Me.labWrong.Text = Me.ValidateResult
                    Return
                End If
                ModBase.RunInNewThread(Sub()
                    Dim CS$<>8__locals1 As MyTextBox._Closure$__46-0 = New MyTextBox._Closure$__46-0(CS$<>8__locals1)
                    CS$<>8__locals1.$VB$Me = Me
                    CS$<>8__locals1.$VB$Local_IsFinished = False
                    While Not CS$<>8__locals1.$VB$Local_IsFinished
                        Thread.Sleep(20)
                        ModBase.RunInUiWait(Sub()
                            If CS$<>8__locals1.$VB$Me.labWrong IsNot Nothing Then
                                CS$<>8__locals1.$VB$Me.labWrong.Text = CS$<>8__locals1.$VB$Me.ValidateResult
                                CS$<>8__locals1.$VB$Local_IsFinished = True
                            End If
                            If Not CS$<>8__locals1.$VB$Me.IsLoaded Then
                                CS$<>8__locals1.$VB$Local_IsFinished = True
                            End If
                        End Sub)
                    End While
                End Sub, "DelayedValidate Text", ThreadPriority.Normal)
            End If
        End Sub

        Public Sub ForceShowAsSuccess()
            Me.IsTextChanged = False
            Me.ChangeValidateResult(Me.IsValidated, True)
        End Sub

        Private Sub ChangeValidateResult(IsSuccessful As Boolean, IsLoaded As Boolean)
            If IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 AndAlso Me.labWrong IsNot Nothing Then
                If IsSuccessful OrElse Not Me.IsTextChanged Then
                    Me.ShownValidateResult = If(IsSuccessful, MyTextBox.ValidateState.Success, MyTextBox.ValidateState.FailedButTextNotChanged)
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.labWrong, -Me.labWrong.Opacity, 150, 0, Nothing, False), ModAnimation.AaHeight(Me.labWrong, -Me.labWrong.Height, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaCode(Sub()
                        Me.labWrong.Visibility = Visibility.Collapsed
                    End Sub, 0, True) }, "MyTextBox Validate " + Conversions.ToString(Me.Uuid), False)
                ElseIf Me.ShowValidateResult Then
                    Me.ShownValidateResult = MyTextBox.ValidateState.FailedAndShowDetail
                    Me.labWrong.Visibility = Visibility.Visible
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.labWrong, 1.0 - Me.labWrong.Opacity, 150, 0, Nothing, False), ModAnimation.AaHeight(Me.labWrong, 21.0 - Me.labWrong.Height, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) }, "MyTextBox Validate " + Conversions.ToString(Me.Uuid), False)
                Else
                    Me.ShownValidateResult = MyTextBox.ValidateState.FailedAndHideDetail
                End If
            Else
                Me.ShownValidateResult = MyTextBox.ValidateState.NotLoaded
            End If
            Me.RefreshColor()
            Dim validateChangedEvent As MyTextBox.ValidateChangedEventHandler = MyTextBox.ValidateChangedEvent
            If validateChangedEvent IsNot Nothing Then
                validateChangedEvent(Me, New EventArgs())
            End If
        End Sub

        Public Property HintText As String
            Get
                Return Conversions.ToString(MyBase.GetValue(MyTextBox.HintTextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyTextBox.HintTextProperty, value)
            End Set
        End Property

        Private Sub MyTextBox_TextChanged(sender As MyTextBox, e As TextChangedEventArgs)
            Try
                If Me.labHint IsNot Nothing Then
                    Me.labHint.Text = If((Operators.CompareString(MyBase.Text, "", False) = 0), Me.HintText, "")
                End If
                Me.IsTextChanged = MyBase.IsLoaded
                Me.Validate()
                If Me.IsValidated Then
                    Me.raise_ValidatedTextChanged(sender, e)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "进行输入验证时出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyTextBox.vb")
            End Try
        End Sub

        Private Sub RefreshColor()
            Try
                If MyBase.TemplatedParent Is Nothing OrElse Not(TypeOf MyBase.TemplatedParent Is MyComboBox) Then
                    Dim text As String
                    Dim text2 As String
                    Dim num As Integer
                    If MyBase.IsEnabled Then
                        If Me.IsValidated OrElse Not Me.IsTextChanged Then
                            If MyBase.IsFocused Then
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
                            text = "ColorBrushRedLight"
                            text2 = "ColorBrushRedBack"
                            num = 200
                        End If
                    Else
                        text = "ColorBrushGray5"
                        text2 = "ColorBrushGray6"
                        num = 200
                    End If
                    If Not Me.HasBackground Then
                        text2 = "ColorBrushTransparent"
                    End If
                    If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                        ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me, Control.BorderBrushProperty, text, num, 0, Nothing, False), ModAnimation.AaColor(Me, Control.BackgroundProperty, text2, num, 0, Nothing, False) }, "MyTextBox Color " + Conversions.ToString(Me.Uuid), False)
                    Else
                        ModAnimation.AniStop("MyTextBox Color " + Conversions.ToString(Me.Uuid))
                        MyBase.SetResourceReference(Control.BorderBrushProperty, text)
                        MyBase.SetResourceReference(Control.BackgroundProperty, text2)
                    End If
                End If
            Catch ex As Exception
                Logger.Warn(ex, "文本框颜色改变出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyTextBox.vb")
            End Try
        End Sub

        Private Sub RefreshTextColor()
            Dim myColor As ModBase.MyColor = If(MyBase.IsEnabled, ModSecret.ColorGray1, ModSecret.ColorGray4)
            If CDbl(CType(MyBase.Foreground, SolidColorBrush).Color.R) <> myColor.R Then
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 AndAlso Operators.CompareString(MyBase.Text, "", False) <> 0 Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me, Control.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrushGray1", "ColorBrushGray4"), 200, 0, Nothing, False) }, "MyTextBox TextColor " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStop("MyTextBox TextColor " + Conversions.ToString(Me.Uuid))
                MyBase.Foreground = myColor
            End If
        End Sub

        Private Sub MyTextBox_KeyUp(sender As Object, e As KeyEventArgs)
            If e.Key = Key.[Return] Then
                Me.RaiseCustomEvent()
            End If
        End Sub

        Sub RefreshSetting(NewValue As String) Implements PCL.ISettingControl.RefreshSetting
            MyBase.Text = NewValue
        End Sub

        Function GetCurrentSetting() As String Implements PCL.ISettingControl.GetCurrentSetting
            Return MyBase.Text
        End Function

        Private Sub SaveSetting()
            SettingService.SaveSetting(Me)
        End Sub

        Public Shared CornerRadiusProperty As DependencyProperty = DependencyProperty.Register("CornerRadius", GetType(CornerRadius), GetType(MyTextBox), New PropertyMetadata(New CornerRadius(3.0)))

        Private _labWrong As TextBlock

        Private _labHint As TextBlock

        Public Uuid As Integer

        Private _ChangedEventList As List(Of RoutedEventHandler)

        Public Shared ValidateResultProperty As DependencyProperty = DependencyProperty.Register("ValidateResult", GetType(String), GetType(MyTextBox), New PropertyMetadata("", Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_6(Of MyTextBox, DependencyPropertyChangedEventArgs)
            If MyTextBox._Closure$__.$I0-0 IsNot Nothing Then
                vb$AnonymousDelegate_ = MyTextBox._Closure$__.$I0-0
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_6(Of MyTextBox, DependencyPropertyChangedEventArgs) = Sub(d As MyTextBox, e As DependencyPropertyChangedEventArgs)
                    d.SetValue(MyTextBox.IsValidatedPropertyKey, String.IsNullOrEmpty(Conversions.ToString(e.NewValue)))
                End Sub
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                MyTextBox._Closure$__.$I0-0 = vb$AnonymousDelegate_2
            End If
            vb$AnonymousDelegate_(CType(a0, MyTextBox), a1)
        End Sub))

        Private Shared IsValidatedPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly("IsValidated", GetType(Boolean), GetType(MyTextBox), New PropertyMetadata(True))

        Public Shared IsValidatedProperty As DependencyProperty = MyTextBox.IsValidatedPropertyKey.DependencyProperty

        Private _ValidateRules As Collection(Of Validate)

        Private ShownValidateResult As MyTextBox.ValidateState

        Private IsTextChanged As Boolean

        Public Shared HintTextProperty As DependencyProperty = DependencyProperty.Register("HintText", GetType(String), GetType(MyTextBox), New PropertyMetadata("", Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_7(Of MyTextBox, DependencyPropertyChangedEventArgs)
            If MyTextBox._Closure$__.$I0-1 IsNot Nothing Then
                vb$AnonymousDelegate_3 = MyTextBox._Closure$__.$I0-1
            Else
                Dim vb$AnonymousDelegate_4 As VB$AnonymousDelegate_7(Of MyTextBox, DependencyPropertyChangedEventArgs) = Sub(t As MyTextBox, e As DependencyPropertyChangedEventArgs)
                    If t.labHint IsNot Nothing Then
                        t.labHint.Text = If((Operators.CompareString(t.Text, "", False) = 0), t.HintText, "")
                    End If
                End Sub
                vb$AnonymousDelegate_3 = vb$AnonymousDelegate_4
                MyTextBox._Closure$__.$I0-1 = vb$AnonymousDelegate_4
            End If
            vb$AnonymousDelegate_3(CType(a0, MyTextBox), a1)
        End Sub))

        Public Delegate Sub ValidateChangedEventHandler(sender As Object, e As EventArgs)

        Private Enum ValidateState
            NotInited
            Success
            FailedButTextNotChanged
            FailedAndShowDetail
            FailedAndHideDetail
            NotLoaded
        End Enum
    End Class
End Namespace
