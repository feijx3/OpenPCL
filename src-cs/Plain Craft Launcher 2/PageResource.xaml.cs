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
    public partial class PageResource : MyScrollViewer
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
            if (this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init, ref flag);
                if (this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State == 0)
                {
                    this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State = 2;
                    this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded = false;
                }
                else if (this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init);
                }
            }
            if (PageResource.TargetInstance != null)
            {
                this.ResetFilter();
                this.TextSearchVersion.Text = PageResource.TargetInstance.Version.VanillaName;
                VB$AnonymousDelegate_12<string, MyComboBoxItem> vb$AnonymousDelegate_ = delegate(string Name)
                {
                    try
                    {
                        foreach (object obj in ((IEnumerable)this.ComboSearchLoader.Items))
                        {
                            MyComboBoxItem myComboBoxItem = (MyComboBoxItem)obj;
                            if (Operators.ConditionalCompareObjectEqual(myComboBoxItem.Content, Name, false))
                            {
                                return myComboBoxItem;
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
                    return (MyComboBoxItem)this.ComboSearchLoader.Items[0];
                };
                if (PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Forge))
                {
                    this.ComboSearchLoader.SelectedItem = vb$AnonymousDelegate_("Forge");
                }
                else if (PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.Fabric))
                {
                    this.ComboSearchLoader.SelectedItem = vb$AnonymousDelegate_("Fabric");
                }
                else if (PageResource.TargetInstance.Version.ModLoaders.HasFlagF(ModLoaders.NeoForge))
                {
                    this.ComboSearchLoader.SelectedItem = vb$AnonymousDelegate_("NeoForge");
                }
                PageResource.TargetInstance = null;
                if (PageResource.TargetName != null)
                {
                    this.TextSearchName.Text = PageResource.TargetName;
                    PageResource.TargetName = null;
                }
                if (this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded)
                {
                    this.StartNewSearch();
                }
                base.ScrollToHome();
            }
            if (!this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded)
            {
                this.$STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded = true;
                ((MyPageRight)base.Parent).PageLoaderInit<ResourceSearcher.SearchRequest, int>(this.Load, this.PanLoad, this.PanContent, this.PanAlways, this.Loader, delegate(ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> a0)
                {
                    this.Load_OnFinish();
                }, new Func<ResourceSearcher.SearchRequest>(this.LoaderInput), true);
                if (ModDownload.AllDrops != null && ModDownload.AllDrops.First<int>() > 250)
                {
                    string text = McVersion.DropToVersion(ModDownload.AllDrops.First<int>());
                    if (Operators.CompareString(((MyComboBoxItem)this.TextSearchVersion.Items[1]).Content.ToString(), text, false) != 0)
                    {
                        this.TextSearchVersion.Items.Insert(1, new MyComboBoxItem
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
                        List<ResourceProject>.Enumerator enumerator;
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

        private void Load_State(object sender, MyLoading.MyLoadingState state, MyLoading.MyLoadingState oldState)
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
                    ((MyPageRight)base.Parent).PageLoaderRestart(null, true);
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
            ModBase.RunInThread(delegate
            {
                Thread.Sleep(100);
                ModBase.RunInUi(delegate
                {
                    this.CardPages.IsEnabled = true;
                }, false);
                this.Loader.Start(null, false);
            });
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

        internal virtual MyCard PanAlways
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextSearchName
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
                MyTextBox myTextBox = this._TextSearchName;
                if (myTextBox != null)
                {
                    myTextBox.KeyDown -= keyEventHandler;
                }
                this._TextSearchName = value;
                myTextBox = this._TextSearchName;
                if (myTextBox != null)
                {
                    myTextBox.KeyDown += keyEventHandler;
                }
            }
        }

        internal virtual MyComboBox ComboSearchSource
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox TextSearchVersion
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
                MyComboBox.TextChangedEventHandler textChangedEventHandler = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.TextSearchVersion_TextChanged();
                };
                EventHandler eventHandler = delegate(object a0, EventArgs a1)
                {
                    this.UpdateSearchLoaderVisibility();
                };
                MyComboBox myComboBox = this._TextSearchVersion;
                if (myComboBox != null)
                {
                    myComboBox.KeyDown -= keyEventHandler;
                    myComboBox.TextChanged -= textChangedEventHandler;
                    myComboBox.DropDownClosed -= eventHandler;
                }
                this._TextSearchVersion = value;
                myComboBox = this._TextSearchVersion;
                if (myComboBox != null)
                {
                    myComboBox.KeyDown += keyEventHandler;
                    myComboBox.TextChanged += textChangedEventHandler;
                    myComboBox.DropDownClosed += eventHandler;
                }
            }
        }

        internal virtual MyComboBox ComboSearchLoader
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboSearchTag
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnSearchRun
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.StartNewSearch();
                };
                MyButton myButton = this._BtnSearchRun;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSearchRun = value;
                myButton = this._BtnSearchRun;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnSearchReset
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
                MyButton.ClickEventHandler clickEventHandler = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.ResetFilter();
                };
                MyButton myButton = this._BtnSearchReset;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSearchReset = value;
                myButton = this._BtnSearchReset;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnSearchInstallModPack
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSearchInstallModPack_Click);
                MyButton myButton = this._BtnSearchInstallModPack;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSearchInstallModPack = value;
                myButton = this._BtnSearchInstallModPack;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual StackPanel PanContent
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyHint HintError
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardProjects
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanProjects
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCard CardPages
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnPageFirst
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
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnPageFirst_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
                };
                MyIconButton myIconButton = this._BtnPageFirst;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnPageFirst = value;
                myIconButton = this._BtnPageFirst;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnPageLeft
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
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnPageLeft_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
                };
                MyIconButton myIconButton = this._BtnPageLeft;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnPageLeft = value;
                myIconButton = this._BtnPageLeft;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual TextBlock LabPage
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnPageRight
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
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnPageRight_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
                };
                MyIconButton myIconButton = this._BtnPageRight;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnPageRight = value;
                myIconButton = this._BtnPageRight;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
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

        private string _TypeName;

        private string _TypeNameSpaced;

        private ResourceTypes _Type;

        public static ModMinecraft.McInstance TargetInstance = null;

        public static string TargetName = null;

        public ModLoader.LoaderTask<ResourceSearcher.SearchRequest, int> Loader;

        public ResourceSearcher.SearchResult Storage;

        public const int PAGE_SIZE = 40;

        public int Page;

        private bool $STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded;

        private StaticLocalInitFlag $STATIC$PageResource_Loaded$20211C128119$IsFirstLoaded$Init;
    }
}
