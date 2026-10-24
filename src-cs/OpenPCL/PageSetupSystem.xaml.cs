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
    public partial class PageSetupSystem : OurPageRight
    {
        public PageSetupSystem()
        {
            base.Loaded += this.PageSetupSystem_Loaded;
            this.InitializeComponent();
            this.SliderDownloadThread_WithEvents = this.SliderDownloadThread;
            this.ComboSystemUpdate_WithEvents = this.ComboSystemUpdate;
            this.ComboSystemActivity_WithEvents = this.ComboSystemActivity;
            this.BtnSystemUpdate_WithEvents = this.BtnSystemUpdate;
            this.BtnSystemSettingExp_WithEvents = this.BtnSystemSettingExp;
            this.BtnSystemSettingImp_WithEvents = this.BtnSystemSettingImp;
            this.BtnSystemIdentify_WithEvents = this.BtnSystemIdentify;
            this.BtnSystemUnlock_WithEvents = this.BtnSystemUnlock;
            this.CheckDebugMode_WithEvents = this.CheckDebugMode;
        }

        private void PageSetupSystem_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            this.PanDonate.Visibility = Visibility.Visible;
            this.ItemSystemUpdateDownload.Content = "在有新版本时自动下载（更新快照版前需要先输入土豆码）";
            if (this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init == null)
            {
                Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init, new StaticLocalInitFlag(), null);
            }
            bool flag = false;
            try
            {
                Monitor.Enter(this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init, ref flag);
                if (this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init.State == 0)
                {
                    this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init.State = 2;
                    this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded = false;
                }
                else if (this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init.State = 1;
                if (flag)
                {
                    Monitor.Exit(this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init);
                }
            }
            checked
            {
                if (!this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded)
                {
                    this._STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded = true;
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
            this.SliderDownloadThread.GetHintText = ((PageSetupSystem._Closure___._I4_0 == null) ? (PageSetupSystem._Closure___._I4_0 = new VB_AnonymousDelegate_25<object, object>(PageSetupSystem._Closure___._I._Lambda___4_0)) : PageSetupSystem._Closure___._I4_0);
            this.SliderDownloadSpeed.GetHintText = ((PageSetupSystem._Closure___._I4_1 == null) ? (PageSetupSystem._Closure___._I4_1 = new VB_AnonymousDelegate_25<object, object>(PageSetupSystem._Closure___._I._Lambda___4_1)) : PageSetupSystem._Closure___._I4_1);
            this.SliderDebugAnim.GetHintText = ((PageSetupSystem._Closure___._I4_2 == null) ? (PageSetupSystem._Closure___._I4_2 = new VB_AnonymousDelegate_25<object, object>(PageSetupSystem._Closure___._I._Lambda___4_2)) : PageSetupSystem._Closure___._I4_2);
        }

        private void SliderDownloadThread_PreviewChange(object sender, ModBase.RouteEventArgs e)
        {
            if (this.SliderDownloadThread.Value >= 100 && !Settings.Get<bool>("HintDownloadThread", null))
            {
                Settings.Set("HintDownloadThread", true, null);
                ModMain.OurMsgBox("如果设置过多的下载线程，可能会导致下载时出现非常严重的卡顿。\r\n一般设置 64 线程即可满足大多数下载需求，除非你知道你在干什么，否则不建议设置更多的线程数！", "警告", "我知道了", "", "", true, true, false, null, null, null);
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
            if (ModAnimation.AniControlEnabled == 0 && this.ComboSystemActivity.SelectedIndex == 2 && ModMain.OurMsgBox("若选择此项，即使在将来出现严重问题时，你也无法获取相关通知。\r\n例如，如果发现某个版本游戏存在严重 Bug，你可能就会因为无法得到通知而导致无法预知的后果。\r\n\r\n一般选择 仅在有重要通知时显示公告 就可以让你尽量不受打扰了。\r\n除非你在制作服务器整合包，或时常手动更新启动器，否则极度不推荐选择此项！", "警告", "我知道我在做什么", "取消", "", true, true, false, null, null, null) == 2 && e.RemovedItems.Count > 0)
            {
                this.ComboSystemActivity.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems[0]);
            }
        }

        private void ComboSystemUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0 && this.ComboSystemUpdate.SelectedIndex == 3 && ModMain.OurMsgBox("若选择此项，即使在启动器将来出现严重问题时，你也无法获取更新并获得修复。\r\n例如，如果官方修改了登录方式，从而导致现有启动器无法登录，你可能就会因为无法更新而无法开始游戏。\r\n\r\n一般选择 仅在有重大漏洞更新时显示提示 就可以让你尽量不受打扰了。\r\n除非你在制作服务器整合包，或时常手动更新启动器，否则极度不推荐选择此项！", "警告", "我知道我在做什么", "取消", "", true, true, false, null, null, null) == 2 && e.RemovedItems.Count > 0)
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





        internal virtual OurSlider SliderDownloadThread_WithEvents
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
                OurSlider.PreviewChangeEventHandler previewChangeEventHandler = new OurSlider.PreviewChangeEventHandler(this.SliderDownloadThread_PreviewChange);
                OurSlider ourSlider = this._SliderDownloadThread;
                if (ourSlider != null)
                {
                    ourSlider.PreviewChange -= previewChangeEventHandler;
                }
                this._SliderDownloadThread = value;
                ourSlider = this._SliderDownloadThread;
                if (ourSlider != null)
                {
                    ourSlider.PreviewChange += previewChangeEventHandler;
                }
            }
        }








        internal virtual OurComboBox ComboSystemUpdate_WithEvents
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
                OurComboBox ourComboBox = this._ComboSystemUpdate;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboSystemUpdate = value;
                ourComboBox = this._ComboSystemUpdate;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }


        internal virtual OurComboBox ComboSystemActivity_WithEvents
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
                OurComboBox ourComboBox = this._ComboSystemActivity;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboSystemActivity = value;
                ourComboBox = this._ComboSystemActivity;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }



        internal virtual OurButton BtnSystemUpdate_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSystemUpdate_Click);
                OurButton ourButton = this._BtnSystemUpdate;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSystemUpdate = value;
                ourButton = this._BtnSystemUpdate;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnSystemSettingExp_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSystemSettingExp_Click);
                OurButton ourButton = this._BtnSystemSettingExp;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSystemSettingExp = value;
                ourButton = this._BtnSystemSettingExp;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnSystemSettingImp_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSystemSettingImp_Click);
                OurButton ourButton = this._BtnSystemSettingImp;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSystemSettingImp = value;
                ourButton = this._BtnSystemSettingImp;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurButton BtnSystemIdentify_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSystemIdentify_Click);
                OurButton ourButton = this._BtnSystemIdentify;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSystemIdentify = value;
                ourButton = this._BtnSystemIdentify;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnSystemUnlock_WithEvents
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
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSystemUnlock_Click);
                OurButton ourButton = this._BtnSystemUnlock;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSystemUnlock = value;
                ourButton = this._BtnSystemUnlock;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }





        internal virtual OurCheckBox CheckDebugMode_WithEvents
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
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R125_1);
                OurCheckBox ourCheckBox = this._CheckDebugMode;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckDebugMode = value;
                ourCheckBox = this._CheckDebugMode;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }


        [CompilerGenerated]
        private void _Lambda___R125_1(object a0, bool a1)
        {
            this.CheckDebugMode_Change();
        }

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboDownloadSource")]
        private OurComboBox _ComboDownloadSource;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboDownloadVersion")]
        private OurComboBox _ComboDownloadVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderDownloadThread")]
        private OurSlider _SliderDownloadThread;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderDownloadSpeed")]
        private OurSlider _SliderDownloadSpeed;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboDownloadMod")]
        private OurComboBox _ComboDownloadMod;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboDownloadTranslateV2")]
        private OurComboBox _ComboDownloadTranslateV2;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboModLocalNameStyle")]
        private OurComboBox _ComboModLocalNameStyle;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckUpdateRelease")]
        private OurCheckBox _CheckUpdateRelease;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckUpdateSnapshot")]
        private OurCheckBox _CheckUpdateSnapshot;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckHelpChinese")]
        private OurCheckBox _CheckHelpChinese;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboSystemUpdate")]
        private OurComboBox _ComboSystemUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemSystemUpdateDownload")]
        private OurComboBoxItem _ItemSystemUpdateDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboSystemActivity")]
        private OurComboBox _ComboSystemActivity;

        [CompilerGenerated]
        [AccessedThroughProperty("TextSystemCache")]
        private OurTextBox _TextSystemCache;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckSystemTelemetry")]
        private OurCheckBox _CheckSystemTelemetry;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSystemUpdate")]
        private OurButton _BtnSystemUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSystemSettingExp")]
        private OurButton _BtnSystemSettingExp;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSystemSettingImp")]
        private OurButton _BtnSystemSettingImp;

        [CompilerGenerated]
        [AccessedThroughProperty("PanDonate")]
        private Grid _PanDonate;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSystemIdentify")]
        private OurButton _BtnSystemIdentify;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSystemUnlock")]
        private OurButton _BtnSystemUnlock;

        [CompilerGenerated]
        [AccessedThroughProperty("CardDebug")]
        private OurCard _CardDebug;

        [CompilerGenerated]
        [AccessedThroughProperty("PanDebugAnim")]
        private Grid _PanDebugAnim;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderDebugAnim")]
        private OurSlider _SliderDebugAnim;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckDebugSkipCopy")]
        private OurCheckBox _CheckDebugSkipCopy;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckDebugMode")]
        private OurCheckBox _CheckDebugMode;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckDebugDelay")]
        private OurCheckBox _CheckDebugDelay;

        private bool _STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded;

        private StaticLocalInitFlag _STATIC_PageSetupSystem_Loaded_20211C128141_IsLoaded_Init;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal object _Lambda___4_0(object v)
            {
                return Operators.AddObject(v, 1);
            }

            internal object _Lambda___4_1(object v)
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
            }

            internal object _Lambda___4_2(object v)
            {
                if (!Operators.ConditionalCompareObjectGreater(v, 29, false))
                {
                    return Operators.ConcatenateObject(Operators.AddObject(Operators.DivideObject(v, 10), 0.1), "x");
                }
                return "关闭";
            }

            public static readonly PageSetupSystem._Closure___ _I = new PageSetupSystem._Closure___();

            public static VB_AnonymousDelegate_25<object, object> _I4_0;

            public static VB_AnonymousDelegate_25<object, object> _I4_1;

            public static VB_AnonymousDelegate_25<object, object> _I4_2;
        }
    }
}
