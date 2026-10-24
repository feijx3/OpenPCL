Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public NotInheritable Module LocalResourceLoaders
        Private Sub LocalResourceFileLoad(Loader As ModLoader.LoaderTask(Of String, List(Of LocalResourceFile)))
            Try
                Dim action As Action
                If LocalResourceLoaders._Closure$__.$I2-0 IsNot Nothing Then
                    action = LocalResourceLoaders._Closure$__.$I2-0
                Else
                    Dim action2 As Action = Sub()
                        If ModMain.FrmInstanceMod IsNot Nothing Then
                            ModMain.FrmInstanceMod.Load.ShowProgress = False
                        End If
                    End Sub
                    action = action2
                    LocalResourceLoaders._Closure$__.$I2-0 = action2
                End If
                ModBase.RunInUiWait(action)
                If PageInstanceMod.UpdatingInstanceModFolders.Contains(Loader.Input) Then
                    Logger.Info(String.Format("等待 Mod 更新完成后才能继续加载 Mod 列表：{0}", Loader.Input), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                    Try
                        Dim action3 As Action
                        If LocalResourceLoaders._Closure$__.$I2-1 IsNot Nothing Then
                            action3 = LocalResourceLoaders._Closure$__.$I2-1
                        Else
                            Dim action4 As Action = Sub()
                                If ModMain.FrmInstanceMod IsNot Nothing Then
                                    ModMain.FrmInstanceMod.Load.Text = "正在更新 Mod"
                                End If
                            End Sub
                            action3 = action4
                            LocalResourceLoaders._Closure$__.$I2-1 = action4
                        End If
                        ModBase.RunInUiWait(action3)
                        While PageInstanceMod.UpdatingInstanceModFolders.Contains(Loader.Input)
                            If Loader.IsCanceled Then
                                Return
                            End If
                            Thread.Sleep(100)
                        End While
                    Finally
                        Dim action5 As Action
                        If LocalResourceLoaders._Closure$__.$I2-2 IsNot Nothing Then
                            action5 = LocalResourceLoaders._Closure$__.$I2-2
                        Else
                            Dim action6 As Action = Sub()
                                If ModMain.FrmInstanceMod IsNot Nothing Then
                                    ModMain.FrmInstanceMod.Load.Text = "正在加载 Mod 列表"
                                End If
                            End Sub
                            action5 = action6
                            LocalResourceLoaders._Closure$__.$I2-2 = action6
                        End If
                        ModBase.RunInUiWait(action5)
                    End Try
                    ModMain.FrmInstanceMod.LoaderRun(ModLoader.LoaderFolderRunType.UpdateOnly)
                End If
                Dim list As List(Of String) = New List(Of String)()
                If DirectoryUtils.Exists(Loader.Input) Then
                    Try
                        For Each text As String In DirectoryUtils.EnumerateFiles(Loader.Input, True, "*")
                            If PathUtils.IsSame(PathUtils.RemoveLastPart(text), Loader.Input) OrElse (PageInstanceLeft.Instance IsNot Nothing AndAlso PageInstanceLeft.Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) AndAlso PageInstanceLeft.Instance.Version.Vanilla.Major < 13 AndAlso Operators.CompareString(PathUtils.GetLastPart(PathUtils.RemoveLastPart(text)), String.Format("1.{0}.{1}", PageInstanceLeft.Instance.Version.Vanilla.Major, PageInstanceLeft.Instance.Version.Vanilla.Build), False) = 0) Then
                                If LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init Is Nothing Then
                                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init, New StaticLocalInitFlag(), Nothing)
                                End If
                                Dim flag As Boolean = False
                                Try
                                    Monitor.Enter(LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init, flag)
                                    If LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State = 0S Then
                                        LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State = 2S
                                        Dim func As Func(Of String, Boolean)
                                        If LocalResourceLoaders._Closure$__.$I2-3 IsNot Nothing Then
                                            func = LocalResourceLoaders._Closure$__.$I2-3
                                        Else
                                            Dim func2 As Func(Of String, Boolean) = Function(FullName As String)
                                                Dim flag2 As Boolean
                                                If FullName Is Nothing Then
                                                    flag2 = False
                                                Else
                                                    If New String() { "disabled", "old" }.Contains(PathUtils.GetExtension(FullName)) Then
                                                        FullName = PathUtils.GetFileNameWithoutExtension(FullName)
                                                    End If
                                                    flag2 = New String() { "jar", "zip", "litemod" }.Contains(PathUtils.GetExtension(FullName))
                                                End If
                                                Return flag2
                                            End Function
                                            func = func2
                                            LocalResourceLoaders._Closure$__.$I2-3 = func2
                                        End If
                                        LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile = func
                                    ElseIf LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State = 2S Then
                                        Throw New IncompleteInitialization()
                                    End If
                                Finally
                                    LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init.State = 1S
                                    If flag Then
                                        Monitor.[Exit](LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init)
                                    End If
                                End Try
                                If LocalResourceLoaders.$STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile(text) Then
                                    list.Add(text)
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of String)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                End If
                Dim text2 As String = ModBase.PathTemp + "Cache\LocalMod.json"
                Dim jobject As JObject = New JObject()
                Try
                    Dim text3 As String = FileUtils.TryReadAsString(text2, Nothing, Nothing)
                    If Not String.IsNullOrWhiteSpace(text3) Then
                        jobject = CType(text3.DeserializeJson(), JObject)
                        If Not jobject.ContainsKey("version") OrElse jobject("version").ToObject(Of Integer)() <> 21 Then
                            Logger.Warn("本地 Mod 信息缓存版本已过期，将弃用这些缓存信息", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                            jobject = New JObject()
                        End If
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, "读取本地 Mod 信息缓存失败，已重置", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                    jobject = New JObject()
                End Try
                jobject("version") = 21
                Dim list2 As List(Of LocalResourceFile) = New List(Of LocalResourceFile)()
                Try
                    For Each text4 As String In list
                        Dim CS$<>8__locals1 As LocalResourceLoaders._Closure$__2-0 = New LocalResourceLoaders._Closure$__2-0(CS$<>8__locals1)
                        If Loader.IsCanceled Then
                            Return
                        End If
                        CS$<>8__locals1.$VB$Local_ModEntry = New LocalResourceFile(text4)
                        Dim localResourceFile As LocalResourceFile = list2.FirstOrDefault(Function(m As LocalResourceFile) Operators.CompareString(m.EnabledName, CS$<>8__locals1.$VB$Local_ModEntry.EnabledName, False) = 0)
                        If localResourceFile IsNot Nothing Then
                            Dim localResourceFile2 As LocalResourceFile = If(localResourceFile.IsEnabled, CS$<>8__locals1.$VB$Local_ModEntry, localResourceFile)
                            Logger.Warn(String.Format("重复的 Mod 文件：{0} 与 {1}，已忽略 {2}", localResourceFile.File.Name, CS$<>8__locals1.$VB$Local_ModEntry.File.Name, localResourceFile2.File.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                            If localResourceFile2 Is CS$<>8__locals1.$VB$Local_ModEntry Then
                                Continue For
                            End If
                            list2.Remove(localResourceFile2)
                        End If
                        list2.Add(CS$<>8__locals1.$VB$Local_ModEntry)
                    Next
                Finally
                    Dim enumerator2 As List(Of String).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                Logger.Info(String.Format("共发现 {0} 个 Mod", list2.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                Dim enumerable As IEnumerable(Of LocalResourceFile) = list2
                Dim func3 As Func(Of LocalResourceFile, String)
                If LocalResourceLoaders._Closure$__.$I2-5 IsNot Nothing Then
                    func3 = LocalResourceLoaders._Closure$__.$I2-5
                Else
                    Dim func4 As Func(Of LocalResourceFile, String) = Function(m As LocalResourceFile) m.File.Name
                    func3 = func4
                    LocalResourceLoaders._Closure$__.$I2-5 = func4
                End If
                list2 = enumerable.OrderBy(func3).ToList()
                If Not Loader.IsCanceled Then
                    Loader.Output = list2
                    LocalResourceLoaders.LocalResourceOnlineLoader.Start(New ValueTuple(Of List(Of LocalResourceFile), JObject, McVersion)(list2, jobject, PageInstanceLeft.Instance.Version), True)
                End If
            Catch ex2 As Exception
                Logger.Warn(ex2, "Mod 列表加载失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                Throw
            End Try
        End Sub

        Private Sub LocalResourceOnlineLoad(<TupleElementNames(New String() { "ModList", "Cache", "Target" })> Loader As ModLoader.LoaderTask(Of ValueTuple(Of List(Of LocalResourceFile), JObject, McVersion), Integer))
            Dim CS$<>8__locals1 As LocalResourceLoaders._Closure$__4-0 = New LocalResourceLoaders._Closure$__4-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Loader = Loader
            CS$<>8__locals1.$VB$Local_Mods = New List(Of LocalResourceFile)()
            Dim item As JObject = CS$<>8__locals1.$VB$Local_Loader.Input.Item2
            Dim modLoaders As ModLoaders = CS$<>8__locals1.$VB$Local_Loader.Input.Item3.ModLoaders
            Try
                For Each localResourceFile As LocalResourceFile In CS$<>8__locals1.$VB$Local_Loader.Input.Item1
                    If CS$<>8__locals1.$VB$Local_Loader.IsCanceled Then
                        Return
                    End If
                    Dim text As String = localResourceFile.ModrinthHash + CS$<>8__locals1.$VB$Local_Loader.Input.Item3.VanillaName + Conversions.ToString(CInt(modLoaders))
                    If item.ContainsKey(text) Then
                        localResourceFile.FromJson(CType(item(text), JObject))
                        If localResourceFile.OnlineDataLoaded AndAlso DateTime.Now - item(text)("Project")("CacheTime").ToObject(Of DateTime)() < New TimeSpan(6, 0, 0) Then
                            Continue For
                        End If
                    End If
                    CS$<>8__locals1.$VB$Local_Mods.Add(localResourceFile)
                Next
            Finally
                Dim enumerator As List(Of LocalResourceFile).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Dim text2 As String = "有 {0} 个 Mod 需要联网获取信息，{1} 个 Mod 需要更新信息"
            Dim $VB$Local_Mods As IEnumerable(Of LocalResourceFile) = CS$<>8__locals1.$VB$Local_Mods
            Dim func As Func(Of LocalResourceFile, Boolean)
            If LocalResourceLoaders._Closure$__.$I4-0 IsNot Nothing Then
                func = LocalResourceLoaders._Closure$__.$I4-0
            Else
                Dim func2 As Func(Of LocalResourceFile, Boolean) = Function(m As LocalResourceFile) m.Project Is Nothing
                func = func2
                LocalResourceLoaders._Closure$__.$I4-0 = func2
            End If
            Dim obj As Object = $VB$Local_Mods.Where(func).Count()
            Dim $VB$Local_Mods2 As IEnumerable(Of LocalResourceFile) = CS$<>8__locals1.$VB$Local_Mods
            Dim func3 As Func(Of LocalResourceFile, Boolean)
            If LocalResourceLoaders._Closure$__.$I4-1 IsNot Nothing Then
                func3 = LocalResourceLoaders._Closure$__.$I4-1
            Else
                Dim func4 As Func(Of LocalResourceFile, Boolean) = Function(m As LocalResourceFile) m.Project IsNot Nothing
                func3 = func4
                LocalResourceLoaders._Closure$__.$I4-1 = func4
            End If
            Logger.Info(String.Format(text2, obj, $VB$Local_Mods2.Where(func3).Count()), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
            If CS$<>8__locals1.$VB$Local_Mods.Any() Then
                Dim item2 As McVersion = CS$<>8__locals1.$VB$Local_Loader.Input.Item3
                CS$<>8__locals1.$VB$Local_VanillaVersion = item2.VanillaName
                Logger.Info(String.Format("正在检查 Mod 信息的版本：{0}，加载器 {1}", CS$<>8__locals1.$VB$Local_VanillaVersion, modLoaders), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                CS$<>8__locals1.$VB$Local_EndedThreadCount = 0
                CS$<>8__locals1.$VB$Local_IsFailed = False
                CS$<>8__locals1.$VB$Local_CurrentTaskThread = Thread.CurrentThread
                ModBase.RunInNewThread(Sub()
                    Try
                        Dim CS$<>8__locals3 As LocalResourceLoaders._Closure$__4-1 = New LocalResourceLoaders._Closure$__4-1(CS$<>8__locals3)
                        CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                        Dim $VB$Local_Mods4 As IEnumerable(Of LocalResourceFile) = CS$<>8__locals1.$VB$Local_Mods
                        Dim func7 As Func(Of LocalResourceFile, String)
                        If LocalResourceLoaders._Closure$__.$I4-3 IsNot Nothing Then
                            func7 = LocalResourceLoaders._Closure$__.$I4-3
                        Else
                            Dim func8 As Func(Of LocalResourceFile, String) = Function(m As LocalResourceFile) m.ModrinthHash
                            func7 = func8
                            LocalResourceLoaders._Closure$__.$I4-3 = func8
                        End If
                        Dim list As List(Of String) = $VB$Local_Mods4.[Select](func7).ToList()
                        Dim jobject As JObject = CType(ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files", HttpMethod.Post, String.Format("{{""hashes"": [""{0}""], ""algorithm"": ""sha1""}}", list.Join(""",""")), "application/json"), JObject)
                        Logger.Info(String.Format("从 Modrinth 获取到 {0} 个本地 Mod 的对应信息", jobject.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                        If jobject.Count <> 0 Then
                            Dim dictionary As Dictionary(Of String, List(Of LocalResourceFile)) = New Dictionary(Of String, List(Of LocalResourceFile))()
                            CS$<>8__locals3.$VB$Local_ModrinthLoaders = New Dictionary(Of LocalResourceFile, List(Of String))()
                            Try
                                For Each localResourceFile3 As LocalResourceFile In CS$<>8__locals1.$VB$Local_Mods
                                    If jobject.ContainsKey(localResourceFile3.ModrinthHash) AndAlso Not(CStr(jobject(localResourceFile3.ModrinthHash)("files")(0)("hashes")("sha1")) <> localResourceFile3.ModrinthHash) Then
                                        Dim text3 As String = jobject(localResourceFile3.ModrinthHash)("project_id").ToString()
                                        If ResourceProject.Cache.ContainsKey(text3) AndAlso localResourceFile3.Project Is Nothing Then
                                            localResourceFile3.Project = ResourceProject.Cache(text3)
                                        End If
                                        If Not dictionary.ContainsKey(text3) Then
                                            dictionary(text3) = New List(Of LocalResourceFile)()
                                        End If
                                        dictionary(text3).Add(localResourceFile3)
                                        Dim resourceVersion As ResourceVersion = ResourceVersion.FromPlatformJson(CType(jobject(localResourceFile3.ModrinthHash), JObject), ResourceTypes.[Mod])
                                        Dim $VB$Local_ModrinthLoaders As Dictionary(Of LocalResourceFile, List(Of String)) = CS$<>8__locals3.$VB$Local_ModrinthLoaders
                                        Dim localResourceFile4 As LocalResourceFile = localResourceFile3
                                        Dim enumerable As IEnumerable(Of JToken) = jobject(localResourceFile3.ModrinthHash)("loaders")
                                        Dim func9 As Func(Of JToken, String)
                                        If LocalResourceLoaders._Closure$__.$I4-4 IsNot Nothing Then
                                            func9 = LocalResourceLoaders._Closure$__.$I4-4
                                        Else
                                            Dim func10 As Func(Of JToken, String) = Function(l As JToken) l.ToString().Lower()
                                            func9 = func10
                                            LocalResourceLoaders._Closure$__.$I4-4 = func10
                                        End If
                                        Dim enumerable2 As IEnumerable(Of String) = enumerable.[Select](func9).Distinct()
                                        Dim func11 As Func(Of String, String)
                                        If LocalResourceLoaders._Closure$__.$I4-5 IsNot Nothing Then
                                            func11 = LocalResourceLoaders._Closure$__.$I4-5
                                        Else
                                            Dim func12 As Func(Of String, String) = Function(l As String) l
                                            func11 = func12
                                            LocalResourceLoaders._Closure$__.$I4-5 = func12
                                        End If
                                        $VB$Local_ModrinthLoaders(localResourceFile4) = enumerable2.OrderBy(func11).ToList()
                                        If localResourceFile3.ProjectVersion Is Nothing OrElse DateTime.Compare(localResourceFile3.ProjectVersion.ReleaseDate, resourceVersion.ReleaseDate) < 0 Then
                                            localResourceFile3.ProjectVersion = resourceVersion
                                        Else
                                            localResourceFile3.ProjectVersion.Version = resourceVersion.Version
                                        End If
                                    End If
                                Next
                            Finally
                                Dim enumerator3 As List(Of LocalResourceFile).Enumerator
                                CType(enumerator3, IDisposable).Dispose()
                            End Try
                            If Not CS$<>8__locals1.$VB$Local_Loader.IsCanceledWithThread(CS$<>8__locals1.$VB$Local_CurrentTaskThread) Then
                                Logger.Info(String.Format("需要从 Modrinth 获取 {0} 个本地 Mod 的工程信息", dictionary.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                If dictionary.Any() Then
                                    Dim jarray As JArray = CType(ModDownload.DlModRequest(String.Format("https://api.modrinth.com/v2/projects?ids=[""{0}""]", dictionary.Keys.Join(""",""")), Nothing, Nothing, Nothing), JArray)
                                    Try
                                        For Each jtoken As JToken In jarray
                                            Dim resourceProject As ResourceProject = New ResourceProject(CType(jtoken, JObject))
                                            Try
                                                For Each localResourceFile5 As LocalResourceFile In dictionary(resourceProject.Id)
                                                    localResourceFile5.Project = resourceProject
                                                Next
                                            Finally
                                                Dim enumerator5 As List(Of LocalResourceFile).Enumerator
                                                CType(enumerator5, IDisposable).Dispose()
                                            End Try
                                        Next
                                    Finally
                                        Dim enumerator4 As IEnumerator(Of JToken)
                                        If enumerator4 IsNot Nothing Then
                                            enumerator4.Dispose()
                                        End If
                                    End Try
                                    Logger.Info("已从 Modrinth 获取本地 Mod 信息，继续获取更新信息", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                    Dim enumerable3 As IEnumerable(Of KeyValuePair(Of String, List(Of LocalResourceFile))) = dictionary
                                    Dim func13 As Func(Of KeyValuePair(Of String, List(Of LocalResourceFile)), IEnumerable(Of LocalResourceFile))
                                    If LocalResourceLoaders._Closure$__.$I4-6 IsNot Nothing Then
                                        func13 = LocalResourceLoaders._Closure$__.$I4-6
                                    Else
                                        Dim func14 As Func(Of KeyValuePair(Of String, List(Of LocalResourceFile)), IEnumerable(Of LocalResourceFile)) = Function(l As KeyValuePair(Of String, List(Of LocalResourceFile))) l.Value
                                        func13 = func14
                                        LocalResourceLoaders._Closure$__.$I4-6 = func14
                                    End If
                                    Dim list2 As List(Of LocalResourceFile) = enumerable3.SelectMany(func13).ToList()
                                    If list2.Any() Then
                                        Dim text4 As String = list2.[Select](Function(m As LocalResourceFile) String.Format("{{""hash"": ""{0}"", ""loaders"": [""{1}""], ""game_versions"": [""{2}""]}}", m.ModrinthHash, CS$<>8__locals3.$VB$Local_ModrinthLoaders(m).Join(""","""), CS$<>8__locals3.$VB$NonLocal_$VB$Closure_2.$VB$Local_VanillaVersion)).Join(",")
                                        Dim jobject2 As JObject = CType(ModDownload.DlModRequest("https://api.modrinth.com/v2/version_files/update_individual", HttpMethod.Post, String.Format("{{""algorithm"": ""sha1"", ""hashes"": [{0}]}}", text4), "application/json"), JObject)
                                        Try
                                            Dim enumerator6 As List(Of LocalResourceFile).Enumerator = CS$<>8__locals1.$VB$Local_Mods.GetEnumerator()
                                            While enumerator6.MoveNext()
                                                Dim CS$<>8__locals4 As LocalResourceLoaders._Closure$__4-2 = New LocalResourceLoaders._Closure$__4-2(CS$<>8__locals4)
                                                CS$<>8__locals4.$VB$Local_Entry = enumerator6.Current
                                                Dim CS$<>8__locals5 As LocalResourceLoaders._Closure$__4-3 = New LocalResourceLoaders._Closure$__4-3(CS$<>8__locals5)
                                                CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals4
                                                If jobject2.ContainsKey(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash) AndAlso CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion IsNot Nothing Then
                                                    CS$<>8__locals5.$VB$Local_UpdateFile = ResourceVersion.FromPlatformJson(CType(jobject2(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash), JObject), ResourceTypes.[Mod])
                                                    If CS$<>8__locals5.$VB$Local_UpdateFile.DownloadAvailable Then
                                                        Dim enumerable4 As IEnumerable(Of JToken) = jobject2(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash)("loaders")
                                                        Dim func15 As Func(Of JToken, String)
                                                        If LocalResourceLoaders._Closure$__.$I4-8 IsNot Nothing Then
                                                            func15 = LocalResourceLoaders._Closure$__.$I4-8
                                                        Else
                                                            Dim func16 As Func(Of JToken, String) = Function(l As JToken) l.ToString().Lower()
                                                            func15 = func16
                                                            LocalResourceLoaders._Closure$__.$I4-8 = func16
                                                        End If
                                                        Dim enumerable5 As IEnumerable(Of String) = enumerable4.[Select](func15).Distinct()
                                                        Dim func17 As Func(Of String, String)
                                                        If LocalResourceLoaders._Closure$__.$I4-9 IsNot Nothing Then
                                                            func17 = LocalResourceLoaders._Closure$__.$I4-9
                                                        Else
                                                            Dim func18 As Func(Of String, String) = Function(l As String) l
                                                            func17 = func18
                                                            LocalResourceLoaders._Closure$__.$I4-9 = func18
                                                        End If
                                                        If enumerable5.OrderBy(func17).ToList().SequenceEqual(CS$<>8__locals3.$VB$Local_ModrinthLoaders(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry)) Then
                                                            Logger.Trace(Function() String.Format("本地文件 {0} 在 Modrinth 上的最新版为 {1}", CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion.FileName, CS$<>8__locals5.$VB$Local_UpdateFile.FileName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                                            If DateTime.Compare(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion.ReleaseDate, CS$<>8__locals5.$VB$Local_UpdateFile.ReleaseDate) < 0 AndAlso Operators.CompareString(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ProjectVersion.Hash, CS$<>8__locals5.$VB$Local_UpdateFile.Hash, False) <> 0 Then
                                                                If CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile IsNot Nothing AndAlso Operators.CompareString(CS$<>8__locals5.$VB$Local_UpdateFile.Hash, CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile.Hash, False) = 0 Then
                                                                    CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ChangelogUrls.Add(String.Format("https://modrinth.com/mod/{0}/changelog?g={1}", jobject2(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash)("project_id"), CS$<>8__locals1.$VB$Local_VanillaVersion))
                                                                    CS$<>8__locals5.$VB$Local_UpdateFile.DownloadUrls.AddRange(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile.DownloadUrls)
                                                                    CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile = CS$<>8__locals5.$VB$Local_UpdateFile
                                                                ElseIf CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile Is Nothing OrElse DateTime.Compare(CS$<>8__locals5.$VB$Local_UpdateFile.ReleaseDate, CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile.ReleaseDate) >= 0 Then
                                                                    CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ChangelogUrls = New List(Of String)() From { String.Format("https://modrinth.com/mod/{0}/changelog?g={1}", jobject2(CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.ModrinthHash)("project_id"), CS$<>8__locals1.$VB$Local_VanillaVersion) }
                                                                    CS$<>8__locals5.$VB$NonLocal_$VB$Closure_3.$VB$Local_Entry.UpdateFile = CS$<>8__locals5.$VB$Local_UpdateFile
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End While
                                        Finally
                                            Dim enumerator6 As List(Of LocalResourceFile).Enumerator
                                            CType(enumerator6, IDisposable).Dispose()
                                        End Try
                                        Logger.Info("从 Modrinth 获取本地 Mod 信息结束", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                    End If
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        Logger.Warn(ex, "从 Modrinth 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                        CS$<>8__locals1.$VB$Local_IsFailed = True
                    Finally
                        CS$<>8__locals1.$VB$Local_EndedThreadCount += 1
                    End Try
                End Sub, "Mod List Detail Loader Modrinth", ThreadPriority.Normal)
                ModBase.RunInNewThread(Sub()
                    Try
                        Dim CS$<>8__locals6 As LocalResourceLoaders._Closure$__4-4 = New LocalResourceLoaders._Closure$__4-4(CS$<>8__locals6)
                        CS$<>8__locals6.$VB$NonLocal_$VB$Closure_4 = CS$<>8__locals1
                        Dim list3 As List(Of UInteger) = New List(Of UInteger)()
                        Try
                            For Each localResourceFile6 As LocalResourceFile In CS$<>8__locals1.$VB$Local_Mods
                                list3.Add(localResourceFile6.CurseForgeHash)
                                If CS$<>8__locals1.$VB$Local_Loader.IsCanceledWithThread(CS$<>8__locals1.$VB$Local_CurrentTaskThread) Then
                                    Return
                                End If
                            Next
                        Finally
                            Dim enumerator7 As List(Of LocalResourceFile).Enumerator
                            CType(enumerator7, IDisposable).Dispose()
                        End Try
                        Dim jcontainer As JContainer = CType(ModDownload.DlModRequest("https://api.curseforge.com/v1/fingerprints/432", HttpMethod.Post, String.Format("{{""fingerprints"": [{0}]}}", list3.Join(",")), "application/json")("data")("exactMatches"), JContainer)
                        Logger.Info(String.Format("从 CurseForge 获取到 {0} 个本地 Mod 的对应信息", jcontainer.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                        If jcontainer.Any() Then
                            Dim dictionary2 As Dictionary(Of Integer, List(Of LocalResourceFile)) = New Dictionary(Of Integer, List(Of LocalResourceFile))()
                            CS$<>8__locals6.$VB$Local_CurseForgeLoaders = New Dictionary(Of LocalResourceFile, ModLoaders)()
                            Try
                                For Each jtoken2 As JToken In jcontainer
                                    Dim text5 As String = jtoken2("id").ToString()
                                    Dim num As UInteger = CUInt(jtoken2("file")("fileFingerprint"))
                                    Try
                                        For Each localResourceFile7 As LocalResourceFile In CS$<>8__locals1.$VB$Local_Mods
                                            If localResourceFile7.CurseForgeHash = num Then
                                                If ResourceProject.Cache.ContainsKey(text5) AndAlso localResourceFile7.Project Is Nothing Then
                                                    localResourceFile7.Project = ResourceProject.Cache(text5)
                                                End If
                                                dictionary2.AddIntoValueCollection(Conversions.ToInteger(text5), localResourceFile7)
                                                Dim resourceVersion2 As ResourceVersion = ResourceVersion.FromPlatformJson(CType(jtoken2("file"), JObject), ResourceTypes.[Mod])
                                                CS$<>8__locals6.$VB$Local_CurseForgeLoaders(localResourceFile7) = resourceVersion2.ModLoaders
                                                If localResourceFile7.ProjectVersion Is Nothing OrElse DateTime.Compare(localResourceFile7.ProjectVersion.ReleaseDate, resourceVersion2.ReleaseDate) < 0 Then
                                                    localResourceFile7.ProjectVersion = resourceVersion2
                                                End If
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator9 As List(Of LocalResourceFile).Enumerator
                                        CType(enumerator9, IDisposable).Dispose()
                                    End Try
                                Next
                            Finally
                                Dim enumerator8 As IEnumerator(Of JToken)
                                If enumerator8 IsNot Nothing Then
                                    enumerator8.Dispose()
                                End If
                            End Try
                            If Not CS$<>8__locals1.$VB$Local_Loader.IsCanceledWithThread(CS$<>8__locals1.$VB$Local_CurrentTaskThread) Then
                                Logger.Info(String.Format("需要从 CurseForge 获取 {0} 个本地 Mod 的工程信息", dictionary2.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                If dictionary2.Any() Then
                                    Dim jtoken3 As JToken = ModDownload.DlModRequest("https://api.curseforge.com/v1/mods", HttpMethod.Post, String.Format("{{""modIds"": [{0}]}}", dictionary2.Keys.Join(",")), "application/json")("data")
                                    Dim dictionary3 As Dictionary(Of Integer, List(Of LocalResourceFile)) = New Dictionary(Of Integer, List(Of LocalResourceFile))()
                                    Dim dictionary4 As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
                                    Try
                                        For Each jtoken4 As JToken In jtoken3
                                            If jtoken4("isAvailable") Is Nothing OrElse jtoken4("isAvailable").ToObject(Of Boolean)() Then
                                                Dim resourceProject2 As ResourceProject = New ResourceProject(CType(jtoken4, JObject))
                                                Try
                                                    Dim enumerator11 As List(Of LocalResourceFile).Enumerator = dictionary2(Conversions.ToInteger(resourceProject2.Id)).GetEnumerator()
                                                    While enumerator11.MoveNext()
                                                        Dim CS$<>8__locals7 As LocalResourceLoaders._Closure$__4-5 = New LocalResourceLoaders._Closure$__4-5(CS$<>8__locals7)
                                                        CS$<>8__locals7.$VB$NonLocal_$VB$Closure_5 = CS$<>8__locals6
                                                        CS$<>8__locals7.$VB$Local_LocalFile = enumerator11.Current
                                                        Dim project As ResourceProject = CS$<>8__locals7.$VB$Local_LocalFile.Project
                                                        If project IsNot Nothing AndAlso project.Platform = ResourcePlatforms.Modrinth Then
                                                            CS$<>8__locals7.$VB$Local_LocalFile.Project = CS$<>8__locals7.$VB$Local_LocalFile.Project
                                                        Else
                                                            CS$<>8__locals7.$VB$Local_LocalFile.Project = resourceProject2
                                                        End If
                                                        Dim enumerable6 As IEnumerable(Of JToken) = jtoken4("latestFilesIndexes").Where(Function(Entry As JToken)
                                                            Dim modLoaders2 As ModLoaders = If((Entry("modLoader") Is Nothing), ModLoaders.None, Resource.FromCurseForgeModLoaderType(Entry("modLoader").ToObject(Of Integer)()))
                                                            If CS$<>8__locals7.$VB$NonLocal_$VB$Closure_5.$VB$Local_CurseForgeLoaders(CS$<>8__locals7.$VB$Local_LocalFile) = ModLoaders.None Then
                                                                If modLoaders2 <> ModLoaders.None Then
                                                                    Return False
                                                                End If
                                                            ElseIf Not CS$<>8__locals7.$VB$NonLocal_$VB$Closure_5.$VB$Local_CurseForgeLoaders(CS$<>8__locals7.$VB$Local_LocalFile).HasFlagF(modLoaders2) Then
                                                                Return False
                                                            End If
                                                            Dim text6 As String = CStr(Entry("gameVersion"))
                                                            Return text6 IsNot Nothing AndAlso Operators.CompareString(text6, CS$<>8__locals7.$VB$NonLocal_$VB$Closure_5.$VB$NonLocal_$VB$Closure_4.$VB$Local_VanillaVersion, False) = 0
                                                        End Function)
                                                        Dim func19 As Func(Of JToken, String)
                                                        If LocalResourceLoaders._Closure$__.$I4-13 IsNot Nothing Then
                                                            func19 = LocalResourceLoaders._Closure$__.$I4-13
                                                        Else
                                                            Dim func20 As Func(Of JToken, String) = Function(Entry As JToken) Entry("gameVersion").ToString()
                                                            func19 = func20
                                                            LocalResourceLoaders._Closure$__.$I4-13 = func20
                                                        End If
                                                        Dim comparison As Comparison(Of String)
                                                        If LocalResourceLoaders._Closure$__.$I4-14 IsNot Nothing Then
                                                            comparison = LocalResourceLoaders._Closure$__.$I4-14
                                                        Else
                                                            Dim comparison2 As Comparison(Of String) = Function(x As String, y As String) McUtils.CompareVersion(x, y)
                                                            comparison = comparison2
                                                            LocalResourceLoaders._Closure$__.$I4-14 = comparison2
                                                        End If
                                                        Dim list4 As List(Of JToken) = enumerable6.MaxByAll(func19, Comparer(Of String).Create(comparison))
                                                        Try
                                                            For Each jtoken5 As JToken In list4
                                                                Dim num2 As Integer = jtoken5("fileId").ToObject(Of Integer)()
                                                                If Not dictionary3.ContainsKey(num2) Then
                                                                    dictionary3(num2) = New List(Of LocalResourceFile)()
                                                                End If
                                                                dictionary3(num2).Add(CS$<>8__locals7.$VB$Local_LocalFile)
                                                                dictionary4(num2) = resourceProject2.Slug
                                                            Next
                                                        Finally
                                                            Dim enumerator12 As List(Of JToken).Enumerator
                                                            CType(enumerator12, IDisposable).Dispose()
                                                        End Try
                                                    End While
                                                Finally
                                                    Dim enumerator11 As List(Of LocalResourceFile).Enumerator
                                                    CType(enumerator11, IDisposable).Dispose()
                                                End Try
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator10 As IEnumerator(Of JToken)
                                        If enumerator10 IsNot Nothing Then
                                            enumerator10.Dispose()
                                        End If
                                    End Try
                                    Logger.Info(String.Format("已从 CurseForge 获取本地 Mod 信息，需要获取 {0} 个用于检查更新的文件信息", dictionary3.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                    If dictionary3.Any() Then
                                        Dim jtoken6 As JToken = ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/files", HttpMethod.Post, String.Format("{{""fileIds"": [{0}]}}", dictionary3.Keys.Join(",")), "application/json")("data")
                                        Dim dictionary5 As Dictionary(Of LocalResourceFile, ResourceVersion) = New Dictionary(Of LocalResourceFile, ResourceVersion)()
                                        Try
                                            For Each jtoken7 As JToken In jtoken6
                                                Dim resourceVersion3 As ResourceVersion = ResourceVersion.FromPlatformJson(CType(jtoken7, JObject), ResourceTypes.[Mod])
                                                If resourceVersion3.DownloadAvailable Then
                                                    Try
                                                        For Each localResourceFile8 As LocalResourceFile In dictionary3(Conversions.ToInteger(resourceVersion3.Id))
                                                            If resourceVersion3.ModLoaders <> CS$<>8__locals6.$VB$Local_CurseForgeLoaders(localResourceFile8) Then
                                                                Logger.Warn(String.Format("从 CurseForge 获取到的更新文件 {0} 加载器为 {1}，与本地文件 {2} 的加载器 {3} 不一致，已忽略", New Object() { resourceVersion3.FileName, resourceVersion3.ModLoaders, localResourceFile8.File.Name, CS$<>8__locals6.$VB$Local_CurseForgeLoaders(localResourceFile8) }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                                            ElseIf Not dictionary5.ContainsKey(localResourceFile8) OrElse DateTime.Compare(dictionary5(localResourceFile8).ReleaseDate, resourceVersion3.ReleaseDate) < 0 Then
                                                                dictionary5(localResourceFile8) = resourceVersion3
                                                            End If
                                                        Next
                                                    Finally
                                                        Dim enumerator14 As List(Of LocalResourceFile).Enumerator
                                                        CType(enumerator14, IDisposable).Dispose()
                                                    End Try
                                                End If
                                            Next
                                        Finally
                                            Dim enumerator13 As IEnumerator(Of JToken)
                                            If enumerator13 IsNot Nothing Then
                                                enumerator13.Dispose()
                                            End If
                                        End Try
                                        Try
                                            For Each keyValuePair As KeyValuePair(Of LocalResourceFile, ResourceVersion) In dictionary5
                                                Dim CS$<>8__locals8 As LocalResourceLoaders._Closure$__4-6 = New LocalResourceLoaders._Closure$__4-6(CS$<>8__locals8)
                                                CS$<>8__locals8.$VB$Local_Entry = keyValuePair.Key
                                                CS$<>8__locals8.$VB$Local_UpdateFile = keyValuePair.Value
                                                Logger.Trace(Function() String.Format("本地文件 {0} 在 CurseForge 上的最新版为 {1}", CS$<>8__locals8.$VB$Local_Entry.ProjectVersion.FileName, CS$<>8__locals8.$VB$Local_UpdateFile.FileName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                                If DateTime.Compare(CS$<>8__locals8.$VB$Local_Entry.ProjectVersion.ReleaseDate, CS$<>8__locals8.$VB$Local_UpdateFile.ReleaseDate) < 0 AndAlso Operators.CompareString(CS$<>8__locals8.$VB$Local_Entry.ProjectVersion.Hash, CS$<>8__locals8.$VB$Local_UpdateFile.Hash, False) <> 0 Then
                                                    If CS$<>8__locals8.$VB$Local_Entry.UpdateFile IsNot Nothing AndAlso Operators.CompareString(CS$<>8__locals8.$VB$Local_UpdateFile.Hash, CS$<>8__locals8.$VB$Local_Entry.UpdateFile.Hash, False) = 0 Then
                                                        CS$<>8__locals8.$VB$Local_Entry.ChangelogUrls.Add(String.Format("https://www.curseforge.com/minecraft/mc-mods/{0}/files/{1}", dictionary4(Conversions.ToInteger(CS$<>8__locals8.$VB$Local_UpdateFile.Id)), CS$<>8__locals8.$VB$Local_UpdateFile.Id))
                                                        CS$<>8__locals8.$VB$Local_Entry.UpdateFile.DownloadUrls.AddRange(CS$<>8__locals8.$VB$Local_UpdateFile.DownloadUrls)
                                                    ElseIf CS$<>8__locals8.$VB$Local_Entry.UpdateFile Is Nothing OrElse DateTime.Compare(CS$<>8__locals8.$VB$Local_UpdateFile.ReleaseDate, CS$<>8__locals8.$VB$Local_Entry.UpdateFile.ReleaseDate) > 0 Then
                                                        CS$<>8__locals8.$VB$Local_Entry.ChangelogUrls = New List(Of String)() From { String.Format("https://www.curseforge.com/minecraft/mc-mods/{0}/files/{1}", dictionary4(Conversions.ToInteger(CS$<>8__locals8.$VB$Local_UpdateFile.Id)), CS$<>8__locals8.$VB$Local_UpdateFile.Id) }
                                                        CS$<>8__locals8.$VB$Local_Entry.UpdateFile = CS$<>8__locals8.$VB$Local_UpdateFile
                                                    End If
                                                End If
                                            Next
                                        Finally
                                            Dim enumerator15 As Dictionary(Of LocalResourceFile, ResourceVersion).Enumerator
                                            CType(enumerator15, IDisposable).Dispose()
                                        End Try
                                        Logger.Info("从 CurseForge 获取 Mod 更新信息结束", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                                    End If
                                End If
                            End If
                        End If
                    Catch ex2 As Exception
                        Logger.Warn(ex2, "从 CurseForge 获取本地 Mod 信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                        CS$<>8__locals1.$VB$Local_IsFailed = True
                    Finally
                        CS$<>8__locals1.$VB$Local_EndedThreadCount += 1
                    End Try
                End Sub, "Mod List Detail Loader CurseForge", ThreadPriority.Normal)
                While CS$<>8__locals1.$VB$Local_EndedThreadCount <> 2
                    Thread.Sleep(10)
                    If CS$<>8__locals1.$VB$Local_Loader.IsCanceled Then
                        Return
                    End If
                End While
                Dim CS$<>8__locals2 As LocalResourceLoaders._Closure$__4-0 = CS$<>8__locals1
                Dim $VB$Local_Mods3 As IEnumerable(Of LocalResourceFile) = CS$<>8__locals1.$VB$Local_Mods
                Dim func5 As Func(Of LocalResourceFile, Boolean)
                If LocalResourceLoaders._Closure$__.$I4-16 IsNot Nothing Then
                    func5 = LocalResourceLoaders._Closure$__.$I4-16
                Else
                    Dim func6 As Func(Of LocalResourceFile, Boolean) = Function(m As LocalResourceFile) m.Project IsNot Nothing
                    func5 = func6
                    LocalResourceLoaders._Closure$__.$I4-16 = func6
                End If
                CS$<>8__locals2.$VB$Local_Mods = $VB$Local_Mods3.Where(func5).ToList()
                Logger.Info(String.Format("联网获取本地 Mod 信息完成，为 {0} 个 Mod 更新缓存", CS$<>8__locals1.$VB$Local_Mods.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\LocalResourceLoaders.vb")
                If CS$<>8__locals1.$VB$Local_Mods.Any() AndAlso Not CS$<>8__locals1.$VB$Local_Loader.IsCanceled Then
                    Try
                        For Each localResourceFile2 As LocalResourceFile In CS$<>8__locals1.$VB$Local_Mods
                            localResourceFile2.OnlineDataLoaded = Not CS$<>8__locals1.$VB$Local_IsFailed
                            item(localResourceFile2.ModrinthHash + CS$<>8__locals1.$VB$Local_VanillaVersion + Conversions.ToString(CInt(modLoaders))) = localResourceFile2.ToJson()
                        Next
                    Finally
                        Dim enumerator2 As List(Of LocalResourceFile).Enumerator
                        CType(enumerator2, IDisposable).Dispose()
                    End Try
                    FileUtils.Write(ModBase.PathTemp + "Cache\LocalMod.json", item.ToString(If(ModBase.ModeDebug, 1, 0), New JsonConverter(-1) {}), Nothing)
                    If Not CS$<>8__locals1.$VB$Local_Loader.IsCanceled Then
                        Dim frmInstanceMod As PageInstanceMod = ModMain.FrmInstanceMod
                        If frmInstanceMod IsNot Nothing AndAlso frmInstanceMod.Filter = PageInstanceMod.FilterType.CanUpdate Then
                            Dim action As Action
                            If LocalResourceLoaders._Closure$__.$I4-17 IsNot Nothing Then
                                action = LocalResourceLoaders._Closure$__.$I4-17
                            Else
                                Dim action2 As Action = Sub()
                                    Dim frmInstanceMod2 As PageInstanceMod = ModMain.FrmInstanceMod
                                    If frmInstanceMod2 Is Nothing Then
                                        Return
                                    End If
                                    frmInstanceMod2.RefreshUI()
                                End Sub
                                action = action2
                                LocalResourceLoaders._Closure$__.$I4-17 = action2
                            End If
                            ModBase.RunInUi(action, False)
                            Return
                        End If
                        Dim action3 As Action
                        If LocalResourceLoaders._Closure$__.$I4-18 IsNot Nothing Then
                            action3 = LocalResourceLoaders._Closure$__.$I4-18
                        Else
                            Dim action4 As Action = Sub()
                                Dim frmInstanceMod3 As PageInstanceMod = ModMain.FrmInstanceMod
                                If frmInstanceMod3 Is Nothing Then
                                    Return
                                End If
                                frmInstanceMod3.RefreshBars()
                            End Sub
                            action3 = action4
                            LocalResourceLoaders._Closure$__.$I4-18 = action4
                        End If
                        ModBase.RunInUi(action3, False)
                    End If
                End If
            End If
        End Sub

        Public LocalResourceLoader As ModLoader.LoaderTask(Of String, List(Of LocalResourceFile)) = New ModLoader.LoaderTask(Of String, List(Of LocalResourceFile))("LocalResourceLoader", AddressOf LocalResourceLoaders.LocalResourceFileLoad, Nothing, ThreadPriority.Normal)

        <TupleElementNames(New String() { "ModList", "Cache", "Target" })>
        Public LocalResourceOnlineLoader As ModLoader.LoaderTask(Of ValueTuple(Of List(Of LocalResourceFile), JObject, McVersion), Integer) = New ModLoader.LoaderTask(Of ValueTuple(Of List(Of LocalResourceFile), JObject, McVersion), Integer)("LocalResourceOnlineLoader", AddressOf LocalResourceLoaders.LocalResourceOnlineLoad, Nothing, ThreadPriority.Normal)

        Private $STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile As Func(Of String, Boolean)

        Private $STATIC$LocalResourceFileLoad$011151286FC2E1512816911280C8$IsModFile$Init As StaticLocalInitFlag
    End Module
End Namespace
