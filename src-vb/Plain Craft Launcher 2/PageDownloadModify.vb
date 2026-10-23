Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public Class PageDownloadModify
        Inherits PageDownloadInstall

        Public Sub New()
            AddHandler MyBase.Unloaded, Sub(a0 As Object, a1 As RoutedEventArgs)
                Me.PageUnloaded()
            End Sub
            Me.PendingComponents = New Dictionary(Of MyLoading, String)()
        End Sub

        Public Overrides Sub LoaderInit()
            MyBase.ControlInit()
            Me.InitLoader = New ModLoader.LoaderTask(Of ModMinecraft.McInstance, Object)("初始化版本修改", AddressOf Me.LoadInstance, Nothing, ThreadPriority.Normal)
            Dim loadMinecraft As MyLoading = Me.LoadMinecraft
            Dim panLoad As FrameworkElement = Me.PanLoad
            Dim panAllBack As FrameworkElement = Me.PanAllBack
            Dim frameworkElement As FrameworkElement = Nothing
            Dim initLoader As ModLoader.LoaderTask(Of ModMinecraft.McInstance, Object) = Me.InitLoader
            Dim action As Action(Of ModLoader.LoaderTask(Of ModMinecraft.McInstance, Object)) = AddressOf Me.LoadInstanceFinished
            Dim func As Func(Of ModMinecraft.McInstance)
            If PageDownloadModify._Closure$__.$I9-0 IsNot Nothing Then
                func = PageDownloadModify._Closure$__.$I9-0
            Else
                Dim func2 As Func(Of ModMinecraft.McInstance) = Function() PageInstanceLeft.Instance
                func = func2
                PageDownloadModify._Closure$__.$I9-0 = func2
            End If
            MyBase.PageLoaderInit(Of ModMinecraft.McInstance, Object)(loadMinecraft, panLoad, panAllBack, frameworkElement, initLoader, action, func, True)
        End Sub

        Public Overrides Sub Init()
            Me.PanBack.ScrollToHome()
            If Me.$STATIC$Init$2001$IsLoad$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$Init$2001$IsLoad$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$Init$2001$IsLoad$Init, flag)
                If Me.$STATIC$Init$2001$IsLoad$Init.State = 0S Then
                    Me.$STATIC$Init$2001$IsLoad$Init.State = 2S
                    Me.$STATIC$Init$2001$IsLoad = False
                ElseIf Me.$STATIC$Init$2001$IsLoad$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$Init$2001$IsLoad$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$Init$2001$IsLoad$Init)
                End If
            End Try
            If Me.$STATIC$Init$2001$IsLoad Then
                MyBase.PageLoaderRestart(Nothing, True)
            End If
            Me.$STATIC$Init$2001$IsLoad = True
        End Sub

        Private Sub PageUnloaded()
            Me.InitLoader.Cancel()
        End Sub

        Private Sub LoadInstance(Task As ModLoader.LoaderTask(Of ModMinecraft.McInstance, Object))
            Dim CS$<>8__locals1 As PageDownloadModify._Closure$__12-0 = New PageDownloadModify._Closure$__12-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_Task = Task
            ModBase.RunInUiWait(Sub()
                ModAnimation.AniStop("FrmDownloadInstall SelectPageSwitch")
                Me.IsSelectNameEdited = False
                MyBase.ClearSelected()
                Me.PendingComponents.Clear()
                Me.InitialTarget = Nothing
                Me.IsInSelectPage = False
                Me.PanAllBack.IsHitTestVisible = False
            End Sub)
            CS$<>8__locals1.$VB$Local_Task.Input.Config.DiscardCache()
            Dim mcInstallManifest As McInstallManifest = Configs.McInstallManifest.[Get](CS$<>8__locals1.$VB$Local_Task.Input.Config)
            If mcInstallManifest IsNot Nothing AndAlso mcInstallManifest.SchemaVersion > 1 Then
                Logger.Warn(String.Format("安装清单由更新版本的 PCL 创建（清单版本：{0}，当前版本：{1}）", mcInstallManifest.SchemaVersion, 1), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\PageDownloadModify.vb")
                Throw New Exception("更新 PCL 后才能修改它！点击【设置 → 其他 → 检查更新】以更新。")
            End If
            If mcInstallManifest IsNot Nothing Then
                Dim mcInstallManifest2 As McInstallManifest = mcInstallManifest.RefreshModComponents(CS$<>8__locals1.$VB$Local_Task.Input.PathIndie)
                If mcInstallManifest2 IsNot mcInstallManifest Then
                    mcInstallManifest = mcInstallManifest2
                    Configs.McInstallManifest.[Set](mcInstallManifest, CS$<>8__locals1.$VB$Local_Task.Input.Config)
                    CS$<>8__locals1.$VB$Local_Task.Input.Config.Save()
                End If
            End If
            CS$<>8__locals1.$VB$Local_OptiFineVersion = Nothing
            If mcInstallManifest IsNot Nothing Then
                CS$<>8__locals1.$VB$Local_Components = mcInstallManifest.Components
                If CS$<>8__locals1.$VB$Local_Components Is Nothing Then
                    Throw New Exception("安装清单缺少组件信息，无法还原已安装组件。")
                End If
                Me.IsVanillaVersionReliable = True
            Else
                Dim version As McVersion = New ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_Task.Input.PathVersion).Version
                If Operators.CompareString(version.VanillaName, "Unknown", False) = 0 Then
                    Throw New Exception("无法确定 Minecraft 版本，无法修改")
                End If
                Dim modLoaderVersions As IEnumerable(Of KeyValuePair(Of ModLoaders, String)) = version.ModLoaderVersions
                Dim func As Func(Of KeyValuePair(Of ModLoaders, String), Boolean)
                If PageDownloadModify._Closure$__.$I12-1 IsNot Nothing Then
                    func = PageDownloadModify._Closure$__.$I12-1
                Else
                    Dim func2 As Func(Of KeyValuePair(Of ModLoaders, String), Boolean) = Function(Loader As KeyValuePair(Of ModLoaders, String)) String.IsNullOrWhiteSpace(Loader.Value) OrElse Operators.CompareString(Loader.Value, "未知版本", False) = 0
                    func = func2
                    PageDownloadModify._Closure$__.$I12-1 = func2
                End If
                Dim enumerable As IEnumerable(Of KeyValuePair(Of ModLoaders, String)) = modLoaderVersions.Where(func)
                Dim func3 As Func(Of KeyValuePair(Of ModLoaders, String), String)
                If PageDownloadModify._Closure$__.$I12-2 IsNot Nothing Then
                    func3 = PageDownloadModify._Closure$__.$I12-2
                Else
                    Dim func4 As Func(Of KeyValuePair(Of ModLoaders, String), String) = Function(Loader As KeyValuePair(Of ModLoaders, String)) Loader.Key.ToString()
                    func3 = func4
                    PageDownloadModify._Closure$__.$I12-2 = func4
                End If
                Dim list As List(Of String) = enumerable.[Select](func3).ToList()
                If list.Any() Then
                    Throw New Exception(String.Format("无法确定 {0} 的版本，无法修改", list.Join("、")))
                End If
                CS$<>8__locals1.$VB$Local_Components = New McComponents(version.VanillaName, version.ModLoaderVersions, Nothing, Nothing, Nothing)
                CS$<>8__locals1.$VB$Local_OptiFineVersion = version.OptiFine
                Me.IsVanillaVersionReliable = version.Reliable
            End If
            CS$<>8__locals1.$VB$Local_MinecraftVersion = CS$<>8__locals1.$VB$Local_Components.MinecraftVersion
            If String.IsNullOrWhiteSpace(CS$<>8__locals1.$VB$Local_MinecraftVersion) Then
                Throw New Exception("无法确定当前 Minecraft 版本，请检查版本文件")
            End If
            Dim dictionary As Dictionary(Of String, ModLoader.LoaderBase) = New Dictionary(Of String, ModLoader.LoaderBase)() From { { "Minecraft", ModDownload.DlClientListLoader }, { "OptiFine", ModDownload.DlOptiFineListLoader }, { "LiteLoader", ModDownload.DlLiteLoaderListLoader }, { "NeoForge", ModDownload.DlNeoForgeListLoader }, { "Fabric", ModDownload.DlFabricListLoader }, { "Fabric API", ModDownload.DlFabricApiLoader }, { "OptiFabric", ModDownload.DlOptiFabricLoader } }
            Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)) = Nothing
            If McVersion.IsStandardFormat(CS$<>8__locals1.$VB$Local_MinecraftVersion) Then
                loaderTask = New ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))("DlForgeVersion " + CS$<>8__locals1.$VB$Local_MinecraftVersion, AddressOf ModDownload.DlForgeVersionMain, Nothing, ThreadPriority.Normal)
                dictionary.Add("Forge", loaderTask)
            End If
            Try
                For Each loaderBase As ModLoader.LoaderBase In dictionary.Values
                    loaderBase.Start(If((loaderBase Is loaderTask), CS$<>8__locals1.$VB$Local_MinecraftVersion, Nothing), CS$<>8__locals1.$VB$Local_Task.IsForceRestarting)
                Next
            Finally
                Dim enumerator As Dictionary(Of String, ModLoader.LoaderBase).ValueCollection.Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            While Not CS$<>8__locals1.$VB$Local_Task.IsCanceled
                Try
                    For Each keyValuePair As KeyValuePair(Of String, ModLoader.LoaderBase) In dictionary
                        Dim value As ModLoader.LoaderBase = keyValuePair.Value
                        If value.State = ModBase.LoadState.Failed Then
                            If value IsNot loaderTask OrElse Operators.CompareString(value.[Error].Message, "无", False) <> 0 Then
                                Throw New Exception(String.Format("获取 {0} 版本列表失败，请稍后再试，或使用 VPN 改善网络环境", keyValuePair.Key), value.[Error])
                            End If
                        ElseIf value.State = ModBase.LoadState.Canceled Then
                            Throw New Exception(String.Format("获取 {0} 版本列表被中断，请稍后再试，或使用 VPN 改善网络环境", keyValuePair.Key))
                        End If
                    Next
                Finally
                    Dim enumerator2 As Dictionary(Of String, ModLoader.LoaderBase).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                Dim values As IEnumerable(Of ModLoader.LoaderBase) = dictionary.Values
                Dim func5 As Func(Of ModLoader.LoaderBase, Boolean)
                If PageDownloadModify._Closure$__.$I12-3 IsNot Nothing Then
                    func5 = PageDownloadModify._Closure$__.$I12-3
                Else
                    Dim func6 As Func(Of ModLoader.LoaderBase, Boolean) = Function(Loader As ModLoader.LoaderBase) Loader.State = ModBase.LoadState.Finished OrElse Loader.State = ModBase.LoadState.Failed
                    func5 = func6
                    PageDownloadModify._Closure$__.$I12-3 = func6
                End If
                If Not values.All(func5) Then
                    Dim $VB$Local_Task As ModLoader.LoaderBase = CS$<>8__locals1.$VB$Local_Task
                    Dim num As Double = 0.7
                    Dim values2 As IEnumerable(Of ModLoader.LoaderBase) = dictionary.Values
                    Dim func7 As Func(Of ModLoader.LoaderBase, Double)
                    If PageDownloadModify._Closure$__.$I12-4 IsNot Nothing Then
                        func7 = PageDownloadModify._Closure$__.$I12-4
                    Else
                        Dim func8 As Func(Of ModLoader.LoaderBase, Double) = Function(Loader As ModLoader.LoaderBase) Loader.Progress
                        func7 = func8
                        PageDownloadModify._Closure$__.$I12-4 = func8
                    End If
                    $VB$Local_Task.Progress = num * values2.Average(func7)
                    Thread.Sleep(10)
                Else
                    If CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                        Exit While
                    End If
                    CS$<>8__locals1.$VB$Local_Task.Progress = 0.7
                    Me.MinecraftVersions = CType(ModDownload.DlClientListLoader.Output.Value("versions").DeepClone(), JArray)
                    Me.VanillaData = Me.MinecraftVersions.OfType().FirstOrDefault(Function(v As JObject) Operators.CompareString(v("id").ToString().Replace("∞", "infinite"), CS$<>8__locals1.$VB$Local_MinecraftVersion.Replace("∞", "infinite"), False) = 0)
                    If Me.VanillaData Is Nothing Then
                        Throw New Exception(String.Format("Minecraft 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_MinecraftVersion))
                    End If
                    Me.VanillaData("id") = CS$<>8__locals1.$VB$Local_MinecraftVersion
                    If mcInstallManifest IsNot Nothing Then
                        Dim optiFineEntry As JObject = CS$<>8__locals1.$VB$Local_Components.OptiFineEntry
                        Me.SelectedOptiFine = If((optiFineEntry IsNot Nothing), optiFineEntry.ToObject(Of ModDownload.DlOptiFineListEntry)(), Nothing)
                        If Me.SelectedOptiFine IsNot Nothing AndAlso Not ModDownload.DlOptiFineListLoader.Output.Value.Any(Function(v As ModDownload.DlOptiFineListEntry) Operators.CompareString(v.FileName, Me.SelectedOptiFine.FileName, False) = 0) Then
                            If ModDownload.DlOptiFineListBmclapiLoader.State <> ModBase.LoadState.Finished Then
                                ModDownload.DlOptiFineListBmclapiLoader.WaitForExit(Nothing, Nothing, False, Nothing, Nothing)
                            End If
                            ModDownload.DlOptiFineListLoader.Output = ModDownload.DlOptiFineListBmclapiLoader.Output
                        End If
                    ElseIf CS$<>8__locals1.$VB$Local_OptiFineVersion IsNot Nothing AndAlso Operators.CompareString(CS$<>8__locals1.$VB$Local_OptiFineVersion, "未知版本", False) <> 0 Then
                        Me.SelectedOptiFine = ModDownload.DlOptiFineListLoader.Output.Value.FirstOrDefault(Function(v As ModDownload.DlOptiFineListEntry) Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, False) = 0 AndAlso Operators.CompareString(v.InstanceName.AfterLast("-OptiFine_", False).Replace("HD_U_", ""), CS$<>8__locals1.$VB$Local_OptiFineVersion.Replace("HD_U_", ""), False) = 0)
                        If Me.SelectedOptiFine Is Nothing Then
                            Throw New Exception(String.Format("OptiFine 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_OptiFineVersion))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge) Then
                        Dim dlForgeVersionEntry As ModDownload.DlForgeVersionEntry
                        If loaderTask Is Nothing Then
                            dlForgeVersionEntry = Nothing
                        Else
                            Dim output As List(Of ModDownload.DlForgeVersionEntry) = loaderTask.Output
                            dlForgeVersionEntry = If((output IsNot Nothing), output.FirstOrDefault(Function(v As ModDownload.DlForgeVersionEntry) Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, False) = 0 AndAlso Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.Forge), False) = 0 AndAlso Operators.CompareString(v.Category, "universal", False) <> 0 AndAlso Operators.CompareString(v.Category, "client", False) <> 0 AndAlso (CS$<>8__locals1.$VB$Me.SelectedOptiFine Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(CS$<>8__locals1.$VB$Me.SelectedOptiFine, v))), Nothing)
                        End If
                        Me.SelectedForge = dlForgeVersionEntry
                        If Me.SelectedForge Is Nothing Then
                            Throw New Exception(String.Format("Forge 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.Forge)))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge) Then
                        Me.SelectedNeoForge = ModDownload.DlNeoForgeListLoader.Output.Value.FirstOrDefault(Function(v As ModDownload.DlNeoForgeListEntry) Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, False) = 0 AndAlso Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.NeoForge), False) = 0)
                        If Me.SelectedNeoForge Is Nothing Then
                            Throw New Exception(String.Format("NeoForge 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.NeoForge)))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.Fabric) Then
                        Dim jobject As JObject = ModDownload.DlFabricListLoader.Output.Value("loader").OfType().FirstOrDefault(New Func(Of JObject, Boolean)() { Function(v As JObject) Operators.CompareString(v("version").ToString(), CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.Fabric), False) = 0, Function(v As JObject) Operators.CompareString(v("version").ToString().Replace("+build", ""), CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.Fabric), False) = 0 })
                        If jobject Is Nothing OrElse Not ModDownload.DlFabricListLoader.Output.Value("game").OfType().Any(Function(v As JObject) Operators.CompareString(v("version").ToString(), CS$<>8__locals1.$VB$Local_MinecraftVersion.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3"), False) = 0) Then
                            Throw New Exception(String.Format("Fabric 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.Fabric)))
                        End If
                        Me.SelectedFabric = jobject("version").ToString()
                    End If
                    If CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions.ContainsKey(ModLoaders.LiteLoader) Then
                        Me.SelectedLiteLoader = ModDownload.DlLiteLoaderListLoader.Output.Value.FirstOrDefault(Function(v As ModDownload.DlLiteLoaderListEntry) Operators.CompareString(v.Inherit, CS$<>8__locals1.$VB$Local_MinecraftVersion, False) = 0 AndAlso Operators.CompareString(v.JsonToken("version").ToString(), CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.LiteLoader), False) = 0)
                        If Me.SelectedLiteLoader Is Nothing Then
                            Throw New Exception(String.Format("LiteLoader 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions(ModLoaders.LiteLoader)))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Components.FabricApiVersion IsNot Nothing Then
                        Me.SelectedFabricApi = ModDownload.DlFabricApiLoader.Output.FirstOrDefault(Function(v As ResourceVersion) (Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Components.FabricApiVersion, False) = 0 OrElse Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Components.FabricApiVersion, False) = 0 OrElse Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Components.FabricApiVersion, False) = 0) AndAlso PageDownloadInstall.IsFabricApiCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion))
                        If Me.SelectedFabricApi Is Nothing Then
                            Throw New Exception(String.Format("Fabric API 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.FabricApiVersion))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion IsNot Nothing Then
                        Me.SelectedOptiFabric = ModDownload.DlOptiFabricLoader.Output.FirstOrDefault(Function(v As ResourceVersion) (Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion, False) = 0 OrElse Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion, False) = 0 OrElse Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion, False) = 0) AndAlso PageDownloadInstall.IsOptiFabricCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion))
                        If Me.SelectedOptiFabric Is Nothing Then
                            Throw New Exception(String.Format("OptiFabric 列表中没有已安装的 {0}", CS$<>8__locals1.$VB$Local_Components.OptiFabricVersion))
                        End If
                    End If
                    Dim list2 As List(Of McInstallManifest.ManagedFile) = New List(Of McInstallManifest.ManagedFile)()
                    If mcInstallManifest IsNot Nothing Then
                        list2.AddRange(mcInstallManifest.ManagedFiles)
                    End If
                    Dim lazy As Lazy(Of List(Of ValueTuple(Of FileInfo, String))) = New Lazy(Of List(Of ValueTuple(Of FileInfo, String)))(Function()
                        Dim text8 As String = CS$<>8__locals1.$VB$Local_Task.Input.PathIndie + "mods\"
                        Dim list9 As List(Of String) = DirectoryUtils.EnumerateFiles(text8, False, "*.jar").ToList()
                        If CS$<>8__locals1.$VB$Me.SelectedLiteLoader IsNot Nothing Then
                            list9.AddRange(DirectoryUtils.EnumerateFiles(Path.Combine(text8, CS$<>8__locals1.$VB$Local_MinecraftVersion), False, "*.jar"))
                        End If
                        Dim list10 As List(Of ValueTuple(Of FileInfo, String)) = New List(Of ValueTuple(Of FileInfo, String))()
                        Try
                            For Each text9 As String In list9
                                If CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                                    Return list10
                                End If
                                Dim info As FileInfo = FileUtils.GetInfo(text9)
                                list10.Add(New ValueTuple(Of FileInfo, String)(info, New LocalResourceFile(info).ModrinthHash))
                                CS$<>8__locals1.$VB$Local_Task.Progress = 0.7 + 0.28 * CDbl(list10.Count) / CDbl(list9.Count)
                            Next
                        Finally
                            Dim enumerator6 As List(Of String).Enumerator
                            CType(enumerator6, IDisposable).Dispose()
                        End Try
                        Return list10
                    End Function)
                    If Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedFabricApi Is Nothing Then
                        Dim list3 As List(Of ResourceVersion) = ModDownload.DlFabricApiLoader.Output.Where(Function(v As ResourceVersion) PageDownloadInstall.IsFabricApiCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion)).ToList()
                        Dim list4 As List(Of ValueTuple(Of FileInfo, String, ResourceVersion)) = New List(Of ValueTuple(Of FileInfo, String, ResourceVersion))()
                        Try
                            Dim enumerator3 As List(Of ValueTuple(Of FileInfo, String)).Enumerator = lazy.Value.GetEnumerator()
                            While enumerator3.MoveNext()
                                Dim CS$<>8__locals2 As PageDownloadModify._Closure$__12-1 = New PageDownloadModify._Closure$__12-1(CS$<>8__locals2)
                                CS$<>8__locals2.$VB$Local_Entry = enumerator3.Current
                                Dim resourceVersion As ResourceVersion = list3.FirstOrDefault(Function(v As ResourceVersion) Operators.CompareString(v.Hash, CS$<>8__locals2.$VB$Local_Entry.Item2, False) = 0)
                                If resourceVersion IsNot Nothing Then
                                    list4.Add(New ValueTuple(Of FileInfo, String, ResourceVersion)(CS$<>8__locals2.$VB$Local_Entry.Item1, CS$<>8__locals2.$VB$Local_Entry.Item2, resourceVersion))
                                End If
                            End While
                        Finally
                            Dim enumerator3 As List(Of ValueTuple(Of FileInfo, String)).Enumerator
                            CType(enumerator3, IDisposable).Dispose()
                        End Try
                        Dim valueTuple As ValueTuple(Of FileInfo, String, ResourceVersion)? = Me.SelectMod(Of ResourceVersion)("Fabric API", list4)
                        If valueTuple IsNot Nothing Then
                            Me.SelectedFabricApi = valueTuple.Value.Item3
                            list2.Add(New McInstallManifest.ManagedFile("mods/" + valueTuple.Value.Item1.Name.Lower(), valueTuple.Value.Item2, valueTuple.Value.Item1.LastWriteTimeUtc, "FabricApi"))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                        Exit While
                    End If
                    If Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedOptiFabric Is Nothing Then
                        Dim list5 As List(Of ResourceVersion) = ModDownload.DlOptiFabricLoader.Output.Where(Function(v As ResourceVersion) PageDownloadInstall.IsOptiFabricCompatible(v, CS$<>8__locals1.$VB$Local_MinecraftVersion)).ToList()
                        Dim enumerable2 As IEnumerable(Of ResourceVersion) = list5
                        Dim func9 As Func(Of ResourceVersion, Boolean)
                        If PageDownloadModify._Closure$__.$I12-20 IsNot Nothing Then
                            func9 = PageDownloadModify._Closure$__.$I12-20
                        Else
                            Dim func10 As Func(Of ResourceVersion, Boolean) = Function(v As ResourceVersion) (If(v.Hash, "")).Length = 32
                            func9 = func10
                            PageDownloadModify._Closure$__.$I12-20 = func10
                        End If
                        Dim flag As Boolean = enumerable2.Any(func9)
                        Dim list6 As List(Of ValueTuple(Of FileInfo, String, ResourceVersion)) = New List(Of ValueTuple(Of FileInfo, String, ResourceVersion))()
                        Try
                            Dim enumerator4 As List(Of ValueTuple(Of FileInfo, String)).Enumerator = lazy.Value.GetEnumerator()
                            While enumerator4.MoveNext()
                                Dim CS$<>8__locals3 As PageDownloadModify._Closure$__12-2 = New PageDownloadModify._Closure$__12-2(CS$<>8__locals3)
                                CS$<>8__locals3.$VB$Local_Entry = enumerator4.Current
                                Dim resourceVersion2 As ResourceVersion = list5.FirstOrDefault(Function(v As ResourceVersion) Operators.CompareString(v.Hash, CS$<>8__locals3.$VB$Local_Entry.Item2, False) = 0)
                                If resourceVersion2 Is Nothing AndAlso flag Then
                                    Dim CS$<>8__locals4 As PageDownloadModify._Closure$__12-3 = New PageDownloadModify._Closure$__12-3(CS$<>8__locals4)
                                    CS$<>8__locals4.$VB$Local_Md5 = CryptographyUtils.ComputeFileHash(CS$<>8__locals3.$VB$Local_Entry.Item1.FullName, CryptographyUtils.HashMethod.Md5)
                                    resourceVersion2 = list5.FirstOrDefault(Function(v As ResourceVersion) Operators.CompareString(v.Hash, CS$<>8__locals4.$VB$Local_Md5, False) = 0)
                                End If
                                If resourceVersion2 IsNot Nothing Then
                                    list6.Add(New ValueTuple(Of FileInfo, String, ResourceVersion)(CS$<>8__locals3.$VB$Local_Entry.Item1, CS$<>8__locals3.$VB$Local_Entry.Item2, resourceVersion2))
                                End If
                            End While
                        Finally
                            Dim enumerator4 As List(Of ValueTuple(Of FileInfo, String)).Enumerator
                            CType(enumerator4, IDisposable).Dispose()
                        End Try
                        Dim valueTuple2 As ValueTuple(Of FileInfo, String, ResourceVersion)? = Me.SelectMod(Of ResourceVersion)("OptiFabric", list6)
                        If valueTuple2 IsNot Nothing Then
                            Me.SelectedOptiFabric = valueTuple2.Value.Item3
                            list2.Add(New McInstallManifest.ManagedFile("mods/" + valueTuple2.Value.Item1.Name.Lower(), valueTuple2.Value.Item2, valueTuple2.Value.Item1.LastWriteTimeUtc, "OptiFabric"))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                        Exit While
                    End If
                    Dim list7 As List(Of ValueTuple(Of FileInfo, String, String)) = New List(Of ValueTuple(Of FileInfo, String, String))()
                    If Me.SelectedOptiFine Is Nothing AndAlso (Me.SelectedForge IsNot Nothing OrElse Me.SelectedLiteLoader IsNot Nothing OrElse Me.SelectedFabric IsNot Nothing) Then
                        If Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag2 As Boolean = False
                        Try
                            Monitor.Enter(Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init, flag2)
                            If Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State = 0S Then
                                Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State = 2S
                                Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes = CType(FileUtils.ReadAsJson("Minecraft\OptiFineHashes.json", Nothing, GetType(Resource)), JObject)
                            ElseIf Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init.State = 1S
                            If flag2 Then
                                Monitor.[Exit](Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init)
                            End If
                        End Try
                        Dim enumerable3 As IEnumerable(Of ValueTuple(Of FileInfo, String, String)) = lazy.Value.[Select](Function(<TupleElementNames(New String() { "File", "Sha1" })> Entry As ValueTuple(Of FileInfo, String))
                            Dim item As FileInfo = Entry.Item1
                            Dim item2 As String = Entry.Item2
                            Dim jtoken As JToken = Me.$STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes(Entry.Item2)
                            Return New ValueTuple(Of FileInfo, String, String)(item, item2, If((jtoken IsNot Nothing), jtoken.ToString(), Nothing))
                        End Function)
                        Dim func11 As Func(Of ValueTuple(Of FileInfo, String, String), Boolean)
                        If PageDownloadModify._Closure$__.$I12-24 IsNot Nothing Then
                            func11 = PageDownloadModify._Closure$__.$I12-24
                        Else
                            Dim func12 As Func(Of ValueTuple(Of FileInfo, String, String), Boolean) = Function(<TupleElementNames(New String() { "File", "Sha1", "OriginalName" })> Entry As ValueTuple(Of FileInfo, String, String)) Entry.Item3 <> Nothing
                            func11 = func12
                            PageDownloadModify._Closure$__.$I12-24 = func12
                        End If
                        list7 = enumerable3.Where(func11).ToList()
                        Dim list8 As List(Of ValueTuple(Of FileInfo, String, ModDownload.DlOptiFineListEntry)) = New List(Of ValueTuple(Of FileInfo, String, ModDownload.DlOptiFineListEntry))()
                        Try
                            Dim enumerator5 As List(Of ValueTuple(Of FileInfo, String, String)).Enumerator = list7.GetEnumerator()
                            While enumerator5.MoveNext()
                                Dim CS$<>8__locals5 As PageDownloadModify._Closure$__12-4 = New PageDownloadModify._Closure$__12-4(CS$<>8__locals5)
                                CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                                CS$<>8__locals5.$VB$Local_Entry = enumerator5.Current
                                Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry = ModDownload.DlOptiFineListLoader.Output.Value.FirstOrDefault(Function(v As ModDownload.DlOptiFineListEntry) Operators.CompareString(v.Inherit, CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2.$VB$Local_MinecraftVersion, False) = 0 AndAlso Operators.CompareString(v.FileName, CS$<>8__locals5.$VB$Local_Entry.Item3, False) = 0 AndAlso (CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2.$VB$Me.SelectedForge Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(v, CS$<>8__locals5.$VB$NonLocal_$VB$Closure_2.$VB$Me.SelectedForge)))
                                If dlOptiFineListEntry Is Nothing Then
                                    ModMain.Hint(String.Format("无法从 OptiFine 列表还原 {0} 对应的 {1}，将跳过该文件。", CS$<>8__locals5.$VB$Local_Entry.Item1.Name, CS$<>8__locals5.$VB$Local_Entry.Item3), ModMain.HintType.Blue, True)
                                Else
                                    list8.Add(New ValueTuple(Of FileInfo, String, ModDownload.DlOptiFineListEntry)(CS$<>8__locals5.$VB$Local_Entry.Item1, CS$<>8__locals5.$VB$Local_Entry.Item2, dlOptiFineListEntry))
                                End If
                            End While
                        Finally
                            Dim enumerator5 As List(Of ValueTuple(Of FileInfo, String, String)).Enumerator
                            CType(enumerator5, IDisposable).Dispose()
                        End Try
                        Dim valueTuple3 As ValueTuple(Of FileInfo, String, ModDownload.DlOptiFineListEntry)? = Me.SelectMod(Of ModDownload.DlOptiFineListEntry)("OptiFine", list8)
                        If valueTuple3 IsNot Nothing Then
                            Me.SelectedOptiFine = valueTuple3.Value.Item3
                            Dim text As String = PathUtils.RemoveExtendedPrefix(valueTuple3.Value.Item1.FullName).Substring(CS$<>8__locals1.$VB$Local_Task.Input.PathIndie.Length).Replace("\", "/").Lower()
                            list2.Add(New McInstallManifest.ManagedFile(text, valueTuple3.Value.Item2, valueTuple3.Value.Item1.LastWriteTimeUtc, "OptiFine"))
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                        Exit While
                    End If
                    If Me.IsVanillaVersionReliable Then
                        Dim CS$<>8__locals6 As PageDownloadModify._Closure$__12-0 = CS$<>8__locals1
                        Dim $VB$Local_MinecraftVersion As String = CS$<>8__locals1.$VB$Local_MinecraftVersion
                        Dim modLoaderVersions2 As Dictionary(Of ModLoaders, String) = CS$<>8__locals1.$VB$Local_Components.ModLoaderVersions
                        Dim jobject2 As JObject = If((Me.SelectedOptiFine Is Nothing), Nothing, JObject.FromObject(Me.SelectedOptiFine))
                        Dim selectedFabricApi As ResourceVersion = Me.SelectedFabricApi
                        Dim text2 As String = If((selectedFabricApi IsNot Nothing), selectedFabricApi.Version, Nothing)
                        Dim text3 As String = text2
                        If text2 Is Nothing Then
                            Dim selectedFabricApi2 As ResourceVersion = Me.SelectedFabricApi
                            Dim text4 As String = If((selectedFabricApi2 IsNot Nothing), selectedFabricApi2.Id, Nothing)
                            text3 = text4
                            If text4 Is Nothing Then
                                Dim selectedFabricApi3 As ResourceVersion = Me.SelectedFabricApi
                                text3 = If((selectedFabricApi3 IsNot Nothing), selectedFabricApi3.Display, Nothing)
                            End If
                        End If
                        Dim selectedOptiFabric As ResourceVersion = Me.SelectedOptiFabric
                        Dim text5 As String = If((selectedOptiFabric IsNot Nothing), selectedOptiFabric.Version, Nothing)
                        Dim text6 As String = text5
                        If text5 Is Nothing Then
                            Dim selectedOptiFabric2 As ResourceVersion = Me.SelectedOptiFabric
                            Dim text7 As String = If((selectedOptiFabric2 IsNot Nothing), selectedOptiFabric2.Id, Nothing)
                            text6 = text7
                            If text7 Is Nothing Then
                                Dim selectedOptiFabric3 As ResourceVersion = Me.SelectedOptiFabric
                                text6 = If((selectedOptiFabric3 IsNot Nothing), selectedOptiFabric3.Display, Nothing)
                            End If
                        End If
                        CS$<>8__locals6.$VB$Local_Components = New McComponents($VB$Local_MinecraftVersion, modLoaderVersions2, jobject2, text3, text6)
                        If mcInstallManifest Is Nothing OrElse Not CS$<>8__locals1.$VB$Local_Components.IsEquivalentTo(mcInstallManifest.Components) OrElse Not list2.SequenceEqual(mcInstallManifest.ManagedFiles) Then
                            Configs.McInstallManifest.[Set](New McInstallManifest(1, CS$<>8__locals1.$VB$Local_Components, DateTime.UtcNow, list2), CS$<>8__locals1.$VB$Local_Task.Input.Config)
                            CS$<>8__locals1.$VB$Local_Task.Input.Config.Save()
                            Logger.Info(String.Format("已为 {0} 更新安装清单", CS$<>8__locals1.$VB$Local_Task.Input.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\PageDownloadModify.vb")
                        End If
                    End If
                    If Not CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                        Me.ForgeVersionLoader = loaderTask
                        Exit While
                    End If
                    Exit While
                End If
            End While
        End Sub

        Private Function SelectMod(Of T)(ComponentName As String, <TupleElementNames(New String() { "File", "Sha1", "Version" })> Matches As List(Of ValueTuple(Of FileInfo, String, T))) As <TupleElementNames(New String() { "File", "Sha1", "Version" })> ValueTuple(Of FileInfo, String, T)?
            Dim valueTuple As ValueTuple(Of FileInfo, String, T)?
            If Matches.Count = 0 Then
                valueTuple = Nothing
            ElseIf Matches.Count = 1 Then
                valueTuple = New ValueTuple(Of FileInfo, String, T)?(Matches(0))
            Else
                Dim Choice As Integer? = Nothing
                ModBase.RunInUiWait(Sub()
                    Dim $VB$Local_Matches As IEnumerable(Of ValueTuple(Of FileInfo, String, $CLS0)) = Matches
                    Dim func As Func(Of ValueTuple(Of FileInfo, String, $CLS0), IMyRadio)
                    If PageDownloadModify._Closure$__13(Of $CLS0).$I13-1 IsNot Nothing Then
                        func = PageDownloadModify._Closure$__13(Of $CLS0).$I13-1
                    Else
                        Dim func2 As Func(Of ValueTuple(Of FileInfo, String, $CLS0), IMyRadio) = Function(<TupleElementNames(New String() { "File", "Sha1", "Version" })> Entry As ValueTuple(Of FileInfo, String, $CLS0)) New MyRadioBox() With { .Text = Entry.Item1.Name }
                        func = func2
                        PageDownloadModify._Closure$__13(Of $CLS0).$I13-1 = func2
                    End If
                    Choice = ModMain.MyMsgBoxSelect($VB$Local_Matches.[Select](func), String.Format("选择当前使用的 {0}", ComponentName), "确定", "", False)
                End Sub)
                If Choice Is Nothing Then
                    Throw New Exception(String.Format("发现了多个 {0}", ComponentName))
                End If
                valueTuple = New ValueTuple(Of FileInfo, String, T)?(Matches(Choice.Value))
            End If
            Return valueTuple
        End Function

        Private Sub LoadInstanceFinished(Task As ModLoader.LoaderTask(Of ModMinecraft.McInstance, Object))
            Me.OriginalName = Task.Input.Name
            Me.CustomLogo = Nothing
            If Conversions.ToBoolean(ModBase.ReadIni(Task.Input.PathVersion + "PCL\Setup.ini", "LogoCustom", Conversions.ToString(False))) Then
                Me.CustomLogo = If(Task.Input.Logo.EndsWithF("PCL\Logo.png", False), (Task.Input.PathVersion + "PCL\Logo.png"), Task.Input.Logo)
            End If
            Dim myListItem As MyListItem = ModDownloadLib.McDownloadListItem(Me.VanillaData, Nothing, False).Init()
            Me.VanillaName = myListItem.Title
            Me.VanillaIcon = myListItem.Logo
            Me.LabMinecraftVersion.Text = Me.VanillaName
            Me.TextSelectName.Text = Me.OriginalName
            Me.IsSelectNameEdited = Not String.Equals(Me.OriginalName, MyBase.GetSelectName(), StringComparison.Ordinal)
            Me.TextSelectName.ValidateRules = New Collection(Of Validate)() From { New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, New List(Of String)() From { Me.OriginalName }) }
            Me.CardMinecraft.Visibility = Visibility.Collapsed
            Me.BtnBack.Visibility = Visibility.Collapsed
            Me.ImgLogo.Margin = New Thickness(15.0, 15.0, 0.0, 0.0)
            Me.TextSelectName.Margin = New Thickness(56.0, 16.0, 16.0, 0.0)
            Me.LabSelectName.Text = String.Format("原名 :   {0}", Me.OriginalName)
            Me.UpdateOriginalNameVisibility()
            Me.BtnStart.Text = "开始修改"
            Me.BtnStart.Logo = ModMain.FrmInstanceLeft.ItemModify.Logo
            Me.AutoSelectedFabricApi = Me.SelectedFabric <> Nothing
            Me.AutoSelectedOptiFabric = Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedOptiFine IsNot Nothing
            If Me.ForgeVersionLoader IsNot Nothing Then
                Me.LoadForge.State = Me.ForgeVersionLoader
            End If
            MyBase.PrepareSelectPage()
            MyBase.ReloadComponentLists()
            Me.PanMinecraft.Visibility = Visibility.Collapsed
            Me.PanMinecraft.Opacity = 0.0
            Me.PanMinecraft.RenderTransform = New TranslateTransform(-50.0, 0.0)
            Me.PanSelect.Opacity = 1.0
            Me.PanSelect.RenderTransform = New TranslateTransform()
            Me.PanBack.IsHitTestVisible = True
            Me.PanAllBack.IsHitTestVisible = True
            Me.InitialTarget = MyBase.GetSelectedComponents()
            Me.BtnMinecraftRestore.Tag = Me.VanillaData
            Me.BtnOptiFineRestore.Tag = Me.SelectedOptiFine
            Me.BtnForgeRestore.Tag = Me.SelectedForge
            Me.BtnNeoForgeRestore.Tag = Me.SelectedNeoForge
            Me.BtnFabricRestore.Tag = Me.SelectedFabric
            Me.BtnFabricApiRestore.Tag = Me.SelectedFabricApi
            Me.BtnOptiFabricRestore.Tag = Me.SelectedOptiFabric
            Me.BtnLiteLoaderRestore.Tag = Me.SelectedLiteLoader
            Me.ImgMinecraftOld.Source = Me.VanillaIcon
            Me.LabMinecraftOld.Text = Me.LabMinecraftVersion.Text
            Me.LabOptiFineOld.Text = If((Me.SelectedOptiFine Is Nothing), "无", Me.LabOptiFine.Text)
            Me.LabForgeOld.Text = If((Me.SelectedForge Is Nothing), "无", Me.LabForge.Text)
            Me.LabNeoForgeOld.Text = If((Me.SelectedNeoForge Is Nothing), "无", Me.LabNeoForge.Text)
            Me.LabFabricOld.Text = If((Me.SelectedFabric Is Nothing), "无", Me.LabFabric.Text)
            Me.LabLiteLoaderOld.Text = If((Me.SelectedLiteLoader Is Nothing), "无", Me.LabLiteLoader.Text)
            Me.LabFabricApiOld.Text = If((Me.SelectedFabricApi Is Nothing), "无", Me.LabFabricApi.Text)
            Me.LabOptiFabricOld.Text = If((Me.SelectedOptiFabric Is Nothing), "无", Me.LabOptiFabric.Text)
            Me.RefreshComparisons()
        End Sub

        Public Overrides Function GetSelectLogo() As String
            Return If(Me.CustomLogo, MyBase.GetSelectLogo())
        End Function

        Public Overrides Sub ShowMinecraftVersionList()
            If Me.IsInSelectPage Then
                Dim selectedComponents As McComponents = MyBase.GetSelectedComponents()
                Dim array As ValueTuple(Of MyLoading, String)() = New ValueTuple(Of MyLoading, String)(6) {}
                array(0) = New ValueTuple(Of MyLoading, String)(Me.LoadOptiFine, selectedComponents.OptiFineVersion)
                array(1) = New ValueTuple(Of MyLoading, String)(Me.LoadFabricApi, selectedComponents.FabricApiVersion)
                array(2) = New ValueTuple(Of MyLoading, String)(Me.LoadOptiFabric, selectedComponents.OptiFabricVersion)
                Dim num As Integer = 3
                Dim loadForge As MyLoading = Me.LoadForge
                Dim selectedForge As ModDownload.DlForgeVersionEntry = Me.SelectedForge
                array(num) = New ValueTuple(Of MyLoading, String)(loadForge, If((selectedForge IsNot Nothing), selectedForge.VersionName, Nothing))
                Dim num2 As Integer = 4
                Dim loadNeoForge As MyLoading = Me.LoadNeoForge
                Dim selectedNeoForge As ModDownload.DlNeoForgeListEntry = Me.SelectedNeoForge
                array(num2) = New ValueTuple(Of MyLoading, String)(loadNeoForge, If((selectedNeoForge IsNot Nothing), selectedNeoForge.VersionName, Nothing))
                array(5) = New ValueTuple(Of MyLoading, String)(Me.LoadFabric, Me.SelectedFabric)
                Dim num3 As Integer = 6
                Dim loadLiteLoader As MyLoading = Me.LoadLiteLoader
                Dim selectedLiteLoader As ModDownload.DlLiteLoaderListEntry = Me.SelectedLiteLoader
                Dim text As String
                If selectedLiteLoader Is Nothing Then
                    text = Nothing
                Else
                    Dim jsonToken As JToken = selectedLiteLoader.JsonToken
                    text = If((jsonToken IsNot Nothing), jsonToken("version").ToString(), Nothing)
                End If
                array(num3) = New ValueTuple(Of MyLoading, String)(loadLiteLoader, text)
                For Each valueTuple As ValueTuple(Of MyLoading, String) In array
                    If valueTuple.Item2 IsNot Nothing Then
                        Me.PendingComponents(valueTuple.Item1) = valueTuple.Item2
                    End If
                Next
            End If
            MyBase.ShowMinecraftVersionList()
        End Sub

        Public Overrides Sub ReloadSelected()
            If Not Me.IsRestoringComponents Then
                If Me.IsInSelectPage AndAlso Me.VanillaName IsNot Nothing AndAlso Me.PendingComponents.Any() Then
                    Me.IsRestoringComponents = True
                    Try
                        Dim flag As Boolean = False
                        For Each myLoading As MyLoading In New MyLoading() { Me.LoadFabric, Me.LoadForge, Me.LoadNeoForge, Me.LoadLiteLoader, Me.LoadOptiFine, Me.LoadFabricApi, Me.LoadOptiFabric }
                            Dim CS$<>8__locals1 As PageDownloadModify._Closure$__17-0 = New PageDownloadModify._Closure$__17-0(CS$<>8__locals1)
                            CS$<>8__locals1.$VB$Local_Version = Nothing
                            If Me.PendingComponents.TryGetValue(myLoading, CS$<>8__locals1.$VB$Local_Version) AndAlso myLoading.State.IsLoader Then
                                Dim loaderBase As ModLoader.LoaderBase = CType(myLoading.State, ModLoader.LoaderBase)
                                If myLoading Is Me.LoadOptiFine Then
                                    If New MyLoading() { Me.LoadFabric, Me.LoadForge, Me.LoadNeoForge, Me.LoadLiteLoader }.Any(Function(Component As MyLoading) Me.PendingComponents.ContainsKey(Component)) Then
                                        GoTo IL_05FC
                                    End If
                                ElseIf myLoading Is Me.LoadFabricApi Then
                                    If Me.PendingComponents.ContainsKey(Me.LoadFabric) Then
                                        GoTo IL_05FC
                                    End If
                                ElseIf myLoading Is Me.LoadForge Then
                                    If Not McVersion.IsStandardFormat(Me.VanillaName) Then
                                        Me.PendingComponents.Remove(myLoading)
                                        GoTo IL_05FC
                                    End If
                                    If Operators.CompareString(CType(loaderBase, ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))).Input, Me.VanillaName, False) <> 0 Then
                                        GoTo IL_05FC
                                    End If
                                    If loaderBase.State = ModBase.LoadState.Failed AndAlso Operators.CompareString(loaderBase.[Error].Message, "无", False) = 0 Then
                                        Me.PendingComponents.Remove(myLoading)
                                        GoTo IL_05FC
                                    End If
                                ElseIf myLoading Is Me.LoadOptiFabric AndAlso (Me.PendingComponents.ContainsKey(Me.LoadFabric) OrElse Me.PendingComponents.ContainsKey(Me.LoadOptiFine)) Then
                                    GoTo IL_05FC
                                End If
                                If loaderBase.State = ModBase.LoadState.Finished Then
                                    Me.PendingComponents.Remove(myLoading)
                                    flag = True
                                    If myLoading Is Me.LoadFabric Then
                                        If Me.SelectedFabric Is Nothing AndAlso MyBase.LoadFabricGetError() Is Nothing Then
                                            Dim enumerable As IEnumerable(Of JObject) = ModDownload.DlFabricListLoader.Output.Value("loader").OfType()
                                            Dim jobject As JObject = If(enumerable.FirstOrDefault(Function(v As JObject) Operators.CompareString(v("version").ToString(), CS$<>8__locals1.$VB$Local_Version, False) = 0), enumerable.FirstOrDefault())
                                            Me.SelectedFabric = If((jobject IsNot Nothing), jobject("version").ToString(), Nothing)
                                        End If
                                    ElseIf myLoading Is Me.LoadOptiFine Then
                                        If Me.SelectedOptiFine Is Nothing AndAlso MyBase.LoadOptiFineGetError() Is Nothing Then
                                            Dim enumerable2 As IEnumerable(Of ModDownload.DlOptiFineListEntry) = ModDownload.DlOptiFineListLoader.Output.Value.Where(Function(v As ModDownload.DlOptiFineListEntry) Operators.CompareString(v.Inherit, Me.VanillaName, False) = 0 AndAlso (Me.SelectedForge Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(v, Me.SelectedForge)))
                                            Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry = enumerable2.FirstOrDefault(Function(v As ModDownload.DlOptiFineListEntry) Operators.CompareString(v.InstanceName.AfterLast("-OptiFine_", False), CS$<>8__locals1.$VB$Local_Version, False) = 0)
                                            Dim dlOptiFineListEntry2 As ModDownload.DlOptiFineListEntry = dlOptiFineListEntry
                                            If dlOptiFineListEntry Is Nothing Then
                                                Dim enumerable3 As IEnumerable(Of ModDownload.DlOptiFineListEntry) = enumerable2
                                                Dim func As Func(Of ModDownload.DlOptiFineListEntry, Boolean)
                                                If PageDownloadModify._Closure$__.$I17-4 IsNot Nothing Then
                                                    func = PageDownloadModify._Closure$__.$I17-4
                                                Else
                                                    Dim func2 As Func(Of ModDownload.DlOptiFineListEntry, Boolean) = Function(v As ModDownload.DlOptiFineListEntry) v.IsPreview
                                                    func = func2
                                                    PageDownloadModify._Closure$__.$I17-4 = func2
                                                End If
                                                Dim orderedEnumerable As IOrderedEnumerable(Of ModDownload.DlOptiFineListEntry) = enumerable3.OrderBy(func)
                                                Dim func3 As Func(Of ModDownload.DlOptiFineListEntry, String)
                                                If PageDownloadModify._Closure$__.$I17-5 IsNot Nothing Then
                                                    func3 = PageDownloadModify._Closure$__.$I17-5
                                                Else
                                                    Dim func4 As Func(Of ModDownload.DlOptiFineListEntry, String) = Function(v As ModDownload.DlOptiFineListEntry) v.DisplayName
                                                    func3 = func4
                                                    PageDownloadModify._Closure$__.$I17-5 = func4
                                                End If
                                                dlOptiFineListEntry2 = orderedEnumerable.ThenByDescending(func3, Comparer(Of String).Create(New Comparison(Of String)(McUtils.CompareVersion))).FirstOrDefault()
                                            End If
                                            Me.SelectedOptiFine = dlOptiFineListEntry2
                                        End If
                                    ElseIf myLoading Is Me.LoadForge Then
                                        If Me.SelectedForge Is Nothing AndAlso MyBase.LoadForgeGetError() Is Nothing Then
                                            Dim enumerable4 As IEnumerable(Of ModDownload.DlForgeVersionEntry) = CType(loaderBase, ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))).Output.Where(Function(v As ModDownload.DlForgeVersionEntry) Operators.CompareString(v.Inherit, Me.VanillaName, False) = 0 AndAlso Operators.CompareString(v.Category, "universal", False) <> 0 AndAlso Operators.CompareString(v.Category, "client", False) <> 0 AndAlso (Me.SelectedOptiFine Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(Me.SelectedOptiFine, v)))
                                            Dim dlForgeVersionEntry As ModDownload.DlForgeVersionEntry = enumerable4.FirstOrDefault(Function(v As ModDownload.DlForgeVersionEntry) Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Version, False) = 0)
                                            Dim dlForgeVersionEntry2 As ModDownload.DlForgeVersionEntry = dlForgeVersionEntry
                                            If dlForgeVersionEntry Is Nothing Then
                                                Dim enumerable5 As IEnumerable(Of ModDownload.DlForgeVersionEntry) = enumerable4
                                                Dim func5 As Func(Of ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry)
                                                If PageDownloadModify._Closure$__.$I17-8 IsNot Nothing Then
                                                    func5 = PageDownloadModify._Closure$__.$I17-8
                                                Else
                                                    Dim func6 As Func(Of ModDownload.DlForgeVersionEntry, ModDownload.DlForgeVersionEntry) = Function(v As ModDownload.DlForgeVersionEntry) v
                                                    func5 = func6
                                                    PageDownloadModify._Closure$__.$I17-8 = func6
                                                End If
                                                dlForgeVersionEntry2 = enumerable5.OrderByDescending(func5).FirstOrDefault()
                                            End If
                                            Me.SelectedForge = dlForgeVersionEntry2
                                        End If
                                    ElseIf myLoading Is Me.LoadNeoForge Then
                                        If Me.SelectedNeoForge Is Nothing AndAlso MyBase.LoadNeoForgeGetError() Is Nothing Then
                                            Dim enumerable6 As IEnumerable(Of ModDownload.DlNeoForgeListEntry) = ModDownload.DlNeoForgeListLoader.Output.Value.Where(Function(v As ModDownload.DlNeoForgeListEntry) Operators.CompareString(v.Inherit, Me.VanillaName, False) = 0)
                                            Me.SelectedNeoForge = If(enumerable6.FirstOrDefault(Function(v As ModDownload.DlNeoForgeListEntry) Operators.CompareString(v.VersionName, CS$<>8__locals1.$VB$Local_Version, False) = 0), enumerable6.FirstOrDefault())
                                        End If
                                    ElseIf myLoading Is Me.LoadLiteLoader Then
                                        If Me.SelectedLiteLoader Is Nothing AndAlso MyBase.LoadLiteLoaderGetError() Is Nothing Then
                                            Dim enumerable7 As IEnumerable(Of ModDownload.DlLiteLoaderListEntry) = ModDownload.DlLiteLoaderListLoader.Output.Value.Where(Function(v As ModDownload.DlLiteLoaderListEntry) Operators.CompareString(v.Inherit, Me.VanillaName, False) = 0)
                                            Me.SelectedLiteLoader = If(enumerable7.FirstOrDefault(Function(v As ModDownload.DlLiteLoaderListEntry) Operators.CompareString(v.JsonToken("version").ToString(), CS$<>8__locals1.$VB$Local_Version, False) = 0), enumerable7.FirstOrDefault())
                                        End If
                                    ElseIf myLoading Is Me.LoadFabricApi Then
                                        If Me.SelectedFabricApi Is Nothing AndAlso MyBase.LoadFabricApiGetError() Is Nothing Then
                                            Dim enumerable8 As IEnumerable(Of ResourceVersion) = ModDownload.DlFabricApiLoader.Output.Where(Function(v As ResourceVersion) PageDownloadInstall.IsFabricApiCompatible(v, Me.VanillaName))
                                            Dim resourceVersion As ResourceVersion = enumerable8.FirstOrDefault(Function(v As ResourceVersion) Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Version, False) = 0 OrElse Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Version, False) = 0 OrElse Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Version, False) = 0)
                                            Dim resourceVersion2 As ResourceVersion = resourceVersion
                                            If resourceVersion Is Nothing Then
                                                Dim enumerable9 As IEnumerable(Of ResourceVersion) = enumerable8
                                                Dim func7 As Func(Of ResourceVersion, DateTime)
                                                If PageDownloadModify._Closure$__.$I17-15 IsNot Nothing Then
                                                    func7 = PageDownloadModify._Closure$__.$I17-15
                                                Else
                                                    Dim func8 As Func(Of ResourceVersion, DateTime) = Function(v As ResourceVersion) v.ReleaseDate
                                                    func7 = func8
                                                    PageDownloadModify._Closure$__.$I17-15 = func8
                                                End If
                                                resourceVersion2 = enumerable9.OrderByDescending(func7).FirstOrDefault()
                                            End If
                                            Me.SelectedFabricApi = resourceVersion2
                                        End If
                                        If Me.SelectedFabricApi IsNot Nothing Then
                                            Me.AutoSelectedFabricApi = True
                                        End If
                                    ElseIf myLoading Is Me.LoadOptiFabric Then
                                        If Me.SelectedOptiFabric Is Nothing AndAlso MyBase.LoadOptiFabricGetError() Is Nothing Then
                                            Dim enumerable10 As IEnumerable(Of ResourceVersion) = ModDownload.DlOptiFabricLoader.Output.Where(Function(v As ResourceVersion) PageDownloadInstall.IsOptiFabricCompatible(v, Me.VanillaName))
                                            Dim resourceVersion3 As ResourceVersion = enumerable10.FirstOrDefault(Function(v As ResourceVersion) Operators.CompareString(v.Version, CS$<>8__locals1.$VB$Local_Version, False) = 0 OrElse Operators.CompareString(v.Id, CS$<>8__locals1.$VB$Local_Version, False) = 0 OrElse Operators.CompareString(v.Display, CS$<>8__locals1.$VB$Local_Version, False) = 0)
                                            Dim resourceVersion4 As ResourceVersion = resourceVersion3
                                            If resourceVersion3 Is Nothing Then
                                                Dim enumerable11 As IEnumerable(Of ResourceVersion) = enumerable10
                                                Dim func9 As Func(Of ResourceVersion, DateTime)
                                                If PageDownloadModify._Closure$__.$I17-18 IsNot Nothing Then
                                                    func9 = PageDownloadModify._Closure$__.$I17-18
                                                Else
                                                    Dim func10 As Func(Of ResourceVersion, DateTime) = Function(v As ResourceVersion) v.ReleaseDate
                                                    func9 = func10
                                                    PageDownloadModify._Closure$__.$I17-18 = func10
                                                End If
                                                resourceVersion4 = enumerable11.OrderByDescending(func9).FirstOrDefault()
                                            End If
                                            Me.SelectedOptiFabric = resourceVersion4
                                        End If
                                        If Me.SelectedOptiFabric IsNot Nothing Then
                                            Me.AutoSelectedOptiFabric = True
                                        End If
                                    End If
                                End If
                            End If
                            IL_05FC:
                        Next
                        If flag Then
                            MyBase.ReloadComponentLists()
                        End If
                    Finally
                        Me.IsRestoringComponents = False
                    End Try
                End If
                MyBase.ReloadSelected()
                If Me.InitialTarget IsNot Nothing Then
                    Me.RefreshComparisons()
                End If
            End If
        End Sub

        Private Sub RefreshComparisons()
            Dim selectedComponents As McComponents = MyBase.GetSelectedComponents()
            Me.ImgMinecraftNew.Source = Me.VanillaIcon
            Dim flag As Boolean = Operators.CompareString(Me.InitialTarget.MinecraftVersion, selectedComponents.MinecraftVersion, False) = 0
            Me.ShowComparison(Me.PanMinecraftCompare, Me.PanMinecraftInfo, Me.InitialTarget.MinecraftVersion, selectedComponents.MinecraftVersion, Me.LabMinecraftVersion.Text, Me.BtnMinecraftRestore, True, Nothing)
            Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry = TryCast(Me.BtnOptiFineRestore.Tag, ModDownload.DlOptiFineListEntry)
            Me.ShowComparison(Me.PanOptiFineCompare, Me.PanOptiFineInfo, Me.InitialTarget.OptiFineVersion, selectedComponents.OptiFineVersion, Me.LabOptiFine.Text, Me.BtnOptiFineRestore, Me.InitialTarget.OptiFineEntry Is Nothing OrElse (flag AndAlso Me.CardOptiFine.MainSwap.Visibility = Visibility.Visible AndAlso dlOptiFineListEntry IsNot Nothing AndAlso (Me.SelectedForge Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(dlOptiFineListEntry, Me.SelectedForge))), Me.BtnOptiFineClear)
            Me.ShowComparison(Me.PanFabricApiCompare, Me.PanFabricApiInfo, Me.InitialTarget.FabricApiVersion, selectedComponents.FabricApiVersion, Me.LabFabricApi.Text, Me.BtnFabricApiRestore, Me.InitialTarget.FabricApiVersion Is Nothing OrElse (flag AndAlso Me.SelectedFabric IsNot Nothing AndAlso Me.CardFabricApi.MainSwap.Visibility = Visibility.Visible), Me.BtnFabricApiClear)
            Me.ShowComparison(Me.PanOptiFabricCompare, Me.PanOptiFabricInfo, Me.InitialTarget.OptiFabricVersion, selectedComponents.OptiFabricVersion, Me.LabOptiFabric.Text, Me.BtnOptiFabricRestore, Me.InitialTarget.OptiFabricVersion Is Nothing OrElse (flag AndAlso Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedOptiFine IsNot Nothing AndAlso Me.CardOptiFabric.MainSwap.Visibility = Visibility.Visible), Me.BtnOptiFabricClear)
            For Each valueTuple As ValueTuple(Of ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton) In New ValueTuple(Of ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton)() { New ValueTuple(Of ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton)(ModLoaders.Forge, Me.CardForge, Me.PanForgeCompare, Me.PanForgeInfo, Me.LabForge, Me.BtnForgeRestore, Me.BtnForgeClear), New ValueTuple(Of ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton)(ModLoaders.NeoForge, Me.CardNeoForge, Me.PanNeoForgeCompare, Me.PanNeoForgeInfo, Me.LabNeoForge, Me.BtnNeoForgeRestore, Me.BtnNeoForgeClear), New ValueTuple(Of ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton)(ModLoaders.Fabric, Me.CardFabric, Me.PanFabricCompare, Me.PanFabricInfo, Me.LabFabric, Me.BtnFabricRestore, Me.BtnFabricClear), New ValueTuple(Of ModLoaders, MyCard, Grid, Grid, TextBlock, MyIconButton, MyIconButton)(ModLoaders.LiteLoader, Me.CardLiteLoader, Me.PanLiteLoaderCompare, Me.PanLiteLoaderInfo, Me.LabLiteLoader, Me.BtnLiteLoaderRestore, Me.BtnLiteLoaderClear) }
                Dim CS$<>8__locals1 As PageDownloadModify._Closure$__18-0 = New PageDownloadModify._Closure$__18-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Local_OldVersion = Nothing
                Dim text As String = Nothing
                Me.InitialTarget.ModLoaderVersions.TryGetValue(valueTuple.Item1, CS$<>8__locals1.$VB$Local_OldVersion)
                selectedComponents.ModLoaderVersions.TryGetValue(valueTuple.Item1, text)
                Dim flag2 As Boolean = CS$<>8__locals1.$VB$Local_OldVersion Is Nothing OrElse (flag AndAlso valueTuple.Item2.MainSwap.Visibility = Visibility.Visible AndAlso valueTuple.Item6.Tag IsNot Nothing)
                If valueTuple.Item1 = ModLoaders.Forge AndAlso CS$<>8__locals1.$VB$Local_OldVersion IsNot Nothing AndAlso flag2 Then
                    flag2 = Me.SelectedOptiFine Is Nothing OrElse PageDownloadInstall.IsOptiFineSuitForForge(Me.SelectedOptiFine, CType(valueTuple.Item6.Tag, ModDownload.DlForgeVersionEntry))
                End If
                If valueTuple.Item1 = ModLoaders.Fabric AndAlso CS$<>8__locals1.$VB$Local_OldVersion IsNot Nothing Then
                    Dim CS$<>8__locals2 As PageDownloadModify._Closure$__18-1 = New PageDownloadModify._Closure$__18-1(CS$<>8__locals2)
                    CS$<>8__locals2.$VB$Local_TargetVersion = Me.VanillaName.Replace("∞", "infinite").Replace("Combat Test 7c", "1.16_combat-3")
                    flag2 = Me.CardFabric.Visibility = Visibility.Visible AndAlso Me.CardFabric.MainSwap.Visibility = Visibility.Visible AndAlso ModDownload.DlFabricListLoader.Output.Value("loader").OfType().Any(Function(v As JObject) Operators.CompareString(v("version").ToString(), CS$<>8__locals1.$VB$Local_OldVersion, False) = 0) AndAlso ModDownload.DlFabricListLoader.Output.Value("game").OfType().Any(Function(v As JObject) Operators.CompareString(v("version").ToString(), CS$<>8__locals2.$VB$Local_TargetVersion, False) = 0)
                End If
                Me.ShowComparison(valueTuple.Item3, valueTuple.Item4, CS$<>8__locals1.$VB$Local_OldVersion, text, valueTuple.Item5.Text, valueTuple.Item6, flag2, valueTuple.Item7)
            Next
            Me.UpdateStartButton()
        End Sub

        Private Sub ShowComparison(ComparePanel As Grid, CurrentPanel As FrameworkElement, OldVersion As String, NewVersion As String, NewText As String, RestoreButton As MyIconButton, CanRestore As Boolean, Optional ClearButton As FrameworkElement = Nothing)
            Dim text As String = Nothing
            If NewVersion Is Nothing Then
                If RestoreButton Is Me.BtnOptiFineRestore Then
                    text = MyBase.LoadOptiFineGetError()
                ElseIf RestoreButton Is Me.BtnForgeRestore Then
                    text = If(McVersion.IsStandardFormat(Me.VanillaName), MyBase.LoadForgeGetError(), "无")
                ElseIf RestoreButton Is Me.BtnNeoForgeRestore Then
                    text = If((Me.CardNeoForge.Visibility = Visibility.Collapsed), "无", MyBase.LoadNeoForgeGetError())
                ElseIf RestoreButton Is Me.BtnFabricRestore Then
                    text = If((Me.CardFabric.Visibility = Visibility.Collapsed), "无", MyBase.LoadFabricGetError())
                ElseIf RestoreButton Is Me.BtnLiteLoaderRestore Then
                    text = If((Me.CardLiteLoader.Visibility = Visibility.Collapsed), "无", MyBase.LoadLiteLoaderGetError())
                ElseIf RestoreButton Is Me.BtnFabricApiRestore Then
                    text = MyBase.LoadFabricApiGetError()
                ElseIf RestoreButton Is Me.BtnOptiFabricRestore Then
                    text = MyBase.LoadOptiFabricGetError()
                End If
                NewText = If(text, "可以添加")
            End If
            Dim flag As Boolean = NewVersion Is Nothing AndAlso (Operators.CompareString(NewText, "获取中……", False) = 0 OrElse NewText.StartsWithF("获取失败：", False))
            Dim flag2 As Boolean = Operators.CompareString(OldVersion, NewVersion, False) <> 0 AndAlso Not flag
            CanRestore = flag2 AndAlso OldVersion IsNot Nothing AndAlso CanRestore
            ComparePanel.Visibility = If(flag2, Visibility.Visible, Visibility.Collapsed)
            CurrentPanel.Visibility = If(flag2, Visibility.Collapsed, Visibility.Visible)
            RestoreButton.Visibility = If(CanRestore, Visibility.Visible, Visibility.Collapsed)
            If ClearButton IsNot Nothing Then
                ClearButton.ToolTip = If((OldVersion Is Nothing), "取消安装", "移除")
            End If
            Dim flag3 As Boolean = ClearButton IsNot Nothing AndAlso ClearButton.Visibility = Visibility.Visible
            RestoreButton.Margin = New Thickness(0.0, 5.0, CDbl(If((CanRestore AndAlso flag3), 60, 32)), 0.0)
            ComparePanel.Margin = New Thickness(ComparePanel.Margin.Left, ComparePanel.Margin.Top, CDbl(If((CanRestore AndAlso flag3), 110, 80)), ComparePanel.Margin.Bottom)
            If flag2 Then
                If OldVersion IsNot Nothing Then
                    Dim myCard As MyCard = CType(ComparePanel.Parent, MyCard)
                    If myCard.Visibility = Visibility.Collapsed Then
                        myCard.IsSwapped = True
                        If myCard.MainSwap IsNot Nothing Then
                            myCard.MainSwap.Visibility = Visibility.Collapsed
                        End If
                        myCard.Visibility = Visibility.Visible
                    End If
                End If
                Dim dockPanel As DockPanel = CType(ComparePanel.Children(0), DockPanel)
                Dim dockPanel2 As DockPanel = CType(ComparePanel.Children(2), DockPanel)
                dockPanel.Children(0).Visibility = If((OldVersion Is Nothing), Visibility.Collapsed, Visibility.Visible)
                CType(dockPanel.Children(1), TextBlock).Margin = If((OldVersion Is Nothing), New Thickness(0.0), New Thickness(7.0, 0.0, 0.0, 0.0))
                dockPanel2.Children(0).Visibility = If((NewVersion Is Nothing), Visibility.Collapsed, Visibility.Visible)
                Dim textBlock As TextBlock = CType(dockPanel2.Children(1), TextBlock)
                textBlock.Margin = If((NewVersion Is Nothing), New Thickness(0.0), New Thickness(7.0, 0.0, 0.0, 0.0))
                textBlock.Text = If((NewVersion Is Nothing), "移除", NewText)
                If NewVersion Is Nothing AndAlso text IsNot Nothing Then
                    textBlock.Inlines.Add(New Run(String.Format("（{0}）", If((Operators.CompareString(text, "无", False) = 0), "无可用版本", text))) With { .Foreground = ModSecret.ColorGray4 })
                End If
            End If
        End Sub

        Private Sub RestoreComponent(sender As Object, e As EventArgs)
            Dim myIconButton As MyIconButton = CType(sender, MyIconButton)
            If myIconButton Is Me.BtnMinecraftRestore Then
                Me.IsRestoringComponents = True
                Try
                    Me.PendingComponents.Clear()
                    Me.MinecraftSelected(ModDownloadLib.McDownloadListItem(CType(myIconButton.Tag, JObject), Nothing, False).Init(), Nothing)
                    If Me.ForgeVersionLoader IsNot Nothing Then
                        Me.LoadForge.State = Me.ForgeVersionLoader
                    End If
                    Me.SelectedOptiFine = TryCast(Me.BtnOptiFineRestore.Tag, ModDownload.DlOptiFineListEntry)
                    Me.SelectedForge = TryCast(Me.BtnForgeRestore.Tag, ModDownload.DlForgeVersionEntry)
                    Me.SelectedNeoForge = TryCast(Me.BtnNeoForgeRestore.Tag, ModDownload.DlNeoForgeListEntry)
                    Me.SelectedFabric = TryCast(Me.BtnFabricRestore.Tag, String)
                    Me.SelectedLiteLoader = TryCast(Me.BtnLiteLoaderRestore.Tag, ModDownload.DlLiteLoaderListEntry)
                    Me.SelectedFabricApi = TryCast(Me.BtnFabricApiRestore.Tag, ResourceVersion)
                    Me.SelectedOptiFabric = TryCast(Me.BtnOptiFabricRestore.Tag, ResourceVersion)
                    Me.AutoSelectedFabricApi = Me.SelectedFabric <> Nothing
                    Me.AutoSelectedOptiFabric = Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedOptiFine IsNot Nothing
                Finally
                    Me.IsRestoringComponents = False
                End Try
                MyBase.ReloadComponentLists()
                Return
            End If
            If myIconButton Is Me.BtnOptiFineRestore Then
                Me.SelectedOptiFine = TryCast(myIconButton.Tag, ModDownload.DlOptiFineListEntry)
                Me.AutoSelectedOptiFabric = Me.SelectedOptiFine IsNot Nothing AndAlso Me.SelectedFabric <> Nothing
                If Me.SelectedOptiFine Is Nothing Then
                    Me.SelectedOptiFabric = Nothing
                End If
            ElseIf myIconButton Is Me.BtnForgeRestore Then
                Me.SelectedForge = TryCast(myIconButton.Tag, ModDownload.DlForgeVersionEntry)
            ElseIf myIconButton Is Me.BtnNeoForgeRestore Then
                Me.SelectedNeoForge = TryCast(myIconButton.Tag, ModDownload.DlNeoForgeListEntry)
            ElseIf myIconButton Is Me.BtnFabricRestore Then
                Me.SelectedFabric = TryCast(myIconButton.Tag, String)
                Me.AutoSelectedFabricApi = Me.SelectedFabric <> Nothing
                Me.AutoSelectedOptiFabric = Me.SelectedFabric IsNot Nothing AndAlso Me.SelectedOptiFine IsNot Nothing
                If Me.SelectedFabric Is Nothing Then
                    Me.SelectedFabricApi = Nothing
                    Me.SelectedOptiFabric = Nothing
                End If
            ElseIf myIconButton Is Me.BtnFabricApiRestore Then
                Me.SelectedFabricApi = TryCast(myIconButton.Tag, ResourceVersion)
                Me.AutoSelectedFabricApi = True
            ElseIf myIconButton Is Me.BtnOptiFabricRestore Then
                Me.SelectedOptiFabric = TryCast(myIconButton.Tag, ResourceVersion)
                Me.AutoSelectedOptiFabric = True
            ElseIf myIconButton Is Me.BtnLiteLoaderRestore Then
                Me.SelectedLiteLoader = TryCast(myIconButton.Tag, ModDownload.DlLiteLoaderListEntry)
            End If
            CType(myIconButton.Parent, MyCard).IsSwapped = True
            MyBase.ReloadComponentLists()
        End Sub

        Private Sub CardMinecraft_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
            Me.ShowMinecraftVersionList()
            e.Handled = True
        End Sub

        Private Sub UpdateStartButton()
            Me.BtnStart.IsEnabled = Me.TextSelectName.IsValidated AndAlso Me.InitialTarget IsNot Nothing AndAlso Not Me.PendingComponents.Any() AndAlso (Not Me.IsVanillaVersionReliable OrElse Not Me.InitialTarget.IsEquivalentTo(MyBase.GetSelectedComponents()) OrElse Not String.Equals(Me.TextSelectName.Text, Me.OriginalName, StringComparison.Ordinal))
        End Sub

        Private Sub UpdateOriginalNameVisibility()
            Dim flag As Boolean = Not String.Equals(Me.TextSelectName.Text, Me.OriginalName, StringComparison.Ordinal)
            Dim visibility As Visibility = If(flag, Visibility.Visible, Visibility.Collapsed)
            If Me.LabSelectName.Visibility <> visibility Then
                Me.LabSelectName.Visibility = visibility
                Me.BtnOriginalNameRestore.Visibility = visibility
                Dim num As Integer = If(flag, 40, 16)
                If Me.IsInSelectPage AndAlso ModAnimation.AniControlEnabled = 0 Then
                    ModAnimation.AniStart(ModAnimation.AaDouble(Sub(Delta As Object)
                        Me.TextSelectName.Margin = New Thickness(56.0, 16.0, Conversions.ToDouble(Operators.AddObject(Me.TextSelectName.Margin.Right, Delta)), 0.0)
                    End Sub, CDbl(num) - Me.TextSelectName.Margin.Right, 150, 0, New ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), False), "DownloadModify Name Margin", False)
                    Return
                End If
                ModAnimation.AniStop("DownloadModify Name Margin")
                Me.TextSelectName.Margin = New Thickness(56.0, 16.0, CDbl(num), 0.0)
            End If
        End Sub

        Private Sub BtnOriginalNameRestore_Click(sender As Object, e As EventArgs)
            Me.TextSelectName.Text = Me.OriginalName
            Me.IsSelectNameEdited = Not String.Equals(Me.OriginalName, MyBase.GetSelectName(), StringComparison.Ordinal)
        End Sub

        Public Overrides Sub StartInstall(Request As ModDownloadLib.McInstallRequest)
            Me.UpdateStartButton()
            If Me.BtnStart.IsEnabled Then
                Dim input As ModMinecraft.McInstance = Me.InitLoader.Input
                Dim components As McComponents = Request.Components
                Dim newInstanceName As String = Request.NewInstanceName
                Request.NewInstanceName = If((Not String.Equals(newInstanceName, input.Name, StringComparison.Ordinal)), newInstanceName, Nothing)
                Request.InstanceFolder = input.PathVersion
                Dim list As List(Of String) = New List(Of String)()
                If(Me.SelectedForge IsNot Nothing OrElse Me.SelectedNeoForge IsNot Nothing OrElse Me.SelectedFabric IsNot Nothing) AndAlso Not PathUtils.IsSame(input.PathIndie, input.PathVersion) Then
                    list.Add("该版本尚未开启版本隔离，可能与别的 MC 版本共用同一个 Mod 和存档文件夹。" & vbCrLf & "修改版本后，已有的 Mod、存档等需要手动迁移到新的游戏目录。")
                End If
                Dim flag As Boolean = Operators.CompareString(Me.InitialTarget.MinecraftVersion, components.MinecraftVersion, False) <> 0
                If Not Me.IsVanillaVersionReliable AndAlso Not flag Then
                    list.Add(String.Format("PCL 未能精准识别该版本，请确认它是不是 Minecraft {0}，如果不正确，请取消并选择正确的版本！", Me.InitialTarget.MinecraftVersion))
                End If
                If Not Configs.HintModifyInstanceWarning.[Get]() Then
                    list.Add("如果该版本安装了 PCL 不兼容的加载器，或经过第三方修改，这些改动将会丢失！")
                End If
                Dim flag2 As Boolean = False
                Try
                    For Each modLoaders As ModLoaders In Me.InitialTarget.ModLoaderVersions.Keys.Union(components.ModLoaderVersions.Keys)
                        Dim text As String = Nothing
                        Dim text2 As String = Nothing
                        Dim flag3 As Boolean = Me.InitialTarget.ModLoaderVersions.TryGetValue(modLoaders, text)
                        Dim flag4 As Boolean = components.ModLoaderVersions.TryGetValue(modLoaders, text2)
                        If flag3 <> flag4 OrElse Operators.CompareString(text, text2, False) <> 0 Then
                            flag2 = True
                            Exit For
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of ModLoaders)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If(flag2 OrElse flag) AndAlso Me.InitialTarget.ModLoaderVersions.Any() Then
                    list.Add("现有的 Mod、资源包等在修改后可能会不兼容！")
                End If
                If Not list.Any() OrElse ModMain.MyMsgBox("请注意：" & vbCrLf & "• " + list.Join(vbCrLf & "• "), "修改确认", "开始修改", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                    If Not Configs.HintModifyInstanceWarning.[Get]() Then
                        Configs.HintModifyInstanceWarning.[Set](True)
                    End If
                    If Me.IsVanillaVersionReliable AndAlso Me.InitialTarget.IsEquivalentTo(components) Then
                        Try
                            Dim pathVersion As String = input.PathVersion
                            Dim mcInstance As ModMinecraft.McInstance = input.Rename(newInstanceName)
                            If PageInstanceLeft.Instance IsNot Nothing AndAlso PathUtils.IsSame(PageInstanceLeft.Instance.PathVersion, pathVersion) Then
                                PageInstanceLeft.Instance = mcInstance
                            End If
                            If ModMinecraft.McInstanceSelected IsNot Nothing AndAlso PathUtils.IsSame(ModMinecraft.McInstanceSelected.PathVersion, pathVersion) Then
                                ModMinecraft.McInstanceSelected = mcInstance
                            End If
                            PageInstanceLeft.ReloadCurrentJava()
                            ModMain.Hint("重命名成功！", ModMain.HintType.Green, True)
                            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                            ModMain.FrmMain.PageBack()
                            Return
                        Catch ex As Exception
                            Logger.[Error](ex, "重命名版本失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Pages\PageDownload\PageDownloadModify.vb")
                            Return
                        End Try
                    End If
                    If ModDownloadLib.McModificationStart(Request, input) Then
                        ModMain.FrmMain.PageBack()
                    End If
                End If
            End If
        End Sub

        Friend Overrides Property BtnMinecraftRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnMinecraftRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnMinecraftRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnMinecraftRestore = value
                myIconButton = MyBase.BtnMinecraftRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property BtnOptiFineRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnOptiFineRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnOptiFineRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnOptiFineRestore = value
                myIconButton = MyBase.BtnOptiFineRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property BtnForgeRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnForgeRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnForgeRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnForgeRestore = value
                myIconButton = MyBase.BtnForgeRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property BtnNeoForgeRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnNeoForgeRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnNeoForgeRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnNeoForgeRestore = value
                myIconButton = MyBase.BtnNeoForgeRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property BtnFabricRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnFabricRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnFabricRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnFabricRestore = value
                myIconButton = MyBase.BtnFabricRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property BtnFabricApiRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnFabricApiRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnFabricApiRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnFabricApiRestore = value
                myIconButton = MyBase.BtnFabricApiRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property BtnOptiFabricRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnOptiFabricRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnOptiFabricRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnOptiFabricRestore = value
                myIconButton = MyBase.BtnOptiFabricRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property BtnLiteLoaderRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnLiteLoaderRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.RestoreComponent
                Dim myIconButton As MyIconButton = MyBase.BtnLiteLoaderRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnLiteLoaderRestore = value
                myIconButton = MyBase.BtnLiteLoaderRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property CardMinecraft As MyCard
            <CompilerGenerated()>
            Get
                Return MyBase.CardMinecraft
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyCard)
                Dim mouseButtonEventHandler As MouseButtonEventHandler = AddressOf Me.CardMinecraft_MouseLeftButtonUp
                Dim myCard As MyCard = MyBase.CardMinecraft
                If myCard IsNot Nothing Then
                    RemoveHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler
                End If
                MyBase.CardMinecraft = value
                myCard = MyBase.CardMinecraft
                If myCard IsNot Nothing Then
                    AddHandler myCard.MouseLeftButtonUp, mouseButtonEventHandler
                End If
            End Set
        End Property

        Friend Overrides Property TextSelectName As MyTextBox
            <CompilerGenerated()>
            Get
                Return MyBase.TextSelectName
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyTextBox)
                Dim textChangedEventHandler As TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.UpdateStartButton()
                End Sub
                Dim validateChangedEventHandler As MyTextBox.ValidateChangedEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    Me.UpdateStartButton()
                End Sub
                Dim textChangedEventHandler2 As TextChangedEventHandler = Sub(a0 As Object, a1 As TextChangedEventArgs)
                    Me.UpdateOriginalNameVisibility()
                End Sub
                Dim myTextBox As MyTextBox = MyBase.TextSelectName
                If myTextBox IsNot Nothing Then
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler
                    RemoveHandler MyTextBox.ValidateChanged, validateChangedEventHandler
                    RemoveHandler myTextBox.TextChanged, textChangedEventHandler2
                End If
                MyBase.TextSelectName = value
                myTextBox = MyBase.TextSelectName
                If myTextBox IsNot Nothing Then
                    AddHandler myTextBox.TextChanged, textChangedEventHandler
                    AddHandler MyTextBox.ValidateChanged, validateChangedEventHandler
                    AddHandler myTextBox.TextChanged, textChangedEventHandler2
                End If
            End Set
        End Property

        Friend Overrides Property BtnOriginalNameRestore As MyIconButton
            <CompilerGenerated()>
            Get
                Return MyBase.BtnOriginalNameRestore
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As MyIconButton)
                Dim clickEventHandler As MyIconButton.ClickEventHandler = AddressOf Me.BtnOriginalNameRestore_Click
                Dim myIconButton As MyIconButton = MyBase.BtnOriginalNameRestore
                If myIconButton IsNot Nothing Then
                    RemoveHandler myIconButton.Click, clickEventHandler
                End If
                MyBase.BtnOriginalNameRestore = value
                myIconButton = MyBase.BtnOriginalNameRestore
                If myIconButton IsNot Nothing Then
                    AddHandler myIconButton.Click, clickEventHandler
                End If
            End Set
        End Property

        Private InitLoader As ModLoader.LoaderTask(Of ModMinecraft.McInstance, Object)

        Private ForgeVersionLoader As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))

        Private InitialTarget As McComponents

        Private IsVanillaVersionReliable As Boolean

        Private OriginalName As String

        Private CustomLogo As String

        Private PendingComponents As Dictionary(Of MyLoading, String)

        Private IsRestoringComponents As Boolean

        Private $STATIC$Init$2001$IsLoad As Boolean

        Private $STATIC$Init$2001$IsLoad$Init As StaticLocalInitFlag

        Private $STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes As JObject

        Private $STATIC$LoadInstance$2011151286FC21287E81C$OptiFineHashes$Init As StaticLocalInitFlag
    End Class
End Namespace
