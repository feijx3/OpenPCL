using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceLeft : MyPageLeft, IRefreshable
    {
        public PageInstanceLeft()
        {
            base.Loaded += this.PageInstanceLeft_Loaded;
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshModDisabled();
            };
            base.Loaded += ((PageInstanceLeft._Closure$__.$IR1-2 == null) ? (PageInstanceLeft._Closure$__.$IR1-2 = delegate(object a0, RoutedEventArgs a1)
            {
                PageInstanceLeft.ReloadCurrentJava();
            }) : PageInstanceLeft._Closure$__.$IR1-2);
            this.PageID = FormMain.PageSubType.Default;
            this.InitializeComponent();
        }

        private void PageInstanceLeft_Loaded(object sender, RoutedEventArgs e)
        {
            PageSetupUI.HiddenRefresh();
        }

        public void RefreshModDisabled()
        {
            if (PageInstanceLeft.Instance != null && PageInstanceLeft.Instance.Modable)
            {
                this.ItemMod.Visibility = Visibility.Visible;
                this.ItemModDisabled.Visibility = Visibility.Collapsed;
                return;
            }
            this.ItemMod.Visibility = Visibility.Collapsed;
            this.ItemModDisabled.Visibility = Visibility.Visible;
        }

        private void PageCheck(FrameworkElement sender, ModBase.RouteEventArgs e)
        {
            if (sender.Tag != null)
            {
                this.PageChange(checked((FormMain.PageSubType)Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(sender.Tag)))));
            }
        }

        public object PageGet(FormMain.PageSubType ID = (FormMain.PageSubType)(-1))
        {
            if (ID == (FormMain.PageSubType)(-1))
            {
                ID = this.PageID;
            }
            object obj;
            switch (ID)
            {
            case FormMain.PageSubType.Default:
                if (ModMain.FrmInstanceOverall == null)
                {
                    ModMain.FrmInstanceOverall = new PageInstanceOverall();
                }
                obj = ModMain.FrmInstanceOverall;
                break;
            case FormMain.PageSubType.SetupLink:
                if (ModMain.FrmInstanceSetup == null)
                {
                    ModMain.FrmInstanceSetup = new PageInstanceSetup();
                }
                obj = ModMain.FrmInstanceSetup;
                break;
            case FormMain.PageSubType.DownloadMod:
                if (ModMain.FrmDownloadModify == null)
                {
                    ModMain.FrmDownloadModify = new PageDownloadModify();
                }
                obj = ModMain.FrmDownloadModify;
                break;
            case FormMain.PageSubType.DownloadPack:
                if (ModMain.FrmInstanceMod == null)
                {
                    ModMain.FrmInstanceMod = new PageInstanceMod();
                }
                obj = ModMain.FrmInstanceMod;
                break;
            case FormMain.PageSubType.DownloadDataPack:
                if (ModMain.FrmInstanceModDisabled == null)
                {
                    ModMain.FrmInstanceModDisabled = new PageInstanceModDisabled();
                }
                obj = ModMain.FrmInstanceModDisabled;
                break;
            case FormMain.PageSubType.DownloadResourcePack:
                if (ModMain.FrmInstanceExport == null)
                {
                    ModMain.FrmInstanceExport = new PageInstanceExport();
                }
                obj = ModMain.FrmInstanceExport;
                break;
            default:
                throw new Exception("未知的版本设置子页面种类：" + Conversions.ToString((int)ID));
            }
            return obj;
        }

        public void PageChange(FormMain.PageSubType ID)
        {
            checked
            {
                if (this.PageID != ID)
                {
                    if (PageInstanceLeft.Instance != null && ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
                    {
                        ModMain.Hint("该版本正在被修改，请稍后再切换页面！", ModMain.HintType.Red, true);
                        return;
                    }
                    ModAnimation.AniControlEnabled++;
                    try
                    {
                        PageInstanceLeft.PageChangeRun((MyPageRight)this.PageGet(ID));
                        this.PageID = ID;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceLeft.xaml.vb");
                    }
                    finally
                    {
                        ModAnimation.AniControlEnabled--;
                    }
                }
            }
        }

        private static void PageChangeRun(MyPageRight Target)
        {
            ModAnimation.AniStop("FrmMain PageChangeRight");
            if (Target.Parent != null)
            {
                Target.SetValue(ContentPresenter.ContentProperty, null);
            }
            ModMain.FrmMain.PageRight = Target;
            ((MyPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnExit();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((PageInstanceLeft._Closure$__.$I9-0 == null) ? (PageInstanceLeft._Closure$__.$I9-0 = delegate
                {
                    ((MyPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnForceExit();
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight;
                    ModMain.FrmMain.PageRight.Opacity = 0.0;
                }) : PageInstanceLeft._Closure$__.$I9-0, 130, false),
                ModAnimation.AaCode((PageInstanceLeft._Closure$__.$I9-1 == null) ? (PageInstanceLeft._Closure$__.$I9-1 = delegate
                {
                    ModMain.FrmMain.PageRight.Opacity = 1.0;
                    ModMain.FrmMain.PageRight.PageOnEnter();
                }) : PageInstanceLeft._Closure$__.$I9-1, 30, true)
            }, "PageLeft PageChange", false);
        }

        public void Refresh_Click(object sender, EventArgs e)
        {
            this.Refresh(checked((FormMain.PageSubType)Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null))))));
        }

        public void Refresh()
        {
            this.Refresh(ModMain.FrmMain.PageCurrentSub);
        }

        public void Refresh(FormMain.PageSubType SubType)
        {
            if (PageInstanceLeft.Instance != null && ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
            {
                ModMain.Hint("该版本正在被修改，请稍后再刷新！", ModMain.HintType.Red, true);
                return;
            }
            switch (SubType)
            {
            case FormMain.PageSubType.DownloadMod:
            {
                PageDownloadModify frmDownloadModify = ModMain.FrmDownloadModify;
                if (frmDownloadModify != null)
                {
                    frmDownloadModify.PageLoaderRestart(null, true);
                }
                this.ItemModify.Checked = true;
                break;
            }
            case FormMain.PageSubType.DownloadPack:
                PageInstanceMod.Refresh();
                this.ItemMod.Checked = true;
                return;
            case FormMain.PageSubType.DownloadDataPack:
                break;
            case FormMain.PageSubType.DownloadResourcePack:
            {
                PageInstanceExport frmInstanceExport = ModMain.FrmInstanceExport;
                if (frmInstanceExport != null)
                {
                    frmInstanceExport.RefreshAll();
                }
                this.ItemExport.Checked = true;
                return;
            }
            default:
                return;
            }
        }

        public void Reset(object sender, EventArgs e)
        {
            if (ModMain.MyMsgBox("是否要初始化该版本的版本独立设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
            {
                if (Information.IsNothing(ModMain.FrmInstanceSetup))
                {
                    ModMain.FrmInstanceSetup = new PageInstanceSetup();
                }
                ModMain.FrmInstanceSetup.Reset();
                this.ItemSetup.Checked = true;
            }
        }

        public static void ReloadCurrentJava()
        {
            if (ModAnimation.AniControlEnabled == 0 && PageInstanceLeft.Instance != null)
            {
                PageInstanceLeft.CurrentJavaWorker.Start(default(CancellationToken), null);
            }
        }

        internal virtual StackPanel PanItem
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyListItem ItemOverall
        {
            [CompilerGenerated]
            get
            {
                return this._ItemOverall;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemOverall;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemOverall = value;
                myListItem = this._ItemOverall;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemSetup
        {
            [CompilerGenerated]
            get
            {
                return this._ItemSetup;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemSetup;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemSetup = value;
                myListItem = this._ItemSetup;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemModify
        {
            [CompilerGenerated]
            get
            {
                return this._ItemModify;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemModify;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemModify = value;
                myListItem = this._ItemModify;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemMod
        {
            [CompilerGenerated]
            get
            {
                return this._ItemMod;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemMod;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemMod = value;
                myListItem = this._ItemMod;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemModDisabled
        {
            [CompilerGenerated]
            get
            {
                return this._ItemModDisabled;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemModDisabled;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemModDisabled = value;
                myListItem = this._ItemModDisabled;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemExport
        {
            [CompilerGenerated]
            get
            {
                return this._ItemExport;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemExport;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemExport = value;
                myListItem = this._ItemExport;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        public static ModMinecraft.McInstance Instance = null;

        public FormMain.PageSubType PageID;

        public static RedoableWorker<Java> CurrentJavaWorker = new RedoableWorker<Java>((CancellationToken? c, ProgressProvider p) => ModJava.SelectOrDownloadJava(PageInstanceLeft.Instance, false, c.Value, p), "CurrentJavaWorker");
    }
}
