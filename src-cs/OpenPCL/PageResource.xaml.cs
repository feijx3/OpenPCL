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
using Microsoft.VisualBasic.CompilerServices;
using PCLCS;

namespace PCL
{
    [ContentProperty("SearchTags")]
    [DesignerGenerated]
    public partial class PageResource : OurScrollViewer
    {
        public PageResource()
        {
            base.Loaded += new RoutedEventHandler(this.PageResource_Loaded);
            this._TypeName = "";
            this._TypeNameSpaced = "";
            this._Type = (ResourceTypes)(-1);
            this.Loader = ResourceSearcher.GetLoader(new Func<ResourceSearcher.SearchRequest>(this.LoaderInput));
            this.Storage = new ResourceSearcher.SearchResult();
            this.Page = 0;
            this.InitializeComponent();
            this.TextSearchName_WithEvents = this.TextSearchName;
            this.TextSearchVersion_WithEvents = this.TextSearchVersion;
            this.BtnSearchRun_WithEvents = this.BtnSearchRun;
            this.BtnSearchReset_WithEvents = this.BtnSearchReset;
            this.BtnSearchInstallModPack_WithEvents = this.BtnSearchInstallModPack;
            this.BtnPageFirst_WithEvents = this.BtnPageFirst;
            this.BtnPageLeft_WithEvents = this.BtnPageLeft;
            this.BtnPageRight_WithEvents = this.BtnPageRight;
            this.Load_WithEvents = this.Load;
        }

        public ItemCollection SearchTags
        {
            get
            {
                return this.ComboSearchTag.Items;
            }
        }

        public string TypeName
        {
            get
            {
                return this._TypeName;
            }
            set
            {
                if (Operators.CompareString(this._TypeName, value, false) != 0)
                {
                    this._TypeName = value;
                    this.Loader.Name = string.Format("社区资源获取：{0}", value);
                }
            }
        }

        public string TypeNameSpaced
        {
            get
            {
                return this._TypeNameSpaced;
            }
            set
            {
                if (Operators.CompareString(this._TypeNameSpaced, value, false) != 0)
                {
                    this._TypeNameSpaced = value;
                    this.PanAlways.Title = string.Format("搜索{0}", value);
                    this.Load.Text = string.Format("正在获取{0}列表", value);
                }
            }
        }

        public ResourceTypes PageType
        {
            get
            {
                return this._Type;
            }
            set
            {
                if (this._Type != value)
                {
                    this._Type = value;
                    this.BtnSearchInstallModPack.Visibility = ((value == ResourceTypes.ModPack) ? Visibility.Visible : Visibility.Collapsed);
                }
            }
        }

