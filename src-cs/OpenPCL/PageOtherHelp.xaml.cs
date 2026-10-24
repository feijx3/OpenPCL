using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageOtherHelp : OurPageRight, IRefreshable
    {
        public PageOtherHelp()
        {
            base.Loaded += this.PageOther_Loaded;
            base.Initialized += this.PageOther_Inited;
            this.InitializeComponent();
            this.SearchBox_WithEvents = this.SearchBox;
        }

        private void PageOther_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
        }

        private void PageOther_Inited(object sender, EventArgs e)
        {
            base.PageLoaderInit<int, List<ModMain.HelpEntry>>(this.Load, this.PanLoad, this.PanBack, null, ModMain.HelpLoader, new Action<ModLoader.LoaderTask<int, List<ModMain.HelpEntry>>>(this.HelpListLoad), null, true);
        }

        private void HelpListLoad(ModLoader.LoaderTask<int, List<ModMain.HelpEntry>> Loader)
        {
            try
            {
                this.PanList.Children.Clear();
                this.PanBack.ScrollToHome();
                List<ModMain.HelpEntry> output = Loader.Output;
                List<string> list = new List<string>();
                try
                {
                    foreach (ModMain.HelpEntry helpEntry in output)
                    {
                        if (helpEntry.ShowInSnapshot)
                        {
                            try
                            {
                                foreach (string text in helpEntry.Types)
                                {
                                    if (!list.Contains(text))
                                    {
                                        list.Add(text);
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
                }
                finally
                {
                    List<ModMain.HelpEntry>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
                if (list.Contains("指南"))
                {
                    list.Remove("指南");
                    list.Insert(0, "指南");
                }
                try
                {
                    foreach (string text2 in list)
                    {
                        List<ModMain.HelpEntry> list2 = new List<ModMain.HelpEntry>();
                        try
                        {
                            foreach (ModMain.HelpEntry helpEntry2 in output)
                            {
                                if (helpEntry2.ShowInSnapshot && helpEntry2.Types.Contains(text2))
                                {
                                    list2.Add(helpEntry2);
                                }
                            }
                        }
                        finally
                        {
                            List<ModMain.HelpEntry>.Enumerator enumerator4 = default;
                            ((IDisposable)enumerator4).Dispose();
                        }
                        OurCard ourCard = new OurCard
                        {
                            Title = text2,
                            Margin = new Thickness(0.0, 0.0, 0.0, 15.0)
                        };
                        StackPanel stackPanel = new StackPanel
                        {
                            Margin = new Thickness(20.0, 40.0, 18.0, 0.0),
                            VerticalAlignment = VerticalAlignment.Top,
                            RenderTransform = new TranslateTransform(0.0, 0.0)
                        };
                        ourCard.Children.Add(stackPanel);
                        ourCard.SwapControl = stackPanel;
                        ourCard.SetContentBuilder<ModMain.HelpEntry>(list2, (PageOtherHelp._Closure___._I3_0 == null) ? (PageOtherHelp._Closure___._I3_0 = new Func<ModMain.HelpEntry, UIElement>(PageOtherHelp._Closure___._I._Lambda___3_0)) : PageOtherHelp._Closure___._I3_0);
                        if (Operators.CompareString(text2, "指南", false) == 0)
                        {
                            ourCard.BuildContent();
                        }
                        else
                        {
                            ourCard.IsSwapped = true;
                        }
                        this.PanList.Children.Add(ourCard);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator3 = default;
                    ((IDisposable)enumerator3).Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "加载帮助列表 UI 失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelp.xaml.vb");
            }
        }

        public static void OnItemClick(ModMain.HelpEntry Entry)
        {
            try
            {
                if (Entry.IsEvent)
                {
                    CustomEvent.Raise(Entry.EventType, Entry.EventData);
                }
                else
                {
                    PageOtherHelp.EnterHelpPage(Entry);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "处理帮助项目点击时发生意外错误", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelp.xaml.vb");
            }
        }

        public static void EnterHelpPage(string Location)
        {
            PageOtherHelp._Closure___5_0 CS_locals1 = new PageOtherHelp._Closure___5_0();
            CS_locals1._VB_Local_Location = Location;
            ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
        }

        public static void EnterHelpPage(ModMain.HelpEntry Entry)
        {
            PageOtherHelp._Closure___6_0 CS_locals1 = new PageOtherHelp._Closure___6_0();
            CS_locals1._VB_Local_Entry = Entry;
            ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
        }

        public void SearchRun()
        {
            if (string.IsNullOrWhiteSpace(this.SearchBox.Text))
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.PanSearch, -this.PanSearch.Opacity, 100, 0, null, false),
                    ModAnimation.AaCode(new ThreadStart(this._Lambda___7_0), 0, true),
                    ModAnimation.AaOpacity(this.PanList, 1.0 - this.PanList.Opacity, 150, 30, null, false)
                }, "FrmOtherHelp Search Switch", false);
                return;
            }
            List<ModBase.SearchEntry<ModMain.HelpEntry>> list = new List<ModBase.SearchEntry<ModMain.HelpEntry>>();
            try
            {
                foreach (ModMain.HelpEntry helpEntry in ModMain.HelpLoader.Output)
                {
                    if (helpEntry.ShowInSearch && helpEntry.ShowInSearch && helpEntry.ShowInSnapshot)
                    {
                        list.Add(new ModBase.SearchEntry<ModMain.HelpEntry>
                        {
                            Item = helpEntry,
                            SearchSource = new List<ModBase.SearchSource>
                            {
                                new ModBase.SearchSource(helpEntry.Title, 1.0),
                                new ModBase.SearchSource(helpEntry.Desc, 0.5),
                                new ModBase.SearchSource(helpEntry.Search, 1.5)
                            }
                        });
                    }
                }
            }
            finally
            {
                List<ModMain.HelpEntry>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            List<ModBase.SearchEntry<ModMain.HelpEntry>> list2 = ModBase.Search<ModMain.HelpEntry>(list, this.SearchBox.Text, 5, 0.08);
            this.PanSearchList.Children.Clear();
            if (!list2.Any<ModBase.SearchEntry<ModMain.HelpEntry>>())
            {
                this.PanSearch.Title = "无搜索结果";
                this.PanSearchList.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.PanSearch.Title = "搜索结果";
                try
                {
                    foreach (ModBase.SearchEntry<ModMain.HelpEntry> searchEntry in list2)
                    {
                        OurListItem ourListItem = searchEntry.Item.ToListItem();
                        if (ModBase.ModeDebug)
                        {
                            ourListItem.Info = string.Concat(new string[]
                            {
                                searchEntry.AbsoluteRight ? "完全匹配，" : "",
                                "相似度：",
                                Conversions.ToString(Math.Round(searchEntry.Similarity, 3)),
                                "，",
                                ourListItem.Info
                            });
                        }
                        this.PanSearchList.Children.Add(ourListItem);
                    }
                }
                finally
                {
                    List<ModBase.SearchEntry<ModMain.HelpEntry>>.Enumerator enumerator2 = default;
                    ((IDisposable)enumerator2).Dispose();
                }
                this.PanSearchList.Visibility = Visibility.Visible;
            }
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.PanList, -this.PanList.Opacity, 100, 0, null, false),
                ModAnimation.AaCode(new ThreadStart(this._Lambda___7_1), 0, true),
                ModAnimation.AaOpacity(this.PanSearch, 1.0 - this.PanSearch.Opacity, 150, 30, null, false)
            }, "FrmOtherHelp Search Switch", false);
        }

        public void Refresh()
        {
            PageOtherLeft.RefreshHelp();
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
                OurSearchBox.TextChangedEventHandler textChangedEventHandler = new OurSearchBox.TextChangedEventHandler(this._Lambda___R19_1);
                OurSearchBox ourSearchBox = this._SearchBox;
                if (ourSearchBox != null)
                {
                    ourSearchBox.TextChanged -= textChangedEventHandler;
                }
                this._SearchBox = value;
                ourSearchBox = this._SearchBox;
                if (ourSearchBox != null)
                {
                    ourSearchBox.TextChanged += textChangedEventHandler;
                }
            }
        }






        [CompilerGenerated]
        private void _Lambda___7_0()
        {
            this.PanSearch.Height = 0.0;
            this.PanSearch.Visibility = Visibility.Collapsed;
            this.PanList.Visibility = Visibility.Visible;
        }

        [CompilerGenerated]
        private void _Lambda___7_1()
        {
            this.PanList.Visibility = Visibility.Collapsed;
            this.PanSearch.Visibility = Visibility.Visible;
            this.PanSearch.TriggerForceResize();
        }

        [CompilerGenerated]
        private void _Lambda___R19_1(object a0, EventArgs a1)
        {
            this.SearchRun();
        }

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
        [AccessedThroughProperty("PanSearch")]
        private OurCard _PanSearch;

        [CompilerGenerated]
        [AccessedThroughProperty("PanSearchList")]
        private StackPanel _PanSearchList;

        [CompilerGenerated]
        [AccessedThroughProperty("PanList")]
        private StackPanel _PanList;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLoad")]
        private OurCard _PanLoad;

        [CompilerGenerated]
        [AccessedThroughProperty("Load")]
        private OurLoading _Load;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal UIElement _Lambda___3_0(ModMain.HelpEntry Entry)
            {
                return Entry.ToListItem();
            }

            public static readonly PageOtherHelp._Closure___ _I = new PageOtherHelp._Closure___();

            public static Func<ModMain.HelpEntry, UIElement> _I3_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___5_0
        {
            internal void _Lambda___0()
            {
                PageOtherHelp._Closure___5_1 CS_locals1 = new PageOtherHelp._Closure___5_1();
                if (ModMain.HelpLoader.State != ModBase.LoadState.Finished)
                {
                    ModMain.HelpLoader.WaitForExit(ModBase.GetUuid(), null, false, default(CancellationToken), null);
                }
                CS_locals1._VB_Local_Entry = new ModMain.HelpEntry(this._VB_Local_Location);
                ModBase.RunInUi(new Action(CS_locals1._Lambda___1), false);
            }

            public string _VB_Local_Location;
        }

        [CompilerGenerated]
        private sealed class _Closure___5_1
        {
            internal void _Lambda___1()
            {
                PageOtherHelpDetail pageOtherHelpDetail = new PageOtherHelpDetail();
                if (pageOtherHelpDetail.Init(this._VB_Local_Entry))
                {
                    ModMain.FrmMain.PageChange(new FormMain.PageStackData
                    {
                        Page = FormMain.PageType.HelpDetail,
                        Additional = new object[] { this._VB_Local_Entry, pageOtherHelpDetail }
                    }, FormMain.PageSubType.Default);
                    return;
                }
                Logger.Warn("已取消进入帮助项目，这一般是由于 xaml 初始化失败，且用户在弹窗中手动放弃", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelp.xaml.vb");
            }

            public ModMain.HelpEntry _VB_Local_Entry;
        }

        [CompilerGenerated]
        private sealed class _Closure___6_0
        {
            internal void _Lambda___0()
            {
                if (ModMain.HelpLoader.State != ModBase.LoadState.Finished)
                {
                    ModMain.HelpLoader.WaitForExit(ModBase.GetUuid(), null, false, default(CancellationToken), null);
                }
                ModBase.RunInUi((this._I1 == null) ? (this._I1 = new Action(this._Lambda___1)) : this._I1, false);
            }

            internal void _Lambda___1()
            {
                PageOtherHelpDetail pageOtherHelpDetail = new PageOtherHelpDetail();
                if (pageOtherHelpDetail.Init(this._VB_Local_Entry))
                {
                    ModMain.FrmMain.PageChange(new FormMain.PageStackData
                    {
                        Page = FormMain.PageType.HelpDetail,
                        Additional = new object[] { this._VB_Local_Entry, pageOtherHelpDetail }
                    }, FormMain.PageSubType.Default);
                    return;
                }
                Logger.Warn("已取消进入帮助项目，这一般是由于 xaml 初始化失败，且用户在弹窗中手动放弃", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelp.xaml.vb");
            }

            public ModMain.HelpEntry _VB_Local_Entry;

            public Action _I1;
        }
    }
}
