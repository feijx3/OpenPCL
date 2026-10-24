Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public Class ResourceVersion
        Public Sub New()
            Me.ModLoaders = ModLoaders.None
            Me.FileName = Nothing
            Me.Hash = Nothing
            Me.Size = -1
            Me.RawDependencies = New List(Of String)()
            Me.Dependencies = New List(Of String)()
        End Sub

        Public ReadOnly Property ReleaseTypeDisplay As String
            Get
                Dim releaseType As ResourceVersion.ReleaseTypes = Me.ReleaseType
                Dim text As String
                If releaseType <> ResourceVersion.ReleaseTypes.Release Then
                    If releaseType <> ResourceVersion.ReleaseTypes.Beta Then
                        text = If(ModBase.ModeDebug, "Alpha 版", "早期测试版")
                    Else
                        text = If(ModBase.ModeDebug, "Beta 版", "测试版")
                    End If
                Else
                    text = "正式版"
                End If
                Return text
            End Get
        End Property

        Public ReadOnly Property DownloadAvailable As Boolean
            Get
                Return Me.FileName IsNot Nothing AndAlso Me.DownloadUrls IsNot Nothing
            End Get
        End Property

        Public Shared Function FromPlatformJson(Data As JObject, ExpectedResourceType As ResourceTypes) As ResourceVersion
            Dim resourceVersion As ResourceVersion = New ResourceVersion()
            Dim resourceVersion2 As ResourceVersion = resourceVersion
            resourceVersion2.ResourceType = ExpectedResourceType
            If Data.ContainsKey("gameId") Then
                resourceVersion2.Id = CStr(Data("id"))
                resourceVersion2.Display = Data("displayName").ToString().Replace(vbTab, "").Trim(New Char() { " "c })
                resourceVersion2.Version = Nothing
                resourceVersion2.ReleaseDate = CType(Data("fileDate"), DateTime)
                resourceVersion2.ReleaseType = CType(Data("releaseType").ToObject(Of Integer)(), ResourceVersion.ReleaseTypes)
                resourceVersion2.DownloadCount = CInt(Data("downloadCount"))
                resourceVersion2.FileName = CStr(Data("fileName"))
                resourceVersion2.Size = CInt(Data("fileLength"))
                If resourceVersion2.Size = 0 Then
                    resourceVersion2.Size = -1
                End If
                Dim resourceVersion3 As ResourceVersion = resourceVersion2
                Dim enumerable As IEnumerable(Of JToken) = CType(Data("hashes"), JArray).ToList()
                Dim func As Func(Of JToken, Boolean)
                If ResourceVersion._Closure$__.$I22-0 IsNot Nothing Then
                    func = ResourceVersion._Closure$__.$I22-0
                Else
                    Dim func2 As Func(Of JToken, Boolean) = Function(s As JToken) s("algo").ToObject(Of Integer)() = 1
                    func = func2
                    ResourceVersion._Closure$__.$I22-0 = func2
                End If
                Dim jtoken As JToken = enumerable.FirstOrDefault(func)
                Dim text As String
                If jtoken Is Nothing Then
                    text = Nothing
                Else
                    Dim jtoken2 As JToken = jtoken("value")
                    text = If((jtoken2 IsNot Nothing), jtoken2.ToString().Lower(), Nothing)
                End If
                resourceVersion3.Hash = text
                If resourceVersion2.Hash Is Nothing Then
                    Dim resourceVersion4 As ResourceVersion = resourceVersion2
                    Dim enumerable2 As IEnumerable(Of JToken) = CType(Data("hashes"), JArray).ToList()
                    Dim func3 As Func(Of JToken, Boolean)
                    If ResourceVersion._Closure$__.$I22-1 IsNot Nothing Then
                        func3 = ResourceVersion._Closure$__.$I22-1
                    Else
                        Dim func4 As Func(Of JToken, Boolean) = Function(s As JToken) s("algo").ToObject(Of Integer)() = 2
                        func3 = func4
                        ResourceVersion._Closure$__.$I22-1 = func4
                    End If
                    Dim jtoken3 As JToken = enumerable2.FirstOrDefault(func3)
                    Dim text2 As String
                    If jtoken3 Is Nothing Then
                        text2 = Nothing
                    Else
                        Dim jtoken4 As JToken = jtoken3("value")
                        text2 = If((jtoken4 IsNot Nothing), jtoken4.ToString().Lower(), Nothing)
                    End If
                    resourceVersion4.Hash = text2
                End If
                Dim text3 As String = Data("downloadUrl").ToString()
                If Operators.CompareString(text3, "", False) = 0 Then
                    text3 = String.Format("https://edge.forgecdn.net/files/{0}/{1}/{2}", Conversions.ToInteger(resourceVersion2.Id.ToString().Substring(0, 4)), Conversions.ToInteger(resourceVersion2.Id.ToString().Substring(4)), resourceVersion2.FileName)
                End If
                resourceVersion2.DownloadUrls = ResourceVersion.ParseCurseForgeDownloadUrls(text3.Replace(resourceVersion2.FileName, StringUtils.UrlEscape(resourceVersion2.FileName)))
                resourceVersion2.DownloadUrls.Add(ModDownload.DlSourceModGet(text3))
                resourceVersion2.DownloadUrls = resourceVersion2.DownloadUrls.Distinct().ToList()
                If Data.ContainsKey("dependencies") Then
                    Dim resourceVersion5 As ResourceVersion = resourceVersion2
                    Dim enumerable3 As IEnumerable(Of JToken) = Data("dependencies")
                    Dim func5 As Func(Of JToken, Boolean)
                    If ResourceVersion._Closure$__.$I22-2 IsNot Nothing Then
                        func5 = ResourceVersion._Closure$__.$I22-2
                    Else
                        Dim func6 As Func(Of JToken, Boolean) = Function(d As JToken) d("relationType").ToObject(Of Integer)() = 3 AndAlso d("modId").ToObject(Of Integer)() <> 306612 AndAlso d("modId").ToObject(Of Integer)() <> 634179
                        func5 = func6
                        ResourceVersion._Closure$__.$I22-2 = func6
                    End If
                    Dim enumerable4 As IEnumerable(Of JToken) = enumerable3.Where(func5)
                    Dim func7 As Func(Of JToken, String)
                    If ResourceVersion._Closure$__.$I22-3 IsNot Nothing Then
                        func7 = ResourceVersion._Closure$__.$I22-3
                    Else
                        Dim func8 As Func(Of JToken, String) = Function(d As JToken) d("modId").ToString()
                        func7 = func8
                        ResourceVersion._Closure$__.$I22-3 = func8
                    End If
                    resourceVersion5.RawDependencies = enumerable4.[Select](func7).ToList()
                End If
                Dim enumerable5 As IEnumerable(Of JToken) = Data("gameVersions")
                Dim func9 As Func(Of JToken, String)
                If ResourceVersion._Closure$__.$I22-4 IsNot Nothing Then
                    func9 = ResourceVersion._Closure$__.$I22-4
                Else
                    Dim func10 As Func(Of JToken, String) = Function(t As JToken) t.ToString().Trim().Lower()
                    func9 = func10
                    ResourceVersion._Closure$__.$I22-4 = func10
                End If
                Dim list As List(Of String) = enumerable5.[Select](func9).ToList()
                Dim resourceVersion6 As ResourceVersion = resourceVersion2
                Dim enumerable6 As IEnumerable(Of String) = list
                Dim func11 As Func(Of String, Boolean)
                If ResourceVersion._Closure$__.$I22-5 IsNot Nothing Then
                    func11 = ResourceVersion._Closure$__.$I22-5
                Else
                    Dim func12 As Func(Of String, Boolean) = Function(v As String) McVersion.IsStandardFormat(v)
                    func11 = func12
                    ResourceVersion._Closure$__.$I22-5 = func12
                End If
                Dim enumerable7 As IEnumerable(Of String) = enumerable6.Where(func11)
                Dim func13 As Func(Of String, String)
                If ResourceVersion._Closure$__.$I22-6 IsNot Nothing Then
                    func13 = ResourceVersion._Closure$__.$I22-6
                Else
                    Dim func14 As Func(Of String, String) = Function(v As String) v.Replace("-snapshot", " 预览版")
                    func13 = func14
                    ResourceVersion._Closure$__.$I22-6 = func14
                End If
                resourceVersion6.GameVersions = enumerable7.[Select](func13).Distinct().ToList()
                If resourceVersion2.GameVersions.IsSingle() Then
                    resourceVersion2.GameVersions = resourceVersion2.GameVersions.ToList()
                ElseIf resourceVersion2.GameVersions.Count > 1 Then
                    resourceVersion2.GameVersions = resourceVersion2.GameVersions.SortByComparison(New Func(Of String, String, Boolean)(McUtils.CompareVersionGE)).ToList()
                    If resourceVersion2.ResourceType = ResourceTypes.ModPack Then
                        resourceVersion2.GameVersions = New String() { resourceVersion2.GameVersions(0) }.ToList()
                    End If
                Else
                    resourceVersion2.GameVersions = New List(Of String)() From { "未知版本" }
                End If
                resourceVersion2.ModLoaders = ModLoaders.None
                Try
                    For Each modLoaders As ModLoaders In EnumUtils.GetAllFlags(Of ModLoaders)()
                        If list.Contains(modLoaders.ToString().Lower()) Then
                            resourceVersion2.ModLoaders = resourceVersion2.ModLoaders Or modLoaders
                        End If
                    Next
                    GoTo IL_0947
                Finally
                    Dim enumerator As List(Of ModLoaders).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
            End If
            resourceVersion2.Id = CStr(Data("id"))
            resourceVersion2.Display = Data("name").ToString().Replace(vbTab, "").Trim(New Char() { " "c })
            resourceVersion2.Version = CStr(Data("version_number"))
            resourceVersion2.ReleaseDate = CType(Data("date_published"), DateTime)
            resourceVersion2.ReleaseType = If((Operators.CompareString(Data("version_type").ToString(), "release", False) = 0), ResourceVersion.ReleaseTypes.Release, If((Operators.CompareString(Data("version_type").ToString(), "beta", False) = 0), ResourceVersion.ReleaseTypes.Beta, ResourceVersion.ReleaseTypes.Alpha))
            resourceVersion2.DownloadCount = CInt(Data("downloads"))
            If CType(Data("files"), JArray).Any() Then
                Dim jtoken5 As JToken = Data("files")(0)
                resourceVersion2.FileName = CStr(jtoken5("filename"))
                resourceVersion2.DownloadUrls = New List(Of String)() From { CStr(jtoken5("url")), ModDownload.DlSourceModGet(CStr(jtoken5("url"))) }.Distinct().ToList()
                resourceVersion2.Size = CInt(jtoken5("size"))
                Dim resourceVersion7 As ResourceVersion = resourceVersion2
                Dim jtoken6 As JToken = jtoken5("hashes")("sha1")
                resourceVersion7.Hash = If((jtoken6 IsNot Nothing), jtoken6.ToString().Lower(), Nothing)
            End If
            Dim enumerable8 As IEnumerable(Of JToken) = Data("loaders")
            Dim func15 As Func(Of JToken, String)
            If ResourceVersion._Closure$__.$I22-7 IsNot Nothing Then
                func15 = ResourceVersion._Closure$__.$I22-7
            Else
                Dim func16 As Func(Of JToken, String) = Function(v As JToken) v.ToString()
                func15 = func16
                ResourceVersion._Closure$__.$I22-7 = func16
            End If
            Dim list2 As List(Of String) = enumerable8.[Select](func15).ToList()
            resourceVersion2.ModLoaders = ModLoaders.None
            Try
                For Each modLoaders2 As ModLoaders In EnumUtils.GetAllFlags(Of ModLoaders)()
                    If list2.Contains(modLoaders2.ToString().Lower()) Then
                        resourceVersion2.ModLoaders = resourceVersion2.ModLoaders Or modLoaders2
                    End If
                Next
            Finally
                Dim enumerator2 As List(Of ModLoaders).Enumerator
                CType(enumerator2, IDisposable).Dispose()
            End Try
            If resourceVersion2.ResourceType.HasFlagF(ResourceTypes.[Mod]) OrElse resourceVersion2.ResourceType.HasFlagF(ResourceTypes.DataPack) Then
                If list2.Intersect(New String() { "bukkit", "folia", "paper", "purpur", "spigot" }).Any() Then
                    resourceVersion2.ResourceType = ResourceTypes.Plugin
                End If
                If list2.Contains("datapack") Then
                    resourceVersion2.ResourceType = ResourceTypes.DataPack
                End If
                If resourceVersion2.ModLoaders.Flags().Any() Then
                    resourceVersion2.ResourceType = ResourceTypes.[Mod]
                End If
            End If
            If Data.ContainsKey("dependencies") Then
                Dim resourceVersion8 As ResourceVersion = resourceVersion2
                Dim enumerable9 As IEnumerable(Of JToken) = Data("dependencies")
                Dim func17 As Func(Of JToken, Boolean)
                If ResourceVersion._Closure$__.$I22-8 IsNot Nothing Then
                    func17 = ResourceVersion._Closure$__.$I22-8
                Else
                    Dim func18 As Func(Of JToken, Boolean) = Function(d As JToken) CStr(d("dependency_type")) = "required" AndAlso CStr(d("project_id")) <> "P7dR8mSH" AndAlso CStr(d("project_id")) <> "qvIfYCYJ" AndAlso d("project_id").ToString().Length > 0
                    func17 = func18
                    ResourceVersion._Closure$__.$I22-8 = func18
                End If
                Dim enumerable10 As IEnumerable(Of JToken) = enumerable9.Where(func17)
                Dim func19 As Func(Of JToken, String)
                If ResourceVersion._Closure$__.$I22-9 IsNot Nothing Then
                    func19 = ResourceVersion._Closure$__.$I22-9
                Else
                    Dim func20 As Func(Of JToken, String) = Function(d As JToken) d("project_id").ToString()
                    func19 = func20
                    ResourceVersion._Closure$__.$I22-9 = func20
                End If
                resourceVersion8.RawDependencies = enumerable10.[Select](func19).ToList()
            End If
            Dim enumerable11 As IEnumerable(Of JToken) = Data("game_versions")
            Dim func21 As Func(Of JToken, String)
            If ResourceVersion._Closure$__.$I22-10 IsNot Nothing Then
                func21 = ResourceVersion._Closure$__.$I22-10
            Else
                Dim func22 As Func(Of JToken, String) = Function(t As JToken) t.ToString().Trim().Lower()
                func21 = func22
                ResourceVersion._Closure$__.$I22-10 = func22
            End If
            Dim list3 As List(Of String) = enumerable11.[Select](func21).ToList()
            Dim resourceVersion9 As ResourceVersion = resourceVersion2
            Dim enumerable12 As IEnumerable(Of String) = list3
            Dim func23 As Func(Of String, Boolean)
            If ResourceVersion._Closure$__.$I22-11 IsNot Nothing Then
                func23 = ResourceVersion._Closure$__.$I22-11
            Else
                Dim func24 As Func(Of String, Boolean) = Function(v As String) v.Contains(".")
                func23 = func24
                ResourceVersion._Closure$__.$I22-11 = func24
            End If
            Dim enumerable13 As IEnumerable(Of String) = enumerable12.Where(func23)
            Dim func25 As Func(Of String, String)
            If ResourceVersion._Closure$__.$I22-12 IsNot Nothing Then
                func25 = ResourceVersion._Closure$__.$I22-12
            Else
                Dim func26 As Func(Of String, String) = Function(v As String)
                    If v.Contains("-") Then
                        Return v.BeforeFirst("-", False) + " 预览版"
                    End If
                    If Not v.StartsWithF("b1.", False) Then
                        Return v
                    End If
                    Return "远古版本"
                End Function
                func25 = func26
                ResourceVersion._Closure$__.$I22-12 = func26
            End If
            resourceVersion9.GameVersions = enumerable13.[Select](func25).Distinct().ToList()
            If Not resourceVersion2.GameVersions.IsSingle() Then
                If resourceVersion2.GameVersions.Count > 1 Then
                    resourceVersion2.GameVersions = resourceVersion2.GameVersions.SortByComparison(New Func(Of String, String, Boolean)(McUtils.CompareVersionGE)).ToList()
                    If resourceVersion2.ResourceType = ResourceTypes.ModPack Then
                        resourceVersion2.GameVersions = New List(Of String)() From { resourceVersion2.GameVersions(0) }
                    End If
                Else
                    Dim enumerable14 As IEnumerable(Of String) = list3
                    Dim func27 As Func(Of String, Boolean)
                    If ResourceVersion._Closure$__.$I22-13 IsNot Nothing Then
                        func27 = ResourceVersion._Closure$__.$I22-13
                    Else
                        Dim func28 As Func(Of String, Boolean) = Function(v As String) v.RegexCheck("[0-9]{2}w[0-9]{2}[a-z]", RegexOptions.None)
                        func27 = func28
                        ResourceVersion._Closure$__.$I22-13 = func28
                    End If
                    If enumerable14.Any(func27) Then
                        Dim resourceVersion10 As ResourceVersion = resourceVersion2
                        Dim enumerable15 As IEnumerable(Of String) = list3
                        Dim func29 As Func(Of String, Boolean)
                        If ResourceVersion._Closure$__.$I22-14 IsNot Nothing Then
                            func29 = ResourceVersion._Closure$__.$I22-14
                        Else
                            Dim func30 As Func(Of String, Boolean) = Function(v As String) v.RegexCheck("[0-9]{2}w[0-9]{2}[a-z]", RegexOptions.None)
                            func29 = func30
                            ResourceVersion._Closure$__.$I22-14 = func30
                        End If
                        resourceVersion10.GameVersions = enumerable15.Where(func29).ToList()
                    Else
                        resourceVersion2.GameVersions = New List(Of String)() From { "未知版本" }
                    End If
                End If
            End If
            IL_0947:
            resourceVersion2 = Nothing
            Return resourceVersion
        End Function

        Public Shared Function FromProjectId(ProjectId As String, Platform As ResourcePlatforms, Optional LoadDependencies As Boolean = True) As List(Of ResourceVersion)
            Dim CS$<>8__locals1 As ResourceVersion._Closure$__23-0 = New ResourceVersion._Closure$__23-0(CS$<>8__locals1)
            If Not Platform.Flags().IsSingle() Then
                Throw New ArgumentException(String.Format("必须指定单一平台，当前值为 {0}", Platform))
            End If
            CS$<>8__locals1.$VB$Local_TargetProject = ResourceProject.FromProjectId(ProjectId, Platform)
            If Not ResourceVersion.ProjectFilesCache.ContainsKey(ProjectId) Then
                Logger.Info(String.Format("开始获取项目版本列表：{0}", ProjectId), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceVersion.vb")
                Dim jarray As JArray = Nothing
                If Platform = ResourcePlatforms.CurseForge Then
                    Dim num As Integer = 0
                    Dim jobject As JObject
                    While True
                        jobject = CType(ModDownload.DlModRequest(String.Format("https://api.curseforge.com/v1/mods/{0}/files?pageSize=", ProjectId) + Conversions.ToString(10000 + num), Nothing, Nothing, Nothing), JObject)
                        If jobject("pagination")("resultCount").ToObject(Of Integer)() = jobject("pagination")("totalCount").ToObject(Of Integer)() Then
                            Exit For
                        End If
                        If num >= 2 Then
                            GoTo IL_0127
                        End If
                        Logger.Warn(String.Format("CurseForge 返回的资源版本列表存在缺失，即将进行第 {0} 次重试", num + 1), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceVersion.vb")
                        Logger.Info(String.Format("返回的原始内容如下：{0}{1}", vbCrLf, jobject), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceVersion.vb")
                        num += 1
                        If num > 2 Then
                            GoTo Block_6
                        End If
                    End While
                    jarray = CType(jobject("data"), JArray)
                    GoTo IL_0178
                    IL_0127:
                    Logger.Info(String.Format("CurseForge 返回的资源版本列表存在缺失，返回的原始内容如下：{0}{1}", vbCrLf, jobject), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceVersion.vb")
                    Throw New Exception("CurseForge 返回的资源版本列表存在缺失")
                    Block_6:
                Else
                    jarray = CType(ModDownload.DlModRequest(String.Format("https://api.modrinth.com/v2/project/{0}/version?include_changelog=false", ProjectId), Nothing, Nothing, Nothing), JArray)
                End If
                IL_0178:
                Dim projectFilesCache As Dictionary(Of String, List(Of ResourceVersion)) = ResourceVersion.ProjectFilesCache
                Dim enumerable As IEnumerable(Of ResourceVersion) = jarray.[Select](Function(a As JToken) ResourceVersion.FromPlatformJson(CType(a, JObject), CS$<>8__locals1.$VB$Local_TargetProject.Types))
                Dim func As Func(Of ResourceVersion, Boolean)
                If ResourceVersion._Closure$__.$I23-1 IsNot Nothing Then
                    func = ResourceVersion._Closure$__.$I23-1
                Else
                    Dim func2 As Func(Of ResourceVersion, Boolean) = Function(a As ResourceVersion) a.DownloadAvailable
                    func = func2
                    ResourceVersion._Closure$__.$I23-1 = func2
                End If
                Dim enumerable2 As IEnumerable(Of ResourceVersion) = enumerable.Where(func)
                Dim func3 As Func(Of ResourceVersion, String)
                If ResourceVersion._Closure$__.$I23-2 IsNot Nothing Then
                    func3 = ResourceVersion._Closure$__.$I23-2
                Else
                    Dim func4 As Func(Of ResourceVersion, String) = Function(a As ResourceVersion) a.Id
                    func3 = func4
                    ResourceVersion._Closure$__.$I23-2 = func4
                End If
                projectFilesCache(ProjectId) = enumerable2.DistinctBy(func3).ToList()
            End If
            Dim list As List(Of ResourceVersion) = ResourceVersion.ProjectFilesCache(ProjectId)
            Dim list2 As List(Of ResourceVersion)
            If Not LoadDependencies Then
                list2 = list
            Else
                Dim enumerable3 As IEnumerable(Of ResourceVersion) = list
                Dim func5 As Func(Of ResourceVersion, IEnumerable(Of String))
                If ResourceVersion._Closure$__.$I23-3 IsNot Nothing Then
                    func5 = ResourceVersion._Closure$__.$I23-3
                Else
                    Dim func6 As Func(Of ResourceVersion, IEnumerable(Of String)) = Function(f As ResourceVersion) f.RawDependencies
                    func5 = func6
                    ResourceVersion._Closure$__.$I23-3 = func6
                End If
                Dim list3 As List(Of String) = enumerable3.SelectMany(func5).Distinct().ToList()
                If list3.Any() Then
                    Logger.Info(String.Format("项目 {0} 中需要加载的前置：{1}", ProjectId, list3.Join("，")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceVersion.vb")
                    Try
                        For Each resourceProject As ResourceProject In ResourceProject.FromProjectIds(list3, CS$<>8__locals1.$VB$Local_TargetProject.Platform)
                            Try
                                For Each resourceVersion As ResourceVersion In list
                                    If resourceVersion.RawDependencies.Contains(resourceProject.Id) AndAlso Operators.CompareString(resourceProject.Id, ProjectId, False) <> 0 AndAlso Not resourceVersion.Dependencies.Contains(resourceProject.Id) Then
                                        resourceVersion.Dependencies.Add(resourceProject.Id)
                                    End If
                                Next
                            Finally
                                Dim enumerator2 As List(Of ResourceVersion).Enumerator
                                CType(enumerator2, IDisposable).Dispose()
                            End Try
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of ResourceProject)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                End If
                list2 = list
            End If
            Return list2
        End Function

        Public Shared Function FromCacheJson(Data As JObject) As ResourceVersion
            Dim resourceVersion As ResourceVersion = New ResourceVersion()
            Dim resourceVersion2 As ResourceVersion = resourceVersion
            resourceVersion2.ResourceType = Data("ResourceType").ToObject(Of ResourceTypes)()
            resourceVersion2.Id = Data("Id").ToString()
            resourceVersion2.Display = Data("DisplayName").ToString()
            If Data.ContainsKey("Version") Then
                resourceVersion2.Version = Data("Version").ToString()
            End If
            resourceVersion2.ReleaseDate = Data("ReleaseDate").ToObject(Of DateTime)()
            resourceVersion2.DownloadCount = Data("DownloadCount").ToObject(Of Integer)()
            resourceVersion2.ReleaseType = Data("ReleaseType").ToObject(Of ResourceVersion.ReleaseTypes)()
            If Data.ContainsKey("FileName") Then
                resourceVersion2.FileName = Data("FileName").ToString()
            End If
            If Data.ContainsKey("DownloadUrls") Then
                resourceVersion2.DownloadUrls = Data("DownloadUrls").ToObject(Of List(Of String))()
            End If
            If Data.ContainsKey("ModLoaders") Then
                resourceVersion2.ModLoaders = Data("ModLoaders").ToObject(Of ModLoaders)()
            End If
            If Data.ContainsKey("Size") Then
                resourceVersion2.Size = Data("Size").ToObject(Of Integer)()
            End If
            If Data.ContainsKey("Hash") Then
                resourceVersion2.Hash = Data("Hash").ToString()
            End If
            If Data.ContainsKey("GameVersions") Then
                resourceVersion2.GameVersions = Data("GameVersions").ToObject(Of List(Of String))()
            End If
            If Data.ContainsKey("RawDependencies") Then
                resourceVersion2.RawDependencies = Data("RawDependencies").ToObject(Of List(Of String))()
            End If
            If Data.ContainsKey("Dependencies") Then
                resourceVersion2.Dependencies = Data("Dependencies").ToObject(Of List(Of String))()
            End If
            Return resourceVersion
        End Function

        Public Function ToCacheJson() As JObject
            Dim jobject As JObject = New JObject()
            Dim jobject2 As JObject = jobject
            jobject2.Add("ResourceType", CInt(Me.ResourceType))
            jobject2.Add("Id", Me.Id)
            If Me.Version IsNot Nothing Then
                jobject2.Add("Version", Me.Version)
            End If
            jobject2.Add("DisplayName", Me.Display)
            jobject2.Add("ReleaseDate", Me.ReleaseDate)
            jobject2.Add("DownloadCount", Me.DownloadCount)
            jobject2.Add("ModLoaders", CInt(Me.ModLoaders))
            jobject2.Add("GameVersions", New JArray(Me.GameVersions))
            jobject2.Add("ReleaseType", CInt(Me.ReleaseType))
            If Me.FileName IsNot Nothing Then
                jobject2.Add("FileName", Me.FileName)
            End If
            If Me.DownloadUrls IsNot Nothing Then
                jobject2.Add("DownloadUrls", New JArray(Me.DownloadUrls))
            End If
            If Me.Hash IsNot Nothing Then
                jobject2.Add("Hash", Me.Hash)
            End If
            If Me.Size >= 0 Then
                jobject2.Add("Size", Me.Size)
            End If
            jobject2.Add("RawDependencies", New JArray(Me.RawDependencies))
            jobject2.Add("Dependencies", New JArray(Me.Dependencies))
            Return jobject
        End Function

        Public Function ToListItem(ClickHandler As OurListItem.ClickEventHandler, Optional SaveAsButtonHandler As OurIconButton.ClickEventHandler = Nothing, Optional IsBadDisplay As Boolean = False) As OurVirtualizingElement(Of OurListItem)
            Dim CS$<>8__locals1 As ResourceVersion._Closure$__27-0 = New ResourceVersion._Closure$__27-0()
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_ClickHandler = ClickHandler
            CS$<>8__locals1.$VB$Local_SaveAsButtonHandler = SaveAsButtonHandler
            CS$<>8__locals1.$VB$Local_IsBadDisplay = IsBadDisplay
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem()
                Dim CS$<>8__locals2 As ResourceVersion._Closure$__27-1 = New ResourceVersion._Closure$__27-1()
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals1
                CS$<>8__locals2.$W2 = myListItem
                CS$<>8__locals2.$W2.SnapsToDevicePixels = True
                CS$<>8__locals2.$W2.Height = 42.0
                CS$<>8__locals2.$W2.Type = OurListItem.CheckType.Clickable
                CS$<>8__locals2.$W2.Tag = CS$<>8__locals1.$VB$Me
                CS$<>8__locals2.$W2.Title = If(CS$<>8__locals1.$VB$Local_IsBadDisplay, CS$<>8__locals1.$VB$Me.FileName, CS$<>8__locals1.$VB$Me.Display).BeforeLastOfAny(New String() { ".zip", ".jar", ".mrpack", ".litemod" }, True)
                CS$<>8__locals2.$W2.Logo = String.Format("{0}ReleaseTypes/{1}.png", ModBase.PathImage, CS$<>8__locals1.$VB$Me.ReleaseType)
                AddHandler CS$<>8__locals2.$W2.Click, CS$<>8__locals1.$VB$Local_ClickHandler
                CS$<>8__locals2.$W2.Info = CS$<>8__locals2._Lambda$__1().Join("，"c)
                If CS$<>8__locals1.$VB$Local_SaveAsButtonHandler IsNot Nothing Then
                    Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z", .ToolTip = "另存为" }
                    AddHandler myIconButton.Click, CS$<>8__locals1.$VB$Local_SaveAsButtonHandler
                    CS$<>8__locals2.$W2.Buttons = New OurIconButton() { myIconButton }
                End If
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Public Function ToNetFile(LocalFileOrFolder As String, Optional TrackDownloadReason As ResourceVersion.DownloadReason? = Nothing, Optional TrackGameVersion As String = Nothing, Optional TrackLoader As ModLoaders = ModLoaders.None) As ModNet.NetFile
            Return New ModNet.NetFile(ResourceVersion.ParseModrinthTrackArguments(Me.DownloadUrls, TrackDownloadReason, TrackGameVersion, TrackLoader), LocalFileOrFolder + If(LocalFileOrFolder.EndsWithF("\", False), Me.FileName, ""), New FileChecker() With { .Hash = Me.Hash, .ActualSize = CLng(Me.Size) }, True)
        End Function

        Public Shared Iterator Function ParseModrinthTrackArguments(Urls As List(Of String), Optional TrackDownloadReason As ResourceVersion.DownloadReason? = Nothing, Optional TrackGameVersion As String = Nothing, Optional TrackLoader As ModLoaders = ModLoaders.None) As IEnumerable(Of String)
            Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            If TrackDownloadReason IsNot Nothing Then
                dictionary.Add("mr_download_reason", TrackDownloadReason.ToString().Lower())
            End If
            If TrackGameVersion IsNot Nothing Then
                dictionary.Add("mr_game_version", TrackGameVersion)
            End If
            If TrackLoader.Flags().Any() Then
                dictionary.Add("mr_loader", TrackLoader.Flags().First().ToString().Lower())
            End If
            Dim enumerable As IEnumerable(Of KeyValuePair(Of String, String)) = dictionary
            Dim func As Func(Of KeyValuePair(Of String, String), String)
            If ResourceVersion._Closure$__.$I29-0 IsNot Nothing Then
                func = ResourceVersion._Closure$__.$I29-0
            Else
                Dim func2 As Func(Of KeyValuePair(Of String, String), String) = Function(a As KeyValuePair(Of String, String)) String.Format("{0}={1}", a.Key, a.Value)
                func = func2
                ResourceVersion._Closure$__.$I29-0 = func2
            End If
            Dim Argument As String = enumerable.[Select](func).Join("&")
            Try
                For Each Url As String In Urls
                    If Not Url.ContainsIgnoreCase("cdn.modrinth.com/") Then
                        Yield Url
                    End If
                    Yield String.Format("{0}{1}{2}", Url, If(Url.Contains("?"), "&", "?"), Argument)
                Next
                Return
            Finally
                Dim enumerator As List(Of String).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Return
        End Function

        Public Shared Function ParseCurseForgeDownloadUrls(Url As String) As List(Of String)
            Return New String() { Url.Replace("-service.overwolf.wtf", ".forgecdn.net").Replace("://edge.", "://mediafilez.").Replace("://media.", "://mediafilez."), Url.Replace("://edge.", "://mediafilez.").Replace("://media.", "://mediafilez."), Url.Replace("-service.overwolf.wtf", ".forgecdn.net"), Url.Replace("://media.", "://edge."), Url }.Distinct().ToList()
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Me.Id, Me.FileName)
        End Function

        Public Id As String

        Public ResourceType As ResourceTypes

        Public Display As String

        Public ReleaseDate As DateTime

        Public DownloadCount As Integer

        Public Version As String

        Public ModLoaders As ModLoaders

        Public GameVersions As List(Of String)

        Public ReleaseType As ResourceVersion.ReleaseTypes

        Public FileName As String

        Public DownloadUrls As List(Of String)

        Public Hash As String

        Public Size As Integer

        Private RawDependencies As List(Of String)

        Public Dependencies As List(Of String)

        Protected Friend Shared ProjectFilesCache As Dictionary(Of String, List(Of ResourceVersion)) = New Dictionary(Of String, List(Of ResourceVersion))()

        Public Enum ReleaseTypes
            Release = 1
            Beta
            Alpha
        End Enum

        Public Enum DownloadReason
            Standalone
            Dependency
            ModPack
            Update
        End Enum
    End Class
End Namespace
