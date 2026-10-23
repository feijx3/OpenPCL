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
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageDownloadResourceDetail : MyPageRight
    {
        public PageDownloadResourceDetail()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.LoadPageArguments();
            };
            base.Initialized += this.PageDownloadResourceDetail_Inited;
            base.PageEnter += this.Init;
            this.ResourceItem = null;
            this.ResourceFileLoader = new ModLoader.LoaderTask<int, List<ResourceVersion>>("Resource File", delegate(ModLoader.LoaderTask<int, List<ResourceVersion>> Task)
            {
                this.LoadPageArguments();
                List<ResourceVersion> list = ResourceVersion.FromProjectId(this.Project.Id, this.Project.Platform, true);
                if (!Task.IsCanceled)
                {
                    Task.Output = list;
                }
            }, null, ThreadPriority.Normal);
            this.IsFirstInit = true;
            this.InitializeComponent();
        }

        public void LoadPageArguments()
        {
            this.Project = (ResourceProject)NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 0 }, null);
            this.TargetVersion = Conversions.ToString(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 2 }, null));
            this.TargetLoaders = (ModLoaders)Conversions.ToInteger(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 3 }, null));
            if (this.TargetLoaders != ModLoaders.None && !this.TargetLoaders.Flags<ModLoaders>().IsSingle<ModLoaders>())
            {
                this.TargetLoaders = this.TargetLoaders.Flags<ModLoaders>().First<ModLoaders>();
            }
            this.TargetResourceType = (ResourceTypes)Conversions.ToInteger(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 4 }, null));
        }

        private void PageDownloadResourceDetail_Inited(object sender, EventArgs e)
        {
            this.LoadPageArguments();
            base.PageLoaderInit<int, List<ResourceVersion>>(this.Load, this.PanLoad, this.PanMain, this.CardIntro, this.ResourceFileLoader, delegate(ModLoader.LoaderTask<int, List<ResourceVersion>> a0)
            {
                this.Load_OnFinish();
            }, null, true);
        }

        private void Load_State(object sender, MyLoading.MyLoadingState state, MyLoading.MyLoadingState oldState)
        {
            ModBase.LoadState state2 = this.ResourceFileLoader.State;
            if (state2 == ModBase.LoadState.Failed)
            {
                string text = "";
                if (this.ResourceFileLoader.Error != null)
                {
                    text = this.ResourceFileLoader.Error.Message;
                }
                if (text.Contains("不是有效的 json 文件"))
                {
                    Logger.Warn("下载的文件 json 列表损坏，已自动重试", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                    base.PageLoaderRestart(null, true);
                }
            }
        }

        private List<ResourceVersion> GetResults()
        {
            List<ResourceVersion> list = this.ResourceFileLoader.Output;
            ResourceTypes targetResourceType = this.TargetResourceType;
            if (targetResourceType != ResourceTypes.ResourcePack && targetResourceType != ResourceTypes.Shader)
            {
                if (targetResourceType == ResourceTypes.Any)
                {
                    list = list.Where<ResourceVersion>((PageDownloadResourceDetail._Closure$__.$I15-0 == null) ? (PageDownloadResourceDetail._Closure$__.$I15-0 = (ResourceVersion r) => r.ResourceType != ResourceTypes.Plugin) : PageDownloadResourceDetail._Closure$__.$I15-0).ToList<ResourceVersion>();
                }
                else
                {
                    list = list.Where<ResourceVersion>((ResourceVersion r) => r.ResourceType == this.TargetResourceType).ToList<ResourceVersion>();
                }
            }
            return list;
        }

        private void Load_OnFinish()
        {
            PageDownloadResourceDetail._Closure$__16-0 CS$<>8__locals1 = new PageDownloadResourceDetail._Closure$__16-0(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Me = this;
            CS$<>8__locals1.$VB$Local_Results = this.GetResults();
            CS$<>8__locals1.$VB$Local_Filters = null;
            VB$AnonymousDelegate_10 vb$AnonymousDelegate_ = delegate
            {
                CS$<>8__locals1.$VB$Local_Filters = (from v in CS$<>8__locals1.$VB$Local_Results.SelectMany<ResourceVersion, string>((PageDownloadResourceDetail._Closure$__.$I16-1 == null) ? (PageDownloadResourceDetail._Closure$__.$I16-1 = (ResourceVersion v) => v.GameVersions) : PageDownloadResourceDetail._Closure$__.$I16-1)
                    select CS$<>8__locals1.$VB$Me.GetGroupedVersionName(v, CS$<>8__locals1.$VB$Me.GroupedDrop, CS$<>8__locals1.$VB$Me.GroupedOld)).Distinct<string>().OrderByDescending<string, string>((PageDownloadResourceDetail._Closure$__.$I16-3 == null) ? (PageDownloadResourceDetail._Closure$__.$I16-3 = (string s) => s) : PageDownloadResourceDetail._Closure$__.$I16-3, Comparer<string>.Create((PageDownloadResourceDetail._Closure$__.$I16-4 == null) ? (PageDownloadResourceDetail._Closure$__.$I16-4 = (string x, string y) => McUtils.CompareVersion(x, y)) : PageDownloadResourceDetail._Closure$__.$I16-4)).ToList<string>();
            };
            this.GroupedDrop = false;
            this.GroupedOld = false;
            vb$AnonymousDelegate_();
            if (CS$<>8__locals1.$VB$Local_Filters.Count >= 9)
            {
                this.GroupedDrop = true;
                this.GroupedOld = false;
                vb$AnonymousDelegate_();
                if (CS$<>8__locals1.$VB$Local_Filters.Count >= 9)
                {
                    this.GroupedDrop = false;
                    this.GroupedOld = true;
                    vb$AnonymousDelegate_();
                    if (CS$<>8__locals1.$VB$Local_Filters.Count >= 9)
                    {
                        this.GroupedDrop = true;
                        this.GroupedOld = true;
                        vb$AnonymousDelegate_();
                    }
                }
            }
            this.PanFilter.Children.Clear();
            if (CS$<>8__locals1.$VB$Local_Filters.Count < 2)
            {
                this.CardFilter.Visibility = Visibility.Collapsed;
                this.VersionFilter = null;
            }
            else
            {
                this.CardFilter.Visibility = Visibility.Visible;
                CS$<>8__locals1.$VB$Local_Filters.Insert(0, "全部");
                try
                {
                    foreach (string text in CS$<>8__locals1.$VB$Local_Filters)
                    {
                        MyRadioButton myRadioButton = new MyRadioButton
                        {
                            Text = text,
                            Margin = new Thickness(2.0, 0.0, 2.0, 0.0),
                            ColorType = MyRadioButton.ColorState.Highlight
                        };
                        myRadioButton.LabText.Margin = new Thickness(-2.0, 0.0, 10.0, 0.0);
                        myRadioButton.Check += delegate(object a0, bool a1)
                        {
                            this._Lambda$__16-5((MyRadioButton)a0, a1);
                        };
                        this.PanFilter.Children.Add(myRadioButton);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                MyRadioButton myRadioButton2 = null;
                if (Operators.CompareString(this.TargetVersion, "", false) != 0 && CS$<>8__locals1.$VB$Local_Results.FirstOrDefault<ResourceVersion>((ResourceVersion v) => v.GameVersions.Contains(this.TargetVersion)) != null)
                {
                    string groupedVersionName = this.GetGroupedVersionName(this.TargetVersion, this.GroupedDrop, this.GroupedOld);
                    try
                    {
                        foreach (object obj in this.PanFilter.Children)
                        {
                            MyRadioButton myRadioButton3 = (MyRadioButton)obj;
                            if (Operators.CompareString(myRadioButton3.Text, groupedVersionName, false) == 0)
                            {
                                myRadioButton2 = myRadioButton3;
                                break;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator2;
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                }
                if (myRadioButton2 == null)
                {
                    myRadioButton2 = (MyRadioButton)this.PanFilter.Children[0];
                }
                myRadioButton2.Checked = true;
            }
            this.UpdateFilterResult();
        }

        private void UpdateFilterResult()
        {
            List<ResourceVersion> results = this.GetResults();
            string text = ((Operators.CompareString(this.TargetVersion, "", false) != 0 || this.TargetLoaders != ModLoaders.None) ? string.Format("{0}{1}（所选版本）", (this.TargetLoaders != ModLoaders.None) ? (this.TargetLoaders.ToString() + " ") : "", this.TargetVersion) : "");
            SortedDictionary<string, List<ResourceVersion>> sortedDictionary = new SortedDictionary<string, List<ResourceVersion>>(new PageDownloadResourceDetail.CardSorter(text));
            sortedDictionary.Add("其他", new List<ResourceVersion>());
            try
            {
                foreach (ResourceVersion resourceVersion in results)
                {
                    try
                    {
                        foreach (string text2 in resourceVersion.GameVersions)
                        {
                            if (this.VersionFilter == null || Operators.CompareString(this.GetGroupedVersionName(text2, this.GroupedDrop, this.GroupedOld), this.VersionFilter, false) == 0)
                            {
                                string groupedVersionName = this.GetGroupedVersionName(text2, false, false);
                                List<string> list = new List<string>();
                                if (this.Project.ModLoaders.Flags<ModLoaders>().Count<ModLoaders>() > 1 && resourceVersion.ResourceType == ResourceTypes.Mod && McVersion.IsStandardFormat(groupedVersionName))
                                {
                                    list.AddRange(resourceVersion.ModLoaders.Flags<ModLoaders>().Select<ModLoaders, string>((PageDownloadResourceDetail._Closure$__.$I17-0 == null) ? (PageDownloadResourceDetail._Closure$__.$I17-0 = (ModLoaders f) => f.ToString() + " ") : PageDownloadResourceDetail._Closure$__.$I17-0));
                                }
                                if (!list.Any<string>())
                                {
                                    list.Add("");
                                }
                                try
                                {
                                    foreach (string text3 in list)
                                    {
                                        string text4 = text3 + groupedVersionName;
                                        if (!sortedDictionary.ContainsKey(text4))
                                        {
                                            sortedDictionary.Add(text4, new List<ResourceVersion>());
                                        }
                                        if (!sortedDictionary[text4].Contains(resourceVersion))
                                        {
                                            sortedDictionary[text4].Add(resourceVersion);
                                        }
                                    }
                                }
                                finally
                                {
                                    List<string>.Enumerator enumerator3;
                                    ((IDisposable)enumerator3).Dispose();
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator2;
                        ((IDisposable)enumerator2).Dispose();
                    }
                }
            }
            finally
            {
                List<ResourceVersion>.Enumerator enumerator;
                ((IDisposable)enumerator).Dispose();
            }
            if (Operators.CompareString(text, "", false) != 0 && (this.VersionFilter == null || this.GetGroupedVersionName(this.TargetVersion, this.GroupedDrop, this.GroupedOld).StartsWithF(this.VersionFilter, false)))
            {
                sortedDictionary.Add(text, new List<ResourceVersion>());
                try
                {
                    foreach (ResourceVersion resourceVersion2 in results)
                    {
                        if (resourceVersion2.GameVersions.Contains(this.TargetVersion) && (this.TargetLoaders == ModLoaders.None || this.TargetLoaders.Flags<ModLoaders>().Intersect<ModLoaders>(resourceVersion2.ModLoaders.Flags<ModLoaders>()).Any<ModLoaders>()) && (this.VersionFilter == null || resourceVersion2.GameVersions.Any<string>((string v) => Operators.CompareString(this.GetGroupedVersionName(v, this.GroupedDrop, this.GroupedOld), this.VersionFilter, false) == 0)) && !sortedDictionary[text].Contains(resourceVersion2))
                        {
                            sortedDictionary[text].Add(resourceVersion2);
                        }
                    }
                }
                finally
                {
                    List<ResourceVersion>.Enumerator enumerator4;
                    ((IDisposable)enumerator4).Dispose();
                }
            }
            try
            {
                this.PanResults.Children.Clear();
                try
                {
                    foreach (KeyValuePair<string, List<ResourceVersion>> keyValuePair in sortedDictionary)
                    {
                        if (keyValuePair.Value.Any<ResourceVersion>() && Operators.CompareString(keyValuePair.Key, text.Replace("（所选版本）", ""), false) != 0)
                        {
                            MyCard myCard = new MyCard
                            {
                                Title = keyValuePair.Key,
                                Margin = new Thickness(0.0, 0.0, 0.0, 15.0)
                            };
                            StackPanel stackPanel = new StackPanel
                            {
                                Margin = new Thickness(20.0, 40.0, 18.0, 0.0),
                                VerticalAlignment = VerticalAlignment.Top,
                                RenderTransform = new TranslateTransform(0.0, 0.0),
                                Tag = keyValuePair.Value
                            };
                            myCard.Children.Add(stackPanel);
                            myCard.SwapControl = stackPanel;
                            myCard.SetContentBuilder(delegate(Panel a0)
                            {
                                this.InitializeStackResourceVersions((StackPanel)a0);
                            });
                            this.PanResults.Children.Add(myCard);
                            if (Operators.CompareString(keyValuePair.Key, text, false) == 0 || (ModMain.FrmMain.PageCurrent.Additional != null && ((List<string>)NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 1 }, null)).Contains(myCard.Title)))
                            {
                                myCard.BuildContent();
                            }
                            else
                            {
                                myCard.IsSwapped = true;
                            }
                            if (Operators.CompareString(keyValuePair.Key, "其他", false) == 0)
                            {
                                stackPanel.Children.Add(new MyHint
                                {
                                    Text = "由于版本信息更新缓慢，可能无法识别刚更新的 MC 版本。几天后即可正常识别。",
                                    Theme = MyHint.Themes.Yellow,
                                    Margin = new Thickness(5.0, 0.0, 0.0, 8.0)
                                });
                            }
                        }
                    }
                }
                finally
                {
                    SortedDictionary<string, List<ResourceVersion>>.Enumerator enumerator5;
                    ((IDisposable)enumerator5).Dispose();
                }
                if (this.PanResults.Children.Count == 1)
                {
                    ((MyCard)this.PanResults.Children[0]).IsSwapped = false;
                }
                if (this.Project.Types == ResourceTypes.ModOrDataPack && (this.TargetResourceType == ResourceTypes.Mod || this.TargetResourceType == ResourceTypes.DataPack))
                {
                    this.HintAlternative.Visibility = Visibility.Visible;
                    this.HintAlternative.Text = ((this.TargetResourceType == ResourceTypes.Mod) ? "以下是该项目的 Mod 版本。点击这里查看其数据包版本。" : "以下是该项目的数据包版本。点击这里查看其 Mod 版本。");
                }
                else
                {
                    this.HintAlternative.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "可视化工程下载列表出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
            }
        }

        private void InitializeStackResourceVersions(StackPanel Stack)
        {
            List<ResourceVersion> list = ((List<ResourceVersion>)Stack.Tag).SortByComparison<ResourceVersion>((PageDownloadResourceDetail._Closure$__.$I18-0 == null) ? (PageDownloadResourceDetail._Closure$__.$I18-0 = (ResourceVersion a, ResourceVersion b) => DateTime.Compare(a.ReleaseDate, b.ReleaseDate) > 0) : PageDownloadResourceDetail._Closure$__.$I18-0);
            bool flag = list.DistinctBy<ResourceVersion, string>((PageDownloadResourceDetail._Closure$__.$I18-1 == null) ? (PageDownloadResourceDetail._Closure$__.$I18-1 = (ResourceVersion a) => a.Display) : PageDownloadResourceDetail._Closure$__.$I18-1).Count<ResourceVersion>() != list.Count;
            List<string> list2;
            if (this.TargetResourceType == ResourceTypes.ModPack)
            {
                list2 = new List<string>();
            }
            else
            {
                list2 = list.SelectMany<ResourceVersion, string>((PageDownloadResourceDetail._Closure$__.$I18-2 == null) ? (PageDownloadResourceDetail._Closure$__.$I18-2 = (ResourceVersion f) => f.Dependencies) : PageDownloadResourceDetail._Closure$__.$I18-2).Distinct<string>().Ordered<string>()
                    .ToList<string>();
            }
            if (list2.Any<string>())
            {
                list2 = list2.Where<string>((PageDownloadResourceDetail._Closure$__.$I18-3 == null) ? (PageDownloadResourceDetail._Closure$__.$I18-3 = delegate(string d)
                {
                    if (!ResourceProject.Cache.ContainsKey(d))
                    {
                        Logger.Warn(string.Format("未找到 ID {0} 的前置信息", d), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                    }
                    return ResourceProject.Cache.ContainsKey(d);
                }) : PageDownloadResourceDetail._Closure$__.$I18-3).ToList<string>();
                Stack.Children.Add(new TextBlock
                {
                    Text = "前置资源",
                    FontSize = 14.0,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(6.0, 2.0, 0.0, 5.0)
                });
                try
                {
                    foreach (string text in list2)
                    {
                        MyVirtualizingElement<MyResourceItem> myVirtualizingElement = ResourceProject.Cache[text].ToResourceItem(false, false);
                        Stack.Children.Add(myVirtualizingElement);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                Stack.Children.Add(new TextBlock
                {
                    Text = "版本列表",
                    FontSize = 14.0,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(6.0, 12.0, 0.0, 5.0)
                });
            }
            try
            {
                foreach (ResourceVersion resourceVersion in list)
                {
                    Stack.Children.Add(resourceVersion.ToListItem(new MyListItem.ClickEventHandler(this.Save_Click), null, flag));
                }
            }
            finally
            {
                List<ResourceVersion>.Enumerator enumerator2;
                ((IDisposable)enumerator2).Dispose();
            }
            Stack.Children.Add(new FrameworkElement
            {
                Height = 18.0
            });
            Stack.Tag = null;
        }

        private string GetGroupedVersionName(string Name, bool GroupedByDrop, bool FoldOld)
        {
            int num = McVersion.VersionToDrop(Name);
            string text;
            if (Name == null)
            {
                text = "其他";
            }
            else if (Name.Contains("w") || num == 209)
            {
                text = "快照版";
            }
            else if (!McVersion.IsStandardFormat(Name) || (FoldOld && num < 120))
            {
                text = "远古版";
            }
            else if (GroupedByDrop)
            {
                text = McVersion.DropToVersion(num);
            }
            else
            {
                text = Name;
            }
            return text;
        }

        public void Init()
        {
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.Project = (ResourceProject)NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 0 }, null);
                this.PanBack.ScrollToHome();
                if (this.IsFirstInit)
                {
                    this.IsFirstInit = false;
                }
                else
                {
                    base.PageLoaderRestart(null, true);
                }
                if (this.ResourceItem != null)
                {
                    this.PanIntro.Children.Remove(this.ResourceItem);
                }
                this.ResourceItem = this.Project.ToResourceItem(true, true).Init();
                this.ResourceItem.CanInteraction = false;
                this.ResourceItem.Margin = new Thickness(-7.0, -7.0, 0.0, 8.0);
                this.PanIntro.Children.Insert(0, this.ResourceItem);
                this.BtnIntroWeb.Text = string.Format("转到 {0}", this.Project.Platform);
                this.BtnIntroWiki.Visibility = ((this.Project.WikiId == 0) ? Visibility.Collapsed : Visibility.Visible);
                ModAnimation.AniControlEnabled--;
            }
        }

        public void Install_Click(MyListItem sender, EventArgs e)
        {
            try
            {
                ResourceVersion resourceVersion = (ResourceVersion)sender.Tag;
                string text = string.Format("{0} 整合包下载：{1} ", this.Project.Platform, this.Project.TranslatedName);
                string text2 = this.Project.TranslatedName.Replace(".zip", "").Replace(".rar", "").Replace(".mrpack", "")
                    .Replace("\\", "＼")
                    .Replace("/", "／")
                    .Replace("|", "｜")
                    .Replace(":", "：")
                    .Replace("<", "＜")
                    .Replace(">", "＞")
                    .Replace("*", "＊")
                    .Replace("?", "？")
                    .Replace("\"", "")
                    .Replace("： ", "：");
                ValidateFolderName validateFolderName = new ValidateFolderName(ModMinecraft.McFolderSelected + "versions", true, null);
                if (Operators.CompareString(validateFolderName.Validate(text2), "", false) != 0)
                {
                    text2 = "";
                }
                string InstanceName = ModMain.MyMsgBoxInput("输入版本名称", "", text2, new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                if (!string.IsNullOrEmpty(InstanceName))
                {
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    string Target = string.Format("{0}versions\\{1}\\原始整合包.{2}", ModMinecraft.McFolderSelected, InstanceName, (this.Project.Platform == ResourcePlatforms.CurseForge) ? "zip" : "mrpack");
                    string LogoFileAddress = this.ResourceItem.PathLogo.ActualSource;
                    list.Add(new ModNet.LoaderDownload("下载整合包文件", new ModNet.NetFile[] { resourceVersion.ToNetFile(Target, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Standalone), resourceVersion.GameVersions.FirstOrDefault<string>(), resourceVersion.ModLoaders) })
                    {
                        ProgressWeight = 10.0,
                        Block = true
                    });
                    list.Add(new ModLoader.LoaderTask<int, int>("准备安装整合包", delegate(ModLoader.LoaderTask<int, int> a0)
                    {
                        base._Lambda$__0();
                    }, null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 0.1
                    });
                    ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(text, list);
                    loaderCombo.OnStateChanged = ((PageDownloadResourceDetail._Closure$__.$I22-1 == null) ? (PageDownloadResourceDetail._Closure$__.$I22-1 = delegate(ModLoader.LoaderBase MyLoader)
                    {
                        switch (MyLoader.State)
                        {
                        case ModBase.LoadState.Loading:
                            return;
                        case ModBase.LoadState.Failed:
                            ModMain.Hint(MyLoader.Name + "失败：" + MyLoader.Error.GetDisplay(false), ModMain.HintType.Red, true);
                            break;
                        case ModBase.LoadState.Canceled:
                            ModMain.Hint(MyLoader.Name + "已取消！", ModMain.HintType.Blue, true);
                            break;
                        }
                        ModDownloadLib.McInstallFailedClearFolder(MyLoader);
                    }) : PageDownloadResourceDetail._Closure$__.$I22-1);
                    loaderCombo.Start(ModMinecraft.McFolderSelected + "versions\\" + InstanceName + "\\", false);
                    ModLoader.LoaderTaskbarAdd<string>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.PageChange(FormMain.PageType.TaskManager, FormMain.PageSubType.Default);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "下载资源整合包失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
            }
        }

        public void Save_Click(object sender, EventArgs e)
        {
            ResourceVersion File = (ResourceVersion)NewLateBinding.LateGet((sender is MyListItem) ? sender : NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            ModBase.RunInNewThread(delegate
            {
                try
                {
                    string Desc = null;
                    ResourceTypes resourceType = File.ResourceType;
                    switch (resourceType)
                    {
                    case ResourceTypes.Mod:
                        Desc = "Mod ";
                        break;
                    case ResourceTypes.ModPack:
                        Desc = "整合包";
                        break;
                    case ResourceTypes.Mod | ResourceTypes.ModPack:
                        break;
                    case ResourceTypes.ResourcePack:
                        Desc = "资源包";
                        break;
                    default:
                        if (resourceType != ResourceTypes.Shader)
                        {
                            if (resourceType == ResourceTypes.DataPack)
                            {
                                Desc = "数据包";
                            }
                        }
                        else
                        {
                            Desc = "光影包";
                        }
                        break;
                    }
                    string DefaultFolder = null;
                    if (File.ResourceType != ResourceTypes.ModPack)
                    {
                        string SubFolder = null;
                        ResourceTypes resourceType2 = File.ResourceType;
                        if (resourceType2 <= ResourceTypes.ResourcePack)
                        {
                            if (resourceType2 != ResourceTypes.Mod)
                            {
                                if (resourceType2 == ResourceTypes.ResourcePack)
                                {
                                    SubFolder = "resourcepacks\\";
                                }
                            }
                            else
                            {
                                SubFolder = "mods\\";
                            }
                        }
                        else if (resourceType2 != ResourceTypes.Shader)
                        {
                            if (resourceType2 == ResourceTypes.DataPack)
                            {
                                SubFolder = "";
                            }
                        }
                        else
                        {
                            SubFolder = "shaderpacks\\";
                        }
                        Func<ModMinecraft.McInstance, bool> IsInstanceSuitable = null;
                        ModLoaders AllowedLoaders = ModLoaders.None;
                        if (File.ModLoaders != ModLoaders.None)
                        {
                            AllowedLoaders = File.ModLoaders;
                        }
                        else if (this.Project.ModLoaders != ModLoaders.None)
                        {
                            AllowedLoaders = this.Project.ModLoaders;
                        }
                        Logger.Info(string.Format("{0}要求的加载器种类：{1}", Desc, AllowedLoaders), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                        IsInstanceSuitable = delegate(ModMinecraft.McInstance Instance)
                        {
                            if (!Instance.IsLoaded)
                            {
                                Instance.Load();
                            }
                            return ((File.ResourceType != ResourceTypes.Mod && File.ResourceType != ResourceTypes.DataPack) || File.GameVersions.Any<string>(delegate(string FileVersion)
                            {
                                bool flag2 = FileVersion.Contains("预览版") || !FileVersion.Contains(".");
                                bool flag3 = Instance.Version.VanillaName.Contains("snapshot") || !Instance.Version.VanillaName.Contains(".");
                                bool flag4;
                                if (flag2 != flag3)
                                {
                                    flag4 = false;
                                }
                                else if (!flag2 && !flag3)
                                {
                                    flag4 = Operators.CompareString(FileVersion, Instance.Version.VanillaName, false) == 0;
                                }
                                else
                                {
                                    bool flag5 = FileVersion.Contains(".") && ModBase.Val(FileVersion.BeforeFirst(".", false)) > 1.0;
                                    bool flag6 = Instance.Version.Drop > 250;
                                    if (flag5 != flag6)
                                    {
                                        flag4 = false;
                                    }
                                    else if (flag5 && flag6)
                                    {
                                        flag4 = Operators.CompareString(FileVersion.BeforeFirst(" ", false), Instance.Version.VanillaName.BeforeFirst("-", false), false) == 0;
                                    }
                                    else
                                    {
                                        flag4 = !FileVersion.Contains("w") || !Instance.Version.VanillaName.Contains("w") || Operators.CompareString(FileVersion, Instance.Version.VanillaName, false) == 0;
                                    }
                                }
                                return flag4;
                            })) && (AllowedLoaders == ModLoaders.None || AllowedLoaders.Flags<ModLoaders>().Intersect<ModLoaders>(Instance.Version.ModLoaders.Flags<ModLoaders>()).Any<ModLoaders>());
                        };
                        if (PageDownloadResourceDetail.CachedFolder.ContainsKey(File.ResourceType))
                        {
                            DefaultFolder = PageDownloadResourceDetail.CachedFolder[File.ResourceType];
                            Logger.Info(string.Format("使用上次下载时的文件夹作为默认下载位置：{0}", DefaultFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                        }
                        else if (ModMinecraft.McInstanceSelected != null && IsInstanceSuitable(ModMinecraft.McInstanceSelected))
                        {
                            DefaultFolder = string.Format("{0}{1}", ModMinecraft.McInstanceSelected.PathIndie, SubFolder);
                            DirectoryUtils.Create(DefaultFolder);
                            Logger.Info(string.Format("使用当前版本作为默认下载位置：{0}", DefaultFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                        }
                        else
                        {
                            bool flag = ModMinecraft.McInstanceListLoader.State != ModBase.LoadState.Finished;
                            if (flag)
                            {
                                ModMain.Hint("正在查找适合的游戏版本……", ModMain.HintType.Blue, true);
                                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", true);
                            }
                            IEnumerable<DirectoryInfo> enumerable = from v in ModMinecraft.McInstanceList.Values.SelectMany<List<ModMinecraft.McInstance>, ModMinecraft.McInstance>((PageDownloadResourceDetail._Closure$__.$I24-3 == null) ? (PageDownloadResourceDetail._Closure$__.$I24-3 = (List<ModMinecraft.McInstance> l) => l) : PageDownloadResourceDetail._Closure$__.$I24-3)
                                where IsInstanceSuitable(v)
                                select DirectoryUtils.GetInfo(string.Format("{0}{1}", v.PathIndie, SubFolder));
                            if (enumerable.Any<DirectoryInfo>())
                            {
                                DirectoryInfo directoryInfo = enumerable.OrderByDescending<DirectoryInfo, DateTime>((PageDownloadResourceDetail._Closure$__.$I24-6 == null) ? (PageDownloadResourceDetail._Closure$__.$I24-6 = delegate(DirectoryInfo Dir)
                                {
                                    if (!Dir.Exists)
                                    {
                                        return DateTime.MinValue;
                                    }
                                    return Dir.LastWriteTimeUtc;
                                }) : PageDownloadResourceDetail._Closure$__.$I24-6).ThenByDescending<DirectoryInfo, int>((PageDownloadResourceDetail._Closure$__.$I24-7 == null) ? (PageDownloadResourceDetail._Closure$__.$I24-7 = delegate(DirectoryInfo Dir)
                                {
                                    if (!Dir.Exists)
                                    {
                                        return -1;
                                    }
                                    return Dir.GetFiles().Length;
                                }) : PageDownloadResourceDetail._Closure$__.$I24-7).First<DirectoryInfo>();
                                DefaultFolder = PathUtils.RemoveExtendedPrefix(directoryInfo.FullName);
                                DirectoryUtils.Create(DefaultFolder);
                                Logger.Info(string.Format("使用适合的游戏版本作为默认下载位置：{0}", DefaultFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                            }
                            else
                            {
                                DefaultFolder = ModMinecraft.McFolderSelected;
                                if (flag)
                                {
                                    ModMain.Hint("当前 MC 文件夹中没有找到合适的版本！", ModMain.HintType.Blue, true);
                                }
                                else
                                {
                                    Logger.Info("由于当前版本不兼容，使用当前的 MC 文件夹作为默认下载位置", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                                }
                            }
                        }
                    }
                    string FileName;
                    if (Operators.CompareString(this.Project.TranslatedName, this.Project.RawName, false) == 0)
                    {
                        FileName = File.FileName;
                    }
                    else
                    {
                        string text = this.Project.TranslatedName.BeforeFirst(" (", false).BeforeFirst(" - ", false).Replace("\\", "＼")
                            .Replace("/", "／")
                            .Replace("|", "｜")
                            .Replace(":", "：")
                            .Replace("<", "＜")
                            .Replace(">", "＞")
                            .Replace("*", "＊")
                            .Replace("?", "？")
                            .Replace("\"", "")
                            .Replace("： ", "：");
                        switch (Settings.Get<int>("ToolDownloadTranslateV2", null))
                        {
                        case 0:
                            FileName = string.Format("【{0}】{1}", text, File.FileName);
                            break;
                        case 1:
                            FileName = string.Format("[{0}] {1}", text, File.FileName);
                            break;
                        case 2:
                            FileName = string.Format("{0}-{1}", text, File.FileName);
                            break;
                        case 3:
                            FileName = string.Format("{0}-{1}", File.FileName, text);
                            break;
                        default:
                            FileName = File.FileName;
                            break;
                        }
                    }
                    if (File.ResourceType == ResourceTypes.Mod)
                    {
                        FileName = FileName.Replace("~", "-");
                    }
                    ModBase.RunInUi(delegate
                    {
                        string text2 = ((File.ResourceType == ResourceTypes.Mod) ? ((Operators.CompareString(PathUtils.GetExtension(File.FileName), "litemod", false) == 0) ? "litemod" : "jar") : ((Operators.CompareString(PathUtils.GetExtension(File.FileName), "mrpack", false) == 0) ? "mrpack" : "zip"));
                        string Target = Dialogs.SaveFile("选择保存位置", FileName, DefaultFolder, new ValueTuple<string, string>[]
                        {
                            new ValueTuple<string, string>(text2, Desc + "文件")
                        });
                        if (Target != null)
                        {
                            string text3 = Desc + "下载：" + PathUtils.GetFileNameWithoutExtension(Target) + " ";
                            if (!PathUtils.IsSame(PathUtils.RemoveLastPart(Target), DefaultFolder))
                            {
                                PageDownloadResourceDetail.CachedFolder[File.ResourceType] = PathUtils.RemoveLastPart(Target);
                            }
                            ModLoader.LoaderCombo<int> loaderCombo = new ModLoader.LoaderCombo<int>(text3, new List<ModLoader.LoaderBase>
                            {
                                new ModNet.LoaderDownload("下载文件", new ModNet.NetFile[] { File.ToNetFile(Target, new ResourceVersion.DownloadReason?((this.TargetResourceType == ResourceTypes.Any) ? ResourceVersion.DownloadReason.Dependency : ResourceVersion.DownloadReason.Standalone), null, ModLoaders.None) })
                                {
                                    ProgressWeight = 6.0,
                                    Block = true
                                }
                            });
                            loaderCombo.OnStateChanged = delegate(ModLoader.LoaderBase MyLoader)
                            {
                                ModDownloadLib.LoaderStateChangedHintOnly(MyLoader);
                                if (MyLoader.State == ModBase.LoadState.Finished && File.ResourceType == ResourceTypes.ModPack)
                                {
                                    ModBase.OpenExplorer(Target);
                                }
                            };
                            loaderCombo.Start(1, false);
                            ModLoader.LoaderTaskbarAdd<int>(loaderCombo);
                            ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                            ModMain.FrmMain.BtnExtraDownload.Ribble();
                        }
                    }, false);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "保存资源文件失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                }
            }, "Download Resource Detail Save", ThreadPriority.Normal);
        }

        private void BtnIntroWeb_Click(object sender, EventArgs e)
        {
            ModBase.OpenWebsite(this.Project.Website);
        }

        private void BtnIntroWiki_Click(object sender, EventArgs e)
        {
            ModBase.OpenWebsite("https://www.mcmod.cn/class/" + Conversions.ToString(this.Project.WikiId) + ".html");
        }

        private void BtnIntroCopy_Click(object sender, EventArgs e)
        {
            ModBase.ClipboardSet(this.ResourceItem.LabTitle.Text + this.ResourceItem.LabTitleRaw.Text, true);
        }

        private void HintAlternative_Click()
        {
            this.TargetResourceType = ((this.TargetResourceType == ResourceTypes.Mod) ? ResourceTypes.DataPack : ResourceTypes.Mod);
            NewLateBinding.LateIndexSet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 4, this.TargetResourceType }, null);
            base.PageLoaderRestart(null, true);
        }

        internal virtual MyScrollViewer PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardIntro
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanIntro
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnIntroWeb
        {
            [CompilerGenerated]
            get
            {
                return this._BtnIntroWeb;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnIntroWeb_Click);
                MyButton myButton = this._BtnIntroWeb;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnIntroWeb = value;
                myButton = this._BtnIntroWeb;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnIntroWiki
        {
            [CompilerGenerated]
            get
            {
                return this._BtnIntroWiki;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnIntroWiki_Click);
                MyButton myButton = this._BtnIntroWiki;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnIntroWiki = value;
                myButton = this._BtnIntroWiki;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnIntroCopy
        {
            [CompilerGenerated]
            get
            {
                return this._BtnIntroCopy;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnIntroCopy_Click);
                MyButton myButton = this._BtnIntroCopy;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnIntroCopy = value;
                myButton = this._BtnIntroCopy;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual StackPanel PanMain
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintAlternative
        {
            [CompilerGenerated]
            get
            {
                return this._HintAlternative;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyHint.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.HintAlternative_Click();
                };
                MyHint myHint = this._HintAlternative;
                if (myHint != null)
                {
                    myHint.Click -= clickEventHandler;
                }
                this._HintAlternative = value;
                myHint = this._HintAlternative;
                if (myHint != null)
                {
                    myHint.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard CardFilter
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanFilter
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanResults
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
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
                MyLoading.StateChangedEventHandler stateChangedEventHandler = new MyLoading.StateChangedEventHandler(this.Load_State);
                MyLoading myLoading = this._Load;
                if (myLoading != null)
                {
                    myLoading.StateChanged -= stateChangedEventHandler;
                }
                this._Load = value;
                myLoading = this._Load;
                if (myLoading != null)
                {
                    myLoading.StateChanged += stateChangedEventHandler;
                }
            }
        }

        private MyResourceItem ResourceItem;

        private ResourceTypes TargetResourceType;

        private ResourceProject Project;

        private string TargetVersion;

        private ModLoaders TargetLoaders;

        private ModLoader.LoaderTask<int, List<ResourceVersion>> ResourceFileLoader;

        private string VersionFilter;

        private bool GroupedDrop;

        private bool GroupedOld;

        private bool IsFirstInit;

        public static Dictionary<ResourceTypes, string> CachedFolder = new Dictionary<ResourceTypes, string>();

        private class CardSorter : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                checked
                {
                    int num;
                    if (Operators.CompareString(x, y, false) == 0)
                    {
                        num = 0;
                    }
                    else if (Operators.CompareString(x, this.Topmost, false) == 0)
                    {
                        num = -1;
                    }
                    else if (Operators.CompareString(y, this.Topmost, false) == 0)
                    {
                        num = 1;
                    }
                    else
                    {
                        bool flag = !x.Contains(".");
                        bool flag2 = !y.Contains(".");
                        if (flag && flag2)
                        {
                            num = x.CompareTo(y);
                        }
                        else if (flag)
                        {
                            num = 1;
                        }
                        else if (flag2)
                        {
                            num = -1;
                        }
                        else
                        {
                            int num2 = 0 - McUtils.CompareVersion(x.Replace(x.BeforeFirst(" ", false) + " ", ""), y.Replace(y.BeforeFirst(" ", false) + " ", ""));
                            if (num2 != 0)
                            {
                                num = num2;
                            }
                            else
                            {
                                num = 0 - McUtils.CompareVersion(x, y);
                            }
                        }
                    }
                    return num;
                }
            }

            public CardSorter(string Topmost = "")
            {
                this.Topmost = "";
                this.Topmost = Topmost ?? "";
            }

            public string Topmost;
        }
    }
}
