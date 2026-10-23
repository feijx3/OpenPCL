using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PCL.My;

namespace PCL
{
    [DesignerGenerated]
    public partial class FormMain : Window
    {
        private void ShowUpdateLog(int LastVersion)
        {
            FormMain._Closure$__1-0 CS$<>8__locals1 = new FormMain._Closure$__1-0(CS$<>8__locals1);
            int num = 0;
            int num2 = 0;
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            if (LastVersion < 414)
            {
                list.Add(new KeyValuePair<int, string>(5, "删除：为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……"));
            }
            checked
            {
                if (LastVersion < 412)
                {
                    list.Add(new KeyValuePair<int, string>(4, "新增：允许修改、添加、移除、更新已安装版本的各个组件，可从版本设置中进入"));
                    list.Add(new KeyValuePair<int, string>(2, "优化：启动游戏时允许按 Esc 取消"));
                    list.Add(new KeyValuePair<int, string>(2, "优化：允许复制主页的启动日志中的内容"));
                    num += 19;
                    num2 += 26;
                }
                if (LastVersion < 410)
                {
                    if (LastVersion == 409)
                    {
                        list.Add(new KeyValuePair<int, string>(1, "修复：更新 Mod 时可能更新成另一个 Mod 加载器的版本"));
                    }
                    num += 2;
                    num2 += 6;
                }
                if (LastVersion < 409)
                {
                    list.Add(new KeyValuePair<int, string>(2, "优化：版本设置中，添加打开截图文件夹的快捷方式"));
                    list.Add(new KeyValuePair<int, string>(2, "优化：导出整合包时，可以勾选导出光影设置文件"));
                    num += 21;
                    num2 += 22;
                }
                if (LastVersion < 407)
                {
                    if (LastVersion == 405)
                    {
                        list.Add(new KeyValuePair<int, string>(1, "修复：使用部分主页预设时崩溃"));
                        list.Add(new KeyValuePair<int, string>(1, "修复：无法删除部分文件夹"));
                        list.Add(new KeyValuePair<int, string>(1, "修复：版本设置中指定的 Java 无法被保存"));
                        num++;
                        num2 += 3;
                    }
                    num += 2;
                }
                if (LastVersion < 405)
                {
                    list.Add(new KeyValuePair<int, string>(4, "新增：重做 Java 管理与相关设置，允许调整 Java 优先级、指定 Java 版本范围等"));
                    list.Add(new KeyValuePair<int, string>(3, "优化：导出整合包时允许自动导出版本文件夹中的 Java"));
                    list.Add(new KeyValuePair<int, string>(3, "优化：重做弹出提示的样式以及动画，以更符合现代 UI 审美"));
                    list.Add(new KeyValuePair<int, string>(1, "修复：可能无法安装 OptiFine 26.1.2"));
                    num += 35;
                    num2 += 17;
                }
                if (LastVersion < 403)
                {
                    list.Add(new KeyValuePair<int, string>(1, "修复：Windows 7 无法正常联网"));
                    num++;
                    num2 += 2;
                }
                if (LastVersion < 401)
                {
                    list.Add(new KeyValuePair<int, string>(1, "修复：可能无法更换披风"));
                    num += 8;
                    num2 += 6;
                }
                if (LastVersion < 399)
                {
                    list.Add(new KeyValuePair<int, string>(2, "优化：升级框架到 .NET Framework 4.8"));
                    num += 28;
                    num2 += 19;
                }
                if (LastVersion < 397 && LastVersion >= 393 && LastVersion < 397)
                {
                    list.Add(new KeyValuePair<int, string>(1, "修复：无法使用自定义隐藏主题"));
                }
                if (LastVersion < 395 && LastVersion == 393)
                {
                    list.Add(new KeyValuePair<int, string>(2, "修复：部分个性化功能失效"));
                }
                if (LastVersion < 393)
                {
                    list.Add(new KeyValuePair<int, string>(2, "优化：允许永久关闭启动页面的快照版提示"));
                    list.Add(new KeyValuePair<int, string>(2, "优化：下载游戏、整合包的稳定性"));
                    num++;
                    num2 += 5;
                }
                if (LastVersion < 390)
                {
                    list.Add(new KeyValuePair<int, string>(1, "修复：会为部分 Forge / NeoForge 选择不支持的 Java"));
                    num2 += 4;
                }
                if (LastVersion < 388)
                {
                    if (LastVersion == 386)
                    {
                        list.Add(new KeyValuePair<int, string>(2, "修复：无法使用 Java 25+ 启动 Forge"));
                    }
                    num += 4;
                    num2 += 3;
                }
                if (LastVersion < 386)
                {
                    list.Add(new KeyValuePair<int, string>(2, "修复：Minecraft 使用 Java 25+ 时的一个性能问题"));
                    if (LastVersion == 384 || LastVersion == 385)
                    {
                        list.Add(new KeyValuePair<int, string>(2, "修复：无法正常重命名游戏版本"));
                    }
                    if (LastVersion == 385)
                    {
                        list.Add(new KeyValuePair<int, string>(1, "修复：无法显示 Modrinth 整合包的版本列表"));
                    }
                    num2++;
                }
                if (LastVersion < 385)
                {
                    list.Add(new KeyValuePair<int, string>(2, "修复：无法下载 NeoForge 26.1"));
                    if (LastVersion == 384)
                    {
                        list.Add(new KeyValuePair<int, string>(2, "修复：下载可能失败，提示任务管理刷新线程出错"));
                        list.Add(new KeyValuePair<int, string>(2, "修复：无法保存选择的 Minecraft 文件夹"));
                        list.Add(new KeyValuePair<int, string>(1, "修复：使用中文搜索 Mod 时，部分结果会忽略筛选条件"));
                    }
                    num += 3;
                    num2 += 6;
                }
                if (LastVersion < 384)
                {
                    list.Add(new KeyValuePair<int, string>(3, "优化：降低 Minecraft 的内存占用"));
                    list.Add(new KeyValuePair<int, string>(2, "修复：部分关键词搜不到资源，特别是中文 Mod 搜索经常没有结果"));
                    list.Add(new KeyValuePair<int, string>(1, "修复：老版本 Windows 无法启动游戏"));
                    list.Add(new KeyValuePair<int, string>(1, "修复：连接 Mojang 的服务可能失败，提示错误码 421"));
                    list.Add(new KeyValuePair<int, string>(1, "修复：无法访问爱发电"));
                    num += 21;
                    num2 += 14;
                }
                if (LastVersion < 382)
                {
                    list.Add(new KeyValuePair<int, string>(2, "修复：下载可能完全卡住，或是下载进度反复回退"));
                    list.Add(new KeyValuePair<int, string>(1, "新增：内存管理设置，可以选择 G1GC、ZGC 或分代 ZGC"));
                    list.Add(new KeyValuePair<int, string>(1, "优化：来自 Modrinth 的文件下载速度"));
                    num += 15;
                    num2 += 16;
                }
                if (LastVersion < 380)
                {
                    num += 3;
                    num2 += 13;
                }
                if (LastVersion < 378 && LastVersion >= 377)
                {
                    list.Add(new KeyValuePair<int, string>(1, "修复：在版本列表中存在 OptiFine 时可能无法加载版本列表"));
                    num2 += 2;
                }
                if (LastVersion < 377)
                {
                    if (LastVersion >= 373)
                    {
                        list.Add(new KeyValuePair<int, string>(5, "删除：暂时隐藏联机入口……不过只是暂时关闭，它还会回来的！"));
                    }
                    list.Add(new KeyValuePair<int, string>(3, "新增：适配新的 Minecraft 版本号系统与 Unobfuscated 版本"));
                    list.Add(new KeyValuePair<int, string>(2, "优化：巨幅优化各个下载页面和 Mod 管理页面的性能"));
                    list.Add(new KeyValuePair<int, string>(1, "优化：更换披风时会显示当前使用的披风"));
                    num += 33;
                    num2 += 29;
                }
                if (LastVersion < 375)
                {
                    num += 16;
                    num2 += 4;
                }
                if (LastVersion < 374)
                {
                    num += 9;
                    num2 += 7;
                }
                if (LastVersion < 373)
                {
                    num += 7;
                    num2 += 10;
                }
                List<string> list2 = new List<string>();
                List<KeyValuePair<int, string>> list3 = list.OrderByDescending<KeyValuePair<int, string>, int>((FormMain._Closure$__.$I1-0 == null) ? (FormMain._Closure$__.$I1-0 = (KeyValuePair<int, string> f) => f.Key) : FormMain._Closure$__.$I1-0).ToList<KeyValuePair<int, string>>();
                if (!list3.Any<KeyValuePair<int, string>>() && num == 0 && num2 == 0)
                {
                    list2.Add("没有更新日志……");
                }
                int num3 = Math.Min(9, list3.Count - 1);
                for (int i = 0; i <= num3; i++)
                {
                    list2.Add(list3[i].Value);
                }
                if (list3.Count > 10)
                {
                    num += list3.Count - 10;
                }
                if (num > 0 || num2 > 0)
                {
                    list2.Add(((num > 0) ? (Conversions.ToString(num) + " 项小调整与修改") : "") + ((num > 0 && num2 > 0) ? "，" : "") + ((num2 > 0) ? ("修复了 " + Conversions.ToString(num2) + " 个 Bug") : "") + "，详见完整更新日志");
                }
                CS$<>8__locals1.$VB$Local_Content = "· " + list2.Join("\r\n· ");
                ModBase.RunInNewThread(delegate
                {
                    if (ModMain.MyMsgBox(CS$<>8__locals1.$VB$Local_Content, "PCL 已更新至快照版 2.99.0.0", "确定", "完整更新日志", "", false, true, false, null, null, null) == 2)
                    {
                        ModBase.OpenWebsite("https://meloong.com/afd/a/LTCat?tab=feed");
                    }
                }, "UpdateLog Output", ThreadPriority.Normal);
            }
        }

