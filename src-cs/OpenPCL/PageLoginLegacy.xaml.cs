using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLoginLegacy : Grid
    {
        public PageLoginLegacy()
        {
            base.Loaded += this.PageLoginLegacy_Loaded;
            this.IsReloaded = false;
            this.InitializeComponent();
            this.Skin_WithEvents = this.Skin;
            this.ComboName_WithEvents = this.ComboName;
            this.Skin.Loader = PageLaunchLeft.SkinLegacy;
        }

        private void PageLoginLegacy_Loaded(object sender, RoutedEventArgs e)
        {
            this.Skin.Loader.Start(null, false);
        }

        public void Reload(bool KeepInput)
        {
            if (KeepInput && this.IsReloaded)
            {
                string text = this.ComboName.Text.Trim();
                this.ComboName.ItemsSource = ((Operators.CompareString(Settings.Get<string>("LoginLegacyName", null), "", false) == 0) ? null : Settings.Get<string>("LoginLegacyName", null).Split("\u00a8", false));
                this.ComboName.Text = text;
            }
            else if (Operators.CompareString(Settings.Get<string>("LoginLegacyName", null), "", false) == 0)
            {
                this.ComboName.ItemsSource = null;
            }
            else
            {
                this.ComboName.ItemsSource = Settings.Get<string>("LoginLegacyName", null).ToString().Split("\u00a8", false);
                this.ComboName.Text = Settings.Get<string>("LoginLegacyName", null).ToString().BeforeFirst("\u00a8", false)
                    .Trim();
            }
            this.IsReloaded = true;
        }

        public static ModLaunch.McLoginData GetLoginData()
        {
            string text = ((ModMain.FrmLoginLegacy == null) ? "" : ModMain.FrmLoginLegacy.ComboName.Text.Replace("\u00a8", "").Trim());
            return new ModLaunch.McLoginLegacy
            {
                UserName = text,
                SkinType = Settings.Get<int>("LaunchSkinType", null),
                SkinName = Settings.Get<string>("LaunchSkinID", null)
            };
        }

        public static string IsVaild(ModLaunch.McLoginLegacy LoginData)
        {
            string text;
            if (Operators.CompareString(LoginData.UserName.Trim(), "", false) == 0)
            {
                text = "玩家名不能为空！";
            }
            else if (LoginData.UserName.Contains("\""))
            {
                text = "玩家名不能包含英文引号！";
            }
            else if (((object)(ModMinecraft.McInstanceSelected) != (object)null) && ModMinecraft.McInstanceSelected.Version.Valid && ModMinecraft.McInstanceSelected.Version.Vanilla >= new Version(20, 0, 3) && LoginData.UserName.Trim().Length > 16)
            {
                text = "自 1.20.3 起，玩家名至多只能包含 16 个字符！";
            }
            else
            {
                text = "";
            }
            return text;
        }

        public string IsVaild()
        {
            return PageLoginLegacy.IsVaild((ModLaunch.McLoginLegacy)PageLoginLegacy.GetLoginData());
        }

        private void ComboName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                int caretIndex = ((OurTextBox)this.ComboName.Template.FindName("PART_EditableTextBox", this.ComboName)).CaretIndex;
                if (caretIndex == this.ComboName.Text.Length || caretIndex == 0)
                {
                    e.Handled = true;
                }
            }
        }

        private void ComboLegacy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Settings.Get<int>("LaunchSkinType", null) == 0)
            {
                PageLaunchLeft.SkinLegacy.Start(null, true);
            }
            this.HintChinese.Visibility = (this.ComboName.Text.RegexCheck("^[0-9A-Za-z_]*$", RegexOptions.None) ? Visibility.Collapsed : Visibility.Visible);
            ModBase.RunInUi(new Action(this._Lambda___8_0), true);
        }

        private void Skin_Click()
        {
            if ((Settings.Get<bool>("UiHiddenPageSetup", null) || Settings.Get<bool>("UiHiddenSetupLaunch", null)) && !PageSetupUI.HiddenForceShow)
            {
                ModMain.Hint("启动设置已被禁用！", ModMain.HintType.Red, true);
                return;
            }
            ModMain.FrmMain.PageChange(FormMain.PageType.Setup, FormMain.PageSubType.Default);
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
                OurSkin.ClickEventHandler clickEventHandler = new OurSkin.ClickEventHandler(this._Lambda___R20_1);
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

        internal virtual OurComboBox ComboName_WithEvents
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
                KeyEventHandler keyEventHandler = new KeyEventHandler(this.ComboName_PreviewKeyDown);
                OurComboBox.TextChangedEventHandler textChangedEventHandler = new OurComboBox.TextChangedEventHandler(this.ComboLegacy_TextChanged);
                OurComboBox ourComboBox = this._ComboName;
                if (ourComboBox != null)
                {
                    ourComboBox.PreviewKeyDown -= keyEventHandler;
                    ourComboBox.TextChanged -= textChangedEventHandler;
                }
                this._ComboName = value;
                ourComboBox = this._ComboName;
                if (ourComboBox != null)
                {
                    ourComboBox.PreviewKeyDown += keyEventHandler;
                    ourComboBox.TextChanged += textChangedEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___8_0()
        {
            this.PanMain.InvalidateMeasure();
        }

        [CompilerGenerated]
        private void _Lambda___R20_1(object a0, MouseButtonEventArgs a1)
        {
            this.Skin_Click();
        }

        public bool IsReloaded;

        [CompilerGenerated]
        [AccessedThroughProperty("PanMain")]
        private StackPanel _PanMain;

        [CompilerGenerated]
        [AccessedThroughProperty("HintChinese")]
        private OurHint _HintChinese;

        [CompilerGenerated]
        [AccessedThroughProperty("Skin")]
        private OurSkin _Skin;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboName")]
        private OurComboBox _ComboName;
    }
}