        private void PageResource_Loaded(object sender, EventArgs e)
        {
            if (this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init, ref flag);
                if (this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init.State == 0)
                {
                    this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init.State = 2;
                    this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded = false;
                }
                else if (this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init);
                }
            }
            if (((object)(PageResource.TargetInstance) != (object)null))
            {
                this.ResetFilter();
                this.TextSearchVersion.Text = PageResource.TargetInstance.Version.VanillaName;
                VB_AnonymousDelegate_12<string, OurComboBoxItem> vb_AnonymousDelegate_ = new VB_AnonymousDelegate_12<string, OurComboBoxItem>(this._Lambda___19_0);
                if (PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                {
                    this.ComboSearchLoader.SelectedItem = vb_AnonymousDelegate_("Forge");
                }
                else if (PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                {
                    this.ComboSearchLoader.SelectedItem = vb_AnonymousDelegate_("Fabric");
                }
                else if (PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                {
                    this.ComboSearchLoader.SelectedItem = vb_AnonymousDelegate_("NeoForge");
                }
                PageResource.TargetInstance = null;
                if (PageResource.TargetName != null)
                {
                    this.TextSearchName.Text = PageResource.TargetName;
                    PageResource.TargetName = null;
                }
                if (this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded)
                {
                    this.StartNewSearch();
                }
                base.ScrollToHome();
            }
            if (!this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded)
            {
                this._STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded = true;
                ((OurPageRight)base.Parent).PageLoaderInit<ResourceSearcher.SearchRequest, int>(this.Load, this.PanLoad, this.PanContent, this.PanAlways, this.Loader, new Action<ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int>>(this._Lambda___R19_1), new Func<ResourceSearcher.SearchRequest>(this.LoaderInput), true);
                if (ModDownload.AllDrops != null && ModDownload.AllDrops.First<int>() > 250)
                {
                    string text = McVersion.DropToVersion(ModDownload.AllDrops.First<int>());
                    if (Operators.CompareString(((OurComboBoxItem)this.TextSearchVersion.Items[1]).Content.ToString(), text, false) != 0)
                    {
                        this.TextSearchVersion.Items.Insert(1, new OurComboBoxItem
                        {
                            Content = text
                        });
                    }
                }
            }
        }

        private ResourceSearcher.SearchRequest LoaderInput()
        {
            checked
            {
                ResourceSearcher.SearchRequest searchRequest = new ResourceSearcher.SearchRequest(this.PageType, this.Storage, (this.Page + 1) * 40);
                string text = ((Operators.CompareString(this.TextSearchVersion.Text, "全部 (也可自行输入)", false) == 0) ? null : ((this.TextSearchVersion.Text.Contains(".") || this.TextSearchVersion.Text.Contains("w")) ? this.TextSearchVersion.Text : null));
                searchRequest.SearchText = this.TextSearchName.Text;
                searchRequest.GameVersion = text;
                searchRequest.Tag = Conversions.ToString(NewLateBinding.LateGet(this.ComboSearchTag.SelectedItem, null, "Tag", new object[0], null, null, null));
                searchRequest.ModLoaders = (ModLoaders)Math.Round((this.PageType == ResourceTypes.Mod) ? ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(this.ComboSearchLoader.SelectedItem, null, "Tag", new object[0], null, null, null))) : 0.0);
                searchRequest.Sources = (ResourcePlatforms)Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(this.ComboSearchSource.SelectedItem, null, "Tag", new object[0], null, null, null))));
                return searchRequest;
            }
        }

        private void Load_OnFinish()
        {
            checked
            {
                try
                {
                    Logger.Info(string.Format("开始可视化{0}列表，已储藏 {1} 个结果，当前在第 {2} 页", this.TypeNameSpaced, this.Storage.Results.Count, this.Page + 1), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageResource.xaml.vb");
                    this.PanProjects.Children.Clear();
                    int num = Math.Min(this.Page * 40, this.Storage.Results.Count - 1);
                    try
                    {
                        foreach (ResourceProject resourceProject in this.Storage.Results.GetRange(num, Math.Min(this.Storage.Results.Count - num, 40)))
                        {
                            this.PanProjects.Children.Add(resourceProject.ToResourceItem(this.Loader.Input.GameVersion == null, this.Loader.Input.ModLoaders == ModLoaders.None && (this.PageType == ResourceTypes.Mod || this.PageType == ResourceTypes.ModPack)));
                        }
                    }
                    finally
                    {
                        List<ResourceProject>.Enumerator enumerator = default;
                        ((IDisposable)enumerator).Dispose();
                    }
                    this.CardPages.Visibility = ((this.Storage.Results.Count > 40 || this.Storage.CurseForgeOffset < this.Storage.CurseForgeTotal || this.Storage.ModrinthOffset < this.Storage.ModrinthTotal) ? Visibility.Visible : Visibility.Collapsed);
                    this.LabPage.Text = Conversions.ToString(this.Page + 1);
                    this.BtnPageFirst.IsEnabled = this.Page > 1;
                    this.BtnPageFirst.Opacity = ((this.Page > 1) ? 1.0 : 0.2);
                    this.BtnPageLeft.IsEnabled = this.Page > 0;
                    this.BtnPageLeft.Opacity = ((this.Page > 0) ? 1.0 : 0.2);
                    bool flag = this.Storage.Results.Count > 40 * (this.Page + 1) || this.Storage.CurseForgeOffset < this.Storage.CurseForgeTotal || this.Storage.ModrinthOffset < this.Storage.ModrinthTotal;
                    this.BtnPageRight.IsEnabled = flag;
                    this.BtnPageRight.Opacity = (flag ? 1.0 : 0.2);
                    if (this.Storage.ErrorMessage == null)
                    {
                        this.HintError.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.HintError.Visibility = Visibility.Visible;
                        this.HintError.Text = this.Storage.ErrorMessage;
                    }
                    base.ScrollToTop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("可视化{0}列表出错", this.TypeNameSpaced), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageResource.xaml.vb");
                }
            }
        }

        private void Load_State(object sender, OurLoading.OurLoadingState state, OurLoading.OurLoadingState oldState)
        {
            ModBase.LoadState state2 = this.Loader.State;
            if (state2 == ModBase.LoadState.Failed)
            {
                string text = "";
                if (this.Loader.Error != null)
                {
                    text = this.Loader.Error.Message;
                }
                if (text.Contains("不是有效的 json 文件"))
                {
                    Logger.Warn(string.Format("下载的{0}列表 json 文件损坏，已自动重试", this.TypeNameSpaced), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageResource.xaml.vb");
                    ((OurPageRight)base.Parent).PageLoaderRestart(null, true);
                }
            }
        }

        private void BtnPageFirst_Click(object sender, RoutedEventArgs e)
        {
            this.ChangePage(0);
        }

        private void BtnPageLeft_Click(object sender, RoutedEventArgs e)
        {
            this.ChangePage(checked(this.Page - 1));
        }

        private void BtnPageRight_Click(object sender, RoutedEventArgs e)
        {
            this.ChangePage(checked(this.Page + 1));
        }

        private void ChangePage(int NewPage)
        {
            this.CardPages.IsEnabled = false;
            this.Page = NewPage;
            ModMain.FrmMain.BackToTop();
            Logger.Info(string.Format("{0}：切换到第 {1} 页", this.TypeName, checked(this.Page + 1)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\PageResource.xaml.vb");
            ModBase.RunInThread(new Action(this._Lambda___29_0));
        }

        private void StartNewSearch()
        {
            this.Page = 0;
            ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> loader = this.Loader;
            ResourceSearcher.SearchRequest searchRequest = this.LoaderInput();
            if (loader.ShouldStart(ref searchRequest, false, false))
            {
                this.Storage = new ResourceSearcher.SearchResult();
            }
            this.Loader.Start(null, false);
        }

        private void EnterTrigger(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.StartNewSearch();
            }
        }

        private void ResetFilter()
        {
            this.TextSearchName.Text = "";
            this.TextSearchVersion.Text = "全部 (也可自行输入)";
            this.TextSearchVersion.SelectedIndex = 0;
            this.ComboSearchSource.SelectedIndex = 0;
            this.ComboSearchTag.SelectedIndex = 0;
            this.ComboSearchLoader.SelectedIndex = 0;
            this.Loader.LastFinishedTime = 0L;
        }

        private void TextSearchVersion_TextChanged()
        {
            if (!this.TextSearchVersion.IsDropDownOpen)
            {
                this.UpdateSearchLoaderVisibility();
            }
        }

        private void UpdateSearchLoaderVisibility()
        {
            if (this.PageType == ResourceTypes.Mod && (this.TextSearchVersion.Text.Contains(".") || this.TextSearchVersion.Text.Contains("w")))
            {
                this.ComboSearchLoader.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(this.TextSearchVersion, 1);
                return;
            }
            this.ComboSearchLoader.Visibility = Visibility.Collapsed;
            Grid.SetColumnSpan(this.TextSearchVersion, 2);
            this.ComboSearchLoader.SelectedIndex = 0;
        }

        private void BtnSearchInstallModPack_Click(object sender, EventArgs e)
        {
            ModModpack.ModpackInstall();
        }


        internal virtual OurTextBox TextSearchName_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextSearchName;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.EnterTrigger);
                OurTextBox ourTextBox = this._TextSearchName;
                if (ourTextBox != null)
                {
                    ourTextBox.KeyDown -= keyEventHandler;
                }
                this._TextSearchName = value;
                ourTextBox = this._TextSearchName;
                if (ourTextBox != null)
                {
                    ourTextBox.KeyDown += keyEventHandler;
                }
            }
        }


        internal virtual OurComboBox TextSearchVersion_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextSearchVersion;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.EnterTrigger);
                OurComboBox.TextChangedEventHandler textChangedEventHandler = new OurComboBox.TextChangedEventHandler(this._Lambda___R50_2);
                EventHandler eventHandler = new EventHandler(this._Lambda___R50_3);
                OurComboBox ourComboBox = this._TextSearchVersion;
                if (ourComboBox != null)
                {
                    ourComboBox.KeyDown -= keyEventHandler;
                    ourComboBox.TextChanged -= textChangedEventHandler;
                    ourComboBox.DropDownClosed -= eventHandler;
                }
                this._TextSearchVersion = value;
                ourComboBox = this._TextSearchVersion;
                if (ourComboBox != null)
                {
                    ourComboBox.KeyDown += keyEventHandler;
                    ourComboBox.TextChanged += textChangedEventHandler;
                    ourComboBox.DropDownClosed += eventHandler;
                }
            }
        }



        internal virtual OurButton BtnSearchRun_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSearchRun;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R62_4);
                OurButton ourButton = this._BtnSearchRun;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSearchRun = value;
                ourButton = this._BtnSearchRun;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnSearchReset_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSearchReset;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R66_5);
                OurButton ourButton = this._BtnSearchReset;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSearchReset = value;
                ourButton = this._BtnSearchReset;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnSearchInstallModPack_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSearchInstallModPack;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSearchInstallModPack_Click);
                OurButton ourButton = this._BtnSearchInstallModPack;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSearchInstallModPack = value;
                ourButton = this._BtnSearchInstallModPack;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }






        internal virtual OurIconButton BtnPageFirst_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnPageFirst;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this._Lambda___R94_6);
                OurIconButton ourIconButton = this._BtnPageFirst;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnPageFirst = value;
                ourIconButton = this._BtnPageFirst;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurIconButton BtnPageLeft_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnPageLeft;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this._Lambda___R98_7);
                OurIconButton ourIconButton = this._BtnPageLeft;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnPageLeft = value;
                ourIconButton = this._BtnPageLeft;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurIconButton BtnPageRight_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnPageRight;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this._Lambda___R106_8);
                OurIconButton ourIconButton = this._BtnPageRight;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnPageRight = value;
                ourIconButton = this._BtnPageRight;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
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
        private OurComboBoxItem _Lambda___19_0(string Name)
        {
            try
            {
                foreach (object obj in ((IEnumerable)this.ComboSearchLoader.Items))
                {
                    OurComboBoxItem ourComboBoxItem = (OurComboBoxItem)obj;
                    if (Operators.ConditionalCompareObjectEqual(ourComboBoxItem.Content, Name, false))
                    {
                        return ourComboBoxItem;
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
            return (OurComboBoxItem)this.ComboSearchLoader.Items[0];
        }

        [CompilerGenerated]
        private void _Lambda___R19_1(ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> a0)
        {
            this.Load_OnFinish();
        }

        [CompilerGenerated]
        private void _Lambda___29_0()
        {
            Thread.Sleep(100);
            ModBase.RunInUi(new Action(this._Lambda___29_1), false);
            this.Loader.Start(null, false);
        }

        [CompilerGenerated]
        private void _Lambda___29_1()
        {
            this.CardPages.IsEnabled = true;
        }

        [CompilerGenerated]
        private void _Lambda___R50_2(object a0, TextChangedEventArgs a1)
        {
            this.TextSearchVersion_TextChanged();
        }

        [CompilerGenerated]
        private void _Lambda___R50_3(object a0, EventArgs a1)
        {
            this.UpdateSearchLoaderVisibility();
        }

        [CompilerGenerated]
        private void _Lambda___R62_4(object a0, MouseButtonEventArgs a1)
        {
            this.StartNewSearch();
        }

        [CompilerGenerated]
        private void _Lambda___R66_5(object a0, MouseButtonEventArgs a1)
        {
            this.ResetFilter();
        }

        [CompilerGenerated]
        private void _Lambda___R94_6(object a0, EventArgs a1)
        {
            this.BtnPageFirst_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
        }

        [CompilerGenerated]
        private void _Lambda___R98_7(object a0, EventArgs a1)
        {
            this.BtnPageLeft_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
        }

        [CompilerGenerated]
        private void _Lambda___R106_8(object a0, EventArgs a1)
        {
            this.BtnPageRight_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
        }

        private string _TypeName;

        private string _TypeNameSpaced;

        private ResourceTypes _Type;

        public static ModMinecraft.McInstance TargetInstance = null;

        public static string TargetName = null;

        public ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> Loader;

        public ResourceSearcher.SearchResult Storage;

        public const int PAGE_SIZE = 40;

        public int Page;

        [CompilerGenerated]
        [AccessedThroughProperty("PanAlways")]
        private OurCard _PanAlways;

        [CompilerGenerated]
        [AccessedThroughProperty("TextSearchName")]
        private OurTextBox _TextSearchName;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboSearchSource")]
        private OurComboBox _ComboSearchSource;

        [CompilerGenerated]
        [AccessedThroughProperty("TextSearchVersion")]
        private OurComboBox _TextSearchVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboSearchLoader")]
        private OurComboBox _ComboSearchLoader;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboSearchTag")]
        private OurComboBox _ComboSearchTag;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSearchRun")]
        private OurButton _BtnSearchRun;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSearchReset")]
        private OurButton _BtnSearchReset;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSearchInstallModPack")]
        private OurButton _BtnSearchInstallModPack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanContent")]
        private StackPanel _PanContent;

        [CompilerGenerated]
        [AccessedThroughProperty("HintError")]
        private OurHint _HintError;

        [CompilerGenerated]
        [AccessedThroughProperty("CardProjects")]
        private OurCard _CardProjects;

        [CompilerGenerated]
        [AccessedThroughProperty("PanProjects")]
        private StackPanel _PanProjects;

        [CompilerGenerated]
        [AccessedThroughProperty("CardPages")]
        private OurCard _CardPages;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnPageFirst")]
        private OurIconButton _BtnPageFirst;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnPageLeft")]
        private OurIconButton _BtnPageLeft;

        [CompilerGenerated]
        [AccessedThroughProperty("LabPage")]
        private TextBlock _LabPage;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnPageRight")]
        private OurIconButton _BtnPageRight;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLoad")]
        private OurCard _PanLoad;

        [CompilerGenerated]
        [AccessedThroughProperty("Load")]
        private OurLoading _Load;

        private bool _STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded;

        private StaticLocalInitFlag _STATIC_PageResource_Loaded_20211C128119_IsFirstLoaded_Init;
    }
}
