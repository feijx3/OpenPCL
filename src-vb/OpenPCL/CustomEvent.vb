Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Windows
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class CustomEvent
        Inherits DependencyObject

        Public Property Type As CustomEvent.EventType
            Get
                Dim Value As CustomEvent.EventType = CustomEvent.EventType.None
                ModBase.RunInUiWait(Sub()
                    Value = CType(Conversions.ToInteger(Me.GetValue(CustomEvent.TypeProperty)), CustomEvent.EventType)
                End Sub)
                Return Value
            End Get
            Set(value As CustomEvent.EventType)
                MyBase.SetValue(CustomEvent.TypeProperty, value)
            End Set
        End Property

        Public Property Data As String
            Get
                Dim Value As String = Nothing
                ModBase.RunInUiWait(Sub()
                    Value = Conversions.ToString(Me.GetValue(CustomEvent.DataProperty))
                End Sub)
                Return Value
            End Get
            Set(value As String)
                MyBase.SetValue(CustomEvent.DataProperty, value)
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(Type As CustomEvent.EventType, Data As String)
            Me.Type = Type
            Me.Data = Data
        End Sub

        Public Sub Raise()
            CustomEvent.Raise(Me.Type, Me.Data)
        End Sub

        Public Shared Sub Raise(Type As CustomEvent.EventType, Arg As String)
            If Type <> CustomEvent.EventType.None Then
                Logger.Info(String.Format("执行自定义事件：{0}, {1}", Type, Arg), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
                If Arg = Nothing Then
                    Arg = ""
                End If
                Dim Args As String() = Arg.Split("|", False)
                Dim Arg0 As String = Arg.BeforeFirst("|", False)
                Dim Arg1 As String = Arg.AfterFirst("|", False)
                Try
                    Select Case Type
                        Case CustomEvent.EventType.打开网页
                            Arg = Arg.Replace("\", "/")
                            If Not Arg.Contains("://") OrElse Arg.StartsWithF("file", True) Then
                                ModMain.OurMsgBox("EventData 必须为一个网址。" & vbCrLf & "如果想要启动程序，请将 EventType 改为 打开文件。", "事件执行失败", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                                Return
                            End If
                            ModMain.Hint("正在开启，请稍候：" + Arg, ModMain.HintType.Blue, True)
                            ModBase.RunInThread(Sub()
                                ModBase.OpenWebsite(Arg)
                            End Sub)
                            GoTo IL_0683
                        Case CustomEvent.EventType.打开文件, CustomEvent.EventType.打开帮助, CustomEvent.EventType.执行命令
                            ModBase.RunInThread(Sub()
                                Try
                                    Dim absoluteUrls As String() = CustomEvent.GetAbsoluteUrls(Arg0, Type)
                                    Dim text As String = absoluteUrls(0)
                                    Dim text2 As String = absoluteUrls(1)
                                    Logger.Info(String.Format("打开类自定义事件实际路径：{0}，工作目录：{1}", text, text2), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
                                    If Type = CustomEvent.EventType.打开帮助 Then
                                        PageOtherHelp.EnterHelpPage(text)
                                    ElseIf CustomEvent.EventSafetyConfirm("即将执行：" + text + If((Args.Length >= 2), (" " + Arg1), "")) Then
                                        ModBase.StartProcess(New ProcessStartInfo() With { .Arguments = If((Args.Length >= 2), Arg1, ""), .FileName = text, .WorkingDirectory = text2 })
                                    End If
                                Catch ex3 As Exception
                                    Logger.[Error](ex3, "执行打开类自定义事件失败", LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
                                End Try
                            End Sub)
                            GoTo IL_0683
                        Case CustomEvent.EventType.启动游戏
                            If Operators.CompareString(Arg0, "\current", False) = 0 Then
                                If ModMinecraft.McInstanceSelected Is Nothing Then
                                    ModMain.Hint("请先选择一个 Minecraft 版本！", ModMain.HintType.Red, True)
                                    Return
                                End If
                                Arg0 = ModMinecraft.McInstanceSelected.Name
                            End If
                            ModBase.RunInUi(Sub()
                                If ModLaunch.McLaunchStart(New ModLaunch.McLaunchOptions() With { .ServerIp = If((Args.Length >= 2), Args(1), Nothing), .Instance = New ModMinecraft.McInstance(Arg0) }) Then
                                    ModMain.Hint(String.Format("正在启动 {0}……", Arg0), ModMain.HintType.Blue, True)
                                End If
                            End Sub, False)
                            GoTo IL_0683
                        Case CustomEvent.EventType.复制文本
                            ModBase.ClipboardSet(Arg, True)
                            GoTo IL_0683
                        Case CustomEvent.EventType.刷新主页
                            ModBase.RunInUi(Sub()
                                ModMain.FrmLaunchRight.ForceRefresh()
                                If String.IsNullOrEmpty(Arg) Then
                                    ModMain.Hint("已刷新！", ModMain.HintType.Green, True)
                                End If
                            End Sub, False)
                            GoTo IL_0683
                        Case CustomEvent.EventType.刷新页面
                            If Not(TypeOf ModMain.FrmMain.PageRight Is IRefreshable) Then
                                ModMain.Hint("当前页面不支持刷新操作！", ModMain.HintType.Red, True)
                                GoTo IL_0683
                            End If
                            Dim action As Action
                            If CustomEvent._Closure$__.$I13-4 IsNot Nothing Then
                                action = CustomEvent._Closure$__.$I13-4
                            Else
                                Dim action2 As Action = Sub()
                                    CType(ModMain.FrmMain.PageRight, IRefreshable).Refresh()
                                End Sub
                                action = action2
                                CustomEvent._Closure$__.$I13-4 = action2
                            End If
                            ModBase.RunInUiWait(action)
                            If String.IsNullOrEmpty(Arg) Then
                                ModMain.Hint("已刷新！", ModMain.HintType.Green, True)
                                GoTo IL_0683
                            End If
                            GoTo IL_0683
                        Case CustomEvent.EventType.刷新帮助
                            Dim action3 As Action
                            If CustomEvent._Closure$__.$I13-5 IsNot Nothing Then
                                action3 = CustomEvent._Closure$__.$I13-5
                            Else
                                Dim action4 As Action = Sub()
                                    PageOtherLeft.RefreshHelp()
                                End Sub
                                action3 = action4
                                CustomEvent._Closure$__.$I13-5 = action4
                            End If
                            ModBase.RunInUiWait(action3)
                            If String.IsNullOrEmpty(Arg) Then
                                ModMain.Hint("已刷新！", ModMain.HintType.Green, True)
                                GoTo IL_0683
                            End If
                            GoTo IL_0683
                        Case CustomEvent.EventType.今日人品
                            PageOtherTest.Jrrp()
                            GoTo IL_0683
                        Case CustomEvent.EventType.内存优化
                            Dim action5 As Action
                            If CustomEvent._Closure$__.$I13-6 IsNot Nothing Then
                                action5 = CustomEvent._Closure$__.$I13-6
                            Else
                                Dim action6 As Action = Sub()
                                    PageOtherTest.MemoryOptimize(True)
                                End Sub
                                action5 = action6
                                CustomEvent._Closure$__.$I13-6 = action6
                            End If
                            ModBase.RunInThread(action5)
                            GoTo IL_0683
                        Case CustomEvent.EventType.清理垃圾
                            Dim action7 As Action
                            If CustomEvent._Closure$__.$I13-7 IsNot Nothing Then
                                action7 = CustomEvent._Closure$__.$I13-7
                            Else
                                Dim action8 As Action = Sub()
                                    PageOtherTest.RubbishClear()
                                End Sub
                                action7 = action8
                                CustomEvent._Closure$__.$I13-7 = action8
                            End If
                            ModBase.RunInThread(action7)
                            GoTo IL_0683
                        Case CustomEvent.EventType.弹出窗口
                            If Args.Length = 1 Then
                                Throw New Exception(String.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 弹窗标题|弹窗内容", Type))
                            End If
                            ModMain.OurMsgBox(Arg1.BeforeLast("|", False).Replace("\n", vbCrLf), Arg0.Replace("\n", vbCrLf), If((Args.Length > 2), Arg1.AfterLast("|", False), "确定"), "", "", False, True, False, Nothing, Nothing, Nothing)
                            GoTo IL_0683
                        Case CustomEvent.EventType.弹出提示
                            ModMain.Hint(Arg0.Replace("\n", vbCrLf), If((Args.Length = 1), ModMain.HintType.Blue, EnumUtils.FromString(Of ModMain.HintType)(Arg1)), True)
                            GoTo IL_0683
                        Case CustomEvent.EventType.切换页面
                            ModBase.RunInUi(Sub()
                                Dim pageType As FormMain.PageType = EnumUtils.FromString(Of FormMain.PageType)(Arg0)
                                If pageType = FormMain.PageType.InstanceSetup Then
                                    If ModMinecraft.McInstanceSelected Is Nothing Then
                                        ModMain.Hint("请先选择一个 Minecraft 版本！", ModMain.HintType.Red, True)
                                        Return
                                    End If
                                    If ModMinecraft.McInstanceLock.IsLocked(ModMinecraft.McInstanceSelected) Then
                                        ModMain.Hint("该版本正在被修改，暂时无法进入版本设置！", ModMain.HintType.Red, True)
                                        Return
                                    End If
                                    PageInstanceLeft.Instance = ModMinecraft.McInstanceSelected
                                End If
                                ModMain.FrmMain.PageChange(pageType, If((Args.Length = 1), FormMain.PageSubType.[Default], EnumUtils.FromString(Of FormMain.PageSubType)(Arg1)))
                            End Sub, False)
                            GoTo IL_0683
                        Case CustomEvent.EventType.导入整合包, CustomEvent.EventType.安装整合包
                            Dim action9 As Action
                            If CustomEvent._Closure$__.$I13-9 IsNot Nothing Then
                                action9 = CustomEvent._Closure$__.$I13-9
                            Else
                                Dim action10 As Action = Sub()
                                    ModModpack.ModpackInstall()
                                End Sub
                                action9 = action10
                                CustomEvent._Closure$__.$I13-9 = action10
                            End If
                            ModBase.RunInUi(action9, False)
                            GoTo IL_0683
                        Case CustomEvent.EventType.下载文件
                            Arg0 = Arg0.Replace("\", "/")
                            If Not Arg0.StartsWithF("http://", True) AndAlso Not Arg0.StartsWithF("https://", True) Then
                                ModMain.OurMsgBox("EventData 必须为以 http:// 或 https:// 开头的网址。" & vbCrLf & "PCL 不支持其他乱七八糟的下载协议。", "事件执行失败", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                                Return
                            End If
                            If Not CustomEvent.EventSafetyConfirm("即将从该网址下载文件：" & vbCrLf + Arg0) Then
                                Return
                            End If
                            ModBase.RunInUi(Sub()
                                Try
                                    Dim num As Integer = Args.Length
                                    If num <> 1 Then
                                        If num <> 2 Then
                                            PageOtherTest.StartCustomDownload(Arg0, Args(1), Args(2))
                                        Else
                                            PageOtherTest.StartCustomDownload(Arg0, Arg1, Nothing)
                                        End If
                                    Else
                                        PageOtherTest.StartCustomDownload(Arg0, PathUtils.GetLastPart(Arg0), Nothing)
                                    End If
                                Catch ex4 As Exception
                                    PageOtherTest.StartCustomDownload(Arg0, "未知", Nothing)
                                End Try
                            End Sub, False)
                            GoTo IL_0683
                        Case CustomEvent.EventType.修改设置, CustomEvent.EventType.写入设置
                            If Args.Length = 1 Then
                                Throw New Exception(String.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 UiLauncherTransparent|400", Type))
                            End If
                            Settings.SetSafe(Arg0, Args(1), ModMinecraft.McInstanceSelected)
                            If Args.Length = 2 Then
                                ModMain.Hint(String.Format("已写入设置：{0} → {1}", Arg0, Args(1)), ModMain.HintType.Green, True)
                                GoTo IL_0683
                            End If
                            GoTo IL_0683
                        Case CustomEvent.EventType.修改变量, CustomEvent.EventType.写入变量
                            If Args.Length = 1 Then
                                Throw New Exception(String.Format("EventType {0} 需要至少 2 个以 | 分割的参数，例如 VariableName|SomeValue", Type))
                            End If
                            Try
                                RegistryUtils.Write(String.Format("HKEY_CURRENT_USER\Software\{0}", "PCL"), String.Format("CustomEvent{0}", Arg0), Args(1))
                                If Args.Length = 2 Then
                                    ModMain.Hint(String.Format("已写入变量：{0} → {1}", Arg0, Args(1)), ModMain.HintType.Green, True)
                                End If
                                GoTo IL_0683
                            Catch ex As Exception
                                Logger.Warn(ex, String.Format("写入变量失败：{0} → {1}", Arg0, Args(1)), LogBehavior.Toast, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
                                GoTo IL_0683
                            End Try
                        Case CustomEvent.EventType.加入房间
                        Case CustomEvent.EventType.检查更新
                            ModSecret.UpdateCheckByButton()
                            GoTo IL_0683
                        Case Else
                            ModMain.OurMsgBox("未知的事件类型：" + Conversions.ToString(CInt(Type)) + vbCrLf & "请检查事件类型填写是否正确，或者 PCL 是否为最新版本。", "事件执行失败", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                            GoTo IL_0683
                    End Select
                    ModBase.RunInUi(Sub()
                        If String.IsNullOrWhiteSpace(Arg) Then
                            PageLinkMain.Join()
                            Return
                        End If
                        If CustomEvent.EventSafetyConfirm("即将加入第三方联机房间。") Then
                            PageLinkMain.Join(Arg)
                        End If
                    End Sub, False)
                    IL_0683:
                Catch ex2 As Exception
                    Logger.[Error](ex2, String.Format("事件执行失败（{0}, {1}）", Type, Arg), LogBehavior.Alert, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
                End Try
            End If
        End Sub

        Public Shared Function GetAbsoluteUrls(RelativeUrl As String, Type As CustomEvent.EventType) As String()
            If RelativeUrl.StartsWithF("http", True) Then
                If ModBase.RunInUi() Then
                    Throw New Exception("能打开联网帮助页面的 OurListItem 必须手动设置 Title、Info 属性！")
                End If
                Dim lastPart As String
                Try
                    lastPart = PathUtils.GetLastPart(RelativeUrl)
                    If Operators.CompareString(PathUtils.GetExtension(lastPart), "json", False) <> 0 Then
                        Throw New Exception("未指向 .json 后缀的文件")
                    End If
                Catch ex As Exception
                    Throw New Exception("联网帮助页面须指向一个帮助 JSON 文件，并在同路径下包含相应 XAML 文件！" & vbCrLf & "例如：" & vbCrLf & " - https://www.baidu.com/test.json（填写这个路径）" & vbCrLf & " - https://www.baidu.com/test.xaml（同时也需要包含这个文件）", ex)
                End Try
                Dim text As String = ModMain.RequestTaskTempFolder(False) + lastPart
                Logger.Info(String.Format("转换网络资源：{0} -> {1}", RelativeUrl, text), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
                Try
                    ModNet.NetDownloadByClient(RelativeUrl, text, False)
                    ModNet.NetDownloadByClient(RelativeUrl.Replace(".json", ".xaml"), text.Replace(".json", ".xaml"), False)
                Catch ex2 As Exception
                    Throw New Exception("下载指定的文件失败！" & vbCrLf & "注意，联网帮助页面须指向一个帮助 JSON 文件，并在同路径下包含相应 XAML 文件！" & vbCrLf & "例如：" & vbCrLf & " - https://www.baidu.com/test.json（填写这个路径）" & vbCrLf & " - https://www.baidu.com/test.xaml（同时也需要包含这个文件）", ex2)
                End Try
                RelativeUrl = text
            End If
            RelativeUrl = RelativeUrl.Replace("/", "\").Lower().TrimStart(New Char() { "\"c })
            Dim text2 As String = Paths.Base + "PCL"
            ModMain.HelpTryExtract()
            Dim text3 As String
            If RelativeUrl.Contains(":\") Then
                text3 = RelativeUrl
                Logger.Info(String.Format("自定义事件中由绝对路径{0}：{1}", Type, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
            ElseIf FileUtils.Exists(Paths.Base + "PCL\" + RelativeUrl) Then
                text3 = Paths.Base + "PCL\" + RelativeUrl
                Logger.Info(String.Format("自定义事件中由相对 PCL 文件夹的路径{0}：{1}", Type, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
            ElseIf FileUtils.Exists(Paths.Base + "PCL\Help\" + RelativeUrl) Then
                text3 = Paths.Base + "PCL\Help\" + RelativeUrl
                text2 = Paths.Base + "PCL\Help\"
                Logger.Info(String.Format("自定义事件中由相对 PCL 本地帮助文件夹的路径{0}：{1}", Type, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
            ElseIf Type = CustomEvent.EventType.打开帮助 AndAlso FileUtils.Exists(ModBase.PathTemp + "Help\" + RelativeUrl) Then
                text3 = ModBase.PathTemp + "Help\" + RelativeUrl
                text2 = ModBase.PathTemp + "Help\"
                Logger.Info(String.Format("自定义事件中由相对 PCL 自带帮助文件夹的路径{0}：{1}", Type, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
            Else
                If Type <> CustomEvent.EventType.打开文件 AndAlso Type <> CustomEvent.EventType.执行命令 Then
                    Throw New FileNotFoundException("未找到 EventData 指向的本地 xaml 文件：" + RelativeUrl, RelativeUrl)
                End If
                text3 = RelativeUrl
                Logger.Info(String.Format("自定义事件中直接{0}：{1}", Type, text3), LogBehavior.None, "E:\Projects\PCL\本体代码\OpenPCL\Modules\ModEvent.vb")
            End If
            Return New String() { text3, text2 }
        End Function

        Private Shared Function EventSafetyConfirm(Message As String) As Boolean
            Dim flag As Boolean
            If Settings.[Get](Of Boolean)("HintCustomCommand", Nothing) Then
                flag = True
            Else
                Dim num As Integer = ModMain.OurMsgBox(Message + vbCrLf & "请在确认没有安全隐患后再继续。", "执行确认", "继续", "继续且今后不再要求确认", "取消", False, True, False, Nothing, Nothing, Nothing)
                If num <> 1 Then
                    If num <> 2 Then
                        flag = False
                    Else
                        Settings.[Set]("HintCustomCommand", True, Nothing)
                        flag = True
                    End If
                Else
                    flag = True
                End If
            End If
            Return flag
        End Function

        Public Shared TypeProperty As DependencyProperty = DependencyProperty.Register("Type", GetType(CustomEvent.EventType), GetType(CustomEvent), New PropertyMetadata(CustomEvent.EventType.None))

        Public Shared DataProperty As DependencyProperty = DependencyProperty.Register("Data", GetType(String), GetType(CustomEvent), New PropertyMetadata(Nothing))

        Public Enum EventType
            None
            打开网页
            打开文件
            打开帮助
            执行命令
            启动游戏
            复制文本
            刷新主页
            刷新页面
            刷新帮助
            今日人品
            内存优化
            清理垃圾
            弹出窗口
            弹出提示
            切换页面
            导入整合包
            安装整合包
            下载文件
            修改设置
            写入设置
            修改变量
            写入变量
            加入房间
            检查更新
        End Enum
    End Class
End Namespace
