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
using PCL.My;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceSetup : MyPageRight
    {
        public PageInstanceSetup()
        {
            base.Loaded += this.PageSetupSystem_Loaded;
            this.LoadedFirst = false;
            this.RamTextLeft = 2;
            this.RamTextRight = 1;
            this.InitializeComponent();
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
                        new ValidateFunc((PageInstanceSetup._Closure$__.$I2-0 == null) ? (PageInstanceSetup._Closure$__.$I2-0 = delegate(string Str)
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
                                    ValueRange<Version> valueRange = ValueRange<Version>.FromString(Str, (PageInstanceSetup._Closure$__.$I2-1 == null) ? (PageInstanceSetup._Closure$__.$I2-1 = (string s) => StringUtils.ParseVersionWithDefaults(s)) : PageInstanceSetup._Closure$__.$I2-1);
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
                                            valueRange.Upper.Major + 1,
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
                        }) : PageInstanceSetup._Closure$__.$I2-0)
                    };
                    DispatcherTimer dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
                    dispatcherTimer.Tick += delegate(object a0, EventArgs a1)
                    {
                        this.RefreshRam();
                    };
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
                double num2 = Math.Round(MyWpfExtension.Computer.Info.TotalPhysicalMemory / 1024.0 / 1024.0 / 1024.0, 1);
                double num3 = Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0, 1);
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
                double num9;
                if (Settings.Get<int>("VersionRamType", Instance) == 0)
                {
                    double num2 = Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0;
                    if (Instance != null && !Instance.IsLoaded)
                    {
                        Instance.Load();
                    }
                    double num4;
                    double num5;
                    double num6;
                    double num7;
                    if (Instance != null && Instance.Modable)
                    {
                        DirectoryInfo info = DirectoryUtils.GetInfo(Instance.PathIndie + "mods\\");
                        int num3 = (info.Exists ? info.GetFiles().Count<FileInfo>((PageInstanceSetup._Closure$__.$I12-0 == null) ? (PageInstanceSetup._Closure$__.$I12-0 = (FileInfo f) => new string[] { ".jar", ".zip", ".litemod" }.Contains(f.Extension.Lower())) : PageInstanceSetup._Closure$__.$I12-0) : 0);
                        num4 = 0.5 + (double)num3 / 150.0;
                        num5 = 1.5 + (double)num3 / 90.0;
                        num6 = 2.7 + (double)num3 / 50.0;
                        num7 = 4.5 + (double)num3 / 25.0;
                    }
                    else if (Instance != null && Instance.Version.HasOptiFine)
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
                if (PageInstanceLeft.Instance != null)
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
            if (Operators.CompareString(this.TextServerAuthServer.Text, "", false) == 0 || Operators.CompareString(this.TextServerAuthServer.Text, "https://littleskin.cn/api/yggdrasil", false) == 0 || ModMain.MyMsgBox("即将把第三方登录设置覆盖为 LittleSkin 登录。\r\n除非你是服主，或者服主要求你这样做，否则请不要继续。\r\n\r\n是否确实需要覆盖当前设置？", "设置覆盖确认", "继续", "取消", "", false, true, false, null, null, null) != 2)
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
                    this.HintArgumentJava.Theme = MyHint.Themes.Blue;
                    return;
                }
                if (currentJavaWorker.LastResult != null)
                {
                    this.HintArgumentJava.Text = string.Format("将会使用：{0}", currentJavaWorker.LastResult);
                    this.HintArgumentJava.Theme = MyHint.Themes.Blue;
                    return;
                }
                if (this.ComboArgumentJava.SelectedIndex == 0)
                {
                    this.HintArgumentJava.Text = "你的电脑上没有可供该版本使用的 Java，PCL 会在启动游戏时自动下载。";
                    this.HintArgumentJava.Theme = MyHint.Themes.Yellow;
                    return;
                }
                this.HintArgumentJava.Text = string.Format("该版本的版本文件夹中没有发现任何 Java！{0}请点击此处打开版本文件夹，然后将 Java 文件夹复制进去。", "\r\n");
                this.HintArgumentJava.Theme = MyHint.Themes.Red;
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
                    this.HintArgumentJava.Theme = MyHint.Themes.Red;
                    return;
                }
                if (!currentJavaWorker.LastSucceeded || currentJavaWorker.Running)
                {
                    if (!currentJavaWorker.Running)
                    {
                        currentJavaWorker.Start(default(CancellationToken), null);
                    }
                    this.HintArgumentJava.Text = "正在查找 Java……";
                    this.HintArgumentJava.Theme = MyHint.Themes.Blue;
                    return;
                }
                if (currentJavaWorker.LastResult == null)
                {
                    this.HintArgumentJava.Text = string.Format("你的电脑上没有任何 Java 符合该版本范围！{0}如果 Mojang 提供了对应版本的 Java，PCL 会在启动游戏时自动下载。", "\r\n");
                    this.HintArgumentJava.Theme = MyHint.Themes.Yellow;
                    return;
                }
                this.HintArgumentJava.Text = string.Format("将会使用：{0}", currentJavaWorker.LastResult);
                this.HintArgumentJava.Theme = MyHint.Themes.Blue;
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
                            PageInstanceSetup._Closure$__23-0 CS$<>8__locals1 = new PageInstanceSetup._Closure$__23-0(CS$<>8__locals1);
                            CS$<>8__locals1.$VB$Me = this;
                            CS$<>8__locals1.$VB$Local_JavaEntry = enumerator.Current;
                            MyListItem myListItem = new MyListItem
                            {
                                FontSize = 13.0,
                                Height = 24.0,
                                IsScaleAnimationEnabled = false,
                                Type = MyListItem.CheckType.Clickable,
                                Tag = CS$<>8__locals1.$VB$Local_JavaEntry,
                                Title = CS$<>8__locals1.$VB$Local_JavaEntry.ToString()
                            };
                            myListItem.MouseLeftButtonUp += ((PageInstanceSetup._Closure$__.$I23-0 == null) ? (PageInstanceSetup._Closure$__.$I23-0 = delegate(object sender, MouseButtonEventArgs e)
                            {
                                e.Handled = true;
                            }) : PageInstanceSetup._Closure$__.$I23-0);
                            myListItem.Click += delegate(object a0, MouseButtonEventArgs a1)
                            {
                                base._Lambda$__1();
                            };
                            this.ComboArgumentJavaSelect.Items.Add(myListItem);
                            List<MyIconButton> list = new List<MyIconButton>();
                            bool flag = PathUtils.IsParentOf(string.Format("{0}.minecraft\\runtime\\", Paths.AppData), CS$<>8__locals1.$VB$Local_JavaEntry.Folder);
                            MyIconButton myIconButton = new MyIconButton
                            {
                                Logo = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z",
                                Height = 24.0,
                                Width = 24.0
                            };
                            myIconButton.IsEnabled = !flag;
                            myIconButton.ToolTip = (myIconButton.IsEnabled ? "从列表中移除" : "无法移除官方 Java");
                            myIconButton.Click += delegate(object a0, EventArgs a1)
                            {
                                base._Lambda$__2();
                            };
                            ToolTipService.SetShowOnDisabled(myIconButton, true);
                            list.Add(myIconButton);
                            MyIconButton myIconButton2 = new MyIconButton
                            {
                                Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z",
                                LogoScale = 1.1,
                                Height = 24.0,
                                Width = 24.0
                            };
                            myIconButton2.ToolTip = "打开文件夹";
                            myIconButton2.Click += delegate(object a0, EventArgs a1)
                            {
                                base._Lambda$__3();
                            };
                            list.Add(myIconButton2);
                            myListItem.Buttons = list;
                        }
                    }
                    finally
                    {
                        IEnumerator<Java> enumerator;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    MyListItem myListItem2 = new MyListItem
                    {
                        FontSize = 13.0,
                        Height = 24.0,
                        IsScaleAnimationEnabled = false,
                        Title = "\u200a导入电脑中已有的 Java…",
                        Type = MyListItem.CheckType.Clickable
                    };
                    myListItem2.MouseLeftButtonUp += ((PageInstanceSetup._Closure$__.$I23-4 == null) ? (PageInstanceSetup._Closure$__.$I23-4 = delegate(object sender, MouseButtonEventArgs e)
                    {
                        e.Handled = true;
                        ModJava.ManuallyImportJava();
                    }) : PageInstanceSetup._Closure$__.$I23-4);
                    this.ComboArgumentJavaSelect.Items.Add(myListItem2);
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
                if (this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init, ref flag);
                    if (this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State == 0)
                    {
                        this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State = 2;
                        this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting = false;
                    }
                    else if (this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init);
                    }
                }
                if (!this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting)
                {
                    if (ModMain.MyMsgBox("调整版本隔离设置后，你需要游戏存档、Mod 等文件手动迁移到新的游戏文件夹中。\r\n如果发现存档消失，把这项设置改回来就能恢复。\r\n如果你不会迁移存档，不建议修改这项设置！", "警告", "我知道我在做什么", "取消", "", true, true, false, null, null, null) == 2)
                    {
                        this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting = true;
                        if (e.RemovedItems.Count > 0)
                        {
                            this.ComboArgumentIndieV2.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems[0]);
                        }
                        this.$STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting = false;
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

        private void ReplaceEnter(MyTextBox sender, TextChangedEventArgs e)
        {
            string text = sender.Text.ReplaceLineEndings(" ", true);
            if (Operators.CompareString(text, sender.Text, false) != 0)
            {
                int caretIndex = sender.CaretIndex;
                sender.Text = text;
                sender.CaretIndex = Math.Max(0, checked(caretIndex - 1));
            }
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

        internal virtual MyCard CardArgument
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboArgumentIndieV2
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
                MyComboBox myComboBox = this._ComboArgumentIndieV2;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboArgumentIndieV2 = value;
                myComboBox = this._ComboArgumentIndieV2;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual MyTextBox TextArgumentTitle
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextArgumentInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboArgumentJava
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
                SelectionChangedEventHandler selectionChangedEventHandler = delegate(object a0, SelectionChangedEventArgs a1)
                {
                    this.ReloadCurrentJava();
                };
                SelectionChangedEventHandler selectionChangedEventHandler2 = delegate(object a0, SelectionChangedEventArgs a1)
                {
                    this.UpdateJavaRelatedUi();
                };
                MyComboBox myComboBox = this._ComboArgumentJava;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                    myComboBox.SelectionChanged -= selectionChangedEventHandler2;
                }
                this._ComboArgumentJava = value;
                myComboBox = this._ComboArgumentJava;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                    myComboBox.SelectionChanged += selectionChangedEventHandler2;
                }
            }
        }

        internal virtual MyTextBox TextArgumentJavaRange
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
                RoutedEventHandler routedEventHandler = delegate(object a0, RoutedEventArgs a1)
                {
                    this.ReloadCurrentJava();
                };
                TextChangedEventHandler textChangedEventHandler = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.UpdateJavaRelatedUi();
                };
                MyTextBox myTextBox = this._TextArgumentJavaRange;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged -= routedEventHandler;
                    myTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextArgumentJavaRange = value;
                myTextBox = this._TextArgumentJavaRange;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged += routedEventHandler;
                    myTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual MyHint HintArgumentJava
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
                MyHint.ClickEventHandler clickEventHandler = new MyHint.ClickEventHandler(this.HintArgumentJava_Click);
                MyHint myHint = this._HintArgumentJava;
                if (myHint != null)
                {
                    myHint.Click -= clickEventHandler;
                }
                this._HintArgumentJava = value;
                myHint = this._HintArgumentJava;
                if (myHint != null)
                {
                    myHint.Click += clickEventHandler;
                }
            }
        }

        internal virtual Grid PanArgumentJavaSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboArgumentJavaSelect
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabArgumentJava
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnArgumentJavaSearch
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
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnAdvanceJavaSearch_Click();
                };
                MyIconButton myIconButton = this._BtnArgumentJavaSearch;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnArgumentJavaSearch = value;
                myIconButton = this._BtnArgumentJavaSearch;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyRadioBox RadioRamType2
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.RefreshRam();
                };
                MyRadioBox myRadioBox = this._RadioRamType2;
                if (myRadioBox != null)
                {
                    myRadioBox.Check -= checkEventHandler;
                }
                this._RadioRamType2 = value;
                myRadioBox = this._RadioRamType2;
                if (myRadioBox != null)
                {
                    myRadioBox.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioBox RadioRamType0
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.RefreshRam();
                };
                MyRadioBox myRadioBox = this._RadioRamType0;
                if (myRadioBox != null)
                {
                    myRadioBox.Check -= checkEventHandler;
                }
                this._RadioRamType0 = value;
                myRadioBox = this._RadioRamType0;
                if (myRadioBox != null)
                {
                    myRadioBox.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyRadioBox RadioRamType1
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.RefreshRam();
                };
                MyRadioBox myRadioBox = this._RadioRamType1;
                if (myRadioBox != null)
                {
                    myRadioBox.Check -= checkEventHandler;
                }
                this._RadioRamType1 = value;
                myRadioBox = this._RadioRamType1;
                if (myRadioBox != null)
                {
                    myRadioBox.Check += checkEventHandler;
                }
            }
        }

        internal virtual MySlider SliderRamCustom
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
                MySlider.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.RefreshRam();
                };
                MySlider mySlider = this._SliderRamCustom;
                if (mySlider != null)
                {
                    mySlider.Change -= changeEventHandler;
                }
                this._SliderRamCustom = value;
                mySlider = this._SliderRamCustom;
                if (mySlider != null)
                {
                    mySlider.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyComboBox ComboRamOptimize
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Grid PanRamDisplay
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnRamUsed
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnRamGame
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnRamEmpty
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle RectRamUsed
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Rectangle RectRamGame
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
                SizeChangedEventHandler sizeChangedEventHandler = delegate(object a0, SizeChangedEventArgs a1)
                {
                    this.RefreshRamText();
                };
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

        internal virtual Rectangle RectRamEmpty
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
                SizeChangedEventHandler sizeChangedEventHandler = delegate(object a0, SizeChangedEventArgs a1)
                {
                    this.RefreshRamText();
                };
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

        internal virtual TextBlock LabRamUsedTitle
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabRamGameTitle
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabRamUsed
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabRamTotal
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabRamGame
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
                SizeChangedEventHandler sizeChangedEventHandler = delegate(object a0, SizeChangedEventArgs a1)
                {
                    this.RefreshRamText();
                };
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

        internal virtual MyCard CardServer
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboServerLogin
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
                SelectionChangedEventHandler selectionChangedEventHandler = delegate(object a0, SelectionChangedEventArgs a1)
                {
                    this.ComboServerLogin_Changed();
                };
                MyComboBox myComboBox = this._ComboServerLogin;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboServerLogin = value;
                myComboBox = this._ComboServerLogin;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual TextBlock LabServerNide
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextServerNide
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
                RoutedEventHandler routedEventHandler = delegate(object a0, RoutedEventArgs a1)
                {
                    this.ComboServerLogin_Changed();
                };
                MyTextBox myTextBox = this._TextServerNide;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged -= routedEventHandler;
                }
                this._TextServerNide = value;
                myTextBox = this._TextServerNide;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged += routedEventHandler;
                }
            }
        }

        internal virtual TextBlock LabServerAuthServer
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextServerAuthServer
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
                RoutedEventHandler routedEventHandler = delegate(object a0, RoutedEventArgs a1)
                {
                    this.ComboServerLogin_Changed();
                };
                MyTextBox myTextBox = this._TextServerAuthServer;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged -= routedEventHandler;
                }
                this._TextServerAuthServer = value;
                myTextBox = this._TextServerAuthServer;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged += routedEventHandler;
                }
            }
        }

        internal virtual TextBlock LabServerAuthRegister
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextServerAuthRegister
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
                RoutedEventHandler routedEventHandler = delegate(object a0, RoutedEventArgs a1)
                {
                    this.ComboServerLogin_Changed();
                };
                MyTextBox myTextBox = this._TextServerAuthRegister;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged -= routedEventHandler;
                }
                this._TextServerAuthRegister = value;
                myTextBox = this._TextServerAuthRegister;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged += routedEventHandler;
                }
            }
        }

        internal virtual TextBlock LabServerAuthName
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextServerAuthName
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextServerEnter
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
                TextChangedEventHandler textChangedEventHandler = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.TextServerEnter_Change();
                };
                MyTextBox myTextBox = this._TextServerEnter;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextServerEnter = value;
                myTextBox = this._TextServerEnter;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual Grid PanServerNide
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnServerAuthLittle
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnServerAuthLittle_Click);
                MyButton myButton = this._BtnServerAuthLittle;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnServerAuthLittle = value;
                myButton = this._BtnServerAuthLittle;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyCard CardAdvance
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextAdvanceJvm
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
                TextChangedEventHandler textChangedEventHandler = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.ReplaceEnter((MyTextBox)a0, a1);
                };
                MyTextBox myTextBox = this._TextAdvanceJvm;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceJvm = value;
                myTextBox = this._TextAdvanceJvm;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual MyTextBox TextAdvanceGame
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
                TextChangedEventHandler textChangedEventHandler = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.ReplaceEnter((MyTextBox)a0, a1);
                };
                MyTextBox myTextBox = this._TextAdvanceGame;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceGame = value;
                myTextBox = this._TextAdvanceGame;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual MyTextBox TextAdvanceRun
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
                MyTextBox myTextBox = this._TextAdvanceRun;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceRun = value;
                myTextBox = this._TextAdvanceRun;
                if (myTextBox != null)
                {
                    myTextBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckAdvanceRunWait
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckAdvanceDisableModUpdate
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckAdvanceAssetsV2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyCheckBox CheckAdvanceDisableJLW
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyExtraTextButton BtnSwitch
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
                MyExtraTextButton.ClickEventHandler clickEventHandler = new MyExtraTextButton.ClickEventHandler(this.BtnSwitch_Click);
                MyExtraTextButton myExtraTextButton = this._BtnSwitch;
                if (myExtraTextButton != null)
                {
                    myExtraTextButton.Click -= clickEventHandler;
                }
                this._BtnSwitch = value;
                myExtraTextButton = this._BtnSwitch;
                if (myExtraTextButton != null)
                {
                    myExtraTextButton.Click += clickEventHandler;
                }
            }
        }

        private bool LoadedFirst;

        private int RamTextLeft;

        private int RamTextRight;

        private int ComboServerLoginLast;

        private bool $STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting;

        private StaticLocalInitFlag $STATIC$ComboArgumentIndieV2_SelectionChanged$20211C128135$IsReverting$Init;
    }
}
