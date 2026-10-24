Imports System
Imports System.CodeDom.Compiler
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Markup
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    <DesignerGenerated()>
    Public Partial Class PageInstanceExport
        Inherits OurPageRight
        Implements IRefreshable

        Public Sub New()
            AddHandler MyBase.Initialized, AddressOf Me.PageInstanceExport_Initialized
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.PageInstanceExport_Loaded()
            End Sub
            AddHandler MyBase.Loaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.RefreshJavaInfo()
            End Sub
            Me.CurrentInstance = ""
            Me.SubOptionBlackList = New String() { "Quark Programmer Art.zip", "+ EuphoriaPatches_", "PCL2 Skin.zip" }
            Me._RulesOverrides = Nothing
            Me.ExtraFiles = Nothing
            Me.ConfigPackPath = Nothing
            Me.InitializeComponent()
        End Sub

        Private Sub PageInstanceExport_Initialized(sender As Object, e As EventArgs)
            AddHandler PageInstanceLeft.CurrentJavaWorker.Stopped, Sub()
                ModBase.RunInUi(AddressOf Me.RefreshJavaInfo, False)
            End Sub
        End Sub

        Private Sub PageInstanceExport_Loaded()
            ' The following expression was wrapped in a checked-statement
            ModAnimation.AniControlEnabled += 1
            If Not PathUtils.IsSame(Me.CurrentInstance, PageInstanceLeft.Instance.PathVersion) Then
                Me.RefreshAll()
            End If
            CustomEventService.SetEventData(Me.BtnAdvancedHelp, "指南/整合包制作 - Snapshot.json")
            ModAnimation.AniControlEnabled -= 1
        End Sub

        Public Sub RefreshAll() Implements PCL.IRefreshable.Refresh
            Logger.Info("刷新导出页面", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
            Me.HintOptiFine.Visibility = If((PageInstanceLeft.Instance.Version.HasOptiFine AndAlso Not PageInstanceLeft.Instance.Version.OptiFineAsMod), Visibility.Visible, Visibility.Collapsed)
            Me.CurrentInstance = PageInstanceLeft.Instance.PathVersion
            Me.TextExportName.Text = ""
            Me.TextExportName.HintText = PageInstanceLeft.Instance.Name
            Me.TextExportVersion.Text = ""
            Me.TextExportVersion.HintText = "1.0.0"
            Me.CheckAdvancedInclude.Checked = False
            Me.CheckAdvancedModrinth.Checked = False
            Me.GetExportOption(Me.CheckOptionsBasic).Description = PageInstanceLeft.Instance.VersionDisplayName()
            Me.ResetConfigOverrides()
            Me.ReloadAllSubOptions()
            Me.RefreshAllOptionsUI()
            Me.RefreshJavaInfo()
            Me.PanBack.ScrollToHome()
        End Sub

        Private Sub ReloadAllSubOptions()
            Me.ReloadSubOptions(Me.PanOptionsResourcePacks, True, True, New String() { "resourcepacks", "texturepacks" })
            Me.ReloadSubOptions(Me.PanOptionsSaves, False, True, New String() { "saves" })
            Me.ReloadSubOptions(Me.PanOptionsShaderPacks, True, True, New String() { "shaderpacks" })
        End Sub

        Private Sub ReloadSubOptions(Panel As StackPanel, AcceptCompressedFile As Boolean, AcceptFolder As Boolean, ParamArray Folders As String())
            Panel.Children.Clear()
            For Each text As String In Folders
                Dim info As DirectoryInfo = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + text)
                If info.Exists Then
                    If AcceptCompressedFile Then
                        Try
                            Dim enumerator As IEnumerator(Of FileInfo) = info.EnumerateFiles("*.zip").Concat(info.EnumerateFiles("*.rar")).GetEnumerator()
                            While enumerator.MoveNext()
                                Dim CS$<>8__locals1 As PageInstanceExport._Closure$__7-0 = New PageInstanceExport._Closure$__7-0(CS$<>8__locals1)
                                CS$<>8__locals1.$VB$Local_File = enumerator.Current
                                If Not Me.SubOptionBlackList.Any(Function(b As String) CS$<>8__locals1.$VB$Local_File.Name.Contains(b)) Then
                                    Panel.Children.Add(New OurCheckBox() With { .Tag = New ExportOption() With { .Title = CS$<>8__locals1.$VB$Local_File.Name, .DefaultChecked = True, .Rules = StringUtils.LikePatternEscape(String.Format("{0}/{1}", text, CS$<>8__locals1.$VB$Local_File.Name)) } })
                                End If
                            End While
                        Finally
                            Dim enumerator As IEnumerator(Of FileInfo)
                            If enumerator IsNot Nothing Then
                                enumerator.Dispose()
                            End If
                        End Try
                    End If
                    If AcceptFolder Then
                        Try
                            Dim enumerable As IEnumerable(Of DirectoryInfo) = info.EnumerateDirectories()
                            Dim func As Func(Of DirectoryInfo, DateTime)
                            If PageInstanceExport._Closure$__.$I7-1 IsNot Nothing Then
                                func = PageInstanceExport._Closure$__.$I7-1
                            Else
                                Dim func2 As Func(Of DirectoryInfo, DateTime) = Function(f As DirectoryInfo) f.LastWriteTime
                                func = func2
                                PageInstanceExport._Closure$__.$I7-1 = func2
                            End If
                            Dim enumerator2 As IEnumerator(Of DirectoryInfo) = enumerable.OrderByDescending(func).GetEnumerator()
                            While enumerator2.MoveNext()
                                Dim CS$<>8__locals2 As PageInstanceExport._Closure$__7-1 = New PageInstanceExport._Closure$__7-1(CS$<>8__locals2)
                                CS$<>8__locals2.$VB$Local_SubFolder = enumerator2.Current
                                If Not Me.SubOptionBlackList.Any(Function(b As String) CS$<>8__locals2.$VB$Local_SubFolder.Name.Contains(b)) AndAlso CS$<>8__locals2.$VB$Local_SubFolder.EnumerateFileSystemInfos().Any() Then
                                    Dim myCheckBox As OurCheckBox = New OurCheckBox() With { .Tag = New ExportOption() With { .Title = CS$<>8__locals2.$VB$Local_SubFolder.Name, .DefaultChecked = True, .Rules = StringUtils.LikePatternEscape(String.Format("{0}/{1}/", text, CS$<>8__locals2.$VB$Local_SubFolder.Name)) } }
                                    If AcceptCompressedFile Then
                                        Me.GetExportOption(myCheckBox).Description = "文件夹"
                                    End If
                                    If Panel Is Me.PanOptionsSaves Then
                                        Me.GetExportOption(myCheckBox).Description = CS$<>8__locals2.$VB$Local_SubFolder.LastWriteTime.ToString("yyyy'/'MM'/'dd HH':'mm")
                                    End If
                                    Panel.Children.Add(myCheckBox)
                                End If
                            End While
                        Finally
                            Dim enumerator2 As IEnumerator(Of DirectoryInfo)
                            If enumerator2 IsNot Nothing Then
                                enumerator2.Dispose()
                            End If
                        End Try
                    End If
                End If
            Next
        End Sub

        Private Sub RefreshAllOptionsUI()
            Dim CS$<>8__locals1 As PageInstanceExport._Closure$__8-2 = New PageInstanceExport._Closure$__8-2(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_AllEntries = New List(Of String)()
            CS$<>8__locals1.$VB$Local_IsValidDirectory = Function(Folder As DirectoryInfo)
                Dim flag2 As Boolean
                Try
                    flag2 = Folder.Exists AndAlso Folder.EnumerateFileSystemInfos().Any(Function(i As FileSystemInfo)
                        Dim CS$<>8__locals3 As PageInstanceExport._Closure$__8-0 = New PageInstanceExport._Closure$__8-0(CS$<>8__locals3)
                        CS$<>8__locals3.$VB$Local_i = i
                        Return Not Me.SubOptionBlackList.Any(Function(b As String) CS$<>8__locals3.$VB$Local_i.Name.Contains(b))
                    End Function)
                Catch ex As Exception
                    flag2 = False
                End Try
                Return flag2
            End Function
            Dim info As DirectoryInfo = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie)
            Dim $VB$Local_AllEntries As List(Of String) = CS$<>8__locals1.$VB$Local_AllEntries
            Dim enumerable As IEnumerable(Of FileInfo) = info.EnumerateFiles()
            Dim func As Func(Of FileInfo, String)
            If PageInstanceExport._Closure$__.$I8-3 IsNot Nothing Then
                func = PageInstanceExport._Closure$__.$I8-3
            Else
                Dim func2 As Func(Of FileInfo, String) = Function(f As FileInfo) f.Name
                func = func2
                PageInstanceExport._Closure$__.$I8-3 = func2
            End If
            $VB$Local_AllEntries.AddRange(enumerable.[Select](func))
            Try
                Dim enumerable2 As IEnumerable(Of DirectoryInfo) = info.EnumerateDirectories()
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_14(Of DirectoryInfo, Boolean) = CS$<>8__locals1.$VB$Local_IsValidDirectory
                Dim enumerator As IEnumerator(Of DirectoryInfo) = enumerable2.Where(If((vb$AnonymousDelegate_ Is Nothing), Nothing, New Func(Of DirectoryInfo, Boolean)(vb$AnonymousDelegate_.Invoke))).GetEnumerator()
                While enumerator.MoveNext()
                    Dim CS$<>8__locals2 As PageInstanceExport._Closure$__8-1 = New PageInstanceExport._Closure$__8-1(CS$<>8__locals2)
                    CS$<>8__locals2.$VB$Local_SubFolder = enumerator.Current
                    CS$<>8__locals1.$VB$Local_AllEntries.Add(String.Format("{0}\", CS$<>8__locals2.$VB$Local_SubFolder.Name))
                    CS$<>8__locals1.$VB$Local_AllEntries.AddRange(CS$<>8__locals2.$VB$Local_SubFolder.EnumerateFiles().[Select](Function(f As FileInfo) String.Format("{0}\{1}", CS$<>8__locals2.$VB$Local_SubFolder.Name, f.Name)))
                    Dim $VB$Local_AllEntries2 As List(Of String) = CS$<>8__locals1.$VB$Local_AllEntries
                    Dim enumerable3 As IEnumerable(Of DirectoryInfo) = CS$<>8__locals2.$VB$Local_SubFolder.EnumerateDirectories()
                    vb$AnonymousDelegate_ = CS$<>8__locals1.$VB$Local_IsValidDirectory
                    $VB$Local_AllEntries2.AddRange(enumerable3.Where(If((vb$AnonymousDelegate_ Is Nothing), Nothing, New Func(Of DirectoryInfo, Boolean)(vb$AnonymousDelegate_.Invoke))).[Select](Function(d As DirectoryInfo) String.Format("{0}\{1}\", CS$<>8__locals2.$VB$Local_SubFolder.Name, d.Name)))
                End While
            Finally
                Dim enumerator As IEnumerator(Of DirectoryInfo)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Logger.Info(String.Format("共发现 {0} 个可行的二级文件/文件夹", CS$<>8__locals1.$VB$Local_AllEntries.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
            Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_15(Of ExportOption, Boolean) = Function(TargetOption As ExportOption)
                Dim flag3 As Boolean
                If TargetOption.RequireOptiFine AndAlso Not PageInstanceLeft.Instance.Version.HasOptiFine Then
                    flag3 = False
                ElseIf TargetOption.RequireModLoader AndAlso Not PageInstanceLeft.Instance.Modable Then
                    flag3 = False
                ElseIf TargetOption.RequireModLoaderOrOptiFine AndAlso Not PageInstanceLeft.Instance.Version.HasOptiFine AndAlso Not PageInstanceLeft.Instance.Modable Then
                    flag3 = False
                Else
                    Dim enumerable4 As IEnumerable(Of String) = CS$<>8__locals1.$VB$Me.StandardizeLines((If(TargetOption.Rules, TargetOption.ShowRules)).Split(New Char() { "|"c }), True)
                    Dim func3 As Func(Of String, Boolean)
                    If CS$<>8__locals1.$I7 IsNot Nothing Then
                        func3 = CS$<>8__locals1.$I7
                    Else
                        Dim func4 As Func(Of String, Boolean) = Function(Rule As String)
                            Dim CS$<>8__locals4 As PageInstanceExport._Closure$__8-3 = New PageInstanceExport._Closure$__8-3(CS$<>8__locals4)
                            CS$<>8__locals4.$VB$Local_Rule = Rule
                            Dim flag4 As Boolean
                            If CS$<>8__locals4.$VB$Local_Rule.StartsWithF("!", False) Then
                                flag4 = False
                            Else
                                Try
                                    If CS$<>8__locals1.$VB$Local_AllEntries.Any(Function(Entry As String) LikeOperator.LikeString(Entry.Lower(), CS$<>8__locals4.$VB$Local_Rule.Lower(), CompareMethod.Binary)) Then
                                        Return True
                                    End If
                                Catch ex As Exception
                                    Logger.[Error](ex, String.Format("错误的规则：{0}", CS$<>8__locals4.$VB$Local_Rule), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                                    Return False
                                End Try
                                CS$<>8__locals4.$VB$Local_Rule = CS$<>8__locals4.$VB$Local_Rule.Trim("*?".ToCharArray())
                                If CS$<>8__locals4.$VB$Local_Rule.Split("\"c, True).Count() >= 3 Then
                                    If CS$<>8__locals4.$VB$Local_Rule.EndsWithF("\", False) Then
                                        flag4 = CS$<>8__locals1.$VB$Local_IsValidDirectory(DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + CS$<>8__locals4.$VB$Local_Rule))
                                    Else
                                        flag4 = FileUtils.Exists(PageInstanceLeft.Instance.PathIndie + CS$<>8__locals4.$VB$Local_Rule)
                                    End If
                                Else
                                    flag4 = False
                                End If
                            End If
                            Return flag4
                        End Function
                        Dim func5 As Func(Of String, Boolean) = func4
                        CS$<>8__locals1.$I7 = func4
                        func3 = func5
                    End If
                    flag3 = enumerable4.Any(func3)
                End If
                Return flag3
            End Function
            Try
                For Each myCheckBox As OurCheckBox In Me.GetAllOptions(True)
                    Dim exportOption As ExportOption = Me.GetExportOption(myCheckBox)
                    If String.IsNullOrEmpty(exportOption.Rules) AndAlso String.IsNullOrEmpty(exportOption.ShowRules) Then
                        myCheckBox.Visibility = Visibility.Visible
                        myCheckBox.Checked = exportOption.DefaultChecked
                    Else
                        Dim flag As Boolean = vb$AnonymousDelegate_2(exportOption)
                        myCheckBox.Visibility = If(flag, Visibility.Visible, Visibility.Collapsed)
                        myCheckBox.Checked = exportOption.DefaultChecked AndAlso flag
                    End If
                    myCheckBox.Inlines.Clear()
                    If myCheckBox.Visibility = Visibility.Visible Then
                        myCheckBox.Inlines.Add(New Run(exportOption.Title))
                        If Not String.IsNullOrEmpty(exportOption.Description) Then
                            myCheckBox.Inlines.Add(New Run("   " + exportOption.Description) With { .Foreground = ModSecret.ColorGray5 })
                        End If
                    End If
                Next
            Finally
                Dim enumerator2 As IEnumerator(Of OurCheckBox)
                If enumerator2 IsNot Nothing Then
                    enumerator2.Dispose()
                End If
            End Try
        End Sub

        Private Iterator Function StandardizeLines(Raw As IEnumerable(Of String), AddSuffixStarToFolderPath As Boolean) As IEnumerable(Of String)
            Try
                For Each text As String In Raw
                    text = text.Trim()
                    If Operators.CompareString(text, "", False) <> 0 AndAlso Not text.StartsWithF("#", False) AndAlso Not text.StartsWithF("=", False) Then
                        text = text.Replace("/", "\")
                        Yield text + If((text.EndsWithF("\", False) AndAlso AddSuffixStarToFolderPath), "*", "")
                    End If
                Next
                Return
            Finally
                Dim enumerator As IEnumerator(Of String)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Return
        End Function

        Private Iterator Function GetAllOptions(IncludeHidden As Boolean) As IEnumerable(Of OurCheckBox)
            Try
                For Each obj As Object In Me.PanOptions.Children
                    Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
                    If IncludeHidden OrElse Not Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(objectValue, Nothing, "Visibility", New Object(-1) {}, Nothing, Nothing, Nothing), Visibility.Visible, False) Then
                        If TypeOf objectValue Is OurCheckBox Then
                            Yield CType(objectValue, OurCheckBox)
                        ElseIf TypeOf objectValue Is StackPanel Then
                            Try
                                For Each obj2 As Object In CType(objectValue, StackPanel).Children
                                    Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(obj2)
                                    If(IncludeHidden OrElse Not Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(objectValue2, Nothing, "Visibility", New Object(-1) {}, Nothing, Nothing, Nothing), Visibility.Visible, False)) AndAlso TypeOf objectValue2 Is OurCheckBox Then
                                        Yield CType(objectValue2, OurCheckBox)
                                    End If
                                Next
                            Finally
                                Dim enumerator2 As IEnumerator
                                If TypeOf enumerator2 Is IDisposable Then
                                    TryCast(enumerator2, IDisposable).Dispose()
                                End If
                            End Try
                        End If
                    End If
                Next
                Return
            Finally
                Dim enumerator As IEnumerator
                If TypeOf enumerator Is IDisposable Then
                    TryCast(enumerator, IDisposable).Dispose()
                End If
            End Try
            Return
        End Function

        Private Function GetExportOption(CheckBox As OurCheckBox) As ExportOption
            Return CType(CheckBox.Tag, ExportOption)
        End Function

        Private Property RulesOverrides As List(Of String)
            Get
                Return Me._RulesOverrides
            End Get
            Set(value As List(Of String))
                Me._RulesOverrides = value
                If value Is Nothing Then
                    Me.BtnOverrideCancel.Visibility = Visibility.Collapsed
                    Me.PanOptions.Visibility = Visibility.Visible
                    Me.CardOptions.Inlines.Clear()
                    Me.CardOptions.Inlines.Add(New Run("导出内容列表") With { .FontWeight = FontWeights.Bold })
                    Return
                End If
                Me.BtnOverrideCancel.Visibility = Visibility.Visible
                Me.PanOptions.Visibility = Visibility.Collapsed
                Me.CardOptions.Inlines.Clear()
                Me.CardOptions.Inlines.Add(New Run("导出内容列表:    ") With { .FontWeight = FontWeights.Bold })
                Me.CardOptions.Inlines.Add(New Run("从配置文件中读取") With { .FontWeight = FontWeights.Normal })
            End Set
        End Property

        Private Iterator Function GetAllRules() As IEnumerable(Of String)
            If Me.RulesOverrides IsNot Nothing Then
                Try
                    For Each text As String In Me.RulesOverrides
                        Yield text
                    Next
                    Return
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
            End If
            Yield ""
            Yield "# 修改下方的规则以控制需要导出的内容。"
            Yield "# 以 ! 开头以反选。可以使用 *、?、[] 通配符。靠后的行覆盖靠前的。"
            Yield ""
            Try
                For Each myCheckBox As OurCheckBox In Me.GetAllOptions(False)
                    If myCheckBox.Checked Then
                        Dim exportOption As ExportOption = Me.GetExportOption(myCheckBox)
                        Dim rules As String = exportOption.Rules
                        Dim Rules As List(Of String) = If(If((rules IsNot Nothing), rules.Split(New Char() { "|"c }).ToList(), Nothing), New List(Of String)())
                        If myCheckBox Is Me.CheckOptionsShaderPackSettings Then
                            Try
                                For Each myCheckBox2 As OurCheckBox In Me.PanOptionsShaderPacks.Children.OfType()
                                    If myCheckBox2.Checked Then
                                        Dim exportOption2 As ExportOption = Me.GetExportOption(myCheckBox2)
                                        Dim text2 As String = String.Format("shaderpacks/{0}.txt", exportOption2.Title)
                                        If FileUtils.Exists(Path.Combine(PageInstanceLeft.Instance.PathIndie, text2)) Then
                                            Rules.Add(StringUtils.LikePatternEscape(text2))
                                        End If
                                    End If
                                Next
                            Finally
                                Dim enumerator3 As IEnumerator(Of OurCheckBox)
                                If enumerator3 IsNot Nothing Then
                                    enumerator3.Dispose()
                                End If
                            End Try
                        End If
                        If Rules.Any() Then
                            Yield String.Format("# {0}", exportOption.Title)
                            Try
                                For Each text3 As String In Rules
                                    Yield text3
                                Next
                            Finally
                                Dim enumerator4 As List(Of String).Enumerator
                                CType(enumerator4, IDisposable).Dispose()
                            End Try
                            Yield ""
                        End If
                    End If
                Next
            Finally
                Dim enumerator2 As IEnumerator(Of OurCheckBox)
                If enumerator2 IsNot Nothing Then
                    enumerator2.Dispose()
                End If
            End Try
            Yield "# 排除的文件"
            Yield "!*.log"
            Yield "!*.dat_old"
            Yield "!*.BakaCoreInfo"
            Yield "!hmclversion.cfg"
            Yield "!log4j2.xml"
            Yield ""
            Return
        End Function

        Private Iterator Function GetExtraFileLines() As IEnumerable(Of String)
            If Me.ExtraFiles IsNot Nothing Then
                Try
                    For Each text As String In Me.ExtraFiles
                        Yield text
                    Next
                    Return
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
            End If
            Yield ""
            Yield "# 如果想将额外的文件自动放到压缩包根目录中，可以将它们的路径写在下方。"
            Yield "# 必须是完整路径。每行中，若以 \ 结尾则代表是文件夹，不以 \ 结尾则代表是文件。"
            Yield ""
            Return
        End Function

        Private Sub ResetConfigOverrides()
            Me.RulesOverrides = Nothing
            Me.ConfigPackPath = Nothing
            Me.ExtraFiles = Nothing
            Me.PanBack.ScrollToHome()
        End Sub

        Private Sub CardOptions_MouseLeftButtonDown()
            If Me.RulesOverrides IsNot Nothing Then
                Me.ResetConfigOverrides()
            End If
        End Sub

        Private Sub ExportConfig()
            Try
                Dim text As String = Dialogs.SaveFile("选择文件位置", "export_config.txt", Settings.[Get](Of String)("CacheExportConfig", Nothing), New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)("txt", "整合包导出配置") })
                If text IsNot Nothing Then
                    Settings.[Set]("CacheExportConfig", text, Nothing)
                    Dim list As List(Of String) = New List(Of String)()
                    list.Add("Name:" + Me.TextExportName.Text)
                    list.Add("Version:" + Me.TextExportVersion.Text)
                    list.Add("")
                    list.Add(String.Format("# 当版本设置中选择了 {0}使用版本文件夹中的 Java{1} 时，是否打包版本文件夹中的 Java。", ModBase.vbLQ, ModBase.vbRQ))
                    list.Add("IncludeJava:" + Conversions.ToString(Me.CheckOptionsJava.Checked))
                    list.Add("")
                    list.Add("# 是否打包正式版 PCL，以便没有启动器的玩家安装整合包。")
                    list.Add("IncludeLauncher:" + Conversions.ToString(Me.CheckOptionsPcl.Checked))
                    list.Add("")
                    list.Add("# 是否打包 PCL 个性化内容，例如功能隐藏设置、主页、背景音乐和图片等。")
                    list.Add("IncludeLauncherCustom:" + Conversions.ToString(Me.CheckOptionsPclCustom.Checked))
                    list.Add("")
                    list.Add("# 是否将 Mod、资源包、光影包的文件直接放入整合包中，这样在导入时就无需联网下载它们。")
                    list.Add("# 建议仅在无法稳定连接 CurseForge 或 Modrinth 时才考虑启用。")
                    list.Add("# 二次分发可能违反使用协议，请尽量不要公开发布包含资源文件的整合包！")
                    list.Add("DontCheckHostedAssets:" + Conversions.ToString(Me.CheckAdvancedInclude.Checked))
                    list.Add("")
                    list.Add("# 若资源未在 Modrinth 上托管，则将其原始文件直接放入整合包中。")
                    list.Add("# 如果你想将整合包上传到 Modrinth，这能帮助你分辨哪些文件未在 Modrinth 上托管。")
                    list.Add("# 此选项与 DontCheckHostedAssets 冲突。")
                    list.Add("ModrinthUploadMode:" + Conversions.ToString(Me.CheckAdvancedModrinth.Checked))
                    list.Add("")
                    list.Add("# 导出的文件的存放位置。")
                    list.Add("# 若设置了此项，在导出时会直接将文件放到此路径，不会弹窗要求选择。")
                    list.Add("# 若 IncludeLauncher 为 True，应以 .zip 结尾；若为 False，应以 .mrpack 结尾。")
                    list.Add("PackPath:" + Me.ConfigPackPath)
                    list.Add("")
                    list.Add("==============================================================")
                    list.AddRange(Me.GetAllRules())
                    list.Add("==============================================================")
                    list.AddRange(Me.GetExtraFileLines())
                    FileUtils.Write(text, list.Join(vbCrLf), Nothing)
                    ModMain.Hint("已保存配置文件：" + text, ModMain.HintType.Green, True)
                    ModBase.OpenExplorer(text)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "保存配置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
            End Try
        End Sub

        Private Sub ImportConfig()
            Try
                Dim text As String = Dialogs.SelectFile("选择配置文件", False, Settings.[Get](Of String)("CacheExportConfig", Nothing), New OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)?(New ValueTuple(Of String(), String)() { New ValueTuple(Of String(), String)(New String() { "txt" }, "整合包导出配置") })).FirstOrDefault()
                If Not String.IsNullOrEmpty(text) Then
                    Settings.[Set]("CacheExportConfig", text, Nothing)
                    Dim array As String() = FileUtils.ReadAsString(text, Nothing, Nothing).Split("==============================================================", False)
                    Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
                    For Each text2 As String In array(0).SplitLines(True)
                        text2 = text2.Trim()
                        If Operators.CompareString(text2, "", False) <> 0 AndAlso Not text2.StartsWithF("#", False) AndAlso Not text2.StartsWithF("=", False) Then
                            Dim num As Integer = text2.IndexOfF(":", False)
                            If num > 0 Then
                                ' The following expression was wrapped in a checked-expression
                                dictionary(text2.Substring(0, num)) = text2.Substring(num + 1)
                            End If
                        End If
                    Next
                    Me.TextExportName.Text = dictionary.GetOrDefault("Name", "")
                    Me.TextExportVersion.Text = dictionary.GetOrDefault("Version", "")
                    Me.CheckOptionsJava.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("IncludeJava", Conversions.ToString(True)))
                    Me.CheckOptionsPcl.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("IncludeLauncher", Conversions.ToString(True)))
                    Me.CheckOptionsPclCustom.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("IncludeLauncherCustom", Conversions.ToString(True)))
                    Me.CheckAdvancedInclude.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("DontCheckHostedAssets", Conversions.ToString(False)))
                    If Not Me.CheckAdvancedInclude.Checked Then
                        Me.CheckAdvancedModrinth.Checked = Conversions.ToBoolean(dictionary.GetOrDefault("ModrinthUploadMode", Conversions.ToString(False)))
                    End If
                    Me.ConfigPackPath = dictionary.GetOrDefault("PackPath", Nothing)
                    Me.RulesOverrides = array(1).SplitLines(True).ToList()
                    If array.Length > 2 Then
                        Me.ExtraFiles = array(2).SplitLines(True).ToList()
                    Else
                        Me.ExtraFiles = Nothing
                    End If
                    ModMain.Hint("已读取配置文件：" + text, ModMain.HintType.Green, True)
                End If
            Catch ex As Exception
                Logger.[Error](ex, "读取配置失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
            End Try
        End Sub

        Private Sub StartExport()
            Dim CS$<>8__locals1 As PageInstanceExport._Closure$__25-0 = New PageInstanceExport._Closure$__25-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_PackName = If(String.IsNullOrEmpty(Me.TextExportName.Text), Me.TextExportName.HintText, Me.TextExportName.Text)
            CS$<>8__locals1.$VB$Local_PackVersion = If(String.IsNullOrEmpty(Me.TextExportVersion.Text), "1.0.0", Me.TextExportVersion.Text)
            CS$<>8__locals1.$VB$Local_LoaderName = "导出整合包：" + CS$<>8__locals1.$VB$Local_PackName
            Try
                Dim enumerator As IEnumerator(Of ModLoader.LoaderBase) = ModLoader.LoaderTaskbar.GetEnumerator()
                While enumerator.MoveNext()
                    If Operators.CompareString(enumerator.Current.Name, CS$<>8__locals1.$VB$Local_LoaderName, False) = 0 Then
                        ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.[Default])
                        Return
                    End If
                End While
            Finally
                Dim enumerator As IEnumerator(Of ModLoader.LoaderBase)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            CS$<>8__locals1.$VB$Local_PackPath = Nothing
            If Not String.IsNullOrWhiteSpace(Me.ConfigPackPath) AndAlso Not Me.ConfigPackPath.EndsWithF("\", False) AndAlso Not Me.ConfigPackPath.EndsWithF("/", False) Then
                Try
                    DirectoryUtils.Create(PathUtils.RemoveLastPart(Me.ConfigPackPath))
                    CS$<>8__locals1.$VB$Local_PackPath = Me.ConfigPackPath
                    Logger.Info(String.Format("使用配置文件中指定的导出路径：{0}", Me.ConfigPackPath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("无法使用配置文件中指定的导出路径（{0}）", Me.ConfigPackPath), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                    If ModMain.OurMsgBox(String.Format("指定的路径：{0}{1}{2}{3}", New Object() { Me.ConfigPackPath, vbCrLf, vbCrLf, ex.GetDisplay(True) }), "无法使用配置文件中指定的导出路径", "确定", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                        Return
                    End If
                End Try
            End If
            If CS$<>8__locals1.$VB$Local_PackPath Is Nothing Then
                Dim list As List(Of ValueTuple(Of String, String)) = New List(Of ValueTuple(Of String, String))()
                If Not Me.CheckOptionsPcl.Checked AndAlso Me.CheckAdvancedModrinth.Checked Then
                    list.Add(New ValueTuple(Of String, String)("mrpack", "Modrinth 整合包文件"))
                End If
                list.Add(New ValueTuple(Of String, String)("zip", "压缩文件"))
                If Not Me.CheckOptionsPcl.Checked AndAlso Not Me.CheckAdvancedModrinth.Checked Then
                    list.Add(New ValueTuple(Of String, String)("mrpack", "Modrinth 整合包文件"))
                End If
                CS$<>8__locals1.$VB$Local_PackPath = Dialogs.SaveFile("选择导出位置", CS$<>8__locals1.$VB$Local_PackName + If(String.IsNullOrEmpty(Me.TextExportVersion.Text), "", (" " + Me.TextExportVersion.Text)), Nothing, list)
                Logger.Info(String.Format("手动指定的导出路径：{0}", CS$<>8__locals1.$VB$Local_PackPath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
            End If
            If CS$<>8__locals1.$VB$Local_PackPath IsNot Nothing Then
                CS$<>8__locals1.$VB$Local_CacheFolder = ModMain.RequestTaskTempFolder(False)
                CS$<>8__locals1.$VB$Local_OverridesFolder = CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\overrides\"
                CS$<>8__locals1.$VB$Local_TargetInstance = PageInstanceLeft.Instance
                CS$<>8__locals1.$VB$Local_IncludeJava = (Me.RulesOverrides Is Nothing AndAlso Me.CheckOptionsJava.Visibility = Visibility.Visible AndAlso Me.CheckOptionsJava.Checked) OrElse (Me.RulesOverrides IsNot Nothing AndAlso Me.CheckOptionsJava.Checked AndAlso Settings.[Get](Of Integer)("VersionArgumentJavaV2", CS$<>8__locals1.$VB$Local_TargetInstance) = 2)
                CS$<>8__locals1.$VB$Local_PathIndie = CS$<>8__locals1.$VB$Local_TargetInstance.PathIndie
                CS$<>8__locals1.$VB$Local_CheckHostedAssets = Not Me.CheckAdvancedInclude.Checked
                CS$<>8__locals1.$VB$Local_ModrinthUploadMode = Me.CheckAdvancedModrinth.Checked
                CS$<>8__locals1.$VB$Local_IncludePCL = Me.CheckOptionsPcl.Checked
                CS$<>8__locals1.$VB$Local_IncludePCLCustom = CS$<>8__locals1.$VB$Local_IncludePCL AndAlso Me.CheckOptionsPclCustom.Checked
                CS$<>8__locals1.$VB$Local_AllRules = Me.StandardizeLines(Me.GetAllRules(), True).ToList()
                CS$<>8__locals1.$VB$Local_AllExtraFiles = Me.StandardizeLines(Me.GetExtraFileLines(), False).ToList()
                Logger.Info(String.Format("准备导出整合包，共有 {0} 条规则，{1} 条追加内容行", CS$<>8__locals1.$VB$Local_AllRules.Count, CS$<>8__locals1.$VB$Local_AllExtraFiles.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                Dim list2 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                If CS$<>8__locals1.$VB$Local_IncludePCL Then
                    list2.Add(New ModLoader.LoaderTask(Of Integer, Integer)("复制 OpenPCL 启动器", Sub(Loader As ModLoader.LoaderTask(Of Integer, Integer))
                        FileUtils.Copy(ModBase.PathExe, CS$<>8__locals1.$VB$Local_CacheFolder + "OpenPCL.exe")
                    End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.5, .Block = False })
                End If
                list2.Add(New ModLoader.LoaderTask(Of Integer, List(Of LocalResourceFile))("复制导出内容", Sub(Loader As ModLoader.LoaderTask(Of Integer, List(Of LocalResourceFile)))
                    Dim CS$<>8__locals2 As PageInstanceExport._Closure$__25-1 = New PageInstanceExport._Closure$__25-1(CS$<>8__locals2)
                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                    CS$<>8__locals2.$VB$Local_Loader = Loader
                    CS$<>8__locals2.$VB$Local_Loader.Output = New List(Of LocalResourceFile)()
                    CS$<>8__locals2.$VB$Local_Progress = 0
                    CS$<>8__locals2.$VB$Local_SearchFolder = Sub(Folder As DirectoryInfo)
                        Try
                            For Each directoryInfo As DirectoryInfo In Folder.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                                If(Not New String() { "assets", "versions", "libraries" }.Contains(directoryInfo.Name) OrElse Not PathUtils.IsSame(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_PathIndie, Folder.FullName)) AndAlso Not New String() { "structureCacheV1", ".fabric", ".git", "avatar-cache", "cosmetic-cache" }.Contains(directoryInfo.Name) Then
                                    CS$<>8__locals2.$VB$Local_SearchFolder(directoryInfo)
                                End If
                            Next
                        Finally
                            Dim enumerator3 As IEnumerator(Of DirectoryInfo)
                            If enumerator3 IsNot Nothing Then
                                enumerator3.Dispose()
                            End If
                        End Try
                        Try
                            For Each fileInfo As FileInfo In Folder.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                                Dim CS$<>8__locals3 As PageInstanceExport._Closure$__25-2 = New PageInstanceExport._Closure$__25-2(CS$<>8__locals3)
                                CS$<>8__locals3.$VB$Local_RelativePath = fileInfo.FullName.AfterFirst(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_PathIndie, False)
                                Dim flag As Boolean = False
                                Try
                                    For Each text5 As String In CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_AllRules
                                        Dim flag2 As Boolean = text5.StartsWithF("!", False)
                                        If LikeOperator.LikeString(CS$<>8__locals3.$VB$Local_RelativePath.Lower(), text5.TrimStart(New Char() { "!"c }).Lower(), CompareMethod.Binary) Then
                                            flag = Not flag2
                                        End If
                                    Next
                                Finally
                                    Dim enumerator5 As List(Of String).Enumerator
                                    CType(enumerator5, IDisposable).Dispose()
                                End Try
                                If flag Then
                                    Dim text6 As String = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_OverridesFolder + CS$<>8__locals3.$VB$Local_RelativePath
                                    FileUtils.Copy(fileInfo.FullName, text6)
                                    If CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CheckHostedAssets AndAlso New String() { ".zip", ".rar", ".jar", ".disabled", ".old" }.Contains(fileInfo.Extension.Lower()) AndAlso New String() { "mods", "packs", "openloader", "resource" }.Any(Function(s As String) CS$<>8__locals3.$VB$Local_RelativePath.Contains(s)) Then
                                        Dim localResourceFile As LocalResourceFile = New LocalResourceFile(text6)
                                        Dim modrinthHash As String = localResourceFile.ModrinthHash
                                        Conversions.ToString(localResourceFile.CurseForgeHash)
                                        CS$<>8__locals2.$VB$Local_Loader.Output.Add(localResourceFile)
                                    End If
                                    CS$<>8__locals2.$VB$Local_Progress += 1
                                    If CS$<>8__locals2.$VB$Local_Progress = 25 Then
                                        Dim $VB$Local_Loader As ModLoader.LoaderTask(Of Integer, List(Of LocalResourceFile)) = CS$<>8__locals2.$VB$Local_Loader
                                        Dim loaderTask As ModLoader.LoaderTask(Of Integer, List(Of LocalResourceFile)) = $VB$Local_Loader
                                        $VB$Local_Loader.Progress = loaderTask.Progress + (0.84 - CS$<>8__locals2.$VB$Local_Loader.Progress) * 0.012
                                        CS$<>8__locals2.$VB$Local_Progress = 0
                                    End If
                                End If
                            Next
                        Finally
                            Dim enumerator4 As IEnumerator(Of FileInfo)
                            If enumerator4 IsNot Nothing Then
                                enumerator4.Dispose()
                            End If
                        End Try
                    End Sub
                    CS$<>8__locals2.$VB$Local_SearchFolder(DirectoryUtils.GetInfo(CS$<>8__locals1.$VB$Local_PathIndie))
                    Logger.Info(String.Format("复制 overrides 文件完成，有 {0} 个文件需要联网检查", CS$<>8__locals2.$VB$Local_Loader.Output.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                    CS$<>8__locals2.$VB$Local_Loader.Progress = 0.85
                    If CS$<>8__locals1.$VB$Local_IncludeJava Then
                        Dim java As Java = ModJava.SelectOrDownloadJava(CS$<>8__locals1.$VB$Local_TargetInstance, False, CS$<>8__locals2.$VB$Local_Loader.CreateCancellationToken(), CS$<>8__locals2.$VB$Local_Loader.CreateSyncProgressProvider(0.85, 0.93))
                        If java Is Nothing Then
                            Throw New FileNotFoundException("版本文件夹中没有找到可导出的 Java。")
                        End If
                        Dim text As String = PathUtils.RemoveLastPart(java.Folder)
                        Dim text2 As String = Path.Combine(CS$<>8__locals1.$VB$Local_OverridesFolder, text.AfterFirst(CS$<>8__locals1.$VB$Local_TargetInstance.PathVersion, True))
                        DirectoryUtils.Copy(text, text2)
                        Logger.Info(String.Format("已复制版本文件夹中的 Java：{0} → {1}", text, text2), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                    End If
                    CS$<>8__locals2.$VB$Local_Loader.Progress = 0.95
                    Dim text3 As String = If(CS$<>8__locals1.$VB$Local_IncludePCL, CS$<>8__locals1.$VB$Local_CacheFolder, (CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\"))
                    Try
                        For Each text4 As String In CS$<>8__locals1.$VB$Local_AllExtraFiles
                            If text4.EndsWithF("\", False) OrElse text4.EndsWithF("/", False) Then
                                If DirectoryUtils.Exists(text4) Then
                                    DirectoryUtils.Copy(text4, text3 + PathUtils.GetLastPart(text4) + "\")
                                Else
                                    ModMain.Hint(String.Format("未找到配置文件中指定的文件夹：{0}", text4), ModMain.HintType.Red, True)
                                End If
                            ElseIf FileUtils.Exists(text4) Then
                                FileUtils.Copy(text4, text3 + PathUtils.GetLastPart(text4))
                            Else
                                ModMain.Hint(String.Format("未找到配置文件中指定的单个文件：{0}", text4), ModMain.HintType.Red, True)
                            End If
                        Next
                    Finally
                        Dim enumerator2 As List(Of String).Enumerator
                        CType(enumerator2, IDisposable).Dispose()
                    End Try
                    CS$<>8__locals2.$VB$Local_Loader.Progress = 0.97
                    If DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_TargetInstance.PathVersion + "PCL\") Then
                        DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_TargetInstance.PathVersion + "PCL\", CS$<>8__locals1.$VB$Local_OverridesFolder + "PCL\")
                    End If
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_OverridesFolder + "PCL\Setup.ini", "IsStar", Conversions.ToString(False))
                    If CS$<>8__locals1.$VB$Local_IncludePCLCustom Then
                        If DirectoryUtils.Exists(Paths.Base + "PCL\Pictures\") Then
                            DirectoryUtils.Copy(Paths.Base + "PCL\Pictures\", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\Pictures\")
                        End If
                        If DirectoryUtils.Exists(Paths.Base + "PCL\Musics\") Then
                            DirectoryUtils.Copy(Paths.Base + "PCL\Musics\", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\Musics\")
                        End If
                        If DirectoryUtils.Exists(Paths.Base + "PCL\Help\") Then
                            DirectoryUtils.Copy(Paths.Base + "PCL\Help\", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\Help\")
                        End If
                        If FileUtils.Exists(Paths.Base + "PCL\Custom.xaml") Then
                            FileUtils.Copy(Paths.Base + "PCL\Custom.xaml", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\Custom.xaml")
                        End If
                        If FileUtils.Exists(Paths.Base + "PCL\Setup.ini") Then
                            FileUtils.Copy(Paths.Base + "PCL\Setup.ini", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\Setup.ini")
                        End If
                        If FileUtils.Exists(Paths.Base + "PCL\hints.txt") Then
                            FileUtils.Copy(Paths.Base + "PCL\hints.txt", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\hints.txt")
                        End If
                        If FileUtils.Exists(Paths.Base + "PCL\Logo.png") Then
                            FileUtils.Copy(Paths.Base + "PCL\Logo.png", CS$<>8__locals1.$VB$Local_CacheFolder + "PCL\Logo.png")
                        End If
                    End If
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 5.0 })
                list2.Add(New ModLoader.LoaderTask(Of List(Of LocalResourceFile), Dictionary(Of LocalResourceFile, List(Of String)))("联网获取文件信息", Sub(Loader As ModLoader.LoaderTask(Of List(Of LocalResourceFile), Dictionary(Of LocalResourceFile, List(Of String))))
                    Dim CS$<>8__locals4 As PageInstanceExport._Closure$__25-3 = New PageInstanceExport._Closure$__25-3(CS$<>8__locals4)
                    CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals1
                    CS$<>8__locals4.$VB$Local_Loader = Loader
                    CS$<>8__locals4.$VB$Local_Loader.Output = New Dictionary(Of LocalResourceFile, List(Of String))()
                    If Not CS$<>8__locals1.$VB$Local_CheckHostedAssets Then
                        Logger.Info("要求跳过联网获取步骤", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                        Return
                    End If
                    If Not CS$<>8__locals4.$VB$Local_Loader.Input.Any() Then
                        Logger.Info("没有需要联网检查的文件，跳过联网获取步骤", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                        Return
                    End If
                    CS$<>8__locals4.$VB$Local_EndedThreadCount = 0
                    CS$<>8__locals4.$VB$Local_FailedExceptions = New List(Of Exception)()
                    ModBase.RunInNewThread(Sub()
                        Try
                            Dim input As IEnumerable(Of LocalResourceFile) = CS$<>8__locals4.$VB$Local_Loader.Input
                            Dim func As Func(Of LocalResourceFile, String)
                            If PageInstanceExport._Closure$__.$I25-6 IsNot Nothing Then
                                func = PageInstanceExport._Closure$__.$I25-6
                            Else
                                Dim func2 As Func(Of LocalResourceFile, String) = Function(m As LocalResourceFile) m.ModrinthHash
                                func = func2
                                PageInstanceExport._Closure$__.$I25-6 = func2
                            End If
                            Dim enumerable As IEnumerable(Of String) = input.[Select](func)
                            Dim jobject As JObject = CType(ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files", HttpMethod.Post, String.Format("{{""hashes"": [""{0}""], ""algorithm"": ""sha1""}}", enumerable.Join(""",""")), "application/json"), JObject)
                            Try
                                For Each localResourceFile2 As LocalResourceFile In CS$<>8__locals4.$VB$Local_Loader.Input
                                    If jobject.ContainsKey(localResourceFile2.ModrinthHash) Then
                                        Dim jtoken As JToken = jobject(localResourceFile2.ModrinthHash)
                                        Dim jtoken2 As JToken
                                        If jtoken Is Nothing Then
                                            jtoken2 = Nothing
                                        Else
                                            Dim jtoken3 As JToken = jtoken("files")
                                            If jtoken3 Is Nothing Then
                                                jtoken2 = Nothing
                                            Else
                                                Dim jtoken4 As JToken = jtoken3(0)("hashes")
                                                jtoken2 = If((jtoken4 IsNot Nothing), jtoken4("sha1"), Nothing)
                                            End If
                                        End If
                                        If Not(CStr(jtoken2) <> localResourceFile2.ModrinthHash) Then
                                            CS$<>8__locals4.$VB$Local_Loader.Output.AddIntoValueCollection(localResourceFile2, CStr(jobject(localResourceFile2.ModrinthHash)("files")(0)("url")))
                                        End If
                                    End If
                                Next
                            Finally
                                Dim enumerator6 As List(Of LocalResourceFile).Enumerator
                                CType(enumerator6, IDisposable).Dispose()
                            End Try
                            Logger.Info(String.Format("从 Modrinth 获取到 {0} 个本地资源项的对应信息", jobject.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                        Catch ex2 As Exception
                            Logger.Warn(ex2, "从 Modrinth 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                            CS$<>8__locals4.$VB$Local_FailedExceptions.Add(ex2)
                        Finally
                            Dim $VB$Local_Loader2 As ModLoader.LoaderTask(Of List(Of LocalResourceFile), Dictionary(Of LocalResourceFile, List(Of String)))
                            Dim loaderTask2 As ModLoader.LoaderTask(Of List(Of LocalResourceFile), Dictionary(Of LocalResourceFile, List(Of String)))
                            CS$<>8__locals4.$VB$Local_EndedThreadCount += 1
                            $VB$Local_Loader2 = CS$<>8__locals4.$VB$Local_Loader
                            loaderTask2 = $VB$Local_Loader2
                            $VB$Local_Loader2.Progress = loaderTask2.Progress + 0.45
                        End Try
                    End Sub, "Modrinth - " + CS$<>8__locals1.$VB$Local_LoaderName, ThreadPriority.Normal)
                    ModBase.RunInNewThread(Sub()
                        Try
                            If Not CS$<>8__locals4.$VB$NonLocal_$VB$Closure_3.$VB$Local_ModrinthUploadMode Then
                                Dim input2 As IEnumerable(Of LocalResourceFile) = CS$<>8__locals4.$VB$Local_Loader.Input
                                Dim func3 As Func(Of LocalResourceFile, UInteger)
                                If PageInstanceExport._Closure$__.$I25-8 IsNot Nothing Then
                                    func3 = PageInstanceExport._Closure$__.$I25-8
                                Else
                                    Dim func4 As Func(Of LocalResourceFile, UInteger) = Function(m As LocalResourceFile) m.CurseForgeHash
                                    func3 = func4
                                    PageInstanceExport._Closure$__.$I25-8 = func4
                                End If
                                Dim enumerable2 As IEnumerable(Of UInteger) = input2.[Select](func3)
                                Dim jcontainer As JContainer = CType(ModDownload.DlModRequest("https://api.curseforge.com/v1/fingerprints/432", HttpMethod.Post, String.Format("{{""fingerprints"": [{0}]}}", enumerable2.Join(",")), "application/json")("data")("exactMatches"), JContainer)
                                Try
                                    For Each jtoken5 As JToken In jcontainer
                                        Dim jobject2 As JObject = CType(jtoken5, JObject)
                                        Dim CS$<>8__locals5 As PageInstanceExport._Closure$__25-4 = New PageInstanceExport._Closure$__25-4(CS$<>8__locals5)
                                        If jobject2.ContainsKey("file") Then
                                            CS$<>8__locals5.$VB$Local_File = CType(jobject2("file"), JObject)
                                            If Not String.IsNullOrEmpty(CStr(CS$<>8__locals5.$VB$Local_File("downloadUrl"))) Then
                                                Dim localResourceFile3 As LocalResourceFile = CS$<>8__locals4.$VB$Local_Loader.Input.FirstOrDefault(Function(m As LocalResourceFile) m.CurseForgeHash = CS$<>8__locals5.$VB$Local_File("fileFingerprint").ToObject(Of UInteger)())
                                                If localResourceFile3 IsNot Nothing Then
                                                    Try
                                                        For Each text7 As String In ResourceVersion.ParseCurseForgeDownloadUrls(CS$<>8__locals5.$VB$Local_File("downloadUrl").ToString())
                                                            CS$<>8__locals4.$VB$Local_Loader.Output.AddIntoValueCollection(localResourceFile3, text7)
                                                        Next
                                                    Finally
                                                        Dim enumerator8 As List(Of String).Enumerator
                                                        CType(enumerator8, IDisposable).Dispose()
                                                    End Try
                                                End If
                                            End If
                                        End If
                                    Next
                                Finally
                                    Dim enumerator7 As IEnumerator(Of JToken)
                                    If enumerator7 IsNot Nothing Then
                                        enumerator7.Dispose()
                                    End If
                                End Try
                                Logger.Info(String.Format("从 CurseForge 获取到 {0} 个本地资源项的对应信息", jcontainer.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                            End If
                        Catch ex3 As Exception
                            Logger.Warn(ex3, "从 CurseForge 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageInstance\PageInstanceExport.xaml.vb")
                            CS$<>8__locals4.$VB$Local_FailedExceptions.Add(ex3)
                        Finally
                            Dim $VB$Local_Loader3 As ModLoader.LoaderTask(Of List(Of LocalResourceFile), Dictionary(Of LocalResourceFile, List(Of String)))
                            Dim loaderTask3 As ModLoader.LoaderTask(Of List(Of LocalResourceFile), Dictionary(Of LocalResourceFile, List(Of String)))
                            CS$<>8__locals4.$VB$Local_EndedThreadCount += 1
                            $VB$Local_Loader3 = CS$<>8__locals4.$VB$Local_Loader
                            loaderTask3 = $VB$Local_Loader3
                            $VB$Local_Loader3.Progress = loaderTask3.Progress + 0.45
                        End Try
                    End Sub, "CurseForge - " + CS$<>8__locals1.$VB$Local_LoaderName, ThreadPriority.Normal)
                    While CS$<>8__locals4.$VB$Local_EndedThreadCount <> 2
                        If CS$<>8__locals4.$VB$Local_Loader.IsCanceled Then
                            Return
                        End If
                        Thread.Sleep(10)
                    End While
                    If CS$<>8__locals4.$VB$Local_FailedExceptions.Count = 1 Then
                        If ModMain.OurMsgBox("联网获取部分文件信息失败，是否继续导出？" & vbCrLf & vbCrLf & "若继续，无法获取信息的文件将被直接打包。" & vbCrLf & "由于二次分发可能违反使用协议，请尽量不要公开发布导出的整合包！", "部分文件信息获取失败", "继续", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                            Throw CS$<>8__locals4.$VB$Local_FailedExceptions.First()
                        End If
                        Return
                    Else
                        If CS$<>8__locals4.$VB$Local_FailedExceptions.Count > 1 AndAlso ModMain.OurMsgBox("联网获取文件信息失败，是否继续导出？" & vbCrLf & vbCrLf & "若继续，所有文件都将被直接打包。" & vbCrLf & "由于二次分发可能违反使用协议，请尽量不要公开发布导出的整合包！", "文件信息获取失败", "继续", "取消", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                            Throw CS$<>8__locals4.$VB$Local_FailedExceptions.First()
                        End If
                        Return
                    End If
                End Sub, Nothing, ThreadPriority.Normal) With { .Show = CS$<>8__locals1.$VB$Local_CheckHostedAssets, .ProgressWeight = If(CS$<>8__locals1.$VB$Local_CheckHostedAssets, 2.0, 0.01) })
                list2.Add(New ModLoader.LoaderTask(Of Dictionary(Of LocalResourceFile, List(Of String)), Integer)("生成压缩包", Sub(Loader As ModLoader.LoaderTask(Of Dictionary(Of LocalResourceFile, List(Of String)), Integer))
                    Dim jarray As JArray = New JArray()
                    Try
                        For Each keyValuePair As KeyValuePair(Of LocalResourceFile, List(Of String)) In Loader.Input
                            Dim key As LocalResourceFile = keyValuePair.Key
                            Dim jarray2 As JArray = jarray
                            Dim jobject3 As JObject = New JObject()
                            jobject3.Add("path", PathUtils.RemoveExtendedPrefix(key.File.FullName).AfterFirst(CS$<>8__locals1.$VB$Local_OverridesFolder, False).Replace("\", "/"))
                            Dim text8 As String = "hashes"
                            Dim jobject4 As JObject = New JObject()
                            jobject4.Add("sha1", key.ModrinthHash)
                            jobject4.Add("sha512", CryptographyUtils.ComputeFileHash(key.File.FullName, CryptographyUtils.HashMethod.Sha512))
                            jobject3.Add(text8, jobject4)
                            Dim text9 As String = "downloads"
                            Dim value As IEnumerable(Of String) = keyValuePair.Value
                            Dim func5 As Func(Of String, Boolean)
                            If PageInstanceExport._Closure$__.$I25-11 IsNot Nothing Then
                                func5 = PageInstanceExport._Closure$__.$I25-11
                            Else
                                Dim func6 As Func(Of String, Boolean) = Function(u As String) u.Contains("modrinth.com")
                                func5 = func6
                                PageInstanceExport._Closure$__.$I25-11 = func6
                            End If
                            jobject3.Add(text9, New JArray(value.OrderBy(func5)))
                            jobject3.Add("fileSize", key.File.Length)
                            jarray2.Add(jobject3)
                            key.File.Delete()
                        Next
                    Finally
                        Dim enumerator9 As Dictionary(Of LocalResourceFile, List(Of String)).Enumerator
                        CType(enumerator9, IDisposable).Dispose()
                    End Try
                    Loader.Progress = 0.2
                    Dim jobject5 As JObject = New JObject()
                    jobject5.Add("minecraft", CS$<>8__locals1.$VB$Local_TargetInstance.Version.VanillaName)
                    Dim jobject6 As JObject = jobject5
                    If CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) Then
                        jobject6.Add("forge", CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaderVersions(ModLoaders.Forge))
                    End If
                    If CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) Then
                        jobject6.Add("fabric-loader", CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaderVersions(ModLoaders.Fabric))
                    End If
                    If CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) Then
                        jobject6.Add("neoforge", CS$<>8__locals1.$VB$Local_TargetInstance.Version.ModLoaderVersions(ModLoaders.NeoForge))
                    End If
                    Dim jobject7 As JObject = New JObject()
                    jobject7.Add("game", "minecraft")
                    jobject7.Add("formatVersion", 1)
                    jobject7.Add("versionId", CS$<>8__locals1.$VB$Local_PackVersion)
                    jobject7.Add("name", CS$<>8__locals1.$VB$Local_PackName)
                    jobject7.Add("summary", CS$<>8__locals1.$VB$Local_TargetInstance.Info)
                    jobject7.Add("files", jarray)
                    jobject7.Add("dependencies", jobject6)
                    Dim jobject8 As JObject = jobject7
                    FileUtils.Write(CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\modrinth.index.json", jobject8.ToString(1, New JsonConverter(-1) {}), Nothing)
                    If CS$<>8__locals1.$VB$Local_IncludePCL Then
                        FileUtils.CreateZipFromDirectory(CS$<>8__locals1.$VB$Local_CacheFolder + "modpack.mrpack", CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\")
                        Loader.Progress = 0.5
                        DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\", False)
                        Loader.Progress = 0.6
                        FileUtils.CreateZipFromDirectory(CS$<>8__locals1.$VB$Local_PackPath, CS$<>8__locals1.$VB$Local_CacheFolder)
                        Loader.Progress = 0.9
                    Else
                        FileUtils.CreateZipFromDirectory(CS$<>8__locals1.$VB$Local_PackPath, CS$<>8__locals1.$VB$Local_CacheFolder + "modpack\")
                        Loader.Progress = 0.8
                    End If
                    ModBase.OpenExplorer(CS$<>8__locals1.$VB$Local_PackPath)
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 6.0 })
                Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(CS$<>8__locals1.$VB$Local_LoaderName, list2)
                loaderCombo.OnStateChanged = AddressOf ModDownloadLib.LoaderStateChangedHintOnly
                loaderCombo.Start(Nothing, False)
                ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo)
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                ModMain.FrmMain.BtnExtraDownload.Ribble()
                ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.[Default])
            End If
        End Sub

        Private Sub RefreshJavaInfo()
            Dim num As Integer = Settings.[Get](Of Integer)("VersionArgumentJavaV2", PageInstanceLeft.Instance)
            Dim currentJavaWorker As RedoableWorker(Of Java) = PageInstanceLeft.CurrentJavaWorker
            Dim java As Java = If((num = 2 AndAlso currentJavaWorker.LastSucceeded AndAlso Not currentJavaWorker.Running), currentJavaWorker.LastResult, Nothing)
            If java Is Nothing Then
                Me.CheckOptionsJava.Visibility = Visibility.Collapsed
            Else
                Me.CheckOptionsJava.Visibility = Visibility.Visible
                Me.CheckOptionsJava.Inlines.Clear()
                Me.CheckOptionsJava.Inlines.Add(New Run(Me.GetExportOption(Me.CheckOptionsJava).Title))
                Me.CheckOptionsJava.Inlines.Add(New Run("   " + String.Format("Java {0} ({1})：{2}", java.Version.Major, java.Version, PathUtils.AddSlashSuffix(PathUtils.RemoveLastPart(java.Folder.Replace(PathUtils.RemoveLastPart(PathUtils.RemoveLastPart(PageInstanceLeft.Instance.PathVersion)), ""))).TrimStart(New Char() { "\"c }))) With { .Foreground = ModSecret.ColorGray5 })
            End If
            If num = 3 Then
                Me.BtnExport.IsEnabled = True
                Me.HintJava.Visibility = Visibility.Visible
                Me.HintJava.Theme = OurHint.Themes.Yellow
                Me.HintJava.Text = String.Format("该版本设置要求 {0}使用指定的 Java{1}，这在导出后很可能会失效。建议修改为 {2}使用版本文件夹中的 Java{3}。", New Object() { ModBase.vbLQ, ModBase.vbRQ, ModBase.vbLQ, ModBase.vbRQ })
                Return
            End If
            If num = 2 AndAlso currentJavaWorker.HasSucceeded AndAlso currentJavaWorker.LastResult Is Nothing Then
                Me.BtnExport.IsEnabled = False
                Me.HintJava.Visibility = Visibility.Visible
                Me.HintJava.Theme = OurHint.Themes.Red
                Me.HintJava.Text = String.Format("该版本设置要求 {0}使用版本文件夹中的 Java{1}，但版本文件夹中没能找到任何 Java。", ModBase.vbLQ, ModBase.vbRQ)
                Return
            End If
            Me.BtnExport.IsEnabled = True
            Me.HintJava.Visibility = Visibility.Collapsed
        End Sub

        Private Sub TextExportName_GotFocus()
            If Operators.CompareString(Me.TextExportName.Text, "", False) = 0 Then
                Me.TextExportName.Text = Me.TextExportName.HintText
                Me.TextExportName.SelectionStart = Me.TextExportName.Text.Length
            End If
        End Sub

        Private Sub CheckAdvancedInclude_Change(sender As Object, user As Boolean)
            If Me.CheckAdvancedInclude.Checked Then
                Me.CheckAdvancedModrinth.Checked = False
            End If
            Me.CheckAdvancedModrinth.IsEnabled = Not Me.CheckAdvancedInclude.Checked
        End Sub

        Private Sub CheckAdvancedModrinth_Change(sender As Object, user As Boolean)
            Me.HintModrinthFormat.Visibility = (Me.CheckAdvancedModrinth.Checked AndAlso Me.CheckOptionsPcl.Checked).ToVisibility()
        End Sub

        Friend Overridable Property PanAllBack As Grid

        Friend Overridable Property PanBack As OurScrollViewer

        Friend Overridable Property HintOptiFine As OurHint

        Friend Overridable Property HintJava As OurHint

        Friend Overridable Property TextExportName As OurTextBox
            <CompilerGenerated()>
            Get
                Return Me._TextExportName
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurTextBox)
                Dim routedEventHandler As RoutedEventHandler = Sub(a0 As Object, a1 As RoutedEventArgs)
                    Me.TextExportName_GotFocus()
                End Sub
                Dim myTextBox As OurTextBox = Me._TextExportName
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.GotFocus, routedEventHandler
                End If
                Me._TextExportName = value
                myTextBox = Me._TextExportName
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.GotFocus, routedEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property TextExportVersion As OurTextBox

        Friend Overridable Property CardOptions As OurCard
            <CompilerGenerated()>
            Get
                Return Me._CardOptions
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCard)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.CardOptions_MouseLeftButtonDown()
                End Sub
                Dim myCard As OurCard = Me._CardOptions
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.MouseLeftButtonDown, mouseButtonEventHandler
                End If
                Me._CardOptions = value
                myCard = Me._CardOptions
                If myCard IsNot Nothing Then
                    AddHandler myCard.MouseLeftButtonDown, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnOverrideCancel As OurIconTextButton

        Friend Overridable Property PanOptions As StackPanel

        Friend Overridable Property CheckOptionsBasic As OurCheckBox

        Friend Overridable Property CheckOptionsOptions As OurCheckBox

        Friend Overridable Property CheckOptionsMod As OurCheckBox

        Friend Overridable Property CheckOptionsResourcePacks As OurCheckBox

        Friend Overridable Property PanOptionsResourcePacks As StackPanel

        Friend Overridable Property CheckOptionsShaderPacks As OurCheckBox

        Friend Overridable Property CheckOptionsShaderPackSettings As OurCheckBox

        Friend Overridable Property PanOptionsShaderPacks As StackPanel

        Friend Overridable Property CheckOptionsSaves As OurCheckBox

        Friend Overridable Property PanOptionsSaves As StackPanel

        Friend Overridable Property CheckOptionsJava As OurCheckBox

        Friend Overridable Property CheckOptionsPcl As OurCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckOptionsPcl
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCheckBox)
                Dim changeEventHandler As OurCheckBox.ChangeEventHandler = AddressOf Me.CheckAdvancedModrinth_Change
                Dim myCheckBox As OurCheckBox = Me._CheckOptionsPcl
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckOptionsPcl = value
                myCheckBox = Me._CheckOptionsPcl
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckOptionsPclCustom As OurCheckBox

        Friend Overridable Property CheckAdvancedInclude As OurCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckAdvancedInclude
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCheckBox)
                Dim changeEventHandler As OurCheckBox.ChangeEventHandler = AddressOf Me.CheckAdvancedInclude_Change
                Dim myCheckBox As OurCheckBox = Me._CheckAdvancedInclude
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckAdvancedInclude = value
                myCheckBox = Me._CheckAdvancedInclude
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property CheckAdvancedModrinth As OurCheckBox
            <CompilerGenerated()>
            Get
                Return Me._CheckAdvancedModrinth
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurCheckBox)
                Dim changeEventHandler As OurCheckBox.ChangeEventHandler = AddressOf Me.CheckAdvancedModrinth_Change
                Dim myCheckBox As OurCheckBox = Me._CheckAdvancedModrinth
                If myCheckBox IsNot Nothing Then
                    RemoveHandler myCheckBox.Change, changeEventHandler
                End If
                Me._CheckAdvancedModrinth = value
                myCheckBox = Me._CheckAdvancedModrinth
                If myCheckBox IsNot Nothing Then
                    AddHandler myCheckBox.Change, changeEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property HintModrinthFormat As OurHint

        Friend Overridable Property BtnAdvancedImport As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnAdvancedImport
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.ImportConfig()
                End Sub
                Dim myButton As OurButton = Me._BtnAdvancedImport
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnAdvancedImport = value
                myButton = Me._BtnAdvancedImport
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnAdvancedExport As OurButton
            <CompilerGenerated()>
            Get
                Return Me._BtnAdvancedExport
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurButton)
                Dim clickEventHandler As OurButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.ExportConfig()
                End Sub
                Dim myButton As OurButton = Me._BtnAdvancedExport
                If myButton IsNot Nothing Then
                    RemoveHandler myButton.Click, clickEventHandler
                End If
                Me._BtnAdvancedExport = value
                myButton = Me._BtnAdvancedExport
                If myButton IsNot Nothing Then
                    AddHandler myButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overridable Property BtnAdvancedHelp As OurButton

        Friend Overridable Property BtnExport As OurExtraTextButton
            <CompilerGenerated()>
            Get
                Return Me._BtnExport
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As OurExtraTextButton)
                Dim clickEventHandler As OurExtraTextButton.ClickEventHandler = Sub(a0 As Object, a1 As MouseButtonEventArgs)
                    Me.StartExport()
                End Sub
                Dim myExtraTextButton As OurExtraTextButton = Me._BtnExport
                If myExtraTextButton IsNot Nothing Then
                    RemoveHandler myExtraTextButton.Click, clickEventHandler
                End If
                Me._BtnExport = value
                myExtraTextButton = Me._BtnExport
                If myExtraTextButton IsNot Nothing Then
                    AddHandler myExtraTextButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private CurrentInstance As String

        Private SubOptionBlackList As String()

        Private Const Sperator As String = "=============================================================="

        Private _RulesOverrides As List(Of String)

        Private ExtraFiles As List(Of String)

        Private ConfigPackPath As String
    End Class
End Namespace
