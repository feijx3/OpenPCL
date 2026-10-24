Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows.Documents
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCL.My
Imports PCLCS

Namespace PCL
    Public NotInheritable Module ModMinecraft
        Public Property McFolderSelected As String
            Get
                If ModMinecraft._McFolderSelected Is Nothing Then
                    ModMinecraft.SetMcFolder(Settings.[Get](Of String)("LaunchFolderSelect", Nothing))
                End If
                Return ModMinecraft._McFolderSelected
            End Get
            Set(value As String)
                ModMinecraft.SetMcFolder(value)
            End Set
        End Property

        Private Sub SetMcFolder(Value As String)
            If Not PathUtils.IsSame(ModMinecraft._McFolderSelected, Value.Replace("$", Paths.Base)) Then
                ModMinecraft._McFolderSelected = Value.Replace("$", Paths.Base)
                ModMinecraft._McFolderSelected = PathUtils.AddSlashSuffix(PathUtils.ForCompare(ModMinecraft._McFolderSelected))
                Settings.[Set]("LaunchFolderSelect", Value.Replace(Paths.Base, "$"), Nothing)
                Logger.Info(String.Format("当前选择的 Minecraft 文件夹：{0}", ModMinecraft._McFolderSelected), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            End If
        End Sub

        Private Sub McFolderListLoadSub()
            Try
                Dim list As List(Of ModMinecraft.McFolder) = New List(Of ModMinecraft.McFolder)()
                Try
                    If DirectoryUtils.Exists(Paths.Base + "versions\") Then
                        list.Add(New ModMinecraft.McFolder() With { .Name = "当前文件夹", .Location = Paths.Base, .Type = ModMinecraft.McFolder.Types.Vanilla })
                    End If
                    For Each directoryInfo As DirectoryInfo In DirectoryUtils.GetInfo(Paths.Base).GetDirectories()
                        If DirectoryUtils.Exists(directoryInfo.FullName + "versions\") OrElse Operators.CompareString(directoryInfo.Name, ".minecraft", False) = 0 Then
                            list.Add(New ModMinecraft.McFolder() With { .Name = "当前文件夹", .Location = PathUtils.AddSlashSuffix(PathUtils.ForCompare(directoryInfo.FullName)), .Type = ModMinecraft.McFolder.Types.Vanilla })
                        End If
                    Next
                Catch ex As Exception
                    Logger.Warn(ex, "扫描 PCL 所在文件夹中是否有 MC 文件夹失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End Try
                Dim text As String = Paths.AppData + ".minecraft\"
                If(Not list.Any() OrElse Not PathUtils.IsSame(text, list(0).Location)) AndAlso DirectoryUtils.Exists(text + "versions\") Then
                    list.Add(New ModMinecraft.McFolder() With { .Name = "官方启动器文件夹", .Location = text, .Type = ModMinecraft.McFolder.Types.Vanilla })
                End If
                For Each text2 As String In Settings.[Get](Of String)("LaunchFolders", Nothing).ToString().Split("|"c, True)
                    If Operators.CompareString(text2, "", False) <> 0 Then
                        If Not text2.Contains(">") OrElse Not text2.EndsWithF("\", False) Then
                            ModMain.Hint("无效的 Minecraft 文件夹：" + text2, ModMain.HintType.Red, True)
                        Else
                            Dim text3 As String = text2.BeforeFirst(">", False)
                            Dim text4 As String = text2.AfterFirst(">", False)
                            Try
                                ModBase.CheckPermissionWithException(text4)
                                Dim flag As Boolean = False
                                Try
                                    For Each mcFolder As ModMinecraft.McFolder In list
                                        If PathUtils.IsSame(mcFolder.Location, text4) Then
                                            mcFolder.Name = text3
                                            mcFolder.Type = ModMinecraft.McFolder.Types.RenamedVanilla
                                            flag = True
                                        End If
                                    Next
                                Finally
                                    Dim enumerator As List(Of ModMinecraft.McFolder).Enumerator
                                    CType(enumerator, IDisposable).Dispose()
                                End Try
                                If Not flag Then
                                    list.Add(New ModMinecraft.McFolder() With { .Name = text3, .Location = PathUtils.AddSlashSuffix(PathUtils.ForCompare(text4)), .Type = ModMinecraft.McFolder.Types.Custom })
                                End If
                            Catch ex2 As Exception
                                ModMain.OurMsgBox("失效的 Minecraft 文件夹：" & vbCrLf + text4 + vbCrLf & vbCrLf + ex2.GetDisplay(False), "Minecraft 文件夹失效", "确定", "", "", True, True, False, Nothing, Nothing, Nothing)
                                Logger.Warn(ex2, String.Format("无法访问 Minecraft 文件夹 {0}", text4), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                            End Try
                        End If
                    End If
                Next
                Dim list2 As List(Of String) = New List(Of String)()
                Try
                    For Each mcFolder2 As ModMinecraft.McFolder In list
                        If mcFolder2.Type <> ModMinecraft.McFolder.Types.Vanilla Then
                            list2.Add(mcFolder2.Name + ">" + mcFolder2.Location)
                        End If
                    Next
                Finally
                    Dim enumerator2 As List(Of ModMinecraft.McFolder).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                If Not list2.Any() Then
                    list2.Add("")
                End If
                Settings.[Set]("LaunchFolders", list2.Join("|"c), Nothing)
                If Not list.Any() Then
                    DirectoryUtils.Create(Paths.Base + ".minecraft\versions\")
                    list.Add(New ModMinecraft.McFolder() With { .Name = "当前文件夹", .Location = Paths.Base + ".minecraft\", .Type = ModMinecraft.McFolder.Types.Vanilla })
                End If
                Try
                    For Each mcFolder3 As ModMinecraft.McFolder In list
                        ModMinecraft.McFolderLauncherProfilesJsonCreate(mcFolder3.Location)
                    Next
                Finally
                    Dim enumerator3 As List(Of ModMinecraft.McFolder).Enumerator
                    CType(enumerator3, IDisposable).Dispose()
                End Try
                If Settings.[Get](Of Boolean)("SystemDebugDelay", Nothing) Then
                    Thread.Sleep(ModBase.RandomInteger(200, 2000))
                End If
                ModMinecraft.McFolderList = list
            Catch ex3 As Exception
                Logger.[Error](ex3, "加载 Minecraft 文件夹列表失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            End Try
        End Sub

        Public Sub McFolderLauncherProfilesJsonCreate(Folder As String)
            Try
                If Not FileUtils.Exists(Folder + "launcher_profiles.json") Then
                    Dim text As String = String.Concat(New String() { "{" & vbCrLf & "    ""profiles"":  {" & vbCrLf & "        ""PCL"": {" & vbCrLf & "            ""icon"": ""Grass""," & vbCrLf & "            ""name"": ""PCL""," & vbCrLf & "            ""lastVersionId"": ""latest-release""," & vbCrLf & "            ""type"": ""latest-release""," & vbCrLf & "            ""lastUsed"": """, DateTime.Now.ToString("yyyy'-'MM'-'dd"), "T", DateTime.Now.ToString("HH':'mm':'ss"), ".0000Z""" & vbCrLf & "        }" & vbCrLf & "    }," & vbCrLf & "    ""selectedProfile"": ""PCL""," & vbCrLf & "    ""clientToken"": ""23323323323323323323323323323333""" & vbCrLf & "}" })
                    FileUtils.Write(Folder + "launcher_profiles.json", text, Encoding.GetEncoding("GB18030"))
                    Logger.Info(String.Format("已创建 launcher_profiles.json：{0}", Folder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("创建 launcher_profiles.json 失败（{0}）", Folder), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            End Try
        End Sub

        Public Property McInstanceSelected As ModMinecraft.McInstance
            Get
                Return ModMinecraft._McInstanceSelected
            End Get
            Set(value As ModMinecraft.McInstance)
                If ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init Is Nothing Then
                    Interlocked.CompareExchange(Of StaticLocalInitFlag)(ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init, New StaticLocalInitFlag(), Nothing)
                End If
                Dim flag As Boolean = False
                Try
                    Monitor.Enter(ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init, flag)
                    If ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State = 0S Then
                        ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State = 2S
                        ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast = 0
                    ElseIf ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State = 2S Then
                        Throw New IncompleteInitialization()
                    End If
                Finally
                    ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init.State = 1S
                    If flag Then
                        Monitor.[Exit](ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init)
                    End If
                End Try
                If Not Object.ReferenceEquals(RuntimeHelpers.GetObjectValue(ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast), value) Then
                    ModMinecraft._McInstanceSelected = value
                    ModMinecraft.$STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast = value
                    Logger.Info(String.Format("当前选择的 Minecraft 版本：{0}", If(If((value IsNot Nothing), value.PathVersion, Nothing), "无")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", If(If((value IsNot Nothing), value.Name, Nothing), ""))
                    If value IsNot Nothing Then
                        PageDownloadResourceDetail.CachedFolder.Clear()
                        If ModAnimation.AniControlEnabled = 0 AndAlso Operators.CompareString(Settings.[Get](Of String)("VersionServerNide", value), Settings.[Get](Of String)("CacheNideServer", Nothing), False) <> 0 AndAlso Settings.[Get](Of Integer)("VersionServerLogin", value) = 3 Then
                            Settings.[Set]("CacheNideAccess", "", Nothing)
                            Logger.Info("服务器改变，要求重新登录统一通行证", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        End If
                        If Settings.[Get](Of Integer)("VersionServerLogin", value) = 3 Then
                            Settings.[Set]("CacheNideServer", Settings.[Get](Of String)("VersionServerNide", value), Nothing)
                        End If
                        If ModAnimation.AniControlEnabled = 0 AndAlso Operators.CompareString(Settings.[Get](Of String)("VersionServerAuthServer", value), Settings.[Get](Of String)("CacheAuthServerServer", Nothing), False) <> 0 AndAlso Settings.[Get](Of Integer)("VersionServerLogin", value) = 4 Then
                            Settings.[Set]("CacheAuthAccess", "", Nothing)
                            Logger.Info("服务器改变，要求重新登录 Authlib-Injector", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        End If
                        If Settings.[Get](Of Integer)("VersionServerLogin", value) = 4 Then
                            Settings.[Set]("CacheAuthServerServer", Settings.[Get](Of String)("VersionServerAuthServer", value), Nothing)
                            Settings.[Set]("CacheAuthServerName", Settings.[Get](Of String)("VersionServerAuthName", value), Nothing)
                            Settings.[Set]("CacheAuthServerRegister", Settings.[Get](Of String)("VersionServerAuthRegister", value), Nothing)
                        End If
                    End If
                End If
            End Set
        End Property

        Public Function GetMcFoolName(Name As String) As String
            Name = Name.Lower()
            Dim text As String
            If Name.StartsWithF("2.0", False) Then
                text = "2013 | 这个秘密计划了两年的更新将游戏推向了一个新高度！"
            ElseIf Operators.CompareString(Name, "15w14a", False) = 0 Then
                text = "2015 | 作为一款全年龄向的游戏，我们需要和平，需要爱与拥抱。"
            ElseIf Operators.CompareString(Name, "1.rv-pre1", False) = 0 Then
                text = "2016 | 是时候将现代科技带入 Minecraft 了！"
            ElseIf Operators.CompareString(Name, "3d shareware v1.34", False) = 0 Then
                text = "2019 | 我们从地下室的废墟里找到了这个开发于 1994 年的杰作！"
            ElseIf Name.StartsWithF("20w14inf", False) OrElse Operators.CompareString(Name, "20w14∞", False) = 0 Then
                text = "2020 | 我们加入了 20 亿个新的维度，让无限的想象变成了现实！"
            ElseIf Operators.CompareString(Name, "22w13oneblockatatime", False) = 0 Then
                text = "2022 | 一次一个方块更新！迎接全新的挖掘、合成与骑乘玩法吧！"
            ElseIf Operators.CompareString(Name, "23w13a_or_b", False) = 0 Then
                text = "2023 | 研究表明：玩家喜欢作出选择——越多越好！"
            ElseIf Operators.CompareString(Name, "24w14potato", False) = 0 Then
                text = "2024 | 毒马铃薯一直都被大家忽视和低估，于是我们超级加强了它！"
            ElseIf Operators.CompareString(Name, "25w14craftmine", False) = 0 Then
                text = "2025 | 你可以合成任何东西——包括合成你的世界！"
            ElseIf Operators.CompareString(Name, "26w14a", False) = 0 Then
                text = "2026 | 为什么需要物品栏？让方块们跟着你走吧！"
            Else
                text = ""
            End If
            Return text
        End Function

        Private Sub InitMcInstanceList(Loader As ModLoader.LoaderTask(Of String, Integer))
            Dim input As String = Loader.Input
            Try
                ModMinecraft.McInstanceList = New Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))()
                Dim list As List(Of String) = New List(Of String)()
                If DirectoryUtils.Exists(input + "versions") Then
                    Try
                        For Each directoryInfo As DirectoryInfo In DirectoryUtils.GetInfo(input + "versions").GetDirectories()
                            list.Add(directoryInfo.Name)
                        Next
                    Catch ex As Exception
                        Throw New Exception("无法读取版本文件夹，可能是由于没有权限（" + input + "versions）", ex)
                    End Try
                End If
                If Not list.Any() Then
                    ModBase.WriteIni(input + "PCL.ini", "InstanceCache", "")
                Else
                    Dim num As Integer = Convert.ToInt32(Decimal.Remainder(New Decimal((Conversions.ToString(39) + "#" + list.Join("#"c)).GetStableHashCode()), 2147483646D))
                    If Not ModMinecraft.McInstanceListForceRefresh AndAlso ModBase.Val(ModBase.ReadIni(input + "PCL.ini", "InstanceCache", "")) = CDbl(num) Then
                        Dim dictionary As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) = ModMinecraft.InitMcInstanceListWithCache(input)
                        If dictionary IsNot Nothing Then
                            ModMinecraft.McInstanceList = dictionary
                            GoTo IL_0161
                        End If
                    End If
                    ModMinecraft.McInstanceListForceRefresh = False
                    Logger.Info("文件夹列表变更，重载所有版本", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    ModBase.WriteIni(input + "PCL.ini", "InstanceCache", Conversions.ToString(num))
                    ModMinecraft.McInstanceList = ModMinecraft.InitMcInstanceListWithoutCache(input)
                    IL_0161:
                    ModMinecraft.IsFirstMcInstanceListLoad = False
                End If
                If Not Loader.IsCanceled Then
                    Dim mcInstanceList As IEnumerable(Of KeyValuePair(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))) = ModMinecraft.McInstanceList
                    Dim func As Func(Of KeyValuePair(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)), Boolean)
                    If ModMinecraft._Closure$__.$I22-0 IsNot Nothing Then
                        func = ModMinecraft._Closure$__.$I22-0
                    Else
                        Dim func2 As Func(Of KeyValuePair(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)), Boolean) = Function(v As KeyValuePair(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))) v.Key <> ModMinecraft.McInstanceCardType.[Error]
                        func = func2
                        ModMinecraft._Closure$__.$I22-0 = func2
                    End If
                    If mcInstanceList.Any(func) Then
                        Dim text As String = ModBase.ReadIni(input + "PCL.ini", "Version", "")
                        If Operators.CompareString(text, "", False) <> 0 Then
                            Try
                                For Each keyValuePair As KeyValuePair(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) In ModMinecraft.McInstanceList
                                    Try
                                        For Each mcInstance As ModMinecraft.McInstance In keyValuePair.Value
                                            If Operators.CompareString(mcInstance.Name, text, False) = 0 AndAlso mcInstance.State <> ModMinecraft.McInstance.McInstanceState.[Error] Then
                                                ModMinecraft.McInstanceSelected = mcInstance
                                                Logger.Info(String.Format("选择该文件夹储存的 Minecraft 版本：{0}", ModMinecraft.McInstanceSelected.PathVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                                Return
                                            End If
                                        Next
                                    Finally
                                        Dim enumerator2 As List(Of ModMinecraft.McInstance).Enumerator
                                        CType(enumerator2, IDisposable).Dispose()
                                    End Try
                                Next
                            Finally
                                Dim enumerator As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)).Enumerator
                                CType(enumerator, IDisposable).Dispose()
                            End Try
                        End If
                        If ModMinecraft.McInstanceList.First().Value(0).State <> ModMinecraft.McInstance.McInstanceState.[Error] Then
                            ModMinecraft.McInstanceSelected = ModMinecraft.McInstanceList.First().Value(0)
                            Logger.Info(String.Format("自动选择 Minecraft 版本：{0}", ModMinecraft.McInstanceSelected.PathVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        End If
                    Else
                        ModMinecraft.McInstanceSelected = Nothing
                        Logger.Info("未找到可用 Minecraft 版本", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    End If
                    If Settings.[Get](Of Boolean)("SystemDebugDelay", Nothing) Then
                        Thread.Sleep(ModBase.RandomInteger(200, 3000))
                    End If
                End If
            Catch ex2 As Exception
                If Not Loader.IsCanceled AndAlso Not ex2.IsCanceled() Then
                    ModBase.WriteIni(input + "PCL.ini", "InstanceCache", "")
                    Logger.[Error](ex2, "加载 .minecraft 版本列表失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End If
            End Try
        End Sub

        Private Function InitMcInstanceListWithCache(Folder As String) As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))
            Dim dictionary As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) = New Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))()
            Dim dictionary2 As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))
            Try
                Dim num As Integer = Conversions.ToInteger(ModBase.ReadIni(Folder + "PCL.ini", "CardCount", Conversions.ToString(-1)))
                If num = -1 Then
                    dictionary2 = Nothing
                Else
                    Dim num2 As Integer = num - 1
                    For i As Integer = 0 To num2
                        Dim mcInstanceCardType As ModMinecraft.McInstanceCardType = CType(Conversions.ToInteger(ModBase.ReadIni(Folder + "PCL.ini", "CardKey" + Conversions.ToString(i + 1), ":")), ModMinecraft.McInstanceCardType)
                        Dim list As List(Of ModMinecraft.McInstance) = New List(Of ModMinecraft.McInstance)()
                        For Each text As String In ModBase.ReadIni(Folder + "PCL.ini", "CardValue" + Conversions.ToString(i + 1), ":").Split(":", False)
                            If Operators.CompareString(text, "", False) <> 0 Then
                                Dim text2 As String = String.Format("{0}versions\{1}\", Folder, text)
                                If FileUtils.Exists(text2 + ".pclignore") Then
                                    If Not ModMinecraft.IsFirstMcInstanceListLoad Then
                                        Logger.Info(String.Format("跳过要求忽略的项目：{0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                        GoTo IL_06D1
                                    End If
                                    Logger.Info(String.Format("清理残留的忽略项目：{0}", text2), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                    FileUtils.Delete(text2 + ".pclignore", False)
                                End If
                                Try
                                    Dim CS$<>8__locals1 As ModMinecraft._Closure$__23-0 = New ModMinecraft._Closure$__23-0(CS$<>8__locals1)
                                    CS$<>8__locals1.$VB$Local_Instance = New ModMinecraft.McInstance(text2)
                                    list.Add(CS$<>8__locals1.$VB$Local_Instance)
                                    CS$<>8__locals1.$VB$Local_Instance.Info = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "CustomInfo", "")
                                    If Operators.CompareString(CS$<>8__locals1.$VB$Local_Instance.Info, "", False) = 0 Then
                                        CS$<>8__locals1.$VB$Local_Instance.Info = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "Info", CS$<>8__locals1.$VB$Local_Instance.Info)
                                    End If
                                    CS$<>8__locals1.$VB$Local_Instance.Logo = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "Logo", CS$<>8__locals1.$VB$Local_Instance.Logo)
                                    CS$<>8__locals1.$VB$Local_Instance.State = CType(Conversions.ToInteger(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "State", Conversions.ToString(CInt(CS$<>8__locals1.$VB$Local_Instance.State)))), ModMinecraft.McInstance.McInstanceState)
                                    CS$<>8__locals1.$VB$Local_Instance.IsStar = Conversions.ToBoolean(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "IsStar", Conversions.ToString(False)))
                                    CS$<>8__locals1.$VB$Local_Instance.DisplayType = CType(Conversions.ToInteger(ModBase.ReadIni(Folder + "PCL\Setup.ini", "DisplayType", Conversions.ToString(0))), ModMinecraft.McInstanceCardType)
                                    Dim mcInstallManifest As McInstallManifest = If((CS$<>8__locals1.$VB$Local_Instance.State = ModMinecraft.McInstance.McInstanceState.[Error]), Nothing, Configs.McInstallManifest.[Get](CS$<>8__locals1.$VB$Local_Instance.Config))
                                    If mcInstallManifest IsNot Nothing AndAlso mcInstallManifest.SchemaVersion <= 1 Then
                                        Dim managedFiles As IEnumerable(Of McInstallManifest.ManagedFile) = mcInstallManifest.ManagedFiles
                                        Dim func As Func(Of McInstallManifest.ManagedFile, Boolean)
                                        If ModMinecraft._Closure$__.$I23-0 IsNot Nothing Then
                                            func = ModMinecraft._Closure$__.$I23-0
                                        Else
                                            Dim func2 As Func(Of McInstallManifest.ManagedFile, Boolean) = Function(File As McInstallManifest.ManagedFile) File.Component <> Nothing
                                            func = func2
                                            ModMinecraft._Closure$__.$I23-0 = func2
                                        End If
                                        If managedFiles.Any(func) Then
                                            Dim mcInstallManifest2 As McInstallManifest = mcInstallManifest.RefreshModComponents(CS$<>8__locals1.$VB$Local_Instance.PathIndie)
                                            If mcInstallManifest2 IsNot mcInstallManifest Then
                                                mcInstallManifest = mcInstallManifest2
                                                Configs.McInstallManifest.[Set](mcInstallManifest, CS$<>8__locals1.$VB$Local_Instance.Config)
                                                CS$<>8__locals1.$VB$Local_Instance.Config.Save()
                                            End If
                                        End If
                                    End If
                                    Dim text3 As String = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "VersionVanillaName", "Unknown")
                                    If CS$<>8__locals1.$VB$Local_Instance.State <> ModMinecraft.McInstance.McInstanceState.[Error] AndAlso (mcInstallManifest IsNot Nothing OrElse Operators.CompareString(text3, "Unknown", False) <> 0) Then
                                        Dim mcVersion2 As McVersion
                                        If mcInstallManifest IsNot Nothing Then
                                            Dim mcVersion As McVersion = New McVersion(mcInstallManifest.Components)
                                            Dim managedFiles2 As IEnumerable(Of McInstallManifest.ManagedFile) = mcInstallManifest.ManagedFiles
                                            Dim func3 As Func(Of McInstallManifest.ManagedFile, Boolean)
                                            If ModMinecraft._Closure$__.$I23-1 IsNot Nothing Then
                                                func3 = ModMinecraft._Closure$__.$I23-1
                                            Else
                                                Dim func4 As Func(Of McInstallManifest.ManagedFile, Boolean) = Function(File As McInstallManifest.ManagedFile) Operators.CompareString(File.Component, "OptiFine", False) = 0
                                                func3 = func4
                                                ModMinecraft._Closure$__.$I23-1 = func4
                                            End If
                                            mcVersion.OptiFineAsMod = managedFiles2.Any(func3)
                                            mcVersion2 = mcVersion
                                        Else
                                            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_20(Of String, String) = Function(Key As String)
                                                Dim text6 As String = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", Key, Nothing)
                                                If Operators.CompareString(text6, "", False) <> 0 Then
                                                    Return text6
                                                End If
                                                Return Nothing
                                            End Function
                                            mcVersion2 = New McVersion(text3, True) With { .OptiFine = vb$AnonymousDelegate_("VersionOptiFine") }
                                            For Each modLoaders As ModLoaders In New ModLoaders() { ModLoaders.Fabric, ModLoaders.Forge, ModLoaders.NeoForge }
                                                Dim text4 As String = vb$AnonymousDelegate_(String.Format("Version{0}", modLoaders))
                                                If text4 IsNot Nothing Then
                                                    mcVersion2.ModLoaderVersions(modLoaders) = text4
                                                End If
                                            Next
                                            If Conversions.ToBoolean(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "VersionLiteLoader", Conversions.ToString(False))) Then
                                                mcVersion2.ModLoaderVersions(ModLoaders.LiteLoader) = "未知版本"
                                            End If
                                        End If
                                        mcVersion2.ReleaseTime = Conversions.ToDate(ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "ReleaseTime", Conversions.ToString(New DateTime(1970, 1, 1, 15, 0, 0))))
                                        mcVersion2.IsFool = mcVersion2.IsFool OrElse CS$<>8__locals1.$VB$Local_Instance.State = ModMinecraft.McInstance.McInstanceState.Fool
                                        mcVersion2.IsSnapshot = mcVersion2.IsSnapshot OrElse CS$<>8__locals1.$VB$Local_Instance.State = ModMinecraft.McInstance.McInstanceState.Snapshot OrElse CS$<>8__locals1.$VB$Local_Instance.Name.ContainsIgnoreCase("combat")
                                        CS$<>8__locals1.$VB$Local_Instance.Version = mcVersion2
                                        If mcInstallManifest IsNot Nothing Then
                                            CS$<>8__locals1.$VB$Local_Instance.RefreshInfo()
                                        End If
                                    Else
                                        CS$<>8__locals1.$VB$Local_Instance.Load()
                                    End If
                                    If CS$<>8__locals1.$VB$Local_Instance.State = ModMinecraft.McInstance.McInstanceState.[Error] Then
                                        Dim info As String = CS$<>8__locals1.$VB$Local_Instance.Info
                                        CS$<>8__locals1.$VB$Local_Instance.State = ModMinecraft.McInstance.McInstanceState.Original
                                        CS$<>8__locals1.$VB$Local_Instance.Check()
                                        Dim text5 As String = ModBase.ReadIni(CS$<>8__locals1.$VB$Local_Instance.PathVersion + "PCL\Setup.ini", "CustomInfo", "")
                                        If CS$<>8__locals1.$VB$Local_Instance.State = ModMinecraft.McInstance.McInstanceState.Original OrElse (Operators.CompareString(text5, "", False) = 0 AndAlso Operators.CompareString(info, CS$<>8__locals1.$VB$Local_Instance.Info, False) <> 0) Then
                                            Logger.Info(String.Format("版本 {0} 的错误状态已变更，新的状态为：{1}", CS$<>8__locals1.$VB$Local_Instance.Name, CS$<>8__locals1.$VB$Local_Instance.Info), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                            Return Nothing
                                        End If
                                    End If
                                    If Operators.CompareString(CS$<>8__locals1.$VB$Local_Instance.Logo, "", False) = 0 Then
                                        Logger.Info(String.Format("版本 {0} 未被加载", CS$<>8__locals1.$VB$Local_Instance.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                        Return Nothing
                                    End If
                                Catch ex As Exception
                                    Logger.Warn(ex, String.Format("读取版本加载缓存失败（{0}）", text), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                    Return Nothing
                                End Try
                            End If
                            IL_06D1:
                        Next
                        If list.Any() Then
                            dictionary.Add(mcInstanceCardType, list)
                        End If
                    Next
                    dictionary2 = dictionary
                End If
            Catch ex2 As Exception
                Logger.Warn(ex2, "读取版本缓存失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                dictionary2 = Nothing
            End Try
            Return dictionary2
        End Function

        Private Function InitMcInstanceListWithoutCache(Folder As String) As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))
            Dim list As List(Of ModMinecraft.McInstance) = New List(Of ModMinecraft.McInstance)()
            For Each directoryInfo As DirectoryInfo In DirectoryUtils.GetInfo(Folder + "versions").GetDirectories()
                If Not directoryInfo.Exists OrElse Not directoryInfo.EnumerateFiles().Any() Then
                    Logger.Info(String.Format("跳过空文件夹：{0}", directoryInfo.FullName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                ElseIf(Operators.CompareString(directoryInfo.Name, "cache", False) = 0 OrElse Operators.CompareString(directoryInfo.Name, "BLClient", False) = 0 OrElse Operators.CompareString(directoryInfo.Name, "PCL", False) = 0) AndAlso Not FileUtils.Exists(directoryInfo.FullName + "\" + directoryInfo.Name + ".json") Then
                    Logger.Info(String.Format("跳过可能不是版本文件夹的项目：{0}", directoryInfo.FullName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                Else
                    Dim text As String = directoryInfo.FullName + "\"
                    If FileUtils.Exists(text + ".pclignore") Then
                        If Not ModMinecraft.IsFirstMcInstanceListLoad Then
                            Logger.Info(String.Format("跳过要求忽略的项目：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                            GoTo IL_016E
                        End If
                        Logger.Info(String.Format("清理残留的忽略项目：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        FileUtils.Delete(text + ".pclignore", False)
                    End If
                    Dim mcInstance As ModMinecraft.McInstance = New ModMinecraft.McInstance(text)
                    list.Add(mcInstance)
                    mcInstance.Load()
                End If
                IL_016E:
            Next
            Dim dictionary As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) = New Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))()
            Try
                Dim dictionary2 As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) = New Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))()
                Dim list2 As List(Of ModMinecraft.McInstance) = New List(Of ModMinecraft.McInstance)()
                Try
                    For Each mcInstance2 As ModMinecraft.McInstance In list.ToList()
                        If mcInstance2.IsStar AndAlso mcInstance2.DisplayType <> ModMinecraft.McInstanceCardType.Hidden Then
                            list2.Add(mcInstance2)
                            list.Remove(mcInstance2)
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of ModMinecraft.McInstance).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                If list2.Any() Then
                    dictionary2.Add(ModMinecraft.McInstanceCardType.Star, list2)
                End If
                ModMinecraft.McInstanceFilter(list, dictionary2, New ModMinecraft.McInstance.McInstanceState(0) {}, ModMinecraft.McInstanceCardType.[Error])
                ModMinecraft.McInstanceFilter(list, dictionary2, New ModMinecraft.McInstance.McInstanceState() { ModMinecraft.McInstance.McInstanceState.Fool }, ModMinecraft.McInstanceCardType.Fool)
                ModMinecraft.McInstanceFilter(list, dictionary2, New ModMinecraft.McInstance.McInstanceState() { ModMinecraft.McInstance.McInstanceState.Forge, ModMinecraft.McInstance.McInstanceState.NeoForge, ModMinecraft.McInstance.McInstanceState.LiteLoader, ModMinecraft.McInstance.McInstanceState.Fabric }, ModMinecraft.McInstanceCardType.API)
                Dim list3 As List(Of ModMinecraft.McInstance) = New List(Of ModMinecraft.McInstance)()
                Dim list4 As List(Of ModMinecraft.McInstance) = New List(Of ModMinecraft.McInstance)()
                ModMinecraft.McInstanceFilter(list, New ModMinecraft.McInstance.McInstanceState() { ModMinecraft.McInstance.McInstanceState.Old }, list4)
                Dim enumerable As IEnumerable(Of ModMinecraft.McInstance) = list
                Dim func As Func(Of ModMinecraft.McInstance, Boolean)
                If ModMinecraft._Closure$__.$I24-0 IsNot Nothing Then
                    func = ModMinecraft._Closure$__.$I24-0
                Else
                    Dim func2 As Func(Of ModMinecraft.McInstance, Boolean) = Function(v As ModMinecraft.McInstance) v.State = ModMinecraft.McInstance.McInstanceState.Original OrElse v.State = ModMinecraft.McInstance.McInstanceState.Snapshot
                    func = func2
                    ModMinecraft._Closure$__.$I24-0 = func2
                End If
                Dim enumerable2 As IEnumerable(Of ModMinecraft.McInstance) = enumerable.Where(func)
                Dim func3 As Func(Of ModMinecraft.McInstance, DateTime)
                If ModMinecraft._Closure$__.$I24-1 IsNot Nothing Then
                    func3 = ModMinecraft._Closure$__.$I24-1
                Else
                    Dim func4 As Func(Of ModMinecraft.McInstance, DateTime) = Function(v As ModMinecraft.McInstance) v.ReleaseTime
                    func3 = func4
                    ModMinecraft._Closure$__.$I24-1 = func4
                End If
                Dim mcInstance3 As ModMinecraft.McInstance = enumerable2.MaxBy(func3)
                If mcInstance3 IsNot Nothing AndAlso mcInstance3.State = ModMinecraft.McInstance.McInstanceState.Snapshot Then
                    list3.Add(mcInstance3)
                    list.Remove(mcInstance3)
                End If
                ModMinecraft.McInstanceFilter(list, New ModMinecraft.McInstance.McInstanceState() { ModMinecraft.McInstance.McInstanceState.Snapshot }, list4)
                Dim dictionary3 As Dictionary(Of String, ModMinecraft.McInstance) = New Dictionary(Of String, ModMinecraft.McInstance)()
                Dim list5 As List(Of Integer) = New List(Of Integer)()
                Try
                    For Each mcInstance4 As ModMinecraft.McInstance In list
                        If mcInstance4.Version.Valid Then
                            If Not list5.Contains(mcInstance4.Version.Drop) Then
                                list5.Add(mcInstance4.Version.Drop)
                            End If
                            Dim text2 As String = Conversions.ToString(mcInstance4.Version.Drop) + "-" + Conversions.ToString(CInt(mcInstance4.State))
                            If Not dictionary3.ContainsKey(text2) Then
                                dictionary3.Add(text2, mcInstance4)
                            ElseIf mcInstance4.State = ModMinecraft.McInstance.McInstanceState.OptiFine Then
                                If mcInstance4.Version.OptiFineCode > dictionary3(text2).Version.OptiFineCode Then
                                    dictionary3(text2) = mcInstance4
                                End If
                            ElseIf DateTime.Compare(mcInstance4.ReleaseTime, dictionary3(text2).ReleaseTime) > 0 Then
                                dictionary3(text2) = mcInstance4
                            End If
                        End If
                    Next
                Finally
                    Dim enumerator2 As List(Of ModMinecraft.McInstance).Enumerator
                    CType(enumerator2, IDisposable).Dispose()
                End Try
                Try
                    For Each num As Integer In list5
                        If dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(4)) AndAlso dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(1)) Then
                            Dim mcInstance5 As ModMinecraft.McInstance = dictionary3(Conversions.ToString(num) + "-" + Conversions.ToString(1))
                            Dim mcInstance6 As ModMinecraft.McInstance = dictionary3(Conversions.ToString(num) + "-" + Conversions.ToString(4))
                            If mcInstance5.Version.Drop > mcInstance6.Version.Drop Then
                                list3.Add(mcInstance5)
                                list.Remove(mcInstance5)
                            End If
                            list3.Add(mcInstance6)
                            list.Remove(mcInstance6)
                        ElseIf dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(4)) Then
                            list3.Add(dictionary3(Conversions.ToString(num) + "-" + Conversions.ToString(4)))
                            list.Remove(dictionary3(Conversions.ToString(num) + "-" + Conversions.ToString(4)))
                        ElseIf dictionary3.ContainsKey(Conversions.ToString(num) + "-" + Conversions.ToString(1)) Then
                            list3.Add(dictionary3(Conversions.ToString(num) + "-" + Conversions.ToString(1)))
                            list.Remove(dictionary3(Conversions.ToString(num) + "-" + Conversions.ToString(1)))
                        End If
                    Next
                Finally
                    Dim enumerator3 As List(Of Integer).Enumerator
                    CType(enumerator3, IDisposable).Dispose()
                End Try
                list4.AddRange(list)
                If list3.Any() Then
                    dictionary2.Add(ModMinecraft.McInstanceCardType.OriginalLike, list3)
                End If
                If list4.Any() Then
                    dictionary2.Add(ModMinecraft.McInstanceCardType.Rubbish, list4)
                End If
                Try
                    For Each keyValuePair As KeyValuePair(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) In dictionary2
                        Try
                            For Each mcInstance7 As ModMinecraft.McInstance In keyValuePair.Value
                                Dim mcInstanceCardType As ModMinecraft.McInstanceCardType = If((mcInstance7.DisplayType = ModMinecraft.McInstanceCardType.Auto OrElse keyValuePair.Key = ModMinecraft.McInstanceCardType.Star), keyValuePair.Key, mcInstance7.DisplayType)
                                If Not dictionary.ContainsKey(mcInstanceCardType) Then
                                    dictionary.Add(mcInstanceCardType, New List(Of ModMinecraft.McInstance)())
                                End If
                                dictionary(mcInstanceCardType).Add(mcInstance7)
                            Next
                        Finally
                            Dim enumerator5 As List(Of ModMinecraft.McInstance).Enumerator
                            CType(enumerator5, IDisposable).Dispose()
                        End Try
                    Next
                Finally
                    Dim enumerator4 As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)).Enumerator
                    CType(enumerator4, IDisposable).Dispose()
                End Try
            Catch ex As Exception
                dictionary.Clear()
                Logger.[Error](ex, "分类版本列表失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            End Try
            Dim dictionary4 As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) = New Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))()
            Dim array As ModMinecraft.McInstanceCardType() = New ModMinecraft.McInstanceCardType() { ModMinecraft.McInstanceCardType.Star, ModMinecraft.McInstanceCardType.API, ModMinecraft.McInstanceCardType.OriginalLike, ModMinecraft.McInstanceCardType.Rubbish, ModMinecraft.McInstanceCardType.Fool, ModMinecraft.McInstanceCardType.[Error], ModMinecraft.McInstanceCardType.Hidden }
            For j As Integer = 0 To array.Length - 1
                Dim text3 As String = Conversions.ToString(CInt(array(j)))
                If dictionary.ContainsKey(CType(Conversions.ToInteger(text3), ModMinecraft.McInstanceCardType)) Then
                    dictionary4.Add(CType(Conversions.ToInteger(text3), ModMinecraft.McInstanceCardType), dictionary(CType(Conversions.ToInteger(text3), ModMinecraft.McInstanceCardType)))
                End If
            Next
            dictionary = dictionary4
            For Each mcInstanceCardType2 As ModMinecraft.McInstanceCardType In New ModMinecraft.McInstanceCardType() { ModMinecraft.McInstanceCardType.Star, ModMinecraft.McInstanceCardType.API, ModMinecraft.McInstanceCardType.OriginalLike, ModMinecraft.McInstanceCardType.Rubbish, ModMinecraft.McInstanceCardType.Fool }
                Dim CS$<>8__locals1 As ModMinecraft._Closure$__24-0 = New ModMinecraft._Closure$__24-0(CS$<>8__locals1)
                If dictionary.ContainsKey(mcInstanceCardType2) Then
                    Dim CS$<>8__locals2 As ModMinecraft._Closure$__24-0 = CS$<>8__locals1
                    Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_21(Of ModMinecraft.McInstance, Integer)
                    If ModMinecraft._Closure$__.$I24-2 IsNot Nothing Then
                        vb$AnonymousDelegate_ = ModMinecraft._Closure$__.$I24-2
                    Else
                        Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_21(Of ModMinecraft.McInstance, Integer) = Function(Instance As ModMinecraft.McInstance)
                            Dim num3 As Integer
                            If Instance.Version.ForgelikeCode > 0 Then
                                num3 = Instance.Version.ForgelikeCode
                            ElseIf Instance.Version.HasOptiFine Then
                                num3 = Instance.Version.OptiFineCode
                            Else
                                num3 = 0
                            End If
                            Return num3
                        End Function
                        vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                        ModMinecraft._Closure$__.$I24-2 = vb$AnonymousDelegate_2
                    End If
                    CS$<>8__locals2.$VB$Local_GetComponentCode = vb$AnonymousDelegate_
                    dictionary(mcInstanceCardType2) = dictionary(mcInstanceCardType2).SortByComparison(Function(Left As ModMinecraft.McInstance, Right As ModMinecraft.McInstance)
                        Dim flag As Boolean
                        If(Left.ReleaseTime.Year >= 2000 OrElse Right.ReleaseTime.Year >= 2000) AndAlso DateTime.Compare(Left.ReleaseTime, Right.ReleaseTime) <> 0 Then
                            flag = DateTime.Compare(Left.ReleaseTime, Right.ReleaseTime) > 0
                        ElseIf Left.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) <> Right.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) Then
                            flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.Fabric)
                        ElseIf Left.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) <> Right.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) Then
                            flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge)
                        ElseIf Left.Version.ModLoaders.HasFlagF(ModLoaders.Forge) <> Right.Version.ModLoaders.HasFlagF(ModLoaders.Forge) Then
                            flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.Forge)
                        ElseIf Left.Version.HasOptiFine <> Right.Version.HasOptiFine Then
                            flag = Left.Version.HasOptiFine
                        ElseIf Left.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) <> Right.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) Then
                            flag = Left.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader)
                        ElseIf CS$<>8__locals1.$VB$Local_GetComponentCode(Left) <> CS$<>8__locals1.$VB$Local_GetComponentCode(Right) Then
                            flag = CS$<>8__locals1.$VB$Local_GetComponentCode(Left) > CS$<>8__locals1.$VB$Local_GetComponentCode(Right)
                        Else
                            flag = Operators.CompareString(Left.Name, Right.Name, False) > 0
                        End If
                        Return flag
                    End Function)
                End If
            Next
            ModBase.WriteIni(Folder + "PCL.ini", "CardCount", Conversions.ToString(dictionary.Count))
            Dim num2 As Integer = dictionary.Count - 1
            For l As Integer = 0 To num2
                ModBase.WriteIni(Folder + "PCL.ini", "CardKey" + Conversions.ToString(l + 1), Conversions.ToString(CInt(dictionary.Keys.ElementAtOrDefault(l))))
                Dim text4 As String = ""
                Try
                    For Each mcInstance8 As ModMinecraft.McInstance In dictionary.Values.ElementAtOrDefault(l)
                        text4 = text4 + mcInstance8.Name + ":"
                    Next
                Finally
                    Dim enumerator6 As List(Of ModMinecraft.McInstance).Enumerator
                    CType(enumerator6, IDisposable).Dispose()
                End Try
                ModBase.WriteIni(Folder + "PCL.ini", "CardValue" + Conversions.ToString(l + 1), text4)
            Next
            Return dictionary
        End Function

        Private Sub McInstanceFilter(ByRef InstanceList As List(Of ModMinecraft.McInstance), ByRef Target As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)), Formula As ModMinecraft.McInstance.McInstanceState(), CardType As ModMinecraft.McInstanceCardType)
            Dim list As List(Of ModMinecraft.McInstance) = InstanceList.Where(Function(v As ModMinecraft.McInstance) Formula.Contains(v.State)).ToList()
            If list.Any() Then
                Target.Add(CardType, list)
                InstanceList = InstanceList.Except(list).ToList()
            End If
        End Sub

        Private Sub McInstanceFilter(ByRef InstanceList As List(Of ModMinecraft.McInstance), Formula As ModMinecraft.McInstance.McInstanceState(), ByRef KeepList As List(Of ModMinecraft.McInstance))
            KeepList.AddRange(InstanceList.Where(Function(v As ModMinecraft.McInstance) Formula.Contains(v.State)))
            If KeepList.Any() Then
                InstanceList = InstanceList.Except(KeepList).ToList()
            End If
        End Sub

        Public Function McSkinSelect() As ModMinecraft.McSkinInfo
            Dim text As String = Dialogs.SelectFile("选择皮肤文件", False, Nothing, New OneOf(Of IEnumerable(Of ValueTuple(Of String(), String)), String)?(New ValueTuple(Of String(), String)() { New ValueTuple(Of String(), String)(New String() { "png", "jpg", "jpeg", "webp" }, "皮肤文件") })).FirstOrDefault()
            Dim mcSkinInfo As ModMinecraft.McSkinInfo
            If String.IsNullOrEmpty(text) Then
                mcSkinInfo = New ModMinecraft.McSkinInfo() With { .IsVaild = False }
            Else
                Try
                    Dim myBitmap As OurBitmap = New OurBitmap(text)
                    If myBitmap.Pic.Width <> 64 OrElse (myBitmap.Pic.Height <> 32 AndAlso myBitmap.Pic.Height <> 64) Then
                        ModMain.Hint("皮肤图片大小应为 64x32 像素或 64x64 像素！", ModMain.HintType.Red, True)
                        Return New ModMinecraft.McSkinInfo() With { .IsVaild = False }
                    End If
                    Dim info As FileInfo = FileUtils.GetInfo(text)
                    If info.Length > 24576L Then
                        ModMain.Hint("皮肤文件大小需小于 24 KB，而所选文件大小为 " + Conversions.ToString(Math.Round(CDbl(info.Length) / 1024.0, 2)) + " KB", ModMain.HintType.Red, True)
                        Return New ModMinecraft.McSkinInfo() With { .IsVaild = False }
                    End If
                Catch ex As Exception
                    Logger.[Error](ex, "皮肤文件存在错误", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    Return New ModMinecraft.McSkinInfo() With { .IsVaild = False }
                End Try
                Dim num As Integer = ModMain.OurMsgBox("此皮肤为 Steve 模型（粗手臂）还是 Alex 模型（细手臂）？", "选择皮肤种类", "Steve 模型", "Alex 模型", "我不知道", False, False, False, Nothing, Nothing, Nothing)
                If num = 3 Then
                    ModMain.Hint("请在皮肤下载页面确认皮肤种类后再使用此皮肤！", ModMain.HintType.Blue, True)
                    mcSkinInfo = New ModMinecraft.McSkinInfo() With { .IsVaild = False }
                Else
                    mcSkinInfo = New ModMinecraft.McSkinInfo() With { .IsVaild = True, .IsSlim = (num = 2), .LocalFile = text }
                End If
            End If
            Return mcSkinInfo
        End Function

        Public Function McSkinGetAddress(UUID As String, Type As String) As String
            If Operators.CompareString(UUID, "", False) = 0 Then
                Throw New Exception("UUID 为空。")
            End If
            If UUID.StartsWithF("00000", False) AndAlso Operators.CompareString(Type, "Auth", False) <> 0 Then
                Throw New Exception("离线 UUID 无正版皮肤文件：" + UUID)
            End If
            Dim text As String = ModBase.ReadIni(ModBase.PathTemp + "Cache\Skin\Index" + Type + ".ini", UUID, "")
            Dim text2 As String
            If Operators.CompareString(text, "", False) <> 0 Then
                text2 = text
            Else
                Dim text3 As String
                If Operators.CompareString(Type, "Mojang", False) <> 0 AndAlso Operators.CompareString(Type, "Ms", False) <> 0 Then
                    If Operators.CompareString(Type, "Nide", False) <> 0 Then
                        If Operators.CompareString(Type, "Auth", False) <> 0 Then
                            Throw New ArgumentException("皮肤地址种类无效：" + (If(Type, "null")))
                        End If
                        text3 = If((ModMinecraft.McInstanceSelected Is Nothing), Settings.[Get](Of String)("CacheAuthServerServer", Nothing), Settings.[Get](Of String)("VersionServerAuthServer", ModMinecraft.McInstanceSelected)) + "/sessionserver/session/minecraft/profile/"
                    Else
                        text3 = "https://auth.mc-user.com:233/" + If((ModMinecraft.McInstanceSelected Is Nothing), Settings.[Get](Of String)("CacheNideServer", Nothing), Settings.[Get](Of String)("VersionServerNide", ModMinecraft.McInstanceSelected)) + "/sessionserver/session/minecraft/profile/"
                    End If
                Else
                    text3 = "https://sessionserver.mojang.com/session/minecraft/profile/"
                End If
                Dim text4 As String = ModNet.NetRequestByClientRetry(text3 + UUID, Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False)
                If Operators.CompareString(text4, "", False) = 0 Then
                    Throw New Exception("皮肤返回值为空，可能是未设置自定义皮肤的用户")
                End If
                Dim text5 As String
                Try
                    Try
                        For Each jtoken As JToken In text4.DeserializeJson()("properties")
                            If CStr(jtoken("name")) = "textures" Then
                                text5 = CStr(jtoken("value"))
                                GoTo IL_021B
                            End If
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of JToken)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    Throw New Exception("未从皮肤返回值中找到符合条件的 Property")
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("无法完成解析的皮肤返回值，可能是未设置自定义皮肤的用户：{0}", text4), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    Throw New Exception("皮肤返回值中不包含皮肤数据项，可能是未设置自定义皮肤的用户", ex)
                End Try
                IL_021B:
                text4 = Encoding.UTF8.GetString(Convert.FromBase64String(text5))
                Dim jobject As JObject = CType(text4.Lower().DeserializeJson(), JObject)
                If jobject("textures") Is Nothing OrElse jobject("textures")("skin") Is Nothing OrElse jobject("textures")("skin")("url") Is Nothing Then
                    Throw New Exception("用户未设置自定义皮肤")
                End If
                Dim text6 As String = jobject("textures")("skin")("url").ToString()
                text5 = If(text6.Contains("minecraft.net/"), text6.Replace("http://", "https://"), text6)
                ModBase.WriteIni(ModBase.PathTemp + "Cache\Skin\Index" + Type + ".ini", UUID, text5)
                Logger.Info(String.Format("UUID {0} 对应的皮肤文件为 {1}", UUID, text5), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                text2 = text5
            End If
            Return text2
        End Function

        Public Function McSkinDownload(Address As String) As String
            PathUtils.GetLastPart(Address)
            Dim text As String = ModBase.PathTemp + "Cache\Skin\" + Conversions.ToString(Address.GetStableHashCode()) + ".png"
            Dim mcSkinDownloadLock As Object = ModMinecraft.McSkinDownloadLock
            ObjectFlowControl.CheckForSyncLockOnValueType(mcSkinDownloadLock)
            Dim text2 As String
            SyncLock mcSkinDownloadLock
                If Not FileUtils.Exists(text) Then
                    ModNet.NetDownloadByClient(Address, text + ".PCLDownloading", False)
                    FileUtils.Move(text + ".PCLDownloading", text, False)
                    Logger.Info(String.Format("皮肤下载成功：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End If
                text2 = text
            End SyncLock
            Return text2
        End Function

        Public Function McSkinSex(Uuid As String) As String
            Dim text As String
            If Uuid.Length <> 32 Then
                text = "Steve"
            Else
                Dim num As Integer = Integer.Parse(Conversions.ToString(Uuid(7)), NumberStyles.AllowHexSpecifier)
                Dim num2 As Integer = Integer.Parse(Conversions.ToString(Uuid(15)), NumberStyles.AllowHexSpecifier)
                Dim num3 As Integer = Integer.Parse(Conversions.ToString(Uuid(23)), NumberStyles.AllowHexSpecifier)
                Dim num4 As Integer = Integer.Parse(Conversions.ToString(Uuid(31)), NumberStyles.AllowHexSpecifier)
                text = If(((num Xor num2 Xor num3 Xor num4) Mod 2 <> 0), "Alex", "Steve")
            End If
            Return text
        End Function

        Public Function McJsonRuleCheck(RuleToken As JToken) As Boolean
            Dim flag As Boolean
            If RuleToken Is Nothing Then
                flag = True
            Else
                Dim flag2 As Boolean = False
                Try
                    For Each jtoken As JToken In RuleToken
                        Dim flag3 As Boolean = True
                        If jtoken("os") IsNot Nothing Then
                            If jtoken("os")("name") IsNot Nothing Then
                                Dim text As String = jtoken("os")("name").ToString()
                                If Operators.CompareString(text, "unknown", False) <> 0 Then
                                    If Operators.CompareString(text, "windows", False) = 0 Then
                                        If jtoken("os")("version") IsNot Nothing Then
                                            flag3 = flag3 AndAlso ModMinecraft.OSVersion.RegexCheck(jtoken("os")("version").ToString(), RegexOptions.None)
                                        End If
                                    Else
                                        flag3 = False
                                    End If
                                End If
                            End If
                            If jtoken("os")("arch") IsNot Nothing Then
                                flag3 = flag3 AndAlso Operators.CompareString(jtoken("os")("arch").ToString(), "x86", False) <> 0 AndAlso Not jtoken("os")("arch").Contains("arm")
                            End If
                        End If
                        If Not Information.IsNothing(jtoken("features")) Then
                            flag3 = flag3 AndAlso Information.IsNothing(jtoken("features")("is_demo_user"))
                            Dim enumerable As IEnumerable(Of JToken) = CType(CType(jtoken("features"), JObject).Children(), IEnumerable(Of JToken))
                            Dim func As Func(Of JToken, Boolean)
                            If ModMinecraft._Closure$__.$IR35-2 IsNot Nothing Then
                                func = ModMinecraft._Closure$__.$IR35-2
                            Else
                                Dim func2 As Func(Of JToken, Boolean) = Function(a0 As JToken)
                                    Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_22(Of JProperty, Boolean)
                                    If ModMinecraft._Closure$__.$I35-0 IsNot Nothing Then
                                        vb$AnonymousDelegate_ = ModMinecraft._Closure$__.$I35-0
                                    Else
                                        Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_22(Of JProperty, Boolean) = Function(j As JProperty) j.Name.Contains("quick_play")
                                        vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                                        ModMinecraft._Closure$__.$I35-0 = vb$AnonymousDelegate_2
                                    End If
                                    Return vb$AnonymousDelegate_(CType(a0, JProperty))
                                End Function
                                func = func2
                                ModMinecraft._Closure$__.$IR35-2 = func2
                            End If
                            If enumerable.Any(func) Then
                                flag3 = False
                            End If
                        End If
                        If Operators.CompareString(jtoken("action").ToString(), "allow", False) = 0 Then
                            If flag3 Then
                                flag2 = True
                            End If
                        ElseIf flag3 Then
                            flag2 = False
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                flag = flag2
            End If
            Return flag
        End Function

        Public Function McLibListGet(Instance As ModMinecraft.McInstance, IncludeMainJar As Boolean) As List(Of ModMinecraft.McLibEntry)
            Logger.Info(String.Format("获取支持库列表：{0}", Instance.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            Dim list As List(Of ModMinecraft.McLibEntry) = ModMinecraft.McLibListGetWithJson(Instance.JsonObject, False, Nothing)
            If IncludeMainJar Then
                Dim jtoken As JToken = Instance.JsonObject("jar")
                Dim text As String = If((jtoken IsNot Nothing), jtoken.ToString(), Nothing)
                Dim mcInstance2 As ModMinecraft.McInstance
                If Instance.IsHmclFormatJson OrElse text Is Nothing Then
                    Dim mcInstance As ModMinecraft.McInstance = Instance
                    If Instance.Version.ModLoaders.HasFlagF(ModLoaders.Forge) OrElse Instance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) Then
                        If Instance.Version.Vanilla.Major >= 17 Then
                            GoTo IL_00E6
                        End If
                    End If
                    While Operators.CompareString(mcInstance.InheritName, "", False) <> 0 AndAlso Operators.CompareString(mcInstance.InheritName, mcInstance.Name, False) <> 0
                        mcInstance = New ModMinecraft.McInstance(ModMinecraft.McFolderSelected + "versions\" + mcInstance.InheritName + "\")
                    End While
                    IL_00E6:
                    mcInstance2 = New ModMinecraft.McInstance(mcInstance.PathVersion)
                Else
                    mcInstance2 = New ModMinecraft.McInstance(text)
                End If
                If Not FileUtils.Exists(mcInstance2.PathVersion + mcInstance2.Name + ".json") Then
                    mcInstance2 = Instance
                    Logger.Warn(String.Format("可能缺少前置版本 {0}，找不到对应的 json 文件", mcInstance2.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End If
                Dim text2 As String
                Dim text3 As String
                If mcInstance2.JsonObject("downloads") IsNot Nothing AndAlso mcInstance2.JsonObject("downloads")("client") IsNot Nothing Then
                    text2 = CStr(mcInstance2.JsonObject("downloads")("client")("url"))
                    text3 = CStr(mcInstance2.JsonObject("downloads")("client")("sha1"))
                Else
                    text2 = Nothing
                    text3 = Nothing
                End If
                list.Add(New ModMinecraft.McLibEntry() With { .LocalPath = mcInstance2.PathVersion + mcInstance2.Name + ".jar", .Size = 0L, .IsNatives = False, .Url = text2, .SHA1 = text3 })
            End If
            Return list
        End Function

        Public Function McLibListGetWithJson(JsonObject As JObject, Optional KeepSameNameDifferentVersionResult As Boolean = False, Optional CustomMcFolder As String = Nothing) As List(Of ModMinecraft.McLibEntry)
            CustomMcFolder = If(CustomMcFolder, ModMinecraft.McFolderSelected)
            Dim list As List(Of ModMinecraft.McLibEntry) = New List(Of ModMinecraft.McLibEntry)()
            Try
                For Each jtoken As JToken In CType(JsonObject("libraries"), JArray).Children()
                    Dim jobject As JObject = CType(jtoken, JObject)
                    For i As Integer = jobject.Properties().Count() - 1 To 0 Step -1
                        If jobject.Properties().ElementAtOrDefault(i).Value.Type = 10 Then
                            jobject.Remove(jobject.Properties().ElementAtOrDefault(i).Name)
                        End If
                    Next
                    If ModMinecraft.McJsonRuleCheck(jobject("rules")) Then
                        Dim text As String = CStr(jobject("url"))
                        If text IsNot Nothing Then
                            text += ModMinecraft.McLibGet(CStr(jobject("name")), False, True, CustomMcFolder).Replace("\", "/")
                        End If
                        If jobject("natives") Is Nothing Then
                            Dim text2 As String = ModMinecraft.McLibGet(CStr(jobject("name")), True, False, CustomMcFolder)
                            Try
                                If jobject("downloads") IsNot Nothing AndAlso jobject("downloads")("artifact") IsNot Nothing Then
                                    Dim list2 As List(Of ModMinecraft.McLibEntry) = list
                                    Dim mcLibEntry As ModMinecraft.McLibEntry = New ModMinecraft.McLibEntry()
                                    mcLibEntry.OriginalName = CStr(jobject("name"))
                                    mcLibEntry.Url = CStr((If(jobject("downloads")("artifact")("url"), text)))
                                    mcLibEntry.LocalPath = If((jobject("downloads")("artifact")("path") Is Nothing), ModMinecraft.McLibGet(CStr(jobject("name")), True, False, CustomMcFolder), (CustomMcFolder + "libraries\" + jobject("downloads")("artifact")("path").ToString().Replace("/", "\")))
                                    mcLibEntry.Size = CLng(Math.Round(ModBase.Val(jobject("downloads")("artifact")("size").ToString())))
                                    mcLibEntry.IsNatives = False
                                    Dim mcLibEntry2 As ModMinecraft.McLibEntry = mcLibEntry
                                    Dim jtoken2 As JToken = jobject("downloads")("artifact")("sha1")
                                    mcLibEntry2.SHA1 = If((jtoken2 IsNot Nothing), jtoken2.ToString(), Nothing)
                                    list2.Add(mcLibEntry)
                                Else
                                    list.Add(New ModMinecraft.McLibEntry() With { .OriginalName = CStr(jobject("name")), .Url = text, .LocalPath = text2, .Size = 0L, .IsNatives = False, .SHA1 = Nothing })
                                End If
                                Continue For
                            Catch ex As Exception
                                Logger.Warn(ex, String.Format("处理实际支持库列表失败（无 Natives，{0}）", If(jobject("name"), "Nothing")), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                list.Add(New ModMinecraft.McLibEntry() With { .OriginalName = CStr(jobject("name")), .Url = text, .LocalPath = text2, .Size = 0L, .IsNatives = False, .SHA1 = Nothing })
                                Continue For
                            End Try
                        End If
                        If jobject("natives")("windows") IsNot Nothing Then
                            Dim text3 As String = CStr(jobject("natives")("windows"))
                            Try
                                If jobject("downloads") IsNot Nothing AndAlso jobject("downloads")("classifiers") IsNot Nothing AndAlso jobject("downloads")("classifiers")(text3) IsNot Nothing Then
                                    Dim jtoken3 As JToken = jobject("downloads")("classifiers")(text3)
                                    list.Add(New ModMinecraft.McLibEntry() With { .OriginalName = CStr(jobject("name")), .Url = CStr((If(jtoken3("url"), text))), .LocalPath = If((jtoken3("path") Is Nothing), ModMinecraft.McLibGet(CStr(jobject("name")), True, False, CustomMcFolder).Replace(".jar", "-" + text3 + ".jar").Replace("${arch}", If(Environment.Is64BitOperatingSystem, "64", "32")), String.Format("{0}libraries\{1}", CustomMcFolder, jtoken3("path").ToString().Replace("/", "\"))), .Size = CLng(Math.Round(ModBase.Val(jtoken3("size").ToString()))), .IsNatives = True, .SHA1 = jtoken3("sha1").ToString() })
                                Else
                                    list.Add(New ModMinecraft.McLibEntry() With { .OriginalName = CStr(jobject("name")), .Url = text, .Size = 0L, .IsNatives = True, .SHA1 = Nothing, .LocalPath = ModMinecraft.McLibGet(CStr(jobject("name")), True, False, CustomMcFolder).Replace(".jar", String.Format("-{0}.jar", text3)).Replace("${arch}", If(Environment.Is64BitOperatingSystem, "64", "32")) })
                                End If
                            Catch ex2 As Exception
                                Logger.Warn(ex2, String.Format("处理实际支持库列表失败（有 Natives，{0}）", If(jobject("name"), "Nothing")), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                list.Add(New ModMinecraft.McLibEntry() With { .OriginalName = CStr(jobject("name")), .Url = text, .Size = 0L, .IsNatives = True, .SHA1 = Nothing, .LocalPath = ModMinecraft.McLibGet(CStr(jobject("name")), True, False, CustomMcFolder).Replace(".jar", String.Format("-{0}.jar", text3)).Replace("${arch}", If(Environment.Is64BitOperatingSystem, "64", "32")) })
                            End Try
                        End If
                    End If
                Next
            Finally
                Dim enumerator As IEnumerator(Of JToken)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Dim dictionary As Dictionary(Of String, ModMinecraft.McLibEntry) = New Dictionary(Of String, ModMinecraft.McLibEntry)()
            Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_23(Of ModMinecraft.McLibEntry, String)
            If ModMinecraft._Closure$__.$I38-0 IsNot Nothing Then
                vb$AnonymousDelegate_ = ModMinecraft._Closure$__.$I38-0
            Else
                Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_23(Of ModMinecraft.McLibEntry, String) = Function(Token As ModMinecraft.McLibEntry) PathUtils.GetLastPart(PathUtils.RemoveLastPart(Token.LocalPath))
                vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                ModMinecraft._Closure$__.$I38-0 = vb$AnonymousDelegate_2
            End If
            Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_23(Of ModMinecraft.McLibEntry, String) = vb$AnonymousDelegate_
            Dim num As Integer = list.Count - 1
            For j As Integer = 0 To num
                Dim text4 As String = list(j).Name + list(j).IsNatives.ToString()
                If dictionary.ContainsKey(text4) Then
                    Dim text5 As String = vb$AnonymousDelegate_3(list(j))
                    Dim text6 As String = vb$AnonymousDelegate_3(dictionary(text4))
                    If Operators.CompareString(text5, text6, False) <> 0 AndAlso KeepSameNameDifferentVersionResult Then
                        Logger.Info(String.Format("发现疑似重复的支持库：{0} ({1}) 与 {2} ({3})", New Object() { list(j), text5, dictionary(text4), text6 }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        dictionary.Add(text4 + Conversions.ToString(ModBase.GetUuid()), list(j))
                    Else
                        Logger.Info(String.Format("发现重复的支持库：{0} ({1}) 与 {2} ({3})，已忽略其中之一", New Object() { list(j), text5, dictionary(text4), text6 }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        If McUtils.CompareVersionGE(text5, text6) Then
                            dictionary(text4) = list(j)
                        End If
                    End If
                Else
                    dictionary.Add(text4, list(j))
                End If
            Next
            Return dictionary.Values.ToList()
        End Function

        Public Function McLibNetFilesFromInstance(Instance As ModMinecraft.McInstance) As List(Of ModNet.NetFile)
            If Not Instance.IsLoaded Then
                Instance.Load()
            End If
            Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
            Try
                Dim netFile As ModNet.NetFile = ModDownload.DlClientJarGet(Instance, True)
                If netFile IsNot Nothing Then
                    list.Add(netFile)
                End If
            Catch ex As Exception
                Logger.Warn(ex, "版本缺失主 jar 文件所必须的信息", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            End Try
            list.AddRange(ModMinecraft.McLibNetFilesFromTokens(ModMinecraft.McLibListGet(Instance, False), Nothing))
            If Settings.[Get](Of Integer)("VersionServerLogin", Instance) = 3 Then
                Dim text As String = ModBase.PathPure.Value + "nide8auth.jar"
                Try
                    Logger.Info("开始获取统一通行证下载信息", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    Dim jobject As JObject = CType(ModNet.NetRequestByClientRetry("https://auth.mc-user.com:233/" + Settings.[Get](Of String)("VersionServerNide", Instance), Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
                    list.Add(New ModNet.NetFile(New String() { "https://login.mc-user.com:233/index/jar" }, text, New FileChecker() With { .Hash = jobject("jarHash").ToString() }, False))
                Catch ex2 As Exception
                    If Not FileUtils.Exists(text) Then
                        Throw New Exception("获取统一通行证下载信息失败", ex2)
                    End If
                    Logger.Warn(ex2, "获取统一通行证下载信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End Try
            End If
            If Settings.[Get](Of Integer)("VersionServerLogin", Instance) = 4 Then
                Dim text2 As String = ModBase.PathPure.Value + "authlib-injector.jar"
                Try
                    Logger.Info("开始获取 Authlib-Injector 下载信息", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    Dim jobject2 As JObject = CType(ModNet.NetRequestByClientRetry("https://authlib-injector.yushi.moe/artifact/latest.json", Nothing, "https://bmclapi2.bangbang93.com/mirrors/authlib-injector/artifact/latest.json", Nothing, Nothing, "*/*", Nothing, True, Nothing, False).DeserializeJson(), JObject)
                    Dim text3 As String = jobject2("download_url").ToString().Replace("bmclapi2.bangbang93.com/mirrors/authlib-injector", "authlib-injector.yushi.moe")
                    list.Add(New ModNet.NetFile(New String() { text3, text3.Replace("authlib-injector.yushi.moe", "bmclapi2.bangbang93.com/mirrors/authlib-injector") }, text2, New FileChecker() With { .Hash = jobject2("checksums")("sha256").ToString() }, False))
                Catch ex3 As Exception
                    If Not FileUtils.Exists(text2) Then
                        Throw New Exception("获取 Authlib-Injector 下载信息失败", ex3)
                    End If
                    Logger.Warn(ex3, "获取 Authlib-Injector 下载信息失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End Try
            End If
            If Conversions.ToBoolean(ModMinecraft.ShouldIgnoreFileCheck(Instance)) Then
                Logger.Warn("设置要求尽量忽略文件检查，这可能会保留有误的文件", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                Dim enumerable As IEnumerable(Of ModNet.NetFile) = list
                Dim func As Func(Of ModNet.NetFile, Boolean)
                If ModMinecraft._Closure$__.$I39-0 IsNot Nothing Then
                    func = ModMinecraft._Closure$__.$I39-0
                Else
                    Dim func2 As Func(Of ModNet.NetFile, Boolean) = Function(f As ModNet.NetFile)
                        Dim flag As Boolean
                        If FileUtils.Exists(f.LocalPath) Then
                            Logger.Warn(String.Format("跳过下载的支持库文件：{0}", f.LocalPath), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                            flag = False
                        Else
                            flag = True
                        End If
                        Return flag
                    End Function
                    func = func2
                    ModMinecraft._Closure$__.$I39-0 = func2
                End If
                list = enumerable.Where(func).ToList()
            End If
            Return list
        End Function

        Public Function McLibNetFilesFromTokens(Libs As List(Of ModMinecraft.McLibEntry), Optional CustomMcFolder As String = Nothing) As List(Of ModNet.NetFile)
            CustomMcFolder = If(CustomMcFolder, ModMinecraft.McFolderSelected)
            Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
            Try
                For Each mcLibEntry As ModMinecraft.McLibEntry In Libs
                    Dim fileChecker As FileChecker
                    If mcLibEntry.Name.ContainsIgnoreCase("labymod") Then
                        fileChecker = New FileChecker()
                    Else
                        fileChecker = New FileChecker() With { .ActualSize = If((mcLibEntry.Size = 0L), (-1L), mcLibEntry.Size), .Hash = mcLibEntry.SHA1 }
                    End If
                    Dim list2 As List(Of String) = New List(Of String)()
                    If mcLibEntry.Url Is Nothing AndAlso Operators.CompareString(mcLibEntry.Name, "net.minecraftforge:forge:universal", False) = 0 Then
                        mcLibEntry.Url = "https://maven.minecraftforge.net" + mcLibEntry.LocalPath.Replace(CustomMcFolder + "libraries", "").Replace("\", "/")
                    End If
                    If mcLibEntry.Url IsNot Nothing Then
                        list2.Add(mcLibEntry.Url)
                        If mcLibEntry.Url.Contains("launcher.mojang.com/v1/objects") OrElse mcLibEntry.Url.Contains("client.txt") OrElse mcLibEntry.Url.Contains(".tsrg") Then
                            list2.AddRange(ModDownload.DlSourceLauncherOrMetaGet(mcLibEntry.Url))
                        End If
                        If mcLibEntry.Url.Contains("maven") Then
                            Dim text As String = mcLibEntry.Url.Replace(mcLibEntry.Url.Substring(0, mcLibEntry.Url.IndexOfF("maven", False)), "https://bmclapi2.bangbang93.com/").Replace("maven.fabricmc.net", "maven").Replace("maven.minecraftforge.net", "maven").Replace("maven.neoforged.net/releases", "maven")
                            If ModDownload.DlSourcePreferMojang Then
                                list2.Add(text)
                            Else
                                list2.Insert(0, text)
                            End If
                        End If
                    End If
                    If mcLibEntry.LocalPath.Contains("transformer-discovery-service") Then
                        If Not FileUtils.Exists(mcLibEntry.LocalPath) Then
                            ModBase.ExtractResources(mcLibEntry.LocalPath, "Transformer")
                        End If
                        Logger.Warn("已自动释放 Transformer Discovery Service", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    Else
                        If mcLibEntry.LocalPath.Contains("optifine\OptiFine") Then
                            Dim text2 As String = mcLibEntry.LocalPath.Replace(CustomMcFolder + "libraries\optifine\OptiFine\", "").Split("_", False)(0) + "/" + PathUtils.GetLastPart(mcLibEntry.LocalPath).Replace("-", "_")
                            text2 = "/maven/com/optifine/" + text2
                            If text2.Contains("_pre") Then
                                text2 = text2.Replace("com/optifine/", "com/optifine/preview_")
                            End If
                            list2.Add("https://bmclapi2.bangbang93.com" + text2)
                        ElseIf list2.Count <= 2 Then
                            list2.AddRange(ModDownload.DlSourceLibraryGet("https://libraries.minecraft.net" + mcLibEntry.LocalPath.Replace(CustomMcFolder + "libraries", "").Replace("\", "/")))
                        End If
                        list.Add(New ModNet.NetFile(list2.Distinct(), mcLibEntry.LocalPath, fileChecker, False))
                    End If
                Next
            Finally
                Dim enumerator As List(Of ModMinecraft.McLibEntry).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Dim enumerable As IEnumerable(Of ModNet.NetFile) = list
            Dim func As Func(Of ModNet.NetFile, String)
            If ModMinecraft._Closure$__.$I40-0 IsNot Nothing Then
                func = ModMinecraft._Closure$__.$I40-0
            Else
                Dim func2 As Func(Of ModNet.NetFile, String) = Function(a As ModNet.NetFile) a.LocalPath
                func = func2
                ModMinecraft._Closure$__.$I40-0 = func2
            End If
            Return enumerable.DistinctBy(func).ToList()
        End Function

        Public Function McLibGet(Original As String, Optional WithHead As Boolean = True, Optional IgnoreLiteLoader As Boolean = False, Optional CustomMcFolder As String = Nothing) As String
            CustomMcFolder = If(CustomMcFolder, ModMinecraft.McFolderSelected)
            Dim array As String() = Original.Split(":", False)
            Return If(WithHead, (CustomMcFolder + "libraries\"), "") + String.Format("{0}\{1}\{2}\{3}.jar", New Object() { array(0).Replace(".", "\"), array(1), array(2), array.Skip(1).Join("-") })
        End Function

        Public Function ShouldIgnoreFileCheck(Instance As ModMinecraft.McInstance) As Object
            Return Settings.[Get](Of Boolean)("VersionAdvanceAssetsV2", Instance) OrElse Settings.[Get](Of Integer)("VersionAdvanceAssets", Instance) = 2
        End Function

        Public Function McAssetsGetIndex(Instance As ModMinecraft.McInstance, Optional ReturnLegacyOnError As Boolean = False, Optional CheckURLEmpty As Boolean = False) As JToken
            Try
                Dim jtoken As JToken
                While True
                    jtoken = Instance.JsonObject("assetIndex")
                    If jtoken IsNot Nothing AndAlso jtoken("id") IsNot Nothing Then
                        Exit For
                    End If
                    If Instance.JsonObject("assets") IsNot Nothing Then
                        Instance.JsonObject("assets").ToString()
                    End If
                    If CheckURLEmpty AndAlso jtoken("url") IsNot Nothing Then
                        GoTo Block_6
                    End If
                    If Operators.CompareString(Instance.InheritName, "", False) = 0 Then
                        GoTo IL_0098
                    End If
                    Instance = New ModMinecraft.McInstance(ModMinecraft.McFolderSelected + "versions\" + Instance.InheritName)
                End While
                Return jtoken
                Block_6:
                Return jtoken
                IL_0098:
            Catch ex As Exception
            End Try
            If Not ReturnLegacyOnError Then
                Throw New Exception("该版本不存在资源文件索引信息")
            End If
            Logger.Info("无法获取资源文件索引下载地址，使用默认的 legacy 下载地址", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            Return "{" & vbCrLf & "                ""id"": ""legacy""," & vbCrLf & "                ""sha1"": ""c0fd82e8ce9fbc93119e40d96d5a4e62cfa3f729""," & vbCrLf & "                ""size"": 134284," & vbCrLf & "                ""url"": ""https://launchermeta.mojang.com/mc-staging/assets/legacy/c0fd82e8ce9fbc93119e40d96d5a4e62cfa3f729/legacy.json""," & vbCrLf & "                ""totalSize"": 111220701" & vbCrLf & "            }".DeserializeJson()
        End Function

        Public Function McAssetsGetIndexName(Instance As ModMinecraft.McInstance) As String
            Try
                While Instance.JsonObject("assetIndex") Is Nothing OrElse Instance.JsonObject("assetIndex")("id") Is Nothing
                    If Instance.JsonObject("assets") IsNot Nothing Then
                        Return Instance.JsonObject("assets").ToString()
                    End If
                    If Operators.CompareString(Instance.InheritName, "", False) = 0 Then
                        GoTo IL_00D2
                    End If
                    Instance = New ModMinecraft.McInstance(ModMinecraft.McFolderSelected + "versions\" + Instance.InheritName)
                End While
                Return Instance.JsonObject("assetIndex")("id").ToString()
            Catch ex As Exception
                Logger.Warn(ex, "获取资源文件索引名失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            End Try
            IL_00D2:
            Return "legacy"
        End Function

        Private Function McAssetsListGet(Instance As ModMinecraft.McInstance) As List(Of ModMinecraft.McAssetsToken)
            Dim text As String = ModMinecraft.McAssetsGetIndexName(Instance)
            Dim list2 As List(Of ModMinecraft.McAssetsToken)
            Try
                If Not FileUtils.Exists(String.Format("{0}assets\indexes\{1}.json", ModMinecraft.McFolderSelected, text)) Then
                    Throw New FileNotFoundException("未找到 Asset Index", ModMinecraft.McFolderSelected + "assets\indexes\" + text + ".json")
                End If
                Dim list As List(Of ModMinecraft.McAssetsToken) = New List(Of ModMinecraft.McAssetsToken)()
                Dim jobject As JObject = CType(FileUtils.ReadAsJson(String.Format("{0}assets\indexes\{1}.json", ModMinecraft.McFolderSelected, text), Nothing, Nothing), JObject)
                Try
                    For Each jtoken As JToken In jobject("objects").Children()
                        Dim jproperty As JProperty = CType(jtoken, JProperty)
                        Dim text2 As String
                        If jobject("map_to_resources") IsNot Nothing AndAlso jobject("map_to_resources").ToObject(Of Boolean)() Then
                            text2 = String.Format("{0}resources\{1}", Instance.PathIndie, jproperty.Name)
                        ElseIf jobject("virtual") IsNot Nothing AndAlso jobject("virtual").ToObject(Of Boolean)() Then
                            text2 = String.Format("{0}assets\virtual\legacy\{1}", ModMinecraft.McFolderSelected, jproperty.Name)
                        Else
                            text2 = String.Format("{0}assets\objects\{1}\{2}", ModMinecraft.McFolderSelected, Strings.Left(jproperty.Value("hash").ToString(), 2), jproperty.Value("hash"))
                        End If
                        list.Add(New ModMinecraft.McAssetsToken() With { .LocalPath = text2.Replace("/", "\"), .SourcePath = jproperty.Name, .Hash = jproperty.Value("hash").ToString(), .Size = Conversions.ToLong(jproperty.Value("size").ToString()) })
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                list2 = list
            Catch ex As Exception
                Logger.Warn(ex, String.Format("获取资源文件列表失败：{0}", text), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                Throw
            End Try
            Return list2
        End Function

        Public Function McAssetsFixList(Instance As ModMinecraft.McInstance, CheckHash As Boolean, Optional ByRef ProgressFeed As ModLoader.LoaderBase = Nothing) As List(Of ModNet.NetFile)
            ' The following expression was wrapped in a checked-statement
            Dim list As List(Of ModNet.NetFile)
            If CheckHash Then
                Dim enumerable As IEnumerable(Of ModMinecraft.McAssetsToken) = ModMinecraft.McAssetsListGet(Instance)
                Dim func As Func(Of ModMinecraft.McAssetsToken, ModNet.NetFile)
                If ModMinecraft._Closure$__.$I47-0 IsNot Nothing Then
                    func = ModMinecraft._Closure$__.$I47-0
                Else
                    Dim func2 As Func(Of ModMinecraft.McAssetsToken, ModNet.NetFile) = Function(Token As ModMinecraft.McAssetsToken) New ModNet.NetFile(ModDownload.DlSourceAssetsGet(String.Format("https://resources.download.minecraft.net/{0}/{1}", Strings.Left(Token.Hash, 2), Token.Hash)), Token.LocalPath, New FileChecker() With { .ActualSize = If((Token.Size = 0L), (-1L), Token.Size), .Hash = Token.Hash }, False)
                    func = func2
                    ModMinecraft._Closure$__.$I47-0 = func2
                End If
                list = enumerable.[Select](func).ToList()
            Else
                Dim list2 As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                Try
                    Dim list3 As List(Of ModMinecraft.McAssetsToken) = ModMinecraft.McAssetsListGet(Instance)
                    If ProgressFeed IsNot Nothing Then
                        ProgressFeed.Progress = 0.04
                    End If
                    Dim num As Integer = list3.Count - 1
                    For i As Integer = 0 To num
                        Dim mcAssetsToken As ModMinecraft.McAssetsToken = list3(i)
                        If ProgressFeed IsNot Nothing Then
                            ' The following expression was wrapped in a unchecked-expression
                            ProgressFeed.Progress = 0.05 + 0.94 * CDbl(i) / CDbl(list3.Count)
                        End If
                        Dim info As FileInfo = FileUtils.GetInfo(mcAssetsToken.LocalPath)
                        If Not info.Exists OrElse (mcAssetsToken.Size <> 0L AndAlso mcAssetsToken.Size <> info.Length) Then
                            list2.Add(New ModNet.NetFile(ModDownload.DlSourceAssetsGet(String.Format("https://resources.download.minecraft.net/{0}/{1}", Strings.Left(mcAssetsToken.Hash, 2), mcAssetsToken.Hash)), mcAssetsToken.LocalPath, New FileChecker() With { .ActualSize = If((mcAssetsToken.Size = 0L), (-1L), mcAssetsToken.Size), .Hash = mcAssetsToken.Hash }, False))
                        End If
                    Next
                Catch ex As Exception
                    Logger.Warn(ex, "获取版本缺失的资源文件下载列表失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End Try
                If ProgressFeed IsNot Nothing Then
                    ProgressFeed.Progress = 0.99
                End If
                list = list2
            End If
            Return list
        End Function

        Public Sub McDownloadClientUpdateHint(VersionName As String, Json As JObject)
            Dim CS$<>8__locals1 As ModMinecraft._Closure$__48-1 = New ModMinecraft._Closure$__48-1(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_VersionName = VersionName
            Try
                Dim CS$<>8__locals2 As ModMinecraft._Closure$__48-0 = New ModMinecraft._Closure$__48-0(CS$<>8__locals2)
                CS$<>8__locals2.$VB$Local_Version = Nothing
                Try
                    For Each jtoken As JToken In Json("versions")
                        If jtoken("id") IsNot Nothing AndAlso Operators.CompareString(jtoken("id").ToString(), CS$<>8__locals1.$VB$Local_VersionName, False) = 0 Then
                            CS$<>8__locals2.$VB$Local_Version = jtoken
                            Exit For
                        End If
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of JToken)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
                If CS$<>8__locals2.$VB$Local_Version IsNot Nothing Then
                    Dim dateTime As DateTime = CType(CS$<>8__locals2.$VB$Local_Version("releaseTime"), DateTime)
                    If ModMain.OurMsgBox(String.Format("新版本：{0}{1}", CS$<>8__locals1.$VB$Local_VersionName, vbCrLf) + If(((DateTime.Now - dateTime).TotalDays > 1.0), ("更新时间：" + dateTime.ToString()), ("更新于：" + StringUtils.FormatTimeSpan(dateTime - DateTime.Now, False))), "Minecraft 更新提示", "确定", "下载", If(((DateTime.Now - dateTime).TotalHours > 3.0), "更新日志", ""), False, True, False, Nothing, Nothing, Sub()
                        ModDownloadLib.McUpdateLogShow(CS$<>8__locals2.$VB$Local_Version)
                    End Sub) = 2 Then
                        ModBase.RunInUi(Sub()
                            PageDownloadInstall.VersionWaitingSelect = CS$<>8__locals1.$VB$Local_VersionName
                            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.[Default])
                        End Sub, False)
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("Minecraft 更新提示发送失败（{0}）", If(CS$<>8__locals1.$VB$Local_VersionName, "Nothing")), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
            End Try
        End Sub

        Public Function FilterAccessToken(Raw As String, FilterChar As Char) As String
            If Raw.Contains("accessToken ") Then
                Try
                    For Each text As String In Raw.RegexSearch("(?<=accessToken ([^ ]{5}))[^ ]+(?=[^ ]{5})", RegexOptions.None)
                        Raw = Raw.Replace(text, New String(FilterChar, text.Count()))
                    Next
                Finally
                    Dim enumerator As IEnumerator(Of String)
                    If enumerator IsNot Nothing Then
                        enumerator.Dispose()
                    End If
                End Try
            End If
            Dim accessToken As String = ModLaunch.McLoginLoader.Output.AccessToken
            If accessToken IsNot Nothing AndAlso accessToken.Length >= 10 AndAlso Raw.ContainsIgnoreCase(accessToken) AndAlso Operators.CompareString(ModLaunch.McLoginLoader.Output.Uuid, ModLaunch.McLoginLoader.Output.AccessToken, False) <> 0 Then
                ' The following expression was wrapped in a checked-expression
                Raw = Raw.Replace(accessToken, Strings.Left(accessToken, 5) + New String(FilterChar, accessToken.Length - 10) + Strings.Right(accessToken, 5))
            End If
            Return Raw
        End Function

        Public Function FilterUserName(Raw As String, FilterChar As Char) As String
            If Raw.Contains(":\Users\") Then
                Raw = Regex.Replace(Raw, "(?<=:\\Users\\)[^\\]+", Function(Token As Match) New String(FilterChar, Token.Length))
            End If
            If Raw.Contains(":/Users/") Then
                Raw = Regex.Replace(Raw, "(?<=:/Users/)[^/]+", Function(Token As Match) New String(FilterChar, Token.Length))
            End If
            Return Raw
        End Function

        Private _McFolderSelected As String

        Public McFolderList As List(Of ModMinecraft.McFolder) = New List(Of ModMinecraft.McFolder)()

        Public McFolderListLoader As ModLoader.LoaderTask(Of Integer, Integer) = New ModLoader.LoaderTask(Of Integer, Integer)("Minecraft Folder List", Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
            ModMinecraft.McFolderListLoadSub()
        End Sub, Nothing, ThreadPriority.AboveNormal)

        Private _McInstanceSelected As ModMinecraft.McInstance

        Public McInstanceList As Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance)) = New Dictionary(Of ModMinecraft.McInstanceCardType, List(Of ModMinecraft.McInstance))()

        Public McInstanceListForceRefresh As Boolean = False

        Private IsFirstMcInstanceListLoad As Boolean = True

        Public McInstanceListLoader As ModLoader.LoaderTask(Of String, Integer) = New ModLoader.LoaderTask(Of String, Integer)("Minecraft Instance List", AddressOf ModMinecraft.InitMcInstanceList, Nothing, ThreadPriority.Normal) With { .ReloadTimeout = 1 }

        Private McSkinDownloadLock As Object = RuntimeHelpers.GetObjectValue(New Object())

        Private OSVersion As String = MyWpfExtension.Computer.Info.OSVersion

        Private $STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast As Object

        Private $STATIC$set_McInstanceSelected$0111287E8$_McInstanceSelectedLast$Init As StaticLocalInitFlag

        Public Class McFolder
            Public Overrides Function Equals(obj As Object) As Boolean
                Dim flag As Boolean
                If Not(TypeOf obj Is ModMinecraft.McFolder) Then
                    flag = False
                Else
                    Dim mcFolder As ModMinecraft.McFolder = CType(obj, ModMinecraft.McFolder)
                    flag = Operators.CompareString(Me.Name, mcFolder.Name, False) = 0 AndAlso PathUtils.IsSame(Me.Location, mcFolder.Location) AndAlso Me.Type = mcFolder.Type
                End If
                Return flag
            End Function

            Public Overrides Function ToString() As String
                Return Me.Location
            End Function

            Public Name As String

            Public Location As String

            Public Type As ModMinecraft.McFolder.Types

            Public Enum Types
                Vanilla
                RenamedVanilla
                Custom
            End Enum
        End Class

        Public Class McInstance
            Public ReadOnly Property PathVersion As String

            Public ReadOnly Property PathIndie As String
                Get
                    If Not Settings.HasSaved("VersionArgumentIndieV2", Me) Then
                        If Not Me.IsLoaded Then
                            Me.Load()
                        End If
                        Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_0(Of Boolean) = Function()
                            Dim flag As Boolean
                            If Settings.HasSaved("VersionArgumentIndie", Me) AndAlso Settings.[Get](Of Integer)("VersionArgumentIndie", Me) > 0 Then
                                Logger.Info(String.Format("版本隔离初始化（{0}）：从老的版本独立设置中迁移", Me.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                flag = Settings.[Get](Of Integer)("VersionArgumentIndie", Me) = 1
                            Else
                                Dim info As DirectoryInfo = DirectoryUtils.GetInfo(Me.PathVersion + "mods\")
                                Dim info2 As DirectoryInfo = DirectoryUtils.GetInfo(Me.PathVersion + "saves\")
                                If(info.Exists AndAlso info.EnumerateFiles().Any()) OrElse (info2.Exists AndAlso info2.EnumerateDirectories().Any()) Then
                                    Logger.Info(String.Format("版本隔离初始化（{0}）：版本文件夹下存在 mods 或 saves 文件夹，自动开启", Me.Name), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                    flag = True
                                Else
                                    Dim flag2 As Boolean = Me.State <> ModMinecraft.McInstance.McInstanceState.Fool AndAlso Me.State <> ModMinecraft.McInstance.McInstanceState.Old AndAlso Me.State <> ModMinecraft.McInstance.McInstanceState.Snapshot
                                    Logger.Info(String.Format("版本隔离初始化（{0}）：从全局默认设置中（{1}）判断，State {2}，IsRelease {3}，Modable {4}", New Object() { Me.Name, Settings.[Get](Of Integer)("LaunchArgumentIndieV2", Nothing), Me.State, flag2, Me.Modable }), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                    Select Case Settings.[Get](Of Integer)("LaunchArgumentIndieV2", Nothing)
                                        Case 0
                                            flag = False
                                        Case 1
                                            flag = Me.Modable
                                        Case 2
                                            flag = Not flag2
                                        Case 3
                                            flag = Not flag2 OrElse Me.Modable
                                        Case Else
                                            flag = True
                                    End Select
                                End If
                            End If
                            Return flag
                        End Function
                        Settings.[Set]("VersionArgumentIndieV2", vb$AnonymousDelegate_(), Me)
                    End If
                    If Not Settings.[Get](Of Boolean)("VersionArgumentIndieV2", Me) Then
                        Return ModMinecraft.McFolderSelected
                    End If
                    Return Me.PathVersion
                End Get
            End Property

            Public ReadOnly Property Name As String
                Get
                    If Me._Name Is Nothing AndAlso Operators.CompareString(Me.PathVersion, "", False) <> 0 Then
                        Me._Name = PathUtils.GetLastPart(Me.PathVersion)
                    End If
                    Return Me._Name
                End Get
            End Property

            Public ReadOnly Property Modable As Boolean
                Get
                    If Not Me.IsLoaded Then
                        Me.Load()
                    End If
                    Return Me.Version.ModLoaders.Flags().Any() OrElse Me.DisplayType = ModMinecraft.McInstanceCardType.API
                End Get
            End Property

            Public Property Version As McVersion
                Get
                    Dim mcVersion As McVersion
                    If Me._Version IsNot Nothing Then
                        mcVersion = Me._Version
                    Else
                        Try
                            Dim mcVersion2 As McVersion = New McVersion(Me.Name, Me.JsonObject, Me.JsonVersion)
                            Me._Version = mcVersion2
                            If Operators.CompareString(mcVersion2.VanillaName, "Unknown", False) = 0 Then
                                Me.Info = "PCL 无法识别该版本的 MC 版本号"
                            End If
                        Catch ex As Exception
                            Logger.Warn(ex, "识别 Minecraft 版本时出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                            Me._Version = New McVersion("Unknown", True)
                            Me.Info = "无法识别：" + ex.Message
                        End Try
                        mcVersion = Me._Version
                    End If
                    Return mcVersion
                End Get
                Set(value As McVersion)
                    Me._Version = value
                End Set
            End Property

            Public ReadOnly Property ReleaseTime As DateTime
                Get
                    Return Me.Version.ReleaseTime
                End Get
            End Property

            Public Function GetJsonPath() As String
                Dim text As String = Me.PathVersion + Me.Name + ".json"
                If Not FileUtils.Exists(text) Then
                    text = Nothing
                    Try
                        For Each text2 As String In DirectoryUtils.EnumerateFiles(Me.PathVersion, False, "*.json")
                            Try
                                Dim jobject As JObject = CType(FileUtils.ReadAsJson(text2, Nothing, Nothing), JObject)
                                If Not jobject.ContainsKey("mainClass") Then
                                    Continue For
                                End If
                                If Not jobject.ContainsKey("type") Then
                                    Continue For
                                End If
                                If Not jobject.ContainsKey("id") Then
                                    Continue For
                                End If
                            Catch ex As Exception
                                Continue For
                            End Try
                            text = text2
                            Exit For
                        Next
                    Finally
                        Dim enumerator As IEnumerator(Of String)
                        If enumerator IsNot Nothing Then
                            enumerator.Dispose()
                        End If
                    End Try
                    If text Is Nothing Then
                        Throw New Exception(String.Format("未找到版本 JSON 文件：{0}{1}.json", Me.PathVersion, Me.Name))
                    End If
                    Logger.Warn(String.Format("未找到同名版本 JSON，自动换用 {0}", text), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End If
                Return text
            End Function

            Public Property JsonText As String
                Get
                    Dim vb$AnonymousDelegate_ As VB$AnonymousDelegate_19(Of String, Boolean)
                    If ModMinecraft.McInstance._Closure$__.$I24-0 IsNot Nothing Then
                        vb$AnonymousDelegate_ = ModMinecraft.McInstance._Closure$__.$I24-0
                    Else
                        Dim vb$AnonymousDelegate_2 As VB$AnonymousDelegate_19(Of String, Boolean) = Function(Json As String)
                            Dim text As String = Json.Trim()
                            Return text.StartsWithF("{", False) AndAlso text.EndsWithF("}", False)
                        End Function
                        vb$AnonymousDelegate_ = vb$AnonymousDelegate_2
                        ModMinecraft.McInstance._Closure$__.$I24-0 = vb$AnonymousDelegate_2
                    End If
                    Dim vb$AnonymousDelegate_3 As VB$AnonymousDelegate_19(Of String, Boolean) = vb$AnonymousDelegate_
                    If Me._JsonText Is Nothing Then
                        Dim jsonPath As String = Me.GetJsonPath()
                        Me._JsonText = If(FileUtils.TryReadAsString(jsonPath, Nothing, Nothing), "")
                        If Not vb$AnonymousDelegate_3(Me._JsonText) Then
                            If ModBase.RunInUi() Then
                                Logger.Warn("版本 JSON 文件为空或有误，由于代码在主线程运行，将不再进行重试", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                Me._JsonText.DeserializeJson()
                            Else
                                Logger.Warn(String.Format("版本 JSON 文件为空或有误，将在 2s 后重试读取（{0}）", jsonPath), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                Thread.Sleep(2000)
                                Me._JsonText = FileUtils.ReadAsString(jsonPath, Nothing, Nothing)
                                If Not vb$AnonymousDelegate_3(Me._JsonText) Then
                                    Me._JsonText.DeserializeJson()
                                End If
                            End If
                        End If
                    End If
                    Return Me._JsonText
                End Get
                Set(value As String)
                    Me._JsonText = value
                End Set
            End Property

            Public Property JsonObject As JObject
                Get
                    If Me._JsonObject Is Nothing Then
                        Dim jsonText As String = Me.JsonText
                        Try
                            Me._JsonObject = CType(jsonText.DeserializeJson(), JObject)
                            If Me._JsonObject.ContainsKey("patches") AndAlso Not Me._JsonObject.ContainsKey("time") Then
                                Me.IsHmclFormatJson = True
                                Dim jobject As JObject = Nothing
                                Dim list As List(Of JObject) = New List(Of JObject)()
                                Try
                                    For Each jtoken As JToken In Me._JsonObject("patches")
                                        Dim jobject2 As JObject = CType(jtoken, JObject)
                                        list.Add(jobject2)
                                    Next
                                Finally
                                    Dim enumerator As IEnumerator(Of JToken)
                                    If enumerator IsNot Nothing Then
                                        enumerator.Dispose()
                                    End If
                                End Try
                                Dim list2 As IList(Of JObject) = list
                                Dim func As Func(Of JObject, JObject, Boolean)
                                If ModMinecraft.McInstance._Closure$__.$I28-0 IsNot Nothing Then
                                    func = ModMinecraft.McInstance._Closure$__.$I28-0
                                Else
                                    Dim func2 As Func(Of JObject, JObject, Boolean) = Function(Left As JObject, Right As JObject) ModBase.Val((If(Left("priority"), "0")).ToString()) < ModBase.Val((If(Right("priority"), "0")).ToString())
                                    func = func2
                                    ModMinecraft.McInstance._Closure$__.$I28-0 = func2
                                End If
                                list = list2.SortByComparison(func)
                                Try
                                    For Each jobject3 As JObject In list
                                        Dim text As String = CStr(jobject3("id"))
                                        If text IsNot Nothing Then
                                            Logger.Info(String.Format("合并 HMCL 分支项：{0}", text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                            If jobject IsNot Nothing Then
                                                jobject.Merge(jobject3)
                                            Else
                                                jobject = jobject3
                                            End If
                                        Else
                                            Logger.Info("存在为空的 HMCL 分支项", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                        End If
                                    Next
                                Finally
                                    Dim enumerator2 As List(Of JObject).Enumerator
                                    CType(enumerator2, IDisposable).Dispose()
                                End Try
                                Me._JsonObject = jobject
                                Me._JsonObject("id") = Me.Name
                                If Me._JsonObject.ContainsKey("inheritsFrom") Then
                                    Me._JsonObject.Remove("inheritsFrom")
                                End If
                            End If
                            Dim obj As Object = Nothing
                            Try
                                obj = If((Me._JsonObject("inheritsFrom") Is Nothing), "", Me._JsonObject("inheritsFrom").ToString())
                                If Operators.ConditionalCompareObjectEqual(obj, Me.Name, False) Then
                                    Logger.Warn(String.Format("自引用的继承版本：{0}", Me.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                                    obj = ""
                                Else
                                    While Operators.ConditionalCompareObjectNotEqual(obj, "", False)
                                        Dim mcInstance As ModMinecraft.McInstance = New ModMinecraft.McInstance(Conversions.ToString(obj))
                                        If Operators.ConditionalCompareObjectEqual(mcInstance.InheritName, obj, False) Then
                                            Throw New Exception(Conversions.ToString(Operators.ConcatenateObject("版本依赖项出现嵌套：", obj)))
                                        End If
                                        obj = mcInstance.InheritName
                                        Dim jarray As JArray = CType(Me._JsonObject("libraries").DeepClone(), JArray)
                                        Try
                                            For Each jtoken2 As JToken In mcInstance.JsonObject("libraries")
                                                jarray.Add(jtoken2)
                                            Next
                                        Finally
                                            Dim enumerator3 As IEnumerator(Of JToken)
                                            If enumerator3 IsNot Nothing Then
                                                enumerator3.Dispose()
                                            End If
                                        End Try
                                        mcInstance.JsonObject.Merge(Me._JsonObject)
                                        Me._JsonObject = mcInstance.JsonObject
                                        Me._JsonObject("libraries") = jarray
                                    End While
                                End If
                            Catch ex As Exception
                                Logger.Warn(ex, String.Format("合并版本依赖项 JSON 失败（{0}）", RuntimeHelpers.GetObjectValue(If(obj, "null"))), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                            End Try
                        Catch ex2 As Exception
                            Throw New Exception("初始化版本 JSON 时失败（" + (If(Me.Name, "null")) + "）", ex2)
                        End Try
                    End If
                    Return Me._JsonObject
                End Get
                Set(value As JObject)
                    Me._JsonObject = value
                End Set
            End Property

            Public ReadOnly Property IsOldJson As Boolean
                Get
                    Return Me.JsonObject("minecraftArguments") IsNot Nothing AndAlso CStr(Me.JsonObject("minecraftArguments")) <> ""
                End Get
            End Property

            Public Property IsHmclFormatJson As Boolean

            Public ReadOnly Property JsonVersion As JObject
                Get
                    If Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init Is Nothing Then
                        Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init, New StaticLocalInitFlag(), Nothing)
                    End If
                    Dim flag As Boolean = False
                    Try
                        Monitor.Enter(Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init, flag)
                        If Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State = 0S Then
                            Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State = 2S
                            Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited = False
                        ElseIf Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State = 2S Then
                            Throw New IncompleteInitialization()
                        End If
                    Finally
                        Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init.State = 1S
                        If flag Then
                            Monitor.[Exit](Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init)
                        End If
                    End Try
                    If Not Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited Then
                        Me.$STATIC$get_JsonVersion$2001281F1$JsonVersionInited = True
                        Try
                            If FileUtils.Exists(String.Format("{0}{1}.jar", Me.PathVersion, Me.Name)) Then
                                Using zipArchive As ZipArchive = FileUtils.OpenZip(Me.PathVersion + Me.Name + ".jar", Nothing)
                                    Dim entry As ZipArchiveEntry = zipArchive.GetEntry("version.json")
                                    If entry IsNot Nothing Then
                                        Using streamReader As StreamReader = New StreamReader(entry.Open())
                                            Me._JsonVersion = CType(streamReader.ReadToEnd().DeserializeJson(), JObject)
                                        End Using
                                    End If
                                End Using
                            End If
                        Catch ex As Exception
                            Logger.Warn(ex, String.Format("从版本 jar 中读取 version.json 失败（{0}{1}.jar）", Me.PathVersion, Me.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        End Try
                    End If
                    Return Me._JsonVersion
                End Get
            End Property

            Public ReadOnly Property InheritName As String
                Get
                    If Me._InheritName Is Nothing Then
                        Me._InheritName = (If(Me.JsonObject("inheritsFrom"), "")).ToString()
                        If Me.JsonText.Contains("liteloader") AndAlso Operators.CompareString(Me.Version.VanillaName, Me.Name, False) <> 0 AndAlso Not Me.JsonText.Contains("logging") AndAlso Operators.CompareString((If(Me.JsonObject("jar"), Me.Version.VanillaName)).ToString(), Me.Version.VanillaName, False) = 0 Then
                            Me._InheritName = Me.Version.VanillaName
                        End If
                        If Me.IsHmclFormatJson Then
                            Me._InheritName = ""
                        End If
                    End If
                    Return Me._InheritName
                End Get
            End Property

            Public Sub ResetSettingsCache()
                ModBase.IniClearCache(Me.PathIndie + "options.txt")
                ModBase.IniClearCache(Me.PathVersion + "PCL\Setup.ini")
                Me.Config.DiscardCache()
                Try
                    For Each setting As Settings.Setting In Settings.Entries.Values
                        If setting.Source = Settings.Sources.Instance Then
                            setting.ValueCache.Remove(Me.PathVersion)
                        End If
                    Next
                Finally
                    Dim enumerator As Dictionary(Of String, Settings.Setting).ValueCollection.Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
            End Sub

            Public Sub New(NameOrFullPath As String)
                Me._Name = Nothing
                Me.Info = "该版本未被加载，请向作者反馈此问题"
                Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                Me.IsStar = False
                Me.DisplayType = ModMinecraft.McInstanceCardType.Auto
                Me._Version = Nothing
                Me._JsonText = Nothing
                Me._JsonObject = Nothing
                Me.IsHmclFormatJson = False
                Me._JsonVersion = Nothing
                Me._InheritName = Nothing
                Me.IsLoaded = False
                NameOrFullPath = PathUtils.RemoveExtendedPrefix(NameOrFullPath)
                Me.PathVersion = PathUtils.AddSlashSuffix(If(NameOrFullPath.Contains(":"), "", (ModMinecraft.McFolderSelected + "versions\")) + NameOrFullPath)
                Me.Config = New JsonConfigProvider(Path.Combine(Me.PathVersion, "PCL\config.json"))
            End Sub

            Public Function Check() As Boolean
                Dim flag As Boolean
                If Not DirectoryUtils.Exists(Me.PathVersion) Then
                    Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                    Me.Info = "未找到版本 " + Me.Name
                    flag = False
                Else
                    Try
                        If Me.JsonObject("mainClass") Is Nothing Then
                            Throw New Exception("版本 JSON 中没有 mainClass 项！")
                        End If
                    Catch ex As Exception
                        Logger.Warn(ex, String.Format("版本 JSON 可用性检查失败（{0}）", Me.PathVersion), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        Me.JsonText = ""
                        Me.JsonObject = Nothing
                        Me.Info = ex.Message
                        Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                        Return False
                    End Try
                    Try
                        If String.IsNullOrEmpty(Me.Version.VanillaName) Then
                            Throw New Exception("无法获取版本号，结果为空")
                        End If
                    Catch ex2 As Exception
                        Logger.Warn(ex2, String.Format("版本号获取失败（{0}）", Me.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                        Me.Info = "版本号获取失败：" + ex2.GetDisplay(False)
                        Return False
                    End Try
                    Try
                        If Operators.CompareString(Me.InheritName, "", False) <> 0 AndAlso Not FileUtils.Exists(String.Format("{0}\{1}\{2}.json", PathUtils.RemoveLastPart(Me.PathVersion), Me.InheritName, Me.InheritName)) Then
                            Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                            Me.Info = "需要安装 " + Me.InheritName + " 作为前置版本"
                            Return False
                        End If
                    Catch ex3 As Exception
                        Logger.Warn(ex3, String.Format("依赖版本检查出错（{0}）", Me.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                        Me.Info = "未知错误：" + ex3.GetDisplay(False)
                        Return False
                    End Try
                    Me.State = ModMinecraft.McInstance.McInstanceState.Original
                    flag = True
                End If
                Return flag
            End Function

            Public Function Load() As ModMinecraft.McInstance
                Try
                    If Me.Check() Then
                        Dim vanillaName As String = Me.Version.VanillaName
                        If Operators.CompareString(vanillaName, "Unknown", False) <> 0 Then
                            If Operators.CompareString(vanillaName, "Old", False) <> 0 Then
                                If Me.Version.IsFool Then
                                    Me.State = ModMinecraft.McInstance.McInstanceState.Fool
                                ElseIf Me.Version.IsSnapshot Then
                                    Me.State = ModMinecraft.McInstance.McInstanceState.Snapshot
                                End If
                                If Me.Version.HasOptiFine Then
                                    Me.State = ModMinecraft.McInstance.McInstanceState.OptiFine
                                End If
                                If Me.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) Then
                                    Me.State = ModMinecraft.McInstance.McInstanceState.LiteLoader
                                End If
                                If Me.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) Then
                                    Me.State = ModMinecraft.McInstance.McInstanceState.Fabric
                                ElseIf Me.Version.ModLoaders.HasFlagF(ModLoaders.Forge) Then
                                    Me.State = ModMinecraft.McInstance.McInstanceState.Forge
                                ElseIf Me.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) Then
                                    Me.State = ModMinecraft.McInstance.McInstanceState.NeoForge
                                End If
                            Else
                                Me.State = ModMinecraft.McInstance.McInstanceState.Old
                            End If
                        Else
                            Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                        End If
                    End If
                    Me.Logo = ModBase.ReadIni(Me.PathVersion + "PCL\Setup.ini", "Logo", "")
                    If Operators.CompareString(Me.Logo, "", False) = 0 OrElse Not Conversions.ToBoolean(ModBase.ReadIni(Me.PathVersion + "PCL\Setup.ini", "LogoCustom", Conversions.ToString(False))) Then
                        Select Case Me.State
                            Case ModMinecraft.McInstance.McInstanceState.Original
                                Me.Logo = ModBase.PathImage + "Blocks/Grass.png"
                            Case ModMinecraft.McInstance.McInstanceState.Snapshot
                                Me.Logo = ModBase.PathImage + "Blocks/CommandBlock.png"
                            Case ModMinecraft.McInstance.McInstanceState.Fool
                                Me.Logo = ModBase.PathImage + "Blocks/GoldBlock.png"
                            Case ModMinecraft.McInstance.McInstanceState.OptiFine
                                Me.Logo = ModBase.PathImage + "Blocks/GrassPath.png"
                            Case ModMinecraft.McInstance.McInstanceState.Old
                                Me.Logo = ModBase.PathImage + "Blocks/CobbleStone.png"
                            Case ModMinecraft.McInstance.McInstanceState.Forge
                                Me.Logo = ModBase.PathImage + "Blocks/Anvil.png"
                            Case ModMinecraft.McInstance.McInstanceState.NeoForge
                                Me.Logo = ModBase.PathImage + "Blocks/NeoForge.png"
                            Case ModMinecraft.McInstance.McInstanceState.LiteLoader
                                Me.Logo = ModBase.PathImage + "Blocks/Egg.png"
                            Case ModMinecraft.McInstance.McInstanceState.Fabric
                                Me.Logo = ModBase.PathImage + "Blocks/Fabric.png"
                            Case Else
                                Me.Logo = ModBase.PathImage + "Blocks/RedstoneBlock.png"
                        End Select
                    End If
                    Me.RefreshInfo()
                    Me.IsStar = Conversions.ToBoolean(ModBase.ReadIni(Me.PathVersion + "PCL\Setup.ini", "IsStar", Conversions.ToString(False)))
                    Me.DisplayType = CType(Conversions.ToInteger(ModBase.ReadIni(Me.PathVersion + "PCL\Setup.ini", "DisplayType", Conversions.ToString(0))), ModMinecraft.McInstanceCardType)
                    If DirectoryUtils.Exists(Me.PathVersion) Then
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "State", Conversions.ToString(CInt(Me.State)))
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "Info", Me.Info)
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "Logo", Me.Logo)
                    End If
                    If Me.State <> ModMinecraft.McInstance.McInstanceState.[Error] AndAlso Me.Version.Reliable Then
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "ReleaseTime", Me.ReleaseTime.ToString("yyyy'-'MM'-'dd HH':'mm"))
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "VersionFabric", If(Me.Version.ModLoaders.HasFlagF(ModLoaders.Fabric), Me.Version.ModLoaderVersions(ModLoaders.Fabric), Nothing))
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "VersionOptiFine", If(Me.Version.OptiFineAsMod, Nothing, Me.Version.OptiFine))
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "VersionLiteLoader", Conversions.ToString(Me.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader)))
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "VersionForge", If(Me.Version.ModLoaders.HasFlagF(ModLoaders.Forge), Me.Version.ModLoaderVersions(ModLoaders.Forge), Nothing))
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "VersionNeoForge", If(Me.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge), Me.Version.ModLoaderVersions(ModLoaders.NeoForge), Nothing))
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "VersionVanillaName", Me.Version.VanillaName)
                        ModBase.WriteIni(Me.PathVersion + "PCL\Setup.ini", "VersionVanilla", Me.Version.Vanilla.ToString())
                    End If
                Catch ex As Exception
                    Me.Info = ex.GetDisplay(False)
                    Me.Logo = ModBase.PathImage + "Blocks/RedstoneBlock.png"
                    Me.State = ModMinecraft.McInstance.McInstanceState.[Error]
                    Logger.Warn(ex, String.Format("加载版本失败（{0}）", Me.Name), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                Finally
                    Me.IsLoaded = True
                End Try
                Return Me
            End Function

            Public Sub RefreshInfo()
                Dim text As String = ModBase.ReadIni(Me.PathVersion + "PCL\Setup.ini", "CustomInfo", "")
                If Operators.CompareString(text, "", False) <> 0 Then
                    Me.Info = text
                    Return
                End If
                Me.Info = Me.VersionDisplayName()
                If Me.State = ModMinecraft.McInstance.McInstanceState.Fool Then
                    Me.Info = ModMinecraft.GetMcFoolName(Me.Version.VanillaName)
                    Return
                End If
                If Me.State <> ModMinecraft.McInstance.McInstanceState.[Error] Then
                    If Settings.[Get](Of Integer)("VersionServerLogin", Me) = 3 Then
                        Dim ptr As __ByRef(Of String) = Me.Info
                        Me.Info = ptr + ", 统一通行证验证"
                    End If
                    If Settings.[Get](Of Integer)("VersionServerLogin", Me) = 4 Then
                        Dim ptr As __ByRef(Of String) = Me.Info
                        Me.Info = ptr + ", Authlib 验证"
                    End If
                End If
            End Sub

            Public Function VersionDisplayName() As String
                Dim text As String = ""
                If Me.Version.ModLoaders.HasFlagF(ModLoaders.Forge) Then
                    text = text + ", Forge" + If((Operators.CompareString(Me.Version.ModLoaderVersions(ModLoaders.Forge), "未知版本", False) = 0), "", (" " + Me.Version.ModLoaderVersions(ModLoaders.Forge)))
                End If
                If Me.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge) Then
                    text = text + ", NeoForge" + If((Operators.CompareString(Me.Version.ModLoaderVersions(ModLoaders.NeoForge), "未知版本", False) = 0), "", (" " + Me.Version.ModLoaderVersions(ModLoaders.NeoForge)))
                End If
                If Me.Version.ModLoaders.HasFlagF(ModLoaders.Fabric) Then
                    text = text + ", Fabric" + If((Operators.CompareString(Me.Version.ModLoaderVersions(ModLoaders.Fabric), "未知版本", False) = 0), "", (" " + Me.Version.ModLoaderVersions(ModLoaders.Fabric)))
                End If
                If Me.Version.HasOptiFine Then
                    text = text + ", OptiFine" + If((Operators.CompareString(Me.Version.OptiFine, "未知版本", False) = 0), "", (" " + Me.Version.OptiFine.Replace("-", " ").Replace("_", " ")))
                End If
                If Me.Version.ModLoaders.HasFlagF(ModLoaders.LiteLoader) Then
                    text += ", LiteLoader"
                End If
                Dim text2 As String
                Select Case Me.State
                    Case ModMinecraft.McInstance.McInstanceState.[Error]
                        Return Me.Info
                    Case ModMinecraft.McInstance.McInstanceState.Original, ModMinecraft.McInstance.McInstanceState.Snapshot, ModMinecraft.McInstance.McInstanceState.OptiFine, ModMinecraft.McInstance.McInstanceState.Forge, ModMinecraft.McInstance.McInstanceState.NeoForge, ModMinecraft.McInstance.McInstanceState.LiteLoader, ModMinecraft.McInstance.McInstanceState.Fabric
                        If Me.Version.VanillaName.ContainsIgnoreCase("pre") Then
                            text2 = "预发布版 " + Me.Version.VanillaName
                        ElseIf Me.Version.VanillaName.ContainsIgnoreCase("rc") Then
                            text2 = "发布候选 " + Me.Version.VanillaName
                        ElseIf Me.Version.VanillaName.ContainsIgnoreCase("experimental") Then
                            text2 = "实验性快照" + Me.Version.VanillaName
                        ElseIf Operators.CompareString(Me.Version.VanillaName, "pending", False) = 0 Then
                            text2 = "实验性快照"
                        ElseIf Me.Version.IsSnapshot Then
                            text2 = If(Me.Version.Reliable, ("快照版 " + Me.Version.VanillaName.Replace("-snapshot", "")), "快照版")
                        Else
                            text2 = If(Me.Version.Reliable, ("正式版 " + Me.Version.VanillaName), "正式版")
                        End If
                    Case ModMinecraft.McInstance.McInstanceState.Fool
                        text2 = "愚人节版本 " + Me.Version.VanillaName
                    Case ModMinecraft.McInstance.McInstanceState.Old
                        text2 = "远古版本"
                    Case Else
                        Return "发生了未知错误，请向作者反馈此问题"
                End Select
                Return(text2 + text).Replace("_", "-")
            End Function

            Public Function ToListItem(Optional ContentHandler As Action(Of OurListItem, EventArgs) = Nothing) As OurVirtualizingElement(Of OurListItem)
                Return New OurVirtualizingElement(Of OurListItem)(Function()
                    Dim myListItem As OurListItem = New OurListItem() With { .Info = Me.Info, .Height = 42.0, .Tag = Me, .SnapsToDevicePixels = True, .Type = OurListItem.CheckType.Clickable, .ContentHandler = ContentHandler }
                    myListItem.Inlines.Clear()
                    myListItem.Inlines.Add(New Run(Me.Name))
                    If Operators.CompareString(ModBase.ReadIni(Me.PathVersion + "PCL\Setup.ini", "CustomInfo", ""), "", False) <> 0 Then
                        myListItem.Inlines.Add(New Run("  |  " + Me.VersionDisplayName()) With { .Foreground = New ModBase.OurColor(215.0, 215.0, 215.0), .FontSize = 12.0 })
                    End If
                    Try
                        If Me.Logo.EndsWithF("PCL\Logo.png", False) Then
                            myListItem.Logo = Me.PathVersion + "PCL\Logo.png"
                        Else
                            myListItem.Logo = Me.Logo
                        End If
                    Catch ex As Exception
                        Logger.[Error](ex, "加载版本图标失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                        myListItem.Logo = "pack://application:,,,/images/Blocks/RedstoneBlock.png"
                    End Try
                    Return myListItem
                End Function) With { .Height = 42.0 }
            End Function

            Public Function Rename(NewName As String) As ModMinecraft.McInstance
                Dim name As String = Me.Name
                Dim pathVersion As String = Me.PathVersion
                Dim text As String = PathUtils.AddSlashSuffix(Path.Combine(PathUtils.RemoveLastPart(pathVersion), NewName))
                Dim jsonPath As String = Me.GetJsonPath()
                Dim jobject As JObject
                Try
                    jobject = CType(FileUtils.ReadAsJson(jsonPath, Nothing, Nothing), JObject)
                Catch ex As Exception
                    Logger.Warn(ex, "在重命名读取 JSON 时失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                    jobject = Me.JsonObject
                End Try
                Me.Config.Save()
                Me.ResetSettingsCache()
                DirectoryUtils.Move(pathVersion, text)
                If DirectoryUtils.Exists(String.Format("{0}{1}-natives", text, name)) Then
                    DirectoryUtils.Move(String.Format("{0}{1}-natives", text, name), String.Format("{0}{1}-natives", text, NewName))
                End If
                If FileUtils.Exists(String.Format("{0}{1}.jar", text, name)) Then
                    FileUtils.Move(String.Format("{0}{1}.jar", text, name), String.Format("{0}{1}.jar", text, NewName), False)
                End If
                If FileUtils.Exists(text + "PCL\Setup.ini") Then
                    FileUtils.Write(text + "PCL\Setup.ini", FileUtils.ReadAsString(text + "PCL\Setup.ini", Nothing, Nothing).Replace(pathVersion, text), Nothing)
                End If
                If Operators.CompareString(ModBase.ReadIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", ""), name, False) = 0 Then
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", NewName)
                End If
                Try
                    jobject("id") = NewName
                    FileUtils.Delete(text + PathUtils.GetLastPart(jsonPath), False)
                    FileUtils.Write(text + NewName + ".json", jobject.ToString(), Nothing)
                Catch ex2 As Exception
                    Logger.Warn(ex2, "重命名版本 json 失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Minecraft\ModMinecraft.vb")
                End Try
                Return New ModMinecraft.McInstance(text).Load()
            End Function

            Public Overrides Function Equals(obj As Object) As Boolean
                Dim mcInstance As ModMinecraft.McInstance = TryCast(obj, ModMinecraft.McInstance)
                Return mcInstance IsNot Nothing AndAlso PathUtils.IsSame(Me.PathVersion, mcInstance.PathVersion)
            End Function

            Public Shared Operator =(a As ModMinecraft.McInstance, b As ModMinecraft.McInstance) As Boolean
                Return(a Is Nothing AndAlso b Is Nothing) OrElse (a IsNot Nothing AndAlso b IsNot Nothing AndAlso PathUtils.IsSame(a.PathVersion, b.PathVersion))
            End Operator

            Public Shared Operator <>(a As ModMinecraft.McInstance, b As ModMinecraft.McInstance) As Boolean
                Return Not(a Is b)
            End Operator

            Private _Name As String

            Public Info As String

            Public State As ModMinecraft.McInstance.McInstanceState

            Public Logo As String

            Public IsStar As Boolean

            Public DisplayType As ModMinecraft.McInstanceCardType

            Private _Version As McVersion

            Private _JsonText As String

            Private _JsonObject As JObject

            Private _JsonVersion As JObject

            Private _InheritName As String

            Public Config As JsonConfigProvider

            Public IsLoaded As Boolean

            Private $STATIC$get_JsonVersion$2001281F1$JsonVersionInited As Boolean

            Private $STATIC$get_JsonVersion$2001281F1$JsonVersionInited$Init As StaticLocalInitFlag

            Public Enum McInstanceState
                [Error]
                Original
                Snapshot
                Fool
                OptiFine
                Old
                Forge
                NeoForge
                LiteLoader
                Fabric
            End Enum
        End Class

        Public NotInheritable Class McInstanceLock
            Friend Shared ReadOnly Property IsAnyLocked As Boolean
                Get
                    Return Not ModMinecraft.McInstanceLock.LockedPaths.IsEmpty
                End Get
            End Property

            Public Shared Function TryAcquire(Worker As IWorker, InstancePath As String) As Boolean
                InstancePath = PathUtils.ForCompare(InstancePath)
                Dim flag As Boolean
                If Not ModMinecraft.McInstanceLock.LockedPaths.TryAdd(InstancePath, Worker) Then
                    flag = False
                Else
                    AddHandler Worker.Stopped, Sub()
                        Dim lockedPaths As ConcurrentDictionary(Of String, IWorker) = ModMinecraft.McInstanceLock.LockedPaths
                        Dim $VB$Local_InstancePath As String = InstancePath
                        Dim worker As IWorker = Nothing
                        lockedPaths.TryRemove($VB$Local_InstancePath, worker)
                    End Sub
                    flag = True
                End If
                Return flag
            End Function

            Public Shared Function IsLocked(Instance As ModMinecraft.McInstance) As Boolean
                Return Instance IsNot Nothing AndAlso ModMinecraft.McInstanceLock.LockedPaths.ContainsKey(PathUtils.ForCompare(Instance.PathVersion))
            End Function

            Private Shared LockedPaths As ConcurrentDictionary(Of String, IWorker) = New ConcurrentDictionary(Of String, IWorker)(StringComparer.OrdinalIgnoreCase)
        End Class

        Public Enum McInstanceCardType
            Star = -1
            Auto
            Hidden
            API
            OriginalLike
            Rubbish
            Fool
            [Error]
        End Enum

        Public Structure McSkinInfo
            Public IsSlim As Boolean

            Public LocalFile As String

            Public IsVaild As Boolean
        End Structure

        Public Class McLibEntry
            Public Sub New()
                Me.Size = 0L
                Me.IsNatives = False
                Me.SHA1 = Nothing
            End Sub

            Public Property Url As String
                Get
                    Return Me._Url
                End Get
                Set(value As String)
                    Me._Url = If(String.IsNullOrWhiteSpace(value), Nothing, value)
                End Set
            End Property

            Public ReadOnly Property Name As String
                Get
                    Dim text As String
                    If Me.OriginalName Is Nothing Then
                        text = Nothing
                    Else
                        Dim list As List(Of String) = New List(Of String)(Me.OriginalName.Split(":", False))
                        list.RemoveAt(2)
                        text = list.Join(":"c)
                    End If
                    Return text
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return If(Me.IsNatives, "[Native] ", "") + StringUtils.FormatByteSize(Me.Size) + " | " + Me.LocalPath
            End Function

            Public LocalPath As String

            Public Size As Long

            Public IsNatives As Boolean

            Public SHA1 As String

            Private _Url As String

            Public OriginalName As String
        End Class

        Private Structure McAssetsToken
            Public Overrides Function ToString() As String
                Return StringUtils.FormatByteSize(Me.Size) + " | " + Me.LocalPath
            End Function

            Public LocalPath As String

            Public SourcePath As String

            Public Size As Long

            Public Hash As String
        End Structure
    End Module
End Namespace
