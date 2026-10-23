using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSetupUI : MyPageRight
    {
        public PageSetupUI()
        {
            base.Loaded += this.PageSetupUI_Loaded;
            base.Loaded += ((PageSetupUI._Closure$__.$IR1-1 == null) ? (PageSetupUI._Closure$__.$IR1-1 = delegate(object a0, RoutedEventArgs a1)
            {
                PageSetupUI.HiddenRefresh();
            }) : PageSetupUI._Closure$__.$IR1-1);
            this.InitializeComponent();
        }

        private void PageSetupUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            ModSecret.ThemeCheckAll(true);
            if (ModSecret.ThemeDontClick != 0)
            {
                string text = null;
                int themeDontClick = ModSecret.ThemeDontClick;
                if (themeDontClick != 1)
                {
                    if (themeDontClick == 2)
                    {
                        text = "真·滑稽彩";
                    }
                }
                else
                {
                    text = "眼瞎白";
                }
                try
                {
                    foreach (object obj in this.PanLauncherTheme.Children)
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(obj);
                        if (objectValue is MyRadioBox && ((MyRadioBox)objectValue).IsEnabled)
                        {
                            ((MyRadioBox)objectValue).Text = text;
                        }
                    }
                }
                finally
                {
                    IEnumerator enumerator;
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            }
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.Refresh();
                ModAnimation.AniControlEnabled--;
                if (this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init, ref flag);
                    if (this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State == 0)
                    {
                        this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State = 2;
                        this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded = false;
                    }
                    else if (this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init);
                    }
                }
                if (!this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded)
                {
                    this.$STATIC$PageSetupUI_Loaded$20211C128141$Reloaded = true;
                    this.SliderLoad();
                    if (!this.RadioLauncherTheme8.IsEnabled)
                    {
                        this.LabLauncherTheme8Copy.ToolTip = string.Format("累积赞助达到 ¥23.33 后，在爱发电私信发送【土豆 {0}】以解锁。", ModBase.Identify) + "\r\n右键打开赞助页面，如果觉得 PCL 做得还不错就支持一下吧 =w=！";
                    }
                    this.RadioLauncherTheme8.ToolTip = string.Format("累积赞助达到 ¥23.33 后，在爱发电私信发送【土豆 {0}】以解锁", ModBase.Identify);
                    if (!this.RadioLauncherTheme9.IsEnabled)
                    {
                        this.LabLauncherTheme9Copy.ToolTip = "· 反馈一个 Bug，在标记为 [完成] 后回复识别码要求解锁（右键打开反馈页面）\r\n· 提交一个 Pull Request 或主页预设，在标记为 [完成] 后回复识别码要求解锁";
                    }
                    this.RadioLauncherTheme9.ToolTip = "· 反馈一个 Bug，在标记为 [完成] 后回复识别码要求解锁\r\n· 提交一个 Pull Request 或主页预设，在标记为 [完成] 后回复识别码要求解锁";
                }
            }
        }

        public void Refresh()
        {
            try
            {
                SettingService.RefreshSettings(this);
                PageSetupUI.BackgroundRefresh(false, false);
                this.CheckLogoLeft.Visibility = (this.RadioLogoType0.Checked ? Visibility.Visible : Visibility.Collapsed);
                this.PanLogoText.Visibility = (this.RadioLogoType2.Checked ? Visibility.Visible : Visibility.Collapsed);
                this.PanLogoChange.Visibility = (this.RadioLogoType3.Checked ? Visibility.Visible : Visibility.Collapsed);
                this.MusicRefreshUI();
                PageSetupUI.OnMainPageTypeChanged();
            }
            catch (NullReferenceException ex)
            {
                Logger.Error(ex, "个性化设置项存在异常，已被自动重置", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                this.Reset();
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "重载个性化设置时出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
            }
        }

        public void Reset()
        {
            try
            {
                SettingService.ResetSettings(this);
                Logger.Info("已初始化个性化设置！", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                ModMain.Hint("已初始化个性化设置", ModMain.HintType.Green, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化个性化设置失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
            }
            this.Refresh();
        }

        private void BtnUIBgOpen_Click(object sender, EventArgs e)
        {
            ModBase.OpenExplorer(Paths.Base + "PCL\\Pictures\\");
        }

        private void BtnBackgroundRefresh_Click(object sender, EventArgs e)
        {
            PageSetupUI.BackgroundRefresh(true, true);
        }

        public void BackgroundRefreshUI(bool Show, int Count)
        {
            if (!Information.IsNothing(this.PanBackgroundOpacity))
            {
                if (Show)
                {
                    this.PanBackgroundOpacity.Visibility = Visibility.Visible;
                    this.PanBackgroundBlur.Visibility = Visibility.Visible;
                    this.PanBackgroundSuit.Visibility = Visibility.Visible;
                    this.BtnBackgroundClear.Visibility = Visibility.Visible;
                    this.CardBackground.Title = "背景图片（" + Conversions.ToString(Count) + " 张）";
                }
                else
                {
                    this.PanBackgroundOpacity.Visibility = Visibility.Collapsed;
                    this.PanBackgroundBlur.Visibility = Visibility.Collapsed;
                    this.PanBackgroundSuit.Visibility = Visibility.Collapsed;
                    this.BtnBackgroundClear.Visibility = Visibility.Collapsed;
                    this.CardBackground.Title = "背景图片";
                }
                this.CardBackground.TriggerForceResize();
            }
        }

        private void BtnBackgroundClear_Click(object sender, EventArgs e)
        {
            if (ModMain.MyMsgBox("即将删除背景图片文件夹中的所有文件。\r\n此操作不可撤销，是否确定？", "警告", "确定", "取消", "", true, true, false, null, null, null) == 1)
            {
                DirectoryUtils.Delete(Paths.Base + "PCL\\Pictures", false);
                PageSetupUI.BackgroundRefresh(false, true);
                ModMain.Hint("背景图片已清空！", ModMain.HintType.Green, true);
            }
        }

        public static void BackgroundRefresh(bool IsHint, bool Refresh)
        {
            try
            {
                DirectoryUtils.Create(Paths.Base + "PCL\\Pictures\\");
                List<string> list = new List<string>();
                try
                {
                    foreach (string text in DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\\Pictures\\", true, "*"))
                    {
                        string extension = PathUtils.GetExtension(text);
                        if (Operators.CompareString(extension, "ini", false) != 0 && Operators.CompareString(extension, "db", false) != 0)
                        {
                            list.Add(text);
                        }
                    }
                }
                finally
                {
                    IEnumerator<string> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                if (!list.Any<string>())
                {
                    if (Refresh)
                    {
                        if (ModMain.FrmMain.ImgBack.Visibility == Visibility.Collapsed)
                        {
                            if (IsHint)
                            {
                                ModMain.Hint("未检测到可用背景图片！", ModMain.HintType.Red, true);
                            }
                        }
                        else
                        {
                            ModMain.FrmMain.ImgBack.Visibility = Visibility.Collapsed;
                            if (IsHint)
                            {
                                ModMain.Hint("背景图片已清除！", ModMain.HintType.Green, true);
                            }
                        }
                    }
                    if (!Information.IsNothing(ModMain.FrmSetupUI))
                    {
                        ModMain.FrmSetupUI.BackgroundRefreshUI(false, 0);
                    }
                }
                else
                {
                    if (Refresh)
                    {
                        string text2 = ModBase.RandomOne<string>(list);
                        try
                        {
                            Logger.Info(string.Format("加载背景图片：{0}", text2), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                            ModMain.FrmMain.ImgBack.Background = new MyBitmap(text2);
                            ModMain.FrmMain.ImgBack.Visibility = Visibility.Visible;
                            ModMain.FrmMain.UpdateBackgroundAndTitleBar();
                            if (IsHint)
                            {
                                ModMain.Hint("背景图片已刷新：" + PathUtils.GetLastPart(text2), ModMain.HintType.Green, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("参数无效"))
                            {
                                Logger.Error(string.Format("刷新背景图片失败，该图片文件可能并非标准格式。{0}你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。{1}文件：{2}", "\r\n", "\r\n", text2), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                            }
                            else
                            {
                                Logger.Error(ex, string.Format("刷新背景图片失败（{0}）", text2), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                            }
                        }
                    }
                    if (!Information.IsNothing(ModMain.FrmSetupUI))
                    {
                        ModMain.FrmSetupUI.BackgroundRefreshUI(true, list.Count);
                    }
                }
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "刷新背景图片时出现未知错误", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
            }
        }

        private void BtnLogoChange_Click(object sender, EventArgs e)
        {
            string text = Dialogs.SelectFile("选择图片", false, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?(new ValueTuple<string[], string>[]
            {
                new ValueTuple<string[], string>(new string[] { "png", "jpg", "jpeg", "gif", "webp" }, "常用图片文件")
            })).FirstOrDefault<string>();
            if (!string.IsNullOrEmpty(text))
            {
                string text2 = Paths.Base + "PCL\\Logo.png";
                try
                {
                    FileUtils.Copy(text, text2);
                    ModMain.FrmMain.ImageTitleLogo.Source = null;
                    ModMain.FrmMain.ImageTitleLogo.Source = text2;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("参数无效"))
                    {
                        Logger.Error(string.Format("改变标题栏图片失败，该图片文件可能并非标准格式。{0}你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。{1}文件：{2}", "\r\n", "\r\n", text2), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                    }
                    else
                    {
                        Logger.Error(ex, "设置标题栏图片失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                    }
                    ModMain.FrmMain.ImageTitleLogo.Source = null;
                }
            }
        }

        private void RadioLogoType3_Check(object sender, ModBase.RouteEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0 && e.RaiseByMouse)
            {
                for (;;)
                {
                    string text = Paths.Base + "PCL\\Logo.png";
                    if (FileUtils.Exists(text))
                    {
                        try
                        {
                            ModMain.FrmMain.ImageTitleLogo.Source = null;
                            ModMain.FrmMain.ImageTitleLogo.Source = text;
                            return;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("参数无效"))
                            {
                                Logger.Error(string.Format("改变标题栏图片失败，该图片文件可能并非标准格式。{0}你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。{1}文件：{2}", "\r\n", "\r\n", text), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                            }
                            else
                            {
                                Logger.Error(ex, "调整标题栏图片失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                            }
                            ModMain.FrmMain.ImageTitleLogo.Source = null;
                            e.Handled = true;
                            try
                            {
                                FileUtils.Delete(text, false);
                            }
                            catch (Exception ex2)
                            {
                                Logger.Error(ex2, "清理错误的标题栏图片失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                            }
                            return;
                        }
                    }
                    string text2 = Dialogs.SelectFile("选择图片", false, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?(new ValueTuple<string[], string>[]
                    {
                        new ValueTuple<string[], string>(new string[] { "png", "jpeg", "jpg", "gif", "webp" }, "常用图片文件")
                    })).FirstOrDefault<string>();
                    if (string.IsNullOrEmpty(text2))
                    {
                        break;
                    }
                    try
                    {
                        FileUtils.Copy(text2, text);
                        continue;
                    }
                    catch (Exception ex3)
                    {
                        Logger.Error(ex3, "复制标题栏图片失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                    }
                    return;
                }
                ModMain.FrmMain.ImageTitleLogo.Source = null;
                e.Handled = true;
                return;
            }
        }

        private void BtnLogoDelete_Click(object sender, EventArgs e)
        {
            try
            {
                FileUtils.Delete(Paths.Base + "PCL\\Logo.png", false);
                this.RadioLogoType1.SetChecked(true, true);
                ModMain.Hint("标题栏图片已清空！", ModMain.HintType.Green, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "清空标题栏图片失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
            }
        }

        private void BtnMusicOpen_Click(object sender, EventArgs e)
        {
            ModBase.OpenExplorer(Paths.Base + "PCL\\Musics\\");
        }

        private void BtnMusicRefresh_Click(object sender, EventArgs e)
        {
            ModMusic.MusicRefreshPlay(true, false);
        }

        public void MusicRefreshUI()
        {
            if (this.PanBackgroundOpacity != null)
            {
                if (ModMusic.MusicAllList.Any<string>())
                {
                    this.PanMusicVolume.Visibility = Visibility.Visible;
                    this.PanMusicDetail.Visibility = Visibility.Visible;
                    this.BtnMusicClear.Visibility = Visibility.Visible;
                    this.CardMusic.Title = "背景音乐（" + Conversions.ToString(DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\\Musics\\", true, "*").Count<string>((PageSetupUI._Closure$__.$I15-0 == null) ? (PageSetupUI._Closure$__.$I15-0 = (string f) => !new string[] { "ini", "jpg", "txt", "cfg", "lrc", "db", "png" }.Contains(PathUtils.GetExtension(f))) : PageSetupUI._Closure$__.$I15-0)) + " 首）";
                }
                else
                {
                    this.PanMusicVolume.Visibility = Visibility.Collapsed;
                    this.PanMusicDetail.Visibility = Visibility.Collapsed;
                    this.BtnMusicClear.Visibility = Visibility.Collapsed;
                    this.CardMusic.Title = "背景音乐";
                }
                this.CardMusic.TriggerForceResize();
            }
        }

        private void BtnMusicClear_Click(object sender, EventArgs e)
        {
            if (ModMain.MyMsgBox("即将删除背景音乐文件夹中的所有文件。\r\n此操作不可撤销，是否确定？", "警告", "确定", "取消", "", true, true, false, null, null, null) == 1)
            {
                ModBase.RunInThread((PageSetupUI._Closure$__.$I16-0 == null) ? (PageSetupUI._Closure$__.$I16-0 = delegate
                {
                    ModMain.Hint("正在删除背景音乐……", ModMain.HintType.Blue, true);
                    ModMusic.MusicNAudio = null;
                    ModMusic.MusicWaitingList = new List<string>();
                    ModMusic.MusicAllList = new List<string>();
                    Thread.Sleep(200);
                    try
                    {
                        DirectoryUtils.Delete(Paths.Base + "PCL\\Musics", false);
                        ModMain.Hint("背景音乐已删除！", ModMain.HintType.Green, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "删除背景音乐失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                    }
                    try
                    {
                        DirectoryUtils.Create(Paths.Base + "PCL\\Musics\\");
                        ModBase.RunInUi((PageSetupUI._Closure$__.$I16-1 == null) ? (PageSetupUI._Closure$__.$I16-1 = delegate
                        {
                            ModMusic.MusicRefreshPlay(false, false);
                        }) : PageSetupUI._Closure$__.$I16-1, false);
                    }
                    catch (Exception ex2)
                    {
                        Logger.Error(ex2, "重建背景音乐文件夹失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                    }
                }) : PageSetupUI._Closure$__.$I16-0);
            }
        }

        private void CheckMusicStart_Change()
        {
            if (ModAnimation.AniControlEnabled == 0 && this.CheckMusicStart.Checked)
            {
                this.CheckMusicStop.Checked = false;
            }
        }

        private void CheckMusicStop_Change()
        {
            if (ModAnimation.AniControlEnabled == 0 && this.CheckMusicStop.Checked)
            {
                this.CheckMusicStart.Checked = false;
            }
        }

        private void BtnCustomFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FileUtils.Exists(Paths.Base + "PCL\\Custom.xaml") || ModMain.MyMsgBox("当前已存在布局文件，继续生成教学文件将会覆盖现有布局文件！", "覆盖确认", "继续", "取消", "", true, true, false, null, null, null) != 2)
                {
                    ModBase.ExtractResources(Paths.Base + "PCL\\Custom.xaml", "Custom");
                    ModMain.Hint("教学文件已生成！", ModMain.HintType.Green, true);
                    ModBase.OpenExplorer(Paths.Base + "PCL\\Custom.xaml");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "生成教学文件失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
            }
        }

        private void BtnCustomRefresh_Click()
        {
            ModMain.FrmLaunchRight.ForceRefresh();
            ModMain.Hint("已刷新主页！", ModMain.HintType.Green, true);
        }

        private void BtnCustomTutorial_Click(object sender, EventArgs e)
        {
            ModMain.MyMsgBox("1. 点击 生成教学文件 按钮，这会在 PCL 文件夹下生成 Custom.xaml 布局文件。\r\n2. 使用记事本等工具打开这个文件并进行修改，修改完记得保存。\r\n3. 点击 刷新主页 按钮，查看主页现在长啥样了。\r\n\r\n你可以在生成教学文件后直接刷新主页，对照着进行修改，更有助于理解。\r\n直接将主页文件拖进 PCL 窗口也可以快捷加载。", "主页自定义教程", "确定", "", "", false, true, false, null, null, null);
        }

        public static void OnMainPageTypeChanged()
        {
            if (ModMain.FrmSetupUI != null)
            {
                switch (Settings.Get<int>("UiCustomType", null))
                {
                case 0:
                    ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.HintCustomWarn.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Visible;
                    ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Visible;
                    ModMain.FrmSetupUI.HintCustomWarn.Visibility = (Settings.Get<bool>("HintCustomWarn", null) ? Visibility.Collapsed : Visibility.Visible);
                    ModMain.FrmSetupUI.HintCustom.Text = string.Format("从 PCL 文件夹下的 Custom.xaml 读取主页内容。{0}你可以手动编辑该文件，向主页添加文本、图片、常用网站、快捷启动等功能。", "\r\n");
                    CustomEventService.SetEventType(ModMain.FrmSetupUI.HintCustom, CustomEvent.EventType.None);
                    break;
                case 2:
                    ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Visible;
                    ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Visible;
                    ModMain.FrmSetupUI.HintCustomWarn.Visibility = (Settings.Get<bool>("HintCustomWarn", null) ? Visibility.Collapsed : Visibility.Visible);
                    ModMain.FrmSetupUI.HintCustom.Text = string.Format("从指定网址联网获取主页内容。服主也可以用于动态更新服务器公告。{0}如果你制作了稳定运行的联网主页，可以点击这条提示投稿，若合格即可加入预设！", "\r\n");
                    CustomEventService.SetEventType(ModMain.FrmSetupUI.HintCustom, CustomEvent.EventType.打开网页);
                    CustomEventService.SetEventData(ModMain.FrmSetupUI.HintCustom, "https://github.com/Meloong-Git/PCL/discussions/2528");
                    break;
                case 3:
                    ModMain.FrmSetupUI.PanCustomPreset.Visibility = Visibility.Visible;
                    ModMain.FrmSetupUI.PanCustomLocal.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.PanCustomNet.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.HintCustom.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupUI.HintCustomWarn.Visibility = Visibility.Collapsed;
                    break;
                }
                ModMain.FrmSetupUI.CardCustom.TriggerForceResize();
            }
        }

        private void BtnCustomFolder_Click(object sender, MouseButtonEventArgs e)
        {
            string text = Paths.Base + "PCL\\Custom.xaml";
            if (FileUtils.Exists(text))
            {
                ModBase.OpenExplorer(text);
                return;
            }
            ModBase.OpenExplorer(Paths.Base + "PCL\\");
        }

        private void LabLauncherTheme5Unlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyRadioBox myRadioBox;
            (myRadioBox = this.RadioLauncherTheme5Gray).Opacity = myRadioBox.Opacity - 0.7;
            (myRadioBox = this.RadioLauncherTheme5).Opacity = myRadioBox.Opacity + 0.7;
            checked
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.RadioLauncherTheme5Gray, 1.0, (int)Math.Round(unchecked(1000.0 * ModAnimation.AniSpeed)), (int)Math.Round(unchecked(500.0 * ModAnimation.AniSpeed)), new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaOpacity(this.RadioLauncherTheme5, -1.0, (int)Math.Round(unchecked(1000.0 * ModAnimation.AniSpeed)), (int)Math.Round(unchecked(500.0 * ModAnimation.AniSpeed)), new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false)
                }, "ThemeUnlock", false);
                if (this.RadioLauncherTheme5Gray.Opacity < 0.02)
                {
                    ModSecret.ThemeUnlock(5, true, "隐藏主题 玄素黑 已解锁！");
                    ModAnimation.AniStop("ThemeUnlock");
                    this.RadioLauncherTheme5.Checked = true;
                }
            }
        }

        private void LabLauncherTheme11Click_MouseLeftButtonUp()
        {
            if ((this.LabLauncherTheme11Click.Visibility == Visibility.Collapsed || (this.LabLauncherTheme11Click.ToolTip ?? "").ToString().Contains("点击")) && ModMain.MyMsgBox("1. 不爬取或攻击相关服务或网站，不盗取相关账号，没有谜题可以或需要以此来解决。\r\n2. 不得篡改或损毁相关公开信息，请尽量让它们保持原状。\r\n3. 在你感到迷茫的时候，看看回声洞可能会给你带来惊喜。\r\n\r\n若违规，可能会被从任意相关群中踢出！", "解密游戏的基本规则", "我知道了", "恕我拒绝", "", false, true, false, null, null, null) == 1)
            {
                ModMain.MyMsgBox("你需要用自己的智慧来找到下一步的线索……\r\n初始线索：gnp.dorC61\\60\\20\\0202\\moc.x1xa.2s\\\\:sp" + "T".Lower() + "th", "解密游戏", "确定", "", "", false, true, false, null, null, null);
            }
        }

        private void LabLauncherTheme8Copy_MouseRightButtonUp()
        {
            ModBase.OpenWebsite("https://meloong.com/afd/a/LTCat");
        }

        private void LabLauncherTheme9Copy_MouseRightButtonUp()
        {
            PageOtherLeft.TryFeedback();
        }

        private void RadioLauncherTheme14_Change(object sender, ModBase.RouteEventArgs e)
        {
            if (this.RadioLauncherTheme14.Checked)
            {
                if (this.LabLauncherHue.Visibility == Visibility.Visible)
                {
                    return;
                }
                this.LabLauncherHue.Visibility = Visibility.Visible;
                this.SliderLauncherHue.Visibility = Visibility.Visible;
                this.LabLauncherSat.Visibility = Visibility.Visible;
                this.SliderLauncherSat.Visibility = Visibility.Visible;
                this.LabLauncherDelta.Visibility = Visibility.Visible;
                this.SliderLauncherDelta.Visibility = Visibility.Visible;
                this.LabLauncherLight.Visibility = Visibility.Visible;
                this.SliderLauncherLight.Visibility = Visibility.Visible;
            }
            else
            {
                if (this.LabLauncherHue.Visibility == Visibility.Collapsed)
                {
                    return;
                }
                this.LabLauncherHue.Visibility = Visibility.Collapsed;
                this.SliderLauncherHue.Visibility = Visibility.Collapsed;
                this.LabLauncherSat.Visibility = Visibility.Collapsed;
                this.SliderLauncherSat.Visibility = Visibility.Collapsed;
                this.LabLauncherDelta.Visibility = Visibility.Collapsed;
                this.SliderLauncherDelta.Visibility = Visibility.Collapsed;
                this.LabLauncherLight.Visibility = Visibility.Collapsed;
                this.SliderLauncherLight.Visibility = Visibility.Collapsed;
            }
            this.CardLauncher.TriggerForceResize();
        }

        private void HSL_Change()
        {
            if (ModAnimation.AniControlEnabled == 0 && this.SliderLauncherSat != null && this.SliderLauncherSat.IsLoaded)
            {
                ModSecret.ThemeRefresh(-1);
            }
        }

        public static bool HiddenForceShow
        {
            get
            {
                return PageSetupUI._HiddenForceShow;
            }
            set
            {
                PageSetupUI._HiddenForceShow = value;
                PageSetupUI.HiddenRefresh();
            }
        }

        public static void HiddenRefresh()
        {
            checked
            {
                if (ModMain.FrmMain.PanTitleSelect != null && ModMain.FrmMain.PanTitleSelect.IsLoaded)
                {
                    try
                    {
                        if (!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenPageDownload", null) && Settings.Get<bool>("UiHiddenPageLink", null) && Settings.Get<bool>("UiHiddenPageSetup", null) && Settings.Get<bool>("UiHiddenPageOther", null))
                        {
                            ModMain.FrmMain.PanTitleSelect.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ModMain.FrmMain.PanTitleSelect.Visibility = Visibility.Visible;
                            ModMain.FrmMain.BtnTitleSelect1.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenPageDownload", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmMain.BtnTitleSelect2.Visibility = Visibility.Collapsed;
                            ModMain.FrmMain.BtnTitleSelect3.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenPageSetup", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmMain.BtnTitleSelect4.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenPageOther", null)) ? Visibility.Collapsed : Visibility.Visible);
                        }
                        ModMain.FrmLaunchLeft.RefreshButtonsUI();
                        if (ModMain.FrmSetupUI != null)
                        {
                            ModMain.FrmSetupUI.CardSwitch.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenFunctionHidden", null)) ? Visibility.Collapsed : Visibility.Visible);
                        }
                        if (ModMain.FrmInstanceLeft != null)
                        {
                            ModMain.FrmInstanceLeft.ItemModify.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenFunctionInstanceModify", null)) ? Visibility.Collapsed : Visibility.Visible);
                        }
                        if (ModMain.FrmSetupLeft != null)
                        {
                            ModMain.FrmSetupLeft.ItemLaunch.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenSetupLaunch", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmSetupLeft.ItemUI.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenSetupUi", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmSetupLeft.ItemLink.Visibility = Visibility.Collapsed;
                            ModMain.FrmSetupLeft.ItemSystem.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenSetupSystem", null)) ? Visibility.Collapsed : Visibility.Visible);
                            int num = 0;
                            if (!Settings.Get<bool>("UiHiddenSetupLaunch", null))
                            {
                                num++;
                            }
                            if (!Settings.Get<bool>("UiHiddenSetupUi", null))
                            {
                                num++;
                            }
                            if (!Settings.Get<bool>("UiHiddenSetupSystem", null))
                            {
                                num++;
                            }
                            ModMain.FrmSetupLeft.PanItem.Visibility = ((num < 2 && !PageSetupUI.HiddenForceShow) ? Visibility.Collapsed : Visibility.Visible);
                        }
                        int num2 = 0;
                        if (!Settings.Get<bool>("UiHiddenOtherHelp", null))
                        {
                            num2++;
                        }
                        if (!Settings.Get<bool>("UiHiddenOtherAbout", null))
                        {
                            num2++;
                        }
                        if (!Settings.Get<bool>("UiHiddenOtherTest", null))
                        {
                            num2++;
                        }
                        if (!Settings.Get<bool>("UiHiddenOtherFeedback", null))
                        {
                            num2++;
                        }
                        if (!Settings.Get<bool>("UiHiddenOtherVote", null))
                        {
                            num2++;
                        }
                        if (ModMain.FrmOtherLeft != null)
                        {
                            ModMain.FrmOtherLeft.ItemHelp.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenOtherHelp", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmOtherLeft.ItemFeedback.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenOtherFeedback", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmOtherLeft.ItemVote.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenOtherVote", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmOtherLeft.ItemAbout.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenOtherAbout", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmOtherLeft.ItemTest.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenOtherTest", null)) ? Visibility.Collapsed : Visibility.Visible);
                            ModMain.FrmOtherLeft.PanItem.Visibility = ((num2 < 2 && !PageSetupUI.HiddenForceShow) ? Visibility.Collapsed : Visibility.Visible);
                        }
                        if (num2 == 1 && !PageSetupUI.HiddenForceShow)
                        {
                            if (!Settings.Get<bool>("UiHiddenOtherHelp", null))
                            {
                                ModMain.FrmMain.BtnTitleSelect4.Text = "帮助";
                            }
                            else if (!Settings.Get<bool>("UiHiddenOtherAbout", null))
                            {
                                ModMain.FrmMain.BtnTitleSelect4.Text = "关于";
                            }
                            else
                            {
                                ModMain.FrmMain.BtnTitleSelect4.Text = "百宝箱";
                            }
                        }
                        else
                        {
                            ModMain.FrmMain.BtnTitleSelect4.Text = "更多";
                        }
                        if (ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSelect)
                        {
                            ModMain.FrmSelectRight.BtnEmptyDownload_Loaded();
                        }
                        if (ModMain.FrmMain.PageCurrent == FormMain.PageType.Launch)
                        {
                            ModMain.FrmLaunchLeft.RefreshButtonsUI();
                        }
                        if (ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSetup && ModMain.FrmInstanceModDisabled != null)
                        {
                            ModMain.FrmInstanceModDisabled.BtnDownload_Loaded();
                        }
                        if (ModMain.FrmSetupUI != null)
                        {
                            ModMain.FrmSetupUI.CardSwitch.Title = (PageSetupUI.HiddenForceShow ? "功能隐藏（已暂时关闭，按 F12 以重新启用）" : "功能隐藏");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "刷新功能隐藏项目失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                    }
                }
            }
        }

        private void HiddenSetupMain()
        {
            if (this.CheckHiddenPageSetup.Checked)
            {
                this.CheckHiddenSetupLaunch.Checked = true;
                this.CheckHiddenSetupSystem.Checked = true;
                this.CheckHiddenSetupLink.Checked = true;
                this.CheckHiddenSetupUI.Checked = true;
                return;
            }
            if (Settings.Get<bool>("UiHiddenSetupLaunch", null) && Settings.Get<bool>("UiHiddenSetupUi", null) && Settings.Get<bool>("UiHiddenSetupSystem", null) && Settings.Get<bool>("UiHiddenSetupLink", null))
            {
                this.CheckHiddenSetupLaunch.Checked = false;
                this.CheckHiddenSetupSystem.Checked = false;
                this.CheckHiddenSetupLink.Checked = false;
                this.CheckHiddenSetupUI.Checked = false;
            }
        }

        private void HiddenSetupSub()
        {
            if (Settings.Get<bool>("UiHiddenSetupLaunch", null) && Settings.Get<bool>("UiHiddenSetupUi", null) && Settings.Get<bool>("UiHiddenSetupSystem", null) && Settings.Get<bool>("UiHiddenSetupLink", null))
            {
                this.CheckHiddenPageSetup.Checked = true;
                return;
            }
            this.CheckHiddenPageSetup.Checked = false;
        }

        private void HiddenOtherMain()
        {
            if (this.CheckHiddenPageOther.Checked)
            {
                this.CheckHiddenOtherAbout.Checked = true;
                this.CheckHiddenOtherTest.Checked = true;
                this.CheckHiddenOtherFeedback.Checked = true;
                this.CheckHiddenOtherVote.Checked = true;
                this.CheckHiddenOtherHelp.Checked = true;
                return;
            }
            if (Settings.Get<bool>("UiHiddenOtherHelp", null) && Settings.Get<bool>("UiHiddenOtherAbout", null) && Settings.Get<bool>("UiHiddenOtherTest", null) && Settings.Get<bool>("UiHiddenOtherVote", null) && Settings.Get<bool>("UiHiddenOtherFeedback", null))
            {
                this.CheckHiddenOtherAbout.Checked = false;
                this.CheckHiddenOtherTest.Checked = false;
                this.CheckHiddenOtherFeedback.Checked = false;
                this.CheckHiddenOtherVote.Checked = false;
                this.CheckHiddenOtherHelp.Checked = false;
            }
        }

        private void HiddenOtherSub(object sender, bool user)
        {
            if (Settings.Get<bool>("UiHiddenOtherHelp", null) && Settings.Get<bool>("UiHiddenOtherAbout", null) && Settings.Get<bool>("UiHiddenOtherTest", null))
            {
                this.CheckHiddenPageOther.Checked = true;
            }
            else
            {
                this.CheckHiddenPageOther.Checked = false;
            }
            if (user && Settings.Get<bool>("UiHiddenOtherHelp", null) && Settings.Get<bool>("UiHiddenOtherAbout", null) && Settings.Get<bool>("UiHiddenOtherTest", null))
            {
                this.CheckHiddenOtherFeedback.Checked = true;
                this.CheckHiddenOtherVote.Checked = true;
            }
        }

        private void HiddenOtherNet(object sender, bool user)
        {
            if (user && Settings.Get<bool>("UiHiddenOtherHelp", null) && Settings.Get<bool>("UiHiddenOtherAbout", null) && Settings.Get<bool>("UiHiddenOtherTest", null) && (!Settings.Get<bool>("UiHiddenOtherFeedback", null) || !Settings.Get<bool>("UiHiddenOtherVote", null)))
            {
                this.CheckHiddenOtherAbout.Checked = false;
                this.CheckHiddenOtherTest.Checked = false;
                this.CheckHiddenOtherHelp.Checked = false;
            }
        }

        private void HiddenHint(object sender, bool user)
        {
            if (Conversions.ToBoolean(ModAnimation.AniControlEnabled == 0 && Conversions.ToBoolean(NewLateBinding.LateGet(sender, null, "Checked", new object[0], null, null, null))))
            {
                ModMain.Hint("按 F12 即可暂时关闭功能隐藏设置。千万别忘了，要不然设置就改不回来了……", ModMain.HintType.Blue, true);
            }
        }

        private void SliderLoad()
        {
            this.SliderMusicVolume.GetHintText = ((PageSetupUI._Closure$__.$I41-0 == null) ? (PageSetupUI._Closure$__.$I41-0 = (object v) => Operators.ConcatenateObject(NewLateBinding.LateGet(null, typeof(Math), "Ceiling", new object[] { Operators.MultiplyObject(v, 0.1) }, null, null, null), "%")) : PageSetupUI._Closure$__.$I41-0);
            this.SliderLauncherTransparent.GetHintText = ((PageSetupUI._Closure$__.$I41-1 == null) ? (PageSetupUI._Closure$__.$I41-1 = (object v) => Operators.ConcatenateObject(NewLateBinding.LateGet(null, typeof(Math), "Round", new object[] { Operators.AddObject(40, Operators.MultiplyObject(v, 0.1)) }, null, null, null), "%")) : PageSetupUI._Closure$__.$I41-1);
            this.SliderLauncherHue.GetHintText = ((PageSetupUI._Closure$__.$I41-2 == null) ? (PageSetupUI._Closure$__.$I41-2 = (object v) => Operators.ConcatenateObject(v, "°")) : PageSetupUI._Closure$__.$I41-2);
            this.SliderLauncherSat.GetHintText = ((PageSetupUI._Closure$__.$I41-3 == null) ? (PageSetupUI._Closure$__.$I41-3 = (object v) => Operators.ConcatenateObject(v, "%")) : PageSetupUI._Closure$__.$I41-3);
            checked
            {
                this.SliderLauncherDelta.GetHintText = ((PageSetupUI._Closure$__.$I41-4 == null) ? (PageSetupUI._Closure$__.$I41-4 = delegate(int Value)
                {
                    string text;
                    if (Value > 90)
                    {
                        text = "+" + Conversions.ToString(Value - 90);
                    }
                    else if (Value == 90)
                    {
                        text = Conversions.ToString(0);
                    }
                    else
                    {
                        text = Conversions.ToString(Value - 90);
                    }
                    return text;
                }) : PageSetupUI._Closure$__.$I41-4);
                this.SliderLauncherLight.GetHintText = ((PageSetupUI._Closure$__.$I41-5 == null) ? (PageSetupUI._Closure$__.$I41-5 = delegate(int Value)
                {
                    string text2;
                    if (Value > 20)
                    {
                        text2 = "+" + Conversions.ToString(Value - 20);
                    }
                    else if (Value == 20)
                    {
                        text2 = Conversions.ToString(0);
                    }
                    else
                    {
                        text2 = Conversions.ToString(Value - 20);
                    }
                    return text2;
                }) : PageSetupUI._Closure$__.$I41-5);
                this.SliderBackgroundOpacity.GetHintText = ((PageSetupUI._Closure$__.$I41-6 == null) ? (PageSetupUI._Closure$__.$I41-6 = (object v) => Operators.ConcatenateObject(NewLateBinding.LateGet(null, typeof(Math), "Round", new object[] { Operators.MultiplyObject(v, 0.1) }, null, null, null), "%")) : PageSetupUI._Closure$__.$I41-6);
                this.SliderBackgroundBlur.GetHintText = ((PageSetupUI._Closure$__.$I41-7 == null) ? (PageSetupUI._Closure$__.$I41-7 = (object v) => Operators.ConcatenateObject(v, " 像素")) : PageSetupUI._Closure$__.$I41-7);
            }
        }

        internal virtual MyScrollViewer PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardLauncher
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderLauncherTransparent
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabLauncherHue
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderLauncherHue
        {
            [CompilerGenerated]
            get
            {
                return this._SliderLauncherHue;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MySlider.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HSL_Change();
                };
                MySlider mySlider = this._SliderLauncherHue;
                if (mySlider != null)
                {
                    mySlider.Change -= changeEventHandler;
                }
                this._SliderLauncherHue = value;
                mySlider = this._SliderLauncherHue;
                if (mySlider != null)
                {
                    mySlider.Change += changeEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherDelta
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderLauncherDelta
        {
            [CompilerGenerated]
            get
            {
                return this._SliderLauncherDelta;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MySlider.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HSL_Change();
                };
                MySlider mySlider = this._SliderLauncherDelta;
                if (mySlider != null)
                {
                    mySlider.Change -= changeEventHandler;
                }
                this._SliderLauncherDelta = value;
                mySlider = this._SliderLauncherDelta;
                if (mySlider != null)
                {
                    mySlider.Change += changeEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherSat
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderLauncherSat
        {
            [CompilerGenerated]
            get
            {
                return this._SliderLauncherSat;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MySlider.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HSL_Change();
                };
                MySlider mySlider = this._SliderLauncherSat;
                if (mySlider != null)
                {
                    mySlider.Change -= changeEventHandler;
                }
                this._SliderLauncherSat = value;
                mySlider = this._SliderLauncherSat;
                if (mySlider != null)
                {
                    mySlider.Change += changeEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherLight
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderLauncherLight
        {
            [CompilerGenerated]
            get
            {
                return this._SliderLauncherLight;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MySlider.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HSL_Change();
                };
                MySlider mySlider = this._SliderLauncherLight;
                if (mySlider != null)
                {
                    mySlider.Change -= changeEventHandler;
                }
                this._SliderLauncherLight = value;
                mySlider = this._SliderLauncherLight;
                if (mySlider != null)
                {
                    mySlider.Change += changeEventHandler;
                }
            }
        }

        internal virtual Grid PanLauncherTheme
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme0
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme1
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme3
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme4
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme5
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme5Gray
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabLauncherTheme5Unlock
        {
            [CompilerGenerated]
            get
            {
                return this._LabLauncherTheme5Unlock;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.LabLauncherTheme5Unlock_MouseLeftButtonUp);
                TextBlock textBlock = this._LabLauncherTheme5Unlock;
                if (textBlock != null)
                {
                    textBlock.MouseLeftButtonUp -= mouseButtonEventHandler;
                }
                this._LabLauncherTheme5Unlock = value;
                textBlock = this._LabLauncherTheme5Unlock;
                if (textBlock != null)
                {
                    textBlock.MouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual MyRadioBox RadioLauncherTheme12
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme6
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme7
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme13
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme8
        {
            [CompilerGenerated]
            get
            {
                return this._RadioLauncherTheme8;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.LabLauncherTheme8Copy_MouseRightButtonUp();
                };
                MyRadioBox myRadioBox = this._RadioLauncherTheme8;
                if (myRadioBox != null)
                {
                    myRadioBox.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._RadioLauncherTheme8 = value;
                myRadioBox = this._RadioLauncherTheme8;
                if (myRadioBox != null)
                {
                    myRadioBox.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherTheme8Copy
        {
            [CompilerGenerated]
            get
            {
                return this._LabLauncherTheme8Copy;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.LabLauncherTheme8Copy_MouseRightButtonUp();
                };
                TextBlock textBlock = this._LabLauncherTheme8Copy;
                if (textBlock != null)
                {
                    textBlock.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._LabLauncherTheme8Copy = value;
                textBlock = this._LabLauncherTheme8Copy;
                if (textBlock != null)
                {
                    textBlock.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual MyRadioBox RadioLauncherTheme9
        {
            [CompilerGenerated]
            get
            {
                return this._RadioLauncherTheme9;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.LabLauncherTheme9Copy_MouseRightButtonUp();
                };
                MyRadioBox myRadioBox = this._RadioLauncherTheme9;
                if (myRadioBox != null)
                {
                    myRadioBox.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._RadioLauncherTheme9 = value;
                myRadioBox = this._RadioLauncherTheme9;
                if (myRadioBox != null)
                {
                    myRadioBox.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherTheme9Copy
        {
            [CompilerGenerated]
            get
            {
                return this._LabLauncherTheme9Copy;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.LabLauncherTheme9Copy_MouseRightButtonUp();
                };
                TextBlock textBlock = this._LabLauncherTheme9Copy;
                if (textBlock != null)
                {
                    textBlock.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._LabLauncherTheme9Copy = value;
                textBlock = this._LabLauncherTheme9Copy;
                if (textBlock != null)
                {
                    textBlock.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual MyRadioBox RadioLauncherTheme10
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLauncherTheme11
        {
            [CompilerGenerated]
            get
            {
                return this._RadioLauncherTheme11;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.LabLauncherTheme11Click_MouseLeftButtonUp();
                };
                MyRadioBox myRadioBox = this._RadioLauncherTheme11;
                if (myRadioBox != null)
                {
                    myRadioBox.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._RadioLauncherTheme11 = value;
                myRadioBox = this._RadioLauncherTheme11;
                if (myRadioBox != null)
                {
                    myRadioBox.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherTheme11Click
        {
            [CompilerGenerated]
            get
            {
                return this._LabLauncherTheme11Click;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.LabLauncherTheme11Click_MouseLeftButtonUp();
                };
                TextBlock textBlock = this._LabLauncherTheme11Click;
                if (textBlock != null)
                {
                    textBlock.MouseLeftButtonUp -= mouseButtonEventHandler;
                }
                this._LabLauncherTheme11Click = value;
                textBlock = this._LabLauncherTheme11Click;
                if (textBlock != null)
                {
                    textBlock.MouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual MyRadioBox RadioLauncherTheme14
        {
            [CompilerGenerated]
            get
            {
                return this._RadioLauncherTheme14;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.ChangedEventHandler changedEventHandler = new IMyRadio.ChangedEventHandler(this.RadioLauncherTheme14_Change);
                MyRadioBox myRadioBox = this._RadioLauncherTheme14;
                if (myRadioBox != null)
                {
                    myRadioBox.Changed -= changedEventHandler;
                }
                this._RadioLauncherTheme14 = value;
                myRadioBox = this._RadioLauncherTheme14;
                if (myRadioBox != null)
                {
                    myRadioBox.Changed += changedEventHandler;
                }
            }
        }

        internal virtual Border PanLauncherHide
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardBackground
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanBackgroundSuit
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanBackgroundOpacity
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderBackgroundOpacity
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanBackgroundBlur
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderBackgroundBlur
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnBackgroundOpen
        {
            [CompilerGenerated]
            get
            {
                return this._BtnBackgroundOpen;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnUIBgOpen_Click);
                MyButton myButton = this._BtnBackgroundOpen;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnBackgroundOpen = value;
                myButton = this._BtnBackgroundOpen;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnBackgroundRefresh
        {
            [CompilerGenerated]
            get
            {
                return this._BtnBackgroundRefresh;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnBackgroundRefresh_Click);
                MyButton myButton = this._BtnBackgroundRefresh;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnBackgroundRefresh = value;
                myButton = this._BtnBackgroundRefresh;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnBackgroundClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnBackgroundClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnBackgroundClear_Click);
                MyButton myButton = this._BtnBackgroundClear;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnBackgroundClear = value;
                myButton = this._BtnBackgroundClear;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard CardMusic
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanMusicVolume
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderMusicVolume
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanMusicDetail
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckMusicRandom
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckMusicAuto
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckMusicStart
        {
            [CompilerGenerated]
            get
            {
                return this._CheckMusicStart;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.CheckMusicStart_Change();
                };
                MyCheckBox myCheckBox = this._CheckMusicStart;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckMusicStart = value;
                myCheckBox = this._CheckMusicStart;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckMusicStop
        {
            [CompilerGenerated]
            get
            {
                return this._CheckMusicStop;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.CheckMusicStop_Change();
                };
                MyCheckBox myCheckBox = this._CheckMusicStop;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckMusicStop = value;
                myCheckBox = this._CheckMusicStop;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyButton BtnMusicOpen
        {
            [CompilerGenerated]
            get
            {
                return this._BtnMusicOpen;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnMusicOpen_Click);
                MyButton myButton = this._BtnMusicOpen;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnMusicOpen = value;
                myButton = this._BtnMusicOpen;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnMusicRefresh
        {
            [CompilerGenerated]
            get
            {
                return this._BtnMusicRefresh;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnMusicRefresh_Click);
                MyButton myButton = this._BtnMusicRefresh;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnMusicRefresh = value;
                myButton = this._BtnMusicRefresh;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnMusicClear
        {
            [CompilerGenerated]
            get
            {
                return this._BtnMusicClear;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnMusicClear_Click);
                MyButton myButton = this._BtnMusicClear;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnMusicClear = value;
                myButton = this._BtnMusicClear;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard CardLogo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLogoType0
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLogoType1
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLogoType2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioLogoType3
        {
            [CompilerGenerated]
            get
            {
                return this._RadioLogoType3;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyRadioBox.PreviewCheckEventHandler previewCheckEventHandler = new MyRadioBox.PreviewCheckEventHandler(this.RadioLogoType3_Check);
                MyRadioBox myRadioBox = this._RadioLogoType3;
                if (myRadioBox != null)
                {
                    myRadioBox.PreviewCheck -= previewCheckEventHandler;
                }
                this._RadioLogoType3 = value;
                myRadioBox = this._RadioLogoType3;
                if (myRadioBox != null)
                {
                    myRadioBox.PreviewCheck += previewCheckEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckLogoLeft
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanLogoText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextLogoText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanLogoChange
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnLogoChange
        {
            [CompilerGenerated]
            get
            {
                return this._BtnLogoChange;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnLogoChange_Click);
                MyButton myButton = this._BtnLogoChange;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnLogoChange = value;
                myButton = this._BtnLogoChange;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnLogoDelete
        {
            [CompilerGenerated]
            get
            {
                return this._BtnLogoDelete;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnLogoDelete_Click);
                MyButton myButton = this._BtnLogoDelete;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnLogoDelete = value;
                myButton = this._BtnLogoDelete;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard CardCustom
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioCustomType0
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioCustomType3
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioCustomType1
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioCustomType2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintCustom
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintCustomWarn
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual WrapPanel PanCustomLocal
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnCustomRefresh
        {
            [CompilerGenerated]
            get
            {
                return this._BtnCustomRefresh;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnCustomRefresh_Click();
                };
                MyButton myButton = this._BtnCustomRefresh;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnCustomRefresh = value;
                myButton = this._BtnCustomRefresh;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnCustomFile
        {
            [CompilerGenerated]
            get
            {
                return this._BtnCustomFile;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnCustomFile_Click);
                MyButton myButton = this._BtnCustomFile;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnCustomFile = value;
                myButton = this._BtnCustomFile;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnCustomTutorial
        {
            [CompilerGenerated]
            get
            {
                return this._BtnCustomTutorial;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnCustomTutorial_Click);
                MyButton myButton = this._BtnCustomTutorial;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnCustomTutorial = value;
                myButton = this._BtnCustomTutorial;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnCustomFolder
        {
            [CompilerGenerated]
            get
            {
                return this._BtnCustomFolder;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnCustomFolder_Click);
                MyButton myButton = this._BtnCustomFolder;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnCustomFolder = value;
                myButton = this._BtnCustomFolder;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual Grid PanCustomNet
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextCustomNet
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanCustomPreset
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardSwitch
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckHiddenPageDownload
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckHiddenPageLink
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckHiddenPageSetup
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenPageSetup;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HiddenSetupMain();
                };
                MyCheckBox.ChangeEventHandler changeEventHandler2 = new MyCheckBox.ChangeEventHandler(this.HiddenHint);
                MyCheckBox myCheckBox = this._CheckHiddenPageSetup;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                    myCheckBox.Change -= changeEventHandler2;
                }
                this._CheckHiddenPageSetup = value;
                myCheckBox = this._CheckHiddenPageSetup;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                    myCheckBox.Change += changeEventHandler2;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenPageOther
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenPageOther;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HiddenOtherMain();
                };
                MyCheckBox myCheckBox = this._CheckHiddenPageOther;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenPageOther = value;
                myCheckBox = this._CheckHiddenPageOther;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenSetupLaunch
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenSetupLaunch;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HiddenSetupSub();
                };
                MyCheckBox myCheckBox = this._CheckHiddenSetupLaunch;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenSetupLaunch = value;
                myCheckBox = this._CheckHiddenSetupLaunch;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenSetupLink
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenSetupLink;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HiddenSetupSub();
                };
                MyCheckBox myCheckBox = this._CheckHiddenSetupLink;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenSetupLink = value;
                myCheckBox = this._CheckHiddenSetupLink;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenSetupUI
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenSetupUI;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HiddenSetupSub();
                };
                MyCheckBox.ChangeEventHandler changeEventHandler2 = new MyCheckBox.ChangeEventHandler(this.HiddenHint);
                MyCheckBox myCheckBox = this._CheckHiddenSetupUI;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                    myCheckBox.Change -= changeEventHandler2;
                }
                this._CheckHiddenSetupUI = value;
                myCheckBox = this._CheckHiddenSetupUI;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                    myCheckBox.Change += changeEventHandler2;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenSetupSystem
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenSetupSystem;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.HiddenSetupSub();
                };
                MyCheckBox myCheckBox = this._CheckHiddenSetupSystem;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenSetupSystem = value;
                myCheckBox = this._CheckHiddenSetupSystem;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenOtherHelp
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenOtherHelp;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.HiddenOtherSub);
                MyCheckBox myCheckBox = this._CheckHiddenOtherHelp;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherHelp = value;
                myCheckBox = this._CheckHiddenOtherHelp;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenOtherAbout
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenOtherAbout;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.HiddenOtherSub);
                MyCheckBox myCheckBox = this._CheckHiddenOtherAbout;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherAbout = value;
                myCheckBox = this._CheckHiddenOtherAbout;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenOtherTest
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenOtherTest;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.HiddenOtherSub);
                MyCheckBox myCheckBox = this._CheckHiddenOtherTest;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherTest = value;
                myCheckBox = this._CheckHiddenOtherTest;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenOtherFeedback
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenOtherFeedback;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.HiddenOtherNet);
                MyCheckBox myCheckBox = this._CheckHiddenOtherFeedback;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherFeedback = value;
                myCheckBox = this._CheckHiddenOtherFeedback;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenOtherVote
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenOtherVote;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.HiddenOtherNet);
                MyCheckBox myCheckBox = this._CheckHiddenOtherVote;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherVote = value;
                myCheckBox = this._CheckHiddenOtherVote;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckLauncherEmail
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckHiddenFunctionSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckHiddenFunctionModUpdate
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckHiddenFunctionHidden
        {
            [CompilerGenerated]
            get
            {
                return this._CheckHiddenFunctionHidden;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = new MyCheckBox.ChangeEventHandler(this.HiddenHint);
                MyCheckBox myCheckBox = this._CheckHiddenFunctionHidden;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenFunctionHidden = value;
                myCheckBox = this._CheckHiddenFunctionHidden;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckHiddenFunctionModify
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private static bool _HiddenForceShow = false;

        private bool $STATIC$PageSetupUI_Loaded$20211C128141$Reloaded;

        private StaticLocalInitFlag $STATIC$PageSetupUI_Loaded$20211C128141$Reloaded$Init;
    }
}
