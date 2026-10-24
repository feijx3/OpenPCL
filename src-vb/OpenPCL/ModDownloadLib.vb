Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json.Linq
Imports PCLCS

Namespace PCL
    Public NotInheritable Module ModDownloadLib
        Private Function McDownloadClientLoader(Id As String, JsonUrl As String, VersionFolder As String) As List(Of ModLoader.LoaderBase)
            Dim InstanceName As String = PathUtils.GetLastPart(VersionFolder)
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            Dim NewInstance As ModMinecraft.McInstance = Nothing
            If JsonUrl Is Nothing Then
                list.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("获取原版 json 文件下载地址", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                    Dim text As String = Conversions.ToString(ModDownload.DlClientListGet(Id))
                    Task.Output = New List(Of ModNet.NetFile)() From { New ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), VersionFolder + InstanceName + ".json", Nothing, False) }
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 2.0, .Show = False })
            End If
            list.Add(New ModNet.LoaderDownload("下载原版 json 文件", New List(Of ModNet.NetFile)() From { New ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(If(JsonUrl, "")), VersionFolder + InstanceName + ".json", New FileChecker() With { .IsJson = True }, False) }) With { .ProgressWeight = 3.0 })
            list.Add(New ModLoader.LoaderTask(Of String, String)("读取 json 文件", Sub(Task As ModLoader.LoaderTask(Of String, String))
                NewInstance = New ModMinecraft.McInstance(VersionFolder)
                NewInstance.Load()
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.0, .Show = False })
            list.Add(New ModLoader.LoaderCombo(Of String)("下载原版支持库文件", New List(Of ModLoader.LoaderBase)() From { New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("分析原版支持库文件（副加载器）", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Logger.Info(String.Format("开始分析原版支持库文件：{0}", VersionFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                Task.Output = ModMinecraft.McLibNetFilesFromInstance(NewInstance)
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0, .Show = False }, New ModNet.LoaderDownload("下载原版支持库文件（副加载器）", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 13.0, .Show = False } }) With { .Block = False, .ProgressWeight = 14.0 })
            list.Add(New ModLoader.LoaderCombo(Of String)("下载原版资源文件", New List(Of ModLoader.LoaderBase)() From { New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("分析资源文件索引地址（副加载器）", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Try
                    Dim netFile As ModNet.NetFile = ModDownload.DlClientAssetIndexGet(NewInstance)
                    Dim list3 As List(Of ModNet.NetFile)
                    If netFile IsNot Nothing Then
                        Dim list2 As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                        list3 = list2
                        list2.Add(netFile)
                    Else
                        list3 = New List(Of ModNet.NetFile)()
                    End If
                    Task.Output = list3
                Catch ex As Exception
                    Throw New Exception("分析资源文件索引地址失败", ex)
                End Try
                Try
                    Dim jobject As JObject = CType(FileUtils.ReadAsJson(VersionFolder + InstanceName + ".json", Nothing, Nothing), JObject)
                    jobject("clientVersion") = Id
                    FileUtils.Write(String.Format("{0}{1}.json", VersionFolder, InstanceName), jobject.ToString(), Nothing)
                Catch ex2 As Exception
                    Throw New Exception("添加客户端版本失败", ex2)
                End Try
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0, .Show = False }, New ModNet.LoaderDownload("下载资源文件索引（副加载器）", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 3.0, .Show = False }, New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("分析所需资源文件（副加载器）", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)) = Task
                Dim $VB$Local_NewInstance As ModMinecraft.McInstance = NewInstance
                Dim flag As Boolean = True
                Dim loaderBase As ModLoader.LoaderBase = Task
                Dim list4 As List(Of ModNet.NetFile) = ModMinecraft.McAssetsFixList($VB$Local_NewInstance, flag, loaderBase)
                Task = CType(loaderBase, ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                loaderTask.Output = list4
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.01, .Show = False }, New ModNet.LoaderDownload("下载资源文件（副加载器）", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 14.0, .Show = False } }) With { .Block = False, .ProgressWeight = 18.0 })
            Return list
        End Function

        Public Function McDownloadListItem(Entry As JObject, OnClick As OurListItem.ClickEventHandler, IsSaveOnly As Boolean) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim jtoken As JToken = Entry("type")
                Dim text As String
                If CStr(jtoken) = "release" Then
                    text = ModBase.PathImage + "Blocks/Grass.png"
                ElseIf CStr(jtoken) = "snapshot" Then
                    text = ModBase.PathImage + "Blocks/CommandBlock.png"
                ElseIf CStr(jtoken) = "special" Then
                    text = ModBase.PathImage + "Blocks/GoldBlock.png"
                Else
                    text = ModBase.PathImage + "Blocks/CobbleStone.png"
                End If
                Dim myListItem As OurListItem = New OurListItem() With { .Logo = text, .SnapsToDevicePixels = True, .Title = Entry("id").ToString(), .Height = 42.0, .Type = OurListItem.CheckType.Clickable, .Tag = Entry }
                If Entry("lore") Is Nothing Then
                    myListItem.Info = Extensions.Value(Of DateTime)(Entry("releaseTime")).ToString("yyyy'/'MM'/'dd HH':'mm")
                Else
                    myListItem.Info = Entry("lore").ToString()
                End If
                If Entry("url").ToString().Contains("pcl") Then
                    myListItem.Info = "[PCL 特供下载] " + myListItem.Info
                End If
                AddHandler myListItem.Click, OnClick
                If IsSaveOnly Then
                    myListItem.ContentHandler = AddressOf ModDownloadLib.McDownloadSaveMenuBuild
                Else
                    myListItem.ContentHandler = AddressOf ModDownloadLib.McDownloadMenuBuild
                End If
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Private Sub McDownloadSaveMenuBuild(sender As Object, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR5-1 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR5-1
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.McDownloadMenuLog(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR5-1 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            Dim myIconButton3 As OurIconButton = New OurIconButton() With { .LogoScale = 1.0, .Logo = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z", .ToolTip = "下载服务端" }
            Dim myIconButton4 As OurIconButton = myIconButton3
            Dim clickEventHandler3 As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR5-2 IsNot Nothing Then
                clickEventHandler3 = ModDownloadLib._Closure$__.$IR5-2
            Else
                Dim clickEventHandler4 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.McDownloadMenuSaveServer(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler3 = clickEventHandler4
                ModDownloadLib._Closure$__.$IR5-2 = clickEventHandler4
            End If
            AddHandler myIconButton4.Click, clickEventHandler3
            NewLateBinding.LateSet(sender, Nothing, "Buttons", New Object() { New OurIconButton() { myIconButton3, myIconButton } }, Nothing, Nothing)
        End Sub

        Private Sub McDownloadMenuBuild(sender As Object, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z", .ToolTip = "另存为" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR6-3 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR6-3
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.McDownloadMenuSave(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR6-3 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            Dim myIconButton3 As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton4 As OurIconButton = myIconButton3
            Dim clickEventHandler3 As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR6-4 IsNot Nothing Then
                clickEventHandler3 = ModDownloadLib._Closure$__.$IR6-4
            Else
                Dim clickEventHandler4 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.McDownloadMenuLog(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler3 = clickEventHandler4
                ModDownloadLib._Closure$__.$IR6-4 = clickEventHandler4
            End If
            AddHandler myIconButton4.Click, clickEventHandler3
            Dim myIconButton5 As OurIconButton = New OurIconButton() With { .LogoScale = 1.0, .Logo = "M224 160a64 64 0 0 0-64 64v576a64 64 0 0 0 64 64h576a64 64 0 0 0 64-64V224a64 64 0 0 0-64-64H224z m0 384h576v256H224v-256z m192 96v64h320v-64H416z m-128 0v64h64v-64H288zM224 224h576v256H224V224z m192 96v64h320v-64H416z m-128 0v64h64v-64H288z", .ToolTip = "下载服务端" }
            Dim myIconButton6 As OurIconButton = myIconButton5
            Dim clickEventHandler5 As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR6-5 IsNot Nothing Then
                clickEventHandler5 = ModDownloadLib._Closure$__.$IR6-5
            Else
                Dim clickEventHandler6 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.McDownloadMenuSaveServer(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler5 = clickEventHandler6
                ModDownloadLib._Closure$__.$IR6-5 = clickEventHandler6
            End If
            AddHandler myIconButton6.Click, clickEventHandler5
            NewLateBinding.LateSet(sender, Nothing, "Buttons", New Object() { New OurIconButton() { myIconButton, myIconButton3, myIconButton5 } }, Nothing, Nothing)
        End Sub

        Private Sub McDownloadMenuLog(sender As Object, e As RoutedEventArgs)
            Dim jtoken As JToken
            If NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                jtoken = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), JToken)
            ElseIf NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                jtoken = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), JToken)
            Else
                jtoken = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), JToken)
            End If
            ModDownloadLib.McUpdateLogShow(jtoken)
        End Sub

        Private Sub McDownloadMenuSaveServer(sender As Object, e As RoutedEventArgs)
            Dim myListItem As OurListItem
            If TypeOf sender Is OurListItem Then
                myListItem = CType(sender, OurListItem)
            ElseIf TypeOf NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing)Is OurListItem Then
                myListItem = CType(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), OurListItem)
            Else
                myListItem = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), OurListItem)
            End If
            Try
                Dim CS$<>8__locals1 As ModDownloadLib._Closure$__8-0 = New ModDownloadLib._Closure$__8-0(CS$<>8__locals1)
                CS$<>8__locals1.$VB$Local_Id = myListItem.Title
                Dim text As String = NewLateBinding.LateIndexGet(myListItem.Tag, New Object() { "url" }, Nothing).ToString()
                CS$<>8__locals1.$VB$Local_VersionFolder = Dialogs.SelectFolder("选择目标文件夹", False, Nothing).FirstOrDefault()
                If CS$<>8__locals1.$VB$Local_VersionFolder IsNot Nothing Then
                    CS$<>8__locals1.$VB$Local_VersionFolder = CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + "\"
                    Try
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator = ModLoader.LoaderTaskbar.ToList().GetEnumerator()
                        While enumerator.MoveNext()
                            If Operators.CompareString(enumerator.Current.Name, String.Format("Minecraft {0} 服务端下载", CS$<>8__locals1.$VB$Local_Id), False) = 0 Then
                                ModMain.Hint("该服务端正在下载中！", ModMain.HintType.Red, True)
                                Return
                            End If
                        End While
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                    list.Add(New ModNet.LoaderDownload("下载版本 JSON 文件", New List(Of ModNet.NetFile)() From { New ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", New FileChecker() With { .IsJson = True }, False) }) With { .ProgressWeight = 2.0 })
                    list.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("构建服务端", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                        Dim mcInstance As ModMinecraft.McInstance = New ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder)
                        If mcInstance.JsonObject("downloads") Is Nothing OrElse mcInstance.JsonObject("downloads")("server") Is Nothing OrElse mcInstance.JsonObject("downloads")("server")("url") Is Nothing Then
                            FileUtils.Delete(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", False)
                            If Not DirectoryUtils.GetInfo(CS$<>8__locals1.$VB$Local_VersionFolder).GetFileSystemInfos().Any() Then
                                DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_VersionFolder, False)
                            End If
                            Task.Output = New List(Of ModNet.NetFile)()
                            ModMain.Hint(String.Format("Mojang 没有给 Minecraft {0} 提供官方服务端下载，没法下，撤退！", CS$<>8__locals1.$VB$Local_Id), ModMain.HintType.Red, True)
                            Thread.Sleep(2000)
                            Task.Cancel()
                            Return
                        End If
                        Dim text2 As String = CStr(mcInstance.JsonObject("downloads")("server")("url"))
                        Dim fileChecker As FileChecker = New FileChecker() With { .MinSize = 1024L, .ActualSize = CLng((If(mcInstance.JsonObject("downloads")("server")("size"), (-1)))), .Hash = CStr(mcInstance.JsonObject("downloads")("server")("sha1")) }
                        Task.Output = New List(Of ModNet.NetFile)() From { New ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text2), CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + "-server.jar", fileChecker, False) }
                        Dim text3 As String = String.Format("@echo off" & vbCrLf & "title {0} 原版服务端" & vbCrLf & "echo 如果服务端立即停止，请右键编辑该脚本，将下一行开头的 java 替换为适合该 Minecraft 版本的完整 java.exe 的路径。" & vbCrLf & "echo 你可以在 PCL 的 [设置 → 启动选项] 中查看已安装的 java，所需的 java.exe 一般在其中的 bin 文件夹下。" & vbCrLf & "echo ------------------------------" & vbCrLf & "echo 如果提示 ""You need to agree to the EULA in order to run the server""，请打开 eula.txt，按说明阅读并同意 Minecraft EULA 后，将该文件最后一行中的 eula=false 改为 eula=true。" & vbCrLf & "echo ------------------------------" & vbCrLf & """java"" -server -XX:+UseG1GC -Xmx6144M -Xms1024M -jar {1}-server.jar nogui" & vbCrLf & "echo ----------------------" & vbCrLf & "echo 服务端已停止。" & vbCrLf & "pause", CS$<>8__locals1.$VB$Local_Id, CS$<>8__locals1.$VB$Local_Id)
                        FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + "Launch Server.bat", text3, If(Encoding.[Default].Equals(Encoding.UTF8), Encoding.UTF8, Encoding.GetEncoding("GB18030")))
                        FileUtils.Delete(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", False)
                    End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.5, .Show = False })
                    list.Add(New ModNet.LoaderDownload("下载服务端文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 5.0 })
                    Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("Minecraft " + CS$<>8__locals1.$VB$Local_Id + " 服务端下载", list)
                    loaderCombo.OnStateChanged = AddressOf ModDownloadLib.LoaderStateChangedHintOnly
                    loaderCombo.Start(CS$<>8__locals1.$VB$Local_Id, False)
                    ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                End If
            Catch ex As Exception
                Logger.[Error](ex, "开始 Minecraft 服务端下载失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Try
        End Sub

        Public Sub McDownloadMenuSave(sender As Object, e As RoutedEventArgs)
            Dim myListItem As OurListItem
            If TypeOf sender Is OurListItem Then
                myListItem = CType(sender, OurListItem)
            ElseIf TypeOf NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing)Is OurListItem Then
                myListItem = CType(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), OurListItem)
            Else
                myListItem = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), OurListItem)
            End If
            Try
                Dim CS$<>8__locals1 As ModDownloadLib._Closure$__9-0 = New ModDownloadLib._Closure$__9-0(CS$<>8__locals1)
                Dim title As String = myListItem.Title
                Dim text As String = NewLateBinding.LateIndexGet(myListItem.Tag, New Object() { "url" }, Nothing).ToString()
                CS$<>8__locals1.$VB$Local_VersionFolder = Dialogs.SelectFolder("选择目标文件夹", False, Nothing).FirstOrDefault()
                If CS$<>8__locals1.$VB$Local_VersionFolder IsNot Nothing Then
                    CS$<>8__locals1.$VB$Local_VersionFolder = CS$<>8__locals1.$VB$Local_VersionFolder + title + "\"
                    Try
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator = ModLoader.LoaderTaskbar.ToList().GetEnumerator()
                        While enumerator.MoveNext()
                            If Operators.CompareString(enumerator.Current.Name, String.Format("Minecraft {0} 下载", title), False) = 0 Then
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, True)
                                Return
                            End If
                        End While
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                    list.Add(New ModNet.LoaderDownload("下载版本 JSON 文件", New List(Of ModNet.NetFile)() From { New ModNet.NetFile(ModDownload.DlSourceLauncherOrMetaGet(text), CS$<>8__locals1.$VB$Local_VersionFolder + title + ".json", New FileChecker() With { .IsJson = True }, False) }) With { .ProgressWeight = 2.0 })
                    list.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("分析版本 JAR 文件下载地址", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                        Task.Output = New List(Of ModNet.NetFile)() From { ModDownload.DlClientJarGet(New ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder), False) }
                    End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.5, .Show = False })
                    list.Add(New ModNet.LoaderDownload("下载版本 JAR 文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 5.0 })
                    Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("Minecraft " + title + " 下载", list)
                    loaderCombo.OnStateChanged = AddressOf ModDownloadLib.LoaderStateChangedHintOnly
                    loaderCombo.Start(title, False)
                    ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                End If
            Catch ex As Exception
                Logger.[Error](ex, "开始 Minecraft 下载失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Try
        End Sub

        Public Sub McUpdateLogShow(InstanceJson As JToken)
            Dim text As String = InstanceJson("id").ToString().Lower()
            Dim text2 As String
            If Operators.CompareString(text, "3d shareware v1.34", False) = 0 Then
                text2 = "3D_Shareware_v1.34"
            ElseIf Operators.CompareString(text, "2.0", False) = 0 Then
                text2 = "Java版2.0"
            ElseIf Operators.CompareString(text, "1.rv-pre1", False) = 0 Then
                text2 = "Java版1.RV-Pre1"
            ElseIf Operators.CompareString(text, "combat test 1", False) = 0 OrElse text.Contains("combat-1") OrElse text.Contains("combat-212796") Then
                text2 = "Java版1.14.3_-_Combat_Test"
            ElseIf Operators.CompareString(text, "combat test 2", False) = 0 OrElse text.Contains("combat-2") OrElse text.Contains("combat-0") Then
                text2 = "Java版Combat_Test_2"
            ElseIf Operators.CompareString(text, "combat test 3", False) = 0 OrElse Operators.CompareString(text, "1.14_combat-3", False) = 0 Then
                text2 = "Java版Combat_Test_3"
            ElseIf Operators.CompareString(text, "combat test 4", False) = 0 OrElse Operators.CompareString(text, "1.15_combat-1", False) = 0 Then
                text2 = "Java版Combat_Test_4"
            ElseIf Operators.CompareString(text, "combat test 5", False) = 0 OrElse Operators.CompareString(text, "1.15_combat-6", False) = 0 Then
                text2 = "Java版Combat_Test_5"
            ElseIf Operators.CompareString(text, "combat test 6", False) = 0 OrElse Operators.CompareString(text, "1.16_combat-0", False) = 0 Then
                text2 = "Java版Combat_Test_6"
            ElseIf Operators.CompareString(text, "combat test 7c", False) = 0 OrElse Operators.CompareString(text, "1.16_combat-3", False) = 0 Then
                text2 = "Java版Combat_Test_7c"
            ElseIf Operators.CompareString(text, "combat test 8b", False) = 0 OrElse Operators.CompareString(text, "1.16_combat-5", False) = 0 Then
                text2 = "Java版Combat_Test_8b"
            ElseIf Operators.CompareString(text, "combat test 8c", False) = 0 OrElse Operators.CompareString(text, "1.16_combat-6", False) = 0 Then
                text2 = "Java版Combat_Test_8c"
            ElseIf Operators.CompareString(text, "1.0.0-rc2-2", False) = 0 Then
                text2 = "Java版RC2"
            ElseIf text.StartsWithF("1.19_deep_dark_experimental_snapshot-", False) OrElse text.StartsWithF("1_19_deep_dark_experimental_snapshot-", False) Then
                text2 = text.Replace("1_19", "1.19").Replace("1.19_deep_dark_experimental_snapshot-", "Java版Deep_Dark_Experimental_Snapshot_")
            ElseIf Operators.CompareString(text, "b1.9-pre6", False) = 0 Then
                text2 = "Java版Beta_1.9_Prerelease_6"
            ElseIf text.Contains("b1.9") Then
                text2 = "Java版Beta_1.9_Prerelease"
            ElseIf CStr(InstanceJson("type")) = "release" OrElse CStr(InstanceJson("type")) = "snapshot" OrElse CStr(InstanceJson("type")) = "special" Then
                text2 = If(text.Contains("w"), "", "Java版") + text.Replace(" Pre-Release ", "-pre")
            ElseIf text.StartsWithF("b", False) Then
                text2 = "Java版" + text.TrimEnd(New Char() { "a"c, "b"c, "c"c, "d"c, "e"c }).Replace("b", "Beta_")
            ElseIf text.StartsWithF("a", False) Then
                text2 = "Java版" + text.TrimEnd(New Char() { "a"c, "b"c, "c"c, "d"c, "e"c }).Replace("a", "Alpha_v")
            ElseIf Operators.CompareString(text, "inf-20100618", False) = 0 Then
                text2 = "Java版Infdev_20100618"
            ElseIf Operators.CompareString(text, "c0.30_01c", False) = 0 OrElse Operators.CompareString(text, "c0.30_survival", False) = 0 OrElse text.Contains("生存测试") Then
                text2 = "Java版Classic_0.30（生存模式）"
            ElseIf text.StartsWithF("c0.31", False) Then
                text2 = "Java版Indev_0.31_20100130"
            ElseIf text.StartsWithF("c", False) Then
                text2 = "Java版" + text.Replace("c", "Classic_")
            Else
                If Not text.StartsWithF("rd-", False) Then
                    Logger.[Error](String.Format("未知的版本格式：{0}。", text), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                    Return
                End If
                text2 = "Java版Pre-classic_" + text
            End If
            ModBase.OpenWebsite("https://zh.minecraft.wiki/w/" + text2.Replace("_experimental-snapshot-", "-exp"))
        End Sub

        Private Sub McDownloadOptiFineSave(DownloadInfo As ModDownload.DlOptiFineListEntry)
            Try
                Dim text As String = Dialogs.SaveFile("选择保存位置", DownloadInfo.FileName, Nothing, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)("jar", "OptiFine Jar") })
                If text IsNot Nothing Then
                    Try
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator = ModLoader.LoaderTaskbar.ToList().GetEnumerator()
                        While enumerator.MoveNext()
                            If Operators.CompareString(enumerator.Current.Name, String.Format("OptiFine {0} 下载", DownloadInfo.DisplayName), False) = 0 Then
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, True)
                                Return
                            End If
                        End While
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Dim loaderCombo As ModLoader.LoaderCombo(Of ModDownload.DlOptiFineListEntry) = New ModLoader.LoaderCombo(Of ModDownload.DlOptiFineListEntry)("OptiFine " + DownloadInfo.DisplayName + " 下载", ModDownloadLib.McDownloadOptiFineSaveLoader(DownloadInfo, text))
                    loaderCombo.OnStateChanged = AddressOf ModDownloadLib.LoaderStateChangedHintOnly
                    loaderCombo.Start(DownloadInfo, False)
                    ModLoader.LoaderTaskbarAdd(Of ModDownload.DlOptiFineListEntry)(loaderCombo)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                End If
            Catch ex As Exception
                Logger.[Error](ex, "开始 OptiFine 下载失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Try
        End Sub

        Private Sub McDownloadOptiFineInstall(BaseMcFolderHome As String, Target As String, Task As ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean), UseJavaWrapper As Boolean)
            Dim CS$<>8__locals1 As ModDownloadLib._Closure$__12-2 = New ModDownloadLib._Closure$__12-2(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Task = Task
            Dim valueRange As ValueRange(Of Version) = ValueRange(Of Version).AtLeast(New Version(21, 0))
            Try
                Using zipArchive As ZipArchive = FileUtils.OpenZip(Target, Nothing)
                    Dim entry As ZipArchiveEntry = zipArchive.GetEntry("optifine/Installer.class")
                    If entry Is Nothing Then
                        Throw New FileNotFoundException("未找到 optifine/Installer.class")
                    End If
                    Using stream As Stream = entry.Open()
                        Using binaryReader As BinaryReader = New BinaryReader(stream)
                            Dim array As Byte() = binaryReader.ReadBytes(8)
                            If array.Length < 8 Then
                                Throw New IOException("Installer.class 长度不足")
                            End If
                            If array(0) <> 202 OrElse array(1) <> 254 OrElse array(2) <> 186 OrElse array(3) <> 190 Then
                                Throw New InvalidDataException("Installer.class 文件头无效")
                            End If
                            Dim num As Integer = CInt(array(6)) * 256 + CInt(array(7))
                            If num < 49 Then
                                Throw New InvalidDataException("Installer.class 版本过低")
                            End If
                            If num > 100 Then
                                Throw New InvalidDataException("Installer.class 版本过高")
                            End If
                            valueRange = ValueRange(Of Version).AtLeast(New Version(num - 44, 0))
                            Logger.Info(String.Format("OptiFine 安装器 class 版本：{0}，需要 Java {1}", num, valueRange), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Logger.Warn(ex, "读取 OptiFine 安装器 Java 版本失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Try
            Dim java As Java = ModJava.SelectOrDownloadJava(valueRange, True, CS$<>8__locals1.$VB$Local_Task.CreateCancellationToken(), CS$<>8__locals1.$VB$Local_Task.CreateSyncProgressProvider(0.0, 0.2))
            If Not CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                If java Is Nothing Then
                    Throw New OperationCanceledException()
                End If
                Dim text As String
                If UseJavaWrapper AndAlso Not Settings.[Get](Of Boolean)("LaunchAdvanceDisableJLW", Nothing) Then
                    text = String.Format("-Doolloo.jlw.tmpdir=""{0}"" -Duser.home=""{1}"" -cp ""{2}"" -jar ""{3}"" optifine.Installer", New Object() { PathUtils.RemoveSlashSuffix(ModBase.PathPure.Value), PathUtils.RemoveSlashSuffix(BaseMcFolderHome), Target, LaunchUtils.ExtractPatch("Launch\JavaWrapper.jar", ModBase.PathPure.Value) })
                Else
                    text = String.Format("-Duser.home=""{0}"" -cp ""{1}"" optifine.Installer", PathUtils.RemoveSlashSuffix(BaseMcFolderHome), Target)
                End If
                If java.Version.Major >= 9 Then
                    text = "--add-exports cpw.mods.bootstraplauncher/cpw.mods.bootstraplauncher=ALL-UNNAMED " + text
                End If
                Dim installSyncLock As Object = ModDownloadLib.InstallSyncLock
                ObjectFlowControl.CheckForSyncLockOnValueType(installSyncLock)
                SyncLock installSyncLock
                    Dim CS$<>8__locals2 As ModDownloadLib._Closure$__12-1 = New ModDownloadLib._Closure$__12-1(CS$<>8__locals2)
                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                    Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With { .FileName = java.JavaExePath, .Arguments = text, .UseShellExecute = False, .CreateNoWindow = True, .RedirectStandardError = True, .RedirectStandardOutput = True, .WorkingDirectory = PathUtils.ToShortPath(BaseMcFolderHome) }
                    If processStartInfo.EnvironmentVariables.ContainsKey("appdata") Then
                        processStartInfo.EnvironmentVariables("appdata") = BaseMcFolderHome
                    Else
                        processStartInfo.EnvironmentVariables.Add("appdata", BaseMcFolderHome)
                    End If
                    Logger.Info(String.Format("开始安装 OptiFine：{0}", Target), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                    CS$<>8__locals2.$VB$Local_TotalLength = 0
                    CS$<>8__locals2.$VB$Local_process = New Process() With { .StartInfo = processStartInfo }
                    CS$<>8__locals2.$VB$Local_LastResult = ""
                    Dim CS$<>8__locals3 As ModDownloadLib._Closure$__12-0 = New ModDownloadLib._Closure$__12-0(CS$<>8__locals3)
                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals2
                    CS$<>8__locals3.$VB$Local_outputWaitHandle = New AutoResetEvent(False)
                    Try
                        Dim CS$<>8__locals4 As ModDownloadLib._Closure$__12-3 = New ModDownloadLib._Closure$__12-3(CS$<>8__locals4)
                        CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4 = CS$<>8__locals3
                        CS$<>8__locals4.$VB$Local_errorWaitHandle = New AutoResetEvent(False)
                        Try
                            AddHandler CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.OutputDataReceived, Sub(a0 As Object, a1 As DataReceivedEventArgs)
                                MyBase._Lambda$__0(RuntimeHelpers.GetObjectValue(a0), a1)
                            End Sub
                            AddHandler CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.ErrorDataReceived, Sub(a0 As Object, a1 As DataReceivedEventArgs)
                                MyBase._Lambda$__1(RuntimeHelpers.GetObjectValue(a0), a1)
                            End Sub
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Start()
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginOutputReadLine()
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginErrorReadLine()
                            While Not CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.HasExited
                                Thread.Sleep(10)
                            End While
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$Local_outputWaitHandle.WaitOne(10000)
                            CS$<>8__locals4.$VB$Local_errorWaitHandle.WaitOne(10000)
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Dispose()
                            If CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_TotalLength < 1000 OrElse CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResult.Contains("at ") Then
                                Throw New Exception("安装器运行出错，末行为 " + CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResult)
                            End If
                        Finally
                            If CS$<>8__locals4.$VB$Local_errorWaitHandle IsNot Nothing Then
                                CType(CS$<>8__locals4.$VB$Local_errorWaitHandle, IDisposable).Dispose()
                            End If
                        End Try
                    Finally
                        If CS$<>8__locals3.$VB$Local_outputWaitHandle IsNot Nothing Then
                            CType(CS$<>8__locals3.$VB$Local_outputWaitHandle, IDisposable).Dispose()
                        End If
                    End Try
                End SyncLock
            End If
        End Sub

        Private Function McDownloadOptiFineLoader(DownloadInfo As ModDownload.DlOptiFineListEntry, McFolder As String, ClientDownloadLoader As ModLoader.LoaderCombo(Of String), ClientFolder As String) As List(Of ModLoader.LoaderBase)
            Dim CS$<>8__locals1 As ModDownloadLib._Closure$__13-0 = New ModDownloadLib._Closure$__13-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_DownloadInfo = DownloadInfo
            CS$<>8__locals1.$VB$Local_McFolder = McFolder
            CS$<>8__locals1.$VB$Local_ClientDownloadLoader = ClientDownloadLoader
            CS$<>8__locals1.$VB$Local_ClientFolder = ClientFolder
            CS$<>8__locals1.$VB$Local_Id = CS$<>8__locals1.$VB$Local_DownloadInfo.InstanceName
            CS$<>8__locals1.$VB$Local_VersionFolder = CS$<>8__locals1.$VB$Local_McFolder + "versions\" + CS$<>8__locals1.$VB$Local_Id + "\"
            Dim flag As Boolean = CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit.Contains("w") OrElse McVersion.VersionToDrop(CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit) >= 140
            CS$<>8__locals1.$VB$Local_Target = If(flag, String.Format("{0}OptiFine.jar", ModMain.RequestTaskTempFolder(True)), String.Format("{0}libraries\optifine\OptiFine\{1}\{2}", CS$<>8__locals1.$VB$Local_McFolder, PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_DownloadInfo.FileName).Replace("OptiFine_", "").Replace("preview_", ""), CS$<>8__locals1.$VB$Local_DownloadInfo.FileName.Replace("OptiFine_", "OptiFine-").Replace("preview_", "")))
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            list.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("获取 OptiFine 主文件下载地址", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Dim list2 As List(Of String) = New List(Of String)()
                Dim text As String = CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit
                If Operators.CompareString(text, "1.8", False) = 0 OrElse Operators.CompareString(text, "1.9", False) = 0 Then
                    text += ".0"
                End If
                If CS$<>8__locals1.$VB$Local_DownloadInfo.IsPreview Then
                    list2.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U_" + CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName.Replace(CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit + " ", "").Replace(" ", "/"))
                Else
                    list2.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U/" + CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName.Replace(CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit + " ", ""))
                End If
                Try
                    Dim text2 As String = ModNet.NetRequestByClientRetry("https://optifine.net/adloadx?f=" + CS$<>8__locals1.$VB$Local_DownloadInfo.FileName, Nothing, Nothing, Nothing, Nothing, "text/html", Nothing, False, New UTF8Encoding(False), True)
                    Task.Progress = 0.8
                    list2.Add("https://optifine.net/" + text2.RegexSearch("downloadx\?f=[^""']+", RegexOptions.None).First())
                    Logger.Info(String.Format("OptiFine {0} 官方下载地址：{1}", CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName, list2.Last()), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("获取 OptiFine {0} 官方下载地址失败", CS$<>8__locals1.$VB$Local_DownloadInfo.DisplayName), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                End Try
                Task.Output = New List(Of ModNet.NetFile)() From { New ModNet.NetFile(list2.ToArray(), CS$<>8__locals1.$VB$Local_Target, New FileChecker() With { .MinSize = 307200L }, False) }
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 8.0 })
            list.Add(New ModNet.LoaderDownload("下载 OptiFine 主文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 8.0 })
            list.Add(New ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean)("等待原版下载", Sub(Task As ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean))
                Dim loaderList As IEnumerable(Of ModLoader.LoaderBase) = CS$<>8__locals1.$VB$Local_ClientDownloadLoader.GetLoaderList(True)
                Dim func As Func(Of ModLoader.LoaderBase, Boolean)
                If ModDownloadLib._Closure$__.$I13-2 IsNot Nothing Then
                    func = ModDownloadLib._Closure$__.$I13-2
                Else
                    Dim func2 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) Operators.CompareString(l.Name, "下载原版支持库文件", False) = 0 OrElse Operators.CompareString(l.Name, "下载原版 json 文件", False) = 0
                    func = func2
                    ModDownloadLib._Closure$__.$I13-2 = func2
                End If
                Dim enumerable As IEnumerable(Of ModLoader.LoaderBase) = loaderList.Where(func)
                Dim func3 As Func(Of ModLoader.LoaderBase, Boolean)
                If ModDownloadLib._Closure$__.$I13-3 IsNot Nothing Then
                    func3 = ModDownloadLib._Closure$__.$I13-3
                Else
                    Dim func4 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) l.State <> ModBase.LoadState.Finished
                    func3 = func4
                    ModDownloadLib._Closure$__.$I13-3 = func4
                End If
                Dim list3 As List(Of ModLoader.LoaderBase) = enumerable.Where(func3).ToList()
                If list3.Any() Then
                    Logger.Info("OptiFine 安装正在等待原版文件下载完成", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                End If
                While list3.Any() AndAlso Not Task.IsCanceled
                    Dim enumerable2 As IEnumerable(Of ModLoader.LoaderBase) = list3
                    Dim func5 As Func(Of ModLoader.LoaderBase, Boolean)
                    If ModDownloadLib._Closure$__.$I13-4 IsNot Nothing Then
                        func5 = ModDownloadLib._Closure$__.$I13-4
                    Else
                        Dim func6 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) l.State <> ModBase.LoadState.Finished
                        func5 = func6
                        ModDownloadLib._Closure$__.$I13-4 = func6
                    End If
                    list3 = enumerable2.Where(func5).ToList()
                    Thread.Sleep(50)
                End While
                If Not Task.IsCanceled Then
                    Dim vanillaSyncLock As Object = ModDownloadLib.VanillaSyncLock
                    ObjectFlowControl.CheckForSyncLockOnValueType(vanillaSyncLock)
                    SyncLock vanillaSyncLock
                        Dim lastPart As String = PathUtils.GetLastPart(CS$<>8__locals1.$VB$Local_ClientFolder)
                        DirectoryUtils.Create(CS$<>8__locals1.$VB$Local_McFolder + "versions\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit)
                        If Not FileUtils.Exists(String.Concat(New String() { CS$<>8__locals1.$VB$Local_McFolder, "versions\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, "\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, ".json" })) Then
                            FileUtils.Copy(String.Format("{0}{1}.json", CS$<>8__locals1.$VB$Local_ClientFolder, lastPart), String.Format("{0}versions\{1}\{2}.json", CS$<>8__locals1.$VB$Local_McFolder, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit))
                        End If
                        If Not FileUtils.Exists(String.Concat(New String() { CS$<>8__locals1.$VB$Local_McFolder, "versions\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, "\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, ".jar" })) Then
                            FileUtils.Copy(String.Format("{0}{1}.jar", CS$<>8__locals1.$VB$Local_ClientFolder, lastPart), String.Format("{0}versions\{1}\{2}.jar", CS$<>8__locals1.$VB$Local_McFolder, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit))
                        End If
                    End SyncLock
                End If
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.1, .Show = False })
            If flag Then
                Logger.Info(String.Format("检测为新版 OptiFine：{0}", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                list.Add(New ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean)("安装 OptiFine（方式 A）", Sub(Task As ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean))
                    Dim text3 As String = ModMain.RequestTaskTempFolder(True)
                    Dim text4 As String = text3 + ".minecraft\"
                    Try
                        If DirectoryUtils.Exists(text4 + "versions\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit) Then
                            DirectoryUtils.Delete(text4 + "versions\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, False)
                        End If
                        DirectoryUtils.Create(text4 + "versions\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit + "\")
                        ModMinecraft.McFolderLauncherProfilesJsonCreate(text4)
                        FileUtils.Copy(String.Concat(New String() { CS$<>8__locals1.$VB$Local_McFolder, "versions\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, "\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, ".json" }), String.Concat(New String() { text4, "versions\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, "\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, ".json" }))
                        FileUtils.Copy(String.Concat(New String() { CS$<>8__locals1.$VB$Local_McFolder, "versions\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, "\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, ".jar" }), String.Concat(New String() { text4, "versions\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, "\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, ".jar" }))
                        Task.Progress = 0.06
                        Dim flag3 As Boolean = True
                        While True
                            Try
                                ModDownloadLib.McDownloadOptiFineInstall(text3, CS$<>8__locals1.$VB$Local_Target, Task, flag3)
                            Catch ex2 As Exception
                                If flag3 Then
                                    Logger.Warn(ex2, "使用 JavaWrapper 安装 OptiFine 失败，将不使用 JavaWrapper 并重试", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                                    flag3 = False
                                    Continue For
                                End If
                                Throw New Exception("运行 OptiFine 安装器失败", ex2)
                            End Try
                            Exit For
                        End While
                        Task.Progress = 0.96
                        FileUtils.Delete(text4 + "launcher_profiles.json", False)
                        DirectoryUtils.Copy(text4, CS$<>8__locals1.$VB$Local_McFolder)
                        Task.Progress = 0.98
                        FileUtils.Delete(CS$<>8__locals1.$VB$Local_Target, False)
                        DirectoryUtils.Delete(text3, False)
                    Catch ex3 As Exception
                        Throw New Exception("安装 OptiFine（方式 A）失败", ex3)
                    End Try
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 8.0 })
            Else
                Logger.Info(String.Format("检测为旧版 OptiFine：{0}", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                list.Add(New ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean)("安装 OptiFine（方式 B）", Sub(Task As ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean))
                    Try
                        FileUtils.Copy(String.Concat(New String() { CS$<>8__locals1.$VB$Local_McFolder, "versions\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, "\", CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, ".jar" }), CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".jar")
                        Task.Progress = 0.7
                        Dim mcInstance As ModMinecraft.McInstance = New ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_McFolder + "versions\" + CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit)
                        Dim text5 As String = String.Concat(New String() { "{" & vbCrLf & "    ""id"": """, CS$<>8__locals1.$VB$Local_Id, """," & vbCrLf & "    ""inheritsFrom"": """, CS$<>8__locals1.$VB$Local_DownloadInfo.Inherit, """," & vbCrLf & "    ""time"": """, If((Operators.CompareString(CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime, "", False) = 0), mcInstance.ReleaseTime.ToString("yyyy'-'MM'-'dd"), CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime.Replace("/", "-")), "T23:33:33+08:00""," & vbCrLf & "    ""releaseTime"": """, If((Operators.CompareString(CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime, "", False) = 0), mcInstance.ReleaseTime.ToString("yyyy'-'MM'-'dd"), CS$<>8__locals1.$VB$Local_DownloadInfo.ReleaseTime.Replace("/", "-")), "T23:33:33+08:00""," & vbCrLf & "    ""type"": ""release""," & vbCrLf & "    ""libraries"": [" & vbCrLf & "        {""name"": ""optifine:OptiFine:", PathUtils.GetFileNameWithoutExtension(CS$<>8__locals1.$VB$Local_DownloadInfo.FileName).Replace("OptiFine_", "").Replace("preview_", ""), """}," & vbCrLf & "        {""name"": ""net.minecraft:launchwrapper:1.12""}" & vbCrLf & "    ]," & vbCrLf & "    ""mainClass"": ""net.minecraft.launchwrapper.Launch""," })
                        Task.Progress = 0.8
                        If mcInstance.IsOldJson Then
                            text5 = text5 + vbCrLf & "    ""minimumLauncherVersion"": 18," & vbCrLf & "    ""minecraftArguments"": """ + mcInstance.JsonObject("minecraftArguments").ToString() + "  --tweakClass optifine.OptiFineTweaker""" & vbCrLf & "}"
                        Else
                            text5 += vbCrLf & "    ""minimumLauncherVersion"": ""21""," & vbCrLf & "    ""arguments"": {" & vbCrLf & "        ""game"": [" & vbCrLf & "            ""--tweakClass""," & vbCrLf & "            ""optifine.OptiFineTweaker""" & vbCrLf & "        ]" & vbCrLf & "    }" & vbCrLf & "}"
                        End If
                        FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_Id + ".json", text5, Nothing)
                    Catch ex4 As Exception
                        Throw New Exception("安装 OptiFine（方式 B）失败", ex4)
                    End Try
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 })
            End If
            Return list
        End Function

        Private Function McDownloadOptiFineSaveLoader(DownloadInfo As ModDownload.DlOptiFineListEntry, TargetFolder As String) As List(Of ModLoader.LoaderBase)
            Return New List(Of ModLoader.LoaderBase)() From { New ModLoader.LoaderTask(Of ModDownload.DlOptiFineListEntry, List(Of ModNet.NetFile))("获取 OptiFine 下载地址", Sub(Task As ModLoader.LoaderTask(Of ModDownload.DlOptiFineListEntry, List(Of ModNet.NetFile)))
                Dim list As List(Of String) = New List(Of String)()
                Dim text As String = DownloadInfo.Inherit
                If Operators.CompareString(text, "1.8", False) = 0 OrElse Operators.CompareString(text, "1.9", False) = 0 Then
                    text += ".0"
                End If
                If DownloadInfo.IsPreview Then
                    list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U_" + DownloadInfo.DisplayName.Replace(DownloadInfo.Inherit + " ", "").Replace(" ", "/"))
                Else
                    list.Add("https://bmclapi2.bangbang93.com/optifine/" + text + "/HD_U/" + DownloadInfo.DisplayName.Replace(DownloadInfo.Inherit + " ", ""))
                End If
                Try
                    Dim text2 As String = ModNet.NetRequestByClientRetry("https://optifine.net/adloadx?f=" + DownloadInfo.FileName, Nothing, Nothing, Nothing, Nothing, "text/html", Nothing, False, New UTF8Encoding(False), True)
                    Task.Progress = 0.8
                    list.Add("https://optifine.net/" + text2.RegexSearch("downloadx\?f=[^""']+", RegexOptions.None).First())
                    Logger.Info(String.Format("OptiFine {0} 官方下载地址：{1}", DownloadInfo.DisplayName, list.Last()), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                Catch ex As Exception
                    Logger.Warn(ex, String.Format("获取 OptiFine {0} 官方下载地址失败", DownloadInfo.DisplayName), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                End Try
                Task.Progress = 0.9
                Task.Output = New List(Of ModNet.NetFile)() From { New ModNet.NetFile(list.ToArray(), TargetFolder, New FileChecker() With { .MinSize = 65536L }, False) }
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 6.0 }, New ModNet.LoaderDownload("下载 OptiFine 主文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 10.0, .Block = True } }
        End Function

        Public Function OptiFineDownloadListItem(Entry As ModDownload.DlOptiFineListEntry, OnClick As OurListItem.ClickEventHandler, IsSaveOnly As Boolean) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem() With { .Title = Entry.DisplayName, .SnapsToDevicePixels = True, .Height = 42.0, .Type = OurListItem.CheckType.Clickable, .Tag = Entry, .Info = If(Entry.IsPreview, "测试版", "正式版") + If((Operators.CompareString(Entry.ReleaseTime, "", False) = 0), "", ("，发布于 " + Entry.ReleaseTime)) + If((Entry.RequiredForgeVersion Is Nothing), "，不兼容 Forge", If((Operators.CompareString(Entry.RequiredForgeVersion, "", False) = 0), "", ("，兼容 Forge " + Entry.RequiredForgeVersion))), .Logo = ModBase.PathImage + "Blocks/GrassPath.png" }
                AddHandler myListItem.Click, OnClick
                If IsSaveOnly Then
                    myListItem.ContentHandler = AddressOf ModDownloadLib.OptiFineSaveContMenuBuild
                Else
                    myListItem.ContentHandler = AddressOf ModDownloadLib.OptiFineContMenuBuild
                End If
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Private Sub OptiFineSaveContMenuBuild(sender As Object, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR16-8 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR16-8
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.OptiFineLog_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR16-8 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            NewLateBinding.LateSet(sender, Nothing, "Buttons", New Object() { New OurIconButton() { myIconButton } }, Nothing, Nothing)
        End Sub

        Private Sub OptiFineContMenuBuild(sender As Object, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z", .ToolTip = "另存为" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR17-9 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR17-9
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.OptiFineSave_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR17-9 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            Dim myIconButton3 As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton4 As OurIconButton = myIconButton3
            Dim clickEventHandler3 As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR17-10 IsNot Nothing Then
                clickEventHandler3 = ModDownloadLib._Closure$__.$IR17-10
            Else
                Dim clickEventHandler4 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.OptiFineLog_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler3 = clickEventHandler4
                ModDownloadLib._Closure$__.$IR17-10 = clickEventHandler4
            End If
            AddHandler myIconButton4.Click, clickEventHandler3
            NewLateBinding.LateSet(sender, Nothing, "Buttons", New Object() { New OurIconButton() { myIconButton, myIconButton3 } }, Nothing, Nothing)
        End Sub

        Private Sub OptiFineLog_Click(sender As Object, e As RoutedEventArgs)
            Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry
            If NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlOptiFineListEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlOptiFineListEntry)
            ElseIf NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlOptiFineListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlOptiFineListEntry)
            Else
                dlOptiFineListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlOptiFineListEntry)
            End If
            ModBase.OpenWebsite("https://optifine.net/changelog?f=" + dlOptiFineListEntry.FileName)
        End Sub

        Public Sub OptiFineSave_Click(sender As Object, e As RoutedEventArgs)
            Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry
            If NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlOptiFineListEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlOptiFineListEntry)
            ElseIf NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlOptiFineListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlOptiFineListEntry)
            Else
                dlOptiFineListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlOptiFineListEntry)
            End If
            ModDownloadLib.McDownloadOptiFineSave(dlOptiFineListEntry)
        End Sub

        Private Sub McDownloadLiteLoaderSave(DownloadInfo As ModDownload.DlLiteLoaderListEntry)
            Try
                Dim inherit As String = DownloadInfo.Inherit
                Dim text As String = Dialogs.SaveFile("选择保存位置", DownloadInfo.FileName.Replace("-SNAPSHOT", ""), Nothing, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)("jar", "LiteLoader 安装器") })
                If text IsNot Nothing Then
                    Try
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator = ModLoader.LoaderTaskbar.ToList().GetEnumerator()
                        While enumerator.MoveNext()
                            If Operators.CompareString(enumerator.Current.Name, String.Format("LiteLoader {0} 下载", inherit), False) = 0 Then
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, True)
                                Return
                            End If
                        End While
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                    Dim list2 As List(Of String) = New List(Of String)()
                    If DownloadInfo.IsLegacy Then
                        Dim inherit2 As String = DownloadInfo.Inherit
                        If Operators.CompareString(inherit2, "1.7.10", False) <> 0 Then
                            If Operators.CompareString(inherit2, "1.7.2", False) <> 0 Then
                                If Operators.CompareString(inherit2, "1.6.4", False) <> 0 Then
                                    If Operators.CompareString(inherit2, "1.6.2", False) <> 0 Then
                                        If Operators.CompareString(inherit2, "1.5.2", False) <> 0 Then
                                            Throw New NotSupportedException("未知的 Minecraft 版本（" + DownloadInfo.Inherit + "）")
                                        End If
                                        list2.Add("https://dl.liteloader.com/redist/1.5.2/liteloader-installer-1.5.2-01.jar")
                                    Else
                                        list2.Add("https://dl.liteloader.com/redist/1.6.2/liteloader-installer-1.6.2-04.jar")
                                    End If
                                Else
                                    list2.Add("https://dl.liteloader.com/redist/1.6.4/liteloader-installer-1.6.4-01.jar")
                                End If
                            Else
                                list2.Add("https://dl.liteloader.com/redist/1.7.2/liteloader-installer-1.7.2-04.jar")
                            End If
                        Else
                            list2.Add("https://dl.liteloader.com/redist/1.7.10/liteloader-installer-1.7.10-04.jar")
                        End If
                    Else
                        list2.Add(String.Concat(New String() { "http://jenkins.liteloader.com/job/LiteLoaderInstaller%20", DownloadInfo.Inherit, "/lastSuccessfulBuild/artifact/", If((Operators.CompareString(DownloadInfo.Inherit, "1.8", False) = 0), "ant/dist/", "build/libs/"), DownloadInfo.FileName }))
                    End If
                    list.Add(New ModNet.LoaderDownload("下载主文件", New List(Of ModNet.NetFile)() From { New ModNet.NetFile(list2.ToArray(), text, New FileChecker() With { .MinSize = 1048576L }, False) }) With { .ProgressWeight = 15.0 })
                    Dim loaderCombo As ModLoader.LoaderCombo(Of ModDownload.DlLiteLoaderListEntry) = New ModLoader.LoaderCombo(Of ModDownload.DlLiteLoaderListEntry)("LiteLoader " + inherit + " 安装器下载", list)
                    loaderCombo.OnStateChanged = AddressOf ModDownloadLib.LoaderStateChangedHintOnly
                    loaderCombo.Start(DownloadInfo, False)
                    ModLoader.LoaderTaskbarAdd(Of ModDownload.DlLiteLoaderListEntry)(loaderCombo)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                End If
            Catch ex As Exception
                Logger.[Error](ex, "开始 LiteLoader 安装器下载失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Try
        End Sub

        Private Function McDownloadLiteLoaderLoader(DownloadInfo As ModDownload.DlLiteLoaderListEntry, McFolder As String) As List(Of ModLoader.LoaderBase)
            Return New List(Of ModLoader.LoaderBase)() From { New ModLoader.LoaderTask(Of String, String)("安装 LiteLoader", Sub(Task As ModLoader.LoaderTask(Of String, String))
                Try
                    Dim text As String = DownloadInfo.Inherit + "-LiteLoader"
                    Dim text2 As String = McFolder + "versions\" + text + "\"
                    Dim jobject As JObject = New JObject()
                    jobject.Add("id", text)
                    jobject.Add("time", DateTime.ParseExact(DownloadInfo.ReleaseTime, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture))
                    jobject.Add("releaseTime", DateTime.ParseExact(DownloadInfo.ReleaseTime, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture))
                    jobject.Add("type", "release")
                    jobject.Add("arguments", ("{""game"":[""--tweakClass"",""" + DownloadInfo.JsonToken("tweakClass").ToString() + """]}").DeserializeJson())
                    jobject.Add("libraries", DownloadInfo.JsonToken("libraries"))
                    CType(jobject("libraries"), JContainer).Add(("{""name"": ""com.mumfrey:liteloader:" + DownloadInfo.JsonToken("version").ToString() + """,""url"": ""https://dl.liteloader.com/versions/""}").DeserializeJson())
                    jobject.Add("mainClass", "net.minecraft.launchwrapper.Launch")
                    jobject.Add("minimumLauncherVersion", 18)
                    jobject.Add("inheritsFrom", DownloadInfo.Inherit)
                    jobject.Add("jar", DownloadInfo.Inherit)
                    FileUtils.Write(text2 + text + ".json", jobject.ToString(), Nothing)
                Catch ex As Exception
                    Throw New Exception("安装新 LiteLoader 版本失败", ex)
                End Try
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 } }
        End Function

        Public Function LiteLoaderDownloadListItem(Entry As ModDownload.DlLiteLoaderListEntry, OnClick As OurListItem.ClickEventHandler, IsSaveOnly As Boolean) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem() With { .Title = Entry.Inherit, .SnapsToDevicePixels = True, .Height = 42.0, .Type = OurListItem.CheckType.Clickable, .Tag = Entry, .Info = If(Entry.IsPreview, "测试版", "稳定版") + If((Operators.CompareString(Entry.ReleaseTime, "", False) = 0), "", ("，发布于 " + Entry.ReleaseTime)), .Logo = ModBase.PathImage + "Blocks/Egg.png" }
                AddHandler myListItem.Click, OnClick
                If IsSaveOnly Then
                    myListItem.ContentHandler = AddressOf ModDownloadLib.LiteLoaderSaveContMenuBuild
                Else
                    myListItem.ContentHandler = AddressOf ModDownloadLib.LiteLoaderContMenuBuild
                End If
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Private Sub LiteLoaderSaveContMenuBuild(sender As OurListItem, e As EventArgs)
            If Conversions.ToBoolean(NewLateBinding.LateGet(sender.Tag, Nothing, "IsLegacy", New Object(-1) {}, Nothing, Nothing, Nothing)) Then
                sender.Buttons = New OurIconButton(-1) {}
                Return
            End If
            Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z", .ToolTip = "查看全部版本", .Tag = sender }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR23-11 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR23-11
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.LiteLoaderAll_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR23-11 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            sender.Buttons = New OurIconButton() { myIconButton }
        End Sub

        Private Sub LiteLoaderContMenuBuild(sender As OurListItem, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z", .ToolTip = "保存安装器", .Tag = sender }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR24-12 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR24-12
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.LiteLoaderSave_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR24-12 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            If Conversions.ToBoolean(NewLateBinding.LateGet(sender.Tag, Nothing, "IsLegacy", New Object(-1) {}, Nothing, Nothing, Nothing)) Then
                sender.Buttons = New OurIconButton() { myIconButton }
                Return
            End If
            Dim myIconButton3 As OurIconButton = New OurIconButton() With { .Logo = "M384 128h640v128H384zM160 192m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 448h640v128H384zM160 512m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0ZM384 768h640v128H384zM160 832m-96 0a96 96 0 1 0 192 0 96 96 0 1 0-192 0Z", .ToolTip = "查看全部版本", .Tag = sender }
            Dim myIconButton4 As OurIconButton = myIconButton3
            Dim clickEventHandler3 As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR24-13 IsNot Nothing Then
                clickEventHandler3 = ModDownloadLib._Closure$__.$IR24-13
            Else
                Dim clickEventHandler4 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.LiteLoaderAll_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler3 = clickEventHandler4
                ModDownloadLib._Closure$__.$IR24-13 = clickEventHandler4
            End If
            AddHandler myIconButton4.Click, clickEventHandler3
            sender.Buttons = New OurIconButton() { myIconButton, myIconButton3 }
        End Sub

        Private Sub LiteLoaderAll_Click(sender As Object, e As RoutedEventArgs)
            Dim dlLiteLoaderListEntry As ModDownload.DlLiteLoaderListEntry
            If TypeOf NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)Is ModDownload.DlLiteLoaderListEntry Then
                dlLiteLoaderListEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlLiteLoaderListEntry)
            Else
                dlLiteLoaderListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlLiteLoaderListEntry)
            End If
            ModBase.OpenWebsite("https://jenkins.liteloader.com/view/" + dlLiteLoaderListEntry.Inherit)
        End Sub

        Public Sub LiteLoaderSave_Click(sender As Object, e As RoutedEventArgs)
            Dim dlLiteLoaderListEntry As ModDownload.DlLiteLoaderListEntry
            If TypeOf NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)Is ModDownload.DlLiteLoaderListEntry Then
                dlLiteLoaderListEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlLiteLoaderListEntry)
            Else
                dlLiteLoaderListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlLiteLoaderListEntry)
            End If
            ModDownloadLib.McDownloadLiteLoaderSave(dlLiteLoaderListEntry)
        End Sub

        Public Sub McDownloadForgelikeSave(Info As ModDownload.DlForgelikeEntry)
            Try
                Dim text As String = Dialogs.SaveFile("选择保存位置", String.Format("{0}-{1}-{2}.{3}", New Object() { Info.LoaderName, Info.Inherit, Info.VersionName, Info.FileExtension }), Nothing, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)(Info.FileExtension, String.Format("{0} 安装器", Info.LoaderName)) })
                If text IsNot Nothing Then
                    Dim text2 As String = String.Format("{0} {1} - {2}", Info.LoaderName, Info.Inherit, Info.VersionName)
                    Try
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator = ModLoader.LoaderTaskbar.ToList().GetEnumerator()
                        While enumerator.MoveNext()
                            If Operators.CompareString(enumerator.Current.Name, String.Format("{0} 下载", text2), False) = 0 Then
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, True)
                                Return
                            End If
                        End While
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Dim list As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                    If Info.IsNeoForge Then
                        Dim text3 As String = CType(Info, ModDownload.DlNeoForgeListEntry).UrlBase + "-installer.jar"
                        list.Add(New ModNet.NetFile(New String() { text3.Replace("maven.neoforged.net/releases", "bmclapi2.bangbang93.com/maven"), text3 }, text, New FileChecker() With { .MinSize = 65536L }, False))
                    Else
                        Dim dlForgeVersionEntry As ModDownload.DlForgeVersionEntry = CType(Info, ModDownload.DlForgeVersionEntry)
                        list.Add(New ModNet.NetFile(New String() { String.Format("https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{0}-{1}/forge-{2}-{3}-{4}.{5}", New Object() { dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Category, dlForgeVersionEntry.FileExtension }), String.Format("https://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}-{1}/forge-{2}-{3}-{4}.{5}", New Object() { dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Inherit, dlForgeVersionEntry.FileVersion, dlForgeVersionEntry.Category, dlForgeVersionEntry.FileExtension }) }, text, New FileChecker() With { .MinSize = 65536L, .Hash = dlForgeVersionEntry.Hash }, False))
                    End If
                    Dim list2 As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                    list2.Add(New ModNet.LoaderDownload("下载主文件", list) With { .ProgressWeight = 6.0 })
                    Dim loaderCombo As ModLoader.LoaderCombo(Of ModDownload.DlForgelikeEntry) = New ModLoader.LoaderCombo(Of ModDownload.DlForgelikeEntry)(text2 + " 下载", list2)
                    loaderCombo.OnStateChanged = AddressOf ModDownloadLib.LoaderStateChangedHintOnly
                    loaderCombo.Start(Info, False)
                    ModLoader.LoaderTaskbarAdd(Of ModDownload.DlForgelikeEntry)(loaderCombo)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("开始 {0} 安装器下载失败", Info.LoaderName), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Try
        End Sub

        Private Sub ForgelikeInjector(Target As String, Task As ModLoader.LoaderTask(Of Boolean, Boolean), McFolder As String, UseJavaWrapper As Boolean, IsNeoForge As Boolean)
            Dim CS$<>8__locals1 As ModDownloadLib._Closure$__28-2 = New ModDownloadLib._Closure$__28-2(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Task = Task
            Dim java As Java = ModJava.SelectOrDownloadJava(ValueRange(Of Version).AtLeast(New Version(8, 0, 60)), True, CS$<>8__locals1.$VB$Local_Task.CreateCancellationToken(), CS$<>8__locals1.$VB$Local_Task.CreateSyncProgressProvider(0.0, 0.2))
            If Not CS$<>8__locals1.$VB$Local_Task.IsCanceled Then
                If java Is Nothing Then
                    Throw New OperationCanceledException()
                End If
                Dim text As String
                If UseJavaWrapper AndAlso Not Settings.[Get](Of Boolean)("LaunchAdvanceDisableJLW", Nothing) Then
                    text = String.Format("-Doolloo.jlw.tmpdir=""{0}"" -cp ""{1}Cache\forge_installer.jar;{2}"" -jar ""{3}"" com.bangbang93.ForgeInstaller ""{4}", New Object() { PathUtils.RemoveSlashSuffix(ModBase.PathPure.Value), ModBase.PathTemp, Target, LaunchUtils.ExtractPatch("Launch\JavaWrapper.jar", ModBase.PathPure.Value), McFolder })
                Else
                    text = String.Format("-cp ""{0}Cache\forge_installer.jar;{1}"" com.bangbang93.ForgeInstaller ""{2}", ModBase.PathTemp, Target, McFolder)
                End If
                If java.Version.Major >= 9 Then
                    text = "--add-exports cpw.mods.bootstraplauncher/cpw.mods.bootstraplauncher=ALL-UNNAMED " + text
                End If
                Dim installSyncLock As Object = ModDownloadLib.InstallSyncLock
                ObjectFlowControl.CheckForSyncLockOnValueType(installSyncLock)
                SyncLock installSyncLock
                    Dim CS$<>8__locals2 As ModDownloadLib._Closure$__28-1 = New ModDownloadLib._Closure$__28-1(CS$<>8__locals2)
                    CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                    Dim processStartInfo As ProcessStartInfo = New ProcessStartInfo() With { .FileName = java.JavaExePath, .Arguments = text, .UseShellExecute = False, .CreateNoWindow = True, .RedirectStandardError = True, .RedirectStandardOutput = True }
                    CS$<>8__locals2.$VB$Local_LoaderName = If(IsNeoForge, "NeoForge", "Forge")
                    Logger.Info(String.Format("开始安装 {0}：{1}", CS$<>8__locals2.$VB$Local_LoaderName, text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                    CS$<>8__locals2.$VB$Local_process = New Process() With { .StartInfo = processStartInfo }
                    CS$<>8__locals2.$VB$Local_LastResults = New Queue(Of String)()
                    Dim CS$<>8__locals3 As ModDownloadLib._Closure$__28-0 = New ModDownloadLib._Closure$__28-0(CS$<>8__locals3)
                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals2
                    CS$<>8__locals3.$VB$Local_outputWaitHandle = New AutoResetEvent(False)
                    Try
                        Dim CS$<>8__locals4 As ModDownloadLib._Closure$__28-3 = New ModDownloadLib._Closure$__28-3(CS$<>8__locals4)
                        CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4 = CS$<>8__locals3
                        CS$<>8__locals4.$VB$Local_errorWaitHandle = New AutoResetEvent(False)
                        Try
                            AddHandler CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.OutputDataReceived, Sub(a0 As Object, a1 As DataReceivedEventArgs)
                                MyBase._Lambda$__0(RuntimeHelpers.GetObjectValue(a0), a1)
                            End Sub
                            AddHandler CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.ErrorDataReceived, Sub(a0 As Object, a1 As DataReceivedEventArgs)
                                MyBase._Lambda$__1(RuntimeHelpers.GetObjectValue(a0), a1)
                            End Sub
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Start()
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginOutputReadLine()
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.BeginErrorReadLine()
                            While Not CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.HasExited
                                Thread.Sleep(10)
                            End While
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$Local_outputWaitHandle.WaitOne(10000)
                            CS$<>8__locals4.$VB$Local_errorWaitHandle.WaitOne(10000)
                            CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_process.Dispose()
                            Dim enumerable As IEnumerable(Of String) = CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Reverse().Take(5)
                            Dim func As Func(Of String, Boolean)
                            If ModDownloadLib._Closure$__.$I28-2 IsNot Nothing Then
                                func = ModDownloadLib._Closure$__.$I28-2
                            Else
                                Dim func2 As Func(Of String, Boolean) = Function(l As String) Operators.CompareString(l, "true", False) = 0
                                func = func2
                                ModDownloadLib._Closure$__.$I28-2 = func2
                            End If
                            If Not enumerable.Any(func) Then
                                Logger.Warn(CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Join(vbCrLf), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                                Dim text2 As String = ""
                                Dim num As Integer = Math.Max(0, CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Count - 5)
                                Dim num2 As Integer = CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.Count - 1
                                For i As Integer = num To num2
                                    text2 = text2 + vbCrLf + CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LastResults.ElementAtOrDefault(i)
                                Next
                                Throw New Exception(String.Format("{0} 安装器出错，日志结束部分为：", CS$<>8__locals4.$VB$NonLocal_$VB$Closure_4.$VB$NonLocal_$VB$Closure_3.$VB$Local_LoaderName) + text2)
                            End If
                        Finally
                            If CS$<>8__locals4.$VB$Local_errorWaitHandle IsNot Nothing Then
                                CType(CS$<>8__locals4.$VB$Local_errorWaitHandle, IDisposable).Dispose()
                            End If
                        End Try
                    Finally
                        If CS$<>8__locals3.$VB$Local_outputWaitHandle IsNot Nothing Then
                            CType(CS$<>8__locals3.$VB$Local_outputWaitHandle, IDisposable).Dispose()
                        End If
                    End Try
                End SyncLock
            End If
        End Sub

        Private Sub ForgelikeInjectorLine(Content As String, Task As ModLoader.LoaderTask(Of Boolean, Boolean))
            Dim num As UInteger = <PrivateImplementationDetails>.ComputeStringHash(Content)
            If num <= 2947477999UI Then
                If num <= 867564423UI Then
                    If num <> 214422873UI Then
                        If num <> 538128555UI Then
                            If num = 867564423UI Then
                                If Operators.CompareString(Content, "Remapping jar... 50%", False) = 0 Then
                                    Task.Progress = 0.9
                                    GoTo IL_036F
                                End If
                            End If
                        ElseIf Operators.CompareString(Content, "Remapping final jar", False) = 0 Then
                            Task.Progress = 0.85
                            GoTo IL_036F
                        End If
                    ElseIf Operators.CompareString(Content, "Splitting: ", False) = 0 Then
                        Task.Progress = 0.55
                        GoTo IL_036F
                    End If
                ElseIf num <= 2394603510UI Then
                    If num <> 1592277622UI Then
                        If num = 2394603510UI Then
                            If Operators.CompareString(Content, "Building Processors", False) = 0 Then
                                Task.Progress = 0.38
                                GoTo IL_036F
                            End If
                        End If
                    ElseIf Operators.CompareString(Content, "log: null", False) = 0 Then
                        Task.Progress = 0.67
                        GoTo IL_036F
                    End If
                ElseIf num <> 2846730644UI Then
                    If num = 2947477999UI Then
                        If Operators.CompareString(Content, "Sorting", False) = 0 Then
                            Task.Progress = 0.8
                            GoTo IL_036F
                        End If
                    End If
                ElseIf Operators.CompareString(Content, "Task: DOWNLOAD_MOJMAPS", False) = 0 Then
                    Task.Progress = 0.4
                    GoTo IL_036F
                End If
            ElseIf num <= 3369270864UI Then
                If num <= 3198574777UI Then
                    If num <> 2971959160UI Then
                        If num = 3198574777UI Then
                            If Operators.CompareString(Content, "Injecting profile", False) = 0 Then
                                Task.Progress = 0.98
                                GoTo IL_036F
                            End If
                        End If
                    ElseIf Operators.CompareString(Content, "  File exists: Checksum validated.", False) = 0 Then
                        Task.Progress += 0.003
                        GoTo IL_036F
                    End If
                ElseIf num <> 3296415987UI Then
                    If num = 3369270864UI Then
                        If Operators.CompareString(Content, "Extracting json", False) = 0 Then
                            Task.Progress = 0.27
                            GoTo IL_036F
                        End If
                    End If
                ElseIf Operators.CompareString(Content, "Remapping jar... 100%", False) = 0 Then
                    Task.Progress = 0.95
                    GoTo IL_036F
                End If
            ElseIf num <= 3905690050UI Then
                If num <> 3587956665UI Then
                    If num = 3905690050UI Then
                        If Operators.CompareString(Content, "Parameter Annotations", False) = 0 Then
                            Task.Progress = 0.6
                            GoTo IL_036F
                        End If
                    End If
                ElseIf Operators.CompareString(Content, "Processing Complete", False) = 0 Then
                    Task.Progress = 0.67
                    GoTo IL_036F
                End If
            ElseIf num <> 3935946959UI Then
                If num = 4046473188UI Then
                    If Operators.CompareString(Content, "Downloading libraries", False) = 0 Then
                        Task.Progress = 0.28
                        GoTo IL_036F
                    End If
                End If
            ElseIf Operators.CompareString(Content, "Task: MERGE_MAPPING", False) = 0 Then
                Task.Progress = 0.5
                GoTo IL_036F
            End If
            Logger.Trace(Content, LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            Return
            IL_036F:
            Logger.Info(Content, LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
        End Sub

        Private Function McDownloadForgelikeLoader(IsNeoForge As Boolean, LoaderVersion As String, NewInstanceName As String, Inherit As String, Info As ModDownload.DlForgelikeEntry, McFolder As String, ClientDownloadLoader As ModLoader.LoaderCombo(Of String), ClientFolder As String) As List(Of ModLoader.LoaderBase)
            Dim CS$<>8__locals1 As ModDownloadLib._Closure$__30-0 = New ModDownloadLib._Closure$__30-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_IsNeoForge = IsNeoForge
            CS$<>8__locals1.$VB$Local_LoaderVersion = LoaderVersion
            CS$<>8__locals1.$VB$Local_NewInstanceName = NewInstanceName
            CS$<>8__locals1.$VB$Local_Inherit = Inherit
            CS$<>8__locals1.$VB$Local_Info = Info
            CS$<>8__locals1.$VB$Local_McFolder = McFolder
            CS$<>8__locals1.$VB$Local_ClientDownloadLoader = ClientDownloadLoader
            CS$<>8__locals1.$VB$Local_ClientFolder = ClientFolder
            If CS$<>8__locals1.$VB$Local_IsNeoForge AndAlso CS$<>8__locals1.$VB$Local_Info Is Nothing Then
                If Operators.CompareString(CS$<>8__locals1.$VB$Local_Inherit, "1.20.1", False) = 0 AndAlso Not CS$<>8__locals1.$VB$Local_LoaderVersion.StartsWithF("1.20.1-", False) Then
                    CS$<>8__locals1.$VB$Local_Info = New ModDownload.DlNeoForgeListEntry("1.20.1-" + CS$<>8__locals1.$VB$Local_LoaderVersion)
                Else
                    CS$<>8__locals1.$VB$Local_Info = New ModDownload.DlNeoForgeListEntry(CS$<>8__locals1.$VB$Local_LoaderVersion)
                End If
            End If
            If Not CS$<>8__locals1.$VB$Local_IsNeoForge AndAlso CS$<>8__locals1.$VB$Local_LoaderVersion.StartsWithF("1.", False) AndAlso CS$<>8__locals1.$VB$Local_LoaderVersion.Contains("-") Then
                CS$<>8__locals1.$VB$Local_Inherit = CS$<>8__locals1.$VB$Local_LoaderVersion.BeforeFirst("-", False)
                CS$<>8__locals1.$VB$Local_LoaderVersion = CS$<>8__locals1.$VB$Local_LoaderVersion.AfterLast("-", False)
            End If
            CS$<>8__locals1.$VB$Local_LoaderName = If(CS$<>8__locals1.$VB$Local_IsNeoForge, "NeoForge", "Forge")
            CS$<>8__locals1.$VB$Local_InstallerAddress = ModMain.RequestTaskTempFolder(True) + "forge_installer.jar"
            CS$<>8__locals1.$VB$Local_VersionFolder = String.Format("{0}versions\{1}\", CS$<>8__locals1.$VB$Local_McFolder, CS$<>8__locals1.$VB$Local_NewInstanceName)
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            If CS$<>8__locals1.$VB$Local_Info Is Nothing Then
                list.Add(New ModLoader.LoaderTask(Of String, String)(String.Format("获取 {0} 详细信息", CS$<>8__locals1.$VB$Local_LoaderName), Sub(Task As ModLoader.LoaderTask(Of String, String))
                    Dim loaderTask As ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry)) = New ModLoader.LoaderTask(Of String, List(Of ModDownload.DlForgeVersionEntry))("McDownloadForgeLoader " + CS$<>8__locals1.$VB$Local_Inherit, AddressOf ModDownload.DlForgeVersionMain, Nothing, ThreadPriority.Normal)
                    loaderTask.WaitForExit(CS$<>8__locals1.$VB$Local_Inherit, Nothing, False, Nothing, Nothing)
                    Task.Progress = 0.8
                    Try
                        For Each dlForgeVersionEntry As ModDownload.DlForgeVersionEntry In loaderTask.Output
                            If McUtils.CompareVersion(dlForgeVersionEntry.Version.ToString(), CS$<>8__locals1.$VB$Local_LoaderVersion) = 0 Then
                                CS$<>8__locals1.$VB$Local_Info = dlForgeVersionEntry
                                Return
                            End If
                        Next
                    Finally
                        Dim enumerator As List(Of ModDownload.DlForgeVersionEntry).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Throw New Exception(String.Concat(New String() { String.Format("未能找到 {0} ", CS$<>8__locals1.$VB$Local_LoaderName), CS$<>8__locals1.$VB$Local_Inherit, "-", CS$<>8__locals1.$VB$Local_LoaderVersion, " 的详细信息！" }))
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 3.0 })
            End If
            list.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))(String.Format("准备下载 {0}", CS$<>8__locals1.$VB$Local_LoaderName), Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Dim list2 As List(Of ModNet.NetFile) = New List(Of ModNet.NetFile)()
                If CS$<>8__locals1.$VB$Local_Info.IsNeoForge Then
                    Dim text As String = CType(CS$<>8__locals1.$VB$Local_Info, ModDownload.DlNeoForgeListEntry).UrlBase + "-installer.jar"
                    list2.Add(New ModNet.NetFile(New String() { text.Replace("maven.neoforged.net/releases", "bmclapi2.bangbang93.com/maven"), text }, CS$<>8__locals1.$VB$Local_InstallerAddress, New FileChecker() With { .MinSize = 65536L }, False))
                Else
                    Dim dlForgeVersionEntry2 As ModDownload.DlForgeVersionEntry = CType(CS$<>8__locals1.$VB$Local_Info, ModDownload.DlForgeVersionEntry)
                    Dim text2 As String = String.Format("{0}-{1}/forge-{2}-{3}-{4}.{5}", New Object() { dlForgeVersionEntry2.Inherit.Replace("-", "_"), dlForgeVersionEntry2.FileVersion, dlForgeVersionEntry2.Inherit.Replace("-", "_"), dlForgeVersionEntry2.FileVersion, dlForgeVersionEntry2.Category, dlForgeVersionEntry2.FileExtension })
                    list2.Add(New ModNet.NetFile(New String() { String.Format("https://bmclapi2.bangbang93.com/maven/net/minecraftforge/forge/{0}", text2), String.Format("https://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}", text2) }, CS$<>8__locals1.$VB$Local_InstallerAddress, New FileChecker() With { .MinSize = 65536L, .Hash = dlForgeVersionEntry2.Hash }, False))
                End If
                Task.Output = list2
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.5, .Show = False })
            list.Add(New ModNet.LoaderDownload(String.Format("下载 {0} 主文件", CS$<>8__locals1.$VB$Local_LoaderName), New List(Of ModNet.NetFile)()) With { .ProgressWeight = 9.0 })
            CS$<>8__locals1.$VB$Local_CurrentMcFolder = ModMinecraft.McFolderSelected
            If CS$<>8__locals1.$VB$Local_IsNeoForge OrElse Conversions.ToDouble(CS$<>8__locals1.$VB$Local_LoaderVersion.BeforeFirst(".", False)) >= 20.0 Then
                Dim CS$<>8__locals2 As ModDownloadLib._Closure$__30-1 = New ModDownloadLib._Closure$__30-1(CS$<>8__locals2)
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                Logger.Info(String.Format("检测为{0}Forge：{1}", If(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge, " Neo", "新版 "), CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                CS$<>8__locals2.$VB$Local_Libs = Nothing
                list.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))(String.Format("分析 {0} 支持库文件", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                    Task.Output = New List(Of ModNet.NetFile)()
                    Dim zipArchive As ZipArchive = Nothing
                    Try
                        zipArchive = FileUtils.OpenZip(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress, Nothing)
                        Task.Progress = 0.2
                        Dim jobject As JObject = CType(zipArchive.GetEntry("install_profile.json").Open().ReadString(Nothing).DeserializeJson(), JObject)
                        Dim jobject2 As JObject = CType(zipArchive.GetEntry("version.json").Open().ReadString(Nothing).DeserializeJson(), JObject)
                        jobject.Merge(jobject2)
                        CS$<>8__locals2.$VB$Local_Libs = ModMinecraft.McLibListGetWithJson(jobject, True, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder)
                        If jobject("data") IsNot Nothing AndAlso jobject("data")("MOJMAPS") IsNot Nothing Then
                            Task.Progress = 0.4
                            Dim jobject3 As JObject = CType(ModNet.NetRequestByLoader(ModDownload.DlSourceLauncherOrMetaGet(Conversions.ToString(ModDownload.DlClientListGet(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit))), 45000, True, False).DeserializeJson(), JObject)
                            Dim text3 As String = jobject("data")("MOJMAPS")("client").ToString().Trim("[]".ToCharArray()).BeforeFirst("@", False)
                            Dim text4 As String = Path.ChangeExtension(ModMinecraft.McLibGet(text3, True, False, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder), "." + jobject("data")("MOJMAPS")("client").ToString().Trim("[]".ToCharArray()).Split("@", False)(1))
                            Dim jtoken As JToken = jobject3("downloads")("client_mappings")
                            CS$<>8__locals2.$VB$Local_Libs.Add(New ModMinecraft.McLibEntry() With { .IsNatives = False, .LocalPath = text4, .OriginalName = text3, .Url = CStr(jtoken("url")), .Size = CLng(jtoken("size")), .SHA1 = CStr(jtoken("sha1")) })
                            Logger.Info(String.Format("需要下载 Mappings：{0} (SHA1: {1})", jtoken("url"), jtoken("sha1")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        End If
                        Task.Progress = 0.8
                        Dim num As Integer = CS$<>8__locals2.$VB$Local_Libs.Count - 1
                        For i As Integer = 0 To num
                            If CS$<>8__locals2.$VB$Local_Libs(i).LocalPath.EndsWithF(String.Format("{0}-{1}-{2}.jar", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName.Lower(), CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderVersion), False) OrElse CS$<>8__locals2.$VB$Local_Libs(i).LocalPath.EndsWithF(String.Format("{0}-{1}-{2}-client.jar", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName.Lower(), CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderVersion), False) Then
                                Logger.Warn(String.Format("已从待下载 {0} 支持库中移除：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName, CS$<>8__locals2.$VB$Local_Libs(i).LocalPath), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                                CS$<>8__locals2.$VB$Local_Libs.RemoveAt(i)
                                Exit For
                            End If
                        Next
                        Task.Output = ModMinecraft.McLibNetFilesFromTokens(CS$<>8__locals2.$VB$Local_Libs, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder)
                    Catch ex As Exception
                        Throw New Exception(String.Format("获取{0}Forge 支持库列表失败", If(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge, " Neo", "新版 ")), ex)
                    Finally
                        If zipArchive IsNot Nothing Then
                            zipArchive.Dispose()
                        End If
                    End Try
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 })
                list.Add(New ModNet.LoaderDownload(String.Format("下载 {0} 支持库文件", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), New List(Of ModNet.NetFile)()) With { .ProgressWeight = 12.0 })
                list.Add(New ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean)(String.Format("获取 {0} 支持库文件", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), Sub(Task As ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean))
                    Try
                        For Each mcLibEntry As ModMinecraft.McLibEntry In CS$<>8__locals2.$VB$Local_Libs
                            Dim text5 As String = Path.Combine(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, mcLibEntry.LocalPath.Substring(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_CurrentMcFolder.Length))
                            If Not FileUtils.Exists(text5) Then
                                FileUtils.Copy(mcLibEntry.LocalPath, text5)
                            End If
                            Logger.Trace(String.Format("复制的 {0} 支持库文件：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName, mcLibEntry.LocalPath), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        Next
                    Finally
                        Dim enumerator2 As List(Of ModMinecraft.McLibEntry).Enumerator
                        CType(enumerator2, IDisposable).Dispose()
                    End Try
                    Dim loaderList As IEnumerable(Of ModLoader.LoaderBase) = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientDownloadLoader.GetLoaderList(True)
                    Dim func As Func(Of ModLoader.LoaderBase, Boolean)
                    If ModDownloadLib._Closure$__.$I30-4 IsNot Nothing Then
                        func = ModDownloadLib._Closure$__.$I30-4
                    Else
                        Dim func2 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) Operators.CompareString(l.Name, "下载原版支持库文件", False) = 0 OrElse Operators.CompareString(l.Name, "下载原版 json 文件", False) = 0
                        func = func2
                        ModDownloadLib._Closure$__.$I30-4 = func2
                    End If
                    Dim enumerable As IEnumerable(Of ModLoader.LoaderBase) = loaderList.Where(func)
                    Dim func3 As Func(Of ModLoader.LoaderBase, Boolean)
                    If ModDownloadLib._Closure$__.$I30-5 IsNot Nothing Then
                        func3 = ModDownloadLib._Closure$__.$I30-5
                    Else
                        Dim func4 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) l.State <> ModBase.LoadState.Finished
                        func3 = func4
                        ModDownloadLib._Closure$__.$I30-5 = func4
                    End If
                    Dim list3 As List(Of ModLoader.LoaderBase) = enumerable.Where(func3).ToList()
                    If list3.Any() Then
                        Logger.Info(String.Format("{0} 安装正在等待原版文件下载完成", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                    End If
                    While list3.Any() AndAlso Not Task.IsCanceled
                        Dim enumerable2 As IEnumerable(Of ModLoader.LoaderBase) = list3
                        Dim func5 As Func(Of ModLoader.LoaderBase, Boolean)
                        If ModDownloadLib._Closure$__.$I30-6 IsNot Nothing Then
                            func5 = ModDownloadLib._Closure$__.$I30-6
                        Else
                            Dim func6 As Func(Of ModLoader.LoaderBase, Boolean) = Function(l As ModLoader.LoaderBase) l.State <> ModBase.LoadState.Finished
                            func5 = func6
                            ModDownloadLib._Closure$__.$I30-6 = func6
                        End If
                        list3 = enumerable2.Where(func5).ToList()
                        Thread.Sleep(50)
                    End While
                    If Not Task.IsCanceled Then
                        Dim vanillaSyncLock As Object = ModDownloadLib.VanillaSyncLock
                        ObjectFlowControl.CheckForSyncLockOnValueType(vanillaSyncLock)
                        SyncLock vanillaSyncLock
                            Dim lastPart As String = PathUtils.GetLastPart(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientFolder)
                            DirectoryUtils.Create(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder + "versions\" + CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit)
                            If Not FileUtils.Exists(String.Concat(New String() { CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, "versions\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, "\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, ".json" })) Then
                                FileUtils.Copy(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientFolder + lastPart + ".json", String.Concat(New String() { CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, "versions\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, "\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, ".json" }))
                            End If
                            If Not FileUtils.Exists(String.Concat(New String() { CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, "versions\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, "\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, ".jar" })) Then
                                FileUtils.Copy(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ClientFolder + lastPart + ".jar", String.Concat(New String() { CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, "versions\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, "\", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Inherit, ".jar" }))
                            End If
                        End SyncLock
                    End If
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.1, .Show = False })
                list.Add(New ModLoader.LoaderTask(Of Boolean, Boolean)(If(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge, "安装 NeoForge", "安装 Forge（方式 A）"), Sub(Task As ModLoader.LoaderTask(Of Boolean, Boolean))
                    Try
                        Dim CS$<>8__locals3 As ModDownloadLib._Closure$__30-2 = New ModDownloadLib._Closure$__30-2(CS$<>8__locals3)
                        Logger.Info(String.Format("开始进行 Forgelike 安装：{0}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        CS$<>8__locals3.$VB$Local_OldList = DirectoryUtils.EnumerateDirectories(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder + "versions\", False, "*").ToList()
                        DirectoryUtils.Create(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder)
                        Task.Progress = 0.04
                        ModMinecraft.McFolderLauncherProfilesJsonCreate(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder)
                        Task.Progress = 0.05
                        Dim flag2 As Boolean = True
                        While True
                            Try
                                ModBase.ExtractResources(ModBase.PathTemp + "Cache\forge_installer.jar", "ForgeInstaller")
                                Task.Progress = 0.06
                                ModDownloadLib.ForgelikeInjector(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress, Task, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder, flag2, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_IsNeoForge)
                                Task.Progress = 0.97
                            Catch ex2 As Exception
                                If flag2 Then
                                    Logger.Warn(ex2, String.Format("使用 JavaWrapper 安装 {0} 失败，将不使用 JavaWrapper 并重试", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                                    flag2 = False
                                    Continue For
                                End If
                                Throw New Exception(String.Format("运行 {0} 安装器失败", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), ex2)
                            End Try
                            Exit For
                        End While
                        Dim enumerable3 As IEnumerable(Of String) = DirectoryUtils.EnumerateDirectories(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_McFolder + "versions\", False, "*").SkipWhile(Function(i As String) CS$<>8__locals3.$VB$Local_OldList.Contains(i))
                        Dim func7 As Func(Of String, DirectoryInfo)
                        If ModDownloadLib._Closure$__.$I30-9 IsNot Nothing Then
                            func7 = ModDownloadLib._Closure$__.$I30-9
                        Else
                            Dim func8 As Func(Of String, DirectoryInfo) = Function(f As String) DirectoryUtils.GetInfo(f)
                            func7 = func8
                            ModDownloadLib._Closure$__.$I30-9 = func8
                        End If
                        Dim list4 As List(Of DirectoryInfo) = enumerable3.[Select](func7).ToList()
                        If list4.Count > 1 Then
                            Dim enumerable4 As IEnumerable(Of DirectoryInfo) = list4
                            Dim func9 As Func(Of DirectoryInfo, Boolean)
                            If ModDownloadLib._Closure$__.$I30-10 IsNot Nothing Then
                                func9 = ModDownloadLib._Closure$__.$I30-10
                            Else
                                Dim func10 As Func(Of DirectoryInfo, Boolean) = Function(l As DirectoryInfo) l.Name.ContainsIgnoreCase("forge") AndAlso l.EnumerateFiles().Any()
                                func9 = func10
                                ModDownloadLib._Closure$__.$I30-10 = func10
                            End If
                            list4 = enumerable4.Where(func9).ToList()
                        End If
                        If list4.Count = 1 Then
                            Dim fileInfo As FileInfo = list4(0).EnumerateFiles().First()
                            FileUtils.Copy(fileInfo.FullName, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder + CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_NewInstanceName + ".json")
                            Logger.Info(String.Format("已复制新增的版本 JSON 文件：{0} -> {1}{2}.json", fileInfo.FullName, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_VersionFolder, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_NewInstanceName), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        ElseIf list4.Count > 1 Then
                            Dim text6 As String = "有多个疑似的新增版本，无法确定：{0}"
                            Dim enumerable5 As IEnumerable(Of DirectoryInfo) = list4
                            Dim func11 As Func(Of DirectoryInfo, String)
                            If ModDownloadLib._Closure$__.$I30-11 IsNot Nothing Then
                                func11 = ModDownloadLib._Closure$__.$I30-11
                            Else
                                Dim func12 As Func(Of DirectoryInfo, String) = Function(d As DirectoryInfo) d.Name
                                func11 = func12
                                ModDownloadLib._Closure$__.$I30-11 = func12
                            End If
                            Logger.Info(String.Format(text6, enumerable5.[Select](func11).Join(";")), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        Else
                            Logger.Info("未找到新增的版本文件夹", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        End If
                    Catch ex3 As Exception
                        Throw New Exception(String.Format("安装新 {0} 版本失败", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), ex3)
                    Finally
                        Try
                            FileUtils.Delete(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_InstallerAddress, False)
                        Catch ex4 As Exception
                            Logger.Warn(ex4, String.Format("安装 {0} 清理文件时出错", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_LoaderName), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        End Try
                    End Try
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 10.0 })
            Else
                Logger.Info(String.Format("检测为非新版 Forge：{0}", CS$<>8__locals1.$VB$Local_LoaderVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                list.Add(New ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean)(String.Format("安装 {0}（方式 B）", CS$<>8__locals1.$VB$Local_LoaderName), Sub(Task As ModLoader.LoaderTask(Of List(Of ModNet.NetFile), Boolean))
                    Dim zipArchive2 As ZipArchive = Nothing
                    Try
                        zipArchive2 = FileUtils.OpenZip(CS$<>8__locals1.$VB$Local_InstallerAddress, Nothing)
                        Task.Progress = 0.2
                        Dim jobject4 As JObject = CType(zipArchive2.GetEntry("install_profile.json").Open().ReadString(Nothing).DeserializeJson(), JObject)
                        Task.Progress = 0.4
                        DirectoryUtils.Create(CS$<>8__locals1.$VB$Local_VersionFolder)
                        Task.Progress = 0.5
                        If jobject4("install") Is Nothing Then
                            Logger.Info(String.Format("开始进行 Forge 安装，Legacy 方式 1：{0}", CS$<>8__locals1.$VB$Local_InstallerAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                            Dim jobject5 As JObject = CType(zipArchive2.GetEntry(jobject4("json").ToString().TrimStart(New Char() { "/"c })).Open().ReadString(Nothing).DeserializeJson(), JObject)
                            jobject5("id") = CS$<>8__locals1.$VB$Local_NewInstanceName
                            FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_NewInstanceName + ".json", jobject5.ToString(), Nothing)
                            Task.Progress = 0.6
                            zipArchive2.Dispose()
                            FileUtils.ExtractToDirectory(CS$<>8__locals1.$VB$Local_InstallerAddress, CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\", Nothing, Nothing)
                            If DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\maven\") Then
                                DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\maven\", CS$<>8__locals1.$VB$Local_McFolder + "libraries\")
                            End If
                            DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\", False)
                        Else
                            Logger.Info(String.Format("开始进行 Forge 安装，Legacy 方式 2：{0}", CS$<>8__locals1.$VB$Local_InstallerAddress), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                            FileUtils.Write(ModMinecraft.McLibGet(CStr(jobject4("install")("path")), True, False, CS$<>8__locals1.$VB$Local_McFolder), zipArchive2.GetEntry(CStr(jobject4("install")("filePath"))).Open())
                            Task.Progress = 0.9
                            jobject4("versionInfo")("id") = CS$<>8__locals1.$VB$Local_NewInstanceName
                            If jobject4("versionInfo")("inheritsFrom") Is Nothing Then
                                CType(jobject4("versionInfo"), JObject).Add("inheritsFrom", CS$<>8__locals1.$VB$Local_Inherit)
                            End If
                            FileUtils.Write(CS$<>8__locals1.$VB$Local_VersionFolder + CS$<>8__locals1.$VB$Local_NewInstanceName + ".json", jobject4("versionInfo").ToString(), Nothing)
                        End If
                    Catch ex5 As Exception
                        Throw New Exception("非新版方式安装 Forge 失败", ex5)
                    Finally
                        Try
                            If zipArchive2 IsNot Nothing Then
                                zipArchive2.Dispose()
                            End If
                            FileUtils.Delete(CS$<>8__locals1.$VB$Local_InstallerAddress, False)
                            DirectoryUtils.Delete(CS$<>8__locals1.$VB$Local_InstallerAddress + "_unrar\", False)
                        Catch ex6 As Exception
                            Logger.Warn(ex6, "非新版方式安装 Forge 清理文件时出错", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        End Try
                    End Try
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0 })
            End If
            Return list
        End Function

        Public Sub ForgeDownloadListItemPreload(Stack As StackPanel, Entries As List(Of ModDownload.DlForgeVersionEntry), OnClick As OurListItem.ClickEventHandler, IsSaveOnly As Boolean)
            If Not Entries.IsSingle() Then
                Dim dlForgeVersionEntry As ModDownload.DlForgeVersionEntry = Nothing
                If Entries.Any() Then
                    dlForgeVersionEntry = Entries(0)
                Else
                    Logger.Info("未找到可用的 Forge 版本", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                End If
                Dim dlForgeVersionEntry2 As ModDownload.DlForgeVersionEntry = Nothing
                Try
                    For Each dlForgeVersionEntry3 As ModDownload.DlForgeVersionEntry In Entries
                        If dlForgeVersionEntry3.IsRecommended Then
                            dlForgeVersionEntry2 = dlForgeVersionEntry3
                        End If
                    Next
                Finally
                    Dim enumerator As List(Of ModDownload.DlForgeVersionEntry).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                If dlForgeVersionEntry IsNot Nothing AndAlso dlForgeVersionEntry Is dlForgeVersionEntry2 Then
                    dlForgeVersionEntry = Nothing
                End If
                If dlForgeVersionEntry2 IsNot Nothing Then
                    Dim myListItem As OurListItem = ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry2, OnClick, IsSaveOnly).Init()
                    myListItem.Info = "推荐版" + If((Operators.CompareString(myListItem.Info, "", False) = 0), "", ("，" + myListItem.Info))
                    Stack.Children.Add(myListItem)
                End If
                If dlForgeVersionEntry IsNot Nothing Then
                    Dim myListItem2 As OurListItem = ModDownloadLib.ForgeDownloadListItem(dlForgeVersionEntry, OnClick, IsSaveOnly).Init()
                    myListItem2.Info = "最新版" + If((Operators.CompareString(myListItem2.Info, "", False) = 0), "", ("，" + myListItem2.Info))
                    Stack.Children.Add(myListItem2)
                End If
                Stack.Children.Add(New TextBlock() With { .Text = "全部版本 (" + Conversions.ToString(Entries.Count) + ")", .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(6.0, 13.0, 0.0, 4.0) })
            End If
        End Sub

        Public Function ForgeDownloadListItem(Entry As ModDownload.DlForgeVersionEntry, OnClick As OurListItem.ClickEventHandler, IsSaveOnly As Boolean) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem()
                myListItem.Title = Entry.VersionName
                myListItem.SnapsToDevicePixels = True
                myListItem.Height = 42.0
                myListItem.Type = OurListItem.CheckType.Clickable
                myListItem.Tag = Entry
                Dim myListItem2 As OurListItem = myListItem
                Dim enumerable As IEnumerable(Of String) = New String() { If((Operators.CompareString(Entry.ReleaseTime, "", False) = 0), "", ("发布于 " + Entry.ReleaseTime)), If(ModBase.ModeDebug, ("种类：" + Entry.Category), "") }
                Dim func As Func(Of String, Boolean)
                If ModDownloadLib._Closure$__.$I32-1 IsNot Nothing Then
                    func = ModDownloadLib._Closure$__.$I32-1
                Else
                    Dim func2 As Func(Of String, Boolean) = Function(d As String) Operators.CompareString(d, "", False) <> 0
                    func = func2
                    ModDownloadLib._Closure$__.$I32-1 = func2
                End If
                myListItem2.Info = enumerable.Where(func).Join("，")
                myListItem.Logo = ModBase.PathImage + "Blocks/Anvil.png"
                Dim myListItem3 As OurListItem = myListItem
                AddHandler myListItem3.Click, OnClick
                If IsSaveOnly Then
                    myListItem3.ContentHandler = AddressOf ModDownloadLib.ForgeSaveContMenuBuild
                Else
                    myListItem3.ContentHandler = AddressOf ModDownloadLib.ForgeContMenuBuild
                End If
                Return myListItem3
            End Function) With { .Height = 42.0 }
        End Function

        Private Sub ForgeContMenuBuild(sender As OurListItem, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z", .ToolTip = "另存为" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR33-16 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR33-16
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.ForgeSave_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR33-16 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            Dim myIconButton3 As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton4 As OurIconButton = myIconButton3
            Dim clickEventHandler3 As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR33-17 IsNot Nothing Then
                clickEventHandler3 = ModDownloadLib._Closure$__.$IR33-17
            Else
                Dim clickEventHandler4 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.ForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler3 = clickEventHandler4
                ModDownloadLib._Closure$__.$IR33-17 = clickEventHandler4
            End If
            AddHandler myIconButton4.Click, clickEventHandler3
            sender.Buttons = New OurIconButton() { myIconButton, myIconButton3 }
        End Sub

        Private Sub ForgeSaveContMenuBuild(sender As OurListItem, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR34-18 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR34-18
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.ForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR34-18 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            sender.Buttons = New OurIconButton() { myIconButton }
        End Sub

        Private Sub ForgeLog_Click(sender As Object, e As RoutedEventArgs)
            Dim dlForgeVersionEntry As ModDownload.DlForgeVersionEntry
            If NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlForgeVersionEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlForgeVersionEntry)
            ElseIf NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlForgeVersionEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlForgeVersionEntry)
            Else
                dlForgeVersionEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlForgeVersionEntry)
            End If
            ModBase.OpenWebsite(String.Format("https://files.minecraftforge.net/maven/net/minecraftforge/forge/{0}-{1}/forge-{2}-{3}-changelog.txt", New Object() { dlForgeVersionEntry.Inherit, dlForgeVersionEntry.VersionName, dlForgeVersionEntry.Inherit, dlForgeVersionEntry.VersionName }))
        End Sub

        Public Sub ForgeSave_Click(sender As Object, e As RoutedEventArgs)
            Dim dlForgeVersionEntry As ModDownload.DlForgeVersionEntry
            If NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlForgeVersionEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlForgeVersionEntry)
            ElseIf NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlForgeVersionEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlForgeVersionEntry)
            Else
                dlForgeVersionEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlForgeVersionEntry)
            End If
            ModDownloadLib.McDownloadForgelikeSave(dlForgeVersionEntry)
        End Sub

        Public Sub McDownloadForgeRecommendedRefresh()
            If Not ModDownloadLib.IsForgeRecommendedRefreshed Then
                ModDownloadLib.IsForgeRecommendedRefreshed = True
                Dim action As Action
                If ModDownloadLib._Closure$__.$I37-0 IsNot Nothing Then
                    action = ModDownloadLib._Closure$__.$I37-0
                Else
                    Dim action2 As Action = Sub()
                        Try
                            Logger.Info("刷新 Forge 推荐版本缓存开始", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                            Dim text As String = ModNet.NetRequestByClientRetry("https://bmclapi2.bangbang93.com/forge/promos", Nothing, Nothing, Nothing, Nothing, "*/*", Nothing, True, Nothing, False)
                            If text.Length < 1000 Then
                                Throw New Exception("获取的结果过短（" + text + "）")
                            End If
                            Dim jcontainer As JContainer = CType(text.DeserializeJson(), JContainer)
                            Dim list As List(Of String) = New List(Of String)()
                            Try
                                For Each jtoken As JToken In jcontainer
                                    Dim jobject As JObject = CType(jtoken, JObject)
                                    If jobject("name") IsNot Nothing AndAlso jobject("build") IsNot Nothing Then
                                        Dim text2 As String = CStr(jobject("name"))
                                        If text2.EndsWithF("-recommended", False) Then
                                            list.Add("""" + text2.Replace("-recommended", """:""" + jobject("build")("version").ToString() + """"))
                                        End If
                                    End If
                                Next
                            Finally
                                Dim enumerator As IEnumerator(Of JToken)
                                If enumerator IsNot Nothing Then
                                    enumerator.Dispose()
                                End If
                            End Try
                            If list.Count < 5 Then
                                Throw New Exception("获取的推荐版本数过少（" + text + "）")
                            End If
                            Dim text3 As String = "{" + list.Join(",") + "}"
                            FileUtils.Write(ModBase.PathTemp + "Cache\ForgeRecommendedList.json", text3, Nothing)
                            Logger.Info("刷新 Forge 推荐版本缓存成功", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        Catch ex As Exception
                            Logger.Warn(ex, "刷新 Forge 推荐版本缓存失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        End Try
                    End Sub
                    action = action2
                    ModDownloadLib._Closure$__.$I37-0 = action2
                End If
                ModBase.RunInNewThread(action, "ForgeRecommendedRefresh", ThreadPriority.Normal)
            End If
        End Sub

        Public Function McDownloadForgeRecommendedGet(McVersionName As String) As String
            Dim text As String
            Try
                If McVersionName Is Nothing Then
                    text = Nothing
                Else
                    Dim text2 As String = FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\ForgeRecommendedList.json", Nothing, Nothing)
                    If String.IsNullOrEmpty(text2) Then
                        Logger.Info("没有 Forge 推荐版本缓存文件", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        text = Nothing
                    Else
                        Dim jobject As JObject = CType(text2.DeserializeJson(), JObject)
                        If jobject Is Nothing OrElse Not(If(McVersionName, "")).Contains(".") OrElse Not jobject.ContainsKey(McVersionName) Then
                            text = Nothing
                        Else
                            text = (If(jobject(McVersionName), "")).ToString()
                        End If
                    End If
                End If
            Catch ex As Exception
                Logger.[Error](ex, String.Format("获取 Forge 推荐版本失败（{0}）", If(McVersionName, "null")), LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                text = Nothing
            End Try
            Return text
        End Function

        Public Sub NeoForgeDownloadListItemPreload(Stack As StackPanel, Entries As List(Of ModDownload.DlNeoForgeListEntry), OnClick As OurListItem.ClickEventHandler, IsSaveOnly As Boolean)
            If Not Entries.IsSingle() Then
                Dim dlNeoForgeListEntry As ModDownload.DlNeoForgeListEntry = Nothing
                Dim dlNeoForgeListEntry2 As ModDownload.DlNeoForgeListEntry = Nothing
                If Entries.Any() Then
                    Try
                        For Each dlNeoForgeListEntry3 As ModDownload.DlNeoForgeListEntry In Entries.ToList()
                            If Not dlNeoForgeListEntry3.IsBeta Then
                                dlNeoForgeListEntry = dlNeoForgeListEntry3
                                Exit For
                            End If
                            If dlNeoForgeListEntry2 Is Nothing Then
                                dlNeoForgeListEntry2 = dlNeoForgeListEntry3
                            End If
                        Next
                        GoTo IL_0069
                    Finally
                        Dim enumerator As List(Of ModDownload.DlNeoForgeListEntry).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                End If
                Logger.Warn("未找到可用的 NeoForge 版本", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                IL_0069:
                If dlNeoForgeListEntry IsNot Nothing Then
                    Dim myListItem As OurListItem = ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry, OnClick, IsSaveOnly).Init()
                    myListItem.Info = If((Operators.CompareString(myListItem.Info, "", False) = 0), "最新稳定版", ("最新" + myListItem.Info))
                    Stack.Children.Add(myListItem)
                End If
                If dlNeoForgeListEntry2 IsNot Nothing Then
                    Dim myListItem2 As OurListItem = ModDownloadLib.NeoForgeDownloadListItem(dlNeoForgeListEntry2, OnClick, IsSaveOnly).Init()
                    myListItem2.Info = If((Operators.CompareString(myListItem2.Info, "", False) = 0), "最新测试版", ("最新" + myListItem2.Info))
                    Stack.Children.Add(myListItem2)
                End If
                Stack.Children.Add(New TextBlock() With { .Text = "全部版本 (" + Conversions.ToString(Entries.Count) + ")", .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(6.0, 13.0, 0.0, 4.0) })
            End If
        End Sub

        Public Function NeoForgeDownloadListItem(Info As ModDownload.DlNeoForgeListEntry, OnClick As OurListItem.ClickEventHandler, IsSaveOnly As Boolean) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem() With { .Title = Info.VersionName, .SnapsToDevicePixels = True, .Height = 42.0, .Type = OurListItem.CheckType.Clickable, .Tag = Info, .Info = If(Info.IsBeta, "测试版", "稳定版"), .Logo = ModBase.PathImage + "Blocks/NeoForge.png" }
                AddHandler myListItem.Click, OnClick
                If IsSaveOnly Then
                    myListItem.ContentHandler = AddressOf ModDownloadLib.NeoForgeSaveContMenuBuild
                Else
                    myListItem.ContentHandler = AddressOf ModDownloadLib.NeoForgeContMenuBuild
                End If
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Private Sub NeoForgeContMenuBuild(sender As OurListItem, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .Logo = "M819.392 0L1024 202.752v652.16a168.96 168.96 0 0 1-168.832 168.768h-104.192a47.296 47.296 0 0 1-10.752 0H283.776a47.232 47.232 0 0 1-10.752 0H168.832A168.96 168.96 0 0 1 0 854.912V168.768A168.96 168.96 0 0 1 168.832 0h650.56z m110.208 854.912V242.112l-149.12-147.776H168.896c-41.088 0-74.432 33.408-74.432 74.432v686.144c0 41.024 33.344 74.432 74.432 74.432h62.4v-190.528c0-33.408 27.136-60.544 60.544-60.544h440.448c33.408 0 60.544 27.136 60.544 60.544v190.528h62.4c41.088 0 74.432-33.408 74.432-74.432z m-604.032 74.432h372.864v-156.736H325.568v156.736z m403.52-596.48a47.168 47.168 0 1 1 0 94.336H287.872a47.168 47.168 0 1 1 0-94.336h441.216z m0-153.728a47.168 47.168 0 1 1 0 94.4H287.872a47.168 47.168 0 1 1 0-94.4h441.216z", .ToolTip = "另存为" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR42-19 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR42-19
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.NeoForgeSave_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR42-19 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            Dim myIconButton3 As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton4 As OurIconButton = myIconButton3
            Dim clickEventHandler3 As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR42-20 IsNot Nothing Then
                clickEventHandler3 = ModDownloadLib._Closure$__.$IR42-20
            Else
                Dim clickEventHandler4 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.NeoForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler3 = clickEventHandler4
                ModDownloadLib._Closure$__.$IR42-20 = clickEventHandler4
            End If
            AddHandler myIconButton4.Click, clickEventHandler3
            sender.Buttons = New OurIconButton() { myIconButton, myIconButton3 }
        End Sub

        Private Sub NeoForgeSaveContMenuBuild(sender As OurListItem, e As EventArgs)
            Dim myIconButton As OurIconButton = New OurIconButton() With { .LogoScale = 1.05, .Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z", .ToolTip = "更新日志" }
            Dim myIconButton2 As OurIconButton = myIconButton
            Dim clickEventHandler As OurIconButton.ClickEventHandler
            If ModDownloadLib._Closure$__.$IR43-21 IsNot Nothing Then
                clickEventHandler = ModDownloadLib._Closure$__.$IR43-21
            Else
                Dim clickEventHandler2 As OurIconButton.ClickEventHandler = Sub(a0 As Object, a1 As EventArgs)
                    ModDownloadLib.NeoForgeLog_Click(RuntimeHelpers.GetObjectValue(a0), CType(a1, RoutedEventArgs))
                End Sub
                clickEventHandler = clickEventHandler2
                ModDownloadLib._Closure$__.$IR43-21 = clickEventHandler2
            End If
            AddHandler myIconButton2.Click, clickEventHandler
            sender.Buttons = New OurIconButton() { myIconButton }
        End Sub

        Private Sub NeoForgeLog_Click(sender As Object, e As RoutedEventArgs)
            Dim dlNeoForgeListEntry As ModDownload.DlNeoForgeListEntry
            If NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlNeoForgeListEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlNeoForgeListEntry)
            ElseIf NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlNeoForgeListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlNeoForgeListEntry)
            Else
                dlNeoForgeListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlNeoForgeListEntry)
            End If
            ModBase.OpenWebsite(dlNeoForgeListEntry.UrlBase + "-changelog.txt")
        End Sub

        Public Sub NeoForgeSave_Click(sender As Object, e As RoutedEventArgs)
            Dim dlNeoForgeListEntry As ModDownload.DlNeoForgeListEntry
            If NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlNeoForgeListEntry = CType(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlNeoForgeListEntry)
            ElseIf NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing) IsNot Nothing Then
                dlNeoForgeListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlNeoForgeListEntry)
            Else
                dlNeoForgeListEntry = CType(NewLateBinding.LateGet(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing), Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing), ModDownload.DlNeoForgeListEntry)
            End If
            ModDownloadLib.McDownloadForgelikeSave(dlNeoForgeListEntry)
        End Sub

        Public Sub McDownloadFabricLoaderSave(DownloadInfo As JObject)
            Try
                Dim text As String = DownloadInfo("url").ToString()
                Dim lastPart As String = PathUtils.GetLastPart(text)
                Dim lastPart2 As String = PathUtils.GetLastPart(DownloadInfo("version").ToString())
                Dim text2 As String = Dialogs.SaveFile("选择保存位置", lastPart, Nothing, New ValueTuple(Of String, String)() { New ValueTuple(Of String, String)("jar", "Fabric 安装器") })
                If text2 IsNot Nothing Then
                    Try
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator = ModLoader.LoaderTaskbar.ToList().GetEnumerator()
                        While enumerator.MoveNext()
                            If Operators.CompareString(enumerator.Current.Name, String.Format("Fabric {0} 安装器下载", lastPart2), False) = 0 Then
                                ModMain.Hint("该版本正在下载中！", ModMain.HintType.Red, True)
                                Return
                            End If
                        End While
                    Finally
                        Dim enumerator As List(Of ModLoader.LoaderBase).Enumerator
                        CType(enumerator, IDisposable).Dispose()
                    End Try
                    Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                    Dim list2 As List(Of String) = New List(Of String)()
                    list2.Add(text)
                    list.Add(New ModNet.LoaderDownload("下载主文件", New List(Of ModNet.NetFile)() From { New ModNet.NetFile(list2.ToArray(), text2, New FileChecker() With { .MinSize = 65536L }, False) }) With { .ProgressWeight = 15.0 })
                    Dim loaderCombo As ModLoader.LoaderCombo(Of JObject) = New ModLoader.LoaderCombo(Of JObject)("Fabric " + lastPart2 + " 安装器下载", list)
                    loaderCombo.OnStateChanged = AddressOf ModDownloadLib.LoaderStateChangedHintOnly
                    loaderCombo.Start(DownloadInfo, False)
                    ModLoader.LoaderTaskbarAdd(Of JObject)(loaderCombo)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                End If
            Catch ex As Exception
                Logger.[Error](ex, "开始 Fabric 安装器下载失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Try
        End Sub

        Private Function McDownloadFabricLoader(FabricVersion As String, MinecraftName As String, McFolder As String) As List(Of ModLoader.LoaderBase)
            Dim Id As String = "fabric-loader-" + FabricVersion + "-" + MinecraftName
            Dim VersionFolder As String = McFolder + "versions\" + Id + "\"
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            MinecraftName = MinecraftName.Replace("∞", "infinite")
            list.Add(New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("获取 Fabric 主文件下载地址", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Task.Output = New List(Of ModNet.NetFile)() From { New ModNet.NetFile(New String() { String.Concat(New String() { "https://bmclapi2.bangbang93.com/fabric-meta/v2/versions/loader/", MinecraftName, "/", FabricVersion, "/profile/json" }), String.Concat(New String() { "https://meta.fabricmc.net/v2/versions/loader/", MinecraftName, "/", FabricVersion, "/profile/json" }) }, VersionFolder + Id + ".json", New FileChecker() With { .IsJson = True }, False) }
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 0.5 })
            list.Add(New ModNet.LoaderDownload("下载 Fabric 主文件", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 2.5 })
            Return list
        End Function

        Public Function FabricDownloadListItem(Entry As JObject, OnClick As OurListItem.ClickEventHandler) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem()
                myListItem.Title = Entry("version").ToString().Replace("+build", "")
                myListItem.SnapsToDevicePixels = True
                myListItem.Height = 42.0
                myListItem.Type = OurListItem.CheckType.Clickable
                myListItem.Tag = Entry
                myListItem.Info = If(Entry("stable").ToObject(Of Boolean)(), "稳定版", "测试版")
                myListItem.Logo = ModBase.PathImage + "Blocks/Fabric.png"
                AddHandler myListItem.Click, OnClick
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Public Function FabricApiDownloadListItem(Entry As ResourceVersion, OnClick As OurListItem.ClickEventHandler) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem()
                myListItem.Title = Entry.Display.Split("]", False)(1).Replace("Fabric API ", "").Replace(" build ", ".").BeforeFirst("+", False).Trim()
                myListItem.SnapsToDevicePixels = True
                myListItem.Height = 42.0
                myListItem.Type = OurListItem.CheckType.Clickable
                myListItem.Tag = Entry
                myListItem.Info = Entry.ReleaseTypeDisplay + "，发布于 " + Entry.ReleaseDate.ToString("yyyy'/'MM'/'dd HH':'mm")
                myListItem.Logo = ModBase.PathImage + "Blocks/Fabric.png"
                AddHandler myListItem.Click, OnClick
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Public Function OptiFabricDownloadListItem(Entry As ResourceVersion, OnClick As OurListItem.ClickEventHandler) As OurVirtualizingElement(Of OurListItem)
            Return New OurVirtualizingElement(Of OurListItem)(Function()
                Dim myListItem As OurListItem = New OurListItem() With { .Title = Entry.Display.Lower().Replace("optifabric-", "").Replace(".jar", "").Trim().TrimStart(New Char() { "v"c }), .SnapsToDevicePixels = True, .Height = 42.0, .Type = OurListItem.CheckType.Clickable, .Tag = Entry, .Info = Entry.ReleaseTypeDisplay + "，发布于 " + Entry.ReleaseDate.ToString("yyyy'/'MM'/'dd HH':'mm"), .Logo = ModBase.PathImage + "Blocks/OptiFabric.png" }
                AddHandler myListItem.Click, OnClick
                Return myListItem
            End Function) With { .Height = 42.0 }
        End Function

        Public Function McInstallLoadersGet(Request As ModDownloadLib.McInstallRequest) As List(Of ModLoader.LoaderBase)
            Dim CS$<>8__locals1 As ModDownloadLib._Closure$__52-0 = New ModDownloadLib._Closure$__52-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Request = Request
            CS$<>8__locals1.$VB$Local_TempMcFolder = ModMain.RequestTaskTempFolder(CS$<>8__locals1.$VB$Local_Request.Components.OptiFineEntry IsNot Nothing OrElse CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge) OrElse CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge))
            Dim optiFineEntry As JObject = CS$<>8__locals1.$VB$Local_Request.Components.OptiFineEntry
            Dim dlOptiFineListEntry As ModDownload.DlOptiFineListEntry = If((optiFineEntry IsNot Nothing), optiFineEntry.ToObject(Of ModDownload.DlOptiFineListEntry)(), Nothing)
            CS$<>8__locals1.$VB$Local_OptiFineFolder = If((dlOptiFineListEntry Is Nothing), Nothing, String.Format("{0}versions\{1}", CS$<>8__locals1.$VB$Local_TempMcFolder, dlOptiFineListEntry.InstanceName))
            CS$<>8__locals1.$VB$Local_ForgeVersion = If(CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Forge), CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions(ModLoaders.Forge), Nothing)
            CS$<>8__locals1.$VB$Local_ForgeFolder = If((CS$<>8__locals1.$VB$Local_ForgeVersion Is Nothing), Nothing, String.Format("{0}versions\forge-{1}", CS$<>8__locals1.$VB$Local_TempMcFolder, CS$<>8__locals1.$VB$Local_ForgeVersion))
            CS$<>8__locals1.$VB$Local_NeoForgeVersion = If(CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.NeoForge), CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions(ModLoaders.NeoForge), Nothing)
            CS$<>8__locals1.$VB$Local_NeoForgeFolder = If((CS$<>8__locals1.$VB$Local_NeoForgeVersion Is Nothing), Nothing, String.Format("{0}versions\neoforge-{1}", CS$<>8__locals1.$VB$Local_TempMcFolder, CS$<>8__locals1.$VB$Local_NeoForgeVersion))
            Dim text As String = If(CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.LiteLoader), CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions(ModLoaders.LiteLoader), Nothing)
            CS$<>8__locals1.$VB$Local_LiteLoaderFolder = If((text Is Nothing), Nothing, String.Format("{0}versions\{1}-LiteLoader", CS$<>8__locals1.$VB$Local_TempMcFolder, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion))
            Dim text2 As String = If(CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.ContainsKey(ModLoaders.Fabric), CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions(ModLoaders.Fabric), Nothing)
            CS$<>8__locals1.$VB$Local_FabricFolder = If((text2 Is Nothing), Nothing, String.Format("{0}versions\fabric-loader-{1}-{2}", CS$<>8__locals1.$VB$Local_TempMcFolder, text2, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion))
            CS$<>8__locals1.$VB$Local_Modable = CS$<>8__locals1.$VB$Local_Request.Components.ModLoaderVersions.Any()
            CS$<>8__locals1.$VB$Local_ModsTempFolder = String.Format("{0}mods\", CS$<>8__locals1.$VB$Local_TempMcFolder)
            CS$<>8__locals1.$VB$Local_OptiFineAsMod = dlOptiFineListEntry IsNot Nothing AndAlso CS$<>8__locals1.$VB$Local_Modable
            If CS$<>8__locals1.$VB$Local_OptiFineAsMod Then
                Logger.Info("OptiFine 将作为 Mod 进行下载", LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                If text IsNot Nothing Then
                    CS$<>8__locals1.$VB$Local_OptiFineFolder = String.Format("{0}{1}\", CS$<>8__locals1.$VB$Local_ModsTempFolder, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion)
                Else
                    CS$<>8__locals1.$VB$Local_OptiFineFolder = CS$<>8__locals1.$VB$Local_ModsTempFolder
                End If
            End If
            If CS$<>8__locals1.$VB$Local_OptiFineFolder IsNot Nothing Then
                Logger.Info(String.Format("OptiFine 缓存：{0}", CS$<>8__locals1.$VB$Local_OptiFineFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End If
            If CS$<>8__locals1.$VB$Local_ForgeFolder IsNot Nothing Then
                Logger.Info(String.Format("Forge 缓存：{0}", CS$<>8__locals1.$VB$Local_ForgeFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End If
            If CS$<>8__locals1.$VB$Local_NeoForgeFolder IsNot Nothing Then
                Logger.Info(String.Format("NeoForge 缓存：{0}", CS$<>8__locals1.$VB$Local_NeoForgeFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End If
            If CS$<>8__locals1.$VB$Local_FabricFolder IsNot Nothing Then
                Logger.Info(String.Format("Fabric 缓存：{0}", CS$<>8__locals1.$VB$Local_FabricFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End If
            If CS$<>8__locals1.$VB$Local_LiteLoaderFolder IsNot Nothing Then
                Logger.Info(String.Format("LiteLoader 缓存：{0}", CS$<>8__locals1.$VB$Local_LiteLoaderFolder), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End If
            Logger.Info(String.Format("对应的原版版本：{0}", CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            CS$<>8__locals1.$VB$Local_VersionFolder = PathUtils.AddSlashSuffix(CS$<>8__locals1.$VB$Local_Request.InstanceFolder)
            If FileUtils.Exists(String.Format("{0}{1}.json", CS$<>8__locals1.$VB$Local_VersionFolder, CS$<>8__locals1.$VB$Local_Request.NewInstanceName)) Then
                ModMain.Hint("版本 " + CS$<>8__locals1.$VB$Local_Request.NewInstanceName + " 已经存在！", ModMain.HintType.Red, True)
                Throw New OperationCanceledException()
            End If
            Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
            CS$<>8__locals1.$VB$Local_ManagedFileSources = New Dictionary(Of String, String)()
            list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("添加忽略标识", Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                MyBase._Lambda$__0()
            End Sub, Nothing, ThreadPriority.Normal) With { .Show = False })
            If CS$<>8__locals1.$VB$Local_Request.Components.FabricApiVersion IsNot Nothing Then
                Dim netFile As ModNet.NetFile = CS$<>8__locals1.$VB$Local_Request.FabricApi.ToNetFile(CS$<>8__locals1.$VB$Local_ModsTempFolder, New ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Dependency), CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModLoaders.Fabric)
                CS$<>8__locals1.$VB$Local_ManagedFileSources.Add("FabricApi", netFile.LocalPath)
                list.Add(New ModNet.LoaderDownload("下载 Fabric API", New List(Of ModNet.NetFile)() From { netFile }) With { .ProgressWeight = 3.0, .Show = True, .Block = False })
            End If
            If CS$<>8__locals1.$VB$Local_Request.Components.OptiFabricVersion IsNot Nothing Then
                Dim netFile2 As ModNet.NetFile = CS$<>8__locals1.$VB$Local_Request.OptiFabric.ToNetFile(CS$<>8__locals1.$VB$Local_ModsTempFolder, New ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Dependency), CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModLoaders.Fabric)
                CS$<>8__locals1.$VB$Local_ManagedFileSources.Add("OptiFabric", netFile2.LocalPath)
                list.Add(New ModNet.LoaderDownload("下载 OptiFabric", New List(Of ModNet.NetFile)() From { netFile2 }) With { .ProgressWeight = 3.0, .Show = True, .Block = False })
            End If
            Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)("下载原版 " + CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModDownloadLib.McDownloadClientLoader(CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_Request.MinecraftJson, CS$<>8__locals1.$VB$Local_VersionFolder)) With { .ProgressWeight = 39.0, .Show = False, .Block = (Not CS$<>8__locals1.$VB$Local_Modable AndAlso dlOptiFineListEntry Is Nothing) }
            list.Add(loaderCombo)
            If dlOptiFineListEntry IsNot Nothing Then
                If CS$<>8__locals1.$VB$Local_OptiFineAsMod Then
                    CS$<>8__locals1.$VB$Local_ManagedFileSources.Add("OptiFine", CS$<>8__locals1.$VB$Local_OptiFineFolder + dlOptiFineListEntry.FileName)
                End If
                list.Add(New ModLoader.LoaderCombo(Of String)("下载 OptiFine " + dlOptiFineListEntry.DisplayName, If(CS$<>8__locals1.$VB$Local_OptiFineAsMod, ModDownloadLib.McDownloadOptiFineSaveLoader(dlOptiFineListEntry, CS$<>8__locals1.$VB$Local_OptiFineFolder + dlOptiFineListEntry.FileName), ModDownloadLib.McDownloadOptiFineLoader(dlOptiFineListEntry, CS$<>8__locals1.$VB$Local_TempMcFolder, loaderCombo, CS$<>8__locals1.$VB$Local_VersionFolder))) With { .ProgressWeight = CDbl(If(CS$<>8__locals1.$VB$Local_Modable, 16, 24)), .Show = False, .Block = Not CS$<>8__locals1.$VB$Local_Modable })
            End If
            If CS$<>8__locals1.$VB$Local_ForgeVersion IsNot Nothing Then
                list.Add(New ModLoader.LoaderCombo(Of String)("下载 Forge " + CS$<>8__locals1.$VB$Local_ForgeVersion, ModDownloadLib.McDownloadForgelikeLoader(False, CS$<>8__locals1.$VB$Local_ForgeVersion, "forge-" + CS$<>8__locals1.$VB$Local_ForgeVersion, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_Request.ForgeEntry, CS$<>8__locals1.$VB$Local_TempMcFolder, loaderCombo, CS$<>8__locals1.$VB$Local_VersionFolder)) With { .ProgressWeight = 25.0, .Show = False, .Block = (text2 Is Nothing AndAlso CS$<>8__locals1.$VB$Local_Request.LiteLoaderEntry Is Nothing AndAlso CS$<>8__locals1.$VB$Local_Request.NeoForgeEntry Is Nothing) })
            End If
            If CS$<>8__locals1.$VB$Local_NeoForgeVersion IsNot Nothing Then
                list.Add(New ModLoader.LoaderCombo(Of String)("下载 NeoForge " + CS$<>8__locals1.$VB$Local_NeoForgeVersion, ModDownloadLib.McDownloadForgelikeLoader(True, CS$<>8__locals1.$VB$Local_NeoForgeVersion, "neoforge-" + CS$<>8__locals1.$VB$Local_NeoForgeVersion, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_Request.NeoForgeEntry, CS$<>8__locals1.$VB$Local_TempMcFolder, loaderCombo, CS$<>8__locals1.$VB$Local_VersionFolder)) With { .ProgressWeight = 25.0, .Show = False, .Block = (CS$<>8__locals1.$VB$Local_Request.ForgeEntry Is Nothing AndAlso text2 Is Nothing AndAlso CS$<>8__locals1.$VB$Local_Request.LiteLoaderEntry Is Nothing) })
            End If
            If text IsNot Nothing Then
                list.Add(New ModLoader.LoaderCombo(Of String)("下载 LiteLoader " + CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, ModDownloadLib.McDownloadLiteLoaderLoader(CS$<>8__locals1.$VB$Local_Request.LiteLoaderEntry, CS$<>8__locals1.$VB$Local_TempMcFolder)) With { .ProgressWeight = 1.0, .Show = False, .Block = (text2 = Nothing) })
            End If
            If text2 IsNot Nothing Then
                list.Add(New ModLoader.LoaderCombo(Of String)("下载 Fabric " + text2, ModDownloadLib.McDownloadFabricLoader(text2, CS$<>8__locals1.$VB$Local_Request.Components.MinecraftVersion, CS$<>8__locals1.$VB$Local_TempMcFolder)) With { .ProgressWeight = 2.0, .Show = False, .Block = True })
            End If
            CS$<>8__locals1.$VB$Local_MinecraftFolder = ModMinecraft.McFolderSelected
            CS$<>8__locals1.$VB$Local_TargetFolder = Nothing
            list.Add(New ModLoader.LoaderTask(Of String, String)("安装游戏", Sub(Task As ModLoader.LoaderTask(Of String, String))
                ModDownloadLib.MergeJson(CS$<>8__locals1.$VB$Local_VersionFolder, CS$<>8__locals1.$VB$Local_OptiFineFolder, CS$<>8__locals1.$VB$Local_OptiFineAsMod, CS$<>8__locals1.$VB$Local_ForgeFolder, CS$<>8__locals1.$VB$Local_NeoForgeFolder, CS$<>8__locals1.$VB$Local_FabricFolder, CS$<>8__locals1.$VB$Local_LiteLoaderFolder)
                Task.Progress = 0.2
                DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_TempMcFolder + "libraries", CS$<>8__locals1.$VB$Local_MinecraftFolder + "libraries")
                Task.Progress = 0.8
                CS$<>8__locals1.$VB$Local_TargetFolder = If(CS$<>8__locals1.$VB$Local_Request.IsIntermediateOutput, CS$<>8__locals1.$VB$Local_VersionFolder, New ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder).PathIndie)
                Dim text3 As String = CS$<>8__locals1.$VB$Local_TargetFolder + "mods\"
                If DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_ModsTempFolder) Then
                    DirectoryUtils.Copy(CS$<>8__locals1.$VB$Local_ModsTempFolder, text3)
                ElseIf CS$<>8__locals1.$VB$Local_Modable Then
                    DirectoryUtils.Create(text3)
                    Logger.Info(String.Format("自动创建 Mod 文件夹：{0}", text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                End If
                Dim text4 As String = CS$<>8__locals1.$VB$Local_TargetFolder + "resourcepacks\"
                DirectoryUtils.Create(text4)
                Logger.Info(String.Format("自动创建资源包文件夹：{0}", text4), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 2.0, .Block = True })
            list.Add(New ModLoader.LoaderCombo(Of String)("下载游戏支持库文件", New List(Of ModLoader.LoaderBase)() From { New ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile))("分析游戏支持库文件（副加载器）", Sub(Task As ModLoader.LoaderTask(Of String, List(Of ModNet.NetFile)))
                Task.Output = ModMinecraft.McLibNetFilesFromInstance(New ModMinecraft.McInstance(CS$<>8__locals1.$VB$Local_VersionFolder))
            End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0, .Show = False }, New ModNet.LoaderDownload("下载游戏支持库文件（副加载器）", New List(Of ModNet.NetFile)()) With { .ProgressWeight = 7.0, .Show = False } }) With { .ProgressWeight = 8.0 })
            list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("记录安装清单", Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                MyBase._Lambda$__3()
            End Sub, Nothing, ThreadPriority.Normal) With { .Show = False })
            If Not CS$<>8__locals1.$VB$Local_Request.IsIntermediateOutput Then
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("删除忽略标识", Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                    MyBase._Lambda$__4()
                End Sub, Nothing, ThreadPriority.Normal) With { .Show = False })
            End If
            Return list
        End Function

        Private Sub MergeJson(VersionFolder As String, OptiFineFolder As String, OptiFineAsMod As Boolean, ForgeFolder As String, NeoForgeFolder As String, FabricFolder As String, LiteLoaderFolder As String)
            Dim list As List(Of ValueTuple(Of String, String)) = New List(Of ValueTuple(Of String, String))() From { New ValueTuple(Of String, String)("Minecraft", VersionFolder) }
            If OptiFineFolder IsNot Nothing AndAlso Not OptiFineAsMod Then
                list.Add(New ValueTuple(Of String, String)("OptiFine", OptiFineFolder))
            End If
            If ForgeFolder IsNot Nothing Then
                list.Add(New ValueTuple(Of String, String)("Forge", ForgeFolder))
            End If
            If NeoForgeFolder IsNot Nothing Then
                list.Add(New ValueTuple(Of String, String)("NeoForge", NeoForgeFolder))
            End If
            If LiteLoaderFolder IsNot Nothing Then
                list.Add(New ValueTuple(Of String, String)("LiteLoader", LiteLoaderFolder))
            End If
            If FabricFolder IsNot Nothing Then
                list.Add(New ValueTuple(Of String, String)("Fabric", FabricFolder))
            End If
            Dim text As String = String.Format("开始进行版本合并，输出：{0}，", VersionFolder)
            Dim text2 As String = "，"
            Dim enumerable As IEnumerable(Of ValueTuple(Of String, String)) = list
            Dim func As Func(Of ValueTuple(Of String, String), String)
            If ModDownloadLib._Closure$__.$I53-0 IsNot Nothing Then
                func = ModDownloadLib._Closure$__.$I53-0
            Else
                Dim func2 As Func(Of ValueTuple(Of String, String), String) = Function(<TupleElementNames(New String() { "Name", "Folder" })> Source As ValueTuple(Of String, String)) String.Format("{0}：{1}", Source.Item1, Source.Item2)
                func = func2
                ModDownloadLib._Closure$__.$I53-0 = func2
            End If
            Logger.Info(text + String.Join(text2, enumerable.[Select](func)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
            Dim list2 As List(Of ValueTuple(Of String, JObject)) = New List(Of ValueTuple(Of String, JObject))()
            Try
                For Each valueTuple As ValueTuple(Of String, String) In list
                    Dim text3 As String = Path.Combine(valueTuple.Item2, PathUtils.GetLastPart(valueTuple.Item2) + ".json")
                    Dim text4 As String = If(FileUtils.TryReadAsString(text3, Nothing, Nothing), "")
                    If Not text4.StartsWithF("{", False) Then
                        Throw New Exception(String.Format("{0} json 有误，地址：{1}，前段内容：{2}", valueTuple.Item1, text3, text4.Substring(0, Math.Min(text4.Length, 1000))))
                    End If
                    list2.Add(New ValueTuple(Of String, JObject)(valueTuple.Item1, text4.DeserializeJson()))
                Next
            Finally
                Dim enumerator As List(Of ValueTuple(Of String, String)).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Dim enumerable2 As IEnumerable(Of ValueTuple(Of String, JObject)) = list2
            Dim func3 As Func(Of ValueTuple(Of String, JObject), String)
            If ModDownloadLib._Closure$__.$I53-1 IsNot Nothing Then
                func3 = ModDownloadLib._Closure$__.$I53-1
            Else
                Dim func4 As Func(Of ValueTuple(Of String, JObject), String) = Function(<TupleElementNames(New String() { "Name", "Json" })> Profile As ValueTuple(Of String, JObject))
                    Dim jtoken As JToken = Profile.Item2("minecraftArguments")
                    Return If(If((jtoken IsNot Nothing), jtoken.ToString(), Nothing), " ")
                End Function
                func3 = func4
                ModDownloadLib._Closure$__.$I53-1 = func4
            End If
            Dim text5 As String = enumerable2.[Select](func3).Join(" ")
            Dim item As JObject = list2(0).Item2
            Try
                For Each valueTuple2 As ValueTuple(Of String, JObject) In list2.Skip(1)
                    valueTuple2.Item2.Remove("releaseTime")
                    valueTuple2.Item2.Remove("time")
                    item.Merge(valueTuple2.Item2)
                Next
            Finally
                Dim enumerator2 As IEnumerator(Of ValueTuple(Of String, JObject))
                If enumerator2 IsNot Nothing Then
                    enumerator2.Dispose()
                End If
            End Try
            Dim list3 As List(Of String) = New List(Of String)()
            Try
                Dim enumerable3 As IEnumerable(Of String) = text5.Split(" ", True)
                Dim func5 As Func(Of String, String)
                If ModDownloadLib._Closure$__.$I53-2 IsNot Nothing Then
                    func5 = ModDownloadLib._Closure$__.$I53-2
                Else
                    Dim func6 As Func(Of String, String) = Function(Value As String) Value.Trim()
                    func5 = func6
                    ModDownloadLib._Closure$__.$I53-2 = func6
                End If
                For Each text6 As String In enumerable3.[Select](func5)
                    If Not text6.StartsWithF("-", False) AndAlso list3.Any() Then
                        Dim text7 As String = list3.Last()
                        If text7.StartsWithF("-", False) AndAlso Not text7.Contains(" ") Then
                            ' The following expression was wrapped in a checked-expression
                            list3(list3.Count - 1) = text7 + " " + text6
                            Continue For
                        End If
                    End If
                    list3.Add(text6)
                Next
            Finally
                Dim enumerator3 As IEnumerator(Of String)
                If enumerator3 IsNot Nothing Then
                    enumerator3.Dispose()
                End If
            End Try
            If list3.Any() Then
                item("minecraftArguments") = list3.Distinct().Join(" ")
            End If
            item.Remove("_comment_")
            item.Remove("inheritsFrom")
            item.Remove("jar")
            Dim lastPart As String = PathUtils.GetLastPart(VersionFolder)
            item("id") = lastPart
            FileUtils.Write(Path.Combine(VersionFolder, lastPart + ".json"), item.ToString(), Nothing)
            Logger.Info(String.Format("版本合并 {0} 完成", lastPart), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
        End Sub

        Public Function McInstallStart(Request As ModDownloadLib.McInstallRequest) As Boolean
            Dim flag As Boolean
            Try
                Dim loaderCombo As ModLoader.LoaderCombo(Of String) = New ModLoader.LoaderCombo(Of String)(Request.NewInstanceName + " 安装", ModDownloadLib.McInstallLoadersGet(Request))
                loaderCombo.OnStateChanged = AddressOf ModDownloadLib.McInstallState
                loaderCombo.Start(Request.InstanceFolder, False)
                ModLoader.LoaderTaskbarAdd(Of String)(loaderCombo)
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                ModMain.FrmMain.BtnExtraDownload.Ribble()
                flag = True
            Catch ex As Exception
                Logger.[Error](ex, "开始合并安装失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                flag = False
            End Try
            Return flag
        End Function

        Public Function McModificationStart(Request As ModDownloadLib.McInstallRequest, Instance As ModMinecraft.McInstance) As Boolean
            Dim CS$<>8__locals1 As ModDownloadLib._Closure$__55-0 = New ModDownloadLib._Closure$__55-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_Request = Request
            CS$<>8__locals1.$VB$Local_Instance = Instance
            Dim flag As Boolean
            Try
                Dim CS$<>8__locals2 As ModDownloadLib._Closure$__55-1 = New ModDownloadLib._Closure$__55-1(CS$<>8__locals2)
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1
                CS$<>8__locals2.$VB$Local_TargetFolder = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathIndie
                Dim list As List(Of ModLoader.LoaderBase) = New List(Of ModLoader.LoaderBase)()
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("检查 Natives 文件占用", Sub(a0 As ModLoader.LoaderTask(Of Integer, Integer))
                    MyBase._Lambda$__0()
                End Sub, Nothing, ThreadPriority.Normal) With { .Show = False })
                CS$<>8__locals2.$VB$Local_BuildFolder = ModMain.RequestTaskTempFolder(False) + CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Name + "\"
                Dim mcInstallRequest As ModDownloadLib.McInstallRequest = New ModDownloadLib.McInstallRequest() With { .NewInstanceName = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Name, .InstanceFolder = CS$<>8__locals2.$VB$Local_BuildFolder, .Components = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.Components, .MinecraftJson = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.MinecraftJson, .ForgeEntry = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.ForgeEntry, .NeoForgeEntry = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NeoForgeEntry, .FabricApi = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.FabricApi, .OptiFabric = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.OptiFabric, .LiteLoaderEntry = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.LiteLoaderEntry, .IsIntermediateOutput = True }
                list.AddRange(ModDownloadLib.McInstallLoadersGet(mcInstallRequest))
                CS$<>8__locals2.$VB$Local_RenameFailed = False
                list.Add(New ModLoader.LoaderTask(Of Integer, Integer)("修改版本", Sub(Task As ModLoader.LoaderTask(Of Integer, Integer))
                    Dim CS$<>8__locals3 As ModDownloadLib._Closure$__55-2 = New ModDownloadLib._Closure$__55-2(CS$<>8__locals3)
                    CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3 = CS$<>8__locals2
                    ModDownloadLib.McInstallCommit(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance, Configs.McInstallManifest.[Get](New ModMinecraft.McInstance(CS$<>8__locals2.$VB$Local_BuildFolder).Config), CS$<>8__locals2.$VB$Local_BuildFolder, CS$<>8__locals2.$VB$Local_TargetFolder)
                    CS$<>8__locals3.$VB$Local_NewInstance = Nothing
                    If CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NewInstanceName Is Nothing Then
                        CS$<>8__locals3.$VB$Local_NewInstance = New ModMinecraft.McInstance(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion).Load()
                    Else
                        Try
                            CS$<>8__locals3.$VB$Local_NewInstance = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Rename(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NewInstanceName)
                        Catch ex2 As Exception
                            CS$<>8__locals2.$VB$Local_RenameFailed = True
                            Logger.Warn(ex2, String.Format("版本修改成功，但重命名为 {0} 失败：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Request.NewInstanceName, ex2.GetDisplay(False)), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                            CS$<>8__locals3.$VB$Local_NewInstance = New ModMinecraft.McInstance(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion).Load()
                        End Try
                    End If
                    If CS$<>8__locals3.$VB$Local_NewInstance Is Nothing Then
                        CS$<>8__locals3.$VB$Local_NewInstance = New ModMinecraft.McInstance(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion).Load()
                    End If
                    ModBase.RunInUiWait(Sub()
                        ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "")
                        If PageInstanceLeft.Instance IsNot Nothing AndAlso PathUtils.IsSame(PageInstanceLeft.Instance.PathVersion, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion) Then
                            PageInstanceLeft.Instance = CS$<>8__locals3.$VB$Local_NewInstance
                        End If
                        If ModMinecraft.McInstanceSelected IsNot Nothing AndAlso PathUtils.IsSame(ModMinecraft.McInstanceSelected.PathVersion, CS$<>8__locals3.$VB$NonLocal_$VB$Closure_3.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion) Then
                            ModMinecraft.McInstanceSelected = CS$<>8__locals3.$VB$Local_NewInstance
                        End If
                    End Sub)
                End Sub, Nothing, ThreadPriority.Normal) With { .ProgressWeight = 1.0, .Block = True })
                CS$<>8__locals2.$VB$Local_Loader = New ModLoader.LoaderCombo(Of String)(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.Name + " 修改", list) With { .OnStateChanged = Sub(CurrentLoader As ModLoader.LoaderBase)
                    Select Case CurrentLoader.State
                        Case ModBase.LoadState.Finished
                            If Not CS$<>8__locals2.$VB$Local_RenameFailed Then
                                ModMain.Hint(CurrentLoader.Name + "成功！", ModMain.HintType.Green, True)
                                Return
                            End If
                            ModMain.Hint(CurrentLoader.Name + "成功，但重命名失败！", ModMain.HintType.Green, True)
                            Return
                        Case ModBase.LoadState.Failed
                            ModMain.OurMsgBox(CurrentLoader.[Error].GetDisplay(True), CurrentLoader.Name + "失败", "确定", "", "", True, True, False, Nothing, Nothing, Nothing)
                            Return
                        Case ModBase.LoadState.Canceled
                            ModMain.Hint(CurrentLoader.Name + "已取消！", ModMain.HintType.Blue, True)
                            Return
                        Case Else
                            Return
                    End Select
                End Sub }
                Dim redoableWorker As RedoableWorker = New RedoableWorker(Sub()
                    CS$<>8__locals2.$VB$Local_Loader.WaitForExit(CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion, Nothing, False, Nothing, Nothing)
                End Sub, "McModificationStart")
                If ModMinecraft.McInstanceLock.TryAcquire(redoableWorker, CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_Instance.PathVersion) Then
                    Dim redoableWorkerBase As RedoableWorkerBase(Of NoType) = redoableWorker
                    Dim action As Action
                    If ModDownloadLib._Closure$__.$I55-5 IsNot Nothing Then
                        action = ModDownloadLib._Closure$__.$I55-5
                    Else
                        Dim action2 As Action = Sub()
                            Dim action3 As Action
                            If ModDownloadLib._Closure$__.$I55-6 IsNot Nothing Then
                                action3 = ModDownloadLib._Closure$__.$I55-6
                            Else
                                Dim action4 As Action = Sub()
                                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                                End Sub
                                action3 = action4
                                ModDownloadLib._Closure$__.$I55-6 = action4
                            End If
                            ModBase.RunInUi(action3, False)
                        End Sub
                        action = action2
                        ModDownloadLib._Closure$__.$I55-5 = action2
                    End If
                    AddHandler redoableWorkerBase.Stopped, action
                    redoableWorker.Start(Nothing, Nothing)
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                    ModLoader.LoaderTaskbarAdd(Of String)(CS$<>8__locals2.$VB$Local_Loader)
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh()
                    ModMain.FrmMain.BtnExtraDownload.Ribble()
                    flag = True
                Else
                    Logger.[Error]("版本被其他 Worker 占用，版本修改失败", LogBehavior.AlertThenFeedback, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                    flag = False
                End If
            Catch ex As Exception
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
                Logger.[Error](ex, "开始修改版本失败", LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                flag = False
            End Try
            Return flag
        End Function

        Public Sub McInstallCommit(Instance As ModMinecraft.McInstance, NewManifest As McInstallManifest, BuildFolder As String, TargetFolder As String)
            Dim CS$<>8__locals1 As ModDownloadLib._Closure$__56-0 = New ModDownloadLib._Closure$__56-0(CS$<>8__locals1)
            CS$<>8__locals1.$VB$Local_TargetFolder = TargetFolder
            Dim mcInstallManifest As McInstallManifest = Configs.McInstallManifest.[Get](Instance.Config)
            Dim readOnlyList As IReadOnlyList(Of McInstallManifest.ManagedFile) = If(If((mcInstallManifest IsNot Nothing), mcInstallManifest.ManagedFiles, Nothing), New McInstallManifest.ManagedFile(-1) {})
            Dim managedFiles As IReadOnlyList(Of McInstallManifest.ManagedFile) = NewManifest.ManagedFiles
            Dim list As List(Of String) = New List(Of String)()
            Dim list2 As List(Of ValueTuple(Of String, String)) = New List(Of ValueTuple(Of String, String))()
            Try
                For Each managedFile As McInstallManifest.ManagedFile In managedFiles
                    list2.Add(New ValueTuple(Of String, String)(Path.Combine(BuildFolder, managedFile.RelativePath), Path.Combine(CS$<>8__locals1.$VB$Local_TargetFolder, managedFile.RelativePath)))
                Next
            Finally
                Dim enumerator As IEnumerator(Of McInstallManifest.ManagedFile)
                If enumerator IsNot Nothing Then
                    enumerator.Dispose()
                End If
            End Try
            Dim enumerable As IEnumerable(Of McInstallManifest.ManagedFile) = managedFiles
            Dim func As Func(Of McInstallManifest.ManagedFile, String)
            If ModDownloadLib._Closure$__.$I56-0 IsNot Nothing Then
                func = ModDownloadLib._Closure$__.$I56-0
            Else
                Dim func2 As Func(Of McInstallManifest.ManagedFile, String) = Function(File As McInstallManifest.ManagedFile) File.RelativePath
                func = func2
                ModDownloadLib._Closure$__.$I56-0 = func2
            End If
            Dim hashSet As HashSet(Of String) = New HashSet(Of String)(enumerable.[Select](func))
            Try
                For Each managedFile2 As McInstallManifest.ManagedFile In readOnlyList
                    If Not hashSet.Contains(managedFile2.RelativePath) Then
                        list.Add(PathUtils.ForCompare(Path.Combine(CS$<>8__locals1.$VB$Local_TargetFolder, managedFile2.RelativePath)))
                    End If
                Next
            Finally
                Dim enumerator2 As IEnumerator(Of McInstallManifest.ManagedFile)
                If enumerator2 IsNot Nothing Then
                    enumerator2.Dispose()
                End If
            End Try
            If DirectoryUtils.Exists(BuildFolder + "resources\") Then
                DirectoryUtils.Copy(BuildFolder + "resources\", CS$<>8__locals1.$VB$Local_TargetFolder + "resources\")
            End If
            Dim jsonPath As String = Instance.GetJsonPath()
            list.Add(jsonPath)
            list.Add(Path.ChangeExtension(jsonPath, ".jar"))
            list.Add(Instance.PathVersion + Instance.Name + ".jar")
            list = list.Distinct(StringComparer.OrdinalIgnoreCase).ToList()
            Instance.Config.Save()
            Try
                For Each text As String In list
                    FileUtils.Delete(text, True)
                Next
            Finally
                Dim enumerator3 As List(Of String).Enumerator
                CType(enumerator3, IDisposable).Dispose()
            End Try
            FileUtils.Move(BuildFolder + Instance.Name + ".jar", Instance.PathVersion + Instance.Name + ".jar", False)
            FileUtils.Move(BuildFolder + Instance.Name + ".json", jsonPath, False)
            Try
                For Each valueTuple As ValueTuple(Of String, String) In list2
                    DirectoryUtils.Create(PathUtils.RemoveLastPart(valueTuple.Item2))
                    FileUtils.Copy(valueTuple.Item1, valueTuple.Item2)
                Next
            Finally
                Dim enumerator4 As List(Of ValueTuple(Of String, String)).Enumerator
                CType(enumerator4, IDisposable).Dispose()
            End Try
            Dim list3 As List(Of McInstallManifest.ManagedFile) = managedFiles.[Select](Function(File As McInstallManifest.ManagedFile) New McInstallManifest.ManagedFile(File.RelativePath, File.Sha1, FileUtils.GetInfo(Path.Combine(CS$<>8__locals1.$VB$Local_TargetFolder, File.RelativePath)).LastWriteTimeUtc, File.Component)).ToList()
            Configs.McInstallManifest.[Set](New McInstallManifest(NewManifest.SchemaVersion, NewManifest.Components, NewManifest.LastModifiedUtc, list3), Instance.Config)
            Instance.Config.Save()
        End Sub

        Public Sub LoaderStateChangedHintOnly(Loader As ModLoader.LoaderBase)
            Select Case Loader.State
                Case ModBase.LoadState.Finished
                    ModMain.Hint(Loader.Name + "成功！", ModMain.HintType.Green, True)
                    Return
                Case ModBase.LoadState.Failed
                    ModMain.Hint(Loader.Name + "失败：" + Loader.[Error].GetDisplay(False), ModMain.HintType.Red, True)
                    Return
                Case ModBase.LoadState.Canceled
                    ModMain.Hint(Loader.Name + "已取消！", ModMain.HintType.Blue, True)
                    Return
                Case Else
                    Return
            End Select
        End Sub

        Public Sub McInstallState(Loader As ModLoader.LoaderBase)
            Select Case Loader.State
                Case ModBase.LoadState.Loading
                    Return
                Case ModBase.LoadState.Finished
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "")
                    ModMain.Hint(Loader.Name + "成功！", ModMain.HintType.Green, True)
                Case ModBase.LoadState.Failed
                    ModMain.OurMsgBox(Loader.[Error].GetDisplay(True), Loader.Name + "失败", "确定", "", "", True, True, False, Nothing, Nothing, Nothing)
                Case ModBase.LoadState.Canceled
                    ModMain.Hint(Loader.Name + "已取消！", ModMain.HintType.Blue, True)
            End Select
            ModDownloadLib.McInstallFailedClearFolder(Loader)
            ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\", False)
        End Sub

        Public Sub McInstallFailedClearFolder(Loader As Object)
            If Not Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(Loader, Nothing, "State", New Object(-1) {}, Nothing, Nothing, Nothing), ModBase.LoadState.Failed, False) OrElse Not Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(Loader, Nothing, "State", New Object(-1) {}, Nothing, Nothing, Nothing), ModBase.LoadState.Canceled, False) Then
                ModBase.RunInThread(Sub()
                    Try
                        Thread.Sleep(1000)
                        If DirectoryUtils.Exists(Conversions.ToString(Operators.ConcatenateObject(NewLateBinding.LateGet(Loader, Nothing, "Input", New Object(-1) {}, Nothing, Nothing, Nothing), "saves\"))) OrElse DirectoryUtils.Exists(Conversions.ToString(Operators.ConcatenateObject(NewLateBinding.LateGet(Loader, Nothing, "Input", New Object(-1) {}, Nothing, Nothing, Nothing), "versions\"))) Then
                            Logger.Warn(String.Format("由于版本已被独立启动，不清理版本文件夹：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Loader, Nothing, "Input", New Object(-1) {}, Nothing, Nothing, Nothing))), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                        Else
                            Logger.Warn(String.Format("由于下载失败或取消，清理版本文件夹：{0}", RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(Loader, Nothing, "Input", New Object(-1) {}, Nothing, Nothing, Nothing))), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                            DirectoryUtils.Delete(Conversions.ToString(NewLateBinding.LateGet(Loader, Nothing, "Input", New Object(-1) {}, Nothing, Nothing, Nothing)), False)
                        End If
                    Catch ex As Exception
                        Logger.Warn(ex, "下载失败或取消后清理版本文件夹失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Pages\PageDownload\ModDownloadLib.vb")
                    End Try
                End Sub)
            End If
        End Sub

        Private Const McDownloadClientLibName As String = "下载原版支持库文件"

        Private Const McDownloadClientJsonName As String = "下载原版 json 文件"

        Private IsForgeRecommendedRefreshed As Boolean = False

        Private InstallSyncLock As Object = RuntimeHelpers.GetObjectValue(New Object())

        Private VanillaSyncLock As Object = RuntimeHelpers.GetObjectValue(New Object())

        Public Class McInstallRequest
            Public Sub New()
                Me.MinecraftJson = Nothing
                Me.ForgeEntry = Nothing
                Me.NeoForgeEntry = Nothing
                Me.FabricApi = Nothing
                Me.OptiFabric = Nothing
                Me.LiteLoaderEntry = Nothing
                Me.IsIntermediateOutput = False
            End Sub

            Public NewInstanceName As String

            Public InstanceFolder As String

            Public Components As McComponents

            Public MinecraftJson As String

            Public ForgeEntry As ModDownload.DlForgeVersionEntry

            Public NeoForgeEntry As ModDownload.DlNeoForgeListEntry

            Public FabricApi As ResourceVersion

            Public OptiFabric As ResourceVersion

            Public LiteLoaderEntry As ModDownload.DlLiteLoaderListEntry

            Public IsIntermediateOutput As Boolean
        End Class
    End Module
End Namespace
