using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLoginAuthSkin : Grid
    {
        public PageLoginAuthSkin()
        {
            base.Loaded += this._Lambda___R0_1;
            this.InitializeComponent();
            this.PanData_WithEvents = this.PanData;
            this.BtnEdit_WithEvents = this.BtnEdit;
            this.BtnExit_WithEvents = this.BtnExit;
            this.Skin_WithEvents = this.Skin;
            this.Skin.Loader = PageLaunchLeft.SkinAuth;
        }

        private void PageLoginLegacy_Loaded()
        {
            this.Skin.Loader.Start(null, false);
        }

        public void Reload(bool KeepInput)
        {
            this.TextName.Text = Settings.Get<string>("CacheAuthName", null);
            this.TextEmail.Text = Settings.Get<string>("CacheAuthUsername", null);
            this.TextEmail.Visibility = (Settings.Get<bool>("UiLauncherEmail", null) ? Visibility.Collapsed : Visibility.Visible);
            this.PageLoginLegacy_Loaded();
        }

        public static ModLaunch.McLoginServer GetLoginData()
        {
            string text = (Information.IsNothing(ModMinecraft.McInstanceSelected) ? Settings.Get<string>("CacheAuthServerServer", null) : Settings.Get<string>("VersionServerAuthServer", ModMinecraft.McInstanceSelected)) + "/authserver";
            return new ModLaunch.McLoginServer(ModLaunch.McLoginType.Auth)
            {
                Token = "Auth",
                BaseUrl = text,
                UserName = Settings.Get<string>("CacheAuthUsername", null),
                Password = Settings.Get<string>("CacheAuthPass", null),
                Description = "Authlib-Injector",
                Type = ModLaunch.McLoginType.Auth
            };
        }

        private void PageLoginAuthSkin_MouseEnter(object sender, MouseEventArgs e)
        {
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.BtnEdit, 1.0 - this.BtnEdit.Opacity, 80, 0, null, false),
                ModAnimation.AaHeight(this.BtnEdit, 25.5 - this.BtnEdit.Height, 140, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnEdit, -1.5, 50, 140, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaOpacity(this.BtnExit, 1.0 - this.BtnExit.Opacity, 80, 0, null, false),
                ModAnimation.AaHeight(this.BtnExit, 25.5 - this.BtnExit.Height, 140, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnExit, -1.5, 50, 140, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false)
            }, "PageLoginAuthSkin Button", false);
        }

        private void PageLoginAuthSkin_MouseLeave(object sender, MouseEventArgs e)
        {
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.BtnEdit, -this.BtnEdit.Opacity, 120, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnEdit, 14.0 - this.BtnEdit.Height, 120, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaOpacity(this.BtnExit, -this.BtnExit.Opacity, 120, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnExit, 14.0 - this.BtnExit.Height, 120, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false)
            }, "PageLoginAuthSkin Button", false);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (ModLaunch.McLoginLoader.State != ModBase.LoadState.Loading)
            {
                ModMain.Hint("正在尝试更换，请稍候！", ModMain.HintType.Blue, true);
                Settings.Set("CacheAuthUuid", "", null);
                Settings.Set("CacheAuthName", "", null);
                ModBase.RunInThread(new Action(this._Lambda___6_0));
                return;
            }
            Logger.Warn("要求更换角色，但登录加载器繁忙", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginAuthSkin.xaml.vb");
            if (ModLaunch.McLoginLoader.Input is ModLaunch.McLoginServer && ((ModLaunch.McLoginServer)ModLaunch.McLoginLoader.Input).ForceReselectProfile)
            {
                ModMain.Hint("正在尝试更换，请稍候！", ModMain.HintType.Blue, true);
                return;
            }
            ModMain.Hint("正在登录中，请稍后再更换角色！", ModMain.HintType.Red, true);
        }

        public static void ExitLogin()
        {
            Settings.Set("CacheAuthAccess", "", null);
            Settings.Set("CacheAuthUuid", "", null);
            Settings.Set("CacheAuthName", "", null);
            ModLaunch.McLoginAuthLoader.Input = null;
            ModMain.FrmLaunchLeft.RefreshPage(false, true);
        }

        private void Skin_Click(object sender, MouseButtonEventArgs e)
        {
            string text = ((((object)(ModMinecraft.McInstanceSelected) != (object)null)) ? Settings.Get<string>("VersionServerAuthRegister", ModMinecraft.McInstanceSelected) : Settings.Get<string>("CacheAuthServerRegister", null));
            if (string.IsNullOrEmpty(new ValidateHttp().Check(text)))
            {
                ModBase.OpenWebsite(text.Replace("/auth/register", "/user/closet"));
            }
        }

        internal virtual Grid PanData_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._PanData;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseEventHandler mouseEventHandler = new MouseEventHandler(this.PageLoginAuthSkin_MouseEnter);
                MouseEventHandler mouseEventHandler2 = new MouseEventHandler(this.PageLoginAuthSkin_MouseLeave);
                Grid grid = this._PanData;
                if (grid != null)
                {
                    grid.MouseEnter -= mouseEventHandler;
                    grid.MouseLeave -= mouseEventHandler2;
                }
                this._PanData = value;
                grid = this._PanData;
                if (grid != null)
                {
                    grid.MouseEnter += mouseEventHandler;
                    grid.MouseLeave += mouseEventHandler2;
                }
            }
        }



        internal virtual OurIconButton BtnEdit_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnEdit;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.BtnEdit_Click);
                OurIconButton ourIconButton = this._BtnEdit;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnEdit = value;
                ourIconButton = this._BtnEdit;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurIconButton BtnExit_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnExit;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurIconButton.ClickEventHandler clickEventHandler = ((PageLoginAuthSkin._Closure___._IR27_2 == null) ? (PageLoginAuthSkin._Closure___._IR27_2 = new OurIconButton.ClickEventHandler(PageLoginAuthSkin._Closure___._I._Lambda___R27_2)) : PageLoginAuthSkin._Closure___._IR27_2);
                OurIconButton ourIconButton = this._BtnExit;
                if (ourIconButton != null)
                {
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnExit = value;
                ourIconButton = this._BtnExit;
                if (ourIconButton != null)
                {
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual OurSkin Skin_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._Skin;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurSkin.ClickEventHandler clickEventHandler = new OurSkin.ClickEventHandler(this.Skin_Click);
                OurSkin ourSkin = this._Skin;
                if (ourSkin != null)
                {
                    ourSkin.Click -= clickEventHandler;
                }
                this._Skin = value;
                ourSkin = this._Skin;
                if (ourSkin != null)
                {
                    ourSkin.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, RoutedEventArgs a1)
        {
            this.PageLoginLegacy_Loaded();
        }

        [CompilerGenerated]
        private void _Lambda___6_0()
        {
            try
            {
                ModLaunch.McLoginServer loginData = PageLoginAuthSkin.GetLoginData();
                loginData.ForceReselectProfile = true;
                ModLaunch.McLoginLoader.WaitForExit(loginData, null, true, default(CancellationToken), null);
                ModBase.RunInUi(new Action(this._Lambda___6_1), false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "更换角色失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginAuthSkin.xaml.vb");
            }
        }

        [CompilerGenerated]
        private void _Lambda___6_1()
        {
            this.Reload(true);
        }

        [CompilerGenerated]
        [AccessedThroughProperty("PanData")]
        private Grid _PanData;

        [CompilerGenerated]
        [AccessedThroughProperty("TextName")]
        private TextBlock _TextName;

        [CompilerGenerated]
        [AccessedThroughProperty("TextEmail")]
        private TextBlock _TextEmail;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnEdit")]
        private OurIconButton _BtnEdit;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnExit")]
        private OurIconButton _BtnExit;

        [CompilerGenerated]
        [AccessedThroughProperty("Skin")]
        private OurSkin _Skin;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___R27_2(object a0, EventArgs a1)
            {
                PageLoginAuthSkin.ExitLogin();
            }

            public static readonly PageLoginAuthSkin._Closure___ _I = new PageLoginAuthSkin._Closure___();

            public static OurIconButton.ClickEventHandler _IR27_2;
        }
    }
}
