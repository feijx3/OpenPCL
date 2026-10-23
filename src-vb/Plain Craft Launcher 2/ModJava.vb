Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public NotInheritable Module ModJava
        Public Sub JavaInit()
            Try
                If Configs.JavaConfigVersion.[Get]() = 0 Then
                    Try
                        Logger.Warn("正在从老版本迁移 Java 列表", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        Dim enumerable As IEnumerable(Of JToken) = Settings.[Get](Of String)("LaunchArgumentJavaAll", Nothing).DeserializeJson()
                        Dim func As Func(Of JToken, Java)
                        If ModJava._Closure$__.$I1-0 IsNot Nothing Then
                            func = ModJava._Closure$__.$I1-0
                        Else
                            Dim func2 As Func(Of JToken, Java) = Function(JavaObject As JToken) New Java(JavaObject("Path").ToString(), Nothing)
                            func = func2
                            ModJava._Closure$__.$I1-0 = func2
                        End If
                        Dim list As List(Of Java) = enumerable.[Select](func).ToList()
                        list = JavaUtils.SortAsync(list, Nothing, Nothing).Run()
                        Logger.Info(String.Format("已从老版本设置中发现 {0} 个 Java", list.Count), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        Dim text As String = Settings.[Get](Of String)("LaunchArgumentJavaSelect", Nothing)
                        Dim SelectedJava As String = If(text.StartsWithF("{", False), text.DeserializeJson()("Path").ToString(), "")
                        Dim java As Java = list.FirstOrDefault(Function(j As Java) PathUtils.IsSame(j.Folder, SelectedJava))
                        If java IsNot Nothing Then
                            list.Remove(java)
                            list.Insert(0, java)
                            Logger.Info(String.Format("已将老版本设置中强制选择的 Java 置顶：{0}", java), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        End If
                        Configs.JavaList.[Set](New ConcurrentList(Of Java)(list))
                        Configs.JavaConfigVersion.[Set](1)
                    Catch ex As Exception
                        Logger.[Error](ex, "从老版本 PCL 迁移 Java 列表失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                    End Try
                End If
                Dim concurrentList As ConcurrentList(Of Java) = Configs.JavaList.[Get]()
                If concurrentList.Any() Then
                    Dim text2 As String = "共有 {0} 个可用的 Java：{1}- {2}"
                    Dim obj As Object = concurrentList.Count
                    Dim obj2 As Object = vbCrLf
                    Dim enumerable2 As IEnumerable(Of Java) = concurrentList
                    Dim func3 As Func(Of Java, String)
                    If ModJava._Closure$__.$I1-2 IsNot Nothing Then
                        func3 = ModJava._Closure$__.$I1-2
                    Else
                        Dim func4 As Func(Of Java, String) = Function(j As Java) j.ToString()
                        func3 = func4
                        ModJava._Closure$__.$I1-2 = func4
                    End If
                    Logger.Info(String.Format(text2, obj, obj2, enumerable2.[Select](func3).Join(vbCrLf & "- ")), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                Else
                    Logger.Warn("初始化未找到可用的 Java，将自动触发搜索", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                    ModJava.JavaListRefreshWorker.Start(Nothing, Nothing)
                End If
            Catch ex2 As Exception
                Logger.[Error](ex2, "初始化 Java 列表失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                Configs.JavaList.Reset()
            End Try
        End Sub

        Public Property JavaListRefreshWorker As RedoableWorker
            <CompilerGenerated()>
            Get
                Return ModJava._JavaListRefreshWorker
            End Get
            <CompilerGenerated()>
            <MethodImpl(MethodImplOptions.Synchronized)>
            Set(value As RedoableWorker)
                Dim action As Action = AddressOf ModJava.UpdateJavaLists
                Dim action2 As Action = AddressOf ModJava.UpdateJavaLists
                Dim redoableWorker As RedoableWorker = ModJava._JavaListRefreshWorker
                If redoableWorker IsNot Nothing Then
                    RemoveHandler redoableWorker.Started, action
                    RemoveHandler redoableWorker.Stopped, action2
                End If
                ModJava._JavaListRefreshWorker = value
                redoableWorker = ModJava._JavaListRefreshWorker
                If redoableWorker IsNot Nothing Then
                    AddHandler redoableWorker.Started, action
                    AddHandler redoableWorker.Stopped, action2
                End If
            End Set
        End Property = New RedoableWorker(Sub(c As CancellationToken?, p As ProgressProvider)
            JavaUtils.RefreshListAsync(c.Value, p).Run()
        End Sub, "JavaListRefreshWorker")

        Public Sub UpdateJavaLists()
            Dim action As Action
            If ModJava._Closure$__.$I6-0 IsNot Nothing Then
                action = ModJava._Closure$__.$I6-0
            Else
                Dim action2 As Action = Sub()
                    Dim frmSetupLaunch As PageSetupLaunch = ModMain.FrmSetupLaunch
                    If frmSetupLaunch IsNot Nothing Then
                        frmSetupLaunch.UpdateJavaList()
                    End If
                    Dim frmInstanceSetup As PageInstanceSetup = ModMain.FrmInstanceSetup
                    If frmInstanceSetup Is Nothing Then
                        Return
                    End If
                    frmInstanceSetup.UpdateJavaList()
                End Sub
                action = action2
                ModJava._Closure$__.$I6-0 = action2
            End If
            ModBase.RunInUi(action, False)
        End Sub

        Public Sub ManuallyImportJava()
            Dim text As String = Dialogs.SelectFile("选择 java.exe（可能在 bin 文件夹中）", False, Nothing, New OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)?("java.exe|java.exe")).FirstOrDefault()
            If Not String.IsNullOrEmpty(text) Then
                Dim NewJava As Java = New Java(PathUtils.RemoveLastPart(text), Nothing)
                If Not NewJava.CheckAsync(Nothing).Run() Then
                    ModMain.Hint("该 Java 存在异常，无法使用", ModMain.HintType.Red, True)
                    Return
                End If
                Dim $I1 As Func(Of String, Boolean)
                Configs.JavaRemovedList.Edit(Sub(ByRef List As List(Of String))
                    Dim collection As ICollection(Of String) = List
                    Dim func As Func(Of String, Boolean)
                    If $I1 IsNot Nothing Then
                        func = $I1
                    Else
                        Dim func2 As Func(Of String, Boolean) = Function(f As String) PathUtils.IsSame(f, NewJava.Folder)
                        Dim func3 As Func(Of String, Boolean) = func2
                        $I1 = func2
                        func = func3
                    End If
                    collection.RemoveIf(func)
                End Sub)
                Configs.JavaList.Edit(Sub(ByRef List As ConcurrentList(Of Java))
                    List.Remove(NewJava)
                    List.Insert(0, NewJava)
                End Sub)
                ModMain.Hint(String.Format("已将 Java 加入列表：{0}", text), ModMain.HintType.Green, True)
                ModJava.UpdateJavaLists()
            End If
        End Sub

        Public Sub ManuallyRemoveJava(JavaEntry As Java)
            Configs.JavaList.Edit(Sub(ByRef List As ConcurrentList(Of Java))
                List.Remove(JavaEntry)
            End Sub)
            Configs.JavaRemovedList.Edit(Function(List As List(Of String))
                Dim enumerable As IEnumerable(Of String) = List.Append(JavaEntry.Folder)
                Dim func As Func(Of String, String)
                If ModJava._Closure$__.$I8-2 IsNot Nothing Then
                    func = ModJava._Closure$__.$I8-2
                Else
                    Dim func2 As Func(Of String, String) = Function(s As String) s.Lower()
                    func = func2
                    ModJava._Closure$__.$I8-2 = func2
                End If
                Return enumerable.DistinctBy(func).ToList()
            End Function)
            ModMain.Hint(String.Format("已将 Java 移除出列表：{0}", JavaEntry), ModMain.HintType.Green, True)
            ModJava.UpdateJavaLists()
            PageInstanceLeft.ReloadCurrentJava()
        End Sub

        Public Sub ManuallySearchJava(Optional CancellationToken As CancellationToken = Nothing)
            ModBase.RunInThread(Sub()
                Dim enumerable As IEnumerable(Of Java) = Configs.JavaList.[Get]()
                Dim action As Action(Of Java)
                If ModJava._Closure$__.$I9-1 IsNot Nothing Then
                    action = ModJava._Closure$__.$I9-1
                Else
                    Dim action2 As Action(Of Java) = Sub(java As Java)
                        java.InvalidateChecked()
                    End Sub
                    action = action2
                    ModJava._Closure$__.$I9-1 = action2
                End If
                enumerable.ForAll(action)
                ModJava.JavaListRefreshWorker.Start(CancellationToken, Nothing)
                ModJava.JavaListRefreshWorker.WaitIfRunningAsync(-1, CancellationToken).Run()
                If Configs.JavaList.[Get]().Any() Then
                    ModMain.Hint("已找到 " + Conversions.ToString(Configs.JavaList.[Get]().Count) + " 个 Java！", ModMain.HintType.Green, True)
                    Return
                End If
                ModMain.Hint("未找到可用的 Java！", ModMain.HintType.Red, True)
            End Sub)
        End Sub

        Public Function GetJavaRequirement(Instance As ModMinecraft.McInstance) As <TupleElementNames(New String() { "Range", "RecommendedComponent" })> ValueTuple(Of ValueRange(Of Version), String)
            If Settings.[Get](Of Integer)("VersionArgumentJavaV2", Instance) = 1 Then
                Try
                    Dim text As String = Settings.[Get](Of String)("VersionArgumentJavaRange", Instance)
                    Dim func As Func(Of String, Version)
                    If ModJava._Closure$__.$I10-0 IsNot Nothing Then
                        func = ModJava._Closure$__.$I10-0
                    Else
                        Dim func2 As Func(Of String, Version) = Function(s As String) StringUtils.ParseVersionWithDefaults(s)
                        func = func2
                        ModJava._Closure$__.$I10-0 = func2
                    End If
                    Dim valueRange As ValueRange(Of Version) = ValueRange(Of Version).FromString(text, func)
                    Return New ValueTuple(Of ValueRange(Of Version), String)(valueRange, Nothing)
                Catch ex As Exception
                    Logger.Warn(String.Format("版本设置中指定的 Java 版本范围有误（{0}），这可能是因为首次修改此设置，尚未初始化", Settings.[Get](Of String)("VersionArgumentJavaRange", Instance)), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                End Try
            End If
            Dim JavaRange As ValueRange(Of Version) = ValueRange(Of Version).All()
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_5(Of ValueRange(Of Version)) = Sub(Constraint As ValueRange(Of Version))
                Dim $VB$Local_JavaRange As ValueRange(Of Version) = JavaRange
                JavaRange = If(($VB$Local_JavaRange IsNot Nothing), $VB$Local_JavaRange.Intersect(Constraint), Nothing)
                If JavaRange.IsEmpty() Then
                    Logger.Warn(String.Format("Java 版本要求冲突，若启动失败请在版本设置手动指定 Java：当前追加的要求为 {0}，游戏版本为 {1}", Constraint, Instance.VersionDisplayName()), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                    JavaRange = Constraint
                End If
            End Sub
            If(Not Instance.Version.Valid AndAlso DateTime.Compare(Instance.ReleaseTime, New DateTime(2024, 4, 2)) >= 0) OrElse (Instance.Version.Valid AndAlso Instance.Version.Vanilla >= New Version(20, 0, 5)) Then
                vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(21, 0)))
            ElseIf(Not Instance.Version.Valid AndAlso DateTime.Compare(Instance.ReleaseTime, New DateTime(2021, 11, 16)) >= 0) OrElse (Instance.Version.Valid AndAlso Instance.Version.Vanilla.Major >= 18) Then
                vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(17, 0)))
            ElseIf(Not Instance.Version.Valid AndAlso DateTime.Compare(Instance.ReleaseTime, New DateTime(2021, 5, 11)) >= 0) OrElse (Instance.Version.Valid AndAlso Instance.Version.Vanilla.Major >= 17) Then
                vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(16, 0)))
            ElseIf Instance.ReleaseTime.Year >= 2017 Then
                vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(8, 0)))
            ElseIf DateTime.Compare(Instance.ReleaseTime, New DateTime(2013, 5, 1)) <= 0 AndAlso Instance.ReleaseTime.Year >= 2001 Then
                vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(9, 0)))
            End If
            Dim text2 As String = Nothing
            Dim jsonObject As JObject = Instance.JsonObject
            Dim obj As Object
            If jsonObject Is Nothing Then
                obj = Nothing
            Else
                Dim jtoken As JToken = jsonObject("javaVersion")
                If jtoken Is Nothing Then
                    obj = Nothing
                Else
                    Dim jtoken2 As JToken = jtoken("majorVersion")
                    obj = If((jtoken2 IsNot Nothing), New Integer?(jtoken2.ToObject(Of Integer)()), Nothing)
                End If
            End If
            Dim obj2 As Object = obj
            Dim num As Integer? = obj2
            Dim num2 As Integer? = obj2
            Dim num4 As Integer
            If num2 Is Nothing Then
                Dim jsonVersion As JObject = Instance.JsonVersion
                Dim num3 As Integer?
                If jsonVersion Is Nothing Then
                    num3 = Nothing
                Else
                    Dim jtoken3 As JToken = jsonVersion("java_version")
                    num3 = If((jtoken3 IsNot Nothing), New Integer?(jtoken3.ToObject(Of Integer)()), Nothing)
                End If
                num2 = num3
                num4 = num2.GetValueOrDefault()
            Else
                num4 = num.GetValueOrDefault()
            End If
            Dim num5 As Integer = num4
            If num5 >= 22 Then
                Logger.Info("Mojang 要求至少使用 Java " + Conversions.ToString(num5), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(num5, 0)))
                Dim jsonObject2 As JObject = Instance.JsonObject
                Dim obj3 As Object
                If jsonObject2 Is Nothing Then
                    obj3 = Nothing
                Else
                    Dim jtoken4 As JToken = jsonObject2("javaVersion")
                    If jtoken4 Is Nothing Then
                        obj3 = Nothing
                    Else
                        Dim jtoken5 As JToken = jtoken4("component")
                        obj3 = If((jtoken5 IsNot Nothing), jtoken5.ToString(), Nothing)
                    End If
                End If
                Dim obj4 As Object = obj3
                Dim text3 As String = obj4
                If obj4 Is Nothing Then
                    Dim jsonVersion2 As JObject = Instance.JsonVersion
                    If jsonVersion2 Is Nothing Then
                        text3 = Nothing
                    Else
                        Dim jtoken6 As JToken = jsonVersion2("java_component")
                        text3 = If((jtoken6 IsNot Nothing), jtoken6.ToString(), Nothing)
                    End If
                End If
                text2 = text3
                If Operators.CompareString(text2, "", False) = 0 Then
                    text2 = Nothing
                End If
            End If
            If Instance.Version.HasOptiFine AndAlso Instance.Version.Valid Then
                If Instance.Version.Vanilla.Major < 7 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(9, 0)))
                ElseIf Instance.Version.Vanilla.Major >= 8 AndAlso Instance.Version.Vanilla.Major < 12 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).ClosedOpen(New Version(8, 0), New Version(9, 0)))
                ElseIf Instance.Version.Vanilla.Major = 12 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(9, 0)))
                End If
            End If
            If Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) Then
                If Instance.Version.Vanilla >= New Version(6, 0, 1) AndAlso Instance.Version.Vanilla <= New Version(7, 0, 2) Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).ClosedOpen(New Version(7, 0), New Version(8, 0)))
                ElseIf Instance.Version.Vanilla.Major <= 12 OrElse Not Instance.Version.Valid Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(9, 0)))
                ElseIf Instance.Version.Vanilla.Major <= 14 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).ClosedOpen(New Version(8, 0), New Version(11, 0)))
                ElseIf Instance.Version.Vanilla.Major = 15 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).ClosedOpen(New Version(8, 0), New Version(16, 0)))
                ElseIf McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions(ModLoaders.Forge), "34.0.0") AndAlso McUtils.CompareVersionGE("36.2.25", Instance.Version.ModLoaderVersions(ModLoaders.Forge)) Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).AtMost(New Version(8, 0, 320)))
                ElseIf McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions(ModLoaders.Forge), "36.2.26") AndAlso McUtils.CompareVersionGE("36.999999.999999", Instance.Version.ModLoaderVersions(ModLoaders.Forge)) Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(24, 0)))
                ElseIf McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions(ModLoaders.Forge), "37.0.0") AndAlso McUtils.CompareVersionGE("37.0.79", Instance.Version.ModLoaderVersions(ModLoaders.Forge)) Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(17, 0)))
                ElseIf Instance.Version.Vanilla.Major = 18 AndAlso Instance.Version.HasOptiFine Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(19, 0)))
                ElseIf McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions(ModLoaders.Forge), "45.0.21") AndAlso McUtils.CompareVersionGE("45.0.65", Instance.Version.ModLoaderVersions(ModLoaders.Forge)) Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(20, 0)))
                ElseIf McUtils.CompareVersionGE(Instance.Version.ModLoaderVersions(ModLoaders.Forge), "45.0.66") AndAlso McUtils.CompareVersionGE("47.4.8", Instance.Version.ModLoaderVersions(ModLoaders.Forge)) Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(22, 0)))
                End If
            End If
            If Instance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) AndAlso (Instance.Version.Vanilla Is New Version(20, 0, 1) OrElse (McUtils.CompareVersionGE("20.2.62-beta", Instance.Version.ModLoaderVersions(ModLoaders.NeoForge)) AndAlso Not Instance.Version.ModLoaderVersions(ModLoaders.NeoForge).Contains("25w14craftmine"))) Then
                vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(22, 0)))
            End If
            If Instance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) AndAlso Instance.Version.Valid Then
                If Instance.Version.Vanilla.Major >= 15 AndAlso Instance.Version.Vanilla.Major <= 16 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(8, 0)))
                ElseIf Instance.Version.Vanilla.Major >= 18 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(17, 0)))
                End If
                If Operators.CompareString(Instance.Version.ModLoaderVersions(ModLoaders.Fabric), "未知版本", False) <> 0 AndAlso McUtils.CompareVersion(Instance.Version.ModLoaderVersions(ModLoaders.Fabric), "0.17.0") < 0 Then
                    vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(25, 0)))
                End If
            End If
            If Instance.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) AndAlso Instance.Version.Valid Then
                vb$AnonymousDelegate_(ValueRange(Of Version).LessThan(New Version(9, 0)))
            End If
            If Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing) = ModLaunch.McLoginType.Nide Then
                vb$AnonymousDelegate_(ValueRange(Of Version).AtLeast(New Version(8, 0, 141)))
            End If
            Dim valueTuple As ValueTuple(Of ValueRange(Of Version), String) = New ValueTuple(Of ValueRange(Of Version), String)(JavaRange, text2)
            Return valueTuple
        End Function

        Public Function SelectOrDownloadJava(Target As OneOf(Of ValueRange(Of Version), ModMinecraft.McInstance), TryFixOrHint As Boolean, Optional c As CancellationToken = Nothing, Optional p As ProgressProvider = Nothing) As Java
            Dim CS$<>8__locals1 As ModJava._Closure$__11-0 = New ModJava._Closure$__11-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_c = c
            Dim java2 As Java
            Try
                If Target.[Is](Of ModMinecraft.McInstance)() Then
                    ModJava.MigrateInstanceJavaSettings(Target.[As](Of ModMinecraft.McInstance)(), CS$<>8__locals1.$VB$Local_c, If((p IsNot Nothing), p.SplitTo(0.2), Nothing))
                End If
                Dim mcInstance As ModMinecraft.McInstance = If(Target.[Is](Of ModMinecraft.McInstance)(), Target.[As](Of ModMinecraft.McInstance)(), Nothing)
                Dim num As Integer = If((mcInstance Is Nothing), 0, Settings.[Get](Of Integer)("VersionArgumentJavaV2", mcInstance))
                If num <> 2 Then
                    If num <> 3 Then
                        Dim CS$<>8__locals2 As ModJava._Closure$__11-1 = New ModJava._Closure$__11-1(CS$<>8__locals2)
                        CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                        CS$<>8__locals2.$VB$Local_Range = If(Target.[Is](Of ModMinecraft.McInstance)(), ModJava.GetJavaRequirement(mcInstance).Item1, Target.[As](Of ValueRange(Of Version))())
                        CS$<>8__locals2.$VB$Local_RecommendedComponent = If(Target.[Is](Of ModMinecraft.McInstance)(), ModJava.GetJavaRequirement(mcInstance).Item2, Nothing)
                        Logger.Info(String.Format("自动选择以下范围的 Java：{0} {1}", CS$<>8__locals2.$VB$Local_Range, CS$<>8__locals2.$VB$Local_RecommendedComponent), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        If CS$<>8__locals2.$VB$Local_Range.IsEmpty() Then
                            Throw New Exception("Java 版本需求存在冲突，导致没有任何可能适配的 Java，请查看 PCL 的日志了解详细信息！")
                        End If
                        Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_0(Of Java) = Function()
                            ModJava.JavaListRefreshWorker.WaitIfRunning(-1, Nothing)
                            Try
                                For Each java5 As Java In Configs.JavaList.[Get]()
                                    If Not java5.CheckAsync(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c).Run() Then
                                        Logger.Warn(String.Format("Java 检查失败，将尝试列表中的下一个 Java：{0}", java5), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                                    ElseIf java5.Version IsNot Nothing AndAlso CS$<>8__locals2.$VB$Local_Range.Contains(java5.Version) Then
                                        Logger.Info(String.Format("已选定 Java：{0}", java5), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                                        Return java5
                                    End If
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of Java)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                            Return Nothing
                        End Function
                        If p IsNot Nothing Then
                            p.[Set](0.25, False, ProgressProvider.ChildrenAction.Finished)
                        End If
                        Dim java As Java = vb$AnonymousDelegate_()
                        If java IsNot Nothing Then
                            If p IsNot Nothing Then
                                p.Skip(ProgressProvider.ChildrenAction.Skiped)
                            End If
                            java2 = java
                        Else
                            Dim javaLock As Object = ModJava.JavaLock
                            ObjectFlowControl.CheckForSyncLockOnValueType(javaLock)
                            SyncLock javaLock
                                Logger.Warn("既有列表中没有合适的 Java，重新触发搜索", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                                ModJava.JavaListRefreshWorker.Start(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c, If((p IsNot Nothing), p.SplitTo(0.3), Nothing))
                                java = vb$AnonymousDelegate_()
                                If java IsNot Nothing Then
                                    If p IsNot Nothing Then
                                        p.Skip(ProgressProvider.ChildrenAction.Skiped)
                                    End If
                                    java2 = java
                                ElseIf Not TryFixOrHint Then
                                    java2 = Nothing
                                Else
                                    Logger.Warn("电脑中没有合适的 Java，开始获取 Mojang 提供的 Java 下载列表", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                                    If ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init Is Nothing Then
                                        Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init, New StaticLocalInitFlag(), Nothing)
                                    End If
                                    Dim flag2 As Boolean = False
                                    Try
                                        Monitor.Enter(ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init, flag2)
                                        If ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State = 0S Then
                                            ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State = 2S
                                            Dim func As Func(Of CancellationToken?, List(Of ModJava.JavaDownloadInfo))
                                            If ModJava._Closure$__.$I11-1 IsNot Nothing Then
                                                func = ModJava._Closure$__.$I11-1
                                            Else
                                                Dim func2 As Func(Of CancellationToken?, List(Of ModJava.JavaDownloadInfo)) = Function(cancellationToken As CancellationToken?)
                                                    Dim text2 As String = ModNet.NetRequestByLoader(ModDownload.DlVersionListOrder(New String() { "https://piston-meta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json" }, New String() { "https://bmclapi2.bangbang93.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json" }), 45000, True, False)
                                                    If cancellationToken IsNot Nothing Then
                                                        cancellationToken.GetValueOrDefault().ThrowIfCancellationRequested()
                                                    End If
                                                    Dim list As List(Of ModJava.JavaDownloadInfo) = New List(Of ModJava.JavaDownloadInfo)()
                                                    Try
                                                        For Each jtoken As JToken In text2.DeserializeJson()("windows-x64")
                                                            Dim jproperty As JProperty = CType(jtoken, JProperty)
                                                            If cancellationToken IsNot Nothing Then
                                                                cancellationToken.GetValueOrDefault().ThrowIfCancellationRequested()
                                                            End If
                                                            If Operators.CompareString(jproperty.Name, "minecraft-java-exe", False) <> 0 Then
                                                                Try
                                                                    Dim jtoken2 As JToken = jproperty.Value.FirstOrDefault()
                                                                    If jtoken2 IsNot Nothing Then
                                                                        Dim text3 As String = jtoken2("version")("name").ToString()
                                                                        If text3.Contains(".") Then
                                                                            Dim array As String() = text3.Split(".", False)
                                                                            list.Add(New ModJava.JavaDownloadInfo() With { .ComponentName = jproperty.Name, .Java = CType(jtoken2, JObject), .JavaVersion = New Version(CInt(Math.Round(ModBase.Val(array(0)))), CInt(Math.Round(ModBase.Val(array(1)))), CInt(Math.Round(If((array.Length >= 3), ModBase.Val(array(2)), 0.0))), CInt(Math.Round(If((array.Length >= 4), ModBase.Val(array(3)), 0.0)))) })
                                                                        ElseIf text3.StartsWithF("8u51-", False) Then
                                                                            list.Add(New ModJava.JavaDownloadInfo() With { .ComponentName = jproperty.Name, .Java = CType(jtoken2, JObject), .JavaVersion = New Version(8, 0, 51, 0) })
                                                                        Else
                                                                            list.Add(New ModJava.JavaDownloadInfo() With { .ComponentName = jproperty.Name, .Java = CType(jtoken2, JObject), .JavaVersion = New Version(CInt(Math.Round(ModBase.Val(text3.BeforeFirstOfAny(New String() { "u", "." }, False)))), 0, 0, 0) })
                                                                        End If
                                                                    End If
                                                                Catch ex2 As Exception
                                                                    Logger.Warn(ex2, String.Format("解析 Mojang 提供的 Java 列表项失败，已跳过（{0}）", jproperty.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                                                                End Try
                                                            End If
                                                        Next
                                                    Finally
                                                        Dim enumerator2 As IEnumerator(Of JToken)
                                                        If enumerator2 IsNot Nothing Then
                                                            enumerator2.Dispose()
                                                        End If
                                                    End Try
                                                    Return list
                                                End Function
                                                func = func2
                                                ModJava._Closure$__.$I11-1 = func2
                                            End If
                                            ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker = New RedoableWorker(Of List(Of ModJava.JavaDownloadInfo))(func, "SelectOrDownloadJava")
                                        ElseIf ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State = 2S Then
                                            Throw New IncompleteInitialization()
                                        End If
                                    Finally
                                        ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init.State = 1S
                                        If flag2 Then
                                            Monitor.[Exit](ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init)
                                        End If
                                    End Try
                                    If Not ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.HasSucceeded Then
                                        ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.Start(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c, If((p IsNot Nothing), p.SplitTo(0.4), Nothing))
                                    End If
                                    ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.WaitIfRunning(-1, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c)
                                    If p IsNot Nothing Then
                                        p.[Set](0.4, True, ProgressProvider.ChildrenAction.Finished)
                                    End If
                                    Dim javaDownloadInfo As ModJava.JavaDownloadInfo = ModJava.$STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker.LastResult.FirstOrDefault(New Func(Of ModJava.JavaDownloadInfo, Boolean)() { Function(Comp As ModJava.JavaDownloadInfo) Operators.CompareString(Comp.Java("version")("name").ToString(), CS$<>8__locals2.$VB$Local_RecommendedComponent, False) = 0, Function(Comp As ModJava.JavaDownloadInfo) CS$<>8__locals2.$VB$Local_Range.Contains(Comp.JavaVersion) })
                                    If javaDownloadInfo Is Nothing Then
                                        Logger.Warn("Mojang 提供的 Java 列表中没有合适的 Java，无法自动下载", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                                        Dim flag3 As Boolean = CS$<>8__locals2.$VB$Local_Range.HasLower AndAlso CS$<>8__locals2.$VB$Local_Range.IsLowerInclusive AndAlso CS$<>8__locals2.$VB$Local_Range.Lower.Minor <= 0 AndAlso CS$<>8__locals2.$VB$Local_Range.Lower.Build <= 0 AndAlso CS$<>8__locals2.$VB$Local_Range.Lower.Revision <= 0
                                        Dim flag4 As Boolean = CS$<>8__locals2.$VB$Local_Range.HasUpper AndAlso Not CS$<>8__locals2.$VB$Local_Range.IsUpperInclusive AndAlso CS$<>8__locals2.$VB$Local_Range.Upper.Minor <= 0 AndAlso CS$<>8__locals2.$VB$Local_Range.Upper.Build <= 0 AndAlso CS$<>8__locals2.$VB$Local_Range.Upper.Revision <= 0
                                        Dim text As String
                                        If CS$<>8__locals2.$VB$Local_Range.HasLower AndAlso CS$<>8__locals2.$VB$Local_Range.HasUpper Then
                                            If flag3 AndAlso flag4 Then
                                                If CS$<>8__locals2.$VB$Local_Range.Lower.Major = CS$<>8__locals2.$VB$Local_Range.Upper.Major - 1 Then
                                                    text = String.Format(" Java {0}", CS$<>8__locals2.$VB$Local_Range.Lower.Major)
                                                Else
                                                    text = String.Format(" Java {0} ~ {1}", CS$<>8__locals2.$VB$Local_Range.Lower.Major, CS$<>8__locals2.$VB$Local_Range.Upper.Major - 1)
                                                End If
                                            Else
                                                text = String.Format(" Java {0}", CS$<>8__locals2.$VB$Local_Range)
                                            End If
                                        ElseIf CS$<>8__locals2.$VB$Local_Range.HasLower Then
                                            If flag3 Then
                                                text = String.Format(" Java {0} 或更高版本的 Java", CS$<>8__locals2.$VB$Local_Range.Lower.Major)
                                            ElseIf CS$<>8__locals2.$VB$Local_Range.IsLowerInclusive Then
                                                text = String.Format(" Java {0} 或更高版本的 Java", CS$<>8__locals2.$VB$Local_Range.Lower)
                                            Else
                                                text = String.Format("版本高于 {0} 的 Java", CS$<>8__locals2.$VB$Local_Range.Lower)
                                            End If
                                        ElseIf flag4 Then
                                            text = String.Format(" Java {0} 或更低版本的 Java", CS$<>8__locals2.$VB$Local_Range.Upper.Major - 1)
                                        ElseIf CS$<>8__locals2.$VB$Local_Range.IsUpperInclusive Then
                                            text = String.Format(" Java {0} 或更低版本的 Java", CS$<>8__locals2.$VB$Local_Range.Upper)
                                        Else
                                            text = String.Format("版本低于 {0} 的 Java", CS$<>8__locals2.$VB$Local_Range.Upper)
                                        End If
                                        ModMain.MyMsgBox(String.Format("PCL 没能找到合适的 Java。{0}请在网上搜索并手动安装{1}， 然后在【设置 → 启动 → 高级选项 → Java 列表】中导入它。", vbCrLf, text), "需要手动安装 Java", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                                        java2 = Nothing
                                    Else
                                        Dim loaderCombo As ModLoader.LoaderCombo(Of Integer) = ModJava.CreateJavaDownloadLoader(javaDownloadInfo)
                                        Try
                                            loaderCombo.WaitForExit(Nothing, Nothing, True, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_c, If((p IsNot Nothing), p.SplitTo(0.95), Nothing))
                                        Finally
                                            loaderCombo.Cancel()
                                        End Try
                                        java = vb$AnonymousDelegate_()
                                        If java Is Nothing Then
                                            Throw New Exception(String.Format("下载 Java {0} 后仍然无法找到合适的 Java", javaDownloadInfo.ComponentName))
                                        End If
                                        If p IsNot Nothing Then
                                            p.Finish(ProgressProvider.ChildrenAction.Finished)
                                        End If
                                        java2 = java
                                    End If
                                End If
                            End SyncLock
                        End If
                    Else
                        Dim java3 As Java = Configs.JavaForced.[Get](mcInstance.Config)
                        Logger.Info(String.Format("版本设置中强制指定的 Java：{0} → {1}", mcInstance.PathVersion, java3), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        If java3 Is Nothing Then
                            If TryFixOrHint Then
                                ModJava.SwitchToInstanceSetup(mcInstance)
                                ModMain.MyMsgBox(String.Format("该版本的版本设置选择了 {0}使用指定的 Java{1}，但还没有指定任何 Java。{2}请先选择你想使用的 Java，然后再试。", ModBase.vbLQ, ModBase.vbRQ, vbCrLf), "未选择 Java", "确定", "", "", True, True, True, Nothing, Nothing, Nothing)
                            End If
                            If p IsNot Nothing Then
                                p.Skip(ProgressProvider.ChildrenAction.Skiped)
                            End If
                            java2 = Nothing
                        ElseIf Not Configs.JavaList.[Get]().Contains(java3) Then
                            Configs.JavaForced.Reset(mcInstance.Config)
                            ModJava.UpdateJavaLists()
                            If TryFixOrHint Then
                                ModJava.SwitchToInstanceSetup(mcInstance)
                                Logger.[Error]("版本设置中指定的 Java 不在 Java 列表中，请重新选择一个 Java！", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                            End If
                            If p IsNot Nothing Then
                                p.Skip(ProgressProvider.ChildrenAction.Skiped)
                            End If
                            java2 = Nothing
                        ElseIf Not java3.CheckAsync(CS$<>8__locals1.$VB$Local_c).Run() Then
                            Configs.JavaForced.Reset(mcInstance.Config)
                            ModJava.UpdateJavaLists()
                            If TryFixOrHint Then
                                ModJava.SwitchToInstanceSetup(mcInstance)
                                Logger.[Error]("版本设置中指定的 Java 已无法使用，请重新选择一个 Java，然后再试。", LogBehavior.Alert, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                            Else
                                Logger.[Error]("版本设置中指定的 Java 已无法使用，请重新选择一个 Java！", LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                            End If
                            If p IsNot Nothing Then
                                p.Finish(ProgressProvider.ChildrenAction.Finished)
                            End If
                            java2 = Nothing
                        Else
                            If p IsNot Nothing Then
                                p.Finish(ProgressProvider.ChildrenAction.Finished)
                            End If
                            java2 = java3
                        End If
                    End If
                Else
                    Logger.Info(String.Format("版本设置要求使用版本文件夹中的 Java：{0}", mcInstance.PathVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                    Dim java4 As Java = JavaUtils.SearchFoldersAsync(True, New String() { mcInstance.PathVersion }, CS$<>8__locals1.$VB$Local_c, If((p IsNot Nothing), p.SplitTo(1.0), Nothing)).Run().FirstOrDefault()
                    If java4 IsNot Nothing Then
                        Logger.Info(String.Format("已发现版本文件夹中的 Java：{0}", java4), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        java2 = java4
                    Else
                        If TryFixOrHint Then
                            ModJava.SwitchToInstanceSetup(mcInstance)
                            ModMain.MyMsgBox(String.Format("该版本的版本设置选择了 {0}使用版本文件夹中的 Java{1}，但版本文件夹里没能找到 Java。{2}请先修改启动选项中的 Java 设置，或在版本文件夹中放一个 Java，然后再试。", ModBase.vbLQ, ModBase.vbRQ, vbCrLf), "未找到 Java", "确定", "", "", True, True, True, Nothing, Nothing, Nothing)
                        End If
                        java2 = Nothing
                    End If
                End If
            Catch ex As Exception
                ex.ThrowIfCanceled()
                Logger.[Error](ex, "选择 Java 时出现异常", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                If p IsNot Nothing Then
                    p.Skip(ProgressProvider.ChildrenAction.Skiped)
                End If
                java2 = Nothing
            End Try
            Return java2
        End Function

        Private Sub SwitchToInstanceSetup(Instance As ModMinecraft.McInstance)
            ModBase.RunInUi(Sub()
                PageInstanceLeft.Instance = Instance
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.SetupLink)
            End Sub, False)
        End Sub

        Private Sub MigrateInstanceJavaSettings(Instance As ModMinecraft.McInstance, Optional c As CancellationToken = Nothing, Optional p As ProgressProvider = Nothing)
            If Configs.JavaMigrated.[Get](Instance.Config) Then
                If p IsNot Nothing Then
                    p.Skip(ProgressProvider.ChildrenAction.Skiped)
                    Return
                End If
            Else
                If ModMain.FrmInstanceSetup IsNot Nothing Then
                    Dim action As Action
                    If ModJava._Closure$__.$I14-0 IsNot Nothing Then
                        action = ModJava._Closure$__.$I14-0
                    Else
                        Dim action2 As Action = Sub()
                            ModMain.FrmInstanceSetup.ComboArgumentJava.IsEnabled = False
                        End Sub
                        action = action2
                        ModJava._Closure$__.$I14-0 = action2
                    End If
                    ModBase.RunInUiWait(action)
                End If
                Try
                    Logger.Info(String.Format("正在尝试迁移版本独立的 Java 设置（{0}）", Instance.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                    Dim text As String = Settings.[Get](Of String)("VersionArgumentJavaSelect", Instance)
                    If Not text.StartsWithF("{", False) Then
                        If p IsNot Nothing Then
                            p.[Set](0.4, True, ProgressProvider.ChildrenAction.Finished)
                        End If
                    Else
                        Dim ForcedJava As Java = New Java(text.DeserializeJson()("Path").ToString(), Nothing)
                        If Not ForcedJava.CheckAsync(c).Run() Then
                            ModMain.Hint("该版本的设置中强制指定的 Java 存在异常，该设置已重置", ModMain.HintType.Red, True)
                            If p IsNot Nothing Then
                                p.[Set](0.4, False, ProgressProvider.ChildrenAction.Finished)
                            End If
                        Else
                            Logger.Info(String.Format("将版本独立设置改为老版本中强制指定的 Java：{0}", ForcedJava), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                            If Not Configs.JavaList.[Get]().Contains(ForcedJava) Then
                                Configs.JavaList.Edit(Sub(ByRef List As ConcurrentList(Of Java))
                                    List.Add(ForcedJava)
                                End Sub)
                                Logger.Info(String.Format("已将强制指定的 Java 加入 Java 列表底部：{0}", ForcedJava), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                            End If
                            Configs.JavaForced.[Set](ForcedJava, Instance.Config)
                            Settings.[Set]("VersionArgumentJavaV2", 3, Instance)
                            ModJava.UpdateJavaLists()
                            Configs.JavaMigrated.[Set](True, Instance.Config)
                            If p Is Nothing Then
                                Return
                            End If
                            p.Finish(ProgressProvider.ChildrenAction.Finished)
                            Return
                        End If
                    End If
                    Dim list As List(Of Java) = JavaUtils.SearchFoldersAsync(True, New String() { Instance.PathVersion }, c, If((p IsNot Nothing), p.SplitTo(1.0), Nothing)).Run()
                    If list.Any() Then
                        Logger.Info(String.Format("在版本文件夹中发现了 Java，将版本独立设置改为优先使用版本文件夹的 Java：{0}", list), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        Settings.[Set]("VersionArgumentJavaV2", 2, Instance)
                    Else
                        Logger.Info("没有需要迁移的内容，自动选择 Java", LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
                        Settings.[Set]("VersionArgumentJavaV2", 0, Instance)
                    End If
                    Configs.JavaMigrated.[Set](True, Instance.Config)
                Finally
                    If ModMain.FrmInstanceSetup IsNot Nothing Then
                        Dim action3 As Action
                        If ModJava._Closure$__.$I14-2 IsNot Nothing Then
                            action3 = ModJava._Closure$__.$I14-2
                        Else
                            Dim action4 As Action = Sub()
                                SettingService.RefreshSettings(ModMain.FrmInstanceSetup.ComboArgumentJava)
                                SettingService.RefreshSettings(ModMain.FrmInstanceSetup.ComboArgumentJavaSelect)
                                ModMain.FrmInstanceSetup.ComboArgumentJava.IsEnabled = True
                            End Sub
                            action3 = action4
                            ModJava._Closure$__.$I14-2 = action4
                        End If
                        ModBase.RunInUiWait(action3)
                    End If
                End Try
            End If
        End Sub

        Private Function CreateJavaDownloadLoader(Target As ModJava.JavaDownloadInfo) As ModLoader.LoaderCombo(Of Integer)
            Dim CS$<>8__locals1 As ModJava._Closure$__16-0 = New ModJava._Closure$__16-0(CS$<>8__locals1)
            Dim text As String = CStr(Target.Java("manifest")("url"))
            Logger.Info(String.Format("准备下载 Java {0}（{1}）：{2}", Target.Java("version")("name"), Target.ComponentName, text), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
            Dim text2 As String = ModNet.NetRequestByLoader(ModDownload.DlSourceOrder(New String() { text }, New String() { text.Replace("piston-meta.mojang.com", "bmclapi2.bangbang93.com") }), 45000, True, False)
            CS$<>8__locals1.$VB$Local_BaseDir = String.Format("{0}.minecraft\runtime\{1}\", Paths.AppData, Target.ComponentName)
            Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
            Try
                For Each jtoken As JToken In CType(text2.DeserializeJson(), JObject)("files")
                    Dim jproperty As JProperty = CType(jtoken, JProperty)
                    Dim jtoken2 As JToken = CType(jproperty.Value, JObject)("downloads")
                    If If((jtoken2 IsNot Nothing), jtoken2("raw"), Nothing) IsNot Nothing Then
                        Dim jobject As JObject = CType(CType(jproperty.Value, JObject)("downloads")("raw"), JObject)
                        Dim fileChecker As FileChecker = New FileChecker() With { .ActualSize = CLng(jobject("size")), .Hash = CStr(jobject("sha1")) }
                        If Operators.CompareString(fileChecker.Hash, "12976a6c2b227cbac58969c1455444596c894656", False) <> 0 AndAlso Operators.CompareString(fileChecker.Hash, "c80e4bab46e34d02826eab226a4441d0970f2aba", False) <> 0 AndAlso Operators.CompareString(fileChecker.Hash, "84d2102ad171863db04e7ee22a259d1f6c5de4a5", False) <> 0 Then
                            Dim text3 As String = CStr(jobject("url"))
                            list.Add(New ModNet.NetFile(ModDownload.DlSourceOrder(New String() { text3 }, New String() { text3.Replace("piston-data.mojang.com", "bmclapi2.bangbang93.com") }), CS$<>8__locals1.$VB$Local_BaseDir + jproperty.Name, fileChecker, False))
                        End If
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of JToken)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Logger.Info(String.Format("该 Java 需要下载 {0} 个文件，目标文件夹：{1}", list.Count, CS$<>8__locals1.$VB$Local_BaseDir), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Modules\Minecraft\ModJava.vb")
            Return New ModLoader.LoaderCombo(Of Integer)("下载 Java", New ModLoader.LoaderBase() { New ModNet.LoaderDownload("下载 Java 文件", list) With { .ProgressWeight = 10.0 }, New ModLoader.LoaderWorker("刷新 Java 列表", ModJava.JavaListRefreshWorker) }) With { .OnStateChanged = Sub(a0 As ModLoader.LoaderBase)
                MyBase._Lambda$__0(CType(a0, ModLoader.LoaderCombo(Of Integer)))
            End Sub }
        End Function

        Private JavaLock As Object = RuntimeHelpers.GetObjectValue(New Object())

        Private $STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker As RedoableWorker(Of List(Of ModJava.JavaDownloadInfo))

        Private $STATIC$SelectOrDownloadJava$041282411511824D21512824511282491287E821180D9128251$DownloadListWorker$Init As StaticLocalInitFlag

        Private Class JavaDownloadInfo
            Public Property ComponentName As String

            Public Property Java As JObject

            Public Property JavaVersion As Version
        End Class
    End Module
End Namespace
