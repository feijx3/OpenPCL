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
        }

        public void Reload(bool KeepInput)
        {
            int selectedIndex = this.ComboAccounts.SelectedIndex;
            this.ComboAccounts.Items.Clear();
            this.ComboAccounts.Items.Add(new MyComboBoxItem
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
                        MyListItem myListItem = (MyListItem)((DataTemplate)base.FindResource("ComboBoxItemTemplateWithDelete")).LoadContent();
                        myListItem.Tag = keyValuePair.Value.ToString();
                        myListItem.Title = keyValuePair.Key;
                        myListItem.Buttons.ElementAtOrDefault<MyIconButton>(0).Tag = keyValuePair.Key;
                        this.ComboAccounts.Items.Add(myListItem);
                    }
                }
                finally
                {
                    IEnumerator<KeyValuePair<string, JToken>> enumerator;
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
                if (this.ComboAccounts.SelectedItem is MyListItem)
                {
                    this.ComboAccounts.ContentPresenter.Content = ((MyListItem)this.ComboAccounts.SelectedItem).Title;
                    return;
                }
                if (this.ComboAccounts.SelectedItem is MyComboBoxItem)
                {
                    this.ComboAccounts.ContentPresenter.Content = RuntimeHelpers.GetObjectValue(((MyComboBoxItem)this.ComboAccounts.SelectedItem).Content);
                }
            }
        }

        public static ModLaunch.McLoginMs GetLoginData()
        {
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
                ModLaunch.McLoginMs Result = null;
                ModBase.RunInUiWait(delegate
                {
                    if (ModMain.FrmLoginMs.ComboAccounts.SelectedIndex == 0)
                    {
                        Result = new ModLaunch.McLoginMs();
                        return;
                    }
                    MyListItem myListItem = (MyListItem)ModMain.FrmLoginMs.ComboAccounts.SelectedItem;
                    Result = new ModLaunch.McLoginMs
                    {
                        OAuthRefreshToken = Conversions.ToString(myListItem.Tag),
                        UserName = myListItem.Title
                    };
                });
                mcLoginMs = Result;
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
            ModBase.RunInNewThread(delegate
            {
                try
                {
                    ModLaunch.McLoginMsLoader.Start(PageLoginMs.GetLoginData(), true);
                    while (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Loading)
                    {
                        ModBase.RunInUi(delegate
                        {
                            this.BtnLogin.Text = Conversions.ToString(Math.Round(ModLaunch.McLoginMsLoader.Progress * 100.0)) + "%";
                        }, false);
                        Thread.Sleep(50);
                    }
                    if (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Finished)
                    {
                        ModBase.RunInUi((PageLoginMs._Closure$__.$I6-2 == null) ? (PageLoginMs._Closure$__.$I6-2 = delegate
                        {
                            ModMain.FrmLaunchLeft.RefreshPage(false, true);
                        }) : PageLoginMs._Closure$__.$I6-2, false);
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
                    ModBase.RunInUi(delegate
                    {
                        this.ComboAccounts.IsEnabled = true;
                        this.BtnLogin.IsEnabled = true;
                        this.BtnLogin.Text = "登录";
                    }, false);
                }
            }, "Ms Login", ThreadPriority.Normal);
        }

        internal virtual Grid PanEmpty
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyComboBox ComboAccounts
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
                MyComboBox myComboBox = this._ComboAccounts;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged -= selectionChangedEventHandler;
                }
                this._ComboAccounts = value;
                myComboBox = this._ComboAccounts;
                if (myComboBox != null)
                {
                    myComboBox.SelectionChanged += selectionChangedEventHandler;
                }
            }
        }

        internal virtual MyButton BtnLogin
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
                MyButton.ClickEventHandler clickEventHandler = new MyButton.ClickEventHandler(this.BtnLogin_Click);
                MyButton myButton = this._BtnLogin;
                if (myButton != null)
                {
                    myButton.Click -= clickEventHandler;
                }
                this._BtnLogin = value;
                myButton = this._BtnLogin;
                if (myButton != null)
                {
                    myButton.Click += clickEventHandler;
                }
            }
        }
    }
}
