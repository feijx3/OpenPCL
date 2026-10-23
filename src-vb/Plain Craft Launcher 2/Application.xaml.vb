Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports MeloongCore.Wpf
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CompilerServices
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports PCL.My
Imports PCL.My.Resources

Namespace PCL
    Public Partial Class Application
        Inherits Application

        Private Sub Application_Startup(sender As Object, e As Global.System.Windows.StartupEventArgs)
            ' The following expression was wrapped in a checked-statement
            Try
                Global.MeloongCore.Main.Init("PCL")
                Global.MeloongCore.Wpf.Main.Init()
                If Not e.Args.Contains("--update") AndAlso Not e.Args.Contains("--gpu") AndAlso Not e.Args.Contains("--memory") Then
                    Application.WaitingMutex(e)
                    Logger.Instance = New PclLogger() With { .logFolder = Paths.BaseThenName, .MinLevel = If(ModBase.ModeDebug, LogLevel.Trace, LogLevel.Info) }
                End If
                Thread.CurrentThread.Priority = ThreadPriority.Highest
                Try
                    If Conversions.ToInteger(RegistryUtils.TryRead("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", "Release", 999999)) < 528040 Then
                        Application.OldEnvironmentAssert(Nothing)
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, "检查 .NET Framework 版本失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                End Try
                Try
                    New FormattedText("", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Fonts.SystemTypefaces.First(), 96.0, New ModBase.MyColor(), CDbl(ModBase.DPI))
                Catch ex2 As UriFormatException
                    Environment.SetEnvironmentVariable("windir", Environment.GetEnvironmentVariable("SystemRoot"), EnvironmentVariableTarget.User)
                    New FormattedText("", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Fonts.SystemTypefaces.First(), 96.0, New ModBase.MyColor(), CDbl(ModBase.DPI))
                End Try
                If e.Args.Length > 0 Then
                    If Operators.CompareString(e.Args(0), "--update", False) = 0 Then
                        ModSecret.UpdateReplace(Conversions.ToInteger(e.Args(1)), e.Args(2).Trim(New Char() { """"c }), e.Args(3).Trim(New Char() { """"c }), Conversions.ToBoolean(e.Args(4)))
                        Environment.[Exit](5)
                    Else
                        If Operators.CompareString(e.Args(0), "--gpu", False) = 0 Then
                            Try
                                ModMain.SetGPUPreference(e.Args(1).Trim(New Char() { """"c }))
                                Environment.[Exit](5)
                                GoTo IL_02C6
                            Catch ex3 As Exception
                                Environment.[Exit](1)
                                GoTo IL_02C6
                            End Try
                        End If
                        If Operators.CompareString(e.Args(0), "--memory", False) = 0 Then
                            Dim availablePhysicalMemory As ULong = MyWpfExtension.Computer.Info.AvailablePhysicalMemory
                            Try
                                PageOtherTest.MemoryOptimizeInternal(False)
                            Catch ex4 As Exception
                                Interaction.MsgBox(ex4.Message, MsgBoxStyle.Critical, "内存优化失败")
                                Environment.[Exit](-1)
                            End Try
                            If MyWpfExtension.Computer.Info.AvailablePhysicalMemory < availablePhysicalMemory Then
                                Environment.[Exit](0)
                            Else
                                Environment.[Exit](CInt(Math.Round((MyWpfExtension.Computer.Info.AvailablePhysicalMemory - availablePhysicalMemory) / 1024.0)))
                            End If
                        End If
                    End If
                End If
                IL_02C6:
                Try
                    DirectoryUtils.Create(Paths.Base + "PCL\Pictures\")
                    DirectoryUtils.Create(Paths.Base + "PCL\Musics\")
                    ModBase.CheckPermissionWithException(Paths.Base + "PCL\")
                Catch ex5 As Exception
                    Dim text As String = String.Format("PCL 没有对当前文件夹的权限（{0}PCL\），请尝试：", Paths.Base)
                    Dim text2 As String = vbCrLf & "1. 将 PCL 移动到其他文件夹"
                    Dim diskName As Char? = PathUtils.GetDiskName(Paths.Base)
                    Interaction.MsgBox(text + text2 + If(If((diskName IsNot Nothing), New Boolean?(diskName.GetValueOrDefault() = "C"c), Nothing).GetValueOrDefault(), "，例如 C 盘和桌面以外的其他位置。", "。") + vbCrLf & "2. 删除当前目录中的 PCL 文件夹，然后再试。" & vbCrLf & "3. 右键 PCL 选择属性，打开 兼容性 中的 以管理员身份运行此程序。", MsgBoxStyle.Critical, "运行环境错误")
                    Environment.[Exit](4)
                End Try
                While True
                    Try
                        DirectoryUtils.Create(ModBase.PathTemp)
                        ModBase.CheckPermissionWithException(ModBase.PathTemp)
                    Catch ex6 As Exception
                        If Not PathUtils.IsSame(ModBase.PathTemp, Path.GetTempPath() + "PCL\") Then
                            ModMain.MyMsgBox("手动设置的缓存文件夹不可用，PCL 将使用默认缓存文件夹。" & vbCrLf & vbCrLf & "错误原因：" + ex6.GetDisplay(True), "缓存文件夹不可用", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                            Settings.[Set]("SystemSystemCache", "", Nothing)
                            ModBase.PathTemp = Path.GetTempPath() + "PCL\"
                            Continue For
                        End If
                        ModMain.MyMsgBox("PCL 无法访问缓存文件夹，可能导致程序出错或无法正常使用！" & vbCrLf & vbCrLf & "错误原因：" + ex6.GetDisplay(True), "缓存文件夹不可用", "确定", "", "", False, True, False, Nothing, Nothing, Nothing)
                    End Try
                    Exit For
                End While
                DirectoryUtils.Create(ModBase.PathTemp + "Cache\")
                DirectoryUtils.Create(Paths.AppDataThenName)
                ToolTipService.InitialShowDelayProperty.OverrideMetadata(GetType(DependencyObject), New FrameworkPropertyMetadata(100))
                ServicePointManager.Expect100Continue = False
                ServicePointManager.DefaultConnectionLimit = 10000
                ServicePointManager.UseNagleAlgorithm = False
                ServicePointManager.EnableDnsRoundRobin = True
                ServicePointManager.ReusePort = True
                If Environment.OSVersion.Version < New Version(6, 2) Then
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
                End If
                If Settings.[Get](Of Boolean)("UiLauncherLogo", Nothing) Then
                    ModMain.FrmStart = New SplashScreen("Images\icon.ico")
                    ModMain.FrmStart.Show(False, True)
                End If
                Logger.Info(String.Format("程序版本：{0} ({1}{2})", "快照版 2.99.0.0", 414, ""), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                Logger.Info(String.Format("识别码：{0}{1}", ModBase.Identify, If(ModSecret.ThemeCheckOne(9), "，已解锁反馈主题", "，未解锁反馈主题")), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                Logger.Info(String.Format("程序路径：{0}", ModBase.PathExe), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                Logger.Info(String.Format("系统编码：{0} ({1}, GBK={2})", Encoding.[Default].HeaderName, Encoding.[Default].CodePage, ModBase.IsGBKEncoding), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                Logger.Info(String.Format("管理员权限：{0}", WindowsUtils.HasAdminRole()), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                If Paths.Base.Contains(Path.GetTempPath()) OrElse Paths.Base.Contains("AppData\Local\Temp\") Then
                    ModMain.MyMsgBox("请将 PCL 从压缩包中解压后再使用！" & vbCrLf & "如果不会解压，可以在网上寻找教程。", "需要解压！", "我知道了", "", "", True, True, True, Nothing, Nothing, Nothing)
                    FormMain.EndProgramForce(ModBase.ProcessReturnValues.Cancel)
                End If
                If Not Environment.Is64BitOperatingSystem Then
                    ModMain.MyMsgBox("PCL 和新版 Minecraft 均不再支持 32 位系统，请重装为 64 位系统后再进行游戏！", "环境警告", "我知道了", "", "", True, True, True, Nothing, Nothing, Nothing)
                    FormMain.EndProgramForce(ModBase.ProcessReturnValues.Cancel)
                End If
                Logger.Info(String.Format("第一阶段加载用时：{0} ms", ModBase.GetTimeMs() - ModBase.ApplicationStartTick), LogBehavior.None, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                ModBase.ApplicationStartTick = ModBase.GetTimeMs()
                ModAnimation.AniControlEnabled += 1
            Catch ex7 As Exception
                Dim text3 As String = Nothing
                Try
                    text3 = ModBase.PathExe
                Catch ex8 As Exception
                End Try
                Interaction.MsgBox(ex7.GetDisplay(True) + vbCrLf & "PCL 所在路径：" + If(String.IsNullOrEmpty(text3), "获取失败", text3), MsgBoxStyle.Critical, "PCL 初始化错误")
                FormMain.EndProgramForce(ModBase.ProcessReturnValues.Exception)
            End Try
        End Sub

        Private Shared Sub WaitingMutex(e As Global.System.Windows.StartupEventArgs)
            Try
                Dim flag As Boolean
                Application.PclMutex = New Mutex(True, "PCL_SingletonMutex", flag)
                If Not flag Then
                    Logger.Warn("已有一个 PCL 实例正在运行", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                    If e.Args.Length > 0 AndAlso Operators.CompareString(e.Args(0), "--wait", False) = 0 Then
                        Try
                            If Application.PclMutex.WaitOne(10000) Then
                                Return
                            End If
                        Catch ex As AbandonedMutexException
                            Return
                        End Try
                    End If
                    Dim text As String = Nothing
                    Dim text2 As String = "Plain Craft Launcher　"
                    Dim intPtr As IntPtr = ModMain.FindWindow(text, text2)
                    If intPtr = IntPtr.Zero Then
                        text2 = Nothing
                        text = "Plain Craft Launcher 2　"
                        intPtr = ModMain.FindWindow(text2, text)
                    End If
                    If intPtr <> IntPtr.Zero Then
                        ModMain.ShowWindowToTop(intPtr)
                    End If
                    Interaction.Beep()
                    Environment.[Exit](4)
                End If
            Catch ex2 As Exception
                Logger.Warn(ex2, "单例检查失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
            End Try
        End Sub

        Private Sub Application_SessionEnding(sender As Object, e As SessionEndingCancelEventArgs)
            Dim frmMain As FormMain = ModMain.FrmMain
            If frmMain Is Nothing Then
                Return
            End If
            frmMain.EndProgram(False)
        End Sub

        Private Sub Application_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs)
            Dim num As Integer
            Dim num4 As Integer
            Dim obj As Object
            Try
                IL_0000:
                ProjectData.ClearProjectError()
                num = 1
                IL_0007:
                Dim num2 As Integer = 2
                Dim frmMain As FormMain = ModMain.FrmMain
                If If((frmMain IsNot Nothing), frmMain.PageLeft, Nothing) Is Nothing OrElse Not(TypeOf ModMain.FrmMain.PageLeft Is IDispatcherUnhandledException) Then
                    GoTo IL_0057
                End If
                IL_002D:
                num2 = 3
                CType(ModMain.FrmMain.PageLeft, IDispatcherUnhandledException).DispatcherUnhandledException(RuntimeHelpers.GetObjectValue(sender), e)
                IL_004A:
                num2 = 4
                If e.Handled Then
                    GoTo IL_0133
                End If
                IL_0057:
                num2 = 6
                Dim frmMain2 As FormMain = ModMain.FrmMain
                If If((frmMain2 IsNot Nothing), frmMain2.PageRight, Nothing) Is Nothing OrElse Not(TypeOf ModMain.FrmMain.PageRight Is IDispatcherUnhandledException) Then
                    GoTo IL_00A7
                End If
                IL_007D:
                num2 = 7
                CType(ModMain.FrmMain.PageRight, IDispatcherUnhandledException).DispatcherUnhandledException(RuntimeHelpers.GetObjectValue(sender), e)
                IL_009A:
                num2 = 8
                If e.Handled Then
                    GoTo IL_0133
                End If
                IL_00A7:
                num2 = 10
                e.Handled = True
                IL_00B1:
                num2 = 11
                If ModBase.IsProgramEnding Then
                    GoTo IL_0133
                End If
                IL_00BB:
                num2 = 13
                ModMain.FeedbackInfo()
                IL_00C3:
                num2 = 14
                Dim display As String = e.Exception.GetDisplay(True)
                IL_00D3:
                num2 = 15
                If Not display.Contains("System.Windows.Threading.Dispatcher.Invoke") AndAlso Not display.Contains("MS.Internal.AppModel.ITaskbarList.HrInit") AndAlso Not display.Contains("未能加载文件或程序集") AndAlso Not display.Contains(".NET Framework") Then
                    GoTo IL_011A
                End If
                IL_010A:
                num2 = 16
                Application.OldEnvironmentAssert(e.Exception)
                GoTo IL_0133
                IL_011A:
                num2 = 18
                Logger.[Error](e.Exception, "程序出现未知错误", LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                IL_0133:
                GoTo IL_01D1
                IL_0138:
                Dim num3 As Integer = num4 + 1
                num4 = 0
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3)
                IL_0192:
                GoTo IL_01C6
                IL_0194:
                num4 = num2
                switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num)
                IL_01A4:
            Catch obj2 When endfilter((TypeOf obj Is Exception) And (num <> 0) And (num4 = 0))
                Dim ex As Exception = CType(obj2, Exception)
                GoTo IL_0194
            End Try
            IL_01C6:
            Throw ProjectData.CreateProjectError(-2146828237)
            IL_01D1:
            If num4 <> 0 Then
                ProjectData.ClearProjectError()
            End If
        End Sub

        Private Shared Sub OldEnvironmentAssert(Optional ex As Exception = Nothing)
            ModBase.OpenWebsite("https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/thank-you/net48-offline-installer")
            If ex Is Nothing Then
                Logger.[Error](String.Format("你的 .NET Framework 版本过低或损坏，请下载并重新安装 .NET Framework 4.8！{0}若安装失败，可以先卸载高版本的 .NET Framework 然后再试。", vbCrLf), LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
                Return
            End If
            Logger.[Error](ex, String.Format("你的 .NET Framework 版本过低或损坏，请下载并重新安装 .NET Framework 4.8！{0}若安装失败，可以先卸载高版本的 .NET Framework 然后再试。", vbCrLf), LogBehavior.AlertThenCrash, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
        End Sub

        Private Sub New()
            AddHandler MyBase.Startup, AddressOf Me.Application_Startup
            AddHandler MyBase.SessionEnding, AddressOf Me.Application_SessionEnding
            AddHandler MyBase.DispatcherUnhandledException, AddressOf Me.Application_DispatcherUnhandledException
            If Me.$STATIC$.ctor$2001$Prefixes$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$.ctor$2001$Prefixes$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$.ctor$2001$Prefixes$Init, flag)
                If Me.$STATIC$.ctor$2001$Prefixes$Init.State = 0S Then
                    Me.$STATIC$.ctor$2001$Prefixes$Init.State = 2S
                    Me.$STATIC$.ctor$2001$Prefixes = New String() { "NAudio", "Newtonsoft.Json", "Ookii.Dialogs.Wpf", "Imazen.WebP", "CacheCow.Common", "CacheCow.Client.FileStore", "CacheCow.Client", "ThrottleDebounce", "System.Net.Http.Formatting", "Microsoft.Win32.Registry", "PCLCS", "MeloongCore.Wpf", "MeloongCore" }
                ElseIf Me.$STATIC$.ctor$2001$Prefixes$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$.ctor$2001$Prefixes$Init.State = 1S
                If flag Then
                    Monitor.[Exit](Me.$STATIC$.ctor$2001$Prefixes$Init)
                End If
            End Try
            If Me.$STATIC$.ctor$2001$LoadedAssemblies$Init Is Nothing Then
                Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$.ctor$2001$LoadedAssemblies$Init, New StaticLocalInitFlag(), Nothing)
            End If
            Dim flag2 As Boolean = False
            Try
                Monitor.Enter(Me.$STATIC$.ctor$2001$LoadedAssemblies$Init, flag2)
                If Me.$STATIC$.ctor$2001$LoadedAssemblies$Init.State = 0S Then
                    Me.$STATIC$.ctor$2001$LoadedAssemblies$Init.State = 2S
                    Me.$STATIC$.ctor$2001$LoadedAssemblies = New ConcurrentDictionary(Of String, Lazy(Of Assembly))(StringComparer.Ordinal)
                ElseIf Me.$STATIC$.ctor$2001$LoadedAssemblies$Init.State = 2S Then
                    Throw New IncompleteInitialization()
                End If
            Finally
                Me.$STATIC$.ctor$2001$LoadedAssemblies$Init.State = 1S
                If flag2 Then
                    Monitor.[Exit](Me.$STATIC$.ctor$2001$LoadedAssemblies$Init)
                End If
            End Try
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, Function(sender As Object, Args As ResolveEventArgs)
                Dim text As String = Me.$STATIC$.ctor$2001$Prefixes.FirstOrDefault(Function(p As String) Args.Name.StartsWith(p, StringComparison.Ordinal))
                Dim assembly As Assembly
                If text Is Nothing Then
                    assembly = Nothing
                Else
                    Dim concurrentDictionary As ConcurrentDictionary(Of String, Lazy(Of Assembly)) = Me.$STATIC$.ctor$2001$LoadedAssemblies
                    Dim text2 As String = text
                    Dim func As Func(Of String, Lazy(Of Assembly))
                    If Application._Closure$__.$I6-2 IsNot Nothing Then
                        func = Application._Closure$__.$I6-2
                    Else
                        Dim func2 As Func(Of String, Lazy(Of Assembly)) = Function(p As String) New Lazy(Of Assembly)(Function()
                            Dim assembly2 As Assembly = Assembly.Load(CType(PCL.My.Resources.Resources.ResourceManager.GetObject(p), Byte()))
                            If Operators.CompareString(p, "Imazen.WebP", False) = 0 Then
                                Application.ExtractLibwebp()
                            End If
                            Return assembly2
                        End Function, LazyThreadSafetyMode.ExecutionAndPublication)
                        func = func2
                        Application._Closure$__.$I6-2 = func2
                    End If
                    assembly = concurrentDictionary.GetOrAdd(text2, func).Value
                End If
                Return assembly
            End Function
        End Sub

        Private Shared Sub ExtractLibwebp()
            Dim text As String = PathUtils.RemoveSlashSuffix(ModBase.PathPure.Value)
            Application.SetDllDirectory(text)
            Try
                ModBase.ExtractResources(ModBase.PathPure.Value + "libwebp.dll", "libwebp64")
            Catch ex As Exception
                Logger.Warn(ex, "写入 libwebp.dll 失败", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Application.xaml.vb")
            End Try
        End Sub

        Private Declare Ansi Function SetDllDirectory Lib "kernel32" Alias "SetDllDirectoryA" (<MarshalAs(UnmanagedType.VBByRefStr)> ByRef lpPathName As String) As Boolean

        Private Sub MyIconButton_Click(sender As Object, e As EventArgs)
            Dim mcLoginType As ModLaunch.McLoginType = Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing)
            If mcLoginType = ModLaunch.McLoginType.Legacy Then
                Dim list As List(Of String) = New List(Of String)()
                list.AddRange(Settings.[Get](Of String)("LoginLegacyName", Nothing).ToString().Split("¨", False))
                list.Remove(Conversions.ToString(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)))
                Settings.[Set]("LoginLegacyName", list.Join("¨"c), Nothing)
                ModMain.FrmLoginLegacy.ComboName.ItemsSource = list
                ModMain.FrmLoginLegacy.ComboName.Text = If(list.Any(), list(0), "")
                Return
            End If
            If mcLoginType = ModLaunch.McLoginType.Ms Then
                Dim jobject As JObject = CType(Settings.[Get](Of String)("LoginMsJson", Nothing).DeserializeJson(), JObject)
                jobject.Remove(Conversions.ToString(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)))
                Settings.[Set]("LoginMsJson", jobject.ToString(0, New JsonConverter(-1) {}), Nothing)
                If ModMain.FrmLoginMs.ComboAccounts.SelectedItem Is NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing) Then
                    ModMain.FrmLoginMs.ComboAccounts.SelectedIndex = 0
                End If
                ModMain.FrmLoginMs.ComboAccounts.Items.Remove(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, Nothing, "Parent", New Object(-1) {}, Nothing, Nothing, Nothing)))
                Return
            End If
            Dim text As String = Settings.[Get](Of ModLaunch.McLoginType)("LoginType", Nothing).ToString()
            Dim dictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Dim list2 As List(Of String) = New List(Of String)()
            Dim list3 As List(Of String) = New List(Of String)()
            If Operators.CompareString(Settings.[Get](Of String)("Login" + text + "Email", Nothing), "", False) <> 0 Then
                list2.AddRange(Settings.[Get](Of String)("Login" + text + "Email", Nothing).Split("¨", False))
            End If
            If Operators.CompareString(Settings.[Get](Of String)("Login" + text + "Pass", Nothing), "", False) <> 0 Then
                list3.AddRange(Settings.[Get](Of String)("Login" + text + "Pass", Nothing).Split("¨", False))
            End If
            Dim num As Integer = list2.Count - 1
            For i As Integer = 0 To num
                dictionary.Add(list2(i), list3(i))
            Next
            dictionary.Remove(Conversions.ToString(NewLateBinding.LateGet(sender, Nothing, "Tag", New Object(-1) {}, Nothing, Nothing, Nothing)))
            Settings.[Set]("Login" + text + "Email", dictionary.Keys.Join("¨"c), Nothing)
            Settings.[Set]("Login" + text + "Pass", dictionary.Values.Join("¨"c), Nothing)
            If Operators.CompareString(text, "Nide", False) = 0 Then
                ModMain.FrmLoginNide.ComboName.ItemsSource = dictionary.Keys
                ModMain.FrmLoginNide.ComboName.Text = If(dictionary.Keys.Any(), dictionary.Keys.ElementAtOrDefault(0), "")
                ModMain.FrmLoginNide.TextPass.Password = If(dictionary.Values.Any(), dictionary.Values.ElementAtOrDefault(0), "")
                Return
            End If
            If Operators.CompareString(text, "Auth", False) <> 0 Then
                Return
            End If
            ModMain.FrmLoginAuth.ComboName.ItemsSource = dictionary.Keys
            ModMain.FrmLoginAuth.ComboName.Text = If(dictionary.Keys.Any(), dictionary.Keys.ElementAtOrDefault(0), "")
            ModMain.FrmLoginAuth.TextPass.Password = If(dictionary.Values.Any(), dictionary.Values.ElementAtOrDefault(0), "")
        End Sub

        Friend ReadOnly Property Info As AssemblyInfo
            <DebuggerHidden()>
            Get
                Return New AssemblyInfo(Assembly.GetExecutingAssembly())
            End Get
        End Property

        Private Shared PclMutex As Mutex

        Private $STATIC$.ctor$2001$Prefixes As String()

        Private $STATIC$.ctor$2001$Prefixes$Init As StaticLocalInitFlag

        Private $STATIC$.ctor$2001$LoadedAssemblies As ConcurrentDictionary(Of String, Lazy(Of Assembly))

        Private $STATIC$.ctor$2001$LoadedAssemblies$Init As StaticLocalInitFlag
    End Class
End Namespace
