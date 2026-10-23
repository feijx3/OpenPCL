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
using PCL.My;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSetupLaunch : MyPageRight
    {
        public PageSetupLaunch()
        {
            base.Loaded += this.PageSetupLaunch_Loaded;
            this.RamTextLeft = 2;
            this.RamTextRight = 1;
            this.InitializeComponent();
        }

        private void PageSetupLaunch_Loaded(object sender, RoutedEventArgs e)
        {
            this.PanBack.ScrollToHome();
            this.RefreshRam(false);
            this.BtnSwitch.Visibility = ((ModMinecraft.McInstanceSelected == null) ? Visibility.Collapsed : Visibility.Visible);
            checked
            {
                ModAnimation.AniControlEnabled++;
                this.Refresh();
                ModAnimation.AniControlEnabled--;
                if (this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init, ref flag);
                    if (this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State == 0)
                    {
                        this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State = 2;
                        this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded = false;
                    }
                    else if (this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init);
                    }
                }
                if (!this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded)
                {
                    this.$STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded = true;
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
                MyBitmap myBitmap = new MyBitmap(Paths.AppDataThenName + "CustomSkin.png");
                if (myBitmap.Pic.Width == 64 && myBitmap.Pic.Height == 32)
                {
                    global::System.Drawing.Image image = myBitmap;
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
            MySkin.Save(PageLaunchLeft.SkinLegacy);
        }

        private void BtnSkinCache_Click(object sender, EventArgs e)
        {
            MySkin.RefreshCache(null);
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
            double num8;
            if (Settings.Get<int>("LaunchRamType", null) == 0)
            {
                double num = Math.Round(MyWpfExtension.Computer.Info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0 * 10.0) / 10.0;
                if (Instance != null && !Instance.IsLoaded)
                {
                    Instance.Load();
                }
                double num3;
                double num4;
                double num5;
                double num6;
                if (Instance != null && Instance.Modable)
                {
                    DirectoryInfo info = DirectoryUtils.GetInfo(Instance.PathIndie + "mods\\");
                    int num2 = (info.Exists ? info.GetFiles().Count<FileInfo>((PageSetupLaunch._Closure$__.$I17-0 == null) ? (PageSetupLaunch._Closure$__.$I17-0 = (FileInfo f) => new string[] { ".jar", ".zip", ".litemod" }.Contains(f.Extension.Lower())) : PageSetupLaunch._Closure$__.$I17-0) : 0);
                    num3 = 0.5 + (double)num2 / 150.0;
                    num4 = 1.5 + (double)num2 / 90.0;
                    num5 = 2.7 + (double)num2 / 50.0;
                    num6 = 4.5 + (double)num2 / 25.0;
                }
                else if (Instance != null && Instance.Version.HasOptiFine)
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
                            PageSetupLaunch._Closure$__18-0 CS$<>8__locals1 = new PageSetupLaunch._Closure$__18-0(CS$<>8__locals1);
                            CS$<>8__locals1.$VB$Me = this;
                            CS$<>8__locals1.$VB$Local_JavaEntry = concurrentList[i];
                            MyListItem myListItem = new MyListItem
                            {
                                FontSize = 13.0,
                                Height = 24.0,
                                IsScaleAnimationEnabled = false,
                                Type = MyListItem.CheckType.Clickable,
                                Tag = CS$<>8__locals1.$VB$Local_JavaEntry,
                                Title = CS$<>8__locals1.$VB$Local_JavaEntry.ToString()
                            };
                            myListItem.MouseLeftButtonUp += ((PageSetupLaunch._Closure$__.$I18-0 == null) ? (PageSetupLaunch._Closure$__.$I18-0 = delegate(object sender, MouseButtonEventArgs e)
                            {
                                e.Handled = true;
                            }) : PageSetupLaunch._Closure$__.$I18-0);
                            myListItem.Click += ((PageSetupLaunch._Closure$__.$IR18-2 == null) ? (PageSetupLaunch._Closure$__.$IR18-2 = delegate(object a0, MouseButtonEventArgs a1)
                            {
                                ((PageSetupLaunch._Closure$__.$I18-1 == null) ? (PageSetupLaunch._Closure$__.$I18-1 = delegate
                                {
                                    ModMain.Hint("点击选项右侧的箭头可以进行排序，以控制 PCL 优先选择哪个 Java！", ModMain.HintType.Blue, true);
                                }) : PageSetupLaunch._Closure$__.$I18-1)();
                            }) : PageSetupLaunch._Closure$__.$IR18-2);
                            this.ComboAdvanceJava.Items.Add(myListItem);
                            List<MyIconButton> list = new List<MyIconButton>();
                            MyIconButton myIconButton = new MyIconButton
                            {
                                Logo = "M554 333V853h-85V333l-228 228-60-60L512 170l331 331-60 60L554 333z",
                                LogoScale = 0.95,
                                Height = 24.0,
                                Width = 24.0
                            };
                            myIconButton.IsEnabled = i > 0;
                            myIconButton.ToolTip = "提高优先级";
                            ToolTipService.SetShowOnDisabled(myIconButton, true);
                            myIconButton.Click += delegate(object a0, EventArgs a1)
                            {
                                base._Lambda$__2();
                            };
                            list.Add(myIconButton);
                            MyIconButton myIconButton2 = new MyIconButton
                            {
                                Logo = "M554 691V171h-85V691L241 463l-60 60L512 854l331-331-60-60L554 691z",
                                LogoScale = 0.95,
                                Height = 24.0,
                                Width = 24.0
                            };
                            myIconButton2.IsEnabled = i < concurrentList.Count - 1;
                            myIconButton2.ToolTip = "降低优先级";
                            ToolTipService.SetShowOnDisabled(myIconButton2, true);
                            myIconButton2.Click += delegate(object a0, EventArgs a1)
                            {
                                base._Lambda$__3();
                            };
                            list.Add(myIconButton2);
                            bool flag = PathUtils.IsParentOf(string.Format("{0}.minecraft\\runtime\\", Paths.AppData), CS$<>8__locals1.$VB$Local_JavaEntry.Folder);
                            MyIconButton myIconButton3 = new MyIconButton
                            {
                                Logo = "M508 990.4c-261.6 0-474.4-212-474.4-474.4S246.4 41.6 508 41.6s474.4 212 474.4 474.4S769.6 990.4 508 990.4zM508 136.8c-209.6 0-379.2 169.6-379.2 379.2 0 209.6 169.6 379.2 379.2 379.2s379.2-169.6 379.2-379.2C887.2 306.4 717.6 136.8 508 136.8zM697.6 563.2 318.4 563.2c-26.4 0-47.2-21.6-47.2-47.2 0-26.4 21.6-47.2 47.2-47.2l379.2 0c26.4 0 47.2 21.6 47.2 47.2C744.8 542.4 724 563.2 697.6 563.2z",
                                Height = 24.0,
                                Width = 24.0
                            };
                            myIconButton3.IsEnabled = !flag;
                            myIconButton3.ToolTip = (myIconButton3.IsEnabled ? "从列表中移除" : "无法移除官方 Java");
                            myIconButton3.Click += delegate(object a0, EventArgs a1)
                            {
                                base._Lambda$__4();
                            };
                            ToolTipService.SetShowOnDisabled(myIconButton3, true);
                            list.Add(myIconButton3);
                            MyIconButton myIconButton4 = new MyIconButton
                            {
                                Logo = "M889.018182 418.909091H884.363636V316.509091a93.090909 93.090909 0 0 0-99.607272-89.832727h-302.545455l-93.090909-76.334546A46.545455 46.545455 0 0 0 358.865455 139.636364H146.152727A93.090909 93.090909 0 0 0 46.545455 229.469091V837.818182a46.545455 46.545455 0 0 0 46.545454 46.545454 46.545455 46.545455 0 0 0 16.756364-3.258181 109.381818 109.381818 0 0 0 25.134545 3.258181h586.472727a85.178182 85.178182 0 0 0 87.04-63.301818l163.374546-302.545454a46.545455 46.545455 0 0 0 5.585454-21.876364A82.385455 82.385455 0 0 0 889.018182 418.909091z m-744.727273-186.181818h198.283636l93.09091 76.334545a46.545455 46.545455 0 0 0 29.323636 10.705455h319.301818a12.101818 12.101818 0 0 1 6.516364 0V418.909091H302.545455a85.178182 85.178182 0 0 0-87.04 63.301818L139.636364 622.778182V232.727273a19.549091 19.549091 0 0 1 6.516363 0z m578.094546 552.029091a27.461818 27.461818 0 0 0-2.792728 6.516363H154.530909l147.083636-272.290909a27.461818 27.461818 0 0 0 2.792728-6.981818h565.061818z",
                                LogoScale = 1.1,
                                Height = 24.0,
                                Width = 24.0
                            };
                            myIconButton4.ToolTip = "打开文件夹";
                            myIconButton4.Click += delegate(object a0, EventArgs a1)
                            {
                                base._Lambda$__5();
                            };
                            list.Add(myIconButton4);
                            myListItem.Buttons = list;
                        }
                        MyListItem myListItem2 = new MyListItem
                        {
                            FontSize = 13.0,
                            Height = 24.0,
                            IsScaleAnimationEnabled = false,
                            Title = "导入电脑中已有的 Java…",
                            Type = MyListItem.CheckType.Clickable
                        };
                        myListItem2.MouseLeftButtonUp += ((PageSetupLaunch._Closure$__.$I18-6 == null) ? (PageSetupLaunch._Closure$__.$I18-6 = delegate(object sender, MouseButtonEventArgs e)
                        {
                            e.Handled = true;
                            ModJava.ManuallyImportJava();
                        }) : PageSetupLaunch._Closure$__.$I18-6);
                        this.ComboAdvanceJava.Items.Add(myListItem2);
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
            Configs.JavaList.Edit(delegate(ref ConcurrentList<Java> List)
            {
                int num = List.IndexOf(JavaEntry);
                int num2 = checked(num + Offset);
                if (num >= 0 && num2 >= 0 && num2 < List.Count)
                {
                    List.RemoveAt(num);
                    List.Insert(num2, JavaEntry);
                }
            });
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
            if (ModAnimation.AniControlEnabled == 0 && this.ComboArgumentVisibie.SelectedIndex == 0 && ModMain.MyMsgBox("若在游戏启动后立即关闭启动器，崩溃检测、更改游戏标题等功能将失效。\r\n如果想保留这些功能，可以选择让启动器在游戏启动后隐藏，游戏退出后自动关闭。", "提醒", "继续", "取消", "", false, true, false, null, null, null) == 2 && e.RemovedItems.Count > 0)
            {
                this.ComboArgumentVisibie.SelectedItem = RuntimeHelpers.GetObjectValue(e.RemovedItems[0]);
            }
        }

        private void CheckArgumentRam_Change()
        {
            if (ModAnimation.AniControlEnabled == 0 && this.CheckArgumentRam.Checked && ModMain.MyMsgBox("内存优化会显著延长启动耗时，建议仅在内存不足时开启。\r\n如果你在使用机械硬盘，这还可能导致一小段时间的严重卡顿。" + (WindowsUtils.HasAdminRole() ? "" : string.Format("{0}{1}每次启动游戏，PCL 都需要申请管理员权限以进行内存优化。{2}若想自动授予权限，可以右键 PCL，打开 属性 → 兼容性 → 以管理员身份运行此程序。", "\r\n", "\r\n", "\r\n")), "提醒", "确定", "取消", "", false, true, false, null, null, null) == 2)
            {
                this.CheckArgumentRam.Checked = false;
            }
        }

        private void ComboArgumentIndie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                ModMain.MyMsgBox("本设置仅会对之后新安装的版本生效。\r\n如果要修改已安装的版本的隔离方式，请在它的版本独立设置中调整。", "提示", "确定", "", "", false, true, false, null, null, null);
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

        private void BtnSwitch_Click(object sender, MouseButtonEventArgs e)
        {
            ModMinecraft.McInstanceSelected.Load();
            PageInstanceLeft.Instance = ModMinecraft.McInstanceSelected;
            ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.SetupLink);
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
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboArgumentIndie_SelectionChanged);
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

        internal virtual MyComboBox ComboArgumentVisibie
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
                MyComboBox myComboBox = this._ComboArgumentVisibie;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboArgumentVisibie = value;
                myComboBox = this._ComboArgumentVisibie;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual Grid PanArgumentWindow
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboArgumentWindowType
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
                SelectionChangedEventHandler selectionChangedEventHandler = delegate(object a0, SelectionChangedEventArgs a1)
                {
                    this.WindowTypeUIRefresh();
                };
                MyComboBox myComboBox = this._ComboArgumentWindowType;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboArgumentWindowType = value;
                myComboBox = this._ComboArgumentWindowType;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual MyTextBox TextArgumentWindowWidth
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabArgumentWindowMiddle
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyTextBox TextArgumentWindowHeight
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
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

        internal virtual MyCheckBox CheckArgumentRam
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
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.CheckArgumentRam_Change();
                };
                MyCheckBox myCheckBox = this._CheckArgumentRam;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckArgumentRam = value;
                myCheckBox = this._CheckArgumentRam;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
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

        internal virtual global::System.Windows.Shapes.Rectangle RectRamUsed
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual global::System.Windows.Shapes.Rectangle RectRamGame
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

        internal virtual global::System.Windows.Shapes.Rectangle RectRamEmpty
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

        internal virtual MyCard CardSkin
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioSkinType0
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioSkinType1
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioSkinType2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioSkinType3
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyRadioBox RadioSkinType4
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
                MyRadioBox.PreviewCheckEventHandler previewCheckEventHandler = new MyRadioBox.PreviewCheckEventHandler(this.RadioSkinType3_Check);
                MyRadioBox myRadioBox = this._RadioSkinType4;
                if (myRadioBox != null)
                {
                    myRadioBox.PreviewCheck -= previewCheckEventHandler;
                }
                this._RadioSkinType4 = value;
                myRadioBox = this._RadioSkinType4;
                if (myRadioBox != null)
                {
                    myRadioBox.PreviewCheck += previewCheckEventHandler;
                }
            }
        }

        internal virtual Grid PanSkinID
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnSkinSave
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSkinSave_Click);
                MyButton myButton = this._BtnSkinSave;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSkinSave = value;
                myButton = this._BtnSkinSave;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnSkinCache
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSkinCache_Click);
                MyButton myButton = this._BtnSkinCache;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSkinCache = value;
                myButton = this._BtnSkinCache;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual Grid PanSkinChange
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyButton BtnSkinChange
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSkinChange_Click);
                MyButton myButton = this._BtnSkinChange;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSkinChange = value;
                myButton = this._BtnSkinChange;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyButton BtnSkinDelete
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnSkinDelete_Click);
                MyButton myButton = this._BtnSkinDelete;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnSkinDelete = value;
                myButton = this._BtnSkinDelete;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnAdvanceJvmReset
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
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.BtnAdvanceJvmReset_Click);
                MyIconButton myIconButton = this._BtnAdvanceJvmReset;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnAdvanceJvmReset = value;
                myIconButton = this._BtnAdvanceJvmReset;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
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
                RoutedEventHandler routedEventHandler = delegate(object a0, RoutedEventArgs a1)
                {
                    this.TextAdvanceJvm_TextChanged();
                };
                TextChangedEventHandler textChangedEventHandler = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.ReplaceEnter((MyTextBox)a0, a1);
                };
                MyTextBox myTextBox = this._TextAdvanceJvm;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged -= routedEventHandler;
                    myTextBox.TextChanged -= textChangedEventHandler;
                }
                this._TextAdvanceJvm = value;
                myTextBox = this._TextAdvanceJvm;
                if (myTextBox != null)
                {
                    myTextBox.ValidatedTextChanged += routedEventHandler;
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

        internal virtual Grid PanAdvanceJava
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboAdvanceJava
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabAdvanceJava
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnAdvanceJavaSearch
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
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnAdvanceJavaSearch_Click();
                };
                MyIconButton myIconButton = this._BtnAdvanceJavaSearch;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnAdvanceJavaSearch = value;
                myIconButton = this._BtnAdvanceJavaSearch;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
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

        private int RamTextLeft;

        private int RamTextRight;

        private bool $STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded;

        private StaticLocalInitFlag $STATIC$PageSetupLaunch_Loaded$20211C128141$Reloaded$Init;
    }
}
