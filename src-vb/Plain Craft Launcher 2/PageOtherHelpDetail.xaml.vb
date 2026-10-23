Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageOtherHelpDetail
        Inherits MyPageRight
        Implements IRefreshable

        Public Sub New()
            AddHandler MyBase.Loaded, AddressOf Me.PageOtherHelpDetail_Loaded
            Me.InitializeComponent()
        End Sub

        Public Sub Refresh() Implements PCL.IRefreshable.Refresh
            Me.Init(New ModMain.HelpEntry(Me.Entry.RawPath))
        End Sub

        Private Sub PageOtherHelpDetail_Loaded(sender As Object, e As RoutedEventArgs)
            Me.PanBack.ScrollToTop()
        End Sub

        Public Function Init(Entry As ModMain.HelpEntry) As Boolean
            Dim text As String = If(Entry.XamlContent, "")
            If Operators.CompareString(text, "", False) = 0 Then
                Throw New Exception("帮助 xaml 文件为空")
            End If
            Dim flag As Boolean
            Try
                text = ModMain.ArgumentReplace(text, New Func(Of String, String)(StringUtils.XmlEscape), True)
                While text.Contains("xmlns")
                    text = text.RegexReplace("xmlns[^""']*(""|')[^""']*(""|')", "", RegexOptions.None).Replace("xmlns", "")
                End While
                text = "<StackPanel xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:sys=""clr-namespace:System;assembly=mscorlib"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:PCL;assembly=Plain Craft Launcher 2"" xmlns:core=""clr-namespace:MeloongCore;assembly=MeloongCore"" xmlns:corewpf=""clr-namespace:MeloongCore.Wpf;assembly=MeloongCore.Wpf"">" + text + "</StackPanel>"
                Me.Entry = Entry
                Me.PanCustom.Children.Clear()
                Me.PanCustom.Children.Add(CType(ModBase.GetObjectFromXML(text), UIElement))
                flag = True
            Catch ex As Exception
                Logger.Info(String.Format("自定义信息内容：{0}{1}", vbCrLf, text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageOther\PageOtherHelpDetail.xaml.vb")
                Logger.[Error](ex, "加载帮助 XAML 文件失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageOther\PageOtherHelpDetail.xaml.vb")
                flag = False
            End Try
            Return flag
        End Function

        Friend Overridable Property PanBack As MyScrollViewer

        Friend Overridable Property PanCustom As StackPanel

        Public Entry As ModMain.HelpEntry
    End Class
End Namespace
