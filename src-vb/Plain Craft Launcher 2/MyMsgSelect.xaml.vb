Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Interop
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class MyMsgSelect
        Inherits Grid

        Public Sub New(Converter As ModMain.MyMsgBoxConverter)
            AddHandler MyBase.Loaded, AddressOf Me.Load
            Me.Uuid = ModBase.GetUuid()
            Me.SelectedIndex = -1
            Try
                Me.InitializeComponent()
                Me.Btn1.Name = Me.Btn1.Name + Conversions.ToString(ModBase.GetUuid())
                Me.Btn2.Name = Me.Btn2.Name + Conversions.ToString(ModBase.GetUuid())
                Me.MyConverter = Converter
                Me.LabTitle.Text = Converter.Title
                Me.Btn1.Text = Converter.Button1
                If Converter.IsWarn Then
                    Me.Btn1.ColorType = MyButton.ColorState.Red
                    Me.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrushRedLight")
                End If
                Me.Btn2.Text = Converter.Button2
                Me.Btn2.Visibility = If((Operators.CompareString(Converter.Button2, "", False) = 0), Visibility.Collapsed, Visibility.Visible)
                Me.ShapeLine.StrokeThickness = ModBase.GetWPFSize(1.0)
                Me.Btn1.IsEnabled = False
                Try
                    For Each obj As Object In CType(Converter.Content, IEnumerable)
                        Dim myRadio As IMyRadio = CType(MyVirtualizingElement.TryInit(CType(RuntimeHelpers.GetObjectValue(obj), FrameworkElement)), IMyRadio)
                        Me.PanSelection.Children.Add(CType(myRadio, UIElement))
                        AddHandler myRadio.Check, Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                            Me.OnChecked(CType(a0, IMyRadio), a1)
                        End Sub
                        If TypeOf myRadio Is MyListItem Then
                            CType(myRadio, MyListItem).Type = MyListItem.CheckType.RadioBox
                            CType(myRadio, MyListItem).MinHeight = 24.0
                        Else
                            CType(myRadio, MyRadioBox).MinHeight = 24.0
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator
                    If TypeOf enumerator Is IDisposable Then
                        TryCast(enumerator, IDisposable).Dispose()
                    End If
                End Try
            Catch ex As Exception
                Logger.[Error](ex, "选择弹窗初始化失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyMsg\MyMsgSelect.xaml.vb")
            End Try
        End Sub

        Private Sub Load(sender As Object, e As EventArgs)
            Try
                If Me.Btn2.IsVisible AndAlso Me.Btn1.ColorType <> MyButton.ColorState.Red Then
                    Me.Btn1.ColorType = MyButton.ColorState.Highlight
                End If
                MyBase.Opacity = 0.0
                ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, If(Me.MyConverter.IsWarn, New ModBase.MyColor(140.0, 80.0, 0.0, 0.0), New ModBase.MyColor(90.0, 0.0, 0.0, 0.0)) - ModMain.FrmMain.PanMsg.Background, 200, 0, Nothing, False), "PanMsg Background", False)
                ModAnimation.AniStart(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me, 1.0, 120, 60, Nothing, False), ModAnimation.AaDouble(Sub(i As Object)
                    Dim transformPos As TranslateTransform = Me.TransformPos
                    Dim translateTransform As TranslateTransform = transformPos
                    transformPos.Y = Conversions.ToDouble(Operators.AddObject(translateTransform.Y, i))
                End Sub, -Me.TransformPos.Y, 300, 60, New ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), False), ModAnimation.AaDouble(Sub(i As Object)
                    Dim transformRotate As RotateTransform = Me.TransformRotate
                    Dim rotateTransform As RotateTransform = transformRotate
                    transformRotate.Angle = Conversions.ToDouble(Operators.AddObject(rotateTransform.Angle, i))
                End Sub, -Me.TransformRotate.Angle, 300, 60, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False) }, "MyMsgBox " + Conversions.ToString(Me.Uuid), False)
                Logger.Info(String.Format("选择弹窗：{0}", Me.LabTitle.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyMsg\MyMsgSelect.xaml.vb")
            Catch ex As Exception
                Logger.[Error](ex, "选择弹窗加载失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyMsg\MyMsgSelect.xaml.vb")
            End Try
        End Sub

        Private Sub Close()
            Me.MyConverter.WaitFrame.[Continue] = False
            ComponentDispatcher.PopModal()
            Dim array As ModAnimation.AniData() = New ModAnimation.AniData(4) {}
            Dim num As Integer = 0
            Dim threadStart As ThreadStart
            If MyMsgSelect._Closure$__.$I4-0 IsNot Nothing Then
                threadStart = MyMsgSelect._Closure$__.$I4-0
            Else
                Dim threadStart2 As ThreadStart = Sub()
                    If Not ModMain.WaitingMyMsgBox.Any() Then
                        ModAnimation.AniStart(ModAnimation.AaColor(ModMain.FrmMain.PanMsg, Panel.BackgroundProperty, New ModBase.MyColor(0.0, 0.0, 0.0, 0.0) - ModMain.FrmMain.PanMsg.Background, 200, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), False), "", False)
                    End If
                End Sub
                threadStart = threadStart2
                MyMsgSelect._Closure$__.$I4-0 = threadStart2
            End If
            array(num) = ModAnimation.AaCode(threadStart, 30, False)
            array(1) = ModAnimation.AaOpacity(Me, -MyBase.Opacity, 80, 20, Nothing, False)
            array(2) = ModAnimation.AaDouble(Sub(i As Object)
                Dim transformPos As TranslateTransform = Me.TransformPos
                Dim translateTransform As TranslateTransform = transformPos
                transformPos.Y = Conversions.ToDouble(Operators.AddObject(translateTransform.Y, i))
            End Sub, 20.0 - Me.TransformPos.Y, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False)
            array(3) = ModAnimation.AaDouble(Sub(i As Object)
                Dim transformRotate As RotateTransform = Me.TransformRotate
                Dim rotateTransform As RotateTransform = transformRotate
                transformRotate.Angle = Conversions.ToDouble(Operators.AddObject(rotateTransform.Angle, i))
            End Sub, 6.0 - Me.TransformRotate.Angle, 150, 0, New ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), False)
            array(4) = ModAnimation.AaCode(Sub()
                CType(MyBase.Parent, Grid).Children.Remove(Me)
            End Sub, 0, True)
            ModAnimation.AniStart(array, "MyMsgBox " + Conversions.ToString(Me.Uuid), False)
        End Sub

        Public Sub Btn1_Click()
            If Not Me.MyConverter.IsExited AndAlso Me.SelectedIndex <> -1 Then
                Me.MyConverter.IsExited = True
                Me.MyConverter.Result = Me.SelectedIndex
                Me.Close()
            End If
        End Sub

        Public Sub Btn2_Click()
            If Not Me.MyConverter.IsExited Then
                Me.MyConverter.IsExited = True
                Me.MyConverter.Result = Nothing
                Me.Close()
            End If
        End Sub

        Private Sub OnChecked(sender As IMyRadio, e As EventArgs)
            Me.Btn1.IsEnabled = True
            Me.SelectedIndex = Me.PanSelection.Children.IndexOf(CType(sender, UIElement))
        End Sub

        Private Sub Drag(sender As Object, e As MouseButtonEventArgs)
            Dim num As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_0000:
                ProjectData.ClearProjectError()
                num = 1
                IL_0007:
                Dim num2 As Integer = 2
                If e.GetPosition(Me.ShapeLine).Y > 2.0 Then
                    GoTo IL_0034
                End If
                IL_0028:
                num2 = 3
                ModMain.FrmMain.DragMove()
                IL_0034:
                GoTo IL_0093
                IL_0036:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_0054:
                GoTo IL_0088
                IL_0056:
                num4 = num2
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                IL_0066:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_0056
            End Try
            IL_0088:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_0093:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Friend Overridable Property TransformRotate As RotateTransform

        Friend Overridable Property TransformPos As TranslateTransform

        Friend Overridable Property PanBorder As Border
            <CompilerGenerated()>
            Get
                Return Me._PanBorder
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As Border)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.Drag
                Dim border As Border = Me._PanBorder
                If border IsNot Nothing Then
                    RemoveHandler border.MouseLeftButtonDown, mouseButtonEventHandler
                End If
                Me._PanBorder = value
                border = Me._PanBorder
                If border IsNot Nothing Then
                    AddHandler border.MouseLeftButtonDown, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property EffectShadow As DropShadowEffect

        Friend Overridable Property PanMain As Grid

        Friend Overridable Property LabTitle As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabTitle
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.Drag
                Dim textBlock As TextBlock = Me._LabTitle
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.MouseLeftButtonDown, mouseButtonEventHandler
                End If
                Me._LabTitle = value
                textBlock = Me._LabTitle
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.MouseLeftButtonDown, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ShapeLine As Rectangle

        Friend Overridable Property PanCaption As MyScrollViewer

        Friend Overridable Property PanSelection As StackPanel

        Friend Overridable Property PanBtn As StackPanel

        Public Overridable Property Btn1 As MyButton
            <CompilerGenerated()>
            Get
                Return Me._Btn1
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Btn1_Click()
                End Sub
                Dim myButton As MyButton = Me._Btn1
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._Btn1 = value
                myButton = Me._Btn1
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Public Overridable Property Btn2 As MyButton
            <CompilerGenerated()>
            Get
                Return Me._Btn2
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyButton)
                Dim clickEventHandler As MyButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.Btn2_Click()
                End Sub
                Dim myButton As MyButton = Me._Btn2
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._Btn2 = value
                myButton = Me._Btn2
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private MyConverter As ModMain.MyMsgBoxConverter

        Private Uuid As Integer

        Private SelectedIndex As Integer
    End Class
End Namespace
