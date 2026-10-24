using System;
using System.CodeDom.Compiler;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using PCL.Our;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceSetup : OurPageRight
    {
        public PageInstanceSetup()
        {
            base.Loaded += this.PageSetupSystem_Loaded;
            this.LoadedFirst = false;
            this.RamTextLeft = 2;
            this.RamTextRight = 1;
            this.InitializeComponent();
            this.ComboArgumentIndieV2_WithEvents = this.ComboArgumentIndieV2;
            this.ComboArgumentJava_WithEvents = this.ComboArgumentJava;
            this.TextArgumentJavaRange_WithEvents = this.TextArgumentJavaRange;
            this.HintArgumentJava_WithEvents = this.HintArgumentJava;
            this.BtnArgumentJavaSearch_WithEvents = this.BtnArgumentJavaSearch;
            this.RadioRamType2_WithEvents = this.RadioRamType2;
            this.RadioRamType0_WithEvents = this.RadioRamType0;
            this.RadioRamType1_WithEvents = this.RadioRamType1;
            this.SliderRamCustom_WithEvents = this.SliderRamCustom;
            this.RectRamGame_WithEvents = this.RectRamGame;
            this.RectRamEmpty_WithEvents = this.RectRamEmpty;
            this.LabRamGame_WithEvents = this.LabRamGame;
            this.ComboServerLogin_WithEvents = this.ComboServerLogin;
            this.TextServerNide_WithEvents = this.TextServerNide;
            this.TextServerAuthServer_WithEvents = this.TextServerAuthServer;
            this.TextServerAuthRegister_WithEvents = this.TextServerAuthRegister;
            this.TextServerEnter_WithEvents = this.TextServerEnter;
            this.BtnServerAuthLittle_WithEvents = this.BtnServerAuthLittle;
            this.TextAdvanceJvm_WithEvents = this.TextAdvanceJvm;
            this.TextAdvanceGame_WithEvents = this.TextAdvanceGame;
            this.TextAdvanceRun_WithEvents = this.TextAdvanceRun;
            this.BtnSwitch_WithEvents = this.BtnSwitch;
        }

        private void PageSetupSystem_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            this.RefreshRam(false);
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.Reload();
                ModAnimation.AniControlEnabled--;
                if (!this.LoadedFirst)
                {
                    this.LoadedFirst = true;
                    PageInstanceLeft.CurrentJavaWorker.Started += this.UpdateJavaRelatedUi;
                    PageInstanceLeft.CurrentJavaWorker.Stopped += this.UpdateJavaRelatedUi;
                    this.TextArgumentJavaRange.ValidateRules = new Collection<Validate>
                    {
                        new ValidateFunc((PageInstanceSetup._Closure___._I2_0 == null) ? (PageInstanceSetup._Closure___._I2_0 = new Func<string, string>(PageInstanceSetup._Closure___._I._Lambda___2_0)) : PageInstanceSetup._Closure___._I2_0)
                    };
                    DispatcherTimer dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
                    dispatcherTimer.Tick += this._Lambda___R2_1;
                    dispatcherTimer.Start();
                }
            }
        }

        public void Reload()
        {
            try
            {
                string pathIndie = PageInstanceLeft.Instance.PathIndie;
                this.ComboArgumentIndieV2.SelectedIndex = ((!Settings.Get<bool>("VersionArgumentIndieV2", PageInstanceLeft.Instance)) ? 1 : 0);
                this.UpdateJavaList();
                this.ComboServerLogin.SelectedIndex = Settings.Get<int>("VersionServerLogin", PageInstanceLeft.Instance);
                this.ComboServerLoginLast = this.ComboServerLogin.SelectedIndex;
                this.UpdateServerLoginUI();
                if (Settings.Get<int>("VersionAdvanceAssets", PageInstanceLeft.Instance) == 2)
                {
                    Logger.Info("已迁移老版本的关闭文件校验设置", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceSetup.xaml.vb");
                    Settings.Reset("VersionAdvanceAssets", PageInstanceLeft.Instance);
                    Settings.Set("VersionAdvanceAssetsV2", true, PageInstanceLeft.Instance);
                }
                SettingService.RefreshSettings(this);
                PageInstanceSetup.OnVersionRamTypeChanged(Settings.Get<int>("VersionRamType", PageInstanceLeft.Instance));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "重载版本独立设置时出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceSetup.xaml.vb");
            }
        }

        public void Reset()
        {
            try
            {
                SettingService.ResetSettings(this);
                Settings.Reset("VersionServerLogin", PageInstanceLeft.Instance);
                Settings.Reset("VersionArgumentIndieV2", PageInstanceLeft.Instance);
                Settings.Reset("VersionAdvanceAssets", PageInstanceLeft.Instance);
                Configs.JavaForced.Reset(PageInstanceLeft.Instance.Config);
                ModJava.JavaListRefreshWorker.Start(default(CancellationToken), null);
                Logger.Info("已初始化版本独立设置", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceSetup.xaml.vb");
                ModMain.Hint("已初始化版本独立设置！", ModMain.HintType.Green, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化版本独立设置失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceSetup.xaml.vb");
            }
            this.Reload();
        }

        public static void OnVersionRamTypeChanged(int Type)
        {
            if (ModMain.FrmInstanceSetup != null)
            {
                ModMain.FrmInstanceSetup.RamType(Type);
            }
        }

        public void RamType(int Type)
        {
            if (this.SliderRamCustom != null)
            {
                this.SliderRamCustom.IsEnabled = Type == 1;
            }
        }

        public void RefreshRam(bool ShowAnim)
        {
            if (this.LabRamGame != null && this.LabRamUsed != null && !(ModMain.FrmMain.PageCurrent != FormMain.PageType.InstanceSetup) && ModMain.FrmInstanceLeft.PageID == FormMain.PageSubType.SetupLink)
            {
                double num = Math.Round(PageInstanceSetup.GetRam(PageInstanceLeft.Instance), 5);
                double num2 = Math.Round(OurWpfExtension.Computer.Info.TotalPhysicalMemory / 1024.0 / 1024.0 / 1024.0, 1);
                double num3 = Math.Round(OurWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0, 1);
                double num4 = Math.Round(Math.Min(num, num3), 5);
                double num5 = Math.Round(num2 - num3, 5);
                double num6 = Math.Round((num2 - num5 - num).Clamp(0.0, 1000.0), 1);
                checked
                {
                    if (num2 <= 1.5)
                    {
                        this.SliderRamCustom.MaxValue = (int)Math.Round(Math.Max(Math.Floor(unchecked(num2 - 0.3) / 0.1), 1.0));
                    }
                    else if (num2 <= 8.0)
                    {
                        this.SliderRamCustom.MaxValue = (int)Math.Round(unchecked(Math.Floor((num2 - 1.5) / 0.5) + 12.0));
                    }
                    else if (num2 <= 16.0)
                    {
                        this.SliderRamCustom.MaxValue = (int)Math.Round(unchecked(Math.Floor((num2 - 8.0) / 1.0) + 25.0));
                    }
                    else
                    {
                        this.SliderRamCustom.MaxValue = (int)Math.Round(unchecked(Math.Floor((num2 - 16.0) / 2.0) + 33.0));
                    }
                    this.LabRamGame.Text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject((num == Math.Floor(num)) ? (Conversions.ToString(num) + ".0") : num, " GB"), (num != num4) ? Operators.ConcatenateObject(Operators.ConcatenateObject(" (可用 ", (num4 == Math.Floor(num4)) ? (Conversions.ToString(num4) + ".0") : num4), " GB)") : ""));
                    this.LabRamUsed.Text = Conversions.ToString(Operators.ConcatenateObject((num5 == Math.Floor(num5)) ? (Conversions.ToString(num5) + ".0") : num5, " GB"));
                    this.LabRamTotal.Text = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(" / ", (num2 == Math.Floor(num2)) ? (Conversions.ToString(num2) + ".0") : num2), " GB"));
                }
                if (ShowAnim)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaGridLengthWidth(this.ColumnRamUsed, num5 - this.ColumnRamUsed.Width.Value, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                        ModAnimation.AaGridLengthWidth(this.ColumnRamGame, num4 - this.ColumnRamGame.Width.Value, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                        ModAnimation.AaGridLengthWidth(this.ColumnRamEmpty, num6 - this.ColumnRamEmpty.Width.Value, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false)
                    }, "VersionSetup Ram Grid", false);
                    return;
                }
                this.ColumnRamUsed.Width = new GridLength(num5, GridUnitType.Star);
                this.ColumnRamGame.Width = new GridLength(num4, GridUnitType.Star);
                this.ColumnRamEmpty.Width = new GridLength(num6, GridUnitType.Star);
            }
        }

        private void RefreshRam()
        {
            this.RefreshRam(true);
        }

        private void RefreshRamText()
        {
            double actualWidth = this.RectRamUsed.ActualWidth;
            double actualWidth2 = this.PanRamDisplay.ActualWidth;
            double actualWidth3 = this.LabRamGame.ActualWidth;
            double actualWidth4 = this.LabRamUsed.ActualWidth;
            double actualWidth5 = this.LabRamTotal.ActualWidth;
            double actualWidth6 = this.LabRamGameTitle.ActualWidth;
            double actualWidth7 = this.LabRamUsedTitle.ActualWidth;
            int num;
            if (actualWidth - 30.0 < actualWidth4 || actualWidth - 30.0 < actualWidth7)
            {
                num = 0;
            }
            else if (actualWidth - 25.0 < actualWidth4 + actualWidth5)
            {
                num = 1;
            }
            else
            {
                num = 2;
            }
            if (this.RamTextLeft != num)
            {
                this.RamTextLeft = num;
                switch (num)
                {
                case 0:
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.LabRamUsed, -this.LabRamUsed.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamTotal, -this.LabRamTotal.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamUsedTitle, -this.LabRamUsedTitle.Opacity, 100, 0, null, false)
                    }, "VersionSetup Ram TextLeft", false);
                    break;
                case 1:
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.LabRamUsed, 1.0 - this.LabRamUsed.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamTotal, -this.LabRamTotal.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamUsedTitle, 0.7 - this.LabRamUsedTitle.Opacity, 100, 0, null, false)
                    }, "VersionSetup Ram TextLeft", false);
                    break;
                case 2:
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.LabRamUsed, 1.0 - this.LabRamUsed.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamTotal, 1.0 - this.LabRamTotal.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamUsedTitle, 0.7 - this.LabRamUsedTitle.Opacity, 100, 0, null, false)
                    }, "VersionSetup Ram TextLeft", false);
                    break;
                }
            }
            int num2;
            if (actualWidth2 < actualWidth3 + 2.0 + actualWidth || actualWidth2 < actualWidth6 + 2.0 + actualWidth)
            {
                num2 = 0;
            }
            else
            {
                num2 = 1;
            }
            if (num2 == 0)
            {
                if (ModAnimation.AniControlEnabled == 0 && (this.RamTextRight != num2 || ModAnimation.AniIsRun("VersionSetup Ram TextRight")))
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaX(this.LabRamGame, actualWidth2 - actualWidth3 - this.LabRamGame.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaX(this.LabRamGameTitle, actualWidth2 - actualWidth6 - this.LabRamGameTitle.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                    }, "VersionSetup Ram TextRight", false);
                }
                else
                {
                    this.LabRamGame.Margin = new Thickness(actualWidth2 - actualWidth3, 3.0, 0.0, 0.0);
                    this.LabRamGameTitle.Margin = new Thickness(actualWidth2 - actualWidth6, 0.0, 0.0, 5.0);
                }
            }
            else if (ModAnimation.AniControlEnabled == 0 && (this.RamTextRight != num2 || ModAnimation.AniIsRun("VersionSetup Ram TextRight")))
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaX(this.LabRamGame, 2.0 + actualWidth - this.LabRamGame.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaX(this.LabRamGameTitle, 2.0 + actualWidth - this.LabRamGameTitle.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                }, "VersionSetup Ram TextRight", false);
            }
            else
            {
                this.LabRamGame.Margin = new Thickness(2.0 + actualWidth, 3.0, 0.0, 0.0);
                this.LabRamGameTitle.Margin = new Thickness(2.0 + actualWidth, 0.0, 0.0, 5.0);
            }
            this.RamTextRight = num2;
        }

        public static double GetRam(ModMinecraft.McInstance Instance)
        {
            double num;
            if (Settings.Get<int>("VersionRamType", Instance) == 2)
            {
                num = PageSetupLaunch.GetRam(Instance, true);
            }
            else
            {
                double num9 = default;
                if (Settings.Get<int>("VersionRamType", Instance) == 0)
                {
                    double num2 = Math.Round(OurWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0;
                    if (((object)Instance != (object)null) && !Instance.IsLoaded)
                    {
                        Instance.Load();
                    }
                    double num4;
                    double num5;
                    double num6;
                    double num7;
                    if (((object)Instance != (object)null) && Instance.Modable)
                    {
                        DirectoryInfo info = DirectoryUtils.GetInfo(Instance.PathIndie + "mods\\");
                        int num3 = (info.Exists ? info.GetFiles().Count<FileInfo>((PageInstanceSetup._Closure___._I12_0 == null) ? (PageInstanceSetup._Closure___._I12_0 = new Func<FileInfo, bool>(PageInstanceSetup._Closure___._I._Lambda___12_0)) : PageInstanceSetup._Closure___._I12_0) : 0);
                        num4 = 0.5 + (double)num3 / 150.0;
                        num5 = 1.5 + (double)num3 / 90.0;
                        num6 = 2.7 + (double)num3 / 50.0;
                        num7 = 4.5 + (double)num3 / 25.0;
                    }
                    else if (((object)Instance != (object)null) && Instance.Version.HasOptiFine)
                    {
                        num4 = 0.5;
                        num5 = 1.5;
                        num6 = 3.0;
                        num7 = 5.0;
                    }
                    else
                    {
                        num4 = 0.5;
                        num5 = 1.5;
                        num6 = 2.5;
                        num7 = 4.0;
                    }
                    double num8 = num5;
                    num9 += Math.Min(num2, num8);
                    num2 -= num8;
                    if (num2 >= 0.1)
                    {
                        num8 = num6 - num5;
                        num9 += Math.Min(num2 * 0.7, num8);
                        num2 -= num8 / 0.7;
                        if (num2 >= 0.1)
                        {
                            num8 = num7 - num6;
                            num9 += Math.Min(num2 * 0.4, num8);
                            num2 -= num8 / 0.4;
                            if (num2 >= 0.1)
                            {
                                num8 = num7;
                                num9 += Math.Min(num2 * 0.15, num8);
                                num2 -= num8 / 0.15;
                            }
                        }
                    }
                    num9 = Math.Round(Math.Max(num9, num4), 1);
                }
                else
                {
                    int num10 = Settings.Get<int>("VersionRamCustom", Instance);
                    if (num10 <= 12)
                    {
                        num9 = (double)num10 * 0.1 + 0.3;
                    }
                    else if (num10 <= 25)
                    {
                        num9 = (double)(checked(num10 - 12)) * 0.5 + 1.5;
                    }
                    else if (num10 <= 33)
                    {
                        num9 = (double)(checked((num10 - 25) * 1 + 8));
                    }
                    else
                    {
                        num9 = (double)(checked((num10 - 33) * 2 + 16));
                    }
                }
                num = num9;
            }
            return num;
        }

        public static void OnVersionServerLoginChanged(int Type)
        {
            if (ModMain.FrmInstanceSetup != null)
            {
                ModBase.WriteIni(ModMinecraft.McFolderSelected + "PCL.ini", "InstanceCache", "");
                if (((object)(PageInstanceLeft.Instance) != (object)null))
                {
                    PageInstanceLeft.Instance = new ModMinecraft.McInstance(PageInstanceLeft.Instance.Name).Load();
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", false);
                }
            }
        }

        private void TextServerEnter_Change()
        {
            string text = this.TextServerEnter.Text.Replace("：", ":").Replace("。", ".");
            if (Operators.CompareString(text, this.TextServerEnter.Text, false) != 0)
            {
                int selectionStart = this.TextServerEnter.SelectionStart;
                this.TextServerEnter.Text = text;
                this.TextServerEnter.SelectionStart = selectionStart;
            }
        }

        private void ComboServerLogin_Changed()
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                this.UpdateServerLoginUI();
                if ((this.ComboServerLogin.SelectedIndex != 3 || this.TextServerNide.IsValidated) && (this.ComboServerLogin.SelectedIndex != 4 || this.TextServerAuthServer.IsValidated) && this.ComboServerLoginLast != this.ComboServerLogin.SelectedIndex)
                {
                    this.ComboServerLoginLast = this.ComboServerLogin.SelectedIndex;
                    Settings.Set("VersionServerLogin", this.ComboServerLogin.SelectedIndex, PageInstanceLeft.Instance);
                }
            }
        }

        private void UpdateServerLoginUI()
        {
            if (this.LabServerNide != null)
            {
                int selectedIndex = this.ComboServerLogin.SelectedIndex;
                this.LabServerNide.Visibility = ((selectedIndex == 3) ? Visibility.Visible : Visibility.Collapsed);
                this.TextServerNide.Visibility = ((selectedIndex == 3) ? Visibility.Visible : Visibility.Collapsed);
                this.PanServerNide.Visibility = ((selectedIndex == 3) ? Visibility.Visible : Visibility.Collapsed);
                this.LabServerAuthName.Visibility = ((selectedIndex == 4) ? Visibility.Visible : Visibility.Collapsed);
                this.TextServerAuthName.Visibility = ((selectedIndex == 4) ? Visibility.Visible : Visibility.Collapsed);
                this.LabServerAuthRegister.Visibility = ((selectedIndex == 4) ? Visibility.Visible : Visibility.Collapsed);
                this.TextServerAuthRegister.Visibility = ((selectedIndex == 4) ? Visibility.Visible : Visibility.Collapsed);
                this.LabServerAuthServer.Visibility = ((selectedIndex == 4) ? Visibility.Visible : Visibility.Collapsed);
                this.TextServerAuthServer.Visibility = ((selectedIndex == 4) ? Visibility.Visible : Visibility.Collapsed);
                this.BtnServerAuthLittle.Visibility = ((selectedIndex == 4) ? Visibility.Visible : Visibility.Collapsed);
                this.CardServer.TriggerForceResize();
            }
        }

        private void BtnServerAuthLittle_Click(object sender, EventArgs e)
        {
            if (Operators.CompareString(this.TextServerAuthServer.Text, "", false) == 0 || Operators.CompareString(this.TextServerAuthServer.Text, "https://littleskin.cn/api/yggdrasil", false) == 0 || ModMain.OurMsgBox("即将把第三方登录设置覆盖为 LittleSkin 登录。\r\n除非你是服主，或者服主要求你这样做，否则请不要继续。\r\n\r\n是否确实需要覆盖当前设置？", "设置覆盖确认", "继续", "取消", "", false, true, false, null, null, null) != 2)
            {
                this.TextServerAuthServer.Text = "https://littleskin.cn/api/yggdrasil";
                this.TextServerAuthRegister.Text = "https://littleskin.cn/auth/register";
                this.TextServerAuthName.Text = "LittleSkin 登录";
            }
        }

        private void ReloadCurrentJava()
        {
            PageInstanceLeft.ReloadCurrentJava();
        }

        private void UpdateJavaRelatedUi()
        {
            ModBase.RunInUi(new Action(this._UpdateJavaRelatedUi), false);
        }

        private void _UpdateJavaRelatedUi()
        {
            RedoableWorker<Java> currentJavaWorker = PageInstanceLeft.CurrentJavaWorker;
            switch (this.ComboArgumentJava.SelectedIndex)
            {
            case 0:
            case 2:
                this.HintArgumentJava.Visibility = Visibility.Visible;
                this.TextArgumentJavaRange.Visibility = Visibility.Collapsed;
                this.PanArgumentJavaSelect.Visibility = Visibility.Collapsed;
                if (!currentJavaWorker.LastSucceeded || currentJavaWorker.Running)
                {
                    if (!currentJavaWorker.Running)
                    {
                        currentJavaWorker.Start(default(CancellationToken), null);
                    }
                    this.HintArgumentJava.Text = "正在查找 Java……";
                    this.HintArgumentJava.Theme = OurHint.Themes.Blue;
                    return;
                }
                if (currentJavaWorker.LastResult != null)
                {
                    this.HintArgumentJava.Text = string.Format("将会使用：{0}", currentJavaWorker.LastResult);
                    this.HintArgumentJava.Theme = OurHint.Themes.Blue;
                    return;
                }
                if (this.ComboArgumentJava.SelectedIndex == 0)
                {
                    this.HintArgumentJava.Text = "你的电脑上没有可供该版本使用的 Java，PCL 会在启动游戏时自动下载。";
                    this.HintArgumentJava.Theme = OurHint.Themes.Yellow;
                    return;
                }
                this.HintArgumentJava.Text = string.Format("该版本的版本文件夹中没有发现任何 Java！{0}请点击此处打开版本文件夹，然后将 Java 文件夹复制进去。", "\r\n");
                this.HintArgumentJava.Theme = OurHint.Themes.Red;
                return;
            case 1:
                this.HintArgumentJava.Visibility = Visibility.Visible;
                this.TextArgumentJavaRange.Visibility = Visibility.Visible;
                this.PanArgumentJavaSelect.Visibility = Visibility.Collapsed;
                if (!Settings.HasSaved("VersionArgumentJavaRange", PageInstanceLeft.Instance))
                {
                    string text = ModJava.GetJavaRequirement(PageInstanceLeft.Instance).Item1.ToString().Replace("-∞", "").Replace("+∞", "");
                    Settings.Set("VersionArgumentJavaRange", text, PageInstanceLeft.Instance);
                    this.TextArgumentJavaRange.Text = text;
                }
                if (!string.IsNullOrEmpty(this.TextArgumentJavaRange.ValidateResult))
                {
                    this.HintArgumentJava.Text = string.Format("Java 版本范围错误：{0}", this.TextArgumentJavaRange.ValidateResult);
                    this.HintArgumentJava.Theme = OurHint.Themes.Red;
                    return;
                }
                if (!currentJavaWorker.LastSucceeded || currentJavaWorker.Running)
                {
                    if (!currentJavaWorker.Running)
                    {
                        currentJavaWorker.Start(default(CancellationToken), null);
                    }
                    this.HintArgumentJava.Text = "正在查找 Java……";
                    this.HintArgumentJava.Theme = OurHint.Themes.Blue;
                    return;
                }
                if (currentJavaWorker.LastResult == null)
                {
                    this.HintArgumentJava.Text = string.Format("你的电脑上没有任何 Java 符合该版本范围！{0}如果 Mojang 提供了对应版本的 Java，PCL 会在启动游戏时自动下载。", "\r\n");
                    this.HintArgumentJava.Theme = OurHint.Themes.Yellow;
                    return;
                }
                this.HintArgumentJava.Text = string.Format("将会使用：{0}", currentJavaWorker.LastResult);
                this.HintArgumentJava.Theme = OurHint.Themes.Blue;
                return;
            case 3:
                this.HintArgumentJava.Visibility = Visibility.Collapsed;
                this.TextArgumentJavaRange.Visibility = Visibility.Collapsed;
                this.PanArgumentJavaSelect.Visibility = Visibility.Visible;
                return;
            default:
                return;
            }
        }

        private void HintArgumentJava_Click(object sender, MouseButtonEventArgs e)
        {
            if (this.ComboArgumentJava.SelectedIndex == 2)
            {
                PageInstanceOverall.OpenInstanceFolder(PageInstanceLeft.Instance);
            }
        }

        public void UpdateJavaList()
        {
            if (this.ComboArgumentJavaSelect != null && this.LabArgumentJava != null && this.ComboArgumentJavaSelect.IsLoaded)
            {
                this.ComboArgumentJavaSelect.Items.Clear();
                if (ModJava.JavaListRefreshWorker.Running)
                {
                    this.BtnArgumentJavaSearch.IsEnabled = false;
                    this.ComboArgumentJavaSelect.IsEnabled = false;
                    this.LabArgumentJava.Text = "搜索中\u200a…";
                    return;
                }
                this.BtnArgumentJavaSearch.IsEnabled = true;
                this.ComboArgumentJavaSelect.IsEnabled = true;
                Java java = Configs.JavaForced.Get(PageInstanceLeft.Instance.Config);
                if (!Configs.JavaList.Get().Any<Java>())
                {
                    this.LabArgumentJava.Text = "未找到 Java，点击以导入已有的 Java";
                }
                else if (java == null)
                {
                    this.LabArgumentJava.Text = "点击选择 Java\u200a…";
                }
                else
                {
                    this.LabArgumentJava.Text = java.ToString();
                }
                try
                {
                    try
                    {
                        IEnumerator<Java> enumerator = Configs.JavaList.Get().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            PageInstanceSetup._Closure___23_0 CS_locals1 = new PageInstanceSetup._Closure___23_0(null);
                            CS_locals1._VB_Me = this;
                            CS_locals1._VB_Local_JavaEntry = enumerator.Current;
                            OurListItem ourListItem = new OurListItem
                            {
                                FontSize = 13.0,
                                Height = 24.0,
                                IsScaleAnimationEnabled = false,
                                Type = OurListItem.CheckType.Clickable,
                                Tag = CS_locals1._VB_Local_JavaEntry,
                                Title = CS_locals1._VB_Local_JavaEntry.ToString()
                            };
                            ourListItem.MouseLeftButtonUp += ((PageInstanceSetup._Closure___._I23_0 == null) ? (PageInstanceSetup._Closure___._I23_0 = new MouseButtonEventHandler(PageInstanceSetup._Closure___._I._Lambda___23_0)) : PageInstanceSetup._Closure___._I23_0);
                            ourListItem.Click += CS_locals1._Lambda___R2;
                            this.ComboArgumentJavaSelect.Items.Add(ourListItem);
                            List<OurIconButton> list = new List<OurIconButton>();
                            bool flag = PathUtils.IsParentOf(string.Format("{0}.minecraft\\runtime\\", Paths.AppData), CS_locals1._VB_Local_JavaEntry.Folder);
                            OurIconButton ourIconButton = new OurIconButton
                            {
                                Logo = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z",
                                Height = 24.0,
                                Width = 24.0
                            };
                            ourIconButton.IsEnabled = !flag;
                            ourIconButton.ToolTip = (ourIconButton.IsEnabled ? "从列表中移除" : "无法移除官方 Java");
                            ourIconButton.Click += CS_locals1._Lambda___R3;
                            ToolTipService.SetShowOnDisabled(ourIconButton, true);
                            list.Add(ourIconButton);
                            OurIconButton ourIconButton2 = new OurIconButton
                            {
                                Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z",
                                LogoScale = 1.1,
                                Height = 24.0,
                                Width = 24.0
                            };
                            ourIconButton2.ToolTip = "打开文件夹";
                            ourIconButton2.Click += CS_locals1._Lambda___R4;
                            list.Add(ourIconButton2);
                            ourListItem.Buttons = list;
                        }
                    }
                    finally
                    {
                        IEnumerator<Java> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    OurListItem ourListItem2 = new OurListItem
                    {
                        FontSize = 13.0,
                        Height = 24.0,
                        IsScaleAnimationEnabled = false,
                        Title = "\u200a导入电脑中已有的 Java…",
                        Type = OurListItem.CheckType.Clickable
                    };
                    ourListItem2.MouseLeftButtonUp += ((PageInstanceSetup._Closure___._I23_4 == null) ? (PageInstanceSetup._Closure___._I23_4 = new MouseButtonEventHandler(PageInstanceSetup._Closure___._I._Lambda___23_4)) : PageInstanceSetup._Closure___._I23_4);
                    this.ComboArgumentJavaSelect.Items.Add(ourListItem2);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "更新 Java 下拉框失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceSetup.xaml.vb");
                }
            }
        }

        private void BtnAdvanceJavaSearch_Click()
        {
            ModJava.ManuallySearchJava(default(CancellationToken));
        }

        private void ComboArgumentIndieV2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                if (this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init, ref flag);
                    if (this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init.State == 0)
                    {
                        this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init.State = 2;
                        this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting = false;
                    }
                    else if (this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init);
                    }
                }
                if (!this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting)
                {
                    if (ModMain.OurMsgBox("调整版本隔离设置后，你需要游戏存档、Mod 等文件手动迁移到新的游戏文件夹中。\r\n如果发现存档消失，把这项设置改回来就能恢复。\r\n如果你不会迁移存档，不建议修改这项设置！", "警告", "我知道我在做什么", "取消", "", true, true, false, null, null, null) == 2)
                    {
                        this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting = true;
                        if (e.RemovedItems.Count > 0)
                        {
                            this.ComboArgumentIndieV2.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems[0]);
                        }
                        this._STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting = false;
                    }
                    Settings.Set("VersionArgumentIndieV2", RuntimeHelpers.GetObjectValue(Operators.CompareObjectEqual(NewLateBinding.LateGet(sender, null, "SelectedIndex", new object[0], null, null, null), 0, false)), PageInstanceLeft.Instance);
                }
            }
        }

        private void TextAdvanceRun_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.CheckAdvanceRunWait.Visibility = (Operators.CompareString(this.TextAdvanceRun.Text, "", false) != 0).ToVisibility();
        }

        private void BtnSwitch_Click(object sender, MouseButtonEventArgs e)
        {
            ModMain.FrmMain.PageChange(FormMain.PageType.Setup, FormMain.PageSubType.Default);
        }

        private void ReplaceEnter(OurTextBox sender, TextChangedEventArgs e)
        {
            string text = sender.Text.ReplaceLineEndings(" ", true);
            if (Operators.CompareString(text, sender.Text, false) != 0)
            {
                int caretIndex = sender.CaretIndex;
                sender.Text = text;
                sender.CaretIndex = Math.Max(0, checked(caretIndex - 1));
            }
        }




        internal virtual OurComboBox ComboArgumentIndieV2_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboArgumentIndieV2;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboArgumentIndieV2_SelectionChanged);
                OurComboBox ourComboBox = this._ComboArgumentIndieV2;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboArgumentIndieV2 = value;
                ourComboBox = this._ComboArgumentIndieV2;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }



        internal virtual OurComboBox ComboArgumentJava_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboArgumentJava;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this._Lambda___R55_5);
                SelectionChangedEventHandler selectionChangedEventHandler2 = new SelectionChangedEventHandler(this._Lambda___R55_6);
                OurComboBox ourComboBox = this._ComboArgumentJava;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler2;
                }
                this._ComboArgumentJava = value;
                ourComboBox = this._ComboArgumentJava;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                    ourComboBox.SelectionChanged += selectionChangedEventHandler2;
                }
            }
        }

        internal virtual OurTextBox TextArgumentJavaRange_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextArgumentJavaRange;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R59_7);
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this._Lambda___R59_8);
                OurTextBox ourTextBox = this._TextArgumentJavaRange;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged -= routedEventHandler;
                    ourTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextArgumentJavaRange = value;
                ourTextBox = this._TextArgumentJavaRange;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged += routedEventHandler;
                    ourTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual OurHint HintArgumentJava_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._HintArgumentJava;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurHint.ClickEventHandler clickEventHandler = new OurHint.ClickEventHandler(this.HintArgumentJava_Click);
                OurHint ourHint = this._HintArgumentJava;
                if (ourHint != null)
                {
                    ourHint.Click -= clickEventHandler;
                }
                this._HintArgumentJava = value;
                ourHint = this._HintArgumentJava;
                if (ourHint != null)
                {
                    ourHint.Click += clickEventHandler;
                }
            }
        }




        internal virtual OurIconButton BtnArgumentJavaSearch_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnArgumentJavaSearch;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this._Lambda___R79_9);
                OurIconButton ourIconButton = this._BtnArgumentJavaSearch;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnArgumentJavaSearch = value;
                ourIconButton = this._BtnArgumentJavaSearch;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurRadioBox RadioRamType2_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RadioRamType2;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R83_10);
                OurRadioBox ourRadioBox = this._RadioRamType2;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Check -= checkEventHandler;
                }
                this._RadioRamType2 = value;
                ourRadioBox = this._RadioRamType2;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurRadioBox RadioRamType0_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RadioRamType0;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R87_11);
                OurRadioBox ourRadioBox = this._RadioRamType0;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Check -= checkEventHandler;
                }
                this._RadioRamType0 = value;
                ourRadioBox = this._RadioRamType0;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurRadioBox RadioRamType1_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RadioRamType1;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R91_12);
                OurRadioBox ourRadioBox = this._RadioRamType1;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Check -= checkEventHandler;
                }
                this._RadioRamType1 = value;
                ourRadioBox = this._RadioRamType1;
                if (ourRadioBox != null)
                {
                    ourRadioBox.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurSlider SliderRamCustom_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._SliderRamCustom;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurSlider.ChangeEventHandler changeEventHandler = new OurSlider.ChangeEventHandler(this._Lambda___R95_13);
                OurSlider ourSlider = this._SliderRamCustom;
                if (ourSlider != null)
                {
                    ourSlider.Change -= changeEventHandler;
                }
                this._SliderRamCustom = value;
                ourSlider = this._SliderRamCustom;
                if (ourSlider != null)
                {
                    ourSlider.Change += changeEventHandler;
                }
            }
        }







        internal virtual Rectangle RectRamGame_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RectRamGame;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R123_14);
                Rectangle rectangle = this._RectRamGame;
                if (rectangle != null)
                {
                    rectangle.SizeChanged -= sizeChangedEventHandler;
                }
                this._RectRamGame = value;
                rectangle = this._RectRamGame;
                if (rectangle != null)
                {
                    rectangle.SizeChanged += sizeChangedEventHandler;
                }
            }
        }

        internal virtual Rectangle RectRamEmpty_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RectRamEmpty;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R127_15);
                Rectangle rectangle = this._RectRamEmpty;
                if (rectangle != null)
                {
                    rectangle.SizeChanged -= sizeChangedEventHandler;
                }
                this._RectRamEmpty = value;
                rectangle = this._RectRamEmpty;
                if (rectangle != null)
                {
                    rectangle.SizeChanged += sizeChangedEventHandler;
                }
            }
        }





        internal virtual TextBlock LabRamGame_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._LabRamGame;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R147_16);
                TextBlock textBlock = this._LabRamGame;
                if (textBlock != null)
                {
                    textBlock.SizeChanged -= sizeChangedEventHandler;
                }
                this._LabRamGame = value;
                textBlock = this._LabRamGame;
                if (textBlock != null)
                {
                    textBlock.SizeChanged += sizeChangedEventHandler;
                }
            }
        }


        internal virtual OurComboBox ComboServerLogin_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboServerLogin;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this._Lambda___R155_17);
                OurComboBox ourComboBox = this._ComboServerLogin;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboServerLogin = value;
                ourComboBox = this._ComboServerLogin;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }


        internal virtual OurTextBox TextServerNide_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextServerNide;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R163_18);
                OurTextBox ourTextBox = this._TextServerNide;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged -= routedEventHandler;
                }
                this._TextServerNide = value;
                ourTextBox = this._TextServerNide;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged += routedEventHandler;
                }
            }
        }


        internal virtual OurTextBox TextServerAuthServer_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextServerAuthServer;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R171_19);
                OurTextBox ourTextBox = this._TextServerAuthServer;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged -= routedEventHandler;
                }
                this._TextServerAuthServer = value;
                ourTextBox = this._TextServerAuthServer;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged += routedEventHandler;
                }
            }
        }


        internal virtual OurTextBox TextServerAuthRegister_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextServerAuthRegister;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R179_20);
                OurTextBox ourTextBox = this._TextServerAuthRegister;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged -= routedEventHandler;
                }
                this._TextServerAuthRegister = value;
                ourTextBox = this._TextServerAuthRegister;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged += routedEventHandler;
                }
            }
        }



        internal virtual OurTextBox TextServerEnter_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextServerEnter;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this._Lambda___R191_21);
                OurTextBox ourTextBox = this._TextServerEnter;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextServerEnter = value;
                ourTextBox = this._TextServerEnter;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }


        internal virtual OurButton BtnServerAuthLittle_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnServerAuthLittle;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnServerAuthLittle_Click);
                OurButton ourButton = this._BtnServerAuthLittle;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnServerAuthLittle = value;
                ourButton = this._BtnServerAuthLittle;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurTextBox TextAdvanceJvm_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextAdvanceJvm;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this._Lambda___R207_22);
                OurTextBox ourTextBox = this._TextAdvanceJvm;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceJvm = value;
                ourTextBox = this._TextAdvanceJvm;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual OurTextBox TextAdvanceGame_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextAdvanceGame;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this._Lambda___R211_23);
                OurTextBox ourTextBox = this._TextAdvanceGame;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceGame = value;
                ourTextBox = this._TextAdvanceGame;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual OurTextBox TextAdvanceRun_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._TextAdvanceRun;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this.TextAdvanceRun_TextChanged);
                OurTextBox ourTextBox = this._TextAdvanceRun;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceRun = value;
                ourTextBox = this._TextAdvanceRun;
                if (ourTextBox != null)
                {
                    ourTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }





        internal virtual OurExtraTextButton BtnSwitch_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSwitch;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurExtraTextButton.ClickEventHandler clickEventHandler = new OurExtraTextButton.ClickEventHandler(this.BtnSwitch_Click);
                OurExtraTextButton ourExtraTextButton = this._BtnSwitch;
                if (ourExtraTextButton != null)
                {
                    ourExtraTextButton.Click -= clickEventHandler;
                }
                this._BtnSwitch = value;
                ourExtraTextButton = this._BtnSwitch;
                if (ourExtraTextButton != null)
                {
                    ourExtraTextButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R2_1(object a0, EventArgs a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R55_5(object a0, SelectionChangedEventArgs a1)
        {
            this.ReloadCurrentJava();
        }

        [CompilerGenerated]
        private void _Lambda___R55_6(object a0, SelectionChangedEventArgs a1)
        {
            this.UpdateJavaRelatedUi();
        }

        [CompilerGenerated]
        private void _Lambda___R59_7(object a0, RoutedEventArgs a1)
        {
            this.ReloadCurrentJava();
        }

        [CompilerGenerated]
        private void _Lambda___R59_8(object a0, TextChangedEventArgs a1)
        {
            this.UpdateJavaRelatedUi();
        }

        [CompilerGenerated]
        private void _Lambda___R79_9(object a0, EventArgs a1)
        {
            this.BtnAdvanceJavaSearch_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R83_10(object a0, ModBase.RouteEventArgs a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R87_11(object a0, ModBase.RouteEventArgs a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R91_12(object a0, ModBase.RouteEventArgs a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R95_13(object a0, bool a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R123_14(object a0, SizeChangedEventArgs a1)
        {
            this.RefreshRamText();
        }

        [CompilerGenerated]
        private void _Lambda___R127_15(object a0, SizeChangedEventArgs a1)
        {
            this.RefreshRamText();
        }

        [CompilerGenerated]
        private void _Lambda___R147_16(object a0, SizeChangedEventArgs a1)
        {
            this.RefreshRamText();
        }

        [CompilerGenerated]
        private void _Lambda___R155_17(object a0, SelectionChangedEventArgs a1)
        {
            this.ComboServerLogin_Changed();
        }

        [CompilerGenerated]
        private void _Lambda___R163_18(object a0, RoutedEventArgs a1)
        {
            this.ComboServerLogin_Changed();
        }

        [CompilerGenerated]
        private void _Lambda___R171_19(object a0, RoutedEventArgs a1)
        {
            this.ComboServerLogin_Changed();
        }

        [CompilerGenerated]
        private void _Lambda___R179_20(object a0, RoutedEventArgs a1)
        {
            this.ComboServerLogin_Changed();
        }

        [CompilerGenerated]
        private void _Lambda___R191_21(object a0, TextChangedEventArgs a1)
        {
            this.TextServerEnter_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R207_22(object a0, TextChangedEventArgs a1)
        {
            this.ReplaceEnter((OurTextBox)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R211_23(object a0, TextChangedEventArgs a1)
        {
            this.ReplaceEnter((OurTextBox)a0, a1);
        }

        private bool LoadedFirst;

        private int RamTextLeft;

        private int RamTextRight;

        private int ComboServerLoginLast;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurScrollViewer _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("CardArgument")]
        private OurCard _CardArgument;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboArgumentIndieV2")]
        private OurComboBox _ComboArgumentIndieV2;

        [CompilerGenerated]
        [AccessedThroughProperty("TextArgumentTitle")]
        private OurTextBox _TextArgumentTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("TextArgumentInfo")]
        private OurTextBox _TextArgumentInfo;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboArgumentJava")]
        private OurComboBox _ComboArgumentJava;

        [CompilerGenerated]
        [AccessedThroughProperty("TextArgumentJavaRange")]
        private OurTextBox _TextArgumentJavaRange;

        [CompilerGenerated]
        [AccessedThroughProperty("HintArgumentJava")]
        private OurHint _HintArgumentJava;

        [CompilerGenerated]
        [AccessedThroughProperty("PanArgumentJavaSelect")]
        private Grid _PanArgumentJavaSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboArgumentJavaSelect")]
        private OurComboBox _ComboArgumentJavaSelect;

        [CompilerGenerated]
        [AccessedThroughProperty("LabArgumentJava")]
        private TextBlock _LabArgumentJava;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnArgumentJavaSearch")]
        private OurIconButton _BtnArgumentJavaSearch;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioRamType2")]
        private OurRadioBox _RadioRamType2;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioRamType0")]
        private OurRadioBox _RadioRamType0;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioRamType1")]
        private OurRadioBox _RadioRamType1;

        [CompilerGenerated]
        [AccessedThroughProperty("SliderRamCustom")]
        private OurSlider _SliderRamCustom;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboRamOptimize")]
        private OurComboBox _ComboRamOptimize;

        [CompilerGenerated]
        [AccessedThroughProperty("PanRamDisplay")]
        private Grid _PanRamDisplay;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnRamUsed")]
        private ColumnDefinition _ColumnRamUsed;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnRamGame")]
        private ColumnDefinition _ColumnRamGame;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnRamEmpty")]
        private ColumnDefinition _ColumnRamEmpty;

        [CompilerGenerated]
        [AccessedThroughProperty("RectRamUsed")]
        private Rectangle _RectRamUsed;

        [CompilerGenerated]
        [AccessedThroughProperty("RectRamGame")]
        private Rectangle _RectRamGame;

        [CompilerGenerated]
        [AccessedThroughProperty("RectRamEmpty")]
        private Rectangle _RectRamEmpty;

        [CompilerGenerated]
        [AccessedThroughProperty("LabRamUsedTitle")]
        private TextBlock _LabRamUsedTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabRamGameTitle")]
        private TextBlock _LabRamGameTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabRamUsed")]
        private TextBlock _LabRamUsed;

        [CompilerGenerated]
        [AccessedThroughProperty("LabRamTotal")]
        private TextBlock _LabRamTotal;

        [CompilerGenerated]
        [AccessedThroughProperty("LabRamGame")]
        private TextBlock _LabRamGame;

        [CompilerGenerated]
        [AccessedThroughProperty("CardServer")]
        private OurCard _CardServer;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboServerLogin")]
        private OurComboBox _ComboServerLogin;

        [CompilerGenerated]
        [AccessedThroughProperty("LabServerNide")]
        private TextBlock _LabServerNide;

        [CompilerGenerated]
        [AccessedThroughProperty("TextServerNide")]
        private OurTextBox _TextServerNide;

        [CompilerGenerated]
        [AccessedThroughProperty("LabServerAuthServer")]
        private TextBlock _LabServerAuthServer;

        [CompilerGenerated]
        [AccessedThroughProperty("TextServerAuthServer")]
        private OurTextBox _TextServerAuthServer;

        [CompilerGenerated]
        [AccessedThroughProperty("LabServerAuthRegister")]
        private TextBlock _LabServerAuthRegister;

        [CompilerGenerated]
        [AccessedThroughProperty("TextServerAuthRegister")]
        private OurTextBox _TextServerAuthRegister;

        [CompilerGenerated]
        [AccessedThroughProperty("LabServerAuthName")]
        private TextBlock _LabServerAuthName;

        [CompilerGenerated]
        [AccessedThroughProperty("TextServerAuthName")]
        private OurTextBox _TextServerAuthName;

        [CompilerGenerated]
        [AccessedThroughProperty("TextServerEnter")]
        private OurTextBox _TextServerEnter;

        [CompilerGenerated]
        [AccessedThroughProperty("PanServerNide")]
        private Grid _PanServerNide;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnServerAuthLittle")]
        private OurButton _BtnServerAuthLittle;

        [CompilerGenerated]
        [AccessedThroughProperty("CardAdvance")]
        private OurCard _CardAdvance;

        [CompilerGenerated]
        [AccessedThroughProperty("TextAdvanceJvm")]
        private OurTextBox _TextAdvanceJvm;

        [CompilerGenerated]
        [AccessedThroughProperty("TextAdvanceGame")]
        private OurTextBox _TextAdvanceGame;

        [CompilerGenerated]
        [AccessedThroughProperty("TextAdvanceRun")]
        private OurTextBox _TextAdvanceRun;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckAdvanceRunWait")]
        private OurCheckBox _CheckAdvanceRunWait;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckAdvanceDisableModUpdate")]
        private OurCheckBox _CheckAdvanceDisableModUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckAdvanceAssetsV2")]
        private OurCheckBox _CheckAdvanceAssetsV2;

        [CompilerGenerated]
        [AccessedThroughProperty("CheckAdvanceDisableJLW")]
        private OurCheckBox _CheckAdvanceDisableJLW;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSwitch")]
        private OurExtraTextButton _BtnSwitch;

        private bool _STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting;

        private StaticLocalInitFlag _STATIC_ComboArgumentIndieV2_SelectionChanged_20211C128135_IsReverting_Init;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal string _Lambda___2_0(string Str)
            {
                string text;
                try
                {
                    if (string.IsNullOrWhiteSpace(Str))
                    {
                        text = "不能为空";
                    }
                    else
                    {
                        ValueRange<Version> valueRange = ValueRange<Version>.FromString(Str, (PageInstanceSetup._Closure___._I2_1 == null) ? (PageInstanceSetup._Closure___._I2_1 = new Func<string, Version>(PageInstanceSetup._Closure___._I._Lambda___2_1)) : PageInstanceSetup._Closure___._I2_1);
                        if (valueRange.IsEmpty())
                        {
                            text = "范围下限的版本号比上限更高，导致该范围无法匹配到任何版本";
                        }
                        else if (valueRange.HasLower && valueRange.Lower.Major <= 1 && (valueRange.Lower.Minor > 0 || valueRange.Lower.Build > 0 || valueRange.Lower.Revision > 0))
                        {
                            text = "不应使用 1.x 格式进行匹配（例如，若想匹配 Java 17，应填写 17 而非 1.17）";
                        }
                        else if (valueRange.HasUpper && valueRange.Upper.Major <= 1 && (valueRange.Upper.Minor > 0 || valueRange.Upper.Build > 0 || valueRange.Upper.Revision > 0))
                        {
                            text = "不应使用 1.x 格式进行匹配（例如，若想匹配 Java 17，应填写 17 而非 1.17）";
                        }
                        else if (valueRange.HasUpper && valueRange.Upper.Major <= 4)
                        {
                            text = "该范围所要求的 Java 版本过低";
                        }
                        else if (valueRange.HasUpper && valueRange.IsUpperInclusive && valueRange.Upper.Minor <= 0 && valueRange.Upper.Build <= 0)
                        {
                            text = string.Format("右侧闭区间的意图并不明确。如果不想允许 Java {0}，请改为 {1}{2}){3}。如果想允许 Java {4}，请改为 {5}{6}){7}。", new object[]
                            {
                                valueRange.Upper.Major,
                                ModBase.vbLQ,
                                valueRange.Upper.Major,
                                ModBase.vbRQ,
                                valueRange.Upper.Major,
                                ModBase.vbLQ,
                                checked(valueRange.Upper.Major + 1),
                                ModBase.vbRQ
                            });
                        }
                        else if (valueRange.Intersect(ValueRange<Version>.OpenClosed(new Version(5, 0), new Version(99, 0))).IsEmpty())
                        {
                            text = "该范围无法匹配到任何常见的 Java";
                        }
                        else
                        {
                            text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    text = ex.Message;
                }
                return text;
            }

            internal Version _Lambda___2_1(string s)
            {
                return StringUtils.ParseVersionWithDefaults(s);
            }

            internal bool _Lambda___12_0(FileInfo f)
            {
                return new string[] { ".jar", ".zip", ".litemod" }.Contains(f.Extension.Lower());
            }

            internal void _Lambda___23_0(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
            }

            internal void _Lambda___23_4(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                ModJava.ManuallyImportJava();
            }

            public static readonly PageInstanceSetup._Closure___ _I = new PageInstanceSetup._Closure___();

            public static Func<string, Version> _I2_1;

            public static Func<string, string> _I2_0;

            public static Func<FileInfo, bool> _I12_0;

            public static MouseButtonEventHandler _I23_0;

            public static MouseButtonEventHandler _I23_4;
        }

        [CompilerGenerated]
        private sealed class _Closure___23_0
        {
            public _Closure___23_0(PageInstanceSetup._Closure___23_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_JavaEntry = arg0._VB_Local_JavaEntry;
                }
            }

            internal void _Lambda___R2(object a0, MouseButtonEventArgs a1)
            {
                this._Lambda___1();
            }

            internal void _Lambda___1()
            {
                Configs.JavaForced.Set(this._VB_Local_JavaEntry, PageInstanceLeft.Instance.Config);
                this._VB_Me.UpdateJavaList();
                this._VB_Me.ComboArgumentJavaSelect.IsDropDownOpen = false;
            }

            internal void _Lambda___R3(object a0, EventArgs a1)
            {
                this._Lambda___2();
            }

            internal void _Lambda___2()
            {
                ModJava.ManuallyRemoveJava(this._VB_Local_JavaEntry);
            }

            internal void _Lambda___R4(object a0, EventArgs a1)
            {
                this._Lambda___3();
            }

            internal void _Lambda___3()
            {
                ModBase.OpenExplorer(this._VB_Local_JavaEntry.JavaExePath);
            }

            public Java _VB_Local_JavaEntry;

            public PageInstanceSetup _VB_Me;
        }
    }
}
