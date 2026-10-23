Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public NotInheritable Module ModDownload
        Public Function DlClientJarGet(Instance As ModMinecraft.McInstance, ReturnNothingOnFileUseable As Boolean) As ModNet.NetFile
            Try
                While Not String.IsNullOrEmpty(Instance.InheritName)
                    Instance = New ModMinecraft.McInstance(Instance.InheritName)
                End While
            Catch ex As Exception
                Logger.Warn(ex, "获取底层继承版本失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
            End Try
            If Instance.JsonObject("downloads") Is Nothing OrElse Instance.JsonObject("downloads")("client") Is Nothing OrElse Instance.JsonObject("downloads")("client")("url") Is Nothing Then
                Throw New Exception("底层版本 " + Instance.Name + " 中无 jar 文件下载信息")
            End If
            Dim fileChecker As FileChecker = New FileChecker() With { .MinSize = 1024L, .ActualSize = CLng((If(Instance.JsonObject("downloads")("client")("size"), (-1)))), .Hash = CStr(Instance.JsonObject("downloads")("client")("sha1")) }
            Dim netFile As ModNet.NetFile
            If ReturnNothingOnFileUseable AndAlso fileChecker.Check(Instance.PathVersion + Instance.Name + ".jar") Is Nothing Then
                netFile = Nothing
            Else
                Dim text As String = CStr(Instance.JsonObject("downloads")("client")("url"))
                netFile = New ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), Instance.PathVersion + Instance.Name + ".jar", fileChecker, False)
            End If
            Return netFile
        End Function

        Public Function DlClientAssetIndexGet(Instance As ModMinecraft.McInstance) As ModNet.NetFile
            While Not String.IsNullOrEmpty(Instance.InheritName)
                Instance = New ModMinecraft.McInstance(Instance.InheritName)
            End While
            Dim jtoken As JToken = ModMinecraft.McAssetsGetIndex(Instance, True, True)
            Dim text As String = ModMinecraft.McFolderSelected + "assets\indexes\" + jtoken("id").ToString() + ".json"
            Logger.Info(String.Format("版本 {0} 对应的资源文件索引为 {1}", Instance.Name, jtoken("id")), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
            Dim text2 As String = CStr((If(jtoken("url"), "")))
            Dim netFile As ModNet.NetFile
            If Operators.CompareString(text2, "", False) = 0 Then
                netFile = Nothing
            Else
                netFile = New ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text2), text, New FileChecker() With { .IsJson = True }, False)
            End If
            Return netFile
        End Function

        Public Function DlClientFix(Instance As ModMinecraft.McInstance, CheckAssetsHash As Boolean, DownloadAssetIndexInBackground As Boolean) As List(Of ModLoader.LoaderBase)
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim list2 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)() From { New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("分析缺失支持库文件", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Task.Output = ModMinecraft.McLibNetFilesFromInstance(Instance)
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 }, New ModNet.LoaderDownload("下载支持库文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 15.0 } }
            list.Add(New ModLoader.LoaderCombo(Of String)("下载支持库文件（主加载器）", list2) With { .Block = False, .Show = False, .ProgressWeight = 16.0 })
            If Conversions.ToBoolean(ModMinecraft.ShouldIgnoreFileCheck(Instance)) Then
                Logger.Warn("设置要求跳过所有 Assets 检查，文件可能不是最新的", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
            Else
                Dim list3 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                Dim TempAddressMain As String = Nothing
                Dim RealAddressMain As String = Nothing
                list3.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("分析资源文件索引地址", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                    Try
                        Dim netFile As ModNet.NetFile = ModDownload.DlClientAssetIndexGet(Instance)
                        If netFile Is Nothing Then
                            Task.Output = New List(Of ModNet.NetFile)()
                            Logger.Warn(String.Format("未找到版本 {0} 的合适的资源索引下载地址，游戏 assets 可能缺失", Instance.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                        Else
                            Dim text As String = ModBase.PathTemp + "Cache\" + netFile.LocalName + "_background"
                            If FileUtils.Exists(text) AndAlso DateTime.Compare(FileUtils.GetInfo(text).LastWriteTime.[Date], FileUtils.GetInfo(netFile.LocalPath).LastWriteTime.[Date]) > 0 Then
                                Logger.Info(String.Format("发现后台更新的资源索引缓存文件：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                                FileUtils.Move(text, netFile.LocalPath, False)
                            End If
                            If DownloadAssetIndexInBackground Then
                                If netFile.Check.Check(netFile.LocalPath) Is Nothing Then
                                    Task.Output = New List(Of ModNet.NetFile)()
                                Else
                                    Task.Output = New List(Of ModNet.NetFile)() From { netFile }
                                End If
                            Else
                                RealAddressMain = netFile.LocalPath
                                TempAddressMain = ModBase.PathTemp + "Cache\" + netFile.LocalName
                                netFile.LocalPath = TempAddressMain
                                Task.Output = New List(Of ModNet.NetFile)() From { netFile }
                            End If
                        End If
                    Catch ex As Exception
                        Throw New Exception("分析资源文件索引地址失败", ex)
                    End Try
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.5, .Show = False })
                list3.Add(New ModNet.LoaderDownload("下载资源文件索引", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 2.0 })
                list3.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("获取资源文件列表", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                    If TempAddressMain IsNot Nothing Then
                        FileUtils.Move(TempAddressMain, RealAddressMain, False)
                    End If
                    Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)) = Task
                    Dim $VB$Local_Instance As ModMinecraft.McInstance = Instance
                    Dim $VB$Local_CheckAssetsHash As Boolean = CheckAssetsHash
                    Dim loaderBase As ModLoader.LoaderBase = Task
                    Dim list4 As List(Of ModNet.NetFile) = ModMinecraft.McAssetsFixList($VB$Local_Instance, $VB$Local_CheckAssetsHash, loaderBase)
                    Task = CType(loaderBase, ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                    loaderTask.Output = list4
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.01 })
                list3.Add(New ModNet.LoaderDownload("下载资源文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 25.0 })
                list.Add(New ModLoader.LoaderCombo(Of String)("下载资源文件（主加载器）", list3) With { .Block = False, .Show = False, .ProgressWeight = 27.0 })
                If DownloadAssetIndexInBackground Then
                    Dim RealAddressBg As String = Nothing
                    Dim TempAddressBg As String = Nothing
                    Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("后台更新资源文件索引", New List(Of ModLoader.LoaderBase)() From { New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("后台分析资源文件索引地址", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                        Dim netFile2 As ModNet.NetFile = ModDownload.DlClientAssetIndexGet(Instance)
                        If netFile2 Is Nothing Then
                            Logger.Warn(String.Format("未找到版本 {0} 的合适的资源索引下载地址，游戏 assets 可能缺失", Instance.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                            Task.Cancel()
                            Throw New OperationCanceledException()
                        End If
                        RealAddressBg = netFile2.LocalPath
                        TempAddressBg = ModBase.PathTemp + "Cache\" + netFile2.LocalName + "_background"
                        netFile2.LocalPath = TempAddressBg
                        Task.Output = New List(Of ModNet.NetFile)() From { netFile2 }
                        Dim [date] As DateTime = FileUtils.GetInfo(RealAddressBg).LastWriteTime.[Date]
                        If FileUtils.Exists(RealAddressBg) AndAlso Math.Abs(([date] - DateAndTime.Now.[Date]).TotalDays) < 1.0 Then
                            Logger.Info(String.Format("无需更新资源文件索引，上次更新于 {0}", [date]), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                            Task.Cancel()
                        End If
                    End Sub, Nothing, ThreadPriority.Normal), New ModNet.LoaderDownload("后台下载资源文件索引", New List(Of ModNet.NetFile)()) })
                    Logger.Info("开始后台检查资源文件索引", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                    loaderCombo.Start(Nothing, False)
                End If
            End If
            Return list
        End Function

        Public Property AllDrops As List(Of Integer)
            Get
                If ModDownload._AllDrops Is Nothing Then
                    Dim enumerable As IEnumerable(Of String) = Settings.[Get](Of String)("CacheDrops", Nothing).ToString().Split(",", True)
                    Dim func As Func(Of String, Integer)
                    If ModDownload._Closure$__.$I5-0 IsNot Nothing Then
                        func = ModDownload._Closure$__.$I5-0
                    Else
                        ' The following expression was wrapped in a checked-expression
                        Dim func2 As Func(Of String, Integer) = Function(d As String) CInt(Math.Round(ModBase.Val(d)))
                        func = func2
                        ModDownload._Closure$__.$I5-0 = func2
                    End If
                    ModDownload._AllDrops = enumerable.[Select](func).ToList()
                End If
                If Not ModDownload._AllDrops.Any() Then
                    Return Nothing
                End If
                Return ModDownload._AllDrops
            End Get
            Set(value As List(Of Integer))
                ModDownload._AllDrops = value
                Settings.[Set]("CacheDrops", value.Join(","c), Nothing)
            End Set
        End Property

        Private Sub DlClientListMain(Loader As ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult))
            Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing)
            If num <> 0 Then
                If num <> 1 Then
                    ModDownload.DlSourceLoader(Of String, ModDownload.DlClientListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer)(ModDownload.DlClientListMojangLoader, 60), New KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer)(ModDownload.DlClientListBmclapiLoader, 120) }, Loader.IsForceRestarting)
                Else
                    ModDownload.DlSourceLoader(Of String, ModDownload.DlClientListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer)(ModDownload.DlClientListMojangLoader, 5), New KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer)(ModDownload.DlClientListBmclapiLoader, 35) }, Loader.IsForceRestarting)
                End If
            Else
                ModDownload.DlSourceLoader(Of String, ModDownload.DlClientListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer)(ModDownload.DlClientListBmclapiLoader, 30), New KeyValuePair(Of ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult), Integer)(ModDownload.DlClientListMojangLoader, 90) }, Loader.IsForceRestarting)
            End If
            Dim list As List(Of Integer) = New List(Of Integer)()
            Try
                For Each jtoken As JToken In Loader.Output.Value("versions")
                    Dim text As String = CType(jtoken, JObject)("id").ToString()
                    If Not text.Contains("-") Then
                        Dim num2 As Integer = McVersion.VersionToDrop(text)
                        If num2 <> 209 Then
                            list.Add(num2)
                        End If
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of JToken)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Dim enumerable As IEnumerable(Of Integer) = list.Distinct()
            Dim func As Func(Of Integer, Integer)
            If ModDownload._Closure$__.$I10-0 IsNot Nothing Then
                func = ModDownload._Closure$__.$I10-0
            Else
                Dim func2 As Func(Of Integer, Integer) = Function(d As Integer) d
                func = func2
                ModDownload._Closure$__.$I10-0 = func2
            End If
            ModDownload.AllDrops = enumerable.OrderByDescending(func).ToList()
        End Sub

        Private Sub DlClientListMojangMain(Loader As ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult))
            Dim timeMs As Long = ModBase.GetTimeMs()
            Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://launchermeta.mojang.com/mc/game/version_manifest.json", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
            Try
                If CType(jobject("versions"), JArray).Count < 200 Then
                    Throw New Exception("获取到的版本列表长度不足（" + jobject.ToString() + "）")
                End If
                If Not ModDownload.DlPreferMojang Then
                    ' The following expression was wrapped in a checked-expression
                    Dim num As Long = ModBase.GetTimeMs() - timeMs
                    ModDownload.DlPreferMojang = num < 4000L
                    Logger.Info(String.Format("Mojang 官方源加载耗时：{0}ms，{1}", num, If(ModDownload.DlPreferMojang, "可优先使用官方源", "不优先使用官方源")), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                End If
                Loader.Output = New ModDownload.DlClientListResult() With { .IsOfficial = True, .SourceName = "Mojang 官方源", .Value = jobject }
                If ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init Is Nothing Then
                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init, New StaticLocalInitFlag(), Nothing)
                End If
                Dim flag As Boolean = False
                Try
                    Monitor.Enter(ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init, flag)
                    If ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init.State = 0S Then
                        ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init.State = 2S
                        ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted = False
                    ElseIf ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init.State = 2S Then
                        Throw New IncompleteInitialization()
                    End If
                Finally
                    ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init.State = 1S
                    If flag Then
                        Monitor.[Exit](ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init)
                    End If
                End Try
                Dim text As String = CStr(jobject("latest")("snapshot"))
                If Settings.[Get](Of Boolean)("ToolUpdateSnapshot", Nothing) AndAlso Operators.CompareString(Settings.[Get](Of String)("ToolUpdateSnapshotLast", Nothing), "", False) <> 0 AndAlso Operators.CompareString(Settings.[Get](Of String)("ToolUpdateSnapshotLast", Nothing), text, False) <> 0 AndAlso Not ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted Then
                    ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted = True
                    ModMinecraft.McDownloadClientUpdateHint(text, jobject)
                End If
                Settings.[Set]("ToolUpdateSnapshotLast", If(text, "Nothing"), Nothing)
                text = CStr(jobject("latest")("release"))
                If Settings.[Get](Of Boolean)("ToolUpdateRelease", Nothing) AndAlso Operators.CompareString(Settings.[Get](Of String)("ToolUpdateReleaseLast", Nothing), "", False) <> 0 AndAlso Operators.CompareString(Settings.[Get](Of String)("ToolUpdateReleaseLast", Nothing), text, False) <> 0 AndAlso Not ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted Then
                    ModDownload.$STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted = True
                    ModMinecraft.McDownloadClientUpdateHint(text, jobject)
                End If
                Settings.[Set]("ToolUpdateReleaseLast", text, Nothing)
            Catch ex As Exception
                Throw New Exception("Minecraft 官方源版本列表解析失败", ex)
            End Try
        End Sub

        Private Sub DlClientListBmclapiMain(Loader As ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult))
            Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/mc/game/version_manifest.json", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
            Try
                Dim jarray As JArray = CType(jobject("versions"), JArray)
                If jarray.Count < 200 Then
                    Throw New Exception("获取到的版本列表长度不足（" + jobject.ToString() + "）")
                End If
                If FileUtils.Exists(ModBase.PathTemp + "Cache\download.json") Then
                    jarray.Merge(FileUtils.ReadAsJson(ModBase.PathTemp + "Cache\download.json", Nothing, Nothing))
                End If
                If Not String.IsNullOrEmpty(Loader.Input) Then
                    Dim Id As String = Loader.Input
                    If ModDownload.DlClientListLoader.Output.Value IsNot Nothing AndAlso Not ModDownload.DlClientListLoader.Output.Value("versions").Any(Function(v As JToken) CStr(v("id")) = Id) Then
                        Throw New Exception("BMCLAPI 源未包含目标版本 " + Id)
                    End If
                End If
                Loader.Output = New ModDownload.DlClientListResult() With { .IsOfficial = False, .SourceName = "BMCLAPI", .Value = jobject }
            Catch ex As Exception
                Throw New Exception("Minecraft BMCLAPI 版本列表解析失败（" + jobject.ToString() + "）", ex)
            End Try
        End Sub

        Public Function DlClientListGet(Id As String) As Object
            Dim obj As Object
            Try
                Id = Id.Replace("_", "-")
                If Operators.CompareString(Id, "1.0", False) <> 0 AndAlso Id.EndsWithF(".0", False) Then
                    ' The following expression was wrapped in a checked-expression
                    Id = Strings.Left(Id, Id.Length - 2)
                End If
                Select Case ModDownload.DlClientListLoader.State
                    Case ModBase.LoadState.Waiting, ModBase.LoadState.Failed, ModBase.LoadState.Canceled
                        ModDownload.DlClientListLoader.WaitForExit(Id, Nothing, True, Nothing, Nothing)
                    Case ModBase.LoadState.Loading
                        ModDownload.DlClientListLoader.WaitForExit(Id, Nothing, False, Nothing, Nothing)
                    Case ModBase.LoadState.Finished
                        Try
                            For Each jtoken As JToken In ModDownload.DlClientListLoader.Output.Value("versions")
                                Dim jobject As JObject = CType(jtoken, JObject)
                                If CStr(jobject("id")) = Id Then
                                    Return jobject("url").ToString()
                                End If
                            Next
                        Finally
                            Dim enumerator As IEnumerator(Of JToken)
                            If enumerator IsNot Nothing Then
                                enumerator.Dispose()
                            End If
                        End Try
                        ModDownload.DlClientListLoader.WaitForExit(Id, Nothing, True, Nothing, Nothing)
                End Select
                Try
                    For Each jtoken2 As JToken In ModDownload.DlClientListLoader.Output.Value("versions")
                        Dim jobject2 As JObject = CType(jtoken2, JObject)
                        If CStr(jobject2("id")) = Id Then
                            Return jobject2("url").ToString()
                        End If
                    Next
                Finally
                    Dim enumerator2 As IEnumerator(Of JToken)
                    If enumerator2 IsNot Nothing Then
                        enumerator2.Dispose()
                    End If
                End Try
                Logger.Warn(String.Format("未发现版本 {0} 的 json 下载地址，版本列表返回为：{1}{2}", Id, vbCrLf, ModDownload.DlClientListLoader.Output.Value), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                obj = Nothing
            Catch ex As Exception
                Logger.Warn(ex, String.Format("获取版本 {0} 的 json 下载地址失败", Id), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModDownload.vb")
                obj = Nothing
            End Try
            Return obj
        End Function

        Private Sub DlOptiFineListMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult))
            Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing)
            If num = 0 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlOptiFineListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer)(ModDownload.DlOptiFineListBmclapiLoader, 30), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer)(ModDownload.DlOptiFineListOfficialLoader, 90) }, Loader.IsForceRestarting)
                Return
            End If
            If num <> 1 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlOptiFineListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer)(ModDownload.DlOptiFineListOfficialLoader, 60), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer)(ModDownload.DlOptiFineListBmclapiLoader, 120) }, Loader.IsForceRestarting)
                Return
            End If
            ModDownload.DlSourceLoader(Of Integer, ModDownload.DlOptiFineListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer)(ModDownload.DlOptiFineListOfficialLoader, 5), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult), Integer)(ModDownload.DlOptiFineListBmclapiLoader, 35) }, Loader.IsForceRestarting)
        End Sub

        Private Sub DlOptiFineListOfficialMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult))
            Dim text As String = ModNet.NetRequestByClientRetry("https://optifine.net/downloads", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, False, Encoding.[Default], False)
            If text.Length < 200 Then
                Throw New Exception("获取到的版本列表长度不足（" + text + "）")
            End If
            Try
                Dim list As List(Of String) = text.RegexSearch("(?<=colForge'>)[^<]*", RegexOptions.None).ToList()
                Dim list2 As List(Of String) = text.RegexSearch("(?<=colDate'>)[^<]+", RegexOptions.None).ToList()
                Dim list3 As List(Of String) = text.RegexSearch("(?<=OptiFine_)[0-9A-Za-z_.]+(?=.jar"")", RegexOptions.None).ToList()
                If list2.Count <> list3.Count Then
                    Throw New Exception("版本与发布时间数据无法对应")
                End If
                If list.Count <> list3.Count Then
                    Throw New Exception("版本与 Forge 兼容数据无法对应")
                End If
                If list2.Count < 10 Then
                    Throw New Exception("获取到的版本数量不足（" + text + "）")
                End If
                Dim list4 As List(Of ModDownload.DlOptiFineListEntry) = New List(Of ModDownload.DlOptiFineListEntry)()
                Dim num As Integer = list2.Count - 1
                For i As Integer = 0 To num
                    list3(i) = list3(i).Replace("_", " ")
                    Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry = New ModDownload.DlOptiFineListEntry() With { .DisplayName = list3(i).Replace("HD U ", "").Replace(".0 ", " "), .ReleaseTime = New String() { list2(i).Split(".", False)(2), list2(i).Split(".", False)(1), list2(i).Split(".", False)(0) }.Join("/"c), .IsPreview = list3(i).ContainsIgnoreCase("pre"), .Inherit = list3(i).ToString().Split(" ", False)(0), .FileName = If(list3(i).ContainsIgnoreCase("pre"), "preview_", "") + "OptiFine_" + list3(i).Replace(" ", "_") + ".jar", .RequiredForgeVersion = list(i).Replace("Forge ", "").Replace("#", "") }
                    If dlOptiFineListEntry.RequiredForgeVersion.Contains("N/A") Then
                        dlOptiFineListEntry.RequiredForgeVersion = Nothing
                    End If
                    dlOptiFineListEntry.InstanceName = dlOptiFineListEntry.Inherit + "-OptiFine_" + list3(i).AfterFirst(" ", False).Replace(" ", "_")
                    list4.Add(dlOptiFineListEntry)
                Next
                Loader.Output = New ModDownload.DlOptiFineListResult() With { .IsOfficial = True, .SourceName = "OptiFine 官方源", .Value = list4 }
            Catch ex As Exception
                Throw New Exception("OptiFine 官方源版本列表解析失败（" + text + "）", ex)
            End Try
        End Sub

        Private Sub DlOptiFineListBmclapiMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult))
            Dim jarray As JArray = CType(ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/optifine/versionList", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JArray)
            Try
                Dim list As List(Of ModDownload.DlOptiFineListEntry) = New List(Of ModDownload.DlOptiFineListEntry)()
                Try
                    For Each jtoken As JToken In jarray
                        Dim jobject As JObject = CType(jtoken, JObject)
                        Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry = New ModDownload.DlOptiFineListEntry() With { .DisplayName = (jobject("mcversion").ToString() + jobject("type").ToString().Replace("HD_U", "").Replace("_", " ") + " " + jobject("patch").ToString()).Replace(".0 ", " "), .ReleaseTime = "", .IsPreview = jobject("patch").ToString().ContainsIgnoreCase("pre"), .Inherit = jobject("mcversion").ToString(), .FileName = jobject("filename").ToString(), .RequiredForgeVersion = (If(jobject("forge"), "")).ToString().Replace("Forge ", "").Replace("#", "") }
                        If dlOptiFineListEntry.RequiredForgeVersion.Contains("N/A") Then
                            dlOptiFineListEntry.RequiredForgeVersion = Nothing
                        End If
                        dlOptiFineListEntry.InstanceName = dlOptiFineListEntry.Inherit + "-OptiFine_" + (jobject("type").ToString() + " " + jobject("patch").ToString()).Replace(".0 ", " ").Replace(" ", "_").Replace(dlOptiFineListEntry.Inherit + "_", "")
                        list.Add(dlOptiFineListEntry)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Loader.Output = New ModDownload.DlOptiFineListResult() With { .IsOfficial = False, .SourceName = "BMCLAPI", .Value = list }
            Catch ex As Exception
                Throw New Exception("OptiFine BMCLAPI 版本列表解析失败（" + jarray.ToString() + "）", ex)
            End Try
        End Sub

        Private Sub DlForgeListMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult))
            Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing)
            If num = 0 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlForgeListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer)(ModDownload.DlForgeListBmclapiLoader, 30), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer)(ModDownload.DlForgeListOfficialLoader, 90) }, Loader.IsForceRestarting)
                Return
            End If
            If num <> 1 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlForgeListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer)(ModDownload.DlForgeListOfficialLoader, 60), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer)(ModDownload.DlForgeListBmclapiLoader, 120) }, Loader.IsForceRestarting)
                Return
            End If
            ModDownload.DlSourceLoader(Of Integer, ModDownload.DlForgeListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer)(ModDownload.DlForgeListOfficialLoader, 5), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult), Integer)(ModDownload.DlForgeListBmclapiLoader, 35) }, Loader.IsForceRestarting)
        End Sub

        Private Sub DlForgeListOfficialMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult))
            Dim text As String = ModNet.NetRequestByClientRetry("https://files.minecraftforge.net/maven/net/minecraftforge/forge/index_1.2.4.html", Nothing, Nothing, Nothing, Nothing, "text/html", Nothing, False, Encoding.[Default], True)
            If text.Length < 200 Then
                Throw New Exception("获取到的版本列表长度不足（" + text + "）")
            End If
            Dim list As List(Of String) = text.RegexSearch("(?<=a href=""index_)[0-9.]+((_pre|[_-]snapshot[_-]?)[0-9]?)?(?=.html)", RegexOptions.None).ToList()
            list.Add("1.2.4")
            If list.Count < 10 Then
                Throw New Exception("获取到的版本数量不足（" + text + "）")
            End If
            Loader.Output = New ModDownload.DlForgeListResult() With { .IsOfficial = True, .SourceName = "Forge 官方源", .Value = list }
        End Sub

        Private Sub DlForgeListBmclapiMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult))
            Dim enumerable As IEnumerable(Of JToken) = CType(ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/forge/minecraft", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Encoding.[Default], False).DeserializeJson(), JArray)
            Dim func As Func(Of JToken, String)
            If ModDownload._Closure$__.$I30-0 IsNot Nothing Then
                func = ModDownload._Closure$__.$I30-0
            Else
                Dim func2 As Func(Of JToken, String) = Function(v As JToken) v.ToString()
                func = func2
                ModDownload._Closure$__.$I30-0 = func2
            End If
            Dim list As List(Of String) = enumerable.[Select](func).ToList()
            If list.Count < 10 Then
                Throw New Exception("获取到的版本数量不足")
            End If
            Loader.Output = New ModDownload.DlForgeListResult() With { .IsOfficial = False, .SourceName = "BMCLAPI", .Value = list }
        End Sub

        Public Sub DlForgeVersionMain(Loader As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)))
            Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)) = New ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))("DlForgeVersion Official", AddressOf ModDownload.DlForgeVersionOfficialMain, Nothing, ThreadPriority.Normal)
            Dim loaderTask2 As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)) = New ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))("DlForgeVersion Bmclapi", AddressOf ModDownload.DlForgeVersionBmclapiMain, Nothing, ThreadPriority.Normal)
            Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing)
            If num = 0 Then
                ModDownload.DlSourceLoader(Of String, List(Of ModDownload.DlForgeVersionEntry))(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer)(loaderTask2, 30), New KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer)(loaderTask, 90) }, Loader.IsForceRestarting)
                Return
            End If
            If num <> 1 Then
                ModDownload.DlSourceLoader(Of String, List(Of ModDownload.DlForgeVersionEntry))(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer)(loaderTask, 60), New KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer)(loaderTask2, 120) }, Loader.IsForceRestarting)
                Return
            End If
            ModDownload.DlSourceLoader(Of String, List(Of ModDownload.DlForgeVersionEntry))(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer)(loaderTask, 5), New KeyValuePair(Of ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)), Integer)(loaderTask2, 35) }, Loader.IsForceRestarting)
        End Sub

        Public Sub DlForgeVersionOfficialMain(Loader As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)))
            Dim text As String
            Try
                text = ModNet.NetRequestByLoader("https://files.minecraftforge.net/maven/net/minecraftforge/forge/index_" + Loader.Input.Replace("-", "_") + ".html", 45000, False, True)
            Catch ex As Exception
                If ex.GetDisplay(False).Contains("(404)") Then
                    Throw New Exception("无")
                End If
                Throw
            End Try
            If text.Length < 1000 Then
                Throw New Exception("获取到的版本列表长度不足（" + text + "）")
            End If
            Dim list As List(Of ModDownload.DlForgeVersionEntry) = New List(Of ModDownload.DlForgeVersionEntry)()
            Try
                Dim array As String() = text.Substring(0, text.LastIndexOfF("</table>", False)).Split("<td class=""download-version", False)
                Dim num As Integer = array.Count() - 1
                For i As Integer = 1 To num
                    Dim text2 As String = array(i)
                    Try
                        Dim text3 As String = text2.RegexSeek("(?<=[^(0-9)]+)[0-9\.]+", RegexOptions.None)
                        Dim flag As Boolean = text2.Contains("fa promo-recommended")
                        Dim input As String = Loader.Input
                        Dim text4 As String = text2.RegexSeek(String.Format("(?<=-{0}-)[^-""]+(?=-[a-z]+.[a-z]{{3}})", text3), RegexOptions.None)
                        If String.IsNullOrWhiteSpace(text4) Then
                            text4 = Nothing
                        End If
                        Dim array2 As String() = text2.RegexSeek("(?<=""download-time"" title="")[^""]+", RegexOptions.None).Split(" -:".ToCharArray())
                        Dim text5 As String = New DateTime(Conversions.ToInteger(array2(0)), Conversions.ToInteger(array2(1)), Conversions.ToInteger(array2(2)), Conversions.ToInteger(array2(3)), Conversions.ToInteger(array2(4)), Conversions.ToInteger(array2(5)), 0, DateTimeKind.Utc).ToLocalTime().ToString("yyyy'/'MM'/'dd HH':'mm")
                        Dim text6 As String
                        Dim text7 As String
                        If text2.Contains("classifier-installer""") Then
                            text2 = text2.Substring(text2.IndexOfF("installer.jar", False))
                            text6 = text2.RegexSeek("(?<=MD5:</strong> )[^<]+", RegexOptions.None)
                            text7 = "installer"
                        ElseIf text2.Contains("classifier-universal""") Then
                            text2 = text2.Substring(text2.IndexOfF("universal.zip", False))
                            text6 = text2.RegexSeek("(?<=MD5:</strong> )[^<]+", RegexOptions.None)
                            text7 = "universal"
                        Else
                            If Not text2.Contains("client.zip") Then
                                GoTo IL_028A
                            End If
                            text2 = text2.Substring(text2.IndexOfF("client.zip", False))
                            text6 = text2.RegexSeek("(?<=MD5:</strong> )[^<]+", RegexOptions.None)
                            text7 = "client"
                        End If
                        list.Add(New ModDownload.DlForgeVersionEntry(text3, text4, input) With { .Category = text7, .IsRecommended = flag, .Hash = text6.ReplaceLineEndings("", False), .ReleaseTime = text5 })
                    Catch ex2 As Exception
                        Throw New Exception("Forge 官方源版本信息提取失败（" + text2 + "）", ex2)
                    End Try
                    IL_028A:
                Next
            Catch ex3 As Exception
                Throw New Exception("Forge 官方源版本列表解析失败（" + text + "）", ex3)
            End Try
            If Not list.Any() Then
                Throw New Exception("无")
            End If
            Loader.Output = list
        End Sub

        Public Sub DlForgeVersionBmclapiMain(Loader As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)))
            Dim jarray As JArray = CType(ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/forge/minecraft/" + Loader.Input.Replace("-", "_"), Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JArray)
            Dim list As List(Of ModDownload.DlForgeVersionEntry) = New List(Of ModDownload.DlForgeVersionEntry)()
            Try
                Dim text As String = ModDownloadLib.McDownloadForgeRecommendedGet(Loader.Input)
                Try
                    For Each jtoken As JToken In jarray
                        Dim jobject As JObject = CType(jtoken, JObject)
                        Dim text2 As String = Nothing
                        Dim text3 As String = "unknown"
                        Dim num As Integer = -1
                        Try
                            For Each jtoken2 As JToken In jobject("files")
                                Dim jobject2 As JObject = CType(jtoken2, JObject)
                                Dim text4 As String = jobject2("category").ToString()
                                If Operators.CompareString(text4, "installer", False) <> 0 Then
                                    If Operators.CompareString(text4, "universal", False) <> 0 Then
                                        If Operators.CompareString(text4, "client", False) = 0 Then
                                            If num <= 0 AndAlso Operators.CompareString(jobject2("format").ToString(), "zip", False) = 0 Then
                                                text2 = CStr(jobject2("hash"))
                                                text3 = "client"
                                                num = 0
                                            End If
                                        End If
                                    ElseIf num <= 1 AndAlso Operators.CompareString(jobject2("format").ToString(), "zip", False) = 0 Then
                                        text2 = CStr(jobject2("hash"))
                                        text3 = "universal"
                                        num = 1
                                    End If
                                ElseIf Operators.CompareString(jobject2("format").ToString(), "jar", False) = 0 Then
                                    text2 = CStr(jobject2("hash"))
                                    text3 = "installer"
                                    num = 2
                                End If
                            Next
                        Finally
                            Dim enumerator2 As IEnumerator(Of JToken)
                            If enumerator2 IsNot Nothing Then
                                enumerator2.Dispose()
                            End If
                        End Try
                        Dim text5 As String = CStr(jobject("branch"))
                        Dim text6 As String = CStr(jobject("version"))
                        Dim dlForgeVersionEntry As ModDownload.DlForgeVersionEntry = New ModDownload.DlForgeVersionEntry(text6, text5, Loader.Input) With { .Hash = text2, .Category = text3, .IsRecommended = (Operators.CompareString(text, text6, False) = 0) }
                        jobject("modified").ToString().Split(New Char() { "-"c, "T"c, ":"c, "."c, " "c, "/"c })
                        dlForgeVersionEntry.ReleaseTime = jobject("modified").ToObject(Of DateTime)().ToLocalTime().ToString("yyyy'/'MM'/'dd HH':'mm")
                        list.Add(dlForgeVersionEntry)
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            Catch ex As Exception
                Throw New Exception("Forge BMCLAPI 版本列表解析失败（" + jarray.ToString() + "）", ex)
            End Try
            If Not list.Any() Then
                Throw New Exception("无")
            End If
            Loader.Output = list
        End Sub

        Private Sub DlNeoForgeListMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult))
            Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing)
            If num = 0 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlNeoForgeListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer)(ModDownload.DlNeoForgeListBmclapiLoader, 30), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer)(ModDownload.DlNeoForgeListOfficialLoader, 90) }, Loader.IsForceRestarting)
                Return
            End If
            If num <> 1 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlNeoForgeListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer)(ModDownload.DlNeoForgeListOfficialLoader, 60), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer)(ModDownload.DlNeoForgeListBmclapiLoader, 120) }, Loader.IsForceRestarting)
                Return
            End If
            ModDownload.DlSourceLoader(Of Integer, ModDownload.DlNeoForgeListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer)(ModDownload.DlNeoForgeListOfficialLoader, 5), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult), Integer)(ModDownload.DlNeoForgeListBmclapiLoader, 35) }, Loader.IsForceRestarting)
        End Sub

        Private Sub DlNeoForgeListOfficialMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult))
            Dim text As String = ModNet.NetRequestByClientRetry("https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/neoforge", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False)
            Dim text2 As String = ModNet.NetRequestByClientRetry("https://maven.neoforged.net/api/maven/versions/releases/net/neoforged/forge", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False)
            If text.Length < 100 OrElse text2.Length < 100 Then
                Throw New Exception("获取到的版本列表长度不足（" + text + "）")
            End If
            Try
                Loader.Output = New ModDownload.DlNeoForgeListResult() With { .IsOfficial = True, .SourceName = "NeoForge 官方源", .Value = ModDownload.GetNeoForgeEntries(text, text2) }
            Catch ex As Exception
                Throw New Exception(String.Concat(New String() { "NeoForge 官方源版本列表解析失败（", text, vbCrLf & vbCrLf, text2, "）" }), ex)
            End Try
        End Sub

        Public Sub DlNeoForgeListBmclapiMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult))
            Dim text As String = ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/neoforge/meta/api/maven/details/releases/net/neoforged/neoforge", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False)
            Dim text2 As String = ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/neoforge/meta/api/maven/details/releases/net/neoforged/forge", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False)
            If text.Length < 100 OrElse text2.Length < 100 Then
                Throw New Exception("获取到的版本列表长度不足（" + text + "）")
            End If
            Try
                Loader.Output = New ModDownload.DlNeoForgeListResult() With { .IsOfficial = True, .SourceName = "BMCLAPI", .Value = ModDownload.GetNeoForgeEntries(text, text2) }
            Catch ex As Exception
                Throw New Exception(String.Concat(New String() { "NeoForge BMCLAPI 版本列表解析失败（", text, vbCrLf & vbCrLf, text2, "）" }), ex)
            End Try
        End Sub

        Private Function GetNeoForgeEntries(LatestJson As String, LatestLegacyJson As String) As List(Of ModDownload.DlNeoForgeListEntry)
            Dim enumerable As IEnumerable(Of String) = (LatestLegacyJson + LatestJson).RegexSearch("(?<="")(1\.20\.1-)?\d+\.[^\.]+\.\d+(\.\d+)?(-(beta|alpha)(\.\d+)?)?(\+snapshot-\d+)?(?="")", RegexOptions.None)
            Dim func As Func(Of String, Boolean)
            If ModDownload._Closure$__.$I44-0 IsNot Nothing Then
                func = ModDownload._Closure$__.$I44-0
            Else
                Dim func2 As Func(Of String, Boolean) = Function(name As String) Operators.CompareString(name, "47.1.82", False) <> 0
                func = func2
                ModDownload._Closure$__.$I44-0 = func2
            End If
            Dim enumerable2 As IEnumerable(Of String) = enumerable.Where(func)
            Dim func3 As Func(Of String, ModDownload.DlNeoForgeListEntry)
            If ModDownload._Closure$__.$I44-1 IsNot Nothing Then
                func3 = ModDownload._Closure$__.$I44-1
            Else
                Dim func4 As Func(Of String, ModDownload.DlNeoForgeListEntry) = Function(name As String) New ModDownload.DlNeoForgeListEntry(name)
                func3 = func4
                ModDownload._Closure$__.$I44-1 = func4
            End If
            Dim enumerable3 As IEnumerable(Of ModDownload.DlNeoForgeListEntry) = enumerable2.[Select](func3)
            Dim func5 As Func(Of ModDownload.DlNeoForgeListEntry, ModDownload.DlNeoForgeListEntry)
            If ModDownload._Closure$__.$I44-2 IsNot Nothing Then
                func5 = ModDownload._Closure$__.$I44-2
            Else
                Dim func6 As Func(Of ModDownload.DlNeoForgeListEntry, ModDownload.DlNeoForgeListEntry) = Function(a As ModDownload.DlNeoForgeListEntry) a
                func5 = func6
                ModDownload._Closure$__.$I44-2 = func6
            End If
            Dim list As List(Of ModDownload.DlNeoForgeListEntry) = enumerable3.OrderByDescending(func5).ToList()
            If Not list.Any() Then
                Throw New Exception("无")
            End If
            Return list
        End Function

        Private Sub DlLiteLoaderListMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult))
            Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing)
            If num = 0 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlLiteLoaderListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer)(ModDownload.DlLiteLoaderListBmclapiLoader, 30), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer)(ModDownload.DlLiteLoaderListOfficialLoader, 90) }, Loader.IsForceRestarting)
                Return
            End If
            If num <> 1 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlLiteLoaderListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer)(ModDownload.DlLiteLoaderListOfficialLoader, 60), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer)(ModDownload.DlLiteLoaderListBmclapiLoader, 120) }, Loader.IsForceRestarting)
                Return
            End If
            ModDownload.DlSourceLoader(Of Integer, ModDownload.DlLiteLoaderListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer)(ModDownload.DlLiteLoaderListOfficialLoader, 5), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult), Integer)(ModDownload.DlLiteLoaderListBmclapiLoader, 35) }, Loader.IsForceRestarting)
        End Sub

        Private Sub DlLiteLoaderListOfficialMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult))
            Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://dl.liteloader.com/versions/versions.json", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
            Try
                Dim jobject2 As JObject = CType(jobject("versions"), JObject)
                Dim list As List(Of ModDownload.DlLiteLoaderListEntry) = New List(Of ModDownload.DlLiteLoaderListEntry)()
                Try
                    For Each keyValuePair As KeyValuePair(Of String, JToken) In jobject2
                        If Not keyValuePair.Key.StartsWithF("1.6", False) AndAlso Not keyValuePair.Key.StartsWithF("1.5", False) Then
                            Dim jtoken As JToken = (If(keyValuePair.Value("artefacts"), keyValuePair.Value("snapshots")))("com.mumfrey:liteloader")("latest")
                            list.Add(New ModDownload.DlLiteLoaderListEntry() With { .Inherit = keyValuePair.Key, .IsLegacy = (Conversions.ToDouble(keyValuePair.Key.Split(".", False)(1)) < 8.0), .IsPreview = (Operators.CompareString(jtoken("stream").ToString().Lower(), "snapshot", False) = 0), .FileName = "liteloader-installer-" + keyValuePair.Key + If((Operators.CompareString(keyValuePair.Key, "1.8", False) = 0 OrElse Operators.CompareString(keyValuePair.Key, "1.9", False) = 0), ".0", "") + "-00-SNAPSHOT.jar", .MD5 = CStr(jtoken("md5")), .ReleaseTime = ModBase.GetLocalTime(ModBase.GetDate(CInt(jtoken("timestamp")))).ToString("yyyy'/'MM'/'dd HH':'mm"), .JsonToken = jtoken })
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of KeyValuePair(Of String, JToken))
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Loader.Output = New ModDownload.DlLiteLoaderListResult() With { .IsOfficial = True, .SourceName = "LiteLoader 官方源", .Value = list }
            Catch ex As Exception
                Throw New Exception("LiteLoader 官方源版本列表解析失败（" + jobject.ToString() + "）", ex)
            End Try
        End Sub

        Private Sub DlLiteLoaderListBmclapiMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult))
            Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/maven/com/mumfrey/liteloader/versions.json", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
            Try
                Dim jobject2 As JObject = CType(jobject("versions"), JObject)
                Dim list As List(Of ModDownload.DlLiteLoaderListEntry) = New List(Of ModDownload.DlLiteLoaderListEntry)()
                Try
                    For Each keyValuePair As KeyValuePair(Of String, JToken) In jobject2
                        If Not keyValuePair.Key.StartsWithF("1.6", False) AndAlso Not keyValuePair.Key.StartsWithF("1.5", False) Then
                            Dim jtoken As JToken = (If(keyValuePair.Value("artefacts"), keyValuePair.Value("snapshots")))("com.mumfrey:liteloader")("latest")
                            list.Add(New ModDownload.DlLiteLoaderListEntry() With { .Inherit = keyValuePair.Key, .IsLegacy = (Conversions.ToDouble(keyValuePair.Key.Split(".", False)(1)) < 8.0), .IsPreview = (Operators.CompareString(jtoken("stream").ToString().Lower(), "snapshot", False) = 0), .FileName = "liteloader-installer-" + keyValuePair.Key + If((Operators.CompareString(keyValuePair.Key, "1.8", False) = 0 OrElse Operators.CompareString(keyValuePair.Key, "1.9", False) = 0), ".0", "") + "-00-SNAPSHOT.jar", .MD5 = CStr(jtoken("md5")), .ReleaseTime = ModBase.GetLocalTime(ModBase.GetDate(CInt(jtoken("timestamp")))).ToString("yyyy'/'MM'/'dd HH':'mm"), .JsonToken = jtoken })
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of KeyValuePair(Of String, JToken))
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Loader.Output = New ModDownload.DlLiteLoaderListResult() With { .IsOfficial = False, .SourceName = "BMCLAPI", .Value = list }
            Catch ex As Exception
                Throw New Exception("LiteLoader BMCLAPI 版本列表解析失败（" + jobject.ToString() + "）", ex)
            End Try
        End Sub

        Private Sub DlFabricListMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult))
            Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing)
            If num = 0 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlFabricListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer)(ModDownload.DlFabricListBmclapiLoader, 30), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer)(ModDownload.DlFabricListOfficialLoader, 90) }, Loader.IsForceRestarting)
                Return
            End If
            If num <> 1 Then
                ModDownload.DlSourceLoader(Of Integer, ModDownload.DlFabricListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer)(ModDownload.DlFabricListOfficialLoader, 60), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer)(ModDownload.DlFabricListBmclapiLoader, 120) }, Loader.IsForceRestarting)
                Return
            End If
            ModDownload.DlSourceLoader(Of Integer, ModDownload.DlFabricListResult)(Loader, New List(Of KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer))() From { New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer)(ModDownload.DlFabricListOfficialLoader, 5), New KeyValuePair(Of ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult), Integer)(ModDownload.DlFabricListBmclapiLoader, 35) }, Loader.IsForceRestarting)
        End Sub

        Private Sub DlFabricListOfficialMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult))
            Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://meta.fabricmc.net/v2/versions", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
            Try
                Dim dlFabricListResult As ModDownload.DlFabricListResult = New ModDownload.DlFabricListResult() With { .IsOfficial = True, .SourceName = "Fabric 官方源", .Value = jobject }
                If dlFabricListResult.Value("game") Is Nothing OrElse dlFabricListResult.Value("loader") Is Nothing OrElse dlFabricListResult.Value("installer") Is Nothing Then
                    Throw New Exception("获取到的列表缺乏必要项")
                End If
                Loader.Output = dlFabricListResult
            Catch ex As Exception
                Throw New Exception("Fabric 官方源版本列表解析失败（" + jobject.ToString() + "）", ex)
            End Try
        End Sub

        Private Sub DlFabricListBmclapiMain(Loader As ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult))
            Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/fabric-meta/v2/versions", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
            Try
                Dim dlFabricListResult As ModDownload.DlFabricListResult = New ModDownload.DlFabricListResult() With { .IsOfficial = False, .SourceName = "BMCLAPI", .Value = jobject }
                If dlFabricListResult.Value("game") Is Nothing OrElse dlFabricListResult.Value("loader") Is Nothing OrElse dlFabricListResult.Value("installer") Is Nothing Then
                    Throw New Exception("获取到的列表缺乏必要项")
                End If
                Loader.Output = dlFabricListResult
            Catch ex As Exception
                Throw New Exception("Fabric BMCLAPI 版本列表解析失败（" + jobject.ToString() + "）", ex)
            End Try
        End Sub

        Public Function DlModRequest(Url As String, Optional Method As HttpMethod = Nothing, Optional Content As String = Nothing, Optional ContentType As String = Nothing) As JToken
            Dim list As List(Of ValueTuple(Of String, Integer)) = New List(Of ValueTuple(Of String, Integer))()
            Dim text As String = ModDownload.DlSourceModGet(Url)
            If Operators.CompareString(text, Url, False) <> 0 Then
                Dim num As Integer = Settings.[Get](Of Integer)("ToolDownloadMod", Nothing)
                If num <> 0 Then
                    If num <> 1 Then
                        list.Add(New ValueTuple(Of String, Integer)(Url, If(Url.Contains("modrinth"), 20, 10)))
                        list.Add(New ValueTuple(Of String, Integer)(Url, 30))
                        list.Add(New ValueTuple(Of String, Integer)(text, 30))
                    Else
                        list.Add(New ValueTuple(Of String, Integer)(Url, If(Url.Contains("modrinth"), 20, 10)))
                        list.Add(New ValueTuple(Of String, Integer)(text, 10))
                        list.Add(New ValueTuple(Of String, Integer)(Url, 30))
                        list.Add(New ValueTuple(Of String, Integer)(text, 30))
                    End If
                Else
                    list.Add(New ValueTuple(Of String, Integer)(text, 10))
                    list.Add(New ValueTuple(Of String, Integer)(text, 10))
                    list.Add(New ValueTuple(Of String, Integer)(Url, 30))
                End If
            Else
                list.Add(New ValueTuple(Of String, Integer)(Url, If(Url.Contains("modrinth"), 20, 10)))
                list.Add(New ValueTuple(Of String, Integer)(Url, 30))
            End If
            Dim list2 As List(Of Exception) = New List(Of Exception)()
            Try
                For Each valueTuple As ValueTuple(Of String, Integer) In list
                    Try
                        ' The following expression was wrapped in a checked-expression
                        Return ModNet.NetRequestByClient(valueTuple.Item1, Method, Content, ContentType, "*/*", valueTuple.Item2 * 1000, Nothing, True, Encoding.UTF8, False, True).DeserializeJson()
                    Catch ex As Exception
                        list2.Add(ex)
                    End Try
                Next
            Finally
                Dim enumerator As List(Of ValueTuple(Of String, Integer)).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Throw New AggregateException(list2)
        End Function

        Public ReadOnly Property DlSourcePreferMojang As Boolean
            Get
                Return Settings.[Get](Of Integer)("ToolDownloadSource", Nothing) = 2 OrElse (Settings.[Get](Of Integer)("ToolDownloadSource", Nothing) = 1 AndAlso ModDownload.DlPreferMojang)
            End Get
        End Property

        Public Function DlSourceOrder(OfficialUrls As IEnumerable(Of String), MirrorUrls As IEnumerable(Of String)) As IEnumerable(Of String)
            If Not ModDownload.DlSourcePreferMojang Then
                Return MirrorUrls.Union(OfficialUrls)
            End If
            Return OfficialUrls.Union(MirrorUrls)
        End Function

        Public ReadOnly Property DlVersionListPreferMojang As Boolean
            Get
                Return Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing) = 2 OrElse (Settings.[Get](Of Integer)("ToolDownloadVersion", Nothing) = 1 AndAlso ModDownload.DlPreferMojang)
            End Get
        End Property

        Public Function DlVersionListOrder(OfficialUrls As IEnumerable(Of String), MirrorUrls As IEnumerable(Of String)) As IEnumerable(Of String)
            If Not ModDownload.DlVersionListPreferMojang Then
                Return MirrorUrls.Union(OfficialUrls)
            End If
            Return OfficialUrls.Union(MirrorUrls)
        End Function

        Public Function DlSourceAssetsGet(Original As String) As IEnumerable(Of String)
            Original = Original.Replace("http://resources.download.minecraft.net", "https://resources.download.minecraft.net")
            Return ModDownload.DlSourceOrder(New String() { Original }, New String() { Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/assets").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/assets").Replace("https://resources.download.minecraft.net", "https://bmclapi2.bangbang93.com/assets") })
        End Function

        Public Function DlSourceLibraryGet(Original As String) As IEnumerable(Of String)
            Dim enumerable As IEnumerable(Of String)
            If New String() { "minecraftforge", "fabricmc", "neoforged" }.Any(Function(k As String) Original.Contains(k)) Then
                enumerable = New String() { Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/maven"), Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/libraries") }
            Else
                enumerable = ModDownload.DlSourceOrder(New String() { Original }, New String() { Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/maven").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/maven"), Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com/libraries").Replace("https://libraries.minecraft.net", "https://bmclapi2.bangbang93.com/libraries") })
            End If
            Return enumerable
        End Function

        Public Function DlSourceLauncherOrMetaGet(Original As String) As IEnumerable(Of String)
            If Original Is Nothing Then
                Throw New Exception("无对应的 json 下载地址")
            End If
            Return ModDownload.DlSourceOrder(New String() { Original }, New String() { Original.Replace("https://piston-data.mojang.com", "https://bmclapi2.bangbang93.com").Replace("https://piston-meta.mojang.com", "https://bmclapi2.bangbang93.com").Replace("https://launcher.mojang.com", "https://bmclapi2.bangbang93.com").Replace("https://launchermeta.mojang.com", "https://bmclapi2.bangbang93.com") })
        End Function

        Public Function DlSourceModGet(Original As String) As String
            Return Original.Replace("api.modrinth.com", "mod.mcimirror.top/modrinth").Replace("staging-api.modrinth.com", "mod.mcimirror.top/modrinth").Replace("cdn.modrinth.com", "mod.mcimirror.top").Replace("api.curseforge.com", "mod.mcimirror.top/curseforge").Replace("edge.forgecdn.net", "mod.mcimirror.top").Replace("mediafilez.forgecdn.net", "mod.mcimirror.top").Replace("media.forgecdn.net", "mod.mcimirror.top")
        End Function

        Private Sub DlSourceLoader(Of InputType, OutputType)(MainLoader As ModLoader.LoaderTask(Of InputType, OutputType), LoaderList As List(Of KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer)), Optional IsForceRestart As Boolean = False)
            Dim num As Integer = 0
            While True
                Dim flag As Boolean = True
                Try
                    For Each keyValuePair As KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer) In LoaderList
                        If num = 0 Then
                            If IsForceRestart OrElse ((keyValuePair.Key.Input Is Nothing) Xor (MainLoader.Input Is Nothing)) Then
                                Continue For
                            End If
                            If keyValuePair.Key.Input IsNot Nothing Then
                                Dim key As ModLoader.LoaderTask(Of InputType, OutputType) = keyValuePair.Key
                                Dim ptr As __ByRef(Of InputType) = key.Input
                                If Nothing Is Nothing Then
                                    Dim input As InputType = key.Input
                                    ptr = input
                                End If
                                If Not ptr.Equals(MainLoader.Input) Then
                                    Continue For
                                End If
                            End If
                        End If
                        If keyValuePair.Key.State <> ModBase.LoadState.Failed Then
                            flag = False
                        End If
                        If keyValuePair.Key.State = ModBase.LoadState.Finished Then
                            MainLoader.Output = keyValuePair.Key.Output
                            ModDownload.DlSourceLoaderCancel(Of InputType, OutputType)(LoaderList)
                            Return
                        End If
                        If flag AndAlso num < keyValuePair.Value * 100 Then
                            num = keyValuePair.Value * 100
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer)).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                If num = 0 Then
                    LoaderList.First().Key.Start(MainLoader.Input, IsForceRestart)
                    Try
                        For Each keyValuePair2 As KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer) In LoaderList.Skip(1)
                            keyValuePair2.Key.State = ModBase.LoadState.Waiting
                        Next
                    Finally
                        Dim enumerator2 As IEnumerator(Of KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer))
                        If enumerator2 IsNot Nothing Then
                            enumerator2.Dispose()
                        End If
                    End Try
                End If
                Dim num2 As Integer = LoaderList.Count - 1
                Dim i As Integer = 0
                While i <= num2
                    If num = LoaderList(i).Value * 100 Then
                        If i >= LoaderList.Count - 1 Then
                            GoTo IL_01FB
                        End If
                        Dim func As Func(Of KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer), Boolean)
                        If ModDownload._Closure$__74(Of InputType, OutputType).$I74-0 IsNot Nothing Then
                            func = ModDownload._Closure$__74(Of InputType, OutputType).$I74-0
                        Else
                            Dim func2 As Func(Of KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer), Boolean) = Function(l As KeyValuePair(Of ModLoader.LoaderTask(Of $CLS0, $CLS1), Integer)) l.Key.State = ModBase.LoadState.Failed
                            func = func2
                            ModDownload._Closure$__74(Of InputType, OutputType).$I74-0 = func2
                        End If
                        If Not LoaderList.All(func) Then
                            LoaderList(i + 1).Key.Start(MainLoader.Input, IsForceRestart)
                            Exit While
                        End If
                        GoTo IL_01FB
                    Else
                        i += 1
                    End If
                End While
                Thread.Sleep(10)
                num += 1
                If MainLoader.IsCanceled Then
                    GoTo Block_12
                End If
            End While
            IL_01FB:
            Dim ex As Exception = Nothing
            Dim num3 As Integer = LoaderList.Count - 1
            For j As Integer = 0 To num3
                LoaderList(j).Key.Input = Nothing
                If LoaderList(j).Key.[Error] IsNot Nothing AndAlso (ex Is Nothing OrElse Operators.CompareString(LoaderList(j).Key.[Error].Message, "无", False) = 0) Then
                    ex = LoaderList(j).Key.[Error]
                End If
            Next
            If ex Is Nothing Then
                ex = New TimeoutException("下载源连接超时")
            End If
            ModDownload.DlSourceLoaderCancel(Of InputType, OutputType)(LoaderList)
            Throw ex
            Block_12:
            ModDownload.DlSourceLoaderCancel(Of InputType, OutputType)(LoaderList)
        End Sub

        Private Sub DlSourceLoaderCancel(Of InputType, OutputType)(LoaderList As List(Of KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer)))
            Try
                For Each keyValuePair As KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer) In LoaderList
                    If keyValuePair.Key.State = ModBase.LoadState.Loading Then
                        keyValuePair.Key.Cancel()
                    End If
                Next
            Finally
                Dim enumerator As List(Of KeyValuePair(Of ModLoader.LoaderTask(Of InputType, OutputType), Integer)).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
        End Sub

        Private _AllDrops As List(Of Integer) = Nothing

        Public DlClientListLoader As ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult) = New ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult)("DlClientList Main", AddressOf ModDownload.DlClientListMain, Nothing, ThreadPriority.Normal)

        Public DlClientListMojangLoader As ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult) = New ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult)("DlClientList Mojang", AddressOf ModDownload.DlClientListMojangMain, Nothing, ThreadPriority.Normal)

        Public DlClientListBmclapiLoader As ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult) = New ModLoader.LoaderTask(Of String, ModDownload.DlClientListResult)("DlClientList Bmclapi", AddressOf ModDownload.DlClientListBmclapiMain, Nothing, ThreadPriority.Normal)

        Public DlOptiFineListLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult)("DlOptiFineList Main", AddressOf ModDownload.DlOptiFineListMain, Nothing, ThreadPriority.Normal)

        Public DlOptiFineListOfficialLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult)("DlOptiFineList Official", AddressOf ModDownload.DlOptiFineListOfficialMain, Nothing, ThreadPriority.Normal)

        Public DlOptiFineListBmclapiLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlOptiFineListResult)("DlOptiFineList Bmclapi", AddressOf ModDownload.DlOptiFineListBmclapiMain, Nothing, ThreadPriority.Normal)

        Public DlForgeListLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult)("DlForgeList Main", AddressOf ModDownload.DlForgeListMain, Nothing, ThreadPriority.Normal)

        Public DlForgeListOfficialLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult)("DlForgeList Official", AddressOf ModDownload.DlForgeListOfficialMain, Nothing, ThreadPriority.Normal)

        Public DlForgeListBmclapiLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlForgeListResult)("DlForgeList Bmclapi", AddressOf ModDownload.DlForgeListBmclapiMain, Nothing, ThreadPriority.Normal)

        Public DlNeoForgeListLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult)("DlNeoForgeList Main", AddressOf ModDownload.DlNeoForgeListMain, Nothing, ThreadPriority.Normal)

        Public DlNeoForgeListOfficialLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult)("DlNeoForgeList Official", AddressOf ModDownload.DlNeoForgeListOfficialMain, Nothing, ThreadPriority.Normal)

        Public DlNeoForgeListBmclapiLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlNeoForgeListResult)("DlNeoForgeList Bmclapi", AddressOf ModDownload.DlNeoForgeListBmclapiMain, Nothing, ThreadPriority.Normal)

        Public DlLiteLoaderListLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult)("DlLiteLoaderList Main", AddressOf ModDownload.DlLiteLoaderListMain, Nothing, ThreadPriority.Normal)

        Public DlLiteLoaderListOfficialLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult)("DlLiteLoaderList Official", AddressOf ModDownload.DlLiteLoaderListOfficialMain, Nothing, ThreadPriority.Normal)

        Public DlLiteLoaderListBmclapiLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlLiteLoaderListResult)("DlLiteLoaderList Bmclapi", AddressOf ModDownload.DlLiteLoaderListBmclapiMain, Nothing, ThreadPriority.Normal)

        Public DlFabricListLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult)("DlFabricList Main", AddressOf ModDownload.DlFabricListMain, Nothing, ThreadPriority.Normal)

        Public DlFabricListOfficialLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult)("DlFabricList Official", AddressOf ModDownload.DlFabricListOfficialMain, Nothing, ThreadPriority.Normal)

        Public DlFabricListBmclapiLoader As ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult) = New ModLoader.LoaderTask(Of Integer, ModDownload.DlFabricListResult)("DlFabricList Bmclapi", AddressOf ModDownload.DlFabricListBmclapiMain, Nothing, ThreadPriority.Normal)

        Public DlFabricApiLoader As ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion)) = New ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion))("Fabric API List Loader", Sub(Task As ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion)))
            Task.Output = ResourceVersion.FromProjectId("fabric-api", ResourcePlatforms.Modrinth, False)
        End Sub, Nothing, ThreadPriority.Normal)

        Public DlOptiFabricLoader As ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion)) = New ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion))("OptiFabric List Loader", Sub(Task As ModLoader.LoaderTask(Of Integer, List(Of ResourceVersion)))
            Task.Output = ResourceVersion.FromProjectId("322385", ResourcePlatforms.CurseForge, False)
        End Sub, Nothing, ThreadPriority.Normal)

        Private DlPreferMojang As Boolean = False

        Private $STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted As Boolean

        Private $STATIC$DlClientListMojangMain$011151286FC2E118758$IsHinted$Init As StaticLocalInitFlag

        Public Structure DlClientListResult
            Public SourceName As String

            Public IsOfficial As Boolean

            Public Value As JObject
        End Structure

        Public Structure DlOptiFineListResult
            Public SourceName As String

            Public IsOfficial As Boolean

            Public Value As List(Of ModDownload.DlOptiFineListEntry)
        End Structure

        Public Class DlOptiFineListEntry
            Public Property Inherit As String
                Get
                    Return Me._inherit
                End Get
                Set(value As String)
                    If value.EndsWithF(".0", False) Then
                        ' The following expression was wrapped in a checked-expression
                        value = Strings.Left(value, value.Length - 2)
                    End If
                    Me._inherit = value
                End Set
            End Property

            Public DisplayName As String

            Public FileName As String

            Public InstanceName As String

            Public IsPreview As Boolean

            Private _inherit As String

            Public ReleaseTime As String

            Public RequiredForgeVersion As String
        End Class

        Public Structure DlForgeListResult
            Public SourceName As String

            Public IsOfficial As Boolean

            Public Value As List(Of String)
        End Structure

        Public MustInherit Class DlForgelikeEntry
            Implements IComparable(Of ModDownload.DlForgelikeEntry)

            Public MustOverride ReadOnly Property LoaderName As String

            Public MustOverride ReadOnly Property FileExtension As String

            Public Function CompareTo(other As ModDownload.DlForgelikeEntry) As Integer Implements System.IComparable(Of PCL.ModDownload.DlForgelikeEntry).CompareTo
                Dim num As Integer
                If Me.Version IsNot other.Version Then
                    num = Me.Version.CompareTo(other.Version)
                Else
                    num = McUtils.CompareVersion(Me.VersionName, other.VersionName)
                End If
                Return num
            End Function

            Public IsNeoForge As Boolean

            Public Version As Version

            Public VersionName As String

            Public Inherit As String
        End Class

        Public Class DlForgeVersionEntry
            Inherits ModDownload.DlForgelikeEntry

            Public Overrides ReadOnly Property LoaderName As String
                Get
                    Return "Forge"
                End Get
            End Property

            Public Overrides ReadOnly Property FileExtension As String
                Get
                    If Operators.CompareString(Me.Category, "installer", False) <> 0 Then
                        Return "zip"
                    End If
                    Return "jar"
                End Get
            End Property

            Public Sub New(Version As String, Branch As String, Inherit As String)
                Me.Hash = Nothing
                If Operators.CompareString(Version, "11.15.1.2318", False) = 0 OrElse Operators.CompareString(Version, "11.15.1.1902", False) = 0 OrElse Operators.CompareString(Version, "11.15.1.1890", False) = 0 Then
                    Branch = "1.8.9"
                End If
                If Branch Is Nothing AndAlso Operators.CompareString(Inherit, "1.7.10", False) = 0 AndAlso Conversions.ToDouble(Version.Split(".", False)(3)) >= 1300.0 Then
                    Branch = "1.7.10"
                End If
                Me.IsNeoForge = False
                Me.VersionName = Version
                Me.Version = New Version(Version)
                Me.Inherit = Inherit
                Me.FileVersion = Version + If((Branch Is Nothing), "", ("-" + Branch))
            End Sub

            Public ReleaseTime As String

            Public Hash As String

            Public IsRecommended As Boolean

            Public Category As String

            Public FileVersion As String
        End Class

        Public Structure DlNeoForgeListResult
            Public SourceName As String

            Public IsOfficial As Boolean

            Public Value As List(Of ModDownload.DlNeoForgeListEntry)
        End Structure

        Public Class DlNeoForgeListEntry
            Inherits ModDownload.DlForgelikeEntry

            Public ReadOnly Property UrlBase As String
                Get
                    Dim text As String = If((Operators.CompareString(Me.Inherit, "1.20.1", False) = 0), "forge", "neoforge")
                    Return String.Format("https://maven.neoforged.net/releases/net/neoforged/{0}/{1}/{2}-{3}", New Object() { text, Me.ApiName, text, Me.ApiName })
                End Get
            End Property

            Public Overrides ReadOnly Property LoaderName As String
                Get
                    Return "NeoForge"
                End Get
            End Property

            Public Overrides ReadOnly Property FileExtension As String
                Get
                    Return "jar"
                End Get
            End Property

            Public Sub New(ApiName As String)
                Me.IsNeoForge = True
                Me.ApiName = ApiName
                Me.IsBeta = ApiName.ContainsIgnoreCase("beta") OrElse ApiName.ContainsIgnoreCase("alpha")
                If ApiName.Contains("1.20.1") Then
                    Me.VersionName = ApiName.Replace("1.20.1-", "")
                    Me.Version = New Version("19." + Me.VersionName)
                    Me.Inherit = "1.20.1"
                    Return
                End If
                If ApiName.StartsWithF("0.", False) Then
                    Me.VersionName = ApiName
                    Dim array As String() = ApiName.BeforeFirst("-", False).Split(New Char() { "."c })
                    Me.Version = New Version(0, 0, Conversions.ToInteger(array.Last()))
                    Me.Inherit = array(1)
                    Return
                End If
                Me.VersionName = ApiName
                Me.Version = New Version(ApiName.BeforeFirst("-", False))
                If Me.Version.Major >= 24 Then
                    Me.Inherit = String.Format("{0}.{1}.{2}", Me.Version.Major, Me.Version.Minor, Me.Version.Build)
                Else
                    Me.Inherit = String.Format("1.{0}.{1}", Me.Version.Major, Me.Version.Minor)
                End If
                If Me.Inherit.EndsWithF(".0", False) Then
                    ' The following expression was wrapped in a checked-expression
                    Me.Inherit = Strings.Left(Me.Inherit, Me.Inherit.Length - 2)
                End If
                If Me.VersionName.Contains("+") Then
                    Dim ptr As __ByRef(Of String) = Me.Inherit
                    Me.Inherit = ptr + "-" + Me.VersionName.AfterFirst("+", False)
                End If
            End Sub

            Public IsBeta As Boolean

            Public ApiName As String
        End Class

        Public Structure DlLiteLoaderListResult
            Public SourceName As String

            Public IsOfficial As Boolean

            Public Value As List(Of ModDownload.DlLiteLoaderListEntry)

            Public OfficialError As Exception
        End Structure

        Public Class DlLiteLoaderListEntry
            Public FileName As String

            Public IsPreview As Boolean

            Public Inherit As String

            Public IsLegacy As Boolean

            Public ReleaseTime As String

            Public MD5 As String

            Public JsonToken As JToken
        End Class

        Public Structure DlFabricListResult
            Public SourceName As String

            Public IsOfficial As Boolean

            Public Value As JObject
        End Structure
    End Module
End Namespace
