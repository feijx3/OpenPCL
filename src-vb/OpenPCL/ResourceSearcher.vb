Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public Class ResourceSearcher
        Public Shared Function GetLoader(InputDelegate As Func(Of ResourceSearcher.SearchRequest)) As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer)
            Return New ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer)("社区资源获取", AddressOf ResourceSearcher.Run, InputDelegate, ThreadPriority.Normal) With { .ReloadTimeout = 60000 }
        End Function

        Private Shared Sub Run(Task As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer))
            Dim CS$<>8__locals1 As ResourceSearcher._Closure$__6-3 = New ResourceSearcher._Closure$__6-3(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Task = Task
            CS$<>8__locals1.$VB$Local_Request = CS$<>8__locals1.$VB$Local_Task.Input
            CS$<>8__locals1.$VB$Local_Storage = CS$<>8__locals1.$VB$Local_Request.Storage
            If CS$<>8__locals1.$VB$Local_Storage.Results.Count >= CS$<>8__locals1.$VB$Local_Request.TargetResultCount Then
                Logger.Info(String.Format("已有 {0} 个结果，多于所需的 {1} 个结果，结束处理", CS$<>8__locals1.$VB$Local_Storage.Results.Count, CS$<>8__locals1.$VB$Local_Request.TargetResultCount), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                Return
            End If
            If CS$<>8__locals1.$VB$Local_Request.CanContinue Then
                CS$<>8__locals1.$VB$Local_RawSearchText = (If(CS$<>8__locals1.$VB$Local_Request.SearchText, "")).Trim()
                CS$<>8__locals1.$VB$Local_RawSearchText = CS$<>8__locals1.$VB$Local_RawSearchText.Lower()
                Logger.Info(String.Format("工程列表搜索原始文本：{0}", CS$<>8__locals1.$VB$Local_RawSearchText), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                CS$<>8__locals1.$VB$Local_RawSearchText = Strings.StrConv(CS$<>8__locals1.$VB$Local_RawSearchText, VbStrConv.SimplifiedChinese, 0)
                Dim flag As Boolean = CS$<>8__locals1.$VB$Local_RawSearchText.RegexCheck("[\u4e00-\u9fbb]", RegexOptions.None) AndAlso Not String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_RawSearchText) AndAlso (CS$<>8__locals1.$VB$Local_Request.Type = ResourceTypes.[Mod] OrElse CS$<>8__locals1.$VB$Local_Request.Type = ResourceTypes.DataPack)
                CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText = Nothing
                CS$<>8__locals1.$VB$Local_ModrinthAltSearchText = Nothing
                CS$<>8__locals1.$VB$Local_ModrinthSlugs = New List(Of String)()
                If flag Then
                    Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_2(Of ModBase.SearchEntry(Of WikiEntry), ResourcePlatforms, IEnumerable(Of String))
                    If ResourceSearcher._Closure$__.$I6-0 IsNot Nothing Then
                        vb$AnonymousDelegate_ = ResourceSearcher._Closure$__.$I6-0
                    Else
                        Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_2(Of ModBase.SearchEntry(Of WikiEntry), ResourcePlatforms, IEnumerable(Of String)) = Function(Result As ModBase.SearchEntry(Of WikiEntry), Source As ResourcePlatforms)
                            Dim CS$<>8__locals5 As ResourceSearcher._Closure$__6-0 = New ResourceSearcher._Closure$__6-0(CS$<>8__locals5)
                            CS$<>8__locals5.$VB$Local_Candidates = New List(Of String)()
                            If Result.Item.Slugs.ContainsKey(Source) Then
                                CS$<>8__locals5.$VB$Local_Candidates.Add(Result.Item.Slugs(Source).Replace("-", " ").Replace("/", " "))
                            End If
                            If Result.Item.ChineseName IsNot Nothing Then
                                CS$<>8__locals5.$VB$Local_Candidates.Add(Result.Item.ChineseName.AfterLast(" (", False).TrimEnd(New Char() { ")"c }).BeforeFirst(" - ", False).Replace("-", " ").Replace("/", " ").Replace(":", " ").Replace("(", " ").Replace(")", ""))
                            End If
                            Dim CS$<>8__locals6 As ResourceSearcher._Closure$__6-0 = CS$<>8__locals5
                            Dim $VB$Local_Candidates As IEnumerable(Of String) = CS$<>8__locals5.$VB$Local_Candidates
                            Dim func28 As Func(Of String, IEnumerable(Of String))
                            If ResourceSearcher._Closure$__.$I6-1 IsNot Nothing Then
                                func28 = ResourceSearcher._Closure$__.$I6-1
                            Else
                                Dim func29 As Func(Of String, IEnumerable(Of String)) = Function(c As String) c.Split(" ", False)
                                func28 = func29
                                ResourceSearcher._Closure$__.$I6-1 = func29
                            End If
                            Dim enumerable11 As IEnumerable(Of String) = $VB$Local_Candidates.SelectMany(func28)
                            Dim func30 As Func(Of String, String)
                            If ResourceSearcher._Closure$__.$I6-2 IsNot Nothing Then
                                func30 = ResourceSearcher._Closure$__.$I6-2
                            Else
                                Dim func31 As Func(Of String, String) = Function(w As String) w.TrimStart(New Char() { "{"c }).TrimEnd(New Char() { "}"c }).Lower()
                                func30 = func31
                                ResourceSearcher._Closure$__.$I6-2 = func31
                            End If
                            Dim enumerable12 As IEnumerable(Of String) = enumerable11.[Select](func30)
                            Dim func32 As Func(Of String, Boolean)
                            If ResourceSearcher._Closure$__.$I6-3 IsNot Nothing Then
                                func32 = ResourceSearcher._Closure$__.$I6-3
                            Else
                                Dim func33 As Func(Of String, Boolean) = Function(w As String) w.Length > 1 AndAlso Not New String() { "the", "of", "mod", "and", "forge", "fabric", "for", "quilt", "neoforge" }.Contains(w) AndAlso ModBase.Val(w) <= 0.0 AndAlso w.IsAsciiOnly()
                                func32 = func33
                                ResourceSearcher._Closure$__.$I6-3 = func33
                            End If
                            CS$<>8__locals6.$VB$Local_Candidates = enumerable12.Where(func32).Distinct().ToList()
                            CS$<>8__locals5.$VB$Local_CanForm = Nothing
                            CS$<>8__locals5.$VB$Local_CanForm = Function(s As String)
                                Dim CS$<>8__locals7 As ResourceSearcher._Closure$__6-1 = New ResourceSearcher._Closure$__6-1(CS$<>8__locals7)
                                CS$<>8__locals7.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals5
                                CS$<>8__locals7.$VB$Local_s = s
                                Return CS$<>8__locals5.$VB$Local_Candidates.Contains(CS$<>8__locals7.$VB$Local_s) OrElse CS$<>8__locals5.$VB$Local_Candidates.Any(Function(c As String) CS$<>8__locals7.$VB$Local_s.StartsWith(c) AndAlso CS$<>8__locals7.$VB$NonLocal_$VB$Closure_2.$VB$Local_CanForm(CS$<>8__locals7.$VB$Local_s.Substring(c.Length)))
                            End Function
                            CS$<>8__locals5.$VB$Local_Candidates = CS$<>8__locals5.$VB$Local_Candidates.Where(Function(w As String)
                                Dim CS$<>8__locals8 As ResourceSearcher._Closure$__6-2 = New ResourceSearcher._Closure$__6-2(CS$<>8__locals8)
                                CS$<>8__locals8.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals5
                                CS$<>8__locals8.$VB$Local_w = w
                                Return Not CS$<>8__locals5.$VB$Local_Candidates.Any(Function(c As String) c.Length < CS$<>8__locals8.$VB$Local_w.Length AndAlso CS$<>8__locals8.$VB$Local_w.StartsWith(c) AndAlso CS$<>8__locals8.$VB$NonLocal_$VB$Closure_3.$VB$Local_CanForm(CS$<>8__locals8.$VB$Local_w.Substring(c.Length)))
                            End Function).ToList()
                            Return CS$<>8__locals5.$VB$Local_Candidates
                        End Function
                        vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                        ResourceSearcher._Closure$__.$I6-0 = vb$AnonymousDelegate_2
                    End If
                    Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_2(Of ModBase.SearchEntry(Of WikiEntry), ResourcePlatforms, IEnumerable(Of String)) = vb$AnonymousDelegate_
                    Dim vb$AnonymousDelegate_4 As VB$AnonymousDelegate_3(Of ResourcePlatforms, IEnumerable(Of ModBase.SearchEntry(Of WikiEntry)))
                    If ResourceSearcher._Closure$__.$I6-8 IsNot Nothing Then
                        vb$AnonymousDelegate_4 = ResourceSearcher._Closure$__.$I6-8
                    Else
                        Dim vb$AnonymousDelegate_5 As VB$AnonymousDelegate_3(Of ResourcePlatforms, IEnumerable(Of ModBase.SearchEntry(Of WikiEntry))) = Function(Source As ResourcePlatforms)
                            Dim vb$StateMachine___Lambda$__6- As ResourceSearcher._Closure$__.VB$StateMachine___Lambda$__6-8 = New ResourceSearcher._Closure$__.VB$StateMachine___Lambda$__6-8(-2)
                            vb$StateMachine___Lambda$__6-.$VB$NonLocal__Closure$__ = ResourceSearcher._Closure$__.$I
                            vb$StateMachine___Lambda$__6-.$P_Source = Source
                            Return vb$StateMachine___Lambda$__6-
                        End Function
                        vb$AnonymousDelegate_4 = vb$AnonymousDelegate_5
                        ResourceSearcher._Closure$__.$I6-8 = vb$AnonymousDelegate_5
                    End If
                    Dim vb$AnonymousDelegate_6 As VB$AnonymousDelegate_3(Of ResourcePlatforms, IEnumerable(Of ModBase.SearchEntry(Of WikiEntry))) = vb$AnonymousDelegate_4
                    If CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.CurseForge) Then
                        If ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag2 As Boolean = False
                        Try
                            Monitor.Enter(ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init, flag2)
                            If ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State = 0S Then
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State = 2S
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries = vb$AnonymousDelegate_6(ResourcePlatforms.CurseForge).ToList()
                            ElseIf ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init.State = 1S
                            If flag2 Then
                                Monitor.[Exit](ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init)
                            End If
                        End Try
                        Dim list As List(Of ModBase.SearchEntry(Of WikiEntry)) = ModBase.Search(Of WikiEntry)(ResourceSearcher.$STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries, CS$<>8__locals1.$VB$Local_RawSearchText, 100, 0.25)
                        If list.Any() Then
                            Dim enumerable2 As IEnumerable(Of ModBase.SearchEntry(Of WikiEntry))
                            If Not list.First().AbsoluteRight Then
                                Dim enumerable As IEnumerable(Of ModBase.SearchEntry(Of WikiEntry)) = list
                                Dim func As Func(Of ModBase.SearchEntry(Of WikiEntry), Double)
                                If ResourceSearcher._Closure$__.$I6-10 IsNot Nothing Then
                                    func = ResourceSearcher._Closure$__.$I6-10
                                Else
                                    Dim func2 As Func(Of ModBase.SearchEntry(Of WikiEntry), Double) = Function(s As ModBase.SearchEntry(Of WikiEntry)) s.Similarity
                                    func = func2
                                    ResourceSearcher._Closure$__.$I6-10 = func2
                                End If
                                enumerable2 = enumerable.MaxByAll(func)
                            Else
                                Dim enumerable3 As IEnumerable(Of ModBase.SearchEntry(Of WikiEntry)) = list
                                Dim func3 As Func(Of ModBase.SearchEntry(Of WikiEntry), Boolean)
                                If ResourceSearcher._Closure$__.$I6-9 IsNot Nothing Then
                                    func3 = ResourceSearcher._Closure$__.$I6-9
                                Else
                                    Dim func4 As Func(Of ModBase.SearchEntry(Of WikiEntry), Boolean) = Function(s As ModBase.SearchEntry(Of WikiEntry)) s.AbsoluteRight
                                    func3 = func4
                                    ResourceSearcher._Closure$__.$I6-9 = func4
                                End If
                                enumerable2 = enumerable3.Where(func3).ToList()
                            End If
                            Dim func5 As Func(Of ModBase.SearchEntry(Of WikiEntry), Integer)
                            If ResourceSearcher._Closure$__.$I6-11 IsNot Nothing Then
                                func5 = ResourceSearcher._Closure$__.$I6-11
                            Else
                                Dim func6 As Func(Of ModBase.SearchEntry(Of WikiEntry), Integer) = Function(s As ModBase.SearchEntry(Of WikiEntry)) s.Item.Popularity
                                func5 = func6
                                ResourceSearcher._Closure$__.$I6-11 = func6
                            End If
                            Dim searchEntry As ModBase.SearchEntry(Of WikiEntry) = enumerable2.MaxBy(func5)
                            CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText = vb$AnonymousDelegate_3(searchEntry, ResourcePlatforms.CurseForge).Join(" ")
                            Logger.Warn(String.Format("中文搜索关键词（CurseForge）：{0}", CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                        End If
                    End If
                    If CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth) Then
                        If ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag3 As Boolean = False
                        Try
                            Monitor.Enter(ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init, flag3)
                            If ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State = 0S Then
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State = 2S
                                ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries = vb$AnonymousDelegate_6(ResourcePlatforms.Modrinth).ToList()
                            ElseIf ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init.State = 1S
                            If flag3 Then
                                Monitor.[Exit](ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init)
                            End If
                        End Try
                        Dim list2 As List(Of ModBase.SearchEntry(Of WikiEntry)) = ModBase.Search(Of WikiEntry)(ResourceSearcher.$STATIC$Run$011151286FC21283B48$ModrinthSearchEntries, CS$<>8__locals1.$VB$Local_RawSearchText, 100, 0.25)
                        If list2.Any() Then
                            Dim dictionary As Dictionary(Of String, Double) = New Dictionary(Of String, Double)()
                            Try
                                For Each searchEntry2 As ModBase.SearchEntry(Of WikiEntry) In list2
                                    Try
                                        For Each text As String In vb$AnonymousDelegate_3(searchEntry2, ResourcePlatforms.Modrinth)
                                            If Not dictionary.ContainsKey(text) Then
                                                dictionary.Add(text, 0.0)
                                            End If
                                            Dim searchSource As IEnumerable(Of ModBase.SearchSource) = searchEntry2.SearchSource
                                            Dim func7 As Func(Of ModBase.SearchSource, Boolean)
                                            If CS$<>8__locals1.$I12 IsNot Nothing Then
                                                func7 = CS$<>8__locals1.$I12
                                            Else
                                                Dim CS$<>8__locals2 As ResourceSearcher._Closure$__6-3 = CS$<>8__locals1
                                                Dim func8 As Func(Of ModBase.SearchSource, Boolean) = Function(s As ModBase.SearchSource) s.Aliases.Contains(CS$<>8__locals1.$VB$Local_RawSearchText)
                                                Dim func9 As Func(Of ModBase.SearchSource, Boolean) = func8
                                                CS$<>8__locals2.$I12 = func8
                                                func7 = func9
                                            End If
                                            Dim num As Double = If(searchSource.Any(func7), 1000.0, searchEntry2.Similarity)
                                            Dim dictionary2 As Dictionary(Of String, Double) = dictionary
                                            Dim dictionary3 As Dictionary(Of String, Double) = dictionary2
                                            Dim text2 As String = text
                                            Dim text3 As String = text2
                                            dictionary2(text2) = dictionary3(text3) + num * CDbl(searchEntry2.Item.Popularity)
                                        Next
                                    Finally
                                        Dim enumerator2 As IEnumerator(Of String)
                                        If enumerator2 IsNot Nothing Then
                                            enumerator2.Dispose()
                                        End If
                                    End Try
                                Next
                            Finally
                                Dim enumerator As List(Of ModBase.SearchEntry(Of WikiEntry)).Enumerator
                                CType(enumerator, IDisposable).Dispose()
                            End Try
                            Dim CS$<>8__locals3 As ResourceSearcher._Closure$__6-3 = CS$<>8__locals1
                            Dim enumerable4 As IEnumerable(Of KeyValuePair(Of String, Double)) = dictionary
                            Dim func10 As Func(Of KeyValuePair(Of String, Double), Double)
                            If ResourceSearcher._Closure$__.$I6-13 IsNot Nothing Then
                                func10 = ResourceSearcher._Closure$__.$I6-13
                            Else
                                Dim func11 As Func(Of KeyValuePair(Of String, Double), Double) = Function(w As KeyValuePair(Of String, Double)) w.Value
                                func10 = func11
                                ResourceSearcher._Closure$__.$I6-13 = func11
                            End If
                            CS$<>8__locals3.$VB$Local_ModrinthAltSearchText = enumerable4.MaxBy(func10).Key
                            Logger.Warn(String.Format("中文搜索关键词（Modrinth）：{0}", CS$<>8__locals1.$VB$Local_ModrinthAltSearchText), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                            Dim CS$<>8__locals4 As ResourceSearcher._Closure$__6-3 = CS$<>8__locals1
                            Dim enumerable5 As IEnumerable(Of ModBase.SearchEntry(Of WikiEntry)) = list2.Take(100)
                            Dim func12 As Func(Of ModBase.SearchEntry(Of WikiEntry), String)
                            If ResourceSearcher._Closure$__.$I6-14 IsNot Nothing Then
                                func12 = ResourceSearcher._Closure$__.$I6-14
                            Else
                                Dim func13 As Func(Of ModBase.SearchEntry(Of WikiEntry), String) = Function(r As ModBase.SearchEntry(Of WikiEntry)) r.Item.Slugs(ResourcePlatforms.Modrinth)
                                func12 = func13
                                ResourceSearcher._Closure$__.$I6-14 = func13
                            End If
                            CS$<>8__locals4.$VB$Local_ModrinthSlugs = enumerable5.[Select](func12).ToList()
                        End If
                    End If
                    If String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText) AndAlso String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_ModrinthAltSearchText) AndAlso Not CS$<>8__locals1.$VB$Local_ModrinthSlugs.Any() Then
                        Throw New Exception("无搜索结果，请尝试搜索其英文名称")
                    End If
                End If
                CS$<>8__locals1.$VB$Local_Task.Progress = 0.05
                CS$<>8__locals1.$VB$Local_RealResults = New List(Of ResourceProject)()
                While True
                    Dim list3 As List(Of ResourceProject) = New List(Of ResourceProject)()
                    Dim list4 As List(Of Thread) = New List(Of Thread)()
                    CS$<>8__locals1.$VB$Local_Errors = New ConcurrentBag(Of ValueTuple(Of Exception, ResourcePlatforms))()
                    CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter = CS$<>8__locals1.$VB$Local_Request.ModLoaders = ModLoaders.Forge AndAlso McVersion.VersionToDrop(CS$<>8__locals1.$VB$Local_Request.GameVersion) < 140
                    CS$<>8__locals1.$VB$Local_SearchResults = New ConcurrentBag(Of ResourceProject)()
                    Try
                        If CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.CurseForge) AndAlso (CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal <= -1 OrElse CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal > CS$<>8__locals1.$VB$Local_Storage.CurseForgeOffset) AndAlso (Not flag OrElse (flag AndAlso Not String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText))) Then
                            list4.Add(ModBase.RunInNewThread(Sub()
                                Try
                                    Dim curseForgeAddress As String = CS$<>8__locals1.$VB$Local_Request.GetCurseForgeAddress(If(CS$<>8__locals1.$VB$Local_CurseForgeAltSearchText, CS$<>8__locals1.$VB$Local_RawSearchText), CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter)
                                    Logger.Info(String.Format("开始 CurseForge 搜索：{0}", curseForgeAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                    Dim jobject As JObject = CType(ModDownload.DlModRequest(curseForgeAddress, Nothing, Nothing, Nothing), JObject)
                                    Dim list8 As List(Of ResourceProject) = New List(Of ResourceProject)()
                                    Try
                                        For Each jtoken As JToken In jobject("data")
                                            Dim jobject2 As JObject = CType(jtoken, JObject)
                                            Dim resourceProject4 As ResourceProject = New ResourceProject(jobject2)
                                            If(CS$<>8__locals1.$VB$Local_Request.Type <> ResourceTypes.ResourcePack OrElse Not resourceProject4.Tags.Contains("数据包")) AndAlso (CS$<>8__locals1.$VB$Local_Request.Type = ResourceTypes.Any OrElse resourceProject4.Types.HasFlagF(CS$<>8__locals1.$VB$Local_Request.Type)) Then
                                                list8.Add(resourceProject4)
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator8 As IEnumerator(Of JToken)
                                        If enumerator8 IsNot Nothing Then
                                            enumerator8.Dispose()
                                        End If
                                    End Try
                                    Dim list9 As List(Of ResourceProject) = list8
                                    Dim action As Action(Of ResourceProject)
                                    If CS$<>8__locals1.$I16 IsNot Nothing Then
                                        action = CS$<>8__locals1.$I16
                                    Else
                                        Dim action2 As Action(Of ResourceProject) = Sub(p As ResourceProject)
                                            CS$<>8__locals1.$VB$Local_SearchResults.Add(p)
                                        End Sub
                                        Dim action3 As Action(Of ResourceProject) = action2
                                        CS$<>8__locals1.$I16 = action2
                                        action = action3
                                    End If
                                    list9.ForEach(action)
                                    Dim $VB$Local_Storage As ResourceSearcher.SearchResult = CS$<>8__locals1.$VB$Local_Storage
                                    Dim ptr As __ByRef(Of Integer) = $VB$Local_Storage.CurseForgeOffset
                                    $VB$Local_Storage.CurseForgeOffset = ptr + jobject("data").Count()
                                    CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal = jobject("pagination")("totalCount").ToObject(Of Integer)()
                                    Logger.Info(String.Format("从 CurseForge 搜索到了 {0} 个工程（总计已获取 {1} 个，共 {2} 个）", list8.Count, CS$<>8__locals1.$VB$Local_Storage.CurseForgeOffset, CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                Catch ex As Exception
                                    Logger.Warn(ex, "CurseForge 搜索失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                    CS$<>8__locals1.$VB$Local_Storage.CurseForgeTotal = -1
                                    CS$<>8__locals1.$VB$Local_Errors.Add(New ValueTuple(Of Exception, ResourcePlatforms)(ex, ResourcePlatforms.CurseForge))
                                End Try
                                If CS$<>8__locals1.$VB$Local_Task.Progress < 0.75 Then
                                    Dim $VB$Local_Task As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer) = CS$<>8__locals1.$VB$Local_Task
                                    Dim loaderTask As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer) = $VB$Local_Task
                                    $VB$Local_Task.Progress = loaderTask.Progress + 0.25
                                End If
                            End Sub, "CurseForge Search", ThreadPriority.Normal))
                        End If
                        If CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth) AndAlso (CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal <= -1 OrElse CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal > CS$<>8__locals1.$VB$Local_Storage.ModrinthOffset) AndAlso (Not flag OrElse (flag AndAlso Not String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_ModrinthAltSearchText))) Then
                            list4.Add(ModBase.RunInNewThread(Sub()
                                Try
                                    Dim modrinthAddress As String = CS$<>8__locals1.$VB$Local_Request.GetModrinthAddress(If(CS$<>8__locals1.$VB$Local_ModrinthAltSearchText, CS$<>8__locals1.$VB$Local_RawSearchText), CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter)
                                    Logger.Info(String.Format("开始 Modrinth 搜索：{0}", modrinthAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                    Dim jobject3 As JObject = CType(ModDownload.DlModRequest(modrinthAddress, Nothing, Nothing, Nothing), JObject)
                                    Dim list10 As List(Of ResourceProject) = New List(Of ResourceProject)()
                                    Try
                                        For Each jtoken2 As JToken In jobject3("hits")
                                            Dim jobject4 As JObject = CType(jtoken2, JObject)
                                            list10.Add(New ResourceProject(jobject4))
                                        Next
                                    Finally
                                        Dim enumerator9 As IEnumerator(Of JToken)
                                        If enumerator9 IsNot Nothing Then
                                            enumerator9.Dispose()
                                        End If
                                    End Try
                                    Dim list11 As List(Of ResourceProject) = list10
                                    Dim action4 As Action(Of ResourceProject)
                                    If CS$<>8__locals1.$I18 IsNot Nothing Then
                                        action4 = CS$<>8__locals1.$I18
                                    Else
                                        Dim action5 As Action(Of ResourceProject) = Sub(p As ResourceProject)
                                            CS$<>8__locals1.$VB$Local_SearchResults.Add(p)
                                        End Sub
                                        Dim action6 As Action(Of ResourceProject) = action5
                                        CS$<>8__locals1.$I18 = action5
                                        action4 = action6
                                    End If
                                    list11.ForEach(action4)
                                    Dim $VB$Local_Storage2 As ResourceSearcher.SearchResult = CS$<>8__locals1.$VB$Local_Storage
                                    Dim ptr2 As __ByRef(Of Integer) = $VB$Local_Storage2.ModrinthOffset
                                    $VB$Local_Storage2.ModrinthOffset = ptr2 + jobject3("hits").Count()
                                    CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal = jobject3("total_hits").ToObject(Of Integer)()
                                    Logger.Info(String.Format("从 Modrinth 搜索到了 {0} 个工程（总计已获取 {1} 个，共 {2} 个）", list10.Count, CS$<>8__locals1.$VB$Local_Storage.ModrinthOffset, CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                Catch ex2 As Exception
                                    Logger.Warn(ex2, "Modrinth 搜索失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                    CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal = -1
                                    CS$<>8__locals1.$VB$Local_Errors.Add(New ValueTuple(Of Exception, ResourcePlatforms)(ex2, ResourcePlatforms.Modrinth))
                                End Try
                                If CS$<>8__locals1.$VB$Local_Task.Progress < 0.75 Then
                                    Dim $VB$Local_Task2 As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer) = CS$<>8__locals1.$VB$Local_Task
                                    Dim loaderTask2 As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer) = $VB$Local_Task2
                                    $VB$Local_Task2.Progress = loaderTask2.Progress + 0.25
                                End If
                            End Sub, "Modrinth Search", ThreadPriority.Normal))
                        End If
                        If CS$<>8__locals1.$VB$Local_Request.Sources.HasFlagF(ResourcePlatforms.Modrinth) AndAlso (CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal <= -1 OrElse CS$<>8__locals1.$VB$Local_Storage.ModrinthTotal > CS$<>8__locals1.$VB$Local_Storage.ModrinthOffset) AndAlso CS$<>8__locals1.$VB$Local_ModrinthSlugs.Any() Then
                            list4.Add(ModBase.RunInNewThread(Sub()
                                Try
                                    Dim text4 As String = String.Format("https://api.modrinth.com/v2/projects?ids=[""{0}""]", CS$<>8__locals1.$VB$Local_ModrinthSlugs.Join(""","""))
                                    Logger.Info(String.Format("开始 Modrinth 直接获取：{0}", text4), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                    Dim list12 As List(Of ResourceProject) = New List(Of ResourceProject)()
                                    Try
                                        For Each jtoken3 As JToken In ModDownload.DlModRequest(text4, Nothing, Nothing, Nothing)
                                            Dim jobject5 As JObject = CType(jtoken3, JObject)
                                            Dim resourceProject5 As ResourceProject = New ResourceProject(jobject5)
                                            If CS$<>8__locals1.$VB$Local_Request.Type = ResourceTypes.Any OrElse resourceProject5.Types.HasFlagF(CS$<>8__locals1.$VB$Local_Request.Type) Then
                                                If Not String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_Request.Tag) Then
                                                    Dim enumerable13 As IEnumerable(Of JToken) = jobject5("categories")
                                                    Dim func34 As Func(Of JToken, Boolean)
                                                    If CS$<>8__locals1.$I20 IsNot Nothing Then
                                                        func34 = CS$<>8__locals1.$I20
                                                    Else
                                                        Dim func35 As Func(Of JToken, Boolean) = Function(c As JToken) Operators.CompareString(c.ToString(), CS$<>8__locals1.$VB$Local_Request.Tag.AfterLast("/", False), False) = 0
                                                        Dim func36 As Func(Of JToken, Boolean) = func35
                                                        CS$<>8__locals1.$I20 = func35
                                                        func34 = func36
                                                    End If
                                                    If Not enumerable13.Any(func34) Then
                                                        Continue For
                                                    End If
                                                End If
                                                If CS$<>8__locals1.$VB$Local_Request.ModLoaders = ModLoaders.None OrElse CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter OrElse resourceProject5.ModLoaders.Flags().Intersect(CS$<>8__locals1.$VB$Local_Request.ModLoaders.Flags()).Any() Then
                                                    If Not String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_Request.GameVersion) Then
                                                        Dim unsafeGameVersions As IEnumerable(Of String) = resourceProject5.UnsafeGameVersions
                                                        Dim func37 As Func(Of String, Boolean)
                                                        If CS$<>8__locals1.$I21 IsNot Nothing Then
                                                            func37 = CS$<>8__locals1.$I21
                                                        Else
                                                            Dim func38 As Func(Of String, Boolean) = Function(d As String) Operators.CompareString(d, CS$<>8__locals1.$VB$Local_Request.GameVersion, False) = 0
                                                            Dim func39 As Func(Of String, Boolean) = func38
                                                            CS$<>8__locals1.$I21 = func38
                                                            func37 = func39
                                                        End If
                                                        If Not unsafeGameVersions.Any(func37) Then
                                                            Continue For
                                                        End If
                                                    End If
                                                    list12.Add(resourceProject5)
                                                End If
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator10 As IEnumerator(Of JToken)
                                        If enumerator10 IsNot Nothing Then
                                            enumerator10.Dispose()
                                        End If
                                    End Try
                                    If list12.Any() AndAlso CS$<>8__locals1.$VB$Local_Request.ModLoaders <> ModLoaders.None AndAlso Not CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter AndAlso Not String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_Request.GameVersion) Then
                                        Dim CS$<>8__locals9 As ResourceSearcher._Closure$__6-4 = New ResourceSearcher._Closure$__6-4(CS$<>8__locals9)
                                        Dim enumerable14 As IEnumerable(Of ResourceProject) = list12
                                        Dim func40 As Func(Of ResourceProject, String)
                                        If ResourceSearcher._Closure$__.$I6-22 IsNot Nothing Then
                                            func40 = ResourceSearcher._Closure$__.$I6-22
                                        Else
                                            Dim func41 As Func(Of ResourceProject, String) = Function(p As ResourceProject) p.Id
                                            func40 = func41
                                            ResourceSearcher._Closure$__.$I6-22 = func41
                                        End If
                                        Dim text5 As String = enumerable14.[Select](func40).Join(""",""project_id:")
                                        Dim enumerable15 As IEnumerable(Of ModLoaders) = CS$<>8__locals1.$VB$Local_Request.ModLoaders.Flags()
                                        Dim func42 As Func(Of ModLoaders, String)
                                        If ResourceSearcher._Closure$__.$I6-23 IsNot Nothing Then
                                            func42 = ResourceSearcher._Closure$__.$I6-23
                                        Else
                                            Dim func43 As Func(Of ModLoaders, String) = Function(f As ModLoaders) f.ToString().Lower()
                                            func42 = func43
                                            ResourceSearcher._Closure$__.$I6-23 = func43
                                        End If
                                        Dim text6 As String = enumerable15.[Select](func42).Join(""",""categories:")
                                        Dim text7 As String = String.Format("https://api.modrinth.com/v2/search?limit={0}&facets=", list12.Count) + String.Format("[[""project_id:{0}""],[""categories:{1}""],[""versions:{2}""]]", text5, text6, CS$<>8__locals1.$VB$Local_Request.GameVersion)
                                        Dim CS$<>8__locals10 As ResourceSearcher._Closure$__6-4 = CS$<>8__locals9
                                        Dim enumerable16 As IEnumerable(Of JToken) = ModDownload.DlModRequest(text7, Nothing, Nothing, Nothing)("hits")
                                        Dim func44 As Func(Of JToken, String)
                                        If ResourceSearcher._Closure$__.$I6-24 IsNot Nothing Then
                                            func44 = ResourceSearcher._Closure$__.$I6-24
                                        Else
                                            Dim func45 As Func(Of JToken, String) = Function(h As JToken) h("project_id").ToString()
                                            func44 = func45
                                            ResourceSearcher._Closure$__.$I6-24 = func45
                                        End If
                                        CS$<>8__locals10.$VB$Local_MatchedIds = enumerable16.[Select](func44).ToList()
                                        list12.KeepIf(Function(p As ResourceProject) CS$<>8__locals9.$VB$Local_MatchedIds.Contains(p.Id))
                                    End If
                                    Dim list13 As List(Of ResourceProject) = list12
                                    Dim action7 As Action(Of ResourceProject)
                                    If CS$<>8__locals1.$I26 IsNot Nothing Then
                                        action7 = CS$<>8__locals1.$I26
                                    Else
                                        Dim action8 As Action(Of ResourceProject) = Sub(p As ResourceProject)
                                            CS$<>8__locals1.$VB$Local_SearchResults.Add(p)
                                        End Sub
                                        Dim action9 As Action(Of ResourceProject) = action8
                                        CS$<>8__locals1.$I26 = action8
                                        action7 = action9
                                    End If
                                    list13.ForEach(action7)
                                    Logger.Info(String.Format("从 Modrinth 直接获取到了 {0} 个工程", list12.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                    CS$<>8__locals1.$VB$Local_ModrinthSlugs.Clear()
                                Catch ex3 As Exception
                                    Logger.Warn(ex3, "Modrinth 直接获取失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                                    CS$<>8__locals1.$VB$Local_Errors.Add(New ValueTuple(Of Exception, ResourcePlatforms)(ex3, ResourcePlatforms.Modrinth))
                                End Try
                                If CS$<>8__locals1.$VB$Local_Task.Progress < 0.75 Then
                                    Dim $VB$Local_Task3 As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer) = CS$<>8__locals1.$VB$Local_Task
                                    Dim loaderTask3 As ModLoader.LoaderTask(Of ResourceSearcher.SearchRequest, Integer) = $VB$Local_Task3
                                    $VB$Local_Task3.Progress = loaderTask3.Progress + 0.25
                                End If
                            End Sub, "Modrinth Get", ThreadPriority.Normal))
                        End If
                        Try
                            For Each thread As Thread In list4
                                thread.Join()
                                If CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                                    Return
                                End If
                            Next
                        Finally
                            Dim enumerator3 As List(Of Thread).Enumerator
                            CType(enumerator3, IDisposable).Dispose()
                        End Try
                        list3 = New List(Of ResourceProject)(CS$<>8__locals1.$VB$Local_SearchResults)
                        If CS$<>8__locals1.$VB$Local_IgnoreModLoaderFilter Then
                            Dim collection As ICollection(Of ResourceProject) = list3
                            Dim func14 As Func(Of ResourceProject, Boolean)
                            If ResourceSearcher._Closure$__.$I6-27 IsNot Nothing Then
                                func14 = ResourceSearcher._Closure$__.$I6-27
                            Else
                                Dim func15 As Func(Of ResourceProject, Boolean) = Function(p As ResourceProject) p.ModLoaders = ModLoaders.None OrElse p.ModLoaders.HasFlagF(ModLoaders.Forge)
                                func14 = func15
                                ResourceSearcher._Closure$__.$I6-27 = func15
                            End If
                            collection.KeepIf(func14)
                        End If
                        CS$<>8__locals1.$VB$Local_Storage.ErrorMessage = Nothing
                        If Not list3.Any() Then
                            If CS$<>8__locals1.$VB$Local_Errors.Any() Then
                                Throw CS$<>8__locals1.$VB$Local_Errors.First().Item1
                            End If
                            If flag AndAlso CS$<>8__locals1.$VB$Local_Request.Type <> ResourceTypes.[Mod] AndAlso CS$<>8__locals1.$VB$Local_Request.Type <> ResourceTypes.DataPack Then
                                Throw New Exception("无搜索结果，请尝试搜索其英文名称")
                            End If
                            If CS$<>8__locals1.$VB$Local_Request.Sources = ResourcePlatforms.CurseForge AndAlso CS$<>8__locals1.$VB$Local_Request.Tag.StartsWithF("/", False) Then
                                Throw New Exception("CurseForge 不兼容所选的类型")
                            End If
                            If CS$<>8__locals1.$VB$Local_Request.Sources = ResourcePlatforms.Modrinth AndAlso CS$<>8__locals1.$VB$Local_Request.Tag.EndsWithF("/", False) Then
                                Throw New Exception("Modrinth 不兼容所选的类型")
                            End If
                            Throw New Exception("没有搜索结果")
                        ElseIf CS$<>8__locals1.$VB$Local_Errors.Any() Then
                            Dim $VB$Local_Errors As IEnumerable(Of ValueTuple(Of Exception, ResourcePlatforms)) = CS$<>8__locals1.$VB$Local_Errors
                            Dim func16 As Func(Of ValueTuple(Of Exception, ResourcePlatforms), Boolean)
                            If ResourceSearcher._Closure$__.$I6-28 IsNot Nothing Then
                                func16 = ResourceSearcher._Closure$__.$I6-28
                            Else
                                Dim func17 As Func(Of ValueTuple(Of Exception, ResourcePlatforms), Boolean) = Function(<TupleElementNames(New String() { "Ex", "Source" })> e As ValueTuple(Of Exception, ResourcePlatforms)) e.Item2 = ResourcePlatforms.CurseForge
                                func16 = func17
                                ResourceSearcher._Closure$__.$I6-28 = func17
                            End If
                            If $VB$Local_Errors.Any(func16) Then
                                CS$<>8__locals1.$VB$Local_Storage.ErrorMessage = String.Format("无法连接到 CurseForge，所以目前仅显示了来自 Modrinth 的内容，搜索结果可能不全。{0}请稍后再试，或使用 VPN 改善网络环境。", vbCrLf)
                            Else
                                CS$<>8__locals1.$VB$Local_Storage.ErrorMessage = String.Format("无法连接到 Modrinth，所以目前仅显示了来自 CurseForge 的内容，搜索结果可能不全。{0}请稍后再试，或使用 VPN 改善网络环境。", vbCrLf)
                            End If
                        End If
                    Finally
                        Try
                            For Each thread2 As Thread In list4
                                If thread2.IsAlive Then
                                    thread2.Interrupt()
                                End If
                            Next
                        Finally
                            Dim enumerator4 As List(Of Thread).Enumerator
                            CType(enumerator4, IDisposable).Dispose()
                        End Try
                    End Try
                    Dim enumerable6 As IEnumerable(Of ResourceProject) = list3
                    Dim func18 As Func(Of ResourceProject, Boolean)
                    If ResourceSearcher._Closure$__.$I6-29 IsNot Nothing Then
                        func18 = ResourceSearcher._Closure$__.$I6-29
                    Else
                        Dim func19 As Func(Of ResourceProject, Boolean) = Function(x As ResourceProject) x.Platform = ResourcePlatforms.CurseForge
                        func18 = func19
                        ResourceSearcher._Closure$__.$I6-29 = func19
                    End If
                    Dim enumerable7 As IEnumerable(Of ResourceProject) = enumerable6.Where(func18)
                    Dim enumerable8 As IEnumerable(Of ResourceProject) = list3
                    Dim func20 As Func(Of ResourceProject, Boolean)
                    If ResourceSearcher._Closure$__.$I6-30 IsNot Nothing Then
                        func20 = ResourceSearcher._Closure$__.$I6-30
                    Else
                        Dim func21 As Func(Of ResourceProject, Boolean) = Function(x As ResourceProject) x.Platform <> ResourcePlatforms.CurseForge
                        func20 = func21
                        ResourceSearcher._Closure$__.$I6-30 = func21
                    End If
                    list3 = enumerable7.Concat(enumerable8.Where(func20)).ToList()
                    Dim list5 As IList(Of ResourceProject) = list3
                    Dim func22 As Func(Of ResourceProject, ResourceProject, Boolean)
                    If ResourceSearcher._Closure$__.$I6-31 IsNot Nothing Then
                        func22 = ResourceSearcher._Closure$__.$I6-31
                    Else
                        Dim func23 As Func(Of ResourceProject, ResourceProject, Boolean) = Function(a As ResourceProject, b As ResourceProject) a.IsLike(b)
                        func22 = func23
                        ResourceSearcher._Closure$__.$I6-31 = func23
                    End If
                    list3 = list5.Distinct(func22).ToList()
                    list3 = list3.Where(Function(r As ResourceProject)
                        Dim CS$<>8__locals11 As ResourceSearcher._Closure$__6-5 = New ResourceSearcher._Closure$__6-5(CS$<>8__locals11)
                        CS$<>8__locals11.$VB$Local_r = r
                        Return Not CS$<>8__locals1.$VB$Local_RealResults.Any(Function(b As ResourceProject) CS$<>8__locals11.$VB$Local_r.IsLike(b)) AndAlso Not CS$<>8__locals1.$VB$Local_Storage.Results.Any(Function(b As ResourceProject) CS$<>8__locals11.$VB$Local_r.IsLike(b))
                    End Function).ToList()
                    CS$<>8__locals1.$VB$Local_RealResults.AddRange(list3)
                    Logger.Info(String.Format("去重、筛选后累计新增结果 {0} 个（目前已有结果 {1} 个）", CS$<>8__locals1.$VB$Local_RealResults.Count, CS$<>8__locals1.$VB$Local_Storage.Results.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                    If CS$<>8__locals1.$VB$Local_RealResults.Count + CS$<>8__locals1.$VB$Local_Storage.Results.Count >= CS$<>8__locals1.$VB$Local_Request.TargetResultCount Then
                        GoTo IL_0B52
                    End If
                    Logger.Info(String.Format("总结果数需求最少 {0} 个，仅获得了 {1} 个", CS$<>8__locals1.$VB$Local_Request.TargetResultCount, CS$<>8__locals1.$VB$Local_RealResults.Count + CS$<>8__locals1.$VB$Local_Storage.Results.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                    If Not CS$<>8__locals1.$VB$Local_Request.CanContinue OrElse CS$<>8__locals1.$VB$Local_Errors.Any() Then
                        Exit For
                    End If
                    Logger.Info("将继续尝试加载下一页", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                End While
                Logger.Info("无法继续加载，将强制结束", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
                IL_0B52:
                Dim dictionary4 As Dictionary(Of ResourceProject, Double) = New Dictionary(Of ResourceProject, Double)()
                Dim vb$AnonymousDelegate_7 As VB$AnonymousDelegate_4(Of ResourceProject, Double) = Function(Project As ResourceProject)
                    Dim type As ResourceTypes = CS$<>8__locals1.$VB$Local_Request.Type
                    If type <= ResourceTypes.ResourcePack Then
                        If type = ResourceTypes.[Mod] OrElse type = ResourceTypes.ModPack Then
                            Return CDbl(If((Project.Platform = ResourcePlatforms.CurseForge), 1, 5))
                        End If
                        If type <> ResourceTypes.ResourcePack Then
                            GoTo IL_005B
                        End If
                    ElseIf type <> ResourceTypes.Shader Then
                        If type <> ResourceTypes.DataPack Then
                            GoTo IL_005B
                        End If
                        Return CDbl(If((Project.Platform = ResourcePlatforms.CurseForge), 10, 1))
                    End If
                    Return CDbl(If((Project.Platform = ResourcePlatforms.CurseForge), 1, 4))
                    IL_005B:
                    Return 1.0
                End Function
                If String.IsNullOrEmpty(CS$<>8__locals1.$VB$Local_RawSearchText) Then
                    Try
                        For Each resourceProject As ResourceProject In CS$<>8__locals1.$VB$Local_RealResults
                            dictionary4(resourceProject) = CDbl(resourceProject.DownloadCount) * vb$AnonymousDelegate_7(resourceProject)
                        Next
                        GoTo IL_0D62
                    Finally
                        Dim enumerator5 As List(Of ResourceProject).Enumerator
                        CType(enumerator5, IDisposable).Dispose()
                    End Try
                End If
                Dim list6 As List(Of ModBase.SearchEntry(Of ResourceProject)) = New List(Of ModBase.SearchEntry(Of ResourceProject))()
                Try
                    For Each resourceProject2 As ResourceProject In CS$<>8__locals1.$VB$Local_RealResults
                        dictionary4(resourceProject2) = If((resourceProject2.WikiId > 0), 0.2, 0.0) + Math.Log10(CDbl(Math.Max(resourceProject2.DownloadCount, 1)) * vb$AnonymousDelegate_7(resourceProject2)) / 9.0
                        list6.Add(New ModBase.SearchEntry(Of ResourceProject)() With { .Item = resourceProject2, .SearchSource = New List(Of ModBase.SearchSource)() From { New ModBase.SearchSource(If(flag, resourceProject2.TranslatedName, resourceProject2.RawName).Split("/"c, True), 1.0), New ModBase.SearchSource(resourceProject2.Description, 0.05) } })
                    Next
                Finally
                    Dim enumerator6 As List(Of ResourceProject).Enumerator
                    CType(enumerator6, IDisposable).Dispose()
                End Try
                Dim list7 As List(Of ModBase.SearchEntry(Of ResourceProject)) = ModBase.Search(Of ResourceProject)(list6, CS$<>8__locals1.$VB$Local_RawSearchText, 10000, -1.0)
                Try
                    For Each searchEntry3 As ModBase.SearchEntry(Of ResourceProject) In list7
                        Dim dictionary5 As Dictionary(Of ResourceProject, Double) = dictionary4
                        Dim dictionary6 As Dictionary(Of ResourceProject, Double) = dictionary5
                        Dim item As ResourceProject = searchEntry3.Item
                        Dim resourceProject3 As ResourceProject = item
                        dictionary5(item) = dictionary6(resourceProject3) + If(searchEntry3.AbsoluteRight, 10.0, searchEntry3.Similarity) / If(list7.First().AbsoluteRight, 10.0, list7.First().Similarity)
                    Next
                Finally
                    Dim enumerator7 As List(Of ModBase.SearchEntry(Of ResourceProject)).Enumerator
                    CType(enumerator7, IDisposable).Dispose()
                End Try
                IL_0D62:
                If CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                    Throw New OperationCanceledException()
                End If
                Dim results As List(Of ResourceProject) = CS$<>8__locals1.$VB$Local_Storage.Results
                Dim enumerable9 As IEnumerable(Of KeyValuePair(Of ResourceProject, Double)) = dictionary4
                Dim func24 As Func(Of KeyValuePair(Of ResourceProject, Double), Double)
                If ResourceSearcher._Closure$__.$I6-36 IsNot Nothing Then
                    func24 = ResourceSearcher._Closure$__.$I6-36
                Else
                    Dim func25 As Func(Of KeyValuePair(Of ResourceProject, Double), Double) = Function(s As KeyValuePair(Of ResourceProject, Double)) s.Value
                    func24 = func25
                    ResourceSearcher._Closure$__.$I6-36 = func25
                End If
                Dim enumerable10 As IEnumerable(Of KeyValuePair(Of ResourceProject, Double)) = enumerable9.OrderByDescending(func24)
                Dim func26 As Func(Of KeyValuePair(Of ResourceProject, Double), ResourceProject)
                If ResourceSearcher._Closure$__.$I6-37 IsNot Nothing Then
                    func26 = ResourceSearcher._Closure$__.$I6-37
                Else
                    Dim func27 As Func(Of KeyValuePair(Of ResourceProject, Double), ResourceProject) = Function(r As KeyValuePair(Of ResourceProject, Double)) r.Key
                    func26 = func27
                    ResourceSearcher._Closure$__.$I6-37 = func27
                End If
                results.AddRange(enumerable10.[Select](func26))
                Return
            End If
            If Not CS$<>8__locals1.$VB$Local_Storage.Results.Any() Then
                Throw New Exception("没有符合条件的结果")
            End If
            Logger.Info(String.Format("已有 {0} 个结果，少于所需的 {1} 个结果，但无法继续获取，结束处理", CS$<>8__locals1.$VB$Local_Storage.Results.Count, CS$<>8__locals1.$VB$Local_Request.TargetResultCount), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Resource\ResourceSearcher.vb")
        End Sub

        Private Const RESULT_PAGE_SIZE As Integer = 40

        Private Const MSG_NO_CHINESE_RESULT As String = "无搜索结果，请尝试搜索其英文名称"

        Private Shared $STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries As List(Of ModBase.SearchEntry(Of WikiEntry))

        Private Shared $STATIC$Run$011151286FC21283B48$CurseForgeSearchEntries$Init As StaticLocalInitFlag

        Private Shared $STATIC$Run$011151286FC21283B48$ModrinthSearchEntries As List(Of ModBase.SearchEntry(Of WikiEntry))

        Private Shared $STATIC$Run$011151286FC21283B48$ModrinthSearchEntries$Init As StaticLocalInitFlag

        Public Class SearchRequest
            Public ReadOnly Property CanContinue As Boolean
                Get
                    If Me.Tag.StartsWithF("/", False) OrElse Not Me.Sources.HasFlagF(ResourcePlatforms.CurseForge) Then
                        Me.Storage.CurseForgeTotal = 0
                    End If
                    If Me.Tag.EndsWithF("/", False) OrElse Not Me.Sources.HasFlagF(ResourcePlatforms.Modrinth) Then
                        Me.Storage.ModrinthTotal = 0
                    End If
                    Return Me.Storage.CurseForgeTotal = -1 OrElse Me.Storage.ModrinthTotal = -1 OrElse Me.Storage.CurseForgeOffset < Me.Storage.CurseForgeTotal OrElse Me.Storage.ModrinthOffset < Me.Storage.ModrinthTotal
                End Get
            End Property

            Public Sub New(Type As ResourceTypes, Storage As ResourceSearcher.SearchResult, TargetResultCount As Integer)
                Me.Tag = ""
                Me.ModLoaders = ModLoaders.None
                Me.GameVersion = Nothing
                Me.SearchText = Nothing
                Me.Sources = ResourcePlatforms.Any
                Me.Type = Type
                Me.Storage = Storage
                Me.TargetResultCount = TargetResultCount
            End Sub

            Public Function GetCurseForgeAddress(SearchText As String, IgnoreModLoaderFilter As Boolean) As String
                If Me.Tag.StartsWithF("/", False) Then
                    Me.Storage.CurseForgeTotal = 0
                End If
                Dim text As String = String.Format("https://api.curseforge.com/v1/mods/search?gameId=432&sortField=2&sortOrder=desc&pageSize={0}", 40)
                Dim type As ResourceTypes = Me.Type
                Select Case type
                    Case ResourceTypes.[Mod]
                        text += "&classId=6"
                    Case ResourceTypes.ModPack
                        text += "&classId=4471"
                    Case ResourceTypes.[Mod] Or ResourceTypes.ModPack
                    Case ResourceTypes.ResourcePack
                        text += "&classId=12"
                    Case Else
                        If type <> ResourceTypes.Shader Then
                            If type = ResourceTypes.DataPack Then
                                text += "&classId=6945"
                            End If
                        Else
                            text += "&classId=6552"
                        End If
                End Select
                If Operators.CompareString(Me.Tag, "", False) <> 0 Then
                    text = text + "&categoryId=" + Me.Tag.BeforeFirst("/", False)
                End If
                If Me.ModLoaders <> ModLoaders.None AndAlso Not IgnoreModLoaderFilter Then
                    text = text + "&modLoaderType=" + Conversions.ToString(Resource.ToCurseForgeModLoaderType(Me.ModLoaders.Flags().First()))
                End If
                If Not String.IsNullOrEmpty(Me.GameVersion) Then
                    text = text + "&gameVersion=" + Me.GameVersion
                End If
                If Not String.IsNullOrEmpty(SearchText) Then
                    text = text + "&searchFilter=" + StringUtils.UrlEscape(SearchText)
                End If
                If Me.Storage.CurseForgeOffset > 0 Then
                    text = text + "&index=" + Conversions.ToString(Me.Storage.CurseForgeOffset)
                End If
                Return text
            End Function

            Public Function GetModrinthAddress(SearchText As String, IgnoreModLoaderFilter As Boolean) As String
                If Me.Tag.EndsWithF("/", False) Then
                    Me.Storage.ModrinthTotal = 0
                End If
                Dim text As String = String.Format("https://api.modrinth.com/v2/search?limit={0}&index=relevance", 40)
                If Not String.IsNullOrEmpty(SearchText) Then
                    text = text + "&query=" + StringUtils.UrlEscape(SearchText)
                End If
                If Me.Storage.ModrinthOffset > 0 Then
                    text = text + "&offset=" + Conversions.ToString(Me.Storage.ModrinthOffset)
                End If
                Dim list As List(Of String) = New List(Of String)()
                list.Add(String.Format("[""project_type:{0}""]", Me.Type.ToString().Lower()))
                If Not String.IsNullOrEmpty(Me.Tag) Then
                    list.Add(String.Format("[""categories:'{0}'""]", Me.Tag.AfterLast("/", False)))
                End If
                If Me.ModLoaders <> ModLoaders.None AndAlso Not IgnoreModLoaderFilter Then
                    Dim list2 As List(Of String) = list
                    Dim text2 As String = "[""categories:{0}""]"
                    Dim enumerable As IEnumerable(Of ModLoaders) = Me.ModLoaders.Flags()
                    Dim func As Func(Of ModLoaders, String)
                    If ResourceSearcher.SearchRequest._Closure$__.$I12-0 IsNot Nothing Then
                        func = ResourceSearcher.SearchRequest._Closure$__.$I12-0
                    Else
                        Dim func2 As Func(Of ModLoaders, String) = Function(f As ModLoaders) f.ToString().Lower()
                        func = func2
                        ResourceSearcher.SearchRequest._Closure$__.$I12-0 = func2
                    End If
                    list2.Add(String.Format(text2, enumerable.[Select](func).Join(""",""categories:")))
                End If
                If Not String.IsNullOrEmpty(Me.GameVersion) Then
                    list.Add(String.Format("[""versions:'{0}'""]", Me.GameVersion))
                End If
                Return text + "&facets=[" + String.Join(",", list) + "]"
            End Function

            Public Overrides Function Equals(obj As Object) As Boolean
                Dim searchRequest As ResourceSearcher.SearchRequest = TryCast(obj, ResourceSearcher.SearchRequest)
                Return searchRequest IsNot Nothing AndAlso Me.Type = searchRequest.Type AndAlso Me.TargetResultCount = searchRequest.TargetResultCount AndAlso Operators.CompareString(Me.Tag, searchRequest.Tag, False) = 0 AndAlso Me.ModLoaders = searchRequest.ModLoaders AndAlso Me.Sources = searchRequest.Sources AndAlso Operators.CompareString(Me.GameVersion, searchRequest.GameVersion, False) = 0 AndAlso Operators.CompareString(Me.SearchText, searchRequest.SearchText, False) = 0
            End Function

            Public Shared Operator =(left As ResourceSearcher.SearchRequest, right As ResourceSearcher.SearchRequest) As Boolean
                Return EqualityComparer(Of ResourceSearcher.SearchRequest).[Default].Equals(left, right)
            End Operator

            Public Shared Operator <>(left As ResourceSearcher.SearchRequest, right As ResourceSearcher.SearchRequest) As Boolean
                Return Not(left Is right)
            End Operator

            Public Overrides Function GetHashCode() As Integer
                Return New ValueTuple(Of ResourceTypes, String, ModLoaders, String, String, ResourcePlatforms)(Me.Type, Me.Tag, Me.ModLoaders, Me.GameVersion, Me.SearchText, Me.Sources).GetHashCode()
            End Function

            Public Storage As ResourceSearcher.SearchResult

            Public TargetResultCount As Integer

            Public Type As ResourceTypes

            Public Tag As String

            Public ModLoaders As ModLoaders

            Public GameVersion As String

            Public SearchText As String

            Public Sources As ResourcePlatforms
        End Class

        Public Class SearchResult
            Public Sub New()
                Me.CurseForgeOffset = 0
                Me.CurseForgeTotal = -1
                Me.ModrinthOffset = 0
                Me.ModrinthTotal = -1
                Me.Results = New List(Of ResourceProject)()
                Me.ErrorMessage = Nothing
            End Sub

            Public CurseForgeOffset As Integer

            Public CurseForgeTotal As Integer

            Public ModrinthOffset As Integer

            Public ModrinthTotal As Integer

            Public Results As List(Of ResourceProject)

            Public ErrorMessage As String
        End Class
    End Class
End Namespace
