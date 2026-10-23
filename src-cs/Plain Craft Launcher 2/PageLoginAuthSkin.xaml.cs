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
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.PageLoginLegacy_Loaded();
            };
            this.InitializeComponent();
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
                ModBase.RunInThread(delegate
                {
                    try
                    {
                        ModLaunch.McLoginServer loginData = PageLoginAuthSkin.GetLoginData();
                        loginData.ForceReselectProfile = true;
                        ModLaunch.McLoginLoader.WaitForExit(loginData, null, true, default(CancellationToken), null);
                        ModBase.RunInUi(delegate
                        {
                            this.Reload(true);
                        }, false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "更换角色失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginAuthSkin.xaml.vb");
                    }
                });
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
            string text = ((ModMinecraft.McInstanceSelected != null) ? Settings.Get<string>("VersionServerAuthRegister", ModMinecraft.McInstanceSelected) : Settings.Get<string>("CacheAuthServerRegister", null));
            if (string.IsNullOrEmpty(new ValidateHttp().Validate(text)))
            {
                ModBase.OpenWebsite(text.Replace("/auth/register", "/user/closet"));
            }
        }

        internal virtual Grid PanData
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

        internal virtual TextBlock TextName
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock TextEmail
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnEdit
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
                MyIconButton.ClickEventHandler clickEventHandler = new MyIconButton.ClickEventHandler(this.BtnEdit_Click);
                MyIconButton myIconButton = this._BtnEdit;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnEdit = value;
                myIconButton = this._BtnEdit;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyIconButton BtnExit
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
                MyIconButton.ClickEventHandler clickEventHandler = ((PageLoginAuthSkin._Closure$__.$IR27-2 == null) ? (PageLoginAuthSkin._Closure$__.$IR27-2 = delegate(object a0, EventArgs a1)
                {
                    PageLoginAuthSkin.ExitLogin();
                }) : PageLoginAuthSkin._Closure$__.$IR27-2);
                MyIconButton myIconButton = this._BtnExit;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnExit = value;
                myIconButton = this._BtnExit;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
        }

        internal virtual MySkin Skin
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
                MySkin.ClickEventHandler clickEventHandler = new MySkin.ClickEventHandler(this.Skin_Click);
                MySkin mySkin = this._Skin;
                if (mySkin != null)
                {
                    mySkin.Click -= clickEventHandler;
                }
                this._Skin = value;
                mySkin = this._Skin;
                if (mySkin != null)
                {
                    mySkin.Click += clickEventHandler;
                }
            }
        }
    }
}
