Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Xml.Linq
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageSelectLeft
        Inherits MyPageLeft
        Implements IRefreshable

        Public Sub New()
            AddHandler MyBase.Initialized, AddressOf Me.PageSelectLeft_Initialized
            AddHandler MyBase.Loaded, AddressOf Me.PageSelectLeft_Loaded
            Me.IsFirstLoad = True
            Me.InitializeComponent()
        End Sub

        Private Sub PageSelectLeft_Initialized(sender As Object, e As EventArgs)
            AddHandler ModMinecraft.McFolderListLoader.PreviewFinish, Sub(a0 As ModLoader.LoaderBase)
                Me._Lambda$__1-0()
            End Sub
        End Sub

        Private Sub PageSelectLeft_Loaded(sender As Object, e As RoutedEventArgs)
            If Me.IsFirstLoad Then
                Me.McFolderListUI()
            End If
            Me.IsFirstLoad = False
        End Sub

        Private Sub McFolderListUI()
            ' The following expression was wrapped in a checked-statement
            Try
                If Me.McFolderListLast IsNot Nothing AndAlso Me.McFolderListLast.SequenceEqual(ModMinecraft.McFolderList) Then
                    Dim flag As Boolean = True
                    Dim num As Integer = Me.McFolderListLast.Count - 1
                    For i As Integer = 0 To num
                        If Not Me.McFolderListLast(i).Equals(ModMinecraft.McFolderList(i)) Then
                            flag = False
                            Exit For
                        End If
                    Next
                    If flag Then
                        Return
                    End If
                End If
                Me.McFolderListLast = ModMinecraft.McFolderList
                ModMain.FrmSelectLeft.PanList.Children.Clear()
                ModMain.FrmSelectLeft.PanList.Children.Add(New TextBlock() With { .Text = "文件夹列表", .Margin = New Thickness(13.0, 18.0, 5.0, 4.0), .Opacity = 0.6, .FontSize = 12.0 })
                For Each mcFolder As ModMinecraft.McFolder In ModMinecraft.McFolderList.ToArray()
                    Dim CS$<>8__locals1 As PageSelectLeft._Closure$__4-0 = New PageSelectLeft._Closure$__4-0(CS$<>8__locals1)
                    CS$<>8__locals1.$VB$Local_ContMenu = Nothing
                    Select Case mcFolder.Type
                        Case ModMinecraft.McFolder.Types.Vanilla
                            Dim CS$<>8__locals2 As PageSelectLeft._Closure$__4-0 = CS$<>8__locals1
                            Dim xelement As XElement = New XElement(XName.[Get]("ContextMenu", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
                            xelement.Add(New XAttribute(XName.[Get]("xmlns", ""), "http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
                            xelement.Add(New XAttribute(XName.[Get]("x", "http://www.w3.org/2000/xmlns/"), "http://schemas.microsoft.com/winfx/2006/xaml"))
                            xelement.Add(New XAttribute(XName.[Get]("local", "http://www.w3.org/2000/xmlns/"), "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            Dim xelement2 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement2.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Rename"))
                            xelement2.Add(New XAttribute(XName.[Get]("Header", ""), "重命名"))
                            xelement2.Add(New XAttribute(XName.[Get]("Padding", ""), "0,2,0,0"))
                            xelement2.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 53.2929,21.2929L 54.7071,22.7071C 56.4645,24.4645 56.4645,27.3137 54.7071,29.0711L 52.2323,31.5459L 44.4541,23.7677L 46.9289,21.2929C 48.6863,19.5355 51.5355,19.5355 53.2929,21.2929 Z M 31.7262,52.052L 23.948,44.2738L 43.0399,25.182L 50.818,32.9601L 31.7262,52.052 Z M 23.2409,47.1023L 28.8977,52.7591L 21.0463,54.9537L 23.2409,47.1023 Z"))
                            xelement.Add(xelement2)
                            Dim xelement3 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement3.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Open"))
                            xelement3.Add(New XAttribute(XName.[Get]("Header", ""), "打开"))
                            xelement3.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 19,50L 28,34L 63,34L 54,50L 19,50 Z M 19,28.0001L 35,28C 36,25 37.4999,24.0001 37.4999,24.0001L 48.75,24C 49.3023,24 50,24.6977 50,25.25L 50,28L 54,28.0001L 54,32L 27,32L 19,46.4L 19,28.0001 Z"))
                            xelement.Add(xelement3)
                            Dim xelement4 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement4.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Refresh"))
                            xelement4.Add(New XAttribute(XName.[Get]("Header", ""), "刷新"))
                            xelement4.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 38,20.5833C 42.9908,20.5833 47.4912,22.6825 50.6667,26.046L 50.6667,17.4167L 55.4166,22.1667L 55.4167,34.8333L 42.75,34.8333L 38,30.0833L 46.8512,30.0833C 44.6768,27.6539 41.517,26.125 38,26.125C 31.9785,26.125 27.0037,30.6068 26.2296,36.4167L 20.6543,36.4167C 21.4543,27.5397 28.9148,20.5833 38,20.5833 Z M 38,49.875C 44.0215,49.875 48.9963,45.3932 49.7703,39.5833L 55.3457,39.5833C 54.5457,48.4603 47.0852,55.4167 38,55.4167C 33.0092,55.4167 28.5088,53.3175 25.3333,49.954L 25.3333,58.5833L 20.5833,53.8333L 20.5833,41.1667L 33.25,41.1667L 38,45.9167L 29.1487,45.9167C 31.3231,48.3461 34.483,49.875 38,49.875 Z"))
                            xelement.Add(xelement4)
                            Dim xelement5 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement5.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Delete"))
                            xelement5.Add(New XAttribute(XName.[Get]("Header", ""), "删除"))
                            xelement5.Add(New XAttribute(XName.[Get]("Padding", ""), "0,0,0,2"))
                            xelement5.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 26.9166,22.1667L 37.9999,33.25L 49.0832,22.1668L 53.8332,26.9168L 42.7499,38L 53.8332,49.0834L 49.0833,53.8334L 37.9999,42.75L 26.9166,53.8334L 22.1666,49.0833L 33.25,38L 22.1667,26.9167L 26.9166,22.1667 Z "))
                            xelement.Add(xelement5)
                            CS$<>8__locals2.$VB$Local_ContMenu = CType(ModBase.GetObjectFromXML(xelement), ContextMenu)
                        Case ModMinecraft.McFolder.Types.RenamedVanilla
                            Dim CS$<>8__locals3 As PageSelectLeft._Closure$__4-0 = CS$<>8__locals1
                            Dim xelement6 As XElement = New XElement(XName.[Get]("ContextMenu", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
                            xelement6.Add(New XAttribute(XName.[Get]("xmlns", ""), "http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
                            xelement6.Add(New XAttribute(XName.[Get]("x", "http://www.w3.org/2000/xmlns/"), "http://schemas.microsoft.com/winfx/2006/xaml"))
                            xelement6.Add(New XAttribute(XName.[Get]("local", "http://www.w3.org/2000/xmlns/"), "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            Dim xelement7 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement7.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Remove"))
                            xelement7.Add(New XAttribute(XName.[Get]("Header", ""), "复原名称"))
                            xelement7.Add(New XAttribute(XName.[Get]("Padding", ""), "0,2,0,0"))
                            xelement7.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 53.2929,21.2929L 54.7071,22.7071C 56.4645,24.4645 56.4645,27.3137 54.7071,29.0711L 52.2323,31.5459L 44.4541,23.7677L 46.9289,21.2929C 48.6863,19.5355 51.5355,19.5355 53.2929,21.2929 Z M 31.7262,52.052L 23.948,44.2738L 43.0399,25.182L 50.818,32.9601L 31.7262,52.052 Z M 23.2409,47.1023L 28.8977,52.7591L 21.0463,54.9537L 23.2409,47.1023 Z"))
                            xelement6.Add(xelement7)
                            Dim xelement8 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement8.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Rename"))
                            xelement8.Add(New XAttribute(XName.[Get]("Header", ""), "重命名"))
                            xelement8.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 53.2929,21.2929L 54.7071,22.7071C 56.4645,24.4645 56.4645,27.3137 54.7071,29.0711L 52.2323,31.5459L 44.4541,23.7677L 46.9289,21.2929C 48.6863,19.5355 51.5355,19.5355 53.2929,21.2929 Z M 31.7262,52.052L 23.948,44.2738L 43.0399,25.182L 50.818,32.9601L 31.7262,52.052 Z M 23.2409,47.1023L 28.8977,52.7591L 21.0463,54.9537L 23.2409,47.1023 Z"))
                            xelement6.Add(xelement8)
                            Dim xelement9 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement9.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Open"))
                            xelement9.Add(New XAttribute(XName.[Get]("Header", ""), "打开"))
                            xelement9.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 19,50L 28,34L 63,34L 54,50L 19,50 Z M 19,28.0001L 35,28C 36,25 37.4999,24.0001 37.4999,24.0001L 48.75,24C 49.3023,24 50,24.6977 50,25.25L 50,28L 54,28.0001L 54,32L 27,32L 19,46.4L 19,28.0001 Z"))
                            xelement6.Add(xelement9)
                            Dim xelement10 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement10.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Refresh"))
                            xelement10.Add(New XAttribute(XName.[Get]("Header", ""), "刷新"))
                            xelement10.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 38,20.5833C 42.9908,20.5833 47.4912,22.6825 50.6667,26.046L 50.6667,17.4167L 55.4166,22.1667L 55.4167,34.8333L 42.75,34.8333L 38,30.0833L 46.8512,30.0833C 44.6768,27.6539 41.517,26.125 38,26.125C 31.9785,26.125 27.0037,30.6068 26.2296,36.4167L 20.6543,36.4167C 21.4543,27.5397 28.9148,20.5833 38,20.5833 Z M 38,49.875C 44.0215,49.875 48.9963,45.3932 49.7703,39.5833L 55.3457,39.5833C 54.5457,48.4603 47.0852,55.4167 38,55.4167C 33.0092,55.4167 28.5088,53.3175 25.3333,49.954L 25.3333,58.5833L 20.5833,53.8333L 20.5833,41.1667L 33.25,41.1667L 38,45.9167L 29.1487,45.9167C 31.3231,48.3461 34.483,49.875 38,49.875 Z"))
                            xelement6.Add(xelement10)
                            Dim xelement11 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement11.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Delete"))
                            xelement11.Add(New XAttribute(XName.[Get]("Header", ""), "删除"))
                            xelement11.Add(New XAttribute(XName.[Get]("Padding", ""), "0,0,0,2"))
                            xelement11.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 26.9166,22.1667L 37.9999,33.25L 49.0832,22.1668L 53.8332,26.9168L 42.7499,38L 53.8332,49.0834L 49.0833,53.8334L 37.9999,42.75L 26.9166,53.8334L 22.1666,49.0833L 33.25,38L 22.1667,26.9167L 26.9166,22.1667 Z "))
                            xelement6.Add(xelement11)
                            CS$<>8__locals3.$VB$Local_ContMenu = CType(ModBase.GetObjectFromXML(xelement6), ContextMenu)
                        Case ModMinecraft.McFolder.Types.Custom
                            Dim CS$<>8__locals4 As PageSelectLeft._Closure$__4-0 = CS$<>8__locals1
                            Dim xelement12 As XElement = New XElement(XName.[Get]("ContextMenu", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
                            xelement12.Add(New XAttribute(XName.[Get]("xmlns", ""), "http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
                            xelement12.Add(New XAttribute(XName.[Get]("x", "http://www.w3.org/2000/xmlns/"), "http://schemas.microsoft.com/winfx/2006/xaml"))
                            xelement12.Add(New XAttribute(XName.[Get]("local", "http://www.w3.org/2000/xmlns/"), "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            Dim xelement13 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement13.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Rename"))
                            xelement13.Add(New XAttribute(XName.[Get]("Header", ""), "重命名"))
                            xelement13.Add(New XAttribute(XName.[Get]("Padding", ""), "0,2,0,0"))
                            xelement13.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 53.2929,21.2929L 54.7071,22.7071C 56.4645,24.4645 56.4645,27.3137 54.7071,29.0711L 52.2323,31.5459L 44.4541,23.7677L 46.9289,21.2929C 48.6863,19.5355 51.5355,19.5355 53.2929,21.2929 Z M 31.7262,52.052L 23.948,44.2738L 43.0399,25.182L 50.818,32.9601L 31.7262,52.052 Z M 23.2409,47.1023L 28.8977,52.7591L 21.0463,54.9537L 23.2409,47.1023 Z"))
                            xelement12.Add(xelement13)
                            Dim xelement14 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement14.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Open"))
                            xelement14.Add(New XAttribute(XName.[Get]("Header", ""), "打开"))
                            xelement14.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 19,50L 28,34L 63,34L 54,50L 19,50 Z M 19,28.0001L 35,28C 36,25 37.4999,24.0001 37.4999,24.0001L 48.75,24C 49.3023,24 50,24.6977 50,25.25L 50,28L 54,28.0001L 54,32L 27,32L 19,46.4L 19,28.0001 Z"))
                            xelement12.Add(xelement14)
                            Dim xelement15 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement15.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Refresh"))
                            xelement15.Add(New XAttribute(XName.[Get]("Header", ""), "刷新"))
                            xelement15.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 38,20.5833C 42.9908,20.5833 47.4912,22.6825 50.6667,26.046L 50.6667,17.4167L 55.4166,22.1667L 55.4167,34.8333L 42.75,34.8333L 38,30.0833L 46.8512,30.0833C 44.6768,27.6539 41.517,26.125 38,26.125C 31.9785,26.125 27.0037,30.6068 26.2296,36.4167L 20.6543,36.4167C 21.4543,27.5397 28.9148,20.5833 38,20.5833 Z M 38,49.875C 44.0215,49.875 48.9963,45.3932 49.7703,39.5833L 55.3457,39.5833C 54.5457,48.4603 47.0852,55.4167 38,55.4167C 33.0092,55.4167 28.5088,53.3175 25.3333,49.954L 25.3333,58.5833L 20.5833,53.8333L 20.5833,41.1667L 33.25,41.1667L 38,45.9167L 29.1487,45.9167C 31.3231,48.3461 34.483,49.875 38,49.875 Z"))
                            xelement12.Add(xelement15)
                            Dim xelement16 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement16.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Remove"))
                            xelement16.Add(New XAttribute(XName.[Get]("Header", ""), "移出列表"))
                            xelement16.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 23.3428,25.205L 23.3805,25.4461C 23.9229,27.177 30.261,29.0992 38,29.0992C 45.7386,29.0992 52.0765,27.1771 52.6194,25.4463L 52.6571,25.205C 52.6571,23.3616 46.0949,21.3109 38,21.3109C 29.9051,21.3109 23.3428,23.3616 23.3428,25.205 Z M 23.3428,53.0204L 19.1571,26.2111C 19.0534,25.8817 19,25.5459 19,25.205C 19,20.9036 27.5066,17.4167 38,17.4167C 48.4934,17.4167 57,20.9036 57,25.205C 57,25.5459 56.9466,25.8818 56.8429,26.2112L 52.6571,53.0204L 52.5974,53.0204C 51.9241,56.1393 45.6457,58.5833 38,58.5833C 30.3543,58.5833 24.076,56.1393 23.4026,53.0204L 23.3428,53.0204 Z M 51.8228,30.5485C 48.3585,32.0537 43.4469,32.9933 38,32.9933C 32.5531,32.9933 27.6415,32.0537 24.1771,30.5484L 27.5988,52.464L 27.6857,52.464C 27.6857,53.3857 32.3036,54.6892 38,54.6892C 43.6964,54.6892 48.3143,53.3857 48.3143,52.464L 48.4011,52.464L 51.8228,30.5485 Z "))
                            xelement12.Add(xelement16)
                            Dim xelement17 As XElement = New XElement(XName.[Get]("MyMenuItem", "clr-namespace:PCL;assembly=Plain Craft Launcher 2"))
                            xelement17.Add(New XAttribute(XName.[Get]("Name", "http://schemas.microsoft.com/winfx/2006/xaml"), "Delete"))
                            xelement17.Add(New XAttribute(XName.[Get]("Header", ""), "删除"))
                            xelement17.Add(New XAttribute(XName.[Get]("Padding", ""), "0,0,0,2"))
                            xelement17.Add(New XAttribute(XName.[Get]("Icon", ""), "F1 M 26.9166,22.1667L 37.9999,33.25L 49.0832,22.1668L 53.8332,26.9168L 42.7499,38L 53.8332,49.0834L 49.0833,53.8334L 37.9999,42.75L 26.9166,53.8334L 22.1666,49.0833L 33.25,38L 22.1667,26.9167L 26.9166,22.1667 Z "))
                            xelement12.Add(xelement17)
                            CS$<>8__locals4.$VB$Local_ContMenu = CType(ModBase.GetObjectFromXML(xelement12), ContextMenu)
                    End Select
                    If(mcFolder.Type = ModMinecraft.McFolder.Types.Vanilla OrElse mcFolder.Type = ModMinecraft.McFolder.Types.RenamedVanilla) AndAlso PathUtils.IsSame(mcFolder.Location, Paths.Base + ".minecraft\") AndAlso ModMinecraft.McFolderList.IsSingle() Then
                        CType(CS$<>8__locals1.$VB$Local_ContMenu.FindName("Delete"), MyMenuItem).Header = "清空"
                    End If
                    If mcFolder.Type <> ModMinecraft.McFolder.Types.Vanilla Then
                        CType(CS$<>8__locals1.$VB$Local_ContMenu.FindName("Remove"), MyMenuItem).[AddHandler](MenuItem.ClickEvent, AddressOf ModMain.FrmSelectLeft.Remove_Click)
                    End If
                    CType(CS$<>8__locals1.$VB$Local_ContMenu.FindName("Open"), MyMenuItem).[AddHandler](MenuItem.ClickEvent, AddressOf ModMain.FrmSelectLeft.Open_Click)
                    CType(CS$<>8__locals1.$VB$Local_ContMenu.FindName("Delete"), MyMenuItem).[AddHandler](MenuItem.ClickEvent, AddressOf ModMain.FrmSelectLeft.Delete_Click)
                    CType(CS$<>8__locals1.$VB$Local_ContMenu.FindName("Rename"), MyMenuItem).[AddHandler](MenuItem.ClickEvent, AddressOf ModMain.FrmSelectLeft.Rename_Click)
                    CType(CS$<>8__locals1.$VB$Local_ContMenu.FindName("Refresh"), MyMenuItem).[AddHandler](MenuItem.ClickEvent, AddressOf ModMain.FrmSelectLeft.Refresh_Click)
                    CS$<>8__locals1.$VB$Local_NewItem = New MyListItem() With { .IsScaleAnimationEnabled = False, .Type = MyListItem.CheckType.RadioBox, .MinPaddingRight = 30, .Title = mcFolder.Name, .Info = mcFolder.Location, .Height = 40.0, .ContextMenu = CS$<>8__locals1.$VB$Local_ContMenu, .Tag = mcFolder }
                    Dim $VB$Local_NewItem As MyListItem = CS$<>8__locals1.$VB$Local_NewItem
                    Dim CS$<>8__locals5 As PageSelectLeft._Closure$__R4-1 = New PageSelectLeft._Closure$__R4-1(CS$<>8__locals5)
                    CS$<>8__locals5.$VB$NonLocal_2 = ModMain.FrmSelectLeft
                    AddHandler $VB$Local_NewItem.Changed, Sub(a0 As Object, a1 As ModBase.RouteEventArgs)
                        CS$<>8__locals5.$VB$NonLocal_2.Folder_Change(CType(a0, MyListItem), a1)
                    End Sub
                    Dim myIconButton As MyIconButton = New MyIconButton() With { .Logo = "M651.946667 1001.813333c-22.186667 0-42.666667-10.24-61.44-27.306666-23.893333-23.893333-49.493333-35.84-75.093334-35.84-29.013333 0-56.32 11.946667-73.386666 30.72v3.413333c-17.066667 17.066667-42.666667 27.306667-66.56 27.306667h-6.826667c-6.826667 0-11.946667-1.706667-15.36-1.706667l-6.826667-1.706667c-64.853333-20.48-121.173333-54.613333-168.96-98.986666-29.013333-23.893333-37.546667-63.146667-25.6-95.573334 8.533333-23.893333 5.12-51.2-10.24-75.093333-15.36-27.306667-34.133333-40.96-59.733333-47.786667h-1.706667l-5.12-1.706666c-35.84-8.533333-61.44-34.133333-66.56-69.973334C1.706667 575.146667 0 537.6 0 512c0-32.426667 3.413333-63.146667 8.533333-93.866667v-6.826666l3.413334-8.533334c10.24-23.893333 23.893333-40.96 44.373333-51.2 5.12-3.413333 11.946667-6.826667 20.48-8.533333 27.306667-8.533333 51.2-25.6 63.146667-44.373333 13.653333-23.893333 17.066667-52.906667 10.24-81.92-11.946667-34.133333 0-71.68 30.72-93.866667 44.373333-37.546667 97.28-68.266667 158.72-93.866667l3.413333-1.706666c44.373333-13.653333 75.093333 3.413333 92.16 20.48 23.893333 23.893333 49.493333 35.84 75.093333 35.84 30.72 0 56.32-10.24 71.68-30.72l3.413334-3.413334c27.306667-27.306667 63.146667-35.84 93.866666-22.186666 63.146667 22.186667 117.76 54.613333 165.546667 97.28 29.013333 23.893333 37.546667 63.146667 25.6 95.573333-8.533333 23.893333-5.12 51.2 10.24 75.093333 15.36 27.306667 34.133333 40.96 59.733333 47.786667h1.706667l5.12 1.706667c35.84 8.533333 61.44 34.133333 66.56 71.68 6.826667 30.72 10.24 63.146667 11.946667 93.866666v3.413334c0 32.426667-3.413333 63.146667-8.533334 93.866666v6.826667l-3.413333 8.533333c-10.24 23.893333-23.893333 40.96-44.373333 51.2-5.12 3.413333-11.946667 6.826667-20.48 8.533334-27.306667 8.533333-51.2 25.6-63.146667 46.08-13.653333 23.893333-17.066667 52.906667-10.24 81.92 11.946667 35.84-1.706667 75.093333-30.72 95.573333-44.373333 35.84-95.573333 66.56-157.013333 92.16-15.36 3.413333-27.306667 3.413333-35.84 3.413333z m3.413333-83.626666z m1.706667 0zM517.12 853.333333c47.786667 0 93.866667 20.48 134.826667 59.733334 1.706667 1.706667 3.413333 1.706667 3.413333 3.413333 52.906667-22.186667 97.28-49.493333 136.533333-80.213333l1.706667-1.706667v-3.413333c-13.653333-52.906667-8.533333-104.106667 17.066667-148.48 23.893333-39.253333 64.853333-69.973333 114.346666-85.333334 1.706667 0 3.413333-1.706667 6.826667-6.826666 5.12-25.6 8.533333-51.2 8.533333-78.506667-1.706667-29.013333-3.413333-56.32-10.24-81.92v-5.12h-1.706666c-51.2-11.946667-90.453333-39.253333-119.466667-87.04-27.306667-44.373333-34.133333-100.693333-17.066667-148.48l-1.706666-1.706667h-3.413334c-39.253333-35.84-85.333333-63.146667-136.533333-80.213333H648.533333s-1.706667 1.706667-3.413333 1.706667c-32.426667 39.253333-80.213333 59.733333-136.533333 59.733333-47.786667 0-93.866667-20.48-134.826667-59.733333l-1.706667-1.706667h-1.706666c-54.613333 22.186667-98.986667 49.493333-136.533334 80.213333l-1.706666 1.706667v3.413333c13.653333 52.906667 8.533333 104.106667-17.066667 148.48-23.893333 39.253333-64.853333 69.973333-114.346667 85.333334-1.706667 0-3.413333 1.706667-6.826666 6.826666-6.826667 25.6-8.533333 51.2-8.533334 78.506667 0 30.72 3.413333 58.026667 6.826667 76.8l1.706667 5.12h1.706666c51.2 11.946667 90.453333 39.253333 119.466667 87.04 27.306667 44.373333 34.133333 100.693333 17.066667 148.48l1.706666 1.706667 1.706667 1.706666c37.546667 35.84 83.626667 63.146667 134.826667 80.213334 1.706667 0 3.413333 0 3.413333 1.706666h1.706667s1.706667 0 5.12-1.706666c34.133333-37.546667 81.92-59.733333 136.533333-59.733334z m-6.826667-146.773333c-110.933333 0-199.68-85.333333-199.68-196.266667 0-109.226667 87.04-196.266667 199.68-196.266666s199.68 85.333333 199.68 196.266666c-1.706667 109.226667-88.746667 196.266667-199.68 196.266667z m0-307.2c-63.146667 0-114.346667 49.493333-114.346666 110.933333 0 63.146667 49.493333 110.933333 114.346666 110.933334 30.72 0 59.733333-11.946667 80.213334-32.426667 20.48-20.48 32.426667-49.493333 32.426666-78.506667 0-63.146667-49.493333-110.933333-112.64-110.933333z", .LogoScale = 1.1 }
                    AddHandler myIconButton.Click, Sub(sender As Object, e As EventArgs)
                        CS$<>8__locals1.$VB$Local_ContMenu.PlacementTarget = CS$<>8__locals1.$VB$Local_NewItem
                        CS$<>8__locals1.$VB$Local_ContMenu.IsOpen = True
                    End Sub
                    CS$<>8__locals1.$VB$Local_NewItem.Buttons = New MyIconButton() { myIconButton }
                    ModMain.FrmSelectLeft.PanList.Children.Add(CS$<>8__locals1.$VB$Local_NewItem)
                    Logger.Info(String.Format("有效的 Minecraft 文件夹：{0} > {1}", mcFolder.Name, mcFolder.Location), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
                Next
                ModMain.FrmSelectLeft.PanList.Children.Add(New TextBlock() With { .Text = "添加或导入", .Margin = New Thickness(13.0, 18.0, 5.0, 4.0), .Opacity = 0.6, .FontSize = 12.0 })
                If Not DirectoryUtils.Exists(Paths.Base + ".minecraft\") Then
                    Dim myListItem As MyListItem = New MyListItem() With { .IsScaleAnimationEnabled = False, .Type = MyListItem.CheckType.Clickable, .Title = "新建 .minecraft 文件夹", .Height = 34.0, .ToolTip = "在 PCL 当前所在文件夹下创建新的 .minecraft 文件夹", .LogoScale = 0.9, .Logo = "M103.331925 384.978025l25.805736 0L129.137661 161.847132c0-18.313088 14.905478-33.718963 33.718963-33.718963l0.969071 0 253.006318 0c10.82044 0 20.218484 4.797259 26.500561 12.257162l117.579929 126.753869 297.819966 0c18.297738 0 33.736359 15.179724 33.736359 33.977859l0 0.952698 0 82.909292 25.547863 0c18.538215 0 34.187637 15.179724 34.187637 33.977859 0 2.163269-0.469698 3.617387-0.469698 5.539156l-54.437843 432.971086c-1.210571 10.382465-7.007601 19.056008-14.968923 24.352641-6.249331 5.765307-14.680351 9.624195-23.595394 9.624195l-0.969071 0-694.906773 0c-9.155521 0-17.344017-3.858888-23.626094-9.155521-8.67252-5.765307-14.453177-14.939247-15.389502-25.758664L69.597613 423.040922c-2.165316-18.313088 10.868535-35.414581 29.665647-38.062897L103.331925 384.978025 103.331925 384.978025zM196.576609 384.978025 196.576609 384.978025l627.938546 0 0-49.625234L546.461371 335.352791l0 0c-9.400091 0-18.329461-4.117784-25.048489-11.110035L402.363486 196.067514 196.576609 196.067514 196.576609 384.978025 196.576609 384.978025zM879.469767 452.916347 879.469767 452.916347l-20.267603 0-0.469698 0-0.969071 0-694.906773 0-0.984421 0-20.218484 0 45.781696 366.728382 646.218888 0L879.469767 452.916347 879.469767 452.916347z" }
                    ModMain.FrmSelectLeft.PanList.Children.Add(myListItem)
                    Dim myListItem2 As MyListItem = myListItem
                    Dim CS$<>8__locals6 As PageSelectLeft._Closure$__R4-2 = New PageSelectLeft._Closure$__R4-2(CS$<>8__locals6)
                    CS$<>8__locals6.$VB$NonLocal_3 = ModMain.FrmSelectLeft
                    AddHandler myListItem2.Click, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                        CS$<>8__locals6.$VB$NonLocal_3.Create_Click()
                    End Sub
                End If
                Dim myListItem3 As MyListItem = New MyListItem() With { .IsScaleAnimationEnabled = False, .Type = MyListItem.CheckType.Clickable, .Title = "添加已有文件夹", .Height = 34.0, .ToolTip = "将一个已有的 Minecraft 文件夹添加到列表", .Logo = "M512.277 954.412c-118.89 0-230.659-46.078-314.73-129.73S67.12 629.666 67.12 511.222s46.327-229.744 130.398-313.427 195.82-129.73 314.73-129.73 230.659 46.078 314.72 129.73S957.397 392.81 957.397 511.183 911.078 740.96 826.97 824.642s-195.8 129.77-314.692 129.77z m0-822.784c-101.972 0-197.809 39.494-269.865 111.222s-111.7 166.997-111.7 268.373 39.653 196.695 111.67 268.335S410.246 890.78 512.248 890.78s197.809-39.484 269.865-111.222 111.7-166.998 111.67-268.374c-0.03-101.375-39.654-196.665-111.67-268.303S614.22 131.628 512.277 131.628z m222.585 347.8H544.073V288.64c-0.76-17.561-15.613-31.18-33.173-30.419-16.495 0.714-29.704 13.924-30.419 30.419v190.787H289.703c-17.56 0.761-31.179 15.614-30.419 33.174 0.715 16.494 13.924 29.703 30.42 30.418H480.48v190.788c0.761 17.56 15.614 31.179 33.174 30.419 16.494-0.715 29.703-13.925 30.418-30.42V543.02h190.788c17.56 0.762 32.413-12.857 33.173-30.418 0.762-17.561-12.858-32.414-30.419-33.174a31.683 31.683 0 0 0-2.753 0z" }
                ModMain.FrmSelectLeft.PanList.Children.Add(myListItem3)
                Dim myListItem4 As MyListItem = myListItem3
                Dim CS$<>8__locals7 As PageSelectLeft._Closure$__R4-3 = New PageSelectLeft._Closure$__R4-3(CS$<>8__locals7)
                CS$<>8__locals7.$VB$NonLocal_4 = ModMain.FrmSelectLeft
                AddHandler myListItem4.Click, Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    CS$<>8__locals7.$VB$NonLocal_4.Add_Click()
                End Sub
                Dim myListItem5 As MyListItem = New MyListItem() With { .IsScaleAnimationEnabled = False, .Type = MyListItem.CheckType.Clickable, .Title = "导入整合包", .Height = 34.0, .ToolTip = "在当前选择的 Minecraft 文件夹下安装整合包", .Logo = "M512 40.96C249.344 40.96 35.84 252.416 35.84 512s213.504 471.04 476.16 471.04c103.424 0 202.752-33.28 286.72-96.256l1.536-1.536c5.12-5.632 7.68-12.8 7.68-19.968 0-16.896-13.824-30.208-30.72-30.208-7.68 0-15.36 2.56-20.992 7.68h-0.512c-71.68 52.224-155.648 79.36-243.712 79.36-227.328 0-412.16-182.784-412.16-407.552 0-224.768 184.832-407.552 412.16-407.552s412.16 182.784 412.16 407.552c0 68.608-15.872 132.608-46.592 190.464-0.512 1.024-1.024 2.048-1.024 3.072-0.512 2.048-1.536 4.608-1.536 8.192 0 16.896 13.824 30.208 30.72 30.208 12.288 0 23.04-7.168 28.16-18.432 35.84-68.608 53.76-141.312 53.76-216.064 0.512-259.584-212.992-471.04-475.648-471.04z M812.032 483.328c-31.744-20.992-71.68 1.536-78.848 6.144-1.024 0.512-104.448 61.44-128 74.752-8.192 4.608-22.528-0.512-27.136-4.096-31.232-36.352-54.272-70.656-68.608-102.4-13.312-29.184 0.512-41.472 3.072-43.52 7.168-4.608 114.688-68.608 143.36-83.456 24.064-12.288 40.96-25.088 46.08-45.056 3.072-13.312 0-27.136-9.216-39.936-22.016-31.744-172.544-84.992-311.296-3.584-157.184 91.648-152.064 242.688-150.528 292.352v9.216c0 18.944-12.8 37.376-14.848 40.448l-20.992 21.504c-6.144 6.144-9.216 13.824-9.216 22.528 0 8.704 3.584 16.384 9.728 22.528 12.8 12.288 32.768 11.776 45.056-0.512l22.528-23.552 0.512-0.512c3.072-3.584 30.208-38.4 30.208-81.92l-0.512-11.264c-1.536-44.544-5.632-162.816 119.296-235.52 88.064-51.2 173.056-32.256 208.896-19.968-36.864 19.456-143.36 83.456-144.896 84.48-22.016 14.336-55.808 58.88-26.112 122.88 17.408 37.376 43.52 76.8 80.896 120.32 14.336 17.408 62.976 37.376 103.424 15.36 24.576-13.312 125.44-73.216 130.048-75.776 2.048-1.024 4.608-2.56 7.68-3.584 0 2.56-0.512 6.144-1.024 10.752-5.632 35.84-35.328 155.136-191.488 181.76-49.664 8.704-89.6 3.584-121.856-0.512h-0.512c-37.888-4.608-73.216-9.216-101.888 14.336-31.232 26.112-40.96 34.304-35.84 54.272 3.584 14.336 16.384 24.064 30.72 24.064 2.56 0 5.12-0.512 7.68-1.024 6.656-1.536 12.8-5.632 16.896-10.752 2.048-2.048 7.68-6.656 20.992-18.432 6.656-5.632 25.088-3.584 52.736 0 34.816 4.608 81.92 10.24 141.312 0.512 157.184-26.624 228.864-138.752 243.2-234.496 7.68-38.912 0-64.512-21.504-78.336z" }
                ModMain.FrmSelectLeft.PanList.Children.Add(myListItem5)
                Dim myListItem6 As MyListItem = myListItem5
                Dim clickEventHandler As MyListItem.ClickEventHandler
                If PageSelectLeft._Closure$__.$IR4-8 IsNot Nothing Then
                    clickEventHandler = PageSelectLeft._Closure$__.$IR4-8
                Else
                    Dim clickEventHandler2 As MyListItem.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                        ModModpack.ModpackInstall()
                    End Sub
                    clickEventHandler = clickEventHandler2
                    PageSelectLeft._Closure$__.$IR4-8 = clickEventHandler2
                End If
                AddHandler myListItem6.Click, clickEventHandler
                ModMain.FrmSelectLeft.PanList.Children.Add(New FrameworkElement() With { .Height = 10.0, .IsHitTestVisible = False })
                Dim num2 As Integer = ModMinecraft.McFolderList.Count - 1
                For k As Integer = 0 To num2
                    If PathUtils.IsSame(ModMinecraft.McFolderList(k).Location, ModMinecraft.McFolderSelected) Then
                        CType(ModMain.FrmSelectLeft.PanList.Children(k + 1), MyListItem).Checked = True
                        Return
                    End If
                Next
                If Not ModMinecraft.McFolderList.Any() Then
                    Throw New ArgumentNullException("没有可用的 Minecraft 文件夹")
                End If
                ModMinecraft.McFolderSelected = ModMinecraft.McFolderList.First().Location
                CType(ModMain.FrmSelectLeft.PanList.Children(1), MyListItem).Checked = True
            Catch ex As Exception
                Logger.[Error](ex, "构建 Minecraft 文件夹列表 UI 出错", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
            Finally
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.RunOnUpdated, 1, "versions\", False)
            End Try
        End Sub

        Public Sub Add_Click()
            Dim text As String = ""
            If ModMinecraft.McInstanceLock.IsAnyLocked OrElse ModNet.HasDownloadingTask(False) Then
                ModMain.Hint("请在其他任务完成后，再添加游戏文件夹！", ModMain.HintType.Red, True)
                Return
            End If
            Try
                text = Dialogs.SelectFolder("选择 Minecraft 文件夹", False, Nothing).FirstOrDefault()
                If text IsNot Nothing Then
                    If text.Contains("!") OrElse text.Contains(";") Then
                        ModMain.Hint("Minecraft 文件夹路径中不能含有感叹号或分号！", ModMain.HintType.Red, True)
                    Else
                        Dim text2 As String = PathUtils.GetLastPart(text)
                        If Operators.CompareString(text2, ".minecraft", False) = 0 Then
                            Dim text3 As String = PathUtils.RemoveLastPart(text)
                            text2 = If(text3.EndsWithF(":", False), "", PathUtils.GetLastPart(text3))
                        End If
                        If text2.Length > 40 Then
                            text2 = text2.Substring(0, 39)
                        End If
                        Dim text4 As String = ModMain.MyMsgBoxInput("输入显示名称", "输入该文件夹在左边栏列表中显示的名称。", text2, New Collection(Of Validate)() From { New ValidateNullOrWhiteSpace(), New ValidateLength(1, 30), New ValidateExcept(New String() { ">", "|" }, "输入内容不能包含 %") }, "", "确定", "取消", False)
                        If Not String.IsNullOrWhiteSpace(text4) Then
                            PageSelectLeft.AddFolder(text, text4, True)
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("添加文件夹失败（{0}）", text), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
            End Try
        End Sub

        Public Shared Sub AddFolder(FolderPath As String, DisplayName As String, ShowHint As Boolean)
            Dim CS$<>8__locals1 As PageSelectLeft._Closure$__7-0 = New PageSelectLeft._Closure$__7-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FolderPath = FolderPath
            CS$<>8__locals1.$VB$Local_DisplayName = DisplayName
            CS$<>8__locals1.$VB$Local_ShowHint = ShowHint
            ModBase.RunInThread(Sub()
                Try
                    CS$<>8__locals1.$VB$Local_FolderPath = PathUtils.AddSlashSuffix(CS$<>8__locals1.$VB$Local_FolderPath)
                    If Not ModBase.CheckPermission(CS$<>8__locals1.$VB$Local_FolderPath) Then
                        If Not CS$<>8__locals1.$VB$Local_ShowHint Then
                            Throw New Exception("PCL 没有访问文件夹的权限：" + CS$<>8__locals1.$VB$Local_FolderPath)
                        End If
                        ModMain.Hint("添加文件夹失败：PCL 没有访问该文件夹的权限！", ModMain.HintType.Red, True)
                    Else
                        If Not ModBase.CheckPermission(CS$<>8__locals1.$VB$Local_FolderPath + "versions\") Then
                            Try
                                For Each text As String In DirectoryUtils.EnumerateDirectories(CS$<>8__locals1.$VB$Local_FolderPath, True, "*")
                                    If ModBase.CheckPermission(text + "\versions\") Then
                                        CS$<>8__locals1.$VB$Local_FolderPath = text + "\"
                                        Exit For
                                    End If
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of String)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                        End If
                        Dim list As List(Of String) = New List(Of String)(Settings.[Get](Of String)("LaunchFolders", Nothing).Split("|", False))
                        Dim flag As Boolean = False
                        Dim flag2 As Boolean = False
                        Dim num As Integer = list.Count - 1
                        Dim i As Integer = 0
                        While i <= num
                            Dim text2 As String = list(i)
                            If Operators.CompareString(text2, "", False) <> 0 AndAlso PathUtils.IsSame(text2.Split(">", False)(1), CS$<>8__locals1.$VB$Local_FolderPath) Then
                                flag = True
                                If Operators.CompareString(text2.Split(">", False)(0), CS$<>8__locals1.$VB$Local_DisplayName, False) = 0 Then
                                    If CS$<>8__locals1.$VB$Local_ShowHint Then
                                        ModMain.Hint("此文件夹已在列表中！", ModMain.HintType.Blue, True)
                                    End If
                                    Return
                                End If
                                list(i) = CS$<>8__locals1.$VB$Local_DisplayName + ">" + CS$<>8__locals1.$VB$Local_FolderPath
                                flag2 = True
                                If CS$<>8__locals1.$VB$Local_ShowHint Then
                                    ModMain.Hint("文件夹名称已更新为 " + CS$<>8__locals1.$VB$Local_DisplayName + " ！", ModMain.HintType.Green, True)
                                    Exit While
                                End If
                                Exit While
                            Else
                                i += 1
                            End If
                        End While
                        If Not flag Then
                            list.Add(CS$<>8__locals1.$VB$Local_DisplayName + ">" + CS$<>8__locals1.$VB$Local_FolderPath)
                        End If
                        Settings.[Set]("LaunchFolders", list.Join("|"), Nothing)
                        ModMinecraft.McFolderSelected = CS$<>8__locals1.$VB$Local_FolderPath
                        ModMinecraft.McFolderListLoader.Start(Nothing, True)
                        If Not flag2 Then
                            If CS$<>8__locals1.$VB$Local_ShowHint Then
                                ModMain.Hint("文件夹 " + CS$<>8__locals1.$VB$Local_DisplayName + " 已添加！", ModMain.HintType.Green, True)
                            End If
                            Dim info As DirectoryInfo = DirectoryUtils.GetInfo(CS$<>8__locals1.$VB$Local_FolderPath + "mods\")
                            If info.Exists AndAlso info.EnumerateFiles().Count() >= 3 Then
                                Dim info2 As DirectoryInfo = DirectoryUtils.GetInfo(CS$<>8__locals1.$VB$Local_FolderPath + "versions\")
                                If info2.Exists AndAlso info2.EnumerateDirectories().Count() <= 3 Then
                                    Try
                                        For Each directoryInfo As DirectoryInfo In info2.EnumerateDirectories()
                                            Dim mcInstance As ModMinecraft.McInstance = New ModMinecraft.McInstance(directoryInfo.FullName)
                                            mcInstance.Load()
                                            If mcInstance.Modable Then
                                                Dim info3 As DirectoryInfo = DirectoryUtils.GetInfo(mcInstance.PathVersion + "mods\")
                                                If info3.Exists AndAlso info3.EnumerateFiles().Any() Then
                                                    Exit For
                                                End If
                                                Settings.[Set]("VersionArgumentIndie", 2, mcInstance)
                                                Settings.[Set]("VersionArgumentIndieV2", False, mcInstance)
                                                Logger.Warn(String.Format("已自动关闭单版本隔离：{0}", mcInstance.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator2 As IEnumerator(Of DirectoryInfo)
                                        If enumerator2 IsNot Nothing Then
                                            enumerator2.Dispose()
                                        End If
                                    End Try
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "向文件夹列表中添加新文件夹失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
                End Try
            End Sub)
        End Sub

        Public Sub Create_Click()
            If ModMinecraft.McInstanceLock.IsAnyLocked OrElse ModNet.HasDownloadingTask(False) Then
                ModMain.Hint("请在其他任务完成后，再创建游戏文件夹！", ModMain.HintType.Red, True)
                Return
            End If
            PageSelectLeft.CreateMcFolderInCurrentPath()
            ModMain.Hint("新建 .minecraft 文件夹成功！", ModMain.HintType.Green, True)
        End Sub

        Public Shared Sub CreateMcFolderInCurrentPath()
            ModMinecraft.McFolderSelected = Paths.Base + ".minecraft\"
            If Not DirectoryUtils.Exists(ModMinecraft.McFolderSelected) Then
                DirectoryUtils.Create(ModMinecraft.McFolderSelected + "versions\")
                ModMinecraft.McFolderLauncherProfilesJsonCreate(ModMinecraft.McFolderSelected)
            End If
            PageSelectLeft.AddFolder(ModMinecraft.McFolderSelected, PathUtils.GetLastPart(Paths.Base), False)
        End Sub

        Public Sub Remove_Click(sender As Object, e As RoutedEventArgs)
            Try
                Dim mcFolder As ModMinecraft.McFolder = CType(CType(CType(CType(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), ContextMenu).Parent, Popup).PlacementTarget, MyListItem).Tag, ModMinecraft.McFolder)
                If mcFolder.Type = ModMinecraft.McFolder.Types.Custom Then
                    If ModMinecraft.McInstanceLock.IsAnyLocked OrElse ModNet.HasDownloadingTask(False) Then
                        ModMain.Hint("请在其他任务完成后，再移除游戏文件夹！", ModMain.HintType.Red, True)
                        Return
                    End If
                    Select Case ModMain.MyMsgBox("是否需要清理 PCL 在该文件夹中的配置文件？" & vbCrLf & "这包括各个版本的独立设置（如自定义图标、第三方登录配置）等，对游戏本身没有影响。", "配置文件清理", "删除", "保留", "取消", False, True, False, Nothing, Nothing, Nothing)
                        Case 1
                            FileUtils.Delete(mcFolder.Location + "PCL.ini", False)
                            Try
                                For Each text As String In DirectoryUtils.EnumerateDirectories(mcFolder.Location + "versions\", False, "*")
                                    DirectoryUtils.Delete(Path.Combine(text, "PCL"), True)
                                Next
                                GoTo IL_0107
                            Finally
                                Dim enumerator As IEnumerator(Of String)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                        Case 2
                            GoTo IL_0107
                        Case 3
                        Case Else
                            GoTo IL_0107
                    End Select
                    Return
                End If
                IL_0107:
                Dim text2 As String = PageSelectLeft.RemoveFolderFromSetup(mcFolder)
                ModMain.Hint(If((mcFolder.Type = ModMinecraft.McFolder.Types.Custom), ("文件夹 " + text2 + " 已从列表中移除！"), "文件夹名称已复原！"), ModMain.HintType.Green, True)
                ModMinecraft.McFolderListLoader.Start(Nothing, True)
            Catch ex As Exception
                Logger.[Error](ex, "从列表中移除游戏文件夹失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
            End Try
        End Sub

        Public Sub Delete_Click(sender As Object, e As RoutedEventArgs)
            If ModMinecraft.McInstanceLock.IsAnyLocked OrElse ModNet.HasDownloadingTask(False) Then
                ModMain.Hint("请在其他任务完成后，再删除游戏文件夹！", ModMain.HintType.Red, True)
                Return
            End If
            Dim Target As ModMinecraft.McFolder = CType(CType(CType(CType(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), ContextMenu).Parent, Popup).PlacementTarget, MyListItem).Tag, ModMinecraft.McFolder)
            Dim DeleteText As String = If(((Target.Type = ModMinecraft.McFolder.Types.Vanilla OrElse Target.Type = ModMinecraft.McFolder.Types.RenamedVanilla) AndAlso Operators.CompareString(Target.Location, Paths.Base + ".minecraft\", False) = 0 AndAlso ModMinecraft.McFolderList.IsSingle()), "清空", "删除")
            If ModMain.MyMsgBox(String.Concat(New String() { "你确定要", DeleteText, "这个文件夹吗？" & vbCrLf & "目标文件夹：", Target.Location, vbCrLf & vbCrLf & "该文件夹中的游戏存档、游戏版本，以及 MC 之外的其他文件，都会永久丢失，不可恢复！" }), "警告", "取消", "确认", "取消", False, True, False, Nothing, Nothing, Nothing) = 2 AndAlso ModMain.MyMsgBoxInput("删除确认", "该文件夹中的游戏存档、游戏版本，以及 MC 之外的其他文件，都会永久丢失，不可恢复！" & vbCrLf & "目标文件夹：" + Target.Location + vbCrLf & vbCrLf & "如果确实要删除，请在下面输入【Potato】这个单词以继续。", Nothing, New Collection(Of Validate)() From { New ValidateSame("Potato", String.Format("请输入 {0}Potato{1}", ModBase.vbLQ, ModBase.vbRQ), True) }, "", "永久删除文件夹", "取消", True) IsNot Nothing Then
                PageSelectLeft.RemoveFolderFromSetup(Target)
                ModBase.RunInNewThread(Sub()
                    Try
                        ModMain.Hint(String.Concat(New String() { "正在", DeleteText, "文件夹 ", Target.Name, "！" }), ModMain.HintType.Blue, True)
                        DirectoryUtils.Delete(Target.Location, False)
                        If Operators.CompareString(DeleteText, "清空", False) = 0 Then
                            DirectoryUtils.Create(Target.Location)
                        End If
                        ModMain.Hint(String.Concat(New String() { "已", DeleteText, "文件夹 ", Target.Name, "！" }), ModMain.HintType.Green, True)
                    Catch ex As Exception
                        Logger.[Error](ex, String.Format("{0}文件夹 {1} 失败", DeleteText, Target.Name), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
                    Finally
                        ModMinecraft.McFolderListLoader.Start(Nothing, True)
                    End Try
                End Sub, "Folder Delete " + Conversions.ToString(ModBase.GetUuid()), ThreadPriority.BelowNormal)
            End If
        End Sub

        Private Shared Function RemoveFolderFromSetup(Target As ModMinecraft.McFolder) As String
            Dim text As String = Nothing
            Dim list As List(Of String) = Settings.[Get](Of String)("LaunchFolders", Nothing).Split("|"c, True).ToList()
            Try
                For Each text2 As String In list.ToList()
                    If text2.Contains(">") AndAlso PathUtils.IsSame(text2.AfterFirst(">", False), Target.Location) Then
                        text = text2.BeforeFirst(">", False)
                        list.Remove(text2)
                        Exit For
                    End If
                Next
            Finally
                Dim enumerator As List(Of String).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Settings.[Set]("LaunchFolders", list.Join("|"), Nothing)
            Return text
        End Function

        Public Sub Open_Click(sender As Object, e As RoutedEventArgs)
            ModBase.OpenExplorer(CType(CType(CType(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), ContextMenu).Parent, Popup).PlacementTarget, MyListItem).Info)
        End Sub

        Public Sub Refresh_Click(sender As Object, e As RoutedEventArgs)
            PageSelectLeft.RefreshCurrent(CType(CType(CType(CType(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), ContextMenu).Parent, Popup).PlacementTarget, MyListItem).Tag, ModMinecraft.McFolder).Location)
        End Sub

        Public Sub RefreshCurrent() Implements PCL.IRefreshable.Refresh
            PageSelectLeft.RefreshCurrent(ModMinecraft.McFolderSelected)
        End Sub

        Public Shared Sub RefreshCurrent(Folder As String)
            ModBase.WriteIni(Folder + "PCL.ini", "InstanceCache", "")
            If PathUtils.IsSame(Folder, ModMinecraft.McFolderSelected) Then
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
            End If
        End Sub

        Public Sub Rename_Click(sender As Object, e As RoutedEventArgs)
            Dim mcFolder As ModMinecraft.McFolder = CType(CType(CType(CType(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), ContextMenu).Parent, Popup).PlacementTarget, MyListItem).Tag, ModMinecraft.McFolder)
            Try
                Dim text As String = ModMain.MyMsgBoxInput("输入新名称", "", mcFolder.Name, New Collection(Of Validate)() From { New ValidateNullOrWhiteSpace(), New ValidateLength(1, 30), New ValidateExcept(New String() { ">", "|" }, "输入内容不能包含 %") }, "", "确定", "取消", False)
                If Not String.IsNullOrWhiteSpace(text) Then
                    Dim list As List(Of String) = New List(Of String)(Settings.[Get](Of String)("LaunchFolders", Nothing).Split("|", False))
                    Dim flag As Boolean = False
                    Dim num As Integer = list.Count - 1
                    Dim i As Integer = 0
                    While i <= num
                        Dim text2 As String = list(i)
                        If Operators.CompareString(text2, "", False) <> 0 AndAlso PathUtils.IsSame(text2.Split(">", False)(1), mcFolder.Location) Then
                            flag = True
                            If Operators.CompareString(text2.Split(">", False)(0), text, False) = 0 Then
                                Return
                            End If
                            list(i) = text + ">" + mcFolder.Location
                            Exit While
                        Else
                            i += 1
                        End If
                    End While
                    If Not flag Then
                        list.Add(text + ">" + mcFolder.Location)
                    End If
                    ModMain.Hint("文件夹名称已更新为 " + text + " ！", ModMain.HintType.Green, True)
                    Settings.[Set]("LaunchFolders", list.Join("|"), Nothing)
                    ModMinecraft.McFolderListLoader.Start(Nothing, True)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "重命名文件夹失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageSelectLeft.xaml.vb")
            End Try
        End Sub

        Public Sub Folder_Change(sender As MyListItem, e As ModBase.RouteEventArgs)
            If e.RaiseByMouse AndAlso sender.Checked Then
                If ModMinecraft.McInstanceLock.IsAnyLocked OrElse ModNet.HasDownloadingTask(True) Then
                    ModMain.Hint("请在其他任务完成后，再切换游戏文件夹！", ModMain.HintType.Red, True)
                    e.Handled = True
                    Return
                End If
                ModMinecraft.McFolderSelected = CType(sender.Tag, ModMinecraft.McFolder).Location
                ModMinecraft.McFolderListLoader.Start(Nothing, True)
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.RunOnUpdated, 1, "versions\", False)
            End If
        End Sub

        Friend Overridable Property PanList As StackPanel

        Private IsFirstLoad As Boolean

        Private McFolderListLast As List(Of ModMinecraft.McFolder)
    End Class
End Namespace
