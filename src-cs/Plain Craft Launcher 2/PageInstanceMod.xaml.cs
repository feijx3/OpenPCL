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
using PCL.My;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceMod : MyPageRight, IRefreshable
    {
        public PageInstanceMod()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.PageOther_Loaded();
            };
            base.Initialized += delegate(object a0, EventArgs a1)
            {
                this.LoaderInit();
            };
            base.PageExit += this.UnselectedAllWithAnimation;
            this.IsLoad = false;
            this.ModItems = new ConcurrentDictionary<string, MyLocalModItem>();
            this.BottomBarShownCount = 0;
            this.SelectedMods = new List<string>();
            this._Filter = PageInstanceMod.FilterType.All;
            this.InitializeComponent();
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
                            ((MyRadioButton)obj).LabText.Margin = new Thickness(-2.0, 0.0, 8.0, 0.0);
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

        void IRefreshable.RefreshSelf()
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
            base.PageLoaderInit<string, List<LocalResourceFile>>(this.Load, this.PanLoad, this.PanAllBack, null, LocalResourceLoaders.LocalResourceLoader, delegate(ModLoader.LoaderTask<string, List<LocalResourceFile>> a0)
            {
                this.LoadUIFromLoaderOutput();
            }, null, false);
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
                        List<LocalResourceFile>.Enumerator enumerator;
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

        private MyLocalModItem McModListItem(LocalResourceFile Entry)
        {
            checked
            {
                ModAnimation.AniControlEnabled++;
                MyLocalModItem NewItem = new MyLocalModItem
                {
                    SnapsToDevicePixels = true,
                    Entry = Entry,
                    ButtonHandler = new Action<MyLocalModItem, EventArgs>(this.McModContent),
                    Checked = this.SelectedMods.Contains(Entry.EnabledName)
                };
                Entry.OnUpdate += delegate(LocalResourceFile a0)
                {
                    base._Lambda$__0();
                };
                ModAnimation.AniControlEnabled--;
                return NewItem;
            }
        }

        private void McModContent(MyLocalModItem sender, EventArgs e)
        {
            sender.Changed += delegate(object a0, ModBase.RouteEventArgs a1)
            {
                this.CheckChanged((MyLocalModItem)a0, a1);
            };
            sender.Click += ((PageInstanceMod._Closure$__.$IR13-6 == null) ? (PageInstanceMod._Closure$__.$IR13-6 = delegate(object a0, MouseButtonEventArgs a1)
            {
                ((PageInstanceMod._Closure$__.$I13-0 == null) ? (PageInstanceMod._Closure$__.$I13-0 = delegate(MyLocalModItem ss, EventArgs ee)
                {
                    ss.Checked = !ss.Checked;
                }) : PageInstanceMod._Closure$__.$I13-0)((MyLocalModItem)a0, a1);
            }) : PageInstanceMod._Closure$__.$IR13-6);
            MyIconButton myIconButton = new MyIconButton
            {
                LogoScale = 1.05,
                Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z",
                Tag = sender
            };
            myIconButton.ToolTip = "打开文件位置";
            myIconButton.Click += delegate(object a0, EventArgs a1)
            {
                this.Open_Click((MyIconButton)a0, a1);
            };
            MyIconButton myIconButton2 = new MyIconButton
            {
                LogoScale = 1.0,
                Logo = "M512 917.333333c223.861333 0 405.333333-181.472 405.333333-405.333333S735.861333 106.666667 512 106.666667 106.666667 288.138667 106.666667 512s181.472 405.333333 405.333333 405.333333z m0 106.666667C229.226667 1024 0 794.773333 0 512S229.226667 0 512 0s512 229.226667 512 512-229.226667 512-512 512z m-32-597.333333h64a21.333333 21.333333 0 0 1 21.333333 21.333333v320a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333V448a21.333333 21.333333 0 0 1 21.333333-21.333333z m0-192h64a21.333333 21.333333 0 0 1 21.333333 21.333333v64a21.333333 21.333333 0 0 1-21.333333 21.333333h-64a21.333333 21.333333 0 0 1-21.333333-21.333333v-64a21.333333 21.333333 0 0 1 21.333333-21.333333z",
                Tag = sender
            };
            myIconButton2.ToolTip = "详情";
            myIconButton2.Click += this.Info_Click;
            sender.MouseRightButtonUp += new MouseButtonEventHandler(this.Info_Click);
            MyIconButton myIconButton3 = new MyIconButton
            {
                LogoScale = 1.0,
                Logo = "M520.192 0C408.43 0 317.44 82.87 313.563 186.734H52.736c-29.038 0-52.663 21.943-52.663 49.079s23.625 49.152 52.663 49.152h58.075v550.473c0 103.35 75.118 187.757 167.717 187.757h472.43c92.599 0 167.716-83.894 167.716-187.757V285.477h52.59c29.038 0 52.59-21.943 52.663-49.08-0.073-27.135-23.625-49.151-52.663-49.151H726.235C723.237 83.017 631.955 0 520.192 0zM404.846 177.957c3.803-50.03 50.176-89.015 107.447-89.015 57.197 0 103.57 38.985 106.788 89.015H404.92zM284.379 933.669c-33.353 0-69.997-39.351-69.997-95.525v-549.01H833.39v549.522c0 56.247-36.645 95.525-69.998 95.525H284.379v-0.512z M357.23 800.695a48.274 48.274 0 0 0 47.616-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.69 49.006V751.69c0 27.282 20.846 49.006 47.617 49.006z m166.62 0a48.274 48.274 0 0 0 47.688-49.006V471.7a48.274 48.274 0 0 0-47.689-49.08 48.274 48.274 0 0 0-47.543 49.006V751.69c0 27.282 21.431 49.006 47.543 49.006z m142.92 0a48.274 48.274 0 0 0 47.543-49.006V471.7a48.274 48.274 0 0 0-47.543-49.08 48.274 48.274 0 0 0-47.616 49.006V751.69c0 27.282 20.773 49.006 47.543 49.006z",
                Tag = sender
            };
            myIconButton3.ToolTip = "删除";
            myIconButton3.Click += delegate(object a0, EventArgs a1)
            {
                this.Delete_Click((MyIconButton)a0, a1);
            };
            MyIconButton myIconButton4 = new MyIconButton
            {
                LogoScale = 1.0,
                Logo = (sender.Entry.IsEnabled ? "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z" : "M512 0a512 512 0 1 0 512 512A512 512 0 0 0 512 0z m0 921.6a409.6 409.6 0 1 1 409.6-409.6 409.6 409.6 0 0 1-409.6 409.6z M716.8 339.968l-256 253.44L328.192 460.8A51.2 51.2 0 0 0 256 532.992l168.448 168.96a51.2 51.2 0 0 0 72.704 0l289.28-289.792A51.2 51.2 0 0 0 716.8 339.968z"),
                Tag = sender
            };
            myIconButton4.ToolTip = (sender.Entry.IsEnabled ? "禁用" : "启用");
            myIconButton4.Click += delegate(object a0, EventArgs a1)
            {
                this.ED_Click((MyIconButton)a0, a1);
            };
            sender.Buttons = new MyIconButton[] { myIconButton2, myIconButton, myIconButton4, myIconButton3 };
        }

        public void RefreshUI()
        {
            PageInstanceMod._Closure$__14-0 CS$<>8__locals1 = new PageInstanceMod._Closure$__14-0(CS$<>8__locals1);
            checked
            {
                if (this.PanList != null)
                {
                    CS$<>8__locals1.$VB$Local_ShowingMods = (this.IsSearching ? this.SearchResult : (LocalResourceLoaders.LocalResourceLoader.Output ?? new List<LocalResourceFile>())).Where<LocalResourceFile>((LocalResourceFile m) => this.CanPassFilter(m)).ToList<LocalResourceFile>();
                    ModAnimation.AniControlEnabled++;
                    if (CS$<>8__locals1.$VB$Local_ShowingMods.Any<LocalResourceFile>())
                    {
                        this.PanList.Visibility = Visibility.Visible;
                        this.PanList.Children.Clear();
                        try
                        {
                            foreach (LocalResourceFile localResourceFile in CS$<>8__locals1.$VB$Local_ShowingMods)
                            {
                                MyLocalModItem myLocalModItem = null;
                                if (this.ModItems.TryGetValue(localResourceFile.EnabledName, out myLocalModItem))
                                {
                                    myLocalModItem.Checked = this.SelectedMods.Contains(localResourceFile.EnabledName);
                                    this.PanList.Children.Add(myLocalModItem);
                                }
                            }
                            goto IL_0104;
                        }
                        finally
                        {
                            List<LocalResourceFile>.Enumerator enumerator;
                            ((IDisposable)enumerator).Dispose();
                        }
                    }
                    this.PanList.Visibility = Visibility.Collapsed;
                    IL_0104:
                    ModAnimation.AniControlEnabled--;
                    this.SelectedMods = this.SelectedMods.Where<string>(delegate(string m)
                    {
                        PageInstanceMod._Closure$__14-1 CS$<>8__locals2 = new PageInstanceMod._Closure$__14-1(CS$<>8__locals2);
                        CS$<>8__locals2.$VB$Local_m = m;
                        return CS$<>8__locals1.$VB$Local_ShowingMods.Any<LocalResourceFile>((LocalResourceFile s) => Operators.CompareString(s.EnabledName, CS$<>8__locals2.$VB$Local_m, false) == 0);
                    }).ToList<string>();
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
                    List<LocalResourceFile>.Enumerator enumerator;
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
                        List<LocalResourceFile>.Enumerator enumerator2;
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
                        ModAnimation.AaCode(delegate
                        {
                            this.CardSelect.Visibility = Visibility.Collapsed;
                        }, 0, true)
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
            PageInstanceMod._Closure$__20-0 CS$<>8__locals1 = new PageInstanceMod._Closure$__20-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Extension = PathUtils.GetExtension(FilePathList.First<string>());
            bool flag;
            if (!new string[] { "jar", "litemod", "disabled", "old" }.Any<string>((string t) => Operators.CompareString(t, CS$<>8__locals1.$VB$Local_Extension, false) == 0))
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
                    if (ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSelect || mcInstance == null || !mcInstance.Modable)
                    {
                        ModMain.Hint("若要安装 Mod，请先选择一个可以安装 Mod 的版本！", ModMain.HintType.Blue, true);
                    }
                    else if ((ModMain.FrmMain.PageCurrent == FormMain.PageType.InstanceSetup && ModMain.FrmMain.PageCurrentSub == FormMain.PageSubType.DownloadPack) || ModMain.MyMsgBox(string.Format("是否要将这{0}文件作为 Mod 安装到 {1}？", FilePathList.IsSingle<string>() ? "个" : "些", mcInstance.Name), "Mod 安装确认", "确定", "取消", "", false, true, false, null, null, null) == 1)
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
                                IEnumerator<string> enumerator;
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

        public void CheckChanged(MyLocalModItem sender, ModBase.RouteEventArgs e)
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
                    foreach (MyLocalModItem myLocalModItem in this.ModItems.Values)
                    {
                        bool flag = Value && this.PanList.Children.Contains(myLocalModItem);
                        myLocalModItem.Checked = flag;
                        if (flag)
                        {
                            this.SelectedMods.Add(myLocalModItem.Entry.EnabledName);
                        }
                    }
                }
                finally
                {
                    IEnumerator<MyLocalModItem> enumerator;
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
            if (ModMain.FrmMain.PageRight == this && MyWpfExtension.Computer.Keyboard.CtrlKeyDown && e.Key == Key.A)
            {
                this.ChangeAllSelected(true);
            }
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.SearchBox.Text.Any<char>() && MyWpfExtension.Computer.Keyboard.CtrlKeyDown && e.Key == Key.A)
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

        private void ChangeFilter(MyRadioButton sender, bool raiseByMouse)
        {
            this.Filter = (PageInstanceMod.FilterType)Conversions.ToInteger(sender.Tag);
            this.RefreshUI();
        }

        private void BtnSelectED_Click(MyIconTextButton sender, ModBase.RouteEventArgs e)
        {
            this.EDMods(LocalResourceLoaders.LocalResourceLoader.Output.Where<LocalResourceFile>((LocalResourceFile m) => this.SelectedMods.Contains(m.EnabledName)), !sender.Equals(this.BtnSelectDisable));
            this.ChangeAllSelected(false);
        }

        private void EDMods(IEnumerable<LocalResourceFile> ModList, bool IsEnable)
        {
            bool flag = true;
            try
            {
                foreach (LocalResourceFile localResourceFile in ModList.ToList<LocalResourceFile>())
                {
                    PageInstanceMod._Closure$__36-0 CS$<>8__locals1 = new PageInstanceMod._Closure$__36-0(CS$<>8__locals1);
                    CS$<>8__locals1.$VB$Local_ModEntity = localResourceFile;
                    string text = null;
                    if (CS$<>8__locals1.$VB$Local_ModEntity.IsEnabled && !IsEnable)
                    {
                        text = PathUtils.RemoveExtendedPrefix(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName) + (FileUtils.Exists(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName + ".old") ? ".old" : ".disabled");
                    }
                    else
                    {
                        if (CS$<>8__locals1.$VB$Local_ModEntity.IsEnabled || !IsEnable)
                        {
                            continue;
                        }
                        text = CS$<>8__locals1.$VB$Local_ModEntity.EnabledFullName;
                    }
                    try
                    {
                        if (FileUtils.Exists(text))
                        {
                            if (!FileUtils.Exists(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName))
                            {
                                Logger.Warn("Mod 的状态已被切换", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                                continue;
                            }
                            if (Operators.CompareString(CryptographyUtils.ComputeFileHash(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, CryptographyUtils.HashMethod.Md5), CryptographyUtils.ComputeFileHash(text, CryptographyUtils.HashMethod.Md5), false) != 0)
                            {
                                ModMain.MyMsgBox(string.Format("目前同时存在启用和禁用的两个 Mod 文件：{0} - {1}{2} - {3}{4}{5}注意，这两个文件的内容并不相同。{6}在手动删除或重命名其中一个文件后，才能继续操作。", new object[]
                                {
                                    "\r\n",
                                    PathUtils.GetLastPart(text),
                                    "\r\n",
                                    CS$<>8__locals1.$VB$Local_ModEntity.File.Name,
                                    "\r\n",
                                    "\r\n",
                                    "\r\n"
                                }), "存在文件冲突", "确定", "", "", false, true, false, null, null, null);
                                continue;
                            }
                        }
                        FileUtils.Move(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, text, false);
                    }
                    catch (FileNotFoundException ex)
                    {
                        Logger.Error(ex, string.Format("未找到需要重命名的 Mod（{0}）", CS$<>8__locals1.$VB$Local_ModEntity.File.FullName ?? "null"), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                        this.ReloadModList(true);
                        return;
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, string.Format("重命名 Mod 失败（{0}）", CS$<>8__locals1.$VB$Local_ModEntity.File.FullName ?? "null"), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                        flag = false;
                    }
                    LocalResourceFile localResourceFile2 = new LocalResourceFile(text);
                    localResourceFile2.FromJson(CS$<>8__locals1.$VB$Local_ModEntity.ToJson());
                    if (LocalResourceLoaders.LocalResourceLoader.Output.Contains(CS$<>8__locals1.$VB$Local_ModEntity))
                    {
                        int num = LocalResourceLoaders.LocalResourceLoader.Output.IndexOf(CS$<>8__locals1.$VB$Local_ModEntity);
                        LocalResourceLoaders.LocalResourceLoader.Output.RemoveAt(num);
                        LocalResourceLoaders.LocalResourceLoader.Output.Insert(num, localResourceFile2);
                    }
                    if (this.SearchResult != null && this.SearchResult.Contains(CS$<>8__locals1.$VB$Local_ModEntity))
                    {
                        int num2 = this.SearchResult.IndexOf(CS$<>8__locals1.$VB$Local_ModEntity);
                        this.SearchResult.Remove(CS$<>8__locals1.$VB$Local_ModEntity);
                        this.SearchResult.Insert(num2, localResourceFile2);
                    }
                    MyLocalModItem myLocalModItem = this.McModListItem(localResourceFile2);
                    this.ModItems[CS$<>8__locals1.$VB$Local_ModEntity.EnabledName] = myLocalModItem;
                    int num3 = this.PanList.Children.IndexOf(this.PanList.Children.OfType<MyLocalModItem>().FirstOrDefault<MyLocalModItem>((MyLocalModItem i) => i.Entry == CS$<>8__locals1.$VB$Local_ModEntity));
                    if (num3 != -1)
                    {
                        this.PanList.Children.RemoveAt(num3);
                        this.PanList.Children.Insert(num3, myLocalModItem);
                        myLocalModItem.UpdateLayout();
                    }
                }
            }
            finally
            {
                List<LocalResourceFile>.Enumerator enumerator;
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
            List<LocalResourceFile> list = LocalResourceLoaders.LocalResourceLoader.Output.Where<LocalResourceFile>((LocalResourceFile m) => this.SelectedMods.Contains(m.EnabledName) && m.HasUpdate).ToList<LocalResourceFile>();
            if (list.Any<LocalResourceFile>())
            {
                this.UpdateMods(list);
                this.ChangeAllSelected(false);
            }
        }

        public void UpdateMods(IEnumerable<LocalResourceFile> ModList)
        {
            PageInstanceMod._Closure$__39-0 CS$<>8__locals1 = new PageInstanceMod._Closure$__39-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_ModList = ModList;
            if (!Settings.Get<bool>("HintUpdateMod", null) || CS$<>8__locals1.$VB$Local_ModList.Count<LocalResourceFile>() >= 15)
            {
                if (ModMain.MyMsgBox(string.Format("新版本 Mod 可能不兼容旧存档或者其他 Mod，这可能导致游戏崩溃，甚至永久损坏存档！{0}如果你在游玩整合包，请千万不要自行更新 Mod！{1}{2}在更新前，请先备份存档，并检查 Mod 的更新日志。{3}如果更新后出现问题，你也可以在回收站找回更新前的 Mod。", new object[] { "\r\n", "\r\n", "\r\n", "\r\n" }), "Mod 更新警告", "我已了解风险，继续更新", "取消", "", true, true, false, null, null, null) != 1)
                {
                    return;
                }
                Settings.Set("HintUpdateMod", true, null);
            }
            try
            {
                PageInstanceMod._Closure$__39-1 CS$<>8__locals2 = new PageInstanceMod._Closure$__39-1(CS$<>8__locals2);
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2 = CS$<>8__locals1;
                CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList = CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList.ToList<LocalResourceFile>();
                List<ModNet.NetFile> list = new List<ModNet.NetFile>();
                CS$<>8__locals2.$VB$Local_FileCopyList = new Dictionary<string, string>();
                List<ModLoader.LoaderBase> list4;
                checked
                {
                    try
                    {
                        foreach (LocalResourceFile localResourceFile in CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList)
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
                                CS$<>8__locals2.$VB$Local_FileCopyList[text3] = text4;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<LocalResourceFile> enumerator;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    list4 = new List<ModLoader.LoaderBase>();
                    CS$<>8__locals2.$VB$Local_FinishedFileNames = new List<string>();
                }
                list4.Add(new ModNet.LoaderDownload("下载新版 Mod 文件", list)
                {
                    ProgressWeight = (double)CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList.Count<LocalResourceFile>() * 1.5
                });
                list4.Add(new ModLoader.LoaderTask<int, int>("替换旧版 Mod 文件", delegate(ModLoader.LoaderTask<int, int> a0)
                {
                    base._Lambda$__0();
                }, null, ThreadPriority.Normal));
                CS$<>8__locals2.$VB$Local_Loader = new ModLoader.LoaderCombo<IEnumerable<LocalResourceFile>>("Mod 更新：" + PageInstanceLeft.Instance.Name, list4);
                CS$<>8__locals2.$VB$Local_PathMods = PageInstanceLeft.Instance.PathIndie + "mods\\";
                CS$<>8__locals2.$VB$Local_Loader.OnStateChanged = delegate(ModLoader.LoaderBase a0)
                {
                    base._Lambda$__1();
                };
                Logger.Info(string.Format("开始更新 {0} 个 Mod：{1}", CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList.Count<LocalResourceFile>(), CS$<>8__locals2.$VB$NonLocal_$VB$Closure_2.$VB$Local_ModList.Select<LocalResourceFile, string>((PageInstanceMod._Closure$__.$I39-3 == null) ? (PageInstanceMod._Closure$__.$I39-3 = (LocalResourceFile m) => m.File.Name) : PageInstanceMod._Closure$__.$I39-3).Join("，")), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                PageInstanceMod.UpdatingInstanceModFolders.Add(CS$<>8__locals2.$VB$Local_PathMods);
                CS$<>8__locals2.$VB$Local_Loader.Start(null, false);
                ModLoader.LoaderTaskbarAdd<IEnumerable<LocalResourceFile>>(CS$<>8__locals2.$VB$Local_Loader);
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
            this.DeleteMods(LocalResourceLoaders.LocalResourceLoader.Output.Where<LocalResourceFile>((LocalResourceFile m) => this.SelectedMods.Contains(m.EnabledName)));
            this.ChangeAllSelected(false);
        }

        private void DeleteMods(IEnumerable<LocalResourceFile> ModsToDelete)
        {
            try
            {
                bool flag = true;
                bool shiftKeyDown = MyWpfExtension.Computer.Keyboard.ShiftKeyDown;
                List<LocalResourceFile> list = ModsToDelete.SelectMany<LocalResourceFile, string>((PageInstanceMod._Closure$__.$I41-0 == null) ? (PageInstanceMod._Closure$__.$I41-0 = delegate(LocalResourceFile Target)
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
                }) : PageInstanceMod._Closure$__.$I41-0).Distinct<string>().Where<string>((PageInstanceMod._Closure$__.$I41-1 == null) ? (PageInstanceMod._Closure$__.$I41-1 = (string m) => FileUtils.Exists(m)) : PageInstanceMod._Closure$__.$I41-1)
                    .Select<string, LocalResourceFile>((PageInstanceMod._Closure$__.$I41-2 == null) ? (PageInstanceMod._Closure$__.$I41-2 = (string m) => new LocalResourceFile(m)) : PageInstanceMod._Closure$__.$I41-2)
                    .ToList<LocalResourceFile>();
                try
                {
                    List<LocalResourceFile>.Enumerator enumerator = list.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        PageInstanceMod._Closure$__41-0 CS$<>8__locals1 = new PageInstanceMod._Closure$__41-0(CS$<>8__locals1);
                        CS$<>8__locals1.$VB$Local_ModEntity = enumerator.Current;
                        try
                        {
                            FileUtils.Delete(CS$<>8__locals1.$VB$Local_ModEntity.File.FullName, !shiftKeyDown);
                        }
                        catch (OperationCanceledException ex)
                        {
                            Logger.Warn(ex, "删除 Mod 被主动取消", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                            this.ReloadModList(true);
                            return;
                        }
                        catch (Exception ex2)
                        {
                            Logger.Error(ex2, string.Format("删除 Mod 失败（{0}）", CS$<>8__locals1.$VB$Local_ModEntity.File.FullName), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
                            flag = false;
                        }
                        this.SelectedMods.Remove(CS$<>8__locals1.$VB$Local_ModEntity.EnabledName);
                        LocalResourceLoaders.LocalResourceLoader.Output.Remove(CS$<>8__locals1.$VB$Local_ModEntity);
                        List<LocalResourceFile> searchResult = this.SearchResult;
                        if (searchResult != null)
                        {
                            searchResult.Remove(CS$<>8__locals1.$VB$Local_ModEntity);
                        }
                        this.ModItems.Remove(CS$<>8__locals1.$VB$Local_ModEntity.EnabledName);
                        int num = this.PanList.Children.IndexOf(this.PanList.Children.OfType<MyLocalModItem>().FirstOrDefault<MyLocalModItem>((MyLocalModItem i) => i.Entry.Equals(CS$<>8__locals1.$VB$Local_ModEntity)));
                        if (num >= 0)
                        {
                            this.PanList.Children.RemoveAt(num);
                        }
                    }
                }
                finally
                {
                    List<LocalResourceFile>.Enumerator enumerator;
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
                    LocalResourceFile entry = ((MyLocalModItem)((sender is MyIconButton) ? NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null) : sender)).Entry;
                    if (entry.Project != null)
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
                        if (ModMain.MyMsgBox(list.Join("\r\n"), entry.Display, "百科搜索", "返回", "", false, true, false, null, null, null) == 1)
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

        public void Open_Click(MyIconButton sender, EventArgs e)
        {
            try
            {
                ModBase.OpenExplorer(((MyLocalModItem)sender.Tag).Entry.File.FullName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "打开 Mod 文件位置失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceMod.xaml.vb");
            }
        }

        public void Delete_Click(MyIconButton sender, EventArgs e)
        {
            MyLocalModItem myLocalModItem = (MyLocalModItem)sender.Tag;
            this.DeleteMods(new LocalResourceFile[] { myLocalModItem.Entry });
        }

        public void ED_Click(MyIconButton sender, EventArgs e)
        {
            MyLocalModItem myLocalModItem = (MyLocalModItem)sender.Tag;
            this.EDMods(new LocalResourceFile[] { myLocalModItem.Entry }, !myLocalModItem.Entry.IsEnabled);
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
                        if (localResourceFile.Project != null)
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
                    List<LocalResourceFile>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                this.SearchResult = ModBase.Search<LocalResourceFile>(list, this.SearchBox.Text, 6, 0.35).Select<ModBase.SearchEntry<LocalResourceFile>, LocalResourceFile>((PageInstanceMod._Closure$__.$I50-0 == null) ? (PageInstanceMod._Closure$__.$I50-0 = (ModBase.SearchEntry<LocalResourceFile> r) => r.Item) : PageInstanceMod._Closure$__.$I50-0).ToList<LocalResourceFile>();
            }
            this.RefreshUI();
        }

        internal virtual Grid PanAllBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
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

        internal virtual MySearchBox SearchBox
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
                MySearchBox.TextChangedEventHandler textChangedEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.SearchRun();
                };
                MySearchBox mySearchBox = this._SearchBox;
                if (mySearchBox != null)
                {
                    mySearchBox.PreviewKeyDown -= keyEventHandler;
                    mySearchBox.TextChanged -= textChangedEventHandler;
                }
                this._SearchBox = value;
                mySearchBox = this._SearchBox;
                if (mySearchBox != null)
                {
                    mySearchBox.PreviewKeyDown += keyEventHandler;
                    mySearchBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual MyCard PanManage
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnManageOpen
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageOpen_Click);
                MyButton myButton = this._BtnManageOpen;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnManageOpen = value;
                myButton = this._BtnManageOpen;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnManageInstall
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageInstall_Click);
                MyButton myButton = this._BtnManageInstall;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnManageInstall = value;
                myButton = this._BtnManageInstall;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnManageDownload
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageDownload_Click);
                MyButton myButton = this._BtnManageDownload;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnManageDownload = value;
                myButton = this._BtnManageDownload;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnManageSelectAll
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageSelectAll_Click);
                MyButton myButton = this._BtnManageSelectAll;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnManageSelectAll = value;
                myButton = this._BtnManageSelectAll;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard PanListBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanFilter
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioButton BtnFilterAll
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
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.ChangeFilter((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnFilterAll;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterAll = value;
                myRadioButton = this._BtnFilterAll;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioButton BtnFilterEnabled
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
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.ChangeFilter((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnFilterEnabled;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterEnabled = value;
                myRadioButton = this._BtnFilterEnabled;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioButton BtnFilterDisabled
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
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.ChangeFilter((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnFilterDisabled;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterDisabled = value;
                myRadioButton = this._BtnFilterDisabled;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioButton BtnFilterCanUpdate
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
                MyRadioButton.CheckEventHandler checkEventHandler = delegate(object a0, bool a1)
                {
                    this.ChangeFilter((MyRadioButton)a0, a1);
                };
                MyRadioButton myRadioButton = this._BtnFilterCanUpdate;
                if (myRadioButton != null)
                {
                    myRadioButton.Check -= checkEventHandler;
                }
                this._BtnFilterCanUpdate = value;
                myRadioButton = this._BtnFilterCanUpdate;
                if (myRadioButton != null)
                {
                    myRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual StackPanel PanList
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard PanEmpty
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnHintDownload
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageDownload_Click);
                MyButton myButton = this._BtnHintDownload;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnHintDownload = value;
                myButton = this._BtnHintDownload;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnHintInstall
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageInstall_Click);
                MyButton myButton = this._BtnHintInstall;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnHintInstall = value;
                myButton = this._BtnHintInstall;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnHintOpen
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnManageOpen_Click);
                MyButton myButton = this._BtnHintOpen;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnHintOpen = value;
                myButton = this._BtnHintOpen;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard PanLoad
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyLoading Load
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
                MyLoading.ClickEventHandler clickEventHandler = new MyLoading.ClickEventHandler(this.Load_Click);
                MyLoading.StateChangedEventHandler stateChangedEventHandler = delegate(object a0, MyLoading.MyLoadingState a1, MyLoading.MyLoadingState a2)
                {
                    this.UnselectedAllWithAnimation();
                };
                MyLoading myLoading = this._Load;
                if (myLoading != null)
                {
                    myLoading.Click -= clickEventHandler;
                    myLoading.StateChanged -= stateChangedEventHandler;
                }
                this._Load = value;
                myLoading = this._Load;
                if (myLoading != null)
                {
                    myLoading.Click += clickEventHandler;
                    myLoading.StateChanged += stateChangedEventHandler;
                }
            }
        }

        internal virtual MyCard CardSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TranslateTransform TransSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconTextButton BtnSelectUpdate
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
                MyIconTextButton.ClickEventHandler clickEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.BtnSelectUpdate_Click();
                };
                MyIconTextButton myIconTextButton = this._BtnSelectUpdate;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectUpdate = value;
                myIconTextButton = this._BtnSelectUpdate;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconTextButton BtnSelectEnable
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
                MyIconTextButton.ClickEventHandler clickEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.BtnSelectED_Click((MyIconTextButton)a0, a1);
                };
                MyIconTextButton myIconTextButton = this._BtnSelectEnable;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectEnable = value;
                myIconTextButton = this._BtnSelectEnable;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconTextButton BtnSelectDisable
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
                MyIconTextButton.ClickEventHandler clickEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.BtnSelectED_Click((MyIconTextButton)a0, a1);
                };
                MyIconTextButton myIconTextButton = this._BtnSelectDisable;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectDisable = value;
                myIconTextButton = this._BtnSelectDisable;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconTextButton BtnSelectDelete
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
                MyIconTextButton.ClickEventHandler clickEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.BtnSelectDelete_Click();
                };
                MyIconTextButton myIconTextButton = this._BtnSelectDelete;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectDelete = value;
                myIconTextButton = this._BtnSelectDelete;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconTextButton BtnSelectCancel
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
                MyIconTextButton.ClickEventHandler clickEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.BtnSelectCancel_Click();
                };
                MyIconTextButton myIconTextButton = this._BtnSelectCancel;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click -= clickEventHandler;
                }
                this._BtnSelectCancel = value;
                myIconTextButton = this._BtnSelectCancel;
                if (myIconTextButton != null)
                {
                    myIconTextButton.Click += clickEventHandler;
                }
            }
        }

        private bool IsLoad;

        public ConcurrentDictionary<string, MyLocalModItem> ModItems;

        private int BottomBarShownCount;

        public List<string> SelectedMods;

        private PageInstanceMod.FilterType _Filter;

        public static List<string> UpdatingInstanceModFolders = new List<string>();

        private List<LocalResourceFile> SearchResult;

        public enum FilterType
        {
            All,
            Enabled,
            Disabled,
            CanUpdate
        }
    }
}
