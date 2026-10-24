Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Controls
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public Class ResourceProject
        Private Property WikiEntry As WikiEntry
            Get
                If Not Me.LoadedDatabase Then
                    Me.LoadedDatabase = True
                    If Me.Types.HasFlagF(ResourceTypes.[Mod]) OrElse Me.Types.HasFlagF(ResourceTypes.DataPack) Then
                        Me._WikiEntry = WikiEntry.All.Value.FirstOrDefault(Function(c As WikiEntry) Operators.CompareString(c.Slugs.GetOrDefault(Me.Platform, Nothing), Me.Slug, False) = 0)
                    End If
                End If
                Return Me._WikiEntry
            End Get
            Set(value As WikiEntry)
                Me.LoadedDatabase = True
                Me._WikiEntry = value
            End Set
        End Property

        Public ReadOnly Property WikiId As Integer
            Get
                If Me.WikiEntry IsNot Nothing Then
                    Return Me.WikiEntry.Id
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property TranslatedName As String
            Get
                Dim wikiEntry As WikiEntry = Me.WikiEntry
                Return If(If((wikiEntry IsNot Nothing), wikiEntry.ChineseName, Nothing), Me.RawName)
            End Get
        End Property

        Public Sub New(Data As JObject)
            Me.ModLoaders = ModLoaders.None
            Me.LogoUrl = Nothing
            Me.LoadedDatabase = False
            Me._WikiEntry = Nothing
            If Data.ContainsKey("Tags") Then
                Me.Platform = CType(Data("Platform").ToObject(Of Integer)(), ResourcePlatforms)
                Me.Types = CType(Data("Types").ToObject(Of Integer)(), ResourceTypes)
                Me.Slug = CStr(Data("Slug"))
                Me.Id = CStr(Data("Id"))
                If Data.ContainsKey("CurseForgeFileIds") Then
                    Dim enumerable As IEnumerable(Of JToken) = CType(Data("CurseForgeFileIds"), JArray)
                    Dim func As Func(Of JToken, Integer)
                    If ResourceProject._Closure$__.$I25-0 IsNot Nothing Then
                        func = ResourceProject._Closure$__.$I25-0
                    Else
                        Dim func2 As Func(Of JToken, Integer) = Function(t As JToken) t.ToObject(Of Integer)()
                        func = func2
                        ResourceProject._Closure$__.$I25-0 = func2
                    End If
                    Me.CurseForgeFileIds = enumerable.[Select](func).ToList()
                End If
                Me.RawName = CStr(Data("RawName"))
                Me.Description = CStr(Data("Description"))
                Me.Website = CStr(Data("Website"))
                Me.LastUpdate = CType(Data("LastUpdate"), DateTime)
                Me.DownloadCount = CInt(Data("DownloadCount"))
                If Data.ContainsKey("ModLoaders") Then
                    Me.ModLoaders = CType(Data("ModLoaders").ToObject(Of Integer)(), ModLoaders)
                End If
                Dim enumerable2 As IEnumerable(Of JToken) = CType(Data("Tags"), JArray)
                Dim func3 As Func(Of JToken, String)
                If ResourceProject._Closure$__.$I25-1 IsNot Nothing Then
                    func3 = ResourceProject._Closure$__.$I25-1
                Else
                    Dim func4 As Func(Of JToken, String) = Function(t As JToken) t.ToString()
                    func3 = func4
                    ResourceProject._Closure$__.$I25-1 = func4
                End If
                Me.Tags = enumerable2.[Select](func3).ToList()
                If Data.ContainsKey("LogoUrl") Then
                    Me.LogoUrl = CStr(Data("LogoUrl"))
                End If
                If Data.ContainsKey("Drops") Then
                    Dim enumerable3 As IEnumerable(Of JToken) = CType(Data("Drops"), JArray)
                    Dim func5 As Func(Of JToken, Integer)
                    If ResourceProject._Closure$__.$I25-2 IsNot Nothing Then
                        func5 = ResourceProject._Closure$__.$I25-2
                    Else
                        Dim func6 As Func(Of JToken, Integer) = Function(t As JToken) t.ToObject(Of Integer)()
                        func5 = func6
                        ResourceProject._Closure$__.$I25-2 = func6
                    End If
                    Me.Drops = enumerable3.[Select](func5).ToList()
                Else
                    Me.Drops = New List(Of Integer)()
                End If
            Else
                Me.Platform = If(Data.ContainsKey("summary"), ResourcePlatforms.CurseForge, ResourcePlatforms.Modrinth)
                If Me.Platform = ResourcePlatforms.CurseForge Then
                    Me.Id = CStr(Data("id"))
                    Me.Slug = CStr(Data("slug"))
                    Me.RawName = CStr(Data("name"))
                    Me.Description = CStr(Data("summary"))
                    Me.Website = Data("links")("websiteUrl").ToString().TrimEnd(New Char() { "/"c })
                    Me.LastUpdate = CType(Data("dateReleased"), DateTime)
                    Me.DownloadCount = CInt(Data("downloadCount"))
                    If Data("logo").Count() > 0 Then
                        If Data("logo")("thumbnailUrl") Is Nothing OrElse CStr(Data("logo")("thumbnailUrl")) = "" Then
                            Me.LogoUrl = CStr(Data("logo")("url"))
                        Else
                            Me.LogoUrl = CStr(Data("logo")("thumbnailUrl"))
                        End If
                    End If
                    If Operators.CompareString(Me.LogoUrl, "", False) = 0 Then
                        Me.LogoUrl = Nothing
                    End If
                    If Me.Website.Contains("/mc-mods/") OrElse Me.Website.Contains("/mod/") Then
                        Me.Types = ResourceTypes.[Mod]
                    ElseIf Me.Website.Contains("/modpacks/") Then
                        Me.Types = ResourceTypes.ModPack
                    ElseIf Me.Website.Contains("/resourcepacks/") Then
                        Me.Types = ResourceTypes.ResourcePack
                    ElseIf Me.Website.Contains("/texture-packs/") Then
                        Me.Types = ResourceTypes.ResourcePack
                    ElseIf Me.Website.Contains("/shaders/") Then
                        Me.Types = ResourceTypes.Shader
                    Else
                        Me.Types = ResourceTypes.DataPack
                    End If
                    Me.ModLoaders = ModLoaders.None
                    Dim list As List(Of KeyValuePair(Of Integer, List(Of String))) = New List(Of KeyValuePair(Of Integer, List(Of String)))()
                    Try
                        For Each jtoken As JToken In (If(Data("latestFiles"), New JArray()))
                            Dim resourceVersion As ResourceVersion = ResourceVersion.FromPlatformJson(CType(jtoken, JObject), Me.Types)
                            If resourceVersion.DownloadAvailable Then
                                Me.ModLoaders = Me.ModLoaders Or resourceVersion.ModLoaders
                                Dim list2 As List(Of String) = jtoken("gameVersions").ToObject(Of List(Of String))()
                                Dim enumerable4 As IEnumerable(Of String) = list2
                                Dim func7 As Func(Of String, Boolean)
                                If ResourceProject._Closure$__.$I25-3 IsNot Nothing Then
                                    func7 = ResourceProject._Closure$__.$I25-3
                                Else
                                    Dim func8 As Func(Of String, Boolean) = Function(v As String) McVersion.IsStandardFormat(v)
                                    func7 = func8
                                    ResourceProject._Closure$__.$I25-3 = func8
                                End If
                                If enumerable4.Any(func7) Then
                                    list.Add(New KeyValuePair(Of Integer, List(Of String))(CInt(jtoken("id")), list2))
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of JToken)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    Try
                        For Each jtoken2 As JToken In (If(Data("latestFilesIndexes"), New JArray()))
                            If McVersion.IsStandardFormat(CStr(jtoken2("gameVersion"))) Then
                                list.Add(New KeyValuePair(Of Integer, List(Of String))(CInt(jtoken2("fileId")), New String() { jtoken2("gameVersion").ToString() }.ToList()))
                            End If
                        Next
                    Finally
                        Dim enumerator2 As IEnumerator(Of JToken)
                        If enumerator2 IsNot Nothing Then
                            enumerator2.Dispose()
                        End If
                    End Try
                    Dim enumerable5 As IEnumerable(Of KeyValuePair(Of Integer, List(Of String))) = list
                    Dim func9 As Func(Of KeyValuePair(Of Integer, List(Of String)), Integer)
                    If ResourceProject._Closure$__.$I25-4 IsNot Nothing Then
                        func9 = ResourceProject._Closure$__.$I25-4
                    Else
                        Dim func10 As Func(Of KeyValuePair(Of Integer, List(Of String)), Integer) = Function(f As KeyValuePair(Of Integer, List(Of String))) f.Key
                        func9 = func10
                        ResourceProject._Closure$__.$I25-4 = func10
                    End If
                    Me.CurseForgeFileIds = enumerable5.[Select](func9).Distinct().ToList()
                    Dim enumerable6 As IEnumerable(Of KeyValuePair(Of Integer, List(Of String))) = list
                    Dim func11 As Func(Of KeyValuePair(Of Integer, List(Of String)), IEnumerable(Of String))
                    If ResourceProject._Closure$__.$I25-5 IsNot Nothing Then
                        func11 = ResourceProject._Closure$__.$I25-5
                    Else
                        Dim func12 As Func(Of KeyValuePair(Of Integer, List(Of String)), IEnumerable(Of String)) = Function(f As KeyValuePair(Of Integer, List(Of String))) f.Value
                        func11 = func12
                        ResourceProject._Closure$__.$I25-5 = func12
                    End If
                    Dim enumerable7 As IEnumerable(Of String) = enumerable6.SelectMany(func11)
                    Dim func13 As Func(Of String, Integer)
                    If ResourceProject._Closure$__.$I25-6 IsNot Nothing Then
                        func13 = ResourceProject._Closure$__.$I25-6
                    Else
                        Dim func14 As Func(Of String, Integer) = Function(v As String) McVersion.VersionToDrop(v)
                        func13 = func14
                        ResourceProject._Closure$__.$I25-6 = func14
                    End If
                    Dim enumerable8 As IEnumerable(Of Integer) = enumerable7.[Select](func13)
                    Dim func15 As Func(Of Integer, Boolean)
                    If ResourceProject._Closure$__.$I25-7 IsNot Nothing Then
                        func15 = ResourceProject._Closure$__.$I25-7
                    Else
                        Dim func16 As Func(Of Integer, Boolean) = Function(v As Integer) v <> 209
                        func15 = func16
                        ResourceProject._Closure$__.$I25-7 = func16
                    End If
                    Dim enumerable9 As IEnumerable(Of Integer) = enumerable8.Where(func15).Distinct()
                    Dim func17 As Func(Of Integer, Integer)
                    If ResourceProject._Closure$__.$I25-8 IsNot Nothing Then
                        func17 = ResourceProject._Closure$__.$I25-8
                    Else
                        Dim func18 As Func(Of Integer, Integer) = Function(v As Integer) v
                        func17 = func18
                        ResourceProject._Closure$__.$I25-8 = func18
                    End If
                    Me.Drops = enumerable9.OrderByDescending(func17).ToList()
                    Me.Tags = New List(Of String)()
                    Try
                        Dim enumerable10 As IEnumerable(Of JToken) = If(Data("categories"), New JArray())
                        Dim func19 As Func(Of JToken, Integer)
                        If ResourceProject._Closure$__.$I25-9 IsNot Nothing Then
                            func19 = ResourceProject._Closure$__.$I25-9
                        Else
                            Dim func20 As Func(Of JToken, Integer) = Function(t As JToken) CInt(t("id"))
                            func19 = func20
                            ResourceProject._Closure$__.$I25-9 = func20
                        End If
                        Dim enumerable11 As IEnumerable(Of Integer) = enumerable10.[Select](func19).Distinct()
                        Dim func21 As Func(Of Integer, Integer)
                        If ResourceProject._Closure$__.$I25-10 IsNot Nothing Then
                            func21 = ResourceProject._Closure$__.$I25-10
                        Else
                            Dim func22 As Func(Of Integer, Integer) = Function(c As Integer) c
                            func21 = func22
                            ResourceProject._Closure$__.$I25-10 = func22
                        End If
                        For Each num As Integer In enumerable11.OrderByDescending(func21)
                            If num <= 5128 Then
                                If num <= 4558 Then
                                    Select Case num
                                        Case 393
                                            Me.Tags.Add("16x")
                                        Case 394
                                            Me.Tags.Add("32x")
                                        Case 395
                                            Me.Tags.Add("64x")
                                        Case 396
                                            Me.Tags.Add("128x")
                                        Case 397
                                            Me.Tags.Add("256x")
                                        Case 398
                                            Me.Tags.Add("超高清")
                                        Case 399
                                            Me.Tags.Add("蒸汽朋克")
                                        Case 400
                                            Me.Tags.Add("写实风")
                                        Case 401
                                            Me.Tags.Add("现代风")
                                        Case 402
                                            Me.Tags.Add("中世纪")
                                        Case 403
                                            Me.Tags.Add("原版风")
                                        Case 404
                                            Me.Tags.Add("动态效果")
                                        Case 405, 413, 418, 425, 426, 427, 428, 429, 430, 431, 432, 433
                                        Case 406
                                            Me.Tags.Add("世界元素")
                                        Case 407
                                            Me.Tags.Add("生物群系")
                                        Case 408
                                            Me.Tags.Add("矿物/资源")
                                        Case 409
                                            Me.Tags.Add("天然结构")
                                        Case 410
                                            Me.Tags.Add("维度")
                                        Case 411
                                            Me.Tags.Add("生物")
                                        Case 412
                                            Me.Tags.Add("科技")
                                        Case 414
                                            Me.Tags.Add("运输")
                                        Case 415
                                            Me.Tags.Add("管道/物流")
                                        Case 416
                                            Me.Tags.Add("农业")
                                        Case 417
                                            Me.Tags.Add("能源")
                                        Case 419
                                            Me.Tags.Add("魔法")
                                        Case 420
                                            Me.Tags.Add("仓储")
                                        Case 421
                                            Me.Tags.Add("支持库")
                                        Case 422
                                            Me.Tags.Add("冒险")
                                        Case 423
                                            Me.Tags.Add("信息显示")
                                        Case 424
                                            Me.Tags.Add("装饰")
                                        Case 434
                                            Me.Tags.Add("装备")
                                        Case 435
                                            Me.Tags.Add("服务器")
                                        Case 436
                                            Me.Tags.Add("食物/烹饪")
                                        Case Else
                                            Select Case num
                                                Case 4465
                                                    Me.Tags.Add("兼容 Mod")
                                                Case 4466, 4467, 4468, 4469, 4470, 4471, 4485, 4486
                                                Case 4472
                                                    Me.Tags.Add("科技")
                                                Case 4473
                                                    Me.Tags.Add("魔法")
                                                Case 4474
                                                    Me.Tags.Add("科幻")
                                                Case 4475
                                                    Me.Tags.Add("冒险")
                                                Case 4476
                                                    Me.Tags.Add("探索")
                                                Case 4477
                                                    Me.Tags.Add("小游戏")
                                                Case 4478
                                                    Me.Tags.Add("任务")
                                                Case 4479
                                                    Me.Tags.Add("硬核")
                                                Case 4480
                                                    Me.Tags.Add("基于地图")
                                                Case 4481
                                                    Me.Tags.Add("轻量")
                                                Case 4482
                                                    Me.Tags.Add("大型")
                                                Case 4483
                                                    Me.Tags.Add("战斗")
                                                Case 4484
                                                    Me.Tags.Add("多人")
                                                Case 4487
                                                    Me.Tags.Add("FTB")
                                                Case Else
                                                    If num = 4558 Then
                                                        Me.Tags.Add("红石")
                                                    End If
                                            End Select
                                    End Select
                                ElseIf num <> 4736 Then
                                    If num <> 4843 Then
                                        If num = 5128 Then
                                            Me.Tags.Add("原版改良")
                                        End If
                                    Else
                                        Me.Tags.Add("自动化")
                                    End If
                                Else
                                    Me.Tags.Add("空岛")
                                End If
                            ElseIf num <= 5244 Then
                                If num <> 5191 Then
                                    If num <> 5193 Then
                                        If num = 5244 Then
                                            Me.Tags.Add("含字体")
                                        End If
                                    Else
                                        Me.Tags.Add("数据包")
                                    End If
                                Else
                                    Me.Tags.Add("改良")
                                End If
                            ElseIf num <= 6814 Then
                                Select Case num
                                    Case 6553
                                        Me.Tags.Add("写实风")
                                    Case 6554
                                        Me.Tags.Add("幻想风")
                                    Case 6555
                                        Me.Tags.Add("原版风")
                                    Case Else
                                        If num = 6814 Then
                                            Me.Tags.Add("性能优化")
                                        End If
                                End Select
                            Else
                                Select Case num
                                    Case 6946
                                        Me.Tags.Add("Mod 相关")
                                    Case 6947
                                    Case 6948
                                        Me.Tags.Add("冒险")
                                    Case 6949
                                        Me.Tags.Add("幻想")
                                    Case 6950
                                        Me.Tags.Add("支持库")
                                    Case 6951
                                        Me.Tags.Add("科技")
                                    Case 6952
                                        Me.Tags.Add("魔法")
                                    Case 6953
                                        Me.Tags.Add("实用")
                                    Case Else
                                        If num = 9026 Then
                                            Me.Tags.Add("创造模式")
                                        End If
                                End Select
                            End If
                        Next
                        GoTo IL_1ED9
                    Finally
                        Dim enumerator3 As IEnumerator(Of Integer)
                        If enumerator3 IsNot Nothing Then
                            enumerator3.Dispose()
                        End If
                    End Try
                End If
                Me.Id = CStr((If(Data("project_id"), Data("id"))))
                Me.Slug = CStr(Data("slug"))
                Me.RawName = CStr(Data("title"))
                Me.Description = CStr(Data("description"))
                Me.LastUpdate = CType((If(Data("date_modified"), Data("updated"))), DateTime)
                Me.DownloadCount = CInt(Data("downloads"))
                Me.LogoUrl = CStr(Data("icon_url"))
                If Operators.CompareString(Me.LogoUrl, "", False) = 0 Then
                    Me.LogoUrl = Nothing
                End If
                Me.Website = String.Format("https://modrinth.com/{0}/{1}", Data("project_type"), Me.Slug)
                Dim enumerable12 As IEnumerable(Of JToken) = If(CType((If(Data("game_versions"), Data("versions"))), JArray), New JArray())
                Dim func23 As Func(Of JToken, String)
                If ResourceProject._Closure$__.$I25-11 IsNot Nothing Then
                    func23 = ResourceProject._Closure$__.$I25-11
                Else
                    Dim func24 As Func(Of JToken, String) = Function(v As JToken) v.ToString()
                    func23 = func24
                    ResourceProject._Closure$__.$I25-11 = func24
                End If
                Me.UnsafeGameVersions = enumerable12.[Select](func23).Distinct().ToList()
                Dim unsafeGameVersions As IEnumerable(Of String) = Me.UnsafeGameVersions
                Dim func25 As Func(Of String, Integer)
                If ResourceProject._Closure$__.$I25-12 IsNot Nothing Then
                    func25 = ResourceProject._Closure$__.$I25-12
                Else
                    Dim func26 As Func(Of String, Integer) = Function(v As String) McVersion.VersionToDrop(v)
                    func25 = func26
                    ResourceProject._Closure$__.$I25-12 = func26
                End If
                Dim enumerable13 As IEnumerable(Of Integer) = unsafeGameVersions.[Select](func25)
                Dim func27 As Func(Of Integer, Boolean)
                If ResourceProject._Closure$__.$I25-13 IsNot Nothing Then
                    func27 = ResourceProject._Closure$__.$I25-13
                Else
                    Dim func28 As Func(Of Integer, Boolean) = Function(v As Integer) v <> 209
                    func27 = func28
                    ResourceProject._Closure$__.$I25-13 = func28
                End If
                Dim enumerable14 As IEnumerable(Of Integer) = enumerable13.Where(func27).Distinct()
                Dim func29 As Func(Of Integer, Integer)
                If ResourceProject._Closure$__.$I25-14 IsNot Nothing Then
                    func29 = ResourceProject._Closure$__.$I25-14
                Else
                    Dim func30 As Func(Of Integer, Integer) = Function(v As Integer) v
                    func29 = func30
                    ResourceProject._Closure$__.$I25-14 = func30
                End If
                Me.Drops = enumerable14.OrderByDescending(func29).ToList()
                Dim text As String = Data("project_type").ToString()
                If Operators.CompareString(text, "modpack", False) <> 0 Then
                    If Operators.CompareString(text, "resourcepack", False) <> 0 Then
                        If Operators.CompareString(text, "shader", False) <> 0 Then
                            Me.Types = ResourceTypes.[Mod]
                        Else
                            Me.Types = ResourceTypes.Shader
                        End If
                    Else
                        Me.Types = ResourceTypes.ResourcePack
                    End If
                Else
                    Me.Types = ResourceTypes.ModPack
                End If
                Me.Tags = New List(Of String)()
                Me.ModLoaders = ModLoaders.None
                Try
                    Dim jtoken3 As JToken = Data("loaders")
                    Dim enumerable15 As IEnumerable(Of String)
                    If jtoken3 Is Nothing Then
                        enumerable15 = Nothing
                    Else
                        Dim func31 As Func(Of JToken, String)
                        If ResourceProject._Closure$__.$I25-15 IsNot Nothing Then
                            func31 = ResourceProject._Closure$__.$I25-15
                        Else
                            Dim func32 As Func(Of JToken, String) = Function(t As JToken) t.ToString()
                            func31 = func32
                            ResourceProject._Closure$__.$I25-15 = func32
                        End If
                        enumerable15 = jtoken3.[Select](func31)
                    End If
                    Dim enumerable16 As IEnumerable(Of String) = If(enumerable15, New String(-1) {})
                    Dim enumerable17 As IEnumerable(Of JToken) = Data("categories")
                    Dim func33 As Func(Of JToken, String)
                    If ResourceProject._Closure$__.$I25-16 IsNot Nothing Then
                        func33 = ResourceProject._Closure$__.$I25-16
                    Else
                        Dim func34 As Func(Of JToken, String) = Function(t As JToken) t.ToString()
                        func33 = func34
                        ResourceProject._Closure$__.$I25-16 = func34
                    End If
                    For Each text2 As String In enumerable16.Concat(enumerable17.[Select](func33))
                        Try
                            For Each modLoaders As ModLoaders In EnumUtils.GetAllFlags(Of ModLoaders)()
                                If Operators.CompareString(text2, modLoaders.ToString().Lower(), False) = 0 Then
                                    Me.ModLoaders = Me.ModLoaders Or modLoaders
                                    Exit For
                                End If
                            Next
                        Finally
                            Dim enumerator5 As List(Of ModLoaders).Enumerator
                            CType(enumerator5, IDisposable).Dispose()
                        End Try
                    Next
                Finally
                    Dim enumerator4 As IEnumerator(Of String)
                    If enumerator4 IsNot Nothing Then
                        enumerator4.Dispose()
                    End If
                End Try
                Try
                    Dim enumerable18 As IEnumerable(Of JToken) = Data("categories")
                    Dim func35 As Func(Of JToken, String)
                    If ResourceProject._Closure$__.$I25-17 IsNot Nothing Then
                        func35 = ResourceProject._Closure$__.$I25-17
                    Else
                        Dim func36 As Func(Of JToken, String) = Function(t As JToken) t.ToString()
                        func35 = func36
                        ResourceProject._Closure$__.$I25-17 = func36
                    End If
                    For Each text3 As String In enumerable18.[Select](func35)
                        Dim num2 As UInteger = <PrivateImplementationDetails>.ComputeStringHash(text3)
                        If num2 <= 1678477998UI Then
                            If num2 <= 889035115UI Then
                                If num2 <= 564424733UI Then
                                    If num2 <= 126402435UI Then
                                        If num2 <> 21694712UI Then
                                            If num2 <> 30055014UI Then
                                                If num2 = 126402435UI Then
                                                    If Operators.CompareString(text3, "decoration", False) = 0 Then
                                                        If Me.Types <> ResourceTypes.ResourcePack Then
                                                            Me.Tags.Add("装饰")
                                                        End If
                                                    End If
                                                End If
                                            ElseIf Operators.CompareString(text3, "storage", False) = 0 Then
                                                Me.Tags.Add("仓储")
                                            End If
                                        ElseIf Operators.CompareString(text3, "32x", False) = 0 Then
                                            Me.Tags.Add("32x")
                                        End If
                                    ElseIf num2 <> 343003787UI Then
                                        If num2 <> 484666913UI Then
                                            If num2 = 564424733UI Then
                                                If Operators.CompareString(text3, "game-mechanics", False) = 0 Then
                                                    Me.Tags.Add("游戏机制")
                                                End If
                                            End If
                                        ElseIf Operators.CompareString(text3, "combat", False) = 0 Then
                                            Me.Tags.Add("战斗")
                                        End If
                                    ElseIf Operators.CompareString(text3, "models", False) = 0 Then
                                        Me.Tags.Add("含模型")
                                    End If
                                ElseIf num2 <= 782861478UI Then
                                    If num2 <> 679943061UI Then
                                        If num2 <> 765533517UI Then
                                            If num2 = 782861478UI Then
                                                If Operators.CompareString(text3, "iris", False) = 0 Then
                                                    Me.Tags.Add("Iris")
                                                End If
                                            End If
                                        ElseIf Operators.CompareString(text3, "core-shaders", False) = 0 Then
                                            Me.Tags.Add("核心着色器")
                                        End If
                                    ElseIf Operators.CompareString(text3, "path-tracing", False) = 0 Then
                                        Me.Tags.Add("路径追踪")
                                    End If
                                ElseIf num2 <> 858719634UI Then
                                    If num2 <> 859535076UI Then
                                        If num2 = 889035115UI Then
                                            If Operators.CompareString(text3, "lightweight", False) = 0 Then
                                                Me.Tags.Add("轻量")
                                            End If
                                        End If
                                    ElseIf Operators.CompareString(text3, "tweaks", False) = 0 Then
                                        Me.Tags.Add("改良")
                                    End If
                                ElseIf Operators.CompareString(text3, "16x", False) = 0 Then
                                    Me.Tags.Add("16x")
                                End If
                            ElseIf num2 <= 1446847267UI Then
                                If num2 <= 1028682697UI Then
                                    If num2 <> 926227488UI Then
                                        If num2 <> 934491434UI Then
                                            If num2 = 1028682697UI Then
                                                If Operators.CompareString(text3, "food", False) = 0 Then
                                                    Me.Tags.Add("食物/烹饪")
                                                End If
                                            End If
                                        ElseIf Operators.CompareString(text3, "128x", False) = 0 Then
                                            Me.Tags.Add("128x")
                                        End If
                                    ElseIf Operators.CompareString(text3, "gui", False) = 0 Then
                                        Me.Tags.Add("含 UI")
                                    End If
                                ElseIf num2 <> 1360377439UI Then
                                    If num2 <> 1418100143UI Then
                                        If num2 = 1446847267UI Then
                                            If Operators.CompareString(text3, "transportation", False) = 0 Then
                                                Me.Tags.Add("运输")
                                            End If
                                        End If
                                    ElseIf Operators.CompareString(text3, "technology", False) = 0 Then
                                        Me.Tags.Add("科技")
                                    End If
                                ElseIf Operators.CompareString(text3, "realistic", False) = 0 Then
                                    Me.Tags.Add("写实风")
                                End If
                            ElseIf num2 <= 1564096620UI Then
                                If num2 <> 1464635778UI Then
                                    If num2 <> 1546653040UI Then
                                        If num2 = 1564096620UI Then
                                            If Operators.CompareString(text3, "simplistic", False) = 0 Then
                                                Me.Tags.Add("简洁")
                                            End If
                                        End If
                                    ElseIf Operators.CompareString(text3, "vanilla", False) = 0 Then
                                        Me.Tags.Add("原版可用")
                                    End If
                                ElseIf Operators.CompareString(text3, "semi-realistic", False) = 0 Then
                                    Me.Tags.Add("半写实风")
                                End If
                            ElseIf num2 <= 1636821932UI Then
                                If num2 <> 1565668191UI Then
                                    If num2 = 1636821932UI Then
                                        If Operators.CompareString(text3, "modded", False) = 0 Then
                                            Me.Tags.Add("兼容 Mod")
                                        End If
                                    End If
                                ElseIf Operators.CompareString(text3, "pbr", False) = 0 Then
                                    Me.Tags.Add("PBR")
                                End If
                            ElseIf num2 <> 1657676564UI Then
                                If num2 = 1678477998UI Then
                                    If Operators.CompareString(text3, "256x", False) = 0 Then
                                        Me.Tags.Add("256x")
                                    End If
                                End If
                            ElseIf Operators.CompareString(text3, "vanilla-like", False) = 0 Then
                                Me.Tags.Add("原版风")
                            End If
                        ElseIf num2 <= 3217471839UI Then
                            If num2 <= 2432105424UI Then
                                If num2 <= 1955283044UI Then
                                    If num2 <> 1796984983UI Then
                                        If num2 <> 1855291081UI Then
                                            If num2 = 1955283044UI Then
                                                If Operators.CompareString(text3, "optimization", False) = 0 Then
                                                    Me.Tags.Add("性能优化")
                                                End If
                                            End If
                                        ElseIf Operators.CompareString(text3, "cartoon", False) = 0 Then
                                            Me.Tags.Add("卡通风")
                                        End If
                                    ElseIf Operators.CompareString(text3, "locale", False) = 0 Then
                                        Me.Tags.Add("含语言")
                                    End If
                                ElseIf num2 <> 2117003656UI Then
                                    If num2 <> 2216639593UI Then
                                        If num2 = 2432105424UI Then
                                            If Operators.CompareString(text3, "library", False) = 0 Then
                                                Me.Tags.Add("支持库")
                                            End If
                                        End If
                                    ElseIf Operators.CompareString(text3, "optifine", False) = 0 Then
                                        Me.Tags.Add("OptiFine")
                                    End If
                                ElseIf Operators.CompareString(text3, "colored-lighting", False) = 0 Then
                                    Me.Tags.Add("彩色光照")
                                End If
                            ElseIf num2 <= 2930129973UI Then
                                If num2 <> 2745962869UI Then
                                    If num2 <> 2898550409UI Then
                                        If num2 = 2930129973UI Then
                                            If Operators.CompareString(text3, "fantasy", False) = 0 Then
                                                Me.Tags.Add("幻想风")
                                            End If
                                        End If
                                    ElseIf Operators.CompareString(text3, "challenging", False) = 0 Then
                                        Me.Tags.Add("硬核")
                                    End If
                                ElseIf Operators.CompareString(text3, "reflections", False) = 0 Then
                                    Me.Tags.Add("反射")
                                End If
                            ElseIf num2 <= 3001865938UI Then
                                If num2 <> 2954106368UI Then
                                    If num2 = 3001865938UI Then
                                        If Operators.CompareString(text3, "social", False) = 0 Then
                                            Me.Tags.Add("服务器")
                                        End If
                                    End If
                                ElseIf Operators.CompareString(text3, "8x-", False) = 0 Then
                                    Me.Tags.Add("极简")
                                End If
                            ElseIf num2 <> 3037049615UI Then
                                If num2 = 3217471839UI Then
                                    If Operators.CompareString(text3, "utility", False) = 0 Then
                                        Me.Tags.Add("实用")
                                    End If
                                End If
                            ElseIf Operators.CompareString(text3, "worldgen", False) = 0 Then
                                Me.Tags.Add("世界元素")
                            End If
                        ElseIf num2 <= 3366373741UI Then
                            If num2 <= 3294764480UI Then
                                If num2 <> 3249952714UI Then
                                    If num2 <> 3269966681UI Then
                                        If num2 = 3294764480UI Then
                                            If Operators.CompareString(text3, "datapack", False) = 0 Then
                                                Me.Types = ResourceTypes.DataPack
                                            End If
                                        End If
                                    ElseIf Operators.CompareString(text3, "fonts", False) = 0 Then
                                        Me.Tags.Add("含字体")
                                    End If
                                ElseIf Operators.CompareString(text3, "quests", False) = 0 Then
                                    Me.Tags.Add("任务")
                                End If
                            ElseIf num2 <> 3302598349UI Then
                                If num2 <> 3306152395UI Then
                                    If num2 = 3366373741UI Then
                                        If Operators.CompareString(text3, "64x", False) = 0 Then
                                            Me.Tags.Add("64x")
                                        End If
                                    End If
                                ElseIf Operators.CompareString(text3, "adventure", False) = 0 Then
                                    Me.Tags.Add("冒险")
                                End If
                            ElseIf Operators.CompareString(text3, "kitchen-sink", False) = 0 Then
                                Me.Tags.Add("水槽包")
                            End If
                        ElseIf num2 <= 3796101307UI Then
                            If num2 <> 3463588087UI Then
                                If num2 <> 3764468121UI Then
                                    If num2 = 3796101307UI Then
                                        If Operators.CompareString(text3, "48x", False) = 0 Then
                                            Me.Tags.Add("48x")
                                        End If
                                    End If
                                ElseIf Operators.CompareString(text3, "audio", False) = 0 Then
                                    Me.Tags.Add("含声音")
                                End If
                            ElseIf Operators.CompareString(text3, "equipment", False) = 0 Then
                                If Me.Types <> ResourceTypes.ResourcePack Then
                                    Me.Tags.Add("装备")
                                End If
                            End If
                        ElseIf num2 <= 4053708971UI Then
                            If num2 <> 3840615820UI Then
                                If num2 = 4053708971UI Then
                                    If Operators.CompareString(text3, "multiplayer", False) = 0 Then
                                        Me.Tags.Add("多人")
                                    End If
                                End If
                            ElseIf Operators.CompareString(text3, "magic", False) = 0 Then
                                Me.Tags.Add("魔法")
                            End If
                        ElseIf num2 <> 4091869486UI Then
                            If num2 = 4269569010UI Then
                                If Operators.CompareString(text3, "mobs", False) = 0 Then
                                    If Me.Types <> ResourceTypes.ResourcePack Then
                                        Me.Tags.Add("生物")
                                    End If
                                End If
                            End If
                        ElseIf Operators.CompareString(text3, "512x+", False) = 0 Then
                            Me.Tags.Add("超高清")
                        End If
                    Next
                Finally
                    Dim enumerator6 As IEnumerator(Of String)
                    If enumerator6 IsNot Nothing Then
                        enumerator6.Dispose()
                    End If
                End Try
                If Me.Types = ResourceTypes.DataPack AndAlso Me.ModLoaders <> ModLoaders.None Then
                    Me.Types = ResourceTypes.ModOrDataPack
                End If
                IL_1ED9:
                If Not Me.Tags.Any() Then
                    Me.Tags.Add("其他")
                End If
                Me.Tags.Sort()
            End If
            ResourceProject.Cache(Me.Id) = Me
        End Sub

        Public Shared Function FromProjectId(ProjectId As String, Platform As ResourcePlatforms) As ResourceProject
            If Not Platform.Flags().IsSingle() Then
                Throw New ArgumentException(String.Format("必须指定单一平台，当前值为 {0}", Platform))
            End If
            Dim resourceProject As ResourceProject
            If ResourceProject.Cache.ContainsKey(ProjectId) Then
                resourceProject = ResourceProject.Cache(ProjectId)
            ElseIf Platform = ResourcePlatforms.CurseForge Then
                resourceProject = New ResourceProject(CType(ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/" + ProjectId, Nothing, Nothing, Nothing)("data"), JObject))
            Else
                resourceProject = New ResourceProject(CType(ModDownload.DlModRequest("https://api.modrinth.com/v2/project/" + ProjectId, Nothing, Nothing, Nothing), JObject))
            End If
            Return resourceProject
        End Function

        Public Shared Iterator Function FromProjectIds(ProjectIds As IEnumerable(Of String), Platform As ResourcePlatforms) As IEnumerable(Of ResourceProject)
            If Not Platform.Flags().IsSingle() Then
                Throw New ArgumentException(String.Format("必须指定单一平台，当前值为 {0}", Platform))
            End If
            Dim MissingProjectIds As List(Of String) = New List(Of String)()
            Try
                For Each text As String In ProjectIds
                    If ResourceProject.Cache.ContainsKey(text) Then
                        Yield ResourceProject.Cache(text)
                    Else
                        MissingProjectIds.Add(text)
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of String)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            If MissingProjectIds.Any() Then
                Dim jarray As JArray
                If Platform.HasFlagF(ResourcePlatforms.CurseForge) Then
                    jarray = CType(ModDownload.DlModRequest("https://api.curseforge.com/v1/mods", HttpMethod.Post, "{""modIds"": [" + MissingProjectIds.Join(","c) + "]}", "application/json")("data"), JArray)
                Else
                    jarray = CType(ModDownload.DlModRequest(String.Format("https://api.modrinth.com/v2/projects?ids=[""{0}""]", MissingProjectIds.Join(""",""")), Nothing, Nothing, Nothing), JArray)
                End If
                Try
                    For Each jtoken As JToken In jarray
                        Yield New ResourceProject(CType(jtoken, JObject))
                    Next
                    Return
                Finally
                    Dim enumerator2 As IEnumerator(Of JToken)
                    If enumerator2 IsNot Nothing Then
                        enumerator2.Dispose()
                    End If
                End Try
            End If
            Return
        End Function

        Public Function ToJson() As JObject
            Dim jobject As JObject = New JObject()
            jobject("Platform") = CInt(Me.Platform)
            jobject("Types") = CInt(Me.Types)
            jobject("Slug") = Me.Slug
            jobject("Id") = Me.Id
            If Me.CurseForgeFileIds IsNot Nothing Then
                jobject("CurseForgeFileIds") = New JArray(Me.CurseForgeFileIds)
            End If
            jobject("RawName") = Me.RawName
            jobject("Description") = Me.Description
            jobject("Website") = Me.Website
            jobject("LastUpdate") = Me.LastUpdate
            jobject("DownloadCount") = Me.DownloadCount
            If Me.ModLoaders <> ModLoaders.None Then
                jobject("ModLoaders") = CInt(Me.ModLoaders)
            End If
            jobject("Tags") = New JArray(Me.Tags)
            If Me.LogoUrl IsNot Nothing Then
                jobject("LogoUrl") = Me.LogoUrl
            End If
            If Me.Drops.Any() Then
                jobject("Drops") = New JArray(Me.Drops)
            End If
            jobject("CacheTime") = DateTime.Now
            Return jobject
        End Function

        Public Function ToResourceItem(ShowMcVersionDesc As Boolean, ShowLoaderDesc As Boolean) As OurVirtualizingElement(Of OurResourceItem)
            Dim CS$<>8__locals1 As ResourceProject._Closure$__29-0 = New ResourceProject._Closure$__29-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Me = Me
            CS$<>8__locals1.$VB$Local_ShowMcVersionDesc = ShowMcVersionDesc
            CS$<>8__locals1.$VB$Local_ShowLoaderDesc = ShowLoaderDesc
            If Me.Drops Is Nothing OrElse Not Me.Drops.Any() Then
                CS$<>8__locals1.$VB$Local_GameVersionDescription = "仅快照版本"
            Else
                Dim list As List(Of String) = New List(Of String)()
                Dim flag As Boolean = False
                Dim num As Integer = Me.Drops.Count - 1
                For i As Integer = 0 To num
                    Dim num2 As Integer = Me.Drops(i)
                    Dim num3 As Integer = Me.Drops(i)
                    If num2 < 120 Then
                        If list.Any() AndAlso Not flag Then
                            Exit For
                        End If
                        flag = True
                    End If
                    Dim num4 As Integer = i + 1
                    Dim num5 As Integer = Me.Drops.Count - 1
                    Dim num6 As Integer = num4
                    While num6 <= num5 AndAlso ModDownload.AllDrops IsNot Nothing AndAlso ModDownload.AllDrops.IndexOf(Me.Drops(num6)) = ModDownload.AllDrops.IndexOf(num3) + 1
                        num3 = Me.Drops(num6)
                        i = num6
                        num6 += 1
                    End While
                    Dim text As String = McVersion.DropToVersion(num2)
                    Dim text2 As String = McVersion.DropToVersion(num3)
                    If num2 = num3 Then
                        list.Add(text)
                    ElseIf ModDownload.AllDrops IsNot Nothing AndAlso num2 >= ModDownload.AllDrops.First() Then
                        If num3 < 120 Then
                            list.Clear()
                            list.Add("全版本")
                            Exit For
                        End If
                        list.Add(text2 + "+")
                    Else
                        If num3 < 120 Then
                            list.Add(text + "-")
                            Exit For
                        End If
                        If ModDownload.AllDrops Is Nothing OrElse ModDownload.AllDrops.IndexOf(num3) - ModDownload.AllDrops.IndexOf(num2) = 1 Then
                            list.Add(text + ", " + text2)
                        Else
                            list.Add(text + "~" + text2)
                        End If
                    End If
                Next
                CS$<>8__locals1.$VB$Local_GameVersionDescription = list.Join(", ")
            End If
            Dim list2 As List(Of ModLoaders) = Me.ModLoaders.Flags().ToList()
            Dim count As Integer = list2.Count
            If count <> 0 Then
                If count <> 1 Then
                    Dim num7 As Integer = If(Me.Drops.Any(), Me.Drops.First(), 9999)
                    If list2.Contains(ModLoaders.Forge) AndAlso (num7 < 140 OrElse list2.Contains(ModLoaders.Fabric)) AndAlso (num7 < 200 OrElse list2.Contains(ModLoaders.NeoForge)) Then
                        CS$<>8__locals1.$VB$Local_ModLoaderDescriptionFull = "任意"
                        CS$<>8__locals1.$VB$Local_ModLoaderDescriptionPart = ""
                    Else
                        CS$<>8__locals1.$VB$Local_ModLoaderDescriptionFull = list2.Join(" / ")
                        CS$<>8__locals1.$VB$Local_ModLoaderDescriptionPart = list2.Join(" / ")
                    End If
                Else
                    CS$<>8__locals1.$VB$Local_ModLoaderDescriptionFull = "仅 " + list2.[Single]().ToString()
                    CS$<>8__locals1.$VB$Local_ModLoaderDescriptionPart = list2.[Single]().ToString()
                End If
            ElseIf list2.IsSingle() Then
                CS$<>8__locals1.$VB$Local_ModLoaderDescriptionFull = "仅 " + list2.[Single]().ToString()
                CS$<>8__locals1.$VB$Local_ModLoaderDescriptionPart = list2.[Single]().ToString()
            Else
                CS$<>8__locals1.$VB$Local_ModLoaderDescriptionFull = "未知"
                CS$<>8__locals1.$VB$Local_ModLoaderDescriptionPart = ""
            End If
            Return New OurVirtualizingElement(Of OurResourceItem)(Function()
                Dim myResourceItem As OurResourceItem = New OurResourceItem() With { .Tag = CS$<>8__locals1.$VB$Me }
                CS$<>8__locals1.$VB$Me.ApplyLogoToOurImage(myResourceItem.PathLogo)
                Dim controlTitle As ValueTuple(Of String, String) = CS$<>8__locals1.$VB$Me.GetControlTitle(True)
                myResourceItem.Title = controlTitle.Item1
                If Operators.CompareString(controlTitle.Item2, "", False) = 0 Then
                    CType(myResourceItem.LabTitleRaw.Parent, StackPanel).Children.Remove(myResourceItem.LabTitleRaw)
                Else
                    myResourceItem.SubTitle = controlTitle.Item2
                End If
                myResourceItem.Tags = CS$<>8__locals1.$VB$Me.Tags
                myResourceItem.Description = CS$<>8__locals1.$VB$Me.Description.ReplaceLineEndings("", False)
                If Not CS$<>8__locals1.$VB$Local_ShowMcVersionDesc AndAlso Not CS$<>8__locals1.$VB$Local_ShowLoaderDesc Then
                    CType(myResourceItem.PathVersion.Parent, Grid).Children.Remove(myResourceItem.PathVersion)
                    CType(myResourceItem.LabVersion.Parent, Grid).Children.Remove(myResourceItem.LabVersion)
                    myResourceItem.ColumnVersion1.Width = New GridLength(0.0)
                    myResourceItem.ColumnVersion2.MaxWidth = 0.0
                    myResourceItem.ColumnVersion3.Width = New GridLength(0.0)
                ElseIf CS$<>8__locals1.$VB$Local_ShowMcVersionDesc AndAlso CS$<>8__locals1.$VB$Local_ShowLoaderDesc Then
                    myResourceItem.LabVersion.Text = If((Operators.CompareString(CS$<>8__locals1.$VB$Local_ModLoaderDescriptionPart, "", False) = 0), "", (CS$<>8__locals1.$VB$Local_ModLoaderDescriptionPart + " ")) + CS$<>8__locals1.$VB$Local_GameVersionDescription
                ElseIf CS$<>8__locals1.$VB$Local_ShowMcVersionDesc Then
                    myResourceItem.LabVersion.Text = CS$<>8__locals1.$VB$Local_GameVersionDescription
                Else
                    myResourceItem.LabVersion.Text = CS$<>8__locals1.$VB$Local_ModLoaderDescriptionFull
                End If
                myResourceItem.LabSource.Text = CS$<>8__locals1.$VB$Me.Platform.ToString()
                myResourceItem.LabTime.Text = StringUtils.FormatTimeSpan(CS$<>8__locals1.$VB$Me.LastUpdate - DateTime.Now, True)
                myResourceItem.LabDownload.Text = Conversions.ToString(If((CS$<>8__locals1.$VB$Me.DownloadCount > 100000000), (Conversions.ToString(Math.Round(CDbl(CS$<>8__locals1.$VB$Me.DownloadCount) / 100000000.0, 2)) + " 亿"), If((CS$<>8__locals1.$VB$Me.DownloadCount > 100000), (Conversions.ToString(Math.Floor(CDbl(CS$<>8__locals1.$VB$Me.DownloadCount) / 10000.0)) + " 万"), CS$<>8__locals1.$VB$Me.DownloadCount)))
                Return myResourceItem
            End Function) With { .Height = 64.0 }
        End Function

        Public Sub ApplyLogoToOurImage(Img As OurImage)
            If String.IsNullOrEmpty(Me.LogoUrl) Then
                Img.Source = ModBase.PathImage + "Icons/NoIcon.png"
                Return
            End If
            Img.Source = Me.LogoUrl
            Img.FallbackSource = ModDownload.DlSourceModGet(Me.LogoUrl)
        End Sub

        Public Function GetControlTitle(HasModLoaderDescription As Boolean) As <TupleElementNames(New String() { "Title", "SubTitle" })> ValueTuple(Of String, String)
            Dim text As String = Me.RawName
            Dim list As List(Of String)
            If Operators.CompareString(Me.TranslatedName, Me.RawName, False) = 0 Then
                Dim enumerable As IEnumerable(Of String) = Me.TranslatedName.Split(New String() { " | ", " - ", "(", ")", "[", "]", "{", "}" }, True)
                Dim func As Func(Of String, String)
                If ResourceProject._Closure$__.$I31-0 IsNot Nothing Then
                    func = ResourceProject._Closure$__.$I31-0
                Else
                    Dim func2 As Func(Of String, String) = Function(s As String) s.Trim(" /\".ToCharArray())
                    func = func2
                    ResourceProject._Closure$__.$I31-0 = func2
                End If
                Dim enumerable2 As IEnumerable(Of String) = enumerable.[Select](func)
                Dim func3 As Func(Of String, Boolean)
                If ResourceProject._Closure$__.$I31-1 IsNot Nothing Then
                    func3 = ResourceProject._Closure$__.$I31-1
                Else
                    Dim func4 As Func(Of String, Boolean) = Function(w As String) Not String.IsNullOrEmpty(w)
                    func3 = func4
                    ResourceProject._Closure$__.$I31-1 = func4
                End If
                Dim enumerable3 As IEnumerable(Of String) = enumerable2.Where(func3)
                If enumerable3.IsSingle() Then
                    GoTo IL_05EC
                End If
                list = New List(Of String)()
                Dim list2 As List(Of String) = New List(Of String)()
                Try
                    For Each text2 As String In enumerable3
                        Dim CS$<>8__locals1 As ResourceProject._Closure$__31-0 = New ResourceProject._Closure$__31-0(CS$<>8__locals1)
                        CS$<>8__locals1.$VB$Local_LowerName = text2.Lower()
                        If Operators.CompareString(text2, text2.Upper(), False) = 0 AndAlso Operators.CompareString(text2, "FPS", False) <> 0 AndAlso Operators.CompareString(text2, "HUD", False) <> 0 Then
                            list.Add(text2)
                        ElseIf New String() { "neoforge", "forge", "fabric", "quilt" }.Any(Function(l As String) CS$<>8__locals1.$VB$Local_LowerName.Contains(l)) AndAlso Not CS$<>8__locals1.$VB$Local_LowerName.Replace("neoforge", "").Replace("forge", "").Replace("fabric", "").Replace("quilt", "").RegexCheck("[a-z]+", RegexOptions.None) Then
                            list.Add(text2)
                        Else
                            list2.Add(text2)
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of String)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If Not list2.Any() OrElse Not list.Any() Then
                    GoTo IL_05EC
                End If
                text = list2.Join(" - ")
            Else
                text = If(Me.TranslatedName.Contains(" ("), Me.TranslatedName.BeforeFirst(" (", False), Me.TranslatedName.BeforeLast(" - ", False))
                Dim text3 As String = ""
                If Me.TranslatedName.AfterLast(")", False).Contains(" - ") Then
                    text3 = Me.TranslatedName.AfterLast(")", False).AfterLast(" - ", False)
                End If
                Dim text4 As String = Me.TranslatedName
                If Operators.CompareString(text3, "", False) <> 0 Then
                    text4 = text4.Replace(" - " + text3, "")
                End If
                text4 = text4.Replace(text, "").Trim(New Char() { "("c, ")"c, " "c })
                Dim enumerable4 As IEnumerable(Of String) = text4.Split(New String() { " | ", " - ", "(", ")", "[", "]", "{", "}" }, True)
                Dim func5 As Func(Of String, String)
                If ResourceProject._Closure$__.$I31-3 IsNot Nothing Then
                    func5 = ResourceProject._Closure$__.$I31-3
                Else
                    Dim func6 As Func(Of String, String) = Function(s As String) s.Trim(" /".ToCharArray())
                    func5 = func6
                    ResourceProject._Closure$__.$I31-3 = func6
                End If
                Dim enumerable5 As IEnumerable(Of String) = enumerable4.[Select](func5)
                Dim func7 As Func(Of String, Boolean)
                If ResourceProject._Closure$__.$I31-4 IsNot Nothing Then
                    func7 = ResourceProject._Closure$__.$I31-4
                Else
                    Dim func8 As Func(Of String, Boolean) = Function(w As String) Not String.IsNullOrEmpty(w)
                    func7 = func8
                    ResourceProject._Closure$__.$I31-4 = func8
                End If
                list = enumerable5.Where(func7).ToList()
                If list.Count > 1 Then
                    Dim enumerable6 As IEnumerable(Of String) = list
                    Dim func9 As Func(Of String, Boolean)
                    If ResourceProject._Closure$__.$I31-5 IsNot Nothing Then
                        func9 = ResourceProject._Closure$__.$I31-5
                    Else
                        Dim func10 As Func(Of String, Boolean) = Function(s As String) s.Lower().Contains("forge") OrElse s.Lower().Contains("fabric") OrElse s.Lower().Contains("quilt")
                        func9 = func10
                        ResourceProject._Closure$__.$I31-5 = func10
                    End If
                    If Not enumerable6.Any(func9) AndAlso (list.Count <> 2 OrElse Operators.CompareString(list.Last().Upper(), list.Last(), False) <> 0) Then
                        list = New List(Of String)() From { text4 }
                    End If
                End If
                If Operators.CompareString(text3, "", False) <> 0 Then
                    list.Add(text3)
                End If
            End If
            list = list.Distinct().ToList()
            Dim text5 As String = ""
            If list.Any() Then
                Try
                    For Each text6 As String In list
                        Dim flag As Boolean = text6.Lower().Contains("neoforge") OrElse text6.Lower().Contains("forge") OrElse text6.Lower().Contains("fabric") OrElse text6.Lower().Contains("quilt")
                        If(HasModLoaderDescription OrElse Not flag) AndAlso (text6.Length >= 16 OrElse Not text6.Lower().Contains("fabric") OrElse Not text6.Lower().Contains("forge")) Then
                            If flag AndAlso Not text6.Contains("版") AndAlso text6.Lower().Replace("neoforge", "").Replace("forge", "").Replace("fabric", "").Replace("quilt", "").Length <= 3 Then
                                text6 = text6.Replace("Edition", "").Replace("edition", "").Trim().Capitalize() + " 版"
                            End If
                            text6 = text6.Replace("neoforge", "NeoForge").Replace("forge", "Forge").Replace("neo", "Neo").Replace("fabric", "Fabric").Replace("quilt", "Quilt")
                            text5 = text5 + "  |  " + text6.Trim()
                        End If
                    Next
                    GoTo IL_05F2
                Finally
                    Dim enumerator2 As List(Of String).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
            End If
            IL_05EC:
            text5 = ""
            IL_05F2:
            Return New ValueTuple(Of String, String)(text, text5)
        End Function

        Public Function IsLike(Project As ResourceProject) As Boolean
            Dim flag As Boolean
            If Operators.CompareString(Me.Id, Project.Id, False) = 0 Then
                flag = True
            Else
                Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_1(Of String, String)
                If ResourceProject._Closure$__.$I33-0 IsNot Nothing Then
                    vb$AnonymousDelegate_ = ResourceProject._Closure$__.$I33-0
                Else
                    Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_1(Of String, String) = Function(Data As String)
                        Dim stringBuilder As StringBuilder = New StringBuilder()
                        Try
                            Dim func As Func(Of Char, Boolean)
                            If ResourceProject._Closure$__.$I33-1 IsNot Nothing Then
                                func = ResourceProject._Closure$__.$I33-1
                            Else
                                Dim func2 As Func(Of Char, Boolean) = Function(c As Char) Char.IsLetterOrDigit(c)
                                func = func2
                                ResourceProject._Closure$__.$I33-1 = func2
                            End If
                            For Each c2 As Char In Data.Where(func)
                                stringBuilder.Append(c2)
                            Next
                        Finally
                            Dim enumerator As IEnumerator(Of Char)
                            If enumerator IsNot Nothing Then
                                enumerator.Dispose()
                            End If
                        End Try
                        Return stringBuilder.ToString().Lower()
                    End Function
                    vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                    ResourceProject._Closure$__.$I33-0 = vb$AnonymousDelegate_2
                End If
                Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_1(Of String, String) = vb$AnonymousDelegate_
                If Me.Platform = Project.Platform Then
                    flag = False
                ElseIf Me.ModLoaders <> Project.ModLoaders Then
                    flag = False
                ElseIf Me.Types <> ResourceTypes.Shader AndAlso Me.Drops.Ordered().SequenceEqual(Project.Drops.Ordered()) Then
                    flag = False
                ElseIf Math.Abs((Me.LastUpdate - Project.LastUpdate).TotalDays) > 7.0 Then
                    flag = False
                ElseIf Operators.CompareString(Me.TranslatedName, Project.TranslatedName, False) = 0 OrElse Operators.CompareString(Me.RawName, Project.RawName, False) = 0 OrElse Operators.CompareString(Me.Description, Project.Description, False) = 0 OrElse Operators.CompareString(vb$AnonymousDelegate_3(Me.Slug), vb$AnonymousDelegate_3(Project.Slug), False) = 0 Then
                    Logger.Info(String.Format("将 {0} ({1}) 与 {2} ({3}) 认定为相似工程", New Object() { Me.RawName, Me.Slug, Project.RawName, Project.Slug }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceProject.vb")
                    If Me.WikiEntry Is Nothing AndAlso Project.WikiEntry IsNot Nothing Then
                        Me.WikiEntry = Project.WikiEntry
                    End If
                    If Me.WikiEntry IsNot Nothing AndAlso Project.WikiEntry Is Nothing Then
                        Project.WikiEntry = Me.WikiEntry
                    End If
                    flag = True
                Else
                    flag = False
                End If
            End If
            Return flag
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1}): {2}", Me.Id, Me.Slug, Me.RawName)
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim resourceProject As ResourceProject = TryCast(obj, ResourceProject)
            Return resourceProject IsNot Nothing AndAlso Operators.CompareString(Me.Id, resourceProject.Id, False) = 0
        End Function

        Public Shared Operator =(left As ResourceProject, right As ResourceProject) As Boolean
            Return EqualityComparer(Of ResourceProject).[Default].Equals(left, right)
        End Operator

        Public Shared Operator <>(left As ResourceProject, right As ResourceProject) As Boolean
            Return Not(left Is right)
        End Operator

        Public Platform As ResourcePlatforms

        Public Types As ResourceTypes

        Public Slug As String

        Public Id As String

        Public CurseForgeFileIds As List(Of Integer)

        Public RawName As String

        Public Description As String

        Public Website As String

        Public LastUpdate As DateTime

        Public DownloadCount As Integer

        Public ModLoaders As ModLoaders

        Public Tags As List(Of String)

        Public LogoUrl As String

        Public Drops As List(Of Integer)

        Public UnsafeGameVersions As List(Of String)

        Private LoadedDatabase As Boolean

        Private _WikiEntry As WikiEntry

        Protected Friend Shared Cache As Dictionary(Of String, ResourceProject) = New Dictionary(Of String, ResourceProject)()
    End Class
End Namespace
