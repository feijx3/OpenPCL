using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PCL.Our;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceOverall : OurPageRight
    {
        public PageInstanceOverall()
        {
            base.Loaded += this.PageSetupLaunch_Loaded;
            this.IsLoad = false;
            this.InitializeComponent();
            this.ComboDisplayLogo_WithEvents = this.ComboDisplayLogo;
            this.ItemDisplayLogoCustom_WithEvents = this.ItemDisplayLogoCustom;
            this.ComboDisplayType_WithEvents = this.ComboDisplayType;
            this.BtnDisplayRename_WithEvents = this.BtnDisplayRename;
            this.BtnDisplayDesc_WithEvents = this.BtnDisplayDesc;
            this.BtnDisplayStar_WithEvents = this.BtnDisplayStar;
            this.BtnFolderVersion_WithEvents = this.BtnFolderVersion;
            this.BtnFolderSaves_WithEvents = this.BtnFolderSaves;
            this.BtnFolderMods_WithEvents = this.BtnFolderMods;
            this.BtnFolderScreen_WithEvents = this.BtnFolderScreen;
            this.BtnManageScript_WithEvents = this.BtnManageScript;
            this.BtnManageCheck_WithEvents = this.BtnManageCheck;
            this.BtnManageDelete_WithEvents = this.BtnManageDelete;
        }

        private void PageSetupLaunch_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            this.Reload();
            if (!this.IsLoad)
            {
                this.IsLoad = true;
                this.PanDisplay.TriggerForceResize();
            }
        }

        private void Reload()
        {
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.ComboDisplayType.SelectedIndex = Conversions.ToInteger(ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "DisplayType", Conversions.ToString(0)));
                this.BtnDisplayStar.Text = (PageInstanceLeft.Instance.IsStar ? "从收藏夹中移除" : "加入收藏夹");
                this.BtnFolderMods.Visibility = (PageInstanceLeft.Instance.Modable ? Visibility.Visible : Visibility.Collapsed);
                this.PanDisplayItem.Children.Clear();
                this.InstanceHeaderItem = PageInstanceLeft.Instance.ToListItem(null).Init();
                this.InstanceHeaderItem.IsHitTestVisible = false;
                this.PanDisplayItem.Children.Add(this.InstanceHeaderItem);
                ModMain.FrmMain.PageNameRefresh();
                this.ComboDisplayLogo.SelectedIndex = 0;
                string text = ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "Logo", "");
                if (Conversions.ToBoolean(ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "LogoCustom", "False")))
                {
                    try
                    {
                        foreach (object obj in ((IEnumerable)this.ComboDisplayLogo.Items))
                        {
                            OurComboBoxItem ourComboBoxItem = (OurComboBoxItem)obj;
                            if (Operators.ConditionalCompareObjectEqual(ourComboBoxItem.Tag, text, false) || (Operators.ConditionalCompareObjectEqual(ourComboBoxItem.Tag, "PCL\\Logo.png", false) && text.EndsWithF("PCL\\Logo.png", false)))
                            {
                                this.ComboDisplayLogo.SelectedItem = ourComboBoxItem;
                                break;
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
                ModAnimation.AniControlEnabled--;
            }
        }

        private void ComboDisplayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoad && ModAnimation.AniControlEnabled == 0)
            {
                if (this.ComboDisplayType.SelectedIndex != 1)
                {
                    try
                    {
                        ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "DisplayType", Conversions.ToString(this.ComboDisplayType.SelectedIndex));
                        PageInstanceLeft.Instance.DisplayType = (ModMinecraft.McInstanceCardType)Conversions.ToInteger(ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "DisplayType", Conversions.ToString(0)));
                        ModMain.FrmInstanceLeft.RefreshModDisabled();
                        ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "");
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("修改版本分类失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
                    }
                    this.Reload();
                    return;
                }
                try
                {
                    if (!Settings.Get<bool>("HintHide", null))
                    {
                        if (ModMain.OurMsgBox("确认要从版本列表中隐藏该版本吗？隐藏该版本后，它将不再出现于 PCL 显示的版本列表中。\r\n此后，在版本列表页面按下 F11 才可以查看被隐藏的版本。", "隐藏版本提示", "确定", "取消", "", false, true, false, null, null, null) != 1)
                        {
                            this.ComboDisplayType.SelectedIndex = 0;
                            return;
                        }
                        Settings.Set("HintHide", true, null);
                    }
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "DisplayType", Conversions.ToString(1));
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "");
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, string.Format("隐藏版本 {0} 失败", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
                }
            }
        }

        private void BtnDisplayDesc_Click(object sender, EventArgs e)
        {
            try
            {
                string text = ModBase.ReadIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "CustomInfo", "");
                string text2 = ModMain.OurMsgBoxInput("更改描述", "修改版本的描述文本，留空则使用 PCL 的默认描述。", text, new Collection<Validate>(), "默认描述", "确定", "取消", false);
                if (text2 != null && Operators.CompareString(text, text2, false) != 0)
                {
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "CustomInfo", text2);
                }
                PageInstanceLeft.Instance = new ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load();
                this.Reload();
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("版本 {0} 描述更改失败", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
            }
        }

        private void BtnDisplayRename_Click(object sender, EventArgs e)
        {
            try
            {
                if (ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
                {
                    ModMain.Hint("该版本正在被修改，请稍后再重命名！", ModMain.HintType.Red, true);
                }
                else
                {
                    string name = PageInstanceLeft.Instance.Name;
                    string text = ModMain.OurMsgBoxInput("重命名版本", "", name, new Collection<Validate>
                    {
                        new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, new List<string> { name })
                    }, "", "确定", "取消", false);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        ModMinecraft.McInstance mcInstance = PageInstanceLeft.Instance.Rename(text);
                        ModMain.Hint("重命名成功！", ModMain.HintType.Green, true);
                        PageInstanceLeft.Instance = mcInstance;
                        if (!Information.IsNothing(ModMinecraft.McInstanceSelected) && ModMinecraft.McInstanceSelected.Equals(PageInstanceLeft.Instance))
                        {
                            ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", text);
                        }
                        this.Reload();
                        ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "重命名版本失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
            }
        }

        private void ComboDisplayLogo_SelectionChanged()
        {
            if (this.IsLoad && ModAnimation.AniControlEnabled == 0)
            {
                try
                {
                    if (this.ComboDisplayLogo.SelectedItem == this.ItemDisplayLogoCustom)
                    {
                        string text = Dialogs.SelectFile("选择图片", false, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?(new ValueTuple<string[], string>[]
                        {
                            new ValueTuple<string[], string>(new string[] { "png", "jpeg", "jpg", "gif", "webp" }, "常用图片文件")
                        })).FirstOrDefault<string>();
                        if (string.IsNullOrEmpty(text))
                        {
                            this.Reload();
                            return;
                        }
                        FileUtils.Copy(text, PageInstanceLeft.Instance.PathVersion + "PCL\\Logo.png");
                    }
                    else
                    {
                        FileUtils.Delete(PageInstanceLeft.Instance.PathVersion + "PCL\\Logo.png", false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("更改自定义版本图标失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
                }
                try
                {
                    string text2 = Conversions.ToString(NewLateBinding.LateGet(this.ComboDisplayLogo.SelectedItem, null, "Tag", new object[0], null, null, null));
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "Logo", text2);
                    ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "LogoCustom", Conversions.ToString(Operators.CompareString(text2, "", false) != 0));
                    ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "");
                    PageInstanceLeft.Instance = new ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load();
                    this.Reload();
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, string.Format("更改版本图标失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
                }
            }
        }

        private void BtnDisplayStar_Click(object sender, EventArgs e)
        {
            try
            {
                ModBase.WriteIni(PageInstanceLeft.Instance.PathVersion + "PCL\\Setup.ini", "IsStar", Conversions.ToString(!PageInstanceLeft.Instance.IsStar));
                PageInstanceLeft.Instance = new ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load();
                this.Reload();
                ModMinecraft.McInstanceListForceRefresh = true;
                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("版本 {0} 收藏状态更改失败", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
            }
        }

        private void BtnFolderVersion_Click()
        {
            PageInstanceOverall.OpenInstanceFolder(PageInstanceLeft.Instance);
        }

        public static void OpenInstanceFolder(ModMinecraft.McInstance Instance)
        {
            ModBase.OpenExplorer(Instance.PathVersion);
        }

        private void BtnFolderSaves_Click()
        {
            string text = PageInstanceLeft.Instance.PathIndie + "saves\\";
            DirectoryUtils.Create(text);
            ModBase.OpenExplorer(text);
        }

        private void BtnFolderMods_Click()
        {
            string text = PageInstanceLeft.Instance.PathIndie + "mods\\";
            DirectoryUtils.Create(text);
            ModBase.OpenExplorer(text);
        }

        private void BtnFolderScreen_Click(object sender, MouseButtonEventArgs e)
        {
            string text = PageInstanceLeft.Instance.PathIndie + "screenshots\\";
            DirectoryUtils.Create(text);
            ModBase.OpenExplorer(text);
        }

        private void BtnManageScript_Click()
        {
            try
            {
                string text = Dialogs.SaveFile("选择脚本保存位置", "启动 " + PageInstanceLeft.Instance.Name + ".bat", null, new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>("bat", "批处理文件")
                });
                if (text != null)
                {
                    if (ModLaunch.McLaunchLoader.State == ModBase.LoadState.Loading)
                    {
                        ModMain.Hint("请在当前启动任务结束后再试！", ModMain.HintType.Red, true);
                    }
                    else if (ModLaunch.McLaunchStart(new ModLaunch.McLaunchOptions
                    {
                        SaveBatch = text,
                        Instance = PageInstanceLeft.Instance
                    }))
                    {
                        if (Settings.Get<ModLaunch.McLoginType>("LoginType", null) == ModLaunch.McLoginType.Legacy)
                        {
                            ModMain.Hint("正在导出启动脚本……", ModMain.HintType.Blue, true);
                        }
                        else
                        {
                            ModMain.Hint("正在导出启动脚本……（注意，使用脚本启动可能会导致登录失效！）", ModMain.HintType.Blue, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("导出启动脚本失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
            }
        }

        private void BtnManageCheck_Click(object sender, EventArgs e)
        {
            try
            {
                PageInstanceOverall._Closure___16_0 CS_locals1 = new PageInstanceOverall._Closure___16_0(null);
                if (ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
                {
                    ModMain.Hint("该版本正在被修改，请稍后再补全文件！", ModMain.HintType.Red, true);
                }
                else if (Conversions.ToBoolean(ModMinecraft.ShouldIgnoreFileCheck(PageInstanceLeft.Instance)))
                {
                    ModMain.Hint("请先关闭 [版本设置 → 设置 → 高级启动选项 → 关闭文件校验]，然后再尝试补全文件！", ModMain.HintType.Blue, true);
                }
                else
                {
                    try
                    {
                        IEnumerator<ModLoader.LoaderBase> enumerator = ModLoader.LoaderTaskbar.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (Operators.CompareString(enumerator.Current.Name, PageInstanceLeft.Instance.Name + " 文件补全", false) == 0)
                            {
                                ModMain.Hint("正在处理中，请稍候！", ModMain.HintType.Red, true);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<ModLoader.LoaderBase> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    CS_locals1._VB_Local_Loader = new ModLoader.LoaderCombo<string>(PageInstanceLeft.Instance.Name + " 文件补全", ModDownload.DlClientFix(PageInstanceLeft.Instance, true, false));
                    CS_locals1._VB_Local_Loader.OnStateChanged = new Action<ModLoader.LoaderBase>(CS_locals1._Lambda___R1);
                    CS_locals1._VB_Local_Loader.Start(PageInstanceLeft.Instance.Name, false);
                    ModLoader.LoaderTaskbarAdd<string>(CS_locals1._VB_Local_Loader);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("尝试补全文件失败（{0}）", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
            }
        }

        private void BtnManageDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
                {
                    ModMain.Hint("该版本正在被修改，请稍后再删除！", ModMain.HintType.Red, true);
                }
                else
                {
                    bool shiftKeyDown = OurWpfExtension.Computer.Keyboard.ShiftKeyDown;
                    bool flag = PageInstanceLeft.Instance.State != ModMinecraft.McInstance.McInstanceState.Error && Operators.CompareString(PageInstanceLeft.Instance.PathIndie, ModMinecraft.McFolderSelected, false) != 0;
                    DirectoryInfo info = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + "saves\\");
                    List<string> list = ((flag && info.Exists) ? info.EnumerateDirectories().OrderByDescending<DirectoryInfo, DateTime>((PageInstanceOverall._Closure___._I17_0 == null) ? (PageInstanceOverall._Closure___._I17_0 = new Func<DirectoryInfo, DateTime>(PageInstanceOverall._Closure___._I._Lambda___17_0)) : PageInstanceOverall._Closure___._I17_0).Select<DirectoryInfo, string>((PageInstanceOverall._Closure___._I17_1 == null) ? (PageInstanceOverall._Closure___._I17_1 = new Func<DirectoryInfo, string>(PageInstanceOverall._Closure___._I._Lambda___17_1)) : PageInstanceOverall._Closure___._I17_1)
                        .ToList<string>() : new List<string>());
                    int num = ModMain.OurMsgBox(string.Format("你确定要{0}删除版本 {1} 吗？", shiftKeyDown ? "永久" : "", PageInstanceLeft.Instance.Name) + (flag ? ("\r\n该版本对应的存档、资源包、Mod 等文件也将被一并删除！" + (list.Any<string>() ? ("\r\n\r\n这会删除以下存档：\r\n· " + list.Join("\r\n· ")) : "")) : ""), "版本删除确认", "确定", "取消", "", flag || shiftKeyDown, true, false, null, null, null);
                    if (num != 1)
                    {
                        if (num == 2)
                        {
                            return;
                        }
                    }
                    else
                    {
                        PageInstanceLeft.Instance.ResetSettingsCache();
                        DirectoryUtils.Delete(PageInstanceLeft.Instance.PathVersion, !shiftKeyDown);
                        ModMain.Hint("已删除版本 " + PageInstanceLeft.Instance.Name + "！", ModMain.HintType.Green, true);
                    }
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                    ModMain.FrmMain.PageBack();
                }
            }
            catch (OperationCanceledException ex)
            {
                Logger.Warn(ex, string.Format("删除版本 {0} 被主动取消", PageInstanceLeft.Instance.Name), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, string.Format("删除版本 {0} 失败", PageInstanceLeft.Instance.Name), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceOverall.xaml.vb");
            }
        }






        internal virtual OurComboBox ComboDisplayLogo_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboDisplayLogo;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this._Lambda___R40_2);
                OurComboBox ourComboBox = this._ComboDisplayLogo;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboDisplayLogo = value;
                ourComboBox = this._ComboDisplayLogo;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual OurComboBoxItem ItemDisplayLogoCustom_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemDisplayLogoCustom;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this._Lambda___R44_3);
                OurComboBoxItem ourComboBoxItem = this._ItemDisplayLogoCustom;
                if (ourComboBoxItem != null)
                {
                    ourComboBoxItem.PreviewMouseLeftButtonUp -= mouseButtonEventHandler;
                }
                this._ItemDisplayLogoCustom = value;
                ourComboBoxItem = this._ItemDisplayLogoCustom;
                if (ourComboBoxItem != null)
                {
                    ourComboBoxItem.PreviewMouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual OurComboBox ComboDisplayType_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboDisplayType;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboDisplayType_SelectionChanged);
                OurComboBox ourComboBox = this._ComboDisplayType;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboDisplayType = value;
                ourComboBox = this._ComboDisplayType;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual OurButton BtnDisplayRename_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDisplayRename;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnDisplayRename_Click);
                OurButton ourButton = this._BtnDisplayRename;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDisplayRename = value;
                ourButton = this._BtnDisplayRename;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnDisplayDesc_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDisplayDesc;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnDisplayDesc_Click);
                OurButton ourButton = this._BtnDisplayDesc;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDisplayDesc = value;
                ourButton = this._BtnDisplayDesc;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnDisplayStar_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnDisplayStar;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnDisplayStar_Click);
                OurButton ourButton = this._BtnDisplayStar;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnDisplayStar = value;
                ourButton = this._BtnDisplayStar;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurButton BtnFolderVersion_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFolderVersion;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R68_4);
                OurButton ourButton = this._BtnFolderVersion;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnFolderVersion = value;
                ourButton = this._BtnFolderVersion;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnFolderSaves_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFolderSaves;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R72_5);
                OurButton ourButton = this._BtnFolderSaves;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnFolderSaves = value;
                ourButton = this._BtnFolderSaves;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnFolderMods_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFolderMods;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R76_6);
                OurButton ourButton = this._BtnFolderMods;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnFolderMods = value;
                ourButton = this._BtnFolderMods;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnFolderScreen_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFolderScreen;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnFolderScreen_Click);
                OurButton ourButton = this._BtnFolderScreen;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnFolderScreen = value;
                ourButton = this._BtnFolderScreen;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurButton BtnManageScript_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnManageScript;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R88_7);
                OurButton ourButton = this._BtnManageScript;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnManageScript = value;
                ourButton = this._BtnManageScript;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnManageCheck_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnManageCheck;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageCheck_Click);
                OurButton ourButton = this._BtnManageCheck;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnManageCheck = value;
                ourButton = this._BtnManageCheck;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnManageDelete_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnManageDelete;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageDelete_Click);
                OurButton ourButton = this._BtnManageDelete;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnManageDelete = value;
                ourButton = this._BtnManageDelete;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R40_2(object a0, SelectionChangedEventArgs a1)
        {
            this.ComboDisplayLogo_SelectionChanged();
        }

        [CompilerGenerated]
        private void _Lambda___R44_3(object a0, MouseButtonEventArgs a1)
        {
            this.ComboDisplayLogo_SelectionChanged();
        }

        [CompilerGenerated]
        private void _Lambda___R68_4(object a0, MouseButtonEventArgs a1)
        {
            this.BtnFolderVersion_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R72_5(object a0, MouseButtonEventArgs a1)
        {
            this.BtnFolderSaves_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R76_6(object a0, MouseButtonEventArgs a1)
        {
            this.BtnFolderMods_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R88_7(object a0, MouseButtonEventArgs a1)
        {
            this.BtnManageScript_Click();
        }

        private bool IsLoad;

        public OurListItem InstanceHeaderItem;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("PanDisplayItem")]
        private Grid _PanDisplayItem;

        [CompilerGenerated]
        [AccessedThroughProperty("PanDisplay")]
        private OurCard _PanDisplay;

        [CompilerGenerated]
        [AccessedThroughProperty("PanDisplayIcon")]
        private Grid _PanDisplayIcon;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboDisplayLogo")]
        private OurComboBox _ComboDisplayLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemDisplayLogoCustom")]
        private OurComboBoxItem _ItemDisplayLogoCustom;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboDisplayType")]
        private OurComboBox _ComboDisplayType;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDisplayRename")]
        private OurButton _BtnDisplayRename;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDisplayDesc")]
        private OurButton _BtnDisplayDesc;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnDisplayStar")]
        private OurButton _BtnDisplayStar;

        [CompilerGenerated]
        [AccessedThroughProperty("PanFolder")]
        private OurCard _PanFolder;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFolderVersion")]
        private OurButton _BtnFolderVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFolderSaves")]
        private OurButton _BtnFolderSaves;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFolderMods")]
        private OurButton _BtnFolderMods;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFolderScreen")]
        private OurButton _BtnFolderScreen;

        [CompilerGenerated]
        [AccessedThroughProperty("PanManage")]
        private OurCard _PanManage;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnManageScript")]
        private OurButton _BtnManageScript;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnManageCheck")]
        private OurButton _BtnManageCheck;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnManageDelete")]
        private OurButton _BtnManageDelete;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal DateTime _Lambda___17_0(DirectoryInfo Save)
            {
                return Save.LastWriteTime;
            }

            internal string _Lambda___17_1(DirectoryInfo Save)
            {
                return string.Format("{0}（上次修改：{1}）", Save.Name, StringUtils.FormatTimeSpan(Save.LastWriteTime - DateTime.Now, false));
            }

            public static readonly PageInstanceOverall._Closure___ _I = new PageInstanceOverall._Closure___();

            public static Func<DirectoryInfo, DateTime> _I17_0;

            public static Func<DirectoryInfo, string> _I17_1;
        }

        [CompilerGenerated]
        private sealed class _Closure___16_0
        {
            public _Closure___16_0(PageInstanceOverall._Closure___16_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                }
            }

            internal void _Lambda___R1(ModLoader.LoaderBase a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                switch (this._VB_Local_Loader.State)
                {
                case ModBase.LoadState.Finished:
                    ModMain.Hint(this._VB_Local_Loader.Name + "成功！", ModMain.HintType.Green, true);
                    return;
                case ModBase.LoadState.Failed:
                    ModMain.Hint(this._VB_Local_Loader.Name + "失败：" + this._VB_Local_Loader.Error.GetDisplay(false), ModMain.HintType.Red, true);
                    return;
                case ModBase.LoadState.Canceled:
                    ModMain.Hint(this._VB_Local_Loader.Name + "已取消！", ModMain.HintType.Blue, true);
                    return;
                default:
                    return;
                }
            }

            public ModLoader.LoaderCombo<string> _VB_Local_Loader;
        }
    }
}
