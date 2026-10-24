using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic.CompilerServices;
using PCL.Our;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceMod : OurPageRight, IRefreshable
    {
        public PageInstanceMod()
        {
            base.Loaded += this._Lambda___R1_1;
            base.Initialized += this._Lambda___R1_2;
            base.PageExit += this.UnselectedAllWithAnimation;
            this.IsLoad = false;
            this.ModItems = new ConcurrentDictionary<string, OurLocalModItem>();
            this.BottomBarShownCount = 0;
            this.SelectedMods = new List<string>();
            this._Filter = PageInstanceMod.FilterType.All;
            this.InitializeComponent();
            this.SearchBox_WithEvents = this.SearchBox;
            this.BtnManageOpen_WithEvents = this.BtnManageOpen;
            this.BtnManageInstall_WithEvents = this.BtnManageInstall;
            this.BtnManageDownload_WithEvents = this.BtnManageDownload;
            this.BtnManageSelectAll_WithEvents = this.BtnManageSelectAll;
            this.BtnFilterAll_WithEvents = this.BtnFilterAll;
            this.BtnFilterEnabled_WithEvents = this.BtnFilterEnabled;
            this.BtnFilterDisabled_WithEvents = this.BtnFilterDisabled;
            this.BtnFilterCanUpdate_WithEvents = this.BtnFilterCanUpdate;
            this.BtnHintDownload_WithEvents = this.BtnHintDownload;
            this.BtnHintInstall_WithEvents = this.BtnHintInstall;
            this.BtnHintOpen_WithEvents = this.BtnHintOpen;
            this.Load_WithEvents = this.Load;
            this.BtnSelectUpdate_WithEvents = this.BtnSelectUpdate;
            this.BtnSelectEnable_WithEvents = this.BtnSelectEnable;
            this.BtnSelectDisable_WithEvents = this.BtnSelectDisable;
            this.BtnSelectDelete_WithEvents = this.BtnSelectDelete;
            this.BtnSelectCancel_WithEvents = this.BtnSelectCancel;
        }

        public void PageOther_Loaded()
        {
            if (ModMain.FrmMain.PageLast.Page != FormMain.PageType.ResourceDetail)
            {
                this.PanBack.ScrollToHome();
            }
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.SelectedMods.Clear();
                this.ReloadModList(false);
                this.ChangeAllSelected(false);
                ModAnimation.AniControlEnabled--;
                if (!this.IsLoad)
                {
                    this.IsLoad = true;
                    ModMain.FrmMain.KeyDown += this.FrmMain_KeyDown;
                    try
                    {
                        foreach (object obj in this.PanFilter.Children)
                        {
                            ((OurRadioButton)obj).LabText.Margin = new Thickness(-2.0, 0.0, 8.0, 0.0);
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
            }
        }

        public void ReloadModList(bool ForceReload = false)
        {
            if (this.LoaderRun(ForceReload ? ModLoader.LoaderFolderRunType.ForceRun : ModLoader.LoaderFolderRunType.RunOnUpdated))
            {
                Logger.Info("已刷新 Mod 列表", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                this.Filter = PageInstanceMod.FilterType.All;
                this.PanBack.ScrollToHome();
                this.SearchBox.Text = "";
            }
        }

        void IRefreshable.Refresh()
        {
            PageInstanceMod.Refresh();
        }

        public static void Refresh()
        {
            try
            {
                ResourceProject.Cache.Clear();
                ResourceVersion.ProjectFilesCache.Clear();
                FileUtils.Delete(ModBase.PathTemp + "Cache\\LocalMod.json", false);
                Logger.Info("由于点击刷新按钮，清理本地 Mod 信息缓存", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "强制刷新时清理本地 Mod 信息缓存失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
            }
            if (ModMain.FrmInstanceMod != null)
            {
                ModMain.FrmInstanceMod.ReloadModList(true);
            }
            ModMain.FrmInstanceLeft.ItemMod.Checked = true;
            ModMain.Hint("正在刷新……", ModMain.HintType.Blue, false);
        }

        private void LoaderInit()
        {
            base.PageLoaderInit<string, List<LocalResourceFile>>(this.Load, this.PanLoad, this.PanAllBack, null, LocalResourceLoaders.LocalResourceLoader, new Action<ModLoader.LoaderTask<string, List<LocalResourceFile>>>(this._Lambda___R7_3), null, false);
        }

        private void Load_Click(object sender, MouseButtonEventArgs e)
        {
            if (LocalResourceLoaders.LocalResourceLoader.State == ModBase.LoadState.Failed)
            {
                this.LoaderRun(ModLoader.LoaderFolderRunType.ForceRun);
            }
        }

        public bool LoaderRun(ModLoader.LoaderFolderRunType Type)
        {
            return ModLoader.LoaderFolderRun(LocalResourceLoaders.LocalResourceLoader, PageInstanceLeft.Instance.PathIndie + "mods\\", Type, 0, "", false);
        }

        private void LoadUIFromLoaderOutput()
        {
            try
            {
                if (LocalResourceLoaders.LocalResourceLoader.Output.Any<LocalResourceFile>())
                {
                    this.PanBack.Visibility = Visibility.Visible;
                    this.PanEmpty.Visibility = Visibility.Collapsed;
                    this.ModItems.Clear();
                    try
                    {
                        foreach (LocalResourceFile localResourceFile in LocalResourceLoaders.LocalResourceLoader.Output)
                        {
                            this.ModItems[localResourceFile.EnabledName] = this.McModListItem(localResourceFile);
                        }
                    }
                    finally
                    {
                        List<LocalResourceFile>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    this.Filter = PageInstanceMod.FilterType.All;
                    this.SearchBox.Text = "";
                    this.RefreshUI();
                }
                else
                {
                    this.PanEmpty.Visibility = Visibility.Visible;
                    this.PanBack.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "加载 Mod 列表 UI 失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
            }
        }

        private OurLocalModItem McModListItem(LocalResourceFile Entry)
        {
            PageInstanceMod._Closure___12_0 CS_locals1 = new PageInstanceMod._Closure___12_0();
            checked
            {
                ModAnimation.AniControlEnabled++;
                CS_locals1._VB_Local_NewItem = new OurLocalModItem
                {
                    SnapsToDevicePixels = true,
                    Entry = Entry,
                    ButtonHandler = new Action<OurLocalModItem, EventArgs>(this.McModContent),
                    Checked = this.SelectedMods.Contains(Entry.EnabledName)
                };
                Entry.OnUpdate += CS_locals1._Lambda___R4;
                ModAnimation.AniControlEnabled--;
                return CS_locals1._VB_Local_NewItem;
            }
        }

        private void McModContent(OurLocalModItem sender, EventArgs e)
        {
            sender.Changed += this._Lambda___R13_5;
            sender.Click += ((PageInstanceMod._Closure___._IR13_6 == null) ? (PageInstanceMod._Closure___._IR13_6 = new OurLocalModItem.ClickEventHandler(PageInstanceMod._Closure___._I._Lambda___R13_6)) : PageInstanceMod._Closure___._IR13_6);
            OurIconButton ourIconButton = new OurIconButton
            {
                LogoScale = 1.05,
                Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z",
                Tag = sender
            };
            ourIconButton.ToolTip = "打开文件位置";
            ourIconButton.Click += this._Lambda___R13_7;
            OurIconButton ourIconButton2 = new OurIconButton
            {
                LogoScale = 1.0,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                Tag = sender
            };
            ourIconButton2.ToolTip = "详情";
            ourIconButton2.Click += this.Info_Click;
            sender.MouseRightButtonUp += new MouseButtonEventHandler(this.Info_Click);
            OurIconButton ourIconButton3 = new OurIconButton
            {
                LogoScale = 1.0,
                Logo = "M520.192 0C408.43 0 317.44 82.87 313.563 186.734H52.736c-29.038 0-52.663 21.943-52.663 49.079s23.625 49.152 52.663 49.152h58.075v550.473c0 103.35 75.118 187.757 167.717 187.757h472.43c92.599 0 167.716-83.894 167.716-187.757V285.477h52.59c29.038 0 52.59-21.943 52.663-49.08-0.073-27.135-23.625-49.151-52.663-49.151H726.235C723.237 83.017 631.955 0 520.192 0zM404.846 177.957c3.803-50.03 50.176-89.015 107.447-89.015 57.197 0 103.57 38.985 106.788 89.015H404.92zM284.379 933.669c-33.353 0-69.997-39.351-69.997-95.525v-549.01H833.39v549.522c0 56.247-36.645 95.525-69.998 95.525H284.379v-0.512z M357.23 800.695a48.274 48.274 0 0 0 47.616-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.69 49.006V751.69c0 27.282 20.846 49.006 47.617 49.006z m166.62 0a48.274 48.274 0 0 0 47.688-49.006V471.7a48.274 48.274 0 0 0-47.689-49.08 48.274 48.274 0 0 0-47.543 49.006V751.69c0 27.282 21.431 49.006 47.543 49.006z m142.92 0a48.274 48.274 0 0 0 47.543-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.616 49.006V751.69c0 27.282 20.773 49.006 47.543 49.006z",
                Tag = sender
            };
            ourIconButton3.ToolTip = "删除";
            ourIconButton3.Click += this._Lambda___R13_8;
            OurIconButton ourIconButton4 = new OurIconButton
            {
                LogoScale = 1.0,
                Logo = (sender.Entry.IsEnabled ? "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z" : "M512 0a512 512 0 1 0 512 512A512 512 0 0 0 512 0z m0 921.6a409.6 409.6 0 1 1 409.6-409.6 409.6 409.6 0 0 1-409.6 409.6z M716.8 339.968l-256 253.44L328.192 460.8A51.2 51.2 0 0 0 256 532.992l168.448 168.96a51.2 51.2 0 0 0 72.704 0l289.28-289.792A51.2 51.2 0 0 0 716.8 339.968z"),
                Tag = sender
            };
            ourIconButton4.ToolTip = (sender.Entry.IsEnabled ? "禁用" : "启用");
            ourIconButton4.Click += this._Lambda___R13_9;
            sender.Buttons = new OurIconButton[] { ourIconButton2, ourIconButton, ourIconButton4, ourIconButton3 };
        }

        public void RefreshUI()
        {
            PageInstanceMod._Closure___14_0 CS_locals1 = new PageInstanceMod._Closure___14_0(null);
            checked
            {
                if (this.PanList != null)
                {
                    CS_locals1._VB_Local_ShowingMods = (this.IsSearching ? this.SearchResult : (LocalResourceLoaders.LocalResourceLoader.Output ?? new List<LocalResourceFile>())).Where<LocalResourceFile>(new Func<LocalResourceFile, bool>(this._Lambda___14_0)).ToList<LocalResourceFile>();
                    ModAnimation.AniControlEnabled++;
                    if (CS_locals1._VB_Local_ShowingMods.Any<LocalResourceFile>())
                    {
                        this.PanList.Visibility = Visibility.Visible;
                        this.PanList.Children.Clear();
                        try
                        {
                            foreach (LocalResourceFile localResourceFile in CS_locals1._VB_Local_ShowingMods)
                            {
                                OurLocalModItem ourLocalModItem = null;
                                if (this.ModItems.TryGetValue(localResourceFile.EnabledName, out ourLocalModItem))
                                {
                                    ourLocalModItem.Checked = this.SelectedMods.Contains(localResourceFile.EnabledName);
                                    this.PanList.Children.Add(ourLocalModItem);
                                }
                            }
                            goto IL_0104;
                        }
                        finally
                        {
                            List<LocalResourceFile>.Enumerator enumerator = default;
                            ((IDisposable)enumerator).Dispose();
                        }
                    }
                    this.PanList.Visibility = Visibility.Collapsed;
                    IL_0104:
                    ModAnimation.AniControlEnabled--;
                    this.SelectedMods = this.SelectedMods.Where<string>(new Func<string, bool>(CS_locals1._Lambda___1)).ToList<string>();
                    this.RefreshBars();
                }
            }
        }

        public void RefreshBars()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int count;
            bool flag;
            checked
            {
                try
                {
                    foreach (LocalResourceFile localResourceFile in (this.IsSearching ? this.SearchResult : (LocalResourceLoaders.LocalResourceLoader.Output ?? new List<LocalResourceFile>())))
                    {
                        num++;
                        if (localResourceFile.HasUpdate)
                        {
                            num3++;
                        }
                        if (localResourceFile.IsEnabled)
                        {
                            num2++;
                        }
                    }
                }
                finally
                {
                    List<LocalResourceFile>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                int num4 = num - num2;
                this.BtnFilterAll.Text = (this.IsSearching ? "搜索结果" : "全部") + string.Format(" ({0})", num);
                this.BtnFilterCanUpdate.Text = string.Format("可更新 ({0})", num3);
                this.BtnFilterCanUpdate.Visibility = ((this.Filter == PageInstanceMod.FilterType.CanUpdate || num3 > 0) ? Visibility.Visible : Visibility.Collapsed);
                this.BtnFilterEnabled.Text = string.Format("启用 ({0})", num2);
                this.BtnFilterEnabled.Visibility = ((this.Filter == PageInstanceMod.FilterType.Enabled || (num2 > 0 && num2 < num)) ? Visibility.Visible : Visibility.Collapsed);
                this.BtnFilterDisabled.Text = string.Format("禁用 ({0})", num4);
                this.BtnFilterDisabled.Visibility = ((this.Filter == PageInstanceMod.FilterType.Disabled || num4 > 0) ? Visibility.Visible : Visibility.Collapsed);
                count = this.SelectedMods.Count;
                flag = count > 0;
                if (flag)
                {
                    this.LabSelect.Text = string.Format("已选择 {0} 个文件", count);
                }
                if (flag)
                {
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    try
                    {
                        foreach (LocalResourceFile localResourceFile2 in LocalResourceLoaders.LocalResourceLoader.Output)
                        {
                            if (this.SelectedMods.Contains(localResourceFile2.EnabledName))
                            {
                                if (localResourceFile2.HasUpdate)
                                {
                                    flag2 = true;
                                }
                                if (localResourceFile2.IsEnabled)
                                {
                                    flag3 = true;
                                }
                                if (!localResourceFile2.IsEnabled)
                                {
                                    flag4 = true;
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<LocalResourceFile>.Enumerator enumerator2 = default;
                        ((IDisposable)enumerator2).Dispose();
                    }
                    this.BtnSelectDisable.IsEnabled = flag3;
                    this.BtnSelectEnable.IsEnabled = flag4;
                    this.BtnSelectUpdate.IsEnabled = flag2;
                }
            }
            if (ModAnimation.AniControlEnabled == 0)
            {
                this.PanListBack.Margin = new Thickness(0.0, 0.0, 0.0, (double)(flag ? 95 : 15));
                if (flag)
                {
                    if (this.BottomBarShownCount >= count)
                    {
                        this.BottomBarShownCount = count;
                        return;
                    }
                    this.BottomBarShownCount = count;
                    this.CardSelect.Visibility = Visibility.Visible;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.CardSelect, 1.0 - this.CardSelect.Opacity, 60, 0, null, false),
                        ModAnimation.AaTranslateY(this.CardSelect, -27.0 - this.TransSelect.Y, 120, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaTranslateY(this.CardSelect, 3.0, 150, 120, new ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Weak, 0.5), false),
                        ModAnimation.AaTranslateY(this.CardSelect, -1.0, 90, 270, new ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Weak, 0.5), false)
                    }, "Mod Sidebar", false);
                    return;
                }
                else if (this.BottomBarShownCount != 0)
                {
                    this.BottomBarShownCount = 0;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.CardSelect, -this.CardSelect.Opacity, 90, 0, null, false),
                        ModAnimation.AaTranslateY(this.CardSelect, -10.0 - this.TransSelect.Y, 90, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaCode(new ThreadStart(this._Lambda___15_0), 0, true)
                    }, "Mod Sidebar", false);
                    return;
                }
            }
            else
            {
                ModAnimation.AniStop("Mod Sidebar");
                this.BottomBarShownCount = count;
                if (flag)
                {
                    this.CardSelect.Visibility = Visibility.Visible;
                    this.CardSelect.Opacity = 1.0;
                    this.TransSelect.Y = -25.0;
                    return;
                }
                this.CardSelect.Visibility = Visibility.Collapsed;
                this.CardSelect.Opacity = 0.0;
                this.TransSelect.Y = -10.0;
            }
        }

        private void BtnManageOpen_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryUtils.Create(PageInstanceLeft.Instance.PathIndie + "mods\\");
                ModBase.OpenExplorer(PageInstanceLeft.Instance.PathIndie + "mods\\");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "打开 Mods 文件夹失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
            }
        }

        private void BtnManageSelectAll_Click(object sender, MouseButtonEventArgs e)
        {
            this.ChangeAllSelected(this.SelectedMods.Count < this.PanList.Children.Count);
        }

        private void BtnManageInstall_Click(object sender, MouseButtonEventArgs e)
        {
            List<string> list = Dialogs.SelectFile("选择要安装的 Mod", true, null, new OneOf<IEnumerable<ValueTuple<string[], string>>, string>?(new ValueTuple<string[], string>[]
            {
                new ValueTuple<string[], string>(new string[] { "jar", "litemod", "disabled", "old" }, "Mod 文件")
            }));
            if (list.Any<string>())
            {
                PageInstanceMod.InstallMods(list);
            }
        }

        public static bool InstallMods(IEnumerable<string> FilePathList)
        {
            PageInstanceMod._Closure___20_0 CS_locals1 = new PageInstanceMod._Closure___20_0(null);
            CS_locals1._VB_Local_Extension = PathUtils.GetExtension(FilePathList.First<string>());
            bool flag;
            if (!new string[] { "jar", "litemod", "disabled", "old" }.Any<string>(new Func<string, bool>(CS_locals1._Lambda___0)))
            {
                flag = false;
            }
            else
            {
                Logger.Info("文件为 jar/litemod 格式，尝试作为 Mod 安装", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                if (FilePathList.First<string>().Contains(":\\$RECYCLE.BIN\\"))
                {
                    ModMain.Hint("请先将文件从回收站还原，再尝试安装！", ModMain.HintType.Red, true);
                    flag = true;
                }
                else
                {
                    ModMinecraft.McInstance mcInstance = ModMinecraft.McInstanceSelected;
                    if (ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSetup)
                    {
                        mcInstance = PageInstanceLeft.Instance;
                    }
                    if (ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSelect || ((object)mcInstance == (object)null) || !mcInstance.Modable)
                    {
                        ModMain.Hint("若要安装 Mod，请先选择一个可以安装 Mod 的版本！", ModMain.HintType.Blue, true);
                    }
                    else if ((ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSetup && ModMain.FrmMain.PageCurrentSub == FormMain.PageSubType.DownloadPack) || ModMain.OurMsgBox(string.Format("是否要将这{0}文件作为 Mod 安装到 {1}？", FilePathList.IsSingle<string>() ? "个" : "些", mcInstance.Name), "Mod 安装确认", "确定", "取消", "", false, true, false, null, null, null) == 1)
                    {
                        try
                        {
                            try
                            {
                                foreach (string text in FilePathList)
                                {
                                    string text2 = new LocalResourceFile(text).EnabledName;
                                    if (!text2.Contains("."))
                                    {
                                        text2 += ".jar";
                                    }
                                    text2 = mcInstance.PathIndie + "mods\\" + text2;
                                    if (!PathUtils.IsSame(text, text2))
                                    {
                                        FileUtils.Delete(text2, true);
                                        FileUtils.Copy(text, text2);
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
                            if (FilePathList.IsSingle<string>())
                            {
                                ModMain.Hint(string.Format("已安装 {0}！", new LocalResourceFile(FilePathList.First<string>()).EnabledName), ModMain.HintType.Green, true);
                            }
                            else
                            {
                                ModMain.Hint(string.Format("已安装 {0} 个 Mod！", FilePathList.Count<string>()), ModMain.HintType.Green, true);
                            }
                            if (ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSetup && ModMain.FrmMain.PageCurrentSub == FormMain.PageSubType.DownloadPack)
                            {
                                ModLoader.LoaderFolderRun(LocalResourceLoaders.LocalResourceLoader, mcInstance.PathIndie + "mods\\", ModLoader.LoaderFolderRunType.ForceRun, 0, "", false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "复制 Mod 文件失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                        }
                    }
                    flag = true;
                }
            }
            return flag;
        }

        private void BtnManageDownload_Click(object sender, MouseButtonEventArgs e)
        {
            PageResource.TargetInstance = PageInstanceLeft.Instance;
            PageResource.TargetName = this.SearchBox.Text;
            ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.DownloadMod);
        }

        public void CheckChanged(OurLocalModItem sender, ModBase.RouteEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                string enabledName = sender.Entry.EnabledName;
                if (sender.Checked)
                {
                    if (!this.SelectedMods.Contains(enabledName))
                    {
                        this.SelectedMods.Add(enabledName);
                    }
                }
                else
                {
                    this.SelectedMods.Remove(enabledName);
                }
                this.RefreshBars();
            }
        }

        private void ChangeAllSelected(bool Value)
        {
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.SelectedMods.Clear();
                try
                {
                    foreach (OurLocalModItem ourLocalModItem in this.ModItems.Values)
                    {
                        bool flag = Value && this.PanList.Children.Contains(ourLocalModItem);
                        ourLocalModItem.Checked = flag;
                        if (flag)
                        {
                            this.SelectedMods.Add(ourLocalModItem.Entry.EnabledName);
                        }
                    }
                }
                finally
                {
                    IEnumerator<OurLocalModItem> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                ModAnimation.AniControlEnabled--;
                this.RefreshBars();
            }
        }

        private void UnselectedAllWithAnimation()
        {
            int aniControlEnabled = ModAnimation.AniControlEnabled;
            ModAnimation.AniControlEnabled = 0;
            this.ChangeAllSelected(false);
            checked
            {
                ModAnimation.AniControlEnabled += aniControlEnabled;
            }
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (ModMain.FrmMain.PageRight == this && OurWpfExtension.Computer.Keyboard.CtrlKeyDown && e.Key == Key.A)
            {
                this.ChangeAllSelected(true);
            }
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.SearchBox.Text.Any<char>() && OurWpfExtension.Computer.Keyboard.CtrlKeyDown && e.Key == Key.A)
            {
                this.PanBack.Focus();
            }
        }

        public PageInstanceMod.FilterType Filter
        {
            get
            {
                return this._Filter;
            }
            set
            {
                if (this._Filter != value)
                {
                    this._Filter = value;
                    switch (value)
                    {
                    case PageInstanceMod.FilterType.All:
                        this.BtnFilterAll.Checked = true;
                        break;
                    case PageInstanceMod.FilterType.Enabled:
                        this.BtnFilterEnabled.Checked = true;
                        break;
                    case PageInstanceMod.FilterType.Disabled:
                        this.BtnFilterDisabled.Checked = true;
                        break;
                    default:
                        this.BtnFilterCanUpdate.Checked = true;
                        break;
                    }
                    this.RefreshUI();
                }
            }
        }

        private bool CanPassFilter(LocalResourceFile CheckingMod)
        {
            bool flag;
            switch (this.Filter)
            {
            case PageInstanceMod.FilterType.All:
                flag = true;
                break;
            case PageInstanceMod.FilterType.Enabled:
                flag = CheckingMod.IsEnabled;
                break;
            case PageInstanceMod.FilterType.Disabled:
                flag = !CheckingMod.IsEnabled;
                break;
            case PageInstanceMod.FilterType.CanUpdate:
                flag = CheckingMod.HasUpdate;
                break;
            default:
                flag = false;
                break;
            }
            return flag;
        }

        private void ChangeFilter(OurRadioButton sender, bool raiseByMouse)
        {
            this.Filter = (PageInstanceMod.FilterType)Conversions.ToInteger(sender.Tag);
            this.RefreshUI();
        }

        private void BtnSelectED_Click(OurIconTextButton sender, ModBase.RouteEventArgs e)
        {
            this.EDMods(LocalResourceLoaders.LocalResourceLoader.Output.Where<LocalResourceFile>(new Func<LocalResourceFile, bool>(this._Lambda___35_0)), !sender.Equals(this.BtnSelectDisable));
            this.ChangeAllSelected(false);
        }

        private void EDMods(IEnumerable<LocalResourceFile> ModList, bool IsEnable)
        {
            bool flag = true;
            try
            {
                foreach (LocalResourceFile localResourceFile in ModList.ToList<LocalResourceFile>())
                {
                    PageInstanceMod._Closure___36_0 CS_locals1 = new PageInstanceMod._Closure___36_0(null);
                    CS_locals1._VB_Local_ModEntity = localResourceFile;
                    string text = null;
                    if (CS_locals1._VB_Local_ModEntity.IsEnabled && !IsEnable)
                    {
                        text = PathUtils.RemoveExtendedPrefix(CS_locals1._VB_Local_ModEntity.File.FullName) + (FileUtils.Exists(CS_locals1._VB_Local_ModEntity.File.FullName + ".old") ? ".old" : ".disabled");
                    }
                    else
                    {
                        if (CS_locals1._VB_Local_ModEntity.IsEnabled || !IsEnable)
                        {
                            continue;
                        }
                        text = CS_locals1._VB_Local_ModEntity.EnabledFullName;
                    }
                    try
                    {
                        if (FileUtils.Exists(text))
                        {
                            if (!FileUtils.Exists(CS_locals1._VB_Local_ModEntity.File.FullName))
                            {
                                Logger.Warn("Mod 的状态已被切换", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                                continue;
                            }
                            if (Operators.CompareString(CryptographyUtils.ComputeFileHash(CS_locals1._VB_Local_ModEntity.File.FullName, CryptographyUtils.HashMethod.Md5), CryptographyUtils.ComputeFileHash(text, CryptographyUtils.HashMethod.Md5), false) != 0)
                            {
                                ModMain.OurMsgBox(string.Format("目前同时存在启用和禁用的两个 Mod 文件：{0} - {1}{2} - {3}{4}{5}注意，这两个文件的内容并不相同。{6}在手动删除或重命名其中一个文件后，才能继续操作。", new object[]
                                {
                                    "\r\n",
                                    PathUtils.GetLastPart(text),
                                    "\r\n",
                                    CS_locals1._VB_Local_ModEntity.File.Name,
                                    "\r\n",
                                    "\r\n",
                                    "\r\n"
                                }), "存在文件冲突", "确定", "", "", false, true, false, null, null, null);
                                continue;
                            }
                        }
                        FileUtils.Move(CS_locals1._VB_Local_ModEntity.File.FullName, text, false);
                    }
                    catch (FileNotFoundException ex)
                    {
                        Logger.Error(ex, string.Format("未找到需要重命名的 Mod（{0}）", CS_locals1._VB_Local_ModEntity.File.FullName ?? "null"), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                        this.ReloadModList(true);
                        return;
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, string.Format("重命名 Mod 失败（{0}）", CS_locals1._VB_Local_ModEntity.File.FullName ?? "null"), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                        flag = false;
                    }
                    LocalResourceFile localResourceFile2 = new LocalResourceFile(text);
                    localResourceFile2.FromJson(CS_locals1._VB_Local_ModEntity.ToJson());
                    if (LocalResourceLoaders.LocalResourceLoader.Output.Contains(CS_locals1._VB_Local_ModEntity))
                    {
                        int num = LocalResourceLoaders.LocalResourceLoader.Output.IndexOf(CS_locals1._VB_Local_ModEntity);
                        LocalResourceLoaders.LocalResourceLoader.Output.RemoveAt(num);
                        LocalResourceLoaders.LocalResourceLoader.Output.Insert(num, localResourceFile2);
                    }
                    if (this.SearchResult != null && this.SearchResult.Contains(CS_locals1._VB_Local_ModEntity))
                    {
                        int num2 = this.SearchResult.IndexOf(CS_locals1._VB_Local_ModEntity);
                        this.SearchResult.Remove(CS_locals1._VB_Local_ModEntity);
                        this.SearchResult.Insert(num2, localResourceFile2);
                    }
                    OurLocalModItem ourLocalModItem = this.McModListItem(localResourceFile2);
                    this.ModItems[CS_locals1._VB_Local_ModEntity.EnabledName] = ourLocalModItem;
                    int num3 = this.PanList.Children.IndexOf(this.PanList.Children.OfType<OurLocalModItem>().FirstOrDefault<OurLocalModItem>(new Func<OurLocalModItem, bool>(CS_locals1._Lambda___0)));
                    if (num3 != -1)
                    {
                        this.PanList.Children.RemoveAt(num3);
                        this.PanList.Children.Insert(num3, ourLocalModItem);
                        ourLocalModItem.UpdateLayout();
                    }
                }
            }
            finally
            {
                List<LocalResourceFile>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            if (flag)
            {
                this.RefreshBars();
            }
            else
            {
                ModMain.Hint("由于文件被占用，Mod 的状态切换失败，请尝试关闭正在运行的游戏后再试！", ModMain.HintType.Red, true);
                this.ReloadModList(true);
            }
            this.LoaderRun(ModLoader.LoaderFolderRunType.UpdateOnly);
        }

        private void BtnSelectUpdate_Click()
        {
            List<LocalResourceFile> list = LocalResourceLoaders.LocalResourceLoader.Output.Where<LocalResourceFile>(new Func<LocalResourceFile, bool>(this._Lambda___37_0)).ToList<LocalResourceFile>();
            if (list.Any<LocalResourceFile>())
            {
                this.UpdateMods(list);
                this.ChangeAllSelected(false);
            }
        }

        public void UpdateMods(IEnumerable<LocalResourceFile> ModList)
        {
            PageInstanceMod._Closure___39_0 CS_locals1 = new PageInstanceMod._Closure___39_0(null);
            CS_locals1._VB_Local_ModList = ModList;
            if (!Settings.Get<bool>("HintUpdateMod", null) || CS_locals1._VB_Local_ModList.Count<LocalResourceFile>() >= 15)
            {
                if (ModMain.OurMsgBox(string.Format("新版本 Mod 可能不兼容旧存档或者其他 Mod，这可能导致游戏崩溃，甚至永久损坏存档！{0}如果你在游玩整合包，请千万不要自行更新 Mod！{1}{2}在更新前，请先备份存档，并检查 Mod 的更新日志。{3}如果更新后出现问题，你也可以在回收站找回更新前的 Mod。", new object[] { "\r\n", "\r\n", "\r\n", "\r\n" }), "Mod 更新警告", "我已了解风险，继续更新", "取消", "", true, true, false, null, null, null) != 1)
                {
                    return;
                }
                Settings.Set("HintUpdateMod", true, null);
            }
            try
            {
                PageInstanceMod._Closure___39_1 CS_locals2 = new PageInstanceMod._Closure___39_1(null);
                CS_locals2._VB_NonLocal__VB_Closure_2 = CS_locals1;
                CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_ModList = CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_ModList.ToList<LocalResourceFile>();
                List<ModNet.NetFile> list = new List<ModNet.NetFile>();
                CS_locals2._VB_Local_FileCopyList = new Dictionary<string, string>();
                List<ModLoader.LoaderBase> list4;
                checked
                {
                    try
                    {
                        foreach (LocalResourceFile localResourceFile in CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_ModList)
                        {
                            ResourceVersion updateFile = localResourceFile.UpdateFile;
                            if (updateFile.DownloadAvailable)
                            {
                                string text = PathUtils.GetFileNameWithoutExtension(localResourceFile.ProjectVersion.FileName);
                                string text2 = PathUtils.GetFileNameWithoutExtension(localResourceFile.UpdateFile.FileName);
                                List<string> list2 = text.Split(new char[] { '-' }).ToList<string>();
                                List<string> list3 = text2.Split(new char[] { '-' }).ToList<string>();
                                bool flag = false;
                                while (list2.Any<string>() && list3.Any<string>() && Operators.CompareString(list2.First<string>(), list3.First<string>(), false) == 0)
                                {
                                    list2.RemoveAt(0);
                                    list3.RemoveAt(0);
                                    flag = true;
                                }
                                while (list2.Any<string>() && list3.Any<string>() && Operators.CompareString(list2.Last<string>(), list3.Last<string>(), false) == 0)
                                {
                                    list2.RemoveAt(list2.Count - 1);
                                    list3.RemoveAt(list3.Count - 1);
                                    flag = true;
                                }
                                if (flag && list2.Any<string>() && list3.Any<string>())
                                {
                                    text = list2.Join("-");
                                    text2 = list3.Join("-");
                                }
                                string text3 = ModBase.PathTemp + "mods\\" + localResourceFile.File.Name.Replace(text, text2);
                                string text4 = Path.Combine(PathUtils.RemoveLastPart(localResourceFile.File.FullName), localResourceFile.File.Name.Replace(text, text2));
                                list.Add(updateFile.ToNetFile(text3, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Update), PageInstanceLeft.Instance.Version.VanillaName, PageInstanceLeft.Instance.Version.ModLoaders));
                                CS_locals2._VB_Local_FileCopyList[text3] = text4;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<LocalResourceFile> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    list4 = new List<ModLoader.LoaderBase>();
                    CS_locals2._VB_Local_FinishedFileNames = new List<string>();
                }
                list4.Add(new ModNet.LoaderDownload("下载新版 Mod 文件", list)
                {
                    ProgressWeight = (double)CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_ModList.Count<LocalResourceFile>() * 1.5
                });
                list4.Add(new ModLoader.LoaderTask<int, int>("替换旧版 Mod 文件", new Action<ModLoader.LoaderTask<int, int>>(CS_locals2._Lambda___R10), null, ThreadPriority.Normal));
                CS_locals2._VB_Local_Loader = new ModLoader.LoaderCombo<IEnumerable<LocalResourceFile>>("Mod 更新：" + PageInstanceLeft.Instance.Name, list4);
                CS_locals2._VB_Local_PathMods = PageInstanceLeft.Instance.PathIndie + "mods\\";
                CS_locals2._VB_Local_Loader.OnStateChanged = new Action<ModLoader.LoaderBase>(CS_locals2._Lambda___R11);
                Logger.Info(string.Format("开始更新 {0} 个 Mod：{1}", CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_ModList.Count<LocalResourceFile>(), CS_locals2._VB_NonLocal__VB_Closure_2._VB_Local_ModList.Select<LocalResourceFile, string>((PageInstanceMod._Closure___._I39_3 == null) ? (PageInstanceMod._Closure___._I39_3 = new Func<LocalResourceFile, string>(PageInstanceMod._Closure___._I._Lambda___39_3)) : PageInstanceMod._Closure___._I39_3).Join("，")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                PageInstanceMod.UpdatingInstanceModFolders.Add(CS_locals2._VB_Local_PathMods);
                CS_locals2._VB_Local_Loader.Start(null, false);
                ModLoader.LoaderTaskbarAdd<IEnumerable<LocalResourceFile>>(CS_locals2._VB_Local_Loader);
                ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                ModMain.FrmMain.BtnExtraDownload.Ribble();
                this.ReloadModList(true);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "初始化 Mod 更新失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
            }
        }

        private void BtnSelectDelete_Click()
        {
            this.DeleteMods(LocalResourceLoaders.LocalResourceLoader.Output.Where<LocalResourceFile>(new Func<LocalResourceFile, bool>(this._Lambda___40_0)));
            this.ChangeAllSelected(false);
        }

        private void DeleteMods(IEnumerable<LocalResourceFile> ModsToDelete)
        {
            try
            {
                bool flag = true;
                bool shiftKeyDown = OurWpfExtension.Computer.Keyboard.ShiftKeyDown;
                List<LocalResourceFile> list = ModsToDelete.SelectMany<LocalResourceFile, string>((PageInstanceMod._Closure___._I41_0 == null) ? (PageInstanceMod._Closure___._I41_0 = new Func<LocalResourceFile, IEnumerable<string>>(PageInstanceMod._Closure___._I._Lambda___41_0)) : PageInstanceMod._Closure___._I41_0).Distinct<string>().Where<string>((PageInstanceMod._Closure___._I41_1 == null) ? (PageInstanceMod._Closure___._I41_1 = new Func<string, bool>(PageInstanceMod._Closure___._I._Lambda___41_1)) : PageInstanceMod._Closure___._I41_1)
                    .Select<string, LocalResourceFile>((PageInstanceMod._Closure___._I41_2 == null) ? (PageInstanceMod._Closure___._I41_2 = new Func<string, LocalResourceFile>(PageInstanceMod._Closure___._I._Lambda___41_2)) : PageInstanceMod._Closure___._I41_2)
                    .ToList<LocalResourceFile>();
                try
                {
                    List<LocalResourceFile>.Enumerator enumerator = list.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        PageInstanceMod._Closure___41_0 CS_locals1 = new PageInstanceMod._Closure___41_0(null);
                        CS_locals1._VB_Local_ModEntity = enumerator.Current;
                        try
                        {
                            FileUtils.Delete(CS_locals1._VB_Local_ModEntity.File.FullName, !shiftKeyDown);
                        }
                        catch (OperationCanceledException ex)
                        {
                            Logger.Warn(ex, "删除 Mod 被主动取消", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                            this.ReloadModList(true);
                            return;
                        }
                        catch (Exception ex2)
                        {
                            Logger.Error(ex2, string.Format("删除 Mod 失败（{0}）", CS_locals1._VB_Local_ModEntity.File.FullName), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                            flag = false;
                        }
                        this.SelectedMods.Remove(CS_locals1._VB_Local_ModEntity.EnabledName);
                        LocalResourceLoaders.LocalResourceLoader.Output.Remove(CS_locals1._VB_Local_ModEntity);
                        List<LocalResourceFile> searchResult = this.SearchResult;
                        if (searchResult != null)
                        {
                            searchResult.Remove(CS_locals1._VB_Local_ModEntity);
                        }
                        this.ModItems.Remove(CS_locals1._VB_Local_ModEntity.EnabledName);
                        int num = this.PanList.Children.IndexOf(this.PanList.Children.OfType<OurLocalModItem>().FirstOrDefault<OurLocalModItem>(new Func<OurLocalModItem, bool>(CS_locals1._Lambda___3)));
                        if (num >= 0)
                        {
                            this.PanList.Children.RemoveAt(num);
                        }
                    }
                }
                finally
                {
                    List<LocalResourceFile>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                this.RefreshBars();
                if (!flag)
                {
                    ModMain.Hint("由于文件被占用，Mod 删除失败，请尝试关闭正在运行的游戏后再试！", ModMain.HintType.Red, true);
                    this.ReloadModList(true);
                }
                else if (this.PanList.Children.Count == 0)
                {
                    this.ReloadModList(true);
                }
                else
                {
                    this.RefreshBars();
                }
                if (!flag)
                {
                    return;
                }
                if (list.IsSingle<LocalResourceFile>())
                {
                    ModMain.Hint(string.Format("已删除 {0}！", list.Single<LocalResourceFile>().File.Name), ModMain.HintType.Green, true);
                }
                else
                {
                    ModMain.Hint(string.Format("已删除 {0} 个文件！", list.Count), ModMain.HintType.Green, true);
                }
            }
            catch (OperationCanceledException ex3)
            {
                Logger.Warn(ex3, "删除 Mod 被主动取消", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                this.ReloadModList(true);
            }
            catch (Exception ex4)
            {
                Logger.Error(ex4, "删除 Mod 出现未知错误", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                this.ReloadModList(true);
            }
            this.LoaderRun(ModLoader.LoaderFolderRunType.UpdateOnly);
        }

        private void BtnSelectCancel_Click()
        {
            this.ChangeAllSelected(false);
        }

        public void Info_Click(object sender, EventArgs e)
        {
            checked
            {
                try
                {
                    LocalResourceFile entry = ((OurLocalModItem)((sender is OurIconButton) ? NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) : sender)).Entry;
                    if (((object)(entry.Project) != (object)null))
                    {
                        ModMain.FrmMain.PageChange(new FormMain.PageStackData
                        {
                            Page = FormMain.PageType.ResourceDetail,
                            Additional = new object[]
                            {
                                entry.Project,
                                new List<string>(),
                                PageInstanceLeft.Instance.Version.VanillaName,
                                PageInstanceLeft.Instance.Version.ModLoaders,
                                ResourceTypes.Mod
                            }
                        }, FormMain.PageSubType.Default);
                    }
                    else
                    {
                        List<string> list = new List<string>();
                        if (entry.Description != null)
                        {
                            list.Add(entry.Description + "\r\n");
                        }
                        list.Add(string.Concat(new string[]
                        {
                            "文件：",
                            entry.File.Name,
                            "（",
                            StringUtils.FormatByteSize(entry.File.Length),
                            "）"
                        }));
                        if (entry.Version != null)
                        {
                            list.Add("版本：" + entry.Version);
                        }
                        List<string> list2 = new List<string>();
                        if (list2.Any<string>())
                        {
                            list.Add("");
                            list.AddRange(list2);
                        }
                        string text = entry.Display.Replace(" ", "+");
                        string text2 = text.Substring(0, 1);
                        int num = text.Count<char>() - 1;
                        for (int i = 1; i <= num; i++)
                        {
                            bool flag = text[i - 1].ToString().Lower().Equals(text[i - 1].ToString());
                            bool flag2 = text[i].ToString().Lower().Equals(text[i].ToString());
                            if (flag && !flag2)
                            {
                                text2 += "+";
                            }
                            text2 += Conversions.ToString(text[i]);
                        }
                        text2 = text2.Replace("++", "+").Replace("pti+Fine", "ptiFine");
                        if (ModMain.OurMsgBox(list.Join("\r\n"), entry.Display, "百科搜索", "返回", "", false, true, false, null, null, null) == 1)
                        {
                            ModBase.OpenWebsite("https://www.mcmod.cn/s?key=" + text2 + "&site=all&filter=0");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "获取 Mod 详情失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                }
            }
        }

        public void Open_Click(OurIconButton sender, EventArgs e)
        {
            try
            {
                ModBase.OpenExplorer(((OurLocalModItem)sender.Tag).Entry.File.FullName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "打开 Mod 文件位置失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
            }
        }

        public void Delete_Click(OurIconButton sender, EventArgs e)
        {
            OurLocalModItem ourLocalModItem = (OurLocalModItem)sender.Tag;
            this.DeleteMods(new LocalResourceFile[] { ourLocalModItem.Entry });
        }

        public void ED_Click(OurIconButton sender, EventArgs e)
        {
            OurLocalModItem ourLocalModItem = (OurLocalModItem)sender.Tag;
            this.EDMods(new LocalResourceFile[] { ourLocalModItem.Entry }, !ourLocalModItem.Entry.IsEnabled);
        }

        public bool IsSearching
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.SearchBox.Text);
            }
        }

        public void SearchRun()
        {
            if (this.IsSearching)
            {
                List<ModBase.SearchEntry<LocalResourceFile>> list = new List<ModBase.SearchEntry<LocalResourceFile>>();
                try
                {
                    foreach (LocalResourceFile localResourceFile in LocalResourceLoaders.LocalResourceLoader.Output)
                    {
                        List<ModBase.SearchSource> list2 = new List<ModBase.SearchSource>();
                        list2.Add(new ModBase.SearchSource(localResourceFile.Display, 1.0));
                        list2.Add(new ModBase.SearchSource(localResourceFile.File.Name, 1.0));
                        if (localResourceFile.Version != null)
                        {
                            list2.Add(new ModBase.SearchSource(localResourceFile.Version, 0.2));
                        }
                        if (localResourceFile.Description != null && Operators.CompareString(localResourceFile.Description, "", false) != 0)
                        {
                            list2.Add(new ModBase.SearchSource(localResourceFile.Description, 0.4));
                        }
                        if (((object)(localResourceFile.Project) != (object)null))
                        {
                            if (Operators.CompareString(localResourceFile.Project.RawName, localResourceFile.Display, false) != 0)
                            {
                                list2.Add(new ModBase.SearchSource(localResourceFile.Project.RawName, 1.0));
                            }
                            if (Operators.CompareString(localResourceFile.Project.TranslatedName, localResourceFile.Project.RawName, false) != 0)
                            {
                                list2.Add(new ModBase.SearchSource(localResourceFile.Project.TranslatedName, 1.0));
                            }
                            if (Operators.CompareString(localResourceFile.Project.Description, localResourceFile.Description, false) != 0)
                            {
                                list2.Add(new ModBase.SearchSource(localResourceFile.Project.Description, 0.4));
                            }
                            list2.Add(new ModBase.SearchSource(string.Join("", localResourceFile.Project.Tags), 0.2));
                        }
                        list.Add(new ModBase.SearchEntry<LocalResourceFile>
                        {
                            Item = localResourceFile,
                            SearchSource = list2
                        });
                    }
                }
                finally
                {
                    List<LocalResourceFile>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                this.SearchResult = ModBase.Search<LocalResourceFile>(list, this.SearchBox.Text, 6, 0.35).Select<ModBase.SearchEntry<LocalResourceFile>, LocalResourceFile>((PageInstanceMod._Closure___._I50_0 == null) ? (PageInstanceMod._Closure___._I50_0 = new Func<ModBase.SearchEntry<LocalResourceFile>, LocalResourceFile>(PageInstanceMod._Closure___._I._Lambda___50_0)) : PageInstanceMod._Closure___._I50_0).ToList<LocalResourceFile>();
            }
            this.RefreshUI();
        }




        internal virtual OurSearchBox SearchBox_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._SearchBox;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.SearchBox_PreviewKeyDown);
                OurSearchBox.TextChangedEventHandler textChangedEventHandler = new OurSearchBox.TextChangedEventHandler(this._Lambda___R65_12);
                OurSearchBox ourSearchBox = this._SearchBox;
                if (ourSearchBox != null)
                {
                    ourSearchBox.PreviewKeyDown -= keyEventHandler;
                    ourSearchBox.TextChanged -= textChangedEventHandler;
                }
                this._SearchBox = value;
                ourSearchBox = this._SearchBox;
                if (ourSearchBox != null)
                {
                    ourSearchBox.PreviewKeyDown += keyEventHandler;
                    ourSearchBox.TextChanged += textChangedEventHandler;
                }
            }
        }


        internal virtual OurButton BtnManageOpen_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnManageOpen;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageOpen_Click);
                OurButton ourButton = this._BtnManageOpen;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnManageOpen = value;
                ourButton = this._BtnManageOpen;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnManageInstall_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnManageInstall;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageInstall_Click);
                OurButton ourButton = this._BtnManageInstall;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnManageInstall = value;
                ourButton = this._BtnManageInstall;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnManageDownload_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnManageDownload;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageDownload_Click);
                OurButton ourButton = this._BtnManageDownload;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnManageDownload = value;
                ourButton = this._BtnManageDownload;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnManageSelectAll_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnManageSelectAll;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageSelectAll_Click);
                OurButton ourButton = this._BtnManageSelectAll;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnManageSelectAll = value;
                ourButton = this._BtnManageSelectAll;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }



        internal virtual OurRadioButton BtnFilterAll_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFilterAll;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurRadioButton.CheckEventHandler checkEventHandler = new OurRadioButton.CheckEventHandler(this._Lambda___R97_13);
                OurRadioButton ourRadioButton = this._BtnFilterAll;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterAll = value;
                ourRadioButton = this._BtnFilterAll;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurRadioButton BtnFilterEnabled_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFilterEnabled;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurRadioButton.CheckEventHandler checkEventHandler = new OurRadioButton.CheckEventHandler(this._Lambda___R101_14);
                OurRadioButton ourRadioButton = this._BtnFilterEnabled;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterEnabled = value;
                ourRadioButton = this._BtnFilterEnabled;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurRadioButton BtnFilterDisabled_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFilterDisabled;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurRadioButton.CheckEventHandler checkEventHandler = new OurRadioButton.CheckEventHandler(this._Lambda___R105_15);
                OurRadioButton ourRadioButton = this._BtnFilterDisabled;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterDisabled = value;
                ourRadioButton = this._BtnFilterDisabled;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurRadioButton BtnFilterCanUpdate_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnFilterCanUpdate;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurRadioButton.CheckEventHandler checkEventHandler = new OurRadioButton.CheckEventHandler(this._Lambda___R109_16);
                OurRadioButton ourRadioButton = this._BtnFilterCanUpdate;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterCanUpdate = value;
                ourRadioButton = this._BtnFilterCanUpdate;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check += checkEventHandler;
                }
            }
        }



        internal virtual OurButton BtnHintDownload_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnHintDownload;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageDownload_Click);
                OurButton ourButton = this._BtnHintDownload;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnHintDownload = value;
                ourButton = this._BtnHintDownload;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnHintInstall_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnHintInstall;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageInstall_Click);
                OurButton ourButton = this._BtnHintInstall;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnHintInstall = value;
                ourButton = this._BtnHintInstall;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnHintOpen_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnHintOpen;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnManageOpen_Click);
                OurButton ourButton = this._BtnHintOpen;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnHintOpen = value;
                ourButton = this._BtnHintOpen;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurLoading Load_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._Load;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurLoading.ClickEventHandler clickEventHandler = new OurLoading.ClickEventHandler(this.Load_Click);
                OurLoading.StateChangedEventHandler stateChangedEventHandler = new OurLoading.StateChangedEventHandler(this._Lambda___R137_17);
                OurLoading ourLoading = this._Load;
                if (ourLoading != null)
                {
                    ourLoading.Click -= clickEventHandler;
                    ourLoading.StateChanged -= stateChangedEventHandler;
                }
                this._Load = value;
                ourLoading = this._Load;
                if (ourLoading != null)
                {
                    ourLoading.Click += clickEventHandler;
                    ourLoading.StateChanged += stateChangedEventHandler;
                }
            }
        }




        internal virtual OurIconTextButton BtnSelectUpdate_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSelectUpdate;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconTextButton.ClickEventHandler clickEventHandler = new OurIconTextButton.ClickEventHandler(this._Lambda___R153_18);
                OurIconTextButton ourIconTextButton = this._BtnSelectUpdate;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectUpdate = value;
                ourIconTextButton = this._BtnSelectUpdate;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurIconTextButton BtnSelectEnable_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSelectEnable;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconTextButton.ClickEventHandler clickEventHandler = new OurIconTextButton.ClickEventHandler(this._Lambda___R157_19);
                OurIconTextButton ourIconTextButton = this._BtnSelectEnable;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectEnable = value;
                ourIconTextButton = this._BtnSelectEnable;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurIconTextButton BtnSelectDisable_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSelectDisable;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconTextButton.ClickEventHandler clickEventHandler = new OurIconTextButton.ClickEventHandler(this._Lambda___R161_20);
                OurIconTextButton ourIconTextButton = this._BtnSelectDisable;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectDisable = value;
                ourIconTextButton = this._BtnSelectDisable;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurIconTextButton BtnSelectDelete_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSelectDelete;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconTextButton.ClickEventHandler clickEventHandler = new OurIconTextButton.ClickEventHandler(this._Lambda___R165_21);
                OurIconTextButton ourIconTextButton = this._BtnSelectDelete;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectDelete = value;
                ourIconTextButton = this._BtnSelectDelete;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurIconTextButton BtnSelectCancel_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSelectCancel;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconTextButton.ClickEventHandler clickEventHandler = new OurIconTextButton.ClickEventHandler(this._Lambda___R169_22);
                OurIconTextButton ourIconTextButton = this._BtnSelectCancel;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectCancel = value;
                ourIconTextButton = this._BtnSelectCancel;
                if (ourIconTextButton != null)
                {
                    ourIconTextButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, RoutedEventArgs a1)
        {
            this.PageOther_Loaded();
        }

        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, EventArgs a1)
        {
            this.LoaderInit();
        }

        [CompilerGenerated]
        private void _Lambda___R7_3(ModLoader.LoaderTask<string, List<LocalResourceFile>> a0)
        {
            this.LoadUIFromLoaderOutput();
        }

        [CompilerGenerated]
        private void _Lambda___R13_5(object a0, ModBase.RouteEventArgs a1)
        {
            this.CheckChanged((OurLocalModItem)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R13_7(object a0, EventArgs a1)
        {
            this.Open_Click((OurIconButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R13_8(object a0, EventArgs a1)
        {
            this.Delete_Click((OurIconButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R13_9(object a0, EventArgs a1)
        {
            this.ED_Click((OurIconButton)a0, a1);
        }

        [CompilerGenerated]
        private bool _Lambda___14_0(LocalResourceFile m)
        {
            return this.CanPassFilter(m);
        }

        [CompilerGenerated]
        private void _Lambda___15_0()
        {
            this.CardSelect.Visibility = Visibility.Collapsed;
        }

        [CompilerGenerated]
        private bool _Lambda___35_0(LocalResourceFile m)
        {
            return this.SelectedMods.Contains(m.EnabledName);
        }

        [CompilerGenerated]
        private bool _Lambda___37_0(LocalResourceFile m)
        {
            return this.SelectedMods.Contains(m.EnabledName) && m.HasUpdate;
        }

        [CompilerGenerated]
        private bool _Lambda___40_0(LocalResourceFile m)
        {
            return this.SelectedMods.Contains(m.EnabledName);
        }

        [CompilerGenerated]
        private void _Lambda___R65_12(object a0, EventArgs a1)
        {
            this.SearchRun();
        }

        [CompilerGenerated]
        private void _Lambda___R97_13(object a0, bool a1)
        {
            this.ChangeFilter((OurRadioButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R101_14(object a0, bool a1)
        {
            this.ChangeFilter((OurRadioButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R105_15(object a0, bool a1)
        {
            this.ChangeFilter((OurRadioButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R109_16(object a0, bool a1)
        {
            this.ChangeFilter((OurRadioButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R137_17(object a0, OurLoading.OurLoadingState a1, OurLoading.OurLoadingState a2)
        {
            this.UnselectedAllWithAnimation();
        }

        [CompilerGenerated]
        private void _Lambda___R153_18(object a0, ModBase.RouteEventArgs a1)
        {
            this.BtnSelectUpdate_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R157_19(object a0, ModBase.RouteEventArgs a1)
        {
            this.BtnSelectED_Click((OurIconTextButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R161_20(object a0, ModBase.RouteEventArgs a1)
        {
            this.BtnSelectED_Click((OurIconTextButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R165_21(object a0, ModBase.RouteEventArgs a1)
        {
            this.BtnSelectDelete_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R169_22(object a0, ModBase.RouteEventArgs a1)
        {
            this.BtnSelectCancel_Click();
        }

        private bool IsLoad;

        public ConcurrentDictionary<string, OurLocalModItem> ModItems;

        private int BottomBarShownCount;

        public List<string> SelectedMods;

        private PageInstanceMod.FilterType _Filter;

        public static List<string> UpdatingInstanceModFolders = new List<string>();

        private List<LocalResourceFile> SearchResult;

        [CompilerGenerated]
        [AccessedThroughProperty("PanAllBack")]
        private Grid _PanAllBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("SearchBox")]
        private OurSearchBox _SearchBox;

        [CompilerGenerated]
        [AccessedThroughProperty("PanManage")]
        private OurCard _PanManage;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnManageOpen")]
        private OurButton _BtnManageOpen;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnManageInstall")]
        private OurButton _BtnManageInstall;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnManageDownload")]
        private OurButton _BtnManageDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnManageSelectAll")]
        private OurButton _BtnManageSelectAll;

        [CompilerGenerated]
        [AccessedThroughProperty("PanListBack")]
        private OurCard _PanListBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanFilter")]
        private StackPanel _PanFilter;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFilterAll")]
        private OurRadioButton _BtnFilterAll;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFilterEnabled")]
        private OurRadioButton _BtnFilterEnabled;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFilterDisabled")]
        private OurRadioButton _BtnFilterDisabled;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnFilterCanUpdate")]
        private OurRadioButton _BtnFilterCanUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("PanList")]
        private StackPanel _PanList;

        [CompilerGenerated]
        [AccessedThroughProperty("PanEmpty")]
        private OurCard _PanEmpty;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnHintDownload")]
        private OurButton _BtnHintDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnHintInstall")]
        private OurButton _BtnHintInstall;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnHintOpen")]
        private OurButton _BtnHintOpen;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLoad")]
        private OurCard _PanLoad;

        [CompilerGenerated]
        [AccessedThroughProperty("Load")]
        private OurLoading _Load;

        [CompilerGenerated]
        [AccessedThroughProperty("CardSelect")]
        private OurCard _CardSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("TransSelect")]
        private TranslateTransform _TransSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("LabSelect")]
        private TextBlock _LabSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSelectUpdate")]
        private OurIconTextButton _BtnSelectUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSelectEnable")]
        private OurIconTextButton _BtnSelectEnable;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSelectDisable")]
        private OurIconTextButton _BtnSelectDisable;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSelectDelete")]
        private OurIconTextButton _BtnSelectDelete;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSelectCancel")]
        private OurIconTextButton _BtnSelectCancel;

        public enum FilterType
        {
            All,
            Enabled,
            Disabled,
            CanUpdate
        }

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal void _Lambda___R13_6(object a0, MouseButtonEventArgs a1)
            {
                ((PageInstanceMod._Closure___._I13_0 == null) ? (PageInstanceMod._Closure___._I13_0 = new VB_AnonymousDelegate_16<OurLocalModItem, EventArgs>(PageInstanceMod._Closure___._I._Lambda___13_0)) : PageInstanceMod._Closure___._I13_0)((OurLocalModItem)a0, a1);
            }

            internal void _Lambda___13_0(OurLocalModItem ss, EventArgs ee)
            {
                ss.Checked = !ss.Checked;
            }

            internal string _Lambda___39_3(LocalResourceFile m)
            {
                return m.File.Name;
            }

            internal IEnumerable<string> _Lambda___41_0(LocalResourceFile Target)
            {
                IEnumerable<string> enumerable;
                if (Target.IsEnabled)
                {
                    enumerable = new string[]
                    {
                        Target.File.FullName,
                        Target.File.FullName + (FileUtils.Exists(Target.File.FullName + ".old") ? ".old" : ".disabled")
                    };
                }
                else
                {
                    enumerable = new string[]
                    {
                        Target.File.FullName,
                        Target.EnabledFullName
                    };
                }
                return enumerable;
            }

            internal bool _Lambda___41_1(string m)
            {
                return FileUtils.Exists(m);
            }

            internal LocalResourceFile _Lambda___41_2(string m)
            {
                return new LocalResourceFile(m);
            }

            internal LocalResourceFile _Lambda___50_0(ModBase.SearchEntry<LocalResourceFile> r)
            {
                return r.Item;
            }

            public static readonly PageInstanceMod._Closure___ _I = new PageInstanceMod._Closure___();

            public static VB_AnonymousDelegate_16<OurLocalModItem, EventArgs> _I13_0;

            public static OurLocalModItem.ClickEventHandler _IR13_6;

            public static Func<LocalResourceFile, string> _I39_3;

            public static Func<LocalResourceFile, IEnumerable<string>> _I41_0;

            public static Func<string, bool> _I41_1;

            public static Func<string, LocalResourceFile> _I41_2;

            public static Func<ModBase.SearchEntry<LocalResourceFile>, LocalResourceFile> _I50_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___12_0
        {
            internal void _Lambda___R4(LocalResourceFile a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                ModBase.RunInUi(new Action(this._VB_Local_NewItem.Refresh), false);
            }

            public OurLocalModItem _VB_Local_NewItem;
        }

        [CompilerGenerated]
        private sealed class _Closure___14_0
        {
            public _Closure___14_0(PageInstanceMod._Closure___14_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_ShowingMods = arg0._VB_Local_ShowingMods;
                }
            }

            internal bool _Lambda___1(string m)
            {
                PageInstanceMod._Closure___14_1 CS_locals1 = new PageInstanceMod._Closure___14_1(null);
                CS_locals1._VB_Local_m = m;
                return this._VB_Local_ShowingMods.Any<LocalResourceFile>(new Func<LocalResourceFile, bool>(CS_locals1._Lambda___2));
            }

            public List<LocalResourceFile> _VB_Local_ShowingMods;
        }

        [CompilerGenerated]
        private sealed class _Closure___14_1
        {
            public _Closure___14_1(PageInstanceMod._Closure___14_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_m = arg0._VB_Local_m;
                }
            }

            internal bool _Lambda___2(LocalResourceFile s)
            {
                return Operators.CompareString(s.EnabledName, this._VB_Local_m, false) == 0;
            }

            public string _VB_Local_m;
        }

        [CompilerGenerated]
        private sealed class _Closure___20_0
        {
            public _Closure___20_0(PageInstanceMod._Closure___20_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Extension = arg0._VB_Local_Extension;
                }
            }

            internal bool _Lambda___0(string t)
            {
                return Operators.CompareString(t, this._VB_Local_Extension, false) == 0;
            }

            public string _VB_Local_Extension;
        }

        [CompilerGenerated]
        private sealed class _Closure___36_0
        {
            public _Closure___36_0(PageInstanceMod._Closure___36_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_ModEntity = arg0._VB_Local_ModEntity;
                }
            }

            internal bool _Lambda___0(OurLocalModItem i)
            {
                return i.Entry == this._VB_Local_ModEntity;
            }

            public LocalResourceFile _VB_Local_ModEntity;
        }

        [CompilerGenerated]
        private sealed class _Closure___39_0
        {
            public _Closure___39_0(PageInstanceMod._Closure___39_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_ModList = arg0._VB_Local_ModList;
                }
            }

            public IEnumerable<LocalResourceFile> _VB_Local_ModList;
        }

        [CompilerGenerated]
        private sealed class _Closure___39_1
        {
            public _Closure___39_1(PageInstanceMod._Closure___39_1 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_FileCopyList = arg0._VB_Local_FileCopyList;
                    this._VB_Local_FinishedFileNames = arg0._VB_Local_FinishedFileNames;
                    this._VB_Local_Loader = arg0._VB_Local_Loader;
                    this._VB_Local_PathMods = arg0._VB_Local_PathMods;
                }
            }

            internal void _Lambda___R10(ModLoader.LoaderTask<int, int> a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                try
                {
                    try
                    {
                        foreach (LocalResourceFile localResourceFile in this._VB_NonLocal__VB_Closure_2._VB_Local_ModList)
                        {
                            if (FileUtils.Exists(localResourceFile.File.FullName))
                            {
                                FileUtils.Delete(localResourceFile.File.FullName, true);
                            }
                            else
                            {
                                Logger.Warn(string.Format("未找到更新前的 Mod 文件，跳过对它的删除：{0}", localResourceFile.File.FullName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<LocalResourceFile> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    try
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in this._VB_Local_FileCopyList)
                        {
                            if (DirectoryUtils.Exists(PathUtils.RemoveLastPart(keyValuePair.Value)))
                            {
                                FileUtils.Move(keyValuePair.Key, keyValuePair.Value, false);
                                this._VB_Local_FinishedFileNames.Add(PathUtils.GetLastPart(keyValuePair.Value));
                            }
                            else
                            {
                                Logger.Warn(string.Format("更新后的目标文件夹已被删除：{0}", keyValuePair.Value), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                            }
                        }
                    }
                    finally
                    {
                        Dictionary<string, string>.Enumerator enumerator2 = default;
                        ((IDisposable)enumerator2).Dispose();
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Logger.Warn(ex, "替换旧版 Mod 文件时被主动取消", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                }
            }

            internal void _Lambda___R11(ModLoader.LoaderBase a0)
            {
                this._Lambda___1();
            }

            internal void _Lambda___1()
            {
                switch (this._VB_Local_Loader.State)
                {
                case ModBase.LoadState.Finished:
                {
                    int count = this._VB_Local_FinishedFileNames.Count;
                    if (count != 0)
                    {
                        if (count != 1)
                        {
                            ModMain.Hint(string.Format("已成功更新 {0} 个 Mod！", this._VB_Local_FinishedFileNames.Count), ModMain.HintType.Green, true);
                        }
                        else
                        {
                            ModMain.Hint(string.Format("已成功更新 {0}！", this._VB_Local_FinishedFileNames.Single<string>()), ModMain.HintType.Green, true);
                        }
                    }
                    else
                    {
                        Logger.Info("没有 Mod 被成功更新", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                    }
                    break;
                }
                case ModBase.LoadState.Failed:
                    ModMain.Hint("Mod 更新失败：" + this._VB_Local_Loader.Error.GetDisplay(false), ModMain.HintType.Red, true);
                    break;
                case ModBase.LoadState.Canceled:
                    ModMain.Hint("Mod 更新已中止！", ModMain.HintType.Blue, true);
                    break;
                default:
                    return;
                }
                Logger.Info(string.Format("已从正在进行 Mod 更新的文件夹列表移除：{0}", this._VB_Local_PathMods), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                PageInstanceMod.UpdatingInstanceModFolders.Remove(this._VB_Local_PathMods);
                ModBase.RunInNewThread((this._I2 == null) ? (this._I2 = new Action(this._Lambda___2)) : this._I2, "Clean Mod Update Cache", ThreadPriority.BelowNormal);
            }

            internal void _Lambda___2()
            {
                try
                {
                    try
                    {
                        foreach (string text in this._VB_Local_FileCopyList.Keys)
                        {
                            FileUtils.Delete(text, false);
                        }
                    }
                    finally
                    {
                        Dictionary<string, string>.KeyCollection.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "清理 Mod 更新缓存失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                }
            }

            public Dictionary<string, string> _VB_Local_FileCopyList;

            public List<string> _VB_Local_FinishedFileNames;

            public ModLoader.LoaderCombo<IEnumerable<LocalResourceFile>> _VB_Local_Loader;

            public string _VB_Local_PathMods;

            public PageInstanceMod._Closure___39_0 _VB_NonLocal__VB_Closure_2;

            public Action _I2;
        }

        [CompilerGenerated]
        private sealed class _Closure___41_0
        {
            public _Closure___41_0(PageInstanceMod._Closure___41_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_ModEntity = arg0._VB_Local_ModEntity;
                }
            }

            internal bool _Lambda___3(OurLocalModItem i)
            {
                return i.Entry.Equals(this._VB_Local_ModEntity);
            }

            public LocalResourceFile _VB_Local_ModEntity;
        }
    }
}
