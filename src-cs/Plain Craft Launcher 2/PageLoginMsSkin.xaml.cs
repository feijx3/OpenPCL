using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageLoginMsSkin : Grid
    {
        public PageLoginMsSkin()
        {
            base.Loaded += this.PageLoginLegacy_Loaded;
            this.IsChanging = false;
            this.InitializeComponent();
            this.Skin.Loader = PageLaunchLeft.SkinMs;
        }

        private void PageLoginLegacy_Loaded(object sender, RoutedEventArgs e)
        {
            this.Skin.Loader.Start(null, false);
        }

        public void Reload(bool KeepInput)
        {
            this.TextName.Text = Settings.Get<string>("CacheMsV2Name", null);
        }

        public static ModLaunch.McLoginMs GetLoginData()
        {
            ModLaunch.McLoginMs mcLoginMs;
            if (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Finished)
            {
                mcLoginMs = new ModLaunch.McLoginMs
                {
                    OAuthRefreshToken = Settings.Get<string>("CacheMsV2OAuthRefresh", null),
                    UserName = Settings.Get<string>("CacheMsV2Name", null),
                    AccessToken = Settings.Get<string>("CacheMsV2Access", null),
                    Uuid = Settings.Get<string>("CacheMsV2Uuid", null),
                    ProfileJson = Settings.Get<string>("CacheMsV2ProfileJson", null)
                };
            }
            else
            {
                mcLoginMs = new ModLaunch.McLoginMs
                {
                    OAuthRefreshToken = Settings.Get<string>("CacheMsV2OAuthRefresh", null),
                    UserName = Settings.Get<string>("CacheMsV2Name", null)
                };
            }
            return mcLoginMs;
        }

        private void ShowPanel(object sender, MouseEventArgs e)
        {
            ModAnimation.AniStart(ModAnimation.AaOpacity(this.PanButtons, 1.0 - this.PanButtons.Opacity, 120, 0, null, false), "PageLoginMsSkin Button", false);
        }

        public void HidePanel(object sender, EventArgs e)
        {
            if (!this.BtnEdit.ContextMenu.IsOpen && !this.BtnSkin.ContextMenu.IsOpen && !this.PanData.IsMouseOver)
            {
                ModAnimation.AniStart(ModAnimation.AaOpacity(this.PanButtons, -this.PanButtons.Opacity, 120, 0, null, false), "PageLoginMsSkin Button", false);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            this.BtnEdit.ContextMenu.IsOpen = true;
        }

        private void BtnExit_Click()
        {
            Settings.Set("CacheMsV2OAuthRefresh", "", null);
            Settings.Set("CacheMsV2Access", "", null);
            Settings.Set("CacheMsV2ProfileJson", "", null);
            Settings.Set("CacheMsV2Uuid", "", null);
            Settings.Set("CacheMsV2Name", "", null);
            Settings.Set("CacheMsV2Expires", 0L, null);
            ModLaunch.McLoginMsLoader.Cancel();
            ModMain.FrmLaunchLeft.RefreshPage(false, true);
        }

        private void BtnSkin_Click(object sender, RoutedEventArgs e)
        {
            this.BtnSkin.ContextMenu.IsOpen = true;
        }

        public void BtnSkinEdit_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsChanging)
            {
                ModMain.Hint("正在更改皮肤中，请稍候！", ModMain.HintType.Blue, true);
                return;
            }
            if (ModLaunch.McLoginLoader.State == ModBase.LoadState.Failed)
            {
                ModMain.Hint("登录失败，无法更改皮肤！", ModMain.HintType.Red, true);
                return;
            }
            ModMinecraft.McSkinInfo SkinInfo = ModMinecraft.McSkinSelect();
            if (SkinInfo.IsVaild)
            {
                ModMain.Hint("正在更改皮肤……", ModMain.HintType.Blue, true);
                this.IsChanging = true;
                ModBase.RunInNewThread(delegate
                {
                    this.EditSkin(SkinInfo);
                }, "Ms Skin Upload", ThreadPriority.Normal);
            }
        }

        private void EditSkin(ModMinecraft.McSkinInfo SkinInfo)
        {
            for (;;)
            {
                try
                {
                    while (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Loading)
                    {
                        Thread.Sleep(10);
                    }
                    if (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Failed)
                    {
                        throw new Exception("登录失败", ModLaunch.McLoginMsLoader.Error);
                    }
                    string text = Settings.Get<string>("CacheMsV2Access", null);
                    Settings.Get<string>("CacheMsV2Uuid", null);
                    string text2 = "https://api.minecraftservices.com/minecraft/profile/skins";
                    HttpMethod post = HttpMethod.Post;
                    string text3 = null;
                    MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                    multipartFormDataContent.Add(new StringContent(SkinInfo.IsSlim ? "slim" : "classic"), "variant");
                    multipartFormDataContent.Add(new ByteArrayContent(FileUtils.ReadAsBytes(SkinInfo.LocalFile, null)), "file", PathUtils.GetLastPart(SkinInfo.LocalFile));
                    string text4 = null;
                    string text5 = "*/*";
                    string[,] array = new string[3, 2];
                    array[0, 0] = "Authorization";
                    array[0, 1] = "Bearer " + text;
                    array[1, 0] = "Accept";
                    array[1, 1] = "*/*";
                    array[2, 0] = "User-Agent";
                    array[2, 1] = "MojangSharp/0.1";
                    string text6 = ModNet.NetRequestByClientRetry(text2, post, text3, multipartFormDataContent, text4, text5, array, false, null, false);
                    if (text6.Contains("request requires user authentication"))
                    {
                        ModMain.Hint("正在重新登录，将在登录后自动更改皮肤……", ModMain.HintType.Blue, true);
                        ModLaunch.McLoginMsLoader.Start(PageLoginMsSkin.GetLoginData(), true);
                        continue;
                    }
                    Logger.Info(string.Format("皮肤修改返回值：{0}{1}", "\r\n", text6), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMsSkin.xaml.vb");
                    JObject jobject = (JObject)text6.DeserializeJson();
                    if (jobject.ContainsKey("errorMessage"))
                    {
                        throw new Exception(jobject["errorMessage"].ToString());
                    }
                    try
                    {
                        foreach (JToken jtoken in jobject["skins"])
                        {
                            JObject jobject2 = (JObject)jtoken;
                            if (Operators.CompareString(jobject2["state"].ToString(), "ACTIVE", false) == 0)
                            {
                                MySkin.ReloadCache((string)jobject2["url"]);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<JToken> enumerator;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    throw new Exception("未知错误（" + text6 + "）");
                }
                catch (Exception ex)
                {
                    if (ex is HttpRequestCodeException)
                    {
                        HttpRequestCodeException ex2 = (HttpRequestCodeException)ex;
                        HttpStatusCode statusCode = ex2.StatusCode;
                        if (statusCode != HttpStatusCode.BadRequest)
                        {
                            if (statusCode == HttpStatusCode.Unauthorized)
                            {
                                Logger.Warn(ex, "更改皮肤时遭遇 401 错误", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMsSkin.xaml.vb");
                                ModMain.Hint("正在重新登录，将在登录后自动更改皮肤……", ModMain.HintType.Blue, true);
                                ModLaunch.McLoginMsLoader.Start(PageLoginMsSkin.GetLoginData(), true);
                                continue;
                            }
                        }
                        else
                        {
                            Logger.Warn(ex, "更改皮肤时遭遇 400 错误", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMsSkin.xaml.vb");
                            string response = ex2.Response;
                            if (response != null && response.Contains("\"error\""))
                            {
                                ModMain.Hint("更改皮肤失败：" + ex2.Response.DeserializeJson()["error"].ToString(), ModMain.HintType.Red, true);
                            }
                            else
                            {
                                string response2 = ex2.Response;
                                if (response2 != null && response2.Contains("\"errorMessage\""))
                                {
                                    ModMain.Hint("更改皮肤失败：" + ex2.Response.DeserializeJson()["errorMessage"].ToString(), ModMain.HintType.Red, true);
                                }
                            }
                        }
                    }
                    else if (ex.IsBadNetwork())
                    {
                        ModMain.Hint("更改皮肤失败：连接 Mojang 服务器超时，请稍后再试，或使用 VPN 改善网络环境", ModMain.HintType.Red, true);
                    }
                    else
                    {
                        Logger.Error(ex, "更改皮肤失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMsSkin.xaml.vb");
                    }
                }
                finally
                {
                    this.IsChanging = false;
                }
                break;
            }
        }

        public void BtnSkinSave_Click(object sender, RoutedEventArgs e)
        {
            this.Skin.BtnSkinSave_Click();
        }

        public void BtnSkinRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.Skin.RefreshClick();
        }

        public void BtnSkinCape_Click(object sender, RoutedEventArgs e)
        {
            this.Skin.BtnSkinCape_Click();
        }

        public void BtnSkinCapeRefresh_Click(object sender, RoutedEventArgs e)
        {
            ModBase.RunInThread((PageLoginMsSkin._Closure$__.$I15-0 == null) ? (PageLoginMsSkin._Closure$__.$I15-0 = delegate
            {
                try
                {
                    ModMain.Hint("正在刷新披风列表……", ModMain.HintType.Blue, true);
                    if (ModLaunch.McLaunchLoader.State == ModBase.LoadState.Loading)
                    {
                        ModLaunch.McLoginMsLoader.WaitForExit(null, null, false, default(CancellationToken), null);
                    }
                    else
                    {
                        ModLaunch.McLoginMsLoader.WaitForExit(PageLoginMsSkin.GetLoginData(), null, true, default(CancellationToken), null);
                    }
                    ModMain.Hint("已刷新披风列表！", ModMain.HintType.Green, true);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "刷新披风列表失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\PageLoginMsSkin.xaml.vb");
                }
            }) : PageLoginMsSkin._Closure$__.$I15-0);
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
                MouseEventHandler mouseEventHandler = new MouseEventHandler(this.ShowPanel);
                MouseEventHandler mouseEventHandler2 = new MouseEventHandler(this.HidePanel);
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

        internal virtual Border PanButtons
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyIconButton BtnSkin
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSkin;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnSkin_Click(RuntimeHelpers.GetObjectValue(a0), (RoutedEventArgs)a1);
                };
                MyIconButton myIconButton = this._BtnSkin;
                if (myIconButton != null)
                {
                    myIconButton.Click -= clickEventHandler;
                }
                this._BtnSkin = value;
                myIconButton = this._BtnSkin;
                if (myIconButton != null)
                {
                    myIconButton.Click += clickEventHandler;
                }
            }
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
                MyIconButton.ClickEventHandler clickEventHandler = delegate(object a0, EventArgs a1)
                {
                    this.BtnExit_Click();
                };
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
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private bool IsChanging;
    }
}
