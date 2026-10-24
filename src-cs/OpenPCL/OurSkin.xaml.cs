using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MeloongCore;
using MeloongCore.Extensions;
using MeloongCore.Wpf;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PCL
{
    [DesignerGenerated]
    public partial class OurSkin : Grid
    {
        public OurSkin()
        {
            base.MouseEnter += this.PanSkin_MouseEnter;
            base.MouseLeave += this.PanSkin_MouseLeave;
            base.MouseLeftButtonDown += this.PanSkin_MouseLeftButtonDown;
            base.MouseLeftButtonUp += this.PanSkin_MouseLeftButtonUp;
            this.IsSkinMouseDown = false;
            this.IsChanging = false;
            this.InitializeComponent();
            this.BtnSkinSave_WithEvents = this.BtnSkinSave;
            this.BtnSkinRefresh_WithEvents = this.BtnSkinRefresh;
            this.BtnSkinCape_WithEvents = this.BtnSkinCape;
        }

        public event OurSkin.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurSkin.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurSkin.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurSkin.ClickEventHandler clickEventHandler3 = (OurSkin.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurSkin.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurSkin.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurSkin.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurSkin.ClickEventHandler clickEventHandler3 = (OurSkin.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurSkin.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        public string Address
        {
            get
            {
                return this._Address;
            }
            set
            {
                this._Address = value;
                base.ToolTip = ((Operators.CompareString(this._Address, "", false) == 0) ? "加载中" : "点击更换皮肤（右键查看更多选项）");
            }
        }

        private void PanSkin_MouseEnter(object sender, MouseEventArgs e)
        {
            ModAnimation.AniStart(ModAnimation.AaOpacity(this.ShadowSkin, 0.8 - this.ShadowSkin.Opacity, 200, 100, null, false), "Skin Shadow", false);
        }

        private void PanSkin_MouseLeave(object sender, MouseEventArgs e)
        {
            ModAnimation.AniStart(ModAnimation.AaOpacity(this.ShadowSkin, 0.2 - this.ShadowSkin.Opacity, 200, 0, null, false), "Skin Shadow", false);
            this.IsSkinMouseDown = false;
            ModAnimation.AniStart(ModAnimation.AaScaleTransform(this, 1.0 - ((ScaleTransform)base.RenderTransform).ScaleX, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false), "Skin Scale", false);
        }

        private void PanSkin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsSkinMouseDown = true;
            ModAnimation.AniStart(ModAnimation.AaScaleTransform(this, 0.9 - ((ScaleTransform)base.RenderTransform).ScaleX, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false), "Skin Scale", false);
        }

        private void PanSkin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ModAnimation.AniStart(ModAnimation.AaScaleTransform(this, 1.0 - ((ScaleTransform)base.RenderTransform).ScaleX, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false), "Skin Scale", false);
            if (this.IsSkinMouseDown)
            {
                this.IsSkinMouseDown = false;
                OurSkin.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
            }
        }

        public void BtnSkinSave_Click()
        {
            OurSkin.Save(this.Loader);
        }

        public static void Save(ModLoader.LoaderTask<ValueTuple<string, string>, string> Loader)
        {
            string output = Loader.Output;
            if (Loader.State != ModBase.LoadState.Finished)
            {
                ModMain.Hint("皮肤正在获取中，请稍候！", ModMain.HintType.Red, true);
                if (Loader.State != ModBase.LoadState.Loading)
                {
                    Loader.Start(null, false);
                    return;
                }
            }
            else
            {
                try
                {
                    string text = Dialogs.SaveFile("选取保存皮肤的位置", PathUtils.GetLastPart(output), null, new ValueTuple<string, string>[]
                    {
                        new ValueTuple<string, string>("png", "皮肤图片文件")
                    });
                    if (text != null)
                    {
                        if (text.Contains("\\"))
                        {
                            FileUtils.Delete(text, false);
                            if (output.StartsWithF(ModBase.PathImage, false))
                            {
                                new OurBitmap(output).Save(text);
                            }
                            else
                            {
                                FileUtils.Copy(output, text);
                            }
                            ModMain.Hint("皮肤保存成功！", ModMain.HintType.Green, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "保存皮肤失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                }
            }
        }

        private void BtnSkinSave_Checked(OurMenuItem sender, RoutedEventArgs e)
        {
            sender.IsEnabled = Operators.CompareString(this.Address, "", false) == 0;
        }

        public void Load()
        {
            checked
            {
                try
                {
                    this.Address = this.Loader.Output;
                    if (string.IsNullOrEmpty(this.Address))
                    {
                        throw new Exception("皮肤加载器 " + this.Loader.Name + " 没有输出");
                    }
                    if (!this.Address.StartsWithF(ModBase.PathImage, false) && !FileUtils.Exists(this.Address))
                    {
                        throw new FileNotFoundException("皮肤文件未找到", this.Address);
                    }
                    OurBitmap ourBitmap;
                    try
                    {
                        ourBitmap = new OurBitmap(this.Address);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("皮肤文件已损坏：{0}", this.Address), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                        FileUtils.Delete(this.Address, false);
                        return;
                    }
                    this.ImgBack.Tag = this.Address;
                    int num = (int)Math.Round((double)ourBitmap.Pic.Width / 64.0);
                    if (ourBitmap.Pic.Width < 32 || ourBitmap.Pic.Height < 32)
                    {
                        this.ImgFore.Source = null;
                        this.ImgBack.Source = null;
                        throw new Exception("图片大小不足，长为 " + Conversions.ToString(ourBitmap.Pic.Height) + "，宽为 " + Conversions.ToString(ourBitmap.Pic.Width));
                    }
                    if (ourBitmap.Pic.Width >= 64 && ourBitmap.Pic.Height >= 32)
                    {
                        if (ourBitmap.Pic.GetPixel(1, 1).A == 0 || ourBitmap.Pic.GetPixel(ourBitmap.Pic.Width - 1, ourBitmap.Pic.Height - 1).A == 0 || ourBitmap.Pic.GetPixel(ourBitmap.Pic.Width - 2, (int)Math.Round(unchecked((double)ourBitmap.Pic.Height / 2.0 - 2.0))).A == 0 || (ourBitmap.Pic.GetPixel(1, 1) != ourBitmap.Pic.GetPixel(num * 41, num * 9) && ourBitmap.Pic.GetPixel(ourBitmap.Pic.Width - 1, ourBitmap.Pic.Height - 1) != ourBitmap.Pic.GetPixel(num * 41, num * 9) && ourBitmap.Pic.GetPixel(ourBitmap.Pic.Width - 2, (int)Math.Round(unchecked((double)ourBitmap.Pic.Height / 2.0 - 2.0))) != ourBitmap.Pic.GetPixel(num * 41, num * 9)))
                        {
                            this.ImgFore.Source = ourBitmap.Clip(num * 40, num * 8, num * 8, num * 8);
                        }
                        else
                        {
                            this.ImgFore.Source = null;
                        }
                    }
                    else
                    {
                        this.ImgFore.Source = null;
                    }
                    this.ImgBack.Source = ourBitmap.Clip(num * 8, num * 8, num * 8, num * 8);
                    Logger.Info(string.Format("载入头像成功：{0}", this.Loader.Name), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                }
                catch (Exception ex2)
                {
                    Logger.Error(ex2, string.Format("载入头像失败（{0},{1}）", this.Address ?? "null", this.Loader.Name), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                }
            }
        }

        public void Clear()
        {
            this.Address = "";
            this.ImgFore.Source = null;
            this.ImgBack.Source = null;
        }

        public void RefreshClick()
        {
            OurSkin.RefreshCache(this.Loader);
        }

        public static void RefreshCache(ModLoader.LoaderTask<ValueTuple<string, string>, string> sender = null)
        {
            OurSkin._Closure___22_0 CS_locals1 = new OurSkin._Closure___22_0(null);
            CS_locals1._VB_Local_sender = sender;
            bool flag = false;
            try
            {
                List<ModLoader.LoaderTask<ValueTuple<string, string>, string>>.Enumerator enumerator = PageLaunchLeft.SkinLoaders.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.State == ModBase.LoadState.Loading)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            finally
            {
                List<ModLoader.LoaderTask<ValueTuple<string, string>, string>>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            if (ModMain.FrmLaunchLeft != null && flag)
            {
                ModMain.Hint("有正在获取中的皮肤，请稍后再试！", ModMain.HintType.Blue, true);
                return;
            }
            ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
        }

        public static void ReloadCache(string SkinAddress)
        {
            OurSkin._Closure___23_0 CS_locals1 = new OurSkin._Closure___23_0(null);
            CS_locals1._VB_Local_SkinAddress = SkinAddress;
            ModBase.RunInThread(new Action(CS_locals1._Lambda___0));
        }

        public bool HasCape
        {
            get
            {
                return this.BtnSkinCape.Visibility == Visibility.Collapsed;
            }
            set
            {
                if (value)
                {
                    this.BtnSkinCape.Visibility = Visibility.Visible;
                    return;
                }
                this.BtnSkinCape.Visibility = Visibility.Collapsed;
            }
        }

        public void BtnSkinCape_Click()
        {
            if (this.IsChanging)
            {
                ModMain.Hint("正在更改披风中，请稍候！", ModMain.HintType.Blue, true);
                return;
            }
            if (ModLaunch.McLoginMsLoader.State == ModBase.LoadState.Failed)
            {
                ModMain.Hint("登录失败，无法更改披风！", ModMain.HintType.Red, true);
                return;
            }
            if (ModLaunch.McLoginMsLoader.State != ModBase.LoadState.Finished)
            {
                ModMain.Hint("正在获取披风列表，请稍候……", ModMain.HintType.Blue, true);
            }
            this.IsChanging = true;
            ModBase.RunInNewThread(new Action(this._Lambda___28_0), "Cape Change", ThreadPriority.Normal);
        }


        internal virtual OurMenuItem BtnSkinSave_WithEvents
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
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R35_1);
                RoutedEventHandler routedEventHandler2 = new RoutedEventHandler(this._Lambda___R35_2);
                OurMenuItem ourMenuItem = this._BtnSkinSave;
                if (ourMenuItem != null)
                {
                    ourMenuItem.Click -= routedEventHandler;
                    ourMenuItem.Checked -= routedEventHandler2;
                }
                this._BtnSkinSave = value;
                ourMenuItem = this._BtnSkinSave;
                if (ourMenuItem != null)
                {
                    ourMenuItem.Click += routedEventHandler;
                    ourMenuItem.Checked += routedEventHandler2;
                }
            }
        }

        internal virtual OurMenuItem BtnSkinRefresh_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSkinRefresh;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R39_3);
                OurMenuItem ourMenuItem = this._BtnSkinRefresh;
                if (ourMenuItem != null)
                {
                    ourMenuItem.Click -= routedEventHandler;
                }
                this._BtnSkinRefresh = value;
                ourMenuItem = this._BtnSkinRefresh;
                if (ourMenuItem != null)
                {
                    ourMenuItem.Click += routedEventHandler;
                }
            }
        }

        internal virtual OurMenuItem BtnSkinCape_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnSkinCape;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                RoutedEventHandler routedEventHandler = new RoutedEventHandler(this._Lambda___R43_4);
                OurMenuItem ourMenuItem = this._BtnSkinCape;
                if (ourMenuItem != null)
                {
                    ourMenuItem.Click -= routedEventHandler;
                }
                this._BtnSkinCape = value;
                ourMenuItem = this._BtnSkinCape;
                if (ourMenuItem != null)
                {
                    ourMenuItem.Click += routedEventHandler;
                }
            }
        }



        [CompilerGenerated]
        private void _Lambda___28_0()
        {
            checked
            {
                for (;;)
                {
                    try
                    {
                        OurSkin._Closure___28_0 CS_locals1 = new OurSkin._Closure___28_0(null);
                        if (ModLaunch.McLoginMsLoader.State != ModBase.LoadState.Finished)
                        {
                            ModLaunch.McLoginMsLoader.WaitForExit(PageLoginMsSkin.GetLoginData(), null, false, default(CancellationToken), null);
                        }
                        if (ModLaunch.McLoginMsLoader.State != ModBase.LoadState.Finished)
                        {
                            ModMain.Hint("登录失败，无法更改披风！", ModMain.HintType.Red, true);
                        }
                        else
                        {
                            string accessToken = ModLaunch.McLoginMsLoader.Output.AccessToken;
                            ModLaunch.McLoginResult output = ModLaunch.McLoginMsLoader.Output;
                            CS_locals1._VB_Local_SkinData = (JObject)ModLaunch.McLoginMsLoader.Output.ProfileJson.DeserializeJson();
                            CS_locals1._VB_Local_SelectedIndex = null;
                            ModBase.RunInUiWait(new Action(CS_locals1._Lambda___1));
                            if (CS_locals1._VB_Local_SelectedIndex != null)
                            {
                                string text = "https://api.minecraftservices.com/minecraft/profile/capes/active";
                                int? num = CS_locals1._VB_Local_SelectedIndex;
                                HttpMethod httpMethod = (((num != null) ? new bool?(num.GetValueOrDefault() == 0) : null).GetValueOrDefault() ? HttpMethod.Delete : HttpMethod.Put);
                                string text2 = null;
                                num = CS_locals1._VB_Local_SelectedIndex;
                                object obj = (((num != null) ? new bool?(num.GetValueOrDefault() == 0) : null).GetValueOrDefault() ? "" : new JObject(new JProperty("capeId", CS_locals1._VB_Local_SkinData["capes"][CS_locals1._VB_Local_SelectedIndex - 1]["id"])).ToString(Formatting.None, new JsonConverter[0]));
                                string text3 = "application/json";
                                string text4 = "*/*";
                                string[,] array = new string[1, 2];
                                array[0, 0] = "Authorization";
                                array[0, 1] = "Bearer " + accessToken;
                                string text5 = ModNet.NetRequestByClientRetry(text, httpMethod, text2, obj, text3, text4, array, false, null, false);
                                if (text5.Contains("\"errorMessage\""))
                                {
                                    ModMain.Hint("更改披风失败：" + text5.DeserializeJson()["errorMessage"].ToString(), ModMain.HintType.Red, true);
                                }
                                else
                                {
                                    ModMain.Hint("更改披风成功！", ModMain.HintType.Green, true);
                                    try
                                    {
                                        foreach (JToken jtoken in ((IEnumerable<JToken>)CS_locals1._VB_Local_SkinData["capes"]))
                                        {
                                            jtoken["state"] = "INACTIVE";
                                        }
                                    }
                                    finally
                                    {
                                        IEnumerator<JToken> enumerator = default;
                                        if (enumerator != null)
                                        {
                                            enumerator.Dispose();
                                        }
                                    }
                                    num = CS_locals1._VB_Local_SelectedIndex;
                                    if (((num != null) ? new bool?(num.GetValueOrDefault() > 0) : null).GetValueOrDefault())
                                    {
                                        CS_locals1._VB_Local_SkinData["capes"][CS_locals1._VB_Local_SelectedIndex - 1]["state"] = "ACTIVE";
                                    }
                                    ModLaunch.McLoginMsLoader.Output.ProfileJson = CS_locals1._VB_Local_SkinData.ToString();
                                    Settings.Set("CacheMsV2ProfileJson", ModLaunch.McLoginMsLoader.Output.ProfileJson, null);
                                }
                            }
                        }
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
                                    Logger.Warn(ex, "更改披风时遭遇 401 错误", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                                    ModMain.Hint("正在重新登录，将在登录后自动更改披风……", ModMain.HintType.Blue, true);
                                    ModLaunch.McLoginMsLoader.Start(PageLoginMsSkin.GetLoginData(), true);
                                    continue;
                                }
                            }
                            else
                            {
                                Logger.Warn(ex, "更改披风时遭遇 400 错误", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                                string response = ex2.Response;
                                if (response != null && response.Contains("\"error\""))
                                {
                                    ModMain.Hint("更改披风失败：" + ex2.Response.DeserializeJson()["error"].ToString(), ModMain.HintType.Red, true);
                                }
                                else
                                {
                                    string response2 = ex2.Response;
                                    if (response2 != null && response2.Contains("\"errorMessage\""))
                                    {
                                        ModMain.Hint("更改披风失败：" + ex2.Response.DeserializeJson()["errorMessage"].ToString(), ModMain.HintType.Red, true);
                                    }
                                }
                            }
                        }
                        else if (ex.IsBadNetwork())
                        {
                            ModMain.Hint("更改披风失败：连接 Mojang 服务器超时，请稍后再试，或使用 VPN 改善网络环境", ModMain.HintType.Red, true);
                        }
                        else
                        {
                            Logger.Error(ex, "更改披风失败", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                        }
                    }
                    finally
                    {
                        this.IsChanging = false;
                    }
                    break;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R35_1(object a0, RoutedEventArgs a1)
        {
            this.BtnSkinSave_Click();
        }

        [CompilerGenerated]
        private void _Lambda___R35_2(object a0, RoutedEventArgs a1)
        {
            this.BtnSkinSave_Checked((OurMenuItem)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R39_3(object a0, RoutedEventArgs a1)
        {
            this.RefreshClick();
        }

        [CompilerGenerated]
        private void _Lambda___R43_4(object a0, RoutedEventArgs a1)
        {
            this.BtnSkinCape_Click();
        }

        [CompilerGenerated]
        private OurSkin.ClickEventHandler ClickEvent;

        private string _Address;

        public ModLoader.LoaderTask<ValueTuple<string, string>, string> Loader;

        private bool IsSkinMouseDown;

        private bool IsChanging;

        [CompilerGenerated]
        [AccessedThroughProperty("ShadowSkin")]
        private DropShadowEffect _ShadowSkin;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSkinSave")]
        private OurMenuItem _BtnSkinSave;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSkinRefresh")]
        private OurMenuItem _BtnSkinRefresh;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnSkinCape")]
        private OurMenuItem _BtnSkinCape;

        [CompilerGenerated]
        [AccessedThroughProperty("ImgBack")]
        private Image _ImgBack;

        [CompilerGenerated]
        [AccessedThroughProperty("ImgFore")]
        private Image _ImgFore;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal bool _Lambda___28_2(JToken c)
            {
                JToken jtoken = c["state"];
                return Operators.CompareString((jtoken != null) ? jtoken.ToString() : null, "ACTIVE", false) == 0;
            }

            public static readonly OurSkin._Closure___ _I = new OurSkin._Closure___();

            public static Func<JToken, bool> _I28_2;
        }

        [CompilerGenerated]
        private sealed class _Closure___22_0
        {
            public _Closure___22_0(OurSkin._Closure___22_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_sender = arg0._VB_Local_sender;
                }
            }

            internal void _Lambda___0()
            {
                checked
                {
                    try
                    {
                        ModMain.Hint("正在刷新皮肤……", ModMain.HintType.Blue, true);
                        Logger.Info("正在清空皮肤缓存", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                        if (DirectoryUtils.Exists(ModBase.PathTemp + "Cache\\Skin"))
                        {
                            DirectoryUtils.Delete(ModBase.PathTemp + "Cache\\Skin", false);
                        }
                        if (DirectoryUtils.Exists(ModBase.PathTemp + "Cache\\Uuid"))
                        {
                            DirectoryUtils.Delete(ModBase.PathTemp + "Cache\\Uuid", false);
                        }
                        ModBase.IniClearCache(ModBase.PathTemp + "Cache\\Skin\\IndexMs.ini");
                        ModBase.IniClearCache(ModBase.PathTemp + "Cache\\Skin\\IndexNide.ini");
                        ModBase.IniClearCache(ModBase.PathTemp + "Cache\\Skin\\IndexAuth.ini");
                        ModBase.IniClearCache(ModBase.PathTemp + "Cache\\Uuid\\Mojang.ini");
                        ModLoader.LoaderTask<ValueTuple<string, string>, string>[] array2;
                        if (this._VB_Local_sender == null)
                        {
                            ModLoader.LoaderTask<ValueTuple<string, string>, string>[] array = new ModLoader.LoaderTask<ValueTuple<string, string>, string>[2];
                            array[0] = PageLaunchLeft.SkinLegacy;
                            array2 = array;
                            array[1] = PageLaunchLeft.SkinMs;
                        }
                        else
                        {
                            (array2 = new ModLoader.LoaderTask<ValueTuple<string, string>, string>[1])[0] = this._VB_Local_sender;
                        }
                        ModLoader.LoaderTask<ValueTuple<string, string>, string>[] array3 = array2;
                        for (int i = 0; i < array3.Length; i++)
                        {
                            array3[i].WaitForExit(null, null, true, default(CancellationToken), null);
                        }
                        ModMain.Hint("已刷新皮肤！", ModMain.HintType.Green, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "刷新皮肤缓存失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                    }
                }
            }

            public ModLoader.LoaderTask<ValueTuple<string, string>, string> _VB_Local_sender;
        }

        [CompilerGenerated]
        private sealed class _Closure___23_0
        {
            public _Closure___23_0(OurSkin._Closure___23_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_SkinAddress = arg0._VB_Local_SkinAddress;
                }
            }

            internal void _Lambda___0()
            {
                checked
                {
                    try
                    {
                        ModBase.WriteIni(ModBase.PathTemp + "Cache\\Skin\\IndexMs.ini", Settings.Get<string>("CacheMsV2Uuid", null), this._VB_Local_SkinAddress);
                        Logger.Info(string.Format("已写入皮肤地址缓存 {0} -> {1}", Settings.Get<string>("CacheMsV2Uuid", null), this._VB_Local_SkinAddress), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                        ModLoader.LoaderTask<ValueTuple<string, string>, string>[] array = new ModLoader.LoaderTask<ValueTuple<string, string>, string>[]
                        {
                            PageLaunchLeft.SkinMs,
                            PageLaunchLeft.SkinLegacy
                        };
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].WaitForExit(null, null, true, default(CancellationToken), null);
                        }
                        ModMain.Hint("更改皮肤成功！", ModMain.HintType.Green, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "更改正版皮肤后刷新皮肤失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                    }
                }
            }

            public string _VB_Local_SkinAddress;
        }

        [CompilerGenerated]
        private sealed class _Closure___28_0
        {
            public _Closure___28_0(OurSkin._Closure___28_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_SkinData = arg0._VB_Local_SkinData;
                    this._VB_Local_SelectedIndex = arg0._VB_Local_SelectedIndex;
                }
            }

            internal void _Lambda___1()
            {
                try
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>
                    {
                        { "Migrator", "迁移者披风" },
                        { "MapMaker", "Realms 地图制作者披风" },
                        { "Moderator", "Mojira 管理员披风" },
                        { "Translator-Chinese", "Crowdin 中文翻译者披风" },
                        { "Translator", "Crowdin 翻译者披风" },
                        { "Cobalt", "Cobalt 披风" },
                        { "Vanilla", "原版披风" },
                        { "Minecon2011", "Minecon 2011 参与者披风" },
                        { "Minecon2012", "Minecon 2012 参与者披风" },
                        { "Minecon2013", "Minecon 2013 参与者披风" },
                        { "Minecon2015", "Minecon 2015 参与者披风" },
                        { "Minecon2016", "Minecon 2016 参与者披风" },
                        { "Cherry Blossom", "樱花披风" },
                        { "15th Anniversary", "15 周年纪念披风" },
                        { "Purple Heart", "紫色心形披风" },
                        { "Follower's", "追随者披风" },
                        { "MCC 15th Year", "MCC 15 周年披风" },
                        { "Minecraft Experience", "村民救援披风" },
                        { "Mojang Office", "Mojang 办公室披风" },
                        { "Home", "家园披风" },
                        { "Menace", "入侵披风" },
                        { "Yearn", "渴望披风" },
                        { "Common", "普通披风" },
                        { "Pan", "薄煎饼披风" },
                        { "Founder's", "创始人披风" },
                        { "Copper", "铜披风" },
                        { "Zombie Horse", "僵尸马披风" },
                        { "Builder", "建造者披风" },
                        { "Crafter", "工匠披风" },
                        { "Aurora", "极光披风" }
                    };
                    List<IOurRadio> list = new List<IOurRadio>
                    {
                        new OurRadioBox
                        {
                            Text = "无披风",
                            Checked = !this._VB_Local_SkinData["capes"].Any<JToken>((OurSkin._Closure___._I28_2 == null) ? (OurSkin._Closure___._I28_2 = new Func<JToken, bool>(OurSkin._Closure___._I._Lambda___28_2)) : OurSkin._Closure___._I28_2)
                        }
                    };
                    try
                    {
                        foreach (JToken jtoken in ((IEnumerable<JToken>)this._VB_Local_SkinData["capes"]))
                        {
                            string text = jtoken["alias"].ToString();
                            if (dictionary.ContainsKey(text))
                            {
                                text = dictionary[text];
                            }
                            List<IOurRadio> list2 = list;
                            OurRadioBox ourRadioBox = new OurRadioBox();
                            ourRadioBox.Text = text;
                            OurRadioBox ourRadioBox2 = ourRadioBox;
                            JToken jtoken2 = jtoken["state"];
                            ourRadioBox2.Checked = Operators.CompareString((jtoken2 != null) ? jtoken2.ToString() : null, "ACTIVE", false) == 0;
                            list2.Add(ourRadioBox);
                        }
                    }
                    finally
                    {
                        IEnumerator<JToken> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    this._VB_Local_SelectedIndex = ModMain.OurMsgBoxSelect(list, "选择披风", "确定", "取消", false);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "获取玩家皮肤列表失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageLaunch\\OurSkin.xaml.vb");
                }
            }

            public JObject _VB_Local_SkinData;

            public int? _VB_Local_SelectedIndex;
        }
    }
}