        public FormMain()
        {
            base.Loaded += this.FormMain_Loaded;
            base.Closing += this.FormMain_Closing;
            base.SizeChanged += delegate(object a0, SizeChangedEventArgs a1)
            {
                this.FormMain_SizeChanged();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.FormMain_SizeChanged();
            };
            base.KeyDown += this.FormMain_KeyDown;
            base.MouseDown += this.FormMain_MouseDown;
            base.Activated += delegate(object a0, EventArgs a1)
            {
                this.FormMain_Activated();
            };
            base.Drop += this.FrmMain_Drop;
            base.MouseMove += this.FormMain_MouseMove;
            this.IsWindowLoadFinished = false;
            this.IsSizeSaveable = false;
            this.IsSystemTimeChanged = false;
            this._Hidden = false;
            this.PageCurrent = FormMain.PageType.Launch;
            this.PageLast = FormMain.PageType.Launch;
            this.PageStack = new List<FormMain.PageStackData>();
            this.IsChangingPage = false;
            this.lastMouseArg = null;
            ModBase.ApplicationStartTick = ModBase.GetTimeMs();
            ModMain.FrmMain = this;
            ModMain.FrmLaunchLeft = new PageLaunchLeft();
            ModMain.FrmLaunchRight = new PageLaunchRight();
            int num = Settings.Get<int>("SystemLastVersionReg", null);
            if (num < 414)
            {
                this.UpgradeSub(num);
            }
            else if (num > 414)
            {
                this.DowngradeSub(num);
            }
            if (!Settings.HasSaved("LaunchArgumentIndieV2", null))
            {
                if (Settings.HasSaved("LaunchArgumentIndie", null))
                {
                    Logger.Info("从老 PCL 迁移版本隔离", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    Settings.Set("LaunchArgumentIndieV2", Settings.Get<int>("LaunchArgumentIndie", null), null);
                }
                else if (ModBase.HasIniKey("Setup", "LaunchVersionSelect"))
                {
                    Logger.Info("从老 PCL 升级，但此前未调整版本隔离，使用老的版本隔离默认值", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    Settings.Set("LaunchArgumentIndieV2", Settings.GetDefault("LaunchArgumentIndie"), null);
                }
                else
                {
                    Logger.Info("全新的 PCL，使用新的版本隔离默认值", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    Settings.Set("LaunchArgumentIndieV2", Settings.GetDefault("LaunchArgumentIndieV2"), null);
                }
            }
            ModSecret.ThemeCheckAll(false);
            ModSecret.ThemeRefresh(Settings.Get<int>("UiLauncherTheme", null));
            base.AddHandler(DragDrop.DragEnterEvent, new DragEventHandler(this.HandleDrag), true);
            base.AddHandler(DragDrop.DragOverEvent, new DragEventHandler(this.HandleDrag), true);
            this.InitializeComponent();
            base.Opacity = 0.0;
            if (WindowsUtils.HasAdminRole())
            {
                if (this.$STATIC$.ctor$2001$Helper$Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$.ctor$2001$Helper$Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this.$STATIC$.ctor$2001$Helper$Init, ref flag);
                    if (this.$STATIC$.ctor$2001$Helper$Init.State == 0)
                    {
                        this.$STATIC$.ctor$2001$Helper$Init.State = 2;
                        this.$STATIC$.ctor$2001$Helper = new DragHelper();
                    }
                    else if (this.$STATIC$.ctor$2001$Helper$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this.$STATIC$.ctor$2001$Helper$Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this.$STATIC$.ctor$2001$Helper$Init);
                    }
                }
                base.SourceInitialized += delegate(object a0, EventArgs a1)
                {
                    this._Lambda$__3-0();
                };
                base.Closing += delegate(object a0, CancelEventArgs a1)
                {
                    this._Lambda$__3-1();
                };
                this.$STATIC$.ctor$2001$Helper.DragDrop += delegate(object a0, EventArgs a1)
                {
                    this._Lambda$__3-2();
                };
            }
            if (!Information.IsNothing(ModMain.FrmLaunchLeft.Parent))
            {
                ModMain.FrmLaunchLeft.SetValue(ContentPresenter.ContentProperty, null);
            }
            if (!Information.IsNothing(ModMain.FrmLaunchRight.Parent))
            {
                ModMain.FrmLaunchRight.SetValue(ContentPresenter.ContentProperty, null);
            }
            this.PanMainLeft.Child = ModMain.FrmLaunchLeft;
            this.PageLeft = ModMain.FrmLaunchLeft;
            this.PanMainRight.Child = ModMain.FrmLaunchRight;
            this.PageRight = ModMain.FrmLaunchRight;
            ModMain.FrmLaunchRight.PageState = MyPageRight.PageStates.ContentStay;
            if (ModBase.ModeDebug)
            {
                ModMain.Hint("[调试模式] PCL 正以调试模式运行，这可能会导致性能下降，若无必要请不要开启！", ModMain.HintType.Blue, true);
            }
            ModMinecraft.McFolderListLoader.Start(0, false);
            Logger.Info(string.Format("第二阶段加载用时：{0} ms", checked(ModBase.GetTimeMs() - ModBase.ApplicationStartTick)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
        }

        private void FormMain_Loaded(object sender, RoutedEventArgs e)
        {
            ModBase.ApplicationStartTick = ModBase.GetTimeMs();
            ModBase.Handle = new WindowInteropHelper(this).Handle;
            this.UpdateBackgroundAndTitleBar();
            PageSetupUI.HiddenRefresh();
            PageSetupUI.BackgroundRefresh(false, true);
            ModMusic.MusicRefreshPlay(false, true);
            this.BtnExtraDownload.ShowCheck = new MyExtraButton.ShowCheckDelegate(this.BtnExtraDownload_ShowCheck);
            this.BtnExtraBack.ShowCheck = new MyExtraButton.ShowCheckDelegate(this.BtnExtraBack_ShowCheck);
            this.BtnExtraApril.ShowCheck = new MyExtraButton.ShowCheckDelegate(this.BtnExtraApril_ShowCheck);
            this.BtnExtraShutdown.ShowCheck = new MyExtraButton.ShowCheckDelegate(this.BtnExtraShutdown_ShowCheck);
            this.BtnExtraApril.ShowRefresh();
            MyResizer myResizer = new MyResizer(this);
            myResizer.addResizerDown(this.ResizerB);
            myResizer.addResizerLeft(this.ResizerL);
            myResizer.addResizerLeftDown(this.ResizerLB);
            myResizer.addResizerLeftUp(this.ResizerLT);
            myResizer.addResizerRight(this.ResizerR);
            myResizer.addResizerRightDown(this.ResizerRB);
            myResizer.addResizerRightUp(this.ResizerRT);
            myResizer.addResizerUp(this.ResizerT);
            if (ModBase.RandomInteger(1, 1000) == 233)
            {
                this.ShapeTitleLogo.Data = (Geometry)new GeometryConverter().ConvertFromString("M26,29 v-25 h6 a7,7 180 0 1 0,14 h-6 M83,6.5 a10,11.5 180 1 0 0,18 M48,2.5 v24.5 h13.5");
            }
            ModSecret.ThemeRefreshMain();
            try
            {
                base.Height = (double)Settings.Get<int>("WindowHeight", null);
                base.Width = (double)Settings.Get<int>("WindowWidth", null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "读取窗口默认大小失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                base.Height = base.MinHeight + 100.0;
                base.Width = base.MinWidth + 100.0;
            }
            base.Topmost = false;
            if (ModMain.FrmStart != null)
            {
                ModMain.FrmStart.Close(new TimeSpan(0, 0, 0, 0, checked((int)Math.Round(400.0 / ModAnimation.AniSpeed))));
            }
            base.Top = (ModBase.GetWPFSize((double)MyWpfExtension.Computer.Screen.WorkingArea.Height) - base.Height) / 2.0;
            base.Left = (ModBase.GetWPFSize((double)MyWpfExtension.Computer.Screen.WorkingArea.Width) - base.Width) / 2.0;
            this.IsSizeSaveable = true;
            this.ShowWindowToTop();
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(new HwndSourceHook(this.WndProc));
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((FormMain._Closure$__.$I4-0 == null) ? (FormMain._Closure$__.$I4-0 = checked(delegate
                {
                    ModAnimation.AniControlEnabled--;
                })) : FormMain._Closure$__.$I4-0, 50, false),
                ModAnimation.AaOpacity(this, (double)Settings.Get<int>("UiLauncherTransparent", null) / 1000.0 + 0.4, 250, 100, null, false),
                ModAnimation.AaDouble(delegate(object i)
                {
                    TranslateTransform transformPos;
                    (transformPos = this.TransformPos).Y = Conversions.ToDouble(Operators.AddObject(transformPos.Y, i));
                }, -this.TransformPos.Y, 600, 100, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaDouble(delegate(object i)
                {
                    RotateTransform transformRotate;
                    (transformRotate = this.TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject(transformRotate.Angle, i));
                }, -this.TransformRotate.Angle, 500, 100, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaCode(delegate
                {
                    this.PanBack.RenderTransform = null;
                    this.IsWindowLoadFinished = true;
                    Logger.Info(string.Format("DPI：{0}，系统版本：{1}，PCL 位置：{2}", ModBase.DPI, Environment.OSVersion.VersionString, ModBase.PathExe), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                }, 0, true)
            }, "Form Show", false);
            ModAnimation.AniStart();
            ModMain.TimerMainStart();
            ModBase.RunInNewThread(delegate
            {
                if (Settings.Get<int>("SystemEulaVersion", null) < 2)
                {
                    int num = ModMain.MyMsgBox((Settings.Get<int>("SystemEulaVersion", null) == 0) ? "在使用 PCL 前，请先阅读用户协议与免责声明。" : string.Format("PCL 的用户协议与免责声明已更新。{0}请阅读更新后的用户协议与免责声明。", "\r\n"), "协议授权", "同意", "拒绝", "查看用户协议与免责声明", false, true, false, null, null, (FormMain._Closure$__.$I4-5 == null) ? (FormMain._Closure$__.$I4-5 = delegate
                    {
                        ModBase.OpenWebsite("https://shimo.im/docs/rGrd8pY8xWkt6ryW");
                    }) : FormMain._Closure$__.$I4-5);
                    if (num != 1)
                    {
                        if (num == 2)
                        {
                            this.EndProgram(false);
                        }
                    }
                    else
                    {
                        Settings.Set("SystemEulaVersion", 2, null);
                    }
                }
                try
                {
                    ModJava.JavaInit();
                    Thread.Sleep(100);
                    ModDownload.DlClientListMojangLoader.Start(1, false);
                    this.RunCountSub();
                    ModSecret.ServerLoader.Start(null, false);
                    ModBase.RunInNewThread(new Action(ModMain.TryClearTaskTemp), "TryClearTaskTemp", ThreadPriority.BelowNormal);
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, "初始化加载池运行失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                }
                try
                {
                    FileUtils.Delete(Paths.Base + "PCL\\Plain Craft Launcher 2.exe", false);
                }
                catch (Exception ex3)
                {
                    Logger.Warn(ex3, "清理自动更新文件失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                }
                ModMain.Telemetry("启动", new string[0]);
            }, "初始化", ThreadPriority.Lowest);
            Logger.Info(string.Format("第三阶段加载用时：{0} ms", checked(ModBase.GetTimeMs() - ModBase.ApplicationStartTick)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
        }

        private void RunCountSub()
        {
            Settings.Set("SystemCount", checked(Settings.Get<int>("SystemCount", null) + 1), null);
            int num = Settings.Get<int>("SystemCount", null);
            if (num == 1)
            {
                ModMain.MyMsgBox("欢迎使用 PCL 快照版！\r\n快照版包含尚未在正式版发布的测试性功能，仅用于赞助者本人尝鲜。所以请不要发给其他人或者用于制作整合包哦！\r\n如果你并非通过赞助或赞助者本人邀请进群获得的本程序，那么可能是有人在违规传播，记得提醒他一下啦。", "快照版使用说明", "确定", "", "", false, true, false, null, null, null);
            }
            if (Settings.Get<int>("SystemCount", null) >= 99 && ModSecret.ThemeUnlock(6, false, null))
            {
                ModMain.MyMsgBox("你已经使用了 99 次 PCL 啦，感谢你长期以来的支持！\r\n隐藏主题 铁杆粉 已解锁！", "提示", "确定", "", "", false, true, false, null, null, null);
            }
        }

        private void UpgradeSub(int LastVersionCode)
        {
            Logger.Info(string.Format("版本号从 {0} 升高到 {1}", LastVersionCode, 414), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            Settings.Set("SystemLastVersionReg", 414, null);
            int num = Settings.Get<int>("SystemHighestAlphaVersionReg", null);
            if (num < 414)
            {
                Settings.Set("SystemHighestAlphaVersionReg", 414, null);
                Logger.Info(string.Format("最高版本号从 {0} 升高到 {1}", num, 414), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
            if (Settings.Get<int>("LaunchArgumentWindowType", null) == 5)
            {
                Settings.Set("LaunchArgumentWindowType", 1, null);
            }
            if (num <= 207)
            {
                List<string> list = new List<string> { "2" };
                list.AddRange(new List<string>(Settings.Get<string>("UiLauncherThemeHide", null).ToString().Split("|", false)));
                list.AddRange(new List<string>(Settings.Get<string>("UiLauncherThemeHide2", null).ToString().Split("|", false)));
                Settings.Set("UiLauncherThemeHide2", list.Distinct<string>().ToList<string>().Join('|'), null);
            }
            if (LastVersionCode <= 115 && Settings.Get<string>("UiLauncherThemeHide2", null).ToString().Split("|", false)
                .Contains("13"))
            {
                List<string> list2 = new List<string>(Settings.Get<string>("UiLauncherThemeHide2", null).ToString().Split("|", false));
                list2.Remove("13");
                Settings.Set("UiLauncherThemeHide2", list2.Join('|'), null);
                ModMain.MyMsgBox("由于新版 PCL 修改了欧皇彩的解锁方式，你需要重新解锁欧皇彩。\r\n多谢各位的理解啦！", "重新解锁提醒", "确定", "", "", false, true, false, null, null, null);
            }
            if (LastVersionCode <= 152 && Settings.Get<string>("UiLauncherThemeHide2", null).ToString().Split("|", false)
                .Contains("12"))
            {
                List<string> list3 = new List<string>(Settings.Get<string>("UiLauncherThemeHide2", null).ToString().Split("|", false));
                list3.Remove("12");
                Settings.Set("UiLauncherThemeHide2", list3.Join('|'), null);
                ModMain.MyMsgBox("由于新版 PCL 修改了滑稽彩的解锁方式，你需要重新解锁滑稽彩。\r\n多谢各位的理解啦！", "重新解锁提醒", "确定", "", "", false, true, false, null, null, null);
            }
            if (LastVersionCode <= 161 && FileUtils.Exists(Paths.Base + "PCL\\CustomSkin.png") && !FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png"))
            {
                FileUtils.Copy(Paths.Base + "PCL\\CustomSkin.png", Paths.AppDataThenName + "CustomSkin.png");
                Logger.Info("已移动离线自定义皮肤 (162)", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
            if (LastVersionCode <= 263 && FileUtils.Exists(ModBase.PathTemp + "CustomSkin.png") && !FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png"))
            {
                FileUtils.Copy(ModBase.PathTemp + "CustomSkin.png", Paths.AppDataThenName + "CustomSkin.png");
                Logger.Info("已移动离线自定义皮肤 (264)", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
            if (LastVersionCode <= 205)
            {
                Settings.Set("UiHiddenOtherHelp", false, null);
                Logger.Info("已解除帮助页面的隐藏", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
            if (!Settings.Get<bool>("CacheMsV2Migrated", null))
            {
                Settings.Set("CacheMsV2Migrated", true, null);
                Settings.Set("CacheMsV2OAuthRefresh", Settings.Get<string>("CacheMsOAuthRefresh", null), null);
                Settings.Set("CacheMsV2Access", Settings.Get<string>("CacheMsAccess", null), null);
                Settings.Set("CacheMsV2ProfileJson", Settings.Get<string>("CacheMsProfileJson", null), null);
                Settings.Set("CacheMsV2Uuid", Settings.Get<string>("CacheMsUuid", null), null);
                Settings.Set("CacheMsV2Name", Settings.Get<string>("CacheMsName", null), null);
                Logger.Info("已从老版本迁移微软登录结果", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
            if (Settings.HasSaved("ToolDownloadTranslate", null) && !Settings.HasSaved("ToolDownloadTranslateV2", null))
            {
                Settings.Set("ToolDownloadTranslateV2", checked(Settings.Get<int>("ToolDownloadTranslate", null) + 1), null);
                Logger.Info("已从老版本迁移 Mod 命名设置", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
            if (LastVersionCode <= 381 && Settings.HasSaved("LaunchAdvanceJvm", null) && Operators.CompareString(Settings.Get<string>("LaunchAdvanceJvm", null).ToString().Replace("-XX:+UseG1GC ", "")
                .Replace("-XX:-UseAdaptiveSizePolicy ", "")
                .Trim(), Settings.GetDefault("LaunchAdvanceJvm"), false) == 0)
            {
                Settings.Reset("LaunchAdvanceJvm", null);
                Logger.Info("已重置 JVM 参数设置", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
            if (LastVersionCode > 0 && num < 414)
            {
                this.ShowUpdateLog(num);
            }
        }

        private void DowngradeSub(int LastVersionCode)
        {
            Logger.Info(string.Format("版本号从 {0} 降低到 {1}", LastVersionCode, 414), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            Settings.Set("SystemLastVersionReg", 414, null);
        }

        private void FormMain_Closing(object sender, CancelEventArgs e)
        {
            this.EndProgram(true);
            e.Cancel = true;
        }

        public void EndProgram(bool SendWarning)
        {
            if (ModNet.HasDownloadingTask(false))
            {
                if (SendWarning && ModMain.MyMsgBox("还有下载任务尚未完成，是否确定退出？", "提示", "确定", "取消", "", false, true, false, null, null, null) == 2)
                {
                    return;
                }
                ModBase.RunInNewThread((FormMain._Closure$__.$I9-0 == null) ? (FormMain._Closure$__.$I9-0 = delegate
                {
                    Logger.Info("正在强行停止任务", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    try
                    {
                        foreach (ModLoader.LoaderBase loaderBase in ModLoader.LoaderTaskbar.ToList<ModLoader.LoaderBase>())
                        {
                            loaderBase.Cancel();
                        }
                    }
                    finally
                    {
                        List<ModLoader.LoaderBase>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                }) : FormMain._Closure$__.$I9-0, "强行停止下载任务", ThreadPriority.Normal);
            }
            PageLinkMain frmLinkMain = ModMain.FrmLinkMain;
            if (frmLinkMain == null || !frmLinkMain.TryExit(!SendWarning, true))
            {
                ModBase.RunInUiWait(delegate
                {
                    base.IsHitTestVisible = false;
                    if (this.PanBack.RenderTransform == null)
                    {
                        FormMain._Closure$__9-0 CS$<>8__locals1 = new FormMain._Closure$__9-0(CS$<>8__locals1);
                        CS$<>8__locals1.$VB$Local_TransformPos = new TranslateTransform(0.0, 0.0);
                        CS$<>8__locals1.$VB$Local_TransformRotate = new RotateTransform(0.0);
                        CS$<>8__locals1.$VB$Local_TransformScale = new ScaleTransform(1.0, 1.0);
                        this.PanBack.RenderTransform = new TransformGroup
                        {
                            Children = new TransformCollection(new Transform[] { CS$<>8__locals1.$VB$Local_TransformRotate, CS$<>8__locals1.$VB$Local_TransformPos, CS$<>8__locals1.$VB$Local_TransformScale })
                        };
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaOpacity(this, -base.Opacity, 140, 40, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                            ModAnimation.AaDouble(delegate(object i)
                            {
                                ScaleTransform scaleTransform;
                                (scaleTransform = CS$<>8__locals1.$VB$Local_TransformScale).ScaleX = Conversions.ToDouble(Operators.AddObject(scaleTransform.ScaleX, i));
                                (scaleTransform = CS$<>8__locals1.$VB$Local_TransformScale).ScaleY = Conversions.ToDouble(Operators.AddObject(scaleTransform.ScaleY, i));
                            }, 0.88 - CS$<>8__locals1.$VB$Local_TransformScale.ScaleX, 180, 0, null, false),
                            ModAnimation.AaDouble(delegate(object i)
                            {
                                TranslateTransform $VB$Local_TransformPos;
                                ($VB$Local_TransformPos = CS$<>8__locals1.$VB$Local_TransformPos).Y = Conversions.ToDouble(Operators.AddObject($VB$Local_TransformPos.Y, i));
                            }, 20.0 - CS$<>8__locals1.$VB$Local_TransformPos.Y, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                            ModAnimation.AaDouble(delegate(object i)
                            {
                                RotateTransform $VB$Local_TransformRotate;
                                ($VB$Local_TransformRotate = CS$<>8__locals1.$VB$Local_TransformRotate).Angle = Conversions.ToDouble(Operators.AddObject($VB$Local_TransformRotate.Angle, i));
                            }, 0.6 - CS$<>8__locals1.$VB$Local_TransformRotate.Angle, 180, 0, new ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Weak, 0.5), false),
                            ModAnimation.AaCode(delegate
                            {
                                base.IsHitTestVisible = false;
                                base.Top = -10000.0;
                                base.ShowInTaskbar = false;
                            }, 210, false),
                            ModAnimation.AaCode((FormMain._Closure$__.$IR9-7 == null) ? (FormMain._Closure$__.$IR9-7 = delegate
                            {
                                FormMain.EndProgramForce(ModBase.ProcessReturnValues.Success);
                            }) : FormMain._Closure$__.$IR9-7, 230, false)
                        }, "Form Close", false);
                    }
                    else
                    {
                        FormMain.EndProgramForce(ModBase.ProcessReturnValues.Success);
                    }
                    Logger.Info("收到关闭指令", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                });
            }
        }

        public static void EndProgramForce(ModBase.ProcessReturnValues ReturnCode = ModBase.ProcessReturnValues.Success)
        {
            int num;
            int num4;
            object obj;
            try
            {
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                int num2 = 2;
                ModBase.IsProgramEnding = true;
                IL_000F:
                num2 = 3;
                checked
                {
                    ModAnimation.AniControlEnabled++;
                }
                IL_001D:
                num2 = 4;
                if (!ModSecret.IsUpdateWaitingRestart)
                {
                    goto IL_002E;
                }
                IL_0026:
                num2 = 5;
                ModSecret.UpdateRestart(false);
                IL_002E:
                num2 = 6;
                if (ReturnCode != ModBase.ProcessReturnValues.Exception)
                {
                    goto IL_008A;
                }
                IL_0034:
                num2 = 7;
                if (FormMain.IsLogShown)
                {
                    goto IL_007D;
                }
                IL_003D:
                num2 = 8;
                ModMain.FeedbackInfo();
                IL_0044:
                num2 = 9;
                Logger.Info("请在 https://github.com/Meloong-Git/PCL/issues 提交错误报告，以便于作者解决此问题！", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                IL_0057:
                num2 = 10;
                FormMain.IsLogShown = true;
                IL_0060:
                num2 = 11;
                ModBase.StartProcess(Paths.Base + "PCL\\Log1.txt", "");
                IL_007D:
                num2 = 12;
                Thread.Sleep(500);
                IL_008A:
                num2 = 13;
                Logger.Info(string.Format("程序已退出，返回值：{0}", ReturnCode), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                IL_00A8:
                num2 = 14;
                ConfigUtils.SaveAll();
                IL_00B0:
                num2 = 15;
                Logger.Instance.Flush();
                IL_00BD:
                num2 = 16;
                if (ReturnCode == ModBase.ProcessReturnValues.Success)
                {
                    goto IL_00CC;
                }
                IL_00C3:
                num2 = 17;
                Environment.Exit((int)ReturnCode);
                IL_00CC:
                num2 = 18;
                Process.GetCurrentProcess().Kill();
                IL_00D9:
                goto IL_0177;
                IL_00DE:
                int num3 = num4 + 1;
                num4 = 0;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3);
                IL_0138:
                goto IL_016C;
                IL_013A:
                num4 = num2;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
                IL_014A:;
            }
            catch when (endfilter((obj is Exception) & (num != 0) & (num4 == 0)))
            {
                Exception ex = (Exception)obj2;
                goto IL_013A;
            }
            IL_016C:
            throw ProjectData.CreateProjectError(-2146828237);
            IL_0177:
            if (num4 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }

        private void BtnTitleClose_Click(object sender, RoutedEventArgs e)
        {
            this.EndProgram(true);
        }

        private void FormDragMove(object sender, MouseButtonEventArgs e)
        {
            int num;
            int num4;
            object obj;
            try
            {
                IL_0000:
                ProjectData.ClearProjectError();
                num = 1;
                IL_0007:
                int num2 = 2;
                if (!Conversions.ToBoolean(NewLateBinding.LateGet(sender, null, "IsMouseDirectlyOver", new object[0], null, null, null)))
                {
                    goto IL_002D;
                }
                IL_0025:
                num2 = 3;
                base.DragMove();
                IL_002D:
                goto IL_008C;
                IL_002F:
                int num3 = num4 + 1;
                num4 = 0;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3);
                IL_004D:
                goto IL_0081;
                IL_004F:
                num4 = num2;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);
                IL_005F:;
            }
            catch when (endfilter((obj is Exception) & (num != 0) & (num4 == 0)))
            {
                Exception ex = (Exception)obj2;
                goto IL_004F;
            }
            IL_0081:
            throw ProjectData.CreateProjectError(-2146828237);
            IL_008C:
            if (num4 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }

        private void FormMain_SizeChanged()
        {
            if (this.IsSizeSaveable)
            {
                Settings.Set("WindowHeight", base.Height, null);
                Settings.Set("WindowWidth", base.Width, null);
            }
            this.RectForm.Rect = new Rect(0.0, 0.0, this.BorderForm.ActualWidth, this.BorderForm.ActualHeight);
            this.PanForm.Width = this.BorderForm.ActualWidth + 0.001;
            this.PanForm.Height = this.BorderForm.ActualHeight + 0.001;
            this.PanMain.Width = this.PanForm.Width;
            this.PanMain.Height = Math.Max(0.0, this.PanForm.Height - this.PanTitle.ActualHeight);
            if (base.WindowState == WindowState.Maximized)
            {
                base.WindowState = WindowState.Normal;
            }
        }

        private void BtnTitleMin_Click()
        {
            base.WindowState = WindowState.Minimized;
        }

        public static void UpdateBackgroundAndTitleBar(object Value)
        {
            if (ModMain.FrmMain != null && ModMain.FrmMain.IsLoaded)
            {
                ModMain.FrmMain.UpdateBackgroundAndTitleBar();
            }
        }

        public void UpdateBackgroundAndTitleBar()
        {
            Logger.Info("从设置更新背景图片与标题栏样式", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            this.ImgBack.Opacity = (double)Settings.Get<int>("UiBackgroundOpacity", null) / 1000.0;
            double num = (double)(checked(Settings.Get<int>("UiBackgroundBlur", null) + 1));
            if (num == 1.0)
            {
                this.ImgBack.Effect = null;
            }
            else
            {
                this.ImgBack.Effect = new BlurEffect
                {
                    Radius = num
                };
            }
            this.ImgBack.Margin = new Thickness(-num / 1.8);
            int num2 = Settings.Get<int>("UiBackgroundSuit", null);
            if (this.ImgBack.Background != null)
            {
                ImageBrush imageBrush = (ImageBrush)this.ImgBack.Background;
                if (num2 == 0)
                {
                    if (imageBrush.ImageSource.Width < this.PanMain.ActualWidth / 2.0 && imageBrush.ImageSource.Height < this.PanMain.ActualHeight / 2.0)
                    {
                        num2 = 4;
                    }
                    else
                    {
                        num2 = 2;
                    }
                }
                imageBrush.Stretch = Stretch.UniformToFill;
                imageBrush.TileMode = TileMode.None;
                imageBrush.Viewport = new Rect(0.0, 0.0, 1.0, 1.0);
                imageBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                imageBrush.AlignmentX = AlignmentX.Center;
                imageBrush.AlignmentY = AlignmentY.Center;
                switch (num2)
                {
                case 1:
                    imageBrush.Stretch = Stretch.None;
                    break;
                case 3:
                    imageBrush.Stretch = Stretch.Fill;
                    break;
                case 4:
                    imageBrush.Stretch = Stretch.None;
                    imageBrush.TileMode = TileMode.Tile;
                    imageBrush.Viewport = new Rect(0.0, 0.0, imageBrush.ImageSource.Width, imageBrush.ImageSource.Height);
                    imageBrush.ViewportUnits = BrushMappingMode.Absolute;
                    break;
                case 5:
                    imageBrush.AlignmentX = AlignmentX.Left;
                    imageBrush.AlignmentY = AlignmentY.Top;
                    break;
                case 6:
                    imageBrush.AlignmentX = AlignmentX.Right;
                    imageBrush.AlignmentY = AlignmentY.Top;
                    break;
                case 7:
                    imageBrush.AlignmentX = AlignmentX.Left;
                    imageBrush.AlignmentY = AlignmentY.Bottom;
                    break;
                case 8:
                    imageBrush.AlignmentX = AlignmentX.Right;
                    imageBrush.AlignmentY = AlignmentY.Bottom;
                    break;
                case 9:
                    imageBrush.AlignmentX = AlignmentX.Left;
                    imageBrush.AlignmentY = AlignmentY.Center;
                    break;
                case 10:
                    imageBrush.AlignmentX = AlignmentX.Right;
                    imageBrush.AlignmentY = AlignmentY.Center;
                    break;
                case 11:
                    imageBrush.AlignmentX = AlignmentX.Center;
                    imageBrush.AlignmentY = AlignmentY.Top;
                    break;
                case 12:
                    imageBrush.AlignmentX = AlignmentX.Center;
                    imageBrush.AlignmentY = AlignmentY.Bottom;
                    break;
                }
            }
            switch (Settings.Get<int>("UiLogoType", null))
            {
            case 0:
                this.ShapeTitleLogo.Visibility = Visibility.Collapsed;
                this.LabTitleLogo.Visibility = Visibility.Collapsed;
                this.ImageTitleLogo.Visibility = Visibility.Collapsed;
                if (!Information.IsNothing(ModMain.FrmSetupUI))
                {
                    ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Visible;
                    ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Collapsed;
                }
                break;
            case 1:
                this.ShapeTitleLogo.Visibility = Visibility.Visible;
                this.LabTitleLogo.Visibility = Visibility.Collapsed;
                this.ImageTitleLogo.Visibility = Visibility.Collapsed;
                if (!Information.IsNothing(ModMain.FrmSetupUI))
                {
                    ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Collapsed;
                }
                break;
            case 2:
                this.ShapeTitleLogo.Visibility = Visibility.Collapsed;
                this.LabTitleLogo.Visibility = Visibility.Visible;
                this.ImageTitleLogo.Visibility = Visibility.Collapsed;
                if (!Information.IsNothing(ModMain.FrmSetupUI))
                {
                    ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Visible;
                    ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Collapsed;
                }
                break;
            case 3:
                this.ShapeTitleLogo.Visibility = Visibility.Collapsed;
                this.LabTitleLogo.Visibility = Visibility.Collapsed;
                this.ImageTitleLogo.Visibility = Visibility.Visible;
                if (!Information.IsNothing(ModMain.FrmSetupUI))
                {
                    ModMain.FrmSetupUI.CheckLogoLeft.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanLogoText.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanLogoChange.Visibility = Visibility.Visible;
                }
                try
                {
                    this.ImageTitleLogo.Source = Paths.Base + "PCL\\Logo.png";
                }
                catch (Exception ex)
                {
                    this.ImageTitleLogo.Source = null;
                    Logger.Error(ex, "显示标题栏图片失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                }
                break;
            }
            PageSetupUI frmSetupUI = ModMain.FrmSetupUI;
            if (frmSetupUI != null)
            {
                frmSetupUI.CardLogo.TriggerForceResize();
            }
            this.LabTitleLogo.Text = Settings.Get<string>("UiLogoText", null);
            this.PanTitleMain.ColumnDefinitions[0].Width = new GridLength((double)((Settings.Get<bool>("UiLogoLeft", null) && Settings.Get<int>("UiLogoType", null) == 0) ? 0 : 1), GridUnitType.Star);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
                {
                    e.Handled = true;
                }
                if (this.PanMsg.Children.Any())
                {
                    if (e.Key == Key.Return)
                    {
                        NewLateBinding.LateCall(this.PanMsg.Children[0], null, "Btn1_Click", new object[0], null, null, null, true);
                        return;
                    }
                    if (e.Key == Key.Escape)
                    {
                        object obj = this.PanMsg.Children[0];
                        if (!(obj is MyMsgInput) && !(obj is MyMsgSelect) && Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(NewLateBinding.LateGet(obj, null, "Btn3", new object[0], null, null, null), null, "Visibility", new object[0], null, null, null), Visibility.Visible, false))
                        {
                            NewLateBinding.LateCall(obj, null, "Btn3_Click", new object[0], null, null, null, true);
                            return;
                        }
                        if (Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(NewLateBinding.LateGet(obj, null, "Btn2", new object[0], null, null, null), null, "Visibility", new object[0], null, null, null), Visibility.Visible, false))
                        {
                            NewLateBinding.LateCall(obj, null, "Btn2_Click", new object[0], null, null, null, true);
                            return;
                        }
                        NewLateBinding.LateCall(obj, null, "Btn1_Click", new object[0], null, null, null, true);
                        return;
                    }
                }
                else
                {
                    if (e.Key == Key.Escape)
                    {
                        if (this.PageCurrent == FormMain.PageType.Launch && ModLaunch.McLaunchLoader.State == ModBase.LoadState.Loading)
                        {
                            ModMain.FrmLaunchLeft.BtnCancel_Click();
                            return;
                        }
                        this.TriggerPageBack();
                    }
                    if (e.Key == Key.F11 && this.PageCurrent == FormMain.PageType.InstanceSelect)
                    {
                        ModMain.FrmSelectRight.ShowHidden = !ModMain.FrmSelectRight.ShowHidden;
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                        return;
                    }
                    if (e.Key == Key.F12)
                    {
                        PageSetupUI.HiddenForceShow = !PageSetupUI.HiddenForceShow;
                        if (PageSetupUI.HiddenForceShow)
                        {
                            ModMain.Hint("功能隐藏设置已暂时关闭！", ModMain.HintType.Green, true);
                        }
                        else
                        {
                            ModMain.Hint("功能隐藏设置已重新开启！", ModMain.HintType.Green, true);
                        }
                        PageSetupUI.HiddenRefresh();
                        return;
                    }
                    if (e.Key == Key.F5)
                    {
                        if (this.PageLeft is IRefreshable)
                        {
                            ((IRefreshable)this.PageLeft).Refresh();
                        }
                        if (this.PageRight is IRefreshable)
                        {
                            ((IRefreshable)this.PageRight).Refresh();
                            return;
                        }
                    }
                    else if (e.Key == Key.Return && this.PageCurrent == FormMain.PageType.Launch)
                    {
                        if (ModMain.IsAprilEnabled && !ModMain.IsAprilGiveup)
                        {
                            ModMain.Hint("木大！", ModMain.HintType.Blue, true);
                            return;
                        }
                        ModMain.FrmLaunchLeft.LaunchButtonClick();
                    }
                }
            }
        }

        private void FormMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ModMain.FrmMain.PanMsg.Children.Count <= 0 && !ModMain.WaitingMyMsgBox.Any<ModMain.MyMsgBoxConverter>() && (e.ChangedButton == MouseButton.XButton1 || e.ChangedButton == MouseButton.XButton2))
            {
                this.TriggerPageBack();
            }
        }

        private void TriggerPageBack()
        {
            if (this.PageCurrent == FormMain.PageType.Download && this.PageCurrentSub == FormMain.PageSubType.Default && ModMain.FrmDownloadInstall.IsInSelectPage)
            {
                ModMain.FrmDownloadInstall.ExitSelectPage();
                return;
            }
            this.PageBack();
        }

        private void FormMain_Activated()
        {
            try
            {
                if (this.PageCurrent == FormMain.PageType.InstanceSetup && this.PageCurrentSub == FormMain.PageSubType.DownloadPack)
                {
                    ModMain.FrmInstanceMod.ReloadModList(false);
                }
                else if (this.PageCurrent == FormMain.PageType.InstanceSetup && (this.PageCurrentSub == FormMain.PageSubType.SetupLink || this.PageCurrentSub == FormMain.PageSubType.DownloadResourcePack))
                {
                    PageInstanceLeft.ReloadCurrentJava();
                }
                else if (this.PageCurrent == FormMain.PageType.InstanceSelect)
                {
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.RunOnUpdated, 1, "versions\\", false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "切回窗口时出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
        }

        private void HandleDrag(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Handled || e.Effects == DragDropEffects.None)
                {
                    e.Handled = true;
                    if (e.Data == this.$STATIC$HandleDrag$20211C1284DD$PrevData)
                    {
                        e.Effects = this.$STATIC$HandleDrag$20211C1284DD$PrevEffects;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                        if (e.Data.GetDataPresent(DataFormats.Text))
                        {
                            string text = Conversions.ToString(e.Data.GetData(DataFormats.Text));
                            if (text.StartsWithF("authlib-injector:yggdrasil-server:", false))
                            {
                                e.Effects = DragDropEffects.Copy;
                            }
                            else if (text.StartsWithF("file:///", false))
                            {
                                e.Effects = DragDropEffects.Copy;
                            }
                        }
                        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                        {
                            string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
                            if (array != null && array.Length > 0)
                            {
                                e.Effects = DragDropEffects.Link;
                            }
                        }
                        this.$STATIC$HandleDrag$20211C1284DD$PrevData = e.Data;
                        this.$STATIC$HandleDrag$20211C1284DD$PrevEffects = e.Effects;
                        Logger.Info(string.Format("设置拖放类型：{0}", e.Effects), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "处理拖放时出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
        }

        private void FrmMain_Drop(object sender, DragEventArgs e)
        {
            try
            {
                this.ShowWindowToTop();
                if (e.Data.GetDataPresent(DataFormats.Text))
                {
                    try
                    {
                        string text = Conversions.ToString(e.Data.GetData(DataFormats.Text));
                        Logger.Info(string.Format("接受文本拖拽：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                        if (text.StartsWithF("authlib-injector:yggdrasil-server:", false))
                        {
                            e.Handled = true;
                            e.Effects = DragDropEffects.Copy;
                            string text2 = StringUtils.FormUrlUnescape(text.Substring("authlib-injector:yggdrasil-server:".Length));
                            Logger.Info(string.Format("Authlib 拖拽：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                            if (!string.IsNullOrEmpty(new ValidateHttp().Validate(text2)))
                            {
                                ModMain.Hint(string.Format("输入的 Authlib 验证服务器不符合网址格式（{0}）！", text2), ModMain.HintType.Red, true);
                            }
                            else
                            {
                                ModMinecraft.McInstance mcInstance = ((this.PageCurrent == FormMain.PageType.InstanceSetup) ? PageInstanceLeft.Instance : ModMinecraft.McInstanceSelected);
                                if (mcInstance == null)
                                {
                                    ModMain.Hint("请先下载游戏，再设置第三方登录！", ModMain.HintType.Red, true);
                                }
                                else
                                {
                                    if (Operators.CompareString(text2, "https://littleskin.cn/api/yggdrasil", false) == 0)
                                    {
                                        if (ModMain.MyMsgBox(string.Format("是否要在版本 {0} 中开启 LittleSkin 登录？", mcInstance.Name) + "\r\n你可以在 版本设置 → 设置 → 服务器选项 中修改登录方式。", "第三方登录开启确认", "确定", "取消", "", false, true, false, null, null, null) == 2)
                                        {
                                            return;
                                        }
                                        Settings.Set("VersionServerLogin", 4, mcInstance);
                                        Settings.Set("VersionServerAuthServer", "https://littleskin.cn/api/yggdrasil", mcInstance);
                                        Settings.Set("VersionServerAuthRegister", "https://littleskin.cn/auth/register", mcInstance);
                                        Settings.Set("VersionServerAuthName", "LittleSkin 登录", mcInstance);
                                    }
                                    else
                                    {
                                        if (ModMain.MyMsgBox(string.Format("是否要在版本 {0} 中开启第三方登录？", mcInstance.Name) + "\r\n" + string.Format("登录服务器：{0}", text2) + "\r\n\r\n你可以在 版本设置 → 设置 → 服务器选项 中修改登录方式。", "第三方登录开启确认", "确定", "取消", "", false, true, false, null, null, null) == 2)
                                        {
                                            return;
                                        }
                                        Settings.Set("VersionServerLogin", 4, mcInstance);
                                        Settings.Set("VersionServerAuthServer", text2, mcInstance);
                                        Settings.Set("VersionServerAuthRegister", text2.Replace("api/yggdrasil", "auth/register"), mcInstance);
                                        Settings.Set("VersionServerAuthName", "", mcInstance);
                                    }
                                    if (this.PageCurrent == FormMain.PageType.InstanceSetup && this.PageCurrentSub == FormMain.PageSubType.SetupLink)
                                    {
                                        ModMain.FrmInstanceSetup.Reload();
                                    }
                                    else if (this.PageCurrent == FormMain.PageType.Launch)
                                    {
                                        ModMain.FrmLaunchLeft.RefreshPage(true, false);
                                    }
                                }
                            }
                        }
                        else if (text.StartsWithF("file:///", false))
                        {
                            string text3 = WebUtility.UrlDecode(text).Substring("file:///".Length).Replace("/", "\\");
                            e.Handled = true;
                            e.Effects = DragDropEffects.Copy;
                            this.FileDrag(new List<string> { text3 });
                        }
                        goto IL_0335;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "无法接取文本拖拽事件", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                        return;
                    }
                }
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(e.Data.GetData(DataFormats.FileDrop));
                    if (objectValue == null)
                    {
                        ModMain.Hint("请将文件解压后再拖入！", ModMain.HintType.Red, true);
                    }
                    else
                    {
                        e.Handled = true;
                        e.Effects = DragDropEffects.Link;
                        this.FileDrag((IEnumerable<string>)objectValue);
                    }
                }
                IL_0335:;
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "接取拖拽事件失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
        }

        private void FileDrag(IEnumerable<string> FilePathList)
        {
            FormMain._Closure$__25-0 CS$<>8__locals1 = new FormMain._Closure$__25-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_FilePathList = FilePathList;
            ModBase.RunInNewThread(delegate
            {
                FormMain._Closure$__25-1 CS$<>8__locals2 = new FormMain._Closure$__25-1(CS$<>8__locals2);
                string text = CS$<>8__locals1.$VB$Local_FilePathList.First<string>();
                Logger.Warn(string.Format("接受文件拖拽：{0}{1}", text, CS$<>8__locals1.$VB$Local_FilePathList.Any<string>() ? string.Format(" 等 {0} 个文件", CS$<>8__locals1.$VB$Local_FilePathList.Count<string>()) : ""), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                if (DirectoryUtils.Exists(CS$<>8__locals1.$VB$Local_FilePathList.First<string>()) && !FileUtils.Exists(CS$<>8__locals1.$VB$Local_FilePathList.First<string>()))
                {
                    ModMain.Hint("请拖入一个文件，而非文件夹！", ModMain.HintType.Red, true);
                    return;
                }
                if (!FileUtils.Exists(CS$<>8__locals1.$VB$Local_FilePathList.First<string>()))
                {
                    ModMain.Hint("拖入的文件不存在：" + CS$<>8__locals1.$VB$Local_FilePathList.First<string>(), ModMain.HintType.Red, true);
                    return;
                }
                List<string> list = CS$<>8__locals1.$VB$Local_FilePathList.ToList<string>();
                if (CS$<>8__locals1.$VB$Local_FilePathList.Count<string>() > 1)
                {
                    try
                    {
                        foreach (string text2 in list)
                        {
                            if (!new string[] { "jar", "litemod", "disabled", "old" }.Contains(PathUtils.GetExtension(text2)))
                            {
                                ModMain.Hint("一次请只拖入一个文件！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator;
                        ((IDisposable)enumerator).Dispose();
                    }
                }
                CS$<>8__locals2.$VB$Local_Extension = PathUtils.GetExtension(text);
                if (Operators.CompareString(CS$<>8__locals2.$VB$Local_Extension, "xaml", false) == 0)
                {
                    Logger.Info("文件后缀为 XAML，作为主页加载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    if (!FileUtils.Exists(Paths.Base + "PCL\\Custom.xaml") || ModMain.MyMsgBox("已存在一个主页文件，是否要将它覆盖？", "覆盖确认", "覆盖", "取消", "", false, true, false, null, null, null) != 2)
                    {
                        FileUtils.Copy(text, Paths.Base + "PCL\\Custom.xaml");
                        ModBase.RunInUi((FormMain._Closure$__.$I25-1 == null) ? (FormMain._Closure$__.$I25-1 = delegate
                        {
                            Settings.Set("UiCustomType", 1, null);
                            ModMain.FrmLaunchRight.ForceRefresh();
                            ModMain.Hint("已加载主页自定义文件！", ModMain.HintType.Green, true);
                        }) : FormMain._Closure$__.$I25-1, false);
                        return;
                    }
                }
                else if (!PageInstanceMod.InstallMods(list))
                {
                    if (new string[] { "zip", "rar", "mrpack" }.Any<string>((string t) => Operators.CompareString(t, CS$<>8__locals2.$VB$Local_Extension, false) == 0))
                    {
                        ModMain.Hint("正在读取压缩包……", ModMain.HintType.Blue, true);
                        try
                        {
                            Logger.Info("文件为压缩包，尝试作为整合包安装", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                            ModModpack.ModpackInstall(text, null, null);
                            ModBase.RunInUi((FormMain._Closure$__.$I25-3 == null) ? (FormMain._Closure$__.$I25-3 = delegate
                            {
                                ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.Default);
                            }) : FormMain._Closure$__.$I25-3, false);
                            return;
                        }
                        catch (Exception ex)
                        {
                            if (Operators.CompareString(ex.Message, "为遵守相关法律法规，PCL 暂时不再提供 MC 下载功能，非常抱歉 qwq……", false) == 0)
                            {
                                ModMain.Hint(ex.Message, ModMain.HintType.Red, true);
                                return;
                            }
                            if (ex.IsCanceled())
                            {
                                return;
                            }
                        }
                    }
                    if (Operators.CompareString(CS$<>8__locals2.$VB$Local_Extension, "rar", false) == 0)
                    {
                        ModMain.Hint("PCL 无法处理 rar 格式的压缩包，请在解压后重新压缩为 zip 格式再试！", ModMain.HintType.Red, true);
                        return;
                    }
                    try
                    {
                        Logger.Info("尝试进行错误报告分析", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                        CrashAnalyzer crashAnalyzer = new CrashAnalyzer(null);
                        crashAnalyzer.Import(text);
                        if (crashAnalyzer.Prepare())
                        {
                            crashAnalyzer.Analyze();
                            crashAnalyzer.Output(true, new List<string>());
                            return;
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Error(ex2, "自主错误报告分析失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    }
                    ModMain.Hint("PCL 无法确定适合的操作……", ModMain.HintType.Red, true);
                }
            }, "文件拖拽", ThreadPriority.Normal);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 30)
            {
                DateTime now = DateTime.Now;
                if (DateTime.Compare(now.Date, ModBase.ApplicationOpenTime.Date) == 0)
                {
                    Logger.Info(string.Format("系统时间微调为：{0} {1}", now.ToLongDateString(), now.ToLongTimeString()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    this.IsSystemTimeChanged = false;
                }
                else
                {
                    Logger.Info(string.Format("系统时间修改为：{0} {1}", now.ToLongDateString(), now.ToLongTimeString()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    this.IsSystemTimeChanged = true;
                }
            }
            else if (msg == 6402)
            {
                Logger.Info(string.Format("收到置顶信息：{0}", hwnd.ToInt64()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                if (!this.IsWindowLoadFinished)
                {
                    Logger.Info("窗口尚未加载完成，忽略置顶请求", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    return IntPtr.Zero;
                }
                this.ShowWindowToTop();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public bool Hidden
        {
            get
            {
                return this._Hidden;
            }
            set
            {
                if (this._Hidden != value)
                {
                    this._Hidden = value;
                    if (value)
                    {
                        base.Left -= 10000.0;
                        base.ShowInTaskbar = false;
                        base.Visibility = Visibility.Hidden;
                        Logger.Info(string.Format("窗口已隐藏，位置：({0},{1})", base.Left, base.Top), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                        return;
                    }
                    if (base.Left < -2000.0)
                    {
                        base.Left += 10000.0;
                    }
                    this.ShowWindowToTop();
                }
            }
        }

        public void ShowWindowToTop()
        {
            ModBase.RunInUi(delegate
            {
                base.Visibility = Visibility.Visible;
                base.ShowInTaskbar = true;
                base.WindowState = WindowState.Normal;
                this.Hidden = false;
                base.Topmost = true;
                base.Topmost = false;
                ModMain.SetForegroundWindow(ModBase.Handle);
                base.Focus();
                Logger.Info(string.Format("窗口已置顶，位置：({0}, {1}), {2} x {3}", new object[] { base.Left, base.Top, base.Width, base.Height }), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }, false);
        }

        private string PageNameGet(FormMain.PageStackData Stack)
        {
            string text;
            switch (Stack.Page)
            {
            case FormMain.PageType.InstanceSelect:
                text = "版本选择";
                break;
            case FormMain.PageType.TaskManager:
                text = "任务管理";
                break;
            case FormMain.PageType.InstanceSetup:
                text = "版本设置 - " + ((PageInstanceLeft.Instance == null) ? "未知版本" : PageInstanceLeft.Instance.Name);
                break;
            case FormMain.PageType.ResourceDetail:
                text = ((ResourceProject)NewLateBinding.LateIndexGet(Stack.Additional, new object[] { 0 }, null)).TranslatedName;
                break;
            case FormMain.PageType.HelpDetail:
                text = ((ModMain.HelpEntry)NewLateBinding.LateIndexGet(Stack.Additional, new object[] { 0 }, null)).Title;
                break;
            default:
                text = "";
                break;
            }
            return text;
        }

        public void PageNameRefresh(FormMain.PageStackData Type)
        {
            this.LabTitleInner.Text = this.PageNameGet(Type);
        }

        public void PageNameRefresh()
        {
            this.PageNameRefresh(this.PageCurrent);
        }

        public FormMain.PageSubType PageCurrentSub
        {
            get
            {
                FormMain.PageStackData pageCurrent = this.PageCurrent;
                FormMain.PageSubType pageSubType;
                if (pageCurrent == FormMain.PageType.Download)
                {
                    if (ModMain.FrmDownloadLeft == null)
                    {
                        ModMain.FrmDownloadLeft = new PageDownloadLeft();
                    }
                    pageSubType = ModMain.FrmDownloadLeft.PageID;
                }
                else if (pageCurrent == FormMain.PageType.Setup)
                {
                    if (ModMain.FrmSetupLeft == null)
                    {
                        ModMain.FrmSetupLeft = new PageSetupLeft();
                    }
                    pageSubType = ModMain.FrmSetupLeft.PageID;
                }
                else if (pageCurrent == FormMain.PageType.Other)
                {
                    if (ModMain.FrmOtherLeft == null)
                    {
                        ModMain.FrmOtherLeft = new PageOtherLeft();
                    }
                    pageSubType = ModMain.FrmOtherLeft.PageID;
                }
                else if (pageCurrent == FormMain.PageType.InstanceSetup)
                {
                    if (ModMain.FrmInstanceLeft == null)
                    {
                        ModMain.FrmInstanceLeft = new PageInstanceLeft();
                    }
                    pageSubType = ModMain.FrmInstanceLeft.PageID;
                }
                else
                {
                    pageSubType = FormMain.PageSubType.Default;
                }
                return pageSubType;
            }
        }

        public void PageChange(FormMain.PageStackData Stack, FormMain.PageSubType SubType = FormMain.PageSubType.Default)
        {
            if (Operators.CompareString(this.PageNameGet(Stack), "", false) == 0)
            {
                this.PageChangeExit();
                this.IsChangingPage = true;
                ((MyRadioButton)this.PanTitleSelect.Children[(int)Stack]).SetChecked(true, true, Operators.CompareString(this.PageNameGet(this.PageCurrent), "", false) == 0);
                this.IsChangingPage = false;
                switch (Stack.Page)
                {
                case FormMain.PageType.Download:
                    if (ModMain.FrmDownloadLeft == null)
                    {
                        ModMain.FrmDownloadLeft = new PageDownloadLeft();
                    }
                    ((MyListItem)ModMain.FrmDownloadLeft.PanItem.Children[(int)SubType]).SetChecked(true, true, Stack == this.PageCurrent);
                    break;
                case FormMain.PageType.Setup:
                    if (ModMain.FrmSetupLeft == null)
                    {
                        ModMain.FrmSetupLeft = new PageSetupLeft();
                    }
                    ((MyListItem)ModMain.FrmSetupLeft.PanItem.Children[(int)SubType]).SetChecked(true, true, Stack == this.PageCurrent);
                    break;
                case FormMain.PageType.Other:
                    if (ModMain.FrmOtherLeft == null)
                    {
                        ModMain.FrmOtherLeft = new PageOtherLeft();
                    }
                    ((MyListItem)ModMain.FrmOtherLeft.PanItem.Children[(int)SubType]).SetChecked(true, true, Stack == this.PageCurrent);
                    break;
                }
                this.PageChangeActual(Stack, SubType);
                return;
            }
            FormMain.PageType page = Stack.Page;
            if (page == FormMain.PageType.InstanceSetup)
            {
                if (ModMain.FrmInstanceLeft == null)
                {
                    ModMain.FrmInstanceLeft = new PageInstanceLeft();
                }
                ((MyListItem)ModMain.FrmInstanceLeft.PanItem.Children[(int)SubType]).SetChecked(true, true, Stack == this.PageCurrent);
            }
            this.PageChangeActual(Stack, SubType);
        }

        private void BtnTitleSelect_Click(MyRadioButton sender, bool raiseByMouse)
        {
            if (!this.IsChangingPage)
            {
                this.PageChangeActual(checked((FormMain.PageType)Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(sender.Tag)))), (FormMain.PageSubType)(-1));
            }
        }

        public void PageBack()
        {
            if (this.PageStack.Any<FormMain.PageStackData>())
            {
                this.PageChangeActual(this.PageStack[0], (FormMain.PageSubType)(-1));
                return;
            }
            this.PageChange(FormMain.PageType.Launch, FormMain.PageSubType.Default);
        }

        private void PageChangeActual(FormMain.PageStackData Stack, FormMain.PageSubType SubType = (FormMain.PageSubType)(-1))
        {
            checked
            {
                if (!(this.PageCurrent == Stack) || (this.PageCurrentSub != SubType && SubType != (FormMain.PageSubType)(-1)))
                {
                    if (Stack.Page == FormMain.PageType.InstanceSetup && PageInstanceLeft.Instance != null && ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
                    {
                        ModMain.Hint("该版本正在被修改，暂时无法进入版本设置！", ModMain.HintType.Red, true);
                        return;
                    }
                    ModAnimation.AniControlEnabled++;
                    try
                    {
                        FormMain._Closure$__50-0 CS$<>8__locals1 = new FormMain._Closure$__50-0(CS$<>8__locals1);
                        CS$<>8__locals1.$VB$Me = this;
                        CS$<>8__locals1.$VB$Local_PageName = this.PageNameGet(Stack);
                        unchecked
                        {
                            if (Operators.CompareString(CS$<>8__locals1.$VB$Local_PageName, "", false) == 0)
                            {
                                this.PageChangeExit();
                            }
                            else if (this.PageStack.Any<FormMain.PageStackData>())
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaOpacity(this.LabTitleInner, -this.LabTitleInner.Opacity, 130, 0, null, false),
                                    ModAnimation.AaCode(delegate
                                    {
                                        CS$<>8__locals1.$VB$Me.LabTitleInner.Text = CS$<>8__locals1.$VB$Local_PageName;
                                    }, 0, true),
                                    ModAnimation.AaOpacity(this.LabTitleInner, 1.0, 150, 30, null, false)
                                }, "FrmMain Titlebar SubLayer", false);
                                if (this.PageStack.Contains(Stack))
                                {
                                    while (this.PageStack.Contains(Stack))
                                    {
                                        this.PageStack.RemoveAt(0);
                                    }
                                }
                                else
                                {
                                    this.PageStack.Insert(0, this.PageCurrent);
                                }
                            }
                            else
                            {
                                this.PanTitleInner.Visibility = Visibility.Visible;
                                this.PanTitleMain.IsHitTestVisible = false;
                                this.PanTitleInner.IsHitTestVisible = true;
                                this.PageNameRefresh(Stack);
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaOpacity(this.PanTitleMain, -this.PanTitleMain.Opacity, 150, 0, null, false),
                                    ModAnimation.AaX(this.PanTitleMain, 12.0 - this.PanTitleMain.Margin.Left, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                                    ModAnimation.AaOpacity(this.PanTitleInner, 1.0 - this.PanTitleInner.Opacity, 150, 200, null, false),
                                    ModAnimation.AaX(this.PanTitleInner, -this.PanTitleInner.Margin.Left, 350, 200, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false),
                                    ModAnimation.AaCode(delegate
                                    {
                                        this.PanTitleMain.Visibility = Visibility.Collapsed;
                                    }, 0, true)
                                }, "FrmMain Titlebar FirstLayer", false);
                                this.PageStack.Insert(0, this.PageCurrent);
                            }
                            this.PageLast = this.PageCurrent;
                            this.PageCurrent = Stack;
                            switch (Stack.Page)
                            {
                            case FormMain.PageType.Launch:
                                this.PageChangeAnim(ModMain.FrmLaunchLeft, ModMain.FrmLaunchRight);
                                break;
                            case FormMain.PageType.Download:
                                if (ModMain.FrmDownloadLeft == null)
                                {
                                    ModMain.FrmDownloadLeft = new PageDownloadLeft();
                                }
                                this.PageChangeAnim(ModMain.FrmDownloadLeft, (FrameworkElement)ModMain.FrmDownloadLeft.PageGet(SubType));
                                break;
                            case FormMain.PageType.Link:
                                if (ModMain.FrmLinkMain == null)
                                {
                                    ModMain.FrmLinkMain = new PageLinkMain();
                                }
                                this.PageChangeAnim(new MyPageLeft(), ModMain.FrmLinkMain);
                                break;
                            case FormMain.PageType.Setup:
                                if (ModMain.FrmSetupLeft == null)
                                {
                                    ModMain.FrmSetupLeft = new PageSetupLeft();
                                }
                                this.PageChangeAnim(ModMain.FrmSetupLeft, (FrameworkElement)ModMain.FrmSetupLeft.PageGet(SubType));
                                break;
                            case FormMain.PageType.Other:
                                if (ModMain.FrmOtherLeft == null)
                                {
                                    ModMain.FrmOtherLeft = new PageOtherLeft();
                                }
                                this.PageChangeAnim(ModMain.FrmOtherLeft, (FrameworkElement)ModMain.FrmOtherLeft.PageGet(SubType));
                                break;
                            case FormMain.PageType.InstanceSelect:
                                if (ModMain.FrmSelectLeft == null)
                                {
                                    ModMain.FrmSelectLeft = new PageSelectLeft();
                                }
                                if (ModMain.FrmSelectRight == null)
                                {
                                    ModMain.FrmSelectRight = new PageSelectRight();
                                }
                                this.PageChangeAnim(ModMain.FrmSelectLeft, ModMain.FrmSelectRight);
                                break;
                            case FormMain.PageType.TaskManager:
                                if (ModMain.FrmSpeedLeft == null)
                                {
                                    ModMain.FrmSpeedLeft = new PageSpeedLeft();
                                }
                                if (ModMain.FrmSpeedRight == null)
                                {
                                    ModMain.FrmSpeedRight = new PageSpeedRight();
                                }
                                this.PageChangeAnim(ModMain.FrmSpeedLeft, ModMain.FrmSpeedRight);
                                break;
                            case FormMain.PageType.InstanceSetup:
                                if (ModMain.FrmInstanceLeft == null)
                                {
                                    ModMain.FrmInstanceLeft = new PageInstanceLeft();
                                }
                                this.PageChangeAnim(ModMain.FrmInstanceLeft, (FrameworkElement)ModMain.FrmInstanceLeft.PageGet(SubType));
                                break;
                            case FormMain.PageType.ResourceDetail:
                                if (ModMain.FrmDownloadResourceDetail == null)
                                {
                                    ModMain.FrmDownloadResourceDetail = new PageDownloadResourceDetail();
                                }
                                this.PageChangeAnim(new MyPageLeft(), ModMain.FrmDownloadResourceDetail);
                                break;
                            case FormMain.PageType.HelpDetail:
                                this.PageChangeAnim(new MyPageLeft(), (FrameworkElement)NewLateBinding.LateIndexGet(Stack.Additional, new object[] { 1 }, null));
                                break;
                            }
                            this.BtnExtraDownload.ShowRefresh();
                            this.BtnExtraApril.ShowRefresh();
                            Logger.Info(string.Format("切换主要页面：{0}, {1}", Stack.Page, SubType), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("切换主要页面失败（ID {0}）", this.PageCurrent.Page), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
                    }
                    finally
                    {
                        ModAnimation.AniControlEnabled--;
                    }
                }
            }
        }

        private void PageChangeAnim(FrameworkElement TargetLeft, FrameworkElement TargetRight)
        {
            ModAnimation.AniStop("FrmMain LeftChange");
            ModAnimation.AniStop("PageLeft PageChange");
            checked
            {
                ModAnimation.AniControlEnabled++;
                if (TargetLeft.Parent != null)
                {
                    TargetLeft.SetValue(ContentPresenter.ContentProperty, null);
                }
                if (TargetRight != null && TargetRight.Parent != null)
                {
                    TargetRight.SetValue(ContentPresenter.ContentProperty, null);
                }
                this.PageLeft = (MyPageLeft)TargetLeft;
                this.PageRight = (MyPageRight)TargetRight;
                ((MyPageLeft)this.PanMainLeft.Child).TriggerHideAnimation();
                ((MyPageRight)this.PanMainRight.Child).PageOnExit();
                ModAnimation.AniControlEnabled--;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaCode(delegate
                    {
                        ModAnimation.AniControlEnabled++;
                        this.PanMainLeft.Child = this.PageLeft;
                        this.PageLeft.Opacity = 0.0;
                        this.PanMainLeft.Background = null;
                        ModAnimation.AniControlEnabled--;
                        ModBase.RunInUi(delegate
                        {
                            this.PanMainLeft_Resize(this.PanMainLeft.ActualWidth);
                        }, true);
                    }, 110, false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PageLeft.Opacity = 1.0;
                        this.PageLeft.TriggerShowAnimation();
                    }, 30, true)
                }, "FrmMain PageChangeLeft", false);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaCode(delegate
                    {
                        ModAnimation.AniControlEnabled++;
                        ((MyPageRight)this.PanMainRight.Child).PageOnForceExit();
                        this.PanMainRight.Child = this.PageRight;
                        this.PageRight.Opacity = 0.0;
                        this.PanMainRight.Background = null;
                        ModAnimation.AniControlEnabled--;
                        ModBase.RunInUi(delegate
                        {
                            this.BtnExtraBack.ShowRefresh();
                        }, true);
                    }, 110, false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PageRight.Opacity = 1.0;
                        this.PageRight.PageOnEnter();
                    }, 30, true)
                }, "FrmMain PageChangeRight", false);
            }
        }

        private void PageChangeExit()
        {
            if (this.PageStack.Any<FormMain.PageStackData>())
            {
                this.PanTitleMain.Visibility = Visibility.Visible;
                this.PanTitleMain.IsHitTestVisible = true;
                this.PanTitleInner.IsHitTestVisible = false;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.PanTitleInner, -this.PanTitleInner.Opacity, 150, 0, null, false),
                    ModAnimation.AaX(this.PanTitleInner, -18.0 - this.PanTitleInner.Margin.Left, 150, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaOpacity(this.PanTitleMain, 1.0 - this.PanTitleMain.Opacity, 150, 200, null, false),
                    ModAnimation.AaX(this.PanTitleMain, -this.PanTitleMain.Margin.Left, 350, 200, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanTitleInner.Visibility = Visibility.Collapsed;
                    }, 0, true)
                }, "FrmMain Titlebar FirstLayer", false);
                this.PageStack.Clear();
            }
        }

        private void PanMainLeft_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                this.PanMainLeft_Resize(e.NewSize.Width);
            }
        }

        private void PanMainLeft_Resize(double NewWidth)
        {
            if (Math.Abs(NewWidth - this.RectLeftBackground.Width) <= 0.1 || ModAnimation.AniControlEnabled != 0)
            {
                this.RectLeftBackground.Width = NewWidth;
                this.PanMainLeft.IsHitTestVisible = true;
                ModAnimation.AniStop("FrmMain LeftChange");
                return;
            }
            if (this.PanMain.Opacity < 0.1)
            {
                this.PanMainLeft.IsHitTestVisible = false;
            }
            if (NewWidth > 0.0)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaWidth(this.RectLeftBackground, NewWidth - this.RectLeftBackground.Width, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false),
                    ModAnimation.AaOpacity(this.RectLeftShadow, 1.0 - this.RectLeftShadow.Opacity, 180, 0, null, false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanMainLeft.IsHitTestVisible = true;
                    }, 150, false)
                }, "FrmMain LeftChange", true);
                return;
            }
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaWidth(this.RectLeftBackground, -this.RectLeftBackground.Width, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaOpacity(this.RectLeftShadow, -this.RectLeftShadow.Opacity, 180, 0, null, false),
                ModAnimation.AaCode(delegate
                {
                    this.PanMainLeft.IsHitTestVisible = true;
                }, 150, false)
            }, "FrmMain LeftChange", true);
        }

        public void DragTick()
        {
            if (ModMain.DragControl != null && Mouse.LeftButton != MouseButtonState.Pressed)
            {
                this.DragStop();
            }
        }

        public void DragDoing()
        {
            if (ModMain.DragControl != null)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    NewLateBinding.LateCall(ModMain.DragControl, null, "DragDoing", new object[0], null, null, null, true);
                    return;
                }
                this.DragStop();
            }
        }

        public void DragStop()
        {
            ModBase.RunInUi((FormMain._Closure$__.$I57-0 == null) ? (FormMain._Closure$__.$I57-0 = delegate
            {
                if (ModMain.DragControl != null)
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(ModMain.DragControl);
                    ModMain.DragControl = null;
                    NewLateBinding.LateCall(objectValue, null, "DragStop", new object[0], null, null, null, true);
                }
            }) : FormMain._Closure$__.$I57-0, false);
        }

        private void BtnExtraMusic_Click(object sender, EventArgs e)
        {
            ModMusic.MusicControlPause();
        }

        private void BtnExtraMusic_RightClick(object sender, EventArgs e)
        {
            ModMusic.MusicControlNext();
        }

        private void BtnExtraDownload_Click(object sender, EventArgs e)
        {
            this.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.Default);
        }

        private bool BtnExtraDownload_ShowCheck()
        {
            return ModNet.HasDownloadingTask(false) && !(this.PageCurrent == FormMain.PageType.TaskManager);
        }

        public void AprilGiveup()
        {
            if (ModMain.IsAprilEnabled && !ModMain.IsAprilGiveup)
            {
                ModMain.Hint("=D", ModMain.HintType.Green, true);
                ModMain.IsAprilGiveup = true;
                ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleX = 1.0;
                ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleY = 1.0;
                this.BtnExtraApril.ShowRefresh();
            }
        }

        public bool BtnExtraApril_ShowCheck()
        {
            return ModMain.IsAprilEnabled && !ModMain.IsAprilGiveup && this.PageCurrent == FormMain.PageType.Launch;
        }

        public void BtnExtraShutdown_Click()
        {
            try
            {
                if (ModLaunch.McLaunchLoaderReal != null)
                {
                    ModLaunch.McLaunchLoaderReal.Cancel();
                }
                try
                {
                    foreach (ModWatcher.Watcher watcher in ModWatcher.McWatcherList)
                    {
                        watcher.Kill();
                    }
                }
                finally
                {
                    List<ModWatcher.Watcher>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                ModMain.Hint("已关闭运行中的 Minecraft！", ModMain.HintType.Green, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "强制关闭所有 Minecraft 失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
            }
        }

        public bool BtnExtraShutdown_ShowCheck()
        {
            return ModWatcher.HasRunningMinecraft;
        }

        public void BackToTop()
        {
            MyScrollViewer myScrollViewer = this.BtnExtraBack_GetRealChild();
            if (myScrollViewer != null)
            {
                myScrollViewer.PerformVerticalOffsetDelta(-myScrollViewer.VerticalOffset);
                return;
            }
            Logger.Error("无法返回顶部，未找到合适的 RealScroll", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\FormMain.xaml.vb");
        }

        private bool BtnExtraBack_ShowCheck()
        {
            MyScrollViewer myScrollViewer = this.BtnExtraBack_GetRealChild();
            return myScrollViewer != null && myScrollViewer.Visibility == Visibility.Visible && myScrollViewer.VerticalOffset > base.Height + (double)(this.BtnExtraBack.Show ? 0 : 700);
        }

        private MyScrollViewer BtnExtraBack_GetRealChild()
        {
            MyScrollViewer myScrollViewer;
            if (this.PanMainRight.Child == null || !(this.PanMainRight.Child is MyPageRight))
            {
                myScrollViewer = null;
            }
            else
            {
                MyPageRight myPageRight = (MyPageRight)this.PanMainRight.Child;
                myScrollViewer = (MyScrollViewer)myPageRight.FindName(myPageRight.PanScroll);
            }
            return myScrollViewer;
        }

        private void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            this.lastMouseArg = e;
        }

        internal virtual FormMain WindMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanBack
        {
            [CompilerGenerated]
            get
            {
                return this._PanBack;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseEventHandler mouseEventHandler = delegate(object a0, MouseEventArgs a1)
                {
                    this.DragDoing();
                };
                Grid grid = this._PanBack;
                if (grid != null)
                {
                    grid.MouseMove -= mouseEventHandler;
                }
                this._PanBack = value;
                grid = this._PanBack;
                if (grid != null)
                {
                    grid.MouseMove += mouseEventHandler;
                }
            }
        }

        internal virtual RotateTransform TransformRotate
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TranslateTransform TransformPos
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerT
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerB
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerR
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerL
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerLT
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerLB
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerRB
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle ResizerRT
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border BorderForm
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual RectangleGeometry RectForm
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanForm
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Canvas ImgBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanTitle
        {
            [CompilerGenerated]
            get
            {
                return this._PanTitle;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.FormDragMove);
                Grid grid = this._PanTitle;
                if (grid != null)
                {
                    grid.MouseLeftButtonDown -= mouseButtonEventHandler;
                }
                this._PanTitle = value;
                grid = this._PanTitle;
                if (grid != null)
                {
                    grid.MouseLeftButtonDown += mouseButtonEventHandler;
                }
            }
        }

        internal virtual MyImage ImgTitle
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnTitleClose
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleClose;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnTitleClose_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
                };
                MyIconButton myIconButton = this._BtnTitleClose;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnTitleClose = value;
                myIconButton = this._BtnTitleClose;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnTitleMin
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleMin;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnTitleMin_Click();
                };
                MyIconButton myIconButton = this._BtnTitleMin;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnTitleMin = value;
                myIconButton = this._BtnTitleMin;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual Grid PanTitleMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path ShapeTitleLogo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabTitleLogo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage ImageTitleLogo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanTitleSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioButton BtnTitleSelect0
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleSelect0;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.BtnTitleSelect_Click((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnTitleSelect0;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnTitleSelect0 = value;
                myRadioButton = this._BtnTitleSelect0;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioButton BtnTitleSelect1
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleSelect1;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.BtnTitleSelect_Click((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnTitleSelect1;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnTitleSelect1 = value;
                myRadioButton = this._BtnTitleSelect1;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioButton BtnTitleSelect2
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleSelect2;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.BtnTitleSelect_Click((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnTitleSelect2;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnTitleSelect2 = value;
                myRadioButton = this._BtnTitleSelect2;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioButton BtnTitleSelect3
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleSelect3;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.BtnTitleSelect_Click((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnTitleSelect3;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnTitleSelect3 = value;
                myRadioButton = this._BtnTitleSelect3;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioButton BtnTitleSelect4
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleSelect4;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.BtnTitleSelect_Click((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnTitleSelect4;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnTitleSelect4 = value;
                myRadioButton = this._BtnTitleSelect4;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual Grid PanTitleInner
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnTitleInner
        {
            [CompilerGenerated]
            get
            {
                return this._BtnTitleInner;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.PageBack();
                };
                MyIconButton myIconButton = this._BtnTitleInner;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnTitleInner = value;
                myIconButton = this._BtnTitleInner;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual TextBlock LabTitleInner
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanLeft
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle RectLeftBackground
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle RectLeftShadow
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border PanMainRight
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border PanMainLeft
        {
            [CompilerGenerated]
            get
            {
                return this._PanMainLeft;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this.PanMainLeft_SizeChanged);
                Border border = this._PanMainLeft;
                if (border != null)
                {
                    border.SizeChanged -= sizeChangedEventHandler;
                }
                this._PanMainLeft = value;
                border = this._PanMainLeft;
                if (border != null)
                {
                    border.SizeChanged += sizeChangedEventHandler;
                }
            }
        }

        internal virtual StackPanel PanHint
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyExtraButton BtnExtraBack
        {
            [CompilerGenerated]
            get
            {
                return this._BtnExtraBack;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyExtraButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BackToTop();
                };
                MyExtraButton myExtraButton = this._BtnExtraBack;
                if (myExtraButton != null)
                {
                    myExtraButton.Click -= clickEventHandler;
                }
                this._BtnExtraBack = value;
                myExtraButton = this._BtnExtraBack;
                if (myExtraButton != null)
                {
                    myExtraButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyExtraButton BtnExtraDownload
        {
            [CompilerGenerated]
            get
            {
                return this._BtnExtraDownload;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyExtraButton.ClickEventHandler clickEventHandler = new MyExtraButton.ClickEventHandler(this.BtnExtraDownload_Click);
                MyExtraButton myExtraButton = this._BtnExtraDownload;
                if (myExtraButton != null)
                {
                    myExtraButton.Click -= clickEventHandler;
                }
                this._BtnExtraDownload = value;
                myExtraButton = this._BtnExtraDownload;
                if (myExtraButton != null)
                {
                    myExtraButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyExtraButton BtnExtraApril
        {
            [CompilerGenerated]
            get
            {
                return this._BtnExtraApril;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyExtraButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.AprilGiveup();
                };
                MyExtraButton myExtraButton = this._BtnExtraApril;
                if (myExtraButton != null)
                {
                    myExtraButton.Click -= clickEventHandler;
                }
                this._BtnExtraApril = value;
                myExtraButton = this._BtnExtraApril;
                if (myExtraButton != null)
                {
                    myExtraButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyExtraButton BtnExtraShutdown
        {
            [CompilerGenerated]
            get
            {
                return this._BtnExtraShutdown;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyExtraButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnExtraShutdown_Click();
                };
                MyExtraButton myExtraButton = this._BtnExtraShutdown;
                if (myExtraButton != null)
                {
                    myExtraButton.Click -= clickEventHandler;
                }
                this._BtnExtraShutdown = value;
                myExtraButton = this._BtnExtraShutdown;
                if (myExtraButton != null)
                {
                    myExtraButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyExtraButton BtnExtraMusic
        {
            [CompilerGenerated]
            get
            {
                return this._BtnExtraMusic;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyExtraButton.ClickEventHandler clickEventHandler = new MyExtraButton.ClickEventHandler(this.BtnExtraMusic_Click);
                MyExtraButton.RightClickEventHandler rightClickEventHandler = new MyExtraButton.RightClickEventHandler(this.BtnExtraMusic_RightClick);
                MyExtraButton myExtraButton = this._BtnExtraMusic;
                if (myExtraButton != null)
                {
                    myExtraButton.Click -= clickEventHandler;
                    myExtraButton.RightClick -= rightClickEventHandler;
                }
                this._BtnExtraMusic = value;
                myExtraButton = this._BtnExtraMusic;
                if (myExtraButton != null)
                {
                    myExtraButton.Click += clickEventHandler;
                    myExtraButton.RightClick += rightClickEventHandler;
                }
            }
        }

        internal virtual Grid PanMsg
        {
            [CompilerGenerated]
            get
            {
                return this._PanMsg;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.FormDragMove);
                Grid grid = this._PanMsg;
                if (grid != null)
                {
                    grid.MouseLeftButtonDown -= mouseButtonEventHandler;
                }
                this._PanMsg = value;
                grid = this._PanMsg;
                if (grid != null)
                {
                    grid.MouseLeftButtonDown += mouseButtonEventHandler;
                }
            }
        }

        private bool IsWindowLoadFinished;

        private static bool IsLogShown = false;

        public bool IsSizeSaveable;

        public bool IsSystemTimeChanged;

        private bool _Hidden;

        public FormMain.PageStackData PageCurrent;

        public FormMain.PageStackData PageLast;

        public List<FormMain.PageStackData> PageStack;

        public MyPageLeft PageLeft;

        public MyPageRight PageRight;

        private bool IsChangingPage;

        public MouseEventArgs lastMouseArg;

        private DragHelper $STATIC$.ctor$2001$Helper;

        private StaticLocalInitFlag $STATIC$.ctor$2001$Helper$Init;

        private IDataObject $STATIC$HandleDrag$20211C1284DD$PrevData;

        private DragDropEffects $STATIC$HandleDrag$20211C1284DD$PrevEffects;

        public enum PageType
        {
            Launch,
            Download,
            Link,
            Setup,
            Other,
            InstanceSelect,
            TaskManager,
            InstanceSetup,
            ResourceDetail,
            HelpDetail
        }

        public enum PageSubType
        {
            Default,
            DownloadInstall = 0,
            DownloadMod = 2,
            DownloadPack,
            DownloadDataPack,
            DownloadResourcePack,
            DownloadShader,
            SetupLaunch = 0,
            SetupLink,
            SetupUI,
            SetupSystem,
            LinkMain = 0,
            OtherHelp = 0,
            OtherAbout,
            OtherTest,
            InstanceOverall = 0,
            InstanceSetup,
            InstanceModify,
            InstanceMod,
            InstanceModDisabled,
            InstanceExport
        }

        public class PageStackData
        {
            public override bool Equals(object other)
            {
                bool flag;
                if (other == null)
                {
                    flag = false;
                }
                else if (other is FormMain.PageStackData)
                {
                    FormMain.PageStackData pageStackData = (FormMain.PageStackData)other;
                    if (this.Page != pageStackData.Page)
                    {
                        flag = false;
                    }
                    else if (this.Additional == null)
                    {
                        flag = pageStackData.Additional == null;
                    }
                    else
                    {
                        flag = pageStackData.Additional != null && this.Additional.Equals(RuntimeHelpers.GetObjectValue(pageStackData.Additional));
                    }
                }
                else
                {
                    flag = other is int && !Operators.ConditionalCompareObjectNotEqual(this.Page, other, false) && this.Additional == null;
                }
                return flag;
            }

            public static bool operator ==(FormMain.PageStackData left, FormMain.PageStackData right)
            {
                return EqualityComparer<FormMain.PageStackData>.Default.Equals(left, right);
            }

            public static bool operator !=(FormMain.PageStackData left, FormMain.PageStackData right)
            {
                return !(left == right);
            }

            public static implicit operator FormMain.PageStackData(FormMain.PageType Value)
            {
                return new FormMain.PageStackData
                {
                    Page = Value
                };
            }

            public static implicit operator FormMain.PageType(FormMain.PageStackData Value)
            {
                return Value.Page;
            }

            public FormMain.PageType Page;

            public object Additional;
        }
    }
}
