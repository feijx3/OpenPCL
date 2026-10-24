Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSpeedLeft
        Inherits OurPageLeft

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.Page_Loaded
            Me.IsLoad = False
            Me.RightCards = New Dictionary(Of String, OurCard)()
            Me.InitializeComponent()
        End Sub

        Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
            Me.Watcher()
            Me.TryReturnToHome()
            If Not Me.IsLoad Then
                Me.IsLoad = True
                Dim dispatcherTimer As DispatcherTimer = New DispatcherTimer()
                dispatcherTimer.Interval = New TimeSpan(0, 0, 0, 0, 300)
                AddHandler dispatcherTimer.Tick, Sub(a0 As Object, a1 As EventArgs)
                    Me.Watcher()
                End Sub
                dispatcherTimer.Start()
                If Not ModBase.ModeDebug Then
                    MyBase.RowDefinitions(12).Height = New GridLength(0.0)
                    MyBase.RowDefinitions(13).Height = New GridLength(0.0)
                    MyBase.RowDefinitions(14).Height = New GridLength(0.0)
                    MyBase.RowDefinitions(15).Height = New GridLength(0.0)
                End If
            End If
        End Sub

        Private Sub Watcher()
            If ModMain.FrmMain.PageCurrent Is FormMain.PageType.TaskManager Then
                Try
                    If Not ModLoader.LoaderTaskbar.Any() Then
                        Me.LabProgress.Text = "100 %"
                        Me.LabSpeed.Text = "0 B/s"
                        Me.LabFile.Text = "0"
                        Me.LabThread.Text = "0 / " + Conversions.ToString(ModNet.NetTaskThreadLimit)
                    Else
                        Dim loaderTaskbar As IEnumerable(Of ModLoader.LoaderBase) = ModLoader.LoaderTaskbar
                        Dim func As Func(Of ModLoader.LoaderBase, Boolean)
                        If PageSpeedLeft._Closure$__.$I5-0 IsNot Nothing Then
                            func = PageSpeedLeft._Closure$__.$I5-0
                        Else
                            Dim func2 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) l.Show
                            func = func2
                            PageSpeedLeft._Closure$__.$I5-0 = func2
                        End If
                        Dim list As List(Of ModLoader.LoaderBase) = loaderTaskbar.Where(func).ToList()
                        Dim num As Double
                        If Not list.Any() Then
                            num = 1.0
                        Else
                            Dim enumerable As IEnumerable(Of ModLoader.LoaderBase) = list
                            Dim func3 As Func(Of ModLoader.LoaderBase, Double)
                            If PageSpeedLeft._Closure$__.$I5-1 IsNot Nothing Then
                                func3 = PageSpeedLeft._Closure$__.$I5-1
                            Else
                                Dim func4 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.Progress
                                func3 = func4
                                PageSpeedLeft._Closure$__.$I5-1 = func4
                            End If
                            num = enumerable.[Select](func3).Average().Clamp(0.0, 1.0)
                        End If
                        Dim num2 As Double = num
                        Dim text As String = Conversions.ToString(Math.Floor(num2 * 100.0)) + "." + Math.Floor((num2 * 100.0 - Math.Floor(num2 * 100.0)) * 100.0).ToString().EnsureLength("0"c, 2) + " %"
                        Me.LabProgress.Text = If((num2 > 0.999999), "100 %", text)
                        Me.LabSpeed.Text = StringUtils.FormatByteSize(ModNet.NetManager.Speed) + "/s"
                        Me.LabFile.Text = Conversions.ToString(If((ModNet.NetManager.FileRemain < 0), "0*", ModNet.NetManager.FileRemain))
                        Me.LabThread.Text = Conversions.ToString(ModNet.NetTaskThreadCount) + " / " + Conversions.ToString(ModNet.NetTaskThreadLimit)
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "任务管理左栏监视出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                End Try
                If ModMain.FrmSpeedRight IsNot Nothing AndAlso ModMain.FrmSpeedRight.PanMain IsNot Nothing Then
                    Try
                        Try
                            For Each loaderBase As ModLoader.LoaderBase In ModLoader.LoaderTaskbar.ToList()
                                Me.TaskRefresh(loaderBase)
                            Next
                        Finally
                            Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        End Try
                    Catch ex2 As Exception
                        Logger.[Error](ex2, "任务管理右栏监视出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                    End Try
                End If
            End If
        End Sub

        Public Sub TaskRefresh(Loader As ModLoader.LoaderBase)
            Dim CS$<>8__locals1 As PageSpeedLeft._Closure$__6-1 = New PageSpeedLeft._Closure$__6-1(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_Loader = Loader
            If CS$<>8__locals1.$VB$Local_Loader IsNot Nothing AndAlso CS$<>8__locals1.$VB$Local_Loader.Show Then
                Try
                    Dim list As List(Of ModLoader.LoaderBase) = CType(NewLateBinding.LateGet(CS$<>8__locals1.$VB$Local_Loader, Nothing, "GetLoaderList", New Object(-1) {}, Nothing, Nothing, Nothing), List(Of ModLoader.LoaderBase))
                    If Me.RightCards.ContainsKey(CS$<>8__locals1.$VB$Local_Loader.Name) Then
                        Dim grid As Grid = Me.RightCards(CS$<>8__locals1.$VB$Local_Loader.Name)
                        Dim num As Double = CS$<>8__locals1.$VB$Local_Loader.Progress + CDbl(CS$<>8__locals1.$VB$Local_Loader.State)
                        If ModBase.Val(RuntimeHelpers.GetObjectValue(grid.Tag)) = num Then
                            Return
                        End If
                        grid.Tag = num
                        If grid.Children.Count <= 3 Then
                            Logger.Warn(String.Format("元素不足的卡片：{0}", CS$<>8__locals1.$VB$Local_Loader.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                            Return
                        End If
                        grid = CType(grid.Children(3), Grid)
                        Try
                            Select Case CS$<>8__locals1.$VB$Local_Loader.State
                                Case ModBase.LoadState.Waiting, ModBase.LoadState.Loading
                                    Try
                                        If grid.Children.Count < list.Count * 2 Then
                                            Logger.Warn(String.Format("刷新任务管理卡片 {0} 失败：卡片中仅有 {1} 个子项，要求至少有 {2} 个子项", CS$<>8__locals1.$VB$Local_Loader.Name, grid.Children.Count, list.Count * 2), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                                        Else
                                            Dim num2 As Integer = 0
                                            Try
                                                For Each loaderBase As ModLoader.LoaderBase In list
                                                    Select Case loaderBase.State
                                                        Case ModBase.LoadState.Waiting
                                                            If Operators.ConditionalCompareObjectNotEqual(CType(grid.Children(num2 * 2), FrameworkElement).Tag, "Waiting", False) Then
                                                                grid.Children.RemoveAt(num2 * 2)
                                                                grid.Children.Insert(num2 * 2, CType(ModBase.GetObjectFromXML("<Path xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:PCL;assembly=OpenPCL"" Stretch=""Uniform"" Tag=""Waiting"" Data=""F1 M5,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 Z"" Width=""18"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""" + Conversions.ToString(num2) + """ Fill=""{DynamicResource ColorBrush3}"" Margin=""0,7,0,0"" VerticalAlignment=""Top"" Height=""6""/>"), UIElement))
                                                            End If
                                                        Case ModBase.LoadState.Loading
                                                            If Operators.ConditionalCompareObjectNotEqual(CType(grid.Children(num2 * 2), FrameworkElement).Tag, "Loading", False) Then
                                                                grid.Children.RemoveAt(num2 * 2)
                                                                grid.Children.Insert(num2 * 2, CType(ModBase.GetObjectFromXML(String.Concat(New String() { "<TextBlock xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:PCL;assembly=OpenPCL"" Text=""", Conversions.ToString(Math.Floor(loaderBase.Progress * 100.0)), "%"" Tag=""Loading"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""", Conversions.ToString(num2), """ Foreground=""{DynamicResource ColorBrush3}""/>" })), UIElement))
                                                            Else
                                                                ' The following expression was wrapped in a unchecked-expression
                                                                CType(grid.Children(num2 * 2), TextBlock).Text = Conversions.ToString(Math.Floor(loaderBase.Progress * 100.0)) + "%"
                                                            End If
                                                        Case ModBase.LoadState.Finished
                                                            If Operators.ConditionalCompareObjectNotEqual(CType(grid.Children(num2 * 2), FrameworkElement).Tag, "Finished", False) Then
                                                                grid.Children.RemoveAt(num2 * 2)
                                                                grid.Children.Insert(num2 * 2, CType(ModBase.GetObjectFromXML("<Path xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:PCL;assembly=OpenPCL"" Stretch=""Uniform"" Tag=""Finished"" Data=""F1 M 23.7501,33.25L 34.8334,44.3333L 52.2499,22.1668L 56.9999,26.9168L 34.8334,53.8333L 19.0001,38L 23.7501,33.25 Z"" Height=""16"" Width=""15"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""" + Conversions.ToString(num2) + """ Fill=""{DynamicResource ColorBrush3}"" Margin=""0,3,0,0"" VerticalAlignment=""Top""/>"), UIElement))
                                                            End If
                                                    End Select
                                                    num2 += 1
                                                Next
                                            Finally
                                                Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                                                CType(enumerator, IDisposable).Dispose()
                                            End Try
                                        End If
                                    Catch ex As Exception
                                        Logger.[Error](ex, String.Format("刷新任务管理卡片 {0} 失败", CS$<>8__locals1.$VB$Local_Loader.Name), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                                    End Try
                                Case ModBase.LoadState.Finished, ModBase.LoadState.Canceled
                                    ModAnimation.AniDispose(CType(grid.Parent, OurCard), True, Sub(a0 As Object)
                                        Me.TryReturnToHome()
                                    End Sub)
                                Case ModBase.LoadState.Failed
                                    grid.RowDefinitions.Clear()
                                    grid.Children.Clear()
                                    grid.Children.Add(CType(ModBase.GetObjectFromXML("<Path xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" Stretch=""Uniform"" Tag=""Failed"" Data=""F1 M2.5,0 L0,2.5 7.5,10 0,17.5 2.5,20 10,12.5 17.5,20 20,17.5 12.5,10 20,2.5 17.5,0 10,7.5 2.5,0Z"" Height=""15"" Width=""15"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""0"" Fill=""{DynamicResource ColorBrush3}"" Margin=""0,1,0,0"" VerticalAlignment=""Top""/>"), UIElement))
                                    Dim textBlock As TextBlock = CType(ModBase.GetObjectFromXML("<TextBlock xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" TextWrapping=""Wrap"" HorizontalAlignment=""Left"" ToolTip=""单击复制错误详情"" Grid.Column=""1"" Grid.Row=""0"" Margin=""0,0,0,5"" />"), TextBlock)
                                    textBlock.Text = CS$<>8__locals1.$VB$Local_Loader.[Error].GetDisplay(True)
                                    Dim uielement As UIElement = textBlock
                                    Dim mouseButtonEventHandler As MouseButtonEventHandler
                                    If PageSpeedLeft._Closure$__.$IR6-2 IsNot Nothing Then
                                        mouseButtonEventHandler = PageSpeedLeft._Closure$__.$IR6-2
                                    Else
                                        Dim mouseButtonEventHandler2 As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                                            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_8(Of TextBlock, EventArgs)
                                            If PageSpeedLeft._Closure$__.$I6-0 IsNot Nothing Then
                                                vb$AnonymousDelegate_ = PageSpeedLeft._Closure$__.$I6-0
                                            Else
                                                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_8(Of TextBlock, EventArgs) = Sub(sender As TextBlock, e As EventArgs)
                                                    ModBase.ClipboardSet(sender.Text, False)
                                                    ModMain.Hint("已复制错误详情！", ModMain.HintType.Green, True)
                                                End Sub
                                                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                                                PageSpeedLeft._Closure$__.$I6-0 = vb$AnonymousDelegate_2
                                            End If
                                            vb$AnonymousDelegate_(CType(a0, TextBlock), a1)
                                        End Sub
                                        mouseButtonEventHandler = mouseButtonEventHandler2
                                        PageSpeedLeft._Closure$__.$IR6-2 = mouseButtonEventHandler2
                                    End If
                                    AddHandler uielement.MouseLeftButtonDown, mouseButtonEventHandler
                                    grid.Children.Add(textBlock)
                            End Select
                            GoTo IL_08A1
                        Catch ex2 As Exception
                            Logger.[Error](ex2, String.Format("更新任务管理显示失败（{0}）", CS$<>8__locals1.$VB$Local_Loader.State), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                            GoTo IL_08A1
                        End Try
                    End If
                    If CS$<>8__locals1.$VB$Local_Loader.State <> ModBase.LoadState.Canceled AndAlso CS$<>8__locals1.$VB$Local_Loader.State <> ModBase.LoadState.Finished Then
                        Try
                            Dim CS$<>8__locals2 As PageSpeedLeft._Closure$__6-0 = New PageSpeedLeft._Closure$__6-0(CS$<>8__locals2)
                            CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                            Dim text As String = String.Concat(New String() { vbCrLf & "                        <local:OurCard xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:PCL;assembly=OpenPCL""" & vbCrLf & "                            Tag=""", Conversions.ToString(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Progress + CDbl(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.State)), """ Title=""", StringUtils.XmlEscape(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Name), """ Margin=""0,0,0,15"">" & vbCrLf & "                            <Grid Margin=""14,40,15,10"">" & vbCrLf & "                                <Grid.ColumnDefinitions>" & vbCrLf & "                                    <ColumnDefinition Width=""50""/>" & vbCrLf & "                                    <ColumnDefinition/>" & vbCrLf & "                                </Grid.ColumnDefinitions>" & vbCrLf & "                                <Grid.RowDefinitions>" })
                            Try
                                For Each loaderBase2 As ModLoader.LoaderBase In list
                                    text += "<RowDefinition Height=""26""/>"
                                Next
                            Finally
                                Dim enumerator2 As List(Of ModLoader.LoaderBase).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                            text += "</Grid.RowDefinitions>"
                            Dim num3 As Integer = 0
                            Try
                                For Each loaderBase3 As ModLoader.LoaderBase In list
                                    Select Case loaderBase3.State
                                        Case ModBase.LoadState.Waiting
                                            text = text + "<Path Stretch=""Uniform"" Tag=""Waiting"" Data=""F1 M5,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 m15,0 a5,5 360 1 0 0,0.0001 Z"" Width=""18"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""" + Conversions.ToString(num3) + """ Fill=""{DynamicResource ColorBrush3}"" Margin=""0,7,0,0"" VerticalAlignment=""Top"" Height=""6""/>"
                                        Case ModBase.LoadState.Loading
                                            text = String.Concat(New String() { text, "<TextBlock Text=""", Conversions.ToString(Math.Floor(loaderBase3.Progress * 100.0)), "%"" Tag=""Loading"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""", Conversions.ToString(num3), """ Foreground=""{DynamicResource ColorBrush3}"" />" })
                                        Case ModBase.LoadState.Finished
                                            text = text + "<Path Stretch=""Uniform"" Tag=""Finished"" Data=""F1 M 23.7501,33.25L 34.8334,44.3333L 52.2499,22.1668L 56.9999,26.9168L 34.8334,53.8333L 19.0001,38L 23.7501,33.25 Z"" Height=""16"" Width=""15"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""" + Conversions.ToString(num3) + """ Fill=""{DynamicResource ColorBrush3}"" Margin=""0,3,0,0"" VerticalAlignment=""Top""/>"
                                        Case Else
                                            text = text + "<Path Stretch=""Uniform"" Tag=""Failed"" Data=""F1 M2.5,0 L0,2.5 7.5,10 0,17.5 2.5,20 10,12.5 17.5,20 20,17.5 12.5,10 20,2.5 17.5,0 10,7.5 2.5,0Z"" Height=""15"" Width=""15"" HorizontalAlignment=""Center"" Grid.Column=""0"" Grid.Row=""" + Conversions.ToString(num3) + """ Fill=""{DynamicResource ColorBrush3}"" Margin=""0,1,0,0"" VerticalAlignment=""Top""/>"
                                    End Select
                                    text = String.Concat(New String() { text, "<TextBlock Text=""", StringUtils.XmlEscape(loaderBase3.Name), """ HorizontalAlignment=""Left"" Grid.Column=""1"" Grid.Row=""", Conversions.ToString(num3), """/>" })
                                    num3 += 1
                                Next
                            Finally
                                Dim enumerator3 As List(Of ModLoader.LoaderBase).Enumerator
                                CType(enumerator3, IDisposable).Dispose()
                            End Try
                            text += "</Grid></local:OurCard>"
                            Try
                                CS$<>8__locals2.$VB$Local_Card = CType(ModBase.GetObjectFromXML(text), OurCard)
                            Catch ex3 As Exception
                                Logger.Warn(ex3, "新建任务管理卡片失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                                Logger.Info(String.Format("出错的卡片内容：{0}{1}", vbCrLf, text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                                Throw
                            End Try
                            ModMain.FrmSpeedRight.PanMain.Children.Insert(0, CS$<>8__locals2.$VB$Local_Card)
                            Me.RightCards.Add(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Name, CS$<>8__locals2.$VB$Local_Card)
                            Logger.Info(String.Format("新建任务管理卡片：{0}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                            Dim myIconButton As OurIconButton = New OurIconButton() With { .Name = "BtnCancel", .Logo = "F1 M2,0 L0,2 8,10 0,18 2,20 10,12 18,20 20,18 12,10 20,2 18,0 10,8 2,0Z", .Height = 20.0, .Margin = New Thickness(0.0, 10.0, 10.0, 0.0), .LogoScale = 1.1, .HorizontalAlignment = HorizontalAlignment.Right, .VerticalAlignment = VerticalAlignment.Top }
                            CS$<>8__locals2.$VB$Local_Card.Children.Add(myIconButton)
                            AddHandler myIconButton.Click, Sub(a0 As Object, a1 As EventArgs)
                                MyBase._Lambda$__1(CType(a0, OurIconButton), a1)
                            End Sub
                            If CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader.State = ModBase.LoadState.Failed Then
                                CS$<>8__locals2.$VB$Local_Card.Tag = Nothing
                                Me.TaskRefresh(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Loader)
                            End If
                        Catch ex4 As Exception
                            Logger.[Error](ex4, "添加任务管理卡片失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                        End Try
                    End If
                    IL_08A1:
                Catch ex5 As Exception
                    Logger.[Error](ex5, "刷新任务管理显示失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                End Try
            End If
        End Sub

        Public Sub TaskRemove(Loader As Object)
            If Me.RightCards.ContainsKey(Conversions.ToString(NewLateBinding.LateGet(Loader, Nothing, "Name", New Object(-1) {}, Nothing, Nothing, Nothing))) Then
                ModBase.RunInUiWait(Sub()
                    Dim grid As Grid = Me.RightCards(Conversions.ToString(NewLateBinding.LateGet(Loader, Nothing, "Name", New Object(-1) {}, Nothing, Nothing, Nothing)))
                    ModMain.FrmSpeedRight.PanMain.Children.Remove(grid)
                    Me.RightCards.Remove(Conversions.ToString(NewLateBinding.LateGet(Loader, Nothing, "Name", New Object(-1) {}, Nothing, Nothing, Nothing)))
                    Logger.Info(String.Format("移除任务管理卡片：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Loader, Nothing, "Name", New Object(-1) {}, Nothing, Nothing, Nothing))), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageSpeedLeft.xaml.vb")
                End Sub)
            End If
        End Sub

        Private Sub TryReturnToHome()
            If ModMain.FrmSpeedRight.PanMain.Children.Count = 0 AndAlso ModMain.FrmMain.PageCurrent Is FormMain.PageType.TaskManager Then
                ModMain.FrmMain.PageBack()
            End If
        End Sub

        Friend Overridable Property LabProgress As TextBlock

        Friend Overridable Property LabSpeed As TextBlock

        Friend Overridable Property LabFile As TextBlock

        Friend Overridable Property LabThread As TextBlock

        Private Const WatcherInterval As Integer = 300

        Private IsLoad As Boolean

        Private RightCards As Dictionary(Of String, OurCard)
    End Class
End Namespace
