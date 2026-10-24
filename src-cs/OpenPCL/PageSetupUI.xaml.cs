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
    public partial class PageSetupUI : OurPageRight
    {
        public PageSetupUI()
        {
            base.Loaded += this.PageSetupUI_Loaded;
            base.Loaded += ((PageSetupUI._Closure___._IR1_1 == null) ? (PageSetupUI._Closure___._IR1_1 = new RoutedEventHandler(PageSetupUI._Closure___._I._Lambda___R1_1)) : PageSetupUI._Closure___._IR1_1);
            this.InitializeComponent();
            this.SliderLauncherHue_WithEvents = this.SliderLauncherHue;
            this.SliderLauncherDelta_WithEvents = this.SliderLauncherDelta;
            this.SliderLauncherSat_WithEvents = this.SliderLauncherSat;
            this.SliderLauncherLight_WithEvents = this.SliderLauncherLight;
            this.LabLauncherTheme5Unlock_WithEvents = this.LabLauncherTheme5Unlock;
            this.RadioLauncherTheme8_WithEvents = this.RadioLauncherTheme8;
            this.LabLauncherTheme8Copy_WithEvents = this.LabLauncherTheme8Copy;
            this.RadioLauncherTheme9_WithEvents = this.RadioLauncherTheme9;
            this.LabLauncherTheme9Copy_WithEvents = this.LabLauncherTheme9Copy;
            this.RadioLauncherTheme11_WithEvents = this.RadioLauncherTheme11;
            this.LabLauncherTheme11Click_WithEvents = this.LabLauncherTheme11Click;
            this.RadioLauncherTheme14_WithEvents = this.RadioLauncherTheme14;
            this.BtnBackgroundOpen_WithEvents = this.BtnBackgroundOpen;
            this.BtnBackgroundRefresh_WithEvents = this.BtnBackgroundRefresh;
            this.BtnBackgroundClear_WithEvents = this.BtnBackgroundClear;
            this.CheckMusicStart_WithEvents = this.CheckMusicStart;
            this.CheckMusicStop_WithEvents = this.CheckMusicStop;
            this.BtnMusicOpen_WithEvents = this.BtnMusicOpen;
            this.BtnMusicRefresh_WithEvents = this.BtnMusicRefresh;
            this.BtnMusicClear_WithEvents = this.BtnMusicClear;
            this.RadioLogoType3_WithEvents = this.RadioLogoType3;
            this.BtnLogoChange_WithEvents = this.BtnLogoChange;
            this.BtnLogoDelete_WithEvents = this.BtnLogoDelete;
            this.BtnCustomRefresh_WithEvents = this.BtnCustomRefresh;
            this.BtnCustomFile_WithEvents = this.BtnCustomFile;
            this.BtnCustomTutorial_WithEvents = this.BtnCustomTutorial;
            this.BtnCustomFolder_WithEvents = this.BtnCustomFolder;
            this.CheckHiddenPageSetup_WithEvents = this.CheckHiddenPageSetup;
            this.CheckHiddenPageOther_WithEvents = this.CheckHiddenPageOther;
            this.CheckHiddenSetupLaunch_WithEvents = this.CheckHiddenSetupLaunch;
            this.CheckHiddenSetupLink_WithEvents = this.CheckHiddenSetupLink;
            this.CheckHiddenSetupUI_WithEvents = this.CheckHiddenSetupUI;
            this.CheckHiddenSetupSystem_WithEvents = this.CheckHiddenSetupSystem;
            this.CheckHiddenOtherHelp_WithEvents = this.CheckHiddenOtherHelp;
            this.CheckHiddenOtherAbout_WithEvents = this.CheckHiddenOtherAbout;
            this.CheckHiddenOtherTest_WithEvents = this.CheckHiddenOtherTest;
            this.CheckHiddenOtherFeedback_WithEvents = this.CheckHiddenOtherFeedback;
            this.CheckHiddenOtherVote_WithEvents = this.CheckHiddenOtherVote;
            this.CheckHiddenFunctionHidden_WithEvents = this.CheckHiddenFunctionHidden;
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
                        if (objectValue is OurRadioBox && ((OurRadioBox)objectValue).IsEnabled)
                        {
                            ((OurRadioBox)objectValue).Text = text;
                        }
                    }
                }
                finally
                {
                    IEnumerator enumerator = default;
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
                if (this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init, ref flag);
                    if (this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init.State == 0)
                    {
                        this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init.State = 2;
                        this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded = false;
                    }
                    else if (this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init);
                    }
                }
                if (!this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded)
                {
                    this._STATIC_PageSetupUI_Loaded_20211C128141_Reloaded = true;
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
            if (ModMain.OurMsgBox("即将删除背景图片文件夹中的所有文件。\r\n此操作不可撤销，是否确定？", "警告", "确定", "取消", "", true, true, false, null, null, null) == 1)
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
                    IEnumerator<string> enumerator = default;
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
                            ModMain.FrmMain.ImgBack.Background = new OurBitmap(text2);
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
                    this.CardMusic.Title = "背景音乐（" + Conversions.ToString(DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\\Musics\\", true, "*").Count<string>((PageSetupUI._Closure___._I15_0 == null) ? (PageSetupUI._Closure___._I15_0 = new Func<string, bool>(PageSetupUI._Closure___._I._Lambda___15_0)) : PageSetupUI._Closure___._I15_0)) + " 首）";
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
            if (ModMain.OurMsgBox("即将删除背景音乐文件夹中的所有文件。\r\n此操作不可撤销，是否确定？", "警告", "确定", "取消", "", true, true, false, null, null, null) == 1)
            {
                ModBase.RunInThread((PageSetupUI._Closure___._I16_0 == null) ? (PageSetupUI._Closure___._I16_0 = new Action(PageSetupUI._Closure___._I._Lambda___16_0)) : PageSetupUI._Closure___._I16_0);
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
                if (!FileUtils.Exists(Paths.Base + "PCL\\Custom.xaml") || ModMain.OurMsgBox("当前已存在布局文件，继续生成教学文件将会覆盖现有布局文件！", "覆盖确认", "继续", "取消", "", true, true, false, null, null, null) != 2)
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
            ModMain.OurMsgBox("1. 点击 生成教学文件 按钮，这会在 PCL 文件夹下生成 Custom.xaml 布局文件。\r\n2. 使用记事本等工具打开这个文件并进行修改，修改完记得保存。\r\n3. 点击 刷新主页 按钮，查看主页现在长啥样了。\r\n\r\n你可以在生成教学文件后直接刷新主页，对照着进行修改，更有助于理解。\r\n直接将主页文件拖进 PCL 窗口也可以快捷加载。", "主页自定义教程", "确定", "", "", false, true, false, null, null, null);
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
            OurRadioBox ourRadioBox;
            (ourRadioBox = this.RadioLauncherTheme5Gray).Opacity = ourRadioBox.Opacity - 0.7;
            (ourRadioBox = this.RadioLauncherTheme5).Opacity = ourRadioBox.Opacity + 0.7;
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
            if ((this.LabLauncherTheme11Click.Visibility == Visibility.Collapsed || (this.LabLauncherTheme11Click.ToolTip ?? "").ToString().Contains("点击")) && ModMain.OurMsgBox("1. 不爬取或攻击相关服务或网站，不盗取相关账号，没有谜题可以或需要以此来解决。\r\n2. 不得篡改或损毁相关公开信息，请尽量让它们保持原状。\r\n3. 在你感到迷茫的时候，看看回声洞可能会给你带来惊喜。\r\n\r\n若违规，可能会被从任意相关群中踢出！", "解密游戏的基本规则", "我知道了", "恕我拒绝", "", false, true, false, null, null, null) == 1)
            {
                ModMain.OurMsgBox("你需要用自己的智慧来找到下一步的线索……\r\n初始线索：gnp.dorC61\\60\\20\\0202\\moc.x1xa.2s\\\\:sp" + "T".Lower() + "th", "解密游戏", "确定", "", "", false, true, false, null, null, null);
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
            this.SliderMusicVolume.GetHintText = ((PageSetupUI._Closure___._I41_0 == null) ? (PageSetupUI._Closure___._I41_0 = new VB_AnonymousDelegate_25<object, object>(PageSetupUI._Closure___._I._Lambda___41_0)) : PageSetupUI._Closure___._I41_0);
            this.SliderLauncherTransparent.GetHintText = ((PageSetupUI._Closure___._I41_1 == null) ? (PageSetupUI._Closure___._I41_1 = new VB_AnonymousDelegate_25<object, object>(PageSetupUI._Closure___._I._Lambda___41_1)) : PageSetupUI._Closure___._I41_1);
            this.SliderLauncherHue.GetHintText = ((PageSetupUI._Closure___._I41_2 == null) ? (PageSetupUI._Closure___._I41_2 = new VB_AnonymousDelegate_25<object, object>(PageSetupUI._Closure___._I._Lambda___41_2)) : PageSetupUI._Closure___._I41_2);
            this.SliderLauncherSat.GetHintText = ((PageSetupUI._Closure___._I41_3 == null) ? (PageSetupUI._Closure___._I41_3 = new VB_AnonymousDelegate_25<object, object>(PageSetupUI._Closure___._I._Lambda___41_3)) : PageSetupUI._Closure___._I41_3);
            this.SliderLauncherDelta.GetHintText = ((PageSetupUI._Closure___._I41_4 == null) ? (PageSetupUI._Closure___._I41_4 = new VB_AnonymousDelegate_26<int, string>(PageSetupUI._Closure___._I._Lambda___41_4)) : PageSetupUI._Closure___._I41_4);
            this.SliderLauncherLight.GetHintText = ((PageSetupUI._Closure___._I41_5 == null) ? (PageSetupUI._Closure___._I41_5 = new VB_AnonymousDelegate_26<int, string>(PageSetupUI._Closure___._I._Lambda___41_5)) : PageSetupUI._Closure___._I41_5);
            this.SliderBackgroundOpacity.GetHintText = ((PageSetupUI._Closure___._I41_6 == null) ? (PageSetupUI._Closure___._I41_6 = new VB_AnonymousDelegate_25<object, object>(PageSetupUI._Closure___._I._Lambda___41_6)) : PageSetupUI._Closure___._I41_6);
            this.SliderBackgroundBlur.GetHintText = ((PageSetupUI._Closure___._I41_7 == null) ? (PageSetupUI._Closure___._I41_7 = new VB_AnonymousDelegate_25<object, object>(PageSetupUI._Closure___._I._Lambda___41_7)) : PageSetupUI._Closure___._I41_7);
        }






        internal virtual OurSlider SliderLauncherHue_WithEvents
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
                OurSlider.ChangeEventHandler changeEventHandler = new OurSlider.ChangeEventHandler(this._Lambda___R64_2);
                OurSlider ourSlider = this._SliderLauncherHue;
                if (ourSlider != null)
                {
                    ourSlider.Change -= changeEventHandler;
                }
                this._SliderLauncherHue = value;
                ourSlider = this._SliderLauncherHue;
                if (ourSlider != null)
                {
                    ourSlider.Change += changeEventHandler;
                }
            }
        }


        internal virtual OurSlider SliderLauncherDelta_WithEvents
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
                OurSlider.ChangeEventHandler changeEventHandler = new OurSlider.ChangeEventHandler(this._Lambda___R72_3);
                OurSlider ourSlider = this._SliderLauncherDelta;
                if (ourSlider != null)
                {
                    ourSlider.Change -= changeEventHandler;
                }
                this._SliderLauncherDelta = value;
                ourSlider = this._SliderLauncherDelta;
                if (ourSlider != null)
                {
                    ourSlider.Change += changeEventHandler;
                }
            }
        }


        internal virtual OurSlider SliderLauncherSat_WithEvents
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
                OurSlider.ChangeEventHandler changeEventHandler = new OurSlider.ChangeEventHandler(this._Lambda___R80_4);
                OurSlider ourSlider = this._SliderLauncherSat;
                if (ourSlider != null)
                {
                    ourSlider.Change -= changeEventHandler;
                }
                this._SliderLauncherSat = value;
                ourSlider = this._SliderLauncherSat;
                if (ourSlider != null)
                {
                    ourSlider.Change += changeEventHandler;
                }
            }
        }


        internal virtual OurSlider SliderLauncherLight_WithEvents
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
                OurSlider.ChangeEventHandler changeEventHandler = new OurSlider.ChangeEventHandler(this._Lambda___R88_5);
                OurSlider ourSlider = this._SliderLauncherLight;
                if (ourSlider != null)
                {
                    ourSlider.Change -= changeEventHandler;
                }
                this._SliderLauncherLight = value;
                ourSlider = this._SliderLauncherLight;
                if (ourSlider != null)
                {
                    ourSlider.Change += changeEventHandler;
                }
            }
        }









        internal virtual TextBlock LabLauncherTheme5Unlock_WithEvents
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





        internal virtual OurRadioBox RadioLauncherTheme8_WithEvents
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
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R144_6);
                OurRadioBox ourRadioBox = this._RadioLauncherTheme8;
                if (ourRadioBox != null)
                {
                    ourRadioBox.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._RadioLauncherTheme8 = value;
                ourRadioBox = this._RadioLauncherTheme8;
                if (ourRadioBox != null)
                {
                    ourRadioBox.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherTheme8Copy_WithEvents
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
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R148_7);
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

        internal virtual OurRadioBox RadioLauncherTheme9_WithEvents
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
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R152_8);
                OurRadioBox ourRadioBox = this._RadioLauncherTheme9;
                if (ourRadioBox != null)
                {
                    ourRadioBox.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._RadioLauncherTheme9 = value;
                ourRadioBox = this._RadioLauncherTheme9;
                if (ourRadioBox != null)
                {
                    ourRadioBox.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherTheme9Copy_WithEvents
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
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R156_9);
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


        internal virtual OurRadioBox RadioLauncherTheme11_WithEvents
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
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R164_10);
                OurRadioBox ourRadioBox = this._RadioLauncherTheme11;
                if (ourRadioBox != null)
                {
                    ourRadioBox.MouseRightButtonUp -= mouseButtonEventHandler;
                }
                this._RadioLauncherTheme11 = value;
                ourRadioBox = this._RadioLauncherTheme11;
                if (ourRadioBox != null)
                {
                    ourRadioBox.MouseRightButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual TextBlock LabLauncherTheme11Click_WithEvents
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
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R168_11);
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

        internal virtual OurRadioBox RadioLauncherTheme14_WithEvents
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
                IOurRadio.ChangedEventHandler changedEventHandler = new IOurRadio.ChangedEventHandler(this.RadioLauncherTheme14_Change);
                OurRadioBox ourRadioBox = this._RadioLauncherTheme14;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Changed -= changedEventHandler;
                }
                this._RadioLauncherTheme14 = value;
                ourRadioBox = this._RadioLauncherTheme14;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Changed += changedEventHandler;
                }
            }
        }








        internal virtual OurButton BtnBackgroundOpen_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnUIBgOpen_Click);
                OurButton ourButton = this._BtnBackgroundOpen;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnBackgroundOpen = value;
                ourButton = this._BtnBackgroundOpen;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnBackgroundRefresh_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnBackgroundRefresh_Click);
                OurButton ourButton = this._BtnBackgroundRefresh;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnBackgroundRefresh = value;
                ourButton = this._BtnBackgroundRefresh;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnBackgroundClear_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnBackgroundClear_Click);
                OurButton ourButton = this._BtnBackgroundClear;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnBackgroundClear = value;
                ourButton = this._BtnBackgroundClear;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }







        internal virtual OurCheckBox CheckMusicStart_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R240_12);
                OurCheckBox ourCheckBox = this._CheckMusicStart;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckMusicStart = value;
                ourCheckBox = this._CheckMusicStart;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckMusicStop_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R244_13);
                OurCheckBox ourCheckBox = this._CheckMusicStop;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckMusicStop = value;
                ourCheckBox = this._CheckMusicStop;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurButton BtnMusicOpen_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnMusicOpen_Click);
                OurButton ourButton = this._BtnMusicOpen;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnMusicOpen = value;
                ourButton = this._BtnMusicOpen;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnMusicRefresh_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnMusicRefresh_Click);
                OurButton ourButton = this._BtnMusicRefresh;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnMusicRefresh = value;
                ourButton = this._BtnMusicRefresh;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnMusicClear_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnMusicClear_Click);
                OurButton ourButton = this._BtnMusicClear;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnMusicClear = value;
                ourButton = this._BtnMusicClear;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }





        internal virtual OurRadioBox RadioLogoType3_WithEvents
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
                OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler = new OurRadioBox.PreviewCheckEventHandler(this.RadioLogoType3_Check);
                OurRadioBox ourRadioBox = this._RadioLogoType3;
                if (ourRadioBox != null)
                {
                    ourRadioBox.PreviewCheck -= previewCheckEventHandler;
                }
                this._RadioLogoType3 = value;
                ourRadioBox = this._RadioLogoType3;
                if (ourRadioBox != null)
                {
                    ourRadioBox.PreviewCheck += previewCheckEventHandler;
                }
            }
        }





        internal virtual OurButton BtnLogoChange_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnLogoChange_Click);
                OurButton ourButton = this._BtnLogoChange;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnLogoChange = value;
                ourButton = this._BtnLogoChange;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnLogoDelete_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnLogoDelete_Click);
                OurButton ourButton = this._BtnLogoDelete;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnLogoDelete = value;
                ourButton = this._BtnLogoDelete;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }









        internal virtual OurButton BtnCustomRefresh_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R336_14);
                OurButton ourButton = this._BtnCustomRefresh;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnCustomRefresh = value;
                ourButton = this._BtnCustomRefresh;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnCustomFile_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnCustomFile_Click);
                OurButton ourButton = this._BtnCustomFile;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnCustomFile = value;
                ourButton = this._BtnCustomFile;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnCustomTutorial_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnCustomTutorial_Click);
                OurButton ourButton = this._BtnCustomTutorial;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnCustomTutorial = value;
                ourButton = this._BtnCustomTutorial;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnCustomFolder_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnCustomFolder_Click);
                OurButton ourButton = this._BtnCustomFolder;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnCustomFolder = value;
                ourButton = this._BtnCustomFolder;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }







        internal virtual OurCheckBox CheckHiddenPageSetup_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R376_15);
                OurCheckBox.ChangeEventHandler changeEventHandler2 = new OurCheckBox.ChangeEventHandler(this.HiddenHint);
                OurCheckBox ourCheckBox = this._CheckHiddenPageSetup;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                    ourCheckBox.Change -= changeEventHandler2;
                }
                this._CheckHiddenPageSetup = value;
                ourCheckBox = this._CheckHiddenPageSetup;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                    ourCheckBox.Change += changeEventHandler2;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenPageOther_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R380_16);
                OurCheckBox ourCheckBox = this._CheckHiddenPageOther;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenPageOther = value;
                ourCheckBox = this._CheckHiddenPageOther;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenSetupLaunch_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R384_17);
                OurCheckBox ourCheckBox = this._CheckHiddenSetupLaunch;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenSetupLaunch = value;
                ourCheckBox = this._CheckHiddenSetupLaunch;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenSetupLink_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R388_18);
                OurCheckBox ourCheckBox = this._CheckHiddenSetupLink;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenSetupLink = value;
                ourCheckBox = this._CheckHiddenSetupLink;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenSetupUI_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R392_19);
                OurCheckBox.ChangeEventHandler changeEventHandler2 = new OurCheckBox.ChangeEventHandler(this.HiddenHint);
                OurCheckBox ourCheckBox = this._CheckHiddenSetupUI;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                    ourCheckBox.Change -= changeEventHandler2;
                }
                this._CheckHiddenSetupUI = value;
                ourCheckBox = this._CheckHiddenSetupUI;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                    ourCheckBox.Change += changeEventHandler2;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenSetupSystem_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R396_20);
                OurCheckBox ourCheckBox = this._CheckHiddenSetupSystem;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenSetupSystem = value;
                ourCheckBox = this._CheckHiddenSetupSystem;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenOtherHelp_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.HiddenOtherSub);
                OurCheckBox ourCheckBox = this._CheckHiddenOtherHelp;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherHelp = value;
                ourCheckBox = this._CheckHiddenOtherHelp;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenOtherAbout_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.HiddenOtherSub);
                OurCheckBox ourCheckBox = this._CheckHiddenOtherAbout;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherAbout = value;
                ourCheckBox = this._CheckHiddenOtherAbout;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenOtherTest_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.HiddenOtherSub);
                OurCheckBox ourCheckBox = this._CheckHiddenOtherTest;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherTest = value;
                ourCheckBox = this._CheckHiddenOtherTest;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenOtherFeedback_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.HiddenOtherNet);
                OurCheckBox ourCheckBox = this._CheckHiddenOtherFeedback;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherFeedback = value;
                ourCheckBox = this._CheckHiddenOtherFeedback;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual OurCheckBox CheckHiddenOtherVote_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.HiddenOtherNet);
                OurCheckBox ourCheckBox = this._CheckHiddenOtherVote;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenOtherVote = value;
                ourCheckBox = this._CheckHiddenOtherVote;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }




        internal virtual OurCheckBox CheckHiddenFunctionHidden_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this.HiddenHint);
                OurCheckBox ourCheckBox = this._CheckHiddenFunctionHidden;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckHiddenFunctionHidden = value;
                ourCheckBox = this._CheckHiddenFunctionHidden;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }


        [CompilerGenerated]
        private void _Lambda___R64_2(object a0, bool a1)
        {
            this.HSL_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R72_3(object a0, bool a1)
        {
            this.HSL_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R80_4(object a0, bool a1)
        {
            this.HSL_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R88_5(object a0, bool a1)
        {
            this.HSL_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R144_6(object a0, MouseButtonEventArgs a1)
        {
            this.LabLauncherTheme8Copy_MouseRightButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R148_7(object a0, MouseButtonEventArgs a1)
        {
            this.LabLauncherTheme8Copy_MouseRightButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R152_8(object a0, MouseButtonEventArgs a1)
        {
            this.LabLauncherTheme9Copy_MouseRightButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R156_9(object a0, MouseButtonEventArgs a1)
        {
            this.LabLauncherTheme9Copy_MouseRightButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R164_10(object a0, MouseButtonEventArgs a1)
        {
            this.LabLauncherTheme11Click_MouseLeftButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R168_11(object a0, MouseButtonEventArgs a1)
        {
            this.LabLauncherTheme11Click_MouseLeftButtonUp();
        }

        [CompilerGenerated]
        private void _Lambda___R240_12(object a0, bool a1)
        {
            this.CheckMusicStart_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R244_13(object a0, bool a1)
        {
            this.CheckMusicStop_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R336_14(object a0, MouseButtonEventArgs a1)
        {
            this.BtnCustomRefresh_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R376_15(object a0, bool a1)
        {
            this.HiddenSetupMain();
        }

        [CompilerGenerated]
        private void _Lambda___R380_16(object a0, bool a1)
        {
            this.HiddenOtherMain();
        }

        [CompilerGenerated]
        private void _Lambda___R384_17(object a0, bool a1)
        {
            this.HiddenSetupSub();
        }

        [CompilerGenerated]
        private void _Lambda___R388_18(object a0, bool a1)
        {
            this.HiddenSetupSub();
        }

        [CompilerGenerated]
        private void _Lambda___R392_19(object a0, bool a1)
        {
            this.HiddenSetupSub();
        }

        [CompilerGenerated]
        private void _Lambda___R396_20(object a0, bool a1)
        {
            this.HiddenSetupSub();
        }

        private static bool _HiddenForceShow = false;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("CardLauncher")]
        private OurCard _CardLauncher;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderLauncherTransparent")]
        private OurSlider _SliderLauncherTransparent;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherHue")]
        private TextBlock _LabLauncherHue;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderLauncherHue")]
        private OurSlider _SliderLauncherHue;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherDelta")]
        private TextBlock _LabLauncherDelta;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderLauncherDelta")]
        private OurSlider _SliderLauncherDelta;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherSat")]
        private TextBlock _LabLauncherSat;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderLauncherSat")]
        private OurSlider _SliderLauncherSat;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherLight")]
        private TextBlock _LabLauncherLight;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderLauncherLight")]
        private OurSlider _SliderLauncherLight;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLauncherTheme")]
        private Grid _PanLauncherTheme;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme0")]
        private OurRadioBox _RadioLauncherTheme0;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme1")]
        private OurRadioBox _RadioLauncherTheme1;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme2")]
        private OurRadioBox _RadioLauncherTheme2;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme3")]
        private OurRadioBox _RadioLauncherTheme3;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme4")]
        private OurRadioBox _RadioLauncherTheme4;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme5")]
        private OurRadioBox _RadioLauncherTheme5;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme5Gray")]
        private OurRadioBox _RadioLauncherTheme5Gray;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherTheme5Unlock")]
        private TextBlock _LabLauncherTheme5Unlock;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme12")]
        private OurRadioBox _RadioLauncherTheme12;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme6")]
        private OurRadioBox _RadioLauncherTheme6;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme7")]
        private OurRadioBox _RadioLauncherTheme7;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme13")]
        private OurRadioBox _RadioLauncherTheme13;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme8")]
        private OurRadioBox _RadioLauncherTheme8;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherTheme8Copy")]
        private TextBlock _LabLauncherTheme8Copy;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme9")]
        private OurRadioBox _RadioLauncherTheme9;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherTheme9Copy")]
        private TextBlock _LabLauncherTheme9Copy;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme10")]
        private OurRadioBox _RadioLauncherTheme10;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme11")]
        private OurRadioBox _RadioLauncherTheme11;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLauncherTheme11Click")]
        private TextBlock _LabLauncherTheme11Click;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLauncherTheme14")]
        private OurRadioBox _RadioLauncherTheme14;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLauncherHide")]
        private Border _PanLauncherHide;

        [CompilerGenerated]
        [AccessedThroughProperty("CardBackground")]
        private OurCard _CardBackground;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBackgroundSuit")]
        private Grid _PanBackgroundSuit;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBackgroundOpacity")]
        private Grid _PanBackgroundOpacity;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderBackgroundOpacity")]
        private OurSlider _SliderBackgroundOpacity;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBackgroundBlur")]
        private Grid _PanBackgroundBlur;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderBackgroundBlur")]
        private OurSlider _SliderBackgroundBlur;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnBackgroundOpen")]
        private OurButton _BtnBackgroundOpen;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnBackgroundRefresh")]
        private OurButton _BtnBackgroundRefresh;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnBackgroundClear")]
        private OurButton _BtnBackgroundClear;

        [CompilerGenerated]
        [AccessedThroughProperty("CardMusic")]
        private OurCard _CardMusic;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMusicVolume")]
        private Grid _PanMusicVolume;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderMusicVolume")]
        private OurSlider _SliderMusicVolume;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMusicDetail")]
        private StackPanel _PanMusicDetail;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckMusicRandom")]
        private OurCheckBox _CheckMusicRandom;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckMusicAuto")]
        private OurCheckBox _CheckMusicAuto;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckMusicStart")]
        private OurCheckBox _CheckMusicStart;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckMusicStop")]
        private OurCheckBox _CheckMusicStop;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnMusicOpen")]
        private OurButton _BtnMusicOpen;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnMusicRefresh")]
        private OurButton _BtnMusicRefresh;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnMusicClear")]
        private OurButton _BtnMusicClear;

        [CompilerGenerated]
        [AccessedThroughProperty("CardLogo")]
        private OurCard _CardLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLogoType0")]
        private OurRadioBox _RadioLogoType0;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLogoType1")]
        private OurRadioBox _RadioLogoType1;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLogoType2")]
        private OurRadioBox _RadioLogoType2;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLogoType3")]
        private OurRadioBox _RadioLogoType3;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckLogoLeft")]
        private OurCheckBox _CheckLogoLeft;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLogoText")]
        private Grid _PanLogoText;

        [CompilerGenerated]
        [AccessedThroughProperty("TextLogoText")]
        private OurTextBox _TextLogoText;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLogoChange")]
        private Grid _PanLogoChange;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnLogoChange")]
        private OurButton _BtnLogoChange;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnLogoDelete")]
        private OurButton _BtnLogoDelete;

        [CompilerGenerated]
        [AccessedThroughProperty("CardCustom")]
        private OurCard _CardCustom;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioCustomType0")]
        private OurRadioBox _RadioCustomType0;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioCustomType3")]
        private OurRadioBox _RadioCustomType3;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioCustomType1")]
        private OurRadioBox _RadioCustomType1;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioCustomType2")]
        private OurRadioBox _RadioCustomType2;

        [CompilerGenerated]
        [AccessedThroughProperty("HintCustom")]
        private OurHint _HintCustom;

        [CompilerGenerated]
        [AccessedThroughProperty("HintCustomWarn")]
        private OurHint _HintCustomWarn;

        [CompilerGenerated]
        [AccessedThroughProperty("PanCustomLocal")]
        private WrapPanel _PanCustomLocal;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnCustomRefresh")]
        private OurButton _BtnCustomRefresh;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnCustomFile")]
        private OurButton _BtnCustomFile;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnCustomTutorial")]
        private OurButton _BtnCustomTutorial;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnCustomFolder")]
        private OurButton _BtnCustomFolder;

        [CompilerGenerated]
        [AccessedThroughProperty("PanCustomNet")]
        private Grid _PanCustomNet;

        [CompilerGenerated]
        [AccessedThroughProperty("TextCustomNet")]
        private OurTextBox _TextCustomNet;

        [CompilerGenerated]
        [AccessedThroughProperty("PanCustomPreset")]
        private Grid _PanCustomPreset;

        [CompilerGenerated]
        [AccessedThroughProperty("CardSwitch")]
        private OurCard _CardSwitch;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenPageDownload")]
        private OurCheckBox _CheckHiddenPageDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenPageLink")]
        private OurCheckBox _CheckHiddenPageLink;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenPageSetup")]
        private OurCheckBox _CheckHiddenPageSetup;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenPageOther")]
        private OurCheckBox _CheckHiddenPageOther;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenSetupLaunch")]
        private OurCheckBox _CheckHiddenSetupLaunch;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenSetupLink")]
        private OurCheckBox _CheckHiddenSetupLink;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenSetupUI")]
        private OurCheckBox _CheckHiddenSetupUI;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenSetupSystem")]
        private OurCheckBox _CheckHiddenSetupSystem;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenOtherHelp")]
        private OurCheckBox _CheckHiddenOtherHelp;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenOtherAbout")]
        private OurCheckBox _CheckHiddenOtherAbout;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenOtherTest")]
        private OurCheckBox _CheckHiddenOtherTest;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenOtherFeedback")]
        private OurCheckBox _CheckHiddenOtherFeedback;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenOtherVote")]
        private OurCheckBox _CheckHiddenOtherVote;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckLauncherEmail")]
        private OurCheckBox _CheckLauncherEmail;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenFunctionSelect")]
        private OurCheckBox _CheckHiddenFunctionSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenFunctionModUpdate")]
        private OurCheckBox _CheckHiddenFunctionModUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenFunctionHidden")]
        private OurCheckBox _CheckHiddenFunctionHidden;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHiddenFunctionModify")]
        private OurCheckBox _CheckHiddenFunctionModify;

        private bool _STATIC_PageSetupUI_Loaded_20211C128141_Reloaded;

        private StaticLocalInitFlag _STATIC_PageSetupUI_Loaded_20211C128141_Reloaded_Init;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal void _Lambda___R1_1(object a0, RoutedEventArgs a1)
            {
                PageSetupUI.HiddenRefresh();
            }

            internal bool _Lambda___15_0(string f)
            {
                return !new string[] { "ini", "jpg", "txt", "cfg", "lrc", "db", "png" }.Contains(PathUtils.GetExtension(f));
            }

            internal void _Lambda___16_0()
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
                    ModBase.RunInUi((PageSetupUI._Closure___._I16_1 == null) ? (PageSetupUI._Closure___._I16_1 = new Action(PageSetupUI._Closure___._I._Lambda___16_1)) : PageSetupUI._Closure___._I16_1, false);
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, "重建背景音乐文件夹失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupUI.xaml.vb");
                }
            }

            internal void _Lambda___16_1()
            {
                ModMusic.MusicRefreshPlay(false, false);
            }

            internal object _Lambda___41_0(object v)
            {
                return Operators.ConcatenateObject(NewLateBinding.LateGet(null, typeof(Math), "Ceiling", new object[] { Operators.MultiplyObject(v, 0.1) }, null, null, null), "%");
            }

            internal object _Lambda___41_1(object v)
            {
                return Operators.ConcatenateObject(NewLateBinding.LateGet(null, typeof(Math), "Round", new object[] { Operators.AddObject(40, Operators.MultiplyObject(v, 0.1)) }, null, null, null), "%");
            }

            internal object _Lambda___41_2(object v)
            {
                return Operators.ConcatenateObject(v, "°");
            }

            internal object _Lambda___41_3(object v)
            {
                return Operators.ConcatenateObject(v, "%");
            }

            internal string _Lambda___41_4(int Value)
            {
                checked
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
                }
            }

            internal string _Lambda___41_5(int Value)
            {
                checked
                {
                    string text;
                    if (Value > 20)
                    {
                        text = "+" + Conversions.ToString(Value - 20);
                    }
                    else if (Value == 20)
                    {
                        text = Conversions.ToString(0);
                    }
                    else
                    {
                        text = Conversions.ToString(Value - 20);
                    }
                    return text;
                }
            }

            internal object _Lambda___41_6(object v)
            {
                return Operators.ConcatenateObject(NewLateBinding.LateGet(null, typeof(Math), "Round", new object[] { Operators.MultiplyObject(v, 0.1) }, null, null, null), "%");
            }

            internal object _Lambda___41_7(object v)
            {
                return Operators.ConcatenateObject(v, " 像素");
            }

            public static readonly PageSetupUI._Closure___ _I = new PageSetupUI._Closure___();

            public static RoutedEventHandler _IR1_1;

            public static Func<string, bool> _I15_0;

            public static Action _I16_1;

            public static Action _I16_0;

            public static VB_AnonymousDelegate_25<object, object> _I41_0;

            public static VB_AnonymousDelegate_25<object, object> _I41_1;

            public static VB_AnonymousDelegate_25<object, object> _I41_2;

            public static VB_AnonymousDelegate_25<object, object> _I41_3;

            public static VB_AnonymousDelegate_26<int, string> _I41_4;

            public static VB_AnonymousDelegate_26<int, string> _I41_5;

            public static VB_AnonymousDelegate_25<object, object> _I41_6;

            public static VB_AnonymousDelegate_25<object, object> _I41_7;
        }
    }
}
