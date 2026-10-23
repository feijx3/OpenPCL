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
using PCL.My;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceOverall : MyPageRight
    {
        public PageInstanceOverall()
        {
            base.Loaded += this.PageSetupLaunch_Loaded;
            this.IsLoad = false;
            this.InitializeComponent();
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
                            MyComboBoxItem myComboBoxItem = (MyComboBoxItem)obj;
                            if (Operators.ConditionalCompareObjectEqual(myComboBoxItem.Tag, text, false) || (Operators.ConditionalCompareObjectEqual(myComboBoxItem.Tag, "PCL\\Logo.png", false) && text.EndsWithF("PCL\\Logo.png", false)))
                            {
                                this.ComboDisplayLogo.SelectedItem = myComboBoxItem;
                                break;
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
                        if (ModMain.MyMsgBox("确认要从版本列表中隐藏该版本吗？隐藏该版本后，它将不再出现于 PCL 显示的版本列表中。\r\n此后，在版本列表页面按下 F11 才可以查看被隐藏的版本。", "隐藏版本提示", "确定", "取消", "", false, true, false, null, null, null) != 1)
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
                string text2 = ModMain.MyMsgBoxInput("更改描述", "修改版本的描述文本，留空则使用 PCL 的默认描述。", text, new Collection<Validate>(), "默认描述", "确定", "取消", false);
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
                    string text = ModMain.MyMsgBoxInput("重命名版本", "", name, new Collection<Validate>
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
                PageInstanceOverall._Closure$__16-0 CS$<>8__locals1 = new PageInstanceOverall._Closure$__16-0(CS$<>8__locals1);
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
                        IEnumerator<ModLoader.LoaderBase> enumerator;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    CS$<>8__locals1.$VB$Local_Loader = new ModLoader.LoaderCombo<string>(PageInstanceLeft.Instance.Name + " 文件补全", ModDownload.DlClientFix(PageInstanceLeft.Instance, true, false));
                    CS$<>8__locals1.$VB$Local_Loader.OnStateChanged = delegate(ModLoader.LoaderBase a0)
                    {
                        base._Lambda$__0();
                    };
                    CS$<>8__locals1.$VB$Local_Loader.Start(PageInstanceLeft.Instance.Name, false);
                    ModLoader.LoaderTaskbarAdd<string>(CS$<>8__locals1.$VB$Local_Loader);
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
                    bool shiftKeyDown = MyWpfExtension.Computer.Keyboard.ShiftKeyDown;
                    bool flag = PageInstanceLeft.Instance.State != ModMinecraft.McInstance.McInstanceState.Error && Operators.CompareString(PageInstanceLeft.Instance.PathIndie, ModMinecraft.McFolderSelected, false) != 0;
                    DirectoryInfo info = DirectoryUtils.GetInfo(PageInstanceLeft.Instance.PathIndie + "saves\\");
                    List<string> list = ((flag && info.Exists) ? info.EnumerateDirectories().OrderByDescending<DirectoryInfo, DateTime>((PageInstanceOverall._Closure$__.$I17-0 == null) ? (PageInstanceOverall._Closure$__.$I17-0 = (DirectoryInfo Save) => Save.LastWriteTime) : PageInstanceOverall._Closure$__.$I17-0).Select<DirectoryInfo, string>((PageInstanceOverall._Closure$__.$I17-1 == null) ? (PageInstanceOverall._Closure$__.$I17-1 = (DirectoryInfo Save) => string.Format("{0}（上次修改：{1}）", Save.Name, StringUtils.FormatTimeSpan(Save.LastWriteTime - DateTime.Now, false))) : PageInstanceOverall._Closure$__.$I17-1)
                        .ToList<string>() : new List<string>());
                    int num = ModMain.MyMsgBox(string.Format("你确定要{0}删除版本 {1} 吗？", shiftKeyDown ? "永久" : "", PageInstanceLeft.Instance.Name) + (flag ? ("\r\n该版本对应的存档、资源包、Mod 等文件也将被一并删除！" + (list.Any<string>() ? ("\r\n\r\n这会删除以下存档：\r\n· " + list.Join("\r\n· ")) : "")) : ""), "版本删除确认", "确定", "取消", "", flag || shiftKeyDown, true, false, null, null, null);
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

        internal virtual Grid PanDisplayItem
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard PanDisplay
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanDisplayIcon
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboDisplayLogo
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
                SelectionChangedEventHandler selectionChangedEventHandler = delegate(object a0, SelectionChangedEventArgs a1)
                {
                    this.ComboDisplayLogo_SelectionChanged();
                };
                MyComboBox myComboBox = this._ComboDisplayLogo;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboDisplayLogo = value;
                myComboBox = this._ComboDisplayLogo;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual MyComboBoxItem ItemDisplayLogoCustom
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
                MouseButtonEventHandler mouseButtonEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.ComboDisplayLogo_SelectionChanged();
                };
                MyComboBoxItem myComboBoxItem = this._ItemDisplayLogoCustom;
                if (myComboBoxItem != null)
                {
                    myComboBoxItem.PreviewMouseLeftButtonUp -= mouseButtonEventHandler;
                }
                this._ItemDisplayLogoCustom = value;
                myComboBoxItem = this._ItemDisplayLogoCustom;
                if (myComboBoxItem != null)
                {
                    myComboBoxItem.PreviewMouseLeftButtonUp += mouseButtonEventHandler;
                }
            }
        }

        internal virtual MyComboBox ComboDisplayType
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
                MyComboBox myComboBox = this._ComboDisplayType;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboDisplayType = value;
                myComboBox = this._ComboDisplayType;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual MyButton BtnDisplayRename
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnDisplayRename_Click);
                MyButton myButton = this._BtnDisplayRename;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnDisplayRename = value;
                myButton = this._BtnDisplayRename;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnDisplayDesc
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnDisplayDesc_Click);
                MyButton myButton = this._BtnDisplayDesc;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnDisplayDesc = value;
                myButton = this._BtnDisplayDesc;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnDisplayStar
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnDisplayStar_Click);
                MyButton myButton = this._BtnDisplayStar;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnDisplayStar = value;
                myButton = this._BtnDisplayStar;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard PanFolder
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnFolderVersion
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnFolderVersion_Click();
                };
                MyButton myButton = this._BtnFolderVersion;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnFolderVersion = value;
                myButton = this._BtnFolderVersion;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnFolderSaves
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnFolderSaves_Click();
                };
                MyButton myButton = this._BtnFolderSaves;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnFolderSaves = value;
                myButton = this._BtnFolderSaves;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnFolderMods
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnFolderMods_Click();
                };
                MyButton myButton = this._BtnFolderMods;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnFolderMods = value;
                myButton = this._BtnFolderMods;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnFolderScreen
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnFolderScreen_Click);
                MyButton myButton = this._BtnFolderScreen;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnFolderScreen = value;
                myButton = this._BtnFolderScreen;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard PanManage
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnManageScript
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.BtnManageScript_Click();
                };
                MyButton myButton = this._BtnManageScript;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnManageScript = value;
                myButton = this._BtnManageScript;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnManageCheck
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageCheck_Click);
                MyButton myButton = this._BtnManageCheck;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnManageCheck = value;
                myButton = this._BtnManageCheck;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnManageDelete
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageDelete_Click);
                MyButton myButton = this._BtnManageDelete;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnManageDelete = value;
                myButton = this._BtnManageDelete;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        private bool IsLoad;

        public MyListItem InstanceHeaderItem;
    }
}
