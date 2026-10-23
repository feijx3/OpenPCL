using System;
using System.CodeDom.Compiler;
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
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSetupSystem : MyPageRight
    {
        public PageSetupSystem()
        {
            base.Loaded += this.PageSetupSystem_Loaded;
            this.InitializeComponent();
        }

        private void PageSetupSystem_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            this.PanDonate.Visibility = Visibility.Visible;
            this.ItemSystemUpdateDownload.Content = "在有新版本时自动下载（更新快照版前需要先输入土豆码）";
            if (this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init, ref flag);
                if (this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State == 0)
                {
                    this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State = 2;
                    this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded = false;
                }
                else if (this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init);
                }
            }
            checked
            {
                if (!this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded)
                {
                    this.$STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded = true;
                    ModAnimation.AniControlEnabled++;
                    this.Reload();
                    this.SliderLoad();
                    ModAnimation.AniControlEnabled--;
                }
            }
        }

        public void Reload()
        {
            SettingService.RefreshSettings(this);
        }

        public void Reset()
        {
            try
            {
                SettingService.ResetSettings(this);
                Logger.Info("已初始化启动器页设置", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupSystem.xaml.vb");
                ModMain.Hint("已初始化启动器页设置！", ModMain.HintType.Green, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化启动器页设置失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupSystem.xaml.vb");
            }
            this.Reload();
        }

        private void SliderLoad()
        {
            this.SliderDownloadThread.GetHintText = ((PageSetupSystem._Closure$__.$I4-0 == null) ? (PageSetupSystem._Closure$__.$I4-0 = (object v) => Operators.AddObject(v, 1)) : PageSetupSystem._Closure$__.$I4-0);
            this.SliderDownloadSpeed.GetHintText = ((PageSetupSystem._Closure$__.$I4-1 == null) ? (PageSetupSystem._Closure$__.$I4-1 = delegate(object v)
            {
                object obj;
                if (Operators.ConditionalCompareObjectLessEqual(v, 14, false))
                {
                    obj = Operators.ConcatenateObject(Operators.MultiplyObject(Operators.AddObject(v, 1), 0.1), " M/s");
                }
                else if (Operators.ConditionalCompareObjectLessEqual(v, 31, false))
                {
                    obj = Operators.ConcatenateObject(Operators.MultiplyObject(Operators.SubtractObject(v, 11), 0.5), " M/s");
                }
                else if (Operators.ConditionalCompareObjectLessEqual(v, 41, false))
                {
                    obj = Operators.ConcatenateObject(Operators.SubtractObject(v, 21), " M/s");
                }
                else
                {
                    obj = "无限制";
                }
                return obj;
            }) : PageSetupSystem._Closure$__.$I4-1);
            this.SliderDebugAnim.GetHintText = ((PageSetupSystem._Closure$__.$I4-2 == null) ? (PageSetupSystem._Closure$__.$I4-2 = delegate(object v)
            {
                if (!Operators.ConditionalCompareObjectGreater(v, 29, false))
                {
                    return Operators.ConcatenateObject(Operators.AddObject(Operators.DivideObject(v, 10), 0.1), "x");
                }
                return "关闭";
            }) : PageSetupSystem._Closure$__.$I4-2);
        }

        private void SliderDownloadThread_PreviewChange(object sender, ModBase.RouteEventArgs e)
        {
            if (this.SliderDownloadThread.Value >= 100 && !Settings.Get<bool>("HintDownloadThread", null))
            {
                Settings.Set("HintDownloadThread", true, null);
                ModMain.MyMsgBox("如果设置过多的下载线程，可能会导致下载时出现非常严重的卡顿。\r\n一般设置 64 线程即可满足大多数下载需求，除非你知道你在干什么，否则不建议设置更多的线程数！", "警告", "我知道了", "", "", true, true, false, null, null, null);
            }
        }

        private void BtnSystemIdentify_Click(object sender, MouseButtonEventArgs e)
        {
            PageOtherAbout.CopyIdentify();
        }

        private void BtnSystemUnlock_Click(object sender, MouseButtonEventArgs e)
        {
            ModSecret.InputPotatoCode(false);
        }

        private void CheckDebugMode_Change()
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                ModMain.Hint("部分调试信息将在刷新或启动器重启后切换显示！", ModMain.HintType.Blue, false);
            }
        }

        private void ComboSystemActivity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0 && this.ComboSystemActivity.SelectedIndex == 2 && ModMain.MyMsgBox("若选择此项，即使在将来出现严重问题时，你也无法获取相关通知。\r\n例如，如果发现某个版本游戏存在严重 Bug，你可能就会因为无法得到通知而导致无法预知的后果。\r\n\r\n一般选择 仅在有重要通知时显示公告 就可以让你尽量不受打扰了。\r\n除非你在制作服务器整合包，或时常手动更新启动器，否则极度不推荐选择此项！", "警告", "我知道我在做什么", "取消", "", true, true, false, null, null, null) == 2 && e.RemovedItems.Count > 0)
            {
                this.ComboSystemActivity.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems[0]);
            }
        }

        private void ComboSystemUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0 && this.ComboSystemUpdate.SelectedIndex == 3 && ModMain.MyMsgBox("若选择此项，即使在启动器将来出现严重问题时，你也无法获取更新并获得修复。\r\n例如，如果官方修改了登录方式，从而导致现有启动器无法登录，你可能就会因为无法更新而无法开始游戏。\r\n\r\n一般选择 仅在有重大漏洞更新时显示提示 就可以让你尽量不受打扰了。\r\n除非你在制作服务器整合包，或时常手动更新启动器，否则极度不推荐选择此项！", "警告", "我知道我在做什么", "取消", "", true, true, false, null, null, null) == 2 && e.RemovedItems.Count > 0)
            {
                this.ComboSystemUpdate.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems[0]);
            }
        }

        private void BtnSystemUpdate_Click(object sender, EventArgs e)
        {
            ModSecret.UpdateCheckByButton();
        }

        public static bool? IsLauncherNewest()
        {
            bool? flag;
            try
            {
                string text = FileUtils.TryReadAsString(ModBase.PathTemp + "Cache\\Notice.cfg", null, null) ?? "";
                if (text.Split("|", false).Count<string>() < 3)
                {
                    flag = null;
                }
                else
                {
                    flag = new bool?(Conversions.ToDouble(text.Split("|", false)[1]) <= 414.0);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "确认启动器更新失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupSystem.xaml.vb");
                flag = null;
            }
            return flag;
        }

        private void BtnSystemSettingExp_Click(object sender, MouseButtonEventArgs e)
        {
            ModMain.Hint("该功能尚在开发中！", ModMain.HintType.Blue, true);
        }

        private void BtnSystemSettingImp_Click(object sender, MouseButtonEventArgs e)
        {
            ModMain.Hint("该功能尚在开发中！", ModMain.HintType.Blue, true);
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

        internal virtual MyComboBox ComboDownloadSource
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboDownloadVersion
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderDownloadThread
        {
            [CompilerGenerated]
            get
            {
                return this._SliderDownloadThread;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MySlider.PreviewChangeEventHandler previewChangeEventHandler = new MySlider.PreviewChangeEventHandler(this.SliderDownloadThread_PreviewChange);
                MySlider mySlider = this._SliderDownloadThread;
                if (mySlider != null)
                {
                    mySlider.PreviewChange -= previewChangeEventHandler;
                }
                this._SliderDownloadThread = value;
                mySlider = this._SliderDownloadThread;
                if (mySlider != null)
                {
                    mySlider.PreviewChange += previewChangeEventHandler;
                }
            }
        }

        internal virtual MySlider SliderDownloadSpeed
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboDownloadMod
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboDownloadTranslateV2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboModLocalNameStyle
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckUpdateRelease
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckUpdateSnapshot
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckHelpChinese
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboSystemUpdate
        {
            [CompilerGenerated]
            get
            {
                return this._ComboSystemUpdate;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboSystemUpdate_SelectionChanged);
                MyComboBox myComboBox = this._ComboSystemUpdate;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboSystemUpdate = value;
                myComboBox = this._ComboSystemUpdate;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual MyComboBoxItem ItemSystemUpdateDownload
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboSystemActivity
        {
            [CompilerGenerated]
            get
            {
                return this._ComboSystemActivity;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboSystemActivity_SelectionChanged);
                MyComboBox myComboBox = this._ComboSystemActivity;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboSystemActivity = value;
                myComboBox = this._ComboSystemActivity;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual MyTextBox TextSystemCache
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckSystemTelemetry
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnSystemUpdate
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSystemUpdate;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSystemUpdate_Click);
                MyButton myButton = this._BtnSystemUpdate;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSystemUpdate = value;
                myButton = this._BtnSystemUpdate;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnSystemSettingExp
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSystemSettingExp;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSystemSettingExp_Click);
                MyButton myButton = this._BtnSystemSettingExp;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSystemSettingExp = value;
                myButton = this._BtnSystemSettingExp;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnSystemSettingImp
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSystemSettingImp;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSystemSettingImp_Click);
                MyButton myButton = this._BtnSystemSettingImp;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSystemSettingImp = value;
                myButton = this._BtnSystemSettingImp;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual Grid PanDonate
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnSystemIdentify
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSystemIdentify;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSystemIdentify_Click);
                MyButton myButton = this._BtnSystemIdentify;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSystemIdentify = value;
                myButton = this._BtnSystemIdentify;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnSystemUnlock
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSystemUnlock;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSystemUnlock_Click);
                MyButton myButton = this._BtnSystemUnlock;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSystemUnlock = value;
                myButton = this._BtnSystemUnlock;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard CardDebug
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanDebugAnim
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MySlider SliderDebugAnim
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckDebugSkipCopy
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckDebugMode
        {
            [CompilerGenerated]
            get
            {
                return this._CheckDebugMode;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.CheckDebugMode_Change();
                };
                MyCheckBox myCheckBox = this._CheckDebugMode;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckDebugMode = value;
                myCheckBox = this._CheckDebugMode;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckDebugDelay
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private bool $STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded;

        private StaticLocalInitFlag $STATIC$PageSetupSystem_Loaded$20211C128141$IsLoaded$Init;
    }
}
