Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <ContentProperty("Inlines")>
    <DesignerGenerated()>
    Public Partial Class MyRadioBox
        Inherits Grid
        Implements IMyRadio, ISettingControl

        Public Sub New()
            AddHandler MyBase.MouseLeftButtonUp, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Radiobox_MouseUp()
            End Sub
            AddHandler MyBase.MouseLeftButtonDown, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.Radiobox_MouseDown()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Radiobox_MouseLeave()
            End Sub
            AddHandler MyBase.IsEnabledChanged, Sub(a0 As Object, a1 As DependencyPropertyChangedEventArgs)
                Me.Radiobox_IsEnabledChanged()
            End Sub
            AddHandler MyBase.MouseEnter, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Radiobox_MouseEnterAnimation()
            End Sub
            AddHandler MyBase.MouseLeave, Sub(a0 As Object, a1 As MouseEventArgs)
                Me.Radiobox_MouseLeaveAnimation()
            End Sub
            AddHandler Me.Check, Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                Me.SaveSetting()
            End Sub
            Me.Uuid = ModBase.GetUuid()
            Me.MouseDowned = False
            Me.AllowMouseDown = True
            Me.InitializeComponent()
        End Sub

        Public Event PreviewCheck As MyRadioBox.PreviewCheckEventHandler

        Public Event PreviewChange As MyRadioBox.PreviewChangeEventHandler

        Public Event Check As IMyRadio.CheckEventHandler Implements PCL.IMyRadio.Check

        Public Event Changed As IMyRadio.ChangedEventHandler Implements PCL.IMyRadio.Changed

        Public Property Checked As Boolean
            Get
                Return Conversions.ToBoolean(MyBase.GetValue(MyRadioBox.CheckedProperty))
            End Get
            Set(value As Boolean)
                Me.SetChecked(value, False)
            End Set
        End Property

        Public Sub SetChecked(value As Boolean, user As Boolean)
            ' The following expression was wrapped in a checked-statement
            Try
                If value AndAlso user Then
                    Dim e As ModBase.RouteEventArgs = New ModBase.RouteEventArgs(user)
                    Dim previewCheckEvent As MyRadioBox.PreviewCheckEventHandler = Me.PreviewCheckEvent
                    If previewCheckEvent IsNot Nothing Then
                        previewCheckEvent(Me, e)
                    End If
                    If e.Handled Then
                        Me.Radiobox_MouseLeave()
                        Return
                    End If
                End If
                Dim flag As Boolean = False
                If MyBase.IsLoaded AndAlso value <> Me.Checked Then
                    Dim previewChangeEvent As MyRadioBox.PreviewChangeEventHandler = Me.PreviewChangeEvent
                    If previewChangeEvent IsNot Nothing Then
                        previewChangeEvent(Me, New ModBase.RouteEventArgs(user))
                    End If
                End If
                If value <> Me.Checked Then
                    MyBase.SetValue(MyRadioBox.CheckedProperty, value)
                    flag = True
                End If
                If MyBase.Parent IsNot Nothing Then
                    Dim list As List(Of MyRadioBox) = New List(Of MyRadioBox)()
                    Dim num As Integer = 0
                    Try
                        For Each obj As Object In CType(NewLateBinding.LateGet(MyBase.Parent, Nothing, "Children", New Object(-1) {}, Nothing, Nothing, Nothing), IEnumerable)
                            Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                            If TypeOf objectValue Is MyRadioBox Then
                                list.Add(CType(objectValue, MyRadioBox))
                                If Conversions.ToBoolean(NewLateBinding.LateGet(objectValue, Nothing, "Checked", New Object(-1) {}, Nothing, Nothing, Nothing)) Then
                                    num += 1
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator
                        If TypeOf enumerator Is IDisposable Then
                            TryCast(enumerator, IDisposable).Dispose()
                        End If
                    End Try
                    If num > 1 Then
                        If Me.Checked Then
                            Try
                                For Each myRadioBox As MyRadioBox In list
                                    If myRadioBox.Checked AndAlso Not myRadioBox.Equals(Me) Then
                                        myRadioBox.Checked = False
                                    End If
                                Next
                                GoTo IL_01C0
                            Finally
                                Dim enumerator2 As List(Of MyRadioBox).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                        End If
                        Dim flag2 As Boolean = False
                        Try
                            For Each myRadioBox2 As MyRadioBox In list
                                If myRadioBox2.Checked Then
                                    If flag2 Then
                                        myRadioBox2.Checked = False
                                    Else
                                        flag2 = True
                                    End If
                                End If
                            Next
                        Finally
                            Dim enumerator3 As List(Of MyRadioBox).Enumerator
                            CType(enumerator3, IDisposable).Dispose()
                        End Try
                    End If
                    IL_01C0:
                    If flag Then
                        If Me.Checked Then
                            Dim checkEvent As IMyRadio.CheckEventHandler = Me.CheckEvent
                            If checkEvent IsNot Nothing Then
                                checkEvent(Me, New ModBase.RouteEventArgs(user))
                            End If
                        End If
                        Dim changedEvent As IMyRadio.ChangedEventHandler = Me.ChangedEvent
                        If changedEvent IsNot Nothing Then
                            changedEvent(Me, New ModBase.RouteEventArgs(user))
                        End If
                        Me.RaiseCustomEvent()
                        Me.SyncUI()
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, "单选框勾选改变错误", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyRadioBox.xaml.vb")
            End Try
        End Sub

        Private Sub SyncUI()
            If ModAnimation.AniControlEnabled = 0 AndAlso MyBase.IsLoaded Then
                If Me.Checked Then
                    If Me.ShapeDot.Opacity < 0.01 Then
                        Me.ShapeDot.Opacity = 1.0
                    End If
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeBorder, 10.0 - Me.ShapeBorder.Width, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False, True), ModAnimation.AaScale(Me.ShapeBorder, 8.0, 300, 90, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), False, True) }, "MyRadioBox Border " + Conversions.ToString(Me.Uuid), False)
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeDot, 9.0 - Me.ShapeDot.Width, 390, 0, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False, True), ModAnimation.AaOpacity(Me.ShapeDot, 1.0 - Me.ShapeDot.Opacity, 75, 90, Nothing, False) }, "MyRadioBox Dot " + Conversions.ToString(Me.Uuid), False)
                    ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeBorder, Shape.StrokeProperty, If(MyBase.IsMouseOver, "ColorBrush3", If(MyBase.IsEnabled, "ColorBrush2", "ColorBrushGray5")), 150, 0, Nothing, False) }, "MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeBorder, 18.0 - Me.ShapeBorder.Width, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False, True) }, "MyRadioBox Border " + Conversions.ToString(Me.Uuid), False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaScale(Me.ShapeDot, -Me.ShapeDot.Width, 150, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), False, True), ModAnimation.AaOpacity(Me.ShapeDot, -Me.ShapeDot.Opacity, 75, 30, Nothing, False) }, "MyRadioBox Dot " + Conversions.ToString(Me.Uuid), False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaColor(Me.ShapeBorder, Shape.StrokeProperty, If(MyBase.IsMouseOver, "ColorBrush3", If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5")), 150, 0, Nothing, False) }, "MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid), False)
                Return
            Else
                ModAnimation.AniStop("MyRadioBox Border " + Conversions.ToString(Me.Uuid))
                ModAnimation.AniStop("MyRadioBox Dot " + Conversions.ToString(Me.Uuid))
                ModAnimation.AniStop("MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid))
                If Me.Checked Then
                    Me.ShapeDot.Width = 9.0
                    Me.ShapeDot.Height = 9.0
                    Me.ShapeDot.Opacity = 1.0
                    Me.ShapeDot.Margin = New Thickness(5.5, 0.0, 0.0, 0.0)
                    Me.ShapeBorder.SetResourceReference(Shape.StrokeProperty, If(MyBase.IsEnabled, "ColorBrush2", "ColorBrushGray5"))
                    Return
                End If
                Me.ShapeDot.Width = 0.0
                Me.ShapeDot.Height = 0.0
                Me.ShapeDot.Opacity = 0.0
                Me.ShapeDot.Margin = New Thickness(10.0, 0.0, 0.0, 0.0)
                Me.ShapeBorder.SetResourceReference(Shape.StrokeProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"))
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
                Return Conversions.ToString(MyBase.GetValue(MyRadioBox.TextProperty))
            End Get
            Set(value As String)
                MyBase.SetValue(MyRadioBox.TextProperty, value)
            End Set
        End Property

        Private Sub Radiobox_MouseUp()
            If Me.MouseDowned Then
                Logger.Info(String.Format("按下单选框：{0}", Me.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyRadioBox.xaml.vb")
                Me.SetChecked(True, True)
                Me.MouseDowned = False
                ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Shape.FillProperty, "ColorBrushHalfWhite", 100, 0, Nothing, False), "MyRadioBox Background " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub Radiobox_MouseDown()
            Me.MouseDowned = True
            MyBase.Focus()
            ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Shape.FillProperty, "ColorBrushBg1", 100, 0, Nothing, False), "MyRadioBox Background " + Conversions.ToString(Me.Uuid), False)
            If Not Me.Checked Then
                ModAnimation.AniStart(ModAnimation.AaScale(Me.ShapeBorder, 16.5 - Me.ShapeBorder.Width, 1000, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False, True), "MyRadioBox Border " + Conversions.ToString(Me.Uuid), False)
            End If
        End Sub

        Private Sub Radiobox_MouseLeave()
            If Me.MouseDowned Then
                Me.MouseDowned = False
                ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Shape.FillProperty, "ColorBrushHalfWhite", 100, 0, Nothing, False), "MyRadioBox Background " + Conversions.ToString(Me.Uuid), False)
                If Not Me.Checked Then
                    ModAnimation.AniStart(ModAnimation.AaScale(Me.ShapeBorder, 18.0 - Me.ShapeBorder.Width, 400, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), False, True), "MyRadioBox Border " + Conversions.ToString(Me.Uuid), False)
                End If
            End If
        End Sub

        Private Sub Radiobox_IsEnabledChanged()
            If Not MyBase.IsLoaded OrElse ModAnimation.AniControlEnabled <> 0 Then
                ModAnimation.AniStop("MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid))
                ModAnimation.AniStop("MyRadioBox TextColor " + Conversions.ToString(Me.Uuid))
                Me.LabText.SetResourceReference(TextBlock.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"))
                Me.ShapeBorder.SetResourceReference(Shape.StrokeProperty, If(MyBase.IsEnabled, If(Me.Checked, "ColorBrush2", "ColorBrush1"), "ColorBrushGray5"))
                Return
            End If
            If MyBase.IsEnabled Then
                Me.Radiobox_MouseLeaveAnimation()
                Return
            End If
            ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Shape.StrokeProperty, ModSecret.ColorGray5 - Me.ShapeBorder.Stroke, 200, 0, Nothing, False), "MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid), False)
            ModAnimation.AniStart(ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, ModSecret.ColorGray5 - Me.LabText.Foreground, 200, 0, Nothing, False), "MyRadioBox TextColor " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub Radiobox_MouseEnterAnimation()
            ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Shape.StrokeProperty, "ColorBrush3", 100, 0, Nothing, False), "MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid), False)
            ModAnimation.AniStart(ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, Nothing, False), "MyRadioBox TextColor " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Private Sub Radiobox_MouseLeaveAnimation()
            If MyBase.IsEnabled Then
                If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                    ModAnimation.AniStart(ModAnimation.AaColor(Me.ShapeBorder, Shape.StrokeProperty, If(MyBase.IsEnabled, If(Me.Checked, "ColorBrush2", "ColorBrush1"), "ColorBrushGray5"), 200, 0, Nothing, False), "MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid), False)
                    ModAnimation.AniStart(ModAnimation.AaColor(Me.LabText, TextBlock.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"), 200, 0, Nothing, False), "MyRadioBox TextColor " + Conversions.ToString(Me.Uuid), False)
                    Return
                End If
                ModAnimation.AniStop("MyRadioBox BorderColor " + Conversions.ToString(Me.Uuid))
                ModAnimation.AniStop("MyRadioBox TextColor " + Conversions.ToString(Me.Uuid))
                Me.ShapeBorder.SetResourceReference(Shape.StrokeProperty, If(MyBase.IsEnabled, If(Me.Checked, "ColorBrush2", "ColorBrush1"), "ColorBrushGray5"))
                Me.LabText.SetResourceReference(TextBlock.ForegroundProperty, If(MyBase.IsEnabled, "ColorBrush1", "ColorBrushGray5"))
            End If
        End Sub

        Sub RefreshSetting(NewValue As String) Implements PCL.ISettingControl.RefreshSetting
            Me.Checked = Operators.CompareString(NewValue, SettingService.GetValue(Me), False) = 0
        End Sub

        Function GetCurrentSetting() As String Implements PCL.ISettingControl.GetCurrentSetting
            If Not Me.Checked Then
                Return Nothing
            End If
            Return SettingService.GetValue(Me)
        End Function

        Private Sub SaveSetting()
            SettingService.SaveSetting(Me)
        End Sub

        Friend Overridable Property PanBack As MyRadioBox

        Friend Overridable Property LabText As TextBlock

        Friend Overridable Property ShapeBorder As Ellipse

        Friend Overridable Property ShapeDot As Ellipse

        Public Uuid As Integer

        Public Shared CheckedProperty As DependencyProperty = DependencyProperty.Register("Checked", GetType(Boolean), GetType(MyRadioBox), New PropertyMetadata(False, Sub(a0 As DependencyObject, a1 As DependencyPropertyChangedEventArgs)
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_6(Of MyRadioBox, DependencyPropertyChangedEventArgs)
            If MyRadioBox._Closure$__.$I0-0 IsNot Nothing Then
                vb$AnonymousDelegate_ = MyRadioBox._Closure$__.$I0-0
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_6(Of MyRadioBox, DependencyPropertyChangedEventArgs) = Sub(d As MyRadioBox, e As DependencyPropertyChangedEventArgs)
                    If Not d.IsLoaded Then
                        d.SyncUI()
                    End If
                End Sub
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                MyRadioBox._Closure$__.$I0-0 = vb$AnonymousDelegate_2
            End If
            vb$AnonymousDelegate_(CType(a0, MyRadioBox), a1)
        End Sub))

        Public Shared TextProperty As DependencyProperty = DependencyProperty.Register("Text", GetType(String), GetType(MyRadioBox), New PropertyMetadata(Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If sender IsNot Nothing Then
                CType(sender, MyRadioBox).LabText.Text = Conversions.ToString(e.NewValue)
            End If
        End Sub))

        Private MouseDowned As Boolean

        Private AllowMouseDown As Boolean

        Private Const AnimationTimeOfMouseIn As Integer = 100

        Private Const AnimationTimeOfMouseOut As Integer = 200

        Private Const AnimationTimeOfCheck As Integer = 150

        Public Delegate Sub PreviewCheckEventHandler(sender As Object, e As ModBase.RouteEventArgs)

        Public Delegate Sub PreviewChangeEventHandler(sender As Object, e As ModBase.RouteEventArgs)
    End Class
End Namespace
