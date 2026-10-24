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
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLaunchLeft : OurPageLeft
    {
        public PageLaunchLeft()
        {
            base.Loaded += this._Lambda___R1_1;
            this.IsLoad = false;
            this.IsLoadFinished = false;
            this.PageCurrent = PageLaunchLeft.PageType.None;
            this.BtnLaunchState = 0;
            this.BtnLaunchInstance = null;
            this.ShowProgress = 0.0;
            this.IsWidthAnimating = false;
            this.IsHeightAnimating = false;
            this.InitializeComponent();
            this.BtnVersion_WithEvents = this.BtnVersion;
            this.BtnMore_WithEvents = this.BtnMore;
            this.RadioLoginType5_WithEvents = this.RadioLoginType5;
            this.RadioLoginType0_WithEvents = this.RadioLoginType0;
            this.BtnLaunch_WithEvents = this.BtnLaunch;
            this.PanLaunchingInfo_WithEvents = this.PanLaunchingInfo;
            this.BtnCancel_WithEvents = this.BtnCancel;
        }

        public void PageLaunchLeft_Loaded()
        {
            if (this.IsLoad)
            {
                this.RefreshPage(true, false);
            }
            this.AprilPosTrans.X = 0.0;
            this.AprilPosTrans.Y = 0.0;
            checked
            {
                if (!this.IsLoad)
                {
                    this.IsLoad = true;
                    ModAnimation.AniControlEnabled++;
                    ModMinecraft.McInstanceListLoader.LoadingStateChanged += this._Lambda___R4_2;
                    ModMinecraft.McFolderListLoader.LoadingStateChanged += this._Lambda___R4_3;
                    this.RefreshButtonsUI();
                    ModBase.RunInNewThread(new Action(this._Lambda___4_0), "Version Check", ThreadPriority.AboveNormal);
                    ModLaunch.McLoginType mcLoginType = Settings.Get<ModLaunch.McLoginType>("LoginType", null);
                    if (mcLoginType == ModLaunch.McLoginType.Legacy || mcLoginType == ModLaunch.McLoginType.Ms)
                    {
                        ((OurRadioButton)base.FindName("RadioLoginType" + Conversions.ToString((int)mcLoginType))).Checked = true;
                    }
                    this.RefreshPage(false, false);
                    ModAnimation.AniControlEnabled--;
                }
            }
        }

        public void PageChangeToLaunching()
        {
            switch (Settings.Get<ModLaunch.McLoginType>("LoginType", null))
            {
            case ModLaunch.McLoginType.Legacy:
                this.LabLaunchingMethod.Text = "离线登录";
                break;
            case ModLaunch.McLoginType.Nide:
                this.LabLaunchingMethod.Text = "统一通行证";
                break;
            case ModLaunch.McLoginType.Auth:
                this.LabLaunchingMethod.Text = "Authlib-Injector";
                break;
            case ModLaunch.McLoginType.Ms:
                this.LabLaunchingMethod.Text = "正版登录";
                break;
            }
            this.LabLaunchingName.Text = ModMinecraft.McInstanceSelected.Name;
            this.LabLaunchingStage.Text = "初始化";
            TextBlock labLaunchingTitle = this.LabLaunchingTitle;
            ModLaunch.McLaunchOptions currentLaunchOptions = ModLaunch.CurrentLaunchOptions;
            labLaunchingTitle.Text = ((((currentLaunchOptions != null) ? currentLaunchOptions.SaveBatch : null) == null) ? "正在启动游戏" : "正在导出启动脚本");
            this.LabLaunchingProgress.Text = "0.00 %";
            this.LabLaunchingProgress.Opacity = 1.0;
            this.LabLaunchingDownload.Visibility = Visibility.Visible;
            this.LabLaunchingProgressLeft.Opacity = 0.6;
            this.LabLaunchingDownload.Visibility = Visibility.Visible;
            this.LabLaunchingDownload.Text = "0 B/s";
            this.LabLaunchingDownload.Opacity = 0.0;
            this.LabLaunchingDownload.Visibility = Visibility.Collapsed;
            this.LabLaunchingDownloadLeft.Opacity = 0.0;
            this.LabLaunchingDownloadLeft.Visibility = Visibility.Collapsed;
            this.ProgressLaunchingFinished.Width = new GridLength(0.0, GridUnitType.Star);
            this.ProgressLaunchingUnfinished.Width = new GridLength(1.0, GridUnitType.Star);
            this.PanLaunchingHint.Opacity = 0.0;
            this.PanLaunchingHint.Visibility = Visibility.Collapsed;
            this.PanLaunchingInfo.Width = double.NaN;
            ModLaunch.McLaunchProcess = null;
            ModLaunch.McLaunchWatcher = null;
            this.LabLaunchingHint.Text = PageOtherTest.GetRandomHint();
            this.PanInput.IsHitTestVisible = false;
            this.PanLaunching.IsHitTestVisible = false;
            this.LoadLaunching.State.LoadingState = OurLoading.OurLoadingState.Run;
            this.PanLaunching.Visibility = Visibility.Visible;
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.PanInput, 0.0, 50, 0, null, false),
                ModAnimation.AaOpacity(this.PanInput, -this.PanInput.Opacity, 110, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), true),
                ModAnimation.AaScaleTransform(this.PanInput, 1.2 - ((ScaleTransform)this.PanInput.RenderTransform).ScaleX, 160, 0, null, false),
                ModAnimation.AaOpacity(this.PanLaunching, 1.0 - this.PanLaunching.Opacity, 150, 100, null, false),
                ModAnimation.AaScaleTransform(this.PanLaunching, 1.0 - ((ScaleTransform)this.PanLaunching.RenderTransform).ScaleX, 500, 100, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaCode(new ThreadStart(this._Lambda___5_0), 150, false)
            }, "Launch State Page", false);
        }

        public void PageChangeToLogin()
        {
            NewLateBinding.LateCall(this.PageGet(this.PageCurrent), null, "Reload", new object[] { false }, new string[] { "KeepInput" }, null, null, true);
            this.PanInput.IsHitTestVisible = false;
            this.PanLaunching.IsHitTestVisible = false;
            this.LoadLaunching.State.LoadingState = OurLoading.OurLoadingState.Stop;
            this.PanInput.Visibility = Visibility.Visible;
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.PanLaunching, -this.PanLaunching.Opacity, 150, 0, null, false),
                ModAnimation.AaScaleTransform(this.PanLaunching, 0.8 - ((ScaleTransform)this.PanLaunching.RenderTransform).ScaleX, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaOpacity(this.PanInput, 1.0 - this.PanInput.Opacity, 250, 50, null, false),
                ModAnimation.AaScaleTransform(this.PanInput, 1.0 - ((ScaleTransform)this.PanInput.RenderTransform).ScaleX, 300, 50, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                ModAnimation.AaCode(new ThreadStart(this._Lambda___6_0), 200, false)
            }, "Launch State Page", true);
        }

        private object PageGet(PageLaunchLeft.PageType Type)
        {
            object obj;
            switch (Type)
            {
            case PageLaunchLeft.PageType.Legacy:
                if (Information.IsNothing(ModMain.FrmLoginLegacy))
                {
                    ModMain.FrmLoginLegacy = new PageLoginLegacy();
                }
                obj = ModMain.FrmLoginLegacy;
                break;
            case PageLaunchLeft.PageType.Nide:
                if (Information.IsNothing(ModMain.FrmLoginNide))
                {
                    ModMain.FrmLoginNide = new PageLoginNide();
                }
                obj = ModMain.FrmLoginNide;
                break;
            case PageLaunchLeft.PageType.NideSkin:
                if (Information.IsNothing(ModMain.FrmLoginNideSkin))
                {
                    ModMain.FrmLoginNideSkin = new PageLoginNideSkin();
                }
                obj = ModMain.FrmLoginNideSkin;
                break;
            case PageLaunchLeft.PageType.Auth:
                if (Information.IsNothing(ModMain.FrmLoginAuth))
                {
                    ModMain.FrmLoginAuth = new PageLoginAuth();
                }
                obj = ModMain.FrmLoginAuth;
                break;
            case PageLaunchLeft.PageType.AuthSkin:
                if (Information.IsNothing(ModMain.FrmLoginAuthSkin))
                {
                    ModMain.FrmLoginAuthSkin = new PageLoginAuthSkin();
                }
                obj = ModMain.FrmLoginAuthSkin;
                break;
            case PageLaunchLeft.PageType.Ms:
                if (Information.IsNothing(ModMain.FrmLoginMs))
                {
                    ModMain.FrmLoginMs = new PageLoginMs();
                }
                obj = ModMain.FrmLoginMs;
                break;
            case PageLaunchLeft.PageType.MsSkin:
                if (Information.IsNothing(ModMain.FrmLoginMsSkin))
                {
                    ModMain.FrmLoginMsSkin = new PageLoginMsSkin();
                }
                obj = ModMain.FrmLoginMsSkin;
                break;
            default:
                throw new ArgumentOutOfRangeException("Type", "即将切换的登录分页编号越界");
            }
            return obj;
        }

        private object PageChange(PageLaunchLeft.PageType Type, bool Anim)
        {
            PageLaunchLeft._Closure___10_0 CS_locals1 = new PageLaunchLeft._Closure___10_0();
            CS_locals1._VB_Me = this;
            CS_locals1._VB_Local_PageNew = ModMain.FrmLoginMs;
            checked
            {
                object obj;
                try
                {
                    if (this.PageCurrent == Type)
                    {
                        obj = CS_locals1._VB_Local_PageNew;
                    }
                    else
                    {
                        CS_locals1._VB_Local_PageNew = RuntimeHelpers.GetObjectValue(this.PageGet(Type));
                        ModAnimation.AniStop("FrmLogin PageChange");
                        if (!Information.IsNothing(RuntimeHelpers.GetObjectValue(CS_locals1._VB_Local_PageNew)) && !Information.IsNothing(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(CS_locals1._VB_Local_PageNew, null, "Parent", new object[0], null, null, null))))
                        {
                            object _VB_Local_PageNew = CS_locals1._VB_Local_PageNew;
                            Type type = null;
                            string text = "SetValue";
                            object[] array = new object[2];
                            array[0] = ContentPresenter.ContentProperty;
                            NewLateBinding.LateCall(_VB_Local_PageNew, type, text, array, null, null, null, true);
                        }
                        if (Anim)
                        {
                            base.Dispatcher.Invoke(new Action(CS_locals1._Lambda___0), DispatcherPriority.Render);
                        }
                        else
                        {
                            ModAnimation.AniControlEnabled++;
                            this.PanLogin.Children.Clear();
                            this.PanLogin.Children.Add((UIElement)CS_locals1._VB_Local_PageNew);
                            ModAnimation.AniControlEnabled--;
                        }
                        this.PageCurrent = Type;
                        obj = CS_locals1._VB_Local_PageNew;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, string.Format("切换登录分页失败（{0}）", Type), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    obj = CS_locals1._VB_Local_PageNew;
                }
                return obj;
            }
        }

        public void RefreshPage(bool KeepInput, bool Anim)
        {
            int num;
            if (((object)(ModMinecraft.McInstanceSelected) != (object)null))
            {
                num = Settings.Get<int>("VersionServerLogin", ModMinecraft.McInstanceSelected);
                Settings.Set("LoginPageType", num, null);
            }
            else
            {
                num = Settings.Get<int>("LoginPageType", null);
            }
            PageLaunchLeft.PageType pageType;
            switch (num)
            {
            case 0:
                break;
            case 1:
                if (Operators.CompareString(Settings.Get<string>("CacheMsV2Access", null), "", false) == 0)
                {
                    pageType = PageLaunchLeft.PageType.Ms;
                }
                else
                {
                    pageType = PageLaunchLeft.PageType.MsSkin;
                }
                Settings.Set("LoginType", ModLaunch.McLoginType.Ms, null);
                this.PanType.Visibility = Visibility.Collapsed;
                this.PanTypeOne.Visibility = Visibility.Visible;
                this.PathTypeOne.Data = (Geometry)new GeometryConverter().ConvertFromString("M511.488256 95.184408c35.310345 22.516742 95.184408 55.78011 167.34033 84.437781 75.738131 29.681159 148.405797 40.93953 191.392304 45.033483v353.615193c0 73.691154-50.662669 164.781609-136.123938 244.101949C649.65917 901.181409 558.568716 942.12094 512 942.12094c-46.568716 0-137.65917-40.93953-222.096952-119.748126C204.441779 742.54073 153.77911 651.450275 153.77911 577.247376v-353.103448c42.474763-4.093953 116.165917-15.352324 191.904048-45.545227 75.226387-30.192904 133.565217-63.456272 165.805098-83.414293M512 0c-4.093953 0-8.187906 1.535232-11.258371 3.582209l-14.84058 10.234882c-1.023488 0.511744-67.550225 47.592204-170.410794 88.531735-100.813593 39.916042-198.556722 41.963018-199.58021 41.963018l-25.075462 0.511744c-10.746627 0.511744-18.934533 8.187906-18.934533 18.422789v414.000999c0 216.97951 286.064968 446.24088 440.09995 446.24088s440.09995-229.261369 440.09995-445.729136V163.758121c0-10.234883-8.69965-18.422789-18.934533-18.422789l-24.563718-0.511744c-1.023488 0-98.766617-2.046977-199.58021-41.963018-103.372314-40.93953-170.410795-88.01999-170.922538-88.531734L523.258371 3.582209c-3.070465-2.558721-7.164418-3.582209-11.258371-3.582209z M743.308346 410.930535l-260.477761 260.477761c-15.864068 15.864068-41.963018 15.864068-57.827087 0l-144.823588-144.823588c-15.864068-15.864068-15.864068-41.963018 0-57.827087 8.187906-8.187906 18.422789-11.770115 29.169415-11.770115 10.234883 0 20.981509 4.093953 29.169416 11.770115l115.654173 115.654173L685.993003 352.591704c15.864068-15.864068 41.963018-15.864068 57.827087 0 15.352324 16.375812 15.352324 42.474763-0.511744 58.338831z");
                this.LabTypeOne.Text = "正版登录";
                this.RadioLoginType5.Visibility = Visibility.Visible;
                this.RadioLoginType0.Visibility = Visibility.Collapsed;
                goto IL_0326;
            case 2:
                pageType = PageLaunchLeft.PageType.Legacy;
                Settings.Set("LoginType", ModLaunch.McLoginType.Legacy, null);
                this.PanType.Visibility = Visibility.Collapsed;
                this.PanTypeOne.Visibility = Visibility.Visible;
                this.PathTypeOne.Data = (Geometry)new GeometryConverter().ConvertFromString("M533.293176 788.841412a60.235294 60.235294 0 1 1 85.202824 85.202823l-42.616471 42.586353c-129.355294 129.385412-339.124706 129.385412-468.510117 0-129.385412-129.385412-129.385412-339.124706 0-468.510117l42.586353-42.616471a60.235294 60.235294 0 1 1 85.202823 85.202824l-42.61647 42.586352a210.823529 210.823529 0 1 0 298.164706 298.164706l42.586352-42.61647z m255.548236-255.548236l42.61647-42.586352a210.823529 210.823529 0 1 0-298.164706-298.164706l-42.586352 42.61647a60.235294 60.235294 0 1 1-85.202824-85.202823l42.616471-42.586353c129.355294-129.385412 339.124706-129.385412 468.510117 0 129.385412 129.385412 129.385412 339.124706 0 468.510117l-42.586353 42.616471a60.235294 60.235294 0 1 1-85.202823-85.202824zM192.542118 192.542118a60.235294 60.235294 0 0 1 85.202823 0l553.712941 553.712941a60.235294 60.235294 0 0 1-85.202823 85.202823L192.542118 277.744941a60.235294 60.235294 0 0 1 0-85.202823z");
                this.LabTypeOne.Text = "离线登录";
                goto IL_0326;
            case 3:
                if (Operators.CompareString(Settings.Get<string>("CacheNideAccess", null), "", false) == 0)
                {
                    pageType = PageLaunchLeft.PageType.Nide;
                }
                else
                {
                    pageType = PageLaunchLeft.PageType.NideSkin;
                }
                Settings.Set("LoginType", ModLaunch.McLoginType.Nide, null);
                this.PanType.Visibility = Visibility.Collapsed;
                this.PanTypeOne.Visibility = Visibility.Visible;
                this.PathTypeOne.Data = (Geometry)new GeometryConverter().ConvertFromString("M834.5 684.1c-31.2-70.4-98.9-120.9-179.1-127.3 63.5-8.5 112.6-63 112.6-128.8 0-71.8-58.2-130-130-130s-130 58.2-130 130c0 65.9 49 120.3 112.6 128.8-80.2 6.4-148 57-179.1 127.3-8.7 19.7 6 42 27.6 42 12.1 0 22.7-7.5 27.7-18.5 24.3-53.9 78.5-91.5 141.3-91.5s117 37.6 141.3 91.5c5 11.1 15.6 18.5 27.7 18.5 21.4 0 36.1-22.3 27.4-42zM567.9 427.9c0-38.6 31.4-70 70-70s70 31.4 70 70-31.4 70-70 70-70-31.4-70-70zM460.3 347.9H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h243.3c16.6 0 30-13.4 30-30 0.1-16.5-13.4-30-29.9-30zM367.4 459.6H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h150.4c16.6 0 30-13.4 30-30 0.1-16.6-13.4-30-29.9-30zM297.4 571.2H217c-16.6 0-30 13.4-30 30s13.4 30 30 30h80.4c16.6 0 30-13.4 30-30 0-16.5-13.5-30-30-30zM900 236v552H124V236h776m0-60H124c-33.1 0-60 26.9-60 60v552c0 33.1 26.9 60 60 60h776c33.1 0 60-26.9 60-60V236c0-33.1-26.9-60-60-60z");
                this.LabTypeOne.Text = "统一通行证登录";
                goto IL_0326;
            case 4:
                if (Operators.CompareString(Settings.Get<string>("CacheAuthAccess", null), "", false) == 0)
                {
                    pageType = PageLaunchLeft.PageType.Auth;
                }
                else
                {
                    pageType = PageLaunchLeft.PageType.AuthSkin;
                }
                Settings.Set("LoginType", ModLaunch.McLoginType.Auth, null);
                this.PanType.Visibility = Visibility.Collapsed;
                this.PanTypeOne.Visibility = Visibility.Visible;
                this.PathTypeOne.Data = (Geometry)new GeometryConverter().ConvertFromString("M834.5 684.1c-31.2-70.4-98.9-120.9-179.1-127.3 63.5-8.5 112.6-63 112.6-128.8 0-71.8-58.2-130-130-130s-130 58.2-130 130c0 65.9 49 120.3 112.6 128.8-80.2 6.4-148 57-179.1 127.3-8.7 19.7 6 42 27.6 42 12.1 0 22.7-7.5 27.7-18.5 24.3-53.9 78.5-91.5 141.3-91.5s117 37.6 141.3 91.5c5 11.1 15.6 18.5 27.7 18.5 21.4 0 36.1-22.3 27.4-42zM567.9 427.9c0-38.6 31.4-70 70-70s70 31.4 70 70-31.4 70-70 70-70-31.4-70-70zM460.3 347.9H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h243.3c16.6 0 30-13.4 30-30 0.1-16.5-13.4-30-29.9-30zM367.4 459.6H216.9c-16.6 0-30 13.4-30 30s13.4 30 30 30h150.4c16.6 0 30-13.4 30-30 0.1-16.6-13.4-30-29.9-30zM297.4 571.2H217c-16.6 0-30 13.4-30 30s13.4 30 30 30h80.4c16.6 0 30-13.4 30-30 0-16.5-13.5-30-30-30zM900 236v552H124V236h776m0-60H124c-33.1 0-60 26.9-60 60v552c0 33.1 26.9 60 60 60h776c33.1 0 60-26.9 60-60V236c0-33.1-26.9-60-60-60z");
                this.LabTypeOne.Text = ((((object)(ModMinecraft.McInstanceSelected) == (object)null)) ? Settings.Get<string>("CacheAuthServerName", null) : Settings.Get<string>("VersionServerAuthName", ModMinecraft.McInstanceSelected));
                if (Operators.CompareString(this.LabTypeOne.Text, "", false) == 0)
                {
                    this.LabTypeOne.Text = "第三方登录";
                    goto IL_0326;
                }
                goto IL_0326;
            default:
                Logger.Error(string.Format("未知的登录页面：{0}", num), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                break;
            }
            if (this.RadioLoginType5.Checked)
            {
                if (Operators.CompareString(Settings.Get<string>("CacheMsV2Access", null), "", false) == 0)
                {
                    pageType = PageLaunchLeft.PageType.Ms;
                }
                else
                {
                    pageType = PageLaunchLeft.PageType.MsSkin;
                }
                Settings.Set("LoginType", ModLaunch.McLoginType.Ms, null);
            }
            else
            {
                pageType = PageLaunchLeft.PageType.Legacy;
                Settings.Set("LoginType", ModLaunch.McLoginType.Legacy, null);
            }
            this.PanType.Visibility = Visibility.Visible;
            this.PanTypeOne.Visibility = Visibility.Collapsed;
            this.RadioLoginType5.Visibility = Visibility.Visible;
            this.RadioLoginType0.Visibility = Visibility.Visible;
            IL_0326:
            if (this.PageCurrent != pageType)
            {
                object[] array;
                bool[] array2;
                NewLateBinding.LateCall(this.PageChange(pageType, Anim), null, "Reload", array = new object[] { KeepInput }, null, null, array2 = new bool[] { true }, true);
                if (array2[0])
                {
                    KeepInput = (bool)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(bool));
                }
                OurRadioButton ourRadioButton = (OurRadioButton)base.FindName("RadioLoginType" + Conversions.ToString((int)Settings.Get<ModLaunch.McLoginType>("LoginType", null)));
                if (ourRadioButton != null)
                {
                    ourRadioButton.Checked = true;
                }
            }
        }

        private void RadioLoginType_Change(object sender, bool raiseByMouse)
        {
            if (raiseByMouse)
            {
                this.RefreshPage(true, true);
            }
        }

        private static ValueTuple<string, string> SkinMsInput()
        {
            return new ValueTuple<string, string>(Settings.Get<string>("CacheMsV2Name", null), Settings.Get<string>("CacheMsV2Uuid", null));
        }

        private static void SkinMsLoad(ModLoader.LoaderTask<ValueTuple<string, string>, string> Data)
        {
            ModBase.RunInUi((PageLaunchLeft._Closure___._I15_0 == null) ? (PageLaunchLeft._Closure___._I15_0 = new Action(PageLaunchLeft._Closure___._I._Lambda___15_0)) : PageLaunchLeft._Closure___._I15_0, false);
            string item = Data.Input.Item1;
            string item2 = Data.Input.Item2;
            if (Operators.CompareString(item, "", false) == 0)
            {
                Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png";
                Logger.Info("获取微软正版皮肤失败，ID 为空", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
            }
            else
            {
                try
                {
                    string text = ModMinecraft.McSkinGetAddress(item2, "Ms");
                    if (Data.IsCanceled)
                    {
                        throw new OperationCanceledException("当前任务已取消：" + item);
                    }
                    text = ModMinecraft.McSkinDownload(text);
                    if (Data.IsCanceled)
                    {
                        throw new OperationCanceledException("当前任务已取消：" + item);
                    }
                    Data.Output = text;
                }
                catch (Exception ex)
                {
                    if (ex.IsCanceled())
                    {
                        Data.Output = "";
                        return;
                    }
                    if (ex.GetDisplay(false).Contains("(429)"))
                    {
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png";
                        Logger.Error(string.Format("获取正版皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else if (ex.GetDisplay(false).Contains("未设置自定义皮肤"))
                    {
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png";
                        Logger.Info("用户未设置自定义皮肤，跳过皮肤加载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else
                    {
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png";
                        Logger.Error(ex, string.Format("获取微软正版皮肤失败（{0}）", item), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                }
            }
            if (ModMain.FrmLoginMsSkin != null)
            {
                ModBase.RunInUi(new Action(ModMain.FrmLoginMsSkin.Skin.Load), false);
                return;
            }
            if (!Data.IsCanceled)
            {
                Data.Input = default(ValueTuple<string, string>);
            }
        }

        private static ValueTuple<string, string> SkinLegacyInput()
        {
            int num = Settings.Get<int>("LaunchSkinType", null);
            ValueTuple<string, string> valueTuple;
            if (num != 0)
            {
                if (num != 3)
                {
                    valueTuple = new ValueTuple<string, string>(num.ToString(), "");
                }
                else
                {
                    valueTuple = new ValueTuple<string, string>("3", Settings.Get<string>("LaunchSkinID", null));
                }
            }
            else
            {
                PageLoginLegacy frmLoginLegacy = ModMain.FrmLoginLegacy;
                if (frmLoginLegacy != null && frmLoginLegacy.IsReloaded)
                {
                    valueTuple = new ValueTuple<string, string>("0", ModMain.FrmLoginLegacy.ComboName.Text.Trim() ?? "");
                }
                else if (Operators.CompareString(Settings.Get<string>("LoginLegacyName", null), "", false) == 0)
                {
                    valueTuple = new ValueTuple<string, string>("0", "");
                }
                else
                {
                    valueTuple = new ValueTuple<string, string>("0", Settings.Get<string>("LoginLegacyName", null).ToString().BeforeFirst("\u00a8", false) ?? "");
                }
            }
            return valueTuple;
        }

        private static void SkinLegacyLoad(ModLoader.LoaderTask<ValueTuple<string, string>, string> Data)
        {
            ModBase.RunInUi((PageLaunchLeft._Closure___._I18_0 == null) ? (PageLaunchLeft._Closure___._I18_0 = new Action(PageLaunchLeft._Closure___._I._Lambda___18_0)) : PageLaunchLeft._Closure___._I18_0, false);
            switch (Conversions.ToInteger(Data.Input.Item1))
            {
            case 0:
                Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(Data.Input.Item2))) + ".png";
                goto IL_0278;
            case 1:
                break;
            case 2:
                Data.Output = ModBase.PathImage + "Skins/Alex.png";
                goto IL_0278;
            case 3:
            {
                string item = Data.Input.Item2;
                try
                {
                    if (item.Count<char>() < 2)
                    {
                        Data.Output = ModBase.PathImage + "Skins/Steve.png";
                    }
                    else
                    {
                        string text = Conversions.ToString(ModLaunch.McLoginMojangUuid(item, true));
                        if (Data.IsCanceled)
                        {
                            throw new OperationCanceledException("当前任务已取消：" + item);
                        }
                        text = ModMinecraft.McSkinGetAddress(text, "Mojang");
                        if (Data.IsCanceled)
                        {
                            throw new OperationCanceledException("当前任务已取消：" + item);
                        }
                        text = ModMinecraft.McSkinDownload(text);
                        if (Data.IsCanceled)
                        {
                            throw new OperationCanceledException("当前任务已取消：" + item);
                        }
                        Data.Output = text;
                    }
                    goto IL_0278;
                }
                catch (Exception ex)
                {
                    if (ex.IsCanceled())
                    {
                        Data.Output = "";
                        return;
                    }
                    if (ex.GetDisplay(false).Contains("(429)"))
                    {
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png";
                        Logger.Info(string.Format("获取离线登录使用的正版皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else
                    {
                        Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png";
                        Logger.Warn(ex, string.Format("获取离线登录使用的正版皮肤失败（{0}）", item), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    goto IL_0278;
                }
                goto IL_022B;
            }
            case 4:
                goto IL_022B;
            default:
                goto IL_0278;
            }
            IL_0093:
            Data.Output = ModBase.PathImage + "Skins/Steve.png";
            goto IL_0278;
            IL_022B:
            if (!FileUtils.Exists(Paths.AppDataThenName + "CustomSkin.png"))
            {
                ModMain.Hint("未找到离线皮肤自定义文件，可能它已被删除。PCL 将使用默认的 Steve 皮肤！", ModMain.HintType.Blue, true);
                Settings.Set("LaunchSkinType", 1, null);
                goto IL_0093;
            }
            Data.Output = Paths.AppDataThenName + "CustomSkin.png";
            IL_0278:
            if (ModMain.FrmLoginLegacy != null)
            {
                ModBase.RunInUi(new Action(ModMain.FrmLoginLegacy.Skin.Load), false);
                return;
            }
            if (!Data.IsCanceled)
            {
                Data.Input = default(ValueTuple<string, string>);
            }
        }

        private static ValueTuple<string, string> SkinNideInput()
        {
            return new ValueTuple<string, string>(Settings.Get<string>("CacheNideName", null), Settings.Get<string>("CacheNideUuid", null));
        }

        private static void SkinNideLoad(ModLoader.LoaderTask<ValueTuple<string, string>, string> Data)
        {
            ModBase.RunInUi((PageLaunchLeft._Closure___._I21_0 == null) ? (PageLaunchLeft._Closure___._I21_0 = new Action(PageLaunchLeft._Closure___._I._Lambda___21_0)) : PageLaunchLeft._Closure___._I21_0, false);
            string item = Data.Input.Item1;
            string item2 = Data.Input.Item2;
            if (Operators.CompareString(item, "", false) == 0)
            {
                Data.Output = ModBase.PathImage + "Skins/" + ModMinecraft.McSkinSex(Conversions.ToString(ModLaunch.McLoginLegacyUuid(item))) + ".png";
                Logger.Info("获取统一通行证皮肤失败，ID 为空", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
            }
            else
            {
                try
                {
                    string text = ModMinecraft.McSkinGetAddress(item2, "Nide");
                    if (Data.IsCanceled)
                    {
                        throw new OperationCanceledException("当前任务已取消：" + item);
                    }
                    text = ModMinecraft.McSkinDownload(text);
                    if (Data.IsCanceled)
                    {
                        throw new OperationCanceledException("当前任务已取消：" + item);
                    }
                    Data.Output = text;
                }
                catch (Exception ex)
                {
                    if (ex.IsCanceled())
                    {
                        Data.Output = "";
                        return;
                    }
                    if (ex.GetDisplay(false).Contains("(429)"))
                    {
                        Data.Output = ModBase.PathImage + "Skins/Steve.png";
                        Logger.Error(string.Format("获取统一通行证皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else if (ex.GetDisplay(false).Contains("未设置自定义皮肤"))
                    {
                        Data.Output = ModBase.PathImage + "Skins/Steve.png";
                        Logger.Info("用户未设置自定义皮肤，跳过皮肤加载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else
                    {
                        Data.Output = ModBase.PathImage + "Skins/Steve.png";
                        Logger.Error(ex, string.Format("获取统一通行证皮肤失败（{0}）", item), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                }
            }
            if (ModMain.FrmLoginNideSkin != null)
            {
                ModBase.RunInUi(new Action(ModMain.FrmLoginNideSkin.Skin.Load), false);
                return;
            }
            if (!Data.IsCanceled)
            {
                Data.Input = default(ValueTuple<string, string>);
            }
        }

        private static ValueTuple<string, string> SkinAuthInput()
        {
            return new ValueTuple<string, string>(Settings.Get<string>("CacheAuthName", null), Settings.Get<string>("CacheAuthUuid", null));
        }

        private static void SkinAuthLoad(ModLoader.LoaderTask<ValueTuple<string, string>, string> Data)
        {
            ModBase.RunInUi((PageLaunchLeft._Closure___._I24_0 == null) ? (PageLaunchLeft._Closure___._I24_0 = new Action(PageLaunchLeft._Closure___._I._Lambda___24_0)) : PageLaunchLeft._Closure___._I24_0, false);
            string item = Data.Input.Item1;
            string item2 = Data.Input.Item2;
            if (Operators.CompareString(item, "", false) == 0)
            {
                Data.Output = ModBase.PathImage + "Skins/Steve.png";
                Logger.Info("获取 Authlib-Injector 皮肤失败，ID 为空", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
            }
            else
            {
                try
                {
                    string text = ModMinecraft.McSkinGetAddress(item2, "Auth");
                    if (Data.IsCanceled)
                    {
                        throw new OperationCanceledException("当前任务已取消：" + item);
                    }
                    text = ModMinecraft.McSkinDownload(text);
                    if (Data.IsCanceled)
                    {
                        throw new OperationCanceledException("当前任务已取消：" + item);
                    }
                    Data.Output = text;
                }
                catch (Exception ex)
                {
                    if (ex.IsCanceled())
                    {
                        Data.Output = "";
                        return;
                    }
                    if (ex.GetDisplay(false).Contains("(429)"))
                    {
                        Data.Output = ModBase.PathImage + "Skins/Steve.png";
                        Logger.Error(string.Format("获取 Authlib-Injector 皮肤失败（{0}）：获取皮肤太过频繁，请 5 分钟后再试！", item), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else if (ex.GetDisplay(false).Contains("未设置自定义皮肤"))
                    {
                        Data.Output = ModBase.PathImage + "Skins/Steve.png";
                        Logger.Info("用户未设置自定义皮肤，跳过皮肤加载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else
                    {
                        Data.Output = ModBase.PathImage + "Skins/Steve.png";
                        Logger.Error(ex, string.Format("获取 Authlib-Injector 皮肤失败（{0}）", item), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                }
            }
            if (ModMain.FrmLoginAuthSkin != null)
            {
                ModBase.RunInUi(new Action(ModMain.FrmLoginAuthSkin.Skin.Load), false);
                return;
            }
            if (!Data.IsCanceled)
            {
                Data.Input = default(ValueTuple<string, string>);
            }
        }

        private void BtnVersion_Click()
        {
            if (ModLaunch.McLaunchLoader.State != ModBase.LoadState.Loading)
            {
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSelect, FormMain.PageSubType.Default);
            }
        }

        public void LaunchButtonClick()
        {
            if (ModLaunch.McLaunchLoader.State != ModBase.LoadState.Loading && this.BtnLaunch.IsEnabled && (ModMain.FrmMain.PageRight == null || ModMain.FrmMain.PageRight.PageState == OurPageRight.PageStates.ContentStay || ModMain.FrmMain.PageRight.PageState == OurPageRight.PageStates.ContentEnter))
            {
                if (ModMain.IsAprilEnabled && !ModMain.IsAprilGiveup)
                {
                    ModSecret.ThemeUnlock(12, false, "隐藏主题 滑稽彩 已解锁！");
                    ModMain.IsAprilGiveup = true;
                    Settings.Set("AprilYear", DateTime.Now.Year, null);
                    ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleX = 1.0;
                    ModMain.FrmLaunchLeft.AprilScaleTrans.ScaleY = 1.0;
                    ModMain.FrmLaunchLeft.AprilPosTrans.X = 0.0;
                    ModMain.FrmLaunchLeft.AprilPosTrans.Y = 0.0;
                    ModMain.FrmMain.BtnExtraApril.ShowRefresh();
                }
                if (Operators.CompareString(this.BtnLaunch.Text, "启动游戏", false) == 0)
                {
                    ModLaunch.McLaunchStart(null);
                    return;
                }
                if (Operators.CompareString(this.BtnLaunch.Text, "下载游戏", false) == 0)
                {
                    ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.Default);
                }
            }
        }

        public void RefreshButtonsUI()
        {
            if (this.BtnLaunch.IsLoaded)
            {
                int num;
                if (!this.IsLoadFinished || ModMinecraft.McInstanceListLoader.State == ModBase.LoadState.Loading || ModMinecraft.McFolderListLoader.State == ModBase.LoadState.Loading)
                {
                    num = 0;
                }
                else if (((object)(ModMinecraft.McInstanceSelected) == (object)null))
                {
                    if (Settings.Get<bool>("UiHiddenPageDownload", null) && !PageSetupUI.HiddenForceShow)
                    {
                        num = 1;
                    }
                    else
                    {
                        num = 2;
                    }
                }
                else
                {
                    num = 3;
                }
                if (num != this.BtnLaunchState || Operators.CompareString((((object)(ModMinecraft.McInstanceSelected) == (object)null)) ? "" : ModMinecraft.McInstanceSelected.PathVersion, (((object)(this.BtnLaunchInstance) == (object)null)) ? "" : this.BtnLaunchInstance.PathVersion, false) != 0)
                {
                    this.BtnLaunchInstance = ModMinecraft.McInstanceSelected;
                    this.BtnLaunchState = num;
                    switch (num)
                    {
                    case 0:
                        Logger.Info("启动按钮：正在加载 Minecraft 版本", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                        ModMain.FrmLaunchLeft.BtnLaunch.Text = "正在加载";
                        ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = false;
                        ModMain.FrmLaunchLeft.LabVersion.Text = "正在加载中，请稍候";
                        ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = false;
                        ModMain.FrmLaunchLeft.BtnMore.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        Logger.Info("启动按钮：无 Minecraft 版本，下载已禁用", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                        ModMain.FrmLaunchLeft.BtnLaunch.Text = "启动游戏";
                        ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = false;
                        ModMain.FrmLaunchLeft.LabVersion.Text = "未找到可用的游戏版本";
                        ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = true;
                        ModMain.FrmLaunchLeft.BtnMore.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        Logger.Info("启动按钮：无 Minecraft 版本，要求下载", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                        ModMain.FrmLaunchLeft.BtnLaunch.Text = "下载游戏";
                        ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = true;
                        ModMain.FrmLaunchLeft.LabVersion.Text = "未找到可用的游戏版本";
                        ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = true;
                        ModMain.FrmLaunchLeft.BtnMore.Visibility = Visibility.Collapsed;
                        break;
                    case 3:
                        Logger.Info(string.Format("启动按钮：Minecraft 版本：{0}", ModMinecraft.McInstanceSelected.PathVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                        ModMain.FrmLaunchLeft.BtnLaunch.Text = "启动游戏";
                        ModMain.FrmLaunchLeft.BtnVersion.IsEnabled = true;
                        ModMain.FrmLaunchLeft.BtnLaunch.IsEnabled = true;
                        ModMain.FrmLaunchLeft.LabVersion.Text = ModMinecraft.McInstanceSelected.Name;
                        break;
                    }
                }
                ModMain.FrmLaunchLeft.BtnVersion.Visibility = ((!PageSetupUI.HiddenForceShow && Settings.Get<bool>("UiHiddenFunctionSelect", null)) ? Visibility.Collapsed : Visibility.Visible);
                if (num == 3)
                {
                    ModMain.FrmLaunchLeft.BtnMore.Visibility = ModMain.FrmLaunchLeft.BtnVersion.Visibility;
                }
            }
        }

        public void BtnCancel_Click()
        {
            if (ModLaunch.McLaunchLoaderReal != null)
            {
                ModLaunch.McLaunchLoaderReal.Cancel();
                ModLaunch.McLaunchLog("已取消启动", LogLevel.Info);
                try
                {
                    if (ModLaunch.McLaunchWatcher != null)
                    {
                        ModLaunch.McLaunchWatcher.Kill();
                    }
                    else if (ModLaunch.McLaunchProcess != null && !ModLaunch.McLaunchProcess.HasExited)
                    {
                        ModLaunch.McLaunchProcess.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "取消启动结束进程失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                }
            }
        }

        private void BtnMore_Click()
        {
            if (ModLaunch.McLaunchLoader.State != ModBase.LoadState.Loading)
            {
                if (ModMinecraft.McInstanceLock.IsLocked(ModMinecraft.McInstanceSelected))
                {
                    ModMain.Hint("该版本正在被修改，暂时无法进入版本设置！", ModMain.HintType.Red, true);
                    return;
                }
                ModMinecraft.McInstanceSelected.Load();
                PageInstanceLeft.Instance = ModMinecraft.McInstanceSelected;
                ModMain.FrmMain.PageChange(FormMain.PageType.InstanceSetup, FormMain.PageSubType.Default);
            }
        }

        public void LaunchingRefresh()
        {
            try
            {
                PageLaunchLeft._Closure___33_0 CS_locals1 = new PageLaunchLeft._Closure___33_0(null);
                CS_locals1._VB_Me = this;
                if (ModLaunch.McLaunchLoaderReal.State != ModBase.LoadState.Canceled)
                {
                    bool flag = false;
                    try
                    {
                        try
                        {
                            foreach (ModLoader.LoaderBase loaderBase in ModLaunch.McLaunchLoaderReal.GetLoaderList(false))
                            {
                                if (loaderBase.State == ModBase.LoadState.Loading || loaderBase.State == ModBase.LoadState.Waiting)
                                {
                                    this.LabLaunchingStage.Text = loaderBase.Name;
                                    flag = Operators.CompareString(loaderBase.Name, "等待游戏窗口出现", false) == 0 || Operators.CompareString(loaderBase.Name, "结束处理", false) == 0;
                                    goto IL_00E2;
                                }
                            }
                        }
                        finally
                        {
                            List<ModLoader.LoaderBase>.Enumerator enumerator = default;
                            ((IDisposable)enumerator).Dispose();
                        }
                        this.LabLaunchingStage.Text = "已完成";
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "获取是否启动完成失败，可能是由于启动状态改变导致集合已修改", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                        return;
                    }
                    IL_00E2:
                    this.LabLaunchingTitle.Text = ((flag || ModLaunch.McLaunchLoaderReal.State == ModBase.LoadState.Finished) ? "已启动游戏" : ((ModLaunch.CurrentLaunchOptions.SaveBatch == null) ? "正在启动游戏" : "正在导出启动脚本"));
                    if (ModAnimation.AniIsRun("Launch State Page"))
                    {
                        flag = false;
                    }
                    double progress = ModLaunch.McLaunchLoaderReal.Progress;
                    if (progress >= this.ShowProgress)
                    {
                        ref double ptr = ref this.ShowProgress;
                        this.ShowProgress = ptr + ((progress - this.ShowProgress) * 0.1 + 0.0025);
                    }
                    if (progress <= this.ShowProgress)
                    {
                        this.ShowProgress = progress;
                    }
                    if (flag)
                    {
                        this.ShowProgress = 1.0;
                    }
                    this.LabLaunchingProgress.Text = (this.ShowProgress * 100.0).ToString("0.00") + " %";
                    CS_locals1._VB_Local_HasLaunchDownloader = false;
                    try
                    {
                        if (ModNet.NetManager.Speed == 0L && this.LabLaunchingDownload.Visibility == Visibility.Collapsed)
                        {
                            CS_locals1._VB_Local_HasLaunchDownloader = false;
                        }
                        else
                        {
                            try
                            {
                                foreach (ModNet.LoaderDownload loaderDownload in ModNet.NetManager.Tasks)
                                {
                                    if (loaderDownload.RealParent != null && Operators.CompareString(loaderDownload.RealParent.Name, "Minecraft 启动", false) == 0 && loaderDownload.State == ModBase.LoadState.Loading)
                                    {
                                        CS_locals1._VB_Local_HasLaunchDownloader = true;
                                    }
                                }
                            }
                            finally
                            {
                                IEnumerator<ModNet.LoaderDownload> enumerator2 = default;
                                if (enumerator2 != null)
                                {
                                    enumerator2.Dispose();
                                }
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, "获取 Minecraft 启动下载器失败，可能是因为启动被取消", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                        CS_locals1._VB_Local_HasLaunchDownloader = false;
                    }
                    this.LabLaunchingDownload.Text = StringUtils.FormatByteSize(ModNet.NetManager.Speed) + "/s";
                    List<ModAnimation.AniData> list = new List<ModAnimation.AniData>
                    {
                        ModAnimation.AaGridLengthWidth(this.ProgressLaunchingFinished, this.ShowProgress - this.ProgressLaunchingFinished.Width.Value, 130, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                        ModAnimation.AaGridLengthWidth(this.ProgressLaunchingUnfinished, 1.0 - this.ShowProgress - this.ProgressLaunchingUnfinished.Width.Value, 130, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                    };
                    if (CS_locals1._VB_Local_HasLaunchDownloader == (this.LabLaunchingDownload.Visibility == Visibility.Collapsed))
                    {
                        this.LabLaunchingDownload.Visibility = Visibility.Visible;
                        this.LabLaunchingDownloadLeft.Visibility = Visibility.Visible;
                        list.AddRange(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaOpacity(this.LabLaunchingDownload, (CS_locals1._VB_Local_HasLaunchDownloader ? 1.0 : 0.0) - this.LabLaunchingDownload.Opacity, 90, 0, null, false),
                            ModAnimation.AaOpacity(this.LabLaunchingDownloadLeft, (CS_locals1._VB_Local_HasLaunchDownloader ? 0.5 : 0.0) - this.LabLaunchingDownloadLeft.Opacity, 90, 0, null, false),
                            ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___0), 110, false)
                        });
                    }
                    if (!flag == (this.LabLaunchingProgress.Visibility == Visibility.Collapsed))
                    {
                        this.LabLaunchingProgress.Visibility = Visibility.Visible;
                        this.LabLaunchingProgressLeft.Visibility = Visibility.Visible;
                        if (flag)
                        {
                            this.PanLaunchingHint.Visibility = Visibility.Visible;
                        }
                        list.AddRange(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaOpacity(this.LabLaunchingProgress, (!flag ? 1.0 : 0.0) - this.LabLaunchingProgress.Opacity, 90, 0, null, false),
                            ModAnimation.AaOpacity(this.LabLaunchingProgressLeft, ((!flag) ? 0.5 : 0.0) - this.LabLaunchingProgressLeft.Opacity, 90, 0, null, false),
                            ModAnimation.AaOpacity(this.PanLaunchingHint, (flag ? 1.0 : 0.0) - this.PanLaunchingHint.Opacity, 90, 0, null, false)
                        });
                    }
                    ModAnimation.AniStart(list, "Launching Progress", false);
                }
            }
            catch (Exception ex3)
            {
                Logger.Error(ex3, "刷新启动信息失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
            }
        }

        private void PanLaunchingInfo_SizeChangedW(object sender, SizeChangedEventArgs e)
        {
            double num = e.NewSize.Width - e.PreviousSize.Width;
            if (e.PreviousSize.Width != 0.0 && !this.IsWidthAnimating && Math.Abs(num) >= 1.0 && this.PanLaunchingInfo.ActualWidth != 0.0)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaWidth(this.PanLaunchingInfo, num, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode(new ThreadStart(this._Lambda___37_0), 0, true)
                }, "Launching Info Width", false);
                this.IsWidthAnimating = true;
                this.ActualUsedWidth = this.PanLaunchingInfo.Width;
                this.PanLaunchingInfo.Width = e.PreviousSize.Width;
            }
        }

        private void PanLaunchingInfo_SizeChangedH(object sender, SizeChangedEventArgs e)
        {
            double num = e.NewSize.Height - e.PreviousSize.Height;
            if (e.PreviousSize.Height != 0.0 && !this.IsHeightAnimating && Math.Abs(num) >= 1.0 && this.PanLaunchingInfo.ActualHeight != 0.0)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaHeight(this.PanLaunchingInfo, num, 180, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode(new ThreadStart(this._Lambda___40_0), 0, true)
                }, "Launching Info Height", false);
                this.IsHeightAnimating = true;
                this.ActualUsedHeight = this.PanLaunchingInfo.Height;
                this.PanLaunchingInfo.Height = e.PreviousSize.Height;
            }
        }



        internal virtual OurButton BtnVersion_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnVersion;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R51_4);
                OurButton ourButton = this._BtnVersion;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnVersion = value;
                ourButton = this._BtnVersion;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurButton BtnMore_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnMore;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R55_5);
                OurButton ourButton = this._BtnMore;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnMore = value;
                ourButton = this._BtnMore;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }






        internal virtual OurRadioButton RadioLoginType5_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RadioLoginType5;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurRadioButton.CheckEventHandler checkEventHandler = new OurRadioButton.CheckEventHandler(this.RadioLoginType_Change);
                OurRadioButton ourRadioButton = this._RadioLoginType5;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check -= checkEventHandler;
                }
                this._RadioLoginType5 = value;
                ourRadioButton = this._RadioLoginType5;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurRadioButton RadioLoginType0_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._RadioLoginType0;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurRadioButton.CheckEventHandler checkEventHandler = new OurRadioButton.CheckEventHandler(this.RadioLoginType_Change);
                OurRadioButton ourRadioButton = this._RadioLoginType0;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check -= checkEventHandler;
                }
                this._RadioLoginType0 = value;
                ourRadioButton = this._RadioLoginType0;
                if (ourRadioButton != null)
                {
                    ourRadioButton.Check += checkEventHandler;
                }
            }
        }



        internal virtual OurButton BtnLaunch_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnLaunch;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R95_6);
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R95_7);
                OurButton ourButton = this._BtnLaunch;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                    ourButton.Loaded -= routedEventHandler;
                }
                this._BtnLaunch = value;
                ourButton = this._BtnLaunch;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                    ourButton.Loaded += routedEventHandler;
                }
            }
        }








        internal virtual Grid PanLaunchingInfo_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._PanLaunchingInfo;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this.PanLaunchingInfo_SizeChangedW);
                SizeChangedEventHandler sizeChangedEventHandler2 = new SizeChangedEventHandler(this.PanLaunchingInfo_SizeChangedH);
                Grid grid = this._PanLaunchingInfo;
                if (grid != null)
                {
                    grid.SizeChanged -= sizeChangedEventHandler;
                    grid.SizeChanged -= sizeChangedEventHandler2;
                }
                this._PanLaunchingInfo = value;
                grid = this._PanLaunchingInfo;
                if (grid != null)
                {
                    grid.SizeChanged += sizeChangedEventHandler;
                    grid.SizeChanged += sizeChangedEventHandler2;
                }
            }
        }









        internal virtual OurButton BtnCancel_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnCancel;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this._Lambda___R163_8);
                OurButton ourButton = this._BtnCancel;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnCancel = value;
                ourButton = this._BtnCancel;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, RoutedEventArgs a1)
        {
            this.PageLaunchLeft_Loaded();
        }

        [CompilerGenerated]
        private void _Lambda___R4_2(OurLoading.OurLoadingState a0, OurLoading.OurLoadingState a1)
        {
            this.RefreshButtonsUI();
        }

        [CompilerGenerated]
        private void _Lambda___R4_3(OurLoading.OurLoadingState a0, OurLoading.OurLoadingState a1)
        {
            this.RefreshButtonsUI();
        }

        [CompilerGenerated]
        private void _Lambda___4_0()
        {
            PageLaunchLeft._Closure___4_0 CS_locals1 = new PageLaunchLeft._Closure___4_0();
            CS_locals1._VB_Me = this;
            string text = null;
            if (FileUtils.Exists(Paths.Base + "modpack.zip"))
            {
                text = Paths.Base + "modpack.zip";
            }
            if (FileUtils.Exists(Paths.Base + "modpack.mrpack"))
            {
                text = Paths.Base + "modpack.mrpack";
            }
            if (text != null)
            {
                Logger.Warn(string.Format("需自动安装整合包：{0}", text), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                PageSelectLeft.CreateMcFolderInCurrentPath();
                ModMinecraft.McFolderListLoader.WaitForExit(null, null, false, default(CancellationToken), null);
            }
            if (Operators.CompareString(ModMinecraft.McFolderSelected, "", false) == 0 || !DirectoryUtils.Exists(ModMinecraft.McFolderSelected))
            {
                if (Operators.CompareString(ModMinecraft.McFolderSelected, "", false) == 0)
                {
                    Logger.Info("没有已储存的 Minecraft 文件夹", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                }
                else
                {
                    Logger.Warn(string.Format("Minecraft 文件夹无效，该文件夹已不存在：{0}", ModMinecraft.McFolderSelected), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                }
                ModMinecraft.McFolderListLoader.WaitForExit(null, null, true, default(CancellationToken), null);
                ModMinecraft.McFolderSelected = ModMinecraft.McFolderList.First<ModMinecraft.McFolder>().Location;
            }
            if (Settings.Get<bool>("SystemDebugDelay", null))
            {
                Thread.Sleep(ModBase.RandomInteger(500, 3000));
            }
            if (text != null)
            {
                try
                {
                    ModLoader.LoaderCombo<string> loaderCombo = ModModpack.ModpackInstall(text, null, null);
                    Logger.Info(string.Format("自动安装整合包已开始：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    loaderCombo.WaitForExit(null, null, false, default(CancellationToken), null);
                    if (loaderCombo.State == ModBase.LoadState.Finished)
                    {
                        Logger.Info(string.Format("自动安装整合包成功，清理安装包：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                        FileUtils.Delete(text, false);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.IsCanceled())
                    {
                        Logger.Info(string.Format("自动安装整合包已取消：{0}", text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                    else
                    {
                        Logger.Error(ex, string.Format("自动安装整合包失败：{0}", text), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                    }
                }
            }
            string text2 = ModBase.ReadIni(ModMinecraft.McFolderSelected + "PCL.ini", "Version", "");
            CS_locals1._VB_Local_Instance = ((Operators.CompareString(text2, "", false) == 0) ? null : new ModMinecraft.McInstance(text2));
            if (((object)(CS_locals1._VB_Local_Instance) == (object)null) || !PathUtils.IsParentOf(ModMinecraft.McFolderSelected, CS_locals1._VB_Local_Instance.PathVersion) || !CS_locals1._VB_Local_Instance.Check())
            {
                Logger.Info(string.Format("当前选择的 Minecraft 版本无效：{0}", (((object)(CS_locals1._VB_Local_Instance) == (object)null)) ? "null" : CS_locals1._VB_Local_Instance.PathVersion), Information.IsNothing(CS_locals1._VB_Local_Instance) ? LogBehavior.None : LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                if (ModMinecraft.McInstanceListLoader.State != ModBase.LoadState.Finished)
                {
                    ModLoader.LoaderFolderRun(ModMinecraft.McInstanceListLoader, ModMinecraft.McFolderSelected, ModLoader.LoaderFolderRunType.ForceRun, 1, "versions\\", true);
                }
                if (!ModMinecraft.McInstanceList.Any<KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>>() || ModMinecraft.McInstanceList.First<KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>>().Value[0].Logo.Contains("RedstoneBlock"))
                {
                    CS_locals1._VB_Local_Instance = null;
                    Logger.Info("无可用 Minecraft 版本", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                }
                else
                {
                    CS_locals1._VB_Local_Instance = ModMinecraft.McInstanceList.First<KeyValuePair<ModMinecraft.McInstanceCardType, List<ModMinecraft.McInstance>>>().Value[0];
                    Logger.Info(string.Format("自动选择 Minecraft 版本：{0}", CS_locals1._VB_Local_Instance.PathVersion), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLaunchLeft.xaml.vb");
                }
            }
            ModBase.RunInUi(new Action(CS_locals1._Lambda___1), false);
        }

        [CompilerGenerated]
        private void _Lambda___4_2()
        {
            Thread.Sleep(500);
            ModBase.RunInUi((PageLaunchLeft._Closure___._I4_3 == null) ? (PageLaunchLeft._Closure___._I4_3 = new Action(PageLaunchLeft._Closure___._I._Lambda___4_3)) : PageLaunchLeft._Closure___._I4_3, false);
            Thread.Sleep(500);
            ModBase.RunInUi((PageLaunchLeft._Closure___._I4_4 == null) ? (PageLaunchLeft._Closure___._I4_4 = new Action(PageLaunchLeft._Closure___._I._Lambda___4_4)) : PageLaunchLeft._Closure___._I4_4, false);
            Thread.Sleep(500);
            ModBase.RunInUi((PageLaunchLeft._Closure___._I4_5 == null) ? (PageLaunchLeft._Closure___._I4_5 = new Action(PageLaunchLeft._Closure___._I._Lambda___4_5)) : PageLaunchLeft._Closure___._I4_5, false);
            Thread.Sleep(500);
            ModBase.RunInUi((PageLaunchLeft._Closure___._I4_6 == null) ? (PageLaunchLeft._Closure___._I4_6 = new Action(PageLaunchLeft._Closure___._I._Lambda___4_6)) : PageLaunchLeft._Closure___._I4_6, false);
            Thread.Sleep(500);
            ModBase.RunInUi(new Action(this._Lambda___4_7), false);
            Thread.Sleep(500);
            ModBase.RunInUi(new Action(this._Lambda___4_8), false);
            Thread.Sleep(500);
            FormMain.EndProgramForce((ModBase.ProcessReturnValues)123);
        }

        [CompilerGenerated]
        private void _Lambda___4_7()
        {
            this.BtnVersion_Click();
        }

        [CompilerGenerated]
        private void _Lambda___4_8()
        {
            this.BtnMore_Click();
        }

        [CompilerGenerated]
        private void _Lambda___5_0()
        {
            this.PanLaunching.IsHitTestVisible = true;
        }

        [CompilerGenerated]
        private void _Lambda___6_0()
        {
            this.PanInput.IsHitTestVisible = true;
        }

        [CompilerGenerated]
        private void _Lambda___37_0()
        {
            this.IsWidthAnimating = false;
            this.PanLaunchingInfo.Width = this.ActualUsedWidth;
        }

        [CompilerGenerated]
        private void _Lambda___40_0()
        {
            this.IsHeightAnimating = false;
            this.PanLaunchingInfo.Height = this.ActualUsedHeight;
        }

        [CompilerGenerated]
        private void _Lambda___R51_4(object a0, MouseButtonEventArgs a1)
        {
            this.BtnVersion_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R55_5(object a0, MouseButtonEventArgs a1)
        {
            this.BtnMore_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R95_6(object a0, MouseButtonEventArgs a1)
        {
            this.LaunchButtonClick();
        }

        [CompilerGenerated]
        private void _Lambda___R95_7(object a0, RoutedEventArgs a1)
        {
            this.RefreshButtonsUI();
        }

        [CompilerGenerated]
        private void _Lambda___R163_8(object a0, MouseButtonEventArgs a1)
        {
            this.BtnCancel_Click();
        }

        private bool IsLoad;

        private bool IsLoadFinished;

        private PageLaunchLeft.PageType PageCurrent;

        public static ModLoader.LoaderTask<ValueTuple<string, string>, string> SkinMs = new ModLoader.LoaderTask<ValueTuple<string, string>, string>("Loader Skin Ms", new Action<ModLoader.LoaderTask<ValueTuple<string, string>, string>>(PageLaunchLeft.SkinMsLoad), new Func<ValueTuple<string, string>>(PageLaunchLeft.SkinMsInput), ThreadPriority.AboveNormal);

        public static ModLoader.LoaderTask<ValueTuple<string, string>, string> SkinLegacy = new ModLoader.LoaderTask<ValueTuple<string, string>, string>("Loader Skin Legacy", new Action<ModLoader.LoaderTask<ValueTuple<string, string>, string>>(PageLaunchLeft.SkinLegacyLoad), new Func<ValueTuple<string, string>>(PageLaunchLeft.SkinLegacyInput), ThreadPriority.AboveNormal);

        public static ModLoader.LoaderTask<ValueTuple<string, string>, string> SkinNide = new ModLoader.LoaderTask<ValueTuple<string, string>, string>("Loader Skin Nide", new Action<ModLoader.LoaderTask<ValueTuple<string, string>, string>>(PageLaunchLeft.SkinNideLoad), new Func<ValueTuple<string, string>>(PageLaunchLeft.SkinNideInput), ThreadPriority.AboveNormal);

        public static ModLoader.LoaderTask<ValueTuple<string, string>, string> SkinAuth = new ModLoader.LoaderTask<ValueTuple<string, string>, string>("Loader Skin Auth", new Action<ModLoader.LoaderTask<ValueTuple<string, string>, string>>(PageLaunchLeft.SkinAuthLoad), new Func<ValueTuple<string, string>>(PageLaunchLeft.SkinAuthInput), ThreadPriority.AboveNormal);

        public static List<ModLoader.LoaderTask<ValueTuple<string, string>, string>> SkinLoaders = new List<ModLoader.LoaderTask<ValueTuple<string, string>, string>>
        {
            PageLaunchLeft.SkinMs,
            PageLaunchLeft.SkinLegacy,
            PageLaunchLeft.SkinNide,
            PageLaunchLeft.SkinAuth
        };

        private int BtnLaunchState;

        private ModMinecraft.McInstance BtnLaunchInstance;

        private double ShowProgress;

        private bool IsWidthAnimating;

        private double ActualUsedWidth;

        private bool IsHeightAnimating;

        private double ActualUsedHeight;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private PageLaunchLeft _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PanInput")]
        private Grid _PanInput;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnVersion")]
        private OurButton _BtnVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnMore")]
        private OurButton _BtnMore;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLogin")]
        private Grid _PanLogin;

        [CompilerGenerated]
        [AccessedThroughProperty("PanTypeOne")]
        private Grid _PanTypeOne;

        [CompilerGenerated]
        [AccessedThroughProperty("PathTypeOne")]
        private Path _PathTypeOne;

        [CompilerGenerated]
        [AccessedThroughProperty("LabTypeOne")]
        private TextBlock _LabTypeOne;

        [CompilerGenerated]
        [AccessedThroughProperty("PanType")]
        private Grid _PanType;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLoginType5")]
        private OurRadioButton _RadioLoginType5;

        [CompilerGenerated]
        [AccessedThroughProperty("RadioLoginType0")]
        private OurRadioButton _RadioLoginType0;

        [CompilerGenerated]
        [AccessedThroughProperty("AprilScaleTrans")]
        private ScaleTransform _AprilScaleTrans;

        [CompilerGenerated]
        [AccessedThroughProperty("AprilPosTrans")]
        private TranslateTransform _AprilPosTrans;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnLaunch")]
        private OurButton _BtnLaunch;

        [CompilerGenerated]
        [AccessedThroughProperty("LabVersion")]
        private TextBlock _LabVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLaunching")]
        private Grid _PanLaunching;

        [CompilerGenerated]
        [AccessedThroughProperty("LoadLaunching")]
        private OurLoading _LoadLaunching;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingTitle")]
        private TextBlock _LabLaunchingTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingName")]
        private TextBlock _LabLaunchingName;

        [CompilerGenerated]
        [AccessedThroughProperty("ProgressLaunchingFinished")]
        private ColumnDefinition _ProgressLaunchingFinished;

        [CompilerGenerated]
        [AccessedThroughProperty("ProgressLaunchingUnfinished")]
        private ColumnDefinition _ProgressLaunchingUnfinished;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLaunchingInfo")]
        private Grid _PanLaunchingInfo;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingStage")]
        private TextBlock _LabLaunchingStage;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingMethod")]
        private TextBlock _LabLaunchingMethod;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingProgressLeft")]
        private TextBlock _LabLaunchingProgressLeft;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingProgress")]
        private TextBlock _LabLaunchingProgress;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingDownloadLeft")]
        private TextBlock _LabLaunchingDownloadLeft;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingDownload")]
        private TextBlock _LabLaunchingDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("PanLaunchingHint")]
        private Grid _PanLaunchingHint;

        [CompilerGenerated]
        [AccessedThroughProperty("LabLaunchingHint")]
        private TextBlock _LabLaunchingHint;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnCancel")]
        private OurButton _BtnCancel;

        private enum PageType
        {
            None,
            Legacy,
            Nide,
            NideSkin,
            Auth,
            AuthSkin,
            Ms,
            MsSkin
        }

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___4_3()
            {
                ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.Default);
            }

            internal void _Lambda___4_4()
            {
                ModMain.FrmMain.PageChange(FormMain.PageType.Download, FormMain.PageSubType.DownloadMod);
            }

            internal void _Lambda___4_5()
            {
                ModMain.FrmMain.PageChange(FormMain.PageType.Setup, FormMain.PageSubType.Default);
            }

            internal void _Lambda___4_6()
            {
                ModMain.FrmMain.PageChange(FormMain.PageType.Other, FormMain.PageSubType.Default);
            }

            internal void _Lambda___15_0()
            {
                if (ModMain.FrmLoginMsSkin != null && ModMain.FrmLoginMsSkin.Skin != null)
                {
                    ModMain.FrmLoginMsSkin.Skin.Clear();
                }
            }

            internal void _Lambda___18_0()
            {
                if (ModMain.FrmLoginLegacy != null && ModMain.FrmLoginLegacy.Skin != null)
                {
                    ModMain.FrmLoginLegacy.Skin.Clear();
                }
            }

            internal void _Lambda___21_0()
            {
                if (ModMain.FrmLoginNideSkin != null && ModMain.FrmLoginNideSkin.Skin != null)
                {
                    ModMain.FrmLoginNideSkin.Skin.Clear();
                }
            }

            internal void _Lambda___24_0()
            {
                if (ModMain.FrmLoginAuthSkin != null && ModMain.FrmLoginAuthSkin.Skin != null)
                {
                    ModMain.FrmLoginAuthSkin.Skin.Clear();
                }
            }

            public static readonly PageLaunchLeft._Closure___ _I = new PageLaunchLeft._Closure___();

            public static Action _I4_3;

            public static Action _I4_4;

            public static Action _I4_5;

            public static Action _I4_6;

            public static Action _I15_0;

            public static Action _I18_0;

            public static Action _I21_0;

            public static Action _I24_0;
        }

        [CompilerGenerated]
        private sealed class _Closure___10_0
        {
            internal void _Lambda___0()
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this._VB_Me.PanLogin, -this._VB_Me.PanLogin.Opacity, 100, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaCode((this._I1 == null) ? (this._I1 = new ThreadStart(this._Lambda___1)) : this._I1, 100, false),
                    ModAnimation.AaOpacity(this._VB_Me.PanLogin, 1.0, 100, 120, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false)
                }, "FrmLogin PageChange", false);
            }

            internal void _Lambda___1()
            {
                checked
                {
                    ModAnimation.AniControlEnabled++;
                    this._VB_Me.PanLogin.Children.Clear();
                    this._VB_Me.PanLogin.Children.Add((UIElement)this._VB_Local_PageNew);
                    ModAnimation.AniControlEnabled--;
                }
            }

            public object _VB_Local_PageNew;

            public PageLaunchLeft _VB_Me;

            public ThreadStart _I1;
        }

        [CompilerGenerated]
        private sealed class _Closure___33_0
        {
            public _Closure___33_0(PageLaunchLeft._Closure___33_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_HasLaunchDownloader = arg0._VB_Local_HasLaunchDownloader;
                }
            }

            internal void _Lambda___0()
            {
                if (!this._VB_Local_HasLaunchDownloader)
                {
                    this._VB_Me.LabLaunchingDownload.Visibility = Visibility.Collapsed;
                    this._VB_Me.LabLaunchingDownloadLeft.Visibility = Visibility.Collapsed;
                }
            }

            public bool _VB_Local_HasLaunchDownloader;

            public PageLaunchLeft _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___4_0
        {
            internal void _Lambda___1()
            {
                ModMinecraft.McInstanceSelected = this._VB_Local_Instance;
                this._VB_Me.IsLoadFinished = true;
                this._VB_Me.RefreshButtonsUI();
                this._VB_Me.RefreshPage(false, false);
                if (Operators.CompareString(ModLaunch.McLoginAble(), "", false) == 0)
                {
                    ModLaunch.McLoginLoader.Start(null, false);
                }
                if (Environment.CommandLine.Contains("--test"))
                {
                    ModBase.RunInThread(new Action(this._VB_Me._Lambda___4_2));
                }
            }

            public ModMinecraft.McInstance _VB_Local_Instance;

            public PageLaunchLeft _VB_Me;
        }
    }
}
