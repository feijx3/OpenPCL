using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLoginMs : StackPanel
    {
        public PageLoginMs()
        {
            this.InitializeComponent();
            this.ComboAccounts_WithEvents = this.ComboAccounts;
            this.BtnLogin_WithEvents = this.BtnLogin;
        }

        public void Reload(bool KeepInput)
        {
            int selectedIndex = this.ComboAccounts.SelectedIndex;
            this.ComboAccounts.Items.Clear();
            this.ComboAccounts.Items.Add(new OurComboBoxItem
            {
                Content = "添加新账号"
            });
            try
            {
                JObject jobject = (JObject)Settings.Get<string>("LoginMsJson", null).DeserializeJson();
                try
                {
                    foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
                    {
                        OurListItem ourListItem = (OurListItem)((DataTemplate)base.FindResource("ComboBoxItemTemplateWithDelete")).LoadContent();
                        ourListItem.Tag = keyValuePair.Value.ToString();
                        ourListItem.Title = keyValuePair.Key;
                        ourListItem.Buttons.ElementAtOrDefault<OurIconButton>(0).Tag = keyValuePair.Key;
                        this.ComboAccounts.Items.Add(ourListItem);
                    }
                }
                finally
                {
                    IEnumerator<KeyValuePair<string, JToken>> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "微软登录信息出错，登录信息已被重置", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMs.xaml.vb");
                Settings.Set("LoginMsJson", "{}", null);
            }
            this.ComboAccounts.SelectedIndex = (KeepInput ? Math.Max(0, selectedIndex) : 0);
        }

        private void ComboAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModAnimation.AniControlEnabled == 0 && this.ComboAccounts.SelectedItem != null && this.ComboAccounts.ContentPresenter != null)
            {
                if (this.ComboAccounts.SelectedItem is OurListItem)
                {
                    this.ComboAccounts.ContentPresenter.Content = ((OurListItem)this.ComboAccounts.SelectedItem).Title;
                    return;
                }
                if (this.ComboAccounts.SelectedItem is OurComboBoxItem)
                {
                    this.ComboAccounts.ContentPresenter.Content = RuntimeHelpers.GetObjectValue(((OurComboBoxItem)this.ComboAccounts.SelectedItem).Content);
                }
            }
        }

        public static ModLaunch.McLoginMs GetLoginData()
        {
            PageLoginMs._Closure___3_0 CS_locals1 = new PageLoginMs._Closure___3_0();
            ModLaunch.McLoginMs mcLoginMs;
            if (ModMain.FrmLoginMs == null)
            {
                mcLoginMs = new ModLaunch.McLoginMs
                {
                    OAuthRefreshToken = Settings.Get<string>("CacheMsV2OAuthRefresh", null),
                    UserName = Settings.Get<string>("CacheMsV2Name", null)
                };
            }
            else
            {
                CS_locals1._VB_Local_Result = null;
                ModBase.RunInUiWait(new Action(CS_locals1._Lambda___0));
                mcLoginMs = CS_locals1._VB_Local_Result;
            }
            return mcLoginMs;
        }

        public static string IsVaild(ModLaunch.McLoginMs LoginData)
        {
            string text;
            if (Operators.CompareString(LoginData.OAuthRefreshToken, "", false) == 0)
            {
                text = "请在登录账号后再启动游戏！";
            }
            else
            {
                text = "";
            }
            return text;
        }

        public string IsVaild()
        {
            return PageLoginMs.IsVaild(PageLoginMs.GetLoginData());
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            this.ComboAccounts.IsEnabled = false;
            this.BtnLogin.IsEnabled = false;
            this.BtnLogin.Text = "0%";
            ModBase.RunInNewThread(new Action(this._Lambda___6_0), "Ms Login", ThreadPriority.Normal);
        }


        internal virtual OurComboBox ComboAccounts_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ComboAccounts;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SelectionChangedEventHandler selectionChangedEventHandler = new SelectionChangedEventHandler(this.ComboAccounts_SelectionChanged);
                OurComboBox ourComboBox = this._ComboAccounts;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboAccounts = value;
                ourComboBox = this._ComboAccounts;
                if (ourComboBox != null)
                {
                    ourComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual OurButton BtnLogin_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnLogin;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                OurButton.ClickEventHandler clickEventHandler = new OurButton.ClickEventHandler(this.BtnLogin_Click);
                OurButton ourButton = this._BtnLogin;
                if (ourButton != null)
                {
                    ourButton.Click -= clickEventHandler;
                }
                this._BtnLogin = value;
                ourButton = this._BtnLogin;
                if (ourButton != null)
                {
                    ourButton.Click += clickEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___6_0()
        {
            try
            {
                ModLaunch.McLoginMsLoader.Start(PageLoginMs.GetLoginData(), true);
                while (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Loading)
                {
                    ModBase.RunInUi(new Action(this._Lambda___6_1), false);
                    Thread.Sleep(50);
                }
                if (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Finished)
                {
                    ModBase.RunInUi((PageLoginMs._Closure___._I6_2 == null) ? (PageLoginMs._Closure___._I6_2 = new Action(PageLoginMs._Closure___._I._Lambda___6_2)) : PageLoginMs._Closure___._I6_2, false);
                }
                else
                {
                    if (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Canceled)
                    {
                        throw new OperationCanceledException();
                    }
                    if (ModLaunch.McLoginMsLoader.Error == null)
                    {
                        throw new Exception("未知错误！");
                    }
                    throw new Exception(ModLaunch.McLoginMsLoader.Error.Message, ModLaunch.McLoginMsLoader.Error);
                }
            }
            catch (Exception ex)
            {
                if (!ex.IsCanceled())
                {
                    if (ex.Message.StartsWithF("$", false))
                    {
                        ModMain.Hint(ex.Message.TrimStart(new char[] { '$' }), ModMain.HintType.Red, true);
                    }
                    else if (ex is AuthenticationException && ex.Message.Contains("SSL/TLS"))
                    {
                        Logger.Error(ex, string.Format("正版登录验证失败，请考虑在 [设置 → 其他] 中关闭 [在正版登录时验证 SSL 证书]，然后再试。{0}{1}原始错误信息：", "\r\n", "\r\n"), LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMs.xaml.vb");
                    }
                    else
                    {
                        Logger.Error(ex, "正版登录尝试失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMs.xaml.vb");
                    }
                }
            }
            finally
            {
                ModBase.RunInUi(new Action(this._Lambda___6_3), false);
            }
        }

        [CompilerGenerated]
        private void _Lambda___6_1()
        {
            this.BtnLogin.Text = Conversions.ToString(Math.Round(ModLaunch.McLoginMsLoader.Progress * 100.0)) + "%";
        }

        [CompilerGenerated]
        private void _Lambda___6_3()
        {
            this.ComboAccounts.IsEnabled = true;
            this.BtnLogin.IsEnabled = true;
            this.BtnLogin.Text = "登录";
        }

        [CompilerGenerated]
        [AccessedThroughProperty("PanEmpty")]
        private Grid _PanEmpty;

        [CompilerGenerated]
        [AccessedThroughProperty("ComboAccounts")]
        private OurComboBox _ComboAccounts;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnLogin")]
        private OurButton _BtnLogin;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___6_2()
            {
                ModMain.FrmLaunchLeft.RefreshPage(false, true);
            }

            public static readonly PageLoginMs._Closure___ _I = new PageLoginMs._Closure___();

            public static Action _I6_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___3_0
        {
            internal void _Lambda___0()
            {
                if (ModMain.FrmLoginMs.ComboAccounts.SelectedIndex == 0)
                {
                    this._VB_Local_Result = new ModLaunch.McLoginMs();
                    return;
                }
                OurListItem ourListItem = (OurListItem)ModMain.FrmLoginMs.ComboAccounts.SelectedItem;
                this._VB_Local_Result = new ModLaunch.McLoginMs
                {
                    OAuthRefreshToken = Conversions.ToString(ourListItem.Tag),
                    UserName = ourListItem.Title
                };
            }

            public ModLaunch.McLoginMs _VB_Local_Result;
        }
    }
}
