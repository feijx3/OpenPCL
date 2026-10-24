using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
    public partial class PageSetupLaunch : OurPageRight
    {
        public PageSetupLaunch()
        {
            base.Loaded += this.PageSetupLaunch_Loaded;
            this.RamTextLeft = 2;
            this.RamTextRight = 1;
            this.InitializeComponent();
            this.ComboArgumentIndieV2_WithEvents = this.ComboArgumentIndieV2;
            this.ComboArgumentVisibie_WithEvents = this.ComboArgumentVisibie;
            this.ComboArgumentWindowType_WithEvents = this.ComboArgumentWindowType;
            this.RadioRamType0_WithEvents = this.RadioRamType0;
            this.RadioRamType1_WithEvents = this.RadioRamType1;
            this.SliderRamCustom_WithEvents = this.SliderRamCustom;
            this.CheckArgumentRam_WithEvents = this.CheckArgumentRam;
            this.RectRamGame_WithEvents = this.RectRamGame;
            this.RectRamEmpty_WithEvents = this.RectRamEmpty;
            this.LabRamGame_WithEvents = this.LabRamGame;
            this.RadioSkinType4_WithEvents = this.RadioSkinType4;
            this.BtnSkinSave_WithEvents = this.BtnSkinSave;
            this.BtnSkinCache_WithEvents = this.BtnSkinCache;
            this.BtnSkinChange_WithEvents = this.BtnSkinChange;
            this.BtnSkinDelete_WithEvents = this.BtnSkinDelete;
            this.BtnAdvanceJvmReset_WithEvents = this.BtnAdvanceJvmReset;
            this.TextAdvanceJvm_WithEvents = this.TextAdvanceJvm;
            this.TextAdvanceGame_WithEvents = this.TextAdvanceGame;
            this.TextAdvanceRun_WithEvents = this.TextAdvanceRun;
            this.BtnAdvanceJavaSearch_WithEvents = this.BtnAdvanceJavaSearch;
            this.BtnSwitch_WithEvents = this.BtnSwitch;
        }

        private void PageSetupLaunch_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            this.RefreshRam(false);
            this.BtnSwitch.Visibility = ((((object)(ModMinecraft.McInstanceSelected) == (object)null)) ? Visibility.Collapsed : Visibility.Visible);
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.Refresh();
                ModAnimation.AniControlEnabled--;
                if (this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init, ref flag);
                    if (this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init.State == 0)
                    {
                        this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init.State = 2;
                        this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded = false;
                    }
                    else if (this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init);
                    }
                }
                if (!this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded)
                {
                    this._STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded = true;
                    DispatcherTimer dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
                    dispatcherTimer.Tick += this._Lambda___R1_1;
                    dispatcherTimer.Start();
                }
            }
        }

        public void Refresh()
        {
            try
            {
                SettingService.RefreshSettings(this);
                PageSetupLaunch.UpdateSkinType();
                PageSetupLaunch.UpdateRamType();
                this.UpdateJavaList();
            }
            catch (NullReferenceException ex)
            {
                Logger.Error(ex, "启动设置项存在异常，已被自动重置", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLaunch.xaml.vb");
                this.Reset();
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "重载启动设置时出错", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLaunch.xaml.vb");
            }
        }

        public void Reset()
        {
            try
            {
                SettingService.ResetSettings(this);
                Settings.Set("LaunchArgumentIndieV2", Settings.GetDefault("LaunchArgumentIndieV2"), null);
                Configs.JavaList.Reset();
                Configs.JavaRemovedList.Reset();
                ModJava.JavaListRefreshWorker.Start(default(CancellationToken), null);
                ModMain.Hint("已初始化启动设置！", ModMain.HintType.Green, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化启动设置失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLaunch.xaml.vb");
            }
            this.Refresh();
        }

        private void BtnSkinChange_Click(object sender, EventArgs e)
        {
            ModMinecraft.McSkinInfo mcSkinInfo = ModMinecraft.McSkinSelect();
            if (mcSkinInfo.IsVaild)
            {
                this.ChangeSkin(mcSkinInfo);
            }
        }

        private void RadioSkinType3_Check(object sender, ModBase.RouteEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0 && e.RaiseByMouse && !FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png"))
            {
                ModMinecraft.McSkinInfo mcSkinInfo = ModMinecraft.McSkinSelect();
                if (!mcSkinInfo.IsVaild)
                {
                    e.Handled = true;
                    return;
                }
                if (!this.ChangeSkin(mcSkinInfo))
                {
                    e.Handled = true;
                }
            }
        }

        private bool ChangeSkin(ModMinecraft.McSkinInfo SkinInfo)
        {
            bool flag;
            try
            {
                FileUtils.Copy(SkinInfo.LocalFile, Paths.AppDataThenName + "CustomSkin.png");
                OurBitmap ourBitmap = new OurBitmap(Paths.AppDataThenName + "CustomSkin.png");
                if (ourBitmap.Pic.Width == 64 && ourBitmap.Pic.Height == 32)
                {
                    global::System.Drawing.Image image = ourBitmap;
                    Bitmap bitmap = new Bitmap(64, 64);
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawImageUnscaled(image, new global::System.Drawing.Point(0, 0));
                    }
                    FileUtils.Delete(Paths.AppDataThenName + "CustomSkin.png", false);
                    bitmap.Save(Paths.AppDataThenName + "CustomSkin.png");
                }
                Settings.Set("LaunchSkinSlim", SkinInfo.IsSlim, null);
                flag = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "改变离线皮肤失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLaunch.xaml.vb");
                flag = false;
            }
            finally
            {
                PageLaunchLeft.SkinLegacy.Start(null, true);
            }
            return flag;
        }

        private void BtnSkinDelete_Click(object sender, EventArgs e)
        {
            try
            {
                FileUtils.Delete(Paths.AppDataThenName + "CustomSkin.png", false);
                this.RadioSkinType0.SetChecked(true, true);
                ModMain.Hint("离线皮肤已清空！", ModMain.HintType.Green, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "清空离线皮肤失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLaunch.xaml.vb");
            }
        }

        private void BtnSkinSave_Click(object sender, EventArgs e)
        {
            OurSkin.Save(PageLaunchLeft.SkinLegacy);
        }

        private void BtnSkinCache_Click(object sender, EventArgs e)
        {
            OurSkin.RefreshCache(null);
        }

        public static void UpdateSkinType()
        {
            PageLaunchLeft.SkinLegacy.Start(null, false);
            if (ModMain.FrmSetupLaunch != null)
            {
                switch (Settings.Get<int>("LaunchSkinType", null))
                {
                case 0:
                case 1:
                case 2:
                    ModMain.FrmSetupLaunch.PanSkinID.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupLaunch.PanSkinChange.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    ModMain.FrmSetupLaunch.PanSkinID.Visibility = Visibility.Visible;
                    ModMain.FrmSetupLaunch.PanSkinChange.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    ModMain.FrmSetupLaunch.PanSkinID.Visibility = Visibility.Collapsed;
                    ModMain.FrmSetupLaunch.PanSkinChange.Visibility = Visibility.Visible;
                    break;
                }
                ModMain.FrmSetupLaunch.CardSkin.TriggerForceResize();
            }
        }

        public static void UpdateRamType()
        {
            PageSetupLaunch frmSetupLaunch = ModMain.FrmSetupLaunch;
            if (((frmSetupLaunch != null) ? frmSetupLaunch.SliderRamCustom : null) != null)
            {
                ModMain.FrmSetupLaunch.SliderRamCustom.IsEnabled = Settings.Get<int>("LaunchRamType", null) == 1;
            }
        }

        public void RefreshRam(bool ShowAnim)
        {
            if (this.LabRamGame != null && this.LabRamUsed != null && !(ModMain.FrmMain.PageCurrent != FormMain.PageType.Setup) && ModMain.FrmSetupLeft.PageID == FormMain.PageSubType.Default)
            {
                double num = Math.Round(PageSetupLaunch.GetRam(ModMinecraft.McInstanceSelected, false), 5);
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
                    }, "SetupLaunch Ram Grid", false);
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
                    }, "SetupLaunch Ram TextLeft", false);
                    break;
                case 1:
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.LabRamUsed, 1.0 - this.LabRamUsed.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamTotal, -this.LabRamTotal.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamUsedTitle, 0.7 - this.LabRamUsedTitle.Opacity, 100, 0, null, false)
                    }, "SetupLaunch Ram TextLeft", false);
                    break;
                case 2:
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(this.LabRamUsed, 1.0 - this.LabRamUsed.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamTotal, 1.0 - this.LabRamTotal.Opacity, 100, 0, null, false),
                        ModAnimation.AaOpacity(this.LabRamUsedTitle, 0.7 - this.LabRamUsedTitle.Opacity, 100, 0, null, false)
                    }, "SetupLaunch Ram TextLeft", false);
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
                if (ModAnimation.AniControlEnabled == 0 && (this.RamTextRight != num2 || ModAnimation.AniIsRun("SetupLaunch Ram TextRight")))
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaX(this.LabRamGame, actualWidth2 - actualWidth3 - this.LabRamGame.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaX(this.LabRamGameTitle, actualWidth2 - actualWidth6 - this.LabRamGameTitle.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                    }, "SetupLaunch Ram TextRight", false);
                }
                else
                {
                    ModAnimation.AniStop("SetupLaunch Ram TextRight");
                    this.LabRamGame.Margin = new Thickness(actualWidth2 - actualWidth3, 3.0, 0.0, 0.0);
                    this.LabRamGameTitle.Margin = new Thickness(actualWidth2 - actualWidth6, 0.0, 0.0, 5.0);
                }
            }
            else if (ModAnimation.AniControlEnabled == 0 && (this.RamTextRight != num2 || ModAnimation.AniIsRun("SetupLaunch Ram TextRight")))
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaX(this.LabRamGame, 2.0 + actualWidth - this.LabRamGame.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaX(this.LabRamGameTitle, 2.0 + actualWidth - this.LabRamGameTitle.Margin.Left, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                }, "SetupLaunch Ram TextRight", false);
            }
            else
            {
                ModAnimation.AniStop("SetupLaunch Ram TextRight");
                this.LabRamGame.Margin = new Thickness(2.0 + actualWidth, 3.0, 0.0, 0.0);
                this.LabRamGameTitle.Margin = new Thickness(2.0 + actualWidth, 0.0, 0.0, 5.0);
            }
            this.RamTextRight = num2;
        }

        public static double GetRam(ModMinecraft.McInstance Instance, bool UseVersionJavaSetup)
        {
            double num8 = default;
            if (Settings.Get<int>("LaunchRamType", null) == 0)
            {
                double num = Math.Round(OurWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0;
                if (((object)Instance != (object)null) && !Instance.IsLoaded)
                {
                    Instance.Load();
                }
                double num3;
                double num4;
                double num5;
                double num6;
                if (((object)Instance != (object)null) && Instance.Modable)
                {
                    DirectoryInfo info = DirectoryUtils.GetInfo(Instance.PathIndie + "mods\\");
                    int num2 = (info.Exists ? info.GetFiles().Count<FileInfo>((PageSetupLaunch._Closure___._I17_0 == null) ? (PageSetupLaunch._Closure___._I17_0 = new Func<FileInfo, bool>(PageSetupLaunch._Closure___._I._Lambda___17_0)) : PageSetupLaunch._Closure___._I17_0) : 0);
                    num3 = 0.5 + (double)num2 / 150.0;
                    num4 = 1.5 + (double)num2 / 90.0;
                    num5 = 2.7 + (double)num2 / 50.0;
                    num6 = 4.5 + (double)num2 / 25.0;
                }
                else if (((object)Instance != (object)null) && Instance.Version.HasOptiFine)
                {
                    num3 = 0.5;
                    num4 = 1.5;
                    num5 = 3.0;
                    num6 = 5.0;
                }
                else
                {
                    num3 = 0.5;
                    num4 = 1.5;
                    num5 = 2.5;
                    num6 = 4.0;
                }
                double num7 = num4;
                num8 += Math.Min(num, num7);
                num -= num7;
                if (num >= 0.1)
                {
                    num7 = num5 - num4;
                    num8 += Math.Min(num * 0.7, num7);
                    num -= num7 / 0.7;
                    if (num >= 0.1)
                    {
                        num7 = num6 - num5;
                        num8 += Math.Min(num * 0.4, num7);
                        num -= num7 / 0.4;
                        if (num >= 0.1)
                        {
                            num7 = num6;
                            num8 += Math.Min(num * 0.15, num7);
                            num -= num7 / 0.15;
                        }
                    }
                }
                num8 = Math.Round(Math.Max(num8, num3), 1);
            }
            else
            {
                int num9 = Settings.Get<int>("LaunchRamCustom", null);
                if (num9 <= 12)
                {
                    num8 = (double)num9 * 0.1 + 0.3;
                }
                else if (num9 <= 25)
                {
                    num8 = (double)(checked(num9 - 12)) * 0.5 + 1.5;
                }
                else if (num9 <= 33)
                {
                    num8 = (double)(checked((num9 - 25) * 1 + 8));
                }
                else
                {
                    num8 = (double)(checked((num9 - 33) * 2 + 16));
                }
            }
            return num8;
        }

        public void UpdateJavaList()
        {
            checked
            {
                if (this.ComboAdvanceJava != null && this.LabAdvanceJava != null && this.ComboAdvanceJava.IsLoaded)
                {
                    this.ComboAdvanceJava.Items.Clear();
                    if (ModJava.JavaListRefreshWorker.Running)
                    {
                        this.BtnAdvanceJavaSearch.IsEnabled = false;
                        this.ComboAdvanceJava.IsEnabled = false;
                        this.LabAdvanceJava.Text = "搜索中\u200a…";
                        return;
                    }
                    this.BtnAdvanceJavaSearch.IsEnabled = true;
                    this.ComboAdvanceJava.IsEnabled = true;
                    int count = Configs.JavaList.Get().Count;
                    this.LabAdvanceJava.Text = ((count > 0) ? string.Format("共有 {0} 个 Java\u200a…", count) : "未找到 Java，点击以导入已有的 Java");
                    try
                    {
                        ConcurrentList<Java> concurrentList = Configs.JavaList.Get();
                        int num = concurrentList.Count - 1;
                        for (int i = 0; i <= num; i++)
                        {
                            PageSetupLaunch._Closure___18_0 CS_locals1 = new PageSetupLaunch._Closure___18_0(null);
                            CS_locals1._VB_Me = this;
                            CS_locals1._VB_Local_JavaEntry = concurrentList[i];
                            OurListItem ourListItem = new OurListItem
                            {
                                FontSize = 13.0,
                                Height = 24.0,
                                IsScaleAnimationEnabled = false,
                                Type = OurListItem.CheckType.Clickable,
                                Tag = CS_locals1._VB_Local_JavaEntry,
                                Title = CS_locals1._VB_Local_JavaEntry.ToString()
                            };
                            ourListItem.MouseLeftButtonUp += ((PageSetupLaunch._Closure___._I18_0 == null) ? (PageSetupLaunch._Closure___._I18_0 = new MouseButtonEventHandler(PageSetupLaunch._Closure___._I._Lambda___18_0)) : PageSetupLaunch._Closure___._I18_0);
                            ourListItem.Click += ((PageSetupLaunch._Closure___._IR18_2 == null) ? (PageSetupLaunch._Closure___._IR18_2 = new OurListItem.ClickEventHandler(PageSetupLaunch._Closure___._I._Lambda___R18_2)) : PageSetupLaunch._Closure___._IR18_2);
                            this.ComboAdvanceJava.Items.Add(ourListItem);
                            List<OurIconButton> list = new List<OurIconButton>();
                            OurIconButton ourIconButton = new OurIconButton
                            {
                                Logo = "M554 333V853h-85V333l-228 228-60-60L512 170l331 331-60 60L554 333z",
                                LogoScale = 0.95,
                                Height = 24.0,
                                Width = 24.0
                            };
                            ourIconButton.IsEnabled = i > 0;
                            ourIconButton.ToolTip = "提高优先级";
                            ToolTipService.SetShowOnDisabled(ourIconButton, true);
                            ourIconButton.Click += CS_locals1._Lambda___R3;
                            list.Add(ourIconButton);
                            OurIconButton ourIconButton2 = new OurIconButton
                            {
                                Logo = "M554 691V171h-85V691L241 463l-60 60L512 854l331-331-60-60L554 691z",
                                LogoScale = 0.95,
                                Height = 24.0,
                                Width = 24.0
                            };
                            ourIconButton2.IsEnabled = i < concurrentList.Count - 1;
                            ourIconButton2.ToolTip = "降低优先级";
                            ToolTipService.SetShowOnDisabled(ourIconButton2, true);
                            ourIconButton2.Click += CS_locals1._Lambda___R4;
                            list.Add(ourIconButton2);
                            bool flag = PathUtils.IsParentOf(string.Format("{0}.minecraft\\runtime\\", Paths.AppData), CS_locals1._VB_Local_JavaEntry.Folder);
                            OurIconButton ourIconButton3 = new OurIconButton
                            {
                                Logo = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z",
                                Height = 24.0,
                                Width = 24.0
                            };
                            ourIconButton3.IsEnabled = !flag;
                            ourIconButton3.ToolTip = (ourIconButton3.IsEnabled ? "从列表中移除" : "无法移除官方 Java");
                            ourIconButton3.Click += CS_locals1._Lambda___R5;
                            ToolTipService.SetShowOnDisabled(ourIconButton3, true);
                            list.Add(ourIconButton3);
                            OurIconButton ourIconButton4 = new OurIconButton
                            {
                                Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z",
                                LogoScale = 1.1,
                                Height = 24.0,
                                Width = 24.0
                            };
                            ourIconButton4.ToolTip = "打开文件夹";
                            ourIconButton4.Click += CS_locals1._Lambda___R6;
                            list.Add(ourIconButton4);
                            ourListItem.Buttons = list;
                        }
                        OurListItem ourListItem2 = new OurListItem
                        {
                            FontSize = 13.0,
                            Height = 24.0,
                            IsScaleAnimationEnabled = false,
                            Title = "导入电脑中已有的 Java…",
                            Type = OurListItem.CheckType.Clickable
                        };
                        ourListItem2.MouseLeftButtonUp += ((PageSetupLaunch._Closure___._I18_6 == null) ? (PageSetupLaunch._Closure___._I18_6 = new MouseButtonEventHandler(PageSetupLaunch._Closure___._I._Lambda___18_6)) : PageSetupLaunch._Closure___._I18_6);
                        this.ComboAdvanceJava.Items.Add(ourListItem2);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "更新 Java 列表下拉框失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLaunch.xaml.vb");
                    }
                }
            }
        }

        private void MoveJavaInList(Java JavaEntry, int Offset)
        {
            PageSetupLaunch._Closure___19_0 CS_locals1 = new PageSetupLaunch._Closure___19_0();
            CS_locals1._VB_Local_JavaEntry = JavaEntry;
            CS_locals1._VB_Local_Offset = Offset;
            Configs.JavaList.Edit(new ConfigEntry<ConcurrentList<Java>>.RefAction(CS_locals1._Lambda___0));
            ModJava.UpdateJavaLists();
        }

        private void BtnAdvanceJavaSearch_Click()
        {
            ModJava.ManuallySearchJava(default(CancellationToken));
        }

        private void WindowTypeUIRefresh()
        {
            Visibility visibility = (this.ComboArgumentWindowType.SelectedIndex == 3).ToVisibility();
            this.TextArgumentWindowHeight.Visibility = visibility;
            this.LabArgumentWindowMiddle.Visibility = visibility;
            this.TextArgumentWindowWidth.Visibility = visibility;
        }

        private void ComboArgumentVisibie_SizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0 && this.ComboArgumentVisibie.SelectedIndex == 0 && ModMain.OurMsgBox("若在游戏启动后立即关闭启动器，崩溃检测、更改游戏标题等功能将失效。\r\n如果想保留这些功能，可以选择让启动器在游戏启动后隐藏，游戏退出后自动关闭。", "提醒", "继续", "取消", "", false, true, false, null, null, null) == 2 && e.RemovedItems.Count > 0)
            {
                this.ComboArgumentVisibie.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems[0]);
            }
        }

        private void CheckArgumentRam_Change()
        {
            if (ModAnimation.AniControlEnabled == 0 && this.CheckArgumentRam.Checked && ModMain.OurMsgBox("内存优化会显著延长启动耗时，建议仅在内存不足时开启。\r\n如果你在使用机械硬盘，这还可能导致一小段时间的严重卡顿。" + (WindowsUtils.HasAdminRole() ? "" : string.Format("{0}{1}每次启动游戏，PCL 都需要申请管理员权限以进行内存优化。{2}若想自动授予权限，可以右键 PCL，打开 属性 → 兼容性 → 以管理员身份运行此程序。", "\r\n", "\r\n", "\r\n")), "提醒", "确定", "取消", "", false, true, false, null, null, null) == 2)
            {
                this.CheckArgumentRam.Checked = false;
            }
        }

        private void ComboArgumentIndie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                ModMain.OurMsgBox("本设置仅会对之后新安装的版本生效。\r\n如果要修改已安装的版本的隔离方式，请在它的版本独立设置中调整。", "提示", "确定", "", "", false, true, false, null, null, null);
            }
        }

        private void TextAdvanceRun_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.CheckAdvanceRunWait.Visibility = ((Operators.CompareString(this.TextAdvanceRun.Text, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
        }

        private void TextAdvanceJvm_TextChanged()
        {
            this.BtnAdvanceJvmReset.Visibility = ((Operators.CompareString(this.TextAdvanceJvm.Text, Settings.GetDefault("LaunchAdvanceJvm"), false) == 0) ? Visibility.Hidden : Visibility.Visible);
        }

        private void BtnAdvanceJvmReset_Click(object sender, EventArgs e)
        {
            Settings.Reset("LaunchAdvanceJvm", null);
            this.Refresh();
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

        private void BtnSwitch_Click(object sender, MouseButtonEventArgs e)
        {
            ModMinecraft.McInstanceSelected.Load();
            PageInstanceLeft.Instance = ModMinecraft.McInstanceSelected;
            ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.SetupLink);
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
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboArgumentIndie_SelectionChanged);
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

        internal virtual OurComboBox ComboArgumentVisibie_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboArgumentVisibie;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboArgumentVisibie_SizeChanged);
                OurComboBox ourComboBox = this._ComboArgumentVisibie;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboArgumentVisibie = value;
                ourComboBox = this._ComboArgumentVisibie;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }


        internal virtual OurComboBox ComboArgumentWindowType_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboArgumentWindowType;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this._Lambda___R56_7);
                OurComboBox ourComboBox = this._ComboArgumentWindowType;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboArgumentWindowType = value;
                ourComboBox = this._ComboArgumentWindowType;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
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
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R72_8);
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
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R76_9);
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
                OurSlider.ChangeEventHandler changeEventHandler = new OurSlider.ChangeEventHandler(this._Lambda___R80_10);
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

        internal virtual OurCheckBox CheckArgumentRam_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._CheckArgumentRam;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurCheckBox.ChangeEventHandler changeEventHandler = new OurCheckBox.ChangeEventHandler(this._Lambda___R84_11);
                OurCheckBox ourCheckBox = this._CheckArgumentRam;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change -= changeEventHandler;
                }
                this._CheckArgumentRam = value;
                ourCheckBox = this._CheckArgumentRam;
                if (ourCheckBox != null)
                {
                    ourCheckBox.Change += changeEventHandler;
                }
            }
        }






        internal virtual global::System.Windows.Shapes.Rectangle RectRamGame_WithEvents
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
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R108_12);
                global::System.Windows.Shapes.Rectangle rectangle = this._RectRamGame;
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

        internal virtual global::System.Windows.Shapes.Rectangle RectRamEmpty_WithEvents
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
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R112_13);
                global::System.Windows.Shapes.Rectangle rectangle = this._RectRamEmpty;
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
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R132_14);
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






        internal virtual OurRadioBox RadioSkinType4_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RadioSkinType4;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurRadioBox.PreviewCheckEventHandler previewCheckEventHandler = new OurRadioBox.PreviewCheckEventHandler(this.RadioSkinType3_Check);
                OurRadioBox ourRadioBox = this._RadioSkinType4;
                if (ourRadioBox != null)
                {
                    ourRadioBox.PreviewCheck -= previewCheckEventHandler;
                }
                this._RadioSkinType4 = value;
                ourRadioBox = this._RadioSkinType4;
                if (ourRadioBox != null)
                {
                    ourRadioBox.PreviewCheck += previewCheckEventHandler;
                }
            }
        }


        internal virtual OurButton BtnSkinSave_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSkinSave;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSkinSave_Click);
                OurButton ourButton = this._BtnSkinSave;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSkinSave = value;
                ourButton = this._BtnSkinSave;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnSkinCache_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSkinCache;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSkinCache_Click);
                OurButton ourButton = this._BtnSkinCache;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSkinCache = value;
                ourButton = this._BtnSkinCache;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }


        internal virtual OurButton BtnSkinChange_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSkinChange;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSkinChange_Click);
                OurButton ourButton = this._BtnSkinChange;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSkinChange = value;
                ourButton = this._BtnSkinChange;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnSkinDelete_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSkinDelete;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnSkinDelete_Click);
                OurButton ourButton = this._BtnSkinDelete;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnSkinDelete = value;
                ourButton = this._BtnSkinDelete;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurIconButton BtnAdvanceJvmReset_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnAdvanceJvmReset;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.BtnAdvanceJvmReset_Click);
                OurIconButton ourIconButton = this._BtnAdvanceJvmReset;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnAdvanceJvmReset = value;
                ourIconButton = this._BtnAdvanceJvmReset;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
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
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R188_15);
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this._Lambda___R188_16);
                OurTextBox ourTextBox = this._TextAdvanceJvm;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged -= routedEventHandler;
                    ourTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceJvm = value;
                ourTextBox = this._TextAdvanceJvm;
                if (ourTextBox != null)
                {
                    ourTextBox.ValidatedTextChanged += routedEventHandler;
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
                TextChangedEventHandler textChangedEventHandler = new TextChangedEventHandler(this._Lambda___R192_17);
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





        internal virtual OurIconButton BtnAdvanceJavaSearch_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnAdvanceJavaSearch;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this._Lambda___R216_18);
                OurIconButton ourIconButton = this._BtnAdvanceJavaSearch;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnAdvanceJavaSearch = value;
                ourIconButton = this._BtnAdvanceJavaSearch;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
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
        private void _Lambda___R1_1(object a0, EventArgs a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R56_7(object a0, SelectionChangedEventArgs a1)
        {
            this.WindowTypeUIRefresh();
        }

        [CompilerGenerated]
        private void _Lambda___R72_8(object a0, ModBase.RouteEventArgs a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R76_9(object a0, ModBase.RouteEventArgs a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R80_10(object a0, bool a1)
        {
            this.RefreshRam();
        }

        [CompilerGenerated]
        private void _Lambda___R84_11(object a0, bool a1)
        {
            this.CheckArgumentRam_Change();
        }

        [CompilerGenerated]
        private void _Lambda___R108_12(object a0, SizeChangedEventArgs a1)
        {
            this.RefreshRamText();
        }

        [CompilerGenerated]
        private void _Lambda___R112_13(object a0, SizeChangedEventArgs a1)
        {
            this.RefreshRamText();
        }

        [CompilerGenerated]
        private void _Lambda___R132_14(object a0, SizeChangedEventArgs a1)
        {
            this.RefreshRamText();
        }

        [CompilerGenerated]
        private void _Lambda___R188_15(object a0, RoutedEventArgs a1)
        {
            this.TextAdvanceJvm_TextChanged();
        }

        [CompilerGenerated]
        private void _Lambda___R188_16(object a0, TextChangedEventArgs a1)
        {
            this.ReplaceEnter((OurTextBox)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R192_17(object a0, TextChangedEventArgs a1)
        {
            this.ReplaceEnter((OurTextBox)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R216_18(object a0, EventArgs a1)
        {
            this.BtnAdvanceJavaSearch_Click();
        }

        private int RamTextLeft;

        private int RamTextRight;

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
        [AccessedThroughProperty("ComboArgumentVisibie")]
        private OurComboBox _ComboArgumentVisibie;

        [CompilerGenerated]
        [AccessedThroughProperty("PanArgumentWindow")]
        private Grid _PanArgumentWindow;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboArgumentWindowType")]
        private OurComboBox _ComboArgumentWindowType;

        [CompilerGenerated]
        [AccessedThroughProperty("TextArgumentWindowWidth")]
        private OurTextBox _TextArgumentWindowWidth;

        [CompilerGenerated]
        [AccessedThroughProperty("LabArgumentWindowMiddle")]
        private TextBlock _LabArgumentWindowMiddle;

        [CompilerGenerated]
        [AccessedThroughProperty("TextArgumentWindowHeight")]
        private OurTextBox _TextArgumentWindowHeight;

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
        [AccessedThroughProperty("CheckArgumentRam")]
        private OurCheckBox _CheckArgumentRam;

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
        private global::System.Windows.Shapes.Rectangle _RectRamUsed;

        [CompilerGenerated]
        [AccessedThroughProperty("RectRamGame")]
        private global::System.Windows.Shapes.Rectangle _RectRamGame;

        [CompilerGenerated]
        [AccessedThroughProperty("RectRamEmpty")]
        private global::System.Windows.Shapes.Rectangle _RectRamEmpty;

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
        [AccessedThroughProperty("CardSkin")]
        private OurCard _CardSkin;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioSkinType0")]
        private OurRadioBox _RadioSkinType0;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioSkinType1")]
        private OurRadioBox _RadioSkinType1;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioSkinType2")]
        private OurRadioBox _RadioSkinType2;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioSkinType3")]
        private OurRadioBox _RadioSkinType3;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioSkinType4")]
        private OurRadioBox _RadioSkinType4;

        [CompilerGenerated]
        [AccessedThroughProperty("PanSkinID")]
        private Grid _PanSkinID;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSkinSave")]
        private OurButton _BtnSkinSave;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSkinCache")]
        private OurButton _BtnSkinCache;

        [CompilerGenerated]
        [AccessedThroughProperty("PanSkinChange")]
        private Grid _PanSkinChange;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSkinChange")]
        private OurButton _BtnSkinChange;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSkinDelete")]
        private OurButton _BtnSkinDelete;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnAdvanceJvmReset")]
        private OurIconButton _BtnAdvanceJvmReset;

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
        [AccessedThroughProperty("PanAdvanceJava")]
        private Grid _PanAdvanceJava;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboAdvanceJava")]
        private OurComboBox _ComboAdvanceJava;

        [CompilerGenerated]
        [AccessedThroughProperty("LabAdvanceJava")]
        private TextBlock _LabAdvanceJava;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnAdvanceJavaSearch")]
        private OurIconButton _BtnAdvanceJavaSearch;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSwitch")]
        private OurExtraTextButton _BtnSwitch;

        private bool _STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded;

        private StaticLocalInitFlag _STATIC_PageSetupLaunch_Loaded_20211C128141_Reloaded_Init;

        [CompilerGenerated]
        [Serializable]
        private new sealed class _Closure___
        {
            internal bool _Lambda___17_0(FileInfo f)
            {
                return new string[] { ".jar", ".zip", ".litemod" }.Contains(f.Extension.Lower());
            }

            internal void _Lambda___18_0(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
            }

            internal void _Lambda___R18_2(object a0, MouseButtonEventArgs a1)
            {
                ((PageSetupLaunch._Closure___._I18_1 == null) ? (PageSetupLaunch._Closure___._I18_1 = new VB_AnonymousDelegate_10(PageSetupLaunch._Closure___._I._Lambda___18_1)) : PageSetupLaunch._Closure___._I18_1)();
            }

            internal void _Lambda___18_1()
            {
                ModMain.Hint("点击选项右侧的箭头可以进行排序，以控制 PCL 优先选择哪个 Java！", ModMain.HintType.Blue, true);
            }

            internal void _Lambda___18_6(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                ModJava.ManuallyImportJava();
            }

            public static readonly PageSetupLaunch._Closure___ _I = new PageSetupLaunch._Closure___();

            public static Func<FileInfo, bool> _I17_0;

            public static MouseButtonEventHandler _I18_0;

            public static VB_AnonymousDelegate_10 _I18_1;

            public static OurListItem.ClickEventHandler _IR18_2;

            public static MouseButtonEventHandler _I18_6;
        }

        [CompilerGenerated]
        private sealed class _Closure___18_0
        {
            public _Closure___18_0(PageSetupLaunch._Closure___18_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_JavaEntry = arg0._VB_Local_JavaEntry;
                }
            }

            internal void _Lambda___R3(object a0, EventArgs a1)
            {
                this._Lambda___2();
            }

            internal void _Lambda___2()
            {
                this._VB_Me.MoveJavaInList(this._VB_Local_JavaEntry, -1);
            }

            internal void _Lambda___R4(object a0, EventArgs a1)
            {
                this._Lambda___3();
            }

            internal void _Lambda___3()
            {
                this._VB_Me.MoveJavaInList(this._VB_Local_JavaEntry, 1);
            }

            internal void _Lambda___R5(object a0, EventArgs a1)
            {
                this._Lambda___4();
            }

            internal void _Lambda___4()
            {
                ModJava.ManuallyRemoveJava(this._VB_Local_JavaEntry);
            }

            internal void _Lambda___R6(object a0, EventArgs a1)
            {
                this._Lambda___5();
            }

            internal void _Lambda___5()
            {
                ModBase.OpenExplorer(this._VB_Local_JavaEntry.JavaExePath);
            }

            public Java _VB_Local_JavaEntry;

            public PageSetupLaunch _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___19_0
        {
            internal void _Lambda___0(ref ConcurrentList<Java> List)
            {
                int num = List.IndexOf(this._VB_Local_JavaEntry);
                int num2 = checked(num + this._VB_Local_Offset);
                if (num >= 0 && num2 >= 0 && num2 < List.Count)
                {
                    List.RemoveAt(num);
                    List.Insert(num2, this._VB_Local_JavaEntry);
                }
            }

            public Java _VB_Local_JavaEntry;

            public int _VB_Local_Offset;
        }
    }
}
