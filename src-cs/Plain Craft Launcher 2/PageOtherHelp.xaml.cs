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
    public partial class PageOtherHelp : MyPageRight, IRefreshable
    {
        public PageOtherHelp()
        {
            base.Loaded += this.PageOther_Loaded;
            base.Initialized += this.PageOther_Inited;
            this.InitializeComponent();
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
                                List<string>.Enumerator enumerator2;
                                ((IDisposable)enumerator2).Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    List<ModMain.HelpEntry>.Enumerator enumerator;
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
                            List<ModMain.HelpEntry>.Enumerator enumerator4;
                            ((IDisposable)enumerator4).Dispose();
                        }
                        MyCard myCard = new MyCard
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
                        myCard.Children.Add(stackPanel);
                        myCard.SwapControl = stackPanel;
                        myCard.SetContentBuilder<ModMain.HelpEntry>(list2, (PageOtherHelp._Closure$__.$I3-0 == null) ? (PageOtherHelp._Closure$__.$I3-0 = (ModMain.HelpEntry Entry) => Entry.ToListItem()) : PageOtherHelp._Closure$__.$I3-0);
                        if (Operators.CompareString(text2, "指南", false) == 0)
                        {
                            myCard.BuildContent();
                        }
                        else
                        {
                            myCard.IsSwapped = true;
                        }
                        this.PanList.Children.Add(myCard);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator3;
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
            ModBase.RunInThread(delegate
            {
                if (ModMain.HelpLoader.State != ModBase.LoadState.Finished)
                {
                    ModMain.HelpLoader.WaitForExit(ModBase.GetUuid(), null, false, default(CancellationToken), null);
                }
                ModMain.HelpEntry Entry = new ModMain.HelpEntry(Location);
                ModBase.RunInUi(delegate
                {
                    PageOtherHelpDetail pageOtherHelpDetail = new PageOtherHelpDetail();
                    if (pageOtherHelpDetail.Init(Entry))
                    {
                        ModMain.FrmMain.PageChange(new FormMain.PageStackData
                        {
                            Page = FormMain.PageType.HelpDetail,
                            Additional = new object[] { Entry, pageOtherHelpDetail }
                        }, FormMain.PageSubType.Default);
                        return;
                    }
                    Logger.Warn("已取消进入帮助项目，这一般是由于 xaml 初始化失败，且用户在弹窗中手动放弃", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelp.xaml.vb");
                }, false);
            });
        }

        public static void EnterHelpPage(ModMain.HelpEntry Entry)
        {
            Action $I1;
            ModBase.RunInThread(delegate
            {
                if (ModMain.HelpLoader.State != ModBase.LoadState.Finished)
                {
                    ModMain.HelpLoader.WaitForExit(ModBase.GetUuid(), null, false, default(CancellationToken), null);
                }
                ModBase.RunInUi(($I1 == null) ? ($I1 = delegate
                {
                    PageOtherHelpDetail pageOtherHelpDetail = new PageOtherHelpDetail();
                    if (pageOtherHelpDetail.Init(Entry))
                    {
                        ModMain.FrmMain.PageChange(new FormMain.PageStackData
                        {
                            Page = FormMain.PageType.HelpDetail,
                            Additional = new object[] { Entry, pageOtherHelpDetail }
                        }, FormMain.PageSubType.Default);
                        return;
                    }
                    Logger.Warn("已取消进入帮助项目，这一般是由于 xaml 初始化失败，且用户在弹窗中手动放弃", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherHelp.xaml.vb");
                }) : $I1, false);
            });
        }

        public void SearchRun()
        {
            if (string.IsNullOrWhiteSpace(this.SearchBox.Text))
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.PanSearch, -this.PanSearch.Opacity, 100, 0, null, false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanSearch.Height = 0.0;
                        this.PanSearch.Visibility = Visibility.Collapsed;
                        this.PanList.Visibility = Visibility.Visible;
                    }, 0, true),
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
                List<ModMain.HelpEntry>.Enumerator enumerator;
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
                        MyListItem myListItem = searchEntry.Item.ToListItem();
                        if (ModBase.ModeDebug)
                        {
                            myListItem.Info = string.Concat(new string[]
                            {
                                searchEntry.AbsoluteRight ? "完全匹配，" : "",
                                "相似度：",
                                Conversions.ToString(Math.Round(searchEntry.Similarity, 3)),
                                "，",
                                myListItem.Info
                            });
                        }
                        this.PanSearchList.Children.Add(myListItem);
                    }
                }
                finally
                {
                    List<ModBase.SearchEntry<ModMain.HelpEntry>>.Enumerator enumerator2;
                    ((IDisposable)enumerator2).Dispose();
                }
                this.PanSearchList.Visibility = Visibility.Visible;
            }
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.PanList, -this.PanList.Opacity, 100, 0, null, false),
                ModAnimation.AaCode(delegate
                {
                    this.PanList.Visibility = Visibility.Collapsed;
                    this.PanSearch.Visibility = Visibility.Visible;
                    this.PanSearch.TriggerForceResize();
                }, 0, true),
                ModAnimation.AaOpacity(this.PanSearch, 1.0 - this.PanSearch.Opacity, 150, 30, null, false)
            }, "FrmOtherHelp Search Switch", false);
        }

        public void Refresh()
        {
            PageOtherLeft.RefreshHelp();
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
                MySearchBox.TextChangedEventHandler textChangedEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.SearchRun();
                };
                MySearchBox mySearchBox = this._SearchBox;
                if (mySearchBox != null)
                {
                    mySearchBox.TextChanged -= textChangedEventHandler;
                }
                this._SearchBox = value;
                mySearchBox = this._SearchBox;
                if (mySearchBox != null)
                {
                    mySearchBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual MyCard PanSearch
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanSearchList
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanList
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
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }
    }
}
