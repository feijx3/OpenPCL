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
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <ContentProperty("Inlines")>
    <DesignerGenerated()>
    Public Partial Class OurCheckBox
        Inherits Grid
        Implements ISettingControl

        Public Sub New()
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Checkbox_MouseUp()
            End Sub
            AddHandler MyBase.MouseLeftButtonDown, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Checkbox_MouseDown()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Checkbox_MouseLeave()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.Checkbox_IsEnabledChanged()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Checkbox_MouseEnterAnimation()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Checkbox_MouseLeaveAnimation()
            End Sub
            AddHandler Me.Change, Sub(a0 As Object, a1 As Boolean)
                Me.SaveSetting()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me.MouseDowned = False
            Me.AllowMouseDown = True
            Me.InitializeComponent()
        End Sub

        Public Event Change As OurCheckBox.ChangeEventHandler

        Public Event PreviewChange As OurCheckBox.PreviewChangeEventHandler

        Public Property Checked As Boolean
            Get
                Return Conversions.ToBoolean(MyBase.GetValue(OurCheckBox.CheckedProperty))
            End Get
            Set(value As Boolean)
                Me.SetChecked(value, False)
            End Set
        End Property

        Public Sub SetChecked(value As Boolean, user As Boolean)
            Try
                If value <> Me.Checked Then
                    If value AndAlso user Then
                        Dim e As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(user)
                        Dim previewChangeEvent As OurCheckBox.PreviewChangeEventHandler = Me.PreviewChangeEvent
                        If previewChangeEvent IsNot Nothing Then
                            previewChangeEvent(Me, e)
                        End If
                        If e.Handled Then
                            Me.MouseDowned = True
                            Me.Checkbox_MouseLeave()
                            Me.MouseDowned = False
                            Return
                        End If
                    End If
                    MyBase.SetValue(OurCheckBox.CheckedProperty, value)
                    If MyBase.IsLoaded Then
                        Dim changeEvent As OurCheckBox.ChangeEventHandler = Me.ChangeEvent
                        If changeEvent IsNot Nothing Then
                            changeEvent(Me, user)
                        End If
                    End If
                    Me.SyncUI()
                    Me.RaiseCustomEvent()
                End If
            Catch ex As Exception
                Logger.Warn(ex, "设置 Checked 失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurCheckBox.xaml.vb")
            End Try
        End Sub

        Private Sub SyncUI()
            If ModAnimation.AniControlEnabled = 0 AndAlso MyBase.IsLoaded Then
                Me.AllowMouseDown = False
                If Me.Checked Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeBorder, 12.0 - Me.ShapeBorder.Width, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False, True), ModAnimation.AaScaleTransform(Me.ShapeCheck, 1.0 - CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleX, 300, 105, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaScale(Me.ShapeBorder, 6.0, 300, 105, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False, True) }, "OurCheckBox Scale " + Conversions.ToString(Me.Uuid), False)
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeBorder, Border.BorderBrushProperty, If(MyBase.IsEnabled, If(MyBase.IsMouseOver, "ColorBrush3", "ColorBrush2"), "ColorBrushGray5"), 150, 0, Nothing, False) }, "OurCheckBox BorderColor " + Conversions.ToString(Me.Uuid), False)
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaCode(Sub()
                        Me.AllowMouseDown = True
                    End Sub, 300, False) }, "OurCheckBox AllowMouseDown " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeBorder, 12.0 - Me.ShapeBorder.Width, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False, True), ModAnimation.AaScaleTransform(Me.ShapeCheck, -CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleX, 135, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaScale(Me.ShapeBorder, 6.0, 300, 105, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False, True) }, "OurCheckBox Scale " + Conversions.ToString(Me.Uuid), False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeBorder, Border.BorderBrushProperty, If(MyBase.IsEnabled, If(MyBase.IsMouseOver, "ColorBrush3", "ColorBrush1"), "ColorBrushGray5"), 150, 0, Nothing, False) }, "OurCheckBox BorderColor " + Conversions.ToString(Me.Uuid), False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaCode(Sub()
                    Me.AllowMouseDown = True
                End Sub, 300, False) }, "OurCheckBox AllowMouseDown " + Conversions.ToString(Me.Uuid), False)
                Return
            Else
                ModAnimation.AniStop("OurCheckBox Scale " + Conversions.ToString(Me.Uuid))
                ModAnimation.AniStop("OurCheckBox BorderColor " + Conversions.ToString(Me.Uuid))
                ModAnimation.AniStop("OurCheckBox AllowMouseDown " + Conversions.ToString(Me.Uuid))
                If Me.Checked Then
                    CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleX = 1.0
                    CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleY = 1.0
                    Me.ShapeBorder.SetResourceReference(Border.BorderBrushProperty, If(MyBase.IsEnabled, "ColorBrush2", "ColorBrushGray5"))
                    Return
                End If
                CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleX = 0.0
                CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleY = 0.0
                Me.ShapeBorder.SetResourceReference(Border.BorderBrushProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"))
                Return
            End If
        End Sub

        Public ReadOnly Property Inlines As InlineCollection
            Get
                Return Me.LabText.Inlines
            End Get
        End Property

        Public Property Text As String
            Get
                Return Conversions.ToString(MyBase.GetValue(OurCheckBox.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(OurCheckBox.TextProperty, value)
            End Set
        End Property

        Private Sub Checkbox_MouseUp()
            If Me.MouseDowned Then
                Logger.Info(String.Format("按下复选框（{0}）：{1}", Not Me.Checked, Me.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Controls\OurCheckBox.xaml.vb")
                Me.MouseDowned = False
                Me.SetChecked(Not Me.Checked, True)
                ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Border.BackgroundProperty, "ColorBrushHalfWhite", 100, 0, Nothing, False), "OurCheckBox Background " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub Checkbox_MouseDown()
            If Me.AllowMouseDown Then
                Me.MouseDowned = True
                MyBase.Focus()
                ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, Nothing, False), "OurCheckBox Background " + Conversions.ToString(Me.Uuid), False)
                If Me.Checked Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeBorder, 16.5 - Me.ShapeBorder.Width, 1000, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False, True), ModAnimation.AaScaleTransform(Me.ShapeCheck, 0.9 - CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleX, 1000, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False) }, "OurCheckBox Scale " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStart(ModAnimation.AaScale(Me.ShapeBorder, 16.5 - Me.ShapeBorder.Width, 1000, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False, True), "OurCheckBox Scale " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub Checkbox_MouseLeave()
            If Me.MouseDowned Then
                Me.MouseDowned = False
                ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Border.BackgroundProperty, "ColorBrushHalfWhite", 100, 0, Nothing, False), "OurCheckBox Background " + Conversions.ToString(Me.Uuid), False)
                If Me.Checked Then
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeBorder, 18.0 - Me.ShapeBorder.Width, 400, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False, True), ModAnimation.AaScaleTransform(Me.ShapeCheck, 1.0 - CType(Me.ShapeCheck.RenderTransform, ScaleTransform).ScaleX, 500, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False) }, "OurCheckBox Scale " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStart(ModAnimation.AaScale(Me.ShapeBorder, 18.0 - Me.ShapeBorder.Width, 400, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False, True), "OurCheckBox Scale " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub Checkbox_IsEnabledChanged()
            If Not MyBase.IsLoaded OrElse ModAnimation.AniControlEnabled <> 0 Then
                ModAnimation.AniStop("OurCheckBox TextColor " + Conversions.ToString(Me.Uuid))
                ModAnimation.AniStop("OurCheckBox BorderColor " + Conversions.ToString(Me.Uuid))
                Me.LabText.SetResourceReference(TextBlock.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"))
                Me.ShapeBorder.SetResourceReference(Border.BorderBrushProperty, If(MyBase.IsEnabled, If(Me.Checked, "ColorBrush2", "ColorBrush1"), "ColorBrushGray5"))
                Return
            End If
            If MyBase.IsEnabled Then
                Me.Checkbox_MouseLeaveAnimation()
                Return
            End If
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeBorder, Border.BorderBrushProperty, ModSecret.ColorGray5 - Me.ShapeBorder.BorderBrush, 200, 0, Nothing, False) }, "OurCheckBox BorderColor " + Conversions.ToString(Me.Uuid), False)
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, ModSecret.ColorGray5 - Me.LabText.Foreground, 200, 0, Nothing, False) }, "OurCheckBox TextColor " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub Checkbox_MouseEnterAnimation()
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, Nothing, False) }, "OurCheckBox TextColor " + Conversions.ToString(Me.Uuid), False)
            ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeBorder, Border.BorderBrushProperty, "ColorBrush3", 100, 0, Nothing, False) }, "OurCheckBox BorderColor " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub Checkbox_MouseLeaveAnimation()
            If MyBase.IsEnabled Then
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"), 200, 0, Nothing, False) }, "OurCheckBox TextColor " + Conversions.ToString(Me.Uuid), False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeBorder, Border.BorderBrushProperty, If(MyBase.IsEnabled, If(Me.Checked, "ColorBrush2", "ColorBrush1"), "ColorBrushGray5"), 200, 0, Nothing, False) }, "OurCheckBox BorderColor " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Sub RefreshSetting(NewValue As String) Implements PCL.ISettingControl.RefreshSetting
            Me.Checked = Boolean.Parse(NewValue)
        End Sub

        Function GetCurrentSetting() As String Implements PCL.ISettingControl.GetCurrentSetting
            Return Me.Checked.ToString()
        End Function

        Private Sub SaveSetting()
            SettingService.SaveSetting(Me)
        End Sub

        Friend Overridable Property PanBack As OurCheckBox

        Public Overridable Property LabText As TextBlock

        Friend Overridable Property ShapeBorder As Border

        Friend Overridable Property ShapeCheck As Path

        Public Uuid As Integer

        Public Shared CheckedProperty As DependencyProperty = DependencyProperty.Register("Checked", GetType(Boolean), GetType(OurCheckBox), New PropertyMetadata(False, Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_6(Of OurCheckBox, DependencyPropertyChangedEventArgs)
            If OurCheckBox._Closure$__.$I0-0 IsNot Nothing Then
                vb$AnonymousDelegate_ = OurCheckBox._Closure$__.$I0-0
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_6(Of OurCheckBox, DependencyPropertyChangedEventArgs) = Sub(d As OurCheckBox, e As DependencyPropertyChangedEventArgs)
                    If Not d.IsLoaded Then
                        d.SyncUI()
                    End If
                End Sub
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                OurCheckBox._Closure$__.$I0-0 = vb$AnonymousDelegate_2
            End If
            vb$AnonymousDelegate_(CType(a0, OurCheckBox), a1)
        End Sub))

        Private Const AnimationTimeOfCheck As Integer = 150

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(OurCheckBox), New PropertyMetadata(Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If Not Information.IsNothing(sender) Then
                CType(sender, OurCheckBox).LabText.Text = Conversions.ToString(e.NewValue)
            End If
        End Sub))

        Private MouseDowned As Boolean

        Private AllowMouseDown As Boolean

        Private Const AnimationTimeOfMouseIn As Integer = 100

        Private Const AnimationTimeOfMouseOut As Integer = 200

        Public Delegate Sub ChangeEventHandler(sender As Object, user As Boolean)

        Public Delegate Sub PreviewChangeEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Class
End Namespace
