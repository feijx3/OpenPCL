Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports MeloongCore
Imports Microsoft.VisualBasic.CompilerServices
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class MyResourceItem
        Inherits Grid

        Public Sub New()
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.Button_MouseUp
            AddHandler Me.Click, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                Me.MyResourceItem_Click(CType(a0, MyResourceItem), a1)
            End Sub
            AddHandler MyBase.PreviewMouseLeftButtonDown, AddressOf Me.Button_MouseDown
            AddHandler MyBase.MouseLeave, AddressOf Me.Button_MouseLeave
            AddHandler MyBase.PreviewMouseLeftButtonUp, AddressOf Me.Button_MouseLeave
            AddHandler MyBase.MouseEnter, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeave, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeftButtonDown, AddressOf Me.RefreshColor
            AddHandler MyBase.MouseLeftButtonUp, AddressOf Me.RefreshColor
            Me.Uuid = ModBase.GetUuid()
            Me.IsMouseDown = False
            Me._RectBack = Nothing
            Me.CanInteraction = True
            Me.InitializeComponent()
        End Sub

        Public Property Title As String
            Get
                Return Me.LabTitle.Text
            End Get
            Set(value As String)
                If Operators.CompareString(Me.LabTitle.Text, value, False) <> 0 Then
                    Me.LabTitle.Text = value
                End If
            End Set
        End Property

        Public Property SubTitle As String
            Get
                Dim labTitleRaw As TextBlock = Me.LabTitleRaw
                Return If(If((labTitleRaw IsNot Nothing), labTitleRaw.Text, Nothing), "")
            End Get
            Set(value As String)
                If Operators.CompareString(Me.LabTitleRaw.Text, value, False) <> 0 Then
                    Me.LabTitleRaw.Text = value
                    Me.LabTitleRaw.Visibility = If((Operators.CompareString(value, "", False) = 0), Visibility.Collapsed, Visibility.Visible)
                End If
            End Set
        End Property

        Public Property Description As String
            Get
                Return Me.LabInfo.Text
            End Get
            Set(value As String)
                If Operators.CompareString(Me.LabInfo.Text, value, False) <> 0 Then
                    Me.LabInfo.Text = value
                End If
            End Set
        End Property

        Private Sub LabInfo_MouseEnter(sender As Object, e As MouseEventArgs)
            Me.ToolTipInfo.Content = Me.LabInfo.Text
            Me.LabInfo.ToolTip = If(Me.LabInfo.IsTextTrimmed(), Me.ToolTipInfo, Nothing)
        End Sub

        Public WriteOnly Property Tags As List(Of String)
            Set(value As List(Of String))
                Me.PanTags.Children.Clear()
                Me.PanTags.Visibility = If(value.Any(), Visibility.Visible, Visibility.Collapsed)
                Try
                    For Each text As String In value
                        Dim border As Border = New Border() With { .Background = New ModBase.MyColor("#11000000"), .Padding = New Thickness(3.0, 1.0, 3.0, 1.0), .CornerRadius = New CornerRadius(3.0), .Margin = New Thickness(0.0, 0.0, 3.0, 0.0), .SnapsToDevicePixels = True, .UseLayoutRounding = False }
                        Dim textBlock As TextBlock = New TextBlock() With { .Text = text, .Foreground = New ModBase.MyColor("#868686"), .FontSize = 11.0 }
                        border.Child = textBlock
                        Me.PanTags.Children.Add(border)
                    Next
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
            End Set
        End Property

        Public Event Click As MyResourceItem.ClickEventHandler

        Private Sub Button_MouseUp(sender As Object, e As MouseButtonEventArgs)
            If Me.IsMouseDown Then
                Dim clickEvent As MyResourceItem.ClickEventHandler = Me.ClickEvent
                If clickEvent IsNot Nothing Then
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e)
                End If
                If Not e.Handled Then
                    Logger.Info(String.Format("按下资源工程列表项：{0}", Me.LabTitle.Text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\Resource\MyResourceItem.xaml.vb")
                End If
            End If
        End Sub

        Private Sub MyResourceItem_Click(sender As MyResourceItem, e As EventArgs)
            Dim list As List(Of String) = New List(Of String)()
            If ModMain.FrmMain.PageCurrent.Page = FormMain.PageType.ResourceDetail Then
                Try
                    For Each obj As Object In ModMain.FrmDownloadResourceDetail.PanResults.Children
                        Dim myCard As MyCard = CType(obj, MyCard)
                        If Operators.CompareString(myCard.Title, "", False) <> 0 AndAlso Not myCard.IsSwapped Then
                            list.Add(myCard.Title)
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator
                    If TypeOf enumerator Is IDisposable Then
                        TryCast(enumerator, IDisposable).Dispose()
                    End If
                End Try
                Logger.Info(String.Format("记录当前已展开的卡片：{0}", String.Join("、", list)), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\Resource\MyResourceItem.xaml.vb")
            End If
            Dim text As String = Nothing
            Dim modLoaders As ModLoaders = ModLoaders.None
            Dim resourceTypes As ResourceTypes
            If ModMain.FrmMain.PageCurrent.Page = FormMain.PageType.Download Then
                Select Case ModMain.FrmMain.PageCurrentSub
                    Case FormMain.PageSubType.DownloadMod
                        resourceTypes = ResourceTypes.[Mod]
                        text = ModMain.FrmDownloadMod.Content.Loader.Input.GameVersion
                        modLoaders = ModMain.FrmDownloadMod.Content.Loader.Input.ModLoaders
                    Case FormMain.PageSubType.DownloadPack
                        resourceTypes = ResourceTypes.ModPack
                        text = ModMain.FrmDownloadPack.Content.Loader.Input.GameVersion
                    Case FormMain.PageSubType.DownloadDataPack
                        resourceTypes = ResourceTypes.DataPack
                        text = ModMain.FrmDownloadDataPack.Content.Loader.Input.GameVersion
                    Case FormMain.PageSubType.DownloadResourcePack
                        resourceTypes = ResourceTypes.ResourcePack
                        text = ModMain.FrmDownloadResourcePack.Content.Loader.Input.GameVersion
                    Case FormMain.PageSubType.DownloadShader
                        resourceTypes = ResourceTypes.Shader
                        text = ModMain.FrmDownloadShader.Content.Loader.Input.GameVersion
                End Select
            Else
                resourceTypes = ResourceTypes.Any
                text = Conversions.ToString(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 2 }, Nothing))
                modLoaders = CType(Conversions.ToInteger(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, New Object() { 3 }, Nothing)), ModLoaders)
            End If
            ModMain.FrmMain.PageChange(New FormMain.PageStackData() With { .Page = FormMain.PageType.ResourceDetail, .Additional = New Object() { sender.Tag, list, text, modLoaders, resourceTypes } }, FormMain.PageSubType.[Default])
        End Sub

        Private Sub Button_MouseDown(sender As Object, e As MouseButtonEventArgs)
            If MyBase.IsMouseOver AndAlso Me.CanInteraction Then
                Me.IsMouseDown = True
            End If
        End Sub

        Private Sub Button_MouseLeave(sender As Object, e As Object)
            Me.IsMouseDown = False
        End Sub

        Public ReadOnly Property RectBack As Border
            Get
                If Me._RectBack Is Nothing Then
                    Dim border As Border = New Border() With { .Name = "RectBack", .CornerRadius = New CornerRadius(3.0), .RenderTransform = New ScaleTransform(0.8, 0.8), .RenderTransformOrigin = New Point(0.5, 0.5), .BorderThickness = New Thickness(ModBase.GetWPFSize(1.0)), .SnapsToDevicePixels = True, .IsHitTestVisible = False, .Opacity = 0.0 }
                    border.SetResourceReference(Border.BackgroundProperty, "ColorBrush7")
                    border.SetResourceReference(Border.BorderBrushProperty, "ColorBrush6")
                    Grid.SetColumnSpan(border, 999)
                    Grid.SetRowSpan(border, 999)
                    MyBase.Children.Insert(0, border)
                    Me._RectBack = border
                End If
                Return Me._RectBack
            End Get
        End Property

        Public Property CanInteraction As Boolean

        Public Sub RefreshColor(sender As Object, e As EventArgs)
            If Me.CanInteraction Then
                Dim text As String
                Dim num As Integer
                If MyBase.IsMouseOver Then
                    If Me.IsMouseDown Then
                        text = "MouseDown"
                        num = 120
                    Else
                        text = "MouseOver"
                        num = 120
                    End If
                Else
                    text = "Idle"
                    num = 180
                End If
                If Operators.CompareString(Me.StateLast, text, False) <> 0 Then
                    Me.StateLast = text
                    If MyBase.IsLoaded AndAlso ModAnimation.AniControlEnabled = 0 Then
                        Dim list As List(Of ModAnimation.AniData) = New List(Of ModAnimation.AniData)()
                        If MyBase.IsMouseOver Then
                            list.AddRange(New ModAnimation.AniData() { ModAnimation.AaColor(Me.RectBack, Border.BackgroundProperty, If(Me.IsMouseDown, "ColorBrush6", "ColorBrushBg1"), num, 0, Nothing, False), ModAnimation.AaOpacity(Me.RectBack, 1.0 - Me.RectBack.Opacity, num, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False) })
                            If Me.IsMouseDown Then
                                ' The following expression was wrapped in a unchecked-expression
                                ' The following expression was wrapped in a unchecked-expression
                                list.Add(ModAnimation.AaScaleTransform(Me.RectBack, 0.996 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, CInt(Math.Round(CDbl(num) * 1.2)), 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                            Else
                                ' The following expression was wrapped in a unchecked-expression
                                ' The following expression was wrapped in a unchecked-expression
                                list.Add(ModAnimation.AaScaleTransform(Me.RectBack, 1.0 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, CInt(Math.Round(CDbl(num) * 1.2)), 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False))
                            End If
                        Else
                            list.AddRange(New ModAnimation.AniData() { ModAnimation.AaOpacity(Me.RectBack, -Me.RectBack.Opacity, num, 0, Nothing, False), ModAnimation.AaColor(Me.RectBack, Border.BackgroundProperty, If(Me.IsMouseDown, "ColorBrush6", "ColorBrush7"), num, 0, Nothing, False), ModAnimation.AaScaleTransform(Me.RectBack, 0.996 - CType(Me.RectBack.RenderTransform, ScaleTransform).ScaleX, num, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), ModAnimation.AaScaleTransform(Me.RectBack, -0.196, 1, 0, Nothing, True) })
                        End If
                        ModAnimation.AniStart(list, "MyResourceItem Color " + Conversions.ToString(Me.Uuid), False)
                        Return
                    End If
                    ModAnimation.AniStop("MyResourceItem Color " + Conversions.ToString(Me.Uuid))
                    If Me._RectBack IsNot Nothing Then
                        Me.RectBack.Opacity = 0.0
                    End If
                End If
            End If
        End Sub

        Friend Overridable Property PanBack As MyResourceItem

        Friend Overridable Property PathLogo As MyImage

        Friend Overridable Property LabTitle As TextBlock

        Friend Overridable Property LabTitleRaw As TextBlock

        Friend Overridable Property PanTags As StackPanel

        Friend Overridable Property LabInfo As TextBlock
            <CompilerGenerated()>
            Get
                Return Me._LabInfo
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As TextBlock)
                Dim mouseEventHandler As MouseEventHandler = AddressOf Me.LabInfo_MouseEnter
                Dim textBlock As TextBlock = Me._LabInfo
                If textBlock IsNot Nothing Then
                    RemoveHandler textBlock.MouseEnter, mouseEventHandler
                End If
                Me._LabInfo = value
                textBlock = Me._LabInfo
                If textBlock IsNot Nothing Then
                    AddHandler textBlock.MouseEnter, mouseEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property ToolTipInfo As ToolTip

        Friend Overridable Property ColumnVersion1 As ColumnDefinition

        Friend Overridable Property ColumnVersion2 As ColumnDefinition

        Friend Overridable Property ColumnVersion3 As ColumnDefinition

        Friend Overridable Property ColumnTime1 As ColumnDefinition

        Friend Overridable Property ColumnTime2 As ColumnDefinition

        Friend Overridable Property ColumnTime3 As ColumnDefinition

        Friend Overridable Property PathVersion As Path

        Friend Overridable Property LabVersion As TextBlock

        Friend Overridable Property PathDownload As Path

        Friend Overridable Property LabDownload As TextBlock

        Friend Overridable Property PathTime As Path

        Friend Overridable Property LabTime As TextBlock

        Friend Overridable Property PathSource As Path

        Friend Overridable Property LabSource As TextBlock

        Public Uuid As Integer

        Private IsMouseDown As Boolean

        Private _RectBack As Border

        Private StateLast As String

        Public Delegate Sub ClickEventHandler(sender As Object, e As MouseButtonEventArgs)
    End Class
End Namespace
