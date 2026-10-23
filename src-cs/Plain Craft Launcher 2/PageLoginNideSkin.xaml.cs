using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLoginNideSkin : Grid
    {
        public PageLoginNideSkin()
        {
            base.Loaded += this.PageLoginLegacy_Loaded;
            this.InitializeComponent();
            this.Skin.Loader = PageLaunchLeft.SkinNide;
        }

        private void PageLoginLegacy_Loaded(object sender, RoutedEventArgs e)
        {
            this.Skin.Loader.Start(null, false);
        }

        public void Reload(bool KeepInput)
        {
            this.TextName.Text = Settings.Get<string>("CacheNideName", null);
            this.TextEmail.Text = Settings.Get<string>("CacheNideUsername", null);
            this.TextEmail.Visibility = (!Settings.Get<bool>("UiLauncherEmail", null)).ToVisibility();
        }

        public static ModLaunch.McLoginServer GetLoginData()
        {
            string text = (Information.IsNothing(ModMinecraft.McInstanceSelected) ? Settings.Get<string>("CacheNideServer", null) : Settings.Get<string>("VersionServerNide", ModMinecraft.McInstanceSelected));
            return new ModLaunch.McLoginServer(ModLaunch.McLoginType.Nide)
            {
                Token = "Nide",
                UserName = Settings.Get<string>("CacheNideUsername", null),
                Password = Settings.Get<string>("CacheNidePass", null),
                Description = "统一通行证",
                Type = ModLaunch.McLoginType.Nide,
                BaseUrl = "https://auth.mc-user.com:233/" + text + "/authserver"
            };
        }

        private void PageLoginNideSkin_MouseEnter(object sender, MouseEventArgs e)
        {
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.BtnEdit, 1.0 - this.BtnEdit.Opacity, 80, 0, null, false),
                ModAnimation.AaHeight(this.BtnEdit, 25.5 - this.BtnEdit.Height, 140, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnEdit, -1.5, 50, 140, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaOpacity(this.BtnExit, 1.0 - this.BtnExit.Opacity, 80, 0, null, false),
                ModAnimation.AaHeight(this.BtnExit, 25.5 - this.BtnExit.Height, 140, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnExit, -1.5, 50, 140, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false)
            }, "PageLoginNideSkin Button", false);
        }

        private void PageLoginNideSkin_MouseLeave(object sender, MouseEventArgs e)
        {
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaOpacity(this.BtnEdit, -this.BtnEdit.Opacity, 120, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnEdit, 14.0 - this.BtnEdit.Height, 120, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaOpacity(this.BtnExit, -this.BtnExit.Opacity, 120, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                ModAnimation.AaHeight(this.BtnExit, 14.0 - this.BtnExit.Height, 120, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Middle), false)
            }, "PageLoginNideSkin Button", false);
        }

        public static void ExitLogin()
        {
            Settings.Set("CacheNideAccess", "", null);
            ModLaunch.McLoginNideLoader.Input = null;
            ModMain.FrmLaunchLeft.RefreshPage(false, true);
        }

        private void Skin_Click(object sender, MouseButtonEventArgs e)
        {
            ModBase.OpenWebsite("https://login.mc-user.com:233/" + (Information.IsNothing(ModMinecraft.McInstanceSelected) ? Settings.Get<string>("CacheNideServer", null) : Settings.Get<string>("VersionServerNide", ModMinecraft.McInstanceSelected)) + "/skin");
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
                MouseEventHandler mouseEventHandler = new MouseEventHandler(this.PageLoginNideSkin_MouseEnter);
                MouseEventHandler mouseEventHandler2 = new MouseEventHandler(this.PageLoginNideSkin_MouseLeave);
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
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
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
                MyIconButton.ClickEventHandler clickEventHandler = ((PageLoginNideSkin._Closure$__.$IR26-1 == null) ? (PageLoginNideSkin._Closure$__.$IR26-1 = delegate(object a0, EventArgs a1)
                {
                    PageLoginNideSkin.ExitLogin();
                }) : PageLoginNideSkin._Closure$__.$IR26-1);
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
