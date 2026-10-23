using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLoginAuth : Grid
    {
        public PageLoginAuth()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.ReloadRegisterButton();
            };
            this.IsFirstLoad = true;
            this.InitializeComponent();
        }

        public void Reload(bool KeepInput)
        {
            this.CheckRemember.Checked = Settings.Get<bool>("LoginRemember", null);
            if (KeepInput && !this.IsFirstLoad)
            {
                string text = this.ComboName.Text;
                this.ComboName.ItemsSource = ((Operators.CompareString(Settings.Get<string>("LoginAuthEmail", null), "", false) == 0) ? null : Settings.Get<string>("LoginAuthEmail", null).Split("\u00a8", false));
                this.ComboName.Text = text;
            }
            else if (Operators.CompareString(Settings.Get<string>("LoginAuthEmail", null), "", false) == 0)
            {
                this.ComboName.ItemsSource = null;
            }
            else
            {
                this.ComboName.ItemsSource = Settings.Get<string>("LoginAuthEmail", null).Split("\u00a8", false);
                this.ComboName.Text = Settings.Get<string>("LoginAuthEmail", null).BeforeFirst("\u00a8", false);
                if (Settings.Get<bool>("LoginRemember", null))
                {
                    this.TextPass.Password = Settings.Get<string>("LoginAuthPass", null).BeforeFirst("\u00a8", false).Trim();
                }
            }
            this.IsFirstLoad = false;
        }

        public static ModLaunch.McLoginServer GetLoginData()
        {
            string text = (Information.IsNothing(ModMinecraft.McInstanceSelected) ? Settings.Get<string>("CacheAuthServerServer", null) : Settings.Get<string>("VersionServerAuthServer", ModMinecraft.McInstanceSelected)) + "/authserver";
            ModLaunch.McLoginServer mcLoginServer;
            if (ModMain.FrmLoginAuth == null)
            {
                mcLoginServer = new ModLaunch.McLoginServer(ModLaunch.McLoginType.Auth)
                {
                    Token = "Auth",
                    BaseUrl = text,
                    UserName = "",
                    Password = "",
                    Description = "Authlib-Injector",
                    Type = ModLaunch.McLoginType.Auth
                };
            }
            else
            {
                mcLoginServer = new ModLaunch.McLoginServer(ModLaunch.McLoginType.Auth)
                {
                    Token = "Auth",
                    BaseUrl = text,
                    UserName = ModMain.FrmLoginAuth.ComboName.Text.Replace("\u00a8", "").Trim(),
                    Password = ModMain.FrmLoginAuth.TextPass.Password.Replace("\u00a8", "").Trim(),
                    Description = "Authlib-Injector",
                    Type = ModLaunch.McLoginType.Auth
                };
            }
            return mcLoginServer;
        }

        public static string IsVaild(ModLaunch.McLoginServer LoginData)
        {
            string text;
            if (Operators.CompareString(LoginData.UserName, "", false) == 0)
            {
                text = "账号不能为空！";
            }
            else if (Operators.CompareString(LoginData.Password, "", false) == 0)
            {
                text = "密码不能为空！";
            }
            else
            {
                text = "";
            }
            return text;
        }

        public string IsVaild()
        {
            return PageLoginAuth.IsVaild(PageLoginAuth.GetLoginData());
        }

        private void ComboName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Operators.ConditionalCompareObjectEqual(NewLateBinding.LateGet(sender, null, "Text", new object[0], null, null, null), "", false))
            {
                this.TextPass.Password = "";
            }
            if (ModAnimation.AniControlEnabled == 0)
            {
                Settings.Set("CacheAuthAccess", "", null);
            }
        }

        private void TextPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                Settings.Set("CacheAuthAccess", "", null);
            }
        }

        private void ComboName_SelectionChanged(MyComboBox sender, SelectionChangedEventArgs e)
        {
            if (sender.SelectedIndex == -1 || !Settings.Get<bool>("LoginRemember", null))
            {
                this.TextPass.Password = "";
                return;
            }
            this.TextPass.Password = Settings.Get<string>("LoginAuthPass", null).ToString().Split("\u00a8", false)[sender.SelectedIndex].Trim();
        }

        private void CheckBoxChange(MyCheckBox sender, object e)
        {
            if (ModAnimation.AniControlEnabled == 0)
            {
                Settings.Set(Conversions.ToString(sender.Tag), sender.Checked, null);
            }
        }

        private void ComboName_TextChanged()
        {
            this.BtnLink.Content = ((Operators.CompareString(this.ComboName.Text, "", false) == 0) ? "注册账号" : "找回密码");
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (Operators.ConditionalCompareObjectEqual(this.BtnLink.Content, "注册账号", false))
            {
                ModBase.OpenWebsite((ModMinecraft.McInstanceSelected != null) ? Settings.Get<string>("VersionServerAuthRegister", ModMinecraft.McInstanceSelected) : Settings.Get<string>("CacheAuthServerRegister", null));
                return;
            }
            ModBase.OpenWebsite(((ModMinecraft.McInstanceSelected != null) ? Settings.Get<string>("VersionServerAuthRegister", ModMinecraft.McInstanceSelected) : Settings.Get<string>("CacheAuthServerRegister", null)).Replace("/auth/register", "/auth/forgot"));
        }

        private void ReloadRegisterButton()
        {
            string text = ((ModMinecraft.McInstanceSelected != null) ? Settings.Get<string>("VersionServerAuthRegister", ModMinecraft.McInstanceSelected) : Settings.Get<string>("CacheAuthServerRegister", null));
            this.BtnLink.Visibility = (string.IsNullOrEmpty(new ValidateHttp().Validate(text)) ? Visibility.Visible : Visibility.Collapsed);
        }

        internal virtual MyComboBox ComboName
        {
            [CompilerGenerated]
            get
            {
                return this._ComboName;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyComboBox.TextChangedEventHandler textChangedEventHandler = new MyComboBox.TextChangedEventHandler(this.ComboName_TextChanged);
                SelectionChangedEventHandler selectionChangedEventHandler = delegate(object a0, SelectionChangedEventArgs a1)
                {
                    this.ComboName_SelectionChanged((MyComboBox)a0, a1);
                };
                MyComboBox.TextChangedEventHandler textChangedEventHandler2 = delegate(object a0, TextChangedEventArgs a1)
                {
                    this.ComboName_TextChanged();
                };
                MyComboBox myComboBox = this._ComboName;
                if (myComboBox != null)
                {
                    myComboBox.TextChanged -= textChangedEventHandler;
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                    myComboBox.TextChanged -= textChangedEventHandler2;
                }
                this._ComboName = value;
                myComboBox = this._ComboName;
                if (myComboBox != null)
                {
                    myComboBox.TextChanged += textChangedEventHandler;
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                    myComboBox.TextChanged += textChangedEventHandler2;
                }
            }
        }

        internal virtual PasswordBox TextPass
        {
            [CompilerGenerated]
            get
            {
                return this._TextPass;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this.TextPass_PasswordChanged);
                PasswordBox passwordBox = this._TextPass;
                if (passwordBox != null)
                {
                    passwordBox.PasswordChanged -= routedEventHandler;
                }
                this._TextPass = value;
                passwordBox = this._TextPass;
                if (passwordBox != null)
                {
                    passwordBox.PasswordChanged += routedEventHandler;
                }
            }
        }

        internal virtual MyCheckBox CheckRemember
        {
            [CompilerGenerated]
            get
            {
                return this._CheckRemember;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyCheckBox.ChangeEventHandler changeEventHandler = delegate(object a0, bool a1)
                {
                    this.CheckBoxChange((MyCheckBox)a0, a1);
                };
                MyCheckBox myCheckBox = this._CheckRemember;
                if (myCheckBox != null)
                {
                    myCheckBox.Change -= changeEventHandler;
                }
                this._CheckRemember = value;
                myCheckBox = this._CheckRemember;
                if (myCheckBox != null)
                {
                    myCheckBox.Change += changeEventHandler;
                }
            }
        }

        internal virtual MyTextButton BtnLink
        {
            [CompilerGenerated]
            get
            {
                return this._BtnLink;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyTextButton.ClickEventHandler clickEventHandler = new MyTextButton.ClickEventHandler(this.Btn_Click);
                MyTextButton myTextButton = this._BtnLink;
                if (myTextButton != null)
                {
                    myTextButton.Click -= clickEventHandler;
                }
                this._BtnLink = value;
                myTextButton = this._BtnLink;
                if (myTextButton != null)
                {
                    myTextButton.Click += clickEventHandler;
                }
            }
        }

        private bool IsFirstLoad;
    }
}
