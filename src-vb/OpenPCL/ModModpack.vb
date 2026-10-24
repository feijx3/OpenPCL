Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Diagnostics
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public NotInheritable Module ModModpack
        Public Sub ModpackInstall()
            ModMain.Hint("为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……", ModMain.HintType.Red, True)
        End Sub

        Public Function ModpackInstall(File As String, Optional InstanceName As String = Nothing, Optional Logo As String = Nothing) As ModLoader.LoaderCombo(Of String)
            Logger.Info(String.Format("整合包安装请求：{0}", If(File, "null")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
            Dim zipArchive As ZipArchive = Nothing
            Try
                Dim text As String = String.Format("{0}versions\{1}\", ModMinecraft.McFolderSelected, InstanceName)
                If text.Contains("!") OrElse text.Contains(";") Then
                    ModMain.Hint("游戏路径中不能含有感叹号或分号：" + text, ModMain.HintType.Red, True)
                    Throw New OperationCanceledException()
                End If
                Try
                    zipArchive = FileUtils.OpenZip(File, Nothing)
                    If zipArchive.GetEntry("mcbbs.packmeta") Is Nothing Then
                        If zipArchive.GetEntry("mmc-pack.json") Is Nothing Then
                            If zipArchive.GetEntry("modrinth.index.json") Is Nothing Then
                                If zipArchive.GetEntry("manifest.json") IsNot Nothing Then
                                    If CType(zipArchive.GetEntry("manifest.json").Open().ReadString(Encoding.UTF8).DeserializeJson(), JObject)("addons") Is Nothing Then
                                    End If
                                ElseIf zipArchive.GetEntry("modpack.json") Is Nothing Then
                                    If zipArchive.GetEntry("modpack.zip") Is Nothing AndAlso zipArchive.GetEntry("modpack.mrpack") Is Nothing Then
                                        Try
                                            For Each zipArchiveEntry As ZipArchiveEntry In zipArchive.Entries
                                                Dim array As String() = zipArchiveEntry.FullName.Split("/", False)
                                                Dim text2 As String = array(0) + "/"
                                                If array.Count() = 2 Then
                                                    If Operators.CompareString(array(1), "mcbbs.packmeta", False) = 0 Then
                                                        Exit For
                                                    End If
                                                    If Operators.CompareString(array(1), "mmc-pack.json", False) = 0 Then
                                                        Exit For
                                                    End If
                                                    If Operators.CompareString(array(1), "modrinth.index.json", False) = 0 Then
                                                        Exit For
                                                    End If
                                                    If Operators.CompareString(array(1), "manifest.json", False) = 0 Then
                                                        If CType(zipArchiveEntry.Open().ReadString(Encoding.UTF8).DeserializeJson(), JObject)("addons") Is Nothing Then
                                                            Exit For
                                                        End If
                                                        Exit For
                                                    Else
                                                        If Operators.CompareString(array(1), "modpack.json", False) = 0 Then
                                                            Exit For
                                                        End If
                                                        If Operators.CompareString(array(1), "modpack.zip", False) = 0 OrElse Operators.CompareString(array(1), "modpack.mrpack", False) = 0 Then
                                                            Exit For
                                                        End If
                                                    End If
                                                End If
                                            Next
                                        Finally
                                            Dim enumerator As IEnumerator(Of ZipArchiveEntry)
                                            If enumerator IsNot Nothing Then
                                                enumerator.Dispose()
                                            End If
                                        End Try
                                    End If
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    If ex.GetDisplay(True).Contains("Error.WinIOError") Then
                        Throw New Exception("打开整合包文件失败", ex)
                    End If
                    If Operators.CompareString(PathUtils.GetExtension(File), "rar", False) = 0 Then
                        Throw New Exception("PCL 无法处理 rar 格式的压缩包，请在解压后重新压缩为 zip 格式再试", ex)
                    End If
                    Throw New Exception("打开整合包文件失败，文件可能损坏或为不支持的压缩包格式", ex)
                End Try
                Throw New Exception("为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……")
            Finally
                If zipArchive IsNot Nothing Then
                    zipArchive.Dispose()
                    GoTo IL_02EE
                End If
                GoTo IL_02EE
                IL_02EE:
            End Try
        End Function

        Private Sub ExtractModpackFiles(InstallTemp As String, FileAddress As String, Optional c As CancellationToken = Nothing, Optional p As ProgressProvider = Nothing)
            DirectoryUtils.Delete(InstallTemp, False)
            c.ThrowIfCancellationRequested()
            FileUtils.ExtractToDirectory(FileAddress, InstallTemp, c, p)
        End Sub

        Private Sub CopyOverrideDirectory(OverridesFolder As String, VersionFolder As String, Loader As ModLoader.LoaderBase)
            OverridesFolder = PathUtils.AddSlashSuffix(OverridesFolder)
            VersionFolder = PathUtils.AddSlashSuffix(VersionFolder)
            If DirectoryUtils.Exists(OverridesFolder) Then
                Logger.Info(String.Format("处理整合包覆写文件夹：{0} → {1}", OverridesFolder, VersionFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                DirectoryUtils.Copy(OverridesFolder, VersionFolder)
            Else
                Logger.Info(String.Format("整合包中没有覆写文件夹：{0}", OverridesFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
            End If
            Dim text As String = String.Format("{0}PCL\Setup.ini", OverridesFolder)
            Dim text2 As String = String.Format("{0}PCL\Setup.ini", VersionFolder)
            If FileUtils.Exists(text) Then
                ModBase.WriteIni(text, "VersionArgumentIndie", Conversions.ToString(1))
                ModBase.WriteIni(text, "VersionArgumentIndieV2", Conversions.ToString(True))
                ModBase.WriteIni(text, "IsStar", Conversions.ToString(False))
                FileUtils.Copy(text, text2)
            Else
                ModBase.WriteIni(text2, "VersionArgumentIndie", Conversions.ToString(1))
                ModBase.WriteIni(text2, "VersionArgumentIndieV2", Conversions.ToString(True))
            End If
            ModBase.IniClearCache(text2)
        End Sub

        Private Sub NotifyIncompatibleLoader(LoaderName As String)
            If ModMain.OurMsgBox(String.Format("整合包中存在不兼容的加载器：{0}{1}如果你知道如何手动安装它，可以先选择跳过，然后在整合包下载结束后手动安装。", LoaderName, vbCrLf), "不兼容的加载器", "取消", String.Format("不安装 {0} 并继续", LoaderName), "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                Throw New OperationCanceledException()
            End If
        End Sub

        Private Function InstallPackCurseForge(FileAddress As String, Archive As ZipArchive, ArchiveBaseFolder As String, Optional InstanceName As String = Nothing, Optional Logo As String = Nothing) As ModLoader.LoaderCombo(Of String)
            Dim CS$<>8__locals1 As ModModpack._Closure$__5-0 = New ModModpack._Closure$__5-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress
            CS$<>8__locals1.$VB$Local_Logo = Logo
            Try
                CS$<>8__locals1.$VB$Local_Json = CType(Archive.GetEntry(ArchiveBaseFolder + "manifest.json").Open().ReadString(Nothing).DeserializeJson(), JObject)
            Catch ex As Exception
                Throw New Exception("CurseForge 整合包安装信息存在问题", ex)
            End Try
            If CS$<>8__locals1.$VB$Local_Json("minecraft") Is Nothing OrElse CS$<>8__locals1.$VB$Local_Json("minecraft")("version") Is Nothing Then
                Throw New Exception("CurseForge 整合包未提供 Minecraft 版本信息")
            End If
            If InstanceName Is Nothing Then
                InstanceName = CStr((If(CS$<>8__locals1.$VB$Local_Json("name"), "")))
                Dim validateFolderName As ValidateFolderName = New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, Nothing)
                If Operators.CompareString(validateFolderName.Validate(InstanceName), "", False) <> 0 Then
                    InstanceName = ""
                End If
                If Operators.CompareString(InstanceName, "", False) = 0 Then
                    InstanceName = ModMain.OurMsgBoxInput("输入版本名称", "", "", New Collection(Of Validate)() From { validateFolderName }, "", "确定", "取消", False)
                End If
                If String.IsNullOrEmpty(InstanceName) Then
                    Throw New OperationCanceledException()
                End If
            End If
            CS$<>8__locals1.$VB$Local_VersionFolder = PathUtils.AddSlashSuffix(Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\")
            CS$<>8__locals1.$VB$Local_TargetVersion = New McVersion(CS$<>8__locals1.$VB$Local_Json("minecraft")("version").ToString(), True)
            Try
                For Each jtoken As JToken In (If(CS$<>8__locals1.$VB$Local_Json("minecraft")("modLoaders"), New Byte(-1) {}))
                    Dim text As String = (If(jtoken("id"), "")).ToString().Lower()
                    If text.StartsWithF("forge-", False) Then
                        If text.Contains("recommended") Then
                            Throw New Exception("该整合包版本过老，已不支持进行安装！")
                        End If
                        Logger.Info(String.Format("整合包 Forge 版本：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions(ModLoaders.Forge) = text.Replace("forge-", "")
                    ElseIf text.StartsWithF("neoforge-", False) Then
                        Logger.Info(String.Format("整合包 NeoForge 版本：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions(ModLoaders.NeoForge) = text.Replace("neoforge-", "")
                    ElseIf text.StartsWithF("fabric-", False) Then
                        Logger.Info(String.Format("整合包 Fabric 版本：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions(ModLoaders.Fabric) = text.Replace("fabric-", "")
                    Else
                        ModModpack.NotifyIncompatibleLoader(text)
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of JToken)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(False)
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim text2 As String = CStr((If(CS$<>8__locals1.$VB$Local_Json("overrides"), "")))
            If Operators.CompareString(text2, "", False) <> 0 Then
                Dim CS$<>8__locals2 As ModModpack._Closure$__5-1 = New ModModpack._Closure$__5-1(CS$<>8__locals2)
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                CS$<>8__locals2.$VB$Local_OverridesFolder = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallTemp + ArchiveBaseFolder + If((Operators.CompareString(text2, ".", False) = 0 OrElse Operators.CompareString(text2, "./", False) = 0), "", text2)
                If Not PathUtils.IsParentOf(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallTemp, CS$<>8__locals2.$VB$Local_OverridesFolder) Then
                    Throw New Exception("整合包的 overrides 路径超出了解压目录，请向整合包作者反馈此问题！")
                End If
                list.Add(New ModLoader.LoaderTask(Of String, Integer)("解压整合包文件", Sub(Task As ModLoader.LoaderTask(Of String, Integer))
                    ModModpack.ExtractModpackFiles(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallTemp, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6))
                    ModModpack.CopyOverrideDirectory(CS$<>8__locals2.$VB$Local_OverridesFolder, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder, Task)
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = CDbl(FileUtils.GetInfo(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_FileAddress).Length) / 1024.0 / 1024.0 / 6.0, .Block = False })
            End If
            CS$<>8__locals1.$VB$Local_ModList = New List(Of Integer)()
            CS$<>8__locals1.$VB$Local_ModOptionalList = New List(Of Integer)()
            Try
                For Each jtoken2 As JToken In (If(CS$<>8__locals1.$VB$Local_Json("files"), New Byte(-1) {}))
                    If jtoken2("projectID") Is Nothing OrElse jtoken2("fileID") Is Nothing Then
                        ModMain.Hint("某项 Mod 缺少必要信息，已跳过：" + jtoken2.ToString(), ModMain.HintType.Blue, True)
                    Else
                        CS$<>8__locals1.$VB$Local_ModList.Add(CInt(jtoken2("fileID")))
                        If jtoken2("required") IsNot Nothing AndAlso Not jtoken2("required").ToObject(Of Boolean)() Then
                            CS$<>8__locals1.$VB$Local_ModOptionalList.Add(CInt(jtoken2("fileID")))
                        End If
                    End If
                Next
            Finally
                Dim enumerator2 As IEnumerator(Of JToken)
                If enumerator2 IsNot Nothing Then
                    enumerator2.Dispose()
                End If
            End Try
            If CS$<>8__locals1.$VB$Local_ModList.Any() Then
                Dim list2 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                list2.Add(New ModLoader.LoaderTask(Of Integer, JArray)("获取 Mod 下载信息", Function(Task As ModLoader.LoaderTask(Of Integer, JArray))
                    Task.Output = CType(ModDownload.DlModRequest("https://api.curseforge.com/v1/mods/files", HttpMethod.Post, "{""fileIds"": [" + CS$<>8__locals1.$VB$Local_ModList.Join(",") + "]}", "application/json")("data"), JArray)
                    If CS$<>8__locals1.$VB$Local_ModList.Count <> Task.Output.Count Then
                        Dim $VB$Local_ModList As IEnumerable(Of Integer) = CS$<>8__locals1.$VB$Local_ModList
                        Dim output As IEnumerable(Of JToken) = Task.Output
                        Dim func7 As Func(Of JToken, Integer)
                        If ModModpack._Closure$__.$I5-2 IsNot Nothing Then
                            func7 = ModModpack._Closure$__.$I5-2
                        Else
                            Dim func8 As Func(Of JToken, Integer) = Function(j As JToken) j("id").ToObject(Of Integer)()
                            func7 = func8
                            ModModpack._Closure$__.$I5-2 = func8
                        End If
                        Dim list8 As List(Of Integer) = $VB$Local_ModList.Except(output.[Select](func7)).ToList()
                        Logger.Info(String.Format("缺失的 fileIds：{0}", list8.Join("、")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        Dim enumerable4 As IEnumerable(Of Integer) = list8
                        Dim func9 As Func(Of Integer, ValueTuple(Of Integer, Integer))
                        If CS$<>8__locals1.$I3 IsNot Nothing Then
                            func9 = CS$<>8__locals1.$I3
                        Else
                            Dim func10 As Func(Of Integer, ValueTuple(Of Integer, Integer)) = Function(id As Integer)
                                Dim CS$<>8__locals4 As ModModpack._Closure$__5-2 = New ModModpack._Closure$__5-2(CS$<>8__locals4)
                                CS$<>8__locals4.$VB$Local_id = id
                                Return New ValueTuple(Of Integer, Integer)(CType(CS$<>8__locals1.$VB$Local_Json("files").AsEnumerable().First(Function(f As JToken) f("fileID").ToObject(Of Integer)() = CS$<>8__locals4.$VB$Local_id), JObject)("projectID").ToObject(Of Integer)(), CS$<>8__locals4.$VB$Local_id)
                            End Function
                            Dim func11 As Func(Of Integer, ValueTuple(Of Integer, Integer)) = func10
                            CS$<>8__locals1.$I3 = func10
                            func9 = func11
                        End If
                        Dim list9 As List(Of ValueTuple(Of Integer, Integer)) = enumerable4.[Select](func9).ToList()
                        Logger.Info(String.Format("缺失项：{0}", list9.Join("、")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        Dim list10 As List(Of ResourceProject) = New List(Of ResourceProject)()
                        Try
                            Dim enumerable5 As IEnumerable(Of ValueTuple(Of Integer, Integer)) = list9
                            Dim func12 As Func(Of ValueTuple(Of Integer, Integer), String)
                            If ModModpack._Closure$__.$I5-5 IsNot Nothing Then
                                func12 = ModModpack._Closure$__.$I5-5
                            Else
                                Dim func13 As Func(Of ValueTuple(Of Integer, Integer), String) = Function(<TupleElementNames(New String() { "ProjectId", "FileId" })> e As ValueTuple(Of Integer, Integer)) e.Item1.ToString()
                                func12 = func13
                                ModModpack._Closure$__.$I5-5 = func13
                            End If
                            list10 = ResourceProject.FromProjectIds(enumerable5.[Select](func12), ResourcePlatforms.CurseForge).ToList()
                        Catch ex2 As Exception
                            Logger.Warn(ex2, "获取缺失的工程信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        End Try
                        Dim list11 As List(Of String) = New List(Of String)()
                        Try
                            Dim enumerator3 As List(Of ValueTuple(Of Integer, Integer)).Enumerator = list9.GetEnumerator()
                            While enumerator3.MoveNext()
                                Dim CS$<>8__locals3 As ModModpack._Closure$__5-3 = New ModModpack._Closure$__5-3(CS$<>8__locals3)
                                CS$<>8__locals3.$VB$Local_Missing = enumerator3.Current
                                Dim resourceProject As ResourceProject = list10.FirstOrDefault(Function(p As ResourceProject) Conversions.ToDouble(p.Id) = CDbl(CS$<>8__locals3.$VB$Local_Missing.Item1))
                                If resourceProject Is Nothing Then
                                    list11.Add(String.Format("未知 CurseForge 项目 (项目 ID {0}，文件 ID {1})", CS$<>8__locals3.$VB$Local_Missing.Item1, CS$<>8__locals3.$VB$Local_Missing.Item2))
                                Else
                                    list11.Add(String.Format("{0} (文件 ID {1})", resourceProject.TranslatedName, CS$<>8__locals3.$VB$Local_Missing.Item2))
                                End If
                            End While
                        Finally
                            Dim enumerator3 As List(Of ValueTuple(Of Integer, Integer)).Enumerator
                            CType(enumerator3, IDisposable).Dispose()
                        End Try
                        If ModMain.OurMsgBox(String.Format("未找到该整合包所需的以下内容，它们可能已被原作者删除：{0}- {1}{2}{3}是否继续安装？缺失这些内容可能导致整合包出现异常！", New Object() { vbCrLf, list11.Join(vbCrLf & "- "), vbCrLf, vbCrLf }), "整合包内容缺失", "继续", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 2 Then
                            Task.Cancel()
                        End If
                    End If
                End Function, Nothing, ThreadPriority.Normal) With { .ProgressWeight = CDbl(CS$<>8__locals1.$VB$Local_ModList.Count) / 10.0 })
                list2.Add(New ModLoader.LoaderTask(Of JArray, List(Of ModNet.NetFile))("构造 Mod 下载信息", Sub(Task As ModLoader.LoaderTask(Of JArray, List(Of ModNet.NetFile)))
                    Dim dictionary As Dictionary(Of Integer, ModNet.NetFile) = New Dictionary(Of Integer, ModNet.NetFile)()
                    Try
                        For Each jtoken3 As JToken In Task.Input
                            Dim num As Integer = jtoken3("id").ToObject(Of Integer)()
                            If Not dictionary.ContainsKey(num) AndAlso (Not CS$<>8__locals1.$VB$Local_ModOptionalList.Contains(num) OrElse ModMain.OurMsgBox("是否要下载整合包中的可选文件 " + jtoken3("displayName").ToString() + "？", "下载可选文件", "是", "否", "", False, True, False, Nothing, Nothing, Nothing) <> 2) Then
                                Dim text3 As String
                                Dim resourceTypes As ResourceTypes
                                If jtoken3("modules").Any() Then
                                    Dim enumerable6 As IEnumerable(Of JToken) = CType(jtoken3("modules"), JArray)
                                    Dim func14 As Func(Of JToken, String)
                                    If ModModpack._Closure$__.$I5-8 IsNot Nothing Then
                                        func14 = ModModpack._Closure$__.$I5-8
                                    Else
                                        Dim func15 As Func(Of JToken, String) = Function(l As JToken) l("name").ToString()
                                        func14 = func15
                                        ModModpack._Closure$__.$I5-8 = func15
                                    End If
                                    Dim list12 As List(Of String) = enumerable6.[Select](func14).ToList()
                                    If Not list12.Contains("META-INF") AndAlso Not list12.Contains("mcmod.info") AndAlso Not list12.Contains("fabric.mod.json") Then
                                        Dim flag As Boolean
                                        If jtoken3 Is Nothing Then
                                            flag = False
                                        Else
                                            Dim jtoken4 As JToken = jtoken3("FileName")
                                            flag = jtoken4 IsNot Nothing AndAlso jtoken4.ToString().EndsWithF(".jar", True)
                                        End If
                                        If Not flag Then
                                            If list12.Contains("pack.mcmeta") Then
                                                text3 = "resourcepacks"
                                                resourceTypes = ResourceTypes.ResourcePack
                                                GoTo IL_018D
                                            End If
                                            If list12.Contains("shaders") Then
                                                text3 = "shaderpacks"
                                                resourceTypes = ResourceTypes.Shader
                                                GoTo IL_018D
                                            End If
                                            text3 = "saves"
                                            resourceTypes = ResourceTypes.Map
                                            GoTo IL_018D
                                        End If
                                    End If
                                    text3 = "mods"
                                    resourceTypes = ResourceTypes.[Mod]
                                Else
                                    text3 = "mods"
                                    resourceTypes = ResourceTypes.[Mod]
                                End If
                                IL_018D:
                                Dim resourceVersion As ResourceVersion = ResourceVersion.FromPlatformJson(CType(jtoken3, JObject), resourceTypes)
                                If resourceVersion.DownloadAvailable Then
                                    dictionary.Add(num, resourceVersion.ToNetFile(PathUtils.AddSlashSuffix(Path.Combine(CS$<>8__locals1.$VB$Local_VersionFolder, text3)), New ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.ModPack), CS$<>8__locals1.$VB$Local_TargetVersion.VanillaName, CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaders))
                                    Task.Progress += 1.0 / CDbl((1 + CS$<>8__locals1.$VB$Local_ModList.Count))
                                End If
                            End If
                        Next
                    Finally
                        Dim enumerator4 As IEnumerator(Of JToken)
                        If enumerator4 IsNot Nothing Then
                            enumerator4.Dispose()
                        End If
                    End Try
                    Task.Output = dictionary.Values.ToList()
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = CDbl(CS$<>8__locals1.$VB$Local_ModList.Count) / 200.0, .Show = False })
                list2.Add(New ModNet.LoaderDownload("下载 Mod", New List(Of ModNet.NetFile)()) With { .ProgressWeight = CDbl(CS$<>8__locals1.$VB$Local_ModList.Count) * 1.5 })
                Dim list3 As List(Of ModLoader.LoaderBase) = list
                Dim loaderCombo As ModLoader.LoaderCombo(Of Integer) = New ModLoader.LoaderCombo(Of Integer)("下载 Mod（主加载器）", list2)
                loaderCombo.Show = False
                Dim enumerable As IEnumerable(Of ModLoader.LoaderBase) = list2
                Dim func As Func(Of ModLoader.LoaderBase, Double)
                If ModModpack._Closure$__.$I5-9 IsNot Nothing Then
                    func = ModModpack._Closure$__.$I5-9
                Else
                    Dim func2 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                    func = func2
                    ModModpack._Closure$__.$I5-9 = func2
                End If
                loaderCombo.ProgressWeight = enumerable.Sum(func)
                list3.Add(loaderCombo)
            End If
            Dim mcComponents As McComponents = New McComponents(CS$<>8__locals1.$VB$Local_TargetVersion.VanillaName, CS$<>8__locals1.$VB$Local_TargetVersion.ModLoaderVersions, Nothing, Nothing, Nothing)
            Dim list4 As List(Of ModLoader.LoaderBase) = ModDownloadLib.McInstallLoadersGet(New ModDownloadLib.McInstallRequest() With { .NewInstanceName = InstanceName, .InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder, .Components = mcComponents })
            Dim list5 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim list6 As List(Of ModLoader.LoaderBase) = list5
            Dim loaderCombo2 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("整合包安装", list)
            loaderCombo2.Show = False
            loaderCombo2.Block = False
            Dim enumerable2 As IEnumerable(Of ModLoader.LoaderBase) = list
            Dim func3 As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I5-10 IsNot Nothing Then
                func3 = ModModpack._Closure$__.$I5-10
            Else
                Dim func4 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func3 = func4
                ModModpack._Closure$__.$I5-10 = func4
            End If
            loaderCombo2.ProgressWeight = enumerable2.Sum(func3)
            list6.Add(loaderCombo2)
            Dim list7 As List(Of ModLoader.LoaderBase) = list5
            Dim loaderCombo3 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("游戏安装", list4)
            loaderCombo3.Show = False
            Dim enumerable3 As IEnumerable(Of ModLoader.LoaderBase) = list4
            Dim func5 As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I5-11 IsNot Nothing Then
                func5 = ModModpack._Closure$__.$I5-11
            Else
                Dim func6 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func5 = func6
                ModModpack._Closure$__.$I5-11 = func6
            End If
            loaderCombo3.ProgressWeight = enumerable3.Sum(func5)
            list7.Add(loaderCombo3)
            list5.Add(New ModLoader.LoaderTask(Of String, String)("最终整理文件", Sub(Task As ModLoader.LoaderTask(Of String, String))
                If CS$<>8__locals1.$VB$Local_Logo IsNot Nothing AndAlso FileUtils.Exists(CS$<>8__locals1.$VB$Local_Logo) Then
                    FileUtils.Copy(CS$<>8__locals1.$VB$Local_Logo, CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Logo.png")
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Setup.ini", "Logo", "PCL\Logo.png")
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Setup.ini", "LogoCustom", "True")
                    Logger.Info(String.Format("已设置整合包 Logo：{0}", CS$<>8__locals1.$VB$Local_Logo), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                End If
                Dim text4 As String = String.Format("{0}saves\", CS$<>8__locals1.$VB$Local_VersionFolder)
                Try
                    For Each text5 As String In DirectoryUtils.EnumerateFiles(text4, False, "*.zip")
                        FileUtils.ExtractToDirectory(text5, text4, Task.CreateCancellationToken(), Nothing)
                        FileUtils.Delete(text5, False)
                    Next
                Finally
                    Dim enumerator5 As IEnumerator(Of String)
                    If enumerator5 IsNot Nothing Then
                        enumerator5.Dispose()
                    End If
                End Try
                For Each text6 As String In New String() { CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.zip", CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.mrpack" }
                    If FileUtils.Exists(text6) Then
                        Logger.Info(String.Format("删除原始整合包文件：{0}", text6), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        FileUtils.Delete(text6, False)
                    End If
                Next
                If FileUtils.Exists(CS$<>8__locals1.$VB$Local_FileAddress) AndAlso Operators.CompareString(PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_FileAddress), "modpack", False) = 0 Then
                    Logger.Info(String.Format("删除安装整合包文件：{0}", CS$<>8__locals1.$VB$Local_FileAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                    FileUtils.Delete(CS$<>8__locals1.$VB$Local_FileAddress, False)
                End If
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.1, .Show = False })
            CS$<>8__locals1.$VB$Local_LoaderName = "CurseForge 整合包安装：" + InstanceName + " "
            If ModLoader.LoaderTaskbar.Any(Function(l As ModLoader.LoaderBase) Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, False) = 0) Then
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim loaderCombo4 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(CS$<>8__locals1.$VB$Local_LoaderName, list5)
            loaderCombo4.OnStateChanged = AddressOf ModDownloadLib.McInstallState
            loaderCombo4.Start(CS$<>8__locals1.$VB$Local_VersionFolder, False)
            ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo4)
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            Return loaderCombo4
        End Function

        Private Function InstallPackModrinth(FileAddress As String, Archive As ZipArchive, ArchiveBaseFolder As String, Optional InstanceName As String = Nothing, Optional Logo As String = Nothing) As ModLoader.LoaderCombo(Of String)
            Dim CS$<>8__locals1 As ModModpack._Closure$__6-0 = New ModModpack._Closure$__6-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder
            CS$<>8__locals1.$VB$Local_Logo = Logo
            Dim jobject As JObject
            Try
                jobject = CType(Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "modrinth.index.json").Open().ReadString(Nothing).DeserializeJson(), JObject)
            Catch ex As Exception
                Throw New Exception("Modrinth 整合包安装信息存在问题", ex)
            End Try
            If jobject("dependencies") Is Nothing OrElse jobject("dependencies")("minecraft") Is Nothing Then
                Throw New Exception("Modrinth 整合包未提供 Minecraft 版本信息")
            End If
            Dim mcVersion As McVersion = New McVersion("Unknown", True)
            Try
                For Each jtoken As JToken In (If(jobject("dependencies"), New Byte(-1) {}))
                    Dim jproperty As JProperty = CType(jtoken, JProperty)
                    Dim text As String = jproperty.Name.Lower()
                    If Operators.CompareString(text, "minecraft", False) <> 0 Then
                        If Operators.CompareString(text, "forge", False) <> 0 Then
                            If Operators.CompareString(text, "neoforge", False) <> 0 AndAlso Operators.CompareString(text, "neo-forge", False) <> 0 Then
                                If Operators.CompareString(text, "fabric-loader", False) <> 0 Then
                                    ModModpack.NotifyIncompatibleLoader(jproperty.Name)
                                Else
                                    mcVersion.ModLoaderVersions(ModLoaders.Fabric) = jproperty.Value.ToString()
                                    Logger.Info(String.Format("整合包 Fabric 版本：{0}", mcVersion.ModLoaderVersions(ModLoaders.Fabric)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                                End If
                            Else
                                mcVersion.ModLoaderVersions(ModLoaders.NeoForge) = jproperty.Value.ToString()
                                Logger.Info(String.Format("整合包 NeoForge 版本：{0}", mcVersion.ModLoaderVersions(ModLoaders.NeoForge)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                            End If
                        Else
                            mcVersion.ModLoaderVersions(ModLoaders.Forge) = jproperty.Value.ToString()
                            Logger.Info(String.Format("整合包 Forge 版本：{0}", mcVersion.ModLoaderVersions(ModLoaders.Forge)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        End If
                    Else
                        mcVersion.VanillaName = jproperty.Value.ToString()
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of JToken)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            If InstanceName Is Nothing Then
                InstanceName = CStr((If(jobject("name"), "")))
                Dim validateFolderName As ValidateFolderName = New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, Nothing)
                If Operators.CompareString(validateFolderName.Validate(InstanceName), "", False) <> 0 Then
                    InstanceName = ""
                End If
                If Operators.CompareString(InstanceName, "", False) = 0 Then
                    InstanceName = ModMain.OurMsgBoxInput("输入版本名称", "", "", New Collection(Of Validate)() From { validateFolderName }, "", "确定", "取消", False)
                End If
                If String.IsNullOrEmpty(InstanceName) Then
                    Throw New OperationCanceledException()
                End If
            End If
            CS$<>8__locals1.$VB$Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\"
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(False)
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            list.Add(New ModLoader.LoaderTask(Of String, Integer)("解压整合包文件", Sub(Task As ModLoader.LoaderTask(Of String, Integer))
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.5))
                ModModpack.CopyOverrideDirectory(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "overrides", CS$<>8__locals1.$VB$Local_VersionFolder, Task)
                ModModpack.CopyOverrideDirectory(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "client-overrides", CS$<>8__locals1.$VB$Local_VersionFolder, Task)
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = CDbl(FileUtils.GetInfo(CS$<>8__locals1.$VB$Local_FileAddress).Length) / 1024.0 / 1024.0 / 6.0, .Block = False })
            Dim list2 As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
            Try
                For Each jtoken2 As JToken In (If(jobject("files"), New Byte(-1) {}))
                    If jtoken2("env") IsNot Nothing Then
                        Dim text2 As String = jtoken2("env")("client").ToString()
                        If Operators.CompareString(text2, "optional", False) <> 0 Then
                            If Operators.CompareString(text2, "unsupported", False) = 0 Then
                                Continue For
                            End If
                        ElseIf ModMain.OurMsgBox("是否要下载可选文件 " + PathUtils.GetLastPart(jtoken2("path").ToString()) + "？", "下载可选文件", "是", "否", "", False, True, False, Nothing, Nothing, Nothing) = 2 Then
                            Continue For
                        End If
                    End If
                    Dim enumerable As IEnumerable(Of JToken) = jtoken2("downloads")
                    Dim func As Func(Of JToken, IEnumerable(Of String))
                    If ModModpack._Closure$__.$I6-1 IsNot Nothing Then
                        func = ModModpack._Closure$__.$I6-1
                    Else
                        Dim func2 As Func(Of JToken, IEnumerable(Of String)) = Function(t As JToken) ResourceVersion.ParseCurseForgeDownloadUrls(t.ToString())
                        func = func2
                        ModModpack._Closure$__.$I6-1 = func2
                    End If
                    Dim list3 As List(Of String) = enumerable.SelectMany(func).ToList()
                    Dim list4 As List(Of String) = list3
                    Dim enumerable2 As IEnumerable(Of String) = list3
                    Dim func3 As Func(Of String, String)
                    If ModModpack._Closure$__.$I6-2 IsNot Nothing Then
                        func3 = ModModpack._Closure$__.$I6-2
                    Else
                        Dim func4 As Func(Of String, String) = Function(u As String) ModDownload.DlSourceModGet(u)
                        func3 = func4
                        ModModpack._Closure$__.$I6-2 = func4
                    End If
                    list4.AddRange(enumerable2.[Select](func3).ToList())
                    list3 = ResourceVersion.ParseModrinthTrackArguments(list3, New ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.ModPack), mcVersion.VanillaName, mcVersion.ModLoaders).ToList()
                    list3 = list3.Distinct().ToList()
                    Dim text3 As String = Path.Combine(CS$<>8__locals1.$VB$Local_VersionFolder, jtoken2("path").ToString().TrimStart(New Char() { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }))
                    If Not PathUtils.IsParentOf(CS$<>8__locals1.$VB$Local_VersionFolder, text3) Then
                        ModMain.OurMsgBox(String.Format("整合包的文件路径超出了版本文件夹，请向整合包作者反馈此问题！{0}目标：{1}{2}错误的文件：{3}", New Object() { vbCrLf, Path.GetFullPath(text3), vbCrLf, text3 }), "文件路径校验失败", "确定", "", "", True, True, False, Nothing, Nothing, Nothing)
                        Throw New OperationCanceledException()
                    End If
                    list2.Add(New ModNet.NetFile(list3, text3, New FileChecker() With { .Hash = jtoken2("hashes")("sha1").ToString() }, True))
                Next
            Finally
                Dim enumerator2 As IEnumerator(Of JToken)
                If enumerator2 IsNot Nothing Then
                    enumerator2.Dispose()
                End If
            End Try
            If list2.Any() Then
                list.Add(New ModNet.LoaderDownload("下载额外文件", list2) With { .ProgressWeight = CDbl(list2.Count) * 1.5 })
            End If
            Dim mcComponents As McComponents = New McComponents(mcVersion.VanillaName, mcVersion.ModLoaderVersions, Nothing, Nothing, Nothing)
            Dim list5 As List(Of ModLoader.LoaderBase) = ModDownloadLib.McInstallLoadersGet(New ModDownloadLib.McInstallRequest() With { .NewInstanceName = InstanceName, .InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder, .Components = mcComponents })
            Dim list6 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim list7 As List(Of ModLoader.LoaderBase) = list6
            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("整合包安装", list)
            loaderCombo.Show = False
            loaderCombo.Block = False
            Dim enumerable3 As IEnumerable(Of ModLoader.LoaderBase) = list
            Dim func5 As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I6-3 IsNot Nothing Then
                func5 = ModModpack._Closure$__.$I6-3
            Else
                Dim func6 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func5 = func6
                ModModpack._Closure$__.$I6-3 = func6
            End If
            loaderCombo.ProgressWeight = enumerable3.Sum(func5)
            list7.Add(loaderCombo)
            Dim list8 As List(Of ModLoader.LoaderBase) = list6
            Dim loaderCombo2 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("游戏安装", list5)
            loaderCombo2.Show = False
            Dim enumerable4 As IEnumerable(Of ModLoader.LoaderBase) = list5
            Dim func7 As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I6-4 IsNot Nothing Then
                func7 = ModModpack._Closure$__.$I6-4
            Else
                Dim func8 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func7 = func8
                ModModpack._Closure$__.$I6-4 = func8
            End If
            loaderCombo2.ProgressWeight = enumerable4.Sum(func7)
            list8.Add(loaderCombo2)
            list6.Add(New ModLoader.LoaderTask(Of String, String)("最终整理文件", Sub(Task As ModLoader.LoaderTask(Of String, String))
                If CS$<>8__locals1.$VB$Local_Logo IsNot Nothing AndAlso FileUtils.Exists(CS$<>8__locals1.$VB$Local_Logo) Then
                    FileUtils.Copy(CS$<>8__locals1.$VB$Local_Logo, CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Logo.png")
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Setup.ini", "Logo", "PCL\Logo.png")
                    ModBase.WriteIni(CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Setup.ini", "LogoCustom", "True")
                    Logger.Info(String.Format("已设置整合包 Logo：{0}", CS$<>8__locals1.$VB$Local_Logo), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                End If
                For Each text4 As String In New String() { CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.zip", CS$<>8__locals1.$VB$Local_VersionFolder + "原始整合包.mrpack" }
                    If FileUtils.Exists(text4) Then
                        Logger.Info(String.Format("删除原始整合包文件：{0}", text4), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        FileUtils.Delete(text4, False)
                    End If
                Next
                If FileUtils.Exists(CS$<>8__locals1.$VB$Local_FileAddress) AndAlso Operators.CompareString(PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_FileAddress), "modpack", False) = 0 Then
                    Logger.Info(String.Format("删除安装整合包文件：{0}", CS$<>8__locals1.$VB$Local_FileAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                    FileUtils.Delete(CS$<>8__locals1.$VB$Local_FileAddress, False)
                End If
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.1, .Show = False })
            CS$<>8__locals1.$VB$Local_LoaderName = String.Format("Modrinth 整合包安装：{0} ", InstanceName)
            If ModLoader.LoaderTaskbar.Any(Function(l As ModLoader.LoaderBase) Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, False) = 0) Then
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim loaderCombo3 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(CS$<>8__locals1.$VB$Local_LoaderName, list6)
            loaderCombo3.OnStateChanged = AddressOf ModDownloadLib.McInstallState
            loaderCombo3.Start(CS$<>8__locals1.$VB$Local_VersionFolder, False)
            ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo3)
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            Return loaderCombo3
        End Function

        Private Function InstallPackHMCL(FileAddress As String, Archive As ZipArchive, ArchiveBaseFolder As String) As ModLoader.LoaderCombo(Of String)
            Dim jobject As JObject
            Try
                jobject = CType(Archive.GetEntry(ArchiveBaseFolder + "modpack.json").Open().ReadString(Encoding.UTF8).DeserializeJson(), JObject)
            Catch ex As Exception
                Throw New Exception("HMCL 整合包安装信息存在问题", ex)
            End Try
            Dim text As String = CStr((If(jobject("name"), "")))
            Dim validateFolderName As ValidateFolderName = New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, Nothing)
            If Operators.CompareString(validateFolderName.Validate(text), "", False) <> 0 Then
                text = ""
            End If
            If Operators.CompareString(text, "", False) = 0 Then
                text = ModMain.OurMsgBoxInput("输入版本名称", "", "", New Collection(Of Validate)() From { validateFolderName }, "", "确定", "取消", False)
            End If
            If String.IsNullOrEmpty(text) Then
                Throw New OperationCanceledException()
            End If
            Dim VersionFolder As String = Path.Combine(ModMinecraft.McFolderSelected, "versions", text) + "\"
            Dim InstallTemp As String = ModMain.RequestTaskTempFolder(False)
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            list.Add(New ModLoader.LoaderTask(Of String, Integer)("解压整合包文件", Sub(Task As ModLoader.LoaderTask(Of String, Integer))
                ModModpack.ExtractModpackFiles(InstallTemp, FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6))
                ModModpack.CopyOverrideDirectory(InstallTemp + ArchiveBaseFolder + "minecraft", VersionFolder, Task)
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = CDbl(FileUtils.GetInfo(FileAddress).Length) / 1024.0 / 1024.0 / 6.0, .Block = False })
            If jobject("gameVersion") Is Nothing Then
                Throw New Exception("该 HMCL 整合包未提供游戏版本信息，无法安装！")
            End If
            Dim mcComponents As McComponents = New McComponents(jobject("gameVersion").ToString(), New Dictionary(Of ModLoaders, String)(), Nothing, Nothing, Nothing)
            Dim list2 As List(Of ModLoader.LoaderBase) = ModDownloadLib.McInstallLoadersGet(New ModDownloadLib.McInstallRequest() With { .NewInstanceName = text, .InstanceFolder = VersionFolder, .Components = mcComponents })
            Dim list3 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("整合包安装", list)
            loaderCombo.Show = False
            loaderCombo.Block = False
            Dim enumerable As IEnumerable(Of ModLoader.LoaderBase) = list
            Dim func As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I7-1 IsNot Nothing Then
                func = ModModpack._Closure$__.$I7-1
            Else
                Dim func2 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func = func2
                ModModpack._Closure$__.$I7-1 = func2
            End If
            loaderCombo.ProgressWeight = enumerable.Sum(func)
            list3.Add(loaderCombo)
            Dim loaderCombo2 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("游戏安装", list2)
            loaderCombo2.Show = False
            Dim enumerable2 As IEnumerable(Of ModLoader.LoaderBase) = list2
            Dim func3 As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I7-2 IsNot Nothing Then
                func3 = ModModpack._Closure$__.$I7-2
            Else
                Dim func4 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func3 = func4
                ModModpack._Closure$__.$I7-2 = func4
            End If
            loaderCombo2.ProgressWeight = enumerable2.Sum(func3)
            list3.Add(loaderCombo2)
            Dim list4 As List(Of ModLoader.LoaderBase) = list3
            Dim LoaderName As String = "HMCL 整合包安装：" + text + " "
            If ModLoader.LoaderTaskbar.Any(Function(l As ModLoader.LoaderBase) Operators.CompareString(l.Name, LoaderName, False) = 0) Then
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim loaderCombo3 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(LoaderName, list4)
            loaderCombo3.OnStateChanged = AddressOf ModDownloadLib.McInstallState
            loaderCombo3.Start(VersionFolder, False)
            ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo3)
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            Return loaderCombo3
        End Function

        Private Function InstallPackMMC(FileAddress As String, Archive As ZipArchive, ArchiveBaseFolder As String) As ModLoader.LoaderCombo(Of String)
            Dim CS$<>8__locals1 As ModModpack._Closure$__8-0 = New ModModpack._Closure$__8-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder
            Dim jobject As JObject
            Dim text As String
            Try
                jobject = CType(Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "mmc-pack.json").Open().ReadString(Encoding.UTF8).DeserializeJson(), JObject)
                text = Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "instance.cfg").Open().ReadString(Encoding.UTF8)
            Catch ex As Exception
                Throw New Exception("MMC 整合包安装信息存在问题", ex)
            End Try
            Dim text2 As String = If(text.RegexSeek("(?<=\nname\=)[^\n]+", RegexOptions.None), "")
            Dim validateFolderName As ValidateFolderName = New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, Nothing)
            If Operators.CompareString(validateFolderName.Validate(text2), "", False) <> 0 Then
                text2 = ""
            End If
            If Operators.CompareString(text2, "", False) = 0 Then
                text2 = ModMain.OurMsgBoxInput("输入版本名称", "", "", New Collection(Of Validate)() From { validateFolderName }, "", "确定", "取消", False)
            End If
            If String.IsNullOrEmpty(text2) Then
                Throw New OperationCanceledException()
            End If
            CS$<>8__locals1.$VB$Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", text2) + "\"
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(False)
            CS$<>8__locals1.$VB$Local_SetupFile = CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Setup.ini"
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            list.Add(New ModLoader.LoaderTask(Of String, Integer)("解压整合包文件", Sub(Task As ModLoader.LoaderTask(Of String, Integer))
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.55))
                Dim text5 As String
                If DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + ".minecraft") AndAlso Not DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "minecraft") Then
                    text5 = CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + ".minecraft"
                Else
                    text5 = CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "minecraft"
                End If
                ModModpack.CopyOverrideDirectory(text5, CS$<>8__locals1.$VB$Local_VersionFolder, Task)
                Try
                    Dim text6 As String = CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "instance.cfg"
                    If FileUtils.Exists(text6) Then
                        Dim list6 As List(Of String) = New List(Of String)()
                        For Each text7 As String In FileUtils.ReadAsLines(text6, True, Nothing, Nothing)
                            If text7.Contains("=") Then
                                list6.Add(text7.BeforeFirst("=", False) + ":" + text7.AfterFirst("=", False))
                            End If
                        Next
                        FileUtils.Write(text6, list6.Join(vbCrLf), Nothing)
                        If Conversions.ToBoolean(ModBase.ReadIni(text6, "OverrideCommands", Conversions.ToString(False))) Then
                            Dim text8 As String = ModBase.ReadIni(text6, "PreLaunchCommand", "")
                            If Operators.CompareString(text8, "", False) <> 0 Then
                                text8 = text8.Replace("\""", """").Replace("$INST_JAVA", "{java}java.exe").Replace("$INST_MC_DIR\", "{minecraft}").Replace("$INST_MC_DIR", "{minecraft}").Replace("$INST_DIR\", "{verpath}").Replace("$INST_DIR", "{verpath}").Replace("$INST_ID", "{name}").Replace("$INST_NAME", "{name}")
                                ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceRun", text8)
                                Logger.Info(String.Format("迁移 MultiMC 版本独立设置：启动前执行命令：{0}", text8), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                            End If
                        End If
                        If Conversions.ToBoolean(ModBase.ReadIni(text6, "JoinServerOnLaunch", Conversions.ToString(False))) Then
                            Dim text9 As String = ModBase.ReadIni(text6, "JoinServerOnLaunchAddress", "").Replace("\""", """")
                            ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionServerEnter", text9)
                            Logger.Info(String.Format("迁移 MultiMC 版本独立设置：自动进入服务器：{0}", text9), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        End If
                        Dim text10 As String = ModBase.ReadIni(text6, "iconKey", "")
                        If Operators.CompareString(text10, "", False) <> 0 AndAlso FileUtils.Exists(String.Format("{0}{1}{2}.png", CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, text10)) Then
                            ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "LogoCustom", Conversions.ToString(True))
                            ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "Logo", "PCL\Logo.png")
                            FileUtils.Copy(String.Format("{0}{1}{2}.png", CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, text10), CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Logo.png")
                            Logger.Info(String.Format("迁移 MultiMC 版本独立设置：版本图标（{0}.png）", text10), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        End If
                        Dim text11 As String = ModBase.ReadIni(text6, "JvmArgs", "")
                        If Operators.CompareString(text11, "", False) <> 0 Then
                            If Conversions.ToBoolean(ModBase.ReadIni(text6, "OverrideJavaArgs", Conversions.ToString(False))) Then
                                ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceJvm", text11)
                                Logger.Info(String.Format("迁移 MultiMC 版本独立设置：JVM 参数（覆盖）：{0}", text11), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                            Else
                                text11 = text11 + " " + Settings.[Get](Of String)("LaunchAdvanceJvm", Nothing)
                                ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceJvm", text11)
                                Logger.Info(String.Format("迁移 MultiMC 版本独立设置：JVM 参数（追加）：{0}", text11), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                            End If
                        End If
                    End If
                Catch ex2 As Exception
                    Logger.Warn(ex2, String.Format("读取 MMC 配置文件失败（{0}{1}instance.cfg）", CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_ArchiveBaseFolder), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                End Try
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = CDbl(FileUtils.GetInfo(CS$<>8__locals1.$VB$Local_FileAddress).Length) / 1024.0 / 1024.0 / 6.0, .Block = False })
            If jobject("components") Is Nothing Then
                Throw New Exception("该 MMC 整合包未提供游戏版本信息，无法安装！")
            End If
            Dim text3 As String = Nothing
            Dim dictionary As Dictionary(Of ModLoaders, String) = New Dictionary(Of ModLoaders, String)()
            Try
                For Each jtoken As JToken In jobject("components")
                    Dim jobject2 As JObject = CType(jtoken, JObject)
                    If jobject2.ContainsKey("uid") Then
                        Dim text4 As String = jobject2("uid").ToString()
                        If Operators.CompareString(text4, "net.minecraft", False) <> 0 Then
                            If Operators.CompareString(text4, "net.minecraftforge", False) <> 0 Then
                                If Operators.CompareString(text4, "net.neoforged", False) <> 0 Then
                                    If Operators.CompareString(text4, "net.fabricmc.fabric-loader", False) <> 0 Then
                                        If text4.StartsWithF("org.lwjgl", False) Then
                                            Logger.Info(String.Format("已跳过 LWJGL 项：{0}", text4), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                                        Else
                                            ModModpack.NotifyIncompatibleLoader(text4)
                                        End If
                                    Else
                                        dictionary(ModLoaders.Fabric) = CStr(jobject2("version"))
                                    End If
                                Else
                                    dictionary(ModLoaders.NeoForge) = CStr(jobject2("version"))
                                End If
                            Else
                                dictionary(ModLoaders.Forge) = CStr(jobject2("version"))
                            End If
                        Else
                            text3 = CStr(jobject2("version"))
                        End If
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of JToken)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Dim mcComponents As McComponents = New McComponents(text3, dictionary, Nothing, Nothing, Nothing)
            Dim list2 As List(Of ModLoader.LoaderBase) = ModDownloadLib.McInstallLoadersGet(New ModDownloadLib.McInstallRequest() With { .NewInstanceName = text2, .InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder, .Components = mcComponents })
            Dim list3 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim list4 As List(Of ModLoader.LoaderBase) = list3
            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("整合包安装", list)
            loaderCombo.Show = False
            loaderCombo.Block = False
            Dim enumerable As IEnumerable(Of ModLoader.LoaderBase) = list
            Dim func As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I8-1 IsNot Nothing Then
                func = ModModpack._Closure$__.$I8-1
            Else
                Dim func2 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func = func2
                ModModpack._Closure$__.$I8-1 = func2
            End If
            loaderCombo.ProgressWeight = enumerable.Sum(func)
            list4.Add(loaderCombo)
            Dim list5 As List(Of ModLoader.LoaderBase) = list3
            Dim loaderCombo2 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("游戏安装", list2)
            loaderCombo2.Show = False
            Dim enumerable2 As IEnumerable(Of ModLoader.LoaderBase) = list2
            Dim func3 As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I8-2 IsNot Nothing Then
                func3 = ModModpack._Closure$__.$I8-2
            Else
                Dim func4 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func3 = func4
                ModModpack._Closure$__.$I8-2 = func4
            End If
            loaderCombo2.ProgressWeight = enumerable2.Sum(func3)
            list5.Add(loaderCombo2)
            CS$<>8__locals1.$VB$Local_LoaderName = "MMC 整合包安装：" + text2 + " "
            If ModLoader.LoaderTaskbar.Any(Function(l As ModLoader.LoaderBase) Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, False) = 0) Then
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim loaderCombo3 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(CS$<>8__locals1.$VB$Local_LoaderName, list3)
            loaderCombo3.OnStateChanged = AddressOf ModDownloadLib.McInstallState
            loaderCombo3.Start(CS$<>8__locals1.$VB$Local_VersionFolder, False)
            ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo3)
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            Return loaderCombo3
        End Function

        Private Function InstallPackMCBBS(FileAddress As String, Archive As ZipArchive, ArchiveBaseFolder As String, Optional InstanceName As String = Nothing) As ModLoader.LoaderCombo(Of String)
            Dim CS$<>8__locals1 As ModModpack._Closure$__9-0 = New ModModpack._Closure$__9-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder
            Try
                Dim zipArchiveEntry As ZipArchiveEntry = If(Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "mcbbs.packmeta"), Archive.GetEntry(CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "manifest.json"))
                CS$<>8__locals1.$VB$Local_Json = CType(zipArchiveEntry.Open().ReadString(Encoding.UTF8).DeserializeJson(), JObject)
            Catch ex As Exception
                Throw New Exception("MCBBS 整合包安装信息存在问题", ex)
            End Try
            If InstanceName Is Nothing Then
                InstanceName = CStr((If(CS$<>8__locals1.$VB$Local_Json("name"), "")))
                Dim validateFolderName As ValidateFolderName = New ValidateFolderName(ModMinecraft.McFolderSelected + "versions", True, Nothing)
                If Operators.CompareString(validateFolderName.Validate(InstanceName), "", False) <> 0 Then
                    InstanceName = ""
                End If
                If Operators.CompareString(InstanceName, "", False) = 0 Then
                    InstanceName = ModMain.OurMsgBoxInput("输入版本名称", "", "", New Collection(Of Validate)() From { validateFolderName }, "", "确定", "取消", False)
                End If
                If String.IsNullOrEmpty(InstanceName) Then
                    Throw New OperationCanceledException()
                End If
            End If
            CS$<>8__locals1.$VB$Local_VersionFolder = Path.Combine(ModMinecraft.McFolderSelected, "versions", InstanceName) + "\"
            CS$<>8__locals1.$VB$Local_InstallTemp = ModMain.RequestTaskTempFolder(False)
            CS$<>8__locals1.$VB$Local_SetupFile = CS$<>8__locals1.$VB$Local_VersionFolder + "PCL\Setup.ini"
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            list.Add(New ModLoader.LoaderTask(Of String, Integer)("解压整合包文件", Sub(Task As ModLoader.LoaderTask(Of String, Integer))
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_InstallTemp, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.6))
                ModModpack.CopyOverrideDirectory(CS$<>8__locals1.$VB$Local_InstallTemp + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder + "overrides", CS$<>8__locals1.$VB$Local_VersionFolder, Task)
                If CS$<>8__locals1.$VB$Local_Json("launchInfo") IsNot Nothing Then
                    Dim jobject2 As JObject = CType(CS$<>8__locals1.$VB$Local_Json("launchInfo"), JObject)
                    If jobject2.ContainsKey("javaArgument") Then
                        ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceJvm", String.Join(Of JToken)(" ", jobject2("javaArgument")))
                    End If
                    If jobject2.ContainsKey("launchArgument") Then
                        ModBase.WriteIni(CS$<>8__locals1.$VB$Local_SetupFile, "VersionAdvanceGame", String.Join(Of JToken)(" ", jobject2("launchArgument")))
                    End If
                End If
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = CDbl(FileUtils.GetInfo(CS$<>8__locals1.$VB$Local_FileAddress).Length) / 1024.0 / 1024.0 / 6.0, .Block = False })
            If CS$<>8__locals1.$VB$Local_Json("addons") Is Nothing Then
                Throw New Exception("该 MCBBS 整合包未提供游戏版本附加信息，无法安装！")
            End If
            Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Try
                For Each jtoken As JToken In CS$<>8__locals1.$VB$Local_Json("addons")
                    dictionary.Add(CStr(jtoken("id")), CStr(jtoken("version")))
                Next
            Finally
                Dim enumerator As IEnumerator(Of JToken)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            If Not dictionary.ContainsKey("game") Then
                Throw New Exception("该 MCBBS 整合包未提供游戏版本信息，无法安装！")
            End If
            If dictionary.ContainsKey("quilt") Then
                ModMain.Hint("PCL 不支持安装 Quilt 整合包！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim dictionary2 As Dictionary(Of ModLoaders, String) = New Dictionary(Of ModLoaders, String)()
            If dictionary.ContainsKey("forge") Then
                dictionary2(ModLoaders.Forge) = dictionary("forge")
            End If
            If dictionary.ContainsKey("neoforge") Then
                dictionary2(ModLoaders.NeoForge) = dictionary("neoforge")
            End If
            If dictionary.ContainsKey("fabric") Then
                dictionary2(ModLoaders.Fabric) = dictionary("fabric")
            End If
            Dim jobject As JObject = Nothing
            If dictionary.ContainsKey("optifine") Then
                Dim CS$<>8__locals2 As ModModpack._Closure$__9-1 = New ModModpack._Closure$__9-1(CS$<>8__locals2)
                CS$<>8__locals2.$VB$Local_MinecraftVersion = dictionary("game")
                If CS$<>8__locals2.$VB$Local_MinecraftVersion.EndsWithF(".0", False) Then
                    ' The following expression was wrapped in a checked-expression
                    CS$<>8__locals2.$VB$Local_MinecraftVersion = CS$<>8__locals2.$VB$Local_MinecraftVersion.Substring(0, CS$<>8__locals2.$VB$Local_MinecraftVersion.Length - 2)
                End If
                CS$<>8__locals2.$VB$Local_OptiFineFileSuffix = "_HD_" + dictionary("optifine").AfterLast("HD_", False) + ".jar"
                If ModDownload.DlOptiFineListBmclapiLoader.State <> ModBase.LoadState.Finished Then
                    ModDownload.DlOptiFineListBmclapiLoader.WaitForExit(Nothing, Nothing, False, Nothing, Nothing)
                End If
                Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry = ModDownload.DlOptiFineListBmclapiLoader.Output.Value.FirstOrDefault(Function(v As ModDownload.DlOptiFineListEntry) Operators.CompareString(v.Inherit, CS$<>8__locals2.$VB$Local_MinecraftVersion, False) = 0 AndAlso v.FileName.EndsWithF(CS$<>8__locals2.$VB$Local_OptiFineFileSuffix, False))
                If dlOptiFineListEntry Is Nothing Then
                    Throw New Exception(String.Format("OptiFine 列表中没有整合包所需的 OptiFine {0}（Minecraft {1}）", dictionary("optifine"), dictionary("game")))
                End If
                jobject = JObject.FromObject(dlOptiFineListEntry)
            End If
            Dim mcComponents As McComponents = New McComponents(dictionary("game"), dictionary2, jobject, Nothing, Nothing)
            Dim list2 As List(Of ModLoader.LoaderBase) = ModDownloadLib.McInstallLoadersGet(New ModDownloadLib.McInstallRequest() With { .NewInstanceName = InstanceName, .InstanceFolder = CS$<>8__locals1.$VB$Local_VersionFolder, .Components = mcComponents })
            Dim list3 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim list4 As List(Of ModLoader.LoaderBase) = list3
            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("整合包安装", list)
            loaderCombo.Show = False
            loaderCombo.Block = False
            Dim enumerable As IEnumerable(Of ModLoader.LoaderBase) = list
            Dim func As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I9-2 IsNot Nothing Then
                func = ModModpack._Closure$__.$I9-2
            Else
                Dim func2 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func = func2
                ModModpack._Closure$__.$I9-2 = func2
            End If
            loaderCombo.ProgressWeight = enumerable.Sum(func)
            list4.Add(loaderCombo)
            Dim list5 As List(Of ModLoader.LoaderBase) = list3
            Dim loaderCombo2 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("游戏安装", list2)
            loaderCombo2.Show = False
            Dim enumerable2 As IEnumerable(Of ModLoader.LoaderBase) = list2
            Dim func3 As Func(Of ModLoader.LoaderBase, Double)
            If ModModpack._Closure$__.$I9-3 IsNot Nothing Then
                func3 = ModModpack._Closure$__.$I9-3
            Else
                Dim func4 As Func(Of ModLoader.LoaderBase, Double) = Function(l As ModLoader.LoaderBase) l.ProgressWeight
                func3 = func4
                ModModpack._Closure$__.$I9-3 = func4
            End If
            loaderCombo2.ProgressWeight = enumerable2.Sum(func3)
            list5.Add(loaderCombo2)
            CS$<>8__locals1.$VB$Local_LoaderName = "MCBBS 整合包安装：" + InstanceName + " "
            If ModLoader.LoaderTaskbar.Any(Function(l As ModLoader.LoaderBase) Operators.CompareString(l.Name, CS$<>8__locals1.$VB$Local_LoaderName, False) = 0) Then
                ModMain.Hint("该整合包正在安装中！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim loaderCombo3 As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(CS$<>8__locals1.$VB$Local_LoaderName, list3)
            loaderCombo3.OnStateChanged = AddressOf ModDownloadLib.McInstallState
            loaderCombo3.Start(CS$<>8__locals1.$VB$Local_VersionFolder, False)
            ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo3)
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            Return loaderCombo3
        End Function

        Private Function InstallPackLauncherPack(FileAddress As String, Archive As ZipArchive, ArchiveBaseFolder As String) As ModLoader.LoaderCombo(Of String)
            Dim CS$<>8__locals1 As ModModpack._Closure$__10-0 = New ModModpack._Closure$__10-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = ArchiveBaseFolder
            ModMain.OurMsgBox("接下来请选择一个空文件夹，它会被安装到这个文件夹里。", "安装", "继续", "", "", False, True, True, Nothing, Nothing, Nothing)
            CS$<>8__locals1.$VB$Local_TargetFolder = Dialogs.SelectFolder("选择安装目标（必须是一个空文件夹）", False, Nothing).FirstOrDefault()
            If CS$<>8__locals1.$VB$Local_TargetFolder Is Nothing Then
                Throw New OperationCanceledException()
            End If
            If Not DirectoryUtils.IsEmpty(CS$<>8__locals1.$VB$Local_TargetFolder) Then
                ModMain.Hint("请选择一个空文件夹作为安装目标！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("解压压缩包", New ModLoader.LoaderBase() { New ModLoader.LoaderTask(Of String, Integer)("解压压缩包", Sub(Task As ModLoader.LoaderTask(Of String, Integer))
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_TargetFolder, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.9))
                ModBase.OpenExplorer(CS$<>8__locals1.$VB$Local_TargetFolder)
                Thread.Sleep(400)
                Dim text As String = Nothing
                Try
                    For Each text2 As String In DirectoryUtils.EnumerateFiles(CS$<>8__locals1.$VB$Local_TargetFolder, False, "*.exe")
                        Dim versionInfo As FileVersionInfo = FileVersionInfo.GetVersionInfo(text2)
                        Logger.Info(String.Format("文件 {0} 的产品名标识为 {1}", text2, versionInfo.ProductName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        If Operators.CompareString(versionInfo.ProductName, "Plain Craft Launcher", False) = 0 Then
                            text = text2
                            Logger.Info(String.Format("发现整合包附带的 PCL 启动器：{0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        ElseIf(versionInfo.ProductName.ContainsIgnoreCase("Launcher") OrElse versionInfo.ProductName.Contains("启动")) AndAlso Operators.CompareString(versionInfo.ProductName, "Plain Craft Launcher Admin Manager", False) <> 0 AndAlso text Is Nothing Then
                            text = text2
                            Logger.Info(String.Format("发现整合包附带的疑似第三方启动器：{0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of String)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                Task.Progress = 0.95
                If text IsNot Nothing Then
                    Logger.Info(String.Format("找到压缩包中附带的启动器：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                    If ModMain.OurMsgBox(String.Format("整合包里似乎自带了启动器，是否换用它继续安装？{0}即将打开：{1}", vbCrLf, text), "换用整合包启动器？", "换用", "不换用", "", False, True, False, Nothing, Nothing, Nothing) = 1 Then
                        ModBase.StartProcess(text, "--wait")
                        Logger.Info("为换用整合包中的启动器启动，强制结束程序", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                        ModMain.FrmMain.EndProgram(False)
                        Return
                    End If
                Else
                    Logger.Info("未找到压缩包中附带的启动器", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                End If
                Dim lastPart As String = PathUtils.GetLastPart(CS$<>8__locals1.$VB$Local_TargetFolder)
                DirectoryUtils.Create(CS$<>8__locals1.$VB$Local_TargetFolder + ".minecraft\")
                PageSelectLeft.AddFolder(CS$<>8__locals1.$VB$Local_TargetFolder + ".minecraft\" + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder.Replace("/", "\").TrimStart(New Char() { "\"c }), lastPart, False)
                Dim text3 As String = DirectoryUtils.EnumerateFiles(CS$<>8__locals1.$VB$Local_TargetFolder, True, "modpack.*").First()
                Logger.Info(String.Format("调用 modpack 文件继续安装：{0}", text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
                ModModpack.ModpackInstall(text3, Nothing, Nothing)
            End Sub, Nothing, ThreadPriority.Normal) })
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_TargetFolder, False)
            ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo)
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            ModMain.FrmMain.BtnExtraDownload.Ribble()
            Return loaderCombo
        End Function

        Private Function InstallPackCompress(FileAddress As String, Archive As ZipArchive) As ModLoader.LoaderCombo(Of String)
            Dim CS$<>8__locals1 As ModModpack._Closure$__11-0 = New ModModpack._Closure$__11-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_FileAddress = FileAddress
            Dim match As Match = Nothing
            Dim regex As Regex = New Regex("^.*\/(?=versions\/(?<ver>[^\/]+)\/(\k<ver>)\.json$)", RegexOptions.IgnoreCase)
            Try
                For Each zipArchiveEntry As ZipArchiveEntry In Archive.Entries
                    Dim match2 As Match = regex.Match("/" + zipArchiveEntry.FullName)
                    If match2.Success Then
                        match = match2
                        Exit For
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of ZipArchiveEntry)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            If match Is Nothing Then
                Throw New Exception("文件结构不匹配，这可能不是 Minecraft 客户端压缩包？")
            End If
            CS$<>8__locals1.$VB$Local_ArchiveBaseFolder = match.Value.Replace("/", "\").TrimStart(New Char() { "\"c })
            Logger.Info(String.Format("检测到压缩包的 .minecraft 根目录：{0}，命中的版本名：{1}", CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, match.Groups(1).Value), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModModpack.vb")
            ModMain.OurMsgBox("接下来请选择一个空文件夹，它会被安装到这个文件夹里。", "安装", "继续", "", "", False, True, True, Nothing, Nothing, Nothing)
            CS$<>8__locals1.$VB$Local_TargetFolder = Dialogs.SelectFolder("选择安装目标（必须是一个空文件夹）", False, Nothing).FirstOrDefault()
            If CS$<>8__locals1.$VB$Local_TargetFolder Is Nothing Then
                Throw New OperationCanceledException()
            End If
            If CS$<>8__locals1.$VB$Local_TargetFolder.Contains("!") OrElse CS$<>8__locals1.$VB$Local_TargetFolder.Contains(";") Then
                ModMain.Hint("Minecraft 文件夹路径中不能含有感叹号或分号！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            If Not DirectoryUtils.IsEmpty(CS$<>8__locals1.$VB$Local_TargetFolder) Then
                ModMain.Hint("请选择一个空文件夹作为安装目标！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("解压压缩包", New ModLoader.LoaderBase() { New ModLoader.LoaderTask(Of String, Integer)("解压压缩包", Sub(Task As ModLoader.LoaderTask(Of String, Integer))
                ModModpack.ExtractModpackFiles(CS$<>8__locals1.$VB$Local_TargetFolder, CS$<>8__locals1.$VB$Local_FileAddress, Task.CreateCancellationToken(), Task.CreateSyncProgressProvider(0.0, 0.95))
                PageSelectLeft.AddFolder(CS$<>8__locals1.$VB$Local_TargetFolder + CS$<>8__locals1.$VB$Local_ArchiveBaseFolder, PathUtils.GetLastPart(CS$<>8__locals1.$VB$Local_TargetFolder), False)
                Thread.Sleep(400)
                Dim action As Action
                If ModModpack._Closure$__.$I11-1 IsNot Nothing Then
                    action = ModModpack._Closure$__.$I11-1
                Else
                    Dim action2 As Action = Sub()
                        ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSelect, FormMain.PageSubType.[Default])
                    End Sub
                    action = action2
                    ModModpack._Closure$__.$I11-1 = action2
                End If
                ModBase.RunInUi(action, False)
            End Sub, Nothing, ThreadPriority.Normal) })
            loaderCombo.OnStateChanged = AddressOf ModDownloadLib.McInstallState
            loaderCombo.Start(CS$<>8__locals1.$VB$Local_TargetFolder, False)
            ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo)
            ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
            ModMain.FrmMain.BtnExtraDownload.Ribble()
            Return loaderCombo
        End Function
    End Module
End Namespace
