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
    public partial class PageDownloadResourceDetail : OurPageRight
    {
        public PageDownloadResourceDetail()
        {
            base.Loaded += this._Lambda___R1_1;
            base.Initialized += this.PageDownloadResourceDetail_Inited;
            base.PageEnter += this.Init;
            this.ResourceItem = null;
            this.ResourceFileLoader = new ModLoader.LoaderTask<int, List<ResourceVersion>>("Resource File", new Action<ModLoader.LoaderTask<int, List<ResourceVersion>>>(this._Lambda___1_0), null, ThreadPriority.Normal);
            this.IsFirstInit = true;
            this.InitializeComponent();
            this.BtnIntroWeb_WithEvents = this.BtnIntroWeb;
            this.BtnIntroWiki_WithEvents = this.BtnIntroWiki;
            this.BtnIntroCopy_WithEvents = this.BtnIntroCopy;
            this.HintAlternative_WithEvents = this.HintAlternative;
            this.Load_WithEvents = this.Load;
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
            base.PageLoaderInit<int, List<ResourceVersion>>(this.Load, this.PanLoad, this.PanMain, this.CardIntro, this.ResourceFileLoader, new Action<ModLoader.LoaderTask<int, List<ResourceVersion>>>(this._Lambda___R9_2), null, true);
        }

        private void Load_State(object sender, OurLoading.OurLoadingState state, OurLoading.OurLoadingState oldState)
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
                    list = list.Where<ResourceVersion>((PageDownloadResourceDetail._Closure___._I15_0 == null) ? (PageDownloadResourceDetail._Closure___._I15_0 = new Func<ResourceVersion, bool>(PageDownloadResourceDetail._Closure___._I._Lambda___15_0)) : PageDownloadResourceDetail._Closure___._I15_0).ToList<ResourceVersion>();
                }
                else
                {
                    list = list.Where<ResourceVersion>(new Func<ResourceVersion, bool>(this._Lambda___15_1)).ToList<ResourceVersion>();
                }
            }
            return list;
        }

        private void Load_OnFinish()
        {
            PageDownloadResourceDetail._Closure___16_0 CS_locals1 = new PageDownloadResourceDetail._Closure___16_0(null);
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_Results = this.GetResults();
            CS_locals1._VB_Local_Filters = null;
            VB_AnonymousDelegate_10 vb_AnonymousDelegate_ = new VB_AnonymousDelegate_10(CS_locals1._Lambda___0);
            this.GroupedDrop = false;
            this.GroupedOld = false;
            vb_AnonymousDelegate_();
            if (CS_locals1._VB_Local_Filters.Count >= 9)
            {
                this.GroupedDrop = true;
                this.GroupedOld = false;
                vb_AnonymousDelegate_();
                if (CS_locals1._VB_Local_Filters.Count >= 9)
                {
                    this.GroupedDrop = false;
                    this.GroupedOld = true;
                    vb_AnonymousDelegate_();
                    if (CS_locals1._VB_Local_Filters.Count >= 9)
                    {
                        this.GroupedDrop = true;
                        this.GroupedOld = true;
                        vb_AnonymousDelegate_();
                    }
                }
            }
            this.PanFilter.Children.Clear();
            if (CS_locals1._VB_Local_Filters.Count < 2)
            {
                this.CardFilter.Visibility = Visibility.Collapsed;
                this.VersionFilter = null;
            }
            else
            {
                this.CardFilter.Visibility = Visibility.Visible;
                CS_locals1._VB_Local_Filters.Insert(0, "全部");
                try
                {
                    foreach (string text in CS_locals1._VB_Local_Filters)
                    {
                        OurRadioButton ourRadioButton = new OurRadioButton
                        {
                            Text = text,
                            Margin = new Thickness(2.0, 0.0, 2.0, 0.0),
                            ColorType = OurRadioButton.ColorState.Highlight
                        };
                        ourRadioButton.LabText.Margin = new Thickness(-2.0, 0.0, 10.0, 0.0);
                        ourRadioButton.Check += this._Lambda___R16_3;
                        this.PanFilter.Children.Add(ourRadioButton);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                OurRadioButton ourRadioButton2 = null;
                if (Operators.CompareString(this.TargetVersion, "", false) != 0 && CS_locals1._VB_Local_Results.FirstOrDefault<ResourceVersion>(new Func<ResourceVersion, bool>(this._Lambda___16_6)) != null)
                {
                    string groupedVersionName = this.GetGroupedVersionName(this.TargetVersion, this.GroupedDrop, this.GroupedOld);
                    try
                    {
                        foreach (object obj in this.PanFilter.Children)
                        {
                            OurRadioButton ourRadioButton3 = (OurRadioButton)obj;
                            if (Operators.CompareString(ourRadioButton3.Text, groupedVersionName, false) == 0)
                            {
                                ourRadioButton2 = ourRadioButton3;
                                break;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator enumerator2 = default;
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                }
                if (ourRadioButton2 == null)
                {
                    ourRadioButton2 = (OurRadioButton)this.PanFilter.Children[0];
                }
                ourRadioButton2.Checked = true;
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
                                    list.AddRange(resourceVersion.ModLoaders.Flags<ModLoaders>().Select<ModLoaders, string>((PageDownloadResourceDetail._Closure___._I17_0 == null) ? (PageDownloadResourceDetail._Closure___._I17_0 = new Func<ModLoaders, string>(PageDownloadResourceDetail._Closure___._I._Lambda___17_0)) : PageDownloadResourceDetail._Closure___._I17_0));
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
                                    List<string>.Enumerator enumerator3 = default;
                                    ((IDisposable)enumerator3).Dispose();
                                }
                            }
                        }
                    }
                    finally
                    {
                        List<string>.Enumerator enumerator2 = default;
                        ((IDisposable)enumerator2).Dispose();
                    }
                }
            }
            finally
            {
                List<ResourceVersion>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            if (Operators.CompareString(text, "", false) != 0 && (this.VersionFilter == null || this.GetGroupedVersionName(this.TargetVersion, this.GroupedDrop, this.GroupedOld).StartsWithF(this.VersionFilter, false)))
            {
                sortedDictionary.Add(text, new List<ResourceVersion>());
                try
                {
                    foreach (ResourceVersion resourceVersion2 in results)
                    {
                        if (resourceVersion2.GameVersions.Contains(this.TargetVersion) && (this.TargetLoaders == ModLoaders.None || this.TargetLoaders.Flags<ModLoaders>().Intersect<ModLoaders>(resourceVersion2.ModLoaders.Flags<ModLoaders>()).Any<ModLoaders>()) && (this.VersionFilter == null || resourceVersion2.GameVersions.Any<string>(new Func<string, bool>(this._Lambda___17_1))) && !sortedDictionary[text].Contains(resourceVersion2))
                        {
                            sortedDictionary[text].Add(resourceVersion2);
                        }
                    }
                }
                finally
                {
                    List<ResourceVersion>.Enumerator enumerator4 = default;
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
                            OurCard ourCard = new OurCard
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
                            ourCard.Children.Add(stackPanel);
                            ourCard.SwapControl = stackPanel;
                            ourCard.SetContentBuilder(new Action<Panel>(this._Lambda___R17_4));
                            this.PanResults.Children.Add(ourCard);
                            if (Operators.CompareString(keyValuePair.Key, text, false) == 0 || (ModMain.FrmMain.PageCurrent.Additional != null && ((List<string>)NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 1 }, null)).Contains(ourCard.Title)))
                            {
                                ourCard.BuildContent();
                            }
                            else
                            {
                                ourCard.IsSwapped = true;
                            }
                            if (Operators.CompareString(keyValuePair.Key, "其他", false) == 0)
                            {
                                stackPanel.Children.Add(new OurHint
                                {
                                    Text = "由于版本信息更新缓慢，可能无法识别刚更新的 MC 版本。几天后即可正常识别。",
                                    Theme = OurHint.Themes.Yellow,
                                    Margin = new Thickness(5.0, 0.0, 0.0, 8.0)
                                });
                            }
                        }
                    }
                }
                finally
                {
                    SortedDictionary<string, List<ResourceVersion>>.Enumerator enumerator5 = default;
                    ((IDisposable)enumerator5).Dispose();
                }
                if (this.PanResults.Children.Count == 1)
                {
                    ((OurCard)this.PanResults.Children[0]).IsSwapped = false;
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
            List<ResourceVersion> list = ((List<ResourceVersion>)Stack.Tag).SortByComparison<ResourceVersion>((PageDownloadResourceDetail._Closure___._I18_0 == null) ? (PageDownloadResourceDetail._Closure___._I18_0 = new Func<ResourceVersion, ResourceVersion, bool>(PageDownloadResourceDetail._Closure___._I._Lambda___18_0)) : PageDownloadResourceDetail._Closure___._I18_0);
            bool flag = list.DistinctBy<ResourceVersion, string>((PageDownloadResourceDetail._Closure___._I18_1 == null) ? (PageDownloadResourceDetail._Closure___._I18_1 = new Func<ResourceVersion, string>(PageDownloadResourceDetail._Closure___._I._Lambda___18_1)) : PageDownloadResourceDetail._Closure___._I18_1).Count<ResourceVersion>() != list.Count;
            List<string> list2;
            if (this.TargetResourceType == ResourceTypes.ModPack)
            {
                list2 = new List<string>();
            }
            else
            {
                list2 = list.SelectMany<ResourceVersion, string>((PageDownloadResourceDetail._Closure___._I18_2 == null) ? (PageDownloadResourceDetail._Closure___._I18_2 = new Func<ResourceVersion, IEnumerable<string>>(PageDownloadResourceDetail._Closure___._I._Lambda___18_2)) : PageDownloadResourceDetail._Closure___._I18_2).Distinct<string>().Ordered<string>()
                    .ToList<string>();
            }
            if (list2.Any<string>())
            {
                list2 = list2.Where<string>((PageDownloadResourceDetail._Closure___._I18_3 == null) ? (PageDownloadResourceDetail._Closure___._I18_3 = new Func<string, bool>(PageDownloadResourceDetail._Closure___._I._Lambda___18_3)) : PageDownloadResourceDetail._Closure___._I18_3).ToList<string>();
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
                        OurVirtualizingElement<OurResourceItem> ourVirtualizingElement = ResourceProject.Cache[text].ToResourceItem(false, false);
                        Stack.Children.Add(ourVirtualizingElement);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator = default;
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
                    Stack.Children.Add(resourceVersion.ToListItem(new OurListItem.ClickEventHandler(this.Save_Click), null, flag));
                }
            }
            finally
            {
                List<ResourceVersion>.Enumerator enumerator2 = default;
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

        public void Install_Click(OurListItem sender, EventArgs e)
        {
            try
            {
                PageDownloadResourceDetail._Closure___22_0 CS_locals1 = new PageDownloadResourceDetail._Closure___22_0();
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
                if (Operators.CompareString(validateFolderName.Check(text2), "", false) != 0)
                {
                    text2 = "";
                }
                CS_locals1._VB_Local_InstanceName = ModMain.OurMsgBoxInput("输入版本名称", "", text2, new Collection<Validate> { validateFolderName }, "", "确定", "取消", false);
                if (!string.IsNullOrEmpty(CS_locals1._VB_Local_InstanceName))
                {
                    List<ModLoader.LoaderBase> list = new List<ModLoader.LoaderBase>();
                    CS_locals1._VB_Local_Target = string.Format("{0}versions\\{1}\\原始整合包.{2}", ModMinecraft.McFolderSelected, CS_locals1._VB_Local_InstanceName, (this.Project.Platform == ResourcePlatforms.CurseForge) ? "zip" : "mrpack");
                    CS_locals1._VB_Local_LogoFileAddress = this.ResourceItem.PathLogo.ActualSource;
                    list.Add(new ModNet.LoaderDownload("下载整合包文件", new ModNet.NetFile[] { resourceVersion.ToNetFile(CS_locals1._VB_Local_Target, new ResourceVersion.DownloadReason?(ResourceVersion.DownloadReason.Standalone), resourceVersion.GameVersions.FirstOrDefault<string>(), resourceVersion.ModLoaders) })
                    {
                        ProgressWeight = 10.0,
                        Block = true
                    });
                    list.Add(new ModLoader.LoaderTask<int, int>("准备安装整合包", new Action<ModLoader.LoaderTask<int, int>>(CS_locals1._Lambda___R6), null, ThreadPriority.Normal)
                    {
                        ProgressWeight = 0.1
                    });
                    ModLoader.LoaderCombo<string> loaderCombo = new ModLoader.LoaderCombo<string>(text, list);
                    loaderCombo.OnStateChanged = ((PageDownloadResourceDetail._Closure___._I22_1 == null) ? (PageDownloadResourceDetail._Closure___._I22_1 = new Action<ModLoader.LoaderBase>(PageDownloadResourceDetail._Closure___._I._Lambda___22_1)) : PageDownloadResourceDetail._Closure___._I22_1);
                    loaderCombo.Start(ModMinecraft.McFolderSelected + "versions\\" + CS_locals1._VB_Local_InstanceName + "\\", false);
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
            PageDownloadResourceDetail._Closure___24_0 CS_locals1 = new PageDownloadResourceDetail._Closure___24_0();
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_File = (ResourceVersion)NewLateBinding.LateGet((sender is OurListItem) ? sender : NewLateBinding.LateGet(sender, null, "Parent", new object[0], null, null, null), null, "Tag", new object[0], null, null, null);
            ModBase.RunInNewThread(new Action(CS_locals1._Lambda___0), "Download Resource Detail Save", ThreadPriority.Normal);
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




        internal virtual OurButton BtnIntroWeb_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnIntroWeb_Click);
                OurButton ourButton = this._BtnIntroWeb;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnIntroWeb = value;
                ourButton = this._BtnIntroWeb;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnIntroWiki_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnIntroWiki_Click);
                OurButton ourButton = this._BtnIntroWiki;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnIntroWiki = value;
                ourButton = this._BtnIntroWiki;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnIntroCopy_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnIntroCopy_Click);
                OurButton ourButton = this._BtnIntroCopy;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnIntroCopy = value;
                ourButton = this._BtnIntroCopy;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurHint HintAlternative_WithEvents
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
                OurHint.ClickEventHandler clickEventHandler = new OurHint.ClickEventHandler(this._Lambda___R59_7);
                OurHint ourHint = this._HintAlternative;
                if (ourHint != null)
                {
                    ourHint.Click -= clickEventHandler;
                }
                this._HintAlternative = value;
                ourHint = this._HintAlternative;
                if (ourHint != null)
                {
                    ourHint.Click += clickEventHandler;
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
                OurLoading.StateChangedEventHandler stateChangedEventHandler = new OurLoading.StateChangedEventHandler(this.Load_State);
                OurLoading ourLoading = this._Load;
                if (ourLoading != null)
                {
                    ourLoading.StateChanged -= stateChangedEventHandler;
                }
                this._Load = value;
                ourLoading = this._Load;
                if (ourLoading != null)
                {
                    ourLoading.StateChanged += stateChangedEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, RoutedEventArgs a1)
        {
            this.LoadPageArguments();
        }

        [CompilerGenerated]
        private void _Lambda___1_0(ModLoader.LoaderTask<int, List<ResourceVersion>> Task)
        {
            this.LoadPageArguments();
            List<ResourceVersion> list = ResourceVersion.FromProjectId(this.Project.Id, this.Project.Platform, true);
            if (!Task.IsCanceled)
            {
                Task.Output = list;
            }
        }

        [CompilerGenerated]
        private void _Lambda___R9_2(ModLoader.LoaderTask<int, List<ResourceVersion>> a0)
        {
            this.Load_OnFinish();
        }

        [CompilerGenerated]
        private bool _Lambda___15_1(ResourceVersion r)
        {
            return r.ResourceType == this.TargetResourceType;
        }

        [CompilerGenerated]
        private string _Lambda___16_2(string v)
        {
            return this.GetGroupedVersionName(v, this.GroupedDrop, this.GroupedOld);
        }

        [CompilerGenerated]
        private void _Lambda___R16_3(object a0, bool a1)
        {
            this._Lambda___16_5((OurRadioButton)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___16_5(OurRadioButton sender, bool raiseByMouse)
        {
            this.VersionFilter = ((Operators.CompareString(sender.Text, "全部", false) == 0) ? null : sender.Text);
            this.UpdateFilterResult();
        }

        [CompilerGenerated]
        private bool _Lambda___16_6(ResourceVersion v)
        {
            return v.GameVersions.Contains(this.TargetVersion);
        }

        [CompilerGenerated]
        private bool _Lambda___17_1(string v)
        {
            return Operators.CompareString(this.GetGroupedVersionName(v, this.GroupedDrop, this.GroupedOld), this.VersionFilter, false) == 0;
        }

        [CompilerGenerated]
        private void _Lambda___R17_4(Panel a0)
        {
            this.InitializeStackResourceVersions((StackPanel)a0);
        }

        [CompilerGenerated]
        private void _Lambda___R18_5(object a0, MouseButtonEventArgs a1)
        {
            this.Install_Click((OurListItem)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R59_7(object a0, MouseButtonEventArgs a1)
        {
            this.HintAlternative_Click();
        }

        private OurResourceItem ResourceItem;

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

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("CardIntro")]
        private OurCard _CardIntro;

        [CompilerGenerated]
        [AccessedThroughProperty("PanIntro")]
        private StackPanel _PanIntro;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnIntroWeb")]
        private OurButton _BtnIntroWeb;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnIntroWiki")]
        private OurButton _BtnIntroWiki;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnIntroCopy")]
        private OurButton _BtnIntroCopy;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("HintAlternative")]
        private OurHint _HintAlternative;

        [CompilerGenerated]
        [AccessedThroughProperty("CardFilter")]
        private OurCard _CardFilter;

        [CompilerGenerated]
        [AccessedThroughProperty("PanFilter")]
        private StackPanel _PanFilter;

        [CompilerGenerated]
        [AccessedThroughProperty("PanResults")]
        private StackPanel _PanResults;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLoad")]
        private OurCard _PanLoad;

        [CompilerGenerated]
        [AccessedThroughProperty("Load")]
        private OurLoading _Load;

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

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal bool _Lambda___15_0(ResourceVersion r)
            {
                return r.ResourceType != ResourceTypes.Plugin;
            }

            internal IEnumerable<string> _Lambda___16_1(ResourceVersion v)
            {
                return v.GameVersions;
            }

            internal string _Lambda___16_3(string s)
            {
                return s;
            }

            internal int _Lambda___16_4(string x, string y)
            {
                return McUtils.CompareVersion(x, y);
            }

            internal string _Lambda___17_0(ModLoaders f)
            {
                return f.ToString() + " ";
            }

            internal bool _Lambda___18_0(ResourceVersion a, ResourceVersion b)
            {
                return DateTime.Compare(a.ReleaseDate, b.ReleaseDate) > 0;
            }

            internal string _Lambda___18_1(ResourceVersion a)
            {
                return a.Display;
            }

            internal IEnumerable<string> _Lambda___18_2(ResourceVersion f)
            {
                return f.Dependencies;
            }

            internal bool _Lambda___18_3(string d)
            {
                if (!ResourceProject.Cache.ContainsKey(d))
                {
                    Logger.Warn(string.Format("未找到 ID {0} 的前置信息", d), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                }
                return ResourceProject.Cache.ContainsKey(d);
            }

            internal void _Lambda___22_1(ModLoader.LoaderBase OurLoader)
            {
                switch (OurLoader.State)
                {
                case ModBase.LoadState.Loading:
                    return;
                case ModBase.LoadState.Failed:
                    ModMain.Hint(OurLoader.Name + "失败：" + OurLoader.Error.GetDisplay(false), ModMain.HintType.Red, true);
                    break;
                case ModBase.LoadState.Canceled:
                    ModMain.Hint(OurLoader.Name + "已取消！", ModMain.HintType.Blue, true);
                    break;
                }
                ModDownloadLib.McInstallFailedClearFolder(OurLoader);
            }

            internal IEnumerable<ModMinecraft.McInstance> _Lambda___24_3(List<ModMinecraft.McInstance> l)
            {
                return l;
            }

            internal DateTime _Lambda___24_6(DirectoryInfo Dir)
            {
                if (!Dir.Exists)
                {
                    return DateTime.MinValue;
                }
                return Dir.LastWriteTimeUtc;
            }

            internal int _Lambda___24_7(DirectoryInfo Dir)
            {
                if (!Dir.Exists)
                {
                    return -1;
                }
                return Dir.GetFiles().Length;
            }

            public static readonly PageDownloadResourceDetail._Closure___ _I = new PageDownloadResourceDetail._Closure___();

            public static Func<ResourceVersion, bool> _I15_0;

            public static Func<ResourceVersion, IEnumerable<string>> _I16_1;

            public static Func<string, string> _I16_3;

            public static Comparison<string> _I16_4;

            public static Func<ModLoaders, string> _I17_0;

            public static Func<ResourceVersion, ResourceVersion, bool> _I18_0;

            public static Func<ResourceVersion, string> _I18_1;

            public static Func<ResourceVersion, IEnumerable<string>> _I18_2;

            public static Func<string, bool> _I18_3;

            public static Action<ModLoader.LoaderBase> _I22_1;

            public static Func<List<ModMinecraft.McInstance>, IEnumerable<ModMinecraft.McInstance>> _I24_3;

            public static Func<DirectoryInfo, DateTime> _I24_6;

            public static Func<DirectoryInfo, int> _I24_7;
        }

        [CompilerGenerated]
        private sealed class _Closure___16_0
        {
            public _Closure___16_0(PageDownloadResourceDetail._Closure___16_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Filters = arg0._VB_Local_Filters;
                    this._VB_Local_Results = arg0._VB_Local_Results;
                }
            }

            internal void _Lambda___0()
            {
                this._VB_Local_Filters = this._VB_Local_Results.SelectMany<ResourceVersion, string>((PageDownloadResourceDetail._Closure___._I16_1 == null) ? (PageDownloadResourceDetail._Closure___._I16_1 = new Func<ResourceVersion, IEnumerable<string>>(PageDownloadResourceDetail._Closure___._I._Lambda___16_1)) : PageDownloadResourceDetail._Closure___._I16_1).Select<string, string>(new Func<string, string>(this._VB_Me._Lambda___16_2)).Distinct<string>()
                    .OrderByDescending<string, string>((PageDownloadResourceDetail._Closure___._I16_3 == null) ? (PageDownloadResourceDetail._Closure___._I16_3 = new Func<string, string>(PageDownloadResourceDetail._Closure___._I._Lambda___16_3)) : PageDownloadResourceDetail._Closure___._I16_3, Comparer<string>.Create((PageDownloadResourceDetail._Closure___._I16_4 == null) ? (PageDownloadResourceDetail._Closure___._I16_4 = new Comparison<string>(PageDownloadResourceDetail._Closure___._I._Lambda___16_4)) : PageDownloadResourceDetail._Closure___._I16_4))
                    .ToList<string>();
            }

            public List<string> _VB_Local_Filters;

            public List<ResourceVersion> _VB_Local_Results;

            public PageDownloadResourceDetail _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___22_0
        {
            internal void _Lambda___R6(ModLoader.LoaderTask<int, int> a0)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                ModModpack.ModpackInstall(this._VB_Local_Target, this._VB_Local_InstanceName, FileUtils.Exists(this._VB_Local_LogoFileAddress) ? this._VB_Local_LogoFileAddress : null);
            }

            public string _VB_Local_Target;

            public string _VB_Local_InstanceName;

            public string _VB_Local_LogoFileAddress;
        }

        [CompilerGenerated]
        private sealed class _Closure___24_0
        {
            internal void _Lambda___0()
            {
                try
                {
                    PageDownloadResourceDetail._Closure___24_3 CS_locals1 = new PageDownloadResourceDetail._Closure___24_3();
                    CS_locals1._VB_NonLocal__VB_Closure_2 = this;
                    CS_locals1._VB_Local_Desc = null;
                    ResourceTypes resourceType = this._VB_Local_File.ResourceType;
                    switch (resourceType)
                    {
                    case ResourceTypes.Mod:
                        CS_locals1._VB_Local_Desc = "Mod ";
                        break;
                    case ResourceTypes.ModPack:
                        CS_locals1._VB_Local_Desc = "整合包";
                        break;
                    case ResourceTypes.Mod | ResourceTypes.ModPack:
                        break;
                    case ResourceTypes.ResourcePack:
                        CS_locals1._VB_Local_Desc = "资源包";
                        break;
                    default:
                        if (resourceType != ResourceTypes.Shader)
                        {
                            if (resourceType == ResourceTypes.DataPack)
                            {
                                CS_locals1._VB_Local_Desc = "数据包";
                            }
                        }
                        else
                        {
                            CS_locals1._VB_Local_Desc = "光影包";
                        }
                        break;
                    }
                    CS_locals1._VB_Local_DefaultFolder = null;
                    if (this._VB_Local_File.ResourceType != ResourceTypes.ModPack)
                    {
                        PageDownloadResourceDetail._Closure___24_2 CS_locals2 = new PageDownloadResourceDetail._Closure___24_2();
                        CS_locals2._VB_NonLocal__VB_Closure_3 = CS_locals1;
                        CS_locals2._VB_Local_SubFolder = null;
                        ResourceTypes resourceType2 = this._VB_Local_File.ResourceType;
                        if (resourceType2 <= ResourceTypes.ResourcePack)
                        {
                            if (resourceType2 != ResourceTypes.Mod)
                            {
                                if (resourceType2 == ResourceTypes.ResourcePack)
                                {
                                    CS_locals2._VB_Local_SubFolder = "resourcepacks\\";
                                }
                            }
                            else
                            {
                                CS_locals2._VB_Local_SubFolder = "mods\\";
                            }
                        }
                        else if (resourceType2 != ResourceTypes.Shader)
                        {
                            if (resourceType2 == ResourceTypes.DataPack)
                            {
                                CS_locals2._VB_Local_SubFolder = "";
                            }
                        }
                        else
                        {
                            CS_locals2._VB_Local_SubFolder = "shaderpacks\\";
                        }
                        CS_locals2._VB_Local_IsInstanceSuitable = null;
                        CS_locals2._VB_Local_AllowedLoaders = ModLoaders.None;
                        if (this._VB_Local_File.ModLoaders != ModLoaders.None)
                        {
                            CS_locals2._VB_Local_AllowedLoaders = this._VB_Local_File.ModLoaders;
                        }
                        else if (this._VB_Me.Project.ModLoaders != ModLoaders.None)
                        {
                            CS_locals2._VB_Local_AllowedLoaders = this._VB_Me.Project.ModLoaders;
                        }
                        Logger.Info(string.Format("{0}要求的加载器种类：{1}", CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_Desc, CS_locals2._VB_Local_AllowedLoaders), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                        CS_locals2._VB_Local_IsInstanceSuitable = new Func<ModMinecraft.McInstance, bool>(CS_locals2._Lambda___1);
                        if (PageDownloadResourceDetail.CachedFolder.ContainsKey(this._VB_Local_File.ResourceType))
                        {
                            CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder = PageDownloadResourceDetail.CachedFolder[this._VB_Local_File.ResourceType];
                            Logger.Info(string.Format("使用上次下载时的文件夹作为默认下载位置：{0}", CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                        }
                        else if (((object)(ModMinecraft.McInstanceSelected) != (object)null) && CS_locals2._VB_Local_IsInstanceSuitable(ModMinecraft.McInstanceSelected))
                        {
                            CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder = string.Format("{0}{1}", ModMinecraft.McInstanceSelected.PathIndie, CS_locals2._VB_Local_SubFolder);
                            DirectoryUtils.Create(CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder);
                            Logger.Info(string.Format("使用当前版本作为默认下载位置：{0}", CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                        }
                        else
                        {
                            bool flag = ModMinecraft.McInstanceListLoader.State != ModBase.LoadState.Finished;
                            if (flag)
                            {
                                ModMain.Hint("正在查找适合的游戏版本……", ModMain.HintType.Blue, true);
                                ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", true);
                            }
                            IEnumerable<DirectoryInfo> enumerable = ModMinecraft.McInstanceList.Values.SelectMany<List<ModMinecraft.McInstance>, ModMinecraft.McInstance>((PageDownloadResourceDetail._Closure___._I24_3 == null) ? (PageDownloadResourceDetail._Closure___._I24_3 = new Func<List<ModMinecraft.McInstance>, IEnumerable<ModMinecraft.McInstance>>(PageDownloadResourceDetail._Closure___._I._Lambda___24_3)) : PageDownloadResourceDetail._Closure___._I24_3).Where<ModMinecraft.McInstance>(new Func<ModMinecraft.McInstance, bool>(CS_locals2._Lambda___4)).Select<ModMinecraft.McInstance, DirectoryInfo>(new Func<ModMinecraft.McInstance, DirectoryInfo>(CS_locals2._Lambda___5));
                            if (enumerable.Any<DirectoryInfo>())
                            {
                                DirectoryInfo directoryInfo = enumerable.OrderByDescending<DirectoryInfo, DateTime>((PageDownloadResourceDetail._Closure___._I24_6 == null) ? (PageDownloadResourceDetail._Closure___._I24_6 = new Func<DirectoryInfo, DateTime>(PageDownloadResourceDetail._Closure___._I._Lambda___24_6)) : PageDownloadResourceDetail._Closure___._I24_6).ThenByDescending<DirectoryInfo, int>((PageDownloadResourceDetail._Closure___._I24_7 == null) ? (PageDownloadResourceDetail._Closure___._I24_7 = new Func<DirectoryInfo, int>(PageDownloadResourceDetail._Closure___._I._Lambda___24_7)) : PageDownloadResourceDetail._Closure___._I24_7).First<DirectoryInfo>();
                                CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder = PathUtils.RemoveExtendedPrefix(directoryInfo.FullName);
                                DirectoryUtils.Create(CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder);
                                Logger.Info(string.Format("使用适合的游戏版本作为默认下载位置：{0}", CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                            }
                            else
                            {
                                CS_locals2._VB_NonLocal__VB_Closure_3._VB_Local_DefaultFolder = ModMinecraft.McFolderSelected;
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
                    if (Operators.CompareString(this._VB_Me.Project.TranslatedName, this._VB_Me.Project.RawName, false) == 0)
                    {
                        CS_locals1._VB_Local_FileName = this._VB_Local_File.FileName;
                    }
                    else
                    {
                        string text = this._VB_Me.Project.TranslatedName.BeforeFirst(" (", false).BeforeFirst(" - ", false).Replace("\\", "＼")
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
                            CS_locals1._VB_Local_FileName = string.Format("【{0}】{1}", text, this._VB_Local_File.FileName);
                            break;
                        case 1:
                            CS_locals1._VB_Local_FileName = string.Format("[{0}] {1}", text, this._VB_Local_File.FileName);
                            break;
                        case 2:
                            CS_locals1._VB_Local_FileName = string.Format("{0}-{1}", text, this._VB_Local_File.FileName);
                            break;
                        case 3:
                            CS_locals1._VB_Local_FileName = string.Format("{0}-{1}", this._VB_Local_File.FileName, text);
                            break;
                        default:
                            CS_locals1._VB_Local_FileName = this._VB_Local_File.FileName;
                            break;
                        }
                    }
                    if (this._VB_Local_File.ResourceType == ResourceTypes.Mod)
                    {
                        CS_locals1._VB_Local_FileName = CS_locals1._VB_Local_FileName.Replace("~", "-");
                    }
                    ModBase.RunInUi(new Action(CS_locals1._Lambda___8), false);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "保存资源文件失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageDownloadCompDetail.xaml.vb");
                }
            }

            public ResourceVersion _VB_Local_File;

            public PageDownloadResourceDetail _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___24_1
        {
            internal bool _Lambda___2(string FileVersion)
            {
                bool flag = FileVersion.Contains("预览版") || !FileVersion.Contains(".");
                bool flag2 = this._VB_Local_Instance.Version.VanillaName.Contains("snapshot") || !this._VB_Local_Instance.Version.VanillaName.Contains(".");
                bool flag3;
                if (flag != flag2)
                {
                    flag3 = false;
                }
                else if (!flag && !flag2)
                {
                    flag3 = Operators.CompareString(FileVersion, this._VB_Local_Instance.Version.VanillaName, false) == 0;
                }
                else
                {
                    bool flag4 = FileVersion.Contains(".") && ModBase.Val(FileVersion.BeforeFirst(".", false)) > 1.0;
                    bool flag5 = this._VB_Local_Instance.Version.Drop > 250;
                    if (flag4 != flag5)
                    {
                        flag3 = false;
                    }
                    else if (flag4 && flag5)
                    {
                        flag3 = Operators.CompareString(FileVersion.BeforeFirst(" ", false), this._VB_Local_Instance.Version.VanillaName.BeforeFirst("-", false), false) == 0;
                    }
                    else
                    {
                        flag3 = !FileVersion.Contains("w") || !this._VB_Local_Instance.Version.VanillaName.Contains("w") || Operators.CompareString(FileVersion, this._VB_Local_Instance.Version.VanillaName, false) == 0;
                    }
                }
                return flag3;
            }

            public ModMinecraft.McInstance _VB_Local_Instance;
        }

        [CompilerGenerated]
        private sealed class _Closure___24_2
        {
            internal bool _Lambda___1(ModMinecraft.McInstance Instance)
            {
                PageDownloadResourceDetail._Closure___24_1 CS_locals1 = new PageDownloadResourceDetail._Closure___24_1();
                CS_locals1._VB_Local_Instance = Instance;
                if (!CS_locals1._VB_Local_Instance.IsLoaded)
                {
                    CS_locals1._VB_Local_Instance.Load();
                }
                return ((this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_File.ResourceType != ResourceTypes.Mod && this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_File.ResourceType != ResourceTypes.DataPack) || this._VB_NonLocal__VB_Closure_3._VB_NonLocal__VB_Closure_2._VB_Local_File.GameVersions.Any<string>(new Func<string, bool>(CS_locals1._Lambda___2))) && (this._VB_Local_AllowedLoaders == ModLoaders.None || this._VB_Local_AllowedLoaders.Flags<ModLoaders>().Intersect<ModLoaders>(CS_locals1._VB_Local_Instance.Version.ModLoaders.Flags<ModLoaders>()).Any<ModLoaders>());
            }

            internal bool _Lambda___4(ModMinecraft.McInstance v)
            {
                return this._VB_Local_IsInstanceSuitable(v);
            }

            internal DirectoryInfo _Lambda___5(ModMinecraft.McInstance v)
            {
                return DirectoryUtils.GetInfo(string.Format("{0}{1}", v.PathIndie, this._VB_Local_SubFolder));
            }

            public ModLoaders _VB_Local_AllowedLoaders;

            public Func<ModMinecraft.McInstance, bool> _VB_Local_IsInstanceSuitable;

            public string _VB_Local_SubFolder;

            public PageDownloadResourceDetail._Closure___24_3 _VB_NonLocal__VB_Closure_3;
        }

        [CompilerGenerated]
        private sealed class _Closure___24_3
        {
            internal void _Lambda___8()
            {
                PageDownloadResourceDetail._Closure___24_4 CS_locals1 = new PageDownloadResourceDetail._Closure___24_4();
                CS_locals1._VB_NonLocal__VB_Closure_4 = this;
                string text = ((this._VB_NonLocal__VB_Closure_2._VB_Local_File.ResourceType == ResourceTypes.Mod) ? ((Operators.CompareString(PathUtils.GetExtension(this._VB_NonLocal__VB_Closure_2._VB_Local_File.FileName), "litemod", false) == 0) ? "litemod" : "jar") : ((Operators.CompareString(PathUtils.GetExtension(this._VB_NonLocal__VB_Closure_2._VB_Local_File.FileName), "mrpack", false) == 0) ? "mrpack" : "zip"));
                CS_locals1._VB_Local_Target = Dialogs.SaveFile("选择保存位置", this._VB_Local_FileName, this._VB_Local_DefaultFolder, new ValueTuple<string, string>[]
                {
                    new ValueTuple<string, string>(text, this._VB_Local_Desc + "文件")
                });
                if (CS_locals1._VB_Local_Target != null)
                {
                    string text2 = this._VB_Local_Desc + "下载：" + PathUtils.GetFileNameWithoutExtension(CS_locals1._VB_Local_Target) + " ";
                    if (!PathUtils.IsSame(PathUtils.RemoveLastPart(CS_locals1._VB_Local_Target), this._VB_Local_DefaultFolder))
                    {
                        PageDownloadResourceDetail.CachedFolder[this._VB_NonLocal__VB_Closure_2._VB_Local_File.ResourceType] = PathUtils.RemoveLastPart(CS_locals1._VB_Local_Target);
                    }
                    ModLoader.LoaderCombo<int> loaderCombo = new ModLoader.LoaderCombo<int>(text2, new List<ModLoader.LoaderBase>
                    {
                        new ModNet.LoaderDownload("下载文件", new ModNet.NetFile[] { this._VB_NonLocal__VB_Closure_2._VB_Local_File.ToNetFile(CS_locals1._VB_Local_Target, new ResourceVersion.DownloadReason?((this._VB_NonLocal__VB_Closure_2._VB_Me.TargetResourceType == ResourceTypes.Any) ? ResourceVersion.DownloadReason.Dependency : ResourceVersion.DownloadReason.Standalone), null, ModLoaders.None) })
                        {
                            ProgressWeight = 6.0,
                            Block = true
                        }
                    });
                    loaderCombo.OnStateChanged = new Action<ModLoader.LoaderBase>(CS_locals1._Lambda___9);
                    loaderCombo.Start(1, false);
                    ModLoader.LoaderTaskbarAdd<int>(loaderCombo);
                    ModMain.FrmMain.BtnExtraDownload.ShowRefresh();
                    ModMain.FrmMain.BtnExtraDownload.Ribble();
                }
            }

            public string _VB_Local_FileName;

            public string _VB_Local_DefaultFolder;

            public string _VB_Local_Desc;

            public PageDownloadResourceDetail._Closure___24_0 _VB_NonLocal__VB_Closure_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___24_4
        {
            internal void _Lambda___9(ModLoader.LoaderBase OurLoader)
            {
                ModDownloadLib.LoaderStateChangedHintOnly(OurLoader);
                if (OurLoader.State == ModBase.LoadState.Finished && this._VB_NonLocal__VB_Closure_4._VB_NonLocal__VB_Closure_2._VB_Local_File.ResourceType == ResourceTypes.ModPack)
                {
                    ModBase.OpenExplorer(this._VB_Local_Target);
                }
            }

            public string _VB_Local_Target;

            public PageDownloadResourceDetail._Closure___24_3 _VB_NonLocal__VB_Closure_4;
        }
    }
}
